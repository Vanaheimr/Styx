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
    /// The AndFilterPipe takes a collection of pipes, where E is boolean.
    /// Each provided pipe is fed the same incoming S object. If all the
    /// pipes emit true, then the AndFilterPipe emits the incoming S object.
    /// If not, then the incoming S object is not emitted.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class AndFilterPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>
    {

        #region Data

        private readonly List<IPipe<S, Boolean>> _Pipes;

        #endregion

        #region Constructor(s)

        #region AndFilterPipe(myPipes)

        public AndFilterPipe(IPipe<S, Boolean>[] myPipes)
        {
            _Pipes = new List<IPipe<S, Boolean>>(myPipes);
        }

        #endregion

        #region AndFilterPipe(myPipes)

        public AndFilterPipe(List<IPipe<S, Boolean>> myPipes)
        {
            _Pipes = myPipes;
        }

        #endregion

        #endregion


        protected override S processNextStart()
        {

            while (true)
            {

                starts.MoveNext();
                var _S = starts.Current;

                var _And = true;
                
                foreach (var _Pipe in _Pipes)
                {

                    _Pipe.SetStarts(new SingleEnumerator<S>(_S));

                    if (!_Pipe.MoveNext())
                    {
                        _And = false;
                        break;
                    }

                }
                
                if (_And)
                    return _S;
            
            }

        }

    }

}
