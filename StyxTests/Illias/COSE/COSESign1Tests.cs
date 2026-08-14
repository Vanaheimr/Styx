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
    /// COSE_Sign1 [RFC 9052, Section 4.2].
    ///
    /// The golden vectors are the ECDSA examples of RFC 9052, Appendix C.2.1
    /// and the "sign1-tests" of the COSE working group example repository
    /// (https://github.com/cose-wg/Examples), taken from their machine-readable
    /// form rather than retyped.
    ///
    /// ECDSA signatures are randomized, so the published signature bytes can
    /// not be reproduced by signing - but they can be verified, and verifying
    /// them is the stronger statement anyway: A single wrong byte anywhere
    /// within the Sig_structure, the header buckets or the key would make the
    /// verification fail.
    /// </summary>
    [TestFixture]
    public class COSESign1Tests
    {

        #region Data

        /// <summary>
        /// The payload of all published examples.
        /// </summary>
        private static readonly Byte[]  payload            = "This is the content.".ToUTF8Bytes();

        /// <summary>
        /// The key identifier of the example key: The two characters "11".
        /// </summary>
        private static readonly Byte[]  keyIdentifier      = "11".ToUTF8Bytes();

        /// <summary>
        /// The external additional authenticated data of "sign-pass-02".
        /// </summary>
        private static readonly Byte[]  externalAAD        = Convert.FromHexString("11AA22BB33CC44DD55006699");

        // The ECDSA P-256 example key of RFC 9052, Appendix C.7, base64url
        // encoded exactly as within the example repository.
        private const String  exampleKeyX                  = "usWxHK2PmfnHKwXPS54m0kTcGJ90UiglWiGahtagnv8";
        private const String  exampleKeyY                  = "IBOL-C3BttVivg-lSreASjpkttcsz-1rb7btKLv8EX4";
        private const String  exampleKeyD                  = "V8kgd2ZBRuh2dgyVINBUqpPDr7BOMGcF22CQMIUHtNM";

        // RFC 9052, Appendix C.2.1 - a single ECDSA signature.
        private const String  appendixC21Message           = "D28443A10126A10442313154546869732069732074686520636F6E74656E742E58408EB33E4CA31D1C465AB05AAC34CC6B23D58FEF5C083106C4D25A91AEF0B0117E2AF9A291AA32E14AB834DC56ED2A223444547E01F11D3B0916E5A4C345CACB36";
        private const String  appendixC21ToBeSigned        = "846A5369676E61747572653143A101264054546869732069732074686520636F6E74656E742E";

        // "sign-pass-02": The very same message, but signed with external
        // additional authenticated data, which is not transported.
        private const String  externalAADMessage           = "D28443A10126A10442313154546869732069732074686520636F6E74656E742E584010729CD711CB3813D8D8E944A8DA7111E7B258C9BDCA6135F7AE1ADBEE9509891267837E1E33BD36C150326AE62755C6BD8E540C3E8F92D7D225E8DB72B8820B";
        private const String  externalAADToBeSigned        = "846A5369676E61747572653143A101264C11AA22BB33CC44DD5500669954546869732069732074686520636F6E74656E742E";

        // "sign-pass-03": The message of Appendix C.2.1 without its CBOR tag.
        private const String  untaggedMessage              = "8443A10126A10442313154546869732069732074686520636F6E74656E742E58408EB33E4CA31D1C465AB05AAC34CC6B23D58FEF5C083106C4D25A91AEF0B0117E2AF9A291AA32E14AB834DC56ED2A223444547E01F11D3B0916E5A4C345CACB36";

        #endregion

        #region (private static) FromBase64URL  (Text)

        /// <summary>
        /// Decode base64url as used by the COSE example key material.
        /// </summary>
        private static Byte[] FromBase64URL(String Text)

            => Convert.FromBase64String(
                   Text.Replace('-', '+').
                        Replace('_', '/').
                        PadRight(Text.Length + (4 - Text.Length % 4) % 4, '=')
               );

        #endregion

        #region (private static) ExamplePublicKey ()

        /// <summary>
        /// The public part of the ECDSA P-256 example key of RFC 9052.
        /// </summary>
        private static ECPublicKeyParameters ExamplePublicKey()

            => new COSEKey(
                   COSEKeyType.EC2,
                   COSECurve.P256,
                   FromBase64URL(exampleKeyX),
                   FromBase64URL(exampleKeyY),
                   null,
                   keyIdentifier,
                   COSEAlgorithm.ES256
               ).ToPublicKey();

        #endregion

        #region (private static) ExamplePrivateKey()

        /// <summary>
        /// The private part of the ECDSA P-256 example key of RFC 9052.
        /// </summary>
        private static ECPrivateKeyParameters ExamplePrivateKey()

            => new COSEKey(
                   COSEKeyType.EC2,
                   COSECurve.P256,
                   FromBase64URL(exampleKeyX),
                   FromBase64URL(exampleKeyY),
                   FromBase64URL(exampleKeyD),
                   keyIdentifier,
                   COSEAlgorithm.ES256
               ).ToPrivateKey();

        #endregion

        #region (private static) GenerateKeyPair(BouncyCastleCurveName)

        /// <summary>
        /// Generate a fresh elliptic curve key pair on the given curve.
        /// </summary>
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

            var sign1 = COSESign1.Parse(Convert.FromHexString(appendixC21Message));

            Assert.That(sign1.IsTagged,                                     Is.True);
            Assert.That(sign1.IsDetached,                                   Is.False);

            // The protected bucket is the encoded map {1: -7}...
            Assert.That(Convert.ToHexString(sign1.ProtectedHeaderBytes),    Is.EqualTo("A10126"));
            Assert.That(sign1.ProtectedHeader.Algorithm,                    Is.EqualTo(COSEAlgorithm.ES256));
            Assert.That(sign1.ProtectedHeader.Count,                        Is.EqualTo(1));

            // ...the key identifier is unprotected and thus NOT signed...
            Assert.That(sign1.UnprotectedHeader.KeyIdentifier,              Is.EqualTo(keyIdentifier));
            Assert.That(sign1.ProtectedHeader.KeyIdentifier,                Is.Null);
            Assert.That(sign1.KeyIdentifier,                                Is.EqualTo(keyIdentifier));

            Assert.That(sign1.Payload,                                      Is.EqualTo(payload));
            Assert.That(sign1.Payload!.ToUTF8String(),                      Is.EqualTo("This is the content."));

            // ...and an ECDSA signature on P-256 is r and s concatenated,
            // which is 2 * 32 bytes and never a DER encoding.
            Assert.That(sign1.Signature.Length,                             Is.EqualTo(64));

            Assert.That(sign1.Algorithm,                                    Is.EqualTo(COSEAlgorithm.ES256));

        }

        #endregion

        #region The_published_signature_input_is_byte_exact()

        [Test]
        public void The_published_signature_input_is_byte_exact()
        {

            var sign1 = COSESign1.Parse(Convert.FromHexString(appendixC21Message));

            Assert.That(Convert.ToHexString(sign1.ToBeSigned()),
                        Is.EqualTo(appendixC21ToBeSigned));

            // The static entry point for external signers must agree...
            Assert.That(Convert.ToHexString(COSESign1.ToBeSigned(sign1.ProtectedHeaderBytes,
                                                                 payload)),
                        Is.EqualTo(appendixC21ToBeSigned));

        }

        #endregion

        #region The_published_signature_verifies_with_the_published_key()

        [Test]
        public void The_published_signature_verifies_with_the_published_key()
        {

            var sign1     = COSESign1.Parse(Convert.FromHexString(appendixC21Message));
            var verified  = sign1.Verify(ExamplePublicKey(), out var errorResponse);

            Assert.That(verified,       Is.True, errorResponse);
            Assert.That(errorResponse,  Is.Null);

        }

        #endregion

        #region The_published_message_is_re_encoded_byte_exact()

        [Test]
        public void The_published_message_is_re_encoded_byte_exact()
        {

            foreach (var hex in new[] { appendixC21Message, externalAADMessage, untaggedMessage })
            {

                var sign1 = COSESign1.Parse(Convert.FromHexString(hex));

                Assert.That(Convert.ToHexString(sign1.ToByteArray()),
                            Is.EqualTo(hex),
                            hex);

            }

        }

        #endregion

        #region External_authenticated_data_is_signed_but_not_transported()

        [Test]
        public void External_authenticated_data_is_signed_but_not_transported()
        {

            var sign1     = COSESign1.Parse(Convert.FromHexString(externalAADMessage));
            var publicKey = ExamplePublicKey();

            // The external data appears within the signature input...
            Assert.That(Convert.ToHexString(sign1.ToBeSigned(externalAAD)),
                        Is.EqualTo(externalAADToBeSigned));

            // ...but nowhere within the message itself, which is byte for byte
            // the message of Appendix C.2.1 except for the signature.
            Assert.That(Convert.ToHexString(sign1.ToByteArray()),
                        Does.Not.Contain("11AA22BB33CC44DD55006699"));

            Assert.That(sign1.Verify(publicKey, out var errorResponse, externalAAD),  Is.True, errorResponse);

            // Whoever does not know the external data can not verify...
            Assert.That(sign1.Verify(publicKey, out var withoutAAD),                  Is.False);
            Assert.That(withoutAAD,                                                   Is.EqualTo("The signature is invalid!"));

            // ...and neither can whoever guesses it wrongly.
            Assert.That(sign1.Verify(publicKey, out _, Convert.FromHexString("11AA22BB33CC44DD55006698")),  Is.False);

        }

        #endregion

        #region The_CBOR_tag_is_not_covered_by_the_signature()

        [Test]
        public void The_CBOR_tag_is_not_covered_by_the_signature()
        {

            var tagged    = COSESign1.Parse(Convert.FromHexString(appendixC21Message));
            var untagged  = COSESign1.Parse(Convert.FromHexString(untaggedMessage));

            Assert.That(tagged.  IsTagged,  Is.True);
            Assert.That(untagged.IsTagged,  Is.False);

            // Removing the tag changes neither the signature input...
            Assert.That(Convert.ToHexString(untagged.ToBeSigned()),
                        Is.EqualTo(Convert.ToHexString(tagged.ToBeSigned())));

            // ...nor the signature itself...
            Assert.That(untagged.Signature,  Is.EqualTo(tagged.Signature));

            // ...and both verify with the very same key.
            Assert.That(untagged.Verify(ExamplePublicKey(), out var errorResponse),  Is.True, errorResponse);

        }

        #endregion

        #region A_tampered_payload_does_not_verify()

        [Test]
        public void A_tampered_payload_does_not_verify()
        {

            var sign1      = COSESign1.Parse(Convert.FromHexString(appendixC21Message));
            var publicKey  = ExamplePublicKey();

            // A single flipped bit within the payload...
            var tamperedPayload     = (Byte[]) sign1.Payload!.Clone();
            tamperedPayload[0]     ^= 0x01;

            var tamperedMessage     = new COSESign1(
                                          sign1.ProtectedHeaderBytes,
                                          sign1.UnprotectedHeader,
                                          tamperedPayload,
                                          sign1.Signature
                                      );

            Assert.That(tamperedMessage.Verify(publicKey, out var payloadError),  Is.False);
            Assert.That(payloadError,  Is.EqualTo("The signature is invalid!"));

            // ...and a single flipped bit within the protected header bucket,
            // which changes the algorithm from ES256 (-7) to ES384 (-35).
            var tamperedHeader      = new COSESign1(
                                          Convert.FromHexString("A1013822"),
                                          sign1.UnprotectedHeader,
                                          sign1.Payload,
                                          sign1.Signature
                                      );

            Assert.That(tamperedHeader.ProtectedHeader.Algorithm,                Is.EqualTo(COSEAlgorithm.ES384));
            Assert.That(tamperedHeader.Verify(publicKey, out var headerError),   Is.False);
            Assert.That(headerError,   Is.EqualTo("The signature is invalid!"));

        }

        #endregion

        #region A_different_key_does_not_verify()

        [Test]
        public void A_different_key_does_not_verify()
        {

            var sign1 = COSESign1.Parse(Convert.FromHexString(appendixC21Message));

            var (_, otherPublicKey) = GenerateKeyPair("secp256r1");

            Assert.That(sign1.Verify(otherPublicKey, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Is.EqualTo("The signature is invalid!"));

        }

        #endregion


        #region Signing_and_verifying_roundtrips_on_every_supported_curve()

        [Test]
        public void Signing_and_verifying_roundtrips_on_every_supported_curve()
        {

            var vectors = new (COSEAlgorithm Algorithm, String CurveName, Int32 SignatureLength)[] {
                              (COSEAlgorithm.ES256,   "secp256r1",       64),
                              (COSEAlgorithm.ES384,   "secp384r1",       96),
                              (COSEAlgorithm.ES512,   "secp521r1",      132),
                              (COSEAlgorithm.ESP256,  "secp256r1",       64),
                              (COSEAlgorithm.ESP384,  "secp384r1",       96),
                              (COSEAlgorithm.ESP512,  "secp521r1",      132),
                              (COSEAlgorithm.ESB256,  "brainpoolP256r1", 64),
                              (COSEAlgorithm.ESB320,  "brainpoolP320r1", 80),
                              (COSEAlgorithm.ESB384,  "brainpoolP384r1", 96),
                              (COSEAlgorithm.ESB512,  "brainpoolP512r1", 128)
                          };

            foreach (var vector in vectors)
            {

                var (privateKey, publicKey)  = GenerateKeyPair(vector.CurveName);

                var signed                   = COSESign1.Sign(payload,
                                                              privateKey,
                                                              vector.Algorithm,
                                                              keyIdentifier);

                // The signature is r and s concatenated, each zero-padded to
                // the width of the group order - never a DER encoding, whose
                // length would vary from signature to signature.
                Assert.That(signed.Signature.Length,  Is.EqualTo(vector.SignatureLength),  vector.Algorithm.Name);

                var reparsed                 = COSESign1.Parse(signed.ToByteArray());

                Assert.That(reparsed.Algorithm,                       Is.EqualTo(vector.Algorithm),  vector.Algorithm.Name);
                Assert.That(reparsed.KeyIdentifier,                   Is.EqualTo(keyIdentifier),     vector.Algorithm.Name);
                Assert.That(reparsed.Payload,                         Is.EqualTo(payload),           vector.Algorithm.Name);
                Assert.That(reparsed.Verify(publicKey, out var errorResponse),  Is.True,             $"{vector.Algorithm.Name}: {errorResponse}");

            }

        }

        #endregion

        #region A_fully_specified_algorithm_rejects_a_key_on_another_curve()

        [Test]
        public void A_fully_specified_algorithm_rejects_a_key_on_another_curve()
        {

            var (privateKey, _) = GenerateKeyPair("secp384r1");

            // ESP256 is defined for P-256 only, whereas the deprecated ES256
            // leaves the curve to the key and therefore accepts it.
            Assert.That(() => COSESign1.Sign(payload, privateKey, COSEAlgorithm.ESP256),
                        Throws.TypeOf<COSEException>());

            Assert.That(() => COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256),
                        Throws.Nothing);

        }

        #endregion

        #region A_detached_payload_is_signed_but_not_transported()

        [Test]
        public void A_detached_payload_is_signed_but_not_transported()
        {

            var (privateKey, publicKey)  = GenerateKeyPair("secp256r1");

            var attached                 = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256, keyIdentifier);
            var detached                 = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256, keyIdentifier, DetachPayload: true);

            Assert.That(detached.IsDetached,                          Is.True);
            Assert.That(detached.Payload,                             Is.Null);
            Assert.That(Convert.ToHexString(detached.ToByteArray()),  Does.Not.Contain(Convert.ToHexString(payload)));

            // The payload has to be supplied for the signature to be checked...
            Assert.That(detached.Verify(publicKey, out var missingPayload),                      Is.False);
            Assert.That(missingPayload,  Is.EqualTo("The payload of this COSE_Sign1 message is detached, therefore it must be supplied for the signature to be computed!"));

            Assert.That(detached.Verify(publicKey, out var errorResponse, null, payload),        Is.True, errorResponse);

            // ...and the wrong payload does not verify.
            Assert.That(detached.Verify(publicKey, out _, null, "Something else.".ToUTF8Bytes()),  Is.False);

            // Detaching an existing message keeps its signature valid, because
            // the signature never covered the message, only the Sig_structure.
            var manuallyDetached = attached.Detach();

            Assert.That(manuallyDetached.Signature,                                           Is.EqualTo(attached.Signature));
            Assert.That(manuallyDetached.Verify(publicKey, out var detachedError, null, payload),  Is.True, detachedError);

            // Attaching it again restores exactly the original message.
            Assert.That(Convert.ToHexString(manuallyDetached.Attach(payload).ToByteArray()),
                        Is.EqualTo(Convert.ToHexString(attached.ToByteArray())));

            // Supplying a detached payload for a message that carries one is
            // rejected, as there would be no telling which one was verified.
            Assert.That(attached.Verify(publicKey, out var bothPayloads, null, payload),      Is.False);
            Assert.That(bothPayloads,  Is.EqualTo("This COSE_Sign1 message carries its payload, therefore no detached payload must be supplied!"));

        }

        #endregion

        #region An_empty_protected_bucket_is_a_zero_length_byte_string()

        [Test]
        public void An_empty_protected_bucket_is_a_zero_length_byte_string()
        {

            // An empty protected bucket is h'' and NOT the encoded empty map
            // h'A0' - the one encoding that must never be "repaired", as the
            // signature covers these exact bytes.
            Assert.That(COSEHeaders.Empty.ToProtectedByteArray(),         Is.Empty);
            Assert.That(COSEHeaders.Empty.ToCBOR().ToDiagnosticString(),  Is.EqualTo("{}"));

            var sign1 = new COSESign1(
                            [],
                            COSEHeaders.Create(COSEAlgorithm.ES256, keyIdentifier),
                            payload,
                            new Byte[64]
                        );

            Assert.That(sign1.ProtectedHeader.IsEmpty,  Is.True);

            Assert.That(CBORValue.Parse(sign1.ToBeSigned()).ToDiagnosticString(),
                        Is.EqualTo($"[\"Signature1\", h'', h'', h'{Convert.ToHexStringLower(payload)}']"));

        }

        #endregion

        #region An_algorithm_that_is_not_integrity_protected_is_not_trusted_silently()

        [Test]
        public void An_algorithm_that_is_not_integrity_protected_is_not_trusted_silently()
        {

            var publicKey  = ExamplePublicKey();

            // Everything about this message is unprotected - an attacker on
            // the way could downgrade the algorithm at will.
            var sign1      = new COSESign1(
                                 [],
                                 COSEHeaders.Create(COSEAlgorithm.ES256, keyIdentifier),
                                 payload,
                                 new Byte[64]
                             );

            Assert.That(sign1.Algorithm,                                Is.EqualTo(COSEAlgorithm.ES256));
            Assert.That(sign1.ProtectedHeader.Algorithm,                Is.Null);

            Assert.That(sign1.Verify(publicKey, out var errorResponse), Is.False);
            Assert.That(errorResponse,  Does.Contain("within the unprotected header bucket only"));

            // Stating the expected algorithm explicitly gets past the policy,
            // and the message then fails on its signature rather than on its
            // headers - which proves the policy was the gate.
            Assert.That(sign1.Verify(publicKey, out var signatureError, null, null, COSEAlgorithm.ES256),  Is.False);
            Assert.That(signatureError,  Is.EqualTo("The signature is invalid!"));

            // A message that states no algorithm at all is refused as well.
            var withoutAlgorithm = new COSESign1([], COSEHeaders.Empty, payload, new Byte[64]);

            Assert.That(withoutAlgorithm.Verify(publicKey, out var withoutError),  Is.False);
            Assert.That(withoutError,  Does.Contain("does not state its signature algorithm"));

        }

        #endregion

        #region An_expected_algorithm_that_the_message_contradicts_is_rejected()

        [Test]
        public void An_expected_algorithm_that_the_message_contradicts_is_rejected()
        {

            var sign1 = COSESign1.Parse(Convert.FromHexString(appendixC21Message));

            Assert.That(sign1.Verify(ExamplePublicKey(), out var errorResponse, null, null, COSEAlgorithm.ES384),  Is.False);
            Assert.That(errorResponse,  Is.EqualTo("This COSE_Sign1 message was signed with the algorithm 'ES256', but the algorithm 'ES384' was expected!"));

        }

        #endregion

        #region A_critical_header_parameter_that_is_not_understood_is_rejected()

        [Test]
        public void A_critical_header_parameter_that_is_not_understood_is_rejected()
        {

            var (privateKey, publicKey) = GenerateKeyPair("secp256r1");

            // The sender demands that the certificate chain be processed,
            // which this implementation surfaces but does not validate.
            var withX5Chain = COSESign1.Sign(
                                  payload,
                                  privateKey,
                                  new COSEHeaders(
                                      (COSEHeaderLabel.Algorithm, COSEAlgorithm.ES256.ToCBOR()),
                                      (COSEHeaderLabel.Critical,  CBORValue.FromArray(COSEHeaderLabel.X5Chain)),
                                      (COSEHeaderLabel.X5Chain,   CBORValue.FromBytes([0x30, 0x82]))
                                  )
                              );

            Assert.That(withX5Chain.Verify(publicKey, out var criticalError),  Is.False);
            Assert.That(criticalError,  Is.EqualTo("The \"crit\" header parameter demands that 'x5chain' be understood, which this implementation does not!"));

            // A parameter that IS understood passes...
            var withKeyIdentifier = COSESign1.Sign(
                                        payload,
                                        privateKey,
                                        new COSEHeaders(
                                            (COSEHeaderLabel.Algorithm,      COSEAlgorithm.ES256.ToCBOR()),
                                            (COSEHeaderLabel.Critical,       CBORValue.FromArray(COSEHeaderLabel.KeyIdentifier)),
                                            (COSEHeaderLabel.KeyIdentifier,  CBORValue.FromBytes(keyIdentifier))
                                        )
                                    );

            Assert.That(withKeyIdentifier.Verify(publicKey, out var errorResponse),  Is.True, errorResponse);

            // ...but only when it is actually present within the protected bucket.
            var withoutTheParameter = COSESign1.Sign(
                                          payload,
                                          privateKey,
                                          new COSEHeaders(
                                              (COSEHeaderLabel.Algorithm, COSEAlgorithm.ES256.ToCBOR()),
                                              (COSEHeaderLabel.Critical,  CBORValue.FromArray(COSEHeaderLabel.KeyIdentifier))
                                          )
                                      );

            Assert.That(withoutTheParameter.Verify(publicKey, out var missingError),  Is.False);
            Assert.That(missingError,  Is.EqualTo("The \"crit\" header parameter lists 'kid', which is not present within the protected header bucket!"));

        }

        #endregion

        #region A_critical_header_parameter_within_the_unprotected_bucket_is_rejected()

        [Test]
        public void A_critical_header_parameter_within_the_unprotected_bucket_is_rejected()
        {

            var sign1 = new COSESign1(
                            COSEHeaders.Create(COSEAlgorithm.ES256).ToProtectedByteArray(),
                            new COSEHeaders(
                                (COSEHeaderLabel.Critical, CBORValue.FromArray(COSEHeaderLabel.Algorithm))
                            ),
                            payload,
                            new Byte[64]
                        );

            Assert.That(sign1.Verify(ExamplePublicKey(), out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Is.EqualTo("The \"crit\" header parameter must be placed within the protected header bucket!"));

        }

        #endregion

        #region A_DER_encoded_signature_is_reported_as_such()

        [Test]
        public void A_DER_encoded_signature_is_reported_as_such()
        {

            // The classic interoperability trap: Bouncy Castle and the .NET
            // signers produce DER by default, COSE wants r and s concatenated.
            var sign1 = new COSESign1(
                            COSEHeaders.Create(COSEAlgorithm.ES256).ToProtectedByteArray(),
                            COSEHeaders.Empty,
                            payload,
                            Convert.FromHexString("3046022100AA022100BB")
                        );

            Assert.That(sign1.Verify(ExamplePublicKey(), out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Does.Contain("must be 64 bytes long"));
            Assert.That(errorResponse,  Does.Contain("DER"));

        }

        #endregion


        #region The_algorithm_may_come_from_the_application_context()

        [Test]
        public void The_algorithm_may_come_from_the_application_context()
        {

            var (privateKey, publicKey)  = GenerateKeyPair("secp256r1");

            var signingKey  = COSEKey.From(privateKey, null, COSEAlgorithm.ES256).
                                      WithThumbprintKeyIdentifier();

            var signed      = COSESign1.SignWithApplicationAlgorithm(
                                  payload,
                                  privateKey,
                                  COSEAlgorithm.ES256,
                                  signingKey.KeyIdentifier
                              );

            // Nothing but the sender travels within the message: the protected
            // bucket is the zero-length byte string, and the algorithm appears
            // nowhere at all - not even unprotected, where it could be changed.
            Assert.That(signed.ProtectedHeaderBytes,       Is.Empty);
            Assert.That(signed.Algorithm,                  Is.Null);
            Assert.That(signed.KeyIdentifier!.Length,      Is.EqualTo(8));

            var received    = COSESign1.Parse(signed.ToByteArray());

            // Whoever does not name an algorithm gets no verification...
            Assert.That(received.Verify(publicKey, out var withoutAlgorithm),  Is.False);
            Assert.That(withoutAlgorithm,  Does.Contain("does not state its signature algorithm"));

            // ...naming it explicitly works...
            Assert.That(received.Verify(publicKey, out var explicitError, null, null, COSEAlgorithm.ES256),  Is.True, explicitError);

            // ...and so does verifying with the COSE key the identifier
            // resolves to, which carries the algorithm along with the key.
            Assert.That(received.KeyIdentifier,  Is.EqualTo(signingKey.KeyIdentifier));

            Assert.That(received.Verify(signingKey.ToPublicCOSEKey(), out var keyError),  Is.True, keyError);

            // A key without an algorithm can not stand in for the context.
            Assert.That(received.Verify(COSEKey.From(publicKey), out var withoutKeyAlgorithm),  Is.False);
            Assert.That(withoutKeyAlgorithm,  Does.Contain("does not state its signature algorithm"));

        }

        #endregion

        #region The_leanest_signed_message_is_measured()

        [Test]
        public void The_leanest_signed_message_is_measured()
        {

            var (privateKey, _)  = GenerateKeyPair("secp256r1");

            var lean     = COSESign1.SignWithApplicationAlgorithm(payload, privateKey, COSEAlgorithm.ES256, new Byte[8]).
                                     ToByteArray();

            var withAlg  = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256, new Byte[8]).
                                     ToByteArray();

            //  1  D2                    tag 18
            //  1  84                    array(4)
            //  1  40                    protected = h''
            // 11  A1 04 48 <8 bytes>    {4: kid}
            //  1  54                    payload header, 20 bytes of payload
            //  2  58 40                 signature header, 64 bytes of signature
            Assert.That(lean.Length,     Is.EqualTo(17 + payload.Length + 64));
            Assert.That(lean.Length,     Is.EqualTo(101));

            // Naming the algorithm within the protected bucket costs three
            // bytes more: 43 A1 01 26 instead of 40.
            Assert.That(withAlg.Length,  Is.EqualTo(lean.Length + 3));

            // ...and dropping the CBOR tag, where the context already says
            // what this is, saves one more.
            var untagged = COSESign1.SignWithApplicationAlgorithm(payload, privateKey, COSEAlgorithm.ES256, new Byte[8], Tagged: false).
                                     ToByteArray();

            Assert.That(untagged.Length,  Is.EqualTo(lean.Length - 1));

        }

        #endregion

        #region An_application_algorithm_that_contradicts_the_headers_is_rejected()

        [Test]
        public void An_application_algorithm_that_contradicts_the_headers_is_rejected()
        {

            var (privateKey, _) = GenerateKeyPair("secp256r1");

            Assert.That(() => COSESign1.Sign(payload,
                                             privateKey,
                                             COSEHeaders.Create(COSEAlgorithm.ES256),
                                             null,
                                             null,
                                             false,
                                             true,
                                             null,
                                             COSEAlgorithm.ES384),
                        Throws.TypeOf<COSEException>());

            Assert.That(() => COSESign1.Sign(payload,
                                             privateKey,
                                             COSEHeaders.Empty),
                        Throws.TypeOf<COSEException>());

        }

        #endregion

        #region Malformed_messages_are_rejected()

        [Test]
        public void Malformed_messages_are_rejected()
        {

            // A COSE_Mac0 (tag 17) is not a COSE_Sign1...
            Assert.That(COSESign1.TryParse(Convert.FromHexString("D18443A10126A0F640"), out _, out var wrongTag),  Is.False);
            Assert.That(wrongTag,     Does.Contain("must be tagged with CBOR tag 18"));

            // ...neither is a map...
            Assert.That(COSESign1.TryParse(Convert.FromHexString("A0"), out _, out var notAnArray),                Is.False);
            Assert.That(notAnArray,   Does.Contain("must be a CBOR array"));

            // ...nor an array of three elements...
            Assert.That(COSESign1.TryParse(Convert.FromHexString("8343A10126A0F6"), out _, out var tooShort),      Is.False);
            Assert.That(tooShort,     Does.Contain("4 elements"));

            // ...and the protected bucket must be a byte string...
            Assert.That(COSESign1.TryParse(Convert.FromHexString("84A10126A0F640"), out _, out var notAByteString),  Is.False);
            Assert.That(notAByteString,  Does.Contain("protected header bucket"));

            // ...holding a CBOR map.
            Assert.That(COSESign1.TryParse(Convert.FromHexString("844101A0F640"), out _, out var notAMap),         Is.False);
            Assert.That(notAMap,      Does.Contain("protected header bucket is invalid"));

            // The payload is a byte string or null, but never a text string.
            Assert.That(COSESign1.TryParse(Convert.FromHexString("8443A10126A0616140"), out _, out var textPayload),  Is.False);
            Assert.That(textPayload,  Does.Contain("payload"));

        }

        #endregion

        #region Duplicate_header_parameters_are_rejected()

        [Test]
        public void Duplicate_header_parameters_are_rejected()
        {

            Assert.That(() => new COSEHeaders(
                                  (COSEHeaderLabel.Algorithm, COSEAlgorithm.ES256.ToCBOR()),
                                  (COSEHeaderLabel.Algorithm, COSEAlgorithm.ES384.ToCBOR())
                              ),
                        Throws.TypeOf<COSEException>());

        }

        #endregion

    }

}
