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
    /// A system identification is unique identification of a single system
    /// within a larger distributed system.
    /// </summary>
    public struct System_Id : IId,
                              IEquatable<System_Id>,
                              IComparable<System_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// The length of the system identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Generates a system identification based on the content of String.
        /// </summary>
        /// <param name="Text">A unique system identification.</param>
        private System_Id(String Text)
        {
            InternalId = Text.Trim();
        }

        #endregion


        #region Random

        /// <summary>
        /// Generate a new random system identification.
        /// </summary>
        public static System_Id Random

            => new System_Id(Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse(Text)

        /// <summary>
        /// Parse the given string as a system identification.
        /// </summary>
        /// <param name="Text">A text representation of a system identification.</param>
        public static System_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a system identification must not be null or empty!");

            #endregion

            return new System_Id(Text);

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a system identification.
        /// </summary>
        /// <param name="Text">A text representation of a system identification.</param>
        public static System_Id? TryParse(String Text)
        {

            if (TryParse(Text, out System_Id _SystemId))
                return _SystemId;

            return new System_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out SystemId)

        /// <summary>
        /// Try to parse the given string as a system identification.
        /// </summary>
        /// <param name="Text">A text representation of a system identification.</param>
        /// <param name="SystemId">The parsed system identification.</param>
        public static Boolean TryParse(String Text, out System_Id SystemId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a system identification must not be null or empty!");

            #endregion

            try
            {
                SystemId = new System_Id(Text);
                return true;
            }
            catch (Exception)
            {
                SystemId = default(System_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone a system identification.
        /// </summary>

        public System_Id Clone
            => new System_Id(new String(InternalId.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (System_Id SystemId1, System_Id SystemId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(SystemId1, SystemId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) SystemId1 == null) || ((Object) SystemId2 == null))
                return false;

            return SystemId1.Equals(SystemId2);

        }

        #endregion

        #region Operator != (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (System_Id SystemId1, System_Id SystemId2)
            => !(SystemId1 == SystemId2);

        #endregion

        #region Operator <  (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (System_Id SystemId1, System_Id SystemId2)
        {

            if ((Object) SystemId1 == null)
                throw new ArgumentNullException(nameof(SystemId1), "The given SystemId1 must not be null!");

            return SystemId1.CompareTo(SystemId2) < 0;

        }

        #endregion

        #region Operator <= (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (System_Id SystemId1, System_Id SystemId2)
            => !(SystemId1 > SystemId2);

        #endregion

        #region Operator >  (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (System_Id SystemId1, System_Id SystemId2)
        {

            if ((Object) SystemId1 == null)
                throw new ArgumentNullException(nameof(SystemId1), "The given SystemId1 must not be null!");

            return SystemId1.CompareTo(SystemId2) > 0;

        }

        #endregion

        #region Operator >= (SystemId1, SystemId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId1">A system identification.</param>
        /// <param name="SystemId2">Another system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (System_Id SystemId1, System_Id SystemId2)
            => !(SystemId1 < SystemId2);

        #endregion

        #endregion

        #region IComparable<SystemId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is System_Id))
                throw new ArgumentException("The given object is not a system identification!",
                                            nameof(Object));

            return CompareTo((System_Id) Object);

        }

        #endregion

        #region CompareTo(SystemId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SystemId">An object to compare with.</param>
        public Int32 CompareTo(System_Id SystemId)
        {

            if ((Object) SystemId == null)
                throw new ArgumentNullException(nameof(SystemId),  "The given system identification must not be null!");

            return String.Compare(InternalId, SystemId.InternalId, StringComparison.Ordinal);

        }

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
        {

            if (Object == null)
                return false;

            if (!(Object is System_Id))
                return false;

            return Equals((System_Id) Object);

        }

        #endregion

        #region Equals(SystemId)

        /// <summary>
        /// Compares two system identifications for equality.
        /// </summary>
        /// <param name="SystemId">An system identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(System_Id SystemId)
        {

            if ((Object) SystemId == null)
                return false;

            return InternalId.Equals(SystemId.InternalId);

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

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
