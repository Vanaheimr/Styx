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
using System.Collections;
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    #region IEndPipe

    /// <summary>
    /// A helper interface for the IPipe pipe interface
    /// defining a general pipe emitting elements.
    /// </summary>
    public interface IEndPipe : IEnumerator, IEnumerable
    {

        /// <summary>
        /// Returns the path traversed to arrive at the current result of the pipe.
        /// </summary> 
        /// <returns>A List of all of the objects traversed for the current iterator position of the pipe.</returns>
        List<Object> Path { get; }

    }

    #endregion

    #region IEndPipe<out E>

    /// <summary>
    /// A helper interface for the IPipe&lt;S, E&gt; pipe interface
    /// defining a general pipe emitting elements of type E.
    /// </summary>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public interface IEndPipe<out E> : IEndPipe, IEnumerator<E>, IEnumerable<E>
    { }

    #endregion

}
