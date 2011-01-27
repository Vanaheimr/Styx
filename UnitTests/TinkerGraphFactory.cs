﻿/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Blueprints.NET
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

using de.ahzf.blueprints.InMemoryGraph;
using de.ahzf.blueprints.Datastructures;

#endregion

namespace de.ahzf.Pipes.UnitTests
{

    public static class TinkerGraphFactory
    {

        public static InMemoryGraph CreateTinkerGraph()
        {

            var _TinkerGraph = new InMemoryGraph();

            var marko = _TinkerGraph.AddVertex(new VertexId("1"));
            marko.SetProperty("name", "marko");
            marko.SetProperty("age", 29);

            var vadas = _TinkerGraph.AddVertex(new VertexId("2"));
            vadas.SetProperty("name", "vadas");
            vadas.SetProperty("age", 27);

            var lop = _TinkerGraph.AddVertex(new VertexId("3"));
            lop.SetProperty("name", "lop");
            lop.SetProperty("lang", "java");

            var josh = _TinkerGraph.AddVertex(new VertexId("4"));
            josh.SetProperty("name", "josh");
            josh.SetProperty("age", 32);

            var ripple = _TinkerGraph.AddVertex(new VertexId("5"));
            ripple.SetProperty("name", "ripple");
            ripple.SetProperty("lang", "java");

            var peter = _TinkerGraph.AddVertex(new VertexId("6"));
            peter.SetProperty("name", "peter");
            peter.SetProperty("age", 35);

            _TinkerGraph.AddEdge(marko, vadas, new EdgeId("7"), "knows").SetProperty("weight", 0.5f);
            _TinkerGraph.AddEdge(marko, josh, new EdgeId("8"), "knows").SetProperty("weight", 1.0f);
            _TinkerGraph.AddEdge(marko, lop, new EdgeId("9"), "created").SetProperty("weight", 0.4f);

            _TinkerGraph.AddEdge(josh, ripple, new EdgeId("10"), "created").SetProperty("weight", 1.0f);
            _TinkerGraph.AddEdge(josh, lop, new EdgeId("11"), "created").SetProperty("weight", 0.4f);

            _TinkerGraph.AddEdge(peter, lop, new EdgeId("12"), "created").SetProperty("weight", 0.2f);

            return _TinkerGraph;

        }

    }

}