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

using de.ahzf.blueprints;
using de.ahzf.blueprints.Datastructures;

#endregion

namespace de.ahzf.Pipes.UnitTests.FilterPipes
{

    [TestFixture]
    public class RangeFilterPipeTest
    {

        #region testRangeFilterNormal()

        [Test]
        public void testRangeFilterNormal()
        {

            var _Names = new List<String>() { "abe", "bob", "carl", "derick", "evan", "fran" };
            var _Pipe = new RangeFilterPipe<String>(0, 3);
            _Pipe.SetSourceCollection(_Names);

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                var _Name = _Pipe.Current;
                _Counter++;
                Assert.IsTrue(_Name.Equals("abe") || _Name.Equals("bob") || _Name.Equals("carl"));
                Assert.IsFalse(_Name.Equals("derick") || _Name.Equals("evan") || _Name.Equals("fran"));
            }

            Assert.AreEqual(3, _Counter);

        }

        #endregion

        #region testRangeFilterHighInfinity()

        [Test]
        public void testRangeFilterHighInfinity()
        {

            var _Names = new List<String>() { "abe", "bob", "carl", "derick", "evan", "fran" };
            var _Pipe = new RangeFilterPipe<String>(2, -1);
            _Pipe.SetSourceCollection(_Names);

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                var _Name = _Pipe.Current;
                _Counter++;
                Assert.IsTrue(_Name.Equals("carl") || _Name.Equals("derick") || _Name.Equals("evan") || _Name.Equals("fran"));
                Assert.IsFalse(_Name.Equals("abe") || _Name.Equals("bob"));
            }

            Assert.AreEqual(4, _Counter);

        }

        #endregion

        #region testRangeFilterLowInfinity()

        [Test]
        public void testRangeFilterLowInfinity()
        {

            var _Names = new List<String>() { "abe", "bob", "carl", "derick", "evan", "fran" };
            var _Pipe = new RangeFilterPipe<String>(-1, 3);
            _Pipe.SetSourceCollection(_Names);

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                var _Name = _Pipe.Current;
                _Counter++;
                Assert.IsTrue(_Name.Equals("abe") || _Name.Equals("bob") || _Name.Equals("carl"));
                Assert.IsFalse(_Name.Equals("derick") || _Name.Equals("evan") || _Name.Equals("fran"));
            }

            Assert.AreEqual(3, _Counter);

        }

        #endregion

        #region testRangeFilterLowHighInfinity()

        [Test]
        public void testRangeFilterLowHighInfinity()
        {

            var _Names = new List<String>() { "abe", "bob", "carl", "derick", "evan", "fran" };
            var _Pipe = new RangeFilterPipe<String>(-1, -1);
            _Pipe.SetSourceCollection(_Names);

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                var _Name = _Pipe.Current;
                _Counter++;
                Assert.IsTrue(_Name.Equals("abe") || _Name.Equals("bob") || _Name.Equals("carl") || _Name.Equals("derick") || _Name.Equals("evan") || _Name.Equals("fran"));
            }

            Assert.AreEqual(6, _Counter);

        }

        #endregion

        #region testRangeFilterLowEqualsHigh()

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void testRangeFilterLowEqualsHigh()
        {

            var _Names = new List<String>() { "abe", "bob", "carl", "derick", "evan", "fran" };
            var _Pipe = new RangeFilterPipe<String>(2, 2);

        }

        #endregion

        #region testRangeFilterAbsurd()

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void testRangeFilterAbsurd()
        {

            var _Names = new List<String>() { "abe", "bob", "carl", "derick", "evan", "fran" };
            var _Pipe = new RangeFilterPipe<String>(2, 1);

        }

        #endregion

        #region testRangeFilterGraphOneObject()

        [Test]
        public void testRangeFilterGraphOneObject()
        {
            // ./outE[2]/inV/@name

            var _Graph      = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko      = _Graph.GetVertex(new VertexId("1"));
            var _Pipe1      = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _Pipe2      = new RangeFilterPipe<IPropertyEdge>(2, 3);
            var _Pipe3      = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _Pipe4      = new PropertyPipe<VertexId, String, IPropertyVertex, String>("name");
            var _Pipeline   = new Pipeline<IPropertyVertex, String>(_Pipe1, _Pipe2, _Pipe3, _Pipe4);
            _Pipeline.SetSource(new SingleEnumerator<IPropertyVertex>(_Marko));
            
            var _Counter = 0;
            while (_Pipeline.MoveNext())
            {
                _Counter++;
                Assert.AreEqual("lop", _Pipeline.Current);
            }
            
            Assert.AreEqual(1, _Counter);

        }

        #endregion
    
    }

}
