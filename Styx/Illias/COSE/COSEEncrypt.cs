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
    /// A COSE_Encrypt message [RFC 9052, Section 5.1]: content encrypted once,
    /// with the content key delivered to each recipient in a structure of its
    /// own. CBOR tag 96.
    ///
    /// <code>
    /// COSE_Encrypt = [
    ///     protected   : bstr .cbor header_map,
    ///     unprotected : header_map,
    ///     ciphertext  : bstr / nil,
    ///     recipients  : [+COSE_recipient]
    /// ]
    /// </code>
    ///
    /// Four elements where COSE_Encrypt0 has three, and the fourth is the
    /// point: COSE_Encrypt0 assumes both parties already hold the key, while
    /// this solves the distribution problem inside the message. The
    /// Enc_structure is the same either way, differing only in its context
    /// string - "Encrypt" here, "Encrypt0" there.
    ///
    /// The one-"direct"-recipient case is COSE_Encrypt0 with ceremony: the
    /// recipient structure carries an empty protected bucket, an empty
    /// ciphertext and a key identifier, and nothing else. That is why
    /// COSE_Encrypt0 exists.
    ///
    /// What holds for COSE_Mac holds here too: with several recipients the
    /// AEAD's integrity guarantee stops distinguishing them, because they all
    /// hold the same content key. Any recipient can produce a message the
    /// others will accept.
    /// </summary>
    public sealed class COSEEncrypt
    {

        #region Data

        /// <summary>
        /// The context text string of the body of a COSE_Encrypt
        /// [RFC 9052, Section 5.3].
        /// </summary>
        public const String EncryptContext = "Encrypt";

        #endregion

        #region Properties

        /// <summary>
        /// The serialized protected header bucket, exactly as authenticated
        /// and as received.
        /// </summary>
        public Byte[]                        ProtectedHeaderBytes    { get; }

        /// <summary>
        /// The protected header parameters, which the AEAD tag covers.
        /// </summary>
        public COSEHeaders                   ProtectedHeader         { get; }

        /// <summary>
        /// The unprotected header parameters, which it does NOT cover.
        /// </summary>
        public COSEHeaders                   UnprotectedHeader       { get; }

        /// <summary>
        /// The ciphertext with the AEAD tag appended, or null when detached.
        /// </summary>
        public Byte[]?                       Ciphertext              { get; }

        /// <summary>
        /// How the content key reaches each party.
        /// </summary>
        public IReadOnlyList<COSERecipient>  Recipients              { get; }

        /// <summary>
        /// Whether this message is wrapped within CBOR tag 96.
        /// </summary>
        public Boolean                       IsTagged                { get; }

        /// <summary>
        /// Whether the ciphertext is detached.
        /// </summary>
        public Boolean                       IsDetached

            => Ciphertext is null;

        /// <summary>
        /// The content encryption algorithm, protected header bucket first.
        /// </summary>
        public COSEAlgorithm?                Algorithm

            => ProtectedHeader.  Algorithm ??
               UnprotectedHeader.Algorithm;

        /// <summary>
        /// The nonce, from either header bucket.
        /// </summary>
        public Byte[]?                       IV

            => ProtectedHeader.  IV ??
               UnprotectedHeader.IV;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new COSE_Encrypt message from its parts.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters.</param>
        /// <param name="Ciphertext">The ciphertext with the tag appended, or null when detached.</param>
        /// <param name="Recipients">How the content key reaches each party.</param>
        /// <param name="IsTagged">Whether the message is wrapped within CBOR tag 96.</param>
        public COSEEncrypt(Byte[]                      ProtectedHeaderBytes,
                           COSEHeaders?                UnprotectedHeader,
                           Byte[]?                     Ciphertext,
                           IEnumerable<COSERecipient>  Recipients,
                           Boolean                     IsTagged   = true)
        {

            if (!COSEHeaders.TryParseProtected(ProtectedHeaderBytes, out var protectedHeader, out var errorResponse))
                throw new COSEException($"The protected header bucket is invalid: {errorResponse}");

            var recipients = Recipients.ToArray();

            if (recipients.Length == 0)
                throw new COSEException("A COSE_Encrypt message must carry at least one recipient!");

            this.ProtectedHeaderBytes  = ProtectedHeaderBytes;
            this.ProtectedHeader       = protectedHeader;
            this.UnprotectedHeader     = UnprotectedHeader ?? COSEHeaders.Empty;
            this.Ciphertext            = Ciphertext;
            this.Recipients            = recipients;
            this.IsTagged              = IsTagged;

        }

        #endregion


        #region ToBeEncrypted(ExternalAAD = null)

        /// <summary>
        /// Return the encoded Enc_structure of this message, whose context
        /// string is "Encrypt" and not "Encrypt0".
        /// </summary>
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
        public Byte[] ToBeEncrypted(Byte[]? ExternalAAD = null)

            => COSEEncrypt0.EncStructure(EncryptContext, ProtectedHeaderBytes, ExternalAAD);

        #endregion

        #region (static) Encrypt(Plaintext, ContentKey, Recipients, IV, ...)

        /// <summary>
        /// Encrypt the given payload under a content key, and deliver that key
        /// to each recipient.
        ///
        /// The content key is the caller's rather than generated here, for the
        /// same reason the nonce is: a generated one would make the message
        /// unreproducible, and whoever has a key management scheme has one for
        /// a reason.
        /// </summary>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <param name="ContentKey">The symmetric content key, naming an AES-GCM algorithm.</param>
        /// <param name="Recipients">How the content key reaches each party.</param>
        /// <param name="IV">The 12-byte nonce, which must never repeat under one key.</param>
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
        /// <param name="DetachPayload">Whether to omit the ciphertext from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 96.</param>
        public static COSEEncrypt Encrypt(Byte[]                      Plaintext,
                                          COSEKey                     ContentKey,
                                          IEnumerable<COSERecipient>  Recipients,
                                          Byte[]                      IV,
                                          Byte[]?                     ExternalAAD     = null,
                                          Boolean                     DetachPayload   = false,
                                          Boolean                     Tagged          = true)
        {

            var algorithm = ContentKey.Algorithm
                                ?? throw new COSEException("An encrypted COSE message needs a content encryption algorithm on its content key!");

            COSEEncrypt0.EnsureUsable(ContentKey, algorithm);

            var protectedHeaderBytes = COSEHeaders.Create(algorithm).ToProtectedByteArray();

            var ciphertext = algorithm.Encrypt(
                                 Plaintext,
                                 ContentKey.K!,
                                 IV,
                                 COSEEncrypt0.EncStructure(EncryptContext, protectedHeaderBytes, ExternalAAD)
                             );

            return new COSEEncrypt(
                       protectedHeaderBytes,
                       new COSEHeaders([(COSEHeaderLabel.IV, CBORValue.FromBytes(IV))]),
                       DetachPayload ? null : ciphertext,
                       Recipients,
                       Tagged
                   );

        }

        #endregion

        #region Decrypt(Key, out Plaintext, out ErrorResponse, ...)

        /// <summary>
        /// Decrypt this message with a key one of its recipients was built for.
        ///
        /// Every recipient is tried, because a party holding one key does not
        /// generally know which entry in the list is theirs. A recipient that
        /// does not yield a key is not an error - it is somebody else's.
        /// </summary>
        /// <param name="Key">A symmetric COSE key this party holds.</param>
        /// <param name="Plaintext">The decrypted content.</param>
        /// <param name="ErrorResponse">The reason why the decryption failed.</param>
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
        /// <param name="DetachedCiphertext">The ciphertext, when this message carries a detached one.</param>
        /// <param name="ExpectedAlgorithm">The content encryption algorithm the caller expects.</param>
        public Boolean Decrypt(COSEKey                           Key,
                               [NotNullWhen(true)]  out Byte[]?  Plaintext,
                               [NotNullWhen(false)] out String?  ErrorResponse,
                               Byte[]?                           ExternalAAD          = null,
                               Byte[]?                           DetachedCiphertext   = null,
                               COSEAlgorithm?                    ExpectedAlgorithm    = null)
        {

            Plaintext = null;

            if (Key.KeyType != COSEKeyType.Symmetric || Key.K is null)
            {
                ErrorResponse = $"A COSE_Encrypt message needs a COSE key of key type Symmetric, but a key of key type {Key.KeyType} was given!";
                return false;
            }

            // The body is decrypted through the COSE_Encrypt0 machinery, which
            // is the same machinery with a different context string.
            var body  = new COSEEncrypt0(ProtectedHeaderBytes, UnprotectedHeader, Ciphertext, IsTagged);
            var tried = 0;

            foreach (var recipient in Recipients)
            {

                var contentKey = recipient.ContentKey(Key);

                if (contentKey is null)
                    continue;

                tried++;

                if (body.DecryptWith(contentKey, EncryptContext, out Plaintext, out _,
                                     ExternalAAD, DetachedCiphertext, ExpectedAlgorithm))
                {
                    ErrorResponse = null;
                    return true;
                }

            }

            ErrorResponse = tried == 0
                                ? "None of the recipients of this COSE_Encrypt message yielded a content key for the given key!"
                                : "A recipient yielded a content key, but the ciphertext did not authenticate under it!";

            return false;

        }

        #endregion


        #region (static) TryParse(Data/CBOR, out Encrypt, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR data as a COSE_Encrypt message.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE_Encrypt message.</param>
        /// <param name="Encrypt">The parsed COSE_Encrypt message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte>                     Data,
                                       [NotNullWhen(true)]  out COSEEncrypt?  Encrypt,
                                       [NotNullWhen(false)] out String?       ErrorResponse)
        {

            if (!CBORValue.TryParse(Data, out var cbor, out ErrorResponse))
            {
                Encrypt = null;
                return false;
            }

            return TryParse(cbor, out Encrypt, out ErrorResponse);

        }

        /// <summary>
        /// Try to parse the given CBOR value as a COSE_Encrypt message.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Encrypt message.</param>
        /// <param name="Encrypt">The parsed COSE_Encrypt message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                              CBOR,
                                       [NotNullWhen(true)]  out COSEEncrypt?  Encrypt,
                                       [NotNullWhen(false)] out String?       ErrorResponse)
        {

            Encrypt = null;

            var isTagged  = false;
            var message   = CBOR;

            if (message.Kind == CBORValueKind.Tagged)
            {

                if (message.Tag != CBORTag.COSEEncrypt)
                {
                    ErrorResponse = $"A COSE_Encrypt message must be tagged with CBOR tag {CBORTag.COSEEncrypt.Value}, but was tagged with CBOR tag {message.Tag.Value}!";
                    return false;
                }

                isTagged  = true;
                message   = message.UntaggedValue;

            }

            if (message.Kind != CBORValueKind.Array)
            {
                ErrorResponse = $"A COSE_Encrypt message must be a CBOR array, but was a CBOR {message.Kind}!";
                return false;
            }

            if (message.Count != 4)
            {
                ErrorResponse = $"A COSE_Encrypt message must be a CBOR array of 4 elements, but had {message.Count} element(s)!";
                return false;
            }

            var items = message.AsArray();

            if (!items[0].TryGetBytes(out var protectedHeaderBytes))
            {
                ErrorResponse = "The protected header bucket of a COSE_Encrypt message must be a byte string!";
                return false;
            }

            if (!COSEHeaders.TryParse(items[1], out var unprotectedHeader, out ErrorResponse))
                return false;

            Byte[]? ciphertext = null;

            if (items[2].Kind != CBORValueKind.Null &&
                !items[2].TryGetBytes(out ciphertext))
            {
                ErrorResponse = "The ciphertext of a COSE_Encrypt message must be a byte string, or null when it is detached!";
                return false;
            }

            if (items[3].Kind != CBORValueKind.Array || items[3].Count == 0)
            {
                ErrorResponse = "A COSE_Encrypt message must carry a non-empty array of recipients!";
                return false;
            }

            var recipients = new List<COSERecipient>();

            foreach (var item in items[3].AsArray())
            {

                if (!COSERecipient.TryParse(item, out var recipient, out ErrorResponse))
                    return false;

                recipients.Add(recipient);

            }

            try
            {

                Encrypt        = new COSEEncrypt(protectedHeaderBytes, unprotectedHeader, ciphertext, recipients, isTagged);
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
        /// Return a CBOR representation of this COSE_Encrypt message.
        /// </summary>
        public CBORValue ToCBOR()
        {

            var message = CBORValue.FromArray(
                              CBORValue.FromBytes(ProtectedHeaderBytes),
                              UnprotectedHeader.ToCBOR(),
                              Ciphertext is not null
                                  ? CBORValue.FromBytes(Ciphertext)
                                  : CBORValue.Null,
                              CBORValue.FromArray(Recipients.Select(static recipient => recipient.ToCBOR()))
                          );

            return IsTagged
                       ? message.WithTag(CBORTag.COSEEncrypt)
                       : message;

        }

        /// <summary>
        /// Return the CBOR encoding of this COSE_Encrypt message.
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

            => $"COSE_Encrypt{(Algorithm.HasValue ? $" {Algorithm.Value.Name}" : "")}, {Recipients.Count} recipient(s)";

        #endregion

    }

}
