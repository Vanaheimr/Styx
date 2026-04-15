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
    /// Extension methods for Siemens (S) values.
    /// </summary>
    public static class SiemensExtensions
    {

        #region Sum    (this SiemensValues)

        /// <summary>
        /// The sum of the given enumeration of Siemens values.
        /// </summary>
        /// <param name="SiemensValues">An enumeration of Siemens values.</param>
        public static Siemens Sum(this IEnumerable<Siemens> SiemensValues)
        {

            var sum = Siemens.Zero;

            foreach (var siemens in SiemensValues)
                sum += siemens;

            return sum;

        }

        #endregion

        #region Avg    (this SiemensValues)

        /// <summary>
        /// The average of the given enumeration of Siemens values.
        /// </summary>
        /// <param name="SiemensValues">An enumeration of Siemens values.</param>
        public static Siemens Avg(this IEnumerable<Siemens> SiemensValues)
        {

            var sum    = Siemens.Zero;
            var count  = 0;

            foreach (var siemens in SiemensValues)
            {
                sum += siemens;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this SiemensValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Siemens values.
        /// </summary>
        /// <param name="SiemensValues">An enumeration of Siemens values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Siemens> StdDev(this IEnumerable<Siemens>  SiemensValues,
                                             Boolean?                   IsSampleData   = null)
        {

            var stdDev = StdDev<Siemens>.From(
                             SiemensValues.Select(siemens => siemens.Value),
                             IsSampleData
                         );

            return new StdDev<Siemens>(
                       Siemens.FromS(stdDev.Mean),
                       Siemens.FromS(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Siemens value (S), the SI unit of electric conductance.
    /// </summary>
    public readonly struct Siemens : IMetrology<Siemens>
    {

        #region Properties

        /// <summary>
        /// The value of the Siemens.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Siemens.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as kiloSiemens.
        /// </summary>
        public Decimal  kS
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The zero value of the Siemens.
        /// </summary>
        public static readonly Siemens Zero = new (0m);

        /// <summary>
        /// The additive identity of Siemens.
        /// </summary>
        public static Siemens AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Siemens (S) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of siemens (S).</param>
        private Siemens(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as siemens using invariant culture.
        /// Supports optional suffixes "S" and "kS".
        /// </summary>
        /// <param name="Text">A text representation of siemens.</param>
        public static Siemens Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse      (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as siemens using the given format provider.
        /// Supports optional suffixes "S" and "kS".
        /// </summary>
        /// <param name="Text">A text representation of siemens.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Siemens Parse(String            Text,
                                    IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var siemens))
                return siemens;

            throw new FormatException($"Invalid text representation of siemens: '{Text}'!");

        }

        #endregion

        #region (static) Parse      (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as siemens using the given format provider.
        /// Supports optional suffixes "S" and "kS".
        /// </summary>
        /// <param name="Span">A text representation of siemens.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Siemens Parse(ReadOnlySpan<Char>  Span,
                                    IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var siemens))
                return siemens;

            throw new FormatException($"Invalid text representation of siemens: '{Span}'!");

        }

        #endregion

        #region (static) ParseS     (Text)

        /// <summary>
        /// Parse the given string as Siemens (S).
        /// </summary>
        /// <param name="Text">A text representation of Siemens (S).</param>
        public static Siemens ParseS(String Text)
        {

            if (TryParseS(Text, out var siemens))
                return siemens;

            throw new FormatException($"Invalid text representation of Siemens (S): '{Text}'!");

        }

        #endregion

        #region (static) ParseKS    (Text)

        /// <summary>
        /// Parse the given string as KiloSiemens (kS).
        /// </summary>
        /// <param name="Text">A text representation of KiloSiemens (kS).</param>
        public static Siemens ParseKS(String Text)
        {

            if (TryParseKS(Text, out var siemens))
                return siemens;

            throw new FormatException($"Invalid text representation of KiloSiemens (kS): '{Text}'!");

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as siemens with an optional unit suffix ("S" or "kS")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of siemens.</param>
        public static Siemens? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var siemens))
                return siemens;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as siemens with an optional unit suffix ("S" or "kS")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of siemens.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Siemens? TryParse(String?           Text,
                                        IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var siemens))
                return siemens;

            return null;

        }

        #endregion

        #region (static) TryParseS  (Text)

        /// <summary>
        /// Try to parse the given text as Siemens (S).
        /// </summary>
        /// <param name="Text">A text representation of Siemens (S).</param>
        public static Siemens? TryParseS(String? Text)
        {

            if (TryParseS(Text, out var siemens))
                return siemens;

            return null;

        }

        #endregion

        #region (static) TryParseKS (Text)

        /// <summary>
        /// Try to parse the given text as KiloSiemens (kS).
        /// </summary>
        /// <param name="Text">A text representation of KiloSiemens (kS).</param>
        public static Siemens? TryParseKS(String? Text)
        {

            if (TryParseKS(Text, out var siemens))
                return siemens;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,                 out Siemens)

        /// <summary>
        /// Try to parse the given string as siemens using invariant culture.
        /// Supports optional suffixes "S" and "kS".
        /// </summary>
        /// <param name="Text">A text representation of siemens.</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Siemens  Siemens)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Siemens);

        #endregion

        #region (static) TryParse   (Text, FormatProvider, out Siemens)

        /// <summary>
        /// Try to parse the given string as siemens using the given format provider.
        /// Supports optional suffixes "S" and "kS".
        /// </summary>
        /// <param name="Text">A text representation of siemens.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Siemens                  Siemens)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Siemens);

        #endregion

        #region (static) TryParse   (Span, FormatProvider, out Siemens)

        /// <summary>
        /// Try to parse the given text span as siemens using the given format provider.
        /// Supports optional suffixes "S" and "kS".
        /// </summary>
        /// <param name="Span">A text representation of siemens.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Siemens         Siemens)
        {

            Siemens = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kS".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("S".AsSpan(),  StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Siemens);
            }

            return false;

        }

        #endregion

        #region (static) TryParseS  (Text,                 out Siemens)

        /// <summary>
        /// Try to parse the given string as Siemens (S).
        /// </summary>
        /// <param name="Text">A text representation of Siemens (S).</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        public static Boolean TryParseS([NotNullWhen(true)] String?  Text,
                                        out                 Siemens  Siemens)
        {

            Siemens = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Siemens);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKS (Text,                 out Siemens)

        /// <summary>
        /// Try to parse the given string as KiloSiemens (kS).
        /// </summary>
        /// <param name="Text">A text representation of KiloSiemens (kS).</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        public static Boolean TryParseKS([NotNullWhen(true)] String?  Text,
                                         out                 Siemens  Siemens)
        {

            Siemens = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Siemens);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static Siemens Create(Decimal  Number,
                                      Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var siemens))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return siemens;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Siemens)

        private static Boolean TryCreate(Decimal      Number,
                                         Int32        Exponent,
                                         out Siemens  Siemens)
        {

            Siemens = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Siemens = Zero;
                return true;
            }

            try
            {
                Siemens = new Siemens(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) FromS      (Number,              Exponent = null)

        /// <summary>
        /// Convert the given number into Siemens (S).
        /// </summary>
        /// <param name="Number">A numeric representation of Siemens (S).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens FromS<TNumber>(TNumber  Number,
                                             Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) FromKS     (Number,              Exponent = null)

        /// <summary>
        /// Convert the given number into KiloSiemens (kS).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloSiemens (kS).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens FromKS<TNumber>(TNumber  Number,
                                              Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       checked((Exponent ?? 0) + 3)
                   );

        #endregion


        #region (static) TryFromS   (Number,              Exponent = null)

        /// <summary>
        /// Try to convert the given number into Siemens (S).
        /// </summary>
        /// <param name="Number">A numeric representation of Siemens (S).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens? TryFromS<TNumber>(TNumber  Number,
                                                 Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromS(Number, out var siemens, Exponent))
                return siemens;

            return null;

        }

        #endregion

        #region (static) TryFromKS  (Number,              Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloSiemens (kS).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloSiemens (kS).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens? TryFromKS<TNumber>(TNumber  Number,
                                                  Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromKS(Number, out var siemens, Exponent))
                return siemens;

            return null;

        }

        #endregion


        #region (static) TryFromS   (Number, out Siemens, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Siemens (S).
        /// </summary>
        /// <param name="Number">A numeric representation of Siemens (S).</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromS<TNumber>(TNumber      Number,
                                                out Siemens  Siemens,
                                                Int32?       Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Siemens = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Siemens);
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

        #region (static) TryFromKS  (Number, out Siemens, Exponent = null)

        /// <summary>
        /// Try to convert the given number into KiloSiemens (kS).
        /// </summary>
        /// <param name="Number">A numeric representation of KiloSiemens (kS).</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKS<TNumber>(TNumber      Number,
                                                 out Siemens  Siemens,
                                                 Int32?       Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Siemens = default;

            if (!MathHelpers.TryAddExponent(Exponent, 3, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Siemens);
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

        #region Operator == (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens value.</param>
        /// <param name="Siemens2">Another Siemens value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Siemens Siemens1,
                                           Siemens Siemens2)

            => Siemens1.Equals(Siemens2);

        #endregion

        #region Operator != (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens value.</param>
        /// <param name="Siemens2">Another Siemens value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Siemens Siemens1,
                                           Siemens Siemens2)

            => !Siemens1.Equals(Siemens2);

        #endregion

        #region Operator <  (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens value.</param>
        /// <param name="Siemens2">Another Siemens value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Siemens Siemens1,
                                          Siemens Siemens2)

            => Siemens1.CompareTo(Siemens2) < 0;

        #endregion

        #region Operator <= (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens value.</param>
        /// <param name="Siemens2">Another Siemens value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Siemens Siemens1,
                                           Siemens Siemens2)

            => Siemens1.CompareTo(Siemens2) <= 0;

        #endregion

        #region Operator >  (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens value.</param>
        /// <param name="Siemens2">Another Siemens value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Siemens Siemens1,
                                          Siemens Siemens2)

            => Siemens1.CompareTo(Siemens2) > 0;

        #endregion

        #region Operator >= (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens value.</param>
        /// <param name="Siemens2">Another Siemens value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Siemens Siemens1,
                                           Siemens Siemens2)

            => Siemens1.CompareTo(Siemens2) >= 0;

        #endregion

        #region Operator +  (Siemens1, Siemens2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens value.</param>
        /// <param name="Siemens2">Another Siemens value.</param>
        public static Siemens operator + (Siemens Siemens1,
                                          Siemens Siemens2)

            => new (Siemens1.Value + Siemens2.Value);

        #endregion

        #region Operator -  (Siemens1, Siemens2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens value.</param>
        /// <param name="Siemens2">Another Siemens value.</param>
        public static Siemens operator - (Siemens Siemens1,
                                          Siemens Siemens2)

            => new (Siemens1.Value - Siemens2.Value);

        #endregion


        #region Operator *  (Siemens,  Scalar)

        /// <summary>
        /// Multiplies a Siemens with a scalar.
        /// </summary>
        /// <param name="Siemens">A Siemens value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Siemens operator * (Siemens  Siemens,
                                          Decimal  Scalar)

            => new (Siemens.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,   Siemens)

        /// <summary>
        /// Multiplies a scalar with a Siemens.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Siemens">A Siemens value.</param>
        public static Siemens operator * (Decimal  Scalar,
                                          Siemens  Siemens)

            => new (Scalar * Siemens.Value);

        #endregion

        #region Operator /  (Siemens,  Scalar)

        /// <summary>
        /// Divides a Siemens with a scalar.
        /// </summary>
        /// <param name="Siemens">A Siemens value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Siemens operator / (Siemens  Siemens,
                                          Decimal  Scalar)

            => new (Siemens.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Siemens> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Siemens.
        /// </summary>
        /// <param name="Object">A Siemens to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null             => 1,
                   Siemens siemens  => CompareTo(siemens),
                   _                => throw new ArgumentException("The given object is not a Siemens!", nameof(Object))
               };

        #endregion

        #region CompareTo(Siemens)

        /// <summary>
        /// Compares two Siemens.
        /// </summary>
        /// <param name="Siemens">A Siemens to compare with.</param>
        public Int32 CompareTo(Siemens Siemens)

            => Value.CompareTo(Siemens.Value);

        #endregion

        #endregion

        #region IEquatable<Siemens> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Siemens for equality.
        /// </summary>
        /// <param name="Object">A Siemens to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Siemens siemens &&
                   Equals(siemens);

        #endregion

        #region Equals(Siemens)

        /// <summary>
        /// Compares two Siemens for equality.
        /// </summary>
        /// <param name="Siemens">A Siemens to compare with.</param>
        public Boolean Equals(Siemens Siemens)

            => Value.Equals(Siemens.Value);

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
        /// Try to format this Siemens into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("S".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " S".AsSpan()
                       );
            }

            if (Format.Equals("kS".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kS,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kS".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " S".AsSpan()
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
                String.Equals(Format, "S",  StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} S";
            }

            if (String.Equals(Format, "kS", StringComparison.OrdinalIgnoreCase))
                return $"{kS.ToString("G", FormatProvider)} kS";

            return $"{Value.ToString(Format, FormatProvider)} S";

        }

        #endregion

    }

}
