/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

using NUnit.Framework;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.SideeffectPipes
{

    [TestFixture]
    public class AggregatorPipeTests
    {

        #region testAggregatorPipe()

        [Test]
        public void TestAggregatorPipe()
        {

            var list     = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };
            var pipe     = new AggregatorPipe<String>([]);
            pipe.SetSource(list);
            var counter = 0;

            while (pipe.MoveNext())
            {
                Assert.That(list[counter],  Is.EqualTo(pipe.Current));
                counter++;
            }


            Assert.That(counter,  Is.EqualTo(6));
            Assert.That(counter,  Is.EqualTo(pipe.SideEffect.Count));
            Assert.That(counter,  Is.EqualTo(list.Count));

            var pipeContent = pipe.SideEffect.ToArray();

            for (var i = 0; i < counter; i++)
            {
                Assert.That(pipeContent[i], Is.EqualTo(list[i]));
            }

        }

        #endregion

        #region testSelfFilter()

        [Test]
        public void TestSelfFilter()
        {

            var list      = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };

            var pipe1     = new AggregatorPipe<String>([]);
            pipe1.SetSource(list);

            var pipe2     = new CollectionFilterPipe<String>(pipe1.SideEffect, ComparisonFilter.NOT_EQUAL);

            //var pipeline  = new Pipeline<String, String>(_Pipe1, _Pipe2);
            //_Pipeline.SetSource(_List);

            //var _Counter = 0;
            //while (_Pipeline.MoveNext())
            //    _Counter++;

            //ClassicAssert.AreEqual(6, _Counter);


            //_Pipe1    = new AggregatorPipe<String>(new List<String>());
            //_Pipe2    = new CollectionFilterPipe<String>(_Pipe1.SideEffect, ComparisonFilter.EQUAL);
            //_Pipeline = new Pipeline<String, String>(_Pipe1, _Pipe2);
            //_Pipeline.SetSourceCollection(_List);

            //_Counter = 0;
            //while (_Pipeline.MoveNext())
            //    _Counter++;

            //ClassicAssert.AreEqual(0, _Counter);

        }

        #endregion

        #region testNullIterator()

        [Test]
        public void TestNullIterator()
        {

            var list        = new List<String?>() { "marko", "antonio", "rodriguez", "was", "here", "." };
            var enumerator  = list.GetEnumerator();
            var counter     = 0;

            while (enumerator.MoveNext())
                counter++;

            Assert.That(counter,                Is.EqualTo(6));
            Assert.That(enumerator.MoveNext(),  Is.False);


            list        = [ null, null, null, null, null, null ];
            enumerator  = list.GetEnumerator();
            counter     = 0;

            while (enumerator.MoveNext())
                counter++;

            Assert.That(counter,                Is.EqualTo(6));
            Assert.That(enumerator.MoveNext(),  Is.False);

        }

        #endregion

    }

}
