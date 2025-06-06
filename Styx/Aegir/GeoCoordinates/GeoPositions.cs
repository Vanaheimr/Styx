﻿/*
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
    /// Geographical positions (e.g. for debugging).
    /// </summary>
    public static class GeoPositions
    {

        /// <summary>
        /// Geographical positions in Germany.
        /// </summary>
        public static class Germany
        {

            /// <summary>
            /// The geographical position of Berlin, Germany.
            /// </summary>
            public static GeoCoordinate Berlin()
            {
                return new GeoCoordinate(Latitude.Parse(52.500556), Longitude.Parse(13.398889));
            }

            /// <summary>
            /// The geographical position of Jena, Germany.
            /// </summary>
            public static GeoCoordinate Jena()
            {
                return new GeoCoordinate(Latitude.Parse(50.929054), Longitude.Parse(11.584074));
            }

        }

    }

}
