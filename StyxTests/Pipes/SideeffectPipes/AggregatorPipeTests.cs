﻿/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.SideeffectPipes
{

    [TestFixture]
    public class AggregatorPipeTests
    {

        #region testAggregatorPipe()

        [Test]
        public void testAggregatorPipe()
        {

            //var _List = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };            
            //var _Pipe = new AggregatorPipe<String>(new List<String>());
            //_Pipe.SetSourceCollection(_List);

            //int _Counter = 0;
            //while (_Pipe.MoveNext())
            //{
            //    ClassicAssert.AreEqual(_List[_Counter], _Pipe.Current);
            //    _Counter++;
            //}

            //ClassicAssert.AreEqual(6, _Counter);
            //ClassicAssert.AreEqual(_Counter, _Pipe.SideEffect.Count);
            //ClassicAssert.AreEqual(_Counter, _List.Count);
            
            //for (int i = 0; i < _Counter; i++)
            //{
            //    ClassicAssert.AreEqual(_Pipe.SideEffect.ToArray()[i], _List[i]);
            //}

        }

        #endregion

        #region testSelfFilter()

        [Test]
        public void testSelfFilter()
        {

            //var _List     = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };
            //var _Pipe1    = new AggregatorPipe<String>(new List<String>());
            //var _Pipe2    = new CollectionFilterPipe<String>(_Pipe1.SideEffect, ComparisonFilter.NOT_EQUAL);
            //var _Pipeline = new Pipeline<String, String>(_Pipe1, _Pipe2);
            //_Pipeline.SetSourceCollection(_List);

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
        public void testNullIterator()
        {

            var _List = new List<String>() { "marko", "antonio", "rodriguez", "was", "here", "." };
            IEnumerator<String> _Enumerator = _List.GetEnumerator();

            var _Counter = 0;
            while (_Enumerator.MoveNext())
                _Counter++;

            ClassicAssert.AreEqual(6, _Counter);
            ClassicAssert.IsFalse(_Enumerator.MoveNext());


            _List = new List<String>() { null, null, null, null, null, null };
            _Enumerator = _List.GetEnumerator();

            _Counter = 0;
            while (_Enumerator.MoveNext())
                _Counter++;

            ClassicAssert.AreEqual(6, _Counter);
            ClassicAssert.IsFalse(_Enumerator.MoveNext());

        }

        #endregion

    }

}
