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

namespace org.GraphDefined.Vanaheimr.CLI
{

    /// <summary>
    /// A terminal that is nothing but bytes: what a VT100-style terminal sends
    /// when a key is pressed comes in, and what it understands goes out.
    /// </summary>
    /// <remarks>
    /// <para>
    /// What somebody signed in over SSH types at, or anything else that brings
    /// a terminal of its own at the far end of a stream. Nothing in here knows
    /// how the bytes travel: whoever has them hands them to <see cref="Feed"/>
    /// - or lets <see cref="ReadFromAsync"/> do it - and says how to send what
    /// is written. Which is also what makes it testable without a connection.
    /// </para>
    /// <para>
    /// There is no line discipline at the far end. A pseudo terminal on Unix
    /// turns "\n" into "\r\n" on its way to the screen, and nothing does that
    /// here unless this does: every "\n" written goes out as "\r\n", and
    /// a "\r\n" already there is left as it is.
    /// </para>
    /// <para>
    /// Writing is queued and sent by a task of its own, several writes at a
    /// time where several are waiting. The line editor writes while it holds
    /// the lock that every thread that logs waits on, and a far end that reads
    /// slowly - or not at all - must not hold all of them up. So the queue has
    /// a limit: a terminal that has not taken <see cref="MaxPendingOutput"/>
    /// bytes is not reading any more, and a write beyond that throws rather
    /// than growing the queue until the process runs out of memory.
    /// </para>
    /// </remarks>
    public sealed class VT100Terminal : ICLITerminal,
                                        IAsyncDisposable
    {

        #region (record struct) Outgoing

        /// <summary>
        /// Bytes to send, or a promise to keep once everything before it is sent.
        /// </summary>
        private readonly record struct Outgoing(Byte[]?                Data,
                                                TaskCompletionSource?  Sent);

        #endregion

        #region Data

        /// <summary>
        /// How long Escape waits for the rest of a sequence it might be the
        /// beginning of, by default - the 100 ms vim waits as well.
        /// </summary>
        public static readonly  TimeSpan                                            DefaultEscapeTimeout     = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// How much may wait to be sent before the far end counts as not reading,
        /// by default.
        /// </summary>
        public const            Int32                                               DefaultMaxPendingOutput  = 1024 * 1024;

        /// <summary>
        /// How much is sent at once at most, where many writes are waiting.
        /// </summary>
        private const           Int32                                               MaxChunk                 = 32 * 1024;

        private static readonly UTF8Encoding                                        utf8                     = new (encoderShouldEmitUTF8Identifier: false);

        private readonly        Func<ReadOnlyMemory<Byte>, CancellationToken, ValueTask>  send;
        private readonly        VT100KeyDecoder                                     decoder                  = new ();
        private readonly        Lock                                                decoderLock              = new ();
        private readonly        Channel<ConsoleKeyInfo>                             keys                     = Channel.CreateUnbounded<ConsoleKeyInfo>(new UnboundedChannelOptions { SingleReader = true });
        private readonly        Channel<Outgoing>                                   output                   = Channel.CreateUnbounded<Outgoing>(new UnboundedChannelOptions { SingleReader = true });
        private readonly        CancellationTokenSource                             stopping                 = new ();
        private readonly        ITimer                                              escapeTimer;
        private readonly        TimeSpan                                            escapeTimeout;
        private readonly        Task                                                sender;

        private                 Int32                                               width;
        private                 Int64                                               pending;
        private                 Exception?                                          failure;

        #endregion

        #region Properties

        /// <summary>
        /// How many columns a row has: what the far end said, at least one.
        /// Setting it says <see cref="Resized"/> where it changed.
        /// </summary>
        public Int32    Width
        {

            get
                => Volatile.Read(ref width);

            set
            {

                var now = Math.Max(1, value);

                if (Interlocked.Exchange(ref width, now) != now)
                    Resized?.Invoke();

            }

        }

        /// <summary>
        /// Yes: Ctrl+C is the byte 0x03 among the others.
        /// </summary>
        public Boolean  InterruptsArriveAsKeys
            => true;

        /// <summary>
        /// How much may wait to be sent before a write throws.
        /// </summary>
        public Int32    MaxPendingOutput    { get; }

        #endregion

        #region Events

        /// <inheritdoc />
        public event Action?  Interrupted;

        /// <inheritdoc />
        public event Action?  Resized;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A terminal whose output goes to the given sender.
        /// </summary>
        /// <param name="Send">Sends bytes to the far end. Called by one task at a time, in order.</param>
        /// <param name="Width">How many columns a row has, until somebody says otherwise.</param>
        /// <param name="EscapeTimeout">How long Escape waits for the rest of a sequence; <see cref="DefaultEscapeTimeout"/> by default.</param>
        /// <param name="MaxPendingOutput">How much may wait to be sent; <see cref="DefaultMaxPendingOutput"/> by default.</param>
        /// <param name="TimeProvider">The clock Escape waits by; the system's by default.</param>
        public VT100Terminal(Func<ReadOnlyMemory<Byte>, CancellationToken, ValueTask>  Send,
                             Int32                                                     Width             = 80,
                             TimeSpan?                                                 EscapeTimeout     = null,
                             Int32                                                     MaxPendingOutput  = DefaultMaxPendingOutput,
                             TimeProvider?                                             TimeProvider      = null)
        {

            this.send              = Send;
            this.width             = Math.Max(1, Width);
            this.escapeTimeout     = EscapeTimeout ?? DefaultEscapeTimeout;
            this.MaxPendingOutput  = MaxPendingOutput;

            this.escapeTimer       = (TimeProvider ?? TimeProvider.System).CreateTimer(_ => EscapeTimedOut(),
                                                                                       null,
                                                                                       Timeout.InfiniteTimeSpan,
                                                                                       Timeout.InfiniteTimeSpan);

            this.sender            = Task.Run(SendAsync);

        }

        #endregion


        #region Feed(Bytes)

        /// <summary>
        /// What the far end sent: the keys in it are read as they come.
        /// </summary>
        /// <param name="Bytes">The bytes, as they arrived.</param>
        public void Feed(ReadOnlySpan<Byte> Bytes)
        {
            lock (decoderLock)
            {

                foreach (var key in decoder.Feed(Bytes))
                    keys.Writer.TryWrite(key);

                escapeTimer.Change(decoder.HasPendingSequence ? escapeTimeout : Timeout.InfiniteTimeSpan,
                                   Timeout.InfiniteTimeSpan);

            }
        }

        #endregion

        #region Complete()

        /// <summary>
        /// The far end will send nothing more: the keys there are can still be
        /// read, and then <see cref="ReadKeyAsync"/> says null.
        /// </summary>
        public void Complete()
        {
            lock (decoderLock)
            {

                escapeTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

                foreach (var key in decoder.Flush())
                    keys.Writer.TryWrite(key);

                keys.Writer.TryComplete();

            }
        }

        #endregion

        #region ReadFromAsync(Input, CancellationToken)

        /// <summary>
        /// Feed everything the given stream brings, and complete when it ends.
        /// </summary>
        /// <param name="Input">What the far end sends.</param>
        /// <param name="CancellationToken">An optional token to stop reading.</param>
        public async Task ReadFromAsync(Stream             Input,
                                        CancellationToken  CancellationToken = default)
        {

            var buffer = new Byte[4096];

            try
            {

                Int32 read;

                while ((read = await Input.ReadAsync(buffer, CancellationToken).ConfigureAwait(false)) > 0)
                    Feed(buffer.AsSpan(0, read));

            }
            finally
            {
                Complete();
            }

        }

        #endregion

        #region Interrupt()

        /// <summary>
        /// The far end asked for whatever is running to stop, beside its keys -
        /// a signal rather than Ctrl+C typed.
        /// </summary>
        public void Interrupt()

            => Interrupted?.Invoke();

        #endregion

        #region ReadKeyAsync(CancellationToken)

        /// <inheritdoc />
        public async ValueTask<ConsoleKeyInfo?> ReadKeyAsync(CancellationToken CancellationToken = default)
        {

            while (await keys.Reader.WaitToReadAsync(CancellationToken).ConfigureAwait(false))
                if (keys.Reader.TryRead(out var key))
                    return key;

            return null;

        }

        #endregion

        #region Write(Text, Foreground = null)

        /// <summary>
        /// Queue the given text to be sent, in the given colour where there is
        /// one. Throws where the far end has stopped taking what it is sent.
        /// </summary>
        /// <param name="Text">What to write.</param>
        /// <param name="Foreground">The colour of the text; the terminal's own when null.</param>
        public void Write(String         Text,
                          ConsoleColor?  Foreground  = null)
        {

            if (Text.Length == 0)
                return;

            var text  = Text.Replace("\r\n", "\n").Replace("\n", "\r\n");

            if (Foreground is ConsoleColor colour)
                text  = $"\x1b[{SGR(colour)}m{text}\x1b[39m";

            var bytes = utf8.GetBytes(text);

            if (Volatile.Read(ref failure) is Exception failed)
                throw new IOException($"The terminal's far end is gone: {failed.Message}", failed);

            if (Interlocked.Add(ref pending, bytes.Length) > MaxPendingOutput)
            {
                Interlocked.Add(ref pending, -bytes.Length);
                throw new IOException($"The terminal's far end has not taken the last {MaxPendingOutput} bytes it was sent, and is not reading any more.");
            }

            if (!output.Writer.TryWrite(new Outgoing(bytes, null)))
                throw new ObjectDisposedException(nameof(VT100Terminal));

        }

        #endregion

        #region WriteLine(Text = "", Foreground = null)

        /// <inheritdoc />
        public void WriteLine(String         Text        = "",
                              ConsoleColor?  Foreground  = null)

            => Write(Text + "\n", Foreground);

        #endregion

        #region FlushAsync(CancellationToken)

        /// <summary>
        /// Wait until everything written so far has been sent - or could not be,
        /// which is thrown.
        /// </summary>
        /// <param name="CancellationToken">An optional token to stop waiting.</param>
        public async Task FlushAsync(CancellationToken CancellationToken = default)
        {

            var sent = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            if (!output.Writer.TryWrite(new Outgoing(null, sent)))
                sent.TrySetResult();

            await sent.Task.WaitAsync(CancellationToken).ConfigureAwait(false);

            if (Volatile.Read(ref failure) is Exception failed)
                throw new IOException($"The terminal's far end is gone: {failed.Message}", failed);

        }

        #endregion


        #region (private) EscapeTimedOut()

        private void EscapeTimedOut()
        {
            lock (decoderLock)
            {
                foreach (var key in decoder.Flush())
                    keys.Writer.TryWrite(key);
            }
        }

        #endregion

        #region (private) SendAsync()

        /// <summary>
        /// Send what was written, in order, as much at once as is waiting.
        /// </summary>
        private async Task SendAsync()
        {

            var chunk  = new MemoryStream();
            var kept   = new List<TaskCompletionSource>();

            try
            {
                while (await output.Reader.WaitToReadAsync(stopping.Token).ConfigureAwait(false))
                {

                    chunk.SetLength(0);
                    kept.Clear();

                    while (chunk.Length < MaxChunk && output.Reader.TryRead(out var next))
                    {

                        if (next.Data is not null)
                            chunk.Write(next.Data);

                        if (next.Sent is not null)
                            kept.Add(next.Sent);

                    }

                    if (chunk.Length > 0 && Volatile.Read(ref failure) is null)
                    {
                        try
                        {
                            await send(chunk.GetBuffer().AsMemory(0, (Int32) chunk.Length), stopping.Token).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            Volatile.Write(ref failure, e);
                        }
                    }

                    Interlocked.Add(ref pending, -chunk.Length);

                    foreach (var sent in kept)
                        sent.TrySetResult();

                }
            }
            catch (OperationCanceledException)
            { }
            finally
            {
                // Whoever waits for a flush is not left waiting by a terminal
                // that stopped sending.
                while (output.Reader.TryRead(out var left))
                    left.Sent?.TrySetResult();
            }

        }

        #endregion

        #region (private static) SGR(Colour)

        /// <summary>
        /// The foreground colour as a terminal is told it: the sixteen colours of
        /// the console, the bright ones as 90 to 97.
        /// </summary>
        private static Int32 SGR(ConsoleColor Colour)

            => Colour switch {
                   ConsoleColor.Black        => 30,
                   ConsoleColor.DarkRed      => 31,
                   ConsoleColor.DarkGreen    => 32,
                   ConsoleColor.DarkYellow   => 33,
                   ConsoleColor.DarkBlue     => 34,
                   ConsoleColor.DarkMagenta  => 35,
                   ConsoleColor.DarkCyan     => 36,
                   ConsoleColor.Gray         => 37,
                   ConsoleColor.DarkGray     => 90,
                   ConsoleColor.Red          => 91,
                   ConsoleColor.Green        => 92,
                   ConsoleColor.Yellow       => 93,
                   ConsoleColor.Blue         => 94,
                   ConsoleColor.Magenta      => 95,
                   ConsoleColor.Cyan         => 96,
                   _                         => 97
               };

        #endregion

        #region DisposeAsync()

        /// <summary>
        /// Send what is still waiting, for a second at most, then stop: no key
        /// is read and nothing is written after this.
        /// </summary>
        /// <remarks>
        /// A second, because a far end that takes nothing would otherwise keep
        /// whoever lets go of its terminal waiting for ever. Somebody who has to
        /// know that the last words arrived says <see cref="FlushAsync"/> first,
        /// with as much patience as they have.
        /// </remarks>
        public async ValueTask DisposeAsync()
        {

            Complete();

            output.Writer.TryComplete();

            try
            {
                await sender.WaitAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {

                await stopping.CancelAsync().ConfigureAwait(false);

                try
                {
                    await sender.ConfigureAwait(false);
                }
                catch
                { }

            }
            catch
            { }

            stopping.Dispose();
            await escapeTimer.DisposeAsync().ConfigureAwait(false);

        }

        #endregion

    }

}
