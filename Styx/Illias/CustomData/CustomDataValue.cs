/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Illias <https://www.github.com/Vanaheimr/Illias>
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

using System.Text.Json;

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

        private static CustomDataValue ParseNumber(ref Utf8JsonReader Reader)
        {

            if (Reader.TryGetInt64(out var int64Value))
                return From(int64Value);

            if (Reader.TryGetUInt64(out var uint64Value))
                return From(uint64Value);

            if (Reader.TryGetDecimal(out var decimalValue))
                return From(decimalValue);

            if (Reader.TryGetDouble(out var doubleValue))
                return From(doubleValue);

            throw new JsonException("Invalid JSON number.");

        }

        #endregion

    }

}
 