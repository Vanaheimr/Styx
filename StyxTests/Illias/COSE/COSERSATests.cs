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

using System.Security.Cryptography;

using NUnit.Framework;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

using BCBigInteger = Org.BouncyCastle.Math.BigInteger;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// RSA keys [RFC 8230] and the RSA signature algorithms: RSASSA-PSS
    /// (PS256, PS384, PS512) and RSASSA-PKCS1-v1_5 (RS256, RS384, RS512).
    ///
    /// There are no published COSE vectors to reproduce, so every signature
    /// is checked against a second, independent implementation instead: the
    /// RSA of .NET. What Styx signs .NET must verify and the other way round,
    /// and RSASSA-PKCS1-v1_5, which has no randomness at all, must come out
    /// of both as the very same bytes.
    /// </summary>
    [TestFixture]
    public class COSERSATests
    {

        #region Data

        private static readonly Byte[] content = "This is the content.".ToUTF8Bytes();

        private static readonly (COSEAlgorithm Algorithm, HashAlgorithmName Hash, RSASignaturePadding Padding)[] algorithms = [
            (COSEAlgorithm.PS256, HashAlgorithmName.SHA256, RSASignaturePadding.Pss),
            (COSEAlgorithm.PS384, HashAlgorithmName.SHA384, RSASignaturePadding.Pss),
            (COSEAlgorithm.PS512, HashAlgorithmName.SHA512, RSASignaturePadding.Pss),
            (COSEAlgorithm.RS256, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1),
            (COSEAlgorithm.RS384, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1),
            (COSEAlgorithm.RS512, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1)
        ];

        private RSA                         dotNetKey   = null!;
        private RsaPrivateCrtKeyParameters  privateKey  = null!;

        #endregion

        #region Setup / Teardown

        [OneTimeSetUp]
        public void CreateKey()
        {
            dotNetKey   = RSA.Create(2048);
            privateKey  = ToBouncyCastle(dotNetKey.ExportParameters(true));
        }

        [OneTimeTearDown]
        public void DisposeKey()
        {
            dotNetKey.Dispose();
        }

        private static RsaPrivateCrtKeyParameters ToBouncyCastle(RSAParameters Parameters)

            => new (new BCBigInteger(1, Parameters.Modulus),
                    new BCBigInteger(1, Parameters.Exponent),
                    new BCBigInteger(1, Parameters.D),
                    new BCBigInteger(1, Parameters.P),
                    new BCBigInteger(1, Parameters.Q),
                    new BCBigInteger(1, Parameters.DP),
                    new BCBigInteger(1, Parameters.DQ),
                    new BCBigInteger(1, Parameters.InverseQ));

        #endregion


        #region An_RSA_key_roundtrips_and_has_one_thumbprint()

        [Test]
        public void An_RSA_key_roundtrips_and_has_one_thumbprint()
        {

            var key        = COSEKey.From(privateKey, Algorithm: COSEAlgorithm.PS256);
            var parsed     = COSEKey.Parse(key.ToByteArray());
            var publicKey  = key.ToPublicCOSEKey();

            Assert.That(key.KeyType,                                  Is.EqualTo(COSEKeyType.RSA));
            Assert.That(key.IsPrivate,                                Is.True);
            Assert.That(key.ToString(),                               Does.StartWith("RSA 2048 private key"));
            Assert.That(parsed.ToByteArray(),                         Is.EqualTo(key.ToByteArray()));
            Assert.That(parsed.ToPrivateKey(),                        Is.EqualTo(privateKey));

            // The public half carries n and e and nothing private.
            Assert.That(publicKey.IsPrivate,                          Is.False);
            Assert.That(publicKey.D,                                  Is.Null);
            Assert.That(publicKey.P,                                  Is.Null);
            Assert.That(publicKey.QInv,                               Is.Null);
            Assert.That(publicKey.ToPublicKey(),                      Is.EqualTo(new RsaKeyParameters(false, privateKey.Modulus, privateKey.PublicExponent)));

            // RFC 9679, Section 4.3: kty, n and e, nothing else - so both
            // halves of the key pair share one identity.
            var expectedInput = CBORValue.FromMap([
                                    new (COSEKey.KeyTypeLabel, CBORValue.FromInt64(3)),
                                    new (COSEKey.NLabel,       CBORValue.FromBytes(key.N!)),
                                    new (COSEKey.ELabel,       CBORValue.FromBytes(key.E!))
                                ]).ToByteArray(CBORWriterOptions.Canonical);

            Assert.That(key.ThumbprintInput(),                        Is.EqualTo(expectedInput));
            Assert.That(publicKey.Thumbprint(),                       Is.EqualTo(key.Thumbprint()));

            // The fewest octets: 2048 bits are 256 bytes, and 65537 three.
            Assert.That(key.N!.Length,                                Is.EqualTo(256));
            Assert.That(key.E,                                        Is.EqualTo(new Byte[] { 0x01, 0x00, 0x01 }));

            // A key identifier from the thumbprint keeps the key whole.
            var withKeyId = key.WithThumbprintKeyIdentifier();

            Assert.That(withKeyId.ToPrivateKey(),                     Is.EqualTo(privateKey));

        }

        #endregion

        #region Every_RSA_algorithm_signs_a_COSE_Sign1_that_verifies()

        [Test]
        public void Every_RSA_algorithm_signs_a_COSE_Sign1_that_verifies()
        {

            foreach (var vector in algorithms)
            {

                var key      = COSEKey.From(privateKey, Algorithm: vector.Algorithm);
                var message  = COSESign1.Sign(content, key);

                Assert.That(message.Signature.Length,                                         Is.EqualTo(256),  vector.Algorithm.Name);
                Assert.That(message.Verify(key.ToPublicCOSEKey(), out var errorResponse),      Is.True,          errorResponse);

                var parsed   = COSESign1.Parse(message.ToByteArray());

                Assert.That(parsed.Verify(key.ToPublicCOSEKey(), out errorResponse),           Is.True,          errorResponse);

                // One bit of the signature changed, and it is no signature.
                var tampered = message.ToByteArray();
                tampered[^1] ^= 0x01;

                Assert.That(COSESign1.Parse(tampered).Verify(key.ToPublicCOSEKey(), out _),   Is.False,         vector.Algorithm.Name);

            }

        }

        #endregion

        #region Styx_and_dotNET_verify_each_others_RSA_signatures()

        [Test]
        public void Styx_and_dotNET_verify_each_others_RSA_signatures()
        {

            var publicKey = new RsaKeyParameters(false, privateKey.Modulus, privateKey.PublicExponent);

            foreach (var vector in algorithms)
            {

                var byStyx    = vector.Algorithm.Sign(content, privateKey);
                var byDotNET  = dotNetKey.SignData(content, vector.Hash, vector.Padding);

                Assert.That(dotNetKey.VerifyData(content, byStyx, vector.Hash, vector.Padding),     Is.True,  vector.Algorithm.Name);
                Assert.That(vector.Algorithm.Verify(content, byDotNET, publicKey, out var error),   Is.True,  error);

                // PKCS #1 v1.5 has no randomness: two implementations, one result.
                if (vector.Padding == RSASignaturePadding.Pkcs1)
                    Assert.That(byStyx,  Is.EqualTo(byDotNET),  vector.Algorithm.Name);

            }

        }

        #endregion

        #region A_deterministic_PSS_signature_is_the_same_every_time()

        [Test]
        public void A_deterministic_PSS_signature_is_the_same_every_time()
        {

            var first   = COSEAlgorithm.PS256.Sign(content, privateKey, Deterministic: true);
            var second  = COSEAlgorithm.PS256.Sign(content, privateKey, Deterministic: true);

            Assert.That(second,  Is.EqualTo(first));

            // ...and an ordinary PSS signature to anybody else, who recovers
            // the salt rather than comparing it with anything.
            Assert.That(dotNetKey.VerifyData(content, first, HashAlgorithmName.SHA256, RSASignaturePadding.Pss),  Is.True);

            // A random salt makes two signatures of the same content differ.
            var salted       = COSEAlgorithm.PS256.Sign(content, privateKey);
            var saltedAgain  = COSEAlgorithm.PS256.Sign(content, privateKey);

            Assert.That(saltedAgain, Is.Not.EqualTo(salted));

        }

        #endregion


        #region Keys_below_2048_bits_are_refused()

        [Test]
        public void Keys_below_2048_bits_are_refused()
        {

            using var small    = RSA.Create(1024);
            var smallKey       = ToBouncyCastle(small.ExportParameters(true));
            var smallPublic    = new RsaKeyParameters(false, smallKey.Modulus, smallKey.PublicExponent);

            Assert.That(() => COSEAlgorithm.PS256.Sign(content, smallKey),
                        Throws.TypeOf<COSEException>().With.Message.Contains("at least 2048 bits"));

            var signature      = small.SignData(content, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

            Assert.That(COSEAlgorithm.PS256.Verify(content, signature, smallPublic, out var error),  Is.False);
            Assert.That(error,  Does.Contain("at least 2048 bits"));

        }

        #endregion

        #region The_wrong_key_for_the_job_is_refused()

        [Test]
        public void The_wrong_key_for_the_job_is_refused()
        {

            var publicKey  = new RsaKeyParameters(false, privateKey.Modulus, privateKey.PublicExponent);
            var signature  = COSEAlgorithm.PS256.Sign(content, privateKey);

            Assert.That(() => COSEAlgorithm.PS256.Sign(content, publicKey),
                        Throws.TypeOf<COSEException>().With.Message.Contains("private RSA key"));

            Assert.That(COSEAlgorithm.PS256.Verify(content, signature, privateKey, out var error),  Is.False);
            Assert.That(error,  Does.Contain("public RSA key"));

            Assert.That(() => COSEAlgorithm.PS256.Sign(content, new Ed25519PrivateKeyParameters(new SecureRandom())),
                        Throws.TypeOf<COSEException>().With.Message.Contains("needs an RSA key"));

            Assert.That(COSEAlgorithm.PS256.Verify(content, signature[1..], publicKey, out error),  Is.False);
            Assert.That(error,  Does.Contain("must be 256 bytes long"));

        }

        #endregion

        #region Malformed_RSA_keys_are_refused()

        [Test]
        public void Malformed_RSA_keys_are_refused()
        {

            var key = COSEKey.From(privateKey);

            // A leading zero: the same integer, another spelling, another
            // thumbprint - so it is refused [RFC 8230, Section 4].
            var leadingZero = CBORValue.FromMap([
                                  new (COSEKey.KeyTypeLabel, CBORValue.FromInt64(3)),
                                  new (COSEKey.NLabel,       CBORValue.FromBytes([ 0x00, .. key.N! ])),
                                  new (COSEKey.ELabel,       CBORValue.FromBytes(key.E!))
                              ]);

            Assert.That(COSEKey.TryParse(leadingZero, out _, out var error),  Is.False);
            Assert.That(error,  Does.Contain("minimum number of octets"));

            // No public exponent.
            var noExponent  = CBORValue.FromMap([
                                  new (COSEKey.KeyTypeLabel, CBORValue.FromInt64(3)),
                                  new (COSEKey.NLabel,       CBORValue.FromBytes(key.N!))
                              ]);

            Assert.That(COSEKey.TryParse(noExponent, out _, out error),  Is.False);
            Assert.That(error,  Does.Contain("public exponent e"));

            // A dP that does not belong to d: a CRT fault waiting to factor n.
            var dp          = new BCBigInteger(1, key.DP!).Add(BCBigInteger.Two);
            var faulty      = new COSEKey(COSEKeyType.RSA,
                                          D:     key.D,
                                          N:     key.N,
                                          E:     key.E,
                                          P:     key.P,
                                          Q:     key.Q,
                                          DP:    dp.ToByteArrayUnsigned(),
                                          DQ:    key.DQ,
                                          QInv:  key.QInv);

            Assert.That(faulty.TryToPrivateKey(out _, out error),  Is.False);
            Assert.That(error,  Does.Contain("CRT parameters"));

            // A private key without its CRT parameters.
            var bare        = new COSEKey(COSEKeyType.RSA, D: key.D, N: key.N, E: key.E);

            Assert.That(bare.TryToPrivateKey(out _, out error),  Is.False);
            Assert.That(error,  Does.Contain("d, p, q, dP, dQ and qInv"));

            // A multi-prime key: kept, but not computed with.
            var multiPrime  = new COSEKey(COSEKeyType.RSA,
                                          D:                     key.D,
                                          AdditionalParameters:  [ new (COSEKey.OtherPrimesLabel, CBORValue.FromArray([])) ],
                                          N:                     key.N,
                                          E:                     key.E,
                                          P:                     key.P,
                                          Q:                     key.Q,
                                          DP:                    key.DP,
                                          DQ:                    key.DQ,
                                          QInv:                  key.QInv);

            Assert.That(multiPrime.TryToPrivateKey(out _, out error),  Is.False);
            Assert.That(error,  Does.Contain("Multi-prime"));
            Assert.That(multiPrime.ToPublicCOSEKey().AdditionalParameters,  Is.Empty);

        }

        #endregion

    }

}
