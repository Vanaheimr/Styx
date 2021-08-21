/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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
using System.Collections;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    /// <summary>
    /// Some helper methods to create endpipes.
    /// </summary>
    public static class EndPipe
    {

        #region CreatePipe<E>(Element)

        /// <summary>
        /// Create a pipe emitting the given element.
        /// </summary>
        /// <typeparam name="E">The type of the emitted element.</typeparam>
        /// <param name="Element">The single element within the pipe.</param>
        /// <returns>A pipe emitting the given element.</returns>
        public static EndPipe<E> CreatePipe<E>(E Element)
        {
            return new EndPipe<E>(new E[] { Element });
        }

        #endregion

        #region CreatePipe<E>(this Enumeration)

        /// <summary>
        /// Create a pipe emitting all elements of the given enumeration.
        /// </summary>
        /// <typeparam name="E">The type of the emitted elements.</typeparam>
        /// <param name="Enumeration">An enumeration of elements.</param>
        /// <returns>A pipe emitting the given enumeration of elements.</returns>
        public static EndPipe<E> CreatePipe<E>(this IEnumerable<E> Enumeration)
        {
            return new EndPipe<E>(Enumeration.GetEnumerator());
        }

        #endregion

        #region CreatePipe<E>(this Enumerator)

        /// <summary>
        /// Create a pipe emitting all elements emitted by the given enumerator.
        /// </summary>
        /// <typeparam name="E">The type of the emitted elements.</typeparam>
        /// <param name="Enumerator">A enumerator emitting elements.</param>
        /// <returns>A pipe emitting the elements emitted by the given enumerator.</returns>

        public static EndPipe<E> CreatePipe<E>(this IEnumerator<E> Enumerator)
        {
            return new EndPipe<E>(Enumerator);
        }

        #endregion

    }


    #region EndPipe<E>

    /// <summary>
    /// A pipe emitting elements.
    /// </summary>
    /// <typeparam name="E">The type of the emitted elements.</typeparam>
    public class EndPipe<E> : IEndPipe<E>
    {

        #region Data

        private readonly IEnumerator<E> Enumerator;
        private          E              _Current;

        #endregion

        #region Constructor(s)

        #region EndPipe(Enumeration)

        /// <summary>
        /// Create a pipe emitting all elements of the given enumeration.
        /// </summary>
        /// <param name="Enumeration">An enumeration of elements.</param>
        public EndPipe(IEnumerable<E> Enumeration)
        {
            this.Enumerator = Enumeration.GetEnumerator();
        }

        #endregion

        #region EndPipe(Enumerator)

        /// <summary>
        /// Create a pipe emitting all elements emitted by the given enumerator.
        /// </summary>
        /// <param name="Enumerator">A enumerator emitting elements.</param>
        public EndPipe(IEnumerator<E> Enumerator)
        {
            this.Enumerator = Enumerator;
        }

        #endregion

        #endregion


        #region EndPipe(Array)

        public EndPipe(E[] Array)
        {
            this.Enumerator = Array.ToList().GetEnumerator();
        }

        #endregion

        public IEnumerator<E> GetEnumerator()
        {
            return Enumerator;
        }

        public E Current
        {
            get
            {
                return _Current;
            }
        }

        public Boolean MoveNext()
        {

            if (Enumerator.MoveNext())
            {
                _Current = Enumerator.Current;
                return true;
            }

            return false;

        }

        public IEndPipe<E> Reset()
        {
            Enumerator.Reset();
            return this;
        }

        public IEnumerable<Object> Path
        {
            get
            {
                return new Object[1] { _Current };
            }
        }

        public void Dispose()
        {
            Enumerator.Dispose();
        }

    }

    #endregion

}