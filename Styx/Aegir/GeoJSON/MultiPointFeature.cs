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
    public class MultiPointFeature : AGeoJSONFeature
    {

        #region Data

        private readonly List<GeoCoordinate>  _GeoCoordinates;

        #endregion

        #region Properties

        /// <summary>
        /// The geo coordinates of this feature.
        /// </summary>
        public IEnumerable<GeoCoordinate> GeoCoordinates
            => _GeoCoordinates;

        #endregion

        #region Constructor(s)

        public MultiPointFeature(String                      Id,
                                 Dictionary<String, Object>  Properties,
                                 IEnumerable<GeoCoordinate>  Points)

            : base(Id,
                   "MultiPoint",
                   Properties)

        {

            this._GeoCoordinates  = new List<GeoCoordinate>();

            foreach (var point in Points)
                _GeoCoordinates.Add(point);

        }

        #endregion


        #region Parse(GeoJSON)

        /// <summary>
        /// Parse the given JSON as GeoJSON.
        /// </summary>
        /// <param name="GeoJSON">A valid GeoJSON JSON.</param>
        public static MultiPointFeature Parse(JObject GeoJSON)
        {

            if (GeoJSON == null)
                throw new ArgumentNullException(nameof(GeoJSON), "The given JSON must not be null!");

            #region Parse geometry...

            var points = new List<GeoCoordinate>();

            if (GeoJSON     ["geometry"]    is JObject geometryJSON &&
                geometryJSON["coordinates"] is JArray  coordinatesJSON)
            {

                foreach (var coordinates in coordinatesJSON)
                {

                    if (coordinates is JArray)
                    {

                        points.Add(GeoCoordinate.Parse(coordinates[1].Value<Double>(),
                                                       coordinates[0].Value<Double>()));

                    }

                }

            }

            #endregion

            return new MultiPointFeature(GeoJSON["id"]?.Value<String>(),
                                         Aegir.GeoJSON.ParseProperties(GeoJSON["properties"] as JObject),
                                         points);

        }

        #endregion


    }

}
