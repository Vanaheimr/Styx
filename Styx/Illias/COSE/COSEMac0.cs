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
    /// A COSE_Mac0 message [RFC 9052, Section 6.2]: A payload authenticated
    /// with a key that both parties hold, tagged with CBOR tag 17.
    ///
    /// <code>
    /// COSE_Mac0 = [
    ///     protected   : bstr .cbor header_map,   ; covered by the tag
    ///     unprotected : header_map,              ; NOT covered by the tag
    ///     payload     : bstr / nil,              ; nil = detached
    ///     tag         : bstr
    /// ]
    /// </code>
    ///
    /// It is the structural twin of COSESign1, deliberately so: four elements
    /// in the same order, CBOR tag 17 against 18, and a MAC_structure that
    /// differs from the Sig_structure in one string - "MAC0" where the other
    /// says "Signature1". Everything the signature code learned applies
    /// unchanged: the protected bucket is kept verbatim, the CBOR tag is not
    /// covered, the payload may be detached, and externally supplied data is
    /// authenticated without travelling.
    ///
    /// WHAT IS NOT THE SAME is what a verified message means.
    ///
    /// A signature says "the holder of that private key produced this", to
    /// anybody who cares to check. A tag says "someone holding the shared key
    /// produced this", and it says it only to someone who holds that key too -
    /// because verifying one requires the very key that creates one. Between
    /// two parties that is still useful: each knows the other made it, having
    /// not made it themselves. Towards a third party it is worth nothing at
    /// all, and a party who later denies having produced a message cannot be
    /// contradicted with a tag.
    ///
    /// That is why a metrological record is SIGNED. The customer, the operator
    /// and the regulator all have to be able to check a reading, and none of
    /// them may be able to manufacture one. A MAC belongs where the two ends
    /// of a link already share a secret and want cheap tamper detection -
    /// eight bytes and one hash, against sixty-four bytes and a curve
    /// multiplication - with the durable evidence carried by a signature
    /// underneath it. COSE nests, so both can travel at once.
    ///
    /// Only HMAC is implemented [RFC 9053, Section 3.1]. AES-CBC-MAC is
    /// registered too and is deliberately absent: raw CBC-MAC is secure only
    /// for messages of a fixed length, and RFC 9053 Section 3.2.1 says so
    /// itself - its safety within COSE rests on the MAC_structure encoding the
    /// lengths, not on the primitive. HMAC needs no such argument. (Note also
    /// that RFC 9052's own Appendix C.6.1 describes algorithm 15 as "AES-CMAC"
    /// while RFC 9053 Section 3.2 states outright that AES-CBC-MAC is NOT
    /// AES-CMAC. The identifier is CBC-MAC; the prose of the other RFC is
    /// wrong.)
    ///
    /// The "tag" field is an AUTHENTICATION tag and has nothing to do with the
    /// CBOR tag 17 this message is wrapped in. RFC 9052 uses the word for both.
    /// </summary>
    public sealed class COSEMac0
    {

        #region Data

        /// <summary>
        /// The context text string of a COSE_Mac0 authentication tag
        /// [RFC 9052, Section 6.3].
        /// </summary>
        public const String MAC0Context = "MAC0";

        #endregion

        #region Properties

        /// <summary>
        /// The serialized protected header bucket, exactly as authenticated
        /// and as received: A zero-length byte string when there are no
        /// protected header parameters.
        /// </summary>
        public Byte[]          ProtectedHeaderBytes    { get; }

        /// <summary>
        /// The protected header parameters, which the authentication tag
        /// covers.
        /// </summary>
        public COSEHeaders     ProtectedHeader         { get; }

        /// <summary>
        /// The unprotected header parameters, which the authentication tag
        /// does NOT cover and which therefore must not be trusted after a
        /// successful verification.
        /// </summary>
        public COSEHeaders     UnprotectedHeader       { get; }

        /// <summary>
        /// The authenticated payload, or null when the payload is detached and
        /// thus transported outside of this message.
        /// </summary>
        public Byte[]?         Payload                 { get; }

        /// <summary>
        /// The authentication tag - not the CBOR tag.
        /// </summary>
        public Byte[]          Tag                     { get; }

        /// <summary>
        /// Whether this message is wrapped within CBOR tag 17. The tag is not
        /// covered by the authentication tag, but it is preserved so that a
        /// parsed message re-encodes to the very same bytes.
        /// </summary>
        public Boolean         IsTagged                { get; }

        /// <summary>
        /// Whether the payload is detached.
        /// </summary>
        public Boolean         IsDetached

            => Payload is null;

        /// <summary>
        /// The MAC algorithm, taken from the protected header bucket and only
        /// otherwise from the unprotected one. Verification does not silently
        /// trust an algorithm that is not integrity protected.
        /// </summary>
        public COSEAlgorithm?  Algorithm

            => ProtectedHeader.  Algorithm ??
               UnprotectedHeader.Algorithm;

        /// <summary>
        /// The key identifier, taken from the protected header bucket and only
        /// otherwise from the unprotected one.
        /// </summary>
        public Byte[]?         KeyIdentifier

            => ProtectedHeader.  KeyIdentifier ??
               UnprotectedHeader.KeyIdentifier;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new COSE_Mac0 message from its parts.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket, which the authentication tag covers byte by byte.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters.</param>
        /// <param name="Payload">The authenticated payload, or null when detached.</param>
        /// <param name="Tag">The authentication tag.</param>
        /// <param name="IsTagged">Whether the message is wrapped within CBOR tag 17.</param>
        public COSEMac0(Byte[]        ProtectedHeaderBytes,
                        COSEHeaders?  UnprotectedHeader,
                        Byte[]?       Payload,
                        Byte[]        Tag,
                        Boolean       IsTagged   = true)
        {

            if (!COSEHeaders.TryParseProtected(ProtectedHeaderBytes, out var protectedHeader, out var errorResponse))
                throw new COSEException($"The protected header bucket is invalid: {errorResponse}");

            this.ProtectedHeaderBytes  = ProtectedHeaderBytes;
            this.ProtectedHeader       = protectedHeader;
            this.UnprotectedHeader     = UnprotectedHeader ?? COSEHeaders.Empty;
            this.Payload               = Payload;
            this.Tag                   = Tag;
            this.IsTagged              = IsTagged;

        }

        #endregion


        #region (static) ToBeMACed(ProtectedHeaderBytes, Payload, ExternalAAD = null)

        /// <summary>
        /// Return the encoded MAC_structure [RFC 9052, Section 6.3], which is
        /// the byte string the MAC is actually computed over:
        ///
        /// <code>
        /// MAC_structure = [
        ///     context      : "MAC0",
        ///     protected    : empty_or_serialized_map,
        ///     external_aad : bstr,
        ///     payload      : bstr
        /// ]
        /// </code>
        ///
        /// The FULL payload goes in here regardless of how it travels, so a
        /// detached message and an attached one carry the very same tag.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket, verbatim.</param>
        /// <param name="Payload">The payload to authenticate.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is authenticated along with the payload without being transported within the message.</param>
        public static Byte[] ToBeMACed(Byte[]   ProtectedHeaderBytes,
                                       Byte[]   Payload,
                                       Byte[]?  ExternalAAD   = null)
        {

            var writer = new CBORWriter();

            writer.WriteStartArray(4);
            writer.WriteTextString(MAC0Context);
            writer.WriteByteString(ProtectedHeaderBytes);
            writer.WriteByteString(ExternalAAD ?? []);
            writer.WriteByteString(Payload);
            writer.WriteEndArray();

            return writer.ToByteArray();

        }

        #endregion

        #region ToBeMACed(ExternalAAD = null, DetachedPayload = null)

        /// <summary>
        /// Return the encoded MAC_structure of this message.
        /// </summary>
        /// <param name="ExternalAAD">Optional externally supplied data that is authenticated along with the payload.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        public Byte[] ToBeMACed(Byte[]?  ExternalAAD       = null,
                                Byte[]?  DetachedPayload   = null)
        {

            if (!TryGetPayload(DetachedPayload, out var payload, out var errorResponse))
                throw new COSEException(errorResponse);

            return ToBeMACed(ProtectedHeaderBytes, payload, ExternalAAD);

        }

        #endregion


        #region (static) Create(Payload, Key, ExternalAAD = null, ...)

        /// <summary>
        /// Authenticate the given payload with the given symmetric COSE key,
        /// placing the algorithm within the protected header bucket and the
        /// key identifier within the unprotected one.
        /// </summary>
        /// <param name="Payload">The payload to authenticate.</param>
        /// <param name="Key">A symmetric COSE key naming the MAC algorithm.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is authenticated along with the payload without being transported within the message.</param>
        /// <param name="DetachPayload">Whether to omit the payload from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 17.</param>
        /// <param name="CanonicalizePayload">Whether to rewrite a CBOR payload in the deterministic encoding of RFC 8949, Section 4.2.1 before authenticating it, so that a receiver who parses and re-serializes it arrives at the very bytes this tag covers. A payload that is not CBOR is authenticated as it is.</param>
        public static COSEMac0 Create(Byte[]    Payload,
                                      COSEKey   Key,
                                      Byte[]?   ExternalAAD           = null,
                                      Boolean   DetachPayload         = false,
                                      Boolean   Tagged                = true,
                                      Boolean   CanonicalizePayload   = true)
        {

            var algorithm = Key.Algorithm
                                ?? throw new COSEException("A COSE_Mac0 message needs a MAC algorithm: either on the key or within the protected header bucket!");

            return Create(Payload,
                          Key,
                          COSEHeaders.Create(algorithm),
                          Key.KeyIdentifier is not null
                              ? COSEHeaders.Create(null, Key.KeyIdentifier)
                              : COSEHeaders.Empty,
                          ExternalAAD,
                          DetachPayload,
                          Tagged,
                          CanonicalizePayload);

        }

        #endregion

        #region (static) Create(Payload, Key, ProtectedHeader, UnprotectedHeader = null, ...)

        /// <summary>
        /// Authenticate the given payload with header buckets the caller
        /// composed.
        /// </summary>
        /// <param name="Payload">The payload to authenticate.</param>
        /// <param name="Key">A symmetric COSE key.</param>
        /// <param name="ProtectedHeader">The protected header parameters, which the authentication tag covers.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is authenticated along with the payload without being transported within the message.</param>
        /// <param name="DetachPayload">Whether to omit the payload from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 17.</param>
        /// <param name="CanonicalizePayload">Whether to rewrite a CBOR payload in the deterministic encoding of RFC 8949, Section 4.2.1 before authenticating it, so that a receiver who parses and re-serializes it arrives at the very bytes this tag covers. A payload that is not CBOR is authenticated as it is.</param>
        public static COSEMac0 Create(Byte[]        Payload,
                                      COSEKey       Key,
                                      COSEHeaders   ProtectedHeader,
                                      COSEHeaders?  UnprotectedHeader     = null,
                                      Byte[]?       ExternalAAD           = null,
                                      Boolean       DetachPayload         = false,
                                      Boolean       Tagged                = true,
                                      Boolean       CanonicalizePayload   = true)
        {

            var algorithm = ProtectedHeader.Algorithm
                                ?? Key.Algorithm
                                ?? throw new COSEException("A COSE_Mac0 message needs a MAC algorithm: either on the key or within the protected header bucket!");

            EnsureUsable(Key, algorithm);

            var protectedHeaderBytes = ProtectedHeader.ToProtectedByteArray();

            var payload = CanonicalizePayload
                              ? COSEPayload.Canonicalize(Payload)
                              : Payload;

            // A MAC dies the same death as a signature: a receiver that
            // decodes the payload and encodes it again produces the
            // deterministic spelling, and a tag over another one no longer
            // verifies. A detached payload is the caller's to transmit, so
            // rewriting it here would authenticate bytes nobody holds.
            if (DetachPayload && !payload.AsSpan().SequenceEqual(Payload))
                throw new COSEException("The payload of this COSE_Mac0 message is detached, so canonicalizing it here would authenticate bytes that nobody holds: Canonicalize the payload yourself (COSEPayload.Canonicalize), authenticate and transmit those, or pass CanonicalizePayload: false to authenticate the payload exactly as it is!");

            var tag = algorithm.ComputeMAC(
                          ToBeMACed(protectedHeaderBytes, payload, ExternalAAD),
                          Key.K!
                      );

            return new COSEMac0(
                       protectedHeaderBytes,
                       UnprotectedHeader,
                       DetachPayload ? null : payload,
                       tag,
                       Tagged
                   );

        }

        #endregion

        #region (private static) EnsureUsable(Key, Algorithm)

        /// <summary>
        /// The key checks of RFC 9053, Section 3.1.
        ///
        /// The last one is the one worth having. A key that names
        /// HMAC 256/256 being used to produce an HMAC 256/64 tag is a
        /// truncation nobody asked for, and it is exactly how a party talks
        /// itself into a weaker tag than the key was issued for.
        /// </summary>
        private static void EnsureUsable(COSEKey Key, COSEAlgorithm Algorithm)
        {

            if (Algorithm.Family != COSEAlgorithmFamily.HMAC)
                throw new COSEException($"The COSE algorithm '{Algorithm.Name}' is not a message authentication algorithm!");

            if (Key.KeyType != COSEKeyType.Symmetric)
                throw new COSEException($"A COSE_Mac0 message needs a COSE key of key type Symmetric [RFC 9053, Section 3.1], but a key of key type {Key.KeyType} was given!");

            if (Key.K is null)
                throw new COSEException("The symmetric COSE key carries no key value!");

            if (Key.Algorithm.HasValue && Key.Algorithm.Value != Algorithm)
                throw new COSEException($"This message is to be authenticated with '{Algorithm.Name}', but the key names '{Key.Algorithm.Value.Name}' [RFC 9053, Section 3.1]!");

        }

        #endregion


        #region Verify(Key, ExternalAAD = null, DetachedPayload = null, ExpectedAlgorithm = null)

        /// <summary>
        /// Verify the authentication tag of this message.
        /// </summary>
        /// <param name="Key">A symmetric COSE key.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that was authenticated along with the payload.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="ExpectedAlgorithm">The MAC algorithm the caller expects, required whenever the message states its algorithm within the unprotected header bucket only.</param>
        public Boolean Verify(COSEKey         Key,
                              Byte[]?         ExternalAAD         = null,
                              Byte[]?         DetachedPayload     = null,
                              COSEAlgorithm?  ExpectedAlgorithm   = null)

            => Verify(Key,
                      out _,
                      ExternalAAD,
                      DetachedPayload,
                      ExpectedAlgorithm);

        #endregion

        #region Verify(Key, out ErrorResponse, ExternalAAD = null, DetachedPayload = null, ExpectedAlgorithm = null)

        /// <summary>
        /// Verify the authentication tag of this message and report why it
        /// failed. A failed verification is not an exception: It is the
        /// expected outcome of checking untrusted data.
        ///
        /// The comparison itself is CONSTANT TIME - see
        /// COSEAlgorithm.VerifyMAC(...) - which matters here in a way it does
        /// not for a signature.
        /// </summary>
        /// <param name="Key">A symmetric COSE key.</param>
        /// <param name="ErrorResponse">The reason why the verification failed.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that was authenticated along with the payload.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        /// <param name="ExpectedAlgorithm">The MAC algorithm the caller expects.</param>
        public Boolean Verify(COSEKey                           Key,
                              [NotNullWhen(false)] out String?  ErrorResponse,
                              Byte[]?                           ExternalAAD         = null,
                              Byte[]?                           DetachedPayload     = null,
                              COSEAlgorithm?                    ExpectedAlgorithm   = null)
        {

            if (!COSEHeaders.VerifyCriticalHeaderParameters(ProtectedHeader,
                                                            UnprotectedHeader,
                                                            out ErrorResponse))
            {
                return false;
            }

            #region Which algorithm to authenticate with

            var algorithm = ExpectedAlgorithm ?? ProtectedHeader.Algorithm ?? Key.Algorithm;

            if (!algorithm.HasValue)
            {

                ErrorResponse = UnprotectedHeader.Algorithm.HasValue
                                    ? $"This COSE_Mac0 message states its algorithm '{UnprotectedHeader.Algorithm.Value.Name}' within the unprotected header bucket only, where it is not covered by the authentication tag: Pass the expected algorithm explicitly in order to accept it!"
                                    :  "This COSE_Mac0 message does not state its MAC algorithm: Pass the expected algorithm explicitly!";

                return false;

            }

            var statedAlgorithm = Algorithm;

            if (statedAlgorithm.HasValue &&
                statedAlgorithm.Value != algorithm.Value)
            {
                ErrorResponse = $"This COSE_Mac0 message was authenticated with the algorithm '{statedAlgorithm.Value.Name}', but the algorithm '{algorithm.Value.Name}' was expected!";
                return false;
            }

            if (algorithm.Value.Family != COSEAlgorithmFamily.HMAC)
            {
                ErrorResponse = $"The COSE algorithm '{algorithm.Value.Name}' is not a message authentication algorithm: A COSE_Mac0 message can not be authenticated with a signature algorithm!";
                return false;
            }

            if (Key.KeyType != COSEKeyType.Symmetric || Key.K is null)
            {
                ErrorResponse = $"A COSE_Mac0 message needs a COSE key of key type Symmetric [RFC 9053, Section 3.1], but a key of key type {Key.KeyType} was given!";
                return false;
            }

            #endregion

            #region The authentication tag itself

            if (!TryGetPayload(DetachedPayload, out var payload, out ErrorResponse))
                return false;

            try
            {

                if (!algorithm.Value.VerifyMAC(ToBeMACed(ProtectedHeaderBytes, payload, ExternalAAD),
                                               Tag,
                                               Key.K))
                {
                    ErrorResponse = "The authentication tag is not the right one for this payload and this key!";
                    return false;
                }

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                return false;
            }

            ErrorResponse = null;
            return true;

            #endregion

        }

        #endregion

        #region (private) TryGetPayload(DetachedPayload, out Payload, out ErrorResponse)

        /// <summary>
        /// Which payload the authentication tag is computed over: The one
        /// carried within the message, or the detached one supplied by the
        /// caller. Supplying both is rejected, because there would be no way
        /// to tell which of the two a verification result refers to.
        /// </summary>
        private Boolean TryGetPayload(Byte[]?                           DetachedPayload,
                                      [NotNullWhen(true)]  out Byte[]?  Payload,
                                      [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (this.Payload is not null)
            {

                if (DetachedPayload is not null)
                {
                    Payload        = null;
                    ErrorResponse  = "This COSE_Mac0 message carries its payload, and a detached payload was supplied as well!";
                    return false;
                }

                Payload        = this.Payload;
                ErrorResponse  = null;
                return true;

            }

            if (DetachedPayload is null)
            {
                Payload        = null;
                ErrorResponse  = "This COSE_Mac0 message carries a detached payload, which has to be supplied in order to verify it!";
                return false;
            }

            Payload        = DetachedPayload;
            ErrorResponse  = null;
            return true;

        }

        #endregion


        #region (static) TryParse(Data, out Mac0, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR data as a COSE_Mac0 message.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE_Mac0 message.</param>
        /// <param name="Mac0">The parsed COSE_Mac0 message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte>                  Data,
                                       [NotNullWhen(true)]  out COSEMac0?  Mac0,
                                       [NotNullWhen(false)] out String?    ErrorResponse)
        {

            if (!CBORValue.TryParse(Data, out var cbor, out ErrorResponse))
            {
                Mac0 = null;
                return false;
            }

            return TryParse(cbor, out Mac0, out ErrorResponse);

        }

        #endregion

        #region (static) TryParse(CBOR, out Mac0, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR value as a COSE_Mac0 message.
        /// Both the tagged and the untagged form are accepted; which one it
        /// was is remembered, as the CBOR tag is not covered by the
        /// authentication tag but is part of the bytes on the wire.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Mac0 message.</param>
        /// <param name="Mac0">The parsed COSE_Mac0 message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                           CBOR,
                                       [NotNullWhen(true)]  out COSEMac0?  Mac0,
                                       [NotNullWhen(false)] out String?    ErrorResponse)
        {

            Mac0 = null;

            var isTagged  = false;
            var message   = CBOR;

            if (message.Kind == CBORValueKind.Tagged)
            {

                if (message.Tag != CBORTag.COSEMac0)
                {
                    ErrorResponse = $"A COSE_Mac0 message must be tagged with CBOR tag {CBORTag.COSEMac0.Value}, but was tagged with CBOR tag {message.Tag.Value}!";
                    return false;
                }

                isTagged  = true;
                message   = message.UntaggedValue;

            }

            if (message.Kind != CBORValueKind.Array)
            {
                ErrorResponse = $"A COSE_Mac0 message must be a CBOR array, but was a CBOR {message.Kind}!";
                return false;
            }

            if (message.Count != 4)
            {
                ErrorResponse = $"A COSE_Mac0 message must be a CBOR array of 4 elements, but had {message.Count} element(s)!";
                return false;
            }

            var items = message.AsArray();

            if (!items[0].TryGetBytes(out var protectedHeaderBytes))
            {
                ErrorResponse = "The protected header bucket of a COSE_Mac0 message must be a byte string!";
                return false;
            }

            if (!COSEHeaders.TryParse(items[1], out var unprotectedHeader, out ErrorResponse))
                return false;

            Byte[]? payload = null;

            if (items[2].Kind != CBORValueKind.Null &&
                !items[2].TryGetBytes(out payload))
            {
                ErrorResponse = "The payload of a COSE_Mac0 message must be a byte string, or null when it is detached!";
                return false;
            }

            if (!items[3].TryGetBytes(out var tag))
            {
                ErrorResponse = "The authentication tag of a COSE_Mac0 message must be a byte string!";
                return false;
            }

            try
            {

                Mac0 = new COSEMac0(
                           protectedHeaderBytes,
                           unprotectedHeader,
                           payload,
                           tag,
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
        /// Return a CBOR representation of this COSE_Mac0 message.
        /// </summary>
        public CBORValue ToCBOR()
        {

            var message = CBORValue.FromArray(
                              CBORValue.FromBytes(ProtectedHeaderBytes),
                              UnprotectedHeader.ToCBOR(),
                              Payload is not null
                                  ? CBORValue.FromBytes(Payload)
                                  : CBORValue.Null,
                              CBORValue.FromBytes(Tag)
                          );

            return IsTagged
                       ? message.WithTag(CBORTag.COSEMac0)
                       : message;

        }

        #endregion

        #region ToByteArray(Options = null)

        /// <summary>
        /// Return the CBOR encoding of this COSE_Mac0 message.
        /// </summary>
        /// <param name="Options">Optional CBOR writer options.</param>
        public Byte[] ToByteArray(CBORWriterOptions? Options = null)

            => ToCBOR().ToByteArray(Options);

        #endregion

        #region Detach()

        /// <summary>
        /// Return a copy of this message without its payload, e.g. because the
        /// payload travels elsewhere.
        ///
        /// The authentication tag stays valid: It never covered the message,
        /// only the MAC_structure, and the MAC_structure always holds the full
        /// payload.
        /// </summary>
        public COSEMac0 Detach()

            => new (ProtectedHeaderBytes,
                    UnprotectedHeader,
                    null,
                    Tag,
                    IsTagged);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"COSE_Mac0{(Algorithm.HasValue ? $" {Algorithm.Value.Name}" : "")}, {Tag.Length} byte tag{(IsDetached ? ", detached payload" : "")}";

        #endregion

    }

}
