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

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// ImmutableOpeningTimes tests.
    /// </summary>
    [TestFixture]
    public class ImmutableOpeningTimes_Tests
    {

        private static readonly DateTimeOffset Christmas  = new (2026, 12, 24,  12, 0, 0, 0,  TimeSpan.Zero);
        private static readonly DateTimeOffset NewYear    = new (2026, 12, 31,  18, 0, 0, 0,  TimeSpan.Zero);


        #region ExceptionalClosings_are_the_closings()

        /// <summary>
        /// The mutable opening times returned their openings as closings.
        /// </summary>
        [Test]
        public void ExceptionalClosings_are_the_closings()
        {

            var openingTimes = new OpeningTimes().
                                   AddExceptionalOpening(Christmas, Christmas.AddHours(2)).
                                   AddExceptionalClosing(NewYear,   NewYear.  AddHours(6));

            Assert.That(openingTimes.ExceptionalOpenings.Single().Begin, Is.EqualTo(Christmas));
            Assert.That(openingTimes.ExceptionalClosings.Single().Begin, Is.EqualTo(NewYear));

        }

        #endregion

        #region A_copy_keeps_every_period_in_its_order()

        /// <summary>
        /// An opening over midnight is split into two periods; the copy and its mutable copy
        /// keep exactly these, in their order, and write the same JSON.
        /// </summary>
        [Test]
        public void A_copy_keeps_every_period_in_its_order()
        {

            var source = new OpeningTimes("Ring the bell").
                             AddRegularOpening    (DayOfWeek.Saturday, HourMin.Parse("22:00"), HourMin.Parse("02:00")).
                             AddRegularOpening    (DayOfWeek.Monday,   HourMin.Parse("07:00"), HourMin.Parse("21:00")).
                             AddExceptionalOpening(Christmas, Christmas.AddHours(2)).
                             AddExceptionalClosing(NewYear,   NewYear.  AddHours(6));

            var copy   = new ImmutableOpeningTimes(source);

            Assert.That(copy.IsOpen24Hours,                                         Is.False);
            Assert.That(copy.FreeText,                                              Is.EqualTo("Ring the bell"));
            Assert.That(copy.RegularOpenings.Keys,                                  Is.EquivalentTo(new[] { DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday }));
            Assert.That(copy.RegularOpenings[DayOfWeek.Sunday].Single().PeriodEnd,  Is.EqualTo(HourMin.Parse("02:00")));
            Assert.That(copy.ExceptionalOpenings.Single().Begin,                    Is.EqualTo(Christmas));
            Assert.That(copy.ExceptionalClosings.Single().Begin,                    Is.EqualTo(NewYear));

            Assert.That(copy.ToJSON().ToString(),                                   Is.EqualTo(source.ToJSON().ToString()));
            Assert.That(copy.ToMutable().ToJSON().ToString(),                       Is.EqualTo(source.ToJSON().ToString()));
            Assert.That(new ImmutableOpeningTimes(copy.ToMutable()).ToJSON().ToString(),
                                                                                    Is.EqualTo(source.ToJSON().ToString()));

        }

        #endregion

        #region A_copy_is_detached_from_its_source()

        [Test]
        public void A_copy_is_detached_from_its_source()
        {

            var source = new OpeningTimes().
                             AddRegularOpening(DayOfWeek.Monday, HourMin.Parse("07:00"), HourMin.Parse("21:00"));

            var copy   = new ImmutableOpeningTimes(source);

            source.AddRegularOpening(DayOfWeek.Tuesday, HourMin.Parse("07:00"), HourMin.Parse("21:00"));
            copy.ToMutable().AddRegularOpening(DayOfWeek.Friday, HourMin.Parse("07:00"), HourMin.Parse("21:00"));

            Assert.That(copy.RegularOpenings.Keys, Is.EquivalentTo(new[] { DayOfWeek.Monday }));

        }

        #endregion

        #region Without_regular_openings_it_is_open_24_hours()

        [Test]
        public void Without_regular_openings_it_is_open_24_hours()
        {

            var copy = new ImmutableOpeningTimes(OpeningTimes.Open24Hours);

            Assert.That(copy.IsOpen24Hours,                                         Is.True);
            Assert.That(copy.ToJSON().ToString(Newtonsoft.Json.Formatting.None),   Is.EqualTo("{\"24/7\":true}"));

        }

        #endregion

        #region JSON_and_CBOR_read_back_what_they_wrote()

        /// <summary>
        /// The JSON and CBOR representations read back into the same opening times, also as JSON text.
        /// </summary>
        [Test]
        public void JSON_and_CBOR_read_back_what_they_wrote()
        {

            var source = new ImmutableOpeningTimes(new OpeningTimes("Ring the bell").
                                                       AddRegularOpening    (DayOfWeek.Saturday, HourMin.Parse("22:00"), HourMin.Parse("02:00")).
                                                       AddRegularOpening    (DayOfWeek.Monday,   HourMin.Parse("07:00"), HourMin.Parse("21:00")).
                                                       AddExceptionalOpening(Christmas, Christmas.AddHours(2)).
                                                       AddExceptionalClosing(NewYear,   NewYear.  AddHours(6)));

            var json   = source.ToJSON();

            using (Assert.EnterMultipleScope())
            {

                Assert.That(ImmutableOpeningTimes.TryParse(json,                                out var fromJSON, out var errorJSON),  Is.True, errorJSON);
                Assert.That(ImmutableOpeningTimes.TryParse(Newtonsoft.Json.Linq.JObject.Parse(json.ToString()),      out var fromText, out var errorText),  Is.True, errorText);
                Assert.That(ImmutableOpeningTimes.TryParse(source.ToCBOR(),                     out var fromCBOR, out var errorCBOR),  Is.True, errorCBOR);
                Assert.That(ImmutableOpeningTimes.TryParse(OpeningTimes.Open24Hours.ToJSON(),  out var open247,  out var error247),   Is.True, error247);

                Assert.That(fromJSON!.ToJSON().ToString(),                   Is.EqualTo(json.ToString()));
                Assert.That(fromText!.ToJSON().ToString(),                   Is.EqualTo(json.ToString()));
                Assert.That(fromCBOR!.ToJSON().ToString(),                   Is.EqualTo(json.ToString()));
                Assert.That(fromCBOR.ExceptionalClosings.Single().Begin,     Is.EqualTo(NewYear));
                Assert.That(open247!.IsOpen24Hours,                          Is.True);

            }

        }

        #endregion

        #region Invalid_JSON_is_refused()

        [Test]
        public void Invalid_JSON_is_refused()
        {

            var invalid = new[] {
                              "{ \"24/7\": true, \"regularOpenings\": [ { \"monday\": [ { \"begin\": \"07:00\", \"end\": \"21:00\" } ] } ] }",
                              "{ \"regularOpenings\": [ { \"Monday\": [ { \"begin\": \"07:00\", \"end\": \"21:00\" } ] } ] }",
                              "{ \"regularOpenings\": [ { \"monday\": [ { \"begin\": \"7 am\", \"end\": \"21:00\" } ] } ] }",
                              "{ \"regularOpenings\": [ { \"monday\": [] }, { \"monday\": [] } ] }",
                              "{ \"exceptionalOpenings\": [ \"2026-12-24T12:00:00.000Z\" ] }",
                              "{ \"exceptionalClosings\": [ \"2026-12-24T12:00:00.000Z -> 2026-12-24T10:00:00.000Z\" ] }",
                              "{ \"opening\": true }"
                          };

            using (Assert.EnterMultipleScope())
            {
                foreach (var text in invalid)
                    Assert.That(OpeningTimes.TryParse(Newtonsoft.Json.Linq.JObject.Parse(text), out _, out _),  Is.False, text);
            }

        }

        #endregion

    }

}
