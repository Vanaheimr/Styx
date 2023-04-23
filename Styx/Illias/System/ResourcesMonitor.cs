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


    public delegate void MemoryMonitorHandler   (ResourcesMonitor  Sender,
                                                 MemoryMetrics     MemoryMetrics);

    public delegate void DiskSpaceMonitorHandler(ResourcesMonitor  Sender,
                                                 Double            FreeSpacePercentage);

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

        private          Timer?                              timer;
        private          TimeSpan                            interval;
        private readonly Double                              FreeMemoryThreshold;
        private readonly IEnumerable<Tuple<String, Double>>  PathAndFreeSpaceThresholds;

        #endregion

        #region Events

        public event MemoryMonitorHandler?    LowMemory;
        public event DiskSpaceMonitorHandler? LowDiskSpace;

        #endregion

        #region Constructor(s)

        // Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory)
        public ResourcesMonitor(Double                              FreeMemoryThreshold,
                                IEnumerable<Tuple<String, Double>>  PathAndFreeSpaceThresholds,
                                TimeSpan?                           Interval   = null)
        {

            this.FreeMemoryThreshold         = FreeMemoryThreshold;
            this.PathAndFreeSpaceThresholds  = PathAndFreeSpaceThresholds.Distinct();

            if (Interval.HasValue)
                StartMonitoring(Interval.Value);

        }

        #endregion


        #region StartMonitoring()

        public void StartMonitoring(TimeSpan Interval)
        {
            this.interval  = Interval;
            this.timer     = new Timer(CheckResources, null, TimeSpan.Zero, Interval);
        }

        #endregion

        #region StopMonitoring()

        public void StopMonitoring()
        {
            this.timer?.Dispose();
        }

        #endregion

        #region (private, timer) CheckResources(State)

        private void CheckResources(Object? State)
        {

            // Free memory in percent
            var memoryMetrics = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                                RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                                    ? GetUnixMetrics()
                                    : GetWindowsMetrics();

            if (memoryMetrics is not null &&
                memoryMetrics.FreePercentage < FreeMemoryThreshold)
            {
                LowMemory?.Invoke(this, memoryMetrics);
            }


            // Free space in percent
            foreach (var pathAndFreeSpaceThreshold in PathAndFreeSpaceThresholds)
            {

                var driveInfo            = new DriveInfo(pathAndFreeSpaceThreshold.Item1);
                var freeSpacePercentage  = (Double) driveInfo.AvailableFreeSpace / driveInfo.TotalSize * 100;

                if (freeSpacePercentage < pathAndFreeSpaceThreshold.Item2)
                    LowDiskSpace?.Invoke(this, freeSpacePercentage);

            }

        }

        #endregion


        #region (private) GetWindowsMetrics()

        private MemoryMetrics? GetWindowsMetrics()
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

        #region (private) GetUnixMetrics()

        private MemoryMetrics? GetUnixMetrics()
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

    }

}
