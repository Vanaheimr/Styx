/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Buffers;
using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Unicode;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A zero-copy reader for Concise Binary Object Representation (CBOR)
    /// data as defined in RFC 8949.
    /// The reader never allocates memory based on claimed lengths before
    /// verifying them against the remaining input, and optionally verifies
    /// the Core Deterministic Encoding Requirements on-the-fly.
    /// As a ref struct it can not be used across lambdas or await points;
    /// use the CBORValue document model in those contexts.
    /// </summary>
    public ref struct CBORReader
    {

        #region (private struct) Frame

        private struct Frame
        {

            /// <summary>
            /// Whether this frame is an array or a map.
            /// </summary>
            public CBORMajorType  Kind;

            /// <summary>
            /// The remaining number of data items within a definite-length
            /// container. Within maps keys and values are counted individually.
            /// </summary>
            public Int64          Remaining;

            /// <summary>
            /// Whether this container has an indefinite length.
            /// </summary>
            public Boolean        IsIndefinite;

            /// <summary>
            /// The number of tag heads read for the current, not yet completed child item.
            /// </summary>
            public Int32          PendingTags;

            /// <summary>
            /// The number of completed immediate child items.
            /// </summary>
            public Int64          CompletedItems;

            /// <summary>
            /// The input position where the current child item started.
            /// </summary>
            public Int32          CurrentItemStart;

            /// <summary>
            /// The input range of the previously completed map key
            /// (only used for deterministic order verification).
            /// </summary>
            public Int32          PrevKeyStart;
            public Int32          PrevKeyEnd;

        }

        #endregion

        #region Data

        private readonly  ReadOnlySpan<Byte>  data;
        private readonly  CBORReaderOptions   options;

        private           Int32               position;
        private           Frame[]?            frames;
        private           Int32               frameCount;
        private           Int32               rootItems;
        private           Int32               rootPendingTags;
        private           Int32               totalPendingTags;

        #endregion

        #region Properties

        /// <summary>
        /// The number of bytes not yet consumed.
        /// </summary>
        public readonly Int32  BytesRemaining
            => data.Length - position;

        /// <summary>
        /// The current nesting depth of open arrays and maps.
        /// </summary>
        public readonly Int32  CurrentDepth
            => frameCount;

        /// <summary>
        /// The current position within the CBOR data.
        /// </summary>
        public readonly Int32  Position
            => position;

        /// <summary>
        /// The CBOR reader options of this reader.
        /// </summary>
        public readonly CBORReaderOptions  Options
            => options;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CBOR reader for the given CBOR data.
        /// </summary>
        /// <param name="Data">The encoded CBOR data.</param>
        /// <param name="Options">Optional CBOR reader options.</param>
        public CBORReader(ReadOnlySpan<Byte>  Data,
                          CBORReaderOptions?  Options = null)
        {

            this.data     = Data;
            this.options  = Options ?? CBORReaderOptions.Default;

        }

        #endregion


        #region PeekState()

        /// <summary>
        /// Return the state of the next data item without consuming any input.
        /// </summary>
        public CBORReaderState PeekState()
        {

            if (frameCount > 0)
            {

                ref var frame = ref frames![frameCount - 1];

                if (!frame.IsIndefinite && frame.Remaining == 0)
                    return frame.Kind == CBORMajorType.Map
                               ? CBORReaderState.EndMap
                               : CBORReaderState.EndArray;

            }

            else if (rootItems >= 1)
                return CBORReaderState.Finished;

            var initialByte = PeekByteOrThrow();

            if (initialByte == 0xFF)
            {

                if (frameCount > 0 && frames![frameCount - 1].IsIndefinite)
                    return frames[frameCount - 1].Kind == CBORMajorType.Map
                               ? CBORReaderState.EndMap
                               : CBORReaderState.EndArray;

                throw new CBORException($"Unexpected CBOR 'break' stop code at position {position}!");

            }

            var majorType       = (CBORMajorType) (initialByte >> 5);
            var additionalInfo  = (Byte)          (initialByte & 0x1F);

            if (additionalInfo >= 28 && additionalInfo <= 30)
                throw new CBORException($"Reserved additional information '{additionalInfo}' at position {position}!");

            return majorType switch {

                CBORMajorType.UnsignedInteger  => CBORReaderState.UnsignedInteger,

                CBORMajorType.NegativeInteger  => CBORReaderState.NegativeInteger,

                CBORMajorType.ByteString       => additionalInfo == 31
                                                      ? CBORReaderState.StartIndefiniteLengthByteString
                                                      : CBORReaderState.ByteString,

                CBORMajorType.TextString       => additionalInfo == 31
                                                      ? CBORReaderState.StartIndefiniteLengthTextString
                                                      : CBORReaderState.TextString,

                CBORMajorType.Array            => CBORReaderState.StartArray,

                CBORMajorType.Map              => CBORReaderState.StartMap,

                CBORMajorType.Tag              => CBORReaderState.Tag,

                _                              => additionalInfo switch {
                                                      20 or 21  => CBORReaderState.Boolean,
                                                      22        => CBORReaderState.Null,
                                                      23        => CBORReaderState.Undefined,
                                                      25        => CBORReaderState.HalfPrecisionFloat,
                                                      26        => CBORReaderState.SinglePrecisionFloat,
                                                      27        => CBORReaderState.DoublePrecisionFloat,
                                                      _         => CBORReaderState.SimpleValue
                                                  }

            };

        }

        #endregion


        #region ReadUInt64()

        /// <summary>
        /// Read an unsigned integer (major type 0).
        /// </summary>
        public UInt64 ReadUInt64()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.UnsignedInteger)
                throw Mismatch("an unsigned integer", head);

            OnItemEnd();

            return head.Argument;

        }

        #endregion

        #region ReadInt64()

        /// <summary>
        /// Read a signed integer (major type 0 or 1).
        /// Values outside Int64 throw an OverflowException;
        /// use ReadInt128() for the full CBOR integer range.
        /// </summary>
        public Int64 ReadInt64()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major == CBORMajorType.UnsignedInteger)
            {

                if (head.Argument > Int64.MaxValue)
                    throw new OverflowException($"The unsigned integer '{head.Argument}' does not fit into an Int64!");

                OnItemEnd();

                return (Int64) head.Argument;

            }

            if (head.Major == CBORMajorType.NegativeInteger)
            {

                if (head.Argument > Int64.MaxValue)
                    throw new OverflowException($"The negative integer '-1-{head.Argument}' does not fit into an Int64!");

                OnItemEnd();

                return -1L - (Int64) head.Argument;

            }

            throw Mismatch("an integer", head);

        }

        #endregion

        #region ReadInt128()

        /// <summary>
        /// Read a signed integer (major type 0 or 1), covering the
        /// full CBOR integer range of -2^64 .. 2^64-1.
        /// </summary>
        public Int128 ReadInt128()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major == CBORMajorType.UnsignedInteger)
            {
                OnItemEnd();
                return head.Argument;
            }

            if (head.Major == CBORMajorType.NegativeInteger)
            {
                OnItemEnd();
                return -1 - (Int128) head.Argument;
            }

            throw Mismatch("an integer", head);

        }

        #endregion

        #region ReadBigInteger()

        /// <summary>
        /// Read an integer of any size: A plain integer (major type 0 or 1)
        /// or an unsigned/negative bignum (tag 2/3, RFC 8949, Section 3.4.3).
        /// </summary>
        public BigInteger ReadBigInteger()
        {

            switch (PeekState())
            {

                case CBORReaderState.UnsignedInteger:
                    return ReadUInt64();

                case CBORReaderState.NegativeInteger:
                    return (BigInteger) ReadInt128();

                case CBORReaderState.Tag:

                    var tag = ReadTag();

                    if (tag != CBORTag.UnsignedBignum &&
                        tag != CBORTag.NegativeBignum)
                    {
                        throw new CBORException($"Expected an unsigned or negative bignum (tag 2 or 3), but found tag {tag} at position {position}!");
                    }

                    return ReadBignumContent(tag);

                default:
                    throw new CBORException($"Expected an integer or bignum at position {position}!");

            }

        }

        #endregion

        #region ReadHalf()

        /// <summary>
        /// Read a half-precision floating-point number (0xf9).
        /// </summary>
        public Half ReadHalf()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.Simple || head.Info != 25)
                throw Mismatch("a half-precision floating-point number", head);

            CheckDeterministicFloat(head.Info, head.Argument);

            OnItemEnd();

            return BitConverter.UInt16BitsToHalf((UInt16) head.Argument);

        }

        #endregion

        #region ReadSingle()

        /// <summary>
        /// Read a single-precision floating-point number.
        /// Half-precision values are widened; double-precision values
        /// are rejected to prevent lossy narrowing.
        /// </summary>
        public Single ReadSingle()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.Simple || (head.Info != 25 && head.Info != 26))
                throw Mismatch("a half- or single-precision floating-point number", head);

            CheckDeterministicFloat(head.Info, head.Argument);

            OnItemEnd();

            return head.Info == 25
                       ? (Single) BitConverter.UInt16BitsToHalf((UInt16) head.Argument)
                       : BitConverter.UInt32BitsToSingle((UInt32) head.Argument);

        }

        #endregion

        #region ReadDouble()

        /// <summary>
        /// Read a floating-point number of any width.
        /// </summary>
        public Double ReadDouble()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.Simple || head.Info < 25 || head.Info > 27)
                throw Mismatch("a floating-point number", head);

            CheckDeterministicFloat(head.Info, head.Argument);

            OnItemEnd();

            return head.Info switch {
                       25  => (Double) BitConverter.UInt16BitsToHalf ((UInt16) head.Argument),
                       26  => (Double) BitConverter.UInt32BitsToSingle((UInt32) head.Argument),
                       _   =>          BitConverter.UInt64BitsToDouble(         head.Argument)
                   };

        }

        #endregion

        #region ReadDecimal()

        /// <summary>
        /// Read a decimal value: A plain integer (major type 0 or 1),
        /// a bignum (tag 2/3) or a decimal fraction (tag 4) with an
        /// integer or bignum mantissa.
        /// The value is reconstructed exactly; trailing zeros are only
        /// reduced when the decimal scale exceeds the maximum scale 28
        /// of System.Decimal. Values that can not be represented exactly
        /// throw an OverflowException.
        /// </summary>
        public Decimal ReadDecimal()
        {

            switch (PeekState())
            {

                case CBORReaderState.UnsignedInteger:
                    return ReadUInt64();

                case CBORReaderState.NegativeInteger:
                    return (Decimal) ReadInt128();

                case CBORReaderState.Tag:
                    break;

                default:
                    throw new CBORException($"Expected an integer or decimal fraction at position {position}!");

            }

            var tag = ReadTag();

            if (tag == CBORTag.UnsignedBignum ||
                tag == CBORTag.NegativeBignum)
            {
                return DecimalFromParts(ReadBignumContent(tag), 0);
            }

            if (tag != CBORTag.DecimalFraction)
                throw new CBORException($"Expected a decimal fraction (tag 4), but found tag {tag} at position {position}!");

            var count = ReadStartArray();

            if (count.HasValue && count.Value != 2)
                throw new CBORException($"A decimal fraction (tag 4) must be an array of two data items, but found {count} at position {position}!");

            var exponent  = ReadInt128();

            BigInteger mantissa;

            switch (PeekState())
            {

                case CBORReaderState.UnsignedInteger:
                    mantissa = ReadUInt64();
                    break;

                case CBORReaderState.NegativeInteger:
                    mantissa = (BigInteger) ReadInt128();
                    break;

                case CBORReaderState.Tag:

                    var mantissaTag = ReadTag();

                    if (mantissaTag != CBORTag.UnsignedBignum &&
                        mantissaTag != CBORTag.NegativeBignum)
                    {
                        throw new CBORException($"The mantissa of a decimal fraction must be an integer or bignum, but found tag {mantissaTag} at position {position}!");
                    }

                    mantissa = ReadBignumContent(mantissaTag);
                    break;

                default:
                    throw new CBORException($"The mantissa of a decimal fraction must be an integer or bignum (position {position})!");

            }

            ReadEndArray();

            return DecimalFromParts(mantissa, exponent);

        }

        #endregion

        #region ReadByteString()

        /// <summary>
        /// Read a byte string (major type 2).
        /// The chunks of indefinite-length byte strings are concatenated.
        /// </summary>
        public Byte[] ReadByteString()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.ByteString)
                throw Mismatch("a byte string", head);

            if (!head.IsIndefinite)
            {

                var length = CheckedLength(head.Argument);
                var result = data.Slice(position, length).ToArray();

                position += length;

                OnItemEnd();

                return result;

            }

            if (options.RequireDeterministic)
                throw new CBORException($"Indefinite-length byte strings are not allowed within deterministic CBOR encoding (position {position})!");

            var chunks  = ReadStringChunks(CBORMajorType.ByteString);
            var total   = 0;

            foreach (var chunk in chunks)
                total += chunk.Length;

            var bytes   = new Byte[total];
            var offset  = 0;

            foreach (var chunk in chunks)
            {
                data.Slice(chunk.Offset, chunk.Length).CopyTo(bytes.AsSpan(offset));
                offset += chunk.Length;
            }

            OnItemEnd();

            return bytes;

        }

        #endregion

        #region ReadTextString()

        /// <summary>
        /// Read a text string (major type 3).
        /// The chunks of indefinite-length text strings are concatenated;
        /// within strict UTF-8 validation every chunk must be valid UTF-8
        /// on its own (RFC 8949, Section 3.2.3).
        /// </summary>
        public String ReadTextString()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.TextString)
                throw Mismatch("a text string", head);

            if (!head.IsIndefinite)
            {

                var length  = CheckedLength(head.Argument);
                var slice   = data.Slice(position, length);

                if (options.UTF8Validation == CBORUTF8Validation.Strict && !Utf8.IsValid(slice))
                    throw new CBORException($"Invalid UTF-8 within the text string at position {position}!");

                var result  = Encoding.UTF8.GetString(slice);

                position += length;

                OnItemEnd();

                return result;

            }

            if (options.RequireDeterministic)
                throw new CBORException($"Indefinite-length text strings are not allowed within deterministic CBOR encoding (position {position})!");

            var chunks  = ReadStringChunks(CBORMajorType.TextString);
            var total   = 0;

            foreach (var chunk in chunks)
                total += chunk.Length;

            var bytes   = new Byte[total];
            var offset  = 0;

            foreach (var chunk in chunks)
            {
                data.Slice(chunk.Offset, chunk.Length).CopyTo(bytes.AsSpan(offset));
                offset += chunk.Length;
            }

            OnItemEnd();

            return Encoding.UTF8.GetString(bytes);

        }

        #endregion

        #region ReadStartArray()

        /// <summary>
        /// Start reading an array (major type 4) and return the number
        /// of data items, or null for an indefinite-length array.
        /// </summary>
        public Int32? ReadStartArray()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.Array)
                throw Mismatch("an array", head);

            EnsureDepth(1);

            if (head.IsIndefinite)
            {

                if (options.RequireDeterministic)
                    throw new CBORException($"Indefinite-length arrays are not allowed within deterministic CBOR encoding (position {position})!");

                PushFrame(CBORMajorType.Array, 0, IsIndefinite: true);

                return null;

            }

            if (head.Argument > (UInt64) (data.Length - position))
                throw new CBORException($"The claimed array length '{head.Argument}' exceeds the {data.Length - position} remaining byte(s) at position {position}!");

            PushFrame(CBORMajorType.Array, (Int64) head.Argument, IsIndefinite: false);

            return (Int32) head.Argument;

        }

        #endregion

        #region ReadEndArray()

        /// <summary>
        /// Finish reading the current array.
        /// </summary>
        public void ReadEndArray()
        {

            if (frameCount == 0 || frames![frameCount - 1].Kind != CBORMajorType.Array)
                throw new CBORException($"There is no open CBOR array to end at position {position}!");

            ref var frame = ref frames[frameCount - 1];

            if (frame.PendingTags > 0)
                throw new CBORException($"The last CBOR tag is not followed by any data item (position {position})!");

            if (frame.IsIndefinite)
            {

                if (PeekByteOrThrow() != 0xFF)
                    throw new CBORException($"Expected the 'break' stop code of the indefinite-length array at position {position}!");

                position++;

            }

            else if (frame.Remaining != 0)
                throw new CBORException($"The CBOR array still expects {frame.Remaining} more data item(s) at position {position}!");

            frameCount--;

            OnItemEnd();

        }

        #endregion

        #region ReadStartMap()

        /// <summary>
        /// Start reading a map (major type 5) and return the number
        /// of key/value pairs, or null for an indefinite-length map.
        /// </summary>
        public Int32? ReadStartMap()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.Map)
                throw Mismatch("a map", head);

            EnsureDepth(1);

            if (head.IsIndefinite)
            {

                if (options.RequireDeterministic)
                    throw new CBORException($"Indefinite-length maps are not allowed within deterministic CBOR encoding (position {position})!");

                PushFrame(CBORMajorType.Map, 0, IsIndefinite: true);

                return null;

            }

            if (head.Argument > (UInt64) (data.Length - position) / 2)
                throw new CBORException($"The claimed map length '{head.Argument}' exceeds the {data.Length - position} remaining byte(s) at position {position}!");

            PushFrame(CBORMajorType.Map, 2 * (Int64) head.Argument, IsIndefinite: false);

            return (Int32) head.Argument;

        }

        #endregion

        #region ReadEndMap()

        /// <summary>
        /// Finish reading the current map.
        /// </summary>
        public void ReadEndMap()
        {

            if (frameCount == 0 || frames![frameCount - 1].Kind != CBORMajorType.Map)
                throw new CBORException($"There is no open CBOR map to end at position {position}!");

            ref var frame = ref frames[frameCount - 1];

            if (frame.PendingTags > 0)
                throw new CBORException($"The last CBOR tag is not followed by any data item (position {position})!");

            if (frame.IsIndefinite)
            {

                if ((frame.CompletedItems & 1) == 1)
                    throw new CBORException($"The last CBOR map key is not followed by any value (position {position})!");

                if (PeekByteOrThrow() != 0xFF)
                    throw new CBORException($"Expected the 'break' stop code of the indefinite-length map at position {position}!");

                position++;

            }

            else if (frame.Remaining != 0)
                throw new CBORException($"The CBOR map still expects {frame.Remaining} more data item(s) at position {position}!");

            frameCount--;

            OnItemEnd();

        }

        #endregion

        #region PeekTag()

        /// <summary>
        /// Return the next CBOR tag without consuming any input.
        /// </summary>
        public CBORTag PeekTag()
        {

            var savedPosition = position;

            try
            {

                var head = ReadHeadInternal();

                if (head.Major != CBORMajorType.Tag)
                    throw Mismatch("a tag", head);

                return new CBORTag(head.Argument);

            }
            finally
            {
                position = savedPosition;
            }

        }

        #endregion

        #region ReadTag()

        /// <summary>
        /// Read a CBOR tag (major type 6). The tag applies to
        /// the next data item read.
        /// </summary>
        public CBORTag ReadTag()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.Tag)
                throw Mismatch("a tag", head);

            EnsureDepth(1);

            if (frameCount > 0)
                frames![frameCount - 1].PendingTags++;
            else
                rootPendingTags++;

            totalPendingTags++;

            return new CBORTag(head.Argument);

        }

        #endregion

        #region ReadSimpleValue()

        /// <summary>
        /// Read a CBOR simple value (major type 7).
        /// </summary>
        public CBORSimpleValue ReadSimpleValue()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.Simple || head.IsIndefinite || (head.Info >= 25 && head.Info <= 27))
                throw Mismatch("a simple value", head);

            OnItemEnd();

            return CBORSimpleValue.Parse((Byte) head.Argument);

        }

        #endregion

        #region ReadBoolean()

        /// <summary>
        /// Read a boolean value.
        /// </summary>
        public Boolean ReadBoolean()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.Simple || (head.Info != 20 && head.Info != 21))
                throw Mismatch("a boolean value", head);

            OnItemEnd();

            return head.Info == 21;

        }

        #endregion

        #region ReadNull()

        /// <summary>
        /// Read a CBOR null value.
        /// </summary>
        public void ReadNull()
        {

            OnItemBegin();

            var head = ReadHeadInternal();

            if (head.Major != CBORMajorType.Simple || head.Info != 22)
                throw Mismatch("null", head);

            OnItemEnd();

        }

        #endregion

        #region ReadDateTime()

        /// <summary>
        /// Read a date/time value: An RFC 3339 text string with tag 0,
        /// or epoch seconds (integer or floating-point) with tag 1.
        /// </summary>
        public DateTimeOffset ReadDateTime()
        {

            if (PeekState() != CBORReaderState.Tag)
                throw new CBORException($"Expected a tagged date/time at position {position}!");

            var tag = ReadTag();

            if (tag == CBORTag.DateTimeString)
            {

                var text = ReadTextString();

                if (!DateTimeOffset.TryParse(text,
                                             CultureInfo.InvariantCulture,
                                             DateTimeStyles.None,
                                             out var timestamp))
                {
                    throw new CBORException($"Invalid RFC 3339 date/time text '{text}'!");
                }

                return timestamp;

            }

            if (tag == CBORTag.EpochDateTime)
            {

                switch (PeekState())
                {

                    case CBORReaderState.UnsignedInteger:
                    case CBORReaderState.NegativeInteger:

                        try
                        {
                            return DateTimeOffset.FromUnixTimeSeconds(ReadInt64());
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            throw new CBORException("The epoch-based date/time is out of range!", e);
                        }

                    case CBORReaderState.HalfPrecisionFloat:
                    case CBORReaderState.SinglePrecisionFloat:
                    case CBORReaderState.DoublePrecisionFloat:

                        var seconds = ReadDouble();

                        if (Double.IsNaN(seconds) || Double.IsInfinity(seconds))
                            throw new CBORException($"Invalid epoch-based date/time '{seconds}'!");

                        var ticks = seconds * TimeSpan.TicksPerSecond;

                        if (ticks < (DateTimeOffset.MinValue - DateTimeOffset.UnixEpoch).Ticks ||
                            ticks > (DateTimeOffset.MaxValue - DateTimeOffset.UnixEpoch).Ticks)
                        {
                            throw new CBORException($"The epoch-based date/time '{seconds}' is out of range!");
                        }

                        return DateTimeOffset.UnixEpoch.AddTicks((Int64) Math.Round(ticks));

                    default:
                        throw new CBORException($"An epoch-based date/time must be an integer or floating-point number (position {position})!");

                }

            }

            throw new CBORException($"Expected a date/time tag (0 or 1), but found tag {tag}!");

        }

        #endregion

        #region SkipValue()

        /// <summary>
        /// Skip the next complete data item, including all tags,
        /// nested containers and indefinite-length chunks, while
        /// verifying its well-formedness.
        /// </summary>
        public void SkipValue()
        {

            OnItemBegin();

            Int64[]? rentedStack = null;

            // -1: indefinite array | -2: indefinite byte string | -3: indefinite text string
            // -4: indefinite map awaiting a key | -5: indefinite map awaiting a value
            var skipStack = options.MaxDepth <= 128
                                ? stackalloc Int64[128]
                                : (rentedStack = ArrayPool<Int64>.Shared.Rent(options.MaxDepth)).AsSpan();

            try
            {

                var depth   = 0;
                var tagRun  = 0;

                while (true)
                {

                    var head = ReadHeadInternal();

                    if (head.Major == CBORMajorType.Tag)
                    {
                        tagRun++;
                        EnsureDepth(depth + tagRun);
                        continue;
                    }

                    var completesItem = false;

                    if (head.Major == CBORMajorType.Simple && head.IsIndefinite)
                    {

                        // The 'break' stop code...
                        if (depth == 0 || skipStack[depth - 1] >= 0)
                            throw new CBORException($"Unexpected CBOR 'break' stop code at position {position}!");

                        if (tagRun > 0)
                            throw new CBORException($"The last CBOR tag is not followed by any data item (position {position})!");

                        if (skipStack[depth - 1] == -5)
                            throw new CBORException($"The last CBOR map key is not followed by any value (position {position})!");

                        depth--;
                        completesItem = true;

                    }

                    else if (depth > 0 && (skipStack[depth - 1] == -2 || skipStack[depth - 1] == -3))
                    {

                        // Within an indefinite-length string only definite-length
                        // chunks of the same major type are allowed...
                        var expectedMajor = skipStack[depth - 1] == -2
                                                ? CBORMajorType.ByteString
                                                : CBORMajorType.TextString;

                        if (tagRun > 0 || head.Major != expectedMajor || head.IsIndefinite)
                            throw new CBORException($"Indefinite-length strings may only contain definite-length chunks of the same type (position {position})!");

                        position += CheckedLength(head.Argument);

                        // Chunks do not count as data items!
                        continue;

                    }

                    else
                    {

                        switch (head.Major)
                        {

                            case CBORMajorType.UnsignedInteger:
                            case CBORMajorType.NegativeInteger:
                                completesItem = true;
                                break;

                            case CBORMajorType.ByteString:
                            case CBORMajorType.TextString:

                                if (head.IsIndefinite)
                                {

                                    if (options.RequireDeterministic)
                                        throw new CBORException($"Indefinite-length strings are not allowed within deterministic CBOR encoding (position {position})!");

                                    EnsureDepth(depth + tagRun + 1);
                                    skipStack[depth++] = head.Major == CBORMajorType.ByteString ? -2 : -3;

                                }
                                else
                                {
                                    position += CheckedLength(head.Argument);
                                    completesItem = true;
                                }

                                break;

                            case CBORMajorType.Array:
                            case CBORMajorType.Map:

                                if (head.IsIndefinite)
                                {

                                    if (options.RequireDeterministic)
                                        throw new CBORException($"Indefinite-length containers are not allowed within deterministic CBOR encoding (position {position})!");

                                    EnsureDepth(depth + tagRun + 1);
                                    skipStack[depth++] = head.Major == CBORMajorType.Map ? -4 : -1;

                                }

                                else
                                {

                                    var itemFactor = head.Major == CBORMajorType.Map ? 2UL : 1UL;

                                    if (head.Argument > (UInt64) (data.Length - position) / itemFactor)
                                        throw new CBORException($"The claimed container length '{head.Argument}' exceeds the {data.Length - position} remaining byte(s) at position {position}!");

                                    var items = (Int64) head.Argument * (Int64) itemFactor;

                                    if (items == 0)
                                        completesItem = true;

                                    else
                                    {
                                        EnsureDepth(depth + tagRun + 1);
                                        skipStack[depth++] = items;
                                    }

                                }

                                break;

                            default:  // Simple values and floating-point numbers
                                CheckDeterministicFloat(head.Info, head.Argument);
                                completesItem = true;
                                break;

                        }

                    }

                    if (!completesItem)
                        continue;

                    // A complete data item cascades upwards through
                    // all definite-length containers it completes...
                    tagRun = 0;

                    var finished = false;

                    while (true)
                    {

                        if (depth == 0)
                        {
                            finished = true;
                            break;
                        }

                        ref var top = ref skipStack[depth - 1];

                        if (top == -4 || top == -5)
                        {
                            top = top == -4 ? -5 : -4;
                            break;
                        }

                        if (top < 0)
                            break;

                        top--;

                        if (top > 0)
                            break;

                        depth--;

                    }

                    if (finished)
                        break;

                }

            }
            finally
            {
                if (rentedStack is not null)
                    ArrayPool<Int64>.Shared.Return(rentedStack);
            }

            OnItemEnd();

        }

        #endregion

        #region ReadEncodedValue()

        /// <summary>
        /// Read the next complete data item as its raw encoded bytes,
        /// verifying its well-formedness.
        /// </summary>
        public ReadOnlySpan<Byte> ReadEncodedValue()
        {

            var start = position;

            SkipValue();

            return data[start..position];

        }

        #endregion


        #region TryReadUInt64    (out Value)

        /// <summary>
        /// Try to read an unsigned integer.
        /// </summary>
        /// <param name="Value">The parsed unsigned integer.</param>
        public Boolean TryReadUInt64(out UInt64 Value)
        {

            var savedPosition = position;

            try
            {
                Value = ReadUInt64();
                return true;
            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            {
                position  = savedPosition;
                Value     = default;
                return false;
            }

        }

        #endregion

        #region TryReadInt64     (out Value)

        /// <summary>
        /// Try to read a signed integer.
        /// </summary>
        /// <param name="Value">The parsed signed integer.</param>
        public Boolean TryReadInt64(out Int64 Value)
        {

            var savedPosition = position;

            try
            {
                Value = ReadInt64();
                return true;
            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            {
                position  = savedPosition;
                Value     = default;
                return false;
            }

        }

        #endregion

        #region TryReadTextString(out Value)

        /// <summary>
        /// Try to read a text string.
        /// </summary>
        /// <param name="Value">The parsed text string.</param>
        public Boolean TryReadTextString([NotNullWhen(true)] out String? Value)
        {

            var savedPosition = position;

            try
            {
                Value = ReadTextString();
                return true;
            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            {
                position  = savedPosition;
                Value     = null;
                return false;
            }

        }

        #endregion

        #region TryReadByteString(out Value)

        /// <summary>
        /// Try to read a byte string.
        /// </summary>
        /// <param name="Value">The parsed byte string.</param>
        public Boolean TryReadByteString([NotNullWhen(true)] out Byte[]? Value)
        {

            var savedPosition = position;

            try
            {
                Value = ReadByteString();
                return true;
            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            {
                position  = savedPosition;
                Value     = null;
                return false;
            }

        }

        #endregion

        #region TryReadBoolean   (out Value)

        /// <summary>
        /// Try to read a boolean value.
        /// </summary>
        /// <param name="Value">The parsed boolean value.</param>
        public Boolean TryReadBoolean(out Boolean Value)
        {

            var savedPosition = position;

            try
            {
                Value = ReadBoolean();
                return true;
            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            {
                position  = savedPosition;
                Value     = default;
                return false;
            }

        }

        #endregion


        #region (private) OnItemBegin()

        private void OnItemBegin()
        {

            if (frameCount > 0)
            {

                ref var frame = ref frames![frameCount - 1];

                if (frame.PendingTags > 0)
                    return;

                if (!frame.IsIndefinite && frame.Remaining == 0)
                    throw new CBORException(frame.Kind == CBORMajorType.Map
                                                ? $"The CBOR map is already complete at position {position}!"
                                                : $"The CBOR array is already complete at position {position}!");

                frame.CurrentItemStart = position;

            }

            else
            {

                if (rootPendingTags > 0)
                    return;

                if (rootItems >= 1)
                    throw new CBORException($"The single top-level CBOR data item was already read (position {position})!");

            }

        }

        #endregion

        #region (private) OnItemEnd()

        private void OnItemEnd()
        {

            if (frameCount > 0)
            {

                ref var frame = ref frames![frameCount - 1];

                totalPendingTags  -= frame.PendingTags;
                frame.PendingTags  = 0;

                if (!frame.IsIndefinite)
                    frame.Remaining--;

                frame.CompletedItems++;

                if (options.RequireDeterministic &&
                    frame.Kind == CBORMajorType.Map &&
                    (frame.CompletedItems & 1) == 1)
                {

                    var key = data[frame.CurrentItemStart..position];

                    if (frame.CompletedItems > 1)
                    {

                        var previousKey  = data[frame.PrevKeyStart..frame.PrevKeyEnd];
                        var comparison   = previousKey.SequenceCompareTo(key);

                        if (comparison == 0)
                            throw new CBORException($"Duplicate CBOR map key at position {frame.CurrentItemStart}!");

                        if (comparison > 0)
                            throw new CBORException($"The CBOR map keys are not sorted in bytewise lexicographic order at position {frame.CurrentItemStart}!");

                    }

                    frame.PrevKeyStart  = frame.CurrentItemStart;
                    frame.PrevKeyEnd    = position;

                }

            }

            else
            {
                totalPendingTags -= rootPendingTags;
                rootPendingTags   = 0;
                rootItems++;
            }

        }

        #endregion

        #region (private) EnsureDepth(AdditionalDepth)

        private readonly void EnsureDepth(Int32 AdditionalDepth)
        {

            if (frameCount + totalPendingTags + AdditionalDepth > options.MaxDepth)
                throw new CBORException($"The maximum CBOR nesting depth of {options.MaxDepth} was exceeded at position {position}!");

        }

        #endregion

        #region (private) PushFrame(Kind, Remaining, IsIndefinite)

        private void PushFrame(CBORMajorType  Kind,
                               Int64          Remaining,
                               Boolean        IsIndefinite)
        {

            frames ??= new Frame[Math.Min(options.MaxDepth, 8)];

            if (frameCount == frames.Length)
                Array.Resize(ref frames,
                             Math.Min(Math.Max(frames.Length * 2, frameCount + 1), Math.Max(options.MaxDepth, frameCount + 1)));

            frames[frameCount++] = new Frame {
                                       Kind          = Kind,
                                       Remaining     = Remaining,
                                       IsIndefinite  = IsIndefinite
                                   };

        }

        #endregion

        #region (private) ReadHeadInternal()

        private (CBORMajorType Major, Byte Info, UInt64 Argument, Boolean IsIndefinite) ReadHeadInternal()
        {

            if (position >= data.Length)
                throw new CBORException($"Unexpected end of CBOR data at position {position}!");

            var initialByte  = data[position++];
            var majorType    = (CBORMajorType) (initialByte >> 5);
            var info         = (Byte)          (initialByte & 0x1F);

            UInt64 argument;

            switch (info)
            {

                case < 24:
                    return (majorType, info, info, false);

                case 24:

                    Need(1);
                    argument = data[position++];

                    if (majorType == CBORMajorType.Simple && argument < 32)
                        throw new CBORException($"The two-byte encoding of the CBOR simple value '{argument}' is reserved (position {position - 2})!");

                    if (options.RequireDeterministic && majorType != CBORMajorType.Simple && argument < 24)
                        throw new CBORException($"Non-shortest CBOR head at position {position - 2}!");

                    return (majorType, info, argument, false);

                case 25:

                    Need(2);
                    argument   = BinaryPrimitives.ReadUInt16BigEndian(data[position..]);
                    position  += 2;

                    if (options.RequireDeterministic && majorType != CBORMajorType.Simple && argument <= Byte.MaxValue)
                        throw new CBORException($"Non-shortest CBOR head at position {position - 3}!");

                    return (majorType, info, argument, false);

                case 26:

                    Need(4);
                    argument   = BinaryPrimitives.ReadUInt32BigEndian(data[position..]);
                    position  += 4;

                    if (options.RequireDeterministic && majorType != CBORMajorType.Simple && argument <= UInt16.MaxValue)
                        throw new CBORException($"Non-shortest CBOR head at position {position - 5}!");

                    return (majorType, info, argument, false);

                case 27:

                    Need(8);
                    argument   = BinaryPrimitives.ReadUInt64BigEndian(data[position..]);
                    position  += 8;

                    if (options.RequireDeterministic && majorType != CBORMajorType.Simple && argument <= UInt32.MaxValue)
                        throw new CBORException($"Non-shortest CBOR head at position {position - 9}!");

                    return (majorType, info, argument, false);

                case 28:
                case 29:
                case 30:
                    throw new CBORException($"Reserved additional information '{info}' at position {position - 1}!");

                default:  // 31

                    if (majorType == CBORMajorType.UnsignedInteger ||
                        majorType == CBORMajorType.NegativeInteger ||
                        majorType == CBORMajorType.Tag)
                    {
                        throw new CBORException($"Major type {(Byte) majorType} must not use an indefinite length (position {position - 1})!");
                    }

                    return (majorType, info, 0, true);

            }

        }

        #endregion

        #region (private) Need(NumberOfBytes)

        private readonly void Need(Int32 NumberOfBytes)
        {

            if (data.Length - position < NumberOfBytes)
                throw new CBORException($"Unexpected end of CBOR data at position {position}: {NumberOfBytes} more byte(s) expected!");

        }

        #endregion

        #region (private) CheckedLength(ClaimedLength)

        private readonly Int32 CheckedLength(UInt64 ClaimedLength)
        {

            if (ClaimedLength > (UInt64) (data.Length - position))
                throw new CBORException($"The claimed string length '{ClaimedLength}' exceeds the {data.Length - position} remaining byte(s) at position {position}!");

            return (Int32) ClaimedLength;

        }

        #endregion

        #region (private) PeekByteOrThrow()

        private readonly Byte PeekByteOrThrow()
        {

            if (position >= data.Length)
                throw new CBORException($"Unexpected end of CBOR data at position {position}!");

            return data[position];

        }

        #endregion

        #region (private) CheckDeterministicFloat(Info, Argument)

        private readonly void CheckDeterministicFloat(Byte    Info,
                                                      UInt64  Argument)
        {

            if (!options.RequireDeterministic)
                return;

            switch (Info)
            {

                case 25:

                    var halfBits = (UInt16) Argument;

                    if (Half.IsNaN(BitConverter.UInt16BitsToHalf(halfBits)) && halfBits != 0x7E00)
                        throw new CBORException($"NaN must be encoded as 0xf97e00 within deterministic CBOR encoding (position {position})!");

                    break;

                case 26:

                    var single = BitConverter.UInt32BitsToSingle((UInt32) Argument);

                    if (Single.IsNaN(single))
                        throw new CBORException($"NaN must be encoded as 0xf97e00 within deterministic CBOR encoding (position {position})!");

                    if (BitConverter.SingleToUInt32Bits((Single) (Half) single) == (UInt32) Argument)
                        throw new CBORException($"Non-shortest floating-point encoding within deterministic CBOR encoding (position {position})!");

                    break;

                case 27:

                    var value = BitConverter.UInt64BitsToDouble(Argument);

                    if (Double.IsNaN(value))
                        throw new CBORException($"NaN must be encoded as 0xf97e00 within deterministic CBOR encoding (position {position})!");

                    if (BitConverter.DoubleToUInt64Bits((Double) (Single) value) == Argument)
                        throw new CBORException($"Non-shortest floating-point encoding within deterministic CBOR encoding (position {position})!");

                    break;

            }

        }

        #endregion

        #region (private) ReadStringChunks(ExpectedMajorType)

        private List<(Int32 Offset, Int32 Length)> ReadStringChunks(CBORMajorType ExpectedMajorType)
        {

            var chunks = new List<(Int32 Offset, Int32 Length)>();

            while (true)
            {

                if (PeekByteOrThrow() == 0xFF)
                {
                    position++;
                    return chunks;
                }

                var chunkHead = ReadHeadInternal();

                if (chunkHead.Major != ExpectedMajorType || chunkHead.IsIndefinite)
                    throw new CBORException($"Indefinite-length strings may only contain definite-length chunks of the same type (position {position})!");

                var chunkLength = CheckedLength(chunkHead.Argument);

                if (ExpectedMajorType == CBORMajorType.TextString &&
                    options.UTF8Validation == CBORUTF8Validation.Strict &&
                    !Utf8.IsValid(data.Slice(position, chunkLength)))
                {
                    throw new CBORException($"Invalid UTF-8 within the text string chunk at position {position}: Every chunk must be valid UTF-8 on its own (RFC 8949, Section 3.2.3)!");
                }

                chunks.Add((position, chunkLength));

                position += chunkLength;

            }

        }

        #endregion

        #region (private) ReadBignumContent(Tag)

        private BigInteger ReadBignumContent(CBORTag Tag)
        {

            var start  = position;
            var bytes  = ReadByteString();
            var value  = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);

            var result = Tag == CBORTag.NegativeBignum
                             ? -value - 1
                             : value;

            if (options.RequirePreferredBignums)
                VerifyPreferredBignum(bytes, result, start);

            return result;

        }

        #endregion

        #region (internal static) VerifyPreferredBignum(Content, Value, Position)

        /// <summary>
        /// Verify the preferred serialization of a bignum (RFC 8949,
        /// Section 4.2.2): No leading zero bytes, and no value that a basic
        /// integer could have carried.
        ///
        /// Eight content bytes are what major types 0 and 1 reach, so a
        /// shorter byte string is always a second spelling of an integer that
        /// was already expressible - which is exactly what a deterministic
        /// encoding must not have two of.
        /// </summary>
        /// <param name="Content">The byte string a bignum tag wrapped.</param>
        /// <param name="Value">The integer it denotes.</param>
        /// <param name="Position">The input position of the byte string.</param>
        internal static void VerifyPreferredBignum(ReadOnlySpan<Byte>  Content,
                                                   BigInteger          Value,
                                                   Int32               Position)
        {

            if (Content.Length > 0 && Content[0] == 0)
                throw new CBORException($"A bignum must be written without leading zero bytes (position {Position})!");

            if (Content.Length <= 8)
                throw new CBORException($"The bignum {Value} fits into a basic integer and must be written as one (position {Position})!");

        }

        #endregion

        #region (internal static) DecimalFromParts(Mantissa, Exponent)

        internal static Decimal DecimalFromParts(BigInteger  Mantissa,
                                                 Int128      Exponent)
        {

            if (Mantissa.IsZero)
            {

                var zeroScale = Exponent >= 0
                                    ? (Byte) 0
                                    : (Byte) Int128.Min(28, -Exponent);

                return new Decimal(0, 0, 0, false, zeroScale);

            }

            // A conformance cap against maliciously large mantissas:
            // Any value representable as a System.Decimal fits into 12 bytes,
            // even with generous headroom for non-canonical trailing zeros.
            if (BigInteger.Abs(Mantissa).GetByteCount(isUnsigned: true) > 64)
                throw new OverflowException("The mantissa of the decimal fraction is too large for a System.Decimal!");

            if (Exponent > 28)
                throw new OverflowException("The decimal fraction is too large for a System.Decimal!");

            var mantissa  = Mantissa;
            var exponent  = Exponent;

            while (exponent < -28 && (mantissa % 10).IsZero)
            {
                mantissa /= 10;
                exponent++;
            }

            if (exponent < -28)
                throw new OverflowException("The decimal fraction has more than 28 decimal places and can not be represented as a System.Decimal without loss!");

            if (exponent > 0)
            {
                mantissa  *= BigInteger.Pow(10, (Int32) exponent);
                exponent   = 0;
            }

            var magnitude = BigInteger.Abs(mantissa);

            if (magnitude >= BigInteger.One << 96)
                throw new OverflowException("The decimal fraction does not fit into the 96 bit mantissa of a System.Decimal!");

            var bits = (UInt128) magnitude;

            return new Decimal((Int32) (UInt32)  bits,
                               (Int32) (UInt32) (bits >> 32),
                               (Int32) (UInt32) (bits >> 64),
                               mantissa.Sign < 0,
                               (Byte) (-exponent));

        }

        #endregion

        #region (private) Mismatch(Expected, Head)

        private readonly CBORException Mismatch(String Expected,
                                                (CBORMajorType Major, Byte Info, UInt64 Argument, Boolean IsIndefinite) Head)

            => new ($"Expected {Expected}, but found major type {(Byte) Head.Major} (additional information {Head.Info}) at position {position}!");

        #endregion

    }

}
