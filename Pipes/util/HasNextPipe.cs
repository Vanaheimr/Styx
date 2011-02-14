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
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// HasNextPipe 
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    public class HasNextPipe<S> : AbstractPipe<S, Boolean>
    {

        #region Data

        private readonly IPipe _InternalPipe;

        #endregion

        #region Constructor(s)

        #region HasNextPipe()

        /// <summary>
        /// Creates a new HasNextPipe.
        /// </summary>
        public HasNextPipe(IPipe myIPipe)
        {
            _InternalPipe = myIPipe;
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

                _InternalPipe.SetSource(new SingleEnumerator<S>(_InternalEnumerator.Current));

                if (_InternalPipe.MoveNext())
                {
                    _CurrentElement = true;
                    return true;
                }

                return false;

            }

            return false;

        }

        #endregion

    }

}
