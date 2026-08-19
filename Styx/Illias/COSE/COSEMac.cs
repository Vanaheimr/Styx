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
    /// A COSE_Mac message [RFC 9052, Section 6.1]: a payload authenticated
    /// with a content key that the message itself delivers to each recipient.
    /// CBOR tag 97.
    ///
    /// <code>
    /// COSE_Mac = [
    ///     protected   : bstr .cbor header_map,
    ///     unprotected : header_map,
    ///     payload     : bstr / nil,
    ///     tag         : bstr,
    ///     recipients  : [+COSE_recipient]
    /// ]
    /// </code>
    ///
    /// Five elements where COSE_Mac0 has four, and the fifth is the whole
    /// point: COSE_Mac0 assumes both parties already hold the key, while this
    /// solves the distribution problem inside the message. The MAC_structure
    /// is the same four-element structure either way, differing only in its
    /// context string - "MAC" here, "MAC0" there.
    ///
    /// A RECIPIENT LIST COSTS MORE THAN BYTES. Every recipient holds the same
    /// content key, so with more than one of them the tag stops distinguishing
    /// them at all: any recipient can produce a message the others will accept
    /// as coming from the sender. A COSE_Mac0 between two parties at least
    /// tells each of them that the other made it, on the grounds that they did
    /// not make it themselves; a COSE_Mac to three parties tells nobody
    /// anything of the kind. RFC 9052 Section 8.2 states it outright - a MAC
    /// provides "either no or very limited data origination" and "cannot be
    /// used to prove the identity of the sender to a third party".
    ///
    /// That is worth having in view before reaching for this structure. What
    /// it is genuinely good at is cheap integrity for a group that already
    /// trusts one another jointly - and if the question is who WITHIN that
    /// group said something, the answer has to be a signature.
    ///
    /// The single-"direct"-recipient case is COSE_Mac0 with extra ceremony:
    /// the recipient structure carries an empty protected bucket, an empty
    /// ciphertext and a key identifier, and nothing else. RFC 9052's Appendix
    /// C.5.1 is exactly that, and it is why COSE_Mac0 exists.
    /// </summary>
    public sealed class COSEMac
    {

        #region Data

        /// <summary>
        /// The context text string of a COSE_Mac authentication tag
        /// [RFC 9052, Section 6.3].
        /// </summary>
        public const String MACContext = "MAC";

        #endregion

        #region Properties

        /// <summary>
        /// The serialized protected header bucket, exactly as authenticated
        /// and as received.
        /// </summary>
        public Byte[]                        ProtectedHeaderBytes    { get; }

        /// <summary>
        /// The protected header parameters, which the authentication tag
        /// covers.
        /// </summary>
        public COSEHeaders                   ProtectedHeader         { get; }

        /// <summary>
        /// The unprotected header parameters, which it does NOT cover.
        /// </summary>
        public COSEHeaders                   UnprotectedHeader       { get; }

        /// <summary>
        /// The authenticated payload, or null when it is detached.
        /// </summary>
        public Byte[]?                       Payload                 { get; }

        /// <summary>
        /// The authentication tag - not the CBOR tag.
        /// </summary>
        public Byte[]                        Tag                     { get; }

        /// <summary>
        /// How the content key reaches each party.
        /// </summary>
        public IReadOnlyList<COSERecipient>  Recipients              { get; }

        /// <summary>
        /// Whether this message is wrapped within CBOR tag 97.
        /// </summary>
        public Boolean                       IsTagged                { get; }

        /// <summary>
        /// Whether the payload is detached.
        /// </summary>
        public Boolean                       IsDetached

            => Payload is null;

        /// <summary>
        /// The MAC algorithm, protected header bucket first.
        /// </summary>
        public COSEAlgorithm?                Algorithm

            => ProtectedHeader.  Algorithm ??
               UnprotectedHeader.Algorithm;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new COSE_Mac message from its parts.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters.</param>
        /// <param name="Payload">The authenticated payload, or null when detached.</param>
        /// <param name="Tag">The authentication tag.</param>
        /// <param name="Recipients">How the content key reaches each party.</param>
        /// <param name="IsTagged">Whether the message is wrapped within CBOR tag 97.</param>
        public COSEMac(Byte[]                      ProtectedHeaderBytes,
                       COSEHeaders?                UnprotectedHeader,
                       Byte[]?                     Payload,
                       Byte[]                      Tag,
                       IEnumerable<COSERecipient>  Recipients,
                       Boolean                     IsTagged   = true)
        {

            if (!COSEHeaders.TryParseProtected(ProtectedHeaderBytes, out var protectedHeader, out var errorResponse))
                throw new COSEException($"The protected header bucket is invalid: {errorResponse}");

            var recipients = Recipients.ToArray();

            if (recipients.Length == 0)
                throw new COSEException("A COSE_Mac message must carry at least one recipient!");

            this.ProtectedHeaderBytes  = ProtectedHeaderBytes;
            this.ProtectedHeader       = protectedHeader;
            this.UnprotectedHeader     = UnprotectedHeader ?? COSEHeaders.Empty;
            this.Payload               = Payload;
            this.Tag                   = Tag;
            this.Recipients            = recipients;
            this.IsTagged              = IsTagged;

        }

        #endregion


        #region (static) ToBeMACed(ProtectedHeaderBytes, Payload, ExternalAAD = null)

        /// <summary>
        /// Return the encoded MAC_structure [RFC 9052, Section 6.3] with the
        /// "MAC" context - the one difference from a COSE_Mac0's.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket, verbatim.</param>
        /// <param name="Payload">The payload to authenticate.</param>
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
        public static Byte[] ToBeMACed(Byte[]   ProtectedHeaderBytes,
                                       Byte[]   Payload,
                                       Byte[]?  ExternalAAD   = null)
        {

            var writer = new CBORWriter();

            writer.WriteStartArray(4);
            writer.WriteTextString(MACContext);
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
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
        /// <param name="DetachedPayload">The payload, when this message carries a detached one.</param>
        public Byte[] ToBeMACed(Byte[]?  ExternalAAD       = null,
                                Byte[]?  DetachedPayload   = null)
        {

            var payload = Payload ?? DetachedPayload
                              ?? throw new COSEException("This COSE_Mac message carries a detached payload, which has to be supplied!");

            if (Payload is not null && DetachedPayload is not null)
                throw new COSEException("This COSE_Mac message carries its payload, and a detached payload was supplied as well!");

            return ToBeMACed(ProtectedHeaderBytes, payload, ExternalAAD);

        }

        #endregion


        #region (static) Create(Payload, ContentKey, Recipients, ExternalAAD = null, ...)

        /// <summary>
        /// Authenticate the given payload under a content key, and deliver
        /// that key to each recipient.
        ///
        /// The content key is the caller's rather than generated here: a
        /// generated one would make the message unreproducible, and whoever
        /// has a key management scheme has it for a reason.
        /// </summary>
        /// <param name="Payload">The payload to authenticate.</param>
        /// <param name="ContentKey">The symmetric content key, naming an HMAC algorithm.</param>
        /// <param name="Recipients">How the content key reaches each party.</param>
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
        /// <param name="DetachPayload">Whether to omit the payload from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 97.</param>
        public static COSEMac Create(Byte[]                      Payload,
                                     COSEKey                     ContentKey,
                                     IEnumerable<COSERecipient>  Recipients,
                                     Byte[]?                     ExternalAAD     = null,
                                     Boolean                     DetachPayload   = false,
                                     Boolean                     Tagged          = true)
        {

            var algorithm = ContentKey.Algorithm
                                ?? throw new COSEException("A COSE_Mac message needs a MAC algorithm on its content key!");

            if (algorithm.Family != COSEAlgorithmFamily.HMAC)
                throw new COSEException($"The COSE algorithm '{algorithm.Name}' is not a message authentication algorithm!");

            if (ContentKey.KeyType != COSEKeyType.Symmetric || ContentKey.K is null)
                throw new COSEException($"A COSE_Mac message needs a content key of key type Symmetric, but a key of key type {ContentKey.KeyType} was given!");

            var protectedHeaderBytes = COSEHeaders.Create(algorithm).ToProtectedByteArray();

            var tag = algorithm.ComputeMAC(
                          ToBeMACed(protectedHeaderBytes, Payload, ExternalAAD),
                          ContentKey.K
                      );

            return new COSEMac(
                       protectedHeaderBytes,
                       null,
                       DetachPayload ? null : Payload,
                       tag,
                       Recipients,
                       Tagged
                   );

        }

        #endregion

        #region Verify(Key, out ErrorResponse, ExternalAAD = null, DetachedPayload = null, ExpectedAlgorithm = null)

        /// <summary>
        /// Verify this message with a key one of its recipients was built for.
        ///
        /// Every recipient is tried, because a party holding one key does not
        /// generally know which entry in the list is theirs.
        /// </summary>
        /// <param name="Key">A symmetric COSE key this party holds.</param>
        /// <param name="ErrorResponse">The reason why the verification failed.</param>
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
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

            var algorithm = ExpectedAlgorithm ?? Algorithm;

            if (!algorithm.HasValue)
            {
                ErrorResponse = "This COSE_Mac message does not state its MAC algorithm: Pass the expected algorithm explicitly!";
                return false;
            }

            if (algorithm.Value.Family != COSEAlgorithmFamily.HMAC)
            {
                ErrorResponse = $"The COSE algorithm '{algorithm.Value.Name}' is not a message authentication algorithm: A COSE_Mac message can not be authenticated with a signature algorithm!";
                return false;
            }

            if (Key.KeyType != COSEKeyType.Symmetric || Key.K is null)
            {
                ErrorResponse = $"A COSE_Mac message needs a COSE key of key type Symmetric, but a key of key type {Key.KeyType} was given!";
                return false;
            }

            if (Payload is not null && DetachedPayload is not null)
            {
                ErrorResponse = "This COSE_Mac message carries its payload, and a detached payload was supplied as well!";
                return false;
            }

            var payload = Payload ?? DetachedPayload;

            if (payload is null)
            {
                ErrorResponse = "This COSE_Mac message carries a detached payload, which has to be supplied in order to verify it!";
                return false;
            }

            var toBeMACed = ToBeMACed(ProtectedHeaderBytes, payload, ExternalAAD);
            var tried     = 0;

            foreach (var recipient in Recipients)
            {

                var contentKey = recipient.ContentKey(Key);

                if (contentKey is null)
                    continue;

                tried++;

                try
                {
                    if (algorithm.Value.VerifyMAC(toBeMACed, Tag, contentKey))
                    {
                        ErrorResponse = null;
                        return true;
                    }
                }
                catch (Exception)
                {
                    // A content key of the wrong width for this algorithm is
                    // somebody else's recipient rather than a failure of ours.
                    continue;
                }

            }

            ErrorResponse = tried == 0
                                ? "None of the recipients of this COSE_Mac message yielded a content key for the given key!"
                                : "A recipient yielded a content key, and the authentication tag is not the right one under it!";

            return false;

        }

        #endregion


        #region (static) TryParse(Data/CBOR, out Mac, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR data as a COSE_Mac message.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE_Mac message.</param>
        /// <param name="Mac">The parsed COSE_Mac message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte>                 Data,
                                       [NotNullWhen(true)]  out COSEMac?  Mac,
                                       [NotNullWhen(false)] out String?   ErrorResponse)
        {

            if (!CBORValue.TryParse(Data, out var cbor, out ErrorResponse))
            {
                Mac = null;
                return false;
            }

            return TryParse(cbor, out Mac, out ErrorResponse);

        }

        /// <summary>
        /// Try to parse the given CBOR value as a COSE_Mac message.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Mac message.</param>
        /// <param name="Mac">The parsed COSE_Mac message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                          CBOR,
                                       [NotNullWhen(true)]  out COSEMac?  Mac,
                                       [NotNullWhen(false)] out String?   ErrorResponse)
        {

            Mac = null;

            var isTagged  = false;
            var message   = CBOR;

            if (message.Kind == CBORValueKind.Tagged)
            {

                if (message.Tag != CBORTag.COSEMac)
                {
                    ErrorResponse = $"A COSE_Mac message must be tagged with CBOR tag {CBORTag.COSEMac.Value}, but was tagged with CBOR tag {message.Tag.Value}!";
                    return false;
                }

                isTagged  = true;
                message   = message.UntaggedValue;

            }

            if (message.Kind != CBORValueKind.Array)
            {
                ErrorResponse = $"A COSE_Mac message must be a CBOR array, but was a CBOR {message.Kind}!";
                return false;
            }

            if (message.Count != 5)
            {
                ErrorResponse = $"A COSE_Mac message must be a CBOR array of 5 elements, but had {message.Count} element(s)!";
                return false;
            }

            var items = message.AsArray();

            if (!items[0].TryGetBytes(out var protectedHeaderBytes))
            {
                ErrorResponse = "The protected header bucket of a COSE_Mac message must be a byte string!";
                return false;
            }

            if (!COSEHeaders.TryParse(items[1], out var unprotectedHeader, out ErrorResponse))
                return false;

            Byte[]? payload = null;

            if (items[2].Kind != CBORValueKind.Null &&
                !items[2].TryGetBytes(out payload))
            {
                ErrorResponse = "The payload of a COSE_Mac message must be a byte string, or null when it is detached!";
                return false;
            }

            if (!items[3].TryGetBytes(out var tag))
            {
                ErrorResponse = "The authentication tag of a COSE_Mac message must be a byte string!";
                return false;
            }

            if (items[4].Kind != CBORValueKind.Array || items[4].Count == 0)
            {
                ErrorResponse = "A COSE_Mac message must carry a non-empty array of recipients!";
                return false;
            }

            var recipients = new List<COSERecipient>();

            foreach (var item in items[4].AsArray())
            {

                if (!COSERecipient.TryParse(item, out var recipient, out ErrorResponse))
                    return false;

                recipients.Add(recipient);

            }

            try
            {

                Mac            = new COSEMac(protectedHeaderBytes, unprotectedHeader, payload, tag, recipients, isTagged);
                ErrorResponse  = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR() / ToByteArray(Options = null)

        /// <summary>
        /// Return a CBOR representation of this COSE_Mac message.
        /// </summary>
        public CBORValue ToCBOR()
        {

            var message = CBORValue.FromArray(
                              CBORValue.FromBytes(ProtectedHeaderBytes),
                              UnprotectedHeader.ToCBOR(),
                              Payload is not null
                                  ? CBORValue.FromBytes(Payload)
                                  : CBORValue.Null,
                              CBORValue.FromBytes(Tag),
                              CBORValue.FromArray(Recipients.Select(static recipient => recipient.ToCBOR()))
                          );

            return IsTagged
                       ? message.WithTag(CBORTag.COSEMac)
                       : message;

        }

        /// <summary>
        /// Return the CBOR encoding of this COSE_Mac message.
        /// </summary>
        /// <param name="Options">Optional CBOR writer options.</param>
        public Byte[] ToByteArray(CBORWriterOptions? Options = null)

            => ToCBOR().ToByteArray(Options);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"COSE_Mac{(Algorithm.HasValue ? $" {Algorithm.Value.Name}" : "")}, {Recipients.Count} recipient(s)";

        #endregion

    }

}
