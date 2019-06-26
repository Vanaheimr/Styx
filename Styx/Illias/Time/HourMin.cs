/*
 * Copyright (c) 2010-2019 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// This class references business details of Charging Station Operators.
    /// </summary>
    public struct HourMin : IEquatable<HourMin>, IComparable<HourMin>, IComparable
    {

        #region Properties

        /// <summary>
        /// The hour.
        /// </summary>
        public Byte Hour     { get; }

        /// <summary>
        /// The minute.
        /// </summary>
        public Byte Minute   { get; }

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


        #region (static) Parse(Text)

        public static HourMin Parse(String Text)
        {

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text),  "The given text must not be null or empty!");

            var Splited = Text.Split(':');

            if (Splited.Length != 2)
                throw new ArgumentException("The given input '" + Text + "' is not valid!");

            return new HourMin(Byte.Parse(Splited[0]),
                               Byte.Parse(Splited[1]));

        }

        #endregion

        #region (static) TryParse(Text, out HourMin)

        public static Boolean TryParse(String Text, out HourMin HourMin)
        {

            HourMin = new HourMin(0, 0);

            if (Text.IsNullOrEmpty())
                return false;

            var Splited = Text.Split(':');

            if (Splited.Length != 2)
                return false;

            Byte Hour = 0;
            if (!Byte.TryParse(Splited[0], out Hour))
                return false;

            Byte Minute = 0;
            if (!Byte.TryParse(Splited[1], out Minute))
                return false;

            HourMin = new HourMin(Hour, Minute);

            return true;

        }

        #endregion


        #region Operator overloading

        #region Operator == (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator ==(HourMin HourMin1, HourMin HourMin2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(HourMin1, HourMin2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) HourMin1 == null) || ((Object) HourMin2 == null))
                return false;

            return HourMin1.Equals(HourMin2);

        }

        #endregion

        #region Operator != (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(HourMin HourMin1, HourMin HourMin2)
        {
            return !(HourMin1 == HourMin2);
        }

        #endregion

        #region Operator <  (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(HourMin HourMin1, HourMin HourMin2)
        {

            if ((Object) HourMin1 == null)
                throw new ArgumentNullException("The given HourMin1 must not be null!");

            return HourMin1.CompareTo(HourMin2) < 0;

        }

        #endregion

        #region Operator <= (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(HourMin HourMin1, HourMin HourMin2)
        {
            return !(HourMin1 > HourMin2);
        }

        #endregion

        #region Operator >  (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(HourMin HourMin1, HourMin HourMin2)
        {

            if ((Object) HourMin1 == null)
                throw new ArgumentNullException("The given HourMin1 must not be null!");

            return HourMin1.CompareTo(HourMin2) > 0;

        }

        #endregion

        #region Operator >= (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(HourMin HourMin1, HourMin HourMin2)
        {
            return !(HourMin1 < HourMin2);
        }

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

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an HourMin.
            if (!(Object is HourMin))
                throw new ArgumentNullException("The given object is not a HourMin!");

            return CompareTo((HourMin) Object);

        }

        #endregion

        #region CompareTo(HourMin)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin">An object to compare with.</param>
        public Int32 CompareTo(HourMin HourMin)
        {

            if ((Object) HourMin == null)
                throw new ArgumentNullException("The given HourMin must not be null!");

            // Compare CountryIds
            var _Result = Hour.CompareTo(HourMin.Hour);

            // If equal: Compare charging operator identifications
            if (_Result == 0)
                _Result = Minute.CompareTo(HourMin.Minute);

            return _Result;

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

            if (Object == null)
                return false;

            // Check if the given object is an HourMin.
            if (!(Object is HourMin))
                return false;

            return this.Equals((HourMin) Object);

        }

        #endregion

        #region Equals(HourMin)

        /// <summary>
        /// Compares two HourMins for equality.
        /// </summary>
        /// <param name="HourMin">A HourMin to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(HourMin HourMin)
        {

            if ((Object) HourMin == null)
                return false;

            return Hour.  Equals(HourMin.Hour) &&
                   Minute.Equals(HourMin.Minute);

        }

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
                return Hour.GetHashCode() * 23 ^ Minute.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => String.Concat(Hour.ToString("D2"), ":", Minute.ToString("D2"));

        #endregion

    }

}
