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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using de.ahzf.Pipes;
using de.ahzf.Pipes.ExtensionMethods;
using de.ahzf.blueprints;
using de.ahzf.blueprints.Datastructures;
using de.ahzf.blueprints.InMemory.PropertyGraph;
using de.ahzf.Pipes.UnitTests;

using NUnit.Framework;
using Mono.CSharp;
using Mono;

#endregion

namespace PipesShell
{
    
    public class Program
    {

        public static Int32 Main(String[] myArgs)
        {


            /*
            var grapha    = TinkerGraphFactory.CreateTinkerGraph();

            var markoa   = grapha.GetVertex(new VertexId("1"));
            var pipe1a   = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var pipe2a   = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var pipe3a   = new PathPipe<IVertex>();
            var pipeline = new Pipeline<IVertex, IEnumerable<Object>>(pipe1a, pipe2a, pipe3a);
            pipeline.SetSource(new SingleEnumerator<IVertex>(markoa));

            foreach (var _Path in pipeline)
            {
                Assert.AreEqual(markoa, _Path.ElementAt(0));
                Assert.IsTrue(_Path.ElementAt(1) is IEdge);
                Assert.IsTrue(_Path.ElementAt(2) is IVertex);
            }
			
			
			var g = TinkerGraphFactory.CreateTinkerGraph();
			var f = g.VertexId(1).OutEdges("knows").InVertex().GetProperty<String>("name");
			
             */





            //var _Graph = ToyGraphFactory.CreateToyGraph();
            //var _Alice = _Graph.GetVertex(new VertexId("1"));
            //var _PPipe = new PropertyPipe<IVertex, String>("name");
            //_PPipe.SetSource(new List<IVertex>() { _Alice }.GetEnumerator());

            //var _Friends = _Graph.VertexId(1).
            //               OutEdges("knows").
            //               InVertex().
            //    //Neighbors("loves").
            //    //Foaf("knows").
            //               GetProperty<String>("name").
            //               ToList();

            //_Friends.ForEach(_Friend => Console.WriteLine(_Friend));

            //var _len = _Friends.MapEach(_Friend => _Friend.Length);


            //var _IdComplicated = _Graph.VertexId(1, 3).IsComplicated().ToList();

            ////BUG: Stopps when an intermediate vertex has no edges!
            //var _FFriends = _Graph.GetVertices(new VertexId("1"), new VertexId("3")).
            //               VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES).
            //               EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX).
            //               GetProperty<String>("name").ToList();

            //var _Counter = 0;
            //while (_PPipe.MoveNext())
            //{
            //    var name = _PPipe.Current;
            //    Assert.AreEqual("marko", name);
            //    _Counter++;
            //}



            /*
            var graph = TinkerGraphFactory.CreateTinkerGraph();

            var marko = graph.GetVertex(new VertexId("1"));
            var pipe1 = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var pipe2 = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var pipe3 = new PropertyPipe<IVertex, String>("name");
            pipe3.SetSource(pipe2);
            pipe2.SetSource(pipe1);
            var _MarkoList = new List<IVertex>() { marko };
            pipe1.SetSource(_MarkoList.GetEnumerator());

            foreach (var name in pipe3)
            {

                var path = pipe3.Path;

                Assert.AreEqual(marko,          path[0]);
                Assert.AreEqual(typeof(Edge),   path[1].GetType());
                Assert.AreEqual(typeof(Vertex), path[2].GetType());
                Assert.AreEqual(typeof(String), path[3].GetType());

                if (name == "vadas")
                {
                    Assert.AreEqual(graph.GetEdge(new EdgeId(7)),     path[1]);
                    Assert.AreEqual(graph.GetVertex(new VertexId(2)), path[2]);
                    Assert.AreEqual("vadas", path[3]);
                }
                
                else if (name == "lop")
                {
                    Assert.AreEqual(graph.GetEdge(new EdgeId(9)),     path[1]);
                    Assert.AreEqual(graph.GetVertex(new VertexId(3)), path[2]);
                    Assert.AreEqual("lop", path[3]);
                }
                
                else if (name == "josh")
                {
                    Assert.AreEqual(graph.GetEdge(new EdgeId(8)),     path[1]);
                    Assert.AreEqual(graph.GetVertex(new VertexId(4)), path[2]);
                    Assert.AreEqual("josh", path[3]);
                }

                else
                    Assert.Fail();

                //System.out.println(name);
                //System.out.println(pipeline.getPath());

            }
             */

            //Console.WriteLine("ok!");
            //Console.ReadLine();





            var _Report = new Report(new ConsoleReportPrinter());
            var _CLP = new CommandLineParser(_Report);
            //          cmd.UnknownOptionHandler += HandleExtraArguments;

            var _Settings = _CLP.ParseArguments(myArgs);
            if (_Settings == null || _Report.Errors > 0)
                return 1;

            var _Evaluator = new Evaluator(_Settings, _Report)
            {
                InteractiveBaseClass = typeof(InteractiveBaseShell),
                DescribeTypeExpressions = true
            };

            // Adding a assembly twice will lead to delayed errors...
            _Evaluator.ReferenceAssembly(typeof(IPropertyGraph).Assembly);
            _Evaluator.ReferenceAssembly(typeof(VertexEdgePipeExtensions).Assembly);
            _Evaluator.ReferenceAssembly(typeof(InMemoryPropertyGraph).Assembly);
            _Evaluator.ReferenceAssembly(typeof(IPipe).Assembly);
            _Evaluator.ReferenceAssembly(typeof(TinkerGraphFactory).Assembly);

            String[] _StartupFiles = { };


            // Create a graph...
            var g = TinkerGraphFactory.CreateTinkerGraph();

            // params seem to fail!
            var f0 = g.GetVertices(new VertexId(1));
            // This is a work-around!
            var f1 = g.GetVertices(new VertexId[] { new VertexId(1) });

            var _Pipe1    = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _Pipe2    = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _Pipe3    = new PropertyPipe<VertexId, String, IVertex, String>(new String[] { "name" });
            var _Pipeline = new Pipeline<IVertex, String>(new IPipe[] { _Pipe1, _Pipe2, _Pipe3 });
            _Pipeline.SetSourceCollection(f1);

            //foreach (var _Friend in _Pipeline) Console.WriteLine(_Friend);


            // Still problems with extension methods?
            //var f3 = g.GetVertices(new VertexId[] { new VertexId(1) }).OutEdges().InVertex().GetProperty<String>("name").ToList();
            //var f4 = g.VertexId(1).OutEdges("knows").InVertex().GetProperty<String>("name").ToList();


            return new CSharpShell(_Evaluator).Run(_StartupFiles);

        }

    }

}
