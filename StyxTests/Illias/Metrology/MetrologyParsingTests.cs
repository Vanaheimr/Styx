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

        #region Format_specifiers_never_substitute_a_different_prefix()

        [Test]
        public void Format_specifiers_never_substitute_a_different_prefix()
        {

            var fiveKilowatt = Watt.FromW(5000);

            // The requested unit is the one that gets printed...
            Assert.That(fiveKilowatt.ToString(),        Is.EqualTo("5000 W"));
            Assert.That(fiveKilowatt.ToString("W", CultureInfo.InvariantCulture),     Is.EqualTo("5000 W"));
            Assert.That(fiveKilowatt.ToString("kW", CultureInfo.InvariantCulture),    Is.EqualTo("5 kW"));
            Assert.That(fiveKilowatt.ToString("MW", CultureInfo.InvariantCulture),    Is.EqualTo("0.005 MW"));

            // ...and asking for milliwatts must never yield megawatts. There is
            // no milliwatt specifier, so "mW" falls through to the numeric
            // format fallback - the same fallback that makes ToString("0.00")
            // work - and is echoed instead of silently switching the prefix.
            Assert.That(fiveKilowatt.ToString("mW",   CultureInfo.InvariantCulture),
                        Is.Not.EqualTo(fiveKilowatt.ToString("MW", CultureInfo.InvariantCulture)));

            Assert.That(fiveKilowatt.ToString("0.00", CultureInfo.InvariantCulture),
                        Is.EqualTo("5000.00 W"));

            Assert.That(WattHour.FromWh(5000).ToString("mWh", CultureInfo.InvariantCulture),
                        Is.Not.EqualTo(WattHour.FromWh(5000).ToString("MWh", CultureInfo.InvariantCulture)));

            Assert.That(Hertz.FromHz(5000).ToString("mHz", CultureInfo.InvariantCulture),
                        Is.Not.EqualTo(Hertz.FromHz(5000).ToString("MHz", CultureInfo.InvariantCulture)));

            Assert.That(WattHour.FromWh(5000).ToString("kWh", CultureInfo.InvariantCulture),    Is.EqualTo("5 kWh"));
            Assert.That(Hertz.   FromHz(5000).ToString("kHz", CultureInfo.InvariantCulture),    Is.EqualTo("5 kHz"));

        }

        #endregion

        #region The_general_format_specifier_stays_case_insensitive()

        [Test]
        public void The_general_format_specifier_stays_case_insensitive()
        {

            // "G" is the standard general specifier of .NET, not a giga prefix,
            // and therefore keeps accepting "g" - unlike the unit symbols.
            var fiveWatt = Watt.FromW(5);

            Assert.That(fiveWatt.ToString("G", CultureInfo.InvariantCulture),         Is.EqualTo(fiveWatt.ToString()));
            Assert.That(fiveWatt.ToString("g", CultureInfo.InvariantCulture),         Is.EqualTo(fiveWatt.ToString()));

            // ...while "GW" remains the giga prefix and is case-sensitive!
            Assert.That(Watt.FromW(5_000_000_000m).ToString("GW", CultureInfo.InvariantCulture),  Is.EqualTo("5 GW"));

            Assert.That(Watt.FromW(5_000_000_000m).ToString("gw", CultureInfo.InvariantCulture),
                        Is.Not.EqualTo(Watt.FromW(5_000_000_000m).ToString("GW", CultureInfo.InvariantCulture)));

        }

        #endregion

        #region BytePerSecond_labels_bytes_as_bytes()

        [Test]
        public void BytePerSecond_labels_bytes_as_bytes()
        {

            // The string formatting path used to accept "kBit/s" and print a
            // BYTE rate under a BIT label - an eightfold misstatement.
            var fiveKiloBytesPerSecond = BytePerSecond.FromBPS(5000);

            Assert.That(fiveKiloBytesPerSecond.ToString("kByte/s", CultureInfo.InvariantCulture),   Is.EqualTo("5 kByte/s"));
            Assert.That(fiveKiloBytesPerSecond.ToString("kB/s", CultureInfo.InvariantCulture),      Is.EqualTo("5 kB/s"));
            Assert.That(fiveKiloBytesPerSecond.ToString("kBps", CultureInfo.InvariantCulture),      Is.EqualTo("5 kBps"));

            Assert.That(fiveKiloBytesPerSecond.ToString("kBit/s", CultureInfo.InvariantCulture),
                        Is.Not.EqualTo("5 kBit/s"),
                        "A byte rate must never be labelled as a bit rate!");

            // The bit rate keeps its own spelling...
            Assert.That(BitPerSecond.FromBPS(5000).ToString("kbit/s", CultureInfo.InvariantCulture),  Is.EqualTo("5 kbit/s"));

        }

        #endregion

        #region Kilotonne_accepts_both_spellings_and_prints_the_SI_one()

        [Test]
        public void Kilotonne_accepts_both_spellings_and_prints_the_SI_one()
        {

            // "kt" is the SI spelling; "kT" is what this library used to emit
            // and stays acceptable as input.
            Assert.That(Tonne.Parse("5 kt").Value,       Is.EqualTo(5_000m));
            Assert.That(Tonne.Parse("5 kT").Value,       Is.EqualTo(5_000m));
            Assert.That(Tonne.Parse("5 t").Value,        Is.EqualTo(5m));

            Assert.That(Tonne.FromT(5000).ToString("kt", CultureInfo.InvariantCulture),  Is.EqualTo("5 kt"));
            Assert.That(Tonne.FromT(5000).ToString("kT", CultureInfo.InvariantCulture),  Is.EqualTo("5 kt"));

            // What we print can be read back...
            Assert.That(Tonne.Parse(Tonne.FromT(5000).ToString("kt", CultureInfo.InvariantCulture)).Value,
                        Is.EqualTo(5_000m));

        }

        #endregion

        #region Capacitance_and_inductance_keep_their_small_prefixes_apart()

        [Test]
        public void Capacitance_and_inductance_keep_their_small_prefixes_apart()
        {

            // Farad and Henry were the only structs that always compared their
            // suffixes case-sensitively, which is why their milli/micro/nano/
            // pico prefixes never collided. Pin that.

            Assert.That(Farad.FromF (5).Value,                   Is.EqualTo(5m));
            Assert.That(Farad.FromµF(5).Value,               Is.EqualTo(0.000005m));
            Assert.That(Farad.FromNF(5).Value,                   Is.EqualTo(0.000000005m));
            Assert.That(Farad.FromPF(5).Value,                   Is.EqualTo(0.000000000005m));

            Assert.That(Farad.Parse("5 F").Value,                Is.EqualTo(5m));
            Assert.That(Farad.Parse("5 nF").Value,               Is.EqualTo(0.000000005m));
            Assert.That(Farad.Parse("5 pF").Value,               Is.EqualTo(0.000000000005m));

            // 'nF' is nano, 'NF' is nothing at all...
            Assert.That(Farad.TryParse("5 NF", out var NF),      Is.False,  $"'5 NF' must not be accepted as {NF}!");

            Assert.That(Henry.FromH (5).Value,                   Is.EqualTo(5m));
            Assert.That(Henry.FromKH(5).Value,                   Is.EqualTo(5_000m));
            Assert.That(Henry.FromMH(5).Value,                   Is.EqualTo(0.005m));       // milli, not mega!
            Assert.That(Henry.FromNH(5).Value,                   Is.EqualTo(0.000000005m));

            Assert.That(Henry.Parse("5 kH").Value,               Is.EqualTo(5_000m));
            Assert.That(Henry.Parse("5 mH").Value,               Is.EqualTo(0.005m));
            Assert.That(Henry.Parse("5 µH").Value,          Is.EqualTo(0.000005m));

            // 'MH' would be mega, which this struct does not offer - and it
            // must not silently fall back to millihenry.
            Assert.That(Henry.TryParse("5 MH", out var MH),      Is.False,  $"'5 MH' must not be accepted as {MH}!");

            // The conversion properties are the inverse of the factories...
            Assert.That(Henry.FromMH(5).mH,                      Is.EqualTo(5m));
            Assert.That(Farad.FromPF(5).pF,                      Is.EqualTo(5m));

        }

        #endregion

        #region Absolute_and_relative_temperatures_and_conductance()

        [Test]
        public void Absolute_and_relative_temperatures_and_conductance()
        {

            Assert.That(Kelvin.FromK(273.15m).Value,             Is.EqualTo(273.15m));
            Assert.That(Kelvin.Parse("273.15 K").Value,          Is.EqualTo(273.15m));

            Assert.That(Celsius.FromC(21.5m).Value,              Is.EqualTo(21.5m));
            Assert.That(Celsius.Parse("21.5 °C").Value,          Is.EqualTo(21.5m));

            Assert.That(Siemens.FromS (5).Value,                 Is.EqualTo(5m));
            Assert.That(Siemens.FromKS(5).Value,                 Is.EqualTo(5_000m));
            Assert.That(Siemens.Parse("5 kS").Value,             Is.EqualTo(5_000m));
            Assert.That(Siemens.FromKS(5).kS,                    Is.EqualTo(5m));

            // Reactive power is measured in var, not in VA...
            Assert.That(VoltAmpereReactive.FromKVAr(5).Value,    Is.EqualTo(5_000m));
            Assert.That(VoltAmpereReactive.FromKVAr(5).kVAr,     Is.EqualTo(5m));
            Assert.That(VoltAmpereReactive.Parse("5 kVAr").Value, Is.EqualTo(5_000m));
            Assert.That(VoltAmpere.FromKVA(5).kVA,               Is.EqualTo(5m));

        }

        #endregion

        #region Data_rates_never_confuse_bits_with_bytes()

        [Test]
        public void Data_rates_never_confuse_bits_with_bytes()
        {

            // A bit rate and a byte rate differ by a factor of eight, and
            // their symbols differ only in the case of a single letter:
            // "Mbps" is megabits, "MBps" is megabytes.

            Assert.That(BitPerSecond.FromBPS  (5).Value,         Is.EqualTo(5m));
            Assert.That(BitPerSecond.FromKBPS (5).Value,         Is.EqualTo(5_000m));
            Assert.That(BitPerSecond.FromMBPS (5).Value,         Is.EqualTo(5_000_000m));
            Assert.That(BitPerSecond.FromGBPS (5).Value,         Is.EqualTo(5_000_000_000m));
            Assert.That(BitPerSecond.FromTBPS (5).Value,         Is.EqualTo(5_000_000_000_000m));

            Assert.That(BytePerSecond.FromKBPS(5).Value,         Is.EqualTo(5_000m));
            Assert.That(BytePerSecond.FromTBPS(5).Value,         Is.EqualTo(5_000_000_000_000m));

            Assert.That(BitPerSecond. Parse("5 kbit/s").Value,   Is.EqualTo(5_000m));
            Assert.That(BitPerSecond. Parse("5 Mbps").Value,     Is.EqualTo(5_000_000m));
            Assert.That(BytePerSecond.Parse("5 kByte/s").Value,  Is.EqualTo(5_000m));
            Assert.That(BytePerSecond.Parse("5 MBps").Value,     Is.EqualTo(5_000_000m));

            // The lower case byte spelling denotes bits and must not be
            // accepted by the byte rate...
            Assert.That(BytePerSecond.TryParse("5 Mbps", out var bits),
                        Is.False,  $"'5 Mbps' is a bit rate and must not be read as {bits}!");

            // ...and vice versa.
            Assert.That(BitPerSecond.TryParse("5 MBps", out var bytes),
                        Is.False,  $"'5 MBps' is a byte rate and must not be read as {bytes}!");

            Assert.That(BitPerSecond. FromMBPS(5).Mbps,          Is.EqualTo(5m));
            Assert.That(BytePerSecond.FromMBPS(5).MBps,          Is.EqualTo(5m));

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
