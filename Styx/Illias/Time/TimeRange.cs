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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for time ranges.
    /// </summary>
    public static class TimeRangeExtensions
    {

        #region To(this TimeRange, EndTime)

        /// <summary>
        /// Return a new time range having the end time set to the given value.
        /// </summary>
        /// <param name="TimeRange">A time range object.</param>
        /// <param name="EndTime">The new ending time.</param>
        public static TimeRange To(this TimeRange  TimeRange,
                                   Time            EndTime)

            => new (TimeRange.StartTime,
                    EndTime);


        /// <summary>
        /// Return a new time range having the end time set to the given value.
        /// </summary>
        /// <param name="TimeRange">A time range object.</param>
        /// <param name="EndTime">The new ending time.</param>
        public static TimeRange To(this TimeRange  TimeRange,
                                   Byte            EndTime)

            => new (TimeRange.StartTime,
                    Time.FromHour(EndTime));


        /// <summary>
        /// Return a new time range having the end time set to the given value.
        /// </summary>
        /// <param name="TimeRange">A time range object.</param>
        /// <param name="EndTime">The new ending time.</param>
        public static TimeRange To(this TimeRange  TimeRange,
                                   String          EndTime)

            => new (TimeRange.StartTime,
                    Time.Parse(EndTime));

        #endregion

    }


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

            if (StartTime.HasValue &&
                EndTime.HasValue &&
                StartTime > EndTime)
            {
                throw new ArgumentException("The starting time of the time range must not be after the ending time!");
            }

            #endregion

            this.StartTime  = StartTime;
            this.EndTime    = EndTime;

            unchecked
            {
                hashCode = (StartTime?.GetHashCode() ?? 0) * 3 ^
                           (EndTime?.  GetHashCode() ?? 0);
            }

        }

        #endregion


        #region From(StartTime)

        /// <summary>
        /// Create a new time range having the given starting time.
        /// </summary>
        /// <param name="StartTime">The starting time.</param>
        public static TimeRange From(Time StartTime)

            => new (StartTime,
                    null);


        /// <summary>
        /// Create a new time range having the given starting time.
        /// </summary>
        /// <param name="StartTime">The starting time.</param>
        public static TimeRange From(Byte StartTime)

            => new (Time.FromHour(StartTime),
                    null);


        /// <summary>
        /// Create a new time range having the given starting time.
        /// </summary>
        /// <param name="StartTime">The starting time.</param>
        public static TimeRange From(String StartTime)

            => new (Time.Parse(StartTime),
                    null);

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
        {
            return String.Concat(StartTime.ToString(), " -> ", EndTime.ToString());
        }

        #endregion

    }

}
