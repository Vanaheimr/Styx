/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Threading;
using System.Collections.Generic;

#if SILVERLIGHT
using de.ahzf.Silverlight;
#endif

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    /// <summary>
    /// The StdDevPipe produces a side effect that is the
    /// sliding standard deviation and the average of the input.
    /// </summary>
    public class StdDevPipe : AbstractTwoSideEffectsPipe<Double, Double, Double, Double>
    {

        #region Data

        private Int64  Counter;
        private Double Sum;
        private Double QuadratSum;

        #endregion

        #region Constructor(s)

        #region StdDevPipe(SourceElement)

        /// <summary>
        /// Creates a new StdDevPipe calculating a side effect that is the
        /// sliding standard deviation and the average of the input.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        public StdDevPipe(IEndPipe<Double> SourcePipe)
            : base(SourcePipe, 0, 0)
        {
            this.Counter  = 0;
        }

        #endregion

        #region StdDevPipe(SourceEnumerator)

        /// <summary>
        /// Creates a new StdDevPipe calculating a side effect that is the
        /// sliding standard deviation and the average of the input.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public StdDevPipe(IEnumerator<Double> SourceEnumerator)
            : base(SourceEnumerator, 0, 0)
        {
            this.Counter  = 0;
        }

        #endregion

        #region StdDevPipe(SourceEnumerable)

        /// <summary>
        /// Creates a new StdDevPipe calculating a side effect that is the
        /// sliding standard deviation and the average of the input.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        public StdDevPipe(IEnumerable<Double> SourceEnumerable)
            : base(SourceEnumerable, 0, 0)
        {
            this.Counter  = 0;
        }

        #endregion

        #endregion


        #region (private) AddToSum(Summand)

        private Double AddToSum(Double Summand)
        {

            Double _InitialValue, _NewLocalSum;

            do
            {
                // Save the current Sum locally.
                _InitialValue = Sum;

                // Add the Summand to the Sum.
                _NewLocalSum  = _InitialValue + Summand;

            }
            // If Sum still equals _InitialValue, exchange it with _NewLocalSum
#if SILVERLIGHT
            while (_InitialValue != SilverlightTools.CompareExchange(ref Sum, _NewLocalSum, _InitialValue));
#else
            while (_InitialValue != Interlocked.CompareExchange(ref Sum, _NewLocalSum, _InitialValue)) ;
#endif

            // Return _NewLocalSum as Sum may already be alternated
            // by a concurrent thread between the end of the loop
            // and the function returns.
            return _NewLocalSum;

        }

        #endregion

        #region (private) AddToQuadratSum(Summand)

        private Double AddToQuadratSum(Double Summand)
        {

            Double _InitialValue, _NewLocalSum;

            do
            {
                // Save the current Sum locally.
                _InitialValue = QuadratSum;

                // Add the Summand^2 to the Sum.
                _NewLocalSum  = _InitialValue + Summand * Summand;

            }
            // If Sum still equals _InitialValue, exchange it with _NewLocalSum
#if SILVERLIGHT
            while (_InitialValue != SilverlightTools.CompareExchange(ref QuadratSum, _NewLocalSum, _InitialValue));
#else
            while (_InitialValue != Interlocked.CompareExchange(ref QuadratSum, _NewLocalSum, _InitialValue));
#endif

            // Return _NewLocalSum as Sum may already be alternated
            // by a concurrent thread between the end of the loop
            // and the function returns.
            return _NewLocalSum;

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

            if (SourcePipe == null)
                return false;

            if (SourcePipe.MoveNext())
            {

                _CurrentElement = SourcePipe.Current;
                var _Counter = Interlocked.Increment(ref Counter);

                var _Sum = AddToSum(_CurrentElement);
                InternalSideEffect1 = AddToQuadratSum(_CurrentElement) - (Math.Pow(_Sum, 2) / _Counter);
                InternalSideEffect2 = _Sum / _Counter;

                if (Counter > 1 && Counter < 30)
                    InternalSideEffect1 = InternalSideEffect1 / (_Counter - 1);  // corr. Var.
                else
                    InternalSideEffect1 = InternalSideEffect1 / _Counter;

                return true;

            }

            return false;

        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<Counter: " + Counter + ", StdDev: " + InternalSideEffect1 + ", Average: " + InternalSideEffect2 + ">";
        }

        #endregion

    }

}
