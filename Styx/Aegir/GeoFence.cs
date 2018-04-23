﻿/*
 * Copyright (c) 2010-2018, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// JSON I/O.
    /// </summary>
    public static class GeoFenceExtentions
    {

        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A geographical location.</param>
        public static JObject ToJSON(this GeoFence? GeoLocation)
            => GeoLocation?.ToJSON();


        #region ToJSON(this GeoLocation, JPropertyKey)

        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A geographical location.</param>
        /// <param name="JPropertyKey">The name of the JSON property key to use.</param>
        public static JProperty ToJSON(this GeoFence GeoLocation, String JPropertyKey)
        {

            //if (GeoLocation == default(GeoFence))
            //    return null;

            return new JProperty(JPropertyKey,
                                 GeoLocation.ToJSON());

        }


        /// <summary>
        /// Create a JSON representation of the given GeoLocation.
        /// </summary>
        /// <param name="GeoLocation">A GeoLocation.</param>
        /// <param name="JPropertyKey">The name of the JSON property key to use.</param>
        public static JProperty ToJSON(this GeoFence? GeoLocation, String JPropertyKey)
        {

            if (!GeoLocation.HasValue)
                return null;

            return new JProperty(JPropertyKey,
                                 GeoLocation.Value.ToJSON());

        }

        #endregion


        public static Boolean TryParseGeoFence(this String Text, out GeoFence GeoLocation)
            => GeoFence.TryParse(JObject.Parse(Text), out GeoLocation);

    }

    /// <summary>
    /// A geo fenche.
    /// </summary>
    public struct GeoFence
    {

        #region Properties

        /// <summary>
        /// An enumeration of geo coordinates.
        /// </summary>
        public IEnumerable<GeoFence>  GeoFences    { get; }

        /// <summary>
        /// An optional geographical distance.
        /// </summary>
        public Meter?                      Distance          { get; }

        /// <summary>
        /// An optional description.
        /// </summary>
        public I18NString                  Description       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new geo fence.
        /// </summary>
        /// <param name="GeoFences">An enumeration of geo coordinates.</param>
        /// <param name="Distance">An optional geographical distance.</param>
        /// <param name="Description">An optional description.</param>
        public GeoFence(IEnumerable<GeoFence>  GeoFences,
                        Meter?                      Distance,
                        I18NString                  Description = null)
        {

            this.GeoFences       = GeoFences;
            this.Distance        = Distance;
            this.Description     = Description ?? new I18NString();

        }

        #endregion



        public static Boolean TryParseJSON(String Text, out GeoFence GeoFence)
            => TryParse(JObject.Parse(Text), out GeoFence);

        public static Boolean TryParse(JObject JSON, out GeoFence GeoFence)
        {

            var type    = JSON["type"  ]?.Value<String>();
            var radius  = JSON["radius"]?.Value<String>();

            if (type != null && type == "circle" && radius.IsNeitherNullNorEmpty())
            {

                try
                {

                    GeoFence = new GeoFence(null,
                                            Meter.Parse(radius));

                    return true;

                }
                catch (Exception e)
                {
                }

            }

            GeoFence = default(GeoFence);
            return false;

        }


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()
        {

            if ((GeoFences == null || !GeoFences.Any()) && Distance.HasValue)
            {

                return JSONObject.Create(

                    new JProperty("type",    "circle"),
                    new JProperty("radius",  Distance.ToString())

                );

            }

            return JSONObject.Create();

        }

        #endregion




        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return GeoFences.GetHashCode() * 5 ^

                       (Distance.HasValue
                            ? Distance.Value.GetHashCode()
                            : 0) * 3 ^

                       (Description.IsNullOrEmpty()
                            ? Description.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {

            if (GeoFences.Count() == 1 && Distance.HasValue)
                return String.Concat(GeoFences.First(),
                                     " with radius ",
                                     Distance.Value,
                                     "km",
                                     Description.IsNullOrEmpty()
                                         ? "(" + Description.FirstText() + ")"
                                         : "");

            if (GeoFences.Count() > 1)
                return GeoFences.
                           Select(coordinate => coordinate.ToString()).
                           AggregateWith(", ");

            return Description.IsNullOrEmpty()
                       ? Description.FirstText()
                       : nameof(GeoFence);

        }

        #endregion


    }

}
