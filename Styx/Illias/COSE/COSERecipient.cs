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
    /// A COSE_recipient [RFC 9052, Section 5.1]: how a content key reaches one
    /// party.
    ///
    /// <code>
    /// COSE_recipient = [
    ///     Headers,
    ///     ciphertext : bstr / nil,
    ///     ? recipients : [+COSE_recipient]
    /// ]
    /// </code>
    ///
    /// This is what separates COSE_Encrypt from COSE_Encrypt0 and COSE_Mac
    /// from COSE_Mac0. The bare forms assume both parties already hold the
    /// key; the enveloped forms solve the distribution problem INSIDE the
    /// message. One content key protects the body, and one recipient structure
    /// per party delivers that key by a route only that party can walk.
    ///
    /// Two routes are implemented, and they are the two reachable from a
    /// pre-shared secret:
    ///
    /// - "direct" [RFC 9053, Section 6.1.1] - the recipient's key IS the
    ///   content key. Nothing is transported: the protected bucket and the
    ///   ciphertext are both empty, and the structure exists only to name a
    ///   key identifier.
    /// - AES key wrap [RFC 9053 Section 6.2.1, RFC 3394] - the content key is
    ///   encrypted under a key-encryption key the recipient holds.
    ///
    /// NOT implemented: ECDH key agreement and the HKDF-based key derivations.
    /// Both need COSE_KDF_Context [RFC 9053, Section 5.2], a structure of its
    /// own carrying PartyU and PartyV information and the supplementary public
    /// info - and one whose fields, got subtly wrong, derive a key that agrees
    /// only with an implementation making the same mistake. It is a piece of
    /// work in its own right rather than a variation on this one.
    ///
    /// The "? recipients" at the end is not decoration: recipient structures
    /// NEST, so a key can be wrapped to a group whose key is in turn wrapped
    /// to its members.
    ///
    /// WHAT A RECIPIENT LIST COSTS, and it is not nothing: every recipient of
    /// a COSE_Mac holds the same content key, so with more than one of them a
    /// tag no longer even tells the recipients apart - any of them can produce
    /// a message the others will accept as coming from the sender. RFC 9052
    /// Section 8.2 puts it plainly: a MAC provides "either no or very limited
    /// data origination". For an encrypted message the same is true of its
    /// integrity guarantee.
    /// </summary>
    public sealed class COSERecipient
    {

        #region Properties

        /// <summary>
        /// The serialized protected header bucket - empty for both implemented
        /// routes.
        /// </summary>
        public Byte[]                        ProtectedHeaderBytes    { get; }

        /// <summary>
        /// The protected header parameters.
        /// </summary>
        public COSEHeaders                   ProtectedHeader         { get; }

        /// <summary>
        /// The unprotected header parameters, which is where the algorithm and
        /// the key identifier live here.
        /// </summary>
        public COSEHeaders                   UnprotectedHeader       { get; }

        /// <summary>
        /// The wrapped content key, or a zero-length string for "direct".
        /// </summary>
        public Byte[]                        Ciphertext              { get; }

        /// <summary>
        /// Recipient structures nested below this one.
        /// </summary>
        public IReadOnlyList<COSERecipient>  Recipients              { get; }

        /// <summary>
        /// The recipient algorithm, protected header bucket first.
        /// </summary>
        public COSEAlgorithm?                Algorithm

            => ProtectedHeader.  Algorithm ??
               UnprotectedHeader.Algorithm;

        /// <summary>
        /// The key identifier, protected header bucket first.
        /// </summary>
        public Byte[]?                       KeyIdentifier

            => ProtectedHeader.  KeyIdentifier ??
               UnprotectedHeader.KeyIdentifier;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new COSE recipient structure from its parts.
        /// </summary>
        /// <param name="ProtectedHeaderBytes">The serialized protected header bucket.</param>
        /// <param name="UnprotectedHeader">The unprotected header parameters.</param>
        /// <param name="Ciphertext">The wrapped content key, or an empty array.</param>
        /// <param name="Recipients">Optional recipient structures nested below this one.</param>
        public COSERecipient(Byte[]                       ProtectedHeaderBytes,
                             COSEHeaders?                 UnprotectedHeader,
                             Byte[]                       Ciphertext,
                             IEnumerable<COSERecipient>?  Recipients   = null)
        {

            if (!COSEHeaders.TryParseProtected(ProtectedHeaderBytes, out var protectedHeader, out var errorResponse))
                throw new COSEException($"The protected header bucket is invalid: {errorResponse}");

            this.ProtectedHeaderBytes  = ProtectedHeaderBytes;
            this.ProtectedHeader       = protectedHeader;
            this.UnprotectedHeader     = UnprotectedHeader ?? COSEHeaders.Empty;
            this.Ciphertext            = Ciphertext;
            this.Recipients            = Recipients?.ToArray() ?? [];

        }

        #endregion


        #region (static) Direct (Key, KeyIdentifier = null, Recipients = null)

        /// <summary>
        /// A "direct" recipient: the key it names IS the content key.
        ///
        /// RFC 9053 Section 6.1.1 requires the protected bucket to be zero
        /// length, and nothing is carried in the ciphertext either - so this
        /// structure conveys an algorithm and a key identifier and no key
        /// material at all.
        /// </summary>
        /// <param name="Key">The symmetric COSE key that is the content key.</param>
        /// <param name="KeyIdentifier">An optional key identifier, defaulting to the key's own.</param>
        /// <param name="Recipients">Optional recipient structures nested below this one.</param>
        public static COSERecipient Direct(COSEKey                      Key,
                                           Byte[]?                      KeyIdentifier   = null,
                                           IEnumerable<COSERecipient>?  Recipients      = null)
        {

            EnsureSymmetric(Key, "A direct recipient");

            return new COSERecipient(
                       [],
                       HeadersFor(COSEAlgorithm.Direct, KeyIdentifier ?? Key.KeyIdentifier),
                       [],
                       Recipients
                   );

        }

        #endregion

        #region (static) KeyWrap(ContentKey, KeyEncryptionKey, KeyIdentifier = null, Recipients = null)

        /// <summary>
        /// A key-wrap recipient: the content key, encrypted under the given
        /// key-encryption key.
        ///
        /// The algorithm follows from the width of the KEY-ENCRYPTION key
        /// rather than from the content key, which is the direction people get
        /// wrong: A256KW wraps a 128-bit content key perfectly well.
        /// </summary>
        /// <param name="ContentKey">The content key to wrap.</param>
        /// <param name="KeyEncryptionKey">The symmetric COSE key to wrap it under.</param>
        /// <param name="KeyIdentifier">An optional key identifier, defaulting to the key's own.</param>
        /// <param name="Recipients">Optional recipient structures nested below this one.</param>
        public static COSERecipient KeyWrap(Byte[]                       ContentKey,
                                            COSEKey                      KeyEncryptionKey,
                                            Byte[]?                      KeyIdentifier   = null,
                                            IEnumerable<COSERecipient>?  Recipients      = null)
        {

            EnsureSymmetric(KeyEncryptionKey, "A key-wrap recipient");

            var kek        = KeyEncryptionKey.K!;
            var algorithm  = COSEAlgorithm.ForKeyWrap(kek.Length);

            return new COSERecipient(
                       [],
                       HeadersFor(algorithm, KeyIdentifier ?? KeyEncryptionKey.KeyIdentifier),
                       algorithm.WrapKey(ContentKey, kek),
                       Recipients
                   );

        }

        #endregion

        #region ContentKey(Key)

        /// <summary>
        /// The content key this recipient carries, or null when the given key
        /// is not the one it was built for.
        ///
        /// Null rather than an exception, and null rather than a reason: a
        /// party holding several keys tries them in turn, and "not this one"
        /// is the ordinary answer rather than an error. For key wrap it is
        /// also the ONLY honest answer - RFC 3394's integrity check is what
        /// distinguishes a wrong key from a right one, and saying more about
        /// which would say something about the key.
        /// </summary>
        /// <param name="Key">A symmetric COSE key this party holds.</param>
        public Byte[]? ContentKey(COSEKey Key)
        {

            var algorithm = Algorithm;

            if (!algorithm.HasValue || Key.KeyType != COSEKeyType.Symmetric || Key.K is null)
                return null;

            if (algorithm.Value.Family == COSEAlgorithmFamily.Direct)
            {

                // "When this algorithm is used, the 'protected' field MUST be
                // zero length" [RFC 9053, Section 6.1.1] - and a non-empty
                // ciphertext would be key material this route does not carry.
                if (ProtectedHeaderBytes.Length != 0 || Ciphertext.Length != 0)
                    return null;

                return Key.K;

            }

            if (algorithm.Value.Family == COSEAlgorithmFamily.KeyWrap)
            {

                // A key-encryption key of the wrong width for the algorithm
                // named is somebody else's recipient rather than an error.
                if (algorithm.Value.KeySizeInBytes != Key.K.Length)
                    return null;

                return algorithm.Value.UnwrapKey(Ciphertext, Key.K);

            }

            return null;

        }

        #endregion


        #region (static) TryParse(CBOR, out Recipient, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR value as a COSE_recipient.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE recipient.</param>
        /// <param name="Recipient">The parsed COSE recipient.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                            CBOR,
                                       [NotNullWhen(true)]  out COSERecipient?  Recipient,
                                       [NotNullWhen(false)] out String?         ErrorResponse)
        {

            Recipient = null;

            if (CBOR.Kind != CBORValueKind.Array)
            {
                ErrorResponse = $"A COSE_recipient must be a CBOR array, but was a CBOR {CBOR.Kind}!";
                return false;
            }

            if (CBOR.Count != 3 && CBOR.Count != 4)
            {
                ErrorResponse = $"A COSE_recipient must be a CBOR array of 3 or 4 elements, but had {CBOR.Count} element(s)!";
                return false;
            }

            var items = CBOR.AsArray();

            if (!items[0].TryGetBytes(out var protectedHeaderBytes))
            {
                ErrorResponse = "The protected header bucket of a COSE_recipient must be a byte string!";
                return false;
            }

            if (!COSEHeaders.TryParse(items[1], out var unprotectedHeader, out ErrorResponse))
                return false;

            Byte[]? ciphertext = [];

            if (items[2].Kind != CBORValueKind.Null &&
                !items[2].TryGetBytes(out ciphertext))
            {
                ErrorResponse = "The ciphertext of a COSE_recipient must be a byte string, or null!";
                return false;
            }

            var nested = new List<COSERecipient>();

            if (CBOR.Count == 4)
            {

                if (items[3].Kind != CBORValueKind.Array || items[3].Count == 0)
                {
                    ErrorResponse = "The nested recipients of a COSE_recipient must be a non-empty CBOR array!";
                    return false;
                }

                foreach (var item in items[3].AsArray())
                {

                    if (!TryParse(item, out var one, out ErrorResponse))
                        return false;

                    nested.Add(one);

                }

            }

            try
            {

                Recipient      = new COSERecipient(protectedHeaderBytes, unprotectedHeader, ciphertext ?? [], nested);
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

        #region ToCBOR()

        /// <summary>
        /// Return a CBOR representation of this COSE recipient.
        /// </summary>
        public CBORValue ToCBOR()
        {

            var items = new List<CBORValue> {
                            CBORValue.FromBytes(ProtectedHeaderBytes),
                            UnprotectedHeader.ToCBOR(),
                            CBORValue.FromBytes(Ciphertext)
                        };

            if (Recipients.Count > 0)
                items.Add(CBORValue.FromArray(Recipients.Select(static recipient => recipient.ToCBOR())));

            return CBORValue.FromArray(items);

        }

        #endregion


        #region (private static) helpers

        private static void EnsureSymmetric(COSEKey Key, String What)
        {

            if (Key.KeyType != COSEKeyType.Symmetric || Key.K is null)
                throw new COSEException($"{What} needs a COSE key of key type Symmetric, but a key of key type {Key.KeyType} was given!");

        }

        /// <summary>
        /// The unprotected bucket of a recipient: its algorithm, and its key
        /// identifier when there is one.
        ///
        /// Both live in the UNPROTECTED bucket, which is what the published
        /// examples do and what "direct" requires - its protected bucket must
        /// be zero length, so there is nowhere else for the algorithm to go.
        /// </summary>
        private static COSEHeaders HeadersFor(COSEAlgorithm Algorithm, Byte[]? KeyIdentifier)
        {

            var parameters = new List<(CBORValue, CBORValue)> {
                                 (COSEHeaderLabel.Algorithm, Algorithm.ToCBOR())
                             };

            if (KeyIdentifier is not null)
                parameters.Add((COSEHeaderLabel.KeyIdentifier, CBORValue.FromBytes(KeyIdentifier)));

            return new COSEHeaders([.. parameters]);

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{(Algorithm.HasValue ? Algorithm.Value.Name : "unknown")} recipient, {Ciphertext.Length} bytes";

        #endregion

    }

}
