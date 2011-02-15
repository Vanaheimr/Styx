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
    /// The VertexPropertyPipe returns the property value of the
    /// Element identified by the provided key.
    /// </summary>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class VertexPropertyPipe<E> : PropertyPipe<IVertex, E>
    {

        #region Constructor(s)

        #region VertexPropertyPipe(myKeys)

        /// <summary>
        /// Creates a new VertexPropertyPipe.
        /// </summary>
        /// <param name="myKeys">The property keys.</param>
        public VertexPropertyPipe(params String[] myKeys)
            : base(myKeys)
        { }

        #endregion

        #endregion

    }

    
    #region Extensions

    /// <summary>
    /// Pipes extensions.
    /// </summary>
    public static partial class Extensions
    {

        /// <summary>
        /// The VertexPropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <param name="myIEnumerable">A collection of consumable objects implementing IVertex.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<Object> VertexPropertyPipe(this IEnumerable<IVertex> myIEnumerable, params String[] myKeys)
        {

            var _Pipe = new VertexPropertyPipe<Object>(myKeys);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        /// <summary>
        /// The VertexPropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerable">A collection of consumable objects implementing IVertex.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> VertexPropertyPipe<E>(this IEnumerable<IVertex> myIEnumerable, params String[] myKeys)
        {

            var _Pipe = new VertexPropertyPipe<E>(myKeys);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }


        /// <summary>
        /// The VertexPropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <param name="myIEnumerator">A enumerator of consumable objects implementing IVertex.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<Object> VertexPropertyPipe(this IEnumerator<IVertex> myIEnumerator, params String[] myKeys)
        {

            var _Pipe = new VertexPropertyPipe<Object>(myKeys);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        /// <summary>
        /// The VertexPropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerator">A enumerator of consumable objects implementing IVertex.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> VertexPropertyPipe<E>(this IEnumerator<IVertex> myIEnumerator, params String[] myKeys)
        {

            var _Pipe = new VertexPropertyPipe<E>(myKeys);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

    }

    #endregion

}
