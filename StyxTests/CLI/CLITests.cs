/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using NUnit.Framework;

#endregion

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

            Assert.That(s1.Length, Is.EqualTo(7));

        }

        #endregion

        #region Suggest_ad()

        [Test]
        public async Task Suggest_ad()
        {

            var cli = new CLI();

            var s1  = await cli.Suggest("ad");

            Assert.That(s1[0].Suggestion,   Is.EqualTo("add"));
            Assert.That(s1[0].Info,         Is.EqualTo(SuggestionInfo.CommandCompleted));

        }

        #endregion

        #region Suggest_re()

        [Test]
        public async Task Suggest_re()
        {

            var cli = new CLI();

            var s1  = await cli.Suggest("re");

            Assert.That(s1[0].Suggestion,   Is.EqualTo("remove"));
            Assert.That(s1[0].Info,         Is.EqualTo(SuggestionInfo.CommandCompleted));

            Assert.That(s1[1].Suggestion,   Is.EqualTo("removeAll"));
            Assert.That(s1[1].Info,         Is.EqualTo(SuggestionInfo.CommandCompleted));

        }

        #endregion

        #region Suggest_remove_a__oneKVP()

        [Test]
        public async Task Suggest_remove_a__oneKVP()
        {

            var cli = new CLI();

            var e1  = await cli.Execute("add a 1");

            Assert.That(e1.First(),                   Is.EqualTo("Item added: a = 1"));

            Assert.That(cli.Environment.   Count(),   Is.EqualTo(1));
            Assert.That(cli.CommandHistory.First(),   Is.EqualTo("add a 1"));

            var s1  = await cli.Suggest("remove a");
            Assert.That(s1[0].Suggestion,             Is.EqualTo("remove a"));
            Assert.That(s1[0].Info,                   Is.EqualTo(SuggestionInfo.ParameterCompleted));

        }

        #endregion

        #region Suggest_remove_a__twoKVPs()

        [Test]
        public async Task Suggest_remove_a__twoKVPs()
        {

            var cli = new CLI();

            var e1  = await cli.Execute("add aa1 1");
            var e2  = await cli.Execute("add aa2 2");

            Assert.That(e1.First(),                        Is.EqualTo("Item added: aa1 = 1"));
            Assert.That(e2.First(),                        Is.EqualTo("Item added: aa2 = 2"));

            Assert.That(cli.Environment.   Count(),        Is.EqualTo(2));
            Assert.That(cli.CommandHistory.ElementAt(0),   Is.EqualTo("add aa1 1"));
            Assert.That(cli.CommandHistory.ElementAt(1),   Is.EqualTo("add aa2 2"));

            var s1  = await cli.Suggest("remove a");
            Assert.That(s1[0].Suggestion,                  Is.EqualTo("remove aa1"));
            Assert.That(s1[0].Info,                        Is.EqualTo(SuggestionInfo.ParameterPrefix));

            Assert.That(s1[1].Suggestion,                  Is.EqualTo("remove aa2"));
            Assert.That(s1[1].Info,                        Is.EqualTo(SuggestionInfo.ParameterPrefix));

        }

        #endregion

        #region Suggest_remove_a__twoKVPs2()

        [Test]
        public async Task Suggest_remove_a__twoKVPs2()
        {

            var cli = new CLI();

            var e1  = await cli.Execute("add a 1");
            var e2  = await cli.Execute("add aa2 2");

            Assert.That(e1.First(),                        Is.EqualTo("Item added: a = 1"));
            Assert.That(e2.First(),                        Is.EqualTo("Item added: aa2 = 2"));

            Assert.That(cli.Environment.   Count(),        Is.EqualTo(2));
            Assert.That(cli.CommandHistory.ElementAt(0),   Is.EqualTo("add a 1"));
            Assert.That(cli.CommandHistory.ElementAt(1),   Is.EqualTo("add aa2 2"));

            var s1  = await cli.Suggest("remove a");
            Assert.That(s1[0].Suggestion,                  Is.EqualTo("remove a"));
            Assert.That(s1[0].Info,                        Is.EqualTo(SuggestionInfo.ParameterCompleted));

            Assert.That(s1[1].Suggestion,                  Is.EqualTo("remove aa2"));
            Assert.That(s1[1].Info,                        Is.EqualTo(SuggestionInfo.ParameterPrefix));

        }

        #endregion


    }

}
