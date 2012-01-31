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
    public class StdDevArrowTests
    {

        #region testStdDevArrowSingle()

        [Test]
        public void testStdDevArrowSingle()
        {

            var Messages    = new List<Double>() { 505.0 };
            var StdDevArrow = new StdDevArrow();

            var Variance = 0.0;
            var StdDev   = 0.0;
            var Average  = 0.0;

            StdDevArrow.OnMessageAvailable += (Arrow, StdDevAverage) => {
                Variance = StdDevAverage.Item1;
                StdDev   = StdDevAverage.Item2;
                Average  = StdDevAverage.Item3;
                return true;
            };

            foreach (var Message in Messages)
                StdDevArrow.ReceiveMessage(Message);

            Assert.AreEqual(  0.0, Variance);
            Assert.AreEqual(  0.0, StdDev);
            Assert.AreEqual(505.0, Average);

        }

        #endregion

        #region testStdDevArrow()

        [Test]
        public void testStdDevArrow()
        {

            var Messages    = new List<Double>() { 505.0, 500.0, 495.0, 505.0 };
            var StdDevArrow = new StdDevArrow();

            var Variance = 0.0;
            var StdDev   = 0.0;
            var Average  = 0.0;

            StdDevArrow.OnMessageAvailable += (Arrow, StdDevAverage) => {
                Variance = StdDevAverage.Item1;
                StdDev   = StdDevAverage.Item2;
                Average  = StdDevAverage.Item3;
                return true;
            };

            foreach (var Message in Messages)
                StdDevArrow.ReceiveMessage(Message);

            Assert.AreEqual( 22.92, Math.Round(Variance, 2));
            Assert.AreEqual(501.25, Average);

        }

        #endregion

        #region testStdDevSideEffectArrowSingle()

        [Test]
        public void testStdDevSideEffectArrowSingle()
        {

            var Messages              = new List<Double>() { 505.0 };
            var StdDevSideEffectArrow = new StdDevSideEffectArrow();

            foreach (var Message in Messages)
                StdDevSideEffectArrow.ReceiveMessage(Message);

            Assert.AreEqual(  0.0, StdDevSideEffectArrow.Variance);
            Assert.AreEqual(  0.0, StdDevSideEffectArrow.StdDev);
            Assert.AreEqual(505.0, StdDevSideEffectArrow.Average);

        }

        #endregion

        #region testStdDevSideEffectArrow()

        [Test]
        public void testStdDevSideEffectArrow()
        {

            var Messages              = new List<Double>() { 505.0, 500.0, 495.0, 505.0 };
            var StdDevSideEffectArrow = new StdDevSideEffectArrow();

            foreach (var Message in Messages)
                StdDevSideEffectArrow.ReceiveMessage(Message);

            Assert.AreEqual( 22.92, Math.Round(StdDevSideEffectArrow.Variance, 2));
            Assert.AreEqual(501.25, StdDevSideEffectArrow.Average);

        }

        #endregion

    }

}
