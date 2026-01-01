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

#region Usings

using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// TimeSpan Extensions.
    /// </summary>
    public static class TimeSpanExtensions
    {

        #region TryParseMilliseconds (Text)

        /// <summary>
        /// Try to parse the given text representation of a milliseconds time span.
        /// </summary>
        /// <param name="Text">A text representation of a milliseconds time span.</param>
        public static TimeSpan? TryParseMilliseconds(String Text)
        {

            if (!UInt32.TryParse(Text, out var number))
                return null;

            return TimeSpan.FromMilliseconds(number);

        }

        #endregion

        #region TryParseMilliseconds (Text)

        /// <summary>
        /// Try to parse the given text representation of a minutes time span.
        /// </summary>
        /// <param name="Text">A text representation of a minutes time span.</param>
        public static Boolean TryParseMilliseconds(String                             Text,
                                                   [NotNullWhen(true)]  out TimeSpan  Time,
                                                   [NotNullWhen(false)] out String?   ErrorResponse)
        {

            ErrorResponse = null;

            if (Double.TryParse(Text, out var number))
            {
                Time = TimeSpan.FromMilliseconds(number);
                return true;
            }

            Time           = TimeSpan.Zero;
            ErrorResponse  = "Invalid time span format!";
            return false;

        }

        #endregion


        #region TryParseSeconds      (Text)

        /// <summary>
        /// Try to parse the given text representation of a seconds time span.
        /// </summary>
        /// <param name="Text">A text representation of a seconds time span.</param>
        public static TimeSpan? TryParseSeconds(String Text)
        {

            if (!UInt32.TryParse(Text, out var number))
                return null;

            return TimeSpan.FromSeconds(number);

        }

        #endregion

        #region TryParseSeconds      (Text)

        /// <summary>
        /// Try to parse the given text representation of a minutes time span.
        /// </summary>
        /// <param name="Text">A text representation of a minutes time span.</param>
        public static Boolean TryParseSeconds(String                             Text,
                                              [NotNullWhen(true)]  out TimeSpan  Time,
                                              [NotNullWhen(false)] out String?   ErrorResponse)
        {

            ErrorResponse = null;

            if (Double.TryParse(Text, out var number))
            {
                Time = TimeSpan.FromSeconds(number);
                return true;
            }

            Time           = TimeSpan.Zero;
            ErrorResponse  = "Invalid time span format!";
            return false;

        }

        #endregion


        #region TryParseMinutes      (Text)

        /// <summary>
        /// Try to parse the given text representation of a minutes time span.
        /// </summary>
        /// <param name="Text">A text representation of a minutes time span.</param>
        public static TimeSpan? TryParseMinutes(String Text)
        {

            if (!UInt32.TryParse(Text, out var number))
                return null;

            return TimeSpan.FromMinutes(number);

        }

        #endregion

        #region TryParseMinutes      (Text)

        /// <summary>
        /// Try to parse the given text representation of a minutes time span.
        /// </summary>
        /// <param name="Text">A text representation of a minutes time span.</param>
        public static Boolean TryParseMinutes(String                             Text,
                                              [NotNullWhen(true)]  out TimeSpan  Time,
                                              [NotNullWhen(false)] out String?   ErrorResponse)
        {

            ErrorResponse = null;

            if (Double.TryParse(Text, out var number))
            {
                Time = TimeSpan.FromMinutes(number);
                return true;
            }

            Time           = TimeSpan.Zero;
            ErrorResponse  = "Invalid time span format!";
            return false;

        }

        #endregion


        #region TryParseISO8601(Text, out Duration, out ErrorResponse)

        /// <summary>
        /// Try to parse ISO 8601 duration string (e.g., PT1H, PT15M, PT30S) as a TimeSpan.
        /// </summary>
        /// <param name="Text">The ISO 8601 representation of the duration to parse.</param>
        /// <param name="Duration">The parsed duration value.</param>
        /// <param name="ErrorResponse">An optional error message if parsing fails.</param>
        public static Boolean TryParseISO8601(String                              Text,
                                              [NotNullWhen(true)]  out TimeSpan?  Duration,
                                              [NotNullWhen(false)] out String?    ErrorResponse)
        {

            // pattern:      "^(-?)P(?=\\d|T\\d)(?:(\\d+)Y)?(?:(\\d+)M)?(?:(\\d+)([DW]))?(?:T(?:(\\d+)H)?(?:(\\d+)M)?(?:(\\d+(?:\\.\\d+)?)S)?)?$"
            // description:  duration in ISO 8601 format
            // example:      PT1H
            // default:      PT0S

            Duration       = null;
            ErrorResponse  = null;

            if (Text.IsNullOrEmpty() || Text == "PT0S")
            {
                Duration = TimeSpan.Zero;
                return true;
            }

            // ISO 8601 duration pattern: P[nY][nM][nD]T[nH][nM][nS]
            var regex = new Regex(@"^P(?=\d|T\d)(?:(\d+)Y)?(?:(\d+)M)?(?:(\d+)D)?(?:T(?:(\d+)H)?(?:(\d+)M)?(?:(\d+(?:\.\d+)?)S)?)?$");
            var match = regex.Match(Text);

            if (!match.Success)
            {
                ErrorResponse = $"Invalid ISO 8601 duration format: {Text}";
                return false;
            }

            var result = TimeSpan.Zero;

            if (match.Groups[1].Success) // Years (approximate as 365 days)
                result += TimeSpan.FromDays   (Int32. Parse(match.Groups[1].Value) * 365);

            if (match.Groups[2].Success) // Months (approximate as 30 days)
                result += TimeSpan.FromDays   (Int32. Parse(match.Groups[2].Value) * 30);

            if (match.Groups[3].Success) // Days
                result += TimeSpan.FromDays   (Int32. Parse(match.Groups[3].Value));

            if (match.Groups[4].Success) // Hours
                result += TimeSpan.FromHours  (Int32. Parse(match.Groups[4].Value));

            if (match.Groups[5].Success) // Minutes
                result += TimeSpan.FromMinutes(Int32. Parse(match.Groups[5].Value));

            if (match.Groups[6].Success) // Seconds
                result += TimeSpan.FromSeconds(Double.Parse(match.Groups[6].Value));

            Duration = result;

            return true;

        }

        #endregion

        #region ToISO8601(Duration)

        /// <summary>
        /// Convert a TimeSpan to its ISO 8601 representation.
        /// </summary>
        /// <param name="Duration">A TimeSpan value to convert.</param>
        public static String ToISO8601(this TimeSpan? Duration)

            => Duration.HasValue
                   ? Duration.Value.ToISO8601()
                   : "PT0S";


        /// <summary>
        /// Convert a TimeSpan to its ISO 8601 representation.
        /// </summary>
        /// <param name="Duration">A TimeSpan value to convert.</param>
        public static String ToISO8601(this TimeSpan Duration)
        {

            if (Duration == TimeSpan.Zero)
                return "PT0S";

            var sb = new StringBuilder("P");

            if (Duration.Days > 0)
                sb.Append($"{Duration.Days}D");

            if (Duration.Hours   > 0 ||
                Duration.Minutes > 0 ||
                Duration.Seconds > 0)
            {

                sb.Append('T');

                if (Duration.Hours > 0)
                    sb.Append($"{Duration.Hours}H");

                if (Duration.Minutes > 0)
                    sb.Append($"{Duration.Minutes}M");

                if (Duration.Seconds > 0)
                    sb.Append($"{Duration.Seconds}S");

            }

            return sb.ToString();

        }

        #endregion

    }

}
