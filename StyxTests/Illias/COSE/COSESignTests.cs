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

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// COSE_Sign [RFC 9052, Section 4.1]: One payload, several signers.
    ///
    /// The golden vectors are the examples of RFC 9052, Appendix C.1.1 (a
    /// single signature) and C.1.2 (two signatures, one on P-256 and one on
    /// P-521), taken from the machine-readable form of the COSE working group
    /// example repository (https://github.com/cose-wg/Examples) rather than
    /// retyped.
    ///
    /// What separates this from a COSE_Sign1 is the Sig_structure: it has
    /// five elements rather than four, because the protected header bucket of
    /// the individual signature sits between the body and the external data.
    /// </summary>
    [TestFixture]
    public class COSESignTests
    {

        #region Data

        private static readonly Byte[]  payload             = "This is the content.".ToUTF8Bytes();
        private static readonly Byte[]  keyIdentifier11     = "11".ToUTF8Bytes();
        private static readonly Byte[]  keyIdentifierBilbo  = "bilbo.baggins@hobbiton.example".ToUTF8Bytes();

        // The ECDSA P-256 example key of RFC 9052, Appendix C.7.
        private const String  key11X       = "usWxHK2PmfnHKwXPS54m0kTcGJ90UiglWiGahtagnv8";
        private const String  key11Y       = "IBOL-C3BttVivg-lSreASjpkttcsz-1rb7btKLv8EX4";
        private const String  key11D       = "V8kgd2ZBRuh2dgyVINBUqpPDr7BOMGcF22CQMIUHtNM";

        // The ECDSA P-521 example key of RFC 9052, Appendix C.7. Both its x
        // coordinate and its private key begin with a zero byte, since the
        // field of P-521 is 521 bits wide and thus uses only a single bit of
        // its first octet.
        private const String  keyBilboX    = "AHKZLLOsCOzz5cY97ewNUajB957y-C-U88c3v13nmGZx6sYl_oJXu9A5RkTKqjqvjyekWF-7ytDyRXYgCF5cj0Kt";
        private const String  keyBilboY    = "AdymlHvOiLxXkEhayXQnNCvDX4h9htZaCJN34kfmC6pV5OhQHiraVySsUdaQkAgDPrwQrJmbnX9cwlGfP-HqHZR1";
        private const String  keyBilboD    = "AAhRON2r9cqXX1hg-RoI6R1tX5p2rUAYdmpHZoC1XNM56KtscrX6zbKipQrCW9CGZH3T4ubpnoTKLDYJ_fF3_rJt";

        // RFC 9052, Appendix C.1.1 - one signer.
        private const String  oneSignerMessage       = "D8628440A054546869732069732074686520636F6E74656E742E818343A10126A1044231315840E2AEAFD40D69D19DFE6E52077C5D7FF4E408282CBEFB5D06CBF414AF2E19D982AC45AC98B8544C908B4507DE1E90B717C3D34816FE926A2B98F53AFD2FA0F30A";

        // RFC 9052, Appendix C.1.2 - two signers.
        private const String  twoSignersMessage      = "D8628440A054546869732069732074686520636F6E74656E742E828343A10126A1044231315840E2AEAFD40D69D19DFE6E52077C5D7FF4E408282CBEFB5D06CBF414AF2E19D982AC45AC98B8544C908B4507DE1E90B717C3D34816FE926A2B98F53AFD2FA0F30A8344A1013823A104581E62696C626F2E62616767696E7340686F626269746F6E2E6578616D706C65588400A2D28A7C2BDB1587877420F65ADF7D0B9A06635DD1DE64BB62974C863F0B160DD2163734034E6AC003B01E8705524C5C4CA479A952F0247EE8CB0B4FB7397BA08D009E0C8BF482270CC5771AA143966E5A469A09F613488030C5B07EC6D722E3835ADB5B2D8C44E95FFB13877DD2582866883535DE3BB03D01753F83AB87BB4F7A0297";

        private const String  signature0ToBeSigned   = "85695369676E61747572654043A101264054546869732069732074686520636F6E74656E742E";
        private const String  signature1ToBeSigned   = "85695369676E61747572654044A10138234054546869732069732074686520636F6E74656E742E";

        #endregion

        #region (private static) FromBase64URL(Text)

        private static Byte[] FromBase64URL(String Text)

            => Convert.FromBase64String(
                   Text.Replace('-', '+').
                        Replace('_', '/').
                        PadRight(Text.Length + (4 - Text.Length % 4) % 4, '=')
               );

        #endregion

        #region (private static) Key11 () / KeyBilbo()

        private static COSEKey Key11()

            => new (COSEKeyType.EC2,
                    COSECurve.P256,
                    FromBase64URL(key11X),
                    FromBase64URL(key11Y),
                    FromBase64URL(key11D),
                    keyIdentifier11,
                    COSEAlgorithm.ES256);

        private static COSEKey KeyBilbo()

            => new (COSEKeyType.EC2,
                    COSECurve.P521,
                    FromBase64URL(keyBilboX),
                    FromBase64URL(keyBilboY),
                    FromBase64URL(keyBilboD),
                    keyIdentifierBilbo,
                    COSEAlgorithm.ES512);

        #endregion

        #region (private static) GenerateKeyPair(BouncyCastleCurveName)

        private static (ECPrivateKeyParameters PrivateKey, ECPublicKeyParameters PublicKey) GenerateKeyPair(String BouncyCastleCurveName)
        {

            var keyPair = Crypto.GenerateKeys(ECNamedCurveTable.GetByName(BouncyCastleCurveName));

            return ((ECPrivateKeyParameters) keyPair.Private,
                    (ECPublicKeyParameters)  keyPair.Public);

        }

        #endregion


        #region The_published_message_is_parsed_as_documented()

        [Test]
        public void The_published_message_is_parsed_as_documented()
        {

            var sign = COSESign.Parse(Convert.FromHexString(twoSignersMessage));

            Assert.That(sign.IsTagged,                     Is.True);
            Assert.That(sign.IsDetached,                   Is.False);
            Assert.That(sign.Payload,                      Is.EqualTo(payload));

            // Both header buckets of the message body are empty, and the
            // protected one is therefore a zero-length byte string rather
            // than an encoded empty map.
            Assert.That(sign.ProtectedHeaderBytes,         Is.Empty);
            Assert.That(sign.ProtectedHeader.IsEmpty,      Is.True);
            Assert.That(sign.UnprotectedHeader.IsEmpty,    Is.True);

            Assert.That(sign.Signatures.Count,             Is.EqualTo(2));

            var first  = sign.Signatures[0];
            var second = sign.Signatures[1];

            Assert.That(first. Algorithm,                            Is.EqualTo(COSEAlgorithm.ES256));
            Assert.That(first. KeyIdentifier,                        Is.EqualTo(keyIdentifier11));
            Assert.That(Convert.ToHexString(first.ProtectedHeaderBytes),   Is.EqualTo("A10126"));
            Assert.That(first. Signature.Length,                     Is.EqualTo(64));

            Assert.That(second.Algorithm,                            Is.EqualTo(COSEAlgorithm.ES512));
            Assert.That(second.KeyIdentifier,                        Is.EqualTo(keyIdentifierBilbo));
            Assert.That(Convert.ToHexString(second.ProtectedHeaderBytes),  Is.EqualTo("A1013823"));

            // P-521: the group order is 521 bits, so r and s are 66 bytes each.
            Assert.That(second.Signature.Length,                     Is.EqualTo(132));

            // The key identifiers live within the UNPROTECTED buckets and are
            // therefore not covered by anything.
            Assert.That(first. ProtectedHeader.KeyIdentifier,        Is.Null);
            Assert.That(second.ProtectedHeader.KeyIdentifier,        Is.Null);

        }

        #endregion

        #region The_published_signature_inputs_are_byte_exact()

        [Test]
        public void The_published_signature_inputs_are_byte_exact()
        {

            var sign = COSESign.Parse(Convert.FromHexString(twoSignersMessage));

            // The five element Sig_structure: context, body_protected,
            // sign_protected, external_aad, payload. The two signatures differ
            // in exactly one of those five, their own protected bucket.
            Assert.That(Convert.ToHexString(sign.ToBeSigned(sign.Signatures[0])),
                        Is.EqualTo(signature0ToBeSigned));

            Assert.That(Convert.ToHexString(sign.ToBeSigned(sign.Signatures[1])),
                        Is.EqualTo(signature1ToBeSigned));

            // The static entry point for external signers must agree...
            Assert.That(Convert.ToHexString(COSESign.ToBeSigned(sign.ProtectedHeaderBytes,
                                                                sign.Signatures[0].ProtectedHeaderBytes,
                                                                payload)),
                        Is.EqualTo(signature0ToBeSigned));

            // ...and the message of Appendix C.1.1, which carries only the
            // first of the two signers, must produce the very same input.
            var oneSigner = COSESign.Parse(Convert.FromHexString(oneSignerMessage));

            Assert.That(Convert.ToHexString(oneSigner.ToBeSigned(oneSigner.Signatures[0])),
                        Is.EqualTo(signature0ToBeSigned));

            Assert.That(oneSigner.Signatures[0].Signature,
                        Is.EqualTo(sign.Signatures[0].Signature));

        }

        #endregion

        #region Every_published_signature_verifies_with_its_own_key()

        [Test]
        public void Every_published_signature_verifies_with_its_own_key()
        {

            var sign       = COSESign.Parse(Convert.FromHexString(twoSignersMessage));

            var publicKey11     = Key11().   ToPublicKey();
            var publicKeyBilbo  = KeyBilbo().ToPublicKey();

            Assert.That(sign.Verify(sign.Signatures[0], publicKey11,    out var error11),     Is.True, error11);
            Assert.That(sign.Verify(sign.Signatures[1], publicKeyBilbo, out var errorBilbo),  Is.True, errorBilbo);

            // ...and neither key verifies the other's signature.
            Assert.That(sign.Verify(sign.Signatures[0], publicKeyBilbo, out _),  Is.False);
            Assert.That(sign.Verify(sign.Signatures[1], publicKey11,    out _),  Is.False);

        }

        #endregion

        #region The_published_messages_are_re_encoded_byte_exact()

        [Test]
        public void The_published_messages_are_re_encoded_byte_exact()
        {

            foreach (var hex in new[] { oneSignerMessage, twoSignersMessage })
            {

                var sign = COSESign.Parse(Convert.FromHexString(hex));

                Assert.That(Convert.ToHexString(sign.ToByteArray()),
                            Is.EqualTo(hex),
                            hex);

            }

        }

        #endregion

        #region Every_signature_covers_the_body_headers()

        [Test]
        public void Every_signature_covers_the_body_headers()
        {

            var sign        = COSESign.Parse(Convert.FromHexString(twoSignersMessage));

            // Giving the message body a protected header bucket where it had
            // none must break BOTH signatures, since body_protected is the
            // second element of every one of their signature inputs.
            var tampered    = new COSESign(
                                  Convert.FromHexString("A10126"),
                                  sign.UnprotectedHeader,
                                  sign.Payload,
                                  sign.Signatures
                              );

            Assert.That(tampered.Verify(tampered.Signatures[0], Key11().   ToPublicKey(), out var error0),  Is.False);
            Assert.That(tampered.Verify(tampered.Signatures[1], KeyBilbo().ToPublicKey(), out var error1),  Is.False);

            Assert.That(error0,  Is.EqualTo("The signature is invalid!"));
            Assert.That(error1,  Is.EqualTo("The signature is invalid!"));

            // The payload is covered as well, of course.
            var tamperedPayload   = (Byte[]) sign.Payload!.Clone();
            tamperedPayload[0]   ^= 0x01;

            var tamperedMessage   = new COSESign(
                                        sign.ProtectedHeaderBytes,
                                        sign.UnprotectedHeader,
                                        tamperedPayload,
                                        sign.Signatures
                                    );

            Assert.That(tamperedMessage.Verify(tamperedMessage.Signatures[0], Key11().ToPublicKey(), out _),  Is.False);

        }

        #endregion


        #region A_second_signer_signs_an_existing_message()

        [Test]
        public void A_second_signer_signs_an_existing_message()
        {

            var (meterPrivateKey,   meterPublicKey)    = GenerateKeyPair("secp256r1");
            var (backendPrivateKey, backendPublicKey)  = GenerateKeyPair("secp384r1");

            var meterKeyIdentifier    = "meter-4711".  ToUTF8Bytes();
            var backendKeyIdentifier  = "backend-0815".ToUTF8Bytes();

            // The meter signs the reading...
            var signed    = COSESign.Sign(payload,
                                          meterPrivateKey,
                                          COSEAlgorithm.ES256,
                                          meterKeyIdentifier);

            Assert.That(signed.Signatures.Count,  Is.EqualTo(1));

            // ...and the backend counter-signs it later on, without the
            // meter's key and without touching the meter's signature.
            var counterSigned = signed.AddSignature(backendPrivateKey,
                                                    COSEAlgorithm.ES384,
                                                    backendKeyIdentifier);

            Assert.That(counterSigned.Signatures.Count,       Is.EqualTo(2));
            Assert.That(counterSigned.Signatures[0].Signature,  Is.EqualTo(signed.Signatures[0].Signature));
            Assert.That(counterSigned.Payload,                Is.EqualTo(payload));

            // Both signatures verify, each with its own key and algorithm...
            var reparsed = COSESign.Parse(counterSigned.ToByteArray());

            Assert.That(reparsed.Signatures[0].Algorithm,      Is.EqualTo(COSEAlgorithm.ES256));
            Assert.That(reparsed.Signatures[0].KeyIdentifier,  Is.EqualTo(meterKeyIdentifier));
            Assert.That(reparsed.Signatures[0].Signature.Length,  Is.EqualTo(64));

            Assert.That(reparsed.Signatures[1].Algorithm,      Is.EqualTo(COSEAlgorithm.ES384));
            Assert.That(reparsed.Signatures[1].KeyIdentifier,  Is.EqualTo(backendKeyIdentifier));
            Assert.That(reparsed.Signatures[1].Signature.Length,  Is.EqualTo(96));

            Assert.That(reparsed.Verify(reparsed.Signatures[0], meterPublicKey,   out var meterError),    Is.True, meterError);
            Assert.That(reparsed.Verify(reparsed.Signatures[1], backendPublicKey, out var backendError),  Is.True, backendError);

            // ...and neither of them verifies with the other's key.
            Assert.That(reparsed.Verify(reparsed.Signatures[0], backendPublicKey, out _),  Is.False);
            Assert.That(reparsed.Verify(reparsed.Signatures[1], meterPublicKey,   out _),  Is.False);

        }

        #endregion

        #region TryVerifyAny_finds_the_signature_belonging_to_a_key()

        [Test]
        public void TryVerifyAny_finds_the_signature_belonging_to_a_key()
        {

            var sign            = COSESign.Parse(Convert.FromHexString(twoSignersMessage));

            var publicKeyBilbo  = KeyBilbo().ToPublicKey();

            Assert.That(sign.TryVerifyAny(publicKeyBilbo, out var signature, out var errorResponse),  Is.True, errorResponse);
            Assert.That(signature,  Is.SameAs(sign.Signatures[1]));

            // A key that signed nothing here is told so, rather than being
            // quietly reported as one of the signers.
            var (_, strangerPublicKey) = GenerateKeyPair("secp256r1");

            Assert.That(sign.TryVerifyAny(strangerPublicKey, out var none, out var noneError),  Is.False);
            Assert.That(none,       Is.Null);
            Assert.That(noneError,  Is.EqualTo("None of the 2 signature(s) of this COSE_Sign message was verified by the given key!"));

        }

        #endregion

        #region A_signature_of_another_message_is_refused()

        [Test]
        public void A_signature_of_another_message_is_refused()
        {

            var (privateKey, publicKey)  = GenerateKeyPair("secp256r1");

            var one    = COSESign.Sign(payload,                       privateKey, COSEAlgorithm.ES256);
            var other  = COSESign.Sign("Something else.".ToUTF8Bytes(), privateKey, COSEAlgorithm.ES256);

            // Verifying a signature against a body it was not computed over
            // would answer a question nobody asked.
            Assert.That(one.Verify(other.Signatures[0], publicKey, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Is.EqualTo("The given signature is not one of the signatures of this COSE_Sign message!"));

        }

        #endregion

        #region A_detached_payload_is_signed_by_every_signer()

        [Test]
        public void A_detached_payload_is_signed_by_every_signer()
        {

            var (firstPrivateKey,  firstPublicKey)   = GenerateKeyPair("secp256r1");
            var (secondPrivateKey, secondPublicKey)  = GenerateKeyPair("secp256r1");

            var detached  = COSESign.Sign(payload,
                                          firstPrivateKey,
                                          COSEAlgorithm.ES256,
                                          DetachPayload: true).
                                     AddSignature(secondPrivateKey,
                                                  COSEAlgorithm.ES256,
                                                  DetachedPayload: payload);

            Assert.That(detached.IsDetached,                          Is.True);
            Assert.That(Convert.ToHexString(detached.ToByteArray()),  Does.Not.Contain(Convert.ToHexString(payload)));

            Assert.That(detached.Verify(detached.Signatures[0], firstPublicKey,  out var missing),  Is.False);
            Assert.That(missing,  Is.EqualTo("The payload of this COSE_Sign message is detached, therefore it must be supplied for the signature to be computed!"));

            Assert.That(detached.Verify(detached.Signatures[0], firstPublicKey,  out var firstError,  null, payload),  Is.True, firstError);
            Assert.That(detached.Verify(detached.Signatures[1], secondPublicKey, out var secondError, null, payload),  Is.True, secondError);

            // Attaching the payload again keeps every signature valid.
            var attached = detached.Attach(payload);

            Assert.That(attached.Verify(attached.Signatures[0], firstPublicKey,  out var attachedError),  Is.True, attachedError);
            Assert.That(attached.Verify(attached.Signatures[1], secondPublicKey, out _),                  Is.True);

        }

        #endregion

        #region External_authenticated_data_is_signed_but_not_transported()

        [Test]
        public void External_authenticated_data_is_signed_but_not_transported()
        {

            var (privateKey, publicKey)  = GenerateKeyPair("secp256r1");
            var externalAAD              = Convert.FromHexString("11AA22BB33CC44DD55006699");

            var signed = COSESign.Sign(payload,
                                       privateKey,
                                       COSEAlgorithm.ES256,
                                       null,
                                       externalAAD);

            Assert.That(Convert.ToHexString(signed.ToByteArray()),  Does.Not.Contain("11AA22BB33CC44DD55006699"));

            Assert.That(signed.Verify(signed.Signatures[0], publicKey, out var errorResponse, externalAAD),  Is.True, errorResponse);
            Assert.That(signed.Verify(signed.Signatures[0], publicKey, out var withoutAAD),                  Is.False);
            Assert.That(withoutAAD,  Is.EqualTo("The signature is invalid!"));

        }

        #endregion

        #region An_algorithm_that_is_not_integrity_protected_is_not_trusted_silently()

        [Test]
        public void An_algorithm_that_is_not_integrity_protected_is_not_trusted_silently()
        {

            var (_, publicKey)  = GenerateKeyPair("secp256r1");

            var sign            = new COSESign(
                                      [],
                                      null,
                                      payload,
                                      [
                                          new COSESignature(
                                              [],
                                              COSEHeaders.Create(COSEAlgorithm.ES256, keyIdentifier11),
                                              new Byte[64]
                                          )
                                      ]
                                  );

            Assert.That(sign.Signatures[0].Algorithm,                    Is.EqualTo(COSEAlgorithm.ES256));
            Assert.That(sign.Signatures[0].ProtectedHeader.Algorithm,    Is.Null);

            Assert.That(sign.Verify(sign.Signatures[0], publicKey, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Does.Contain("within the unprotected header bucket only"));

            // Naming the expected algorithm gets past the policy, and the
            // signature then fails on its own merits.
            Assert.That(sign.Verify(sign.Signatures[0], publicKey, out var signatureError, null, null, COSEAlgorithm.ES256),  Is.False);
            Assert.That(signatureError,  Is.EqualTo("The signature is invalid!"));

        }

        #endregion

        #region A_critical_header_parameter_that_is_not_understood_is_rejected()

        [Test]
        public void A_critical_header_parameter_that_is_not_understood_is_rejected()
        {

            var (privateKey, publicKey) = GenerateKeyPair("secp256r1");

            // Within the header bucket of the signature...
            var withinSignature = COSESign.Sign(
                                      payload,
                                      privateKey,
                                      new COSEHeaders(
                                          (COSEHeaderLabel.Algorithm, COSEAlgorithm.ES256.ToCBOR()),
                                          (COSEHeaderLabel.Critical,  CBORValue.FromArray(COSEHeaderLabel.X5Chain)),
                                          (COSEHeaderLabel.X5Chain,   CBORValue.FromBytes([0x30, 0x82]))
                                      )
                                  );

            Assert.That(withinSignature.Verify(withinSignature.Signatures[0], publicKey, out var signatureError),  Is.False);
            Assert.That(signatureError,  Is.EqualTo("The \"crit\" header parameter demands that 'x5chain' be understood, which this implementation does not!"));

            // ...and within the header bucket of the message body.
            var withinBody = COSESign.Sign(
                                 payload,
                                 privateKey,
                                 COSEHeaders.Create(COSEAlgorithm.ES256),
                                 null,
                                 null,
                                 false,
                                 true,
                                 new COSEHeaders(
                                     (COSEHeaderLabel.Critical,  CBORValue.FromArray(COSEHeaderLabel.X5Chain)),
                                     (COSEHeaderLabel.X5Chain,   CBORValue.FromBytes([0x30, 0x82]))
                                 )
                             );

            Assert.That(withinBody.Verify(withinBody.Signatures[0], publicKey, out var bodyError),  Is.False);
            Assert.That(bodyError,  Is.EqualTo("The \"crit\" header parameter demands that 'x5chain' be understood, which this implementation does not!"));

        }

        #endregion

        #region Malformed_messages_are_rejected()

        [Test]
        public void Malformed_messages_are_rejected()
        {

            // A COSE_Sign1 (tag 18) is not a COSE_Sign (tag 98)...
            Assert.That(COSESign.TryParse(Convert.FromHexString("D28443A10126A0F640"), out _, out var wrongTag),  Is.False);
            Assert.That(wrongTag,        Does.Contain("must be tagged with CBOR tag 98"));

            // ...a COSE_Sign has four elements, not three...
            Assert.That(COSESign.TryParse(Convert.FromHexString("8340A0F6"), out _, out var tooShort),            Is.False);
            Assert.That(tooShort,        Does.Contain("4 elements"));

            // ...its signatures are an array...
            Assert.That(COSESign.TryParse(Convert.FromHexString("8440A0F640"), out _, out var notAnArray),        Is.False);
            Assert.That(notAnArray,      Does.Contain("signatures of a COSE_Sign message must be a CBOR array"));

            // ...of at least one signature...
            Assert.That(COSESign.TryParse(Convert.FromHexString("8440A0F680"), out _, out var noSignature),       Is.False);
            Assert.That(noSignature,     Does.Contain("at least one signature"));

            // ...and every one of them is an array of three elements.
            Assert.That(COSESign.TryParse(Convert.FromHexString("8440A0F6818240A0"), out _, out var shortSignature),  Is.False);
            Assert.That(shortSignature,  Does.Contain("3 elements"));

        }

        #endregion

    }

}
