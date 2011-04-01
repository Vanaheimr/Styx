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
using System;

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
        public static IEnumerable<IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>>

                      EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                     TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                     THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(

                                     this IEnumerable<IPropertyEdge<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                    TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                    THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>> myIEnumerable,

                                     Steps.EdgeVertexStep myStep)

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

            var _Pipe = new EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(myStep);

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
        public static IEnumerable<IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>>

                      InVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                               TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                               THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(
            
                               this IEnumerable<IPropertyEdge<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                              TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                              THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>> myIEnumerable)

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

            var _Pipe = new EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(

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
        public static IEnumerable<IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>>

                      OutVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(

                                this IEnumerable<IPropertyEdge<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                               TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                               THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>> myIEnumerable)
        
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

            var _Pipe = new EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(Steps.EdgeVertexStep.OUT_VERTEX);

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
        public static IEnumerable<IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>>

                      BothVertices<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                   TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                   THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(

                                   this IEnumerable<IPropertyEdge<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>> myIEnumerable)
        
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

            var _Pipe = new EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(Steps.EdgeVertexStep.BOTH_VERTICES);

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
        public static IEnumerable<IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>>

                      EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                     TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                     THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(

                                     this IEnumerator<IPropertyEdge<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                    TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                    THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>> myIEnumerator,
                                     Steps.EdgeVertexStep myStep)
        
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

            var _Pipe = new EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(myStep);

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
        public static IEnumerable<IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>>

                      InVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                               TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                               THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(

                               this IEnumerator<IPropertyEdge<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                              TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                              THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>> myIEnumerator)
        
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

            var _Pipe = new EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);

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
        public static IEnumerable<IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>>

                      OutVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(

                                this IEnumerator<IPropertyEdge<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                               TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                               THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>> myIEnumerator)
        
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

            var _Pipe = new EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(Steps.EdgeVertexStep.OUT_VERTEX);

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
        public static IEnumerable<IPropertyVertex<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>>

                      BothVertices<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                   TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                   THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(

                                   this IEnumerator<IPropertyEdge<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                                                  TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                                                  THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>> myIEnumerator)
        
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

            var _Pipe = new EdgeVertexPipe<TVertexId,    TVertexRevisionId,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                                           TEdgeId,      TEdgeRevisionId,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                                           THyperEdgeId, THyperEdgeRevisionId, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(Steps.EdgeVertexStep.BOTH_VERTICES);

            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

    }

}
