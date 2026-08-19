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

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An algorithm identifier of the IANA "COSE Algorithms" registry
    /// [RFC 9052, Section 13].
    /// Any integer is a syntactically valid identifier, as algorithms may be
    /// registered after this implementation was written; IsKnown tells whether
    /// this library knows the algorithm, IsSupportedForSigning whether it can
    /// actually create and verify signatures with it.
    /// </summary>
    public readonly struct COSEAlgorithm : IEquatable<COSEAlgorithm>,
                                           IComparable<COSEAlgorithm>,
                                           IComparable
    {

        #region (class) AlgorithmInfo

        /// <summary>
        /// The metadata of a registered COSE algorithm.
        /// </summary>
        /// <param name="Name">The registered name of the algorithm.</param>
        /// <param name="Description">The registered description of the algorithm.</param>
        /// <param name="HashAlgorithm">The message digest algorithm, or null for algorithms that do not hash separately.</param>
        /// <param name="FixedCurve">The elliptic curve this algorithm is defined for, or null when the algorithm leaves the curve to the key.</param>
        /// <param name="IsSupportedForSigning">Whether this implementation can create and verify signatures of this algorithm.</param>
        /// <param name="IsDeprecated">Whether this algorithm was deprecated in favour of a fully-specified one [RFC 9864].</param>
        /// <param name="Family">Which signature machinery this algorithm needs.</param>
        /// <param name="MLDsaParameterSet">The ML-DSA parameter set, or null for everything else.</param>
        private sealed class AlgorithmInfo(String               Name,
                                           String               Description,
                                           COSEAlgorithmFamily  Family,
                                           HashAlgorithmName?   HashAlgorithm,
                                           COSECurve?           FixedCurve,
                                           Boolean              IsSupportedForSigning,
                                           Boolean              IsDeprecated,
                                           MLDsaParameters?     MLDsaParameterSet = null,
                                           Int32?               TagSizeInBytes    = null,
                                           Int32?               KeySizeInBytes    = null)
        {

            public String               Name                     { get; } = Name;
            public String               Description              { get; } = Description;
            public COSEAlgorithmFamily  Family                   { get; } = Family;
            public HashAlgorithmName?   HashAlgorithm            { get; } = HashAlgorithm;
            public COSECurve?           FixedCurve               { get; } = FixedCurve;
            public Boolean              IsSupportedForSigning    { get; } = IsSupportedForSigning;
            public Boolean              IsDeprecated             { get; } = IsDeprecated;
            public MLDsaParameters?     MLDsaParameterSet        { get; } = MLDsaParameterSet;
            public Int32?               TagSizeInBytes           { get; } = TagSizeInBytes;
            public Int32?               KeySizeInBytes           { get; } = KeySizeInBytes;

        }

        #endregion

        #region Data

        /// <summary>
        /// The nonce width AES-GCM is fixed to in COSE: 96 bits
        /// [RFC 9053, Section 4.1].
        /// </summary>
        public const Int32 AESGCMNonceSize = 12;

        /// <summary>
        /// The authentication tag width AES-GCM is fixed to in COSE: 128 bits.
        /// </summary>
        public const Int32 AESGCMTagSize   = 16;

        /// <summary>
        /// The registered algorithms this implementation knows about.
        /// The ECDSA algorithms ES256, ES384 and ES512 leave the choice of the
        /// elliptic curve to the key, which is why RFC 9864 deprecated them in
        /// favour of the fully-specified ESP* and ESB* algorithms. They are
        /// nevertheless the ones deployed everywhere today.
        /// </summary>
        private static readonly Dictionary<Int32, AlgorithmInfo> registry = new () {

            {   -7, new ("ES256",   "ECDSA w/ SHA-256",                              COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA256, null,                      true,  true ) },
            {  -35, new ("ES384",   "ECDSA w/ SHA-384",                              COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA384, null,                      true,  true ) },
            {  -36, new ("ES512",   "ECDSA w/ SHA-512",                              COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA512, null,                      true,  true ) },
            {  -47, new ("ES256K",  "ECDSA using secp256k1 curve and SHA-256",       COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA256, COSECurve.Secp256k1,       true,  false) },

            {   -9, new ("ESP256",  "ECDSA using P-256 curve and SHA-256",           COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA256, COSECurve.P256,            true,  false) },
            {  -51, new ("ESP384",  "ECDSA using P-384 curve and SHA-384",           COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA384, COSECurve.P384,            true,  false) },
            {  -52, new ("ESP512",  "ECDSA using P-521 curve and SHA-512",           COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA512, COSECurve.P521,            true,  false) },

            { -265, new ("ESB256",  "ECDSA using BrainpoolP256r1 curve and SHA-256", COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA256, COSECurve.BrainpoolP256r1, true,  false) },
            { -266, new ("ESB320",  "ECDSA using BrainpoolP320r1 curve and SHA-384", COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA384, COSECurve.BrainpoolP320r1, true,  false) },
            { -267, new ("ESB384",  "ECDSA using BrainpoolP384r1 curve and SHA-384", COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA384, COSECurve.BrainpoolP384r1, true,  false) },
            { -268, new ("ESB512",  "ECDSA using BrainpoolP512r1 curve and SHA-512", COSEAlgorithmFamily.ECDSA, HashAlgorithmName.SHA512, COSECurve.BrainpoolP512r1, true,  false) },

            // EdDSA [RFC 8032]. No digest of its own: the message is signed
            // whole, and the nonce comes from the key and the message.
            {   -8, new ("EdDSA",   "EdDSA",                                         COSEAlgorithmFamily.EdDSA, null,                     null,                      true,  true ) },
            {  -19, new ("Ed25519", "EdDSA using the Ed25519 parameter set",         COSEAlgorithmFamily.EdDSA, null,                     COSECurve.Ed25519,         true,  false) },
            {  -53, new ("Ed448",   "EdDSA using the Ed448 parameter set",           COSEAlgorithmFamily.EdDSA, null,                     COSECurve.Ed448,           true,  false) },

            // ML-DSA [FIPS 204, RFC 9964]. Also pure, and also without a
            // curve: its keys are algorithm key pairs (key type AKP).
            {  -48, new ("ML-DSA-44", "CBOR Object Signing Algorithm for ML-DSA-44", COSEAlgorithmFamily.MLDSA, null,                     null,                      true,  false, MLDsaParameters.ml_dsa_44) },
            {  -49, new ("ML-DSA-65", "CBOR Object Signing Algorithm for ML-DSA-65", COSEAlgorithmFamily.MLDSA, null,                     null,                      true,  false, MLDsaParameters.ml_dsa_65) },
            {  -50, new ("ML-DSA-87", "CBOR Object Signing Algorithm for ML-DSA-87", COSEAlgorithmFamily.MLDSA, null,                     null,                      true,  false, MLDsaParameters.ml_dsa_87) },

            // HMAC [RFC 9053, Section 3.1]. Message authentication rather
            // than signature: symmetric, so IsSupportedForSigning is false for
            // all four and Sign refuses them by family. The name reads "hash
            // size / tag size", and the tag is the LEFTMOST bits of the full
            // HMAC - truncation applies to the output, never to the key.
            {    4, new ("HMAC 256/64",  "HMAC w/ SHA-256 truncated to 64 bits",   COSEAlgorithmFamily.HMAC,  HashAlgorithmName.SHA256, null,                      false, false, null, 8 ) },
            {    5, new ("HMAC 256/256", "HMAC w/ SHA-256",                        COSEAlgorithmFamily.HMAC,  HashAlgorithmName.SHA256, null,                      false, false, null, 32) },
            {    6, new ("HMAC 384/384", "HMAC w/ SHA-384",                        COSEAlgorithmFamily.HMAC,  HashAlgorithmName.SHA384, null,                      false, false, null, 48) },
            {    7, new ("HMAC 512/512", "HMAC w/ SHA-512",                        COSEAlgorithmFamily.HMAC,  HashAlgorithmName.SHA512, null,                      false, false, null, 64) },

            // AES-GCM [RFC 9053, Section 4.1], the content encryption
            // algorithms. COSE fixes the nonce at 96 bits and the tag at 128,
            // so the key width is all the identifier has left to name.
            {    1, new ("A128GCM", "AES-GCM mode w/ 128-bit key, 128-bit tag",   COSEAlgorithmFamily.AESGCM,  null,                     null,                      false, false, null, 16,   16) },
            {    2, new ("A192GCM", "AES-GCM mode w/ 192-bit key, 128-bit tag",   COSEAlgorithmFamily.AESGCM,  null,                     null,                      false, false, null, 16,   24) },
            {    3, new ("A256GCM", "AES-GCM mode w/ 256-bit key, 128-bit tag",   COSEAlgorithmFamily.AESGCM,  null,                     null,                      false, false, null, 16,   32) },

            // AES key wrap [RFC 9053 Section 6.2.1, RFC 3394]. The width named
            // is that of the KEY-ENCRYPTION key, not of the key being wrapped.
            {   -3, new ("A128KW",  "AES Key Wrap w/ 128-bit key",                COSEAlgorithmFamily.KeyWrap, null,                     null,                      false, false, null, null, 16) },
            {   -4, new ("A192KW",  "AES Key Wrap w/ 192-bit key",                COSEAlgorithmFamily.KeyWrap, null,                     null,                      false, false, null, null, 24) },
            {   -5, new ("A256KW",  "AES Key Wrap w/ 256-bit key",                COSEAlgorithmFamily.KeyWrap, null,                     null,                      false, false, null, null, 32) },

            {   -6, new ("direct",  "Direct use of content encryption key (CEK)", COSEAlgorithmFamily.Direct,  null,                     null,                      false, false) },

            // Hash algorithms [RFC 9054]. They sign nothing; they name the
            // digest of a certificate thumbprint (x5t) and the like.
            {  -16, new ("SHA-256", "SHA-2 256-bit Hash",                            COSEAlgorithmFamily.None,  HashAlgorithmName.SHA256, null,                      false, false) },
            {  -43, new ("SHA-384", "SHA-2 384-bit Hash",                            COSEAlgorithmFamily.None,  HashAlgorithmName.SHA384, null,                      false, false) },
            {  -44, new ("SHA-512", "SHA-2 512-bit Hash",                            COSEAlgorithmFamily.None,  HashAlgorithmName.SHA512, null,                      false, false) },
            {  -14, new ("SHA-1",   "SHA-1 Hash",                                    COSEAlgorithmFamily.None,  null,                     null,                      false, true ) }

        };

        #endregion

        #region Properties

        /// <summary>
        /// The numeric identification of this algorithm.
        /// </summary>
        public Int32               Value

        { get; }

        /// <summary>
        /// Whether this algorithm is registered and known
        /// to this implementation.
        /// </summary>
        public Boolean             IsKnown

            => registry.ContainsKey(Value);

        /// <summary>
        /// The registered name of this algorithm, e.g. "ES256",
        /// or its numeric identification when unknown.
        /// </summary>
        public String              Name

            => registry.TryGetValue(Value, out var info)
                   ? info.Name
                   : Value.ToString();

        /// <summary>
        /// The registered description of this algorithm.
        /// </summary>
        public String?             Description

            => registry.TryGetValue(Value, out var info)
                   ? info.Description
                   : null;

        /// <summary>
        /// The message digest algorithm applied to the signature input
        /// before the elliptic curve operation.
        /// </summary>
        public HashAlgorithmName?  HashAlgorithm

            => registry.TryGetValue(Value, out var info)
                   ? info.HashAlgorithm
                   : null;

        /// <summary>
        /// The elliptic curve this algorithm is defined for, or null when
        /// the algorithm leaves the choice of the curve to the key, as the
        /// deprecated ES256, ES384 and ES512 do.
        /// </summary>
        public COSECurve?          FixedCurve

            => registry.TryGetValue(Value, out var info)
                   ? info.FixedCurve
                   : null;

        /// <summary>
        /// Which signature machinery this algorithm needs. The one that
        /// matters is ECDSA against the rest: ECDSA signs a digest of the
        /// message, EdDSA and ML-DSA sign the message.
        /// </summary>
        public COSEAlgorithmFamily Family

            => registry.TryGetValue(Value, out var info)
                   ? info.Family
                   : COSEAlgorithmFamily.None;

        /// <summary>
        /// The ML-DSA parameter set of this algorithm [FIPS 204],
        /// or null for every algorithm that is not ML-DSA.
        /// </summary>
        public MLDsaParameters?    MLDsaParameterSet

            => registry.TryGetValue(Value, out var info)
                   ? info.MLDsaParameterSet
                   : null;

        /// <summary>
        /// The width of the authentication tag in bytes, for a MAC algorithm,
        /// and null for everything else.
        ///
        /// It is part of the identifier rather than a parameter: HMAC 256/64
        /// and HMAC 256/256 are two registered algorithms over the same hash,
        /// and a verifier learns which width to expect from the message.
        /// </summary>
        public Int32?              TagSizeInBytes

            => registry.TryGetValue(Value, out var info)
                   ? info.TagSizeInBytes
                   : null;

        /// <summary>
        /// The width of the key in bytes, for an algorithm that fixes one, and
        /// null for everything else.
        ///
        /// The AES algorithms do: A128GCM and A256GCM are two registered
        /// identifiers over one cipher, and the width is what tells them
        /// apart. The signature algorithms leave it to the curve or to the
        /// parameter set.
        /// </summary>
        public Int32?              KeySizeInBytes

            => registry.TryGetValue(Value, out var info)
                   ? info.KeySizeInBytes
                   : null;

        /// <summary>
        /// Whether this implementation can create and verify
        /// signatures of this algorithm.
        /// </summary>
        public Boolean             IsSupportedForSigning

            => registry.TryGetValue(Value, out var info) &&
               info.IsSupportedForSigning;

        /// <summary>
        /// Whether this algorithm was deprecated by RFC 9864 in favour of a
        /// fully-specified one, which names the elliptic curve as well.
        /// Deprecated does not mean insecure: ES256 remains the most widely
        /// deployed COSE signature algorithm.
        /// </summary>
        public Boolean             IsDeprecated

            => registry.TryGetValue(Value, out var info) &&
               info.IsDeprecated;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new COSE algorithm identifier based on the given number.
        /// </summary>
        /// <param name="Value">The numeric identification of the algorithm.</param>
        public COSEAlgorithm(Int32 Value)
        {
            this.Value = Value;
        }

        #endregion


        #region Static defaults

        /// <summary>
        /// ECDSA with SHA-256 (algorithm -7), by far the most widely deployed
        /// COSE signature algorithm, in practice used with the curve P-256.
        /// Deprecated by RFC 9864 in favour of ESP256, because the algorithm
        /// itself does not name the elliptic curve.
        /// </summary>
        public static COSEAlgorithm  ES256      { get; } = new (  -7);

        /// <summary>
        /// ECDSA with SHA-384 (algorithm -35), in practice used with the
        /// curve P-384. Deprecated by RFC 9864 in favour of ESP384.
        /// </summary>
        public static COSEAlgorithm  ES384      { get; } = new ( -35);

        /// <summary>
        /// ECDSA with SHA-512 (algorithm -36), in practice used with the
        /// curve P-521. Deprecated by RFC 9864 in favour of ESP512.
        /// </summary>
        public static COSEAlgorithm  ES512      { get; } = new ( -36);

        /// <summary>
        /// ECDSA using the curve secp256k1 and SHA-256 (algorithm -47) [RFC 8812].
        /// </summary>
        public static COSEAlgorithm  ES256K     { get; } = new ( -47);

        /// <summary>
        /// ECDSA using the curve P-256 and SHA-256 (algorithm -9) [RFC 9864].
        /// </summary>
        public static COSEAlgorithm  ESP256     { get; } = new (  -9);

        /// <summary>
        /// ECDSA using the curve P-384 and SHA-384 (algorithm -51) [RFC 9864].
        /// </summary>
        public static COSEAlgorithm  ESP384     { get; } = new ( -51);

        /// <summary>
        /// ECDSA using the curve P-521 and SHA-512 (algorithm -52) [RFC 9864].
        /// </summary>
        public static COSEAlgorithm  ESP512     { get; } = new ( -52);

        /// <summary>
        /// ECDSA using the curve brainpoolP256r1 and SHA-256 (algorithm -265)
        /// [RFC 9864]. The brainpool curves are what German conformity
        /// assessment bodies and the metering formats of the charging
        /// infrastructure predominantly use.
        /// </summary>
        public static COSEAlgorithm  ESB256     { get; } = new (-265);

        /// <summary>
        /// ECDSA using the curve brainpoolP320r1 and SHA-384 (algorithm -266) [RFC 9864].
        /// </summary>
        public static COSEAlgorithm  ESB320     { get; } = new (-266);

        /// <summary>
        /// ECDSA using the curve brainpoolP384r1 and SHA-384 (algorithm -267) [RFC 9864].
        /// </summary>
        public static COSEAlgorithm  ESB384     { get; } = new (-267);

        /// <summary>
        /// ECDSA using the curve brainpoolP512r1 and SHA-512 (algorithm -268) [RFC 9864].
        /// </summary>
        public static COSEAlgorithm  ESB512     { get; } = new (-268);

        /// <summary>
        /// EdDSA (algorithm -8), without naming the parameter set, which is
        /// what RFC 9864 deprecated it for: the curve comes from the key.
        /// </summary>
        public static COSEAlgorithm  EdDSA      { get; } = new (  -8);

        /// <summary>
        /// EdDSA using the Ed25519 parameter set (algorithm -19) [RFC 9864].
        /// </summary>
        public static COSEAlgorithm  Ed25519    { get; } = new ( -19);

        /// <summary>
        /// EdDSA using the Ed448 parameter set (algorithm -53) [RFC 9864].
        /// </summary>
        public static COSEAlgorithm  Ed448      { get; } = new ( -53);

        /// <summary>
        /// ML-DSA-44 (algorithm -48) [FIPS 204, RFC 9964]: the post-quantum
        /// signature scheme, whose keys are algorithm key pairs rather than
        /// points on a curve. Its signature is 2420 bytes.
        /// </summary>
        public static COSEAlgorithm  MLDsa44    { get; } = new ( -48);

        /// <summary>
        /// ML-DSA-65 (algorithm -49) [FIPS 204, RFC 9964].
        /// Its signature is 3309 bytes.
        /// </summary>
        public static COSEAlgorithm  MLDsa65    { get; } = new ( -49);

        /// <summary>
        /// ML-DSA-87 (algorithm -50) [FIPS 204, RFC 9964].
        /// Its signature is 4627 bytes - which is the argument for carrying
        /// it in CBOR rather than as base64 within JSON.
        /// </summary>
        public static COSEAlgorithm  MLDsa87    { get; } = new ( -50);


        /// <summary>
        /// The COSE algorithm of the given ML-DSA parameter set, or null when
        /// that parameter set is not one COSE registers. The HashML-DSA
        /// variants are deliberately among those: RFC 9964 registers the pure
        /// ones only.
        /// </summary>
        /// <param name="ParameterSet">An ML-DSA parameter set.</param>
        public static COSEAlgorithm? TryGetFor(MLDsaParameters ParameterSet)
        {

            foreach (var entry in registry)
            {
                if (entry.Value.MLDsaParameterSet is not null &&
                    entry.Value.MLDsaParameterSet.Equals(ParameterSet))
                {
                    return new COSEAlgorithm(entry.Key);
                }
            }

            return null;

        }

        /// <summary>
        /// HMAC with SHA-256, truncated to 64 bits (algorithm 4)
        /// [RFC 9053, Section 3.1]. The smallest authentication tag COSE
        /// registers: eight bytes, where the smallest signature is sixty-four.
        /// </summary>
        public static COSEAlgorithm  HMAC256_64  { get; } = new (   4);

        /// <summary>
        /// HMAC with SHA-256 (algorithm 5) [RFC 9053, Section 3.1].
        /// </summary>
        public static COSEAlgorithm  HMAC256_256 { get; } = new (   5);

        /// <summary>
        /// HMAC with SHA-384 (algorithm 6) [RFC 9053, Section 3.1].
        /// </summary>
        public static COSEAlgorithm  HMAC384_384 { get; } = new (   6);

        /// <summary>
        /// HMAC with SHA-512 (algorithm 7) [RFC 9053, Section 3.1].
        /// </summary>
        public static COSEAlgorithm  HMAC512_512 { get; } = new (   7);


        /// <summary>
        /// AES-GCM with a 128-bit key and a 128-bit tag (algorithm 1)
        /// [RFC 9053, Section 4.1].
        /// </summary>
        public static COSEAlgorithm  A128GCM     { get; } = new (   1);

        /// <summary>
        /// AES-GCM with a 192-bit key (algorithm 2) [RFC 9053, Section 4.1].
        /// </summary>
        public static COSEAlgorithm  A192GCM     { get; } = new (   2);

        /// <summary>
        /// AES-GCM with a 256-bit key (algorithm 3) [RFC 9053, Section 4.1].
        /// </summary>
        public static COSEAlgorithm  A256GCM     { get; } = new (   3);

        /// <summary>
        /// AES key wrap with a 128-bit key-encryption key (algorithm -3)
        /// [RFC 9053, Section 6.2.1].
        /// </summary>
        public static COSEAlgorithm  A128KW      { get; } = new (  -3);

        /// <summary>
        /// AES key wrap with a 192-bit key-encryption key (algorithm -4).
        /// </summary>
        public static COSEAlgorithm  A192KW      { get; } = new (  -4);

        /// <summary>
        /// AES key wrap with a 256-bit key-encryption key (algorithm -5).
        /// </summary>
        public static COSEAlgorithm  A256KW      { get; } = new (  -5);

        /// <summary>
        /// Direct use of the content encryption key (algorithm -6)
        /// [RFC 9053, Section 6.1.1]: the recipient key IS the content key,
        /// and nothing is transported.
        /// </summary>
        public static COSEAlgorithm  Direct      { get; } = new (  -6);


        /// <summary>
        /// The SHA-2 256-bit hash (algorithm -16) [RFC 9054], the default
        /// digest of a certificate thumbprint.
        /// </summary>
        public static COSEAlgorithm  SHA256     { get; } = new ( -16);

        /// <summary>
        /// The SHA-2 384-bit hash (algorithm -43) [RFC 9054].
        /// </summary>
        public static COSEAlgorithm  SHA384     { get; } = new ( -43);

        /// <summary>
        /// The SHA-2 512-bit hash (algorithm -44) [RFC 9054].
        /// </summary>
        public static COSEAlgorithm  SHA512     { get; } = new ( -44);

        /// <summary>
        /// The SHA-1 hash (algorithm -14) [RFC 9054].
        /// Known so that it can be recognized and refused, not used.
        /// </summary>
        public static COSEAlgorithm  SHA1       { get; } = new ( -14);

        /// <summary>
        /// An enumeration of all algorithms known to this implementation.
        /// </summary>
        public static IEnumerable<COSEAlgorithm> All

            => registry.Keys.Select(static value => new COSEAlgorithm(value));

        #endregion


        #region (static) Parse   (Name)

        /// <summary>
        /// Parse the given text as a COSE algorithm name.
        /// </summary>
        /// <param name="Name">The registered name of an algorithm.</param>
        public static COSEAlgorithm Parse(String Name)
        {

            if (TryParse(Name, out var algorithm))
                return algorithm;

            throw new COSEException($"The given text '{Name}' is not a known COSE algorithm name!");

        }

        #endregion

        #region (static) TryParse(Name,   out Algorithm)

        /// <summary>
        /// Try to parse the given text as a COSE algorithm name.
        /// Names are compared case-sensitively, as "ES256" and "es256"
        /// are not the same registered name.
        /// </summary>
        /// <param name="Name">The registered name of an algorithm.</param>
        /// <param name="Algorithm">The parsed algorithm.</param>
        public static Boolean TryParse(String Name, out COSEAlgorithm Algorithm)
        {

            foreach (var entry in registry)
            {
                if (String.Equals(entry.Value.Name, Name, StringComparison.Ordinal))
                {
                    Algorithm = new COSEAlgorithm(entry.Key);
                    return true;
                }
            }

            Algorithm = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out Algorithm)

        /// <summary>
        /// Try to parse the given number as a COSE algorithm identifier
        /// known to this implementation.
        /// </summary>
        /// <param name="Number">The numeric identification of an algorithm.</param>
        /// <param name="Algorithm">The parsed algorithm.</param>
        public static Boolean TryParse(Int32 Number, out COSEAlgorithm Algorithm)
        {

            if (registry.ContainsKey(Number))
            {
                Algorithm = new COSEAlgorithm(Number);
                return true;
            }

            Algorithm = default;
            return false;

        }

        #endregion

        #region (static) TryParse(CBOR,   out Algorithm, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR value as a COSE algorithm identifier.
        /// Unknown identifiers are accepted here: Whether an algorithm can
        /// actually be used is decided by IsSupportedForSigning, in order to
        /// keep unknown algorithms inspectable instead of unparsable.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of an algorithm identifier.</param>
        /// <param name="Algorithm">The parsed algorithm.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                        CBOR,
                                       out COSEAlgorithm                Algorithm,
                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Algorithm = default;

            if (!CBOR.TryGetInt64(out var value) ||
                value < Int32.MinValue ||
                value > Int32.MaxValue)
            {
                ErrorResponse = "The COSE algorithm identifier must be an integer!";
                return false;
            }

            Algorithm      = new COSEAlgorithm((Int32) value);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region ToCBOR()

        /// <summary>
        /// Return a CBOR representation of this algorithm identifier.
        /// </summary>
        public CBORValue ToCBOR()

            => CBORValue.FromInt64(Value);

        #endregion

        #region Hash(Data)

        /// <summary>
        /// Compute the message digest of the given data as defined
        /// by this algorithm.
        /// </summary>
        /// <param name="Data">The data to hash, i.e. the encoded Sig_structure.</param>
        public Byte[] Hash(ReadOnlySpan<Byte> Data)
        {

            var hashAlgorithm = HashAlgorithm
                                    ?? throw new COSEException($"The COSE algorithm '{Name}' does not define a separate message digest!");

            // Fully qualified: the static algorithm identifiers SHA256, SHA384
            // and SHA512 of this very class would otherwise shadow the types.
            if (hashAlgorithm == HashAlgorithmName.SHA256)
                return System.Security.Cryptography.SHA256.HashData(Data);

            if (hashAlgorithm == HashAlgorithmName.SHA384)
                return System.Security.Cryptography.SHA384.HashData(Data);

            if (hashAlgorithm == HashAlgorithmName.SHA512)
                return System.Security.Cryptography.SHA512.HashData(Data);

            throw new COSEException($"The message digest '{hashAlgorithm.Name}' of the COSE algorithm '{Name}' is not implemented!");

        }

        #endregion

        #region ComputeMAC(ToBeMaced, Key)

        /// <summary>
        /// Compute the authentication tag of the given data with the given
        /// shared key, as defined by this HMAC algorithm.
        ///
        /// This is deliberately NOT reachable through Sign(...). A MAC is not
        /// a signature with a shorter key: it is symmetric, so whoever can
        /// verify one can produce one, and an API letting one stand in for the
        /// other would let a caller believe a message was signed when it was
        /// merely authenticated between two parties sharing a secret.
        ///
        /// The truncation of HMAC 256/64 applies to the OUTPUT and never to
        /// the key: RFC 9053 keeps "the leftmost tag-length bits" of the full
        /// HMAC.
        /// </summary>
        /// <param name="ToBeMaced">The data to authenticate, i.e. the encoded MAC_structure.</param>
        /// <param name="Key">The shared secret.</param>
        public Byte[] ComputeMAC(ReadOnlySpan<Byte>  ToBeMaced,
                                 Byte[]              Key)
        {

            if (Family != COSEAlgorithmFamily.HMAC)
                throw new COSEException($"The COSE algorithm '{Name}' is not a message authentication algorithm supported by this implementation!");

            var hashAlgorithm  = HashAlgorithm
                                     ?? throw new COSEException($"The COSE algorithm '{Name}' does not define a message digest!");

            var tagSize        = TagSizeInBytes
                                     ?? throw new COSEException($"The COSE algorithm '{Name}' does not define an authentication tag width!");

            // The key is passed through as it is. RFC 9053 says an HMAC key
            // SHOULD be as wide as the hash output, which is advice about key
            // management rather than a rule about the primitive - RFC 2104
            // accepts any width, and the published vectors of RFC 4231 include
            // a four-byte key.
            Byte[] full;

            if      (hashAlgorithm == HashAlgorithmName.SHA256)  full = HMACSHA256.HashData(Key, ToBeMaced);
            else if (hashAlgorithm == HashAlgorithmName.SHA384)  full = HMACSHA384.HashData(Key, ToBeMaced);
            else if (hashAlgorithm == HashAlgorithmName.SHA512)  full = HMACSHA512.HashData(Key, ToBeMaced);
            else
                throw new COSEException($"The message digest '{hashAlgorithm.Name}' of the COSE algorithm '{Name}' is not implemented!");

            if (tagSize > full.Length)
                throw new COSEException($"An HMAC over {hashAlgorithm.Name} is {full.Length} bytes long and can not be truncated to {tagSize}!");

            return tagSize == full.Length
                       ? full
                       : full[..tagSize];

        }

        #endregion

        #region VerifyMAC(ToBeMaced, Tag, Key)

        /// <summary>
        /// Whether the given authentication tag is the right one for the given
        /// data and shared key.
        ///
        /// The comparison is CONSTANT TIME, which matters here in a way it
        /// does not for a signature: a compare returning early would tell
        /// whoever is guessing a tag how many of their leading bytes were
        /// right, turning the forgery of a 32-byte tag from 2^256 work into
        /// 32 x 256. Everything a signature verification compares is public,
        /// so it has no equivalent exposure.
        ///
        /// The length check in front leaks nothing: the width of a tag follows
        /// from the algorithm, which travels in the message.
        /// </summary>
        /// <param name="ToBeMaced">The authenticated data, i.e. the encoded MAC_structure.</param>
        /// <param name="Tag">The authentication tag that arrived.</param>
        /// <param name="Key">The shared secret.</param>
        public Boolean VerifyMAC(ReadOnlySpan<Byte>  ToBeMaced,
                                 Byte[]              Tag,
                                 Byte[]              Key)
        {

            var computed = ComputeMAC(ToBeMaced, Key);

            return computed.Length == Tag.Length &&
                   CryptographicOperations.FixedTimeEquals(computed, Tag);

        }

        #endregion

        #region Encrypt(Plaintext, Key, Nonce, AdditionalData)

        /// <summary>
        /// Encrypt with AES-GCM, returning ciphertext with the 16-byte
        /// authentication tag APPENDED.
        ///
        /// That the tag is appended rather than carried in a field of its own
        /// is how COSE transports it, and an implementation that kept them
        /// apart would interoperate with nothing.
        ///
        /// The nonce is the caller's and there is no default, deliberately: a
        /// nonce reused with the same key breaks AES-GCM outright - two
        /// messages under one nonce leak the XOR of their plaintexts AND the
        /// authentication subkey, which lets an adversary forge afterwards -
        /// and this library cannot know which nonces a caller has spent.
        /// </summary>
        /// <param name="Plaintext">The content to encrypt.</param>
        /// <param name="Key">The content encryption key.</param>
        /// <param name="Nonce">The 12-byte nonce, which must never repeat under one key.</param>
        /// <param name="AdditionalData">The encoded Enc_structure, authenticated but not encrypted.</param>
        public Byte[] Encrypt(ReadOnlySpan<Byte>  Plaintext,
                              Byte[]              Key,
                              Byte[]              Nonce,
                              ReadOnlySpan<Byte>  AdditionalData)
        {

            EnsureAESGCM(Key, Nonce);

            var ciphertext = new Byte[Plaintext.Length + AESGCMTagSize];

            using var aes = new AesGcm(Key, AESGCMTagSize);

            aes.Encrypt(Nonce,
                        Plaintext,
                        ciphertext.AsSpan(0, Plaintext.Length),
                        ciphertext.AsSpan(Plaintext.Length),
                        AdditionalData);

            return ciphertext;

        }

        #endregion

        #region Decrypt(Ciphertext, Key, Nonce, AdditionalData)

        /// <summary>
        /// Decrypt with AES-GCM, or return null when the message does not
        /// authenticate.
        ///
        /// Null rather than an exception, and null rather than a partial
        /// plaintext: an AEAD failure means the WHOLE message is
        /// unauthenticated, and handing back what was decrypted before the tag
        /// was checked is the classic way to build an oracle out of a
        /// decryptor.
        /// </summary>
        /// <param name="Ciphertext">The ciphertext with the authentication tag appended.</param>
        /// <param name="Key">The content encryption key.</param>
        /// <param name="Nonce">The 12-byte nonce.</param>
        /// <param name="AdditionalData">The encoded Enc_structure.</param>
        public Byte[]? Decrypt(Byte[]              Ciphertext,
                               Byte[]              Key,
                               Byte[]              Nonce,
                               ReadOnlySpan<Byte>  AdditionalData)
        {

            EnsureAESGCM(Key, Nonce);

            if (Ciphertext.Length < AESGCMTagSize)
                return null;

            var plaintext = new Byte[Ciphertext.Length - AESGCMTagSize];

            try
            {

                using var aes = new AesGcm(Key, AESGCMTagSize);

                aes.Decrypt(Nonce,
                            Ciphertext.AsSpan(0, plaintext.Length),
                            Ciphertext.AsSpan(plaintext.Length),
                            plaintext,
                            AdditionalData);

                return plaintext;

            }
            catch (CryptographicException)
            {
                return null;
            }

        }

        #endregion

        #region (private) EnsureAESGCM(Key, Nonce)

        private void EnsureAESGCM(Byte[] Key, Byte[] Nonce)
        {

            if (Family != COSEAlgorithmFamily.AESGCM)
                throw new COSEException($"The COSE algorithm '{Name}' is not a content encryption algorithm supported by this implementation!");

            var keySize = KeySizeInBytes
                              ?? throw new COSEException($"The COSE algorithm '{Name}' does not define a key width!");

            if (Key.Length != keySize)
                throw new COSEException($"The COSE algorithm '{Name}' needs a {keySize}-byte key, but a {Key.Length}-byte key was given!");

            if (Nonce.Length != AESGCMNonceSize)
                throw new COSEException($"AES-GCM within COSE uses a {AESGCMNonceSize}-byte nonce [RFC 9053, Section 4.1], but a {Nonce.Length}-byte one was given!");

        }

        #endregion

        #region WrapKey(ContentKey, KeyEncryptionKey) / UnwrapKey(Wrapped, KeyEncryptionKey)

        /// <summary>
        /// Wrap a content key under a key-encryption key [RFC 3394].
        ///
        /// The result is eight bytes longer than the key: RFC 3394's integrity
        /// check value travels with it, which is what lets an unwrap FAIL
        /// rather than silently return rubbish.
        /// </summary>
        /// <param name="ContentKey">The key to wrap.</param>
        /// <param name="KeyEncryptionKey">The key to wrap it under.</param>
        public Byte[] WrapKey(Byte[] ContentKey, Byte[] KeyEncryptionKey)
        {

            EnsureKeyWrap(KeyEncryptionKey);

            if (ContentKey.Length % 8 != 0 || ContentKey.Length < 16)
                throw new COSEException($"AES key wrap needs a key of at least 16 bytes and a multiple of 8 [RFC 3394], but a {ContentKey.Length}-byte key was given!");

            var engine = new AesWrapEngine();
            engine.Init(true, new KeyParameter(KeyEncryptionKey));

            return engine.Wrap(ContentKey, 0, ContentKey.Length);

        }

        /// <summary>
        /// Unwrap a content key, or return null when the integrity check
        /// fails.
        ///
        /// Failing is the useful behaviour: the check value is what tells a
        /// recipient that this wrapped key was not meant for them, rather than
        /// handing them a plausible-looking key that decrypts nothing.
        /// </summary>
        /// <param name="Wrapped">The wrapped key.</param>
        /// <param name="KeyEncryptionKey">The key it was wrapped under.</param>
        public Byte[]? UnwrapKey(Byte[] Wrapped, Byte[] KeyEncryptionKey)
        {

            EnsureKeyWrap(KeyEncryptionKey);

            if (Wrapped.Length % 8 != 0 || Wrapped.Length < 24)
                return null;

            try
            {

                var engine = new AesWrapEngine();
                engine.Init(false, new KeyParameter(KeyEncryptionKey));

                return engine.Unwrap(Wrapped, 0, Wrapped.Length);

            }
            catch (Exception)
            {
                return null;
            }

        }

        private void EnsureKeyWrap(Byte[] KeyEncryptionKey)
        {

            if (Family != COSEAlgorithmFamily.KeyWrap)
                throw new COSEException($"The COSE algorithm '{Name}' is not a key wrap algorithm supported by this implementation!");

            var keySize = KeySizeInBytes
                              ?? throw new COSEException($"The COSE algorithm '{Name}' does not define a key width!");

            if (KeyEncryptionKey.Length != keySize)
                throw new COSEException($"The COSE algorithm '{Name}' needs a {keySize}-byte key-encryption key, but a {KeyEncryptionKey.Length}-byte key was given!");

        }

        #endregion

        #region (static) ForKeyWrap(KeySizeInBytes)

        /// <summary>
        /// The key wrap algorithm a key-encryption key of the given width
        /// belongs to.
        ///
        /// The width is that of the KEY-ENCRYPTION key rather than of the key
        /// being wrapped, which is the direction people get wrong: A256KW
        /// wraps a 128-bit content key perfectly well.
        /// </summary>
        /// <param name="KeySizeInBytes">The width of the key-encryption key.</param>
        public static COSEAlgorithm ForKeyWrap(Int32 KeySizeInBytes)

            => KeySizeInBytes switch {
                   16 => A128KW,
                   24 => A192KW,
                   32 => A256KW,
                   _  => throw new COSEException($"AES key wrap needs a key-encryption key of 16, 24 or 32 bytes, but a {KeySizeInBytes}-byte key was given!")
               };

        #endregion

        #region EnsureUsableWith(DomainParameters)

        /// <summary>
        /// Verify that this implementation can sign with this algorithm and
        /// that the key lies on the curve a fully-specified algorithm
        /// prescribes.
        /// </summary>
        /// <param name="DomainParameters">The elliptic curve domain parameters of the key to be used.</param>
        public void EnsureUsableWith(ECDomainParameters DomainParameters)
        {

            if (!IsSupportedForSigning)
                throw new COSEException($"The COSE algorithm '{Name}' is not supported for signing by this implementation!");

            if (FixedCurve.HasValue)
            {

                if (!COSECurve.TryGetFor(DomainParameters, out var curve))
                    throw new COSEException($"The COSE algorithm '{Name}' is defined for the elliptic curve '{FixedCurve.Value.Name}', but the elliptic curve of the given key is not registered at all!");

                if (curve != FixedCurve.Value)
                    throw new COSEException($"The COSE algorithm '{Name}' is defined for the elliptic curve '{FixedCurve.Value.Name}', but the given key is on the curve '{curve.Name}'!");

            }

        }

        #endregion

        #region CreateDigest()

        /// <summary>
        /// Create the message digest of this algorithm as a Bouncy Castle
        /// digest, for the HMAC construction of the deterministic nonce.
        /// </summary>
        private IDigest CreateDigest()
        {

            var hashAlgorithm = HashAlgorithm
                                    ?? throw new COSEException($"The COSE algorithm '{Name}' does not define a separate message digest!");

            if (hashAlgorithm == HashAlgorithmName.SHA256)
                return new Sha256Digest();

            if (hashAlgorithm == HashAlgorithmName.SHA384)
                return new Sha384Digest();

            if (hashAlgorithm == HashAlgorithmName.SHA512)
                return new Sha512Digest();

            throw new COSEException($"The message digest '{hashAlgorithm.Name}' of the COSE algorithm '{Name}' is not implemented!");

        }

        #endregion

        #region Sign  (ToBeSigned, PrivateKey, Random = null, Deterministic = false)

        /// <summary>
        /// Sign the given signature input with the given private key.
        /// The result is the concatenation of the two ECDSA components r and
        /// s, each zero-padded to the width of the group order of the
        /// elliptic curve [RFC 9053, Section 2.1] - NOT the DER encoding that
        /// the Bouncy Castle and .NET signer utilities produce by default.
        ///
        /// An ECDSA signature is ordinarily randomized, so signing the same
        /// data twice yields two different signatures, both valid. Setting
        /// Deterministic derives the nonce from the private key and the
        /// message instead [RFC 6979], which makes the signature a pure
        /// function of what is signed: the same input yields the same bytes,
        /// every time and on every implementation.
        ///
        /// That is worth having twice over. A device that has no dependable
        /// source of randomness - a meter, a smart card - can not be made
        /// unsafe by a poor one, since a repeated nonce reveals the private
        /// key. And a published example becomes recomputable rather than
        /// merely verifiable.
        /// </summary>
        /// <param name="ToBeSigned">The signature input, i.e. an encoded Sig_structure.</param>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        /// <param name="Deterministic">Whether to derive the nonce from the key and the message as defined by RFC 6979, instead of drawing it at random.</param>
        public Byte[] Sign(ReadOnlySpan<Byte>      ToBeSigned,
                           AsymmetricKeyParameter  PrivateKey,
                           SecureRandom?           Random          = null,
                           Boolean                 Deterministic   = false)
        {

            if (Deterministic && Random is not null)
                throw new COSEException("A deterministic signature derives its nonce from the private key and the message, so no source of randomness must be supplied!");

            return Family switch {
                COSEAlgorithmFamily.ECDSA  => SignECDSA(ToBeSigned, PrivateKey, Random, Deterministic),
                COSEAlgorithmFamily.EdDSA  => SignPure (ToBeSigned, PrivateKey, CreateEdDSASigner(PrivateKey, true)),
                COSEAlgorithmFamily.MLDSA  => SignPure (ToBeSigned, PrivateKey, CreateMLDsaSigner(Deterministic), Random),
                _                          => throw new COSEException($"The COSE algorithm '{Name}' is not supported for signing by this implementation!")
            };

        }

        #endregion

        #region (private) SignECDSA(ToBeSigned, PrivateKey, Random, Deterministic)

        /// <summary>
        /// ECDSA, which signs the DIGEST of the signature input and produces
        /// the concatenation of r and s.
        /// </summary>
        private Byte[] SignECDSA(ReadOnlySpan<Byte>      ToBeSigned,
                                 AsymmetricKeyParameter  PrivateKey,
                                 SecureRandom?           Random,
                                 Boolean                 Deterministic)
        {

            if (PrivateKey is not ECPrivateKeyParameters ecPrivateKey)
                throw new COSEException($"The COSE algorithm '{Name}' needs an elliptic curve private key, but was given a {PrivateKey.GetType().Name}!");

            EnsureUsableWith(ecPrivateKey.Parameters);

            var componentSizeInBytes  = (ecPrivateKey.Parameters.N.BitLength + 7) / 8;

            var signer                = Deterministic
                                            ? new ECDsaSigner(new HMacDsaKCalculator(CreateDigest()))
                                            : new ECDsaSigner();

            if (Deterministic)
                signer.Init(true, ecPrivateKey);
            else
                signer.Init(true, new ParametersWithRandom(ecPrivateKey, Random ?? new SecureRandom()));

            var components            = signer.GenerateSignature(Hash(ToBeSigned));

            var signature             = new Byte[2 * componentSizeInBytes];
            BigIntegers.AsUnsignedByteArray(components[0], signature, 0,                    componentSizeInBytes);
            BigIntegers.AsUnsignedByteArray(components[1], signature, componentSizeInBytes, componentSizeInBytes);

            return signature;

        }

        #endregion

        #region (private) SignPure(ToBeSigned, PrivateKey, Signer, Random = null)

        /// <summary>
        /// EdDSA and ML-DSA, which sign the signature input ITSELF. There is
        /// no digest step: handing either of them a hash would produce a
        /// signature that is valid for the hash and that nobody else accepts.
        /// </summary>
        private Byte[] SignPure(ReadOnlySpan<Byte>      ToBeSigned,
                                AsymmetricKeyParameter  PrivateKey,
                                ISigner                 Signer,
                                SecureRandom?           Random   = null)
        {

            Signer.Init(true,
                        Random is not null
                            ? new ParametersWithRandom(PrivateKey, Random)
                            : PrivateKey);

            Signer.BlockUpdate(ToBeSigned);

            return Signer.GenerateSignature();

        }

        #endregion

        #region (private) CreateEdDSASigner(Key, ForSigning) / CreateMLDsaSigner(Deterministic)

        /// <summary>
        /// The EdDSA signer belonging to a key, in the pure variant of
        /// RFC 8032 with an empty context - which is what COSE uses
        /// [RFC 9053, Section 2.2].
        /// </summary>
        private ISigner CreateEdDSASigner(AsymmetricKeyParameter Key, Boolean ForSigning)
        {

            var isEd448 = Key is Ed448PrivateKeyParameters or Ed448PublicKeyParameters;

            if (!isEd448 && Key is not (Ed25519PrivateKeyParameters or Ed25519PublicKeyParameters))
                throw new COSEException($"The COSE algorithm '{Name}' needs an Ed25519 or Ed448 key, but was given a {Key.GetType().Name}!");

            if (FixedCurve.HasValue &&
                FixedCurve.Value != (isEd448 ? COSECurve.Ed448 : COSECurve.Ed25519))
            {
                throw new COSEException($"The COSE algorithm '{Name}' is defined for the curve '{FixedCurve.Value.Name}', which is not the curve of the given key!");
            }

            return isEd448
                       ? new Ed448Signer([])
                       : new Ed25519Signer();

        }

        /// <summary>
        /// The ML-DSA signer of this algorithm's parameter set.
        /// FIPS 204 defines a deterministic variant, in which the
        /// per-signature randomness is 32 zero bytes rather than drawn, and
        /// RFC 9964 leaves the choice open. It is the choice that decides
        /// whether two implementations can be compared byte for byte.
        /// </summary>
        private MLDsaSigner CreateMLDsaSigner(Boolean Deterministic)

            => new (MLDsaParameterSet
                        ?? throw new COSEException($"The COSE algorithm '{Name}' does not name an ML-DSA parameter set!"),
                    Deterministic);

        #endregion

        #region Verify(ToBeSigned, Signature, PublicKey, out ErrorResponse)

        /// <summary>
        /// Verify the given signature over the given signature input.
        /// A failed verification is not an exception: It is the expected
        /// outcome of checking untrusted data.
        /// </summary>
        /// <param name="ToBeSigned">The signature input, i.e. an encoded Sig_structure.</param>
        /// <param name="Signature">The signature, as the concatenation of r and s.</param>
        /// <param name="PublicKey">An elliptic curve public key.</param>
        /// <param name="ErrorResponse">The reason why the verification failed.</param>
        public Boolean Verify(ReadOnlySpan<Byte>                ToBeSigned,
                              Byte[]                            Signature,
                              AsymmetricKeyParameter            PublicKey,
                              [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (!IsSupportedForSigning)
            {
                ErrorResponse = $"The COSE algorithm '{Name}' is not supported for verification by this implementation!";
                return false;
            }

            try
            {

                switch (Family)
                {

                    case COSEAlgorithmFamily.ECDSA:
                        return VerifyECDSA(ToBeSigned, Signature, PublicKey, out ErrorResponse);

                    case COSEAlgorithmFamily.EdDSA:
                        return VerifyPure(ToBeSigned, Signature, PublicKey,
                                          CreateEdDSASigner(PublicKey, false), out ErrorResponse);

                    case COSEAlgorithmFamily.MLDSA:
                        return VerifyPure(ToBeSigned, Signature, PublicKey,
                                          CreateMLDsaSigner(false), out ErrorResponse);

                    default:
                        ErrorResponse = $"The COSE algorithm '{Name}' is not a signature algorithm!";
                        return false;

                }

            }
            catch (Exception e)
            {
                // Untrusted input is allowed to be nonsense - a key that is not
                // a key, a signature of the wrong shape. That is a failed
                // verification and not an exception for the caller to handle.
                ErrorResponse = e.Message;
                return false;
            }

        }

        #endregion

        #region (private) VerifyECDSA(ToBeSigned, Signature, PublicKey, out ErrorResponse)

        private Boolean VerifyECDSA(ReadOnlySpan<Byte>                ToBeSigned,
                                    Byte[]                            Signature,
                                    AsymmetricKeyParameter            PublicKey,
                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (PublicKey is not ECPublicKeyParameters ecPublicKey)
            {
                ErrorResponse = $"The COSE algorithm '{Name}' needs an elliptic curve public key, but was given a {PublicKey.GetType().Name}!";
                return false;
            }

            if (FixedCurve.HasValue)
            {

                if (!COSECurve.TryGetFor(ecPublicKey.Parameters, out var curve) ||
                    curve != FixedCurve.Value)
                {
                    ErrorResponse = $"The COSE algorithm '{Name}' is defined for the elliptic curve '{FixedCurve.Value.Name}', which is not the curve of the given key!";
                    return false;
                }

            }

            var componentSizeInBytes = (ecPublicKey.Parameters.N.BitLength + 7) / 8;

            if (Signature.Length != 2 * componentSizeInBytes)
            {
                ErrorResponse = $"An ECDSA signature on the given elliptic curve must be {2 * componentSizeInBytes} bytes long, but was {Signature.Length} bytes long! (A DER encoded signature is a common cause: COSE concatenates r and s.)";
                return false;
            }

            var verifier = new ECDsaSigner();
            verifier.Init(false, ecPublicKey);

            if (!verifier.VerifySignature(Hash(ToBeSigned),
                                          new BigInteger(1, Signature, 0,                    componentSizeInBytes),
                                          new BigInteger(1, Signature, componentSizeInBytes, componentSizeInBytes)))
            {
                ErrorResponse = "The signature is invalid!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region (private) VerifyPure(ToBeSigned, Signature, PublicKey, Verifier, out ErrorResponse)

        /// <summary>
        /// EdDSA and ML-DSA verify the signature input itself, so there is no
        /// digest here either - and no component width to check, because
        /// neither signature is a pair of numbers.
        /// </summary>
        private static Boolean VerifyPure(ReadOnlySpan<Byte>                ToBeSigned,
                                          Byte[]                            Signature,
                                          AsymmetricKeyParameter            PublicKey,
                                          ISigner                           Verifier,
                                          [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Verifier.Init(false, PublicKey);
            Verifier.BlockUpdate(ToBeSigned);

            if (!Verifier.VerifySignature(Signature))
            {
                ErrorResponse = "The signature is invalid!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion


        #region Operator overloading

        #region Operator == (COSEAlgorithm1, COSEAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSEAlgorithm1">An algorithm.</param>
        /// <param name="COSEAlgorithm2">Another algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (COSEAlgorithm COSEAlgorithm1,
                                           COSEAlgorithm COSEAlgorithm2)

            => COSEAlgorithm1.Equals(COSEAlgorithm2);

        #endregion

        #region Operator != (COSEAlgorithm1, COSEAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSEAlgorithm1">An algorithm.</param>
        /// <param name="COSEAlgorithm2">Another algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (COSEAlgorithm COSEAlgorithm1,
                                           COSEAlgorithm COSEAlgorithm2)

            => !COSEAlgorithm1.Equals(COSEAlgorithm2);

        #endregion

        #region Operator <  (COSEAlgorithm1, COSEAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSEAlgorithm1">An algorithm.</param>
        /// <param name="COSEAlgorithm2">Another algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (COSEAlgorithm COSEAlgorithm1,
                                          COSEAlgorithm COSEAlgorithm2)

            => COSEAlgorithm1.CompareTo(COSEAlgorithm2) < 0;

        #endregion

        #region Operator <= (COSEAlgorithm1, COSEAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSEAlgorithm1">An algorithm.</param>
        /// <param name="COSEAlgorithm2">Another algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (COSEAlgorithm COSEAlgorithm1,
                                           COSEAlgorithm COSEAlgorithm2)

            => COSEAlgorithm1.CompareTo(COSEAlgorithm2) <= 0;

        #endregion

        #region Operator >  (COSEAlgorithm1, COSEAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSEAlgorithm1">An algorithm.</param>
        /// <param name="COSEAlgorithm2">Another algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (COSEAlgorithm COSEAlgorithm1,
                                          COSEAlgorithm COSEAlgorithm2)

            => COSEAlgorithm1.CompareTo(COSEAlgorithm2) > 0;

        #endregion

        #region Operator >= (COSEAlgorithm1, COSEAlgorithm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSEAlgorithm1">An algorithm.</param>
        /// <param name="COSEAlgorithm2">Another algorithm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (COSEAlgorithm COSEAlgorithm1,
                                           COSEAlgorithm COSEAlgorithm2)

            => COSEAlgorithm1.CompareTo(COSEAlgorithm2) >= 0;

        #endregion

        #endregion

        #region IComparable<COSEAlgorithm> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two algorithms.
        /// </summary>
        /// <param name="Object">An algorithm to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is COSEAlgorithm cOSEAlgorithm
                   ? CompareTo(cOSEAlgorithm)
                   : throw new ArgumentException("The given object is not a COSE algorithm!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(COSEAlgorithm)

        /// <summary>
        /// Compares two algorithms.
        /// </summary>
        /// <param name="COSEAlgorithm">An algorithm to compare with.</param>
        public Int32 CompareTo(COSEAlgorithm COSEAlgorithm)

            => Value.CompareTo(COSEAlgorithm.Value);

        #endregion

        #endregion

        #region IEquatable<COSEAlgorithm> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two algorithms for equality.
        /// </summary>
        /// <param name="Object">An algorithm to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is COSEAlgorithm cOSEAlgorithm &&
                   Equals(cOSEAlgorithm);

        #endregion

        #region Equals(COSEAlgorithm)

        /// <summary>
        /// Compares two algorithms for equality.
        /// </summary>
        /// <param name="COSEAlgorithm">An algorithm to compare with.</param>
        public Boolean Equals(COSEAlgorithm COSEAlgorithm)

            => Value == COSEAlgorithm.Value;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Name;

        #endregion

    }

}
