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
    /// Extension methods for Meter (m) values.
    /// </summary>
    public static class MeterExtensions
    {

        #region Sum    (this MeterValues)

        /// <summary>
        /// The sum of the given enumeration of Meter values.
        /// </summary>
        /// <param name="MeterValues">An enumeration of Meter values.</param>
        public static Meter Sum(this IEnumerable<Meter> MeterValues)
        {

            var sum = Meter.Zero;

            foreach (var meter in MeterValues)
                sum += meter;

            return sum;

        }

        #endregion

        #region Avg    (this MeterValues)

        /// <summary>
        /// The average of the given enumeration of Meter values.
        /// </summary>
        /// <param name="MeterValues">An enumeration of Meter values.</param>
        public static Meter Avg(this IEnumerable<Meter> MeterValues)
        {

            var sum    = Meter.Zero;
            var count  = 0;

            foreach (var meter in MeterValues)
            {
                sum += meter;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this MeterValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Meter values.
        /// </summary>
        /// <param name="MeterValues">An enumeration of Meter values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Meter> StdDev(this IEnumerable<Meter>  MeterValues,
                                           Boolean?                 IsSampleData   = null)
        {

            var stdDev = StdDev<Meter>.From(
                             MeterValues.Select(meter => meter.m),
                             IsSampleData
                         );

            return new StdDev<Meter>(
                       Meter.From_m(stdDev.Mean),
                       Meter.From_m(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Meter value (m), the SI unit of length.
    /// </summary>
    public readonly struct Meter : IMetrology<Meter>
    {

        #region Properties

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// The value as meters (m).
        /// </summary>
        public Decimal  m    { get; }

#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The rounded integer value of the Meter.
        /// </summary>
        public Int64    RoundedIntegerValue

            => Decimal.ToInt64(
                   Decimal.Round(m, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// The value as millimeters (mm).
        /// </summary>
        public Decimal  mm
            => m * 1000;

        /// <summary>
        /// The value as centimeters (cm).
        /// </summary>
        public Decimal  cm
            => m * 100;

        /// <summary>
        /// The value as decimeters (dm).
        /// </summary>
        public Decimal  dm
            => m * 10;

        /// <summary>
        /// The value as KiloMeters (km).
        /// </summary>
        public Decimal  km
            => m / 1000m;

#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The zero value of a Meter.
        /// </summary>
        public static readonly Meter Zero = new (0m);

        /// <summary>
        /// The additive identity of Meter.
        /// </summary>
        public static Meter AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Meter (m) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of meters (m).</param>
        private Meter(Decimal Value)
        {
            this.m = Value;
        }

        #endregion


        #region (static) Parse       (Text)

        /// <summary>
        /// Parse the given string as meters using invariant culture.
        /// Supports optional suffixes "mm", "mm", "cm", "dm", "m" and "km".
        /// </summary>
        /// <param name="Text">A text representation of meters.</param>
        public static Meter Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse       (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as meters using the given format provider.
        /// Supports optional suffixes "mm", "cm", "dm", "m" and "km".
        /// </summary>
        /// <param name="Text">A text representation of meters.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Meter Parse(String            Text,
                                  IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var meter))
                return meter;

            throw new FormatException($"Invalid text representation of meters: '{Text}'!");

        }

        #endregion

        #region (static) Parse       (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as meters using the given format provider.
        /// Supports optional suffixes "mm", "cm", "dm", "m" and "km".
        /// </summary>
        /// <param name="Span">A text representation of a Meter.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Meter Parse(ReadOnlySpan<Char>  Span,
                                  IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var ampere))
                return ampere;

            throw new FormatException($"Invalid text representation of meters: '{Span}'!");

        }

        #endregion

        #region (static) Parse_mm    (Text)

        /// <summary>
        /// Parse the given string as millimeters (mm).
        /// </summary>
        /// <param name="Text">A text representation of millimeters (mm).</param>
        public static Meter Parse_mm(String Text)
        {

            if (TryParse_mm(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of millimeters (mm): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse_cm    (Text)

        /// <summary>
        /// Parse the given string as centimeters (cm).
        /// </summary>
        /// <param name="Text">A text representation of centimeters (cm).</param>
        public static Meter Parse_cm(String Text)
        {

            if (TryParse_cm(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of centimeters (cm): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse_dm    (Text)

        /// <summary>
        /// Parse the given string as decimeters (dm).
        /// </summary>
        /// <param name="Text">A text representation of decimeters (dm).</param>
        public static Meter Parse_dm(String Text)
        {

            if (TryParse_dm(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of decimeters (dm): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse_m     (Text)

        /// <summary>
        /// Parse the given string as meters (m).
        /// </summary>
        /// <param name="Text">A text representation of meters (m).</param>
        public static Meter Parse_m(String Text)
        {

            if (TryParse_m(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of meters (m): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse_km    (Text)

        /// <summary>
        /// Parse the given string as kilometers (km).
        /// </summary>
        /// <param name="Text">A text representation of kilometers (km).</param>
        public static Meter Parse_km(String Text)
        {

            if (TryParse_km(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of kilometers (km): '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) TryParse    (Text)

        /// <summary>
        /// Try to parse the given text as meters with an optional unit suffix ("mm", "cm", "dm", "m" or "km")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of a Meter.</param>
        public static Meter? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParse    (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as meters with an optional unit suffix ("mm", "cm", "dm", "m" or "km")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of a Meter.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Meter? TryParse(String?           Text,
                                      IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParse_mm (Text)

        /// <summary>
        /// Try to parse the given text as millimeters (mm).
        /// </summary>
        /// <param name="Text">A text representation of a millimeters (mm).</param>
        public static Meter? TryParse_mm(String? Text)
        {

            if (TryParse_mm(Text, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParse_cm (Text)

        /// <summary>
        /// Try to parse the given text as centimeters (cm).
        /// </summary>
        /// <param name="Text">A text representation of a centimeters (cm).</param>
        public static Meter? TryParse_cm(String? Text)
        {

            if (TryParse_cm(Text, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParse_dm (Text)

        /// <summary>
        /// Try to parse the given text as decimeters.
        /// </summary>
        /// <param name="Text">A text representation of decimeters.</param>
        public static Meter? TryParse_dm(String? Text)
        {

            if (TryParse_dm(Text, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParse_m  (Text)

        /// <summary>
        /// Try to parse the given text as meters.
        /// </summary>
        /// <param name="Text">A text representation of meters.</param>
        public static Meter? TryParse_m(String? Text)
        {

            if (TryParse_m(Text, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParse_km (Text)

        /// <summary>
        /// Try to parse the given text as kilometers.
        /// </summary>
        /// <param name="Text">A text representation of kilometers.</param>
        public static Meter? TryParse_km(String? Text)
        {

            if (TryParse_km(Text, out var meter))
                return meter;

            return null;

        }

        #endregion


        #region (static) TryParse    (Text,                 out Meter)

        /// <summary>
        /// Try to parse the given string as meters using invariant culture.
        /// Supports optional suffixes "mm", "cm", "dm", "m" and "km".
        /// </summary>
        /// <param name="Text">A text representation of meters.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Meter    Meter)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Meter);

        #endregion

        #region (static) TryParse    (Text, FormatProvider, out Meter)

        /// <summary>
        /// Try to parse the given string as meters using the given format provider.
        /// Supports optional suffixes "mm", "cm", "dm", "m" and "km".
        /// </summary>
        /// <param name="Text">A text representation of meters.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Meter                    Meter)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Meter);

        #endregion

        #region (static) TryParse    (Span, FormatProvider, out Meter)

        /// <summary>
        /// Try to parse the given text span as meters using the given format provider.
        /// Supports optional suffixes "mm", "cm", "dm", "m" and "km".
        /// </summary>
        /// <param name="Span">A text representation of meters.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Meter           Meter)
        {

            Meter = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("mm".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = -3;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("cm".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = -2;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("dm".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = -1;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("km".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("m". AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Meter);
            }

            return false;

        }

        #endregion

        #region (static) TryParse_mm (Text,   out Meter)

        /// <summary>
        /// Parse the given string as millimeters (mm).
        /// </summary>
        /// <param name="Text">A text representation of millimeters (mm).</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse_mm([NotNullWhen(true)] String?  Text,
                                          out                 Meter    Meter)
        {

            Meter = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -3, out Meter);
            }

            return false;

        }

        #endregion

        #region (static) TryParse_cm (Text,   out Meter)

        /// <summary>
        /// Parse the given string as centimeters (cm).
        /// </summary>
        /// <param name="Text">A text representation of centimeters (cm).</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse_cm([NotNullWhen(true)] String?  Text,
                                          out                 Meter    Meter)
        {

            Meter = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -2, out Meter);
            }

            return false;

        }

        #endregion

        #region (static) TryParse_dm (Text,   out Meter)

        /// <summary>
        /// Parse the given string as decimeters.
        /// </summary>
        /// <param name="Text">A text representation of decimeters.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse_dm([NotNullWhen(true)] String?  Text,
                                          out                 Meter    Meter)
        {

            Meter = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -1, out Meter);
            }

            return false;

        }

        #endregion

        #region (static) TryParse_m  (Text,   out Meter)

        /// <summary>
        /// Parse the given string as meters.
        /// </summary>
        /// <param name="Text">A text representation of meters.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse_m([NotNullWhen(true)] String?  Text,
                                         out                 Meter    Meter)
        {

            Meter = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Meter);
            }

            return false;

        }

        #endregion

        #region (static) TryParse_km (Text,   out Meter)

        /// <summary>
        /// Parse the given string as kilometers.
        /// </summary>
        /// <param name="Text">A text representation of kilometers.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse_km([NotNullWhen(true)] String?  Text,
                                          out                 Meter    Meter)
        {

            Meter = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Meter);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static Meter Create(Decimal  Number,
                                    Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var meter))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return meter;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Meter)

        private static Boolean TryCreate(Decimal    Number,
                                         Int32      Exponent,
                                         out Meter  Meter)
        {

            Meter = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Meter = Zero;
                return true;
            }

            try
            {
                Meter = new Meter(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) From_mm     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into millimeters (mm).
        /// </summary>
        /// <param name="Number">A numeric representation of millimeters (mm).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter From_mm<TNumber>(TNumber  Number,
                                             Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 3)
                   );

        #endregion

        #region (static) From_cm     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into centimeters (cm).
        /// </summary>
        /// <param name="Number">A numeric representation of centimeters (cm).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter From_cm<TNumber>(TNumber  Number,
                                             Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 2)
                   );

        #endregion

        #region (static) From_dm     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into decimeters (dm).
        /// </summary>
        /// <param name="Number">A numeric representation of decimeters (dm).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter From_dm<TNumber>(TNumber  Number,
                                             Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 1)
                   );

        #endregion

        #region (static) From_m      (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into meters (m).
        /// </summary>
        /// <param name="Number">A numeric representation of meters (m).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter From_m<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) From_km     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into kilometers (km).
        /// </summary>
        /// <param name="Number">A numeric representation of kilometers (km).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter From_km<TNumber>(TNumber  Number,
                                             Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 3)
                   );

        #endregion


        #region (static) TryFrom_mm  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into millimeters (mm).
        /// </summary>
        /// <param name="Number">A numeric representation of millimeters (mm).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryFrom_mm<TNumber>(TNumber  Number,
                                                 Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFrom_mm(Number, out var meter, Exponent))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryFrom_cm  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into centimeters (cm).
        /// </summary>
        /// <param name="Number">A numeric representation of centimeters (cm).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryFrom_cm<TNumber>(TNumber  Number,
                                                 Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFrom_cm(Number, out var meter, Exponent))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryFrom_dm  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into decimeters (dm).
        /// </summary>
        /// <param name="Number">A numeric representation of decimeters (dm).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryFrom_dm<TNumber>(TNumber  Number,
                                                 Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFrom_dm(Number, out var meter, Exponent))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryFrom_m   (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into meters (m).
        /// </summary>
        /// <param name="Number">A numeric representation of meters (m).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryFrom_m<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFrom_m(Number, out var meter, Exponent))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryFrom_km  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into kilometers (km).
        /// </summary>
        /// <param name="Number">A numeric representation of kilometers (km).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryFrom_km<TNumber>(TNumber  Number,
                                                 Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFrom_km(Number, out var meter, Exponent))
                return meter;

            return null;

        }

        #endregion


        #region (static) TryFrom_mm  (Number, out Meter, Exponent = null)

        /// <summary>
        /// Try to convert the given number into millimeters (mm).
        /// </summary>
        /// <param name="Number">A numeric representation of millimeters (mm).</param>
        /// <param name="Meter">The parsed Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFrom_mm<TNumber>(TNumber    Number,
                                                  out Meter  Meter,
                                                  Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Meter = default;

            if (!MathHelpers.TryAddExponent(Exponent, -3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Meter);
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

        #region (static) TryFrom_cm  (Number, out Meter, Exponent = null)

        /// <summary>
        /// Try to convert the given number into centimeters (cm).
        /// </summary>
        /// <param name="Number">A numeric representation of centimeters (cm).</param>
        /// <param name="Meter">The parsed Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFrom_cm<TNumber>(TNumber    Number,
                                                  out Meter  Meter,
                                                  Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Meter = default;

            if (!MathHelpers.TryAddExponent(Exponent, -2, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Meter);
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

        #region (static) TryFrom_dm  (Number, out Meter, Exponent = null)

        /// <summary>
        /// Try to convert the given number into decimeters (dm).
        /// </summary>
        /// <param name="Number">A numeric representation of decimeters (dm).</param>
        /// <param name="Meter">The parsed Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFrom_dm<TNumber>(TNumber    Number,
                                                  out Meter  Meter,
                                                  Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Meter = default;

            if (!MathHelpers.TryAddExponent(Exponent, -1, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Meter);
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

        #region (static) TryFrom_m   (Number, out Meter, Exponent = null)

        /// <summary>
        /// Try to convert the given number into meters (m).
        /// </summary>
        /// <param name="Number">A numeric representation of meters (m).</param>
        /// <param name="Meter">The parsed Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFrom_m<TNumber>(TNumber    Number,
                                                 out Meter  Meter,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Meter = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Meter);
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

        #region (static) TryFrom_km  (Number, out Meter, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kilometers (km).
        /// </summary>
        /// <param name="Number">A numeric representation of kilometers (km).</param>
        /// <param name="Meter">The parsed Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFrom_km<TNumber>(TNumber    Number,
                                                  out Meter  Meter,
                                                  Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Meter = default;

            if (!MathHelpers.TryAddExponent(Exponent, 3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Meter);
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

        #region Operator == (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Meter Meter1,
                                           Meter Meter2)

            => Meter1.Equals(Meter2);

        #endregion

        #region Operator != (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Meter Meter1,
                                           Meter Meter2)

            => !Meter1.Equals(Meter2);

        #endregion

        #region Operator <  (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Meter Meter1,
                                          Meter Meter2)

            => Meter1.CompareTo(Meter2) < 0;

        #endregion

        #region Operator <= (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Meter Meter1,
                                           Meter Meter2)

            => Meter1.CompareTo(Meter2) <= 0;

        #endregion

        #region Operator >  (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Meter Meter1,
                                          Meter Meter2)

            => Meter1.CompareTo(Meter2) > 0;

        #endregion

        #region Operator >= (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Meter Meter1,
                                           Meter Meter2)

            => Meter1.CompareTo(Meter2) >= 0;

        #endregion

        #region Operator +  (Meter1, Meter2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        public static Meter operator + (Meter Meter1,
                                        Meter Meter2)

            => new (Meter1.m + Meter2.m);

        #endregion

        #region Operator -  (Meter1, Meter2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        public static Meter operator - (Meter Meter1,
                                        Meter Meter2)

            => new (Meter1.m - Meter2.m);

        #endregion


        #region Operator *  (Meter,  Scalar)

        /// <summary>
        /// Multiplies a Meter with a scalar.
        /// </summary>
        /// <param name="Meter">A Meter value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Meter operator * (Meter    Meter,
                                        Decimal  Scalar)

            => new (Meter.m * Scalar);

        #endregion

        #region Operator *  (Scalar, Meter)

        /// <summary>
        /// Multiplies a scalar with a Meter.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Meter">A Meter value.</param>
        public static Meter operator * (Decimal  Scalar,
                                        Meter    Meter)

            => new (Scalar * Meter.m);

        #endregion

        #region Operator /  (Meter,  Scalar)

        /// <summary>
        /// Divides a Meter with a scalar.
        /// </summary>
        /// <param name="Meter">A Meter value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Meter operator / (Meter    Meter,
                                        Decimal  Scalar)

            => new (Meter.m / Scalar);

        #endregion

        #endregion

        #region IComparable<Meter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two meters.
        /// </summary>
        /// <param name="Object">A meter to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null         => 1,
                   Meter meter  => CompareTo(meter),
                   _            => throw new ArgumentException("The given object is not a Meter!", nameof(Object))
               };

        #endregion

        #region CompareTo(Meter)

        /// <summary>
        /// Compares two meters.
        /// </summary>
        /// <param name="Meter">A meter to compare with.</param>
        public Int32 CompareTo(Meter Meter)

            => m.CompareTo(Meter.m);

        #endregion

        #endregion

        #region IEquatable<Meter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two meters for equality.
        /// </summary>
        /// <param name="Object">A meter to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Meter meter &&
                   Equals(meter);

        #endregion

        #region Equals(Meter)

        /// <summary>
        /// Compares two meters for equality.
        /// </summary>
        /// <param name="Meter">A meter to compare with.</param>
        public Boolean Equals(Meter Meter)

            => m.Equals(Meter.m);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => m.GetHashCode();

        #endregion


        #region TryFormat(Destination, out CharsWritten, Format, FormatProvider)

        /// <summary>
        /// Try to format this Meter into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("m".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           m,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " m".AsSpan()
                       );
            }

            if (Format.Equals("mm".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           mm,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " mm".AsSpan()
                       );

            if (Format.Equals("cm".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           cm,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " cm".AsSpan()
                       );

            if (Format.Equals("dm".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           dm,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " dm".AsSpan()
                       );

            if (Format.Equals("km".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           km,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " km".AsSpan()
                       );

            return TryFormatWithSuffix(
                       m,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " m".AsSpan()
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
                String.Equals(Format, "m",  StringComparison.OrdinalIgnoreCase))
            {
                return $"{m.ToString("G", FormatProvider)} m";
            }

            if (String.Equals(Format, "mm", StringComparison.OrdinalIgnoreCase))
                return $"{mm.ToString("G", FormatProvider)} mm";

            if (String.Equals(Format, "cm", StringComparison.OrdinalIgnoreCase))
                return $"{cm.ToString("G", FormatProvider)} cm";

            if (String.Equals(Format, "dm", StringComparison.OrdinalIgnoreCase))
                return $"{dm.ToString("G", FormatProvider)} dm";

            if (String.Equals(Format, "km", StringComparison.OrdinalIgnoreCase))
                return $"{km.ToString("G", FormatProvider)} km";

            return $"{m.ToString(Format, FormatProvider)} m";

        }

        #endregion

    }

}
