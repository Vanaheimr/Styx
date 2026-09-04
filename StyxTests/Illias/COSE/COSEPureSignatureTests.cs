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

using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The two PURE signature schemes: EdDSA [RFC 8032] and ML-DSA
    /// [FIPS 204, RFC 9964].
    ///
    /// What they have in common is the thing that makes them different from
    /// ECDSA here: they sign the Sig_structure ITSELF rather than a digest of
    /// it. Handing either of them a hash produces a signature that is valid
    /// for that hash and that no other implementation will ever accept - a
    /// failure with no symptom until two implementations meet.
    ///
    /// EdDSA is checked against the published vectors of RFC 8032, and checked
    /// harder than ECDSA can be: it has no nonce to draw, so the published
    /// signatures are not merely verifiable but reproducible.
    /// </summary>
    [TestFixture]
    public class COSEPureSignatureTests
    {

        #region Data

        private static readonly Byte[] content = "This is the content.".ToUTF8Bytes();

        // RFC 8032, Section 7.1 (Ed25519) and Section 7.4 (Ed448).
        private const String ed25519Secret     = "9D61B19DEFFD5A60BA844AF492EC2CC44449C5697B326919703BAC031CAE7F60";
        private const String ed25519Public     = "D75A980182B10AB7D54BFED3C964073A0EE172F3DAA62325AF021A68F707511A";
        private const String ed25519Signature  = "E5564300C360AC729086E2CC806E828A84877F1EB8E5D974D873E06522490155" +
                                                 "5FB8821590A33BACC61E39701CF9B46BD25BF5F0595BBE24655141438E7A100B";

        private const String ed25519Secret3    = "C5AA8DF43F9F837BEDB7442F31DCB7B166D38535076F094B85CE3A2E0B4458F7";
        private const String ed25519Message3   = "AF82";
        private const String ed25519Signature3 = "6291D657DEEC24024827E69C3ABE01A30CE548A284743A445E3680D7DB5AC3AC" +
                                                 "18FF9B538D16F290AE67F760984DC6594A7C15E9716ED28DC027BECEEA1EC40A";

        private const String ed448Secret       = "6C82A562CB808D10D632BE89C8513EBF6C929F34DDFA8C9F63C9960EF6E348A3" +
                                                 "528C8A3FCC2F044E39A3FC5B94492F8F032E7549A20098F95B";
        private const String ed448Public       = "5FD7449B59B461FD2CE787EC616AD46A1DA1342485A70E1F8A0EA75D80E96778" +
                                                 "EDF124769B46C7061BD6783DF1E50F6CD1FA1ABEAFE8256180";
        private const String ed448Signature    = "533A37F6BBE457251F023C0D88F976AE2DFB504A843E34D2074FD823D41A591F" +
                                                 "2B233F034F628281F2FD7A22DDD47D7828C59BD0A21BFD3980FF0D2028D4B18A" +
                                                 "9DF63E006C5D1C2D345B925D8DC00B4104852DB99AC5C7CDDA8530A113A0F4DB" +
                                                 "B61149F05A7363268C71D95808FF2E652600";

        /// <summary>An arbitrary ML-DSA seed. It is a test key and secures nothing.</summary>
        private const String mlDsaSeed         = "000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F";

        #endregion


        #region The_published_EdDSA_vectors_are_reproduced()

        [Test]
        public void The_published_EdDSA_vectors_are_reproduced()
        {

            var vectors = new (String Name, COSEAlgorithm Algorithm, String Secret, String Message, String Signature)[] {
                              ("Ed25519 TEST 1", COSEAlgorithm.Ed25519, ed25519Secret,  "",                ed25519Signature),
                              ("Ed25519 TEST 3", COSEAlgorithm.Ed25519, ed25519Secret3, ed25519Message3,   ed25519Signature3),
                              ("Ed448 Blank",    COSEAlgorithm.Ed448,   ed448Secret,    "",                ed448Signature)
                          };

            foreach (var vector in vectors)
            {

                var isEd448     = vector.Algorithm == COSEAlgorithm.Ed448;

                var privateKey  = isEd448
                                      ? new Ed448PrivateKeyParameters  (Convert.FromHexString(vector.Secret), 0)
                                      : (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)
                                        new Ed25519PrivateKeyParameters(Convert.FromHexString(vector.Secret), 0);

                // Deterministic without asking: RFC 8032 derives the nonce
                // from the private key and the message, and offers no other
                // option at all.
                var signature   = vector.Algorithm.Sign(
                                      Convert.FromHexString(vector.Message),
                                      privateKey
                                  );

                Assert.That(Convert.ToHexString(signature),  Is.EqualTo(vector.Signature),  vector.Name);

            }

        }

        #endregion

        #region The_published_EdDSA_public_keys_are_derived()

        [Test]
        public void The_published_EdDSA_public_keys_are_derived()
        {

            var ed25519 = COSEKey.From(new Ed25519PrivateKeyParameters(Convert.FromHexString(ed25519Secret), 0),
                                       Algorithm: COSEAlgorithm.Ed25519);

            Assert.That(ed25519.KeyType,                       Is.EqualTo(COSEKeyType.OKP));
            Assert.That(Convert.ToHexString(ed25519.X!),       Is.EqualTo(ed25519Public));
            Assert.That(ed25519.Y,                             Is.Null,  "An octet key pair has no y coordinate!");
            Assert.That(ed25519.D!.Length,                     Is.EqualTo(32));

            var ed448   = COSEKey.From(new Ed448PrivateKeyParameters(Convert.FromHexString(ed448Secret), 0),
                                       Algorithm: COSEAlgorithm.Ed448);

            // 57 bytes and not 56: RFC 8032 appends a sign bit to the 456-bit
            // encoding, which costs a whole further octet.
            Assert.That(Convert.ToHexString(ed448.X!),         Is.EqualTo(ed448Public));
            Assert.That(ed448.D!.Length,                       Is.EqualTo(57));

        }

        #endregion

        #region An_EdDSA_COSE_Sign1_signs_verifies_and_roundtrips()

        [Test]
        public void An_EdDSA_COSE_Sign1_signs_verifies_and_roundtrips()
        {

            var vectors = new (String Name, COSEAlgorithm Algorithm, String Secret, Int32 SignatureSize)[] {
                              ("Ed25519", COSEAlgorithm.Ed25519, ed25519Secret, 64),
                              ("Ed448",   COSEAlgorithm.Ed448,   ed448Secret,  114)
                          };

            foreach (var vector in vectors)
            {

                var key      = COSEKey.From(
                                   vector.Algorithm == COSEAlgorithm.Ed448
                                       ? new Ed448PrivateKeyParameters  (Convert.FromHexString(vector.Secret), 0)
                                       : (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)
                                         new Ed25519PrivateKeyParameters(Convert.FromHexString(vector.Secret), 0),
                                   Algorithm: vector.Algorithm
                               );

                var message  = COSESign1.Sign(content, key);

                Assert.That(message.Signature.Length,  Is.EqualTo(vector.SignatureSize),  vector.Name);
                Assert.That(message.Verify(key.ToPublicCOSEKey(), out var errorResponse),  Is.True,  errorResponse);

                var parsed   = COSESign1.Parse(message.ToByteArray());

                Assert.That(Convert.ToHexString(parsed.ToByteArray()),
                            Is.EqualTo(Convert.ToHexString(message.ToByteArray())),  vector.Name);

                Assert.That(parsed.Verify(key.ToPublicCOSEKey(), out var parsedError),  Is.True,  parsedError);

                // The signature is over the Sig_structure ITSELF, with no
                // digest in between.
                Assert.That(Convert.ToHexString(message.Signature),
                            Is.EqualTo(Convert.ToHexString(
                                vector.Algorithm.Sign(message.ToBeSigned(), key.ToPrivateKey()))),
                            vector.Name);

            }

        }

        #endregion

        #region An_ML_DSA_COSE_key_carries_the_seed_and_nothing_larger()

        [Test]
        public void An_ML_DSA_COSE_key_carries_the_seed_and_nothing_larger()
        {

            var vectors = new (COSEAlgorithm Algorithm, Int32 PublicKeySize, Int32 SignatureSize)[] {
                              (COSEAlgorithm.MLDsa44, 1312, 2420),
                              (COSEAlgorithm.MLDsa65, 1952, 3309),
                              (COSEAlgorithm.MLDsa87, 2592, 4627)
                          };

            foreach (var vector in vectors)
            {

                var key = COSEKey.From(
                              MLDsaPrivateKeyParameters.FromSeed(
                                  vector.Algorithm.MLDsaParameterSet!,
                                  Convert.FromHexString(mlDsaSeed)
                              )
                          );

                Assert.That(key.KeyType,        Is.EqualTo(COSEKeyType.AKP),        vector.Algorithm.Name);
                Assert.That(key.Algorithm,      Is.EqualTo(vector.Algorithm),       vector.Algorithm.Name);
                Assert.That(key.Pub!.Length,    Is.EqualTo(vector.PublicKeySize),   vector.Algorithm.Name);

                // The expanded ML-DSA-87 secret key is 4896 bytes; RFC 9964
                // puts the 32-byte seed on the wire instead.
                Assert.That(key.Priv!.Length,   Is.EqualTo(32),                     vector.Algorithm.Name);
                Assert.That(Convert.ToHexString(key.Priv),  Is.EqualTo(mlDsaSeed),  vector.Algorithm.Name);

                var message = COSESign1.Sign(content, key, Deterministic: true);

                Assert.That(message.Signature.Length,  Is.EqualTo(vector.SignatureSize),  vector.Algorithm.Name);
                Assert.That(message.Verify(key.ToPublicCOSEKey(), out var errorResponse),  Is.True,  errorResponse);

            }

        }

        #endregion

        #region An_algorithm_key_pair_reads_label_minus_one_as_its_public_key()

        [Test]
        public void An_algorithm_key_pair_reads_label_minus_one_as_its_public_key()
        {

            // The trap of RFC 9964: on an EC2 or OKP key, -1 is the curve and
            // -2 the x coordinate; on an AKP key they are the public and the
            // private key. A parser that switched on the label alone would
            // read a 1312-byte public key as a curve identifier and report
            // nothing wrong at all.
            var key     = COSEKey.From(
                              MLDsaPrivateKeyParameters.FromSeed(
                                  COSEAlgorithm.MLDsa44.MLDsaParameterSet!,
                                  Convert.FromHexString(mlDsaSeed)
                              )
                          );

            var parsed  = COSEKey.Parse(key.ToByteArray());

            Assert.That(parsed.KeyType,       Is.EqualTo(COSEKeyType.AKP));
            Assert.That(parsed.Pub!.Length,   Is.EqualTo(1312));
            Assert.That(parsed.Priv!.Length,  Is.EqualTo(32));
            Assert.That(parsed.Curve,         Is.Null,  "An algorithm key pair is on no curve!");
            Assert.That(parsed.X,             Is.Null);

            Assert.That(Convert.ToHexString(parsed.ToByteArray()),
                        Is.EqualTo(Convert.ToHexString(key.ToByteArray())));

            // ...while a key that really is on a curve still reads -1 as one.
            var onCurve = COSEKey.Parse(
                              COSEKey.From(new Ed25519PrivateKeyParameters(Convert.FromHexString(ed25519Secret), 0),
                                           Algorithm: COSEAlgorithm.Ed25519).ToByteArray()
                          );

            Assert.That(onCurve.Curve,  Is.EqualTo(COSECurve.Ed25519));
            Assert.That(onCurve.Pub,    Is.Null);

        }

        #endregion

        #region The_thumbprint_of_an_algorithm_key_pair_covers_its_algorithm()

        [Test]
        public void The_thumbprint_of_an_algorithm_key_pair_covers_its_algorithm()
        {

            var key    = COSEKey.From(
                             MLDsaPrivateKeyParameters.FromSeed(
                                 COSEAlgorithm.MLDsa44.MLDsaParameterSet!,
                                 Convert.FromHexString(mlDsaSeed)
                             )
                         );

            var input  = CBORValue.Parse(key.ThumbprintInput()).AsMap().ToArray();

            // kty, alg and pub - the algorithm included, unlike every other
            // key type, because an ML-DSA public key does not say which
            // parameter set produced it and two strengths must not be able to
            // share an identity [RFC 9964].
            Assert.That(input.Length,  Is.EqualTo(3));

            Assert.That(input[0].Key,               Is.EqualTo(COSEKey.KeyTypeLabel));
            Assert.That(input[0].Value.AsInt64(),   Is.EqualTo((Int64) COSEKeyType.AKP));

            Assert.That(input[1].Key,               Is.EqualTo(COSEKey.AlgorithmLabel));
            Assert.That(input[1].Value.AsInt64(),   Is.EqualTo(-48));

            Assert.That(input[2].Key,               Is.EqualTo(COSEKey.PubLabel));
            Assert.That(input[2].Value.AsBytes().Length,  Is.EqualTo(1312));

            // The private half stays out of it, so both halves of one key pair
            // keep the same identity.
            Assert.That(Convert.ToHexString(key.Thumbprint()),
                        Is.EqualTo(Convert.ToHexString(key.ToPublicCOSEKey().Thumbprint())));

        }

        #endregion

        #region ML_DSA_signs_deterministically_when_it_is_asked_to()

        [Test]
        public void ML_DSA_signs_deterministically_when_it_is_asked_to()
        {

            var key    = COSEKey.From(
                             MLDsaPrivateKeyParameters.FromSeed(
                                 COSEAlgorithm.MLDsa44.MLDsaParameterSet!,
                                 Convert.FromHexString(mlDsaSeed)
                             )
                         );

            // FIPS 204 defines a deterministic variant, in which the
            // per-signature randomness is 32 zero bytes rather than drawn, and
            // RFC 9964 does not choose between them. The choice is what
            // decides whether two implementations can be compared byte for
            // byte or only asked whether each accepts the other.
            var first   = COSESign1.Sign(content, key, Deterministic: true);
            var second  = COSESign1.Sign(content, key, Deterministic: true);

            Assert.That(Convert.ToHexString(second.ToByteArray()),
                        Is.EqualTo(Convert.ToHexString(first.ToByteArray())));

            // ...and the randomized variant, which is the default, does not
            // repeat itself while verifying just as well.
            var random  = COSESign1.Sign(content, key);

            Assert.That(Convert.ToHexString(random.Signature),
                        Is.Not.EqualTo(Convert.ToHexString(first.Signature)));

            Assert.That(random.Verify(key.ToPublicCOSEKey(), out var errorResponse),  Is.True,  errorResponse);

        }

        #endregion

        #region A_post_quantum_signature_is_what_a_byte_string_is_for()

        [Test]
        public void A_post_quantum_signature_is_what_a_byte_string_is_for()
        {

            // A metrological reading of about thirty bytes, signed with the
            // strongest parameter set. In JSON the signature would travel as
            // base64 and grow by a further third; in CBOR a byte string costs
            // its bytes and a three-byte head.
            var reading  = Convert.FromHexString("D9ACDC84C482221A0012D6870203A401C48220187B020203C48221185F0401");

            var key      = COSEKey.From(
                               MLDsaPrivateKeyParameters.FromSeed(
                                   COSEAlgorithm.MLDsa87.MLDsaParameterSet!,
                                   Convert.FromHexString(mlDsaSeed)
                               )
                           );

            var message  = COSESign1.Sign(reading, key, Deterministic: true).ToByteArray();

            Assert.That(reading.Length,   Is.EqualTo(31));
            Assert.That(message.Length,   Is.GreaterThan(4627));
            Assert.That(message.Length,   Is.LessThan(4800));

        }

        #endregion

    }

}
