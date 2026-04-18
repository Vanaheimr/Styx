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
    /// Extension methods for Henry (H) values.
    /// </summary>
    public static class HenryExtensions
    {

        #region Sum    (this HenryValues)

        /// <summary>
        /// The sum of the given enumeration of Henry values.
        /// </summary>
        /// <param name="HenryValues">An enumeration of Henry values.</param>
        public static Henry Sum(this IEnumerable<Henry> HenryValues)
        {

            var sum = Henry.Zero;

            foreach (var henry in HenryValues)
                sum += henry;

            return sum;

        }

        #endregion

        #region Avg    (this HenryValues)

        /// <summary>
        /// The average of the given enumeration of Henry values.
        /// </summary>
        /// <param name="HenryValues">An enumeration of Henry values.</param>
        public static Henry Avg(this IEnumerable<Henry> HenryValues)
        {

            var sum    = Henry.Zero;
            var count  = 0;

            foreach (var henry in HenryValues)
            {
                sum += henry;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this HenryValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Henry values.
        /// </summary>
        /// <param name="HenryValues">An enumeration of Henry values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Henry> StdDev(this IEnumerable<Henry>  HenryValues,
                                           Boolean?                 IsSampleData   = null)
        {

            var stdDev = StdDev<Henry>.From(
                             HenryValues.Select(henry => henry.Value),
                             IsSampleData
                         );

            return new StdDev<Henry>(
                       Henry.FromH(stdDev.Mean),
                       Henry.FromH(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Henry value (H), the SI unit of inductance.
    /// </summary>
    public readonly struct Henry : IMetrology<Henry>,
                                   IDivisionOperators<Henry, Henry, Decimal>
    {

        #region Properties

        /// <summary>
        /// The value of the Henry.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Henry.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// The value as kiloHenry.
        /// </summary>
        public Decimal  kH
            => Value / 1000m;

        /// <summary>
        /// The value as milliHenry.
        /// </summary>
        public Decimal  mH
            => Value * 1000m;

        /// <summary>
        /// The value as microHenry.
        /// </summary>
        public Decimal  µH
            => Value * 1000000m;

        /// <summary>
        /// The value as nanoHenry.
        /// </summary>
        public Decimal  nH
            => Value * 1000000000m;

        /// <summary>
        /// The value as pikohenry.
        /// </summary>
        public Decimal  pH
            => Value * 1000000000000m;

#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The zero value of the Henry.
        /// </summary>
        public static readonly Henry Zero = new (0m);

        /// <summary>
        /// The additive identity of Henry.
        /// </summary>
        public static Henry AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Henry (H) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of henry (H).</param>
        private Henry(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as henry using invariant culture.
        /// Supports optional suffixes "H", "µH", "nH" and "pH".
        /// </summary>
        /// <param name="Text">A text representation of henry.</param>
        public static Henry Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse      (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as henry using the given format provider.
        /// Supports optional suffixes "H", "µH", "nH" and "pH".
        /// </summary>
        /// <param name="Text">A text representation of henry.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Henry Parse(String            Text,
                                  IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var henry))
                return henry;

            throw new FormatException($"Invalid text representation of henry: '{Text}'!");

        }

        #endregion

        #region (static) Parse      (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as henry using the given format provider.
        /// Supports optional suffixes "H", "µH", "nH" and "pH".
        /// </summary>
        /// <param name="Span">A text representation of henry.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Henry Parse(ReadOnlySpan<Char>  Span,
                                  IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var henry))
                return henry;

            throw new FormatException($"Invalid text representation of henry: '{Span}'!");

        }

        #endregion

        #region (static) ParseKH    (Text)

        /// <summary>
        /// Parse the given string as KiloHenry (kH).
        /// </summary>
        /// <param name="Text">A text representation of KiloHenry (kH).</param>
        public static Henry ParseKH(String Text)
        {

            if (TryParseKH(Text, out var henry))
                return henry;

            throw new FormatException($"Invalid text representation of KiloHenry (kH): '{Text}'!");

        }

        #endregion

        #region (static) ParseH     (Text)

        /// <summary>
        /// Parse the given string as Henry (H).
        /// </summary>
        /// <param name="Text">A text representation of Henry (H).</param>
        public static Henry ParseH(String Text)
        {

            if (TryParseH(Text, out var henry))
                return henry;

            throw new FormatException($"Invalid text representation of Henry (H): '{Text}'!");

        }

        #endregion

        #region (static) ParseMH    (Text)

        /// <summary>
        /// Parse the given string as MilliHenry (mH).
        /// </summary>
        /// <param name="Text">A text representation of MilliHenry (mH).</param>
        public static Henry ParseMH(String Text)
        {

            if (TryParseMH(Text, out var henry))
                return henry;

            throw new FormatException($"Invalid text representation of MilliHenry (mH): '{Text}'!");

        }

        #endregion

        #region (static) ParseµH    (Text)

        /// <summary>
        /// Parse the given string as MicroHenry (µH).
        /// </summary>
        /// <param name="Text">A text representation of MicroHenry (µH).</param>
        public static Henry ParseµH(String Text)
        {

            if (TryParseµH(Text, out var henry))
                return henry;

            throw new FormatException($"Invalid text representation of MicroHenry (µH): '{Text}'!");

        }

        #endregion

        #region (static) ParseNH    (Text)

        /// <summary>
        /// Parse the given string as NanoHenry (nH).
        /// </summary>
        /// <param name="Text">A text representation of NanoHenry (nH).</param>
        public static Henry ParseNH(String Text)
        {

            if (TryParseNH(Text, out var henry))
                return henry;

            throw new FormatException($"Invalid text representation of NanoHenry (nH): '{Text}'!");

        }

        #endregion

        #region (static) ParsePH    (Text)

        /// <summary>
        /// Parse the given string as PikoHenry (pH).
        /// </summary>
        /// <param name="Text">A text representation of PikoHenry (pH).</param>
        public static Henry ParsePH(String Text)
        {

            if (TryParsePH(Text, out var henry))
                return henry;

            throw new FormatException($"Invalid text representation of PikoHenry (pH): '{Text}'!");

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as henry with an optional unit suffix ("H", "µH", "nH" or "pH")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of henry.</param>
        public static Henry? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var henry))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as henry with an optional unit suffix ("H", "µH", "nH" or "pH")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of henry.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Henry? TryParse(String?           Text,
                                      IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var henry))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryParseKH (Text)

        /// <summary>
        /// Try to parse the given text as KiloHenry (kH).
        /// </summary>
        /// <param name="Text">A text representation of KiloHenry (kH).</param>
        public static Henry? TryParseKH(String? Text)
        {

            if (TryParseKH(Text, out var henry))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryParseH  (Text)

        /// <summary>
        /// Try to parse the given text as Henry (H).
        /// </summary>
        /// <param name="Text">A text representation of Henry (H).</param>
        public static Henry? TryParseH(String? Text)
        {

            if (TryParseH(Text, out var henry))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryParseMH (Text)

        /// <summary>
        /// Try to parse the given text as MilliHenry (mH).
        /// </summary>
        /// <param name="Text">A text representation of MilliHenry (mH).</param>
        public static Henry? TryParseMH(String? Text)
        {

            if (TryParseMH(Text, out var henry))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryParseµH (Text)

        /// <summary>
        /// Try to parse the given text as MicroHenry (µH).
        /// </summary>
        /// <param name="Text">A text representation of MicroHenry (µH).</param>
        public static Henry? TryParseµH(String? Text)
        {

            if (TryParseµH(Text, out var henry))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryParseNH (Text)

        /// <summary>
        /// Try to parse the given text as NanoHenry (nH).
        /// </summary>
        /// <param name="Text">A text representation of NanoHenry (nH).</param>
        public static Henry? TryParseNH(String? Text)
        {

            if (TryParseNH(Text, out var henry))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryParsePH (Text)

        /// <summary>
        /// Try to parse the given text as PikoHenry (pH).
        /// </summary>
        /// <param name="Text">A text representation of PikoHenry (pH).</param>
        public static Henry? TryParsePH(String? Text)
        {

            if (TryParsePH(Text, out var henry))
                return henry;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,                 out Henry)

        /// <summary>
        /// Try to parse the given string as henry using invariant culture.
        /// Supports optional suffixes "H", "µH", "nH" and "pH".
        /// </summary>
        /// <param name="Text">A text representation of henry.</param>
        /// <param name="Henry">The parsed Henry.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Henry    Henry)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Henry);

        #endregion

        #region (static) TryParse   (Text, FormatProvider, out Henry)

        /// <summary>
        /// Try to parse the given string as henry using the given format provider.
        /// Supports optional suffixes "H", "µH", "nH" and "pH".
        /// </summary>
        /// <param name="Text">A text representation of henry.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Henry">The parsed Henry.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Henry                    Henry)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Henry);

        #endregion

        #region (static) TryParse   (Span, FormatProvider, out Henry)

        /// <summary>
        /// Try to parse the given text span as henry using the given format provider.
        /// Supports optional suffixes "H", "µH", "nH" and "pH".
        /// </summary>
        /// <param name="Span">A text representation of henry.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Henry">The parsed Henry.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Henry           Henry)
        {

            Henry = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kH".AsSpan(), StringComparison.Ordinal))
            {
                exponent  = 3;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("mH".AsSpan(), StringComparison.Ordinal))
            {
                exponent  = -3;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("µH".AsSpan(), StringComparison.Ordinal))
            {
                exponent  = -6;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("nH".AsSpan(), StringComparison.Ordinal))
            {
                exponent  = -9;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("pH".AsSpan(), StringComparison.Ordinal))
            {
                exponent  = -12;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("H".AsSpan(),  StringComparison.Ordinal))
            {
                Span      = Span[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Henry);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKH (Text,                 out Henry)

        /// <summary>
        /// Try to parse the given string as KiloHenry (kH).
        /// </summary>
        /// <param name="Text">A text representation of KiloHenry (kH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        public static Boolean TryParseKH([NotNullWhen(true)] String?  Text,
                                         out                 Henry    Henry)
        {

            Henry = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Henry);
            }

            return false;

        }

        #endregion

        #region (static) TryParseH  (Text,                 out Henry)

        /// <summary>
        /// Try to parse the given string as Henry (H).
        /// </summary>
        /// <param name="Text">A text representation of Henry (H).</param>
        /// <param name="Henry">The parsed Henry.</param>
        public static Boolean TryParseH([NotNullWhen(true)] String?  Text,
                                        out                 Henry    Henry)
        {

            Henry = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Henry);
            }

            return false;

        }

        #endregion

        #region (static) TryParseMH (Text,                 out Henry)

        /// <summary>
        /// Try to parse the given string as MilliHenry (mH).
        /// </summary>
        /// <param name="Text">A text representation of MilliHenry (mH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        public static Boolean TryParseMH([NotNullWhen(true)] String?  Text,
                                         out                 Henry    Henry)
        {

            Henry = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -3, out Henry);
            }

            return false;

        }

        #endregion

        #region (static) TryParseµH (Text,                 out Henry)

        /// <summary>
        /// Try to parse the given string as MicroHenry (µH).
        /// </summary>
        /// <param name="Text">A text representation of MicroHenry (µH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        public static Boolean TryParseµH([NotNullWhen(true)] String?  Text,
                                         out                 Henry    Henry)
        {

            Henry = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -6, out Henry);
            }

            return false;

        }

        #endregion

        #region (static) TryParseNH (Text,                 out Henry)

        /// <summary>
        /// Try to parse the given string as NanoHenry (nH).
        /// </summary>
        /// <param name="Text">A text representation of NanoHenry (nH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        public static Boolean TryParseNH([NotNullWhen(true)] String?  Text,
                                         out                 Henry    Henry)
        {

            Henry = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -9, out Henry);
            }

            return false;

        }

        #endregion

        #region (static) TryParsePH (Text,                 out Henry)

        /// <summary>
        /// Try to parse the given string as PikoHenry (pH).
        /// </summary>
        /// <param name="Text">A text representation of PikoHenry (pH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        public static Boolean TryParsePH([NotNullWhen(true)] String?  Text,
                                         out                 Henry    Henry)
        {

            Henry = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -12, out Henry);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static Henry Create(Decimal  Number,
                                    Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var henry))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return henry;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Henry)

        private static Boolean TryCreate(Decimal    Number,
                                         Int32      Exponent,
                                         out Henry  Henry)
        {

            Henry = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Henry = Zero;
                return true;
            }

            try
            {
                Henry = new Henry(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) FromKH     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into KiloHenry (kH).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloHenry (kH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry FromKH<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 3)
                   );

        #endregion

        #region (static) FromH      (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into Henry (H).
        /// </summary>
        /// <param name="Number">A numeric representation of Henry (H).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry FromH<TNumber>(TNumber  Number,
                                           Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) FromMH     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into MilliHenry (mH).
        /// </summary>
        /// <param name="Number">A numeric representation of MilliHenry (mH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry FromMH<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 3)
                   );

        #endregion

        #region (static) FromµH     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into MicroHenry (µH).
        /// </summary>
        /// <param name="Number">A numeric representation of MicroHenry (µH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry FromµH<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 6)
                   );

        #endregion

        #region (static) FromNH     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into NanoHenry (nH).
        /// </summary>
        /// <param name="Number">A numeric representation of NanoHenry (nH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry FromNH<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 9)
                   );

        #endregion

        #region (static) FromPH     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into PikoHenry (pH).
        /// </summary>
        /// <param name="Number">A numeric representation of PikoHenry (pH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry FromPH<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 12)
                   );

        #endregion


        #region (static) TryFromH   (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into Henry (H).
        /// </summary>
        /// <param name="Number">A numeric representation of Henry (H).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry? TryFromH<TNumber>(TNumber  Number,
                                               Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromH(Number, out var henry, Exponent))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryFromKH  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloHenry (kH).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloHenry (kH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry? TryFromKH<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromKH(Number, out var henry, Exponent))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryFromMH  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into MilliHenry (mH).
        /// </summary>
        /// <param name="Number">A numeric representation of MilliHenry (mH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry? TryFromMH<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromMH(Number, out var henry, Exponent))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryFromµH  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into MicroHenry (µH).
        /// </summary>
        /// <param name="Number">A numeric representation of MicroHenry (µH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry? TryFromµH<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromµH(Number, out var henry, Exponent))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryFromNH  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into NanoHenry (nH).
        /// </summary>
        /// <param name="Number">A numeric representation of NanoHenry (nH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry? TryFromNH<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromNH(Number, out var henry, Exponent))
                return henry;

            return null;

        }

        #endregion

        #region (static) TryFromPH  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into PikoHenry (pH).
        /// </summary>
        /// <param name="Number">A numeric representation of PikoHenry (pH).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Henry? TryFromPH<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromPH(Number, out var henry, Exponent))
                return henry;

            return null;

        }

        #endregion


        #region (static) TryFromH   (Number, out Henry, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Henry (H).
        /// </summary>
        /// <param name="Number">A numeric representation of Henry (H).</param>
        /// <param name="Henry">The parsed Henry.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromH<TNumber>(TNumber    Number,
                                                out Henry  Henry,
                                                Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Henry = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Henry);
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

        #region (static) TryFromKH  (Number, out Henry, Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloHenry (kH).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloHenry (kH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKH<TNumber>(TNumber    Number,
                                                 out Henry  Henry,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Henry = default;

            if (!MathHelpers.TryAddExponent(Exponent, 3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Henry);
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

        #region (static) TryFromMH  (Number, out Henry, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MilliHenry (mH).
        /// </summary>
        /// <param name="Number">A numeric representation of MilliHenry (mH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMH<TNumber>(TNumber    Number,
                                                 out Henry  Henry,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Henry = default;

            if (!MathHelpers.TryAddExponent(Exponent, -3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Henry);
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

        #region (static) TryFromµH  (Number, out Henry, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MicroHenry (µH).
        /// </summary>
        /// <param name="Number">A numeric representation of MicroHenry (µH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromµH<TNumber>(TNumber    Number,
                                                 out Henry  Henry,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Henry = default;

            if (!MathHelpers.TryAddExponent(Exponent, -6, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Henry);
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

        #region (static) TryFromNH  (Number, out Henry, Exponent = null)

        /// <summary>
        /// Try to convert the given number into NanoHenry (nH).
        /// </summary>
        /// <param name="Number">A numeric representation of NanoHenry (nH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromNH<TNumber>(TNumber    Number,
                                                 out Henry  Henry,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Henry = default;

            if (!MathHelpers.TryAddExponent(Exponent, -9, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Henry);
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

        #region (static) TryFromPH  (Number, out Henry, Exponent = null)

        /// <summary>
        /// Try to convert the given number into PikoHenry (pH).
        /// </summary>
        /// <param name="Number">A numeric representation of PikoHenry (pH).</param>
        /// <param name="Henry">The parsed Henry.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromPH<TNumber>(TNumber    Number,
                                                 out Henry  Henry,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Henry = default;

            if (!MathHelpers.TryAddExponent(Exponent, -12, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Henry);
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

        #region Operator == (Henry1, Henry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Henry1">A Henry value.</param>
        /// <param name="Henry2">Another Henry value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Henry Henry1,
                                           Henry Henry2)

            => Henry1.Equals(Henry2);

        #endregion

        #region Operator != (Henry1, Henry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Henry1">A Henry value.</param>
        /// <param name="Henry2">Another Henry value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Henry Henry1,
                                           Henry Henry2)

            => !Henry1.Equals(Henry2);

        #endregion

        #region Operator <  (Henry1, Henry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Henry1">A Henry value.</param>
        /// <param name="Henry2">Another Henry value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Henry Henry1,
                                          Henry Henry2)

            => Henry1.CompareTo(Henry2) < 0;

        #endregion

        #region Operator <= (Henry1, Henry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Henry1">A Henry value.</param>
        /// <param name="Henry2">Another Henry value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Henry Henry1,
                                           Henry Henry2)

            => Henry1.CompareTo(Henry2) <= 0;

        #endregion

        #region Operator >  (Henry1, Henry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Henry1">A Henry value.</param>
        /// <param name="Henry2">Another Henry value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Henry Henry1,
                                          Henry Henry2)

            => Henry1.CompareTo(Henry2) > 0;

        #endregion

        #region Operator >= (Henry1, Henry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Henry1">A Henry value.</param>
        /// <param name="Henry2">Another Henry value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Henry Henry1,
                                           Henry Henry2)

            => Henry1.CompareTo(Henry2) >= 0;

        #endregion

        #region Operator +  (Henry1, Henry2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Henry1">A Henry value.</param>
        /// <param name="Henry2">Another Henry value.</param>
        public static Henry operator + (Henry Henry1,
                                          Henry Henry2)

            => new (Henry1.Value + Henry2.Value);

        #endregion

        #region Operator -  (Henry1, Henry2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Henry1">A Henry value.</param>
        /// <param name="Henry2">Another Henry value.</param>
        public static Henry operator - (Henry Henry1,
                                          Henry Henry2)

            => new (Henry1.Value - Henry2.Value);

        #endregion


        #region Operator *  (Henry,  Scalar)

        /// <summary>
        /// Multiplies a Henry with a scalar.
        /// </summary>
        /// <param name="Henry">A Henry value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Henry operator * (Henry  Henry,
                                          Decimal  Scalar)

            => new (Henry.Value * Scalar);

        #endregion

        #region Operator *  (Scalar, Henry)

        /// <summary>
        /// Multiplies a scalar with a Henry.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Henry">A Henry value.</param>
        public static Henry operator * (Decimal  Scalar,
                                          Henry  Henry)

            => new (Scalar * Henry.Value);

        #endregion

        #region Operator /  (Henry,  Scalar)

        /// <summary>
        /// Divides a Henry with a scalar.
        /// </summary>
        /// <param name="Henry">A Henry value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Henry operator / (Henry  Henry,
                                          Decimal  Scalar)

            => new (Henry.Value / Scalar);

        #endregion

        #region Operator /  (Henry,  Henry)

        /// <summary>
        /// Divides a Henry with another Henry, resulting in a dimensionless scalar.
        /// </summary>
        /// <param name="Henry1">A Henry value.</param>
        /// <param name="Henry2">Another Henry value.</param>
        public static Decimal operator / (Henry  Henry1,
                                          Henry  Henry2)

            => Henry1.Value / Henry2.Value;

        #endregion

        #endregion

        #region IComparable<Henry> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Henry.
        /// </summary>
        /// <param name="Object">A Henry to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null             => 1,
                   Henry henry  => CompareTo(henry),
                   _                => throw new ArgumentException("The given object is not a Henry!", nameof(Object))
               };

        #endregion

        #region CompareTo(Henry)

        /// <summary>
        /// Compares two Henry.
        /// </summary>
        /// <param name="Henry">A Henry to compare with.</param>
        public Int32 CompareTo(Henry Henry)

            => Value.CompareTo(Henry.Value);

        #endregion

        #endregion

        #region IEquatable<Henry> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Henry for equality.
        /// </summary>
        /// <param name="Object">A Henry to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Henry henry &&
                   Equals(henry);

        #endregion

        #region Equals(Henry)

        /// <summary>
        /// Compares two Henry for equality.
        /// </summary>
        /// <param name="Henry">A Henry to compare with.</param>
        public Boolean Equals(Henry Henry)

            => Value.Equals(Henry.Value);

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
        /// Try to format this Henry into the given character span using the given format and culture-specific format provider.
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

            if (Format.IsEmpty ||
                Format.Equals("G".AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                Format.Equals("H".AsSpan(), StringComparison.Ordinal))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " H".AsSpan()
                       );
            }

            if (Format.Equals("kH".AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           kH,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kH".AsSpan()
                       );

            if (Format.Equals("mH".AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           mH,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " mH".AsSpan()
                       );

            if (Format.Equals("µH".AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           µH,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " µH".AsSpan()
                       );

            if (Format.Equals("nH".AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           nH,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " nH".AsSpan()
                       );

            if (Format.Equals("pH".AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           pH,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " pH".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " H".AsSpan()
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

            if (String.IsNullOrEmpty(Format) ||
                String.Equals(Format, "G", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(Format, "H", StringComparison.Ordinal))
            {
                return $"{Value.ToString("G", FormatProvider)} H";
            }

            if (String.Equals(Format, "kH", StringComparison.Ordinal))
                return $"{kH.ToString("G", FormatProvider)} kH";

            if (String.Equals(Format, "mH", StringComparison.Ordinal))
                return $"{mH.ToString("G", FormatProvider)} mH";

            if (String.Equals(Format, "µH", StringComparison.Ordinal))
                return $"{µH.ToString("G", FormatProvider)} µH";

            if (String.Equals(Format, "nH", StringComparison.Ordinal))
                return $"{nH.ToString("G", FormatProvider)} nH";

            if (String.Equals(Format, "pH", StringComparison.Ordinal))
                return $"{pH.ToString("G", FormatProvider)} pH";

            return $"{Value.ToString(Format, FormatProvider)} H";

        }

        #endregion

    }

}
