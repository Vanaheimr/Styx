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
    /// Extension methods for Farad (F) values.
    /// </summary>
    public static class FaradExtensions
    {

        #region Sum    (this FaradValues)

        /// <summary>
        /// The sum of the given enumeration of Farad values.
        /// </summary>
        /// <param name="FaradValues">An enumeration of Farad values.</param>
        public static Farad Sum(this IEnumerable<Farad> FaradValues)
        {

            var sum = Farad.Zero;

            foreach (var farad in FaradValues)
                sum += farad;

            return sum;

        }

        #endregion

        #region Avg    (this FaradValues)

        /// <summary>
        /// The average of the given enumeration of Farad values.
        /// </summary>
        /// <param name="FaradValues">An enumeration of Farad values.</param>
        public static Farad Avg(this IEnumerable<Farad> FaradValues)
        {

            var sum    = Farad.Zero;
            var count  = 0;

            foreach (var farad in FaradValues)
            {
                sum += farad;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this FaradValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Farad values.
        /// </summary>
        /// <param name="FaradValues">An enumeration of Farad values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Farad> StdDev(this IEnumerable<Farad>  FaradValues,
                                           Boolean?                 IsSampleData   = null)
        {

            var stdDev = StdDev<Farad>.From(
                             FaradValues.Select(farad => farad.Value),
                             IsSampleData
                         );

            return new StdDev<Farad>(
                       Farad.FromF(stdDev.Mean),
                       Farad.FromF(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Farad value (F), the SI unit of capacitance.
    /// </summary>
    public readonly struct Farad : IMetrology<Farad>
    {

        #region Properties

        /// <summary>
        /// The value of the Farad.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Farad.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// The value as microFarad.
        /// </summary>
        public Decimal  µF
            => Value * 1000000m;

        /// <summary>
        /// The value as nanoFarad.
        /// </summary>
        public Decimal  nF
            => Value * 1000000000m;

        /// <summary>
        /// The value as pikofarad.
        /// </summary>
        public Decimal  pF
            => Value * 1000000000000m;

#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The zero value of the Farad.
        /// </summary>
        public static readonly Farad Zero = new (0m);

        /// <summary>
        /// The additive identity of Farad.
        /// </summary>
        public static Farad AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Farad (F) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of farad (F).</param>
        private Farad(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as farad using invariant culture.
        /// Supports optional suffixes "F", "µF", "nF" and "pF".
        /// </summary>
        /// <param name="Text">A text representation of farad.</param>
        public static Farad Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse      (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as farad using the given format provider.
        /// Supports optional suffixes "F", "µF", "nF" and "pF".
        /// </summary>
        /// <param name="Text">A text representation of farad.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Farad Parse(String            Text,
                                  IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var farad))
                return farad;

            throw new FormatException($"Invalid text representation of farad: '{Text}'!");

        }

        #endregion

        #region (static) Parse      (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as farad using the given format provider.
        /// Supports optional suffixes "F", "µF", "nF" and "pF".
        /// </summary>
        /// <param name="Span">A text representation of farad.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Farad Parse(ReadOnlySpan<Char>  Span,
                                  IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var farad))
                return farad;

            throw new FormatException($"Invalid text representation of farad: '{Span}'!");

        }

        #endregion

        #region (static) ParseF     (Text)

        /// <summary>
        /// Parse the given string as Farad (F).
        /// </summary>
        /// <param name="Text">A text representation of Farad (F).</param>
        public static Farad ParseF(String Text)
        {

            if (TryParseF(Text, out var farad))
                return farad;

            throw new FormatException($"Invalid text representation of Farad (F): '{Text}'!");

        }

        #endregion

        #region (static) ParseµF    (Text)

        /// <summary>
        /// Parse the given string as MicroFarad (µF).
        /// </summary>
        /// <param name="Text">A text representation of MicroFarad (µF).</param>
        public static Farad ParseµF(String Text)
        {

            if (TryParseµF(Text, out var farad))
                return farad;

            throw new FormatException($"Invalid text representation of MicroFarad (µF): '{Text}'!");

        }

        #endregion

        #region (static) ParseNF    (Text)

        /// <summary>
        /// Parse the given string as NanoFarad (nF).
        /// </summary>
        /// <param name="Text">A text representation of NanoFarad (nF).</param>
        public static Farad ParseNF(String Text)
        {

            if (TryParseNF(Text, out var farad))
                return farad;

            throw new FormatException($"Invalid text representation of NanoFarad (nF): '{Text}'!");

        }

        #endregion

        #region (static) ParsePF    (Text)

        /// <summary>
        /// Parse the given string as PikoFarad (pF).
        /// </summary>
        /// <param name="Text">A text representation of PikoFarad (pF).</param>
        public static Farad ParsePF(String Text)
        {

            if (TryParsePF(Text, out var farad))
                return farad;

            throw new FormatException($"Invalid text representation of PikoFarad (pF): '{Text}'!");

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as farad with an optional unit suffix ("F", "µF", "nF" or "pF")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of farad.</param>
        public static Farad? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var farad))
                return farad;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as farad with an optional unit suffix ("F", "µF", "nF" or "pF")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of farad.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Farad? TryParse(String?           Text,
                                      IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var farad))
                return farad;

            return null;

        }

        #endregion

        #region (static) TryParseF  (Text)

        /// <summary>
        /// Try to parse the given text as Farad (F).
        /// </summary>
        /// <param name="Text">A text representation of Farad (F).</param>
        public static Farad? TryParseF(String? Text)
        {

            if (TryParseF(Text, out var farad))
                return farad;

            return null;

        }

        #endregion

        #region (static) TryParseµF (Text)

        /// <summary>
        /// Try to parse the given text as MicroFarad (µF).
        /// </summary>
        /// <param name="Text">A text representation of MicroFarad (µF).</param>
        public static Farad? TryParseµF(String? Text)
        {

            if (TryParseµF(Text, out var farad))
                return farad;

            return null;

        }

        #endregion

        #region (static) TryParseNF (Text)

        /// <summary>
        /// Try to parse the given text as NanoFarad (nF).
        /// </summary>
        /// <param name="Text">A text representation of NanoFarad (nF).</param>
        public static Farad? TryParseNF(String? Text)
        {

            if (TryParseNF(Text, out var farad))
                return farad;

            return null;

        }

        #endregion

        #region (static) TryParsePF (Text)

        /// <summary>
        /// Try to parse the given text as PikoFarad (pF).
        /// </summary>
        /// <param name="Text">A text representation of PikoFarad (pF).</param>
        public static Farad? TryParsePF(String? Text)
        {

            if (TryParsePF(Text, out var farad))
                return farad;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,                 out Farad)

        /// <summary>
        /// Try to parse the given string as farad using invariant culture.
        /// Supports optional suffixes "F", "µF", "nF" and "pF".
        /// </summary>
        /// <param name="Text">A text representation of farad.</param>
        /// <param name="Farad">The parsed Farad.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Farad    Farad)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Farad);

        #endregion

        #region (static) TryParse   (Text, FormatProvider, out Farad)

        /// <summary>
        /// Try to parse the given string as farad using the given format provider.
        /// Supports optional suffixes "F", "µF", "nF" and "pF".
        /// </summary>
        /// <param name="Text">A text representation of farad.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Farad">The parsed Farad.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Farad                    Farad)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Farad);

        #endregion

        #region (static) TryParse   (Span, FormatProvider, out Farad)

        /// <summary>
        /// Try to parse the given text span as farad using the given format provider.
        /// Supports optional suffixes "F", "µF", "nF" and "pF".
        /// </summary>
        /// <param name="Span">A text representation of farad.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Farad">The parsed Farad.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Farad           Farad)
        {

            Farad = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("µF".AsSpan(), StringComparison.Ordinal))
            {
                exponent  = -6;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("nF".AsSpan(), StringComparison.Ordinal))
            {
                exponent  = -9;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("pF".AsSpan(), StringComparison.Ordinal))
            {
                exponent  = -12;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("F".AsSpan(),  StringComparison.Ordinal))
            {
                Span      = Span[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Farad);
            }

            return false;

        }

        #endregion

        #region (static) TryParseF  (Text,                 out Farad)

        /// <summary>
        /// Try to parse the given string as Farad (F).
        /// </summary>
        /// <param name="Text">A text representation of Farad (F).</param>
        /// <param name="Farad">The parsed Farad.</param>
        public static Boolean TryParseF([NotNullWhen(true)] String?  Text,
                                        out                 Farad    Farad)
        {

            Farad = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Farad);
            }

            return false;

        }

        #endregion

        #region (static) TryParseµF (Text,                 out Farad)

        /// <summary>
        /// Try to parse the given string as MicroFarad (µF).
        /// </summary>
        /// <param name="Text">A text representation of MicroFarad (µF).</param>
        /// <param name="Farad">The parsed Farad.</param>
        public static Boolean TryParseµF([NotNullWhen(true)] String?  Text,
                                         out                 Farad    Farad)
        {

            Farad = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -6, out Farad);
            }

            return false;

        }

        #endregion

        #region (static) TryParseNF (Text,                 out Farad)

        /// <summary>
        /// Try to parse the given string as NanoFarad (nF).
        /// </summary>
        /// <param name="Text">A text representation of NanoFarad (nF).</param>
        /// <param name="Farad">The parsed Farad.</param>
        public static Boolean TryParseNF([NotNullWhen(true)] String?  Text,
                                         out                 Farad    Farad)
        {

            Farad = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -9, out Farad);
            }

            return false;

        }

        #endregion

        #region (static) TryParsePF (Text,                 out Farad)

        /// <summary>
        /// Try to parse the given string as PikoFarad (pF).
        /// </summary>
        /// <param name="Text">A text representation of PikoFarad (pF).</param>
        /// <param name="Farad">The parsed Farad.</param>
        public static Boolean TryParsePF([NotNullWhen(true)] String?  Text,
                                         out                 Farad    Farad)
        {

            Farad = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, -12, out Farad);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static Farad Create(Decimal  Number,
                                    Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var farad))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return farad;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Farad)

        private static Boolean TryCreate(Decimal    Number,
                                         Int32      Exponent,
                                         out Farad  Farad)
        {

            Farad = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Farad = Zero;
                return true;
            }

            try
            {
                Farad = new Farad(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) FromF      (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into Farad (F).
        /// </summary>
        /// <param name="Number">A numeric representation of Farad (F).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Farad FromF<TNumber>(TNumber  Number,
                                           Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) FromµF     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into MicroFarad (µF).
        /// </summary>
        /// <param name="Number">A numeric representation of MicroFarad (µF).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Farad FromµF<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 6)
                   );

        #endregion

        #region (static) FromNF     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into NanoFarad (nF).
        /// </summary>
        /// <param name="Number">A numeric representation of NanoFarad (nF).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Farad FromNF<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 9)
                   );

        #endregion

        #region (static) FromPF     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into PikoFarad (pF).
        /// </summary>
        /// <param name="Number">A numeric representation of PikoFarad (pF).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Farad FromPF<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) - 12)
                   );

        #endregion


        #region (static) TryFromF   (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into Farad (F).
        /// </summary>
        /// <param name="Number">A numeric representation of Farad (F).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Farad? TryFromF<TNumber>(TNumber  Number,
                                               Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromF(Number, out var farad, Exponent))
                return farad;

            return null;

        }

        #endregion

        #region (static) TryFromµF  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into MicroFarad (µF).
        /// </summary>
        /// <param name="Number">A numeric representation of MicroFarad (µF).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Farad? TryFromµF<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromµF(Number, out var farad, Exponent))
                return farad;

            return null;

        }

        #endregion

        #region (static) TryFromNF  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into NanoFarad (nF).
        /// </summary>
        /// <param name="Number">A numeric representation of NanoFarad (nF).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Farad? TryFromNF<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromNF(Number, out var farad, Exponent))
                return farad;

            return null;

        }

        #endregion

        #region (static) TryFromPF  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into PikoFarad (pF).
        /// </summary>
        /// <param name="Number">A numeric representation of PikoFarad (pF).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Farad? TryFromPF<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromPF(Number, out var farad, Exponent))
                return farad;

            return null;

        }

        #endregion


        #region (static) TryFromF   (Number, out Farad, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Farad (F).
        /// </summary>
        /// <param name="Number">A numeric representation of Farad (F).</param>
        /// <param name="Farad">The parsed Farad.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromF<TNumber>(TNumber    Number,
                                                out Farad  Farad,
                                                Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Farad = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Farad);
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

        #region (static) TryFromµF  (Number, out Farad, Exponent = null)

        /// <summary>
        /// Try to convert the given number into MicroFarad (µF).
        /// </summary>
        /// <param name="Number">A numeric representation of MicroFarad (µF).</param>
        /// <param name="Farad">The parsed Farad.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromµF<TNumber>(TNumber    Number,
                                                 out Farad  Farad,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Farad = default;

            if (!MathHelpers.TryAddExponent(Exponent, -6, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Farad);
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

        #region (static) TryFromNF  (Number, out Farad, Exponent = null)

        /// <summary>
        /// Try to convert the given number into NanoFarad (nF).
        /// </summary>
        /// <param name="Number">A numeric representation of NanoFarad (nF).</param>
        /// <param name="Farad">The parsed Farad.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromNF<TNumber>(TNumber    Number,
                                                 out Farad  Farad,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Farad = default;

            if (!MathHelpers.TryAddExponent(Exponent, -9, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Farad);
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

        #region (static) TryFromPF  (Number, out Farad, Exponent = null)

        /// <summary>
        /// Try to convert the given number into PikoFarad (pF).
        /// </summary>
        /// <param name="Number">A numeric representation of PikoFarad (pF).</param>
        /// <param name="Farad">The parsed Farad.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromPF<TNumber>(TNumber    Number,
                                                 out Farad  Farad,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Farad = default;

            if (!MathHelpers.TryAddExponent(Exponent, -12, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Farad);
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

        #region Operator == (Farad1, Farad2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Farad1">A Farad value.</param>
        /// <param name="Farad2">Another Farad value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Farad Farad1,
                                           Farad Farad2)

            => Farad1.Equals(Farad2);

        #endregion

        #region Operator != (Farad1, Farad2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Farad1">A Farad value.</param>
        /// <param name="Farad2">Another Farad value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Farad Farad1,
                                           Farad Farad2)

            => !Farad1.Equals(Farad2);

        #endregion

        #region Operator <  (Farad1, Farad2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Farad1">A Farad value.</param>
        /// <param name="Farad2">Another Farad value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Farad Farad1,
                                          Farad Farad2)

            => Farad1.CompareTo(Farad2) < 0;

        #endregion

        #region Operator <= (Farad1, Farad2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Farad1">A Farad value.</param>
        /// <param name="Farad2">Another Farad value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Farad Farad1,
                                           Farad Farad2)

            => Farad1.CompareTo(Farad2) <= 0;

        #endregion

        #region Operator >  (Farad1, Farad2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Farad1">A Farad value.</param>
        /// <param name="Farad2">Another Farad value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Farad Farad1,
                                          Farad Farad2)

            => Farad1.CompareTo(Farad2) > 0;

        #endregion

        #region Operator >= (Farad1, Farad2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Farad1">A Farad value.</param>
        /// <param name="Farad2">Another Farad value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Farad Farad1,
                                           Farad Farad2)

            => Farad1.CompareTo(Farad2) >= 0;

        #endregion

        #region Operator +  (Farad1, Farad2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Farad1">A Farad value.</param>
        /// <param name="Farad2">Another Farad value.</param>
        public static Farad operator + (Farad Farad1,
                                          Farad Farad2)

            => new (Farad1.Value + Farad2.Value);

        #endregion

        #region Operator -  (Farad1, Farad2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Farad1">A Farad value.</param>
        /// <param name="Farad2">Another Farad value.</param>
        public static Farad operator - (Farad Farad1,
                                          Farad Farad2)

            => new (Farad1.Value - Farad2.Value);

        #endregion


        #region Operator *  (Farad,  Scalar)

        /// <summary>
        /// Multiplies a Farad with a scalar.
        /// </summary>
        /// <param name="Farad">A Farad value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Farad operator * (Farad  Farad,
                                          Decimal  Scalar)

            => new (Farad.Value * Scalar);

        #endregion

        #region Operator *  (Scalar, Farad)

        /// <summary>
        /// Multiplies a scalar with a Farad.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Farad">A Farad value.</param>
        public static Farad operator * (Decimal  Scalar,
                                          Farad  Farad)

            => new (Scalar * Farad.Value);

        #endregion

        #region Operator /  (Farad,  Scalar)

        /// <summary>
        /// Divides a Farad with a scalar.
        /// </summary>
        /// <param name="Farad">A Farad value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Farad operator / (Farad  Farad,
                                          Decimal  Scalar)

            => new (Farad.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Farad> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Farad.
        /// </summary>
        /// <param name="Object">A Farad to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null             => 1,
                   Farad farad  => CompareTo(farad),
                   _                => throw new ArgumentException("The given object is not a Farad!", nameof(Object))
               };

        #endregion

        #region CompareTo(Farad)

        /// <summary>
        /// Compares two Farad.
        /// </summary>
        /// <param name="Farad">A Farad to compare with.</param>
        public Int32 CompareTo(Farad Farad)

            => Value.CompareTo(Farad.Value);

        #endregion

        #endregion

        #region IEquatable<Farad> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Farad for equality.
        /// </summary>
        /// <param name="Object">A Farad to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Farad farad &&
                   Equals(farad);

        #endregion

        #region Equals(Farad)

        /// <summary>
        /// Compares two Farad for equality.
        /// </summary>
        /// <param name="Farad">A Farad to compare with.</param>
        public Boolean Equals(Farad Farad)

            => Value.Equals(Farad.Value);

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
        /// Try to format this Farad into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("F".AsSpan(), StringComparison.Ordinal))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " F".AsSpan()
                       );
            }

            if (Format.Equals("µF".AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           µF,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " µF".AsSpan()
                       );

            if (Format.Equals("nF".AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           nF,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " nF".AsSpan()
                       );

            if (Format.Equals("pF".AsSpan(), StringComparison.Ordinal))
                return TryFormatWithSuffix(
                           pF,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " pF".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " F".AsSpan()
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
                String.Equals(Format, "F",  StringComparison.Ordinal))
            {
                return $"{Value.ToString("G", FormatProvider)} F";
            }

            if (String.Equals(Format, "µF", StringComparison.Ordinal))
                return $"{µF.ToString("G", FormatProvider)} µF";

            if (String.Equals(Format, "nF", StringComparison.Ordinal))
                return $"{nF.ToString("G", FormatProvider)} nF";

            if (String.Equals(Format, "pF", StringComparison.Ordinal))
                return $"{pF.ToString("G", FormatProvider)} pF";

            return $"{Value.ToString(Format, FormatProvider)} F";

        }

        #endregion

    }

}
