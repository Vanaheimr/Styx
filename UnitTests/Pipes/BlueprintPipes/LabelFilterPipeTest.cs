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

using NUnit.Framework;

using de.ahzf.blueprints.Datastructures;
using System;
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes.UnitTests.Blueprints
{

    [TestFixture]
    public class LabelFilterPipeTest
    {

        #region testFilterLabels()

        [Test]
        public void testFilterLabels()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();

            var _Marko = _Graph.GetVertex(new VertexId("1"));

            var _LFP   = new LabelFilterPipe<VertexId,    RevisionId, String, Object,
                                             EdgeId,      RevisionId, String, Object,
                                             HyperEdgeId, RevisionId, String, Object>("knows", ComparisonFilter.NOT_EQUAL);

            _LFP.SetSourceCollection(_Marko.OutEdges);

            var _Counter = 0;
            while (_LFP.MoveNext())
            {
                var _E = _LFP.Current;
                Assert.AreEqual(_Marko, _E.OutVertex);
                Assert.IsTrue(_E.InVertex.Id.Equals(new VertexId("2")) || _E.InVertex.Id.Equals(new VertexId("4")));
                _Counter++;
            }

            Assert.AreEqual(2, _Counter);


            _LFP = new LabelFilterPipe<VertexId,    RevisionId, String, Object,
                                       EdgeId,      RevisionId, String, Object,
                                       HyperEdgeId, RevisionId, String, Object>("knows", ComparisonFilter.EQUAL);

            _LFP.SetSourceCollection(_Marko.OutEdges);

            _Counter = 0;
            while (_LFP.MoveNext())
            {
                var _E = _LFP.Current;
                Assert.AreEqual(_Marko, _E.OutVertex);
                Assert.IsTrue(_E.InVertex.Id.Equals(new VertexId("3")));
                _Counter++;
            }

            Assert.AreEqual(1, _Counter);

        }

        #endregion

    }

}
