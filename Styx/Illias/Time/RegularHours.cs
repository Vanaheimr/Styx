/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Regular recurring operation or access hours.
    /// </summary>
    public readonly struct RegularHours : IEquatable<RegularHours>,
                                          IComparable<RegularHours>,
                                          IComparable
    {

        #region Properties

        /// <summary>
        /// Day of the week.
        /// </summary>
        public DayOfWeek  DayOfWeek      { get; }

        /// <summary>
        /// Begin of the regular period.
        /// </summary>
        public HourMin    PeriodBegin    { get; }

        /// <summary>
        /// End of the regular period.
        /// </summary>
        public HourMin    PeriodEnd      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new regular hours object.
        /// </summary>
        /// <param name="DayOfWeek">Day of the week.</param>
        /// <param name="PeriodBegin">Begin of the regular period.</param>
        /// <param name="PeriodEnd">End of the regular period.</param>
        public RegularHours(DayOfWeek  DayOfWeek,
                            HourMin    PeriodBegin,
                            HourMin    PeriodEnd)
        {

            #region Initial checks

            if ((PeriodEnd.Hour    < PeriodBegin.Hour   &&
                 PeriodEnd.Hour   != 0) ||

                (PeriodEnd.Hour   == PeriodBegin.Hour   &&
                 PeriodEnd.Minute  < PeriodBegin.Minute &&
                 PeriodEnd.Minute != 0))
            {

                throw new ArgumentException("The end period '" + PeriodEnd + "h' must be after the start period '" + PeriodBegin + "h'!",
                                            nameof(PeriodEnd));

            }

            #endregion

            this.DayOfWeek    = DayOfWeek;
            this.PeriodBegin  = PeriodBegin;
            this.PeriodEnd    = PeriodEnd;

        }

        #endregion


        #region ToJSON(CustomPeriodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(
                   new JProperty("begin",  PeriodBegin.ToString()),
                   new JProperty("end",    PeriodEnd.  ToString())
               );

        #endregion


        #region Operator overloading

        #region Operator == (RegularHour1, RegularHour2)

        /// <summary>
        /// Compares two regular hourss for equality.
        /// </summary>
        /// <param name="RegularHour1">A regular hour.</param>
        /// <param name="RegularHour2">Another regular hour.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RegularHours RegularHour1,
                                           RegularHours RegularHour2)

            => RegularHour1.Equals(RegularHour2);

        #endregion

        #region Operator != (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two regular hourss for inequality.
        /// </summary>
        /// <param name="RegularHours1">A regular hour.</param>
        /// <param name="RegularHours2">Another regular hour.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RegularHours RegularHour1,
                                           RegularHours RegularHour2)

            => !RegularHour1.Equals(RegularHour2);

        #endregion

        #region Operator <  (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hour.</param>
        /// <param name="RegularHours2">Another regular hour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RegularHours RegularHour1,
                                          RegularHours RegularHour2)

            => RegularHour1.CompareTo(RegularHour2) < 0;

        #endregion

        #region Operator <= (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hour.</param>
        /// <param name="RegularHours2">Another regular hour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RegularHours RegularHour1,
                                           RegularHours RegularHour2)

            => RegularHour1.CompareTo(RegularHour2) <= 0;

        #endregion

        #region Operator >  (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hour.</param>
        /// <param name="RegularHours2">Another regular hour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RegularHours RegularHour1,
                                          RegularHours RegularHour2)

            => RegularHour1.CompareTo(RegularHour2) > 0;

        #endregion

        #region Operator >= (RegularHours1, RegularHours2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RegularHours1">A regular hour.</param>
        /// <param name="RegularHours2">Another regular hour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RegularHours RegularHour1,
                                           RegularHours RegularHour2)

            => RegularHour1.CompareTo(RegularHour2) >= 0;

        #endregion

        #endregion

        #region IComparable<RegularHours> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two regular hours objects for equality.
        /// </summary>
        /// <param name="Object">A regular hour to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RegularHours regularHour
                   ? CompareTo(regularHour)
                   : throw new ArgumentException("The given object is not a regular hour!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(HourMin)

        /// <summary>
        /// Compares two regular hours objects for equality.
        /// </summary>
        /// <param name="RegularHour">A regular hour to compare with.</param>
        public Int32 CompareTo(RegularHours RegularHour)
        {

            var c = DayOfWeek.  CompareTo(RegularHour.DayOfWeek);

            if (c == 0)
                c = PeriodBegin.CompareTo(RegularHour.PeriodBegin);

            if (c == 0)
                c = PeriodEnd.  CompareTo(RegularHour.PeriodEnd);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<RegularHours> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two regular hours objects for equality.
        /// </summary>
        /// <param name="Object">A regular hour to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RegularHours regularHour
                   && Equals(regularHour);

        #endregion

        #region Equals(RegularHour)

        /// <summary>
        /// Compares two regular hours objects for equality.
        /// </summary>
        /// <param name="RegularHour">A regular hour to compare with.</param>
        public Boolean Equals(RegularHours RegularHour)

            => DayOfWeek.  Equals(RegularHour.DayOfWeek)   &&
               PeriodBegin.Equals(RegularHour.PeriodBegin) &&
               PeriodEnd.  Equals(RegularHour.PeriodEnd);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return DayOfWeek.  GetHashCode() * 5 ^
                       PeriodBegin.GetHashCode() * 3 ^
                       PeriodEnd.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => DayOfWeek           == DayOfWeek.Sunday &&
               PeriodBegin.Hour    == 0                &&
               PeriodBegin.Minute  == 0                &&
               PeriodEnd.  Hour    == 0                &&
               PeriodEnd.  Minute  == 0
               ? ""
               : String.Concat(DayOfWeek.ToString(), "s from ", PeriodBegin.ToString(), "h to ", PeriodEnd.ToString(), "h");

        #endregion

    }

}
