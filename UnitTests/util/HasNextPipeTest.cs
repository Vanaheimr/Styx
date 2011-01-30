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
using System.Collections.Generic;

using de.ahzf.blueprints;
using de.ahzf.blueprints.Datastructures;

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.util
{

    [TestFixture]
    public class HasNextPipeTest
    {

        #region testPipeBasic()

        [Test]
        public void testPipeBasic()
        {

            var names = new List<String>() { "marko", "povel", "peter", "josh" };

            var pipe1 = new HasNextPipe<String>(new IdentityPipe<String>());
            pipe1.SetSourceCollection(names);

            int counter = 0;
            while (pipe1.MoveNext())
            {
                counter++;
                Assert.IsTrue(pipe1.Current);
            }
            
            Assert.AreEqual(4, counter);

        }

        #endregion

    }

}
