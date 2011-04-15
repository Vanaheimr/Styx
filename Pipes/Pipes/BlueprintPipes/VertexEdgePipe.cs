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

using de.ahzf.blueprints.PropertyGraph;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The VertexEdgePipe returns either the incoming or
    /// outgoing edges of the given vertex.
    /// </summary>
    public class VertexEdgePipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>

                                : AbstractPipe<

                                    IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                    TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                    TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>,

                                    IPropertyEdge  <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                    TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                    TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

        where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
        where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
        where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable

        where TIdVertex               : IEquatable<TIdVertex>,            IComparable<TIdVertex>,            IComparable, TValueVertex
        where TIdEdge                 : IEquatable<TIdEdge>,              IComparable<TIdEdge>,              IComparable, TValueEdge
        where TIdHyperEdge            : IEquatable<TIdHyperEdge>,         IComparable<TIdHyperEdge>,         IComparable, TValueHyperEdge

        where TRevisionIdVertex       : IEquatable<TRevisionIdVertex>,    IComparable<TRevisionIdVertex>,    IComparable, TValueVertex
        where TRevisionIdEdge         : IEquatable<TRevisionIdEdge>,      IComparable<TRevisionIdEdge>,      IComparable, TValueEdge
        where TRevisionIdHyperEdge    : IEquatable<TRevisionIdHyperEdge>, IComparable<TRevisionIdHyperEdge>, IComparable, TValueHyperEdge

    {

        #region Data

        private readonly Steps.VertexEdgeStep _Step;

        /// <summary>
        /// Stores all edges not yet visited.
        /// </summary>
        protected IEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> _StoredEdges;

        #endregion

        #region Constructor(s)

        #region VertexEdgePipe(myStep)

        /// <summary>
        /// The VertexEdgePipe returns either the incoming or
        /// outgoing Edges of the given vertex.
        /// </summary>
        /// <param name="myStep">Visiting only outgoing edges, only incoming edges or both.</param>
        public VertexEdgePipe(Steps.VertexEdgeStep myStep)
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

            while (true)
            {

                if (_StoredEdges != null && _StoredEdges.MoveNext())
                {
                    _CurrentElement = _StoredEdges.Current;
                    return true;
                }

                else
                {

                    if (_InternalEnumerator.MoveNext())
                    {

                        switch (_Step)
                        {

                            case Steps.VertexEdgeStep.OUT_EDGES:
                                
                                _StoredEdges = _InternalEnumerator.Current.OutEdges.GetEnumerator() as IEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                                                                                 TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                                                                                 TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>;

                                if (_StoredEdges.MoveNext())
                                {
                                    _CurrentElement = _StoredEdges.Current;
                                    return true;
                                }
                                else
                                    return false;


                            case Steps.VertexEdgeStep.IN_EDGES:

                                _StoredEdges = _InternalEnumerator.Current.InEdges.GetEnumerator() as IEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                                                                                TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                                                                                TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>;

                                if (_StoredEdges.MoveNext())
                                {
                                    _CurrentElement = _StoredEdges.Current;
                                    return true;
                                }
                                else
                                    return false;


                            case Steps.VertexEdgeStep.BOTH_EDGES:

                                var _IPropertyVertex = _InternalEnumerator.Current;

                                _StoredEdges = new MultiEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                                 TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                                 TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>(
                                                                                 
                                                                                 _IPropertyVertex.InEdges.GetEnumerator()  as IEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                                                                                                        TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                                                                                                        TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>,

                                                                                 _IPropertyVertex.OutEdges.GetEnumerator() as IEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                                                                                                        TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                                                                                                        TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>);

                                if (_StoredEdges.MoveNext())
                                {
                                    _CurrentElement = _StoredEdges.Current;
                                    return true;
                                }
                                else
                                    return false;


                            // Should not happen, but makes the compiler happy!
                            default:
                                throw new IllegalStateException("This is an illegal state as there is no step set!");

                        }

                    }

                    else
                        return false;

                }                

            }

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
