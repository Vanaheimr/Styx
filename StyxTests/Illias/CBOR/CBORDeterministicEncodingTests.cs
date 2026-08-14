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
    /// Tests for the Core Deterministic Encoding Requirements
    /// of RFC 8949, Section 4.2.1.
    /// </summary>
    [TestFixture]
    public class CBORDeterministicEncodingTests
    {

        #region Map_keys_are_sorted_bytewise_not_length_first()

        [Test]
        public void Map_keys_are_sorted_bytewise_not_length_first()
        {

            // The encoded keys sort bytewise (RFC 8949), NOT length-first (RFC 7049):
            //   0A (10) < 20 (-1) < 41FF (h'ff') < 6161 ("a") < 626161 ("aa") < 811864 ([100])
            var map = new CBORMap {
                          { "aa",                          1 },
                          { CBORValue.FromArray(100),      2 },
                          { -1,                            3 },
                          { CBORValue.FromBytes([ 0xFF ]), 4 },
                          { "a",                           5 },
                          { 10,                            6 }
                      }.ToValue();

            Assert.That(Convert.ToHexString(map.ToByteArray(CBORWriterOptions.Canonical)),
                        Is.EqualTo("A60A06200341FF046161056261610181186402"));

        }

        #endregion

        #region Any_insertion_order_produces_identical_canonical_bytes()

        [Test]
        public void Any_insertion_order_produces_identical_canonical_bytes()
        {

            var map1 = new CBORMap {
                           { "b", 2 },
                           { "a", 1 },
                           { 10,  3 }
                       }.ToValue();

            var map2 = new CBORMap {
                           { 10,  3 },
                           { "a", 1 },
                           { "b", 2 }
                       }.ToValue();

            var canonical1 = map1.ToByteArray(CBORWriterOptions.Canonical);
            var canonical2 = map2.ToByteArray(CBORWriterOptions.Canonical);

            Assert.That(Convert.ToHexString(canonical1),  Is.EqualTo(Convert.ToHexString(canonical2)));
            Assert.That(Convert.ToHexString(canonical1),  Is.EqualTo("A30A03616101616202"));

            // ...and repeated encodings stay identical!
            Assert.That(map1.ToByteArray(CBORWriterOptions.Canonical),
                        Is.EqualTo(canonical1));

        }

        #endregion

        #region NaN_values_are_canonicalized_under_preferred_encoding()

        [Test]
        public void NaN_values_are_canonicalized_under_preferred_encoding()
        {

            // A NaN with a payload...
            var nanWithPayload = CBORValue.FromHalf(BitConverter.UInt16BitsToHalf(0x7E01));

            Assert.That(Convert.ToHexString(nanWithPayload.ToByteArray()),
                        Is.EqualTo("F97E00"));

            // ...which survives only when preferred float encoding is disabled!
            Assert.That(Convert.ToHexString(nanWithPayload.ToByteArray(new CBORWriterOptions {
                                                                           PreferredFloatEncoding = false
                                                                       })),
                        Is.EqualTo("F97E01"));

            Assert.That(Convert.ToHexString(CBORValue.FromDouble(Double.NaN).ToByteArray()),
                        Is.EqualTo("F97E00"));

        }

        #endregion

        #region Deterministic_parsing_rejects_non_deterministic_input()

        [Test]
        public void Deterministic_parsing_rejects_non_deterministic_input()
        {

            // A canonically encoded document parses fine...
            Assert.That(CBORValue.TryParse(Convert.FromHexString("A20A03616101"),
                                           out _,
                                           out var errorResponse,
                                           CBORReaderOptions.Canonical),
                        Is.True);

            Assert.That(errorResponse,  Is.Null);

            // ...but unsorted map keys are rejected!
            Assert.That(CBORValue.TryParse(Convert.FromHexString("A26161010A03"),
                                           out _,
                                           out var errorResponse2,
                                           CBORReaderOptions.Canonical),
                        Is.False);

            Assert.That(errorResponse2,  Does.Contain("order"));

            // A non-shortest integer head...
            Assert.That(CBORValue.TryParse(Convert.FromHexString("1900FF"),
                                           out _,
                                           out var errorResponse3,
                                           CBORReaderOptions.Canonical),
                        Is.False);

            Assert.That(errorResponse3,  Does.Contain("shortest"));

            // An indefinite-length array...
            Assert.That(CBORValue.TryParse(Convert.FromHexString("9F01FF"),
                                           out _,
                                           out var errorResponse4,
                                           CBORReaderOptions.Canonical),
                        Is.False);

            Assert.That(errorResponse4,  Does.Contain("ndefinite"));

        }

        #endregion

        #region Deterministic_roundtrips_stay_deterministic()

        [Test]
        public void Deterministic_roundtrips_stay_deterministic()
        {

            var document = new CBORMap {
                               { "zz",  new CBORArray { 1, 2, 3 } },
                               { "a",   CBORValue.FromDecimal(1.10m) },
                               { 100,   "hundred" },
                               { -1,    true }
                           }.ToValue();

            var canonicalBytes = document.ToByteArray(CBORWriterOptions.Canonical);

            // The canonical encoding parses under deterministic verification...
            var reparsed = CBORValue.Parse(canonicalBytes, CBORReaderOptions.Canonical);

            // ...and re-encodes to identical bytes!
            Assert.That(reparsed.ToByteArray(CBORWriterOptions.Canonical),
                        Is.EqualTo(canonicalBytes));

            // Note: The reparsed map is NOT representationally equal to the
            // original, as its entries are now in canonical order. The values
            // are still all there...
            Assert.That(reparsed["zz"].Count,           Is.EqualTo(3));
            Assert.That(reparsed["a"].AsDecimal(),      Is.EqualTo(1.10m));
            Assert.That(reparsed[100L].AsText(),        Is.EqualTo("hundred"));
            Assert.That(reparsed[-1L].AsBoolean(),      Is.True);

        }

        #endregion

    }

}
