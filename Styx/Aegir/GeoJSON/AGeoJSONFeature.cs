/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// http://www.geojson.org/geojson-spec.html
    /// </summary>
    public abstract class AGeoJSONFeature
    {

        #region Data

        private readonly Dictionary<String, Object> properties;

        #endregion

        #region Properties

        /// <summary>
        /// An enumeration of all geo json properties.
        /// </summary>
        public IEnumerable<KeyValuePair<String, Object>>  Properties
            => properties;

        /// <summary>
        /// The GeoJSON feature identification.
        /// </summary>
        public String                                     Id                { get; }

        /// <summary>
        /// The GeoJSON feature type.
        /// </summary>
        public String                                     Type              { get; }

        #endregion

        #region Constructor(s)

        public AGeoJSONFeature(String                      Id,
                               String                      Type,
                               Dictionary<String, Object>  Properties)
        {

            this.Id          = Id;
            this.Type        = Type;
            this.properties  = Properties ?? [];

        }

        #endregion


        #region ContainsKey(Key)

        public Object ContainsKey(String Key)
            => properties.ContainsKey(Key);

        #endregion

        #region GetProperty(Key)

        public Object? GetProperty(String Key)
        {

            if (properties.TryGetValue(Key, out var value))
                return value;

            return null;

        }

        #endregion


        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
            => Id;

        #endregion

    }

}
