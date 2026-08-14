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
        private sealed class AlgorithmInfo(String              Name,
                                           String              Description,
                                           HashAlgorithmName?  HashAlgorithm,
                                           COSECurve?          FixedCurve,
                                           Boolean             IsSupportedForSigning,
                                           Boolean             IsDeprecated)
        {

            public String              Name                     { get; } = Name;
            public String              Description              { get; } = Description;
            public HashAlgorithmName?  HashAlgorithm            { get; } = HashAlgorithm;
            public COSECurve?          FixedCurve               { get; } = FixedCurve;
            public Boolean             IsSupportedForSigning    { get; } = IsSupportedForSigning;
            public Boolean             IsDeprecated             { get; } = IsDeprecated;

        }

        #endregion

        #region Data

        /// <summary>
        /// The registered algorithms this implementation knows about.
        /// The ECDSA algorithms ES256, ES384 and ES512 leave the choice of the
        /// elliptic curve to the key, which is why RFC 9864 deprecated them in
        /// favour of the fully-specified ESP* and ESB* algorithms. They are
        /// nevertheless the ones deployed everywhere today.
        /// </summary>
        private static readonly Dictionary<Int32, AlgorithmInfo> registry = new () {

            {   -7, new ("ES256",   "ECDSA w/ SHA-256",                              HashAlgorithmName.SHA256, null,                         true,  true ) },
            {  -35, new ("ES384",   "ECDSA w/ SHA-384",                              HashAlgorithmName.SHA384, null,                         true,  true ) },
            {  -36, new ("ES512",   "ECDSA w/ SHA-512",                              HashAlgorithmName.SHA512, null,                         true,  true ) },
            {  -47, new ("ES256K",  "ECDSA using secp256k1 curve and SHA-256",       HashAlgorithmName.SHA256, COSECurve.Secp256k1,          true,  false) },

            {   -9, new ("ESP256",  "ECDSA using P-256 curve and SHA-256",           HashAlgorithmName.SHA256, COSECurve.P256,               true,  false) },
            {  -51, new ("ESP384",  "ECDSA using P-384 curve and SHA-384",           HashAlgorithmName.SHA384, COSECurve.P384,               true,  false) },
            {  -52, new ("ESP512",  "ECDSA using P-521 curve and SHA-512",           HashAlgorithmName.SHA512, COSECurve.P521,               true,  false) },

            { -265, new ("ESB256",  "ECDSA using BrainpoolP256r1 curve and SHA-256", HashAlgorithmName.SHA256, COSECurve.BrainpoolP256r1,    true,  false) },
            { -266, new ("ESB320",  "ECDSA using BrainpoolP320r1 curve and SHA-384", HashAlgorithmName.SHA384, COSECurve.BrainpoolP320r1,    true,  false) },
            { -267, new ("ESB384",  "ECDSA using BrainpoolP384r1 curve and SHA-384", HashAlgorithmName.SHA384, COSECurve.BrainpoolP384r1,    true,  false) },
            { -268, new ("ESB512",  "ECDSA using BrainpoolP512r1 curve and SHA-512", HashAlgorithmName.SHA512, COSECurve.BrainpoolP512r1,    true,  false) },

            {   -8, new ("EdDSA",   "EdDSA",                                         null,                     null,                         false, true ) },
            {  -19, new ("Ed25519", "EdDSA using the Ed25519 parameter set",         null,                     COSECurve.Ed25519,            false, false) },
            {  -53, new ("Ed448",   "EdDSA using the Ed448 parameter set",           null,                     COSECurve.Ed448,              false, false) }

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
        /// EdDSA (algorithm -8), without naming the parameter set.
        /// Known, but not implemented here.
        /// </summary>
        public static COSEAlgorithm  EdDSA      { get; } = new (  -8);

        /// <summary>
        /// EdDSA using the Ed25519 parameter set (algorithm -19) [RFC 9864].
        /// Known, but not implemented here.
        /// </summary>
        public static COSEAlgorithm  Ed25519    { get; } = new ( -19);

        /// <summary>
        /// EdDSA using the Ed448 parameter set (algorithm -53) [RFC 9864].
        /// Known, but not implemented here.
        /// </summary>
        public static COSEAlgorithm  Ed448      { get; } = new ( -53);

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

            if (hashAlgorithm == HashAlgorithmName.SHA256)
                return SHA256.HashData(Data);

            if (hashAlgorithm == HashAlgorithmName.SHA384)
                return SHA384.HashData(Data);

            if (hashAlgorithm == HashAlgorithmName.SHA512)
                return SHA512.HashData(Data);

            throw new COSEException($"The message digest '{hashAlgorithm.Name}' of the COSE algorithm '{Name}' is not implemented!");

        }

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

        #region Sign  (ToBeSigned, PrivateKey, Random = null)

        /// <summary>
        /// Sign the given signature input with the given private key.
        /// The result is the concatenation of the two ECDSA components r and
        /// s, each zero-padded to the width of the group order of the
        /// elliptic curve [RFC 9053, Section 2.1] - NOT the DER encoding that
        /// the Bouncy Castle and .NET signer utilities produce by default.
        /// </summary>
        /// <param name="ToBeSigned">The signature input, i.e. an encoded Sig_structure.</param>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="Random">An optional source of randomness for the ECDSA nonce.</param>
        public Byte[] Sign(ReadOnlySpan<Byte>      ToBeSigned,
                           ECPrivateKeyParameters  PrivateKey,
                           SecureRandom?           Random   = null)
        {

            EnsureUsableWith(PrivateKey.Parameters);

            var componentSizeInBytes  = (PrivateKey.Parameters.N.BitLength + 7) / 8;

            var signer                = new ECDsaSigner();
            signer.Init(true, new ParametersWithRandom(PrivateKey, Random ?? new SecureRandom()));

            var components            = signer.GenerateSignature(Hash(ToBeSigned));

            var signature             = new Byte[2 * componentSizeInBytes];
            BigIntegers.AsUnsignedByteArray(components[0], signature, 0,                    componentSizeInBytes);
            BigIntegers.AsUnsignedByteArray(components[1], signature, componentSizeInBytes, componentSizeInBytes);

            return signature;

        }

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
                              ECPublicKeyParameters             PublicKey,
                              [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (!IsSupportedForSigning)
            {
                ErrorResponse = $"The COSE algorithm '{Name}' is not supported for verification by this implementation!";
                return false;
            }

            if (FixedCurve.HasValue)
            {

                if (!COSECurve.TryGetFor(PublicKey.Parameters, out var curve) ||
                    curve != FixedCurve.Value)
                {
                    ErrorResponse = $"The COSE algorithm '{Name}' is defined for the elliptic curve '{FixedCurve.Value.Name}', which is not the curve of the given key!";
                    return false;
                }

            }

            var componentSizeInBytes = (PublicKey.Parameters.N.BitLength + 7) / 8;

            if (Signature.Length != 2 * componentSizeInBytes)
            {
                ErrorResponse = $"An ECDSA signature on the given elliptic curve must be {2 * componentSizeInBytes} bytes long, but was {Signature.Length} bytes long! (A DER encoded signature is a common cause: COSE concatenates r and s.)";
                return false;
            }

            var verifier = new ECDsaSigner();
            verifier.Init(false, PublicKey);

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
