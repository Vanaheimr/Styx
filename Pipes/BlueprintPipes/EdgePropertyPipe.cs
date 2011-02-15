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
    /// The EdgePropertyPipe returns the property value of the
    /// Element identified by the provided key.
    /// </summary>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class EdgePropertyPipe<E> : PropertyPipe<IEdge, E>
    {

        #region Constructor(s)

        #region EdgePropertyPipe(myKey)

        /// <summary>
        /// Creates a new EdgePropertyPipe.
        /// </summary>
        /// <param name="myKey">The property key.</param>
        public EdgePropertyPipe(String myKey)
            : base(myKey)
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
        /// The EdgePropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <param name="myIEnumerable">A collection of consumable objects implementing IEdge.</param>
        /// <param name="myKey">The property key.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<Object> EdgePropertyPipe(this IEnumerable<IEdge> myIEnumerable, String myKey)
        {

            var _Pipe = new EdgePropertyPipe<Object>(myKey);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        /// <summary>
        /// The EdgePropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerable">A collection of consumable objects implementing IEdge.</param>
        /// <param name="myKey">The property key.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> EdgePropertyPipe<E>(this IEnumerable<IEdge> myIEnumerable, String myKey)
        {

            var _Pipe = new EdgePropertyPipe<E>(myKey);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }


        /// <summary>
        /// The EdgePropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerator">An enumerator of consumable objects implementing IEdge.</param>
        /// <param name="myKey">The property key.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> EdgePropertyPipe<E>(this IEnumerator<IEdge> myIEnumerator, String myKey)
        {

            var _Pipe = new EdgePropertyPipe<E>(myKey);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

    }

    #endregion

}
