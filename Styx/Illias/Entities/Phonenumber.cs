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
    /// A phone number.
    /// </summary>
    public struct Phonenumber : IId,
                                IEquatable<Phonenumber>,
                                IComparable<Phonenumber>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// The length of the phone number.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new phone number based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the phone number.</param>
        private Phonenumber(String String)
        {
            InternalId = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a phone number.
        /// </summary>
        /// <param name="Text">A text representation of a phone number.</param>
        public static Phonenumber Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a phone number must not be null or empty!");

            #endregion

            return new Phonenumber(Text);

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a phone number.
        /// </summary>
        /// <param name="Text">A text representation of a phone number.</param>
        public static Phonenumber? TryParse(String Text)
        {

            if (TryParse(Text, out Phonenumber _PhoneNumber))
                return _PhoneNumber;

            return new Phonenumber?();

        }

        #endregion

        #region (static) TryParse(Text, out PhoneNumber)

        /// <summary>
        /// Try to parse the given string as a phone number.
        /// </summary>
        /// <param name="Text">A text representation of a phone number.</param>
        /// <param name="PhoneNumber">The parsed phone number.</param>
        public static Boolean TryParse(String Text, out Phonenumber PhoneNumber)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a phone number must not be null or empty!");

            #endregion

            try
            {
                PhoneNumber = new Phonenumber(Text);
                return true;
            }
            catch (Exception)
            {
                PhoneNumber = default(Phonenumber);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone a phone number.
        /// </summary>

        public Phonenumber Clone
            => new Phonenumber(new String(InternalId.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Phonenumber PhoneNumber1, Phonenumber PhoneNumber2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(PhoneNumber1, PhoneNumber2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) PhoneNumber1 == null) || ((Object) PhoneNumber2 == null))
                return false;

            return PhoneNumber1.Equals(PhoneNumber2);

        }

        #endregion

        #region Operator != (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Phonenumber PhoneNumber1, Phonenumber PhoneNumber2)
            => !(PhoneNumber1 == PhoneNumber2);

        #endregion

        #region Operator <  (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Phonenumber PhoneNumber1, Phonenumber PhoneNumber2)
        {

            if ((Object) PhoneNumber1 == null)
                throw new ArgumentNullException(nameof(PhoneNumber1), "The given PhoneNumber1 must not be null!");

            return PhoneNumber1.CompareTo(PhoneNumber2) < 0;

        }

        #endregion

        #region Operator <= (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Phonenumber PhoneNumber1, Phonenumber PhoneNumber2)
            => !(PhoneNumber1 > PhoneNumber2);

        #endregion

        #region Operator >  (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Phonenumber PhoneNumber1, Phonenumber PhoneNumber2)
        {

            if ((Object) PhoneNumber1 == null)
                throw new ArgumentNullException(nameof(PhoneNumber1), "The given PhoneNumber1 must not be null!");

            return PhoneNumber1.CompareTo(PhoneNumber2) > 0;

        }

        #endregion

        #region Operator >= (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Phonenumber PhoneNumber1, Phonenumber PhoneNumber2)
            => !(PhoneNumber1 < PhoneNumber2);

        #endregion

        #endregion

        #region IComparable<PhoneNumber> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is Phonenumber))
                throw new ArgumentException("The given object is not a phone number!",
                                            nameof(Object));

            return CompareTo((Phonenumber) Object);

        }

        #endregion

        #region CompareTo(PhoneNumber)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber">An object to compare with.</param>
        public Int32 CompareTo(Phonenumber PhoneNumber)
        {

            if ((Object) PhoneNumber == null)
                throw new ArgumentNullException(nameof(PhoneNumber),  "The given phone number must not be null!");

            return String.Compare(InternalId, PhoneNumber.InternalId, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<PhoneNumber> Members

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

            if (!(Object is Phonenumber))
                return false;

            return Equals((Phonenumber) Object);

        }

        #endregion

        #region Equals(PhoneNumber)

        /// <summary>
        /// Compares two phone numbers for equality.
        /// </summary>
        /// <param name="PhoneNumber">An phone number to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Phonenumber PhoneNumber)
        {

            if ((Object) PhoneNumber == null)
                return false;

            return InternalId.Equals(PhoneNumber.InternalId);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => InternalId.GetHashCode();

        #endregion

        #region ToString()

        /// <summary>
        /// Return a string represtentation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
