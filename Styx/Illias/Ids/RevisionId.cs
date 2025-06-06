﻿/*
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

#region Usings

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A RevId is an identifier for a specific IElement revision in
    /// a distributed system consisting of a timestamp and a SystemId.
    /// </summary>
    public class RevisionId : IComparable, IComparable<RevisionId>, IEquatable<RevisionId>
    {

        #region Properties

        /// <summary>
        /// The timestamp of this revision.
        /// </summary>
        public UInt64     Timestamp    { get; private set; }

        /// <summary>
        /// A unique identification of the generating system,
        /// process or thread of this revision.
        /// </summary>
        public System_Id  SystemId     { get; private set; }

        #endregion

        #region Constructors

        #region RevisionId(SystemId)

        /// <summary>
        /// Generates a RevisionId based on the actual timestamp and the given SystemId.
        /// </summary>
        /// <param name="SystemId">An unique identifier for the generating system, process or thread</param>
        public RevisionId(System_Id SystemId)
        {
            this.Timestamp = (UInt64) UniqueTimestamp.Ticks;
            this.SystemId  = SystemId;
        }

        #endregion

        #region RevisionId(Timestamp, SystemId)

        /// <summary>
        /// Generates a RevisionId based on the given UInt64 timestamp and the given SystemId.
        /// </summary>
        /// <param name="Timestamp">A timestamp</param>
        /// <param name="SystemId">An unique identifier for the generating system, process or thread</param>
        public RevisionId(UInt64 Timestamp, System_Id SystemId)
        {
            this.Timestamp = Timestamp;
            this.SystemId  = SystemId;
        }

        #endregion

        #region RevisionId(DateTime, SystemId)

        /// <summary>
        /// Generates a RevisionId based on the given DateTime object and the given SystemId.
        /// </summary>
        /// <param name="DateTime">A DateTime object</param>
        /// <param name="SystemId">An unique identifier for the generating system, process or thread</param>
        public RevisionId(DateTime DateTime, System_Id SystemId)
        {
            this.Timestamp = (UInt64) DateTime.Ticks;
            this.SystemId  = SystemId;
        }

        #endregion

        #region RevisionId(DateTimeString, SystemId)

        /// <summary>
        /// Generates a RevisionId based on the "yyyyddMM.HHmmss.fffffff" formatted
        /// string representation of a DateTime object and the given SystemId.
        /// </summary>
        /// <param name="DateTimeString">A DateTime object as "yyyyddMM.HHmmss.fffffff"-formatted string</param>
        /// <param name="SystemId">An unique identifier for the generating system, process or thread</param>
        /// <exception cref="System.FormatException"></exception>
        public RevisionId(String DateTimeString, System_Id SystemId)
        {
            try
            {
                this.Timestamp = (UInt64) DateTime.ParseExact(DateTimeString, "yyyyddMM.HHmmss.fffffff", null).Ticks;
                this.SystemId  = SystemId;
            }
            catch
            {
                throw new FormatException("The given string could not be parsed!");
            }
        }

        #endregion

        #region RevisionId(RevIdString)

        /// <summary>
        /// Generates a RevisionId based on the "yyyyddMM.HHmmss.fffffff(SystemId)"
        /// formatted string representation of a RevId.
        /// </summary>
        /// <param name="RevIdString">A RevId object as "yyyyddMM.HHmmss.fffffff(SystemId)"-formatted string</param>
        public RevisionId(String RevIdString)
        {

            try
            {

                var __Timestamp = RevIdString.Remove(RevIdString.IndexOf("("));
                var __SystemId  = RevIdString.Substring(__Timestamp.Length + 1, RevIdString.Length - __Timestamp.Length - 2);

                this.Timestamp  = (UInt64) (DateTime.ParseExact(__Timestamp, "yyyyddMM.HHmmss.fffffff", null)).Ticks;
                this.SystemId   = System_Id.Parse(__SystemId);

            }
            catch
            {
                throw new FormatException("The given string could not be parsed!");
            }

        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (RevId1, RevId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RevId1">A RevId.</param>
        /// <param name="RevId2">Another RevId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RevisionId RevId1, RevisionId RevId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(RevId1, RevId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) RevId1 == null) || ((Object) RevId2 == null))
                return false;

            return RevId1.Equals(RevId2);

        }

        #endregion

        #region Operator != (RevId1, RevId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RevId1">A RevId.</param>
        /// <param name="RevId2">Another RevId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RevisionId RevId1, RevisionId RevId2)
        {
            return !(RevId1 == RevId2);
        }

        #endregion

        #region Operator <  (RevId1, RevId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RevId1">A RevId.</param>
        /// <param name="RevId2">Another RevId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RevisionId RevId1, RevisionId RevId2)
        {

            if (RevId1.Timestamp < RevId2.Timestamp)
                return true;

            if (RevId1.Timestamp > RevId2.Timestamp)
                return false;

            // RevId1.Timestamp == RevId2.Timestamp
            if (RevId1.SystemId < RevId2.SystemId)
                return true;

            return false;

        }

        #endregion

        #region Operator <= (RevId1, RevId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RevId1">A RevId.</param>
        /// <param name="RevId2">Another RevId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RevisionId RevId1, RevisionId RevId2)
        {
            return !(RevId1 > RevId2);
        }

        #endregion

        #region Operator >  (RevId1, RevId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RevId1">A RevId.</param>
        /// <param name="RevId2">Another RevId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RevisionId RevId1, RevisionId RevId2)
        {

            if (RevId1.Timestamp > RevId2.Timestamp)
                return true;

            if (RevId1.Timestamp < RevId2.Timestamp)
                return false;

            // RevId1.Timestamp == RevId2.Timestamp
            if (RevId1.SystemId > RevId2.SystemId)
                return true;

            return false;

        }

        #endregion

        #region Operator >= (RevId1, RevId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RevId1">A RevId.</param>
        /// <param name="RevId2">Another RevId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RevisionId RevId1, RevisionId RevId2)
        {
            return !(RevId1 < RevId2);
        }

        #endregion

        #endregion

        #region IComparable<RevisionId> Member

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is a RevId.
            var _RevId = Object as RevisionId;
            if ((Object) _RevId == null)
                throw new ArgumentException("The given object is not a RevId!");

            if (this < _RevId) return -1;
            if (this > _RevId) return +1;

            return 0;

        }

        #endregion

        #region CompareTo(RevisionId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RevisionId">A RevisionId to compare with.</param>
        public Int32 CompareTo(RevisionId RevisionId)
        {

            if ((Object) RevisionId == null)
                throw new ArgumentNullException();

            if (this < RevisionId) return -1;
            if (this > RevisionId) return +1;

            return 0;

        }

        #endregion

        #endregion

        #region IEquatable<RevId> Members

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

            // Check if the given object is a RevId.
            var _RevId = Object as RevisionId;
            if ((Object) _RevId == null)
                return false;

            return Equals(_RevId);

        }

        #endregion

        #region Equals(RevisionId)

        /// <summary>
        /// Compares two RevIds for equality.
        /// </summary>
        /// <param name="RevisionId">A RevisionId to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(RevisionId RevisionId)
        {

            if ((Object) RevisionId == null)
                return false;

            // Check if the inner fields have the same values
            if (Timestamp != RevisionId.Timestamp)
                return false;

            if (SystemId != RevisionId.SystemId)
                return false;

            return true;

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return Timestamp.GetHashCode() ^ SystemId.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a formatted string representation of this revision
        /// </summary>
        /// <returns>A formatted string representation of this revision</returns>
        public override String ToString()
        {
            return String.Format("{0:yyyyddMM.HHmmss.fffffff}({1})", new DateTime((Int64) Timestamp), SystemId.ToString());
        }

        #endregion

    }

}
