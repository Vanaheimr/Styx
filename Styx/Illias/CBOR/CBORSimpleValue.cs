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
    /// A CBOR simple value (RFC 8949, Section 3.3).
    /// Valid simple values are 0..23 and 32..255; the values 24..31 are
    /// reserved by the CBOR specification and can not be represented.
    /// </summary>
    public readonly struct CBORSimpleValue : IEquatable <CBORSimpleValue>,
                                             IComparable<CBORSimpleValue>,
                                             IComparable
    {

        #region Properties

        /// <summary>
        /// The numeric value of this CBOR simple value.
        /// </summary>
        public Byte  Value    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CBOR simple value based on the given number.
        /// </summary>
        /// <param name="Value">The numeric value of the CBOR simple value.</param>
        private CBORSimpleValue(Byte Value)
        {
            this.Value = Value;
        }

        #endregion


        #region Static defaults

        /// <summary>
        /// The CBOR simple value "false" (20).
        /// </summary>
        public static CBORSimpleValue  False        { get; } = new (20);

        /// <summary>
        /// The CBOR simple value "true" (21).
        /// </summary>
        public static CBORSimpleValue  True         { get; } = new (21);

        /// <summary>
        /// The CBOR simple value "null" (22).
        /// </summary>
        public static CBORSimpleValue  Null         { get; } = new (22);

        /// <summary>
        /// The CBOR simple value "undefined" (23).
        /// </summary>
        public static CBORSimpleValue  Undefined    { get; } = new (23);

        #endregion


        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a CBOR simple value.
        /// </summary>
        /// <param name="Number">A numeric representation of a CBOR simple value.</param>
        public static CBORSimpleValue Parse(Byte Number)
        {

            if (TryParse(Number, out var cborSimpleValue))
                return cborSimpleValue;

            throw new ArgumentException($"Invalid numeric representation of a CBOR simple value: '{Number}'! The values 24..31 are reserved.",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a CBOR simple value.
        /// </summary>
        /// <param name="Number">A numeric representation of a CBOR simple value.</param>
        public static CBORSimpleValue? TryParse(Byte Number)
        {

            if (TryParse(Number, out var cborSimpleValue))
                return cborSimpleValue;

            return null;

        }

        #endregion

        #region (static) TryParse(Number, out CBORSimpleValue)

        /// <summary>
        /// Try to parse the given number as a CBOR simple value.
        /// </summary>
        /// <param name="Number">A numeric representation of a CBOR simple value.</param>
        /// <param name="CBORSimpleValue">The parsed CBOR simple value.</param>
        public static Boolean TryParse(Byte Number, out CBORSimpleValue CBORSimpleValue)
        {

            if (Number >= 24 && Number <= 31)
            {
                CBORSimpleValue = default;
                return false;
            }

            CBORSimpleValue = new CBORSimpleValue(Number);
            return true;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CBORSimpleValue1, CBORSimpleValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORSimpleValue1">A CBOR simple value.</param>
        /// <param name="CBORSimpleValue2">Another CBOR simple value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CBORSimpleValue CBORSimpleValue1,
                                           CBORSimpleValue CBORSimpleValue2)

            => CBORSimpleValue1.Equals(CBORSimpleValue2);

        #endregion

        #region Operator != (CBORSimpleValue1, CBORSimpleValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORSimpleValue1">A CBOR simple value.</param>
        /// <param name="CBORSimpleValue2">Another CBOR simple value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CBORSimpleValue CBORSimpleValue1,
                                           CBORSimpleValue CBORSimpleValue2)

            => !CBORSimpleValue1.Equals(CBORSimpleValue2);

        #endregion

        #region Operator <  (CBORSimpleValue1, CBORSimpleValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORSimpleValue1">A CBOR simple value.</param>
        /// <param name="CBORSimpleValue2">Another CBOR simple value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CBORSimpleValue CBORSimpleValue1,
                                          CBORSimpleValue CBORSimpleValue2)

            => CBORSimpleValue1.CompareTo(CBORSimpleValue2) < 0;

        #endregion

        #region Operator <= (CBORSimpleValue1, CBORSimpleValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORSimpleValue1">A CBOR simple value.</param>
        /// <param name="CBORSimpleValue2">Another CBOR simple value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CBORSimpleValue CBORSimpleValue1,
                                           CBORSimpleValue CBORSimpleValue2)

            => CBORSimpleValue1.CompareTo(CBORSimpleValue2) <= 0;

        #endregion

        #region Operator >  (CBORSimpleValue1, CBORSimpleValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORSimpleValue1">A CBOR simple value.</param>
        /// <param name="CBORSimpleValue2">Another CBOR simple value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CBORSimpleValue CBORSimpleValue1,
                                          CBORSimpleValue CBORSimpleValue2)

            => CBORSimpleValue1.CompareTo(CBORSimpleValue2) > 0;

        #endregion

        #region Operator >= (CBORSimpleValue1, CBORSimpleValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CBORSimpleValue1">A CBOR simple value.</param>
        /// <param name="CBORSimpleValue2">Another CBOR simple value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CBORSimpleValue CBORSimpleValue1,
                                           CBORSimpleValue CBORSimpleValue2)

            => CBORSimpleValue1.CompareTo(CBORSimpleValue2) >= 0;

        #endregion

        #endregion

        #region IComparable<CBORSimpleValue> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two CBOR simple values.
        /// </summary>
        /// <param name="Object">A CBOR simple value to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null                                 => 1,
                   CBORSimpleValue cborSimpleValue      => CompareTo(cborSimpleValue),
                   _                                    => throw new ArgumentException("The given object is not a CBOR simple value!", nameof(Object))
               };

        #endregion

        #region CompareTo(CBORSimpleValue)

        /// <summary>
        /// Compares two CBOR simple values.
        /// </summary>
        /// <param name="CBORSimpleValue">A CBOR simple value to compare with.</param>
        public Int32 CompareTo(CBORSimpleValue CBORSimpleValue)

            => Value.CompareTo(CBORSimpleValue.Value);

        #endregion

        #endregion

        #region IEquatable<CBORSimpleValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CBOR simple values for equality.
        /// </summary>
        /// <param name="Object">A CBOR simple value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CBORSimpleValue cborSimpleValue &&
                   Equals(cborSimpleValue);

        #endregion

        #region Equals(CBORSimpleValue)

        /// <summary>
        /// Compares two CBOR simple values for equality.
        /// </summary>
        /// <param name="CBORSimpleValue">A CBOR simple value to compare with.</param>
        public Boolean Equals(CBORSimpleValue CBORSimpleValue)

            => Value == CBORSimpleValue.Value;

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
        /// Return a text representation of this object,
        /// using the RFC 8949 diagnostic notation.
        /// </summary>
        public override String ToString()

            => Value switch {
                   20  => "false",
                   21  => "true",
                   22  => "null",
                   23  => "undefined",
                   _   => $"simple({Value})"
               };

        #endregion

    }

}
