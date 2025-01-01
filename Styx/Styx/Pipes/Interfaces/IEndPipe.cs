/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    #region IEndPipe

    /// <summary>
    /// An interface for the element emitting part of a pipe.
    /// Pipes implementing just this interface do not neccessarily
    /// consume elements, but e.g. might receive them via network.
    /// </summary>
    public interface IEndPipe : IDisposable
    {

        /// <summary>
        /// Returns the path traversed to arrive at the current result of the pipe.
        /// </summary> 
        /// <returns>A List of all of the objects traversed for the current iterator position of the pipe.</returns>
        IEnumerable<Object> Path { get; }

    }

    #endregion

    #region IEndPipe<out E>

    /// <summary>
    /// An interface for the element emitting part of a pipe.
    /// Pipes implementing just this interface do not neccessarily
    /// consume elements, but e.g. might receive them via network.
    /// </summary>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public interface IEndPipe<E> : IEndPipe
    {

        /// <summary>
        /// Return an enumerator to traverse this pipe.
        /// </summary>
        IEnumerator<E> GetEnumerator();

        /// <summary>
        /// Return the current element in the pipe.
        /// </summary>
        E Current { get; }

        /// <summary>
        /// Advances the enumerator to the next element of the pipe.
        /// </summary>
        /// <returns></returns>
        Boolean MoveNext();

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the pipe. If the pipe has
        /// no internal state the pipe will just call Reset() on
        /// its source pipe.
        /// </summary>
        IEndPipe<E> Reset();

    }

    #endregion

}
