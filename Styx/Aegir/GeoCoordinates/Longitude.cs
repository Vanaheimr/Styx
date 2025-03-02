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

#region Usings

using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// A geographical longitude (parallel to equator).
    /// </summary>
    public readonly struct Longitude : IEquatable<Longitude>,
                                       IComparable<Longitude>,
                                       IComparable
    {

        #region Properties

        /// <summary>
        /// The numeric value of the longitude.
        /// </summary>
        public Double  Value    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new geographical longitude (parallel to equator).
        /// </summary>
        /// <param name="Value">The numeric value of the longitude.</param>
        private Longitude(Double Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse    (Number)

        /// <summary>
        /// Parse the given number as a geographical longitude (parallel to equator).
        /// </summary>
        /// <param name="Number">A numeric representation of a longitude.</param>
        public static Longitude Parse(Double Number)
        {

            if (TryParse(Number, out var longitude))
                return longitude;

            throw new ArgumentException($"Invalid longitude '{Number}'! The value must be between -180 and 180!", nameof(Number));

        }

        #endregion

        #region (static) TryParse (Number)

        /// <summary>
        /// Try to parse the given number as a geographical longitude (parallel to equator).
        /// </summary>
        /// <param name="Number">A numeric representation of a longitude.</param>
        public static Longitude? TryParse(Double Number)
        {

            if (TryParse(Number, out var longitude))
                return longitude;

            return null;

        }

        #endregion

        #region (static) TryParse (Number, out Longitude)

        /// <summary>
        /// Try to parse the given number as a geographical longitude (parallel to equator).
        /// </summary>
        /// <param name="Number">A numeric representation of a longitude.</param>
        /// <param name="Longitude">The parsed longitude.</param>
        public static Boolean TryParse(Double Number, out Longitude Longitude)
        {

            if (Number >= -180 && Number <= 180)
            {
                Longitude = new Longitude(Number);
                return true;
            }

            Longitude = default;
            return false;

        }

        #endregion


        public static Longitude Parse(String Value)
            => Parse(Double.Parse(Value, CultureInfo.InvariantCulture));

        public static Longitude Parse(String Value, IFormatProvider FormatProvider)
            => Parse(Double.Parse(Value, FormatProvider));

        public static Longitude Parse(String Value, NumberStyles NumberStyle)
            => Parse(Double.Parse(Value, NumberStyle));

        public static Longitude Parse(String Value, NumberStyles NumberStyle, IFormatProvider FormatProvider)
            => Parse(Double.Parse(Value, NumberStyle, FormatProvider));



        #region TryParse     (Text)

        /// <summary>
        /// Try to parse the given string as a geographical longitude (south to nord).
        /// </summary>
        /// <param name="Text">The text representation of a longitude to parse.</param>
        public static Longitude? TryParse(String Text)
        {

            if (TryParse(Text, out var longitude))
                return longitude;

            return null;

        }

        #endregion

        #region TryParse     (Text,                ValueMapper)

        /// <summary>
        /// Try to parse the given string as a geographical longitude (south to nord).
        /// </summary>
        /// <param name="Text">The text representation of a longitude to parse.</param>
        /// <param name="ValueMapper">An optional mapping function to modify the parsed longitude value.</param>
        public static Longitude? TryParse(String Text, Func<Double, Double>? ValueMapper)
        {

            if (TryParse(Text, out var longitude, ValueMapper))
                return longitude;

            return null;

        }

        #endregion

        #region TryParse     (Text, out Longitude)

        /// <summary>
        /// Try to parse the given string as a geographical longitude (south to nord).
        /// </summary>
        /// <param name="Text">The text representation of a longitude to parse.</param>
        /// <param name="Longitude">The parsed longitude.</param>
        public static Boolean TryParse(String Text, out Longitude Longitude)

            => TryParse(Text,
                        out Longitude,
                        null);

        #endregion

        #region TryParse     (Text, out Longitude, ValueMapper)

        /// <summary>
        /// Try to parse the given string as a geographical longitude (south to nord).
        /// </summary>
        /// <param name="Text">The text representation of a longitude to parse.</param>
        /// <param name="Longitude">The parsed longitude.</param>
        /// <param name="ValueMapper">An optional mapping function to modify the parsed longitude value.</param>
        public static Boolean TryParse(String                 Text,
                                       out Longitude          Longitude,
                                       Func<Double, Double>?  ValueMapper)
        {

            Longitude = default;

            if (Double.TryParse(Text,
                                NumberStyles.Any,
                                CultureInfo.InvariantCulture,
                                out var Value))
            {

                Longitude = new Longitude(ValueMapper is not null
                                              ? ValueMapper(Value)
                                              : Value);

                return true;

            }

            return false;

        }

        #endregion


        #region TryParseNMEA (NMEA, out Longitude, ValueMapper = null)

        /// <summary>
        /// Try to parse the given NMEA string as a geographical longitude (parallel to equator).
        /// </summary>
        /// <param name="NMEA"></param>
        /// <param name="Longitude"></param>
        /// <param name="ValueMapper"></param>
        public static Boolean TryParseNMEA(String                 NMEA,
                                           out Longitude          Longitude,
                                           Func<Double, Double>?  ValueMapper   = null)
        {

            try
            {

                var dot   = NMEA.IndexOf('.') - 2;
                if (dot < 0) dot = 0;

                var space = NMEA.IndexOf(' ');
                var sign  = NMEA[^1];

                if (space > 0 &&
                    (sign == 'e' || sign == 'E' || sign == 'w' || sign == 'W') &&
                    Double.TryParse(NMEA.Substring(0,   dot),       NumberStyles.Any, CultureInfo.InvariantCulture, out var degrees) &&
                    Double.TryParse(NMEA.Substring(dot, space-dot), NumberStyles.Any, CultureInfo.InvariantCulture, out var minutes))
                {

                    var value = degrees + (minutes / 60);

                    if (sign == 'w' || sign == 'W')
                        value *= -1;

                    return TryParse(ValueMapper is not null
                                        ? ValueMapper(value)
                                        : value,
                                    out Longitude);

                }

            }
            catch
            { }

            Longitude = default;
            return false;

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this geographical longitude.
        /// </summary>
        public Longitude Clone()

            => new (Value);

        #endregion


        #region Distance(OtherLongitude)

        /// <summary>
        /// Calculate the distance between two longitude.
        /// </summary>
        /// <param name="OtherLongitude">Another longitude.</param>
        public Double DistanceTo(Longitude OtherLongitude)
        {
            return Math.Abs(Value - OtherLongitude.Value);
        }

        #endregion


        #region Operator overloading

        #region Operator == (Longitude1, Longitude2)

        /// <summary>
        /// Compares two longitudes for equality.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Longitude Longitude1,
                                           Longitude Longitude2)

            => Longitude1.Equals(Longitude2);

        #endregion

        #region Operator != (Longitude1, Longitude2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Longitude Longitude1,
                                           Longitude Longitude2)

            => !Longitude1.Equals(Longitude2);

        #endregion

        #region Operator <  (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  < (Longitude Longitude1,
                                           Longitude Longitude2)

            => Longitude1.CompareTo(Longitude2) < 0;

        #endregion

        #region Operator <= (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Longitude Longitude1,
                                           Longitude Longitude2)

            => Longitude1.CompareTo(Longitude2) <= 0;

        #endregion

        #region Operator >  (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  > (Longitude Longitude1,
                                           Longitude Longitude2)

            => Longitude1.CompareTo(Longitude2) > 0;

        #endregion

        #region Operator >= (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Longitude Longitude1,
                                           Longitude Longitude2)

            => Longitude1.CompareTo(Longitude2) >= 0;

        #endregion

        #endregion

        #region IComparable Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two geographical longitudes.
        /// </summary>
        /// <param name="Object">Another geographical longitude.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Longitude longitude
                   ? CompareTo(longitude)
                   : throw new ArgumentException("The given object is not a geographical longitude!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Longitude)

        /// <summary>
        /// Compares two geographical longitudes.
        /// </summary>
        /// <param name="Longitude">Another geographical longitude.</param>
        public Int32 CompareTo(Longitude Longitude)

            => Value.CompareTo(Longitude.Value);

        #endregion

        #endregion

        #region IEquatable Members

        #region Equals(Object)

        /// <summary>
        /// Compares two geographical longitudes for equality.
        /// </summary>
        /// <param name="Object">Another geographical longitude.</param>
        public override Boolean Equals(Object? Object)

            => Object is Longitude longitude &&
                   Equals(longitude);

        #endregion

        #region Equals(Longitude)

        /// <summary>
        /// Compares two geographical longitudes for equality.
        /// </summary>
        /// <param name="Longitude">Another geographical longitude.</param>
        public Boolean Equals(Longitude Longitude)

            => Value.Equals(Longitude.Value);

        #endregion

        #region Equals(Longitude, Epsilon = 1e-7)

        /// <summary>
        /// Compares two geographical longitudes for equality.
        /// </summary>
        /// <param name="Longitude">Another geographical longitude.</param>
        /// <param name="Epsilon">An optional numeric tolerance.</param>
        public Boolean EqualsWithTolerance(Longitude  Longitude,
                                           Double     Epsilon = 1e-7)

            => Value - Longitude.Value < Epsilon;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        public override String ToString()

            => Value.ToString(CultureInfo.InvariantCulture);

        #endregion

        #region ToString(FormatProvider)

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <param name="FormatProvider">An object that supplies culture-specific formatting information.</param>
        public String ToString(IFormatProvider FormatProvider)

            => Value.ToString(FormatProvider);

        #endregion

        #region ToString(Format)

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <param name="Format">A composite format string</param>
        public String ToString(String Format)

            => String.Format(Format, Value);

        #endregion

    }

}
