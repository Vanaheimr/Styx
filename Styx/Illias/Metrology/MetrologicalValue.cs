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

using System.Numerics;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A metrological value: A decimal reading of a physical quantity
    /// with its unit of measure, an SI prefix and an optional symmetric
    /// standard measurement uncertainty u with coverage factor k=1, as
    /// defined by the "Guide to the Expression of Uncertainty in
    /// Measurement" (GUM, JCGM 100:2008). The uncertainty is expressed
    /// in the same unit and prefix as the value.
    /// The decimal scale and the SI prefix are preserved as displayed
    /// by the measuring instrument: 5.0 mA stays 5.0 mA and never
    /// silently becomes 0.005 A - therefore Equals() compares the
    /// representation, while EquivalentTo() compares the physical
    /// quantity exactly and without rounding.
    /// The CBOR representation uses tag 44252 (0xACDC):
    /// [value, unit, ?prefix, ?uncertainty].
    /// </summary>
    public readonly struct MetrologicalValue : IEquatable<MetrologicalValue>,
                                               ICBORSerializable<MetrologicalValue>
    {

        #region Properties

        /// <summary>
        /// The value of this metrological value,
        /// scaled by its SI prefix.
        /// </summary>
        public Decimal         Value          { get; }

        /// <summary>
        /// The unit of measure of this metrological value: either a single
        /// named unit, or a product of powers such as "m·s^-2" or "V·Hz^-1/2".
        /// </summary>
        public UnitExpression   Unit           { get; }

        /// <summary>
        /// The SI prefix of this metrological value.
        /// </summary>
        public SIPrefix         Prefix         { get; }

        /// <summary>
        /// The optional symmetric measurement uncertainty (GUM), expressed in
        /// the same unit and prefix as the value. A plain number is taken as
        /// the standard uncertainty u (k=1); a calibration certificate stating
        /// an expanded U keeps its coverage factor.
        /// </summary>
        public MeasurementUncertainty?  Uncertainty    { get; }

        /// <summary>
        /// Whether this is the default instance without a unit of measure.
        /// </summary>
        public Boolean          IsEmpty
            => Unit.Factors.Count == 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new metrological value.
        /// </summary>
        /// <param name="Value">The value, scaled by its SI prefix.</param>
        /// <param name="Unit">The unit of measure. A single named unit converts implicitly.</param>
        /// <param name="Prefix">The optional SI prefix (default: none).</param>
        /// <param name="Uncertainty">The optional measurement uncertainty (GUM). A plain number converts implicitly into a standard uncertainty u (k=1).</param>
        public MetrologicalValue(Decimal                  Value,
                                 UnitExpression           Unit,
                                 SIPrefix?                Prefix        = null,
                                 MeasurementUncertainty?  Uncertainty   = null)
        {

            if (Unit.Factors.Count == 0)
                throw new ArgumentException("A metrological value must have a unit of measure!",
                                            nameof(Unit));

            this.Value        = Value;
            this.Unit         = Unit;
            this.Prefix       = Prefix ?? SIPrefix.None;
            this.Uncertainty  = Uncertainty;

        }

        #endregion


        #region (static) From(StdDev, Unit, Prefix = null)

        /// <summary>
        /// Create a new metrological value from the given mean value and
        /// its standard deviation, taken as the standard measurement
        /// uncertainty u (k=1).
        /// </summary>
        /// <param name="StdDev">A mean value with its standard deviation.</param>
        /// <param name="Unit">The unit of measure.</param>
        /// <param name="Prefix">The optional SI prefix (default: none).</param>
        public static MetrologicalValue From(StdDev<Decimal>  StdDev,
                                             UnitOfMeasure    Unit,
                                             SIPrefix?        Prefix = null)

            => new (StdDev.Mean,
                    Unit,
                    Prefix,
                    StdDev.StandardDeviation);

        #endregion


        #region ConvertTo    (Prefix)

        /// <summary>
        /// Convert this metrological value (and its uncertainty) into the
        /// given SI prefix, e.g. 5.0 mA into 0.005 A.
        /// Throws an OverflowException whenever the conversion can not be
        /// represented as a System.Decimal without loss.
        /// </summary>
        /// <param name="Prefix">The target SI prefix.</param>
        public MetrologicalValue ConvertTo(SIPrefix Prefix)
        {

            var exponentDifference = this.Prefix.Exponent - Prefix.Exponent;

            return new MetrologicalValue(
                       ScaleByPowerOfTen(Value, exponentDifference),
                       Unit,
                       Prefix,
                       Uncertainty.HasValue
                           ? new MeasurementUncertainty(
                                 ScaleByPowerOfTen(Uncertainty.Value.Value, exponentDifference),
                                 Uncertainty.Value.CoverageFactor,
                                 Uncertainty.Value.CoverageProbability,
                                 Uncertainty.Value.Distribution,
                                 Uncertainty.Value.DegreesOfFreedom
                             )
                           : null
                   );

        }

        #endregion

        #region TryToBaseUnit(out BaseValue)

        /// <summary>
        /// Try to convert this metrological value (and its uncertainty)
        /// into its unprefixed base unit, e.g. 5.0 mA into 0.005 A.
        /// Returns false whenever the conversion can not be represented
        /// as a System.Decimal without loss.
        /// </summary>
        /// <param name="BaseValue">The converted metrological value.</param>
        public Boolean TryToBaseUnit(out MetrologicalValue BaseValue)
        {

            try
            {
                BaseValue = ConvertTo(SIPrefix.None);
                return true;
            }
            catch (Exception e) when (e is OverflowException || e is ArgumentOutOfRangeException)
            {
                BaseValue = default;
                return false;
            }

        }

        #endregion

        #region EquivalentTo (Other)

        /// <summary>
        /// Whether this and the given metrological value describe exactly
        /// the same physical quantity, e.g. 5.0 mA and 0.005 A.
        /// The comparison folds the SI prefixes via exact big-integer
        /// arithmetic and is therefore neither limited by the decimal
        /// range of quecto/quetta nor subject to any rounding.
        /// Uncertainties must be equivalent as well.
        /// </summary>
        /// <param name="Other">Another metrological value.</param>
        public Boolean EquivalentTo(MetrologicalValue Other)
        {

            if (IsEmpty || Other.IsEmpty)
                return IsEmpty && Other.IsEmpty;

            if (Unit != Other.Unit)
                return false;

            if (Uncertainty.HasValue != Other.Uncertainty.HasValue)
                return false;

            if (!SameQuantity(Value, Prefix.Exponent, Other.Value, Other.Prefix.Exponent))
                return false;

            if (Uncertainty.HasValue)
            {

                // Compare the standard uncertainties, so that U = 0.02 with
                // k = 2 is equivalent to u = 0.01 - they state the same spread.
                if (!SameQuantity(Uncertainty.Value.StandardUncertainty,
                                  Prefix.Exponent,
                                  Other.Uncertainty!.Value.StandardUncertainty,
                                  Other.Prefix.Exponent))
                {
                    return false;
                }

            }

            return true;

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return the JSON representation of this metrological value,
        /// e.g. { "value": 5.0, "uncertainty": 0.2, "unit": "A", "prefix": "m" }.
        /// The prefix is omitted when it is none; the uncertainty is
        /// omitted when it is not present. The decimal scale is preserved.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(

                         new JProperty("value",         Value),

                   Uncertainty.HasValue
                       ? new JProperty("uncertainty",   Uncertainty.Value.Value)
                       : null,

                   Uncertainty.HasValue && Uncertainty.Value.CoverageFactor != 1
                       ? new JProperty("coverageFactor", Uncertainty.Value.CoverageFactor)
                       : null,

                         new JProperty("unit",          Unit.ToString()),

                   !Prefix.IsNone
                       ? new JProperty("prefix",        Prefix.Symbol)
                       : null

               );

        #endregion

        #region (static) TryParse(JSON, out MetrologicalValue, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON object as a metrological value.
        /// </summary>
        /// <param name="JSON">A JSON object to be parsed.</param>
        /// <param name="MetrologicalValue">The parsed metrological value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       out MetrologicalValue             MetrologicalValue,
                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            MetrologicalValue = default;

            if (!TryParseJSONNumber(JSON["value"], "value", out var value, out ErrorResponse))
            {
                ErrorResponse ??= "Missing JSON property 'value'!";
                return false;
            }

            var unitText = JSON["unit"]?.Value<String>();

            if (unitText is null)
            {
                ErrorResponse = "Missing JSON property 'unit'!";
                return false;
            }

            if (!UnitExpression.TryParse(unitText, out var unit))
            {
                ErrorResponse = $"Unknown unit of measure '{unitText}'!";
                return false;
            }

            var prefix       = SIPrefix.None;
            var prefixToken  = JSON["prefix"];

            if (prefixToken is not null &&
                !SIPrefix.TryParse(prefixToken.Value<String>(), out prefix))
            {
                ErrorResponse = $"Unknown SI prefix '{prefixToken}'!";
                return false;
            }

            MeasurementUncertainty? uncertainty = null;

            if (JSON["uncertainty"] is not null)
            {

                if (!TryParseJSONNumber(JSON["uncertainty"], "uncertainty", out var uncertaintyValue, out ErrorResponse))
                {
                    // The token is not null here, so the helper always reports
                    // a reason - but the compiler can not see that.
                    ErrorResponse ??= "Invalid JSON property 'uncertainty'!";
                    return false;
                }

                if (uncertaintyValue < 0)
                {
                    ErrorResponse = "The measurement uncertainty must not be negative!";
                    return false;
                }

                var coverageFactor = 1m;

                if (JSON["coverageFactor"] is not null)
                {

                    if (!TryParseJSONNumber(JSON["coverageFactor"], "coverage factor", out coverageFactor, out ErrorResponse))
                    {
                        ErrorResponse ??= "Invalid JSON property 'coverageFactor'!";
                        return false;
                    }

                    if (coverageFactor <= 0)
                    {
                        ErrorResponse = "The coverage factor must be positive!";
                        return false;
                    }

                }

                uncertainty = new MeasurementUncertainty(
                                  uncertaintyValue,
                                  coverageFactor
                              );

            }

            MetrologicalValue  = new MetrologicalValue(value, unit, prefix, uncertainty);
            ErrorResponse      = null;

            return true;

        }

        #endregion


        #region ToCBOR (CustomSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this metrological value:
        /// Tag 44252 (0xACDC) wrapping [value, unit, ?prefix, ?uncertainty].
        /// The unit is written as its numeric identification; integral
        /// values use plain CBOR integers, all other values become exact,
        /// scale-preserving decimal fractions (tag 4) - never binary floats.
        /// The prefix is written whenever it is not none or an uncertainty
        /// follows; the uncertainty is written whenever it is present.
        /// </summary>
        /// <param name="CustomSerializer">An optional delegate to customize the CBOR representation.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<MetrologicalValue>? CustomSerializer = null)

            => ToCBOR(SymbolicUnit: false,
                      CustomSerializer);


        /// <summary>
        /// Return the CBOR representation of this metrological value:
        /// Tag 44252 (0xACDC) wrapping [value, unit, ?prefix, ?uncertainty].
        /// </summary>
        /// <param name="SymbolicUnit">Whether to write the unit as its symbol text instead of its numeric identification.</param>
        /// <param name="CustomSerializer">An optional delegate to customize the CBOR representation.</param>
        public CBORValue ToCBOR(Boolean                                           SymbolicUnit,
                                CustomCBORSerializerDelegate<MetrologicalValue>?  CustomSerializer = null)
        {

            var items = new List<CBORValue>(4) {

                            ToCBORNumber(Value),
                            ToCBORUnit  (Unit, SymbolicUnit)

                        };

            if (!Prefix.IsNone || Uncertainty.HasValue)
                items.Add(CBORValue.FromInt64(Prefix.Exponent));

            if (Uncertainty.HasValue)
                items.Add(ToCBORUncertainty(Uncertainty.Value));

            var cbor = CBORValue.FromArray(items).WithTag(CBORTag.MetrologicalValue);

            return CustomSerializer?.Invoke(this, cbor) ?? cbor;

        }

        #endregion

        #region WriteTo(Writer, SymbolicUnit = false)

        /// <summary>
        /// Write this metrological value to the given CBOR writer:
        /// Tag 44252 (0xACDC) wrapping [value, unit, ?prefix, ?uncertainty].
        /// </summary>
        /// <param name="Writer">A CBOR writer.</param>
        /// <param name="SymbolicUnit">Whether to write the unit as its symbol text instead of its numeric identification.</param>
        public void WriteTo(CBORWriter  Writer,
                            Boolean     SymbolicUnit = false)
        {

            Writer.WriteTag(CBORTag.MetrologicalValue);

            var length = 2;

            if (!Prefix.IsNone || Uncertainty.HasValue)
                length++;

            if (Uncertainty.HasValue)
                length++;

            Writer.WriteStartArray(length);

            WriteCBORNumber(Writer, Value);

            ToCBORUnit(Unit, SymbolicUnit).WriteTo(Writer);

            if (length >= 3)
                Writer.WriteInt64(Prefix.Exponent);

            if (Uncertainty.HasValue)
                ToCBORUncertainty(Uncertainty.Value).WriteTo(Writer);

            Writer.WriteEndArray();

        }

        #endregion

        #region (static) TryParse(CBOR, out MetrologicalValue, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR value as a metrological value:
        /// Tag 44252 (0xACDC) wrapping [value, unit, ?prefix, ?uncertainty].
        /// Unknown units, binary floating-point values, non-canonical
        /// SI prefix exponents, negative uncertainties and array lengths
        /// other than 2..4 are errors.
        /// </summary>
        /// <param name="CBOR">The CBOR value to be parsed.</param>
        /// <param name="MetrologicalValue">The parsed metrological value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                         CBOR,
                                       out MetrologicalValue             MetrologicalValue,
                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            MetrologicalValue = default;

            if (!CBOR.HasTag(CBORTag.MetrologicalValue))
            {
                ErrorResponse = $"Expected a metrological value (tag {CBORTag.MetrologicalValue})!";
                return false;
            }

            var array = CBOR.UntaggedValue;

            if (array.Kind != CBORValueKind.Array ||
                array.Count < 2 ||
                array.Count > 4)
            {
                ErrorResponse = "A metrological value must be an array of 2..4 data items!";
                return false;
            }

            // 1. The value...
            if (!TryParseCBORNumber(array[0], "value", out var value, out ErrorResponse))
                return false;

            // 2. The unit of measure...
            if (!TryParseCBORUnit(array[1], out var unit, out ErrorResponse))
                return false;

            // 3. The optional SI prefix...
            var prefix = SIPrefix.None;

            if (array.Count >= 3)
            {

                var prefixElement = array[2];

                if ((prefixElement.Kind != CBORValueKind.UnsignedInteger &&
                     prefixElement.Kind != CBORValueKind.NegativeInteger)  ||
                    !prefixElement.TryGetInt64(out var prefixExponent)     ||
                    prefixExponent < SByte.MinValue                        ||
                    prefixExponent > SByte.MaxValue                        ||
                    !SIPrefix.TryFrom((Int32) prefixExponent, out prefix))
                {
                    ErrorResponse = $"The prefix of a metrological value must be one of the canonical SI prefix exponents, but is '{prefixElement.ToDiagnosticString()}'!";
                    return false;
                }

            }

            // 4. The optional measurement uncertainty...
            MeasurementUncertainty? uncertainty = null;

            if (array.Count == 4)
            {

                if (!TryParseCBORUncertainty(array[3], out var parsedUncertainty, out ErrorResponse))
                    return false;

                uncertainty = parsedUncertainty;

            }

            MetrologicalValue  = new MetrologicalValue(value, unit, prefix, uncertainty);
            ErrorResponse      = null;

            return true;

        }

        #endregion


        #region (private static) TryParseCBORUnit  (Node, out Unit, out ErrorResponse)

        /// <summary>
        /// Read a unit: a numeric identification, a symbol, or an array of
        /// [id, exponent] factors describing a product of powers.
        /// </summary>
        private static Boolean TryParseCBORUnit(CBORValue                         Node,
                                                out UnitExpression                Unit,
                                                [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Unit           = default;
            ErrorResponse  = null;

            // A product of powers...
            if (Node.Kind == CBORValueKind.Array)
            {

                if (Node.Count == 0)
                {
                    ErrorResponse = "A compound unit of measure must have at least one factor!";
                    return false;
                }

                var factors = new List<UnitFactor>(Node.Count);

                foreach (var factorNode in Node.AsArray())
                {

                    if (factorNode.Kind != CBORValueKind.Array ||
                        factorNode.Count != 2)
                    {
                        ErrorResponse = "Every factor of a compound unit of measure must be a [unit, exponent] pair!";
                        return false;
                    }

                    if (!TryParseCBORNamedUnit(factorNode[0], out var factorUnit, out ErrorResponse))
                        return false;

                    var exponentNode  = factorNode[1];
                    var numerator     = 0L;
                    var denominator   = 1L;

                    if (exponentNode.Kind == CBORValueKind.Array)
                    {

                        if (exponentNode.Count != 2                                ||
                            !exponentNode[0].TryGetInt64(out numerator)            ||
                            !exponentNode[1].TryGetInt64(out denominator))
                        {
                            ErrorResponse = "A rational unit exponent must be a [numerator, denominator] pair of integers!";
                            return false;
                        }

                    }

                    else if (!exponentNode.TryGetInt64(out numerator))
                    {
                        ErrorResponse = $"The exponent of a unit factor must be an integer or a [numerator, denominator] pair, but is '{exponentNode.ToDiagnosticString()}'!";
                        return false;
                    }

                    if (numerator   == 0 ||
                        denominator <= 0 ||
                        numerator   < Int32.MinValue || numerator   > Int32.MaxValue ||
                        denominator > Int32.MaxValue)
                    {
                        ErrorResponse = $"Invalid unit exponent {numerator}/{denominator}: the numerator must not be zero and the denominator must be positive!";
                        return false;
                    }

                    factors.Add(new UnitFactor(factorUnit, (Int32) numerator, (Int32) denominator));

                }

                Unit = new UnitExpression(factors);
                return true;

            }

            // A single named unit...
            if (!TryParseCBORNamedUnit(Node, out var namedUnit, out ErrorResponse))
                return false;

            Unit = namedUnit;
            return true;

        }

        #endregion

        #region (private static) TryParseCBORNamedUnit(Node, out Unit, out ErrorResponse)

        private static Boolean TryParseCBORNamedUnit(CBORValue                         Node,
                                                     [NotNullWhen(true)] out UnitOfMeasure?  Unit,
                                                     [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Unit           = null;
            ErrorResponse  = null;

            if (Node.Kind == CBORValueKind.UnsignedInteger)
            {

                var numericUnit = Node.AsUInt64();

                if (numericUnit > UInt16.MaxValue ||
                    !UnitOfMeasure.TryParse((UInt16) numericUnit, out Unit))
                {
                    ErrorResponse = $"Unknown unit of measure '{numericUnit}'!";
                    return false;
                }

                return true;

            }

            if (Node.Kind == CBORValueKind.TextString)
            {

                if (!UnitOfMeasure.TryParse(Node.AsText(), out Unit))
                {
                    ErrorResponse = $"Unknown unit of measure '{Node.AsText()}'!";
                    return false;
                }

                return true;

            }

            ErrorResponse = "A unit of measure must be an unsigned integer, a text string, or an array of [unit, exponent] factors!";
            return false;

        }

        #endregion

        #region (private static) TryParseCBORUncertainty(Node, out Uncertainty, out ErrorResponse)

        /// <summary>
        /// Read an uncertainty: a bare number is the standard uncertainty u
        /// (k=1); an integer-keyed map may additionally state the coverage
        /// factor, the coverage probability, the distribution and the
        /// effective degrees of freedom.
        /// </summary>
        private static Boolean TryParseCBORUncertainty(CBORValue                         Node,
                                                       out MeasurementUncertainty        Uncertainty,
                                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Uncertainty    = default;
            ErrorResponse  = null;

            if (Node.Kind != CBORValueKind.Map)
            {

                if (!TryParseCBORNumber(Node, "uncertainty", out var standardUncertainty, out ErrorResponse))
                    return false;

                if (standardUncertainty < 0)
                {
                    ErrorResponse = "The measurement uncertainty must not be negative!";
                    return false;
                }

                Uncertainty = new MeasurementUncertainty(standardUncertainty);
                return true;

            }

            if (!Node.TryGetValue(CBORValue.FromUInt64(1), out var magnitudeNode))
            {
                ErrorResponse = "A measurement uncertainty map must state its magnitude in key 1!";
                return false;
            }

            if (!TryParseCBORNumber(magnitudeNode, "uncertainty", out var magnitude, out ErrorResponse))
                return false;

            if (magnitude < 0)
            {
                ErrorResponse = "The measurement uncertainty must not be negative!";
                return false;
            }

            var coverageFactor = 1m;

            if (Node.TryGetValue(CBORValue.FromUInt64(2), out var coverageFactorNode))
            {

                if (!TryParseCBORNumber(coverageFactorNode, "coverage factor", out coverageFactor, out ErrorResponse))
                    return false;

                if (coverageFactor <= 0)
                {
                    ErrorResponse = "The coverage factor must be positive!";
                    return false;
                }

            }

            Double? coverageProbability = null;

            if (Node.TryGetValue(CBORValue.FromUInt64(3), out var coverageProbabilityNode))
            {

                if (!TryParseCBORNumber(coverageProbabilityNode, "coverage probability", out var probability, out ErrorResponse))
                    return false;

                if (probability <= 0 || probability > 1)
                {
                    ErrorResponse = "The coverage probability must be within ]0, 1]!";
                    return false;
                }

                coverageProbability = (Double) probability;

            }

            var distribution = UncertaintyDistribution.Unspecified;

            if (Node.TryGetValue(CBORValue.FromUInt64(4), out var distributionNode))
            {

                if (distributionNode.Kind != CBORValueKind.UnsignedInteger ||
                    distributionNode.AsUInt64() > (UInt64) UncertaintyDistribution.StudentT)
                {
                    ErrorResponse = $"Unknown uncertainty distribution '{distributionNode.ToDiagnosticString()}'!";
                    return false;
                }

                distribution = (UncertaintyDistribution) distributionNode.AsUInt64();

            }

            Double? degreesOfFreedom = null;

            if (Node.TryGetValue(CBORValue.FromUInt64(5), out var degreesOfFreedomNode))
            {

                if (!TryParseCBORNumber(degreesOfFreedomNode, "degrees of freedom", out var freedom, out ErrorResponse))
                    return false;

                if (freedom <= 0)
                {
                    ErrorResponse = "The effective degrees of freedom must be positive!";
                    return false;
                }

                degreesOfFreedom = (Double) freedom;

            }

            Uncertainty = new MeasurementUncertainty(
                              magnitude,
                              coverageFactor,
                              coverageProbability,
                              distribution,
                              degreesOfFreedom
                          );

            return true;

        }

        #endregion

        #region (private static) ToCBORUnit        (Unit, SymbolicUnit)

        /// <summary>
        /// A single named unit becomes its numeric identification (or its
        /// symbol); a product of powers becomes an array of [id, exponent]
        /// pairs, where a rational exponent is itself a [numerator,
        /// denominator] pair - so V/√Hz is expressible while every whole
        /// number exponent stays a single integer.
        /// </summary>
        private static CBORValue ToCBORUnit(UnitExpression  Unit,
                                            Boolean         SymbolicUnit)
        {

            if (Unit.IsSimple)
                return SymbolicUnit
                           ? CBORValue.FromText  (Unit.SingleUnit.Symbol)
                           : CBORValue.FromUInt64(Unit.SingleUnit.Numeric);

            var factors = new List<CBORValue>(Unit.Factors.Count);

            foreach (var factor in Unit.Factors)
                factors.Add(
                    CBORValue.FromArray(
                        SymbolicUnit
                            ? CBORValue.FromText  (factor.Unit.Symbol)
                            : CBORValue.FromUInt64(factor.Unit.Numeric),
                        factor.IsInteger
                            ? CBORValue.FromInt64(factor.Numerator)
                            : CBORValue.FromArray(
                                  CBORValue.FromInt64 (factor.Numerator),
                                  CBORValue.FromUInt64((UInt64) factor.Denominator)
                              )
                    )
                );

            return CBORValue.FromArray(factors);

        }

        #endregion

        #region (private static) ToCBORUncertainty (Uncertainty)

        /// <summary>
        /// A plain standard uncertainty stays a bare number; anything that
        /// says more - a coverage factor, a coverage probability, a
        /// distribution or degrees of freedom - becomes an integer-keyed map.
        /// </summary>
        private static CBORValue ToCBORUncertainty(MeasurementUncertainty Uncertainty)
        {

            if (Uncertainty.IsPlainStandardUncertainty)
                return ToCBORNumber(Uncertainty.Value);

            var entries = new List<KeyValuePair<CBORValue, CBORValue>> {
                              new (CBORValue.FromUInt64(1), ToCBORNumber(Uncertainty.Value))
                          };

            if (Uncertainty.CoverageFactor != 1)
                entries.Add(new (CBORValue.FromUInt64(2), ToCBORNumber(Uncertainty.CoverageFactor)));

            if (Uncertainty.CoverageProbability.HasValue)
                entries.Add(new (CBORValue.FromUInt64(3), ToCBORNumber((Decimal) Uncertainty.CoverageProbability.Value)));

            if (Uncertainty.Distribution != UncertaintyDistribution.Unspecified)
                entries.Add(new (CBORValue.FromUInt64(4), CBORValue.FromUInt64((UInt64) Uncertainty.Distribution)));

            if (Uncertainty.DegreesOfFreedom.HasValue)
                entries.Add(new (CBORValue.FromUInt64(5), ToCBORNumber((Decimal) Uncertainty.DegreesOfFreedom.Value)));

            return CBORValue.FromMap(entries);

        }

        #endregion

        #region (private static) ToCBORNumber      (Value)

        private static CBORValue ToCBORNumber(Decimal Value)

            => Value.Scale == 0
                   ? CBORValue.FromBigInteger((BigInteger) Value)
                   : CBORValue.FromDecimal(Value);

        #endregion

        #region (private static) WriteCBORNumber   (Writer, Value)

        private static void WriteCBORNumber(CBORWriter  Writer,
                                            Decimal     Value)
        {

            if (Value.Scale == 0)
                Writer.WriteBigInteger((BigInteger) Value);
            else
                Writer.WriteDecimal(Value);

        }

        #endregion

        #region (private static) TryParseCBORNumber(Node, What, out Number, out ErrorResponse)

        private static Boolean TryParseCBORNumber(CBORValue                         Node,
                                                  String                            What,
                                                  out Decimal                       Number,
                                                  [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Number = 0;

            if (Node.Kind == CBORValueKind.HalfFloat   ||
                Node.Kind == CBORValueKind.SingleFloat ||
                Node.Kind == CBORValueKind.DoubleFloat)
            {
                ErrorResponse = $"The {What} of a metrological value must never be a binary floating-point number!";
                return false;
            }

            try
            {

                Number         = Node.AsDecimal();
                ErrorResponse  = null;

                return true;

            }
            catch (Exception e) when (e is CBORException || e is OverflowException)
            {
                ErrorResponse = $"Invalid {What} of a metrological value: {e.Message}";
                return false;
            }

        }

        #endregion

        #region (private static) TryParseJSONNumber(Token, What, out Number, out ErrorResponse)

        private static Boolean TryParseJSONNumber(JToken?      Token,
                                                  String       What,
                                                  out Decimal  Number,
                                                  out String?  ErrorResponse)
        {

            Number         = 0;
            ErrorResponse  = null;

            if (Token is null)
                return false;

            if (Token.Type == JTokenType.String)
            {

                if (Decimal.TryParse(Token.Value<String>(),
                                     NumberStyles.Number,
                                     CultureInfo.InvariantCulture,
                                     out Number))
                {
                    return true;
                }

                ErrorResponse = $"Invalid {What} '{Token}'!";
                return false;

            }

            try
            {
                Number = Token.Value<Decimal>();
                return true;
            }
            catch (Exception)
            {
                ErrorResponse = $"Invalid {What} '{Token}'!";
                return false;
            }

        }

        #endregion

        #region (private static) SameQuantity      (Value1, PrefixExponent1, Value2, PrefixExponent2)

        private static Boolean SameQuantity(Decimal  Value1,
                                            Int32    PrefixExponent1,
                                            Decimal  Value2,
                                            Int32    PrefixExponent2)
        {

            var (mantissa1, exponent1) = Decompose(Value1, PrefixExponent1);
            var (mantissa2, exponent2) = Decompose(Value2, PrefixExponent2);

            if (exponent1 > exponent2)
                mantissa1 *= BigInteger.Pow(10, exponent1 - exponent2);

            else if (exponent2 > exponent1)
                mantissa2 *= BigInteger.Pow(10, exponent2 - exponent1);

            return mantissa1 == mantissa2;

        }

        #endregion

        #region (private static) Decompose         (Value, PrefixExponent)

        private static (BigInteger Mantissa, Int32 Exponent) Decompose(Decimal  Value,
                                                                       Int32    PrefixExponent)
        {

            Span<Int32> bits = stackalloc Int32[4];
            Decimal.GetBits(Value, bits);

            var scale      = (bits[3] >> 16) & 0xFF;

            var magnitude  = ((UInt128) (UInt32) bits[2] << 64) |
                             ((UInt128) (UInt32) bits[1] << 32) |
                                        (UInt32) bits[0];

            var mantissa   = (BigInteger) magnitude;

            if ((bits[3] & unchecked((Int32) 0x80000000)) != 0)
                mantissa = -mantissa;

            return (mantissa, PrefixExponent - scale);

        }

        #endregion

        #region (private static) ScaleByPowerOfTen (Value, Exponent)

        private static Decimal ScaleByPowerOfTen(Decimal  Value,
                                                 Int32    Exponent)
        {

            var result     = Value;
            var remaining  = Exponent;

            while (remaining > 0)
            {

                var step   = Math.Min(remaining, 28);

                result     = result * MathHelpers.Pow10(step);
                remaining -= step;

            }

            while (remaining < 0)
            {

                var step     = Math.Max(remaining, -28);
                var divisor  = MathHelpers.Pow10(-step);
                var divided  = result / divisor;

                // System.Decimal division rounds silently, which would
                // violate the exactness guarantees of metrological values!
                if (divided * divisor != result)
                    throw new OverflowException($"The value '{Value}' can not be scaled by 10^{Exponent} without loss!");

                result      = divided;
                remaining  -= step;

            }

            return result;

        }

        #endregion


        #region Operator overloading

        #region Operator == (MetrologicalValue1, MetrologicalValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MetrologicalValue1">A metrological value.</param>
        /// <param name="MetrologicalValue2">Another metrological value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MetrologicalValue MetrologicalValue1,
                                           MetrologicalValue MetrologicalValue2)

            => MetrologicalValue1.Equals(MetrologicalValue2);

        #endregion

        #region Operator != (MetrologicalValue1, MetrologicalValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MetrologicalValue1">A metrological value.</param>
        /// <param name="MetrologicalValue2">Another metrological value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MetrologicalValue MetrologicalValue1,
                                           MetrologicalValue MetrologicalValue2)

            => !MetrologicalValue1.Equals(MetrologicalValue2);

        #endregion

        #endregion

        #region IEquatable<MetrologicalValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two metrological values for representational equality:
        /// The value (including its decimal scale), the unit, the SI prefix
        /// and the uncertainty must all match. 5.0 mA is not equal to
        /// 0.005 A - use EquivalentTo() for physical equality.
        /// </summary>
        /// <param name="Object">A metrological value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MetrologicalValue metrologicalValue &&
                   Equals(metrologicalValue);

        #endregion

        #region Equals(MetrologicalValue)

        /// <summary>
        /// Compares two metrological values for representational equality:
        /// The value (including its decimal scale), the unit, the SI prefix
        /// and the uncertainty must all match. 5.0 mA is not equal to
        /// 0.005 A - use EquivalentTo() for physical equality.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value to compare with.</param>
        public Boolean Equals(MetrologicalValue MetrologicalValue)

            => Value                    == MetrologicalValue.Value                    &&
               Value.Scale              == MetrologicalValue.Value.Scale              &&
               Unit                     == MetrologicalValue.Unit                     &&
               Prefix                   == MetrologicalValue.Prefix                   &&
               Uncertainty              == MetrologicalValue.Uncertainty              &&
               Uncertainty?.Value.Scale == MetrologicalValue.Uncertainty?.Value.Scale;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => HashCode.Combine(
                   Value,
                   Value.Scale,
                   Unit,
                   Prefix,
                   Uncertainty,
                   Uncertainty?.Value.Scale
               );

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object, e.g. "5.0 mA",
        /// or "(5.00 ±0.02) mA" with a measurement uncertainty.
        /// </summary>
        public override String ToString()
        {

            var unitText = !IsEmpty
                               ? $" {Prefix.Symbol}{Unit}"
                               : "";

            return Uncertainty.HasValue

                       ? String.Concat("(",
                                       Value.            ToString(CultureInfo.InvariantCulture),
                                       " ±",
                                       Uncertainty.Value.ToString(),
                                       ")",
                                       unitText)

                       : String.Concat(Value.ToString(CultureInfo.InvariantCulture),
                                       unitText);

        }

        #endregion

    }

}
