/*
 * Copyright (c) 2010-2019, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
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
    /// A latitude.
    /// </summary>
    public struct Latitude : IEquatable<Latitude>,
                             IComparable<Latitude>,
                             IComparable

    {

        #region Properties

        /// <summary>
        /// Returns the value of the latitude.
        /// </summary>
        public Double  Value   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new latitude.
        /// </summary>
        /// <param name="Value">The value of the latitude.</param>
        private Latitude(Double Value)
        {
            this.Value = Value;
        }

        #endregion


        public static Latitude Parse(Double Value)
        {

            if (Value < -90 || Value > 90)
                throw new ArgumentException("Invalid latitude value '" + Value + "'! Must be between -90 and 90!", nameof(Value));

            return new Latitude(Value);

        }

        public static Latitude? TryParse(Double Value)
        {

            if (TryParse(Value, out Latitude _Latitude))
                return _Latitude;

            return new Latitude?();

        }

        public static Boolean TryParse(Double Value, out Latitude Latitude)
        {

            if (Value < -90 || Value > 90)
            {
                Latitude = default(Latitude);
                return false;
            }

            Latitude = new Latitude(Value);
            return true;

        }

        public static Latitude Parse(String Value)
            => Parse(Double.Parse(Value, CultureInfo.InvariantCulture));

        public static Latitude Parse(String Value, IFormatProvider FormatProvider)
            => Parse(Double.Parse(Value, FormatProvider));

        public static Latitude Parse(String Value, NumberStyles NumberStyle)
            => Parse(Double.Parse(Value, NumberStyle));

        public static Latitude Parse(String Value, NumberStyles NumberStyle, IFormatProvider FormatProvider)
            => Parse(Double.Parse(Value, NumberStyle, FormatProvider));

        public static Latitude? TryParse(String Text)
        {

            if (TryParse(Text, out Latitude _Latitude))
                return _Latitude;

            return new Latitude?();

        }

        public static Latitude? TryParse(String Text, Func<Double, Double> ValueMapper)
        {

            if (TryParse(Text, out Latitude _Latitude, ValueMapper))
                return _Latitude;

            return new Latitude?();

        }

        public static Boolean TryParse(String Text, out Latitude Latitude, Func<Double, Double> ValueMapper = null)
        {

            try
            {
                if (Double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Double Value))
                {
                    Latitude = new Latitude(ValueMapper != null ? ValueMapper(Value) : Value);
                    return true;
                }
            }
            catch (Exception)
            { }

            Latitude = default(Latitude);
            return false;

        }

        public static Boolean TryParseNema(String Text, out Latitude Latitude, Func<Double, Double> ValueMapper = null)
        {

            try
            {

                var dot   = Text.IndexOf(".") - 2;
                if (dot < 0) dot = 0;

                var space = Text.IndexOf(" ");
                var sign  = Text[Text.Length - 1];

                if (space > 0 &&
                    (sign == 'n' || sign == 'N' || sign == 's' || sign == 'S') &&
                    Double.TryParse(Text.Substring(0,   dot),       NumberStyles.Any, CultureInfo.InvariantCulture, out Double degrees) &&
                    Double.TryParse(Text.Substring(dot, space-dot), NumberStyles.Any, CultureInfo.InvariantCulture, out Double minutes))
                {

                    var value = degrees + (minutes / 60);

                    if (sign == 's' || sign == 'S')
                        value *= -1;

                    return TryParse(ValueMapper != null
                                        ? ValueMapper(value)
                                        : value,
                                    out Latitude);

                }

            }
            catch (Exception)
            { }

            Latitude = default(Latitude);
            return false;

        }


        #region Distance(OtherLatitude)

        /// <summary>
        /// A method to calculate the distance between two latitudes.
        /// </summary>
        /// <param name="OtherLatitude">Another latitude.</param>
        /// <returns>The distance between a and b.</returns>
        public Double DistanceTo(Latitude OtherLatitude)
        {
            return Math.Abs(Value - OtherLatitude.Value);
        }

        #endregion


        #region Operator overloading

        #region Operator == (Latitude1, Latitude2)

        /// <summary>
        /// Compares two latitudes for equality.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Latitude Latitude1, Latitude Latitude2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Latitude1, Latitude2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Latitude1 == null) || ((Object) Latitude2 == null))
                return false;

            return Latitude1.Equals(Latitude2);

        }

        #endregion

        #region Operator != (Latitude1, Latitude2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Latitude Latitude1, Latitude Latitude2)
        {
            return !(Latitude1 == Latitude2);
        }

        #endregion

        #region Operator <  (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  < (Latitude Latitude1, Latitude Latitude2)
        {

            if ((Object) Latitude1 == null)
                throw new ArgumentNullException("The given Latitude1 must not be null!");

            if ((Object) Latitude2 == null)
                throw new ArgumentNullException("The given Latitude2 must not be null!");

            return Latitude1.CompareTo(Latitude2) < 0;

        }

        #endregion

        #region Operator <= (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Latitude Latitude1, Latitude Latitude2)
        {
            return !(Latitude1 > Latitude2);
        }

        #endregion

        #region Operator >  (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  > (Latitude Latitude1, Latitude Latitude2)
        {

            if ((Object) Latitude1 == null)
                throw new ArgumentNullException("The given Latitude1 must not be null!");

            if ((Object) Latitude2 == null)
                throw new ArgumentNullException("The given Latitude2 must not be null!");

            return Latitude1.CompareTo(Latitude2) > 0;

        }

        #endregion

        #region Operator >= (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Latitude Latitude1, Latitude Latitude2)
        {
            return !(Latitude1 < Latitude2);
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

            return CompareTo((Latitude) Object);

        }

        #endregion

        #region CompareTo(Latitude)

        /// <summary>
        /// Compares two latitudes.
        /// </summary>
        /// <param name="Latitude">Another latitude.</param>
        public Int32 CompareTo(Latitude Latitude)
        {
            return this.Value.CompareTo(Latitude.Value);
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
                return this.Equals((Latitude) Object);
            }
            catch (InvalidCastException)
            {
                return false;
            }

        }

        #endregion

        #region Equals(Latitude)

        /// <summary>
        /// Compares two latitudes for equality.
        /// </summary>
        /// <param name="Latitude">Another latitude.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(Latitude Latitude)
        {
            return this.Value.Equals(Latitude.Value);
        }

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
            => Value.ToString();

        #endregion

        #region ToString(FormatProvider)

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <param name="FormatProvider">An object that supplies culture-specific formatting information.</param>
        public String ToString(IFormatProvider FormatProvider)
        {
            return this.Value.ToString(FormatProvider);
        }

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
