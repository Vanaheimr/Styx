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
    /// The CountPipe produces a side effect that is the total
    /// number of objects that have passed through it.
    /// </summary>
    public class CountPipe<S> : AbstractPipe<S, S>, ISideEffectPipe<S, S, UInt64>
    {

        #region Data

        private UInt64 _Counter;

        #endregion

        #region Constructor(s)

        #region CountPipe()

        public CountPipe()
        {
            _Counter = 0UL;
        }

        #endregion

        #endregion


        #region MoveNext()

        public override Boolean MoveNext()
        {

            _Starts.MoveNext();
            _CurrentItem = _Starts.Current;
            _Counter++;

            return true;

        }

        #endregion

        #region SideEffect

        public UInt64 SideEffect
        {
            get
            {
                return _Counter;
            }
        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "<" + _Counter + ">";
        }

        #endregion

    }

}
