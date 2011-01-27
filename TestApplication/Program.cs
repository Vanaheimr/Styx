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

using de.ahzf.Pipes;
using de.ahzf.blueprints;
using de.ahzf.blueprints.Datastructures;
using de.ahzf.blueprints.InMemoryGraph;
using de.ahzf.Pipes.UnitTests;

#endregion

namespace TestApplication
{

    public class Program
    {

        public static void Main(String[] myArgs)
        {

            var _SingleEnumerator = new SingleEnumerator<UInt64>(123);
            var _List             = new List<UInt64>();
                _List.Add(123);
            var _ListEnumerator   = _List.GetEnumerator();

            var _List1 = new List<UInt64>();
            var _List2 = new List<UInt64>();

            while (_SingleEnumerator.MoveNext())
                _List1.Add(_SingleEnumerator.Current);

            while (_ListEnumerator.MoveNext())
                _List2.Add(_ListEnumerator.Current);




            IGraph graph = TinkerGraphFactory.CreateTinkerGraph();

            IVertex marko = graph.GetVertex(new VertexId("1"));
            IPipe<IVertex, IEdge>  pipe1 = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            IPipe<IEdge, IVertex>  pipe2 = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            IPipe<IVertex, String> pipe3 = new PropertyPipe<IVertex, String>("name");
            pipe3.SetStarts(pipe2.GetEnumerator());
            pipe2.SetStarts(pipe1.GetEnumerator());
            var _MarkoList = new List<IVertex>() { marko };
            pipe1.SetStarts(_MarkoList.GetEnumerator());

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

    }

}
