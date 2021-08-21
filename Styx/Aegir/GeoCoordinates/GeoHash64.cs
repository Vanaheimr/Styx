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
 * Idea based upon: http://github.com/karussell/GraphHopper
 * 2011 (c) Peter Karich
 * Distributed under the Apache License 2
 */

#region Usings

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    #region GeoHash64Extentions

    /// <summary>
    /// Extention methods for the GeoHash64 data structure.
    /// </summary>
    public static class GeoHash64Extentions
    {

        /// <summary>
        /// Transform the given geo coordinate into a geohash64.
        /// </summary>
        /// <param name="GeoCoordinate">The geo coordinate.</param>
        /// <param name="Precision">An optional precision aka number of bits of the resulting geohash (1-32 bit).</param>
        public static GeoHash64 ToGeoHash64(this GeoCoordinate GeoCoordinate, Byte Precision = 32)
        {
            return new GeoHash64(GeoCoordinate, Precision);
        }

    }

    #endregion

    #region GeoHash64

    /// <summary>
    /// An UInt64-encoded geohash, which has a
    /// precision of approx 9,3 mm = 40075m/2^32.
    /// </summary>
    public struct GeoHash64 : IGeoHash<UInt64>,
                              IEquatable<GeoHash64>,
                              IComparable<GeoHash64>

    {

        #region Data

        /// <summary>
        /// The internal geohash.
        /// </summary>
        private readonly UInt64 InternalGeoHash;

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
        public UInt64 Value
        {
            get
            {
                return this.InternalGeoHash;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region GeoHash64(GeoHash)

        private GeoHash64(UInt64 GeoHash)
        {
            InternalGeoHash = GeoHash;
            _Digits         = 12;
        }

        #endregion

        #region GeoHash64(GeoCoordinate, Precision = 32)

        /// <summary>
        /// Create a new base32-encoded alphanumeric geohash.
        /// </summary>
        /// <param name="GeoCoordinate">A geocoordinate.</param>
        /// <param name="Precision">An optional precision aka number of bits of the resulting geohash (1-32 bit).</param>
        public GeoHash64(GeoCoordinate GeoCoordinate, Byte Precision = 32)
            : this(Encode(GeoCoordinate.Latitude, GeoCoordinate.Longitude, Precision))
        { }

        #endregion

        #region GeoHash64(Latitude, Longitude, Precision = 32)

        /// <summary>
        /// Create a new base32-encoded alphanumeric geohash.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="Precision">An optional precision aka number of bits of the resulting geohash (1-32 bit).</param>
        public GeoHash64(Latitude Latitude, Longitude Longitude, Byte Precision = 32)
            : this(Encode(Latitude, Longitude, Precision))
        { }

        #endregion

        #endregion


        #region (private) (static) Encode(Latitude, Longitude, Precision = 32)

        /// <summary>
        /// Encode the given latitude and longitude as geohash.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="Precision">An optional precision aka number of bits of the resulting geohash (1-32 bit).</param>
        /// <returns>The latitude and longitude encoded as geohash.</returns>
        private static UInt64 Encode(Latitude Latitude, Longitude Longitude, Byte Precision = 32)
        {

            var _GeoHash      = 0UL;
            var _MidLatitude  = 0.0;
            var _MidLongitude = 0.0;
            var _MinLatitude  =  -90.0;
            var _MaxLatitude  =   90.0;
            var _MinLongitude = -180.0;
            var _MaxLongitude =  180.0;

            if (Precision > 32)
                Precision = 32;

            for (var i = 0; i < Precision; i++)
            {

                // Shrink latitude intervall
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

                // Shrink longitude intervall
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

                if (i < Precision-1)
                    _GeoHash <<= 1;

            }

            return _GeoHash;

        }

        #endregion

        #region Decode<T>(Processor, Digits = 12)

        private void RefineInterval(ref Double[] Interval, UInt64 Bitmask)
        {

            if ((this.InternalGeoHash & Bitmask) != 0)
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

            var bitmask = 1UL << 63;
            Double[] LatitudeInterval  = {  -90,  90 };
            Double[] LongitudeInterval = { -180, 180 };

            while (bitmask > 0)
            {

                RefineInterval(ref LatitudeInterval, bitmask);
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

        #region Operator == (GeoHash641, GeoHash642)

        /// <summary>
        /// Compares two geohashs for equality.
        /// </summary>
        /// <param name="GeoHash641">A geohash.</param>
        /// <param name="GeoHash642">Another geohash.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GeoHash64 GeoHash641, GeoHash64 GeoHash642)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(GeoHash641, GeoHash642))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GeoHash641 == null) || ((Object) GeoHash642 == null))
                return false;

            return GeoHash641.Equals(GeoHash642);

        }

        #endregion

        #region Operator != (GeoHash641, GeoHash642)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="GeoHash641">A geohash.</param>
        /// <param name="GeoHash642">Another geohash.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GeoHash64 GeoHash641, GeoHash64 GeoHash642)
        {
            return !(GeoHash641 == GeoHash642);
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

            return CompareTo((GeoHash64) Object);

        }

        #endregion

        #region CompareTo(GeoHash64)

        /// <summary>
        /// Compares two geohashes.
        /// </summary>
        /// <param name="GeoHash64">Another geohash.</param>
        public Int32 CompareTo(GeoHash64 GeoHash64)
        {
            return this.Value.CompareTo(GeoHash64.Value);
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
                return this.Equals((GeoHash64) Object);
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

        #region Equals(GeoHash64)

        /// <summary>
        /// Compares two geohashes for equality.
        /// </summary>
        /// <param name="GeoHash64">Another geohash.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(GeoHash64 GeoHash64)
        {
            return this.Value.Equals(GeoHash64.Value);
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
            return this.InternalGeoHash.ToString();
        }

        #endregion

    }

    #endregion

}
