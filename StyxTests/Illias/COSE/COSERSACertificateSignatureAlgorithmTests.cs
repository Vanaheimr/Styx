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

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// The parameters beside an RSASSA-PKCS1-v1_5 signature algorithm of a
    /// certificate. "When any of these four object identifiers appears within
    /// an AlgorithmIdentifier, the parameters MUST be NULL. Implementations
    /// MUST accept the parameters being absent as well as present" [RFC 4055,
    /// Section 5].
    ///
    /// Bouncy Castle compares the two copies of the identifier, the one in the
    /// signed body and the one beside it, but it accepts whatever parameters
    /// both share. A certificate whose odd parameters are consistent in both
    /// copies has to be minted for that, and the signature factory below does
    /// it: a real RSASSA-PKCS1-v1_5 signature under an identifier that carries
    /// whatever parameters a test asks for.
    /// </summary>
    [TestFixture]
    public class COSERSACertificateSignatureAlgorithmTests
    {

        #region Data

        private static readonly Byte[] payload = "This is the content.".ToUTF8Bytes();

        private static Int64 serialNumber;

        private AsymmetricCipherKeyPair  rootKey          = null!;
        private AsymmetricCipherKeyPair  intermediateKey  = null!;
        private AsymmetricCipherKeyPair  leafKey          = null!;
        private X509Certificate          root             = null!;

        #endregion

        #region (private) ParameterisedSignatureFactory

        /// <summary>
        /// A signature factory that signs as the given algorithm does and
        /// names that algorithm with the given parameters, which Bouncy
        /// Castle's generator then writes into both copies.
        /// </summary>
        private sealed class ParameterisedSignatureFactory(String                  Algorithm,
                                                           DerObjectIdentifier     Identifier,
                                                           Asn1Encodable?          Parameters,
                                                           AsymmetricKeyParameter  PrivateKey) : ISignatureFactory
        {

            private readonly Asn1SignatureFactory signer = new (Algorithm, PrivateKey);

            public Object AlgorithmDetails

                => Parameters is null
                       ? new AlgorithmIdentifier(Identifier)
                       : new AlgorithmIdentifier(Identifier, Parameters);

            public IStreamCalculator<IBlockResult> CreateCalculator()

                => signer.CreateCalculator();

        }

        #endregion

        #region Init()

        [OneTimeSetUp]
        public void Init()
        {

            var generator = new RsaKeyPairGenerator();
            generator.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf(65537), new SecureRandom(), 2048, 100));

            rootKey          = generator.GenerateKeyPair();
            intermediateKey  = generator.GenerateKeyPair();
            leafKey          = generator.GenerateKeyPair();

            root             = Issue("RSA Test Root CA", rootKey.Public, IsCA: true,
                                     new Asn1SignatureFactory("SHA256WITHRSA", rootKey.Private), "RSA Test Root CA");

        }

        #endregion

        #region (private) Issue(Subject, PublicKey, IsCA, Signer, Issuer)

        private static X509Certificate Issue(String                  Subject,
                                             AsymmetricKeyParameter  PublicKey,
                                             Boolean                 IsCA,
                                             ISignatureFactory       Signer,
                                             String                  Issuer)
        {

            var generator = new X509V3CertificateGenerator();

            generator.SetSerialNumber (BigInteger.ValueOf(Interlocked.Increment(ref serialNumber)));
            generator.SetSubjectDN    (new X509Name($"CN={Subject}"));
            generator.SetIssuerDN     (new X509Name($"CN={Issuer}"));
            generator.SetNotBefore    (DateTime.UtcNow.AddYears(-1));
            generator.SetNotAfter     (DateTime.UtcNow.AddYears( 1));
            generator.SetPublicKey    (PublicKey);

            generator.AddExtension(X509Extensions.BasicConstraints,
                                   true,
                                   new BasicConstraints(IsCA));

            generator.AddExtension(X509Extensions.KeyUsage,
                                   true,
                                   new KeyUsage(IsCA
                                                    ? KeyUsage.KeyCertSign | KeyUsage.DigitalSignature
                                                    : KeyUsage.DigitalSignature));

            return generator.Generate(Signer);

        }

        #endregion

        #region (private) Leaf(Parameters, Algorithm = SHA256WITHRSA, Identifier = sha256WithRSAEncryption)

        /// <summary>
        /// A leaf certificate signed by the root with RSASSA-PKCS1-v1_5, its
        /// signature algorithm written with the given parameters, or with none
        /// for null.
        /// </summary>
        private X509Certificate Leaf(Asn1Encodable?        Parameters,
                                     String                Algorithm   = "SHA256WITHRSA",
                                     DerObjectIdentifier?  Identifier  = null)

            => Issue("Meter RSA",
                     leafKey.Public,
                     IsCA:   false,
                     new ParameterisedSignatureFactory(Algorithm,
                                                       Identifier ?? PkcsObjectIdentifiers.Sha256WithRsaEncryption,
                                                       Parameters,
                                                       rootKey.Private),
                     "RSA Test Root CA");

        #endregion

        #region (private) Verify(Certificates, out ErrorResponse)

        private Boolean Verify(X509Certificate[]  Certificates,
                               out String?        ErrorResponse)
        {

            var signed = COSESign1.Sign(payload,
                                        leafKey.Private,
                                        new COSEHeaders([
                                            (COSEHeaderLabel.Algorithm, COSEAlgorithm.PS256.ToCBOR()),
                                            (COSEHeaderLabel.X5Chain,   new COSECertificateChain(Certificates).ToCBOR())
                                        ]));

            return COSESign1.Parse(signed.ToByteArray()).
                             VerifyWithCertificateChain([ root ], out _, out ErrorResponse);

        }

        #endregion


        #region A_NULL_beside_the_signature_algorithm_is_read()

        [Test]
        public void A_NULL_beside_the_signature_algorithm_is_read()
        {

            var leaf = Leaf(DerNull.Instance);

            Assert.That(leaf.CertificateStructure.SignatureAlgorithm.Parameters,  Is.InstanceOf<Asn1Null>());

            Assert.That(Verify([ leaf ], out var errorResponse),
                        Is.True, errorResponse);

        }

        #endregion

        #region No_parameters_beside_the_signature_algorithm_are_read()

        [Test]
        public void No_parameters_beside_the_signature_algorithm_are_read()
        {

            // "Implementations MUST accept the parameters being absent".
            var leaf = Leaf(null);

            Assert.That(leaf.CertificateStructure.SignatureAlgorithm.Parameters,  Is.Null);

            Assert.That(Verify([ leaf ], out var errorResponse),
                        Is.True, errorResponse);

        }

        #endregion

        #region Other_parameters_beside_the_signature_algorithm_are_refused(Parameters)

        private static readonly Object[] otherParameters = [
            new Object[] { "a zero-length OCTET STRING", new DerOctetString(Array.Empty<Byte>()) },
            new Object[] { "an INTEGER",                 DerInteger.ValueOf(0) },
            new Object[] { "an empty SEQUENCE",          DerSequence.Empty }
        ];

        [TestCaseSource(nameof(otherParameters))]
        public void Other_parameters_beside_the_signature_algorithm_are_refused(String         What,
                                                                               Asn1Encodable  Parameters)
        {

            var leaf = Leaf(Parameters);

            // The signature is good and both copies agree, so this is not a
            // refusal for a broken certificate. Bouncy Castle verifies it.
            leaf.Verify(rootKey.Public);

            Assert.That(Verify([ leaf ], out var errorResponse),
                        Is.False, What);

            Assert.That(errorResponse,  Does.Contain("position 0"));
            Assert.That(errorResponse,  Does.Contain("'1.2.840.113549.1.1.11'"));
            Assert.That(errorResponse,  Does.Contain("RFC 4055 Section 5"));

        }

        #endregion

        #region Every_PKCS1_signature_algorithm_is_held_to_it(Algorithm)

        private static readonly Object[] pkcs1Algorithms = [
            new Object[] { "SHA1WITHRSA",   PkcsObjectIdentifiers.Sha1WithRsaEncryption   },
            new Object[] { "SHA384WITHRSA", PkcsObjectIdentifiers.Sha384WithRsaEncryption },
            new Object[] { "SHA512WITHRSA", PkcsObjectIdentifiers.Sha512WithRsaEncryption }
        ];

        [TestCaseSource(nameof(pkcs1Algorithms))]
        public void Every_PKCS1_signature_algorithm_is_held_to_it(String               Algorithm,
                                                                 DerObjectIdentifier  Identifier)
        {

            Assert.That(Verify([ Leaf(DerNull.Instance, Algorithm, Identifier) ], out var errorResponse),
                        Is.True, errorResponse);

            Assert.That(Verify([ Leaf(new DerOctetString(Array.Empty<Byte>()), Algorithm, Identifier) ], out errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain($"'{Identifier}'"));

        }

        #endregion

        #region An_issuing_certificate_is_held_to_it_as_well()

        [Test]
        public void An_issuing_certificate_is_held_to_it_as_well()
        {

            var intermediate = Issue("RSA Test Manufacturer CA",
                                     intermediateKey.Public,
                                     IsCA: true,
                                     new ParameterisedSignatureFactory("SHA256WITHRSA",
                                                                       PkcsObjectIdentifiers.Sha256WithRsaEncryption,
                                                                       new DerOctetString(Array.Empty<Byte>()),
                                                                       rootKey.Private),
                                     "RSA Test Root CA");

            var leaf         = Issue("Meter RSA",
                                     leafKey.Public,
                                     IsCA: false,
                                     new Asn1SignatureFactory("SHA256WITHRSA", intermediateKey.Private),
                                     "RSA Test Manufacturer CA");

            Assert.That(Verify([ leaf, intermediate ], out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("position 1"));

        }

        #endregion

    }

}
