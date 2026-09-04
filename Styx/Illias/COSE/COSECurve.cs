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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An elliptic curve identifier of the IANA "COSE Elliptic Curves"
    /// registry [RFC 9052, Section 13].
    /// Any integer is a syntactically valid identifier, as curves may be
    /// registered after this implementation was written; IsKnown tells
    /// whether this library knows the curve, and DomainParameters whether
    /// it can compute with it.
    /// </summary>
    public readonly struct COSECurve : IEquatable<COSECurve>,
                                       IComparable<COSECurve>,
                                       IComparable
    {

        #region Data

        /// <summary>
        /// The registered curves: numeric identification =&gt; (name, key type,
        /// name within the Bouncy Castle named curve table, key size).
        ///
        /// A curve of key type OKP has no named elliptic curve parameters
        /// here, because it is not used through the elliptic curve machinery:
        /// an EdDSA key is a fixed-width octet string, and the width is what
        /// this registry has to state instead. Ed448 is 57 bytes and not 56 —
        /// RFC 8032 appends a sign bit to the 456-bit encoding, which costs a
        /// whole further octet.
        ///
        /// X25519 and X448 carry no size, because they sign nothing: they are
        /// key agreement curves, and this library has no use for them beyond
        /// recognizing them.
        /// </summary>
        private static readonly Dictionary<Int32, (String Name, COSEKeyType KeyType, String? BouncyCastleName, Int32? OctetKeySize)> registry = new () {
            {   1, ("P-256",           COSEKeyType.EC2, "secp256r1",       null) },
            {   2, ("P-384",           COSEKeyType.EC2, "secp384r1",       null) },
            {   3, ("P-521",           COSEKeyType.EC2, "secp521r1",       null) },
            {   4, ("X25519",          COSEKeyType.OKP, null,              null) },
            {   5, ("X448",            COSEKeyType.OKP, null,              null) },
            {   6, ("Ed25519",         COSEKeyType.OKP, null,              32  ) },
            {   7, ("Ed448",           COSEKeyType.OKP, null,              57  ) },
            {   8, ("secp256k1",       COSEKeyType.EC2, "secp256k1",       null) },
            { 256, ("brainpoolP256r1", COSEKeyType.EC2, "brainpoolP256r1", null) },
            { 257, ("brainpoolP320r1", COSEKeyType.EC2, "brainpoolP320r1", null) },
            { 258, ("brainpoolP384r1", COSEKeyType.EC2, "brainpoolP384r1", null) },
            { 259, ("brainpoolP512r1", COSEKeyType.EC2, "brainpoolP512r1", null) }
        };

        /// <summary>
        /// A cache of the elliptic curve domain parameters, as looking them
        /// up within Bouncy Castle rebuilds the curve every single time.
        /// </summary>
        private static readonly ConcurrentDictionary<Int32, ECDomainParameters>  domainParameterCache   = [];

        #endregion

        #region Properties

        /// <summary>
        /// The numeric identification of this elliptic curve.
        /// </summary>
        public Int32       Value               { get; }

        /// <summary>
        /// Whether this elliptic curve is registered and known
        /// to this implementation.
        /// </summary>
        public Boolean     IsKnown

            => registry.ContainsKey(Value);

        /// <summary>
        /// The registered name of this elliptic curve, e.g. "P-256",
        /// or its numeric identification when unknown.
        /// </summary>
        public String      Name

            => registry.TryGetValue(Value, out var entry)
                   ? entry.Name
                   : Value.ToString();

        /// <summary>
        /// The key type this elliptic curve belongs to.
        /// </summary>
        public COSEKeyType?  KeyType

            => registry.TryGetValue(Value, out var entry)
                   ? entry.KeyType
                   : null;

        /// <summary>
        /// The elliptic curve domain parameters of this curve,
        /// or null when this implementation can not compute with it.
        /// </summary>
        public ECDomainParameters?  DomainParameters

            => registry.TryGetValue(Value, out var entry) &&
               entry.BouncyCastleName is not null

                   ? domainParameterCache.GetOrAdd(
                         Value,
                         static (_, bouncyCastleName) => {

                             var parameters = ECNamedCurveTable.GetByName(bouncyCastleName)
                                                  ?? throw new COSEException($"The elliptic curve '{bouncyCastleName}' is unknown to Bouncy Castle!");

                             return new ECDomainParameters(
                                        parameters.Curve,
                                        parameters.G,
                                        parameters.N,
                                        parameters.H,
                                        parameters.GetSeed()
                                    );

                         },
                         entry.BouncyCastleName
                     )

                   : null;

        /// <summary>
        /// The size of a field element of this curve in bytes, which is
        /// the fixed width of the encoded x and y coordinates
        /// [RFC 9053, Section 7.1.1].
        /// </summary>
        public Int32?      FieldSizeInBytes
        {
            get
            {

                var domainParameters = DomainParameters;

                // An OKP curve has no field element to measure: its public key
                // is one fixed-width octet string, and the registry states it.
                return domainParameters is not null
                           ? (domainParameters.Curve.FieldSize + 7) / 8
                           : OctetKeySizeInBytes;

            }
        }

        /// <summary>
        /// The size of the group order of this curve in bytes, which is
        /// the fixed width of both ECDSA signature components and of an
        /// encoded private key [RFC 9053, Section 2.1].
        /// </summary>
        public Int32?      OrderSizeInBytes
        {
            get
            {

                var domainParameters = DomainParameters;

                return domainParameters is not null
                           ? (domainParameters.N.BitLength + 7) / 8
                           : OctetKeySizeInBytes;

            }
        }

        /// <summary>
        /// The fixed width of an EdDSA key on this curve, or null when the
        /// curve is not one this library signs with [RFC 8032].
        /// </summary>
        public Int32?      OctetKeySizeInBytes

            => registry.TryGetValue(Value, out var entry)
                   ? entry.OctetKeySize
                   : null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new COSE elliptic curve identifier based on the given number.
        /// </summary>
        /// <param name="Value">The numeric identification of the elliptic curve.</param>
        public COSECurve(Int32 Value)
        {
            this.Value = Value;
        }

        #endregion


        #region Static defaults

        /// <summary>
        /// NIST P-256, also known as secp256r1 or prime256v1 (curve 1).
        /// </summary>
        public static COSECurve  P256               { get; } = new (  1);

        /// <summary>
        /// NIST P-384, also known as secp384r1 (curve 2).
        /// </summary>
        public static COSECurve  P384               { get; } = new (  2);

        /// <summary>
        /// NIST P-521, also known as secp521r1 (curve 3).
        /// Note that the group order is 521 bits, thus 66 bytes wide.
        /// </summary>
        public static COSECurve  P521               { get; } = new (  3);

        /// <summary>
        /// X25519 for use with ECDH only (curve 4).
        /// </summary>
        public static COSECurve  X25519             { get; } = new (  4);

        /// <summary>
        /// X448 for use with ECDH only (curve 5).
        /// </summary>
        public static COSECurve  X448               { get; } = new (  5);

        /// <summary>
        /// Ed25519 for use with EdDSA only (curve 6).
        /// </summary>
        public static COSECurve  Ed25519            { get; } = new (  6);

        /// <summary>
        /// Ed448 for use with EdDSA only (curve 7).
        /// </summary>
        public static COSECurve  Ed448              { get; } = new (  7);

        /// <summary>
        /// The SECG curve secp256k1 (curve 8) [RFC 8812].
        /// </summary>
        public static COSECurve  Secp256k1          { get; } = new (  8);

        /// <summary>
        /// brainpoolP256r1 (curve 256), registered by ISO/IEC 18013-5:2021.
        /// </summary>
        public static COSECurve  BrainpoolP256r1    { get; } = new (256);

        /// <summary>
        /// brainpoolP320r1 (curve 257), registered by ISO/IEC 18013-5:2021.
        /// </summary>
        public static COSECurve  BrainpoolP320r1    { get; } = new (257);

        /// <summary>
        /// brainpoolP384r1 (curve 258), registered by ISO/IEC 18013-5:2021.
        /// </summary>
        public static COSECurve  BrainpoolP384r1    { get; } = new (258);

        /// <summary>
        /// brainpoolP512r1 (curve 259), registered by ISO/IEC 18013-5:2021.
        /// </summary>
        public static COSECurve  BrainpoolP512r1    { get; } = new (259);

        /// <summary>
        /// An enumeration of all elliptic curves known to this implementation.
        /// </summary>
        public static IEnumerable<COSECurve> All

            => registry.Keys.Select(static value => new COSECurve(value));

        #endregion


        #region (static) Parse    (Name)

        /// <summary>
        /// Parse the given text as a COSE elliptic curve name.
        /// </summary>
        /// <param name="Name">The registered name of an elliptic curve.</param>
        public static COSECurve Parse(String Name)
        {

            if (TryParse(Name, out var curve))
                return curve;

            throw new COSEException($"The given text '{Name}' is not a known COSE elliptic curve name!");

        }

        #endregion

        #region (static) TryParse (Name,             out Curve)

        /// <summary>
        /// Try to parse the given text as a COSE elliptic curve name.
        /// Names are compared case-sensitively, as "P-256" and "p-256"
        /// are not the same registered name.
        /// </summary>
        /// <param name="Name">The registered name of an elliptic curve.</param>
        /// <param name="Curve">The parsed elliptic curve.</param>
        public static Boolean TryParse(String Name, out COSECurve Curve)
        {

            foreach (var entry in registry)
            {
                if (String.Equals(entry.Value.Name, Name, StringComparison.Ordinal))
                {
                    Curve = new COSECurve(entry.Key);
                    return true;
                }
            }

            Curve = default;
            return false;

        }

        #endregion

        #region (static) TryParse (Number,           out Curve)

        /// <summary>
        /// Try to parse the given number as a COSE elliptic curve identifier
        /// known to this implementation.
        /// </summary>
        /// <param name="Number">The numeric identification of an elliptic curve.</param>
        /// <param name="Curve">The parsed elliptic curve.</param>
        public static Boolean TryParse(Int32 Number, out COSECurve Curve)
        {

            if (registry.ContainsKey(Number))
            {
                Curve = new COSECurve(Number);
                return true;
            }

            Curve = default;
            return false;

        }

        #endregion

        #region (static) TryGetFor(DomainParameters, out Curve)

        /// <summary>
        /// Try to find the COSE elliptic curve identifier of the given
        /// elliptic curve domain parameters, e.g. in order to serialize
        /// a Bouncy Castle key as a COSE_Key.
        /// </summary>
        /// <param name="DomainParameters">Elliptic curve domain parameters.</param>
        /// <param name="Curve">The matching elliptic curve.</param>
        public static Boolean TryGetFor(ECDomainParameters DomainParameters, out COSECurve Curve)
        {

            foreach (var entry in registry)
            {

                if (entry.Value.BouncyCastleName is null)
                    continue;

                var candidate = new COSECurve(entry.Key).DomainParameters;

                if (candidate is not null &&
                    candidate.Curve.Equals(DomainParameters.Curve) &&
                    candidate.N.    Equals(DomainParameters.N))
                {
                    Curve = new COSECurve(entry.Key);
                    return true;
                }

            }

            Curve = default;
            return false;

        }

        #endregion

        #region (static) TryParse (CBOR,             out Curve, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR value as a COSE elliptic curve identifier.
        /// </summary>
        /// <param name="CBOR">A CBOR representation of an elliptic curve identifier.</param>
        /// <param name="Curve">The parsed elliptic curve.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                        CBOR,
                                       out COSECurve                    Curve,
                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Curve = default;

            if (!CBOR.TryGetInt64(out var value) ||
                value < Int32.MinValue ||
                value > Int32.MaxValue)
            {
                ErrorResponse = "The COSE elliptic curve identifier must be an integer!";
                return false;
            }

            Curve          = new COSECurve((Int32) value);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region ToCBOR()

        /// <summary>
        /// Return a CBOR representation of this elliptic curve identifier.
        /// </summary>
        public CBORValue ToCBOR()

            => CBORValue.FromInt64(Value);

        #endregion


        #region Operator overloading

        #region Operator == (COSECurve1, COSECurve2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSECurve1">An elliptic curve.</param>
        /// <param name="COSECurve2">Another elliptic curve.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (COSECurve COSECurve1,
                                           COSECurve COSECurve2)

            => COSECurve1.Equals(COSECurve2);

        #endregion

        #region Operator != (COSECurve1, COSECurve2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSECurve1">An elliptic curve.</param>
        /// <param name="COSECurve2">Another elliptic curve.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (COSECurve COSECurve1,
                                           COSECurve COSECurve2)

            => !COSECurve1.Equals(COSECurve2);

        #endregion

        #region Operator <  (COSECurve1, COSECurve2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSECurve1">An elliptic curve.</param>
        /// <param name="COSECurve2">Another elliptic curve.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (COSECurve COSECurve1,
                                          COSECurve COSECurve2)

            => COSECurve1.CompareTo(COSECurve2) < 0;

        #endregion

        #region Operator <= (COSECurve1, COSECurve2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSECurve1">An elliptic curve.</param>
        /// <param name="COSECurve2">Another elliptic curve.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (COSECurve COSECurve1,
                                           COSECurve COSECurve2)

            => COSECurve1.CompareTo(COSECurve2) <= 0;

        #endregion

        #region Operator >  (COSECurve1, COSECurve2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSECurve1">An elliptic curve.</param>
        /// <param name="COSECurve2">Another elliptic curve.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (COSECurve COSECurve1,
                                          COSECurve COSECurve2)

            => COSECurve1.CompareTo(COSECurve2) > 0;

        #endregion

        #region Operator >= (COSECurve1, COSECurve2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="COSECurve1">An elliptic curve.</param>
        /// <param name="COSECurve2">Another elliptic curve.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (COSECurve COSECurve1,
                                           COSECurve COSECurve2)

            => COSECurve1.CompareTo(COSECurve2) >= 0;

        #endregion

        #endregion

        #region IComparable<COSECurve> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two elliptic curves.
        /// </summary>
        /// <param name="Object">An elliptic curve to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is COSECurve cOSECurve
                   ? CompareTo(cOSECurve)
                   : throw new ArgumentException("The given object is not a COSE elliptic curve!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(COSECurve)

        /// <summary>
        /// Compares two elliptic curves.
        /// </summary>
        /// <param name="COSECurve">An elliptic curve to compare with.</param>
        public Int32 CompareTo(COSECurve COSECurve)

            => Value.CompareTo(COSECurve.Value);

        #endregion

        #endregion

        #region IEquatable<COSECurve> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two elliptic curves for equality.
        /// </summary>
        /// <param name="Object">An elliptic curve to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is COSECurve cOSECurve &&
                   Equals(cOSECurve);

        #endregion

        #region Equals(COSECurve)

        /// <summary>
        /// Compares two elliptic curves for equality.
        /// </summary>
        /// <param name="COSECurve">An elliptic curve to compare with.</param>
        public Boolean Equals(COSECurve COSECurve)

            => Value == COSECurve.Value;

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
