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
    /// Extension methods for Volt-Ampere-Reactives.
    /// </summary>
    public static class VoltAmpereReactiveExtensions
    {

        #region Sum    (this VoltAmpereReactiveValues)

        /// <summary>
        /// The sum of the given enumeration of VoltAmpereReactive values.
        /// </summary>
        /// <param name="VoltAmpereReactiveValues">An enumeration of VoltAmpereReactive values.</param>
        public static VoltAmpereReactive Sum(this IEnumerable<VoltAmpereReactive> VoltAmpereReactiveValues)
        {

            var sum = VoltAmpereReactive.Zero;

            foreach (var voltAmpereReactive in VoltAmpereReactiveValues)
                sum += voltAmpereReactive;

            return sum;

        }

        #endregion

        #region Avg    (this VoltAmpereReactiveValues)

        /// <summary>
        /// The average of the given enumeration of VoltAmpereReactive values.
        /// </summary>
        /// <param name="VoltAmpereReactiveValues">An enumeration of VoltAmpereReactive values.</param>
        public static VoltAmpereReactive Avg(this IEnumerable<VoltAmpereReactive> VoltAmpereReactiveValues)
        {

            var sum    = VoltAmpereReactive.Zero;
            var count  = 0;

            foreach (var voltAmpereReactive in VoltAmpereReactiveValues)
            {
                sum += voltAmpereReactive;
                count++;
            }

            return count > 0
                       ? sum / count
                       : throw new InvalidOperationException("The sequence must not be empty!");

        }

        #endregion

        #region StdDev (this VoltAmpereReactiveValues)

        /// <summary>
        /// The standard deviation of the given enumeration of VoltAmpereReactive values.
        /// </summary>
        /// <param name="VoltAmpereReactiveValues">An enumeration of VoltAmpereReactive values.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<VoltAmpereReactive> StdDev(this IEnumerable<VoltAmpereReactive>  VoltAmpereReactiveValues,
                                                        Boolean?                              IsSampleData   = null)
        {

            var stdDev = StdDev<VoltAmpereReactive>.From(
                             VoltAmpereReactiveValues.Select(voltAmpereReactive => voltAmpereReactive.Value),
                             IsSampleData
                         );

            return new StdDev<VoltAmpereReactive>(
                       VoltAmpereReactive.FromVAr(stdDev.Mean),
                       VoltAmpereReactive.FromVAr(stdDev.StandardDeviation)
                   );

        }

        #endregion

    }


    /// <summary>
    /// A Volt-Ampere-Reactive (VAr), the SI unit of reactive power in an AC electric power system.
    /// </summary>
    public readonly struct VoltAmpereReactive : IMetrology<VoltAmpereReactive>
    {

        #region Properties

        /// <summary>
        /// The value of the VoltAmpereReactive.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the VoltAmpereReactive.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as Kilo-Volt-Ampere.
        /// </summary>
        public Decimal  kVA
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles


        /// <summary>
        /// The zero value of the VoltAmpereReactive.
        /// </summary>
        public static readonly VoltAmpereReactive Zero = new (0m);

        /// <summary>
        /// The additive identity of VoltAmpereReactive.
        /// </summary>
        public static VoltAmpereReactive AdditiveIdentity
            => Zero;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Volt-Ampere-Reactive (VAr) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a Volt-Ampere-Reactive (VAr).</param>
        private VoltAmpereReactive(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse        (Text)

        /// <summary>
        /// Parse the given string as volt-ampere-reactives using invariant culture.
        /// Supports optional suffixes "VAr" and "kVAr".
        /// </summary>
        /// <param name="Text">A text representation of volt-ampere-reactives.</param>
        public static VoltAmpereReactive Parse(String Text)

            => Parse(Text, CultureInfo.InvariantCulture);

        #endregion

        #region (static) Parse        (Text, FormatProvider)

        /// <summary>
        /// Parse the given string as volt-ampere-reactives using the given format provider.
        /// Supports optional suffixes "VAr" and "kVAr".
        /// </summary>
        /// <param name="Text">A text representation of volt-ampere-reactives.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static VoltAmpereReactive Parse(String            Text,
                                               IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var voltAmpereReactive))
                return voltAmpereReactive;

            throw new FormatException($"Invalid text representation of volt-ampere-reactives: '{Text}'!");

        }

        #endregion

        #region (static) Parse        (Span, FormatProvider)

        /// <summary>
        /// Parse the given text span as volt-ampere-reactives using the given format provider.
        /// Supports optional suffixes "VAr" and "kVAr".
        /// </summary>
        /// <param name="Span">A text representation of volt-ampere-reactives.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static VoltAmpereReactive Parse(ReadOnlySpan<Char>  Span,
                                               IFormatProvider?    FormatProvider)
        {

            if (TryParse(Span, FormatProvider, out var voltAmpereReactive))
                return voltAmpereReactive;

            throw new FormatException($"Invalid text representation of volt-ampere-reactives: '{Span}'!");

        }

        #endregion

        #region (static) ParseVAr     (Text)

        /// <summary>
        /// Parse the given string as a Volt-Ampere-Reactive (VAr).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere-Reactive (VAr).</param>
        public static VoltAmpereReactive ParseVAr(String Text)
        {

            if (TryParseVAr(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            throw new ArgumentException($"Invalid text representation of volt-ampere-reactives: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKVAr    (Text)

        /// <summary>
        /// Parse the given string as kilo-Volt-Ampere-Reactives.
        /// </summary>
        /// <param name="Text">A text representation of kilo-Volt-Ampere-Reactives.</param>
        public static VoltAmpereReactive ParseKVAr(String Text)
        {

            if (TryParseKVAr(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            throw new ArgumentException($"Invalid text representation of kilo-volt-ampere-reactives: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) FromVAr      (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of volt-ampere-reactives.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive FromVAr(Decimal  Number,
                                                 Int32?   Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of volt-ampere-reactives.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive FromVAr(Byte    Number,
                                                 Int32?  Exponent = null)

            => new (Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion

        #region (static) FromKVAr     (Number, Exponent = null)

        /// <summary>
        /// Convert the given number into kilo-volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of kilo-volt-ampere-reactives.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive FromKVAr(Decimal  Number,
                                                  Int32?   Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));


        /// <summary>
        /// Convert the given number into kilo-volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of kilo-volt-ampere-reactives.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive FromKVAr(Byte    Number,
                                                  Int32?  Exponent = null)

            => new (1000m * Number * MathHelpers.Pow10(Exponent ?? 0));

        #endregion


        #region (static) TryParse     (Text)

        /// <summary>
        /// Try to parse the given text as volt-ampere-reactives with an optional unit suffix ("VAr" or "kVAr")
        /// using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of volt-ampere-reactives.</param>
        public static VoltAmpereReactive? TryParse(String? Text)
        {

            if (TryParse(Text, CultureInfo.InvariantCulture, out var voltAmpereReactive))
                return voltAmpereReactive;

            return null;

        }

        #endregion

        #region (static) TryParse     (Text, FormatProvider)

        /// <summary>
        /// Try to parse the given text as volt-ampere-reactives with an optional unit suffix ("VAr" or "kVAr")
        /// using the given format provider.
        /// </summary>
        /// <param name="Text">A text representation of volt-ampere-reactives.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        public static VoltAmpereReactive? TryParse(String?           Text,
                                                   IFormatProvider?  FormatProvider)
        {

            if (TryParse(Text, FormatProvider, out var voltAmpereReactive))
                return voltAmpereReactive;

            return null;

        }

        #endregion

        #region (static) TryParseVAr  (Text)

        /// <summary>
        /// Try to parse the given text as volt-ampere-reactives.
        /// </summary>
        /// <param name="Text">A text representation of volt-ampere-reactives.</param>
        public static VoltAmpereReactive? TryParseVAr(String? Text)
        {

            if (TryParseVAr(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            return null;

        }

        #endregion

        #region (static) TryParseKVAr (Text)

        /// <summary>
        /// Try to parse the given text as kilo-volt-ampere-reactives.
        /// </summary>
        /// <param name="Text">A text representation of kilo-volt-ampere-reactives.</param>
        public static VoltAmpereReactive? TryParseKVAr(String? Text)
        {

            if (TryParseKVAr(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            return null;

        }

        #endregion


        #region (static) TryFromVAr   (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of volt-ampere-reactives.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive? TryFromVAr(Decimal  Number,
                                                     Int32?   Exponent = null)
        {

            if (TryFromVAr(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of volt-ampere-reactives.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive? TryFromVAr(Byte    Number,
                                                     Int32?  Exponent = null)
        {

            if (TryFromVAr(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            return null;

        }

        #endregion

        #region (static) TryFromKVAr  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as kilo-volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of kilo-volt-ampere-reactives.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive? TryFromKVAr(Decimal  Number,
                                                      Int32?   Exponent = null)
        {

            if (TryFromKVAr(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as kilo-volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of kilo-volt-ampere-reactives.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive? TryFromKVAr(Byte    Number,
                                                      Int32?  Exponent = null)
        {

            if (TryFromKVAr(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            return null;

        }

        #endregion


        #region (static) TryParse     (Text,                 out VoltAmpereReactive)

        /// <summary>
        /// Try to parse the given string as volt-ampere-reactives using invariant culture.
        /// Supports optional suffixes "VAr" and "kVAr".
        /// </summary>
        /// <param name="Text">A text representation of volt-ampere-reactives.</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?             Text,
                                       out                 VoltAmpereReactive  VoltAmpereReactive)

            => TryParse(Text.AsSpan(),
                        CultureInfo.InvariantCulture,
                        out VoltAmpereReactive);

        #endregion

        #region (static) TryParse     (Text, FormatProvider, out VoltAmpereReactive)

        /// <summary>
        /// Try to parse the given string as volt-ampere-reactives using the given format provider.
        /// Supports optional suffixes "VAr" and "kVAr".
        /// </summary>
        /// <param name="Text">A text representation of volt-ampere-reactives.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             FormatProvider,
                                       out VoltAmpereReactive       VoltAmpereReactive)

            => TryParse(Text.AsSpan(),
                        FormatProvider,
                        out VoltAmpereReactive);

        #endregion

        #region (static) TryParse     (Span, FormatProvider, out VoltAmpereReactive)

        /// <summary>
        /// Try to parse the given text span as volt-ampere-reactives using the given format provider.
        /// Supports optional suffixes "VAr" and "kVAr".
        /// </summary>
        /// <param name="Span">A text representation of volt-ampere-reactives.</param>
        /// <param name="FormatProvider">An optional format provider.</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        public static Boolean TryParse(ReadOnlySpan<Char>      Span,
                                       IFormatProvider?        FormatProvider,
                                       out VoltAmpereReactive  VoltAmpereReactive)
        {

            VoltAmpereReactive = default;

            Span = Span.Trim();

            if (Span.IsEmpty)
                return false;

            var exponent  = 0;

            if      (Span.EndsWith("kVAr".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                exponent  = 3;
                Span      = Span[..^4].TrimEnd();
            }

            else if (Span.EndsWith("VAr".AsSpan(),  StringComparison.OrdinalIgnoreCase))
            {
                Span      = Span[..^3].TrimEnd();
            }

            if (Decimal.TryParse(Span,
                                 NumberStyles.Number,
                                 NumberFormatInfo.GetInstance(FormatProvider),
                                 out var value))
            {
                return TryCreate(value, exponent, out VoltAmpereReactive);
            }

            return false;

        }

        #endregion

        #region (static) TryParseVAr  (Text,                 out VoltAmpereReactive)

        /// <summary>
        /// Try to parse the given string as volt-ampere-reactives using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of volt-ampere-reactives.</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        public static Boolean TryParseVAr([NotNullWhen(true)] String?             Text,
                                          out                 VoltAmpereReactive  VoltAmpereReactive)
        {

            VoltAmpereReactive = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 0, out VoltAmpereReactive);
            }

            return false;

        }

        #endregion

        #region (static) TryParseKVAr (Text,                 out VoltAmpereReactive)

        /// <summary>
        /// Try to parse the given string as kilo-volt-ampere-reactives using invariant culture.
        /// </summary>
        /// <param name="Text">A text representation of kilo-volt-ampere-reactives.</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        public static Boolean TryParseKVAr([NotNullWhen(true)] String?             Text,
                                           out                 VoltAmpereReactive  VoltAmpereReactive)
        {

            VoltAmpereReactive = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            if (Decimal.TryParse(Text.Trim(),
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {
                return TryCreate(value, 3, out VoltAmpereReactive);
            }

            return false;

        }

        #endregion


        #region (private static) TryCreate(Number, Exponent, out VoltAmpereReactive)

        private static Boolean TryCreate(Decimal                 Number,
                                         Int32                   Exponent,
                                         out VoltAmpereReactive  VoltAmpereReactive)
        {

            VoltAmpereReactive = default;

            if (Exponent < -28 || Exponent > 28)
                return false;

            if (Number == 0m)
            {
                VoltAmpereReactive = Zero;
                return true;
            }

            try
            {
                VoltAmpereReactive = new VoltAmpereReactive(Number * MathHelpers.Pow10(Exponent));
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

        #region (static) TryFromVAr   (Number, out VoltAmpereReactive, Exponent = null)

        /// <summary>
        /// Try to convert the given number into volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of volt-ampere-reactives.</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromVAr(Byte                    Number,
                                         out VoltAmpereReactive  VoltAmpereReactive,
                                         Int32?                  Exponent = null)
        {

            VoltAmpereReactive = default;

            return MathHelpers.TryAddExponent(Exponent, 0, out var exponent) &&
                   TryCreate(Number, exponent, out VoltAmpereReactive);

        }


        /// <summary>
        /// Try to convert the given number into volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of volt-ampere-reactives.</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromVAr(Decimal                 Number,
                                         out VoltAmpereReactive  VoltAmpereReactive,
                                         Int32?                  Exponent = null)
        {

            VoltAmpereReactive = default;

            return MathHelpers.TryAddExponent(Exponent, 0, out var exponent) &&
                   TryCreate(Number, exponent, out VoltAmpereReactive);

        }

        #endregion

        #region (static) TryFromKVAr  (Number, out VoltAmpereReactive, Exponent = null)

        /// <summary>
        /// Try to convert the given number into kilo-volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of kilo-volt-ampere-reactives.</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKVAr(Byte                    Number,
                                          out VoltAmpereReactive  VoltAmpereReactive,
                                          Int32?                  Exponent = null)
        {

            VoltAmpereReactive = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out VoltAmpereReactive);

        }


        /// <summary>
        /// Try to convert the given number into kilo-volt-ampere-reactives.
        /// </summary>
        /// <param name="Number">A numeric representation of kilo-volt-ampere-reactives.</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKVAr(Decimal                Number,
                                         out VoltAmpereReactive  VoltAmpereReactive,
                                         Int32?                  Exponent = null)
        {

            VoltAmpereReactive = default;

            return MathHelpers.TryAddExponent(Exponent, 3, out var exponent) &&
                   TryCreate(Number, exponent, out VoltAmpereReactive);

        }

        #endregion


        #region Operator overloading

        #region Operator == (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A VoltAmpereReactive value.</param>
        /// <param name="VoltAmpereReactive2">Another VoltAmpereReactive value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VoltAmpereReactive VoltAmpereReactive1,
                                           VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.Equals(VoltAmpereReactive2);

        #endregion

        #region Operator != (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A VoltAmpereReactive value.</param>
        /// <param name="VoltAmpereReactive2">Another VoltAmpereReactive value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VoltAmpereReactive VoltAmpereReactive1,
                                           VoltAmpereReactive VoltAmpereReactive2)

            => !VoltAmpereReactive1.Equals(VoltAmpereReactive2);

        #endregion

        #region Operator <  (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A VoltAmpereReactive value.</param>
        /// <param name="VoltAmpereReactive2">Another VoltAmpereReactive value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (VoltAmpereReactive VoltAmpereReactive1,
                                          VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.CompareTo(VoltAmpereReactive2) < 0;

        #endregion

        #region Operator <= (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A VoltAmpereReactive value.</param>
        /// <param name="VoltAmpereReactive2">Another VoltAmpereReactive value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (VoltAmpereReactive VoltAmpereReactive1,
                                           VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.CompareTo(VoltAmpereReactive2) <= 0;

        #endregion

        #region Operator >  (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A VoltAmpereReactive value.</param>
        /// <param name="VoltAmpereReactive2">Another VoltAmpereReactive value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (VoltAmpereReactive VoltAmpereReactive1,
                                          VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.CompareTo(VoltAmpereReactive2) > 0;

        #endregion

        #region Operator >= (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A VoltAmpereReactive value.</param>
        /// <param name="VoltAmpereReactive2">Another VoltAmpereReactive value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (VoltAmpereReactive VoltAmpereReactive1,
                                           VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.CompareTo(VoltAmpereReactive2) >= 0;

        #endregion

        #region Operator +  (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Accumulates two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A VoltAmpereReactive value.</param>
        /// <param name="VoltAmpereReactive2">Another VoltAmpereReactive value.</param>
        public static VoltAmpereReactive operator + (VoltAmpereReactive VoltAmpereReactive1,
                                             VoltAmpereReactive VoltAmpereReactive2)

            => new (VoltAmpereReactive1.Value + VoltAmpereReactive2.Value);

        #endregion

        #region Operator -  (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Subtracts two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A VoltAmpereReactive value.</param>
        /// <param name="VoltAmpereReactive2">Another VoltAmpereReactive value.</param>
        public static VoltAmpereReactive operator - (VoltAmpereReactive VoltAmpereReactive1,
                                             VoltAmpereReactive VoltAmpereReactive2)

            => new (VoltAmpereReactive1.Value - VoltAmpereReactive2.Value);

        #endregion


        #region Operator *  (VoltAmpereReactive,  Scalar)

        /// <summary>
        /// Multiplies a VoltAmpereReactive with a scalar.
        /// </summary>
        /// <param name="VoltAmpereReactive">A VoltAmpereReactive value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static VoltAmpereReactive operator * (VoltAmpereReactive  VoltAmpereReactive,
                                             Decimal     Scalar)

            => new (VoltAmpereReactive.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,              VoltAmpereReactive)

        /// <summary>
        /// Multiplies a scalar with a VoltAmpereReactive.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="VoltAmpereReactive">A VoltAmpereReactive value.</param>
        public static VoltAmpereReactive operator * (Decimal     Scalar,
                                             VoltAmpereReactive  VoltAmpereReactive)

            => new (Scalar * VoltAmpereReactive.Value);

        #endregion

        #region Operator /  (VoltAmpereReactive,  Scalar)

        /// <summary>
        /// Divides a VoltAmpereReactive with a scalar.
        /// </summary>
        /// <param name="VoltAmpereReactive">A VoltAmpereReactive value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static VoltAmpereReactive operator / (VoltAmpereReactive  VoltAmpereReactive,
                                             Decimal     Scalar)

            => new (VoltAmpereReactive.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<VoltAmpereReactive> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Volt-Ampere-Reactives.
        /// </summary>
        /// <param name="Object">A Volt-Ampere-Reactive to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null                                   => 1,
                   VoltAmpereReactive voltAmpereReactive  => CompareTo(voltAmpereReactive),
                   _                                      => throw new ArgumentException("The given object is not a VoltAmpereReactive!", nameof(Object))
               };

        #endregion

        #region CompareTo(VoltAmpereReactive)

        /// <summary>
        /// Compares two Volt-Ampere-Reactives.
        /// </summary>
        /// <param name="VoltAmpereReactive">A Volt-Ampere-Reactive to compare with.</param>
        public Int32 CompareTo(VoltAmpereReactive VoltAmpereReactive)

            => Value.CompareTo(VoltAmpereReactive.Value);

        #endregion

        #endregion

        #region IEquatable<VoltAmpereReactive> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Volt-Ampere-Reactives for equality.
        /// </summary>
        /// <param name="Object">A Volt-Ampere-Reactive to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VoltAmpereReactive voltAmpereReactive &&
                   Equals(voltAmpereReactive);

        #endregion

        #region Equals(VoltAmpereReactive)

        /// <summary>
        /// Compares two Volt-Ampere-Reactives for equality.
        /// </summary>
        /// <param name="VoltAmpereReactive">A Volt-Ampere-Reactive to compare with.</param>
        public Boolean Equals(VoltAmpereReactive VoltAmpereReactive)

            => Value.Equals(VoltAmpereReactive.Value);

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
                Format.Equals("G".  AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                Format.Equals("VAr".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return TryFormatWithSuffix(
                           Value,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " VAr".AsSpan()
                       );
            }

            if (Format.Equals("kVAr".AsSpan(), StringComparison.OrdinalIgnoreCase))
                return TryFormatWithSuffix(
                           kVA,
                           Destination,
                           out CharsWritten,
                           "G".AsSpan(),
                           FormatProvider,
                           " kVAr".AsSpan()
                       );

            return TryFormatWithSuffix(
                       Value,
                       Destination,
                       out CharsWritten,
                       Format,
                       FormatProvider,
                       " VAr".AsSpan()
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
                String.Equals(Format, "G",   StringComparison.OrdinalIgnoreCase) ||
                String.Equals(Format, "VAr", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} VAr";
            }

            if (String.Equals(Format, "kVAr", StringComparison.OrdinalIgnoreCase))
                return $"{kVA.ToString("G", FormatProvider)} kVAr";

            return $"{Value.ToString(Format, FormatProvider)} VAr";

        }

        #endregion

    }

}
