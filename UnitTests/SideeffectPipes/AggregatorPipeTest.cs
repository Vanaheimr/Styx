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
    public class AggregatorPipeTest
    {

        #region testAggregatorPipe()

        [Test]
        public void testAggregatorPipe()
        {

            var list = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };
            
            var pipe1 = new AggregatorPipe<String>(new List<String>());
            pipe1.SetSourceCollection(list);

            int counter = 0;
            while (pipe1.MoveNext())
            {
                Assert.AreEqual(list[counter], pipe1.Current);
                counter++;
            }

            Assert.AreEqual(6, counter);
            Assert.AreEqual(counter, pipe1.SideEffect.Count);
            Assert.AreEqual(counter, list.Count);
            
            for (int i = 0; i < counter; i++)
            {
                Assert.AreEqual(pipe1.SideEffect.ToArray()[i], list[i]);
            }

        }

        #endregion

        #region testSelfFilter()

        [Test]
        public void testSelfFilter()
        {

            var list = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };

            var pipe1    = new AggregatorPipe<String>(new List<String>());
            var pipe2    = new CollectionFilterPipe<String>(pipe1.SideEffect, ComparisonFilter.NOT_EQUAL);
            var pipeline = new Pipeline<String, String>(pipe1, pipe2);
            pipeline.SetSourceCollection(list);

            int counter = 0;
            while (pipeline.MoveNext())
            {
                counter++;
            }

            Assert.AreEqual(6, counter);


            pipe1    = new AggregatorPipe<String>(new List<String>());
            pipe2    = new CollectionFilterPipe<String>(pipe1.SideEffect, ComparisonFilter.EQUAL);
            pipeline = new Pipeline<String, String>(pipe1, pipe2);
            pipeline.SetSourceCollection(list);

            counter = 0;
            while (pipeline.MoveNext())
            {
                counter++;
            }

            Assert.AreEqual(0, counter);

        }

        #endregion

        #region testNullIterator()

        [Test]
        public void testNullIterator()
        {

            var list = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };
            IEnumerator<String> itty = list.GetEnumerator();

            int counter = 0;
            while (itty.MoveNext())
            {
                counter++;
            }
            
            Assert.AreEqual(6, counter);
            Assert.IsFalse(itty.MoveNext());


            list = new List<String>() { null, null, null, null, null, null };
            itty = list.GetEnumerator();

            counter = 0;
            while (itty.MoveNext())
            {
                counter++;
            }

            Assert.AreEqual(6, counter);
            Assert.IsFalse(itty.MoveNext());

        }

        #endregion

    }

}
