/*
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

using de.ahzf.blueprints;
using de.ahzf.blueprints.InMemory.PropertyGraph;
using de.ahzf.blueprints.Datastructures;
using System.Collections.Generic;
using System;

#endregion

namespace de.ahzf.Pipes.UnitTests
{

    public static class ToyGraphFactory
    {

        public static IPropertyGraph CreateToyGraph()
        {

            var _ToyGraph    = new InMemoryPropertyGraph() as IPropertyGraph;

            var _Alice       = _ToyGraph.AddVertex(new PropertyVertex(new VertexId("1"), v => v.SetProperty("name", "Alice").    SetProperty("age", 29)));
            var _Bob         = _ToyGraph.AddVertex(new PropertyVertex(new VertexId("2"), v => v.SetProperty("name", "Bob").      SetProperty("age", 27)));
            var _Carol       = _ToyGraph.AddVertex(new PropertyVertex(new VertexId("3"), v => v.SetProperty("name", "Carol").    SetProperty("age", 23)));
            var _Dave        = _ToyGraph.AddVertex(new PropertyVertex(new VertexId("4"), v => v.SetProperty("name", "Dave").     SetProperty("age", 32)));
            var _Eve         = _ToyGraph.AddVertex(new PropertyVertex(new VertexId("5"), v => v.SetProperty("name", "Eve").      SetProperty("age", 12)));
            var _Fred        = _ToyGraph.AddVertex(new PropertyVertex(new VertexId("6"), v => v.SetProperty("name", "Fred").     SetProperty("age", 35)));
            var _Geraldine   = _ToyGraph.AddVertex(new PropertyVertex(new VertexId("7"), v => v.SetProperty("name", "Geraldine").SetProperty("age", 35)));

            _ToyGraph.AddDoubleEdge(_Alice, _Bob,   new EdgeId("1a"), new EdgeId("1b"), "knows");
            _ToyGraph.AddDoubleEdge(_Alice, _Bob,   new EdgeId("2a"), new EdgeId("2b"), "loves");
            _ToyGraph.AddDoubleEdge(_Alice, _Carol, new EdgeId("3a"), new EdgeId("3b"), "knows");
            _ToyGraph.AddDoubleEdge(_Carol, _Dave,  new EdgeId("4a"), new EdgeId("4b"), "knows");

            _ToyGraph.AddEdge(_Carol, _Bob,   new EdgeId("5"), "knows");
            _ToyGraph.AddEdge(_Carol, _Bob,   new EdgeId("6"), "loves");
            _ToyGraph.AddEdge(_Dave,  _Carol, new EdgeId("7"), "loves");
            _ToyGraph.AddEdge(_Eve,   _Alice, new EdgeId("8"),  "knows");
            _ToyGraph.AddEdge(_Eve,   _Alice, new EdgeId("9"),  "loves");
            _ToyGraph.AddEdge(_Eve,   _Alice, new EdgeId("10"), "spies"); 
            _ToyGraph.AddEdge(_Eve,   _Bob,   new EdgeId("11"), "knows");            
            _ToyGraph.AddEdge(_Eve,   _Bob,   new EdgeId("12"), "spies");

            return _ToyGraph;

        }

    }

}
