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

namespace de.ahzf.Pipes.UnitTests.Blueprints
{

    [TestFixture]
    public class VertexEdgePipeTest
    {

        #region testOutGoingEdges()

        [Test]
        public void testOutGoingEdges()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();
            var marko = graph.GetVertex(new VertexId("1"));

            var vsf = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            vsf.SetSource(new List<IVertex>() { marko }.GetEnumerator());

            int counter = 0;
            while (vsf.MoveNext())
            {
                var e = vsf.Current;
                Assert.AreEqual(marko, e.OutVertex);
                Assert.IsTrue(e.InVertex.Id.Equals(new VertexId("2")) || e.InVertex.Id.Equals(new VertexId("3")) || e.InVertex.Id.Equals(new VertexId("4")));
                counter++;
            }

            Assert.AreEqual(3, counter);
            

            
            var josh = graph.GetVertex(new VertexId("4"));
            vsf = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            vsf.SetSource(new List<IVertex>() { josh }.GetEnumerator());

            counter = 0;
            while (vsf.MoveNext())
            {
                var e = vsf.Current;
                Assert.AreEqual(josh, e.OutVertex);
                Assert.IsTrue(e.InVertex.Id.Equals(new VertexId("5")) || e.InVertex.Id.Equals(new VertexId("3")));
                counter++;
            }

            Assert.AreEqual(2, counter);



            var lop = graph.GetVertex(new VertexId("3"));
            vsf = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            vsf.SetSource(new List<IVertex>() { lop }.GetEnumerator());

            counter = 0;
            while (vsf.MoveNext())
            {
                counter++;
            }

            Assert.AreEqual(0, counter);

        }

        #endregion

        #region testInEdges()

        [Test]
        public void testInEdges()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();
            var josh = graph.GetVertex(new VertexId("4"));

            var pipe = new VertexEdgePipe(VertexEdgePipe.Step.IN_EDGES);
            pipe.SetSource(new SingleEnumerator<IVertex>(josh));
            int counter = 0;
            while (pipe.MoveNext())
            {
                counter++;
                var edge = pipe.Current;
                Assert.AreEqual(new EdgeId("8"), edge.Id);
            }
            
            Assert.AreEqual(1, counter);

        }

        #endregion

        #region testBothEdges()

        [Test]
        public void testBothEdges()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();
            var josh = graph.GetVertex(new VertexId("4"));

            var pipe = new VertexEdgePipe(VertexEdgePipe.Step.BOTH_EDGES);
            pipe.SetSource(new SingleEnumerator<IVertex>(josh));

            int counter = 0;
            while (pipe.MoveNext())
            {
                counter++;
                var edge = pipe.Current;
                Assert.IsTrue(edge.Id.Equals(new EdgeId("8")) || edge.Id.Equals(new EdgeId("10")) || edge.Id.Equals(new EdgeId("11")));
            }

            Assert.AreEqual(3, counter);

        }

        #endregion

        #region testBigGraphWithNoEdges()

        [Test]
        public void testBigGraphWithNoEdges()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            for (int i = 0; i < 100000; i++)
                graph.AddVertex(null);

            var vertices = new GraphElementPipe<IVertex>(GraphElementPipe<IVertex>.ElementType.VERTEX);
            vertices.SetSource(new SingleEnumerator<IGraph>(graph));
            var outEdges = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            outEdges.SetSource(vertices);

            int counter = 0;
            while (outEdges.MoveNext())
            {
                counter++;
            }
            
            Assert.AreEqual(0, counter);

        }

        #endregion

    }

}
