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

namespace de.ahzf.Pipes.ExtensionMethods
{

    /// <summary>
    /// VertexEdgePipe extensions.
    /// </summary>
    public static class VertexEdgePipeExtensions
    {

        #region VertexEdgePipe(this myIEnumerable, myStep)

        /// <summary>
        /// The VertexEdgePipe returns either the incoming or
        /// outgoing edges of the given vertex.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IVertex.</param>
        /// <param name="myStep">Visiting only outgoing edges, only incoming edges or both.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> VertexEdgePipe(this IEnumerable<IVertex> myIEnumerable, de.ahzf.Pipes.VertexEdgePipe.Step myStep)
        {

            var _Pipe = new VertexEdgePipe(myStep);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region OutEdges(this myIEnumerable)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the OutEdges
        /// of an IVertex.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IVertex.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> OutEdges(this IEnumerable<IVertex> myIEnumerable)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region OutEdges(this myIEnumerable, myLabel)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the OutEdges
        /// of an IVertex filtered by their label.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> OutEdges(this IEnumerable<IVertex> myIEnumerable, String myLabel)
        {

            var _Pipe1 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe1.SetSourceCollection(myIEnumerable);

            var _Pipe2 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe2.SetSource(_Pipe1);

            return _Pipe2;

        }

        #endregion

        #region InEdges(this myIEnumerable)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the InEdges
        /// of an IVertex.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IVertex.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> InEdges(this IEnumerable<IVertex> myIEnumerable)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.IN_EDGES);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region InEdges(this myIEnumerable, myLabel)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the InEdges
        /// of an IVertex filtered by their label.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> InEdges(this IEnumerable<IVertex> myIEnumerable, String myLabel)
        {

            var _Pipe1 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.IN_EDGES);
            _Pipe1.SetSourceCollection(myIEnumerable);

            var _Pipe2 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe2.SetSource(_Pipe1);

            return _Pipe2;

        }

        #endregion

        #region BothEdges(this myIEnumerable)

        /// <summary>
        /// This specialized VertexEdgePipe returns both the InEdges
        /// and OutEdges of an IVertex.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IVertex.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> BothEdges(this IEnumerable<IVertex> myIEnumerable)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.BOTH_EDGES);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion


        #region VertexEdgePipe(this myIEnumerator, myStep)

        /// <summary>
        /// The VertexEdgePipe returns either the incoming or
        /// outgoing edges of the given vertex.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IVertex.</param>
        /// <param name="myStep">Visiting only outgoing edges, only incoming edges or both.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> VertexEdgePipe(this IEnumerator<IVertex> myIEnumerator, de.ahzf.Pipes.VertexEdgePipe.Step myStep)
        {

            var _Pipe = new VertexEdgePipe(myStep);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region OutEdges(this myIEnumerator)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the OutEdges
        /// of an IVertex.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IVertex.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> OutEdges(this IEnumerator<IVertex> myIEnumerator)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region OutEdges(this myIEnumerator, myLabel)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the OutEdges
        /// of an IVertex filtered by their label.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> OutEdges(this IEnumerator<IVertex> myIEnumerator, String myLabel)
        {

            var _Pipe1 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe1.SetSource(myIEnumerator);

            var _Pipe2 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe2.SetSource(_Pipe1);

            return _Pipe2;

        }

        #endregion

        #region InEdges(this myIEnumerator)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the InEdges
        /// of an IVertex.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IVertex.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> InEdges(this IEnumerator<IVertex> myIEnumerator)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.IN_EDGES);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region InEdges(this myIEnumerator, myLabel)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the InEdges
        /// of an IVertex filtered by their label.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> InEdges(this IEnumerator<IVertex> myIEnumerator, String myLabel)
        {

            var _Pipe1 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.IN_EDGES);
            _Pipe1.SetSource(myIEnumerator);

            var _Pipe2 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe2.SetSource(_Pipe1);

            return _Pipe2;

        }

        #endregion

        #region BothEdges(this myIEnumerator)

        /// <summary>
        /// This specialized VertexEdgePipe returns both the InEdges
        /// and OutEdges of an IVertex.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IVertex.</param>
        /// <returns>A collection of objects implementing IEdge.</returns>
        public static IEnumerable<IEdge> BothEdges(this IEnumerator<IVertex> myIEnumerator)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.BOTH_EDGES);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

    }

}
