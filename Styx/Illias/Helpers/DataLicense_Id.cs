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
using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The unique identification of a data license.
    /// </summary>
    public struct DataLicense_Id : IId,
                                   IEquatable <DataLicense_Id>,
                                   IComparable<DataLicense_Id>

    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// The length of the data license.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new data license.
        /// based on the given string.
        /// </summary>
        private DataLicense_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a data license.
        /// </summary>
        /// <param name="Text">A text representation of a data license.</param>
        public static DataLicense_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a data license must not be null or empty!");

            #endregion

            return new DataLicense_Id(Text);

        }

        #endregion

        #region TryParse(Text, out DataLicenseId)

        /// <summary>
        /// Parse the given string as a data license.
        /// </summary>
        /// <param name="Text">A text representation of a data license.</param>
        /// <param name="DataLicenseId">The parsed data license.</param>
        public static Boolean TryParse(String Text, out DataLicense_Id DataLicenseId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                DataLicenseId = default(DataLicense_Id);
                return false;
            }

            #endregion

            try
            {

                DataLicenseId = new DataLicense_Id(Text);

                return true;

            }

#pragma warning disable RCS1075  // Avoid empty catch clause that catches System.Exception.
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch (Exception)
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning restore RCS1075  // Avoid empty catch clause that catches System.Exception.
            { }

            DataLicenseId = default(DataLicense_Id);
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this data license.
        /// </summary>
        public DataLicense_Id Clone

            => new DataLicense_Id(
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Provider overloading

        #region Provider == (DataLicenseId1, DataLicenseId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicenseId1">A data license.</param>
        /// <param name="DataLicenseId2">Another data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DataLicense_Id DataLicenseId1, DataLicense_Id DataLicenseId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(DataLicenseId1, DataLicenseId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) DataLicenseId1 == null) || ((Object) DataLicenseId2 == null))
                return false;

            return DataLicenseId1.Equals(DataLicenseId2);

        }

        #endregion

        #region Provider != (DataLicenseId1, DataLicenseId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicenseId1">A data license.</param>
        /// <param name="DataLicenseId2">Another data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DataLicense_Id DataLicenseId1, DataLicense_Id DataLicenseId2)
            => !(DataLicenseId1 == DataLicenseId2);

        #endregion

        #region Provider <  (DataLicenseId1, DataLicenseId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicenseId1">A data license.</param>
        /// <param name="DataLicenseId2">Another data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DataLicense_Id DataLicenseId1, DataLicense_Id DataLicenseId2)
        {

            if ((Object) DataLicenseId1 == null)
                throw new ArgumentNullException(nameof(DataLicenseId1), "The given DataLicenseId1 must not be null!");

            return DataLicenseId1.CompareTo(DataLicenseId2) < 0;

        }

        #endregion

        #region Provider <= (DataLicenseId1, DataLicenseId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicenseId1">A data license.</param>
        /// <param name="DataLicenseId2">Another data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DataLicense_Id DataLicenseId1, DataLicense_Id DataLicenseId2)
            => !(DataLicenseId1 > DataLicenseId2);

        #endregion

        #region Provider >  (DataLicenseId1, DataLicenseId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicenseId1">A data license.</param>
        /// <param name="DataLicenseId2">Another data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DataLicense_Id DataLicenseId1, DataLicense_Id DataLicenseId2)
        {

            if ((Object) DataLicenseId1 == null)
                throw new ArgumentNullException(nameof(DataLicenseId1), "The given DataLicenseId1 must not be null!");

            return DataLicenseId1.CompareTo(DataLicenseId2) > 0;

        }

        #endregion

        #region Provider >= (DataLicenseId1, DataLicenseId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicenseId1">A data license.</param>
        /// <param name="DataLicenseId2">Another data license.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DataLicense_Id DataLicenseId1, DataLicense_Id DataLicenseId2)
            => !(DataLicenseId1 < DataLicenseId2);

        #endregion

        #endregion

        #region IComparable<DataLicenseId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is DataLicense_Id))
                throw new ArgumentException("The given object is not a data license!",
                                            nameof(Object));

            return CompareTo((DataLicense_Id) Object);

        }

        #endregion

        #region CompareTo(DataLicenseId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataLicenseId">An object to compare with.</param>
        public Int32 CompareTo(DataLicense_Id DataLicenseId)
        {

            if ((Object) DataLicenseId == null)
                throw new ArgumentNullException(nameof(DataLicenseId),  "The given data license must not be null!");

            // Compare the length of the DataLicenseIds
            var _Result = this.Length.CompareTo(DataLicenseId.Length);

            if (_Result == 0)
                _Result = String.Compare(InternalId, DataLicenseId.InternalId, StringComparison.Ordinal);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<DataLicenseId> Members

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

            if (!(Object is DataLicense_Id))
                return false;

            return Equals((DataLicense_Id) Object);

        }

        #endregion

        #region Equals(DataLicenseId)

        /// <summary>
        /// Compares two DataLicenseIds for equality.
        /// </summary>
        /// <param name="DataLicenseId">A data license to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(DataLicense_Id DataLicenseId)
        {

            if ((Object) DataLicenseId == null)
                return false;

            return InternalId.Equals(DataLicenseId.InternalId);

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
