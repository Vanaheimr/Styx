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

using System.Text.Json.Nodes;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The specification's own test vectors, executed against this library.
    ///
    /// The specification of tag 44252 carries a machine-readable conformance
    /// annex (test-vectors/ next to it): golden encodings, must-reject
    /// inputs, canonical text renderings and the exact JSON conversion. This
    /// suite runs every normative entry; entries the specification
    /// deliberately leaves open are classed "survey" there and are not
    /// judged here - the cross-implementation conformance suite observes
    /// those.
    ///
    /// The annex is looked for in three places, first match wins: the
    /// MCBOR_TEST_VECTORS environment variable, spec/test-vectors/ in this
    /// repository (git-ignored; CI fetches it), and the sibling checkout of
    /// the Whitepapers repository that the conformance suite's layout
    /// provides. Where none exists, the suite reports itself as ignored
    /// rather than passing vacuously.
    /// </summary>
    [TestFixture]
    public class SpecificationVectorTests
    {

        #region Locating the annex

        private static readonly String? annexDirectory = LocateAnnex();

        private static String? LocateAnnex()
        {

            var candidates = new List<String>();

            var fromEnvironment = Environment.GetEnvironmentVariable("MCBOR_TEST_VECTORS");

            if (fromEnvironment is not null)
                candidates.Add(fromEnvironment);

            // Walk up from the test assembly to the repository root, marked
            // by the solution file.
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Styx.slnx")))
                directory = directory.Parent!;

            if (directory is not null)
            {

                candidates.Add(Path.Combine(directory.FullName, "spec", "test-vectors"));

                // The layout of the conformance suite: this repository and the
                // Whitepapers repository are sibling submodules.
                candidates.Add(Path.Combine(directory.FullName, "..", "specification", "MetrologicalCBOR", "test-vectors"));

            }

            foreach (var candidate in candidates)
            {
                if (File.Exists(Path.Combine(candidate, "values.json")))
                    return Path.GetFullPath(candidate);
            }

            return null;

        }


        private static IEnumerable<JsonObject> Cases(String FileName)
        {

            if (annexDirectory is null)
                yield break;

            var document = JsonNode.Parse(File.ReadAllText(Path.Combine(annexDirectory, FileName)))!.AsObject();

            foreach (var entry in document["cases"]!.AsArray())
                yield return entry!.AsObject();

        }

        private static IEnumerable<TestCaseData> CaseSource(String FileName, Func<JsonObject, Boolean>? Filter = null)

            => Cases(FileName)
                   .Where(testCase => Filter is null || Filter(testCase))
                   .Select(testCase => new TestCaseData(testCase)
                               .SetArgDisplayNames(testCase["id"]!.GetValue<String>()));


        public static IEnumerable<TestCaseData> ValuesCases()
            => CaseSource("values.json");

        public static IEnumerable<TestCaseData> InvalidCases()
            => CaseSource("values-invalid.json",
                          testCase => testCase["expect"]?.GetValue<String>() != "survey");

        public static IEnumerable<TestCaseData> DocumentCases()
            => CaseSource("documents.json");

        public static IEnumerable<TestCaseData> JSONToCBORCases()
            => CaseSource("json-to-cbor.json",
                          testCase => testCase["cborHex"] is not null &&
                                      testCase["class"]?.GetValue<String>() != "survey");

        #endregion


        #region The_annex_was_found_or_this_suite_is_idle()

        [Test]
        public void The_annex_was_found_or_this_suite_is_idle()
        {

            if (annexDirectory is null)
                Assert.Ignore("The specification's test-vectors annex was not found. " +
                              "Set MCBOR_TEST_VECTORS, fetch the annex to spec/test-vectors/, " +
                              "or check this repository out next to the Whitepapers repository.");

            Assert.That(ValuesCases(), Is.Not.Empty);

        }

        #endregion


        #region Values(TestCase)

        [TestCaseSource(nameof(ValuesCases))]
        public void Values(JsonObject TestCase)
        {

            var hex        = TestCase["hex"]!.GetValue<String>();
            var canonical  = TestCase["canonicalHex"]?.GetValue<String>() ?? hex;

            // The decoder accepts the vector...
            Assert.That(MetrologicalValue.TryParse(CBORValue.Parse(Convert.FromHexString(hex)),
                                                   out var decoded,
                                                   out var errorResponse),
                        Is.True,
                        errorResponse);

            // ...re-encodes it canonically...
            if (TestCase["canonicalHexClass"]?.GetValue<String>() != "survey")
                Assert.That(Convert.ToHexString(decoded.ToCBOR().ToByteArray(CBORWriterOptions.Canonical)),
                            Is.EqualTo(canonical));

            // ...and its text form is a second encoding of it.
            if (TestCase["text"] is not null &&
                TestCase["textClass"]?.GetValue<String>() != "survey")
            {

                var text = TestCase["text"]!.GetValue<String>();

                Assert.That(decoded.ToString(),  Is.EqualTo(text));
                Assert.That(ParseToHex(text),    Is.EqualTo(canonical), text);

            }

            if (TestCase["parseTexts"] is JsonArray parseTexts)
            {
                foreach (var entry in parseTexts)
                {

                    var expectation = entry!["expect"]!.GetValue<String>();

                    if (expectation == "survey")
                        continue;

                    var text = entry["text"]!.GetValue<String>();

                    if (expectation == "reject")
                    {
                        Assert.That(MetrologicalValue.TryParse(text, out _, out _),
                                    Is.False,
                                    $"'{text}' was accepted as a metrological value!");
                        continue;
                    }

                    Assert.That(ParseToHex(text),
                                Is.EqualTo(entry["hex"]?.GetValue<String>() ?? canonical),
                                text);

                }
            }

        }

        #endregion

        #region Rejects(TestCase)

        [TestCaseSource(nameof(InvalidCases))]
        public void Rejects(JsonObject TestCase)
        {

            var reason = TestCase["reason"]!.GetValue<String>();

            if (TestCase["hex"] is not null)
            {

                var accepted = CBORValue.TryParse(Convert.FromHexString(TestCase["hex"]!.GetValue<String>()),
                                                  out var cbor,
                                                  out _) &&
                               MetrologicalValue.TryParse(cbor, out _, out _);

                Assert.That(accepted, Is.False, reason);

            }

            if (TestCase["text"] is not null)
                Assert.That(MetrologicalValue.TryParse(TestCase["text"]!.GetValue<String>(), out _, out _),
                            Is.False,
                            reason);

        }

        #endregion

        #region Documents(TestCase)

        [TestCaseSource(nameof(DocumentCases))]
        public void Documents(JsonObject TestCase)
        {

            var cborHex = TestCase["cborHex"]!.GetValue<String>();

            if (TestCase["expectToJsonError"]?.GetValue<Boolean>() == true)
            {
                Assert.That(() => CBORJSON.ToJSONText(Convert.FromHexString(cborHex)),
                            Throws.Exception);
                return;
            }

            var json = CBORJSON.ToJSONText(Convert.FromHexString(cborHex));

            if (TestCase["json"] is not null &&
                TestCase["jsonClass"]?.GetValue<String>() != "survey")
            {
                Assert.That(json, Is.EqualTo(TestCase["json"]!.GetValue<String>()));
            }

            if (TestCase["roundtripHex"] is not null)
                Assert.That(JSONToHex(json), Is.EqualTo(TestCase["roundtripHex"]!.GetValue<String>()));

            else if (TestCase["roundtrip"]?.GetValueKind() == System.Text.Json.JsonValueKind.True)
                Assert.That(JSONToHex(json), Is.EqualTo(cborHex));

        }

        #endregion

        #region JSONToCBOR(TestCase)

        [TestCaseSource(nameof(JSONToCBORCases))]
        public void JSONToCBOR(JsonObject TestCase)
        {

            Assert.That(JSONToHex(TestCase["json"]!.GetValue<String>()),
                        Is.EqualTo(TestCase["cborHex"]!.GetValue<String>()));

        }

        #endregion


        #region (private static) ParseToHex(Text)

        private static String ParseToHex(String Text)
        {

            Assert.That(MetrologicalValue.TryParse(Text, out var value, out var errorResponse),
                        Is.True,
                        $"'{Text}': {errorResponse}");

            return Convert.ToHexString(value.ToCBOR().ToByteArray(CBORWriterOptions.Canonical));

        }

        #endregion

        #region (private static) AsSpecified

        /// <summary>
        /// The conversion metrological-text.md Section 3 describes: a string
        /// that reads as a reading becomes one. It is not the default - the
        /// default guesses nothing about somebody else's document - so a
        /// vector that asserts the specified conversion asks for it.
        /// </summary>
        private static readonly CBORJSONOptions AsSpecified = new () {
                                                                  Readings = CBORJSONReadings.Auto
                                                              };

        #endregion

        #region (private static) JSONToHex (JSONText)

        private static String JSONToHex(String JSONText)

            => Convert.ToHexString(
                   CBORJSON.ToCBOR(JSONText, AsSpecified).ToByteArray(CBORWriterOptions.Canonical)
               );

        #endregion

    }

}
