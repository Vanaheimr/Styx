/*
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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// http://www.geojson.org/geojson-spec.html
    /// </summary>
    public class GeoJSONFeature
    {

        #region Data

        private readonly Dictionary<String, String> _Properties;

        private readonly List<GeoCoordinate>        _GeoCoordinates;

        #endregion

        #region Properties

        /// <summary>
        /// An enumeration of all geo json properties.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>  Properties
            => _Properties;

        /// <summary>
        /// The GeoJSON feature identification.
        /// </summary>
        public String                                     Id                { get; }

        /// <summary>
        /// The GeoJSON feature type.
        /// </summary>
        public String                                     Type              { get; }

        /// <summary>
        /// The geo coordinates of this feature.
        /// </summary>
        public IEnumerable<GeoCoordinate>                 GeoCoordinates
            => _GeoCoordinates;

        #endregion

        #region Constructor(s)

        private GeoJSONFeature(String                      Id,
                               String                      Type,
                               Dictionary<String, String>  Properties,
                               IEnumerable<GeoCoordinate>  GeoCoordinates)
        {

            this.Id               = Id;
            this.Type             = Type;
            this._Properties      = Properties ?? new Dictionary<String, String>();
            this._GeoCoordinates  = GeoCoordinates != null ? new List<GeoCoordinate>(GeoCoordinates) : new List<GeoCoordinate>();

        }

        #endregion


        #region Parse(GeoJSON)

        /// <summary>
        /// Parse the given JSON as GeoJSON.
        /// </summary>
        /// <param name="GeoJSON">A valid GeoJSON JSON.</param>
        public static GeoJSONFeature Parse(JObject GeoJSON)
        {

            if (GeoJSON == null)
                throw new ArgumentNullException(nameof(GeoJSON), "The given JSON must not be null!");

            #region Parse properties...

            var _Properties  = new Dictionary<String, String>();

            // {
            //
            //    "type":  "Feature",
            //    "id":    "node/265616096",
            //
            //    "properties": {
            //      "@id":               "node/265616096",
            //      "amenity":           "post_box",
            //      "brand":             "Deutsche Post",
            //      "collection_times":  "Mo-Fr 14:30,16:30; Sa 12:45",
            //      "operator":          "Deutsche Post",
            //      "ref":               "Löbdergraben, 07743 Jena"
            //    },
            //
            //    "geometry": {
            //      "type": "Point",
            //      "coordinates": [
            //        11.587241,
            //        50.9268133
            //      ]
            //    }
            //
            // }

            if (GeoJSON["properties"] is JObject propertiesJSON)
            {

                foreach (var property in propertiesJSON)
                {

                    switch (property.Value.Type)
                    {

                        case JTokenType.Array:
                        case JTokenType.Object:
                        case JTokenType.Null:
                        case JTokenType.Constructor:
                        case JTokenType.Comment:
                        case JTokenType.None:
                        case JTokenType.Raw:
                        case JTokenType.Undefined:
                            break;

                        default:
                            _Properties.Add(property.Key, property.Value.Value<String>());
                            break;

                    }

                }

            }

            #endregion

            #region Parse geometry...

            var geoCoordinates = new List<GeoCoordinate>();

            if (GeoJSON     ["geometry"]    is JObject geometryJSON &&
                geometryJSON["coordinates"] is JArray  coordinatesJSON)
            {
                geoCoordinates.Add(GeoCoordinate.Parse(coordinatesJSON[1].Value<Double>(),
                                                       coordinatesJSON[0].Value<Double>()));
            }

            #endregion

            return new GeoJSONFeature(GeoJSON["id"  ]?.Value<String>(),
                                      GeoJSON["type"]?.Value<String>(),
                                      _Properties,
                                      geoCoordinates);

        }

        #endregion


    }

}
