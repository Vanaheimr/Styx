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
    public class IdEdgePipeTest
    {

        #region testIdEdgePipeGraph()

        [Test]
        public void testIdEdgePipeGraph()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            var ids = new List<EdgeId>() { new EdgeId("9"), new EdgeId("11"), new EdgeId("12") };
            IPipe<EdgeId, IEdge> pipe = new IdEdgePipe<EdgeId>(graph);
            pipe.SetSourceCollection(ids);
            int counter = 0;

            while (pipe.MoveNext())
            {
                
                IEdge edge = pipe.Current;

                if (counter == 0)
                {
                    Assert.AreEqual(new EdgeId("9"), edge.Id);
                    Assert.AreEqual(0.4f, edge.GetProperty("weight"));
                }
                else if (counter == 1)
                {
                    Assert.AreEqual(new EdgeId("11"), edge.Id);
                    Assert.AreEqual(graph.GetVertex(new VertexId("3")), edge.InVertex);
                }
                else if (counter == 2)
                {
                    Assert.AreEqual(new EdgeId("12"), edge.Id);
                    Assert.AreEqual("created", edge.Label);
                }
                else
                {
                    throw new Exception("Illegal state.");
                }

                counter++;

            }

            Assert.AreEqual(3, counter);

        }

        #endregion

    }

}
