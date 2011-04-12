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
using System.Linq;
using System.Collections.Generic;

using de.ahzf.blueprints;
using de.ahzf.blueprints.Datastructures;

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.util
{

    [TestFixture]
    public class GatherPipeTest
    {

        #region testBasicGather()

        [Test]
        public void testBasicGather()
        {

            var _Graph    = TinkerGraphFactory.CreateTinkerGraph();

            var _Pipe0    = new VertexEdgePipe<VertexId,    RevisionId, String, Object,
                                               EdgeId,      RevisionId, String, Object,
                                               HyperEdgeId, RevisionId, String, Object>(Steps.VertexEdgeStep.OUT_EDGES);
            
            var _Pipe1    = new EdgeVertexPipe<VertexId,    RevisionId, String, Object,
                                               EdgeId,      RevisionId, String, Object,
                                               HyperEdgeId, RevisionId, String, Object>(Steps.EdgeVertexStep.IN_VERTEX);

            var _Pipe2    = new GatherPipe<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                           EdgeId,      RevisionId, String, Object,
                                                           HyperEdgeId, RevisionId, String, Object>>();

            var _Pipeline = new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                         EdgeId,      RevisionId, String, Object,
                                                         HyperEdgeId, RevisionId, String, Object>,

                                         IEnumerable<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                                     EdgeId,      RevisionId, String, Object,
                                                                     HyperEdgeId, RevisionId, String, Object>>>(_Pipe0, _Pipe1, _Pipe2);

            _Pipeline.SetSource(new SingleEnumerator<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                                     EdgeId,      RevisionId, String, Object,
                                                                     HyperEdgeId, RevisionId, String, Object>>(_Graph.GetVertex(new VertexId("1"))));

            while (_Pipeline.MoveNext())
                Console.WriteLine(_Pipeline.Current + "--->");// + pipeline.Path);

        }

        #endregion

    }

}
