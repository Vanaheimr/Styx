/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
        /// <param name="SystemId">An system identification.</param>
        public static Boolean IsNullOrEmpty(this System_Id? SystemId)
            => !SystemId.HasValue || SystemId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this system identification is NOT null or empty.
        /// </summary>
        /// <param name="SystemId">An system identification.</param>
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
            => (UInt64) InternalId?.Length;

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


        #region (static) Random(Length)

        /// <summary>
        /// Create a new system identification.
        /// </summary>
        /// <param name="Length">The expected length of the system identification.</param>
        public static System_Id Random(Byte Length = 15)

            => new System_Id(RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as an system identification.
        /// </summary>
        /// <param name="Text">A text representation of an system identification.</param>
        public static System_Id Parse(String Text)
        {

            if (TryParse(Text, out System_Id systemId))
                return systemId;

            throw new ArgumentException("Invalid text representation of an system identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as an system identification.
        /// </summary>
        /// <param name="Text">A text representation of an system identification.</param>
        public static System_Id? TryParse(String Text)
        {

            if (TryParse(Text, out System_Id systemId))
                return systemId;

            return null;

        }

        #endregion

        #region TryParse(Text, out SystemId)

        /// <summary>
        /// Try to parse the given string as an system identification.
        /// </summary>
        /// <param name="Text">A text representation of an system identification.</param>
        /// <param name="SystemId">The parsed system identification.</param>
        public static Boolean TryParse(String Text, out System_Id SystemId)
        {

            Text = Text?.Trim();

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

            => new System_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (SystemIdId1, SystemIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemIdId1">An system identification.</param>
        /// <param name="SystemIdId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (System_Id SystemIdId1,
                                           System_Id SystemIdId2)

            => SystemIdId1.Equals(SystemIdId2);

        #endregion

        #region Operator != (SystemIdId1, SystemIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemIdId1">An system identification.</param>
        /// <param name="SystemIdId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (System_Id SystemIdId1,
                                           System_Id SystemIdId2)

            => !SystemIdId1.Equals(SystemIdId2);

        #endregion

        #region Operator <  (SystemIdId1, SystemIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemIdId1">An system identification.</param>
        /// <param name="SystemIdId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (System_Id SystemIdId1,
                                          System_Id SystemIdId2)

            => SystemIdId1.CompareTo(SystemIdId2) < 0;

        #endregion

        #region Operator <= (SystemIdId1, SystemIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemIdId1">An system identification.</param>
        /// <param name="SystemIdId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (System_Id SystemIdId1,
                                           System_Id SystemIdId2)

            => SystemIdId1.CompareTo(SystemIdId2) <= 0;

        #endregion

        #region Operator >  (SystemIdId1, SystemIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemIdId1">An system identification.</param>
        /// <param name="SystemIdId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (System_Id SystemIdId1,
                                          System_Id SystemIdId2)

            => SystemIdId1.CompareTo(SystemIdId2) > 0;

        #endregion

        #region Operator >= (SystemIdId1, SystemIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemIdId1">An system identification.</param>
        /// <param name="SystemIdId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (System_Id SystemIdId1,
                                           System_Id SystemIdId2)

            => SystemIdId1.CompareTo(SystemIdId2) >= 0;

        #endregion

        #endregion

        #region IComparable<SystemId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is System_Id systemId
                   ? CompareTo(systemId)
                   : throw new ArgumentException("The given object is not an system identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SystemId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId">An object to compare with.</param>
        public Int32 CompareTo(System_Id SystemId)

            => String.Compare(InternalId,
                              SystemId.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<SystemId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is System_Id systemId &&
                   Equals(systemId);

        #endregion

        #region Equals(SystemId)

        /// <summary>
        /// Compares two SystemIds for equality.
        /// </summary>
        /// <param name="SystemId">An system identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(System_Id SystemId)

            => String.Equals(InternalId,
                             SystemId.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region GetHashCode()

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
