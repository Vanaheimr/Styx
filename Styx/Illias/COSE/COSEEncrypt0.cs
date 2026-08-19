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
    /// A COSE_Encrypt0 message [RFC 9052, Section 5.2]: content encrypted for
    /// a recipient who already holds the key, tagged with CBOR tag 16.
    ///
    /// <code>
    /// COSE_Encrypt0 = [
    ///     protected   : bstr .cbor header_map,
    ///     unprotected : header_map,
    ///     ciphertext  : bstr / nil
    /// ]
    /// </code>
    ///
    /// Three things about the encrypted structures differ from everything else
    /// in this namespace, and all three catch people out.
    ///
    /// THE Enc_structure HAS THREE ELEMENTS, NOT FOUR. It is
    /// ["Encrypt0", protected, external_aad] - no payload. That is not an
    /// oversight: the payload is what is being ENCRYPTED, and the
    /// Enc_structure is what is merely AUTHENTICATED alongside it. It becomes
    /// the AEAD's additional data, so the recipient rebuilds it from the
    /// message rather than receiving it.
    ///
    /// THE AUTHENTICATION TAG IS NOT A FIELD. AES-GCM's 16-byte tag is
    /// appended to the ciphertext and travels inside the same byte string. An
    /// implementation giving it a field of its own interoperates with nothing.
    ///
    /// THE NONCE IS PUBLIC AND MUST NEVER REPEAT. It travels in the "iv"
    /// header parameter, in the clear, and that is fine - what is not fine is
    /// using one twice with the same key. GCM fails catastrophically on nonce
    /// reuse: two messages under one nonce leak the XOR of their plaintexts
    /// AND the authentication subkey, which lets an adversary forge
    /// afterwards. The nonce is therefore never generated here. The caller
    /// passes it, because only the caller knows whether it has been used.
    ///
    /// And the point worth keeping in view: an encrypted message says nothing
    /// about WHO sent it. AEAD integrity means "whoever holds this key wrote
    /// this". RFC 9052 Section 8.3 says as much - content encryption provides
    /// "either no or very limited data origination". A signed payload inside
    /// an encrypted envelope is how one gets both.
    /// </summary>
    public sealed class COSEEncrypt0
    {

        #region Data

        /// <summary>
        /// The context text string of a COSE_Encrypt0 [RFC 9052, Section 5.3].
        /// </summary>
        public const String Encrypt0Context = "Encrypt0";

        #endregion

        #region Properties

        /// <summary>
        /// The serialized protected header bucket, exactly as authenticated
        /// and as received.
        /// </summary>
        public Byte[]          ProtectedHeaderBytes    { get; }

        /// <summary>
        /// The protected header parameters, which the AEAD tag covers.
        /// </summary>
        public COSEHeaders     ProtectedHeader         { get; }

        /// <summary>
        /// The unprotected header parameters, which it does NOT cover - and
        /// which therefore must not be trusted after a successful decryption.
        /// </summary>
        public COSEHeaders     UnprotectedHeader       { get; }

        /// <summary>
        /// The ciphertext with the AEAD tag appended, or null when detached.
        /// </summary>
        public Byte[]?         Ciphertext              { get; }

        /// <summary>
        /// Whether this message is wrapped within CBOR tag 16.
        /// </summary>
        public Boolean         IsTagged                { get; }

        /// <summary>
        /// Whether the ciphertext is detached.
        /// </summary>
        public Boolean         IsDetached

            => Ciphertext is null;

        /// <summary>
        /// The content encryption algorithm, protected header bucket first.
        /// </summary>
        public COSEAlgorithm?  Algorithm

            => ProtectedHeader.  Algorithm ??
               UnprotectedHeader.Algorithm;

        /// <summary>
        /// The key identifier, protected header bucket first.
        /// </summary>
        public Byte[]?         KeyIdentifier

            => ProtectedHeader.  KeyIdentifier ??
               UnprotectedHeader.KeyIdentifier;

        /// <summary>
        /// The nonce, from either header bucket. It travels in the clear, and
        /// it must never repeat under one key.
        /// </summary>
        public Byte[]?         IV

            => ProtectedHeader.  IV ??
               UnprotectedHeader.IV;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new COSE_Encrypt0 message from its parts.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket, which the AEAD tag covers byte by byte.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters.</param>
        /// <param name="Ciphertext">The ciphertext with the tag appended, or null when detached.</param>
        /// <param name="IsTagged">Whether the message is wrapped within CBOR tag 16.</param>
        public COSEEncrypt0(Byte[]        ProtectedHeaderBytes,
                            COSEHeaders?  UnprotectedHeader,
                            Byte[]?       Ciphertext,
                            Boolean       IsTagged   = true)
        {

            if (!COSEHeaders.TryParseProtected(ProtectedHeaderBytes, out var protectedHeader, out var errorResponse))
                throw new COSEException($"The protected header bucket is invalid: {errorResponse}");

            this.ProtectedHeaderBytes  = ProtectedHeaderBytes;
            this.ProtectedHeader       = protectedHeader;
            this.UnprotectedHeader     = UnprotectedHeader ?? COSEHeaders.Empty;
            this.Ciphertext            = Ciphertext;
            this.IsTagged              = IsTagged;

        }

        #endregion


        #region (static) EncStructure(Context, ProtectedHeaderBytes, ExternalAAD = null)

        /// <summary>
        /// Return the encoded Enc_structure [RFC 9052, Section 5.3]:
        ///
        /// <code>
        /// Enc_structure = [
        ///     context      : "Encrypt" / "Encrypt0" / "Enc_Recipient" /
        ///                    "Mac_Recipient" / "Rec_Recipient",
        ///     protected    : empty_or_serialized_map,
        ///     external_aad : bstr
        /// ]
        /// </code>
        ///
        /// Three elements. There is no payload here, because the payload is
        /// what gets encrypted rather than what gets authenticated alongside.
        /// </summary>
        /// <param name="Context">The context text string.</param>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket, verbatim.</param>
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
        public static Byte[] EncStructure(String   Context,
                                          Byte[]   ProtectedHeaderBytes,
                                          Byte[]?  ExternalAAD   = null)
        {

            var writer = new CBORWriter();

            writer.WriteStartArray(3);
            writer.WriteTextString(Context);
            writer.WriteByteString(ProtectedHeaderBytes);
            writer.WriteByteString(ExternalAAD ?? []);
            writer.WriteEndArray();

            return writer.ToByteArray();

        }

        #endregion

        #region ToBeEncrypted(ExternalAAD = null)

        /// <summary>
        /// Return the encoded Enc_structure of this message.
        /// </summary>
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
        public Byte[] ToBeEncrypted(Byte[]? ExternalAAD = null)

            => EncStructure(Encrypt0Context, ProtectedHeaderBytes, ExternalAAD);

        #endregion


        #region (static) Encrypt(Plaintext, Key, IV, ExternalAAD = null, ...)

        /// <summary>
        /// Encrypt the given payload, placing the algorithm within the
        /// protected header bucket and the nonce and key identifier within the
        /// unprotected one.
        /// </summary>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <param name="Key">A symmetric COSE key naming an AES-GCM algorithm.</param>
        /// <param name="IV">The 12-byte nonce, which must never repeat under one key.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that is authenticated without being transported.</param>
        /// <param name="DetachPayload">Whether to omit the ciphertext from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 16.</param>
        public static COSEEncrypt0 Encrypt(Byte[]    Plaintext,
                                           COSEKey   Key,
                                           Byte[]    IV,
                                           Byte[]?   ExternalAAD     = null,
                                           Boolean   DetachPayload   = false,
                                           Boolean   Tagged          = true)
        {

            var algorithm = Key.Algorithm
                                ?? throw new COSEException("An encrypted COSE message needs a content encryption algorithm: either on the key or within the protected header bucket!");

            var unprotected = new List<(CBORValue, CBORValue)> {
                                  (COSEHeaderLabel.IV, CBORValue.FromBytes(IV))
                              };

            if (Key.KeyIdentifier is not null)
                unprotected.Add((COSEHeaderLabel.KeyIdentifier, CBORValue.FromBytes(Key.KeyIdentifier)));

            return Encrypt(Plaintext,
                           Key,
                           COSEHeaders.Create(algorithm),
                           new COSEHeaders([.. unprotected]),
                           IV,
                           ExternalAAD,
                           DetachPayload,
                           Tagged);

        }

        #endregion

        #region (static) Encrypt(Plaintext, Key, ProtectedHeader, UnprotectedHeader, IV, ...)

        /// <summary>
        /// Encrypt the given payload with header buckets the caller composed.
        /// </summary>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <param name="Key">A symmetric COSE key.</param>
        /// <param name="ProtectedHeader">The protected header parameters, which the AEAD tag covers.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters.</param>
        /// <param name="IV">The 12-byte nonce.</param>
        /// <param name="ExternalAAD">Optional externally supplied data.</param>
        /// <param name="DetachPayload">Whether to omit the ciphertext from the message.</param>
        /// <param name="Tagged">Whether to wrap the message within CBOR tag 16.</param>
        public static COSEEncrypt0 Encrypt(Byte[]        Plaintext,
                                           COSEKey       Key,
                                           COSEHeaders   ProtectedHeader,
                                           COSEHeaders?  UnprotectedHeader,
                                           Byte[]        IV,
                                           Byte[]?       ExternalAAD     = null,
                                           Boolean       DetachPayload   = false,
                                           Boolean       Tagged          = true)
        {

            var algorithm = ProtectedHeader.Algorithm
                                ?? Key.Algorithm
                                ?? throw new COSEException("An encrypted COSE message needs a content encryption algorithm: either on the key or within the protected header bucket!");

            EnsureUsable(Key, algorithm);

            var protectedHeaderBytes = ProtectedHeader.ToProtectedByteArray();

            var ciphertext = algorithm.Encrypt(
                                 Plaintext,
                                 Key.K!,
                                 IV,
                                 EncStructure(Encrypt0Context, protectedHeaderBytes, ExternalAAD)
                             );

            return new COSEEncrypt0(
                       protectedHeaderBytes,
                       UnprotectedHeader,
                       DetachPayload ? null : ciphertext,
                       Tagged
                   );

        }

        #endregion

        #region (internal static) EnsureUsable(Key, Algorithm)

        /// <summary>
        /// The key checks of RFC 9053, Section 4.1.
        ///
        /// The width check earns its place: A128GCM and A256GCM are one cipher
        /// and two identifiers, so a key of the wrong width is not a different
        /// strength but a DIFFERENT ALGORITHM, and letting it through would
        /// silently produce a message nobody can read.
        /// </summary>
        internal static void EnsureUsable(COSEKey Key, COSEAlgorithm Algorithm)
        {

            if (Algorithm.Family != COSEAlgorithmFamily.AESGCM)
                throw new COSEException($"The COSE algorithm '{Algorithm.Name}' is not a content encryption algorithm supported by this implementation!");

            if (Key.KeyType != COSEKeyType.Symmetric || Key.K is null)
                throw new COSEException($"An encrypted COSE message needs a COSE key of key type Symmetric [RFC 9053, Section 4.1], but a key of key type {Key.KeyType} was given!");

            if (Key.Algorithm.HasValue && Key.Algorithm.Value != Algorithm)
                throw new COSEException($"This message is to be encrypted with '{Algorithm.Name}', but the key names '{Key.Algorithm.Value.Name}' [RFC 9053, Section 4.1]!");

            if (Algorithm.KeySizeInBytes.HasValue && Key.K.Length != Algorithm.KeySizeInBytes.Value)
                throw new COSEException($"The COSE algorithm '{Algorithm.Name}' needs a {Algorithm.KeySizeInBytes.Value}-byte key, but a {Key.K.Length}-byte key was given!");

        }

        #endregion


        #region Decrypt(Key, out Plaintext, out ErrorResponse, ExternalAAD = null, DetachedCiphertext = null, ExpectedAlgorithm = null)

        /// <summary>
        /// Decrypt this message with the key it was encrypted for.
        ///
        /// A failed decryption is not an exception: it is the expected outcome
        /// of processing untrusted data. And there is no partial plaintext in
        /// the failure case and never can be - an AEAD failure means the whole
        /// message is unauthenticated.
        /// </summary>
        /// <param name="Key">A symmetric COSE key.</param>
        /// <param name="Plaintext">The decrypted content.</param>
        /// <param name="ErrorResponse">The reason why the decryption failed.</param>
        /// <param name="ExternalAAD">Optional externally supplied data that was authenticated along with the payload.</param>
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
                ErrorResponse = $"A COSE_Encrypt0 message needs a COSE key of key type Symmetric, but a key of key type {Key.KeyType} was given!";
                return false;
            }

            return DecryptWith(Key.K, Encrypt0Context, out Plaintext, out ErrorResponse,
                               ExternalAAD, DetachedCiphertext, ExpectedAlgorithm);

        }

        #endregion

        #region (internal) DecryptWith(ContentKey, Context, out Plaintext, out ErrorResponse, ...)

        /// <summary>
        /// Decrypt with a content key that has already been established: the
        /// half COSE_Encrypt0 and COSE_Encrypt share.
        /// </summary>
        internal Boolean DecryptWith(Byte[]                            ContentKey,
                                     String                            Context,
                                     [NotNullWhen(true)]  out Byte[]?  Plaintext,
                                     [NotNullWhen(false)] out String?  ErrorResponse,
                                     Byte[]?                           ExternalAAD,
                                     Byte[]?                           DetachedCiphertext,
                                     COSEAlgorithm?                    ExpectedAlgorithm)
        {

            Plaintext = null;

            if (!COSEHeaders.VerifyCriticalHeaderParameters(ProtectedHeader,
                                                            UnprotectedHeader,
                                                            out ErrorResponse))
            {
                return false;
            }

            var algorithm = ExpectedAlgorithm ?? Algorithm;

            if (!algorithm.HasValue)
            {
                ErrorResponse = "This COSE message does not state its content encryption algorithm: Pass the expected algorithm explicitly!";
                return false;
            }

            var stated = Algorithm;

            if (stated.HasValue && stated.Value != algorithm.Value)
            {
                ErrorResponse = $"This COSE message was encrypted with the algorithm '{stated.Value.Name}', but the algorithm '{algorithm.Value.Name}' was expected!";
                return false;
            }

            if (algorithm.Value.Family != COSEAlgorithmFamily.AESGCM)
            {
                ErrorResponse = $"The COSE algorithm '{algorithm.Value.Name}' is not a content encryption algorithm supported by this implementation!";
                return false;
            }

            if (algorithm.Value.KeySizeInBytes.HasValue &&
                ContentKey.Length != algorithm.Value.KeySizeInBytes.Value)
            {
                ErrorResponse = $"The COSE algorithm '{algorithm.Value.Name}' needs a {algorithm.Value.KeySizeInBytes.Value}-byte key, but the content key is {ContentKey.Length} bytes long!";
                return false;
            }

            var nonce = IV;

            if (nonce is null)
            {
                ErrorResponse = "This COSE message carries no initialization vector!";
                return false;
            }

            if (Ciphertext is not null && DetachedCiphertext is not null)
            {
                ErrorResponse = "This COSE message carries its ciphertext, and a detached ciphertext was supplied as well!";
                return false;
            }

            var body = Ciphertext ?? DetachedCiphertext;

            if (body is null)
            {
                ErrorResponse = "This COSE message carries a detached ciphertext, which has to be supplied in order to decrypt it!";
                return false;
            }

            Byte[]? plaintext;

            try
            {
                plaintext = algorithm.Value.Decrypt(body, ContentKey, nonce,
                                                    EncStructure(Context, ProtectedHeaderBytes, ExternalAAD));
            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                return false;
            }

            if (plaintext is null)
            {
                ErrorResponse = "The ciphertext does not authenticate under this key: it was altered, or the key, the nonce or the additional data is not the right one!";
                return false;
            }

            Plaintext      = plaintext;
            ErrorResponse  = null;
            return true;

        }

        #endregion


        #region (static) TryParse(Data/CBOR, out Encrypt0, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR data as a COSE_Encrypt0 message.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE_Encrypt0 message.</param>
        /// <param name="Encrypt0">The parsed COSE_Encrypt0 message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte>                      Data,
                                       [NotNullWhen(true)]  out COSEEncrypt0?  Encrypt0,
                                       [NotNullWhen(false)] out String?        ErrorResponse)
        {

            if (!CBORValue.TryParse(Data, out var cbor, out ErrorResponse))
            {
                Encrypt0 = null;
                return false;
            }

            return TryParse(cbor, out Encrypt0, out ErrorResponse);

        }

        /// <summary>
        /// Try to parse the given CBOR value as a COSE_Encrypt0 message.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE_Encrypt0 message.</param>
        /// <param name="Encrypt0">The parsed COSE_Encrypt0 message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                               CBOR,
                                       [NotNullWhen(true)]  out COSEEncrypt0?  Encrypt0,
                                       [NotNullWhen(false)] out String?        ErrorResponse)
        {

            Encrypt0 = null;

            var isTagged  = false;
            var message   = CBOR;

            if (message.Kind == CBORValueKind.Tagged)
            {

                if (message.Tag != CBORTag.COSEEncrypt0)
                {
                    ErrorResponse = $"A COSE_Encrypt0 message must be tagged with CBOR tag {CBORTag.COSEEncrypt0.Value}, but was tagged with CBOR tag {message.Tag.Value}!";
                    return false;
                }

                isTagged  = true;
                message   = message.UntaggedValue;

            }

            if (message.Kind != CBORValueKind.Array)
            {
                ErrorResponse = $"A COSE_Encrypt0 message must be a CBOR array, but was a CBOR {message.Kind}!";
                return false;
            }

            if (message.Count != 3)
            {
                ErrorResponse = $"A COSE_Encrypt0 message must be a CBOR array of 3 elements, but had {message.Count} element(s)!";
                return false;
            }

            var items = message.AsArray();

            if (!items[0].TryGetBytes(out var protectedHeaderBytes))
            {
                ErrorResponse = "The protected header bucket of a COSE_Encrypt0 message must be a byte string!";
                return false;
            }

            if (!COSEHeaders.TryParse(items[1], out var unprotectedHeader, out ErrorResponse))
                return false;

            Byte[]? ciphertext = null;

            if (items[2].Kind != CBORValueKind.Null &&
                !items[2].TryGetBytes(out ciphertext))
            {
                ErrorResponse = "The ciphertext of a COSE_Encrypt0 message must be a byte string, or null when it is detached!";
                return false;
            }

            try
            {

                Encrypt0       = new COSEEncrypt0(protectedHeaderBytes, unprotectedHeader, ciphertext, isTagged);
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
        /// Return a CBOR representation of this COSE_Encrypt0 message.
        /// </summary>
        public CBORValue ToCBOR()
        {

            var message = CBORValue.FromArray(
                              CBORValue.FromBytes(ProtectedHeaderBytes),
                              UnprotectedHeader.ToCBOR(),
                              Ciphertext is not null
                                  ? CBORValue.FromBytes(Ciphertext)
                                  : CBORValue.Null
                          );

            return IsTagged
                       ? message.WithTag(CBORTag.COSEEncrypt0)
                       : message;

        }

        /// <summary>
        /// Return the CBOR encoding of this COSE_Encrypt0 message.
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

            => $"COSE_Encrypt0{(Algorithm.HasValue ? $" {Algorithm.Value.Name}" : "")}, {Ciphertext?.Length ?? 0} bytes";

        #endregion

    }

}
