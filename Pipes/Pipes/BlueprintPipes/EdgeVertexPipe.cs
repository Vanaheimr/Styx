/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/pipes.NET> <http://www.github.com/ahzf/pipes.NET>
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

using de.ahzf.blueprints.PropertyGraph;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The EdgeVertexPipe returns either the incoming or
    /// outgoing vertex of the given edge.
    /// </summary>
    public class EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>

                                : AbstractPipe<
                                     IPropertyEdge  <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                     TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                     TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>,
                                     IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
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

        private readonly Steps.EdgeVertexStep _Step;

        /// <summary>
        /// The stored OutVertex.
        /// </summary>
        protected        IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                         TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                         TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge> _StoredOutVertex;

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
                        _CurrentElement = _InternalEnumerator.Current.OutVertex as IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                                                   TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                                                   TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>;
                        return true;
                    }
                    return false;

                case Steps.EdgeVertexStep.IN_VERTEX:
                    if (_InternalEnumerator.MoveNext())
                    {
                        _CurrentElement = _InternalEnumerator.Current.InVertex as IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>;
                        return true;
                    }
                    return false;

                case Steps.EdgeVertexStep.BOTH_VERTICES:
                    {
                        if (_StoredOutVertex == null)
                        {
                            if (_InternalEnumerator.MoveNext())
                            {

                                _StoredOutVertex = _InternalEnumerator.Current.OutVertex as IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>;

                                _CurrentElement  = _InternalEnumerator.Current.InVertex as IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                                                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                                                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>;

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
