/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>WattHours
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

using System.Numerics;
using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class WattHourExtensions
    {
        public static WattHour Sum(this IEnumerable<WattHour> Source)
        {

            var sum = WattHour.Zero;

            foreach (var value in Source)
                sum = sum + value;

            return sum;

        }

    }


    /// <summary>
    /// A WattHour (Wh) value.
    /// </summary>
    public readonly struct WattHour : IEquatable <WattHour>,
                                      IComparable<WattHour>,
                                      IComparable,
                                      IAdditionOperators   <WattHour, WattHour, WattHour>,
                                      ISubtractionOperators<WattHour, WattHour, WattHour>,
                                      IMultiplyOperators   <WattHour, Decimal,  WattHour>,
                                      IDivisionOperators   <WattHour, Decimal,  WattHour>
    {

        #region Properties

        /// <summary>
        /// The zero value of the WattHour.
        /// </summary>
        public static readonly WattHour Zero = new (0m);

        /// <summary>
        /// The value of the WattHour.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the WattHour.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as KiloWattHours.
        /// </summary>
        public Decimal  kWh
            => Value / 1000;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The value as MegaWattHours.
        /// </summary>
        public Decimal  MWh
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


        #region (static) Parse       (Text)

        /// <summary>
        /// Parse the given string as a Wh.
        /// </summary>
        /// <param name="Text">A text representation of a Wh.</param>
        public static WattHour Parse(String Text)
        {

            if (TryParse(Text, out var wattHour))
                return wattHour;

            throw new ArgumentException($"Invalid text representation of a Wh: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseWh     (Text)

        /// <summary>
        /// Parse the given string as a Wh.
        /// </summary>
        /// <param name="Text">A text representation of a Wh.</param>
        public static WattHour ParseWh(String Text)
        {

            if (TryParseWh(Text, out var wattHour))
                return wattHour;

            throw new ArgumentException($"Invalid text representation of a Wh: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKWh    (Text)

        /// <summary>
        /// Parse the given string as a kWh.
        /// </summary>
        /// <param name="Text">A text representation of a kWh.</param>
        public static WattHour ParseKWh(String Text)
        {

            if (TryParseKWh(Text, out var wattHour))
                return wattHour;

            throw new ArgumentException($"Invalid text representation of a kWh: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseMWh    (Text)

        /// <summary>
        /// Parse the given string as a MWh.
        /// </summary>
        /// <param name="Text">A text representation of a MWh.</param>
        public static WattHour ParseMWh(String Text)
        {

            if (TryParseMWh(Text, out var wattHour))
                return wattHour;

            throw new ArgumentException($"Invalid text representation of a MWh: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseWh     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Wh.
        /// </summary>
        /// <param name="Number">A numeric representation of a Wh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour ParseWh(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryParseWh(Number, out var wattHour, Exponent))
                return wattHour;

            throw new ArgumentException($"Invalid numeric representation of a Wh: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Wh.
        /// </summary>
        /// <param name="Number">A numeric representation of a Wh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour ParseWh(Byte    Number,
                                       Int32?  Exponent = null)
        {

            if (TryParseWh(Number, out var wattHour, Exponent))
                return wattHour;

            throw new ArgumentException($"Invalid numeric representation of a Wh: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKWh    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a kWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a kWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour ParseKWh(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseKWh(Number, out var wattHour, Exponent))
                return wattHour;

            throw new ArgumentException($"Invalid numeric representation of a kWh: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a kWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a kWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour ParseKWh(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseKWh(Number, out var wattHour, Exponent))
                return wattHour;

            throw new ArgumentException($"Invalid numeric representation of a kWh: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseMWh    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a MWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a MWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour ParseMWh(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseMWh(Number, out var wattHour, Exponent))
                return wattHour;

            throw new ArgumentException($"Invalid numeric representation of a MWh: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a MWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a MWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour ParseMWh(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseMWh(Number, out var wattHour, Exponent))
                return wattHour;

            throw new ArgumentException($"Invalid numeric representation of a MWh: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse    (Text)

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

        #region (static) TryParseWh  (Text)

        /// <summary>
        /// Try to parse the given text as a Wh.
        /// </summary>
        /// <param name="Text">A text representation of a Wh.</param>
        public static WattHour? TryParseWh(String Text)
        {

            if (TryParseWh(Text, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParseKWh (Text)

        /// <summary>
        /// Try to parse the given text as a kWh.
        /// </summary>
        /// <param name="Text">A text representation of a kWh.</param>
        public static WattHour? TryParseKWh(String Text)
        {

            if (TryParseKWh(Text, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParseMWh (Text)

        /// <summary>
        /// Try to parse the given text as a MWh.
        /// </summary>
        /// <param name="Text">A text representation of a MWh.</param>
        public static WattHour? TryParseMWh(String Text)
        {

            if (TryParseMWh(Text, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion


        #region (static) TryParseWh  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Wh.
        /// </summary>
        /// <param name="Number">A numeric representation of a Wh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryParseWh(Decimal  Number,
                                           Int32?   Exponent = null)
        {

            if (TryParseWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Wh.
        /// </summary>
        /// <param name="Number">A numeric representation of a Wh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryParseWh(Byte    Number,
                                           Int32?  Exponent = null)
        {

            if (TryParseWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParseKWh (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a kWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a kWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryParseKWh(Decimal  Number,
                                            Int32?   Exponent = null)
        {

            if (TryParseKWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a kWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a kWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryParseKWh(Byte    Number,
                                            Int32?  Exponent = null)
        {

            if (TryParseKWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParseMWh (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a MWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a MWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryParseMWh(Decimal  Number,
                                            Int32?   Exponent = null)
        {

            if (TryParseMWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a MWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a MWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryParseMWh(Byte    Number,
                                            Int32?  Exponent = null)
        {

            if (TryParseMWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }

        #endregion


        #region (static) TryParse    (Text,   out WattHour)

        /// <summary>
        /// Parse the given string as a WattHour.
        /// </summary>
        /// <param name="Text">A text representation of a WattHour.</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        public static Boolean TryParse(String Text, out WattHour WattHour)
        {

            WattHour = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            Text = Text.Trim();

            var factor = 1m;

            if      (Text.EndsWith("kWh", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000m;
                Text    = Text[..^3].TrimEnd();
            }

            if      (Text.EndsWith("MWh", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000000m;
                Text    = Text[..^3].TrimEnd();
            }

            if      (Text.EndsWith("GWh", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000000m;
                Text    = Text[..^3].TrimEnd();
            }

            else if (Text.EndsWith("Wh",  StringComparison.OrdinalIgnoreCase))
            {
                Text    = Text[..^2].TrimEnd();
            }

            if (Decimal.TryParse(Text,
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {

                WattHour = new WattHour(value * factor);

                return true;

            }

            return false;

        }

        #endregion

        #region (static) TryParseWh  (Text,   out WattHour)

        /// <summary>
        /// Parse the given string as a Wh.
        /// </summary>
        /// <param name="Text">A text representation of a Wh.</param>
        /// <param name="WattHour">The parsed Wh.</param>
        public static Boolean TryParseWh(String Text, out WattHour WattHour)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    WattHour = new WattHour(value);

                    return true;

                }

            }
            catch
            { }

            WattHour = default;
            return false;

        }

        #endregion

        #region (static) TryParseKWh (Text,   out WattHour)

        /// <summary>
        /// Parse the given string as a kWh.
        /// </summary>
        /// <param name="Text">A text representation of a kWh.</param>
        /// <param name="WattHour">The parsed Wh.</param>
        public static Boolean TryParseKWh(String Text, out WattHour WattHour)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    WattHour = new WattHour(1000 * value);

                    return true;

                }

            }
            catch
            { }

            WattHour = default;
            return false;

        }

        #endregion

        #region (static) TryParseMWh (Text,   out WattHour)

        /// <summary>
        /// Parse the given string as a MWh.
        /// </summary>
        /// <param name="Text">A text representation of a MWh.</param>
        /// <param name="WattHour">The parsed Wh.</param>
        public static Boolean TryParseMWh(String Text, out WattHour WattHour)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    WattHour = new WattHour(1000000 * value);

                    return true;

                }

            }
            catch
            { }

            WattHour = default;
            return false;

        }

        #endregion


        #region (static) TryParseWh  (Number, out WattHour, Exponent = null)

        /// <summary>
        /// Parse the given number as a Wh.
        /// </summary>
        /// <param name="Number">A numeric representation of a Wh.</param>
        /// <param name="WattHour">The parsed Wh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseWh(Byte          Number,
                                         out WattHour  WattHour,
                                         Int32?        Exponent = null)
        {

            try
            {

                WattHour = new WattHour(Number * MathHelpers.Pow10(Exponent ?? 0));

                return true;

            }
            catch
            {
                WattHour = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Wh.
        /// </summary>
        /// <param name="Number">A numeric representation of a Wh.</param>
        /// <param name="WattHour">The parsed Wh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseWh(Decimal       Number,
                                         out WattHour  WattHour,
                                         Int32?        Exponent = null)
        {

            try
            {

                WattHour = new WattHour(Number * MathHelpers.Pow10(Exponent ?? 0));

                return true;

            }
            catch
            {
                WattHour = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKWh (Number, out WattHour, Exponent = null)

        /// <summary>
        /// Parse the given number as a kWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a kWh.</param>
        /// <param name="WattHour">The parsed kWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKWh(Byte          Number,
                                          out WattHour  WattHour,
                                          Int32?        Exponent = null)
        {

            try
            {

                WattHour = new WattHour(1000 * Number * MathHelpers.Pow10(Exponent ?? 0));

                return true;

            }
            catch
            {
                WattHour = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a kWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a kWh.</param>
        /// <param name="WattHour">The parsed kWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKWh(Decimal       Number,
                                          out WattHour  WattHour,
                                          Int32?        Exponent = null)
        {

            try
            {

                WattHour = new WattHour(1000 * Number * MathHelpers.Pow10(Exponent ?? 0));

                return true;

            }
            catch
            {
                WattHour = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseMWh (Number, out WattHour, Exponent = null)

        /// <summary>
        /// Parse the given number as a MWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a MWh.</param>
        /// <param name="WattHour">The parsed MWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseMWh(Byte          Number,
                                          out WattHour  WattHour,
                                          Int32?        Exponent = null)
        {

            try
            {

                WattHour = new WattHour(1000000 * Number * MathHelpers.Pow10(Exponent ?? 0));

                return true;

            }
            catch
            {
                WattHour = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a MWh.
        /// </summary>
        /// <param name="Number">A numeric representation of a MWh.</param>
        /// <param name="WattHour">The parsed MWh.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseMWh(Decimal       Number,
                                          out WattHour  WattHour,
                                          Int32?        Exponent = null)
        {

            try
            {

                WattHour = new WattHour(1000000 * Number * MathHelpers.Pow10(Exponent ?? 0));

                return true;

            }
            catch
            {
                WattHour = default;
                return false;
            }

        }

        #endregion


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

            => new (WattHour1.Value + WattHour2.Value);

        #endregion

        #region Operator -  (WattHour1, WattHour2)

        /// <summary>
        /// Substracts two WattHours.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        public static WattHour operator - (WattHour WattHour1,
                                           WattHour WattHour2)

            => new (WattHour1.Value - WattHour2.Value);

        #endregion


        #region Operator *  (WattHour,  Scalar)

        /// <summary>
        /// Multiplies a WattHour with a scalar.
        /// </summary>
        /// <param name="WattHour">A WattHour value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static WattHour operator * (WattHour  WattHour,
                                           Decimal   Scalar)

            => new (WattHour.Value * Scalar);

        #endregion

        #region Operator /  (WattHour,  Scalar)

        /// <summary>
        /// Divides a WattHour with a scalar.
        /// </summary>
        /// <param name="WattHour">A WattHour value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static WattHour operator / (WattHour  WattHour,
                                           Decimal   Scalar)

            => new (WattHour.Value / Scalar);

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

            => $"{Value.ToString(CultureInfo.InvariantCulture)} Wh";

        #endregion

    }

}
