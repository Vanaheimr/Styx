/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The unique identification of a time zone.
    /// </summary>
    public readonly struct Time_Zone : IId<Time_Zone>
    {

        #region Data

        //ToDo: Implement proper time zone format!
        // https://github.com/hubject/oicp/blob/master/OICP-2.3/OICP%202.3%20CPO/03_CPO_Data_Types.asciidoc#TimeZoneType
        // [U][T][C][+,-][0-9][0-9][:][0-9][0-9]

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
        /// The length of the time zone identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new time zone identification.
        /// based on the given string.
        /// </summary>
        private Time_Zone(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a time zone identification.
        /// </summary>
        /// <param name="Text">A text-representation of a time zone identification.</param>
        public static Time_Zone Parse(String Text)
        {

            if (TryParse(Text, out Time_Zone timeZone))
                return timeZone;

            throw new ArgumentException("Invalid text-representation of a time zone identification: '" + Text + "'!", nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a time zone identification.
        /// </summary>
        /// <param name="Text">A text-representation of a time zone identification.</param>
        public static Time_Zone? TryParse(String Text)
        {

            if (TryParse(Text, out Time_Zone timeZone))
                return timeZone;

            return new Time_Zone?();

        }

        #endregion

        #region TryParse(Text, out TimeZone)

        /// <summary>
        /// Try to parse the given string as a time zone identification.
        /// </summary>
        /// <param name="Text">A text-representation of a time zone identification.</param>
        /// <param name="TimeZone">The parsed time zone identification.</param>
        public static Boolean TryParse(String Text, out Time_Zone TimeZone)
        {

            #region Initial checks

            TimeZone = default;

            Text = Text?.Trim();

            if (Text.IsNullOrEmpty())
                return false;

            #endregion

            try
            {
                TimeZone = new Time_Zone(Text);
                return true;
            }

#pragma warning disable RCS1075  // Avoid empty catch clause that catches System.Exception.
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch (Exception)
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning restore RCS1075  // Avoid empty catch clause that catches System.Exception.
            { }

            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this time zone identification.
        /// </summary>
        public Time_Zone Clone

            => new Time_Zone(
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (TimeZone1, TimeZone2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeZone1">A time zone identification.</param>
        /// <param name="TimeZone2">Another time zone identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Time_Zone TimeZone1, Time_Zone TimeZone2)
            => TimeZone1.Equals(TimeZone2);

        #endregion

        #region Operator != (TimeZone1, TimeZone2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeZone1">A time zone identification.</param>
        /// <param name="TimeZone2">Another time zone identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Time_Zone TimeZone1, Time_Zone TimeZone2)
            => !TimeZone1.Equals(TimeZone2);

        #endregion

        #region Operator <  (TimeZone1, TimeZone2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeZone1">A time zone identification.</param>
        /// <param name="TimeZone2">Another time zone identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Time_Zone TimeZone1, Time_Zone TimeZone2)
            => TimeZone1.CompareTo(TimeZone2) < 0;

        #endregion

        #region Operator <= (TimeZone1, TimeZone2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeZone1">A time zone identification.</param>
        /// <param name="TimeZone2">Another time zone identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Time_Zone TimeZone1, Time_Zone TimeZone2)
            => TimeZone1.CompareTo(TimeZone2) <= 0;

        #endregion

        #region Operator >  (TimeZone1, TimeZone2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeZone1">A time zone identification.</param>
        /// <param name="TimeZone2">Another time zone identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Time_Zone TimeZone1, Time_Zone TimeZone2)
            => TimeZone1.CompareTo(TimeZone2) > 0;

        #endregion

        #region Operator >= (TimeZone1, TimeZone2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeZone1">A time zone identification.</param>
        /// <param name="TimeZone2">Another time zone identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Time_Zone TimeZone1, Time_Zone TimeZone2)
            => TimeZone1.CompareTo(TimeZone2) >= 0;

        #endregion

        #endregion

        #region IComparable<TimeZone> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Time_Zone timeZone
                   ? CompareTo(timeZone)
                   : throw new ArgumentException("The given object is not a time zone identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TimeZone)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeZone">An object to compare with.</param>
        public Int32 CompareTo(Time_Zone TimeZone)

            => String.Compare(InternalId,
                              TimeZone.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TimeZone> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Time_Zone timeZone &&
                   Equals(timeZone);

        #endregion

        #region Equals(TimeZone)

        /// <summary>
        /// Compares two TimeZones for equality.
        /// </summary>
        /// <param name="TimeZone">A time zone identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Time_Zone TimeZone)

            => String.Equals(InternalId,
                             TimeZone.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => InternalId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text-representation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
