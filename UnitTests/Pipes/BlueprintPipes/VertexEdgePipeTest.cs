/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/pipes.NET>
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

using de.ahzf.blueprints;
using de.ahzf.blueprints.PropertyGraph;

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.Blueprints
{

    [TestFixture]
    public class VertexEdgePipeTest
    {

        #region testOutGoingEdges()

        [Test]
        public void testOutGoingEdges()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko = _Graph.GetVertex(new VertexId("1"));

            var _VSF   = new VertexEdgePipe<VertexId,    RevisionId, String, Object,
                                            EdgeId,      RevisionId, String, Object,
                                            HyperEdgeId, RevisionId, String, Object>(Steps.VertexEdgeStep.OUT_EDGES);

            _VSF.SetSource(new List<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                    EdgeId,      RevisionId, String, Object,
                                                    HyperEdgeId, RevisionId, String, Object>>() { _Marko }.GetEnumerator());

            var _Counter = 0;
            while (_VSF.MoveNext())
            {
                var _E = _VSF.Current;
                Assert.AreEqual(_Marko, _E.OutVertex);
                Assert.IsTrue(_E.InVertex.Id.Equals(new VertexId("2")) || _E.InVertex.Id.Equals(new VertexId("3")) || _E.InVertex.Id.Equals(new VertexId("4")));
                _Counter++;
            }

            Assert.AreEqual(3, _Counter);
            

            
            var _Josh = _Graph.GetVertex(new VertexId("4"));

            _VSF = new VertexEdgePipe<VertexId,    RevisionId, String, Object,
                                      EdgeId,      RevisionId, String, Object,
                                      HyperEdgeId, RevisionId, String, Object>(Steps.VertexEdgeStep.OUT_EDGES);

            _VSF.SetSource(new List<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                    EdgeId,      RevisionId, String, Object,
                                                    HyperEdgeId, RevisionId, String, Object>>() { _Josh }.GetEnumerator());

            _Counter = 0;
            while (_VSF.MoveNext())
            {
                var e = _VSF.Current;
                Assert.AreEqual(_Josh, e.OutVertex);
                Assert.IsTrue(e.InVertex.Id.Equals(new VertexId("5")) || e.InVertex.Id.Equals(new VertexId("3")));
                _Counter++;
            }

            Assert.AreEqual(2, _Counter);



            var _Lop = _Graph.GetVertex(new VertexId("3"));

            _VSF = new VertexEdgePipe<VertexId,    RevisionId, String, Object,
                                      EdgeId,      RevisionId, String, Object,
                                      HyperEdgeId, RevisionId, String, Object>(Steps.VertexEdgeStep.OUT_EDGES);

            _VSF.SetSource(new List<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                    EdgeId,      RevisionId, String, Object,
                                                    HyperEdgeId, RevisionId, String, Object>>() { _Lop }.GetEnumerator());

            _Counter = 0;
            while (_VSF.MoveNext())
            {
                _Counter++;
            }

            Assert.AreEqual(0, _Counter);

        }

        #endregion

        #region testInEdges()

        [Test]
        public void testInEdges()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();
            var _Josh  = _Graph.GetVertex(new VertexId("4"));
            
            var _Pipe = new VertexEdgePipe<VertexId,    RevisionId, String, Object,
                                           EdgeId,      RevisionId, String, Object,
                                           HyperEdgeId, RevisionId, String, Object>(Steps.VertexEdgeStep.IN_EDGES);

            _Pipe.SetSource(new SingleEnumerator<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                                 EdgeId,      RevisionId, String, Object,
                                                                 HyperEdgeId, RevisionId, String, Object>>(_Josh));

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                _Counter++;
                var edge = _Pipe.Current;
                Assert.AreEqual(new EdgeId("8"), edge.Id);
            }
            
            Assert.AreEqual(1, _Counter);

        }

        #endregion

        #region testBothEdges()

        [Test]
        public void testBothEdges()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();
            var _Josh  = _Graph.GetVertex(new VertexId("4"));

            var _Pipe = new VertexEdgePipe<VertexId,    RevisionId, String, Object,
                                           EdgeId,      RevisionId, String, Object,
                                           HyperEdgeId, RevisionId, String, Object>(Steps.VertexEdgeStep.BOTH_EDGES);

            _Pipe.SetSource(new SingleEnumerator<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                                 EdgeId,      RevisionId, String, Object,
                                                                 HyperEdgeId, RevisionId, String, Object>>(_Josh));

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                _Counter++;
                var edge = _Pipe.Current;
                Assert.IsTrue(edge.Id.Equals(new EdgeId("8")) || edge.Id.Equals(new EdgeId("10")) || edge.Id.Equals(new EdgeId("11")));
            }

            Assert.AreEqual(3, _Counter);

        }

        #endregion

        #region testBigGraphWithNoEdges()

        [Test]
        public void testBigGraphWithNoEdges()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();

            for (var i = 0; i < 100000; i++)
                _Graph.AddVertex(null);

            var _Vertices = new GraphElementPipe<VertexId, RevisionId, String, Object, IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                                                                       EdgeId,      RevisionId, String, Object,
                                                                                                       HyperEdgeId, RevisionId, String, Object>>(Steps.ElementType.VERTEX);

            _Vertices.SetSource(new SingleEnumerator<IPropertyGraph>(_Graph));

            var _OutEdges = new VertexEdgePipe<VertexId,    RevisionId, String, Object,
                                               EdgeId,      RevisionId, String, Object,
                                               HyperEdgeId, RevisionId, String, Object>(Steps.VertexEdgeStep.OUT_EDGES);

            _OutEdges.SetSource(_Vertices);

            var _Counter = 0;
            while (_OutEdges.MoveNext())
                _Counter++;
            
            Assert.AreEqual(0, _Counter);

        }

        #endregion

    }

}
