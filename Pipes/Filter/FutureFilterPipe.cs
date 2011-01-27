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
    /// FutureFilterPipe will allow an object to pass through it if the
    /// object has an output from the pipe provided in the constructor
    /// of the FutureFilterPipe.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class FutureFilterPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>
    {

        #region Data

        private readonly IPipe<S, S> _Pipe;

        #endregion

        #region Constructor(s)

        #region FutureFilterPipe(myPipes)

        public FutureFilterPipe(IPipe<S, S> myPipe)
        {
            _Pipe = myPipe;
        }

        #endregion

        #endregion


        #region MoveNext()

        public override Boolean MoveNext()
        {

            while (true)
            {

                _Starts.MoveNext();
                var _S = _Starts.Current;

                _Pipe.SetStarts(new SingleEnumerator<S>(_S));

                // District of chaos, discord and confusion ;)!
                //if (_Pipe.hasNext())
                //{

                //    while (_Pipe.hasNext())
                //        _Pipe.next();

                //    return _S;

                //}
            
            }

        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "<" + _Pipe + ">";
        }

        #endregion

    }

}
