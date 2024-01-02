/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for user identifications.
    /// </summary>
    public static class UserIdExtensions
    {

        /// <summary>
        /// Indicates whether this user identification is null or empty.
        /// </summary>
        /// <param name="UserId">An user identification.</param>
        public static Boolean IsNullOrEmpty(this User_Id? UserId)
            => !UserId.HasValue || UserId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this user identification is null or empty.
        /// </summary>
        /// <param name="UserId">An user identification.</param>
        public static Boolean IsNotNullOrEmpty(this User_Id? UserId)
            => UserId.HasValue && UserId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an user.
    /// </summary>
    public readonly struct User_Id : IId,
                                     IEquatable<User_Id>,
                                     IComparable<User_Id>
    {

        #region Data

        /// <summary>
        /// The internal user identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this user identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this user identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the user identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user identification based on the given string.
        /// </summary>
        private User_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Random  (Length = 15)

        /// <summary>
        /// Create a new random user identification.
        /// </summary>
        /// <param name="Length">The expected length of the random user identification.</param>
        public static User_Id Random(Byte Length = 15)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region (static) Anonymous

        /// <summary>
        /// An anonymous user identification.
        /// </summary>
        public static User_Id Anonymous

            => new ("anonymous");

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an user identification.
        /// </summary>
        /// <param name="Text">A text representation of an user identification.</param>
        public static User_Id Parse(String Text)
        {

            if (TryParse(Text, out var userId))
                return userId;

            throw new ArgumentException($"Invalid text representation of an user identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as an user identification.
        /// </summary>
        /// <param name="Text">A text representation of an user identification.</param>
        public static User_Id? TryParse(String? Text)
        {

            if (Text is not null && TryParse(Text, out var userId))
                return userId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out UserId)

        /// <summary>
        /// Try to parse the given string as an user identification.
        /// </summary>
        /// <param name="Text">A text representation of an user identification.</param>
        /// <param name="UserId">The parsed user identification.</param>
        public static Boolean TryParse(String Text, out User_Id UserId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    UserId = new User_Id(Text);
                    return true;
                }
                catch
                { }
            }

            UserId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this user identification.
        /// </summary>
        public User_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">An user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (User_Id UserId1,
                                           User_Id UserId2)

            => UserId1.Equals(UserId2);

        #endregion

        #region Operator != (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">An user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (User_Id UserId1,
                                           User_Id UserId2)

            => !UserId1.Equals(UserId2);

        #endregion

        #region Operator <  (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">An user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (User_Id UserId1,
                                          User_Id UserId2)

            => UserId1.CompareTo(UserId2) < 0;

        #endregion

        #region Operator <= (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">An user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (User_Id UserId1,
                                           User_Id UserId2)

            => UserId1.CompareTo(UserId2) <= 0;

        #endregion

        #region Operator >  (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">An user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (User_Id UserId1,
                                          User_Id UserId2)

            => UserId1.CompareTo(UserId2) > 0;

        #endregion

        #region Operator >= (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">An user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (User_Id UserId1,
                                           User_Id UserId2)

            => UserId1.CompareTo(UserId2) >= 0;

        #endregion

        #endregion

        #region IComparable<User_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is User_Id userId
                   ? CompareTo(userId)
                   : throw new ArgumentException("The given object is not an user identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(UserId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId">An object to compare with.</param>
        public Int32 CompareTo(User_Id UserId)

            => String.Compare(InternalId,
                              UserId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<User_Id> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is User_Id userId &&
                   Equals(userId);

        #endregion

        #region Equals(UserId)

        /// <summary>
        /// Compares two user identifications for equality.
        /// </summary>
        /// <param name="UserId">An user identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(User_Id UserId)

            => String.Equals(InternalId,
                             UserId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

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
