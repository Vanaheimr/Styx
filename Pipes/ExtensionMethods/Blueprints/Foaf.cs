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
using System.Linq;
using System.Collections.Generic;

using de.ahzf.blueprints;

#endregion

namespace de.ahzf.Pipes.ExtensionMethods
{

    /// <summary>
    /// A class of specialized pipelines returning the
    /// 1-hop neighborhood (friend-of-a-friend) of a vertex.
    /// </summary>
    public static class FoafExtensions
    {

        #region Foaf(this myIEnumerable)

        /// <summary>
        /// A specialized pipeline returning the 1-hop neighborhood
        /// (friend-of-a-friend).
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IVertex.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> Foaf(this IEnumerable<IVertex> myIEnumerable)
        {

            var _Pipe1 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe1.SetSourceCollection(myIEnumerable);

            var _Pipe2 = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe2.SetSource(_Pipe1);

            var _Pipe3 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe3.SetSource(_Pipe2);

            var _Pipe4 = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe4.SetSource(_Pipe3);

            return _Pipe4;

        }

        #endregion

        #region Foaf(this myIEnumerable, myLabel)

        /// <summary>
        /// A specialized pipeline returning the 1-hop neighborhood
        /// (friend-of-a-friend) filtered by the connecting edge label.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> Foaf(this IEnumerable<IVertex> myIEnumerable, String myLabel)
        {

            var _Pipe1 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe1.SetSourceCollection(myIEnumerable);

            var _Pipe2 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe2.SetSource(_Pipe1);

            var _Pipe3 = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe3.SetSource(_Pipe2);

            var _Pipe4 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe4.SetSource(_Pipe3);

            var _Pipe5 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe5.SetSource(_Pipe4);

            var _Pipe6 = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe6.SetSource(_Pipe5);

            return _Pipe6;

        }

        #endregion


        #region Foaf(this myIEnumerable)

        /// <summary>
        /// A specialized pipeline returning the 1-hop neighborhood
        /// (friend-of-a-friend).
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IVertex.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> Foaf(this IEnumerator<IVertex> myIEnumerator)
        {

            var _Pipe1 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe1.SetSource(myIEnumerator);

            var _Pipe2 = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe2.SetSource(_Pipe1);

            var _Pipe3 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe3.SetSource(_Pipe2);

            var _Pipe4 = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe4.SetSource(_Pipe3);

            return _Pipe4;

        }

        #endregion

        #region Foaf(this myIEnumerable, myLabel)

        /// <summary>
        /// A specialized pipeline returning the 1-hop neighborhood
        /// (friend-of-a-friend) filtered by the connecting edge label.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IVertex.</returns>
        public static IEnumerable<IVertex> Foaf(this IEnumerator<IVertex> myIEnumerator, String myLabel)
        {

            var _Pipe1 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe1.SetSource(myIEnumerator);

            var _Pipe2 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe2.SetSource(_Pipe1);

            var _Pipe3 = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe3.SetSource(_Pipe2);

            var _Pipe4 = new VertexEdgePipe(Pipes.VertexEdgePipe.Step.OUT_EDGES);
            _Pipe4.SetSource(_Pipe3);

            var _Pipe5 = new LabelFilterPipe(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe5.SetSource(_Pipe4);

            var _Pipe6 = new EdgeVertexPipe(Pipes.EdgeVertexPipe.Step.IN_VERTEX);
            _Pipe6.SetSource(_Pipe5);

            return _Pipe6;

        }

        #endregion

    }

}
