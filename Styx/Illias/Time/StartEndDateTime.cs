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
    /// A structure to store a start and end time.
    /// </summary>
    public struct StartEndDateTime
    {

        #region Properties

        /// <summary>
        /// The start time.
        /// </summary>
        public DateTime   StartTime   { get; }

        /// <summary>
        /// The end time.
        /// </summary>
        public DateTime?  EndTime     { get; }

        /// <summary>
        /// The duration.
        /// </summary>
        public TimeSpan? Duration

            => EndTime.HasValue
                   ? EndTime.Value - StartTime
                   : new TimeSpan?();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new start and end time structure.
        /// </summary>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        public StartEndDateTime(DateTime   StartTime,
                                DateTime?  EndTime = null)
        {

            #region Initial checks

            if (EndTime.HasValue && StartTime > EndTime)
                throw new ArgumentException("The Starttime must not be after the Endtime!");

            #endregion

            this.StartTime  = StartTime;
            this.EndTime    = EndTime;

        }

        #endregion


        #region Now

        /// <summary>
        /// Return a StartEndDateTime object which start time
        /// is set to the current date and time.
        /// </summary>
        public static StartEndDateTime Now

            => new StartEndDateTime(DateTime.Now);

        #endregion

        #region UtcNow

        /// <summary>
        /// Return a StartEndDateTime object which start time
        /// is set to the current UTC date and time.
        /// </summary>
        public static StartEndDateTime UtcNow

            => new StartEndDateTime(DateTime.UtcNow);

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

            => String.Concat(StartTime.ToIso8601(), " -> ", EndTime.HasValue ? EndTime.Value.ToIso8601() : "...");

        #endregion

    }

}
