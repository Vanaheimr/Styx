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
    /// Extension methods for Amperes.
    /// </summary>
    public static class AmpereExtensions
    {

        #region Sum (this Amperes)

        /// <summary>
        /// The sum of the given Ampere values.
        /// </summary>
        /// <param name="Amperes">An enumeration of Ampere values.</param>
        public static Ampere Sum(this IEnumerable<Ampere> Amperes)
        {

            var sum = Ampere.Zero;

            foreach (var ampere in Amperes)
                sum += ampere;

            return sum;

        }

        #endregion

        #region Avg (this Amperes)

        /// <summary>
        /// The average of the given Ampere values.
        /// </summary>
        /// <param name="Amperes">An enumeration of Ampere values.</param>
        public static Ampere Avg(this IEnumerable<Ampere> Amperes)
        {

            var sum    = Ampere.Zero;
            var count  = 0;

            foreach (var ampere in Amperes)
            {
                sum += ampere;
                count++;
            }

            return count == 0
                       ? Ampere.Zero
                       : sum / count;

        }

        #endregion

    }


    /// <summary>
    /// An Ampere value (A), the SI unit of electric current.
    /// </summary>
    public readonly struct Ampere : IParsable    <Ampere>,
                                    ISpanParsable<Ampere>,
                                    IEquatable   <Ampere>,
                                    IComparable  <Ampere>,
                                    IComparable,
                                    IFormattable,
                                    ISpanFormattable,
                                    IAdditionOperators   <Ampere,  Ampere,  Ampere>,
                                    ISubtractionOperators<Ampere,  Ampere,  Ampere>,
                                    IMultiplyOperators   <Ampere,  Decimal, Ampere>,
                                    IDivisionOperators   <Ampere,  Decimal, Ampere>,
                                    IComparisonOperators <Ampere,  Ampere,  Boolean>,
                                    IEqualityOperators   <Ampere,  Ampere,  Boolean>,
                                    IAdditiveIdentity    <Ampere,  Ampere>
    {

        #region Properties

        /// <summary>
        /// The value of the Ampere.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Ampere.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as kiloAmperes.
        /// </summary>
        public Decimal  kA
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The zero value of the Ampere.
        /// </summary>
        public static readonly Ampere Zero = new (0m);

        /// <summary>
        /// The additive identity of Ampere.
        /// </summary>
        public static Ampere AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Ampere (A) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of amperes (A).</param>
        private Ampere(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as amperes using invariant culture.
        /// Supports optional suffixes "A" and "kA".
        /// </summary>
        /// <param name="Text">A text representation of amperes.</param>
        public static Ampere Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse      (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as amperes using the given format provider.
        /// Supports optional suffixes "A" and "kA".
        /// </summary>
        /// <param name="Text">A text representation of amperes.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Ampere Parse(String            Text,
                                   IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var ampere))
                return ampere;

            throw new FormatException($"Invalid text representation of amperes: '{Text}'!");

        }

        #endregion

        #region (static) Parse      (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as amperes using the given format provider.
        /// Supports optional suffixes "A" and "kA".
        /// </summary>
        /// <param name="Span">A text representation of amperes.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Ampere Parse(ReadOnlySpan<Char>  Span,
                                   IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var ampere))
                return ampere;

            throw new FormatException($"Invalid text representation of amperes: '{Span}'!");

        }

        #endregion

        #region (static) ParseA     (Text)

        /// <summary>
        /// Parse the given string as amperes.
        /// </summary>
        /// <param name="Text">A text representation of amperes.</param>
        public static Ampere ParseA(String Text)
        {

            if (TryParseA(Text, out var ampere))
                return ampere;

            throw new FormatException($"Invalid text representation of amperes: '{Text}'!");

        }

        #endregion

        #region (static) ParseKA    (Text)

        /// <summary>
        /// Parse the given string as kiloAmperes.
        /// </summary>
        /// <param name="Text">A text representation of a kiloAmperes.</param>
        public static Ampere ParseKA(String Text)
        {

            if (TryParseKA(Text, out var ampere))
                return ampere;

            throw new FormatException($"Invalid text representation of a kiloAmperes: '{Text}'!");

        }

        #endregion


        #region (static) FromA      (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into amperes.
        /// </summary>
        /// <param name="Number">A numeric representation of amperes.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ampere FromA(Decimal  Number,
                                   Int32?   Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into amperes.
        /// </summary>
        /// <param name="Number">A numeric representation of amperes.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ampere FromA(Byte    Number,
                                   Int32?  Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromKA     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into kiloAmperes.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloAmperes.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ampere FromKA(Decimal  Number,
                                    Int32?   Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into kiloAmperes.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloAmperes.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ampere FromKA(Byte    Number,
                                    Int32?  Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as amperes with an optional unit suffix ("A" or "kA")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of amperes.</param>
        public static Ampere? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var ampere))
                return ampere;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as amperes with an optional unit suffix ("A" or "kA")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of amperes.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Ampere? TryParse(String?           Text,
                                       IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var ampere))
                return ampere;

            return null;

        }

        #endregion

        #region (static) TryParseA  (Text)

        /// <summary>
        /// Try to parse the given text as amperes.
        /// </summary>
        /// <param name="Text">A text representation of amperes.</param>
        public static Ampere? TryParseA(String? Text)
        {

            if (TryParseA(Text, out var ampere))
                return ampere;

            return null;

        }

        #endregion

        #region (static) TryParseKA (Text)

        /// <summary>
        /// Try to parse the given text as kiloAmperes.
        /// </summary>
        /// <param name="Text">A text representation of a kiloAmperes.</param>
        public static Ampere? TryParseKA(String? Text)
        {

            if (TryParseKA(Text, out var ampere))
                return ampere;

            return null;

        }

        #endregion


        #region (static) TryFromA   (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into amperes.
        /// </summary>
        /// <param name="Number">A numeric representation of amperes.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ampere? TryFromA(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryFromA(Number, out var ampere, Exponent))
                return ampere;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into amperes.
        /// </summary>
        /// <param name="Number">A numeric representation of amperes.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ampere? TryFromA(Byte    Number,
                                       Int32?  Exponent = null)
        {

            if (TryFromA(Number, out var ampere, Exponent))
                return ampere;

            return null;

        }

        #endregion

        #region (static) TryFromKA  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kiloAmperes.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloAmperes.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ampere? TryFromKA(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryFromKA(Number, out var ampere, Exponent))
                return ampere;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into kiloAmperes.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloAmperes.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ampere? TryFromKA(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryFromKA(Number, out var ampere, Exponent))
                return ampere;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,                 out Ampere)

        /// <summary>
        /// Try to parse the given string as amperes using invariant culture.
        /// Supports optional suffixes "A" and "kA".
        /// </summary>
        /// <param name="Text">A text representation of amperes.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Ampere   Ampere)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Ampere);

        #endregion

        #region (static) TryParse   (Text, FormatProvider, out Ampere)

        /// <summary>
        /// Try to parse the given string as amperes using the given format provider.
        /// Supports optional suffixes "A" and "kA".
        /// </summary>
        /// <param name="Text">A text representation of amperes.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Ampere                   Ampere)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Ampere);

        #endregion

        #region (static) TryParse   (Span, FormatProvider, out Ampere)

        /// <summary>
        /// Try to parse the given text span as amperes using the given format provider.
        /// Supports optional suffixes "A" and "kA".
        /// </summary>
        /// <param name="Span">A text representation of amperes.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Ampere          Ampere)
        {

            Ampere = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kA".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("A".AsSpan(),  StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Ampere);
            }

            return false;

        }

        #endregion

        #region (static) TryParseA  (Text,   out Ampere)

        /// <summary>
        /// Try to parse the given string as amperes using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of amperes.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        public static Boolean TryParseA(String? Text, out Ampere Ampere)
        {

            Ampere = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Ampere);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKA (Text,   out Ampere)

        /// <summary>
        /// Try to parse the given string as kiloAmperes using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of an kiloAmperes.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        public static Boolean TryParseKA(String? Text, out Ampere Ampere)
        {

            Ampere = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Ampere);
            }

            return false;

        }

        #endregion


        #region (private static) TryCreate(Number, Exponent, out Ampere)

        private static Boolean TryCreate(Decimal     Number,
                                         Int32       Exponent,
                                         out Ampere  Ampere)
        {

            Ampere = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Ampere = Zero;
                return true;
            }

            try
            {
                Ampere = new Ampere(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) TryFromA   (Number, out Ampere, Exponent = null)

        /// <summary>
        /// Try to convert the given number into amperes.
        /// </summary>
        /// <param name="Number">A numeric representation of amperes.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromA(Byte        Number,
                                       out Ampere  Ampere,
                                       Int32?      Exponent = null)
        {

            Ampere = default;

            return MathHelpers.TryAddExponent(Exponent, 0, out var exponent) &&
                   TryCreate(Number, exponent, out Ampere);

        }


        /// <summary>
        /// Try to convert the given number into amperes.
        /// </summary>
        /// <param name="Number">A numeric representation of amperes.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromA(Decimal     Number,
                                       out Ampere  Ampere,
                                       Int32?      Exponent = null)
        {

            Ampere = default;

            return MathHelpers.TryAddExponent(Exponent, 0, out var exponent) &&
                   TryCreate(Number, exponent, out Ampere);

        }

        #endregion

        #region (static) TryFromKA  (Number, out Ampere, Exponent = null)

        /// <summary>
        /// Try to convert the given number into a kiloAmperes.
        /// </summary>
        /// <param name="Number">A numeric representation of a kiloAmperes.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKA(Byte        Number,
                                        out Ampere  Ampere,
                                        Int32?      Exponent = null)
        {

            Ampere = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out Ampere);

        }


        /// <summary>
        /// Try to convert the given number into a kiloAmperes.
        /// </summary>
        /// <param name="Number">A numeric representation of a kiloAmperes.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKA(Decimal     Number,
                                        out Ampere  Ampere,
                                        Int32?      Exponent = null)
        {

            Ampere = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out Ampere);

        }

        #endregion


        #region Operator overloading

        #region Operator == (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Ampere Ampere1,
                                           Ampere Ampere2)

            => Ampere1.Equals(Ampere2);

        #endregion

        #region Operator != (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Ampere Ampere1,
                                           Ampere Ampere2)

            => !Ampere1.Equals(Ampere2);

        #endregion

        #region Operator <  (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Ampere Ampere1,
                                          Ampere Ampere2)

            => Ampere1.CompareTo(Ampere2) < 0;

        #endregion

        #region Operator <= (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Ampere Ampere1,
                                           Ampere Ampere2)

            => Ampere1.CompareTo(Ampere2) <= 0;

        #endregion

        #region Operator >  (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Ampere Ampere1,
                                          Ampere Ampere2)

            => Ampere1.CompareTo(Ampere2) > 0;

        #endregion

        #region Operator >= (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Ampere Ampere1,
                                           Ampere Ampere2)

            => Ampere1.CompareTo(Ampere2) >= 0;

        #endregion

        #region Operator +  (Ampere1, Ampere2)

        /// <summary>
        /// Accumulates two Amperes.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        public static Ampere operator + (Ampere Ampere1,
                                         Ampere Ampere2)

            => new (Ampere1.Value + Ampere2.Value);

        #endregion

        #region Operator -  (Ampere1, Ampere2)

        /// <summary>
        /// Substracts two Amperes.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        public static Ampere operator - (Ampere Ampere1,
                                         Ampere Ampere2)

            => new (Ampere1.Value - Ampere2.Value);

        #endregion


        #region Operator *  (Ampere,  Scalar)

        /// <summary>
        /// Multiplies an Ampere with a scalar.
        /// </summary>
        /// <param name="Ampere">An Ampere value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Ampere operator * (Ampere  Ampere,
                                         Decimal Scalar)

            => new (Ampere.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,  Ampere)

        /// <summary>
        /// Multiplies a scalar with an Ampere.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Ampere">An Ampere value.</param>
        public static Ampere operator * (Decimal Scalar,
                                         Ampere  Ampere)

            => new (Scalar * Ampere.Value);

        #endregion

        #region Operator /  (Ampere,  Scalar)

        /// <summary>
        /// Divides an Ampere with a scalar.
        /// </summary>
        /// <param name="Ampere">An Ampere value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Ampere operator / (Ampere  Ampere,
                                         Decimal Scalar)

            => new (Ampere.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Ampere> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Amperes.
        /// </summary>
        /// <param name="Object">An Ampere to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null           => 1,
                   Ampere ampere  => CompareTo(ampere),
                   _              => throw new ArgumentException("The given object is not an Ampere!", nameof(Object))
               };

        #endregion

        #region CompareTo(Ampere)

        /// <summary>
        /// Compares two Amperes.
        /// </summary>
        /// <param name="Ampere">An Ampere to compare with.</param>
        public Int32 CompareTo(Ampere Ampere)

            => Value.CompareTo(Ampere.Value);

        #endregion

        #endregion

        #region IEquatable<Ampere> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Amperes for equality.
        /// </summary>
        /// <param name="Object">An Ampere to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Ampere ampere &&
                   Equals(ampere);

        #endregion

        #region Equals(Ampere)

        /// <summary>
        /// Compares two Amperes for equality.
        /// </summary>
        /// <param name="Ampere">An Ampere to compare with.</param>
        public Boolean Equals(Ampere Ampere)

            => Value.Equals(Ampere.Value);

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
        /// Try to format this Ampere into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("A".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " A".AsSpan()
                       );
            }

            if (Format.Equals("kA".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kA,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kA".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " A".AsSpan()
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
                String.Equals(Format, "A",  StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} A";
            }

            if (String.Equals(Format, "kA", StringComparison.OrdinalIgnoreCase))
                return $"{kA.ToString("G", FormatProvider)} kA";

            return $"{Value.ToString(Format, FormatProvider)} A";

        }

        #endregion

    }

}
