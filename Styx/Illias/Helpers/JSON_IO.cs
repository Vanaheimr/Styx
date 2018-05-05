/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// JSON I/O.
    /// </summary>
    public static class JSON_IO
    {

        #region ToJSON(this Id, JPropertyKey)

        /// <summary>
        /// Create a JSON representation of the given identificator.
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
