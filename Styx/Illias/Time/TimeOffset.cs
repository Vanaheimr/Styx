/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A TimeOffset.
    /// </summary>
    public readonly struct TimeOffset : IEquatable<TimeOffset>,
                                        IComparable<TimeOffset>,
                                        IComparable
    {

        #region Properties

        /// <summary>
        /// The hour offset.
        /// </summary>
        public readonly Int16  Hour      { get; }

        /// <summary>
        /// The minute offset.
        /// </summary>
        public readonly Byte   Minute    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TimeOffset.
        /// </summary>
        /// <param name="Hour">The hour offset.</param>
        /// <param name="Minute">The minute offset.</param>
        private TimeOffset(Int16  Hour,
                           Byte   Minute)
        {
            this.Hour    = Hour;
            this.Minute  = Minute;
        }

        #endregion


        #region FromHour    (Hour)

        /// <summary>
        /// Create a new TimeOffset based on the given hour offset.
        /// </summary>
        /// <param name="Hour">The hour offset.</param>
        public static TimeOffset? FromHour(Int16  Hour)
        {

            if (TryParse(Hour, 0, out var timeOffset, out _))
                return timeOffset;

            return null;

        }

        #endregion

        #region FromHourMin (Hour, Minute)

        /// <summary>
        /// Create a new TimeOffset based on the given hour and minute offsets.
        /// </summary>
        /// <param name="Hour">The hour offset.</param>
        /// <param name="Minute">The minute offset.</param>
        public static TimeOffset? FromHourMin(Int16  Hour,
                                              Byte   Minute)
        {

            if (TryParse(Hour, Minute, out var timeOffset, out _))
                return timeOffset;

            return null;

        }

        #endregion


        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as TimeOffset.
        /// </summary>
        /// <param name="Text">A text representation of the TimeOffset.</param>
        public static TimeOffset Parse(String Text)
        {

            if (TryParse(Text, out var timeOffset, out var errorResponse))
                return timeOffset;

            throw new ArgumentException($"Invalid text representation '{Text}' of a TimeOffset: " + errorResponse,
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as TimeOffset.
        /// </summary>
        /// <param name="Text">A text representation of the TimeOffset.</param>
        public static TimeOffset? TryParse(String Text)
        {

            if (TryParse(Text, out var timeOffset, out _))
                return timeOffset;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out TimeOffset, out ErrorResponse)

        /// <summary>
        /// Try to parse the given text as TimeOffset.
        /// </summary>
        /// <param name="Text">A text representation of the TimeOffset.</param>
        /// <param name="TimeOffset">The parsed TimeOffset offset.</param>
        public static Boolean TryParse(String                               Text,
                                       [NotNullWhen(true)]  out TimeOffset  TimeOffset,
                                       [NotNullWhen(false)] out String?     ErrorResponse)
        {

            TimeOffset     = default;
            ErrorResponse  = null;

            var elements   = Text.Trim().Split(':');

            Int16 hour     = 0;
            Byte  minute   = 0;

            if (elements.Length == 1)
            {

                if (!Int16.TryParse(elements[0], out hour))
                {
                    ErrorResponse = $"The hour offset value '{elements[0]}' is not a valid number!";
                    return false;
                }

                if (hour > 23 || hour < -23)
                {
                    ErrorResponse = $"The hour offset value '{hour}' is out of range!";
                    return false;
                }

            }

            if (elements.Length >= 2)
            {

                if (!Byte.TryParse(elements[1], out minute))
                {
                    ErrorResponse = $"The minute offset value '{elements[1]}' is not a valid number!";
                    return false;
                }

                if (minute > 59)
                {
                    ErrorResponse = $"The minute offset value '{minute}' is out of range!";
                    return false;
                }

            }


            TimeOffset = new TimeOffset(hour, minute);
            return true;

        }

        #endregion


        #region (static) TryParse (Hour, Minute, out TimeOffset, out ErrorResponse)

        /// <summary>
        /// Try to parse the given text as TimeOffset.
        /// </summary>
        /// <param name="Hour">The hour offset.</param>
        /// <param name="Minute">The minute offset.</param>
        /// <param name="TimeOffset">The parsed TimeOffset offset.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(Int16                                Hour,
                                       Byte                                 Minute,
                                       [NotNullWhen(true)]  out TimeOffset  TimeOffset,
                                       [NotNullWhen(false)] out String?     ErrorResponse)
        {

            TimeOffset     = default;
            ErrorResponse  = null;

            if (Hour > 23 || Hour < -23)
            {
                ErrorResponse = $"The hour value '{Hour}' is out of range!";
                return false;
            }

            if (Minute > 59)
            {
                ErrorResponse = $"The minute value '{Minute}' is out of range!";
                return false;
            }

            TimeOffset = new TimeOffset(Hour, Minute);
            return true;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TimeOffset1, TimeOffset2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeOffset1">A TimeOffset.</param>
        /// <param name="TimeOffset2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TimeOffset TimeOffset1,
                                           TimeOffset TimeOffset2)

            => TimeOffset1.Equals(TimeOffset2);

        #endregion

        #region Operator != (TimeOffset1, TimeOffset2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeOffset1">A TimeOffset.</param>
        /// <param name="TimeOffset2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TimeOffset TimeOffset1,
                                           TimeOffset TimeOffset2)

            => !(TimeOffset1 == TimeOffset2);

        #endregion

        #region Operator <  (TimeOffset1, TimeOffset2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeOffset1">A TimeOffset.</param>
        /// <param name="TimeOffset2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TimeOffset TimeOffset1,
                                          TimeOffset TimeOffset2)

            => TimeOffset1.CompareTo(TimeOffset2) < 0;

        #endregion

        #region Operator <= (TimeOffset1, TimeOffset2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeOffset1">A TimeOffset.</param>
        /// <param name="TimeOffset2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TimeOffset TimeOffset1,
                                           TimeOffset TimeOffset2)

            => !(TimeOffset1 > TimeOffset2);

        #endregion

        #region Operator >  (TimeOffset1, TimeOffset2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeOffset1">A TimeOffset.</param>
        /// <param name="TimeOffset2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TimeOffset TimeOffset1,
                                          TimeOffset TimeOffset2)

            => TimeOffset1.CompareTo(TimeOffset2) > 0;

        #endregion

        #region Operator >= (TimeOffset1, TimeOffset2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeOffset1">A TimeOffset.</param>
        /// <param name="TimeOffset2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TimeOffset TimeOffset1,
                                           TimeOffset TimeOffset2)

            => !(TimeOffset1 < TimeOffset2);

        #endregion

        #endregion

        #region IComparable<TimeOffset> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two TimeOffsets.
        /// </summary>
        /// <param name="Object">A TimeOffset to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TimeOffset timeOffset
                   ? CompareTo(timeOffset)
                   : throw new ArgumentException("The given object is not a TimeOffset!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TimeOffset)

        /// <summary>
        /// Compares two TimeOffsets.
        /// </summary>
        /// <param name="TimeOffset">A TimeOffset to compare with.</param>
        public Int32 CompareTo(TimeOffset TimeOffset)
        {

            var c = Hour.  CompareTo(TimeOffset.Hour);

            if (c == 0)
                c = Minute.CompareTo(TimeOffset.Minute);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<TimeOffset> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TimeOffsets for equality.
        /// </summary>
        /// <param name="Object">A TimeOffset to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TimeOffset timeOffset &&
                   Equals(timeOffset);

        #endregion

        #region Equals(TimeOffset)

        /// <summary>
        /// Compares two TimeOffsets for equality.
        /// </summary>
        /// <param name="TimeOffset">A TimeOffset to compare with.</param>
        public Boolean Equals(TimeOffset TimeOffset)

            => Hour.  Equals(TimeOffset.Hour) &&
               Minute.Equals(TimeOffset.Minute);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Hour.  GetHashCode() * 3 ^
               Minute.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Hour:D2}:{Minute:D2}";

        #endregion

    }

}
