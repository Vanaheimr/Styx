/*
 * Copyright (c) 2010-2023, Achim Friedland <achim.friedland@graphdefined.com>
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
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// http://www.geojson.org/geojson-spec.html
    /// </summary>
    public class GeoJSON
    {

        #region Data

        private readonly Dictionary<String, Object>  _Properties;

        private readonly List<AGeoJSONFeature>       _Features;

        #endregion

        #region Properties

        /// <summary>
        /// An enumeration of all GeoJSON properties.
        /// </summary>
        public IEnumerable<KeyValuePair<String, Object>>  Properties
            => _Properties;

        /// <summary>
        /// An enumeration of all GeoJSON features.
        /// </summary>
        public IEnumerable<AGeoJSONFeature>               Features
            => _Features;

        #endregion

        #region (private) Constructor(s)

        private GeoJSON(Dictionary<String, Object>    Properties,
                        IEnumerable<AGeoJSONFeature>  Features)
        {

            this._Properties  = Properties ?? new Dictionary<String, Object>();
            this._Features    = Features != null ? new List<AGeoJSONFeature>(Features) : new List<AGeoJSONFeature>();

        }

        #endregion


        internal static Dictionary<String, Object> ParseProperties(JObject GeoJSON)
        {

            var properties  = new Dictionary<String, Object>();

            // "type":      "FeatureCollection",
            // "generator": "overpass-turbo",
            // "copyright": "The data included in this document is from www.openstreetmap.org. The data is made available under ODbL.",
            // "timestamp": "2016-02-06T01:14:02Z",

            if (GeoJSON != null)
            {
                foreach (var property in GeoJSON)
                {

                    if (property.Key == "features")
                        continue;

                    switch (property.Value.Type)
                    {

                        case JTokenType.None:           // None         = 0
                        case JTokenType.Constructor:    // Constructor  = 3
                        case JTokenType.Comment:        // Comment      = 5
                        case JTokenType.Null:           // Null         = 10
                        case JTokenType.Undefined:      // Undefined    = 11
                            break;


                        // Object       = 1
                        case JTokenType.Object:
                            properties.Add(property.Key, property.Value.Value<JObject>());
                            break;

                        // Array        = 2
                        case JTokenType.Array:
                            properties.Add(property.Key, property.Value.Value<JArray>());
                            break;

                        // Integer      = 6
                        case JTokenType.Integer:
                            properties.Add(property.Key, property.Value.Value<Int32>());
                            break;

                        // Float        = 7
                        case JTokenType.Float:
                            properties.Add(property.Key, property.Value.Value<Single>());
                            break;

                        // Boolean      = 9
                        case JTokenType.Boolean:
                            properties.Add(property.Key, property.Value.Value<Boolean>());
                            break;

                        // Date         = 12
                        case JTokenType.Date:
                            properties.Add(property.Key, property.Value.Value<DateTime>());
                            break;

                        // Bytes        = 14
                        case JTokenType.Bytes:
                            properties.Add(property.Key, property.Value.Value<Byte[]>());
                            break;

                        // Guid         = 15
                        case JTokenType.Guid:
                            properties.Add(property.Key, property.Value.Value<Guid>());
                            break;

                        // Uri          = 16
                        case JTokenType.Uri:
                            properties.Add(property.Key, property.Value.Value<Uri>());
                            break;

                        // TimeSpan     = 17
                        case JTokenType.TimeSpan:
                            properties.Add(property.Key, property.Value.Value<TimeSpan>());
                            break;


                        // Property     = 4
                        // String       = 8
                        // Raw          = 13
                        default:
                            properties.Add(property.Key, property.Value.Value<String>());
                            break;

                    }

                }
            }

            return properties;

        }



        #region Parse   (GeoJSON)

        /// <summary>
        /// Parse the given JSON as GeoJSON.
        /// </summary>
        /// <param name="GeoJSON">A valid GeoJSON JSON.</param>
        public static GeoJSON Parse(JObject GeoJSON)
        {

            if (GeoJSON == null)
                throw new ArgumentNullException(nameof(GeoJSON), "The given GeoJSON must not be null!");

            #region Parse features...

            var features    = new List<AGeoJSONFeature>();

            if (GeoJSON["features"] is JArray featuresJSON)
            {
                foreach (var feature in featuresJSON)
                {
                    if (feature is JObject JSONFeature)
                    {

                        switch (JSONFeature["geometry"]["type"].Value<String>().ToLower())
                        {

                            case "point":
                                features.Add(PointFeature.          Parse(JSONFeature));
                                break;

                            case "multipoint":
                                features.Add(MultiPointFeature.     Parse(JSONFeature));
                                break;

                            case "linestring":
                                features.Add(LineStringFeature.     Parse(JSONFeature));
                                break;

                            case "multilinestring":
                                features.Add(MultiLineStringFeature.Parse(JSONFeature));
                                break;

                            case "polygon":
                                features.Add(PolygonFeature.        Parse(JSONFeature));
                                break;

                        }

                    }
                }
            }

            #endregion

            return new GeoJSON(ParseProperties(GeoJSON),
                               features);

        }

        #endregion

        #region LoadFile(GeoJSONFile)

        /// <summary>
        /// Read the given GeoJSON file.
        /// </summary>
        /// <param name="Filename">The GeoJSON file name.</param>
        public static GeoJSON LoadFile(String Filename)
        {

            if (!File.Exists(Filename))
                throw new ArgumentException("The given GeoJSON file '" + Filename + "' does not exists!", nameof(Filename));

            try
            {

                return Parse(JObject.Parse(File.ReadAllText(Filename)));

            }
            catch (Exception e)
            {
                throw new ArgumentException("Could not parse the given GeoJSON file '" + Filename + "'!", e);
            }

        }

        #endregion


        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
            => _Properties["Id"]?.ToString();

        #endregion

    }

}
