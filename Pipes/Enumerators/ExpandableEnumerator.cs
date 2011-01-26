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
    /// ExpandableEnumerator takes an IEnumerator and will emit its elements.
    /// However, if an object is added to ExpandableEnumerator, then its put into
    /// an internal queue. The queue has priority over the internal enumerator when
    /// accessing the current element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class ExpandableEnumerator<T> : IEnumerator<T>
    {

        #region Data

        private readonly Queue<T>       _Queue;
        private readonly IEnumerator<T> _IEnumerator;

        #endregion

        #region Constructor(s)

        #region ExpandableIterator(myIEnumerator)

        /// <summary>
        /// Creates a new ExpandableEnumerator based on the given myIEnumerator.
        /// </summary>
        /// <param name="myIEnumerator">The enumerator to be wrapped.</param>
        public ExpandableEnumerator(IEnumerator<T> myIEnumerator)
        {
            _Queue       = new Queue<T>();
            _IEnumerator = myIEnumerator;
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
                if (_Queue.Count == 0)
                    return _IEnumerator.Current;

                else
                    return _Queue.Dequeue();
            }
        }

        /// <summary>
        /// Return the current element of the internal IEnumertor.
        /// </summary>
        Object System.Collections.IEnumerator.Current
        {
            get
            {
                if (_Queue.Count == 0)
                    return _IEnumerator.Current;

                else
                    return _Queue.Dequeue();
            }
        }

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public Boolean MoveNext()
        {
            return (_Queue.Count > 0 || _IEnumerator.MoveNext());
        }

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Add(myT)

        /// <summary>
        /// Adds an element to the internal queue.
        /// </summary>
        /// <param name="myT">The element to add.</param>
        public void Add(T myT)
        {
            _Queue.Enqueue(myT);
        }

        #endregion


        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        {
            _IEnumerator.Dispose();
        }

        #endregion

    }

}
