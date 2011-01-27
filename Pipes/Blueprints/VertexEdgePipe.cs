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
    /// The VertexEdgePipe returns either the incoming or
    /// outgoing Edges of the Vertex start.
    /// </summary>
    public class VertexEdgePipe : AbstractPipe<IVertex, IEdge>
    {

        #region Data

        private   readonly Step               _Step;
        protected          IEnumerator<IEdge> _NextEnds;

        #endregion

        public enum Step
        {
            OUT_EDGES,
            IN_EDGES,
            BOTH_EDGES
        }

        #region Constructor(s)

        #region VertexEdgePipe(myStep)

        public VertexEdgePipe(Step myStep)
        {
            _Step = myStep;
        }

        #endregion

        #endregion

        #region MoveNext()

        public override Boolean MoveNext()
        {
            while (true)
            {

                if (_NextEnds != null && _NextEnds.MoveNext())
                {
                    _CurrentItem = _NextEnds.Current;
                    return true;
                }

                else
                {
                    switch (_Step)
                    {

                        case Step.OUT_EDGES:
                            {
                                _Starts.MoveNext();
                                _NextEnds = _Starts.Current.OutEdges.GetEnumerator();
                                break;
                            }

                        case Step.IN_EDGES:
                            {
                                _Starts.MoveNext();
                                _NextEnds = _Starts.Current.InEdges.GetEnumerator();
                                break;
                            }

                        case Step.BOTH_EDGES:
                            {
                                _Starts.MoveNext();
                                var _IVertex = _Starts.Current;
                                _NextEnds = new MultiEnumerator<IEdge>(_IVertex.InEdges.GetEnumerator(), _IVertex.OutEdges.GetEnumerator());
                                break;
                            }

                        // Should not happen, but makes the compiler happy!
                        default: throw new IllegalStateException("This is an illegal state as there is no step set!");

                    }
                }

            }
        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "<" + _Step + ">";
        }

        #endregion

    }

}
