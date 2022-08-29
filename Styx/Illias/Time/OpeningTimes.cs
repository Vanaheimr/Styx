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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class OpeningTimesExtensions
    {

        #region ToJSON(this OpeningTimes)

        public static JObject ToJSON(this OpeningTimes OpeningTimes)
        {

            var JO = new JObject();
            //OpeningTimes.RegularHours.ForEach(rh => JO.Add(rh.Weekday.ToString(), new JObject(new JProperty("from", rh.Begin.ToString()), new JProperty("to", rh.End.ToString()))));
            OpeningTimes.RegularOpenings.ForEach(rh => JO.Add(rh.DayOfWeek.ToString(), new JArray(rh.PeriodBegin.ToString(), rh.PeriodEnd.ToString())));
            if (OpeningTimes.FreeText.IsNotNullOrEmpty())
                JO.Add("Text", OpeningTimes.FreeText);

            return (OpeningTimes != null)
                       ? OpeningTimes.IsOpen24Hours
                             ? new JObject()
                             : JO
                       : null;

        }

        #endregion

        #region ToJSON(this OpeningTimes, JPropertyKey)

        public static JProperty ToJSON(this OpeningTimes OpeningTimes, String JPropertyKey)

            => OpeningTimes != null
                   ? OpeningTimes.IsOpen24Hours
                         ? new JProperty(JPropertyKey, OpeningTimes._24_7)
                         : new JProperty(JPropertyKey, OpeningTimes.ToJSON())
                   : null;

        #endregion

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

        #endregion

        #region Properties

        #region RegularOpenings

        private readonly RegularHours[] regularOpenings;

        /// <summary>
        /// The regular openings.
        /// </summary>
        public IEnumerable<RegularHours> RegularOpenings
        {
            get
            {
                return regularOpenings.Where(rh => !(rh.DayOfWeek        == DayOfWeek.Sunday                &&
                                                     rh.PeriodBegin.Hour == 0 && rh.PeriodBegin.Minute == 0 &&
                                                     rh.PeriodEnd.  Hour == 0 && rh.PeriodEnd.  Minute == 0));
            }
        }

        #endregion

        private readonly List<ExceptionalPeriod>  exceptionalOpenings;
        private readonly List<ExceptionalPeriod>  exceptionalClosings;

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

        private OpeningTimes(Boolean  IsOpen24Hours,
                             String   FreeText = "")
        {

            this.regularOpenings      = new RegularHours[7];
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

            SetRegularOpening(FromWeekday, ToWeekday, new HourMin(0, 0), new HourMin(0, 0));

        }

        #endregion

        #region OpeningTime(Weekday, Begin, End, Text = "")

        public OpeningTimes(DayOfWeek  Weekday,
                            HourMin    Begin,
                            HourMin    End,
                            String     FreeText = "")

            : this(false, FreeText)

        {

            SetRegularOpening(Weekday, Begin, End);

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

            SetRegularOpening(FromWeekday, ToWeekday, Begin, End);

        }

        #endregion

        #endregion


        // "opening_times": {
        //
        //     "regular_hours": [
        //       {
        //         "weekday": 1,
        //         "period_begin": "08:00",
        //         "period_end":   "20:00"
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


        public OpeningTimes SetRegularOpening(DayOfWeek  Weekday,
                                              HourMin    Begin,
                                              HourMin    End)
        {

            regularOpenings[(int) Weekday] = new RegularHours(Weekday, Begin, End);

            return this;

        }

        public OpeningTimes SetRegularOpening(DayOfWeek  FromWeekday,
                                              DayOfWeek  ToWeekday,
                                              HourMin    Begin,
                                              HourMin    End)
        {

            var fromWeekday = (int) FromWeekday;
            var toWeekday   = (int) ToWeekday;

            if (toWeekday < fromWeekday)
                toWeekday += 7;

            for (var weekday = fromWeekday; weekday <= toWeekday; weekday++)
                regularOpenings[weekday % 7] = new RegularHours((DayOfWeek) (weekday % 7), Begin, End);

            return this;

        }

        public OpeningTimes AddExceptionalOpening(DateTime Start, DateTime End)
        {

            exceptionalOpenings.Add(new ExceptionalPeriod(Start, End));

            return this;

        }

        public OpeningTimes AddExceptionalClosing(DateTime Start, DateTime End)
        {

            exceptionalClosings.Add(new ExceptionalPeriod(Start, End));

            return this;

        }


        public static OpeningTimes FromFreeText(String   Text,
                                                Boolean  IsOpen24Hours = true)

            => new OpeningTimes(IsOpen24Hours, Text);


        public static OpeningTimes? Parse(String Text)
        {

            if (TryParse(Text, out OpeningTimes? openingTimes))
                return openingTimes;

            return null;

        }

        public static Boolean TryParse(String Text, out OpeningTimes? OpeningTimes)
        {

            if (Text == _24_7)
            {
                OpeningTimes = new OpeningTimes(true);
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

            HourMin Begin;
            HourMin End;

            if (HourMin.TryParse(match.Groups[4].Value, out Begin) &&
                HourMin.TryParse(match.Groups[5].Value, out End))
            {
                OpeningTimes = new OpeningTimes(FromWeekday, ToWeekday, Begin, End);
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
        public Boolean Equals(OpeningTimes OpeningTimes)
        {

            return OpeningTimes is not null &&

                   IsOpen24Hours && OpeningTimes.IsOpen24Hours &&

               ((FreeText is     null && OpeningTimes.FreeText is     null) ||
                (FreeText is not null && OpeningTimes.FreeText is not null && FreeText.Equals(OpeningTimes.FreeText)));



        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => FreeText.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => IsOpen24Hours ? "24 hours" : FreeText;

        #endregion

    }

}
