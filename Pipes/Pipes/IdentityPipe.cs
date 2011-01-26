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

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The IdentityPipe is the most basic pipe.
    /// It simply maps the input to the output without any processing.
    /// This Pipe is useful in various test case situations.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the pipe.</typeparam>
    public class IdentityPipe<S> : AbstractPipe<S, S>
    {

        protected override S processNextStart()
        {
            starts.MoveNext();
            return Current;
        }

    }

}
