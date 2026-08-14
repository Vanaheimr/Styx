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

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A COSE_Sign1 message [RFC 9052, Section 4.2]: A payload signed by a
    /// single signer, tagged with CBOR tag 18.
    ///
    /// <code>
    /// COSE_Sign1 = [
    ///     protected   : bstr .cbor header_map,   ; covered by the signature
    ///     unprotected : header_map,              ; NOT covered by the signature
    ///     payload     : bstr / nil,              ; nil = detached
    ///     signature   : bstr
    /// ]
    /// </code>
    ///
    /// What is actually signed is never the message itself, but the
    /// Sig_structure ["Signature1", protected, external_aad, payload]
    /// [RFC 9052, Section 4.4] - see ToBeSigned(). The CBOR tag is therefore
    /// not covered by the signature, and an untagged message signed by the
    /// same key carries the very same signature bytes.
    ///
    /// The serialized protected bucket is kept verbatim, because a
    /// re-serialization that differs in a single byte - a non-preferred
    /// integer head, a different map order - would invalidate every signature
    /// made over the original bytes.
    /// </summary>
    public sealed class COSESign1
    {

        #region Data

        /// <summary>
        /// The context text string of a COSE_Sign1 signature
        /// [RFC 9052, Section 4.4].
        /// </summary>
        public const String SignatureContext = "Signature1";

        /// <summary>
        /// The context text string of a countersignature on a COSE_Sign1
        /// message, in the version 2 form that also covers the signature
        /// being countersigned [RFC 9338, Section 3.3].
        /// </summary>
        public const String CountersignatureContext = "CounterSignatureV2";

        #endregion

        #region Properties

        /// <summary>
        /// The serialized protected header bucket, exactly as signed and as
        /// received: A zero-length byte string when there are no protected
        /// header parameters.
        /// </summary>
        public Byte[]          ProtectedHeaderBytes    { get; }

        /// <summary>
        /// The protected header parameters, which are covered by the signature.
        /// </summary>
        public COSEHeaders     ProtectedHeader         { get; }

        /// <summary>
        /// The unprotected header parameters, which are NOT covered by the
        /// signature and therefore must not be trusted after a successful
        /// verification.
        /// </summary>
        public COSEHeaders     UnprotectedHeader       { get; }

        /// <summary>
        /// The signed payload, or null when the payload is detached and thus
        /// transported outside of this message.
        /// </summary>
        public Byte[]?         Payload                 { get; }

        /// <summary>
        /// The signature: The ECDSA components r and s, each zero-padded to
        /// the width of the group order of the elliptic curve and
        /// concatenated [RFC 9053, Section 2.1]. This is NOT the DER encoding
        /// most other .NET and Bouncy Castle APIs produce.
        /// </summary>
        public Byte[]          Signature               { get; }

        /// <summary>
        /// Whether this message is wrapped within CBOR tag 18. The tag is not
        /// covered by the signature, but it is preserved so that a parsed
        /// message re-encodes to the very same bytes.
        /// </summary>
        public Boolean         IsTagged                { get; }

        /// <summary>
        /// Whether the payload is detached.
        /// </summary>
        public Boolean         IsDetached

            => Payload is null;

        /// <summary>
        /// The signature algorithm, taken from the protected header bucket
        /// and only otherwise from the unprotected one. Verification does not
        /// silently trust an algorithm that is not integrity protected.
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
        /// Create a new COSE_Sign1 message from its parts.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket, which the signature covers byte by byte.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters.</param>
        /// <param name="Payload">The signed payload, or null when detached.</param>
        /// <param name="Signature">The signature.</param>
        /// <param name="IsTagged">Whether the message is wrapped within CBOR tag 18.</param>
        public COSESign1(Byte[]        ProtectedHeaderBytes,
                         COSEHeaders?  UnprotectedHeader,
                         Byte[]?       Payload,
                         Byte[]        Signature,
                         Boolean       IsTagged   = true)
        {

            if (!COSEHeaders.TryParseProtected(ProtectedHeaderBytes, out var protectedHeader, out var errorResponse))
                throw new COSEException($"The protected header bucket is invalid: {errorResponse}");

            this.ProtectedHeaderBytes  = ProtectedHeaderBytes;
            this.ProtectedHeader       = protectedHeader;
            this.UnprotectedHeader     = UnprotectedHeader ?? COSEHeaders.Empty;
            this.Payload               = Payload;
            this.Signature             = Signature;
            this.IsTagged              = IsTagged;

        }

        #endregion


        #region (static) ToBeSigned(ProtectedHeaderBytes, Payload, ExternalAAD = null)

        /// <summary>
        /// Return the encoded Sig_structure of a COSE_Sign1 message
        /// [RFC 9052, Section 4.4], which is the byte string an ECDSA signer
        /// actually signs:
        ///
        /// <code>
        /// Sig_structure = [
        ///     context      : "Signature1",
        ///     body_protected : empty_or_serialized_map,
        ///     external_aad : bstr,
        ///     payload      : bstr
        /// ]
        /// </code>
        ///
        /// This is public on purpose: Whenever the signing key does not live
        /// in this process - a meter, a smart card, a hardware security
        /// module - this is the input to hand over.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket.</param>
        /// <param name="Payload">The payload, also when it is transported detached.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        public static Byte[] ToBeSigned(Byte[]   ProtectedHeaderBytes,
                                        Byte[]   Payload,
                                        Byte[]?  ExternalAAD   = null)
        {

            var writer = new CBORWriter();

            writer.WriteStartArray(4);
            writer.WriteTextString(SignatureContext);
            writer.WriteByteString(ProtectedHeaderBytes);
            writer.WriteByteString(ExternalAAD ?? []);
            writer.WriteByteString(Payload);
            writer.WriteEndArray();

            return writer.ToByteArray();

        }

        #endregion

        #region ToBeSigned(ExternalAAD = null, DetachedPayload = null)

        /// <summary>
        /// Return the encoded Sig_structure of this message
        /// [RFC 9052, Section 4.4].
        /// </summary>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        public Byte[] ToBeSigned(Byte[]?  ExternalAAD       = null,
                                 Byte[]?  DetachedPayload   = null)
        {

            if (!TryGetPayload(DetachedPayload, out var payload, out var errorResponse))
                throw new COSEException(errorResponse);

            return ToBeSigned(ProtectedHeaderBytes,
                              payload,
                              ExternalAAD);

        }

        #endregion

        #region (private) TryGetPayload(DetachedPayload, out Payload, out ErrorResponse)

        /// <summary>
        /// Resolve which payload the signature is computed over: The one
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
                    ErrorResponse  = "This COSE_Sign1 message carries its payload, therefore no detached payload must be supplied!";
                    return false;
                }

                Payload        = this.Payload;
                ErrorResponse  = null;
                return true;

            }

            if (DetachedPayload is null)
            {
                Payload        = null;
                ErrorResponse  = "The payload of this COSE_Sign1 message is detached, therefore it must be supplied for the signature to be computed!";
                return false;
            }

            Payload        = DetachedPayload;
            ErrorResponse  = null;
            return true;

        }

        #endregion


        #region (static) Sign(Payload, PrivateKey, Algorithm, KeyIdentifier = null, ...)

        /// <summary>
        /// Sign the given payload, placing the algorithm within the protected
        /// header bucket and the optional key identifier within the
        /// unprotected one - the layout of the examples of RFC 9052 and of
        /// virtually every deployed COSE message.
        /// </summary>
        /// <param name="Payload">The payload to sign.</param>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="Algorithm">The signature algorithm.</param>
        /// <param name="KeyIdentifier">An optional key identifier.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachPayload">Whether to omit the payload from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 18.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        public static COSESign1 Sign(Byte[]                  Payload,
                                     ECPrivateKeyParameters  PrivateKey,
                                     COSEAlgorithm           Algorithm,
                                     Byte[]?                 KeyIdentifier   = null,
                                     Byte[]?                 ExternalAAD     = null,
                                     Boolean                 DetachPayload   = false,
                                     Boolean                 Tagged          = true,
                                     SecureRandom?           Random          = null)

            => Sign(Payload,
                    PrivateKey,
                    COSEHeaders.Create(Algorithm),
                    KeyIdentifier is not null
                        ? COSEHeaders.Create(null, KeyIdentifier)
                        : COSEHeaders.Empty,
                    ExternalAAD,
                    DetachPayload,
                    Tagged,
                    Random);

        #endregion

        #region (static) SignWithApplicationAlgorithm(Payload, PrivateKey, Algorithm, KeyIdentifier = null, ...)

        /// <summary>
        /// Sign the given payload with the algorithm taken from the
        /// application context rather than from the message: the protected
        /// header bucket stays empty and only the key identifier travels.
        ///
        /// This is the leanest signed COSE message there is, and for a
        /// protocol that already agrees on its algorithm it is also the
        /// safest arrangement: an algorithm that is not in the message can
        /// not be tampered with on the way, whereas one within the
        /// unprotected bucket can. The price is that the algorithm becomes a
        /// property of the profile and of the key rather than of the message,
        /// so agility means changing the profile.
        ///
        /// The verifier has to name the expected algorithm as well - either
        /// explicitly, or by verifying with a COSE key that carries it.
        /// </summary>
        /// <param name="Payload">The payload to sign.</param>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="Algorithm">The signature algorithm, known to both sides out of band.</param>
        /// <param name="KeyIdentifier">An optional key identifier, e.g. the leading bytes of the COSE Key Thumbprint of the signing key.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachPayload">Whether to omit the payload from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 18.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        public static COSESign1 SignWithApplicationAlgorithm(Byte[]                  Payload,
                                                             ECPrivateKeyParameters  PrivateKey,
                                                             COSEAlgorithm           Algorithm,
                                                             Byte[]?                 KeyIdentifier   = null,
                                                             Byte[]?                 ExternalAAD     = null,
                                                             Boolean                 DetachPayload   = false,
                                                             Boolean                 Tagged          = true,
                                                             SecureRandom?           Random          = null)

            => Sign(Payload,
                    PrivateKey,
                    COSEHeaders.Empty,
                    KeyIdentifier is not null
                        ? COSEHeaders.Create(null, KeyIdentifier)
                        : COSEHeaders.Empty,
                    ExternalAAD,
                    DetachPayload,
                    Tagged,
                    Random,
                    Algorithm);

        #endregion

        #region (static) Sign(Payload, PrivateKey, ProtectedHeader, UnprotectedHeader = null, ...)

        /// <summary>
        /// Sign the given payload with full control over both header buckets.
        /// The signature algorithm is taken from the protected bucket, as an
        /// algorithm that is not covered by the signature could be changed by
        /// anyone on the way.
        /// </summary>
        /// <param name="Payload">The payload to sign.</param>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="ProtectedHeader">The protected header parameters, which must name the signature algorithm.</param>
        /// <param name="UnprotectedHeader">The optional unprotected header parameters.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachPayload">Whether to omit the payload from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 18.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        public static COSESign1 Sign(Byte[]                  Payload,
                                     ECPrivateKeyParameters  PrivateKey,
                                     COSEHeaders             ProtectedHeader,
                                     COSEHeaders?            UnprotectedHeader   = null,
                                     Byte[]?                 ExternalAAD         = null,
                                     Boolean                 DetachPayload         = false,
                                     Boolean                 Tagged                = true,
                                     SecureRandom?           Random                = null,
                                     COSEAlgorithm?          ApplicationAlgorithm  = null)
        {

            if (ApplicationAlgorithm.HasValue        &&
                ProtectedHeader.Algorithm.HasValue   &&
                ProtectedHeader.Algorithm.Value != ApplicationAlgorithm.Value)
            {
                throw new COSEException($"The protected header bucket names the algorithm '{ProtectedHeader.Algorithm.Value.Name}', but the application context names '{ApplicationAlgorithm.Value.Name}'!");
            }

            var algorithm       = ProtectedHeader.Algorithm
                                      ?? ApplicationAlgorithm
                                      ?? throw new COSEException("Neither the protected header bucket nor the application context names the signature algorithm!");

            var protectedBytes  = ProtectedHeader.ToProtectedByteArray();

            var signature       = algorithm.Sign(
                                      ToBeSigned(protectedBytes, Payload, ExternalAAD),
                                      PrivateKey,
                                      Random
                                  );

            return new COSESign1(
                       protectedBytes,
                       UnprotectedHeader,
                       DetachPayload ? null : Payload,
                       signature,
                       Tagged
                   );

        }

        #endregion

        #region (static) Sign(Payload, Key, ...)

        /// <summary>
        /// Sign the given payload with the given COSE key, taking the
        /// signature algorithm and the key identifier from the key itself.
        /// </summary>
        /// <param name="Payload">The payload to sign.</param>
        /// <param name="Key">A COSE key holding private key material, an algorithm and an optional key identifier.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachPayload">Whether to omit the payload from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 18.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        public static COSESign1 Sign(Byte[]          Payload,
                                     COSEKey         Key,
                                     Byte[]?         ExternalAAD     = null,
                                     Boolean         DetachPayload   = false,
                                     Boolean         Tagged          = true,
                                     SecureRandom?   Random          = null)
        {

            var algorithm = Key.Algorithm
                                ?? throw new COSEException("The COSE key does not name the signature algorithm to use!");

            return Sign(Payload,
                        Key.ToPrivateKey(),
                        algorithm,
                        Key.KeyIdentifier,
                        ExternalAAD,
                        DetachPayload,
                        Tagged,
                        Random);

        }

        #endregion

        #region Verify(PublicKey, ExternalAAD = null, DetachedPayload = null, ExpectedAlgorithm = null)

        /// <summary>
        /// Verify the signature of this message.
        /// </summary>
        /// <param name="PublicKey">An elliptic curve public key.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that was signed along with the payload.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="ExpectedAlgorithm">The signature algorithm the caller expects, required whenever the message states its algorithm within the unprotected header bucket only.</param>
        public Boolean Verify(ECPublicKeyParameters  PublicKey,
                              Byte[]?                ExternalAAD         = null,
                              Byte[]?                DetachedPayload     = null,
                              COSEAlgorithm?         ExpectedAlgorithm   = null)

            => Verify(PublicKey,
                      out _,
                      ExternalAAD,
                      DetachedPayload,
                      ExpectedAlgorithm);

        #endregion

        #region Verify(PublicKey, out ErrorResponse, ExternalAAD = null, DetachedPayload = null, ExpectedAlgorithm = null)

        /// <summary>
        /// Verify the signature of this message and report why it failed.
        /// A failed verification is not an exception: It is the expected
        /// outcome of checking untrusted data.
        /// </summary>
        /// <param name="PublicKey">An elliptic curve public key.</param>
        /// <param name="ErrorResponse">The reason why the verification failed.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that was signed along with the payload.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="ExpectedAlgorithm">The signature algorithm the caller expects, required whenever the message states its algorithm within the unprotected header bucket only.</param>
        public Boolean Verify(ECPublicKeyParameters             PublicKey,
                              [NotNullWhen(false)] out String?  ErrorResponse,
                              Byte[]?                           ExternalAAD         = null,
                              Byte[]?                           DetachedPayload     = null,
                              COSEAlgorithm?                    ExpectedAlgorithm   = null)
        {

            #region The header parameters a recipient must understand

            if (!COSEHeaders.VerifyCriticalHeaderParameters(ProtectedHeader,
                                                            UnprotectedHeader,
                                                            out ErrorResponse))
            {
                return false;
            }

            #endregion

            #region Which algorithm to verify with

            var algorithm = ExpectedAlgorithm ?? ProtectedHeader.Algorithm;

            if (!algorithm.HasValue)
            {

                ErrorResponse = UnprotectedHeader.Algorithm.HasValue
                                    ? $"This COSE_Sign1 message states its algorithm '{UnprotectedHeader.Algorithm.Value.Name}' within the unprotected header bucket only, where it is not covered by the signature: Pass the expected algorithm explicitly in order to accept it!"
                                    :  "This COSE_Sign1 message does not state its signature algorithm: Pass the expected algorithm explicitly!";

                return false;

            }

            var statedAlgorithm = Algorithm;

            if (statedAlgorithm.HasValue &&
                statedAlgorithm.Value != algorithm.Value)
            {
                ErrorResponse = $"This COSE_Sign1 message was signed with the algorithm '{statedAlgorithm.Value.Name}', but the algorithm '{algorithm.Value.Name}' was expected!";
                return false;
            }

            #endregion

            #region The signature itself

            if (!TryGetPayload(DetachedPayload, out var payload, out ErrorResponse))
                return false;

            return algorithm.Value.Verify(
                       ToBeSigned(ProtectedHeaderBytes, payload, ExternalAAD),
                       Signature,
                       PublicKey,
                       out ErrorResponse
                   );

            #endregion

        }

        #endregion

        #region Verify(Key, out ErrorResponse, ExternalAAD = null, DetachedPayload = null)

        /// <summary>
        /// Verify the signature of this message with the given COSE key.
        /// </summary>
        /// <param name="Key">A COSE key.</param>
        /// <param name="ErrorResponse">The reason why the verification failed.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that was signed along with the payload.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        public Boolean Verify(COSEKey                           Key,
                              [NotNullWhen(false)] out String?  ErrorResponse,
                              Byte[]?                           ExternalAAD       = null,
                              Byte[]?                           DetachedPayload   = null)
        {

            if (!Key.TryToPublicKey(out var publicKey, out ErrorResponse))
                return false;

            return Verify(publicKey,
                          out ErrorResponse,
                          ExternalAAD,
                          DetachedPayload,
                          Key.Algorithm);

        }

        #endregion



        #region Countersignatures

        /// <summary>
        /// The countersignatures of this message [RFC 9338], i.e. the
        /// signatures OF ITS SIGNATURE, taken from the "Countersignature
        /// version 2" header parameter (label 11) of the unprotected bucket.
        ///
        /// A countersignature endorses an existing signature without
        /// re-wrapping the message: the payload stays the payload, the body
        /// signature stays byte for byte what it was, and whoever only knows
        /// the original signer can still verify it. It is therefore what a
        /// party who vouches for someone else's signature produces, whereas a
        /// party who asserts a statement of their own signs a payload of
        /// their own.
        ///
        /// A single countersignature is written bare, several are written as
        /// an array of them. Throws when the header parameter is present but
        /// not a countersignature, rather than pretending there is none.
        /// </summary>
        public IReadOnlyList<COSESignature>  Countersignatures
        {
            get
            {

                if (!UnprotectedHeader.TryGet(COSEHeaderLabel.CounterSignatureV2, out var value))
                    return [];

                if (value.Kind != CBORValueKind.Array)
                    throw new COSEException("The countersignature header parameter must be a COSE_Countersignature or an array of them!");

                var items = value.AsArray();

                // The first element tells the two shapes apart: a single
                // countersignature starts with its protected bucket, a byte
                // string, whereas an array of them starts with an array.
                if (items.Count > 0 && items[0].Kind == CBORValueKind.Array)
                {

                    var countersignatures = new List<COSESignature>();

                    foreach (var item in items)
                    {

                        if (!COSESignature.TryParse(item, out var countersignature, out var itemError))
                            throw new COSEException($"A countersignature of this COSE_Sign1 message is invalid: {itemError}");

                        countersignatures.Add(countersignature);

                    }

                    return countersignatures;

                }

                if (!COSESignature.TryParse(value, out var single, out var errorResponse))
                    throw new COSEException($"The countersignature of this COSE_Sign1 message is invalid: {errorResponse}");

                return [single];

            }
        }

        #endregion

        #region (static) ToBeCountersigned(BodyProtectedHeaderBytes, CountersignatureProtectedHeaderBytes, Payload, Signature, ExternalAAD = null)

        /// <summary>
        /// Return the encoded Countersign_structure of a countersignature on
        /// a COSE_Sign1 message [RFC 9338, Section 3.3]:
        ///
        /// <code>
        /// Countersign_structure = [
        ///     context        : "CounterSignatureV2",
        ///     body_protected : empty_or_serialized_map,
        ///     sign_protected : empty_or_serialized_map,
        ///     external_aad   : bstr,
        ///     payload        : bstr,
        ///     other_fields   : [ signature ]
        /// ]
        /// </code>
        ///
        /// The last element is what makes this the version 2 of the
        /// structure, and it is the whole point of RFC 9338: the
        /// countersignature of RFC 8152 covered the payload but NOT the
        /// signature it was supposed to countersign, so it did not actually
        /// attest to having seen it.
        /// </summary>
        /// <param name="BodyProtectedHeaderBytes">The serialized protected header bucket of the countersigned message.</param>
        /// <param name="CountersignatureProtectedHeaderBytes">The serialized protected header bucket of the countersignature itself.</param>
        /// <param name="Payload">The payload of the countersigned message, also when it is transported detached.</param>
        /// <param name="Signature">The signature of the countersigned message.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        public static Byte[] ToBeCountersigned(Byte[]   BodyProtectedHeaderBytes,
                                               Byte[]   CountersignatureProtectedHeaderBytes,
                                               Byte[]   Payload,
                                               Byte[]   Signature,
                                               Byte[]?  ExternalAAD   = null)
        {

            var writer = new CBORWriter();

            writer.WriteStartArray(6);
            writer.WriteTextString(CountersignatureContext);
            writer.WriteByteString(BodyProtectedHeaderBytes);
            writer.WriteByteString(CountersignatureProtectedHeaderBytes);
            writer.WriteByteString(ExternalAAD ?? []);
            writer.WriteByteString(Payload);
            writer.WriteStartArray(1);
            writer.WriteByteString(Signature);
            writer.WriteEndArray();
            writer.WriteEndArray();

            return writer.ToByteArray();

        }

        #endregion

        #region ToBeCountersigned(Countersignature, ExternalAAD = null, DetachedPayload = null)

        /// <summary>
        /// Return the encoded Countersign_structure of the given
        /// countersignature on this message [RFC 9338, Section 3.3].
        /// </summary>
        /// <param name="Countersignature">A countersignature on this message.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        public Byte[] ToBeCountersigned(COSESignature  Countersignature,
                                        Byte[]?        ExternalAAD       = null,
                                        Byte[]?        DetachedPayload   = null)
        {

            if (!TryGetPayload(DetachedPayload, out var payload, out var errorResponse))
                throw new COSEException(errorResponse);

            return ToBeCountersigned(ProtectedHeaderBytes,
                                     Countersignature.ProtectedHeaderBytes,
                                     payload,
                                     Signature,
                                     ExternalAAD);

        }

        #endregion

        #region AddCountersignature(PrivateKey, Algorithm, KeyIdentifier = null, ...)

        /// <summary>
        /// Return a copy of this message endorsed by one more countersignature.
        /// The body signature keeps its bytes and stays valid, because
        /// countersignatures live within the UNPROTECTED header bucket, which
        /// no signature covers.
        /// </summary>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="Algorithm">The signature algorithm.</param>
        /// <param name="KeyIdentifier">An optional key identifier.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        public COSESign1 AddCountersignature(ECPrivateKeyParameters  PrivateKey,
                                             COSEAlgorithm           Algorithm,
                                             Byte[]?                 KeyIdentifier     = null,
                                             Byte[]?                 ExternalAAD       = null,
                                             Byte[]?                 DetachedPayload   = null,
                                             SecureRandom?           Random            = null)

            => AddCountersignature(PrivateKey,
                                   COSEHeaders.Create(Algorithm),
                                   KeyIdentifier is not null
                                       ? COSEHeaders.Create(null, KeyIdentifier)
                                       : COSEHeaders.Empty,
                                   ExternalAAD,
                                   DetachedPayload,
                                   Random);

        #endregion

        #region AddCountersignature(PrivateKey, CountersignatureProtectedHeader, ...)

        /// <summary>
        /// Return a copy of this message endorsed by one more
        /// countersignature, with full control over its header buckets.
        /// </summary>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="CountersignatureProtectedHeader">The protected header parameters of the countersignature, which must name the signature algorithm.</param>
        /// <param name="CountersignatureUnprotectedHeader">The optional unprotected header parameters of the countersignature.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is signed along with the payload without being transported within the message.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        public COSESign1 AddCountersignature(ECPrivateKeyParameters  PrivateKey,
                                             COSEHeaders             CountersignatureProtectedHeader,
                                             COSEHeaders?            CountersignatureUnprotectedHeader   = null,
                                             Byte[]?                 ExternalAAD                         = null,
                                             Byte[]?                 DetachedPayload                     = null,
                                             SecureRandom?           Random                              = null)
        {

            var algorithm                     = CountersignatureProtectedHeader.Algorithm
                                                    ?? throw new COSEException("The protected header bucket of the countersignature must name the signature algorithm!");

            if (!TryGetPayload(DetachedPayload, out var payload, out var errorResponse))
                throw new COSEException(errorResponse);

            var countersignatureProtectedBytes  = CountersignatureProtectedHeader.ToProtectedByteArray();

            var countersignature                = new COSESignature(
                                                      countersignatureProtectedBytes,
                                                      CountersignatureUnprotectedHeader,
                                                      algorithm.Sign(
                                                          ToBeCountersigned(ProtectedHeaderBytes,
                                                                            countersignatureProtectedBytes,
                                                                            payload,
                                                                            Signature,
                                                                            ExternalAAD),
                                                          PrivateKey,
                                                          Random
                                                      )
                                                  );

            var all                             = new List<COSESignature>(Countersignatures) {
                                                      countersignature
                                                  };

            return new COSESign1(
                       ProtectedHeaderBytes,
                       UnprotectedHeader.Set(
                           COSEHeaderLabel.CounterSignatureV2,
                           all.Count == 1
                               ? all[0].ToCBOR()
                               : CBORValue.FromArray(all.Select(static countersignature => countersignature.ToCBOR()))
                       ),
                       Payload,
                       Signature,
                       IsTagged
                   );

        }

        #endregion

        #region VerifyCountersignature(Countersignature, PublicKey, out ErrorResponse, ...)

        /// <summary>
        /// Verify a countersignature on this message [RFC 9338].
        /// Verifying it says that its signer saw this exact body signature -
        /// it says nothing about whether the body signature itself is valid,
        /// which is a separate question answered by Verify(...).
        /// </summary>
        /// <param name="Countersignature">A countersignature on this message.</param>
        /// <param name="PublicKey">An elliptic curve public key.</param>
        /// <param name="ErrorResponse">The reason why the verification failed.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that was signed along with the payload.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="ExpectedAlgorithm">The signature algorithm the caller expects, required whenever the countersignature states its algorithm within its unprotected header bucket only.</param>
        public Boolean VerifyCountersignature(COSESignature                     Countersignature,
                                              ECPublicKeyParameters             PublicKey,
                                              [NotNullWhen(false)] out String?  ErrorResponse,
                                              Byte[]?                           ExternalAAD         = null,
                                              Byte[]?                           DetachedPayload     = null,
                                              COSEAlgorithm?                    ExpectedAlgorithm   = null)
        {

            if (!COSEHeaders.VerifyCriticalHeaderParameters(Countersignature.ProtectedHeader,
                                                            Countersignature.UnprotectedHeader,
                                                            out ErrorResponse))
            {
                return false;
            }

            var algorithm = ExpectedAlgorithm ?? Countersignature.ProtectedHeader.Algorithm;

            if (!algorithm.HasValue)
            {

                ErrorResponse = Countersignature.UnprotectedHeader.Algorithm.HasValue
                                    ? $"This countersignature states its algorithm '{Countersignature.UnprotectedHeader.Algorithm.Value.Name}' within the unprotected header bucket only, where it is not covered by the countersignature: Pass the expected algorithm explicitly in order to accept it!"
                                    :  "This countersignature does not state its algorithm: Pass the expected algorithm explicitly!";

                return false;

            }

            var statedAlgorithm = Countersignature.Algorithm;

            if (statedAlgorithm.HasValue &&
                statedAlgorithm.Value != algorithm.Value)
            {
                ErrorResponse = $"This countersignature was created with the algorithm '{statedAlgorithm.Value.Name}', but the algorithm '{algorithm.Value.Name}' was expected!";
                return false;
            }

            if (!TryGetPayload(DetachedPayload, out var payload, out ErrorResponse))
                return false;

            return algorithm.Value.Verify(
                       ToBeCountersigned(ProtectedHeaderBytes,
                                         Countersignature.ProtectedHeaderBytes,
                                         payload,
                                         Signature,
                                         ExternalAAD),
                       Countersignature.Signature,
                       PublicKey,
                       out ErrorResponse
                   );

        }

        #endregion


        #region (static) Parse   (CBOR)

        /// <summary>
        /// Parse the given CBOR value as a COSE_Sign1 message.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Sign1 message.</param>
        public static COSESign1 Parse(CBORValue CBOR)
        {

            if (TryParse(CBOR, out var sign1, out var errorResponse))
                return sign1;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region (static) Parse   (Data)

        /// <summary>
        /// Parse the given CBOR data as a COSE_Sign1 message.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE_Sign1 message.</param>
        public static COSESign1 Parse(ReadOnlySpan<Byte> Data)
        {

            if (TryParse(Data, out var sign1, out var errorResponse))
                return sign1;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region (static) TryParse(Data, out Sign1, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR data as a COSE_Sign1 message.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE_Sign1 message.</param>
        /// <param name="Sign1">The parsed COSE_Sign1 message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte>                   Data,
                                       [NotNullWhen(true)]  out COSESign1?  Sign1,
                                       [NotNullWhen(false)] out String?     ErrorResponse)
        {

            if (!CBORValue.TryParse(Data, out var cbor, out ErrorResponse))
            {
                Sign1 = null;
                return false;
            }

            return TryParse(cbor, out Sign1, out ErrorResponse);

        }

        #endregion

        #region (static) TryParse(CBOR, out Sign1, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR value as a COSE_Sign1 message.
        /// Both the tagged and the untagged form are accepted; which one it
        /// was is remembered, as the CBOR tag is not covered by the signature
        /// but is part of the bytes on the wire.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Sign1 message.</param>
        /// <param name="Sign1">The parsed COSE_Sign1 message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                            CBOR,
                                       [NotNullWhen(true)]  out COSESign1?  Sign1,
                                       [NotNullWhen(false)] out String?     ErrorResponse)
        {

            Sign1 = null;

            var isTagged  = false;
            var message   = CBOR;

            if (message.Kind == CBORValueKind.Tagged)
            {

                if (message.Tag != CBORTag.COSESign1)
                {
                    ErrorResponse = $"A COSE_Sign1 message must be tagged with CBOR tag {CBORTag.COSESign1.Value}, but was tagged with CBOR tag {message.Tag.Value}!";
                    return false;
                }

                isTagged  = true;
                message   = message.UntaggedValue;

            }

            if (message.Kind != CBORValueKind.Array)
            {
                ErrorResponse = $"A COSE_Sign1 message must be a CBOR array, but was a CBOR {message.Kind}!";
                return false;
            }

            if (message.Count != 4)
            {
                ErrorResponse = $"A COSE_Sign1 message must be a CBOR array of 4 elements, but had {message.Count} element(s)!";
                return false;
            }

            var items = message.AsArray();

            if (!items[0].TryGetBytes(out var protectedHeaderBytes))
            {
                ErrorResponse = "The protected header bucket of a COSE_Sign1 message must be a byte string!";
                return false;
            }

            if (!COSEHeaders.TryParse(items[1], out var unprotectedHeader, out ErrorResponse))
                return false;

            Byte[]? payload = null;

            if (items[2].Kind != CBORValueKind.Null &&
                !items[2].TryGetBytes(out payload))
            {
                ErrorResponse = "The payload of a COSE_Sign1 message must be a byte string, or null when it is detached!";
                return false;
            }

            if (!items[3].TryGetBytes(out var signature))
            {
                ErrorResponse = "The signature of a COSE_Sign1 message must be a byte string!";
                return false;
            }

            try
            {

                Sign1 = new COSESign1(
                            protectedHeaderBytes,
                            unprotectedHeader,
                            payload,
                            signature,
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
        /// Return a CBOR representation of this COSE_Sign1 message.
        /// </summary>
        public CBORValue ToCBOR()
        {

            var message = CBORValue.FromArray(
                              CBORValue.FromBytes(ProtectedHeaderBytes),
                              UnprotectedHeader.ToCBOR(),
                              Payload is not null
                                  ? CBORValue.FromBytes(Payload)
                                  : CBORValue.Null,
                              CBORValue.FromBytes(Signature)
                          );

            return IsTagged
                       ? message.WithTag(CBORTag.COSESign1)
                       : message;

        }

        #endregion

        #region ToByteArray(Options = null)

        /// <summary>
        /// Return the CBOR encoding of this COSE_Sign1 message.
        /// </summary>
        /// <param name="Options">Optional CBOR writer options.</param>
        public Byte[] ToByteArray(CBORWriterOptions? Options = null)

            => ToCBOR().ToByteArray(Options);

        #endregion

        #region Detach()

        /// <summary>
        /// Return a copy of this message without its payload, e.g. because
        /// the payload is transported elsewhere. The signature stays valid:
        /// It never covered the message, only the Sig_structure.
        /// </summary>
        public COSESign1 Detach()

            => new (ProtectedHeaderBytes,
                    UnprotectedHeader,
                    null,
                    Signature,
                    IsTagged);

        #endregion

        #region Attach(Payload)

        /// <summary>
        /// Return a copy of this message carrying the given payload, e.g. in
        /// order to archive a detached message together with the data it
        /// signs. Whether the payload actually belongs to the signature is
        /// decided by verifying it, not by attaching it.
        /// </summary>
        /// <param name="Payload">The payload to attach.</param>
        public COSESign1 Attach(Byte[] Payload)

            => new (ProtectedHeaderBytes,
                    UnprotectedHeader,
                    Payload,
                    Signature,
                    IsTagged);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   "COSE_Sign1: ",

                   Algorithm.HasValue
                       ? Algorithm.Value.Name
                       : "without an algorithm",

                   IsDetached
                       ? ", detached payload"
                       : $", {Payload!.Length} byte(s) of payload",

                   KeyIdentifier is not null
                       ? $", key '{KeyIdentifier.ToHexString()}'"
                       : "",

                   $", {Signature.Length} byte(s) of signature"

               );

        #endregion

    }

}
