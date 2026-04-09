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
    /// Implements a practical scheduler for measurement rounds using exponentially
    /// distributed inter-arrival times (Poisson-inspired) with clamping.
    ///
    /// Why this approach?
    ///  - Exponentially distributed intervals (before clamping) guarantee strong
    ///    statistical independence between drones WITHOUT any coordination.
    ///  - No aliasing effects with periodic server behaviors (cron jobs, log rotation,
    ///    network effects, etc.).
    ///  - Near-memoryless behavior in the typical operating range gives zero
    ///    information about when the next one will occur.
    ///  - The aggregate traffic from N independent drones remains approximately Poisson
    ///    with rate = N × λ (especially when clamping rarely triggers).
    ///
    /// The mean interval is configurable (e.g., 60 seconds).
    /// Actual intervals follow Exp(1/mean), clamped to [min, max] to avoid unreasonably
    /// short bursts or long gaps.
    /// 
    /// Important note:
    /// Actual intervals follow an exponential distribution that is clamped to [min, max]
    /// to avoid unreasonably short bursts or long gaps. This technically makes the
    /// distribution a winsorized exponential rather than a pure exponential.
    /// The strict mathematical properties (perfect memorylessness, exact superposition)
    /// hold only approximately — but the practical benefits are largely preserved.
    /// </summary>
    /// <param name="RandomSource">Optional random source for testing; defaults to Random.Shared.</param>
    public sealed class PoissonScheduler(Random? RandomSource = null)
    {

        #region Data

        private readonly Random random = RandomSource ?? Random.Shared;

        #endregion


        #region NextInterval          (                 MeanInterval, MinInterval, MaxInterval)

        /// <summary>
        /// Calculates the next interval for a Poisson-scheduled measurement.
        /// </summary>
        /// <param name="MeanInterval">The target mean interval between measurements.</param>
        /// <param name="MinInterval">Minimum clamped interval (default: 10s).</param>
        /// <param name="MaxInterval">Maximum clamped interval (default: 5× mean).</param>
        public TimeSpan NextInterval(TimeSpan   MeanInterval,
                                     TimeSpan?  MinInterval   = null,
                                     TimeSpan?  MaxInterval   = null)
        {

            if (MeanInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(MeanInterval), "MeanInterval must be positive!");

            var min       = MinInterval ?? TimeSpan.FromSeconds(10);
            var max       = MaxInterval ?? 5 * MeanInterval;

            if (MinInterval is not null && MinInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(MinInterval), "MinInterval must be positive!");

            if (MaxInterval is not null && MaxInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(MaxInterval), "MaxInterval must be positive!");

            if (min >= max)
                throw new ArgumentException("MinInterval must not be greater than or equal to MaxInterval!");

            // Exponential distribution: X = -mean × ln(U), where U ~ Uniform(0,1)
            // We use 1 - U to avoid ln(0).
            double u;
            do
            {
                u = random.NextDouble();
            }
            while (u <= 0.0);

            var expValue  = -MeanInterval.TotalSeconds * Math.Log(u);
            var interval  = TimeSpan.FromSeconds(expValue);

            // Clamp to [min, max]
            if (interval < min) interval = min;
            if (interval > max) interval = max;

            return interval;

        }

        #endregion

        #region NextTimestamp         (LastMeasurement, MeanInterval, MinInterval, MaxInterval)

        /// <summary>
        /// Calculates the next timestamp for a Poisson-scheduled measurement based on the last measurement time.
        /// </summary>
        /// <param name="LastMeasurement">The timestamp of the last measurement.</param>
        /// <param name="MeanInterval">The target mean interval between measurements.</param>
        /// <param name="MinInterval">Minimum clamped interval (default: 10s).</param>
        /// <param name="MaxInterval">Maximum clamped interval (default: 5× mean).</param>
        public DateTimeOffset NextTimestamp(DateTimeOffset  LastMeasurement,
                                            TimeSpan        MeanInterval,
                                            TimeSpan?       MinInterval   = null,
                                            TimeSpan?       MaxInterval   = null)

            => LastMeasurement + NextInterval(
                                     MeanInterval,
                                     MinInterval,
                                     MaxInterval
                                 );

        #endregion


        #region JitteredInterval      (                 MeanInterval)

        /// <summary>
        /// Get a simple jittered interval (±25% around the mean).
        /// Used as fallback when Poisson timing is not desired.
        /// </summary>
        /// <param name="MeanInterval">The target mean interval between measurements.</param>
        public TimeSpan JitteredInterval(TimeSpan MeanInterval)
        {

            if (MeanInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(MeanInterval), "MeanInterval must be positive!");

            return TimeSpan.FromSeconds(
                       MeanInterval.TotalSeconds * (0.75 + random.NextDouble() * 0.50)
                   );

        }

        #endregion

        #region JitteredNextTimestamp (LastMeasurement, MeanInterval)

        /// <summary>
        /// Calculates the next timestamp for a jittered measurement.
        /// </summary>
        /// <param name="LastMeasurement">The timestamp of the last measurement.</param>
        /// <param name="MeanInterval">The target mean interval between measurements.</param>
        public DateTimeOffset JitteredNextTimestamp(DateTimeOffset  LastMeasurement,
                                                    TimeSpan        MeanInterval)

            => LastMeasurement + JitteredInterval(MeanInterval);

        #endregion


        #region Delay                 (                 MeanInterval, CancellationToken)

        /// <summary>
        /// Wait for the next Poisson-scheduled interval.
        /// </summary>
        /// <param name="MeanInterval">The target mean interval between measurements.</param>
        /// <param name="Action">An optional action to receive the actual interval before waiting.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel the delay.</param>
        public async Task Delay(TimeSpan           MeanInterval,
                                Action<TimeSpan>?  Action              = null,
                                CancellationToken  CancellationToken   = default)
        {

            var interval = NextInterval(MeanInterval);

            Action?.Invoke(interval);

            await Task.Delay(
                      interval,
                      CancellationToken
                  );

        }

        #endregion

        #region DelayJittered         (                 MeanInterval, CancellationToken)

        /// <summary>
        /// Wait for the next jittered interval.
        /// </summary>
        /// <param name="MeanInterval">The target mean interval between measurements.</param>
        /// <param name="Action">An optional action to receive the actual interval before waiting.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel the delay.</param>
        public async Task DelayJittered(TimeSpan           MeanInterval,
                                        Action<TimeSpan>?  Action              = null,
                                        CancellationToken  CancellationToken   = default)
        {

            var interval = JitteredInterval(MeanInterval);

            Action?.Invoke(interval);

            await Task.Delay(
                      interval,
                      CancellationToken
                  );

        }

        #endregion

    }

}
