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

using NUnit.Framework;

using de.ahzf.blueprints.Datastructures;
using de.ahzf.blueprints;
using System;
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes.UnitTests.Blueprints
{

    [TestFixture]
    public class EdgeVertexPipeTest
    {

        #region testInCommingVertex()

        [Test]
        public void testInCommingVertex()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();

            var _Marko = _Graph.GetVertex(new VertexId("1"));

            var _EVP   = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                            EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                            HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);

            _EVP.SetSourceCollection(_Marko.OutEdges);

            var _Counter = 0;
            while (_EVP.MoveNext())
            {
                var v = _EVP.Current;
                Assert.IsTrue(v.Id.Equals(new VertexId("2")) || v.Id.Equals(new VertexId("3")) || v.Id.Equals(new VertexId("4")));
                _Counter++;
            }

            Assert.AreEqual(3, _Counter);


            var _Josh = _Graph.GetVertex(new VertexId("4"));
            
            _EVP = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                      EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                      HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);

            _EVP.SetSource(_Josh.OutEdges.GetEnumerator());

            _Counter = 0;
            while (_EVP.MoveNext())
            {
                var v = _EVP.Current;
                Assert.IsTrue(v.Id.Equals(new VertexId("5")) || v.Id.Equals(new VertexId("3")));
                _Counter++;
            }

            Assert.AreEqual(2, _Counter);

            Assert.IsFalse(_EVP.MoveNext());

        }

        #endregion

        #region testBothVertices()

        [Test]
        public void testBothVertices()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();

            var _Josh  = _Graph.GetVertex(new VertexId("4"));
            IPropertyEdge _TmpEdge = null;

            foreach (var _Edge in _Josh.OutEdges)
            {
                if (_Edge.Id.Equals(new VertexId("11")))
                    _TmpEdge = _Edge;
            }

            var _Pipe = new EdgeVertexPipe(Steps.EdgeVertexStep.BOTH_VERTICES);

            _Pipe.SetSource(new SingleEnumerator<IPropertyEdge>(_TmpEdge));
            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                _Counter++;
                var _Vertex = _Pipe.Current;
                Assert.IsTrue(_Vertex.Id.Equals(new VertexId("4")) || _Vertex.Id.Equals(new VertexId("3")));
            }

            Assert.AreEqual(2, _Counter);

        }

        #endregion

    }

}
