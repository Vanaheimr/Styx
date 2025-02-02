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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A timestamp that allows time traveling.
    /// </summary>
    public static class Timestamp
    {

        #region Data

        private static TimeSpan timeTravel = TimeSpan.Zero;

        #endregion


        #region (static) Now

        /// <summary>
        /// The current time stamp respecting time travels.
        /// </summary>
        public static DateTime Now

            => DateTime.UtcNow + timeTravel;

        #endregion

        #region (static) NowDate

        /// <summary>
        /// The current time stamp respecting time travels.
        /// </summary>
        public static DateOnly NowDate

            => DateOnly.FromDateTime(DateTime.UtcNow + timeTravel);

        #endregion

        #region (static) NowTime

        /// <summary>
        /// The current time stamp respecting time travels.
        /// </summary>
        public static TimeOnly NowTime

            => TimeOnly.FromDateTime(DateTime.UtcNow + timeTravel);

        #endregion


        #region (static) TravelBackInTime   (TimeTravel)

        /// <summary>
        /// Travel back in time.
        /// </summary>
        /// <param name="TimeTravel">The amount of time you want to travel back in time.</param>
        public static void TravelBackInTime(TimeSpan TimeTravel)
        {

            if (TimeTravel.Ticks < 0)
                TimeTravel.Negate();

            timeTravel += TimeTravel;

        }

        #endregion

        #region (static) TravelForwardInTime(TimeTravel)

        /// <summary>
        /// Travel forward in time.
        /// </summary>
        /// <param name="TimeTravel">The amount of time you want to travel forward in time.</param>
        public static void TravelForwardInTime(TimeSpan TimeTravel)
        {

            if (TimeTravel.Ticks < 0)
                TimeTravel.Negate();

            timeTravel += TimeTravel;

        }

        #endregion

        #region (static) Reset()

        /// <summary>
        /// Return to normal time.
        /// </summary>
        public static void Reset()
        {
            timeTravel = TimeSpan.Zero;
        }

        #endregion


    }

}
