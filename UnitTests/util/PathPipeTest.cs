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

            var _Graph    = TinkerGraphFactory.CreateTinkerGraph();

            var _Marko    = _Graph.GetVertex(new VertexId("1"));
            var _Pipe1    = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _Pipe2    = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _Pipe3    = new PathPipe<IVertex>();
            var _Pipeline = new Pipeline<IVertex, IEnumerable<Object>>(_Pipe1, _Pipe2, _Pipe3);
            _Pipeline.SetSource(new SingleEnumerator<IVertex>(_Marko));

            foreach (var _Path in _Pipeline)
            {
                Assert.AreEqual(_Marko, _Path.ElementAt(0));
                Assert.IsTrue(_Path.ElementAt(1) is IEdge);
                Assert.IsTrue(_Path.ElementAt(2) is IVertex);
            }

        }

        #endregion

    }

}
