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
using System.Globalization;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Tests for the document-level conversion between CBOR and JSON, in
    /// which every metrological value (tag 44252) becomes one JSON string.
    /// </summary>
    [TestFixture]
    public class CBORJSONTests
    {

        #region (private static) MeterReading()

        /// <summary>
        /// A document of the shape this conversion exists for: a few
        /// measurements, a timestamp, some metadata.
        /// </summary>
        private static CBORValue MeterReading()
        {

            var readings = new CBORArray();

            readings.Add(new MetrologicalValue(1.10m,   UnitOfMeasure.WattHour, SIPrefix.Kilo).ToCBOR());
            readings.Add(new MetrologicalValue(230.00m, UnitOfMeasure.Volt,     SIPrefix.None,
                                               new MeasurementUncertainty(0.12m, CoverageFactor: 2)).ToCBOR());
            readings.Add(new MetrologicalValue(5.0m,    UnitOfMeasure.Ampere,   SIPrefix.Milli).ToCBOR());

            var document = new CBORMap();

            document.Add(CBORValue.FromText("meter"),      CBORValue.FromText("EVSE-42"));
            document.Add(CBORValue.FromText("timestamp"),  CBORValue.FromText("2026-08-18T10:15:00.000Z"));
            document.Add(CBORValue.FromText("readings"),   readings.ToValue());
            document.Add(CBORValue.FromText("valid"),      CBORValue.True);

            return document.ToValue();

        }

        #endregion

        #region (private static) BothPathsAgree(CBOR, Options = null)

        /// <summary>
        /// Convert the given document along both JSON paths and return the
        /// text they have to agree on.
        /// </summary>
        private static String BothPathsAgree(CBORValue         CBOR,
                                             CBORJSONOptions?  Options   = null)
        {

            var utf8Text  = CBORJSON.ToJSONText(CBOR, Options);
            var jtoken    = CBORJSON.ToJSON    (CBOR, Options);

            Assert.That(jtoken.ToString(Newtonsoft.Json.Formatting.None),
                        Is.EqualTo(utf8Text),
                        "The Newtonsoft tree and the UTF-8 text differ!");

            return utf8Text;

        }

        #endregion


        #region A_metrological_value_becomes_one_string()

        [Test]
        public void A_metrological_value_becomes_one_string()
        {

            var json = BothPathsAgree(MeterReading());

            Assert.That(json,
                        Is.EqualTo("{\"meter\":\"EVSE-42\"," +
                                    "\"timestamp\":\"2026-08-18T10:15:00.000Z\"," +
                                    "\"readings\":[\"1.10 kWh\",\"(230.00 ±0.12) V, k=2\",\"5.0 mA\"]," +
                                    "\"valid\":true}"));

        }

        #endregion

        #region The_document_converts_back_into_the_very_same_bytes()

        [Test]
        public void The_document_converts_back_into_the_very_same_bytes()
        {

            var cbor      = MeterReading();
            var expected  = Convert.ToHexString(cbor.ToByteArray());

            // ...through the Newtonsoft tree...
            Assert.That(Convert.ToHexString(CBORJSON.ToCBOR(CBORJSON.ToJSON(cbor)).ToByteArray()),
                        Is.EqualTo(expected),
                        "The Newtonsoft round trip changed the bytes!");

            // ...and through UTF-8 text.
            Assert.That(Convert.ToHexString(CBORJSON.ToCBOR(CBORJSON.ToJSONUTF8(cbor).AsSpan()).ToByteArray()),
                        Is.EqualTo(expected),
                        "The UTF-8 round trip changed the bytes!");

        }

        #endregion

        #region Every_example_of_the_specification_survives_the_JSON_document()

        [Test]
        public void Every_example_of_the_specification_survives_the_JSON_document()
        {

            // Section 5 of tag-44252.md - without the symbolic-unit row,
            // which converts back into the numeric identification the
            // canonical encoding asks for.
            var vectors = new [] {
                "D9ACDC820504",                              // 5 A
                "D9ACDC8218E605",                            // 230 V
                "D9ACDC83C4822018320422",                    // 5.0 mA
                "D9ACDC83C48221186E0203",                    // 1.10 kWh
                "D9ACDC84C482211901F40422C4822102",          // (5.00 ±0.02) mA
                "D9ACDC84050400C4822005",                    // (5 ±0.5) A
                "D9ACDC82C482211903D582820F01820821",        // 9.81 m·s^-2
                "D9ACDC84C482211959D80500A201C482210C0202",  // (230.00 ±0.12) V, k=2
                "D9ACDC83C48220182D82820501820982200228"     // 4.5 nV·Hz^-1/2
            };

            foreach (var hex in vectors)
            {

                var cbor  = CBORValue.Parse(Convert.FromHexString(hex));
                var json  = BothPathsAgree(cbor);

                Assert.That(Convert.ToHexString(CBORJSON.ToCBOR(json).ToByteArray()),
                            Is.EqualTo(hex),
                            $"{hex} came back as {json}");

            }

        }

        #endregion

        #region Numbers_keep_every_digit_they_were_written_with()

        [Test]
        public void Numbers_keep_every_digit_they_were_written_with()
        {

            var numbers = new CBORArray();

            numbers.Add(CBORValue.FromInt64      (42));
            numbers.Add(CBORValue.FromInt64      (-17));
            numbers.Add(CBORValue.FromDecimal    (1.10m));                                    // tag 4, scale 2
            numbers.Add(CBORValue.FromDecimal    (-273.15m));
            numbers.Add(CBORValue.FromUInt64     (18446744073709551615));                     // 2^64-1
            numbers.Add(CBORValue.FromBigInteger (BigInteger.Pow(10, 40)));                   // a bignum

            var cbor = numbers.ToValue();
            var json = BothPathsAgree(cbor);

            Assert.That(json,
                        Is.EqualTo("[42,-17,1.10,-273.15,18446744073709551615,10000000000000000000000000000000000000000]"));

            Assert.That(Convert.ToHexString(CBORJSON.ToCBOR(json).ToByteArray()),
                        Is.EqualTo(Convert.ToHexString(cbor.ToByteArray())),
                        "The numbers did not come back as they went in!");

            // A binary float becomes an ordinary JSON number and comes back
            // as an exact decimal - JSON has one number type, and the
            // metrological profile forbids binary floats anyway.
            Assert.That(BothPathsAgree(CBORValue.FromDouble(1.5)),   Is.EqualTo("1.5"));
            Assert.That(BothPathsAgree(CBORValue.FromDouble(1.0)),   Is.EqualTo("1.0"));

        }

        #endregion

        #region Byte_strings_are_rendered_the_way_the_options_ask_for()

        [Test]
        public void Byte_strings_are_rendered_the_way_the_options_ask_for()
        {

            var bytes = CBORValue.FromBytes([0x01, 0x02, 0xFB, 0xFF]);

            Assert.That(BothPathsAgree(bytes),
                        Is.EqualTo("\"AQL7_w\""));

            Assert.That(BothPathsAgree(bytes, new CBORJSONOptions { ByteStrings = CBORJSONByteStrings.Base64 }),
                        Is.EqualTo("\"AQL7/w==\""));

            Assert.That(BothPathsAgree(bytes, new CBORJSONOptions { ByteStrings = CBORJSONByteStrings.Hex }),
                        Is.EqualTo("\"0102fbff\""));

            // ...and back it is a string, because JSON has no binary type.
            Assert.That(CBORJSON.ToCBOR("\"0102fbff\"").Kind,
                        Is.EqualTo(CBORValueKind.TextString));

        }

        #endregion

        #region A_map_with_integer_keys_needs_a_word_from_the_caller()

        [Test]
        public void A_map_with_integer_keys_needs_a_word_from_the_caller()
        {

            // The shape of a COSE_Key: integer labels, byte string values.
            var coseKey = new CBORMap();
            coseKey.Add(CBORValue.FromInt64(1),  CBORValue.FromInt64(2));
            coseKey.Add(CBORValue.FromInt64(3),  CBORValue.FromInt64(-7));
            coseKey.Add(CBORValue.FromInt64(-1), CBORValue.FromInt64(1));

            var cbor = coseKey.ToValue();

            Assert.That(() => CBORJSON.ToJSON(cbor),
                        Throws.TypeOf<CBORException>());

            Assert.That(BothPathsAgree(cbor, new CBORJSONOptions { StringifyMapKeys = true }),
                        Is.EqualTo("{\"1\":2,\"3\":-7,\"-1\":1}"));

            // The way back gives text keys, not the integers they were:
            // this direction is documented as lossy.
            Assert.That(CBORJSON.ToCBOR("{\"1\":2}").AsMap()[0].Key.Kind,
                        Is.EqualTo(CBORValueKind.TextString));

        }

        #endregion

        #region What_this_profile_does_not_cover_is_refused()

        [Test]
        public void What_this_profile_does_not_cover_is_refused()
        {

            var unknownTag = CBORValue.Tagged(new CBORTag(4711), CBORValue.FromInt64(1));

            Assert.That(() => CBORJSON.ToJSON    (unknownTag),  Throws.TypeOf<CBORException>());
            Assert.That(() => CBORJSON.ToJSONUTF8(unknownTag),  Throws.TypeOf<CBORException>());

            Assert.That(BothPathsAgree(unknownTag, CBORJSONOptions.Lenient),
                        Is.EqualTo("\"4711(1)\""));

            // ...as are the things JSON has no room for at all.
            Assert.That(() => CBORJSON.ToJSON(CBORValue.Undefined),          Throws.TypeOf<CBORException>());
            Assert.That(() => CBORJSON.ToJSON(CBORValue.FromDouble(Double.NaN)),       Throws.TypeOf<CBORException>());
            Assert.That(() => CBORJSON.ToJSON(CBORValue.FromDouble(Double.PositiveInfinity)), Throws.TypeOf<CBORException>());

            // A broken metrological value is an error and never a placeholder.
            Assert.That(() => CBORJSON.ToJSON(CBORValue.Parse(Convert.FromHexString("D9ACDC820563"))),
                        Throws.Exception);

        }

        #endregion

        #region A_detector_decides_which_strings_are_measurements()

        [Test]
        public void A_detector_decides_which_strings_are_measurements()
        {

            // "1 h" is a perfectly good measurement and a perfectly good
            // piece of prose, so the caller gets to say.
            var json = "{\"duration\":\"1 h\",\"comment\":\"1 h\"}";

            var everything = CBORJSON.ToCBOR(json);

            Assert.That(everything["duration"].HasTag(CBORTag.MetrologicalValue),  Is.True);
            Assert.That(everything["comment"]. HasTag(CBORTag.MetrologicalValue),  Is.True);

            var onlyDuration = CBORJSON.ToCBOR(
                                   json,
                                   new CBORJSONOptions {
                                       DetectMetrologicalValues = (path, text) => path == "/duration"
                                   }
                               );

            Assert.That(onlyDuration["duration"].HasTag(CBORTag.MetrologicalValue),  Is.True);
            Assert.That(onlyDuration["comment"]. Kind,                               Is.EqualTo(CBORValueKind.TextString));

            // The path is a JSON Pointer, so it reaches into arrays as well.
            var nested = CBORJSON.ToCBOR(
                             "{\"readings\":[\"1.10 kWh\",\"5.0 mA\"]}",
                             new CBORJSONOptions {
                                 DetectMetrologicalValues = (path, text) => path == "/readings/0"
                             }
                         );

            Assert.That(nested["readings"][0].HasTag(CBORTag.MetrologicalValue),  Is.True);
            Assert.That(nested["readings"][1].Kind,                               Is.EqualTo(CBORValueKind.TextString));

        }

        #endregion

        #region The_object_form_converts_in_both_directions()

        [Test]
        public void The_object_form_converts_in_both_directions()
        {

            var options  = new CBORJSONOptions { Metrology = CBORJSONMetrology.Object };
            var cbor     = new MetrologicalValue(1.10m, UnitOfMeasure.WattHour, SIPrefix.Kilo).ToCBOR();

            Assert.That(BothPathsAgree(cbor, options),
                        Is.EqualTo("{\"value\":1.10,\"unit\":\"Wh\",\"prefix\":\"k\"}"));

            Assert.That(Convert.ToHexString(CBORJSON.ToCBOR(CBORJSON.ToJSON(cbor, options), options).ToByteArray()),
                        Is.EqualTo(Convert.ToHexString(cbor.ToByteArray())));

            Assert.That(Convert.ToHexString(CBORJSON.ToCBOR(CBORJSON.ToJSONUTF8(cbor, options).AsSpan(), options).ToByteArray()),
                        Is.EqualTo(Convert.ToHexString(cbor.ToByteArray())));

            // Switched off, tag 44252 is just another unknown tag.
            Assert.That(() => CBORJSON.ToJSON(cbor, new CBORJSONOptions { Metrology = CBORJSONMetrology.None }),
                        Throws.TypeOf<CBORException>());

        }

        #endregion

        #region Points_in_time_and_UUIDs_become_the_text_they_are()

        [Test]
        public void Points_in_time_and_UUIDs_become_the_text_they_are()
        {

            Assert.That(BothPathsAgree(CBORValue.Tagged(CBORTag.DateTimeString, CBORValue.FromText("2026-08-18T10:15:00.000Z"))),
                        Is.EqualTo("\"2026-08-18T10:15:00.000Z\""));

            Assert.That(BothPathsAgree(CBORValue.Tagged(CBORTag.EpochDateTime, CBORValue.FromInt64(1_787_048_100))),
                        Is.EqualTo("\"2026-08-18T10:15:00.000Z\""));

            var uuid = Guid.Parse("f81d4fae-7dec-11d0-a765-00a0c91e6bf6");

            Assert.That(BothPathsAgree(CBORValue.Tagged(CBORTag.UUID, CBORValue.FromBytes(uuid.ToByteArray(true)))),
                        Is.EqualTo("\"f81d4fae-7dec-11d0-a765-00a0c91e6bf6\""));

            // Self-described CBOR is a wrapper and nothing else.
            Assert.That(BothPathsAgree(CBORValue.Tagged(CBORTag.SelfDescribedCBOR, CBORValue.FromInt64(42))),
                        Is.EqualTo("42"));

            // None of them converts back into its tag: a JSON string that
            // looks like a timestamp is still just a string, and reading a
            // tag into it would be guessing.
            Assert.That(CBORJSON.ToCBOR("\"2026-08-18T10:15:00.000Z\"").Kind,
                        Is.EqualTo(CBORValueKind.TextString));

        }

        #endregion

        #region A_document_nested_too_deeply_is_refused()

        [Test]
        public void A_document_nested_too_deeply_is_refused()
        {

            var cbor = CBORValue.FromInt64(1);

            for (var i = 0; i < 40; i++)
                cbor = CBORValue.FromArray(cbor);

            var options = new CBORJSONOptions { MaxDepth = 8 };

            Assert.That(() => CBORJSON.ToJSON    (cbor, options),  Throws.TypeOf<CBORException>());
            Assert.That(() => CBORJSON.ToJSONUTF8(cbor, options),  Throws.TypeOf<CBORException>());

            var json = CBORJSON.ToJSONText(cbor);

            Assert.That(() => CBORJSON.ToCBOR(json,                              options),  Throws.Exception);
            Assert.That(() => CBORJSON.ToCBOR(CBORJSON.ToJSON(cbor),             options),  Throws.TypeOf<CBORException>());

        }

        #endregion

        #region An_invalid_document_is_reported_and_not_thrown_at_the_caller()

        [Test]
        public void An_invalid_document_is_reported_and_not_thrown_at_the_caller()
        {

            Assert.That(CBORJSON.TryToJSON(Convert.FromHexString("D82A"), out var json, out var errorResponse),  Is.False);
            Assert.That(json,                                                                                    Is.Null);
            Assert.That(errorResponse,                                                                           Is.Not.Null);

            Assert.That(CBORJSON.TryToCBOR("{\"a\":"u8, out _, out errorResponse),  Is.False);
            Assert.That(errorResponse,                                              Is.Not.Null);

            Assert.That(CBORJSON.TryToCBOR("{\"a\":1}"u8, out var cbor, out errorResponse),  Is.True);
            Assert.That(errorResponse,                                                       Is.Null);
            Assert.That(cbor["a"].AsInt64(),                                                 Is.EqualTo(1));

            // Trailing data is not silently ignored - the JSON reader says
            // so in its own words, and a JSON error stays a JSON error.
            Assert.That(CBORJSON.TryToCBOR("{} {}"u8, out _, out errorResponse),  Is.False);
            Assert.That(errorResponse,                                            Is.Not.Null);

        }

        #endregion

        #region The_conversion_is_culture_invariant()

        [Test]
        public void The_conversion_is_culture_invariant()
        {

            // The CI runs on Windows AND within a debian:13 container.
            var previousCulture = CultureInfo.CurrentCulture;

            try
            {

                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                Assert.That(BothPathsAgree(MeterReading()),
                            Does.Contain("\"1.10 kWh\""));

                Assert.That(BothPathsAgree(CBORValue.FromDecimal(-273.15m)),
                            Is.EqualTo("-273.15"));

            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }

        }

        #endregion

    }

}
