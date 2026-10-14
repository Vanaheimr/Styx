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

using System.Xml.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.XML
{

    /// <summary>
    /// Unit tests for the XElement extension methods.
    /// </summary>
    [TestFixture]
    public class XElementExtensionsTests
    {

        #region (struct) TestId

        /// <summary>
        /// A value type with the common pair of TryParse overloads.
        /// Given 'out TestId? x', C# binds the method group 'TestId.TryParse'
        /// to the Func&lt;String, T&gt; overload of TryParseOptional.
        /// </summary>
        private readonly struct TestId(Int32 Value)
        {

            public Int32 Value { get; } = Value;

            public static TestId? TryParse(String Text)

                => TryParse(Text, out var testId)
                       ? testId
                       : null;

            public static Boolean TryParse(String Text, out TestId TestId)
            {

                if (Int32.TryParse(Text, out var value) && value > 0)
                {
                    TestId = new TestId(value);
                    return true;
                }

                TestId = default;
                return false;

            }

        }

        #endregion


        #region TryParseOptional_Func_MissingElement()

        [Test]
        public void TryParseOptional_Func_MissingElement()
        {

            var xml = new XElement("parent");

            Assert.That(xml.TryParseOptional("id",
                                             "test identification",
                                             TestId.TryParse,
                                             out TestId? testId,
                                             out var errorResponse),
                        Is.True);

            Assert.That(testId,        Is.Null);
            Assert.That(errorResponse, Is.Null);

        }

        #endregion

        #region TryParseOptional_Func_ValidElement()

        [Test]
        public void TryParseOptional_Func_ValidElement()
        {

            var xml = new XElement("parent",
                          new XElement("id", " 23 "));

            Assert.That(xml.TryParseOptional("id",
                                             "test identification",
                                             TestId.TryParse,
                                             out TestId? testId,
                                             out var errorResponse),
                        Is.True);

            Assert.That(testId?.Value, Is.EqualTo(23));
            Assert.That(errorResponse, Is.Null);

        }

        #endregion

        #region TryParseOptional_Func_InvalidElement()

        [Test]
        public void TryParseOptional_Func_InvalidElement()
        {

            var xml = new XElement("parent",
                          new XElement("id", "-5"));

            Assert.That(xml.TryParseOptional("id",
                                             "test identification",
                                             TestId.TryParse,
                                             out TestId? testId,
                                             out var errorResponse),
                        Is.False);

            Assert.That(testId,        Is.Null);
            Assert.That(errorResponse, Is.EqualTo("Invalid optional 'test identification' value '-5'!"));

        }

        #endregion

        #region TryParseOptional_Func_ReferenceType()

        [Test]
        public void TryParseOptional_Func_ReferenceType()
        {

            static Uri? parseURL(String Text)
                => Uri.TryCreate(Text, UriKind.Absolute, out var url) ? url : null;

            var missing = new XElement("parent");
            var valid   = new XElement("parent", new XElement("url", "https://example.org/"));
            var invalid = new XElement("parent", new XElement("url", "not a url"));


            Assert.That(missing.TryParseOptional("url", "url", parseURL, out var url1, out var errorResponse1), Is.True);
            Assert.That(url1,                    Is.Null);
            Assert.That(errorResponse1,          Is.Null);

            Assert.That(valid.  TryParseOptional("url", "url", parseURL, out var url2, out var errorResponse2), Is.True);
            Assert.That(url2?.ToString(),        Is.EqualTo("https://example.org/"));
            Assert.That(errorResponse2,          Is.Null);

            Assert.That(invalid.TryParseOptional("url", "url", parseURL, out var url3, out var errorResponse3), Is.False);
            Assert.That(url3,                    Is.Null);
            Assert.That(errorResponse3,          Is.EqualTo("Invalid optional 'url' value 'not a url'!"));

        }

        #endregion


    }

}
