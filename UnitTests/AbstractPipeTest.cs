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

#endregion

namespace de.ahzf.Pipes.UnitTests
{

    [TestFixture]
    public class AbstractPipeTest
    {

        [Test]
        public void TestIEnumerator()
        {
            
            var names = new List<String>() { "marko", "josh", "peter" };

            IPipe<String, String> pipe = new IdentityPipe<String>();
            pipe.SetStarts(names);

            var counter = 0UL;
            while (pipe.MoveNext())
            {
                counter++;
                String name = pipe.Current;
                Assert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
            }
            
            Assert.AreEqual(counter, 3UL);
            pipe.SetStarts(names);
            counter = 0UL;
            
            foreach (var name in pipe)
            {
                Assert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
                counter++;
            }
            
            Assert.AreEqual(counter, 3UL);

        }

        //public void testPathConstruction()
        //{
        //    Graph graph = TinkerGraphFactory.createTinkerGraph();
        //    Vertex marko = graph.getVertex("1");
        //    Pipe<Vertex, Edge> pipe1 = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
        //    Pipe<Edge, Vertex> pipe2 = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
        //    Pipe<Vertex, String> pipe3 = new PropertyPipe<Vertex, String>("name");
        //    pipe3.setStarts(pipe2.iterator());
        //    pipe2.setStarts(pipe1.iterator());
        //    pipe1.setStarts(Arrays.asList(marko).iterator());

        //    for (String name : pipe3) {
        //        List path = pipe3.getPath();
        //        assertEquals(path.get(0), marko);
        //        assertEquals(path.get(1).getClass(), TinkerEdge.class);
        //        assertEquals(path.get(2).getClass(), TinkerVertex.class);
        //        assertEquals(path.get(3).getClass(), String.class);
        //        if (name.equals("vadas")) {
        //            assertEquals(path.get(1), graph.getEdge(7));
        //            assertEquals(path.get(2), graph.getVertex(2));
        //            assertEquals(path.get(3), "vadas");
        //        } else if (name.equals("lop")) {
        //            assertEquals(path.get(1), graph.getEdge(9));
        //            assertEquals(path.get(2), graph.getVertex(3));
        //            assertEquals(path.get(3), "lop");
        //        } else if (name.equals("josh")) {
        //            assertEquals(path.get(1), graph.getEdge(8));
        //            assertEquals(path.get(2), graph.getVertex(4));
        //            assertEquals(path.get(3), "josh");
        //        } else {
        //            assertFalse(true);
        //        }
        //        //System.out.println(name);
        //        //System.out.println(pipeline.getPath());
        //    }
        //}

    }

}
