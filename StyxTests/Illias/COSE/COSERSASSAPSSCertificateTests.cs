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
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Nist;
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
    /// A certificate whose key is written as id-RSASSA-PSS [RFC 4055,
    /// Section 3.1] rather than as rsaEncryption. That identifier says what
    /// the key is for, and its parameters, when present, say which
    /// RSASSA-PSS. "All parameters in the signature structure algorithm
    /// identifier MUST match the parameters in the key structure algorithm
    /// identifier except the saltLength field", and the salt length of the
    /// signature "MUST be greater or equal to that in the key parameters
    /// field" [RFC 4055, Section 3.3].
    ///
    /// Bouncy Castle reads such a key as an RsaKeyParameters that cannot be
    /// told apart from an rsaEncryption key. Before COSECertificateChain read
    /// the certificate for it, the restriction never reached the
    /// verification, and a PS256 message verified under a key restricted to
    /// SHA-384.
    /// </summary>
    [TestFixture]
    public class COSERSASSAPSSCertificateTests
    {

        #region Data

        private static readonly Byte[] payload = "This is the content.".ToUTF8Bytes();

        private static Int64 serialNumber;

        private AsymmetricCipherKeyPair  rootKey       = null!;
        private AsymmetricCipherKeyPair  leafKey       = null!;
        private AsymmetricCipherKeyPair  authorityKey  = null!;
        private X509Certificate          root          = null!;

        #endregion

        #region Init()

        [OneTimeSetUp]
        public void Init()
        {

            var generator = new RsaKeyPairGenerator();
            generator.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf(65537), new SecureRandom(), 2048, 100));

            rootKey       = generator.GenerateKeyPair();
            leafKey       = generator.GenerateKeyPair();
            authorityKey  = generator.GenerateKeyPair();

            root          = Issue("RSA Test Root CA",
                                  SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(rootKey.Public),
                                  IsCA: true);

        }

        #endregion

        #region (private) Issue(Subject, PublicKeyInfo, IsCA = false)

        /// <summary>
        /// Issue a certificate for the given subject public key info, signed by
        /// the root (or self-signed, for the root itself) with
        /// sha256WithRSAEncryption.
        /// </summary>
        private X509Certificate Issue(String                Subject,
                                      SubjectPublicKeyInfo  PublicKeyInfo,
                                      Boolean               IsCA   = false)

            => IssueBy(Subject, PublicKeyInfo, "RSA Test Root CA", rootKey, "SHA256WITHRSA", IsCA);

        #endregion

        #region (private) IssueBy(Subject, PublicKeyInfo, Issuer, IssuerKey, SignatureAlgorithm, IsCA = false)

        /// <summary>
        /// Issue a certificate for the given subject public key info, signed by
        /// the given issuer with the given signature algorithm, as Bouncy Castle
        /// names it.
        /// </summary>
        private static X509Certificate IssueBy(String                   Subject,
                                               SubjectPublicKeyInfo     PublicKeyInfo,
                                               String                   Issuer,
                                               AsymmetricCipherKeyPair  IssuerKey,
                                               String                   SignatureAlgorithm,
                                               Boolean                  IsCA   = false)
        {

            var generator = new X509V3CertificateGenerator();

            generator.SetSerialNumber         (BigInteger.ValueOf(Interlocked.Increment(ref serialNumber)));
            generator.SetSubjectDN            (new X509Name($"CN={Subject}"));
            generator.SetIssuerDN             (new X509Name($"CN={Issuer}"));
            generator.SetNotBefore            (DateTime.UtcNow.AddYears(-1));
            generator.SetNotAfter             (DateTime.UtcNow.AddYears( 1));
            generator.SetSubjectPublicKeyInfo (PublicKeyInfo);

            generator.AddExtension(X509Extensions.BasicConstraints,
                                   true,
                                   new BasicConstraints(IsCA));

            generator.AddExtension(X509Extensions.KeyUsage,
                                   true,
                                   new KeyUsage(IsCA
                                                    ? KeyUsage.KeyCertSign | KeyUsage.DigitalSignature
                                                    : KeyUsage.DigitalSignature));

            return generator.Generate(new Asn1SignatureFactory(SignatureAlgorithm, IssuerKey.Private));

        }

        #endregion

        #region (private) PssKey(Parameters)

        /// <summary>
        /// The leaf key, or the given one, as an id-RSASSA-PSS subject public
        /// key info, with the given parameters or, for null, with none at all.
        /// </summary>
        private SubjectPublicKeyInfo PssKey(Asn1Encodable?            Parameters,
                                            AsymmetricCipherKeyPair?  Key   = null)
        {

            var key = (RsaKeyParameters) (Key ?? leafKey).Public;

            return new SubjectPublicKeyInfo(Parameters is null
                                                ? new AlgorithmIdentifier(PkcsObjectIdentifiers.IdRsassaPss)
                                                : new AlgorithmIdentifier(PkcsObjectIdentifiers.IdRsassaPss, Parameters),
                                            new RsaPublicKeyStructure(key.Modulus, key.Exponent));

        }

        #endregion

        #region (private static) Restriction(Hash, Salt, MaskHash = null, Trailer = 1)

        /// <summary>
        /// RSASSA-PSS-params [RFC 4055, Section 3.1], every field written out.
        /// </summary>
        private static RsassaPssParameters Restriction(DerObjectIdentifier   Hash,
                                                       Int32                 Salt,
                                                       DerObjectIdentifier?  MaskHash   = null,
                                                       Int32                 Trailer    = 1)

            => new (new AlgorithmIdentifier(Hash, DerNull.Instance),
                    new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1,
                                            new AlgorithmIdentifier(MaskHash ?? Hash, DerNull.Instance)),
                    new DerInteger(Salt),
                    new DerInteger(Trailer));

        #endregion

        #region (private) Verify(Certificate, Algorithm, out ErrorResponse)

        /// <summary>
        /// Sign with the leaf key under the given algorithm, carry the
        /// certificate as the message's x5chain, and verify it against the root.
        /// </summary>
        private Boolean Verify(X509Certificate  Certificate,
                               COSEAlgorithm    Algorithm,
                               out String?      ErrorResponse)
        {

            var signed = COSESign1.Sign(payload,
                                        leafKey.Private,
                                        new COSEHeaders([
                                            (COSEHeaderLabel.Algorithm, Algorithm.ToCBOR()),
                                            (COSEHeaderLabel.X5Chain,   new COSECertificateChain([ Certificate ]).ToCBOR())
                                        ]));

            return COSESign1.Parse(signed.ToByteArray()).
                             VerifyWithCertificateChain([ root ], out _, out ErrorResponse);

        }

        #endregion


        #region A_key_restricted_to_the_hash_its_message_uses_verifies(Algorithm)

        private static readonly Object[] matchingRestrictions = [
            new Object[] { "PS256", NistObjectIdentifiers.IdSha256, 32 },
            new Object[] { "PS384", NistObjectIdentifiers.IdSha384, 48 },
            new Object[] { "PS512", NistObjectIdentifiers.IdSha512, 64 },
        ];

        [TestCaseSource(nameof(matchingRestrictions))]
        public void A_key_restricted_to_the_hash_its_message_uses_verifies(String               Algorithm,
                                                                          DerObjectIdentifier  Hash,
                                                                          Int32                Salt)
        {

            var certificate = Issue("Meter PSS", PssKey(Restriction(Hash, Salt)));

            Assert.That(Verify(certificate, COSEAlgorithm.Parse(Algorithm), out var errorResponse),
                        Is.True, errorResponse);

        }

        #endregion

        #region A_key_restricted_to_another_hash_is_refused(Algorithm, Hash)

        private static readonly Object[] otherRestrictions = [
            new Object[] { "PS256", NistObjectIdentifiers.IdSha384, "SHA-384" },
            new Object[] { "PS256", NistObjectIdentifiers.IdSha512, "SHA-512" },
            new Object[] { "PS384", NistObjectIdentifiers.IdSha256, "SHA-256" },
            new Object[] { "PS512", NistObjectIdentifiers.IdSha384, "SHA-384" },
        ];

        [TestCaseSource(nameof(otherRestrictions))]
        public void A_key_restricted_to_another_hash_is_refused(String               Algorithm,
                                                               DerObjectIdentifier  Hash,
                                                               String               HashName)
        {

            var certificate = Issue("Meter PSS", PssKey(Restriction(Hash, 20)));

            // The signature itself is fine: it is the certificate that says
            // this key may not have made it.
            Assert.That(Verify(certificate, COSEAlgorithm.Parse(Algorithm), out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain($"restricts its key to RSASSA-PSS with {HashName}"));
            Assert.That(errorResponse,  Does.Contain($"'{Algorithm}'"));
            Assert.That(errorResponse,  Does.Contain("RFC 4055 Section 3.3"));

        }

        #endregion

        #region The_salt_of_a_key_is_a_floor_and_not_a_width()

        [Test]
        public void The_salt_of_a_key_is_a_floor_and_not_a_width()
        {

            // 20 is Section 3.1's DEFAULT, and every PS* signs with more.
            Assert.That(Verify(Issue("Meter PSS", PssKey(Restriction(NistObjectIdentifiers.IdSha256, 20))), COSEAlgorithm.PS256, out var errorResponse),
                        Is.True, errorResponse);

            Assert.That(Verify(Issue("Meter PSS", PssKey(Restriction(NistObjectIdentifiers.IdSha256, 32))), COSEAlgorithm.PS256, out errorResponse),
                        Is.True, errorResponse);

            // A floor PS256 does not reach.
            Assert.That(Verify(Issue("Meter PSS", PssKey(Restriction(NistObjectIdentifiers.IdSha256, 33))), COSEAlgorithm.PS256, out errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("a salt of at least 33 octets"));

        }

        #endregion

        #region A_key_masking_with_another_hash_is_refused()

        [Test]
        public void A_key_masking_with_another_hash_is_refused()
        {

            var certificate = Issue("Meter PSS", PssKey(Restriction(NistObjectIdentifiers.IdSha256, 32, MaskHash: NistObjectIdentifiers.IdSha384)));

            Assert.That(Verify(certificate, COSEAlgorithm.PS256, out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("MGF1 over SHA-384"));

        }

        #endregion

        #region A_key_with_another_trailer_field_is_refused()

        [Test]
        public void A_key_with_another_trailer_field_is_refused()
        {

            var certificate = Issue("Meter PSS", PssKey(Restriction(NistObjectIdentifiers.IdSha256, 32, Trailer: 2)));

            Assert.That(Verify(certificate, COSEAlgorithm.PS256, out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("the trailer field 2"));

        }

        #endregion

        #region A_key_whose_parameters_are_left_to_their_defaults_is_restricted_to_SHA1()

        [Test]
        public void A_key_whose_parameters_are_left_to_their_defaults_is_restricted_to_SHA1()
        {

            // An empty SEQUENCE is a parameter set, and every one of its fields
            // takes its DEFAULT: SHA-1, MGF1 with SHA-1, a salt of 20. No PS*
            // is that.
            var certificate = Issue("Meter PSS", PssKey(new RsassaPssParameters()));

            foreach (var algorithm in new[] { COSEAlgorithm.PS256, COSEAlgorithm.PS384, COSEAlgorithm.PS512 })
            {

                Assert.That(Verify(certificate, algorithm, out var errorResponse),
                            Is.False, algorithm.Name);

                Assert.That(errorResponse,  Does.Contain("with SHA-1"));

            }

        }

        #endregion

        #region A_key_without_parameters_restricts_nothing_but_the_scheme()

        [Test]
        public void A_key_without_parameters_restricts_nothing_but_the_scheme()
        {

            // Scenario 2 of RFC 4055 Section 3.3: no parameters, so no hash.
            var certificate = Issue("Meter PSS", PssKey(null));

            foreach (var algorithm in new[] { COSEAlgorithm.PS256, COSEAlgorithm.PS384, COSEAlgorithm.PS512 })
                Assert.That(Verify(certificate, algorithm, out var errorResponse),
                            Is.True, $"{algorithm.Name}: {errorResponse}");

        }

        #endregion

        #region The_scheme_is_enforced(Algorithm)

        private static readonly Object[] otherSchemes = [
            new Object[] { "RS256" },
            new Object[] { "RS384" },
            new Object[] { "RS512" },
        ];

        [TestCaseSource(nameof(otherSchemes))]
        public void The_scheme_is_enforced(String Algorithm)
        {

            // RFC 4055 states the restriction to RSASSA-PSS as what the key
            // owner wishes, and it is honoured: a PKCS #1 v1.5 message under
            // this key is refused, even with the very hash the key names. The
            // conformance suite asks the same in its row
            // a-key-restricted-to-the-other-rsa-scheme.
            var certificate = Issue("Meter PSS", PssKey(Restriction(NistObjectIdentifiers.IdSha256, 32)));

            Assert.That(Verify(certificate, COSEAlgorithm.Parse(Algorithm), out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("restricts its key to RSASSA-PSS"));
            Assert.That(errorResponse,  Does.Contain($"'{Algorithm}', which is not RSASSA-PSS"));

        }

        #endregion

        #region A_key_without_parameters_still_restricts_the_scheme()

        [Test]
        public void A_key_without_parameters_still_restricts_the_scheme()
        {

            // Scenario 2 names no hash, and still names the scheme. The
            // conformance suite asks this in its row
            // a-restriction-that-names-no-hash-with-the-other-scheme.
            var certificate = Issue("Meter PSS", PssKey(null));

            Assert.That(Verify(certificate, COSEAlgorithm.RS256, out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("'RS256', which is not RSASSA-PSS"));

        }

        #endregion

        #region An_rsaEncryption_key_is_not_restricted()

        [Test]
        public void An_rsaEncryption_key_is_not_restricted()
        {

            // The control: the same leaf key, written as rsaEncryption, which
            // says what the key is and nothing about what it is for.
            var certificate = Issue("Meter RSA", SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(leafKey.Public));

            foreach (var algorithm in new[] { COSEAlgorithm.PS256, COSEAlgorithm.PS384, COSEAlgorithm.PS512, COSEAlgorithm.RS256 })
                Assert.That(Verify(certificate, algorithm, out var errorResponse),
                            Is.True, $"{algorithm.Name}: {errorResponse}");

        }

        #endregion


        #region (private) ValidateUnder(Restriction, SignatureAlgorithm, out ErrorResponse)

        /// <summary>
        /// An authority whose key is written as id-RSASSA-PSS with the given
        /// parameters (or none, for null), issued by the root with
        /// sha256WithRSAEncryption, and a meter certificate the authority
        /// signs with the given algorithm. The chain of the two is validated
        /// against the root.
        /// </summary>
        private Boolean ValidateUnder(Asn1Encodable?   Restriction,
                                      String           SignatureAlgorithm,
                                      out String?      ErrorResponse)
        {

            var authority = Issue("PSS Authority", PssKey(Restriction, authorityKey), IsCA: true);

            var meter     = IssueBy("Meter",
                                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(leafKey.Public),
                                    "PSS Authority",
                                    authorityKey,
                                    SignatureAlgorithm);

            return new COSECertificateChain([ meter, authority ]).
                       Validate([ root ], out ErrorResponse);

        }

        #endregion

        #region An_authority_restricted_to_RSASSA_PSS_signs_no_certificate_in_the_other_scheme(Parameters)

        private static readonly Object?[] authorityRestrictions = [
            new Object?[] { null },
            new Object?[] { Restriction(NistObjectIdentifiers.IdSha256, 32) },
        ];

        [TestCaseSource(nameof(authorityRestrictions))]
        public void An_authority_restricted_to_RSASSA_PSS_signs_no_certificate_in_the_other_scheme(Asn1Encodable? Parameters)
        {

            // The restriction is the key's, and the key of an authority is
            // what signs certificates. Bouncy Castle verifies this signature
            // under the authority's two integers and would accept it.
            Assert.That(ValidateUnder(Parameters, "SHA256WITHRSA", out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("CN=Meter' is signed with"));
            Assert.That(errorResponse,  Does.Contain("which is not RSASSA-PSS, by 'CN=PSS Authority', which restricts its key to RSASSA-PSS [RFC 4055, Section 3.1]"));

        }

        #endregion

        #region An_authority_restricted_to_RSASSA_PSS_signs_with_it()

        [Test]
        public void An_authority_restricted_to_RSASSA_PSS_signs_with_it()
        {

            // Scenario 2 restricts the scheme and no hash.
            foreach (var algorithm in new[] { "SHA256WITHRSAANDMGF1", "SHA384WITHRSAANDMGF1", "SHA512WITHRSAANDMGF1" })
                Assert.That(ValidateUnder(null, algorithm, out var errorResponse),
                            Is.True, $"{algorithm}: {errorResponse}");

            // Scenario 3, with the parameters Bouncy Castle signs with for
            // SHA-256 and with Section 3.1's DEFAULT salt as the floor.
            foreach (var salt in new[] { 20, 32 })
                Assert.That(ValidateUnder(Restriction(NistObjectIdentifiers.IdSha256, salt), "SHA256WITHRSAANDMGF1", out var errorResponse),
                            Is.True, $"salt {salt}: {errorResponse}");

        }

        #endregion

        #region An_authority_restricted_by_its_parameters_signs_with_them_only()

        [Test]
        public void An_authority_restricted_by_its_parameters_signs_with_them_only()
        {

            // Another hash.
            Assert.That(ValidateUnder(Restriction(NistObjectIdentifiers.IdSha256, 32), "SHA384WITHRSAANDMGF1", out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("restricts its key to RSASSA-PSS with SHA-256, and the certificate is signed over SHA-384"));

            // A salt floor above the 32 octets Bouncy Castle signs with.
            Assert.That(ValidateUnder(Restriction(NistObjectIdentifiers.IdSha256, 33), "SHA256WITHRSAANDMGF1", out errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("a salt of at least 33 octets, and the certificate is signed with a salt of 32"));

            // Another mask.
            Assert.That(ValidateUnder(Restriction(NistObjectIdentifiers.IdSha256, 32, MaskHash: NistObjectIdentifiers.IdSha384), "SHA256WITHRSAANDMGF1", out errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("with MGF1 over SHA-384, and the certificate is masked with MGF1 over SHA-256"));

        }

        #endregion

        #region A_trust_anchor_restricted_to_RSASSA_PSS_is_held_to_it()

        [Test]
        public void A_trust_anchor_restricted_to_RSASSA_PSS_is_held_to_it()
        {

            // An anchor is trusted to be who it is, not to sign however it
            // likes: the last certificate of a chain is checked against it
            // like against any other issuer.
            var anchor  = Issue("PSS Authority", PssKey(null, authorityKey), IsCA: true);

            var meter   = IssueBy("Meter",
                                  SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(leafKey.Public),
                                  "PSS Authority",
                                  authorityKey,
                                  "SHA256WITHRSA");

            Assert.That(new COSECertificateChain([ meter ]).Validate([ anchor ], out var errorResponse),
                        Is.False);

            Assert.That(errorResponse,  Does.Contain("restricts its key to RSASSA-PSS [RFC 4055, Section 3.1]"));

        }

        #endregion

        #region An_authority_with_an_rsaEncryption_key_signs_in_either_scheme()

        [Test]
        public void An_authority_with_an_rsaEncryption_key_signs_in_either_scheme()
        {

            // The control: the same authority key, written as rsaEncryption.
            var authority = Issue("RSA Authority", SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(authorityKey.Public), IsCA: true);

            foreach (var algorithm in new[] { "SHA256WITHRSA", "SHA256WITHRSAANDMGF1", "SHA384WITHRSAANDMGF1" })
            {

                var meter = IssueBy("Meter",
                                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(leafKey.Public),
                                    "RSA Authority",
                                    authorityKey,
                                    algorithm);

                Assert.That(new COSECertificateChain([ meter, authority ]).Validate([ root ], out var errorResponse),
                            Is.True, $"{algorithm}: {errorResponse}");

            }

        }

        #endregion

    }

}
