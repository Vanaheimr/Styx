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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Every ParseOptional keeps one contract: true while the property is
    /// there - with an error response while its value is not valid - and false
    /// while it is not there or null, without an error response.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nine of them did not. A value not valid returned false - the overloads
    /// for a Boolean?, ParseOptionalEnums, ParseOptionalStruct of an
    /// IEnumerable, ParseOptionalJSON and ParseOptionalJSONMayBeNull of a
    /// class, the one for a JArray - so that a parser asking as the others are
    /// asked, if (JSON.ParseOptional...(...)) { if (ErrorResponse is not null)
    /// return false; }, passed over the value not valid without a word.
    /// ParseOptionalEnums, ParseOptionalMS and ParseOptionalJSON of an
    /// IEnumerable said a property not there was an error, and some of them a
    /// null as well. And the one for a JArray took an array only while it was
    /// null as well - never.
    /// </para>
    /// <para>
    /// A parser can tell all of them alike: call, then refuse while the error
    /// response is not null.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class ParseOptionalContractTests
    {

        #region (private) Thing / AssertNotThere / AssertNotValid

        /// <summary>
        /// A class with an identification of its own, which it needs.
        /// </summary>
        private sealed class Thing(String Id)
        {

            public String Id { get; } = Id;

            public static Boolean TryParse(JObject                           JSON,
                                           [NotNullWhen(true)]  out Thing?   Thing,
                                           [NotNullWhen(false)] out String?  ErrorResponse)
            {

                Thing          = null;
                ErrorResponse  = null;

                if (JSON["id"]?.Type == JTokenType.String)
                {
                    Thing = new Thing(JSON["id"]!.Value<String>()!);
                    return true;
                }

                ErrorResponse = "A thing needs its identification!";
                return false;

            }

            public static Boolean TryParseMaybeNull(JObject                           JSON,
                                                    out Thing?                        Thing,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)

                => TryParse(JSON, out Thing, out ErrorResponse);

        }

        private delegate Boolean ParseOptional(JObject JSON, out String? ErrorResponse);

        /// <summary>
        /// Not there and null are both no value, and no error.
        /// </summary>
        private static void AssertNotThere(ParseOptional Parse)
        {

            var notThere = Parse(JObject.Parse("""{ "other": 1 }"""),     out var errorNotThere);
            var isNull   = Parse(JObject.Parse("""{ "value": null }"""),  out var errorNull);

            Assert.Multiple(() => {
                Assert.That(notThere,      Is.False, "A value not there is said to be there.");
                Assert.That(errorNotThere, Is.Null,  "A value not there is said to be an error.");
                Assert.That(isNull,        Is.False, "A value null is said to be there.");
                Assert.That(errorNull,     Is.Null,  "A value null is said to be an error.");
            });

        }

        /// <summary>
        /// A value not valid is there, and an error.
        /// </summary>
        private static void AssertNotValid(String JSON, ParseOptional Parse)
        {

            var there = Parse(JObject.Parse(JSON), out var error);

            Assert.Multiple(() => {
                Assert.That(there, Is.True,      $"A value not valid ({JSON}) is said not to be there.");
                Assert.That(error, Is.Not.Null, $"A value not valid ({JSON}) is no error.");
            });

        }

        #endregion


        #region Boolean?

        [Test]
        public void ParseOptionalBoolean_KeepsTheContract()
        {

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptional("value", "value", out Boolean? _, out error));
            AssertNotValid("""{ "value": "maybe" }""",         (JObject json, out String? error) => json.ParseOptional("value", "value", out Boolean? _, out error));

            Assert.That(JObject.Parse("""{ "value": false }""").ParseOptional("value", "value", out Boolean? value, out var error) && value == false && error is null, Is.True);

        }

        #endregion

        #region ParseOptionalEnum

        [Test]
        public void ParseOptionalEnum_KeepsTheContract()
        {

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptionalEnum("value", "value", out DayOfWeek? _, out error));
            AssertNotValid("""{ "value": "Funday" }""",        (JObject json, out String? error) => json.ParseOptionalEnum("value", "value", out DayOfWeek? _, out error));

            Assert.That(JObject.Parse("""{ "value": "Monday" }""").ParseOptionalEnum("value", "value", out DayOfWeek? value, out var error) && value == DayOfWeek.Monday && error is null, Is.True);

        }

        #endregion

        #region ParseOptionalEnums

        [Test]
        public void ParseOptionalEnums_KeepsTheContract()
        {

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptionalEnums("value", "value", out HashSet<DayOfWeek> _, out error));
            AssertNotValid("""{ "value": "Monday" }""",        (JObject json, out String? error) => json.ParseOptionalEnums("value", "value", out HashSet<DayOfWeek> _, out error));
            AssertNotValid("""{ "value": [ "Funday" ] }""",    (JObject json, out String? error) => json.ParseOptionalEnums("value", "value", out HashSet<DayOfWeek> _, out error));

            Assert.That(JObject.Parse("""{ "value": [ "Monday", "Friday" ] }""").ParseOptionalEnums("value", "value", out HashSet<DayOfWeek> values, out var error) && values.Count == 2 && error is null, Is.True);

        }

        #endregion

        #region ParseOptionalStruct of an IEnumerable

        [Test]
        public void ParseOptionalStructs_KeepsTheContract()
        {

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptionalStruct("value", "value", Int32.TryParse, out IEnumerable<Int32> _, out error));
            AssertNotValid("""{ "value": 1 }""",               (JObject json, out String? error) => json.ParseOptionalStruct("value", "value", Int32.TryParse, out IEnumerable<Int32> _, out error));
            AssertNotValid("""{ "value": [ "one" ] }""",       (JObject json, out String? error) => json.ParseOptionalStruct("value", "value", Int32.TryParse, out IEnumerable<Int32> _, out error));

            Assert.That(JObject.Parse("""{ "value": [ 1, 2 ] }""").ParseOptionalStruct("value", "value", Int32.TryParse, out IEnumerable<Int32> values, out var error) && values.Count() == 2 && error is null, Is.True);

        }

        #endregion

        #region ParseOptionalMS

        [Test]
        public void ParseOptionalMS_KeepsTheContract()
        {

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptionalMS("value", "value", out TimeSpan? _, out error));
            AssertNotValid("""{ "value": "long" }""",          (JObject json, out String? error) => json.ParseOptionalMS("value", "value", out TimeSpan? _, out error));

            Assert.That(JObject.Parse("""{ "value": 1500 }""").ParseOptionalMS("value", "value", out TimeSpan? value, out var error) && value == TimeSpan.FromMilliseconds(1500) && error is null, Is.True);

        }

        #endregion

        #region ParseOptionalJSON of a class

        [Test]
        public void ParseOptionalJSON_KeepsTheContract()
        {

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptionalJSON("value", "value", Thing.TryParse, out Thing? _, out error));
            AssertNotValid("""{ "value": "thing" }""",         (JObject json, out String? error) => json.ParseOptionalJSON("value", "value", Thing.TryParse, out Thing? _, out error));
            AssertNotValid("""{ "value": { } }""",             (JObject json, out String? error) => json.ParseOptionalJSON("value", "value", Thing.TryParse, out Thing? _, out error));

            Assert.That(JObject.Parse("""{ "value": { "id": "T1" } }""").ParseOptionalJSON("value", "value", Thing.TryParse, out Thing? value, out var error) && value?.Id == "T1" && error is null, Is.True);

        }

        #endregion

        #region ParseOptionalJSONMayBeNull

        [Test]
        public void ParseOptionalJSONMayBeNull_KeepsTheContract()
        {

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptionalJSONMayBeNull("value", "value", Thing.TryParseMaybeNull, out Thing? _, out error));
            AssertNotValid("""{ "value": "thing" }""",         (JObject json, out String? error) => json.ParseOptionalJSONMayBeNull("value", "value", Thing.TryParseMaybeNull, out Thing? _, out error));
            AssertNotValid("""{ "value": { } }""",             (JObject json, out String? error) => json.ParseOptionalJSONMayBeNull("value", "value", Thing.TryParseMaybeNull, out Thing? _, out error));

            Assert.That(JObject.Parse("""{ "value": { "id": "T1" } }""").ParseOptionalJSONMayBeNull("value", "value", Thing.TryParseMaybeNull, out Thing? value, out var error) && value?.Id == "T1" && error is null, Is.True);

        }

        #endregion

        #region ParseOptionalJSON of an IEnumerable

        [Test]
        public void ParseOptionalJSONs_KeepsTheContract()
        {

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptionalJSON("value", "value", Thing.TryParse, out IEnumerable<Thing> _, out error));
            AssertNotValid("""{ "value": "things" }""",        (JObject json, out String? error) => json.ParseOptionalJSON("value", "value", Thing.TryParse, out IEnumerable<Thing> _, out error));
            AssertNotValid("""{ "value": [ { } ] }""",         (JObject json, out String? error) => json.ParseOptionalJSON("value", "value", Thing.TryParse, out IEnumerable<Thing> _, out error));

            Assert.That(JObject.Parse("""{ "value": [ { "id": "T1" } ] }""").ParseOptionalJSON("value", "value", Thing.TryParse, out IEnumerable<Thing> values, out var error) && values.Count() == 1 && error is null, Is.True);

        }

        #endregion

        #region Mapper

        /// <summary>
        /// A mapper's null - as URL.TryParse(String) or Watt.TryParse(String)
        /// return for a text not valid - is a value not valid, not none.
        /// </summary>
        [Test]
        public void ParseOptionalMapper_KeepsTheContract()
        {

            static String? Mapper(String Text) => Text == "valid" ? "VALID" : null;

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptional("value", "value", (Func<String, String?>) Mapper, out String? _, out error));
            AssertNotValid("""{ "value": "not valid" }""",     (JObject json, out String? error) => json.ParseOptional("value", "value", (Func<String, String?>) Mapper, out String? _, out error));

            Assert.That(JObject.Parse("""{ "value": "valid" }""").ParseOptional("value", "value", (Func<String, String?>) Mapper, out String? value, out var error) && value == "VALID" && error is null, Is.True);

        }

        #endregion

        #region JArray

        [Test]
        public void ParseOptionalJArray_KeepsTheContract()
        {

            AssertNotThere(                                    (JObject json, out String? error) => json.ParseOptional("value", "value", out JArray _, out error));
            AssertNotValid("""{ "value": "array" }""",         (JObject json, out String? error) => json.ParseOptional("value", "value", out JArray _, out error));

            var there = JObject.Parse("""{ "value": [ 1, 2 ] }""").ParseOptional("value", "value", out JArray array, out var error);

            Assert.Multiple(() => {
                Assert.That(there,       Is.True,  "An array there is said not to be.");
                Assert.That(array.Count, Is.EqualTo(2), "An array there is not taken.");
                Assert.That(error,       Is.Null,  "An array there is said to be an error.");
            });

        }

        #endregion

    }

}
