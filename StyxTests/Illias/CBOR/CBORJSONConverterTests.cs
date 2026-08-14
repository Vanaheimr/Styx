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
    /// Regression tests for the CBOR to JSON converter, pinning the
    /// observable behavior of the original System.Formats.Cbor-based
    /// implementation: Byte strings become Base64 within CBOR2JSON,
    /// but lowercase hex within DecodeCborMap (WebAuthn COSE keys).
    /// </summary>
    [TestFixture]
    public class CBORJSONConverterTests
    {

        #region CBOR2JSON_converts_a_WebAuthn_style_attestation_map()

        [Test]
        public void CBOR2JSON_converts_a_WebAuthn_style_attestation_map()
        {

            var cborWriter = new CBORWriter();

            cborWriter.WriteStartMap(3);
            cborWriter.WriteTextString("fmt");
            cborWriter.WriteTextString("packed");
            cborWriter.WriteTextString("attStmt");
            cborWriter.WriteStartMap(2);
            cborWriter.WriteTextString("alg");
            cborWriter.WriteInt64(-7);
            cborWriter.WriteTextString("sig");
            cborWriter.WriteByteString([ 0x01, 0x02, 0x03 ]);
            cborWriter.WriteEndMap();
            cborWriter.WriteTextString("authData");
            cborWriter.WriteByteString([ 0xAA, 0xBB, 0xCC ]);
            cborWriter.WriteEndMap();

            var json = cborWriter.ToByteArray().CBOR2JSON();

            Assert.That(json,  Is.Not.Null);

            // Note: Byte strings surface as JValue(Byte[]) and serialize as Base64,
            // therefore the comparison uses the serialized text!
            Assert.That(json!.ToString(Formatting.None),
                        Is.EqualTo(JObject.Parse("""
                                       {
                                           "fmt":      "packed",
                                           "attStmt":  {
                                               "alg":  -7,
                                               "sig":  "AQID"
                                           },
                                           "authData": "qrvM"
                                       }
                                       """).ToString(Formatting.None)));

        }

        #endregion

        #region CBOR2JSON_converts_numbers_booleans_nulls_and_arrays()

        [Test]
        public void CBOR2JSON_converts_numbers_booleans_nulls_and_arrays()
        {

            var cborWriter = new CBORWriter();

            cborWriter.WriteStartMap(6);
            cborWriter.WriteTextString("unsigned");   cborWriter.WriteUInt64(5);
            cborWriter.WriteTextString("negative");   cborWriter.WriteInt64(-5);
            cborWriter.WriteTextString("single");     cborWriter.WriteSingle(100000.0f);
            cborWriter.WriteTextString("double");     cborWriter.WriteDouble(1.1);
            cborWriter.WriteTextString("boolean");    cborWriter.WriteBoolean(true);
            cborWriter.WriteTextString("array");
            cborWriter.WriteStartArray(3);
            cborWriter.WriteUInt64(1);
            cborWriter.WriteTextString("x");
            cborWriter.WriteNull();
            cborWriter.WriteEndArray();
            cborWriter.WriteEndMap();

            var json = cborWriter.ToByteArray().CBOR2JSON();

            Assert.That(json,  Is.Not.Null);

            Assert.That(json!.ToString(Formatting.None),
                        Is.EqualTo(JObject.Parse("""
                                       {
                                           "unsigned":  5,
                                           "negative":  -5,
                                           "single":    100000.0,
                                           "double":    1.1,
                                           "boolean":   true,
                                           "array":     [ 1, "x", null ]
                                       }
                                       """).ToString(Formatting.None)));

        }

        #endregion

        #region CBOR2JSON_supports_integer_map_keys_as_text()

        [Test]
        public void CBOR2JSON_supports_integer_map_keys_as_text()
        {

            // {1: "one", -2: "minus two"}
            var cborWriter = new CBORWriter();

            cborWriter.WriteStartMap(2);
            cborWriter.WriteUInt64(1);
            cborWriter.WriteTextString("one");
            cborWriter.WriteInt64(-2);
            cborWriter.WriteTextString("minus two");
            cborWriter.WriteEndMap();

            var json = cborWriter.ToByteArray().CBOR2JSON();

            Assert.That(json,  Is.Not.Null);

            Assert.That(json!.ToString(Formatting.None),
                        Is.EqualTo(JObject.Parse("""
                                       {
                                           "1":   "one",
                                           "-2":  "minus two"
                                       }
                                       """).ToString(Formatting.None)));

        }

        #endregion

        #region CBOR2JSON_returns_null_for_CBOR_null()

        [Test]
        public void CBOR2JSON_returns_null_for_CBOR_null()
        {

            Assert.That(Convert.FromHexString("F6").CBOR2JSON(),
                        Is.Null);

        }

        #endregion

        #region DecodeCborMap_renders_byte_strings_as_lowercase_hex_with_integer_keys()

        [Test]
        public void DecodeCborMap_renders_byte_strings_as_lowercase_hex_with_integer_keys()
        {

            // The shape of a WebAuthn EC2 COSE_Key...
            var xCoordinate  = new Byte[32];
            var yCoordinate  = new Byte[32];

            for (var i = 0; i < 32; i++)
            {
                xCoordinate[i] = (Byte)  i;
                yCoordinate[i] = (Byte) (i + 32);
            }

            var cborWriter = new CBORWriter();

            cborWriter.WriteStartMap(5);
            cborWriter.WriteUInt64(1);    cborWriter.WriteUInt64(2);
            cborWriter.WriteUInt64(3);    cborWriter.WriteInt64(-7);
            cborWriter.WriteInt64(-1);    cborWriter.WriteUInt64(1);
            cborWriter.WriteInt64(-2);    cborWriter.WriteByteString(xCoordinate);
            cborWriter.WriteInt64(-3);    cborWriter.WriteByteString(yCoordinate);
            cborWriter.WriteEndMap();

            var json = CborToJsonConverter.DecodeCborMap(cborWriter.ToByteArray());

            Assert.That(json["1"]?. Value<Int64>(),   Is.EqualTo(2));
            Assert.That(json["3"]?. Value<Int64>(),   Is.EqualTo(-7));
            Assert.That(json["-1"]?.Value<Int64>(),   Is.EqualTo(1));
            Assert.That(json["-2"]?.Value<String>(),  Is.EqualTo(Convert.ToHexStringLower(xCoordinate)));
            Assert.That(json["-3"]?.Value<String>(),  Is.EqualTo(Convert.ToHexStringLower(yCoordinate)));

        }

        #endregion

        #region Unsupported_CBOR_kinds_throw_NotSupportedExceptions()

        [Test]
        public void Unsupported_CBOR_kinds_throw_NotSupportedExceptions()
        {

            // A tagged value within CBOR2JSON...
            var cborWriter = new CBORWriter();

            cborWriter.WriteStartMap(1);
            cborWriter.WriteTextString("timestamp");
            cborWriter.WriteDateTime(new DateTimeOffset(2013, 3, 21, 20, 4, 0, TimeSpan.Zero));
            cborWriter.WriteEndMap();

            Assert.That(() => cborWriter.ToByteArray().CBOR2JSON(),
                        Throws.TypeOf<NotSupportedException>());

            // A boolean value within DecodeCborMap...
            Assert.That(() => CborToJsonConverter.DecodeCborMap(Convert.FromHexString("A101F5")),
                        Throws.TypeOf<NotSupportedException>());

        }

        #endregion

    }

}
