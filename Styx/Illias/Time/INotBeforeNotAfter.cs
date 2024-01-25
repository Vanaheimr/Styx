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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for the INotBeforeNotAfter interface.
    /// </summary>
    public static class INotBeforeNotAfterExtensions
    {

        #region IsOverlapping         (this ExistingValue,     NewValue,  Tolerance = 1 second)

        /// <summary>
        /// Checks whether the validity timespans of the given objects overlap.
        /// </summary>
        /// <param name="ExistingValue">An existing object.</param>
        /// <param name="NewValue">A new object.</param>
        /// <param name="Tolerance">An optional tolerance for comparing timestamps (default: 1 second).</param>
        public static Boolean IsOverlapping(this INotBeforeNotAfter  ExistingValue,
                                            INotBeforeNotAfter       NewValue,
                                            TimeSpan?                Tolerance   = null)
        {

            var tolerance      = Tolerance               ?? TimeSpan.FromSeconds(1);
            var existingStart  = ExistingValue.NotBefore ?? DateTime.MinValue;
            var existingEnd    = ExistingValue.NotAfter  ?? DateTime.MaxValue;
            var newStart       = NewValue.     NotBefore ?? DateTime.MinValue;
            var newEnd         = NewValue.     NotAfter  ?? DateTime.MaxValue;

            // Check if one interval ends when the other begins
            // respecting the tolerance (no overlap)
            if (existingEnd.IsEqualToWithinTolerance(newStart,      tolerance) ||
                newEnd.     IsEqualToWithinTolerance(existingStart, tolerance))
            {
                return false;
            }

            // Check if the timespans overlap considering the tolerance
            return newStart.Subtract(tolerance) < existingEnd.  Add     (tolerance) &&
                   newEnd.  Add     (tolerance) > existingStart.Subtract(tolerance);

        }

        #endregion

        #region IsNotBeforeWithinRange(this NotBeforeNotAfter, Timestamp, Tolerance = 1 second)

        /// <summary>
        /// Check whether the given NotBefore timestamp is within the given tolerance.
        /// </summary>
        /// <param name="NotBeforeNotAfter">A INotBeforeNotAfter object.</param>
        /// <param name="Timestamp">A timestamp.</param>
        /// <param name="Tolerance">An optional tolerance (default: 1 second)</param>
        public static Boolean IsNotBeforeWithinRange(this INotBeforeNotAfter  NotBeforeNotAfter,
                                                     DateTime                 Timestamp,
                                                     TimeSpan?                Tolerance   = null)

            => Timestamp >= (NotBeforeNotAfter.NotBefore ?? DateTime.MinValue).Subtract(Tolerance ?? TimeSpan.FromSeconds(1));

        #endregion

        #region IsNotAfterWithinRange (this NotBeforeNotAfter, Timestamp, Tolerance = 1 second)

        /// <summary>
        /// Check whether the given NotAfter timestamp is within the given tolerance.
        /// </summary>
        /// <param name="NotBeforeNotAfter">A INotBeforeNotAfter object.</param>
        /// <param name="Timestamp">A timestamp.</param>
        /// <param name="Tolerance">An optional tolerance (default: 1 second)</param>

        public static Boolean IsNotAfterWithinRange(this INotBeforeNotAfter  NotBeforeNotAfter,
                                                    DateTime                 Timestamp,
                                                    TimeSpan?                Tolerance   = null)

            => Timestamp <= (NotBeforeNotAfter.NotAfter ?? DateTime.MaxValue).Add(Tolerance ?? TimeSpan.FromSeconds(1));

        #endregion

    }


    /// <summary>
    /// The common interface for all objects having a "NotBefore" and "NotAfter"
    /// property to control their validity over time.
    /// </summary>
    public interface INotBeforeNotAfter
    {

        /// <summary>
        /// The object is not valid before this optional timestamp.
        /// </summary>
        public DateTime?  NotBefore    { get; }

        /// <summary>
        /// The object is not valid after this optional timestamp.
        /// </summary>
        public DateTime?  NotAfter     { get; }


    }

}
