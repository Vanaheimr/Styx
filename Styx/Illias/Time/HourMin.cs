/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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
    /// A hour with minutes.
    /// </summary>
    public readonly struct HourMin : IEquatable<HourMin>,
                                     IComparable<HourMin>,
                                     IComparable
    {

        #region Properties

        /// <summary>
        /// The hour.
        /// </summary>
        public readonly Byte  Hour      { get; }

        /// <summary>
        /// The minutes.
        /// </summary>
        public readonly Byte  Minute    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new hour with minutes.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minutes.</param>
        public HourMin(Byte  Hour,
                       Byte  Minute)
        {

            #region Initial checks

            if (Hour   > 23)
                throw new ArgumentException("The given hour is invalid!",   nameof(Hour));

            if (Minute > 59)
                throw new ArgumentException("The given minute is invalid!", nameof(Minute));

            #endregion

            this.Hour    = Hour;
            this.Minute  = Minute;

        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a HourMin.
        /// </summary>
        /// <param name="Text">A text representation of a HourMin.</param>
        public static HourMin Parse(String Text)
        {

            if (TryParse(Text, out HourMin hourMin))
                return hourMin;

            throw new ArgumentException("The given text representation of a hour with minutes is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a HourMin.
        /// </summary>
        /// <param name="Text">A text representation of a HourMin.</param>
        public static HourMin? TryParse(String Text)
        {

            if (TryParse(Text, out HourMin hourMin))
                return hourMin;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out HourMin)

        public static Boolean TryParse(String Text, out HourMin HourMin)
        {

            HourMin = default;

            if (Text.IsNotNullOrEmpty())
            {
                try
                {

                    var splited = Text.Split(':');

                    if (splited.Length != 2)
                        return false;

                    if (!Byte.TryParse(splited[0], out var hour))
                        return false;

                    if (!Byte.TryParse(splited[1], out var minute))
                        return false;

                    HourMin = new HourMin(hour, minute);

                    return true;

                }
                catch
                { }
            }

            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this HourMin.
        /// </summary>
        public HourMin Clone

            => new (Hour, Minute);

        #endregion


        #region Operator overloading

        #region Operator == (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A hour with minutes.</param>
        /// <param name="HourMin2">Another hour with minutes.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (HourMin HourMin1,
                                           HourMin HourMin2)

            => HourMin1.Equals(HourMin2);

        #endregion

        #region Operator != (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A hour with minutes.</param>
        /// <param name="HourMin2">Another hour with minutes.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (HourMin HourMin1,
                                           HourMin HourMin2)

            => !HourMin1.Equals(HourMin2);

        #endregion

        #region Operator <  (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A hour with minutes.</param>
        /// <param name="HourMin2">Another hour with minutes.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (HourMin HourMin1,
                                          HourMin HourMin2)

            => HourMin1.CompareTo(HourMin2) < 0;

        #endregion

        #region Operator <= (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A hour with minutes.</param>
        /// <param name="HourMin2">Another hour with minutes.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (HourMin HourMin1,
                                           HourMin HourMin2)

            => HourMin1.CompareTo(HourMin2) <= 0;

        #endregion

        #region Operator >  (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A hour with minutes.</param>
        /// <param name="HourMin2">Another hour with minutes.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (HourMin HourMin1,
                                          HourMin HourMin2)

            => HourMin1.CompareTo(HourMin2) > 0;

        #endregion

        #region Operator >= (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A hour with minutes.</param>
        /// <param name="HourMin2">Another hour with minutes.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (HourMin HourMin1,
                                           HourMin HourMin2)

            => HourMin1.CompareTo(HourMin2) >= 0;

        #endregion

        #endregion

        #region IComparable<HourMin> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two hours with minutes.
        /// </summary>
        /// <param name="Object">A hour with minutes to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is HourMin hourMin
                   ? CompareTo(hourMin)
                   : throw new ArgumentException("The given object is not a hour with minutes!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(HourMin)

        /// <summary>
        /// Compares two hours with minutes.
        /// </summary>
        /// <param name="HourMin">A hour with minutes to compare with.</param>
        public Int32 CompareTo(HourMin HourMin)
        {

            var c = Hour.CompareTo(HourMin.Hour);

            if (c == 0)
                c = Minute.CompareTo(HourMin.Minute);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<HourMin> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two hours with minutes for equality.
        /// </summary>
        /// <param name="HourMin">A hour with minutes to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is HourMin hourMin
                   && Equals(hourMin);

        #endregion

        #region Equals(HourMin)

        /// <summary>
        /// Compares two hours with minutes for equality.
        /// </summary>
        /// <param name="HourMin">A hour with minutes to compare with.</param>
        public Boolean Equals(HourMin HourMin)

            => Hour.  Equals(HourMin.Hour) &&
               Minute.Equals(HourMin.Minute);

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
                return Hour.  GetHashCode() * 3 ^
                       Minute.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Hour.  ToString("D2"),
                             ":",
                             Minute.ToString("D2"));

        #endregion

    }

}
