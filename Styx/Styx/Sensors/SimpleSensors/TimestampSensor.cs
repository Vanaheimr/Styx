﻿/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Styx <http://www.github.com/Vanaheimr/Styx>
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

namespace org.GraphDefined.Vanaheimr.Styx.Sensors.Simple
{

    #region TimestampSensor

    /// <summary>
    /// A sensor measuring the actual time.
    /// </summary>
    public class TimestampSensor : TimestampSensor<SensorId>
    {

        #region TimestampSensor(SensorName)

        /// <summary>
        /// A sensor measuring the actual time.
        /// </summary>
        /// <param name="SensorName">The name of the sensor.</param>
        public TimestampSensor(String SensorName)
            : base(SensorId.NewSensorId, SensorName)
        { }

        #endregion

        #region TimestampSensor(SensorId, SensorName)

        /// <summary>
        /// A sensor measuring the actual time.
        /// </summary>
        /// <param name="SensorId">The unique identification of the sensor.</param>
        /// <param name="SensorName">The name of the sensor.</param>
        public TimestampSensor(SensorId SensorId, String SensorName)
            : base(SensorId, SensorName)
        { }

        #endregion

        #region TimestampSensor(SensorId_UInt64, SensorName)

        /// <summary>
        /// A sensor measuring the actual time.
        /// </summary>
        /// <param name="SensorId_UInt64">The unique identification of the sensor.</param>
        /// <param name="SensorName">The name of the sensor.</param>
        public TimestampSensor(UInt64 SensorId_UInt64, String SensorName)
            : base(new SensorId(SensorId_UInt64), SensorName)
        { }

        #endregion

    }

    #endregion

    #region TimestampSensor<TId>

    /// <summary>
    /// A sensor measuring the actual time.
    /// </summary>
    /// <typeparam name="TId">The type of the unique identification.</typeparam>
    public class TimestampSensor<TId> : AbstractSensor<TId, DateTime>
        where TId : IEquatable<TId>, IComparable<TId>, IComparable
    {

        #region Data

        /// <summary>
        /// The source of randomness.
        /// </summary>
        private readonly Random _Random;

        #endregion

        #region TimestampSensor(SensorId, SensorName)

        /// <summary>
        /// A sensor measuring the actual time.
        /// </summary>
        /// <param name="SensorId">The unique identification of the sensor.</param>
        /// <param name="SensorName">The name of the sensor.</param>
        public TimestampSensor(TId SensorId, String SensorName)
            : base(SensorId, SensorName, "DateTime")
        {
            _Random = new Random();
        }

        #endregion

        #region MoveNext()

        /// <summary>
        /// The current value of this performance counter.
        /// </summary>
        public override Boolean MoveNext()
        {

            // Respect sensor throttling in the AbstractSensor class.
            base.MoveNext();

            _Current = DateTime.Now;
            return true;

        }

        #endregion

    }

    #endregion

}
