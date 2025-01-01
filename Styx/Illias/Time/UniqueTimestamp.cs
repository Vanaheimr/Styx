/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// This class will return the current timestamp, but will ensure its
    /// uniqueness which means it will never return the same value twice.
    /// </summary>
    public static class UniqueTimestamp
    {

        #region Data

        private static Int64 lastTimestamp;

        #endregion

        #region Properties

        #region Now

        /// <summary>
        /// Returns an unique timestamp as a DateTime object
        /// </summary>
        public static DateTime Now

            => new ((Int64) GetUniqueTimestamp);

        #endregion

        #region Ticks

        /// <summary>
        /// Returns an unique timestamp as an UInt64
        /// </summary>
        public static UInt64 Ticks

            => GetUniqueTimestamp;

        #endregion

        #endregion


        #region (private) GetUniqueTimestamp

        /// <summary>
        /// Return a unique timestamp
        /// </summary>
        private static UInt64 GetUniqueTimestamp
        {

            get
            {

                Int64 initialValue, newValue;

                do
                {

                    // Save the last known timestamp in a local variable.
                    initialValue  = lastTimestamp;

                    newValue      = Math.Max(Timestamp.Now.Ticks, initialValue + 1);

                }
                // Use CompareExchange to avoid locks!
                while (initialValue != Interlocked.CompareExchange(ref lastTimestamp, newValue, initialValue));

                // Return newValue, as lastTimestamp might
                // already be changed by yet another thread!
                return (UInt64) newValue;

            }

        }

        #endregion


    }

}
