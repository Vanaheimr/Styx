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
using System.Numerics;
using System.Text.Unicode;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A streaming writer for Concise Binary Object Representation (CBOR)
    /// data as defined in RFC 8949.
    /// Integer heads are always encoded in their shortest form; floating-point
    /// values follow the configured CBORWriterOptions. In deterministic mode
    /// the writer enforces the Core Deterministic Encoding Requirements of
    /// RFC 8949, Section 4.2.1, including the bytewise lexicographic ordering
    /// of map keys.
    /// </summary>
    public class CBORWriter
    {

        #region (private class) Frame

        /// <summary>
        /// The bookkeeping data of an open CBOR array or map.
        /// </summary>
        private sealed class Frame(CBORMajorType  ContainerType,
                                   Int32?         DeclaredCount)
        {

            /// <summary>
            /// Whether this frame is an array or a map.
            /// </summary>
            public CBORMajorType             ContainerType    { get; } = ContainerType;

            /// <summary>
            /// The declared number of items (arrays) or pairs (maps),
            /// or null for indefinite-length containers.
            /// </summary>
            public Int32?                    DeclaredCount    { get; } = DeclaredCount;

            /// <summary>
            /// The number of completed immediate child items.
            /// Within maps keys and values are counted individually.
            /// </summary>
            public Int32                     WrittenItems     { get; set; }

            /// <summary>
            /// The number of tag heads written for the current,
            /// not yet completed child item.
            /// </summary>
            public Int32                     PendingTags      { get; set; }

            /// <summary>
            /// The scratch buffer collecting the encoded map entries
            /// for deterministic map key sorting.
            /// </summary>
            public ArrayBufferWriter<Byte>?  Scratch          { get; set; }

            /// <summary>
            /// The recorded (KeyStart, KeyEnd, ValueEnd) boundaries of all
            /// completed map entries within the scratch buffer.
            /// </summary>
            public List<(Int32 KeyStart, Int32 KeyEnd, Int32 ValueEnd)>?  Entries    { get; set; }

            /// <summary>
            /// The scratch buffer position after the most recently completed child item.
            /// </summary>
            public Int32                     LastBoundary     { get; set; }

            /// <summary>
            /// The scratch buffer start position of the current entry's key.
            /// </summary>
            public Int32                     PendingKeyStart  { get; set; }

            /// <summary>
            /// The scratch buffer end position of the current entry's key.
            /// </summary>
            public Int32                     PendingKeyEnd    { get; set; }

        }

        #endregion

        #region Data

        private readonly  IBufferWriter<Byte>              output;
        private readonly  ArrayBufferWriter<Byte>?         ownedOutput;
        private readonly  CBORWriterOptions                options;
        private readonly  List<Frame>                      frames           = [];
        private readonly  Stack<ArrayBufferWriter<Byte>>   scratchCache     = [];

        private           IBufferWriter<Byte>              sink;
        private           Int32                            rootItems;
        private           Int32                            rootPendingTags;

        #endregion

        #region Properties

        /// <summary>
        /// The current nesting depth of open arrays and maps.
        /// </summary>
        public Int32    CurrentDepth
            => frames.Count;

        /// <summary>
        /// Whether exactly one complete top-level data item has been written.
        /// </summary>
        public Boolean  IsComplete
            => frames.Count    == 0 &&
               rootItems       == 1 &&
               rootPendingTags == 0;

        /// <summary>
        /// The CBOR writer options of this writer.
        /// </summary>
        public CBORWriterOptions  Options
            => options;

        #endregion

        #region Constructor(s)

        #region CBORWriter(Options = null)

        /// <summary>
        /// Create a new CBOR writer using an internal buffer.
        /// </summary>
        /// <param name="Options">Optional CBOR writer options.</param>
        public CBORWriter(CBORWriterOptions? Options = null)
        {

            this.options      = Options ?? CBORWriterOptions.Default;
            this.ownedOutput  = new ArrayBufferWriter<Byte>();
            this.output       = this.ownedOutput;
            this.sink         = this.output;

        }

        #endregion

        #region CBORWriter(Output, Options = null)

        /// <summary>
        /// Create a new CBOR writer writing into the given buffer writer.
        /// </summary>
        /// <param name="Output">The buffer writer to write the encoded CBOR data to.</param>
        /// <param name="Options">Optional CBOR writer options.</param>
        public CBORWriter(IBufferWriter<Byte>  Output,
                          CBORWriterOptions?   Options = null)
        {

            this.options      = Options ?? CBORWriterOptions.Default;
            this.ownedOutput  = null;
            this.output       = Output;
            this.sink         = this.output;

        }

        #endregion

        #endregion


        #region WriteUInt64      (Value)

        /// <summary>
        /// Write the given unsigned integer (major type 0).
        /// </summary>
        /// <param name="Value">An unsigned integer.</param>
        public void WriteUInt64(UInt64 Value)
        {

            EnsureCanWriteItem();
            WriteHead(CBORMajorType.UnsignedInteger, Value);
            CompleteItem();

        }

        #endregion

        #region WriteInt64       (Value)

        /// <summary>
        /// Write the given signed integer (major type 0 or 1).
        /// </summary>
        /// <param name="Value">A signed integer.</param>
        public void WriteInt64(Int64 Value)
        {

            EnsureCanWriteItem();

            if (Value >= 0)
                WriteHead(CBORMajorType.UnsignedInteger, (UInt64) Value);
            else
                WriteHead(CBORMajorType.NegativeInteger, unchecked((UInt64) (-1L - Value)));

            CompleteItem();

        }

        #endregion

        #region WriteInt128      (Value)

        /// <summary>
        /// Write the given signed integer (major type 0 or 1), covering the
        /// full CBOR integer range of -2^64 .. 2^64-1.
        /// </summary>
        /// <param name="Value">A signed integer.</param>
        public void WriteInt128(Int128 Value)
        {

            if (Value >= 0)
            {

                if (Value > (Int128) UInt64.MaxValue)
                    throw new OverflowException($"The value '{Value}' is larger than the maximum CBOR integer 2^64-1!");

                EnsureCanWriteItem();
                WriteHead(CBORMajorType.UnsignedInteger, (UInt64) Value);
                CompleteItem();

            }
            else
            {

                var argument = (UInt128) (-(Value + 1));

                if (argument > UInt64.MaxValue)
                    throw new OverflowException($"The value '{Value}' is smaller than the minimum CBOR integer -2^64!");

                EnsureCanWriteItem();
                WriteHead(CBORMajorType.NegativeInteger, (UInt64) argument);
                CompleteItem();

            }

        }

        #endregion

        #region WriteBigInteger  (Value)

        /// <summary>
        /// Write the given big integer, either as a plain CBOR integer
        /// (major type 0 or 1) when it fits into -2^64 .. 2^64-1, or as
        /// an unsigned/negative bignum (tag 2/3, RFC 8949, Section 3.4.3).
        /// </summary>
        /// <param name="Value">A big integer.</param>
        public void WriteBigInteger(BigInteger Value)
        {

            if (Value >= 0)
            {

                if (Value <= UInt64.MaxValue)
                {
                    WriteUInt64((UInt64) Value);
                    return;
                }

                WriteTag       (CBORTag.UnsignedBignum);
                WriteByteString(Value.ToByteArray(isUnsigned: true, isBigEndian: true));

            }

            else
            {

                var argument = -Value - 1;

                if (argument <= UInt64.MaxValue)
                {
                    EnsureCanWriteItem();
                    WriteHead(CBORMajorType.NegativeInteger, (UInt64) argument);
                    CompleteItem();
                    return;
                }

                WriteTag       (CBORTag.NegativeBignum);
                WriteByteString(argument.ToByteArray(isUnsigned: true, isBigEndian: true));

            }

        }

        #endregion

        #region WriteDecimal     (Value)

        /// <summary>
        /// Write the given decimal as a decimal fraction
        /// (tag 4, RFC 8949, Section 3.4.4): An array of
        /// [-scale, mantissa], preserving the exact decimal scale,
        /// e.g. 1.10 is encoded differently than 1.1.
        /// The sign is folded into the mantissa, which becomes an
        /// unsigned/negative bignum (tag 2/3) whenever it exceeds 64 bit.
        /// </summary>
        /// <param name="Value">A decimal value.</param>
        public void WriteDecimal(Decimal Value)
        {

            Span<Int32> bits = stackalloc Int32[4];
            Decimal.GetBits(Value, bits);

            var scale       = (Byte) ((bits[3] >> 16) & 0xFF);
            var isNegative  = (bits[3] & unchecked((Int32) 0x80000000)) != 0;

            var magnitude   = ((UInt128) (UInt32) bits[2] << 64) |
                              ((UInt128) (UInt32) bits[1] << 32) |
                                         (UInt32) bits[0];

            WriteTag(CBORTag.DecimalFraction);
            WriteStartArray(2);
            WriteInt64(-scale);
            WriteMantissa(isNegative, magnitude);
            WriteEndArray();

        }

        #endregion

        #region WriteHalf        (Value)

        /// <summary>
        /// Write the given half-precision floating-point number (0xf9).
        /// In preferred or deterministic mode NaN values are
        /// canonicalized to 0xf97e00.
        /// </summary>
        /// <param name="Value">A half-precision floating-point number.</param>
        public void WriteHalf(Half Value)
        {

            EnsureCanWriteItem();

            if (Half.IsNaN(Value) && (options.PreferredFloatEncoding || options.Deterministic))
                WriteHalfBits(0x7E00);
            else
                WriteHalfBits(BitConverter.HalfToUInt16Bits(Value));

            CompleteItem();

        }

        #endregion

        #region WriteSingle      (Value)

        /// <summary>
        /// Write the given single-precision floating-point number.
        /// In preferred or deterministic mode the value is shrunk to a
        /// half-precision encoding whenever this preserves its exact value,
        /// and NaN values are canonicalized to 0xf97e00.
        /// </summary>
        /// <param name="Value">A single-precision floating-point number.</param>
        public void WriteSingle(Single Value)
        {

            EnsureCanWriteItem();
            WriteSingleInternal(Value);
            CompleteItem();

        }

        #endregion

        #region WriteDouble      (Value)

        /// <summary>
        /// Write the given double-precision floating-point number.
        /// In preferred or deterministic mode the value is shrunk to the
        /// shortest floating-point encoding that preserves its exact value,
        /// and NaN values are canonicalized to 0xf97e00.
        /// </summary>
        /// <param name="Value">A double-precision floating-point number.</param>
        public void WriteDouble(Double Value)
        {

            EnsureCanWriteItem();

            var preferred = options.PreferredFloatEncoding || options.Deterministic;

            if (Double.IsNaN(Value) && preferred)
                WriteHalfBits(0x7E00);

            else if (preferred &&
                     BitConverter.DoubleToUInt64Bits((Single) Value) == BitConverter.DoubleToUInt64Bits(Value))
                WriteSingleInternal((Single) Value);

            else
            {
                var span = sink.GetSpan(9);
                span[0] = 0xFB;
                BinaryPrimitives.WriteUInt64BigEndian(span[1..], BitConverter.DoubleToUInt64Bits(Value));
                sink.Advance(9);
            }

            CompleteItem();

        }

        #endregion

        #region WriteByteString  (Value)

        /// <summary>
        /// Write the given byte string (major type 2).
        /// </summary>
        /// <param name="Value">A byte string.</param>
        public void WriteByteString(ReadOnlySpan<Byte> Value)
        {

            EnsureCanWriteItem();
            WriteHead(CBORMajorType.ByteString, (UInt64) Value.Length);
            sink.Write(Value);
            CompleteItem();

        }

        #endregion

        #region WriteTextString  (Value)

        /// <summary>
        /// Write the given text string as UTF-8 (major type 3).
        /// </summary>
        /// <param name="Value">A text string.</param>
        public void WriteTextString(ReadOnlySpan<Char> Value)
        {

            EnsureCanWriteItem();

            var maxByteCount  = Value.Length * 3;
            var rentedBuffer  = maxByteCount > 768
                                    ? ArrayPool<Byte>.Shared.Rent(maxByteCount)
                                    : null;

            try
            {

                var utf8Buffer  = rentedBuffer is not null
                                      ? rentedBuffer.AsSpan()
                                      : stackalloc Byte[maxByteCount];

                var status      = Utf8.FromUtf16(Value,
                                                 utf8Buffer,
                                                 out _,
                                                 out var bytesWritten,
                                                 replaceInvalidSequences: false);

                if (status != OperationStatus.Done)
                    throw new CBORException("The given text contains unpaired surrogates and can not be encoded as valid UTF-8!");

                WriteHead(CBORMajorType.TextString, (UInt64) bytesWritten);
                sink.Write(utf8Buffer[..bytesWritten]);

            }
            finally
            {
                if (rentedBuffer is not null)
                    ArrayPool<Byte>.Shared.Return(rentedBuffer);
            }

            CompleteItem();

        }

        #endregion

        #region WriteStartArray  (Length)

        /// <summary>
        /// Start a new array (major type 4).
        /// </summary>
        /// <param name="Length">The number of items, or null for an indefinite-length array.</param>
        public void WriteStartArray(Int32? Length)
        {

            if (Length < 0)
                throw new ArgumentOutOfRangeException(nameof(Length), "The length of a CBOR array must not be negative!");

            EnsureCanWriteItem();
            EnsureDepth(1);

            if (Length.HasValue)
                WriteHead(CBORMajorType.Array, (UInt64) Length.Value);

            else
            {

                if (options.Deterministic)
                    throw new CBORException("Indefinite-length arrays are not allowed within deterministic CBOR encoding!");

                WriteRawByte(0x9F);

            }

            frames.Add(new Frame(CBORMajorType.Array, Length));

        }

        #endregion

        #region WriteStartMap    (Length)

        /// <summary>
        /// Start a new map (major type 5).
        /// </summary>
        /// <param name="Length">The number of key/value pairs, or null for an indefinite-length map.</param>
        public void WriteStartMap(Int32? Length)
        {

            if (Length < 0)
                throw new ArgumentOutOfRangeException(nameof(Length), "The length of a CBOR map must not be negative!");

            EnsureCanWriteItem();
            EnsureDepth(1);

            if (Length.HasValue)
                WriteHead(CBORMajorType.Map, (UInt64) Length.Value);

            else
            {

                if (options.Deterministic)
                    throw new CBORException("Indefinite-length maps are not allowed within deterministic CBOR encoding!");

                WriteRawByte(0xBF);

            }

            var frame = new Frame(CBORMajorType.Map, Length);

            if (options.Deterministic && Length > 0)
            {

                frame.Scratch  = scratchCache.TryPop(out var cachedScratch)
                                     ? cachedScratch
                                     : new ArrayBufferWriter<Byte>();

                frame.Entries  = [];

            }

            frames.Add(frame);

            if (frame.Scratch is not null)
                sink = frame.Scratch;

        }

        #endregion

        #region WriteEndArray()

        /// <summary>
        /// End the current array.
        /// </summary>
        public void WriteEndArray()
        {

            if (frames.Count == 0 || frames[^1].ContainerType != CBORMajorType.Array)
                throw new CBORException("There is no open CBOR array to end here!");

            var frame = frames[^1];

            if (frame.PendingTags > 0)
                throw new CBORException("The last CBOR tag is not followed by any data item!");

            if (frame.DeclaredCount.HasValue &&
                frame.WrittenItems != frame.DeclaredCount.Value)
            {
                throw new CBORException($"The CBOR array was declared to hold {frame.DeclaredCount} data item(s), but {frame.WrittenItems} were written!");
            }

            if (!frame.DeclaredCount.HasValue)
                WriteRawByte(0xFF);

            frames.RemoveAt(frames.Count - 1);
            RecomputeSink();

            CompleteItem();

        }

        #endregion

        #region WriteEndMap()

        /// <summary>
        /// End the current map. In deterministic mode the collected map
        /// entries are sorted in bytewise lexicographic order of their
        /// encoded keys and duplicate keys are rejected.
        /// </summary>
        public void WriteEndMap()
        {

            if (frames.Count == 0 || frames[^1].ContainerType != CBORMajorType.Map)
                throw new CBORException("There is no open CBOR map to end here!");

            var frame = frames[^1];

            if (frame.PendingTags > 0)
                throw new CBORException("The last CBOR tag is not followed by any data item!");

            if ((frame.WrittenItems & 1) == 1)
                throw new CBORException("The last CBOR map key is not followed by any value!");

            if (frame.DeclaredCount.HasValue &&
                frame.WrittenItems != 2 * frame.DeclaredCount.Value)
            {
                throw new CBORException($"The CBOR map was declared to hold {frame.DeclaredCount} key/value pair(s), but {frame.WrittenItems / 2} were written!");
            }

            if (!frame.DeclaredCount.HasValue)
                WriteRawByte(0xFF);

            frames.RemoveAt(frames.Count - 1);
            RecomputeSink();

            if (frame.Scratch is not null)
                FlushDeterministicMap(frame);

            CompleteItem();

        }

        #endregion

        #region WriteTag         (Tag)

        /// <summary>
        /// Write the given CBOR tag (major type 6). The tag applies
        /// to the next data item written.
        /// </summary>
        /// <param name="Tag">A CBOR tag.</param>
        public void WriteTag(CBORTag Tag)
        {

            EnsureCanWriteItem();
            EnsureDepth(1);

            WriteHead(CBORMajorType.Tag, Tag.Value);

            if (frames.Count > 0)
                frames[^1].PendingTags++;
            else
                rootPendingTags++;

        }

        #endregion

        #region WriteSimpleValue (Value)

        /// <summary>
        /// Write the given CBOR simple value (major type 7).
        /// </summary>
        /// <param name="Value">A CBOR simple value.</param>
        public void WriteSimpleValue(CBORSimpleValue Value)
        {

            EnsureCanWriteItem();

            if (Value.Value < 24)
                WriteRawByte((Byte) (0xE0 | Value.Value));

            else
            {
                var span = sink.GetSpan(2);
                span[0] = 0xF8;
                span[1] = Value.Value;
                sink.Advance(2);
            }

            CompleteItem();

        }

        #endregion

        #region WriteBoolean     (Value)

        /// <summary>
        /// Write the given boolean value.
        /// </summary>
        /// <param name="Value">A boolean value.</param>
        public void WriteBoolean(Boolean Value)

            => WriteSimpleValue(Value
                                    ? CBORSimpleValue.True
                                    : CBORSimpleValue.False);

        #endregion

        #region WriteNull()

        /// <summary>
        /// Write a CBOR null value.
        /// </summary>
        public void WriteNull()

            => WriteSimpleValue(CBORSimpleValue.Null);

        #endregion

        #region WriteUndefined()

        /// <summary>
        /// Write a CBOR undefined value.
        /// </summary>
        public void WriteUndefined()

            => WriteSimpleValue(CBORSimpleValue.Undefined);

        #endregion

        #region WriteDateTime    (Value, Format = TextString)

        /// <summary>
        /// Write the given date/time, either as an RFC 3339 text string
        /// with tag 0, or as epoch seconds with tag 1.
        /// </summary>
        /// <param name="Value">A date/time value.</param>
        /// <param name="Format">The CBOR date/time format to use.</param>
        public void WriteDateTime(DateTimeOffset      Value,
                                  CBORDateTimeFormat  Format = CBORDateTimeFormat.TextString)
        {

            switch (Format)
            {

                case CBORDateTimeFormat.TextString:
                    WriteTag       (CBORTag.DateTimeString);
                    WriteTextString(Value.ToISO8601());
                    break;

                case CBORDateTimeFormat.EpochSeconds:
                    WriteTag  (CBORTag.EpochDateTime);
                    WriteInt64(Value.ToUnixTimestamp());
                    break;

                default:
                    throw new ArgumentException($"Unknown CBOR date/time format '{Format}'!", nameof(Format));

            }

        }

        #endregion


        #region ToByteArray()

        /// <summary>
        /// Return the encoded CBOR data.
        /// Only available when the writer was created with an internal buffer.
        /// </summary>
        public Byte[] ToByteArray()
        {

            if (ownedOutput is null)
                throw new InvalidOperationException("ToByteArray() is not available when the CBOR writer was created over an external buffer writer!");

            if (frames.Count > 0)
                throw new CBORException($"The encoded CBOR data is not yet complete: {frames.Count} container(s) are still open!");

            if (rootPendingTags > 0)
                throw new CBORException("The last CBOR tag is not followed by any data item!");

            if (rootItems == 0)
                throw new CBORException("No CBOR data item was written!");

            return ownedOutput.WrittenSpan.ToArray();

        }

        #endregion

        #region Reset()

        /// <summary>
        /// Reset this CBOR writer for writing a new CBOR data item.
        /// Only available when the writer was created with an internal buffer.
        /// </summary>
        public void Reset()
        {

            if (ownedOutput is null)
                throw new InvalidOperationException("Reset() is not available when the CBOR writer was created over an external buffer writer!");

            ownedOutput.ResetWrittenCount();
            frames.Clear();
            sink             = output;
            rootItems        = 0;
            rootPendingTags  = 0;

        }

        #endregion


        #region (private) EnsureCanWriteItem()

        private void EnsureCanWriteItem()
        {

            if (frames.Count == 0)
            {

                if (rootItems >= 1)
                    throw new CBORException("A CBOR message must contain a single top-level data item!");

                return;

            }

            var frame = frames[^1];

            if (frame.DeclaredCount.HasValue)
            {

                var maxItems = frame.ContainerType == CBORMajorType.Map
                                   ? 2 * frame.DeclaredCount.Value
                                   :     frame.DeclaredCount.Value;

                if (frame.WrittenItems >= maxItems)
                    throw new CBORException(frame.ContainerType == CBORMajorType.Map
                                                ? $"The CBOR map already holds all of its {frame.DeclaredCount} declared key/value pair(s)!"
                                                : $"The CBOR array already holds all of its {frame.DeclaredCount} declared data item(s)!");

            }

        }

        #endregion

        #region (private) EnsureDepth(AdditionalDepth)

        private void EnsureDepth(Int32 AdditionalDepth)
        {

            var pendingTags = frames.Count > 0
                                  ? frames[^1].PendingTags
                                  : rootPendingTags;

            if (frames.Count + pendingTags + AdditionalDepth > options.MaxDepth)
                throw new CBORException($"The maximum CBOR nesting depth of {options.MaxDepth} was exceeded!");

        }

        #endregion

        #region (private) CompleteItem()

        private void CompleteItem()
        {

            if (frames.Count == 0)
            {
                rootPendingTags = 0;
                rootItems++;
                return;
            }

            var frame = frames[^1];

            frame.PendingTags = 0;
            frame.WrittenItems++;

            if (frame.Entries is not null)
            {

                var position = frame.Scratch!.WrittenCount;

                if ((frame.WrittenItems & 1) == 1)
                {
                    frame.PendingKeyStart  = frame.LastBoundary;
                    frame.PendingKeyEnd    = position;
                }
                else
                    frame.Entries.Add((frame.PendingKeyStart, frame.PendingKeyEnd, position));

                frame.LastBoundary = position;

            }

        }

        #endregion

        #region (private) FlushDeterministicMap(Frame)

        private void FlushDeterministicMap(Frame Frame)
        {

            var scratch  = Frame.Scratch!;
            var entries  = Frame.Entries!;

            entries.Sort((entry1, entry2)
                => scratch.WrittenSpan[entry1.KeyStart..entry1.KeyEnd].
                       SequenceCompareTo(
                   scratch.WrittenSpan[entry2.KeyStart..entry2.KeyEnd]));

            for (var i = 1; i < entries.Count; i++)
            {

                if (scratch.WrittenSpan[entries[i - 1].KeyStart..entries[i - 1].KeyEnd].
                        SequenceEqual(
                    scratch.WrittenSpan[entries[i].KeyStart..entries[i].KeyEnd]))
                {
                    throw new CBORException($"Duplicate CBOR map key '{Convert.ToHexString(scratch.WrittenSpan[entries[i].KeyStart..entries[i].KeyEnd])}' within deterministic encoding!");
                }

            }

            foreach (var entry in entries)
                sink.Write(scratch.WrittenSpan[entry.KeyStart..entry.ValueEnd]);

            scratch.ResetWrittenCount();
            scratchCache.Push(scratch);

        }

        #endregion

        #region (private) RecomputeSink()

        private void RecomputeSink()
        {

            for (var i = frames.Count - 1; i >= 0; i--)
            {
                if (frames[i].Scratch is not null)
                {
                    sink = frames[i].Scratch!;
                    return;
                }
            }

            sink = output;

        }

        #endregion

        #region (private) WriteHead(MajorType, Argument)

        private void WriteHead(CBORMajorType  MajorType,
                               UInt64         Argument)
        {

            var span       = sink.GetSpan(9);
            var majorBits  = (Byte) ((Byte) MajorType << 5);

            if (Argument < 24)
            {
                span[0] = (Byte) (majorBits | Argument);
                sink.Advance(1);
            }

            else if (Argument <= Byte.MaxValue)
            {
                span[0] = (Byte) (majorBits | 24);
                span[1] = (Byte) Argument;
                sink.Advance(2);
            }

            else if (Argument <= UInt16.MaxValue)
            {
                span[0] = (Byte) (majorBits | 25);
                BinaryPrimitives.WriteUInt16BigEndian(span[1..], (UInt16) Argument);
                sink.Advance(3);
            }

            else if (Argument <= UInt32.MaxValue)
            {
                span[0] = (Byte) (majorBits | 26);
                BinaryPrimitives.WriteUInt32BigEndian(span[1..], (UInt32) Argument);
                sink.Advance(5);
            }

            else
            {
                span[0] = (Byte) (majorBits | 27);
                BinaryPrimitives.WriteUInt64BigEndian(span[1..], Argument);
                sink.Advance(9);
            }

        }

        #endregion

        #region (private) WriteRawByte(Value)

        private void WriteRawByte(Byte Value)
        {

            var span = sink.GetSpan(1);
            span[0] = Value;
            sink.Advance(1);

        }

        #endregion

        #region (private) WriteHalfBits(Bits)

        private void WriteHalfBits(UInt16 Bits)
        {

            var span = sink.GetSpan(3);
            span[0] = 0xF9;
            BinaryPrimitives.WriteUInt16BigEndian(span[1..], Bits);
            sink.Advance(3);

        }

        #endregion

        #region (private) WriteSingleInternal(Value)

        private void WriteSingleInternal(Single Value)
        {

            var preferred = options.PreferredFloatEncoding || options.Deterministic;

            if (Single.IsNaN(Value) && preferred)
            {
                WriteHalfBits(0x7E00);
                return;
            }

            if (preferred)
            {

                var half = (Half) Value;

                if (BitConverter.SingleToUInt32Bits((Single) half) == BitConverter.SingleToUInt32Bits(Value))
                {
                    WriteHalfBits(BitConverter.HalfToUInt16Bits(half));
                    return;
                }

            }

            var span = sink.GetSpan(5);
            span[0] = 0xFA;
            BinaryPrimitives.WriteUInt32BigEndian(span[1..], BitConverter.SingleToUInt32Bits(Value));
            sink.Advance(5);

        }

        #endregion

        #region (private) WriteMantissa(IsNegative, Magnitude)

        private void WriteMantissa(Boolean  IsNegative,
                                   UInt128  Magnitude)
        {

            if (!IsNegative || Magnitude == 0)
            {

                if (Magnitude <= UInt64.MaxValue)
                {
                    WriteUInt64((UInt64) Magnitude);
                    return;
                }

                WriteTag       (CBORTag.UnsignedBignum);
                WriteByteString(ToBigEndianTrimmed(Magnitude));

            }

            else
            {

                var argument = Magnitude - 1;

                if (argument <= UInt64.MaxValue)
                {
                    EnsureCanWriteItem();
                    WriteHead(CBORMajorType.NegativeInteger, (UInt64) argument);
                    CompleteItem();
                    return;
                }

                WriteTag       (CBORTag.NegativeBignum);
                WriteByteString(ToBigEndianTrimmed(argument));

            }

        }

        #endregion

        #region (private static) ToBigEndianTrimmed(Value)

        private static Byte[] ToBigEndianTrimmed(UInt128 Value)
        {

            Span<Byte> buffer = stackalloc Byte[16];
            BinaryPrimitives.WriteUInt128BigEndian(buffer, Value);

            var start = 0;
            while (start < 15 && buffer[start] == 0)
                start++;

            return buffer[start..].ToArray();

        }

        #endregion

    }

}
