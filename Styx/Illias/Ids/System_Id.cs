/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for system identifications.
    /// </summary>
    public static class SystemIdExtensions
    {

        /// <summary>
        /// Indicates whether this system identification is null or empty.
        /// </summary>
        /// <param name="SystemId">A system identification.</param>
        public static Boolean IsNullOrEmpty(this System_Id? SystemId)
            => !SystemId.HasValue || SystemId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this system identification is NOT null or empty.
        /// </summary>
        /// <param name="SystemId">A system identification.</param>
        public static Boolean IsNotNullOrEmpty(this System_Id? SystemId)
            => SystemId.HasValue && SystemId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A system identification is unique identification of a single system
    /// within a larger distributed system.
    /// </summary>
    public readonly struct System_Id : IId<System_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the system identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new system identification based on the given string.
        /// </summary>
        private System_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(Length)

        /// <summary>
        /// Create a new system identification.
        /// </summary>
        /// <param name="Length">The expected length of the system identification.</param>
        public static System_Id NewRandom(Byte Length = 15)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a system identification.
        /// </summary>
        /// <param name="Text">A text representation of a system identification.</param>
        public static System_Id Parse(String Text)
        {

            if (TryParse(Text, out var systemId))
                return systemId;

            throw new ArgumentException($"Invalid text representation of a system identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a system identification.
        /// </summary>
        /// <param name="Text">A text representation of a system identification.</param>
        public static System_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var systemId))
                return systemId;

            return null;

        }

        #endregion

        #region TryParse(Text, out SystemId)

        /// <summary>
        /// Try to parse the given string as a system identification.
        /// </summary>
        /// <param name="Text">A text representation of a system identification.</param>
        /// <param name="SystemId">The parsed system identification.</param>
        public static Boolean TryParse(String Text, out System_Id SystemId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    SystemId = new System_Id(Text);
                    return true;
                }
                catch
                { }
            }

            SystemId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this system identification.
        /// </summary>
        public System_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        public static System_Id  Local     { get; }
            = Parse("local");

        public static System_Id  Remote    { get; }
            = Parse("remote");


        #region Operator overloading

        #region Operator == (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (System_Id SystemId1,
                                           System_Id SystemId2)

            => SystemId1.Equals(SystemId2);

        #endregion

        #region Operator != (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (System_Id SystemId1,
                                           System_Id SystemId2)

            => !SystemId1.Equals(SystemId2);

        #endregion

        #region Operator <  (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (System_Id SystemId1,
                                          System_Id SystemId2)

            => SystemId1.CompareTo(SystemId2) < 0;

        #endregion

        #region Operator <= (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (System_Id SystemId1,
                                           System_Id SystemId2)

            => SystemId1.CompareTo(SystemId2) <= 0;

        #endregion

        #region Operator >  (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (System_Id SystemId1,
                                          System_Id SystemId2)

            => SystemId1.CompareTo(SystemId2) > 0;

        #endregion

        #region Operator >= (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (System_Id SystemId1,
                                           System_Id SystemId2)

            => SystemId1.CompareTo(SystemId2) >= 0;

        #endregion

        #endregion

        #region IComparable<SystemId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two system identifications.
        /// </summary>
        /// <param name="Object">A system identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is System_Id systemId
                   ? CompareTo(systemId)
                   : throw new ArgumentException("The given object is not a system identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SystemId)

        /// <summary>
        /// Compares two system identifications.
        /// </summary>
        /// <param name="SystemId">A system identification to compare with.</param>
        public Int32 CompareTo(System_Id SystemId)

            => String.Compare(InternalId,
                              SystemId.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<SystemId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two system identifications for equality.
        /// </summary>
        /// <param name="Object">A system identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is System_Id systemId &&
                   Equals(systemId);

        #endregion

        #region Equals(SystemId)

        /// <summary>
        /// Compares two system identifications for equality.
        /// </summary>
        /// <param name="SystemId">A system identification to compare with.</param>
        public Boolean Equals(System_Id SystemId)

            => String.Equals(InternalId,
                             SystemId.InternalId,
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
