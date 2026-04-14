/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

    /// <summary>
    /// Extension methods for BitsPerSeconds.
    /// </summary>
    public static class BitsPerSecondExtensions
    {

        /// <summary>
        /// The sum of the given BitsPerSecond values.
        /// </summary>
        /// <param name="BitsPerSeconds">An enumeration of BitsPerSecond values.</param>
        public static BitsPerSecond Sum(this IEnumerable<BitsPerSecond> BitsPerSeconds)
        {

            var sum = BitsPerSecond.Zero;

            foreach (var bps in BitsPerSeconds)
                sum = sum + bps;

            return sum;

        }

    }


    /// <summary>
    /// A BitsPerSecond value.
    /// </summary>
    public readonly struct BitsPerSecond : IEquatable <BitsPerSecond>,
                                           IComparable<BitsPerSecond>,
                                           IComparable,
                                           IFormattable,
                                           IAdditionOperators   <BitsPerSecond, BitsPerSecond, BitsPerSecond>,
                                           ISubtractionOperators<BitsPerSecond, BitsPerSecond, BitsPerSecond>,
                                           IMultiplyOperators   <BitsPerSecond, Decimal,       BitsPerSecond>,
                                           IDivisionOperators   <BitsPerSecond, Decimal,       BitsPerSecond>
    {

        #region Properties

        /// <summary>
        /// The zero value of BitsPerSecond.
        /// </summary>
        public static readonly BitsPerSecond Zero = new (0m);

        /// <summary>
        /// The value of the BitsPerSeconds.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the BitsPerSeconds.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as KiloBitsPerSeconds.
        /// </summary>
        public Decimal  kBPS
            => Value / 1000;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The value as MegaBitsPerSeconds.
        /// </summary>
        public Decimal  MBPS
            => Value / 1000000;

        /// <summary>
        /// The value as GigaBitsPerSeconds.
        /// </summary>
        public Decimal  GBPS
            => Value / 1000000000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new BitsPerSecond based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of bits/sec.</param>
        private BitsPerSecond(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse        (Text)

        /// <summary>
        /// Parse the given string as bits/sec.
        /// </summary>
        /// <param name="Text">A text representation of bits/sec.</param>
        public static BitsPerSecond Parse(String Text)
        {

            if (TryParse(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of bits per second: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseBPS     (Text)

        /// <summary>
        /// Parse the given string as bits/sec.
        /// </summary>
        /// <param name="Text">A text representation of bits/sec.</param>
        public static BitsPerSecond ParseBPS(String Text)
        {

            if (TryParseBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of bits per second: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKBPS    (Text)

        /// <summary>
        /// Parse the given string as kBits/second.
        /// </summary>
        /// <param name="Text">A text representation of kBits/second.</param>
        public static BitsPerSecond ParseKBPS(String Text)
        {

            if (TryParseKBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of kilobits per second: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseMBPS    (Text)

        /// <summary>
        /// Parse the given string as MBits/second.
        /// </summary>
        /// <param name="Text">A text representation of MBits/second.</param>
        public static BitsPerSecond ParseMBPS(String Text)
        {

            if (TryParseMBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of megabits per second: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseGBPS    (Text)

        /// <summary>
        /// Parse the given string as GBits/second..
        /// </summary>
        /// <param name="Text">A text representation of GBits/second..</param>
        public static BitsPerSecond ParseGBPS(String Text)
        {

            if (TryParseGBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of gigabits per second: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseBPS     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as bits/sec.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond ParseBPS(Decimal  Number,
                                             Int32?   Exponent = null)
        {

            if (TryParseBPS(Number, out var bps, Exponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of bits per second: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as bits/sec.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond ParseBPS(Byte    Number,
                                             Int32?  Exponent = null)
        {

            if (TryParseBPS(Number, out var bps, Exponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of bits per second: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKBPS    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as kBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of kBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond ParseKBPS(Decimal  Number,
                                              Int32?   Exponent = null)
        {

            if (TryParseKBPS(Number, out var bps, Exponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of kilobits per second: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as kBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of kBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond ParseKBPS(Byte    Number,
                                              Int32?  Exponent = null)
        {

            if (TryParseKBPS(Number, out var bps, Exponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of kilobits per second: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseMBPS    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as MBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of MBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond ParseMBPS(Decimal  Number,
                                              Int32?   Exponent = null)
        {

            if (TryParseMBPS(Number, out var bps, Exponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of megabits per second: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as MBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of MBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond ParseMBPS(Byte    Number,
                                              Int32?  Exponent = null)
        {

            if (TryParseMBPS(Number, out var bps, Exponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of megabits per second: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseGBPS    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as GBits/second..
        /// </summary>
        /// <param name="Number">A numeric representation of GBits/second..</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond ParseGBPS(Decimal  Number,
                                              Int32?   Exponent = null)
        {

            if (TryParseGBPS(Number, out var bps, Exponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of gigabits per second: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as GBits/second..
        /// </summary>
        /// <param name="Number">A numeric representation of GBits/second..</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond ParseGBPS(Byte    Number,
                                              Int32?  Exponent = null)
        {

            if (TryParseGBPS(Number, out var bps, Exponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of gigabits per second: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) ParseBPS     (Number, StdDev, NumberExponent = null, StdDevExponent = null)

        /// <summary>
        /// Parse the given number as bits/sec.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="StdDev">The standard deviation of the value.</param>
        /// <param name="NumberExponent">An optional 10^exponent for the number.</param>
        /// <param name="StdDevExponent">An optional 10^exponent for the standard deviation.</param>
        public static StdDev<BitsPerSecond> ParseBPS(Decimal  Number,
                                                     Decimal  StdDev,
                                                     Int32?   NumberExponent = null,
                                                     Int32?   StdDevExponent = null)
        {

            if (TryParseBPS(Number, StdDev, out var bps, NumberExponent, StdDevExponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of bits per second with standard deviation: '{Number} {StdDev}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as bits/sec.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="StdDev">The standard deviation of the value.</param>
        /// <param name="NumberExponent">An optional 10^exponent for the number.</param>
        /// <param name="StdDevExponent">An optional 10^exponent for the standard deviation.</param>
        public static StdDev<BitsPerSecond> ParseBPS(Byte    Number,
                                                     Byte    StdDev,
                                                     Int32?  NumberExponent = null,
                                                     Int32?  StdDevExponent = null)
        {

            if (TryParseBPS(Number, StdDev, out var bps, NumberExponent, StdDevExponent))
                return bps;

            throw new ArgumentException($"Invalid numeric representation of bits per second with standard deviation: '{Number} {StdDev}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse     (Text)

        /// <summary>
        /// Try to parse the given text as bits/sec.
        /// </summary>
        /// <param name="Text">A text representation of bits/sec.</param>
        public static BitsPerSecond? TryParse(String Text)
        {

            if (TryParse(Text, out var bps))
                return bps;

            return null;

        }

        #endregion

        #region (static) TryParseBPS  (Text)

        /// <summary>
        /// Try to parse the given text as bits/sec.
        /// </summary>
        /// <param name="Text">A text representation of bits/sec.</param>
        public static BitsPerSecond? TryParseBPS(String Text)
        {

            if (TryParseBPS(Text, out var bps))
                return bps;

            return null;

        }

        #endregion

        #region (static) TryParseKBPS (Text)

        /// <summary>
        /// Try to parse the given text as kBits/second.
        /// </summary>
        /// <param name="Text">A text representation of kBits/second.</param>
        public static BitsPerSecond? TryParseKBPS(String Text)
        {

            if (TryParseKBPS(Text, out var bps))
                return bps;

            return null;

        }

        #endregion

        #region (static) TryParseMBPS (Text)

        /// <summary>
        /// Try to parse the given text as MBits/second.
        /// </summary>
        /// <param name="Text">A text representation of MBits/second.</param>
        public static BitsPerSecond? TryParseMBPS(String Text)
        {

            if (TryParseMBPS(Text, out var bps))
                return bps;

            return null;

        }

        #endregion

        #region (static) TryParseGBPS (Text)

        /// <summary>
        /// Try to parse the given text as GBits/second..
        /// </summary>
        /// <param name="Text">A text representation of GBits/second..</param>
        public static BitsPerSecond? TryParseGBPS(String Text)
        {

            if (TryParseGBPS(Text, out var bps))
                return bps;

            return null;

        }

        #endregion


        #region (static) TryParseBPS  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as bits/sec.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond? TryParseBPS(Decimal  Number,
                                                 Int32?   Exponent = null)
        {

            if (TryParseBPS(Number, out var bps, Exponent))
                return bps;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as bits/sec.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond? TryParseBPS(Byte    Number,
                                                 Int32?  Exponent = null)
        {

            if (TryParseBPS(Number, out var bps, Exponent))
                return bps;

            return null;

        }

        #endregion

        #region (static) TryParseKBPS (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as kBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of kBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond? TryParseKBPS(Decimal  Number,
                                                  Int32?   Exponent = null)
        {

            if (TryParseKBPS(Number, out var bps, Exponent))
                return bps;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as kBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of kBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond? TryParseKBPS(Byte    Number,
                                                  Int32?  Exponent = null)
        {

            if (TryParseKBPS(Number, out var bps, Exponent))
                return bps;

            return null;

        }

        #endregion

        #region (static) TryParseMBPS (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as MBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of MBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond? TryParseMBPS(Decimal  Number,
                                                  Int32?   Exponent = null)
        {

            if (TryParseMBPS(Number, out var bps, Exponent))
                return bps;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as MBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of MBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond? TryParseMBPS(Byte    Number,
                                                  Int32?  Exponent = null)
        {

            if (TryParseMBPS(Number, out var bps, Exponent))
                return bps;

            return null;

        }

        #endregion

        #region (static) TryParseGBPS (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as GBits/second..
        /// </summary>
        /// <param name="Number">A numeric representation of GBits/second..</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond? TryParseGBPS(Decimal  Number,
                                                  Int32?   Exponent = null)
        {

            if (TryParseGBPS(Number, out var bps, Exponent))
                return bps;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as GBits/second..
        /// </summary>
        /// <param name="Number">A numeric representation of GBits/second..</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitsPerSecond? TryParseGBPS(Byte    Number,
                                                  Int32?  Exponent = null)
        {

            if (TryParseGBPS(Number, out var bps, Exponent))
                return bps;

            return null;

        }

        #endregion


        #region (static) TryParse     (Text,   out BitsPerSecond)

        /// <summary>
        /// Parse the given string as bits/sec.
        /// </summary>
        /// <param name="Text">A text representation of bits/sec.</param>
        /// <param name="BitsPerSecond">The parsed bits/sec.</param>
        public static Boolean TryParse(String Text, out BitsPerSecond BitsPerSecond)
        {

            BitsPerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            Text = Text.Trim();

            var factor = 1m;

            if (Text.EndsWith("kbps",   StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000m;
                Text    = Text[..^4].TrimEnd();
            }
            if (Text.EndsWith("kBit/s", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000m;
                Text    = Text[..^6].TrimEnd();
            }

            if (Text.EndsWith("Mbps",   StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000000m;
                Text    = Text[..^4].TrimEnd();
            }
            if (Text.EndsWith("MBit/s", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000000m;
                Text    = Text[..^6].TrimEnd();
            }

            if (Text.EndsWith("Gbps",   StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000000000m;
                Text    = Text[..^4].TrimEnd();
            }
            if (Text.EndsWith("GBit/s", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000000000m;
                Text    = Text[..^6].TrimEnd();
            }

            if (Decimal.TryParse(Text,
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {

                BitsPerSecond = new BitsPerSecond(value * factor);

                return true;

            }

            return false;

        }

        #endregion

        #region (static) TryParseBPS  (Text,   out BitsPerSecond)

        /// <summary>
        /// Parse the given string as bits/sec.
        /// </summary>
        /// <param name="Text">A text representation of bits/sec.</param>
        /// <param name="BitsPerSecond">The parsed W.</param>
        public static Boolean TryParseBPS(String Text, out BitsPerSecond BitsPerSecond)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    BitsPerSecond = new BitsPerSecond(value);

                    return true;

                }

            }
            catch
            { }

            BitsPerSecond = default;
            return false;

        }

        #endregion

        #region (static) TryParseKBPS (Text,   out BitsPerSecond)

        /// <summary>
        /// Parse the given string as kBits/second.
        /// </summary>
        /// <param name="Text">A text representation of kBits/second.</param>
        /// <param name="BitsPerSecond">The parsed kBits/second.</param>
        public static Boolean TryParseKBPS(String Text, out BitsPerSecond BitsPerSecond)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    BitsPerSecond = new BitsPerSecond(1000 * value);

                    return true;

                }

            }
            catch
            { }

            BitsPerSecond = default;
            return false;

        }

        #endregion

        #region (static) TryParseMBPS (Text,   out BitsPerSecond)

        /// <summary>
        /// Parse the given string as MBits/second.
        /// </summary>
        /// <param name="Text">A text representation of MBits/second.</param>
        /// <param name="BitsPerSecond">The parsed MBits/second.</param>
        public static Boolean TryParseMBPS(String Text, out BitsPerSecond BitsPerSecond)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    BitsPerSecond = new BitsPerSecond(1000000 * value);

                    return true;

                }

            }
            catch
            { }

            BitsPerSecond = default;
            return false;

        }

        #endregion

        #region (static) TryParseGBPS (Text,   out BitsPerSecond)

        /// <summary>
        /// Parse the given string as GBits/second..
        /// </summary>
        /// <param name="Text">A text representation of GBits/second..</param>
        /// <param name="BitsPerSecond">The parsed GBits/second.</param>
        public static Boolean TryParseGBPS(String Text, out BitsPerSecond BitsPerSecond)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    BitsPerSecond = new BitsPerSecond(1000000000 * value);

                    return true;

                }

            }
            catch
            { }

            BitsPerSecond = default;
            return false;

        }

        #endregion


        #region (static) TryParseBPS  (Number, out BitsPerSecond, Exponent = null)

        /// <summary>
        /// Parse the given number as bits/sec.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="BitsPerSecond">The parsed bits/sec.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseBPS(Byte               Number,
                                          out BitsPerSecond  BitsPerSecond,
                                          Int32?             Exponent = null)
        {

            try
            {

                BitsPerSecond = new BitsPerSecond(Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                BitsPerSecond = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as bits/sec.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="BitsPerSecond">The parsed bits/sec.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseBPS(Decimal            Number,
                                          out BitsPerSecond  BitsPerSecond,
                                          Int32?             Exponent = null)
        {

            try
            {

                BitsPerSecond = new BitsPerSecond(Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                BitsPerSecond = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKBPS (Number, out BitsPerSecond, Exponent = null)

        /// <summary>
        /// Parse the given number as kBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of kBits/second.</param>
        /// <param name="BitsPerSecond">The parsed kBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKBPS(Byte               Number,
                                           out BitsPerSecond  BitsPerSecond,
                                           Int32?             Exponent = null)
        {

            try
            {

                BitsPerSecond = new BitsPerSecond(1000 * Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                BitsPerSecond = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as kBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of kBits/second.</param>
        /// <param name="BitsPerSecond">The parsed kBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKBPS(Decimal            Number,
                                           out BitsPerSecond  BitsPerSecond,
                                           Int32?             Exponent = null)
        {

            try
            {

                BitsPerSecond = new BitsPerSecond(1000 * Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                BitsPerSecond = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseMBPS (Number, out BitsPerSecond, Exponent = null)

        /// <summary>
        /// Parse the given number as MBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of MBits/second.</param>
        /// <param name="BitsPerSecond">The parsed MBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseMBPS(Byte               Number,
                                           out BitsPerSecond  BitsPerSecond,
                                           Int32?             Exponent = null)
        {

            try
            {

                BitsPerSecond = new BitsPerSecond(1000 * Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                BitsPerSecond = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as MBits/second.
        /// </summary>
        /// <param name="Number">A numeric representation of MBits/second.</param>
        /// <param name="BitsPerSecond">The parsed MBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseMBPS(Decimal            Number,
                                           out BitsPerSecond  BitsPerSecond,
                                           Int32?             Exponent = null)
        {

            try
            {

                BitsPerSecond = new BitsPerSecond(1000 * Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                BitsPerSecond = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseGBPS (Number, out BitsPerSecond, Exponent = null)

        /// <summary>
        /// Parse the given number as GBits/second..
        /// </summary>
        /// <param name="Number">A numeric representation of GBits/second..</param>
        /// <param name="BitsPerSecond">The parsed GBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseGBPS(Byte               Number,
                                           out BitsPerSecond  BitsPerSecond,
                                           Int32?             Exponent = null)
        {

            try
            {

                BitsPerSecond = new BitsPerSecond(1000 * Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                BitsPerSecond = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as GBits/second..
        /// </summary>
        /// <param name="Number">A numeric representation of GBits/second..</param>
        /// <param name="BitsPerSecond">The parsed GBits/second.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseGBPS(Decimal            Number,
                                           out BitsPerSecond  BitsPerSecond,
                                           Int32?             Exponent = null)
        {

            try
            {

                BitsPerSecond = new BitsPerSecond(1000 * Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                BitsPerSecond = default;
                return false;
            }

        }

        #endregion


        #region (static) TryParseBPS  (Number, StdDev, out BitsPerSecond, NumberExponent = null, StdDevExponent = null)

        /// <summary>
        /// Parse the given number as bits/sec with standard deviation.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="StdDev">The standard deviation of the value.</param>
        /// <param name="BitsPerSecond">The parsed bits/sec with standard deviation.</param>
        /// <param name="NumberExponent">An optional 10^exponent for the number.</param>
        /// <param name="StdDevExponent">An optional 10^exponent for the standard deviation.</param>
        public static Boolean TryParseBPS(Byte                       Number,
                                          Byte                       StdDev,
                                          out StdDev<BitsPerSecond>  BitsPerSecond,
                                          Int32?                     NumberExponent = null,
                                          Int32?                     StdDevExponent = null)
        {

            try
            {
                if (TryParseBPS(Number, out var number, NumberExponent) &&
                    TryParseBPS(StdDev, out var stddev, StdDevExponent))
                {

                    BitsPerSecond = new StdDev<BitsPerSecond>(number, stddev);

                    return true;

                }
            }
            catch
            { }

            BitsPerSecond = default;
            return false;

        }


        /// <summary>
        /// Parse the given number as bits/sec with standard deviation.
        /// </summary>
        /// <param name="Number">A numeric representation of bits/sec.</param>
        /// <param name="StdDev">The standard deviation of the value.</param>
        /// <param name="BitsPerSecond">The parsed bits/sec with standard deviation.</param>
        /// <param name="NumberExponent">An optional 10^exponent for the number.</param>
        /// <param name="StdDevExponent">An optional 10^exponent for the standard deviation.</param>
        public static Boolean TryParseBPS(Decimal                    Number,
                                          Decimal                    StdDev,
                                          out StdDev<BitsPerSecond>  BitsPerSecond,
                                          Int32?                     NumberExponent = null,
                                          Int32?                     StdDevExponent = null)
        {

            try
            {
                if (TryParseBPS(Number, out var number, NumberExponent) &&
                    TryParseBPS(StdDev, out var stddev, StdDevExponent))
                {

                    BitsPerSecond = new StdDev<BitsPerSecond>(number, stddev);

                    return true;

                }
            }
            catch
            { }

            BitsPerSecond = default;
            return false;

        }

        #endregion


        #region Operator overloading

        #region Operator == (BitsPerSecond1, BitsPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitsPerSecond1">A bits/sec.</param>
        /// <param name="BitsPerSecond2">Another bits/sec.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BitsPerSecond BitsPerSecond1,
                                           BitsPerSecond BitsPerSecond2)

            => BitsPerSecond1.Equals(BitsPerSecond2);

        #endregion

        #region Operator != (BitsPerSecond1, BitsPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitsPerSecond1">A bits/sec.</param>
        /// <param name="BitsPerSecond2">Another bits/sec.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BitsPerSecond BitsPerSecond1,
                                           BitsPerSecond BitsPerSecond2)

            => !BitsPerSecond1.Equals(BitsPerSecond2);

        #endregion

        #region Operator <  (BitsPerSecond1, BitsPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitsPerSecond1">A bits/sec.</param>
        /// <param name="BitsPerSecond2">Another bits/sec.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BitsPerSecond BitsPerSecond1,
                                          BitsPerSecond BitsPerSecond2)

            => BitsPerSecond1.CompareTo(BitsPerSecond2) < 0;

        #endregion

        #region Operator <= (BitsPerSecond1, BitsPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitsPerSecond1">A bits/sec.</param>
        /// <param name="BitsPerSecond2">Another bits/sec.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BitsPerSecond BitsPerSecond1,
                                           BitsPerSecond BitsPerSecond2)

            => BitsPerSecond1.CompareTo(BitsPerSecond2) <= 0;

        #endregion

        #region Operator >  (BitsPerSecond1, BitsPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitsPerSecond1">A bits/sec.</param>
        /// <param name="BitsPerSecond2">Another bits/sec.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BitsPerSecond BitsPerSecond1,
                                          BitsPerSecond BitsPerSecond2)

            => BitsPerSecond1.CompareTo(BitsPerSecond2) > 0;

        #endregion

        #region Operator >= (BitsPerSecond1, BitsPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitsPerSecond1">A bits/sec.</param>
        /// <param name="BitsPerSecond2">Another bits/sec.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BitsPerSecond BitsPerSecond1,
                                           BitsPerSecond BitsPerSecond2)

            => BitsPerSecond1.CompareTo(BitsPerSecond2) >= 0;

        #endregion

        #region Operator +  (BitsPerSecond1, BitsPerSecond2)

        /// <summary>
        /// Accumulates two BitsPerSeconds.
        /// </summary>
        /// <param name="BitsPerSecond1">A bits/sec.</param>
        /// <param name="BitsPerSecond2">Another bits/sec.</param>
        public static BitsPerSecond operator + (BitsPerSecond BitsPerSecond1,
                                                BitsPerSecond BitsPerSecond2)

            => new (BitsPerSecond1.Value + BitsPerSecond2.Value);

        #endregion

        #region Operator -  (BitsPerSecond1, BitsPerSecond2)

        /// <summary>
        /// Substracts two BitsPerSeconds.
        /// </summary>
        /// <param name="BitsPerSecond1">A bits/sec.</param>
        /// <param name="BitsPerSecond2">Another bits/sec.</param>
        public static BitsPerSecond operator - (BitsPerSecond BitsPerSecond1,
                                                BitsPerSecond BitsPerSecond2)

            => new (BitsPerSecond1.Value - BitsPerSecond2.Value);

        #endregion


        #region Operator *  (BitsPerSecond,  Scalar)

        /// <summary>
        /// Multiplies a BitsPerSecond with a scalar.
        /// </summary>
        /// <param name="BitsPerSecond">A BitsPerSecond value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static BitsPerSecond operator * (BitsPerSecond  BitsPerSecond,
                                                Decimal        Scalar)

            => new (BitsPerSecond.Value * Scalar);

        #endregion

        #region Operator /  (BitsPerSecond,  Scalar)

        /// <summary>
        /// Divides a BitsPerSecond by a scalar.
        /// </summary>
        /// <param name="BitsPerSecond">A BitsPerSecond value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static BitsPerSecond operator / (BitsPerSecond  BitsPerSecond,
                                                Decimal        Scalar)

            => new (BitsPerSecond.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<BitsPerSecond> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two BitsPerSeconds.
        /// </summary>
        /// <param name="Object">A BitsPerSecond to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BitsPerSecond bps
                   ? CompareTo(bps)
                   : throw new ArgumentException("The given object is not bits per second!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BitsPerSecond)

        /// <summary>
        /// Compares two BitsPerSeconds.
        /// </summary>
        /// <param name="BitsPerSecond">A BitsPerSecond to compare with.</param>
        public Int32 CompareTo(BitsPerSecond BitsPerSecond)

            => Value.CompareTo(BitsPerSecond.Value);

        #endregion

        #endregion

        #region IEquatable<BitsPerSecond> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two BitsPerSeconds for equality.
        /// </summary>
        /// <param name="Object">A BitsPerSecond to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BitsPerSecond bps &&
                   Equals(bps);

        #endregion

        #region Equals(BitsPerSecond)

        /// <summary>
        /// Compares two BitsPerSeconds for equality.
        /// </summary>
        /// <param name="BitsPerSecond">A BitsPerSecond to compare with.</param>
        public Boolean Equals(BitsPerSecond BitsPerSecond)

            => Value.Equals(BitsPerSecond.Value);

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

            => ToString(
                   null,
                   CultureInfo.InvariantCulture
               );


        /// <summary>
        /// Return a text representation of this object using the given
        /// format and culture-specific format provider.
        /// </summary>
        public String ToString(String?           Format,
                               IFormatProvider?  FormatProvider)
        {

            if (String.IsNullOrEmpty(Format) ||
                Format.Equals("G",    StringComparison.OrdinalIgnoreCase) ||
                Format.Equals("bps",  StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} bits/sec";
            }

            if (Format.Equals("kbps", StringComparison.OrdinalIgnoreCase))
            {
                return $"{(Value / 1000m).ToString("G", FormatProvider)} kbits/sec";
            }

            return $"{Value.ToString(Format, FormatProvider)} bits/sec";

        }

        #endregion

    }

}
