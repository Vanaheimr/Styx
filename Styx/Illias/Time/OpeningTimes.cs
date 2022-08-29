/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class OpeningTimesExtensions
    {

        public static String AsString(DayOfWeek dayOfWeek)

            => dayOfWeek switch {
                   DayOfWeek.Monday     => "monday",
                   DayOfWeek.Tuesday    => "tuesday",
                   DayOfWeek.Wednesday  => "wednesday",
                   DayOfWeek.Thursday   => "thursday",
                   DayOfWeek.Friday     => "friday",
                   DayOfWeek.Saturday   => "saturday",
                   _                    => "sunday"
               };



        //#region ToJSON(this OpeningTimes, JPropertyKey)

        //public static JProperty ToJSON(this OpeningTimes OpeningTimes, String JPropertyKey)

        //    => OpeningTimes != null
        //           ? OpeningTimes.IsOpen24Hours
        //                 ? new JProperty(JPropertyKey, OpeningTimes._24_7)
        //                 : new JProperty(JPropertyKey, OpeningTimes.ToJSON())
        //           : null;

        //#endregion

    }


    /// <summary>
    /// Opening times.
    /// </summary>
    public class OpeningTimes : IEquatable<OpeningTimes>
    {

        #region Data

        /// <summary>
        /// Open 24/7.
        /// </summary>
        public const String _24_7 = "24/7";

        private readonly Dictionary<DayOfWeek, List<RegularHours>>  regularOpenings;
        private readonly List<ExceptionalPeriod>                    exceptionalOpenings;
        private readonly List<ExceptionalPeriod>                    exceptionalClosings;

        #endregion

        #region Properties

        /// <summary>
        /// Regular Openings
        /// </summary>
        public ReadOnlyDictionary<DayOfWeek, IEnumerable<RegularHours>> RegularOpenings
        {
            get
            {
                var d = new Dictionary<DayOfWeek, IEnumerable<RegularHours>>();

                foreach (var regularOpening in regularOpenings)
                    d.Add(regularOpening.Key, regularOpening.Value);

                return new (d);

            }
        }

        /// <summary>
        /// Exceptional Openings
        /// </summary>
        public IEnumerable<ExceptionalPeriod> ExceptionalOpenings
            => exceptionalOpenings;

        /// <summary>
        /// Exceptional Closings
        /// </summary>
        public IEnumerable<ExceptionalPeriod> ExceptionalClosings
            => exceptionalOpenings;

        /// <summary>
        /// 24/7 open...
        /// </summary>
        public Boolean  IsOpen24Hours    { get; }

        /// <summary>
        /// An additoonal free text.
        /// </summary>
        public String?  FreeText         { get; set; }

        #endregion

        #region Constructor(s)

        #region OpeningTime(IsOpen24Hours, FreeText = "")

        public OpeningTimes(Boolean  IsOpen24Hours,
                            String   FreeText = "")
        {

            this.regularOpenings      = new Dictionary<DayOfWeek, List<RegularHours>>();
            this.exceptionalOpenings  = new List<ExceptionalPeriod>();
            this.exceptionalClosings  = new List<ExceptionalPeriod>();
            this.FreeText             = FreeText;
            this.IsOpen24Hours        = IsOpen24Hours;

        }

        #endregion

        #region OpeningTime(FromWeekday, ToWeekday, Text = "")

        public OpeningTimes(DayOfWeek  FromWeekday,
                            DayOfWeek  ToWeekday,
                            String     FreeText = "")

            : this(false, FreeText)

        {

            AddRegularOpenings(FromWeekday,
                               ToWeekday,
                               new HourMin(0, 0),
                               new HourMin(0, 0));

        }

        #endregion

        #region OpeningTime(Weekday, Begin, End, Text = "")

        public OpeningTimes(DayOfWeek  Weekday,
                            HourMin    Begin,
                            HourMin    End,
                            String     FreeText = "")

            : this(false, FreeText)

        {

            AddRegularOpening(Weekday,
                              Begin,
                              End);

        }

        #endregion

        #region OpeningTime(FromWeekday, ToWeekday, Begin, End, Text = "")

        public OpeningTimes(DayOfWeek  FromWeekday,
                            DayOfWeek  ToWeekday,
                            HourMin    Begin,
                            HourMin    End,
                            String     FreeText = "")

            : this(false, FreeText)

        {

            AddRegularOpenings(FromWeekday,
                               ToWeekday,
                               Begin,
                               End);

        }

        #endregion

        #endregion


        #region Documentation

        // "opening_times": {
        //
        //     "regular_hours": [
        //       {
        //         "weekday": 1,
        //         "period_begin": "08:00",
        //         "period_end":   "14:00"
        //       },
        //       {
        //         "weekday": 1,
        //         "period_begin": "19:00",
        //         "period_end":   "23:00"
        //       },
        //       {
        //         "weekday": 2,
        //         "period_begin": "08:00",
        //         "period_end":   "20:00"
        //       },
        //       {
        //         "weekday": 3,
        //         "period_begin": "08:00",
        //         "period_end":   "20:00"
        //       },
        //       {
        //         "weekday": 4,
        //         "period_begin": "08:00",
        //         "period_end":   "20:00"
        //       },
        //       {
        //         "weekday": 5,
        //         "period_begin": "08:00",
        //         "period_end":   "20:00"
        //       }
        //     ],
        //
        //     "twentyfourseven": false,
        //
        //     "exceptional_openings": [
        //       {
        //         "period_begin": "2014-06-21T09:00:00Z",
        //         "period_end":   "2014-06-21T12:00:00Z"
        //       }
        //     ],
        //
        //     "exceptional_closings": [
        //       {
        //         "period_begin": "2014-06-24T00:00:00Z",
        //         "period_end":   "2014-06-25T00:00:00Z"
        //       }
        //     ]
        //
        //   }

        #endregion


        #region AddRegularOpening (Weekday,                Begin, End)

        public OpeningTimes AddRegularOpening(DayOfWeek  Weekday,
                                              HourMin    Begin,
                                              HourMin    End)
        {

            if (!regularOpenings.ContainsKey(Weekday))
                regularOpenings.Add(Weekday, new List<RegularHours>());

            regularOpenings[Weekday].Add(new RegularHours(Weekday,
                                                          Begin,
                                                          End));

            return this;

        }

        #endregion

        #region AddRegularOpenings(FromWeekday, ToWeekday, Begin, End)

        public OpeningTimes AddRegularOpenings(DayOfWeek  FromWeekday,
                                               DayOfWeek  ToWeekday,
                                               HourMin    Begin,
                                               HourMin    End)
        {

            var fromWeekday = (int) FromWeekday;
            var toWeekday   = (int) ToWeekday;

            if (toWeekday < fromWeekday)
                toWeekday += 7;

            for (var weekday = fromWeekday; weekday <= toWeekday; weekday++)
            {

                if (!regularOpenings.ContainsKey((DayOfWeek) (weekday % 7)))
                    regularOpenings.Add((DayOfWeek) (weekday % 7), new List<RegularHours>());

                regularOpenings[(DayOfWeek) (weekday % 7)].Add(new RegularHours((DayOfWeek)(weekday % 7), Begin, End));

            }

            return this;

        }

        #endregion

        #region AddExceptionalClosing(StartTimestamp, EndTimestamp)

        public OpeningTimes AddExceptionalOpening(DateTime StartTimestamp, DateTime EndTimestamp)
        {

            exceptionalOpenings.Add(new ExceptionalPeriod(StartTimestamp, EndTimestamp));

            return this;

        }

        #endregion

        #region AddExceptionalClosing(StartTimestamp, EndTimestamp)

        public OpeningTimes AddExceptionalClosing(DateTime StartTimestamp, DateTime EndTimestamp)
        {

            exceptionalClosings.Add(new ExceptionalPeriod(StartTimestamp, EndTimestamp));

            return this;

        }

        #endregion


        #region FromFreeText(Text, IsOpen24Hours = true)

        public static OpeningTimes FromFreeText(String   Text,
                                                Boolean  IsOpen24Hours = true)

            => new (IsOpen24Hours, Text);

        #endregion

        #region Parse(Text)

        public static OpeningTimes? Parse(String Text)
        {

            if (TryParse(Text, out OpeningTimes? openingTimes))
                return openingTimes;

            return null;

        }

        #endregion

        #region TryParse(Text, out OpeningTimes)

        public static Boolean TryParse(String Text, out OpeningTimes? OpeningTimes)
        {

            if (Text == _24_7)
            {
                OpeningTimes = new OpeningTimes(IsOpen24Hours: true);
                return true;
            }

            OpeningTimes = null;

            if (Text == "{}")
                return true;

            var match = Regex.Match(Text, "([a-zA-Z]+) - ([a-zA-Z]+) (([0-9]{2}:[0-9]{2})h - ([0-9]{2}:[0-9]{2})h|open|closed)");
            if (!match.Success)
                return false;

            #region Parse weekdays

            DayOfWeek FromWeekday;

            switch (match.Groups[1].Value.ToLower())
            {

                case "mo":
                case "mon":
                case "monday":
                    FromWeekday = DayOfWeek.Monday;
                    break;

                case "tu":
                case "di":
                case "tue":
                case "tuesday":
                    FromWeekday = DayOfWeek.Tuesday;
                    break;

                case "we":
                case "mi":
                case "wed":
                case "wednesday":
                    FromWeekday = DayOfWeek.Wednesday;
                    break;

                case "th":
                case "do":
                case "thu":
                case "thursday":
                    FromWeekday = DayOfWeek.Thursday;
                    break;

                case "fr":
                case "fri":
                case "friday":
                    FromWeekday = DayOfWeek.Friday;
                    break;

                case "sa":
                case "sat":
                case "saturday":
                    FromWeekday = DayOfWeek.Saturday;
                    break;

                case "su":
                case "so":
                case "sun":
                case "sunday":
                    FromWeekday = DayOfWeek.Sunday;
                    break;

                default:
                    return false;

            }


            DayOfWeek ToWeekday;

            switch (match.Groups[2].Value.ToLower())
            {

                case "mo":
                case "mon":
                case "monday":
                    ToWeekday = DayOfWeek.Monday;
                    break;

                case "tu":
                case "di":
                case "tue":
                case "tuesday":
                    ToWeekday = DayOfWeek.Tuesday;
                    break;

                case "we":
                case "mi":
                case "wed":
                case "wednesday":
                    ToWeekday = DayOfWeek.Wednesday;
                    break;

                case "th":
                case "do":
                case "thu":
                case "thursday":
                    ToWeekday = DayOfWeek.Thursday;
                    break;

                case "fr":
                case "fri":
                case "friday":
                    ToWeekday = DayOfWeek.Friday;
                    break;

                case "sa":
                case "sat":
                case "saturday":
                    ToWeekday = DayOfWeek.Saturday;
                    break;

                case "su":
                case "so":
                case "sun":
                case "sunday":
                    ToWeekday = DayOfWeek.Sunday;
                    break;

                default:
                    return false;

            }

            #endregion

            #region Parse hours...

            if (HourMin.TryParse(match.Groups[4].Value, out HourMin begin) &&
                HourMin.TryParse(match.Groups[5].Value, out HourMin end))
            {
                OpeningTimes = new OpeningTimes(FromWeekday, ToWeekday, begin, end);
                return true;
            }

            #endregion

            #region ...or parse "open"

            else if (match.Groups[3].Value == "open")
            {
                OpeningTimes = new OpeningTimes(FromWeekday, ToWeekday);
                return true;
            }

            #endregion

            #region ...or parse "closed"

            else if (match.Groups[3].Value == "closed")
            {
                OpeningTimes = new OpeningTimes(false, Text);
                return true;
            }

            #endregion

            return false;

        }

        #endregion

        #region ToJSON(this OpeningTimes)

        public JObject ToJSON()
        {

            var JSON = JSONObject.Create(

                           new JProperty("24/7", IsOpen24Hours),

                           regularOpenings.Any()
                               ? new JProperty("regularOpenings",      new JArray(regularOpenings.Select(regularOpening =>
                                                                           new JObject(
                                                                               new JProperty(
                                                                                   OpeningTimesExtensions.AsString(regularOpening.Key),
                                                                                   new JArray(regularOpening.Value.Select(regularHours => regularHours.ToJSON()))
                                                                               )
                                                                           )
                                                                       ))
                                 )
                               : null,

                           exceptionalOpenings.Any()
                               ? new JProperty("exceptionalOpenings",  new JArray(exceptionalOpenings.Select(exceptionalOpening => exceptionalOpening.ToString())))
                               : null,

                           exceptionalClosings.Any()
                               ? new JProperty("exceptionalClosings",  new JArray(exceptionalClosings.Select(exceptionalClosing => exceptionalClosing.ToString())))
                               : null,

                           FreeText.IsNotNullOrEmpty()
                               ? new JProperty("freeText",             FreeText)
                               : null

                       );

            return JSON;

        }

        #endregion



        #region (static) Open24Hours

        /// <summary>
        /// Is open for 24 hours a day (7 days a week).
        /// </summary>
        public static OpeningTimes Open24Hours

            => new (IsOpen24Hours: true);

        #endregion


        #region Operator overloading

        #region Operator == (OpeningTime1, OpeningTime2)

        /// <summary>
        /// Compares two opening times for equality.
        /// </summary>
        /// <param name="OpeningTime1">An opening time.</param>
        /// <param name="OpeningTime2">Another opening time.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (OpeningTimes OpeningTime1, OpeningTimes OpeningTime2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(OpeningTime1, OpeningTime2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) OpeningTime1 == null) || ((Object) OpeningTime2 == null))
                return false;

            return OpeningTime1.Equals(OpeningTime2);

        }

        #endregion

        #region Operator != (OpeningTime1, OpeningTime2)

        /// <summary>
        /// Compares two opening times for inequality.
        /// </summary>
        /// <param name="OpeningTime1">An opening time.</param>
        /// <param name="OpeningTime2">Another opening time.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (OpeningTimes OpeningTime1, OpeningTimes OpeningTime2)
        {
            return !(OpeningTime1 == OpeningTime2);
        }

        #endregion

        #endregion

        #region IEquatable<OpeningTime> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is OpeningTimes openingTimes &&
                   Equals(openingTimes);

        #endregion

        #region Equals(OpeningTimes)

        /// <summary>
        /// Compares two opening times for equality.
        /// </summary>
        /// <param name="OpeningTimes">An opening time to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(OpeningTimes? OpeningTimes)

            => OpeningTimes is not null &&

               IsOpen24Hours && OpeningTimes.IsOpen24Hours &&

               ((FreeText is     null && OpeningTimes.FreeText is     null) ||
                (FreeText is not null && OpeningTimes.FreeText is not null && FreeText.Equals(OpeningTimes.FreeText)));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => FreeText?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => IsOpen24Hours
                   ? "Open 24 hours"
                   : FreeText ?? "";

        #endregion

    }

}
