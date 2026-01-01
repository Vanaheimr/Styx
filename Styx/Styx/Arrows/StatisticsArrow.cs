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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    public class Statistics
    {

        public Double Min { get; private set; }
        public Double Avg { get; private set; }
        public Double StdDev { get; private set; }
        public Double Variance { get; private set; }
        public Double Max { get; private set; }

        public Statistics(Double Min,
                          Double Avg,
                          Double StdDev,
                          Double Variance,
                          Double Max)
        {

            this.Min = Min;
            this.Avg = Avg;
            this.StdDev = StdDev;
            this.Variance = Variance;
            this.Max = Max;

        }

    }

    /// <summary>
    /// The StdDevArrow consumes doubles and emitts the
    /// sliding standard deviation and the average of
    /// messages/objects that have passed through it.
    /// </summary>
    public class StatisticsArrow : AbstractArrow<Double, Statistics>
    {

        #region Data

        private Int64  Counter;
        private Double Sum;
        private Double QuadratSum;
        private Double Min = Double.MaxValue;
        private Double Max = Double.MinValue;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The StdDevArrow consumes doubles and emitts the
        /// sliding standard deviation and the average of
        /// messages/objects that have passed through it.
        /// </summary>
        public StatisticsArrow(IArrowSender<Double>? ArrowSender = null)

            : base(ArrowSender)

        { }

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
            while (_InitialValue != Interlocked.CompareExchange(ref Sum, _NewLocalSum, _InitialValue));
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


        #region ProcessMessage(MessageIn, out MessageOut)

        /// <summary>
        /// Process the incoming message and return an outgoing message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        /// <param name="MessageOut">The outgoing message.</param>
        protected override Boolean ProcessMessage(EventTracking_Id EventTrackingId, Double MessageIn, out Statistics MessageOut)
        {

            var Value     = MessageIn;
            var _Counter  = Interlocked.Increment(ref Counter);

            if (Value < Min)
                Min = Value;

            if (Value > Max)
                Max = Value;

            var _Sum      = AddToSum(Value);
            var _Variance = AddToQuadratSum(Value) - (Math.Pow(_Sum, 2) / _Counter);
            var _Average  = _Sum / _Counter;

            if (Counter > 1 && Counter < 30)
                _Variance = _Variance / (_Counter - 1);  // corr. Var.
            else
                _Variance = _Variance / _Counter;

            MessageOut = new Statistics(Min, _Average, Math.Sqrt(_Variance), _Variance, Max);

            return true;

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of this Arrow.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<Counter: " + Counter + ">";
        }

        #endregion

    }

}
