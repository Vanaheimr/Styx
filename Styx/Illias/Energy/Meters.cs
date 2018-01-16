﻿/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A meter.
    /// </summary>
    public struct Meter : IComparable<Meter>,
                          IEquatable<Meter>
    {

        #region Data

        /// <summary>
        /// The value of the meter.
        /// </summary>
        public Single  Value   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter.
        /// </summary>
        private Meter(Single Value)
        {
            this.Value = Value;
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a meter.
        /// </summary>
        /// <param name="Text">A text representation of a meter.</param>
        public static Meter Parse(String Text)

            => new Meter(Single.Parse(Text));

        #endregion

        #region Parse(Number)

        /// <summary>
        /// Parse the given number as a meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a meter.</param>
        public static Meter Parse(Single Number)

            => new Meter(Number);

        #endregion

        #region TryParse(Text,   out Meter)

        /// <summary>
        /// Parse the given string as a meter.
        /// </summary>
        /// <param name="Text">A text representation of a meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(String Text, out Meter Meter)
        {
            try
            {

                Meter = new Meter(Single.Parse(Text));

                return true;

            }
            catch (Exception)
            {
                Meter = default(Meter);
                return false;
            }
        }

        #endregion

        #region TryParse(Number, out Meter)

        /// <summary>
        /// Parse the given number as a meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(Single Number, out Meter Meter)
        {
            try
            {

                Meter = new Meter(Number);

                return true;

            }
            catch (Exception)
            {
                Meter = default(Meter);
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this Meter.
        /// </summary>
        public Meter Clone
            => new Meter(Value);

        #endregion


        #region Operator overloading

        #region Operator == (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Meter Meter1, Meter Meter2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Meter1, Meter2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Meter1 == null) || ((Object) Meter2 == null))
                return false;

            return Meter1.Equals(Meter2);

        }

        #endregion

        #region Operator != (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Meter Meter1, Meter Meter2)
            => !(Meter1 == Meter2);

        #endregion

        #region Operator <  (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Meter Meter1, Meter Meter2)
        {

            if ((Object) Meter1 == null)
                throw new ArgumentNullException(nameof(Meter1), "The given Meter1 must not be null!");

            return Meter1.CompareTo(Meter2) < 0;

        }

        #endregion

        #region Operator <= (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Meter Meter1, Meter Meter2)
            => !(Meter1 > Meter2);

        #endregion

        #region Operator >  (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Meter Meter1, Meter Meter2)
        {

            if ((Object) Meter1 == null)
                throw new ArgumentNullException(nameof(Meter1), "The given Meter1 must not be null!");

            return Meter1.CompareTo(Meter2) > 0;

        }

        #endregion

        #region Operator >= (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Meter Meter1, Meter Meter2)
            => !(Meter1 < Meter2);

        #endregion

        #region Operator +  (Meter1, Meter2)

        /// <summary>
        /// Accumulates two Meters.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        public static Meter operator +  (Meter Meter1, Meter Meter2)
            => Meter.Parse(Meter1.Value + Meter2.Value);

        #endregion

        #region Operator -  (Meter1, Meter2)

        /// <summary>
        /// Substracts two Meters.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        public static Meter operator -  (Meter Meter1, Meter Meter2)
            => Meter.Parse(Meter1.Value - Meter2.Value);

        #endregion

        #endregion

        #region IComparable<Meter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is Meter))
                throw new ArgumentException("The given object is not a meter!",
                                            nameof(Object));

            return CompareTo((Meter) Object);

        }

        #endregion

        #region CompareTo(Meter)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter">An object to compare with.</param>
        public Int32 CompareTo(Meter Meter)
        {

            if ((Object) Meter == null)
                throw new ArgumentNullException(nameof(Meter),  "The given Meter must not be null!");

            return Value.CompareTo(Meter.Value);

        }

        #endregion

        #endregion

        #region IEquatable<Meter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            if (!(Object is Meter))
                return false;

            return Equals((Meter) Object);

        }

        #endregion

        #region Equals(Meter)

        /// <summary>
        /// Compares two Meters for equality.
        /// </summary>
        /// <param name="Meter">A Meter to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Meter Meter)
        {

            if ((Object) Meter == null)
                return false;

            return Value.Equals(Meter.Value);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Value.ToString();

        #endregion

    }

}