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
    /// Extension methods for Volts.
    /// </summary>
    public static class VoltExtensions
    {

        #region Sum    (this VoltValues)

        /// <summary>
        /// The sum of the given enumeration of Volt values.
        /// </summary>
        /// <param name="VoltValues">An enumeration of Volt values.</param>
        public static Volt Sum(this IEnumerable<Volt> VoltValues)
        {

            var sum = Volt.Zero;

            foreach (var volt in VoltValues)
                sum += volt;

            return sum;

        }

        #endregion

        #region Avg    (this VoltValues)

        /// <summary>
        /// The average of the given enumeration of Volt values.
        /// </summary>
        /// <param name="VoltValues">An enumeration of Volt values.</param>
        public static Volt Avg(this IEnumerable<Volt> VoltValues)
        {

            var sum    = Volt.Zero;
            var count  = 0;

            foreach (var volt in VoltValues)
            {
                sum += volt;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this VoltValues)

        /// <summary>
        /// The standard deviation of the given enumeration of Volt values.
        /// </summary>
        /// <param name="VoltValues">An enumeration of Volt values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Volt> StdDev(this IEnumerable<Volt>  VoltValues,
                                          Boolean?                IsSampleData   = null)
        {

            var stdDev = StdDev<Volt>.From(
                             VoltValues.Select(volt => volt.Value),
                             IsSampleData
                         );

            return new StdDev<Volt>(
                       Volt.FromV(stdDev.Mean),
                       Volt.FromV(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Volt value (V), the SI unit of electric potential.
    /// </summary>
    public readonly struct Volt : IMetrology<Volt>
    {

        #region Properties

        /// <summary>
        /// The value of the Volt.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Volt.
        /// </summary>
        public Int64    RoundedIntegerValue

            => Decimal.ToInt64(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as KiloVolts.
        /// </summary>
        public Decimal  kV
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The zero value of the Volt.
        /// </summary>
        public static readonly Volt Zero = new (0m);

        /// <summary>
        /// The additive identity of Ampere.
        /// </summary>
        public static Volt AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Volt (V) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of volts (V).</param>
        private Volt(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as volts using invariant culture.
        /// Supports optional suffixes "V" and "kV".
        /// </summary>
        /// <param name="Text">A text representation of volts.</param>
        public static Volt Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse      (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as volts using the given format provider.
        /// Supports optional suffixes "V" and "kV".
        /// </summary>
        /// <param name="Text">A text representation of volts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Volt Parse(String            Text,
                                 IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var ampere))
                return ampere;

            throw new FormatException($"Invalid text representation of volts: '{Text}'!");

        }

        #endregion

        #region (static) Parse      (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as volts using the given format provider.
        /// Supports optional suffixes "V" and "kV".
        /// </summary>
        /// <param name="Span">A text representation of volts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Volt Parse(ReadOnlySpan<Char>  Span,
                                 IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var ampere))
                return ampere;

            throw new FormatException($"Invalid text representation of volts: '{Span}'!");

        }

        #endregion

        #region (static) ParseV     (Text)

        /// <summary>
        /// Parse the given string as volts.
        /// </summary>
        /// <param name="Text">A text representation of volts.</param>
        public static Volt ParseV(String Text)
        {

            if (TryParseV(Text, out var volt))
                return volt;

            throw new FormatException($"Invalid text representation of volts: '{Text}'!");

        }

        #endregion

        #region (static) ParseKV    (Text)

        /// <summary>
        /// Parse the given string as kiloVolts.
        /// </summary>
        /// <param name="Text">A text representation of kiloVolts.</param>
        public static Volt ParseKV(String Text)
        {

            if (TryParseKV(Text, out var volt))
                return volt;

            throw new FormatException($"Invalid text representation of kiloVolts: '{Text}'!");

        }

        #endregion


        #region (static) FromV      (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into volts.
        /// </summary>
        /// <param name="Number">A numeric representation of volts.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt FromV(Decimal  Number,
                                 Int32?   Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into volts.
        /// </summary>
        /// <param name="Number">A numeric representation of volts.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt FromV(Byte    Number,
                                 Int32?  Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromKV     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into kiloVolts.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloVolts.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt FromKV(Decimal  Number,
                                  Int32?   Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into kiloVolts.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloVolts.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt FromKV(Byte    Number,
                                  Int32?  Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as volts.
        /// </summary>
        /// <param name="Text">A text representation of volts.</param>
        public static Volt? TryParse(String Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var volt))
                return volt;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as volts with an optional unit suffix ("V" or "kV")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of volts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static Volt? TryParse(String?           Text,
                                     IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var volt))
                return volt;

            return null;

        }

        #endregion

        #region (static) TryParseV  (Text)

        /// <summary>
        /// Try to parse the given text as volts.
        /// </summary>
        /// <param name="Text">A text representation of volts.</param>
        public static Volt? TryParseV(String? Text)
        {

            if (TryParseV(Text, out var volt))
                return volt;

            return null;

        }

        #endregion

        #region (static) TryParseKV (Text)

        /// <summary>
        /// Try to parse the given text as kiloVolts.
        /// </summary>
        /// <param name="Text">A text representation of kiloVolts.</param>
        public static Volt? TryParseKV(String? Text)
        {

            if (TryParseKV(Text, out var volt))
                return volt;

            return null;

        }

        #endregion


        #region (static) TryFromV   (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into volts.
        /// </summary>
        /// <param name="Number">A numeric representation of volts.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt? TryFromV(Decimal  Number,
                                     Int32?   Exponent = null)
        {

            if (TryFromV(Number, out var volt, Exponent))
                return volt;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into volts.
        /// </summary>
        /// <param name="Number">A numeric representation of volts.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt? TryFromV(Byte    Number,
                                     Int32?  Exponent = null)
        {

            if (TryFromV(Number, out var volt, Exponent))
                return volt;

            return null;

        }

        #endregion

        #region (static) TryFromKV  (Number, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kiloVolts.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloVolts.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt? TryFromKV(Decimal  Number,
                                      Int32?   Exponent = null)
        {

            if (TryFromKV(Number, out var volt, Exponent))
                return volt;

            return null;

        }


        /// <summary>
        /// Try to convert the given number into kiloVolts.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloVolts.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt? TryFromKV(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryFromKV(Number, out var volt, Exponent))
                return volt;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,                 out Volt)

        /// <summary>
        /// Try to parse the given string as volts using invariant culture.
        /// Supports optional suffixes "V" and "kV".
        /// </summary>
        /// <param name="Text">A text representation of volts.</param>
        /// <param name="Volt">The parsed Volt.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out                 Volt     Volt)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out Volt);

        #endregion

        #region (static) TryParse   (Text, FormatProvider, out Volt)

        /// <summary>
        /// Try to parse the given string as volts using the given format provider.
        /// Supports optional suffixes "V" and "kV".
        /// </summary>
        /// <param name="Text">A text representation of volts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Volt">The parsed Volt.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out Volt                     Volt)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out Volt);

        #endregion

        #region (static) TryParse   (Span, FormatProvider, out Volt)

        /// <summary>
        /// Try to parse the given text span as volts using the given format provider.
        /// Supports optional suffixes "V" and "kV".
        /// </summary>
        /// <param name="Span">A text representation of volts.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="Volt">The parsed Volt.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>  Span,
                                       IFormatProvider?    FormatProvider,
                                       out Volt            Volt)
        {

            Volt = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kV".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^2].TrimEnd();
            }

            else if (Span.EndsWith("V".AsSpan(),  StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out Volt);
            }

            return false;

        }

        #endregion

        #region (static) TryParseV  (Text,                 out Volt)

        /// <summary>
        /// Try to parse the given string as volts using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of volts.</param>
        /// <param name="Volt">The parsed Volt.</param>
        public static Boolean TryParseV([NotNullWhen(true)] String?  Text,
                                        out                 Volt     Volt)
        {

            Volt = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out Volt);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKV (Text,                 out Volt)

        /// <summary>
        /// Try to parse the given string as kiloVolts using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of kiloVolts.</param>
        /// <param name="Volt">The parsed Volt.</param>
        public static Boolean TryParseKV([NotNullWhen(true)] String?  Text,
                                         out                 Volt     Volt)
        {

            Volt = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out Volt);
            }

            return false;

        }

        #endregion


        #region (private static) TryCreate(Number, Exponent, out Volt)

        private static Boolean TryCreate(Decimal   Number,
                                         Int32     Exponent,
                                         out Volt  Volt)
        {

            Volt = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                Volt = Zero;
                return true;
            }

            try
            {
                Volt = new Volt(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) TryFromV   (Number, out Volt, Exponent = null)

        /// <summary>
        /// Try to convert the given number into volts.
        /// </summary>
        /// <param name="Number">A numeric representation of volts.</param>
        /// <param name="Volt">The parsed Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromV(Byte      Number,
                                       out Volt  Volt,
                                       Int32?    Exponent = null)

            => TryCreate(Number, Exponent ?? 0, out Volt);


        /// <summary>
        /// Try to convert the given number into volts.
        /// </summary>
        /// <param name="Number">A numeric representation of volts.</param>
        /// <param name="Volt">The parsed Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromV(Decimal   Number,
                                       out Volt  Volt,
                                       Int32?    Exponent = null)

            => TryCreate(Number, Exponent ?? 0, out Volt);

        #endregion

        #region (static) TryFromKV  (Number, out Volt, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kiloVolts.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloVolts.</param>
        /// <param name="Volt">The parsed Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKV(Byte      Number,
                                        out Volt  Volt,
                                        Int32?    Exponent = null)
        {

            Volt = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out Volt);

        }


        /// <summary>
        /// Try to convert the given number into kiloVolts.
        /// </summary>
        /// <param name="Number">A numeric representation of kiloVolts.</param>
        /// <param name="Volt">The parsed Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKV(Decimal   Number,
                                        out Volt  Volt,
                                        Int32?    Exponent = null)
        {

            Volt = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out Volt);

        }

        #endregion


        #region Operator overloading

        #region Operator == (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Volt Volt1,
                                           Volt Volt2)

            => Volt1.Equals(Volt2);

        #endregion

        #region Operator != (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Volt Volt1,
                                           Volt Volt2)

            => !Volt1.Equals(Volt2);

        #endregion

        #region Operator <  (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Volt Volt1,
                                          Volt Volt2)

            => Volt1.CompareTo(Volt2) < 0;

        #endregion

        #region Operator <= (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Volt Volt1,
                                           Volt Volt2)

            => Volt1.CompareTo(Volt2) <= 0;

        #endregion

        #region Operator >  (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Volt Volt1,
                                          Volt Volt2)

            => Volt1.CompareTo(Volt2) > 0;

        #endregion

        #region Operator >= (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Volt Volt1,
                                           Volt Volt2)

            => Volt1.CompareTo(Volt2) >= 0;

        #endregion

        #region Operator +  (Volt1, Volt2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        public static Volt operator + (Volt Volt1,
                                       Volt Volt2)

            => new (Volt1.Value + Volt2.Value);

        #endregion

        #region Operator -  (Volt1, Volt2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        public static Volt operator - (Volt Volt1,
                                       Volt Volt2)

            => new (Volt1.Value - Volt2.Value);

        #endregion


        #region Operator *  (Volt,   Scalar)

        /// <summary>
        /// Multiplies a Volt with a scalar.
        /// </summary>
        /// <param name="Volt">A Volt value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Volt operator * (Volt    Volt,
                                       Decimal Scalar)

            => new (Volt.Value * Scalar);

        #endregion

        #region Operator *  (Scalar, Volt)

        /// <summary>
        /// Multiplies a scalar with a Volt.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Volt">A Volt value.</param>
        public static Volt operator * (Decimal Scalar,
                                       Volt    Volt)

            => new (Scalar * Volt.Value);

        #endregion

        #region Operator /  (Volt,   Scalar)

        /// <summary>
        /// Divides a Volt with a scalar.
        /// </summary>
        /// <param name="Volt">A Volt value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Volt operator / (Volt    Volt,
                                       Decimal Scalar)

            => new (Volt.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Volt> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Volts.
        /// </summary>
        /// <param name="Object">A Volt to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null       => 1,
                   Volt volt  => CompareTo(volt),
                   _          => throw new ArgumentException("The given object is not a Volt!", nameof(Object))
               };

        #endregion

        #region CompareTo(Volt)

        /// <summary>
        /// Compares two Volts.
        /// </summary>
        /// <param name="Volt">A Volt to compare with.</param>
        public Int32 CompareTo(Volt Volt)

            => Value.CompareTo(Volt.Value);

        #endregion

        #endregion

        #region IEquatable<Volt> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Volts for equality.
        /// </summary>
        /// <param name="Object">A Volt to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Volt volt &&
                   Equals(volt);

        #endregion

        #region Equals(Volt)

        /// <summary>
        /// Compares two Volts for equality.
        /// </summary>
        /// <param name="Volt">A Volt to compare with.</param>
        public Boolean Equals(Volt Volt)

            => Value.Equals(Volt.Value);

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
        /// Try to format this Volt into the given character span using the given format and culture-specific format provider.
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
                Format.Equals("V".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " V".AsSpan()
                       );
            }

            if (Format.Equals("kV".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kV,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kV".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " V".AsSpan()
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
                String.Equals(Format, "V",  StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} V";
            }

            if (String.Equals(Format, "kV", StringComparison.OrdinalIgnoreCase))
                return $"{kV.ToString("G", FormatProvider)} kV";

            return $"{Value.ToString(Format, FormatProvider)} V";

        }

        #endregion

    }

}
