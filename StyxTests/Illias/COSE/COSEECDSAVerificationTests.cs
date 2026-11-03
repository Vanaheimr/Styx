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
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// ECDSA signatures on the NIST curves are verified by the platform (CNG, OpenSSL), the other curves by Bouncy Castle.
    /// Both must decide alike: every case here is also decided by Bouncy Castle itself, the way Styx verified before.
    /// </summary>
    [TestFixture]
    public class COSEECDSAVerificationTests
    {

        #region Data

        private static readonly Byte[] toBeSigned = "The signature input of a COSE_Sign1.".ToUTF8Bytes();

        /// <summary>
        /// The algorithms with the curves of their keys: the fully-specified ones, the deployed ones leaving the curve to
        /// the key, also with a digest shorter and longer than the order, and Brainpool, still verified by Bouncy Castle.
        /// </summary>
        private static readonly Object[] algorithmsAndCurves = [
            new Object[] { "ESP256", "P-256" },
            new Object[] { "ESP384", "P-384" },
            new Object[] { "ESP512", "P-521" },
            new Object[] { "ES256",  "P-256" },
            new Object[] { "ES512",  "P-521" },
            new Object[] { "ES256",  "P-521" },
            new Object[] { "ES512",  "P-256" },
            new Object[] { "ESB256", "brainpoolP256r1" }
        ];

        #endregion

        #region (static) KeyPair(Curve)

        private static (ECPrivateKeyParameters Private, ECPublicKeyParameters Public, BigInteger N) KeyPair(String Curve)
        {

            var domainParameters  = COSECurve.Parse(Curve).DomainParameters!;
            var generator         = new ECKeyPairGenerator();
            generator.Init(new ECKeyGenerationParameters(domainParameters, new SecureRandom()));

            var keyPair = generator.GenerateKeyPair();

            return ((ECPrivateKeyParameters) keyPair.Private, (ECPublicKeyParameters) keyPair.Public, domainParameters.N);

        }

        #endregion

        #region (static) ByBouncyCastle(Algorithm, PublicKey, ToBeSigned, Signature)

        /// <summary>
        /// The decision of Bouncy Castle, as Styx verified before.
        /// </summary>
        private static Boolean ByBouncyCastle(COSEAlgorithm          Algorithm,
                                              ECPublicKeyParameters  PublicKey,
                                              Byte[]                 ToBeSigned,
                                              Byte[]                 Signature)
        {

            var size      = (PublicKey.Parameters.N.BitLength + 7) / 8;
            var verifier  = new ECDsaSigner();
            verifier.Init(false, PublicKey);

            return verifier.VerifySignature(Algorithm.Hash(ToBeSigned),
                                            new BigInteger(1, Signature, 0,    size),
                                            new BigInteger(1, Signature, size, size));

        }

        #endregion

        #region (static) Component(Value, Size)

        private static Byte[] Component(BigInteger Value, Int32 Size)
            => BigIntegers.AsUnsignedByteArray(Size, Value);

        #endregion


        #region Verified_as_Bouncy_Castle_verifies(Algorithm, Curve)

        [TestCaseSource(nameof(algorithmsAndCurves))]
        public void Verified_as_Bouncy_Castle_verifies(String Algorithm, String Curve)
        {

            var algorithm  = COSEAlgorithm.Parse(Algorithm);
            var key        = KeyPair(Curve);
            var size       = (key.N.BitLength + 7) / 8;
            var signature  = algorithm.Sign(toBeSigned, key.Private);
            var r          = new BigInteger(1, signature, 0,    size);
            var s          = new BigInteger(1, signature, size, size);

            var otherInput = toBeSigned.ToArray();
            otherInput[0] ^= 0x01;

            var flipped    = signature.ToArray();
            flipped[^1]   ^= 0x01;

            var cases = new (String Case, Byte[] ToBeSigned, Byte[] Signature, Boolean Valid)[] {
                ("valid",                   toBeSigned,  signature,                                                       true),
                // Its mirror, valid in ECDSA as such: COSE does not ask for a low s.
                ("high s",                  toBeSigned,  [ .. Component(r, size), .. Component(key.N.Subtract(s), size) ], true),
                ("another input",           otherInput,  signature,                                                       false),
                ("a bit flipped",           toBeSigned,  flipped,                                                         false),
                ("r zero",                  toBeSigned,  [ .. Component(BigInteger.Zero, size), .. Component(s, size) ],   false),
                ("s zero",                  toBeSigned,  [ .. Component(r, size), .. Component(BigInteger.Zero, size) ],   false),
                ("r the order",             toBeSigned,  [ .. Component(key.N, size), .. Component(s, size) ],            false),
                ("s the order",             toBeSigned,  [ .. Component(r, size), .. Component(key.N, size) ],            false),
                ("all bits set",            toBeSigned,  Enumerable.Repeat((Byte) 0xFF, 2 * size).ToArray(),              false)
            };

            using (Assert.EnterMultipleScope())
            {
                foreach (var (@case, input, candidate, valid) in cases)
                {

                    var expected = ByBouncyCastle(algorithm, key.Public, input, candidate);

                    Assert.That(expected, Is.EqualTo(valid), $"{@case}: Bouncy Castle");

                    Assert.That(algorithm.Verify(input, candidate, key.Public, out var errorResponse),
                                Is.EqualTo(expected),
                                $"{@case}: {errorResponse}");

                }
            }

        }

        #endregion

        #region A_signature_of_the_wrong_size_is_refused()

        [Test]
        public void A_signature_of_the_wrong_size_is_refused()
        {

            var key        = KeyPair("P-256");
            var signature  = COSEAlgorithm.ESP256.Sign(toBeSigned, key.Private);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(COSEAlgorithm.ESP256.Verify(toBeSigned, signature[..^1],         key.Public, out var shorter), Is.False);
                Assert.That(shorter, Does.Contain("64 bytes long"));
                Assert.That(COSEAlgorithm.ESP256.Verify(toBeSigned, [ .. signature, 0x00 ], key.Public, out var longer),  Is.False);
                Assert.That(longer,  Does.Contain("64 bytes long"));
            }

        }

        #endregion

        #region A_key_on_another_curve_than_the_algorithm_is_refused()

        [Test]
        public void A_key_on_another_curve_than_the_algorithm_is_refused()
        {

            var key        = KeyPair("P-384");
            var signature  = COSEAlgorithm.ESP384.Sign(toBeSigned, key.Private);

            Assert.That(COSEAlgorithm.ESP256.Verify(toBeSigned, signature, key.Public, out var errorResponse), Is.False);
            Assert.That(errorResponse, Does.Contain("not the curve of the given key"));

        }

        #endregion

    }

}
