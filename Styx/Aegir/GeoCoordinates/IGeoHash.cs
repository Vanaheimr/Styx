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
    /// Extensionmethods for the IGeoHash interface.
    /// </summary>
    public static class IGeoHashExtensions
    {

        /// <summary>
        /// Decode this geohash to a geocoordinate.
        /// </summary>
        /// <param name="Digits">Rounds the double-precision value to the given number of fractional digits.</param>
        public static GeoCoordinate ToGeoCoordinate<T>(this IGeoHash<T> GeoHash, Byte Digits = 12)

            => GeoHash.Decode((lat, lon) => new GeoCoordinate(lat, lon), Digits);

    }


    /// <summary>
    /// The common interface for all geohashes.
    /// </summary>
    /// <typeparam name="T">The type of the geohash.</typeparam>
    public interface IGeoHash<T> : IGeoCoordinate
    {

        /// <summary>
        /// Decode the geohash into latitude and longitude using the given
        /// delegate to transfor it into the resulting data structure.
        /// </summary>
        /// <typeparam name="TReturn">The type of the resulting data structure.</typeparam>
        /// <param name="Processor">A delegate to transform the decoded latitude and longitude into the resulting data structure.</param>
        /// <param name="Digits">Rounds the double-precision latitude and longitude to the given number of fractional digits.</param>
        TReturn Decode<TReturn>(Func<Latitude, Longitude, TReturn> Processor, Byte Digits = 12);

        /// <summary>
        /// The value of the geohash.
        /// </summary>
        T Value { get; }

    }

}
