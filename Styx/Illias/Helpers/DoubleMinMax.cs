/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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
    /// Min/Max values for doubles.
    /// </summary>
    public struct DoubleMinMax
    {

        #region Min

        private readonly Double? _Min;

        /// <summary>
        /// The minimum value or lower bound.
        /// </summary>
        public Double? Min
        {
            get
            {
                return _Min;
            }
        }

        #endregion

        #region Max

        private readonly Double? _Max;

        /// <summary>
        /// The maximum value or upper bound.
        /// </summary>
        public Double? Max
        {
            get
            {
                return _Max;
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new range of double values.
        /// </summary>
        /// <param name="Min">The minimum value or lower bound.</param>
        /// <param name="Max">The maximum value or upper bound.</param>
        public DoubleMinMax(Double? Min, Double? Max)
        {

            #region Initial checks

            if (Min.HasValue && Max.HasValue && Min > Max)
                throw new ArgumentException("The minimum must not be larger than the maximum!");

            #endregion

            _Min = Min;
            _Max = Max;

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
