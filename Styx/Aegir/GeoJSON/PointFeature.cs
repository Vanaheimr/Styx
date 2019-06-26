/*
 * Copyright (c) 2010-2019, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// http://www.geojson.org/geojson-spec.html
    /// </summary>
    public class PointFeature : AGeoJSONFeature
    {

        #region Properties

        /// <summary>
        /// The geo coordinates of this feature.
        /// </summary>
        public GeoCoordinate  GeoCoordinate    { get; }

        #endregion

        #region Constructor(s)

        public PointFeature(String                      Id,
                            Dictionary<String, Object>  Properties,
                            GeoCoordinate               GeoCoordinate)

            : base(Id,
                   "Point",
                   Properties)

        {

            this.GeoCoordinate = GeoCoordinate;

        }

        #endregion


        #region Parse(GeoJSON)

        /// <summary>
        /// Parse the given JSON as GeoJSON.
        /// </summary>
        /// <param name="GeoJSON">A valid GeoJSON JSON.</param>
        public static PointFeature Parse(JObject GeoJSON)
        {

            if (GeoJSON == null)
                throw new ArgumentNullException(nameof(GeoJSON), "The given GeoJSON must not be null!");


            if (GeoJSON     ["geometry"]    is JObject geometryJSON &&
                geometryJSON["coordinates"] is JArray  coordinatesJSON)
            {

                return new PointFeature(GeoJSON["id"]?.Value<String>(),
                                        Aegir.GeoJSON.ParseProperties(GeoJSON["properties"] as JObject),
                                        GeoCoordinate.Parse(coordinatesJSON[1].Value<Double>(),
                                                            coordinatesJSON[0].Value<Double>()));

            }

            throw new ArgumentException("The given GeoJSON is invalid!", nameof(GeoJSON));

        }

        #endregion


    }

}
