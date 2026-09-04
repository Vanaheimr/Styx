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

using System.Globalization;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    #region Delegates

    /// <summary>
    /// A delegate to parse custom CBOR data.
    /// </summary>
    /// <param name="CBOR">The CBOR value to be parsed.</param>
    /// <param name="DataObject">The data object to be enriched.</param>
    public delegate T          CustomCBORParserDelegate<T>    (CBORValue  CBOR,
                                                               T          DataObject);

    /// <summary>
    /// A delegate to serialize custom CBOR data.
    /// </summary>
    /// <param name="DataObject">The data object to be serialized.</param>
    /// <param name="CBOR">The default CBOR representation of the data object.</param>
    public delegate CBORValue  CustomCBORSerializerDelegate<T>(T          DataObject,
                                                               CBORValue  CBOR);

    /// <summary>
    /// A delegate to try to parse the given CBOR value.
    /// </summary>
    /// <param name="Input">The CBOR value to be parsed.</param>
    /// <param name="Result">The parsed result.</param>
    /// <param name="ErrorResponse">An optional error response.</param>
    public delegate Boolean    TryCBORParser<TResult>         (CBORValue                         Input,
                                                               [NotNullWhen(true)]  out TResult? Result,
                                                               [NotNullWhen(false)] out String?  ErrorResponse);

    #endregion


    /// <summary>
    /// Extension methods for parsing mandatory and optional properties
    /// of CBOR maps, following the exact three-way contract of the
    /// JSON parsing extension methods: A missing OPTIONAL property
    /// returns false WITHOUT an error response, a property which is
    /// present but invalid (or explicitly null) returns false WITH
    /// an error response.
    /// CBOR maps within e.g. COSE structures often use integer keys,
    /// therefore all methods are also available with Int64 keys.
    /// </summary>
    public static class CBORExtensions
    {

        #region (private) ParseMandatoryInternal<T>(CBOR, Key, KeyText, PropertyDescription, Converter, out Value, out ErrorResponse)

        private static Boolean ParseMandatoryInternal<T>(CBORValue                         CBOR,
                                                         CBORValue                         Key,
                                                         String                            KeyText,
                                                         String                            PropertyDescription,
                                                         Func<CBORValue, T>                Converter,
                                                         [NotNullWhen(true)]  out T?       Value,
                                                         [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Value = default;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = "The given CBOR value is not a map!";
                return false;
            }

            if (!CBOR.TryGetValue(Key, out var property))
            {
                ErrorResponse = $"Missing CBOR property '{KeyText}'!";
                return false;
            }

            if (property.Kind == CBORValueKind.Null)
            {
                ErrorResponse = $"CBOR property '{KeyText}' must not be null!";
                return false;
            }

            try
            {

                Value          = Converter(property)!;
                ErrorResponse  = null;

                return true;

            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            {
                ErrorResponse = $"Invalid '{PropertyDescription}': {e.Message}";
                return false;
            }

        }

        #endregion

        #region (private) ParseOptionalInternal<T> (CBOR, Key, KeyText, PropertyDescription, Converter, out Value, out ErrorResponse)

        private static Boolean ParseOptionalInternal<T>(CBORValue           CBOR,
                                                        CBORValue           Key,
                                                        String              KeyText,
                                                        String              PropertyDescription,
                                                        Func<CBORValue, T>  Converter,
                                                        out T?              Value,
                                                        out String?         ErrorResponse)
        {

            Value          = default;
            ErrorResponse  = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = "The given CBOR value is not a map!";
                return false;
            }

            // A missing OPTIONAL property is not an error!
            if (!CBOR.TryGetValue(Key, out var property))
                return false;

            if (property.Kind == CBORValueKind.Null)
            {
                ErrorResponse = $"CBOR property '{KeyText}' must not be null!";
                return false;
            }

            try
            {

                Value = Converter(property);

                return true;

            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            {
                ErrorResponse = $"CBOR property '{KeyText}' ({PropertyDescription}) could not be parsed: {e.Message}";
                return false;
            }

        }

        #endregion

        #region (private static) ConvertDateTime(CBOR)

        /// <summary>
        /// Convert the given CBOR value into a date/time:
        /// A tagged (0/1) or untagged RFC 3339 text or epoch timestamp.
        /// </summary>
        private static DateTimeOffset ConvertDateTime(CBORValue CBOR)
        {

            if (CBOR.Kind == CBORValueKind.Tagged)
            {

                if (!CBOR.HasTag(CBORTag.DateTimeString) &&
                    !CBOR.HasTag(CBORTag.EpochDateTime))
                {
                    throw new CBORException($"Expected a date/time tag (0 or 1), but found tag {CBOR.Tag}!");
                }

                CBOR = CBOR.UntaggedValue;

            }

            switch (CBOR.Kind)
            {

                case CBORValueKind.TextString:

                    if (!DateTimeOffset.TryParse(CBOR.AsText(),
                                                 CultureInfo.InvariantCulture,
                                                 DateTimeStyles.None,
                                                 out var timestamp))
                    {
                        throw new CBORException($"Invalid RFC 3339 date/time text '{CBOR.AsText()}'!");
                    }

                    return timestamp;

                case CBORValueKind.UnsignedInteger:
                case CBORValueKind.NegativeInteger:

                    try
                    {
                        return DateTimeOffset.FromUnixTimeSeconds(CBOR.AsInt64());
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        throw new CBORException("The epoch-based date/time is out of range!", e);
                    }

                case CBORValueKind.HalfFloat:
                case CBORValueKind.SingleFloat:
                case CBORValueKind.DoubleFloat:

                    var seconds = CBOR.AsDouble();

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
                    throw new CBORException($"A CBOR {CBOR.Kind} is not a date/time!");

            }

        }

        #endregion

        #region (private static) ConvertEnum<TEnum>(CBOR)

        private static TEnum ConvertEnum<TEnum>(CBORValue CBOR)
            where TEnum : struct, Enum
        {

            if (CBOR.Kind == CBORValueKind.TextString &&
                Enum.TryParse<TEnum>(CBOR.AsText(), ignoreCase: true, out var parsedEnum) &&
                Enum.IsDefined(parsedEnum))
            {
                return parsedEnum;
            }

            if ((CBOR.Kind == CBORValueKind.UnsignedInteger ||
                 CBOR.Kind == CBORValueKind.NegativeInteger))
            {

                var numericEnum = (TEnum) Enum.ToObject(typeof(TEnum), CBOR.AsInt64());

                if (Enum.IsDefined(numericEnum))
                    return numericEnum;

            }

            throw new CBORException($"Invalid {typeof(TEnum).Name} value '{CBOR.ToDiagnosticString()}'!");

        }

        #endregion


        #region ParseMandatory<T>      (this CBOR, PropertyName/Key, PropertyDescription, CBORParser, out Value, out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory CBOR map property.
        /// </summary>
        /// <param name="CBOR">A CBOR map.</param>
        /// <param name="PropertyName">The name of the property.</param>
        /// <param name="PropertyDescription">A description of the property.</param>
        /// <param name="CBORParser">A delegate to parse the property value.</param>
        /// <param name="Value">The parsed value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean ParseMandatory<T>(this CBORValue                    CBOR,
                                                String                            PropertyName,
                                                String                            PropertyDescription,
                                                TryCBORParser<T>                  CBORParser,
                                                [NotNullWhen(true)]  out T?       Value,
                                                [NotNullWhen(false)] out String?  ErrorResponse)

            => ParseMandatoryInternal(CBOR,
                                      CBORValue.FromText(PropertyName),
                                      PropertyName,
                                      PropertyDescription,
                                      property => CBORParser(property, out var parsedValue, out var errorResponse)
                                                      ? parsedValue!
                                                      : throw new CBORException(errorResponse),
                                      out Value,
                                      out ErrorResponse);


        /// <summary>
        /// Parse the given mandatory CBOR map property.
        /// </summary>
        /// <param name="CBOR">A CBOR map.</param>
        /// <param name="PropertyKey">The integer key of the property.</param>
        /// <param name="PropertyDescription">A description of the property.</param>
        /// <param name="CBORParser">A delegate to parse the property value.</param>
        /// <param name="Value">The parsed value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean ParseMandatory<T>(this CBORValue                    CBOR,
                                                Int64                             PropertyKey,
                                                String                            PropertyDescription,
                                                TryCBORParser<T>                  CBORParser,
                                                [NotNullWhen(true)]  out T?       Value,
                                                [NotNullWhen(false)] out String?  ErrorResponse)

            => ParseMandatoryInternal(CBOR,
                                      CBORValue.FromInt64(PropertyKey),
                                      PropertyKey.ToString(CultureInfo.InvariantCulture),
                                      PropertyDescription,
                                      property => CBORParser(property, out var parsedValue, out var errorResponse)
                                                      ? parsedValue!
                                                      : throw new CBORException(errorResponse),
                                      out Value,
                                      out ErrorResponse);

        #endregion

        #region ParseOptional<T>       (this CBOR, PropertyName/Key, PropertyDescription, CBORParser, out Value, out ErrorResponse)

        /// <summary>
        /// Parse the given optional CBOR map property.
        /// A missing optional property is not an error!
        /// </summary>
        /// <param name="CBOR">A CBOR map.</param>
        /// <param name="PropertyName">The name of the property.</param>
        /// <param name="PropertyDescription">A description of the property.</param>
        /// <param name="CBORParser">A delegate to parse the property value.</param>
        /// <param name="Value">The parsed value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean ParseOptional<T>(this CBORValue    CBOR,
                                               String            PropertyName,
                                               String            PropertyDescription,
                                               TryCBORParser<T>  CBORParser,
                                               out T?            Value,
                                               out String?       ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromText(PropertyName),
                                     PropertyName,
                                     PropertyDescription,
                                     property => CBORParser(property, out var parsedValue, out var errorResponse)
                                                     ? parsedValue!
                                                     : throw new CBORException(errorResponse),
                                     out Value,
                                     out ErrorResponse);


        /// <summary>
        /// Parse the given optional CBOR map property.
        /// A missing optional property is not an error!
        /// </summary>
        /// <param name="CBOR">A CBOR map.</param>
        /// <param name="PropertyKey">The integer key of the property.</param>
        /// <param name="PropertyDescription">A description of the property.</param>
        /// <param name="CBORParser">A delegate to parse the property value.</param>
        /// <param name="Value">The parsed value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean ParseOptional<T>(this CBORValue    CBOR,
                                               Int64             PropertyKey,
                                               String            PropertyDescription,
                                               TryCBORParser<T>  CBORParser,
                                               out T?            Value,
                                               out String?       ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromInt64(PropertyKey),
                                     PropertyKey.ToString(CultureInfo.InvariantCulture),
                                     PropertyDescription,
                                     property => CBORParser(property, out var parsedValue, out var errorResponse)
                                                     ? parsedValue!
                                                     : throw new CBORException(errorResponse),
                                     out Value,
                                     out ErrorResponse);

        #endregion


        #region ParseMandatoryText     (this CBOR, PropertyName/Key, PropertyDescription, out Text,      out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory text property.
        /// </summary>
        public static Boolean ParseMandatoryText(this CBORValue                    CBOR,
                                                 String                            PropertyName,
                                                 String                            PropertyDescription,
                                                 [NotNullWhen(true)]  out String?  Text,
                                                 [NotNullWhen(false)] out String?  ErrorResponse)

            => ParseMandatoryInternal(CBOR,
                                      CBORValue.FromText(PropertyName),
                                      PropertyName,
                                      PropertyDescription,
                                      static property => property.AsText(),
                                      out Text,
                                      out ErrorResponse);


        /// <summary>
        /// Parse the given mandatory text property.
        /// </summary>
        public static Boolean ParseMandatoryText(this CBORValue                    CBOR,
                                                 Int64                             PropertyKey,
                                                 String                            PropertyDescription,
                                                 [NotNullWhen(true)]  out String?  Text,
                                                 [NotNullWhen(false)] out String?  ErrorResponse)

            => ParseMandatoryInternal(CBOR,
                                      CBORValue.FromInt64(PropertyKey),
                                      PropertyKey.ToString(CultureInfo.InvariantCulture),
                                      PropertyDescription,
                                      static property => property.AsText(),
                                      out Text,
                                      out ErrorResponse);

        #endregion

        #region ParseOptionalText      (this CBOR, PropertyName/Key, PropertyDescription, out Text,      out ErrorResponse)

        /// <summary>
        /// Parse the given optional text property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalText(this CBORValue  CBOR,
                                                String          PropertyName,
                                                String          PropertyDescription,
                                                out String?     Text,
                                                out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromText(PropertyName),
                                     PropertyName,
                                     PropertyDescription,
                                     static property => property.AsText(),
                                     out Text,
                                     out ErrorResponse);


        /// <summary>
        /// Parse the given optional text property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalText(this CBORValue  CBOR,
                                                Int64           PropertyKey,
                                                String          PropertyDescription,
                                                out String?     Text,
                                                out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromInt64(PropertyKey),
                                     PropertyKey.ToString(CultureInfo.InvariantCulture),
                                     PropertyDescription,
                                     static property => property.AsText(),
                                     out Text,
                                     out ErrorResponse);

        #endregion

        #region ParseMandatoryBoolean  (this CBOR, PropertyName/Key, PropertyDescription, out Boolean,   out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory boolean property.
        /// </summary>
        public static Boolean ParseMandatoryBoolean(this CBORValue                    CBOR,
                                                    String                            PropertyName,
                                                    String                            PropertyDescription,
                                                    out Boolean                       Value,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<Boolean>(CBOR,
                                                          CBORValue.FromText(PropertyName),
                                                          PropertyName,
                                                          PropertyDescription,
                                                          static property => property.AsBoolean(),
                                                          out var value,
                                                          out ErrorResponse);

            Value = value;
            return success;

        }


        /// <summary>
        /// Parse the given mandatory boolean property.
        /// </summary>
        public static Boolean ParseMandatoryBoolean(this CBORValue                    CBOR,
                                                    Int64                             PropertyKey,
                                                    String                            PropertyDescription,
                                                    out Boolean                       Value,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<Boolean>(CBOR,
                                                          CBORValue.FromInt64(PropertyKey),
                                                          PropertyKey.ToString(CultureInfo.InvariantCulture),
                                                          PropertyDescription,
                                                          static property => property.AsBoolean(),
                                                          out var value,
                                                          out ErrorResponse);

            Value = value;
            return success;

        }

        #endregion

        #region ParseOptionalBoolean   (this CBOR, PropertyName/Key, PropertyDescription, out Boolean,   out ErrorResponse)

        /// <summary>
        /// Parse the given optional boolean property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalBoolean(this CBORValue  CBOR,
                                                   String          PropertyName,
                                                   String          PropertyDescription,
                                                   out Boolean?    Value,
                                                   out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromText(PropertyName),
                                     PropertyName,
                                     PropertyDescription,
                                     static property => (Boolean?) property.AsBoolean(),
                                     out Value,
                                     out ErrorResponse);


        /// <summary>
        /// Parse the given optional boolean property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalBoolean(this CBORValue  CBOR,
                                                   Int64           PropertyKey,
                                                   String          PropertyDescription,
                                                   out Boolean?    Value,
                                                   out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromInt64(PropertyKey),
                                     PropertyKey.ToString(CultureInfo.InvariantCulture),
                                     PropertyDescription,
                                     static property => (Boolean?) property.AsBoolean(),
                                     out Value,
                                     out ErrorResponse);

        #endregion

        #region ParseMandatoryBytes    (this CBOR, PropertyName/Key, PropertyDescription, out Bytes,     out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory byte string property.
        /// </summary>
        public static Boolean ParseMandatoryBytes(this CBORValue                    CBOR,
                                                  String                            PropertyName,
                                                  String                            PropertyDescription,
                                                  [NotNullWhen(true)]  out Byte[]?  Bytes,
                                                  [NotNullWhen(false)] out String?  ErrorResponse)

            => ParseMandatoryInternal(CBOR,
                                      CBORValue.FromText(PropertyName),
                                      PropertyName,
                                      PropertyDescription,
                                      static property => property.AsBytes(),
                                      out Bytes,
                                      out ErrorResponse);


        /// <summary>
        /// Parse the given mandatory byte string property.
        /// </summary>
        public static Boolean ParseMandatoryBytes(this CBORValue                    CBOR,
                                                  Int64                             PropertyKey,
                                                  String                            PropertyDescription,
                                                  [NotNullWhen(true)]  out Byte[]?  Bytes,
                                                  [NotNullWhen(false)] out String?  ErrorResponse)

            => ParseMandatoryInternal(CBOR,
                                      CBORValue.FromInt64(PropertyKey),
                                      PropertyKey.ToString(CultureInfo.InvariantCulture),
                                      PropertyDescription,
                                      static property => property.AsBytes(),
                                      out Bytes,
                                      out ErrorResponse);

        #endregion

        #region ParseOptionalBytes     (this CBOR, PropertyName/Key, PropertyDescription, out Bytes,     out ErrorResponse)

        /// <summary>
        /// Parse the given optional byte string property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalBytes(this CBORValue  CBOR,
                                                 String          PropertyName,
                                                 String          PropertyDescription,
                                                 out Byte[]?     Bytes,
                                                 out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromText(PropertyName),
                                     PropertyName,
                                     PropertyDescription,
                                     static property => property.AsBytes(),
                                     out Bytes,
                                     out ErrorResponse);


        /// <summary>
        /// Parse the given optional byte string property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalBytes(this CBORValue  CBOR,
                                                 Int64           PropertyKey,
                                                 String          PropertyDescription,
                                                 out Byte[]?     Bytes,
                                                 out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromInt64(PropertyKey),
                                     PropertyKey.ToString(CultureInfo.InvariantCulture),
                                     PropertyDescription,
                                     static property => property.AsBytes(),
                                     out Bytes,
                                     out ErrorResponse);

        #endregion

        #region ParseMandatoryUInt64   (this CBOR, PropertyName/Key, PropertyDescription, out Value,     out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory unsigned integer property.
        /// </summary>
        public static Boolean ParseMandatoryUInt64(this CBORValue                    CBOR,
                                                   String                            PropertyName,
                                                   String                            PropertyDescription,
                                                   out UInt64                        Value,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<UInt64>(CBOR,
                                                         CBORValue.FromText(PropertyName),
                                                         PropertyName,
                                                         PropertyDescription,
                                                         static property => property.AsUInt64(),
                                                         out var value,
                                                         out ErrorResponse);

            Value = value;
            return success;

        }


        /// <summary>
        /// Parse the given mandatory unsigned integer property.
        /// </summary>
        public static Boolean ParseMandatoryUInt64(this CBORValue                    CBOR,
                                                   Int64                             PropertyKey,
                                                   String                            PropertyDescription,
                                                   out UInt64                        Value,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<UInt64>(CBOR,
                                                         CBORValue.FromInt64(PropertyKey),
                                                         PropertyKey.ToString(CultureInfo.InvariantCulture),
                                                         PropertyDescription,
                                                         static property => property.AsUInt64(),
                                                         out var value,
                                                         out ErrorResponse);

            Value = value;
            return success;

        }

        #endregion

        #region ParseOptionalUInt64    (this CBOR, PropertyName/Key, PropertyDescription, out Value,     out ErrorResponse)

        /// <summary>
        /// Parse the given optional unsigned integer property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalUInt64(this CBORValue  CBOR,
                                                  String          PropertyName,
                                                  String          PropertyDescription,
                                                  out UInt64?     Value,
                                                  out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromText(PropertyName),
                                     PropertyName,
                                     PropertyDescription,
                                     static property => (UInt64?) property.AsUInt64(),
                                     out Value,
                                     out ErrorResponse);


        /// <summary>
        /// Parse the given optional unsigned integer property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalUInt64(this CBORValue  CBOR,
                                                  Int64           PropertyKey,
                                                  String          PropertyDescription,
                                                  out UInt64?     Value,
                                                  out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromInt64(PropertyKey),
                                     PropertyKey.ToString(CultureInfo.InvariantCulture),
                                     PropertyDescription,
                                     static property => (UInt64?) property.AsUInt64(),
                                     out Value,
                                     out ErrorResponse);

        #endregion

        #region ParseMandatoryInt64    (this CBOR, PropertyName/Key, PropertyDescription, out Value,     out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory integer property.
        /// </summary>
        public static Boolean ParseMandatoryInt64(this CBORValue                    CBOR,
                                                  String                            PropertyName,
                                                  String                            PropertyDescription,
                                                  out Int64                         Value,
                                                  [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<Int64>(CBOR,
                                                        CBORValue.FromText(PropertyName),
                                                        PropertyName,
                                                        PropertyDescription,
                                                        static property => property.AsInt64(),
                                                        out var value,
                                                        out ErrorResponse);

            Value = value;
            return success;

        }


        /// <summary>
        /// Parse the given mandatory integer property.
        /// </summary>
        public static Boolean ParseMandatoryInt64(this CBORValue                    CBOR,
                                                  Int64                             PropertyKey,
                                                  String                            PropertyDescription,
                                                  out Int64                         Value,
                                                  [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<Int64>(CBOR,
                                                        CBORValue.FromInt64(PropertyKey),
                                                        PropertyKey.ToString(CultureInfo.InvariantCulture),
                                                        PropertyDescription,
                                                        static property => property.AsInt64(),
                                                        out var value,
                                                        out ErrorResponse);

            Value = value;
            return success;

        }

        #endregion

        #region ParseOptionalInt64     (this CBOR, PropertyName/Key, PropertyDescription, out Value,     out ErrorResponse)

        /// <summary>
        /// Parse the given optional integer property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalInt64(this CBORValue  CBOR,
                                                 String          PropertyName,
                                                 String          PropertyDescription,
                                                 out Int64?      Value,
                                                 out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromText(PropertyName),
                                     PropertyName,
                                     PropertyDescription,
                                     static property => (Int64?) property.AsInt64(),
                                     out Value,
                                     out ErrorResponse);


        /// <summary>
        /// Parse the given optional integer property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalInt64(this CBORValue  CBOR,
                                                 Int64           PropertyKey,
                                                 String          PropertyDescription,
                                                 out Int64?      Value,
                                                 out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromInt64(PropertyKey),
                                     PropertyKey.ToString(CultureInfo.InvariantCulture),
                                     PropertyDescription,
                                     static property => (Int64?) property.AsInt64(),
                                     out Value,
                                     out ErrorResponse);

        #endregion

        #region ParseMandatoryDecimal  (this CBOR, PropertyName/Key, PropertyDescription, out Value,     out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory decimal property:
        /// A plain integer, a bignum (tag 2/3) or a decimal fraction (tag 4).
        /// </summary>
        public static Boolean ParseMandatoryDecimal(this CBORValue                    CBOR,
                                                    String                            PropertyName,
                                                    String                            PropertyDescription,
                                                    out Decimal                       Value,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<Decimal>(CBOR,
                                                          CBORValue.FromText(PropertyName),
                                                          PropertyName,
                                                          PropertyDescription,
                                                          static property => property.AsDecimal(),
                                                          out var value,
                                                          out ErrorResponse);

            Value = value;
            return success;

        }


        /// <summary>
        /// Parse the given mandatory decimal property:
        /// A plain integer, a bignum (tag 2/3) or a decimal fraction (tag 4).
        /// </summary>
        public static Boolean ParseMandatoryDecimal(this CBORValue                    CBOR,
                                                    Int64                             PropertyKey,
                                                    String                            PropertyDescription,
                                                    out Decimal                       Value,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<Decimal>(CBOR,
                                                          CBORValue.FromInt64(PropertyKey),
                                                          PropertyKey.ToString(CultureInfo.InvariantCulture),
                                                          PropertyDescription,
                                                          static property => property.AsDecimal(),
                                                          out var value,
                                                          out ErrorResponse);

            Value = value;
            return success;

        }

        #endregion

        #region ParseOptionalDecimal   (this CBOR, PropertyName/Key, PropertyDescription, out Value,     out ErrorResponse)

        /// <summary>
        /// Parse the given optional decimal property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalDecimal(this CBORValue  CBOR,
                                                   String          PropertyName,
                                                   String          PropertyDescription,
                                                   out Decimal?    Value,
                                                   out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromText(PropertyName),
                                     PropertyName,
                                     PropertyDescription,
                                     static property => (Decimal?) property.AsDecimal(),
                                     out Value,
                                     out ErrorResponse);


        /// <summary>
        /// Parse the given optional decimal property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalDecimal(this CBORValue  CBOR,
                                                   Int64           PropertyKey,
                                                   String          PropertyDescription,
                                                   out Decimal?    Value,
                                                   out String?     ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromInt64(PropertyKey),
                                     PropertyKey.ToString(CultureInfo.InvariantCulture),
                                     PropertyDescription,
                                     static property => (Decimal?) property.AsDecimal(),
                                     out Value,
                                     out ErrorResponse);

        #endregion

        #region ParseMandatoryTimestamp(this CBOR, PropertyName/Key, PropertyDescription, out Timestamp, out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory date/time property:
        /// A tagged (0/1) or untagged RFC 3339 text or epoch timestamp.
        /// </summary>
        public static Boolean ParseMandatoryTimestamp(this CBORValue                    CBOR,
                                                      String                            PropertyName,
                                                      String                            PropertyDescription,
                                                      out DateTimeOffset                Timestamp,
                                                      [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<DateTimeOffset>(CBOR,
                                                                 CBORValue.FromText(PropertyName),
                                                                 PropertyName,
                                                                 PropertyDescription,
                                                                 ConvertDateTime,
                                                                 out var timestamp,
                                                                 out ErrorResponse);

            Timestamp = timestamp;
            return success;

        }


        /// <summary>
        /// Parse the given mandatory date/time property:
        /// A tagged (0/1) or untagged RFC 3339 text or epoch timestamp.
        /// </summary>
        public static Boolean ParseMandatoryTimestamp(this CBORValue                    CBOR,
                                                      Int64                             PropertyKey,
                                                      String                            PropertyDescription,
                                                      out DateTimeOffset                Timestamp,
                                                      [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<DateTimeOffset>(CBOR,
                                                                 CBORValue.FromInt64(PropertyKey),
                                                                 PropertyKey.ToString(CultureInfo.InvariantCulture),
                                                                 PropertyDescription,
                                                                 ConvertDateTime,
                                                                 out var timestamp,
                                                                 out ErrorResponse);

            Timestamp = timestamp;
            return success;

        }

        #endregion

        #region ParseOptionalTimestamp (this CBOR, PropertyName/Key, PropertyDescription, out Timestamp, out ErrorResponse)

        /// <summary>
        /// Parse the given optional date/time property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalTimestamp(this CBORValue       CBOR,
                                                     String               PropertyName,
                                                     String               PropertyDescription,
                                                     out DateTimeOffset?  Timestamp,
                                                     out String?          ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromText(PropertyName),
                                     PropertyName,
                                     PropertyDescription,
                                     static property => (DateTimeOffset?) ConvertDateTime(property),
                                     out Timestamp,
                                     out ErrorResponse);


        /// <summary>
        /// Parse the given optional date/time property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalTimestamp(this CBORValue       CBOR,
                                                     Int64                PropertyKey,
                                                     String               PropertyDescription,
                                                     out DateTimeOffset?  Timestamp,
                                                     out String?          ErrorResponse)

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromInt64(PropertyKey),
                                     PropertyKey.ToString(CultureInfo.InvariantCulture),
                                     PropertyDescription,
                                     static property => (DateTimeOffset?) ConvertDateTime(property),
                                     out Timestamp,
                                     out ErrorResponse);

        #endregion

        #region ParseMandatoryEnum     (this CBOR, PropertyName/Key, PropertyDescription, out Value,     out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory enumeration property,
        /// given as its text name or numeric value.
        /// </summary>
        public static Boolean ParseMandatoryEnum<TEnum>(this CBORValue                    CBOR,
                                                        String                            PropertyName,
                                                        String                            PropertyDescription,
                                                        out TEnum                         Value,
                                                        [NotNullWhen(false)] out String?  ErrorResponse)

            where TEnum : struct, Enum

        {

            var success = ParseMandatoryInternal<TEnum>(CBOR,
                                                        CBORValue.FromText(PropertyName),
                                                        PropertyName,
                                                        PropertyDescription,
                                                        ConvertEnum<TEnum>,
                                                        out var value,
                                                        out ErrorResponse);

            Value = value;
            return success;

        }


        /// <summary>
        /// Parse the given mandatory enumeration property,
        /// given as its text name or numeric value.
        /// </summary>
        public static Boolean ParseMandatoryEnum<TEnum>(this CBORValue                    CBOR,
                                                        Int64                             PropertyKey,
                                                        String                            PropertyDescription,
                                                        out TEnum                         Value,
                                                        [NotNullWhen(false)] out String?  ErrorResponse)

            where TEnum : struct, Enum

        {

            var success = ParseMandatoryInternal<TEnum>(CBOR,
                                                        CBORValue.FromInt64(PropertyKey),
                                                        PropertyKey.ToString(CultureInfo.InvariantCulture),
                                                        PropertyDescription,
                                                        ConvertEnum<TEnum>,
                                                        out var value,
                                                        out ErrorResponse);

            Value = value;
            return success;

        }

        #endregion

        #region ParseOptionalEnum      (this CBOR, PropertyName/Key, PropertyDescription, out Value,     out ErrorResponse)

        /// <summary>
        /// Parse the given optional enumeration property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalEnum<TEnum>(this CBORValue  CBOR,
                                                       String          PropertyName,
                                                       String          PropertyDescription,
                                                       out TEnum?      Value,
                                                       out String?     ErrorResponse)

            where TEnum : struct, Enum

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromText(PropertyName),
                                     PropertyName,
                                     PropertyDescription,
                                     static property => (TEnum?) ConvertEnum<TEnum>(property),
                                     out Value,
                                     out ErrorResponse);


        /// <summary>
        /// Parse the given optional enumeration property.
        /// A missing optional property is not an error!
        /// </summary>
        public static Boolean ParseOptionalEnum<TEnum>(this CBORValue  CBOR,
                                                       Int64           PropertyKey,
                                                       String          PropertyDescription,
                                                       out TEnum?      Value,
                                                       out String?     ErrorResponse)

            where TEnum : struct, Enum

            => ParseOptionalInternal(CBOR,
                                     CBORValue.FromInt64(PropertyKey),
                                     PropertyKey.ToString(CultureInfo.InvariantCulture),
                                     PropertyDescription,
                                     static property => (TEnum?) ConvertEnum<TEnum>(property),
                                     out Value,
                                     out ErrorResponse);

        #endregion

        #region ParseMandatoryCBOR     (this CBOR, PropertyName/Key, PropertyDescription, out Value,     out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory CBOR property without any conversion.
        /// </summary>
        public static Boolean ParseMandatoryCBOR(this CBORValue                    CBOR,
                                                 String                            PropertyName,
                                                 String                            PropertyDescription,
                                                 out CBORValue                     Value,
                                                 [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<CBORValue>(CBOR,
                                                            CBORValue.FromText(PropertyName),
                                                            PropertyName,
                                                            PropertyDescription,
                                                            static property => property,
                                                            out var value,
                                                            out ErrorResponse);

            Value = value;
            return success;

        }


        /// <summary>
        /// Parse the given mandatory CBOR property without any conversion.
        /// </summary>
        public static Boolean ParseMandatoryCBOR(this CBORValue                    CBOR,
                                                 Int64                             PropertyKey,
                                                 String                            PropertyDescription,
                                                 out CBORValue                     Value,
                                                 [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var success = ParseMandatoryInternal<CBORValue>(CBOR,
                                                            CBORValue.FromInt64(PropertyKey),
                                                            PropertyKey.ToString(CultureInfo.InvariantCulture),
                                                            PropertyDescription,
                                                            static property => property,
                                                            out var value,
                                                            out ErrorResponse);

            Value = value;
            return success;

        }

        #endregion

        #region ParseMandatoryArray    (this CBOR, PropertyName/Key, PropertyDescription, out Values,    out ErrorResponse)

        /// <summary>
        /// Parse the given mandatory CBOR array property.
        /// </summary>
        public static Boolean ParseMandatoryArray(this CBORValue                                     CBOR,
                                                  String                                             PropertyName,
                                                  String                                             PropertyDescription,
                                                  [NotNullWhen(true)]  out IReadOnlyList<CBORValue>? Values,
                                                  [NotNullWhen(false)] out String?                   ErrorResponse)

            => ParseMandatoryInternal(CBOR,
                                      CBORValue.FromText(PropertyName),
                                      PropertyName,
                                      PropertyDescription,
                                      static property => property.AsArray(),
                                      out Values,
                                      out ErrorResponse);


        /// <summary>
        /// Parse the given mandatory CBOR array property.
        /// </summary>
        public static Boolean ParseMandatoryArray(this CBORValue                                     CBOR,
                                                  Int64                                              PropertyKey,
                                                  String                                             PropertyDescription,
                                                  [NotNullWhen(true)]  out IReadOnlyList<CBORValue>? Values,
                                                  [NotNullWhen(false)] out String?                   ErrorResponse)

            => ParseMandatoryInternal(CBOR,
                                      CBORValue.FromInt64(PropertyKey),
                                      PropertyKey.ToString(CultureInfo.InvariantCulture),
                                      PropertyDescription,
                                      static property => property.AsArray(),
                                      out Values,
                                      out ErrorResponse);

        #endregion

    }

}
