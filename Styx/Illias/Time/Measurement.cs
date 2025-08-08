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
    /// This class represents a timestamp value pair of a measurement.
    /// </summary>
    public readonly struct Measurement<TValue, TUnits>
    {

        #region Data

        /// <summary>
        /// The timestamp of the measurement;
        /// </summary>
        public DateTimeOffset  Timestamp    { get; }

        /// <summary>
        /// The value of the measurement;
        /// </summary>
        public TValue          Value        { get; }

        /// <summary>
        /// The unit of the measurement;
        /// </summary>
        public TUnits          Unit         { get; }

        #endregion

        #region Constructor(s)

        #region Measurement(           Value, Unit)

        /// <summary>
        /// Create a new timestamped measurement with units.
        /// </summary>
        /// <param name="Value">The value of the measurement.</param>
        /// <param name="Unit">The unit of the measurement.</param>
        public Measurement(TValue    Value,
                           TUnits    Unit)
        {

            this.Timestamp  = Illias.Timestamp.Now;
            this.Value      = Value;
            this.Unit       = Unit;

        }

        #endregion

        #region Measurement(Timestamp, Value, Unit)

        /// <summary>
        /// Create a new timestamped measurement with units.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the measurement.</param>
        /// <param name="Value">The value of the measurement.</param>
        /// <param name="Unit">The unit of the measurement.</param>
        public Measurement(DateTimeOffset  Timestamp,
                           TValue          Value,
                           TUnits          Unit)
        {

            this.Timestamp  = Timestamp;
            this.Value      = Value;
            this.Unit       = Unit;

        }

        #endregion

        #endregion

    }

}
