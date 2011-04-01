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

	        var _Pipe0 			= new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                     EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                     HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);
	        
            var _Pipe1 			= new LabelFilterPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                      EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                      HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>("created", ComparisonFilter.NOT_EQUAL);

            var _Pipe2 			= new PropertyFilterPipe<EdgeId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                        EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                        HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, Double>("weight", 0.5, ComparisonFilter.LESS_THAN_EQUAL);

            var _ORFilterPipe	= new OrFilterPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(

                                      new HasNextPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                    EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                    HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_Pipe1),

                                      new HasNextPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                    EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                    HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_Pipe2));

	        var _Pipeline 		= new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>,

                                                 IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_Pipe0, _ORFilterPipe);

	        _Pipeline.SetSourceCollection(new List<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>() { _Marko, _Peter, _Marko });
	        
			var _Counter = 0;
	        while (_Pipeline.MoveNext())
			{
	            var _Edge = _Pipeline.Current;
	            Assert.IsTrue(_Edge.Id.Equals(new EdgeId("8")) || _Edge.Id.Equals(new EdgeId("9")) || _Edge.Id.Equals(new EdgeId("12")));
	            Assert.IsTrue(((Double)_Edge.GetProperty("weight")) > 0.5f || _Edge.Label.Equals("created"));
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
	        
            var _Pipe1 		= new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);

	        var _PipeA 		= new LabelFilterPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>("created", ComparisonFilter.NOT_EQUAL);

	        var _PipeB 		= new LabelFilterPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>("knows", ComparisonFilter.NOT_EQUAL);

	        var _PipeC 		= new PropertyFilterPipe<EdgeId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                    EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                    HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, Double>("weight", 0.5, ComparisonFilter.LESS_THAN_EQUAL);

	        var _PipeD 		= new AndFilterPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                              EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                              HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(

                                  new HasNextPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_PipeB),
                                  new HasNextPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_PipeC));

	        var _Pipe2 		= new OrFilterPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                             EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                             HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(

                                  new HasNextPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_PipeA),
                                  new HasNextPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_PipeD));
	        
            var _Pipeline 	= new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                           EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                           HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>,

                                           IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                         EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                         HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_Pipe1, _Pipe2);

	        _Pipeline.SetSourceCollection(new List<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>() { _Marko });
	        
			int _Counter = 0;
	        while (_Pipeline.MoveNext())
			{
	            var _Edge = _Pipeline.Current;
	            Assert.IsTrue(_Edge.Id.Equals(new EdgeId("8")) || _Edge.Id.Equals(new EdgeId("9")));
	            Assert.IsTrue(_Edge.Label.Equals("created") || (((Double)_Edge.GetProperty("weight")) > 0.5 && _Edge.Label.Equals("knows")));
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

            var _PipeA      = new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);

            var _PipeB      = new LabelFilterPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>("created", ComparisonFilter.NOT_EQUAL);

            var _PipeC      = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);

            var _PipeD      = new PropertyFilterPipe<VertexId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                        EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                        HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, String>("name", "lop", ComparisonFilter.NOT_EQUAL);

            var _Pipe1      = new AndFilterPipe<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(

                                  new HasNextPipe<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(

                                  new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>,

                                               IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_PipeA, _PipeB, _PipeC, _PipeD)));
            
            var _Pipe2      = new PropertyPipe<VertexId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, String>("name");

            var _Pipeline   = new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                           EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                           HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, String>(_Pipe1, _Pipe2);
            
            _Pipeline.SetSourceCollection(new List<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>() { _Marko });

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

            var _PipeA      = new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);
            
            var _PipeB      = new PropertyFilterPipe<EdgeId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                      EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                      HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, Double>("weight", 0.5, ComparisonFilter.LESS_THAN_EQUAL);

            var _PipeC      = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);

            var _Pipe1      = new AndFilterPipe<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(

                                  new HasNextPipe<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(
                                                                  
                                      new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>,

                                                   IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_PipeA, _PipeB, _PipeC)));

            var _Pipe2      = new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);

            var _Pipe3      = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);

            var _Pipe4      = new PropertyPipe<VertexId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, String>("name");

            var _Pipeline   = new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                           EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                           HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, String>(_Pipe1, _Pipe2, _Pipe3, _Pipe4);

            _Pipeline.SetSourceCollection(new List<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>() { _Marko });

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

            var _PipeA      = new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);

            var _PipeB      = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);

            var _Pipe1      = new OrFilterPipe<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(

                                  new HasNextPipe<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(

                                      new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>,

                                                   IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_PipeA, _PipeB)));

            var _PipeC      = new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);

            var _Pipe2      = new OrFilterPipe<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                               EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                               HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(

                                  new HasNextPipe<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_PipeC));

            var _Pipe3      = new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);

            var _Pipe4      = new EdgeVertexPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                 EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                 HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.EdgeVertexStep.IN_VERTEX);

            var _Pipe5      = new PropertyPipe<VertexId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                  EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                  HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, String>("name");

            var _Pipeline   = new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                           EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                           HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, String>(_Pipe1, _Pipe2, _Pipe3, _Pipe4, _Pipe5);

            _Pipeline.SetSourceCollection(new List<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>() { _Marko });

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
