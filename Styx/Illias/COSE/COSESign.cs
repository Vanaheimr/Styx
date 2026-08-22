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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A COSE_Sign message [RFC 9052, Section 4.1]: A payload signed by one
    /// or more signers, tagged with CBOR tag 98.
    ///
    /// <code>
    /// COSE_Sign = [
    ///     protected   : bstr .cbor header_map,   ; the body headers
    ///     unprotected : header_map,
    ///     payload     : bstr / nil,              ; nil = detached
    ///     signatures  : [+ COSE_Signature]
    /// ]
    /// </code>
    ///
    /// Each signature is computed over the Sig_structure
    /// ["Signature", body_protected, sign_protected, external_aad, payload]
    /// [RFC 9052, Section 4.4], which differs from the four-element structure
    /// of a COSE_Sign1 by the protected header bucket of the individual
    /// signature.
    ///
    /// The signatures are independent of one another: Each covers the body,
    /// its own header bucket and the payload, but never another signature.
    /// A second party can therefore sign an existing message later on, with
    /// its own key and its own algorithm, without invalidating - or even
    /// seeing - the first signature. That is the "the meter signs, the
    /// backend counter-signs" arrangement.
    ///
    /// A signature over another signature is a different thing entirely, the
    /// counter signature of RFC 9338, which is not implemented here.
    /// </summary>
    public sealed class COSESign
    {

        #region Data

        /// <summary>
        /// The context text string of a COSE_Sign signature
        /// [RFC 9052, Section 4.4].
        /// </summary>
        public const String SignatureContext = "Signature";

        private readonly COSESignature[] signatures;

        #endregion

        #region Properties

        /// <summary>
        /// The serialized protected header bucket of the message body,
        /// exactly as signed and as received: A zero-length byte string when
        /// there are no protected header parameters.
        /// </summary>
        public Byte[]                          ProtectedHeaderBytes    { get; }

        /// <summary>
        /// The protected header parameters of the message body,
        /// which every signature covers.
        /// </summary>
        public COSEHeaders                     ProtectedHeader         { get; }

        /// <summary>
        /// The unprotected header parameters of the message body, which no
        /// signature covers and which therefore must not be trusted after a
        /// successful verification.
        /// </summary>
        public COSEHeaders                     UnprotectedHeader       { get; }

        /// <summary>
        /// The signed payload, or null when the payload is detached and thus
        /// transported outside of this message.
        /// </summary>
        public Byte[]?                         Payload                 { get; }

        /// <summary>
        /// The signatures, in the order they appear within the message.
        /// </summary>
        public IReadOnlyList<COSESignature>    Signatures

            => signatures;

        /// <summary>
        /// Whether this message is wrapped within CBOR tag 98.
        /// </summary>
        public Boolean                         IsTagged                { get; }

        /// <summary>
        /// Whether the payload is detached.
        /// </summary>
        public Boolean                         IsDetached

            => Payload is null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new COSE_Sign message from its parts.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket of the message body, which every signature covers byte by byte.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters of the message body.</param>
        /// <param name="Payload">The signed payload, or null when detached.</param>
        /// <param name="Signatures">The signatures, of which there has to be at least one.</param>
        /// <param name="IsTagged">Whether the message is wrapped within CBOR tag 98.</param>
        public COSESign(Byte[]                      ProtectedHeaderBytes,
                        COSEHeaders?                UnprotectedHeader,
                        Byte[]?                     Payload,
                        IEnumerable<COSESignature>  Signatures,
                        Boolean                     IsTagged   = true)
        {

            if (!COSEHeaders.TryParseProtected(ProtectedHeaderBytes, out var protectedHeader, out var errorResponse))
                throw new COSEException($"The protected header bucket of the message body is invalid: {errorResponse}");

            this.signatures            = Signatures.ToArray();

            if (this.signatures.Length == 0)
                throw new COSEException("A COSE_Sign message must carry at least one signature!");

            this.ProtectedHeaderBytes  = ProtectedHeaderBytes;
            this.ProtectedHeader       = protectedHeader;
            this.UnprotectedHeader     = UnprotectedHeader ?? COSEHeaders.Empty;
            this.Payload               = Payload;
            this.IsTagged              = IsTagged;

        }

        #endregion


        #region (static) ToBeSigned(BodyProtectedHeaderBytes, SignatureProtectedHeaderBytes, Payload, ExternalAAD = null)

        /// <summary>
        /// Return the encoded Sig_structure of one signature of a COSE_Sign
        /// message [RFC 9052, Section 4.4], which is the byte string an ECDSA
        /// signer actually signs:
        ///
        /// <code>
        /// Sig_structure = [
        ///     context        : "Signature",
        ///     body_protected : empty_or_serialized_map,
        ///     sign_protected : empty_or_serialized_map,
        ///     external_aad   : bstr,
        ///     payload        : bstr
        /// ]
        /// </code>
        ///
        /// This is public on purpose: Whenever the signing key does not live
        /// in this process - a meter, a smart card, a hardware security
        /// module - this is the input to hand over.
        /// </summary>
        /// <param name="BodyProtectedHeaderBytes">The serialized protected header bucket of the message body.</param>
        /// <param name="SignatureProtectedHeaderBytes">The serialized protected header bucket of the individual signature.</param>
        /// <param name="Payload">The payload, also when it is transported detached.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        public static Byte[] ToBeSigned(Byte[]   BodyProtectedHeaderBytes,
                                        Byte[]   SignatureProtectedHeaderBytes,
                                        Byte[]   Payload,
                                        Byte[]?  ExternalAAD   = null)
        {

            var writer = new CBORWriter();

            writer.WriteStartArray(5);
            writer.WriteTextString(SignatureContext);
            writer.WriteByteString(BodyProtectedHeaderBytes);
            writer.WriteByteString(SignatureProtectedHeaderBytes);
            writer.WriteByteString(ExternalAAD ?? []);
            writer.WriteByteString(Payload);
            writer.WriteEndArray();

            return writer.ToByteArray();

        }

        #endregion

        #region ToBeSigned(Signature, ExternalAAD = null, DetachedPayload = null)

        /// <summary>
        /// Return the encoded Sig_structure of the given signature of this
        /// message [RFC 9052, Section 4.4].
        /// </summary>
        /// <param name="Signature">One of the signatures of this message.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        public Byte[] ToBeSigned(COSESignature  Signature,
                                 Byte[]?        ExternalAAD       = null,
                                 Byte[]?        DetachedPayload   = null)
        {

            if (!TryGetPayload(DetachedPayload, out var payload, out var errorResponse))
                throw new COSEException(errorResponse);

            return ToBeSigned(ProtectedHeaderBytes,
                              Signature.ProtectedHeaderBytes,
                              payload,
                              ExternalAAD);

        }

        #endregion

        #region (private) TryGetPayload(DetachedPayload, out Payload, out ErrorResponse)

        /// <summary>
        /// Resolve which payload the signatures are computed over: The one
        /// carried within the message, or the detached one supplied by the
        /// caller. Supplying both is rejected, because there would be no way
        /// to tell which of the two the verification result refers to.
        /// </summary>
        private Boolean TryGetPayload(Byte[]?                          DetachedPayload,
                                      [NotNullWhen(true)]  out Byte[]? Payload,
                                      [NotNullWhen(false)] out String? ErrorResponse)
        {

            if (this.Payload is not null)
            {

                if (DetachedPayload is not null)
                {
                    Payload        = null;
                    ErrorResponse  = "This COSE_Sign message carries its payload, therefore no detached payload must be supplied!";
                    return false;
                }

                Payload        = this.Payload;
                ErrorResponse  = null;
                return true;

            }

            if (DetachedPayload is null)
            {
                Payload        = null;
                ErrorResponse  = "The payload of this COSE_Sign message is detached, therefore it must be supplied for the signature to be computed!";
                return false;
            }

            Payload        = DetachedPayload;
            ErrorResponse  = null;
            return true;

        }

        #endregion


        #region (static) Sign(Payload, PrivateKey, Algorithm, KeyIdentifier = null, ...)

        /// <summary>
        /// Sign the given payload, creating a COSE_Sign message with its
        /// first signature. Further signers are added afterwards via
        /// AddSignature, which is what makes this message type worth its
        /// extra bytes over a COSE_Sign1.
        /// </summary>
        /// <param name="Payload">The payload to sign.</param>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="Algorithm">The signature algorithm.</param>
        /// <param name="KeyIdentifier">An optional key identifier.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachPayload">Whether to omit the payload from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 98.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        /// <param name="CanonicalizePayload">Whether to rewrite a CBOR payload in the deterministic encoding of RFC 8949, Section 4.2.1 before signing it, so that a receiver who parses and re-serializes it arrives at the very bytes this signature covers. A payload that is not CBOR is signed as it is. Not to be confused with Deterministic, which is about the ECDSA nonce rather than about the bytes.</param>
        public static COSESign Sign(Byte[]                  Payload,
                                    AsymmetricKeyParameter  PrivateKey,
                                    COSEAlgorithm           Algorithm,
                                    Byte[]?                 KeyIdentifier         = null,
                                    Byte[]?                 ExternalAAD           = null,
                                    Boolean                 DetachPayload         = false,
                                    Boolean                 Tagged                = true,
                                    SecureRandom?           Random                = null,
                                    Boolean                 Deterministic         = false,
                                    Boolean                 CanonicalizePayload   = true)

            => Sign(Payload,
                    PrivateKey,
                    COSEHeaders.Create(Algorithm),
                    KeyIdentifier is not null
                        ? COSEHeaders.Create(null, KeyIdentifier)
                        : COSEHeaders.Empty,
                    ExternalAAD,
                    DetachPayload,
                    Tagged,
                    null,
                    null,
                    Random,
                    Deterministic,
                    CanonicalizePayload);

        #endregion

        #region (static) Sign(Payload, PrivateKey, SignatureProtectedHeader, ...)

        /// <summary>
        /// Sign the given payload with full control over all four header
        /// buckets: The two of the message body, which every signature
        /// covers, and the two of this first signature.
        /// </summary>
        /// <param name="Payload">The payload to sign.</param>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="SignatureProtectedHeader">The protected header parameters of this signature, which must name the signature algorithm.</param>
        /// <param name="SignatureUnprotectedHeader">The optional unprotected header parameters of this signature.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachPayload">Whether to omit the payload from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 98.</param>
        /// <param name="BodyProtectedHeader">The optional protected header parameters of the message body.</param>
        /// <param name="BodyUnprotectedHeader">The optional unprotected header parameters of the message body.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        /// <param name="CanonicalizePayload">Whether to rewrite a CBOR payload in the deterministic encoding of RFC 8949, Section 4.2.1 before signing it, so that a receiver who parses and re-serializes it arrives at the very bytes this signature covers. A payload that is not CBOR is signed as it is. Not to be confused with Deterministic, which is about the ECDSA nonce rather than about the bytes.</param>
        public static COSESign Sign(Byte[]                  Payload,
                                    AsymmetricKeyParameter  PrivateKey,
                                    COSEHeaders             SignatureProtectedHeader,
                                    COSEHeaders?            SignatureUnprotectedHeader   = null,
                                    Byte[]?                 ExternalAAD                  = null,
                                    Boolean                 DetachPayload                = false,
                                    Boolean                 Tagged                       = true,
                                    COSEHeaders?            BodyProtectedHeader          = null,
                                    COSEHeaders?            BodyUnprotectedHeader        = null,
                                    SecureRandom?           Random                       = null,
                                    Boolean                 Deterministic                = false,
                                    Boolean                 CanonicalizePayload          = true)
        {

            var algorithm                = SignatureProtectedHeader.Algorithm
                                               ?? throw new COSEException("The protected header bucket of the signature must name the signature algorithm!");

            var bodyProtectedBytes       = (BodyProtectedHeader ?? COSEHeaders.Empty).ToProtectedByteArray();
            var signatureProtectedBytes  = SignatureProtectedHeader.ToProtectedByteArray();

            var payload                  = CanonicalizePayload
                                               ? COSEPayload.Canonicalize(Payload)
                                               : Payload;

            // A detached payload is the caller's to transmit, and every
            // further signer added later signs the very same bytes. Rewriting
            // them here would sign a spelling nobody else has.
            if (DetachPayload && !payload.AsSpan().SequenceEqual(Payload))
                throw new COSEException("The payload of this COSE_Sign message is detached, so canonicalizing it here would sign bytes that nobody holds: Canonicalize the payload yourself (COSEPayload.Canonicalize), sign and transmit those, or pass CanonicalizePayload: false to sign the payload exactly as it is!");

            var signature                = algorithm.Sign(
                                               ToBeSigned(bodyProtectedBytes, signatureProtectedBytes, payload, ExternalAAD),
                                               PrivateKey,
                                               Random,
                                               Deterministic
                                           );

            return new COSESign(
                       bodyProtectedBytes,
                       BodyUnprotectedHeader,
                       DetachPayload ? null : payload,
                       [
                           new COSESignature(
                               signatureProtectedBytes,
                               SignatureUnprotectedHeader,
                               signature
                           )
                       ],
                       Tagged
                   );

        }

        #endregion

        #region AddSignature(PrivateKey, Algorithm, KeyIdentifier = null, ...)

        /// <summary>
        /// Return a copy of this message with one more signature over the
        /// very same payload. The existing signatures stay valid, as none of
        /// them ever covered the others.
        /// </summary>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="Algorithm">The signature algorithm.</param>
        /// <param name="KeyIdentifier">An optional key identifier.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        public COSESign AddSignature(AsymmetricKeyParameter  PrivateKey,
                                     COSEAlgorithm           Algorithm,
                                     Byte[]?                 KeyIdentifier     = null,
                                     Byte[]?                 ExternalAAD       = null,
                                     Byte[]?                 DetachedPayload   = null,
                                     SecureRandom?           Random            = null,
                                     Boolean                 Deterministic     = false)

            => AddSignature(PrivateKey,
                            COSEHeaders.Create(Algorithm),
                            KeyIdentifier is not null
                                ? COSEHeaders.Create(null, KeyIdentifier)
                                : COSEHeaders.Empty,
                            ExternalAAD,
                            DetachedPayload,
                            Random,
                            Deterministic);

        #endregion

        #region AddSignature(PrivateKey, SignatureProtectedHeader, SignatureUnprotectedHeader = null, ...)

        /// <summary>
        /// Return a copy of this message with one more signature over the
        /// very same payload, with full control over the header buckets of
        /// that signature.
        /// </summary>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="SignatureProtectedHeader">The protected header parameters of the new signature, which must name the signature algorithm.</param>
        /// <param name="SignatureUnprotectedHeader">The optional unprotected header parameters of the new signature.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        public COSESign AddSignature(AsymmetricKeyParameter  PrivateKey,
                                     COSEHeaders             SignatureProtectedHeader,
                                     COSEHeaders?            SignatureUnprotectedHeader   = null,
                                     Byte[]?                 ExternalAAD                  = null,
                                     Byte[]?                 DetachedPayload              = null,
                                     SecureRandom?           Random                       = null,
                                     Boolean                 Deterministic                = false)
        {

            var algorithm                = SignatureProtectedHeader.Algorithm
                                               ?? throw new COSEException("The protected header bucket of the signature must name the signature algorithm!");

            if (!TryGetPayload(DetachedPayload, out var payload, out var errorResponse))
                throw new COSEException(errorResponse);

            var signatureProtectedBytes  = SignatureProtectedHeader.ToProtectedByteArray();

            var signature                = algorithm.Sign(
                                               ToBeSigned(ProtectedHeaderBytes, signatureProtectedBytes, payload, ExternalAAD),
                                               PrivateKey,
                                               Random,
                                               Deterministic
                                           );

            return new COSESign(
                       ProtectedHeaderBytes,
                       UnprotectedHeader,
                       Payload,
                       [
                           .. signatures,
                           new COSESignature(
                               signatureProtectedBytes,
                               SignatureUnprotectedHeader,
                               signature
                           )
                       ],
                       IsTagged
                   );

        }

        #endregion


        #region Verify      (Signature, PublicKey, out ErrorResponse, ...)

        /// <summary>
        /// Verify one of the signatures of this message.
        /// A failed verification is not an exception: It is the expected
        /// outcome of checking untrusted data.
        /// </summary>
        /// <param name="Signature">One of the signatures of this message.</param>
        /// <param name="PublicKey">An elliptic curve public key.</param>
        /// <param name="ErrorResponse">The reason why the verification failed.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that was signed along with the payload.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="ExpectedAlgorithm">The signature algorithm the caller expects, required whenever the signature states its algorithm within its unprotected header bucket only.</param>
        public Boolean Verify(COSESignature                     Signature,
                              AsymmetricKeyParameter            PublicKey,
                              [NotNullWhen(false)] out String?  ErrorResponse,
                              Byte[]?                           ExternalAAD         = null,
                              Byte[]?                           DetachedPayload     = null,
                              COSEAlgorithm?                    ExpectedAlgorithm   = null)
        {

            #region Whether the signature belongs to this message at all

            if (!signatures.Any(signature => ReferenceEquals(signature, Signature)))
            {
                ErrorResponse = "The given signature is not one of the signatures of this COSE_Sign message!";
                return false;
            }

            #endregion

            #region The header parameters a recipient must understand

            if (!COSEHeaders.VerifyCriticalHeaderParameters(ProtectedHeader,
                                                            UnprotectedHeader,
                                                            out ErrorResponse))
            {
                return false;
            }

            if (!COSEHeaders.VerifyCriticalHeaderParameters(Signature.ProtectedHeader,
                                                            Signature.UnprotectedHeader,
                                                            out ErrorResponse))
            {
                return false;
            }

            #endregion

            #region Which algorithm to verify with

            var algorithm = ExpectedAlgorithm ?? Signature.ProtectedHeader.Algorithm;

            if (!algorithm.HasValue)
            {

                ErrorResponse = Signature.UnprotectedHeader.Algorithm.HasValue
                                    ? $"This signature states its algorithm '{Signature.UnprotectedHeader.Algorithm.Value.Name}' within the unprotected header bucket only, where it is not covered by the signature: Pass the expected algorithm explicitly in order to accept it!"
                                    :  "This signature does not state its algorithm: Pass the expected algorithm explicitly!";

                return false;

            }

            var statedAlgorithm = Signature.Algorithm;

            if (statedAlgorithm.HasValue &&
                statedAlgorithm.Value != algorithm.Value)
            {
                ErrorResponse = $"This signature was created with the algorithm '{statedAlgorithm.Value.Name}', but the algorithm '{algorithm.Value.Name}' was expected!";
                return false;
            }

            #endregion

            #region The signature itself

            if (!TryGetPayload(DetachedPayload, out var payload, out ErrorResponse))
                return false;

            return algorithm.Value.Verify(
                       ToBeSigned(ProtectedHeaderBytes, Signature.ProtectedHeaderBytes, payload, ExternalAAD),
                       Signature.Signature,
                       PublicKey,
                       out ErrorResponse
                   );

            #endregion

        }

        #endregion

        #region TryVerifyAny(PublicKey, out Signature, out ErrorResponse, ...)

        /// <summary>
        /// Find a signature of this message that the given public key
        /// verifies, e.g. in order to answer "did this party sign this?"
        /// without knowing which of the signatures is theirs.
        /// </summary>
        /// <param name="PublicKey">An elliptic curve public key.</param>
        /// <param name="Signature">The signature the given key verifies.</param>
        /// <param name="ErrorResponse">The reason why none of the signatures verified.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that was signed along with the payload.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        public Boolean TryVerifyAny(AsymmetricKeyParameter                 PublicKey,
                                    [NotNullWhen(true)]  out COSESignature?  Signature,
                                    [NotNullWhen(false)] out String?         ErrorResponse,
                                    Byte[]?                                  ExternalAAD       = null,
                                    Byte[]?                                  DetachedPayload   = null)
        {

            foreach (var signature in signatures)
            {

                if (Verify(signature,
                           PublicKey,
                           out _,
                           ExternalAAD,
                           DetachedPayload))
                {
                    Signature      = signature;
                    ErrorResponse  = null;
                    return true;
                }

            }

            Signature      = null;
            ErrorResponse  = $"None of the {signatures.Length} signature(s) of this COSE_Sign message was verified by the given key!";
            return false;

        }

        #endregion


        #region (static) Parse   (CBOR)

        /// <summary>
        /// Parse the given CBOR value as a COSE_Sign message.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Sign message.</param>
        public static COSESign Parse(CBORValue CBOR)
        {

            if (TryParse(CBOR, out var sign, out var errorResponse))
                return sign;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region (static) Parse   (Data)

        /// <summary>
        /// Parse the given CBOR data as a COSE_Sign message.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE_Sign message.</param>
        public static COSESign Parse(ReadOnlySpan<Byte> Data)
        {

            if (TryParse(Data, out var sign, out var errorResponse))
                return sign;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region (static) TryParse(Data, out Sign, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR data as a COSE_Sign message.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE_Sign message.</param>
        /// <param name="Sign">The parsed COSE_Sign message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte>                  Data,
                                       [NotNullWhen(true)]  out COSESign?  Sign,
                                       [NotNullWhen(false)] out String?    ErrorResponse)
        {

            if (!CBORValue.TryParse(Data, out var cbor, out ErrorResponse))
            {
                Sign = null;
                return false;
            }

            return TryParse(cbor, out Sign, out ErrorResponse);

        }

        #endregion

        #region (static) TryParse(CBOR, out Sign, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR value as a COSE_Sign message.
        /// Both the tagged and the untagged form are accepted; which one it
        /// was is remembered, as the CBOR tag is not covered by any signature
        /// but is part of the bytes on the wire.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Sign message.</param>
        /// <param name="Sign">The parsed COSE_Sign message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                           CBOR,
                                       [NotNullWhen(true)]  out COSESign?  Sign,
                                       [NotNullWhen(false)] out String?    ErrorResponse)
        {

            Sign = null;

            var isTagged  = false;
            var message   = CBOR;

            if (message.Kind == CBORValueKind.Tagged)
            {

                if (message.Tag != CBORTag.COSESign)
                {
                    ErrorResponse = $"A COSE_Sign message must be tagged with CBOR tag {CBORTag.COSESign.Value}, but was tagged with CBOR tag {message.Tag.Value}!";
                    return false;
                }

                isTagged  = true;
                message   = message.UntaggedValue;

            }

            if (message.Kind != CBORValueKind.Array)
            {
                ErrorResponse = $"A COSE_Sign message must be a CBOR array, but was a CBOR {message.Kind}!";
                return false;
            }

            if (message.Count != 4)
            {
                ErrorResponse = $"A COSE_Sign message must be a CBOR array of 4 elements, but had {message.Count} element(s)!";
                return false;
            }

            var items = message.AsArray();

            if (!items[0].TryGetBytes(out var protectedHeaderBytes))
            {
                ErrorResponse = "The protected header bucket of a COSE_Sign message must be a byte string!";
                return false;
            }

            if (!COSEHeaders.TryParse(items[1], out var unprotectedHeader, out ErrorResponse))
                return false;

            Byte[]? payload = null;

            if (items[2].Kind != CBORValueKind.Null &&
                !items[2].TryGetBytes(out payload))
            {
                ErrorResponse = "The payload of a COSE_Sign message must be a byte string, or null when it is detached!";
                return false;
            }

            if (items[3].Kind != CBORValueKind.Array)
            {
                ErrorResponse = "The signatures of a COSE_Sign message must be a CBOR array!";
                return false;
            }

            var signatures = new List<COSESignature>();

            foreach (var item in items[3].AsArray())
            {

                if (!COSESignature.TryParse(item, out var signature, out ErrorResponse))
                    return false;

                signatures.Add(signature);

            }

            try
            {

                Sign = new COSESign(
                           protectedHeaderBytes,
                           unprotectedHeader,
                           payload,
                           signatures,
                           isTagged
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
        /// Return a CBOR representation of this COSE_Sign message.
        /// </summary>
        public CBORValue ToCBOR()
        {

            var message = CBORValue.FromArray(
                              CBORValue.FromBytes(ProtectedHeaderBytes),
                              UnprotectedHeader.ToCBOR(),
                              Payload is not null
                                  ? CBORValue.FromBytes(Payload)
                                  : CBORValue.Null,
                              CBORValue.FromArray(signatures.Select(static signature => signature.ToCBOR()))
                          );

            return IsTagged
                       ? message.WithTag(CBORTag.COSESign)
                       : message;

        }

        #endregion

        #region ToByteArray(Options = null)

        /// <summary>
        /// Return the CBOR encoding of this COSE_Sign message.
        /// </summary>
        /// <param name="Options">Optional CBOR writer options.</param>
        public Byte[] ToByteArray(CBORWriterOptions? Options = null)

            => ToCBOR().ToByteArray(Options);

        #endregion

        #region Detach()

        /// <summary>
        /// Return a copy of this message without its payload, e.g. because
        /// the payload is transported elsewhere. All signatures stay valid:
        /// None of them ever covered the message, only the Sig_structure.
        /// </summary>
        public COSESign Detach()

            => new (ProtectedHeaderBytes,
                    UnprotectedHeader,
                    null,
                    signatures,
                    IsTagged);

        #endregion

        #region Attach(Payload)

        /// <summary>
        /// Return a copy of this message carrying the given payload, e.g. in
        /// order to archive a detached message together with the data it
        /// signs. Whether the payload actually belongs to the signatures is
        /// decided by verifying them, not by attaching it.
        /// </summary>
        /// <param name="Payload">The payload to attach.</param>
        public COSESign Attach(Byte[] Payload)

            => new (ProtectedHeaderBytes,
                    UnprotectedHeader,
                    Payload,
                    signatures,
                    IsTagged);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   "COSE_Sign: ",

                   IsDetached
                       ? "detached payload"
                       : $"{Payload!.Length} byte(s) of payload",

                   $", {signatures.Length} signature(s): ",

                   signatures.Select(static signature => signature.Algorithm?.Name ?? "?").
                              AggregateWith(", ")

               );

        #endregion

    }

}
