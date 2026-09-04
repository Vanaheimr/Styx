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
using Org.BouncyCastle.Crypto;

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

                var parser = new X509CertificateParser();

                Chain          = new COSECertificateChain(
                                     encoded.Select(certificate => parser.ReadCertificate(certificate))
                                 );

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

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Join(" <- ", certificates.Select(static certificate => certificate.SubjectDN.ToString()));

        #endregion

    }

}
