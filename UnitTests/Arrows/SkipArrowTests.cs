/*
 * Copyright (c) 2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Arrows.NET <http://www.github.com/ahzf/Arrows.NET>
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

using System;
using System.Collections.Generic;

using de.ahzf.Arrows;

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests
{

    [TestFixture]
    public class SkipArrowTests
    {

        #region testSkipArrowZero()

        [Test]
        public void testSkipArrowZero()
        {

            var Messages  = new List<Double>() { 1, 2, 3, 4, 5, 6, 7 };
            var SkipArrow = new SkipArrow<Double>(0);

            var Counter = 0;

            SkipArrow.OnMessageAvailable += (Arrow, StdDevAverage) => {
                Counter++;
                return true;
            };

            foreach (var Message in Messages)
                SkipArrow.ReceiveMessage(Message);

            Assert.AreEqual(Messages.Count, Counter);

        }

        #endregion

        #region testSkipArrowSmaller()

        [Test]
        public void testSkipArrowSmaller()
        {

            var Messages  = new List<Double>() { 1, 2, 3, 4, 5, 6, 7 };
            var SkipArrow = new SkipArrow<Double>(3);

            var Counter = 0;

            SkipArrow.OnMessageAvailable += (Arrow, StdDevAverage) => {
                Counter++;
                return true;
            };

            foreach (var Message in Messages)
                SkipArrow.ReceiveMessage(Message);

            Assert.AreEqual(Messages.Count-3, Counter);

        }

        #endregion

        #region testSkipArrowLarger()

        [Test]
        public void testSkipArrowLarger()
        {

            var Messages  = new List<Double>() { 1, 2, 3, 4, 5, 6, 7 };
            var SkipArrow = new SkipArrow<Double>(10);

            var Counter = 0;

            SkipArrow.OnMessageAvailable += (Arrow, StdDevAverage) => {
                Counter++;
                return true;
            };

            foreach (var Message in Messages)
                SkipArrow.ReceiveMessage(Message);

            Assert.AreEqual(0, Counter);

        }

        #endregion

    }

}
