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
using System.Security.Cryptography;

using Org.BouncyCastle.X509;

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
    /// Four routes are implemented. Two of them start from a secret both
    /// parties already hold:
    ///
    /// - "direct" [RFC 9053, Section 6.1.1] - the recipient's key IS the
    ///   content key. Nothing is transported: the protected bucket and the
    ///   ciphertext are both empty, and the structure exists only to name a
    ///   key identifier.
    /// - AES key wrap [RFC 9053 Section 6.2.1, RFC 3394] - the content key is
    ///   encrypted under a key-encryption key the recipient holds.
    ///
    /// The other two need nothing shared at all, only the recipient's public
    /// key [RFC 9053, Section 6.3]; how the key is agreed is told in
    /// COSEKeyAgreement:
    ///
    /// - ECDH-ES + HKDF - the agreed key IS the content key, so a message may
    ///   carry only this one recipient.
    /// - ECDH-ES + AES key wrap - the agreed key wraps the content key, so
    ///   every recipient gets the same content key through a key of its own.
    ///
    /// NOT implemented: the static-static variants (ECDH-SS), in which the
    /// sender agrees the key from a long-lived key of its own. What they add
    /// is that the recipient learns who sent the message, and a signature
    /// inside the envelope says that to everybody rather than to the
    /// recipient alone.
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

        /// <summary>
        /// The thumbprint of the certificate this recipient names [RFC 9360],
        /// taken from the "x5t" header parameter (label 34), protected header
        /// bucket first. Throws when the parameter is present but is not a
        /// COSE_CertHash.
        /// </summary>
        public COSECertificateHash?          CertificateThumbprint
        {
            get
            {

                if (!ProtectedHeader.  TryGet(COSEHeaderLabel.X5T, out var value) &&
                    !UnprotectedHeader.TryGet(COSEHeaderLabel.X5T, out value))
                {
                    return null;
                }

                if (!COSECertificateHash.TryParse(value, out var thumbprint, out var errorResponse))
                    throw new COSEException($"The certificate thumbprint of this COSE recipient is invalid: {errorResponse}");

                return thumbprint;

            }
        }

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

        #region IsFor(Certificate)

        /// <summary>
        /// Whether this recipient may be meant for the key of the given
        /// certificate: false when it names a certificate by "x5t" and that is
        /// another one, or when what it names is not a thumbprint at all.
        ///
        /// A recipient that names no certificate may be meant for anybody, and
        /// is tried as before. One that names another certificate is somebody
        /// else's - or the recipient's own, under a key it no longer uses - and
        /// trying the key on it would only ever fail later and less clearly.
        ///
        /// The thumbprint names a certificate; it does not vouch for one.
        /// Whether the certificate belongs to the key it is used with, and
        /// whether it may be trusted, is the caller's to have established.
        /// COSE.TS draws the same line.
        /// </summary>
        /// <param name="Certificate">The certificate of the key the caller holds.</param>
        public Boolean IsFor(X509Certificate Certificate)
        {

            COSECertificateHash? thumbprint;

            try
            {
                thumbprint = CertificateThumbprint;
            }
            catch (COSEException)
            {
                return false;
            }

            return thumbprint is null ||
                   thumbprint.Matches(Certificate, out _);

        }

        #endregion

        #region (static) ECDHDirect (RecipientKey, EphemeralKey, ContentAlgorithm, ...)

        /// <summary>
        /// An ECDH-ES recipient with direct key agreement [RFC 9053, Section
        /// 6.3]: the key agreed with the recipient's public key IS the content
        /// key. Returns the structure together with that content key, which
        /// the message is then encrypted or authenticated with and which never
        /// travels.
        ///
        /// The content algorithm is part of the derivation - the key is
        /// derived FOR it - so it has to be the algorithm of the message. The
        /// ephemeral key is a private key on the recipient's curve, made for
        /// this one message.
        /// </summary>
        /// <param name="RecipientKey">The recipient's public key.</param>
        /// <param name="EphemeralKey">A private key on the same curve, made for this one message.</param>
        /// <param name="ContentAlgorithm">The algorithm the content key is for.</param>
        /// <param name="Algorithm">ECDH-ES + HKDF-256 (the default) or + HKDF-512.</param>
        /// <param name="Salt">An optional HKDF salt (header parameter -20).</param>
        /// <param name="PartyU">Optional PartyU information (header parameters -21 to -23).</param>
        /// <param name="PartyV">Optional PartyV information (header parameters -24 to -26).</param>
        /// <param name="KeyIdentifier">An optional key identifier, defaulting to the recipient key's own.</param>
        /// <param name="Recipients">Optional recipient structures nested below this one.</param>
        /// <param name="CertificateThumbprint">The optional thumbprint of the recipient's certificate, written as "x5t" (header parameter 34) [RFC 9360].</param>
        public static (COSERecipient Recipient, COSEKey ContentKey) ECDHDirect(COSEKey                      RecipientKey,
                                                                               COSEKey                      EphemeralKey,
                                                                               COSEAlgorithm                ContentAlgorithm,
                                                                               COSEAlgorithm?               Algorithm               = null,
                                                                               Byte[]?                      Salt                    = null,
                                                                               COSEPartyInfo?               PartyU                  = null,
                                                                               COSEPartyInfo?               PartyV                  = null,
                                                                               Byte[]?                      KeyIdentifier           = null,
                                                                               IEnumerable<COSERecipient>?  Recipients              = null,
                                                                               COSECertificateHash?         CertificateThumbprint   = null)
        {

            var algorithm = Algorithm ?? COSEAlgorithm.ECDH_ES_HKDF_256;

            if (algorithm.Family != COSEAlgorithmFamily.ECDHDirect)
                throw new COSEException($"The COSE algorithm '{algorithm.Name}' is not a direct ECDH key agreement algorithm!");

            var keyLength            = ContentKeySizeOf(ContentAlgorithm);
            var protectedHeaderBytes = COSEHeaders.Create(algorithm).ToProtectedByteArray();
            var secret               = COSEKeyAgreement.SharedSecret(EphemeralKey, RecipientKey);

            var contentKey = COSEKeyAgreement.DeriveKey(
                                 HashOf(algorithm),
                                 secret,
                                 COSEKeyAgreement.KDFContext(ContentAlgorithm, keyLength, protectedHeaderBytes, PartyU, PartyV),
                                 keyLength,
                                 Salt
                             );

            return (
                       new COSERecipient(
                           protectedHeaderBytes,
                           ECDHHeadersFor(EphemeralKey, Salt, PartyU, PartyV, KeyIdentifier ?? RecipientKey.KeyIdentifier, CertificateThumbprint),
                           [],
                           Recipients
                       ),
                       COSEKey.FromSymmetricKey(contentKey, Algorithm: ContentAlgorithm)
                   );

        }

        #endregion

        #region (static) ECDHKeyWrap(ContentKey, RecipientKey, EphemeralKey, Algorithm, ...)

        /// <summary>
        /// An ECDH-ES recipient with key wrap [RFC 9053, Section 6.3]: the key
        /// agreed with the recipient's public key wraps the content key.
        ///
        /// The algorithm - ECDH-ES + A128KW, + A192KW or + A256KW - names the
        /// width of the key-encryption key, which is agreed rather than held.
        /// Unlike direct agreement, any number of these may share a message:
        /// each recipient unwraps the one content key with a key of its own.
        /// </summary>
        /// <param name="ContentKey">The content key to deliver.</param>
        /// <param name="RecipientKey">The recipient's public key.</param>
        /// <param name="EphemeralKey">A private key on the same curve, made for this one message.</param>
        /// <param name="Algorithm">ECDH-ES + A128KW, + A192KW or + A256KW.</param>
        /// <param name="Salt">An optional HKDF salt (header parameter -20).</param>
        /// <param name="PartyU">Optional PartyU information (header parameters -21 to -23).</param>
        /// <param name="PartyV">Optional PartyV information (header parameters -24 to -26).</param>
        /// <param name="KeyIdentifier">An optional key identifier, defaulting to the recipient key's own.</param>
        /// <param name="Recipients">Optional recipient structures nested below this one.</param>
        /// <param name="CertificateThumbprint">The optional thumbprint of the recipient's certificate, written as "x5t" (header parameter 34) [RFC 9360].</param>
        public static COSERecipient ECDHKeyWrap(Byte[]                       ContentKey,
                                                COSEKey                      RecipientKey,
                                                COSEKey                      EphemeralKey,
                                                COSEAlgorithm                Algorithm,
                                                Byte[]?                      Salt                    = null,
                                                COSEPartyInfo?               PartyU                  = null,
                                                COSEPartyInfo?               PartyV                  = null,
                                                Byte[]?                      KeyIdentifier           = null,
                                                IEnumerable<COSERecipient>?  Recipients              = null,
                                                COSECertificateHash?         CertificateThumbprint   = null)
        {

            if (Algorithm.Family != COSEAlgorithmFamily.ECDHKeyWrap || Algorithm.KeySizeInBytes is not Int32 keySize)
                throw new COSEException($"The COSE algorithm '{Algorithm.Name}' is not an ECDH key agreement with key wrap!");

            var keyWrap              = COSEAlgorithm.ForKeyWrap(keySize);
            var protectedHeaderBytes = COSEHeaders.Create(Algorithm).ToProtectedByteArray();
            var secret               = COSEKeyAgreement.SharedSecret(EphemeralKey, RecipientKey);

            var keyEncryptionKey = COSEKeyAgreement.DeriveKey(
                                       HashOf(Algorithm),
                                       secret,
                                       COSEKeyAgreement.KDFContext(keyWrap, keySize, protectedHeaderBytes, PartyU, PartyV),
                                       keySize,
                                       Salt
                                   );

            return new COSERecipient(
                       protectedHeaderBytes,
                       ECDHHeadersFor(EphemeralKey, Salt, PartyU, PartyV, KeyIdentifier ?? RecipientKey.KeyIdentifier, CertificateThumbprint),
                       keyWrap.WrapKey(ContentKey, keyEncryptionKey),
                       Recipients
                   );

        }

        #endregion

        #region ContentKey(Key, ContentAlgorithm = null)

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
        /// <param name="Key">A COSE key this party holds: symmetric for "direct" and key wrap, a private EC2 or OKP key for ECDH-ES.</param>
        /// <param name="ContentAlgorithm">The algorithm of the enclosing message. Only direct key agreement needs it, because there the content key is derived FOR it; the enclosing message passes it in.</param>
        public Byte[]? ContentKey(COSEKey         Key,
                                  COSEAlgorithm?  ContentAlgorithm   = null)
        {

            var algorithm = Algorithm;

            if (!algorithm.HasValue)
                return null;

            if (algorithm.Value.Family is COSEAlgorithmFamily.ECDHDirect or COSEAlgorithmFamily.ECDHKeyWrap)
                return AgreedContentKey(algorithm.Value, Key, ContentAlgorithm);

            if (Key.KeyType != COSEKeyType.Symmetric || Key.K is null)
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


        #region (private) AgreedContentKey(Algorithm, Key, ContentAlgorithm)

        /// <summary>
        /// The content key of an ECDH-ES recipient, agreed between the given
        /// private key and the ephemeral key the sender left in the headers.
        ///
        /// Null for every way of not being this recipient's key: a key without
        /// a private half, one on another curve, an ephemeral key that is
        /// missing or not a point, and - for direct agreement - a call that
        /// does not say which content algorithm the key is for, since the
        /// derivation depends on it and a guess would derive a key for the
        /// wrong one.
        /// </summary>
        private Byte[]? AgreedContentKey(COSEAlgorithm   Algorithm,
                                         COSEKey         Key,
                                         COSEAlgorithm?  ContentAlgorithm)
        {

            if ((Key.KeyType != COSEKeyType.EC2 && Key.KeyType != COSEKeyType.OKP) || !Key.IsPrivate)
                return null;

            if (!TryGetParameter(COSEHeaderLabel.EphemeralKey, out var ephemeralValue) ||
                !COSEKey.TryParse(ephemeralValue, out var ephemeral, out _))
            {
                return null;
            }

            if (ephemeral.KeyType != Key.KeyType || ephemeral.Curve != Key.Curve)
                return null;

            Byte[]         secret;
            Byte[]?        salt;
            COSEPartyInfo  partyU;
            COSEPartyInfo  partyV;

            try
            {
                secret  = COSEKeyAgreement.SharedSecret(Key, ephemeral);
                salt    = BytesParameter(COSEHeaderLabel.Salt);
                partyU  = PartyInfo(COSEHeaderLabel.PartyUIdentity, COSEHeaderLabel.PartyUNonce, COSEHeaderLabel.PartyUOther);
                partyV  = PartyInfo(COSEHeaderLabel.PartyVIdentity, COSEHeaderLabel.PartyVNonce, COSEHeaderLabel.PartyVOther);
            }
            catch (COSEException)
            {
                return null;
            }

            if (Algorithm.Family == COSEAlgorithmFamily.ECDHDirect)
            {

                // The agreed key is the content key: nothing to unwrap, and
                // nothing that may pretend to be something to unwrap.
                if (!ContentAlgorithm.HasValue || Ciphertext.Length != 0)
                    return null;

                Int32 keyLength;

                try
                {
                    keyLength = ContentKeySizeOf(ContentAlgorithm.Value);
                }
                catch (COSEException)
                {
                    return null;
                }

                return COSEKeyAgreement.DeriveKey(
                           HashOf(Algorithm),
                           secret,
                           COSEKeyAgreement.KDFContext(ContentAlgorithm.Value, keyLength, ProtectedHeaderBytes, partyU, partyV),
                           keyLength,
                           salt
                       );

            }

            if (Algorithm.KeySizeInBytes is not Int32 keySize)
                return null;

            var keyWrap = COSEAlgorithm.ForKeyWrap(keySize);

            var keyEncryptionKey = COSEKeyAgreement.DeriveKey(
                                       HashOf(Algorithm),
                                       secret,
                                       COSEKeyAgreement.KDFContext(keyWrap, keySize, ProtectedHeaderBytes, partyU, partyV),
                                       keySize,
                                       salt
                                   );

            return keyWrap.UnwrapKey(Ciphertext, keyEncryptionKey);

        }

        /// <summary>A header parameter of this recipient, protected bucket first.</summary>
        private Boolean TryGetParameter(CBORValue Label, out CBORValue Value)

            => ProtectedHeader.  TryGet(Label, out Value) ||
               UnprotectedHeader.TryGet(Label, out Value);

        /// <summary>A byte-string header parameter, or null when absent; any other type is refused.</summary>
        private Byte[]? BytesParameter(CBORValue Label)
        {

            if (!TryGetParameter(Label, out var value))
                return null;

            if (!value.TryGetBytes(out var bytes))
                throw new COSEException($"The ECDH header parameter '{COSEHeaderLabel.Name(Label)}' must be a byte string!");

            return bytes;

        }

        private COSEPartyInfo PartyInfo(CBORValue Identity, CBORValue Nonce, CBORValue Other)
        {

            CBORValue? nonce = null;

            if (TryGetParameter(Nonce, out var nonceValue))
            {

                if (!nonceValue.TryGetBytes(out _) && !nonceValue.TryGetInt64(out _))
                    throw new COSEException("A party nonce must be a byte string or an integer!");

                nonce = nonceValue;

            }

            return new COSEPartyInfo(BytesParameter(Identity), nonce, BytesParameter(Other));

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


        #region (internal static) EnsureRecipientsCompatible(Recipients, What)

        /// <summary>
        /// One that agrees its content key directly with a recipient may have
        /// only that recipient [RFC 9053, Section 6.3], since every other
        /// recipient would have to be handed the key that recipient agreed -
        /// which is the one thing ECDH-ES is there to keep from anybody else.
        /// </summary>
        internal static void EnsureRecipientsCompatible(IReadOnlyCollection<COSERecipient>  Recipients,
                                                        String                              What)
        {

            if (Recipients.Count > 1 &&
                Recipients.Any(static recipient => recipient.Algorithm?.Family == COSEAlgorithmFamily.ECDHDirect))
            {
                throw new COSEException($"{What} with a direct ECDH key agreement recipient may have only that one recipient!");
            }

        }

        #endregion


        #region (private static) helpers

        /// <summary>
        /// The width of the key a content algorithm needs: what AES-GCM or
        /// ChaCha20/Poly1305 names,
        /// and for HMAC the width of its hash - which is what the published
        /// examples derive for HMAC 512/512, and what RFC 9053 Section 3.1 asks
        /// an HMAC key to be.
        /// </summary>
        private static Int32 ContentKeySizeOf(COSEAlgorithm Algorithm)
        {

            if (Algorithm.IsContentEncryption && Algorithm.KeySizeInBytes is Int32 keySize)
                return keySize;

            if (Algorithm.Family == COSEAlgorithmFamily.HMAC)
            {
                if (Algorithm.HashAlgorithm == HashAlgorithmName.SHA256) return 32;
                if (Algorithm.HashAlgorithm == HashAlgorithmName.SHA384) return 48;
                if (Algorithm.HashAlgorithm == HashAlgorithmName.SHA512) return 64;
            }

            throw new COSEException($"The COSE algorithm '{Algorithm.Name}' is not a content algorithm a key can be agreed for!");

        }

        private static HashAlgorithmName HashOf(COSEAlgorithm Algorithm)

            => Algorithm.HashAlgorithm
                   ?? throw new COSEException($"The COSE algorithm '{Algorithm.Name}' names no HKDF hash!");

        /// <summary>
        /// The unprotected bucket of an ECDH-ES recipient: the ephemeral public
        /// key first, then whatever the sender adds to the KDF context, then
        /// the key identifier of the recipient key - the order of the published
        /// examples - and last the thumbprint of the recipient's certificate,
        /// which they do not use.
        /// </summary>
        private static COSEHeaders ECDHHeadersFor(COSEKey               EphemeralKey,
                                                  Byte[]?               Salt,
                                                  COSEPartyInfo?        PartyU,
                                                  COSEPartyInfo?        PartyV,
                                                  Byte[]?               KeyIdentifier,
                                                  COSECertificateHash?  CertificateThumbprint)
        {

            var parameters = new List<(CBORValue, CBORValue)> {
                                 (COSEHeaderLabel.EphemeralKey, EphemeralPublicKey(EphemeralKey))
                             };

            void Bytes(CBORValue Label, Byte[]? Value)
            {
                if (Value is not null)
                    parameters.Add((Label, CBORValue.FromBytes(Value)));
            }

            void Nonce(CBORValue Label, CBORValue? Value)
            {
                if (Value is CBORValue nonce)
                    parameters.Add((Label, nonce));
            }

            Bytes(COSEHeaderLabel.Salt,           Salt);
            Bytes(COSEHeaderLabel.PartyUIdentity, PartyU?.Identity);
            Nonce(COSEHeaderLabel.PartyUNonce,    PartyU?.Nonce);
            Bytes(COSEHeaderLabel.PartyUOther,    PartyU?.Other);
            Bytes(COSEHeaderLabel.PartyVIdentity, PartyV?.Identity);
            Nonce(COSEHeaderLabel.PartyVNonce,    PartyV?.Nonce);
            Bytes(COSEHeaderLabel.PartyVOther,    PartyV?.Other);

            if (KeyIdentifier is not null)
                parameters.Add((COSEHeaderLabel.KeyIdentifier, CBORValue.FromBytes(KeyIdentifier)));

            if (CertificateThumbprint is not null)
                parameters.Add((COSEHeaderLabel.X5T, CertificateThumbprint.ToCBOR()));

            return new COSEHeaders([.. parameters]);

        }

        /// <summary>
        /// The ephemeral key as it travels: its key type, curve and public
        /// part, and nothing more - no key identifier, no algorithm, and above
        /// all no private half, whatever the key handed in carries.
        /// </summary>
        private static CBORValue EphemeralPublicKey(COSEKey Key)
        {

            var curve = COSEKeyAgreement.RequireAgreementCurve(Key, "The ephemeral key");

            if (Key.X is null || (Key.KeyType == COSEKeyType.EC2 && Key.Y is null))
                throw new COSEException("The ephemeral key carries no public key!");

            var entries = new List<KeyValuePair<CBORValue, CBORValue>> {
                              new (COSEKey.KeyTypeLabel, CBORValue.FromInt64((Int64) Key.KeyType)),
                              new (COSEKey.CurveLabel,   curve.ToCBOR()),
                              new (COSEKey.XLabel,       CBORValue.FromBytes(Key.X))
                          };

            if (Key.KeyType == COSEKeyType.EC2)
                entries.Add(new (COSEKey.YLabel, CBORValue.FromBytes(Key.Y!)));

            return CBORValue.FromMap(entries);

        }

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
