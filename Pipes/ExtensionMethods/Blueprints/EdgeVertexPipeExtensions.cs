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

#region Usings

using System;
using System.Collections.Generic;

using de.ahzf.blueprints.PropertyGraph;

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
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyEdge.</param>
        /// <param name="myStep">Visiting only the outgoing vertex, only the incoming vertex or both.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                     TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                     TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                                     this IEnumerable<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                    TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                    TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerable,

                                     Steps.EdgeVertexStep myStep)

            where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
            where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
            where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable

            where TIdVertex               : IEquatable<TIdVertex>,            IComparable<TIdVertex>,            IComparable, TValueVertex
            where TIdEdge                 : IEquatable<TIdEdge>,              IComparable<TIdEdge>,              IComparable, TValueEdge
            where TIdHyperEdge            : IEquatable<TIdHyperEdge>,         IComparable<TIdHyperEdge>,         IComparable, TValueHyperEdge

            where TRevisionIdVertex       : IEquatable<TRevisionIdVertex>,    IComparable<TRevisionIdVertex>,    IComparable, TValueVertex
            where TRevisionIdEdge         : IEquatable<TRevisionIdEdge>,      IComparable<TRevisionIdEdge>,      IComparable, TValueEdge
            where TRevisionIdHyperEdge    : IEquatable<TRevisionIdHyperEdge>, IComparable<TRevisionIdHyperEdge>, IComparable, TValueHyperEdge

        {

            var _Pipe = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(myStep);

            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region InVertex(this myIEnumerable)

        /// <summary>
        /// This specialized EdgeVertexPipe returns just the InVertex
        /// of an IPropertyEdge.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyEdge.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      InVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                               TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                               TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(
            
                               this IEnumerable<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                              TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                              TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerable)

            where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
            where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
            where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable

            where TIdVertex               : IEquatable<TIdVertex>,            IComparable<TIdVertex>,            IComparable, TValueVertex
            where TIdEdge                 : IEquatable<TIdEdge>,              IComparable<TIdEdge>,              IComparable, TValueEdge
            where TIdHyperEdge            : IEquatable<TIdHyperEdge>,         IComparable<TIdHyperEdge>,         IComparable, TValueHyperEdge

            where TRevisionIdVertex       : IEquatable<TRevisionIdVertex>,    IComparable<TRevisionIdVertex>,    IComparable, TValueVertex
            where TRevisionIdEdge         : IEquatable<TRevisionIdEdge>,      IComparable<TRevisionIdEdge>,      IComparable, TValueEdge
            where TRevisionIdHyperEdge    : IEquatable<TRevisionIdHyperEdge>, IComparable<TRevisionIdHyperEdge>, IComparable, TValueHyperEdge

        {

            var _Pipe = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                                           Steps.EdgeVertexStep.IN_VERTEX);

            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region OutVertex(this myIEnumerable)

        /// <summary>
        /// This specialized EdgeVertexPipe returns just the OutVertex
        /// of an IPropertyEdge.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyEdge.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      OutVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                                this IEnumerable<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                               TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                               TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerable)
        
            where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
            where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
            where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable

            where TIdVertex               : IEquatable<TIdVertex>,            IComparable<TIdVertex>,            IComparable, TValueVertex
            where TIdEdge                 : IEquatable<TIdEdge>,              IComparable<TIdEdge>,              IComparable, TValueEdge
            where TIdHyperEdge            : IEquatable<TIdHyperEdge>,         IComparable<TIdHyperEdge>,         IComparable, TValueHyperEdge

            where TRevisionIdVertex       : IEquatable<TRevisionIdVertex>,    IComparable<TRevisionIdVertex>,    IComparable, TValueVertex
            where TRevisionIdEdge         : IEquatable<TRevisionIdEdge>,      IComparable<TRevisionIdEdge>,      IComparable, TValueEdge
            where TRevisionIdHyperEdge    : IEquatable<TRevisionIdHyperEdge>, IComparable<TRevisionIdHyperEdge>, IComparable, TValueHyperEdge

        {

            var _Pipe = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.OUT_VERTEX);

            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region BothVertices(this myIEnumerable)

        /// <summary>
        /// This specialized EdgeVertexPipe returns both the InVertex
        /// and OutVertex of an IPropertyEdge.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyEdge.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      BothVertices<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                   TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                   TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                                   this IEnumerable<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerable)
        
            where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
            where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
            where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable

            where TIdVertex               : IEquatable<TIdVertex>,            IComparable<TIdVertex>,            IComparable, TValueVertex
            where TIdEdge                 : IEquatable<TIdEdge>,              IComparable<TIdEdge>,              IComparable, TValueEdge
            where TIdHyperEdge            : IEquatable<TIdHyperEdge>,         IComparable<TIdHyperEdge>,         IComparable, TValueHyperEdge

            where TRevisionIdVertex       : IEquatable<TRevisionIdVertex>,    IComparable<TRevisionIdVertex>,    IComparable, TValueVertex
            where TRevisionIdEdge         : IEquatable<TRevisionIdEdge>,      IComparable<TRevisionIdEdge>,      IComparable, TValueEdge
            where TRevisionIdHyperEdge    : IEquatable<TRevisionIdHyperEdge>, IComparable<TRevisionIdHyperEdge>, IComparable, TValueHyperEdge

        {

            var _Pipe = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.BOTH_VERTICES);

            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion


        #region EdgeVertexPipe(this myIEnumerator, myStep)

        /// <summary>
        /// The EdgeVertexPipe returns either the incoming or
        /// outgoing vertex of the given edge.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyEdge.</param>
        /// <param name="myStep">Visiting only the outgoing vertex, only the incoming vertex or both.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                     TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                     TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                                     this IEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                    TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                    TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerator,
                                     Steps.EdgeVertexStep myStep)
        
            where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
            where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
            where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable

            where TIdVertex               : IEquatable<TIdVertex>,            IComparable<TIdVertex>,            IComparable, TValueVertex
            where TIdEdge                 : IEquatable<TIdEdge>,              IComparable<TIdEdge>,              IComparable, TValueEdge
            where TIdHyperEdge            : IEquatable<TIdHyperEdge>,         IComparable<TIdHyperEdge>,         IComparable, TValueHyperEdge

            where TRevisionIdVertex       : IEquatable<TRevisionIdVertex>,    IComparable<TRevisionIdVertex>,    IComparable, TValueVertex
            where TRevisionIdEdge         : IEquatable<TRevisionIdEdge>,      IComparable<TRevisionIdEdge>,      IComparable, TValueEdge
            where TRevisionIdHyperEdge    : IEquatable<TRevisionIdHyperEdge>, IComparable<TRevisionIdHyperEdge>, IComparable, TValueHyperEdge
            
        {

            var _Pipe = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(myStep);

            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region InVertex(this myIEnumerator)

        /// <summary>
        /// This specialized EdgeVertexPipe returns just the InVertex
        /// of an IPropertyEdge.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyEdge.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      InVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                               TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                               TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                               this IEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                              TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                              TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerator)
        
            where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
            where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
            where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable

            where TIdVertex               : IEquatable<TIdVertex>,            IComparable<TIdVertex>,            IComparable, TValueVertex
            where TIdEdge                 : IEquatable<TIdEdge>,              IComparable<TIdEdge>,              IComparable, TValueEdge
            where TIdHyperEdge            : IEquatable<TIdHyperEdge>,         IComparable<TIdHyperEdge>,         IComparable, TValueHyperEdge

            where TRevisionIdVertex       : IEquatable<TRevisionIdVertex>,    IComparable<TRevisionIdVertex>,    IComparable, TValueVertex
            where TRevisionIdEdge         : IEquatable<TRevisionIdEdge>,      IComparable<TRevisionIdEdge>,      IComparable, TValueEdge
            where TRevisionIdHyperEdge    : IEquatable<TRevisionIdHyperEdge>, IComparable<TRevisionIdHyperEdge>, IComparable, TValueHyperEdge
            
        {

            var _Pipe = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);

            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region OutVertex(this myIEnumerator)

        /// <summary>
        /// This specialized EdgeVertexPipe returns just the OutVertex
        /// of an IPropertyEdge.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyEdge.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      OutVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                                this IEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                               TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                               TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerator)

            where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
            where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
            where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable

            where TIdVertex               : IEquatable<TIdVertex>,            IComparable<TIdVertex>,            IComparable, TValueVertex
            where TIdEdge                 : IEquatable<TIdEdge>,              IComparable<TIdEdge>,              IComparable, TValueEdge
            where TIdHyperEdge            : IEquatable<TIdHyperEdge>,         IComparable<TIdHyperEdge>,         IComparable, TValueHyperEdge

            where TRevisionIdVertex       : IEquatable<TRevisionIdVertex>,    IComparable<TRevisionIdVertex>,    IComparable, TValueVertex
            where TRevisionIdEdge         : IEquatable<TRevisionIdEdge>,      IComparable<TRevisionIdEdge>,      IComparable, TValueEdge
            where TRevisionIdHyperEdge    : IEquatable<TRevisionIdHyperEdge>, IComparable<TRevisionIdHyperEdge>, IComparable, TValueHyperEdge
            
        {

            var _Pipe = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.OUT_VERTEX);

            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region BothVertices(this myIEnumerator)

        /// <summary>
        /// This specialized EdgeVertexPipe returns both the InVertex
        /// and OutVertex of an IPropertyEdge.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyEdge.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      BothVertices<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                   TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                   TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                                   this IEnumerator<IPropertyEdge<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerator)

            where TKeyVertex              : IEquatable<TKeyVertex>,           IComparable<TKeyVertex>,           IComparable
            where TKeyEdge                : IEquatable<TKeyEdge>,             IComparable<TKeyEdge>,             IComparable
            where TKeyHyperEdge           : IEquatable<TKeyHyperEdge>,        IComparable<TKeyHyperEdge>,        IComparable

            where TIdVertex               : IEquatable<TIdVertex>,            IComparable<TIdVertex>,            IComparable, TValueVertex
            where TIdEdge                 : IEquatable<TIdEdge>,              IComparable<TIdEdge>,              IComparable, TValueEdge
            where TIdHyperEdge            : IEquatable<TIdHyperEdge>,         IComparable<TIdHyperEdge>,         IComparable, TValueHyperEdge

            where TRevisionIdVertex       : IEquatable<TRevisionIdVertex>,    IComparable<TRevisionIdVertex>,    IComparable, TValueVertex
            where TRevisionIdEdge         : IEquatable<TRevisionIdEdge>,      IComparable<TRevisionIdEdge>,      IComparable, TValueEdge
            where TRevisionIdHyperEdge    : IEquatable<TRevisionIdHyperEdge>, IComparable<TRevisionIdHyperEdge>, IComparable, TValueHyperEdge
            
        {

            var _Pipe = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.BOTH_VERTICES);

            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

    }

}
