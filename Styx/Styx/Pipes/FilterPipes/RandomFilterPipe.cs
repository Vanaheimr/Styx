/*
 * Copyright (c) 2010-2021 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Styx <http://www.github.com/Vanaheimr/Styx>
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
    /// The RandomFilterPipe filters out objects that pass through it using a biased coin.
    /// For each passing object, a random number generator creates a double value between 0 and 1.
    /// If the randomly generated double is less than or equal the provided bias, then the object is allowed to pass.
    /// If the randomly generated double is greater than the provided bias, then the object is not allowed to pass.
    /// </summary>
    public static class RandomFilterPipeExtensions
    {

        #region RandomFilter(this SourcePipe, Bias, Random = null)

        /// <summary>
        /// The RandomFilterPipe filters out objects that pass through it using a biased coin.
        /// For each passing object, a random number generator creates a double value between 0 and 1.
        /// If the randomly generated double is less than or equal the provided bias, then the object is allowed to pass.
        /// If the randomly generated double is greater than the provided bias, then the object is not allowed to pass.        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="Bias">The bias.</param>
        /// <param name="Random">An optional source of randomness.</param>
        /// <typeparam name="S">The type of the elements within the filter.</typeparam>
        public static RandomFilterPipe<S> RandomFilter<S>(this IEndPipe<S> SourcePipe, Double Bias, Random Random = null)
        {
            return new RandomFilterPipe<S>(SourcePipe, Bias, Random);
        }

        #endregion

    }


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

        private readonly Random Random;
        private readonly Double Bias;

        #endregion

        #region Constructor(s)

        #region RandomFilterPipe(SourceElement, Bias, Random = null)

        /// <summary>
        /// Creates an new random filter pipe using the given single value as element source.
        /// </summary>
        /// <param name="SourceElement">A single value as element source.</param>
        /// <param name="Bias">The bias.</param>
        /// <param name="Random">An optional source of randomness.</param>
        public RandomFilterPipe(S SourceElement, Double Bias, Random Random = null)
            : base(SourceElement)
        {
            this.Bias    = Bias;
            this.Random  = (Random == null) ? new Random() : Random;
        }

        #endregion

        #region RandomFilterPipe(SourcePipe, Bias, Random = null)

        /// <summary>
        /// Creates an new random filter pipe using the given pipe as element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="Bias">The bias.</param>
        /// <param name="Random">An optional source of randomness.</param>
        public RandomFilterPipe(IEndPipe<S> SourcePipe, Double Bias, Random Random = null)
            : base(SourcePipe)
        {
            this.Bias    = Bias;
            this.Random  = (Random == null) ? new Random() : Random;
        }

        #endregion

        #region RandomFilterPipe(SourceEnumerator, Bias, Random = null)

        /// <summary>
        /// Creates an new random filter pipe using the given enumerator as element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        /// <param name="Bias">The bias.</param>
        /// <param name="Random">An optional source of randomness.</param>
        public RandomFilterPipe(IEnumerator<S> SourceEnumerator, Double Bias, Random Random = null)
            : base(SourceEnumerator)
        {
            this.Bias    = Bias;
            this.Random  = (Random == null) ? new Random() : Random;
        }

        #endregion

        #region RandomFilterPipe(SourceEnumerable, Bias, Random = null)

        /// <summary>
        /// Creates an new random filter pipe using the given enumerable as element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        /// <param name="Bias">The bias.</param>
        /// <param name="Random">An optional source of randomness.</param>
        public RandomFilterPipe(IEnumerable<S> SourceEnumerable, Double Bias, Random Random = null)
            : base(SourceEnumerable)
        {
            this.Bias    = Bias;
            this.Random  = (Random == null) ? new Random() : Random;
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

            if (SourcePipe == null)
                return false;

            while (SourcePipe.MoveNext())
            {

                if (Bias >= Random.NextDouble())
                {
                    _CurrentElement = SourcePipe.Current;
                    return true;
                }

            }

            return false;

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + Bias + ">";
        }

        #endregion

    }

}
