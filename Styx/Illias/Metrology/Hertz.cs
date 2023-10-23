/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

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
            => (Int32) Value;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new frequency based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a frequency.</param>
        private Hertz(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a frequency.
        /// </summary>
        /// <param name="Text">A text representation of a frequency.</param>
        public static Hertz Parse(String Text)
        {

            if (TryParse(Text, out var hertz))
                return hertz;

            throw new ArgumentException($"Invalid text representation of a frequency: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number, Multiplicator = null)

        /// <summary>
        /// Parse the given number as a frequency.
        /// </summary>
        /// <param name="Number">A numeric representation of a frequency.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Hertz Parse(Decimal  Number,
                                  Int32?   Multiplicator = null)
        {

            if (TryParse(Number, out var hertz, Multiplicator))
                return hertz;

            throw new ArgumentException($"Invalid numeric representation of a frequency: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a frequency.
        /// </summary>
        /// <param name="Number">A numeric representation of a frequency.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Hertz Parse(Byte    Number,
                                  Int32?  Multiplicator = null)
        {

            if (TryParse(Number, out var hertz, Multiplicator))
                return hertz;

            throw new ArgumentException($"Invalid numeric representation of a frequency: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a frequency.
        /// </summary>
        /// <param name="Text">A text representation of a frequency.</param>
        public static Hertz? TryParse(String Text)
        {

            if (TryParse(Text, out var hertz))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParse(Number, Multiplicator = null)

        /// <summary>
        /// Try to parse the given number as a frequency.
        /// </summary>
        /// <param name="Number">A numeric representation of a frequency.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Hertz? TryParse(Decimal  Number,
                                      Int32?   Multiplicator = null)
        {

            if (TryParse(Number, out var hertz, Multiplicator))
                return hertz;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a frequency.
        /// </summary>
        /// <param name="Number">A numeric representation of a frequency.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Hertz? TryParse(Byte    Number,
                                      Int32?  Multiplicator = null)
        {

            if (TryParse(Number, out var hertz, Multiplicator))
                return hertz;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out Hertz)

        /// <summary>
        /// Parse the given string as a frequency.
        /// </summary>
        /// <param name="Text">A text representation of a frequency.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        public static Boolean TryParse(String Text, out Hertz Hertz)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("kW") || Text.EndsWith("KW"))
                    factor = 1000;

                if (Text.EndsWith("MW"))
                    factor = 1000000;

                if (Text.EndsWith("GW"))
                    factor = 1000000;

                if (Decimal.TryParse(Text, out var value))
                {

                    Hertz = new Hertz(value / factor);

                    return true;

                }

            }
            catch
            { }

            Hertz = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out Hertz, Multiplicator = null)

        /// <summary>
        /// Parse the given number as a frequency.
        /// </summary>
        /// <param name="Number">A numeric representation of a frequency.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Boolean TryParse(Byte       Number,
                                       out Hertz  Hertz,
                                       Int32?     Multiplicator = null)
        {

            try
            {

                Hertz = new Hertz(Number * (10 ^ (Multiplicator ?? 0)));

                return true;

            }
            catch
            {
                Hertz = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a frequency.
        /// </summary>
        /// <param name="Number">A numeric representation of a frequency.</param>
        /// <param name="Hertz">The parsed Hertz.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Boolean TryParse(Decimal    Number,
                                       out Hertz  Hertz,
                                       Int32?     Multiplicator = null)
        {

            try
            {

                Hertz = new Hertz(Number * (10 ^ (Multiplicator ?? 0)));

                return true;

            }
            catch
            {
                Hertz = default;
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this Hertz.
        /// </summary>
        public Hertz Clone

            => new (Value);

        #endregion


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

            => Hertz.Parse(Hertz1.Value + Hertz2.Value);

        #endregion

        #region Operator -  (Hertz1, Hertz2)

        /// <summary>
        /// Substracts two frequency.
        /// </summary>
        /// <param name="Hertz1">A frequency.</param>
        /// <param name="Hertz2">Another frequency.</param>
        public static Hertz operator - (Hertz Hertz1,
                                       Hertz Hertz2)

            => Hertz.Parse(Hertz1.Value - Hertz2.Value);

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
