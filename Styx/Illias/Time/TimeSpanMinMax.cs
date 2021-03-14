/*
 * Copyright (c) 2010-2021 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Min/Max values for TimeSpans.
    /// </summary>
    public struct TimeSpanMinMax
    {

        #region Min

        private readonly TimeSpan? _Min;

        /// <summary>
        /// The minimum value or lower bound.
        /// </summary>
        public TimeSpan? Min
        {
            get
            {
                return _Min;
            }
        }

        #endregion

        #region Max

        private readonly TimeSpan? _Max;

        /// <summary>
        /// The maximum value or upper bound.
        /// </summary>
        public TimeSpan? Max
        {
            get
            {
                return _Max;
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new range of TimeSpan values.
        /// </summary>
        /// <param name="Min">The minimum value or lower bound.</param>
        /// <param name="Max">The maximum value or upper bound.</param>
        public TimeSpanMinMax(TimeSpan? Min, TimeSpan? Max)
        {

            #region Initial checks

            if (Min > Max)
                throw new ArgumentException("The minimum must not be larger than the maximum!");

            #endregion

            _Min = Min;
            _Max = Max;

        }

        #endregion


        #region (static) FromMin(MinValue)

        /// <summary>
        /// Create a new half-open definition having just a minimum value.
        /// </summary>
        /// <param name="MinValue">The minimum value.</param>
        public static TimeSpanMinMax FromMin(TimeSpan MinValue)
        {
            return new TimeSpanMinMax(MinValue, null);
        }

        #endregion

        #region (static) FromMax(MaxValue)

        /// <summary>
        /// Create a new half-open definition having just a maximum value.
        /// </summary>
        /// <param name="MaxValue">The maximum value.</param>
        public static TimeSpanMinMax FromMax(TimeSpan MaxValue)
        {
            return new TimeSpanMinMax(null, MaxValue);
        }

        #endregion


        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                return _Min.GetHashCode() * 17 ^ _Max.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Concat(_Min.ToString(), " -> ", _Max.ToString());
        }

        #endregion

    }

}
