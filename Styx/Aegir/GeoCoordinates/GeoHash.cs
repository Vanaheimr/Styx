/*
 * Copyright (c) 2010-2021, Achim Friedland <achim.friedland@graphdefined.com>
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

/*
 * Based upon: http://github.com/sharonjl/geohash-net
 * 2011 (c) Sharon Lourduraj
 * Distributed under the MIT License
 */

#region Usings

using System;
using System.Text;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    #region GeoHashExtentions

    /// <summary>
    /// Extention methods for the GeoHash data structure.
    /// </summary>
    public static class GeoHashExtentions
    {

        /// <summary>
        /// Transform the given geo coordinate into a geohash64.
        /// </summary>
        /// <param name="GeoCoordinate">The geo coordinate.</param>
        /// <param name="Precision">An optional precision aka number of characters of the resulting geohash.</param>
        public static GeoHash ToGeoHash(this GeoCoordinate GeoCoordinate, Byte Precision = 12)
        {
            return new GeoHash(GeoCoordinate, Precision);
        }

    }

    #endregion

    #region GeoHash

    /// <summary>
    /// A base32-encoded alphanumeric geohash.
    /// </summary>
    public struct GeoHash : IGeoHash<String>,
                            IGeoCoordinate,
                            IEquatable<GeoHash>,
                            IComparable<GeoHash>

    {

        #region Direction enum

        public enum Direction
        {
            Top    = 0,
            Right  = 1,
            Bottom = 2,
            Left   = 3
        }

        #endregion

        #region Data

        private static readonly string[][] Neighbors = {
                                                           new[]
                                                               {
                                                                   "p0r21436x8zb9dcf5h7kjnmqesgutwvy", // Top
                                                                   "bc01fg45238967deuvhjyznpkmstqrwx", // Right
                                                                   "14365h7k9dcfesgujnmqp0r2twvyx8zb", // Bottom
                                                                   "238967debc01fg45kmstqrwxuvhjyznp", // Left
                                                               }, new[]
                                                                      {
                                                                          "bc01fg45238967deuvhjyznpkmstqrwx", // Top
                                                                          "p0r21436x8zb9dcf5h7kjnmqesgutwvy", // Right
                                                                          "238967debc01fg45kmstqrwxuvhjyznp", // Bottom
                                                                          "14365h7k9dcfesgujnmqp0r2twvyx8zb", // Left
                                                                      }
                                                       };

        private static readonly string[][] Borders = {
                                                         new[] {"prxz", "bcfguvyz", "028b", "0145hjnp"},
                                                         new[] {"bcfguvyz", "prxz", "0145hjnp", "028b"}
                                                     };

        /// <summary>
        /// Special GeoHash Base32 alphabet
        /// </summary>
        /// <see cref="http://en.wikipedia.org/wiki/Geohash"/>
        public const String GeoHashAlphabet = "0123456789bcdefghjkmnpqrstuvwxyz";

        /// <summary>
        /// The internal geohash.
        /// </summary>
        private readonly String InternalGeoHash;

        #endregion

        #region Properties

        #region Digits

        private Byte _Digits;

        /// <summary>
        /// Rounds the double-precision value to the given number of fractional digits.
        /// </summary>
        public Byte Digits
        {

            get
            {
                return _Digits;
            }

            set
            {
                _Digits = value;
            }

        }

        #endregion

        #region Latitude

        /// <summary>
        /// The latitude (south to nord).
        /// </summary>
        public Latitude Latitude
        {
            get
            {
                return Decode((lat, lon) => lat, Digits);
            }
        }

        #endregion

        #region Longitude

        /// <summary>
        /// The longitude (parallel to equator).
        /// </summary>
        public Longitude Longitude
        {
            get
            {
                return Decode((lat, lon) => lon, Digits);
            }
        }

        #endregion

        #region Value

        /// <summary>
        /// Returns the value of the geohash.
        /// </summary>
        public String Value
        {
            get
            {
                return this.InternalGeoHash;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region GeoHash(GeoHash)

        private GeoHash(String GeoHash)
        {
            InternalGeoHash = GeoHash;
            _Digits         = 12;
        }

        #endregion

        #region GeoHash(GeoCoordinate, Precision = 12)

        /// <summary>
        /// Create a new base32-encoded alphanumeric geohash.
        /// </summary>
        /// <param name="GeoCoordinate">A geocoordinate.</param>
        /// <param name="Precision">An optional precision aka number of characters of the resulting geohash.</param>
        public GeoHash(GeoCoordinate GeoCoordinate, Byte Precision = 12)
            : this(Encode(GeoCoordinate.Latitude, GeoCoordinate.Longitude, Precision))
        { }

        #endregion

        #region GeoHash(Latitude, Longitude, Precision = 12)

        /// <summary>
        /// Create a new base32-encoded alphanumeric geohash.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="Precision">An optional precision aka number of characters of the resulting geohash.</param>
        public GeoHash(Latitude Latitude, Longitude Longitude, Byte Precision = 12)
            : this(Encode(Latitude, Longitude, Precision))
        { }

        #endregion

        #endregion


        #region (private) (static) Encode(Latitude, Longitude, Precision = 12)

        /// <summary>
        /// Encode the given latitude and longitude as geohash.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="Precision">An optional precision aka number of characters of the resulting geohash.</param>
        /// <returns>The latitude and longitude encoded as geohash.</returns>
        private static String Encode(Latitude Latitude, Longitude Longitude, Byte Precision = 12)
        {

            var even      = true;
            var bit       = 0;
            var character = 0;
            var GeoHash   = new StringBuilder();
            var bitmask   = 16;
            Double midLat, midLon;

            var _MinLatitude  =  -90.0;
            var _MaxLatitude  =   90.0;
            var _MinLongitude = -180.0;
            var _MaxLongitude =  180.0;

            if (Precision < 1 || Precision > 20) Precision = 12;

            for (var i = 0; i < 5*Precision; i++)
            {

                if (even)
                {

                    midLon = (_MinLongitude + _MaxLongitude) / 2;

                    if (Longitude.Value > midLon)
                    {
                        // Set 1!
                        character |= bitmask;
                        _MinLongitude = midLon;
                    }
                    else
                        // Set 0!
                        _MaxLongitude = midLon;

                }
                else
                {

                    midLat = (_MinLatitude + _MaxLatitude) / 2;

                    if (Latitude.Value > midLat)
                    {
                        // Set 1!
                        character |= bitmask;
                        _MinLatitude = midLat;
                    }
                    else
                        // Set 0!
                        _MaxLatitude = midLat;

                }

                even = !even;

                if (bit == 4)
                {
                    GeoHash.Append(GeoHashAlphabet[character]);
                    bit       = 0;
                    bitmask   = 16;
                    character = 0;
                }
                else
                {
                    bit++;
                    bitmask >>= 1;
                }


            }

            return GeoHash.ToString();

        }

        #endregion

        #region Decode<T>(Processor, Digits = 12)

        private static void RefineInterval(ref Double[] Interval, Int32 CharValue, Int32 Bitmask)
        {

            if ((CharValue & Bitmask) != 0)
                Interval[0] = (Interval[0] + Interval[1]) / 2; // Min

            else
                Interval[1] = (Interval[0] + Interval[1]) / 2; // Max

        }

        /// <summary>
        /// Decode the geohash into latitude and longitude using the given
        /// delegate to transfor it into the resulting data structure.
        /// </summary>
        /// <typeparam name="T">The type of the resulting data structure.</typeparam>
        /// <param name="Processor">A delegate to transform the decoded latitude and longitude into the resulting data structure.</param>
        /// <param name="Digits">Rounds the double-precision latitude and longitude to the given number of fractional digits.</param>
        public T Decode<T>(Func<Latitude, Longitude, T> Processor, Byte Digits = 12)
        {

            #region Initial checks

            if (Processor == null)
                throw new ArgumentNullException("The given delegate must not be null!");

            #endregion

            var even      = true;
            var CharValue = 0;
            var Bitmask   = 0;

            Double[] LatitudeInterval  = {  -90,  90 };
            Double[] LongitudeInterval = { -180, 180 };

            foreach (var Character in InternalGeoHash)
            {

                //ToDo: Faster GeoHashAlphabet lookup!
                CharValue = GeoHashAlphabet.IndexOf(Character);

                for (int j = 0; j < 5; j++)
                {

                    Bitmask = 16 >> j;

                    if (even)
                        RefineInterval(ref LongitudeInterval, CharValue, Bitmask);

                    else
                        RefineInterval(ref LatitudeInterval,  CharValue, Bitmask);

                    even = !even;

                }

            }

            return Processor(
                Latitude. Parse(Math.Round((LatitudeInterval[0]  + LatitudeInterval[1])  / 2, Digits)),
                Longitude.Parse(Math.Round((LongitudeInterval[0] + LongitudeInterval[1]) / 2, Digits))
            );

        }

        #endregion


        #region CalculateAdjacent(Direction)

        /// <summary>
        /// Calculate the adjacent geohashes.
        /// </summary>
        /// <param name="Direction">The direction.</param>
        public String CalculateAdjacent(Direction Direction)
        {
            return CalculateAdjacent_private(this.InternalGeoHash, Direction);
        }

        private String CalculateAdjacent_private(String hash, Direction Direction)
        {

            hash = hash.ToLower();

            var lastChr = hash[hash.Length - 1];
            var type    = hash.Length % 2;
            var dir     = (int) Direction;
            var nHash   = hash.Substring(0, hash.Length - 1);

            if (Borders[type][dir].IndexOf(lastChr) != -1)
                nHash = CalculateAdjacent_private(nHash, (Direction) dir);

            return nHash + GeoHashAlphabet[Neighbors[type][dir].IndexOf(lastChr)];

        }

        #endregion


        #region Operator overloading

        #region Operator == (GeoHash1, GeoHash2)

        /// <summary>
        /// Compares two geohashs for equality.
        /// </summary>
        /// <param name="GeoHash1">A geohash.</param>
        /// <param name="GeoHash2">Another geohash.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GeoHash GeoHash1, GeoHash GeoHash2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(GeoHash1, GeoHash2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GeoHash1 == null) || ((Object) GeoHash2 == null))
                return false;

            return GeoHash1.Equals(GeoHash2);

        }

        #endregion

        #region Operator != (GeoHash1, GeoHash2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="GeoHash1">A geohash.</param>
        /// <param name="GeoHash2">Another geohash.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GeoHash GeoHash1, GeoHash GeoHash2)
        {
            return !(GeoHash1 == GeoHash2);
        }

        #endregion

        #endregion

        #region IComparable Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given Object must not be null!");

            return CompareTo((GeoHash32)Object);

        }

        #endregion

        #region CompareTo(GeoHash)

        /// <summary>
        /// Compares two geohashes.
        /// </summary>
        /// <param name="GeoHash">Another geohash.</param>
        public Int32 CompareTo(GeoHash GeoHash)
        {
            return this.Value.CompareTo(GeoHash.Value);
        }

        #endregion

        #endregion

        #region IEquatable Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            try
            {
                return this.Equals((GeoHash32)Object);
            }
            catch (InvalidCastException)
            {
                return false;
            }

        }

        #endregion

        #region Equals(IGeoCoordinate)

        /// <summary>
        /// Compares two geo coordinates for equality.
        /// </summary>
        /// <param name="IGeoCoordinate">Another geo coordinate.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(IGeoCoordinate IGeoCoordinate)
        {

            if (IGeoCoordinate.Latitude.Value != this.Latitude.Value)
                return false;

            if (IGeoCoordinate.Longitude.Value != this.Longitude.Value)
                return false;

            return true;

        }

        #endregion

        #region Equals(GeoHash)

        /// <summary>
        /// Compares two geohashes for equality.
        /// </summary>
        /// <param name="GeoHash">Another geohash.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(GeoHash GeoHash)
        {
            return this.Value.Equals(GeoHash.Value);
        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hashcode of this object.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return this.InternalGeoHash.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        public override String ToString()
        {
            return this.InternalGeoHash;
        }

        #endregion

    }

    #endregion

}
