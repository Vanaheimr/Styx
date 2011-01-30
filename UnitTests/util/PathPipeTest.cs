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
using System.Linq;
using System.Collections.Generic;

using de.ahzf.blueprints;
using de.ahzf.blueprints.Datastructures;

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.util
{

    [TestFixture]
    public class PathPipeTest
    {

        #region testPipeBasic()

        [Test]
        public void testPipeBasic()
        {

            var graph    = TinkerGraphFactory.CreateTinkerGraph();

            var marko    = graph.GetVertex(new VertexId("1"));
            var pipe1    = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var pipe2    = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var pipe3    = new PathPipe<IVertex>();
            var pipeline = new Pipeline<IVertex, List<Object>>(pipe1, pipe2, pipe3);
            pipeline.SetSource(new SingleEnumerator<IVertex>(marko));

            foreach (var _Path in pipeline)
            {
                Assert.AreEqual(marko, _Path[0]);
                Assert.IsTrue(_Path[1] is IEdge);
                Assert.IsTrue(_Path[2] is IVertex);
            }

        }

        #endregion

    }

}
