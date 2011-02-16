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
    /// Pipes extensions.
    /// </summary>
    public static class PropertyPipeExtensions
    {

        #region PropertyPipe<S, E>(this myIEnumerable, myKeys)

        /// <summary>
        /// The PropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="S">The type of the consuming objects.</typeparam>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerable">A collection of consumable objects.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> PropertyPipe<S, E>(this IEnumerable<S> myIEnumerable, String[] myKeys)
            where S : IElement
        {

            var _Pipe = new PropertyPipe<S, E>(myKeys);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        #endregion

        #region PropertyPipe<S, E>(this myIEnumerator, myKeys)

        /// <summary>
        /// The PropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="S">The type of the consuming objects.</typeparam>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerator">A enumerator of consumable objects.</param>
        /// <param name="myKeys">The property keys.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> PropertyPipe<S, E>(this IEnumerator<S> myIEnumerator, String[] myKeys)
            where S : IElement
        {

            var _Pipe = new PropertyPipe<S, E>(myKeys);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

        #endregion

    }

}
