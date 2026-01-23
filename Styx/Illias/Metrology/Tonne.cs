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
    /// Extension methods for Tonne.
    /// </summary>
    public static class TonneExtensions
    {

        /// <summary>
        /// The sum of the given Tonne values.
        /// </summary>
        /// <param name="Tonnes">An enumeration of Tonne values.</param>
        public static Tonne Sum(this IEnumerable<Tonne> Tonnes)
        {

            var sum = Tonne.Zero;

            foreach (var tonne in Tonnes)
                sum = sum + tonne;

            return sum;

        }

    }


    /// <summary>
    /// A weight in Tonnes.
    /// </summary>
    public readonly struct Tonne : IEquatable <Tonne>,
                                   IComparable<Tonne>,
                                   IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the frequency.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The value of the frequency as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Math.Round(Value);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new frequency based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a frequency.</param>
        private Tonne(Decimal Value)
        {

            this.Value = Value >= 0
                             ? Value
                             : 0;

        }

        #endregion


        #region (static) Parse       (Text)

        /// <summary>
        /// Parse the given string as a Tonne.
        /// </summary>
        /// <param name="Text">A text representation of a Tonne.</param>
        public static Tonne Parse(String Text)
        {

            if (TryParse(Text, out var tonne))
                return tonne;

            throw new ArgumentException($"Invalid text representation of a Tonne: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseT     (Text)

        /// <summary>
        /// Parse the given string as a Tonne.
        /// </summary>
        /// <param name="Text">A text representation of a Tonne.</param>
        public static Tonne ParseT(String Text)
        {

            if (TryParseT(Text, out var tonne))
                return tonne;

            throw new ArgumentException($"Invalid text representation of a Tonne: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKT    (Text)

        /// <summary>
        /// Parse the given string as a kT.
        /// </summary>
        /// <param name="Text">A text representation of a kT.</param>
        public static Tonne ParseKT(String Text)
        {

            if (TryParseKT(Text, out var tonne))
                return tonne;

            throw new ArgumentException($"Invalid text representation of a kT: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseT     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Tonne.
        /// </summary>
        /// <param name="Number">A numeric representation of a Tonne.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne ParseT(Decimal  Number,
                                    Int32?   Exponent = null)
        {

            if (TryParseT(Number, out var tonne, Exponent))
                return tonne;

            throw new ArgumentException($"Invalid numeric representation of a Tonne: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Tonne.
        /// </summary>
        /// <param name="Number">A numeric representation of a Tonne.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne ParseT(Byte    Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseT(Number, out var tonne, Exponent))
                return tonne;

            throw new ArgumentException($"Invalid numeric representation of a Tonne: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKT    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a kT.
        /// </summary>
        /// <param name="Number">A numeric representation of a kT.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne ParseKT(Decimal  Number,
                                     Int32?   Exponent = null)
        {

            if (TryParseKT(Number, out var tonne, Exponent))
                return tonne;

            throw new ArgumentException($"Invalid numeric representation of a kT: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a kT.
        /// </summary>
        /// <param name="Number">A numeric representation of a kT.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne ParseKT(Byte    Number,
                                     Int32?  Exponent = null)
        {

            if (TryParseKT(Number, out var tonne, Exponent))
                return tonne;

            throw new ArgumentException($"Invalid numeric representation of a kT: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse    (Text)

        /// <summary>
        /// Try to parse the given text as a Tonne.
        /// </summary>
        /// <param name="Text">A text representation of a Tonne.</param>
        public static Tonne? TryParse(String Text)
        {

            if (TryParse(Text, out var tonne))
                return tonne;

            return null;

        }

        #endregion

        #region (static) TryParseT  (Text)

        /// <summary>
        /// Try to parse the given text as a Tonne.
        /// </summary>
        /// <param name="Text">A text representation of a Tonne.</param>
        public static Tonne? TryParseT(String Text)
        {

            if (TryParseT(Text, out var tonne))
                return tonne;

            return null;

        }

        #endregion

        #region (static) TryParseKT (Text)

        /// <summary>
        /// Try to parse the given text as a kT.
        /// </summary>
        /// <param name="Text">A text representation of a kT.</param>
        public static Tonne? TryParseKT(String Text)
        {

            if (TryParseKT(Text, out var tonne))
                return tonne;

            return null;

        }

        #endregion


        #region (static) TryParseT  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Tonne.
        /// </summary>
        /// <param name="Number">A numeric representation of a Tonne.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne? TryParseT(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseT(Number, out var tonne, Exponent))
                return tonne;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Tonne.
        /// </summary>
        /// <param name="Number">A numeric representation of a Tonne.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne? TryParseT(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseT(Number, out var tonne, Exponent))
                return tonne;

            return null;

        }

        #endregion

        #region (static) TryParseKT (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a kT.
        /// </summary>
        /// <param name="Number">A numeric representation of a kT.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne? TryParseKT(Decimal  Number,
                                         Int32?   Exponent = null)
        {

            if (TryParseKT(Number, out var tonne, Exponent))
                return tonne;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a kT.
        /// </summary>
        /// <param name="Number">A numeric representation of a kT.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Tonne? TryParseKT(Byte    Number,
                                         Int32?  Exponent = null)
        {

            if (TryParseKT(Number, out var tonne, Exponent))
                return tonne;

            return null;

        }

        #endregion


        #region (static) TryParse    (Text,   out Tonne)

        /// <summary>
        /// Parse the given string as a Tonne.
        /// </summary>
        /// <param name="Text">A text representation of a Tonne.</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        public static Boolean TryParse(String Text, out Tonne Tonne)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("kT") || Text.EndsWith("KT"))
                    factor = 1000;

                if (Decimal.TryParse(Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Tonne = new Tonne(factor * value);

                    return true;

                }

            }
            catch
            { }

            Tonne = default;
            return false;

        }

        #endregion

        #region (static) TryParseT  (Text,   out Tonne)

        /// <summary>
        /// Parse the given string as a Tonne.
        /// </summary>
        /// <param name="Text">A text representation of a Tonne.</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        public static Boolean TryParseT(String Text, out Tonne Tonne)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Tonne = new Tonne(value);

                    return true;

                }

            }
            catch
            { }

            Tonne = default;
            return false;

        }

        #endregion

        #region (static) TryParseKT (Text,   out Tonne)

        /// <summary>
        /// Parse the given string as a kT.
        /// </summary>
        /// <param name="Text">A text representation of a kT.</param>
        /// <param name="Tonne">The parsed kT.</param>
        public static Boolean TryParseKT(String Text, out Tonne Tonne)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Tonne = new Tonne(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Tonne = default;
            return false;

        }

        #endregion


        #region (static) TryParseT  (Number, out Tonne, Exponent = null)

        /// <summary>
        /// Parse the given number as a Tonne.
        /// </summary>
        /// <param name="Number">A numeric representation of a Tonne.</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseT(Byte       Number,
                                         out Tonne  Tonne,
                                         Int32?     Exponent = null)
        {

            try
            {

                Tonne = new Tonne(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Tonne = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Tonne.
        /// </summary>
        /// <param name="Number">A numeric representation of a Tonne.</param>
        /// <param name="Tonne">The parsed Tonne.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseT(Decimal    Number,
                                         out Tonne  Tonne,
                                         Int32?     Exponent = null)
        {

            try
            {

                Tonne = new Tonne(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Tonne = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKT (Number, out Tonne, Exponent = null)

        /// <summary>
        /// Parse the given number as a kT.
        /// </summary>
        /// <param name="Number">A numeric representation of a kT.</param>
        /// <param name="Tonne">The parsed kT.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKT(Byte       Number,
                                          out Tonne  Tonne,
                                          Int32?     Exponent = null)
        {

            try
            {

                Tonne = new Tonne(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Tonne = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a kT.
        /// </summary>
        /// <param name="Number">A numeric representation of a kT.</param>
        /// <param name="Tonne">The parsed kT.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKT(Decimal    Number,
                                          out Tonne  Tonne,
                                          Int32?     Exponent = null)
        {

            try
            {

                Tonne = new Tonne(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Tonne = default;
                return false;
            }

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this Tonne.
        /// </summary>
        public Tonne Clone()

            => new (Value);

        #endregion


        public static Tonne Zero
            => new (0);


        #region Operator overloading

        #region Operator == (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A frequency.</param>
        /// <param name="Tonne2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tonne Tonne1,
                                           Tonne Tonne2)

            => Tonne1.Equals(Tonne2);

        #endregion

        #region Operator != (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A frequency.</param>
        /// <param name="Tonne2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tonne Tonne1,
                                           Tonne Tonne2)

            => !Tonne1.Equals(Tonne2);

        #endregion

        #region Operator <  (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A frequency.</param>
        /// <param name="Tonne2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tonne Tonne1,
                                          Tonne Tonne2)

            => Tonne1.CompareTo(Tonne2) < 0;

        #endregion

        #region Operator <= (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A frequency.</param>
        /// <param name="Tonne2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tonne Tonne1,
                                           Tonne Tonne2)

            => Tonne1.CompareTo(Tonne2) <= 0;

        #endregion

        #region Operator >  (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A frequency.</param>
        /// <param name="Tonne2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tonne Tonne1,
                                          Tonne Tonne2)

            => Tonne1.CompareTo(Tonne2) > 0;

        #endregion

        #region Operator >= (Tonne1, Tonne2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tonne1">A frequency.</param>
        /// <param name="Tonne2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tonne Tonne1,
                                           Tonne Tonne2)

            => Tonne1.CompareTo(Tonne2) >= 0;

        #endregion

        #region Operator +  (Tonne1, Tonne2)

        /// <summary>
        /// Accumulates two frequency.
        /// </summary>
        /// <param name="Tonne1">A frequency.</param>
        /// <param name="Tonne2">Another frequency.</param>
        public static Tonne operator + (Tonne Tonne1,
                                       Tonne Tonne2)

            => new (Tonne1.Value + Tonne2.Value);

        #endregion

        #region Operator -  (Tonne1, Tonne2)

        /// <summary>
        /// Substracts two frequency.
        /// </summary>
        /// <param name="Tonne1">A frequency.</param>
        /// <param name="Tonne2">Another frequency.</param>
        public static Tonne operator - (Tonne Tonne1,
                                       Tonne Tonne2)

            => new (Tonne1.Value - Tonne2.Value);

        #endregion

        #endregion

        #region IComparable<Tonne> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two frequency.
        /// </summary>
        /// <param name="Object">A frequency to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Tonne tonne
                   ? CompareTo(tonne)
                   : throw new ArgumentException("The given object is not a frequency!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Tonne)

        /// <summary>
        /// Compares two frequency.
        /// </summary>
        /// <param name="Tonne">A Tonne to compare with.</param>
        public Int32 CompareTo(Tonne Tonne)

            => Value.CompareTo(Tonne.Value);

        #endregion

        #endregion

        #region IEquatable<Tonne> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two frequency for equality.
        /// </summary>
        /// <param name="Object">A frequency to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Tonne tonne &&
                   Equals(tonne);

        #endregion

        #region Equals(Tonne)

        /// <summary>
        /// Compares two frequency for equality.
        /// </summary>
        /// <param name="Tonne">A frequency to compare with.</param>
        public Boolean Equals(Tonne Tonne)

            => Value.Equals(Tonne.Value);

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

            => $"{Value} T";

        #endregion

    }

}
