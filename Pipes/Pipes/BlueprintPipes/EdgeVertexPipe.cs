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
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The EdgeVertexPipe returns either the incoming or
    /// outgoing vertex of the given edge.
    /// </summary>
    public class EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>

                                : AbstractPipe<
                                     IPropertyEdge  <TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                     TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                     THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>,
                                     IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
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

        private readonly Steps.EdgeVertexStep _Step;

        /// <summary>
        /// The stored OutVertex.
        /// </summary>
        protected        IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                         TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                         THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge> _StoredOutVertex;

        #endregion


        #region Constructor(s)

        #region EdgeVertexPipe(myStep)

        /// <summary>
        /// The EdgeVertexPipe returns either the incoming or
        /// outgoing vertex of the given edge.
        /// </summary>
        /// <param name="myStep">Visiting only the outgoing vertex, only the incoming vertex or both.</param>
        public EdgeVertexPipe(Steps.EdgeVertexStep myStep)
        {
            _Step = myStep;
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

            switch (_Step)
            {

                case Steps.EdgeVertexStep.OUT_VERTEX:
                    if (_InternalEnumerator.MoveNext())
                    {
                        _CurrentElement = _InternalEnumerator.Current.OutVertex as IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                                                   TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                                                   THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>;
                        return true;
                    }
                    return false;

                case Steps.EdgeVertexStep.IN_VERTEX:
                    if (_InternalEnumerator.MoveNext())
                    {
                        _CurrentElement = _InternalEnumerator.Current.InVertex as IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>;
                        return true;
                    }
                    return false;

                case Steps.EdgeVertexStep.BOTH_VERTICES:
                    {
                        if (_StoredOutVertex == null)
                        {
                            if (_InternalEnumerator.MoveNext())
                            {

                                _StoredOutVertex = _InternalEnumerator.Current.OutVertex as IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                                                            TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                                                            THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>;

                                _CurrentElement  = _InternalEnumerator.Current.InVertex as IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>;

                                return true;

                            }
                            return false;
                        }
                        else
                        {
                            var _Temp = _StoredOutVertex;
                            _StoredOutVertex = null;
                            _CurrentElement  = _Temp;
                        }
                    }
                    break;

                // Should not happen, but makes the compiler happy!
                default: throw new IllegalStateException("This is an illegal state as there is no step set!");

            }

            return true;

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + _Step + ">";
        }

        #endregion

    }

}
