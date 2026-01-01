/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A generic range of values.
    /// </summary>
    /// <typeparam name="T">The type of the range values.</typeparam>
    /// <param name="Min">The minimum value or lower bound.</param>
    /// <param name="Max">The maximum value or upper bound.</param>
    public readonly struct Range<T>(T Min, T Max)
    {

        #region Properties

        /// <summary>
        /// The minimum value or lower bound.
        /// </summary>
        public T  Min    { get; } = Min;

        /// <summary>
        /// The maximum value or upper bound.
        /// </summary>
        public T  Max    { get; } = Max;

        #endregion


        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return (Min?.GetHashCode() ?? 0) * 3 ^
                       (Max?.GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{Min?.ToString()}' -> '{Max?.ToString()}'";

        #endregion

    }

}
