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
using System.Collections.Generic;

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.SideeffectPipes
{

    [TestFixture]
    public class CountPipeTest
    {

        #region testCountPipeNormal()

        [Test]
        public void testCountPipeNormal()
        {

            var list = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };

            var pipe1 = new CountPipe<String>();
            pipe1.SetSourceCollection(list);

            var counter = 0UL;
            while (pipe1.MoveNext())
            {
                var s = pipe1.Current;
                Assert.IsTrue(s.Equals("marko") || s.Equals("antonio") || s.Equals("rodriguez") || s.Equals("was") || s.Equals("here") || s.Equals("."));
                counter++;
                Assert.AreEqual(counter, pipe1.SideEffect);
            }
            
            Assert.AreEqual(6UL, pipe1.SideEffect);

        }

        #endregion

        #region testCountPipeZero()

        [Test]
        public void testCountPipeZero()
        {

            var list  = new List<String>();
            var pipe1 = new CountPipe<String>();
            pipe1.SetSourceCollection(list);

            while (pipe1.MoveNext())
            { }

            Assert.AreEqual(0UL, pipe1.SideEffect);

        }

        #endregion

    }

}
