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

#region Usings

using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A Time.
    /// </summary>
    public readonly struct Time : IEquatable<Time>,
                                  IComparable<Time>,
                                  IComparable
    {

        #region Properties

        /// <summary>
        /// The hour.
        /// </summary>
        public readonly Int16        Hour        { get; }

        /// <summary>
        /// The minute.
        /// </summary>
        public readonly Byte         Minute      { get; }

        /// <summary>
        /// The second.
        /// </summary>
        public readonly Byte?        Second      { get; }

        /// <summary>
        /// The fractional part of the second (e.g., milliseconds or microseconds).
        /// Null if not specified. You must decide the scale (e.g. 3 digits = milliseconds).
        /// </summary>
        public readonly UInt16?      Fraction    { get; }

        /// <summary>
        /// An optional TimeOffset zone offset.
        /// For example, +02:00 or -05:30, or zero (Z). Null means no offset specified.
        /// </summary>
        public readonly TimeOffset?  Offset      { get; }

        /// <summary>
        /// Whether the TimeOffset is in Zulu TimeOffset.
        /// </summary>
        public readonly Boolean      IsZulu      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Time.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute.</param>
        /// <param name="Second">The second.</param>
        /// <param name="Offset">An optional TimeOffset zone offset.</param>
        /// <param name="IsZulu">Whether the TimeOffset is in Zulu TimeOffset.</param>
        private Time(Int16        Hour,
                     Byte         Minute     = 0,
                     Byte?        Second     = null,
                     UInt16?      Fraction   = null,
                     TimeOffset?  Offset     = null,
                     Boolean      IsZulu     = false)
        {

            this.Hour      = Hour;
            this.Minute    = Minute;
            this.Second    = Second;
            this.Fraction  = Fraction;
            this.IsZulu    = IsZulu;

            unchecked
            {

                hashCode = Hour.     GetHashCode()       * 13 ^
                           Minute.   GetHashCode()       * 11 ^
                          (Second?.  GetHashCode() ?? 0) *  7 ^
                          (Fraction?.GetHashCode() ?? 0) *  5 ^
                          (Offset?.  GetHashCode() ?? 0) *  3 ^
                           IsZulu.   GetHashCode();

            }

        }

        #endregion


        //   "HH"
        //   "HH:mm"
        //   "HH:mm:ss"
        //   "HH:mm:ss.fff"
        //   "HH:mm:ss.fffZ"
        //   "HH:mm:ssZ"
        //   "HH:mm:ss+02:00"
        //   "HH:mm:ss.fffff-05:00"


        #region FromHour      (Hour)

        /// <summary>
        /// Create a new TimeOffset based on the given hour.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        public static Time? FromHour(Int16 Hour)
        {

            if (TryParse(Hour, out var TimeOffset, out _))
                return TimeOffset;

            return null;

        }

        #endregion

        #region FromHourMin   (Hour, Minute)

        /// <summary>
        /// Create a new TimeOffset based on the given hour and minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute</param>
        public static Time? FromHourMin(Int16 Hour,
                                        Byte  Minute)
        {

            if (TryParse(Hour, out var TimeOffset, out _, Minute))
                return TimeOffset;

            return null;

        }

        #endregion

        #region FromHourMinSec(Hour, Minute, Second)

        /// <summary>
        /// Create a new TimeOffset based on the given hour and minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute</param>
        /// <param name="Second">The second.</param>
        public static Time? FromHourMinSec(Int16 Hour,
                                           Byte  Minute,
                                           Byte  Second)
        {

            if (TryParse(Hour, out var TimeOffset, out _, Minute, Second))
                return TimeOffset;

            return null;

        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as Time.
        /// </summary>
        /// <param name="Text">A text representation of the Time.</param>
        public static Time Parse(String Text)
        {

            if (TryParse(Text, out var time, out var errorResponse))
                return time;

            throw new ArgumentException($"Invalid text representation '{Text}' of a Time: " + errorResponse,
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as Time.
        /// </summary>
        /// <param name="Text">A text representation of the Time.</param>
        public static Time? TryParse(String Text)
        {

            if (TryParse(Text, out var time, out _))
                return time;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Time, out ErrorResponse)

        /// <summary>
        /// Try to parse the given text as Time.
        /// </summary>
        /// <param name="Text">A text representation of the Time.</param>
        /// <param name="Time">The parsed Time.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(String                            Text,
                                       [NotNullWhen(true)]  out Time     Time,
                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Time           = default;
            ErrorResponse  = null;
            Text           = Text.Trim();

            var isZulu              = false;

            if (Text.EndsWith('Z'))
            {
                isZulu              = true;
                Text                = Text[..^1].Trim();
            }

            //var positiveTimeOffset  = Text.Contains('+');
            //var negativeTimeOffset  = Text.Contains('-');

            //if (isZulu && (positiveTimeOffset || negativeTimeOffset))
            //{
            //    ErrorResponse = "The TimeOffset string contains both a TimeOffset zone offset and a Zulu TimeOffset!";
            //    return false;
            //}

            //if (positiveTimeOffset && negativeTimeOffset)
            //{
            //    ErrorResponse = "The TimeOffset string contains both a positive and a negative TimeOffset offset!";
            //    return false;
            //}

            var timeOffsetPosition  = Text.LastIndexOfAny(['+', '-']);

            if (timeOffsetPosition > 0 && isZulu)
            {
                ErrorResponse = "The TimeOffset string contains both a TimeOffset zone offset and a Zulu TimeOffset!";
                return false;
            }

            TimeOffset? timeOffset  = null;

            var timeOffsetText      = timeOffsetPosition >= 0
                                          ? Text[timeOffsetPosition..]
                                          : null;

            if (timeOffsetText is not null)
            {

                if (!TimeOffset.TryParse(timeOffsetText, out var _TimeOffset, out ErrorResponse))
                    return false;

                timeOffset = _TimeOffset;

            }


            var timeElements        = Text.Split([':', '.', ',']);

            Byte   hour             = 0;
            Byte   minute           = 0;
            Byte   second           = 0;
            UInt16 fraction         = 0;

            if (timeElements.Length >= 1)
            {

                if (!Byte.TryParse(timeElements[0], out hour))
                {
                    ErrorResponse = $"The hour value '{timeElements[0]}' is not a valid number!";
                    return false;
                }

                if (hour > 23)
                {
                    ErrorResponse = $"The hour value '{hour}' is out of range!";
                    return false;
                }

            }

            if (timeElements.Length >= 2)
            {

                if (!Byte.TryParse(timeElements[1], out minute))
                {
                    ErrorResponse = $"The minute value '{timeElements[1]}' is not a valid number!";
                    return false;
                }

                if (minute > 59)
                {
                    ErrorResponse = $"The minute value '{minute}' is out of range!";
                    return false;
                }

            }

            if (timeElements.Length >= 3)
            {

                if (!Byte.TryParse(timeElements[2], out second))
                {
                    ErrorResponse = $"The second value '{timeElements[2]}' is not a valid number!";
                    return false;
                }

                if (second > 59)
                {
                    ErrorResponse = $"The second value '{second}' is out of range!";
                    return false;
                }

            }

            if (timeElements.Length >= 4)
            {

                if (!UInt16.TryParse(timeElements[3], out fraction))
                {
                    ErrorResponse = $"The fraction value '{timeElements[3]}' is not a valid number!";
                    return false;
                }

                if (fraction > 999)
                {
                    ErrorResponse = $"The fraction value '{fraction}' is out of range!";
                    return false;
                }

            }


            Time = new Time(
                       hour,
                       minute,
                       second,
                       fraction,
                       timeOffset,
                       isZulu
                   );

            return true;

        }

        #endregion


        #region (static) TryParse(Hour, Minute, Second, Fraction, TimeOffset, out Time, out ErrorResponse)

        /// <summary>
        /// Try to parse the given text as TimeOffset.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute.</param>
        /// <param name="Second">The second.</param>
        /// <param name="Fraction">The fraction of a second.</param>
        /// <param name="TimeOffset">The TimeOffset offset.</param>
        /// <param name="Time">The parsed TimeOffset.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(Int16                             Hour,
                                       [NotNullWhen(true)]  out Time     Time,
                                       [NotNullWhen(false)] out String?  ErrorResponse,
                                       Byte                              Minute       = 0,
                                       Byte                              Second       = 0,
                                       UInt16                            Fraction     = 0,
                                       TimeOffset?                       TimeOffset   = null,
                                       Boolean?                          IsZulu       = null)
        {

            Time           = default;
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

            if (Second > 59)
            {
                ErrorResponse = $"The second value '{Second}' is out of range!";
                return false;
            }

            if (Fraction > 999)
            {
                ErrorResponse = $"The fraction value '{Fraction}' is out of range!";
                return false;
            }

            if (TimeOffset.HasValue && IsZulu.HasValue && IsZulu.Value != false)
            {
                ErrorResponse = "The TimeOffset offset and Zulu TimeOffset flag are mutually exclusive!";
                return false;
            }

            Time = new Time(
                       Hour,
                       Minute,
                       Second,
                       Fraction,
                       TimeOffset,
                       IsZulu ?? false
                   );

            return true;

        }

        #endregion



        #region Operator overloading

        #region Operator == (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A TimeOffset.</param>
        /// <param name="Time2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Time Time1,
                                           Time Time2)

            => Time1.Equals(Time2);

        #endregion

        #region Operator != (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A TimeOffset.</param>
        /// <param name="Time2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Time Time1,
                                           Time Time2)

            => !(Time1 == Time2);

        #endregion

        #region Operator <  (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A TimeOffset.</param>
        /// <param name="Time2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Time Time1,
                                          Time Time2)

            => Time1.CompareTo(Time2) < 0;

        #endregion

        #region Operator <= (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A TimeOffset.</param>
        /// <param name="Time2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Time Time1,
                                           Time Time2)

            => !(Time1 > Time2);

        #endregion

        #region Operator >  (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A TimeOffset.</param>
        /// <param name="Time2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Time Time1,
                                          Time Time2)

            => Time1.CompareTo(Time2) > 0;

        #endregion

        #region Operator >= (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A TimeOffset.</param>
        /// <param name="Time2">Another TimeOffset.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Time Time1,
                                           Time Time2)

            => !(Time1 < Time2);

        #endregion

        #region Operator +  (Time1, Time2)

        /// <summary>
        /// Operator +
        /// </summary>
        /// <param name="Time1">A TimeOffset.</param>
        /// <param name="Time2">Another TimeOffset.</param>
        public static TimeSpan operator +  (Time Time1,
                                            Time Time2)
        {

            var Days     = 0;
            var Hours    = Time1.Hour   + Time2.Hour;
            var Minutes  = Time1.Minute + Time2.Minute;
            var Seconds  = Time1.Second + Time2.Second;

            if (Seconds > 59)
            {
                Seconds -= 59;
                Minutes++;
            }

            if (Minutes > 59)
            {
                Minutes -= 59;
                Hours++;
            }

            if (Hours > 23)
            {
                Hours -= 23;
                Days++;
            }

            return new TimeSpan(Hours, Minutes, Seconds.Value).
                       Add(TimeSpan.FromDays(Days));

        }

        #endregion

        #region Operator -  (Time1, Time2)

        /// <summary>
        /// Operator -
        /// </summary>
        /// <param name="Time1">A TimeOffset.</param>
        /// <param name="Time2">Another TimeOffset.</param>
        public static TimeSpan operator -  (Time Time1,
                                            Time Time2)
        {

            var Hours    = Time1.Hour   - Time2.Hour;
            var Minutes  = Time1.Minute - Time2.Minute;
            var Seconds  = Time1.Second - Time2.Second;

            return new TimeSpan(Hours   >= 0 ? Hours         : 0,
                                Minutes >= 0 ? Minutes       : 0,
                                Seconds >= 0 ? Seconds.Value : 0);

        }

        #endregion

        #endregion

        #region IComparable<Time> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two TimeOffsets.
        /// </summary>
        /// <param name="Object">A TimeOffset to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Time TimeOffset
                   ? CompareTo(TimeOffset)
                   : throw new ArgumentException("The given object is not a TimeOffset!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Time)

        /// <summary>
        /// Compares two TimeOffsets.
        /// </summary>
        /// <param name="Time">A TimeOffset to compare with.</param>
        public Int32 CompareTo(Time Time)
        {

            var c = Hour.        CompareTo(Time.Hour);

            if (c == 0)
                c = Minute.      CompareTo(Time.Minute);

            if (c == 0 && Second.HasValue && Time.Second.HasValue)
                c = Second.Value.CompareTo(Time.Second.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Time> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TimeOffsets for equality.
        /// </summary>
        /// <param name="Object">A TimeOffset to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Time TimeOffset &&
                   Equals(TimeOffset);

        #endregion

        #region Equals(Time)

        /// <summary>
        /// Compares two TimeOffsets for equality.
        /// </summary>
        /// <param name="Time">A TimeOffset to compare with.</param>
        public Boolean Equals(Time Time)

            => Hour.  Equals(Time.Hour)   &&
               Minute.Equals(Time.Minute) &&
               Second.Equals(Time.Second);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Hour:D2}:{Minute:D2}{(Second.HasValue
                                            ? $":{Second.Value:D2}"
                                            : String.Empty)}";

        #endregion

    }

}
