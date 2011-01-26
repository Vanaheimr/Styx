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
    /// The GroupCountPipe will simply emit the incoming object, but generate a map side effect.
    /// The map's keys are the objects that come into the pipe.
    /// The map's values are the number of times that the key object has come into the pipe.
    /// </summary>
    public class GroupCountPipe<S> : AbstractPipe<S, S>, ISideEffectPipe<S, S, IDictionary<S, UInt64>>
    {

        #region Data

        private IDictionary<S, UInt64> _CountMap;

        #endregion

        #region Constructor(s)

        #region GroupCountPipe()

        public GroupCountPipe()
        {
            _CountMap = new Dictionary<S, UInt64>();
        }

        #endregion

        #region GroupCountPipe(myIDictionary)

        public GroupCountPipe(IDictionary<S, UInt64> myIDictionary)
        {
            _CountMap = myIDictionary;
        }

        #endregion

        #endregion


        #region ProcessNextStart()

        protected override S ProcessNextStart()
        {

            _Starts.MoveNext();
            var _S = _Starts.Current;

            UpdateMap(_S);

            return _S;

        }

        #endregion

        public IDictionary<S, UInt64> SideEffect
        {
            get
            {
                return _CountMap;
            }
        }


        private void UpdateMap(S myS)
        {

            UInt64 _Counter;

            if (_CountMap.TryGetValue(myS, out _Counter))
                _CountMap[myS] = _Counter++;

            else
                _CountMap.Add(myS, 1);

        }


    }

}
