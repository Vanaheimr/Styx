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

/*
 * Idea based upon: http://github.com/karussell/GraphHopper
 * 2011 (c) Peter Karich
 * Distributed under the Apache License 2
 */

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// Extension methods for the GeoHash32 data structure.
    /// </summary>
    public static class GeoHash32Extensions
    {

        /// <summary>
        /// Transform the given geo coordinate into a 32 bit geo hash.
        /// </summary>
        /// <param name="GeoCoordinate">The geo coordinate.</param>
        /// <param name="Precision">TAn optional precision aka number of bits of the resulting geo hash (1-16 bit).</param>
        public static GeoHash32 ToGeoHash32(this GeoCoordinate  GeoCoordinate,
                                            Byte                Precision   = 16)

            => new (GeoCoordinate,
                    Precision);

    }


    /// <summary>
    /// An UInt32-encoded geo hash, which has a precision of approx 611 m = 40075m/2^16.
    /// </summary>
    public readonly struct GeoHash32 : IGeoHash<UInt32>,
                                       IEquatable<GeoHash32>,
                                       IComparable<GeoHash32>

    {

        #region Data

        /// <summary>
        /// The internal geo hash.
        /// </summary>
        private readonly UInt32 internalGeoHash;

        #endregion

        #region Properties

        /// <summary>
        /// Rounds the double-precision value to the given number of fractional digits.
        /// </summary>
        public Byte       Digits    { get; }


        /// <summary>
        /// The latitude of the geo hash (south to nord).
        /// </summary>
        public Latitude   Latitude
            => Decode((lat, lon) => lat, Digits);

        /// <summary>
        /// The longitude of the geo hash (parallel to equator).
        /// </summary>
        public Longitude  Longitude
            => Decode((lat, lon) => lon, Digits);


        /// <summary>
        /// The value of the geo hash.
        /// </summary>
        public UInt32     Value
            => internalGeoHash;

        #endregion

        #region Constructor(s)

        #region GeoHash32(GeoHash)

        private GeoHash32(UInt32 GeoHash)
        {
            internalGeoHash  = GeoHash;
            Digits           = 12;
        }

        #endregion

        #region GeoHash32(GeoCoordinate, Precision = 16)

        /// <summary>
        /// Create a new 32 bit geo hash.
        /// </summary>
        /// <param name="GeoCoordinate">A geo coordinate.</param>
        /// <param name="Precision">An optional precision aka number of bits of the resulting geo hash (1-16 bit).</param>
        public GeoHash32(GeoCoordinate  GeoCoordinate,
                         Byte           Precision   = 16)

            : this(Encode(GeoCoordinate.Latitude,
                          GeoCoordinate.Longitude,
                          Precision))

        { }

        #endregion

        #region GeoHash32(Latitude, Longitude, Precision = 16)

        /// <summary>
        /// Create a new 32 bit geo hash.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="Precision">An optional precision aka number of bits of the resulting geo hash (1-16 bit).</param>
        public GeoHash32(Latitude   Latitude,
                         Longitude  Longitude,
                         Byte       Precision   = 16)

            : this(Encode(Latitude,
                          Longitude,
                          Precision))

        { }

        #endregion

        #endregion


        #region (private) (static) Encode(Latitude, Longitude, Precision = 16)

        /// <summary>
        /// Encode the given latitude and longitude as geo hash.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="Precision">An optional precision aka number of bits of the resulting geo hash.</param>
        /// <returns>The latitude and longitude encoded as geo hash.</returns>
        private static UInt32 Encode(Latitude Latitude, Longitude Longitude, Byte Precision = 16)
        {

            var _GeoHash      = 0U;
            var _MidLatitude  = 0.0;
            var _MidLongitude = 0.0;
            var _MinLatitude  =  -90.0;
            var _MaxLatitude  =   90.0;
            var _MinLongitude = -180.0;
            var _MaxLongitude =  180.0;

            if (Precision > 16)
                Precision = 16;

            for (var i = 0; i < Precision; i++)
            {

                // Shrink latitude interval
                if (_MinLatitude < _MaxLatitude)
                {

                    _MidLatitude = (_MinLatitude + _MaxLatitude) / 2;

                    if (Latitude.Value > _MidLatitude)
                    {
                        _GeoHash |= 1;
                        _MinLatitude = _MidLatitude;
                    }

                    else
                        _MaxLatitude = _MidLatitude;

                }

                _GeoHash <<= 1;

                // Shrink longitude interval
                if (_MinLongitude < _MaxLongitude)
                {

                    _MidLongitude = (_MinLongitude + _MaxLongitude) / 2;

                    if (Longitude.Value > _MidLongitude)
                    {
                        _GeoHash |= 1;
                        _MinLongitude = _MidLongitude;
                    }

                    else
                        _MaxLongitude = _MidLongitude;

                }

                if (i < Precision - 1)
                    _GeoHash <<= 1;

            }

            return _GeoHash;

        }

        #endregion

        #region Decode<T>(Processor, Digits = 12)

        public void RefineInterval(ref Double[] interval, UInt32 Bitmask)
        {

            if ((this.internalGeoHash & Bitmask) != 0)
                interval[0] = (interval[0] + interval[1]) / 2; // Min

            else
                interval[1] = (interval[0] + interval[1]) / 2; // Max

        }

        /// <summary>
        /// Decode the geo hash into latitude and longitude using the given
        /// delegate to transform it into the resulting data structure.
        /// </summary>
        /// <typeparam name="T">The type of the resulting data structure.</typeparam>
        /// <param name="Processor">A delegate to transform the decoded latitude and longitude into the resulting data structure.</param>
        /// <param name="Digits">Rounds the double-precision latitude and longitude to the given number of fractional digits.</param>
        public T Decode<T>(Func<Latitude, Longitude, T> Processor, Byte Digits = 12)
        {

            var bitmask = 1U << 31;
            Double[] LatitudeInterval  = {  -90,  90 };
            Double[] LongitudeInterval = { -180, 180 };

            while (bitmask > 0)
            {

                RefineInterval(ref LatitudeInterval,  bitmask);
                bitmask >>= 1;

                RefineInterval(ref LongitudeInterval, bitmask);
                bitmask >>= 1;

            }

            return Processor(
                Latitude. Parse(Math.Round((LatitudeInterval[0]  + LatitudeInterval[1])  / 2, Digits)),
                Longitude.Parse(Math.Round((LongitudeInterval[0] + LongitudeInterval[1]) / 2, Digits))
            );

        }

        #endregion


        #region Operator overloading

        #region Operator == (Left, Right)

        /// <summary>
        /// Compares two geo hashes for equality.
        /// </summary>
        /// <param name="Left">A geo hash.</param>
        /// <param name="Right">Another geo hash.</param>
        public static Boolean operator == (GeoHash32 Left,
                                           GeoHash32 Right)

            => Left.Equals(Right);

        #endregion

        #region Operator != (Left, Right)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Left">A geo hash.</param>
        /// <param name="Right">Another geo hash.</param>
        public static Boolean operator != (GeoHash32 Left,
                                           GeoHash32 Right)

            => !Left.Equals(Right);

        #endregion

        #endregion

        #region IComparable<GeoHash32/IGeoCoordinate> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two 32 bit geo hashes.
        /// </summary>
        /// <param name="Object">Another 32 bit geo hash.</param>
        public Int32 CompareTo(Object? Object)

            => Object is GeoHash32 geoHash32
                   ? CompareTo(geoHash32)
                   : throw new ArgumentException("The given object is not a 32 bit geo hash!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(GeoHash32)

        /// <summary>
        /// Compares two 32 bit geo hashes.
        /// </summary>
        /// <param name="GeoHash32">Another 32 bit geo hash.</param>
        public Int32 CompareTo(GeoHash32 GeoHash32)

            => Value.CompareTo(GeoHash32.Value);

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

        #region IEquatable<GeoHash32/IGeoCoordinate> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two 32 bit geo hashes for equality.
        /// </summary>
        /// <param name="Object">Another 32 bit geo hash.</param>
        public override Boolean Equals(Object? Object)

            => Object is GeoHash32 geoHash32 &&
                   Equals(geoHash32);

        #endregion

        #region Equals(GeoHash32)

        /// <summary>
        /// Compares two 32 bit geo hashes for equality.
        /// </summary>
        /// <param name="GeoHash32">Another 32 bit geo hash.</param>
        public Boolean Equals(GeoHash32 GeoHash32)

            => Value.Equals(GeoHash32.Value);

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
        /// Return the hash code of this object.
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

             => internalGeoHash.ToString();

        #endregion

    }

}
