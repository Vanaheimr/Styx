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
    /// An altitude.
    /// </summary>
    public readonly struct Altitude : IEquatable<Altitude>,
                                      IComparable<Altitude>,
                                      IComparable

    {

        #region Properties

        /// <summary>
        /// The value of the altitude.
        /// </summary>
        public Double  Value    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new altitude.
        /// </summary>
        /// <param name="Value">The value of the altitude.</param>
        private Altitude(Double Value)
        {
            this.Value = Value;
        }

        #endregion


        public static Altitude Parse(Double Longitude)
            => new Altitude(Longitude);

        public static Altitude Parse(String Altitude)
            => new Altitude(Double.Parse(Altitude, CultureInfo.InvariantCulture));

        public static Altitude Parse(String Altitude, IFormatProvider FormatProvider)
            => new Altitude(Double.Parse(Altitude, FormatProvider));

        public static Altitude Parse(String Altitude, NumberStyles NumberStyle)
            => new Altitude(Double.Parse(Altitude, NumberStyle));

        public static Altitude Parse(String Altitude, NumberStyles NumberStyle, IFormatProvider FormatProvider)
            => new Altitude(Double.Parse(Altitude, NumberStyle, FormatProvider));

        public static Boolean TryParse(String Text, out Altitude Altitude)
        {
            try
            {
                if (Double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Double Value))
                {
                    Altitude = new Altitude(Value);
                    return true;
                }
            }
            catch
            { }

            Altitude = default;
            return false;

        }

        public static Boolean TryParse(Double Value, out Altitude Altitude)
        {

            try
            {
                Altitude = new Altitude(Value);
                return true;
            }
            catch
            { }

            Altitude = default;
            return false;

        }


        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Altitude Clone()

            => new (Value);

        #endregion


        #region Distance(OtherAltitude)

        /// <summary>
        /// A method to calculate the distance between two altitude.
        /// </summary>
        /// <param name="OtherAltitude">Another Altitude.</param>
        /// <returns>The distance between a and b.</returns>
        public Double DistanceTo(Altitude OtherAltitude)

            => Math.Abs(Value - OtherAltitude.Value);

        #endregion


        #region Operator overloading

        #region Operator == (Altitude1, Altitude2)

        /// <summary>
        /// Compares two altitudes for equality.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Altitude Altitude1, Altitude Altitude2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Altitude1, Altitude2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Altitude1 == null) || ((Object) Altitude2 == null))
                return false;

            return Altitude1.Equals(Altitude2);

        }

        #endregion

        #region Operator != (Altitude1, Altitude2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Altitude Altitude1, Altitude Altitude2)
        {
            return !(Altitude1 == Altitude2);
        }

        #endregion

        #region Operator <  (Altitude1, Altitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  < (Altitude Altitude1, Altitude Altitude2)
        {

            if ((Object) Altitude1 == null)
                throw new ArgumentNullException("The given Altitude1 must not be null!");

            if ((Object) Altitude2 == null)
                throw new ArgumentNullException("The given Altitude2 must not be null!");

            return Altitude1.CompareTo(Altitude2) < 0;

        }

        #endregion

        #region Operator <= (Altitude1, Altitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Altitude Altitude1, Altitude Altitude2)
        {
            return !(Altitude1 > Altitude2);
        }

        #endregion

        #region Operator >  (Altitude1, Altitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  > (Altitude Altitude1, Altitude Altitude2)
        {

            if ((Object) Altitude1 == null)
                throw new ArgumentNullException("The given Altitude1 must not be null!");

            if ((Object) Altitude2 == null)
                throw new ArgumentNullException("The given Altitude2 must not be null!");

            return Altitude1.CompareTo(Altitude2) > 0;

        }

        #endregion

        #region Operator >= (Altitude1, Altitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Altitude Altitude1, Altitude Altitude2)
        {
            return !(Altitude1 < Altitude2);
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

            return CompareTo((Altitude) Object);

        }

        #endregion

        #region CompareTo(Altitude)

        /// <summary>
        /// Compares two altitudes.
        /// </summary>
        /// <param name="Altitude">Another altitude.</param>
        public Int32 CompareTo(Altitude Altitude)
        {
            return this.Value.CompareTo(Altitude.Value);
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

            => Object is Altitude altitude &&
                   Equals(altitude);

        #endregion

        #region Equals(Altitude)

        /// <summary>
        /// Compares two altitudes for equality.
        /// </summary>
        /// <param name="Altitude">Another altitude.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(Altitude Altitude)

            => Value.Equals(Altitude.Value);

        #endregion

        #endregion

        #region (override) GetHashCode()

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

    }

}
