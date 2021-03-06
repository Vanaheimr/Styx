﻿/*
 * Copyright (c) 2010-2021 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A HourMin.
    /// </summary>
    public readonly struct HourMin : IEquatable<HourMin>,
                                     IComparable<HourMin>,
                                     IComparable
    {

        #region Properties

        /// <summary>
        /// The hour.
        /// </summary>
        public readonly Byte Hour     { get; }

        /// <summary>
        /// The minute.
        /// </summary>
        public readonly Byte Minute   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new hour/minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute.</param>
        public HourMin(Byte  Hour,
                       Byte  Minute)
        {

            #region Initial checks

            if (Hour > 23)
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

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a HourMin must not be null or empty!");

            throw new ArgumentException("The given text representation of a HourMin îs invalid!", nameof(Text));

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

                    var Splited = Text.Split(':');

                    if (Splited.Length != 2)
                        return false;

                    if (!Byte.TryParse(Splited[0], out Byte Hour))
                        return false;

                    if (!Byte.TryParse(Splited[1], out Byte Minute))
                        return false;

                    HourMin = new HourMin(Hour, Minute);

                    return true;

                }
                catch (Exception)
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

            => new HourMin(Hour,
                           Minute);

        #endregion


        #region Operator overloading

        #region Operator == (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (HourMin HourMin1,
                                           HourMin HourMin2)

            => HourMin1.Equals(HourMin2);

        #endregion

        #region Operator != (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (HourMin HourMin1,
                                           HourMin HourMin2)

            => !(HourMin1 == HourMin2);

        #endregion

        #region Operator <  (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (HourMin HourMin1,
                                          HourMin HourMin2)

            => HourMin1.CompareTo(HourMin2) < 0;

        #endregion

        #region Operator <= (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (HourMin HourMin1,
                                           HourMin HourMin2)

            => !(HourMin1 > HourMin2);

        #endregion

        #region Operator >  (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (HourMin HourMin1,
                                          HourMin HourMin2)

            => HourMin1.CompareTo(HourMin2) > 0;

        #endregion

        #region Operator >= (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (HourMin HourMin1,
                                           HourMin HourMin2)

            => !(HourMin1 < HourMin2);

        #endregion

        #endregion

        #region IComparable<HourMin> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is HourMin hourMin)
                return CompareTo(hourMin);

            throw new ArgumentException("The given object is not a HourMin!",
                                        nameof(Object));

        }

        #endregion

        #region CompareTo(HourMin)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin">An object to compare with.</param>
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
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is HourMin HourMin)
                return Equals(HourMin);

            return false;

        }

        #endregion

        #region Equals(HourMin)

        /// <summary>
        /// Compares two HourMins for equality.
        /// </summary>
        /// <param name="HourMin">A HourMin to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(HourMin HourMin)

            => Hour.  Equals(HourMin.Hour) &&
               Minute.Equals(HourMin.Minute);

        #endregion

        #endregion

        #region GetHashCode()

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

            => String.Concat(Hour.ToString("D2"),
                             ":",
                             Minute.ToString("D2"));

        #endregion

    }

}
