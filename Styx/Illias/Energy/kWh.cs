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
    /// A kilo watt hour.
    /// </summary>
    public struct kWh : IEquatable <kWh>,
                        IComparable<kWh>
    {

        #region Data

        /// <summary>
        /// The value of the kWh in kilo watts.
        /// </summary>
        public Single  Value   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new kWh.
        /// </summary>
        private kWh(Single Value)
        {
            this.Value = Value;
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a kWh.
        /// </summary>
        /// <param name="Text">A text representation of a kWh.</param>
        public static kWh Parse(String Text)

            => new kWh(Single.Parse(Text));

        #endregion

        #region Parse(Number)

        /// <summary>
        /// Parse the given number as a kWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a kWh.</param>
        public static kWh Parse(Single Number)

            => new kWh(Number);

        #endregion

        #region TryParse(Text,   out kWh)

        /// <summary>
        /// Parse the given string as a kWh.
        /// </summary>
        /// <param name="Text">A text representation of a kWh.</param>
        /// <param name="kWh">The parsed kWh.</param>
        public static Boolean TryParse(String Text, out kWh kWh)
        {
            try
            {

                kWh = new kWh(Single.Parse(Text));

                return true;

            }
            catch (Exception)
            {
                kWh = default(kWh);
                return false;
            }
        }

        #endregion

        #region TryParse(Number, out kWh)

        /// <summary>
        /// Parse the given number as a kWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a kWh.</param>
        /// <param name="kWh">The parsed kWh.</param>
        public static Boolean TryParse(Single Number, out kWh kWh)
        {
            try
            {

                kWh = new kWh(Number);

                return true;

            }
            catch (Exception)
            {
                kWh = default(kWh);
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this kWh.
        /// </summary>
        public kWh Clone
            => new kWh(Value);

        #endregion


        #region Operator overloading

        #region Operator == (kWh1, kWh2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kWh1">A kWh.</param>
        /// <param name="kWh2">Another kWh.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (kWh kWh1, kWh kWh2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(kWh1, kWh2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) kWh1 == null) || ((Object) kWh2 == null))
                return false;

            return kWh1.Equals(kWh2);

        }

        #endregion

        #region Operator != (kWh1, kWh2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kWh1">A kWh.</param>
        /// <param name="kWh2">Another kWh.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (kWh kWh1, kWh kWh2)
            => !(kWh1 == kWh2);

        #endregion

        #region Operator <  (kWh1, kWh2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kWh1">A kWh.</param>
        /// <param name="kWh2">Another kWh.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (kWh kWh1, kWh kWh2)
        {

            if ((Object) kWh1 == null)
                throw new ArgumentNullException(nameof(kWh1), "The given kWh1 must not be null!");

            return kWh1.CompareTo(kWh2) < 0;

        }

        #endregion

        #region Operator <= (kWh1, kWh2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kWh1">A kWh.</param>
        /// <param name="kWh2">Another kWh.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (kWh kWh1, kWh kWh2)
            => !(kWh1 > kWh2);

        #endregion

        #region Operator >  (kWh1, kWh2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kWh1">A kWh.</param>
        /// <param name="kWh2">Another kWh.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (kWh kWh1, kWh kWh2)
        {

            if ((Object) kWh1 == null)
                throw new ArgumentNullException(nameof(kWh1), "The given kWh1 must not be null!");

            return kWh1.CompareTo(kWh2) > 0;

        }

        #endregion

        #region Operator >= (kWh1, kWh2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kWh1">A kWh.</param>
        /// <param name="kWh2">Another kWh.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (kWh kWh1, kWh kWh2)
            => !(kWh1 < kWh2);

        #endregion

        #region Operator +  (kWh1, kWh2)

        /// <summary>
        /// Accumulates two kWhs.
        /// </summary>
        /// <param name="kWh1">A kWh.</param>
        /// <param name="kWh2">Another kWh.</param>
        public static kWh operator +  (kWh kWh1, kWh kWh2)
            => kWh.Parse(kWh1.Value + kWh2.Value);

        #endregion

        #region Operator -  (kWh1, kWh2)

        /// <summary>
        /// Substracts two kWhs.
        /// </summary>
        /// <param name="kWh1">A kWh.</param>
        /// <param name="kWh2">Another kWh.</param>
        public static kWh operator -  (kWh kWh1, kWh kWh2)
            => kWh.Parse(kWh1.Value - kWh2.Value);

        #endregion

        #endregion

        #region IComparable<kWh> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is kWh))
                throw new ArgumentException("The given object is not a kWh!",
                                            nameof(Object));

            return CompareTo((kWh) Object);

        }

        #endregion

        #region CompareTo(kWh)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="kWh">An object to compare with.</param>
        public Int32 CompareTo(kWh kWh)
        {

            if ((Object) kWh == null)
                throw new ArgumentNullException(nameof(kWh),  "The given kWh must not be null!");

            return Value.CompareTo(kWh.Value);

        }

        #endregion

        #endregion

        #region IEquatable<kWh> Members

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

            if (!(Object is kWh))
                return false;

            return Equals((kWh) Object);

        }

        #endregion

        #region Equals(kWh)

        /// <summary>
        /// Compares two kWhs for equality.
        /// </summary>
        /// <param name="kWh">A kWh to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(kWh kWh)
        {

            if ((Object) kWh == null)
                return false;

            return Value.Equals(kWh.Value);

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
