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

#region Usings

using System.Diagnostics;
using System.Runtime.InteropServices;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public delegate Task RAMUsageMonitorHandler        (ResourcesMonitor  Sender,
                                                        DateTimeOffset    Timestamp,
                                                        UInt64            RAMUsageByOS,
                                                        UInt64            RAMUsageShared,
                                                        UInt64            RAMUsagePrivate,
                                                        UInt64            RAMUsagePrivateThreshold);

    public delegate Task FreeSystemMemoryMonitorHandler(ResourcesMonitor  Sender,
                                                        DateTimeOffset    Timestamp,
                                                        MemoryMetrics     MemoryMetrics,
                                                        Double            FreeSystemMemoryThreshold);

    public delegate Task DiskSpaceMonitorHandler       (ResourcesMonitor  Sender,
                                                        DateTimeOffset    Timestamp,
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

        #region MemoryMetrics(Total, Used, Free)

        public MemoryMetrics(Double  Total,
                             Double  Used,
                             Double  Free)
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
        public TimeSpan                            CheckInterval                     { get; set; }

        /// <summary>
        /// The current process.
        /// </summary>
        public Process                             Process                           { get; }


        /// <summary>
        /// The threshold in MBytes of RAM used before the OnHighRAMUsage event will be triggered.
        /// </summary>
        public UInt64                              HighPrivateRAMUsageThreshold      { get; }

        /// <summary>
        /// The threshold in % of free RAM left before the OnLowSystemMemory event will be triggered.
        /// </summary>
        public Double                              FreeSystemMemoryThreshold         { get; }

        /// <summary>
        /// The threshold in % of free hard disk space left before the OnLowDiskSpace event will be triggered.
        /// </summary>
        public IEnumerable<Tuple<String, Double>>  PathAndFreeDiscSpaceThresholds    { get; }



        /// <summary>
        /// The latest current RAM usage as reported by the operating system.
        /// </summary>
        public UInt64                              CurrentRAMUsageByOS               { get; private set; }

        /// <summary>
        /// The latest current shared RAM usage.
        /// </summary>
        public UInt64                              CurrentSharedRAMUsage             { get; private set; }

        /// <summary>
        /// The latest current private RAM usage.
        /// </summary>
        public UInt64                              CurrentPrivateRAMUsage            { get; private set; }

        /// <summary>
        /// The latest free system memory metrics.
        /// </summary>
        public MemoryMetrics?                      FreeSystemMemoryMetrics           { get; private set; }

        /// <summary>
        /// The latest free disk space percentage.
        /// </summary>
        public Double                              FreeDiscSpacePercentage           { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// An event called whenever the current RAM usage was reported.
        /// </summary>
        public event RAMUsageMonitorHandler?          OnCurrentRAMUsage;

        /// <summary>
        /// An event called whenever the current RAM usage is above its threshold.
        /// </summary>
        public event RAMUsageMonitorHandler?          OnHighRAMUsage;


        /// <summary>
        /// An event called whenever the current free system memory was reported.
        /// </summary>
        public event FreeSystemMemoryMonitorHandler?  OnSystemMemory;

        /// <summary>
        /// An event called whenever the current free system memory is below its threshold.
        /// </summary>
        public event FreeSystemMemoryMonitorHandler?  OnLowSystemMemory;


        /// <summary>
        /// An event called whenever the current free disk space was reported.
        /// </summary>
        public event DiskSpaceMonitorHandler?      OnDiskSpace;

        /// <summary>
        /// An event called whenever the current free disk space is below its threshold.
        /// </summary>
        public event DiskSpaceMonitorHandler?      OnLowDiskSpace;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new resources monitor.
        /// </summary>
        /// <param name="HighPrivateRAMUsageThreshold">An threshold in MBytes of private RAM used before the OnHighRAMUsage event will be triggered.</param>
        /// <param name="FreeSystemMemoryThreshold">An threshold in % of free RAM left before the OnLowSystemMemory event will be triggered.</param>
        /// <param name="PathAndFreeSpaceThresholds">An threshold in % of free hard disk space left before the OnLowDiskSpace event will be triggered.</param>
        /// <param name="CheckInterval">An optional checking interval.</param>
        public ResourcesMonitor(UInt64                              HighPrivateRAMUsageThreshold,
                                Double                              FreeSystemMemoryThreshold,
                                IEnumerable<Tuple<String, Double>>  PathAndFreeSpaceThresholds,
                                TimeSpan?                           CheckInterval   = null)
        {

            this.CheckInterval                   = CheckInterval ?? TimeSpan.FromMinutes(1);

            this.Process                         = Process.GetCurrentProcess();

            this.HighPrivateRAMUsageThreshold    = HighPrivateRAMUsageThreshold;
            this.FreeSystemMemoryThreshold       = FreeSystemMemoryThreshold;
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

            Process.Refresh();

            CurrentRAMUsageByOS     = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                                      RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                                          ? GetRAMUsageOnUnix()
                                          : GetRAMUsageOnWindows();

            CurrentSharedRAMUsage   = (UInt64) Process.WorkingSet64        / (1024 * 1024);
            CurrentPrivateRAMUsage  = (UInt64) Process.PrivateMemorySize64 / (1024 * 1024);

            // High RAM usage
            if (CurrentSharedRAMUsage > HighPrivateRAMUsageThreshold)
            {

                var onHighRAMUsage = OnHighRAMUsage?.GetInvocationList()?.Cast<RAMUsageMonitorHandler>()
                                         ?? Array.Empty<RAMUsageMonitorHandler>();

                if (onHighRAMUsage.Any())
                    await Task.WhenAll(onHighRAMUsage.
                                       Select(async ramUsageMonitorHandler => {
                                           try
                                           {
                                               await ramUsageMonitorHandler(this,
                                                                            now,
                                                                            CurrentRAMUsageByOS,
                                                                            CurrentSharedRAMUsage,
                                                                            CurrentPrivateRAMUsage,
                                                                            HighPrivateRAMUsageThreshold);
                                           }
                                           catch
                                           { }
                                       })).
                                       ConfigureAwait(false);

            }

            // Current RAM usage
            var onCurrentRAMUsage = OnCurrentRAMUsage?.GetInvocationList()?.Cast<RAMUsageMonitorHandler>()
                                        ?? Array.Empty<RAMUsageMonitorHandler>();

            if (onCurrentRAMUsage.Any())
                await Task.WhenAll(onCurrentRAMUsage.
                                   Select(async ramUsageMonitorHandler => {
                                       try
                                       {
                                           await ramUsageMonitorHandler(this,
                                                                        now,
                                                                        CurrentRAMUsageByOS,
                                                                        CurrentSharedRAMUsage,
                                                                        CurrentPrivateRAMUsage,
                                                                        HighPrivateRAMUsageThreshold);
                                       }
                                       catch
                                       { }
                                   })).
                                   ConfigureAwait(false);

            #endregion

            #region Free system memory        (%)

            FreeSystemMemoryMetrics = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                                      RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                                          ? GetMemoryMetricsOnUnix()
                                          : GetMemoryMetricsOnWindows();

            if (FreeSystemMemoryMetrics is not null)
            {

                var onFreeSystemMemory = OnSystemMemory?.GetInvocationList()?.Cast<FreeSystemMemoryMonitorHandler>()
                                             ?? Array.Empty<FreeSystemMemoryMonitorHandler>();

                if (onFreeSystemMemory.Any())
                    await Task.WhenAll(onFreeSystemMemory.
                                       Select(async freeSystemMemoryMonitorHandler => {
                                           try
                                           {
                                               await freeSystemMemoryMonitorHandler(this,
                                                                                    now,
                                                                                    FreeSystemMemoryMetrics,
                                                                                    FreeSystemMemoryThreshold);
                                           }
                                           catch
                                           { }
                                       })).
                                       ConfigureAwait(false);


                if (FreeSystemMemoryMetrics.FreePercentage < FreeSystemMemoryThreshold)
                {

                    var onLowSystemMemory = OnLowSystemMemory?.GetInvocationList()?.Cast<FreeSystemMemoryMonitorHandler>()
                                                 ?? Array.Empty<FreeSystemMemoryMonitorHandler>();

                    if (onLowSystemMemory.Any())
                        await Task.WhenAll(onLowSystemMemory.
                                           Select(async freeSystemMemoryMonitorHandler => {
                                               try
                                               {
                                                   await freeSystemMemoryMonitorHandler(this,
                                                                                        now,
                                                                                        FreeSystemMemoryMetrics,
                                                                                        FreeSystemMemoryThreshold);
                                               }
                                               catch
                                               { }
                                           })).
                                           ConfigureAwait(false);

                }

            }

            #endregion

            #region Free disk space           (%)

            foreach (var pathAndFreeDiscSpaceThreshold in PathAndFreeDiscSpaceThresholds)
            {

                var driveInfo            = new DriveInfo(pathAndFreeDiscSpaceThreshold.Item1);
                FreeDiscSpacePercentage  = (Double) driveInfo.AvailableFreeSpace / driveInfo.TotalSize * 100;


                var onDiskSpace = OnDiskSpace?.GetInvocationList()?.Cast<DiskSpaceMonitorHandler>()
                                      ?? Array.Empty<DiskSpaceMonitorHandler>();

                if (onDiskSpace.Any())
                    await Task.WhenAll(onDiskSpace.
                                       Select(async diskSpaceMonitorHandler => {
                                           try
                                           {
                                               await diskSpaceMonitorHandler(this,
                                                                             now,
                                                                             FreeDiscSpacePercentage);
                                           }
                                           catch
                                           { }
                                       })).
                                       ConfigureAwait(false);


                if (FreeDiscSpacePercentage < pathAndFreeDiscSpaceThreshold.Item2)
                {

                    var onLowDiskSpace = OnLowDiskSpace?.GetInvocationList()?.Cast<DiskSpaceMonitorHandler>()
                                             ?? Array.Empty<DiskSpaceMonitorHandler>();

                    if (onLowDiskSpace.Any())
                        await Task.WhenAll(onLowDiskSpace.
                                           Select(async diskSpaceMonitorHandler => {
                                               try
                                               {
                                                   await diskSpaceMonitorHandler(this,
                                                                                 now,
                                                                                 FreeDiscSpacePercentage);
                                               }
                                               catch
                                               { }
                                           })).
                                           ConfigureAwait(false);

                }

            }

            #endregion

        }

        #endregion


        #region (private) GetRAMUsageOnWindows()

        /// <summary>
        /// Get RAM usage of this process on Windows.
        /// </summary>
        private UInt64 GetRAMUsageOnWindows()
{

            var processInfo  = new ProcessStartInfo("cmd.exe", $"/c wmic process where processid='{Process.Id}' get WorkingSetSize") {
                                   RedirectStandardOutput  = true,
                                   UseShellExecute         = false,
                                   CreateNoWindow          = true
                               };

            using (var process = Process.Start(processInfo))
            {

                var output = process?.StandardOutput.ReadToEnd()?.Trim()?.Split("\n")
                                 ?? Array.Empty<String>();

                if (output.Length >= 2 && UInt64.TryParse(output[1].Trim(), out var ramUsage))
                    return ramUsage;

            }

            return 0;

        }

        #endregion

        #region (private) GetRAMUsageOnUnix()

        /// <summary>
        /// Get RAM usage of this process on UNIX.
        /// </summary>
        private UInt64 GetRAMUsageOnUnix()
        {

            var cmd          = $"ps -o rss= -p {Process.Id} |awk '{{print $1/1024}}'";

            var processInfo  = new ProcessStartInfo("/bin/bash", $"-c \"{cmd}\"") {
                                   RedirectStandardOutput  = true,
                                   UseShellExecute         = false,
                                   CreateNoWindow          = true
                               };

            using (var process = Process.Start(processInfo))
            {

                var output = process?.StandardOutput.ReadToEnd()?.Trim()?.Split("\n")
                                 ?? Array.Empty<String>();

                if (output.Length >= 1 && Double.TryParse(output[0], out var ramUsage))
                    return (UInt64) ramUsage;

            }

            return 0;

        }

        #endregion


        #region (static) GetMemoryMetricsOnWindows()

        /// <summary>
        /// Get memory metrics on Windows.
        /// </summary>
        public static MemoryMetrics? GetMemoryMetricsOnWindows()
        {

            var processInfo = new ProcessStartInfo {
                                  FileName                = "wmic",
                                  Arguments               = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
                                  RedirectStandardOutput  = true
                              };

            using (var process = Process.Start(processInfo))
            {

                var output = process?.StandardOutput.ReadToEnd()?.Trim()?.Split("\n")
                                 ?? Array.Empty<String>();

                if (output.Length == 2)
                {

                    var freeMemoryParts   = output[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
                    var totalMemoryParts  = output[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

                    return new MemoryMetrics(
                               Total: Math.Round(Double.Parse(totalMemoryParts[1]) / 1024, 0),
                               Free:  Math.Round(Double.Parse(freeMemoryParts[1])  / 1024, 0)
                           );

                }

            }

            return null;

        }

        #endregion

        #region (static) GetMemoryMetricsOnUnix()

        /// <summary>
        /// Get memory metrics on UNIX.
        /// </summary>
        public static MemoryMetrics? GetMemoryMetricsOnUnix()
        {

            var cmd          = "free -m";

            var processInfo  = new ProcessStartInfo("/bin/bash", $"-c \"{cmd}\"") {
                                   RedirectStandardOutput  = true,
                                   UseShellExecute         = false,
                                   CreateNoWindow          = true
                               };

            using (var process = Process.Start(processInfo))
            {

                var output = process?.StandardOutput.ReadToEnd()?.Trim()?.Split("\n")
                                 ?? Array.Empty<String>();

                if (output.Length == 2)
                {

                    var memory = output[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    if (memory.Length >= 4)
                        return new MemoryMetrics(
                                   Total: Double.Parse(memory[1]),
                                   Used:  Double.Parse(memory[2]),
                                   Free:  Double.Parse(memory[3])
                               );

                }

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
