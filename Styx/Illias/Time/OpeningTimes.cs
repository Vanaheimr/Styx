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

#region Usings

using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Styx;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class OpeningTimesExtensions
    {

        public static String AsString(this DayOfWeek dayOfWeek)

            => dayOfWeek switch {
                   DayOfWeek.Monday     => "monday",
                   DayOfWeek.Tuesday    => "tuesday",
                   DayOfWeek.Wednesday  => "wednesday",
                   DayOfWeek.Thursday   => "thursday",
                   DayOfWeek.Friday     => "friday",
                   DayOfWeek.Saturday   => "saturday",
                   _                    => "sunday"
               };


        public static String AsFreeText(this DayOfWeek dayOfWeek)

            => dayOfWeek switch {
                   DayOfWeek.Monday     => "Monday",
                   DayOfWeek.Tuesday    => "Tuesday",
                   DayOfWeek.Wednesday  => "Wednesday",
                   DayOfWeek.Thursday   => "Thursday",
                   DayOfWeek.Friday     => "Friday",
                   DayOfWeek.Saturday   => "Saturday",
                   _                    => "Sunday"
               };

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

                var copy = new Dictionary<DayOfWeek, IEnumerable<RegularHours>>();

                foreach (var regularOpening in regularOpenings)
                    copy.Add(regularOpening.Key, regularOpening.Value);

                return new (copy);

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
        public Boolean  IsOpen24Hours
            => regularOpenings.Count == 0;

        /// <summary>
        /// An additoonal free text.
        /// </summary>
        public String?  FreeText         { get; set; }

        #endregion

        #region Constructor(s)

        #region OpeningTime(FreeText = "")

        public OpeningTimes(String  FreeText = "")
        {

            this.regularOpenings      = [];
            this.exceptionalOpenings  = [];
            this.exceptionalClosings  = [];
            this.FreeText             = FreeText;

        }

        #endregion

        #region OpeningTime(FromWeekday, ToWeekday, Text = "")

        public OpeningTimes(DayOfWeek  FromWeekday,
                            DayOfWeek  ToWeekday,
                            String     FreeText = "")

            : this(FreeText)

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

            : this(FreeText)

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

            : this(FreeText)

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

            // Ignore: Begin == End

            if (Begin < End)
            {

                if (!regularOpenings.ContainsKey(Weekday))
                    regularOpenings.Add(Weekday, []);

                regularOpenings[Weekday].Add(
                    new RegularHours(
                        Weekday,
                        Begin,
                        End
                    )
                );

            }

            else if (Begin > End)
            {

                if (!regularOpenings.ContainsKey(Weekday))
                    regularOpenings.Add(Weekday, []);

                regularOpenings[Weekday].Add(
                    new RegularHours(
                        Weekday,
                        Begin,
                        HourMin.Midnight
                    )
                );


                var dayAfter = (DayOfWeek) ((((Byte) Weekday) + 1) % 7);

                if (!regularOpenings.ContainsKey(dayAfter))
                    regularOpenings.Add(dayAfter, []);

                regularOpenings[dayAfter].Add(
                    new RegularHours(
                        dayAfter,
                        HourMin.Midnight,
                        End
                    )
                );

            }

            return this;

        }

        #endregion

        #region AddRegularOpenings(FromWeekday, ToWeekday)

        public OpeningTimes AddRegularOpenings(DayOfWeek  FromWeekday,
                                               DayOfWeek  ToWeekday)
        {

            var fromWeekday = (int) FromWeekday;
            var toWeekday   = (int) ToWeekday;

            if (toWeekday < fromWeekday)
                toWeekday += 7;

            for (var weekday = fromWeekday; weekday <= toWeekday; weekday++)
            {

                if (!regularOpenings.ContainsKey((DayOfWeek) (weekday % 7)))
                    regularOpenings.Add((DayOfWeek) (weekday % 7), []);

                regularOpenings[(DayOfWeek) (weekday % 7)].Add(
                    new RegularHours(
                        (DayOfWeek)(weekday % 7),
                        new HourMin(0, 0),
                        new HourMin(0, 0)
                    )
                );

            }

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
                AddRegularOpening((DayOfWeek) weekday, Begin, End);

            return this;

        }

        #endregion

        #region AddExceptionalClosing(StartTimestamp, EndTimestamp)

        public OpeningTimes AddExceptionalOpening(DateTime  StartTimestamp,
                                                  DateTime  EndTimestamp)
        {

            exceptionalOpenings.Add(
                new ExceptionalPeriod(
                    StartTimestamp,
                    EndTimestamp
                )
            );

            return this;

        }

        #endregion

        #region AddExceptionalClosing(StartTimestamp, EndTimestamp)

        public OpeningTimes AddExceptionalClosing(DateTime  StartTimestamp,
                                                  DateTime  EndTimestamp)
        {

            exceptionalClosings.Add(
                new ExceptionalPeriod(
                    StartTimestamp,
                    EndTimestamp
                )
            );

            return this;

        }

        #endregion


        #region Parse    (Text)

        public static OpeningTimes? Parse(String Text)
        {

            if (TryParse(Text, out var openingTimes, out var errorResponse))
                return openingTimes;

            throw new ArgumentException($"Invalid text representation of opening times: {errorResponse}",
                                        nameof(Text));

        }

        #endregion

        #region Parse    (Texts)

        public static OpeningTimes? Parse(IEnumerable<String> Texts)
        {

            if (TryParse(Texts, out var openingTimes, out var errorResponse))
                return openingTimes;

            throw new ArgumentException($"Invalid text representation of opening times: {errorResponse}",
                                        nameof(Texts));

        }

        #endregion

        #region TryParse (Text)

        public static OpeningTimes? TryParse(String Text)
        {

            if (TryParse(Text, out var openingTimes, out _))
                return openingTimes;

            return null;

        }

        #endregion

        #region TryParse (Text,  out OpeningTimes, out ErrorResponse)

        public static Boolean TryParse(String                                  Text,
                                       [NotNullWhen(true)]  out OpeningTimes?  OpeningTimes,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse([ Text ],
                        out OpeningTimes,
                        out ErrorResponse);

        #endregion

        #region TryParse (Texts, out OpeningTimes, out ErrorResponse)

        public static Boolean TryParse(IEnumerable<String>                     Texts,
                                       [NotNullWhen(true)]  out OpeningTimes?  OpeningTimes,
                                       [NotNullWhen(false)] out String?        ErrorResponse)
        {

            ErrorResponse = null;

            if (!Texts.Any() || Texts.First() == _24_7 || Texts.First() == "{}")
            {
                OpeningTimes = Open24Hours;
                return true;
            }

            OpeningTimes = new OpeningTimes();

            foreach (var text in Texts)
            {

                // "Monday 07:00h - 21:00h"
                // "Monday - Sunday 06:00h - 21:00h"
                var match = Regex.Match(text, "([a-zA-Z]+)( - ([a-zA-Z]+))* ((([0-9]{2}:[0-9]{2})h - ([0-9]{2}:[0-9]{2})h)|open|closed)");
                if (!match.Success)
                {
                    OpeningTimes   = null;
                    ErrorResponse  = "Invalid opening times!";
                    return false;
                }

                #region Parse FromWeekday

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
                        OpeningTimes   = null;
                        ErrorResponse  = $"Invalid FromWeekday '{match.Groups[1].Value.ToLower()}' opening times!";
                        return false;

                }

                #endregion

                #region Parse ToWeekday

                DayOfWeek ToWeekday;

                switch (match.Groups[3].Value.ToLower())
                {

                    case "":
                        ToWeekday = FromWeekday;
                        break;

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
                        OpeningTimes   = null;
                        ErrorResponse  = $"Invalid ToWeekday '{match.Groups[3].Value.ToLower()}' opening times!";
                        return false;

                }

                #endregion

                #region Parse hours...

                if (HourMin.TryParse(match.Groups[6].Value, out var begin) &&
                    HourMin.TryParse(match.Groups[7].Value, out var end))
                {

                    // "Monday 07:00h - 07:00h"
                    if (FromWeekday == ToWeekday && begin == end)
                    {
                        OpeningTimes   = null;
                        ErrorResponse  = $"Invalid hours '{text}'!";
                        return false;
                    }

                    OpeningTimes.AddRegularOpenings(
                        FromWeekday,
                        ToWeekday,
                        begin,
                        end
                    );

                }

                #endregion

                #region ...or parse "open"

                else if (match.Groups[4].Value == "open")
                {
                    OpeningTimes.AddRegularOpenings(FromWeekday, ToWeekday);
                }

                #endregion

                #region ...or parse "closed"

                else if (match.Groups[4].Value == "closed")
                {
                    //OpeningTimes.AddExceptionalClosing(FromWeekday, ToWeekday);
                    OpeningTimes = OpeningTimes.FromFreeText(text);
                    return true;
                }

                #endregion

            }

            if (OpeningTimes.regularOpenings.    Count != 0 ||
                OpeningTimes.exceptionalOpenings.Count != 0)
            {
                return true;
            }

            OpeningTimes   = null;
            ErrorResponse  = $"Invalid opening times!";
            return false;

        }

        #endregion

        #region ToJSON(this OpeningTimes)

        public JObject ToJSON()
        {

            var JSON = JSONObject.Create(

                           new JProperty("24/7", IsOpen24Hours),

                           regularOpenings.Count != 0
                               ? new JProperty("regularOpenings",       new JArray(regularOpenings.Select(regularOpening =>
                                                                            new JObject(
                                                                                new JProperty(
                                                                                    OpeningTimesExtensions.AsString(regularOpening.Key),
                                                                                    new JArray(regularOpening.Value.Select(regularHours => regularHours.ToJSON()))
                                                                                )
                                                                            )
                                                                        ))
                                 )
                               : null,

                           exceptionalOpenings.Count != 0
                               ? new JProperty("exceptionalOpenings",   new JArray(exceptionalOpenings.Select(exceptionalOpening => exceptionalOpening.ToString())))
                               : null,

                           exceptionalClosings.Count != 0
                               ? new JProperty("exceptionalClosings",   new JArray(exceptionalClosings.Select(exceptionalClosing => exceptionalClosing.ToString())))
                               : null,

                           FreeText.IsNotNullOrEmpty()
                               ? new JProperty("freeText",              FreeText)
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
            => new ();

        #endregion

        #region (static) FromFreeText(Text)

        /// <summary>
        /// The opening times are described within the free text.
        /// </summary>
        public static OpeningTimes FromFreeText(String Text)
            => new (Text);

        #endregion


        #region AsFreeText()

        /// <summary>
        /// Return a free-text representation of this opening times.
        /// </summary>
        public String AsFreeText()
        {

            var list = new List<String>();

            foreach (var regularOpening in regularOpenings)
            {
                foreach (var regularHours in regularOpening.Value)
                {
                    list.Add($"{regularHours.DayOfWeek.AsFreeText()} {regularHours.PeriodBegin}h - {regularHours.PeriodEnd}h");
                }
            }

            return list.AggregateWith("; ");

        }

        #endregion


        #region Operator overloading

        #region Operator == (OpeningTime1, OpeningTime2)

        /// <summary>
        /// Compares two opening times for equality.
        /// </summary>
        /// <param name="OpeningTime1">An opening time.</param>
        /// <param name="OpeningTime2">Another opening time.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (OpeningTimes OpeningTime1,
                                           OpeningTimes OpeningTime2)
        {

            if (ReferenceEquals(OpeningTime1, OpeningTime2))
                return true;

            if (OpeningTime1 is null || OpeningTime2 is null)
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
        public static Boolean operator != (OpeningTimes OpeningTime1,
                                           OpeningTimes OpeningTime2)

            => !(OpeningTime1 == OpeningTime2);

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

               IsOpen24Hours.Equals(OpeningTimes.IsOpen24Hours) &&

               ((FreeText is     null && OpeningTimes.FreeText is     null) ||
                (FreeText is not null && OpeningTimes.FreeText is not null && FreeText.Equals(OpeningTimes.FreeText)));

        #endregion

        #endregion

        #region (override) GetHashCode()

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
