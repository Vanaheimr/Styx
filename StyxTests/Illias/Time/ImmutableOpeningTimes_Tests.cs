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

        private static readonly DateTime Christmas  = new (2026, 12, 24, 12, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime NewYear    = new (2026, 12, 31, 18, 0, 0, DateTimeKind.Utc);


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

    }

}
