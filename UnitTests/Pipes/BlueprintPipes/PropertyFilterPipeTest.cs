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

            var _Graph    = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko    = _Graph.GetVertex(new VertexId("1"));
            var _Pipe1    = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _Pipe2    = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _Pipe3    = new PropertyFilterPipe<IVertex, String>("lang", "java", ComparisonFilter.NOT_EQUAL);
            var _Pipeline = new Pipeline<IVertex, IVertex>(new List<IPipe>() { _Pipe1, _Pipe2, _Pipe3 });
            _Pipeline.SetSource(new List<IVertex>() { _Marko }.GetEnumerator());

            var _Counter = 0;
            while (_Pipeline.MoveNext())
            {
                _Counter++;
                var _Vertex = _Pipeline.Current;
                Assert.AreEqual(new VertexId("3"), _Vertex.Id);
                Assert.AreEqual("java", _Vertex.GetProperty("lang"));
                Assert.AreEqual("lop",  _Vertex.GetProperty("name"));
            }

            Assert.AreEqual(1, _Counter);

        }

        #endregion

    }

}
