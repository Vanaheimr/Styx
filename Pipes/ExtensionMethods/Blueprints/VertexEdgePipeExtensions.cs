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
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        /// <param name="myStep">Visiting only outgoing edges, only incoming edges or both.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> VertexEdgePipe(this IEnumerable<IPropertyVertex> myIEnumerable, de.ahzf.Pipes.VertexEdgePipe.Step myStep)
        {

            var _Pipe = new VertexEdgePipe(myStep);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region OutEdges(this myIEnumerable)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the OutEdges
        /// of an IPropertyVertex.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> OutEdges(this IEnumerable<IPropertyVertex> myIEnumerable)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region OutEdges(this myIEnumerable, myLabel)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the OutEdges
        /// of an IPropertyVertex filtered by their label.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> OutEdges(this IEnumerable<IPropertyVertex> myIEnumerable, String myLabel)
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
        /// of an IPropertyVertex.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> InEdges(this IEnumerable<IPropertyVertex> myIEnumerable)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.IN_EDGES);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region InEdges(this myIEnumerable, myLabel)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the InEdges
        /// of an IPropertyVertex filtered by their label.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> InEdges(this IEnumerable<IPropertyVertex> myIEnumerable, String myLabel)
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
        /// and OutEdges of an IPropertyVertex.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> BothEdges(this IEnumerable<IPropertyVertex> myIEnumerable)
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
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        /// <param name="myStep">Visiting only outgoing edges, only incoming edges or both.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> VertexEdgePipe(this IEnumerator<IPropertyVertex> myIEnumerator, de.ahzf.Pipes.VertexEdgePipe.Step myStep)
        {

            var _Pipe = new VertexEdgePipe(myStep);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region OutEdges(this myIEnumerator)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the OutEdges
        /// of an IPropertyVertex.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> OutEdges(this IEnumerator<IPropertyVertex> myIEnumerator)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region OutEdges(this myIEnumerator, myLabel)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the OutEdges
        /// of an IPropertyVertex filtered by their label.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> OutEdges(this IEnumerator<IPropertyVertex> myIEnumerator, String myLabel)
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
        /// of an IPropertyVertex.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> InEdges(this IEnumerator<IPropertyVertex> myIEnumerator)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.IN_EDGES);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region InEdges(this myIEnumerator, myLabel)

        /// <summary>
        /// This specialized VertexEdgePipe returns just the InEdges
        /// of an IPropertyVertex filtered by their label.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> InEdges(this IEnumerator<IPropertyVertex> myIEnumerator, String myLabel)
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
        /// and OutEdges of an IPropertyVertex.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        /// <returns>A collection of objects implementing IPropertyEdge.</returns>
        public static IEnumerable<IPropertyEdge> BothEdges(this IEnumerator<IPropertyVertex> myIEnumerator)
        {

            var _Pipe = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.BOTH_EDGES);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

    }

}
