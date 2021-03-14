/*
 * Copyright (c) 2010-2021 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
    /// A structure to store a start and end time.
    /// </summary>
    public readonly struct TimeRange
    {

        #region Properties

        /// <summary>
        /// The starting time.
        /// </summary>
        public Time?  StartTime    { get; }

        /// <summary>
        /// The ending time.
        /// </summary>
        public Time?  EndTime      { get; }


        /// <summary>
        /// The duration of the time range.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {

                if (StartTime.HasValue && EndTime.HasValue)
                    return EndTime.Value - StartTime.Value;

                return TimeSpan.Zero;

            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new time range having a start and end time.
        /// </summary>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        public TimeRange(Time?  StartTime,
                         Time?  EndTime)
        {

            #region Initial checks

            if (StartTime.HasValue && EndTime.HasValue && StartTime > EndTime)
                throw new ArgumentException("The starting time of the time range must not be after the ending time!");

            #endregion

            this.StartTime  = StartTime;
            this.EndTime    = EndTime;

        }

        #endregion


        #region From(StartTime)

        /// <summary>
        /// Create a new time range having the given starting time.
        /// </summary>
        /// <param name="StartTime">The starting time.</param>
        public static TimeRange From(Time StartTime)
        {
            return new TimeRange(StartTime, null);
        }

        /// <summary>
        /// Create a new time range having the given starting time.
        /// </summary>
        /// <param name="StartTime">The starting time.</param>
        public static TimeRange From(Byte StartTime)
        {
            return new TimeRange(Time.FromHour(StartTime), null);
        }

        /// <summary>
        /// Create a new time range having the given starting time.
        /// </summary>
        /// <param name="StartTime">The starting time.</param>
        public static TimeRange From(String StartTime)
        {
            return new TimeRange(Time.Parse(StartTime), null);
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
                return StartTime.GetHashCode() * 17 ^ EndTime.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Concat(StartTime.ToString(), " -> ", EndTime.ToString());
        }

        #endregion

    }

}
