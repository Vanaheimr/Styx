/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#region Usings

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Min/Max values for decimals.
    /// </summary>
    public struct DecimalMinMax
    {

        #region Min

        private readonly Decimal? _Min;

        /// <summary>
        /// The minimum value or lower bound.
        /// </summary>
        public Decimal? Min
        {
            get
            {
                return _Min;
            }
        }

        #endregion

        #region Max

        private readonly Decimal? _Max;

        /// <summary>
        /// The maximum value or upper bound.
        /// </summary>
        public Decimal? Max
        {
            get
            {
                return _Max;
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new range of decimal values.
        /// </summary>
        /// <param name="Min">The minimum value or lower bound.</param>
        /// <param name="Max">The maximum value or upper bound.</param>
        public DecimalMinMax(Decimal? Min, Decimal? Max)
        {

            #region Initial checks

            if (Min.HasValue && Max.HasValue && Min > Max)
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
        public static DecimalMinMax FromMin(Decimal MinValue)
        {
            return new DecimalMinMax(MinValue, null);
        }

        #endregion

        #region (static) FromMax(MaxValue)

        /// <summary>
        /// Create a new half-open definition having just a maximum value.
        /// </summary>
        /// <param name="MaxValue">The maximum value.</param>
        public static DecimalMinMax FromMax(Decimal MaxValue)
        {
            return new DecimalMinMax(null, MaxValue);
        }

        #endregion


        #region (override) GetHashCode()

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
