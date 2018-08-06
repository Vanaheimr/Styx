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
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// http://www.geojson.org/geojson-spec.html
    /// </summary>
    public class GeoJSONFile
    {

        #region Data

        private readonly Dictionary<String, String> _Properties;

        private readonly List<GeoJSONFeature>       _Features;

        #endregion

        #region Properties

        /// <summary>
        /// An enumeration of all geo json properties.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>> Properties
            => _Properties;

        /// <summary>
        /// An enumeration of all geo json features.
        /// </summary>
        public IEnumerable<GeoJSONFeature> Features
            => _Features;

        #endregion

        #region Constructor(s)

        private GeoJSONFile(Dictionary<String, String>   Properties,
                            IEnumerable<GeoJSONFeature>  Features)
        {

            this._Properties  = Properties ?? new Dictionary<String, String>();
            this._Features    = Features != null ? new List<GeoJSONFeature>(Features) : new List<GeoJSONFeature>();

        }

        #endregion


        #region Parse       (GeoJSON)

        /// <summary>
        /// Parse the given JSON as GeoJSON.
        /// </summary>
        /// <param name="GeoJSON">A valid GeoJSON JSON.</param>
        public static GeoJSONFile Parse(JObject GeoJSON)
        {

            if (GeoJSON == null)
                throw new ArgumentNullException(nameof(GeoJSON), "The given JSON must not be null!");

            #region Parse properties...

            var _Properties  = new Dictionary<String, String>();

            // "type":      "FeatureCollection",
            // "generator": "overpass-turbo",
            // "copyright": "The data included in this document is from www.openstreetmap.org. The data is made available under ODbL.",
            // "timestamp": "2016-02-06T01:14:02Z",

            foreach (var property in GeoJSON)
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

            #endregion

            #region Parse features...

            var _Features    = new List<GeoJSONFeature>();

            if (GeoJSON["features"] is JArray features)
            {
                foreach (var feature in features)
                {
                    if (feature is JObject JSONFeature)
                        _Features.Add(GeoJSONFeature.Parse(JSONFeature));
                }
            }

            #endregion

            return new GeoJSONFile(_Properties,
                                   _Features);

        }

        #endregion

        #region LoadFromFile(GeoJSONFile)

        /// <summary>
        /// Read the given GeoJSON file.
        /// </summary>
        /// <param name="GeoJSONFile">The GeoJSON file name.</param>
        public static GeoJSONFile LoadFromFile(String GeoJSONFile)
        {

            if (!File.Exists(GeoJSONFile))
                throw new ArgumentException("The given GeoJSON file '" + GeoJSONFile + "' does not exists!", nameof(GeoJSONFile));

            JObject GeoJSON = null;

            try
            {

                GeoJSON = JObject.Parse(File.ReadAllText(GeoJSONFile));

            }
            catch (Exception e)
            {
                throw new ArgumentException("Could not parse the given GeoJSON file '" + GeoJSONFile + "'!", e);
            }

            return Parse(GeoJSON);

        }

        #endregion


    }

}
