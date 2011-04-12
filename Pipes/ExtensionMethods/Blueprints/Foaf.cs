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
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      Foaf<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,    TDatastructureVertex,
                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,      TDatastructureEdge,
                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge, TDatastructureHyperEdge>(

                           this IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerable)

            where TDatastructureVertex    : IDictionary<TKeyVertex,    TValueVertex>
            where TDatastructureEdge      : IDictionary<TKeyEdge,      TValueEdge>
            where TDatastructureHyperEdge : IDictionary<TKeyHyperEdge, TValueHyperEdge>

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

            var _Pipe1 = new VertexEdgePipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.VertexEdgeStep.OUT_EDGES);

            _Pipe1.SetSourceCollection(myIEnumerable);

            var _Pipe2 = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);

            _Pipe2.SetSource(_Pipe1);

            var _Pipe3 = new VertexEdgePipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.VertexEdgeStep.OUT_EDGES);

            _Pipe3.SetSource(_Pipe2);

            var _Pipe4 = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);

            _Pipe4.SetSource(_Pipe3);

            return _Pipe4.Distinct().Except(myIEnumerable);

        }

        #endregion

        #region Foaf(this myIEnumerable, myLabel)

        /// <summary>
        /// A specialized pipeline returning the 1-hop neighborhood
        /// (friend-of-a-friend) filtered by the connecting edge label.
        /// </summary>
        /// <param name="myIEnumerable">A collection of objects implementing IPropertyVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      Foaf<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                           this IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerable,

                           String myLabel)

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

            var _Pipe1 = new VertexEdgePipe <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.VertexEdgeStep.OUT_EDGES);
            _Pipe1.SetSourceCollection(myIEnumerable);

            var _Pipe2 = new LabelFilterPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe2.SetSource(_Pipe1);

            var _Pipe3 = new EdgeVertexPipe <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);
            _Pipe3.SetSource(_Pipe2);

            var _Pipe4 = new VertexEdgePipe <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.VertexEdgeStep.OUT_EDGES);
            _Pipe4.SetSource(_Pipe3);

            var _Pipe5 = new LabelFilterPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe5.SetSource(_Pipe4);

            var _Pipe6 = new EdgeVertexPipe <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);
            _Pipe6.SetSource(_Pipe5);

            return _Pipe6.Distinct().Except(myIEnumerable);

        }

        #endregion


        #region Foaf(this myIEnumerator)

        /// <summary>
        /// A specialized pipeline returning the 1-hop neighborhood
        /// (friend-of-a-friend).
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      Foaf<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                           this IEnumerator<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
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

            var _Pipe1 = new VertexEdgePipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.VertexEdgeStep.OUT_EDGES);

            _Pipe1.SetSource(myIEnumerator);

            var _Pipe2 = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);

            _Pipe2.SetSource(_Pipe1);

            var _Pipe3 = new VertexEdgePipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.VertexEdgeStep.OUT_EDGES);

            _Pipe3.SetSource(_Pipe2);

            var _Pipe4 = new EdgeVertexPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);

            _Pipe4.SetSource(_Pipe3);

            return _Pipe4.Distinct();

        }

        #endregion

        #region Foaf(this myIEnumerator, myLabel)

        /// <summary>
        /// A specialized pipeline returning the 1-hop neighborhood
        /// (friend-of-a-friend) filtered by the connecting edge label.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of objects implementing IPropertyVertex.</param>
        /// <param name="myLabel">The edge label.</param>
        /// <returns>A collection of objects implementing IPropertyVertex.</returns>
        public static IEnumerable<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                  TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                  TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>>

                      Foaf<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                           TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                           TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(

                           this IEnumerator<IPropertyVertex<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                                            TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                                            TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>> myIEnumerator,

                           String myLabel)

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

            var _Pipe1 = new VertexEdgePipe <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.VertexEdgeStep.OUT_EDGES);
            _Pipe1.SetSource(myIEnumerator);

            var _Pipe2 = new LabelFilterPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe2.SetSource(_Pipe1);

            var _Pipe3 = new EdgeVertexPipe <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);
            _Pipe3.SetSource(_Pipe2);

            var _Pipe4 = new VertexEdgePipe <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.VertexEdgeStep.OUT_EDGES);
            _Pipe4.SetSource(_Pipe3);

            var _Pipe5 = new LabelFilterPipe<TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(myLabel, ComparisonFilter.NOT_EQUAL);
            _Pipe5.SetSource(_Pipe4);

            var _Pipe6 = new EdgeVertexPipe <TIdVertex,    TRevisionIdVertex,    TKeyVertex,    TValueVertex,
                                             TIdEdge,      TRevisionIdEdge,      TKeyEdge,      TValueEdge,
                                             TIdHyperEdge, TRevisionIdHyperEdge, TKeyHyperEdge, TValueHyperEdge>(Steps.EdgeVertexStep.IN_VERTEX);
            _Pipe6.SetSource(_Pipe5);

            return _Pipe6.Distinct();

        }

        #endregion

    }

}
