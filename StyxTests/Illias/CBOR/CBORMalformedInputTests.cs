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
    /// Tests rejecting malformed CBOR data (RFC 8949, Appendix F)
    /// and hardening against hostile inputs.
    /// </summary>
    [TestFixture]
    public class CBORMalformedInputTests
    {

        #region (private static) Validate(Hex)

        /// <summary>
        /// Verify the well-formedness of a single complete data item.
        /// </summary>
        private static void Validate(String Hex)
        {

            var reader = new CBORReader(Convert.FromHexString(Hex));

            reader.SkipValue();

            if (reader.BytesRemaining != 0)
                throw new CBORException($"{reader.BytesRemaining} trailing byte(s)!");

        }

        #endregion


        #region Truncated_inputs_throw_CBORExceptions_at_every_byte_position()

        [Test]
        public void Truncated_inputs_throw_CBORExceptions_at_every_byte_position()
        {

            var wellFormedVectors = new String[] {
                                        "1BFFFFFFFFFFFFFFFF",
                                        "62C3BC",
                                        "A26161016162820203",
                                        "9F018202039F0405FFFF",
                                        "C48221196AB3",
                                        "5F42010243030405FF",
                                        "FB3FF199999999999A"
                                    };

            foreach (var vector in wellFormedVectors)
            {

                Assert.That(() => Validate(vector),  Throws.Nothing,  $"Vector '{vector}' should be well-formed!");

                var bytes = Convert.FromHexString(vector);

                for (var length = 0; length < bytes.Length; length++)
                {

                    var truncated = bytes[..length];

                    Assert.That(() => {
                                    var reader = new CBORReader(truncated);
                                    reader.SkipValue();
                                },
                                Throws.TypeOf<CBORException>(),
                                $"Truncating '{vector}' to {length} byte(s) must be malformed!");

                }

            }

        }

        #endregion

        #region Break_stop_codes_outside_indefinite_containers_are_malformed()

        [Test]
        public void Break_stop_codes_outside_indefinite_containers_are_malformed()
        {

            // A lonely break...
            Assert.That(() => Validate("FF"),        Throws.TypeOf<CBORException>());

            // A break within a definite-length array...
            Assert.That(() => Validate("8201FF"),    Throws.TypeOf<CBORException>());

            // A break as a tagged value...
            Assert.That(() => Validate("9FC0FF"),    Throws.TypeOf<CBORException>());

        }

        #endregion

        #region Unclosed_indefinite_containers_are_malformed()

        [Test]
        public void Unclosed_indefinite_containers_are_malformed()
        {

            Assert.That(() => Validate("9F01"),      Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("BF6161"),    Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("5F4201"),    Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("7F"),        Throws.TypeOf<CBORException>());

        }

        #endregion

        #region Indefinite_maps_with_odd_item_counts_are_malformed()

        [Test]
        public void Indefinite_maps_with_odd_item_counts_are_malformed()
        {

            // {_ "a"} - a key without a value...
            Assert.That(() => Validate("BF6161FF"),          Throws.TypeOf<CBORException>());

            // {_ "a": 1, "b"}...
            Assert.That(() => Validate("BF616101 6162FF".Replace(" ", "")),
                        Throws.TypeOf<CBORException>());

            // ...while a complete pair is fine!
            Assert.That(() => Validate("BF616101FF"),        Throws.Nothing);

        }

        #endregion

        #region Indefinite_strings_only_allow_definite_chunks_of_the_same_type()

        [Test]
        public void Indefinite_strings_only_allow_definite_chunks_of_the_same_type()
        {

            // A text chunk within an indefinite byte string...
            Assert.That(() => Validate("5F6161FF"),      Throws.TypeOf<CBORException>());

            // A byte chunk within an indefinite text string...
            Assert.That(() => Validate("7F4161FF"),      Throws.TypeOf<CBORException>());

            // A nested indefinite chunk...
            Assert.That(() => Validate("5F5F4101FFFF"),  Throws.TypeOf<CBORException>());

            // A tag between chunks...
            Assert.That(() => Validate("7F6161C06162FF"),
                        Throws.TypeOf<CBORException>());

            // An integer within an indefinite text string...
            Assert.That(() => Validate("7F01FF"),        Throws.TypeOf<CBORException>());

        }

        #endregion

        #region A_multibyte_character_split_across_text_chunks_is_malformed()

        [Test]
        public void A_multibyte_character_split_across_text_chunks_is_malformed()
        {

            // "水" (E6 B0 B4) split into the chunks (E6 B0) and (B4):
            // Every chunk must be valid UTF-8 on its own (RFC 8949, Section 3.2.3)!
            Assert.That(() => new CBORReader(Convert.FromHexString("7F62E6B061B4FF")).ReadTextString(),
                        Throws.TypeOf<CBORException>());

            // ...but the same character within a single chunk is fine!
            Assert.That(new CBORReader(Convert.FromHexString("7F63E6B0B4FF")).ReadTextString(),
                        Is.EqualTo("水"));

        }

        #endregion

        #region Huge_claimed_lengths_fail_fast_without_allocating()

        [Test]
        public void Huge_claimed_lengths_fail_fast_without_allocating()
        {

            // A byte string claiming 2^64-1 bytes...
            Assert.That(() => new CBORReader(Convert.FromHexString("5BFFFFFFFFFFFFFFFF010203")).ReadByteString(),
                        Throws.TypeOf<CBORException>());

            // A text string claiming 4 GByte...
            Assert.That(() => new CBORReader(Convert.FromHexString("7A FFFFFFFF 616263".Replace(" ", ""))).ReadTextString(),
                        Throws.TypeOf<CBORException>());

            // An array claiming 2^64-1 data items...
            Assert.That(() => new CBORReader(Convert.FromHexString("9BFFFFFFFFFFFFFFFF01")).ReadStartArray(),
                        Throws.TypeOf<CBORException>());

            // A map claiming 2^32 key/value pairs...
            Assert.That(() => new CBORReader(Convert.FromHexString("BB000000010000000001")).ReadStartMap(),
                        Throws.TypeOf<CBORException>());

            // The same during skipping...
            Assert.That(() => Validate("5BFFFFFFFFFFFFFFFF010203"),
                        Throws.TypeOf<CBORException>());

            Assert.That(() => Validate("9BFFFFFFFFFFFFFFFF01"),
                        Throws.TypeOf<CBORException>());

        }

        #endregion

        #region Deeply_nested_documents_hit_MaxDepth()

        [Test]
        public void Deeply_nested_documents_hit_MaxDepth()
        {

            // 100k nested indefinite arrays...
            var arrayBomb = new Byte[100_000];
            Array.Fill(arrayBomb, (Byte) 0x9F);

            Assert.That(() => {
                            var reader = new CBORReader(arrayBomb);
                            reader.SkipValue();
                        },
                        Throws.TypeOf<CBORException>().With.Message.Contains("depth"));

            // 100k chained tags...
            var tagBomb = new Byte[100_001];
            Array.Fill(tagBomb, (Byte) 0xC0);
            tagBomb[^1] = 0x01;

            Assert.That(() => {
                            var reader = new CBORReader(tagBomb);
                            reader.SkipValue();
                        },
                        Throws.TypeOf<CBORException>().With.Message.Contains("depth"));

            // ...and via the container reading API!
            Assert.That(() => {

                            var reader = new CBORReader(arrayBomb);

                            while (true)
                                reader.ReadStartArray();

                        },
                        Throws.TypeOf<CBORException>().With.Message.Contains("depth"));

        }

        #endregion

        #region Invalid_UTF8_is_rejected_by_default_and_replaced_when_lenient()

        [Test]
        public void Invalid_UTF8_is_rejected_by_default_and_replaced_when_lenient()
        {

            // 0xC3 0x28 is not valid UTF-8...
            Assert.That(() => new CBORReader(Convert.FromHexString("62C328")).ReadTextString(),
                        Throws.TypeOf<CBORException>());

            var lenientOptions  = new CBORReaderOptions {
                                      UTF8Validation = CBORUTF8Validation.Lenient
                                  };

            Assert.That(new CBORReader(Convert.FromHexString("62C328"), lenientOptions).ReadTextString(),
                        Is.EqualTo("�("));

        }

        #endregion

        #region Reserved_initial_bytes_are_malformed()

        [Test]
        public void Reserved_initial_bytes_are_malformed()
        {

            // Additional information 28..30 is reserved for all major types...
            Assert.That(() => Validate("1C"),    Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("3D"),    Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("5E"),    Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("FC"),    Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("FD"),    Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("FE"),    Throws.TypeOf<CBORException>());

            // The two-byte encoding of simple values 0..31 is reserved...
            Assert.That(() => Validate("F800"),  Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("F818"),  Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("F81F"),  Throws.TypeOf<CBORException>());

            // ...but 32 is fine!
            Assert.That(() => Validate("F820"),  Throws.Nothing);

            // Integers and tags must not use an indefinite length...
            Assert.That(() => Validate("1F"),    Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("3F"),    Throws.TypeOf<CBORException>());
            Assert.That(() => Validate("DF00"),  Throws.TypeOf<CBORException>());

        }

        #endregion

        #region Definite_containers_enforce_their_declared_counts()

        [Test]
        public void Definite_containers_enforce_their_declared_counts()
        {

            // Reading more data items than declared...
            Assert.That(() => {
                            var reader = new CBORReader(Convert.FromHexString("810102"));
                            reader.ReadStartArray();
                            reader.ReadUInt64();
                            reader.ReadUInt64();
                        },
                        Throws.TypeOf<CBORException>());

            // Ending an array while data items remain...
            Assert.That(() => {
                            var reader = new CBORReader(Convert.FromHexString("820102"));
                            reader.ReadStartArray();
                            reader.ReadEndArray();
                        },
                        Throws.TypeOf<CBORException>());

            // Ending an array as a map...
            Assert.That(() => {
                            var reader = new CBORReader(Convert.FromHexString("80"));
                            reader.ReadStartArray();
                            reader.ReadEndMap();
                        },
                        Throws.TypeOf<CBORException>());

        }

        #endregion

    }

}
