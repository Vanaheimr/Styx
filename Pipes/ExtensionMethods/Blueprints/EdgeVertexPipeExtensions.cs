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

using System.Collections.Generic;

using de.ahzf.blueprints;

#endregion

namespace de.ahzf.Pipes.ExtensionMethods
{

    /// <summary>
    /// EdgeVertexPipe extensions.
    /// </summary>
    public static class EdgeVertexPipeExtensions
    {

        #region EdgeVertexPipe(this myIEnumerable, myStep)

        /// <summary>
        /// The EdgeVertexPipe returns either the incoming or
        /// outgoing vertex of the given edge.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IEdge.</param>
        /// <param name="myStep">Visiting only the outgoing vertex, only the incoming vertex or both.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> EdgeVertexPipe(this IEnumerable<IEdge> myIEnumerable, de.ahzf.Pipes.EdgeVertexPipe.Step myStep)
        {

            var _Pipe = new EdgeVertexPipe(myStep);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region InVertex(this myIEnumerable)

        /// <summary>
        /// This specialized EdgeVertexPipe returns just the InVertex
        /// of an IEdge.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IEdge.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> InVertex(this IEnumerable<IEdge> myIEnumerable)
        {

            var _Pipe = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region OutVertex(this myIEnumerable)

        /// <summary>
        /// This specialized EdgeVertexPipe returns just the OutVertex
        /// of an IEdge.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IEdge.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> OutVertex(this IEnumerable<IEdge> myIEnumerable)
        {

            var _Pipe = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.OUT_VERTEX);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region BothVertices(this myIEnumerable)

        /// <summary>
        /// This specialized EdgeVertexPipe returns both the InVertex
        /// and OutVertex of an IEdge.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IEdge.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> BothVertices(this IEnumerable<IEdge> myIEnumerable)
        {

            var _Pipe = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.BOTH_VERTICES);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion


        #region EdgeVertexPipe(this myIEnumerator, myStep)

        /// <summary>
        /// The EdgeVertexPipe returns either the incoming or
        /// outgoing vertex of the given edge.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IEdge.</param>
        /// <param name="myStep">Visiting only the outgoing vertex, only the incoming vertex or both.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> EdgeVertexPipe(this IEnumerator<IEdge> myIEnumerator, de.ahzf.Pipes.EdgeVertexPipe.Step myStep)
        {

            var _Pipe = new EdgeVertexPipe(myStep);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region EdgeVertexPipe(this myIEnumerator)

        /// <summary>
        /// This specialized EdgeVertexPipe returns just the InVertex
        /// of an IEdge.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IEdge.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> EdgeVertexPipe(this IEnumerator<IEdge> myIEnumerator)
        {

            var _Pipe = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region OutVertex(this myIEnumerator)

        /// <summary>
        /// This specialized EdgeVertexPipe returns just the OutVertex
        /// of an IEdge.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IEdge.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> OutVertex(this IEnumerator<IEdge> myIEnumerator)
        {

            var _Pipe = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.OUT_VERTEX);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region BothVertices(this myIEnumerator)

        /// <summary>
        /// This specialized EdgeVertexPipe returns both the InVertex
        /// and OutVertex of an IEdge.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IEdge.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> BothVertices(this IEnumerator<IEdge> myIEnumerator)
        {

            var _Pipe = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.BOTH_VERTICES);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

    }

}
