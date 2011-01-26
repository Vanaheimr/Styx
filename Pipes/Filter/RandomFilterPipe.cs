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
    /// The RandomFilterPipe filters out objects that pass through it using a biased coin.
    /// For each passing object, a random number generator creates a double value between 0 and 1.
    /// If the randomly generated double is less than or equal the provided bias, then the object is allowed to pass.
    /// If the randomly generated double is greater than the provided bias, then the object is not allowed to pass.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class RandomFilterPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>
    {

        #region Data

        private static readonly Random RANDOM = new Random();
        private        readonly double _Bias;

        #endregion

        #region Constructor(s)

        #region RandomFilterPipe(myBias)

        public RandomFilterPipe(double myBias)
        {
            this._Bias = myBias;
        }

        #endregion

        #endregion



        protected override S processNextStart()
        {
            while (true)
            {

                starts.MoveNext();
                var _S = starts.Current;
                
                if (_Bias >= RANDOM.NextDouble())
                    return _S;

            }
        }


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "<" + _Bias + ">";
        }

        #endregion

    }

}
