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

using NUnit.Framework;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// StatusSchedule tests.
    /// </summary>
    [TestFixture]
    public class StatusSchedule_Tests
    {

        #region CurrentValue_UsesNewestPastTimestamp_FromConstructor()

        /// <summary>
        /// Constructor input should not have to be pre-sorted.
        /// </summary>
        [Test]
        public void CurrentValue_UsesNewestPastTimestamp_FromConstructor()
        {

            var now       = Timestamp.Now;
            var older     = new Timestamped<String>(now - TimeSpan.FromDays(2), "old");
            var newer     = new Timestamped<String>(now - TimeSpan.FromDays(1), "new");
            var schedule  = new StatusSchedule<String>([ older, newer ]);

            Assert.That(schedule.CurrentValue, Is.EqualTo("new"));

        }

        #endregion

        #region NextStatus_UsesNearestFutureTimestamp_FromConstructor()

        /// <summary>
        /// The next status should be the nearest future status, independent of input order.
        /// </summary>
        [Test]
        public void NextStatus_UsesNearestFutureTimestamp_FromConstructor()
        {

            var now       = Timestamp.Now;
            var past      = new Timestamped<String>(now - TimeSpan.FromDays(1), "current");
            var near      = new Timestamped<String>(now + TimeSpan.FromDays(1), "next");
            var far       = new Timestamped<String>(now + TimeSpan.FromDays(2), "later");
            var schedule  = new StatusSchedule<String>([ past, near, far ]);

            Assert.That(schedule.CurrentValue,       Is.EqualTo("current"));
            Assert.That(schedule.NextStatus?.Value,  Is.EqualTo("next"));

        }

        #endregion

        #region InsertList_EnforcesMaxHistorySizeAcrossExistingEntries()

        /// <summary>
        /// Bulk insert should apply MaxStatusHistorySize to the whole schedule.
        /// </summary>
        [Test]
        public void InsertList_EnforcesMaxHistorySizeAcrossExistingEntries()
        {

            var now       = Timestamp.Now;
            var schedule  = new StatusSchedule<String>(
                                [
                                    new Timestamped<String>(now - TimeSpan.FromDays(3), "old")
                                ],
                                MaxStatusListSize: 2
                            );

            schedule.Insert(
                [
                    new Timestamped<String>(now - TimeSpan.FromDays(2), "newer"),
                    new Timestamped<String>(now - TimeSpan.FromDays(1), "newest")
                ]
            );

            Assert.That(schedule.Select(status => status.Value), Is.EqualTo(new[] { "newest", "newer" }));

        }

        #endregion

        #region InsertList_ReplacesExistingEntryWithSameTimestamp()

        /// <summary>
        /// Bulk insert should replace an existing entry having the same timestamp.
        /// </summary>
        [Test]
        public void InsertList_ReplacesExistingEntryWithSameTimestamp()
        {

            var timestamp  = Timestamp.Now - TimeSpan.FromDays(1);
            var schedule   = new StatusSchedule<String>(
                                 [
                                     new Timestamped<String>(timestamp, "old")
                                 ]
                             );

            schedule.Insert(
                [
                    new Timestamped<String>(timestamp, "new")
                ]
            );

            Assert.That(schedule.Select(status => status.Value), Is.EqualTo(new[] { "new" }));

        }

        #endregion

        #region Insert_ReplacesExistingEntryWithSameISO8601Timestamp()

        /// <summary>
        /// Single inserts with the same ISO 8601 timestamp should keep the last value.
        /// </summary>
        [Test]
        public void Insert_ReplacesExistingEntryWithSameISO8601Timestamp()
        {

            var timestamp1  = Timestamp.Now - TimeSpan.FromDays(1);
            var timestamp2  = timestamp1.AddTicks(1);
            var schedule    = new StatusSchedule<String>();

            Assert.That(timestamp2.ToISO8601(), Is.EqualTo(timestamp1.ToISO8601()));

            schedule.Insert("old", timestamp1);
            schedule.Insert("new", timestamp2);

            Assert.That(schedule.Select(status => status.Value), Is.EqualTo(new[] { "new" }));
            Assert.That(schedule.CurrentValue,                   Is.EqualTo("new"));

        }

        #endregion

        #region InsertList_ReplacesExistingEntryWithSameISO8601Timestamp()

        /// <summary>
        /// Bulk inserts with the same ISO 8601 timestamp should keep the last value.
        /// </summary>
        [Test]
        public void InsertList_ReplacesExistingEntryWithSameISO8601Timestamp()
        {

            var timestamp1  = Timestamp.Now - TimeSpan.FromDays(1);
            var timestamp2  = timestamp1.AddTicks(1);
            var schedule    = new StatusSchedule<String>();

            Assert.That(timestamp2.ToISO8601(), Is.EqualTo(timestamp1.ToISO8601()));

            schedule.Insert(
                [
                    new Timestamped<String>(timestamp1, "old"),
                    new Timestamped<String>(timestamp2, "new")
                ]
            );

            Assert.That(schedule.Select(status => status.Value), Is.EqualTo(new[] { "new" }));
            Assert.That(schedule.CurrentValue,                   Is.EqualTo("new"));

        }

        #endregion

        #region Insert_DoesNotCompareNewValueOnlyAgainstNewestScheduledEntry()

        /// <summary>
        /// A value equal to a future status must still be insertable when the current status differs.
        /// </summary>
        [Test]
        public void Insert_DoesNotCompareNewValueOnlyAgainstNewestScheduledEntry()
        {

            var now       = Timestamp.Now;
            var schedule  = new StatusSchedule<String>();

            schedule.Insert("available",  now - TimeSpan.FromDays(1));
            schedule.Insert("reserved",   now + TimeSpan.FromDays(1));
            schedule.Insert("reserved",   now);

            Assert.That(schedule.CurrentValue, Is.EqualTo("reserved"));

        }

        #endregion

    }

}
