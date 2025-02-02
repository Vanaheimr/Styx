/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for Siemens.
    /// </summary>
    public static class SiemensExtensions
    {

        /// <summary>
        /// The sum of the given Siemens values.
        /// </summary>
        /// <param name="Siemens">An enumeration of Siemens values.</param>
        public static Siemens Sum(this IEnumerable<Siemens> Siemens)
        {

            var sum = Illias.Siemens.Zero;

            foreach (var siemens in Siemens)
                sum = sum + siemens;

            return sum;

        }

    }


    /// <summary>
    /// A Siemens value.
    /// </summary>
    public readonly struct Siemens : IEquatable <Siemens>,
                                     IComparable<Siemens>,
                                     IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the Siemens.
        /// </summary>
        public Decimal  Value           { get; }

        /// <summary>
        /// The value of the Siemens as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Value;


        /// <summary>
        /// The value as Kilo-Siemens.
        /// </summary>
        public Decimal  KS
            => Value / 1000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Siemens based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a Siemens.</param>
        private Siemens(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as a Siemens.
        /// </summary>
        /// <param name="Text">A text representation of a Siemens.</param>
        public static Siemens Parse(String Text)
        {

            if (TryParse(Text, out var siemens))
                return siemens;

            throw new ArgumentException($"Invalid text representation of a Siemens: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseS     (Text)

        /// <summary>
        /// Parse the given string as a Siemens.
        /// </summary>
        /// <param name="Text">A text representation of a Siemens.</param>
        public static Siemens ParseS(String Text)
        {

            if (TryParseS(Text, out var siemens))
                return siemens;

            throw new ArgumentException($"Invalid text representation of a Siemens: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKS    (Text)

        /// <summary>
        /// Parse the given string as a Kilo-Siemens (kS).
        /// </summary>
        /// <param name="Text">A text representation of a Kilo-Siemens (kS).</param>
        public static Siemens ParseKS(String Text)
        {

            if (TryParseKS(Text, out var siemens))
                return siemens;

            throw new ArgumentException($"Invalid text representation of a Kilo-Siemens (kS): '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseS     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Siemens.
        /// </summary>
        /// <param name="Number">A numeric representation of a Siemens.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens ParseS(Decimal  Number,
                                     Int32?   Exponent = null)
        {

            if (TryParseS(Number, out var siemens, Exponent))
                return siemens;

            throw new ArgumentException($"Invalid numeric representation of a Siemens: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Siemens.
        /// </summary>
        /// <param name="Number">A numeric representation of a Siemens.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens ParseS(Byte    Number,
                                     Int32?  Exponent = null)
        {

            if (TryParseS(Number, out var siemens, Exponent))
                return siemens;

            throw new ArgumentException($"Invalid numeric representation of a Siemens: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKS    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Kilo-Siemens (kS).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Siemens (kS).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens ParseKS(Decimal  Number,
                                      Int32?   Exponent = null)
        {

            if (TryParseKS(Number, out var siemens, Exponent))
                return siemens;

            throw new ArgumentException($"Invalid numeric representation of a Kilo-Siemens (kS): '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Kilo-Siemens (kS).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Siemens (kS).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens ParseKS(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryParseKS(Number, out var siemens, Exponent))
                return siemens;

            throw new ArgumentException($"Invalid numeric representation of a Kilo-Siemens (kS): '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as a Siemens.
        /// </summary>
        /// <param name="Text">A text representation of a Siemens.</param>
        public static Siemens? TryParse(String Text)
        {

            if (TryParse(Text, out var siemens))
                return siemens;

            return null;

        }

        #endregion

        #region (static) TryParseS  (Text)

        /// <summary>
        /// Try to parse the given text as a Siemens.
        /// </summary>
        /// <param name="Text">A text representation of a Siemens.</param>
        public static Siemens? TryParseS(String Text)
        {

            if (TryParseS(Text, out var siemens))
                return siemens;

            return null;

        }

        #endregion

        #region (static) TryParseKS (Text)

        /// <summary>
        /// Try to parse the given text as a Kilo-Siemens (kS).
        /// </summary>
        /// <param name="Text">A text representation of a Kilo-Siemens (kS).</param>
        public static Siemens? TryParseKS(String Text)
        {

            if (TryParseKS(Text, out var siemens))
                return siemens;

            return null;

        }

        #endregion


        #region (static) TryParseS  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Siemens.
        /// </summary>
        /// <param name="Number">A numeric representation of a Siemens.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens? TryParseS(Decimal  Number,
                                         Int32?   Exponent = null)
        {

            if (TryParseS(Number, out var siemens, Exponent))
                return siemens;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Siemens.
        /// </summary>
        /// <param name="Number">A numeric representation of a Siemens.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens? TryParseS(Byte    Number,
                                         Int32?  Exponent = null)
        {

            if (TryParseS(Number, out var siemens, Exponent))
                return siemens;

            return null;

        }

        #endregion

        #region (static) TryParseKS (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Kilo-Siemens (kS).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Siemens (kS).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens? TryParseKS(Decimal  Number,
                                          Int32?   Exponent = null)
        {

            if (TryParseKS(Number, out var siemens, Exponent))
                return siemens;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Kilo-Siemens (kS).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Siemens (kS).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Siemens? TryParseKS(Byte    Number,
                                          Int32?  Exponent = null)
        {

            if (TryParseKS(Number, out var siemens, Exponent))
                return siemens;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,   out Siemens)

        /// <summary>
        /// Parse the given string as a Siemens.
        /// </summary>
        /// <param name="Text">A text representation of a Siemens.</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        public static Boolean TryParse(String Text, out Siemens Siemens)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("kS") || Text.EndsWith("KS"))
                    factor = 1000;

                if (Decimal.TryParse(Text, out var value))
                {

                    Siemens = new Siemens(value / factor);

                    return true;

                }

            }
            catch
            { }

            Siemens = default;
            return false;

        }

        #endregion

        #region (static) TryParseS  (Text,   out Siemens)

        /// <summary>
        /// Parse the given string as a Siemens.
        /// </summary>
        /// <param name="Text">A text representation of a Siemens.</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        public static Boolean TryParseS(String Text, out Siemens Siemens)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Siemens = new Siemens(value);

                    return true;

                }

            }
            catch
            { }

            Siemens = default;
            return false;

        }

        #endregion

        #region (static) TryParseKS (Text,   out Siemens)

        /// <summary>
        /// Parse the given string as a Kilo-Siemens (kS).
        /// </summary>
        /// <param name="Text">A text representation of a Kilo-Siemens (kS).</param>
        /// <param name="Siemens">The parsed Kilo-Siemens (kS).</param>
        public static Boolean TryParseKS(String Text, out Siemens Siemens)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Siemens = new Siemens(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Siemens = default;
            return false;

        }

        #endregion


        #region (static) TryParseS  (Number, out Siemens, Exponent = null)

        /// <summary>
        /// Parse the given number as a Siemens.
        /// </summary>
        /// <param name="Number">A numeric representation of a Siemens.</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseS(Byte         Number,
                                        out Siemens  Siemens,
                                        Int32?       Exponent = null)
        {

            try
            {

                Siemens = new Siemens(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Siemens = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Siemens.
        /// </summary>
        /// <param name="Number">A numeric representation of a Siemens.</param>
        /// <param name="Siemens">The parsed Siemens.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseS(Decimal      Number,
                                        out Siemens  Siemens,
                                        Int32?       Exponent = null)
        {

            try
            {

                Siemens = new Siemens(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Siemens = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKS (Number, out Siemens, Exponent = null)

        /// <summary>
        /// Parse the given number as a Kilo-Siemens (kS).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Siemens (kS).</param>
        /// <param name="Siemens">The parsed Kilo-Siemens (kS).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKS(Byte         Number,
                                         out Siemens  Siemens,
                                         Int32?       Exponent = null)
        {

            try
            {

                Siemens = new Siemens(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Siemens = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Kilo-Siemens (kS).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Siemens (kS).</param>
        /// <param name="Siemens">The parsed Kilo-Siemens (kS).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKS(Decimal      Number,
                                         out Siemens  Siemens,
                                         Int32?       Exponent = null)
        {

            try
            {

                Siemens = new Siemens(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Siemens = default;
                return false;
            }

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this Siemens.
        /// </summary>
        public Siemens Clone()

            => new (Value);

        #endregion


        public static Siemens Zero
            => new (0);


        #region Operator overloading

        #region Operator == (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens.</param>
        /// <param name="Siemens2">Another Siemens.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Siemens Siemens1,
                                           Siemens Siemens2)

            => Siemens1.Equals(Siemens2);

        #endregion

        #region Operator != (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens.</param>
        /// <param name="Siemens2">Another Siemens.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Siemens Siemens1,
                                           Siemens Siemens2)

            => !Siemens1.Equals(Siemens2);

        #endregion

        #region Operator <  (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens.</param>
        /// <param name="Siemens2">Another Siemens.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Siemens Siemens1,
                                          Siemens Siemens2)

            => Siemens1.CompareTo(Siemens2) < 0;

        #endregion

        #region Operator <= (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens.</param>
        /// <param name="Siemens2">Another Siemens.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Siemens Siemens1,
                                           Siemens Siemens2)

            => Siemens1.CompareTo(Siemens2) <= 0;

        #endregion

        #region Operator >  (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens.</param>
        /// <param name="Siemens2">Another Siemens.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Siemens Siemens1,
                                          Siemens Siemens2)

            => Siemens1.CompareTo(Siemens2) > 0;

        #endregion

        #region Operator >= (Siemens1, Siemens2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Siemens1">A Siemens.</param>
        /// <param name="Siemens2">Another Siemens.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Siemens Siemens1,
                                           Siemens Siemens2)

            => Siemens1.CompareTo(Siemens2) >= 0;

        #endregion

        #region Operator +  (Siemens1, Siemens2)

        /// <summary>
        /// Accumulates two Siemens.
        /// </summary>
        /// <param name="Siemens1">A Siemens.</param>
        /// <param name="Siemens2">Another Siemens.</param>
        public static Siemens operator + (Siemens Siemens1,
                                       Siemens Siemens2)

            => new (Siemens1.Value + Siemens2.Value);

        #endregion

        #region Operator -  (Siemens1, Siemens2)

        /// <summary>
        /// Substracts two Siemens.
        /// </summary>
        /// <param name="Siemens1">A Siemens.</param>
        /// <param name="Siemens2">Another Siemens.</param>
        public static Siemens operator - (Siemens Siemens1,
                                       Siemens Siemens2)

            => new (Siemens1.Value - Siemens2.Value);

        #endregion

        #endregion

        #region IComparable<Siemens> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Siemens.
        /// </summary>
        /// <param name="Object">A Siemens to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Siemens siemens
                   ? CompareTo(siemens)
                   : throw new ArgumentException("The given object is not a Siemens!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Siemens)

        /// <summary>
        /// Compares two Siemens.
        /// </summary>
        /// <param name="Siemens">A Siemens to compare with.</param>
        public Int32 CompareTo(Siemens Siemens)

            => Value.CompareTo(Siemens.Value);

        #endregion

        #endregion

        #region IEquatable<Siemens> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Siemens for equality.
        /// </summary>
        /// <param name="Object">A Siemens to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Siemens siemens &&
                   Equals(siemens);

        #endregion

        #region Equals(Siemens)

        /// <summary>
        /// Compares two Siemens for equality.
        /// </summary>
        /// <param name="Siemens">A Siemens to compare with.</param>
        public Boolean Equals(Siemens Siemens)

            => Value.Equals(Siemens.Value);

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

            => $"{Value} V";

        #endregion

    }

}
