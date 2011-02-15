﻿/*
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
    /// The PropertyPipe returns the property value of the
    /// Element identified by the provided key.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class PropertyPipe<S, E> : AbstractPipe<S, E>
        where S : IElement
    {

        #region Data

        private readonly String _Key;

        #endregion

        #region Constructor(s)

        #region PropertyPipe(myKey)

        /// <summary>
        /// Creates a new PropertyPipe.
        /// </summary>
        /// <param name="myKey">The property key.</param>
        public PropertyPipe(String myKey)
        {
            _Key   = myKey;
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

            if (_InternalEnumerator.MoveNext())
            {
                _CurrentElement = _InternalEnumerator.Current.GetProperty<E>(_Key);
                return true;
            }

            else
                return false;

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + _Key + ">";
        }

        #endregion

    }

    
    #region Extensions

    /// <summary>
    /// Pipes extensions.
    /// </summary>
    public static partial class Extensions
    {

        /// <summary>
        /// The PropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="S">The type of the consuming objects.</typeparam>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerable">A collection of consumable objects.</param>
        /// <param name="myKey">The property key.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> PropertyPipe<S, E>(this IEnumerable<S> myIEnumerable, String myKey)
            where S : IElement
        {

            var _Pipe = new PropertyPipe<S, E>(myKey);
            _Pipe.SetSourceCollection(myIEnumerable);

            return _Pipe;

        }

        /// <summary>
        /// The PropertyPipe returns the property value of the
        /// Element identified by the provided key.
        /// </summary>
        /// <typeparam name="S">The type of the consuming objects.</typeparam>
        /// <typeparam name="E">The type of the emitting objects.</typeparam>
        /// <param name="myIEnumerator">An enumerator of consumable objects.</param>
        /// <param name="myKey">The property key.</param>
        /// <returns>A collection of emittable objects.</returns>
        public static IEnumerable<E> PropertyPipe<S, E>(this IEnumerator<S> myIEnumerator, String myKey)
            where S : IElement
        {

            var _Pipe = new PropertyPipe<S, E>(myKey);
            _Pipe.SetSource(myIEnumerator);

            return _Pipe;

        }

    }

    #endregion

}
