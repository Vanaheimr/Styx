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
    /// Extensions to the DateTimeOffset class.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {

        #region UnixEpoch

        /// <summary>
        /// The UNIX epoch.
        /// </summary>
        public static readonly DateTimeOffset UnixEpoch = new (1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        #endregion

        #region ToUnixTimestamp   (this DateTime)

        /// <summary>
        /// Convert the given DateTime object to UNIX timestamp.
        /// </summary>
        /// <param name="DateTime">A DateTime object.</param>
        /// <returns>The seconds since 1. January 1970</returns>
        public static Int64 ToUnixTimestamp(this DateTimeOffset DateTime)

            => (Int64) Math.Floor(DateTime.Subtract(UnixEpoch).TotalSeconds);

        #endregion

        #region FromUnixTimestamp (this UnixTimestamp)

        /// <summary>
        /// Convert the given UNIX timestamp to a .NET DateTime object.
        /// </summary>
        /// <param name="UnixTimestamp">A UNIX timestamp (seconds since 1. January 1970)</param>
        public static DateTimeOffset FromUnixTimestamp(this Int64 UnixTimestamp)

            => UnixEpoch.AddSeconds(UnixTimestamp);

        #endregion


        #region ToIso8601(this DateTimeOffset, Fractions = true)

        /// <summary>
        /// Convert the given DateTimeOffset object to an ISO 8601 datetime string.
        /// </summary>
        /// <param name="DateTimeOffset">A DateTimeOffset object.</param>
        /// <param name="Fractions">Include the fractions of seconds.</param>
        /// <returns>The DateTimeOffset formatted as "yyyy-MM-ddTHH:mm:ss.fff" + "Z"</returns>
        /// <example>2014-02-01T15:45:00.000Z</example>
        public static String ToIso8601(this DateTimeOffset  DateTimeOffset,
                                       Boolean              Fractions   = true)

            => DateTimeOffset.ToUniversalTime().
                        ToString("yyyy-MM-ddTHH:mm:ss" + (Fractions ? ".fff" : "")) + "Z";

        #endregion

        #region ToIso8601WithOffset(this DateTimeOffset, Fractions = true)

        /// <summary>
        /// Convert the given DateTimeOffset object to an ISO 8601 datetime string with time zone offset.
        /// </summary>
        /// <param name="DateTimeOffset">A DateTimeOffset object.</param>
        /// <param name="Fractions">Include the fractions of seconds.</param>
        /// <returns>The DateTimeOffset formatted as "yyyy-MM-ddTHH:mm:ss.fffzzz"</returns>
        /// <example>2014-02-01T15:45:00.000+00:00</example>
        public static String ToIso8601WithOffset(this DateTimeOffset  DateTimeOffset,
                                                 Boolean              Fractions   = true)

            => DateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ss" + (Fractions ? ".fff" : "") + "zzz");

        #endregion


    }

}
