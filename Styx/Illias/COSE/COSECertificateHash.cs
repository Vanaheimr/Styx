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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A COSE_CertHash [RFC 9360, Section 2]: the hash of a DER encoded
    /// X.509 certificate, as carried by the "x5t" header parameter.
    ///
    /// <code>
    /// COSE_CertHash = [ hashAlg: (int / tstr), hashValue: bstr ]
    /// </code>
    ///
    /// A thumbprint says which certificate is meant, not that it may be
    /// trusted: it is a lookup key for a certificate the recipient is
    /// expected to have already, never a substitute for validating one.
    /// </summary>
    public sealed class COSECertificateHash
    {

        #region Properties

        /// <summary>
        /// The hash algorithm the thumbprint was computed with.
        /// </summary>
        public COSEAlgorithm  Algorithm    { get; }

        /// <summary>
        /// The hash of the DER encoding of the certificate.
        /// </summary>
        public Byte[]         Value        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new certificate thumbprint.
        /// </summary>
        /// <param name="Algorithm">The hash algorithm.</param>
        /// <param name="Value">The hash of the DER encoding of the certificate.</param>
        public COSECertificateHash(COSEAlgorithm  Algorithm,
                                   Byte[]         Value)
        {

            this.Algorithm  = Algorithm;
            this.Value      = Value;

        }

        #endregion


        #region (static) From    (Certificate, Algorithm = null)

        /// <summary>
        /// Compute the thumbprint of the given certificate.
        /// </summary>
        /// <param name="Certificate">An X.509 certificate.</param>
        /// <param name="Algorithm">The hash algorithm to use, SHA-256 by default.</param>
        public static COSECertificateHash From(X509Certificate  Certificate,
                                               COSEAlgorithm?   Algorithm   = null)
        {

            var algorithm = Algorithm ?? COSEAlgorithm.SHA256;

            return new COSECertificateHash(
                       algorithm,
                       algorithm.Hash(Certificate.GetEncoded())
                   );

        }

        #endregion

        #region (static) TryParse(CBOR, out CertificateHash, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR array as a COSE_CertHash.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a certificate thumbprint.</param>
        /// <param name="CertificateHash">The parsed certificate thumbprint.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                                    CBOR,
                                       [NotNullWhen(true)]  out COSECertificateHash?  CertificateHash,
                                       [NotNullWhen(false)] out String?               ErrorResponse)
        {

            CertificateHash = null;

            if (CBOR.Kind != CBORValueKind.Array)
            {
                ErrorResponse = $"A COSE_CertHash must be a CBOR array, but was a CBOR {CBOR.Kind}!";
                return false;
            }

            if (CBOR.Count != 2)
            {
                ErrorResponse = $"A COSE_CertHash must be a CBOR array of 2 elements, but had {CBOR.Count} element(s)!";
                return false;
            }

            var items = CBOR.AsArray();

            if (!COSEAlgorithm.TryParse(items[0], out var algorithm, out ErrorResponse))
                return false;

            if (!items[1].TryGetBytes(out var value))
            {
                ErrorResponse = "The hash value of a COSE_CertHash must be a byte string!";
                return false;
            }

            CertificateHash  = new COSECertificateHash(algorithm, value);
            ErrorResponse    = null;
            return true;

        }

        #endregion

        #region Matches(Certificate, out ErrorResponse)

        /// <summary>
        /// Whether this thumbprint is the thumbprint of the given certificate.
        /// </summary>
        /// <param name="Certificate">An X.509 certificate.</param>
        /// <param name="ErrorResponse">The reason why it is not.</param>
        public Boolean Matches(X509Certificate                   Certificate,
                               [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (Algorithm.HashAlgorithm is null)
            {
                ErrorResponse = $"The certificate thumbprint uses the hash algorithm '{Algorithm.Name}', which this implementation does not compute!";
                return false;
            }

            if (!Algorithm.Hash(Certificate.GetEncoded()).SequenceEqual(Value))
            {
                ErrorResponse = "The certificate thumbprint does not belong to the given certificate!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ToCBOR()

        /// <summary>
        /// Return a CBOR representation of this certificate thumbprint.
        /// </summary>
        public CBORValue ToCBOR()

            => CBORValue.FromArray(
                   Algorithm.ToCBOR(),
                   CBORValue.FromBytes(Value)
               );

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Algorithm.Name}: {Value.ToHexString()}";

        #endregion

    }

}
