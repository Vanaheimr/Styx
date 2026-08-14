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

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A COSE key [RFC 9052, Section 7], currently for elliptic curve keys
    /// with both coordinates (key type EC2) as used by ECDSA.
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
        /// </summary>
        public Byte[]?                                            D                       { get; }

        /// <summary>
        /// All key parameters that are not part of the fixed shape above,
        /// preserved so that unknown parameters survive a roundtrip.
        /// </summary>
        public IReadOnlyList<KeyValuePair<CBORValue, CBORValue>>  AdditionalParameters    { get; }

        /// <summary>
        /// Whether this key holds private key material.
        /// </summary>
        public Boolean                                            IsPrivate

            => D is not null;

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
        public COSEKey(COSEKeyType                                      KeyType,
                       COSECurve?                                       Curve                  = null,
                       Byte[]?                                          X                      = null,
                       Byte[]?                                          Y                      = null,
                       Byte[]?                                          D                      = null,
                       Byte[]?                                          KeyIdentifier          = null,
                       COSEAlgorithm?                                   Algorithm              = null,
                       IEnumerable<CBORValue>?                          KeyOperations          = null,
                       IEnumerable<KeyValuePair<CBORValue, CBORValue>>? AdditionalParameters   = null)
        {

            this.KeyType               = KeyType;
            this.Curve                 = Curve;
            this.X                     = X;
            this.Y                     = Y;
            this.D                     = D;
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

            var additionalParameters = new List<KeyValuePair<CBORValue, CBORValue>>();

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

                else if (parameter.Key == CurveLabel)
                {

                    if (!COSECurve.TryParse(parameter.Value, out var parsedCurve, out ErrorResponse))
                        return false;

                    curve = parsedCurve;

                }

                else if (parameter.Key == XLabel)
                {

                    if (!parameter.Value.TryGetBytes(out x))
                    {
                        ErrorResponse = "The x coordinate of a COSE key must be a byte string!";
                        return false;
                    }

                }

                else if (parameter.Key == YLabel)
                    y = parameter.Value;

                else if (parameter.Key == DLabel)
                {

                    if (!parameter.Value.TryGetBytes(out d))
                    {
                        ErrorResponse = "The private key of a COSE key must be a byte string!";
                        return false;
                    }

                }

                else
                    additionalParameters.Add(parameter);

            }

            if (!keyType.HasValue)
            {
                ErrorResponse = "A COSE key must have a key type!";
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
                      additionalParameters
                  );

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
        /// Convert this COSE key into an elliptic curve public key.
        /// </summary>
        public ECPublicKeyParameters ToPublicKey()
        {

            if (TryToPublicKey(out var publicKey, out var errorResponse))
                return publicKey;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region TryToPublicKey (out PublicKey,  out ErrorResponse)

        /// <summary>
        /// Try to convert this COSE key into an elliptic curve public key.
        /// The resulting point is validated to actually lie on the curve.
        /// </summary>
        /// <param name="PublicKey">The elliptic curve public key.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public Boolean TryToPublicKey([NotNullWhen(true)]  out ECPublicKeyParameters?  PublicKey,
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
        /// Convert this COSE key into an elliptic curve private key.
        /// </summary>
        public ECPrivateKeyParameters ToPrivateKey()
        {

            if (TryToPrivateKey(out var privateKey, out var errorResponse))
                return privateKey;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region TryToPrivateKey(out PrivateKey, out ErrorResponse)

        /// <summary>
        /// Try to convert this COSE key into an elliptic curve private key.
        /// </summary>
        /// <param name="PrivateKey">The elliptic curve private key.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public Boolean TryToPrivateKey([NotNullWhen(true)]  out ECPrivateKeyParameters?  PrivateKey,
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

            if (Curve.HasValue)
                parameters.Add(new (CurveLabel,         Curve.Value.ToCBOR()));

            if (X is not null)
                parameters.Add(new (XLabel,             CBORValue.FromBytes(X)));

            if (Y is not null)
                parameters.Add(new (YLabel,             CBORValue.FromBytes(Y)));

            if (D is not null)
                parameters.Add(new (DLabel,             CBORValue.FromBytes(D)));

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

        #region ToPublicCOSEKey()

        /// <summary>
        /// Return a copy of this COSE key without its private key material,
        /// e.g. in order to publish it.
        /// </summary>
        public COSEKey ToPublicCOSEKey()

            => new (KeyType,
                    Curve,
                    X,
                    Y,
                    null,
                    KeyIdentifier,
                    Algorithm,
                    KeyOperations,
                    AdditionalParameters);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{KeyType}{(Curve.HasValue ? $" {Curve.Value.Name}" : "")}{(IsPrivate ? " private" : " public")} key{(KeyIdentifier is not null ? $" '{KeyIdentifier.ToHexString()}'" : "")}";

        #endregion

    }

}
