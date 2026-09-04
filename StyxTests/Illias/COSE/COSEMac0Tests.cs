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

using System.Text;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.COSE
{

    /// <summary>
    /// COSE_Mac0 [RFC 9052, Section 6.2] with HMAC [RFC 9053, Section 3.1].
    ///
    /// The vectors here are split in two on purpose, because RFC 9052 leaves a
    /// gap: its only COSE_Mac0 example, Appendix C.6.1, uses AES-CBC-MAC and
    /// not HMAC, so no published message pins both halves at once.
    ///
    /// - The STRUCTURE is pinned against C.6.1 all the same. Its 37 bytes are
    ///   parsed, every field is checked, the message re-encodes to the
    ///   identical bytes, and the MAC_structure built from its inputs is
    ///   asserted. That covers everything except the primitive.
    /// - The PRIMITIVE is pinned against RFC 4231, the canonical HMAC-SHA-2
    ///   vectors, including the truncation HMAC 256/64 asks for.
    ///
    /// What neither can reach - that this implementation and the TypeScript
    /// one build the same COSE_Mac0 for the same inputs - is what the
    /// conformance suite is for. A MAC is deterministic, so those bytes are
    /// directly comparable, with none of the arrangements a signature needs.
    /// </summary>
    [TestFixture]
    public class COSEMac0Tests
    {

        #region Data

        private static readonly Byte[] payload = Encoding.UTF8.GetBytes("This is the content.");

        private static readonly Byte[] secret  = Convert.FromHexString("849B57219DAE48DE646D07DBB533566E976686457C1491BE3A76DCEA6C427188");

        /// <summary>
        /// RFC 9052 Appendix C.6.1, assembled from the diagnostic notation the
        /// RFC prints:
        ///
        ///     17([ h'a1010f', {}, 'This is the content.', h'726043745027214f' ])
        ///
        /// Its stated size, 37 bytes, is the check on the assembly.
        /// </summary>
        private static readonly Byte[] c61 = Convert.FromHexString(
                                                 "D18443A1010FA054546869732069732074686520" +
                                                 "636F6E74656E742E48726043745027214F"
                                             );

        private static COSEKey KeyFor(COSEAlgorithm? Algorithm = null)

            => COSEKey.FromSymmetricKey(secret,
                                        Convert.FromHexString("3131"),
                                        Algorithm ?? COSEAlgorithm.HMAC256_256);

        #endregion


        #region The one COSE_Mac0 example RFC 9052 prints

        [Test]
        public void The_published_example_is_thirty_seven_bytes()
        {
            Assert.That(c61.Length, Is.EqualTo(37));
        }

        [Test]
        public void The_published_example_parses_field_by_field()
        {

            Assert.That(COSEMac0.TryParse(c61, out var message, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {

                Assert.That(message!.IsTagged,                                  Is.True);
                Assert.That(Convert.ToHexString(message.ProtectedHeaderBytes),  Is.EqualTo("A1010F"));
                Assert.That(message.UnprotectedHeader.Parameters,               Is.Empty);
                Assert.That(message.Payload,                                    Is.EqualTo(payload));
                Assert.That(Convert.ToHexString(message.Tag),                   Is.EqualTo("726043745027214F"));

                // Algorithm 15 is AES-MAC 256/64, which this implementation
                // does not provide. Reading a message is not the place to
                // refuse it: the identifier is recorded as it travelled, and
                // only USING it fails.
                Assert.That(message.Algorithm?.Value,                           Is.EqualTo(15));
                Assert.That(message.Algorithm?.Family,                          Is.EqualTo(COSEAlgorithmFamily.None));

            });

        }

        [Test]
        public void The_published_example_re_encodes_to_the_identical_bytes()
        {

            Assert.That(COSEMac0.TryParse(c61, out var message, out var errorResponse), Is.True, errorResponse);
            Assert.That(message!.ToByteArray(), Is.EqualTo(c61));

        }

        [Test]
        public void The_MAC_structure_is_the_one_the_RFC_defines()
        {

            //  84                array(4)
            //  64 4D414330       "MAC0"
            //  43 A1010F         the protected bucket, verbatim
            //  40                no external data
            //  54 ...            the payload, all 20 bytes of it
            Assert.That(
                Convert.ToHexString(COSEMac0.ToBeMACed(Convert.FromHexString("A1010F"), payload)),
                Is.EqualTo("84" + "644D414330" + "43A1010F" + "40" + "54" + Convert.ToHexString(payload))
            );

        }

        [Test]
        public void The_AES_MAC_example_is_read_but_never_pretended_to_verify()
        {

            Assert.That(COSEMac0.TryParse(c61, out var message, out _), Is.True);

            Assert.That(message!.Verify(COSEKey.FromSymmetricKey(secret), out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("not a message authentication algorithm"));

        }

        #endregion

        #region The HMAC primitive against RFC 4231

        /// <summary>
        /// The published HMAC-SHA-2 vectors. Test Case 2 uses a FOUR-BYTE key,
        /// which is shorter than the hash output - RFC 9053 only SHOULD-nots
        /// that, and refusing it would make the vector unreproducible. Test
        /// Case 7 uses a key longer than the block size, which the primitive
        /// has to hash before using: the one case a naive implementation gets
        /// wrong.
        /// </summary>
        private static readonly (String Name, String Key, String Data, String SHA256, String SHA384, String SHA512)[] rfc4231 = [

            ("Test Case 1",
             new String('0', 0) + String.Concat(Enumerable.Repeat("0b", 20)),
             "4869205468657265",
             "b0344c61d8db38535ca8afceaf0bf12b881dc200c9833da726e9376c2e32cff7",
             "afd03944d84895626b0825f4ab46907f15f9dadbe4101ec682aa034c7cebc59c" +
             "faea9ea9076ede7f4af152e8b2fa9cb6",
             "87aa7cdea5ef619d4ff0b4241a1d6cb02379f4e2ce4ec2787ad0b30545e17cde" +
             "daa833b7d6b8a702038b274eaea3f4e4be9d914eeb61f1702e696c203a126854"),

            ("Test Case 2",
             "4a656665",
             "7768617420646f2079612077616e7420666f72206e6f7468696e673f",
             "5bdcc146bf60754e6a042426089575c75a003f089d2739839dec58b964ec3843",
             "af45d2e376484031617f78d2b58a6b1b9c7ef464f5a01b47e42ec3736322445e" +
             "8e2240ca5e69e2c78b3239ecfab21649",
             "164b7a7bfcf819e2e395fbe73b56e0a387bd64222e831fd610270cd7ea250554" +
             "9758bf75c05a994a6d034f65f8f0e6fdcaeab1a34d4a6b4b636e070a38bce737"),

            ("Test Case 3",
             String.Concat(Enumerable.Repeat("aa", 20)),
             String.Concat(Enumerable.Repeat("dd", 50)),
             "773ea91e36800e46854db8ebd09181a72959098b3ef8c122d9635514ced565fe",
             "88062608d3e6ad8a0aa2ace014c8a86f0aa635d947ac9febe83ef4e55966144b" +
             "2a5ab39dc13814b94e3ab6e101a34f27",
             "fa73b0089d56a284efb0f0756c890be9b1b5dbdd8ee81a3655f83e33b2279d39" +
             "bf3e848279a722c806b485a47e67c807b946a337bee8942674278859e13292fb"),

            ("Test Case 7",
             String.Concat(Enumerable.Repeat("aa", 131)),
             Convert.ToHexString(Encoding.ASCII.GetBytes(
                 "This is a test using a larger than block-size key and a larger " +
                 "than block-size data. The key needs to be hashed before being " +
                 "used by the HMAC algorithm.")),
             "9b09ffa71b942fcb27635fbcd5b0e944bfdc63644f0713938a7f51535c3a35e2",
             "6617178e941f020d351e2f254e8fd32c602420feb0b8fb9adccebb82461e99c5" +
             "a678cc31e799176d3860e6110c46523e",
             "e37b6a775dc87dbaa4dfa9f96e5e3ffddebd71f8867289865df5a32d20cdc944" +
             "b6022cac3c4982b10d5eeb55c3e4de15134676fb6de0446065c97440fa8c6a58")

        ];

        [Test]
        public void The_HMAC_primitive_reproduces_RFC_4231()
        {

            foreach (var vector in rfc4231)
            {

                var key   = Convert.FromHexString(vector.Key);
                var data  = Convert.FromHexString(vector.Data);

                Assert.Multiple(() => {

                    Assert.That(Convert.ToHexString(COSEAlgorithm.HMAC256_256.ComputeMAC(data, key)).ToLowerInvariant(),
                                Is.EqualTo(vector.SHA256), vector.Name);

                    Assert.That(Convert.ToHexString(COSEAlgorithm.HMAC384_384.ComputeMAC(data, key)).ToLowerInvariant(),
                                Is.EqualTo(vector.SHA384), vector.Name);

                    Assert.That(Convert.ToHexString(COSEAlgorithm.HMAC512_512.ComputeMAC(data, key)).ToLowerInvariant(),
                                Is.EqualTo(vector.SHA512), vector.Name);

                });

            }

        }

        [Test]
        public void The_truncation_keeps_the_leftmost_bits()
        {

            var vector = rfc4231[0];
            var tag    = COSEAlgorithm.HMAC256_64.ComputeMAC(Convert.FromHexString(vector.Data),
                                                             Convert.FromHexString(vector.Key));

            Assert.Multiple(() => {
                Assert.That(tag.Length,                                       Is.EqualTo(8));
                Assert.That(Convert.ToHexString(tag).ToLowerInvariant(),      Is.EqualTo(vector.SHA256[..16]));
            });

        }

        #endregion

        #region Authenticating and verifying

        [Test]
        public void A_message_round_trips_through_its_bytes()
        {

            var key      = KeyFor();
            var created  = COSEMac0.Create(payload, key);

            Assert.That(COSEMac0.TryParse(created.ToByteArray(), out var message, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {
                Assert.That(message!.IsTagged,                                Is.True);
                Assert.That(message.Algorithm?.Name,                          Is.EqualTo("HMAC 256/256"));
                Assert.That(Convert.ToHexString(message.KeyIdentifier!),      Is.EqualTo("3131"));
                Assert.That(message.Tag.Length,                               Is.EqualTo(32));
                Assert.That(message.Verify(key),                              Is.True);
                Assert.That(message.ToByteArray(),                            Is.EqualTo(created.ToByteArray()));
            });

        }

        [Test]
        public void Every_algorithm_produces_the_tag_width_it_names()
        {

            foreach (var (algorithm, width) in new (COSEAlgorithm, Int32)[] {
                         (COSEAlgorithm.HMAC256_64,   8),
                         (COSEAlgorithm.HMAC256_256, 32),
                         (COSEAlgorithm.HMAC384_384, 48),
                         (COSEAlgorithm.HMAC512_512, 64) })
            {

                var key      = KeyFor(algorithm);
                var message  = COSEMac0.Create(payload, key);

                Assert.Multiple(() => {
                    Assert.That(message.Tag.Length,  Is.EqualTo(width),  algorithm.Name);
                    Assert.That(message.Verify(key), Is.True,            algorithm.Name);
                });

            }

        }

        [Test]
        public void A_changed_payload_is_refused()
        {

            var key       = KeyFor();
            var created   = COSEMac0.Create(payload, key);

            var tampered  = new COSEMac0(created.ProtectedHeaderBytes,
                                         created.UnprotectedHeader,
                                         Encoding.UTF8.GetBytes("This is the contenu."),
                                         created.Tag,
                                         created.IsTagged);

            Assert.That(tampered.Verify(key, out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("not the right one"));

        }

        [Test]
        public void A_changed_tag_is_refused_in_any_single_bit()
        {

            var key      = KeyFor();
            var created  = COSEMac0.Create(payload, key);

            foreach (var position in new[] { 0, 15, 31 })
            {

                var tag = (Byte[]) created.Tag.Clone();
                tag[position] ^= 0x01;

                var tampered = new COSEMac0(created.ProtectedHeaderBytes,
                                            created.UnprotectedHeader,
                                            created.Payload,
                                            tag,
                                            created.IsTagged);

                Assert.That(tampered.Verify(key), Is.False, $"byte {position}");

            }

        }

        [Test]
        public void Another_key_is_refused()
        {

            var other = COSEKey.FromSymmetricKey(new Byte[32], null, COSEAlgorithm.HMAC256_256);

            Assert.That(COSEMac0.Create(payload, KeyFor()).Verify(other), Is.False);

        }

        [Test]
        public void External_data_is_authenticated_without_being_transported()
        {

            var key       = KeyFor();
            var aad       = Convert.FromHexString("11AA22BB33CC44DD55006699");
            var created   = COSEMac0.Create(payload, key, aad);

            Assert.Multiple(() => {
                Assert.That(Convert.ToHexString(created.ToByteArray()),  Does.Not.Contain("11AA22BB"));
                Assert.That(created.Verify(key, aad),                    Is.True);
                Assert.That(created.Verify(key),                         Is.False);
            });

        }

        [Test]
        public void A_detached_payload_carries_the_same_tag()
        {

            var key       = KeyFor();
            var attached  = COSEMac0.Create(payload, key);
            var detached  = COSEMac0.Create(payload, key, null, true);

            Assert.Multiple(() => {
                Assert.That(detached.Tag,                                  Is.EqualTo(attached.Tag));
                Assert.That(detached.Payload,                              Is.Null);
                Assert.That(detached.Verify(key, null, payload),           Is.True);
                Assert.That(detached.Verify(key),                          Is.False);
            });

        }

        [Test]
        public void The_CBOR_tag_is_not_covered_by_the_authentication_tag()
        {

            var key       = KeyFor();
            var tagged    = COSEMac0.Create(payload, key);
            var untagged  = COSEMac0.Create(payload, key, null, false, false);

            Assert.Multiple(() => {
                Assert.That(untagged.Tag,                     Is.EqualTo(tagged.Tag));
                Assert.That(untagged.ToByteArray().Length,    Is.EqualTo(tagged.ToByteArray().Length - 1));
            });

        }

        #endregion

        #region A MAC is not a signature, and the code says so

        [Test]
        public void A_signature_algorithm_can_not_authenticate_a_message()
        {

            var key = COSEKey.FromSymmetricKey(secret, null, COSEAlgorithm.ES256);

            var exception = Assert.Throws<COSEException>(() => COSEMac0.Create(payload, key));

            Assert.That(exception!.Message, Does.Contain("not a message authentication algorithm"));

        }

        [Test]
        public void A_MAC_algorithm_can_not_sign_a_message()
        {

            // The other direction, and the one that would be a category error
            // rather than merely a failure: Sign has no branch for this family.
            var exception = Assert.Throws<COSEException>(
                                () => COSEAlgorithm.HMAC256_256.Sign(payload, null!)
                            );

            Assert.That(exception!.Message, Does.Contain("not supported for signing"));

        }

        [Test]
        public void A_key_that_is_not_symmetric_can_not_authenticate_a_message()
        {

            var elliptic = COSEKey.From(
                               Crypto.CalculatePublicKey(
                                   new Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters(
                                       new Org.BouncyCastle.Math.BigInteger("57C92077664146E876760C9520D054AA93C3AFB04E306705DB6090308507B4D3", 16),
                                       COSECurve.P256.DomainParameters!
                                   )
                               ),
                               null,
                               COSEAlgorithm.HMAC256_256
                           );

            var exception = Assert.Throws<COSEException>(() => COSEMac0.Create(payload, elliptic));

            Assert.That(exception!.Message, Does.Contain("key type Symmetric"));

        }

        [Test]
        public void A_key_issued_for_one_algorithm_is_not_talked_into_another()
        {

            // A key issued for HMAC 256/256 must not be used to produce the
            // 64-bit tag: that is a downgrade its holder never agreed to.
            var key       = KeyFor(COSEAlgorithm.HMAC256_256);
            var headers   = COSEHeaders.Create(COSEAlgorithm.HMAC256_64);

            var exception = Assert.Throws<COSEException>(() => COSEMac0.Create(payload, key, headers));

            Assert.That(exception!.Message, Does.Contain("but the key names"));

        }

        #endregion

        #region The symmetric COSE key [RFC 9053, Section 7.3]

        [Test]
        public void A_symmetric_key_is_key_type_four_and_carries_k_under_label_minus_one()
        {

            var key = COSEKey.FromSymmetricKey(secret);

            Assert.Multiple(() => {
                Assert.That(key.KeyType,  Is.EqualTo(COSEKeyType.Symmetric));
                Assert.That(key.K,        Is.EqualTo(secret));
                Assert.That(key.IsPrivate, Is.True);
            });

            Assert.That(COSEKey.TryParse(key.ToCBOR(), out var read, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {
                Assert.That(read!.KeyType,  Is.EqualTo(COSEKeyType.Symmetric));
                Assert.That(read.K,         Is.EqualTo(secret));

                // The same label is the curve on an EC2 key and the public key
                // on an algorithm key pair. Establishing the key type first is
                // what keeps the three apart.
                Assert.That(read.Curve,     Is.Null);
                Assert.That(read.Pub,       Is.Null);
            });

        }

        [Test]
        public void A_symmetric_key_has_no_public_half()
        {

            var exception = Assert.Throws<COSEException>(() => COSEKey.FromSymmetricKey(secret).ToPublicCOSEKey());

            Assert.That(exception!.Message, Does.Contain("no public half"));

        }

        [Test]
        public void A_symmetric_key_without_a_key_value_is_refused()
        {

            var cbor = CBORValue.FromMap([
                           new (COSEKey.KeyTypeLabel, CBORValue.FromInt64(4))
                       ]);

            Assert.That(COSEKey.TryParse(cbor, out _, out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("must carry its key value"));

        }

        [Test]
        public void The_thumbprint_of_a_symmetric_key_covers_kty_and_k_only()
        {

            var withAlgorithm     = COSEKey.FromSymmetricKey(secret, null, COSEAlgorithm.HMAC256_256);
            var withoutAlgorithm  = COSEKey.FromSymmetricKey(secret);

            // Unlike an algorithm key pair, where the algorithm MUST be
            // covered, a symmetric thumbprint is kty and k and nothing else
            // [RFC 9679, Section 4.4].
            Assert.That(withAlgorithm.Thumbprint(), Is.EqualTo(withoutAlgorithm.Thumbprint()));

        }

        #endregion

    }

}
