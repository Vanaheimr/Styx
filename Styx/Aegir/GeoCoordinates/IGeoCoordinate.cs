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

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// The interface for all latitude/longitude/altitude based geo coordinates.
    /// </summary>
    public interface IGeoCoordinate : IEquatable<IGeoCoordinate>,
                                      IComparable<IGeoCoordinate>,
                                      IComparable
    {

        /// <summary>
        /// The geographical latitude (south to nord).
        /// </summary>
        Latitude   Latitude     { get; }

        /// <summary>
        /// The geographical longitude (parallel to equator).
        /// </summary>
        Longitude  Longitude    { get; }


        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        String ToString();

    }

}
