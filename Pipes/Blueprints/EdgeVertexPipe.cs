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

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The EdgeVertexPipe returns either the incoming or
    /// outgoing vertex of the given edge.
    /// </summary>
    public class EdgeVertexPipe : AbstractPipe<IEdge, IVertex>
    {

        #region Data

        private readonly Step    _Step;
        private          IVertex _StoredOutVertex;

        #endregion

        #region Enum Step

        /// <summary>
        /// An enum for traversing vertices starting at an edge.
        /// </summary>
        public enum Step
        {

            /// <summary>
            /// Only traverse the incoming vertex.
            /// </summary>
            IN_VERTEX,

            /// <summary>
            /// Only traverse the outgoing vertex.
            /// </summary>
            OUT_VERTEX,

            /// <summary>
            /// Traverse both incoming and outgoing vertex.
            /// </summary>
            BOTH_VERTICES

        }

        #endregion

        #region Constructor(s)

        #region EdgeVertexPipe(myStep)

        /// <summary>
        /// The EdgeVertexPipe returns either the incoming or
        /// outgoing vertex of the given edge.
        /// </summary>
        /// <param name="myStep">Visiting only the outgoing vertex, only the incoming vertex or both.</param>
        public EdgeVertexPipe(Step myStep)
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

                case Step.OUT_VERTEX:
                    _InternalEnumerator.MoveNext();
                    _CurrentElement = _InternalEnumerator.Current.OutVertex;
                    break;

                case Step.IN_VERTEX:
                    _InternalEnumerator.MoveNext();
                    _CurrentElement = _InternalEnumerator.Current.InVertex;
                    break;

                case Step.BOTH_VERTICES:
                    {
                        if (_StoredOutVertex == null)
                        {
                            _InternalEnumerator.MoveNext();
                            _StoredOutVertex = _InternalEnumerator.Current.OutVertex;
                            _CurrentElement  = _InternalEnumerator.Current.InVertex;
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
