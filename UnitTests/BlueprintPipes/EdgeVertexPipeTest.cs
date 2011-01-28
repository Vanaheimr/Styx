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

namespace de.ahzf.Pipes.UnitTests
{

    [TestFixture]
    public class EdgeVertexPipeTest
    {

        #region testInCommingVertex()

        [Test]
        public void testInCommingVertex()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            
            var marko = graph.GetVertex(new VertexId("1"));
            var evp = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            evp.SetSourceCollection(marko.OutEdges);
            //Assert.IsTrue(evp.MoveNext());
            int counter = 0;
            while (evp.MoveNext())
            {
                var v = evp.Current;
                Assert.IsTrue(v.Id.Equals(new VertexId("2")) || v.Id.Equals(new VertexId("3")) || v.Id.Equals(new VertexId("4")));
                counter++;
            }
            Assert.AreEqual(3, counter);


            var josh = graph.GetVertex(new VertexId("4"));
            evp = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            evp.SetSource(josh.OutEdges.GetEnumerator());
            //Assert.IsTrue(evp.hasNext());
            counter = 0;
            while (evp.MoveNext())
            {
                var v = evp.Current;
                Assert.IsTrue(v.Id.Equals(new VertexId("5")) || v.Id.Equals(new VertexId("3")));
                counter++;
            }
            Assert.AreEqual(2, counter);

            Assert.IsFalse(evp.MoveNext());

        }

        #endregion

        #region testBothVertices()

        [Test]
        public void testBothVertices()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();


            var josh = graph.GetVertex(new VertexId("4"));
            IEdge tempEdge = null;

            foreach (var edge in josh.OutEdges)
            {
                if (edge.Id.Equals(new VertexId("11")))
                    tempEdge = edge;
            }

            var pipe = new EdgeVertexPipe(EdgeVertexPipe.Step.BOTH_VERTICES);

            pipe.SetSource(new SingleEnumerator<IEdge>(tempEdge));
            int counter = 0;
            while (pipe.MoveNext())
            {
                counter++;
                var vertex = pipe.Current;
                Assert.IsTrue(vertex.Id.Equals(new VertexId("4")) || vertex.Id.Equals(new VertexId("3")));
            }

            Assert.AreEqual(2, counter);

        }

        #endregion

    }

}
