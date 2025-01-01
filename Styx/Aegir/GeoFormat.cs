/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Different representations of geographical coordinates/positions.
    /// </summary>
    public enum GeoFormat
    {

        /// <summary>
        /// A decimal (h ddd.dddddd°) representation of a geographical position using the World Geodetic System 84 (WGS 84),
        /// e.g. 49.449030° N, 11.074880° E
        /// </summary>
        Decimal,

        /// <summary>
        /// A sexagesimal (h dd° mm' ss.s'' or degrees, minutes, secondes) representation of a geographical position,
        /// e.g. 49° 26' 56.5'' N, 11° 4' 29.6'' E
        /// </summary>
        Sexagesimal

    }

}
