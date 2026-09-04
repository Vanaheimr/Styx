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



        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a metrological value, in the one-string text
        /// format written by ToString(), e.g. "5.0 mA" or "(230.00 ±0.12) V, k=2".
        /// </summary>
        /// <param name="Text">A text representation of a metrological value.</param>
        public static MetrologicalValue Parse(String Text)
        {

            if (TryParse(Text, out var metrologicalValue, out var errorResponse))
                return metrologicalValue;

            throw new FormatException($"Invalid text representation of a metrological value '{Text}': {errorResponse}");

        }

        #endregion

        #region (static) TryParse(Text, out MetrologicalValue)

        /// <summary>
        /// Try to parse the given text as a metrological value, in the
        /// one-string text format written by ToString().
        /// </summary>
        /// <param name="Text">A text representation of a metrological value.</param>
        /// <param name="MetrologicalValue">The parsed metrological value.</param>
        public static Boolean TryParse(String?                Text,
                                       out MetrologicalValue  MetrologicalValue)

            => TryParse(Text, out MetrologicalValue, out _);

        #endregion

        #region (static) TryParse(Text, out MetrologicalValue, out ErrorResponse)

        /// <summary>
        /// Try to parse the given text as a metrological value, in the one-string
        /// text format written by ToString(): the value, optionally together with
        /// its uncertainty in parentheses, then the unit of measure carrying the
        /// SI prefix, then whatever else was stated about the uncertainty, e.g.
        /// "5.0 mA", "9.81 m·s^-2" or "(230.00 ±0.12) V, k=2".
        ///
        /// The decimal scale of every number is data and is preserved as written.
        /// Accepted beyond the canonical spelling are the ASCII alternatives
        /// "+-" and "+/-" for "±", "*" for "·", "x" for "×" and "nu" for "ν",
        /// "t" for "student-t", superscript digits for unit exponents and for
        /// the scale, scientific notation for the numbers, and both code points
        /// of the micro sign and of the ohm sign. What is written back is
        /// always the canonical form.
        /// </summary>
        /// <param name="Text">A text representation of a metrological value.</param>
        /// <param name="MetrologicalValue">The parsed metrological value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(String?                           Text,
                                       out MetrologicalValue             MetrologicalValue,
                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            MetrologicalValue = default;

            if (Text is null)
            {
                ErrorResponse = "The given text representation of a metrological value must not be null!";
                return false;
            }

            // "+/-" is an accepted spelling of "±"; normalised up front so
            // that the splitting below only has the two forms to deal with.
            var text = Text.Trim().Replace("+/-", "±");

            if (text.Length == 0)
            {
                ErrorResponse = "The given text representation of a metrological value must not be empty!";
                return false;
            }

            #region Split off what is stated about the uncertainty

            var statements  = "";
            var comma       = text.IndexOf(',');

            if (comma >= 0)
            {
                statements  = text[(comma + 1)..];
                text        = text[..comma].TrimEnd();
            }

            #endregion

            #region The value and its optional uncertainty

            Decimal   value;
            Decimal?  uncertaintyValue  = null;
            var       index             = 0;

            if (text[0] == '(')
            {

                var close = text.IndexOf(')');

                if (close < 0)
                {
                    ErrorResponse = "The parenthesis around the value and its uncertainty is never closed!";
                    return false;
                }

                var inner      = text[1..close];
                var plusMinus  = inner.IndexOf('±');
                var width      = 1;

                if (plusMinus < 0)
                {
                    plusMinus  = inner.IndexOf("+-", StringComparison.Ordinal);
                    width      = 2;
                }

                if (plusMinus < 0)
                {
                    ErrorResponse = "The parenthesis must hold a value and its uncertainty, separated by '±'!";
                    return false;
                }

                if (!TryParseTextNumber(inner[..plusMinus],           "value",                   out value,            out ErrorResponse) ||
                    !TryParseTextNumber(inner[(plusMinus + width)..], "measurement uncertainty", out var uncertainty,  out ErrorResponse))
                {
                    return false;
                }

                if (uncertainty < 0)
                {
                    ErrorResponse = "The measurement uncertainty must not be negative!";
                    return false;
                }

                uncertaintyValue  = uncertainty;
                index             = close + 1;

            }

            else
            {

                while (index < text.Length            &&
                      !Char.IsWhiteSpace(text[index]) &&
                       text[index] != '×'             &&
                       text[index] != 'x'             &&
                       text[index] != '*')
                {
                    index++;
                }

                if (!TryParseTextNumber(text[..index], "value", out value, out ErrorResponse))
                    return false;

            }

            #endregion

            #region The optional power-of-ten scale

            var prefix         = SIPrefix.None;
            var scaleWasGiven  = false;

            if (index < text.Length &&
               (text[index] == '×' || text[index] == '*' || text[index] == 'x'))
            {

                if (index + 2 >= text.Length ||
                    text[index + 1] != '1'  ||
                    text[index + 2] != '0')
                {
                    ErrorResponse = "A power-of-ten scale must be written as '×10^3'!";
                    return false;
                }

                var exponentStart = index + 3;
                String exponentText;
                Int32  end;

                if (exponentStart < text.Length && text[exponentStart] == '^')
                {

                    end = exponentStart + 1;

                    while (end < text.Length && !Char.IsWhiteSpace(text[end]))
                        end++;

                    exponentText = text[(exponentStart + 1)..end];

                }

                else
                {

                    // Superscript digits: "×10³" is "×10^3".
                    end = exponentStart;

                    while (end < text.Length && UnitExpression.IsSuperscript(text[end]))
                        end++;

                    if (end == exponentStart)
                    {
                        ErrorResponse = "A power-of-ten scale must be written as '×10^3'!";
                        return false;
                    }

                    exponentText = UnitExpression.FromSuperscript(text[exponentStart..end]) ?? "";

                }

                if (!Int32.TryParse(exponentText, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var exponent) ||
                    !SIPrefix.TryFrom(exponent, out prefix))
                {
                    ErrorResponse = $"'10^{exponentText}' is not one of the 25 canonical SI prefixes!";
                    return false;
                }

                scaleWasGiven  = true;
                index          = end;

            }

            #endregion

            #region The unit of measure

            var unitText = text[index..].Trim();

            if (unitText.Length == 0)
            {
                ErrorResponse = "A metrological value must state its unit of measure!";
                return false;
            }

            UnitExpression unit;

            if (scaleWasGiven)
            {
                if (!UnitExpression.TryParse(unitText, out unit))
                {
                    ErrorResponse = $"Unknown unit of measure '{unitText}'!";
                    return false;
                }
            }

            else if (!TryParseTextUnit(unitText, out unit, out prefix, out ErrorResponse))
                return false;

            #endregion

            #region What else was stated about the uncertainty

            MeasurementUncertainty? measurementUncertainty = null;

            if (uncertaintyValue.HasValue)
            {

                if (!TryParseTextStatements(statements, uncertaintyValue.Value, out var uncertainty, out ErrorResponse))
                    return false;

                measurementUncertainty = uncertainty;

            }

            else if (statements.Trim().Length > 0)
            {
                ErrorResponse = $"'{statements.Trim()}' states something about a measurement uncertainty, but none is given!";
                return false;
            }

            #endregion

            MetrologicalValue  = new MetrologicalValue(value, unit, prefix, measurementUncertainty);
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

        #region ToByteArray(Options = null)

        /// <summary>
        /// Return the CBOR encoding of this metrological value:
        /// Tag 44252 (0xACDC) wrapping [value, unit, ?prefix, ?uncertainty].
        ///
        /// Deterministic by default, unlike the generic CBOR writer: Section 6
        /// of the tag specification makes the encoding a function of the value
        /// alone, so there is exactly one right answer here and the writer
        /// options do not get a vote. Pass CBORWriterOptions.Default only where
        /// producing a non-deterministic encoding is the point of the exercise,
        /// e.g. when generating test data for a decoder.
        /// </summary>
        /// <param name="Options">Optional CBOR writer options; deterministic encoding by default.</param>
        public Byte[] ToByteArray(CBORWriterOptions? Options = null)

            => ToCBOR().ToByteArray(Options ?? CBORWriterOptions.Canonical);

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

            // A prefix of 0 is written only because an uncertainty follows;
            // without one it would give the same reading two encodings
            // (specification section 3.3).
            if (array.Count == 3 && prefix.IsNone)
            {
                ErrorResponse = "A prefix of 0 must be omitted when no uncertainty follows!";
                return false;
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


        #region (static) Parse   (CBORBytes,                          Options = null)

        /// <summary>
        /// Parse the given CBOR data as a metrological value:
        /// Tag 44252 (0xACDC) wrapping [value, unit, ?prefix, ?uncertainty].
        /// </summary>
        /// <param name="CBORBytes">The encoded CBOR data.</param>
        /// <param name="Options">Optional CBOR reader options; the strict profile of Section 6 by default.</param>
        public static MetrologicalValue Parse(ReadOnlySpan<Byte>  CBORBytes,
                                              CBORReaderOptions?  Options   = null)
        {

            if (TryParse(CBORBytes, out var metrologicalValue, out var errorResponse, Options))
                return metrologicalValue;

            throw new CBORException($"Invalid CBOR representation of a metrological value: {errorResponse}");

        }

        #endregion

        #region (static) TryParse(CBORBytes, out MetrologicalValue, out ErrorResponse, Options = null)

        /// <summary>
        /// Try to parse the given CBOR data as a metrological value:
        /// Tag 44252 (0xACDC) wrapping [value, unit, ?prefix, ?uncertainty].
        ///
        /// This overload reads the STRICT decoder profile of Section 6 of the
        /// tag specification by default, which the generic CBOR reader does
        /// not: non-shortest heads, indefinite lengths, unsorted map keys and
        /// non-canonical NaNs are refused rather than normalized away.
        ///
        /// Two reasons the profile lives here rather than in the reader's own
        /// default. It is a rule of this tag, not of CBOR: the generic reader
        /// also serves COSE, where RFC 9052 does not require the sender to
        /// have encoded deterministically and a verifier must hash the bytes
        /// as they arrived. And it is a rule about BYTES, which a caller who
        /// already holds a CBORValue can no longer check - by then the
        /// evidence has been read away. That is why there is no options
        /// parameter on the CBORValue overload: it would be a promise this
        /// layer cannot keep.
        ///
        /// Pass CBORReaderOptions.Default for the lenient profile, which MAY
        /// accept a non-deterministic encoding - and note what Section 6 then
        /// requires of the caller: what was read leniently MUST NOT be
        /// re-encoded as it arrived. ToByteArray() sees to that.
        /// </summary>
        /// <param name="CBORBytes">The encoded CBOR data.</param>
        /// <param name="MetrologicalValue">The parsed metrological value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="Options">Optional CBOR reader options; the strict profile of Section 6 by default.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte>                CBORBytes,
                                       out MetrologicalValue             MetrologicalValue,
                                       [NotNullWhen(false)] out String?  ErrorResponse,
                                       CBORReaderOptions?                Options   = null)
        {

            MetrologicalValue = default;

            if (!CBORValue.TryParse(CBORBytes,
                                    out var cbor,
                                    out ErrorResponse,
                                    Options ?? CBORReaderOptions.Canonical))
            {
                return false;
            }

            return TryParse(cbor, out MetrologicalValue, out ErrorResponse);

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

                    // A rational exponent has one spelling: lowest terms, and
                    // never the rational form of an integer (specification
                    // section 3.2).
                    if (exponentNode.Kind == CBORValueKind.Array)
                    {

                        if (denominator == 1)
                        {
                            ErrorResponse = $"The unit exponent [{numerator}, 1] is the rational spelling of the integer {numerator}, which is written as an integer!";
                            return false;
                        }

                        if (System.Numerics.BigInteger.GreatestCommonDivisor(numerator, denominator) > 1)
                        {
                            ErrorResponse = $"The unit exponent {numerator}/{denominator} is not in lowest terms!";
                            return false;
                        }

                    }

                    factors.Add(new UnitFactor(factorUnit, (Int32) numerator, (Int32) denominator));

                }

                // A single named unit is written in the named form, never as
                // a one-element product (specification section 3.2).
                if (factors.Count == 1           &&
                    factors[0].Numerator   == 1  &&
                    factors[0].Denominator == 1)
                {
                    ErrorResponse = "A single named unit must be written in the named form, never as a one-element product!";
                    return false;
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

                // The wire carries symbols and aliases, never names
                // (specification section 3.2).
                if (!UnitOfMeasure.TryParseSymbol(Node.AsText(), out Unit))
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

            // An unknown key could state anything - an asymmetry, a
            // correlation, a second magnitude - and dropping it would
            // silently change what the uncertainty says (specification
            // section 3.4).
            foreach (var entry in Node.AsMap())
            {

                if (entry.Key.Kind != CBORValueKind.UnsignedInteger ||
                    entry.Key.AsUInt64() < 1                        ||
                    entry.Key.AsUInt64() > 5)
                {
                    ErrorResponse = $"An uncertainty map must not hold the unknown key '{entry.Key.ToDiagnosticString()}'!";
                    return false;
                }

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

            // A map holding nothing but the magnitude says exactly what the
            // bare number says, because the coverage factor defaults to 1 -
            // which would give one uncertainty two encodings (specification
            // section 3.4). Checked after the magnitude itself, so that a
            // negative one still reports the deeper fault.
            if (Node.AsMap().Count == 1)
            {
                ErrorResponse = "An uncertainty map stating only its magnitude says what a bare number says, which would give the same uncertainty two encodings!";
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

                // Distribution 0 means "not stated" and MUST be omitted rather
                // than written (specification section 3.4).
                if (distributionNode.AsUInt64() == 0)
                {
                    ErrorResponse = "The uncertainty distribution 0 means 'not stated' and must be omitted rather than written!";
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

            // A decimal fraction states decimal places, so its exponent is
            // negative on the wire; an integral reading is written as an
            // integer (specification section 3.1).
            if (Node.Kind == CBORValueKind.Tagged        &&
                Node.Tag  == CBORTag.DecimalFraction     &&
                Node.UntaggedValue.Kind  == CBORValueKind.Array &&
                Node.UntaggedValue.Count == 2            &&
                Node.UntaggedValue[0].TryGetInt64(out var decimalExponent) &&
                decimalExponent >= 0)
            {
                ErrorResponse = $"The {What} of a metrological value must not be a decimal fraction with a non-negative exponent: an integral reading is written as an integer!";
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

        #region (private static) TryParseTextNumber    (Text, What, out Number, out ErrorResponse)

        private static Boolean TryParseTextNumber(String                            Text,
                                                  String                            What,
                                                  out Decimal                       Number,
                                                  [NotNullWhen(false)] out String?  ErrorResponse)
        {

            var trimmed = Text.Trim();

            // The decimal scale is data: Decimal.TryParse keeps the trailing
            // zero of "1.10", and 5.0 mA must not come back as 5 mA. The
            // grammar gate in front of it requires digits on both sides of the
            // decimal point, which Decimal.TryParse alone would not: '5.' and
            // '.5' are not numbers of this format.
            if (IsGrammarNumber(trimmed) &&
                Decimal.TryParse(trimmed,
                                 NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                                 CultureInfo.InvariantCulture,
                                 out Number))
            {
                ErrorResponse = null;
                return true;
            }

            Number         = 0;
            ErrorResponse  = $"'{trimmed}' is not a valid {What}!";

            return false;

        }

        #endregion

        #region (private static) IsGrammarNumber       (Text)

        /// <summary>
        /// Whether the text matches the number production of the metrological
        /// text grammar: [sign] digits [. digits] [e [sign] digits]. Digits
        /// are required on both sides of the decimal point and after the
        /// exponent marker, so '5.', '.5' and '5e' are not numbers here,
        /// whatever a lenient numeric parser would accept.
        /// </summary>
        private static Boolean IsGrammarNumber(ReadOnlySpan<Char> Text)
        {

            var i = 0;

            if (i < Text.Length && (Text[i] == '+' || Text[i] == '-'))
                i++;

            var digits = 0;
            while (i < Text.Length && Text[i] >= '0' && Text[i] <= '9')
            {
                i++;
                digits++;
            }

            if (digits == 0)
                return false;

            if (i < Text.Length && Text[i] == '.')
            {

                i++;
                digits = 0;

                while (i < Text.Length && Text[i] >= '0' && Text[i] <= '9')
                {
                    i++;
                    digits++;
                }

                if (digits == 0)
                    return false;

            }

            if (i < Text.Length && (Text[i] == 'e' || Text[i] == 'E'))
            {

                i++;

                if (i < Text.Length && (Text[i] == '+' || Text[i] == '-'))
                    i++;

                digits = 0;

                while (i < Text.Length && Text[i] >= '0' && Text[i] <= '9')
                {
                    i++;
                    digits++;
                }

                if (digits == 0)
                    return false;

            }

            return i == Text.Length;

        }

        #endregion

        #region (private static) TryParseTextUnit      (Text, out Unit, out Prefix, out ErrorResponse)

        /// <summary>
        /// Try to parse a unit of measure that may carry an SI prefix folded
        /// onto the symbol of its leading factor.
        /// </summary>
        private static Boolean TryParseTextUnit(String                            Text,
                                                out UnitExpression                Unit,
                                                out SIPrefix                      Prefix,
                                                [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Prefix = SIPrefix.None;

            // The whole symbol wins over a prefix: "cd" is the candela and
            // never centi-day, "min" the minute and never milli-inch.
            if (UnitExpression.TryParse(Text, out Unit))
            {
                ErrorResponse = null;
                return true;
            }

            var separator  = Text.IndexOfAny(['·', '*']);
            var head       = separator < 0 ? Text : Text[..separator];
            var tail       = separator < 0 ? ""   : Text[separator..];

            // Only the leading factor may carry the prefix, and only when it
            // stands for the first power of itself: "ks^-2" would read as
            // (ks)^-2, a millionth of what a prefixed s^-2 means.
            if (!head.Contains('^'))
            {

                // Longest first: "da" is deca and not deci-are.
                for (var length = Math.Min(2, head.Length - 1); length >= 1; length--)
                {

                    if (!SIPrefix.TryParse(head[..length], out var siPrefix) || siPrefix.IsNone)
                        continue;

                    var symbol = head[length..];

                    // A prefix binds tighter than a power: "km²" reads as square
                    // kilometre, a million square metres, and not the thousand a
                    // prefixed "m²" would mean.
                    if (SymbolIsAPower(symbol))
                        continue;

                    if (UnitExpression.TryParse(symbol + tail, out var unitExpression))
                    {
                        Unit           = unitExpression;
                        Prefix         = siPrefix;
                        ErrorResponse  = null;
                        return true;
                    }

                }

            }

            ErrorResponse = $"Unknown unit of measure '{Text}'!";

            return false;

        }

        #endregion

        #region (private static) TryParseTextStatements(Text, Magnitude, out Uncertainty, out ErrorResponse)

        /// <summary>
        /// Try to parse what a text states about a measurement uncertainty
        /// beyond its magnitude: "k=2, p=0.95, dist=normal, ν=45".
        /// </summary>
        private static Boolean TryParseTextStatements(String                            Text,
                                                      Decimal                           Magnitude,
                                                      out MeasurementUncertainty        Uncertainty,
                                                      [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Uncertainty = default;

            Decimal?                  coverageFactor       = null;
            Double?                   coverageProbability  = null;
            UncertaintyDistribution?  distribution         = null;
            Double?                   degreesOfFreedom     = null;

            foreach (var statement in Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {

                var equals = statement.IndexOf('=');

                if (equals < 0)
                {
                    ErrorResponse = $"'{statement}' must be written as 'key=value'!";
                    return false;
                }

                var key    = statement[..equals].       TrimEnd();
                var value  = statement[(equals + 1)..]. TrimStart();

                switch (key)
                {

                    #region k=  the coverage factor

                    case "k":

                        if (coverageFactor.HasValue)
                        {
                            ErrorResponse = "The coverage factor is stated twice!";
                            return false;
                        }

                        if (!IsGrammarNumber(value) ||
                            !Decimal.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var k) ||
                            k <= 0)
                        {
                            ErrorResponse = $"'{value}' is not a valid coverage factor!";
                            return false;
                        }

                        coverageFactor = k;
                        break;

                    #endregion

                    #region p=  the coverage probability

                    case "p":

                        if (coverageProbability.HasValue)
                        {
                            ErrorResponse = "The coverage probability is stated twice!";
                            return false;
                        }

                        if (!IsGrammarNumber(value) ||
                            !Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var p) ||
                            p <= 0 || p > 1)
                        {
                            ErrorResponse = $"'{value}' is not a valid coverage probability!";
                            return false;
                        }

                        coverageProbability = p;
                        break;

                    #endregion

                    #region dist=  the probability distribution

                    case "dist":

                        if (distribution.HasValue)
                        {
                            ErrorResponse = "The probability distribution is stated twice!";
                            return false;
                        }

                        if (!TryParseDistribution(value, out var uncertaintyDistribution))
                        {
                            ErrorResponse = $"'{value}' is not a known probability distribution!";
                            return false;
                        }

                        distribution = uncertaintyDistribution;
                        break;

                    #endregion

                    #region ν=  the effective degrees of freedom

                    case "ν":
                    case "nu":

                        if (degreesOfFreedom.HasValue)
                        {
                            ErrorResponse = "The effective degrees of freedom are stated twice!";
                            return false;
                        }

                        if (!IsGrammarNumber(value) ||
                            !Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var nu) ||
                            nu <= 0)
                        {
                            ErrorResponse = $"'{value}' is not a valid number of effective degrees of freedom!";
                            return false;
                        }

                        degreesOfFreedom = nu;
                        break;

                    #endregion

                    default:
                        ErrorResponse = $"'{key}' states nothing this format knows about a measurement uncertainty!";
                        return false;

                }

            }

            Uncertainty = new MeasurementUncertainty(
                              Magnitude,
                              coverageFactor,
                              coverageProbability,
                              distribution,
                              degreesOfFreedom
                          );

            ErrorResponse = null;

            return true;

        }

        #endregion

        #region (private static) TryParseDistribution  (Text, out Distribution)

        private static Boolean TryParseDistribution(String                       Text,
                                                    out UncertaintyDistribution  Distribution)
        {

            Distribution = Text.ToLowerInvariant() switch {
                               "normal"       => UncertaintyDistribution.Normal,
                               "rectangular"  => UncertaintyDistribution.Rectangular,
                               "triangular"   => UncertaintyDistribution.Triangular,
                               "u-shaped"     => UncertaintyDistribution.UShaped,
                               "u-shape"      => UncertaintyDistribution.UShaped,
                               "t"            => UncertaintyDistribution.StudentT,
                               "student-t"    => UncertaintyDistribution.StudentT,
                               _              => UncertaintyDistribution.Unspecified
                           };

            return Distribution != UncertaintyDistribution.Unspecified;

        }

        #endregion

        #region (private static) DistributionText      (Distribution)

        private static String DistributionText(UncertaintyDistribution Distribution)

            => Distribution switch {
                   UncertaintyDistribution.Normal       => "normal",
                   UncertaintyDistribution.Rectangular  => "rectangular",
                   UncertaintyDistribution.Triangular   => "triangular",
                   UncertaintyDistribution.UShaped      => "u-shaped",
                   UncertaintyDistribution.StudentT     => "student-t",
                   _                                    => ""
               };

        #endregion

        #region (private static) SymbolIsAPower        (Symbol)

        /// <summary>
        /// Whether the given unit symbol already denotes a power of itself,
        /// such as "m²" or "m³", and therefore takes no folded SI prefix.
        /// </summary>
        private static Boolean SymbolIsAPower(String Symbol)

            => Symbol.Length > 0 &&
               Symbol[^1] is '⁰' or '¹' or '²' or '³' or '⁴' or
                             '⁵' or '⁶' or '⁷' or '⁸' or '⁹';

        #endregion

        #region (private static) RenderUnit            (Unit, Prefix)

        /// <summary>
        /// Render the unit of measure together with its SI prefix, and whatever
        /// power-of-ten scale the prefix could not be folded into.
        /// </summary>
        private static (String Unit, String Scale) RenderUnit(UnitExpression  Unit,
                                                              SIPrefix        Prefix)
        {

            var unitText = Unit.ToString();

            if (Prefix.IsNone)
                return (unitText, "");

            // Fold the prefix onto the leading symbol only where reading that
            // back yields the very same unit and prefix again. "mA" does, but
            // "km²" would read as square kilometre - a million square metres,
            // where this value means a thousand - and "cd" is the candela and
            // never a centi-day. Whatever does not survive its own parser is
            // written with an explicit power-of-ten scale instead.
            var folded = Prefix.Symbol + unitText;

            if (TryParseTextUnit(folded, out var unit, out var prefix, out _) &&
                unit   == Unit &&
                prefix == Prefix)
            {
                return (folded, "");
            }

            return (unitText, $"×10^{Prefix.Exponent.ToString(CultureInfo.InvariantCulture)}");

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
        /// Return the one-string text representation of this metrological value,
        /// e.g. "5.0 mA", "9.81 m·s^-2" or "(230.00 ±0.12) V, k=2".
        /// This is the format TryParse(String, ...) reads back, and it is
        /// lossless: the value, its decimal scale, the unit of measure, the SI
        /// prefix and everything stated about the uncertainty survive the round
        /// trip unchanged.
        /// </summary>
        public override String ToString()
        {

            if (IsEmpty)
                return Value.ToString(CultureInfo.InvariantCulture);

            var (unitText, scaleText)  = RenderUnit(Unit, Prefix);
            var stringBuilder          = new StringBuilder();

            if (Uncertainty.HasValue)
                stringBuilder.Append('(').
                              Append(Value.                   ToString(CultureInfo.InvariantCulture)).
                              Append(" ±").
                              Append(Uncertainty.Value.Value. ToString(CultureInfo.InvariantCulture)).
                              Append(')');

            else
                stringBuilder.Append(Value.ToString(CultureInfo.InvariantCulture));

            stringBuilder.Append(scaleText).
                          Append(' ').
                          Append(unitText);

            if (Uncertainty.HasValue)
            {

                var uncertainty = Uncertainty.Value;

                if (uncertainty.CoverageFactor != 1)
                    stringBuilder.Append(", k=").    Append(uncertainty.CoverageFactor.            ToString(CultureInfo.InvariantCulture));

                if (uncertainty.CoverageProbability.HasValue)
                    stringBuilder.Append(", p=").    Append(uncertainty.CoverageProbability.Value. ToString(CultureInfo.InvariantCulture));

                if (uncertainty.Distribution != UncertaintyDistribution.Unspecified)
                    stringBuilder.Append(", dist="). Append(DistributionText(uncertainty.Distribution));

                if (uncertainty.DegreesOfFreedom.HasValue)
                    stringBuilder.Append(", ν=").    Append(uncertainty.DegreesOfFreedom.Value.    ToString(CultureInfo.InvariantCulture));

            }

            return stringBuilder.ToString();

        }

        #endregion

    }

}
