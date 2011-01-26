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
    /// outgoing Vertex of an Edge start.
    /// </summary>
    public class EdgeVertexPipe : AbstractPipe<IEdge, IVertex>
    {

        #region Data

        private readonly Step    _Step;
        private          IVertex _StoredOutVertex;

        #endregion

        public enum Step
        {
            IN_VERTEX,
            OUT_VERTEX,
            BOTH_VERTICES
        }

        #region Constructor(s)

        #region EdgeVertexPipe(myStep)

        public EdgeVertexPipe(Step myStep)
        {
            _Step = myStep;
        }

        #endregion

        #endregion

        #region ProcessNextStart()

        protected override IVertex ProcessNextStart()
        {

            switch (_Step)
            {

                case Step.OUT_VERTEX:
                    _Starts.MoveNext();
                    return _Starts.Current.OutVertex;

                case Step.IN_VERTEX:
                    _Starts.MoveNext();
                    return _Starts.Current.InVertex;

                case Step.BOTH_VERTICES:
                    {
                        if (_StoredOutVertex == null)
                        {
                            _Starts.MoveNext();
                            _StoredOutVertex = _Starts.Current.OutVertex;
                            return _Starts.Current.InVertex;
                        }
                        else
                        {
                            var _Temp = _StoredOutVertex;
                            _StoredOutVertex = null;
                            return _Temp;
                        }
                    }

                // Should not happen, but makes the compiler happy!
                default: throw new IllegalStateException("This is an illegal state as there is no step set!");

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
