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

#endregion

namespace de.ahzf.Pipes.UnitTests
{

    [TestFixture]
    public class IdentityPipeTest
    {

        #region testIdentityPipeNormal()

        [Test]
        public void testIdentityPipeNormal()
        {

            var uuids = BaseTest.GenerateUUIDs(100);
            IPipe<String, String> pipe = new IdentityPipe<String>();
            pipe.SetSourceCollection(uuids);
            int counter = 0;
            Assert.IsTrue(pipe.Any());
            pipe.Reset();
            while (pipe.MoveNext())
            {
                Assert.AreEqual(pipe.Current, uuids.ElementAt(counter));
                counter++;
            }
            Assert.AreEqual(counter, 100);
            Assert.IsFalse(pipe.Any());

        }

        #endregion

        #region testIdentityPipeZero
        
        [Test]
        public void testIdentityPipeZero()
        {

            var uuids = BaseTest.GenerateUUIDs(0);
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
