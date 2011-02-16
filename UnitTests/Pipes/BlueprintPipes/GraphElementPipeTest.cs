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

using System.Collections.Generic;

using NUnit.Framework;

using de.ahzf.blueprints;

#endregion

namespace de.ahzf.Pipes.UnitTests.Blueprints
{

    [TestFixture]
    public class GraphElementPipeTest
    {

        #region testVertexIterator()

        [Test]
        public void testVertexIterator()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();
            var _Pipe  = new GraphElementPipe<IVertex>(GraphElementPipe<IVertex>.ElementType.VERTEX);
            _Pipe.SetSource(new SingleEnumerator<IGraph>(_Graph));
            
            var _Counter = 0;
            var _Vertices = new HashSet<IVertex>();

            while (_Pipe.MoveNext())
            {
                _Counter++;
                var _Vertex = _Pipe.Current;
                _Vertices.Add(_Vertex);
            }

            Assert.AreEqual(6, _Counter);
            Assert.AreEqual(6, _Vertices.Count);

        }

        #endregion

        #region testEdgeIterator()

        [Test]
        public void testEdgeIterator()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();
            var _Pipe = new GraphElementPipe<IEdge>(GraphElementPipe<IEdge>.ElementType.EDGE);
            _Pipe.SetSource(new SingleEnumerator<IGraph>(_Graph));

            var _Counter = 0;
            var _Edges = new HashSet<IEdge>();

            while (_Pipe.MoveNext())
            {
                _Counter++;
                var _Edge = _Pipe.Current;
                _Edges.Add(_Edge);
            }

            Assert.AreEqual(6, _Counter);
            Assert.AreEqual(6, _Edges.Count);

        }

        #endregion

        #region testEdgeIteratorThreeGraphs()

        [Test]
        public void testEdgeIteratorThreeGraphs()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();
            var _Pipe  = new GraphElementPipe<IEdge>(GraphElementPipe<IEdge>.ElementType.EDGE);
            _Pipe.SetSourceCollection(new List<IGraph>() { _Graph, _Graph, _Graph });
            
            var _Counter = 0;
            var _Edges   = new HashSet<IEdge>();

            while (_Pipe.MoveNext())
            {
                _Counter++;
                var _Edge = _Pipe.Current;
                _Edges.Add(_Edge);
            }

            Assert.AreEqual(18, _Counter);
            Assert.AreEqual(6, _Edges.Count);

        }

        #endregion

    }

}
