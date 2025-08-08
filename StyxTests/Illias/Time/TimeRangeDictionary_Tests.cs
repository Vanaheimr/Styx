/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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
    /// TimeRangeDictionary tests.
    /// </summary>
    [TestFixture]
    public class TimeRangeDictionary_Tests
    {

        #region (class) Things

        public class Things(String           Name,
                            DateTimeOffset?  NotBefore,
                            DateTimeOffset?  NotAfter) : INotBeforeNotAfter
        {

            public String           Name         { get; } = Name;
            public DateTimeOffset?  NotBefore    { get; } = NotBefore;
            public DateTimeOffset?  NotAfter     { get; } = NotAfter;

        }

        #endregion

        #region Data

        private readonly TimeSpan Tolerance = TimeSpan.FromSeconds(1);

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public async Task SetupOnce()
        {

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public void SetupEachTest()
        {

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public void ShutdownEachTest()
        {

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public virtual async Task ShutdownOnce()
        {

        }

        #endregion


        #region AddOneAndGetItAtDifferentTimestamps_Test1()

        /// <summary>
        /// Add one and get it at different timestamps.
        /// </summary>
        [Test]
        public void AddOneAndGetItAtDifferentTimestamps_Test1()
        {

            var dict   = new TimeRangeDictionary<String, Things>();
            var key    = "1";
            var now    = DateTime.Now;
            var thing  = new Things("1", now, now + TimeSpan.FromDays(1));

            Assert.That(dict.TryAdd     (key, thing),                               Is.True);

            Assert.That(dict.TryGetValue(key, out _, now - TimeSpan.FromDays (1)),  Is.False);
            Assert.That(dict.TryGetValue(key, out _, now                        ),  Is.True);
            Assert.That(dict.TryGetValue(key, out _, now + TimeSpan.FromHours(6)),  Is.True);
            Assert.That(dict.TryGetValue(key, out _, now + TimeSpan.FromDays (2)),  Is.False);

        }

        #endregion

        #region AddTwoAndGetThemAtDifferentTimestamps_Test1()

        /// <summary>
        /// Add two and get them at different timestamps.
        /// </summary>
        [Test]
        public void AddTwoAndGetThemAtDifferentTimestamps_Test1()
        {

            var dict    = new TimeRangeDictionary<String, Things>();
            var key     = "1";
            var now     = DateTime.Now;
            var thing1  = new Things("1", now,                        now + TimeSpan.FromDays(1));
            var thing2  = new Things("2", now + TimeSpan.FromDays(1), now + TimeSpan.FromDays(2));

            Assert.That(dict.TryAdd     (key, thing1),                                    Is.True);
            Assert.That(dict.TryAdd     (key, thing2),                                    Is.True);

            Assert.That(dict.TryGetValue(key, out _,      now - TimeSpan.FromDays (1)),   Is.False);

            Assert.That(dict.TryGetValue(key, out var t1, now),  Is.True);
            Assert.That(t1?.Name, Is.EqualTo("1"));

            Assert.That(dict.TryGetValue(key, out var t2, now + TimeSpan.FromHours(6)),   Is.True);
            Assert.That(t2?.Name, Is.EqualTo("1"));

            Assert.That(dict.TryGetValue(key, out var t3, now + TimeSpan.FromDays (1)),   Is.True);
            Assert.That(t3?.Name, Is.EqualTo("2"));

            Assert.That(dict.TryGetValue(key, out var t4, now + TimeSpan.FromHours(30)),  Is.True);
            Assert.That(t4?.Name, Is.EqualTo("2"));

            Assert.That(dict.TryGetValue(key, out var t5, now + TimeSpan.FromDays (2)),   Is.True);
            Assert.That(t5?.Name, Is.EqualTo("2"));

            Assert.That(dict.TryGetValue(key, out _,      now + TimeSpan.FromDays (3)),   Is.False);

        }

        #endregion


    }

}
