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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Tests for compound units of measure and the full GUM uncertainty model
    /// of CBOR tag 44252.
    /// </summary>
    [TestFixture]
    public class CompoundUnitTests
    {

        #region Derived_units_are_expressible()

        [Test]
        public void Derived_units_are_expressible()
        {

            // An acceleration: m·s^-2
            var acceleration = new UnitExpression(
                                   new UnitFactor(UnitOfMeasure.Meter,   1),
                                   new UnitFactor(UnitOfMeasure.Second, -2)
                               );

            Assert.That(acceleration.ToString(),          Is.EqualTo("m·s^-2"));
            Assert.That(acceleration.IsSimple,            Is.False);
            Assert.That(acceleration.Factors.Count,       Is.EqualTo(2));

            // A single named unit converts implicitly and stays simple...
            UnitExpression watt = UnitOfMeasure.Watt;
            Assert.That(watt.IsSimple,                    Is.True);
            Assert.That(watt.SingleUnit,                  Is.EqualTo(UnitOfMeasure.Watt));
            Assert.That(watt.ToString(),                  Is.EqualTo("W"));

            // A heat transfer coefficient: W·m^-2·K^-1
            var heatTransfer = new UnitExpression(
                                   new UnitFactor(UnitOfMeasure.Watt,    1),
                                   new UnitFactor(UnitOfMeasure.Meter,  -2),
                                   new UnitFactor(UnitOfMeasure.Kelvin, -1)
                               );

            Assert.That(heatTransfer.ToString(),          Is.EqualTo("W·m^-2·K^-1"));

        }

        #endregion

        #region Fractional_exponents_carry_noise_densities()

        [Test]
        public void Fractional_exponents_carry_noise_densities()
        {

            // Voltage noise density is stated per square root of hertz:
            // V/√Hz = V·Hz^-1/2. This is the case integer exponents can not
            // express, and it is everyday electrical metrology.
            var noiseDensity = new UnitExpression(
                                   new UnitFactor(UnitOfMeasure.Volt,  1),
                                   new UnitFactor(UnitOfMeasure.Hertz, -1, 2)
                               );

            Assert.That(noiseDensity.ToString(),          Is.EqualTo("V·Hz^-1/2"));

            var factor = noiseDensity.Factors[1];
            Assert.That(factor.IsInteger,                 Is.False);
            Assert.That(factor.Numerator,                 Is.EqualTo(-1));
            Assert.That(factor.Denominator,               Is.EqualTo(2));
            Assert.That(factor.Exponent,                  Is.EqualTo(-0.5));

            // Exponents are reduced to lowest terms, so -2/4 IS -1/2...
            Assert.That(new UnitFactor(UnitOfMeasure.Hertz, -2, 4),
                        Is.EqualTo(new UnitFactor(UnitOfMeasure.Hertz, -1, 2)));

            // ...and the sign always lives in the numerator.
            Assert.That(new UnitFactor(UnitOfMeasure.Hertz, 1, -2),
                        Is.EqualTo(new UnitFactor(UnitOfMeasure.Hertz, -1, 2)));

            // A whole number exponent stays a whole number...
            Assert.That(new UnitFactor(UnitOfMeasure.Meter, 4, 2).IsInteger,   Is.True);
            Assert.That(new UnitFactor(UnitOfMeasure.Meter, 4, 2).Numerator,   Is.EqualTo(2));

            // Zero exponents and zero denominators are meaningless...
            Assert.That(() => new UnitFactor(UnitOfMeasure.Meter, 0),      Throws.TypeOf<ArgumentException>());
            Assert.That(() => new UnitFactor(UnitOfMeasure.Meter, 1, 0),   Throws.TypeOf<ArgumentException>());

        }

        #endregion

        #region Compound_units_survive_a_CBOR_roundtrip()

        [Test]
        public void Compound_units_survive_a_CBOR_roundtrip()
        {

            var acceleration = new MetrologicalValue(
                                   9.81m,
                                   new UnitExpression(
                                       new UnitFactor(UnitOfMeasure.Meter,   1),
                                       new UnitFactor(UnitOfMeasure.Second, -2)
                                   )
                               );

            var cbor = acceleration.ToCBOR();

            Assert.That(MetrologicalValue.TryParse(cbor, out var parsed, out var errorResponse),  Is.True);
            Assert.That(errorResponse,                    Is.Null);
            Assert.That(parsed,                           Is.EqualTo(acceleration));
            Assert.That(parsed.Unit.ToString(),           Is.EqualTo("m·s^-2"));
            Assert.That(parsed.ToString(),                Is.EqualTo("9.81 m·s^-2"));

            // ...and so do fractional ones.
            var noiseDensity = new MetrologicalValue(
                                   4.5m,
                                   new UnitExpression(
                                       new UnitFactor(UnitOfMeasure.Volt,   1),
                                       new UnitFactor(UnitOfMeasure.Hertz, -1, 2)
                                   ),
                                   SIPrefix.Nano
                               );

            Assert.That(MetrologicalValue.TryParse(noiseDensity.ToCBOR(), out var parsedDensity, out _),  Is.True);
            Assert.That(parsedDensity,                    Is.EqualTo(noiseDensity));
            Assert.That(parsedDensity.ToString(),         Is.EqualTo("4.5 nV·Hz^-1/2"));

            // A simple unit must NOT grow into an array - the compact form
            // stays byte-identical to what it always was.
            var simple = new MetrologicalValue(5m, UnitOfMeasure.Ampere);
            Assert.That(Convert.ToHexString(simple.ToCBOR().ToByteArray()),  Is.EqualTo("D9ACDC820504"));

            // The golden vectors of the specification...
            Assert.That(Convert.ToHexString(acceleration.ToCBOR().ToByteArray()),
                        Is.EqualTo("D9ACDC82C482211903D582820F01820821"));

            Assert.That(Convert.ToHexString(noiseDensity.ToCBOR().ToByteArray()),
                        Is.EqualTo("D9ACDC83C48220182D82820501820982200228"));

        }

        #endregion

        #region Compound_units_survive_a_JSON_roundtrip()

        [Test]
        public void Compound_units_survive_a_JSON_roundtrip()
        {

            var acceleration = new MetrologicalValue(
                                   9.81m,
                                   new UnitExpression(
                                       new UnitFactor(UnitOfMeasure.Meter,   1),
                                       new UnitFactor(UnitOfMeasure.Second, -2)
                                   )
                               );

            var json = acceleration.ToJSON();
            Assert.That(json["unit"]?.ToString(),          Is.EqualTo("m·s^-2"));

            Assert.That(MetrologicalValue.TryParse(json, out var parsed, out var errorResponse),  Is.True);
            Assert.That(errorResponse,                     Is.Null);
            Assert.That(parsed,                            Is.EqualTo(acceleration));

            // The text form parses back, including fractional exponents
            // and an ASCII asterisk instead of the middle dot...
            Assert.That(UnitExpression.TryParse("V·Hz^-1/2", out var density),   Is.True);
            Assert.That(density.ToString(),                Is.EqualTo("V·Hz^-1/2"));

            Assert.That(UnitExpression.TryParse("m*s^-2",   out var withStar),   Is.True);
            Assert.That(withStar.ToString(),               Is.EqualTo("m·s^-2"));

            Assert.That(UnitExpression.TryParse("W",        out var simple),     Is.True);
            Assert.That(simple.IsSimple,                   Is.True);

            Assert.That(UnitExpression.TryParse("m·Xyz^2",  out _),              Is.False);
            Assert.That(UnitExpression.TryParse("m^",       out _),              Is.False);

        }

        #endregion

        #region A_calibration_certificate_keeps_its_coverage_factor()

        [Test]
        public void A_calibration_certificate_keeps_its_coverage_factor()
        {

            // Calibration certificates state an expanded uncertainty U with
            // k=2. Forcing it to k=1 would discard what the certificate says.
            var certified = new MetrologicalValue(
                                230.00m,
                                UnitOfMeasure.Volt,
                                SIPrefix.None,
                                new MeasurementUncertainty(
                                    Value:                0.12m,
                                    CoverageFactor:       2,
                                    CoverageProbability:  0.95,
                                    Distribution:         UncertaintyDistribution.Normal
                                )
                            );

            Assert.That(certified.Uncertainty!.Value.Value,               Is.EqualTo(0.12m));
            Assert.That(certified.Uncertainty!.Value.CoverageFactor,      Is.EqualTo(2m));
            Assert.That(certified.Uncertainty!.Value.StandardUncertainty, Is.EqualTo(0.06m));
            Assert.That(certified.ToString(),                             Is.EqualTo("(230.00 ±0.12 (k=2)) V"));

            Assert.That(MetrologicalValue.TryParse(certified.ToCBOR(), out var parsed, out var errorResponse),  Is.True);
            Assert.That(errorResponse,                                    Is.Null);
            Assert.That(parsed,                                           Is.EqualTo(certified));
            Assert.That(parsed.Uncertainty!.Value.CoverageProbability,    Is.EqualTo(0.95));
            Assert.That(parsed.Uncertainty!.Value.Distribution,           Is.EqualTo(UncertaintyDistribution.Normal));

            // The golden vector of the specification, with the coverage
            // factor alone - the map holds only what is actually stated.
            var plainCertificate = new MetrologicalValue(
                                       230.00m,
                                       UnitOfMeasure.Volt,
                                       SIPrefix.None,
                                       new MeasurementUncertainty(0.12m, CoverageFactor: 2)
                                   );

            Assert.That(Convert.ToHexString(plainCertificate.ToCBOR().ToByteArray()),
                        Is.EqualTo("D9ACDC84C482211959D80500A201C482210C0202"));

            // U = 0.12 with k=2 states the same spread as u = 0.06...
            var asStandard = new MetrologicalValue(230.00m, UnitOfMeasure.Volt, SIPrefix.None, 0.06m);
            Assert.That(certified.EquivalentTo(asStandard),               Is.True);

            // ...but it is not the same statement, so strict equality differs.
            Assert.That(certified.Equals(asStandard),                     Is.False);

        }

        #endregion

        #region A_plain_uncertainty_stays_a_bare_number()

        [Test]
        public void A_plain_uncertainty_stays_a_bare_number()
        {

            // The compact form must not grow: a standard uncertainty without
            // any further statement is still written as a plain number, so
            // the golden vector of the original format is unchanged.
            var value = new MetrologicalValue(5.00m, UnitOfMeasure.Ampere, SIPrefix.Milli, 0.02m);

            Assert.That(Convert.ToHexString(value.ToCBOR().ToByteArray()),
                        Is.EqualTo("D9ACDC84C482211901F40422C4822102"));

            Assert.That(value.Uncertainty!.Value.IsPlainStandardUncertainty,  Is.True);
            Assert.That(value.ToString(),                                     Is.EqualTo("(5.00 ±0.02) mA"));

            // A negative uncertainty remains impossible...
            Assert.That(() => new MeasurementUncertainty(-1m),                Throws.TypeOf<ArgumentException>());
            Assert.That(() => new MeasurementUncertainty(1m, CoverageFactor: 0),  Throws.TypeOf<ArgumentException>());

        }

        #endregion

        #region Every_metrology_struct_bridges_to_a_metrological_value()

        [Test]
        public void Every_metrology_struct_bridges_to_a_metrological_value()
        {

            // Twelve of the eighteen IMetrology structs could be converted
            // to and from a metrological value, six could not, for no reason
            // other than that nobody had written them. Their units were in
            // the registry all along.

            Assert.That(Farad.       FromF  (5).AsMetrologicalValue().Unit,  Is.EqualTo(UnitOfMeasure.Farad));
            Assert.That(Henry.       FromH  (5).AsMetrologicalValue().Unit,  Is.EqualTo(UnitOfMeasure.Henry));
            Assert.That(Siemens.     FromS  (5).AsMetrologicalValue().Unit,  Is.EqualTo(UnitOfMeasure.Siemens));
            Assert.That(BitPerSecond.FromBPS(5).AsMetrologicalValue().Unit,  Is.EqualTo(UnitOfMeasure.BitPerSecond));
            Assert.That(BytePerSecond.FromBPS(5).AsMetrologicalValue().Unit, Is.EqualTo(UnitOfMeasure.BytePerSecond));

            // ...and back again, through a prefix on the way.
            Assert.That(Henry.FromMH(5).AsMetrologicalValue(SIPrefix.Milli).TryToHenry(out var henry),  Is.True);
            Assert.That(henry.Value,                                         Is.EqualTo(0.005m));

            Assert.That(Farad.FromPF(5).AsMetrologicalValue().TryToFarad(out var farad),                Is.True);
            Assert.That(farad.Value,                                         Is.EqualTo(0.000000000005m));

            Assert.That(BitPerSecond.FromMBPS(5).AsMetrologicalValue(SIPrefix.Mega).TryToBitPerSecond(out var bits),  Is.True);
            Assert.That(bits.Value,                                          Is.EqualTo(5_000_000m));

            // The ohm carries the registry's own unit, whichever omega the
            // struct happens to spell its factories with...
            var resistance = new MetrologicalValue(50m, UnitOfMeasure.Ohm);
            Assert.That(resistance.TryToOhm(out var ohm),                    Is.True);
            Assert.That(ohm.Value,                                           Is.EqualTo(50m));
            Assert.That(ohm.AsMetrologicalValue().Unit,                      Is.EqualTo(UnitOfMeasure.Ohm));

            // A mismatched unit must not convert...
            Assert.That(new MetrologicalValue(5m, UnitOfMeasure.Watt).TryToFarad(out _),  Is.False);

        }

        #endregion

        #region Dimensionless_quantities_have_a_unit()

        [Test]
        public void Dimensionless_quantities_have_a_unit()
        {

            // Ratios, efficiencies and counts are dimensionless but still
            // need a unit - the SI calls it "one". SenML spells it "/".
            // One holds the first identification: it is the neutral element
            // of unit multiplication, and its symbol is "1".
            Assert.That(UnitOfMeasure.One.Symbol,                        Is.EqualTo("1"));
            Assert.That(UnitOfMeasure.One.Numeric,                       Is.EqualTo(1));

            // The single-byte range is spent on what e-mobility actually
            // sends, so the watt-hour - the unit of charging - is the second
            // identification and costs one byte, not two.
            Assert.That(UnitOfMeasure.WattHour.Numeric,                  Is.EqualTo(2));
            Assert.That(UnitOfMeasure.Percent. Numeric,                  Is.LessThan(24));
            Assert.That(UnitOfMeasure.Celsius. Numeric,                  Is.LessThan(24));
            Assert.That(UnitOfMeasure.Candela. Numeric,                  Is.GreaterThan(23));

            Assert.That(UnitOfMeasure.TryParse("1",   out var byOne),    Is.True);
            Assert.That(byOne,                                           Is.EqualTo(UnitOfMeasure.One));

            Assert.That(UnitOfMeasure.TryParse("/",   out var bySenML),  Is.True);
            Assert.That(bySenML,                                         Is.EqualTo(UnitOfMeasure.One));

            var efficiency = new MetrologicalValue(0.923m, UnitOfMeasure.One);
            Assert.That(MetrologicalValue.TryParse(efficiency.ToCBOR(), out var parsed, out _),  Is.True);
            Assert.That(parsed,                                          Is.EqualTo(efficiency));

        }

        #endregion

    }

}
