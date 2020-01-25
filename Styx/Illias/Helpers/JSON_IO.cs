/*
 * Copyright (c) 2010-2020, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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


        #region Create(params JProperties)

        /// <summary>
        /// Create a JSON array using the given JSON properties, but filter null values.
        /// </summary>
        /// <param name="JProperties">JSON properties.</param>
        public static JArray Create(params JProperty[] JProperties)
        {

            if (JProperties == null)
                return new JArray();

            var data = JProperties.Where(jproperty => jproperty != null).ToArray();

            return data.Length > 0 ? new JArray(data) : new JArray();

        }

        #endregion

        #region Create(JProperties)

        /// <summary>
        /// Create a JSON array using the given JSON properties, but filter null values.
        /// </summary>
        /// <param name="JProperties">JSON properties.</param>
        public static JArray Create(IEnumerable<JProperty> JProperties)
        {

            if (JProperties == null)
                return new JArray();

            var data = JProperties.Where(jproperty => jproperty != null).ToArray();

            return data.Length > 0 ? new JArray(data) : new JArray();

        }

        #endregion

    }

    /// <summary>
    /// Extention methods for JSON representations of common classes.
    /// </summary>
    public static class JSONExtentionsOld
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
        /// Return a JSON representation of the given timestamped value.
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
        /// Return a JSON representation of the given internationalized string.
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
        /// Return a JSON representation of the given internationalized string.
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


        #region ToJSON(this Id, JPropertyKey)

        /// <summary>
        /// Return a JSON representation of the given identificator.
        /// </summary>
        /// <param name="Id">An identificator.</param>
        /// <param name="JPropertyKey">The name of the JSON property key to use.</param>
        public static JProperty ToJSON(this IId Id, String JPropertyKey)

            => Id != null
                   ? new JProperty(JPropertyKey, Id.ToString())
                   : null;

        #endregion

        #region ToJSON(this Text, JPropertyKey)

        public static JProperty ToJSON(this String Text, String JPropertyKey)

            => Text.IsNotNullOrEmpty()
                   ? new JProperty(JPropertyKey, Text)
                   : null;

        #endregion


        #region ToJSON(this DataLicenseIds)

        public static JArray ToJSON(this IEnumerable<DataLicense_Id> DataLicenseIds)

            => DataLicenseIds != null
                   ? new JArray(DataLicenseIds)
                   : null;

        #endregion

        #region ToJSON(this DataLicenseIds, JPropertyKey)

        public static JProperty ToJSON(this IEnumerable<DataLicense_Id> DataLicenseIds, String JPropertyKey)

            => DataLicenseIds != null
                   ? new JProperty(JPropertyKey, new JArray(DataLicenseIds))
                   : null;

        #endregion

        #region ToJSON(this DataLicense)

        public static JObject ToJSON(this DataLicense DataLicense)

            => DataLicense != null
                   ? JSONObject.Create(
                         new JProperty("@id",          DataLicense.Id.ToString()),
                         new JProperty("@context",     "https://opendata.social/contexts/dataLicenses"),
                         new JProperty("description",  DataLicense.Description),
                         new JProperty("uris",         new JArray(DataLicense.URIs))
                     )
                   : null;

        #endregion

        #region ToJSON(this DataLicenses)

        public static JArray ToJSON(this IEnumerable<DataLicense> DataLicenses)

            => DataLicenses != null
                   ? new JArray(DataLicenses.SafeSelect(ToJSON))
                   : null;

        #endregion

        #region ToJSON(this DataLicenses, JPropertyKey)

        public static JProperty ToJSON(this IEnumerable<DataLicense> DataLicenses, String JPropertyKey)

            => DataLicenses != null
                   ? new JProperty(JPropertyKey,
                                   new JArray(DataLicenses.SafeSelect(license => license.ToJSON())))
                   : null;

        #endregion

    }

}
