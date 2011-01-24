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

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// A FilterPipe has no specified behavior save that it takes the same objects it emits.
    /// This interface is used to specify that a Pipe will either emit its input or not.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public interface IFilterPipe<S> : IPipe<S,S>
        where S : IEquatable<S>
    { }

}
