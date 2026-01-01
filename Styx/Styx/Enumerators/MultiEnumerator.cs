/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A MultiEnumerator takes multiple IEnumerators in its constructor
    /// and makes them behave like a single enumerator.
    /// The order in which objects are returned from both enumerators are with
    /// respect to the order of the enumerators passed into the constructor.
    /// </summary>
    /// <typeparam name="T">The type of the stored elements.</typeparam>
    public class MultiEnumerator<T> : IEnumerator<T>
    {

        #region Data

        private readonly IEnumerator<IEnumerator<T>>  _AllEnumerators;
        private          IEnumerator<T>               _CurrentEnumerator;

        #endregion

        #region Constructor(s)

        #region MultiEnumerator(Enumerators)

        /// <summary>
        /// Creates a new MultiEnumerator based on the given Enumerators.
        /// </summary>
        /// <param name="Enumerators">The enumerators to be wrapped.</param>
        public MultiEnumerator(params IEnumerator<T>[] Enumerators)
            : this(new List<IEnumerator<T>>(Enumerators))
        { }

        #endregion

        #region MultiEnumerator(Enumerators)

        /// <summary>
        /// Creates a new MultiEnumerator based on the given Enumerators.
        /// </summary>
        /// <param name="Enumerators">The enumerators to be wrapped.</param>
        public MultiEnumerator(IEnumerable<IEnumerator<T>> Enumerators)
        {

            _AllEnumerators = Enumerators.GetEnumerator();

            if (_AllEnumerators.MoveNext())
                _CurrentEnumerator = _AllEnumerators.Current;

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
                return _CurrentEnumerator.Current;
            }
        }

        /// <summary>
        /// Return the current element of the internal IEnumertor.
        /// </summary>
        Object System.Collections.IEnumerator.Current
        {    
            get
            {
                return _CurrentEnumerator.Current;
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

            while (true)
            {

                if (_CurrentEnumerator is null)
                    return false;

                // Move to the next element of the current enumerator
                if (_CurrentEnumerator.MoveNext())
                    return true;

                else
                {

                    // Move to the next enumerator
                    if (_AllEnumerators.MoveNext())
                        _CurrentEnumerator = _AllEnumerators.Current;

                    else
                        return false;

                }

            }

        }

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _AllEnumerators.Reset();
        }

        #endregion


        #region Dispose()

        /// <summary>
        /// Dispose this enumerator.
        /// </summary>
        public void Dispose()
        { }

        #endregion

    }

}
