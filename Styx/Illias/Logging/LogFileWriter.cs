/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

using System.Text;
using System.Threading.Channels;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Logging
{

    /// <summary>
    /// A line a LogFileWriter took and did not write to its file.
    /// </summary>
    /// <param name="Timestamp">When the writer gave the line up.</param>
    /// <param name="Sender">The writer.</param>
    /// <param name="FileName">The file the line was for.</param>
    /// <param name="Line">The line, without its line break.</param>
    /// <param name="Exception">Why: what the file threw; an OperationCanceledException
    /// for a write that DisposeAsync cut off, which may have left part of the line in
    /// the file; a TimeoutException for a line still waiting when DisposeAsync gave up.</param>
    public delegate void OnLineNotWrittenDelegate(DateTimeOffset  Timestamp,
                                                  LogFileWriter   Sender,
                                                  String          FileName,
                                                  String          Line,
                                                  Exception       Exception);


    /// <summary>
    /// Appends lines to files off the caller's path, one line at a time and
    /// in the order they were handed over.
    /// </summary>
    /// <remarks>
    /// EnqueueAsync returns once the line is taken, not once it is written. A
    /// line that is taken and then not written - its file refused it, or
    /// DisposeAsync ran out of time before its turn came - is told to
    /// <see cref="OnLineNotWritten"/>, and to nobody else: a caller that must
    /// know whether its line is in the file writes it itself.
    /// </remarks>
    public sealed class LogFileWriter : IAsyncDisposable
    {

        #region Data

        /// <summary>
        /// How long DisposeAsync waits for the lines taken before it, unless
        /// the constructor is told otherwise.
        /// </summary>
        public static readonly TimeSpan DefaultDisposeTimeout = TimeSpan.FromSeconds(30);

        private readonly Channel<(String FileName, String Line)>         channel;
        private readonly Func<String, String, CancellationToken, Task>   appendAllTextAsync;
        private readonly CancellationTokenSource                         cts = new();
        private readonly Task                                            backgroundTask;
        private readonly Lazy<Task>                                      disposal;

        #endregion

        #region Properties

        /// <summary>
        /// How long DisposeAsync waits for the lines taken before it to be
        /// written, before it gives up on the rest.
        /// </summary>
        public TimeSpan DisposeTimeout { get; }

        #endregion

        #region Events

        /// <summary>
        /// A line was taken and not written to its file. Raised on the
        /// writer's own loop, which waits for every subscriber: keep it short.
        /// </summary>
        public event OnLineNotWrittenDelegate? OnLineNotWritten;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new writer.
        /// </summary>
        /// <param name="capacity">How many lines may wait to be written; a caller handing over one more waits in EnqueueAsync.</param>
        /// <param name="DisposeTimeout">How long DisposeAsync waits for the lines taken before it (default: <see cref="DefaultDisposeTimeout"/>).</param>
        public LogFileWriter(Int32      capacity         = 1000,
                             TimeSpan?  DisposeTimeout   = null)

            : this(capacity,
                   DisposeTimeout,
                   (fileName, text, cancellationToken) => File.AppendAllTextAsync(
                                                              fileName,
                                                              text,
                                                              Encoding.UTF8,
                                                              cancellationToken
                                                          ))

        { }

        /// <summary>
        /// Create a new writer that appends to its files the given way - for a
        /// test that needs a file whose write never ends.
        /// </summary>
        internal LogFileWriter(Int32                                           capacity,
                               TimeSpan?                                       DisposeTimeout,
                               Func<String, String, CancellationToken, Task>   AppendAllTextAsync)
        {

            this.DisposeTimeout      = DisposeTimeout ?? DefaultDisposeTimeout;
            this.appendAllTextAsync  = AppendAllTextAsync;

            this.channel             = Channel.CreateBounded<(String, String)>(
                                           new BoundedChannelOptions(capacity) {
                                               FullMode      = BoundedChannelFullMode.Wait,
                                               SingleReader  = true,
                                               SingleWriter  = false
                                           }
                                       );

            this.disposal            = new Lazy<Task>(DisposeOnceAsync);
            this.backgroundTask      = Task.Run(WriteLoopAsync);

        }

        #endregion


        #region EnqueueAsync(FileName, Line, cancellationToken = default)

        /// <summary>
        /// Hand a line over to be appended to the given file.
        /// </summary>
        /// <remarks>
        /// Returns once the line is taken. Once DisposeAsync has begun no line
        /// is taken any more: this throws a ChannelClosedException instead.
        /// </remarks>
        /// <param name="FileName">The file.</param>
        /// <param name="Line">The line, without a line break.</param>
        /// <param name="cancellationToken">An optional cancellation token, for the wait while the queue is full.</param>
        public ValueTask EnqueueAsync(String             FileName,
                                      String             Line,
                                      CancellationToken  cancellationToken   = default)

            => channel.Writer.WriteAsync(
                   (FileName, Line),
                   cancellationToken
               );

        #endregion

        #region (private) WriteLoopAsync()

        private async Task WriteLoopAsync()
        {

            // Until the queue is completed and empty: DisposeAsync completes
            // it, and the lines taken before are written all the same. Only a
            // DisposeAsync that runs out of time cancels.
            try
            {
                while (await channel.Reader.WaitToReadAsync(cts.Token))
                {
                    while (!cts.IsCancellationRequested &&
                           channel.Reader.TryRead(out var next))
                    {
                        try
                        {

                            await appendAllTextAsync(
                                      next.FileName,
                                      next.Line + Environment.NewLine,
                                      cts.Token
                                  );

                        }
                        catch (Exception e)
                        {
                            LineNotWritten(next.FileName, next.Line, e);
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested)
            { }

            // What was still waiting when DisposeAsync gave up.
            while (channel.Reader.TryRead(out var waiting))
                LineNotWritten(
                    waiting.FileName,
                    waiting.Line,
                    new TimeoutException($"Not written: the writer was disposed and gave up after {DisposeTimeout} before this line's turn came.")
                );

        }

        #endregion

        #region (private) LineNotWritten(FileName, Line, Exception)

        private void LineNotWritten(String     FileName,
                                    String     Line,
                                    Exception  Exception)
        {

            DebugX.Log($"[ERROR] Could not write to file {FileName}: {Exception.Message}");

            var timestamp = Timestamp.Now;

            foreach (var onLineNotWritten in OnLineNotWritten?.GetInvocationList().OfType<OnLineNotWrittenDelegate>() ?? [])
            {
                try
                {
                    onLineNotWritten(timestamp, this, FileName, Line, Exception);
                }
                catch
                {
                    // A subscriber's own failure costs neither the other
                    // subscribers their report nor the lines after this one.
                }
            }

        }

        #endregion

        #region DisposeAsync()

        /// <summary>
        /// Take no more lines, and wait until the lines taken before are
        /// written - at most <see cref="DisposeTimeout"/>. Then the write in
        /// progress is cut off, and it and every line still waiting are told
        /// to <see cref="OnLineNotWritten"/>. A second call waits for the first.
        /// </summary>
        public ValueTask DisposeAsync()

            => new (disposal.Value);

        private async Task DisposeOnceAsync()
        {

            channel.Writer.TryComplete();

            try
            {
                await backgroundTask.WaitAsync(DisposeTimeout).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                await cts.CancelAsync().ConfigureAwait(false);
                await backgroundTask.ConfigureAwait(false);
            }

            cts.Dispose();

        }

        #endregion

    }

}
