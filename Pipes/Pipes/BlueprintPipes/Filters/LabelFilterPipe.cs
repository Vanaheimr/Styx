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
    /// The LabelFilterPipe either allows or disallows all
    /// Edges that have the provided label.
    /// </summary>
    public class LabelFilterPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                 TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                 THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>

                    : AbstractComparisonFilterPipe<IPropertyEdge<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                 TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                 THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>,
                                                   String>

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

        private readonly String _Label;

        #endregion

        #region Constructor(s)

        #region LabelFilterPipe(myLabel, myComparisonFilter)

        /// <summary>
        /// Creates a new LabelFilterPipe.
        /// </summary>
        /// <param name="myLabel">The edge label.</param>
        /// <param name="myComparisonFilter">The filter to use.</param>
        public LabelFilterPipe(String myLabel, ComparisonFilter myComparisonFilter)
            : base(myComparisonFilter)
        {
            _Label = myLabel;
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

                    var _Edge = _InternalEnumerator.Current;

                    if (!CompareObjects(_Edge.Label, _Label))
                    {
                        _CurrentElement = _Edge;
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
            return base.ToString() + "<" + _Filter + "," + _Label + ">";
        }

        #endregion

    }

}
