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
    /// The SideEffectCapPipe will yield an E that is the side effect of
    /// the provided SideEffectPipe. This is useful for when the side
    /// effect of a Pipe is desired in a computational stream.
    /// </summary>
    public class SideEffectCapPipe<S, T> : AbstractPipe<S, T>
    {

        #region Data

        private readonly ISideEffectPipe<S, Object, T> _PipeToCap;
        private          Boolean                       _Alive;

        #endregion

        #region Constructor(s)

        #region SideEffectCapPipe(myPipeToCap)

        public SideEffectCapPipe(ISideEffectPipe<S, Object, T> myPipeToCap)
        {
            _PipeToCap = myPipeToCap;
            _Alive     = true;
        }

        #endregion

        #endregion


        public void setStarts(IEnumerator<S> starts)
        {
            _PipeToCap.SetStarts(starts);
        }

        #region ProcessNextStart()

        protected override T ProcessNextStart()
        {
            
            if (_Alive)
            {

                // Consume the entire pipe!
                while (_PipeToCap.MoveNext())
                { }

                _Alive = false;
                
                return _PipeToCap.SideEffect;

            }
            
            else
                throw new NoSuchElementException();

        }

        #endregion


        public new List<Object> Path
        {
            get
            {

                var _List = _PipeToCap.Path;
                _List.Add(this._CurrentEnd);

                return _List;

            }
        }


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "[" + _PipeToCap + "]";
        }

        #endregion

    }

}
