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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.CLI.Tests
{

    /// <summary>
    /// The line editor typed at over a terminal that is only bytes, as
    /// somebody signed in over SSH types at it: keys in, a screen out.
    /// </summary>
    /// <remarks>
    /// The console of a test host cannot be typed at - its input is
    /// redirected - so the editor was only ever tried by hand, or under a
    /// pseudo console. A terminal that is bytes can be typed at by a test,
    /// and what the editor does is the same on either.
    /// </remarks>
    [TestFixture]
    public class CLIOnATerminal_Tests
    {

        #region (class) TestCLI, WaitCommand, EchoCommand

        /// <summary>
        /// A command line with a prompt short enough to read in an assertion.
        /// </summary>
        public sealed class TestCLI(ICLITerminal Terminal) : CLI(Terminal, typeof(TestCLI).Assembly)
        {
            protected override String GetPrompt() => "> ";
        }

        /// <summary>
        /// A command that runs until it is cancelled, and says when it started.
        /// </summary>
        public sealed class WaitCommand(CLI CLI) : ACLICommand(CLI),
                                                   ICLICommand
        {

            public static readonly SemaphoreSlim Started = new (0);

            public override IEnumerable<SuggestionResponse> Suggest(String[] Arguments)
                => "wait".StartsWith(Arguments[0], StringComparison.OrdinalIgnoreCase)
                       ? [ SuggestionResponse.CommandCompleted("wait") ]
                       : [];

            public override async Task<String[]> Execute(String[] Arguments, CancellationToken CancellationToken)
            {
                Started.Release();
                await Task.Delay(Timeout.Infinite, CancellationToken);
                return [ "never" ];
            }

            public override String Help() => "wait - until cancelled";

        }

        /// <summary>
        /// A command that answers with what it was given.
        /// </summary>
        public sealed class EchoCommand(CLI CLI) : ACLICommand(CLI),
                                                   ICLICommand
        {

            public override IEnumerable<SuggestionResponse> Suggest(String[] Arguments)
                => "echo".StartsWith(Arguments[0], StringComparison.OrdinalIgnoreCase)
                       ? [ SuggestionResponse.CommandCompleted("echo") ]
                       : [];

            public override Task<String[]> Execute(String[] Arguments, CancellationToken CancellationToken)
                => Task.FromResult<String[]>([ "said: " + String.Join(" ", Arguments.Skip(1)) ]);

            public override String Help() => "echo <words> - says them";

        }

        #endregion

        #region (private) Typing(Width = 80)

        /// <summary>
        /// A command line on a terminal of the given width, running, and the
        /// screen at the far end of it.
        /// </summary>
        private static (Screen Screen, VT100Terminal Terminal, TestCLI CLI, Task Running) Typing(Int32 Width = 80)
        {

            var screen    = new Screen();
            var terminal  = new VT100Terminal(screen.SendAsync, Width);
            var cli       = new TestCLI(terminal);

            return (screen, terminal, cli, Task.Run(() => cli.Run()));

        }

        /// <summary>
        /// Wait for the command line to end, and fail where it does not.
        /// </summary>
        private static async Task Ended(Task Running, String Why)
        {
            Assert.That(await Task.WhenAny(Running, Task.Delay(TimeSpan.FromSeconds(5))), Is.SameAs(Running), Why);
            await Running;
        }

        #endregion


        #region ACommandTypedIsRunAndAnswered()

        /// <summary>
        /// A command typed and Enter: its answer under it, and a new prompt.
        /// </summary>
        [Test]
        public async Task ACommandTypedIsRunAndAnswered()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("echo hello\r"u8);

            await screen.WaitForAsync(s => s.Rows.SequenceEqual([ "> echo hello", "said: hello", ">" ]), "the answer and a new prompt");

            terminal.Feed("quit\r"u8);
            await Ended(running, "quit did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region TheArrowsEditTheLine()

        /// <summary>
        /// Left twice and a character inserts it there; Home and End go to the
        /// ends, in PuTTY's spelling; Delete takes the character under the
        /// cursor - and '.', which is ConsoleKey.Delete's number, is typed.
        /// </summary>
        [Test]
        public async Task TheArrowsEditTheLine()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("echo ac\x1b[Db"u8);          // "echo abc"
            terminal.Feed("\x1b[1~X\x1b[3~"u8);         // Home, X, Delete: "Xcho abc"
            terminal.Feed("\x1b[4~."u8);                // End, ".": "Xcho abc."

            await screen.WaitForAsync(s => s.CursorRow == "> Xcho abc.", "the edited line");

            Assert.That(screen.CursorColumn, Is.EqualTo("> Xcho abc.".Length));

            terminal.Complete();
            await Ended(running, "the end of the input did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region TabCompletesOverTheTerminal()

        /// <summary>
        /// Tab completes what was typed, as it does at a console.
        /// </summary>
        [Test]
        public async Task TabCompletesOverTheTerminal()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("ec\t"u8);

            await screen.WaitForAsync(s => s.CursorRow == "> echo", "the completed command");

            terminal.Complete();
            await Ended(running, "the end of the input did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region UpBringsBackWhatWasTyped()

        /// <summary>
        /// Up brings back the last command, in the arrow's application spelling.
        /// </summary>
        [Test]
        public async Task UpBringsBackWhatWasTyped()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("echo once\r"u8);
            await screen.WaitForAsync(s => s.Rows.Contains("said: once"), "the answer");

            terminal.Feed("\x1bOA"u8);
            await screen.WaitForAsync(s => s.CursorRow == "> echo once", "the command again");

            terminal.Complete();
            await Ended(running, "the end of the input did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region CtrlCAbandonsTheLine()

        /// <summary>
        /// Ctrl+C while typing abandons the line, as a shell does: it stays on
        /// the screen with ^C behind it, nothing is run, and a new prompt comes.
        /// </summary>
        [Test]
        public async Task CtrlCAbandonsTheLine()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("echo no\x03"u8);

            await screen.WaitForAsync(s => s.Rows.SequenceEqual([ "> echo no^C", ">" ]), "the abandoned line and a new prompt");

            Assert.That(cli.CommandHistory, Is.Empty);

            terminal.Complete();
            await Ended(running, "the end of the input did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region CtrlCStopsTheCommandAndOnlyThatOne()

        /// <summary>
        /// Ctrl+C while a command runs stops that command - and only that one:
        /// the next command is not cancelled before it starts, which is what
        /// the one cancellation for the whole command line used to do.
        /// </summary>
        [Test]
        public async Task CtrlCStopsTheCommandAndOnlyThatOne()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("wait\r"u8);
            Assert.That(await WaitCommand.Started.WaitAsync(TimeSpan.FromSeconds(5)), Is.True, "the command did not start");

            terminal.Feed("\x03"u8);
            await screen.WaitForAsync(s => s.Rows.Contains("Command execution cancelled"), "the command to be cancelled");

            terminal.Feed("echo after\r"u8);
            await screen.WaitForAsync(s => s.Rows.Contains("said: after"), "the next command's answer");

            terminal.Complete();
            await Ended(running, "the end of the input did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region AnInterruptStopsTheCommand()

        /// <summary>
        /// An interrupt beside the keys - a signal from the far end - stops the
        /// command that is running as Ctrl+C does.
        /// </summary>
        [Test]
        public async Task AnInterruptStopsTheCommand()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("wait\r"u8);
            Assert.That(await WaitCommand.Started.WaitAsync(TimeSpan.FromSeconds(5)), Is.True, "the command did not start");

            terminal.Interrupt();
            await screen.WaitForAsync(s => s.Rows.Contains("Command execution cancelled"), "the command to be cancelled");

            terminal.Complete();
            await Ended(running, "the end of the input did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region WhatIsTypedWhileACommandRunsIsKept()

        /// <summary>
        /// Keys typed while a command runs are read - to see whether one is
        /// Ctrl+C - and kept for the line after it.
        /// </summary>
        [Test]
        public async Task WhatIsTypedWhileACommandRunsIsKept()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("wait\r"u8);
            Assert.That(await WaitCommand.Started.WaitAsync(TimeSpan.FromSeconds(5)), Is.True, "the command did not start");

            terminal.Feed("echo ahead"u8);
            await Task.Delay(100);
            terminal.Feed("\x03"u8);

            await screen.WaitForAsync(s => s.CursorRow == "> echo ahead", "what was typed ahead, on the next line");

            terminal.Complete();
            await Ended(running, "the end of the input did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region CtrlDLeavesOnAnEmptyLine()

        /// <summary>
        /// Ctrl+D on an empty line leaves; on a line with something in it, it
        /// takes the character under the cursor.
        /// </summary>
        [Test]
        public async Task CtrlDLeavesOnAnEmptyLine()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("ab\x01\x04"u8);
            await screen.WaitForAsync(s => s.CursorRow == "> b", "the character under the cursor taken");

            terminal.Feed("\x05\x7f\x04"u8);
            await Ended(running, "Ctrl+D on an empty line did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region ExitLeavesAsQuitDoes()

        /// <summary>
        /// 'exit' leaves, because that is what everybody types at the end of a
        /// session.
        /// </summary>
        [Test]
        public async Task ExitLeavesAsQuitDoes()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("exit\r"u8);
            await Ended(running, "exit did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region TheTokenEndsTheCommandLine()

        /// <summary>
        /// The token the command line runs with ends it, while somebody is in the
        /// middle of a line - a session that is being closed.
        /// </summary>
        [Test]
        public async Task TheTokenEndsTheCommandLine()
        {

            var screen     = new Screen();
            var terminal   = new VT100Terminal(screen.SendAsync);
            var cli        = new TestCLI(terminal);
            using var cts  = new CancellationTokenSource();
            var running    = Task.Run(() => cli.Run(cts.Token));

            terminal.Feed("ech"u8);
            await screen.WaitForAsync(s => s.CursorRow == "> ech", "the half-typed line");

            await cts.CancelAsync();
            await Ended(running, "the token did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region ABlockIsWrittenAboveTheLineBeingTyped()

        /// <summary>
        /// What another thread writes while somebody types goes above the line,
        /// and the line comes back under it with the cursor where it was - on
        /// this terminal, through the block it was handed.
        /// </summary>
        [Test]
        public async Task ABlockIsWrittenAboveTheLineBeingTyped()
        {

            var (screen, terminal, cli, running) = Typing();

            terminal.Feed("echo hal\x1b[D"u8);
            await screen.WaitForAsync(s => s.CursorRow == "> echo hal" && s.CursorColumn == 9, "the half-typed line");

            cli.WriteBlock(t => t.WriteLine("12:00:00 a log entry", ConsoleColor.Gray));

            await screen.WaitForAsync(s => s.Rows.SequenceEqual([ "12:00:00 a log entry", "> echo hal" ]), "the entry and the line under it");

            Assert.That(screen.CursorColumn, Is.EqualTo(9), "the cursor did not come back where it was");

            terminal.Complete();
            await Ended(running, "the end of the input did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

        #region ANarrowerTerminalRedrawsTheLine()

        /// <summary>
        /// A terminal that becomes narrower than the line gets the line drawn
        /// again, through a window onto it that fits.
        /// </summary>
        [Test]
        public async Task ANarrowerTerminalRedrawsTheLine()
        {

            var (screen, terminal, cli, running) = Typing(Width: 40);

            terminal.Feed(Encoding.ASCII.GetBytes("echo " + new String('x', 20)));
            await screen.WaitForAsync(s => s.CursorRow.EndsWith("xxxxx"), "the line");

            terminal.Width = 16;

            await screen.WaitForAsync(s => s.CursorRow.StartsWith("> <") || s.CursorRow.StartsWith("<"), "a window onto the line");

            terminal.Complete();
            await Ended(running, "the end of the input did not end the command line");

            await terminal.DisposeAsync();

        }

        #endregion

    }

}
