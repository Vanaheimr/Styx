/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET
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
using System.Linq;

using NUnit.Framework;
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes.UnitTests.Pipes
{

    [TestFixture]
    public class IdentityPipeTest
    {

        #region testIdentityPipeNormal()

        [Test]
        public void testIdentityPipeNormal()
        {

            var _UUIDs = de.ahzf.Pipes.UnitTests.BaseTest.GenerateUUIDs(100);
            var _Pipe  = new IdentityPipe<String>();
            _Pipe.SetSourceCollection(_UUIDs);

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                Assert.AreEqual(_Pipe.Current, _UUIDs.ElementAt(_Counter));
                _Counter++;
            }

            Assert.AreEqual(_Counter, 100);

        }

        #endregion

        #region testIdentityPipeZero
        
        [Test]
        public void testIdentityPipeZero()
        {

            var uuids = de.ahzf.Pipes.UnitTests.BaseTest.GenerateUUIDs(0);
            IPipe<String, String> pipe = new IdentityPipe<String>();
            pipe.SetSourceCollection(uuids);
            int counter = 0;
            Assert.IsFalse(pipe.Any());
            Assert.AreEqual(counter, 0);
            Assert.IsFalse(pipe.Any());

        }

        #endregion

    }

}
