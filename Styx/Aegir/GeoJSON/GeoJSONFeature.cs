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
    public class GeoJSONFeature
    {

        private readonly Dictionary<String, Object> _Properties;

        public IEnumerable<KeyValuePair<String, Object>> Properties
            => _Properties;

        public String Id   { get; }
        public String Type { get; }


        public GeoJSONFeature(JObject JSON)
        {

            this.Id   = JSON["id"].  Value<String>();
            this.Type = JSON["type"].Value<String>();


            this._Properties = new Dictionary<String, Object>();

            // "type": "FeatureCollection",
            // "generator": "overpass-turbo",
            // "copyright": "The data included in this document is from www.openstreetmap.org. The data is made available under ODbL.",
            // "timestamp": "2016-02-06T01:14:02Z",

            foreach (var token in JSON["properties"])
            {

                var property = token as JProperty;

                if (property != null)
                {

                    switch (property.Type)
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
                            _Properties.Add(property.Name, property.Value.Value<Object>());
                            break;

                    }

                }

            }

        }


    }

}
