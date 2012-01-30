/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/Pipes.NET>
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

    #region ISplitPipe

    /// <summary>
    /// A helper interface for the ISplitPipe&lt;S, E&gt; pipe interface
    /// defining a general pipe.
    /// </summary>
    public interface ISplitPipe : IDisposable
    { }

    #endregion

    #region ISplitPipe<in S, out E1, out E2>

    /// <summary>
    /// The generic interface for any Pipe implementation.
    /// A Pipe takes/consumes objects of type S and returns/emits objects of type E1 and E2.
    /// S refers to <i>starts</i> and the E1 and E2 refers to <i>ends</i>.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E1">The type of the first emitting objects.</typeparam>
    /// <typeparam name="E2">The type of the second emitting objects.</typeparam>
    public interface ISplitPipe<in S, out E1, out E2> : ISplitPipe
	{ }

    #endregion

}
