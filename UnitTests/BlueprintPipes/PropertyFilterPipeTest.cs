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
    public class PropertyFilterPipeTest
    {

        #region testPropertyFilter()

        [Test]
        public void testPropertyFilter()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();
            var marko = graph.GetVertex(new VertexId("1"));

            var pipe1    = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var pipe2    = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var pipe3    = new PropertyFilterPipe<IVertex, String>("lang", "java", ComparisonFilter.NOT_EQUAL);
            var pipeline = new Pipeline<IVertex, IVertex>(new List<IPipe>() { pipe1, pipe2, pipe3});
            pipeline.SetSource(new List<IVertex>() { marko }.GetEnumerator());

            int counter = 0;
            while (pipeline.MoveNext())
            {
                counter++;
                var vertex = pipeline.Current;
                Assert.AreEqual(new VertexId("3"), vertex.Id);
                Assert.AreEqual("java", vertex.GetProperty("lang"));
                Assert.AreEqual("lop", vertex.GetProperty("name"));
            }

            Assert.AreEqual(1, counter);

        }

        #endregion

    }

}
