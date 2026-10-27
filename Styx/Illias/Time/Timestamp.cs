/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

        /// <summary>
        /// How far the timestamp is ahead of the clock, in ticks: behind it when negative. Read and changed atomically,
        /// as e.g. timers read the timestamp while a test travels.
        /// </summary>
        private static Int64 timeTravel;

        #endregion


        #region (static) Now

        /// <summary>
        /// The current time stamp respecting time travels.
        /// </summary>
        public static DateTimeOffset Now

            => DateTimeOffset.UtcNow + TimeSpan.FromTicks(Interlocked.Read(ref timeTravel));
               // Stopwatch.GetTimestamp() + timeTravel; <= Might be much faster, but changes semantics!

        #endregion

        #region (static) NowDate

        /// <summary>
        /// The current time stamp respecting time travels.
        /// </summary>
        public static DateOnly NowDate

            => DateOnly.FromDateTime(Now.DateTime);

        #endregion

        #region (static) NowTime

        /// <summary>
        /// The current time stamp respecting time travels.
        /// </summary>
        public static TimeOnly NowTime

            => TimeOnly.FromDateTime(Now.DateTime);

        #endregion


        #region (static) Provider

        /// <summary>
        /// This timestamp as a TimeProvider, for everything that would rather
        /// be handed a clock than reach for a global one.
        /// </summary>
        /// <remarks>
        /// Meant as the default for anything that accepts a TimeProvider and
        /// was not given one, so that "nobody passed a clock" keeps meaning
        /// exactly what it meant before this property existed: Timestamp.Now,
        /// time travel included.
        ///
        /// TimeProvider.System would be the obvious default and is the wrong
        /// one: it reads the system clock directly and so quietly opts out of
        /// TravelBackInTime. That is a change of behaviour hiding inside a
        /// default value, which is the kind that is found late.
        /// </remarks>
        public static TimeProvider Provider { get; } = new TimeTravellingTimeProvider();

        private sealed class TimeTravellingTimeProvider : TimeProvider
        {
            public override DateTimeOffset GetUtcNow()
                => Now;
        }

        #endregion


        #region (static) TravelBackInTime   (TimeTravel)

        /// <summary>
        /// Travel back in time: the timestamp goes back by the given amount, whatever its sign.
        /// </summary>
        /// <param name="TimeTravel">The amount of time you want to travel back in time.</param>
        public static void TravelBackInTime(TimeSpan TimeTravel)
        {
            Interlocked.Add(ref timeTravel, -TimeTravel.Duration().Ticks);
        }

        #endregion

        #region (static) TravelForwardInTime(TimeTravel)

        /// <summary>
        /// Travel forward in time: the timestamp goes forward by the given amount, whatever its sign.
        /// </summary>
        /// <param name="TimeTravel">The amount of time you want to travel forward in time.</param>
        public static void TravelForwardInTime(TimeSpan TimeTravel)
        {
            Interlocked.Add(ref timeTravel, TimeTravel.Duration().Ticks);
        }

        #endregion

        #region (static) Reset()

        /// <summary>
        /// Return to normal time.
        /// </summary>
        public static void Reset()
        {
            Interlocked.Exchange(ref timeTravel, 0);
        }

        #endregion


    }

}
