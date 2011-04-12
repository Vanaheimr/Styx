﻿/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/pipes.NET>
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

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.FilterPipes
{

    [TestFixture]
    public class RandomFilterPipeTest
    {

        #region testRangeFilter1_0()

        [Test]
        public void testRangeFilter1_0()
        {

            var _Pipe = new RandomFilterPipe<String>(1.0d);
            _Pipe.SetSourceCollection(BaseTest.GenerateUUIDs(100));

            var _Counter = 0;
            while (_Pipe.MoveNext())
                _Counter++;

            Assert.AreEqual(100, _Counter);

        }

        #endregion

        #region testRangeFilter0_0()

        [Test]
        public void testRangeFilter0_0()
        {

            var _Pipe = new RandomFilterPipe<String>(0.0d);
            _Pipe.SetSourceCollection(BaseTest.GenerateUUIDs(100));

            var _Counter = 0;
            while (_Pipe.MoveNext())
                _Counter++;

            Assert.AreEqual(0, _Counter);

        }

        #endregion

        #region testRangeFilter0_5()

        [Test]
        public void testRangeFilter0_5()
        {

            var _Pipe = new RandomFilterPipe<String>(0.5d);
            _Pipe.SetSourceCollection(BaseTest.GenerateUUIDs(10000));

            var _Counter = 0;
            while (_Pipe.MoveNext())
                _Counter++;

            Assert.GreaterOrEqual(_Counter, 4000);
            Assert.LessOrEqual(_Counter, 6000);

        }

        #endregion

    }

}
