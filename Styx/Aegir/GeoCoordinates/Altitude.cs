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
    /// An geographical altitude.
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
        /// Create a new geographical altitude.
        /// </summary>
        /// <param name="Value">The numeric value of the altitude.</param>
        private Altitude(Double Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse    (Number)

        /// <summary>
        /// Parse the given number as a geographical altitude.
        /// </summary>
        /// <param name="Number">A numeric representation of a altitude.</param>
        public static Altitude Parse(Double Number)
        {

            if (TryParse(Number, out var altitude))
                return altitude;

            throw new ArgumentException($"Invalid altitude '{Number}'! The value must be between -180 and 180!", nameof(Number));

        }

        #endregion

        #region (static) TryParse (Number)

        /// <summary>
        /// Try to parse the given number as a geographical altitude.
        /// </summary>
        /// <param name="Number">A numeric representation of a altitude.</param>
        public static Altitude? TryParse(Double Number)
        {

            if (TryParse(Number, out var altitude))
                return altitude;

            return null;

        }

        #endregion

        #region (static) TryParse (Number, out Altitude)

        /// <summary>
        /// Try to parse the given number as a geographical altitude.
        /// </summary>
        /// <param name="Number">A numeric representation of a altitude.</param>
        /// <param name="Altitude">The parsed altitude.</param>
        public static Boolean TryParse(Double Number, out Altitude Altitude)
        {

            try
            {
                Altitude = new Altitude(Number);
                return true;
            }
            catch
            { }

            Altitude = default;
            return false;

        }

        #endregion


        public static Altitude Parse(String Altitude)
            => Parse(Double.Parse(Altitude, CultureInfo.InvariantCulture));

        public static Altitude Parse(String Altitude, IFormatProvider FormatProvider)
            => Parse(Double.Parse(Altitude, FormatProvider));

        public static Altitude Parse(String Altitude, NumberStyles NumberStyle)
            => Parse(Double.Parse(Altitude, NumberStyle));

        public static Altitude Parse(String Altitude, NumberStyles NumberStyle, IFormatProvider FormatProvider)
            => Parse(Double.Parse(Altitude, NumberStyle, FormatProvider));


        #region TryParse (Text)

        /// <summary>
        /// Try to parse the given string as a geographical altitude.
        /// </summary>
        /// <param name="Text">The text representation of a altitude to parse.</param>
        public static Altitude? TryParse(String Text)
        {

            if (TryParse(Text, out var altitude))
                return altitude;

            return null;

        }

        #endregion

        #region TryParse (Text, out Altitude)

        /// <summary>
        /// Try to parse the given string as a geographical altitude.
        /// </summary>
        /// <param name="Text">The text representation of a altitude to parse.</param>
        /// <param name="Altitude">The parsed altitude.</param>
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

        #endregion


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
        public static Boolean operator == (Altitude Altitude1,
                                           Altitude Altitude2)

            => Altitude1.Equals(Altitude2);

        #endregion

        #region Operator != (Altitude1, Altitude2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Altitude Altitude1,
                                           Altitude Altitude2)

            => !Altitude1.Equals(Altitude2);

        #endregion

        #region Operator <  (Altitude1, Altitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  < (Altitude Altitude1,
                                           Altitude Altitude2)

            => Altitude1.CompareTo(Altitude2) < 0;

        #endregion

        #region Operator <= (Altitude1, Altitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Altitude Altitude1,
                                           Altitude Altitude2)

            => Altitude1.CompareTo(Altitude2) <= 0;

        #endregion

        #region Operator >  (Altitude1, Altitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  > (Altitude Altitude1,
                                           Altitude Altitude2)

            => Altitude1.CompareTo(Altitude2) > 0;

        #endregion

        #region Operator >= (Altitude1, Altitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Altitude1">A altitude.</param>
        /// <param name="Altitude2">Another altitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Altitude Altitude1,
                                           Altitude Altitude2)

            => Altitude1.CompareTo(Altitude2) >= 0;

        #endregion

        #endregion

        #region IComparable Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two geographical altitudes.
        /// </summary>
        /// <param name="Object">Another geographical altitude.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Altitude altitude
                   ? CompareTo(altitude)
                   : throw new ArgumentException("The given object is not a geographical altitude!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Altitude)

        /// <summary>
        /// Compares two geographical altitudes.
        /// </summary>
        /// <param name="Altitude">Another geographical altitude.</param>
        public Int32 CompareTo(Altitude Altitude)
        {
            return Value.CompareTo(Altitude.Value);
        }

        #endregion

        #endregion

        #region IEquatable Members

        #region Equals(Object)

        /// <summary>
        /// Compares two geographical altitudes for equality.
        /// </summary>
        /// <param name="Object">Another geographical altitude.</param>
        public override Boolean Equals(Object? Object)

            => Object is Altitude altitude &&
                   Equals(altitude);

        #endregion

        #region Equals(Altitude)

        /// <summary>
        /// Compares two geographical altitudes for equality.
        /// </summary>
        /// <param name="Altitude">Another geographical altitude.</param>
        public Boolean Equals(Altitude Altitude)

            => Value.Equals(Altitude.Value);

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

            => Value.ToString();

        #endregion

    }

}
