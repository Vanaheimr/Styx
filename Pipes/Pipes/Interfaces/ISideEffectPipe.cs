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

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// A SideEffectPipe will produce a side effect which can be retrieved by the SideEffect property.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    /// <typeparam name="T">The type of the sideeffect.</typeparam>
    public interface ISideEffectPipe<in S, out E, out T> : IPipe<S, E>
    {

        /// <summary>
        /// The sideeffect produced by this pipe.
        /// </summary>
        T SideEffect { get; }

    }

}
