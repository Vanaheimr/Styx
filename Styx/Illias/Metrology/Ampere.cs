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
    /// An Ampere value.
    /// </summary>
    public readonly struct Ampere : IEquatable <Ampere>,
                                    IComparable<Ampere>,
                                    IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the Ampere.
        /// </summary>
        public Decimal  Value           { get; }

        /// <summary>
        /// The value of the Ampere as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Value;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Ampere based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of an Ampere.</param>
        private Ampere(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an Ampere.
        /// </summary>
        /// <param name="Text">A text representation of an Ampere.</param>
        public static Ampere Parse(String Text)
        {

            if (TryParse(Text, out var ampere))
                return ampere;

            throw new ArgumentException($"Invalid text representation of an Ampere: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number, Multiplicator = null)

        /// <summary>
        /// Parse the given number as an Ampere.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ampere.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Ampere Parse(Decimal  Number,
                                   Int32?   Multiplicator = null)
        {

            if (TryParse(Number, out var ampere, Multiplicator))
                return ampere;

            throw new ArgumentException($"Invalid numeric representation of an Ampere: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as an Ampere.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ampere.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Ampere Parse(Byte    Number,
                                   Int32?  Multiplicator = null)
        {

            if (TryParse(Number, out var ampere, Multiplicator))
                return ampere;

            throw new ArgumentException($"Invalid numeric representation of an Ampere: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an Ampere.
        /// </summary>
        /// <param name="Text">A text representation of an Ampere.</param>
        public static Ampere? TryParse(String Text)
        {

            if (TryParse(Text, out var ampere))
                return ampere;

            return null;

        }

        #endregion

        #region (static) TryParse(Number, Multiplicator = null)

        /// <summary>
        /// Try to parse the given number as an Ampere.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ampere.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Ampere? TryParse(Decimal  Number,
                                       Int32?   Multiplicator = null)
        {

            if (TryParse(Number, out var ampere, Multiplicator))
                return ampere;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as an Ampere.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ampere.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Ampere? TryParse(Byte    Number,
                                       Int32?  Multiplicator = null)
        {

            if (TryParse(Number, out var ampere, Multiplicator))
                return ampere;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out Ampere)

        /// <summary>
        /// Parse the given string as an Ampere.
        /// </summary>
        /// <param name="Text">A text representation of an Ampere.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        public static Boolean TryParse(String Text, out Ampere Ampere)
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

                    Ampere = new Ampere(value / factor);

                    return true;

                }

            }
            catch
            { }

            Ampere = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out Ampere, Multiplicator = null)

        /// <summary>
        /// Parse the given number as an Ampere.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ampere.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Boolean TryParse(Byte        Number,
                                       out Ampere  Ampere,
                                       Int32?      Multiplicator = null)
        {

            try
            {

                Ampere = new Ampere(Number * (10 ^ (Multiplicator ?? 0)));

                return true;

            }
            catch
            {
                Ampere = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as an Ampere.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ampere.</param>
        /// <param name="Ampere">The parsed Ampere.</param>
        /// <param name="Multiplicator">An optional 10^n multiplicator.</param>
        public static Boolean TryParse(Decimal     Number,
                                       out Ampere  Ampere,
                                       Int32?      Multiplicator = null)
        {

            try
            {

                Ampere = new Ampere(Number * (10 ^ (Multiplicator ?? 0)));

                return true;

            }
            catch
            {
                Ampere = default;
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this Ampere.
        /// </summary>
        public Ampere Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Ampere Ampere1,
                                           Ampere Ampere2)

            => Ampere1.Equals(Ampere2);

        #endregion

        #region Operator != (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Ampere Ampere1,
                                           Ampere Ampere2)

            => !Ampere1.Equals(Ampere2);

        #endregion

        #region Operator <  (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Ampere Ampere1,
                                          Ampere Ampere2)

            => Ampere1.CompareTo(Ampere2) < 0;

        #endregion

        #region Operator <= (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Ampere Ampere1,
                                           Ampere Ampere2)

            => Ampere1.CompareTo(Ampere2) <= 0;

        #endregion

        #region Operator >  (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Ampere Ampere1,
                                          Ampere Ampere2)

            => Ampere1.CompareTo(Ampere2) > 0;

        #endregion

        #region Operator >= (Ampere1, Ampere2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Ampere Ampere1,
                                           Ampere Ampere2)

            => Ampere1.CompareTo(Ampere2) >= 0;

        #endregion

        #region Operator +  (Ampere1, Ampere2)

        /// <summary>
        /// Accumulates two Amperes.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        public static Ampere operator + (Ampere Ampere1,
                                       Ampere Ampere2)

            => Ampere.Parse(Ampere1.Value + Ampere2.Value);

        #endregion

        #region Operator -  (Ampere1, Ampere2)

        /// <summary>
        /// Substracts two Amperes.
        /// </summary>
        /// <param name="Ampere1">An Ampere value.</param>
        /// <param name="Ampere2">Another Ampere value.</param>
        public static Ampere operator - (Ampere Ampere1,
                                       Ampere Ampere2)

            => Ampere.Parse(Ampere1.Value - Ampere2.Value);

        #endregion

        #endregion

        #region IComparable<Ampere> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Amperes.
        /// </summary>
        /// <param name="Object">An Ampere to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Ampere ampere
                   ? CompareTo(ampere)
                   : throw new ArgumentException("The given object is not an Ampere!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Ampere)

        /// <summary>
        /// Compares two Amperes.
        /// </summary>
        /// <param name="Ampere">An Ampere to compare with.</param>
        public Int32 CompareTo(Ampere Ampere)

            => Value.CompareTo(Ampere.Value);

        #endregion

        #endregion

        #region IEquatable<Ampere> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Amperes for equality.
        /// </summary>
        /// <param name="Object">An Ampere to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Ampere ampere &&
                   Equals(ampere);

        #endregion

        #region Equals(Ampere)

        /// <summary>
        /// Compares two Amperes for equality.
        /// </summary>
        /// <param name="Ampere">An Ampere to compare with.</param>
        public Boolean Equals(Ampere Ampere)

            => Value.Equals(Ampere.Value);

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

            => $"{Value} W";

        #endregion

    }

}
