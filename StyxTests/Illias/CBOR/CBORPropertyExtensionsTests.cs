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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Tests for the ParseMandatory/ParseOptional extension methods over
    /// CBOR maps, replicating the exact contract of the JSON
    /// parsing extension methods: A missing or null OPTIONAL property
    /// returns false WITHOUT an error response; a present but invalid
    /// one returns true WITH an error response.
    /// </summary>
    [TestFixture]
    public class CBORPropertyExtensionsTests
    {

        #region Data

        // {"text": "hello", "number": 42, "nothing": null, "flag": true,
        //  "blob": h'0102', 5: "five", -1: 7, "when": 1(1363896240), "amount": 4([-2, 500])}
        private static CBORValue TestMap()

            => new CBORMap {
                   { "text",     "hello" },
                   { "number",   42 },
                   { "nothing",  CBORValue.Null },
                   { "flag",     true },
                   { "blob",     CBORValue.FromBytes([ 1, 2 ]) },
                   { 5,          "five" },
                   { -1,         7 },
                   { "when",     CBORValue.FromUInt64(1363896240).WithTag(CBORTag.EpochDateTime) },
                   { "amount",   CBORValue.FromDecimal(5.00m) }
               };

        #endregion


        #region ParseMandatory_fails_with_an_error_when_the_property_is_absent()

        [Test]
        public void ParseMandatory_fails_with_an_error_when_the_property_is_absent()
        {

            var success = TestMap().ParseMandatoryText("missing",
                                                       "some text",
                                                       out var text,
                                                       out var errorResponse);

            Assert.That(success,        Is.False);
            Assert.That(text,           Is.Null);
            Assert.That(errorResponse,  Is.EqualTo("Missing CBOR property 'missing'!"));

        }

        #endregion

        #region ParseOptional_returns_false_without_an_error_when_the_property_is_absent()

        [Test]
        public void ParseOptional_returns_false_without_an_error_when_the_property_is_absent()
        {

            var success = TestMap().ParseOptionalText("missing",
                                                      "some text",
                                                      out var text,
                                                      out var errorResponse);

            // A missing OPTIONAL property is not an error!
            Assert.That(success,        Is.False);
            Assert.That(text,           Is.Null);
            Assert.That(errorResponse,  Is.Null);

        }

        #endregion

        #region ParseOptional_returns_false_without_an_error_when_the_property_is_null()

        [Test]
        public void ParseOptional_returns_false_without_an_error_when_the_property_is_null()
        {

            var success = TestMap().ParseOptionalText("nothing",
                                                      "some text",
                                                      out var text,
                                                      out var errorResponse);

            // Like JSON: an explicit null is ignored!
            Assert.That(success,        Is.False);
            Assert.That(text,           Is.Null);
            Assert.That(errorResponse,  Is.Null);

        }

        #endregion

        #region ParseOptional_returns_true_with_an_error_when_the_property_is_invalid()

        [Test]
        public void ParseOptional_returns_true_with_an_error_when_the_property_is_invalid()
        {

            // "number" is an integer, not a text...
            var success = TestMap().ParseOptionalText("number",
                                                      "some text",
                                                      out var text,
                                                      out var errorResponse);

            // Like JSON: present, but invalid!
            Assert.That(success,        Is.True);
            Assert.That(text,           Is.Null);
            Assert.That(errorResponse,  Does.StartWith("CBOR property 'number' (some text) could not be parsed:"));

        }

        #endregion

        #region ParseOptional_returns_true_when_the_property_is_valid()

        [Test]
        public void ParseOptional_returns_true_when_the_property_is_valid()
        {

            var success = TestMap().ParseOptionalText("text",
                                                      "some text",
                                                      out var text,
                                                      out var errorResponse);

            Assert.That(success,        Is.True);
            Assert.That(text,           Is.EqualTo("hello"));
            Assert.That(errorResponse,  Is.Null);

        }

        #endregion

        #region The_four_way_contract_also_holds_for_integer_keys()

        [Test]
        public void The_four_way_contract_also_holds_for_integer_keys()
        {

            var map = TestMap();

            // Valid...
            Assert.That(map.ParseMandatoryText(5, "the five", out var five, out var errorResponse1),  Is.True);
            Assert.That(five,            Is.EqualTo("five"));
            Assert.That(errorResponse1,  Is.Null);

            Assert.That(map.ParseOptionalUInt64(-1, "seven", out var seven, out var errorResponse2),  Is.True);
            Assert.That(seven,           Is.EqualTo(7));
            Assert.That(errorResponse2,  Is.Null);

            // Absent...
            Assert.That(map.ParseMandatoryText(99, "something", out _, out var errorResponse3),       Is.False);
            Assert.That(errorResponse3,  Is.EqualTo("Missing CBOR property '99'!"));

            Assert.That(map.ParseOptionalText(99, "something", out _, out var errorResponse4),        Is.False);
            Assert.That(errorResponse4,  Is.Null);

            // Invalid...
            Assert.That(map.ParseOptionalText(-1, "something", out _, out var errorResponse5),        Is.True);
            Assert.That(errorResponse5,  Does.Contain("could not be parsed"));

        }

        #endregion

        #region Typed_helpers_parse_numbers_bytes_timestamps_and_enums()

        [Test]
        public void Typed_helpers_parse_numbers_bytes_timestamps_and_enums()
        {

            var map = TestMap();

            Assert.That(map.ParseMandatoryUInt64("number", "a number", out var number, out _),        Is.True);
            Assert.That(number,     Is.EqualTo(42));

            Assert.That(map.ParseMandatoryBoolean("flag", "a flag", out var flag, out _),             Is.True);
            Assert.That(flag,       Is.True);

            Assert.That(map.ParseMandatoryBytes("blob", "a blob", out var blob, out _),               Is.True);
            Assert.That(blob,       Is.EqualTo(new Byte[] { 1, 2 }));

            Assert.That(map.ParseMandatoryTimestamp("when", "a timestamp", out var when, out _),      Is.True);
            Assert.That(when,       Is.EqualTo(new DateTimeOffset(2013, 3, 21, 20, 4, 0, TimeSpan.Zero)));

            Assert.That(map.ParseMandatoryDecimal("amount", "an amount", out var amount, out _),      Is.True);
            Assert.That(amount,        Is.EqualTo(5.00m));
            Assert.That(amount.Scale,  Is.EqualTo(2));

            Assert.That(map.ParseMandatoryEnum<DayOfWeek>("number", "a weekday", out _, out var enumError),  Is.False);
            Assert.That(enumError,  Does.StartWith("Invalid 'a weekday'"));

            var weekdayMap = new CBORMap {
                                 { "day",       "Friday" },
                                 { "dayNumber", 5 }
                             }.ToValue();

            Assert.That(weekdayMap.ParseMandatoryEnum<DayOfWeek>("day",       "a weekday", out var day1, out _),  Is.True);
            Assert.That(day1,  Is.EqualTo(DayOfWeek.Friday));

            Assert.That(weekdayMap.ParseMandatoryEnum<DayOfWeek>("dayNumber", "a weekday", out var day2, out _),  Is.True);
            Assert.That(day2,  Is.EqualTo(DayOfWeek.Friday));

        }

        #endregion

        #region Generic_TryCBORParser_delegates_compose()

        [Test]
        public void Generic_TryCBORParser_delegates_compose()
        {

            var map = TestMap();

            static Boolean TryParseUpperCase(CBORValue                                     Input,
                                             [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]  out String?  Result,
                                             [System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out String?  ErrorResponse)
            {

                if (Input.TryGetText(out var text))
                {
                    Result         = text.ToUpperInvariant();
                    ErrorResponse  = null;
                    return true;
                }

                Result         = null;
                ErrorResponse  = "The CBOR value is not a text string!";
                return false;

            }

            Assert.That(map.ParseMandatory<String>("text", "some text", TryParseUpperCase, out var upper, out _),  Is.True);
            Assert.That(upper,  Is.EqualTo("HELLO"));

            Assert.That(map.ParseOptional<String>("number", "some text", TryParseUpperCase, out _, out var errorResponse),  Is.True);
            Assert.That(errorResponse,  Does.Contain("not a text string"));

            Assert.That(map.ParseOptional<String>("missing", "some text", TryParseUpperCase, out _, out var errorResponse2),  Is.False);
            Assert.That(errorResponse2,  Is.Null);

        }

        #endregion

        #region Text_parsers_like_an_identifications_TryParse_compose()

        [Test]
        public void Text_parsers_like_an_identifications_TryParse_compose()
        {

            var map = new CBORMap {
                          { "id",      "8c5d5e2c-4b1f-4f43-9d3e-0e6a1c2b3d4e" },
                          { "badId",   "not-a-guid" },
                          { "number",  42 },
                          { 5,         "0e6a1c2b-3d4e-4f43-9d3e-8c5d5e2c4b1f" }
                      }.ToValue();

            using (Assert.EnterMultipleScope())
            {

                // Valid...
                Assert.That(map.ParseMandatory("id", "an identification", Guid.TryParse, out Guid id, out var errorResponse1),  Is.True);
                Assert.That(id,              Is.EqualTo(Guid.Parse("8c5d5e2c-4b1f-4f43-9d3e-0e6a1c2b3d4e")));
                Assert.That(errorResponse1,  Is.Null);

                Assert.That(map.ParseMandatory(5, "an identification", Guid.TryParse, out Guid five, out _),  Is.True);
                Assert.That(five,            Is.EqualTo(Guid.Parse("0e6a1c2b-3d4e-4f43-9d3e-8c5d5e2c4b1f")));

                // Invalid text...
                Assert.That(map.ParseMandatory("badId", "an identification", Guid.TryParse, out Guid _, out var errorResponse2),  Is.False);
                Assert.That(errorResponse2,  Does.Contain("'not-a-guid' is not valid"));

                // Not a text...
                Assert.That(map.ParseOptional("number", "an identification", Guid.TryParse, out Guid _, out var errorResponse3),  Is.True);
                Assert.That(errorResponse3,  Does.Contain("not a text string"));

                // Missing...
                Assert.That(map.ParseMandatory("missing", "an identification", Guid.TryParse, out Guid _, out var errorResponse4),  Is.False);
                Assert.That(errorResponse4,  Is.EqualTo("Missing CBOR property 'missing'!"));

                Assert.That(map.ParseOptional("missing", "an identification", Guid.TryParse, out Guid _, out var errorResponse5),  Is.False);
                Assert.That(errorResponse5,  Is.Null);

            }

        }

        #endregion

        #region NonMap_values_report_a_clear_error()

        [Test]
        public void NonMap_values_report_a_clear_error()
        {

            var array = CBORValue.FromArray(1, 2, 3);

            Assert.That(array.ParseMandatoryText("x", "some text", out _, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Is.EqualTo("The given CBOR value is not a map!"));

            Assert.That(array.ParseOptionalText("x", "some text", out _, out var errorResponse2),  Is.True);
            Assert.That(errorResponse2,  Is.EqualTo("The given CBOR value is not a map!"));

        }

        #endregion

    }

}
