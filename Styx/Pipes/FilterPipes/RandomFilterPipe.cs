﻿/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/Pipes.NET>
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

#endregion

namespace de.ahzf.Styx
{
    
    /// <summary>
    /// The RandomFilterPipe filters out objects that pass through it using a biased coin.
    /// For each passing object, a random number generator creates a double value between 0 and 1.
    /// If the randomly generated double is less than or equal the provided bias, then the object is allowed to pass.
    /// If the randomly generated double is greater than the provided bias, then the object is not allowed to pass.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class RandomFilterPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>
    {

        #region Data

        private static readonly Random _Random = new Random();
        private        readonly Double _Bias;

        #endregion

        #region Constructor(s)

        #region RandomFilterPipe(myBias)

        /// <summary>
        /// Creates a new RandomFilterPipe.
        /// </summary>
        /// <param name="myBias">The bias.</param>
        public RandomFilterPipe(Double myBias)
        {
            _Bias = myBias;
        }

        #endregion

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

            while (_InternalEnumerator.MoveNext())
            {

                if (_Bias >= _Random.NextDouble())
                {
                    _CurrentElement = _InternalEnumerator.Current;
                    return true;
                }

            }

            return false;

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + _Bias + ">";
        }

        #endregion

    }

}
