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

using System.Globalization;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class WattHourExtensions
    {

        #region Sum    (this WattHours)

        /// <summary>
        /// The sum of the given WattHour values.
        /// </summary>
        /// <param name="WattHours">An enumeration of WattHour values.</param>
        public static WattHour Sum(this IEnumerable<WattHour> Source)
        {

            var sum = WattHour.Zero;

            foreach (var value in Source)
                sum += value;

            return sum;

        }

        #endregion

        #region Avg    (this WattHours)

        /// <summary>
        /// The average of the given WattHour values.
        /// </summary>
        /// <param name="WattHours">An enumeration of WattHour values.</param>
        public static WattHour Avg(this IEnumerable<WattHour> WattHours)
        {

            var sum    = WattHour.Zero;
            var count  = 0;

            foreach (var wattHour in WattHours)
            {
                sum += wattHour;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this WattHours)

        /// <summary>
        /// The standard deviation of the given WattHour values.
        /// </summary>
        /// <param name="WattHours">An enumeration of WattHour values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<WattHour> StdDev(this IEnumerable<WattHour>  WattHours,
                                              Boolean?                    IsSampleData   = null)
        {

            var stdDev = StdDev<WattHour>.From(
                             WattHours.Select(wattHour => wattHour.Value),
                             IsSampleData
                         );

            return new StdDev<WattHour>(
                       WattHour.FromWh(stdDev.Mean),
                       WattHour.FromWh(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A WattHour (Wh) value, the SI unit of energy.
    /// </summary>
    public readonly struct WattHour : IMetrology<WattHour>
    {

        #region Properties

        /// <summary>
        /// The value of the WattHour.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the WattHour.
        /// </summary>
        public Int64    RoundedIntegerValue

            => Decimal.ToInt64(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as KiloWattHours.
        /// </summary>
        public Decimal  kWh
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The value as MegaWattHours.
        /// </summary>
        public Decimal  MWh
            => Value / 1000000m;

        /// <summary>
        /// The value as GigaWattHours.
        /// </summary>
        public Decimal  GWh
            => Value / 1000000000m;

        /// <summary>
        /// The zero value of the WattHour.
        /// </summary>
        public static readonly WattHour Zero = new (0m);

        /// <summary>
        /// The additive identity of WattHour.
        /// </summary>
        public static WattHour AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new WattHour (Wh) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of WattHours (Wh).</param>
        private WattHour(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse       (Text)

        /// <summary>
        /// Parse the given string as WattHours using invariant culture.
        /// Supports optional suffixes "Wh", "kWh", "MWh" or "GWh".
        /// </summary>
        /// <param name="Text">A text representation of WattHours.</param>
        public static WattHour Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse       (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as WattHours using the given format provider.
        /// Supports optional suffixes "Wh", "kWh", "MWh" or "GWh".
        /// </summary>
        /// <param name="Text">A text representation of WattHours.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static WattHour Parse(String            Text,
                                     IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var wattHour))
                return wattHour;

            throw new FormatException($"Invalid text representation of WattHours (Wh): '{Text}'!");

        }

        #endregion

        #region (static) Parse       (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as WattHours using the given format provider.
        /// Supports optional suffixes "Wh", "kWh", "MWh" or "GWh".
        /// </summary>
        /// <param name="Span">A text representation of WattHours.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static WattHour Parse(ReadOnlySpan<Char>  Span,
                                     IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var wattHour))
                return wattHour;

            throw new FormatException($"Invalid text representation of WattHours (Wh): '{Span}'!");

        }

        #endregion

        #region (static) ParseWh     (Text)

        /// <summary>
        /// Parse the given string as WattHours (Wh).
        /// </summary>
        /// <param name="Text">A text representation of WattHours (Wh).</param>
        public static WattHour ParseWh(String Text)
        {

            if (TryParseWh(Text, out var wattHour))
                return wattHour;

            throw new ArgumentException($"Invalid text representation of WattHours (Wh): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKWh    (Text)

        /// <summary>
        /// Parse the given string as KiloWattHours (kWh).
        /// </summary>
        /// <param name="Text">A text representation of KiloWattHours (kWh).</param>
        public static WattHour ParseKWh(String Text)
        {

            if (TryParseKWh(Text, out var wattHour))
                return wattHour;

            throw new ArgumentException($"Invalid text representation of KiloWattHours (kWh): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseMWh    (Text)

        /// <summary>
        /// Parse the given string as MegaWattHours (MWh).
        /// </summary>
        /// <param name="Text">A text representation of MegaWattHours (MWh).</param>
        public static WattHour ParseMWh(String Text)
        {

            if (TryParseMWh(Text, out var wattHour))
                return wattHour;

            throw new ArgumentException($"Invalid text representation of MegaWattHours (MWh): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseGWh    (Text)

        /// <summary>
        /// Parse the given string as GigaWattHours (GWh).
        /// </summary>
        /// <param name="Text">A text representation of GigaWattHours (GWh).</param>
        public static WattHour ParseGWh(String Text)
        {

            if (TryParseGWh(Text, out var wattHour))
                return wattHour;

            throw new ArgumentException($"Invalid text representation of GigaWattHours (GWh): '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) FromWh      (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into WattHours (Wh).
        /// </summary>
        /// <param name="Number">A numeric representation of WattHours (Wh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour FromWh(Decimal  Number,
                                      Int32?   Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into WattHours (Wh).
        /// </summary>
        /// <param name="Number">A numeric representation of WattHours (Wh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour FromWh(Byte    Number,
                                      Int32?  Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromKWh     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into KiloWattHours (kWh).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloWattHours (kWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour FromKWh(Decimal  Number,
                                       Int32?   Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into KiloWattHours (kWh).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloWattHours (kWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour FromKWh(Byte    Number,
                                       Int32?  Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromMWh     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into MegaWattHours (MWh).
        /// </summary>
        /// <param name="Number">A numeric representation of MegaWattHours (MWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour FromMWh(Decimal  Number,
                                       Int32?   Exponent = null)

            => new (1000000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into MegaWattHours (MWh).
        /// </summary>
        /// <param name="Number">A numeric representation of MegaWattHours (MWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour FromMWh(Byte    Number,
                                       Int32?  Exponent = null)

            => new (1000000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromGWh     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into GigaWattHours (GWh).
        /// </summary>
        /// <param name="Number">A numeric representation of GigaWattHours (GWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour FromGWh(Decimal  Number,
                                       Int32?   Exponent = null)

            => new (1000000000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into GigaWattHours (GWh).
        /// </summary>
        /// <param name="Number">A numeric representation of GigaWattHours (GWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour FromGWh(Byte    Number,
                                       Int32?  Exponent = null)

            => new (1000000000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion


        #region (static) TryParse    (Text)

        /// <summary>
        /// Try to parse the given text as WattHours with an optional unit suffix ("Wh", "kWh", "MWh" or "GWh")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of WattHours.</param>
        public static WattHour? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParse    (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as WattHours with an optional unit suffix ("Wh", "kWh", "MWh" or "GWh")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of WattHours.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static WattHour? TryParse(String?           Text,
                                         IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParseWh  (Text)

        /// <summary>
        /// Try to parse the given text as WattHours (Wh).
        /// </summary>
        /// <param name="Text">A text representation of WattHours (Wh).</param>
        public static WattHour? TryParseWh(String? Text)
        {

            if (TryParseWh(Text, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParseKWh (Text)

        /// <summary>
        /// Try to parse the given text as KiloWattHours (kWh).
        /// </summary>
        /// <param name="Text">A text representation of KiloWattHours (kWh).</param>
        public static WattHour? TryParseKWh(String? Text)
        {

            if (TryParseKWh(Text, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParseMWh (Text)

        /// <summary>
        /// Try to parse the given text as MegaWattHours (MWh).
        /// </summary>
        /// <param name="Text">A text representation of MegaWattHours (MWh).</param>
        public static WattHour? TryParseMWh(String? Text)
        {

            if (TryParseMWh(Text, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryParseGWh (Text)

        /// <summary>
        /// Try to parse the given text as GigaWattHours (GWh).
        /// </summary>
        /// <param name="Text">A text representation of GigaWattHours (GWh).</param>
        public static WattHour? TryParseGWh(String? Text)
        {

            if (TryParseGWh(Text, out var wattHour))
                return wattHour;

            return null;

        }

        #endregion


        #region (static) TryFromWh   (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into WattHours.
        /// </summary>
        /// <param name="Number">A numeric representation of WattHours.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryFromWh(Decimal  Number,
                                          Int32?   Exponent = null)
        {

            if (TryFromWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into WattHours.
        /// </summary>
        /// <param name="Number">A numeric representation of WattHours.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryFromWh(Byte    Number,
                                          Int32?  Exponent = null)
        {

            if (TryFromWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryFromKWh  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloWattHours (kWh).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloWattHours (kWh)..</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryFromKWh(Decimal  Number,
                                           Int32?   Exponent = null)
        {

            if (TryFromKWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into KiloWattHours (kWh).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloWattHours (kWh)..</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryFromKWh(Byte    Number,
                                           Int32?  Exponent = null)
        {

            if (TryFromKWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryFromMWh  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MegaWattHours (MWh).
        /// </summary>
        /// <param name="Number">A numeric representation of MegaWattHours (MWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryFromMWh(Decimal  Number,
                                           Int32?   Exponent = null)
        {

            if (TryFromMWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into MegaWattHours (MWh).
        /// </summary>
        /// <param name="Number">A numeric representation of MegaWattHours (MWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryFromMWh(Byte    Number,
                                           Int32?  Exponent = null)
        {

            if (TryFromMWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }

        #endregion

        #region (static) TryFromGWh  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into GigaWattHours (GWh).
        /// </summary>
        /// <param name="Number">A numeric representation of GigaWattHours (GWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryFromGWh(Decimal  Number,
                                           Int32?   Exponent = null)
        {

            if (TryFromGWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into GigaWattHours (GWh).
        /// </summary>
        /// <param name="Number">A numeric representation of GigaWattHours (GWh).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static WattHour? TryFromGWh(Byte    Number,
                                           Int32?  Exponent = null)
        {

            if (TryFromGWh(Number, out var wattHour, Exponent))
                return wattHour;

            return null;

        }

        #endregion


        #region (static) TryParse    (Text,                 out WattHour)

        /// <summary>
        /// Try to parse the given string as WattHours using invariant culture.
        /// Supports optional suffixes "Wh", "kWh", "MWh" or "GWh".
        /// </summary>
        /// <param name="Text">A text representation of WattHours.</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?   Text,
                                       out                 WattHour  WattHour)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out WattHour);

        #endregion

        #region (static) TryParse    (Text, FormatProvider, out WattHour)

        /// <summary>
        /// Try to parse the given string as WattHours using the given format provider.
        /// Supports optional suffixes "Wh", "kWh", "MWh" or "GWh".
        /// </summary>
        /// <param name="Text">A text representation of WattHours.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out WattHour                 WattHour)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out WattHour);

        #endregion

        #region (static) TryParse    (Span, FormatProvider, out WattHour)

        /// <summary>
        /// Try to parse the given text span as WattHours using the given format provider.
        /// Supports optional suffixes "Wh", "kWh", "MWh" or "GWh".
        /// </summary>
        /// <param name="Span">A text representation of WattHours.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out WattHour        WattHour)
        {

            WattHour = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kWh".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^3].TrimEnd();
            }

            else if (Span.EndsWith("MWh".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 6;
                Span      = Span[..^3].TrimEnd();
            }

            else if (Span.EndsWith("GWh".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 9;
                Span      = Span[..^3].TrimEnd();
            }

            else if (Span.EndsWith("Wh". AsSpan(),  StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^2].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out WattHour);
            }

            return false;

        }

        #endregion

        #region (static) TryParseWh  (Text,                 out WattHour)

        /// <summary>
        /// Parse the given string as WattHours (Wh).
        /// </summary>
        /// <param name="Text">A text representation of WattHours (Wh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        public static Boolean TryParseWh([NotNullWhen(true)] String?   Text,
                                         out                 WattHour  WattHour)
        {

            WattHour = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out WattHour);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKWh (Text,                 out WattHour)

        /// <summary>
        /// Parse the given string as KiloWattHours (kWh).
        /// </summary>
        /// <param name="Text">A text representation of KiloWattHours (kWh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        public static Boolean TryParseKWh([NotNullWhen(true)] String?   Text,
                                          out                 WattHour  WattHour)
        {

            WattHour = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out WattHour);
            }

            return false;

        }

        #endregion

        #region (static) TryParseMWh (Text,                 out WattHour)

        /// <summary>
        /// Parse the given string as GigaWattHours (MWh).
        /// </summary>
        /// <param name="Text">A text representation of GigaWattHours (MWh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        public static Boolean TryParseMWh([NotNullWhen(true)] String?   Text,
                                          out                 WattHour  WattHour)
        {

            WattHour = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 6, out WattHour);
            }

            return false;

        }

        #endregion

        #region (static) TryParseGWh (Text,                 out WattHour)

        /// <summary>
        /// Parse the given string as GigaWattHours (GWh).
        /// </summary>
        /// <param name="Text">A text representation of GigaWattHours (GWh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        public static Boolean TryParseGWh([NotNullWhen(true)] String?   Text,
                                          out                 WattHour  WattHour)
        {

            WattHour = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 9, out WattHour);
            }

            return false;

        }

        #endregion


        #region (private static) TryCreate(Number, Exponent, out WattHour)

        private static Boolean TryCreate(Decimal       Number,
                                         Int32         Exponent,
                                         out WattHour  WattHour)
        {

            WattHour = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                WattHour = Zero;
                return true;
            }

            try
            {
                WattHour = new WattHour(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) TryFromWh   (Number, out WattHour, Exponent = null)

        /// <summary>
        /// Try to convert the given number into WattHours (Wh).
        /// </summary>
        /// <param name="Number">A numeric representation of WattHours (Wh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromWh(Byte          Number,
                                        out WattHour  WattHour,
                                        Int32?        Exponent = null)

            => TryCreate(Number, Exponent ?? 0, out WattHour);


        /// <summary>
        /// Try to convert the given number into WattHours (Wh).
        /// </summary>
        /// <param name="Number">A numeric representation of WattHours (Wh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromWh(Decimal       Number,
                                        out WattHour  WattHour,
                                        Int32?        Exponent = null)

            => TryCreate(Number, Exponent ?? 0, out WattHour);

        #endregion

        #region (static) TryFromKWh  (Number, out WattHour, Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloWattHours (kWh).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloWattHours (kWh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKWh(Byte          Number,
                                         out WattHour  WattHour,
                                         Int32?        Exponent = null)
        {

            WattHour = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out WattHour);

        }


        /// <summary>
        /// Try to convert the given number into KiloWattHours (kWh).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloWattHours (kWh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKWh(Decimal       Number,
                                         out WattHour  WattHour,
                                         Int32?        Exponent = null)
        {

            WattHour = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out WattHour);

        }

        #endregion

        #region (static) TryFromMWh  (Number, out WattHour, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MegaWattHours (MWh).
        /// </summary>
        /// <param name="Number">A numeric representation of MegaWattHours (MWh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMWh(Byte          Number,
                                         out WattHour  WattHour,
                                         Int32?        Exponent = null)
        {

            WattHour = default;

            return MathHelpers.TryAddExponent(Exponent, 6, out var exponent) &&
                   TryCreate(Number, exponent, out WattHour);

        }


        /// <summary>
        /// Try to convert the given number into MegaWattHours (MWh).
        /// </summary>
        /// <param name="Number">A numeric representation of MegaWattHours (MWh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMWh(Decimal       Number,
                                         out WattHour  WattHour,
                                         Int32?        Exponent = null)
        {

            WattHour = default;

            return MathHelpers.TryAddExponent(Exponent, 6, out var exponent) &&
                   TryCreate(Number, exponent, out WattHour);

        }

        #endregion

        #region (static) TryFromGWh  (Number, out WattHour, Exponent = null)

        /// <summary>
        /// Try to convert the given number into GigaWattHours (GWh).
        /// </summary>
        /// <param name="Number">A numeric representation of GigaWattHours (GWh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromGWh(Byte          Number,
                                         out WattHour  WattHour,
                                         Int32?        Exponent = null)
        {

            WattHour = default;

            return MathHelpers.TryAddExponent(Exponent, 9, out var exponent) &&
                   TryCreate(Number, exponent, out WattHour);

        }


        /// <summary>
        /// Try to convert the given number into GigaWattHours (GWh).
        /// </summary>
        /// <param name="Number">A numeric representation of GigaWattHours (GWh).</param>
        /// <param name="WattHour">The parsed WattHour.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromGWh(Decimal       Number,
                                         out WattHour  WattHour,
                                         Int32?        Exponent = null)
        {

            WattHour = default;

            return MathHelpers.TryAddExponent(Exponent, 9, out var exponent) &&
                   TryCreate(Number, exponent, out WattHour);

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
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        public static WattHour operator + (WattHour WattHour1,
                                           WattHour WattHour2)

            => new (WattHour1.Value + WattHour2.Value);

        #endregion

        #region Operator -  (WattHour1, WattHour2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="WattHour1">A WattHour.</param>
        /// <param name="WattHour2">Another WattHour.</param>
        public static WattHour operator - (WattHour WattHour1,
                                           WattHour WattHour2)

            => new (WattHour1.Value - WattHour2.Value);

        #endregion


        #region Operator *  (WattHour,  Scalar)

        /// <summary>
        /// Multiplies WattHours with a scalar.
        /// </summary>
        /// <param name="WattHour">A WattHour value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static WattHour operator * (WattHour  WattHour,
                                           Decimal   Scalar)

            => new (WattHour.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,    WattHour)

        /// <summary>
        /// Multiplies a scalar with WattHours.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="WattHour">A WattHour value.</param>
        public static WattHour operator * (Decimal   Scalar,
                                           WattHour  WattHour)

            => new (Scalar * WattHour.Value);

        #endregion

        #region Operator /  (WattHour,  Scalar)

        /// <summary>
        /// Divides WattHours with a scalar.
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

            => Object switch {
                   null               => 1,
                   WattHour wattHour  => CompareTo(wattHour),
                   _                  => throw new ArgumentException("The given object is not a WattHour!", nameof(Object))
               };

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


        #region TryFormat(Destination, out CharsWritten, Format, FormatProvider)

        /// <summary>
        /// Try to format this WattHour into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("G". AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                Format.Equals("Wh".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Wh".AsSpan()
                       );
            }

            if (Format.Equals("kWh".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kWh,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kWh".AsSpan()
                       );

            if (Format.Equals("MWh".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           MWh,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " MWh".AsSpan()
                       );

            if (Format.Equals("GWh".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           GWh,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " GWh".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " Wh".AsSpan()
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
                String.Equals(Format, "Wh", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} Wh";
            }

            if (String.Equals(Format, "kWh", StringComparison.OrdinalIgnoreCase))
                return $"{kWh.ToString("G", FormatProvider)} kWh";

            if (String.Equals(Format, "MWh", StringComparison.OrdinalIgnoreCase))
                return $"{MWh.ToString("G", FormatProvider)} MWh";

            if (String.Equals(Format, "GWh", StringComparison.OrdinalIgnoreCase))
                return $"{GWh.ToString("G", FormatProvider)} GWh";

            return $"{Value.ToString(Format, FormatProvider)} Wh";

        }

        #endregion

    }

}
