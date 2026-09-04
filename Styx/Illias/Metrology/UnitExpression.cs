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

using System.Text;
using System.Numerics;
using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A unit of measure raised to a rational power, e.g. the "s^-2" of "m·s^-2"
    /// or the "Hz^-1/2" of a noise spectral density in "V·Hz^-1/2" (V/√Hz).
    ///
    /// Fractional powers are not exotic in metrology: amplitude spectral
    /// densities are stated per √Hz, fracture toughness in Pa·m^1/2 and the
    /// Warburg impedance in Ω·s^-1/2.
    ///
    /// The exponent is always kept in its lowest terms with a positive
    /// denominator, so that 2/4 and 1/2 compare equal.
    /// </summary>
    public readonly struct UnitFactor : IEquatable<UnitFactor>
    {

        #region Properties

        /// <summary>
        /// The unit of measure.
        /// </summary>
        public UnitOfMeasure  Unit           { get; }

        /// <summary>
        /// The numerator of the power the unit is raised to. Never zero.
        /// </summary>
        public Int32          Numerator      { get; }

        /// <summary>
        /// The denominator of the power the unit is raised to. Always positive.
        /// </summary>
        public Int32          Denominator    { get; }

        /// <summary>
        /// Whether the power the unit is raised to is a whole number.
        /// </summary>
        public Boolean        IsInteger
            => Denominator == 1;

        /// <summary>
        /// The power the unit is raised to, as a floating point number.
        /// </summary>
        public Double         Exponent
            => (Double) Numerator / Denominator;

        #endregion

        #region Constructor(s)

        #region UnitFactor(Unit, Exponent)

        /// <summary>
        /// Create a new unit factor with a whole-number power.
        /// </summary>
        /// <param name="Unit">A unit of measure.</param>
        /// <param name="Exponent">The power the unit is raised to. Must not be zero.</param>
        public UnitFactor(UnitOfMeasure  Unit,
                          Int32          Exponent)

            : this(Unit, Exponent, 1)

        { }

        #endregion

        #region UnitFactor(Unit, Numerator, Denominator)

        /// <summary>
        /// Create a new unit factor with a rational power.
        /// </summary>
        /// <param name="Unit">A unit of measure.</param>
        /// <param name="Numerator">The numerator of the power. Must not be zero.</param>
        /// <param name="Denominator">The denominator of the power. Must not be zero.</param>
        public UnitFactor(UnitOfMeasure  Unit,
                          Int32          Numerator,
                          Int32          Denominator)
        {

            if (Numerator == 0)
                throw new ArgumentException("The exponent of a unit factor must not be zero!",     nameof(Numerator));

            if (Denominator == 0)
                throw new ArgumentException("The denominator of an exponent must not be zero!",    nameof(Denominator));

            // Keep the sign in the numerator and reduce to lowest terms,
            // so that Hz^-2/4 and Hz^-1/2 are the same factor.
            if (Denominator < 0)
            {
                Numerator    = -Numerator;
                Denominator  = -Denominator;
            }

            var greatestCommonDivisor = (Int32) BigInteger.GreatestCommonDivisor(
                                                    Math.Abs(Numerator),
                                                    Denominator
                                                );

            this.Unit         = Unit;
            this.Numerator    = Numerator   / greatestCommonDivisor;
            this.Denominator  = Denominator / greatestCommonDivisor;

        }

        #endregion

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitFactor1">A unit factor.</param>
        /// <param name="UnitFactor2">Another unit factor.</param>
        public static Boolean operator == (UnitFactor UnitFactor1,
                                           UnitFactor UnitFactor2)

            => UnitFactor1.Equals(UnitFactor2);

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitFactor1">A unit factor.</param>
        /// <param name="UnitFactor2">Another unit factor.</param>
        public static Boolean operator != (UnitFactor UnitFactor1,
                                           UnitFactor UnitFactor2)

            => !UnitFactor1.Equals(UnitFactor2);

        #endregion

        #region IEquatable<UnitFactor> Members

        /// <summary>
        /// Compares two unit factors for equality.
        /// </summary>
        /// <param name="Object">A unit factor to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnitFactor unitFactor &&
                   Equals(unitFactor);

        /// <summary>
        /// Compares two unit factors for equality.
        /// </summary>
        /// <param name="UnitFactor">A unit factor to compare with.</param>
        public Boolean Equals(UnitFactor UnitFactor)

            => Unit.Equals(UnitFactor.Unit)      &&
               Numerator   == UnitFactor.Numerator &&
               Denominator == UnitFactor.Denominator;

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => HashCode.Combine(Unit, Numerator, Denominator);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object, e.g. "W", "s^-2" or "Hz^-1/2".
        /// </summary>
        public override String ToString()

            => IsInteger
                   ? Numerator == 1
                         ?  Unit.Symbol
                         : $"{Unit.Symbol}^{Numerator}"
                   :        $"{Unit.Symbol}^{Numerator}/{Denominator}";

        #endregion

    }


    /// <summary>
    /// A unit of measure, either a single named one such as the watt, or a
    /// product of powers of named units such as "m·s^-2", "W·m^-2·K^-1" or
    /// "V·Hz^-1/2".
    ///
    /// A flat list of named units can not express derived quantities, which is
    /// why the metrological CBOR tag 44252 accepts both forms; see
    /// https://github.com/OpenChargingTechnology/Whitepapers/blob/master/MetrologicalCBOR/README.md.
    /// </summary>
    public readonly struct UnitExpression : IEquatable<UnitExpression>
    {

        #region Data

        private readonly UnitFactor[]? factors;

        #endregion

        #region Properties

        /// <summary>
        /// The factors of this unit expression, in the order given.
        /// </summary>
        public IReadOnlyList<UnitFactor>  Factors
            => factors ?? [];

        /// <summary>
        /// Whether this expression is a single named unit raised to the first power.
        /// </summary>
        public Boolean                    IsSimple
            => factors is { Length: 1 } &&
               factors[0].Numerator   == 1 &&
               factors[0].Denominator == 1;

        /// <summary>
        /// The single named unit of a simple expression.
        /// Throws whenever this expression is a product of powers.
        /// </summary>
        public UnitOfMeasure              SingleUnit
            => IsSimple
                   ? factors![0].Unit
                   : throw new InvalidOperationException($"The unit expression '{this}' is not a single named unit!");

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unit expression based on the given factors.
        /// </summary>
        /// <param name="Factors">An enumeration of unit factors.</param>
        public UnitExpression(params IEnumerable<UnitFactor> Factors)
        {

            var unitFactors = Factors.ToArray();

            if (unitFactors.Length == 0)
                throw new ArgumentException("A unit expression must have at least one factor!", nameof(Factors));

            this.factors = unitFactors;

        }

        /// <summary>
        /// Create a new unit expression based on the given single named unit.
        /// </summary>
        /// <param name="Unit">A unit of measure.</param>
        public UnitExpression(UnitOfMeasure Unit)
        {
            this.factors = [new UnitFactor(Unit, 1)];
        }

        #endregion


        #region (static) TryParse(Text, out UnitExpression)

        /// <summary>
        /// Try to parse the given text as a unit expression, in the form
        /// written by ToString(): "W", "m·s^-2" or "V·Hz^-1/2".
        /// Both the middle dot and an ASCII asterisk separate the factors.
        /// </summary>
        /// <param name="Text">A text representation of a unit expression.</param>
        /// <param name="UnitExpression">The parsed unit expression.</param>
        public static Boolean TryParse(String? Text, out UnitExpression UnitExpression)
        {

            UnitExpression = default;

            if (Text is null)
                return false;

            var text = Text.Trim();

            if (text.Length == 0)
                return false;

            var unitFactors = new List<UnitFactor>();

            foreach (var token in text.Split(['·', '*'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {

                var symbol       = token;
                var numerator    = 1;
                var denominator  = 1;

                var caret = token.IndexOf('^');

                if (caret >= 0)
                {

                    symbol         = token[..caret].TrimEnd();
                    var exponent   = token[(caret + 1)..].Trim();
                    var slash      = exponent.IndexOf('/');

                    if (slash >= 0)
                    {

                        if (!Int32.TryParse(exponent[..slash],      NumberStyles.Integer, CultureInfo.InvariantCulture, out numerator) ||
                            !Int32.TryParse(exponent[(slash + 1)..], NumberStyles.Integer, CultureInfo.InvariantCulture, out denominator))
                        {
                            return false;
                        }

                    }

                    else if (!Int32.TryParse(exponent, NumberStyles.Integer, CultureInfo.InvariantCulture, out numerator))
                        return false;

                }

                if (numerator == 0 || denominator == 0)
                    return false;

                UnitOfMeasure? unitOfMeasure;

                if (caret < 0)
                {

                    // The whole token wins over an exponent split: "m²" is the
                    // registered square metre, never the metre to the second
                    // power. Only when the token is no symbol is a trailing
                    // superscript exponent peeled off: "s⁻²" is "s^-2".
                    if (!UnitOfMeasure.TryParseSymbol(symbol, out unitOfMeasure))
                    {

                        var split = SplitSuperscriptExponent(symbol);

                        if (split is null ||
                            !UnitOfMeasure.TryParseSymbol(split.Value.Symbol, out unitOfMeasure))
                        {
                            return false;
                        }

                        numerator = split.Value.Exponent;

                        if (numerator == 0)
                            return false;

                    }

                }

                else if (!UnitOfMeasure.TryParseSymbol(symbol, out unitOfMeasure))
                    return false;

                unitFactors.Add(new UnitFactor(unitOfMeasure, numerator, denominator));

            }

            if (unitFactors.Count == 0)
                return false;

            UnitExpression = new UnitExpression(unitFactors);
            return true;

        }

        #endregion


        #region (internal static) IsSuperscript / FromSuperscript / SplitSuperscriptExponent

        /// <summary>
        /// Whether the given character is a superscript digit or sign.
        /// </summary>
        internal static Boolean IsSuperscript(Char Character)

            => Character is '⁰' or '¹' or '²' or '³' or
                            '⁴' or '⁵' or '⁶' or '⁷' or
                            '⁸' or '⁹' or '⁺' or '⁻';


        /// <summary>
        /// The superscript characters as their ASCII counterparts, or null
        /// where a character is no superscript.
        /// </summary>
        internal static String? FromSuperscript(ReadOnlySpan<Char> Text)
        {

            Span<Char> ascii = stackalloc Char[Text.Length];

            for (var i = 0; i < Text.Length; i++)
            {

                ascii[i] = Text[i] switch {
                               '⁰' => '0',
                               '¹' => '1',
                               '²' => '2',
                               '³' => '3',
                               '⁴' => '4',
                               '⁵' => '5',
                               '⁶' => '6',
                               '⁷' => '7',
                               '⁸' => '8',
                               '⁹' => '9',
                               '⁺' => '+',
                               '⁻' => '-',
                               _        => '\0'
                           };

                if (ascii[i] == '\0')
                    return null;

            }

            return new String(ascii);

        }


        /// <summary>
        /// Split a trailing superscript exponent off a unit token: "s⁻²"
        /// becomes ("s", -2). Null where the token carries none, or where
        /// the superscripts are not a number.
        /// </summary>
        private static (String Symbol, Int32 Exponent)? SplitSuperscriptExponent(String Token)
        {

            var start = Token.Length;

            while (start > 0 && IsSuperscript(Token[start - 1]))
                start--;

            if (start == Token.Length || start == 0)
                return null;

            var digits = FromSuperscript(Token.AsSpan(start));

            if (digits is null ||
                !Int32.TryParse(digits, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var exponent))
            {
                return null;
            }

            return (Token[..start], exponent);

        }

        #endregion


        #region (implicit) UnitExpression(Unit)

        /// <summary>
        /// Convert the given single named unit into a unit expression.
        /// </summary>
        /// <param name="Unit">A unit of measure.</param>
        public static implicit operator UnitExpression(UnitOfMeasure Unit)

            => new (Unit);

        #endregion


        #region Operator overloading

        #region Operator == (UnitExpression1, UnitExpression2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitExpression1">A unit expression.</param>
        /// <param name="UnitExpression2">Another unit expression.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UnitExpression UnitExpression1,
                                           UnitExpression UnitExpression2)

            => UnitExpression1.Equals(UnitExpression2);

        #endregion

        #region Operator != (UnitExpression1, UnitExpression2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitExpression1">A unit expression.</param>
        /// <param name="UnitExpression2">Another unit expression.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UnitExpression UnitExpression1,
                                           UnitExpression UnitExpression2)

            => !UnitExpression1.Equals(UnitExpression2);

        #endregion

        #endregion

        #region IEquatable<UnitExpression> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two unit expressions for equality.
        /// </summary>
        /// <param name="Object">A unit expression to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object switch {
                   UnitExpression unitExpression  => Equals(unitExpression),
                   UnitOfMeasure  unitOfMeasure   => Equals(new UnitExpression(unitOfMeasure)),
                   _                              => false
               };

        #endregion

        #region Equals(UnitExpression)

        /// <summary>
        /// Compares two unit expressions for equality.
        /// The order of the factors is significant, because it is what gets written.
        /// </summary>
        /// <param name="UnitExpression">A unit expression to compare with.</param>
        public Boolean Equals(UnitExpression UnitExpression)
        {

            var these  = Factors;
            var those  = UnitExpression.Factors;

            if (these.Count != those.Count)
                return false;

            for (var i = 0; i < these.Count; i++)
            {
                if (!these[i].Equals(those[i]))
                    return false;
            }

            return true;

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {

            var hashCode = new HashCode();

            foreach (var factor in Factors)
                hashCode.Add(factor);

            return hashCode.ToHashCode();

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object, e.g. "W", "m·s^-2" or "V·Hz^-1/2".
        /// </summary>
        public override String ToString()
        {

            var these = Factors;

            if (these.Count == 1)
                return these[0].ToString();

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < these.Count; i++)
            {

                if (i > 0)
                    stringBuilder.Append('·');

                stringBuilder.Append(these[i].ToString());

            }

            return stringBuilder.ToString();

        }

        #endregion

    }

}
