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

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// X.509 certificate chains within COSE messages [RFC 9360].
    ///
    /// The question a chain answers is not "did this key sign" - a bare
    /// public key answers that - but "why should I believe this key belongs
    /// to that meter". It is only an answer once two things hold together:
    /// the chain leads to an anchor the recipient decided to trust, AND the
    /// key of its end-entity certificate is the key that verified the
    /// signature. Either half alone proves nothing, which is why they are
    /// never checked apart here.
    /// </summary>
    [TestFixture]
    public class COSECertificateChainTests
    {

        #region Data

        private static readonly Byte[] payload = "This is the content.".ToUTF8Bytes();

        private static Int64 serialNumber;

        #endregion

        #region (private static) NewKey()

        private static ECPrivateKeyParameters NewKey()

            => (ECPrivateKeyParameters) Crypto.GenerateKeys(ECNamedCurveTable.GetByName("secp256r1")).Private;

        #endregion

        #region (private static) Issue(Subject, PublicKeyOf, IsCA, Issuer = null, ...)

        /// <summary>
        /// Issue a certificate, self-signed when no issuer is given.
        /// </summary>
        private static X509Certificate Issue(String                                                   Subject,
                                             ECPrivateKeyParameters                                   PublicKeyOf,
                                             Boolean                                                  IsCA,
                                             (X509Certificate Certificate, ECPrivateKeyParameters Key)?  Issuer         = null,
                                             DateTime?                                                NotBefore      = null,
                                             DateTime?                                                NotAfter       = null,
                                             Boolean                                                  AllowSigning   = true)
        {

            var generator = new X509V3CertificateGenerator();

            generator.SetSerialNumber(BigInteger.ValueOf(Interlocked.Increment(ref serialNumber)));
            generator.SetSubjectDN   (new X509Name($"CN={Subject}"));
            generator.SetIssuerDN    (new X509Name($"CN={(Issuer.HasValue ? Issuer.Value.Certificate.SubjectDN.GetValueList(X509Name.CN)[0] : Subject)}"));
            generator.SetNotBefore   (NotBefore ?? DateTime.UtcNow.AddDays(-1));
            generator.SetNotAfter    (NotAfter  ?? DateTime.UtcNow.AddDays(365));
            generator.SetPublicKey   (Crypto.CalculatePublicKey(PublicKeyOf));

            generator.AddExtension(X509Extensions.BasicConstraints,
                                   true,
                                   new BasicConstraints(IsCA));

            generator.AddExtension(X509Extensions.KeyUsage,
                                   true,
                                   new KeyUsage(IsCA
                                                    ? KeyUsage.KeyCertSign | KeyUsage.DigitalSignature
                                                    : AllowSigning
                                                          ? KeyUsage.DigitalSignature
                                                          : KeyUsage.KeyEncipherment));

            return generator.Generate(
                       new Asn1SignatureFactory("SHA256withECDSA",
                                                Issuer.HasValue
                                                    ? Issuer.Value.Key
                                                    : PublicKeyOf)
                   );

        }

        #endregion

        #region (private static) BuildPKI()

        /// <summary>
        /// A root, an intermediate and an end-entity certificate - the shape
        /// a meter key would actually arrive in.
        /// </summary>
        private static (X509Certificate Root,
                        (X509Certificate Certificate, ECPrivateKeyParameters Key) Intermediate,
                        (X509Certificate Certificate, ECPrivateKeyParameters Key) Leaf,
                        ECPrivateKeyParameters RootKey) BuildPKI(String Name = "Test")
        {

            // A certification authority outlives what it certifies, here as
            // in the field: otherwise a record could not be checked as of the
            // time it was made.
            var authorityFrom    = DateTime.UtcNow.AddYears(-5);
            var authorityUntil   = DateTime.UtcNow.AddYears( 5);

            var rootKey          = NewKey();
            var root             = Issue($"{Name} Root CA", rootKey, true, null, authorityFrom, authorityUntil);

            var intermediateKey  = NewKey();
            var intermediate     = Issue($"{Name} Manufacturer CA", intermediateKey, true, (root, rootKey), authorityFrom, authorityUntil);

            var leafKey          = NewKey();
            var leaf             = Issue("Meter 1ISA0000000042", leafKey, false, (intermediate, intermediateKey));

            return (root, (intermediate, intermediateKey), (leaf, leafKey), rootKey);

        }

        #endregion

        #region (private static) SignWithChain(LeafKey, Certificates, Critical = false)

        private static COSESign1 SignWithChain(ECPrivateKeyParameters  LeafKey,
                                               X509Certificate[]       Certificates,
                                               Boolean                 Critical   = false)
        {

            var chain      = new COSECertificateChain(Certificates);

            var parameters = new List<(CBORValue, CBORValue)> {
                                 (COSEHeaderLabel.Algorithm, COSEAlgorithm.ES256.ToCBOR())
                             };

            if (Critical)
                parameters.Add((COSEHeaderLabel.Critical, CBORValue.FromArray(COSEHeaderLabel.X5Chain)));

            parameters.Add((COSEHeaderLabel.X5Chain, chain.ToCBOR()));

            return COSESign1.Sign(payload,
                                  LeafKey,
                                  new COSEHeaders([.. parameters]));

        }

        #endregion


        #region A_chain_that_leads_to_a_trusted_root_identifies_the_signer()

        [Test]
        public void A_chain_that_leads_to_a_trusted_root_identifies_the_signer()
        {

            var (root, intermediate, leaf, _) = BuildPKI();

            var signed    = SignWithChain(leaf.Key, [leaf.Certificate, intermediate.Certificate]);
            var received  = COSESign1.Parse(signed.ToByteArray());

            Assert.That(received.CertificateChain,                    Is.Not.Null);
            Assert.That(received.CertificateChain!.Certificates.Count,  Is.EqualTo(2));

            Assert.That(received.VerifyWithCertificateChain([root], out var signer, out var errorResponse),
                        Is.True, errorResponse);

            // The message now names WHO signed it, not merely that someone with
            // a certain key did.
            Assert.That(signer!.SubjectDN.ToString(),  Is.EqualTo("CN=Meter 1ISA0000000042"));

        }

        #endregion

        #region A_chain_to_an_unknown_root_is_refused()

        [Test]
        public void A_chain_to_an_unknown_root_is_refused()
        {

            var (_, intermediate, leaf, _)  = BuildPKI();
            var (otherRoot, _, _, _)        = BuildPKI("Foreign");

            var signed = SignWithChain(leaf.Key, [leaf.Certificate, intermediate.Certificate]);

            // The signature is perfectly valid - and worth nothing, because
            // nothing connects it to anyone this recipient trusts.
            Assert.That(signed.Verify(Crypto.CalculatePublicKey(leaf.Key), out _),  Is.True);

            Assert.That(signed.VerifyWithCertificateChain([otherRoot], out var signer, out var errorResponse),  Is.False);
            Assert.That(signer,         Is.Null);
            Assert.That(errorResponse,  Does.Contain("neither a trust anchor nor issued by one"));

        }

        #endregion

        #region A_chain_certifying_another_key_than_the_one_that_signed_is_refused()

        [Test]
        public void A_chain_certifying_another_key_than_the_one_that_signed_is_refused()
        {

            var (root, intermediate, _, _) = BuildPKI();

            // Two meters of the same manufacturer: both certificates are
            // genuine and both chain to the trusted root.
            var honestKey    = NewKey();
            var honest       = Issue("Meter A", honestKey, false, intermediate);

            var attackerKey  = NewKey();
            var attacker     = Issue("Meter B", attackerKey, false, intermediate);

            // The attacker signs with its own key but presents the honest
            // meter's certificate, hoping the reading will be attributed there.
            var forged       = COSESign1.Sign(
                                   payload,
                                   attackerKey,
                                   new COSEHeaders(
                                       (COSEHeaderLabel.Algorithm, COSEAlgorithm.ES256.ToCBOR()),
                                       (COSEHeaderLabel.X5Chain,   new COSECertificateChain([honest, intermediate.Certificate]).ToCBOR())
                                   )
                               );

            // The chain alone validates beautifully...
            Assert.That(forged.CertificateChain!.Validate([root], out var chainError),  Is.True, chainError);

            // ...and the whole message is refused nevertheless, because the
            // certified key is not the key that signed. This binding is the
            // entire point of the exercise.
            Assert.That(forged.VerifyWithCertificateChain([root], out var signer, out var errorResponse),  Is.False);
            Assert.That(signer,         Is.Null);
            Assert.That(errorResponse,  Is.EqualTo("The signature is invalid!"));

        }

        #endregion

        #region An_expired_certificate_is_refused()

        [Test]
        public void An_expired_certificate_is_refused()
        {

            var (root, intermediate, _, _) = BuildPKI();

            var leafKey  = NewKey();
            var expired  = Issue("Retired meter",
                                 leafKey,
                                 false,
                                 intermediate,
                                 DateTime.UtcNow.AddDays(-30),
                                 DateTime.UtcNow.AddDays(-1));

            var signed   = SignWithChain(leafKey, [expired, intermediate.Certificate]);

            Assert.That(signed.VerifyWithCertificateChain([root], out _, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Does.Contain("is not valid at"));

            // ...but it was valid while it was valid, and a record signed back
            // then can still be checked as of the time it was made.
            Assert.That(signed.VerifyWithCertificateChain([root],
                                                          out var signer,
                                                          out var pastError,
                                                          null,
                                                          null,
                                                          null,
                                                          DateTimeOffset.UtcNow.AddDays(-15)),
                        Is.True, pastError);

            Assert.That(signer!.SubjectDN.ToString(),  Is.EqualTo("CN=Retired meter"));

        }

        #endregion

        #region A_certificate_that_may_not_sign_or_may_not_issue_is_refused()

        [Test]
        public void A_certificate_that_may_not_sign_or_may_not_issue_is_refused()
        {

            var (root, intermediate, _, _) = BuildPKI();

            // An end-entity certificate whose key usage does not allow signing.
            var encipherKey   = NewKey();
            var encipherOnly  = Issue("Encryption only", encipherKey, false, intermediate, AllowSigning: false);

            Assert.That(SignWithChain(encipherKey, [encipherOnly, intermediate.Certificate]).
                            VerifyWithCertificateChain([root], out _, out var usageError),  Is.False);

            Assert.That(usageError,  Does.Contain("not allowed to create digital signatures"));

            // An "intermediate" that is not a certification authority at all.
            var notACAKey  = NewKey();
            var notACA     = Issue("Not a CA", notACAKey, false, intermediate);

            var leafKey    = NewKey();
            var leaf       = Issue("Meter behind a non-CA", leafKey, false, (notACA, notACAKey));

            Assert.That(SignWithChain(leafKey, [leaf, notACA, intermediate.Certificate]).
                            VerifyWithCertificateChain([root], out _, out var caError),  Is.False);

            Assert.That(caError,  Does.Contain("not a certification authority"));

        }

        #endregion

        #region A_broken_chain_is_refused()

        [Test]
        public void A_broken_chain_is_refused()
        {

            var (root, intermediate, leaf, _)  = BuildPKI();
            var (_, otherIntermediate, _, _)   = BuildPKI("Foreign");

            // The leaf was not issued by this intermediate.
            var signed = SignWithChain(leaf.Key, [leaf.Certificate, otherIntermediate.Certificate]);

            Assert.That(signed.VerifyWithCertificateChain([root], out _, out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Does.Contain("which is not the subject of"));

            // A message without any chain at all.
            var bare = COSESign1.Sign(payload, leaf.Key, COSEAlgorithm.ES256);

            Assert.That(bare.VerifyWithCertificateChain([root], out _, out var withoutChain),  Is.False);
            Assert.That(withoutChain,  Is.EqualTo("This COSE_Sign1 message carries no certificate chain!"));

        }

        #endregion

        #region The_thumbprint_has_to_name_the_certificate_that_travelled()

        [Test]
        public void The_thumbprint_has_to_name_the_certificate_that_travelled()
        {

            var (root, intermediate, leaf, _) = BuildPKI();

            var honest = COSESign1.Sign(
                             payload,
                             leaf.Key,
                             new COSEHeaders(
                                 (COSEHeaderLabel.Algorithm, COSEAlgorithm.ES256.ToCBOR()),
                                 (COSEHeaderLabel.X5Chain,   new COSECertificateChain([leaf.Certificate, intermediate.Certificate]).ToCBOR()),
                                 (COSEHeaderLabel.X5T,       COSECertificateHash.From(leaf.Certificate).ToCBOR())
                             )
                         );

            Assert.That(honest.CertificateThumbprint,             Is.Not.Null);
            Assert.That(honest.CertificateThumbprint!.Algorithm,  Is.EqualTo(COSEAlgorithm.SHA256));
            Assert.That(honest.CertificateThumbprint!.Value.Length,  Is.EqualTo(32));

            Assert.That(honest.VerifyWithCertificateChain([root], out _, out var errorResponse),  Is.True, errorResponse);

            // A thumbprint of a different certificate contradicts the chain.
            var contradictory = COSESign1.Sign(
                                    payload,
                                    leaf.Key,
                                    new COSEHeaders(
                                        (COSEHeaderLabel.Algorithm, COSEAlgorithm.ES256.ToCBOR()),
                                        (COSEHeaderLabel.X5Chain,   new COSECertificateChain([leaf.Certificate, intermediate.Certificate]).ToCBOR()),
                                        (COSEHeaderLabel.X5T,       COSECertificateHash.From(intermediate.Certificate).ToCBOR())
                                    )
                                );

            Assert.That(contradictory.VerifyWithCertificateChain([root], out _, out var thumbprintError),  Is.False);
            Assert.That(thumbprintError,  Is.EqualTo("The certificate thumbprint does not belong to the given certificate!"));

        }

        #endregion

        #region A_single_certificate_is_a_bare_byte_string()

        [Test]
        public void A_single_certificate_is_a_bare_byte_string()
        {

            var rootKey  = NewKey();
            var root     = Issue("Self-signed meter", rootKey, true);

            var chain    = new COSECertificateChain([root]);

            // RFC 9360 spells the array as 2*certs: one certificate travels bare.
            Assert.That(chain.ToCBOR().Kind,  Is.EqualTo(CBORValueKind.ByteString));

            Assert.That(COSECertificateChain.TryParse(chain.ToCBOR(), out var reparsed, out var errorResponse),  Is.True, errorResponse);
            Assert.That(reparsed!.Certificates.Count,  Is.EqualTo(1));
            Assert.That(reparsed!.EndEntity.SubjectDN.ToString(),  Is.EqualTo("CN=Self-signed meter"));

            // ...and an array of one is malformed, not merely unusual.
            Assert.That(COSECertificateChain.TryParse(CBORValue.FromArray(CBORValue.FromBytes(root.GetEncoded())),
                                                      out _,
                                                      out var arrayError),  Is.False);

            Assert.That(arrayError,  Does.Contain("must be a bare byte string, not an array"));

        }

        #endregion

        #region A_sender_may_now_demand_that_the_chain_be_processed()

        [Test]
        public void A_sender_may_now_demand_that_the_chain_be_processed()
        {

            var (root, intermediate, leaf, _) = BuildPKI();

            var signed = SignWithChain(leaf.Key,
                                       [leaf.Certificate, intermediate.Certificate],
                                       Critical: true);

            // Whoever verifies with a bare public key has NOT looked at the
            // certificates, and the sender said that is not good enough.
            Assert.That(signed.Verify(Crypto.CalculatePublicKey(leaf.Key), out var critError),  Is.False);
            Assert.That(critError,  Is.EqualTo("The \"crit\" header parameter demands that 'x5chain' be understood, which this implementation does not!"));

            // Whoever validates the chain has, and is answered.
            Assert.That(signed.VerifyWithCertificateChain([root], out var signer, out var errorResponse),  Is.True, errorResponse);
            Assert.That(signer!.SubjectDN.ToString(),  Is.EqualTo("CN=Meter 1ISA0000000042"));

        }

        #endregion

        #region A_chain_can_not_be_validated_without_an_anchor()

        [Test]
        public void A_chain_can_not_be_validated_without_an_anchor()
        {

            var (_, intermediate, leaf, _) = BuildPKI();

            var chain = new COSECertificateChain([leaf.Certificate, intermediate.Certificate]);

            Assert.That(chain.Validate([], out var errorResponse),  Is.False);
            Assert.That(errorResponse,  Does.Contain("without at least one trust anchor"));

        }

        #endregion

    }

}
