/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

namespace org.GraphDefined.Vanaheimr.Styx
{

    /// <summary>
    /// A HistoryEnumerator wraps and behaves like a classical IEnumerator.
    /// However, it will remember what was last returned out of the IEnumerator.
    /// </summary>
    /// <typeparam name="T">The type of the stored elements.</typeparam>
    public class HistoryEnumerator<T> : IHistoryEnumerator, IEnumerator<T>
    {

        #region Data

        private readonly IEnumerator<T>  _InternalEnumerator;
        private          T               _Last;
        private          Boolean         _FirstMove, _Finished;

        #endregion

        #region Constructor(s)

        #region HistoryEnumerator(IEnumerator)

        /// <summary>
        /// Creates a new HistoryEnumerator based on the given enumerator.
        /// </summary>
        /// <param name="IEnumerator">The enumerator to be wrapped.</param>
        public HistoryEnumerator(IEnumerator<T> IEnumerator)
        {
            _InternalEnumerator  = IEnumerator;
            _Last                = default(T);
            _FirstMove           = true;
            _Finished            = false;
        }

        #endregion

        #region HistoryEnumerator(IEnumerable)

        /// <summary>
        /// Creates a new HistoryEnumerator based on the given enumerable.
        /// </summary>
        /// <param name="IEnumerable">The enumerable to be wrapped.</param>
        public HistoryEnumerator(IEnumerable<T> IEnumerable)
        {
            _InternalEnumerator  = IEnumerable.GetEnumerator();
            _Last                = default(T);
            _FirstMove           = true;
            _Finished            = false;
        }

        #endregion

        #endregion


        #region Current

        /// <summary>
        /// Return the current element of the internal IEnumertor.
        /// </summary>
        public T Current
        {
            get
            {
                return _InternalEnumerator.Current;
            }
        }

        /// <summary>
        /// Return the current element of the internal IEnumertor.
        /// </summary>
        Object System.Collections.IEnumerator.Current
        {    
            get
            {
                return _InternalEnumerator.Current;
            }
        }

        #endregion

        #region Last

        /// <summary>
        /// Return the last element of the internal IEnumertor&lt;T&gt;.
        /// </summary>
        public T Last
        {
            get
            {
                return _Last;
            }
        }

        /// <summary>
        /// Return the last element of the internal IEnumertor.
        /// </summary>
        Object IHistoryEnumerator.Last
        {
            get
            {
                return _Last;
            }
        }

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public Boolean MoveNext()
        {

            //if (_Finished)
            //    return false;

            if (_FirstMove)
            {

                if (_InternalEnumerator.MoveNext())
                {
                    _Last       = _InternalEnumerator.Current;
                    _FirstMove  = false;
                    return true;
                }

                return false;

            }

            var result = _InternalEnumerator.MoveNext();

            if (!result)
                _Finished = true;

            return result;

        }

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            //_InternalEnumerator.Reset();
            _Last   = default(T);
        }

        #endregion


        #region Dispose()

        /// <summary>
        /// Dispose this enumerator.
        /// </summary>
        public void Dispose()
        {
            _InternalEnumerator.Dispose();
        }

        #endregion

    }

}
