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

using de.ahzf.blueprints.Datastructures;
using de.ahzf.blueprints;

#endregion

namespace de.ahzf.Pipes.UnitTests.Blueprints
{

    [TestFixture]
    public class PropertyPipeTest
    {

        #region testSingleProperty()

        [Test]
        public void testSingleProperty()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko = _Graph.GetVertex(new VertexId("1"));

            var _PPipe = new PropertyPipe<VertexId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                             EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                             HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>("name");
            
            _PPipe.SetSource(new List<IPropertyVertex>() { _Marko }.GetEnumerator());

            var _Counter = 0;
            while (_PPipe.MoveNext())
            {
                var name = _PPipe.Current;
                Assert.AreEqual("marko", name);
                _Counter++;
            }

        }

        #endregion

        #region testMultiProperty()

        [Test]
        public void testMultiProperty()
        {

            var _Graph    = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko    = _Graph.GetVertex(new VertexId("1"));
            
            var _EVP      = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);

            var _PPipe    = new PropertyPipe<VertexId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>("name");

            var _Pipeline = new Pipeline<IPropertyEdge, String>(_EVP, _PPipe);
            _Pipeline.SetSourceCollection(_Marko.OutEdges);

            var _Counter = 0;
            while (_Pipeline.MoveNext())
            {
                var _Name = _Pipeline.Current;
                Assert.IsTrue(_Name.Equals("vadas") || _Name.Equals("josh") || _Name.Equals("lop"));
                _Counter++;
            }

            Assert.AreEqual(3, _Counter);

        }

        #endregion

        #region testListProperty()

        [Test]
        public void testListProperty()
        {

            var _Graph    = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko    = _Graph.GetVertex(new VertexId("1"));
            var _Vadas    = _Graph.GetVertex(new VertexId("2"));

            var _Pipe     = new PropertyPipe<VertexId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>("name");

            var _Pipeline = new Pipeline<IPropertyVertex, String>(_Pipe);
            _Pipeline.SetSource(new List<IPropertyVertex>() { _Marko, _Vadas }.GetEnumerator());

            var _Counter = 0;
            while (_Pipeline.MoveNext())
            {
                var _Name = _Pipeline.Current;
                Assert.IsTrue(_Name.Equals("vadas") || _Name.Equals("marko"));
                _Counter++;
            }

            Assert.AreEqual(2, _Counter);

        }

        #endregion

    }

}
