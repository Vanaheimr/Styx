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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A CBOR tag number (RFC 8949, Section 3.4), used to give
    /// additional semantics to the data item that follows it.
    /// </summary>
    public readonly struct CBORTag : IEquatable <CBORTag>,
                                     IComparable<CBORTag>,
                                     IComparable
    {

        #region Properties

        /// <summary>
        /// The numeric value of this CBOR tag.
        /// </summary>
        public UInt64  Value    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CBOR tag based on the given number.
        /// </summary>
        /// <param name="Value">The numeric value of the CBOR tag.</param>
        public CBORTag(UInt64 Value)
        {
            this.Value = Value;
        }

        #endregion


        #region Static defaults

        /// <summary>
        /// Standard date/time string (tag 0): The tagged text string is
        /// a date/time string as per RFC 3339 [RFC 8949, Section 3.4.1].
        /// </summary>
        public static CBORTag  DateTimeString       { get; } = new (0);

        /// <summary>
        /// Epoch-based date/time (tag 1): The tagged integer or floating-point
        /// number is the number of seconds since 1970-01-01T00:00Z
        /// [RFC 8949, Section 3.4.2].
        /// </summary>
        public static CBORTag  EpochDateTime        { get; } = new (1);

        /// <summary>
        /// Unsigned bignum (tag 2): The tagged byte string is an unsigned
        /// integer n in network byte order [RFC 8949, Section 3.4.3].
        /// </summary>
        public static CBORTag  UnsignedBignum       { get; } = new (2);

        /// <summary>
        /// Negative bignum (tag 3): The tagged byte string is an unsigned
        /// integer n in network byte order representing the value -1-n
        /// [RFC 8949, Section 3.4.3].
        /// </summary>
        public static CBORTag  NegativeBignum       { get; } = new (3);

        /// <summary>
        /// Decimal fraction (tag 4): The tagged array of [exponent, mantissa]
        /// represents the value mantissa * 10^exponent [RFC 8949, Section 3.4.4].
        /// </summary>
        public static CBORTag  DecimalFraction      { get; } = new (4);

        /// <summary>
        /// Bigfloat (tag 5): The tagged array of [exponent, mantissa]
        /// represents the value mantissa * 2^exponent [RFC 8949, Section 3.4.4].
        /// </summary>
        public static CBORTag  BigFloat             { get; } = new (5);

        /// <summary>
        /// COSE Single Recipient Encrypted Data Object, COSE_Encrypt0
        /// (tag 16) [RFC 9052, Section 2].
        /// </summary>
        public static CBORTag  COSEEncrypt0         { get; } = new (16);

        /// <summary>
        /// COSE MACed Data Object without recipients, COSE_Mac0
        /// (tag 17) [RFC 9052, Section 2].
        /// </summary>
        public static CBORTag  COSEMac0             { get; } = new (17);

        /// <summary>
        /// COSE Single Signer Data Object, COSE_Sign1 (tag 18)
        /// [RFC 9052, Section 4.2]: The tagged array
        /// [protected, unprotected, payload, signature] is a payload signed
        /// by a single signer. The tag itself is not covered by the
        /// signature.
        /// </summary>
        public static CBORTag  COSESign1            { get; } = new (18);

        /// <summary>
        /// Encoded CBOR data item (tag 24): The tagged byte string
        /// contains a single encoded CBOR data item [RFC 8949, Section 3.4.5.1].
        /// </summary>
        public static CBORTag  EncodedCBOR          { get; } = new (24);

        /// <summary>
        /// URI (tag 32): The tagged text string is a Uniform Resource
        /// Identifier as per RFC 3986 [RFC 8949, Section 3.4.5.3].
        /// </summary>
        public static CBORTag  URI                  { get; } = new (32);

        /// <summary>
        /// base64url (tag 33): The tagged text string is base64url-encoded
        /// data [RFC 8949, Section 3.4.5.3].
        /// </summary>
        public static CBORTag  Base64URL            { get; } = new (33);

        /// <summary>
        /// base64 (tag 34): The tagged text string is base64-encoded
        /// data [RFC 8949, Section 3.4.5.3].
        /// </summary>
        public static CBORTag  Base64               { get; } = new (34);

        /// <summary>
        /// MIME message (tag 36): The tagged text string is a
        /// MIME message as per RFC 2045.
        /// </summary>
        public static CBORTag  MIMEMessage          { get; } = new (36);

        /// <summary>
        /// UUID (tag 37): The tagged byte string is a binary
        /// Universally Unique Identifier as per RFC 9562.
        /// </summary>
        public static CBORTag  UUID                 { get; } = new (37);

        /// <summary>
        /// COSE Encrypted Data Object, COSE_Encrypt
        /// (tag 96) [RFC 9052, Section 2].
        /// </summary>
        public static CBORTag  COSEEncrypt          { get; } = new (96);

        /// <summary>
        /// COSE MACed Data Object, COSE_Mac
        /// (tag 97) [RFC 9052, Section 2].
        /// </summary>
        public static CBORTag  COSEMac              { get; } = new (97);

        /// <summary>
        /// COSE Signed Data Object, COSE_Sign (tag 98) [RFC 9052, Section 4.1]:
        /// A payload signed by one or more signers, each with its own header
        /// buckets and signature.
        /// </summary>
        public static CBORTag  COSESign             { get; } = new (98);

        /// <summary>
        /// A metrological value (tag 44252, 0xACDC): The tagged array
        /// [value, unit, ?prefix, ?uncertainty] represents the reading of a
        /// physical quantity. 'value' is an integer or a decimal fraction
        /// (tag 4, never a binary floating-point number, preserving the decimal
        /// scale as displayed by the measuring instrument). 'unit' is either a
        /// named unit - an unsigned integer identifier from the unit registry
        /// of this specification, or a text string holding its symbol - or an
        /// array of [named unit, exponent] factors describing a product of
        /// powers such as metre per second squared, where an exponent may be a
        /// [numerator, denominator] pair for rational powers such as the
        /// reciprocal square root of hertz. The optional 'prefix' is the
        /// decimal power of the SI prefix the whole reading is scaled by (e.g.
        /// 3 for kilo, -3 for milli; absent means 0) and must be one of the
        /// canonical SI prefix exponents. The optional 'uncertainty' is the
        /// symmetric measurement uncertainty as defined by the "Guide to the
        /// Expression of Uncertainty in Measurement" (JCGM 100:2008),
        /// expressed in the same unit and prefix as the value and never
        /// negative: a bare number is the standard uncertainty u, while a map
        /// may additionally state the coverage factor the magnitude belongs to,
        /// the coverage probability, the probability distribution and the
        /// effective degrees of freedom.
        /// See: https://github.com/Vanaheimr/Styx/blob/master/Styx/Illias/CBOR/tag-44252.md
        /// </summary>
        public static CBORTag  MetrologicalValue    { get; } = new (44252);

        /// <summary>
        /// Self-described CBOR (tag 55799): The tagged data item can be
        /// used as a "magic number" to identify CBOR data [RFC 8949, Section 3.4.6].
        /// </summary>
        public static CBORTag  SelfDescribedCBOR    { get; } = new (55799);

        #endregion


        #region (implicit) CBORTag(Number)

        /// <summary>
        /// Convert the given number into a CBOR tag.
        /// </summary>
        /// <param name="Number">The numeric value of the CBOR tag.</param>
        public static implicit operator CBORTag(UInt64 Number)

            => new (Number);

        #endregion


        #region Operator overloading

        #region Operator == (CBORTag1, CBORTag2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORTag1">A CBOR tag.</param>
        /// <param name="CBORTag2">Another CBOR tag.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CBORTag CBORTag1,
                                           CBORTag CBORTag2)

            => CBORTag1.Equals(CBORTag2);

        #endregion

        #region Operator != (CBORTag1, CBORTag2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORTag1">A CBOR tag.</param>
        /// <param name="CBORTag2">Another CBOR tag.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CBORTag CBORTag1,
                                           CBORTag CBORTag2)

            => !CBORTag1.Equals(CBORTag2);

        #endregion

        #region Operator <  (CBORTag1, CBORTag2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORTag1">A CBOR tag.</param>
        /// <param name="CBORTag2">Another CBOR tag.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CBORTag CBORTag1,
                                          CBORTag CBORTag2)

            => CBORTag1.CompareTo(CBORTag2) < 0;

        #endregion

        #region Operator <= (CBORTag1, CBORTag2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORTag1">A CBOR tag.</param>
        /// <param name="CBORTag2">Another CBOR tag.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CBORTag CBORTag1,
                                           CBORTag CBORTag2)

            => CBORTag1.CompareTo(CBORTag2) <= 0;

        #endregion

        #region Operator >  (CBORTag1, CBORTag2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORTag1">A CBOR tag.</param>
        /// <param name="CBORTag2">Another CBOR tag.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CBORTag CBORTag1,
                                          CBORTag CBORTag2)

            => CBORTag1.CompareTo(CBORTag2) > 0;

        #endregion

        #region Operator >= (CBORTag1, CBORTag2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORTag1">A CBOR tag.</param>
        /// <param name="CBORTag2">Another CBOR tag.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CBORTag CBORTag1,
                                           CBORTag CBORTag2)

            => CBORTag1.CompareTo(CBORTag2) >= 0;

        #endregion

        #endregion

        #region IComparable<CBORTag> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two CBOR tags.
        /// </summary>
        /// <param name="Object">A CBOR tag to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null             => 1,
                   CBORTag cborTag  => CompareTo(cborTag),
                   _                => throw new ArgumentException("The given object is not a CBOR tag!", nameof(Object))
               };

        #endregion

        #region CompareTo(CBORTag)

        /// <summary>
        /// Compares two CBOR tags.
        /// </summary>
        /// <param name="CBORTag">A CBOR tag to compare with.</param>
        public Int32 CompareTo(CBORTag CBORTag)

            => Value.CompareTo(CBORTag.Value);

        #endregion

        #endregion

        #region IEquatable<CBORTag> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CBOR tags for equality.
        /// </summary>
        /// <param name="Object">A CBOR tag to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CBORTag cborTag &&
                   Equals(cborTag);

        #endregion

        #region Equals(CBORTag)

        /// <summary>
        /// Compares two CBOR tags for equality.
        /// </summary>
        /// <param name="CBORTag">A CBOR tag to compare with.</param>
        public Boolean Equals(CBORTag CBORTag)

            => Value == CBORTag.Value;

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

            => Value.ToString();

        #endregion

    }

}
