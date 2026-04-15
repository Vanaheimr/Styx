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
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for BitPerSecond (bit/s) values.
    /// </summary>
    public static class BitPerSecondExtensions
    {

        #region Sum    (this BitPerSecondValues)

        /// <summary>
        /// The sum of the given enumeration of BitPerSecond values.
        /// </summary>
        /// <param name="BitPerSecondValues">An enumeration of BitPerSecond values.</param>
        public static BitPerSecond Sum(this IEnumerable<BitPerSecond> BitPerSecondValues)
        {

            var sum = BitPerSecond.Zero;

            foreach (var bitPerSecond in BitPerSecondValues)
                sum += bitPerSecond;

            return sum;

        }

        #endregion

        #region Avg    (this BitPerSecondValues)

        /// <summary>
        /// The average of the given enumeration of BitPerSecond values.
        /// </summary>
        /// <param name="BitPerSecondValues">An enumeration of BitPerSecond values.</param>
        public static BitPerSecond Avg(this IEnumerable<BitPerSecond> BitPerSecondValues)
        {

            var sum    = BitPerSecond.Zero;
            var count  = 0;

            foreach (var bitPerSecond in BitPerSecondValues)
            {
                sum += bitPerSecond;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this BitPerSecondValues)

        /// <summary>
        /// The standard deviation of the given enumeration of BitPerSecond values.
        /// </summary>
        /// <param name="BitPerSecondValues">An enumeration of BitPerSecond values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<BitPerSecond> StdDev(this IEnumerable<BitPerSecond>  BitPerSecondValues,
                                                  Boolean?                        IsSampleData   = null)
        {

            var stdDev = StdDev<BitPerSecond>.From(
                             BitPerSecondValues.Select(bitPerSecond => bitPerSecond.Value),
                             IsSampleData
                         );

            return new StdDev<BitPerSecond>(
                       BitPerSecond.FromBPS(stdDev.Mean),
                       BitPerSecond.FromBPS(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }

    //Note: MBit/s == MegaBytes per second is not the same as Mbit/s == Megabits per second!

    /// <summary>
    /// A BitPerSecond value.
    /// </summary>
    public readonly struct BitPerSecond : IMetrology<BitPerSecond>
    {

        #region Properties

        /// <summary>
        /// The value of the BitPerSecond.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the BitPerSecond.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as kbit/s.
        /// </summary>
        public Decimal  kbps
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The value as Mbit/s.
        /// </summary>
        public Decimal  Mbps
            => Value / 1000000m;

        /// <summary>
        /// The value as Gbit/s.
        /// </summary>
        public Decimal  Gbps
            => Value / 1000000000m;

        /// <summary>
        /// The value as Tbit/s.
        /// </summary>
        public Decimal  Tbps
            => Value / 1000000000000m;


        /// <summary>
        /// The zero value of BitPerSecond.
        /// </summary>
        public static readonly BitPerSecond Zero = new (0m);

        /// <summary>
        /// The additive identity of BitPerSecond.
        /// </summary>
        public static BitPerSecond AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new BitPerSecond based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of bit/s.</param>
        private BitPerSecond(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse        (Text)

        /// <summary>
        /// Parse the given string as BitPerSecond using invariant culture.
        /// Supports optional suffixes "bit/s", "kbit/s", "Mbit/s", "Gbit/s" and "Tbit/s".
        /// </summary>
        /// <param name="Text">A text representation of BitPerSecond.</param>
        public static BitPerSecond Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse        (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as BitPerSecond using the given format provider.
        /// Supports optional suffixes "bit/s", "kbit/s", "Mbit/s", "Gbit/s" and "Tbit/s".
        /// </summary>
        /// <param name="Text">A text representation of BitPerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static BitPerSecond Parse(String            Text,
                                         IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var BitPerSecond))
                return BitPerSecond;

            throw new FormatException($"Invalid text representation of BitPerSecond: '{Text}'!");

        }

        #endregion

        #region (static) Parse        (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as BitPerSecond using the given format provider.
        /// Supports optional suffixes "bit/s", "kbit/s", "Mbit/s", "Gbit/s" and "Tbit/s".
        /// </summary>
        /// <param name="Span">A text representation of BitPerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static BitPerSecond Parse(ReadOnlySpan<Char>  Span,
                                         IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var BitPerSecond))
                return BitPerSecond;

            throw new FormatException($"Invalid text representation of BitPerSecond: '{Span}'!");

        }

        #endregion

        #region (static) ParseBPS     (Text)

        /// <summary>
        /// Parse the given string as bit/s.
        /// </summary>
        /// <param name="Text">A text representation of bit/s.</param>
        public static BitPerSecond ParseBPS(String Text)
        {

            if (TryParseBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            throw new ArgumentException($"Invalid text representation of bit/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKBPS    (Text)

        /// <summary>
        /// Parse the given string as kbit/s.
        /// </summary>
        /// <param name="Text">A text representation of kbit/s.</param>
        public static BitPerSecond ParseKBPS(String Text)
        {

            if (TryParseKBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            throw new ArgumentException($"Invalid text representation of kbit/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseMBPS    (Text)

        /// <summary>
        /// Parse the given string as Mbit/s.
        /// </summary>
        /// <param name="Text">A text representation of Mbit/s.</param>
        public static BitPerSecond ParseMBPS(String Text)
        {

            if (TryParseMBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of Mbit/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseGBPS    (Text)

        /// <summary>
        /// Parse the given string as Gbit/s.
        /// </summary>
        /// <param name="Text">A text representation of Gbit/s.</param>
        public static BitPerSecond ParseGBPS(String Text)
        {

            if (TryParseGBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of Gbit/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseTBPS    (Text)

        /// <summary>
        /// Parse the given string as Tbit/s.
        /// </summary>
        /// <param name="Text">A text representation of Tbit/s.</param>
        public static BitPerSecond ParseTBPS(String Text)
        {

            if (TryParseTBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of Tbit/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) TryParse     (Text)

        /// <summary>
        /// Try to parse the given text as BitPerSecond with an optional unit suffix ("bit/s", "kbit/s", "Mbit/s", "Gbit/s" or "Tbit/s")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of BitPerSecond.</param>
        public static BitPerSecond? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParse     (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as BitPerSecond with an optional unit suffix ("bit/s", "kbit/s", "Mbit/s", "Gbit/s" or "Tbit/s")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of BitPerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static BitPerSecond? TryParse(String?           Text,
                                             IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseBPS  (Text)

        /// <summary>
        /// Try to parse the given text as bit/s.
        /// </summary>
        /// <param name="Text">A text representation of bit/s.</param>
        public static BitPerSecond? TryParseBPS(String? Text)
        {

            if (TryParseBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseKBPS (Text)

        /// <summary>
        /// Try to parse the given text as kbit/s.
        /// </summary>
        /// <param name="Text">A text representation of kbit/s.</param>
        public static BitPerSecond? TryParseKBPS(String? Text)
        {

            if (TryParseKBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseMBPS (Text)

        /// <summary>
        /// Try to parse the given text as Mbit/s.
        /// </summary>
        /// <param name="Text">A text representation of Mbit/s.</param>
        public static BitPerSecond? TryParseMBPS(String? Text)
        {

            if (TryParseMBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseGBPS (Text)

        /// <summary>
        /// Try to parse the given text as Gbit/s.
        /// </summary>
        /// <param name="Text">A text representation of Gbit/s.</param>
        public static BitPerSecond? TryParseGBPS(String? Text)
        {

            if (TryParseGBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseTBPS (Text)

        /// <summary>
        /// Try to parse the given text as Tbit/s.
        /// </summary>
        /// <param name="Text">A text representation of Tbit/s.</param>
        public static BitPerSecond? TryParseTBPS(String? Text)
        {

            if (TryParseTBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion


        #region (static) TryParse     (Text,                 out BitPerSecond)

        /// <summary>
        /// Try to parse the given string as BitPerSecond using invariant culture.
        /// Supports optional suffixes "bit/s", "kbit/s", "Mbit/s", "Gbit/s" and "Tbit/s".
        /// </summary>
        /// <param name="Text">A text representation of BitPerSecond.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?       Text,
                                       out                 BitPerSecond  BitPerSecond)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out BitPerSecond);

        #endregion

        #region (static) TryParse     (Text, FormatProvider, out BitPerSecond)

        /// <summary>
        /// Try to parse the given string as BitPerSecond using the given format provider.
        /// Supports optional suffixes "bit/s", "kbit/s", "Mbit/s", "Gbit/s" and "Tbit/s".
        /// </summary>
        /// <param name="Text">A text representation of BitPerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out BitPerSecond             BitPerSecond)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out BitPerSecond);

        #endregion

        #region (static) TryParse     (Span, FormatProvider, out BitPerSecond)

        /// <summary>
        /// Try to parse the given text span as BitPerSecond using the given format provider.
        /// Supports optional suffixes "bit/s", "kbit/s", "Mbit/s", "Gbit/s" and "Tbit/s".
        /// </summary>
        /// <param name="Span">A text representation of BitPerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out BitPerSecond    BitPerSecond)
        {

            BitPerSecond = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            //Note: "MBps" == MegaBytes per second is not the same as "Mbps" == Megabits per second!

            if      (Span.EndsWith("kbit/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^6].TrimEnd();
            }

            else if (Span.EndsWith("kb/s".  AsSpan(), StringComparison.Ordinal) ||
                     Span.EndsWith("kbps".  AsSpan(), StringComparison.Ordinal))
            {
                exponent  = 3;
                Span      = Span[..^4].TrimEnd();
            }


            else if (Span.EndsWith("Mbit/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 6;
                Span      = Span[..^6].TrimEnd();
            }

            else if (Span.EndsWith("Mb/s".  AsSpan(), StringComparison.Ordinal) ||
                     Span.EndsWith("Mbps".  AsSpan(), StringComparison.Ordinal))
            {
                exponent  = 6;
                Span      = Span[..^4].TrimEnd();
            }


            else if (Span.EndsWith("Gbit/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 9;
                Span      = Span[..^6].TrimEnd();
            }

            else if (Span.EndsWith("Gb/s".  AsSpan(), StringComparison.Ordinal) ||
                     Span.EndsWith("Gbps".  AsSpan(), StringComparison.Ordinal))
            {
                exponent  = 9;
                Span      = Span[..^4].TrimEnd();
            }


            else if (Span.EndsWith("Tbit/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 12;
                Span      = Span[..^6].TrimEnd();
            }

            else if (Span.EndsWith("Tb/s".  AsSpan(), StringComparison.Ordinal) ||
                     Span.EndsWith("Tbps".  AsSpan(), StringComparison.Ordinal))
            {
                exponent  = 12;
                Span      = Span[..^4].TrimEnd();
            }


            else if (Span.EndsWith("bit/s". AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^5].TrimEnd();
            }

            else if (Span.EndsWith("b/s".   AsSpan(), StringComparison.Ordinal) ||
                     Span.EndsWith("bps".   AsSpan(), StringComparison.Ordinal))
            {
                Span      = Span[..^3].TrimEnd();
            }


            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out BitPerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseBPS  (Text,                 out BitPerSecond)

        /// <summary>
        /// Parse the given string as bit/s.
        /// </summary>
        /// <param name="Text">A text representation of bit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        public static Boolean TryParseBPS([NotNullWhen(true)] String?       Text,
                                          out                 BitPerSecond  BitPerSecond)
        {

            BitPerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out BitPerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKBPS (Text,                 out BitPerSecond)

        /// <summary>
        /// Parse the given string as kbit/s.
        /// </summary>
        /// <param name="Text">A text representation of kbit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        public static Boolean TryParseKBPS([NotNullWhen(true)] String?       Text,
                                           out                 BitPerSecond  BitPerSecond)
        {

            BitPerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out BitPerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseMBPS (Text,                 out BitPerSecond)

        /// <summary>
        /// Parse the given string as Mbit/s.
        /// </summary>
        /// <param name="Text">A text representation of Mbit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        public static Boolean TryParseMBPS([NotNullWhen(true)] String?       Text,
                                           out                 BitPerSecond  BitPerSecond)
        {

            BitPerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 6, out BitPerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseGBPS (Text,                 out BitPerSecond)

        /// <summary>
        /// Parse the given string as Gbit/s.
        /// </summary>
        /// <param name="Text">A text representation of Gbit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        public static Boolean TryParseGBPS([NotNullWhen(true)] String?       Text,
                                           out                 BitPerSecond  BitPerSecond)
        {

            BitPerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 9, out BitPerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseTBPS (Text,                 out BitPerSecond)

        /// <summary>
        /// Parse the given string as Tbit/s.
        /// </summary>
        /// <param name="Text">A text representation of Tbit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        public static Boolean TryParseTBPS([NotNullWhen(true)] String?       Text,
                                           out                 BitPerSecond  BitPerSecond)
        {

            BitPerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 12, out BitPerSecond);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static BitPerSecond Create(Decimal  Number,
                                           Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var bitPerSecond))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return bitPerSecond;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Ampere)

        private static Boolean TryCreate(Decimal           Number,
                                         Int32             Exponent,
                                         out BitPerSecond  BitPerSecond)
        {

            BitPerSecond = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                BitPerSecond = Zero;
                return true;
            }

            try
            {
                BitPerSecond = new BitPerSecond(Number * MathHelpers.Pow10(Exponent));
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }

        }

        #endregion

        #region (static) FromBPS      (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into Bit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of Bit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond FromBPS<TNumber>(TNumber  Number,
                                                    Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) FromKBPS     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into kBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of kBit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond FromKBPS<TNumber>(TNumber  Number,
                                                     Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 3)
                   );

        #endregion

        #region (static) FromMBPS     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into MBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of MBit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond FromMBPS<TNumber>(TNumber  Number,
                                                     Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 6)
                   );

        #endregion

        #region (static) FromGBPS     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into GBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of GBit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond FromGBPS<TNumber>(TNumber  Number,
                                                     Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 9)
                   );

        #endregion

        #region (static) FromTBPS     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into TBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of TBit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond FromTBPS<TNumber>(TNumber  Number,
                                                     Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 12)
                   );

        #endregion


        #region (static) TryFromBPS   (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Bit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of Bit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond? TryFromBPS<TNumber>(TNumber  Number,
                                                        Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromBPS(Number, out var bitPerSecond, Exponent))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryFromKBPS  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of kBit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond? TryFromKBPS<TNumber>(TNumber  Number,
                                                         Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromKBPS(Number, out var bitPerSecond, Exponent))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryFromMBPS  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of MBit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond? TryFromMBPS<TNumber>(TNumber  Number,
                                                         Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromMBPS(Number, out var bitPerSecond, Exponent))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryFromGBPS  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into GBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of GBit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond? TryFromGBPS<TNumber>(TNumber  Number,
                                                         Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromGBPS(Number, out var bitPerSecond, Exponent))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryFromTBPS  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into TBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of TBit/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BitPerSecond? TryFromTBPS<TNumber>(TNumber  Number,
                                                         Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromTBPS(Number, out var bitPerSecond, Exponent))
                return bitPerSecond;

            return null;

        }

        #endregion


        #region (static) TryFromBPS   (Number, out BitPerSecond, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Bit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of Bit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromBPS<TNumber>(TNumber           Number,
                                                  out BitPerSecond  BitPerSecond,
                                                  Int32?            Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BitPerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BitPerSecond);
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

        }

        #endregion

        #region (static) TryFromKBPS  (Number, out BitPerSecond, Exponent = null)

        /// <summary>
        /// From the given number as kBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of kBit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKBPS<TNumber>(TNumber           Number,
                                                   out BitPerSecond  BitPerSecond,
                                                   Int32?            Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BitPerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BitPerSecond);
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

        }

        #endregion

        #region (static) TryFromMBPS  (Number, out BitPerSecond, Exponent = null)

        /// <summary>
        /// From the given number as MBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of MBit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMBPS<TNumber>(TNumber           Number,
                                                   out BitPerSecond  BitPerSecond,
                                                   Int32?            Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BitPerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 6, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BitPerSecond);
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

        }

        #endregion

        #region (static) TryFromGBPS  (Number, out BitPerSecond, Exponent = null)

        /// <summary>
        /// From the given number as GBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of GBit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromGBPS<TNumber>(TNumber           Number,
                                                   out BitPerSecond  BitPerSecond,
                                                   Int32?            Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BitPerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 9, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BitPerSecond);
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

        }

        #endregion

        #region (static) TryFromTBPS  (Number, out BitPerSecond, Exponent = null)

        /// <summary>
        /// From the given number as TBit/s.
        /// </summary>
        /// <param name="Number">A numeric representation of TBit/s.</param>
        /// <param name="BitPerSecond">The parsed BitPerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromTBPS<TNumber>(TNumber           Number,
                                                   out BitPerSecond  BitPerSecond,
                                                   Int32?            Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BitPerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 12, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BitPerSecond);
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (BitPerSecond1, BitPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitPerSecond1">A BitPerSecond.</param>
        /// <param name="BitPerSecond2">Another BitPerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BitPerSecond BitPerSecond1,
                                           BitPerSecond BitPerSecond2)

            => BitPerSecond1.Equals(BitPerSecond2);

        #endregion

        #region Operator != (BitPerSecond1, BitPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitPerSecond1">A BitPerSecond.</param>
        /// <param name="BitPerSecond2">Another BitPerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BitPerSecond BitPerSecond1,
                                           BitPerSecond BitPerSecond2)

            => !BitPerSecond1.Equals(BitPerSecond2);

        #endregion

        #region Operator <  (BitPerSecond1, BitPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitPerSecond1">A BitPerSecond.</param>
        /// <param name="BitPerSecond2">Another BitPerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BitPerSecond BitPerSecond1,
                                          BitPerSecond BitPerSecond2)

            => BitPerSecond1.CompareTo(BitPerSecond2) < 0;

        #endregion

        #region Operator <= (BitPerSecond1, BitPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitPerSecond1">A BitPerSecond.</param>
        /// <param name="BitPerSecond2">Another BitPerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BitPerSecond BitPerSecond1,
                                           BitPerSecond BitPerSecond2)

            => BitPerSecond1.CompareTo(BitPerSecond2) <= 0;

        #endregion

        #region Operator >  (BitPerSecond1, BitPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitPerSecond1">A BitPerSecond.</param>
        /// <param name="BitPerSecond2">Another BitPerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BitPerSecond BitPerSecond1,
                                          BitPerSecond BitPerSecond2)

            => BitPerSecond1.CompareTo(BitPerSecond2) > 0;

        #endregion

        #region Operator >= (BitPerSecond1, BitPerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BitPerSecond1">A BitPerSecond.</param>
        /// <param name="BitPerSecond2">Another BitPerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BitPerSecond BitPerSecond1,
                                           BitPerSecond BitPerSecond2)

            => BitPerSecond1.CompareTo(BitPerSecond2) >= 0;

        #endregion

        #region Operator +  (BitPerSecond1, BitPerSecond2)

        /// <summary>
        /// Accumulates two BitPerSecond.
        /// </summary>
        /// <param name="BitPerSecond1">A BitPerSecond.</param>
        /// <param name="BitPerSecond2">Another BitPerSecond.</param>
        public static BitPerSecond operator + (BitPerSecond BitPerSecond1,
                                               BitPerSecond BitPerSecond2)

            => new (BitPerSecond1.Value + BitPerSecond2.Value);

        #endregion

        #region Operator -  (BitPerSecond1, BitPerSecond2)

        /// <summary>
        /// Subtracts two BitPerSecond.
        /// </summary>
        /// <param name="BitPerSecond1">A BitPerSecond.</param>
        /// <param name="BitPerSecond2">Another BitPerSecond.</param>
        public static BitPerSecond operator - (BitPerSecond BitPerSecond1,
                                               BitPerSecond BitPerSecond2)

            => new (BitPerSecond1.Value - BitPerSecond2.Value);

        #endregion


        #region Operator *  (BitPerSecond,  Scalar)

        /// <summary>
        /// Multiplies a BitPerSecond with a scalar.
        /// </summary>
        /// <param name="BitPerSecond">A BitPerSecond value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static BitPerSecond operator * (BitPerSecond  BitPerSecond,
                                               Decimal       Scalar)

            => new (BitPerSecond.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,        BitPerSecond)

        /// <summary>
        /// Multiplies a scalar with a BitPerSecond.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="BitPerSecond">A BitPerSecond value.</param>
        public static BitPerSecond operator * (Decimal       Scalar,
                                               BitPerSecond  BitPerSecond)

            => new (Scalar * BitPerSecond.Value);

        #endregion

        #region Operator /  (BitPerSecond,  Scalar)

        /// <summary>
        /// Divides a BitPerSecond by a scalar.
        /// </summary>
        /// <param name="BitPerSecond">A BitPerSecond value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static BitPerSecond operator / (BitPerSecond  BitPerSecond,
                                               Decimal       Scalar)

            => new (BitPerSecond.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<BitPerSecond> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two BitPerSecond.
        /// </summary>
        /// <param name="Object">A BitPerSecond to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null                       => 1,
                   BitPerSecond bitPerSecond  => CompareTo(bitPerSecond),
                   _                          => throw new ArgumentException("The given object is not a BitPerSecond!", nameof(Object))
               };

        #endregion

        #region CompareTo(BitPerSecond)

        /// <summary>
        /// Compares two BitPerSecond.
        /// </summary>
        /// <param name="BitPerSecond">A BitPerSecond to compare with.</param>
        public Int32 CompareTo(BitPerSecond BitPerSecond)

            => Value.CompareTo(BitPerSecond.Value);

        #endregion

        #endregion

        #region IEquatable<BitPerSecond> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two BitPerSecond for equality.
        /// </summary>
        /// <param name="Object">A BitPerSecond to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BitPerSecond bps &&
                   Equals(bps);

        #endregion

        #region Equals(BitPerSecond)

        /// <summary>
        /// Compares two BitPerSecond for equality.
        /// </summary>
        /// <param name="BitPerSecond">A BitPerSecond to compare with.</param>
        public Boolean Equals(BitPerSecond BitPerSecond)

            => Value.Equals(BitPerSecond.Value);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion


        #region TryFormat(Destination, out CharsWritten, Format, FormatProvider)

        /// <summary>
        /// Try to format this Ampere into the given character span using the given format and culture-specific format provider.
        /// </summary>
        /// <param name="Destination">The destination span to write the formatted value.</param>
        /// <param name="CharsWritten">The number of characters written to the destination span.</param>
        /// <param name="Format">The format to use.</param>
        /// <param name="FormatProvider">The format provider to use.</param>
        public Boolean TryFormat(Span<Char>          Destination,
                                 out Int32           CharsWritten,
                                 ReadOnlySpan<Char>  Format,
                                 IFormatProvider?    FormatProvider)
        {

            //Note: "MBps" == MegaBytes per second is not the same as "Mbps" == Megabits per second!

            if (Format.IsEmpty ||
                Format.Equals("G".    AsSpan(), StringComparison.Ordinal) ||
                Format.Equals("bit/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " bit/s".AsSpan()
                       );
            }

            else if (Format.Equals("b/s".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " b/s".AsSpan()
                       );

            else if (Format.Equals("bps".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " bps".AsSpan()
                       );


            else if (Format.Equals("kbit/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kbit/s".AsSpan()
                       );

            else if (Format.Equals("kb/s".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           kbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kb/s".AsSpan()
                       );

            else if (Format.Equals("kbps".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           kbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kbps".AsSpan()
                       );


            else if (Format.Equals("Mbit/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           Mbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Mbit/s".AsSpan()
                       );

            else if (Format.Equals("Mb/s".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Mbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Mb/s".AsSpan()
                       );

            else if (Format.Equals("Mbps".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Mbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Mbps".AsSpan()
                       );


            else if (Format.Equals("Gbit/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           Gbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Gbit/s".AsSpan()
                       );

            else if (Format.Equals("Gb/s".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Gbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Gb/s".AsSpan()
                       );

            else if (Format.Equals("Gbps".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Gbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Gbps".AsSpan()
                       );


            else if (Format.Equals("Tbit/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           Tbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Tbit/s".AsSpan()
                       );

            else if (Format.Equals("Tb/s".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Tbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Tb/s".AsSpan()
                       );

            else if (Format.Equals("Tbps".AsSpan(),   StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Tbps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Tbps".AsSpan()
                       );


            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " bit/s".AsSpan()
                   );

        }

        #endregion

        #region (private static) TryFormatWithSuffix(Value, Destination, out CharsWritten, NumericFormat, FormatProvider, Suffix)

        private static Boolean TryFormatWithSuffix(Decimal             Value,
                                                   Span<Char>          Destination,
                                                   out Int32           CharsWritten,
                                                   ReadOnlySpan<Char>  NumericFormat,
                                                   IFormatProvider?    FormatProvider,
                                                   ReadOnlySpan<Char>  Suffix)
        {

            CharsWritten = 0;

            if (!Value.TryFormat(Destination,
                                 out var valueCharsWritten,
                                 NumericFormat,
                                 FormatProvider))
            {
                return false;
            }

            if (Destination.Length < valueCharsWritten + Suffix.Length)
                return false;

            Suffix.CopyTo(Destination[valueCharsWritten..]);

            CharsWritten = valueCharsWritten + Suffix.Length;
            return true;

        }

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

            Span<Char> buffer = stackalloc Char[64];

            if (TryFormat(buffer, out var charsWritten, Format.AsSpan(), FormatProvider))
                return new String(buffer[..charsWritten]);

            //Note: "MBps" == MegaBytes per second is not the same as "Mbps" == Megabits per second!

            if (String.IsNullOrEmpty(Format) ||
                String.Equals(Format, "G",     StringComparison.OrdinalIgnoreCase) ||
                String.Equals(Format, "bit/s", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} bit/s";
            }

            else if (String.Equals(Format, "b/s",    StringComparison.Ordinal))
                return $"{Value.ToString("G", FormatProvider)} b/s";

            else if (String.Equals(Format, "bps",    StringComparison.Ordinal))
                return $"{Value.ToString("G", FormatProvider)} bps";


            else if (String.Equals(Format, "kbit/s", StringComparison.OrdinalIgnoreCase))
                return $"{kbps.ToString("G", FormatProvider)} kbit/s";

            else if (String.Equals(Format, "kb/s",   StringComparison.Ordinal))
                return $"{kbps.ToString("G", FormatProvider)} kb/s";

            else if (String.Equals(Format, "kbps",   StringComparison.Ordinal))
                return $"{kbps.ToString("G", FormatProvider)} kbps";


            else if (String.Equals(Format, "Mbit/s", StringComparison.OrdinalIgnoreCase))
                return $"{Mbps.ToString("G", FormatProvider)} Mbit/s";

            else if (String.Equals(Format, "Mb/s",   StringComparison.Ordinal))
                return $"{Mbps.ToString("G", FormatProvider)} Mb/s";

            else if (String.Equals(Format, "Mbps",   StringComparison.Ordinal))
                return $"{Mbps.ToString("G", FormatProvider)} Mbps";


            else if (String.Equals(Format, "Gbit/s", StringComparison.OrdinalIgnoreCase))
                return $"{Gbps.ToString("G", FormatProvider)} Gbit/s";

            else if (String.Equals(Format, "Gb/s",   StringComparison.Ordinal))
                return $"{Gbps.ToString("G", FormatProvider)} Gb/s";

            else if (String.Equals(Format, "Gbps",   StringComparison.Ordinal))
                return $"{Gbps.ToString("G", FormatProvider)} Gbps";


            else if (String.Equals(Format, "Tbit/s", StringComparison.OrdinalIgnoreCase))
                return $"{Tbps.ToString("G", FormatProvider)} Tbit/s";

            else if (String.Equals(Format, "Tb/s",   StringComparison.Ordinal))
                return $"{Tbps.ToString("G", FormatProvider)} Tb/s";

            else if (String.Equals(Format, "Tbps",   StringComparison.Ordinal))
                return $"{Tbps.ToString("G", FormatProvider)} Tbps";


            return $"{Value.ToString(Format, FormatProvider)} bit/s";

        }

        #endregion

    }

}
