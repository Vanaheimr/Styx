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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// ISO 8601 texts of timestamps.
    /// </summary>
    [TestFixture]
    public class ISO8601_Tests
    {

        private static readonly DateTimeOffset Timestamp = new (2026, 10, 11,  14, 5, 6, 789,  TimeSpan.FromHours(2));


        #region ISO8601_does_not_depend_on_the_current_culture(Culture)

        /// <summary>
        /// A custom format takes its time separator and its calendar from the current culture:
        /// th-TH counts the Buddhist era (2569), fi-FI separates hours and minutes with dots.
        /// </summary>
        [TestCase("en-US")]
        [TestCase("de-DE")]
        [TestCase("fi-FI")]
        [TestCase("th-TH")]
        public void ISO8601_does_not_depend_on_the_current_culture(String Culture)
        {

            var previous = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo(Culture);

            try
            {

                Assert.That(Timestamp.ToISO8601(),                    Is.EqualTo("2026-10-11T12:05:06.789Z"));
                Assert.That(Timestamp.ToISO8601(Fractions: false),    Is.EqualTo("2026-10-11T12:05:06Z"));
                Assert.That(Timestamp.ToISO8601WithOffset(),          Is.EqualTo("2026-10-11T14:05:06.789+02:00"));

                Assert.That(Timestamp.UtcDateTime.ToISO8601(),        Is.EqualTo("2026-10-11T12:05:06.789Z"));

            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }

        }

        #endregion

    }

}
