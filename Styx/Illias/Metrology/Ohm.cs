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
    /// Extension methods for Ohm.
    /// </summary>
    public static class OhmExtensions
    {

        #region Sum    (this OhmValues)

        /// <summary>
        /// The sum of the given enumeration of Ohm values.
        /// </summary>
        /// <param name="OhmValues">An enumeration of Ohm values.</param>
        public static Ohm Sum(this IEnumerable<Ohm> OhmValues)
        {

            var sum = Ohm.Zero;

            foreach (var ohm in OhmValues)
                sum += ohm;

            return sum;

        }

        #endregion

        #region Avg    (this OhmValues)

        /// <summary>
        /// The average of the given enumeration of Ohm values.
        /// </summary>
        /// <param name="OhmValues">An enumeration of Ohm values.</param>
        public static Ohm Avg(this IEnumerable<Ohm> OhmValues)
        {

            var sum    = Ohm.Zero;
            var count  = 0;

            foreach (var ohm in OhmValues)
            {
                sum += ohm;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this OhmValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Ohm values.
        /// </summary>
        /// <param name="OhmValues">An enumeration of Ohm values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Ohm> StdDev(this IEnumerable<Ohm>  OhmValues,
                                         Boolean?               IsSampleData   = null)
        {

            var stdDev = StdDev<Ohm>.From(
                             OhmValues.Select(ohm => ohm.Value),
                             IsSampleData
                         );

            return new StdDev<Ohm>(
                       Ohm.FromOhm(stdDev.Mean),
                       Ohm.FromOhm(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Ohm value (Ω), the SI unit of electrical resistance.
    /// </summary>
    public readonly struct Ohm : IMetrology<Ohm>
    {

        #region Properties

        /// <summary>
        /// The value of the Ohm.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Ohm.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as kΩ.
        /// </summary>
        public Decimal  kΩ
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The value as MΩ.
        /// </summary>
        public Decimal  MΩ
            => Value / 1000000m;


        /// <summary>
        /// The zero value of the Ohm.
        /// </summary>
        public static readonly Ohm Zero = new (0m);

        /// <summary>
        /// The additive identity of Ohm.
        /// </summary>
        public static Ohm AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Ohm (Ω) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of ohm (Ω).</param>
        private Ohm(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse        (Text)

        /// <summary>
        /// Parse the given string as ohms using invariant culture.
        /// Supports optional suffixes "Ω", "kΩ" and "MΩ".
        /// </summary>
        /// <param name="Text">A text representation of ohms.</param>
        public static Ohm Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse        (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as ohms using the given format provider.
        /// Supports optional suffixes "Ω", "kΩ" and "MΩ".
        /// </summary>
        /// <param name="Text">A text representation of ohms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Ohm Parse(String            Text,
                                IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var ohm))
                return ohm;

            throw new FormatException($"Invalid text representation of ohms: '{Text}'!");

        }

        #endregion

        #region (static) Parse        (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as ohms using the given format provider.
        /// Supports optional suffixes "Ω", "kΩ" and "MΩ".
        /// </summary>
        /// <param name="Span">A text representation of ohms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Ohm Parse(ReadOnlySpan<Char>  Span,
                                IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var ohm))
                return ohm;

            throw new FormatException($"Invalid text representation of ohm: '{Span}'!");

        }

        #endregion

        #region (static) ParseOhm     (Text)

        /// <summary>
        /// Parse the given string as ohms.
        /// </summary>
        /// <param name="Text">A text representation of ohms.</param>
        public static Ohm ParseOhm(String Text)
        {

            if (TryParseOhm(Text, out var ohm))
                return ohm;

            throw new ArgumentException($"Invalid text representation of ohm: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKOhm    (Text)

        /// <summary>
        /// Parse the given string as kOhms.
        /// </summary>
        /// <param name="Text">A text representation of kOhms.</param>
        public static Ohm ParseKOhm(String Text)
        {

            if (TryParseKOhm(Text, out var ohm))
                return ohm;

            throw new ArgumentException($"Invalid text representation of kOhm: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseMOhm    (Text)

        /// <summary>
        /// Parse the given string as MOhms.
        /// </summary>
        /// <param name="Text">A text representation of MOhms.</param>
        public static Ohm ParseMOhm(String Text)
        {

            if (TryParseMOhm(Text, out var ohm))
                return ohm;

            throw new ArgumentException($"Invalid text representation of MOhm: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) FromOhm      (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into ohms.
        /// </summary>
        /// <param name="Number">A numeric representation of ohms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm FromOhm(Decimal  Number,
                                  Int32?   Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into ohms.
        /// </summary>
        /// <param name="Number">A numeric representation of ohms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm FromOhm(Byte    Number,
                                  Int32?  Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromKOhm     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into kOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of kOhms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm FromKOhm(Decimal  Number,
                                    Int32?   Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into kOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of kOhms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm FromKOhm(Byte    Number,
                                    Int32?  Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromMOhm     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into MOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of MOhms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm FromMOhm(Decimal  Number,
                                    Int32?   Exponent = null)

            => new (1000000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into MOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of MOhms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm FromMOhm(Byte    Number,
                                    Int32?  Exponent = null)

            => new (1000000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion


        #region (static) TryParse     (Text)

        /// <summary>
        /// Try to parse the given text as ohms with an optional unit suffix ("Ω", "kΩ" and "MΩ")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of ohms.</param>
        public static Ohm? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var ohm))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryParse     (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as ohms with an optional unit suffix ("Ω", "kΩ" and "MΩ")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of ohms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Ohm? TryParse(String?           Text,
                                      IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var ohm))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryParseOhm  (Text)

        /// <summary>
        /// Try to parse the given text as ohms.
        /// </summary>
        /// <param name="Text">A text representation of ohms.</param>
        public static Ohm? TryParseOhm(String? Text)
        {

            if (TryParseOhm(Text, out var ohm))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryParseKOhm (Text)

        /// <summary>
        /// Try to parse the given text as kiloOhms.
        /// </summary>
        /// <param name="Text">A text representation of kiloOhms.</param>
        public static Ohm? TryParseKOhm(String? Text)
        {

            if (TryParseKOhm(Text, out var ohm))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryParseMOhm (Text)

        /// <summary>
        /// Try to parse the given text as a MOhms.
        /// </summary>
        /// <param name="Text">A text representation of a MOhms.</param>
        public static Ohm? TryParseMOhm(String? Text)
        {

            if (TryParseMOhm(Text, out var ohm))
                return ohm;

            return null;

        }

        #endregion


        #region (static) TryFromOhm   (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into ohms.
        /// </summary>
        /// <param name="Number">A numeric representation of ohms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryFromOhm(Decimal  Number,
                                      Int32?   Exponent = null)
        {

            if (TryFromOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into ohms.
        /// </summary>
        /// <param name="Number">A numeric representation of ohms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryFromOhm(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryFromOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryFromKOhm  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of kOhms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryFromKOhm(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryFromKOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into kOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of kOhms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryFromKOhm(Byte    Number,
                                       Int32?  Exponent = null)
        {

            if (TryFromKOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryFromMOhm  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of MOhms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryFromMOhm(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryFromMOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into MOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of MOhms.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryFromMOhm(Byte    Number,
                                       Int32?  Exponent = null)
        {

            if (TryFromMOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }

        #endregion


        #region (static) TryParse     (Text,                 out Ohm)

        /// <summary>
        /// Try to parse the given string as ohms using invariant culture.
        /// Supports optional suffixes "Ω", "kΩ" and "MΩ".
        /// </summary>
        /// <param name="Text">A text representation of ohms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Ohm      Ohm)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Ohm);

        #endregion

        #region (static) TryParse     (Text, FormatProvider, out Ohm)

        /// <summary>
        /// Try to parse the given string as ohms using the given format provider.
        /// Supports optional suffixes "Ω", "kΩ" and "MΩ".
        /// </summary>
        /// <param name="Text">A text representation of ohms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Ohm                      Ohm)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Ohm);

        #endregion

        #region (static) TryParse     (Span, FormatProvider, out Ohm)

        /// <summary>
        /// Try to parse the given text span as ohms using the given format provider.
        /// Supports optional suffixes "Ω", "kΩ" and "MΩ".
        /// </summary>
        /// <param name="Span">A text representation of ohms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Ohm             Ohm)
        {

            Ohm = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kOhm".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^3].TrimEnd();
            }

            else if (Span.EndsWith("MOhm".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 6;
                Span      = Span[..^3].TrimEnd();
            }

            else if (Span.EndsWith("GOhm".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 9;
                Span      = Span[..^3].TrimEnd();
            }

            else if (Span.EndsWith("Ohm". AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^2].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number, // e.g. "1.23e3 Ohm"
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Ohm);
            }

            return false;

        }

        #endregion

        #region (static) TryParseOhm  (Text,                 out Ohm)

        /// <summary>
        /// Try to parse the given string as ohms using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of ohms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParseOhm([NotNullWhen(true)] String?  Text,
                                          out                 Ohm      Ohm)
        {

            Ohm = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Ohm);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKOhm (Text,                 out Ohm)

        /// <summary>
        /// Try to parse the given string as kOhms using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of kOhms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParseKOhm([NotNullWhen(true)] String?  Text,
                                           out                 Ohm      Ohm)
        {

            Ohm = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Ohm);
            }

            return false;

        }

        #endregion

        #region (static) TryParseMOhm (Text,                 out Ohm)

        /// <summary>
        /// Try to parse the given string as MOhms using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of MOhms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParseMOhm([NotNullWhen(true)] String?  Text,
                                           out                 Ohm      Ohm)
        {

            Ohm = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 6, out Ohm);
            }

            return false;

        }

        #endregion


        #region (private static) TryCreate(Number, Exponent, out Ohm)

        private static Boolean TryCreate(Decimal  Number,
                                         Int32    Exponent,
                                         out Ohm  Ohm)
        {

            Ohm = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Ohm = Zero;
                return true;
            }

            try
            {
                Ohm = new Ohm(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) TryFromOhm   (Number, out Ohm, Exponent = null)

        /// <summary>
        /// Try to convert the given number into ohms.
        /// </summary>
        /// <param name="Number">A numeric representation of ohms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromOhm(Byte    Number,
                                        out Ohm  Ohm,
                                        Int32?   Exponent = null)
        {

            Ohm = default;

            return MathHelpers.TryAddExponent(Exponent, 0, out var exponent) &&
                   TryCreate(Number, exponent, out Ohm);

        }


        /// <summary>
        /// Try to convert the given number into ohms.
        /// </summary>
        /// <param name="Number">A numeric representation of ohms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromOhm(Decimal  Number,
                                        out Ohm   Ohm,
                                        Int32?    Exponent = null)
        {

            Ohm = default;

            return MathHelpers.TryAddExponent(Exponent, 0, out var exponent) &&
                   TryCreate(Number, exponent, out Ohm);

        }

        #endregion

        #region (static) TryFromKOhm  (Number, out Ohm, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of kOhms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKOhm(Byte    Number,
                                         out Ohm  Ohm,
                                         Int32?   Exponent = null)
        {

            Ohm = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out Ohm);

        }


        /// <summary>
        /// Try to convert the given number into kOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of kOhms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKOhm(Decimal  Number,
                                         out Ohm   Ohm,
                                         Int32?    Exponent = null)
        {

            Ohm = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out Ohm);

        }

        #endregion

        #region (static) TryFromMOhm  (Number, out Ohm, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of MOhms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMOhm(Byte    Number,
                                         out Ohm  Ohm,
                                         Int32?   Exponent = null)
        {

            Ohm = default;

            return MathHelpers.TryAddExponent(Exponent, 6, out var exponent) &&
                   TryCreate(Number, exponent, out Ohm);

        }


        /// <summary>
        /// Try to convert the given number into MOhms.
        /// </summary>
        /// <param name="Number">A numeric representation of MOhms.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMOhm(Decimal  Number,
                                         out Ohm   Ohm,
                                         Int32?    Exponent = null)
        {

            Ohm = default;

            return MathHelpers.TryAddExponent(Exponent, 6, out var exponent) &&
                   TryCreate(Number, exponent, out Ohm);

        }

        #endregion


        #region Operator overloading

        #region Operator == (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">A Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Ohm Ohm1,
                                           Ohm Ohm2)

            => Ohm1.Equals(Ohm2);

        #endregion

        #region Operator != (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">A Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Ohm Ohm1,
                                           Ohm Ohm2)

            => !Ohm1.Equals(Ohm2);

        #endregion

        #region Operator <  (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">A Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Ohm Ohm1,
                                          Ohm Ohm2)

            => Ohm1.CompareTo(Ohm2) < 0;

        #endregion

        #region Operator <= (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">A Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Ohm Ohm1,
                                           Ohm Ohm2)

            => Ohm1.CompareTo(Ohm2) <= 0;

        #endregion

        #region Operator >  (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">A Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Ohm Ohm1,
                                          Ohm Ohm2)

            => Ohm1.CompareTo(Ohm2) > 0;

        #endregion

        #region Operator >= (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">A Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Ohm Ohm1,
                                           Ohm Ohm2)

            => Ohm1.CompareTo(Ohm2) >= 0;

        #endregion

        #region Operator +  (Ohm1, Ohm2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Ohm1">A Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        public static Ohm operator + (Ohm Ohm1,
                                        Ohm Ohm2)

            => new (Ohm1.Value + Ohm2.Value);

        #endregion

        #region Operator -  (Ohm1, Ohm2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Ohm1">A Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        public static Ohm operator - (Ohm Ohm1,
                                        Ohm Ohm2)

            => new (Ohm1.Value - Ohm2.Value);

        #endregion


        #region Operator *  (Ohm,  Scalar)

        /// <summary>
        /// Multiplies Ohm with a scalar.
        /// </summary>
        /// <param name="Ohm">A Ohm value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Ohm operator * (Ohm      Ohm,
                                      Decimal  Scalar)

            => new (Ohm.Value * Scalar);

        #endregion

        #region Operator *  (Scalar, Ohm)

        /// <summary>
        /// Multiplies scalar with a Ohm.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Ohm">A Ohm value.</param>
        public static Ohm operator * (Decimal  Scalar,
                                      Ohm      Ohm)

            => new (Scalar * Ohm.Value);

        #endregion

        #region Operator /  (Ohm,  Scalar)

        /// <summary>
        /// Divides Ohm by a scalar.
        /// </summary>
        /// <param name="Ohm">A Ohm value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Ohm operator / (Ohm      Ohm,
                                      Decimal  Scalar)

            => new (Ohm.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Ohm> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Ohm.
        /// </summary>
        /// <param name="Object">A Ohm to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null     => 1,
                   Ohm ohm  => CompareTo(ohm),
                   _        => throw new ArgumentException("The given object is not a Ohm!", nameof(Object))
               };

        #endregion

        #region CompareTo(Ohm)

        /// <summary>
        /// Compares two Ohm.
        /// </summary>
        /// <param name="Ohm">A Ohm to compare with.</param>
        public Int32 CompareTo(Ohm Ohm)

            => Value.CompareTo(Ohm.Value);

        #endregion

        #endregion

        #region IEquatable<Ohm> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Ohm for equality.
        /// </summary>
        /// <param name="Object">A Ohm to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Ohm ohm &&
                   Equals(ohm);

        #endregion

        #region Equals(Ohm)

        /// <summary>
        /// Compares two Ohm for equality.
        /// </summary>
        /// <param name="Ohm">A Ohm to compare with.</param>
        public Boolean Equals(Ohm Ohm)

            => Value.Equals(Ohm.Value);

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
        /// Try to format this Ohm into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("Ω".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " Ω".AsSpan()
                       );
            }

            if (Format.Equals("kΩ".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kΩ,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kΩ".AsSpan()
                       );

            if (Format.Equals("MΩ".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           MΩ,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " MΩ".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " Ω".AsSpan()
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
                String.Equals(Format, "Ω", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} Ω";
            }

            if (String.Equals(Format, "kΩ", StringComparison.OrdinalIgnoreCase))
                return $"{kΩ.ToString("G", FormatProvider)} kΩ";

            if (String.Equals(Format, "MΩ", StringComparison.OrdinalIgnoreCase))
                return $"{MΩ.ToString("G", FormatProvider)} MΩ";

            return $"{Value.ToString(Format, FormatProvider)} Ohm";

        }

        #endregion

    }

}
