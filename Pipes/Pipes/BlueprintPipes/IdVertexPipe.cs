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

using de.ahzf.blueprints;
using de.ahzf.blueprints.Datastructures;
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The IdVertexPipe will convert the given VertexIds into the
    /// corresponding vertices of the given graph.
    /// </summary>
    public class IdVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                              TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                              THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge,
                              TGraphDatastructure>

                              : AbstractPipe<TVertexId, IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                        TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                        THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>>

        where TDatastructureVertex    : IDictionary<TKeyVertex,    TValueVertex>
        where TDatastructureEdge      : IDictionary<TKeyEdge,      TValueEdge>
        where TDatastructureHyperEdge : IDictionary<TKeyHyperEdge, TValueHyperEdge>

        where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
        where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
        where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable
                                                                                                            
        where TVertexId               : IEquatable<TVertexId>,            IComparable<TVertexId>,            IComparable, TValueVertex
        where TEdgeId                 : IEquatable<TEdgeId>,              IComparable<TEdgeId>,              IComparable, TValueEdge
        where THyperEdgeId            : IEquatable<THyperEdgeId>,         IComparable<THyperEdgeId>,         IComparable, TValueHyperEdge

        where TVertexRevisionId       : IEquatable<TVertexRevisionId>,    IComparable<TVertexRevisionId>,    IComparable, TValueVertex
        where TEdgeRevisionId         : IEquatable<TEdgeRevisionId>,      IComparable<TEdgeRevisionId>,      IComparable, TValueEdge
        where THyperEdgeRevisionId    : IEquatable<THyperEdgeRevisionId>, IComparable<THyperEdgeRevisionId>, IComparable, TValueHyperEdge

    {

        #region Data

        private readonly IPropertyGraph<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                        TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                        THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge,
                                        TGraphDatastructure> _IPropertyGraph;

        #endregion

        #region Constructor(s)

        #region IdVertexPipe(myIPropertyGraph)

        /// <summary>
        /// Creates a new IdVertexPipe.
        /// </summary>
        /// <param name="myIPropertyGraph">The IPropertyGraph to use.</param>
        public IdVertexPipe(IPropertyGraph<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge,
                                           TGraphDatastructure> myIPropertyGraph)
        {
            _IPropertyGraph = myIPropertyGraph;
        }

        #endregion

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (_InternalEnumerator == null)
                return false;

            if (_InternalEnumerator.MoveNext())
            {
                _CurrentElement = _IPropertyGraph.GetVertex(_InternalEnumerator.Current) as IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                                                            TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                                                            THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>;
                return true;
            }

            return false;

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + _InternalEnumerator.Current + ">";
        }

        #endregion

    }

}
