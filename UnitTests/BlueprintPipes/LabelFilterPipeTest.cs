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

using NUnit.Framework;

using de.ahzf.blueprints.Datastructures;

#endregion

namespace de.ahzf.Pipes.UnitTests.Blueprints
{

    [TestFixture]
    public class LabelFilterPipeTest
    {

        #region testFilterLabels()

        [Test]
        public void testFilterLabels()
        {

            var graph = TinkerGraphFactory.CreateTinkerGraph();

            var marko = graph.GetVertex(new VertexId("1"));
            var lfp = new LabelFilterPipe("knows", ComparisonFilter.NOT_EQUAL);
            lfp.SetSourceCollection(marko.OutEdges);

            int counter = 0;
            while (lfp.MoveNext())
            {
                var e = lfp.Current;
                Assert.AreEqual(marko, e.OutVertex);
                Assert.IsTrue(e.InVertex.Id.Equals(new VertexId("2")) || e.InVertex.Id.Equals(new VertexId("4")));
                counter++;
            }

            Assert.AreEqual(2, counter);


            lfp = new LabelFilterPipe("knows", ComparisonFilter.EQUAL);
            lfp.SetSourceCollection(marko.OutEdges);

            counter = 0;
            while (lfp.MoveNext())
            {
                var e = lfp.Current;
                Assert.AreEqual(marko, e.OutVertex);
                Assert.IsTrue(e.InVertex.Id.Equals(new VertexId("3")));
                counter++;
            }

            Assert.AreEqual(1, counter);

        }

        #endregion

    }

}
