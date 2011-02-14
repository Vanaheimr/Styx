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

#endregion

namespace de.ahzf.Pipes
{
    
    /// <summary>
    /// The RangeFilterPipe will only allow a sequential subset of its incoming
    /// objects to be emitted to its output. This pipe can be provided -1 for
    /// both its high and low range to denote a wildcard for high and/or low.
    /// Note that -1 for both high and low is equivalent to the IdentityPipe.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class RangeFilterPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>
    {

        #region Data

        private readonly Int32 _Low;
        private readonly Int32 _High;
        private          Int32 _Counter;

        #endregion

        #region Constructor(s)

        #region RangeFilterPipe(myLow, myHigh)

        /// <summary>
        /// Creates a new RangeFilterPipe.
        /// </summary>
        /// <param name="myLow">The minima.</param>
        /// <param name="myHigh">The maxima.</param>
        public RangeFilterPipe(Int32 myLow, Int32 myHigh)
        {

            if (myLow > -1 && myHigh > -1 && myLow >= myHigh)
                throw new ArgumentOutOfRangeException("myLow must be smaller than myHigh!");

            _Low     = myLow;
            _High    = myHigh;
            _Counter = -1;

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

            while (true)
            {

                if (_InternalEnumerator.MoveNext())
                {

                    var _S = _InternalEnumerator.Current;

                    _Counter++;

                    if ((_Low  == -1 || _Counter >= _Low) &&
                        (_High == -1 || _Counter <  _High))
                    {
                        _CurrentElement = _S;
                        return true;
                    }

                    if (_High > 0 && _Counter > _High)
                        return false;

                }

                else
                    return false;

            }

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + _Low + "," + _High + ">";
        }

        #endregion

    }

}
