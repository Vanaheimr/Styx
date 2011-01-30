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

#endregion

namespace de.ahzf.Pipes.UnitTests
{

    [TestFixture]
    public class IdFilterPipeTest
    {

        #region testIdEdgePipeGraph()

        [Test]
        public void testIdEdgePipeGraph()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            IVertex marko = graph.GetVertex(new VertexId("1"));

            var pipe1 = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var pipe2 = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var pipe3 = new VertexIdFilterPipe(new VertexId("3"), ComparisonFilter.NOT_EQUAL);

            var pipeline = new Pipeline<IVertex, IVertex>(pipe1, pipe2, pipe3);
            pipeline.SetSourceCollection(new List<IVertex>() { marko });

            int counter = 0;
            while (pipeline.MoveNext())
            {
                var vertex = pipeline.Current;
                Assert.AreEqual("lop", vertex.GetProperty("name"));
                counter++;
            }

            Assert.AreEqual(1, counter);

        }

        #endregion

    }

}
