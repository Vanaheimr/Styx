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
    /// Extension methods for StdDev&lt;TimeSpan&gt;.
    /// </summary>
    public static partial class StdDevTimeSpanExtensions
    {

        #region (static) FromMilliseconds (Number, StdDev)

        /// <summary>
        /// Parse the given numbers as a milliseconds time span with standard deviation.
        /// </summary>
        /// <param name="Number">A numeric representation of milliseconds.</param>
        /// <param name="StdDev">The standard deviation of the value in milliseconds.</param>
        public static StdDev<TimeSpan> FromMilliseconds(Double Number,
                                                        Double StdDev)

            => new (
                   TimeSpan.FromMilliseconds(Number),
                   TimeSpan.FromMilliseconds(StdDev)
               );

        #endregion

        #region (static) FromSeconds      (Number, StdDev)

        /// <summary>
        /// Parse the given numbers as a seconds time span with standard deviation.
        /// </summary>
        /// <param name="Number">A numeric representation of seconds.</param>
        /// <param name="StdDev">The standard deviation of the value in seconds.</param>
        public static StdDev<TimeSpan> FromSeconds(Double Number,
                                                   Double StdDev)

            => new (
                   TimeSpan.FromSeconds(Number),
                   TimeSpan.FromSeconds(StdDev)
               );

        #endregion

    }

}
