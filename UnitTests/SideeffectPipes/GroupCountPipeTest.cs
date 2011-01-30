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
    public class GroupCountPipeTest
    {

        #region testCountCombinePipeNormal()

        [Test]
        public void testCountCombinePipeNormal()
        {

            var names = new List<String>() { "marko", "josh", "peter", "peter", "peter", "josh" };

            ISideEffectPipe<String, String, IDictionary<String, UInt64>> pipe = new GroupCountPipe<String>();
            pipe.SetSourceCollection(names);

            int counter = 0;
            foreach (var name in pipe)
            {
                Assert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
                counter++;
            }

            Assert.AreEqual(6UL, counter);
            Assert.AreEqual(1UL, pipe.SideEffect["marko"]);
            Assert.AreEqual(2UL, pipe.SideEffect["josh"]);
            Assert.AreEqual(3UL, pipe.SideEffect["peter"]);

            Assert.IsFalse(pipe.SideEffect.ContainsKey("povel"));

        }

        #endregion

        #region testCountCombinePipeZero()

        [Test]
        public void testCountCombinePipeZero()
        {

            var names = new List<String>();
            var pipe  = new GroupCountPipe<String>();
            pipe.SetSourceCollection(names);

            Assert.IsFalse(pipe.MoveNext());
            Assert.IsFalse(pipe.SideEffect.ContainsKey("povel"));

        }

        #endregion

    }

}
