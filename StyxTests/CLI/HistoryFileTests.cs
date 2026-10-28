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
    /// The history kept in a file between sessions: what comes back, what
    /// never goes into it, and how much of it there is.
    /// </summary>
    [TestFixture]
    public class HistoryFile_Tests
    {

        #region (class) SecretCommand

        /// <summary>
        /// A command whose line carries a secret, and says so.
        /// </summary>
        public sealed class SecretCommand(CLI CLI) : ACLICommand(CLI),
                                                     ICLICommand
        {

            public override IEnumerable<SuggestionResponse> Suggest(String[] Arguments)
                => "secret".StartsWith(Arguments[0], StringComparison.OrdinalIgnoreCase)
                       ? [ SuggestionResponse.CommandCompleted("secret") ]
                       : [];

            public override Task<String[]> Execute(String[] Arguments, CancellationToken CancellationToken)
                => Task.FromResult<String[]>([ "kept to myself" ]);

            public override String Help() => "secret <secret> - keeps it";

            public override Boolean MayBeKept(String[] Arguments)
                => false;

        }

        #endregion

        #region Data

        private String directory = "";

        #endregion

        #region SetupEachTest() / ShutdownEachTest()

        [SetUp]
        public void SetupEachTest()
        {
            directory = Path.Combine(Path.GetTempPath(), $"styx-history-{Guid.NewGuid().ToString("N")[..12]}");
        }

        [TearDown]
        public void ShutdownEachTest()
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }

        #endregion

        #region (private) InTheDirectory(Name = "history")

        private String InTheDirectory(String Name = "history")
            => Path.Combine(directory, "below", Name);

        #endregion


        #region AKeptHistoryComesBackInTheNextSession()

        /// <summary>
        /// The lines of one session come back in the next, the oldest first,
        /// and the directory the file is in is made where there is none.
        /// </summary>
        [Test]
        public async Task AKeptHistoryComesBackInTheNextSession()
        {

            using (var first = new CLI())
            {
                first.KeepHistoryIn(new HistoryFile(InTheDirectory()));
                await first.Execute("addEnv a 1");
                await first.Execute("listEnv");
            }

            using var next = new CLI();
            next.KeepHistoryIn(new HistoryFile(InTheDirectory()));

            Assert.That(next.CommandHistory, Is.EqualTo(new[] { "addEnv a 1", "listEnv" }));

            await next.Execute("addEnv b 2");

            Assert.That(next.CommandHistory,                        Is.EqualTo(new[] { "addEnv a 1", "listEnv", "addEnv b 2" }));
            Assert.That(new HistoryFile(InTheDirectory()).Read(),   Is.EqualTo(new[] { "addEnv a 1", "listEnv", "addEnv b 2" }));

        }

        #endregion

        #region AMistypedLineStaysInItsSession()

        /// <summary>
        /// A line nothing answered to comes back with Up in its own session, to
        /// be put right, and is never written down: nothing could say whether
        /// it carries a secret - "apiKys add key ..." would.
        /// </summary>
        [Test]
        public async Task AMistypedLineStaysInItsSession()
        {

            using var first = new CLI();
            first.KeepHistoryIn(new HistoryFile(InTheDirectory()));

            await first.Execute("addEnw a 1");
            await first.Execute("addEnv a 1");

            Assert.That(first.CommandHistory,                       Is.EqualTo(new[] { "addEnw a 1", "addEnv a 1" }));
            Assert.That(new HistoryFile(InTheDirectory()).Read(),   Is.EqualTo(new[] { "addEnv a 1" }));

        }

        #endregion

        #region ALineItsCommandMayNotKeepStaysInItsSession()

        /// <summary>
        /// A command that says its line may not be kept - a secret in it - has
        /// it in the session's history only, where it stands on the screen
        /// anyway, and never in the file.
        /// </summary>
        [Test]
        public async Task ALineItsCommandMayNotKeepStaysInItsSession()
        {

            using var first = new CLI(typeof(HistoryFile_Tests).Assembly);
            first.KeepHistoryIn(new HistoryFile(InTheDirectory()));

            await first.Execute("addEnv a 1");
            var answer = await first.Execute("secret hunter2");

            Assert.That(answer,                                     Is.EqualTo(new[] { "kept to myself" }));
            Assert.That(first.CommandHistory,                       Is.EqualTo(new[] { "addEnv a 1", "secret hunter2" }));
            Assert.That(new HistoryFile(InTheDirectory()).Read(),   Is.EqualTo(new[] { "addEnv a 1" }));
            Assert.That(File.ReadAllText(InTheDirectory()),         Does.Not.Contain("hunter2"));

        }

        #endregion

        #region TheOldestLinesAreLetGo()

        /// <summary>
        /// No more than the newest MaxLines lines are kept, and no more are read
        /// from a file that was written with more.
        /// </summary>
        [Test]
        public void TheOldestLinesAreLetGo()
        {

            var file = new HistoryFile(InTheDirectory(), MaxLines: 3);

            foreach (var n in Enumerable.Range(1, 5))
                Assert.That(file.Add($"line {n}"), Is.True);

            Assert.That(file.Read(),                                     Is.EqualTo(new[] { "line 3", "line 4", "line 5" }));
            Assert.That(File.ReadAllLines(InTheDirectory()),             Is.EqualTo(new[] { "line 3", "line 4", "line 5" }));

            File.WriteAllLines(InTheDirectory(), Enumerable.Range(1, 9).Select(n => $"line {n}"));

            Assert.That(file.Read(),                                     Is.EqualTo(new[] { "line 7", "line 8", "line 9" }));
            Assert.That(new HistoryFile(InTheDirectory()).Read().Count,  Is.EqualTo(9));

        }

        #endregion

        #region TheSameLineTwiceInARowIsKeptOnce()

        /// <summary>
        /// The same line again is kept once, across sessions as within one:
        /// the newest line of the last session typed first in the next.
        /// </summary>
        [Test]
        public async Task TheSameLineTwiceInARowIsKeptOnce()
        {

            using (var first = new CLI())
            {
                first.KeepHistoryIn(new HistoryFile(InTheDirectory()));
                await first.Execute("addEnv a 1");
                await first.Execute("addEnv a 1");
            }

            using var next = new CLI();
            next.KeepHistoryIn(new HistoryFile(InTheDirectory()));
            await next.Execute("addEnv a 1");

            Assert.That(next.CommandHistory,                        Is.EqualTo(new[] { "addEnv a 1" }));
            Assert.That(new HistoryFile(InTheDirectory()).Read(),   Is.EqualTo(new[] { "addEnv a 1" }));

        }

        #endregion

        #region ALineTypedBeforeTheFileComesAfterIt()

        /// <summary>
        /// What was typed before the file was given stays the newest: the file's
        /// lines are older than anything of this session.
        /// </summary>
        [Test]
        public async Task ALineTypedBeforeTheFileComesAfterIt()
        {

            new HistoryFile(InTheDirectory()).Add("addEnv a 1");

            using var cli = new CLI();
            await cli.Execute("addEnv b 2");
            cli.KeepHistoryIn(new HistoryFile(InTheDirectory()));

            Assert.That(cli.CommandHistory, Is.EqualTo(new[] { "addEnv a 1", "addEnv b 2" }));

        }

        #endregion

        #region SessionsSharingAFileLoseNoLine()

        /// <summary>
        /// Two sessions of the same account write to the same file at the same
        /// time, and every line of each is in it afterwards.
        /// </summary>
        [Test]
        public void SessionsSharingAFileLoseNoLine()
        {

            const Int32 sessions  = 8;
            const Int32 lines     = 40;

            Parallel.For(0, sessions, session => {
                var file = new HistoryFile(InTheDirectory());
                for (var line = 0; line < lines; line++)
                    Assert.That(file.Add($"session {session} line {line}"), Is.True);
            });

            var kept = new HistoryFile(InTheDirectory()).Read();

            Assert.That(kept.Count,  Is.EqualTo(sessions * lines));
            Assert.That(kept,        Is.Unique);

            Assert.That(Directory.GetFiles(Path.GetDirectoryName(InTheDirectory())!), Is.EqualTo(new[] { InTheDirectory() }), "a file left beside it");

        }

        #endregion

        #region ALineThatIsNotOneLineIsNotKept()

        /// <summary>
        /// A line break, or any other control character, in what is to be kept
        /// would make two lines of it, or a line that rewrites the screen it is
        /// shown on.
        /// </summary>
        [Test]
        public void ALineThatIsNotOneLineIsNotKept()
        {

            var file = new HistoryFile(InTheDirectory());

            Assert.That(file.Add("addEnv a 1\naddEnv b 2"),  Is.False);
            Assert.That(file.Add("addEnv a \x1b[2J"),        Is.False);
            Assert.That(file.Add("   "),                     Is.False);

            Assert.That(File.Exists(InTheDirectory()),       Is.False);

        }

        #endregion

        #region AFileThatCannotBeUsedStopsNoCommand()

        /// <summary>
        /// Where the file cannot be read or written - a directory stands where
        /// it should be - the history starts empty and commands run as ever.
        /// </summary>
        [Test]
        public async Task AFileThatCannotBeUsedStopsNoCommand()
        {

            Directory.CreateDirectory(InTheDirectory());

            var file = new HistoryFile(InTheDirectory());

            Assert.That(file.Read(),                Is.Empty);
            Assert.That(file.Add("addEnv a 1"),     Is.False);

            using var cli = new CLI();
            cli.KeepHistoryIn(file);

            var answer = await cli.Execute("addEnv a 1");

            Assert.That(answer.First(),             Is.EqualTo("Environment key added: 'a' = 1"));
            Assert.That(cli.CommandHistory,         Is.EqualTo(new[] { "addEnv a 1" }));
            Assert.That(Directory.GetFiles(Path.GetDirectoryName(InTheDirectory())!), Is.Empty, "a file left beside it");

        }

        #endregion

        #region TheFileIsTheOwnersAlone()

        /// <summary>
        /// On a platform that says so in modes, the file is read and written by
        /// its owner only, and the directory made for it is entered by its
        /// owner only.
        /// </summary>
        [Test]
        public void TheFileIsTheOwnersAlone()
        {

            if (OperatingSystem.IsWindows())
                Assert.Ignore("Windows has no file modes; the file inherits the ACL of the directory above.");

            var file = new HistoryFile(InTheDirectory());

            Assert.That(file.Add("addEnv a 1"), Is.True);
            Assert.That(file.Add("addEnv b 2"), Is.True);

            Assert.That(File.GetUnixFileMode(InTheDirectory()),                         Is.EqualTo(UnixFileMode.UserRead | UnixFileMode.UserWrite));
            Assert.That(File.GetUnixFileMode(Path.GetDirectoryName(InTheDirectory())!), Is.EqualTo(UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute));

        }

        #endregion

    }

}
