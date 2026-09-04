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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// One signature of a COSE_Sign message [RFC 9052, Section 4.1]:
    ///
    /// <code>
    /// COSE_Signature = [
    ///     protected   : bstr .cbor header_map,   ; covered by this signature
    ///     unprotected : header_map,              ; NOT covered by any signature
    ///     signature   : bstr
    /// ]
    /// </code>
    ///
    /// A signature carries its own pair of header buckets, which is what
    /// allows several parties to sign one and the same payload each with
    /// their own algorithm and their own key.
    ///
    /// The very same structure is a COSE_Countersignature [RFC 9338], which
    /// the specification defines as being a COSE_Signature - what differs is
    /// not the structure but what is signed.
    ///
    /// It can not be verified on its own: What is signed is a Sig_structure
    /// that also names the body of the message and its payload. Use
    /// COSESign.Verify(...) or COSESign1.VerifyCountersignature(...) for that.
    /// </summary>
    public sealed class COSESignature
    {

        #region Properties

        /// <summary>
        /// The serialized protected header bucket of this signature, exactly
        /// as signed and as received: A zero-length byte string when there
        /// are no protected header parameters.
        /// </summary>
        public Byte[]          ProtectedHeaderBytes    { get; }

        /// <summary>
        /// The protected header parameters of this signature,
        /// which this signature covers.
        /// </summary>
        public COSEHeaders     ProtectedHeader         { get; }

        /// <summary>
        /// The unprotected header parameters of this signature, which no
        /// signature covers and which therefore must not be trusted after a
        /// successful verification.
        /// </summary>
        public COSEHeaders     UnprotectedHeader       { get; }

        /// <summary>
        /// The signature: The ECDSA components r and s, each zero-padded to
        /// the width of the group order of the elliptic curve and
        /// concatenated [RFC 9053, Section 2.1].
        /// </summary>
        public Byte[]          Signature               { get; }

        /// <summary>
        /// The signature algorithm, taken from the protected header bucket
        /// and only otherwise from the unprotected one.
        /// </summary>
        public COSEAlgorithm?  Algorithm

            => ProtectedHeader.  Algorithm ??
               UnprotectedHeader.Algorithm;

        /// <summary>
        /// The key identifier, taken from the protected header bucket
        /// and only otherwise from the unprotected one.
        /// </summary>
        public Byte[]?         KeyIdentifier

            => ProtectedHeader.  KeyIdentifier ??
               UnprotectedHeader.KeyIdentifier;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signature of a COSE_Sign message from its parts.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket, which the signature covers byte by byte.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters.</param>
        /// <param name="Signature">The signature.</param>
        public COSESignature(Byte[]        ProtectedHeaderBytes,
                             COSEHeaders?  UnprotectedHeader,
                             Byte[]        Signature)
        {

            if (!COSEHeaders.TryParseProtected(ProtectedHeaderBytes, out var protectedHeader, out var errorResponse))
                throw new COSEException($"The protected header bucket of the signature is invalid: {errorResponse}");

            this.ProtectedHeaderBytes  = ProtectedHeaderBytes;
            this.ProtectedHeader       = protectedHeader;
            this.UnprotectedHeader     = UnprotectedHeader ?? COSEHeaders.Empty;
            this.Signature             = Signature;

        }

        #endregion


        #region (static) Parse   (CBOR)

        /// <summary>
        /// Parse the given CBOR array as one signature of a COSE_Sign message.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Signature.</param>
        public static COSESignature Parse(CBORValue CBOR)
        {

            if (TryParse(CBOR, out var signature, out var errorResponse))
                return signature;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region (static) TryParse(CBOR, out Signature, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR array as one signature of a
        /// COSE_Sign message.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Signature.</param>
        /// <param name="Signature">The parsed signature.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                              CBOR,
                                       [NotNullWhen(true)]  out COSESignature?  Signature,
                                       [NotNullWhen(false)] out String?         ErrorResponse)
        {

            Signature = null;

            if (CBOR.Kind != CBORValueKind.Array)
            {
                ErrorResponse = $"A COSE_Signature must be a CBOR array, but was a CBOR {CBOR.Kind}!";
                return false;
            }

            if (CBOR.Count != 3)
            {
                ErrorResponse = $"A COSE_Signature must be a CBOR array of 3 elements, but had {CBOR.Count} element(s)!";
                return false;
            }

            var items = CBOR.AsArray();

            if (!items[0].TryGetBytes(out var protectedHeaderBytes))
            {
                ErrorResponse = "The protected header bucket of a COSE_Signature must be a byte string!";
                return false;
            }

            if (!COSEHeaders.TryParse(items[1], out var unprotectedHeader, out ErrorResponse))
                return false;

            if (!items[2].TryGetBytes(out var signature))
            {
                ErrorResponse = "The signature of a COSE_Signature must be a byte string!";
                return false;
            }

            try
            {

                Signature = new COSESignature(
                                protectedHeaderBytes,
                                unprotectedHeader,
                                signature
                            );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR()

        /// <summary>
        /// Return a CBOR representation of this signature.
        /// </summary>
        public CBORValue ToCBOR()

            => CBORValue.FromArray(
                   CBORValue.FromBytes(ProtectedHeaderBytes),
                   UnprotectedHeader.ToCBOR(),
                   CBORValue.FromBytes(Signature)
               );

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Algorithm.HasValue
                       ? Algorithm.Value.Name
                       : "without an algorithm",

                   KeyIdentifier is not null
                       ? $", key '{KeyIdentifier.ToHexString()}'"
                       : "",

                   $", {Signature.Length} byte(s) of signature"

               );

        #endregion

    }

}
