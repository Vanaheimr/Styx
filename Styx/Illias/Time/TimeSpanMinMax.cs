/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Min/Max values for TimeSpans.
    /// </summary>
    public readonly struct TimeSpanMinMax
    {

        #region Properties

        /// <summary>
        /// The minimum value or lower bound.
        /// </summary>
        public TimeSpan?  Min    { get; }

        /// <summary>
        /// The maximum value or upper bound.
        /// </summary>
        public TimeSpan?  Max    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new range of TimeSpan values.
        /// </summary>
        /// <param name="Min">The minimum value or lower bound.</param>
        /// <param name="Max">The maximum value or upper bound.</param>
        public TimeSpanMinMax(TimeSpan? Min,
                              TimeSpan? Max)
        {

            #region Initial checks

            if (Min.HasValue &&
                Max.HasValue &&
                Min > Max)
            {
                throw new ArgumentException("The minimum must not be larger than the maximum!");
            }

            #endregion

            this.Min = Min;
            this.Max = Max;

            unchecked
            {
                hashCode = (Min?.GetHashCode() ?? 0) * 3 ^
                           (Max?.GetHashCode() ?? 0);
            }

        }

        #endregion


        #region (static) FromMin(Min)

        /// <summary>
        /// Create a new half-open definition having just a minimum value.
        /// </summary>
        /// <param name="Min">The minimum value.</param>
        public static TimeSpanMinMax FromMin(TimeSpan Min)

            => new (Min,
                    null);

        #endregion

        #region (static) FromMax(Max)

        /// <summary>
        /// Create a new half-open definition having just a maximum value.
        /// </summary>
        /// <param name="Max">The maximum value.</param>
        public static TimeSpanMinMax FromMax(TimeSpan Max)

            => new (null,
                    Max);

        #endregion


        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Min?.ToString() ?? "-"} -> {Max?.ToString() ?? "-"}";

        #endregion

    }

}
