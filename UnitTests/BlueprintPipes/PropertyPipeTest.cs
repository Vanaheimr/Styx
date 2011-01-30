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
    public class PropertyPipeTest
    {

        #region testSingleProperty()

        [Test]
        public void testSingleProperty()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();
            var marko = graph.GetVertex(new VertexId("1"));

            var pp = new PropertyPipe<IVertex, String>("name");
            pp.SetSource(new List<IVertex>() { marko }.GetEnumerator());

            int counter = 0;
            while (pp.MoveNext())
            {
                var name = pp.Current;
                Assert.AreEqual("marko", name);
                counter++;
            }

        }

        #endregion

        #region testMultiProperty()

        [Test]
        public void testMultiProperty()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();
            var marko = graph.GetVertex(new VertexId("1"));

            var evp      = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var pp       = new PropertyPipe<IVertex, String>("name");
            var pipeline = new Pipeline<IEdge, String>(evp, pp);
            pipeline.SetSourceCollection(marko.OutEdges);

            int counter = 0;
            while (pipeline.MoveNext())
            {
                var name = pipeline.Current;
                Assert.IsTrue(name.Equals("vadas") || name.Equals("josh") || name.Equals("lop"));
                counter++;
            }

            Assert.AreEqual(3, counter);

        }

        #endregion

        #region testListProperty()

        [Test]
        public void testListProperty()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();
            var marko = graph.GetVertex(new VertexId("1"));

            var vadas    = graph.GetVertex(new VertexId("2"));
            var pipe     = new PropertyPipe<IVertex, String>("name");
            var pipeline = new Pipeline<IVertex, String>(pipe);
            pipeline.SetSource(new List<IVertex>() { marko, vadas }.GetEnumerator());

            int counter = 0;
            while (pipeline.MoveNext())
            {
                var name = pipeline.Current;
                Assert.IsTrue(name.Equals("vadas") || name.Equals("marko"));
                counter++;
            }

            Assert.AreEqual(2, counter);

        }

        #endregion

    }

}
