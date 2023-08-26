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

/*
 * Based upon: http://github.com/sharonjl/geoHash-net
 * 2011 (c) Sharon Lourduraj
 * Distributed under the MIT License
 */

#region Usings

using System.Text;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// Extension methods for the GeoHash data structure.
    /// </summary>
    public static class GeoHashExtensions
    {

        /// <summary>
        /// Transform the given geo coordinate into a geo hash.
        /// </summary>
        /// <param name="GeoCoordinate">The geo coordinate.</param>
        /// <param name="Precision">An optional precision aka number of characters of the resulting geo hash.</param>
        public static GeoHash ToGeoHash(this GeoCoordinate  GeoCoordinate,
                                        Byte                Precision   = 12)

            => new (GeoCoordinate,
                    Precision);

    }


    /// <summary>
    /// A base32-encoded alphanumeric geo hash.
    /// </summary>
    public readonly struct GeoHash : IGeoHash<String>,
                                     IGeoCoordinate,
                                     IEquatable<GeoHash>,
                                     IComparable<GeoHash>

    {

        #region (enum) Direction

        public enum Direction
        {
            Top     = 0,
            Right   = 1,
            Bottom  = 2,
            Left    = 3
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
        /// The internal geo hash.
        /// </summary>
        private readonly String internalGeoHash;

        #endregion

        #region Properties

        /// <summary>
        /// Rounds the double-precision value to the given number of fractional digits.
        /// </summary>
        public Byte       Digits    { get; }

        /// <summary>
        /// The latitude (south to nord).
        /// </summary>
        public Latitude   Latitude
            => Decode((lat, lon) => lat, Digits);

        /// <summary>
        /// The longitude (parallel to equator).
        /// </summary>
        public Longitude  Longitude
            => Decode((lat, lon) => lon, Digits);


        /// <summary>
        /// Returns the value of the geo hash.
        /// </summary>
        public String     Value
            => internalGeoHash;

        #endregion

        #region Constructor(s)

        #region GeoHash(GeoHash)

        private GeoHash(String GeoHash)
        {
            internalGeoHash  = GeoHash;
            Digits           = 12;
        }

        #endregion

        #region GeoHash(GeoCoordinate, Precision = 12)

        /// <summary>
        /// Create a new base32-encoded alphanumeric geo hash.
        /// </summary>
        /// <param name="GeoCoordinate">A geocoordinate.</param>
        /// <param name="Precision">An optional precision aka number of characters of the resulting geo hash.</param>
        public GeoHash(GeoCoordinate  GeoCoordinate,
                       Byte           Precision   = 12)

            : this(Encode(GeoCoordinate.Latitude,
                          GeoCoordinate.Longitude,
                          Precision))

        { }

        #endregion

        #region GeoHash(Latitude, Longitude, Precision = 12)

        /// <summary>
        /// Create a new base32-encoded alphanumeric geo hash.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="Precision">An optional precision aka number of characters of the resulting geo hash.</param>
        public GeoHash(Latitude   Latitude,
                       Longitude  Longitude,
                       Byte       Precision   = 12)

            : this(Encode(Latitude,
                          Longitude,
                          Precision))

        { }

        #endregion

        #endregion


        #region (private) (static) Encode(Latitude, Longitude, Precision = 12)

        /// <summary>
        /// Encode the given latitude and longitude as geo hash.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="Precision">An optional precision aka number of characters of the resulting geo hash.</param>
        /// <returns>The latitude and longitude encoded as geo hash.</returns>
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
        /// Decode the geo hash into latitude and longitude using the given
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

            foreach (var Character in internalGeoHash)
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
        /// Calculate the adjacent geo hashes.
        /// </summary>
        /// <param name="Direction">The direction.</param>
        public String CalculateAdjacent(Direction Direction)
        {
            return CalculateAdjacent_private(this.internalGeoHash, Direction);
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

        #region Operator == (Left, Right)

        /// <summary>
        /// Compares two geo hashs for equality.
        /// </summary>
        /// <param name="Left">A geo hash.</param>
        /// <param name="Right">Another geo hash.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GeoHash Left,
                                           GeoHash Right)

            => Left.Equals(Right);

        #endregion

        #region Operator != (Left, Right)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Left">A geo hash.</param>
        /// <param name="Right">Another geo hash.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GeoHash Left,
                                           GeoHash Right)

            => !Left.Equals(Right);

        #endregion

        #endregion

        #region IComparable<GeoHash/IGeoCoordinate> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is GeoHash geoHash
                   ? CompareTo(geoHash)
                   : throw new ArgumentException("The given object is not a geo hash!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(GeoHash)

        /// <summary>
        /// Compares two geo hashes.
        /// </summary>
        /// <param name="GeoHash">Another geo hash.</param>
        public Int32 CompareTo(GeoHash GeoHash)

            => Value.CompareTo(GeoHash.Value);

        #endregion

        #region CompareTo(IGeoCoordinate)

        /// <summary>
        /// Compares two geo coordinates.
        /// </summary>
        /// <param name="IGeoCoordinate">Another geo coordinate.</param>
        public Int32 CompareTo(IGeoCoordinate? IGeoCoordinate)
        {

            if (IGeoCoordinate is null)
                throw new ArgumentNullException(nameof(IGeoCoordinate),
                                                "The given object is not a geo coordinate!");

            var c = Latitude. Value.CompareTo(IGeoCoordinate.Latitude. Value);

            if (c == 0)
                c = Longitude.Value.CompareTo(IGeoCoordinate.Longitude.Value);

            //if (c == 0 && Altitude.HasValue && IGeoCoordinate.Altitude.HasValue)
            //    c = Altitude. Value.CompareTo(IGeoCoordinate.Altitude. Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<GeoHash/IGeoCoordinate> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two geo hashes for equality.
        /// </summary>
        /// <param name="Object">Another geo hash.</param>
        public override Boolean Equals(Object? Object)

            => Object is GeoHash geoHash &&
                   Equals(geoHash);

        #endregion

        #region Equals(GeoHash)

        /// <summary>
        /// Compares two geo hashes for equality.
        /// </summary>
        /// <param name="GeoHash">Another geo hash.</param>
        public Boolean Equals(GeoHash GeoHash)

            => Value.Equals(GeoHash.Value);

        #endregion

        #region Equals(IGeoCoordinate)

        /// <summary>
        /// Compares two geo coordinates for equality.
        /// </summary>
        /// <param name="IGeoCoordinate">Another geo coordinate.</param>
        public Boolean Equals(IGeoCoordinate? IGeoCoordinate)

            => IGeoCoordinate is not null                &&
               Latitude. Equals(IGeoCoordinate.Latitude) &&
               Longitude.Equals(IGeoCoordinate.Longitude);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hashcode of this object.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()

            => internalGeoHash.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        public override String ToString()

            => internalGeoHash;

        #endregion

    }

}
