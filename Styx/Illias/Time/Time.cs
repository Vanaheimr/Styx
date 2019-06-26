/*
 * Copyright (c) 2010-2019 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
    /// A structure to store a simple time.
    /// </summary>
    public struct Time
    {

        #region Properties

        #region Hour

        private readonly Byte _Hour;

        /// <summary>
        /// The hour.
        /// </summary>
        public Byte Hour
        {
            get
            {
                return _Hour;
            }
        }

        #endregion

        #region Minute

        private readonly Byte _Minute;

        /// <summary>
        /// The minute.
        /// </summary>
        public Byte Minute
        {
            get
            {
                return _Minute;
            }
        }

        #endregion

        #region Second

        private readonly Byte? _Second;

        /// <summary>
        /// The second.
        /// </summary>
        public Byte? Second
        {
            get
            {
                return _Second;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a simple time.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute.</param>
        /// <param name="Second">The second.</param>
        private Time(Byte   Hour,
                     Byte   Minute  = 0,
                     Byte?  Second  = null)
        {

            #region Initial checks

            if (Hour > 23)
                throw new ArgumentException("The value of the parameter is invalid!", "Hour");

            if (Minute > 59)
                throw new ArgumentException("The value of the parameter is invalid!", "Minute");

            if (Second > 59)
                throw new ArgumentException("The value of the parameter is invalid!", "Second");

            #endregion

            _Hour    = Hour;
            _Minute  = Minute;
            _Second  = Second;

        }

        #endregion


        #region FromHour(Hour)

        /// <summary>
        /// Create a new time based on the given hour.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        public static Time FromHour(Byte Hour)
        {
            return new Time(Hour);
        }

        #endregion

        #region FromHourMin(Hour, Minute)

        /// <summary>
        /// Create a new time based on the given hour and minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute</param>
        public static Time FromHourMin(Byte Hour, Byte Minute)
        {
            return new Time(Hour, Minute);
        }

        #endregion

        #region FromHourMinSec(Hour, Minute, Second)

        /// <summary>
        /// Create a new time based on the given hour and minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute</param>
        /// <param name="Second">The second.</param>
        public static Time FromHourMinSec(Byte Hour, Byte Minute, Byte Second)
        {
            return new Time(Hour, Minute, Second);
        }

        #endregion


        #region (static) Parse(Text)

        /// <summary>
        /// Parse the given text as time.
        /// </summary>
        /// <param name="Text">A text representation of the time.</param>
        public static Time Parse(String Text)
        {

            Time Time;

            if (!TryParse(Text, out Time))
                throw new ArgumentException("Could not parse '" + Text + "' as a time.!");

            return Time;

        }

        #endregion

        #region (static) TryParse(Text, out Time)

        /// <summary>
        /// Try to parse the given text as time.
        /// </summary>
        /// <param name="Text">A text representation of the time.</param>
        /// <param name="Time">The parsed time.</param>
        public static Boolean TryParse(String Text, out Time Time)
        {

            var Fragments = Text.Trim().Split(':');

            Byte Hour    = 0;
            Byte Minute  = 0;
            Byte Second  = 0;

            Time = Time.FromHour(0);

            if (Fragments.Length == 1)
            {

                if (!Byte.TryParse(Fragments[0], out Hour))
                    return false;

                Time = Time.FromHour(Hour);
                return true;

            }

            else if (Fragments.Length == 2)
            {

                if (!Byte.TryParse(Fragments[0], out Hour))
                    return false;

                if (!Byte.TryParse(Fragments[1], out Minute))
                    return false;

                Time = Time.FromHourMin(Hour, Minute);
                return true;

            }

            else if (Fragments.Length == 3)
            {

                if (!Byte.TryParse(Fragments[0], out Hour))
                    return false;

                if (!Byte.TryParse(Fragments[1], out Minute))
                    return false;

                if (!Byte.TryParse(Fragments[2], out Second))
                    return false;

                Time = Time.FromHourMinSec(Hour, Minute, Second);
                return true;

            }

            return false;

        }

        #endregion


        #region (static) Operator -

        /// <summary>
        /// Operator -
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        public static TimeSpan operator -  (Time Time1, Time Time2)
        {

            var Hours    = Time1.Hour   - Time2.Hour;
            var Minutes  = Time1.Minute - Time2.Minute;
            var Seconds  = Time1.Second - Time2.Second;

            return new TimeSpan(Hours   >= 0 ? Hours         : 0,
                                Minutes >= 0 ? Minutes       : 0,
                                Seconds >= 0 ? Seconds.Value : 0);

        }

        #endregion

        #region (static) Operator +

        /// <summary>
        /// Operator +
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        public static TimeSpan operator +  (Time Time1, Time Time2)
        {

            var Days    = 0;
            var Hours   = Time1.Hour   + Time2.Hour;
            var Minutes = Time1.Minute + Time2.Minute;
            var Seconds = Time1.Second + Time2.Second;

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

        #region (static) Operator >

        /// <summary>
        /// Operator >
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        public static Boolean operator >  (Time Time1, Time Time2)
        {

            if (Time1.Hour > Time2.Hour)
                return true;

            if (Time1.Hour < Time2.Hour)
                return false;

            if (Time1.Minute > Time2.Minute)
                return true;

            if (Time1.Minute < Time2.Minute)
                return false;

            if (Time1.Second > Time2.Second)
                return true;

            return false;

        }

        #endregion

        #region (static) Operator <=

        /// <summary>
        /// Operator <=
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        public static Boolean operator <= (Time Time1, Time Time2)
        {

            return !(Time1 > Time2);

        }

        #endregion

        #region (static) Operator <

        /// <summary>
        /// Operator <
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        public static Boolean operator <  (Time Time1, Time Time2)
        {

            if (Time1.Hour > Time2.Hour)
                return false;

            if (Time1.Hour < Time2.Hour)
                return true;

            if (Time1.Minute > Time2.Minute)
                return false;

            if (Time1.Minute < Time2.Minute)
                return true;

            if (Time1.Second > Time2.Second)
                return false;

            return false;

        }

        #endregion

        #region (static) Operator >=

        /// <summary>
        /// Operator >=
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        public static Boolean operator >= (Time Time1, Time Time2)
        {

            return !(Time1 < Time2);

        }

        #endregion


        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                return _Hour.GetHashCode() * 23 ^ _Minute.GetHashCode() * 17 ^ _Second.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {

            if (_Second.HasValue)
                return String.Concat(_Hour.ToString("D2"), ":", _Minute.ToString("D2"), ":", _Second.Value.ToString("D2"));

            return String.Concat(_Hour.ToString("D2"), ":", _Minute.ToString("D2"));

        }

        #endregion

    }

}
