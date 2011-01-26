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
    /// A MultiEnumerator takes multiple IEnumerators in its constructor
    /// and makes them behave like a single enumerator.
    /// The order in which objects are returned from both enumerators are with
    /// respect to the order of the enumerators passed into the constructor.
    /// </summary>
    /// <typeparam name="T">The type of the internal enumerator.</typeparam>
	public class SingleEnumerator<T> : IEnumerator<T>
    {

        #region EnumeratorLiveliness

        private enum EnumeratorLiveliness
        {
            PREBIRTH,
            ALIVE,
            DEATH
        }

        #endregion

        #region Data

        private readonly T                    _Element;
        private          EnumeratorLiveliness _Alive;
	
		#endregion
		
		#region Constructor(s)

        #region SingleEnumerator(myElement)

        /// <summary>
        /// Creates a new SingleEnumerator based on the given element.
        /// </summary>
        /// <param name="myElement"></param>
        public SingleEnumerator(T myElement)
        {
            _Element = myElement;
            _Alive   = EnumeratorLiveliness.PREBIRTH;
        }

        #endregion

        #endregion


        #region Current

        /// <summary>
        /// Return the current element of the current IEnumertor.
        /// </summary>
		public T Current
		{
			get
			{

                if (_Alive == EnumeratorLiveliness.ALIVE)
                    return _Element;

                throw new InvalidOperationException();
                
			}
		}

        /// <summary>
        /// Return the current element of the internal IEnumertor.
        /// </summary>
		Object System.Collections.IEnumerator.Current
		{	
			get
			{

                if (_Alive == EnumeratorLiveliness.ALIVE)
                    return _Element;

                throw new InvalidOperationException();

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

            switch (_Alive)
            {
                case EnumeratorLiveliness.PREBIRTH : _Alive = EnumeratorLiveliness.ALIVE;  return true;
                case EnumeratorLiveliness.ALIVE:    _Alive = EnumeratorLiveliness.DEATH; return false;
                case EnumeratorLiveliness.DEATH:   _Alive = EnumeratorLiveliness.DEATH; return false;
            }

            return false;

		}

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the collection.
        /// </summary>
        public void Reset()
		{
            _Alive = EnumeratorLiveliness.PREBIRTH;
		}

        #endregion


        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        { }

        #endregion
	
	}

}
