/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Hermod <https://www.github.com/Vanaheimr/Hermod>
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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for JSON objects.
    /// </summary>
    public static class JSONObject
    {

        #region Create(params JProperties)

        /// <summary>
        /// Create a JSON object using the given JSON properties, but filter null values.
        /// </summary>
        /// <param name="JProperties">JSON properties.</param>
        public static JObject Create(params JProperty?[] JProperties)
        {

            if (JProperties is null || JProperties.Length == 0)
                return [];

            var data = JProperties.
                           Where(jProperty => jProperty is not null && jProperty.Value is not null).
                           Cast<JProperty>().
                           ToArray();

            return data.Length > 0
                       ? new JObject(data)
                       : [];

        }

        #endregion

        #region Create(JProperties)

        /// <summary>
        /// Create a JSON object using the given JSON properties, but filter null values.
        /// </summary>
        /// <param name="JProperties">JSON properties.</param>
        public static JObject Create(IEnumerable<JProperty?> JProperties)
        {

            if (JProperties is null || !JProperties.Any())
                return [];

            var data = JProperties.
                           Where(jProperty => jProperty is not null && jProperty.Value is not null).
                           Cast<JProperty>().
                           ToArray();

            return data.Length > 0
                       ? new JObject(data)
                       : [];

        }

        #endregion


        #region ToUTF8Bytes(this JSONObject, Format = None)

        public static Byte[] ToUTF8Bytes(this JObject                JSONObject,
                                         Newtonsoft.Json.Formatting  Format = Newtonsoft.Json.Formatting.None)
        {

            if (JSONObject == null)
                return [];

            return Encoding.UTF8.GetBytes(JSONObject.ToString(Format));

        }

        #endregion

    }

    /// <summary>
    /// Extension methods for JSON arrays.
    /// </summary>
    public static class JSONArray
    {

        #region Create(params JObjects)

        /// <summary>
        /// Create a JSON array using the given JSON objects, but filter null values.
        /// </summary>
        /// <param name="JObjects">JSON objects.</param>
        public static JArray Create(params JObject?[] JObjects)
        {

            if (JObjects is null || !JObjects.Any())
                return new JArray();

            var data = JObjects.
                           Where(jobject => jobject is not null).
                           Cast<JObject>().
                           ToArray();

            return data.Length > 0
                       ? new JArray(data)
                       : new JArray();

        }

        #endregion

        #region Create(JObjects)

        /// <summary>
        /// Create a JSON array using the given JSON objects, but filter null values.
        /// </summary>
        /// <param name="JObjects">JSON objects.</param>
        public static JArray Create(IEnumerable<JObject?> JObjects)
        {

            if (JObjects is null || !JObjects.Any())
                return new JArray();

            var data = JObjects.
                           Where(jobject => jobject is not null).
                           Cast<JObject>().
                           ToArray();

            return data.Length > 0
                       ? new JArray(data)
                       : new JArray();

        }

        #endregion


        #region Create(params JProperties)

        /// <summary>
        /// Create a JSON array using the given JSON properties, but filter null values.
        /// </summary>
        /// <param name="JProperties">JSON properties.</param>
        public static JArray Create(params JProperty[] JProperties)
        {

            if (JProperties is null || !JProperties.Any())
                return new JArray();

            var data = JProperties.
                           Where(jProperty => jProperty is not null).
                           Cast<JProperty>().
                           ToArray();

            return data.Length > 0
                       ? new JArray(data)
                       : new JArray();

        }

        #endregion

        #region Create(JProperties)

        /// <summary>
        /// Create a JSON array using the given JSON properties, but filter null values.
        /// </summary>
        /// <param name="JProperties">JSON properties.</param>
        public static JArray Create(IEnumerable<JProperty> JProperties)
        {

            if (JProperties is null || !JProperties.Any())
                return new JArray();

            var data = JProperties.
                           Where(jProperty => jProperty is not null).
                           Cast<JProperty>().
                           ToArray();

            return data.Length > 0
                       ? new JArray(data)
                       : new JArray();

        }

        #endregion


        #region ToUTF8Bytes(this JSONArray, Format = None)

        public static Byte[] ToUTF8Bytes(this JArray                 JSONArray,
                                         Newtonsoft.Json.Formatting  Format = Newtonsoft.Json.Formatting.None)
        {

            if (JSONArray == null)
                return new Byte[0];

            return Encoding.UTF8.GetBytes(JSONArray.ToString(Format));

        }

        #endregion

    }


    /// <summary>
    /// Extension methods for JSON properties.
    /// </summary>
    public static class JSONProperties
    {

        #region Create(params JProperties)

        /// <summary>
        /// Create a JSON object using the given JSON properties, but filter null values.
        /// </summary>
        /// <param name="JProperties">JSON properties.</param>
        public static IEnumerable<JProperty> Create(params JProperty?[] JProperties)
        {

            if (JProperties is null || !JProperties.Any())
                return Array.Empty<JProperty>();

            var data = JProperties.
                           Where(jProperty => jProperty is not null).
                           Cast<JProperty>().
                           ToArray();

            return data.Length > 0
                       ? data
                       : Array.Empty<JProperty>();

        }

        #endregion

    }

}
