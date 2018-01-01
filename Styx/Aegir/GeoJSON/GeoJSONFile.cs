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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// http://www.geojson.org/geojson-spec.html
    /// </summary>
    public class GeoJSONFile
    {

        private readonly Dictionary<String, Object> _Properties;

        public IEnumerable<KeyValuePair<String, Object>> Properties
            => _Properties;

        private List<GeoJSONFeature> _Features;

        public IEnumerable<GeoJSONFeature> Features
            => _Features;


        private GeoJSONFile(JObject JSON)
        {

            this._Properties = new Dictionary<String, Object>();

            // "type": "FeatureCollection",
            // "generator": "overpass-turbo",
            // "copyright": "The data included in this document is from www.openstreetmap.org. The data is made available under ODbL.",
            // "timestamp": "2016-02-06T01:14:02Z",

            foreach (var property in JSON)
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
                        _Properties.Add(property.Key, property.Value.Value<Object>());
                        break;

                }

            }

            var _features = JSON["features"] as JArray;

            if (_features != null)
                _Features = _features.Select(feature => new GeoJSONFeature(feature as JObject)).ToList();

        }

        public static GeoJSONFile Load(String Path)
        {
            return new GeoJSONFile(JObject.Parse(File.ReadAllText(Path)));
        }

    }

}
