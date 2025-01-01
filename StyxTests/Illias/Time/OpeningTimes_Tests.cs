/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// OpeningTimes tests.
    /// </summary>
    [TestFixture]
    public class OpeningTimes_Tests
    {

        #region OpeningTimes_Test1()

        /// <summary>
        /// "Monday 07:00h - 21:00h"
        /// </summary>
        [Test]
        public void OpeningTimes_Test1()
        {

            if (!OpeningTimes.TryParse("Monday 07:00h - 21:00h", out var openingTimes, out var errorResponse))
            {
                Assert.Fail($"Could not parse '\"Monday 07:00h - 21:00h\"': {errorResponse}");
                return;
            }

            Assert.That(openingTimes,                                                      Is.Not.Null);
            Assert.That(openingTimes.ToJSON().ToString(Newtonsoft.Json.Formatting.None),   Is.EqualTo("{\"24/7\":false,\"regularOpenings\":[{\"monday\":[{\"begin\":\"07:00\",\"end\":\"21:00\"}]}]}"));

            Assert.That(openingTimes.AsFreeText(),                                         Is.EqualTo("Monday 07:00h - 21:00h"));

        }

        #endregion

        #region OpeningTimes_Test2()

        /// <summary>
        /// "Monday - Sunday 06:00h - 21:00h"
        /// </summary>
        [Test]
        public void OpeningTimes_Test2()
        {

            if (!OpeningTimes.TryParse("Monday - Sunday 06:00h - 21:00h", out var openingTimes, out var errorResponse))
            {
                Assert.Fail($"Could not parse '\"Monday - Sunday 06:00h - 21:00h\"': {errorResponse}");
                return;
            }

            Assert.That(openingTimes,                                                      Is.Not.Null);
            Assert.That(openingTimes.ToJSON().ToString(Newtonsoft.Json.Formatting.None),   Is.EqualTo("{\"24/7\":false,\"regularOpenings\":[{\"monday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]},{\"tuesday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]},{\"wednesday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]},{\"thursday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]},{\"friday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]},{\"saturday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]},{\"sunday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]}]}"));

            Assert.That(openingTimes.AsFreeText(),                                         Is.EqualTo("Monday 06:00h - 21:00h; Tuesday 06:00h - 21:00h; Wednesday 06:00h - 21:00h; Thursday 06:00h - 21:00h; Friday 06:00h - 21:00h; Saturday 06:00h - 21:00h; Sunday 06:00h - 21:00h"));

        }

        #endregion

        #region OpeningTimes_Test3()

        /// <summary>
        /// "Sunday - Tuesday 06:00h - 21:00h"
        /// </summary>
        [Test]
        public void OpeningTimes_Test3()
        {

            if (!OpeningTimes.TryParse("Sunday - Tuesday 06:00h - 21:00h", out var openingTimes, out var errorResponse))
            {
                Assert.Fail($"Could not parse '\"Sunday - Tuesday 06:00h - 21:00h\"': {errorResponse}");
                return;
            }

            Assert.That(openingTimes,                                                      Is.Not.Null);
            Assert.That(openingTimes.ToJSON().ToString(Newtonsoft.Json.Formatting.None),   Is.EqualTo("{\"24/7\":false,\"regularOpenings\":[{\"sunday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]},{\"monday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]},{\"tuesday\":[{\"begin\":\"06:00\",\"end\":\"21:00\"}]}]}"));

            Assert.That(openingTimes.AsFreeText(),                                         Is.EqualTo("Sunday 06:00h - 21:00h; Monday 06:00h - 21:00h; Tuesday 06:00h - 21:00h"));

        }

        #endregion


        #region OpeningTimes_TestX()

        /// <summary>
        /// "Monday 22:00h - 06:00h"
        /// </summary>
        [Test]
        public void OpeningTimes_TestX()
        {

            if (!OpeningTimes.TryParse("Monday 22:00h - 06:00h", out var openingTimes, out var errorResponse))
            {
                Assert.Fail($"Could not parse '\"Monday 22:00h - 06:00h\"': {errorResponse}");
                return;
            }

            Assert.That(openingTimes,                                                      Is.Not.Null);
            Assert.That(openingTimes.ToJSON().ToString(Newtonsoft.Json.Formatting.None),   Is.EqualTo("{\"24/7\":false,\"regularOpenings\":[{\"monday\":[{\"begin\":\"22:00\",\"end\":\"00:00\"}]},{\"tuesday\":[{\"begin\":\"00:00\",\"end\":\"06:00\"}]}]}"));

            Assert.That(openingTimes.AsFreeText(),                                         Is.EqualTo("Monday 22:00h - 00:00h; Tuesday 00:00h - 06:00h"));

        }

        #endregion


        #region OpeningTimes_TestY()

        /// <summary>
        /// "Monday 07:00h - 07:00h"
        /// </summary>
        [Test]
        public void OpeningTimes_TestY()
        {

            OpeningTimes.TryParse("Monday 07:00h - 07:00h", out var openingTimes, out var errorResponse);

            Assert.That(openingTimes,    Is.Null);
            Assert.That(errorResponse,   Is.EqualTo("Invalid hours 'Monday 07:00h - 07:00h'!"));

        }

        #endregion



        //ToDo: Monday - Sunday closed


    }

}
