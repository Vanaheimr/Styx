﻿/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/pipes.NET>
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

using de.ahzf.blueprints;
using de.ahzf.blueprints.PropertyGraph;

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.Blueprints
{

    [TestFixture]
    public class LabelPipeTest
    {

        #region testLabels()

        [Test]
        public void testLabels()
        {

            var _Graph = TinkerGraphFactory.CreateTinkerGraph();
            
            var _Pipe  = new LabelPipe<VertexId,    RevisionId, String, Object,
                                       EdgeId,      RevisionId, String, Object,
                                       HyperEdgeId, RevisionId, String, Object>();

            _Pipe.SetSourceCollection(_Graph.GetVertex(new VertexId("1")).OutEdges);

            var _Counter = 0;
            while (_Pipe.MoveNext())
            {
                String label = _Pipe.Current;
                Assert.IsTrue(label.Equals("knows") || label.Equals("created"));
                _Counter++;
            }

            Assert.AreEqual(3, _Counter);

        }

        #endregion

    }

}
