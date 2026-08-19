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
    /// Encryption and enveloped authentication: COSE_Encrypt0 (tag 16),
    /// COSE_Encrypt (tag 96) and COSE_Mac (tag 97), with AES-GCM, AES key wrap
    /// and the "direct" recipient algorithm.
    ///
    /// The vectors are published ones throughout, and unusually complete for
    /// this corner of COSE:
    ///
    /// - The COSE working group example repository carries whole messages
    ///   together with their intermediates - the Enc_structure as hex, the
    ///   content key, the nonce and the resulting CBOR. Every one of those
    ///   intermediates is checked here, not merely the final bytes: a message
    ///   that comes out right by way of a wrong AAD is a message that stops
    ///   coming out right the moment anything changes.
    /// - RFC 9052 Appendix C.5.4 is a COSE_Mac whose second recipient wraps
    ///   the content key under a published 256-bit key. Unwrapping it and
    ///   recomputing the tag reproduces the RFC's published value byte for
    ///   byte, which pins the key wrap, the recipient structure, the "MAC"
    ///   context and HMAC in one go.
    /// </summary>
    [TestFixture]
    public class COSEEncryptTests
    {

        #region Data

        private static readonly Byte[] plaintext = Encoding.UTF8.GetBytes("This is the content.");

        private static readonly Byte[] iv        = Convert.FromHexString("02D1F7E6F26C43D4868D87CE");

        /// <summary>The 128-, 192- and 256-bit content keys of the working group's examples.</summary>
        private static readonly Byte[] cek128    = Convert.FromHexString("849B57219DAE48DE646D07DBB533566E");
        private static readonly Byte[] cek192    = Convert.FromHexString("0F1E2D3C4B5A69788796A5B4C3D2E1F01F2E3D4C5B6A7988");
        private static readonly Byte[] cek256    = Convert.FromHexString("0F1E2D3C4B5A69788796A5B4C3D2E1F01F2E3D4C5B6A798897A6B5C4D3E2F100");

        /// <summary>cose-wg aes-gcm-examples/aes-gcm-enc-0{1,2,3}: COSE_Encrypt0.</summary>
        private static readonly (COSEAlgorithm Algorithm, Byte[] Key, String AAD, String CBOR)[] encrypt0 = [

            (COSEAlgorithm.A128GCM, cek128,
             "8368456E63727970743043A1010140",
             "D08343A10101A1054C02D1F7E6F26C43D4868D87CE582460973A94BB2898009EE52ECFD9AB1DD25867374B162E2C03568B41F57C3CC16F9166250A"),

            (COSEAlgorithm.A192GCM, cek192,
             "8368456E63727970743043A1010240",
             "D08343A10102A1054C02D1F7E6F26C43D4868D87CE5824134D3B9223A00C1552C77585C157F467F295919D530FBE21F7689AB3CD4D18FFE8E17CEB"),

            (COSEAlgorithm.A256GCM, cek256,
             "8368456E63727970743043A1010340",
             "D08343A10103A1054C02D1F7E6F26C43D4868D87CE58249D64A5A59A3B04867DCCF6B8EF82F7D1A3B25EF84ECA2BC5D7593A96E943859A9CC24AD3")

        ];

        #endregion


        #region COSE_Encrypt0 with AES-GCM

        [Test]
        public void The_published_Encrypt0_examples_are_reproduced_byte_for_byte()
        {

            foreach (var vector in encrypt0)
            {

                var key      = COSEKey.FromSymmetricKey(vector.Key, null, vector.Algorithm);
                var message  = COSEEncrypt0.Encrypt(plaintext, key, iv);

                Assert.Multiple(() => {

                    // The intermediate first: the Enc_structure is what the
                    // AEAD authenticates, and getting the final bytes right by
                    // way of a wrong one would not survive any change.
                    Assert.That(Convert.ToHexString(message.ToBeEncrypted()),  Is.EqualTo(vector.AAD),  vector.Algorithm.Name);
                    Assert.That(Convert.ToHexString(message.ToByteArray()),    Is.EqualTo(vector.CBOR), vector.Algorithm.Name);

                });

            }

        }

        [Test]
        public void The_published_Encrypt0_examples_are_read_back()
        {

            foreach (var vector in encrypt0)
            {

                Assert.That(COSEEncrypt0.TryParse(Convert.FromHexString(vector.CBOR),
                                                  out var message, out var parseError), Is.True, parseError);

                var key = COSEKey.FromSymmetricKey(vector.Key, null, vector.Algorithm);

                Assert.Multiple(() => {
                    Assert.That(message!.IsTagged,                       Is.True);
                    Assert.That(message.Algorithm,                       Is.EqualTo(vector.Algorithm));
                    Assert.That(message.IV,                              Is.EqualTo(iv));
                });

                Assert.That(message!.Decrypt(key, out var decrypted, out var errorResponse), Is.True, errorResponse);
                Assert.That(decrypted, Is.EqualTo(plaintext));

            }

        }

        [Test]
        public void The_Enc_structure_has_three_elements_and_no_payload()
        {

            // [ "Encrypt0", h'a10101', h'' ] - the payload is what is
            // encrypted, not what is authenticated alongside.
            Assert.That(
                Convert.ToHexString(COSEEncrypt0.EncStructure("Encrypt0", Convert.FromHexString("A10101"))),
                Is.EqualTo("83" + "68" + "456E6372797074" + "30" + "43A10101" + "40")
            );

        }

        [Test]
        public void The_authentication_tag_is_appended_to_the_ciphertext()
        {

            var key      = COSEKey.FromSymmetricKey(cek128, null, COSEAlgorithm.A128GCM);
            var message  = COSEEncrypt0.Encrypt(plaintext, key, iv);

            Assert.That(message.Ciphertext!.Length, Is.EqualTo(plaintext.Length + 16));

        }

        [Test]
        public void A_ciphertext_that_was_altered_is_refused_in_any_single_bit()
        {

            var key       = COSEKey.FromSymmetricKey(cek128, null, COSEAlgorithm.A128GCM);
            var original  = COSEEncrypt0.Encrypt(plaintext, key, iv);

            foreach (var position in new[] { 0, 19, 20, 35 })
            {

                var broken = (Byte[]) original.Ciphertext!.Clone();
                broken[position] ^= 0x01;

                var message = new COSEEncrypt0(original.ProtectedHeaderBytes,
                                               original.UnprotectedHeader,
                                               broken,
                                               original.IsTagged);

                Assert.That(message.Decrypt(key, out _, out _), Is.False, $"byte {position}");

            }

        }

        [Test]
        public void A_protected_bucket_that_was_altered_is_refused_because_it_is_the_AAD()
        {

            var key       = COSEKey.FromSymmetricKey(cek128, null, COSEAlgorithm.A128GCM);
            var original  = COSEEncrypt0.Encrypt(plaintext, key, iv);

            // Say A256GCM in the header while the ciphertext is A128GCM.
            var message = new COSEEncrypt0(Convert.FromHexString("A10103"),
                                           original.UnprotectedHeader,
                                           original.Ciphertext,
                                           original.IsTagged);

            Assert.That(message.Decrypt(key, out _, out _), Is.False);

        }

        [Test]
        public void External_data_is_authenticated_without_being_transported()
        {

            var key       = COSEKey.FromSymmetricKey(cek128, null, COSEAlgorithm.A128GCM);
            var aad       = Convert.FromHexString("11AA22BB33CC44DD55006699");
            var message   = COSEEncrypt0.Encrypt(plaintext, key, iv, aad);

            Assert.Multiple(() => {
                Assert.That(Convert.ToHexString(message.ToByteArray()),  Does.Not.Contain("11AA22BB"));
                Assert.That(message.Decrypt(key, out _, out _, aad),     Is.True);
                Assert.That(message.Decrypt(key, out _, out _),          Is.False);
            });

        }

        [Test]
        public void A_nonce_of_the_wrong_width_is_refused_rather_than_invented()
        {

            // There is no default and there must not be one: a repeated nonce
            // breaks AES-GCM outright, and this library cannot know which ones
            // a caller has already spent.
            var key       = COSEKey.FromSymmetricKey(cek128, null, COSEAlgorithm.A128GCM);
            var exception = Assert.Throws<COSEException>(
                                () => COSEEncrypt0.Encrypt(plaintext, key, Convert.FromHexString("0011"))
                            );

            Assert.That(exception!.Message, Does.Contain("12-byte nonce"));

        }

        [Test]
        public void A_key_of_the_wrong_width_for_the_algorithm_is_refused()
        {

            var wrong     = COSEKey.FromSymmetricKey(cek256, null, COSEAlgorithm.A128GCM);
            var exception = Assert.Throws<COSEException>(() => COSEEncrypt0.Encrypt(plaintext, wrong, iv));

            Assert.That(exception!.Message, Does.Contain("16-byte key"));

        }

        [Test]
        public void A_detached_ciphertext_is_the_same_ciphertext()
        {

            var key       = COSEKey.FromSymmetricKey(cek128, null, COSEAlgorithm.A128GCM);
            var attached  = COSEEncrypt0.Encrypt(plaintext, key, iv);
            var detached  = COSEEncrypt0.Encrypt(plaintext, key, iv, null, true);

            Assert.Multiple(() => {
                Assert.That(detached.Ciphertext,                                            Is.Null);
                Assert.That(detached.Decrypt(key, out _, out _, null, attached.Ciphertext),  Is.True);
                Assert.That(detached.Decrypt(key, out _, out _),                             Is.False);
            });

        }

        #endregion

        #region COSE_Encrypt with recipients

        [Test]
        public void The_published_Encrypt_example_is_reproduced_byte_for_byte()
        {

            // cose-wg aes-gcm-examples/aes-gcm-01, enveloped with one "direct"
            // recipient whose key identifier is "our-secret".
            var key = COSEKey.FromSymmetricKey(cek128,
                                               Encoding.ASCII.GetBytes("our-secret"),
                                               COSEAlgorithm.A128GCM);

            var message = COSEEncrypt.Encrypt(plaintext, key, [COSERecipient.Direct(key)], iv);

            Assert.Multiple(() => {

                Assert.That(Convert.ToHexString(message.ToBeEncrypted()),
                            Is.EqualTo("8367456E637279707443A1010140"));

                Assert.That(Convert.ToHexString(message.ToByteArray()),
                            Is.EqualTo("D8608443A10101A1054C02D1F7E6F26C43D4868D87CE582460973A94BB2898009EE52E" +
                                       "CFD9AB1DD25867374B3581F2C80039826350B97AE2300E42FC818340A20125044A6F" +
                                       "75722D73656372657440"));

            });

        }

        [Test]
        public void The_Encrypt_context_is_not_the_Encrypt0_context()
        {

            foreach (var (algorithm, cek, aad) in new (COSEAlgorithm, Byte[], String)[] {
                         (COSEAlgorithm.A128GCM, cek128, "8367456E637279707443A1010140"),
                         (COSEAlgorithm.A192GCM, cek192, "8367456E637279707443A1010240"),
                         (COSEAlgorithm.A256GCM, cek256, "8367456E637279707443A1010340") })
            {

                var key      = COSEKey.FromSymmetricKey(cek, null, algorithm);
                var message  = COSEEncrypt.Encrypt(plaintext, key, [COSERecipient.Direct(key)], iv);

                Assert.That(Convert.ToHexString(message.ToBeEncrypted()), Is.EqualTo(aad), algorithm.Name);

            }

        }

        [Test]
        public void An_enveloped_message_round_trips_and_decrypts()
        {

            var key = COSEKey.FromSymmetricKey(cek256, null, COSEAlgorithm.A256GCM);

            var written = COSEEncrypt.Encrypt(plaintext, key, [COSERecipient.Direct(key)], iv).ToByteArray();

            Assert.That(COSEEncrypt.TryParse(written, out var message, out var parseError), Is.True, parseError);

            Assert.Multiple(() => {
                Assert.That(message!.Recipients,                       Has.Count.EqualTo(1));
                Assert.That(message.Recipients[0].Algorithm?.Name,     Is.EqualTo("direct"));
                Assert.That(message.Recipients[0].Ciphertext,          Is.Empty);
            });

            Assert.That(message!.Decrypt(key, out var decrypted, out var errorResponse), Is.True, errorResponse);
            Assert.That(decrypted, Is.EqualTo(plaintext));

        }

        [Test]
        public void A_direct_recipient_carrying_key_material_is_refused()
        {

            var key       = COSEKey.FromSymmetricKey(cek128, null, COSEAlgorithm.A128GCM);
            var original  = COSEEncrypt.Encrypt(plaintext, key, [COSERecipient.Direct(key)], iv);

            // RFC 9053 Section 6.1.1: nothing is transported by this route, so
            // a non-empty ciphertext means the structure is not what it claims.
            var tampered = new COSEEncrypt(
                               original.ProtectedHeaderBytes,
                               original.UnprotectedHeader,
                               original.Ciphertext,
                               [new COSERecipient(original.Recipients[0].ProtectedHeaderBytes,
                                                  original.Recipients[0].UnprotectedHeader,
                                                  Convert.FromHexString("00112233"))],
                               original.IsTagged);

            Assert.That(tampered.Decrypt(key, out _, out _), Is.False);

        }

        #endregion

        #region AES key wrap

        [Test]
        public void The_content_key_of_RFC_9052_Appendix_C_5_4_is_recovered()
        {

            var kek      = Convert.FromHexString("849B57219DAE48DE646D07DBB533566E976686457C1491BE3A76DCEA6C427188");
            var wrapped  = Convert.FromHexString("0B2C7CFCE04E98276342D6476A7723C090DFDD15F9A518E7736549E998370695E6D6A83B4AE507BB");

            var cek = COSEAlgorithm.A256KW.UnwrapKey(wrapped, kek);

            Assert.That(cek, Is.Not.Null);
            Assert.That(Convert.ToHexString(cek!),
                        Is.EqualTo("2B7459201E5046E33FDB514C5E14A1B01D9893F8936335F821FCB1AFF450B226"));

            // Deterministic, which is the property that makes this checkable
            // at all - and safe only because what it wraps is a random key.
            Assert.That(COSEAlgorithm.A256KW.WrapKey(cek!, kek), Is.EqualTo(wrapped));

        }

        [Test]
        public void An_unwrap_under_the_wrong_key_fails_rather_than_returning_rubbish()
        {

            var wrapped = Convert.FromHexString("0B2C7CFCE04E98276342D6476A7723C090DFDD15F9A518E7736549E998370695E6D6A83B4AE507BB");

            Assert.That(COSEAlgorithm.A256KW.UnwrapKey(wrapped, new Byte[32]), Is.Null);

        }

        [Test]
        public void The_key_wrap_algorithm_is_named_after_the_key_encryption_key()
        {

            Assert.Multiple(() => {
                Assert.That(COSEAlgorithm.ForKeyWrap(16), Is.EqualTo(COSEAlgorithm.A128KW));
                Assert.That(COSEAlgorithm.ForKeyWrap(24), Is.EqualTo(COSEAlgorithm.A192KW));
                Assert.That(COSEAlgorithm.ForKeyWrap(32), Is.EqualTo(COSEAlgorithm.A256KW));
            });

            Assert.Throws<COSEException>(() => COSEAlgorithm.ForKeyWrap(20));

        }

        [Test]
        public void A_wrapped_key_grows_by_the_eight_bytes_of_the_check_value()
        {

            var kek = Convert.FromHexString("849B57219DAE48DE646D07DBB533566E");

            Assert.Multiple(() => {
                Assert.That(COSEAlgorithm.A128KW.WrapKey(cek128, kek).Length, Is.EqualTo(16 + 8));
                Assert.That(COSEAlgorithm.A128KW.WrapKey(cek256, kek).Length, Is.EqualTo(32 + 8));
            });

        }

        #endregion

        #region COSE_Mac with recipients

        [Test]
        public void The_published_tag_of_RFC_9052_Appendix_C_5_4_is_reproduced()
        {

            var kek      = Convert.FromHexString("849B57219DAE48DE646D07DBB533566E976686457C1491BE3A76DCEA6C427188");
            var wrapped  = Convert.FromHexString("0B2C7CFCE04E98276342D6476A7723C090DFDD15F9A518E7736549E998370695E6D6A83B4AE507BB");

            // The whole chain in one test: unwrap the content key with A256KW,
            // build the MAC_structure with the "MAC" context, and recompute the
            // tag the RFC prints.
            var cek         = COSEAlgorithm.A256KW.UnwrapKey(wrapped, kek)!;
            var contentKey  = COSEKey.FromSymmetricKey(cek, null, COSEAlgorithm.HMAC256_256);

            var message = COSEMac.Create(plaintext, contentKey,
                                         [COSERecipient.KeyWrap(cek, COSEKey.FromSymmetricKey(kek))]);

            Assert.Multiple(() => {

                Assert.That(Convert.ToHexString(message.Tag),
                            Is.EqualTo("BF48235E809B5C42E995F2B7D5FA13620E7ED834E337F6AA43DF161E49E9323E"));

                // ...and the MAC_structure differs from a COSE_Mac0's in one
                // string: "MAC" rather than "MAC0".
                Assert.That(Convert.ToHexString(message.ToBeMACed()),
                            Is.EqualTo("84" + "63" + "4D4143" + "43A10105" + "40" + "54" + Convert.ToHexString(plaintext)));

            });

        }

        [Test]
        public void A_wrapped_MAC_verifies_with_the_key_encryption_key_alone()
        {

            var kek         = Convert.FromHexString("849B57219DAE48DE646D07DBB533566E976686457C1491BE3A76DCEA6C427188");
            var cek         = cek256;
            var contentKey  = COSEKey.FromSymmetricKey(cek, null, COSEAlgorithm.HMAC256_256);
            var kekKey      = COSEKey.FromSymmetricKey(kek);

            var written = COSEMac.Create(plaintext, contentKey,
                                         [COSERecipient.KeyWrap(cek, kekKey)]).ToByteArray();

            Assert.That(COSEMac.TryParse(written, out var message, out var parseError), Is.True, parseError);

            Assert.Multiple(() => {
                Assert.That(message!.Verify(kekKey, out _),                                          Is.True);
                Assert.That(message.Verify(COSEKey.FromSymmetricKey(new Byte[32]), out _),           Is.False);
            });

        }

        [Test]
        public void A_COSE_Mac_with_one_direct_recipient_is_a_COSE_Mac0_with_ceremony()
        {

            var key      = COSEKey.FromSymmetricKey(cek256, null, COSEAlgorithm.HMAC256_256);
            var message  = COSEMac.Create(plaintext, key, [COSERecipient.Direct(key)]);

            Assert.Multiple(() => {
                Assert.That(message.Recipients[0].ProtectedHeaderBytes,  Is.Empty);
                Assert.That(message.Recipients[0].Ciphertext,            Is.Empty);
                Assert.That(message.Verify(key, out _),                  Is.True);
            });

        }

        [Test]
        public void One_content_key_reaches_several_recipients()
        {

            var contentKey = COSEKey.FromSymmetricKey(cek256, null, COSEAlgorithm.HMAC256_256);

            var alice = COSEKey.FromSymmetricKey(Enumerable.Repeat((Byte) 0x00, 32).ToArray(), Encoding.ASCII.GetBytes("alice"));
            var bob   = COSEKey.FromSymmetricKey(Enumerable.Repeat((Byte) 0x11, 16).ToArray(), Encoding.ASCII.GetBytes("bob"));

            var written = COSEMac.Create(plaintext, contentKey,
                                         [COSERecipient.KeyWrap(cek256, alice),
                                          COSERecipient.KeyWrap(cek256, bob)]).ToByteArray();

            Assert.That(COSEMac.TryParse(written, out var message, out var parseError), Is.True, parseError);

            Assert.Multiple(() => {

                Assert.That(message!.Recipients,                    Has.Count.EqualTo(2));
                Assert.That(message.Recipients[0].Algorithm?.Name,  Is.EqualTo("A256KW"));
                Assert.That(message.Recipients[1].Algorithm?.Name,  Is.EqualTo("A128KW"));

                // Both get in, each through their own entry - and that is
                // exactly the property that makes the tag say nothing about
                // which of them wrote it.
                Assert.That(message.Verify(alice, out _),           Is.True);
                Assert.That(message.Verify(bob,   out _),           Is.True);

                Assert.That(message.Verify(COSEKey.FromSymmetricKey(Enumerable.Repeat((Byte) 0x22, 32).ToArray()), out _),
                            Is.False);

            });

        }

        [Test]
        public void A_COSE_Mac_without_recipients_is_refused()
        {

            var key = COSEKey.FromSymmetricKey(cek256, null, COSEAlgorithm.HMAC256_256);

            var exception = Assert.Throws<COSEException>(() => COSEMac.Create(plaintext, key, []));

            Assert.That(exception!.Message, Does.Contain("at least one recipient"));

        }

        [Test]
        public void A_changed_payload_is_refused()
        {

            var key       = COSEKey.FromSymmetricKey(cek256, null, COSEAlgorithm.HMAC256_256);
            var original  = COSEMac.Create(plaintext, key, [COSERecipient.Direct(key)]);

            var tampered = new COSEMac(original.ProtectedHeaderBytes,
                                       original.UnprotectedHeader,
                                       Encoding.UTF8.GetBytes("This is the contenu."),
                                       original.Tag,
                                       original.Recipients,
                                       original.IsTagged);

            Assert.That(tampered.Verify(key, out _), Is.False);

        }

        #endregion

    }

}
