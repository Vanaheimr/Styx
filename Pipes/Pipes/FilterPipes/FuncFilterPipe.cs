/*
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
    /// Filters the consuming objects by calling a Func&lt;S, E&gt;.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    public class FuncFilterPipe<S> : AbstractPipe<S, S>
    {

        #region Data

        private Func<S, Boolean> _FilterFunc;

        #endregion

        #region Constructor(s)

        #region FuncFilterPipe(myFilterFunc)

        /// <summary>
        /// Creates a new FuncFilterPipe using the given Func&lt;S, E&gt;.
        /// </summary>
        /// <param name="myFilterFunc">A Func&lt;S, E&gt; filtering the consuming objects. True means filter (ignore).</param>
        public FuncFilterPipe(Func<S, Boolean> myFilterFunc)
        {

            if (myFilterFunc == null)
                throw new ArgumentNullException("myFilterFunc must not be null!");

            _FilterFunc = myFilterFunc;

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

            while (_InternalEnumerator.MoveNext())
            {

                if (!_FilterFunc(_InternalEnumerator.Current))
                {
                    _CurrentElement = _InternalEnumerator.Current;
                    return true;
                }

            }

            return false;

        }

        #endregion

    }

}
