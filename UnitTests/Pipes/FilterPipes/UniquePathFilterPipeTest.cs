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
using de.ahzf.blueprints.Datastructures;

#endregion

namespace de.ahzf.Pipes.UnitTests.FilterPipes
{

    [TestFixture]
    public class UniquePathFilterPipeTest
    {

        #region testAndPipeBasic()

        [Test]
        public void testUniquePathFilter()
        {
	
            var _Graph      = TinkerGraphFactory.CreateTinkerGraph();
            var _Pipe1      = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _Pipe2      = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _Pipe3      = new VertexEdgePipe(VertexEdgePipe.Step.IN_EDGES);
            var _Pipe4      = new EdgeVertexPipe(EdgeVertexPipe.Step.OUT_VERTEX);
            var _Pipe5      = new UniquePathFilterPipe<IVertex>();
            var _Pipeline   = new Pipeline<IVertex, IVertex>(_Pipe1, _Pipe2, _Pipe3, _Pipe4, _Pipe5);
            _Pipeline.SetSource(new SingleEnumerator<IVertex>(_Graph.GetVertex(new VertexId(1))));
	
            var _Counter = 0;
	
            foreach (var _Object in _Pipeline)
            {
                
                _Counter++;
                
                Assert.IsTrue(_Object.Equals(_Graph.GetVertex(new VertexId(6))) ||
                              _Object.Equals(_Graph.GetVertex(new VertexId(4))));

            }
	
            Assert.AreEqual(2, _Counter);
	
        }

        #endregion

    }

}
