/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A Watt value.
    /// </summary>
    public readonly struct Watt : IEquatable <Watt>,
                                  IComparable<Watt>,
                                  IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the Watts.
        /// </summary>
        public Decimal  Value           { get; }

        /// <summary>
        /// The value of the Amperes as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Value;


        /// <summary>
        /// The value as KiloWatts.
        /// </summary>
        public Decimal  KW
            => Value / 1000;

        /// <summary>
        /// The value as MegaWatts.
        /// </summary>
        public Decimal  MW
            => Value / 1000000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Watt based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a Watt.</param>
        private Watt(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a Watt.
        /// </summary>
        /// <param name="Text">A text representation of a Watt.</param>
        public static Watt Parse(String Text)
        {

            if (TryParse(Text, out var watt))
                return watt;

            throw new ArgumentException($"Invalid text representation of a Watt: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt Parse(Decimal  Number,
                                 Int32?   Exponent = null)
        {

            if (TryParse(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a Watt: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt Parse(Byte    Number,
                                 Int32?  Exponent = null)
        {

            if (TryParse(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a Watt: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a Watt.
        /// </summary>
        /// <param name="Text">A text representation of a Watt.</param>
        public static Watt? TryParse(String Text)
        {

            if (TryParse(Text, out var watt))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParse(Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryParse(Decimal  Number,
                                     Int32?   Exponent = null)
        {

            if (TryParse(Number, out var watt, Exponent))
                return watt;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryParse(Byte    Number,
                                     Int32?  Exponent = null)
        {

            if (TryParse(Number, out var watt, Exponent))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out Watt)

        /// <summary>
        /// Parse the given string as a Watt.
        /// </summary>
        /// <param name="Text">A text representation of a Watt.</param>
        /// <param name="Watt">The parsed Watt.</param>
        public static Boolean TryParse(String Text, out Watt Watt)
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

                    Watt = new Watt(value / factor);

                    return true;

                }

            }
            catch
            { }

            Watt = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out Watt, Exponent = null)

        /// <summary>
        /// Parse the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Watt">The parsed Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParse(Byte      Number,
                                       out Watt  Watt,
                                       Int32?    Exponent = null)
        {

            try
            {

                Watt = new Watt(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Watt = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Watt">The parsed Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParse(Decimal   Number,
                                       out Watt  Watt,
                                       Int32?    Exponent = null)
        {

            try
            {

                Watt = new Watt(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Watt = default;
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this Watt.
        /// </summary>
        public Watt Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Watt Watt1,
                                           Watt Watt2)

            => Watt1.Equals(Watt2);

        #endregion

        #region Operator != (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Watt Watt1,
                                           Watt Watt2)

            => !Watt1.Equals(Watt2);

        #endregion

        #region Operator <  (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Watt Watt1,
                                          Watt Watt2)

            => Watt1.CompareTo(Watt2) < 0;

        #endregion

        #region Operator <= (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Watt Watt1,
                                           Watt Watt2)

            => Watt1.CompareTo(Watt2) <= 0;

        #endregion

        #region Operator >  (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Watt Watt1,
                                          Watt Watt2)

            => Watt1.CompareTo(Watt2) > 0;

        #endregion

        #region Operator >= (Watt1, Watt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Watt Watt1,
                                           Watt Watt2)

            => Watt1.CompareTo(Watt2) >= 0;

        #endregion

        #region Operator +  (Watt1, Watt2)

        /// <summary>
        /// Accumulates two Watts.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        public static Watt operator + (Watt Watt1,
                                       Watt Watt2)

            => Watt.Parse(Watt1.Value + Watt2.Value);

        #endregion

        #region Operator -  (Watt1, Watt2)

        /// <summary>
        /// Substracts two Watts.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        public static Watt operator - (Watt Watt1,
                                       Watt Watt2)

            => Watt.Parse(Watt1.Value - Watt2.Value);

        #endregion

        #endregion

        #region IComparable<Watt> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Watts.
        /// </summary>
        /// <param name="Object">A Watt to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Watt watt
                   ? CompareTo(watt)
                   : throw new ArgumentException("The given object is not a Watt!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Watt)

        /// <summary>
        /// Compares two Watts.
        /// </summary>
        /// <param name="Watt">A Watt to compare with.</param>
        public Int32 CompareTo(Watt Watt)

            => Value.CompareTo(Watt.Value);

        #endregion

        #endregion

        #region IEquatable<Watt> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Watts for equality.
        /// </summary>
        /// <param name="Object">A Watt to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Watt watt &&
                   Equals(watt);

        #endregion

        #region Equals(Watt)

        /// <summary>
        /// Compares two Watts for equality.
        /// </summary>
        /// <param name="Watt">A Watt to compare with.</param>
        public Boolean Equals(Watt Watt)

            => Value.Equals(Watt.Value);

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
