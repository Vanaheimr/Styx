/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>WattHours
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

using System.Net.WebSockets;

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A WattHour value.
    /// </summary>
    public readonly struct WattHour : IEquatable <WattHour>,
                                      IComparable<WattHour>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the WattHours.
        /// </summary>
        public Decimal  Value           { get; }

        /// <summary>
        /// The value of the Amperes as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Value;


        /// <summary>
        /// The value as KiloWattHours.
        /// </summary>
        public Decimal  KW
            => Value / 1000;

        /// <summary>
        /// The value as MegaWattHours.
        /// </summary>
        public Decimal  MW
            => Value / 1000000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new WattHour based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a WattHour.</param>
        private WattHour(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a WattHour.
        /// </summary>
        /// <param name="Text">A text representation of a WattHour.</param>
        public static WattHour Parse(String Text)
        {

            if (TryParse(Text, out var wattHour))
                return wattHour;

            throw new ArgumentException($"Invalid text representation of a WattHour: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a WattHour.
        /// </summary>
        /// <param name="Number">A numeric representation of a WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour Parse(Decimal  Number,
                                     Int32?   Exponent = null)
        {

            if (TryParse(Number, out var wattHour, Exponent))
                return wattHour;

            throw new ArgumentException($"Invalid numeric representation of a WattHour: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a WattHour.
        /// </summary>
        /// <param name="Number">A numeric representation of a WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour Parse(Byte    Number,
                                     Int32?  Exponent = null)
        {

            if (TryParse(Number, out var wattHour, Exponent))
                return wattHour;

            throw new ArgumentException($"Invalid numeric representation of a WattHour: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a WattHour.
        /// </summary>
        /// <param name="Text">A text representation of a WattHour.</param>
        public static WattHour? TryParse(String Text)
        {

            if (TryParse(Text, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParse(Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a WattHour.
        /// </summary>
        /// <param name="Number">A numeric representation of a WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryParse(Decimal  Number,
                                         Int32?   Exponent = null)
        {

            if (TryParse(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a WattHour.
        /// </summary>
        /// <param name="Number">A numeric representation of a WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryParse(Byte    Number,
                                         Int32?  Exponent = null)
        {

            if (TryParse(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out WattHour)

        /// <summary>
        /// Parse the given string as a WattHour.
        /// </summary>
        /// <param name="Text">A text representation of a WattHour.</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        public static Boolean TryParse(String Text, out WattHour WattHour)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("kWh") || Text.EndsWith("KWh"))
                    factor = 1000;

                if (Text.EndsWith("MWh"))
                    factor = 1000000;

                if (Text.EndsWith("GWh"))
                    factor = 1000000000;

                if (Decimal.TryParse(Text, out var value))
                {

                    WattHour = new WattHour(value / factor);

                    return true;

                }

            }
            catch
            { }

            WattHour = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out WattHour, Exponent = null)

        /// <summary>
        /// Parse the given number as a WattHour.
        /// </summary>
        /// <param name="Number">A numeric representation of a WattHour.</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParse(Byte          Number,
                                       out WattHour  WattHour,
                                       Int32?        Exponent = null)
        {

            try
            {

                WattHour = new WattHour(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                WattHour = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a WattHour.
        /// </summary>
        /// <param name="Number">A numeric representation of a WattHour.</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParse(Decimal       Number,
                                       out WattHour  WattHour,
                                       Int32?        Exponent = null)
        {

            try
            {

                WattHour = new WattHour(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                WattHour = default;
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this WattHour.
        /// </summary>
        public WattHour Clone

            => new (Value);

        #endregion


        public static WattHour Zero
            => new (0);


        #region Operator overloading

        #region Operator == (WattHour1, WattHour2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (WattHour WattHour1,
                                           WattHour WattHour2)

            => WattHour1.Equals(WattHour2);

        #endregion

        #region Operator != (WattHour1, WattHour2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (WattHour WattHour1,
                                           WattHour WattHour2)

            => !WattHour1.Equals(WattHour2);

        #endregion

        #region Operator <  (WattHour1, WattHour2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (WattHour WattHour1,
                                          WattHour WattHour2)

            => WattHour1.CompareTo(WattHour2) < 0;

        #endregion

        #region Operator <= (WattHour1, WattHour2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (WattHour WattHour1,
                                           WattHour WattHour2)

            => WattHour1.CompareTo(WattHour2) <= 0;

        #endregion

        #region Operator >  (WattHour1, WattHour2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (WattHour WattHour1,
                                          WattHour WattHour2)

            => WattHour1.CompareTo(WattHour2) > 0;

        #endregion

        #region Operator >= (WattHour1, WattHour2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (WattHour WattHour1,
                                           WattHour WattHour2)

            => WattHour1.CompareTo(WattHour2) >= 0;

        #endregion

        #region Operator +  (WattHour1, WattHour2)

        /// <summary>
        /// Accumulates two WattHours.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        public static WattHour operator + (WattHour WattHour1,
                                       WattHour WattHour2)

            => WattHour.Parse(WattHour1.Value + WattHour2.Value);

        #endregion

        #region Operator -  (WattHour1, WattHour2)

        /// <summary>
        /// Substracts two WattHours.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        public static WattHour operator - (WattHour WattHour1,
                                       WattHour WattHour2)

            => WattHour.Parse(WattHour1.Value - WattHour2.Value);

        #endregion

        #endregion

        #region IComparable<WattHour> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two WattHours.
        /// </summary>
        /// <param name="Object">A WattHour to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is WattHour wattHour
                   ? CompareTo(wattHour)
                   : throw new ArgumentException("The given object is not a WattHour!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(WattHour)

        /// <summary>
        /// Compares two WattHours.
        /// </summary>
        /// <param name="WattHour">A WattHour to compare with.</param>
        public Int32 CompareTo(WattHour WattHour)

            => Value.CompareTo(WattHour.Value);

        #endregion

        #endregion

        #region IEquatable<WattHour> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two WattHours for equality.
        /// </summary>
        /// <param name="Object">A WattHour to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is WattHour wattHour &&
                   Equals(wattHour);

        #endregion

        #region Equals(WattHour)

        /// <summary>
        /// Compares two WattHours for equality.
        /// </summary>
        /// <param name="WattHour">A WattHour to compare with.</param>
        public Boolean Equals(WattHour WattHour)

            => Value.Equals(WattHour.Value);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Value} Wh";

        #endregion

    }

}
