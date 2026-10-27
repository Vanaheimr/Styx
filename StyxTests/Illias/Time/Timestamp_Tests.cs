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
    /// Time travels of the timestamp: process-wide, so these tests run alone and return to normal time.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class Timestamp_Tests
    {

        /// <summary>
        /// How much the clock may move on during a test.
        /// </summary>
        private static readonly TimeSpan Slack = TimeSpan.FromMinutes(1);

        [TearDown]
        public void ReturnToNormalTime()
            => Timestamp.Reset();


        #region TravelBackInTime_GoesBack()

        [Test]
        public void TravelBackInTime_GoesBack()
        {

            var before  = DateTimeOffset.UtcNow;

            Timestamp.TravelBackInTime(TimeSpan.FromHours(1));

            var now     = Timestamp.Now;

            Assert.That(now - before, Is.InRange(TimeSpan.FromHours(-1), TimeSpan.FromHours(-1) + Slack));

        }

        #endregion

        #region TravelBackInTime_IgnoresTheSign()

        [Test]
        public void TravelBackInTime_IgnoresTheSign()
        {

            var before  = DateTimeOffset.UtcNow;

            Timestamp.TravelBackInTime(TimeSpan.FromHours(-1));

            var now     = Timestamp.Now;

            Assert.That(now - before, Is.InRange(TimeSpan.FromHours(-1), TimeSpan.FromHours(-1) + Slack));

        }

        #endregion

        #region TravelForwardInTime_GoesForward_WhateverTheSign()

        [Test]
        public void TravelForwardInTime_GoesForward_WhateverTheSign()
        {

            var before  = DateTimeOffset.UtcNow;

            Timestamp.TravelForwardInTime(TimeSpan.FromHours( 1));
            Timestamp.TravelForwardInTime(TimeSpan.FromHours(-1));

            var now     = Timestamp.Now;

            Assert.That(now - before, Is.InRange(TimeSpan.FromHours(2), TimeSpan.FromHours(2) + Slack));

        }

        #endregion

        #region Travels_AddUp_AndReset_ReturnsToNormalTime()

        [Test]
        public void Travels_AddUp_AndReset_ReturnsToNormalTime()
        {

            Timestamp.TravelForwardInTime(TimeSpan.FromHours(3));
            Timestamp.TravelBackInTime   (TimeSpan.FromHours(5));

            var before    = DateTimeOffset.UtcNow;
            var travelled = Timestamp.Now;
            var provided  = Timestamp.Provider.GetUtcNow();

            Timestamp.Reset();

            var now       = Timestamp.Now;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(travelled - before,                Is.InRange(TimeSpan.FromHours(-2), TimeSpan.FromHours(-2) + Slack));
                Assert.That(provided  - before,                Is.InRange(TimeSpan.FromHours(-2), TimeSpan.FromHours(-2) + Slack));
                Assert.That(now       - DateTimeOffset.UtcNow, Is.InRange(-Slack, TimeSpan.Zero));
            }

        }

        #endregion

    }

}
