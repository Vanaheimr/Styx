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

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The GraphElementPipe takes a start of type IPropertyGraph and will
    /// return elements (i.e. vertices or edges).
    /// This pipe is useful for processing all of the vertices (or edges) of a graph.
    /// </summary>
    public class GraphElementPipe<TId, TRevisionId, TKey, TValue, E>
                    : AbstractPipe<IPropertyGraph, E>

        where TId            : IEquatable<TId>,         IComparable<TId>,         IComparable, TValue
        where TRevisionId    : IEquatable<TRevisionId>, IComparable<TRevisionId>, IComparable, TValue
        where TKey           : IEquatable<TKey>,        IComparable<TKey>,        IComparable
        where E              : IPropertyElement<TId, TRevisionId, TKey, TValue>

    {

        #region Data

        private readonly Steps.ElementType _ElementType;

        /// <summary>
        /// Stores all IElements not yet visited.
        /// </summary>
        protected IEnumerator<E> _StoredIElements;

        #endregion

        #region Constructor(s)

        #region GraphElementPipe(myElementType)

        /// <summary>
        /// The GraphElementPipe takes a start of type IPropertyGraph and will
        /// return elements (i.e. vertices or edges).
        /// This pipe is useful for processing all of the vertices (or edges) of a graph.
        /// </summary>
        /// <param name="myElementType">Return vertex or edge.</param>
        public GraphElementPipe(Steps.ElementType myElementType)
        {
            _ElementType = myElementType;
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

                if (_StoredIElements != null && _StoredIElements.MoveNext())
                {
                    _CurrentElement = _StoredIElements.Current;
                    return true;
                }

                else
                {

                    if (_InternalEnumerator.MoveNext())
                    {

                        switch (_ElementType)
                        {

                            case Steps.ElementType.VERTEX:

                                _StoredIElements = (IEnumerator<E>) _InternalEnumerator.Current.GetVertices().GetEnumerator();

                                if (_StoredIElements.MoveNext())
                                {
                                    _CurrentElement = _StoredIElements.Current;
                                    return true;
                                }
                                else
                                    return false;


                            case Steps.ElementType.EDGE:
                                    
                                _StoredIElements = (IEnumerator<E>) _InternalEnumerator.Current.GetEdges().GetEnumerator();

                                if (_StoredIElements.MoveNext())
                                {
                                    _CurrentElement = _StoredIElements.Current;
                                    return true;
                                }
                                else
                                    return false;

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
            return base.ToString() + "<" + _ElementType + ">";
        }

        #endregion

    }

}
