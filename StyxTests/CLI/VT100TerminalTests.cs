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

namespace org.GraphDefined.Vanaheimr.CLI.Tests
{

    /// <summary>
    /// A terminal that is only bytes: what goes out for what is written, what
    /// comes in as keys, and what it does when the far end stops reading.
    /// </summary>
    [TestFixture]
    public class VT100Terminal_Tests
    {

        #region (class) ManualTimeProvider

        /// <summary>
        /// A clock whose timers fire when a test says so.
        /// </summary>
        private sealed class ManualTimeProvider : TimeProvider
        {

            private readonly List<(TimerCallback Callback, Object? State, Boolean Armed)> timers = [];

            public override ITimer CreateTimer(TimerCallback Callback, Object? State, TimeSpan DueTime, TimeSpan Period)
            {
                var timer = new ManualTimer(this, timers.Count);
                timers.Add((Callback, State, DueTime != Timeout.InfiniteTimeSpan));
                return timer;
            }

            public void FireAll()
            {
                for (var i = 0; i < timers.Count; i++)
                    if (timers[i].Armed)
                    {
                        timers[i] = timers[i] with { Armed = false };
                        timers[i].Callback(timers[i].State);
                    }
            }

            private sealed class ManualTimer(ManualTimeProvider Provider, Int32 Index) : ITimer
            {
                public Boolean Change(TimeSpan DueTime, TimeSpan Period)
                {
                    Provider.timers[Index] = Provider.timers[Index] with { Armed = DueTime != Timeout.InfiniteTimeSpan };
                    return true;
                }
                public void Dispose() { }
                public ValueTask DisposeAsync() => ValueTask.CompletedTask;
            }

        }

        #endregion


        #region ALineEndsAsATerminalWantsIt()

        /// <summary>
        /// There is no line discipline at the far end: "\n" goes out as "\r\n",
        /// and a "\r\n" that is already there is not made "\r\r\n".
        /// </summary>
        [Test]
        public async Task ALineEndsAsATerminalWantsIt()
        {

            var screen = new Screen();

            await using var terminal = new VT100Terminal(screen.SendAsync);

            terminal.Write("one\ntwo\r\nthree");
            terminal.WriteLine("!");

            await terminal.FlushAsync();

            Assert.That(screen.Received, Is.EqualTo("one\r\ntwo\r\nthree!\r\n"));

        }

        #endregion

        #region AColourIsSaidAndTakenBack()

        /// <summary>
        /// Coloured text is preceded by its colour and followed by the
        /// terminal's own again, so that nothing after it is coloured by it.
        /// </summary>
        [Test]
        public async Task AColourIsSaidAndTakenBack()
        {

            var screen = new Screen();

            await using var terminal = new VT100Terminal(screen.SendAsync);

            terminal.Write("warning", ConsoleColor.Yellow);
            terminal.Write(" plain");

            await terminal.FlushAsync();

            Assert.That(screen.Received, Is.EqualTo("\x1b[93mwarning\x1b[39m plain"));

        }

        #endregion

        #region KeysComeInTheOrderTheyWereSentAndThenNull()

        /// <summary>
        /// The keys of what was fed, in order, and null once the far end said it
        /// sends nothing more.
        /// </summary>
        [Test]
        public async Task KeysComeInTheOrderTheyWereSentAndThenNull()
        {

            await using var terminal = new VT100Terminal((_, _) => ValueTask.CompletedTask);

            terminal.Feed("ab\x1b[D\r"u8);
            terminal.Complete();

            var keys = new List<ConsoleKeyInfo>();

            while (await terminal.ReadKeyAsync() is ConsoleKeyInfo key)
                keys.Add(key);

            Assert.That(keys.Select(key => key.Key == 0 ? key.KeyChar.ToString() : key.Key.ToString()),
                        Is.EqualTo(new[] { "a", "b", "LeftArrow", "Enter" }));

        }

        #endregion

        #region EscapeComesOnceNothingFollowedIt()

        /// <summary>
        /// Escape on its own is a key once the clock says nothing followed it -
        /// and not before, because something still might.
        /// </summary>
        [Test]
        public async Task EscapeComesOnceNothingFollowedIt()
        {

            var clock = new ManualTimeProvider();

            await using var terminal = new VT100Terminal((_, _) => ValueTask.CompletedTask, TimeProvider: clock);

            terminal.Feed([ 0x1b ]);

            var reading = terminal.ReadKeyAsync().AsTask();

            await Task.Delay(50);
            Assert.That(reading.IsCompleted, Is.False, "Escape came before anybody could know it was alone");

            clock.FireAll();

            Assert.That((await reading.WaitAsync(TimeSpan.FromSeconds(5)))?.Key, Is.EqualTo(ConsoleKey.Escape));

        }

        #endregion

        #region ANewWidthIsSaid()

        /// <summary>
        /// A width that changed is said, at least one column, and a width that
        /// did not is not.
        /// </summary>
        [Test]
        public async Task ANewWidthIsSaid()
        {

            await using var terminal = new VT100Terminal((_, _) => ValueTask.CompletedTask, Width: 80);

            var said = 0;
            terminal.Resized += () => said++;

            terminal.Width = 80;
            terminal.Width = 120;
            terminal.Width = 0;

            Assert.Multiple(() => {
                Assert.That(said,            Is.EqualTo(2));
                Assert.That(terminal.Width,  Is.EqualTo(1));
            });

        }

        #endregion

        #region AFarEndThatDoesNotReadDoesNotHoldUpTheWriter()

        /// <summary>
        /// A far end that takes nothing does not make a write wait: writes are
        /// queued - until the queue is full, and then a write throws rather than
        /// growing it without end.
        /// </summary>
        [Test]
        public async Task AFarEndThatDoesNotReadDoesNotHoldUpTheWriter()
        {

            var never = new TaskCompletionSource();

            await using var terminal = new VT100Terminal(async (_, ct) => await never.Task.WaitAsync(ct),
                                                         MaxPendingOutput: 1000);

            var line = new String('x', 98);

            // Ten lines of a hundred bytes each, "\r\n" included, are the limit.
            for (var i = 0; i < 10; i++)
                terminal.WriteLine(line);

            Assert.That(() => terminal.WriteLine(line), Throws.TypeOf<IOException>());

        }

        #endregion

        #region AFarEndThatFailedIsSaidToHave()

        /// <summary>
        /// A send that failed is what every write after it says, rather than a
        /// write that silently goes nowhere.
        /// </summary>
        [Test]
        public async Task AFarEndThatFailedIsSaidToHave()
        {

            await using var terminal = new VT100Terminal((_, _) => throw new IOException("connection reset"));

            terminal.Write("anything");

            Assert.That(async () => await terminal.FlushAsync(),  Throws.TypeOf<IOException>());
            Assert.That(() => terminal.Write("more"),             Throws.TypeOf<IOException>().With.Message.Contains("connection reset"));

        }

        #endregion

    }

}
