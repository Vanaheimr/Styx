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

    public interface IHistoryEnumerator
    {
        Object Last { get; }
    }

    /// <summary>
    /// A HistoryEnumerator wraps and behaves like a classical IEnumerator.
    /// However, it will remember what was last returned out of the IEnumerator.
    /// </summary>
    /// <typeparam name="T">The type of the internal IEnumerator.</typeparam>
	public class HistoryEnumerator<T> : IHistoryEnumerator, IEnumerator<T>
	{
		
		#region Data
		
	    private readonly IEnumerator<T> _IEnumerator;
	    private          T              _Last;
	
		#endregion
		
		#region Constructor(s)

        #region HistoryEnumerator(myIEnumerator)

        /// <summary>
        /// Creates a new HistoryEnumerator based on the given myIEnumerator.
        /// </summary>
        /// <param name="myIEnumerator">The enumerator to be wrapped.</param>
        public HistoryEnumerator(IEnumerator<T> myIEnumerator)
        {
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
				return _IEnumerator.Current;
			}
		}

        /// <summary>
        /// Return the current element of the internal IEnumertor.
        /// </summary>
		Object System.Collections.IEnumerator.Current
		{	
			get
			{
                return _IEnumerator.Current;
			}
		}

        #endregion

        #region Last

        /// <summary>
        /// Return the last element of the internal IEnumertor.
        /// </summary>
        public T Last
        {
            get
            {
                return _Last;
            }
        }

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
            _Last = _IEnumerator.Current;
			return _IEnumerator.MoveNext();
		}

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the collection.
        /// </summary>
        public void Reset()
		{
            _IEnumerator.Reset();
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
