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
    /// Extension methods for Watt (W) values.
    /// </summary>
    public static class WattExtensions
    {

        #region Sum    (this Watts)

        /// <summary>
        /// The sum of the given Watt values.
        /// </summary>
        /// <param name="Watts">An enumeration of Watt values.</param>
        public static Watt Sum(this IEnumerable<Watt> Watts)
        {

            var sum = Watt.Zero;

            foreach (var watt in Watts)
                sum += watt;

            return sum;

        }

        #endregion

        #region Avg    (this Watts)

        /// <summary>
        /// The average of the given Watt values.
        /// </summary>
        /// <param name="Watts">An enumeration of Watt values.</param>
        public static Watt Avg(this IEnumerable<Watt> Watts)
        {

            var sum    = Watt.Zero;
            var count  = 0;

            foreach (var watt in Watts)
            {
                sum += watt;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this Watts)

        /// <summary>
        /// The standard deviation of the given Watt values.
        /// </summary>
        /// <param name="Watts">An enumeration of Watt values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Watt> StdDev(this IEnumerable<Watt>  Watts,
                                          Boolean?                IsSampleData   = null)
        {

            var stdDev = StdDev<Watt>.From(
                             Watts.Select(watt => watt.Value),
                             IsSampleData
                         );

            return new StdDev<Watt>(
                       Watt.FromW(stdDev.Mean),
                       Watt.FromW(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Watt value (W), the SI unit of power.
    /// </summary>
    public readonly struct Watt : IMetrology<Watt>
    {

        #region Properties

        /// <summary>
        /// The value of the Watt.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Watt.
        /// </summary>
        public Int64    RoundedIntegerValue

            => Decimal.ToInt64(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as kiloWatts.
        /// </summary>
        public Decimal  kW
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The value as MegaWatts.
        /// </summary>
        public Decimal  MW
            => Value / 1000000m;

        /// <summary>
        /// The value as GigaWatts.
        /// </summary>
        public Decimal  GW
            => Value / 1000000000m;


        /// <summary>
        /// The zero value of the Watt.
        /// </summary>
        public static readonly Watt Zero = new (0m);

        /// <summary>
        /// The additive identity of Watt.
        /// </summary>
        public static Watt AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Watt (W) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of watts (W).</param>
        private Watt(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as watts using invariant culture.
        /// Supports optional suffixes "W", "kW", "MW", and "GW".
        /// </summary>
        /// <param name="Text">A text representation of watts.</param>
        public static Watt Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse      (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as watts using the given format provider.
        /// Supports optional suffixes "W", "kW", "MW", and "GW".
        /// </summary>
        /// <param name="Text">A text representation of watts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Watt Parse(String            Text,
                                 IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var watt))
                return watt;

            throw new FormatException($"Invalid text representation of watts: '{Text}'!");

        }

        #endregion

        #region (static) Parse      (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as watts using the given format provider.
        /// Supports optional suffixes "W", "kW", "MW", and "GW".
        /// </summary>
        /// <param name="Span">A text representation of watts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Watt Parse(ReadOnlySpan<Char>  Span,
                                 IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var ampere))
                return ampere;

            throw new FormatException($"Invalid text representation of watts: '{Span}'!");

        }

        #endregion

        #region (static) ParseW     (Text)

        /// <summary>
        /// Parse the given string as Watts (W).
        /// </summary>
        /// <param name="Text">A text representation of Watts (W).</param>
        public static Watt ParseW(String Text)
        {

            if (TryParseW(Text, out var watt))
                return watt;

            throw new FormatException($"Invalid text representation of Watts (W): '{Text}'!");

        }

        #endregion

        #region (static) ParseKW    (Text)

        /// <summary>
        /// Parse the given string as KiloWatts (kW).
        /// </summary>
        /// <param name="Text">A text representation of KiloWatts (kW).</param>
        public static Watt ParseKW(String Text)
        {

            if (TryParseKW(Text, out var watt))
                return watt;

            throw new FormatException($"Invalid text representation of kiloWatts: '{Text}'!");

        }

        #endregion

        #region (static) ParseMW    (Text)

        /// <summary>
        /// Parse the given string as MegaWatts (MW).
        /// </summary>
        /// <param name="Text">A text representation of MegaWatts (MW).</param>
        public static Watt ParseMW(String Text)
        {

            if (TryParseMW(Text, out var watt))
                return watt;

            throw new FormatException($"Invalid text representation of MegaWatts: '{Text}'!");

        }

        #endregion

        #region (static) ParseGW    (Text)

        /// <summary>
        /// Parse the given string as GigaWatts (GW).
        /// </summary>
        /// <param name="Text">A text representation of GigaWatts (GW).</param>
        public static Watt ParseGW(String Text)
        {

            if (TryParseGW(Text, out var watt))
                return watt;

            throw new FormatException($"Invalid text representation of GigaWatts: '{Text}'!");

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as watts with an optional unit suffix ("W", "kW", "MW", "GW")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of watts.</param>
        public static Watt? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var watt))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as watts with an optional unit suffix ("W", "kW", "MW", "GW")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of watts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Watt? TryParse(String?           Text,
                                     IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var watt))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParseW  (Text)

        /// <summary>
        /// Try to parse the given text as Watts (W).
        /// </summary>
        /// <param name="Text">A text representation of Watts (W).</param>
        public static Watt? TryParseW(String? Text)
        {

            if (TryParseW(Text, out var watt))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParseKW (Text)

        /// <summary>
        /// Try to parse the given text as KiloWatts (kW).
        /// </summary>
        /// <param name="Text">A text representation of KiloWatts (kW).</param>
        public static Watt? TryParseKW(String? Text)
        {

            if (TryParseKW(Text, out var watt))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParseMW (Text)

        /// <summary>
        /// Try to parse the given text as MegaWatts (MW).
        /// </summary>
        /// <param name="Text">A text representation of MegaWatts (MW).</param>
        public static Watt? TryParseMW(String? Text)
        {

            if (TryParseMW(Text, out var watt))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParseGW (Text)

        /// <summary>
        /// Try to parse the given text as GigaWatts (GW).
        /// </summary>
        /// <param name="Text">A text representation of GigaWatts (GW).</param>
        public static Watt? TryParseGW(String? Text)
        {

            if (TryParseGW(Text, out var watt))
                return watt;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,                 out Watt)

        /// <summary>
        /// Try to parse the given string as watts using invariant culture.
        /// Supports optional suffixes "W", "kW", "MW", "GW".
        /// </summary>
        /// <param name="Text">A text representation of watts.</param>
        /// <param name="Watt">The parsed Watt.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Watt     Watt)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Watt);

        #endregion

        #region (static) TryParse   (Text, FormatProvider, out Watt)

        /// <summary>
        /// Try to parse the given string as watts using the given format provider.
        /// Supports optional suffixes "W", "kW", "MW", "GW".
        /// </summary>
        /// <param name="Text">A text representation of watts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Watt">The parsed Watt.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Watt                     Watt)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Watt);

        #endregion

        #region (static) TryParse   (Span, FormatProvider, out Watt)

        /// <summary>
        /// Try to parse the given text span as watts using the given format provider.
        /// Supports optional suffixes "W", "kW", "MW", "GW".
        /// </summary>
        /// <param name="Span">A text representation of watts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Watt">The parsed Watt.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Watt            Watt)
        {

            Watt = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kW".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("MW".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 6;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("GW".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 9;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("W". AsSpan(),  StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Watt);
            }

            return false;

        }

        #endregion

        #region (static) TryParseW  (Text,                 out Watt)

        /// <summary>
        /// Try to parse the given string as Watts (W).
        /// </summary>
        /// <param name="Text">A text representation of Watts (W).</param>
        /// <param name="Watt">The parsed Watt.</param>
        public static Boolean TryParseW(String? Text, out Watt Watt)
        {

            Watt = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Watt);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKW (Text,                 out Watt)

        /// <summary>
        /// Try to parse the given string as KiloWatts (kW).
        /// </summary>
        /// <param name="Text">A text representation of a KiloWatts (kW).</param>
        /// <param name="Watt">The parsed Watt.</param>
        public static Boolean TryParseKW(String? Text, out Watt Watt)
        {

            Watt = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Watt);
            }

            return false;

        }

        #endregion

        #region (static) TryParseMW (Text,                 out Watt)

        /// <summary>
        /// Try to parse the given string as MegaWatts (MW).
        /// </summary>
        /// <param name="Text">A text representation of MegaWatts (MW).</param>
        /// <param name="Watt">The parsed Watt.</param>
        public static Boolean TryParseMW(String? Text, out Watt Watt)
        {

            Watt = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 6, out Watt);
            }

            return false;

        }

        #endregion

        #region (static) TryParseGW (Text,                 out Watt)

        /// <summary>
        /// Try to parse the given string as GigaWatts (GW).
        /// </summary>
        /// <param name="Text">A text representation of GigaWatts (GW).</param>
        /// <param name="Watt">The parsed Watt.</param>
        public static Boolean TryParseGW(String? Text, out Watt Watt)
        {

            Watt = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 9, out Watt);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static Watt Create(Decimal  Number,
                                   Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var watt))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return watt;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Watt)

        private static Boolean TryCreate(Decimal   Number,
                                         Int32     Exponent,
                                         out Watt  Watt)
        {

            Watt = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Watt = Zero;
                return true;
            }

            try
            {
                Watt = new Watt(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) FromW      (Number,           Exponent = null)

        /// <summary>
        /// Convert the given number into Watts (W).
        /// </summary>
        /// <param name="Number">A numeric representation of Watts (W).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromW<TNumber>(TNumber  Number,
                                          Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) FromKW     (Number,           Exponent = null)

        /// <summary>
        /// Convert the given number into KiloWatts (kW).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloWatts (kW).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromKW<TNumber>(TNumber  Number,
                                           Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 3)
                   );

        #endregion

        #region (static) FromMW     (Number,           Exponent = null)

        /// <summary>
        /// Convert the given number into MegaWatts (MW).
        /// </summary>
        /// <param name="Number">A numeric representation of MegaWatts (MW).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromMW<TNumber>(TNumber  Number,
                                           Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 6)
                   );

        #endregion

        #region (static) FromGW     (Number,           Exponent = null)

        /// <summary>
        /// Convert the given number into GigaWatts (GW).
        /// </summary>
        /// <param name="Number">A numeric representation of GigaWatts (GW).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromGW<TNumber>(TNumber  Number,
                                           Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 9)
                   );

        #endregion


        #region (static) TryFromW   (Number,           Exponent = null)

        /// <summary>
        /// Try to convert the given number into Watts (W).
        /// </summary>
        /// <param name="Number">A numeric representation of Watts (W).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromW<TNumber>(TNumber  Number,
                                              Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromW(Number, out var watt, Exponent))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryFromKW  (Number,           Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloWatts (kW).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloWatts (kW).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromKW<TNumber>(TNumber  Number,
                                               Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromKW(Number, out var watt, Exponent))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryFromMW  (Number,           Exponent = null)

        /// <summary>
        /// Try to convert the given number into MegaWatts (MW).
        /// </summary>
        /// <param name="Number">A numeric representation of MegaWatts (MW).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromMW<TNumber>(TNumber  Number,
                                               Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromMW(Number, out var watt, Exponent))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryFromGW  (Number,           Exponent = null)

        /// <summary>
        /// Try to convert the given number into GigaWatts (GW).
        /// </summary>
        /// <param name="Number">A numeric representation of GigaWatts (GW).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromGW<TNumber>(TNumber  Number,
                                               Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromGW(Number, out var watt, Exponent))
                return watt;

            return null;

        }

        #endregion


        #region (static) TryFromW   (Number, out Watt, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Watts (W).
        /// </summary>
        /// <param name="Number">A numeric representation of Watts (W).</param>
        /// <param name="Watt">The parsed Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromW<TNumber>(TNumber   Number,
                                                out Watt  Watt,
                                                Int32?    Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Watt = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Watt);
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

        #region (static) TryFromKW  (Number, out Watt, Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloWatts (kW).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloWatts (kW).</param>
        /// <param name="Watt">The parsed Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKW<TNumber>(TNumber   Number,
                                                 out Watt  Watt,
                                                 Int32?    Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Watt = default;

            if (!MathHelpers.TryAddExponent(Exponent, 3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Watt);
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

        #region (static) TryFromMW  (Number, out Watt, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MegaWatts (MW).
        /// </summary>
        /// <param name="Number">A numeric representation of MegaWatts (MW).</param>
        /// <param name="Watt">The parsed Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMW<TNumber>(TNumber   Number,
                                                 out Watt  Watt,
                                                 Int32?    Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Watt = default;

            if (!MathHelpers.TryAddExponent(Exponent, 6, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Watt);
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

        #region (static) TryFromGW  (Number, out Watt, Exponent = null)

        /// <summary>
        /// Try to convert the given number into GigaWatts (GW).
        /// </summary>
        /// <param name="Number">A numeric representation of GigaWatts (GW).</param>
        /// <param name="Watt">The parsed Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromGW<TNumber>(TNumber   Number,
                                                 out Watt  Watt,
                                                 Int32?    Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Watt = default;

            if (!MathHelpers.TryAddExponent(Exponent, 9, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Watt);
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

        #region Operator == (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Watt Watt1,
                                           Watt Watt2)

            => Watt1.Equals(Watt2);

        #endregion

        #region Operator != (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Watt Watt1,
                                           Watt Watt2)

            => !Watt1.Equals(Watt2);

        #endregion

        #region Operator <  (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Watt Watt1,
                                          Watt Watt2)

            => Watt1.CompareTo(Watt2) < 0;

        #endregion

        #region Operator <= (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Watt Watt1,
                                           Watt Watt2)

            => Watt1.CompareTo(Watt2) <= 0;

        #endregion

        #region Operator >  (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Watt Watt1,
                                          Watt Watt2)

            => Watt1.CompareTo(Watt2) > 0;

        #endregion

        #region Operator >= (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Watt Watt1,
                                           Watt Watt2)

            => Watt1.CompareTo(Watt2) >= 0;

        #endregion

        #region Operator +  (Watt1, Watt2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        public static Watt operator + (Watt Watt1,
                                       Watt Watt2)

            => new (Watt1.Value + Watt2.Value);

        #endregion

        #region Operator -  (Watt1, Watt2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        public static Watt operator - (Watt Watt1,
                                       Watt Watt2)

            => new (Watt1.Value - Watt2.Value);

        #endregion


        #region Operator /  (Watt,  Volt)

        /// <summary>
        /// Ampere = Watt / Volt
        /// </summary>
        /// <param name="Watt">A Watt.</param>
        /// <param name="Volt">A Volt.</param>
        public static Ampere operator / (Watt Watt,
                                         Volt Volt)

            => Ampere.FromA(Watt.Value / Volt.Value);

        #endregion


        #region Operator *  (Watt,  Scalar)

        /// <summary>
        /// Multiplies a Watt with a scalar.
        /// </summary>
        /// <param name="Watt">A Watt value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Watt operator * (Watt     Watt,
                                       Decimal  Scalar)

            => new (Watt.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,  Watt)

        /// <summary>
        /// Multiplies a scalar with a Watt.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Watt">A Watt value.</param>
        public static Watt operator * (Decimal  Scalar,
                                       Watt     Watt)

            => new (Scalar * Watt.Value);

        #endregion

        #region Operator /  (Watt,  Scalar)

        /// <summary>
        /// Divides a Watt with a scalar.
        /// </summary>
        /// <param name="Watt">A Watt value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Watt operator / (Watt     Watt,
                                       Decimal  Scalar)

            => new (Watt.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Watt> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Watts.
        /// </summary>
        /// <param name="Object">A Watt to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null       => 1,
                   Watt watt  => CompareTo(watt),
                   _          => throw new ArgumentException("The given object is not a Watt!", nameof(Object))
               };

        #endregion

        #region CompareTo(Watt)

        /// <summary>
        /// Compares two Watts.
        /// </summary>
        /// <param name="Watt">A Watt to compare with.</param>
        public Int32 CompareTo(Watt Watt)

            => Value.CompareTo(Watt.Value);

        #endregion

        #endregion

        #region IEquatable<Watt> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Watts for equality.
        /// </summary>
        /// <param name="Object">A Watt to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Watt watt &&
                   Equals(watt);

        #endregion

        #region Equals(Watt)

        /// <summary>
        /// Compares two Watts for equality.
        /// </summary>
        /// <param name="Watt">A Watt to compare with.</param>
        public Boolean Equals(Watt Watt)

            => Value.Equals(Watt.Value);

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
        /// Try to format this Watt into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("W".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " W".AsSpan()
                       );
            }

            if (Format.Equals("kW".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kW,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kW".AsSpan()
                       );

            if (Format.Equals("MW".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           MW,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " MW".AsSpan()
                       );

            if (Format.Equals("GW".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           GW,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " GW".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " W".AsSpan()
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
                String.Equals(Format, "G",  StringComparison.OrdinalIgnoreCase) ||
                String.Equals(Format, "W",  StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} W";
            }

            if (String.Equals(Format, "kW", StringComparison.OrdinalIgnoreCase))
                return $"{kW.ToString("G", FormatProvider)} kW";

            if (String.Equals(Format, "MW", StringComparison.OrdinalIgnoreCase))
                return $"{MW.ToString("G", FormatProvider)} MW";

            if (String.Equals(Format, "GW", StringComparison.OrdinalIgnoreCase))
                return $"{GW.ToString("G", FormatProvider)} GW";

            return $"{Value.ToString(Format, FormatProvider)} W";

        }

        #endregion

    }

}
