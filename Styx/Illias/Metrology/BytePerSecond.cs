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
    /// Extension methods for BytePerSecond (Byte/s) values.
    /// </summary>
    public static class BytePerSecondExtensions
    {

        #region Sum    (this BytePerSecondValues)

        /// <summary>
        /// The sum of the given enumeration of BytePerSecond values.
        /// </summary>
        /// <param name="BytePerSecondValues">An enumeration of BytePerSecond values.</param>
        public static BytePerSecond Sum(this IEnumerable<BytePerSecond> BytePerSecondValues)
        {

            var sum = BytePerSecond.Zero;

            foreach (var bitPerSecond in BytePerSecondValues)
                sum += bitPerSecond;

            return sum;

        }

        #endregion

        #region Avg    (this BytePerSecondValues)

        /// <summary>
        /// The average of the given enumeration of BytePerSecond values.
        /// </summary>
        /// <param name="BytePerSecondValues">An enumeration of BytePerSecond values.</param>
        public static BytePerSecond Avg(this IEnumerable<BytePerSecond> BytePerSecondValues)
        {

            var sum    = BytePerSecond.Zero;
            var count  = 0;

            foreach (var bitPerSecond in BytePerSecondValues)
            {
                sum += bitPerSecond;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this BytePerSecondValues)

        /// <summary>
        /// The standard deviation of the given enumeration of BytePerSecond values.
        /// </summary>
        /// <param name="BytePerSecondValues">An enumeration of BytePerSecond values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<BytePerSecond> StdDev(this IEnumerable<BytePerSecond>  BytePerSecondValues,
                                                  Boolean?                        IsSampleData   = null)
        {

            var stdDev = StdDev<BytePerSecond>.From(
                             BytePerSecondValues.Select(bitPerSecond => bitPerSecond.Value),
                             IsSampleData
                         );

            return new StdDev<BytePerSecond>(
                       BytePerSecond.FromBPS(stdDev.Mean),
                       BytePerSecond.FromBPS(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }

    //Note: MBit/s == MegaBytes per second is not the same as Mbit/s == Megabits per second!

    /// <summary>
    /// A BytePerSecond value.
    /// </summary>
    public readonly struct BytePerSecond : IMetrology<BytePerSecond>
    {

        #region Properties

        /// <summary>
        /// The value of the BytePerSecond.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the BytePerSecond.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as kByte/s.
        /// </summary>
        public Decimal  kBps
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The value as MByte/s.
        /// </summary>
        public Decimal  MBps
            => Value / 1000000m;

        /// <summary>
        /// The value as GByte/s.
        /// </summary>
        public Decimal  GBps
            => Value / 1000000000m;

        /// <summary>
        /// The value as TByte/s.
        /// </summary>
        public Decimal  TBps
            => Value / 1000000000000m;


        /// <summary>
        /// The zero value of BytePerSecond.
        /// </summary>
        public static readonly BytePerSecond Zero = new (0m);

        /// <summary>
        /// The additive identity of BytePerSecond.
        /// </summary>
        public static BytePerSecond AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new BytePerSecond based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of Byte/s.</param>
        private BytePerSecond(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse        (Text)

        /// <summary>
        /// Parse the given string as BytePerSecond using invariant culture.
        /// Supports optional suffixes "Byte/s", "kByte/s", "MByte/s", "GByte/s" and "TByte/s".
        /// </summary>
        /// <param name="Text">A text representation of BytePerSecond.</param>
        public static BytePerSecond Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse        (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as BytePerSecond using the given format provider.
        /// Supports optional suffixes "Byte/s", "kByte/s", "MByte/s", "GByte/s" and "TByte/s".
        /// </summary>
        /// <param name="Text">A text representation of BytePerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static BytePerSecond Parse(String            Text,
                                          IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var BytePerSecond))
                return BytePerSecond;

            throw new FormatException($"Invalid text representation of BytePerSecond: '{Text}'!");

        }

        #endregion

        #region (static) Parse        (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as BytePerSecond using the given format provider.
        /// Supports optional suffixes "Byte/s", "kByte/s", "MByte/s", "GByte/s" and "TByte/s".
        /// </summary>
        /// <param name="Span">A text representation of BytePerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static BytePerSecond Parse(ReadOnlySpan<Char>  Span,
                                          IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var BytePerSecond))
                return BytePerSecond;

            throw new FormatException($"Invalid text representation of BytePerSecond: '{Span}'!");

        }

        #endregion

        #region (static) ParseBPS     (Text)

        /// <summary>
        /// Parse the given string as Byte/s.
        /// </summary>
        /// <param name="Text">A text representation of Byte/s.</param>
        public static BytePerSecond ParseBPS(String Text)
        {

            if (TryParseBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            throw new ArgumentException($"Invalid text representation of Byte/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKBPS    (Text)

        /// <summary>
        /// Parse the given string as kByte/s.
        /// </summary>
        /// <param name="Text">A text representation of kByte/s.</param>
        public static BytePerSecond ParseKBPS(String Text)
        {

            if (TryParseKBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            throw new ArgumentException($"Invalid text representation of kByte/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseMBPS    (Text)

        /// <summary>
        /// Parse the given string as MByte/s.
        /// </summary>
        /// <param name="Text">A text representation of MByte/s.</param>
        public static BytePerSecond ParseMBPS(String Text)
        {

            if (TryParseMBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of MByte/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseGBPS    (Text)

        /// <summary>
        /// Parse the given string as GByte/s.
        /// </summary>
        /// <param name="Text">A text representation of GByte/s.</param>
        public static BytePerSecond ParseGBPS(String Text)
        {

            if (TryParseGBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of GByte/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseTBPS    (Text)

        /// <summary>
        /// Parse the given string as TByte/s.
        /// </summary>
        /// <param name="Text">A text representation of TByte/s.</param>
        public static BytePerSecond ParseTBPS(String Text)
        {

            if (TryParseTBPS(Text, out var bps))
                return bps;

            throw new ArgumentException($"Invalid text representation of TByte/s: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) TryParse     (Text)

        /// <summary>
        /// Try to parse the given text as BytePerSecond with an optional unit suffix ("Byte/s", "kByte/s", "MByte/s", "GByte/s" or "TByte/s")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of BytePerSecond.</param>
        public static BytePerSecond? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParse     (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as BytePerSecond with an optional unit suffix ("Byte/s", "kByte/s", "MByte/s", "GByte/s" or "TByte/s")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of BytePerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static BytePerSecond? TryParse(String?           Text,
                                              IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseBPS  (Text)

        /// <summary>
        /// Try to parse the given text as Byte/s.
        /// </summary>
        /// <param name="Text">A text representation of Byte/s.</param>
        public static BytePerSecond? TryParseBPS(String? Text)
        {

            if (TryParseBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseKBPS (Text)

        /// <summary>
        /// Try to parse the given text as kByte/s.
        /// </summary>
        /// <param name="Text">A text representation of kByte/s.</param>
        public static BytePerSecond? TryParseKBPS(String? Text)
        {

            if (TryParseKBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseMBPS (Text)

        /// <summary>
        /// Try to parse the given text as MByte/s.
        /// </summary>
        /// <param name="Text">A text representation of MByte/s.</param>
        public static BytePerSecond? TryParseMBPS(String? Text)
        {

            if (TryParseMBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseGBPS (Text)

        /// <summary>
        /// Try to parse the given text as GByte/s.
        /// </summary>
        /// <param name="Text">A text representation of GByte/s.</param>
        public static BytePerSecond? TryParseGBPS(String? Text)
        {

            if (TryParseGBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion

        #region (static) TryParseTBPS (Text)

        /// <summary>
        /// Try to parse the given text as TByte/s.
        /// </summary>
        /// <param name="Text">A text representation of TByte/s.</param>
        public static BytePerSecond? TryParseTBPS(String? Text)
        {

            if (TryParseTBPS(Text, out var bitPerSecond))
                return bitPerSecond;

            return null;

        }

        #endregion


        #region (static) TryParse     (Text,                 out BytePerSecond)

        /// <summary>
        /// Try to parse the given string as BytePerSecond using invariant culture.
        /// Supports optional suffixes "Byte/s", "kByte/s", "MByte/s", "GByte/s" and "TByte/s".
        /// </summary>
        /// <param name="Text">A text representation of BytePerSecond.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?       Text,
                                       out                 BytePerSecond  BytePerSecond)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out BytePerSecond);

        #endregion

        #region (static) TryParse     (Text, FormatProvider, out BytePerSecond)

        /// <summary>
        /// Try to parse the given string as BytePerSecond using the given format provider.
        /// Supports optional suffixes "Byte/s", "kByte/s", "MByte/s", "GByte/s" and "TByte/s".
        /// </summary>
        /// <param name="Text">A text representation of BytePerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out BytePerSecond            BytePerSecond)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out BytePerSecond);

        #endregion

        #region (static) TryParse     (Span, FormatProvider, out BytePerSecond)

        /// <summary>
        /// Try to parse the given text span as BytePerSecond using the given format provider.
        /// Supports optional suffixes "Byte/s", "kByte/s", "MByte/s", "GByte/s" and "TByte/s".
        /// </summary>
        /// <param name="Span">A text representation of BytePerSecond.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out BytePerSecond   BytePerSecond)
        {

            BytePerSecond = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            //Note: "MBps" == MegaBytes per second is not the same as "Mbps" == Megabits per second!

            if      (Span.EndsWith("kByte/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^7].TrimEnd();
            }

            else if (Span.EndsWith("kB/s".   AsSpan(), StringComparison.Ordinal) ||
                     Span.EndsWith("kBps".   AsSpan(), StringComparison.Ordinal))
            {
                exponent  = 3;
                Span      = Span[..^4].TrimEnd();
            }


            else if (Span.EndsWith("MByte/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 6;
                Span      = Span[..^7].TrimEnd();
            }

            else if (Span.EndsWith("MB/s".   AsSpan(), StringComparison.Ordinal) ||
                     Span.EndsWith("MBps".   AsSpan(), StringComparison.Ordinal))
            {
                exponent  = 6;
                Span      = Span[..^4].TrimEnd();
            }


            else if (Span.EndsWith("GByte/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 9;
                Span      = Span[..^7].TrimEnd();
            }

            else if (Span.EndsWith("GB/s".   AsSpan(), StringComparison.Ordinal) ||
                     Span.EndsWith("GBps".   AsSpan(), StringComparison.Ordinal))
            {
                exponent  = 9;
                Span      = Span[..^4].TrimEnd();
            }


            else if (Span.EndsWith("TByte/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 12;
                Span      = Span[..^7].TrimEnd();
            }

            else if (Span.EndsWith("TB/s".   AsSpan(), StringComparison.Ordinal) ||
                     Span.EndsWith("TBps".   AsSpan(), StringComparison.Ordinal))
            {
                exponent  = 12;
                Span      = Span[..^4].TrimEnd();
            }


            else if (Span.EndsWith("Byte/s". AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^6].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out BytePerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseBPS  (Text,                 out BytePerSecond)

        /// <summary>
        /// Parse the given string as Byte/s.
        /// </summary>
        /// <param name="Text">A text representation of Byte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        public static Boolean TryParseBPS([NotNullWhen(true)] String?        Text,
                                          out                 BytePerSecond  BytePerSecond)
        {

            BytePerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out BytePerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKBPS (Text,                 out BytePerSecond)

        /// <summary>
        /// Parse the given string as kByte/s.
        /// </summary>
        /// <param name="Text">A text representation of kByte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        public static Boolean TryParseKBPS([NotNullWhen(true)] String?        Text,
                                           out                 BytePerSecond  BytePerSecond)
        {

            BytePerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out BytePerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseMBPS (Text,                 out BytePerSecond)

        /// <summary>
        /// Parse the given string as MByte/s.
        /// </summary>
        /// <param name="Text">A text representation of MByte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        public static Boolean TryParseMBPS([NotNullWhen(true)] String?        Text,
                                           out                 BytePerSecond  BytePerSecond)
        {

            BytePerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 6, out BytePerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseGBPS (Text,                 out BytePerSecond)

        /// <summary>
        /// Parse the given string as GByte/s.
        /// </summary>
        /// <param name="Text">A text representation of GByte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        public static Boolean TryParseGBPS([NotNullWhen(true)] String?        Text,
                                           out                 BytePerSecond  BytePerSecond)
        {

            BytePerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 9, out BytePerSecond);
            }

            return false;

        }

        #endregion

        #region (static) TryParseTBPS (Text,                 out BytePerSecond)

        /// <summary>
        /// Parse the given string as TByte/s.
        /// </summary>
        /// <param name="Text">A text representation of TByte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        public static Boolean TryParseTBPS([NotNullWhen(true)] String?        Text,
                                           out                 BytePerSecond  BytePerSecond)
        {

            BytePerSecond = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 12, out BytePerSecond);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static BytePerSecond Create(Decimal  Number,
                                            Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var bytePerSecond))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return bytePerSecond;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Ampere)

        private static Boolean TryCreate(Decimal            Number,
                                         Int32              Exponent,
                                         out BytePerSecond  BytePerSecond)
        {

            BytePerSecond = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                BytePerSecond = Zero;
                return true;
            }

            try
            {
                BytePerSecond = new BytePerSecond(Number * MathHelpers.Pow10(Exponent));
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
        /// Convert the given number into Byte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of Byte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond FromBPS<TNumber>(TNumber  Number,
                                                     Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) FromKBPS     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into kByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of kByte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond FromKBPS<TNumber>(TNumber  Number,
                                                      Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 3)
                   );

        #endregion

        #region (static) FromMBPS     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into MByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of MByte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond FromMBPS<TNumber>(TNumber  Number,
                                                      Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 6)
                   );

        #endregion

        #region (static) FromGBPS     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into GByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of GByte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond FromGBPS<TNumber>(TNumber  Number,
                                                      Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 9)
                   );

        #endregion

        #region (static) FromTBPS     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into TByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of TByte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond FromTBPS<TNumber>(TNumber  Number,
                                                      Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 12)
                   );

        #endregion


        #region (static) TryFromBPS   (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Byte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of Byte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond? TryFromBPS<TNumber>(TNumber  Number,
                                                         Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromBPS(Number, out var bytePerSecond, Exponent))
                return bytePerSecond;

            return null;

        }

        #endregion

        #region (static) TryFromKBPS  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of kByte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond? TryFromKBPS<TNumber>(TNumber  Number,
                                                          Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromKBPS(Number, out var bytePerSecond, Exponent))
                return bytePerSecond;

            return null;

        }

        #endregion

        #region (static) TryFromMBPS  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of MByte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond? TryFromMBPS<TNumber>(TNumber  Number,
                                                          Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromMBPS(Number, out var bytePerSecond, Exponent))
                return bytePerSecond;

            return null;

        }

        #endregion

        #region (static) TryFromGBPS  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into GByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of GByte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond? TryFromGBPS<TNumber>(TNumber  Number,
                                                          Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromGBPS(Number, out var bytePerSecond, Exponent))
                return bytePerSecond;

            return null;

        }

        #endregion

        #region (static) TryFromTBPS  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into TByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of TByte/s.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static BytePerSecond? TryFromTBPS<TNumber>(TNumber  Number,
                                                          Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromTBPS(Number, out var bytePerSecond, Exponent))
                return bytePerSecond;

            return null;

        }

        #endregion


        #region (static) TryFromBPS   (Number, out BytePerSecond, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Byte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of Byte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromBPS<TNumber>(TNumber            Number,
                                                  out BytePerSecond  BytePerSecond,
                                                  Int32?             Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BytePerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BytePerSecond);
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

        #region (static) TryFromKBPS  (Number, out BytePerSecond, Exponent = null)

        /// <summary>
        /// From the given number as kByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of kByte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKBPS<TNumber>(TNumber            Number,
                                                   out BytePerSecond  BytePerSecond,
                                                   Int32?             Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BytePerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BytePerSecond);
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

        #region (static) TryFromMBPS  (Number, out BytePerSecond, Exponent = null)

        /// <summary>
        /// From the given number as MByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of MByte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMBPS<TNumber>(TNumber            Number,
                                                   out BytePerSecond  BytePerSecond,
                                                   Int32?             Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BytePerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 6, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BytePerSecond);
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

        #region (static) TryFromGBPS  (Number, out BytePerSecond, Exponent = null)

        /// <summary>
        /// From the given number as GByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of GByte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromGBPS<TNumber>(TNumber            Number,
                                                   out BytePerSecond  BytePerSecond,
                                                   Int32?             Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BytePerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 9, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BytePerSecond);
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

        #region (static) TryFromTBPS  (Number, out BytePerSecond, Exponent = null)

        /// <summary>
        /// From the given number as TByte/s.
        /// </summary>
        /// <param name="Number">A numeric representation of TByte/s.</param>
        /// <param name="BytePerSecond">The parsed BytePerSecond.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromTBPS<TNumber>(TNumber            Number,
                                                   out BytePerSecond  BytePerSecond,
                                                   Int32?             Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            BytePerSecond = default;

            if (!MathHelpers.TryAddExponent(Exponent, 12, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out BytePerSecond);
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

        #region Operator == (BytePerSecond1, BytePerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BytePerSecond1">A BytePerSecond.</param>
        /// <param name="BytePerSecond2">Another BytePerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BytePerSecond BytePerSecond1,
                                           BytePerSecond BytePerSecond2)

            => BytePerSecond1.Equals(BytePerSecond2);

        #endregion

        #region Operator != (BytePerSecond1, BytePerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BytePerSecond1">A BytePerSecond.</param>
        /// <param name="BytePerSecond2">Another BytePerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BytePerSecond BytePerSecond1,
                                           BytePerSecond BytePerSecond2)

            => !BytePerSecond1.Equals(BytePerSecond2);

        #endregion

        #region Operator <  (BytePerSecond1, BytePerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BytePerSecond1">A BytePerSecond.</param>
        /// <param name="BytePerSecond2">Another BytePerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BytePerSecond BytePerSecond1,
                                          BytePerSecond BytePerSecond2)

            => BytePerSecond1.CompareTo(BytePerSecond2) < 0;

        #endregion

        #region Operator <= (BytePerSecond1, BytePerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BytePerSecond1">A BytePerSecond.</param>
        /// <param name="BytePerSecond2">Another BytePerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BytePerSecond BytePerSecond1,
                                           BytePerSecond BytePerSecond2)

            => BytePerSecond1.CompareTo(BytePerSecond2) <= 0;

        #endregion

        #region Operator >  (BytePerSecond1, BytePerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BytePerSecond1">A BytePerSecond.</param>
        /// <param name="BytePerSecond2">Another BytePerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BytePerSecond BytePerSecond1,
                                          BytePerSecond BytePerSecond2)

            => BytePerSecond1.CompareTo(BytePerSecond2) > 0;

        #endregion

        #region Operator >= (BytePerSecond1, BytePerSecond2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BytePerSecond1">A BytePerSecond.</param>
        /// <param name="BytePerSecond2">Another BytePerSecond.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BytePerSecond BytePerSecond1,
                                           BytePerSecond BytePerSecond2)

            => BytePerSecond1.CompareTo(BytePerSecond2) >= 0;

        #endregion

        #region Operator +  (BytePerSecond1, BytePerSecond2)

        /// <summary>
        /// Accumulates two BytePerSecond.
        /// </summary>
        /// <param name="BytePerSecond1">A BytePerSecond.</param>
        /// <param name="BytePerSecond2">Another BytePerSecond.</param>
        public static BytePerSecond operator + (BytePerSecond BytePerSecond1,
                                               BytePerSecond BytePerSecond2)

            => new (BytePerSecond1.Value + BytePerSecond2.Value);

        #endregion

        #region Operator -  (BytePerSecond1, BytePerSecond2)

        /// <summary>
        /// Subtracts two BytePerSecond.
        /// </summary>
        /// <param name="BytePerSecond1">A BytePerSecond.</param>
        /// <param name="BytePerSecond2">Another BytePerSecond.</param>
        public static BytePerSecond operator - (BytePerSecond BytePerSecond1,
                                               BytePerSecond BytePerSecond2)

            => new (BytePerSecond1.Value - BytePerSecond2.Value);

        #endregion


        #region Operator *  (BytePerSecond,  Scalar)

        /// <summary>
        /// Multiplies a BytePerSecond with a scalar.
        /// </summary>
        /// <param name="BytePerSecond">A BytePerSecond value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static BytePerSecond operator * (BytePerSecond  BytePerSecond,
                                               Decimal       Scalar)

            => new (BytePerSecond.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,        BytePerSecond)

        /// <summary>
        /// Multiplies a scalar with a BytePerSecond.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="BytePerSecond">A BytePerSecond value.</param>
        public static BytePerSecond operator * (Decimal       Scalar,
                                               BytePerSecond  BytePerSecond)

            => new (Scalar * BytePerSecond.Value);

        #endregion

        #region Operator /  (BytePerSecond,  Scalar)

        /// <summary>
        /// Divides a BytePerSecond by a scalar.
        /// </summary>
        /// <param name="BytePerSecond">A BytePerSecond value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static BytePerSecond operator / (BytePerSecond  BytePerSecond,
                                               Decimal       Scalar)

            => new (BytePerSecond.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<BytePerSecond> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two BytePerSecond.
        /// </summary>
        /// <param name="Object">A BytePerSecond to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null                       => 1,
                   BytePerSecond bitPerSecond  => CompareTo(bitPerSecond),
                   _                          => throw new ArgumentException("The given object is not a BytePerSecond!", nameof(Object))
               };

        #endregion

        #region CompareTo(BytePerSecond)

        /// <summary>
        /// Compares two BytePerSecond.
        /// </summary>
        /// <param name="BytePerSecond">A BytePerSecond to compare with.</param>
        public Int32 CompareTo(BytePerSecond BytePerSecond)

            => Value.CompareTo(BytePerSecond.Value);

        #endregion

        #endregion

        #region IEquatable<BytePerSecond> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two BytePerSecond for equality.
        /// </summary>
        /// <param name="Object">A BytePerSecond to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BytePerSecond bps &&
                   Equals(bps);

        #endregion

        #region Equals(BytePerSecond)

        /// <summary>
        /// Compares two BytePerSecond for equality.
        /// </summary>
        /// <param name="BytePerSecond">A BytePerSecond to compare with.</param>
        public Boolean Equals(BytePerSecond BytePerSecond)

            => Value.Equals(BytePerSecond.Value);

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
                Format.Equals("G".     AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                Format.Equals("Byte/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Byte/s".AsSpan()
                       );
            }

            else if (Format.Equals("B/s".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " B/s".AsSpan()
                       );

            else if (Format.Equals("Bps".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Bps".AsSpan()
                       );


            else if (Format.Equals("kByte/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kByte/s".AsSpan()
                       );

            else if (Format.Equals("kB/s".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           kBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kB/s".AsSpan()
                       );

            else if (Format.Equals("kBps".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           kBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kBps".AsSpan()
                       );


            else if (Format.Equals("MByte/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           MBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " MByte/s".AsSpan()
                       );

            else if (Format.Equals("MB/s".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           MBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " MB/s".AsSpan()
                       );

            else if (Format.Equals("MBps".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           MBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " MBps".AsSpan()
                       );


            else if (Format.Equals("GByte/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           GBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " GByte/s".AsSpan()
                       );

            else if (Format.Equals("GB/s".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           GBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " GB/s".AsSpan()
                       );

            else if (Format.Equals("GBps".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           GBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " GBps".AsSpan()
                       );


            else if (Format.Equals("TByte/s".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           TBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " TByte/s".AsSpan()
                       );

            else if (Format.Equals("TB/s".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           TBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " TB/s".AsSpan()
                       );

            else if (Format.Equals("TBps".   AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           TBps,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " TBps".AsSpan()
                       );


            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " Byte/s".AsSpan()
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
                String.Equals(Format, "Byte/s", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} Byte/s";
            }

            else if (String.Equals(Format, "B/s",    StringComparison.Ordinal))
                return $"{Value.ToString("G", FormatProvider)} B/s";

            else if (String.Equals(Format, "Bps",    StringComparison.Ordinal))
                return $"{Value.ToString("G", FormatProvider)} Bps";


            else if (String.Equals(Format, "kBit/s", StringComparison.OrdinalIgnoreCase))
                return $"{kBps.ToString("G", FormatProvider)} kBit/s";

            else if (String.Equals(Format, "kB/s",   StringComparison.Ordinal))
                return $"{kBps.ToString("G", FormatProvider)} kB/s";

            else if (String.Equals(Format, "kBps",   StringComparison.Ordinal))
                return $"{kBps.ToString("G", FormatProvider)} kBps";


            else if (String.Equals(Format, "MBit/s", StringComparison.OrdinalIgnoreCase))
                return $"{MBps.ToString("G", FormatProvider)} MBit/s";

            else if (String.Equals(Format, "MB/s",   StringComparison.Ordinal))
                return $"{MBps.ToString("G", FormatProvider)} MB/s";

            else if (String.Equals(Format, "MBps",   StringComparison.Ordinal))
                return $"{MBps.ToString("G", FormatProvider)} MBps";


            else if (String.Equals(Format, "GBit/s", StringComparison.OrdinalIgnoreCase))
                return $"{GBps.ToString("G", FormatProvider)} GBit/s";

            else if (String.Equals(Format, "GB/s",   StringComparison.Ordinal))
                return $"{GBps.ToString("G", FormatProvider)} GB/s";

            else if (String.Equals(Format, "GBps",   StringComparison.Ordinal))
                return $"{GBps.ToString("G", FormatProvider)} GBps";


            else if (String.Equals(Format, "TBit/s", StringComparison.OrdinalIgnoreCase))
                return $"{TBps.ToString("G", FormatProvider)} TBit/s";

            else if (String.Equals(Format, "TB/s",   StringComparison.Ordinal))
                return $"{TBps.ToString("G", FormatProvider)} TB/s";

            else if (String.Equals(Format, "TBps",   StringComparison.Ordinal))
                return $"{TBps.ToString("G", FormatProvider)} TBps";


            return $"{Value.ToString(Format, FormatProvider)} Byte/s";

        }

        #endregion

    }

}
