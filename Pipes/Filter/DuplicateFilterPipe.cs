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
    /// The DuplicateFilterPipe will not allow a duplicate object to pass through it.
    /// This is accomplished by the Pipe maintaining an internal HashSet that is used
    /// to store a history of previously seen objects.
    /// Thus, the more unique objects that pass through this Pipe, the slower it
    /// becomes as a log_2 index is checked for every object.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class DuplicateFilterPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>
    {

        #region Data

        private readonly HashSet<S> _HistorySet = new HashSet<S>();

        #endregion

        #region Constructor(s)

        #region DuplicateFilterPipe()

        public DuplicateFilterPipe()
        { }

        #endregion

        #endregion


        #region MoveNext()

        public override Boolean MoveNext()
        {

            while (true)
            {

                _Starts.MoveNext();
                _CurrentItem = _Starts.Current;

                if (!_HistorySet.Contains(_CurrentItem))
                {
                    _HistorySet.Add(_CurrentItem);
                    return true;
                }
            
            }

        }

        #endregion

    }

}
