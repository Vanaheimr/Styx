/*
 * Copyright (c) 2010-2018, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extention methods for JSON objects.
    /// </summary>
    public static class JSONObject
    {

        #region Create(params JProperties)

        /// <summary>
        /// Create a JSON object using the given JSON properties, but filter null values.
        /// </summary>
        /// <param name="JProperties">JSON properties.</param>
        public static JObject Create(params JProperty[] JProperties)
        {

            if (JProperties == null)
                return new JObject();

            var data = JProperties.Where(jproperty => jproperty != null).ToArray();

            return data.Length > 0 ? new JObject(data) : new JObject();

        }

        #endregion

        #region Create(JProperties)

        /// <summary>
        /// Create a JSON object using the given JSON properties, but filter null values.
        /// </summary>
        /// <param name="JProperties">JSON properties.</param>
        public static JObject Create(IEnumerable<JProperty> JProperties)
        {

            if (JProperties == null)
                return new JObject();

            var data = JProperties.Where(jproperty => jproperty != null).ToArray();

            return data.Length > 0 ? new JObject(data) : new JObject();

        }

        #endregion

    }

    /// <summary>
    /// Extention methods for JSON arrays.
    /// </summary>
    public static class JSONArray
    {

        #region Create(params JObjects)

        /// <summary>
        /// Create a JSON array using the given JSON objects, but filter null values.
        /// </summary>
        /// <param name="JObjects">JSON objects.</param>
        public static JArray Create(params JObject[] JObjects)
        {

            if (JObjects == null)
                return new JArray();

            var data = JObjects.Where(jobject => jobject != null).ToArray();

            return data.Length > 0 ? new JArray(data) : new JArray();

        }

        #endregion

        #region Create(JObjects)

        /// <summary>
        /// Create a JSON array using the given JSON objects, but filter null values.
        /// </summary>
        /// <param name="JObjects">JSON objects.</param>
        public static JArray Create(IEnumerable<JObject> JObjects)
        {

            if (JObjects == null)
                return new JArray();

            var data = JObjects.Where(jobject => jobject != null).ToArray();

            return data.Length > 0 ? new JArray(data) : new JArray();

        }

        #endregion

    }

    /// <summary>
    /// Extention methods for JSON representations of common classes.
    /// </summary>
    public static class JSONExtentions
    {

        #region ToUTF8Bytes(this JSONArray)

        public static Byte[] ToUTF8Bytes(this JArray JSONArray)
        {

            if (JSONArray == null)
                return new Byte[0];

            return Encoding.UTF8.GetBytes(JSONArray.ToString());

        }

        #endregion

        #region ToUTF8Bytes(this JSONObject)

        public static Byte[] ToUTF8Bytes(this JObject JSONObject)
        {

            if (JSONObject == null)
                return new Byte[0];

            return Encoding.UTF8.GetBytes(JSONObject.ToString());

        }

        #endregion


        #region ToJSON(this Decimal, JPropertyKey)

        /// <summary>
        /// Create a Iso8601 representation of the given DateTime.
        /// </summary>
        /// <param name="Decimal">A decimal.</param>
        /// <param name="JPropertyKey">The name of the JSON property key.</param>
        public static JProperty ToJSON(this Decimal Decimal, String JPropertyKey)

            => new JProperty(JPropertyKey, Decimal.ToString());

        #endregion


        #region ToJSON(this Timestamp, JPropertyKey)

        /// <summary>
        /// Create a Iso8601 representation of the given DateTime.
        /// </summary>
        /// <param name="Timestamp">A timestamp.</param>
        /// <param name="JPropertyKey">The name of the JSON property key.</param>
        public static JProperty ToJSON(this DateTime Timestamp, String JPropertyKey)

            => new JProperty(JPropertyKey, Timestamp.ToIso8601());

        #endregion

        #region ToJSON(this TimestampedT)

        /// <summary>
        /// Create a JSON representation of the given timestamped value.
        /// </summary>
        /// <param name="TimestampedT">A timestamped value.</param>
        public static JObject ToJSON<T>(this Timestamped<T> TimestampedT)

            => new JObject(
                   new JProperty("Timestamp", TimestampedT.Timestamp.ToIso8601()),
                   new JProperty("Status",    TimestampedT.Value.    ToString())
               );

        #endregion


        #region ToJSON(this I18NString)

        /// <summary>
        /// Create a JSON representation of the given internationalized string.
        /// </summary>
        /// <param name="I18NString">An internationalized string.</param>
        public static JObject ToJSON(this I18NString I18NString)
        {

            if (I18NString == null || !I18NString.Any())
                return new JObject();

            return new JObject(I18NString.SafeSelect(i18n => new JProperty(i18n.Language.ToString(), i18n.Text)));

        }

        #endregion

        #region ToJSON(this I18NString, JPropertyKey)

        /// <summary>
        /// Create a JSON representation of the given internationalized string.
        /// </summary>
        /// <param name="I18NString">An internationalized string.</param>
        /// <param name="JPropertyKey">The name of the JSON property key.</param>
        public static JProperty ToJSON(this I18NString I18NString, String JPropertyKey)
        {

            if (I18NString == null || !I18NString.Any())
                return null;

            return new JProperty(JPropertyKey, I18NString.ToJSON());

        }

        #endregion

        #region ParseI18NString(this JToken)

        public static I18NString ParseI18NString(this JToken JToken)
        {

            var jobject = JToken as JObject;

            if (jobject == null)
                throw new ArgumentException("The given JSON token is not a JSON object!", nameof(JToken));

            return jobject.ParseI18NString();

        }

        #endregion

        #region ParseI18NString(this JObject)

        public static I18NString ParseI18NString(this JObject JObject)
        {

            var i18NString = I18NString.Empty;

            foreach (var jproperty in JObject)
                i18NString.Add((Languages) Enum.Parse(typeof(Languages), jproperty.Key),
                               jproperty.Value.Value<String>());

            return i18NString;

        }

        #endregion

        #region ParseI18NString(this JObject, PropertyKey)

        public static I18NString ParseI18NString(this JObject JObject, String PropertyKey)
        {

            if (PropertyKey.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(PropertyKey), "The given property key must not be null or empty!");

            var jobject = JObject[PropertyKey] as JObject;

            if (jobject == null)
                return I18NString.Empty;

            var i18NString = I18NString.Empty;

            foreach (var jproperty in jobject)
                i18NString.Add((Languages) Enum.Parse(typeof(Languages), jproperty.Key),
                               jproperty.Value.Value<String>());

            return i18NString;

        }

        #endregion

        #region TryParseI18NString(this JToken,  out I18NString)

        public static Boolean TryParseI18NString(this JToken JToken, out I18NString I18NString)
        {

            var jobject = JToken as JObject;

            if (jobject == null)
            {
                I18NString = null;
                return false;
            }

            return jobject.TryParseI18NString(out I18NString);

        }

        #endregion

        #region TryParseI18NString(this JObject, out I18NString)

        public static Boolean TryParseI18NString(this JObject JObject, out I18NString I18NString)
        {

            I18NString = I18NString.Empty;

            try
            {

                foreach (var jproperty in JObject)
                    I18NString.Add((Languages)Enum.Parse(typeof(Languages), jproperty.Key),
                                   jproperty.Value.Value<String>());

                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion

        #region TryParseI18NString(this JObject, PropertyKey, out I18NString)

        public static Boolean TryParseI18NString(this JObject JObject, String PropertyKey, out I18NString I18NString)
        {

            if (PropertyKey.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(PropertyKey), "The given property key must not be null or empty!");

            I18NString = I18NString.Empty;
            var jobject = JObject[PropertyKey] as JObject;

            if (jobject == null)
                throw new ArgumentException("The value of the given JSON property '" + PropertyKey + "' is not a JSON object!", nameof(JObject));

            try
            {

                foreach (var jproperty in JObject)
                    I18NString.Add((Languages)Enum.Parse(typeof(Languages), jproperty.Key),
                                   jproperty.Value.Value<String>());

                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion


        #region ValueOrDefault(this ParentJObject, PropertyName, DefaultValue = null)

        /// <summary>
        /// Return the value of the JSON property or the given default value.
        /// </summary>
        /// <param name="ParentJObject">The JSON parent object.</param>
        /// <param name="PropertyName">The property name to match.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static JToken ValueOrDefault(this JObject  ParentJObject,
                                            String        PropertyName,
                                            String        DefaultValue = null)
        {

            #region Initial checks

            if (ParentJObject == null)
                return DefaultValue;

            #endregion

            JToken JSONValue = null;

            if (ParentJObject.TryGetValue(PropertyName, out JSONValue))
                return JSONValue;

            return DefaultValue;

        }

        #endregion

        #region ValueOrFail   (this ParentJObject, PropertyName, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the JSON property or the given default value.
        /// </summary>
        /// <param name="ParentJObject">The JSON parent object.</param>
        /// <param name="PropertyName">The property name to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static JToken ValueOrFail(this JObject  ParentJObject,
                                         String        PropertyName,
                                         String        ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentJObject == null)
                throw new ArgumentNullException(nameof(ParentJObject),  "The given JSON object must not be null!");

            #endregion

            JToken JSONValue = null;

            if (ParentJObject.TryGetValue(PropertyName, out JSONValue))
                return JSONValue;

            throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given JSON property does not exist!");

        }

        #endregion


        #region MapValueOrDefault(ParentJObject, PropertyName, ValueMapper, DefaultValue = null)

        /// <summary>
        /// Return the mapped value of the JSON property or the given default value.
        /// </summary>
        /// <param name="ParentJObject">The JSON parent object.</param>
        /// <param name="PropertyName">The property name to match.</param>
        /// <param name="ValueMapper">A delegate to map the JSON property value.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static T MapValueOrDefault<T>(this JObject     ParentJObject,
                                             String           PropertyName,
                                             Func<JToken, T>  ValueMapper,
                                             T                DefaultValue = default(T))
        {

            #region Initial checks

            if (ParentJObject == null)
                return DefaultValue;

            #endregion

            JToken JSONValue;

            if (ParentJObject.TryGetValue(PropertyName, out JSONValue))
            {

                try
                {
                    return ValueMapper(JSONValue);
                }
#pragma warning disable RCS1075  // Avoid empty catch clause that catches System.Exception.
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch (Exception)
#pragma warning restore RECS0022
#pragma warning restore RCS1075
                { }

            }

            return DefaultValue;

        }

        #endregion

        #region MapValueOrFail   (ParentJObject, PropertyName, ValueMapper, ExceptionMessage = null)

        /// <summary>
        /// Return the mapped value of the JSON property or throw an exception
        /// having the given optional message.
        /// </summary>
        /// <param name="ParentJObject">The JSON parent object.</param>
        /// <param name="PropertyName">The property name to match.</param>
        /// <param name="ValueMapper">A delegate to map the JSON property value.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static T MapValueOrFail<T>(this JObject     ParentJObject,
                                          String           PropertyName,
                                          Func<JToken, T>  ValueMapper,
                                          String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentJObject == null)
                throw new ArgumentNullException(nameof(ParentJObject),  "The given JSON object must not be null!");

            if (ValueMapper == null)
                throw new ArgumentNullException(nameof(ValueMapper),    "The given JSON value mapper delegate must not be null!");

            #endregion

            JToken JSONValue;

            if (ParentJObject.TryGetValue(PropertyName, out JSONValue))
            {

                try
                {
                    return ValueMapper(JSONValue);
                }
                catch (Exception e)
                {
                    throw ExceptionMessage.IsNotNullOrEmpty() ? new Exception(ExceptionMessage) : e;
                }

            }

            throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given JSON property does not exist!");

        }

        #endregion

    }

}
