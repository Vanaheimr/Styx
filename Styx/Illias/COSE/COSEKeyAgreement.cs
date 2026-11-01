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

using System.Security.Cryptography;

using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// What one party says about itself in a COSE_KDF_Context [RFC 9053,
    /// Section 5.2]: who it is, a nonce, and anything else both sides agree
    /// on. Every field is optional and travels as nil when absent, which is
    /// what all of the published examples do. When present they travel in the
    /// header parameters -21 to -26 of the recipient structure, so that the
    /// receiving side can rebuild the same context.
    /// </summary>
    /// <param name="Identity">The party's identity, or null.</param>
    /// <param name="Nonce">A nonce, a byte string or an integer, or null.</param>
    /// <param name="Other">Other information both parties agree on, or null.</param>
    public sealed record COSEPartyInfo(Byte[]?     Identity   = null,
                                       CBORValue?  Nonce      = null,
                                       Byte[]?     Other      = null);


    /// <summary>
    /// ECDH key agreement and the HKDF key derivation over it, as COSE uses
    /// them for ECDH-ES [RFC 9053, Sections 5 and 6.3].
    ///
    /// The point of the whole construction is that a sender can encrypt to a
    /// party whose PUBLIC key is all it knows. It makes a key pair for this
    /// one message - the ephemeral key, "ES" for ephemeral-static - combines
    /// its private half with the recipient's public key, and sends the
    /// ephemeral public half along. The recipient combines that with its own
    /// private key and arrives at the same secret. Nobody else can.
    ///
    /// That secret is not used as a key. It is a point coordinate rather than
    /// a uniformly random string, and the same pair of keys always yields the
    /// same one. HKDF turns it into a key, and COSE_KDF_Context is what it is
    /// turned FOR. Three of its fields are where implementations disagree,
    /// and all three are pinned here against the published examples of the
    /// COSE working group:
    ///
    /// - AlgorithmID names what the derived key is for, not how it was
    ///   derived: the content algorithm when the agreed key IS the content
    ///   key, the key-wrap algorithm (A128KW) when it wraps one. Never the
    ///   ECDH algorithm itself.
    /// - keyDataLength is in BITS.
    /// - protected is the recipient's protected bucket as it travels, which is
    ///   where the ECDH algorithm is named - so a recipient whose algorithm is
    ///   rewritten derives a different key.
    ///
    /// The ephemeral private key is the caller's and never drawn here, for the
    /// reason the nonce and the content key are: so that every message can be
    /// written again, byte for byte, and two implementations can be compared
    /// on the bytes. Making a fresh one for every message is the caller's
    /// duty; a reused one gives every message to the same recipient the same
    /// key.
    ///
    /// This is the counterpart of COSE.TS's src/ecdh.ts, and the cross-signing
    /// conformance suite holds the two to the same bytes.
    /// </summary>
    public static class COSEKeyAgreement
    {

        #region IsAgreementCurve(Curve)

        /// <summary>
        /// Whether this implementation agrees keys on the given curve: every
        /// EC2 curve it computes with, and X25519 and X448.
        ///
        /// Ed25519 and Ed448 are not among them. They share their fields with
        /// X25519 and X448, but COSE registers them as separate curves, and a
        /// signing key used for key agreement is a key used for two purposes.
        /// </summary>
        /// <param name="Curve">An elliptic curve.</param>
        public static Boolean IsAgreementCurve(COSECurve Curve)

            => Curve.KeyType == COSEKeyType.EC2
                   ? Curve.DomainParameters is not null
                   : Curve == COSECurve.X25519 || Curve == COSECurve.X448;

        #endregion

        #region SharedSecret(PrivateKey, PublicKey)

        /// <summary>
        /// The raw ECDH secret of a private key and a public key on the same
        /// curve.
        ///
        /// For EC2 it is the x coordinate of the product, padded to the width
        /// of the field - a secret that dropped its leading zero octets would
        /// differ from the other party's about one time in 256, which is the
        /// same trap as an unpadded r||s. For X25519 and X448 it is the
        /// u-coordinate of RFC 7748, and an all-zero one - the result of a
        /// public key of small order, which RFC 7748 Section 6.1 says to check
        /// for, since its secret is known to everybody - is refused.
        /// </summary>
        /// <param name="PrivateKey">A COSE key holding a private key.</param>
        /// <param name="PublicKey">A COSE key holding the other party's public key.</param>
        public static Byte[] SharedSecret(COSEKey PrivateKey,
                                          COSEKey PublicKey)
        {

            var curve = RequireAgreementCurve(PrivateKey, "The private key");

            if (RequireAgreementCurve(PublicKey, "The public key") != curve)
                throw new COSEException($"ECDH needs both keys on the same curve, but they are on '{curve.Name}' and '{PublicKey.Curve?.Name ?? "an unknown curve"}'!");

            if (!PrivateKey.TryToPrivateKey(out var privateKey, out var errorResponse))
                throw new COSEException($"ECDH needs a private key: {errorResponse}");

            if (!PublicKey.TryToPublicKey(out var publicKey, out errorResponse))
                throw new COSEException($"ECDH needs the other party's public key: {errorResponse}");

            try
            {

                switch (privateKey, publicKey)
                {

                    case (ECPrivateKeyParameters ecPrivate, ECPublicKeyParameters ecPublic):

                        var ecdh = new ECDHBasicAgreement();
                        ecdh.Init(ecPrivate);

                        return BigIntegers.AsUnsignedByteArray(curve.FieldSizeInBytes!.Value,
                                                               ecdh.CalculateAgreement(ecPublic));

                    case (X25519PrivateKeyParameters x25519Private, X25519PublicKeyParameters x25519Public):

                        var x25519 = new X25519Agreement();
                        x25519.Init(x25519Private);

                        var x25519Secret = new Byte[x25519.AgreementSize];
                        x25519.CalculateAgreement(x25519Public, x25519Secret, 0);

                        return x25519Secret;

                    case (X448PrivateKeyParameters x448Private, X448PublicKeyParameters x448Public):

                        var x448 = new X448Agreement();
                        x448.Init(x448Private);

                        var x448Secret = new Byte[x448.AgreementSize];
                        x448.CalculateAgreement(x448Public, x448Secret, 0);

                        return x448Secret;

                }

            }
            catch (Exception e) when (e is not COSEException)
            {
                // BouncyCastle refuses an all-zero X25519 or X448 secret, and
                // an EC product at infinity, with exceptions of its own.
                throw new COSEException($"No key could be agreed on the curve '{curve.Name}': {e.Message}");
            }

            throw new COSEException($"No key could be agreed on the curve '{curve.Name}'!");

        }

        #endregion

        #region KDFContext(AlgorithmID, KeyLengthInBytes, ProtectedHeaderBytes, PartyU = null, PartyV = null)

        /// <summary>
        /// The encoded COSE_KDF_Context [RFC 9053, Section 5.2]:
        ///
        /// <code>
        /// COSE_KDF_Context = [
        ///     AlgorithmID : int / tstr,
        ///     PartyUInfo  : [ identity : bstr / nil, nonce : bstr / int / nil, other : bstr / nil ],
        ///     PartyVInfo  : [ identity : bstr / nil, nonce : bstr / int / nil, other : bstr / nil ],
        ///     SuppPubInfo : [ keyDataLength : uint, protected : empty_or_serialized_map ]
        /// ]
        /// </code>
        ///
        /// SuppPubInfo has an optional "other" and the context an optional
        /// SuppPrivInfo, both mutually agreed values outside the message;
        /// neither is written here, and no header parameter carries either.
        /// </summary>
        /// <param name="AlgorithmID">What the derived key is for: the content algorithm, or the key-wrap algorithm.</param>
        /// <param name="KeyLengthInBytes">The length of the derived key in BYTES - written as bits.</param>
        /// <param name="ProtectedHeaderBytes">The protected bucket of the recipient, as it travels.</param>
        /// <param name="PartyU">What the sender says about itself.</param>
        /// <param name="PartyV">What is said about the recipient.</param>
        public static Byte[] KDFContext(COSEAlgorithm   AlgorithmID,
                                        Int32           KeyLengthInBytes,
                                        Byte[]          ProtectedHeaderBytes,
                                        COSEPartyInfo?  PartyU   = null,
                                        COSEPartyInfo?  PartyV   = null)
        {

            var writer = new CBORWriter();

            writer.WriteStartArray(4);
            writer.WriteInt64(AlgorithmID.Value);
            WritePartyInfo(writer, PartyU);
            WritePartyInfo(writer, PartyV);
            writer.WriteStartArray(2);
            writer.WriteUInt64((UInt64) KeyLengthInBytes * 8);
            writer.WriteByteString(ProtectedHeaderBytes);
            writer.WriteEndArray();
            writer.WriteEndArray();

            return writer.ToByteArray();

        }

        private static void WritePartyInfo(CBORWriter Writer, COSEPartyInfo? Party)
        {

            Writer.WriteStartArray(3);

            if (Party?.Identity is not null) Writer.WriteByteString(Party.Identity);
            else                             Writer.WriteNull();

            if (Party?.Nonce is CBORValue nonce)
            {
                if      (nonce.TryGetBytes(out var bytes))  Writer.WriteByteString(bytes);
                else if (nonce.TryGetInt64(out var number)) Writer.WriteInt64(number);
                else    throw new COSEException("A party nonce must be a byte string or an integer!");
            }
            else
                Writer.WriteNull();

            if (Party?.Other is not null)    Writer.WriteByteString(Party.Other);
            else                             Writer.WriteNull();

            Writer.WriteEndArray();

        }

        #endregion

        #region DeriveKey(HashAlgorithm, Secret, Context, KeyLengthInBytes, Salt = null)

        /// <summary>
        /// HKDF [RFC 5869] over a raw ECDH secret, as RFC 9053 Section 5.1
        /// uses it: the secret is the input keying material, the encoded
        /// COSE_KDF_Context is the info, and an absent salt is the empty one -
        /// which HKDF reads as a hash's width of zeroes, exactly as the RFC
        /// asks.
        /// </summary>
        /// <param name="HashAlgorithm">HKDF's hash.</param>
        /// <param name="Secret">The raw ECDH secret.</param>
        /// <param name="Context">The encoded COSE_KDF_Context.</param>
        /// <param name="KeyLengthInBytes">The length of the key to derive.</param>
        /// <param name="Salt">An optional salt.</param>
        public static Byte[] DeriveKey(HashAlgorithmName  HashAlgorithm,
                                       Byte[]             Secret,
                                       Byte[]             Context,
                                       Int32              KeyLengthInBytes,
                                       Byte[]?            Salt   = null)

            => HKDF.DeriveKey(HashAlgorithm, Secret, KeyLengthInBytes, Salt ?? [], Context);

        #endregion


        #region (internal) RequireAgreementCurve(Key, What)

        internal static COSECurve RequireAgreementCurve(COSEKey Key, String What)
        {

            if ((Key.KeyType != COSEKeyType.EC2 && Key.KeyType != COSEKeyType.OKP) ||
                !Key.Curve.HasValue ||
                !IsAgreementCurve(Key.Curve.Value))
            {
                throw new COSEException($"{What} is not on a curve this implementation agrees keys on!");
            }

            return Key.Curve.Value;

        }

        #endregion

    }

}
