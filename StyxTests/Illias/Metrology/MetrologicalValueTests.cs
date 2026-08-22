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

using Newtonsoft.Json;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Tests for metrological values and their CBOR representation
    /// (tag 44252, 0xACDC): [value, unit, ?prefix, ?uncertainty].
    /// </summary>
    [TestFixture]
    public class MetrologicalValueTests
    {

        #region (private static) CBORHexOf(MetrologicalValue, SymbolicUnit = false)

        private static String CBORHexOf(MetrologicalValue  MetrologicalValue,
                                        Boolean            SymbolicUnit = false)
        {

            // The DOM path...
            var domHex = Convert.ToHexString(MetrologicalValue.ToCBOR(SymbolicUnit).ToByteArray());

            // ...and the streaming path must agree!
            var cborWriter = new CBORWriter();
            MetrologicalValue.WriteTo(cborWriter, SymbolicUnit);

            Assert.That(Convert.ToHexString(cborWriter.ToByteArray()),
                        Is.EqualTo(domHex),
                        "The DOM and the streaming encoding differ!");

            return domHex;

        }

        #endregion


        #region The_golden_wire_vectors_encode_byte_exact_and_decode_back()

        [Test]
        public void The_golden_wire_vectors_encode_byte_exact_and_decode_back()
        {

            var vectors = new (MetrologicalValue Value, String Hex)[] {

                              // 5 A - the integral fast path
                              (new MetrologicalValue(5m,     UnitOfMeasure.Ampere),
                               "D9ACDC820504"),

                              // 230 V
                              (new MetrologicalValue(230m,   UnitOfMeasure.Volt),
                               "D9ACDC8218E605"),

                              // 5.0 mA - the decimal scale is preserved
                              (new MetrologicalValue(5.0m,   UnitOfMeasure.Ampere,   SIPrefix.Milli),
                               "D9ACDC83C4822018320422"),

                              // 1.10 kWh
                              (new MetrologicalValue(1.10m,  UnitOfMeasure.WattHour, SIPrefix.Kilo),
                               "D9ACDC83C48221186E0203"),

                              // (5.00 ±0.02) mA - with a GUM standard uncertainty
                              (new MetrologicalValue(5.00m,  UnitOfMeasure.Ampere,   SIPrefix.Milli,  0.02m),
                               "D9ACDC84C482211901F40422C4822102")

                          };

            foreach (var vector in vectors)
            {

                Assert.That(CBORHexOf(vector.Value),
                            Is.EqualTo(vector.Hex),
                            vector.Value.ToString());

                Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString(vector.Hex)),
                                                       out var parsedValue,
                                                       out var errorResponse),
                            Is.True,
                            errorResponse);

                Assert.That(parsedValue,  Is.EqualTo(vector.Value),  vector.Hex);

            }

        }

        #endregion

        #region Wire_scale_is_preserved()

        [Test]
        public void Wire_scale_is_preserved()
        {

            var oneTen  = new MetrologicalValue(1.10m, UnitOfMeasure.WattHour, SIPrefix.Kilo);
            var oneOne  = new MetrologicalValue(1.1m,  UnitOfMeasure.WattHour, SIPrefix.Kilo);

            // 1.10 kWh and 1.1 kWh are different representations...
            Assert.That(CBORHexOf(oneTen),   Is.Not.EqualTo(CBORHexOf(oneOne)));
            Assert.That(oneTen,              Is.Not.EqualTo(oneOne));

            // ...of the same physical quantity!
            Assert.That(oneTen.EquivalentTo(oneOne),  Is.True);

            // The scale survives the roundtrip...
            MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString(CBORHexOf(oneTen))), out var reparsed, out _);

            Assert.That(reparsed.Value,        Is.EqualTo(1.10m));
            Assert.That(reparsed.Value.Scale,  Is.EqualTo(2));

        }

        #endregion

        #region All_prefixes_roundtrip_over_CBOR()

        [Test]
        public void All_prefixes_roundtrip_over_CBOR()
        {

            foreach (var siPrefix in SIPrefix.All)
            {

                var value = new MetrologicalValue(12.34m, UnitOfMeasure.Ampere, siPrefix);
                var bytes = value.ToCBOR().ToByteArray();

                Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(bytes),
                                                       out var parsedValue,
                                                       out var errorResponse),
                            Is.True,
                            $"{siPrefix.Name}: {errorResponse}");

                Assert.That(parsedValue,  Is.EqualTo(value),  siPrefix.Name);

            }

        }

        #endregion

        #region Symbolic_units_are_written_on_request_and_always_accepted()

        [Test]
        public void Symbolic_units_are_written_on_request_and_always_accepted()
        {

            var value = new MetrologicalValue(5.0m, UnitOfMeasure.Ampere, SIPrefix.Milli);

            // 5.0 mA with the unit as the text "A" (0x6141)...
            var symbolicHex = CBORHexOf(value, SymbolicUnit: true);

            Assert.That(symbolicHex,  Is.EqualTo("D9ACDC83C482201832614122"));

            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString(symbolicHex)),
                                                   out var parsedValue,
                                                   out var errorResponse),
                        Is.True,
                        errorResponse);

            Assert.That(parsedValue,  Is.EqualTo(value));

        }

        #endregion

        #region Uncertainty_forces_an_explicit_prefix_and_rejects_negative_values()

        [Test]
        public void Uncertainty_forces_an_explicit_prefix_and_rejects_negative_values()
        {

            // (5 ±0.5) A: The prefix must be written explicitly (0),
            // as the uncertainty is trailing-optional...
            var value = new MetrologicalValue(5m, UnitOfMeasure.Ampere, null, 0.5m);

            Assert.That(CBORHexOf(value),  Is.EqualTo("D9ACDC84050400C4822005"));

            // A negative uncertainty is rejected at construction...
            Assert.That(() => new MetrologicalValue(5m, UnitOfMeasure.Ampere, null, -0.1m),
                        Throws.TypeOf<ArgumentException>());

            // ...and when parsing: 44252([5, 4, 0, -1])
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC8405040020")),
                                                   out _,
                                                   out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("negative"));

        }

        #endregion

        #region GUM_ToString_renders_the_parenthesized_form()

        [Test]
        public void GUM_ToString_renders_the_parenthesized_form()
        {

            Assert.That(new MetrologicalValue(5.0m,  UnitOfMeasure.Ampere, SIPrefix.Milli).ToString(),
                        Is.EqualTo("5.0 mA"));

            Assert.That(new MetrologicalValue(230m,  UnitOfMeasure.Volt).ToString(),
                        Is.EqualTo("230 V"));

            Assert.That(new MetrologicalValue(5.00m, UnitOfMeasure.Ampere, SIPrefix.Milli, 0.02m).ToString(),
                        Is.EqualTo("(5.00 ±0.02) mA"));

            Assert.That(new MetrologicalValue(1.10m, UnitOfMeasure.WattHour, SIPrefix.Kilo).ToString(),
                        Is.EqualTo("1.10 kWh"));

        }

        #endregion

        #region EquivalentTo_folds_prefixes_and_Equals_does_not()

        [Test]
        public void EquivalentTo_folds_prefixes_and_Equals_does_not()
        {

            var fiveMilliAmpere   = new MetrologicalValue(5.0m,   UnitOfMeasure.Ampere, SIPrefix.Milli);
            var fiveThousandthsA  = new MetrologicalValue(0.005m, UnitOfMeasure.Ampere);

            Assert.That(fiveMilliAmpere.EquivalentTo(fiveThousandthsA),   Is.True);
            Assert.That(fiveMilliAmpere,                                  Is.Not.EqualTo(fiveThousandthsA));

            // Different units are never equivalent...
            Assert.That(fiveMilliAmpere.EquivalentTo(new MetrologicalValue(5.0m, UnitOfMeasure.Volt, SIPrefix.Milli)),
                        Is.False);

            // Uncertainties must be equivalent as well...
            var preciseMilli  = new MetrologicalValue(5.0m,   UnitOfMeasure.Ampere, SIPrefix.Milli, 0.5m);
            var preciseBase   = new MetrologicalValue(0.005m, UnitOfMeasure.Ampere, null,           0.0005m);

            Assert.That(preciseMilli.EquivalentTo(preciseBase),           Is.True);
            Assert.That(preciseMilli.EquivalentTo(fiveThousandthsA),      Is.False);

            // Quetta and quecto exceed the decimal range of Factor,
            // but EquivalentTo runs on exact big-integer arithmetic:
            // 1 QA == 1000 RA!
            var oneQuettaAmpere  = new MetrologicalValue(1m,    UnitOfMeasure.Ampere, SIPrefix.Quetta);
            var thousandRonna    = new MetrologicalValue(1000m, UnitOfMeasure.Ampere, SIPrefix.Ronna);

            Assert.That(oneQuettaAmpere.EquivalentTo(thousandRonna),      Is.True);
            Assert.That(oneQuettaAmpere.TryToBaseUnit(out _),             Is.False);

        }

        #endregion

        #region ConvertTo_scales_value_and_uncertainty_without_loss()

        [Test]
        public void ConvertTo_scales_value_and_uncertainty_without_loss()
        {

            var value      = new MetrologicalValue(5.0m, UnitOfMeasure.Ampere, SIPrefix.Milli, 0.5m);

            var microAmps  = value.ConvertTo(SIPrefix.Micro);

            Assert.That(microAmps.Value,                 Is.EqualTo(5000.0m));
            Assert.That(microAmps.Uncertainty!.Value.Value,  Is.EqualTo(500.0m));
            Assert.That(microAmps.Prefix,                Is.EqualTo(SIPrefix.Micro));

            var baseAmps   = value.ConvertTo(SIPrefix.None);

            Assert.That(baseAmps.Value,                  Is.EqualTo(0.005m));
            Assert.That(baseAmps.Uncertainty!.Value.Value,   Is.EqualTo(0.0005m));

            Assert.That(value.EquivalentTo(microAmps),  Is.True);
            Assert.That(value.EquivalentTo(baseAmps),   Is.True);

            // A value at the decimal scale limit can not be divided any further...
            var tiny = new MetrologicalValue(0.0000000000000000000000000001m, UnitOfMeasure.Ampere);

            Assert.That(() => tiny.ConvertTo(SIPrefix.Kilo),
                        Throws.TypeOf<OverflowException>());

        }

        #endregion

        #region StdDev_bridges_into_the_measurement_uncertainty()

        [Test]
        public void StdDev_bridges_into_the_measurement_uncertainty()
        {

            var stdDev  = new StdDev<Decimal>(5.00m, 0.02m);

            var value   = MetrologicalValue.From(stdDev,
                                                 UnitOfMeasure.Ampere,
                                                 SIPrefix.Milli);

            Assert.That(value,  Is.EqualTo(new MetrologicalValue(5.00m, UnitOfMeasure.Ampere, SIPrefix.Milli, 0.02m)));

        }

        #endregion

        #region Semantic_errors_are_rejected_when_parsing()

        [Test]
        public void Semantic_errors_are_rejected_when_parsing()
        {

            // An unknown numeric unit: 44252([5, 999])
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC82051903E7")),
                                                   out _, out var error1),  Is.False);
            Assert.That(error1,  Does.Contain("Unknown unit"));

            // An unknown symbolic unit: 44252([5, "XYZ"])
            Assert.That(MetrologicalValue.TryParse(CBORValue.Tagged(CBORTag.MetrologicalValue,
                                                                    CBORValue.FromArray(5, "XYZ")),
                                                   out _, out var error2),  Is.False);
            Assert.That(error2,  Does.Contain("Unknown unit"));

            // A binary float value: 44252([1.5, 4])
            Assert.That(MetrologicalValue.TryParse(CBORValue.Tagged(CBORTag.MetrologicalValue,
                                                                    CBORValue.FromArray(CBORValue.FromDouble(1.5), 4)),
                                                   out _, out var error3),  Is.False);
            Assert.That(error3,  Does.Contain("binary floating-point"));

            // A non-canonical SI prefix exponent: 44252([5, 4, 4])
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC83050404")),
                                                   out _, out var error4),  Is.False);
            Assert.That(error4,  Does.Contain("canonical SI prefix"));

            // Array lengths other than 2..4...
            Assert.That(MetrologicalValue.TryParse(CBORValue.Tagged(CBORTag.MetrologicalValue,
                                                                    CBORValue.FromArray(5)),
                                                   out _, out var error5),  Is.False);
            Assert.That(error5,  Does.Contain("2..4"));

            Assert.That(MetrologicalValue.TryParse(CBORValue.Tagged(CBORTag.MetrologicalValue,
                                                                    CBORValue.FromArray(5, 4, 0, 1, 2)),
                                                   out _, out var error6),  Is.False);

            // A missing tag...
            Assert.That(MetrologicalValue.TryParse(CBORValue.FromArray(5, 4),
                                                   out _, out var error7),  Is.False);
            Assert.That(error7,  Does.Contain("44252"));

            // A written distribution 0, which means "not stated" and must be
            // omitted rather than written: 44252([5, 4, 0, {1: 1, 4: 0}])
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC84050400A201010400")),
                                                   out _, out var error8),  Is.False);
            Assert.That(error8,  Does.Contain("omitted"));

            // One spelling per reading, per exponent, per unit, per prefix
            // (specification sections 3.1..3.4):

            // A decimal fraction with a non-negative exponent: 4([0, 500]), 4([1, 50])
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC82C482001901F405")),
                                                   out _, out var error9),  Is.False);
            Assert.That(error9,  Does.Contain("non-negative"));

            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC82C48201183205")),
                                                   out _, out _),  Is.False);

            // A rational exponent that is not in lowest terms: [[8, [-2, 4]]]
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC8205818208822104")),
                                                   out _, out var error10),  Is.False);
            Assert.That(error10,  Does.Contain("lowest terms"));

            // The rational spelling of an integer exponent: [[8, [2, 1]]]
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC8205818208820201")),
                                                   out _, out var error11),  Is.False);
            Assert.That(error11,  Does.Contain("integer"));

            // A single named unit as a one-element product: [[4, 1]]
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC820581820401")),
                                                   out _, out var error12),  Is.False);
            Assert.That(error12,  Does.Contain("named form"));

            // A redundant prefix 0 without a trailing uncertainty: [5, 4, 0]
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC83050400")),
                                                   out _, out var error13),  Is.False);
            Assert.That(error13,  Does.Contain("omitted"));

            // An unknown uncertainty map key: {1: 1, 6: 1}
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC84050400A201010601")),
                                                   out _, out var error14),  Is.False);
            Assert.That(error14,  Does.Contain("unknown key"));

            // An uncertainty map stating nothing but its magnitude: {1: 2}.
            // The coverage factor defaults to 1, so it says what the bare
            // number says and the reading would have two encodings.
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString("D9ACDC84050400A10102")),
                                                   out _, out var error15),  Is.False);
            Assert.That(error15,  Does.Contain("two encodings"));

            // A unit *name* on the wire - symbols and aliases only: 44252([5, "Ampere"])
            Assert.That(MetrologicalValue.TryParse(CBORValue.Tagged(CBORTag.MetrologicalValue,
                                                                    CBORValue.FromArray(5, "Ampere")),
                                                   out _, out var error16),  Is.False);
            Assert.That(error16,  Does.Contain("Unknown unit"));

        }

        #endregion

        #region A_generic_decoder_reads_the_tagged_array_without_knowing_the_tag()

        [Test]
        public void A_generic_decoder_reads_the_tagged_array_without_knowing_the_tag()
        {

            // 5.0 mA seen by a decoder that knows nothing about tag 44252...
            var cbor = CBORValue.Parse(Convert.FromHexString("D9ACDC83C4822018320422"));

            Assert.That(cbor.Kind,                          Is.EqualTo(CBORValueKind.Tagged));
            Assert.That(cbor.Tag.Value,                     Is.EqualTo(44252));

            var array = cbor.UntaggedValue;

            Assert.That(array.Count,                        Is.EqualTo(3));
            Assert.That(array[0].AsDecimal(),               Is.EqualTo(5.0m));
            Assert.That(array[1].AsUInt64(),                Is.EqualTo(4));
            Assert.That(array[2].AsInt64(),                 Is.EqualTo(-3));

            Assert.That(cbor.ToDiagnosticString(),          Is.EqualTo("44252([4([-1, 50]), 4, -3])"));

        }

        #endregion

        #region The_reader_convenience_parses_streamed_metrological_values()

        [Test]
        public void The_reader_convenience_parses_streamed_metrological_values()
        {

            var reader   = new CBORReader(Convert.FromHexString("D9ACDC83C4822018320422"));

            var success  = reader.TryReadMetrologicalValue(out var value, out var errorResponse);

            Assert.That(success,          Is.True,  errorResponse);
            Assert.That(value,            Is.EqualTo(new MetrologicalValue(5.0m, UnitOfMeasure.Ampere, SIPrefix.Milli)));
            Assert.That(reader.PeekState(),  Is.EqualTo(CBORReaderState.Finished));

        }

        #endregion

        #region Typed_metrology_structs_bridge_to_metrological_values()

        [Test]
        public void Typed_metrology_structs_bridge_to_metrological_values()
        {

            // Watt...
            var power = Watt.FromW(1500m).AsMetrologicalValue(SIPrefix.Kilo);

            Assert.That(power.ToString(),                     Is.EqualTo("1.5 kW"));
            Assert.That(power.TryToWatt(out var watt),        Is.True);
            Assert.That(watt.Value,                           Is.EqualTo(1500m));

            // WattHour...
            var energy = WattHour.FromWh(42000m).AsMetrologicalValue(SIPrefix.Kilo);

            Assert.That(energy.ToString(),                    Is.EqualTo("42 kWh"));
            Assert.That(energy.TryToWattHour(out var wh),     Is.True);
            Assert.That(wh.Value,                             Is.EqualTo(42000m));

            // Kilogram bridges to (Gram, Kilo)...
            var mass = Kilogram.FromKG(5m).AsMetrologicalValue();

            Assert.That(mass.Unit,                            Is.EqualTo(UnitOfMeasure.Gram));
            Assert.That(mass.Prefix,                          Is.EqualTo(SIPrefix.Kilo));
            Assert.That(mass.ToString(),                      Is.EqualTo("5 kg"));

            Assert.That(mass.TryToKilogram(out var kilogram), Is.True);
            Assert.That(kilogram.Value,                       Is.EqualTo(5m));

            // ...and 5000 g is 5 kg as well!
            Assert.That(new MetrologicalValue(5000m, UnitOfMeasure.Gram).TryToKilogram(out var kilogram2),  Is.True);
            Assert.That(kilogram2.Value,                      Is.EqualTo(5m));

            // The wrong unit does not convert...
            Assert.That(power.TryToWattHour(out _),           Is.False);

        }

        #endregion

        #region JSON_roundtrips_preserve_the_scale()

        [Test]
        public void JSON_roundtrips_preserve_the_scale()
        {

            var value = new MetrologicalValue(5.00m, UnitOfMeasure.Ampere, SIPrefix.Milli, 0.02m);
            var json  = value.ToJSON();

            // JValue(Decimal) preserves the decimal scale...
            Assert.That(json.ToString(Formatting.None),
                        Is.EqualTo("""{"value":5.00,"uncertainty":0.02,"unit":"A","prefix":"m"}"""));

            Assert.That(MetrologicalValue.TryParse(json, out var parsedValue, out var errorResponse),
                        Is.True,
                        errorResponse);

            Assert.That(parsedValue,  Is.EqualTo(value));

            // Without a prefix and uncertainty both properties are omitted.
            // Note: Newtonsoft always serializes decimals with at least
            // one decimal place ("230.0")...
            Assert.That(new MetrologicalValue(230m, UnitOfMeasure.Volt).ToJSON().ToString(Formatting.None),
                        Is.EqualTo("""{"value":230.0,"unit":"V"}"""));

        }

        #endregion

        #region Reading_from_bytes_is_strict_by_default()

        [Test]
        public void Reading_from_bytes_is_strict_by_default()
        {

            // Section 6 of the tag specification RECOMMENDS the strict decoder
            // profile and names the byte level it covers: shortest integer
            // heads, definite lengths, sorted map keys, preferred bignums.
            // Every second spelling below says exactly what the canonical
            // bytes beside it say - which is what a format whose encoding is
            // a function of its value must not have two of.
            var spellings = new (String Canonical, String Second, String What)[] {
                                 ("D9ACDC820504",              "D9ACDC82180504",            "a non-shortest integer head"),
                                 ("D9ACDC820504",              "D9ACDC9F0504FF",            "an indefinite-length array"),
                                 ("D9ACDC820504",              "D9ACDC82C2410504",          "a bignum a basic integer could carry"),
                                 ("D9ACDC84050400A201020202",  "D9ACDC84050400A202020102",  "an unsorted uncertainty map")
                             };

            foreach (var (canonical, second, what) in spellings)
            {

                Assert.That(MetrologicalValue.TryParse(Convert.FromHexString(canonical),
                                                       out var expected,
                                                       out var canonicalError),
                            Is.True,
                            $"the canonical spelling beside {what}: {canonicalError}");

                // Strict is the default, and this is what it is for.
                Assert.That(MetrologicalValue.TryParse(Convert.FromHexString(second),
                                                       out _,
                                                       out _),
                            Is.False,
                            $"strict accepted {what}");

                // Lenient on request: read, and read as the SAME reading -
                // which is what makes it a second spelling rather than a
                // different document.
                Assert.That(MetrologicalValue.TryParse(Convert.FromHexString(second),
                                                       out var lenient,
                                                       out var lenientError,
                                                       CBORReaderOptions.Default),
                            Is.True,
                            $"lenient refused {what}: {lenientError}");

                Assert.That(lenient,  Is.EqualTo(expected),  what);

                // And the other half of Section 6: what a lenient decoder
                // read MUST NOT be written back as it arrived.
                Assert.That(Convert.ToHexString(lenient.ToByteArray()),
                            Is.EqualTo(canonical),
                            what);

            }

        }

        #endregion

    }

}
