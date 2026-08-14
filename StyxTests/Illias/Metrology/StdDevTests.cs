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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Tests for the mean value with standard deviation.
    /// </summary>
    [TestFixture]
    public class StdDevTests
    {

        #region The_standard_deviation_matches_the_textbook_result()

        [Test]
        public void The_standard_deviation_matches_the_textbook_result()
        {

            // A well known example: 2, 4, 4, 4, 5, 5, 7, 9
            // mean = 5, population deviation = 2, sample deviation = sqrt(32/7).
            Double[] values = [2, 4, 4, 4, 5, 5, 7, 9];

            var population = StdDev<Double>.From(values, IsSampleData: false);
            Assert.That(population.Mean,                       Is.EqualTo(5.0).Within(1e-12));
            Assert.That(population.StandardDeviation,          Is.EqualTo(2.0).Within(1e-12));

            var sample = StdDev<Double>.From(values, IsSampleData: true);
            Assert.That(sample.Mean,                           Is.EqualTo(5.0).Within(1e-12));
            Assert.That(sample.StandardDeviation,              Is.EqualTo(Math.Sqrt(32.0 / 7.0)).Within(1e-12));

            // The span overload has to agree with the enumerable one...
            var fromSpan = StdDev<Double>.From(values.AsSpan(), IsSampleData: false);
            Assert.That(fromSpan.StandardDeviation,            Is.EqualTo(population.StandardDeviation).Within(1e-12));

            // Degenerate inputs must not throw...
            Assert.That(StdDev<Double>.From(Array.Empty<Double>()).Mean,              Is.EqualTo(0.0));
            Assert.That(StdDev<Double>.From(new Double[] { 42 }).Mean,                Is.EqualTo(42.0));
            Assert.That(StdDev<Double>.From(new Double[] { 42 }).StandardDeviation,   Is.EqualTo(0.0));

            // ...and the decimal overload computes the same mean.
            var decimals = StdDev<Decimal>.From(new Decimal[] { 2, 4, 4, 4, 5, 5, 7, 9 }, IsSampleData: false);
            Assert.That(decimals.Mean,                         Is.EqualTo(5m));
            Assert.That(decimals.StandardDeviation,            Is.EqualTo(2m));

        }

        #endregion

        #region A_formatted_deviation_can_be_read_back()

        [Test]
        public void A_formatted_deviation_can_be_read_back()
        {

            var stdDev = new StdDev<Decimal>(42.5m, 3.25m);

            Assert.That(stdDev.ToString(),                     Is.EqualTo("42.5 ±3.25"));

            Assert.That(StdDev<Decimal>.TryParse(stdDev.ToString(), out var parsed),  Is.True);
            Assert.That(parsed.Mean,                           Is.EqualTo(42.5m));
            Assert.That(parsed.StandardDeviation,              Is.EqualTo(3.25m));
            Assert.That(parsed,                                Is.EqualTo(stdDev));

            // Spacing around the sign is optional...
            Assert.That(StdDev<Decimal>.Parse("42.5±3.25"),     Is.EqualTo(stdDev));
            Assert.That(StdDev<Decimal>.Parse(" 42.5 ± 3.25 "), Is.EqualTo(stdDev));

            Assert.That(StdDev<Decimal>.TryParse("42.5",  out _),  Is.False);
            Assert.That(StdDev<Decimal>.TryParse("a ± b", out _),  Is.False);
            Assert.That(StdDev<Decimal>.TryParse(null,    out _),  Is.False);

        }

        #endregion

        #region Formatting_and_parsing_are_culture_invariant()

        [Test]
        public void Formatting_and_parsing_are_culture_invariant()
        {

            // A deviation written on a German machine has to be readable
            // on an English one - the CI runs both.
            var previousCulture = CultureInfo.CurrentCulture;

            try
            {

                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                var stdDev = new StdDev<Decimal>(42.5m, 3.25m);

                Assert.That(stdDev.ToString(),                 Is.EqualTo("42.5 ±3.25"),
                            "The invariant decimal point must not turn into a comma!");

                Assert.That(StdDev<Decimal>.Parse(stdDev.ToString()),  Is.EqualTo(stdDev));

                // An explicitly given culture is still honoured...
                Assert.That(StdDev<Decimal>.Parse("42,5 ± 3,25", new CultureInfo("de-DE")),
                            Is.EqualTo(stdDev));

            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }

        }

        #endregion

        #region The_JSON_array_can_be_read_back()

        [Test]
        public void The_JSON_array_can_be_read_back()
        {

            // ToJSON() writes [ mean, deviation ] and, with a unit,
            // [ mean, deviation, unit ]. Both have to parse - the JSON
            // parser used to return true while handing out nothing at all.

            var stdDev = new StdDev<Decimal>(42.5m, 3.25m);

            var json = stdDev.ToJSON();
            Assert.That(json.Count,                            Is.EqualTo(2));

            Assert.That(StdDev<Decimal>.TryParse(json, out var parsed, out var errorResponse),  Is.True);
            Assert.That(errorResponse,                         Is.Null);
            Assert.That(parsed,                                Is.EqualTo(stdDev));

            // The unit is not part of this data structure and is ignored...
            var withUnit = stdDev.ToJSON("W");
            Assert.That(withUnit.Count,                        Is.EqualTo(3));
            Assert.That(StdDev<Decimal>.TryParse(withUnit, out var parsedWithUnit, out _),  Is.True);
            Assert.That(parsedWithUnit,                        Is.EqualTo(stdDev));

            // Malformed input reports a reason instead of claiming success...
            Assert.That(StdDev<Decimal>.TryParse(new JArray(1), out _, out var tooShort),        Is.False);
            Assert.That(tooShort,                              Is.Not.Null);

            Assert.That(StdDev<Decimal>.TryParse(new JArray("a", "b"), out _, out var invalid),  Is.False);
            Assert.That(invalid,                               Is.Not.Null);

        }

        #endregion

        #region TimeSpans_carry_their_deviation_too()

        [Test]
        public void TimeSpans_carry_their_deviation_too()
        {

            var milliseconds = StdDevTimeSpanExtensions.FromMilliseconds(1500, 250);
            Assert.That(milliseconds.Mean,                     Is.EqualTo(TimeSpan.FromMilliseconds(1500)));
            Assert.That(milliseconds.StandardDeviation,        Is.EqualTo(TimeSpan.FromMilliseconds(250)));

            var seconds = StdDevTimeSpanExtensions.FromSeconds(90, 5);
            Assert.That(seconds.Mean,                          Is.EqualTo(TimeSpan.FromSeconds(90)));
            Assert.That(seconds.Mean,                          Is.EqualTo(TimeSpan.FromMinutes(1.5)));
            Assert.That(seconds.StandardDeviation,             Is.EqualTo(TimeSpan.FromSeconds(5)));

            var minutes = StdDevTimeSpanExtensions.FromMinutes(2, 0.5);
            Assert.That(minutes.Mean,                          Is.EqualTo(TimeSpan.FromMinutes(2)));
            Assert.That(minutes.StandardDeviation,             Is.EqualTo(TimeSpan.FromSeconds(30)));

            // The three factories describe the same quantity in different units...
            Assert.That(StdDevTimeSpanExtensions.FromSeconds(60, 1).Mean,
                        Is.EqualTo(StdDevTimeSpanExtensions.FromMinutes(1, 1.0 / 60).Mean));

        }

        #endregion

        #region Unit_statistics_stay_in_the_base_unit()

        [Test]
        public void Unit_statistics_stay_in_the_base_unit()
        {

            // The extensions compute on the raw value, so they have to feed
            // the result back through a factory of exponent zero - otherwise
            // the statistics come back scaled by a power of ten.

            var watts = new[] {
                            Watt.FromW(100),
                            Watt.FromW(200),
                            Watt.FromW(300)
                        };

            Assert.That(watts.Sum().Value,                     Is.EqualTo(600m));
            Assert.That(watts.Avg().Value,                     Is.EqualTo(200m));
            Assert.That(watts.StdDev(IsSampleData: false).Mean.Value,   Is.EqualTo(200m));

            // Kilogram counts kilograms, so its statistics must too...
            var masses = new[] {
                             Kilogram.FromKG(10),
                             Kilogram.FromKG(20),
                             Kilogram.FromKG(30)
                         };

            Assert.That(masses.Sum().Value,                    Is.EqualTo(60m));
            Assert.That(masses.Avg().Value,                    Is.EqualTo(20m));
            Assert.That(masses.StdDev(IsSampleData: false).Mean.Value,  Is.EqualTo(20m));

        }

        #endregion

    }

}
