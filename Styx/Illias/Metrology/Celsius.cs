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
    /// Extension methods for Celsius (°C) values.
    /// </summary>
    public static class CelsiusExtensions
    {

        #region Sum    (this CelsiusValues)

        /// <summary>
        /// The sum of the given enumeration of Celsius values.
        /// </summary>
        /// <param name="CelsiusValues">An enumeration of Celsius values.</param>
        public static Celsius Sum(this IEnumerable<Celsius> CelsiusValues)
        {

            var sum = Celsius.Zero;

            foreach (var celsius in CelsiusValues)
                sum += celsius;

            return sum;

        }

        #endregion

        #region Avg    (this CelsiusValues)

        /// <summary>
        /// The average of the given enumeration of Celsius values.
        /// </summary>
        /// <param name="CelsiusValues">An enumeration of Celsius values.</param>
        public static Celsius Avg(this IEnumerable<Celsius> CelsiusValues)
        {

            var sum    = Celsius.Zero;
            var count  = 0;

            foreach (var celsius in CelsiusValues)
            {
                sum += celsius;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this CelsiusValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Celsius values.
        /// </summary>
        /// <param name="CelsiusValues">An enumeration of Celsius values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Celsius> StdDev(this IEnumerable<Celsius>  CelsiusValues,
                                             Boolean?                   IsSampleData   = null)
        {

            var stdDev = StdDev<Celsius>.From(
                             CelsiusValues.Select(celsius => celsius.Value),
                             IsSampleData
                         );

            return new StdDev<Celsius>(
                       Celsius.FromC(stdDev.Mean),
                       Celsius.FromC(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Celsius value (°C), the SI unit of temperature.
    /// </summary>
    public readonly struct Celsius : IMetrology<Celsius>
    {

        #region Properties

        /// <summary>
        /// The value of the Celsius.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Celsius.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


        /// <summary>
        /// The zero value of the Celsius.
        /// </summary>
        public static readonly Celsius Zero = new (0m);

        /// <summary>
        /// The additive identity of Celsius.
        /// </summary>
        public static Celsius AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Celsius (°C) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of celsius (°C).</param>
        private Celsius(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as celsius using invariant culture.
        /// Supports optional suffix "°C".
        /// </summary>
        /// <param name="Text">A text representation of celsius.</param>
        public static Celsius Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse      (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as celsius using the given format provider.
        /// Supports optional suffix "°C".
        /// </summary>
        /// <param name="Text">A text representation of celsius.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Celsius Parse(String            Text,
                                    IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var celsius))
                return celsius;

            throw new FormatException($"Invalid text representation of celsius: '{Text}'!");

        }

        #endregion

        #region (static) Parse      (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as celsius using the given format provider.
        /// Supports optional suffix "°C".
        /// </summary>
        /// <param name="Span">A text representation of celsius.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Celsius Parse(ReadOnlySpan<Char>  Span,
                                    IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var celsius))
                return celsius;

            throw new FormatException($"Invalid text representation of celsius: '{Span}'!");

        }

        #endregion

        #region (static) ParseC     (Text)

        /// <summary>
        /// Parse the given string as Celsius (°C).
        /// </summary>
        /// <param name="Text">A text representation of Celsius (°C).</param>
        public static Celsius ParseC(String Text)
        {

            if (TryParseC(Text, out var celsius))
                return celsius;

            throw new FormatException($"Invalid text representation of Celsius (°C): '{Text}'!");

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as celsius with an optional unit suffix (°C)
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of celsius.</param>
        public static Celsius? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var celsius))
                return celsius;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as celsius with an optional unit suffix (°C)
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of celsius.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Celsius? TryParse(String?           Text,
                                        IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var celsius))
                return celsius;

            return null;

        }

        #endregion

        #region (static) TryParseC  (Text)

        /// <summary>
        /// Try to parse the given text as Celsius (°C).
        /// </summary>
        /// <param name="Text">A text representation of Celsius (°C).</param>
        public static Celsius? TryParseC(String? Text)
        {

            if (TryParseC(Text, out var celsius))
                return celsius;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,                 out Celsius)

        /// <summary>
        /// Try to parse the given string as celsius using invariant culture.
        /// Supports optional suffix "°C".
        /// </summary>
        /// <param name="Text">A text representation of celsius.</param>
        /// <param name="Celsius">The parsed Celsius.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Celsius  Celsius)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Celsius);

        #endregion

        #region (static) TryParse   (Text, FormatProvider, out Celsius)

        /// <summary>
        /// Try to parse the given string as celsius using the given format provider.
        /// Supports optional suffix "°C".
        /// </summary>
        /// <param name="Text">A text representation of celsius.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Celsius">The parsed Celsius.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Celsius                  Celsius)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Celsius);

        #endregion

        #region (static) TryParse   (Span, FormatProvider, out Celsius)

        /// <summary>
        /// Try to parse the given text span as celsius using the given format provider.
        /// Supports optional suffix "°C".
        /// </summary>
        /// <param name="Span">A text representation of celsius.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Celsius">The parsed Celsius.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Celsius         Celsius)
        {

            Celsius = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if (Span.EndsWith("°C".AsSpan(),  StringComparison.OrdinalIgnoreCase))
            {
                Span = Span[..^2].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Celsius);
            }

            return false;

        }

        #endregion

        #region (static) TryParseC  (Text,                 out Celsius)

        /// <summary>
        /// Try to parse the given string as Celsius (°C).
        /// </summary>
        /// <param name="Text">A text representation of Celsius (°C).</param>
        /// <param name="Celsius">The parsed Celsius.</param>
        public static Boolean TryParseC([NotNullWhen(true)] String?  Text,
                                        out                 Celsius  Celsius)
        {

            Celsius = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Celsius);
            }

            return false;

        }

        #endregion


        #region (private static) Create    (Number, Exponent)

        private static Celsius Create(Decimal  Number,
                                      Int32    Exponent)
        {

            if (!TryCreate(Number, Exponent, out var celsius))
                throw new ArgumentOutOfRangeException(nameof(Exponent));

            return celsius;

        }

        #endregion

        #region (private static) TryCreate (Number, Exponent, out Celsius)

        private static Boolean TryCreate(Decimal      Number,
                                         Int32        Exponent,
                                         out Celsius  Celsius)
        {

            Celsius = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Celsius = Zero;
                return true;
            }

            try
            {
                Celsius = new Celsius(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) FromC      (Number,              Exponent = null)

        /// <summary>
        /// Convert the given number into Celsius (°C).
        /// </summary>
        /// <param name="Number">A numeric representation of Celsius (°C).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Celsius FromC<TNumber>(TNumber  Number,
                                             Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

                => Create(
                       Decimal.CreateChecked(Number),
                       Exponent ?? 0
                   );

        #endregion

        #region (static) TryFromC   (Number,              Exponent = null)

        /// <summary>
        /// Try to convert the given number into Celsius (°C).
        /// </summary>
        /// <param name="Number">A numeric representation of Celsius (°C).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Celsius? TryFromC<TNumber>(TNumber  Number,
                                                 Int32?   Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            if (TryFromC(Number, out var celsius, Exponent))
                return celsius;

            return null;

        }

        #endregion

        #region (static) TryFromC   (Number, out Celsius, Exponent = null)

        /// <summary>
        /// Try to convert the given number into Celsius (°C).
        /// </summary>
        /// <param name="Number">A numeric representation of Celsius (°C).</param>
        /// <param name="Celsius">The parsed Celsius.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromC<TNumber>(TNumber      Number,
                                                out Celsius  Celsius,
                                                Int32?       Exponent   = null)

            where TNumber : INumberBase<TNumber>

        {

            Celsius = default;

            if (!MathHelpers.TryAddExponent(Exponent, 0, out var combinedExponent))
                return false;

            try
            {
                return TryCreate(Decimal.CreateChecked(Number),
                                 combinedExponent,
                                 out Celsius);
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


        #region ToKelvin()

        /// <summary>
        /// Convert this Celsius into Kelvin (K).
        /// </summary>
        public Kelvin ToKelvin()

            => Kelvin.FromK(Value + 273.15m);

        #endregion


        #region Operator overloading

        #region Operator == (Celsius1, Celsius2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Celsius1">A Celsius value.</param>
        /// <param name="Celsius2">Another Celsius value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Celsius Celsius1,
                                           Celsius Celsius2)

            => Celsius1.Equals(Celsius2);

        #endregion

        #region Operator != (Celsius1, Celsius2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Celsius1">A Celsius value.</param>
        /// <param name="Celsius2">Another Celsius value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Celsius Celsius1,
                                           Celsius Celsius2)

            => !Celsius1.Equals(Celsius2);

        #endregion

        #region Operator <  (Celsius1, Celsius2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Celsius1">A Celsius value.</param>
        /// <param name="Celsius2">Another Celsius value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Celsius Celsius1,
                                          Celsius Celsius2)

            => Celsius1.CompareTo(Celsius2) < 0;

        #endregion

        #region Operator <= (Celsius1, Celsius2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Celsius1">A Celsius value.</param>
        /// <param name="Celsius2">Another Celsius value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Celsius Celsius1,
                                           Celsius Celsius2)

            => Celsius1.CompareTo(Celsius2) <= 0;

        #endregion

        #region Operator >  (Celsius1, Celsius2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Celsius1">A Celsius value.</param>
        /// <param name="Celsius2">Another Celsius value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Celsius Celsius1,
                                          Celsius Celsius2)

            => Celsius1.CompareTo(Celsius2) > 0;

        #endregion

        #region Operator >= (Celsius1, Celsius2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Celsius1">A Celsius value.</param>
        /// <param name="Celsius2">Another Celsius value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Celsius Celsius1,
                                           Celsius Celsius2)

            => Celsius1.CompareTo(Celsius2) >= 0;

        #endregion

        #region Operator +  (Celsius1, Celsius2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Celsius1">A Celsius value.</param>
        /// <param name="Celsius2">Another Celsius value.</param>
        public static Celsius operator + (Celsius Celsius1,
                                          Celsius Celsius2)

            => new (Celsius1.Value + Celsius2.Value);

        #endregion

        #region Operator -  (Celsius1, Celsius2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Celsius1">A Celsius value.</param>
        /// <param name="Celsius2">Another Celsius value.</param>
        public static Celsius operator - (Celsius Celsius1,
                                          Celsius Celsius2)

            => new (Celsius1.Value - Celsius2.Value);

        #endregion


        #region Operator *  (Celsius,  Scalar)

        /// <summary>
        /// Multiplies a Celsius with a scalar.
        /// </summary>
        /// <param name="Celsius">A Celsius value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Celsius operator * (Celsius  Celsius,
                                          Decimal  Scalar)

            => new (Celsius.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,   Celsius)

        /// <summary>
        /// Multiplies a scalar with a Celsius.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Celsius">A Celsius value.</param>
        public static Celsius operator * (Decimal  Scalar,
                                          Celsius  Celsius)

            => new (Scalar * Celsius.Value);

        #endregion

        #region Operator /  (Celsius,  Scalar)

        /// <summary>
        /// Divides a Celsius with a scalar.
        /// </summary>
        /// <param name="Celsius">A Celsius value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Celsius operator / (Celsius  Celsius,
                                          Decimal  Scalar)

            => new (Celsius.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Celsius> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Celsius.
        /// </summary>
        /// <param name="Object">A Celsius to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null             => 1,
                   Celsius celsius  => CompareTo(celsius),
                   _                => throw new ArgumentException("The given object is not a Celsius!", nameof(Object))
               };

        #endregion

        #region CompareTo(Celsius)

        /// <summary>
        /// Compares two Celsius.
        /// </summary>
        /// <param name="Celsius">A Celsius to compare with.</param>
        public Int32 CompareTo(Celsius Celsius)

            => Value.CompareTo(Celsius.Value);

        #endregion

        #endregion

        #region IEquatable<Celsius> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Celsius for equality.
        /// </summary>
        /// <param name="Object">A Celsius to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Celsius celsius &&
                   Equals(celsius);

        #endregion

        #region Equals(Celsius)

        /// <summary>
        /// Compares two Celsius for equality.
        /// </summary>
        /// <param name="Celsius">A Celsius to compare with.</param>
        public Boolean Equals(Celsius Celsius)

            => Value.Equals(Celsius.Value);

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
        /// Try to format this Celsius into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("°C".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " °C".AsSpan()
                       );
            }

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " °C".AsSpan()
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
                String.Equals(Format, "°C", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} °C";
            }

            return $"{Value.ToString(Format, FormatProvider)} °C";

        }

        #endregion

    }

}
