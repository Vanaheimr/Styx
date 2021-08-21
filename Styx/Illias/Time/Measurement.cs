/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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
    /// This class represents a timestamp value pair of a measurement.
    /// </summary>
    public struct Measurement<TValue>
    {

        #region Data

        /// <summary>
        /// The timestamp of the measurement;
        /// </summary>
        public readonly DateTime Timestamp;

        /// <summary>
        /// The value of the measurement;
        /// </summary>
        public readonly TValue Value;

        #endregion

        #region Constructor(s)

        #region Measurement(Timestamp, Value)

        /// <summary>
        /// Create a new timestamp value pair of a measurement.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the measurement.</param>
        /// <param name="Value">The value of the measurement.</param>
        public Measurement(DateTime Timestamp, TValue Value)
        {
            this.Timestamp = Timestamp;
            this.Value     = Value;
        }

        #endregion

        #endregion

    }

}
