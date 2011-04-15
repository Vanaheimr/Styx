/*
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
using System.Collections.Generic;

using de.ahzf.blueprints;
using de.ahzf.blueprints.PropertyGraph;

using NUnit.Framework;

#endregion

namespace de.ahzf.Pipes.UnitTests.FilterPipes
{

    [TestFixture]
    public class FutureFilterPipeTest
	{
		
		#region testBasicFutureFilter()

        [Test]
        public void testBasicFutureFilter()
        {	

			var _Names	= new List<String>() { "marko", "povel", "peter", "josh" };
	        var _Pipe1	= new FutureFilterPipe<String>(new IdentityPipe<String>());
	        _Pipe1.SetSourceCollection(_Names);
			
	        var _Counter = 0;
            while (_Pipe1.MoveNext())
            {
                _Counter++;
            }

			Assert.AreEqual(4, _Counter);

		}

        #endregion

		#region testAdvancedFutureFilter()

        [Test]
        public void testAdvancedFutureFilter()
        {

			var _Names = new List<String>() { "marko", "povel", "peter", "josh" };
	        var _Pipe  = new FutureFilterPipe<String>(new CollectionFilterPipe<String>(new List<String>() { "marko", "povel" }, ComparisonFilter.EQUAL));
	        _Pipe.SetSourceCollection(_Names);

			var _Counter = 0;
	        while (_Pipe.MoveNext())
			{
	            _Counter++;
	            var _Name = _Pipe.Current;
	            Assert.IsTrue(_Name.Equals("peter") || _Name.Equals("josh"));
	        }

	        Assert.AreEqual(2, _Counter);

		}

        #endregion
		
		#region testGraphFutureFilter()

        [Test]
        public void testGraphFutureFilter()
        {

			var _Graph 				= TinkerGraphFactory.CreateTinkerGraph();
	        var _Marko 				= _Graph.GetVertex(new VertexId(1));

	        var _OutEPipe 			= new VertexEdgePipe<VertexId,    RevisionId, String, Object,
                                                         EdgeId,      RevisionId, String, Object,
                                                         HyperEdgeId, RevisionId, String, Object>(Steps.VertexEdgeStep.OUT_EDGES);

	        var _InVPipe 			= new EdgeVertexPipe<VertexId,    RevisionId, String, Object,
                                                         EdgeId,      RevisionId, String, Object,
                                                         HyperEdgeId, RevisionId, String, Object>(Steps.EdgeVertexStep.IN_VERTEX);

	        var _PropertyFilterPipe = new PropertyFilterPipe<VertexId, RevisionId, String, Object, IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                                                                                   EdgeId,      RevisionId, String, Object,
                                                                                                                   HyperEdgeId, RevisionId, String, Object>, String>("name", "lop", ComparisonFilter.NOT_EQUAL);

	        var _FutureFilterPipe 	= new FutureFilterPipe<IPropertyEdge<VertexId,    RevisionId, String, Object,
                                                                         EdgeId,      RevisionId, String, Object,
                                                                         HyperEdgeId, RevisionId, String, Object>>(

                                          new Pipeline<IPropertyEdge<VertexId,    RevisionId, String, Object,
                                                                     EdgeId,      RevisionId, String, Object,
                                                                     HyperEdgeId, RevisionId, String, Object>,

                                                       IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                                       EdgeId,      RevisionId, String, Object,
                                                                       HyperEdgeId, RevisionId, String, Object>>(_InVPipe, _PropertyFilterPipe));

	        var _Pipeline 			= new Pipeline<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                                   EdgeId,      RevisionId, String, Object,
                                                                   HyperEdgeId, RevisionId, String, Object>,

                                                   IPropertyEdge<VertexId,    RevisionId, String, Object,
                                                                 EdgeId,      RevisionId, String, Object,
                                                                 HyperEdgeId, RevisionId, String, Object>>(_OutEPipe, _FutureFilterPipe);

	        _Pipeline.SetSourceCollection(new List<IPropertyVertex<VertexId,    RevisionId, String, Object,
                                                                   EdgeId,      RevisionId, String, Object,
                                                                   HyperEdgeId, RevisionId, String, Object>>() { _Marko });
	        
			int _Counter = 0;
	        while (_Pipeline.MoveNext())
			{
	            _Counter++;
                Assert.AreEqual(new EdgeId(9), _Pipeline.Current.Id);
	        }

			Assert.AreEqual(1, _Counter);

		}

        #endregion

	}
	
}
