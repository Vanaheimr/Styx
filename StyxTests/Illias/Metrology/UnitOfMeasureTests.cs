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

                                  (UnitOfMeasure.One,                        1,  "1"),
                                  (UnitOfMeasure.WattHour,                   2,  "Wh"),
                                  (UnitOfMeasure.Watt,                       3,  "W"),
                                  (UnitOfMeasure.Ampere,                     4,  "A"),
                                  (UnitOfMeasure.Volt,                       5,  "V"),
                                  (UnitOfMeasure.Percent,                    6,  "%"),
                                  (UnitOfMeasure.Celsius,                    7,  "°C"),
                                  (UnitOfMeasure.Second,                     8,  "s"),
                                  (UnitOfMeasure.Hertz,                      9,  "Hz"),
                                  (UnitOfMeasure.VoltAmpereReactive,        10,  "var"),
                                  (UnitOfMeasure.VoltAmpere,                11,  "VA"),
                                  (UnitOfMeasure.AmpereHour,                12,  "Ah"),
                                  (UnitOfMeasure.VoltAmpereReactiveHour,    13,  "varh"),
                                  (UnitOfMeasure.Ohm,                       14,  "Ω"),
                                  (UnitOfMeasure.Meter,                     15,  "m"),
                                  (UnitOfMeasure.Gram,                      16,  "g"),
                                  (UnitOfMeasure.Kelvin,                    17,  "K"),
                                  (UnitOfMeasure.Hour,                      18,  "h"),
                                  (UnitOfMeasure.Minute,                    19,  "min"),
                                  (UnitOfMeasure.Joule,                     20,  "J"),
                                  (UnitOfMeasure.Pascal,                    21,  "Pa"),
                                  (UnitOfMeasure.BitPerSecond,              22,  "bit/s"),
                                  (UnitOfMeasure.Siemens,                   23,  "S"),
                                  (UnitOfMeasure.Mole,                      24,  "mol"),
                                  (UnitOfMeasure.Candela,                   25,  "cd"),
                                  (UnitOfMeasure.Newton,                    26,  "N"),
                                  (UnitOfMeasure.Coulomb,                   27,  "C"),
                                  (UnitOfMeasure.Farad,                     28,  "F"),
                                  (UnitOfMeasure.Weber,                     29,  "Wb"),
                                  (UnitOfMeasure.Tesla,                     30,  "T"),
                                  (UnitOfMeasure.Henry,                     31,  "H"),
                                  (UnitOfMeasure.Lumen,                     32,  "lm"),
                                  (UnitOfMeasure.Lux,                       33,  "lx"),
                                  (UnitOfMeasure.Becquerel,                 34,  "Bq"),
                                  (UnitOfMeasure.Gray,                      35,  "Gy"),
                                  (UnitOfMeasure.Sievert,                   36,  "Sv"),
                                  (UnitOfMeasure.Katal,                     37,  "kat"),
                                  (UnitOfMeasure.Radian,                    38,  "rad"),
                                  (UnitOfMeasure.Steradian,                 39,  "sr"),
                                  (UnitOfMeasure.Day,                       60,  "d"),
                                  (UnitOfMeasure.Degree,                    61,  "°"),
                                  (UnitOfMeasure.Litre,                     62,  "l"),
                                  (UnitOfMeasure.Tonne,                     63,  "t"),
                                  (UnitOfMeasure.Permille,                  64,  "‰"),
                                  (UnitOfMeasure.PartsPerMillion,           65,  "ppm"),
                                  (UnitOfMeasure.Bit,                      120,  "bit"),
                                  (UnitOfMeasure.Byte,                     121,  "B"),
                                  (UnitOfMeasure.BytePerSecond,            122,  "B/s"),
                                  (UnitOfMeasure.SquareMeter,              140,  "m²"),
                                  (UnitOfMeasure.CubicMeter,               141,  "m³")

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
            Assert.That(UnitOfMeasure.TryParse((UInt16) 3, out var watt),   Is.True);
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
