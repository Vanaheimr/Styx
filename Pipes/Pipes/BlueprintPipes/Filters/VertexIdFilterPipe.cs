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

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The IdPipe will return the Id of the given graph element.
    /// </summary>
    public class VertexIdFilterPipe : AbstractComparisonFilterPipe<IPropertyVertex, VertexId>
    {

        #region Data

        private readonly VertexId _VertexId;

        #endregion

        #region Constructor(s)

        #region IdFilterPipe(VertexId, myComparisonFilter)

        /// <summary>
        /// Creates a new VertexIdFilterPipe.
        /// </summary>
        /// <param name="myVertexId">The Id of the IElement.</param>
        /// <param name="myComparisonFilter">The filter to use.</param>
        public VertexIdFilterPipe(VertexId myVertexId, ComparisonFilter myComparisonFilter)
            : base(myComparisonFilter)
        {
            _VertexId = myVertexId;
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

                if (_InternalEnumerator.MoveNext())
                {

                    var _Vertex = _InternalEnumerator.Current;

                    if (!CompareObjects(_Vertex.Id, _VertexId))
                    {
                        _CurrentElement = _Vertex;
                        return true;
                    }

                }

                else
                    return false;

            }

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
