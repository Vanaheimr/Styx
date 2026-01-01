/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for Volt-Amperes (VAs).
    /// </summary>
    public static class VoltampereExtensions
    {

        /// <summary>
        /// The sum of the given Volt-Ampere values.
        /// </summary>
        /// <param name="Voltamperes">An enumeration of Volt-Ampere values.</param>
        public static VoltAmpere Sum(this IEnumerable<VoltAmpere> Voltamperes)
        {

            var sum = VoltAmpere.Zero;

            foreach (var voltAmpere in Voltamperes)
                sum = sum + voltAmpere;

            return sum;

        }

    }


    /// <summary>
    /// A Volt-Ampere (VA) value.
    /// </summary>
    public readonly struct VoltAmpere : IEquatable <VoltAmpere>,
                                        IComparable<VoltAmpere>,
                                        IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the Volt-Ampere.
        /// </summary>
        public Decimal  Value           { get; }

        /// <summary>
        /// The value of the Volt-Ampere as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Value;


        /// <summary>
        /// The value as Kilo-Volt-Ampere.
        /// </summary>
        public Decimal  KW
            => Value / 1000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Volt-Ampere (VA) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a Volt-Ampere (VA).</param>
        private VoltAmpere(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse       (Text)

        /// <summary>
        /// Parse the given string as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere (VA).</param>
        public static VoltAmpere Parse(String Text)
        {

            if (TryParse(Text, out var voltAmpere))
                return voltAmpere;

            throw new ArgumentException($"Invalid text representation of a Volt-Ampere (VA): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseVA     (Text)

        /// <summary>
        /// Parse the given string as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere (VA).</param>
        public static VoltAmpere ParseVA(String Text)
        {

            if (TryParseVA(Text, out var voltAmpere))
                return voltAmpere;

            throw new ArgumentException($"Invalid text representation of a Volt-Ampere (VA): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKVA    (Text)

        /// <summary>
        /// Parse the given string as a kVA.
        /// </summary>
        /// <param name="Text">A text representation of a kVA.</param>
        public static VoltAmpere ParseKVA(String Text)
        {

            if (TryParseKVA(Text, out var voltAmpere))
                return voltAmpere;

            throw new ArgumentException($"Invalid text representation of a Kilo-Volt-Ampere (kVA): '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseVA     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere (VA).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpere ParseVA(Decimal  Number,
                                         Int32?   Exponent = null)
        {

            if (TryParseVA(Number, out var voltAmpere, Exponent))
                return voltAmpere;

            throw new ArgumentException($"Invalid numeric representation of a Volt-Ampere (VA): '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere (VA).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpere ParseVA(Byte    Number,
                                         Int32?  Exponent = null)
        {

            if (TryParseVA(Number, out var voltAmpere, Exponent))
                return voltAmpere;

            throw new ArgumentException($"Invalid numeric representation of a Volt-Ampere (VA): '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKVA    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a kVA.
        /// </summary>
        /// <param name="Number">A numeric representation of a kVA.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpere ParseKVA(Decimal  Number,
                                          Int32?   Exponent = null)
        {

            if (TryParseKVA(Number, out var voltAmpere, Exponent))
                return voltAmpere;

            throw new ArgumentException($"Invalid numeric representation of a kVA: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a kVA.
        /// </summary>
        /// <param name="Number">A numeric representation of a kVA.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpere ParseKVA(Byte    Number,
                                          Int32?  Exponent = null)
        {

            if (TryParseKVA(Number, out var voltAmpere, Exponent))
                return voltAmpere;

            throw new ArgumentException($"Invalid numeric representation of a kVA: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse    (Text)

        /// <summary>
        /// Try to parse the given text as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere (VA).</param>
        public static VoltAmpere? TryParse(String Text)
        {

            if (TryParse(Text, out var voltAmpere))
                return voltAmpere;

            return null;

        }

        #endregion

        #region (static) TryParseVA  (Text)

        /// <summary>
        /// Try to parse the given text as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere (VA).</param>
        public static VoltAmpere? TryParseVA(String Text)
        {

            if (TryParseVA(Text, out var voltAmpere))
                return voltAmpere;

            return null;

        }

        #endregion

        #region (static) TryParseKVA (Text)

        /// <summary>
        /// Try to parse the given text as a kVA.
        /// </summary>
        /// <param name="Text">A text representation of a kVA.</param>
        public static VoltAmpere? TryParseKVA(String Text)
        {

            if (TryParseKVA(Text, out var voltAmpere))
                return voltAmpere;

            return null;

        }

        #endregion


        #region (static) TryParseVA  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere (VA).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpere? TryParseVA(Decimal  Number,
                                             Int32?   Exponent = null)
        {

            if (TryParseVA(Number, out var voltAmpere, Exponent))
                return voltAmpere;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere (VA).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpere? TryParseVA(Byte    Number,
                                             Int32?  Exponent = null)
        {

            if (TryParseVA(Number, out var voltAmpere, Exponent))
                return voltAmpere;

            return null;

        }

        #endregion

        #region (static) TryParseKVA (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a kVA.
        /// </summary>
        /// <param name="Number">A numeric representation of a kVA.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpere? TryParseKVA(Decimal  Number,
                                              Int32?   Exponent = null)
        {

            if (TryParseKVA(Number, out var voltAmpere, Exponent))
                return voltAmpere;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a kVA.
        /// </summary>
        /// <param name="Number">A numeric representation of a kVA.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpere? TryParseKVA(Byte    Number,
                                              Int32?  Exponent = null)
        {

            if (TryParseKVA(Number, out var voltAmpere, Exponent))
                return voltAmpere;

            return null;

        }

        #endregion


        #region (static) TryParse    (Text,   out VoltAmpere)

        /// <summary>
        /// Parse the given string as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere (VA).</param>
        /// <param name="VoltAmpere">The parsed Volt-Ampere.</param>
        public static Boolean TryParse(String Text, out VoltAmpere VoltAmpere)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("kV") || Text.EndsWith("KV"))
                    factor = 1000;

                if (Decimal.TryParse(Text, out var value))
                {

                    VoltAmpere = new VoltAmpere(value / factor);

                    return true;

                }

            }
            catch
            { }

            VoltAmpere = default;
            return false;

        }

        #endregion

        #region (static) TryParseVA  (Text,   out VoltAmpere)

        /// <summary>
        /// Parse the given string as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere (VA).</param>
        /// <param name="VoltAmpere">The parsed Volt-Ampere.</param>
        public static Boolean TryParseVA(String Text, out VoltAmpere VoltAmpere)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    VoltAmpere = new VoltAmpere(value);

                    return true;

                }

            }
            catch
            { }

            VoltAmpere = default;
            return false;

        }

        #endregion

        #region (static) TryParseKVA (Text,   out VoltAmpere)

        /// <summary>
        /// Parse the given string as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere (VA).</param>
        /// <param name="VoltAmpere">The parsed Volt-Ampere.</param>
        public static Boolean TryParseKVA(String Text, out VoltAmpere VoltAmpere)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    VoltAmpere = new VoltAmpere(1000 * value);

                    return true;

                }

            }
            catch
            { }

            VoltAmpere = default;
            return false;

        }

        #endregion


        #region (static) TryParseVA  (Number, out VoltAmpere, Exponent = null)

        /// <summary>
        /// Parse the given number as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere (VA).</param>
        /// <param name="VoltAmpere">The parsed Volt-Ampere.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseVA(Byte            Number,
                                         out VoltAmpere  VoltAmpere,
                                         Int32?          Exponent = null)
        {

            try
            {

                VoltAmpere = new VoltAmpere(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                VoltAmpere = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere (VA).</param>
        /// <param name="VoltAmpere">The parsed Volt-Ampere.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseVA(Decimal         Number,
                                         out VoltAmpere  VoltAmpere,
                                         Int32?          Exponent = null)
        {

            try
            {

                VoltAmpere = new VoltAmpere(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                VoltAmpere = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKVA (Number, out VoltAmpere, Exponent = null)

        /// <summary>
        /// Parse the given number as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere (VA).</param>
        /// <param name="VoltAmpere">The parsed Volt-Ampere.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKVA(Byte            Number,
                                          out VoltAmpere  VoltAmpere,
                                          Int32?          Exponent = null)
        {

            try
            {

                VoltAmpere = new VoltAmpere(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                VoltAmpere = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Volt-Ampere (VA).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere (VA).</param>
        /// <param name="Voltampere">The parsed Volt-Ampere.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKVA(Decimal         Number,
                                          out VoltAmpere  Voltampere,
                                          Int32?          Exponent = null)
        {

            try
            {

                Voltampere = new VoltAmpere(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Voltampere = default;
                return false;
            }

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this Voltampere.
        /// </summary>
        public VoltAmpere Clone()

            => new (Value);

        #endregion


        public static VoltAmpere Zero
            => new (0);


        #region Operator overloading

        #region Operator == (Voltampere1, Voltampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Voltampere1">A Volt-Ampere (VA).</param>
        /// <param name="Voltampere2">Another Volt-Ampere (VA).</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VoltAmpere Voltampere1,
                                           VoltAmpere Voltampere2)

            => Voltampere1.Equals(Voltampere2);

        #endregion

        #region Operator != (Voltampere1, Voltampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Voltampere1">A Volt-Ampere (VA).</param>
        /// <param name="Voltampere2">Another Volt-Ampere (VA).</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VoltAmpere Voltampere1,
                                           VoltAmpere Voltampere2)

            => !Voltampere1.Equals(Voltampere2);

        #endregion

        #region Operator <  (Voltampere1, Voltampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Voltampere1">A Volt-Ampere (VA).</param>
        /// <param name="Voltampere2">Another Volt-Ampere (VA).</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (VoltAmpere Voltampere1,
                                          VoltAmpere Voltampere2)

            => Voltampere1.CompareTo(Voltampere2) < 0;

        #endregion

        #region Operator <= (Voltampere1, Voltampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Voltampere1">A Volt-Ampere (VA).</param>
        /// <param name="Voltampere2">Another Volt-Ampere (VA).</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (VoltAmpere Voltampere1,
                                           VoltAmpere Voltampere2)

            => Voltampere1.CompareTo(Voltampere2) <= 0;

        #endregion

        #region Operator >  (Voltampere1, Voltampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Voltampere1">A Volt-Ampere (VA).</param>
        /// <param name="Voltampere2">Another Volt-Ampere (VA).</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (VoltAmpere Voltampere1,
                                          VoltAmpere Voltampere2)

            => Voltampere1.CompareTo(Voltampere2) > 0;

        #endregion

        #region Operator >= (Voltampere1, Voltampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Voltampere1">A Volt-Ampere (VA).</param>
        /// <param name="Voltampere2">Another Volt-Ampere (VA).</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (VoltAmpere Voltampere1,
                                           VoltAmpere Voltampere2)

            => Voltampere1.CompareTo(Voltampere2) >= 0;

        #endregion

        #region Operator +  (Voltampere1, Voltampere2)

        /// <summary>
        /// Accumulates two Volt-Amperes (VAs).
        /// </summary>
        /// <param name="Voltampere1">A Volt-Ampere (VA).</param>
        /// <param name="Voltampere2">Another Volt-Ampere (VA).</param>
        public static VoltAmpere operator + (VoltAmpere Voltampere1,
                                       VoltAmpere Voltampere2)

            => new (Voltampere1.Value + Voltampere2.Value);

        #endregion

        #region Operator -  (Voltampere1, Voltampere2)

        /// <summary>
        /// Substracts two Volt-Amperes (VAs).
        /// </summary>
        /// <param name="Voltampere1">A Volt-Ampere (VA).</param>
        /// <param name="Voltampere2">Another Volt-Ampere (VA).</param>
        public static VoltAmpere operator - (VoltAmpere Voltampere1,
                                       VoltAmpere Voltampere2)

            => new (Voltampere1.Value - Voltampere2.Value);

        #endregion

        #endregion

        #region IComparable<Voltampere> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Volt-Amperes (VAs).
        /// </summary>
        /// <param name="Object">A Volt-Ampere (VA) to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is VoltAmpere voltAmpere
                   ? CompareTo(voltAmpere)
                   : throw new ArgumentException("The given object is not a Volt-Ampere (VA)!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Voltampere)

        /// <summary>
        /// Compares two Volt-Amperes (VAs).
        /// </summary>
        /// <param name="Voltampere">A Volt-Ampere (VA) to compare with.</param>
        public Int32 CompareTo(VoltAmpere Voltampere)

            => Value.CompareTo(Voltampere.Value);

        #endregion

        #endregion

        #region IEquatable<Voltampere> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Volt-Amperes (VAs) for equality.
        /// </summary>
        /// <param name="Object">A Volt-Ampere (VA) to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VoltAmpere voltAmpere &&
                   Equals(voltAmpere);

        #endregion

        #region Equals(Voltampere)

        /// <summary>
        /// Compares two Volt-Amperes (VAs) for equality.
        /// </summary>
        /// <param name="Voltampere">A Volt-Ampere (VA) to compare with.</param>
        public Boolean Equals(VoltAmpere Voltampere)

            => Value.Equals(Voltampere.Value);

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

            => $"{Value} VA";

        #endregion

    }

}
