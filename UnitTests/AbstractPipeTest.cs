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
using de.ahzf.blueprints;
using de.ahzf.blueprints.InMemoryGraph;
using de.ahzf.blueprints.Datastructures;

#endregion

namespace de.ahzf.Pipes.UnitTests
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
            pipe.SetIEnumerable(names);

            var counter = 0UL;
            while (pipe.MoveNext())
            {
                counter++;
                String name = pipe.Current;
                Assert.IsTrue(name.Equals("marko") || name.Equals("josh") || name.Equals("peter"));
            }
            
            Assert.AreEqual(counter, 3UL);
            pipe.SetIEnumerable(names);
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

        public void testPathConstruction()
        {

            IGraph graph = TinkerGraphFactory.CreateTinkerGraph();

            IVertex marko = graph.GetVertex(new VertexId("1"));
            IPipe<IVertex, IEdge>  pipe1 = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            IPipe<IEdge, IVertex>  pipe2 = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            IPipe<IVertex, String> pipe3 = new PropertyPipe<IVertex, String>("name");
            pipe3.SetIEnumerator(pipe2.GetEnumerator());
            pipe2.SetIEnumerator(pipe1.GetEnumerator());
            var _MarkoList = new List<IVertex>() { marko };
            pipe1.SetIEnumerator(_MarkoList.GetEnumerator());

            foreach (var name in pipe3)
            {

                var path = pipe3.Path;

                Assert.AreEqual(marko,          path[0]);
                Assert.AreEqual(typeof(Edge),   path[1].GetType());
                Assert.AreEqual(typeof(Vertex), path[2].GetType());
                Assert.AreEqual(typeof(String), path[3].GetType());

                //if (name.equals("vadas"))
                //{
                //    Assert.AreEqual(path.get(1), graph.getEdge(7));
                //    Assert.AreEqual(path.get(2), graph.getVertex(2));
                //    Assert.AreEqual(path.get(3), "vadas");
                //}
                //else if (name.equals("lop")) {
                //    Assert.AreEqual(path.get(1), graph.getEdge(9));
                //    Assert.AreEqual(path.get(2), graph.getVertex(3));
                //    Assert.AreEqual(path.get(3), "lop");
                //} else if (name.equals("josh")) {
                //    Assert.AreEqual(path.get(1), graph.getEdge(8));
                //    Assert.AreEqual(path.get(2), graph.getVertex(4));
                //    Assert.AreEqual(path.get(3), "josh");
                //} else {
                //    assertFalse(true);
                //}
                ////System.out.println(name);
                ////System.out.println(pipeline.getPath());
            }

        }

        #endregion

    }

}
