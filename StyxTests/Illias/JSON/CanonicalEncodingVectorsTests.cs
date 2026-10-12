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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The test vectors of the canonical JSON and the deterministic CBOR of JSON documents,
    /// as given in Styx/Illias/JSON/README.md: what is written there is what these tests check.
    /// </summary>
    [TestFixture]
    public class CanonicalEncodingVectorsTests
    {

        #region (private static) Parse(Text)

        /// <summary>
        /// Read JSON text as hashing applications do: numbers with a fraction or an exponent as decimals, dates as text.
        /// </summary>
        private static JToken Parse(String Text)

            => JToken.ReadFrom(new JsonTextReader(new StringReader(Text)) {
                                   FloatParseHandling  = FloatParseHandling.Decimal,
                                   DateParseHandling   = DateParseHandling.None
                               });

        #endregion

        #region (private static) Esc(Hex)

        /// <summary>
        /// The JSON escape of the given UTF-16 code unit, e.g. Esc("001F").
        /// </summary>
        private static String Esc(String Hex)

            => "\\" + "u" + Hex;

        #endregion

        #region (private static) Char(CodePoint)

        private static String Char(Int32 CodePoint)

            => System.Char.ConvertFromUtf32(CodePoint);

        #endregion

        #region (private static) CBORHex(JSONText)

        private static String CBORHex(String JSONText)

            => Convert.ToHexStringLower(CBORJSON.ToCBOR(Parse(JSONText)).ToByteArray(CBORWriterOptions.Canonical));

        #endregion


        #region Keys_are_sorted_by_UTF16_code_units()

        [Test]
        public void Keys_are_sorted_by_UTF16_code_units()
        {

            // U+1F600 is the surrogate pair D83D DE00 and therefore sorts before U+FF61.
            var json = new JObject(
                           new JProperty("b",               1),
                           new JProperty("a",               2),
                           new JProperty("B",               3),
                           new JProperty(Char(0xE9),        4),
                           new JProperty("aa",              5),
                           new JProperty(Char(0x1F600),     6),
                           new JProperty(Char(0xFF61),      7)
                       );

            Assert.That(CanonicalJSON.Serialize(json),
                        Is.EqualTo("{\"B\":3,\"a\":2,\"aa\":5,\"b\":1,\"" + Char(0xE9) + "\":4,\"" + Esc("D83D") + Esc("DE00") + "\":6,\"" + Char(0xFF61) + "\":7}"));

        }

        #endregion

        #region Strings_escape_as_specified()

        [Test]
        public void Strings_escape_as_specified()
        {

            var escaped = new (Int32 CodePoint, String Expected)[] {
                              (0x22,     "\\\""),
                              (0x5C,     "\\\\"),
                              (0x08,     "\\b"),
                              (0x0C,     "\\f"),
                              (0x0A,     "\\n"),
                              (0x0D,     "\\r"),
                              (0x09,     "\\t"),
                              (0x01,     Esc("0001")),
                              (0x1F,     Esc("001F")),
                              (0x7F,     Esc("007F")),
                              (0x85,     Esc("0085")),
                              (0xA0,     Esc("00A0")),
                              (0x378,    Esc("0378")),
                              (0x2028,   Esc("2028")),
                              (0x2029,   Esc("2029")),
                              (0x1680,   Esc("1680")),
                              (0x2000,   Esc("2000")),
                              (0x200A,   Esc("200A")),
                              (0x202F,   Esc("202F")),
                              (0x205F,   Esc("205F")),
                              (0x3000,   Esc("3000")),
                              (0xE000,   Esc("E000")),
                              (0xFDD0,   Esc("FDD0")),
                              (0xFEFF,   Esc("FEFF")),
                              (0xFFFE,   Esc("FFFE")),
                              (0x10000,  Esc("D800") + Esc("DC00")),
                              (0x1F600,  Esc("D83D") + Esc("DE00")),
                              (0x10FFFF, Esc("DBFF") + Esc("DFFF"))
                          };

            var verbatim = new[] { 0x20, 0x27, 0x2B, 0x2F, 0x3C, 0x3E, 0x26, 0x60, 0x7E, 0xAD, 0xE9, 0x300, 0x600, 0x200B, 0x200D, 0x2066, 0x20AC, 0xFFF9, 0xFF61, 0xFFFD };

            using (Assert.EnterMultipleScope())
            {

                foreach (var (codePoint, expected) in escaped)
                    Assert.That(CanonicalJSON.Serialize(new JValue(Char(codePoint))),
                                Is.EqualTo("\"" + expected + "\""),
                                $"U+{codePoint:X4}");

                foreach (var codePoint in verbatim)
                    Assert.That(CanonicalJSON.Serialize(new JValue(Char(codePoint))),
                                Is.EqualTo("\"" + Char(codePoint) + "\""),
                                $"U+{codePoint:X4}");

            }

        }

        #endregion

        #region Decimals_keep_their_scale()

        [Test]
        public void Decimals_keep_their_scale()
        {

            using (Assert.EnterMultipleScope())
            {
                Assert.That(CanonicalJSON.Serialize(Parse("[ 1.0, 1, 1.50, 0.000, -0.0, 100, 1e-7, 1.5e2, 1E-2 ]")),
                            Is.EqualTo("[1.0,1,1.50,0.000,0.0,100,0.0000001,150,0.01]"));

                Assert.That(CanonicalJSON.Serialize(Parse("[ 18446744073709551615, -9223372036854775809, true, false, null, \"2026-10-12T12:00:00Z\" ]")),
                            Is.EqualTo("[18446744073709551615,-9223372036854775809,true,false,null,\"2026-10-12T12:00:00Z\"]"));
            }

        }

        #endregion

        #region A_document_is_written_without_whitespace()

        [Test]
        public void A_document_is_written_without_whitespace()

            => Assert.That(CanonicalJSON.Serialize(Parse("{ \"b\" : 1,\n  \"a\" : [ 1.0, 1, 1.50, 100, -2, 0.10 ],\n  \"aa\" : \"x\", \"c\" : null, \"d\" : true }")),
                           Is.EqualTo("{\"a\":[1.0,1,1.50,100,-2,0.10],\"aa\":\"x\",\"b\":1,\"c\":null,\"d\":true}"));

        #endregion


        #region CBOR_of_a_document()

        [Test]
        public void CBOR_of_a_document()
        {

            using (Assert.EnterMultipleScope())
            {

                // Map keys in the bytewise order of their encodings: "a", "b", "c", "d", then "aa".
                Assert.That(CBORHex("{\"b\":1,\"a\":[1.0,1,1.50,100,-2,0.10],\"aa\":\"x\",\"c\":null,\"d\":true}"),
                            Is.EqualTo("a5" +
                                       "6161" + "86" + "c482200a" + "01" + "c482211896" + "1864" + "21" + "c482210a" +
                                       "6162" + "01" +
                                       "6163" + "f6" +
                                       "6164" + "f5" +
                                       "626161" + "6178"));

                Assert.That(CBORHex("{\"aa\":1,\"b\":2,\"a\":3,\"" + Char(0xE9) + "\":4}"),
                            Is.EqualTo("a4" + "6161" + "03" + "6162" + "02" + "626161" + "01" + "62c3a9" + "04"));

                Assert.That(CBORHex("{\"o\":{},\"a\":[],\"s\":\"\"}"),
                            Is.EqualTo("a3" + "6161" + "80" + "616f" + "a0" + "6173" + "60"));

            }

        }

        #endregion

        #region CBOR_of_numbers()

        [Test]
        public void CBOR_of_numbers()
        {

            using (Assert.EnterMultipleScope())
            {

                Assert.That(CBORHex("[22000.0, -1.5, 0.000, 79228162514264337593543950335]"),
                            Is.EqualTo("84" +
                                       "c482201a00035b60" +
                                       "c482202e" +
                                       "c4822200" +
                                       "c24cffffffffffffffffffffffff"));

                Assert.That(CBORHex("[18446744073709551615, -9223372036854775809]"),
                            Is.EqualTo("82" + "1bffffffffffffffff" + "3b8000000000000000"));

                // Decimals without decimal places are the integers they equal, as their canonical JSON says,
                // whether they were read from text or held as decimals.
                var decimals = new JArray(new JValue(22000m), new JValue(-0m), new JValue(79228162514264337593543950335m), Parse("1.5e2"));

                Assert.That(CanonicalJSON.Serialize(decimals),
                            Is.EqualTo("[22000,0,79228162514264337593543950335,150]"));

                Assert.That(Convert.ToHexStringLower(CBORJSON.ToCBOR(decimals).ToByteArray(CBORWriterOptions.Canonical)),
                            Is.EqualTo("84" + "1955f0" + "00" + "c24cffffffffffffffffffffffff" + "1896"));

                Assert.That(Convert.ToHexStringLower(CBORJSON.ToCBOR(System.Text.Encoding.UTF8.GetBytes(CanonicalJSON.Serialize(decimals))).ToByteArray(CBORWriterOptions.Canonical)),
                            Is.EqualTo("84" + "1955f0" + "00" + "c24cffffffffffffffffffffffff" + "1896"));

            }

        }

        #endregion

        #region CBOR_keeps_strings_as_text()

        [Test]
        public void CBOR_keeps_strings_as_text()

            // Neither readings nor points in time are guessed from strings.
            => Assert.That(CBORHex("[\"1.10 kWh\", \"2026-10-12T12:00:00Z\"]"),
                           Is.EqualTo("82" + "68312e3130206b5768" + "74323032362d31302d31325431323a30303a30305a"));

        #endregion

    }

}
