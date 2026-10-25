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

using System.Numerics;
using System.Text.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Custom data tests.
    /// </summary>
    /// <remarks>
    /// Custom data was read as JSON and written again through JObject.Parse(), with the defaults of Newtonsoft: a number
    /// with a fraction became a double, 1.50 became 1.5, and a text that looks like a point in time a date,
    /// "2024-01-01T12:00:00Z" became "2024-01-01T12:00:00.0000000Z" in the canonical JSON; one with an offset was taken
    /// into the time zone of the machine. Two servers in different time zones held the same custom data otherwise.
    /// A number neither a 64-bit integer nor a decimal holds became a double, and a JSON object was read from the JSON
    /// text Newtonsoft writes of it, which writes the decimal 150 as 150.0.
    /// </remarks>
    [TestFixture]
    public class CustomData_Tests
    {

        #region (private static) Parse(Text)

        /// <summary>
        /// Read JSON text as hashing applications do: numbers with a fraction or an exponent as decimals, dates as text.
        /// </summary>
        private static JObject Parse(String Text)

            => (JObject) JToken.ReadFrom(new JsonTextReader(new StringReader(Text)) {
                                             FloatParseHandling  = FloatParseHandling.Decimal,
                                             DateParseHandling   = DateParseHandling.None
                                         });

        #endregion

        #region (private static) Canonical(Text)

        /// <summary>
        /// The canonical JSON of the given JSON text, with every number and every text exactly as written.
        /// </summary>
        private static String Canonical(String Text)
        {

            using var document = JsonDocument.Parse(Text);

            return CanonicalJSON.Serialize(document);

        }

        #endregion

        #region (private static) CBORHex(JSON)

        private static String CBORHex(JToken JSON)

            => Convert.ToHexStringLower(CBORJSON.ToCBOR(JSON).ToByteArray(CBORWriterOptions.Canonical));

        #endregion


        #region NumbersWithAFractionKeepTheirDecimalPlaces()

        [Test]
        public void NumbersWithAFractionKeepTheirDecimalPlaces()
        {

            var text        = """{"price":1.50,"tenth":0.1,"negative":-12.50,"one":1.0,"smallest":0.0000000000000000000000000001}""";
            var customData  = CustomDataNew.ParseJSON(text);
            var json        = customData.ToJObject();

            Assert.That(Canonical(customData.ToJSONString()),  Is.EqualTo("""{"negative":-12.50,"one":1.0,"price":1.50,"smallest":0.0000000000000000000000000001,"tenth":0.1}"""));
            Assert.That(CanonicalJSON.Serialize(json),         Is.EqualTo(Canonical(text)));

            Assert.That(json["price"]!.Type,                   Is.EqualTo(JTokenType.Float));
            Assert.That(json["price"]!.Value<Decimal>(),       Is.EqualTo(1.50m));
            Assert.That(json["price"]!.Value<Decimal>().Scale, Is.EqualTo(2));
            Assert.That(json["tenth"]!.Value<Decimal>().Scale, Is.EqualTo(1));

        }

        #endregion

        #region LargeNumbersKeepAllTheirDigits()

        [Test]
        public void LargeNumbersKeepAllTheirDigits()
        {

            var text        = """{"int64":9223372036854775807,"uint64":18446744073709551615,"beyond64Bits":123456789012345678901234567890,"beyondADecimal":12345678901234567890123456789012345678901234567890,"maxDecimal":79228162514264337593543950335,"negative":-123456789012345678901234567890}""";
            var customData  = CustomDataNew.ParseJSON(text);
            var json        = customData.ToJObject();

            Assert.That(Canonical(customData.ToJSONString()),  Is.EqualTo(Canonical(text)));
            Assert.That(CanonicalJSON.Serialize(json),         Is.EqualTo(Canonical(text)));
            Assert.That(CanonicalJSON.Serialize(json),         Is.EqualTo(CanonicalJSON.Serialize(Parse(text))));
            Assert.That(CBORHex(json),                         Is.EqualTo(CBORHex(Parse(text))));

            Assert.That(json["maxDecimal"]!.Type,              Is.EqualTo(JTokenType.Integer));
            Assert.That(json["beyondADecimal"]!.Type,          Is.EqualTo(JTokenType.Integer));
            Assert.That(((JValue) json["beyondADecimal"]!).Value,
                        Is.EqualTo(BigInteger.Parse("12345678901234567890123456789012345678901234567890")));

        }

        #endregion

        #region NumbersNoDecimalHoldsAsWrittenAreKeptAsWritten()

        /// <summary>
        /// A number with an exponent, with more than 28 decimal places or with a mantissa above 96 bits is written as
        /// it was given; a decimal rounds it, 1e-30 to 0, a double even more. As a JSON object, it is what Newtonsoft
        /// reads with FloatParseHandling.Decimal, as hashing applications read it.
        /// </summary>
        [Test]
        public void NumbersNoDecimalHoldsAsWrittenAreKeptAsWritten()
        {

            var text        = """{"exponent":1.5e2,"upperCase":1E+2,"tiny":1e-30,"manyPlaces":0.1234567890123456789012345678901,"wideMantissa":7922816251426433759354395033.5,"negativeZero":-0.0}""";
            var customData  = CustomDataNew.ParseJSON(text);
            var json        = customData.ToJObject();

            Assert.That(Canonical(customData.ToJSONString()),  Is.EqualTo(Canonical(text)));
            Assert.That(CanonicalJSON.Serialize(json),         Is.EqualTo(CanonicalJSON.Serialize(Parse(text))));
            Assert.That(CBORHex(json),                         Is.EqualTo(CBORHex(Parse(text))));

            Assert.That(customData["tiny"]!.Value.Kind,        Is.EqualTo(CustomDataValueKind.Number));
            Assert.That(customData["tiny"]!.Value.StringValue, Is.EqualTo("1e-30"));

        }

        #endregion

        #region NumbersBeyondTheRangeOfADecimalAreDoublesAsJSONObjects()

        /// <summary>
        /// Newtonsoft reads no decimal beyond the range of a decimal: as a JSON object, such a number is the double
        /// Newtonsoft reads otherwise. Its JSON text is kept as written.
        /// </summary>
        [Test]
        public void NumbersBeyondTheRangeOfADecimalAreDoublesAsJSONObjects()
        {

            var customData  = CustomDataNew.ParseJSON("""{"huge":1e30}""");
            var json        = customData.ToJObject();

            Assert.That(customData.ToJSONString(),             Is.EqualTo("""{"huge":1e30}"""));
            Assert.That(json["huge"]!.Type,                    Is.EqualTo(JTokenType.Float));
            Assert.That(((JValue) json["huge"]!).Value,        Is.EqualTo(1e30));

        }

        #endregion

        #region TimestampsStayTexts()

        /// <summary>
        /// Texts that look like points in time stay the texts given, whatever the time zone of the machine: read with
        /// the defaults of Newtonsoft, one with an offset was taken into the local time zone, and of the offsets
        /// +02:00, -05:00 and +14:00 at least two are not the one of the machine running this test.
        /// </summary>
        [Test]
        public void TimestampsStayTexts()
        {

            var text        = """{"utc":"2024-01-01T12:00:00Z","east":"2024-01-01T12:00:00+02:00","west":"2024-01-01T12:00:00-05:00","farEast":"2024-01-01T12:00:00+14:00","fraction":"2024-01-01T12:00:00.120Z","date":"2024-01-01","nested":{"at":"2024-06-30T23:59:59+01:00","list":["2024-01-01T00:00:00Z",1.50]}}""";
            var customData  = CustomDataNew.ParseJSON(text);
            var json        = customData.ToJObject();

            Assert.That(Canonical(customData.ToJSONString()),  Is.EqualTo(Canonical(text)));
            Assert.That(CanonicalJSON.Serialize(json),         Is.EqualTo(Canonical(text)));
            Assert.That(CBORHex(json),                         Is.EqualTo(CBORHex(Parse(text))));

            foreach (var (property, given) in new[] {
                                                  ("utc",       "2024-01-01T12:00:00Z"),
                                                  ("east",      "2024-01-01T12:00:00+02:00"),
                                                  ("west",      "2024-01-01T12:00:00-05:00"),
                                                  ("farEast",   "2024-01-01T12:00:00+14:00"),
                                                  ("fraction",  "2024-01-01T12:00:00.120Z")
                                              })
            {
                Assert.That(json[property]!.Type,              Is.EqualTo(JTokenType.String), property);
                Assert.That(json[property]!.Value<String>(),   Is.EqualTo(given),             property);
            }

            Assert.That(json["nested"]!["at"]!.Type,           Is.EqualTo(JTokenType.String));
            Assert.That(json["nested"]!["list"]![0]!.Type,     Is.EqualTo(JTokenType.String));

        }

        #endregion

        #region AJSONObjectKeepsItsCanonicalJSON()

        /// <summary>
        /// A JSON object read as hashing applications read JSON text is the same JSON object, with the same canonical
        /// JSON and CBOR, after it was custom data: also the decimal 150 of 1.5e2, which Newtonsoft writes as 150.0.
        /// </summary>
        [Test]
        public void AJSONObjectKeepsItsCanonicalJSON()
        {

            var given       = Parse("""{"vendor":{"tier":2,"rate":0.390,"exponent":1.5e2,"since":"2024-01-01T12:00:00+02:00","verified":true,"none":null},"tags":["fast",7,1.50,{"at":"2024-01-01T12:00:00Z"}],"big":123456789012345678901234567890}""");
            var customData  = CustomDataNew.ParseJObject(given);
            var json        = customData.ToJObject();

            Assert.That(CanonicalJSON.Serialize(json),         Is.EqualTo(CanonicalJSON.Serialize(given)));
            Assert.That(CBORHex(json),                         Is.EqualTo(CBORHex(given)));
            Assert.That(JToken.DeepEquals(json, given),        Is.True);

            Assert.That(CanonicalJSON.Serialize(json),         Is.EqualTo("""{"big":123456789012345678901234567890,"tags":["fast",7,1.50,{"at":"2024-01-01T12:00:00Z"}],"vendor":{"exponent":150,"none":null,"rate":0.390,"since":"2024-01-01T12:00:00+02:00","tier":2,"verified":true}}"""));

        }

        #endregion

        #region AJSONObjectMadeInCodeKeepsItsCanonicalJSON()

        [Test]
        public void AJSONObjectMadeInCodeKeepsItsCanonicalJSON()
        {

            var given       = new JObject(
                                  new JProperty("amount",    150m),
                                  new JProperty("rate",      0.39m),
                                  new JProperty("factor",    0.75),
                                  new JProperty("count",     3),
                                  new JProperty("unsigned",  UInt64.MaxValue),
                                  new JProperty("validFrom", "2026-01-01T00:00:00Z")
                              );

            var customData  = CustomDataNew.ParseJObject(given);
            var json        = customData.ToJObject();

            Assert.That(CanonicalJSON.Serialize(json),         Is.EqualTo(CanonicalJSON.Serialize(given)));
            Assert.That(CBORHex(json),                         Is.EqualTo(CBORHex(given)));

            Assert.That(CanonicalJSON.Serialize(json),         Is.EqualTo("""{"amount":150,"count":3,"factor":0.75,"rate":0.39,"unsigned":18446744073709551615,"validFrom":"2026-01-01T00:00:00Z"}"""));
            Assert.That(json["factor"]!.Type,                  Is.EqualTo(JTokenType.Float));
            Assert.That(((JValue) json["factor"]!).Value,      Is.EqualTo(0.75));

        }

        #endregion

        #region TheImplicitJObjectIsToJObject()

        [Test]
        public void TheImplicitJObjectIsToJObject()
        {

            var customData  = CustomDataNew.ParseJSON("""{"price":1.50,"at":"2024-01-01T12:00:00+02:00"}""");
            JObject json    = customData;

            Assert.That(JToken.DeepEquals(json, customData.ToJObject()),  Is.True);
            Assert.That(json["at"]!.Type,                                 Is.EqualTo(JTokenType.String));

        }

        #endregion

    }

}
