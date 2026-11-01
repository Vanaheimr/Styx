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

using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.UnitTests.COSE
{

    /// <summary>
    /// ChaCha20/Poly1305 [RFC 9053, Section 4.3], pinned the way AES-GCM is:
    /// the primitive against RFC 8439's own AEAD example, the COSE structures
    /// against the COSE working group's two examples - together with the
    /// Enc_structure they publish, since a message that comes out right by way
    /// of a wrong additional-data structure stops coming out right the moment
    /// anything changes. COSE.TS is held to the very same values.
    /// </summary>
    [TestFixture]
    public class COSEChaCha20Poly1305Tests
    {

        #region Data

        private static readonly Byte[] content = Encoding.UTF8.GetBytes("This is the content.");

        /// <summary>The 256-bit key of the working group's examples, kid "sec-256".</summary>
        private static readonly Byte[] key256  = Convert.FromHexString("0F1E2D3C4B5A69788796A5B4C3D2E1F01F2E3D4C5B6A798897A6B5C4D3E2F100");

        #endregion


        [Test]
        public void The_primitive_reproduces_the_AEAD_example_of_RFC_8439()
        {

            var key       = Convert.FromHexString("808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9F");
            var nonce     = Convert.FromHexString("070000004041424344454647");
            var aad       = Convert.FromHexString("50515253C0C1C2C3C4C5C6C7");
            var plaintext = Encoding.UTF8.GetBytes("Ladies and Gentlemen of the class of '99: If I could offer you only one tip for the future, sunscreen would be it.");

            var sealed_   = COSEAlgorithm.ChaCha20Poly1305.Encrypt(plaintext, key, nonce, aad);

            Assert.That(Convert.ToHexString(sealed_), Is.EqualTo(
                "D31A8D34648E60DB7B86AFBC53EF7EC2A4ADED51296E08FEA9E2B5A736EE62D63DBEA45E8CA9671282FAFB69DA92728B" +
                "1A71DE0A9E060B2905D6A5B67ECD3B3692DDBD7F2D778B8C9803AEE328091B58FAB324E4FAD675945585808B4831D7BC" +
                "3FF4DEF08E4B7A9DE576D26586CEC64B6116" +
                "1AE10B594F09E26A7E902ECBD0600691"));

            Assert.That(COSEAlgorithm.ChaCha20Poly1305.Decrypt(sealed_, key, nonce, aad),  Is.EqualTo(plaintext));

        }

        [Test]
        public void A_key_or_nonce_of_the_wrong_width_is_refused_and_damage_answered_with_null()
        {

            Assert.Throws<COSEException>(() => COSEAlgorithm.ChaCha20Poly1305.Encrypt(content, new Byte[16], new Byte[12], []));
            Assert.Throws<COSEException>(() => COSEAlgorithm.ChaCha20Poly1305.Encrypt(content, key256,      new Byte[8],  []));

            var sealed_ = COSEAlgorithm.ChaCha20Poly1305.Encrypt(content, key256, new Byte[12], []);
            sealed_[0] ^= 0x01;

            Assert.That(COSEAlgorithm.ChaCha20Poly1305.Decrypt(sealed_,      key256, new Byte[12], []),  Is.Null);
            Assert.That(COSEAlgorithm.ChaCha20Poly1305.Decrypt(new Byte[15], key256, new Byte[12], []),  Is.Null);

        }

        [Test]
        public void It_is_algorithm_24_with_one_key_width()
        {

            Assert.Multiple(() => {
                Assert.That(COSEAlgorithm.ChaCha20Poly1305.Value,                Is.EqualTo(24));
                Assert.That(COSEAlgorithm.ChaCha20Poly1305.Name,                 Is.EqualTo("ChaCha20/Poly1305"));
                Assert.That(COSEAlgorithm.ChaCha20Poly1305.KeySizeInBytes,       Is.EqualTo(32));
                Assert.That(COSEAlgorithm.ChaCha20Poly1305.IsContentEncryption,  Is.True);
            });

        }

        [Test]
        public void It_writes_chacha_poly_enc_01_a_COSE_Encrypt0()
        {

            var key     = COSEKey.FromSymmetricKey(key256, null, COSEAlgorithm.ChaCha20Poly1305);
            var message = COSEEncrypt0.Encrypt(content, key, Convert.FromHexString("5C3A9950BD2852F66E6C8D4F"));

            Assert.That(Convert.ToHexString(message.ToBeEncrypted()),  Is.EqualTo("8368456E63727970743044A101181840"));
            Assert.That(Convert.ToHexString(message.ToByteArray()),    Is.EqualTo(
                "D08344A1011818A1054C5C3A9950BD2852F66E6C8D4F5824CA119C45926DA993D29B5D0CAC9A84228C7668D492A1B9D7E32020EF21372E74DEF431B9"));

            Assert.That(COSEEncrypt0.TryParse(message.ToByteArray(), out var parsed, out var parseError),  Is.True, parseError);
            Assert.That(parsed!.Decrypt(key, out var opened, out var errorResponse),                         Is.True, errorResponse);
            Assert.That(opened,                                                                              Is.EqualTo(content));

        }

        [Test]
        public void It_writes_chacha_poly_01_a_COSE_Encrypt_with_a_direct_recipient()
        {

            var named   = COSEKey.FromSymmetricKey(key256, Encoding.UTF8.GetBytes("sec-256"), COSEAlgorithm.ChaCha20Poly1305);
            var message = COSEEncrypt.Encrypt(content,
                                              COSEKey.FromSymmetricKey(key256, null, COSEAlgorithm.ChaCha20Poly1305),
                                              [COSERecipient.Direct(named)],
                                              Convert.FromHexString("26682306D4FB28CA01B43B80"));

            Assert.That(Convert.ToHexString(message.ToBeEncrypted()),  Is.EqualTo("8367456E637279707444A101181840"));
            Assert.That(Convert.ToHexString(message.ToByteArray()),    Is.EqualTo(
                "D8608444A1011818A1054C26682306D4FB28CA01B43B8058241CD5D49DAA014CCAFFB30E765DC5CD410689AAE1C60B4564" +
                "8853298FF6808DB3FA8235DB818340A2012504477365632D32353640"));

            Assert.That(message.Decrypt(named, out _, out var errorResponse),  Is.True, errorResponse);

        }

        [Test]
        public void A_key_of_another_width_is_refused_and_an_altered_message_opens_nothing()
        {

            var exception = Assert.Throws<COSEException>(
                                () => COSEEncrypt0.Encrypt(content, COSEKey.FromSymmetricKey(new Byte[16], null, COSEAlgorithm.ChaCha20Poly1305), new Byte[12])
                            );

            Assert.That(exception!.Message,  Does.Contain("32-byte key"));

            var key     = COSEKey.FromSymmetricKey(key256, null, COSEAlgorithm.ChaCha20Poly1305);
            var sealed_ = COSEEncrypt0.Encrypt(content, key, new Byte[12]);
            var broken  = (Byte[]) sealed_.Ciphertext!.Clone();
            broken[3] ^= 0x01;

            var altered = new COSEEncrypt0(sealed_.ProtectedHeaderBytes, sealed_.UnprotectedHeader, broken);

            Assert.That(altered.Decrypt(key, out _, out _),  Is.False);

        }

        [Test]
        public void ECDH_ES_agrees_a_256_bit_key_for_it()
        {

            var sender   = COSEKey.From(new X25519PrivateKeyParameters(Enumerable.Repeat((Byte) 0x11, 32).ToArray(), 0));
            var receiver = COSEKey.From(new X25519PrivateKeyParameters(Enumerable.Repeat((Byte) 0x22, 32).ToArray(), 0));
            var agreed   = COSERecipient.ECDHDirect(COSEKey.From(receiver.ToPublicKey()), sender, COSEAlgorithm.ChaCha20Poly1305);

            Assert.That(agreed.ContentKey.K,  Has.Length.EqualTo(32));

            var message  = COSEEncrypt.Encrypt(content, agreed.ContentKey, [agreed.Recipient], Enumerable.Repeat((Byte) 7, 12).ToArray());

            Assert.That(message.Decrypt(receiver, out var opened, out var errorResponse),  Is.True, errorResponse);
            Assert.That(opened,                                                              Is.EqualTo(content));

        }

    }

}
