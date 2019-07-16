/*
 * Copyright (c) 2010-2019, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Hermod <http://www.github.com/Vanaheimr/Hermod>
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

using System;
using System.Linq;
using System.Xml.Linq;
using System.Globalization;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.ConsoleLog;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public delegate Boolean TryParser        <TResult>(String  Input, out TResult arg);
    public delegate Boolean TryJObjectParser <TResult>(JObject Input, out TResult arg, out String ErrorResponse);

    /// <summary>
    /// Extention methods to parse JSON.
    /// </summary>
    public static class JSONExtentions
    {

        #region Contains             (this JSON, PropertyName)

        public static Boolean Contains(this JObject  JSON,
                                       String        PropertyName)
        {

            if (JSON == null || PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
                return false;

            return JSON[PropertyName] != null;

        }

        #endregion

        #region GetString            (this JSON, PropertyName)

        public static String GetString(this JObject  JSON,
                                       String        PropertyName)
        {

            if (JSON == null || PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
                return null;

            return JSON[PropertyName]?.Value<String>();

        }

        #endregion


        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Text,                   out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out String    Text,
                                             out String    ErrorResponse)
        {

            Text = String.Empty;

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                if (JSONToken.ToString() == "{}")
                {
                    Text           = null;
                    ErrorResponse  = null;
                    return true;
                }

                Text = JSONToken?.Value<String>();

            }
            catch (Exception)
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory<T>    (this JSON, PropertyName, PropertyDescription,                    Mapper,    out Value,                  out ErrorResponse)

        public static Boolean ParseMandatory<T>(this JObject     JSON,
                                                String           PropertyName,
                                                String           PropertyDescription,
                                                Func<String, T>  Mapper,
                                                out T            Value,
                                                out String       ErrorResponse)
        {

            Value = default(T);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (Mapper == null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                var JSONValue = JSONToken?.Value<String>()?.Trim();

                if (JSONValue.IsNeitherNullNorEmpty())
                {
                    Value          = Mapper(JSONValue);
                    ErrorResponse  = null;
                    return true;
                }

            }
#pragma warning disable RCS1075  // Avoid empty catch clause that catches System.Exception.
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch (Exception)
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning restore RCS1075  // Avoid empty catch clause that catches System.Exception.
            { }

            Value          = default(T);
            ErrorResponse  = "Invalid " + PropertyDescription ?? PropertyName + "!";
            return false;

        }

        public static Boolean ParseMandatory<T>(this JObject     JSON,
                                                String           PropertyName,
                                                String           PropertyDescription,
                                                Func<String, T>  Mapper,
                                                out T?           Value,
                                                out String       ErrorResponse)

            where T : struct

        {

            Value = null;

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (Mapper == null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                var JSONValue = JSONToken?.Value<String>()?.Trim();

                if (JSONValue.IsNeitherNullNorEmpty())
                {
                    Value          = Mapper(JSONValue);
                    ErrorResponse  = null;
                    return true;
                }

            }
#pragma warning disable RCS1075  // Avoid empty catch clause that catches System.Exception.
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch (Exception)
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning restore RCS1075  // Avoid empty catch clause that catches System.Exception.
            { }

            Value          = null;
            ErrorResponse  = "Invalid " + PropertyDescription ?? PropertyName + "!";
            return false;

        }

        #endregion

        #region ParseMandatory<T>    (this JSON, PropertyName, PropertyDescription,                    TryParser, out Value,                  out ErrorResponse)

        public static Boolean ParseMandatory<T>(this JObject  JSON,
                                                String        PropertyName,
                                                String        PropertyDescription,
                                                TryParser<T>  TryParser,
                                                out T         Value,
                                                out String    ErrorResponse)
        {

            Value = default(T);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser == null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty() &&
                //!TryParser(JSONValue, out Value))
                TryParser(JSONValue, out Value))
            {
                ErrorResponse = null;
                return true;
            }

            Value          = default(T);
            ErrorResponse  = "Invalid " + PropertyDescription ?? PropertyName + "!";
            return false;

        }

        public static Boolean ParseMandatory<T>(this JObject  JSON,
                                                String        PropertyName,
                                                String        PropertyDescription,
                                                TryParser<T>  TryParser,
                                                out T?        Value,
                                                out String    ErrorResponse)

            where T : struct

        {

            Value = null;

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser == null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty() &&
                //!TryParser(JSONValue, out T _Value))
                TryParser(JSONValue, out T _Value))
            {
                Value          = _Value;
                ErrorResponse  = null;
                return true;
            }

            Value          = null;
            ErrorResponse  = "Invalid " + PropertyDescription ?? PropertyName + "!";
            return false;

        }

        #endregion

        #region ParseMandatory<T>    (this JSON, PropertyName, PropertyDescription,                    TryJObjectParser, out Value,                  out ErrorResponse)

        public static Boolean ParseMandatory<T>(this JObject         JSON,
                                                String               PropertyName,
                                                String               PropertyDescription,
                                                TryJObjectParser<T>  TryJObjectParser,
                                                out T                Value,
                                                out String           ErrorResponse)
        {

            Value = default(T);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryJObjectParser == null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "' (" + PropertyDescription + ")!";
                return false;
            }

            if (!(JSONToken is JObject JSONValue))
            {
                ErrorResponse = "Invalid JSON object '" + PropertyName + "' (" + PropertyDescription + ")!";
                return false;
            }

            if (!TryJObjectParser(JSONValue, out T _Value, out String ErrorResponse2))
            {
                Value         = default(T);
                ErrorResponse = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed: " + ErrorResponse2;
                return false;
            }

            Value         = _Value;
            ErrorResponse = null;
            return true;

        }

        public static Boolean ParseMandatory<T>(this JObject         JSON,
                                                String               PropertyName,
                                                String               PropertyDescription,
                                                TryJObjectParser<T>  TryJObjectParser,
                                                out T?               Value,
                                                out String           ErrorResponse)

            where T : struct

        {

            Value = null;

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryJObjectParser == null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "' (" + PropertyDescription + ")!";
                return false;
            }

            if (!(JSONToken is JObject JSONValue))
            {
                ErrorResponse = "Invalid JSON object '" + PropertyName + "' (" + PropertyDescription + ")!";
                return false;
            }

            if (!TryJObjectParser(JSONValue, out T _Value, out String ErrorResponse2))
            {
                Value         = null;
                ErrorResponse = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed: " + ErrorResponse2;
                return false;
            }

            Value         = _Value;
            ErrorResponse = null;
            return true;
 
        }

        #endregion


        #region ParseMandatoryEnum<TEnum>(this JSON, PropertyName, PropertyDescription,                               out EnumValue,              out ErrorResponse)

        public static Boolean ParseMandatoryEnum<TEnum>(this JObject  JSON,
                                                        String        PropertyName,
                                                        String        PropertyDescription,
                                                        out TEnum     EnumValue,
                                                        out String    ErrorResponse)

             where TEnum : struct

        {

            EnumValue = default(TEnum);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken == null ||
                !Enum.TryParse(JSONToken.Value<String>(), true, out EnumValue))
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion


        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Boolean,                out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Boolean   BooleanValue,
                                             out String    ErrorResponse)
        {

            BooleanValue = default(Boolean);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken == null)
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            try
            {
                BooleanValue = JSONToken.Value<Boolean>();
            }
            catch (Exception e)
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Single,                 out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Single    SingleValue,
                                             out String    ErrorResponse)
        {

            SingleValue = default(Single);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken == null ||
                !Single.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out SingleValue))
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Double,                 out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Double    DoubleValue,
                                             out String    ErrorResponse)
        {

            DoubleValue = default(Double);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken == null ||
                !Double.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out DoubleValue))
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Decimal,                out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Decimal   DecimalValue,
                                             out String    ErrorResponse)
        {

            DecimalValue = default(Decimal);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken == null ||
                !Decimal.TryParse(JSONToken.Value<String>(), out DecimalValue))
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Byte,                   out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Byte     ByteValue,
                                             out String    ErrorResponse)
        {

            ByteValue = default(Byte);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken == null ||
                !Byte.TryParse(JSONToken.Value<String>(), out ByteValue))
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out SByte,                  out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out SByte     SByteValue,
                                             out String    ErrorResponse)
        {

            SByteValue = default(SByte);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken == null ||
                !SByte.TryParse(JSONToken.Value<String>(), out SByteValue))
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Int32,                  out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Int32     Int32Value,
                                             out String    ErrorResponse)
        {

            Int32Value = default(Int32);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken == null ||
                !Int32.TryParse(JSONToken.Value<String>(), out Int32Value))
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Int64,                  out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Int64     Int64Value,
                                             out String    ErrorResponse)
        {

            Int64Value = default(Int64);

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken == null ||
                !Int64.TryParse(JSONToken.Value<String>(), out Int64Value))
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Timestamp,              out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out DateTime  Timestamp,
                                             out String    ErrorResponse)

        {

            Timestamp = DateTime.MinValue;

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                Timestamp = JSONToken.Value<DateTime>();

                if (Timestamp.Kind != DateTimeKind.Utc)
                    Timestamp = Timestamp.ToUniversalTime();

            }
            catch (Exception)
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out I18NText,               out ErrorResponse)

        public static Boolean ParseMandatory(this JObject    JSON,
                                             String          PropertyName,
                                             String          PropertyDescription,
                                             out I18NString  I18NText,
                                             out String      ErrorResponse)

        {

            I18NText = I18NString.Empty;

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            var i18nJSON = JSONToken as JObject;

            if (i18nJSON == null)
            {
                ErrorResponse = "Invalid i18n JSON string provided!";
                return false;
            }

            var i18NString = I18NString.Empty;

            foreach (var i18nProperty in i18nJSON)
            {

                try
                {

                    i18NString.Add((Languages) Enum.Parse(typeof(Languages), i18nProperty.Key),
                                   i18nProperty.Value.Value<String>());

                } catch (Exception)
                {
                    ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                    return false;
                }

            }

            ErrorResponse = null;
            I18NText      = i18NString;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out JObject,                out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out JObject   JObject,
                                             out String    ErrorResponse)
        {

            JObject = null;

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                JObject = JSONToken as JObject;

            }
            catch (Exception)
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out JArray,                 out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out JArray    JArray,
                                             out String    ErrorResponse)
        {

            JArray = null;

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                JArray = JSONToken as JArray;

            }
            catch (Exception)
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                          out StringArray,                 out ErrorResponse)

        public static Boolean ParseMandatory(this JObject             JSON,
                                             String                   PropertyName,
                                             String                   PropertyDescription,
                                             out IEnumerable<String>  StringArray,
                                             out String               ErrorResponse)
        {

            StringArray = null;

            if (JSON == null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                if (!(JSONToken is JArray JArray))
                {
                    ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                    return false;
                }

                StringArray = JArray.SafeSelect(item => item.Value<String>()).ToArray();

            }
            catch (Exception)
            {
                ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion


        // Mandatory multiple values...

        #region GetMandatory(this JSON, Key, out Values)

        public static Boolean GetMandatory(this JObject             JSON,
                                           String                   Key,
                                           out IEnumerable<String>  Values)
        {

            if (JSON.TryGetValue(Key, out JToken JSONToken) && 
                JSONToken is JArray _Values)
            {

                Values = _Values.AsEnumerable().
                                 Select(jtoken => jtoken.Value<String>()).
                                 Where (value  => value != null);

                return true;

            }

            Values = null;
            return false;

        }

        #endregion




        public static Boolean ParseMandatory<T>(this JObject      JSON,
                                                String            PropertyName,
                                                Func<String, T>   Mapper,
                                                T                 InvalidResult,
                                                out T             TOut)
        {

            if (JSON == null ||
                PropertyName.IsNullOrEmpty() ||
                Mapper == null)
            {
                TOut = default(T);
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken _JToken) && _JToken?.Value<String>() != null)
            {

                try
                {

                    TOut = Mapper(_JToken?.Value<String>());

                    return !TOut.Equals(InvalidResult);

                }
#pragma warning disable RCS1075  // Avoid empty catch clause that catches System.Exception.
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch (Exception)
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning restore RCS1075  // Avoid empty catch clause that catches System.Exception.
                { }

            }

            TOut = default(T);
            return false;

        }




        // -------------------------------------------------------------------------------------------------------------------------------------
        // Parse Optional
        // -------------------------------------------------------------------------------------------------------------------------------------

        #region ParseOptional       (this JSON, PropertyName,                                                      out StringValue,        out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            out String    StringValue,
                                            out String    ErrorResponse)
        {

            StringValue    = String.Empty;
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                StringValue = JSONToken?.Value<String>();

                // "properyKey": null -> will be ignored!
                if (StringValue == null)
                    return false;

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                                 out BooleanValue,       out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Boolean?  BooleanValue,
                                            out String    ErrorResponse)
        {

            BooleanValue   = new Boolean?();
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                BooleanValue = JSONToken?.Value<Boolean>();

                // "properyKey": null -> will be ignored!
                if (BooleanValue == null)
                    return false;

                return true;

            }

            return false;

        }

        #endregion


        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                    Mapper, out Value,                   out ErrorResponse)

        public static Boolean ParseOptional<T>(this JObject     JSON,
                                               String           PropertyName,
                                               String           PropertyDescription,
                                               Func<String, T>  Mapper,
                                               out T            Value,
                                               out String       ErrorResponse)
        {

            Value          = default(T);
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                try
                {

                    Value = JSONToken.Type == JTokenType.String
                                         ? Mapper(JSONToken.Value<String>())
                                         : Mapper(JSONToken.ToString());

                }
                catch (Exception)
                {
                    ErrorResponse = "Invalid " + PropertyDescription + "!";
                }

                return true;

            }

            return false;

        }

        #endregion


        #region ParseOptionalStruct<TStruct?>(this JSON, PropertyName, PropertyDescription,                    Parser, out Value,              out ErrorResponse)

        public static Boolean ParseOptionalStruct<TStruct>(this JObject        JSON,
                                                           String              PropertyName,
                                                           String              PropertyDescription,
                                                           TryParser<TStruct>  Parser,
                                                           out TStruct?        Value,
                                                           out String          ErrorResponse)

            where TStruct : struct

        {

            Value = new TStruct?();

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken) && JSONToken != null)
            {

                var JSONValue = JSON[PropertyName];

                if (JSONValue == null)
                {
                    ErrorResponse = null;
                    return true;
                }

                if ((JSONToken as JObject)?.Count == 0)
                {
                    ErrorResponse = null;
                    return true;
                }

                if ((JSONToken as JArray)?.Count == 0)
                {
                    ErrorResponse = null;
                    return true;
                }

                if (!Parser(JSONValue.ToString().Trim(), out TStruct TValue))
                {
                    ErrorResponse =  "Unknown " + PropertyDescription + "!";
                    Value         = new TStruct?();
                    return false;
                }

                Value = new TStruct?(TValue);

            }

            ErrorResponse = null;
            return true;

        }

        #endregion


        #region ParseOptional<T>        (this JSON, PropertyName, PropertyDescription,                    Parser, out Value,              out HTTPResponse)

        public static Boolean ParseOptional<T>(this JObject      JSON,
                                               String            PropertyName,
                                               String            PropertyDescription,
                                               TryParser<T>      Parser,
                                               out T             Value,
                                               out String        ErrorResponse)
        {

            Value          = default(T);
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                if (!Parser(JSONToken.Type == JTokenType.String
                                ? JSONToken.Value<String>()
                                : JSONToken.ToString(),
                            out Value))
                {

                    Value          = default(T);
                    ErrorResponse  = "The value '" + JSONToken + "' is not valid for JSON property '" + PropertyDescription + "!";

                }

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional<T>        (this JSON, PropertyName, PropertyDescription,             JObjectParser, out Value,              out HTTPResponse)

        public static Boolean ParseOptional<T>(this JObject         JSON,
                                               String               PropertyName,
                                               String               PropertyDescription,
                                               TryJObjectParser<T>  JObjectParser,
                                               out T                Value,
                                               out String           ErrorResponse)
        {

            Value          = default(T);
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                if (!(JSONToken is JObject JSON2))
                    ErrorResponse  = "Invalid " + PropertyDescription + "!";

                else if (!JObjectParser(JSON2, out Value, out String ErrorResponse2))
                    ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed: " + ErrorResponse2;

                return true;

            }

            return false;

        }

        #endregion


        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out I18NText,                out ErrorResponse)

        public static Boolean ParseOptional(this JObject    JSON,
                                            String          PropertyName,
                                            String          PropertyDescription,
                                            out I18NString  I18NText,
                                            out String      ErrorResponse)

        {

            I18NText       = I18NString.Empty;
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                if (!(JSONToken is JObject i18NJSON))
                {
                    ErrorResponse = "Invalid " + PropertyDescription + "!";
                    return true;
                }

                foreach (var jproperty in i18NJSON)
                {

                    try
                    {

                        I18NText.Add((Languages) Enum.Parse(typeof(Languages), jproperty.Key),
                                     jproperty.Value.Value<String>());

                    }
                    catch (Exception)
                    {
                        ErrorResponse = "Invalid " + PropertyDescription + "!";
                        return true;
                    }

                }

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional<TEnum>(this JSON, PropertyName, PropertyDescription,                            out EnumValue,               out ErrorResponse)

        public static Boolean ParseOptional<TEnum>(this JObject  JSON,
                                                   String        PropertyName,
                                                   String        PropertyDescription,
                                                   out TEnum?    EnumValue,
                                                   out String    ErrorResponse)

            where TEnum : struct

        {

            EnumValue      = null;
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                var JSONValue = JSONToken?.Value<String>();
                if (JSONValue == null)
                {
                    ErrorResponse  = "Unknown " + PropertyDescription + "!";
                    EnumValue      = null;
                }

                if (Enum.TryParse(JSONValue, true, out TEnum enumValue))
                {
                    EnumValue      = enumValue;
                    ErrorResponse  = null;
                }

                else
                {
                    ErrorResponse  = "Invalid " + PropertyDescription + "!";
                    EnumValue      = null;
                }

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out Timestamp,               out ErrorResponse)

        public static Boolean ParseOptional(this JObject   JSON,
                                            String         PropertyName,
                                            String         PropertyDescription,
                                            out DateTime?  Timestamp,
                                            out String     ErrorResponse)
        {

            Timestamp      = null;
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                try
                {
                    Timestamp = JSONToken.Value<DateTime>();
                }
                catch (Exception)
                {
                    ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                }

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out TimeSpan,                out ErrorResponse)

        public static Boolean ParseOptional(this JObject   JSON,
                                            String         PropertyName,
                                            String         PropertyDescription,
                                            out TimeSpan?  Timespan,
                                            out String     ErrorResponse)
        {

            Timespan       = null;
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                try
                {
                    Timespan = TimeSpan.FromSeconds(JSONToken.Value<UInt32>());
                }
                catch (Exception)
                {
                    ErrorResponse = "Invalid " + PropertyDescription ?? PropertyName + "!";
                    return false;
                }

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out JSONObject,              out ErrorResponse)

        public static Boolean ParseOptional(this JObject    JSON,
                                            String          PropertyName,
                                            String          PropertyDescription,
                                            out JObject     JSONObject,
                                            out String      ErrorResponse)

        {

            JSONObject     = new JObject();
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                JSONObject = JSONToken as JObject;

                if (JSONObject == null)
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON object!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out JSONArray,               out ErrorResponse)

        public static Boolean ParseOptional(this JObject    JSON,
                                            String          PropertyName,
                                            String          PropertyDescription,
                                            out JArray      JSONArray,
                                            out String      ErrorResponse)

        {

            JSONArray      = new JArray();
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                JSONArray = JSONToken as JArray;

                if (JSONArray == null)
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptionalHashSet(this JSON, PropertyName, PropertyDescription,                    Parser, out HashSet,                 out ErrorResponse)

        public static Boolean ParseOptionalHashSet<T>(this JObject    JSON,
                                                      String          PropertyName,
                                                      String          PropertyDescription,
                                                      TryParser<T>    Parser,
                                                      out HashSet<T>  HashSet,
                                                      out String      ErrorResponse)

        {

            HashSet        = null;
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                if (!(JSONToken is JArray JSONArray))
                {
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
                    return true;
                }

                var item = "";
                HashSet = new HashSet<T>();

                foreach (var element in JSONArray)
                {

                    if (element == null)
                    {
                        ErrorResponse = "A given value within the array is null!";
                        return true;
                    }

                    item = element.Value<String>();
                    if (item != null)
                        item = item.Trim();

                    if (item.IsNullOrEmpty())
                    {
                        ErrorResponse = "A given value within the array is null or empty!";
                        return true;
                    }

                    if (Parser(item, out T itemT))
                        HashSet.Add(itemT);

                }

                return true;

            }

            return false;

        }

        #endregion


        #region GetOptional(this JSON, Key)

        public static String GetOptional(this JObject  JSON,
                                         String        PropertyName)
        {

            if (JSON == null)
                return String.Empty;

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
                return JSONToken.Value<String>();

            return String.Empty;

        }

        #endregion

        #region GetOptional(this JSON, PropertyName, out Values)

        public static Boolean GetOptional(this JObject             JSON,
                                          String                   PropertyName,
                                          out IEnumerable<String>  Values,
                                          out String               ErrorResponse)
        {

            Values         = new String[0];
            ErrorResponse  = null;

            if (JSON == null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty() || PropertyName.Trim().IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out JToken JSONToken))
            {

                // "properyKey": null -> will be ignored!
                if (JSONToken == null)
                    return false;

                if (!(JSONToken is JArray JSONArray))
                {
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
                    return true;
                }

                try
                {

                    Values = JSONArray.AsEnumerable().
                                 Select(jtoken => jtoken?.Value<String>()).
                                 Where (value  => value != null);

                    return true;

                }
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
                catch (Exception)
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.
                { }

                return true;

            }

            return false;

        }

        #endregion


    }

}
