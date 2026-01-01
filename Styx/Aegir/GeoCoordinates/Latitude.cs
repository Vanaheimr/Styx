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

#region Usings

using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// A geographical latitude (south to nord).
    /// </summary>
    public readonly struct Latitude : IEquatable<Latitude>,
                                      IComparable<Latitude>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The numeric value of the latitude.
        /// </summary>
        public Double  Value    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new geographical latitude (south to nord).
        /// </summary>
        /// <param name="Value">The numeric value of the latitude.</param>
        private Latitude(Double Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse        (Number)

        /// <summary>
        /// Parse the given number as a geographical latitude (south to nord).
        /// </summary>
        /// <param name="Number">The numeric representation of a latitude to parse.</param>
        public static Latitude Parse(Double Number)
        {

            if (TryParse(Number, out var latitude))
                return latitude;

            throw new ArgumentException($"Invalid latitude '{Number}'! The value must be between -90 and 90!", nameof(Number));

        }

        #endregion

        #region (static) TryParse     (Number)

        /// <summary>
        /// Try to parse the given number as a geographical latitude (south to nord).
        /// </summary>
        /// <param name="Number">A numeric representation of a latitude.</param>
        public static Latitude? TryParse(Double Number)
        {

            if (TryParse(Number, out var latitude))
                return latitude;

            return null;

        }

        #endregion

        #region (static) TryParse     (Number, out Latitude)

        /// <summary>
        /// Try to parse the given number as a geographical latitude (south to nord).
        /// </summary>
        /// <param name="Number">A numeric representation of a latitude.</param>
        /// <param name="Latitude">The parsed latitude.</param>
        public static Boolean TryParse(Double Number, out Latitude Latitude)
        {

            if (Number >= -90 && Number <= 90)
            {
                Latitude = new Latitude(Number);
                return true;
            }

            Latitude = default;
            return false;

        }

        #endregion


        public static Latitude Parse(String Value)
            => Parse(Double.Parse(Value, CultureInfo.InvariantCulture));

        public static Latitude Parse(String Value, IFormatProvider FormatProvider)
            => Parse(Double.Parse(Value, FormatProvider));

        public static Latitude Parse(String Value, NumberStyles NumberStyle)
            => Parse(Double.Parse(Value, NumberStyle));

        public static Latitude Parse(String Value, NumberStyles NumberStyle, IFormatProvider FormatProvider)
            => Parse(Double.Parse(Value, NumberStyle, FormatProvider));


        #region TryParse     (Text)

        /// <summary>
        /// Try to parse the given string as a geographical latitude (south to nord).
        /// </summary>
        /// <param name="Text">The text representation of a latitude to parse.</param>
        public static Latitude? TryParse(String Text)
        {

            if (TryParse(Text, out var latitude))
                return latitude;

            return null;

        }

        #endregion

        #region TryParse     (Text,               ValueMapper)

        /// <summary>
        /// Try to parse the given string as a geographical latitude (south to nord).
        /// </summary>
        /// <param name="Text">The text representation of a latitude to parse.</param>
        /// <param name="ValueMapper">An optional mapping function to modify the parsed latitude value.</param>
        public static Latitude? TryParse(String Text, Func<Double, Double> ValueMapper)
        {

            if (TryParse(Text, out var latitude, ValueMapper))
                return latitude;

            return null;

        }

        #endregion

        #region TryParse     (Text, out Latitude)

        /// <summary>
        /// Try to parse the given string as a geographical latitude (south to nord).
        /// </summary>
        /// <param name="Text">The text representation of a latitude to parse.</param>
        /// <param name="Latitude">The parsed latitude.</param>
        public static Boolean TryParse(String Text, out Latitude Latitude)

            => TryParse(Text,
                        out Latitude,
                        null);

        #endregion

        #region TryParse     (Text, out Latitude, ValueMapper)

        /// <summary>
        /// Try to parse the given string as a geographical latitude (south to nord).
        /// </summary>
        /// <param name="Text">The text representation of a latitude to parse.</param>
        /// <param name="Latitude">The parsed latitude.</param>
        /// <param name="ValueMapper">An optional mapping function to modify the parsed latitude value.</param>
        public static Boolean TryParse(String                 Text,
                                       out Latitude           Latitude,
                                       Func<Double, Double>?  ValueMapper)
        {

            Latitude = default;

            return Double.TryParse(Text,
                                   NumberStyles.Any,
                                   CultureInfo.InvariantCulture,
                                   out var Value) &&

                   TryParse(ValueMapper is not null
                                ? ValueMapper(Value)
                                : Value,
                            out Latitude);

        }

        #endregion


        #region TryParseNMEA (Text, out Latitude, ValueMapper = null)

        /// <summary>
        /// Try to parse the given NMEA string as a geographical latitude (south to nord).
        /// </summary>
        /// <param name="NMEA"></param>
        /// <param name="Latitude"></param>
        /// <param name="ValueMapper"></param>
        /// <returns></returns>
        public static Boolean TryParseNMEA(String                 NMEA,
                                           out Latitude           Latitude,
                                           Func<Double, Double>?  ValueMapper   = null)
        {

            try
            {

                var dot   = NMEA.IndexOf('.') - 2;
                if (dot < 0) dot = 0;

                var space = NMEA.IndexOf(' ');
                var sign  = NMEA[^1];

                if (space > 0 &&
                    (sign == 'n' || sign == 'N' || sign == 's' || sign == 'S') &&
                    Double.TryParse(NMEA.Substring(0,   dot),       NumberStyles.Any, CultureInfo.InvariantCulture, out var degrees) &&
                    Double.TryParse(NMEA.Substring(dot, space-dot), NumberStyles.Any, CultureInfo.InvariantCulture, out var minutes))
                {

                    var value = degrees + (minutes / 60);

                    if (sign == 's' || sign == 'S')
                        value *= -1;

                    return TryParse(ValueMapper is not null
                                        ? ValueMapper(value)
                                        : value,
                                    out Latitude);

                }

            }
            catch
            { }

            Latitude = default;
            return false;

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this geographical latitude.
        /// </summary>
        public Latitude Clone()

            => new (Value);

        #endregion


        #region Distance(OtherLatitude)

        /// <summary>
        /// Calculate the distance between two latitudes.
        /// </summary>
        /// <param name="OtherLatitude">Another latitude.</param>
        public Double DistanceTo(Latitude OtherLatitude)

            => Math.Abs(Value - OtherLatitude.Value);

        #endregion


        #region Operator overloading

        #region Operator == (Latitude1, Latitude2)

        /// <summary>
        /// Compares two latitudes for equality.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Latitude Latitude1,
                                           Latitude Latitude2)

            => Latitude1.Equals(Latitude2);

        #endregion

        #region Operator != (Latitude1, Latitude2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Latitude Latitude1,
                                           Latitude Latitude2)

            => !Latitude1.Equals(Latitude2);

        #endregion

        #region Operator <  (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  < (Latitude Latitude1,
                                           Latitude Latitude2)

            => Latitude1.CompareTo(Latitude2) < 0;

        #endregion

        #region Operator <= (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Latitude Latitude1,
                                           Latitude Latitude2)

            => Latitude1.CompareTo(Latitude2) <= 0;

        #endregion

        #region Operator >  (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  > (Latitude Latitude1,
                                           Latitude Latitude2)

            => Latitude1.CompareTo(Latitude2) > 0;

        #endregion

        #region Operator >= (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Latitude Latitude1,
                                           Latitude Latitude2)

            => Latitude1.CompareTo(Latitude2) >= 0;

        #endregion

        #endregion

        #region IComparable<Latitude> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two geographical latitudes.
        /// </summary>
        /// <param name="Object">Another geographical latitude.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Latitude latitude
                   ? CompareTo(latitude)
                   : throw new ArgumentException("The given object is not a geographical latitude!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Latitude)

        /// <summary>
        /// Compares two geographical latitudes.
        /// </summary>
        /// <param name="Latitude">Another geographical latitude.</param>
        public Int32 CompareTo(Latitude Latitude)

            => Value.CompareTo(Latitude.Value);

        #endregion

        #endregion

        #region IEquatable<Latitude> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two geographical latitudes for equality.
        /// </summary>
        /// <param name="Object">Another geographical latitude.</param>
        public override Boolean Equals(Object? Object)

            => Object is Latitude latitude &&
                   Equals(latitude);

        #endregion

        #region Equals(Latitude)

        /// <summary>
        /// Compares two geographical latitudes for equality.
        /// </summary>
        /// <param name="Latitude">Another geographical latitude.</param>
        public Boolean Equals(Latitude Latitude)

            => Value.Equals(Latitude.Value);

        #endregion

        #region Equals(Latitude, Epsilon = 1e-7)

        /// <summary>
        /// Compares two geographical latitudes for equality.
        /// </summary>
        /// <param name="Latitude">Another geographical latitude.</param>
        /// <param name="Epsilon">An optional numeric tolerance.</param>
        public Boolean EqualsWithTolerance(Latitude  Latitude,
                                           Double    Epsilon = 1e-7)

            => Value - Latitude.Value < Epsilon;

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
