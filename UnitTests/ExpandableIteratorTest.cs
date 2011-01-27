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
    public class ExpandableIteratorTest
    {

        #region testIdentityPipeNormal()

        [Test]
        public void testIdentityPipeNormal()
        {

            var itty = new ExpandableEnumerator<Int32>((new List<Int32>() { 1, 2, 3 }).GetEnumerator());

            Assert.IsTrue(itty.MoveNext());
            Assert.AreEqual(1, itty.Current);

            itty.Add(4);
            Assert.IsTrue(itty.MoveNext());
            Assert.AreEqual(4, itty.Current);
            itty.Add(5);
            itty.Add(6);
            Assert.IsTrue(itty.MoveNext());
            Assert.AreEqual(5, itty.Current);
            Assert.IsTrue(itty.MoveNext());
            Assert.AreEqual(6, itty.Current);
            Assert.IsTrue(itty.MoveNext());
            Assert.AreEqual(2, itty.Current);
            Assert.IsTrue(itty.MoveNext());
            Assert.AreEqual(3, itty.Current);
            Assert.IsFalse(itty.MoveNext());

        }
        #endregion

    }

}
