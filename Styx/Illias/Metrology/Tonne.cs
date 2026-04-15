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
    /// Extension methods for metric Tonne (t) values.
    /// </summary>
    public static class TonneExtensions
    {

        #region Sum    (this TonneValues)

        /// <summary>
        /// The sum of the given enumeration of Tonne values.
        /// </summary>
        /// <param name="TonneValues">An enumeration of Tonne values.</param>
        public static Tonne Sum(this IEnumerable<Tonne> TonneValues)
        {

            var sum = Tonne.Zero;

            foreach (var tonne in TonneValues)
                sum += tonne;

            return sum;

        }

        #endregion

        #region Avg    (this TonneValues)

        /// <summary>
        /// The average of the given enumeration of Tonne values.
        /// </summary>
        /// <param name="TonneValues">An enumeration of Tonne values.</param>
        public static Tonne Avg(this IEnumerable<Tonne> TonneValues)
        {

            var sum    = Tonne.Zero;
            var count  = 0;

            foreach (var tonne in TonneValues)
            {
                sum += tonne;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this TonneValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Tonne values.
        /// </summary>
        /// <param name="TonneValues">An enumeration of Tonne values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Tonne> StdDev(this IEnumerable<Tonne>  TonneValues,
                                           Boolean?                 IsSampleData   = null)
        {

            var stdDev = StdDev<Tonne>.From(
                             TonneValues.Select(tonne => tonne.Value),
                             IsSampleData
                         );

            return new StdDev<Tonne>(
                       Tonne.FromT(stdDev.Mean),
                       Tonne.FromT(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A metric Tonne value (t), the SI unit of mass, is defined as being equal to 1000 kilograms.
    /// Do not mix up with the similar sounding "ton" which is a unit of mass used in the US and UK, but with different values:
    ///   - ton (US)  ~907 kg
    ///   - ton (UK) ~1016 kg
    /// </summary>
    public readonly struct Tonne : IMetrology<Tonne>
    {

        #region Properties

        /// <summary>
        /// The value of the metric Tonne.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the metric Tonne.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as kiloTonne.
        /// </summary>
        public Decimal  kT
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The zero value of the metric Tonne.
        /// </summary>
        public static readonly Tonne Zero = new (0m);

        /// <summary>
        /// The additive identity of metric Tonne.
        /// </summary>
        public static Tonne AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new metric Tonne (t) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of tonne (t).</param>
        private Tonne(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as tonne using invariant culture.
        /// Supports optional suffixes "t" and "kT".
        /// </summary>
        /// <param name="Text">A text representation of tonne.</param>
        public static Tonne Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse      (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as tonne using the given format provider.
        /// Supports optional suffixes "t" and "kT".
        /// </summary>
        /// <param name="Text">A text representation of tonne.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Tonne Parse(String            Text,
                                  IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var tonne))
                return tonne;

            throw new FormatException($"Invalid text representation of tonne: '{Text}'!");

        }

        #endregion

        #region (static) Parse      (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as tonne using the given format provider.
        /// Supports optional suffixes "t" and "kT".
        /// </summary>
        /// <param name="Span">A text representation of tonne.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Tonne Parse(ReadOnlySpan<Char>  Span,
                                  IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var tonne))
                return tonne;

            throw new FormatException($"Invalid text representation of tonne: '{Span}'!");

        }

        #endregion

        #region (static) ParseT     (Text)

        /// <summary>
        /// Parse the given string as Tonne (t).
        /// </summary>
        /// <param name="Text">A text representation of Tonne (t).</param>
        public static Tonne ParseT(String Text)
        {

            if (TryParseT(Text, out var tonne))
                return tonne;

            throw new FormatException($"Invalid text representation of Tonne (t): '{Text}'!");

        }

        #endregion

        #region (static) ParseKT    (Text)

        /// <summary>
        /// Parse the given string as KiloTonne (kT).
        /// </summary>
        /// <param name="Text">A text representation of KiloTonne (kT).</param>
        public static Tonne ParseKT(String Text)
        {

            if (TryParseKT(Text, out var tonne))
                return tonne;

            throw new FormatException($"Invalid text representation of KiloTonne (kT): '{Text}'!");

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as tonne with an optional unit suffix ("t" or "kT")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of tonne.</param>
        public static Tonne? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var tonne))
                return tonne;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as tonne with an optional unit suffix ("t" or "kT")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of tonne.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Tonne? TryParse(String?           Text,
                                      IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var tonne))
                return tonne;

            return null;

        }

        #endregion

        #region (static) TryParseT  (Text)

        /// <summary>
        /// Try to parse the given text as Tonne (t).
        /// </summary>
        /// <param name="Text">A text representation of Tonne (t).</param>
        public static Tonne? TryParseT(String? Text)
        {

            if (TryParseT(Text, out var tonne))
                return tonne;

            return null;

        }

        #endregion

        #region (static) TryParseKT (Text)

        /// <summary>
        /// Try to parse the given text as KiloTonne (kT).
        /// </summary>
        /// <param name="Text">A text representation of KiloTonne (kT).</param>
        public static Tonne? TryParseKT(String? Text)
        {

            if (TryParseKT(Text, out var tonne))
                return tonne;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,                 out Tonne)

        /// <summary>
        /// Try to parse the given string as tonne using invariant culture.
        /// Supports optional suffixes "t" and "kT".
        /// </summary>
        /// <param name="Text">A text representation of tonne.</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Tonne    Tonne)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Tonne);

        #endregion

        #region (static) TryParse   (Text, FormatProvider, out Tonne)

        /// <summary>
        /// Try to parse the given string as tonne using the given format provider.
        /// Supports optional suffixes "t" and "kT".
        /// </summary>
        /// <param name="Text">A text representation of tonne.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Tonne                    Tonne)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Tonne);

        #endregion

        #region (static) TryParse   (Span, FormatProvider, out Tonne)

        /// <summary>
        /// Try to parse the given text span as tonne using the given format provider.
        /// Supports optional suffixes "t" and "kT".
        /// </summary>
        /// <param name="Span">A text representation of tonne.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Tonne           Tonne)
        {

            Tonne = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kT".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("t".AsSpan(),  StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Tonne);
            }

            return false;

        }

        #endregion

        #region (static) TryParseT  (Text,                 out Tonne)

        /// <summary>
        /// Try to parse the given string as Tonne (t).
        /// </summary>
        /// <param name="Text">A text representation of Tonne (t).</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        public static Boolean TryParseT([NotNullWhen(true)] String?  Text,
                                        out                 Tonne    Tonne)
        {

            Tonne = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Tonne);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKT (Text,                 out Tonne)

        /// <summary>
        /// Try to parse the given string as KiloTonne (kT).
        /// </summary>
        /// <param name="Text">A text representation of KiloTonne (kT).</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        public static Boolean TryParseKT([NotNullWhen(true)] String?  Text,
                                         out                 Tonne    Tonne)
        {

            Tonne = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Tonne);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static Tonne Create(Decimal  Number,
                                    Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var tonne))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return tonne;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Tonne)

        private static Boolean TryCreate(Decimal    Number,
                                         Int32      Exponent,
                                         out Tonne  Tonne)
        {

            Tonne = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Tonne = Zero;
                return true;
            }

            try
            {
                Tonne = new Tonne(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) FromT      (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into Tonne (t).
        /// </summary>
        /// <param name="Number">A numeric representation of Tonne (t).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne FromT<TNumber>(TNumber  Number,
                                           Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) FromKT     (Number,            Exponent = null)

        /// <summary>
        /// Convert the given number into KiloTonne (kT).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloTonne (kT).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne FromKT<TNumber>(TNumber  Number,
                                            Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 3)
                   );

        #endregion


        #region (static) TryFromT   (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into Tonne (t).
        /// </summary>
        /// <param name="Number">A numeric representation of Tonne (t).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne? TryFromT<TNumber>(TNumber  Number,
                                               Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromT(Number, out var tonne, Exponent))
                return tonne;

            return null;

        }

        #endregion

        #region (static) TryFromKT  (Number,            Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloTonne (kT).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloTonne (kT).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne? TryFromKT<TNumber>(TNumber  Number,
                                                Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromKT(Number, out var tonne, Exponent))
                return tonne;

            return null;

        }

        #endregion


        #region (static) TryFromT   (Number, out Tonne, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Tonne (t).
        /// </summary>
        /// <param name="Number">A numeric representation of Tonne (t).</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromT<TNumber>(TNumber    Number,
                                                out Tonne  Tonne,
                                                Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Tonne = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Tonne);
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

        #region (static) TryFromKT  (Number, out Tonne, Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloTonne (kT).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloTonne (kT).</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKT<TNumber>(TNumber    Number,
                                                 out Tonne  Tonne,
                                                 Int32?     Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Tonne = default;

            if (!MathHelpers.TryAddExponent(Exponent, 3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Tonne);
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


        #region Conversions to/from kilograms

        /// <summary>
        /// Convert the current value to its equivalent in kilograms.
        /// </summary>
        /// <returns></returns>
        public Kilogram ToKilogram()
            => Kilogram.FromKG(Value * 1000m);

        /// <summary>
        /// Convert the given kilogram value to its equivalent in tonnes.
        /// </summary>
        /// <param name="Kilogram">A kilogram value.</param>
        public static Tonne FromKilogram(Kilogram Kilogram)
            => FromT(Kilogram.Value / 1000m);

        #endregion


        #region Operator overloading

        #region Operator == (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A Tonne value.</param>
        /// <param name="Tonne2">Another Tonne value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tonne Tonne1,
                                           Tonne Tonne2)

            => Tonne1.Equals(Tonne2);

        #endregion

        #region Operator != (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A Tonne value.</param>
        /// <param name="Tonne2">Another Tonne value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tonne Tonne1,
                                           Tonne Tonne2)

            => !Tonne1.Equals(Tonne2);

        #endregion

        #region Operator <  (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A Tonne value.</param>
        /// <param name="Tonne2">Another Tonne value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tonne Tonne1,
                                          Tonne Tonne2)

            => Tonne1.CompareTo(Tonne2) < 0;

        #endregion

        #region Operator <= (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A Tonne value.</param>
        /// <param name="Tonne2">Another Tonne value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tonne Tonne1,
                                           Tonne Tonne2)

            => Tonne1.CompareTo(Tonne2) <= 0;

        #endregion

        #region Operator >  (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A Tonne value.</param>
        /// <param name="Tonne2">Another Tonne value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tonne Tonne1,
                                          Tonne Tonne2)

            => Tonne1.CompareTo(Tonne2) > 0;

        #endregion

        #region Operator >= (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A Tonne value.</param>
        /// <param name="Tonne2">Another Tonne value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tonne Tonne1,
                                           Tonne Tonne2)

            => Tonne1.CompareTo(Tonne2) >= 0;

        #endregion

        #region Operator +  (Tonne1, Tonne2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A Tonne value.</param>
        /// <param name="Tonne2">Another Tonne value.</param>
        public static Tonne operator + (Tonne Tonne1,
                                        Tonne Tonne2)

            => new (Tonne1.Value + Tonne2.Value);

        #endregion

        #region Operator -  (Tonne1, Tonne2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A Tonne value.</param>
        /// <param name="Tonne2">Another Tonne value.</param>
        public static Tonne operator - (Tonne Tonne1,
                                        Tonne Tonne2)

            => new (Tonne1.Value - Tonne2.Value);

        #endregion


        #region Operator *  (Tonne,  Scalar)

        /// <summary>
        /// Multiplies a Tonne with a scalar.
        /// </summary>
        /// <param name="Tonne">A Tonne value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Tonne operator * (Tonne    Tonne,
                                        Decimal  Scalar)

            => new (Tonne.Value * Scalar);

        #endregion

        #region Operator *  (Scalar, Tonne)

        /// <summary>
        /// Multiplies a scalar with a Tonne.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Tonne">A Tonne value.</param>
        public static Tonne operator * (Decimal  Scalar,
                                        Tonne    Tonne)

            => new (Scalar * Tonne.Value);

        #endregion

        #region Operator /  (Tonne,  Scalar)

        /// <summary>
        /// Divides a Tonne with a scalar.
        /// </summary>
        /// <param name="Tonne">A Tonne value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Tonne operator / (Tonne    Tonne,
                                        Decimal  Scalar)

            => new (Tonne.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Tonne> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Tonne.
        /// </summary>
        /// <param name="Object">A Tonne to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null         => 1,
                   Tonne tonne  => CompareTo(tonne),
                   _            => throw new ArgumentException("The given object is not a Tonne!", nameof(Object))
               };

        #endregion

        #region CompareTo(Tonne)

        /// <summary>
        /// Compares two Tonne.
        /// </summary>
        /// <param name="Tonne">A Tonne to compare with.</param>
        public Int32 CompareTo(Tonne Tonne)

            => Value.CompareTo(Tonne.Value);

        #endregion

        #endregion

        #region IEquatable<Tonne> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Tonne for equality.
        /// </summary>
        /// <param name="Object">A Tonne to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Tonne tonne &&
                   Equals(tonne);

        #endregion

        #region Equals(Tonne)

        /// <summary>
        /// Compares two Tonne for equality.
        /// </summary>
        /// <param name="Tonne">A Tonne to compare with.</param>
        public Boolean Equals(Tonne Tonne)

            => Value.Equals(Tonne.Value);

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
        /// Try to format this Tonne into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("t".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " t".AsSpan()
                       );
            }

            if (Format.Equals("kT".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kT,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kT".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " t".AsSpan()
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
                String.Equals(Format, "t",  StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} t";
            }

            if (String.Equals(Format, "kT", StringComparison.OrdinalIgnoreCase))
                return $"{kT.ToString("G", FormatProvider)} kT";

            return $"{Value.ToString(Format, FormatProvider)} t";

        }

        #endregion

    }

}
