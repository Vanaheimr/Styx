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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.Illias
{

    /// <summary>
    /// Unit tests for the JObject.ParseMandatory / ParseOptional extension
    /// methods and their [NotNullWhen] out-parameter contracts.
    /// </summary>
    [TestFixture]
    public class JSONParserTests
    {

        #region (private) UpperCaseParser(Input, out Result)

        // A minimal TryParser<String> that upper-cases the raw value; the
        // [NotNullWhen(true)] matches the delegate contract exactly.
        private static Boolean UpperCaseParser(String Input, [NotNullWhen(true)] out String? Result)
        {
            Result = Input.ToUpper();
            return true;
        }

        #endregion


        #region ParseMandatoryText_Present_ReturnsTrueAndValue()

        [Test]
        public void ParseMandatoryText_Present_ReturnsTrueAndValue()
        {

            var json = JObject.Parse("""{ "name": "Berlin" }""");

            var success = json.ParseMandatoryText("name",
                                                  "the name",
                                                  out var text,
                                                  out var errorResponse);

            Assert.That(success,       Is.True);
            Assert.That(text,          Is.EqualTo("Berlin"));   // non-null on success
            Assert.That(errorResponse, Is.Null);

        }

        #endregion

        #region ParseMandatoryText_Missing_ReturnsFalseAndError()

        [Test]
        public void ParseMandatoryText_Missing_ReturnsFalseAndError()
        {

            var json = JObject.Parse("""{ "other": "value" }""");

            var success = json.ParseMandatoryText("name",
                                                  "the name",
                                                  out var text,
                                                  out var errorResponse);

            Assert.That(success,       Is.False);
            Assert.That(text,          Is.Null);
            Assert.That(errorResponse, Is.Not.Null);   // non-null on failure

        }

        #endregion

        #region ParseMandatoryText_NullJSON_ReturnsFalseAndError()

        [Test]
        public void ParseMandatoryText_NullJSON_ReturnsFalseAndError()
        {

            JObject? json = null;

            var success = json!.ParseMandatoryText("name",
                                                   "the name",
                                                   out var text,
                                                   out var errorResponse);

            Assert.That(success,       Is.False);
            Assert.That(text,          Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region ParseMandatory_WithTryParser_Present_ReturnsMappedValue()

        [Test]
        public void ParseMandatory_WithTryParser_Present_ReturnsMappedValue()
        {

            var json = JObject.Parse("""{ "code": "de" }""");

            var success = json.ParseMandatory("code",
                                              "the code",
                                              UpperCaseParser,
                                              out String? value,
                                              out var     errorResponse);

            Assert.That(success,       Is.True);
            Assert.That(value,         Is.EqualTo("DE"));
            Assert.That(errorResponse, Is.Null);

        }

        #endregion

        #region ParseMandatory_WithTryParser_Missing_ReturnsFalseAndError()

        [Test]
        public void ParseMandatory_WithTryParser_Missing_ReturnsFalseAndError()
        {

            var json = JObject.Parse("""{ "other": "value" }""");

            var success = json.ParseMandatory("code",
                                              "the code",
                                              UpperCaseParser,
                                              out String? value,
                                              out var     errorResponse);

            Assert.That(success,       Is.False);
            Assert.That(value,         Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region ParseOptional_Present_ReturnsTrueAndValue()

        [Test]
        public void ParseOptional_Present_ReturnsTrueAndValue()
        {

            var json = JObject.Parse("""{ "description": "hello" }""");

            var found = json.ParseOptional("description",
                                           "the description",
                                           out String? value,
                                           out var     errorResponse);

            Assert.That(found,         Is.True);
            Assert.That(value,         Is.EqualTo("hello"));
            Assert.That(errorResponse, Is.Null);

        }

        #endregion

        #region ParseOptional_Missing_ReturnsFalseWithoutError()

        [Test]
        public void ParseOptional_Missing_ReturnsFalseWithoutError()
        {

            var json = JObject.Parse("""{ "other": "value" }""");

            var found = json.ParseOptional("description",
                                           "the description",
                                           out String? value,
                                           out var     errorResponse);

            Assert.That(found,         Is.False);   // absent => not found
            Assert.That(value,         Is.Null);
            Assert.That(errorResponse, Is.Null);    // absence is not an error

        }

        #endregion

    }

}
