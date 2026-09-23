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
    /// Command Line Interface tests.
    /// </summary>
    [TestFixture]
    public class CLI_Tests
    {

        #region SetupOnce()

        [OneTimeSetUp]
        public async Task SetupOnce()
        {

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public void SetupEachTest()
        {

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public void ShutdownEachTest()
        {

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public virtual async Task ShutdownOnce()
        {

        }

        #endregion

        #region Suggest_()

        [Test]
        public async Task Suggest_()
        {

            var cli = new CLI();

            var s1  = await cli.Suggest("");

            Assert.That(s1.Length, Is.EqualTo(8));

        }

        #endregion

        #region Suggest_ad()

        [Test]
        public async Task Suggest_ad()
        {

            var cli = new CLI();

            var s1  = await cli.Suggest("ad");

            Assert.That(s1[0].Suggestion,   Is.EqualTo("addEnv"));
            Assert.That(s1[0].Info,         Is.EqualTo(SuggestionInfo.CommandCompleted));

        }

        #endregion

        #region Suggest_re()

        [Test]
        public async Task Suggest_re()
        {

            var cli = new CLI();

            var s1  = await cli.Suggest("re");

            Assert.That(s1[0].Suggestion,   Is.EqualTo("removeAll"));
            Assert.That(s1[0].Info,         Is.EqualTo(SuggestionInfo.CommandCompleted));

            Assert.That(s1[1].Suggestion,   Is.EqualTo("removeEnv"));
            Assert.That(s1[1].Info,         Is.EqualTo(SuggestionInfo.CommandCompleted));

        }

        #endregion

        #region Suggest_remove_a__oneKVP()

        [Test]
        public async Task Suggest_remove_a__oneKVP()
        {

            var cli = new CLI();

            var e1  = await cli.Execute("addEnv a 1");

            Assert.That(e1.First(),                   Is.EqualTo("Environment key added: 'a' = 1"));

            Assert.That(cli.Environment.   Count(),   Is.EqualTo(1));
            Assert.That(cli.CommandHistory.First(),   Is.EqualTo("addEnv a 1"));

            var s1  = await cli.Suggest("removeEnv a");
            Assert.That(s1[0].Suggestion,             Is.EqualTo("removeEnv a"));
            Assert.That(s1[0].Info,                   Is.EqualTo(SuggestionInfo.ParameterCompleted));

        }

        #endregion

        #region Suggest_remove_a__twoKVPs()

        [Test]
        public async Task Suggest_remove_a__twoKVPs()
        {

            var cli = new CLI();

            var e1  = await cli.Execute("addEnv aa1 1");
            var e2  = await cli.Execute("addEnv aa2 2");

            Assert.That(e1.First(),                        Is.EqualTo("Environment key added: 'aa1' = 1"));
            Assert.That(e2.First(),                        Is.EqualTo("Environment key added: 'aa2' = 2"));

            Assert.That(cli.Environment.   Count(),        Is.EqualTo(2));
            Assert.That(cli.CommandHistory.ElementAt(0),   Is.EqualTo("addEnv aa1 1"));
            Assert.That(cli.CommandHistory.ElementAt(1),   Is.EqualTo("addEnv aa2 2"));

            var s1  = await cli.Suggest("removeEnv a");
            Assert.That(s1[0].Suggestion,                  Is.EqualTo("removeEnv aa1"));
            Assert.That(s1[0].Info,                        Is.EqualTo(SuggestionInfo.ParameterPrefix));

            Assert.That(s1[1].Suggestion,                  Is.EqualTo("removeEnv aa2"));
            Assert.That(s1[1].Info,                        Is.EqualTo(SuggestionInfo.ParameterPrefix));

        }

        #endregion

        #region Suggest_remove_a__twoKVPs2()

        [Test]
        public async Task Suggest_remove_a__twoKVPs2()
        {

            var cli = new CLI();

            var e1  = await cli.Execute("addEnv a 1");
            var e2  = await cli.Execute("addEnv aa2 2");

            Assert.That(e1.First(),                        Is.EqualTo("Environment key added: 'a' = 1"));
            Assert.That(e2.First(),                        Is.EqualTo("Environment key added: 'aa2' = 2"));

            Assert.That(cli.Environment.   Count(),        Is.EqualTo(2));
            Assert.That(cli.CommandHistory.ElementAt(0),   Is.EqualTo("addEnv a 1"));
            Assert.That(cli.CommandHistory.ElementAt(1),   Is.EqualTo("addEnv aa2 2"));

            var s1  = await cli.Suggest("removeEnv a");
            Assert.That(s1[0].Suggestion,                  Is.EqualTo("removeEnv a"));
            Assert.That(s1[0].Info,                        Is.EqualTo(SuggestionInfo.ParameterCompleted));

            Assert.That(s1[1].Suggestion,                  Is.EqualTo("removeEnv aa2"));
            Assert.That(s1[1].Info,                        Is.EqualTo(SuggestionInfo.ParameterPrefix));

        }

        #endregion


        #region A_block_arrives_in_one_piece()

        /// <summary>
        /// Several threads writing several lines each, and no line of one ending
        /// up between two lines of another.
        /// </summary>
        /// <remarks>
        /// This is the half of WriteBlock that can be checked without a screen.
        /// A program with a command line on the same console as its log writes
        /// from whichever thread did the thing being logged, and Console.Out
        /// makes each single WriteLine atomic and promises nothing at all about
        /// three of them in a row - which is exactly what one log entry, or one
        /// command's answer, consists of.
        ///
        /// Without the lock this fails immediately and by a lot: with 32 threads
        /// the first interleaved trio usually appears within the first hundred
        /// lines.
        /// </remarks>
        [Test]
        public void A_block_arrives_in_one_piece()
        {

            const Int32 writers  = 32;
            const Int32 rounds   = 8;

            var cli       = new CLI();
            var captured  = new StringWriter();
            var previous  = Console.Out;

            try
            {

                Console.SetOut(captured);

                Parallel.For(0, writers, writer => {
                    for (var round = 0; round < rounds; round++)
                        cli.WriteBlock(() => {
                            Console.WriteLine($"{writer} one");
                            Console.WriteLine($"{writer} two");
                            Console.WriteLine($"{writer} three");
                        });
                });

            }
            finally
            {
                Console.SetOut(previous);
            }

            var lines = captured.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            Assert.That(lines.Length, Is.EqualTo(writers * rounds * 3));

            for (var i = 0; i < lines.Length; i += 3)
            {

                var who = lines[i].Split(' ')[0];

                Assert.Multiple(() => {
                    Assert.That(lines[i],      Is.EqualTo($"{who} one"));
                    Assert.That(lines[i + 1],  Is.EqualTo($"{who} two"));
                    Assert.That(lines[i + 2],  Is.EqualTo($"{who} three"));
                });

            }

        }

        #endregion

        #region A_block_needs_no_command_line_to_be_written()

        /// <summary>
        /// Nothing is being typed, so there is nothing to take off the screen
        /// and nothing to put back - and the block is simply written.
        /// </summary>
        /// <remarks>
        /// Worth its own test because the restoring half of WriteBlock moves the
        /// cursor, and a process with no console at all - a service, a test host,
        /// a CI runner - would throw if that ran when it had no reason to.
        /// </remarks>
        [Test]
        public void A_block_needs_no_command_line_to_be_written()
        {

            var cli       = new CLI();
            var captured  = new StringWriter();
            var previous  = Console.Out;

            try
            {
                Console.SetOut(captured);
                cli.WriteBlock(() => Console.WriteLine("nobody is typing"));
            }
            finally
            {
                Console.SetOut(previous);
            }

            Assert.That(captured.ToString().Trim(), Is.EqualTo("nobody is typing"));

        }

        #endregion

    }

}
