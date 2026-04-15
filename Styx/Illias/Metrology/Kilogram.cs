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
    /// Extension methods for kilogram (kg) values.
    /// </summary>
    public static class KilogramExtensions
    {

        #region Sum    (this KilogramValues)

        /// <summary>
        /// The sum of the given enumeration of Kilogram values.
        /// </summary>
        /// <param name="KilogramValues">An enumeration of Kilogram values.</param>
        public static Kilogram Sum(this IEnumerable<Kilogram> KilogramValues)
        {

            var sum = Kilogram.Zero;

            foreach (var kilogram in KilogramValues)
                sum += kilogram;

            return sum;

        }

        #endregion

        #region Avg    (this KilogramValues)

        /// <summary>
        /// The average of the given enumeration of Kilogram values.
        /// </summary>
        /// <param name="KilogramValues">An enumeration of Kilogram values.</param>
        public static Kilogram Avg(this IEnumerable<Kilogram> KilogramValues)
        {

            var sum    = Kilogram.Zero;
            var count  = 0;

            foreach (var kilogram in KilogramValues)
            {
                sum += kilogram;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this KilogramValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Kilogram values.
        /// </summary>
        /// <param name="KilogramValues">An enumeration of Kilogram values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Kilogram> StdDev(this IEnumerable<Kilogram>  KilogramValues,
                                              Boolean?                    IsSampleData   = null)
        {

            var stdDev = StdDev<Kilogram>.From(
                             KilogramValues.Select(ampere => ampere.Value),
                             IsSampleData
                         );

            return new StdDev<Kilogram>(
                       Kilogram.FromKG(stdDev.Mean),
                       Kilogram.FromKG(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A kilogram value (kg), the SI unit of mass.
    /// </summary>
    public readonly struct Kilogram : IMetrology<Kilogram>
    {

        #region Properties

        /// <summary>
        /// The value of the kilogram.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the kilogram.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as grams.
        /// </summary>
        public Decimal  g
            => Value * 1000m;
#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The zero value of a kilogram.
        /// </summary>
        public static readonly Kilogram Zero = new (0m);

        /// <summary>
        /// The additive identity of Kilogram.
        /// </summary>
        public static Kilogram AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new kilogram (kg) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of kilograms (kg).</param>
        private Kilogram(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as kilograms using invariant culture.
        /// Supports optional suffixes "kg" and "g".
        /// </summary>
        /// <param name="Text">A text representation of kilograms.</param>
        public static Kilogram Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse      (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as kilograms using the given format provider.
        /// Supports optional suffixes "kg" and "g".
        /// </summary>
        /// <param name="Text">A text representation of kilograms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Kilogram Parse(String            Text,
                                     IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var kilogram))
                return kilogram;

            throw new FormatException($"Invalid text representation of kilograms: '{Text}'!");

        }

        #endregion

        #region (static) Parse      (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as kilograms using the given format provider.
        /// Supports optional suffixes "kg" and "g".
        /// </summary>
        /// <param name="Span">A text representation of kilograms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Kilogram Parse(ReadOnlySpan<Char>  Span,
                                     IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var kilogram))
                return kilogram;

            throw new FormatException($"Invalid text representation of kilograms: '{Span}'!");

        }

        #endregion

        #region (static) ParseG     (Text)

        /// <summary>
        /// Parse the given string as grams (G).
        /// </summary>
        /// <param name="Text">A text representation of grams (G).</param>
        public static Kilogram ParseGram(String Text)
        {

            if (TryParseG(Text, out var kilogram))
                return kilogram;

            throw new ArgumentException($"Invalid text representation of grams (G): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKG    (Text)

        /// <summary>
        /// Parse the given string as kilograms.
        /// </summary>
        /// <param name="Text">A text representation of kilograms.</param>
        public static Kilogram ParseKG(String Text)
        {

            if (TryParseKG(Text, out var kilogram))
                return kilogram;

            throw new ArgumentException($"Invalid text representation of kilograms: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as kilograms with an optional unit suffix ("kg" and "g")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of kilograms.</param>
        public static Kilogram? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var kilogram))
                return kilogram;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as kilograms with an optional unit suffix ("kg" and "g")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of kilograms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Kilogram? TryParse(String?           Text,
                                         IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var kilogram))
                return kilogram;

            return null;

        }

        #endregion

        #region (static) TryParseG  (Text)

        /// <summary>
        /// Try to parse the given text as grams (G).
        /// </summary>
        /// <param name="Text">A text representation of grams (G).</param>
        public static Kilogram? TryParseG(String? Text)
        {

            if (TryParseG(Text, out var kilogram))
                return kilogram;

            return null;

        }

        #endregion

        #region (static) TryParseKG (Text)

        /// <summary>
        /// Try to parse the given text as kilograms (kg).
        /// </summary>
        /// <param name="Text">A text representation of kilograms (kg).</param>
        public static Kilogram? TryParseKG(String? Text)
        {

            if (TryParseKG(Text, out var kilogram))
                return kilogram;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,                 out Kilogram)

        /// <summary>
        /// Try to parse the given string as kilograms using invariant culture.
        /// Supports optional suffixes "kg" and "g".
        /// </summary>
        /// <param name="Text">A text representation of kilograms.</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?   Text,
                                       out                 Kilogram  Kilogram)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Kilogram);

        #endregion

        #region (static) TryParse   (Text, FormatProvider, out Kilogram)

        /// <summary>
        /// Try to parse the given string as kilograms using the given format provider.
        /// Supports optional suffixes "kg" and "g".
        /// </summary>
        /// <param name="Text">A text representation of kilograms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Kilogram                 Kilogram)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Kilogram);

        #endregion

        #region (static) TryParse   (Span, FormatProvider, out Kilogram)

        /// <summary>
        /// Try to parse the given text span as kilograms using the given format provider.
        /// Supports optional suffixes "kg" and "g".
        /// </summary>
        /// <param name="Span">A text representation of kilograms.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Kilogram        Kilogram)
        {

            Kilogram = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kg".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 0;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("g".AsSpan(),  StringComparison.OrdinalIgnoreCase))
            {
                exponent  = -3;
                Span      = Span[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Kilogram);
            }

            return false;

        }

        #endregion

        #region (static) TryParseG  (Text,                 out Kilogram)

        /// <summary>
        /// Parse the given string as a grams (G).
        /// </summary>
        /// <param name="Text">A text representation of a grams (G).</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        public static Boolean TryParseG([NotNullWhen(true)] String?   Text,
                                        out                 Kilogram  Kilogram)
        {

            Kilogram = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Kilogram);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKG (Text,                 out Kilogram)

        /// <summary>
        /// Parse the given string as a kilograms (kg).
        /// </summary>
        /// <param name="Text">A text representation of a kilograms (kg).</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        public static Boolean TryParseKG([NotNullWhen(true)] String?   Text,
                                         out                 Kilogram  Kilogram)
        {

            Kilogram = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Kilogram);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static Kilogram Create(Decimal  Number,
                                       Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var kilogram))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return kilogram;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Kilogram)

        private static Boolean TryCreate(Decimal       Number,
                                         Int32         Exponent,
                                         out Kilogram  Kilogram)
        {

            Kilogram = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Kilogram = Zero;
                return true;
            }

            try
            {
                Kilogram = new Kilogram(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) FromG      (Number,               Exponent = null)

        /// <summary>
        /// Convert the given number into grams (G).
        /// </summary>
        /// <param name="Number">A numeric representation of grams (G).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram FromG<TNumber>(TNumber  Number,
                                              Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) FromKG     (Number,               Exponent = null)

        /// <summary>
        /// Convert the given number into kilograms (kg).
        /// </summary>
        /// <param name="Number">A numeric representation of kilograms (kg).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram FromKG<TNumber>(TNumber  Number,
                                               Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 3)
                   );

        #endregion


        #region (static) TryFromG   (Number,               Exponent = null)

        /// <summary>
        /// Try to convert the given number into grams (G).
        /// </summary>
        /// <param name="Number">A numeric representation of grams (G).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram? TryFromG<TNumber>(TNumber  Number,
                                                  Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromG(Number, out var kilogram, Exponent))
                return kilogram;

            return null;

        }

        #endregion

        #region (static) TryFromKG  (Number,               Exponent = null)

        /// <summary>
        /// Try to convert the given number into kilograms (kg).
        /// </summary>
        /// <param name="Number">A numeric representation of kilograms (kg).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram? TryFromKG<TNumber>(TNumber  Number,
                                                   Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromKG(Number, out var kilogram, Exponent))
                return kilogram;

            return null;

        }

        #endregion


        #region (static) TryFromG   (Number, out Kilogram, Exponent = null)

        /// <summary>
        /// Try to convert the given number into grams (G).
        /// </summary>
        /// <param name="Number">A numeric representation of grams (G).</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromG<TNumber>(TNumber       Number,
                                                out Kilogram  Kilogram,
                                                Int32?        Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Kilogram = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Kilogram);
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

        #region (static) TryFromKG  (Number, out Kilogram, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kilograms (kg).
        /// </summary>
        /// <param name="Number">A numeric representation of kilograms (kg).</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKG<TNumber>(TNumber       Number,
                                                 out Kilogram  Kilogram,
                                                 Int32?        Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Kilogram = default;

            if (!MathHelpers.TryAddExponent(Exponent, 3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Kilogram);
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


        #region Conversions to/from metric tonnes

        /// <summary>
        /// Convert the current value to its equivalent in metric tonnes.
        /// </summary>
        public Tonne ToTonne()
            => Tonne.FromT(Value / 1000m);

        /// <summary>
        /// Convert the given metric tonne value to its equivalent in kilograms.
        /// </summary>
        /// <param name="Tonne">A metric tonne value.</param>
        public static Kilogram FromTonne(Tonne Tonne)
            => FromKG(Tonne.Value * 1000m);

        #endregion


        #region Operator overloading

        #region Operator == (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => Kilogram1.Equals(Kilogram2);

        #endregion

        #region Operator != (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => !Kilogram1.Equals(Kilogram2);

        #endregion

        #region Operator <  (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Kilogram Kilogram1,
                                          Kilogram Kilogram2)

            => Kilogram1.CompareTo(Kilogram2) < 0;

        #endregion

        #region Operator <= (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => Kilogram1.CompareTo(Kilogram2) <= 0;

        #endregion

        #region Operator >  (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Kilogram Kilogram1,
                                          Kilogram Kilogram2)

            => Kilogram1.CompareTo(Kilogram2) > 0;

        #endregion

        #region Operator >= (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => Kilogram1.CompareTo(Kilogram2) >= 0;

        #endregion

        #region Operator +  (Kilogram1, Kilogram2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        public static Kilogram operator + (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => new (Kilogram1.Value + Kilogram2.Value);

        #endregion

        #region Operator -  (Kilogram1, Kilogram2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        public static Kilogram operator - (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => new (Kilogram1.Value - Kilogram2.Value);

        #endregion


        #region Operator *  (Kilogram,  Scalar)

        /// <summary>
        /// Multiplies a Kilogram with a scalar.
        /// </summary>
        /// <param name="Kilogram">A Kilogram value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Kilogram operator * (Kilogram  Kilogram,
                                           Decimal   Scalar)

            => new (Kilogram.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,    Kilogram)

        /// <summary>
        /// Multiplies a scalar with a Kilogram.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Kilogram">A Kilogram value.</param>
        public static Kilogram operator * (Decimal   Scalar,
                                           Kilogram  Kilogram)

            => new (Scalar * Kilogram.Value);

        #endregion

        #region Operator /  (Kilogram,  Scalar)

        /// <summary>
        /// Divides a Kilogram with a scalar.
        /// </summary>
        /// <param name="Kilogram">A Kilogram value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Kilogram operator / (Kilogram  Kilogram,
                                           Decimal   Scalar)

            => new (Kilogram.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Kilogram> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two kilograms.
        /// </summary>
        /// <param name="Object">A kilogram to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null               => 1,
                   Kilogram kilogram  => CompareTo(kilogram),
                   _                  => throw new ArgumentException("The given object is not a Kilogram!", nameof(Object))
               };

        #endregion

        #region CompareTo(Kilogram)

        /// <summary>
        /// Compares two kilograms.
        /// </summary>
        /// <param name="Kilogram">A Kilogram to compare with.</param>
        public Int32 CompareTo(Kilogram Kilogram)

            => Value.CompareTo(Kilogram.Value);

        #endregion

        #endregion

        #region IEquatable<Kilogram> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two kilograms for equality.
        /// </summary>
        /// <param name="Object">A kilogram to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Kilogram kilogram &&
                   Equals(kilogram);

        #endregion

        #region Equals(Kilogram)

        /// <summary>
        /// Compares two kilograms for equality.
        /// </summary>
        /// <param name="Kilogram">A kilogram to compare with.</param>
        public Boolean Equals(Kilogram Kilogram)

            => Value.Equals(Kilogram.Value);

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
        /// Try to format this Kilogram into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("kg".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kg".AsSpan()
                       );
            }

            if (Format.Equals("g".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           g,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " g".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " kg".AsSpan()
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
                String.Equals(Format, "kg", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} kg";
            }

            if (String.Equals(Format, "g", StringComparison.OrdinalIgnoreCase))
                return $"{g.ToString("G", FormatProvider)} g";

            return $"{Value.ToString(Format, FormatProvider)} kg";

        }

        #endregion

    }

}
