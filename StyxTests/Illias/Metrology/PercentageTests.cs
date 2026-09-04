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
    /// Tests for the percentage structs, which - unlike the SI unit structs -
    /// are bounded value types without a prefix scheme.
    /// </summary>
    [TestFixture]
    public class PercentageTests
    {

        #region Each_percentage_type_has_the_range_its_purpose_implies()

        [Test]
        public void Each_percentage_type_has_the_range_its_purpose_implies()
        {

            // A percentage is a ratio times a hundred and is NOT capped at
            // 100: a load of 150 % or 200 % of a target are ordinary
            // readings. Only negative values are impossible here.
            Assert.That(Percentage.TryParse(  0m, out var zero),          Is.True);
            Assert.That(zero.Value,                                       Is.EqualTo(0m));

            Assert.That(Percentage.TryParse(150m, out var overload),      Is.True);
            Assert.That(overload.Value,                                   Is.EqualTo(150m));

            Assert.That(Percentage.TryParse( -1m, out _),                 Is.False);
            Assert.That(PercentageDouble.TryParse(150.5, out var big),    Is.True);
            Assert.That(big.Value,                                        Is.EqualTo(150.5));
            Assert.That(PercentageDouble.TryParse(-0.5,  out _),          Is.False);

            // ...and an unbounded upper end must not let infinity in.
            Assert.That(PercentageDouble.TryParse(Double.PositiveInfinity, out _),  Is.False);
            Assert.That(PercentageDouble.TryParse(Double.NaN,              out _),  Is.False);

            // A percentage change has no bound in either direction:
            // -100 % is a total loss, +500 % is fivefold growth.
            Assert.That(SignedPercentage.TryParse(-100m, out var loss),   Is.True);
            Assert.That(loss.Value,                                       Is.EqualTo(-100m));
            Assert.That(SignedPercentage.TryParse( 500m, out var growth), Is.True);
            Assert.That(growth.Value,                                     Is.EqualTo(500m));
            Assert.That(SignedPercentage.TryParse(-250m, out var drop),   Is.True);
            Assert.That(drop.Value,                                       Is.EqualTo(-250m));

            // The byte backed type is the exception: it exists to pack a
            // share of a whole into one byte, so it stays within 0..100
            // although a Byte could hold up to 255.
            Assert.That(PercentageByte.TryParse((Byte) 100, out var full),  Is.True);
            Assert.That(full.Value,                                         Is.EqualTo((Byte) 100));
            Assert.That(PercentageByte.TryParse((Byte) 101, out _),         Is.False);
            Assert.That(PercentageByte.TryParse((Byte) 255, out _),         Is.False);

        }

        #endregion

        #region A_formatted_percentage_can_be_read_back()

        [Test]
        public void A_formatted_percentage_can_be_read_back()
        {

            // ToString() appends " %", so parsing has to accept it -
            // otherwise a value can be printed but never read back.

            var percentage = Percentage.Parse(42.5m);
            Assert.That(percentage.ToString(),                        Is.EqualTo("42.5 %"));
            Assert.That(Percentage.Parse(percentage.ToString()).Value, Is.EqualTo(42.5m));

            var signed = SignedPercentage.Parse(-42.5m);
            Assert.That(signed.ToString(),                            Is.EqualTo("-42.5 %"));
            Assert.That(SignedPercentage.Parse(signed.ToString()).Value, Is.EqualTo(-42.5m));

            var asDouble = PercentageDouble.Parse(42.5);
            Assert.That(PercentageDouble.Parse(asDouble.ToString()).Value, Is.EqualTo(42.5));

            var asByte = PercentageByte.Parse((Byte) 42);
            Assert.That(PercentageByte.Parse(asByte.ToString()).Value,     Is.EqualTo((Byte) 42));

            // A bare number stays acceptable, with or without the sign...
            Assert.That(Percentage.Parse("42.5").Value,               Is.EqualTo(42.5m));
            Assert.That(Percentage.Parse("42.5%").Value,              Is.EqualTo(42.5m));

        }

        #endregion

        #region Percentage_parsing_is_culture_invariant()

        [Test]
        public void Percentage_parsing_is_culture_invariant()
        {

            // ToString() always writes an invariant decimal point, so parsing
            // must read one - regardless of the culture of the machine. The CI
            // runs on Windows and inside a debian:13 container.

            var previousCulture = CultureInfo.CurrentCulture;

            try
            {

                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                Assert.That(Percentage.TryParse("12.5", out var value),   Is.True);
                Assert.That(value.Value,                                  Is.EqualTo(12.5m),
                            "'12.5' must be twelve and a half, not one hundred and twenty five!");

                Assert.That(Percentage.Parse(Percentage.Parse(12.5m).ToString()).Value,
                            Is.EqualTo(12.5m));

            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }

        }

        #endregion

        #region The_IParsable_implementation_is_usable()

        [Test]
        public void The_IParsable_implementation_is_usable()
        {

            // These structs declare IParsable<T>, so generic code may reach
            // them through that interface - it must not throw.

            Assert.That(Percentage.      Parse("42.5", CultureInfo.InvariantCulture).Value,  Is.EqualTo(42.5m));
            Assert.That(SignedPercentage.Parse("-42.5", CultureInfo.InvariantCulture).Value, Is.EqualTo(-42.5m));
            Assert.That(PercentageDouble.Parse("42.5", CultureInfo.InvariantCulture).Value,  Is.EqualTo(42.5));
            Assert.That(PercentageByte.  Parse("42",   CultureInfo.InvariantCulture).Value,  Is.EqualTo((Byte) 42));

            Assert.That(Percentage.TryParse("42.5", CultureInfo.InvariantCulture, out var value),  Is.True);
            Assert.That(value.Value,                                                               Is.EqualTo(42.5m));

            Assert.That(Percentage.TryParse("101", CultureInfo.InvariantCulture, out var above),  Is.True);
            Assert.That(above.Value,                                                              Is.EqualTo(101m));

            Assert.That(Percentage.TryParse("-1",  CultureInfo.InvariantCulture, out _),  Is.False);
            Assert.That(Percentage.TryParse(null,  CultureInfo.InvariantCulture, out _),  Is.False);
            Assert.That(Percentage.TryParse("abc", CultureInfo.InvariantCulture, out _),  Is.False);

            // A given culture is honoured...
            Assert.That(Percentage.Parse("12,5", new CultureInfo("de-DE")).Value,  Is.EqualTo(12.5m));

        }

        #endregion

        #region PercentageByte_rounds_to_a_whole_percent()

        [Test]
        public void PercentageByte_rounds_to_a_whole_percent()
        {

            Assert.That(PercentageByte.TryParseDouble (42.4,  out var down),  Is.True);
            Assert.That(down.Value,                                           Is.EqualTo((Byte) 42));

            Assert.That(PercentageByte.TryParseDouble (42.6,  out var up),    Is.True);
            Assert.That(up.Value,                                             Is.EqualTo((Byte) 43));

            Assert.That(PercentageByte.TryParseDecimal(42.6m, out var upDec), Is.True);
            Assert.That(upDec.Value,                                          Is.EqualTo((Byte) 43));

            // Out of range values are rejected before any conversion...
            Assert.That(PercentageByte.TryParseDouble (100.6, out _),         Is.False);
            Assert.That(PercentageByte.TryParseDecimal(-0.6m, out _),         Is.False);

        }

        #endregion

        #region PercentageOf_carries_a_value_and_its_share()

        [Test]
        public void PercentageOf_carries_a_value_and_its_share()
        {

            var share = new PercentageOf<String>("solar", 42.5f);

            Assert.That(share.Value,                                  Is.EqualTo("solar"));
            Assert.That(share.Percent,                                Is.EqualTo(42.5f));

            var (value, percent) = share;
            Assert.That(value,                                        Is.EqualTo("solar"));
            Assert.That(percent,                                      Is.EqualTo(42.5f));

            Assert.That(share,                                        Is.EqualTo(new PercentageOf<String>("solar", 42.5f)));
            Assert.That(share == new PercentageOf<String>("solar", 42.5f),  Is.True);
            Assert.That(share == new PercentageOf<String>("wind",  42.5f),  Is.False);

            // The inequality operator has to negate - it used to return the
            // result of Equals, inverting every comparison made with it.
            Assert.That(share != new PercentageOf<String>("wind",  42.5f),  Is.True);
            Assert.That(share != new PercentageOf<String>("solar", 99.9f),  Is.True);
            Assert.That(share != new PercentageOf<String>("solar", 42.5f),  Is.False);

        }

        #endregion

        #region Triple_inequality_negates_too()

        [Test]
        public void Triple_inequality_negates_too()
        {

            // PercentageOf was copied from Triple - including an inequality
            // operator that returned Equals() unnegated. Pin the original,
            // so the defect cannot travel any further.

            var triple = new Triple<String, Int32, Boolean>("a", 1, true);

            Assert.That(triple == new Triple<String, Int32, Boolean>("a", 1, true),   Is.True);
            Assert.That(triple != new Triple<String, Int32, Boolean>("a", 1, true),   Is.False);

            Assert.That(triple == new Triple<String, Int32, Boolean>("b", 1, true),   Is.False);
            Assert.That(triple != new Triple<String, Int32, Boolean>("b", 1, true),   Is.True);
            Assert.That(triple != new Triple<String, Int32, Boolean>("a", 2, true),   Is.True);
            Assert.That(triple != new Triple<String, Int32, Boolean>("a", 1, false),  Is.True);

        }

        #endregion

    }

}
