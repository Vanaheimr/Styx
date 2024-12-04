/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for phone numbers.
    /// </summary>
    public static class PhoneNumberExtensions
    {

        /// <summary>
        /// Indicates whether this phone number is null or empty.
        /// </summary>
        /// <param name="PhoneNumber">A phone number.</param>
        public static Boolean IsNullOrEmpty(this PhoneNumber? PhoneNumber)
            => !PhoneNumber.HasValue || PhoneNumber.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this phone number is null or empty.
        /// </summary>
        /// <param name="PhoneNumber">A phone number.</param>
        public static Boolean IsNotNullOrEmpty(this PhoneNumber? PhoneNumber)
            => PhoneNumber.HasValue && PhoneNumber.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A phone number.
    /// </summary>
    public readonly struct PhoneNumber : IId,
                                         IEquatable<PhoneNumber>,
                                         IComparable<PhoneNumber>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        /// <summary>
        /// The regular expression init string for matching phone numbers.
        /// </summary>
        public static readonly Regex IsPhoneNumber_RegExprString = new Regex("\\+?[0-9\\ \\-\\/]{5,30}");

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this phone number is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this phone number is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the phone number.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new phone number based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the phone number.</param>
        private PhoneNumber(String String)
        {
            InternalId = String;
        }

        #endregion


        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a phone number.
        /// </summary>
        /// <param name="Text">A text representation of a phone number.</param>
        public static PhoneNumber Parse(String Text)
        {

            if (TryParse(Text, out var phoneNumber))
                return phoneNumber;

            throw new ArgumentException($"Invalid text representation of a phone number: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a phone number.
        /// </summary>
        /// <param name="Text">A text representation of a phone number.</param>
        public static PhoneNumber? TryParse(String Text)
        {

            if (TryParse(Text, out var phoneNumber))
                return phoneNumber;

            return null;

        }

        #endregion

        #region TryParse(Text, out PhoneNumber)

        /// <summary>
        /// Try to parse the given string as a phone number.
        /// </summary>
        /// <param name="Text">A text representation of a phone number.</param>
        /// <param name="PhoneNumber">The parsed phone number.</param>
        public static Boolean TryParse(String                               Text,
                                       [NotNullWhen(true)] out PhoneNumber  PhoneNumber)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                IsPhoneNumber_RegExprString.IsMatch(Text))
            {
                try
                {
                    PhoneNumber = new PhoneNumber(Text);
                    return true;
                }
                catch
                { }
            }

            PhoneNumber = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this phone number.
        /// </summary>
        public PhoneNumber Clone

            => new PhoneNumber(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PhoneNumber PhoneNumber1,
                                           PhoneNumber PhoneNumber2)

            => PhoneNumber1.Equals(PhoneNumber2);

        #endregion

        #region Operator != (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PhoneNumber PhoneNumber1,
                                           PhoneNumber PhoneNumber2)

            => !PhoneNumber1.Equals(PhoneNumber2);

        #endregion

        #region Operator <  (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PhoneNumber PhoneNumber1,
                                          PhoneNumber PhoneNumber2)

            => PhoneNumber1.CompareTo(PhoneNumber2) < 0;

        #endregion

        #region Operator <= (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PhoneNumber PhoneNumber1,
                                           PhoneNumber PhoneNumber2)

            => PhoneNumber1.CompareTo(PhoneNumber2) <= 0;

        #endregion

        #region Operator >  (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PhoneNumber PhoneNumber1,
                                          PhoneNumber PhoneNumber2)

            => PhoneNumber1.CompareTo(PhoneNumber2) > 0;

        #endregion

        #region Operator >= (PhoneNumber1, PhoneNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber1">A phone number.</param>
        /// <param name="PhoneNumber2">Another phone number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PhoneNumber PhoneNumber1,
                                           PhoneNumber PhoneNumber2)

            => PhoneNumber1.CompareTo(PhoneNumber2) >= 0;

        #endregion

        #endregion

        #region IComparable<PhoneNumber> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is PhoneNumber phoneNumber
                   ? CompareTo(phoneNumber)
                   : throw new ArgumentException("The given object is not a phone number!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PhoneNumber)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PhoneNumber">An object to compare with.</param>
        public Int32 CompareTo(PhoneNumber PhoneNumber)

            => String.Compare(InternalId,
                              PhoneNumber.InternalId,
                              StringComparison.Ordinal);

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

            => Object is PhoneNumber phoneNumber &&
                   Equals(phoneNumber);

        #endregion

        #region Equals(PhoneNumber)

        /// <summary>
        /// Compares two PhoneNumbers for equality.
        /// </summary>
        /// <param name="PhoneNumber">A phone number to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(PhoneNumber PhoneNumber)

            => String.Equals(InternalId,
                             PhoneNumber.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
