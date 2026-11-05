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

using System.Diagnostics.CodeAnalysis;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A COSE_X509 [RFC 9360, Section 2]: a chain of X.509 certificates, as
    /// carried by the "x5chain" header parameter.
    ///
    /// <code>
    /// COSE_X509 = bstr / [ 2*certs: bstr ]
    /// </code>
    ///
    /// A single certificate is a bare byte string; two or more form an array.
    /// The order is end-entity first, then the certificate that signed it,
    /// and so on. The trust anchor need not be present - the recipient is
    /// expected to have it already, which is the whole point of it being an
    /// anchor.
    ///
    /// A chain proves nothing on its own. It is untrusted input like the rest
    /// of the message, and it only becomes an answer once Validate has walked
    /// it to an anchor the recipient configured - and once the key of its
    /// end-entity certificate turns out to be the key that verified the
    /// signature, which is what
    /// COSESign1.VerifyWithCertificateChain(...) does.
    /// </summary>
    public sealed class COSECertificateChain
    {

        #region Data

        private readonly X509Certificate[] certificates;

        #endregion

        #region Properties

        /// <summary>
        /// The certificates, end-entity first.
        /// </summary>
        public IReadOnlyList<X509Certificate>  Certificates

            => certificates;

        /// <summary>
        /// The end-entity certificate, i.e. the one holding the key that
        /// is expected to have signed.
        /// </summary>
        public X509Certificate                 EndEntity

            => certificates[0];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new certificate chain.
        /// </summary>
        /// <param name="Certificates">The certificates, end-entity first.</param>
        public COSECertificateChain(IEnumerable<X509Certificate> Certificates)
        {

            this.certificates = Certificates.ToArray();

            if (this.certificates.Length == 0)
                throw new COSEException("A certificate chain must hold at least one certificate!");

        }

        #endregion


        #region (static) TryParse(CBOR, out Chain, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR value as a COSE_X509.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a certificate chain.</param>
        /// <param name="Chain">The parsed certificate chain.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                                     CBOR,
                                       [NotNullWhen(true)]  out COSECertificateChain?  Chain,
                                       [NotNullWhen(false)] out String?                ErrorResponse)
        {

            Chain = null;

            var encoded = new List<Byte[]>();

            if (CBOR.TryGetBytes(out var single))
                encoded.Add(single);

            else if (CBOR.Kind == CBORValueKind.Array)
            {

                // RFC 9360 spells the array as "2*certs": a chain of exactly
                // one certificate is a bare byte string, never an array of one.
                if (CBOR.Count < 2)
                {
                    ErrorResponse = "A COSE_X509 of fewer than two certificates must be a bare byte string, not an array!";
                    return false;
                }

                foreach (var item in CBOR.AsArray())
                {

                    if (!item.TryGetBytes(out var certificate))
                    {
                        ErrorResponse = "Every certificate of a COSE_X509 must be a byte string!";
                        return false;
                    }

                    encoded.Add(certificate);

                }

            }

            else
            {
                ErrorResponse = $"A COSE_X509 must be a byte string or an array of them, but was a CBOR {CBOR.Kind}!";
                return false;
            }

            try
            {

                var parser  = new X509CertificateParser();
                var certs   = new List<X509Certificate>();

                for (var position = 0; position < encoded.Count; position++)
                {

                    var certificate = parser.ReadCertificate(encoded[position]);

                    // RFC 5280 requires a certificate to be DER, and RFC 9360
                    // takes x5t over that DER encoding. Bouncy Castle also reads
                    // BER, stops after the first certificate, and GetEncoded()
                    // encodes anew rather than returning what was read - so a
                    // thumbprint would cover bytes nobody sent. Only a
                    // certificate whose encoding is the bytes it travelled as is
                    // accepted, and then what x5t hashes is what travelled.
                    if (certificate is null ||
                        !certificate.GetEncoded().SequenceEqual(encoded[position]))
                    {
                        ErrorResponse = $"The certificate at position {position} of the COSE_X509 is not a single DER-encoded certificate!";
                        return false;
                    }

                    if (!HasPKCS1SignatureParameters(certificate, out var signatureAlgorithm))
                    {
                        ErrorResponse = $"The certificate at position {position} of the COSE_X509 says it was signed with '{signatureAlgorithm}' and carries parameters beside it, where RFC 4055 Section 5 allows a NULL or nothing at all!";
                        return false;
                    }

                    certs.Add(certificate);

                }

                Chain          = new COSECertificateChain(certs);

                ErrorResponse  = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = $"A certificate of the COSE_X509 could not be read: {e.Message}";
                return false;
            }

        }

        #endregion

        #region (private static) HasPKCS1SignatureParameters(Certificate, out SignatureAlgorithm)

        /// <summary>
        /// The RSASSA-PKCS1-v1_5 signature algorithms, whose parameters are a
        /// NULL [RFC 3279, Section 2.2.1; RFC 4055, Section 5].
        /// </summary>
        private static readonly HashSet<DerObjectIdentifier> pkcs1SignatureAlgorithms = [
            PkcsObjectIdentifiers.MD5WithRsaEncryption,
            PkcsObjectIdentifiers.Sha1WithRsaEncryption,
            PkcsObjectIdentifiers.Sha224WithRsaEncryption,
            PkcsObjectIdentifiers.Sha256WithRsaEncryption,
            PkcsObjectIdentifiers.Sha384WithRsaEncryption,
            PkcsObjectIdentifiers.Sha512WithRsaEncryption
        ];

        /// <summary>
        /// Whether a certificate signed with RSASSA-PKCS1-v1_5 writes its
        /// signature algorithm as RFC 4055 Section 5 says: "When any of these
        /// four object identifiers appears within an AlgorithmIdentifier, the
        /// parameters MUST be NULL. Implementations MUST accept the
        /// parameters being absent as well as present." Anything else beside
        /// such an identifier is not a valid certificate.
        ///
        /// Bouncy Castle does not ask this. It compares the parameters of the
        /// two copies of the signature algorithm, the one within the signed
        /// body and the one beside it, but it accepts any parameters both
        /// copies share, because it has no use for them. Both copies are
        /// checked here, so that this answer does not depend on that
        /// comparison happening later. A certificate signed with any other
        /// algorithm passes this method unchanged.
        /// </summary>
        /// <param name="Certificate">The certificate.</param>
        /// <param name="SignatureAlgorithm">The identifier of its signature algorithm.</param>
        private static Boolean HasPKCS1SignatureParameters(X509Certificate                  Certificate,
                                                           out DerObjectIdentifier          SignatureAlgorithm)
        {

            var outer           = Certificate.CertificateStructure.SignatureAlgorithm;
            var inner           = Certificate.CertificateStructure.TbsCertificate.Signature;

            SignatureAlgorithm  = outer.Algorithm;

            static Boolean NullOrAbsent(AlgorithmIdentifier Identifier)

                => Identifier.Parameters is null ||
                   Identifier.Parameters.ToAsn1Object() is Asn1Null;

            return (!pkcs1SignatureAlgorithms.Contains(outer.Algorithm) || NullOrAbsent(outer)) &&
                   (!pkcs1SignatureAlgorithms.Contains(inner.Algorithm) || NullOrAbsent(inner));

        }

        #endregion

        #region Validate(TrustAnchors, out ErrorResponse, At = null)

        /// <summary>
        /// Validate this chain against the given trust anchors: every
        /// certificate signed by the next one, the last one signed by an
        /// anchor or being one, everything within its validity period, every
        /// issuing certificate actually allowed to issue, and the end-entity
        /// certificate allowed to sign.
        ///
        /// This answers "does this chain lead somewhere I trust". It does NOT
        /// answer whether the key of the end-entity certificate is the key
        /// that signed the message - that binding is made by
        /// COSESign1.VerifyWithCertificateChain(...), and without it a valid
        /// chain says nothing about the signature it travelled with.
        ///
        /// Revocation is not checked. Neither are name constraints, policies
        /// or path length limits beyond the CA flag.
        /// </summary>
        /// <param name="TrustAnchors">The certificates the recipient trusts a priori.</param>
        /// <param name="ErrorResponse">The reason why the chain was rejected.</param>
        /// <param name="At">The point in time to validate at, now by default.</param>
        public Boolean Validate(IEnumerable<X509Certificate>      TrustAnchors,
                                [NotNullWhen(false)] out String?  ErrorResponse,
                                DateTimeOffset?                   At   = null)
        {

            var anchors  = TrustAnchors.ToArray();
            var at       = (At ?? DateTimeOffset.UtcNow).UtcDateTime;

            if (anchors.Length == 0)
            {
                ErrorResponse = "A certificate chain can not be validated without at least one trust anchor!";
                return false;
            }

            #region Every certificate within its validity period

            foreach (var certificate in certificates)
            {
                if (!IsValidAt(certificate, at, out ErrorResponse))
                    return false;
            }

            #endregion

            #region Every certificate signed by the next one

            for (var i = 0; i < certificates.Length - 1; i++)
            {

                if (!IsIssuedBy(certificates[i], certificates[i + 1], out ErrorResponse))
                    return false;

                if (!MayIssue(certificates[i + 1], out ErrorResponse))
                    return false;

            }

            #endregion

            #region ...and the last one anchored

            var last = certificates[^1];

            if (!anchors.Any(anchor => anchor.GetEncoded().SequenceEqual(last.GetEncoded())))
            {

                var issuer = anchors.FirstOrDefault(anchor => anchor.SubjectDN.Equivalent(last.IssuerDN));

                if (issuer is null)
                {
                    ErrorResponse = $"The certificate chain ends at '{last.SubjectDN}', which is neither a trust anchor nor issued by one!";
                    return false;
                }

                if (!IsValidAt(issuer, at, out ErrorResponse) ||
                    !MayIssue (issuer,     out ErrorResponse) ||
                    !IsIssuedBy(last, issuer, out ErrorResponse))
                {
                    return false;
                }

            }

            #endregion

            #region The end-entity certificate allowed to sign

            var keyUsage = EndEntity.GetKeyUsage();

            // Index 0 is digitalSignature. An absent extension means
            // unrestricted, which is why the null check comes first.
            if (keyUsage is not null && keyUsage.Length > 0 && !keyUsage[0])
            {
                ErrorResponse = $"The end-entity certificate '{EndEntity.SubjectDN}' is not allowed to create digital signatures!";
                return false;
            }

            #endregion

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region (private static) IsValidAt (Certificate, At, out ErrorResponse)

        private static Boolean IsValidAt(X509Certificate                   Certificate,
                                         DateTime                          At,
                                         [NotNullWhen(false)] out String?  ErrorResponse)
        {

            try
            {
                Certificate.CheckValidity(At);
                ErrorResponse = null;
                return true;
            }
            catch (Exception e)
            {
                ErrorResponse = $"The certificate '{Certificate.SubjectDN}' is not valid at {At:yyyy-MM-ddTHH:mm:ssZ}: {e.Message}";
                return false;
            }

        }

        #endregion

        #region (private static) IsIssuedBy(Certificate, Issuer, out ErrorResponse)

        private static Boolean IsIssuedBy(X509Certificate                   Certificate,
                                          X509Certificate                   Issuer,
                                          [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (!Certificate.IssuerDN.Equivalent(Issuer.SubjectDN))
            {
                ErrorResponse = $"The certificate '{Certificate.SubjectDN}' names the issuer '{Certificate.IssuerDN}', which is not the subject of '{Issuer.SubjectDN}'!";
                return false;
            }

            if (!IssuerKeyPermits(Certificate, Issuer, out ErrorResponse))
                return false;

            try
            {
                Certificate.Verify(Issuer.GetPublicKey());
                ErrorResponse = null;
                return true;
            }
            catch (Exception e)
            {
                ErrorResponse = $"The certificate '{Certificate.SubjectDN}' was not signed by '{Issuer.SubjectDN}': {e.Message}";
                return false;
            }

        }

        #endregion

        #region (private static) IssuerKeyPermits(Certificate, Issuer, out ErrorResponse)

        /// <summary>
        /// Whether the key of the issuer may have signed the certificate, as
        /// far as the issuer's own certificate restricts that key.
        ///
        /// It is the restriction PermitsAlgorithm reads for a message, and it
        /// is the key's rather than the message's: an authority whose key is
        /// written as id-RSASSA-PSS has limited it "exclusively to RSASSA-PSS"
        /// [RFC 4055, Section 3.1], and the certificates it signs are no
        /// exception. Where its parameters are present, scenario 3 of Section
        /// 3.3 applies as written, because the signature of a certificate
        /// carries RSASSA-PSS-params of its own: "all parameters in the
        /// signature structure algorithm identifier MUST match the parameters
        /// in the key structure algorithm identifier except the saltLength
        /// field", which "MUST be greater or equal to that in the key
        /// parameters field".
        ///
        /// Bouncy Castle's Verify takes the issuer's key as two integers and
        /// never sees the identifier it was written with, so it is read here.
        /// A trust anchor is held to it like any other issuer, because the
        /// last certificate of a chain is checked against it by IsIssuedBy.
        /// </summary>
        private static Boolean IssuerKeyPermits(X509Certificate                   Certificate,
                                                X509Certificate                   Issuer,
                                                [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ErrorResponse           = null;

            var keyAlgorithm        = Issuer.SubjectPublicKeyInfo.Algorithm;

            if (!keyAlgorithm.Algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss))
                return true;

            var signatureAlgorithm  = Certificate.CertificateStructure.SignatureAlgorithm;

            // The scheme, whatever the parameters say.
            if (!signatureAlgorithm.Algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss))
            {
                ErrorResponse = $"The certificate '{Certificate.SubjectDN}' is signed with '{Certificate.SigAlgName}', which is not RSASSA-PSS, by '{Issuer.SubjectDN}', which restricts its key to RSASSA-PSS [RFC 4055, Section 3.1]!";
                return false;
            }

            if (keyAlgorithm.Parameters is null)
                return true;

            var restricts = $"The certificate '{Certificate.SubjectDN}' is signed by '{Issuer.SubjectDN}', which restricts its key to RSASSA-PSS";

            try
            {

                var key        = RsassaPssParameters.GetInstance(keyAlgorithm.Parameters);

                // Absent parameters beside id-RSASSA-PSS are every DEFAULT.
                var signature  = signatureAlgorithm.Parameters is null
                                     ? new RsassaPssParameters()
                                     : RsassaPssParameters.GetInstance(signatureAlgorithm.Parameters);

                var keyHash        = key.      HashAlgorithm.Algorithm;
                var signatureHash  = signature.HashAlgorithm.Algorithm;

                if (!keyHash.Equals(signatureHash))
                {
                    ErrorResponse = $"{restricts} with {HashName(keyHash)}, and the certificate is signed over {HashName(signatureHash)}: RFC 4055 Section 3.3 requires the parameters of the signature to be the key's!";
                    return false;
                }

                var keyMask        = key.      MaskGenAlgorithm.Algorithm;
                var signatureMask  = signature.MaskGenAlgorithm.Algorithm;

                if (!keyMask.Equals(signatureMask))
                {
                    ErrorResponse = $"{restricts} with the mask generation function '{keyMask}', and the certificate is signed with '{signatureMask}': RFC 4055 Section 3.3 requires the parameters of the signature to be the key's!";
                    return false;
                }

                if (keyMask.Equals(PkcsObjectIdentifiers.IdMgf1))
                {

                    var keyMaskHash        = AlgorithmIdentifier.GetInstance(key.      MaskGenAlgorithm.Parameters).Algorithm;
                    var signatureMaskHash  = AlgorithmIdentifier.GetInstance(signature.MaskGenAlgorithm.Parameters).Algorithm;

                    if (!keyMaskHash.Equals(signatureMaskHash))
                    {
                        ErrorResponse = $"{restricts} with MGF1 over {HashName(keyMaskHash)}, and the certificate is masked with MGF1 over {HashName(signatureMaskHash)}: RFC 4055 Section 3.3 requires the parameters of the signature to be the key's!";
                        return false;
                    }

                }

                var keyTrailer        = key.      TrailerField.IntValueExact;
                var signatureTrailer  = signature.TrailerField.IntValueExact;

                if (keyTrailer != signatureTrailer)
                {
                    ErrorResponse = $"{restricts} with the trailer field {keyTrailer}, and the certificate is signed with {signatureTrailer}: RFC 4055 Section 3.3 requires the parameters of the signature to be the key's!";
                    return false;
                }

                // The one field a key states differently from a signature: a
                // floor, not the width that was used.
                var keySalt        = key.      SaltLength.IntValueExact;
                var signatureSalt  = signature.SaltLength.IntValueExact;

                if (signatureSalt < keySalt)
                {
                    ErrorResponse = $"{restricts} with a salt of at least {keySalt} octets, and the certificate is signed with a salt of {signatureSalt} [RFC 4055, Section 3.3]!";
                    return false;
                }

            }
            catch (Exception e) when (e is ArgumentException or InvalidCastException or ArithmeticException)
            {
                ErrorResponse = $"{restricts} with parameters that are not RSASSA-PSS-params, or the certificate is signed with such [RFC 4055, Section 3.1]: {e.Message}";
                return false;
            }

            return true;

        }

        #endregion

        #region (private static) MayIssue  (Certificate, out ErrorResponse)

        private static Boolean MayIssue(X509Certificate                   Certificate,
                                        [NotNullWhen(false)] out String?  ErrorResponse)
        {

            // -1 whenever the basic constraints say this is not a CA.
            if (Certificate.GetBasicConstraints() < 0)
            {
                ErrorResponse = $"The certificate '{Certificate.SubjectDN}' issued another one although it is not a certification authority!";
                return false;
            }

            var keyUsage = Certificate.GetKeyUsage();

            // Index 5 is keyCertSign.
            if (keyUsage is not null && keyUsage.Length > 5 && !keyUsage[5])
            {
                ErrorResponse = $"The certificate '{Certificate.SubjectDN}' issued another one although it is not allowed to sign certificates!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion


        #region ToCBOR()

        /// <summary>
        /// Return a CBOR representation of this certificate chain: a bare
        /// byte string for a single certificate, an array for several.
        /// </summary>
        public CBORValue ToCBOR()

            => certificates.Length == 1
                   ? CBORValue.FromBytes(certificates[0].GetEncoded())
                   : CBORValue.FromArray(certificates.Select(static certificate => CBORValue.FromBytes(certificate.GetEncoded())));

        #endregion

        #region PublicKey()

        /// <summary>
        /// The public key of the end-entity certificate.
        /// </summary>
        public AsymmetricKeyParameter PublicKey()

            => EndEntity.GetPublicKey();

        #endregion

        #region PermitsAlgorithm(Algorithm, out ErrorResponse)

        /// <summary>
        /// Whether the end-entity certificate allows its key to verify a
        /// message signed with the given algorithm, as far as the certificate
        /// itself restricts the key.
        ///
        /// One key type does that: id-RSASSA-PSS [RFC 4055, Section 3.1],
        /// written "when the RSA private key owner wishes to limit the use of
        /// the public key exclusively to RSASSA-PSS". Bouncy Castle reads such
        /// a key as an RsaKeyParameters that cannot be told apart from an
        /// rsaEncryption key, and keeps the identifier on the certificate. So
        /// the restriction reaches the verification only if it is read here.
        ///
        /// What is enforced is scenario 3 of RFC 4055, Section 3.3, which is a
        /// MUST. A key with parameters names a hash, a mask generation function
        /// and a salt, and "all parameters in the signature structure algorithm
        /// identifier MUST match the parameters in the key structure algorithm
        /// identifier except the saltLength field", which "MUST be greater or
        /// equal to that in the key parameters field". A COSE PS* is MGF1 over
        /// its own hash with a salt as long as that hash [RFC 8230, Section 2].
        /// The key's parameters therefore have to name exactly that hash and
        /// that MGF1, and its salt is a floor the hash has to reach. A key
        /// without parameters is scenario 2 and restricts nothing but the
        /// scheme.
        ///
        /// The scheme is enforced as well: a message signed with anything but
        /// RSASSA-PSS, RSASSA-PKCS1-v1_5 in particular, is refused under such
        /// a key, with or without parameters. RFC 4055 states that
        /// restriction as what the key owner wishes, and neither it nor RFC
        /// 9360 makes it a duty of the verifier in so many words. It is
        /// honoured all the same: the owner said what the key is for, and a
        /// verifier that accepts another scheme gains nothing a legitimate
        /// signer needs.
        /// </summary>
        /// <param name="Algorithm">The algorithm the message was signed with.</param>
        /// <param name="ErrorResponse">Why the certificate does not allow it.</param>
        public Boolean PermitsAlgorithm(COSEAlgorithm                     Algorithm,
                                        [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ErrorResponse     = null;

            var keyAlgorithm  = EndEntity.SubjectPublicKeyInfo.Algorithm;

            if (!keyAlgorithm.Algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss))
                return true;

            var restricts = $"The end-entity certificate '{EndEntity.SubjectDN}' restricts its key to RSASSA-PSS";

            // The scheme, whatever the parameters say.
            if (Algorithm.Family != COSEAlgorithmFamily.RSAPSS)
            {
                ErrorResponse = $"{restricts}, and this message is signed with '{Algorithm.Name}', which is not RSASSA-PSS [RFC 4055, Section 3.1]!";
                return false;
            }

            if (keyAlgorithm.Parameters is null)
                return true;

            var (hash, hashLength) = Algorithm.HashAlgorithm?.Name switch {
                                         "SHA256" => (NistObjectIdentifiers.IdSha256, 32),
                                         "SHA384" => (NistObjectIdentifiers.IdSha384, 48),
                                         "SHA512" => (NistObjectIdentifiers.IdSha512, 64),
                                         _        => throw new COSEException($"The RSASSA-PSS algorithm '{Algorithm.Name}' names no hash this implementation knows!")
                                     };

            try
            {

                var restriction = RsassaPssParameters.GetInstance(keyAlgorithm.Parameters);

                var keyHash     = restriction.HashAlgorithm.Algorithm;

                if (!keyHash.Equals(hash))
                {
                    ErrorResponse = $"{restricts} with {HashName(keyHash)}, and this message is signed with '{Algorithm.Name}', which hashes with {HashName(hash)}: RFC 4055 Section 3.3 requires the parameters of the signature to be the key's!";
                    return false;
                }

                if (!restriction.MaskGenAlgorithm.Algorithm.Equals(PkcsObjectIdentifiers.IdMgf1))
                {
                    ErrorResponse = $"{restricts} with the mask generation function '{restriction.MaskGenAlgorithm.Algorithm}', and '{Algorithm.Name}' masks with MGF1 [RFC 8230, Section 2]: RFC 4055 Section 3.3 requires the parameters of the signature to be the key's!";
                    return false;
                }

                var maskHash = AlgorithmIdentifier.GetInstance(restriction.MaskGenAlgorithm.Parameters).Algorithm;

                if (!maskHash.Equals(hash))
                {
                    ErrorResponse = $"{restricts} with MGF1 over {HashName(maskHash)}, and '{Algorithm.Name}' masks with MGF1 over {HashName(hash)} [RFC 8230, Section 2]: RFC 4055 Section 3.3 requires the parameters of the signature to be the key's!";
                    return false;
                }

                var trailerField = restriction.TrailerField.IntValueExact;

                if (trailerField != 1)
                {
                    ErrorResponse = $"{restricts} with the trailer field {trailerField}, where RFC 4055 Section 3.1 says the value must be 1!";
                    return false;
                }

                // The one field a key states differently from a signature: a
                // floor, not the width that was used.
                var saltLength = restriction.SaltLength.IntValueExact;

                if (saltLength > hashLength)
                {
                    ErrorResponse = $"{restricts} with a salt of at least {saltLength} octets, and '{Algorithm.Name}' signs with a salt of {hashLength} [RFC 4055, Section 3.3; RFC 8230, Section 2]!";
                    return false;
                }

            }
            catch (Exception e) when (e is ArgumentException or InvalidCastException or ArithmeticException)
            {
                ErrorResponse = $"{restricts} with parameters that are not RSASSA-PSS-params [RFC 4055, Section 3.1]: {e.Message}";
                return false;
            }

            return true;

        }

        private static String HashName(DerObjectIdentifier Hash)

            => Hash.Equals(OiwObjectIdentifiers.IdSha1)    ? "SHA-1"
             : Hash.Equals(NistObjectIdentifiers.IdSha256) ? "SHA-256"
             : Hash.Equals(NistObjectIdentifiers.IdSha384) ? "SHA-384"
             : Hash.Equals(NistObjectIdentifiers.IdSha512) ? "SHA-512"
             : $"the hash '{Hash}'";

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Join(" <- ", certificates.Select(static certificate => certificate.SubjectDN.ToString()));

        #endregion

    }

}
