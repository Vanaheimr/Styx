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
    /// Extention methods for time ranges.
    /// </summary>
    public static class TimeRangeExtentions
    {

        #region To(this TimeRange, EndTime)

        /// <summary>
        /// Return a new time range having the end time set to the given value.
        /// </summary>
        /// <param name="TimeRange">A time range object.</param>
        /// <param name="EndTime">The new ending time.</param>
        public static TimeRange To(this TimeRange TimeRange, Time EndTime)
        {
            return new TimeRange(TimeRange.StartTime, EndTime);
        }

        /// <summary>
        /// Return a new time range having the end time set to the given value.
        /// </summary>
        /// <param name="TimeRange">A time range object.</param>
        /// <param name="EndTime">The new ending time.</param>
        public static TimeRange To(this TimeRange TimeRange, Byte EndTime)
        {
            return new TimeRange(TimeRange.StartTime, Time.FromHour(EndTime));
        }

        /// <summary>
        /// Return a new time range having the end time set to the given value.
        /// </summary>
        /// <param name="TimeRange">A time range object.</param>
        /// <param name="EndTime">The new ending time.</param>
        public static TimeRange To(this TimeRange TimeRange, String EndTime)
        {
            return new TimeRange(TimeRange.StartTime, Time.Parse(EndTime));
        }

        #endregion

    }

}
