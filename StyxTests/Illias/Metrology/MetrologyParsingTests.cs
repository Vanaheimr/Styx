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
    /// Tests for the factories and the text parsing of the metrology structs.
    /// SI prefixes are case-sensitive: 'm' is milli, 'M' is mega - confusing
    /// them is an error of nine orders of magnitude.
    /// </summary>
    [TestFixture]
    public class MetrologyParsingTests
    {

        #region Factories_apply_the_documented_decimal_prefixes()

        [Test]
        public void Factories_apply_the_documented_decimal_prefixes()
        {

            // Watt...
            Assert.That(Watt.FromW (5).Value,                Is.EqualTo(5m));
            Assert.That(Watt.FromKW(5).Value,                Is.EqualTo(5_000m));
            Assert.That(Watt.FromMW(5).Value,                Is.EqualTo(5_000_000m));
            Assert.That(Watt.FromGW(5).Value,                Is.EqualTo(5_000_000_000m));

            // WattHour...
            Assert.That(WattHour.FromWh (5).Value,           Is.EqualTo(5m));
            Assert.That(WattHour.FromKWh(5).Value,           Is.EqualTo(5_000m));
            Assert.That(WattHour.FromMWh(5).Value,           Is.EqualTo(5_000_000m));

            // Ampere, Volt...
            Assert.That(Ampere.FromA (5).Value,              Is.EqualTo(5m));
            Assert.That(Ampere.FromKA(5).Value,              Is.EqualTo(5_000m));
            Assert.That(Volt.FromV   (5).Value,              Is.EqualTo(5m));
            Assert.That(Volt.FromKV  (5).Value,              Is.EqualTo(5_000m));

            // Hertz...
            Assert.That(Hertz.FromHz (5).Value,              Is.EqualTo(5m));
            Assert.That(Hertz.FromKHz(5).Value,              Is.EqualTo(5_000m));
            Assert.That(Hertz.FromMHz(5).Value,              Is.EqualTo(5_000_000m));
            Assert.That(Hertz.FromGHz(5).Value,              Is.EqualTo(5_000_000_000m));

            // Meter - the value property is 'm'...
            Assert.That(Meter.From_mm(5).m,                  Is.EqualTo(0.005m));
            Assert.That(Meter.From_cm(5).m,                  Is.EqualTo(0.05m));
            Assert.That(Meter.From_dm(5).m,                  Is.EqualTo(0.5m));
            Assert.That(Meter.From_m (5).m,                  Is.EqualTo(5m));
            Assert.That(Meter.From_km(5).m,                  Is.EqualTo(5_000m));

            // Note: The Ohm factories are named with the Unicode-deprecated
            // OHM SIGN (U+2126), which is hard to write reliably in source.
            // They are covered through the text parser below, which spells
            // its suffixes in ASCII ("mOhm", "MOhm", ...).

        }

        #endregion

        #region Kilogram_counts_kilograms_not_grams()

        [Test]
        public void Kilogram_counts_kilograms_not_grams()
        {

            // The struct stores kilograms - see its constructor and the .g property.
            Assert.That(Kilogram.FromKG(5).Value,            Is.EqualTo(5m));
            Assert.That(Kilogram.FromKG(5).g,                Is.EqualTo(5_000m));

            Assert.That(Kilogram.FromG(5_000).Value,         Is.EqualTo(5m));
            Assert.That(Kilogram.FromG(5_000).g,             Is.EqualTo(5_000m));

            Assert.That(Kilogram.TryFromKG(5, out var fromKG),      Is.True);
            Assert.That(fromKG.Value,                        Is.EqualTo(5m));

            Assert.That(Kilogram.TryFromG(5_000, out var fromG),    Is.True);
            Assert.That(fromG.Value,                         Is.EqualTo(5m));

            // A tonne is a thousand kilograms...
            Assert.That(Tonne.FromT(2).ToKilogram().Value,   Is.EqualTo(2_000m));

        }

        #endregion

        #region Milli_is_never_parsed_as_mega()

        [Test]
        public void Milli_is_never_parsed_as_mega()
        {

            // 'm' (milli) and 'M' (mega) differ by a factor of 10^9. A parser
            // that confuses them silently turns 5 milliwatt into 5 megawatt.

            Assert.That(Watt.Parse("5 MW").Value,             Is.EqualTo(5_000_000m));
            Assert.That(Watt.TryParse("5 mW", out var mW),    Is.False,  $"'5 mW' must not be accepted as {mW}!");

            Assert.That(WattHour.Parse("5 MWh").Value,        Is.EqualTo(5_000_000m));
            Assert.That(WattHour.TryParse("5 mWh", out var mWh),  Is.False,  $"'5 mWh' must not be accepted as {mWh}!");

            Assert.That(Hertz.Parse("5 MHz").Value,           Is.EqualTo(5_000_000m));
            Assert.That(Hertz.TryParse("5 mHz", out var mHz), Is.False,  $"'5 mHz' must not be accepted as {mHz}!");

            // Meter has no megameter, so 'Mm' must not fall back to millimeter...
            Assert.That(Meter.TryParse("5 Mm", out var megaMeter),  Is.False,  $"'5 Mm' must not be accepted as {megaMeter}!");

        }

        #endregion

        #region Ohm_distinguishes_milliohm_from_megaohm()

        [Test]
        public void Ohm_distinguishes_milliohm_from_megaohm()
        {

            // Ohm is the only struct offering both prefixes in its text parser,
            // so a case-insensitive comparison makes one of them unreachable.
            Assert.That(Ohm.Parse("5 mOhm").Value,            Is.EqualTo(0.005m));
            Assert.That(Ohm.Parse("5 MOhm").Value,            Is.EqualTo(5_000_000m));

            Assert.That(Ohm.Parse("5 µOhm").Value,       Is.EqualTo(0.000005m));
            Assert.That(Ohm.Parse("5 kOhm").Value,            Is.EqualTo(5_000m));
            Assert.That(Ohm.Parse("5 GOhm").Value,            Is.EqualTo(5_000_000_000m));
            Assert.That(Ohm.Parse("5 Ohm").Value,             Is.EqualTo(5m));

        }

        #endregion

        #region Text_parsing_agrees_with_the_factories()

        [Test]
        public void Text_parsing_agrees_with_the_factories()
        {

            Assert.That(Watt.Parse    ("5 W").Value,          Is.EqualTo(Watt.FromW      (5).Value));
            Assert.That(Watt.Parse    ("5 kW").Value,         Is.EqualTo(Watt.FromKW     (5).Value));
            Assert.That(Watt.Parse    ("5 GW").Value,         Is.EqualTo(Watt.FromGW     (5).Value));
            Assert.That(WattHour.Parse("5 kWh").Value,        Is.EqualTo(WattHour.FromKWh(5).Value));
            Assert.That(Ampere.Parse  ("5 kA").Value,         Is.EqualTo(Ampere.FromKA   (5).Value));
            Assert.That(Volt.Parse    ("5 kV").Value,         Is.EqualTo(Volt.FromKV     (5).Value));
            Assert.That(Hertz.Parse   ("5 kHz").Value,        Is.EqualTo(Hertz.FromKHz   (5).Value));
            Assert.That(Meter.Parse   ("5 km").m,             Is.EqualTo(Meter.From_km   (5).m));
            Assert.That(Meter.Parse   ("5 mm").m,             Is.EqualTo(Meter.From_mm   (5).m));
            Assert.That(Kilogram.Parse("5 kg").Value,         Is.EqualTo(Kilogram.FromKG (5).Value));

            // Plain numbers without a unit symbol stay valid...
            Assert.That(Watt.Parse("5").Value,                Is.EqualTo(5m));
            Assert.That(Meter.Parse("500").m,                 Is.EqualTo(500m));

            // Decimal values keep their scale...
            Assert.That(WattHour.Parse("1.10 kWh").Value,     Is.EqualTo(1100m));

        }

        #endregion

        #region Both_omega_code_points_resolve_to_the_ohm()

        [Test]
        public void Both_omega_code_points_resolve_to_the_ohm()
        {

            // U+03A9 GREEK CAPITAL LETTER OMEGA is the canonical symbol,
            // U+2126 OHM SIGN is visually identical, deprecated by Unicode
            // and still used by the Ohm struct of this namespace. Both must
            // resolve, otherwise a symbolic CBOR unit fails depending on
            // which of the two an external source happened to send.
            Assert.That(UnitOfMeasure.TryParse("\u03A9", out var canonical),   Is.True);
            Assert.That(canonical,                                             Is.EqualTo(UnitOfMeasure.Ohm));

            Assert.That(UnitOfMeasure.TryParse("\u2126", out var ohmSign),     Is.True);
            Assert.That(ohmSign,                                               Is.EqualTo(UnitOfMeasure.Ohm));

            // The canonical code point is what gets written...
            Assert.That(UnitOfMeasure.Ohm.Symbol,                              Is.EqualTo("\u03A9"));

        }

        #endregion

        #region Parsing_is_culture_invariant()

        [Test]
        public void Parsing_is_culture_invariant()
        {

            // The CI runs on Windows AND within a debian:13 container...
            var previousCulture = CultureInfo.CurrentCulture;

            try
            {

                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                Assert.That(Watt.Parse("1.5 kW", CultureInfo.InvariantCulture).Value,
                            Is.EqualTo(1_500m));

                Assert.That(Meter.Parse("2.25 km", CultureInfo.InvariantCulture).m,
                            Is.EqualTo(2_250m));

            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }

        }

        #endregion

    }

}
