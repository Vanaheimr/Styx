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

using de.ahzf.blueprints.Datastructures;
using de.ahzf.blueprints;
using System;

#endregion

namespace de.ahzf.Pipes.UnitTests.Blueprints
{

    [TestFixture]
    public class VertexIdFilterPipeTest
    {

        #region testFilterIds1()

        [Test]
        public void testFilterIds1()
        {

            var _Graph    = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko    = _Graph.GetVertex(new VertexId("1"));

            var _Pipe1    = new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);

            var _Pipe2    = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);

            var _Pipe3    = new VertexIdFilterPipe(new VertexId("3"), ComparisonFilter.NOT_EQUAL);

            var _Pipeline = new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                         EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                         HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>,

                                         IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                         EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                         HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_Pipe1, _Pipe2, _Pipe3);

            _Pipeline.SetSourceCollection(new List<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>() { _Marko });

            var _Counter = 0;
            while (_Pipeline.MoveNext())
            {
                var _Vertex = _Pipeline.Current;
                Assert.AreEqual("lop", _Vertex.GetProperty("name"));
                _Counter++;
            }

            Assert.AreEqual(1, _Counter);

        }

        #endregion

        #region testFilterIds2()

        [Test]
        public void testFilterIds2()
        {

            var _Graph    = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko    = _Graph.GetVertex(new VertexId("1"));

            var _Pipe1    = new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);
            
            var _Pipe2    = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);
            
            var _Pipe3    = new VertexIdFilterPipe(new VertexId("3"), ComparisonFilter.EQUAL);

            var _Pipeline = new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                         EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                         HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>,

                                         IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                         EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                         HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_Pipe1, _Pipe2, _Pipe3);

            _Pipeline.SetSourceCollection(new List<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>() { _Marko });

            var _Counter = 0;
            while (_Pipeline.MoveNext())
            {
                var _Vertex = _Pipeline.Current;
                Assert.IsTrue(_Vertex.GetProperty("name").Equals("vadas") || _Vertex.GetProperty("name").Equals("josh"));
                _Counter++;
            }
            Assert.AreEqual(2, _Counter);

        }

        #endregion

    }

}
