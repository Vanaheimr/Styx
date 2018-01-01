/*
 * Copyright (c) 2010-2017, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
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

using Newtonsoft.Json.Linq;
using System.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// JSON I/O.
    /// </summary>
    public static class JSON_IO
    {

        #region ToJSON(this GeoLocation)

        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A geographical location.</param>
        public static JObject ToJSON(this GeoCoordinate GeoLocation)
        {

            if (GeoLocation == default(GeoCoordinate))
                return null;

            return new JObject(
                       new JProperty[] {
                           GeoLocation.Projection != GravitationalModel.WGS84 ? new JProperty("projection", GeoLocation.Projection.ToString()) : null,
                           new JProperty("lat", GeoLocation.Latitude. Value),
                           new JProperty("lng", GeoLocation.Longitude.Value),
                           GeoLocation.Altitude.HasValue                      ? new JProperty("alt",        GeoLocation.Altitude.Value.Value)  : null
                       }.Where(_ => _ != null)
                   );

        }


        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A geographical location.</param>
        public static JObject ToJSON(this GeoCoordinate? GeoLocation)
            => GeoLocation?.ToJSON();

        #endregion

        #region ToJSON(this GeoLocation, JPropertyKey)

        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A geographical location.</param>
        /// <param name="JPropertyKey">The name of the JSON property key to use.</param>
        public static JProperty ToJSON(this GeoCoordinate GeoLocation, String JPropertyKey)
        {

            if (GeoLocation == default(GeoCoordinate))
                return null;

            return new JProperty(JPropertyKey,
                                 GeoLocation.ToJSON());

        }


        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A GeoLocation.</param>
        /// <param name="JPropertyKey">The name of the JSON property key to use.</param>
        public static JProperty ToJSON(this GeoCoordinate? GeoLocation, String JPropertyKey)
        {

            if (!GeoLocation.HasValue)
                return null;

            return new JProperty(JPropertyKey,
                                 GeoLocation.Value.ToJSON());

        }

        #endregion


        public static GeoCoordinate? ParseGeoCoordinate(this JObject JSON)
        {

            var projection  = JSON["projection"];
            var lat         = JSON["lat"];
            var lng         = JSON["lng"];
            var alt         = JSON["alt"];

            if (lat != null && lng != null)
            {

                try
                {

                    return GeoCoordinate.Parse(lat.Value<String>(),
                                               lng.Value<String>(),
                                               alt.Value<String>());

                }
                catch (Exception)
                { }

            }

            return null;

        }

        public static GeoCoordinate? ParseGeoCoordinate(this String Text)
            => ParseGeoCoordinate(JObject.Parse(Text));


        public static Boolean TryParseGeoCoordinate(this JObject JSON, out GeoCoordinate GeoLocation)
        {

            var projection  = JSON["projection"];
            var lat         = JSON["lat"];
            var lng         = JSON["lng"];
            var alt         = JSON["alt"];

            if (lat != null && lng != null)
            {

                try
                {

                    GeoLocation = GeoCoordinate.Parse(lat. Value<Double>(),
                                                      lng. Value<Double>(),
                                                      alt?.Value<Double>());

                    return true;

                }
                catch (Exception e)
                {
                }

            }

            GeoLocation = default(GeoCoordinate);
            return false;

        }

        public static Boolean TryParseGeoCoordinate(this String Text, out GeoCoordinate GeoLocation)
            => TryParseGeoCoordinate(JObject.Parse(Text), out GeoLocation);

    }

}
