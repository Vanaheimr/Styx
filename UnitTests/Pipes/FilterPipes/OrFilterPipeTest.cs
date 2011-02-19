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
    public class OrFilterPipeTest
	{
		
		#region testOrPipeBasic()

        [Test]
        public void testOrPipeBasic()
        {	

			var _Names 			= new List<String>() { "marko", "povel", "peter", "povel", "marko" };
	        var _Pipe1 			= new ObjectFilterPipe<String>("marko", ComparisonFilter.NOT_EQUAL);
	        var _Pipe2 			= new ObjectFilterPipe<String>("povel", ComparisonFilter.NOT_EQUAL);
	        var _ORFilterPipe 	= new OrFilterPipe<String>(new HasNextPipe<String>(_Pipe1), new HasNextPipe<String>(_Pipe2));
	        _ORFilterPipe.SetSourceCollection(_Names);
	        
			int _Counter = 0;
	        while (_ORFilterPipe.MoveNext())
			{
	            var name = _ORFilterPipe.Current;
	            Assert.IsTrue(name.Equals("marko") || name.Equals("povel"));
	            _Counter++;
	        }
	        
			Assert.AreEqual(4, _Counter);

		}

        #endregion
		
		#region testOrPipeGraph()

        [Test]
        public void testOrPipeGraph()
        {	

			// ./outE[@label='created' or @weight > 0.5]

	        var _Graph 			= TinkerGraphFactory.CreateTinkerGraph();
	        var _Marko 			= _Graph.GetVertex(new VertexId("1"));
	        var _Peter 			= _Graph.GetVertex(new VertexId("6"));
	        var _Pipe0 			= new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
	        var _Pipe1 			= new LabelFilterPipe("created", ComparisonFilter.NOT_EQUAL);
	        var _Pipe2 			= new PropertyFilterPipe<IEdge, Double>("weight", 0.5, ComparisonFilter.LESS_THAN_EQUAL);
	        var _ORFilterPipe	= new OrFilterPipe<IEdge>(new HasNextPipe<IEdge>(_Pipe1), new HasNextPipe<IEdge>(_Pipe2));
	        var _Pipeline 		= new Pipeline<IVertex, IEdge>(_Pipe0, _ORFilterPipe);
	        _Pipeline.SetSourceCollection(new List<IVertex>() { _Marko, _Peter, _Marko });
	        
			var _Counter = 0;
	        while (_Pipeline.MoveNext())
			{
	            var _Edge = _Pipeline.Current;
	            Assert.IsTrue(_Edge.Id.Equals(new EdgeId("8")) || _Edge.Id.Equals(new EdgeId("9")) || _Edge.Id.Equals(new EdgeId("12")));
	            Assert.IsTrue(_Edge.GetProperty<Double>("weight") > 0.5f || _Edge.Label.Equals("created"));
	            _Counter++;
	        }

	        Assert.AreEqual(5, _Counter);

		}

        #endregion

		#region testAndOrPipeGraph()

        [Test]
        public void testAndOrPipeGraph()
        {	

			// ./outE[@label='created' or (@label='knows' and @weight > 0.5)]

	        var _Graph 		= TinkerGraphFactory.CreateTinkerGraph();
	        var _Marko 		= _Graph.GetVertex(new VertexId("1"));
	        var _Pipe1 		= new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
	        var _PipeA 		= new LabelFilterPipe("created", ComparisonFilter.NOT_EQUAL);
	        var _PipeB 		= new LabelFilterPipe("knows", ComparisonFilter.NOT_EQUAL);
	        var _PipeC 		= new PropertyFilterPipe<IEdge, Double>("weight", 0.5, ComparisonFilter.LESS_THAN_EQUAL);
	        var _PipeD 		= new AndFilterPipe<IEdge>(new HasNextPipe<IEdge>(_PipeB), new HasNextPipe<IEdge>(_PipeC));
	        var _Pipe2 		= new OrFilterPipe<IEdge>(new HasNextPipe<IEdge>(_PipeA), new HasNextPipe<IEdge>(_PipeD));
	        var _Pipeline 	= new Pipeline<IVertex, IEdge>(_Pipe1, _Pipe2);
	        _Pipeline.SetSourceCollection(new List<IVertex>() { _Marko });
	        
			int _Counter = 0;
	        while (_Pipeline.MoveNext())
			{
	            var _Edge = _Pipeline.Current;
	            Assert.IsTrue(_Edge.Id.Equals(new EdgeId("8")) || _Edge.Id.Equals(new EdgeId("9")));
	            Assert.IsTrue(_Edge.Label.Equals("created") || (_Edge.GetProperty<Double>("weight") > 0.5 && _Edge.Label.Equals("knows")));
	            _Counter++;
	        }
	        
			Assert.AreEqual(2, _Counter);

		}

        #endregion
		
		#region testFutureFilter()

        [Test]
        public void testFutureFilter()
        {	

			var _Names 		= new List<String>() { "marko", "peter", "josh", "marko", "jake", "marko", "marko" };
	        var _PipeA 		= new CharacterCountPipe();
	        var _PipeB 		= new ObjectFilterPipe<UInt64>(4, ComparisonFilter.EQUAL);
            var _Pipe1 		= new OrFilterPipe<String>(new HasNextPipe<String>(new Pipeline<String, UInt64>(_PipeA, _PipeB)));
	        var _Pipeline 	= new Pipeline<String, String>(_Pipe1);
	        _Pipeline.SetSourceCollection(_Names);
	        
			int _Counter = 0;
	        while (_Pipeline.MoveNext())
			{
	            var name = _Pipeline.Current;
	            _Counter++;
	            Assert.IsTrue((name.Equals("marko") || name.Equals("peter")) && !name.Equals("josh") && !name.Equals("jake"));
	        }

			Assert.AreEqual(5, _Counter);

		}

        #endregion

        #region testFutureFilterGraph()

        [Test]
        public void testFutureFilterGraph()
        {

            // ./outE[@label='created']/inV[@name='lop']/../../@name

            var _Graph      = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko      = _Graph.GetVertex(new VertexId("1"));

            var _PipeA      = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _PipeB      = new LabelFilterPipe("created", ComparisonFilter.NOT_EQUAL);
            var _PipeC      = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _PipeD      = new PropertyFilterPipe<IVertex, String>("name", "lop", ComparisonFilter.NOT_EQUAL);
            var _Pipe1      = new AndFilterPipe<IVertex>(new HasNextPipe<IVertex>(new Pipeline<IVertex, IVertex>(_PipeA, _PipeB, _PipeC, _PipeD)));
            var _Pipe2      = new PropertyPipe<IVertex, String>("name");
            var _Pipeline   = new Pipeline<IVertex, String>(_Pipe1, _Pipe2);
            _Pipeline.SetSourceCollection(new List<IVertex>() { _Marko });

            var _Counter = 0;
            while (_Pipeline.MoveNext())
            {
                var name = _Pipeline.Current;
                Assert.AreEqual("marko", name);
                _Counter++;
            }
            
            Assert.AreEqual(1, _Counter);

		}

        #endregion

        #region testComplexFutureFilterGraph()

        [Test]
        public void testComplexFutureFilterGraph()
        {

            // ./outE[@weight > 0.5]/inV/../../outE/inV/@name

            var _Graph      = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko      = _Graph.GetVertex(new VertexId("1"));

            var _PipeA      = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _PipeB      = new PropertyFilterPipe<IEdge, Double>("weight", 0.5, ComparisonFilter.LESS_THAN_EQUAL);
            var _PipeC      = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _Pipe1      = new AndFilterPipe<IVertex>(new HasNextPipe<IVertex>(new Pipeline<IVertex, IVertex>(_PipeA, _PipeB, _PipeC)));
            var _Pipe2      = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _Pipe3      = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _Pipe4      = new PropertyPipe<IVertex, String>("name");
            var _Pipeline   = new Pipeline<IVertex, String>(_Pipe1, _Pipe2, _Pipe3, _Pipe4);
            _Pipeline.SetSourceCollection(new List<IVertex>() { _Marko });

            var _Counter = 0;
            while (_Pipeline.MoveNext())
            {
                var _Name = _Pipeline.Current;
                Assert.IsTrue(_Name.Equals("vadas") || _Name.Equals("lop") || _Name.Equals("josh"));
                _Counter++;
            }
            
            Assert.AreEqual(3, _Counter);

        }

        #endregion

        #region testComplexTwoFutureFilterGraph()

        [Test]
        public void testComplexTwoFutureFilterGraph()
        {

            // ./outE/inV/../../outE/../outE/inV/@name

            var _Graph      = TinkerGraphFactory.CreateTinkerGraph();
            var _Marko      = _Graph.GetVertex(new VertexId("1"));

            var _PipeA      = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _PipeB      = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _Pipe1      = new OrFilterPipe<IVertex>(new HasNextPipe<IVertex>(new Pipeline<IVertex, IVertex>(_PipeA, _PipeB)));
            var _PipeC      = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _Pipe2      = new OrFilterPipe<IVertex>(new HasNextPipe<IVertex>(_PipeC));
            var _Pipe3      = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
            var _Pipe4      = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
            var _Pipe5      = new PropertyPipe<IVertex, String>("name");
            var _Pipeline   = new Pipeline<IVertex, String>(_Pipe1, _Pipe2, _Pipe3, _Pipe4, _Pipe5);
            _Pipeline.SetSourceCollection(new List<IVertex>() { _Marko });

            var _Counter = 0;
            while (_Pipeline.MoveNext())
            {
                var _Name = _Pipeline.Current;
                Assert.IsTrue(_Name.Equals("vadas") || _Name.Equals("lop") || _Name.Equals("josh"));
                _Counter++;
            }

            Assert.AreEqual(3, _Counter);

        }

        #endregion

	}
	
}