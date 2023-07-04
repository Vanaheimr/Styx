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

#region Usings

using System;
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
        /// Returns the value of the longitude.
        /// </summary>
        public Double  Value    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new geographical longitude (parallel to equator).
        /// </summary>
        /// <param name="Value">The value of the longitude.</param>
        public Longitude(Double Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse   (Value)

        /// <summary>
        /// Parse the given string as a geographical longitude (parallel to equator).
        /// </summary>
        /// <param name="Text">A text representation of a longitude.</param>
        public static Longitude Parse(Double Value)
        {

            if (TryParse(Value, out var longitude))
                return longitude;

            throw new ArgumentException("Invalid longitude '" + Value + "'! The value must be between -180 and 180!", nameof(Value));

        }

        #endregion

        #region (static) TryParse(Value)

        /// <summary>
        /// Try to parse the given string as a geographical longitude (parallel to equator).
        /// </summary>
        /// <param name="Text">A text representation of a longitude.</param>
        public static Longitude? TryParse(Double Value)
        {

            if (TryParse(Value, out var longitude))
                return longitude;

            return null;

        }

        #endregion

        #region (static) TryParse(Value, out Longitude)

        /// <summary>
        /// Try to parse the given string as a geographical longitude (parallel to equator).
        /// </summary>
        /// <param name="Value">A nummeric representation of a longitude.</param>
        /// <param name="Longitude">The parsed longitude.</param>
        public static Boolean TryParse(Double Value, out Longitude Longitude)
        {

            if (Value >= -180 && Value <= 180)
            {
                Longitude = new Longitude(Value);
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


        public static Longitude? TryParse(String Text)
        {

            if (TryParse(Text, out Longitude longitude))
                return longitude;

            return null;

        }

        public static Longitude? TryParse(String Text, Func<Double, Double>? ValueMapper)
        {

            if (TryParse(Text, out Longitude longitude, ValueMapper))
                return longitude;

            return null;

        }

        public static Boolean TryParse(String Text, out Longitude Longitude)

            => TryParse(Text,
                        out Longitude,
                        null);

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

        public static Boolean TryParseNMEA(String                 Text,
                                           out Longitude          Longitude,
                                           Func<Double, Double>?  ValueMapper   = null)
        {

            try
            {

                var dot   = Text.IndexOf(".") - 2;
                if (dot < 0) dot = 0;

                var space = Text.IndexOf(" ");
                var sign  = Text[Text.Length - 1];

                if (space > 0 &&
                    (sign == 'e' || sign == 'E' || sign == 'w' || sign == 'W') &&
                    Double.TryParse(Text.Substring(0,   dot),       NumberStyles.Any, CultureInfo.InvariantCulture, out var degrees) &&
                    Double.TryParse(Text.Substring(dot, space-dot), NumberStyles.Any, CultureInfo.InvariantCulture, out var minutes))
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


        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Longitude Clone()

            => new (Value);

        #endregion


        #region Distance(OtherLongitude)

        /// <summary>
        /// A method to calculate the distance between two longitude.
        /// </summary>
        /// <param name="OtherLongitude">Another longitude.</param>
        /// <returns>The distance between a and b.</returns>
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
        public static Boolean operator == (Longitude Longitude1, Longitude Longitude2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Longitude1, Longitude2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Longitude1 == null) || ((Object) Longitude2 == null))
                return false;

            return Longitude1.Equals(Longitude2);

        }

        #endregion

        #region Operator != (Longitude1, Longitude2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Longitude Longitude1, Longitude Longitude2)
            => !(Longitude1 == Longitude2);

        #endregion

        #region Operator <  (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  < (Longitude Longitude1, Longitude Longitude2)
        {

            if ((Object) Longitude1 == null)
                throw new ArgumentNullException("The given Longitude1 must not be null!");

            if ((Object) Longitude2 == null)
                throw new ArgumentNullException("The given Longitude2 must not be null!");

            return Longitude1.CompareTo(Longitude2) < 0;

        }

        #endregion

        #region Operator <= (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Longitude Longitude1, Longitude Longitude2)
            => !(Longitude1 > Longitude2);

        #endregion

        #region Operator >  (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  > (Longitude Longitude1, Longitude Longitude2)
        {

            if ((Object) Longitude1 == null)
                throw new ArgumentNullException("The given Longitude1 must not be null!");

            if ((Object) Longitude2 == null)
                throw new ArgumentNullException("The given Longitude2 must not be null!");

            return Longitude1.CompareTo(Longitude2) > 0;

        }

        #endregion

        #region Operator >= (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Longitude Longitude1, Longitude Longitude2)
            => !(Longitude1 < Longitude2);

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

            if (!(Object is Longitude Longitude))
                throw new ArgumentException("The given object is not a longitude!", nameof(Object));

            return CompareTo(Longitude);

        }

        #endregion

        #region CompareTo(Longitude)

        /// <summary>
        /// Compares two longitudes.
        /// </summary>
        /// <param name="Longitude">Another longitude.</param>
        public Int32 CompareTo(Longitude Longitude)

            => Value.CompareTo(Longitude.Value);

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

            if (!(Object is Longitude Longitude))
                return false;

            return Equals(Longitude);

        }

        #endregion

        #region Equals(Longitude)

        /// <summary>
        /// Compares two longitudes for equality.
        /// </summary>
        /// <param name="Longitude">Another longitude.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(Longitude Longitude)

            => Value.Equals(Longitude.Value);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hashcode of this object.
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
