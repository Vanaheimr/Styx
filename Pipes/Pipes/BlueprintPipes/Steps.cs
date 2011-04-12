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

namespace de.ahzf.Pipes
{

    /// <summary>
    /// Traversal Steps
    /// </summary>
    public class Steps
    {

        #region Enum EdgeVertexStep

        /// <summary>
        /// An enum for traversing vertices starting at an edge.
        /// </summary>
        public enum EdgeVertexStep
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

        #region Enum VertexEdgeStep

        /// <summary>
        /// An enum for traversing edges starting at a vertex.
        /// </summary>
        public enum VertexEdgeStep
        {

            /// <summary>
            /// Only traverse the outgoing edges.
            /// </summary>
            OUT_EDGES,

            /// <summary>
            /// Only traverse the incoming edges.
            /// </summary>
            IN_EDGES,

            /// <summary>
            /// Traverse both incoming and outgoing edges.
            /// </summary>
            BOTH_EDGES

        }

        #endregion

        #region Enum ElementType

        /// <summary>
        /// The IElement to return.
        /// </summary>
        public enum ElementType
        {

            /// <summary>
            /// Return the vertex.
            /// </summary>
            VERTEX,

            /// <summary>
            /// Return the edge.
            /// </summary>
            EDGE

        }

        #endregion

    }

}
