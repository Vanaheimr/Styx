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
    public class ScatterPipeTest
    {

        #region testScatterPipe()

        [Test]
        public void testScatterPipe()
        {

            var scatter = new ScatterPipe<Int32, Int32>();
            scatter.SetSourceCollection(new List<Int32>() { 1, 2, 3 });

            int counter = 0;
            while (scatter.MoveNext())
            {
                var _Object = scatter.Current;
                Assert.IsTrue(_Object.Equals(1) || _Object.Equals(2) || _Object.Equals(3));
                counter++;
            }

            Assert.AreEqual(3, counter);

        }

        #endregion

        #region testScatterPipeComplex()

        [Test]
        public void testScatterPipeComplex()
        {

            var scatter = new ScatterPipe<Object, Int32>();
            scatter.SetSourceCollection(new List<Object>() { 1, 2, new List<Object>() { 3, 4 }, 5, 6 });

            int counter = 0;
            while (scatter.MoveNext())
            {
                var _Object = scatter.Current;
                Assert.IsTrue(_Object.Equals(1) || _Object.Equals(2) || _Object.Equals(3) || _Object.Equals(4) || _Object.Equals(5) || _Object.Equals(6));
                counter++;
            }

            Assert.AreEqual(6, counter);

        }

        #endregion

    }

}
