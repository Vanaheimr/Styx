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

    #region IMetaPipe

    /// <summary>
    /// A MetaPipe is a pipe that "wraps" some collection of pipes.
    /// </summary>
    public interface IMetaPipe : IDisposable
    { }

    #endregion

    #region IMetaPipe<in S, out E>

    /// <summary>
    /// A MetaPipe is a pipe that "wraps" some collection of pipes.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public interface IMetaPipe<S, E> : IPipe<S, E>, IMetaPipe
    {

        /// <summary>
        /// A list of all wrapped pipes
        /// </summary>
        IEnumerable<IPipe> Pipes { get; }

    }

    #endregion

}
