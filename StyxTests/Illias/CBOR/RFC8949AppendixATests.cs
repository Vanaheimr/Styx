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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The complete example table of RFC 8949, Appendix A.
    /// Every vector is decoded and verified via its diagnostic notation;
    /// all vectors that are already in the preferred (shortest, definite)
    /// encoding are also re-encoded byte-exact.
    /// </summary>
    [TestFixture]
    public class RFC8949AppendixATests
    {

        #region Data

        /// <summary>
        /// (Hex, expected diagnostic notation, roundtrips byte-exact under preferred encoding)
        /// </summary>
        private static readonly (String Hex, String Diagnostic, Boolean Roundtrips)[] appendixAVectors = [

            ("00",                                                            "0",                          true),
            ("01",                                                            "1",                          true),
            ("0A",                                                            "10",                         true),
            ("17",                                                            "23",                         true),
            ("1818",                                                          "24",                         true),
            ("1819",                                                          "25",                         true),
            ("1864",                                                          "100",                        true),
            ("1903E8",                                                        "1000",                       true),
            ("1A000F4240",                                                    "1000000",                    true),
            ("1B000000E8D4A51000",                                            "1000000000000",              true),
            ("1BFFFFFFFFFFFFFFFF",                                            "18446744073709551615",       true),
            ("C249010000000000000000",                                        "18446744073709551616",       true),
            ("3BFFFFFFFFFFFFFFFF",                                            "-18446744073709551616",      true),
            ("C349010000000000000000",                                        "-18446744073709551617",      true),
            ("20",                                                            "-1",                         true),
            ("29",                                                            "-10",                        true),
            ("3863",                                                          "-100",                       true),
            ("3903E7",                                                        "-1000",                      true),

            ("F90000",                                                        "0.0",                        true),
            ("F98000",                                                        "-0.0",                       true),
            ("F93C00",                                                        "1.0",                        true),
            ("FB3FF199999999999A",                                            "1.1",                        true),
            ("F93E00",                                                        "1.5",                        true),
            ("F97BFF",                                                        "65504.0",                    true),
            ("FA47C35000",                                                    "100000.0",                   true),
            ("FA7F7FFFFF",                                                    "3.4028234663852886e+38",     true),
            ("FB7E37E43C8800759C",                                            "1.0e+300",                   true),
            ("F90001",                                                        "5.960464477539063e-8",       true),
            ("F90400",                                                        "6.103515625e-5",             true),
            ("F9C400",                                                        "-4.0",                       true),
            ("FBC010666666666666",                                            "-4.1",                       true),
            ("F97C00",                                                        "Infinity",                   true),
            ("F97E00",                                                        "NaN",                        true),
            ("F9FC00",                                                        "-Infinity",                  true),
            ("FA7F800000",                                                    "Infinity",                   false),
            ("FA7FC00000",                                                    "NaN",                        false),
            ("FAFF800000",                                                    "-Infinity",                  false),
            ("FB7FF0000000000000",                                            "Infinity",                   false),
            ("FB7FF8000000000000",                                            "NaN",                        false),
            ("FBFFF0000000000000",                                            "-Infinity",                  false),

            ("F4",                                                            "false",                      true),
            ("F5",                                                            "true",                       true),
            ("F6",                                                            "null",                       true),
            ("F7",                                                            "undefined",                  true),
            ("F0",                                                            "simple(16)",                 true),
            ("F8FF",                                                          "simple(255)",                true),

            ("C074323031332D30332D32315432303A30343A30305A",                  "0(\"2013-03-21T20:04:00Z\")",  true),
            ("C11A514B67B0",                                                  "1(1363896240)",              true),
            ("C1FB41D452D9EC200000",                                          "1(1363896240.5)",            true),
            ("D74401020304",                                                  "23(h'01020304')",            true),
            ("D818456449455446",                                              "24(h'6449455446')",          true),
            ("D82076687474703A2F2F7777772E6578616D706C652E636F6D",            "32(\"http://www.example.com\")",  true),

            ("40",                                                            "h''",                        true),
            ("4401020304",                                                    "h'01020304'",                true),
            ("60",                                                            "\"\"",                       true),
            ("6161",                                                          "\"a\"",                      true),
            ("6449455446",                                                    "\"IETF\"",                   true),
            ("62225C",                                                        "\"\\\"\\\\\"",               true),
            ("62C3BC",                                                        "\"ü\"",                 true),
            ("63E6B0B4",                                                      "\"水\"",                 true),
            ("64F0908591",                                                    "\"𐅑\"",           true),

            ("80",                                                            "[]",                         true),
            ("83010203",                                                      "[1, 2, 3]",                  true),
            ("8301820203820405",                                              "[1, [2, 3], [4, 5]]",        true),
            ("98190102030405060708090A0B0C0D0E0F101112131415161718181819",    "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25]",  true),
            ("A0",                                                            "{}",                         true),
            ("A201020304",                                                    "{1: 2, 3: 4}",               true),
            ("A26161016162820203",                                            "{\"a\": 1, \"b\": [2, 3]}",  true),
            ("826161A161626163",                                              "[\"a\", {\"b\": \"c\"}]",    true),
            ("A56161614161626142616361436164614461656145",                    "{\"a\": \"A\", \"b\": \"B\", \"c\": \"C\", \"d\": \"D\", \"e\": \"E\"}",  true),

            ("5F42010243030405FF",                                            "h'0102030405'",              false),
            ("7F657374726561646D696E67FF",                                    "\"streaming\"",              false),
            ("9FFF",                                                          "[]",                         false),
            ("9F018202039F0405FFFF",                                          "[1, [2, 3], [4, 5]]",        false),
            ("9F01820203820405FF",                                            "[1, [2, 3], [4, 5]]",        false),
            ("83018202039F0405FF",                                            "[1, [2, 3], [4, 5]]",        false),
            ("83019F0203FF820405",                                            "[1, [2, 3], [4, 5]]",        false),
            ("9F0102030405060708090A0B0C0D0E0F101112131415161718181819FF",    "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25]",  false),
            ("BF61610161629F0203FFFF",                                        "{\"a\": 1, \"b\": [2, 3]}",  false),
            ("826161BF61626163FF",                                            "[\"a\", {\"b\": \"c\"}]",    false),
            ("BF6346756EF563416D7421FF",                                      "{\"Fun\": true, \"Amt\": -2}",  false)

        ];

        #endregion


        #region All_AppendixA_vectors_decode_to_the_expected_values()

        [Test]
        public void All_AppendixA_vectors_decode_to_the_expected_values()
        {

            foreach (var vector in appendixAVectors)
            {

                var cbor = CBORValue.Parse(Convert.FromHexString(vector.Hex));

                Assert.That(cbor.ToDiagnosticString(),
                            Is.EqualTo(vector.Diagnostic),
                            $"Vector '{vector.Hex}'");

            }

        }

        #endregion

        #region All_AppendixA_vectors_with_preferred_encodings_encode_back_byte_exact()

        [Test]
        public void All_AppendixA_vectors_with_preferred_encodings_encode_back_byte_exact()
        {

            foreach (var vector in appendixAVectors)
            {

                if (!vector.Roundtrips)
                    continue;

                var cbor = CBORValue.Parse(Convert.FromHexString(vector.Hex));

                Assert.That(Convert.ToHexString(cbor.ToByteArray()),
                            Is.EqualTo(vector.Hex),
                            $"Vector '{vector.Hex}' ({vector.Diagnostic})");

            }

        }

        #endregion

        #region AppendixA_halfprecision_specials_decode_to_the_exact_values()

        [Test]
        public void AppendixA_halfprecision_specials_decode_to_the_exact_values()
        {

            Assert.That(CBORValue.Parse(Convert.FromHexString("F90000")).AsDouble(),  Is.EqualTo(0.0));
            Assert.That(CBORValue.Parse(Convert.FromHexString("F98000")).AsDouble(),  Is.EqualTo(-0.0));
            Assert.That(CBORValue.Parse(Convert.FromHexString("F97BFF")).AsDouble(),  Is.EqualTo(65504.0));
            Assert.That(CBORValue.Parse(Convert.FromHexString("F90001")).AsDouble(),  Is.EqualTo(5.960464477539063e-8));
            Assert.That(CBORValue.Parse(Convert.FromHexString("F90400")).AsDouble(),  Is.EqualTo(0.00006103515625));
            Assert.That(CBORValue.Parse(Convert.FromHexString("F97C00")).AsDouble(),  Is.EqualTo(Double.PositiveInfinity));
            Assert.That(CBORValue.Parse(Convert.FromHexString("F9FC00")).AsDouble(),  Is.EqualTo(Double.NegativeInfinity));
            Assert.That(CBORValue.Parse(Convert.FromHexString("F97E00")).AsDouble(),  Is.NaN);

            // The sign of a negative zero survives...
            Assert.That(Double.IsNegative(CBORValue.Parse(Convert.FromHexString("F98000")).AsDouble()),  Is.True);

        }

        #endregion

        #region AppendixA_bignum_vectors_roundtrip_through_BigInteger()

        [Test]
        public void AppendixA_bignum_vectors_roundtrip_through_BigInteger()
        {

            // Note: The content of a negative bignum (tag 3) is |value| - 1!
            Assert.That(CBORValue.Parse(Convert.FromHexString("C249010000000000000000")).AsBigInteger(),
                        Is.EqualTo(BigInteger.Parse("18446744073709551616")));

            Assert.That(CBORValue.Parse(Convert.FromHexString("C349010000000000000000")).AsBigInteger(),
                        Is.EqualTo(BigInteger.Parse("-18446744073709551617")));

            Assert.That(Convert.ToHexString(CBORValue.FromBigInteger(BigInteger.Parse( "18446744073709551616")).ToByteArray()),
                        Is.EqualTo("C249010000000000000000"));

            Assert.That(Convert.ToHexString(CBORValue.FromBigInteger(BigInteger.Parse("-18446744073709551617")).ToByteArray()),
                        Is.EqualTo("C349010000000000000000"));

        }

        #endregion

        #region AppendixA_indefinite_length_vectors_normalize_to_their_definite_counterparts()

        [Test]
        public void AppendixA_indefinite_length_vectors_normalize_to_their_definite_counterparts()
        {

            Assert.That(CBORValue.Parse(Convert.FromHexString("9F018202039F0405FFFF")),
                        Is.EqualTo(CBORValue.Parse(Convert.FromHexString("8301820203820405"))));

            Assert.That(CBORValue.Parse(Convert.FromHexString("BF61610161629F0203FFFF")),
                        Is.EqualTo(CBORValue.Parse(Convert.FromHexString("A26161016162820203"))));

            Assert.That(CBORValue.Parse(Convert.FromHexString("5F42010243030405FF")).AsBytes(),
                        Is.EqualTo(new Byte[] { 1, 2, 3, 4, 5 }));

            Assert.That(Convert.ToHexString(CBORValue.Parse(Convert.FromHexString("9F018202039F0405FFFF")).ToByteArray()),
                        Is.EqualTo("8301820203820405"));

        }

        #endregion

    }

}
