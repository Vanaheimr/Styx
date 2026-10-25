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

using System.Text;
using System.Buffers;
using System.Numerics;
using System.Text.Json;
using System.Globalization;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A compact JSON value.
    /// </summary>
    public readonly struct CustomDataValue
    {

        #region Properties

        public CustomDataValueKind  Kind            { get; }
        public Int64                Int64Value      { get; }
        public UInt64               UInt64Value     { get; }
        public Decimal              DecimalValue    { get; }
        public Double               DoubleValue     { get; }
        public String?              StringValue     { get; }
        public CustomDataNew?       ObjectValue     { get; }
        public CustomDataArray?     ArrayValue      { get; }

        public Boolean              BooleanValue
            => Int64Value != 0;

        #endregion

        #region Constructor(s)

        private CustomDataValue(CustomDataValueKind  Kind,
                                Int64                Int64Value   = default,
                                UInt64               UInt64Value  = default,
                                Decimal              DecimalValue = default,
                                Double               DoubleValue  = default,
                                String?              StringValue  = default,
                                CustomDataNew?       ObjectValue  = default,
                                CustomDataArray?     ArrayValue   = default)
        {
            this.Kind          = Kind;
            this.Int64Value    = Int64Value;
            this.UInt64Value   = UInt64Value;
            this.DecimalValue  = DecimalValue;
            this.DoubleValue   = DoubleValue;
            this.StringValue   = StringValue;
            this.ObjectValue   = ObjectValue;
            this.ArrayValue    = ArrayValue;
        }

        #endregion


        #region Static values

        public static CustomDataValue Null  { get; }
            = new(CustomDataValueKind.Null);

        public static CustomDataValue True  { get; }
            = new(CustomDataValueKind.Boolean, Int64Value: 1);

        public static CustomDataValue False { get; }
            = new(CustomDataValueKind.Boolean);

        #endregion

        #region Factory methods

        public static CustomDataValue From(Boolean Value)
            => Value ? True : False;

        public static CustomDataValue From(Int32 Value)
            => new(CustomDataValueKind.Int64, Int64Value: Value);

        public static CustomDataValue From(Int64 Value)
            => new(CustomDataValueKind.Int64, Int64Value: Value);

        public static CustomDataValue From(UInt64 Value)
            => new(CustomDataValueKind.UInt64, UInt64Value: Value);

        public static CustomDataValue From(Decimal Value)
            => new(CustomDataValueKind.Decimal, DecimalValue: Value);

        public static CustomDataValue From(Double Value)
            => new(CustomDataValueKind.Double, DoubleValue: Value);

        public static CustomDataValue From(String Value)
            => new(CustomDataValueKind.String, StringValue: Value);

        public static CustomDataValue From(CustomDataNew Value)
            => new(CustomDataValueKind.Object, ObjectValue: Value);

        public static CustomDataValue From(CustomDataArray Value)
            => new(CustomDataValueKind.Array, ArrayValue: Value);

        private static CustomDataValue FromNumber(String JSONNumber)
            => new(CustomDataValueKind.Number, StringValue: JSONNumber);

        #endregion

        #region (static) ParseJSON(ref Reader)

        internal static CustomDataValue ParseJSON(ref Utf8JsonReader Reader)
        {

            switch (Reader.TokenType)
            {

                case JsonTokenType.StartObject:
                    return From(ParseObject(ref Reader));

                case JsonTokenType.StartArray:
                    return From(ParseArray(ref Reader));

                case JsonTokenType.String:
                    return From(Reader.GetString() ?? String.Empty);

                case JsonTokenType.Number:
                    return ParseNumber(ref Reader);

                case JsonTokenType.True:
                    return True;

                case JsonTokenType.False:
                    return False;

                case JsonTokenType.Null:
                    return Null;

                default:
                    throw new JsonException($"Unexpected JSON token '{Reader.TokenType}'.");

            }

        }

        #endregion

        #region (static) ParseJToken(Token)

        /// <summary>
        /// The value of the given JSON token, as the token holds it: a decimal with its decimal places, an integer
        /// beyond 64 bits with all its digits, a double as the double, a text as the text.
        /// </summary>
        /// <remarks>
        /// Other tokens are read from the JSON text Newtonsoft writes of them, e.g. a date, which only a JSON text read
        /// with DateParseHandling.DateTime holds: as Newtonsoft writes it, "2024-01-01T12:00:00Z", the text it was read
        /// from is lost.
        /// </remarks>
        internal static CustomDataValue ParseJToken(JToken Token)
        {

            switch (Token.Type)
            {

                case JTokenType.Object:
                    return From(CustomDataNew.ParseJObject((JObject) Token));

                case JTokenType.Array:
                    var array = (JArray) Token;
                    return From(array.Count == 0
                                    ? CustomDataArray.Empty
                                    : new CustomDataArray(array.Select(ParseJToken)));

                case JTokenType.Null:
                    return Null;

                case JTokenType.Boolean:
                    return From(Token.Value<Boolean>());

                case JTokenType.String:
                    return Token.Value<String>() is String text
                               ? From(text)
                               : Null;

                case JTokenType.Integer:
                    return ((JValue) Token).Value switch {
                               BigInteger bigInteger when bigInteger >= Int64.MinValue && bigInteger <= Int64.MaxValue   => From((Int64)  bigInteger),
                               BigInteger bigInteger when bigInteger >= 0              && bigInteger <= UInt64.MaxValue  => From((UInt64) bigInteger),
                               BigInteger bigInteger                                                                     => FromNumber(bigInteger.ToString(CultureInfo.InvariantCulture)),
                               UInt64     unsigned   when unsigned   >  Int64.MaxValue                                   => From(unsigned),
                               var        integer                                                                        => From(Convert.ToInt64(integer, CultureInfo.InvariantCulture))
                           };

                case JTokenType.Float:

                    if (((JValue) Token).Value is Decimal decimalValue)
                        return From(decimalValue);

                    var doubleValue = Token.Value<Double>();

                    if (!Double.IsFinite(doubleValue))
                        throw new JsonException($"The number {doubleValue} can not be represented as JSON.");

                    return From(doubleValue);

                default:
                    var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(Token.ToString(Newtonsoft.Json.Formatting.None)));
                    reader.Read();
                    return ParseJSON(ref reader);

            }

        }

        #endregion

        #region ToJToken()

        /// <summary>
        /// This value as a JSON token, as Newtonsoft reads its JSON text with DateParseHandling.None and
        /// FloatParseHandling.Decimal: a text stays a text, also one that looks like a point in time, a decimal keeps
        /// its decimal places.
        /// </summary>
        /// <remarks>
        /// A number kept as its JSON text is a JSON token only as precise as Newtonsoft reads it: an integer beyond
        /// 64 bits as the BigInteger, any other number as a decimal, rounded to 28 decimal places, or as a double,
        /// when it is beyond the range of a decimal.
        /// </remarks>
        public JToken ToJToken()
        {

            switch (Kind)
            {

                case CustomDataValueKind.Null:
                    return JValue.CreateNull();

                case CustomDataValueKind.Boolean:
                    return new JValue(BooleanValue);

                case CustomDataValueKind.Int64:
                    return new JValue(Int64Value);

                case CustomDataValueKind.UInt64:
                    return new JValue(UInt64Value);

                case CustomDataValueKind.Decimal:
                    return new JValue(DecimalValue);

                case CustomDataValueKind.Double:
                    return new JValue(DoubleValue);

                case CustomDataValueKind.String:
                    return JValue.CreateString(StringValue);

                case CustomDataValueKind.Number:
                    try
                    {
                        using var reader = new Newtonsoft.Json.JsonTextReader(new StringReader(StringValue!)) {
                                               FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Decimal
                                           };
                        return JToken.ReadFrom(reader);
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        return new JValue(Double.Parse(StringValue!, NumberStyles.Float, CultureInfo.InvariantCulture));
                    }

                case CustomDataValueKind.Object:
                    return ObjectValue is not null
                               ? ObjectValue.ToJObject()
                               : JValue.CreateNull();

                case CustomDataValueKind.Array:
                    return ArrayValue is not null
                               ? new JArray(ArrayValue.Select(value => value.ToJToken()))
                               : JValue.CreateNull();

                default:
                    throw new JsonException($"Unsupported custom data value kind '{Kind}'.");

            }

        }

        #endregion

        #region WriteJSON(Writer)

        public void WriteJSON(Utf8JsonWriter Writer)
        {

            switch (Kind)
            {

                case CustomDataValueKind.Null:
                    Writer.WriteNullValue();
                    break;

                case CustomDataValueKind.Boolean:
                    Writer.WriteBooleanValue(BooleanValue);
                    break;

                case CustomDataValueKind.Int64:
                    Writer.WriteNumberValue(Int64Value);
                    break;

                case CustomDataValueKind.UInt64:
                    Writer.WriteNumberValue(UInt64Value);
                    break;

                case CustomDataValueKind.Decimal:
                    Writer.WriteNumberValue(DecimalValue);
                    break;

                case CustomDataValueKind.Double:
                    Writer.WriteNumberValue(DoubleValue);
                    break;

                case CustomDataValueKind.Number:
                    Writer.WriteRawValue(StringValue!, skipInputValidation: false);
                    break;

                case CustomDataValueKind.String:
                    Writer.WriteStringValue(StringValue);
                    break;

                case CustomDataValueKind.Object:
                    if (ObjectValue is null)
                        Writer.WriteNullValue();
                    else
                        ObjectValue.WriteJSON(Writer);
                    break;

                case CustomDataValueKind.Array:
                    if (ArrayValue is null)
                        Writer.WriteNullValue();
                    else
                        ArrayValue.WriteJSON(Writer);
                    break;

                default:
                    throw new JsonException($"Unsupported custom data value kind '{Kind}'.");

            }

        }

        #endregion

        #region WriteRAWJSON(Writer)

        /// <summary>
        /// Serialize the internal representation as JSON.
        /// This is mainly intended for tests and debugging, not for public APIs.
        /// </summary>
        public void WriteRAWJSON(Utf8JsonWriter Writer)
        {

            switch (Kind)
            {

                case CustomDataValueKind.Null:
                    Writer.WriteNullValue();
                    break;

                case CustomDataValueKind.Boolean:
                    Writer.WriteBooleanValue(BooleanValue);
                    break;

                case CustomDataValueKind.Int64:
                    Writer.WriteNumberValue(Int64Value);
                    break;

                case CustomDataValueKind.UInt64:
                    Writer.WriteNumberValue(UInt64Value);
                    break;

                case CustomDataValueKind.Decimal:
                    Writer.WriteNumberValue(DecimalValue);
                    break;

                case CustomDataValueKind.Double:
                    Writer.WriteNumberValue(DoubleValue);
                    break;

                case CustomDataValueKind.Number:
                    Writer.WriteRawValue(StringValue!, skipInputValidation: false);
                    break;

                case CustomDataValueKind.String:
                    Writer.WriteStringValue(StringValue);
                    break;

                case CustomDataValueKind.Object:
                    if (ObjectValue is null)
                        Writer.WriteNullValue();
                    else
                        ObjectValue.WriteRAWJSONObject(Writer);
                    break;

                case CustomDataValueKind.Array:
                    if (ArrayValue is null)
                        Writer.WriteNullValue();
                    else
                        ArrayValue.WriteRAWJSON(Writer);
                    break;

                default:
                    throw new JsonException($"Unsupported custom data value kind '{Kind}'.");

            }

        }

        #endregion


        #region (private static) ParseObject(ref Reader)

        private static CustomDataNew ParseObject(ref Utf8JsonReader Reader)
        {

            var properties = new List<CustomDataProperty>();
            var indexes    = new Dictionary<Int32, Int32>();

            while (Reader.Read())
            {

                if (Reader.TokenType == JsonTokenType.EndObject)
                    return new CustomDataNew(properties);

                if (Reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException($"Expected a JSON property name, but found '{Reader.TokenType}'.");

                var propertyName  = Reader.GetString() ?? String.Empty;
                var propertyKeyId = CustomDataPropertyKeyLookup.GetOrAdd(propertyName);

                if (!Reader.Read())
                    throw new JsonException($"Missing JSON value for property '{propertyName}'.");

                var property = new CustomDataProperty(
                                   propertyKeyId,
                                   ParseJSON(ref Reader)
                               );

                if (indexes.TryGetValue(propertyKeyId, out var index))
                    properties[index] = property;
                else
                {
                    indexes.Add(propertyKeyId, properties.Count);
                    properties.Add(property);
                }

            }

            throw new JsonException("Unexpected end of JSON object.");

        }

        #endregion

        #region (private static) ParseArray(ref Reader)

        private static CustomDataArray ParseArray(ref Utf8JsonReader Reader)
        {

            var values = new List<CustomDataValue>();

            while (Reader.Read())
            {

                if (Reader.TokenType == JsonTokenType.EndArray)
                    return values.Count == 0
                               ? CustomDataArray.Empty
                               : new CustomDataArray(values);

                values.Add(ParseJSON(ref Reader));

            }

            throw new JsonException("Unexpected end of JSON array.");

        }

        #endregion

        #region (private static) ParseNumber(ref Reader)

        /// <summary>
        /// A JSON number as written: an integer as an Int64 or an UInt64, a number with a fraction as a decimal, when
        /// the decimal is written as the number was, e.g. 1.50 with its two decimal places. Any other number is kept as
        /// its JSON text: an integer beyond 64 bits, which Newtonsoft reads as a BigInteger, a number with an exponent,
        /// with more than 28 decimal places or with a mantissa above 96 bits, which a decimal rounds, 1e-30 to 0, and a
        /// double even more.
        /// </summary>
        private static CustomDataValue ParseNumber(ref Utf8JsonReader Reader)
        {

            if (Reader.TryGetInt64(out var int64Value))
                return From(int64Value);

            if (Reader.TryGetUInt64(out var uint64Value))
                return From(uint64Value);

            ReadOnlySpan<Byte> number = Reader.HasValueSequence
                                            ? Reader.ValueSequence.ToArray()
                                            : Reader.ValueSpan;

            Span<Byte> decimalText = stackalloc Byte[64];

            if (number.IndexOfAny((Byte) '.', (Byte) 'e', (Byte) 'E') >= 0 &&
                Reader.TryGetDecimal(out var decimalValue) &&
                decimalValue.TryFormat(decimalText, out var length, default, CultureInfo.InvariantCulture) &&
                decimalText[..length].SequenceEqual(number))
            {
                return From(decimalValue);
            }

            return FromNumber(Encoding.UTF8.GetString(number));

        }

        #endregion


        public override Int32 GetHashCode()
        {
            return HashCode.Combine(
                       Kind,
                       Int64Value,
                       UInt64Value,
                       DecimalValue,
                       DoubleValue,
                       StringValue,
                       ObjectValue,
                       ArrayValue
                   );
        }

    }

}
 