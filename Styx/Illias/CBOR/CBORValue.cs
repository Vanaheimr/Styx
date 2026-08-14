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

using System.Buffers.Binary;
using System.Numerics;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An immutable CBOR value within the CBOR document model:
    /// A memory-efficient tagged union that allocates nothing for scalars.
    /// Non-negative integers are normalized to the UnsignedInteger kind,
    /// therefore map keys are found independently of how they were created.
    /// Equality is representational: Two values are equal when they have
    /// the same kind and content; floating-point values are compared by
    /// their bits, so e.g. a half-precision 1.0 is not equal to a
    /// double-precision 1.0, and identical NaNs are equal.
    /// </summary>
    public readonly struct CBORValue : IEquatable<CBORValue>
    {

        #region (private class) CBORTagBox

        /// <summary>
        /// The boxed inner value of a tagged CBOR value.
        /// </summary>
        private sealed class CBORTagBox(CBORValue Inner)
        {
            public CBORValue Inner { get; } = Inner;
        }

        #endregion

        #region Data

        private readonly CBORValueKind  kind;
        private readonly UInt64         numeric;
        private readonly Object?        reference;

        #endregion

        #region Properties

        /// <summary>
        /// The kind of this CBOR value.
        /// </summary>
        public CBORValueKind  Kind
            => kind;

        /// <summary>
        /// The number of data items within an array,
        /// or the number of key/value pairs within a map.
        /// </summary>
        public Int32  Count
            => kind switch {
                   CBORValueKind.Array  => ((CBORValue[]) reference!).Length,
                   CBORValueKind.Map    => ((KeyValuePair<CBORValue, CBORValue>[]) reference!).Length,
                   _                    => throw new CBORException($"A CBOR {kind} has no count!")
               };

        /// <summary>
        /// The tag of a tagged CBOR value.
        /// </summary>
        public CBORTag  Tag
            => kind == CBORValueKind.Tagged
                   ? new CBORTag(numeric)
                   : throw new CBORException($"A CBOR {kind} has no tag!");

        /// <summary>
        /// The inner value of a tagged CBOR value,
        /// with exactly one tag layer removed.
        /// </summary>
        public CBORValue  UntaggedValue
            => kind == CBORValueKind.Tagged
                   ? ((CBORTagBox) reference!).Inner
                   : throw new CBORException($"A CBOR {kind} has no inner value!");

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CBOR value.
        /// </summary>
        private CBORValue(CBORValueKind  Kind,
                          UInt64         Numeric,
                          Object?        Reference)
        {

            this.kind       = Kind;
            this.numeric    = Numeric;
            this.reference  = Reference;

        }

        #endregion


        #region Static defaults

        /// <summary>
        /// The CBOR null value.
        /// </summary>
        public static CBORValue  Null         { get; } = new (CBORValueKind.Null,      0, null);

        /// <summary>
        /// The CBOR undefined value.
        /// </summary>
        public static CBORValue  Undefined    { get; } = new (CBORValueKind.Undefined, 0, null);

        /// <summary>
        /// The boolean value true.
        /// </summary>
        public static CBORValue  True         { get; } = new (CBORValueKind.Boolean,   1, null);

        /// <summary>
        /// The boolean value false.
        /// </summary>
        public static CBORValue  False        { get; } = new (CBORValueKind.Boolean,   0, null);

        #endregion


        #region (static) FromUInt64      (Value)

        /// <summary>
        /// Create a CBOR value from the given unsigned integer.
        /// </summary>
        /// <param name="Value">An unsigned integer.</param>
        public static CBORValue FromUInt64(UInt64 Value)

            => new (CBORValueKind.UnsignedInteger,
                    Value,
                    null);

        #endregion

        #region (static) FromInt64       (Value)

        /// <summary>
        /// Create a CBOR value from the given signed integer.
        /// </summary>
        /// <param name="Value">A signed integer.</param>
        public static CBORValue FromInt64(Int64 Value)

            => Value >= 0

                   ? new (CBORValueKind.UnsignedInteger,
                          (UInt64) Value,
                          null)

                   : new (CBORValueKind.NegativeInteger,
                          unchecked((UInt64) (-1L - Value)),
                          null);

        #endregion

        #region (static) FromInt128      (Value)

        /// <summary>
        /// Create a CBOR value from the given signed integer,
        /// covering the full CBOR integer range of -2^64 .. 2^64-1.
        /// </summary>
        /// <param name="Value">A signed integer.</param>
        public static CBORValue FromInt128(Int128 Value)
        {

            if (Value >= 0)
            {

                if (Value > (Int128) UInt64.MaxValue)
                    throw new OverflowException($"The value '{Value}' is larger than the maximum CBOR integer 2^64-1!");

                return new (CBORValueKind.UnsignedInteger,
                            (UInt64) Value,
                            null);

            }

            var argument = (UInt128) (-(Value + 1));

            if (argument > UInt64.MaxValue)
                throw new OverflowException($"The value '{Value}' is smaller than the minimum CBOR integer -2^64!");

            return new (CBORValueKind.NegativeInteger,
                        (UInt64) argument,
                        null);

        }

        #endregion

        #region (static) FromBigInteger  (Value)

        /// <summary>
        /// Create a CBOR value from the given big integer:
        /// A plain integer when it fits into -2^64 .. 2^64-1,
        /// otherwise an unsigned/negative bignum (tag 2/3).
        /// </summary>
        /// <param name="Value">A big integer.</param>
        public static CBORValue FromBigInteger(BigInteger Value)
        {

            if (Value >= 0)
                return Value <= UInt64.MaxValue
                           ? FromUInt64((UInt64) Value)
                           : Tagged(CBORTag.UnsignedBignum,
                                    FromBytes(Value.ToByteArray(isUnsigned: true, isBigEndian: true)));

            var argument = -Value - 1;

            return argument <= UInt64.MaxValue
                       ? new (CBORValueKind.NegativeInteger,
                              (UInt64) argument,
                              null)
                       : Tagged(CBORTag.NegativeBignum,
                                FromBytes(argument.ToByteArray(isUnsigned: true, isBigEndian: true)));

        }

        #endregion

        #region (static) FromDecimal     (Value)

        /// <summary>
        /// Create a CBOR value from the given decimal as a decimal fraction
        /// (tag 4): An array of [-scale, mantissa], preserving the exact
        /// decimal scale, e.g. 1.10 is represented differently than 1.1.
        /// </summary>
        /// <param name="Value">A decimal value.</param>
        public static CBORValue FromDecimal(Decimal Value)
        {

            Span<Int32> bits = stackalloc Int32[4];
            Decimal.GetBits(Value, bits);

            var scale       = (Byte) ((bits[3] >> 16) & 0xFF);
            var isNegative  = (bits[3] & unchecked((Int32) 0x80000000)) != 0;

            var magnitude   = ((UInt128) (UInt32) bits[2] << 64) |
                              ((UInt128) (UInt32) bits[1] << 32) |
                                         (UInt32) bits[0];

            CBORValue mantissa;

            if (!isNegative || magnitude == 0)
                mantissa = magnitude <= UInt64.MaxValue
                               ? FromUInt64((UInt64) magnitude)
                               : Tagged(CBORTag.UnsignedBignum,
                                        FromBytes(ToBigEndianTrimmed(magnitude)));

            else
            {

                var argument = magnitude - 1;

                mantissa = argument <= UInt64.MaxValue
                               ? new (CBORValueKind.NegativeInteger,
                                      (UInt64) argument,
                                      null)
                               : Tagged(CBORTag.NegativeBignum,
                                        FromBytes(ToBigEndianTrimmed(argument)));

            }

            return Tagged(CBORTag.DecimalFraction,
                          FromArray(FromInt64(-scale), mantissa));

        }

        #endregion

        #region (static) FromHalf        (Value)

        /// <summary>
        /// Create a CBOR value from the given half-precision floating-point number.
        /// </summary>
        /// <param name="Value">A half-precision floating-point number.</param>
        public static CBORValue FromHalf(Half Value)

            => new (CBORValueKind.HalfFloat,
                    BitConverter.HalfToUInt16Bits(Value),
                    null);

        #endregion

        #region (static) FromSingle      (Value)

        /// <summary>
        /// Create a CBOR value from the given single-precision floating-point number.
        /// </summary>
        /// <param name="Value">A single-precision floating-point number.</param>
        public static CBORValue FromSingle(Single Value)

            => new (CBORValueKind.SingleFloat,
                    BitConverter.SingleToUInt32Bits(Value),
                    null);

        #endregion

        #region (static) FromDouble      (Value)

        /// <summary>
        /// Create a CBOR value from the given double-precision floating-point number.
        /// </summary>
        /// <param name="Value">A double-precision floating-point number.</param>
        public static CBORValue FromDouble(Double Value)

            => new (CBORValueKind.DoubleFloat,
                    BitConverter.DoubleToUInt64Bits(Value),
                    null);

        #endregion

        #region (static) FromBoolean     (Value)

        /// <summary>
        /// Create a CBOR value from the given boolean value.
        /// </summary>
        /// <param name="Value">A boolean value.</param>
        public static CBORValue FromBoolean(Boolean Value)

            => Value
                   ? True
                   : False;

        #endregion

        #region (static) FromText        (Value)

        /// <summary>
        /// Create a CBOR value from the given text.
        /// </summary>
        /// <param name="Value">A text.</param>
        public static CBORValue FromText(String Value)

            => new (CBORValueKind.TextString,
                    0,
                    Value ?? throw new ArgumentNullException(nameof(Value)));

        #endregion

        #region (static) FromBytes       (Value)

        /// <summary>
        /// Create a CBOR value from the given byte array.
        /// </summary>
        /// <param name="Value">A byte array.</param>
        public static CBORValue FromBytes(Byte[] Value)

            => new (CBORValueKind.ByteString,
                    0,
                    Value ?? throw new ArgumentNullException(nameof(Value)));

        #endregion

        #region (static) FromSimpleValue (Value)

        /// <summary>
        /// Create a CBOR value from the given CBOR simple value.
        /// The simple values 20..23 are normalized to
        /// false, true, null and undefined.
        /// </summary>
        /// <param name="Value">A CBOR simple value.</param>
        public static CBORValue FromSimpleValue(CBORSimpleValue Value)

            => Value.Value switch {
                   20  => False,
                   21  => True,
                   22  => Null,
                   23  => Undefined,
                   _   => new (CBORValueKind.SimpleValue,
                               Value.Value,
                               null)
               };

        #endregion

        #region (static) FromArray       (Items)

        /// <summary>
        /// Create a CBOR array from the given CBOR values.
        /// </summary>
        /// <param name="Items">An enumeration of CBOR values.</param>
        public static CBORValue FromArray(params ReadOnlySpan<CBORValue> Items)

            => new (CBORValueKind.Array,
                    0,
                    Items.ToArray());


        /// <summary>
        /// Create a CBOR array from the given CBOR values.
        /// </summary>
        /// <param name="Items">An enumeration of CBOR values.</param>
        public static CBORValue FromArray(IEnumerable<CBORValue> Items)

            => new (CBORValueKind.Array,
                    0,
                    Items.ToArray());

        #endregion

        #region (static) FromMap         (Entries)

        /// <summary>
        /// Create a CBOR map from the given CBOR key/value pairs,
        /// preserving their order. Keys may be of any CBOR kind.
        /// </summary>
        /// <param name="Entries">An enumeration of CBOR key/value pairs.</param>
        public static CBORValue FromMap(IEnumerable<KeyValuePair<CBORValue, CBORValue>> Entries)

            => new (CBORValueKind.Map,
                    0,
                    Entries.ToArray());

        #endregion

        #region (static) Tagged          (Tag, Value)

        /// <summary>
        /// Create a tagged CBOR value.
        /// </summary>
        /// <param name="Tag">A CBOR tag.</param>
        /// <param name="Value">The tagged CBOR value.</param>
        public static CBORValue Tagged(CBORTag    Tag,
                                       CBORValue  Value)

            => new (CBORValueKind.Tagged,
                    Tag.Value,
                    new CBORTagBox(Value));

        #endregion

        #region WithTag(Tag)

        /// <summary>
        /// Wrap this CBOR value within the given CBOR tag.
        /// </summary>
        /// <param name="Tag">A CBOR tag.</param>
        public CBORValue WithTag(CBORTag Tag)

            => Tagged(Tag, this);

        #endregion

        #region HasTag (Tag)

        /// <summary>
        /// Whether this CBOR value is tagged with the given CBOR tag.
        /// </summary>
        /// <param name="Tag">A CBOR tag.</param>
        public Boolean HasTag(CBORTag Tag)

            => kind    == CBORValueKind.Tagged &&
               numeric == Tag.Value;

        #endregion


        #region (implicit) CBORValue(...)

        /// <summary>
        /// Convert the given number into a CBOR value.
        /// </summary>
        public static implicit operator CBORValue(Int32 Value)
            => FromInt64(Value);

        /// <summary>
        /// Convert the given number into a CBOR value.
        /// </summary>
        public static implicit operator CBORValue(Int64 Value)
            => FromInt64(Value);

        /// <summary>
        /// Convert the given number into a CBOR value.
        /// </summary>
        public static implicit operator CBORValue(UInt64 Value)
            => FromUInt64(Value);

        /// <summary>
        /// Convert the given boolean into a CBOR value.
        /// </summary>
        public static implicit operator CBORValue(Boolean Value)
            => FromBoolean(Value);

        /// <summary>
        /// Convert the given text into a CBOR value.
        /// </summary>
        public static implicit operator CBORValue(String? Value)
            => Value is null ? Null : FromText(Value);

        /// <summary>
        /// Convert the given number into a CBOR value.
        /// </summary>
        public static implicit operator CBORValue(Double Value)
            => FromDouble(Value);

        /// <summary>
        /// Convert the given byte array into a CBOR value.
        /// </summary>
        public static implicit operator CBORValue(Byte[] Value)
            => FromBytes(Value);

        #endregion


        #region this[Index]

        /// <summary>
        /// Return the CBOR value at the given position within this CBOR array.
        /// </summary>
        /// <param name="Index">The position within this CBOR array.</param>
        public CBORValue this[Int32 Index]
        {
            get
            {

                if (kind != CBORValueKind.Array)
                    throw new CBORException($"A CBOR {kind} has no indexed items!");

                var items = (CBORValue[]) reference!;

                if (Index < 0 || Index >= items.Length)
                    throw new CBORException($"The CBOR array has {items.Length} data item(s), but item {Index} was requested!");

                return items[Index];

            }
        }

        #endregion

        #region this[Key: Text]

        /// <summary>
        /// Return the CBOR value of the given key within this CBOR map.
        /// </summary>
        /// <param name="Key">A map key.</param>
        public CBORValue this[String Key]
        {
            get
            {

                if (TryGetValue(FromText(Key), out var value))
                    return value;

                throw new CBORException($"The CBOR map has no key '{Key}'!");

            }
        }

        #endregion

        #region this[Key: Integer]

        /// <summary>
        /// Return the CBOR value of the given key within this CBOR map.
        /// </summary>
        /// <param name="Key">A map key.</param>
        public CBORValue this[Int64 Key]
        {
            get
            {

                if (TryGetValue(FromInt64(Key), out var value))
                    return value;

                throw new CBORException($"The CBOR map has no key '{Key}'!");

            }
        }

        #endregion

        #region TryGetValue(Key, out Value)

        /// <summary>
        /// Try to return the CBOR value of the given key within this CBOR map.
        /// </summary>
        /// <param name="Key">A map key of any CBOR kind.</param>
        /// <param name="Value">The CBOR value of the given key.</param>
        public Boolean TryGetValue(CBORValue      Key,
                                   out CBORValue  Value)
        {

            if (kind != CBORValueKind.Map)
                throw new CBORException($"A CBOR {kind} has no keys!");

            foreach (var entry in (KeyValuePair<CBORValue, CBORValue>[]) reference!)
            {
                if (entry.Key.Equals(Key))
                {
                    Value = entry.Value;
                    return true;
                }
            }

            Value = Null;
            return false;

        }

        #endregion


        #region As...()

        /// <summary>
        /// Return this CBOR value as an unsigned integer.
        /// </summary>
        public UInt64 AsUInt64()

            => kind == CBORValueKind.UnsignedInteger
                   ? numeric
                   : throw new CBORException($"A CBOR {kind} is not an unsigned integer!");


        /// <summary>
        /// Return this CBOR value as a signed integer.
        /// </summary>
        public Int64 AsInt64()

            => kind switch {

                   CBORValueKind.UnsignedInteger  => numeric <= Int64.MaxValue
                                                         ? (Int64) numeric
                                                         : throw new OverflowException($"The unsigned integer '{numeric}' does not fit into an Int64!"),

                   CBORValueKind.NegativeInteger  => numeric <= Int64.MaxValue
                                                         ? -1L - (Int64) numeric
                                                         : throw new OverflowException($"The negative integer '-1-{numeric}' does not fit into an Int64!"),

                   _                              => throw new CBORException($"A CBOR {kind} is not an integer!")

               };


        /// <summary>
        /// Return this CBOR value as a signed integer,
        /// covering the full CBOR integer range of -2^64 .. 2^64-1.
        /// </summary>
        public Int128 AsInt128()

            => kind switch {
                   CBORValueKind.UnsignedInteger  => numeric,
                   CBORValueKind.NegativeInteger  => -1 - (Int128) numeric,
                   _                              => throw new CBORException($"A CBOR {kind} is not an integer!")
               };


        /// <summary>
        /// Return this CBOR value as a big integer:
        /// A plain integer or an unsigned/negative bignum (tag 2/3).
        /// </summary>
        public BigInteger AsBigInteger()
        {

            switch (kind)
            {

                case CBORValueKind.UnsignedInteger:
                    return numeric;

                case CBORValueKind.NegativeInteger:
                    return BigInteger.MinusOne - numeric;

                case CBORValueKind.Tagged:

                    var tag = new CBORTag(numeric);

                    if (tag != CBORTag.UnsignedBignum &&
                        tag != CBORTag.NegativeBignum)
                    {
                        break;
                    }

                    var inner = ((CBORTagBox) reference!).Inner;

                    if (inner.Kind != CBORValueKind.ByteString)
                        throw new CBORException($"The content of a bignum (tag {tag}) must be a byte string!");

                    var magnitude = new BigInteger(inner.AsBytes(),
                                                   isUnsigned:  true,
                                                   isBigEndian: true);

                    return tag == CBORTag.NegativeBignum
                               ? -magnitude - 1
                               : magnitude;

            }

            throw new CBORException($"A CBOR {kind} is not an integer or bignum!");

        }


        /// <summary>
        /// Return this CBOR value as a decimal:
        /// A plain integer, a bignum (tag 2/3) or a
        /// decimal fraction (tag 4).
        /// </summary>
        public Decimal AsDecimal()
        {

            switch (kind)
            {

                case CBORValueKind.UnsignedInteger:
                    return numeric;

                case CBORValueKind.NegativeInteger:
                    return (Decimal) (-1 - (Int128) numeric);

                case CBORValueKind.Tagged:

                    var tag = new CBORTag(numeric);

                    if (tag == CBORTag.UnsignedBignum ||
                        tag == CBORTag.NegativeBignum)
                    {
                        return CBORReader.DecimalFromParts(AsBigInteger(), 0);
                    }

                    if (tag != CBORTag.DecimalFraction)
                        break;

                    var inner = ((CBORTagBox) reference!).Inner;

                    if (inner.Kind != CBORValueKind.Array || inner.Count != 2)
                        throw new CBORException("A decimal fraction (tag 4) must be an array of two data items!");

                    var exponent  = inner[0].AsInt128();
                    var mantissa  = inner[1].AsBigInteger();

                    return CBORReader.DecimalFromParts(mantissa, exponent);

            }

            throw new CBORException($"A CBOR {kind} is not a decimal!");

        }


        /// <summary>
        /// Return this CBOR value as a double-precision floating-point number.
        /// </summary>
        public Double AsDouble()

            => kind switch {
                   CBORValueKind.HalfFloat    => (Double) BitConverter.UInt16BitsToHalf  ((UInt16) numeric),
                   CBORValueKind.SingleFloat  => (Double) BitConverter.UInt32BitsToSingle((UInt32) numeric),
                   CBORValueKind.DoubleFloat  =>          BitConverter.UInt64BitsToDouble(         numeric),
                   _                          => throw new CBORException($"A CBOR {kind} is not a floating-point number!")
               };


        /// <summary>
        /// Return this CBOR value as a boolean value.
        /// </summary>
        public Boolean AsBoolean()

            => kind == CBORValueKind.Boolean
                   ? numeric != 0
                   : throw new CBORException($"A CBOR {kind} is not a boolean value!");


        /// <summary>
        /// Return this CBOR value as a text.
        /// </summary>
        public String AsText()

            => kind == CBORValueKind.TextString
                   ? (String) reference!
                   : throw new CBORException($"A CBOR {kind} is not a text string!");


        /// <summary>
        /// Return this CBOR value as a byte array.
        /// </summary>
        public Byte[] AsBytes()

            => kind == CBORValueKind.ByteString
                   ? (Byte[]) reference!
                   : throw new CBORException($"A CBOR {kind} is not a byte string!");


        /// <summary>
        /// Return this CBOR value as a CBOR simple value.
        /// </summary>
        public CBORSimpleValue AsSimpleValue()

            => kind switch {
                   CBORValueKind.Boolean      => numeric != 0 ? CBORSimpleValue.True : CBORSimpleValue.False,
                   CBORValueKind.Null         => CBORSimpleValue.Null,
                   CBORValueKind.Undefined    => CBORSimpleValue.Undefined,
                   CBORValueKind.SimpleValue  => CBORSimpleValue.Parse((Byte) numeric),
                   _                          => throw new CBORException($"A CBOR {kind} is not a simple value!")
               };


        /// <summary>
        /// Return the data items of this CBOR array.
        /// </summary>
        public IReadOnlyList<CBORValue> AsArray()

            => kind == CBORValueKind.Array
                   ? (CBORValue[]) reference!
                   : throw new CBORException($"A CBOR {kind} is not an array!");


        /// <summary>
        /// Return the key/value pairs of this CBOR map in their original order.
        /// </summary>
        public IReadOnlyList<KeyValuePair<CBORValue, CBORValue>> AsMap()

            => kind == CBORValueKind.Map
                   ? (KeyValuePair<CBORValue, CBORValue>[]) reference!
                   : throw new CBORException($"A CBOR {kind} is not a map!");

        #endregion

        #region TryGet...(out Value)

        /// <summary>
        /// Try to return this CBOR value as an unsigned integer.
        /// </summary>
        /// <param name="Value">The unsigned integer.</param>
        public Boolean TryGetUInt64(out UInt64 Value)
        {

            if (kind == CBORValueKind.UnsignedInteger)
            {
                Value = numeric;
                return true;
            }

            Value = default;
            return false;

        }


        /// <summary>
        /// Try to return this CBOR value as a signed integer.
        /// </summary>
        /// <param name="Value">The signed integer.</param>
        public Boolean TryGetInt64(out Int64 Value)
        {

            if (kind == CBORValueKind.UnsignedInteger && numeric <= Int64.MaxValue)
            {
                Value = (Int64) numeric;
                return true;
            }

            if (kind == CBORValueKind.NegativeInteger && numeric <= Int64.MaxValue)
            {
                Value = -1L - (Int64) numeric;
                return true;
            }

            Value = default;
            return false;

        }


        /// <summary>
        /// Try to return this CBOR value as a decimal.
        /// </summary>
        /// <param name="Value">The decimal value.</param>
        public Boolean TryGetDecimal(out Decimal Value)
        {

            try
            {

                if (kind == CBORValueKind.UnsignedInteger ||
                    kind == CBORValueKind.NegativeInteger ||
                    kind == CBORValueKind.Tagged)
                {
                    Value = AsDecimal();
                    return true;
                }

            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            { }

            Value = default;
            return false;

        }


        /// <summary>
        /// Try to return this CBOR value as a boolean value.
        /// </summary>
        /// <param name="Value">The boolean value.</param>
        public Boolean TryGetBoolean(out Boolean Value)
        {

            if (kind == CBORValueKind.Boolean)
            {
                Value = numeric != 0;
                return true;
            }

            Value = default;
            return false;

        }


        /// <summary>
        /// Try to return this CBOR value as a text.
        /// </summary>
        /// <param name="Value">The text.</param>
        public Boolean TryGetText([NotNullWhen(true)] out String? Value)
        {

            if (kind == CBORValueKind.TextString)
            {
                Value = (String) reference!;
                return true;
            }

            Value = null;
            return false;

        }


        /// <summary>
        /// Try to return this CBOR value as a byte array.
        /// </summary>
        /// <param name="Value">The byte array.</param>
        public Boolean TryGetBytes([NotNullWhen(true)] out Byte[]? Value)
        {

            if (kind == CBORValueKind.ByteString)
            {
                Value = (Byte[]) reference!;
                return true;
            }

            Value = null;
            return false;

        }

        #endregion


        #region (static) Parse   (Data,                          Options = null)

        /// <summary>
        /// Parse the given CBOR data as a single CBOR value.
        /// </summary>
        /// <param name="Data">The encoded CBOR data.</param>
        /// <param name="Options">Optional CBOR reader options.</param>
        public static CBORValue Parse(ReadOnlySpan<Byte>  Data,
                                      CBORReaderOptions?  Options = null)
        {

            var reader  = new CBORReader(Data, Options);
            var value   = ReadFrom(ref reader);

            if (reader.BytesRemaining > 0)
                throw new CBORException($"{reader.BytesRemaining} unexpected trailing byte(s) after the CBOR data item!");

            return value;

        }

        #endregion

        #region (static) TryParse(Data, out CBOR, out ErrorResponse, Options = null)

        /// <summary>
        /// Try to parse the given CBOR data as a single CBOR value.
        /// </summary>
        /// <param name="Data">The encoded CBOR data.</param>
        /// <param name="CBOR">The parsed CBOR value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="Options">Optional CBOR reader options.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte>                Data,
                                       out CBORValue                     CBOR,
                                       [NotNullWhen(false)] out String?  ErrorResponse,
                                       CBORReaderOptions?                Options = null)
        {

            try
            {

                CBOR           = Parse(Data, Options);
                ErrorResponse  = null;

                return true;

            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            {

                CBOR           = Null;
                ErrorResponse  = e.Message;

                return false;

            }

        }

        #endregion

        #region (static) ReadFrom(ref Reader)

        /// <summary>
        /// Read a single CBOR value from the given CBOR reader.
        /// </summary>
        /// <param name="Reader">A CBOR reader.</param>
        public static CBORValue ReadFrom(ref CBORReader Reader)
        {

            switch (Reader.PeekState())
            {

                case CBORReaderState.UnsignedInteger:
                    return FromUInt64(Reader.ReadUInt64());

                case CBORReaderState.NegativeInteger:

                    var negativeValue = Reader.ReadInt128();

                    return new CBORValue(CBORValueKind.NegativeInteger,
                                         (UInt64) (UInt128) (-(negativeValue + 1)),
                                         null);

                case CBORReaderState.ByteString:
                case CBORReaderState.StartIndefiniteLengthByteString:
                    return FromBytes(Reader.ReadByteString());

                case CBORReaderState.TextString:
                case CBORReaderState.StartIndefiniteLengthTextString:
                    return FromText(Reader.ReadTextString());

                case CBORReaderState.StartArray:
                {

                    var count  = Reader.ReadStartArray();
                    var items  = count.HasValue
                                     ? new List<CBORValue>(Math.Min(count.Value, 128))
                                     : [];

                    if (count.HasValue)
                    {
                        for (var i = 0; i < count.Value; i++)
                            items.Add(ReadFrom(ref Reader));
                    }
                    else
                    {
                        while (Reader.PeekState() != CBORReaderState.EndArray)
                            items.Add(ReadFrom(ref Reader));
                    }

                    Reader.ReadEndArray();

                    return new CBORValue(CBORValueKind.Array,
                                         0,
                                         items.ToArray());

                }

                case CBORReaderState.StartMap:
                {

                    var pairs    = Reader.ReadStartMap();
                    var entries  = pairs.HasValue
                                       ? new List<KeyValuePair<CBORValue, CBORValue>>(Math.Min(pairs.Value, 128))
                                       : [];

                    if (pairs.HasValue)
                    {
                        for (var i = 0; i < pairs.Value; i++)
                        {
                            var key    = ReadFrom(ref Reader);
                            var value  = ReadFrom(ref Reader);
                            entries.Add(new KeyValuePair<CBORValue, CBORValue>(key, value));
                        }
                    }
                    else
                    {
                        while (Reader.PeekState() != CBORReaderState.EndMap)
                        {
                            var key    = ReadFrom(ref Reader);
                            var value  = ReadFrom(ref Reader);
                            entries.Add(new KeyValuePair<CBORValue, CBORValue>(key, value));
                        }
                    }

                    Reader.ReadEndMap();

                    return new CBORValue(CBORValueKind.Map,
                                         0,
                                         ApplyDuplicateKeyPolicy(entries, Reader.Options.DuplicateKeyPolicy));

                }

                case CBORReaderState.Tag:

                    var tag    = Reader.ReadTag();
                    var inner  = ReadFrom(ref Reader);

                    return Tagged(tag, inner);

                case CBORReaderState.Boolean:
                    return Reader.ReadBoolean() ? True : False;

                case CBORReaderState.Null:
                    Reader.ReadNull();
                    return Null;

                case CBORReaderState.Undefined:
                    Reader.ReadSimpleValue();
                    return Undefined;

                case CBORReaderState.SimpleValue:
                    return FromSimpleValue(Reader.ReadSimpleValue());

                case CBORReaderState.HalfPrecisionFloat:
                    return FromHalf(Reader.ReadHalf());

                case CBORReaderState.SinglePrecisionFloat:
                    return FromSingle(Reader.ReadSingle());

                case CBORReaderState.DoublePrecisionFloat:
                    return FromDouble(Reader.ReadDouble());

                default:
                    throw new CBORException($"Unexpected CBOR reader state '{Reader.PeekState()}'!");

            }

        }

        #endregion


        #region WriteTo(Writer)

        /// <summary>
        /// Write this CBOR value to the given CBOR writer.
        /// Floating-point numbers follow the writer's encoding options.
        /// </summary>
        /// <param name="Writer">A CBOR writer.</param>
        public void WriteTo(CBORWriter Writer)
        {

            switch (kind)
            {

                case CBORValueKind.Null:
                    Writer.WriteNull();
                    break;

                case CBORValueKind.Undefined:
                    Writer.WriteUndefined();
                    break;

                case CBORValueKind.Boolean:
                    Writer.WriteBoolean(numeric != 0);
                    break;

                case CBORValueKind.UnsignedInteger:
                    Writer.WriteUInt64(numeric);
                    break;

                case CBORValueKind.NegativeInteger:
                    Writer.WriteInt128(-1 - (Int128) numeric);
                    break;

                case CBORValueKind.ByteString:
                    Writer.WriteByteString((Byte[]) reference!);
                    break;

                case CBORValueKind.TextString:
                    Writer.WriteTextString((String) reference!);
                    break;

                case CBORValueKind.Array:

                    var items = (CBORValue[]) reference!;

                    Writer.WriteStartArray(items.Length);

                    foreach (var item in items)
                        item.WriteTo(Writer);

                    Writer.WriteEndArray();
                    break;

                case CBORValueKind.Map:

                    var entries = (KeyValuePair<CBORValue, CBORValue>[]) reference!;

                    Writer.WriteStartMap(entries.Length);

                    foreach (var entry in entries)
                    {
                        entry.Key.  WriteTo(Writer);
                        entry.Value.WriteTo(Writer);
                    }

                    Writer.WriteEndMap();
                    break;

                case CBORValueKind.Tagged:
                    Writer.WriteTag(new CBORTag(numeric));
                    ((CBORTagBox) reference!).Inner.WriteTo(Writer);
                    break;

                case CBORValueKind.SimpleValue:
                    Writer.WriteSimpleValue(CBORSimpleValue.Parse((Byte) numeric));
                    break;

                case CBORValueKind.HalfFloat:
                    Writer.WriteHalf(BitConverter.UInt16BitsToHalf((UInt16) numeric));
                    break;

                case CBORValueKind.SingleFloat:
                    Writer.WriteSingle(BitConverter.UInt32BitsToSingle((UInt32) numeric));
                    break;

                case CBORValueKind.DoubleFloat:
                    Writer.WriteDouble(BitConverter.UInt64BitsToDouble(numeric));
                    break;

            }

        }

        #endregion

        #region ToByteArray(Options = null)

        /// <summary>
        /// Return the encoded CBOR data of this CBOR value.
        /// </summary>
        /// <param name="Options">Optional CBOR writer options.</param>
        public Byte[] ToByteArray(CBORWriterOptions? Options = null)
        {

            var writer = new CBORWriter(Options);

            WriteTo(writer);

            return writer.ToByteArray();

        }

        #endregion


        #region (private static) ApplyDuplicateKeyPolicy(Entries, Policy)

        private static KeyValuePair<CBORValue, CBORValue>[] ApplyDuplicateKeyPolicy(List<KeyValuePair<CBORValue, CBORValue>>  Entries,
                                                                                    CBORDuplicateKeyPolicy                    Policy)
        {

            if (Entries.Count < 2)
                return [.. Entries];

            switch (Policy)
            {

                case CBORDuplicateKeyPolicy.Error:
                {

                    var seenKeys = new HashSet<CBORValue>();

                    foreach (var entry in Entries)
                    {
                        if (!seenKeys.Add(entry.Key))
                            throw new CBORException($"Duplicate CBOR map key {entry.Key.ToDiagnosticString()}!");
                    }

                    return [.. Entries];

                }

                case CBORDuplicateKeyPolicy.TakeFirst:
                {

                    var seenKeys  = new HashSet<CBORValue>();
                    var result    = new List<KeyValuePair<CBORValue, CBORValue>>(Entries.Count);

                    foreach (var entry in Entries)
                    {
                        if (seenKeys.Add(entry.Key))
                            result.Add(entry);
                    }

                    return [.. result];

                }

                default:  // TakeLast
                {

                    var indexOfKey  = new Dictionary<CBORValue, Int32>();
                    var result      = new List<KeyValuePair<CBORValue, CBORValue>>(Entries.Count);

                    foreach (var entry in Entries)
                    {

                        if (indexOfKey.TryGetValue(entry.Key, out var index))
                            result[index] = entry;

                        else
                        {
                            indexOfKey.Add(entry.Key, result.Count);
                            result.Add(entry);
                        }

                    }

                    return [.. result];

                }

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


        #region Operator overloading

        #region Operator == (CBORValue1, CBORValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORValue1">A CBOR value.</param>
        /// <param name="CBORValue2">Another CBOR value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CBORValue CBORValue1,
                                           CBORValue CBORValue2)

            => CBORValue1.Equals(CBORValue2);

        #endregion

        #region Operator != (CBORValue1, CBORValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORValue1">A CBOR value.</param>
        /// <param name="CBORValue2">Another CBOR value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CBORValue CBORValue1,
                                           CBORValue CBORValue2)

            => !CBORValue1.Equals(CBORValue2);

        #endregion

        #endregion

        #region IEquatable<CBORValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CBOR values for representational equality.
        /// </summary>
        /// <param name="Object">A CBOR value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CBORValue cborValue &&
                   Equals(cborValue);

        #endregion

        #region Equals(CBORValue)

        /// <summary>
        /// Compares two CBOR values for representational equality.
        /// </summary>
        /// <param name="CBORValue">A CBOR value to compare with.</param>
        public Boolean Equals(CBORValue CBORValue)
        {

            if (kind    != CBORValue.kind ||
                numeric != CBORValue.numeric)
            {
                return false;
            }

            switch (kind)
            {

                case CBORValueKind.ByteString:
                    return ((Byte[]) reference!).AsSpan().SequenceEqual((Byte[]) CBORValue.reference!);

                case CBORValueKind.TextString:
                    return String.Equals((String) reference!,
                                         (String) CBORValue.reference!,
                                         StringComparison.Ordinal);

                case CBORValueKind.Array:
                {

                    var items1 = (CBORValue[]) reference!;
                    var items2 = (CBORValue[]) CBORValue.reference!;

                    if (items1.Length != items2.Length)
                        return false;

                    for (var i = 0; i < items1.Length; i++)
                    {
                        if (!items1[i].Equals(items2[i]))
                            return false;
                    }

                    return true;

                }

                case CBORValueKind.Map:
                {

                    var entries1 = (KeyValuePair<CBORValue, CBORValue>[]) reference!;
                    var entries2 = (KeyValuePair<CBORValue, CBORValue>[]) CBORValue.reference!;

                    if (entries1.Length != entries2.Length)
                        return false;

                    for (var i = 0; i < entries1.Length; i++)
                    {
                        if (!entries1[i].Key.  Equals(entries2[i].Key) ||
                            !entries1[i].Value.Equals(entries2[i].Value))
                        {
                            return false;
                        }
                    }

                    return true;

                }

                case CBORValueKind.Tagged:
                    return ((CBORTagBox) reference!).Inner.Equals(((CBORTagBox) CBORValue.reference!).Inner);

                default:
                    return true;

            }

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {

            var hash = new HashCode();

            hash.Add((Int32) kind);
            hash.Add(numeric);

            switch (kind)
            {

                case CBORValueKind.ByteString:
                    hash.AddBytes((Byte[]) reference!);
                    break;

                case CBORValueKind.TextString:
                    hash.Add((String) reference!, StringComparer.Ordinal);
                    break;

                case CBORValueKind.Array:

                    foreach (var item in (CBORValue[]) reference!)
                        hash.Add(item);

                    break;

                case CBORValueKind.Map:

                    foreach (var entry in (KeyValuePair<CBORValue, CBORValue>[]) reference!)
                    {
                        hash.Add(entry.Key);
                        hash.Add(entry.Value);
                    }

                    break;

                case CBORValueKind.Tagged:
                    hash.Add(((CBORTagBox) reference!).Inner);
                    break;

            }

            return hash.ToHashCode();

        }

        #endregion

        #region ToDiagnosticString()

        /// <summary>
        /// Return the RFC 8949, Section 8 diagnostic notation
        /// of this CBOR value, e.g. 4([-1, 50]).
        /// </summary>
        public String ToDiagnosticString()

            => CBORDiagnosticNotation.Format(this);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object,
        /// using the RFC 8949 diagnostic notation.
        /// </summary>
        public override String ToString()

            => CBORDiagnosticNotation.Format(this);

        #endregion

    }

}
