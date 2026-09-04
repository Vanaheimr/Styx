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

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Deterministic ECDSA [RFC 6979]: the nonce derived from the private key
    /// and the message instead of drawn at random.
    ///
    /// Two things follow. A device without a dependable source of randomness
    /// - a meter, a smart card - can no longer be made unsafe by a poor one,
    /// since a repeated nonce hands over the private key. And a signature
    /// becomes a pure function of what is signed, so a published example can
    /// be recomputed rather than merely verified.
    ///
    /// The golden vectors are those of RFC 6979, Appendix A.2.5.
    /// </summary>
    [TestFixture]
    public class COSEDeterministicSignatureTests
    {

        #region Data

        // RFC 6979, Appendix A.2.5: ECDSA, 256 bits, curve NIST P-256.
        private const String  privateKey     = "C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721";

        private static readonly (String Message, String R, String S)[] vectors = [
            ("sample", "EFD48B2AACB6A8FD1140DD9CD45E81D69D2C877B56AAF991C34D0EA84EAF3716",
                       "F7CB1C942D657C41D436C7A1B6E29F65F3E900DBB9AFF4064DC4AB2F843ACDA8"),
            ("test",   "F1ABB023518351CD71D881567B1EA663ED3EFCF6C5132B354F28D3B0B7D38367",
                       "019F4113742A2B14BD25926B49C649155F267E60D3814B4C0CC84250E46F0083")
        ];

        #endregion

        #region (private static) ExamplePrivateKey()

        private static ECPrivateKeyParameters ExamplePrivateKey()

            => new (new BigInteger(privateKey, 16),
                    COSECurve.P256.DomainParameters!);

        #endregion


        #region The_published_vectors_are_reproduced()

        [Test]
        public void The_published_vectors_are_reproduced()
        {

            foreach (var vector in vectors)
            {

                var signature = COSEAlgorithm.ES256.Sign(
                                    vector.Message.ToUTF8Bytes(),
                                    ExamplePrivateKey(),
                                    Deterministic: true
                                );

                Assert.That(signature.Length,  Is.EqualTo(64),  vector.Message);

                // r and s, each 32 bytes wide, exactly as the RFC prints them.
                Assert.That(Convert.ToHexString(signature[..32]),  Is.EqualTo(vector.R),  $"{vector.Message}: r");
                Assert.That(Convert.ToHexString(signature[32..]),  Is.EqualTo(vector.S),  $"{vector.Message}: s");

            }

        }

        #endregion

        #region A_deterministic_signature_is_a_function_of_what_it_signs()

        [Test]
        public void A_deterministic_signature_is_a_function_of_what_it_signs()
        {

            var privateKey  = ExamplePrivateKey();
            var publicKey   = Crypto.CalculatePublicKey(privateKey);
            var payload     = "This is the content.".ToUTF8Bytes();

            var first       = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256, Deterministic: true);
            var second      = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256, Deterministic: true);

            // Signing the same thing twice yields the very same bytes...
            Assert.That(Convert.ToHexString(second.ToByteArray()),
                        Is.EqualTo(Convert.ToHexString(first.ToByteArray())));

            Assert.That(first.Verify(publicKey, out var errorResponse),  Is.True, errorResponse);

            // ...whereas the randomized signature of the same thing does not,
            // although it verifies just as well.
            var random      = COSESign1.Sign(payload, privateKey, COSEAlgorithm.ES256);

            Assert.That(random.Signature,  Is.Not.EqualTo(first.Signature));
            Assert.That(random.Verify(publicKey, out var randomError),  Is.True, randomError);

            // A different payload of course yields a different signature.
            var other       = COSESign1.Sign("Something else.".ToUTF8Bytes(), privateKey, COSEAlgorithm.ES256, Deterministic: true);

            Assert.That(other.Signature,  Is.Not.EqualTo(first.Signature));

        }

        #endregion

        #region Determinism_reaches_every_signing_path()

        [Test]
        public void Determinism_reaches_every_signing_path()
        {

            var meter    = new ECPrivateKeyParameters(new BigInteger("0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20", 16),
                                                      COSECurve.P256.DomainParameters!);

            var operatr  = new ECPrivateKeyParameters(new BigInteger("202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F", 16),
                                                      COSECurve.P256.DomainParameters!);

            var payload  = "This is the content.".ToUTF8Bytes();

            String  CountersignedSign1()

                => Convert.ToHexString(
                       COSESign1.Sign(payload, meter, COSEAlgorithm.ES256, Deterministic: true).
                                 AddCountersignature(operatr, COSEAlgorithm.ES256, Deterministic: true).
                                 ToByteArray()
                   );

            String  SignWithTwoSigners()

                => Convert.ToHexString(
                       COSESign.Sign(payload, meter, COSEAlgorithm.ES256, Deterministic: true).
                                AddSignature(operatr, COSEAlgorithm.ES256, Deterministic: true).
                                ToByteArray()
                   );

            // Every signing path has to honour it, or a record that mixes
            // them is reproducible only in part.
            Assert.That(CountersignedSign1(),   Is.EqualTo(CountersignedSign1()),   "COSE_Sign1 with a countersignature");
            Assert.That(SignWithTwoSigners(),   Is.EqualTo(SignWithTwoSigners()),   "COSE_Sign with two signatures");

        }

        #endregion

        #region Randomness_and_determinism_are_mutually_exclusive()

        [Test]
        public void Randomness_and_determinism_are_mutually_exclusive()
        {

            Assert.That(() => COSEAlgorithm.ES256.Sign("sample".ToUTF8Bytes(),
                                                       ExamplePrivateKey(),
                                                       new SecureRandom(),
                                                       true),
                        Throws.TypeOf<COSEException>());

        }

        #endregion

        #region Deterministic_signatures_work_on_every_supported_curve()

        [Test]
        public void Deterministic_signatures_work_on_every_supported_curve()
        {

            var vectors = new (COSEAlgorithm Algorithm, String CurveName)[] {
                              (COSEAlgorithm.ES256,   "secp256r1"),
                              (COSEAlgorithm.ES384,   "secp384r1"),
                              (COSEAlgorithm.ES512,   "secp521r1"),
                              (COSEAlgorithm.ESB256,  "brainpoolP256r1"),
                              (COSEAlgorithm.ESB512,  "brainpoolP512r1")
                          };

            foreach (var vector in vectors)
            {

                var keyPair     = Crypto.GenerateKeys(ECNamedCurveTable.GetByName(vector.CurveName));
                var privateKey  = (ECPrivateKeyParameters) keyPair.Private;
                var publicKey   = (ECPublicKeyParameters)  keyPair.Public;

                var payload     = "This is the content.".ToUTF8Bytes();

                var first       = vector.Algorithm.Sign(payload, privateKey, null, true);
                var second      = vector.Algorithm.Sign(payload, privateKey, null, true);

                Assert.That(second,  Is.EqualTo(first),  vector.Algorithm.Name);

                Assert.That(vector.Algorithm.Verify(payload, first, publicKey, out var errorResponse),
                            Is.True,  $"{vector.Algorithm.Name}: {errorResponse}");

            }

        }

        #endregion

    }

}
