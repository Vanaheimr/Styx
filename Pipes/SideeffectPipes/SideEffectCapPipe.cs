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

        /// <summary>
        /// Creates a new SideEffectCapPipe.
        /// </summary>
        /// <param name="myPipeToCap">A ISideEffectCapPipe.</param>
        public SideEffectCapPipe(ISideEffectPipe<S, Object, T> myPipeToCap)
        {
            _PipeToCap = myPipeToCap;
            _Alive     = true;
        }

        #endregion

        #endregion


        #region SetSource(myIEnumerator)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S&gt; as input.
        /// </summary>
        /// <param name="myIEnumerator">An IEnumerator&lt;S&gt; as element source.</param>
        public override void SetSource(IEnumerator<S> myIEnumerator)
        {
            _PipeToCap.SetSource(myIEnumerator);
        }

        #endregion

        #region SetSourceCollection(myIEnumerable)

        /// <summary>
        /// Set the elements emitted by the given IEnumerator&lt;S&gt; as input.
        /// </summary>
        /// <param name="myIEnumerable">An IEnumerable&lt;S&gt; as element source.</param>
        public override void SetSourceCollection(IEnumerable<S> myIEnumerable)
        {
            _PipeToCap.SetSource(myIEnumerable.GetEnumerator());
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
                return false;

        }

        #endregion


        #region Path

        /// <summary>
        /// Returns the transformation path to arrive at the current object
        /// of the pipe. This is a list of all of the objects traversed for
        /// the current iterator position of the pipe.
        /// </summary>
        public new List<Object> Path
        {
            get
            {

                var _List = _PipeToCap.Path;
                _List.Add(this._CurrentElement);

                return _List;

            }
        }

        #endregion


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
