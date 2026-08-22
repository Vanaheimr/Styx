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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// What signing does to a payload before it signs it.
    ///
    /// A COSE payload is an opaque byte string, so a signature covers one
    /// SPELLING of a record rather than the record. That distinction costs
    /// nothing until somebody receives a message, decodes it and encodes it
    /// again - which in e-mobility is the normal case rather than the
    /// exception: a meter reading passes a charging station, a backend and a
    /// roaming hub on its way to the customer, and every one of them parses
    /// and re-serializes it. Their encoders write the deterministic encoding
    /// of RFC 8949, Section 4.2.1. Where the signer wrote something else, the
    /// record arrives unaltered in meaning and broken in fact, and the
    /// failure looks exactly like tampering.
    ///
    /// COSESign1.Sign therefore rewrites a CBOR payload in that encoding
    /// before signing it, unless told not to. The two tests in the middle of
    /// this file are the pair that shows the default is doing the work: the
    /// same record, forwarded the same way, survives with it and breaks
    /// without it.
    /// </summary>
    [TestFixture]
    public class COSEPayloadCanonicalizationTests
    {

        #region (private static) ReadingInReadingOrder()

        /// <summary>
        /// One meter reading, written the way a human reads it: meter, time,
        /// energy. Deterministic encoding sorts map keys by their encoded
        /// bytes, which puts the shortest name first - time, meter, energy -
        /// so these bytes are well-formed CBOR and not the deterministic
        /// encoding of themselves.
        /// </summary>
        private static Byte[] ReadingInReadingOrder()

            => CBORValue.FromMap([
                   new (CBORValue.FromText("meter"),   CBORValue.FromText("1ISA0000000042")),
                   new (CBORValue.FromText("time"),    CBORValue.Tagged(CBORTag.DateTimeString, CBORValue.FromText("2026-08-15T08:14:00Z"))),
                   new (CBORValue.FromText("energy"),  new MetrologicalValue(1234.567m, UnitOfMeasure.WattHour, SIPrefix.Kilo).ToCBOR())
               ]).ToByteArray();

        #endregion

        #region (private static) Forward(Message)

        /// <summary>
        /// What a backend or a roaming hub does to a record it passes on:
        /// take the message apart, decode the payload, encode it again with
        /// its own encoder - which writes the deterministic encoding - and
        /// put the message back together. Not one value is touched.
        /// </summary>
        private static COSESign1 Forward(COSESign1 Message)

            => new (Message.ProtectedHeaderBytes,
                    Message.UnprotectedHeader,
                    CBORValue.Parse(Message.Payload!).ToByteArray(CBORWriterOptions.Canonical),
                    Message.Signature,
                    Message.IsTagged);

        #endregion

        #region (private static) GenerateKeyPair(BouncyCastleCurveName)

        private static (ECPrivateKeyParameters PrivateKey, ECPublicKeyParameters PublicKey) GenerateKeyPair(String BouncyCastleCurveName)
        {

            var keyPair = Crypto.GenerateKeys(ECNamedCurveTable.GetByName(BouncyCastleCurveName));

            return ((ECPrivateKeyParameters) keyPair.Private,
                    (ECPublicKeyParameters)  keyPair.Public);

        }

        #endregion


        #region A_record_in_reading_order_is_not_canonical()

        [Test]
        public void A_record_in_reading_order_is_not_canonical()
        {

            var reading    = ReadingInReadingOrder();

            Assert.That(COSEPayload.IsCanonical(reading),  Is.False);

            var canonical  = COSEPayload.Canonicalize(reading);

            // Sorting a map moves bytes around without adding or removing
            // any: same length, same entries, same values. That is precisely
            // what makes the breakage below impossible to see by eye.
            Assert.That(canonical.Length,                          Is.EqualTo(reading.Length));
            Assert.That(COSEPayload.IsCanonical(canonical),        Is.True);
            Assert.That(CBORValue.Parse(canonical)["energy"],      Is.EqualTo(CBORValue.Parse(reading)["energy"]));
            Assert.That(CBORValue.Parse(canonical).AsMap().Count,  Is.EqualTo(3));

        }

        #endregion

        #region A_payload_that_is_not_CBOR_is_signed_as_it_is()

        [Test]
        public void A_payload_that_is_not_CBOR_is_signed_as_it_is()
        {

            // The payload of every published COSE example there is - and not
            // CBOR at all. There is nothing here to canonicalize, and
            // refusing to sign it would make the default useless for the
            // format's own examples.
            var text = "This is the content.".ToUTF8Bytes();

            Assert.That(COSEPayload.Canonicalize(text),  Is.EqualTo(text));
            Assert.That(COSEPayload.IsCanonical(text),   Is.False);

            var (privateKey, publicKey)  = GenerateKeyPair("secp256r1");

            var signed = COSESign1.Sign(text, privateKey, COSEAlgorithm.ES256);

            Assert.That(signed.Payload,            Is.EqualTo(text));
            Assert.That(signed.Verify(publicKey),  Is.True);

        }

        #endregion

        #region Signing_canonicalizes_by_default_so_forwarding_survives()

        [Test]
        public void Signing_canonicalizes_by_default_so_forwarding_survives()
        {

            var (privateKey, publicKey)  = GenerateKeyPair("secp256r1");

            var reading  = ReadingInReadingOrder();
            var signed   = COSESign1.Sign(reading, privateKey, COSEAlgorithm.ES256);

            // What was signed is not what was handed in ...
            Assert.That(signed.Payload,                             Is.Not.EqualTo(reading));
            Assert.That(COSEPayload.IsCanonical(signed.Payload!),   Is.True);

            // ... and that is what lets everybody who forwards the record
            // decode and encode it again without destroying it.
            Assert.That(Forward(signed).Verify(publicKey, out var errorResponse),  Is.True);
            Assert.That(errorResponse,  Is.Null);

        }

        #endregion

        #region Opting_out_signs_the_bytes_as_they_are_and_forwarding_breaks_them()

        [Test]
        public void Opting_out_signs_the_bytes_as_they_are_and_forwarding_breaks_them()
        {

            var (privateKey, publicKey)  = GenerateKeyPair("secp256r1");

            var reading  = ReadingInReadingOrder();
            var signed   = COSESign1.Sign(reading,
                                          privateKey,
                                          COSEAlgorithm.ES256,
                                          CanonicalizePayload: false);

            Assert.That(signed.Payload,            Is.EqualTo(reading));
            Assert.That(signed.Verify(publicKey),  Is.True);

            // Nobody tampered with anything: the forwarder re-encoded the
            // very same three entries, and the signature is gone. This is the
            // failure the default exists to prevent, and the reason it is a
            // default rather than an option: the party who loses is never the
            // party who chose.
            Assert.That(Forward(signed).Verify(publicKey, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Is.EqualTo("The signature is invalid!"));

        }

        #endregion

        #region A_detached_payload_that_canonicalization_would_change_is_refused()

        [Test]
        public void A_detached_payload_that_canonicalization_would_change_is_refused()
        {

            var (privateKey, publicKey)  = GenerateKeyPair("secp256r1");

            var reading  = ReadingInReadingOrder();

            // A detached payload does not travel within the message, so the
            // verifier is handed the caller's own bytes. Quietly signing a
            // different spelling of them would produce a message that can
            // never verify, whoever holds it - so this is the one place where
            // canonicalizing is refused rather than performed.
            Assert.That(() => COSESign1.Sign(reading,
                                             privateKey,
                                             COSEAlgorithm.ES256,
                                             DetachPayload: true),
                        Throws.TypeOf<COSEException>());

            // Doing what the exception asks for leaves nothing to change.
            var canonical  = COSEPayload.Canonicalize(reading);
            var detached   = COSESign1.Sign(canonical,
                                            privateKey,
                                            COSEAlgorithm.ES256,
                                            DetachPayload: true);

            Assert.That(detached.Payload,  Is.Null);
            Assert.That(detached.Verify(publicKey, DetachedPayload: canonical),  Is.True);

        }

        #endregion

        #region A_metrological_value_is_canonical_by_construction()

        [Test]
        public void A_metrological_value_is_canonical_by_construction()
        {

            // Not luck: Section 6 of the tag specification makes the encoding
            // a function of the value, so ToByteArray writes the
            // deterministic encoding and TryParse reads the strict profile.
            // A reading signed on its own therefore never needs the
            // canonicalization above - only the documents it travels in do.
            var bytes = new MetrologicalValue(1234.567m,
                                              UnitOfMeasure.WattHour,
                                              SIPrefix.Kilo).ToByteArray();

            Assert.That(COSEPayload.IsCanonical(bytes),  Is.True);

            Assert.That(MetrologicalValue.TryParse(bytes, out var readBack, out var errorResponse),  Is.True);
            Assert.That(errorResponse,            Is.Null);
            Assert.That(readBack.ToByteArray(),   Is.EqualTo(bytes));

        }

        #endregion

    }

}
