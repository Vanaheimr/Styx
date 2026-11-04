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

using NUnit.Framework;

using Org.BouncyCastle.Crypto;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The countersignature of RFC 8152, header parameter 7, which RFC 9338
    /// deprecated and RFC 9052, Section 3.1 still says "must be understood by
    /// new implementations, to remain compatible with senders that adhere to"
    /// RFC 8152.
    ///
    /// The golden vectors are the COSE working group's own,
    /// countersign/signed1-01.json and signed1-02.json of
    /// https://github.com/cose-wg/Examples: a COSE_Sign1 signed with EdDSA on
    /// Ed25519, countersigned once with EdDSA and then also with ES256 on
    /// P-256. The working group publishes the structure each countersigner
    /// signed as well, so the Sig_structure is checked byte for byte, and the
    /// EdDSA countersignature, being deterministic, verifies only if it was
    /// computed over exactly those bytes.
    /// </summary>
    [TestFixture]
    public class COSERFC8152CountersignatureTests
    {

        #region Data

        private static readonly Byte[]  payload        = "This is the content.".ToUTF8Bytes();
        private static readonly Byte[]  keyIdentifier  = "11".ToUTF8Bytes();

        // The keys of both examples: an Ed25519 key and the P-256 key of
        // RFC 9052, Appendix C.7, both with the key identifier "11".
        private const String  ed25519X  = "D75A980182B10AB7D54BFED3C964073A0EE172F3DAA62325AF021A68F707511A";
        private const String  p256X     = "usWxHK2PmfnHKwXPS54m0kTcGJ90UiglWiGahtagnv8";
        private const String  p256Y     = "IBOL-C3BttVivg-lSreASjpkttcsz-1rb7btKLv8EX4";

        // countersign/signed1-01.json: one countersignature, EdDSA.
        private const String  signed1_01                  = "D28445A201270300A2078343A10127A10442313158406DAED158AFE4032E8DD477D3D2B7F667E7957AA8302BB5E568B4DCBCCE3CF0ED5A90F831351C85D6155A42A17CA1F25F501CC13F67108AE53BDA92DB88272E000442313154546869732069732074686520636F6E74656E742E58407142FD2FF96D56DB85BEE905A76BA1D0B7321A95C8C4D3607C5781932B7AFB8711497DFA751BF40B58B3BCC32300B1487F3DB34085EEF013BF08F4A44D6FEF0D";

        // countersign/signed1-02.json: two of them, EdDSA and ES256.
        private const String  signed1_02                  = "D28445A201270300A207828343A10127A10442313158406DAED158AFE4032E8DD477D3D2B7F667E7957AA8302BB5E568B4DCBCCE3CF0ED5A90F831351C85D6155A42A17CA1F25F501CC13F67108AE53BDA92DB88272E008343A10126A104423131584093487D09256A3EF4963719BA5CF101ACE2FC13D6314B49582171FFA4A1314DC93E4A4ADFA42A79E31B35D7304358585B41799678CE00CA47C3E023863923F8C80442313154546869732069732074686520636F6E74656E742E58407142FD2FF96D56DB85BEE905A76BA1D0B7321A95C8C4D3607C5781932B7AFB8711497DFA751BF40B58B3BCC32300B1487F3DB34085EEF013BF08F4A44D6FEF0D";

        // The "intermediates" of both examples: what each countersigner signed.
        private const String  toBeSignedEdDSA             = "8570436F756E7465725369676E617475726545A20127030043A101274054546869732069732074686520636F6E74656E742E";
        private const String  toBeSignedES256             = "8570436F756E7465725369676E617475726545A20127030043A101264054546869732069732074686520636F6E74656E742E";

        #endregion

        #region (private static) Ed25519Key() / P256Key()

        private static AsymmetricKeyParameter Ed25519Key()

            => new COSEKey(COSEKeyType.OKP,
                           COSECurve.Ed25519,
                           Convert.FromHexString(ed25519X)).ToPublicKey();

        private static AsymmetricKeyParameter P256Key()

            => new COSEKey(COSEKeyType.EC2,
                           COSECurve.P256,
                           FromBase64URL(p256X),
                           FromBase64URL(p256Y)).ToPublicKey();

        private static Byte[] FromBase64URL(String Text)

            => Convert.FromBase64String(
                   Text.Replace('-', '+').
                        Replace('_', '/').
                        PadRight(Text.Length + (4 - Text.Length % 4) % 4, '=')
               );

        #endregion


        #region The_published_countersignature_verifies()

        [Test]
        public void The_published_countersignature_verifies()
        {

            var message = COSESign1.Parse(Convert.FromHexString(signed1_01));

            // A single countersignature is written bare.
            Assert.That(message.RFC8152Countersignatures.Count,               Is.EqualTo(1));

            var countersignature = message.RFC8152Countersignatures[0];

            Assert.That(countersignature.Algorithm,                           Is.EqualTo(COSEAlgorithm.EdDSA));
            Assert.That(countersignature.KeyIdentifier,                       Is.EqualTo(keyIdentifier));

            // What the working group says the countersigner signed.
            Assert.That(COSESign1.ToBeRFC8152Countersigned(message.ProtectedHeaderBytes,
                                                           countersignature.ProtectedHeaderBytes,
                                                           payload),          Is.EqualTo(Convert.FromHexString(toBeSignedEdDSA)));

            Assert.That(message.VerifyRFC8152Countersignature(countersignature, Ed25519Key(), out var errorResponse),
                        Is.True, errorResponse);

            // And the body signature, which is a separate question.
            Assert.That(message.Verify(Ed25519Key(), out errorResponse),
                        Is.True, errorResponse);

        }

        #endregion

        #region Both_published_countersignatures_verify()

        [Test]
        public void Both_published_countersignatures_verify()
        {

            var message = COSESign1.Parse(Convert.FromHexString(signed1_02));

            // Several are written as an array of them.
            Assert.That(message.RFC8152Countersignatures.Count,               Is.EqualTo(2));

            var eddsa = message.RFC8152Countersignatures[0];
            var es256 = message.RFC8152Countersignatures[1];

            Assert.That(eddsa.Algorithm,                                      Is.EqualTo(COSEAlgorithm.EdDSA));
            Assert.That(es256.Algorithm,                                      Is.EqualTo(COSEAlgorithm.ES256));

            Assert.That(COSESign1.ToBeRFC8152Countersigned(message.ProtectedHeaderBytes,
                                                           es256.ProtectedHeaderBytes,
                                                           payload),          Is.EqualTo(Convert.FromHexString(toBeSignedES256)));

            Assert.That(message.VerifyRFC8152Countersignature(eddsa, Ed25519Key(), out var errorResponse),
                        Is.True, errorResponse);

            Assert.That(message.VerifyRFC8152Countersignature(es256, P256Key(),    out errorResponse),
                        Is.True, errorResponse);

            // Each with its own key only.
            Assert.That(message.VerifyRFC8152Countersignature(es256, Ed25519Key(), out _),
                        Is.False);

        }

        #endregion

        #region They_are_not_countersignatures_of_version_2()

        [Test]
        public void They_are_not_countersignatures_of_version_2()
        {

            var message = COSESign1.Parse(Convert.FromHexString(signed1_01));

            // Label 7 is not label 11...
            Assert.That(message.Countersignatures,                            Is.Empty);

            // ...and the structure of version 2 is not the one that was signed.
            Assert.That(message.VerifyCountersignature(message.RFC8152Countersignatures[0], Ed25519Key(), out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,                                        Is.Not.Null);

        }

        #endregion

        #region It_does_not_cover_the_body_signature()

        [Test]
        public void It_does_not_cover_the_body_signature()
        {

            var published = COSESign1.Parse(Convert.FromHexString(signed1_01));

            // The body signature replaced by 64 zero bytes: the message no
            // longer verifies, and its RFC 8152 countersignature still does.
            // That is the flaw RFC 9338 replaced it for, and why a valid one
            // does not say that its signer saw the signature.
            var replaced  = new COSESign1(published.ProtectedHeaderBytes,
                                          published.UnprotectedHeader,
                                          published.Payload,
                                          new Byte[64]);

            Assert.That(replaced.Verify(Ed25519Key(), out _),                 Is.False);

            Assert.That(replaced.VerifyRFC8152Countersignature(replaced.RFC8152Countersignatures[0], Ed25519Key(), out var errorResponse),
                        Is.True, errorResponse);

        }

        #endregion

        #region It_does_cover_the_payload_and_the_protected_bucket()

        [Test]
        public void It_does_cover_the_payload_and_the_protected_bucket()
        {

            var published  = COSESign1.Parse(Convert.FromHexString(signed1_01));

            var otherPayload   = new COSESign1(published.ProtectedHeaderBytes,
                                               published.UnprotectedHeader,
                                               "This is other content.".ToUTF8Bytes(),
                                               published.Signature);

            Assert.That(otherPayload.VerifyRFC8152Countersignature(otherPayload.RFC8152Countersignatures[0], Ed25519Key(), out _),
                        Is.False);

            // The same algorithm, without the content type the published
            // protected bucket carries.
            var otherBucket    = new COSESign1(Convert.FromHexString("A10127"),
                                               published.UnprotectedHeader,
                                               published.Payload,
                                               published.Signature);

            Assert.That(otherBucket.VerifyRFC8152Countersignature(otherBucket.RFC8152Countersignatures[0], Ed25519Key(), out _),
                        Is.False);

        }

        #endregion

        #region A_counter_signature_that_is_not_one_is_refused()

        [Test]
        public void A_counter_signature_that_is_not_one_is_refused()
        {

            var published  = COSESign1.Parse(Convert.FromHexString(signed1_01));

            var notAnArray = new COSESign1(published.ProtectedHeaderBytes,
                                           new COSEHeaders((COSEHeaderLabel.CounterSignature, CBORValue.FromText("countersigned"))),
                                           published.Payload,
                                           published.Signature);

            Assert.That(() => notAnArray.RFC8152Countersignatures,
                        Throws.TypeOf<COSEException>().With.Message.Contains("must be a COSE_Signature or an array of them"));

            var notASignature = new COSESign1(published.ProtectedHeaderBytes,
                                              new COSEHeaders((COSEHeaderLabel.CounterSignature, CBORValue.FromArray(CBORValue.FromInt64(1)))),
                                              published.Payload,
                                              published.Signature);

            Assert.That(() => notASignature.RFC8152Countersignatures,
                        Throws.TypeOf<COSEException>().With.Message.Contains("The RFC 8152 countersignature of this COSE_Sign1 message is invalid"));

        }

        #endregion

    }

}
