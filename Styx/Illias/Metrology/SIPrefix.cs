/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An SI prefix (Système international d'unités), including the
    /// 2022 additions ronna/quetta and ronto/quecto, stored as its
    /// decimal power, e.g. 3 for kilo and -3 for milli.
    /// The default value is None (10^0).
    /// </summary>
    public readonly struct SIPrefix : IEquatable <SIPrefix>,
                                      IComparable<SIPrefix>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The decimal power of this SI prefix, e.g. 3 for kilo.
        /// </summary>
        public SByte  Exponent    { get; }


        /// <summary>
        /// The symbol of this SI prefix, e.g. "k" for kilo.
        /// Micro uses the micro sign 'µ' (U+00B5).
        /// </summary>
        public String  Symbol

            => Exponent switch {
                    30  => "Q",
                    27  => "R",
                    24  => "Y",
                    21  => "Z",
                    18  => "E",
                    15  => "P",
                    12  => "T",
                     9  => "G",
                     6  => "M",
                     3  => "k",
                     2  => "h",
                     1  => "da",
                     0  => "",
                    -1  => "d",
                    -2  => "c",
                    -3  => "m",
                    -6  => "µ",
                    -9  => "n",
                   -12  => "p",
                   -15  => "f",
                   -18  => "a",
                   -21  => "z",
                   -24  => "y",
                   -27  => "r",
                   -30  => "q",
                    _   => $"10^{Exponent}"
               };


        /// <summary>
        /// The name of this SI prefix, e.g. "Kilo".
        /// </summary>
        public String  Name

            => Exponent switch {
                    30  => "Quetta",
                    27  => "Ronna",
                    24  => "Yotta",
                    21  => "Zetta",
                    18  => "Exa",
                    15  => "Peta",
                    12  => "Tera",
                     9  => "Giga",
                     6  => "Mega",
                     3  => "Kilo",
                     2  => "Hecto",
                     1  => "Deca",
                     0  => "",
                    -1  => "Deci",
                    -2  => "Centi",
                    -3  => "Milli",
                    -6  => "Micro",
                    -9  => "Nano",
                   -12  => "Pico",
                   -15  => "Femto",
                   -18  => "Atto",
                   -21  => "Zepto",
                   -24  => "Yocto",
                   -27  => "Ronto",
                   -30  => "Quecto",
                    _   => $"10^{Exponent}"
               };


        /// <summary>
        /// The scaling factor of this SI prefix as a decimal.
        /// Quecto and quetta (10^±30) exceed the range of
        /// System.Decimal and therefore throw an
        /// ArgumentOutOfRangeException.
        /// </summary>
        public Decimal  Factor
            => MathHelpers.Pow10(Exponent);


        /// <summary>
        /// Whether this is the neutral prefix (10^0).
        /// </summary>
        public Boolean  IsNone
            => Exponent == 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SI prefix based on the given decimal power.
        /// </summary>
        /// <param name="Exponent">The decimal power of the SI prefix.</param>
        private SIPrefix(SByte Exponent)
        {
            this.Exponent = Exponent;
        }

        #endregion


        #region Static defaults

        /// <summary>
        /// The neutral prefix (10^0).
        /// </summary>
        public static SIPrefix  None      { get; } = new (  0);

        /// <summary>
        /// Quecto (10^-30, "q").
        /// </summary>
        public static SIPrefix  Quecto    { get; } = new (-30);

        /// <summary>
        /// Ronto (10^-27, "r").
        /// </summary>
        public static SIPrefix  Ronto     { get; } = new (-27);

        /// <summary>
        /// Yocto (10^-24, "y").
        /// </summary>
        public static SIPrefix  Yocto     { get; } = new (-24);

        /// <summary>
        /// Zepto (10^-21, "z").
        /// </summary>
        public static SIPrefix  Zepto     { get; } = new (-21);

        /// <summary>
        /// Atto (10^-18, "a").
        /// </summary>
        public static SIPrefix  Atto      { get; } = new (-18);

        /// <summary>
        /// Femto (10^-15, "f").
        /// </summary>
        public static SIPrefix  Femto     { get; } = new (-15);

        /// <summary>
        /// Pico (10^-12, "p").
        /// </summary>
        public static SIPrefix  Pico      { get; } = new (-12);

        /// <summary>
        /// Nano (10^-9, "n").
        /// </summary>
        public static SIPrefix  Nano      { get; } = new ( -9);

        /// <summary>
        /// Micro (10^-6, "µ").
        /// </summary>
        public static SIPrefix  Micro     { get; } = new ( -6);

        /// <summary>
        /// Milli (10^-3, "m").
        /// </summary>
        public static SIPrefix  Milli     { get; } = new ( -3);

        /// <summary>
        /// Centi (10^-2, "c").
        /// </summary>
        public static SIPrefix  Centi     { get; } = new ( -2);

        /// <summary>
        /// Deci (10^-1, "d").
        /// </summary>
        public static SIPrefix  Deci      { get; } = new ( -1);

        /// <summary>
        /// Deca (10^1, "da").
        /// </summary>
        public static SIPrefix  Deca      { get; } = new (  1);

        /// <summary>
        /// Hecto (10^2, "h").
        /// </summary>
        public static SIPrefix  Hecto     { get; } = new (  2);

        /// <summary>
        /// Kilo (10^3, "k").
        /// </summary>
        public static SIPrefix  Kilo      { get; } = new (  3);

        /// <summary>
        /// Mega (10^6, "M").
        /// </summary>
        public static SIPrefix  Mega      { get; } = new (  6);

        /// <summary>
        /// Giga (10^9, "G").
        /// </summary>
        public static SIPrefix  Giga      { get; } = new (  9);

        /// <summary>
        /// Tera (10^12, "T").
        /// </summary>
        public static SIPrefix  Tera      { get; } = new ( 12);

        /// <summary>
        /// Peta (10^15, "P").
        /// </summary>
        public static SIPrefix  Peta      { get; } = new ( 15);

        /// <summary>
        /// Exa (10^18, "E").
        /// </summary>
        public static SIPrefix  Exa       { get; } = new ( 18);

        /// <summary>
        /// Zetta (10^21, "Z").
        /// </summary>
        public static SIPrefix  Zetta     { get; } = new ( 21);

        /// <summary>
        /// Yotta (10^24, "Y").
        /// </summary>
        public static SIPrefix  Yotta     { get; } = new ( 24);

        /// <summary>
        /// Ronna (10^27, "R").
        /// </summary>
        public static SIPrefix  Ronna     { get; } = new ( 27);

        /// <summary>
        /// Quetta (10^30, "Q").
        /// </summary>
        public static SIPrefix  Quetta    { get; } = new ( 30);


        /// <summary>
        /// All 25 SI prefixes, ordered by their decimal power.
        /// </summary>
        public static IReadOnlyList<SIPrefix>  All    { get; }

            = [ Quecto, Ronto, Yocto, Zepto,  Atto, Femto, Pico, Nano, Micro, Milli, Centi, Deci,
                None,
                Deca,   Hecto, Kilo,  Mega,   Giga, Tera,  Peta, Exa,  Zetta, Yotta, Ronna, Quetta ];

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an SI prefix symbol (case-sensitive,
        /// accepting 'µ' U+00B5, 'μ' U+03BC and "u" for micro) or
        /// SI prefix name (case-insensitive).
        /// </summary>
        /// <param name="Text">A text representation of an SI prefix.</param>
        public static SIPrefix Parse(String Text)
        {

            if (TryParse(Text, out var siPrefix))
                return siPrefix;

            throw new ArgumentException($"Invalid text representation of an SI prefix: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an SI prefix.
        /// </summary>
        /// <param name="Text">A text representation of an SI prefix.</param>
        public static SIPrefix? TryParse(String Text)
        {

            if (TryParse(Text, out var siPrefix))
                return siPrefix;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SIPrefix)

        /// <summary>
        /// Try to parse the given text as an SI prefix.
        /// </summary>
        /// <param name="Text">A text representation of an SI prefix.</param>
        /// <param name="SIPrefix">The parsed SI prefix.</param>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out SIPrefix                 SIPrefix)
        {

            Text = Text?.Trim() ?? "";

            // The micro sign 'µ' (U+00B5), the Greek small letter mu 'μ' (U+03BC)
            // and the ASCII fallback "u" are all accepted for micro!
            if (Text == "µ" || Text == "μ" || Text == "u")
            {
                SIPrefix = Micro;
                return true;
            }

            foreach (var siPrefix in All)
            {
                if (String.Equals(siPrefix.Symbol, Text, StringComparison.Ordinal))
                {
                    SIPrefix = siPrefix;
                    return true;
                }
            }

            if (Text.Length > 0)
            {
                foreach (var siPrefix in All)
                {
                    if (String.Equals(siPrefix.Name, Text, StringComparison.OrdinalIgnoreCase))
                    {
                        SIPrefix = siPrefix;
                        return true;
                    }
                }
            }

            SIPrefix = default;
            return false;

        }

        #endregion

        #region (static) TryFrom (Exponent, out SIPrefix)

        /// <summary>
        /// Try to return the SI prefix of the given decimal power.
        /// Only the 25 canonical SI prefix exponents are valid.
        /// </summary>
        /// <param name="Exponent">The decimal power of an SI prefix.</param>
        /// <param name="SIPrefix">The SI prefix.</param>
        public static Boolean TryFrom(Int32         Exponent,
                                      out SIPrefix  SIPrefix)
        {

            foreach (var siPrefix in All)
            {
                if (siPrefix.Exponent == Exponent)
                {
                    SIPrefix = siPrefix;
                    return true;
                }
            }

            SIPrefix = default;
            return false;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SIPrefix1, SIPrefix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SIPrefix1">An SI prefix.</param>
        /// <param name="SIPrefix2">Another SI prefix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SIPrefix SIPrefix1,
                                           SIPrefix SIPrefix2)

            => SIPrefix1.Equals(SIPrefix2);

        #endregion

        #region Operator != (SIPrefix1, SIPrefix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SIPrefix1">An SI prefix.</param>
        /// <param name="SIPrefix2">Another SI prefix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SIPrefix SIPrefix1,
                                           SIPrefix SIPrefix2)

            => !SIPrefix1.Equals(SIPrefix2);

        #endregion

        #region Operator <  (SIPrefix1, SIPrefix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SIPrefix1">An SI prefix.</param>
        /// <param name="SIPrefix2">Another SI prefix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SIPrefix SIPrefix1,
                                          SIPrefix SIPrefix2)

            => SIPrefix1.CompareTo(SIPrefix2) < 0;

        #endregion

        #region Operator <= (SIPrefix1, SIPrefix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SIPrefix1">An SI prefix.</param>
        /// <param name="SIPrefix2">Another SI prefix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SIPrefix SIPrefix1,
                                           SIPrefix SIPrefix2)

            => SIPrefix1.CompareTo(SIPrefix2) <= 0;

        #endregion

        #region Operator >  (SIPrefix1, SIPrefix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SIPrefix1">An SI prefix.</param>
        /// <param name="SIPrefix2">Another SI prefix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SIPrefix SIPrefix1,
                                          SIPrefix SIPrefix2)

            => SIPrefix1.CompareTo(SIPrefix2) > 0;

        #endregion

        #region Operator >= (SIPrefix1, SIPrefix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SIPrefix1">An SI prefix.</param>
        /// <param name="SIPrefix2">Another SI prefix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SIPrefix SIPrefix1,
                                           SIPrefix SIPrefix2)

            => SIPrefix1.CompareTo(SIPrefix2) >= 0;

        #endregion

        #endregion

        #region IComparable<SIPrefix> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two SI prefixes.
        /// </summary>
        /// <param name="Object">An SI prefix to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null               => 1,
                   SIPrefix siPrefix  => CompareTo(siPrefix),
                   _                  => throw new ArgumentException("The given object is not an SI prefix!", nameof(Object))
               };

        #endregion

        #region CompareTo(SIPrefix)

        /// <summary>
        /// Compares two SI prefixes.
        /// </summary>
        /// <param name="SIPrefix">An SI prefix to compare with.</param>
        public Int32 CompareTo(SIPrefix SIPrefix)

            => Exponent.CompareTo(SIPrefix.Exponent);

        #endregion

        #endregion

        #region IEquatable<SIPrefix> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SI prefixes for equality.
        /// </summary>
        /// <param name="Object">An SI prefix to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SIPrefix siPrefix &&
                   Equals(siPrefix);

        #endregion

        #region Equals(SIPrefix)

        /// <summary>
        /// Compares two SI prefixes for equality.
        /// </summary>
        /// <param name="SIPrefix">An SI prefix to compare with.</param>
        public Boolean Equals(SIPrefix SIPrefix)

            => Exponent == SIPrefix.Exponent;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Exponent.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Symbol;

        #endregion

    }

}
