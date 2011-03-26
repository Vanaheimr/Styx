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
    /// VertexPropertyPipe extensions.
    /// </summary>
    public static class VertexPropertyPipeExtensions
    {

        #region GetProperty<TKey>(this myIEnumerable, params myKeys)

        /// <summary>
        /// The VertexPropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property keys.</typeparam>
        /// <param name="myIEnumerable">A collection of consumable objects implementing IVertex.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<Object> GetProperty<TId, TKey>(this IEnumerable<IVertex<TId, TKey>> myIEnumerable, params TKey[] myKeys)
            where TId : IEquatable<TId>, IComparable<TId>, IComparable
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            var _Pipe = new VertexPropertyPipe<TId, TKey, Object>(myKeys);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region GetProperty<TKey, E>(this myIEnumerable, params myKeys)

        /// <summary>
        /// The VertexPropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property keys.</typeparam>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerable">A collection of consumable objects implementing IVertex.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> GetProperty<TId, TKey, E>(this IEnumerable<IVertex<TId, TKey>> myIEnumerable, params TKey[] myKeys)
            where TId : IEquatable<TId>, IComparable<TId>, IComparable
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            var _Pipe = new VertexPropertyPipe<TId, TKey, E>(myKeys);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion


        #region GetProperty<TKey>(this myIEnumerator, params myKeys)

        /// <summary>
        /// The VertexPropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property keys.</typeparam>
        /// <param name="myIEnumerator">A enumerator of consumable objects implementing IVertex.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<Object> GetProperty<TId, TKey>(this IEnumerator<IVertex<TId, TKey>> myIEnumerator, params TKey[] myKeys)
            where TId : IEquatable<TId>, IComparable<TId>, IComparable
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            var _Pipe = new VertexPropertyPipe<TId, TKey, Object>(myKeys);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

        #region GetProperty<TKey, E>(this myIEnumerator, params myKeys)

        /// <summary>
        /// The VertexPropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property keys.</typeparam>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerator">A enumerator of consumable objects implementing IVertex.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> GetProperty<TId, TKey, E>(this IEnumerator<IVertex<TId, TKey>> myIEnumerator, params TKey[] myKeys)
            where TId : IEquatable<TId>, IComparable<TId>, IComparable
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            var _Pipe = new VertexPropertyPipe<TId, TKey, E>(myKeys);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

    }

}
