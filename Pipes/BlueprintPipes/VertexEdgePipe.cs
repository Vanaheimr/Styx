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
    /// outgoing edges of the given vertex.
    /// </summary>
    public class VertexEdgePipe : AbstractPipe<IVertex, IEdge>
    {

        #region Data

        private readonly Step _Step;

        /// <summary>
        /// Stores all edges not yet visited.
        /// </summary>
        protected IEnumerator<IEdge> _StoredEdges;

        #endregion

        #region Enum Step

        /// <summary>
        /// An enum for traversing edges starting at a vertex.
        /// </summary>
        public enum Step
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

        #region Constructor(s)

        #region VertexEdgePipe(myStep)

        /// <summary>
        /// The VertexEdgePipe returns either the incoming or
        /// outgoing Edges of the given vertex.
        /// </summary>
        /// <param name="myStep">Visiting only outgoing edges, only incoming edges or both.</param>
        public VertexEdgePipe(Step myStep)
        {
            _Step = myStep;
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

                if (_StoredEdges != null && _StoredEdges.MoveNext())
                {
                    _CurrentElement = _StoredEdges.Current;
                    return true;
                }

                else
                {

                    if (_InternalEnumerator.MoveNext())
                    {

                        switch (_Step)
                        {

                            case Step.OUT_EDGES:
                                
                                _StoredEdges = _InternalEnumerator.Current.OutEdges.GetEnumerator();

                                if (_StoredEdges.MoveNext())
                                {
                                    _CurrentElement = _StoredEdges.Current;
                                    return true;
                                }
                                else
                                    return false;


                            case Step.IN_EDGES:

                                _StoredEdges = _InternalEnumerator.Current.InEdges.GetEnumerator();

                                if (_StoredEdges.MoveNext())
                                {
                                    _CurrentElement = _StoredEdges.Current;
                                    return true;
                                }
                                else
                                    return false;


                            case Step.BOTH_EDGES:

                                var _IVertex = _InternalEnumerator.Current;
                                _StoredEdges = new MultiEnumerator<IEdge>(_IVertex.InEdges.GetEnumerator(), _IVertex.OutEdges.GetEnumerator());

                                if (_StoredEdges.MoveNext())
                                {
                                    _CurrentElement = _StoredEdges.Current;
                                    return true;
                                }
                                else
                                    return false;


                            // Should not happen, but makes the compiler happy!
                            default:
                                throw new IllegalStateException("This is an illegal state as there is no step set!");

                        }

                    }

                    else
                        return false;

                }                

            }

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + _Step + ">";
        }

        #endregion

    }


    #region Extensions

    /// <summary>
    /// Pipes extensions.
    /// </summary>
    public static partial class Extensions
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

    #endregion

}
