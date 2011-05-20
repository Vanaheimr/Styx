﻿/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
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
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// GatherPipe 
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    public class GatherPipe<S> : SideEffectCapPipe<S, IEnumerable<S>>
    {

        #region Constructor(s)

        #region GatherPipe()

        /// <summary>
        /// Creates a new GatherPipe.
        /// </summary>
        public GatherPipe()
            : base(((ISideEffectPipe<S, Object, IEnumerable<S>>)((ISideEffectPipe<S, S, IEnumerable<S>>) new AggregatorPipe<S>(new List<S>()))))
        { }

        #endregion

        #endregion

    }

}
