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

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// ConcurrentLog tests: a log only ever appended to, stamped by its clock, waited for.
    /// </summary>
    [TestFixture]
    public class ConcurrentLog_Tests
    {

        #region Data

        /// <summary>
        /// A clock that stands still, and whose timers go off only when told to.
        /// </summary>
        private sealed class StandingClock(DateTimeOffset Now) : TimeProvider
        {

            private readonly List<(TimerCallback Callback, Object? State)> timers = [];

            public override DateTimeOffset GetUtcNow()
                => Now;

            public override ITimer CreateTimer(TimerCallback Callback, Object? State, TimeSpan DueTime, TimeSpan Period)
            {
                lock (timers)
                    timers.Add((Callback, State));
                return new NoTimer();
            }

            /// <summary>
            /// Every timer goes off.
            /// </summary>
            public void Expire()
            {
                (TimerCallback Callback, Object? State)[] expired;
                lock (timers)
                    expired = [.. timers];
                foreach (var (callback, state) in expired)
                    callback(state);
            }

            private sealed class NoTimer : ITimer
            {
                public Boolean   Change(TimeSpan DueTime, TimeSpan Period) => true;
                public void      Dispose() { }
                public ValueTask DisposeAsync() => ValueTask.CompletedTask;
            }

        }

        private static readonly DateTimeOffset  noon     = new (2026, 11, 21, 12, 0, 0, TimeSpan.Zero);

        private static readonly TimeSpan        aWhile   = TimeSpan.FromSeconds(10);

        #endregion


        #region EntriesAreStampedByTheClockOfTheLog()

        [Test]
        public void EntriesAreStampedByTheClockOfTheLog()
        {

            var log     = new ConcurrentLog(TimeProvider: new StandingClock(noon));

            var first   = log.Add("first");
            var second  = log.Add("second", noon.AddHours(1));

            Assert.Multiple(() => {
                Assert.That(first,                           Is.EqualTo(new LogEntry<String>(0, noon,             "first")));
                Assert.That(second,                          Is.EqualTo(new LogEntry<String>(1, noon.AddHours(1), "second")));
                Assert.That(log.Count,                       Is.EqualTo(2));
                Assert.That(log[1],                          Is.SameAs(second));
                Assert.That(log.Values,                      Is.EqualTo(new[] { "first", "second" }));
                Assert.That(() => log[2],                    Throws.TypeOf<ArgumentOutOfRangeException>());
            });

        }

        #endregion

        #region WithoutAClockTheTimestampIsTheClock()

        /// <summary>
        /// Without a clock of its own, a log reads Timestamp.Provider - time travel included -
        /// not the system's clock.
        /// </summary>
        [Test]
        public void WithoutAClockTheTimestampIsTheClock()
        {
            Assert.That(new ConcurrentLog<Int32>().TimeProvider, Is.SameAs(Timestamp.Provider));
        }

        #endregion

        #region ManyEntriesStandInTheirOrderOverManyChunks()

        [Test]
        public void ManyEntriesStandInTheirOrderOverManyChunks()
        {

            const Int32 many = 5_000;

            var log = new ConcurrentLog<Int32>();

            for (var i = 0; i < many; i++)
                log.Add(i);

            Assert.Multiple(() => {
                Assert.That(log.Count,                              Is.EqualTo(many));
                Assert.That(log.Values,                             Is.EqualTo(Enumerable.Range(0, many)));
                Assert.That(log.Select(entry => entry.Position),    Is.EqualTo(Enumerable.Range(0, many)));
                Assert.That(log[4097].Value,                        Is.EqualTo(4097));
                Assert.That(log.From(many - 3).Select(e => e.Value), Is.EqualTo(new[] { many - 3, many - 2, many - 1 }));
                Assert.That(log.From(many),                         Is.Empty);
            });

        }

        #endregion

        #region AWalkSeesTheLogAsItWasWhenItBegan()

        [Test]
        public void AWalkSeesTheLogAsItWasWhenItBegan()
        {

            var log   = new ConcurrentLog();
            var seen  = new List<String>();

            log.Add("a");
            log.Add("b");

            foreach (var entry in log)
            {
                seen.Add(entry.Value);
                log.Add(entry.Value + "'");
            }

            Assert.Multiple(() => {
                Assert.That(seen,       Is.EqualTo(new[] { "a", "b" }));
                Assert.That(log.Values, Is.EqualTo(new[] { "a", "b", "a'", "b'" }));
            });

        }

        #endregion


        #region WaitFor_FindsAnEntryLoggedAlready()

        [Test]
        public async Task WaitFor_FindsAnEntryLoggedAlready()
        {

            var log = new ConcurrentLog();

            log.Add("boot");
            log.Add("heartbeat");

            var heartbeat = await log.WaitFor(entry => entry.Value == "heartbeat", aWhile);

            Assert.That(heartbeat?.Position, Is.EqualTo(1));

        }

        #endregion

        #region WaitFor_IsWokenByAnEntryAddedLater()

        [Test]
        public async Task WaitFor_IsWokenByAnEntryAddedLater()
        {

            var log      = new ConcurrentLog();
            var waiting  = log.WaitFor(entry => entry.Value == "heartbeat", aWhile);

            log.Add("boot");
            await Task.Delay(20);
            Assert.That(waiting.IsCompleted, Is.False, "woken by an entry that does not match");

            _ = Task.Run(() => log.Add("heartbeat"));

            var heartbeat = await waiting;

            Assert.That(heartbeat?.Value, Is.EqualTo("heartbeat"));

        }

        #endregion

        #region WaitFor_LooksFromTheGivenPositionOn()

        [Test]
        public async Task WaitFor_LooksFromTheGivenPositionOn()
        {

            var log     = new ConcurrentLog();

            log.Add("heartbeat");

            var before  = log.End;
            var waiting = log.WaitFor(entry => entry.Value == "heartbeat", aWhile, From: before);

            await Task.Delay(20);
            Assert.That(waiting.IsCompleted, Is.False, "found the heartbeat before the position");

            log.Add("heartbeat");

            Assert.That((await waiting)?.Position, Is.EqualTo(1));

        }

        #endregion

        #region WaitFor_TimesOutByTheClockOfTheLog()

        /// <summary>
        /// The time waited is the log's: here a clock that stands still until it is told to expire.
        /// </summary>
        [Test]
        public async Task WaitFor_TimesOutByTheClockOfTheLog()
        {

            var clock    = new StandingClock(noon);
            var log      = new ConcurrentLog(TimeProvider: clock);

            var waiting  = log.WaitFor(entry => entry.Value == "heartbeat", TimeSpan.FromMilliseconds(1));
            var counting = log.WaitFor(3, TimeSpan.FromMilliseconds(1));

            log.Add("boot");
            await Task.Delay(50);

            Assert.That(waiting.IsCompleted,  Is.False, "timed out by another clock than the log's");

            clock.Expire();

            var heartbeat  = await waiting;
            var counted    = await counting;

            Assert.Multiple(() => {
                Assert.That(heartbeat,                                  Is.Null);
                Assert.That(counted.Select(entry => entry.Value),       Is.EqualTo(new[] { "boot" }));
            });

        }

        #endregion

        #region WaitFor_IsCancelled()

        [Test]
        public void WaitFor_IsCancelled()
        {

            var log           = new ConcurrentLog();
            var cancellation  = new CancellationTokenSource();
            var waiting       = log.WaitFor(entry => true, aWhile, CancellationToken: cancellation.Token);

            cancellation.Cancel();

            Assert.That(async () => await waiting, Throws.InstanceOf<OperationCanceledException>());

        }

        #endregion

        #region WaitFor_CountsEntriesThatMatch()

        [Test]
        public async Task WaitFor_CountsEntriesThatMatch()
        {

            var log      = new ConcurrentLog();

            log.Add("heartbeat 1");
            log.Add("boot");

            var waiting  = log.WaitFor(3, aWhile, entry => entry.Value.StartsWith("heartbeat"));

            log.Add("heartbeat 2");
            log.Add("status");
            await Task.Delay(20);
            Assert.That(waiting.IsCompleted, Is.False, "two of three heartbeats");

            log.Add("heartbeat 3");
            log.Add("heartbeat 4");

            Assert.That((await waiting).Select(entry => entry.Value), Is.EqualTo(new[] { "heartbeat 1", "heartbeat 2", "heartbeat 3" }));

        }

        #endregion


        #region Clear_EmptiesTheLogAndPositionsGoOn()

        [Test]
        public async Task Clear_EmptiesTheLogAndPositionsGoOn()
        {

            var log = new ConcurrentLog();

            for (var i = 0; i < 3_000; i++)
                log.Add("old " + i);

            log.Clear();

            Assert.Multiple(() => {
                Assert.That(log.Count,              Is.Zero);
                Assert.That(log,                    Is.Empty);
                Assert.That(log.Start,              Is.EqualTo(3_000));
                Assert.That(log.End,                Is.EqualTo(3_000));
                Assert.That(() => log[2_999],       Throws.TypeOf<ArgumentOutOfRangeException>());
            });

            var waiting  = log.WaitFor(entry => entry.Value.StartsWith("old") || entry.Value == "new", aWhile);
            var added    = log.Add("new");
            var found    = await waiting;

            Assert.Multiple(() => {
                Assert.That(added.Position,         Is.EqualTo(3_000));
                Assert.That(log.Values,             Is.EqualTo(new[] { "new" }));
                Assert.That(log[3_000],             Is.SameAs(added));
                Assert.That(found,                  Is.SameAs(added), "found an entry cleared");
            });

            log.Clear();
            log.Clear();

            Assert.That(log.Count, Is.Zero);

        }

        #endregion

        #region MaxEntries_KeepsTheNewestEntries()

        [TestCase(    1)]
        [TestCase(    3)]
        [TestCase( 1023)]
        [TestCase( 1024)]
        [TestCase( 1500)]
        [TestCase(10000)]
        public void MaxEntries_KeepsTheNewestEntries(Int32 MaxEntries)
        {

            const Int32 many = 5_000;

            var log = new ConcurrentLog<Int32>(MaxEntries);

            for (var i = 0; i < many; i++)
            {
                log.Add(i);
                Assert.That(log.Count, Is.EqualTo(Math.Min(i + 1, MaxEntries)));
            }

            var kept = Math.Min(many, MaxEntries);

            Assert.Multiple(() => {
                Assert.That(log.MaxEntries,         Is.EqualTo(MaxEntries));
                Assert.That(log.Start,              Is.EqualTo(many - kept));
                Assert.That(log.End,                Is.EqualTo(many));
                Assert.That(log.Values,             Is.EqualTo(Enumerable.Range(many - kept, kept)));
                Assert.That(log[many - 1].Value,    Is.EqualTo(many - 1));
                Assert.That(log[many - kept].Value, Is.EqualTo(many - kept));
                Assert.That(() => log[many - kept - 1], Throws.TypeOf<ArgumentOutOfRangeException>());
                Assert.That(log.From(0).First().Position, Is.EqualTo(many - kept));
            });

        }

        #endregion

        #region MaxEntries_IsAtLeastOne()

        [Test]
        public void MaxEntries_IsAtLeastOne()
        {
            Assert.That(() => new ConcurrentLog(0), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        #endregion

        #region NoGapWhileManyThreadsAddKeepAndClear()

        /// <summary>
        /// Many threads add to a log of at most some entries, while one clears it now and then and one
        /// reads: a reader never sees a gap, nor more than the most entries, nor a position again.
        /// </summary>
        [Test]
        public async Task NoGapWhileManyThreadsAddKeepAndClear()
        {

            const Int32 threads     = 8;
            const Int32 each        = 5_000;
            const Int32 maxEntries  = 1_500;

            var log      = new ConcurrentLog<Int32>(maxEntries);
            var start    = new TaskCompletionSource();
            var adding   = 0;

            var adders   = Enumerable.Range(0, threads).
                               Select(thread => Task.Run(async () => {
                                   await start.Task;
                                   for (var i = 0; i < each; i++)
                                       log.Add(thread * each + i);
                                   Interlocked.Increment(ref adding);
                               })).
                               ToArray();

            var clearer  = Task.Run(async () => {
                               await start.Task;
                               while (Volatile.Read(ref adding) < threads)
                               {
                                   await Task.Delay(1);
                                   log.Clear();
                               }
                           });

            var reader   = Task.Run(async () => {
                               await start.Task;
                               var lastStart = 0L;
                               while (Volatile.Read(ref adding) < threads)
                               {
                                   var positions = log.Select(entry => entry.Position).ToArray();
                                   Assert.That(positions.Length,  Is.LessThanOrEqualTo(maxEntries), "too many");
                                   if (positions.Length > 0)
                                   {
                                       Assert.That(positions, Is.EqualTo(Enumerable.Range(0, positions.Length).Select(i => positions[0] + i)), "a gap");
                                       Assert.That(positions[0], Is.GreaterThanOrEqualTo(lastStart), "an entry again");
                                       lastStart = positions[0];
                                   }
                               }
                           });

            start.SetResult();

            await Task.WhenAll(adders);
            await clearer;
            await reader;

            log.Add(-1);

            Assert.Multiple(() => {
                Assert.That(log.End,         Is.EqualTo(threads * each + 1));
                Assert.That(log.Last().Value, Is.EqualTo(-1));
                Assert.That(log.Count,       Is.InRange(1, maxEntries));
            });

        }

        #endregion


        #region NoEntryIsLostWhenAddedFromManyThreads()

        /// <summary>
        /// Many threads add at once, while one reads and one waits: no entry is lost, a reader never sees
        /// a gap, and the one who waits for all of them is woken.
        /// </summary>
        [Test]
        public async Task NoEntryIsLostWhenAddedFromManyThreads()
        {

            const Int32 threads  = 8;
            const Int32 each     = 2_000;
            const Int32 all      = threads * each;

            var log      = new ConcurrentLog<Int32>();
            var start    = new TaskCompletionSource();

            var waiting  = log.WaitFor(all, TimeSpan.FromSeconds(30));

            var adders   = Enumerable.Range(0, threads).
                               Select(thread => Task.Run(async () => {
                                   await start.Task;
                                   for (var i = 0; i < each; i++)
                                       log.Add(thread * each + i);
                               })).
                               ToArray();

            var reader   = Task.Run(async () => {
                               await start.Task;
                               var last = 0;
                               while (last < all)
                               {
                                   var positions = log.Select(entry => entry.Position).ToArray();
                                   Assert.That(positions, Is.EqualTo(Enumerable.Range(0, positions.Length)), "a gap");
                                   Assert.That(positions,  Has.Length.GreaterThanOrEqualTo(last),           "shrunk");
                                   last = positions.Length;
                               }
                           });

            start.SetResult();

            await Task.WhenAll(adders);
            await reader;

            var waitedFor = await waiting;

            Assert.Multiple(() => {
                Assert.That(log.Values.Order(),  Is.EqualTo(Enumerable.Range(0, all)));
                Assert.That(waitedFor,           Has.Count.EqualTo(all));
            });

        }

        #endregion

    }

}
