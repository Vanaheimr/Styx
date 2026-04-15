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

    /// <summary>
    /// Extension methods for Hertz.
    /// </summary>
    public static class HertzExtensions
    {

        #region Sum    (this HertzValues)

        /// <summary>
        /// The sum of the given enumeration of Hertz values.
        /// </summary>
        /// <param name="HertzValues">An enumeration of Hertz values.</param>
        public static Hertz Sum(this IEnumerable<Hertz> HertzValues)
        {

            var sum = Hertz.Zero;

            foreach (var hertz in HertzValues)
                sum += hertz;

            return sum;

        }

        #endregion

        #region Avg    (this HertzValues)

        /// <summary>
        /// The average of the given enumeration of Hertz values.
        /// </summary>
        /// <param name="HertzValues">An enumeration of Hertz values.</param>
        public static Hertz Avg(this IEnumerable<Hertz> HertzValues)
        {

            var sum    = Hertz.Zero;
            var count  = 0;

            foreach (var hertz in HertzValues)
            {
                sum += hertz;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this HertzValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Hertz values.
        /// </summary>
        /// <param name="HertzValues">An enumeration of Hertz values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Hertz> StdDev(this IEnumerable<Hertz>  HertzValues,
                                           Boolean?                 IsSampleData   = null)
        {

            var stdDev = StdDev<Hertz>.From(
                             HertzValues.Select(hertz => hertz.Value),
                             IsSampleData
                         );

            return new StdDev<Hertz>(
                       Hertz.FromHz(stdDev.Mean),
                       Hertz.FromHz(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Hertz value (Hz), the SI unit of frequency.
    /// </summary>
    public readonly struct Hertz : IMetrology<Hertz>
    {

        #region Properties

        /// <summary>
        /// The value of the Hertz.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Hertz.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as kHz.
        /// </summary>
        public Decimal  kHz
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The value as MHz.
        /// </summary>
        public Decimal  MHz
            => Value / 1000000m;

        /// <summary>
        /// The value as GHz.
        /// </summary>
        public Decimal  GHz
            => Value / 1000000000m;


        /// <summary>
        /// The zero value of the Hertz.
        /// </summary>
        public static readonly Hertz Zero = new (0m);

        /// <summary>
        /// The additive identity of Hertz.
        /// </summary>
        public static Hertz AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Hertz (Hz) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of hertz (Hz).</param>
        private Hertz(Decimal Value)
        {
            // Note: We allow negative frequencies, as they can be used
            // to represent phase shifts in signal processing.
            this.Value = Value;
        }

        #endregion


        #region (static) Parse       (Text)

        /// <summary>
        /// Parse the given string as hertz using invariant culture.
        /// Supports optional suffixes "Hz", "kHz", "MHz" and "GHz".
        /// </summary>
        /// <param name="Text">A text representation of hertz.</param>
        public static Hertz Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse       (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as hertz using the given format provider.
        /// Supports optional suffixes "Hz", "kHz", "MHz" and "GHz".
        /// </summary>
        /// <param name="Text">A text representation of hertz.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Hertz Parse(String            Text,
                                  IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var hertz))
                return hertz;

            throw new FormatException($"Invalid text representation of hertz: '{Text}'!");

        }

        #endregion

        #region (static) Parse       (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as hertz using the given format provider.
        /// Supports optional suffixes "Hz", "kHz", "MHz" and "GHz".
        /// </summary>
        /// <param name="Span">A text representation of hertz.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Hertz Parse(ReadOnlySpan<Char>  Span,
                                  IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var hertz))
                return hertz;

            throw new FormatException($"Invalid text representation of hertz: '{Span}'!");

        }

        #endregion

        #region (static) ParseHz     (Text)

        /// <summary>
        /// Parse the given string as hertz.
        /// </summary>
        /// <param name="Text">A text representation of hertz.</param>
        public static Hertz ParseHz(String Text)
        {

            if (TryParseHz(Text, out var hertz))
                return hertz;

            throw new ArgumentException($"Invalid text representation of hertz: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKHz    (Text)

        /// <summary>
        /// Parse the given string as kHz.
        /// </summary>
        /// <param name="Text">A text representation of kHz.</param>
        public static Hertz ParseKHz(String Text)
        {

            if (TryParseKHz(Text, out var hertz))
                return hertz;

            throw new ArgumentException($"Invalid text representation of kHz: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseMHz    (Text)

        /// <summary>
        /// Parse the given string as MHz.
        /// </summary>
        /// <param name="Text">A text representation of MHz.</param>
        public static Hertz ParseMHz(String Text)
        {

            if (TryParseMHz(Text, out var hertz))
                return hertz;

            throw new ArgumentException($"Invalid text representation of MHz: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseGHz    (Text)

        /// <summary>
        /// Parse the given string as GHz.
        /// </summary>
        /// <param name="Text">A text representation of GHz.</param>
        public static Hertz ParseGHz(String Text)
        {

            if (TryParseGHz(Text, out var hertz))
                return hertz;

            throw new ArgumentException($"Invalid text representation of GHz: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) FromHz      (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz FromHz(Decimal  Number,
                                   Int32?   Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz FromHz(Byte    Number,
                                   Int32?  Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromKHz     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz FromKHz(Decimal  Number,
                                    Int32?   Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz FromKHz(Byte    Number,
                                    Int32?  Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromMHz     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into MHz.
        /// </summary>
        /// <param name="Number">A numeric representation of MHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz FromMHz(Decimal  Number,
                                    Int32?   Exponent = null)

            => new (1000000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into MHz.
        /// </summary>
        /// <param name="Number">A numeric representation of MHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz FromMHz(Byte    Number,
                                    Int32?  Exponent = null)

            => new (1000000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromGHz     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into GHz.
        /// </summary>
        /// <param name="Number">A numeric representation of GHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz FromGHz(Decimal  Number,
                                    Int32?   Exponent = null)

            => new (1000000000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into GHz.
        /// </summary>
        /// <param name="Number">A numeric representation of GHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz FromGHz(Byte    Number,
                                    Int32?  Exponent = null)

            => new (1000000000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion


        #region (static) TryParse    (Text)

        /// <summary>
        /// Try to parse the given text as hertz with an optional unit suffix ("Hz", "kHz", "MHz" and "GHz")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of hertz.</param>
        public static Hertz? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var hertz))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParse    (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as hertz with an optional unit suffix ("Hz", "kHz", "MHz" and "GHz")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of hertz.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Hertz? TryParse(String?           Text,
                                      IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var hertz))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParseHz  (Text)

        /// <summary>
        /// Try to parse the given text as hertz.
        /// </summary>
        /// <param name="Text">A text representation of hertz.</param>
        public static Hertz? TryParseHz(String? Text)
        {

            if (TryParseHz(Text, out var hertz))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParseKHz (Text)

        /// <summary>
        /// Try to parse the given text as kiloHertz.
        /// </summary>
        /// <param name="Text">A text representation of kiloHertz.</param>
        public static Hertz? TryParseKHz(String? Text)
        {

            if (TryParseKHz(Text, out var hertz))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParseMHz (Text)

        /// <summary>
        /// Try to parse the given text as a MHz.
        /// </summary>
        /// <param name="Text">A text representation of a MHz.</param>
        public static Hertz? TryParseMHz(String? Text)
        {

            if (TryParseMHz(Text, out var hertz))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParseGHz (Text)

        /// <summary>
        /// Try to parse the given text as a GHz.
        /// </summary>
        /// <param name="Text">A text representation of a GHz.</param>
        public static Hertz? TryParseGHz(String? Text)
        {

            if (TryParseGHz(Text, out var hertz))
                return hertz;

            return null;

        }

        #endregion


        #region (static) TryFromHz   (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryFromHz(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryFromHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryFromHz(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryFromHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryFromKHz  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryFromKHz(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryFromKHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryFromKHz(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryFromKHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryFromMHz  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MHz.
        /// </summary>
        /// <param name="Number">A numeric representation of MHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryFromMHz(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryFromMHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into MHz.
        /// </summary>
        /// <param name="Number">A numeric representation of MHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryFromMHz(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryFromMHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryFromGHz  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into GHz.
        /// </summary>
        /// <param name="Number">A numeric representation of GHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryFromGHz(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryFromGHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into GHz.
        /// </summary>
        /// <param name="Number">A numeric representation of GHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryFromGHz(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryFromGHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }

        #endregion


        #region (static) TryParse    (Text,                 out Hertz)

        /// <summary>
        /// Try to parse the given string as hertz using invariant culture.
        /// Supports optional suffixes "Hz", "kHz", "MHz" and "GHz".
        /// </summary>
        /// <param name="Text">A text representation of hertz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Hertz    Hertz)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Hertz);

        #endregion

        #region (static) TryParse    (Text, FormatProvider, out Hertz)

        /// <summary>
        /// Try to parse the given string as hertz using the given format provider.
        /// Supports optional suffixes "Hz", "kHz", "MHz" and "GHz".
        /// </summary>
        /// <param name="Text">A text representation of hertz.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Hertz                    Hertz)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Hertz);

        #endregion

        #region (static) TryParse    (Span, FormatProvider, out Hertz)

        /// <summary>
        /// Try to parse the given text span as hertz using the given format provider.
        /// Supports optional suffixes "Hz", "kHz", "MHz" and "GHz".
        /// </summary>
        /// <param name="Span">A text representation of hertz.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Hertz           Hertz)
        {

            Hertz = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kHz".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^3].TrimEnd();
            }

            else if (Span.EndsWith("MHz".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 6;
                Span      = Span[..^3].TrimEnd();
            }

            else if (Span.EndsWith("GHz".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 9;
                Span      = Span[..^3].TrimEnd();
            }

            else if (Span.EndsWith("Hz". AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^2].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number | NumberStyles.AllowExponent, // e.g. "1.23e3 Hz"
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Hertz);
            }

            return false;

        }

        #endregion

        #region (static) TryParseHz  (Text,                 out Hertz)

        /// <summary>
        /// Try to parse the given string as hertz using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of hertz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParseHz([NotNullWhen(true)] String?  Text,
                                         out                 Hertz    Hertz)
        {

            Hertz = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number | NumberStyles.AllowExponent,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Hertz);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKHz (Text,                 out Hertz)

        /// <summary>
        /// Try to parse the given string as kHz using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of kHz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParseKHz([NotNullWhen(true)] String?  Text,
                                          out                 Hertz    Hertz)
        {

            Hertz = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number | NumberStyles.AllowExponent,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Hertz);
            }

            return false;

        }

        #endregion

        #region (static) TryParseMHz (Text,                 out Hertz)

        /// <summary>
        /// Try to parse the given string as MHz using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of MHz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParseMHz([NotNullWhen(true)] String?  Text,
                                          out                 Hertz    Hertz)
        {

            Hertz = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number | NumberStyles.AllowExponent,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 6, out Hertz);
            }

            return false;

        }

        #endregion

        #region (static) TryParseGHz (Text,                 out Hertz)

        /// <summary>
        /// Try to parse the given string as GHz using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of GHz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParseGHz([NotNullWhen(true)] String?  Text,
                                          out                 Hertz    Hertz)
        {

            Hertz = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number | NumberStyles.AllowExponent,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 9, out Hertz);
            }

            return false;

        }

        #endregion


        #region (private static) TryCreate(Number, Exponent, out Hertz)

        private static Boolean TryCreate(Decimal    Number,
                                         Int32      Exponent,
                                         out Hertz  Hertz)
        {

            Hertz = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Hertz = Zero;
                return true;
            }

            try
            {
                Hertz = new Hertz(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) TryFromHz   (Number, out Hertz, Exponent = null)

        /// <summary>
        /// Try to convert the given number into hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of hertz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromHz(Byte       Number,
                                        out Hertz  Hertz,
                                        Int32?     Exponent = null)
        {

            Hertz = default;

            return MathHelpers.TryAddExponent(Exponent, 0, out var exponent) &&
                   TryCreate(Number, exponent, out Hertz);

        }


        /// <summary>
        /// Try to convert the given number into hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of hertz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromHz(Decimal    Number,
                                        out Hertz  Hertz,
                                        Int32?     Exponent = null)
        {

            Hertz = default;

            return MathHelpers.TryAddExponent(Exponent, 0, out var exponent) &&
                   TryCreate(Number, exponent, out Hertz);

        }

        #endregion

        #region (static) TryFromKHz  (Number, out Hertz, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of kHz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKHz(Byte       Number,
                                         out Hertz  Hertz,
                                         Int32?     Exponent = null)
        {

            Hertz = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out Hertz);

        }


        /// <summary>
        /// Try to convert the given number into kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of kHz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKHz(Decimal    Number,
                                         out Hertz  Hertz,
                                         Int32?     Exponent = null)
        {

            Hertz = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out Hertz);

        }

        #endregion

        #region (static) TryFromMHz  (Number, out Hertz, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MHz.
        /// </summary>
        /// <param name="Number">A numeric representation of MHz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMHz(Byte       Number,
                                         out Hertz  Hertz,
                                         Int32?     Exponent = null)
        {

            Hertz = default;

            return MathHelpers.TryAddExponent(Exponent, 6, out var exponent) &&
                   TryCreate(Number, exponent, out Hertz);

        }


        /// <summary>
        /// Try to convert the given number into MHz.
        /// </summary>
        /// <param name="Number">A numeric representation of MHz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMHz(Decimal    Number,
                                         out Hertz  Hertz,
                                         Int32?     Exponent = null)
        {

            Hertz = default;

            return MathHelpers.TryAddExponent(Exponent, 6, out var exponent) &&
                   TryCreate(Number, exponent, out Hertz);

        }

        #endregion

        #region (static) TryFromGHz  (Number, out Hertz, Exponent = null)

        /// <summary>
        /// Try to convert the given number into GHz.
        /// </summary>
        /// <param name="Number">A numeric representation of GHz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromGHz(Byte       Number,
                                          out Hertz  Hertz,
                                          Int32?     Exponent = null)
        {

            Hertz = default;

            return MathHelpers.TryAddExponent(Exponent, 9, out var exponent) &&
                   TryCreate(Number, exponent, out Hertz);

        }


        /// <summary>
        /// Try to convert the given number into GHz.
        /// </summary>
        /// <param name="Number">A numeric representation of GHz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromGHz(Decimal    Number,
                                          out Hertz  Hertz,
                                          Int32?     Exponent = null)
        {

            Hertz = default;

            return MathHelpers.TryAddExponent(Exponent, 9, out var exponent) &&
                   TryCreate(Number, exponent, out Hertz);

        }

        #endregion


        #region Operator overloading

        #region Operator == (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A Hertz.</param>
        /// <param name="Hertz2">Another Hertz.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Hertz Hertz1,
                                           Hertz Hertz2)

            => Hertz1.Equals(Hertz2);

        #endregion

        #region Operator != (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A Hertz.</param>
        /// <param name="Hertz2">Another Hertz.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Hertz Hertz1,
                                           Hertz Hertz2)

            => !Hertz1.Equals(Hertz2);

        #endregion

        #region Operator <  (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A Hertz.</param>
        /// <param name="Hertz2">Another Hertz.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Hertz Hertz1,
                                          Hertz Hertz2)

            => Hertz1.CompareTo(Hertz2) < 0;

        #endregion

        #region Operator <= (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A Hertz.</param>
        /// <param name="Hertz2">Another Hertz.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Hertz Hertz1,
                                           Hertz Hertz2)

            => Hertz1.CompareTo(Hertz2) <= 0;

        #endregion

        #region Operator >  (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A Hertz.</param>
        /// <param name="Hertz2">Another Hertz.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Hertz Hertz1,
                                          Hertz Hertz2)

            => Hertz1.CompareTo(Hertz2) > 0;

        #endregion

        #region Operator >= (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A Hertz.</param>
        /// <param name="Hertz2">Another Hertz.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Hertz Hertz1,
                                           Hertz Hertz2)

            => Hertz1.CompareTo(Hertz2) >= 0;

        #endregion

        #region Operator +  (Hertz1, Hertz2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A Hertz.</param>
        /// <param name="Hertz2">Another Hertz.</param>
        public static Hertz operator + (Hertz Hertz1,
                                        Hertz Hertz2)

            => new (Hertz1.Value + Hertz2.Value);

        #endregion

        #region Operator -  (Hertz1, Hertz2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A Hertz.</param>
        /// <param name="Hertz2">Another Hertz.</param>
        public static Hertz operator - (Hertz Hertz1,
                                        Hertz Hertz2)

            => new (Hertz1.Value - Hertz2.Value);

        #endregion


        #region Operator *  (Hertz,  Scalar)

        /// <summary>
        /// Multiplies Hertz with a scalar.
        /// </summary>
        /// <param name="Hertz">A Hertz value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Hertz operator * (Hertz    Hertz,
                                        Decimal  Scalar)

            => new (Hertz.Value * Scalar);

        #endregion

        #region Operator *  (Scalar, Hertz)

        /// <summary>
        /// Multiplies scalar with a Hertz.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Hertz">A Hertz value.</param>
        public static Hertz operator * (Decimal  Scalar,
                                        Hertz    Hertz)

            => new (Scalar * Hertz.Value);

        #endregion

        #region Operator /  (Hertz,  Scalar)

        /// <summary>
        /// Divides Hertz by a scalar.
        /// </summary>
        /// <param name="Hertz">A Hertz value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Hertz operator / (Hertz    Hertz,
                                        Decimal  Scalar)

            => new (Hertz.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Hertz> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Hertz.
        /// </summary>
        /// <param name="Object">A Hertz to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null         => 1,
                   Hertz hertz  => CompareTo(hertz),
                   _            => throw new ArgumentException("The given object is not a Hertz!", nameof(Object))
               };

        #endregion

        #region CompareTo(Hertz)

        /// <summary>
        /// Compares two Hertz.
        /// </summary>
        /// <param name="Hertz">A Hertz to compare with.</param>
        public Int32 CompareTo(Hertz Hertz)

            => Value.CompareTo(Hertz.Value);

        #endregion

        #endregion

        #region IEquatable<Hertz> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Hertz for equality.
        /// </summary>
        /// <param name="Object">A Hertz to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Hertz hertz &&
                   Equals(hertz);

        #endregion

        #region Equals(Hertz)

        /// <summary>
        /// Compares two Hertz for equality.
        /// </summary>
        /// <param name="Hertz">A Hertz to compare with.</param>
        public Boolean Equals(Hertz Hertz)

            => Value.Equals(Hertz.Value);

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
        /// Try to format this Hertz into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("Hz".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Hz".AsSpan()
                       );
            }

            if (Format.Equals("kHz".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kHz,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kHz".AsSpan()
                       );

            if (Format.Equals("MHz".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           MHz,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " MHz".AsSpan()
                       );

            if (Format.Equals("GHz".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           GHz,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " GHz".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " Hz".AsSpan()
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
                String.Equals(Format, "Hz", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} Hz";
            }

            if (String.Equals(Format, "kHz", StringComparison.OrdinalIgnoreCase))
                return $"{kHz.ToString("G", FormatProvider)} kHz";

            if (String.Equals(Format, "MHz", StringComparison.OrdinalIgnoreCase))
                return $"{MHz.ToString("G", FormatProvider)} MHz";

            if (String.Equals(Format, "GHz", StringComparison.OrdinalIgnoreCase))
                return $"{GHz.ToString("G", FormatProvider)} GHz";

            return $"{Value.ToString(Format, FormatProvider)} Hz";

        }

        #endregion

    }

}
