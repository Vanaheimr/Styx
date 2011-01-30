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

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.SideeffectPipes
{

    [TestFixture]
    public class SideEffectCapPipeTest
    {

        #region testSideEffectCapPipeNormalCount()

        [Test]
        public void testSideEffectCapPipeNormalCount()
        {

            var list = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };
            
            var pipe = new SideEffectCapPipe<String, UInt64>(new CountPipe<String>());
            pipe.SetSourceCollection(list);

            Assert.IsTrue(pipe.MoveNext());
            Assert.AreEqual(6UL, pipe.Current);
            Assert.IsFalse(pipe.MoveNext());

        }

        #endregion

        #region testSideEffectCapPipeZeroCount()

        [Test]
        public void testSideEffectCapPipeZeroCount()
        {

            var list = new List<String>();
            var pipe = new SideEffectCapPipe<String, UInt64>(new CountPipe<String>());
            pipe.SetSourceCollection(list);

            Assert.IsTrue(pipe.MoveNext());
            Assert.AreEqual(0UL, pipe.Current);
            Assert.IsFalse(pipe.MoveNext());

        }

        #endregion

    }

}
