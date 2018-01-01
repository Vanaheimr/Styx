/*
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
    /// A kilo watt.
    /// </summary>
    public struct kW : IEquatable <kW>,
                       IComparable<kW>
    {

        #region Data

        /// <summary>
        /// The value of the kW in kilo watts.
        /// </summary>
        public Single  Value   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new kW.
        /// </summary>
        private kW(Single Value)
        {
            this.Value = Value;
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a kW.
        /// </summary>
        /// <param name="Text">A text representation of a kW.</param>
        public static kW Parse(String Text)

            => new kW(Single.Parse(Text));

        #endregion

        #region Parse(Number)

        /// <summary>
        /// Parse the given number as a kW.
        /// </summary>
        /// <param name="Number">A numeric representation of a kW.</param>
        public static kW Parse(Single Number)

            => new kW(Number);

        #endregion

        #region TryParse(Text,   out kW)

        /// <summary>
        /// Parse the given string as a kW.
        /// </summary>
        /// <param name="Text">A text representation of a kW.</param>
        /// <param name="kW">The parsed kW.</param>
        public static Boolean TryParse(String Text, out kW kW)
        {
            try
            {

                kW = new kW(Single.Parse(Text));

                return true;

            }
            catch (Exception)
            {
                kW = default(kW);
                return false;
            }
        }

        #endregion

        #region TryParse(Number, out kW)

        /// <summary>
        /// Parse the given number as a kW.
        /// </summary>
        /// <param name="Number">A numeric representation of a kW.</param>
        /// <param name="kW">The parsed kW.</param>
        public static Boolean TryParse(Single Number, out kW kW)
        {
            try
            {

                kW = new kW(Number);

                return true;

            }
            catch (Exception)
            {
                kW = default(kW);
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this kW.
        /// </summary>
        public kW Clone
            => new kW(Value);

        #endregion


        #region Operator overloading

        #region Operator == (kW1, kW2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kW1">A kW.</param>
        /// <param name="kW2">Another kW.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (kW kW1, kW kW2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(kW1, kW2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) kW1 == null) || ((Object) kW2 == null))
                return false;

            return kW1.Equals(kW2);

        }

        #endregion

        #region Operator != (kW1, kW2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kW1">A kW.</param>
        /// <param name="kW2">Another kW.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (kW kW1, kW kW2)
            => !(kW1 == kW2);

        #endregion

        #region Operator <  (kW1, kW2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kW1">A kW.</param>
        /// <param name="kW2">Another kW.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (kW kW1, kW kW2)
        {

            if ((Object) kW1 == null)
                throw new ArgumentNullException(nameof(kW1), "The given kW1 must not be null!");

            return kW1.CompareTo(kW2) < 0;

        }

        #endregion

        #region Operator <= (kW1, kW2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kW1">A kW.</param>
        /// <param name="kW2">Another kW.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (kW kW1, kW kW2)
            => !(kW1 > kW2);

        #endregion

        #region Operator >  (kW1, kW2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kW1">A kW.</param>
        /// <param name="kW2">Another kW.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (kW kW1, kW kW2)
        {

            if ((Object) kW1 == null)
                throw new ArgumentNullException(nameof(kW1), "The given kW1 must not be null!");

            return kW1.CompareTo(kW2) > 0;

        }

        #endregion

        #region Operator >= (kW1, kW2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kW1">A kW.</param>
        /// <param name="kW2">Another kW.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (kW kW1, kW kW2)
            => !(kW1 < kW2);

        #endregion

        #region Operator +  (kW1, kW2)

        /// <summary>
        /// Accumulates two kWs.
        /// </summary>
        /// <param name="kW1">A kW.</param>
        /// <param name="kW2">Another kW.</param>
        public static kW operator +  (kW kW1, kW kW2)
            => kW.Parse(kW1.Value + kW2.Value);

        #endregion

        #region Operator -  (kW1, kW2)

        /// <summary>
        /// Substracts two kWs.
        /// </summary>
        /// <param name="kW1">A kW.</param>
        /// <param name="kW2">Another kW.</param>
        public static kW operator -  (kW kW1, kW kW2)
            => kW.Parse(kW1.Value - kW2.Value);

        #endregion

        #endregion

        #region IComparable<kW> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is kW))
                throw new ArgumentException("The given object is not a kW!",
                                            nameof(Object));

            return CompareTo((kW) Object);

        }

        #endregion

        #region CompareTo(kW)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kW">An object to compare with.</param>
        public Int32 CompareTo(kW kW)
        {

            if ((Object) kW == null)
                throw new ArgumentNullException(nameof(kW),  "The given kW must not be null!");

            return Value.CompareTo(kW.Value);

        }

        #endregion

        #endregion

        #region IEquatable<kW> Members

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

            if (!(Object is kW))
                return false;

            return Equals((kW) Object);

        }

        #endregion

        #region Equals(kW)

        /// <summary>
        /// Compares two kWs for equality.
        /// </summary>
        /// <param name="kW">A kW to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(kW kW)
        {

            if ((Object) kW == null)
                return false;

            return Value.Equals(kW.Value);

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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => Value.ToString();

        #endregion

    }

}