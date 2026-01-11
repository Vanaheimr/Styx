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

using System.Threading.Channels;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Logging
{

    public sealed class SingleLogFileStreamWriter : IAsyncDisposable
    {

        private readonly  Channel<String>          channel;
        private           StreamWriter             streamWriter;
        private           DateTimeOffset           currentPeriod;

        private readonly  String                   basePath;
        private readonly  String                   extension;
        private readonly  LogRotation              rotation;
        private readonly  UInt32                   bufferSize;
        private readonly  Boolean                  autoFlush;

        private readonly  Task                     backgroundTask;
        private readonly  CancellationTokenSource  cts = new();

        public SingleLogFileStreamWriter(String       basePath,
                                         LogRotation  Rotation     = LogRotation.Monthly,
                                         Int32?       Capacity     = 1000,
                                         UInt32       BufferSize   = 65536,
                                         Boolean      AutoFlush    = false)
        {

            this.basePath        = Path.Combine(
                                       Path.GetDirectoryName(basePath) ?? ".",
                                       Path.GetFileNameWithoutExtension(basePath)
                                   );

            this.extension       = Path.GetExtension(basePath);
            this.rotation        = Rotation;
            this.bufferSize      = BufferSize;
            this.autoFlush       = AutoFlush;

            if (String.IsNullOrEmpty(extension))
                extension        = ".log";

            this.channel         = Capacity.HasValue

                                       ? Channel.CreateBounded  <String>(
                                             new BoundedChannelOptions(Capacity.Value) {
                                                 FullMode      = BoundedChannelFullMode.DropWrite,
                                                 SingleReader  = true,
                                                 SingleWriter  = false
                                             }
                                         )

                                       : Channel.CreateUnbounded<String>(
                                             new UnboundedChannelOptions {
                                                 SingleReader  = true,
                                                 SingleWriter  = false
                                             }
                                         );

            this.currentPeriod   = GetPeriodStart(Timestamp.Now);
            this.streamWriter    = CreateWriter();
            this.backgroundTask  = ProcessQueueAsync(cts.Token);

        }


        private DateTimeOffset GetPeriodStart(DateTimeOffset timestamp)

            => rotation switch {
                   LogRotation.Hourly   => new DateTimeOffset(timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, 0, 0, TimeSpan.Zero),
                   LogRotation.Daily    => new DateTimeOffset(timestamp.Year, timestamp.Month, timestamp.Day,              0, 0, 0, TimeSpan.Zero),
                   LogRotation.Monthly  => new DateTimeOffset(timestamp.Year, timestamp.Month,             1,              0, 0, 0, TimeSpan.Zero),
                   LogRotation.Yearly   => new DateTimeOffset(timestamp.Year,               1,             1,              0, 0, 0, TimeSpan.Zero),
                   _                    => DateTimeOffset.MinValue
               };

        private String GetCurrentFilePath()
        {

            var suffix = rotation switch {
                             LogRotation.Hourly   => currentPeriod.ToString("_yyyy-MM-dd_HH"),
                             LogRotation.Daily    => currentPeriod.ToString("_yyyy-MM-dd"),
                             LogRotation.Monthly  => currentPeriod.ToString("_yyyy-MM"),
                             LogRotation.Yearly   => currentPeriod.ToString("_yyyy"),
                             _                    => ""
                         };

            return $"{basePath}{suffix}{extension}";

        }

        private StreamWriter CreateWriter()
        {

            var filePath  = GetCurrentFilePath();
            var dir       = Path.GetDirectoryName(filePath);

            if (!String.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return new StreamWriter(
                       new FileStream(
                           filePath,
                           FileMode.   Append,
                           FileAccess. Write,
                           FileShare.  Read,
                           (Int32) bufferSize,
                           FileOptions.Asynchronous
                       )
                   ) {
                       AutoFlush  = autoFlush
                     };

        }

        private async ValueTask RotateIfNeededAsync(DateTimeOffset timestamp)
        {

            if (rotation == LogRotation.None)
                return;

            var newPeriod = GetPeriodStart(timestamp);

            if (newPeriod > currentPeriod)
            {

                await streamWriter.FlushAsync(CancellationToken.None);
                await streamWriter.DisposeAsync();

                currentPeriod  = newPeriod;
                streamWriter   = CreateWriter();
            }

        }











        public ValueTask LogAsync(String Message)
            => channel.Writer.WriteAsync(Message);

        //public Boolean TryLog(String message)
        //    => channel.Writer.TryWrite(message);

        //public void Log(String message)
        //{
        //    if (!channel.Writer.TryWrite(message))
        //        channel.Writer.WriteAsync(message).AsTask().Wait();
        //}

        //public ValueTask LogAsync(String Format, params Object[] args)
        //    => LogAsync(String.Format(Format, args));

        //public ValueTask LogLineAsync(String message)
        //    => LogAsync($"[{Timestamp.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");

        private async Task ProcessQueueAsync(CancellationToken ct)
        {
            try
            {
                await foreach (var message in channel.Reader.ReadAllAsync(ct))
                {
                    await RotateIfNeededAsync(Timestamp.Now);
                    await streamWriter.WriteLineAsync(message.AsMemory(), ct);
                    //await streamWriter.WriteLineAsync(message);
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // Graceful shutdown – drain remaining messages
                while (channel.Reader.TryRead(out var remaining))
                {
                    await RotateIfNeededAsync(Timestamp.Now);
                    await streamWriter.WriteLineAsync(remaining);
                }
            }
            finally
            {
                // Write any remaining buffered data
                await streamWriter.FlushAsync(CancellationToken.None);
            }
        }

        public async ValueTask FlushAsync()
        {
            await streamWriter.FlushAsync();
        }

        public String CurrentFilePath
            => GetCurrentFilePath();

        public async ValueTask DisposeAsync()
        {

            channel.Writer.Complete();

            // Wait for queue to drain (with timeout)
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            try
            {
                await backgroundTask.WaitAsync(timeoutCts.Token);
            }
            catch (TimeoutException)
            {
                await cts.CancelAsync();
                await backgroundTask;
            }

            cts.Dispose();

            await streamWriter.DisposeAsync();

        }

    }

}
