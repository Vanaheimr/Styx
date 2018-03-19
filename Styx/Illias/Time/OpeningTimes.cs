/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static partial class JSON_IO
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
                         ? new JProperty(JPropertyKey, "24/7")
                         : new JProperty(JPropertyKey, OpeningTimes.ToJSON())
                   : null;

        #endregion

    }


    /// <summary>
    /// Opening times.
    /// </summary>
    public class OpeningTimes : IEquatable<OpeningTimes>
    {

        #region Properties

        #region RegularOpenings

        private readonly RegularHours[] _RegularOpenings;

        /// <summary>
        /// The regular openings.
        /// </summary>
        public IEnumerable<RegularHours> RegularOpenings
        {
            get
            {
                return _RegularOpenings.Where(rh => !(rh.DayOfWeek == DayOfWeek.Sunday &&
                                                    rh.PeriodBegin.Hour == 0 && rh.PeriodBegin.Minute == 0 &&
                                                    rh.PeriodEnd.  Hour == 0 && rh.PeriodEnd.  Minute == 0));
            }
        }

        #endregion

        private readonly List<ExceptionalPeriod> _ExceptionalOpenings;
        private readonly List<ExceptionalPeriod> _ExceptionalClosings;

        #region IsOpen24Hours

        private readonly Boolean _IsOpen24Hours;

        /// <summary>
        /// 24/7 open...
        /// </summary>
        public Boolean IsOpen24Hours
        {
            get
            {
                return _IsOpen24Hours;
            }
        }

        #endregion

        #region FreeText

        private String _FreeText;

        /// <summary>
        /// An additoonal free text.
        /// </summary>
        public String FreeText
        {

            get
            {
                return _FreeText;
            }

            set
            {
                _FreeText = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        #region OpeningTime(IsOpen24Hours, FreeText = "")

        private OpeningTimes(Boolean  IsOpen24Hours,
                             String   FreeText = "")
        {

            this._RegularOpenings      = new RegularHours[7];
            this._ExceptionalOpenings  = new List<ExceptionalPeriod>();
            this._ExceptionalClosings  = new List<ExceptionalPeriod>();
            this._FreeText             = FreeText;
            this._IsOpen24Hours        = IsOpen24Hours;

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


        public OpeningTimes SetRegularOpening(DayOfWeek  Weekday,
                                              HourMin    Begin,
                                              HourMin    End)
        {

            _RegularOpenings[(int) Weekday] = new RegularHours(Weekday, Begin, End);

            return this;

        }

        public OpeningTimes SetRegularOpening(DayOfWeek  FromWeekday,
                                              DayOfWeek  ToWeekday,
                                              HourMin    Begin,
                                              HourMin    End)
        {

            var _FromWeekday = (int) FromWeekday;
            var _ToWeekday   = (int) ToWeekday;

            if (_ToWeekday < _FromWeekday)
                _ToWeekday += 7;

            for (var weekday = _FromWeekday; weekday <= _ToWeekday; weekday++)
                _RegularOpenings[weekday % 7] = new RegularHours((DayOfWeek) (weekday % 7), Begin, End);

            return this;

        }

        public OpeningTimes AddExceptionalOpening(DateTime Start, DateTime End)
        {

            _ExceptionalOpenings.Add(new ExceptionalPeriod(Start, End));

            return this;

        }

        public OpeningTimes AddExceptionalClosing(DateTime Start, DateTime End)
        {

            _ExceptionalClosings.Add(new ExceptionalPeriod(Start, End));

            return this;

        }


        public static OpeningTimes FromFreeText(String   Text,
                                                Boolean  IsOpen24Hours = true)
        {

            return new OpeningTimes(IsOpen24Hours, Text);

        }


        public static OpeningTimes Parse(String Text)
        {

            OpeningTimes _OpeningTimes = null;

            if (TryParse(Text, out _OpeningTimes))
                return _OpeningTimes;

            return null;

        }

        public static Boolean TryParse(String Text, out OpeningTimes OpeningTimes)
        {

            OpeningTimes = null;

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

            => new OpeningTimes(IsOpen24Hours: true);

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
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is an OpeningTime.
            var OpenTime = Object as OpeningTimes;
            if ((Object) OpenTime == null)
                return false;

            return this.Equals(OpenTime);

        }

        #endregion

        #region Equals(OpeningTimes)

        /// <summary>
        /// Compares two opening times for equality.
        /// </summary>
        /// <param name="OpeningTimes">An opening time to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(OpeningTimes OpeningTimes)
        {

            if ((Object) OpeningTimes == null)
                return false;

            if (IsOpen24Hours && OpeningTimes.IsOpen24Hours)
                return true;

            return FreeText.Equals(OpeningTimes.FreeText);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return FreeText.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return IsOpen24Hours ? "24 hours" : FreeText;
        }

        #endregion

    }

}
