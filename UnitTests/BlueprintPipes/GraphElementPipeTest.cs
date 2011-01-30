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
    public class GraphElementPipeTest
    {

        #region testVertexIterator()

        [Test]
        public void testVertexIterator()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            IPipe<IGraph, IVertex> pipe = new GraphElementPipe<IVertex>(GraphElementPipe<IVertex>.ElementType.VERTEX);
            pipe.SetSource(new SingleEnumerator<IGraph>(graph));
            int counter = 0;
            var vertices = new HashSet<IVertex>();

            while (pipe.MoveNext())
            {
                counter++;
                var vertex = pipe.Current;
                vertices.Add(vertex);
                //System.out.println(vertex);
            }

            Assert.AreEqual(6, counter);
            Assert.AreEqual(6, vertices.Count);

        }

        #endregion

        #region testEdgeIterator()

        [Test]
        public void testEdgeIterator()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            IPipe<IGraph, IEdge> pipe = new GraphElementPipe<IEdge>(GraphElementPipe<IEdge>.ElementType.EDGE);
            pipe.SetSource(new SingleEnumerator<IGraph>(graph));
            int counter = 0;
            var edges = new HashSet<IEdge>();

            while (pipe.MoveNext())
            {
                counter++;
                var edge = pipe.Current;
                edges.Add(edge);
                //System.out.println(edge);
            }

            Assert.AreEqual(6, counter);
            Assert.AreEqual(6, edges.Count);

        }

        #endregion

        #region testEdgeIteratorThreeGraphs()

        [Test]
        public void testEdgeIteratorThreeGraphs()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            IPipe<IGraph, IEdge> pipe = new GraphElementPipe<IEdge>(GraphElementPipe<IEdge>.ElementType.EDGE);
            pipe.SetSourceCollection(new List<IGraph>() { graph, graph, graph });
            int counter = 0;
            var edges = new HashSet<IEdge>();

            while (pipe.MoveNext())
            {
                counter++;
                var edge = pipe.Current;
                edges.Add(edge);
                //System.out.println(edge);
            }
            Assert.AreEqual(18, counter);
            Assert.AreEqual(6, edges.Count);

        }

        #endregion

    }

}
