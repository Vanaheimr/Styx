/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
    public readonly struct Time : IEquatable<Time>,
                                  IComparable<Time>,
                                  IComparable
    {

        #region Properties

        /// <summary>
        /// The hour.
        /// </summary>
        public readonly Byte   Hour      { get; }

        /// <summary>
        /// The minute.
        /// </summary>
        public readonly Byte   Minute    { get; }

        /// <summary>
        /// The second.
        /// </summary>
        public readonly Byte?  Second    { get; }

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

            if (Hour   > 23)
                throw new ArgumentException("The hour value is invalid!",   nameof(Hour));

            if (Minute > 59)
                throw new ArgumentException("The minute value is invalid!", nameof(Minute));

            if (Second > 59)
                throw new ArgumentException("The second value is invalid!", nameof(Second));


            this.Hour    = Hour;
            this.Minute  = Minute;
            this.Second  = Second;

        }

        #endregion


        #region FromHour      (Hour)

        /// <summary>
        /// Create a new time based on the given hour.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        public static Time FromHour(Byte Hour)

            => new Time(Hour);

        #endregion

        #region FromHourMin   (Hour, Minute)

        /// <summary>
        /// Create a new time based on the given hour and minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute</param>
        public static Time FromHourMin(Byte Hour, Byte Minute)

            => new Time(Hour,
                        Minute);

        #endregion

        #region FromHourMinSec(Hour, Minute, Second)

        /// <summary>
        /// Create a new time based on the given hour and minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute</param>
        /// <param name="Second">The second.</param>
        public static Time FromHourMinSec(Byte Hour, Byte Minute, Byte Second)

            => new Time(Hour,
                        Minute,
                        Second);

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as time.
        /// </summary>
        /// <param name="Text">A text representation of the time.</param>
        public static Time Parse(String Text)
        {

            if (TryParse(Text, out Time time))
                return time;

            throw new ArgumentException("Could not parse the given text as a time.!");

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as time.
        /// </summary>
        /// <param name="Text">A text representation of the time.</param>
        public static Time? TryParse(String Text)
        {

            if (TryParse(Text, out Time time))
                return time;

            return null;

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

            Time = default;

            var Fragments = Text?.Trim()?.Split(':');

            if (Fragments == null)
                return false;

            else if (Fragments.Length == 1)
            {

                if (!Byte.TryParse(Fragments[0], out Byte Hour))
                    return false;

                Time = Time.FromHour(Hour);
                return true;

            }

            else if (Fragments.Length == 2)
            {

                if (!Byte.TryParse(Fragments[0], out Byte Hour))
                    return false;

                if (!Byte.TryParse(Fragments[1], out Byte Minute))
                    return false;

                Time = Time.FromHourMin(Hour, Minute);
                return true;

            }

            else if (Fragments.Length == 3)
            {

                if (!Byte.TryParse(Fragments[0], out Byte Hour))
                    return false;

                if (!Byte.TryParse(Fragments[1], out Byte Minute))
                    return false;

                if (!Byte.TryParse(Fragments[2], out Byte Second))
                    return false;

                Time = Time.FromHourMinSec(Hour, Minute, Second);
                return true;

            }

            return false;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Time Time1,
                                           Time Time2)

            => Time1.Equals(Time2);

        #endregion

        #region Operator != (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Time Time1,
                                           Time Time2)

            => !(Time1 == Time2);

        #endregion

        #region Operator <  (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Time Time1,
                                          Time Time2)

            => Time1.CompareTo(Time2) < 0;

        #endregion

        #region Operator <= (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Time Time1,
                                           Time Time2)

            => !(Time1 > Time2);

        #endregion

        #region Operator >  (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Time Time1,
                                          Time Time2)

            => Time1.CompareTo(Time2) > 0;

        #endregion

        #region Operator >= (Time1, Time2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Time Time1,
                                           Time Time2)

            => !(Time1 < Time2);

        #endregion

        #region Operator +  (Time1, Time2)

        /// <summary>
        /// Operator +
        /// </summary>
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
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
        /// <param name="Time1">A time.</param>
        /// <param name="Time2">Another time.</param>
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
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Time time
                   ? CompareTo(time)
                   : throw new ArgumentException("The given object is not a time!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Time)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Time">An object to compare with.</param>
        public Int32 CompareTo(Time Time)
        {

            var c = Hour.        CompareTo(Time.Hour);

            if (c == 0)
                c = Minute.      CompareTo(Time.Minute);

            if (c == 0 && Second.HasValue)
                c = Second.Value.CompareTo(Time.Second.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Time> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Time time &&
                   Equals(time);

        #endregion

        #region Equals(Time)

        /// <summary>
        /// Compares two price components for equality.
        /// </summary>
        /// <param name="Time">A time to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Time Time)

            => Hour.  Equals(Time.Hour)   &&
               Minute.Equals(Time.Minute) &&
               Second.Equals(Time.Second);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Hour.  GetHashCode() * 5 ^
                       Minute.GetHashCode() * 3 ^
                       Second.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Second.HasValue
                   ? String.Concat(Hour.ToString("D2"), ":", Minute.ToString("D2"), ":", Second.Value.ToString("D2"))
                   : String.Concat(Hour.ToString("D2"), ":", Minute.ToString("D2"));

        #endregion

    }

}
