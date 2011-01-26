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

using de.ahzf.blueprints;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The GraphElementPipe takes a start of type Graph and will
    /// return elements (i.e. vertices or edges).
    /// This pipe is useful for processing all of the vertices (or edges) of a graph.
    /// </summary>
    public class GraphElementPipe<E> : AbstractPipe<IGraph, E>
        where E : IElement
    {

        #region Data

        protected          IEnumerator<E> _NextEnds;
        private   readonly ElementType    _ElementType;

        #endregion

        public enum ElementType
        {
            VERTEX,
            EDGE
        }

        #region Constructor(s)

        #region GraphElementPipe(myElementType)

        public GraphElementPipe(ElementType myElementType)
        {
            _ElementType = myElementType;
        }

        #endregion

        #endregion

        #region ProcessNextStart()

        protected override E ProcessNextStart()
        {

            while (true)
            {

                //if (_NextEnds != null && _NextEnds.hasNext())
                //    return _NextEnds.next();

                //else
                //{
                //    switch (_ElementType)
                //    {
                //        case ElementType.VERTEX:
                //            {
                //                _NextEnds = (IEnumerator<E>) _Starts.next().getVertices().iterator();
                //                break;
                //            }
                //        case ElementType.EDGE:
                //            {
                //                _NextEnds = (IEnumerator<E>) _Starts.next().getEdges().iterator();
                //                break;
                //            }
                //    }
                //}

            }

        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "<" + _ElementType + ">";
        }

        #endregion

    }

}
