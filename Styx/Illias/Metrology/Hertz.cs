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
    /// Extension methods for Hertz.
    /// </summary>
    public static class HertzExtensions
    {

        /// <summary>
        /// The sum of the given Hertz values.
        /// </summary>
        /// <param name="Hertzs">An enumeration of Hertz values.</param>
        public static Hertz Sum(this IEnumerable<Hertz> Hertzs)
        {

            var sum = Hertz.Zero;

            foreach (var hertz in Hertzs)
                sum = sum + hertz;

            return sum;

        }

    }


    /// <summary>
    /// A frequency in Hertz.
    /// </summary>
    public readonly struct Hertz : IEquatable <Hertz>,
                                   IComparable<Hertz>,
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
        private Hertz(Decimal Value)
        {

            this.Value = Value >= 0
                             ? Value
                             : 0;

        }

        #endregion


        #region (static) Parse       (Text)

        /// <summary>
        /// Parse the given string as a Hertz.
        /// </summary>
        /// <param name="Text">A text representation of a Hertz.</param>
        public static Hertz Parse(String Text)
        {

            if (TryParse(Text, out var hertz))
                return hertz;

            throw new ArgumentException($"Invalid text representation of a Hertz: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseHz     (Text)

        /// <summary>
        /// Parse the given string as a Hertz.
        /// </summary>
        /// <param name="Text">A text representation of a Hertz.</param>
        public static Hertz ParseHz(String Text)
        {

            if (TryParseHz(Text, out var hertz))
                return hertz;

            throw new ArgumentException($"Invalid text representation of a Hertz: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKHz    (Text)

        /// <summary>
        /// Parse the given string as a kHz.
        /// </summary>
        /// <param name="Text">A text representation of a kHz.</param>
        public static Hertz ParseKHz(String Text)
        {

            if (TryParseKHz(Text, out var hertz))
                return hertz;

            throw new ArgumentException($"Invalid text representation of a kHz: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseHz     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of a Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz ParseHz(Decimal  Number,
                                    Int32?   Exponent = null)
        {

            if (TryParseHz(Number, out var hertz, Exponent))
                return hertz;

            throw new ArgumentException($"Invalid numeric representation of a Hertz: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of a Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz ParseHz(Byte    Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseHz(Number, out var hertz, Exponent))
                return hertz;

            throw new ArgumentException($"Invalid numeric representation of a Hertz: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKHz    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of a kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz ParseKHz(Decimal  Number,
                                     Int32?   Exponent = null)
        {

            if (TryParseKHz(Number, out var hertz, Exponent))
                return hertz;

            throw new ArgumentException($"Invalid numeric representation of a kHz: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of a kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz ParseKHz(Byte    Number,
                                     Int32?  Exponent = null)
        {

            if (TryParseKHz(Number, out var hertz, Exponent))
                return hertz;

            throw new ArgumentException($"Invalid numeric representation of a kHz: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse    (Text)

        /// <summary>
        /// Try to parse the given text as a Hertz.
        /// </summary>
        /// <param name="Text">A text representation of a Hertz.</param>
        public static Hertz? TryParse(String Text)
        {

            if (TryParse(Text, out var hertz))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParseHz  (Text)

        /// <summary>
        /// Try to parse the given text as a Hertz.
        /// </summary>
        /// <param name="Text">A text representation of a Hertz.</param>
        public static Hertz? TryParseHz(String Text)
        {

            if (TryParseHz(Text, out var hertz))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParseKHz (Text)

        /// <summary>
        /// Try to parse the given text as a kHz.
        /// </summary>
        /// <param name="Text">A text representation of a kHz.</param>
        public static Hertz? TryParseKHz(String Text)
        {

            if (TryParseKHz(Text, out var hertz))
                return hertz;

            return null;

        }

        #endregion


        #region (static) TryParseHz  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of a Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryParseHz(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of a Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryParseHz(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParseKHz (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of a kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryParseKHz(Decimal  Number,
                                         Int32?   Exponent = null)
        {

            if (TryParseKHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of a kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Hertz? TryParseKHz(Byte    Number,
                                         Int32?  Exponent = null)
        {

            if (TryParseKHz(Number, out var hertz, Exponent))
                return hertz;

            return null;

        }

        #endregion


        #region (static) TryParse    (Text,   out Hertz)

        /// <summary>
        /// Parse the given string as a Hertz.
        /// </summary>
        /// <param name="Text">A text representation of a Hertz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParse(String Text, out Hertz Hertz)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("kHz") || Text.EndsWith("KHz"))
                    factor = 1000;

                if (Decimal.TryParse(Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Hertz = new Hertz(factor * value);

                    return true;

                }

            }
            catch
            { }

            Hertz = default;
            return false;

        }

        #endregion

        #region (static) TryParseHz  (Text,   out Hertz)

        /// <summary>
        /// Parse the given string as a Hertz.
        /// </summary>
        /// <param name="Text">A text representation of a Hertz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParseHz(String Text, out Hertz Hertz)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Hertz = new Hertz(value);

                    return true;

                }

            }
            catch
            { }

            Hertz = default;
            return false;

        }

        #endregion

        #region (static) TryParseKHz (Text,   out Hertz)

        /// <summary>
        /// Parse the given string as a kHz.
        /// </summary>
        /// <param name="Text">A text representation of a kHz.</param>
        /// <param name="Hertz">The parsed kHz.</param>
        public static Boolean TryParseKHz(String Text, out Hertz Hertz)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Hertz = new Hertz(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Hertz = default;
            return false;

        }

        #endregion


        #region (static) TryParseHz  (Number, out Hertz, Exponent = null)

        /// <summary>
        /// Parse the given number as a Hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of a Hertz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseHz(Byte       Number,
                                         out Hertz  Hertz,
                                         Int32?     Exponent = null)
        {

            try
            {

                Hertz = new Hertz(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Hertz = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Hertz.
        /// </summary>
        /// <param name="Number">A numeric representation of a Hertz.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseHz(Decimal    Number,
                                         out Hertz  Hertz,
                                         Int32?     Exponent = null)
        {

            try
            {

                Hertz = new Hertz(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Hertz = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKHz (Number, out Hertz, Exponent = null)

        /// <summary>
        /// Parse the given number as a kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of a kHz.</param>
        /// <param name="Hertz">The parsed kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKHz(Byte       Number,
                                          out Hertz  Hertz,
                                          Int32?     Exponent = null)
        {

            try
            {

                Hertz = new Hertz(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Hertz = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a kHz.
        /// </summary>
        /// <param name="Number">A numeric representation of a kHz.</param>
        /// <param name="Hertz">The parsed kHz.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKHz(Decimal    Number,
                                          out Hertz  Hertz,
                                          Int32?     Exponent = null)
        {

            try
            {

                Hertz = new Hertz(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Hertz = default;
                return false;
            }

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this Hertz.
        /// </summary>
        public Hertz Clone()

            => new (Value);

        #endregion


        public static Hertz Zero
            => new (0);


        #region Operator overloading

        #region Operator == (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A frequency.</param>
        /// <param name="Hertz2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Hertz Hertz1,
                                           Hertz Hertz2)

            => Hertz1.Equals(Hertz2);

        #endregion

        #region Operator != (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A frequency.</param>
        /// <param name="Hertz2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Hertz Hertz1,
                                           Hertz Hertz2)

            => !Hertz1.Equals(Hertz2);

        #endregion

        #region Operator <  (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A frequency.</param>
        /// <param name="Hertz2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Hertz Hertz1,
                                          Hertz Hertz2)

            => Hertz1.CompareTo(Hertz2) < 0;

        #endregion

        #region Operator <= (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A frequency.</param>
        /// <param name="Hertz2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Hertz Hertz1,
                                           Hertz Hertz2)

            => Hertz1.CompareTo(Hertz2) <= 0;

        #endregion

        #region Operator >  (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A frequency.</param>
        /// <param name="Hertz2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Hertz Hertz1,
                                          Hertz Hertz2)

            => Hertz1.CompareTo(Hertz2) > 0;

        #endregion

        #region Operator >= (Hertz1, Hertz2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hertz1">A frequency.</param>
        /// <param name="Hertz2">Another frequency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Hertz Hertz1,
                                           Hertz Hertz2)

            => Hertz1.CompareTo(Hertz2) >= 0;

        #endregion

        #region Operator +  (Hertz1, Hertz2)

        /// <summary>
        /// Accumulates two frequency.
        /// </summary>
        /// <param name="Hertz1">A frequency.</param>
        /// <param name="Hertz2">Another frequency.</param>
        public static Hertz operator + (Hertz Hertz1,
                                       Hertz Hertz2)

            => new (Hertz1.Value + Hertz2.Value);

        #endregion

        #region Operator -  (Hertz1, Hertz2)

        /// <summary>
        /// Substracts two frequency.
        /// </summary>
        /// <param name="Hertz1">A frequency.</param>
        /// <param name="Hertz2">Another frequency.</param>
        public static Hertz operator - (Hertz Hertz1,
                                       Hertz Hertz2)

            => new (Hertz1.Value - Hertz2.Value);

        #endregion

        #endregion

        #region IComparable<Hertz> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two frequency.
        /// </summary>
        /// <param name="Object">A frequency to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Hertz hertz
                   ? CompareTo(hertz)
                   : throw new ArgumentException("The given object is not a frequency!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Hertz)

        /// <summary>
        /// Compares two frequency.
        /// </summary>
        /// <param name="Hertz">A Hertz to compare with.</param>
        public Int32 CompareTo(Hertz Hertz)

            => Value.CompareTo(Hertz.Value);

        #endregion

        #endregion

        #region IEquatable<Hertz> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two frequency for equality.
        /// </summary>
        /// <param name="Object">A frequency to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Hertz hertz &&
                   Equals(hertz);

        #endregion

        #region Equals(Hertz)

        /// <summary>
        /// Compares two frequency for equality.
        /// </summary>
        /// <param name="Hertz">A frequency to compare with.</param>
        public Boolean Equals(Hertz Hertz)

            => Value.Equals(Hertz.Value);

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

            => $"{Value} Hz";

        #endregion

    }

}
