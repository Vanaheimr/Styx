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


        #region SetStartIEnumerator(myIEnumerator)

        public override void SetIEnumerator(IEnumerator<S> myIEnumerator)
        {
            _PipeToCap.SetIEnumerator(myIEnumerator);
        }

        #endregion

        #region SetStartIEnumerable(myIEnumerable)

        public override void SetIEnumerable(IEnumerable<S> myIEnumerable)
        {
            _PipeToCap.SetIEnumerator(myIEnumerable.GetEnumerator());
        }

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
            
            if (_Alive)
            {

                // Consume the entire pipe!
                while (_PipeToCap.MoveNext())
                { }

                _Alive = false;
                
                _CurrentElement = _PipeToCap.SideEffect;
                return true;

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
                _List.Add(this._CurrentElement);

                return _List;

            }
        }


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "[" + _PipeToCap + "]";
        }

        #endregion

    }

}
