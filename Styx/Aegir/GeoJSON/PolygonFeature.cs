/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Aegir <https://www.github.com/Vanaheimr/Aegir>
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
    public class PolygonFeature : AGeoJSONFeature
    {

        #region Data

        private readonly List<List<GeoCoordinate>>  _Rings;

        #endregion

        #region Properties

        /// <summary>
        /// An array of LinearRing coordinate arrays.
        /// For Polygons with multiple rings, the first must be the exterior ring
        /// and any others must be interior rings or holes.
        /// </summary>
        public IEnumerable<IEnumerable<GeoCoordinate>> Rings
            => _Rings;

        #endregion

        #region Constructor(s)

        public PolygonFeature(String                                   Id,
                              Dictionary<String, Object>               Properties,
                              IEnumerable<IEnumerable<GeoCoordinate>>  Rings)

            : base(Id,
                   "Polygon",
                   Properties)

        {

            this._Rings  = new List<List<GeoCoordinate>>();

            foreach (var ring in Rings)
                _Rings.Add(new List<GeoCoordinate>(ring));

        }

        #endregion


        #region Parse(GeoJSON)

        /// <summary>
        /// Parse the given JSON as GeoJSON.
        /// </summary>
        /// <param name="GeoJSON">A valid GeoJSON JSON.</param>
        public static PolygonFeature Parse(JObject GeoJSON)
        {

            if (GeoJSON == null)
                throw new ArgumentNullException(nameof(GeoJSON), "The given JSON must not be null!");

            #region Parse geometry...

            var polygons = new List<List<GeoCoordinate>>();

            if (GeoJSON     ["geometry"]    is JObject geometryJSON &&
                geometryJSON["coordinates"] is JArray  coordinatesJSON)
            {

                foreach (var outer in coordinatesJSON)
                {

                    if (outer is JArray)
                    {

                        polygons.Add(outer.Select(coordinates => GeoCoordinate.Parse(coordinates[1].Value<Double>(),
                                                                                     coordinates[0].Value<Double>())).ToList());

                    }

                }

            }

            #endregion

            return new PolygonFeature(GeoJSON["id"]?.Value<String>(),
                                      Aegir.GeoJSON.ParseProperties(GeoJSON["properties"] as JObject),
                                      polygons);

        }

        #endregion


    }

}
