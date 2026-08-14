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
    /// Tests for the unit of measure registry.
    /// </summary>
    [TestFixture]
    public class UnitOfMeasureTests
    {

        #region Published_numeric_ids_are_stable()

        [Test]
        public void Published_numeric_ids_are_stable()
        {

            // The numeric identifications are part of the CBOR wire format
            // (tag 44252) and must therefore NEVER be renumbered!
            var expectedIds = new (UnitOfMeasure Unit, UInt16 Numeric, String Symbol)[] {

                                  (UnitOfMeasure.Second,                   1,  "s"),
                                  (UnitOfMeasure.Meter,                    2,  "m"),
                                  (UnitOfMeasure.Gram,                     3,  "g"),
                                  (UnitOfMeasure.Ampere,                   4,  "A"),
                                  (UnitOfMeasure.Kelvin,                   5,  "K"),
                                  (UnitOfMeasure.Mole,                     6,  "mol"),
                                  (UnitOfMeasure.Candela,                  7,  "cd"),

                                  (UnitOfMeasure.Hertz,                    8,  "Hz"),
                                  (UnitOfMeasure.Newton,                   9,  "N"),
                                  (UnitOfMeasure.Pascal,                  10,  "Pa"),
                                  (UnitOfMeasure.Joule,                   11,  "J"),
                                  (UnitOfMeasure.Watt,                    12,  "W"),
                                  (UnitOfMeasure.Coulomb,                 13,  "C"),
                                  (UnitOfMeasure.Volt,                    14,  "V"),
                                  (UnitOfMeasure.Farad,                   15,  "F"),
                                  (UnitOfMeasure.Ohm,                     16,  "Ω"),
                                  (UnitOfMeasure.Siemens,                 17,  "S"),
                                  (UnitOfMeasure.Weber,                   18,  "Wb"),
                                  (UnitOfMeasure.Tesla,                   19,  "T"),
                                  (UnitOfMeasure.Henry,                   20,  "H"),
                                  (UnitOfMeasure.Celsius,                 21,  "°C"),
                                  (UnitOfMeasure.Lumen,                   22,  "lm"),
                                  (UnitOfMeasure.Lux,                     23,  "lx"),
                                  (UnitOfMeasure.Becquerel,               24,  "Bq"),
                                  (UnitOfMeasure.Gray,                    25,  "Gy"),
                                  (UnitOfMeasure.Sievert,                 26,  "Sv"),
                                  (UnitOfMeasure.Katal,                   27,  "kat"),
                                  (UnitOfMeasure.Radian,                  28,  "rad"),
                                  (UnitOfMeasure.Steradian,               29,  "sr"),

                                  (UnitOfMeasure.Minute,                  30,  "min"),
                                  (UnitOfMeasure.Hour,                    31,  "h"),
                                  (UnitOfMeasure.Day,                     32,  "d"),
                                  (UnitOfMeasure.Degree,                  33,  "°"),
                                  (UnitOfMeasure.Litre,                   34,  "l"),
                                  (UnitOfMeasure.Tonne,                   35,  "t"),
                                  (UnitOfMeasure.Percent,                 36,  "%"),
                                  (UnitOfMeasure.Permille,                37,  "‰"),
                                  (UnitOfMeasure.PartsPerMillion,         38,  "ppm"),

                                  (UnitOfMeasure.WattHour,                50,  "Wh"),
                                  (UnitOfMeasure.VoltAmpere,              51,  "VA"),
                                  (UnitOfMeasure.VoltAmpereReactive,      52,  "var"),
                                  (UnitOfMeasure.VoltAmpereReactiveHour,  53,  "varh"),
                                  (UnitOfMeasure.AmpereHour,              54,  "Ah"),

                                  (UnitOfMeasure.Bit,                     70,  "bit"),
                                  (UnitOfMeasure.Byte,                    71,  "B"),
                                  (UnitOfMeasure.BitPerSecond,            72,  "bit/s"),
                                  (UnitOfMeasure.BytePerSecond,           73,  "B/s"),

                                  (UnitOfMeasure.SquareMeter,             90,  "m²"),
                                  (UnitOfMeasure.CubicMeter,              91,  "m³")

                              };

            foreach (var expected in expectedIds)
            {
                Assert.That(expected.Unit.Numeric,  Is.EqualTo(expected.Numeric),  expected.Unit.Name);
                Assert.That(expected.Unit.Symbol,   Is.EqualTo(expected.Symbol),   expected.Unit.Name);
            }

        }

        #endregion

        #region TryParse_resolves_symbols_aliases_and_names()

        [Test]
        public void TryParse_resolves_symbols_aliases_and_names()
        {

            // Symbols are case-sensitive...
            Assert.That(UnitOfMeasure.Parse("A"),          Is.EqualTo(UnitOfMeasure.Ampere));
            Assert.That(UnitOfMeasure.Parse("t"),          Is.EqualTo(UnitOfMeasure.Tonne));
            Assert.That(UnitOfMeasure.Parse("T"),          Is.EqualTo(UnitOfMeasure.Tesla));
            Assert.That(UnitOfMeasure.Parse("h"),          Is.EqualTo(UnitOfMeasure.Hour));
            Assert.That(UnitOfMeasure.Parse("H"),          Is.EqualTo(UnitOfMeasure.Henry));
            Assert.That(UnitOfMeasure.Parse("Wh"),         Is.EqualTo(UnitOfMeasure.WattHour));

            // Aliases...
            Assert.That(UnitOfMeasure.Parse("Ω"),      Is.EqualTo(UnitOfMeasure.Ohm));
            Assert.That(UnitOfMeasure.Parse("Ohm"),        Is.EqualTo(UnitOfMeasure.Ohm));
            Assert.That(UnitOfMeasure.Parse("Cel"),        Is.EqualTo(UnitOfMeasure.Celsius));
            Assert.That(UnitOfMeasure.Parse("L"),          Is.EqualTo(UnitOfMeasure.Litre));
            Assert.That(UnitOfMeasure.Parse("m2"),         Is.EqualTo(UnitOfMeasure.SquareMeter));

            // Names are case-insensitive...
            Assert.That(UnitOfMeasure.Parse("ampere"),     Is.EqualTo(UnitOfMeasure.Ampere));
            Assert.That(UnitOfMeasure.Parse("WATTHOUR"),   Is.EqualTo(UnitOfMeasure.WattHour));
            Assert.That(UnitOfMeasure.Parse("celsius"),    Is.EqualTo(UnitOfMeasure.Celsius));

            // Numeric identifications...
            Assert.That(UnitOfMeasure.TryParse((UInt16) 12, out var watt),  Is.True);
            Assert.That(watt,                              Is.EqualTo(UnitOfMeasure.Watt));

            Assert.That(UnitOfMeasure.TryParse((UInt16) 12345, out _),      Is.False);

            // Unknown texts...
            Assert.That(UnitOfMeasure.TryParse("wattsecond", out _),        Is.False);
            Assert.That(UnitOfMeasure.TryParse("",           out _),        Is.False);
            Assert.That(UnitOfMeasure.TryParse("wh",         out _),        Is.False);

        }

        #endregion

        #region Register_rejects_conflicts_and_supports_private_units()

        [Test]
        public void Register_rejects_conflicts_and_supports_private_units()
        {

            // A conflicting numeric identification...
            Assert.That(UnitOfMeasure.TryRegister("SomethingNew", "xx1", 4, out _),         Is.False);

            // A conflicting symbol...
            Assert.That(UnitOfMeasure.TryRegister("SomethingNew", "A", 40001, out _),       Is.False);

            // A conflicting alias...
            Assert.That(UnitOfMeasure.TryRegister("SomethingNew", "xx2", 40002, out _, "Ohm"),  Is.False);

            // ...and the failed registrations did not leak anything!
            Assert.That(UnitOfMeasure.TryParse((UInt16) 40001, out _),                      Is.False);
            Assert.That(UnitOfMeasure.TryParse("xx1", out _),                               Is.False);
            Assert.That(UnitOfMeasure.TryParse("xx2", out _),                               Is.False);

            // A private unit within the user range (>= 32768)...
            var registered = UnitOfMeasure.TryRegister("Franklin", "Fr", 40100, out var franklin);

            Assert.That(registered,            Is.True);
            Assert.That(franklin,              Is.Not.Null);
            Assert.That(franklin!.Numeric,     Is.EqualTo(40100));

            Assert.That(UnitOfMeasure.Parse("Fr"),                          Is.EqualTo(franklin));
            Assert.That(UnitOfMeasure.TryParse((UInt16) 40100, out var byId),  Is.True);
            Assert.That(byId,                                               Is.EqualTo(franklin));

            // Re-registering the same unit fails...
            Assert.That(UnitOfMeasure.TryRegister("Franklin", "Fr", 40100, out _),          Is.False);

        }

        #endregion

        #region Concurrent_registrations_of_the_same_numeric_id_yield_exactly_one_winner()

        [Test]
        public void Concurrent_registrations_of_the_same_numeric_id_yield_exactly_one_winner()
        {

            var successCount = 0;

            Parallel.For(0, 16, i => {

                if (UnitOfMeasure.TryRegister($"ConcurrentUnit{i}",
                                              $"cu{i}",
                                              41999,
                                              out _))
                {
                    Interlocked.Increment(ref successCount);
                }

            });

            Assert.That(successCount,  Is.EqualTo(1));

        }

        #endregion

    }

}
