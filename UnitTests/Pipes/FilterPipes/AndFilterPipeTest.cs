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
    public class AndFilterPipeTest
	{
		
		#region testAndPipeBasic()

        [Test]
        public void testAndPipeBasic()
        {	

			var _Names = new List<String>() { "marko", "povel", "peter", "povel", "marko" };
	        var _Pipe1 = new ObjectFilterPipe<String>("marko", ComparisonFilter.NOT_EQUAL);
	        var _Pipe2 = new ObjectFilterPipe<String>("povel", ComparisonFilter.NOT_EQUAL);
	        var _AndFilterPipe = new AndFilterPipe<String>(new HasNextPipe<String>(_Pipe1), new HasNextPipe<String>(_Pipe2));
	        _AndFilterPipe.SetSourceCollection(_Names);

			var _Counter = 0;
	        while (_AndFilterPipe.MoveNext())
	            _Counter++;
			
	        Assert.AreEqual(0, _Counter);

		}

        #endregion
		
		#region testAndPipeBasic2()

        [Test]
        public void testAndPipeBasic2()
        {	

			var _Names 			= new List<String>() { "marko", "povel", "peter", "povel", "marko" };
	        var _Pipe1 			= new ObjectFilterPipe<String>("marko", ComparisonFilter.NOT_EQUAL);
	        var _Pipe2 			= new ObjectFilterPipe<String>("marko", ComparisonFilter.NOT_EQUAL);
	        var _AndFilterPipe 	= new AndFilterPipe<String>(new HasNextPipe<String>(_Pipe1), new HasNextPipe<String>(_Pipe2));
	        _AndFilterPipe.SetSourceCollection(_Names);

			var _Counter = 0;
	        while (_AndFilterPipe.MoveNext())
	            _Counter++;
			
	        Assert.AreEqual(2, _Counter);

		}

        #endregion
		
		#region testAndPipeGraph()

        [Test]
        public void testAndPipeGraph()
        {	

			var _Graph 			= TinkerGraphFactory.CreateTinkerGraph();
		    var _Marko 			= _Graph.GetVertex(new VertexId("1"));
		    var _Peter 			= _Graph.GetVertex(new VertexId("6"));

		    var _Pipe0 			= new VertexEdgePipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                     EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                     HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>(Steps.VertexEdgeStep.OUT_EDGES);

		    var _Pipe1 			= new LabelFilterPipe<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                      EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                      HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>("knows", ComparisonFilter.NOT_EQUAL);

		    var _Pipe2 			= new PropertyFilterPipe<EdgeId, RevisionId, String, Object, IDictionary<String, Object>, IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                        EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                                                                                        HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>, Double>("weight", 0.5, ComparisonFilter.LESS_THAN_EQUAL);

		    var _AndFilterPipe	= new AndFilterPipe<IPropertyEdge<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
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
                                                             HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>(_Pipe0, _AndFilterPipe);

		    _Pipeline.SetSourceCollection(new List<IPropertyVertex<VertexId,    RevisionId, String, Object, IDictionary<String, Object>,
                                                                   EdgeId,      RevisionId, String, Object, IDictionary<String, Object>,
                                                                   HyperEdgeId, RevisionId, String, Object, IDictionary<String, Object>>>() { _Marko, _Peter, _Marko });

			var _Counter = 0;
		    while (_Pipeline.MoveNext())
			{
		        var _Edge = _Pipeline.Current;
		        Assert.IsTrue(_Edge.Id.Equals(new EdgeId("8")));
		        Assert.IsTrue(((Double)_Edge.GetProperty("weight")) > 0.5f && _Edge.Label.Equals("knows"));
		        _Counter++;
		    }
			
		    Assert.AreEqual(2, _Counter);

		}

        #endregion
	
	}

}
