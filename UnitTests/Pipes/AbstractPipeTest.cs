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

using de.ahzf.blueprints;
using de.ahzf.blueprints.InMemoryGraph;
using de.ahzf.blueprints.Datastructures;

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.Pipes
{

    [TestFixture]
    public class AbstractPipeTest
    {
        
        #region TestIEnumerator()

        [Test]
        public void TestIEnumerator()
        {
            
            var names = new List<String>() { "marko", "josh", "peter" };

            IPipe<String, String> pipe = new IdentityPipe<String>();
            pipe.SetSourceCollection(names);

            var counter = 0UL;
            while (pipe.MoveNext())
            {
                counter++;
                String name = pipe.Current;
                Assert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
            }
            
            Assert.AreEqual(counter, 3UL);
            pipe.SetSourceCollection(names);
            counter = 0UL;
            
            foreach (var name in pipe)
            {
                Assert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
                counter++;
            }
            
            Assert.AreEqual(counter, 3UL);

        }

        #endregion

        #region testPathConstruction

        [Test]
        public void testPathConstruction()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            var marko = graph.GetVertex(new VertexId("1"));
            var pipe1 = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var pipe2 = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var pipe3 = new PropertyPipe<IVertex, String>("name");
            pipe3.SetSource(pipe2);
            pipe2.SetSource(pipe1);
            var _MarkoList = new List<IVertex>() { marko };
            pipe1.SetSource(_MarkoList.GetEnumerator());

            foreach (var name in pipe3)
            {

                var path = pipe3.Path;

                Assert.AreEqual(marko,          path[0]);
                Assert.AreEqual(typeof(Edge),   path[1].GetType());
                Assert.AreEqual(typeof(Vertex), path[2].GetType());
                Assert.AreEqual(typeof(String), path[3].GetType());

                if (name == "vadas")
                {
                    Assert.AreEqual(graph.GetEdge(new EdgeId(7)),     path[1]);
                    Assert.AreEqual(graph.GetVertex(new VertexId(2)), path[2]);
                    Assert.AreEqual("vadas", path[3]);
                }
                
                else if (name == "lop")
                {
                    Assert.AreEqual(graph.GetEdge(new EdgeId(9)),     path[1]);
                    Assert.AreEqual(graph.GetVertex(new VertexId(3)), path[2]);
                    Assert.AreEqual("lop", path[3]);
                }
                
                else if (name == "josh")
                {
                    Assert.AreEqual(graph.GetEdge(new EdgeId(8)),     path[1]);
                    Assert.AreEqual(graph.GetVertex(new VertexId(4)), path[2]);
                    Assert.AreEqual("josh", path[3]);
                }

                else
                    Assert.Fail();

                //System.out.println(name);
                //System.out.println(pipeline.getPath());

            }

        }

        #endregion

    }

}
