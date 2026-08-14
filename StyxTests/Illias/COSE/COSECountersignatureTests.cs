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
    /// Countersignatures [RFC 9338]: a signature OF A SIGNATURE.
    ///
    /// The golden vector is the worked example of RFC 9338, Appendix A.2.1 -
    /// a COSE_Sign1 signed with ES256 on P-256 and countersigned with ES512
    /// on P-521. The RFC prints it in diagnostic notation only, so the
    /// message is assembled here from the documented parts; that both its
    /// body signature and its countersignature then verify against the
    /// published example keys is what proves the assembly, the transcription
    /// and the Countersign_structure all at once.
    ///
    /// The point of RFC 9338 is its version 2 structure. The countersignature
    /// of RFC 8152 covered the payload but not the signature it was supposed
    /// to countersign, so it never actually attested to having seen it. The
    /// version 2 structure adds that signature as its last element, and one
    /// of the tests below shows that the published countersignature does NOT
    /// verify against the old structure.
    /// </summary>
    [TestFixture]
    public class COSECountersignatureTests
    {

        #region Data

        private static readonly Byte[]  payload             = "This is the content.".ToUTF8Bytes();
        private static readonly Byte[]  keyIdentifier11     = "11".ToUTF8Bytes();
        private static readonly Byte[]  keyIdentifierBilbo  = "bilbo.baggins@hobbiton.example".ToUTF8Bytes();

        // The ECDSA example keys of RFC 9052, Appendix C.7.
        private const String  key11X       = "usWxHK2PmfnHKwXPS54m0kTcGJ90UiglWiGahtagnv8";
        private const String  key11Y       = "IBOL-C3BttVivg-lSreASjpkttcsz-1rb7btKLv8EX4";
        private const String  key11D       = "V8kgd2ZBRuh2dgyVINBUqpPDr7BOMGcF22CQMIUHtNM";

        private const String  keyBilboX    = "AHKZLLOsCOzz5cY97ewNUajB957y-C-U88c3v13nmGZx6sYl_oJXu9A5RkTKqjqvjyekWF-7ytDyRXYgCF5cj0Kt";
        private const String  keyBilboY    = "AdymlHvOiLxXkEhayXQnNCvDX4h9htZaCJN34kfmC6pV5OhQHiraVySsUdaQkAgDPrwQrJmbnX9cwlGfP-HqHZR1";
        private const String  keyBilboD    = "AAhRON2r9cqXX1hg-RoI6R1tX5p2rUAYdmpHZoC1XNM56KtscrX6zbKipQrCW9CGZH3T4ubpnoTKLDYJ_fF3_rJt";

        // RFC 9338, Appendix A.2.1.
        private const String  bodyProtected              = "A201260300";
        private const String  countersignatureProtected  = "A1013823";
        private const String  bodySignature              = "BB587D6B15F47BFD54D2CBFCECEF75451E92B08A514BD439FA3AA65C6AC92DF0D7328C4A47529B32ADD3DD1B4E940071C021E9A8F2641F1D8E3B053DDD65AE52";
        private const String  countersignatureSignature  = "01B1291B0E60A79C459A4A9184A0D393E034B34AF069A1CCA34F5A913AFFFF698002295FA9F8FCBFB6FDFF59132FC0C406E98754A98F1FBFE81C03095F481856BC470170227206FA5BEE3C0431C56A66824E7AAF692985952E31271434B2BA2E47A335C658B5E995AEB5D63CF2D0CED367D3E4CC8FFFD53B70D115BAA9E86961FBD1A5CF";

        #endregion

        #region (private static) FromBase64URL(Text)

        private static Byte[] FromBase64URL(String Text)

            => Convert.FromBase64String(
                   Text.Replace('-', '+').
                        Replace('_', '/').
                        PadRight(Text.Length + (4 - Text.Length % 4) % 4, '=')
               );

        #endregion

        #region (private static) PublicKey11() / PublicKeyBilbo()

        private static ECPublicKeyParameters PublicKey11()

            => new COSEKey(COSEKeyType.EC2,
                           COSECurve.P256,
                           FromBase64URL(key11X),
                           FromBase64URL(key11Y),
                           FromBase64URL(key11D)).ToPublicKey();

        private static ECPublicKeyParameters PublicKeyBilbo()

            => new COSEKey(COSEKeyType.EC2,
                           COSECurve.P521,
                           FromBase64URL(keyBilboX),
                           FromBase64URL(keyBilboY),
                           FromBase64URL(keyBilboD)).ToPublicKey();

        #endregion

        #region (private static) PublishedExample()

        /// <summary>
        /// The countersigned COSE_Sign1 message of RFC 9338, Appendix A.2.1,
        /// assembled from the parts the appendix prints.
        /// </summary>
        private static COSESign1 PublishedExample()
        {

            var countersignature = new COSESignature(
                                       Convert.FromHexString(countersignatureProtected),
                                       new COSEHeaders(
                                           (COSEHeaderLabel.KeyIdentifier, CBORValue.FromBytes(keyIdentifierBilbo))
                                       ),
                                       Convert.FromHexString(countersignatureSignature)
                                   );

            return new COSESign1(
                       Convert.FromHexString(bodyProtected),
                       new COSEHeaders(
                           (COSEHeaderLabel.KeyIdentifier,      CBORValue.FromBytes(keyIdentifier11)),
                           (COSEHeaderLabel.CounterSignatureV2, countersignature.ToCBOR())
                       ),
                       payload,
                       Convert.FromHexString(bodySignature)
                   );

        }

        #endregion

        #region (private static) GenerateKeyPair(BouncyCastleCurveName)

        private static (ECPrivateKeyParameters PrivateKey, ECPublicKeyParameters PublicKey) GenerateKeyPair(String BouncyCastleCurveName)
        {

            var keyPair = Crypto.GenerateKeys(ECNamedCurveTable.GetByName(BouncyCastleCurveName));

            return ((ECPrivateKeyParameters) keyPair.Private,
                    (ECPublicKeyParameters)  keyPair.Public);

        }

        #endregion


        #region The_published_countersignature_verifies()

        [Test]
        public void The_published_countersignature_verifies()
        {

            var message = PublishedExample();

            // The body of the published example is an ordinary COSE_Sign1
            // whose protected bucket carries two parameters...
            Assert.That(message.ProtectedHeader.Algorithm,    Is.EqualTo(COSEAlgorithm.ES256));
            Assert.That(message.ProtectedHeader.ContentType,  Is.EqualTo(CBORValue.FromInt64(0)));
            Assert.That(message.KeyIdentifier,                Is.EqualTo(keyIdentifier11));

            Assert.That(message.Verify(PublicKey11(), out var bodyError),  Is.True, bodyError);

            // ...and it carries exactly one countersignature, in the
            // unprotected bucket, made with the other published key.
            Assert.That(message.Countersignatures.Count,                      Is.EqualTo(1));
            Assert.That(message.Countersignatures[0].Algorithm,               Is.EqualTo(COSEAlgorithm.ES512));
            Assert.That(message.Countersignatures[0].KeyIdentifier,           Is.EqualTo(keyIdentifierBilbo));
            Assert.That(message.Countersignatures[0].Signature.Length,        Is.EqualTo(132));

            Assert.That(message.VerifyCountersignature(message.Countersignatures[0],
                                                       PublicKeyBilbo(),
                                                       out var counterError),  Is.True, counterError);

        }

        #endregion

        #region The_published_countersignature_does_not_verify_against_the_deprecated_structure()

        [Test]
        public void The_published_countersignature_does_not_verify_against_the_deprecated_structure()
        {

            var message = PublishedExample();

            // The countersignature of RFC 8152: five elements, context
            // "CounterSignature", and no trace of the signature it is
            // supposed to countersign.
            var writer  = new CBORWriter();

            writer.WriteStartArray(5);
            writer.WriteTextString("CounterSignature");
            writer.WriteByteString(Convert.FromHexString(bodyProtected));
            writer.WriteByteString(Convert.FromHexString(countersignatureProtected));
            writer.WriteByteString([]);
            writer.WriteByteString(payload);
            writer.WriteEndArray();

            Assert.That(COSEAlgorithm.ES512.Verify(writer.ToByteArray(),
                                                   message.Countersignatures[0].Signature,
                                                   PublicKeyBilbo(),
                                                   out var errorResponse),  Is.False);

            Assert.That(errorResponse,  Is.EqualTo("The signature is invalid!"));

            // The version 2 structure has six elements, the last of which is
            // an array holding the countersigned signature.
            var structure = CBORValue.Parse(message.ToBeCountersigned(message.Countersignatures[0]));

            Assert.That(structure.Count,                Is.EqualTo(6));
            Assert.That(structure[0].AsText(),          Is.EqualTo("CounterSignatureV2"));
            Assert.That(structure[5].Count,             Is.EqualTo(1));
            Assert.That(structure[5][0].AsBytes(),      Is.EqualTo(message.Signature));

        }

        #endregion

        #region A_countersignature_breaks_when_the_signature_it_covers_changes()

        [Test]
        public void A_countersignature_breaks_when_the_signature_it_covers_changes()
        {

            var (privateKey,      publicKey)       = GenerateKeyPair("secp256r1");
            var (counterPrivate,  counterPublic)   = GenerateKeyPair("secp256r1");

            var signed          = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256);
            var countersigned   = signed.AddCountersignature(counterPrivate, COSEAlgorithm.ES256);

            Assert.That(countersigned.VerifyCountersignature(countersigned.Countersignatures[0],
                                                             counterPublic,
                                                             out var errorResponse),  Is.True, errorResponse);

            // Re-signing the very same payload with the very same key yields
            // a different signature, since ECDSA is randomized. That is
            // enough to invalidate the countersignature - which is exactly
            // what RFC 9338 set out to achieve and what the structure of
            // RFC 8152 could not do.
            var reSigned        = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256);

            Assert.That(reSigned.Signature,  Is.Not.EqualTo(signed.Signature));

            var swapped         = new COSESign1(
                                      countersigned.ProtectedHeaderBytes,
                                      countersigned.UnprotectedHeader,
                                      countersigned.Payload,
                                      reSigned.Signature
                                  );

            // The body still verifies, because the new signature is a valid
            // signature of the same payload...
            Assert.That(swapped.Verify(publicKey, out var bodyError),  Is.True, bodyError);

            // ...but the countersignature does not, because it attested to
            // the signature that was replaced.
            Assert.That(swapped.VerifyCountersignature(swapped.Countersignatures[0],
                                                       counterPublic,
                                                       out var counterError),  Is.False);

            Assert.That(counterError,  Is.EqualTo("The signature is invalid!"));

        }

        #endregion

        #region Countersigning_leaves_the_body_signature_untouched()

        [Test]
        public void Countersigning_leaves_the_body_signature_untouched()
        {

            var (privateKey,     publicKey)      = GenerateKeyPair("secp256r1");
            var (counterPrivate, counterPublic)  = GenerateKeyPair("secp384r1");

            var signed         = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256, keyIdentifier11);
            var countersigned  = signed.AddCountersignature(counterPrivate, COSEAlgorithm.ES384, keyIdentifierBilbo);

            // Countersignatures live within the unprotected bucket, which no
            // signature covers, so the body signature and everything it
            // covers stay exactly as they were.
            Assert.That(countersigned.Signature,             Is.EqualTo(signed.Signature));
            Assert.That(countersigned.ProtectedHeaderBytes,  Is.EqualTo(signed.ProtectedHeaderBytes));
            Assert.That(countersigned.Payload,               Is.EqualTo(signed.Payload));

            Assert.That(countersigned.Verify(publicKey, out var bodyError),  Is.True, bodyError);

            // ...and whoever knows nothing of countersignatures still reads
            // the message, because the parameter is one they may ignore.
            var reparsed = COSESign1.Parse(countersigned.ToByteArray());

            Assert.That(reparsed.Verify(publicKey, out var reparsedError),  Is.True, reparsedError);
            Assert.That(reparsed.Countersignatures.Count,                   Is.EqualTo(1));
            Assert.That(reparsed.Countersignatures[0].KeyIdentifier,        Is.EqualTo(keyIdentifierBilbo));

            Assert.That(reparsed.VerifyCountersignature(reparsed.Countersignatures[0],
                                                        counterPublic,
                                                        out var counterError),  Is.True, counterError);

        }

        #endregion

        #region Several_parties_countersign_the_same_signature()

        [Test]
        public void Several_parties_countersign_the_same_signature()
        {

            var (privateKey,  publicKey)   = GenerateKeyPair("secp256r1");
            var (firstKey,    firstPublic)  = GenerateKeyPair("secp256r1");
            var (secondKey,   secondPublic) = GenerateKeyPair("secp521r1");

            var signed  = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256);

            // A single countersignature is written bare...
            var once    = signed.AddCountersignature(firstKey, COSEAlgorithm.ES256);

            Assert.That(once.Countersignatures.Count,  Is.EqualTo(1));
            Assert.That(once.UnprotectedHeader.TryGet(COSEHeaderLabel.CounterSignatureV2, out var bare),  Is.True);
            Assert.That(bare[0].Kind,  Is.EqualTo(CBORValueKind.ByteString), "A single countersignature must not be wrapped within an array!");

            // ...and several as an array of them.
            var twice   = once.AddCountersignature(secondKey, COSEAlgorithm.ES512);

            Assert.That(twice.Countersignatures.Count,  Is.EqualTo(2));
            Assert.That(twice.UnprotectedHeader.TryGet(COSEHeaderLabel.CounterSignatureV2, out var array),  Is.True);
            Assert.That(array[0].Kind,  Is.EqualTo(CBORValueKind.Array));

            var reparsed = COSESign1.Parse(twice.ToByteArray());

            Assert.That(reparsed.Countersignatures.Count,                 Is.EqualTo(2));
            Assert.That(reparsed.Countersignatures[0].Algorithm,          Is.EqualTo(COSEAlgorithm.ES256));
            Assert.That(reparsed.Countersignatures[1].Algorithm,          Is.EqualTo(COSEAlgorithm.ES512));
            Assert.That(reparsed.Countersignatures[1].Signature.Length,   Is.EqualTo(132));

            // Every one of them verifies with its own key, the body with its own...
            Assert.That(reparsed.Verify(publicKey, out var bodyError),  Is.True, bodyError);

            Assert.That(reparsed.VerifyCountersignature(reparsed.Countersignatures[0], firstPublic,  out var firstError),   Is.True, firstError);
            Assert.That(reparsed.VerifyCountersignature(reparsed.Countersignatures[1], secondPublic, out var secondError),  Is.True, secondError);

            // ...and none of them with anyone else's.
            Assert.That(reparsed.VerifyCountersignature(reparsed.Countersignatures[0], publicKey, out _),  Is.False);

        }

        #endregion

        #region A_chain_of_meter_charging_station_and_operator()

        [Test]
        public void A_chain_of_meter_charging_station_and_operator()
        {

            var (meterKey,    meterPublic)    = GenerateKeyPair("brainpoolP256r1");
            var (stationKey,  stationPublic)  = GenerateKeyPair("secp256r1");
            var (operatorKey, operatorPublic) = GenerateKeyPair("secp384r1");

            // 1. The meter signs each reading together with its metadata.
            var readings = new List<COSESign1>();

            for (var i = 1; i <= 3; i++)
            {

                var reading = CBORValue.FromMap([
                                  new (CBORValue.FromText("meter"),     CBORValue.FromText("1ISA0000000042")),
                                  new (CBORValue.FromText("index"),     CBORValue.FromInt64(i)),
                                  new (CBORValue.FromText("energy"),    CBORValue.FromDecimal(12.345m * i)),
                                  new (CBORValue.FromText("timestamp"), CBORValue.FromInt64(1786000000 + i * 60))
                              ]);

                readings.Add(
                    COSESign1.Sign(reading.ToByteArray(),
                                   meterKey,
                                   COSEAlgorithm.ESB256,
                                   "meter-1ISA0000000042".ToUTF8Bytes())
                );

            }

            // 2. The charging station bundles the signed readings together
            //    with metadata of its own and signs THAT: a new statement
            //    about new data, so a payload of its own - not a
            //    countersignature.
            var bundle  = CBORValue.FromMap([
                              new (CBORValue.FromText("chargingStation"), CBORValue.FromText("DE*GEF*E12345678")),
                              new (CBORValue.FromText("session"),         CBORValue.FromText("a4f1c9")),
                              new (CBORValue.FromText("readings"),        CBORValue.FromArray(readings.Select(static reading => CBORValue.FromBytes(reading.ToByteArray()))))
                          ]);

            var signedBundle  = COSESign1.Sign(bundle.ToByteArray(),
                                               stationKey,
                                               COSEAlgorithm.ES256,
                                               "station-DE*GEF*E12345678".ToUTF8Bytes());

            // 3. The operator vouches for the charging station's signature
            //    before the bundle goes to the customer. It asserts nothing
            //    new about the data, so it countersigns rather than wrapping:
            //    the customer can still verify the station's signature on its
            //    own, and the station's bytes do not change.
            var released      = signedBundle.AddCountersignature(operatorKey,
                                                                 COSEAlgorithm.ES384,
                                                                 "cpo-DE*GEF".ToUTF8Bytes());

            Assert.That(released.Signature,  Is.EqualTo(signedBundle.Signature));

            // What the customer receives...
            var received      = COSESign1.Parse(released.ToByteArray());

            // ...the operator vouched for the station's signature...
            Assert.That(received.Countersignatures.Count,  Is.EqualTo(1));
            Assert.That(received.VerifyCountersignature(received.Countersignatures[0],
                                                        operatorPublic,
                                                        out var operatorError),  Is.True, operatorError);

            // ...the station signed the bundle...
            Assert.That(received.Verify(stationPublic, out var stationError),  Is.True, stationError);

            // ...and every reading within it still carries the signature of
            // the meter that measured it.
            var receivedBundle    = CBORValue.Parse(received.Payload!);
            var receivedReadings  = receivedBundle["readings"].AsArray();

            Assert.That(receivedReadings.Count,  Is.EqualTo(3));

            for (var i = 0; i < receivedReadings.Count; i++)
            {

                var reading = COSESign1.Parse(receivedReadings[i].AsBytes());

                Assert.That(reading.Algorithm,      Is.EqualTo(COSEAlgorithm.ESB256), $"reading {i}");
                Assert.That(reading.Verify(meterPublic, out var meterError),  Is.True, $"reading {i}: {meterError}");

                var values = CBORValue.Parse(reading.Payload!);

                Assert.That(values["index"].AsInt64(),  Is.EqualTo(i + 1));

            }

            // The operator can not silently swap a reading: doing so changes
            // the bundle, which breaks the station's signature.
            var tampered = (Byte[]) received.Payload!.Clone();
            tampered[^1] ^= 0x01;

            Assert.That(new COSESign1(received.ProtectedHeaderBytes,
                                      received.UnprotectedHeader,
                                      tampered,
                                      received.Signature).
                            Verify(stationPublic, out _),  Is.False);

        }

        #endregion

    }

}
