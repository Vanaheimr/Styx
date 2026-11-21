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

using System.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An entry of a log: a value, and when it was logged.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="Position">Where the entry stands in its log, counted from 0 - for ever: positions are not reused when entries are cleared or dropped.</param>
    /// <param name="Timestamp">When the entry was logged.</param>
    /// <param name="Value">The value logged.</param>
    public sealed record LogEntry<T>(Int64           Position,
                                     DateTimeOffset  Timestamp,
                                     T               Value)
    {

        public override String ToString()

            => $"{Position}: {Timestamp:o} {Value}";

    }


    /// <summary>
    /// A log of texts, only ever appended to: entries are added without a lock and read without a copy.
    /// </summary>
    /// <param name="MaxEntries">How many entries are kept at most: the oldest are dropped. All by default.</param>
    /// <param name="TimeProvider">The clock the entries are stamped and waited by; Timestamp.Provider by default, time travel included.</param>
    public class ConcurrentLog(Int32?         MaxEntries     = null,
                               TimeProvider?  TimeProvider   = null)

        : ConcurrentLog<String>(MaxEntries,
                                TimeProvider)

    { }


    /// <summary>
    /// A log, only ever appended to: entries are added without a lock and read without a copy - every
    /// reader sees the log without a gap, from its oldest entry kept up to one, and LINQ runs over it as it is.
    /// </summary>
    /// <remarks>
    /// The entries stand in chunks of 1024 places. Adding one takes a place by an interlocked increment and
    /// writes it; only every 1024 entries a new chunk is made, under a lock. A place taken but not yet
    /// written is empty, and a reader stops before it: it sees what is written without a gap.
    ///
    /// Positions go on for ever. Clearing the log, and keeping it to its most entries, moves where it
    /// starts; whole chunks before that are let go, so that a log of at most n entries holds n and at most
    /// two chunks more.
    ///
    /// The entries stand in the order their places were taken. Their timestamps are taken just before, so
    /// two entries added at the same time by two threads may stand in the other order than their timestamps.
    /// </remarks>
    /// <typeparam name="T">The type of the values logged.</typeparam>
    public class ConcurrentLog<T> : IEnumerable<LogEntry<T>>
    {

        #region Data

        private const    Int32                    Shift       = 10;            // 1024 entries per chunk
        private const    Int32                    ChunkSize   = 1 << Shift;
        private const    Int32                    Mask        = ChunkSize - 1;

        /// <summary>
        /// The chunks from the chunk numbered First on. Replaced under the lock, by a bigger one or by one
        /// that lets go of chunks before the start - and only then, after the start was moved.
        /// </summary>
        private sealed class Directory(Int64                First,
                                       LogEntry<T>?[]?[]    Chunks)
        {
            public Int64              First    { get; } = First;
            public LogEntry<T>?[]?[]  Chunks   { get; } = Chunks;
        }

        private          Directory                directory   = new (0, new LogEntry<T>?[]?[16]);

        /// <summary>
        /// The number of places taken: the position of the next entry.
        /// </summary>
        private          Int64                    taken;

        /// <summary>
        /// A position all places below are written at - or before the start, perhaps more.
        /// </summary>
        private          Int64                    written;

        /// <summary>
        /// Where the log starts: what stands before is cleared or dropped. Only ever moved forward.
        /// </summary>
        private          Int64                    start;

        /// <summary>
        /// What those who wait are woken by when an entry is added: null while nobody waits.
        /// </summary>
        private          TaskCompletionSource?    added;

        private readonly Lock                     chunkLock   = new();

        #endregion

        #region Properties

        /// <summary>
        /// How many entries are kept at most: the oldest are dropped. Null for all.
        /// </summary>
        public Int32?        MaxEntries     { get; }

        /// <summary>
        /// The clock the entries are stamped and waited by.
        /// </summary>
        public TimeProvider  TimeProvider   { get; }

        /// <summary>
        /// The number of entries kept and written: one still being written by another thread is not
        /// counted yet, nor what stands behind it.
        /// </summary>
        public Int64 Count
        {
            get
            {
                var (start, end, _) = Snapshot();
                return end - start;
            }
        }

        /// <summary>
        /// The position of the oldest entry kept.
        /// </summary>
        public Int64 Start
            => Snapshot().Start;

        /// <summary>
        /// The position after the last entry written: where to wait from for what comes next.
        /// </summary>
        public Int64 End
            => Snapshot().End;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new log.
        /// </summary>
        /// <param name="MaxEntries">How many entries are kept at most: the oldest are dropped. All by default.</param>
        /// <param name="TimeProvider">The clock the entries are stamped and waited by; Timestamp.Provider by default, time travel included.</param>
        public ConcurrentLog(Int32?         MaxEntries     = null,
                             TimeProvider?  TimeProvider   = null)
        {

            if (MaxEntries <= 0)
                throw new ArgumentOutOfRangeException(nameof(MaxEntries), MaxEntries, "A log keeps at least one entry.");

            this.MaxEntries    = MaxEntries;
            this.TimeProvider  = TimeProvider ?? Timestamp.Provider;

        }

        #endregion


        #region Add(Value)

        /// <summary>
        /// Add the given value, stamped with the time of the log's clock.
        /// </summary>
        /// <param name="Value">A value.</param>
        /// <returns>The entry added.</returns>
        public LogEntry<T> Add(T Value)

            => Add(Value, TimeProvider.GetUtcNow());

        #endregion

        #region Add(Value, Timestamp)

        /// <summary>
        /// Add the given value, stamped with the given time - e.g. that of a message it tells of.
        /// </summary>
        /// <param name="Value">A value.</param>
        /// <param name="Timestamp">When the value is logged.</param>
        /// <returns>The entry added.</returns>
        public LogEntry<T> Add(T               Value,
                               DateTimeOffset  Timestamp)
        {

            var position  = Interlocked.Increment(ref taken) - 1;
            var entry     = new LogEntry<T>(position, Timestamp, Value);

            // No chunk when the log was cleared meanwhile, past this place: the entry is gone already.
            var chunk     = ChunkOf(position >> Shift);

            if (chunk is not null)
                Volatile.Write(ref chunk[position & Mask], entry);

            // A full fence before looking for those who wait: one who began to wait before is seen here,
            // and one who begins after sees what was written above. Only when somebody waits, the
            // exchange writes - so that adding alone leaves the field untouched for the other threads.
            Interlocked.MemoryBarrier();

            if (Volatile.Read(ref added) is not null)
                Interlocked.Exchange(ref added, null)?.TrySetResult();

            return entry;

        }

        #endregion

        #region (private) ChunkOf(Number)

        /// <summary>
        /// The chunk of the given number - made when there is none yet, null when it was let go of.
        /// </summary>
        private LogEntry<T>?[]? ChunkOf(Int64 Number)
        {

            var current  = Volatile.Read(ref directory);
            var index    = Number - current.First;

            // Nearly always: the chunk is there.
            if (index >= 0 && index < current.Chunks.Length && Volatile.Read(ref current.Chunks[index]) is { } chunk)
                return chunk;

            // Once per 1024 entries.
            lock (chunkLock)
            {

                // Before a new chunk, those that are too old to be kept are let go of.
                if (MaxEntries.HasValue)
                    LetGoBefore(Snapshot().End - MaxEntries.Value);

                current  = directory;
                index    = Number - current.First;

                if (index < 0)
                    return null;

                if (index >= current.Chunks.Length)
                {

                    var bigger = new LogEntry<T>?[]?[Math.Max(current.Chunks.Length * 2, index + 1)];

                    current.Chunks.CopyTo(bigger, 0);

                    current = new Directory(current.First, bigger);
                    Volatile.Write(ref directory, current);

                }

                if (current.Chunks[index] is null)
                    Volatile.Write(ref current.Chunks[index], new LogEntry<T>?[ChunkSize]);

                return current.Chunks[index];

            }

        }

        #endregion

        #region (private) LetGoBefore(Position)

        /// <summary>
        /// Move the start to the given position, unless it is there already, and let go of the chunks
        /// that stand all before it. Under the lock.
        /// </summary>
        /// <param name="Position">Where the log starts from now on.</param>
        private void LetGoBefore(Int64 Position)
        {

            // The start first, the chunks after: one who reads the directory reads a start that is not
            // before its first chunk.
            Int64 seen;
            while ((seen = Volatile.Read(ref start)) < Position)
                if (Interlocked.CompareExchange(ref start, Position, seen) == seen)
                    break;

            var current  = directory;
            var first    = Volatile.Read(ref start) >> Shift;

            if (first <= current.First)
                return;

            // The chunks kept: from the first on, up to that of the next place - not the whole directory,
            // or it would grow with every clearing.
            var last     = Math.Min(Volatile.Read(ref taken) >> Shift, current.First + current.Chunks.Length - 1);
            var kept     = Math.Max(last - first + 1, 0);
            var chunks   = new LogEntry<T>?[]?[Math.Max(16, kept * 2)];

            if (kept > 0)
                Array.Copy(current.Chunks, first - current.First, chunks, 0, kept);

            Volatile.Write(ref directory, new Directory(first, chunks));

        }

        #endregion

        #region (private) Snapshot()

        /// <summary>
        /// The log as it is now: where it starts, where it ends - all places in between written - and
        /// the directory to read them in.
        /// </summary>
        private (Int64 Start, Int64 End, Directory Directory) Snapshot()
        {

            // In this order: the chunks of all places below what is read as written are in the directory
            // read after it, but for those let go of before the start read after that.
            var end      = Volatile.Read(ref written);
            var current  = Volatile.Read(ref directory);
            var first    = Volatile.Read(ref start);
            var placed   = Volatile.Read(ref taken);

            end = Math.Max(end, first);

            while (end < placed)
            {

                var index = (end >> Shift) - current.First;

                if (index >= current.Chunks.Length                                ||
                    Volatile.Read(ref current.Chunks[index]) is not { } chunk     ||
                    Volatile.Read(ref chunk[end & Mask])     is null)
                {
                    break;
                }

                end++;

            }

            // Only ever forward: another reader may have counted further meanwhile.
            Int64 seen;
            while ((seen = Volatile.Read(ref written)) < end)
                if (Interlocked.CompareExchange(ref written, end, seen) == seen)
                    break;

            // At most the most entries, though their chunks are let go of only with the next one.
            if (MaxEntries.HasValue)
                first = Math.Max(first, end - MaxEntries.Value);

            return (first, end, current);

        }

        #endregion


        #region Clear()

        /// <summary>
        /// Clear the log: all entries added so far are gone - also those still being written by another
        /// thread. Positions go on where they were.
        /// </summary>
        public void Clear()
        {
            lock (chunkLock)
            {
                LetGoBefore(Volatile.Read(ref taken));
            }
        }

        #endregion


        #region this[Position]

        /// <summary>
        /// The entry at the given position.
        /// </summary>
        /// <param name="Position">A position, counted from 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">No entry is kept or written there.</exception>
        public LogEntry<T> this[Int64 Position]
        {
            get
            {

                var (start, end, current) = Snapshot();

                return Position >= start && Position < end
                           ? current.Chunks[(Position >> Shift) - current.First]![Position & Mask]!
                           : throw new ArgumentOutOfRangeException(nameof(Position));

            }
        }

        #endregion

        #region From(Position)

        /// <summary>
        /// The entries kept from the given position on, up to the last one written when the walk begins:
        /// what is added meanwhile is not seen.
        /// </summary>
        /// <param name="Position">A position, counted from 0.</param>
        public IEnumerable<LogEntry<T>> From(Int64 Position)
        {

            var (start, end, current) = Snapshot();

            for (var position = Math.Max(Position, start); position < end; position++)
                yield return current.Chunks[(position >> Shift) - current.First]![position & Mask]!;

        }

        #endregion

        #region Values

        /// <summary>
        /// The values kept, up to the last one written when the walk begins.
        /// </summary>
        public IEnumerable<T> Values

            => From(0).Select(entry => entry.Value);

        #endregion


        #region WaitFor(Match, Timeout, From = 0, CancellationToken = default)

        /// <summary>
        /// Wait for an entry that matches: the first one from the given position on, kept already or
        /// added within the given time.
        /// </summary>
        /// <param name="Match">Whether an entry is the one waited for.</param>
        /// <param name="Timeout">How long to wait, by the log's clock; Timeout.InfiniteTimeSpan for ever.</param>
        /// <param name="From">From where on to look: e.g. the End before something was done.</param>
        /// <param name="CancellationToken">An optional cancellation token.</param>
        /// <returns>The entry, or null when none came within the given time.</returns>
        public async Task<LogEntry<T>?> WaitFor(Func<LogEntry<T>, Boolean>  Match,
                                                TimeSpan                    Timeout,
                                                Int64                       From                = 0,
                                                CancellationToken           CancellationToken   = default)
        {

            using var timeout  = new CancellationTokenSource(Timeout, TimeProvider);
            using var linked   = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, CancellationToken);

            var position = From;

            while (true)
            {

                // Be woken before looking: an entry added after the look wakes this.
                var wakeUp = WakeUp();

                foreach (var entry in this.From(position))
                {

                    if (Match(entry))
                        return entry;

                    position = entry.Position + 1;

                }

                try
                {
                    await wakeUp.WaitAsync(linked.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (timeout.IsCancellationRequested && !CancellationToken.IsCancellationRequested)
                {
                    return null;
                }

            }

        }

        #endregion

        #region WaitFor(Count, Timeout, Match = null, From = 0, CancellationToken = default)

        /// <summary>
        /// Wait for the given number of entries that match: from the given position on, kept already or
        /// added within the given time.
        /// </summary>
        /// <param name="Count">How many entries are waited for.</param>
        /// <param name="Timeout">How long to wait, by the log's clock; Timeout.InfiniteTimeSpan for ever.</param>
        /// <param name="Match">Whether an entry counts; every entry does by default.</param>
        /// <param name="From">From where on to look: e.g. the End before something was done.</param>
        /// <param name="CancellationToken">An optional cancellation token.</param>
        /// <returns>The entries, in their order - fewer than waited for when they did not come within the given time.</returns>
        public async Task<IReadOnlyList<LogEntry<T>>> WaitFor(Int32                        Count,
                                                              TimeSpan                     Timeout,
                                                              Func<LogEntry<T>, Boolean>?  Match               = null,
                                                              Int64                        From                = 0,
                                                              CancellationToken            CancellationToken   = default)
        {

            var found = new List<LogEntry<T>>();

            if (Count <= 0)
                return found;

            using var timeout  = new CancellationTokenSource(Timeout, TimeProvider);
            using var linked   = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, CancellationToken);

            var position = From;

            while (true)
            {

                var wakeUp = WakeUp();

                foreach (var entry in this.From(position))
                {

                    if (Match is null || Match(entry))
                    {
                        found.Add(entry);
                        if (found.Count == Count)
                            return found;
                    }

                    position = entry.Position + 1;

                }

                try
                {
                    await wakeUp.WaitAsync(linked.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (timeout.IsCancellationRequested && !CancellationToken.IsCancellationRequested)
                {
                    return found;
                }

            }

        }

        #endregion

        #region (private) WakeUp()

        /// <summary>
        /// What the next entry added wakes.
        /// </summary>
        private Task WakeUp()
        {

            var current = Volatile.Read(ref added);

            if (current is null)
            {
                var fresh = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                current   = Interlocked.CompareExchange(ref added, fresh, null) ?? fresh;
            }

            // A full fence: what is looked at next was written before an entry added after this.
            Interlocked.MemoryBarrier();

            return current.Task;

        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Walk through the entries kept, up to the last one written when the walk begins.
        /// </summary>
        public IEnumerator<LogEntry<T>> GetEnumerator()

            => From(0).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()

            => GetEnumerator();

        #endregion

    }

}
