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

using System.Diagnostics;
using System.Runtime.InteropServices;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public delegate Task RAMUsageMonitorHandler        (ResourcesMonitor  Sender,
                                                        DateTime          Timestamp,
                                                        UInt64            RAMUsage,
                                                        UInt64            RAMUsageThreshold);

    public delegate Task FreeSystemMemoryMonitorHandler(ResourcesMonitor  Sender,
                                                        DateTime          Timestamp,
                                                        MemoryMetrics     MemoryMetrics);

    public delegate Task LowDiskSpaceMonitorHandler    (ResourcesMonitor  Sender,
                                                        DateTime          Timestamp,
                                                        Double            FreeDiscSpacePercentage);

    public class MemoryMetrics
    {

        #region Properties

        public Double  Total             { get; }
        public Double  Free              { get; }
        public Double  Used              { get; }
        public Double  FreePercentage    { get; }
        public Double  UsedPercentage    { get; }

        #endregion

        #region Constructor(s)

        #region MemoryMetrics(Total, Free, Used)

        public MemoryMetrics(Double  Total,
                             Double  Free,
                             Double  Used)
        {

            this.Total           = Total;
            this.Free            = Free;
            this.Used            = Used;
            this.FreePercentage  = this.Free / this.Total * 100;
            this.UsedPercentage  = this.Used / this.Total * 100;

        }

        #endregion

        #region MemoryMetrics(Total, Free)

        public MemoryMetrics(Double  Total,
                             Double  Free)
        {

            this.Total           = Total;
            this.Free            = Free;
            this.Used            = Total - Free;
            this.FreePercentage  = this.Free / this.Total * 100;
            this.UsedPercentage  = this.Used / this.Total * 100;

        }

        #endregion

        #endregion

    }

    public class ResourcesMonitor
    {

        #region Data

        private Timer? timer;

        #endregion

        #region Properties

        /// <summary>
        /// The checking interval.
        /// </summary>
        public TimeSpan                            CheckInterval                 { get; set; }

        /// <summary>
        /// The current process identification.
        /// </summary>
        public Int32                               ProcessId                     { get; }

        /// <summary>
        /// The current process.
        /// </summary>
        public Process                             Process                       { get; }


        /// <summary>
        /// The threshold in MBytes of RAM used before the OnHighRAMUsage event will be triggered.
        /// </summary>
        public UInt64                              HighRAMUsageThreshold         { get; }

        /// <summary>
        /// The threshold in % of free RAM left before the OnLowSystemMemory event will be triggered.
        /// </summary>
        public Double                              FreeSystemMemoryThreshold     { get; }

        /// <summary>
        /// The threshold in % of free hard disc space left before the OnLowDiskSpace event will be triggered.
        /// </summary>
        public IEnumerable<Tuple<String, Double>>  PathAndFreeDiscSpaceThresholds    { get; }


        /// <summary>
        /// The latest current RAM usage.
        /// </summary>
        public UInt64                              CurrentRAMUsage               { get; private set; }

        /// <summary>
        /// The latest free system memory metrics.
        /// </summary>
        public MemoryMetrics?                      FreeSystemMemoryMetrics       { get; private set; }

        /// <summary>
        /// The latest free disc space percentage.
        /// </summary>
        public Double                              FreeDiscSpacePercentage       { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// An event called whenever the current RAM usage has changed.
        /// </summary>
        public event RAMUsageMonitorHandler?          OnCurrentRAMUsage;

        /// <summary>
        /// An event called whenever the current RAM usage is above its threshold.
        /// </summary>
        public event RAMUsageMonitorHandler?          OnHighRAMUsage;

        /// <summary>
        /// An event called whenever the current free system memory is below its threshold.
        /// </summary>
        public event FreeSystemMemoryMonitorHandler?  OnLowSystemMemory;

        /// <summary>
        /// An event called whenever the current free disc space is below its threshold.
        /// </summary>
        public event LowDiskSpaceMonitorHandler?      OnLowDiskSpace;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new resources monitor.
        /// </summary>
        /// <param name="HighRAMUsageThreshold">An threshold in MBytes of RAM used before the OnHighRAMUsage event will be triggered.</param>
        /// <param name="FreeSystemMemoryThreshold">An threshold in % of free RAM left before the OnLowSystemMemory event will be triggered.</param>
        /// <param name="PathAndFreeSpaceThresholds">An threshold in % of free hard disc space left before the OnLowDiskSpace event will be triggered.</param>
        /// <param name="CheckInterval">An optional checking interval.</param>
        public ResourcesMonitor(UInt64                              HighRAMUsageThreshold,
                                Double                              FreeSystemMemoryThreshold,
                                IEnumerable<Tuple<String, Double>>  PathAndFreeSpaceThresholds,
                                TimeSpan?                           CheckInterval   = null)
        {

            this.CheckInterval               = CheckInterval ?? TimeSpan.FromMinutes(1);

            this.ProcessId                   = Environment.ProcessId;
            this.Process                     = Process.GetProcessById(ProcessId);

            this.HighRAMUsageThreshold       = HighRAMUsageThreshold;
            this.FreeSystemMemoryThreshold   = FreeSystemMemoryThreshold;
            this.PathAndFreeDiscSpaceThresholds  = PathAndFreeSpaceThresholds.Distinct();

            if (CheckInterval.HasValue)
                StartMonitoring(CheckInterval.Value);

        }

        #endregion


        #region (private, timer) CheckResources(State)

        private async void CheckResources(Object? State)
        {

            var now = Timestamp.Now;

            #region RAM usage of this process (MBytes)

            CurrentRAMUsage = (UInt64) Process.WorkingSet64 / (1024 * 1024);

            // High RAM usage
            if (CurrentRAMUsage > HighRAMUsageThreshold)
            {

                var onHighRAMUsage = OnHighRAMUsage?.GetInvocationList()?.Cast<RAMUsageMonitorHandler>()
                                         ?? Array.Empty<RAMUsageMonitorHandler>();

                if (onHighRAMUsage.Any())
                    await Task.WhenAll(onHighRAMUsage.
                                       Select(async ramUsageMonitorHandler => {
                                           await ramUsageMonitorHandler(this,
                                                                        now,
                                                                        CurrentRAMUsage,
                                                                        HighRAMUsageThreshold);
                                       })).
                                       ConfigureAwait(false);

            }

            // Current RAM usage
            var onCurrentRAMUsage = OnCurrentRAMUsage?.GetInvocationList()?.Cast<RAMUsageMonitorHandler>()
                                        ?? Array.Empty<RAMUsageMonitorHandler>();

            if (onCurrentRAMUsage.Any())
                await Task.WhenAll(onCurrentRAMUsage.
                                   Select(async ramUsageMonitorHandler => {
                                       await ramUsageMonitorHandler(this,
                                                                    now,
                                                                    CurrentRAMUsage,
                                                                    HighRAMUsageThreshold);
                                   })).
                                   ConfigureAwait(false);

            #endregion

            #region Free system memory        (%)

            FreeSystemMemoryMetrics = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                                      RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                                          ? GetMemoryMetricsOnUnix()
                                          : GetMemoryMetricsOnWindows();

            if (FreeSystemMemoryMetrics is not null &&
                FreeSystemMemoryMetrics.FreePercentage < FreeSystemMemoryThreshold)
            {

                var onLowSystemMemory = OnLowSystemMemory?.GetInvocationList()?.Cast<FreeSystemMemoryMonitorHandler>()
                                             ?? Array.Empty<FreeSystemMemoryMonitorHandler>();

                if (onLowSystemMemory.Any())
                    await Task.WhenAll(onLowSystemMemory.
                                       Select(async freeSystemMemoryMonitorHandler => {
                                           await freeSystemMemoryMonitorHandler(this,
                                                                                now,
                                                                                FreeSystemMemoryMetrics);
                                       })).
                                       ConfigureAwait(false);

            }

            #endregion

            #region Free disc space           (%)

            foreach (var pathAndFreeDiscSpaceThreshold in PathAndFreeDiscSpaceThresholds)
            {

                var driveInfo            = new DriveInfo(pathAndFreeDiscSpaceThreshold.Item1);
                FreeDiscSpacePercentage  = (Double) driveInfo.AvailableFreeSpace / driveInfo.TotalSize * 100;

                if (FreeDiscSpacePercentage < pathAndFreeDiscSpaceThreshold.Item2)
                {

                    var onLowDiskSpace = OnLowDiskSpace?.GetInvocationList()?.Cast<LowDiskSpaceMonitorHandler>()
                                             ?? Array.Empty<LowDiskSpaceMonitorHandler>();

                    if (onLowDiskSpace.Any())
                        await Task.WhenAll(onLowDiskSpace.
                                           Select(async lowDiskSpaceMonitorHandler => {
                                               await lowDiskSpaceMonitorHandler(this,
                                                                                now,
                                                                                FreeDiscSpacePercentage);
                                           })).
                                           ConfigureAwait(false);

                }

            }

            #endregion

        }

        #endregion

        #region (private static) GetMemoryMetricsOnWindows()

        /// <summary>
        /// Get memory metrics on Windows.
        /// </summary>
        private static MemoryMetrics? GetMemoryMetricsOnWindows()
        {

            var processInfo = new ProcessStartInfo {
                                  FileName                = "wmic",
                                  Arguments               = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
                                  RedirectStandardOutput  = true
                              };

            var output = "";

            using (var process = Process.Start(processInfo))
            {
                output = process?.StandardOutput.ReadToEnd() ?? "";
            }

            var lines = output.Trim().Split("\n");

            if (lines.Length == 2)
            {

                var freeMemoryParts   = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
                var totalMemoryParts  = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

                return new MemoryMetrics(
                           Total: Math.Round(Double.Parse(totalMemoryParts[1]) / 1024, 0),
                           Free:  Math.Round(Double.Parse(freeMemoryParts[1])  / 1024, 0)
                       );

            }

            return null;

        }

        #endregion

        #region (private static) GetMemoryMetricsOnUnix()

        /// <summary>
        /// Get memory metrics on UNIX.
        /// </summary>
        private static MemoryMetrics? GetMemoryMetricsOnUnix()
        {

            var processInfo = new ProcessStartInfo("free -m") {
                                  FileName                = "/bin/bash",
                                  Arguments               = "-c \"free -m\"",
                                  RedirectStandardOutput  = true
                              };

            var output = "";

            using (var process = Process.Start(processInfo))
            {
                output = process?.StandardOutput.ReadToEnd() ?? "";
            }

            var lines = output.Split("\n");

            if (lines.Length == 2)
            {

                var memory  = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                return new MemoryMetrics(
                           Total: Double.Parse(memory[1]),
                           Free:  Double.Parse(memory[3]),
                           Used:  Double.Parse(memory[2])
                       );

            }

            return null;

        }

        #endregion


        #region StartMonitoring()

        /// <summary>
        /// Start monitoring.
        /// </summary>
        /// <param name="Interval">The check service interval.</param>
        public void StartMonitoring(TimeSpan Interval)
        {
            if (timer is null)
            {

                CheckInterval  = Interval;

                timer          = new Timer(CheckResources,
                                           null,
                                           CheckInterval,
                                           CheckInterval);

            }
        }

        #endregion

        #region StopMonitoring()

        public void StopMonitoring()
        {
            if (timer is not null)
            {
                timer?.Dispose();
                timer = null;
            }
        }

        #endregion


    }

}
