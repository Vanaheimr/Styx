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
    public class GatherPipeTest
    {

        #region testBasicGather()

        [Test]
        public void testBasicGather()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            var pipe0    = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var pipe1    = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var pipe2    = new GatherPipe<IVertex>();
            var pipeline = new Pipeline<IVertex, IEnumerable<IVertex>>(pipe0, pipe1, pipe2);
            pipeline.SetSource(new SingleEnumerator<IVertex>(graph.GetVertex(new VertexId("1"))));

            while (pipeline.MoveNext())
            {
                Console.WriteLine(pipeline.Current + "--->");// + pipeline.Path);
            }

        }

        #endregion

    }

}
