/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extensions to the DateTime class.
    /// </summary>
    public static class DateTimeExtensions
    {

        #region UnixEpoch

        /// <summary>
        /// The UNIX epoch.
        /// </summary>
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region ToUnixTimestamp(this DateTime)

        /// <summary>
        /// Convert the given DateTime object to UNIX timestamp.
        /// </summary>
        /// <param name="DateTime">A DateTime object.</param>
        /// <returns>The seconds since 1. January 1970</returns>
        public static Int64 ToUnixTimestamp(this DateTime DateTime)

            => (Int64) Math.Floor(DateTime.Subtract(UnixEpoch).TotalSeconds);

        #endregion

        #region FromUnixTimestamp(this UnixTimestamp)

        /// <summary>
        /// Convert the given UNIX timestamp to a .NET DateTime object.
        /// </summary>
        /// <param name="UnixTimestamp">A UNIX timestamp (seconds since 1. January 1970)</param>
        public static DateTime FromUnixTimestamp(this Int64 UnixTimestamp)

            => UnixEpoch.AddSeconds(UnixTimestamp);

        #endregion


        #region ToIso8601(this DateTime, Fractions = true)

        /// <summary>
        /// Convert the given DateTime object to an ISO 8601 datetime string.
        /// </summary>
        /// <param name="DateTime">A DateTime object.</param>
        /// <param name="Fractions">Iclude the fractions of seconds.</param>
        /// <returns>The DateTime formated as "yyyy-MM-ddTHH:mm:ss.fff" + "Z"</returns>
        /// <example>2014-02-01T15:45:00.000Z</example>
        public static String ToIso8601(this DateTime  DateTime,
                                       Boolean        Fractions   = true)

            => DateTime.ToUniversalTime().
                        ToString("yyyy-MM-ddTHH:mm:ss" + (Fractions ? ".fff" : "")) + "Z";

        #endregion

        #region ToIso8601WithOffset(this DateTime, Fractions = true)

        /// <summary>
        /// Convert the given DateTime object to an ISO 8601 datetime string with timezone offset.
        /// </summary>
        /// <param name="DateTime">A DateTime object.</param>
        /// <param name="Fractions">Iclude the fractions of seconds.</param>
        /// <returns>The DateTime formated as "yyyy-MM-ddTHH:mm:ss.fffzzz"</returns>
        /// <example>2014-02-01T15:45:00.000+00:00</example>
        public static String ToIso8601WithOffset(this DateTime  DateTime,
                                                 Boolean        Fractions   = true)

            => (DateTime.Kind == DateTimeKind.Utc
                    ? DateTime.ToLocalTime()
                    : DateTime).

                ToString("yyyy-MM-ddTHH:mm:ss" + (Fractions ? ".fff" : "") + "zzz");

        #endregion

        #region ToRfc1123(this DateTime)

        /// <summary>
        /// Convert the given DateTime object to an RFC 1123 datetime string.
        /// </summary>
        /// <param name="DateTime">A DateTime object.</param>
        /// <returns>The DateTime formated as e.g. "Wed, 24 Nov 2016 09:44:55 GMT"</returns>
        public static String ToRfc1123(this DateTime DateTime)

            => DateTime.ToUniversalTime().ToString("R");

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text representation of a DateTime object.
        /// </summary>
        /// <param name="Text">A text representation of a DateTime object.</param>
        public static DateTime? TryParse(String Text)
        {

            if (DateTime.TryParse(Text, out DateTime dateTime))
                return dateTime;

            return null;

        }

        #endregion


    }

}
