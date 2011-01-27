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

namespace de.ahzf.Pipes.UnitTests
{

    [TestFixture]
    public class MultiIteratorTest
    {

        #region testMultiIteratorSimple()

        [Test]
        public void testMultiIteratorSimple()
        {

            var a       = new List<Int32>() {1, 2}.GetEnumerator();
            var b       = new List<Int32>() { 3, 4, 5 }.GetEnumerator();
            var c       = new List<Int32>() { 6, 7, 8 }.GetEnumerator();

            var itty    = new MultiEnumerator<Int32>(a, b, c);
            var counter = 0;
            
            while (itty.MoveNext())
            {
                counter++;
                Assert.AreEqual(counter, itty.Current);
            }

            Assert.AreEqual(counter, 8);

        }

        #endregion

        #region testMultiIteratorNoParameters()

        [Test]
        public void testMultiIteratorNoParameters()
        {

            var itty    = new MultiEnumerator<Int32>();
            int counter = 0;

            while (itty.MoveNext())
            {
                counter++;
            }

            Assert.AreEqual(0, counter);

        }

        #endregion

    }

}
