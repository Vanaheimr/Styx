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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A COSE key [RFC 9052, Section 7]: an elliptic curve key with both
    /// coordinates (key type EC2) as used by ECDSA, an EdDSA key (OKP), an
    /// ML-DSA key (AKP), an RSA key (RSA) or a symmetric key.
    ///
    /// The coordinates are fixed-width, zero-padded byte strings: Leading zero
    /// octets MUST be preserved [RFC 9053, Section 7.1.1]. Stripping them - as
    /// a plain big integer serialization does - produces keys that other
    /// implementations reject, which is why this class always pads and
    /// verifies the width when reading.
    /// </summary>
    public sealed class COSEKey
    {

        #region Data

        /// <summary>
        /// The key type (label 1).
        /// </summary>
        public static CBORValue  KeyTypeLabel         { get; } = CBORValue.FromInt64( 1);

        /// <summary>
        /// The key identifier (label 2).
        /// </summary>
        public static CBORValue  KeyIdentifierLabel   { get; } = CBORValue.FromInt64( 2);

        /// <summary>
        /// The algorithm this key is restricted to (label 3).
        /// </summary>
        public static CBORValue  AlgorithmLabel       { get; } = CBORValue.FromInt64( 3);

        /// <summary>
        /// The operations this key may be used for (label 4).
        /// </summary>
        public static CBORValue  KeyOperationsLabel   { get; } = CBORValue.FromInt64( 4);

        /// <summary>
        /// The elliptic curve (label -1).
        /// </summary>
        public static CBORValue  CurveLabel           { get; } = CBORValue.FromInt64(-1);

        /// <summary>
        /// The x coordinate (label -2).
        /// </summary>
        public static CBORValue  XLabel               { get; } = CBORValue.FromInt64(-2);

        /// <summary>
        /// The y coordinate (label -3).
        /// </summary>
        public static CBORValue  YLabel               { get; } = CBORValue.FromInt64(-3);

        /// <summary>
        /// The private key (label -4).
        /// </summary>
        public static CBORValue  DLabel               { get; } = CBORValue.FromInt64(-4);

        /// <summary>
        /// The public key of an algorithm key pair (label -1) [RFC 9964].
        /// The same label as the curve, and a different parameter: which of
        /// the two it is depends on the key type.
        /// </summary>
        public static CBORValue  PubLabel             { get; } = CBORValue.FromInt64(-1);

        /// <summary>
        /// The private key of an algorithm key pair (label -2) [RFC 9964].
        /// The same label as the x coordinate.
        /// </summary>
        public static CBORValue  PrivLabel            { get; } = CBORValue.FromInt64(-2);

        /// <summary>
        /// The value of a symmetric key (label -1) [RFC 9053, Section 7.3].
        /// The same label a THIRD time: the curve on an EC2 or OKP key, the
        /// public key on an algorithm key pair, and the shared secret here.
        /// </summary>
        public static CBORValue  KLabel               { get; } = CBORValue.FromInt64(-1);

        /// <summary>
        /// The modulus n of an RSA key (label -1) [RFC 8230, Section 4].
        /// Label -1 a FOURTH time, and -2 to -8 follow suit: on an RSA key
        /// -3 is the private exponent where an EC2 key has its y coordinate,
        /// and -4 the first prime where an EC2 key has its private key.
        /// </summary>
        public static CBORValue  NLabel               { get; } = CBORValue.FromInt64(-1);

        /// <summary>
        /// The public exponent e of an RSA key (label -2).
        /// </summary>
        public static CBORValue  ELabel               { get; } = CBORValue.FromInt64(-2);

        /// <summary>
        /// The private exponent d of an RSA key (label -3).
        /// </summary>
        public static CBORValue  RSADLabel            { get; } = CBORValue.FromInt64(-3);

        /// <summary>
        /// The first prime factor p of an RSA key (label -4).
        /// </summary>
        public static CBORValue  PLabel               { get; } = CBORValue.FromInt64(-4);

        /// <summary>
        /// The second prime factor q of an RSA key (label -5).
        /// </summary>
        public static CBORValue  QLabel               { get; } = CBORValue.FromInt64(-5);

        /// <summary>
        /// The first CRT exponent dP = d mod (p - 1) of an RSA key (label -6).
        /// </summary>
        public static CBORValue  DPLabel              { get; } = CBORValue.FromInt64(-6);

        /// <summary>
        /// The second CRT exponent dQ = d mod (q - 1) of an RSA key (label -7).
        /// </summary>
        public static CBORValue  DQLabel              { get; } = CBORValue.FromInt64(-7);

        /// <summary>
        /// The CRT coefficient qInv = q^-1 mod p of an RSA key (label -8).
        /// </summary>
        public static CBORValue  QInvLabel            { get; } = CBORValue.FromInt64(-8);

        /// <summary>
        /// The further primes of a multi-prime RSA key (label -9), which this
        /// implementation keeps but can not compute with.
        /// </summary>
        public static CBORValue  OtherPrimesLabel     { get; } = CBORValue.FromInt64(-9);

        #endregion

        #region Properties

        /// <summary>
        /// The type of this key.
        /// </summary>
        public COSEKeyType                                        KeyType                 { get; }

        /// <summary>
        /// An optional key identifier: An opaque byte string that carries
        /// no trust by itself, it merely says which key to try.
        /// </summary>
        public Byte[]?                                            KeyIdentifier           { get; }

        /// <summary>
        /// An optional algorithm this key is restricted to.
        /// </summary>
        public COSEAlgorithm?                                     Algorithm               { get; }

        /// <summary>
        /// The optional operations this key may be used for.
        /// </summary>
        public IReadOnlyList<CBORValue>?                          KeyOperations           { get; }

        /// <summary>
        /// The elliptic curve of this key.
        /// </summary>
        public COSECurve?                                         Curve                   { get; }

        /// <summary>
        /// The x coordinate of the public point, zero-padded
        /// to the width of a field element.
        /// </summary>
        public Byte[]?                                            X                       { get; }

        /// <summary>
        /// The y coordinate of the public point, zero-padded
        /// to the width of a field element.
        /// </summary>
        public Byte[]?                                            Y                       { get; }

        /// <summary>
        /// The private key, zero-padded to the width of the group order.
        /// On a key of key type OKP this is the EdDSA private key, which is
        /// a fixed-width octet string rather than a scalar. On a key of key
        /// type RSA it is the private exponent d (label -3), without leading
        /// zeroes like every RSA parameter.
        /// </summary>
        public Byte[]?                                            D                       { get; }

        /// <summary>
        /// The modulus n of an RSA key (label -1) [RFC 8230]. Every RSA
        /// parameter is an unsigned big-endian integer of the fewest octets
        /// that hold it: no leading zeroes, unlike the coordinates of an
        /// elliptic curve key, and the thumbprint depends on it.
        /// </summary>
        public Byte[]?                                            N                       { get; }

        /// <summary>
        /// The public exponent e of an RSA key (label -2).
        /// </summary>
        public Byte[]?                                            E                       { get; }

        /// <summary>
        /// The first prime factor p of an RSA private key (label -4).
        /// </summary>
        public Byte[]?                                            P                       { get; }

        /// <summary>
        /// The second prime factor q of an RSA private key (label -5).
        /// </summary>
        public Byte[]?                                            Q                       { get; }

        /// <summary>
        /// The first CRT exponent dP of an RSA private key (label -6).
        /// </summary>
        public Byte[]?                                            DP                      { get; }

        /// <summary>
        /// The second CRT exponent dQ of an RSA private key (label -7).
        /// </summary>
        public Byte[]?                                            DQ                      { get; }

        /// <summary>
        /// The CRT coefficient qInv of an RSA private key (label -8).
        /// </summary>
        public Byte[]?                                            QInv                    { get; }

        /// <summary>
        /// The public key of an algorithm key pair (label -1) [RFC 9964].
        ///
        /// Note the numeric collision: on a key of key type EC2 or OKP, label
        /// -1 is the CURVE and -2 the x coordinate. Which of the two readings
        /// applies is decided by the key type alone, which is why parsing
        /// establishes that first.
        /// </summary>
        public Byte[]?                                            Pub                     { get; }

        /// <summary>
        /// The private key of an algorithm key pair (label -2) [RFC 9964],
        /// which is the 32-byte SEED and not the expanded secret key: the
        /// expanded ML-DSA-87 key is 4896 bytes and derivable from it.
        /// </summary>
        public Byte[]?                                            Priv                    { get; }

        /// <summary>
        /// The value of a symmetric key (label -1) [RFC 9053, Section 7.3].
        ///
        /// And that is label -1 meaning a THIRD thing: the curve on a key of
        /// key type EC2 or OKP, the public key on an algorithm key pair, and
        /// here the shared secret itself. A parser switching on the label
        /// alone reads a shared secret as a curve identifier and reports
        /// nothing wrong, which is why the key type is established first.
        /// </summary>
        public Byte[]?                                            K                       { get; }

        /// <summary>
        /// All key parameters that are not part of the fixed shape above,
        /// preserved so that unknown parameters survive a roundtrip.
        /// </summary>
        public IReadOnlyList<KeyValuePair<CBORValue, CBORValue>>  AdditionalParameters    { get; }

        /// <summary>
        /// Whether this key holds private key material.
        ///
        /// A symmetric key is ALWAYS secret - it has nothing else - which is
        /// why K counts here beside the two private halves.
        /// </summary>
        public Boolean                                            IsPrivate

            => D is not null || Priv is not null || K is not null ||
               P is not null || Q    is not null || DP is not null || DQ is not null || QInv is not null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new COSE key.
        /// </summary>
        /// <param name="KeyType">The type of the key.</param>
        /// <param name="Curve">An optional elliptic curve.</param>
        /// <param name="X">An optional x coordinate of the public point.</param>
        /// <param name="Y">An optional y coordinate of the public point.</param>
        /// <param name="D">An optional private key.</param>
        /// <param name="KeyIdentifier">An optional key identifier.</param>
        /// <param name="Algorithm">An optional algorithm this key is restricted to.</param>
        /// <param name="KeyOperations">The optional operations this key may be used for.</param>
        /// <param name="AdditionalParameters">Optional additional key parameters.</param>
        /// <param name="Pub">The optional public key of an algorithm key pair.</param>
        /// <param name="Priv">The optional private key seed of an algorithm key pair.</param>
        /// <param name="K">The optional value of a symmetric key.</param>
        /// <param name="N">The optional modulus of an RSA key.</param>
        /// <param name="E">The optional public exponent of an RSA key.</param>
        /// <param name="P">The optional first prime factor of an RSA private key.</param>
        /// <param name="Q">The optional second prime factor of an RSA private key.</param>
        /// <param name="DP">The optional first CRT exponent of an RSA private key.</param>
        /// <param name="DQ">The optional second CRT exponent of an RSA private key.</param>
        /// <param name="QInv">The optional CRT coefficient of an RSA private key.</param>
        public COSEKey(COSEKeyType                                      KeyType,
                       COSECurve?                                       Curve                  = null,
                       Byte[]?                                          X                      = null,
                       Byte[]?                                          Y                      = null,
                       Byte[]?                                          D                      = null,
                       Byte[]?                                          KeyIdentifier          = null,
                       COSEAlgorithm?                                   Algorithm              = null,
                       IEnumerable<CBORValue>?                          KeyOperations          = null,
                       IEnumerable<KeyValuePair<CBORValue, CBORValue>>? AdditionalParameters   = null,
                       Byte[]?                                          Pub                    = null,
                       Byte[]?                                          Priv                   = null,
                       Byte[]?                                          K                      = null,
                       Byte[]?                                          N                      = null,
                       Byte[]?                                          E                      = null,
                       Byte[]?                                          P                      = null,
                       Byte[]?                                          Q                      = null,
                       Byte[]?                                          DP                     = null,
                       Byte[]?                                          DQ                     = null,
                       Byte[]?                                          QInv                   = null)
        {

            this.KeyType               = KeyType;
            this.Curve                 = Curve;
            this.X                     = X;
            this.Y                     = Y;
            this.D                     = D;
            this.Pub                   = Pub;
            this.Priv                  = Priv;
            this.K                     = K;
            this.N                     = N;
            this.E                     = E;
            this.P                     = P;
            this.Q                     = Q;
            this.DP                    = DP;
            this.DQ                    = DQ;
            this.QInv                  = QInv;
            this.KeyIdentifier         = KeyIdentifier;
            this.Algorithm             = Algorithm;
            this.KeyOperations         = KeyOperations?.       ToArray();
            this.AdditionalParameters  = AdditionalParameters?.ToArray() ?? [];

        }

        #endregion


        #region (static) From    (PublicKey,  KeyIdentifier = null, Algorithm = null)

        /// <summary>
        /// Create a COSE key from the given elliptic curve public key.
        /// </summary>
        /// <param name="PublicKey">An elliptic curve public key.</param>
        /// <param name="KeyIdentifier">An optional key identifier.</param>
        /// <param name="Algorithm">An optional algorithm this key is restricted to.</param>
        public static COSEKey From(ECPublicKeyParameters  PublicKey,
                                   Byte[]?                KeyIdentifier   = null,
                                   COSEAlgorithm?         Algorithm       = null)
        {

            if (!COSECurve.TryGetFor(PublicKey.Parameters, out var curve))
                throw new COSEException("The elliptic curve of the given public key is not registered within the IANA \"COSE Elliptic Curves\" registry!");

            var point = PublicKey.Q.Normalize();

            return new COSEKey(
                       COSEKeyType.EC2,
                       curve,
                       point.AffineXCoord.GetEncoded(),
                       point.AffineYCoord.GetEncoded(),
                       null,
                       KeyIdentifier,
                       Algorithm
                   );

        }

        #endregion

        #region (static) From    (PrivateKey, KeyIdentifier = null, Algorithm = null)

        /// <summary>
        /// Create a COSE key from the given elliptic curve private key.
        /// The public point is computed from the private key, so that the
        /// resulting COSE key holds the complete key pair, exactly as the
        /// private key examples of RFC 9052 do.
        /// </summary>
        /// <param name="PrivateKey">An elliptic curve private key.</param>
        /// <param name="KeyIdentifier">An optional key identifier.</param>
        /// <param name="Algorithm">An optional algorithm this key is restricted to.</param>
        public static COSEKey From(ECPrivateKeyParameters  PrivateKey,
                                   Byte[]?                 KeyIdentifier   = null,
                                   COSEAlgorithm?          Algorithm       = null)
        {

            if (!COSECurve.TryGetFor(PrivateKey.Parameters, out var curve))
                throw new COSEException("The elliptic curve of the given private key is not registered within the IANA \"COSE Elliptic Curves\" registry!");

            var publicKey  = Crypto.CalculatePublicKey(PrivateKey);
            var point      = publicKey.Q.Normalize();

            return new COSEKey(
                       COSEKeyType.EC2,
                       curve,
                       point.AffineXCoord.GetEncoded(),
                       point.AffineYCoord.GetEncoded(),
                       BigIntegers.AsUnsignedByteArray(
                           curve.OrderSizeInBytes ?? (PrivateKey.Parameters.N.BitLength + 7) / 8,
                           PrivateKey.D
                       ),
                       KeyIdentifier,
                       Algorithm
                   );

        }

        #endregion

        #region (static) From    (Key, KeyIdentifier = null, Algorithm = null)

        /// <summary>
        /// Create a COSE key from any key this library can sign or verify
        /// with: an elliptic curve key (key type EC2), an EdDSA key (OKP) or
        /// an ML-DSA key (AKP).
        ///
        /// A private key always yields the complete key pair, as the private
        /// key examples of RFC 9052 do: the public half is derived rather than
        /// asked for, so the two cannot disagree.
        /// </summary>
        /// <param name="Key">A public or private key.</param>
        /// <param name="KeyIdentifier">An optional key identifier.</param>
        /// <param name="Algorithm">An optional algorithm this key is restricted to. Not optional for an ML-DSA key, where the algorithm is part of the key's identity - it is then derived from the key's own parameter set.</param>
        public static COSEKey From(AsymmetricKeyParameter  Key,
                                   Byte[]?                 KeyIdentifier   = null,
                                   COSEAlgorithm?          Algorithm       = null)

            => Key switch {

                   ECPrivateKeyParameters  ecPrivate
                       => From(ecPrivate,  KeyIdentifier, Algorithm),

                   ECPublicKeyParameters   ecPublic
                       => From(ecPublic,   KeyIdentifier, Algorithm),

                   // EdDSA: the public key is the whole of x, and there is no
                   // y to go with it.
                   Ed25519PrivateKeyParameters  ed25519Private
                       => new (COSEKeyType.OKP, COSECurve.Ed25519,
                               ed25519Private.GeneratePublicKey().GetEncoded(), null,
                               ed25519Private.GetEncoded(), KeyIdentifier, Algorithm),

                   Ed25519PublicKeyParameters   ed25519Public
                       => new (COSEKeyType.OKP, COSECurve.Ed25519,
                               ed25519Public.GetEncoded(), null, null, KeyIdentifier, Algorithm),

                   Ed448PrivateKeyParameters    ed448Private
                       => new (COSEKeyType.OKP, COSECurve.Ed448,
                               ed448Private.GeneratePublicKey().GetEncoded(), null,
                               ed448Private.GetEncoded(), KeyIdentifier, Algorithm),

                   Ed448PublicKeyParameters     ed448Public
                       => new (COSEKeyType.OKP, COSECurve.Ed448,
                               ed448Public.GetEncoded(), null, null, KeyIdentifier, Algorithm),

                   // ML-DSA: no curve, and the private key is the seed.
                   MLDsaPrivateKeyParameters    mlDsaPrivate
                       => new (COSEKeyType.AKP,
                               KeyIdentifier:  KeyIdentifier,
                               Algorithm:      Algorithm ?? COSEAlgorithm.TryGetFor(mlDsaPrivate.Parameters)
                                                   ?? throw new COSEException("The ML-DSA parameter set of the given private key is not registered within the IANA \"COSE Algorithms\" registry!"),
                               Pub:            mlDsaPrivate.GetPublicKeyEncoded(),
                               Priv:           mlDsaPrivate.GetSeed()
                                                   ?? throw new COSEException("The given ML-DSA private key was imported from an encoding and no longer holds its seed, which RFC 9964 requires a COSE key to carry!")),

                   MLDsaPublicKeyParameters     mlDsaPublic
                       => new (COSEKeyType.AKP,
                               KeyIdentifier:  KeyIdentifier,
                               Algorithm:      Algorithm ?? COSEAlgorithm.TryGetFor(mlDsaPublic.Parameters)
                                                   ?? throw new COSEException("The ML-DSA parameter set of the given public key is not registered within the IANA \"COSE Algorithms\" registry!"),
                               Pub:            mlDsaPublic.GetEncoded()),

                   // RSA: every parameter an integer of the fewest octets
                   // [RFC 8230, Section 4]. A private key without its CRT
                   // parameters does not even know its public exponent.
                   RsaPrivateCrtKeyParameters   rsaPrivate
                       => new (COSEKeyType.RSA,
                               D:              BigIntegers.AsUnsignedByteArray(rsaPrivate.Exponent),
                               KeyIdentifier:  KeyIdentifier,
                               Algorithm:      Algorithm,
                               N:              BigIntegers.AsUnsignedByteArray(rsaPrivate.Modulus),
                               E:              BigIntegers.AsUnsignedByteArray(rsaPrivate.PublicExponent),
                               P:              BigIntegers.AsUnsignedByteArray(rsaPrivate.P),
                               Q:              BigIntegers.AsUnsignedByteArray(rsaPrivate.Q),
                               DP:             BigIntegers.AsUnsignedByteArray(rsaPrivate.DP),
                               DQ:             BigIntegers.AsUnsignedByteArray(rsaPrivate.DQ),
                               QInv:           BigIntegers.AsUnsignedByteArray(rsaPrivate.QInv)),

                   RsaKeyParameters             rsaPrivate when rsaPrivate.IsPrivate
                       => throw new COSEException("An RSA private key without its CRT parameters can not become a COSE key, which carries all of them [RFC 8230, Section 4]!"),

                   RsaKeyParameters             rsaPublic
                       => new (COSEKeyType.RSA,
                               KeyIdentifier:  KeyIdentifier,
                               Algorithm:      Algorithm,
                               N:              BigIntegers.AsUnsignedByteArray(rsaPublic.Modulus),
                               E:              BigIntegers.AsUnsignedByteArray(rsaPublic.Exponent)),

                   _   => throw new COSEException($"A COSE key can not be created from a {Key.GetType().Name}!")

               };

        #endregion

        #region (static) Parse   (CBOR)

        /// <summary>
        /// Parse the given CBOR map as a COSE key.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE key.</param>
        public static COSEKey Parse(CBORValue CBOR)
        {

            if (TryParse(CBOR, out var key, out var errorResponse))
                return key;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region (static) Parse   (Data)

        /// <summary>
        /// Parse the given CBOR data as a COSE key.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE key.</param>
        public static COSEKey Parse(ReadOnlySpan<Byte> Data)
        {

            if (TryParse(Data, out var key, out var errorResponse))
                return key;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region (static) TryParse(Data, out Key, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR data as a COSE key.
        /// </summary>
        /// <param name="Data">The encoded CBOR data of a COSE key.</param>
        /// <param name="Key">The parsed COSE key.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte>                 Data,
                                       [NotNullWhen(true)]  out COSEKey?  Key,
                                       [NotNullWhen(false)] out String?   ErrorResponse)
        {

            if (!CBORValue.TryParse(Data, out var cbor, out ErrorResponse))
            {
                Key = null;
                return false;
            }

            return TryParse(cbor, out Key, out ErrorResponse);

        }

        #endregion

        #region (static) TryParse(CBOR, out Key, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR map as a COSE key.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of a COSE key.</param>
        /// <param name="Key">The parsed COSE key.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                          CBOR,
                                       [NotNullWhen(true)]  out COSEKey?  Key,
                                       [NotNullWhen(false)] out String?   ErrorResponse)
        {

            Key = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = $"A COSE key must be a CBOR map, but was a CBOR {CBOR.Kind}!";
                return false;
            }

            COSEKeyType?             keyType         = null;
            Byte[]?                  keyIdentifier   = null;
            COSEAlgorithm?           algorithm       = null;
            IEnumerable<CBORValue>?  keyOperations   = null;
            COSECurve?               curve           = null;
            Byte[]?                  x               = null;
            CBORValue?               y               = null;
            Byte[]?                  d               = null;
            Byte[]?                  pub             = null;
            Byte[]?                  priv            = null;
            Byte[]?                  n               = null;
            Byte[]?                  e               = null;
            Byte[]?                  p               = null;
            Byte[]?                  q               = null;
            Byte[]?                  dp              = null;
            Byte[]?                  dq              = null;
            Byte[]?                  qInv            = null;

            var additionalParameters = new List<KeyValuePair<CBORValue, CBORValue>>();

            #region The key type first, because it decides what -1 and -2 mean

            // On an EC2 or OKP key, label -1 is the curve and -2 the x
            // coordinate; on an AKP key [RFC 9964] they are the public and the
            // private key. Reading the key type in a pass of its own rather
            // than relying on it arriving first costs one loop and survives a
            // map in any order - and a parser that switched on the label alone
            // would read a 1312-byte ML-DSA public key as a curve identifier
            // and report nothing wrong at all.
            foreach (var parameter in CBOR.AsMap())
            {
                if (parameter.Key == KeyTypeLabel &&
                    parameter.Value.TryGetInt64(out var keyTypeValue))
                {
                    keyType = (COSEKeyType) keyTypeValue;
                }
            }

            var isAlgorithmKeyPair = keyType == COSEKeyType.AKP;
            var isSymmetric        = keyType == COSEKeyType.Symmetric;
            var isRSA              = keyType == COSEKeyType.RSA;

            Byte[]? k              = null;

            #endregion

            foreach (var parameter in CBOR.AsMap())
            {

                if      (parameter.Key == KeyTypeLabel)
                {

                    if (!parameter.Value.TryGetInt64(out var value))
                    {
                        ErrorResponse = "The key type of a COSE key must be an integer!";
                        return false;
                    }

                    keyType = (COSEKeyType) value;

                }

                else if (parameter.Key == KeyIdentifierLabel)
                {

                    if (!parameter.Value.TryGetBytes(out keyIdentifier))
                    {
                        ErrorResponse = "The key identifier of a COSE key must be a byte string!";
                        return false;
                    }

                }

                else if (parameter.Key == AlgorithmLabel)
                {

                    if (!COSEAlgorithm.TryParse(parameter.Value, out var parsedAlgorithm, out ErrorResponse))
                        return false;

                    algorithm = parsedAlgorithm;

                }

                else if (parameter.Key == KeyOperationsLabel)
                {

                    if (parameter.Value.Kind != CBORValueKind.Array)
                    {
                        ErrorResponse = "The key operations of a COSE key must be an array!";
                        return false;
                    }

                    keyOperations = parameter.Value.AsArray();

                }

                else if (parameter.Key == PubLabel)
                {

                    if (isAlgorithmKeyPair)
                    {

                        if (!parameter.Value.TryGetBytes(out pub))
                        {
                            ErrorResponse = "The public key of an algorithm key pair must be a byte string!";
                            return false;
                        }

                    }

                    else if (isSymmetric)
                    {

                        if (!parameter.Value.TryGetBytes(out k))
                        {
                            ErrorResponse = "The key value of a symmetric COSE key must be a byte string!";
                            return false;
                        }

                    }

                    else if (isRSA)
                    {
                        if (!TryReadRSAInteger(parameter.Value, "modulus n", out n, out ErrorResponse))
                            return false;
                    }

                    else
                    {

                        if (!COSECurve.TryParse(parameter.Value, out var parsedCurve, out ErrorResponse))
                            return false;

                        curve = parsedCurve;

                    }

                }

                else if (parameter.Key == PrivLabel)
                {

                    if (isAlgorithmKeyPair)
                    {

                        if (!parameter.Value.TryGetBytes(out priv))
                        {
                            ErrorResponse = "The private key of an algorithm key pair must be a byte string!";
                            return false;
                        }

                    }

                    else if (isRSA)
                    {
                        if (!TryReadRSAInteger(parameter.Value, "public exponent e", out e, out ErrorResponse))
                            return false;
                    }

                    else
                    {

                        if (!parameter.Value.TryGetBytes(out x))
                        {
                            ErrorResponse = "The x coordinate of a COSE key must be a byte string!";
                            return false;
                        }

                    }

                }

                else if (parameter.Key == YLabel)
                {

                    if (isRSA)
                    {
                        if (!TryReadRSAInteger(parameter.Value, "private exponent d", out d, out ErrorResponse))
                            return false;
                    }

                    else
                        y = parameter.Value;

                }

                else if (parameter.Key == DLabel)
                {

                    if (isRSA)
                    {
                        if (!TryReadRSAInteger(parameter.Value, "prime factor p", out p, out ErrorResponse))
                            return false;
                    }

                    else if (!parameter.Value.TryGetBytes(out d))
                    {
                        ErrorResponse = "The private key of a COSE key must be a byte string!";
                        return false;
                    }

                }

                else if (isRSA && parameter.Key == QLabel)
                {
                    if (!TryReadRSAInteger(parameter.Value, "prime factor q",          out q,    out ErrorResponse))
                        return false;
                }

                else if (isRSA && parameter.Key == DPLabel)
                {
                    if (!TryReadRSAInteger(parameter.Value, "CRT exponent dP",         out dp,   out ErrorResponse))
                        return false;
                }

                else if (isRSA && parameter.Key == DQLabel)
                {
                    if (!TryReadRSAInteger(parameter.Value, "CRT exponent dQ",         out dq,   out ErrorResponse))
                        return false;
                }

                else if (isRSA && parameter.Key == QInvLabel)
                {
                    if (!TryReadRSAInteger(parameter.Value, "CRT coefficient qInv",    out qInv, out ErrorResponse))
                        return false;
                }

                else
                    additionalParameters.Add(parameter);

            }

            if (!keyType.HasValue)
            {
                ErrorResponse = "A COSE key must have a key type!";
                return false;
            }

            // "For symmetric keys, it is REQUIRED that 'k' be present in the
            // structure" [RFC 9053, Section 7.3] - and unlike a missing public
            // key there is nothing to recompute it from.
            if (isSymmetric && k is null)
            {
                ErrorResponse = "A COSE key of key type Symmetric must carry its key value [RFC 9053, Section 7.3]!";
                return false;
            }

            // The public half is required of a private RSA key as well
            // [RFC 8230, Section 4].
            if (isRSA && (n is null || e is null))
            {
                ErrorResponse = "A COSE key of key type RSA must carry its modulus n and its public exponent e [RFC 8230, Section 4]!";
                return false;
            }

            Byte[]? yBytes = null;

            if (y.HasValue)
            {

                if (y.Value.TryGetBytes(out var explicitY))
                    yBytes = explicitY;

                else if (y.Value.Kind == CBORValueKind.Boolean)
                {

                    if (!TryDecompressY(curve, x, y.Value.AsBoolean(), out yBytes, out ErrorResponse))
                        return false;

                }

                else
                {
                    ErrorResponse = "The y coordinate of a COSE key must be a byte string or a boolean sign bit!";
                    return false;
                }

            }

            Key = new COSEKey(
                      keyType.Value,
                      curve,
                      x,
                      yBytes,
                      d,
                      keyIdentifier,
                      algorithm,
                      keyOperations,
                      additionalParameters,
                      pub,
                      priv,
                      k,
                      n,
                      e,
                      p,
                      q,
                      dp,
                      dq,
                      qInv
                  );

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region (private static) TryReadRSAInteger(Value, Name, out Integer, out ErrorResponse)

        /// <summary>
        /// Read an RSA key parameter: a byte string holding an unsigned
        /// big-endian integer in "the minimum number of octets needed to
        /// represent the value" [RFC 8230, Section 4]. A leading zero is
        /// refused rather than stripped, as the thumbprint is computed over
        /// the bytes as they are: the same key written twice would otherwise
        /// have two identities.
        /// </summary>
        private static Boolean TryReadRSAInteger(CBORValue                         Value,
                                                 String                            Name,
                                                 [NotNullWhen(true)]  out Byte[]?  Integer,
                                                 [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (!Value.TryGetBytes(out Integer))
            {
                ErrorResponse = $"The {Name} of an RSA key must be a byte string!";
                return false;
            }

            if (Integer.Length == 0 || Integer[0] == 0x00)
            {
                Integer       = null;
                ErrorResponse = $"The {Name} of an RSA key must be encoded in the minimum number of octets, without leading zeroes [RFC 8230, Section 4]!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region (private static) TryDecompressY(Curve, X, SignBit, out Y, out ErrorResponse)

        /// <summary>
        /// Recover the y coordinate from the x coordinate and the sign bit of
        /// the compressed point representation [RFC 9053, Section 7.1.1].
        /// </summary>
        private static Boolean TryDecompressY(COSECurve?                       Curve,
                                              Byte[]?                          X,
                                              Boolean                          SignBit,
                                              [NotNullWhen(true)]  out Byte[]? Y,
                                              [NotNullWhen(false)] out String? ErrorResponse)
        {

            Y = null;

            if (!Curve.HasValue || Curve.Value.DomainParameters is not ECDomainParameters domainParameters)
            {
                ErrorResponse = "A COSE key with a compressed y coordinate must name an elliptic curve this implementation can compute with!";
                return false;
            }

            if (X is null)
            {
                ErrorResponse = "A COSE key with a compressed y coordinate must have an x coordinate!";
                return false;
            }

            try
            {

                var compressed = new Byte[X.Length + 1];
                compressed[0]  = (Byte) (SignBit ? 0x03 : 0x02);
                X.CopyTo(compressed, 1);

                Y = domainParameters.Curve.DecodePoint(compressed).Normalize().AffineYCoord.GetEncoded();

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = $"The compressed public point of the COSE key could not be decompressed: {e.Message}";
                return false;
            }

        }

        #endregion


        #region ToPublicKey ()

        /// <summary>
        /// Convert this COSE key into a public key of its key type.
        /// </summary>
        public AsymmetricKeyParameter ToPublicKey()
        {

            if (TryToPublicKey(out var publicKey, out var errorResponse))
                return publicKey;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region TryToPublicKey (out PublicKey,  out ErrorResponse)

        /// <summary>
        /// Try to convert this COSE key into a public key of its key type:
        /// an elliptic curve point (EC2), an EdDSA key (OKP), an ML-DSA key
        /// (AKP) or an RSA key (RSA).
        /// </summary>
        /// <param name="PublicKey">The public key.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public Boolean TryToPublicKey([NotNullWhen(true)]  out AsymmetricKeyParameter?  PublicKey,
                                      [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            switch (KeyType)
            {

                case COSEKeyType.OKP:
                    return TryToOkpPublicKey(out PublicKey, out ErrorResponse);

                case COSEKeyType.AKP:
                    return TryToAkpPublicKey(out PublicKey, out ErrorResponse);

                case COSEKeyType.RSA:
                    return TryToRSAPublicKey(out PublicKey, out ErrorResponse);

            }

            var result = TryToECPublicKey(out var ecPublicKey, out ErrorResponse);
            PublicKey  = ecPublicKey;
            return result;

        }

        #endregion

        #region (private) TryToOkpPublicKey / TryToAkpPublicKey

        /// <summary>
        /// An EdDSA public key, which is the whole of x: an octet key pair has
        /// no second coordinate.
        /// </summary>
        private Boolean TryToOkpPublicKey([NotNullWhen(true)]  out AsymmetricKeyParameter?  PublicKey,
                                          [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            PublicKey = null;

            if (!Curve.HasValue)
            {
                ErrorResponse = "A COSE key of key type OKP must name its curve!";
                return false;
            }

            var expected = Curve.Value.OctetKeySizeInBytes;

            if (expected is null)
            {
                ErrorResponse = $"The curve '{Curve.Value.Name}' is not an EdDSA signature curve!";
                return false;
            }

            if (X is null || X.Length != expected)
            {
                ErrorResponse = $"The public key of a COSE key on the curve '{Curve.Value.Name}' must be {expected} bytes long, but was {X?.Length ?? 0} bytes long!";
                return false;
            }

            try
            {

                PublicKey      = Curve.Value == COSECurve.Ed448
                                     ? new Ed448PublicKeyParameters  (X, 0)
                                     : new Ed25519PublicKeyParameters(X, 0);

                ErrorResponse  = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = $"The public key of the COSE key is invalid: {e.Message}";
                return false;
            }

        }

        /// <summary>
        /// An ML-DSA public key, whose parameter set comes from the algorithm
        /// rather than from a curve.
        /// </summary>
        private Boolean TryToAkpPublicKey([NotNullWhen(true)]  out AsymmetricKeyParameter?  PublicKey,
                                          [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            PublicKey = null;

            if (!TryGetMLDsaParameters(out var parameterSet, out ErrorResponse))
                return false;

            if (Pub is null)
            {
                ErrorResponse = "A COSE key of key type AKP must carry its public key!";
                return false;
            }

            try
            {

                PublicKey      = MLDsaPublicKeyParameters.FromEncoding(parameterSet, Pub);
                ErrorResponse  = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = $"The public key of the COSE key is invalid: {e.Message}";
                return false;
            }

        }

        /// <summary>
        /// The ML-DSA parameter set this key belongs to, which is a property
        /// of its algorithm: an algorithm key pair has no curve to ask.
        /// </summary>
        private Boolean TryGetMLDsaParameters([NotNullWhen(true)]  out MLDsaParameters?  ParameterSet,
                                              [NotNullWhen(false)] out String?           ErrorResponse)
        {

            ParameterSet = Algorithm?.MLDsaParameterSet;

            if (ParameterSet is null)
            {
                ErrorResponse = Algorithm.HasValue
                                    ? $"The algorithm '{Algorithm.Value.Name}' of this COSE key is not an ML-DSA algorithm!"
                                    :  "A COSE key of key type AKP must name its algorithm, as its public key does not say which parameter set produced it!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region (private) TryToECPublicKey(out PublicKey, out ErrorResponse)

        /// <summary>
        /// Try to convert this COSE key into an elliptic curve public key.
        /// The resulting point is validated to actually lie on the curve.
        /// </summary>
        /// <param name="PublicKey">The elliptic curve public key.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        private Boolean TryToECPublicKey([NotNullWhen(true)]  out ECPublicKeyParameters?  PublicKey,
                                         [NotNullWhen(false)] out String?                 ErrorResponse)
        {

            PublicKey = null;

            if (!TryGetDomainParameters(out var domainParameters, out var fieldSizeInBytes, out ErrorResponse))
                return false;

            if (X is null || Y is null)
            {
                ErrorResponse = "A COSE key needs both an x and a y coordinate in order to become a public key!";
                return false;
            }

            if (X.Length != fieldSizeInBytes ||
                Y.Length != fieldSizeInBytes)
            {
                ErrorResponse = $"The coordinates of a COSE key on the curve '{Curve!.Value.Name}' must be {fieldSizeInBytes} bytes wide, including leading zeroes, but were {X.Length} and {Y.Length} bytes wide!";
                return false;
            }

            try
            {

                var point = domainParameters.Curve.CreatePoint(
                                new BigInteger(1, X),
                                new BigInteger(1, Y)
                            );

                if (!point.IsValid())
                {
                    ErrorResponse = $"The public point of the COSE key does not lie on the curve '{Curve!.Value.Name}'!";
                    return false;
                }

                PublicKey      = new ECPublicKeyParameters("ECDSA", point, domainParameters);
                ErrorResponse  = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = $"The public point of the COSE key is invalid: {e.Message}";
                return false;
            }

        }

        #endregion

        #region ToPrivateKey()

        /// <summary>
        /// Convert this COSE key into a private key of its key type.
        /// </summary>
        public AsymmetricKeyParameter ToPrivateKey()
        {

            if (TryToPrivateKey(out var privateKey, out var errorResponse))
                return privateKey;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region TryToPrivateKey(out PrivateKey, out ErrorResponse)

        /// <summary>
        /// Try to convert this COSE key into a private key of its key type.
        /// </summary>
        /// <param name="PrivateKey">The private key.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public Boolean TryToPrivateKey([NotNullWhen(true)]  out AsymmetricKeyParameter?  PrivateKey,
                                       [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            switch (KeyType)
            {

                case COSEKeyType.OKP:
                    return TryToOkpPrivateKey(out PrivateKey, out ErrorResponse);

                case COSEKeyType.AKP:
                    return TryToAkpPrivateKey(out PrivateKey, out ErrorResponse);

                case COSEKeyType.RSA:
                    return TryToRSAPrivateKey(out PrivateKey, out ErrorResponse);

            }

            var result  = TryToECPrivateKey(out var ecPrivateKey, out ErrorResponse);
            PrivateKey  = ecPrivateKey;
            return result;

        }

        #endregion

        #region (private) TryToOkpPrivateKey / TryToAkpPrivateKey

        private Boolean TryToOkpPrivateKey([NotNullWhen(true)]  out AsymmetricKeyParameter?  PrivateKey,
                                           [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            PrivateKey = null;

            if (!Curve.HasValue)
            {
                ErrorResponse = "A COSE key of key type OKP must name its curve!";
                return false;
            }

            var expected = Curve.Value.OctetKeySizeInBytes;

            if (expected is null)
            {
                ErrorResponse = $"The curve '{Curve.Value.Name}' is not an EdDSA signature curve!";
                return false;
            }

            if (D is null)
            {
                ErrorResponse = "The COSE key holds no private key material!";
                return false;
            }

            if (D.Length != expected)
            {
                ErrorResponse = $"The private key of a COSE key on the curve '{Curve.Value.Name}' must be {expected} bytes long, but was {D.Length} bytes long!";
                return false;
            }

            PrivateKey     = Curve.Value == COSECurve.Ed448
                                 ? new Ed448PrivateKeyParameters  (D, 0)
                                 : new Ed25519PrivateKeyParameters(D, 0);

            ErrorResponse  = null;
            return true;

        }

        /// <summary>
        /// An ML-DSA private key, expanded from the seed the COSE key carries
        /// [RFC 9964]: the wire form is 32 bytes, the working form up to 4896.
        /// </summary>
        private Boolean TryToAkpPrivateKey([NotNullWhen(true)]  out AsymmetricKeyParameter?  PrivateKey,
                                           [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            PrivateKey = null;

            if (!TryGetMLDsaParameters(out var parameterSet, out ErrorResponse))
                return false;

            if (Priv is null)
            {
                ErrorResponse = "The COSE key holds no private key material!";
                return false;
            }

            if (Priv.Length != 32)
            {
                ErrorResponse = $"The private key of an algorithm key pair is the seed and must be 32 bytes long [RFC 9964], but was {Priv.Length} bytes long!";
                return false;
            }

            try
            {

                PrivateKey     = MLDsaPrivateKeyParameters.FromSeed(parameterSet, Priv);
                ErrorResponse  = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = $"The private key of the COSE key is invalid: {e.Message}";
                return false;
            }

        }

        #endregion

        #region (private) TryToRSAPublicKey / TryToRSAPrivateKey

        /// <summary>
        /// An RSA public key: the modulus and the public exponent.
        /// </summary>
        private Boolean TryToRSAPublicKey([NotNullWhen(true)]  out AsymmetricKeyParameter?  PublicKey,
                                          [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            PublicKey = null;

            if (N is null || E is null)
            {
                ErrorResponse = "A COSE key of key type RSA must carry its modulus n and its public exponent e [RFC 8230, Section 4]!";
                return false;
            }

            try
            {

                PublicKey      = new RsaKeyParameters(false, new BigInteger(1, N), new BigInteger(1, E));
                ErrorResponse  = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = $"The public key of the COSE key is invalid: {e.Message}";
                return false;
            }

        }

        /// <summary>
        /// An RSA private key with all of its CRT parameters, which RFC 8230
        /// requires of a two-prime key, and which must agree with each other.
        ///
        /// The check is not pedantry. A CRT signer computes the signature
        /// modulo p and modulo q and joins the halves; if one half is wrong,
        /// the faulty signature and the message give away a factor of n
        /// [Boneh, DeMillo, Lipton 1997]. A key whose dP does not belong to
        /// its d is exactly such a fault, built in.
        /// </summary>
        private Boolean TryToRSAPrivateKey([NotNullWhen(true)]  out AsymmetricKeyParameter?  PrivateKey,
                                           [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            PrivateKey = null;

            if (AdditionalParameters.Any(parameter => parameter.Key == OtherPrimesLabel))
            {
                ErrorResponse = "Multi-prime RSA keys are not supported by this implementation!";
                return false;
            }

            if (N is null || E is null)
            {
                ErrorResponse = "A COSE key of key type RSA must carry its modulus n and its public exponent e [RFC 8230, Section 4]!";
                return false;
            }

            if (D is null)
            {
                ErrorResponse = "The COSE key holds no private key material!";
                return false;
            }

            if (P is null || Q is null || DP is null || DQ is null || QInv is null)
            {
                ErrorResponse = "An RSA private key must carry d, p, q, dP, dQ and qInv [RFC 8230, Section 4]!";
                return false;
            }

            var n     = new BigInteger(1, N);
            var e     = new BigInteger(1, E);
            var d     = new BigInteger(1, D);
            var p     = new BigInteger(1, P);
            var q     = new BigInteger(1, Q);
            var dp    = new BigInteger(1, DP);
            var dq    = new BigInteger(1, DQ);
            var qInv  = new BigInteger(1, QInv);

            var pMinus1 = p.Subtract(BigInteger.One);
            var qMinus1 = q.Subtract(BigInteger.One);

            if (!p.Multiply(q).Equals(n))
            {
                ErrorResponse = "The prime factors p and q of the RSA key are not the factors of its modulus n!";
                return false;
            }

            // e·d ≡ 1 modulo the least common multiple of p-1 and q-1, which
            // holds whether d was reduced modulo λ(n) or modulo φ(n).
            var lambda = pMinus1.Multiply(qMinus1).Divide(pMinus1.Gcd(qMinus1));

            if (!e.Multiply(d).Mod(lambda).Equals(BigInteger.One))
            {
                ErrorResponse = "The private exponent d of the RSA key does not belong to its public exponent e!";
                return false;
            }

            if (!d.Mod(pMinus1).Equals(dp) ||
                !d.Mod(qMinus1).Equals(dq) ||
                !q.Multiply(qInv).Mod(p).Equals(BigInteger.One))
            {
                ErrorResponse = "The CRT parameters dP, dQ and qInv of the RSA key do not belong to its d, p and q!";
                return false;
            }

            try
            {

                PrivateKey     = new RsaPrivateCrtKeyParameters(n, e, d, p, q, dp, dq, qInv);
                ErrorResponse  = null;
                return true;

            }
            catch (Exception exception)
            {
                ErrorResponse = $"The private key of the COSE key is invalid: {exception.Message}";
                return false;
            }

        }

        #endregion

        #region (private) TryToECPrivateKey(out PrivateKey, out ErrorResponse)

        private Boolean TryToECPrivateKey([NotNullWhen(true)]  out ECPrivateKeyParameters?  PrivateKey,
                                          [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            PrivateKey = null;

            if (!TryGetDomainParameters(out var domainParameters, out _, out ErrorResponse))
                return false;

            if (D is null)
            {
                ErrorResponse = "The COSE key holds no private key material!";
                return false;
            }

            var orderSizeInBytes = (domainParameters.N.BitLength + 7) / 8;

            if (D.Length != orderSizeInBytes)
            {
                ErrorResponse = $"The private key of a COSE key on the curve '{Curve!.Value.Name}' must be {orderSizeInBytes} bytes wide, including leading zeroes, but was {D.Length} bytes wide!";
                return false;
            }

            var d = new BigInteger(1, D);

            if (d.SignValue <= 0 || d.CompareTo(domainParameters.N) >= 0)
            {
                ErrorResponse = "The private key of the COSE key is not within the group order of its elliptic curve!";
                return false;
            }

            PrivateKey     = new ECPrivateKeyParameters("ECDSA", d, domainParameters);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region (private) TryGetDomainParameters(out DomainParameters, out FieldSizeInBytes, out ErrorResponse)

        /// <summary>
        /// Try to get the elliptic curve domain parameters of this COSE key.
        /// </summary>
        private Boolean TryGetDomainParameters([NotNullWhen(true)]  out ECDomainParameters?  DomainParameters,
                                                                    out Int32                FieldSizeInBytes,
                                               [NotNullWhen(false)] out String?              ErrorResponse)
        {

            DomainParameters  = null;
            FieldSizeInBytes  = 0;

            if (KeyType != COSEKeyType.EC2)
            {
                ErrorResponse = $"Only COSE keys of key type EC2 are supported, but this key is of key type {KeyType}!";
                return false;
            }

            if (!Curve.HasValue)
            {
                ErrorResponse = "A COSE key of key type EC2 must name an elliptic curve!";
                return false;
            }

            DomainParameters = Curve.Value.DomainParameters;

            if (DomainParameters is null)
            {
                ErrorResponse = $"This implementation can not compute with the elliptic curve '{Curve.Value.Name}'!";
                return false;
            }

            FieldSizeInBytes  = (DomainParameters.Curve.FieldSize + 7) / 8;
            ErrorResponse     = null;
            return true;

        }

        #endregion


        #region ToCBOR()

        /// <summary>
        /// Return a CBOR map representation of this COSE key.
        /// The public point is always written uncompressed, as the byte string
        /// form of the y coordinate is understood by every implementation.
        /// </summary>
        public CBORValue ToCBOR()
        {

            var parameters = new List<KeyValuePair<CBORValue, CBORValue>> {
                                 new (KeyTypeLabel, CBORValue.FromInt64((Int64) KeyType))
                             };

            if (KeyIdentifier is not null)
                parameters.Add(new (KeyIdentifierLabel, CBORValue.FromBytes(KeyIdentifier)));

            if (Algorithm.HasValue)
                parameters.Add(new (AlgorithmLabel,     Algorithm.Value.ToCBOR()));

            if (KeyOperations is not null)
                parameters.Add(new (KeyOperationsLabel, CBORValue.FromArray(KeyOperations)));

            // An algorithm key pair carries its two parameters under the very
            // labels an elliptic curve key uses for the curve and the x
            // coordinate, so the two shapes are written apart rather than
            // together [RFC 9964].
            if (KeyType == COSEKeyType.AKP)
            {

                if (Pub is not null)
                    parameters.Add(new (PubLabel,       CBORValue.FromBytes(Pub)));

                if (Priv is not null)
                    parameters.Add(new (PrivLabel,      CBORValue.FromBytes(Priv)));

            }

            // Encoding a symmetric key writes the secret out, always: there is
            // no half of it that can be published. RFC 9053 says as much -
            // "care must be taken that it is never transmitted accidentally or
            // insecurely".
            else if (KeyType == COSEKeyType.Symmetric)
            {

                if (K is not null)
                    parameters.Add(new (KLabel,         CBORValue.FromBytes(K)));

            }

            // An RSA key: -1 to -8 once more, with meanings of their own
            // [RFC 8230, Section 4].
            else if (KeyType == COSEKeyType.RSA)
            {

                if (N    is not null) parameters.Add(new (NLabel,    CBORValue.FromBytes(N)));
                if (E    is not null) parameters.Add(new (ELabel,    CBORValue.FromBytes(E)));
                if (D    is not null) parameters.Add(new (RSADLabel, CBORValue.FromBytes(D)));
                if (P    is not null) parameters.Add(new (PLabel,    CBORValue.FromBytes(P)));
                if (Q    is not null) parameters.Add(new (QLabel,    CBORValue.FromBytes(Q)));
                if (DP   is not null) parameters.Add(new (DPLabel,   CBORValue.FromBytes(DP)));
                if (DQ   is not null) parameters.Add(new (DQLabel,   CBORValue.FromBytes(DQ)));
                if (QInv is not null) parameters.Add(new (QInvLabel, CBORValue.FromBytes(QInv)));

            }

            else
            {

                if (Curve.HasValue)
                    parameters.Add(new (CurveLabel,     Curve.Value.ToCBOR()));

                if (X is not null)
                    parameters.Add(new (XLabel,         CBORValue.FromBytes(X)));

                if (Y is not null)
                    parameters.Add(new (YLabel,         CBORValue.FromBytes(Y)));

                if (D is not null)
                    parameters.Add(new (DLabel,         CBORValue.FromBytes(D)));

            }

            parameters.AddRange(AdditionalParameters);

            return CBORValue.FromMap(parameters);

        }

        #endregion

        #region ToByteArray(Options = null)

        /// <summary>
        /// Return the CBOR encoding of this COSE key.
        /// </summary>
        /// <param name="Options">Optional CBOR writer options.</param>
        public Byte[] ToByteArray(CBORWriterOptions? Options = null)

            => ToCBOR().ToByteArray(Options);

        #endregion

        #region ThumbprintInput()

        /// <summary>
        /// Return the byte string a COSE Key Thumbprint is computed over
        /// [RFC 9679]: A CBOR map holding ONLY the parameters that are
        /// required for this key type, in the deterministic encoding of
        /// RFC 8949, Section 4.2.1.
        ///
        /// The optional parameters are deliberately left out, so that adding
        /// a key identifier, an algorithm or the private key to a key does
        /// not change its thumbprint. The public and the private half of one
        /// key pair therefore have the very same thumbprint, which is what
        /// makes it usable as an identity.
        ///
        /// This is public for the same reason ToBeSigned is: whoever computes
        /// the thumbprint elsewhere needs to be able to compare the input,
        /// not just the result.
        /// </summary>
        public Byte[] ThumbprintInput()
        {

            var parameters = new List<KeyValuePair<CBORValue, CBORValue>> {
                                 new (KeyTypeLabel, CBORValue.FromInt64((Int64) KeyType))
                             };

            switch (KeyType)
            {

                case COSEKeyType.EC2:
                {

                    if (!Curve.HasValue || X is null || Y is null)
                        throw new COSEException("The thumbprint of a COSE key of key type EC2 needs its curve and both of its coordinates!");

                    parameters.Add(new (CurveLabel, Curve.Value.ToCBOR()));
                    parameters.Add(new (XLabel,     CBORValue.FromBytes(X)));
                    parameters.Add(new (YLabel,     CBORValue.FromBytes(Y)));

                    break;

                }

                case COSEKeyType.OKP:
                {

                    if (!Curve.HasValue || X is null)
                        throw new COSEException("The thumbprint of a COSE key of key type OKP needs its curve and its public key!");

                    parameters.Add(new (CurveLabel, Curve.Value.ToCBOR()));
                    parameters.Add(new (XLabel,     CBORValue.FromBytes(X)));

                    break;

                }

                case COSEKeyType.AKP:
                {

                    // The one key type whose ALGORITHM is a required parameter
                    // of its thumbprint [RFC 9964], and necessarily so: an
                    // ML-DSA public key does not say which parameter set
                    // produced it, so two keys of different strengths would
                    // otherwise be able to share an identity.
                    if (!Algorithm.HasValue || Pub is null)
                        throw new COSEException("The thumbprint of a COSE key of key type AKP needs its algorithm and its public key!");

                    parameters.Add(new (AlgorithmLabel, Algorithm.Value.ToCBOR()));
                    parameters.Add(new (PubLabel,       CBORValue.FromBytes(Pub)));

                    break;

                }

                case COSEKeyType.RSA:
                {

                    // The modulus and the public exponent [RFC 9679, Section
                    // 4.3], as the fewest octets: that is why a parsed key
                    // with a leading zero is refused rather than kept.
                    if (N is null || E is null)
                        throw new COSEException("The thumbprint of a COSE key of key type RSA needs its modulus and its public exponent!");

                    parameters.Add(new (NLabel, CBORValue.FromBytes(N)));
                    parameters.Add(new (ELabel, CBORValue.FromBytes(E)));

                    break;

                }

                case COSEKeyType.Symmetric:
                {

                    if (K is null)
                        throw new COSEException("The thumbprint of a COSE key of key type Symmetric needs its key value!");

                    // RFC 9679 Section 4.4 defines this and Section 7
                    // immediately warns about it: the thumbprint of a
                    // symmetric key is a hash OF THE SECRET, so a low-entropy
                    // one can simply be looked up in a precomputed table. It
                    // is a usable identifier for a randomly chosen key of at
                    // least 128 bits and MUST NOT be used for passwords or
                    // anything resembling one. Note also what is NOT covered:
                    // the algorithm, unlike an algorithm key pair.
                    parameters.Add(new (KLabel, CBORValue.FromBytes(K)));

                    break;

                }

                default:
                    throw new COSEException($"The thumbprint of a COSE key of key type {KeyType} is not implemented!");

            }

            return CBORValue.FromMap(parameters).ToByteArray(CBORWriterOptions.Canonical);

        }

        #endregion

        #region Thumbprint(HashAlgorithm = null)

        /// <summary>
        /// Return the COSE Key Thumbprint of this key [RFC 9679]: The hash of
        /// the required key parameters in their deterministic encoding.
        /// </summary>
        /// <param name="HashAlgorithm">The hash algorithm to use, SHA-256 by default, which RFC 9679 requires every implementation to support.</param>
        public Byte[] Thumbprint(HashAlgorithmName? HashAlgorithm = null)
        {

            var input          = ThumbprintInput();
            var hashAlgorithm  = HashAlgorithm ?? HashAlgorithmName.SHA256;

            if (hashAlgorithm == HashAlgorithmName.SHA256)
                return SHA256.HashData(input);

            if (hashAlgorithm == HashAlgorithmName.SHA384)
                return SHA384.HashData(input);

            if (hashAlgorithm == HashAlgorithmName.SHA512)
                return SHA512.HashData(input);

            throw new COSEException($"The hash algorithm '{hashAlgorithm.Name}' is not implemented for COSE key thumbprints!");

        }

        #endregion

        #region ThumbprintKeyIdentifier(LengthInBytes = 8, HashAlgorithm = null)

        /// <summary>
        /// Return the leading bytes of the COSE Key Thumbprint of this key,
        /// for use as a key identifier.
        ///
        /// Everyone who holds the public key can recompute this identifier,
        /// so it needs no registry and no agreement beyond its length. And
        /// because the thumbprint covers the curve, a signer who changes
        /// their algorithm necessarily has a different key and therefore a
        /// different identifier: an algorithm downgrade under an unchanged
        /// identity is not expressible.
        /// </summary>
        /// <param name="LengthInBytes">The number of leading bytes to take, 8 by default.</param>
        /// <param name="HashAlgorithm">The hash algorithm to use, SHA-256 by default.</param>
        public Byte[] ThumbprintKeyIdentifier(Int32               LengthInBytes   = 8,
                                              HashAlgorithmName?  HashAlgorithm   = null)
        {

            var thumbprint = Thumbprint(HashAlgorithm);

            if (LengthInBytes < 1 || LengthInBytes > thumbprint.Length)
                throw new COSEException($"A key identifier derived from a COSE key thumbprint must be between 1 and {thumbprint.Length} bytes long!");

            return thumbprint[..LengthInBytes];

        }

        #endregion

        #region WithThumbprintKeyIdentifier(LengthInBytes = 8, HashAlgorithm = null)

        /// <summary>
        /// Return a copy of this key whose key identifier is the leading
        /// bytes of its own COSE Key Thumbprint - the one line a key needs
        /// when it is provisioned.
        /// </summary>
        /// <param name="LengthInBytes">The number of leading bytes to take, 8 by default.</param>
        /// <param name="HashAlgorithm">The hash algorithm to use, SHA-256 by default.</param>
        public COSEKey WithThumbprintKeyIdentifier(Int32               LengthInBytes   = 8,
                                                   HashAlgorithmName?  HashAlgorithm   = null)

            => new (KeyType,
                    Curve,
                    X,
                    Y,
                    D,
                    ThumbprintKeyIdentifier(LengthInBytes, HashAlgorithm),
                    Algorithm,
                    KeyOperations,
                    AdditionalParameters,
                    Pub,
                    Priv,
                    K,
                    N,
                    E,
                    P,
                    Q,
                    DP,
                    DQ,
                    QInv);

        #endregion

        #region ToPublicCOSEKey()

        /// <summary>
        /// Return a copy of this COSE key without its private key material,
        /// e.g. in order to publish it.
        ///
        /// A symmetric key has no such form, and saying so is the point rather
        /// than a limitation: RFC 9053 Section 7.3 states outright that the
        /// structure "does not have a form that contains only public members".
        /// Dropping D and Priv from one would return it unchanged, handing the
        /// caller the shared secret under a name promising the opposite.
        /// </summary>
        public COSEKey ToPublicCOSEKey()

            => KeyType == COSEKeyType.Symmetric

                   ? throw new COSEException("A COSE key of key type Symmetric has no public half [RFC 9053, Section 7.3]!")

                   : new (KeyType,
                    Curve,
                    X,
                    Y,
                    null,
                    KeyIdentifier,
                    Algorithm,
                    KeyOperations,
                    // The further primes of a multi-prime RSA key are private.
                    AdditionalParameters.Where(parameter => KeyType != COSEKeyType.RSA || parameter.Key != OtherPrimesLabel),
                    Pub,
                    null,
                    null,
                    N,
                    E);

        #endregion

        #region (static) FromSymmetricKey(K, KeyIdentifier = null, Algorithm = null)

        /// <summary>
        /// Create a symmetric COSE key from a shared secret
        /// [RFC 9053, Section 7.3].
        ///
        /// No length is enforced. RFC 9053 says an HMAC key SHOULD be as wide
        /// as the hash output, which is advice about key management rather
        /// than a rule about the primitive - RFC 2104 accepts any width, and
        /// the published vectors of RFC 4231 include a four-byte key. What a
        /// caller must not do is derive one from a password, and no length
        /// check would catch that.
        /// </summary>
        /// <param name="K">The shared secret.</param>
        /// <param name="KeyIdentifier">An optional key identifier.</param>
        /// <param name="Algorithm">An optional algorithm this key is restricted to.</param>
        public static COSEKey FromSymmetricKey(Byte[]          K,
                                               Byte[]?         KeyIdentifier   = null,
                                               COSEAlgorithm?  Algorithm       = null)
        {

            if (K.Length == 0)
                throw new COSEException("A symmetric COSE key must carry a key value!");

            return new (COSEKeyType.Symmetric,
                        null, null, null, null,
                        KeyIdentifier,
                        Algorithm,
                        null,
                        null,
                        null,
                        null,
                        K);

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{KeyType}{(Curve.HasValue ? $" {Curve.Value.Name}" : "")}{(N is not null ? $" {new BigInteger(1, N).BitLength}" : "")}{(IsPrivate ? " private" : " public")} key{(KeyIdentifier is not null ? $" '{KeyIdentifier.ToHexString()}'" : "")}";

        #endregion

    }

}
