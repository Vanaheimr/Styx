/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// NotBefore NotAfter tests.
    /// </summary>
    [TestFixture]
    public class NotBeforeNotAfter_Tests
    {

        #region (class) Things

        public class Things(String     Name,
                            DateTime?  NotBefore,
                            DateTime?  NotAfter) : INotBeforeNotAfter
        {

            public String     Name         { get; } = Name;
            public DateTime?  NotBefore    { get; } = NotBefore;
            public DateTime?  NotAfter     { get; } = NotAfter;

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


        #region Overlap_Equals_Test1()

        /// <summary>
        /// |- thing1 -|
        /// |- thing2 -|
        /// </summary>
        [Test]
        public void Overlap_Equals_Test1()
        {

            var now     = DateTime.Now;
            var thing1  = new Things("1", now, now + TimeSpan.FromDays(1));
            var thing2  = new Things("2", now, now + TimeSpan.FromDays(1));

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.True);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.True);

        }

        #endregion


        #region Overlap_Overlap_SmallTolerance_Test1()

        /// <summary>
        /// |- thing1 -|
        /// ..........|- thing2 -|
        /// </summary>
        [Test]
        public void Overlap_Overlap_SmallTolerance_Test1()
        {

            var now        = DateTime.Now;
            var thing1     = new Things("1", now - TimeSpan.FromDays(1),    now);
            var thing2     = new Things("2", now - TimeSpan.FromSeconds(5), now + TimeSpan.FromDays(1));

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.True);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.True);

        }

        #endregion

        #region Overlap_Overlap_BigTolerance_Test1()

        /// <summary>
        /// |- thing1 -|
        /// ..........|- thing2 -|
        /// </summary>
        [Test]
        public void Overlap_Overlap_BigTolerance_Test1()
        {

            var now        = DateTime.Now;
            var thing1     = new Things("1", now - TimeSpan.FromDays(1),    now);
            var thing2     = new Things("2", now - TimeSpan.FromSeconds(5), now + TimeSpan.FromDays(1));
            var tolerance  = TimeSpan.FromMinutes(1);

            //Assert.That(thing1.IsOverlapping(thing2, tolerance), Is.False);
            Assert.That(thing2.IsOverlapping(thing1, tolerance), Is.False);

        }

        #endregion


        #region Overlap_NoGap_Test1()

        /// <summary>
        /// |- thing1 -|
        /// ............|- thing2 -|
        /// </summary>
        [Test]
        public void Overlap_NoGap_Test1()
        {

            var now     = DateTime.Now;
            var thing1  = new Things("1", now - TimeSpan.FromDays(1), now);
            var thing2  = new Things("2", now,                        now + TimeSpan.FromDays(1));

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.False);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.False);

        }

        #endregion

        #region Overlap_NoGap_Infinite_Test1()

        /// <summary>
        /// :- thing1 -|
        /// ............|- thing2 -:
        /// </summary>
        [Test]
        public void Overlap_NoGap_Infinite_Test1()
        {

            var now     = DateTime.Now;
            var thing1  = new Things("1", null, now);
            var thing2  = new Things("2", now,  null);

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.False);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.False);

        }

        #endregion


        #region Overlap_Gap_Infinite_Test1()

        /// <summary>
        /// :- thing1 -|
        /// ..............|- thing2 -:
        /// </summary>
        [Test]
        public void Overlap_Gap_Infinite_Test1()
        {

            var now     = DateTime.Now;
            var thing1  = new Things("1", null, now - TimeSpan.FromDays(1));
            var thing2  = new Things("2", now,  null);

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.False);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.False);

        }

        #endregion

        #region Overlap_Gap_Test1()

        /// <summary>
        /// |- thing1 -|
        /// ..............|- thing2 -|
        /// </summary>
        [Test]
        public void Overlap_Gap_Test1()
        {

            var now     = DateTime.Now;
            var thing1  = new Things("1", now - TimeSpan.FromDays(2), now - TimeSpan.FromDays(1));
            var thing2  = new Things("2", now,                        now + TimeSpan.FromDays(1));

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.False);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.False);

        }

        #endregion


        #region Overlap_Included_Test1()

        /// <summary>
        /// |--- thing1 ---|
        /// ..|- thing2 -|
        /// </summary>
        [Test]
        public void Overlap_Included_Test1()
        {

            var now     = DateTime.Now;
            var thing1  = new Things("1", now - TimeSpan.FromDays(2), now + TimeSpan.FromDays(2));
            var thing2  = new Things("2", now - TimeSpan.FromDays(1), now + TimeSpan.FromDays(1));

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.True);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.True);

        }

        #endregion

        #region Overlap_Included_Infinite_Test1()

        /// <summary>
        /// :--- thing1 ---:
        /// ..|- thing2 -|
        /// </summary>
        [Test]
        public void Overlap_Included_Infinite_Test1()
        {

            var now     = DateTime.Now;
            var thing1  = new Things("1", null,                       null);
            var thing2  = new Things("2", now - TimeSpan.FromDays(1), now + TimeSpan.FromDays(1));

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.True);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.True);

        }

        #endregion


        #region Overlap_Overlapping_Test1()

        /// <summary>
        /// |--- thing1 ---|
        /// ..|- thing2 ------|
        /// </summary>
        [Test]
        public void Overlap_Overlapping_Test1()
        {

            var now     = DateTime.Now;
            var thing1  = new Things("1", now - TimeSpan.FromDays(2), now + TimeSpan.FromDays(1));
            var thing2  = new Things("2", now - TimeSpan.FromDays(1), now + TimeSpan.FromDays(2));

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.True);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.True);

        }

        #endregion

        #region Overlap_Overlapping_Infinite_Test1()

        /// <summary>
        /// :--- thing1 ---|
        /// ..|- thing2 ------:
        /// </summary>
        [Test]
        public void Overlap_Overlapping_Infinite_Test1()
        {

            var now     = DateTime.Now;
            var thing1  = new Things("1", null,                       now + TimeSpan.FromDays(1));
            var thing2  = new Things("2", now - TimeSpan.FromDays(1), null);

            Assert.That(thing1.IsOverlapping(thing2, Tolerance), Is.True);
            Assert.That(thing2.IsOverlapping(thing1, Tolerance), Is.True);

        }

        #endregion


        //ToDo: Check whether timezone setting may affect these tests!
        //ToDo: Zero-Length durations (should be checked by the parent object, but you never know ;) )
        //ToDo: NotAfter < NotBefore (should be checked by the parent object, but you never know ;) )


    }

}
