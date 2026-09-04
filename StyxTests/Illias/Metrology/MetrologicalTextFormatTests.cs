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

using System.Globalization;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Tests for the one-string text format of a metrological value:
    /// "5.0 mA", "9.81 m·s^-2", "(230.00 ±0.12) V, k=2".
    /// This is the format the CBOR/JSON document conversion maps every
    /// tag 44252 item onto, so it has to be lossless in both directions.
    /// </summary>
    [TestFixture]
    public class MetrologicalTextFormatTests
    {

        #region (private static) RoundTrip(MetrologicalValue)

        /// <summary>
        /// Render the given metrological value, read the text back and
        /// return what came out of it.
        /// </summary>
        private static MetrologicalValue RoundTrip(MetrologicalValue MetrologicalValue)
        {

            var text = MetrologicalValue.ToString();

            Assert.That(MetrologicalValue.TryParse(text, out var parsed, out var errorResponse),
                        Is.True,
                        $"'{text}' does not parse back: {errorResponse}");

            Assert.That(parsed.ToString(),
                        Is.EqualTo(text),
                        "The second rendering differs from the first!");

            return parsed;

        }

        #endregion


        #region The_examples_of_the_specification_render_as_one_string()

        [Test]
        public void The_examples_of_the_specification_render_as_one_string()
        {

            // Section 5 of the tag 44252 specification, the "Reading" column - with the
            // caret spelling of the unit exponents Styx writes everywhere.
            var vectors = new (String Hex, String Text) [] {
                ("D9ACDC8205 04",                              "5 A"),
                ("D9ACDC8218E6 05",                            "230 V"),
                ("D9ACDC83C482201832 04 22",                   "5.0 mA"),
                ("D9ACDC83C48221186E 02 03",                   "1.10 kWh"),
                ("D9ACDC84C482211901F4 04 22 C4822102",        "(5.00 ±0.02) mA"),
                ("D9ACDC8405 04 00 C4822005",                  "(5 ±0.5) A"),
                ("D9ACDC83C482201832 6141 22",                 "5.0 mA"),
                ("D9ACDC82C482211903D5 82820F01820821",        "9.81 m·s^-2"),
                ("D9ACDC84C482211959D8 05 00 A201C482210C0202", "(230.00 ±0.12) V, k=2"),
                ("D9ACDC83C48220182D 82820501820982200228",    "4.5 nV·Hz^-1/2")
            };

            foreach (var vector in vectors)
            {

                var cbor = CBORValue.Parse(Convert.FromHexString(vector.Hex.Replace(" ", "")));

                Assert.That(MetrologicalValue.TryParse(cbor, out var metrologicalValue, out var errorResponse),
                            Is.True,
                            $"{vector.Hex}: {errorResponse}");

                Assert.That(metrologicalValue.ToString(),
                            Is.EqualTo(vector.Text),
                            vector.Hex);

                // ...and the text is the whole value: it reads back into an equal one.
                Assert.That(MetrologicalValue.Parse(vector.Text),
                            Is.EqualTo(metrologicalValue),
                            vector.Text);

            }

        }

        #endregion

        #region The_decimal_scale_survives_the_text()

        [Test]
        public void The_decimal_scale_survives_the_text()
        {

            // "1.10 kWh" is a different reading than "1.1 kWh": the trailing
            // zero states the resolution of the instrument.
            Assert.That(new MetrologicalValue(1.10m, UnitOfMeasure.WattHour, SIPrefix.Kilo).ToString(),
                        Is.EqualTo("1.10 kWh"));

            Assert.That(new MetrologicalValue(1.1m,  UnitOfMeasure.WattHour, SIPrefix.Kilo).ToString(),
                        Is.EqualTo("1.1 kWh"));

            Assert.That(MetrologicalValue.Parse("1.10 kWh").Value.ToString(CultureInfo.InvariantCulture),
                        Is.EqualTo("1.10"));

            Assert.That(MetrologicalValue.Parse("1.10 kWh"),
                        Is.Not.EqualTo(MetrologicalValue.Parse("1.1 kWh")));

            // The uncertainty keeps its own scale as well.
            Assert.That(MetrologicalValue.Parse("(5.00 ±0.020) mA").ToString(),
                        Is.EqualTo("(5.00 ±0.020) mA"));

        }

        #endregion

        #region The_whole_symbol_wins_over_a_prefix()

        [Test]
        public void The_whole_symbol_wins_over_a_prefix()
        {

            // Every one of these could be read as a prefix plus a symbol,
            // and every one of them is a unit of its own.
            var vectors = new (String Text, UnitOfMeasure Unit) [] {
                ("1 cd",   UnitOfMeasure.Candela),      // and never centi-day
                ("1 min",  UnitOfMeasure.Minute),       // and never milli-inch
                ("1 Pa",   UnitOfMeasure.Pascal),       // and never peta-are
                ("1 mol",  UnitOfMeasure.Mole),
                ("1 kat",  UnitOfMeasure.Katal),
                ("1 rad",  UnitOfMeasure.Radian),
                ("1 h",    UnitOfMeasure.Hour),         // and never hecto-anything
                ("1 T",    UnitOfMeasure.Tesla),        // and never tera-anything
                ("1 t",    UnitOfMeasure.Tonne),
                ("1 Wb",   UnitOfMeasure.Weber),
                ("1 lm",   UnitOfMeasure.Lumen),
                ("1 Gy",   UnitOfMeasure.Gray)
            };

            foreach (var vector in vectors)
            {

                var metrologicalValue = MetrologicalValue.Parse(vector.Text);

                Assert.That(metrologicalValue.Unit.SingleUnit,  Is.EqualTo(vector.Unit),      vector.Text);
                Assert.That(metrologicalValue.Prefix.IsNone,    Is.True,                      vector.Text);
                Assert.That(metrologicalValue.ToString(),       Is.EqualTo(vector.Text),      vector.Text);

            }

        }

        #endregion

        #region A_prefix_is_split_off_only_when_the_whole_symbol_is_unknown()

        [Test]
        public void A_prefix_is_split_off_only_when_the_whole_symbol_is_unknown()
        {

            var vectors = new (String Text, UnitOfMeasure Unit, SIPrefix Prefix) [] {
                ("5.0 mA",    UnitOfMeasure.Ampere,    SIPrefix.Milli),
                ("1.10 kWh",  UnitOfMeasure.WattHour,  SIPrefix.Kilo),
                ("5 kg",      UnitOfMeasure.Gram,      SIPrefix.Kilo),    // there is no "kg" in the registry
                ("2 dam",     UnitOfMeasure.Meter,     SIPrefix.Deca),    // "da" wins over "d": deca-metre
                ("3 mΩ",      UnitOfMeasure.Ohm,       SIPrefix.Milli),
                ("7 µF",      UnitOfMeasure.Farad,     SIPrefix.Micro),
                ("9 dB",      UnitOfMeasure.Byte,      SIPrefix.Deci),    // the bel is not a registered unit!
                ("4 m°C",     UnitOfMeasure.Celsius,   SIPrefix.Milli)
            };

            foreach (var vector in vectors)
            {

                var metrologicalValue = MetrologicalValue.Parse(vector.Text);

                Assert.That(metrologicalValue.Unit.SingleUnit,  Is.EqualTo(vector.Unit),    vector.Text);
                Assert.That(metrologicalValue.Prefix,           Is.EqualTo(vector.Prefix),  vector.Text);
                Assert.That(metrologicalValue.ToString(),       Is.EqualTo(vector.Text),    vector.Text);

            }

        }

        #endregion

        #region A_prefix_is_folded_only_where_that_would_not_lie()

        [Test]
        public void A_prefix_is_folded_only_where_that_would_not_lie()
        {

            // "km²" reads as square kilometre - a million square metres, while
            // a prefixed m² means a thousand of them. The same applies to any
            // leading factor whose exponent is not 1. Both are written with an
            // explicit power-of-ten scale instead.
            Assert.That(new MetrologicalValue(5m, UnitOfMeasure.SquareMeter, SIPrefix.Kilo).ToString(),
                        Is.EqualTo("5×10^3 m²"));

            Assert.That(new MetrologicalValue(5m, UnitOfMeasure.CubicMeter,  SIPrefix.Milli).ToString(),
                        Is.EqualTo("5×10^-3 m³"));

            Assert.That(new MetrologicalValue(2m,
                                              new UnitExpression(new UnitFactor(UnitOfMeasure.Second, -2)),
                                              SIPrefix.Kilo).ToString(),
                        Is.EqualTo("2×10^3 s^-2"));

            // ...and with an uncertainty the whole parenthesis is scaled.
            Assert.That(new MetrologicalValue(5m, UnitOfMeasure.SquareMeter, SIPrefix.Kilo, 0.5m).ToString(),
                        Is.EqualTo("(5 ±0.5)×10^3 m²"));

            // ...and neither is a prefix folded where the result would read
            // as a unit of its own: "cd" is the candela, so a centi-day has
            // to be written the long way.
            Assert.That(new MetrologicalValue(1.25m, UnitOfMeasure.Day, SIPrefix.Centi).ToString(),
                        Is.EqualTo("1.25×10^-2 d"));

            Assert.That(MetrologicalValue.Parse("1.25×10^-2 d").Unit.SingleUnit,
                        Is.EqualTo(UnitOfMeasure.Day));

            // A leading factor of exponent 1 does take the prefix, even
            // within a product of powers.
            Assert.That(new MetrologicalValue(4.5m,
                                              new UnitExpression(new UnitFactor(UnitOfMeasure.Volt,  1),
                                                                 new UnitFactor(UnitOfMeasure.Hertz, -1, 2)),
                                              SIPrefix.Nano).ToString(),
                        Is.EqualTo("4.5 nV·Hz^-1/2"));

        }

        #endregion

        #region The_power_of_ten_scale_reads_back_as_the_prefix_it_came_from()

        [Test]
        public void The_power_of_ten_scale_reads_back_as_the_prefix_it_came_from()
        {

            var scaled = MetrologicalValue.Parse("5×10^3 m²");

            Assert.Multiple(() => {
                Assert.That(scaled.Value,                Is.EqualTo(5m));
                Assert.That(scaled.Unit.SingleUnit,      Is.EqualTo(UnitOfMeasure.SquareMeter));
                Assert.That(scaled.Prefix,               Is.EqualTo(SIPrefix.Kilo));
            });

            // The ASCII spelling is accepted as well.
            Assert.That(MetrologicalValue.Parse("5*10^3 m²"),  Is.EqualTo(scaled));

            // A scale is a prefix and nothing else: only the 25 canonical
            // exponents exist, and 10^4 is not one of them.
            Assert.That(MetrologicalValue.TryParse("5×10^4 m²", out _, out var errorResponse),  Is.False);
            Assert.That(errorResponse,                                                          Does.Contain("10^4"));

            // A prefix folded where it would lie is refused rather than guessed.
            Assert.That(MetrologicalValue.TryParse("5 km²",   out _, out _),  Is.False);
            Assert.That(MetrologicalValue.TryParse("2 ks^-2", out _, out _),  Is.False);

        }

        #endregion

        #region Everything_stated_about_the_uncertainty_survives_the_text()

        [Test]
        public void Everything_stated_about_the_uncertainty_survives_the_text()
        {

            var certified = new MetrologicalValue(
                                230.00m,
                                UnitOfMeasure.Volt,
                                SIPrefix.None,
                                new MeasurementUncertainty(
                                    0.12m,
                                    CoverageFactor:       2,
                                    CoverageProbability:  0.95,
                                    Distribution:         UncertaintyDistribution.Normal,
                                    DegreesOfFreedom:     45
                                )
                            );

            Assert.That(certified.ToString(),
                        Is.EqualTo("(230.00 ±0.12) V, k=2, p=0.95, dist=normal, ν=45"));

            var parsed = RoundTrip(certified);

            Assert.Multiple(() => {
                Assert.That(parsed.Uncertainty!.Value.Value,                Is.EqualTo(0.12m));
                Assert.That(parsed.Uncertainty!.Value.CoverageFactor,       Is.EqualTo(2m));
                Assert.That(parsed.Uncertainty!.Value.CoverageProbability,  Is.EqualTo(0.95));
                Assert.That(parsed.Uncertainty!.Value.Distribution,         Is.EqualTo(UncertaintyDistribution.Normal));
                Assert.That(parsed.Uncertainty!.Value.DegreesOfFreedom,     Is.EqualTo(45));
            });

            // Every distribution has a name of its own.
            foreach (var distribution in Enum.GetValues<UncertaintyDistribution>())
            {

                if (distribution == UncertaintyDistribution.Unspecified)
                    continue;

                var value = new MetrologicalValue(1m, UnitOfMeasure.Volt, SIPrefix.None,
                                                  new MeasurementUncertainty(0.1m, Distribution: distribution));

                Assert.That(RoundTrip(value).Uncertainty!.Value.Distribution,
                            Is.EqualTo(distribution),
                            distribution.ToString());

            }

        }

        #endregion

        #region A_statement_without_an_uncertainty_is_refused()

        [Test]
        public void A_statement_without_an_uncertainty_is_refused()
        {

            Assert.That(MetrologicalValue.TryParse("230 V, k=2", out _, out var errorResponse),  Is.False);
            Assert.That(errorResponse,                                                           Does.Contain("k=2"));

            // ...and so is a statement this format does not know.
            Assert.That(MetrologicalValue.TryParse("(230 ±1) V, kk=2", out _, out errorResponse),  Is.False);
            Assert.That(errorResponse,                                                             Does.Contain("kk"));

            // ...and one stated twice.
            Assert.That(MetrologicalValue.TryParse("(230 ±1) V, k=2, k=3", out _, out errorResponse),  Is.False);
            Assert.That(errorResponse,                                                                 Does.Contain("twice"));

        }

        #endregion

        #region The_text_accepts_more_spellings_than_it_writes()

        [Test]
        public void The_text_accepts_more_spellings_than_it_writes()
        {

            var canonical = MetrologicalValue.Parse("(5.00 ±0.02) mA, k=2, ν=12");

            // "+-" for the plus-minus sign, "nu" for the Greek nu.
            Assert.That(MetrologicalValue.Parse("(5.00 +-0.02) mA, k=2, nu=12"),  Is.EqualTo(canonical));

            // The order of the statements does not matter on the way in.
            Assert.That(MetrologicalValue.Parse("(5.00 ±0.02) mA, nu=12, k=2"),   Is.EqualTo(canonical));

            // Whitespace around the statements is ignored.
            Assert.That(MetrologicalValue.Parse("(5.00 ±0.02) mA ,  k = 2 , ν = 12"),  Is.EqualTo(canonical));

            // Both code points of the micro sign, both of the ohm sign.
            Assert.That(MetrologicalValue.Parse("7 μF"),  Is.EqualTo(MetrologicalValue.Parse("7 µF")));
            Assert.That(MetrologicalValue.Parse("3 mΩ"),  Is.EqualTo(MetrologicalValue.Parse("3 mΩ")));

            // The ASCII asterisk separates the factors of a product just as
            // the middle dot does.
            Assert.That(MetrologicalValue.Parse("9.81 m*s^-2"),  Is.EqualTo(MetrologicalValue.Parse("9.81 m·s^-2")));

            // Scientific notation is accepted and normalised away.
            Assert.That(MetrologicalValue.Parse("4.5e-9 V").ToString(),  Is.EqualTo("0.0000000045 V"));

            // ...and every one of them writes the canonical form back.
            Assert.That(canonical.ToString(),  Is.EqualTo("(5.00 ±0.02) mA, k=2, ν=12"));

        }

        #endregion

        #region A_text_without_a_unit_of_measure_is_refused()

        [Test]
        public void A_text_without_a_unit_of_measure_is_refused()
        {

            // A metrological value always states its unit - which is what keeps
            // the document conversion from reading prose as a measurement.
            foreach (var text in new [] { "5", "5 ", "-17.3", "(5 ±0.5)", "" })
            {
                Assert.That(MetrologicalValue.TryParse(text, out _, out _),
                            Is.False,
                            $"'{text}' was accepted as a metrological value!");
            }

            Assert.That(MetrologicalValue.TryParse((String?) null, out _, out _),  Is.False);

        }

        #endregion

        #region Accepted_alternate_spellings_parse_to_the_canonical_bytes()

        [Test]
        public void Accepted_alternate_spellings_parse_to_the_canonical_bytes()
        {

            // Superscript exponents, the superscript scale, "x" for "×",
            // "+/-" for "±" and "student-t" next to "t" - all accepted on
            // input, none of them written.
            var vectors = new (String Text, String Hex) [] {
                ("9.81 m·s⁻²",               "D9ACDC82C482211903D582820F01820821"),
                ("9.81 m*s⁻²",               "D9ACDC82C482211903D582820F01820821"),
                ("5×10³ m²",                  "D9ACDC8305188C03"),
                ("5x10^3 m²",                 "D9ACDC8305188C03"),
                ("(5.00 +/-0.02) mA",         "D9ACDC84C482211901F40422C4822102"),
                ("(5 ±1) A, dist=student-t",  "D9ACDC84050400A201010405"),
                ("(5 ±1) A, dist=t",          "D9ACDC84050400A201010405"),
                ("5 m²",                      "D9ACDC8205188C")
            };

            foreach (var vector in vectors)
            {

                Assert.That(MetrologicalValue.TryParse(vector.Text, out var metrologicalValue, out var errorResponse),
                            Is.True,
                            $"'{vector.Text}': {errorResponse}");

                Assert.That(Convert.ToHexString(metrologicalValue.ToCBOR().ToByteArray()),
                            Is.EqualTo(vector.Hex),
                            $"'{vector.Text}'");

            }

            // The canonical spelling of Student's t is the self-describing one.
            Assert.That(MetrologicalValue.Parse("(5 ±1) A, dist=t").ToString(),
                        Is.EqualTo("(5 ±1) A, dist=student-t"));

        }

        #endregion

        #region Unit_names_are_not_symbols()

        [Test]
        public void Unit_names_are_not_symbols()
        {

            // "1 hour" is prose, "1 h" is a reading: names are English words,
            // and words must not read as measurements.
            Assert.That(MetrologicalValue.TryParse("1 hour",   out _, out _),  Is.False);
            Assert.That(MetrologicalValue.TryParse("5 Ampere", out _, out _),  Is.False);
            Assert.That(MetrologicalValue.TryParse("1 h",      out _, out _),  Is.True);

        }

        #endregion

        #region A_number_needs_digits_on_both_sides_of_the_point()

        [Test]
        public void A_number_needs_digits_on_both_sides_of_the_point()
        {

            // Decimal.TryParse would read every one of these - the grammar
            // does not: digits are required on both sides of the decimal
            // point and after the exponent marker, for the value, the
            // uncertainty and the statements alike.
            foreach (var text in new [] {
                         "5. A",
                         ".5 A",
                         "5.e1 A",
                         "5.0e A",
                         "(5. ±0.5) A",
                         "(5 ±.5) A",
                         "(5 ±0.5) A, k=.5",
                         "(5 ±0.5) A, p=.9",
                         "(5 ±0.5) A, nu=1."
                     })
            {
                Assert.That(MetrologicalValue.TryParse(text, out _, out _),
                            Is.False,
                            $"'{text}' was accepted as a metrological value!");
            }

        }

        #endregion

        #region An_invalid_text_names_what_is_wrong_with_it()

        [Test]
        public void An_invalid_text_names_what_is_wrong_with_it()
        {

            var vectors = new (String Text, String Reason) [] {
                ("5 Foo",              "Foo"),
                ("five A",             "five"),
                ("(5 ±0.5 A",          "closed"),
                ("(5 0.5) A",          "±"),
                ("(5 ±-0.5) A",        "negative"),
                ("(5 ±0.5) A, p=2",    "coverage probability"),
                ("(5 ±0.5) A, k=0",    "coverage factor"),
                ("(5 ±0.5) A, dist=x", "distribution"),
                ("(5 ±0.5) A, ν=0",    "degrees of freedom"),
                ("(5 ±0.5) A, k",      "key=value")
            };

            foreach (var vector in vectors)
            {

                Assert.That(MetrologicalValue.TryParse(vector.Text, out _, out var errorResponse),
                            Is.False,
                            $"'{vector.Text}' was accepted!");

                Assert.That(errorResponse,
                            Does.Contain(vector.Reason),
                            $"'{vector.Text}' -> {errorResponse}");

            }

        }

        #endregion

        #region The_text_format_is_culture_invariant()

        [Test]
        public void The_text_format_is_culture_invariant()
        {

            // The CI runs on Windows AND within a debian:13 container, and a
            // German decimal comma would collide with the statement separator!
            var previousCulture = CultureInfo.CurrentCulture;

            try
            {

                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                Assert.That(new MetrologicalValue(1.10m, UnitOfMeasure.WattHour, SIPrefix.Kilo).ToString(),
                            Is.EqualTo("1.10 kWh"));

                Assert.That(new MetrologicalValue(230.00m, UnitOfMeasure.Volt, SIPrefix.None,
                                                  new MeasurementUncertainty(0.12m, CoverageFactor: 2, CoverageProbability: 0.95)).ToString(),
                            Is.EqualTo("(230.00 ±0.12) V, k=2, p=0.95"));

                Assert.That(MetrologicalValue.Parse("1.10 kWh").Value,
                            Is.EqualTo(1.10m));

            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }

        }

        #endregion

        #region Every_prefix_and_every_registered_unit_round_trips()

        [Test]
        public void Every_prefix_and_every_registered_unit_round_trips()
        {

            var refused = new List<String>();

            foreach (var unit in UnitOfMeasure.All)
            {
                foreach (var prefix in SIPrefix.All)
                {

                    var value  = new MetrologicalValue(1.25m, unit, prefix, 0.05m);
                    var text   = value.ToString();

                    if (!MetrologicalValue.TryParse(text, out var parsed, out var errorResponse))
                        refused.Add($"{text} ({errorResponse})");

                    else if (parsed != value)
                        refused.Add($"{text} came back as {parsed}");

                }
            }

            Assert.That(refused,
                        Is.Empty,
                        $"{refused.Count} of {UnitOfMeasure.All.Count() * SIPrefix.All.Count} renderings do not read back: {String.Join(", ", refused.Take(10))}");

        }

        #endregion

    }

}
