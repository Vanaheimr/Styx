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

#region Usings

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Min/Max values for UInt64s.
    /// </summary>
    public struct UInt64MinMax
    {

        #region Min

        private readonly UInt64? _Min;

        /// <summary>
        /// The minimum value or lower bound.
        /// </summary>
        public UInt64? Min
        {
            get
            {
                return _Min;
            }
        }

        #endregion

        #region Max

        private readonly UInt64? _Max;

        /// <summary>
        /// The maximum value or upper bound.
        /// </summary>
        public UInt64? Max
        {
            get
            {
                return _Max;
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new range of UInt64 values.
        /// </summary>
        /// <param name="Min">The minimum value or lower bound.</param>
        /// <param name="Max">The maximum value or upper bound.</param>
        public UInt64MinMax(UInt64? Min, UInt64? Max)
        {

            #region Initial checks

            if (Min.HasValue && Max.HasValue && Min > Max)
                throw new ArgumentException("The minimum must not be larger than the maximum!");

            #endregion

            _Min = Min;
            _Max = Max;

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
