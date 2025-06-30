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
    /// Extension methods for Ohms.
    /// </summary>
    public static class OhmExtensions
    {

        /// <summary>
        /// The sum of the given Ohm values.
        /// </summary>
        /// <param name="Ohms">An enumeration of Ohm values.</param>
        public static Ohm Sum(this IEnumerable<Ohm> Ohms)
        {

            var sum = Ohm.Zero;

            foreach (var ohm in Ohms)
                sum = sum + ohm;

            return sum;

        }

    }


    /// <summary>
    /// An Ohm value.
    /// </summary>
    public readonly struct Ohm : IEquatable <Ohm>,
                                  IComparable<Ohm>,
                                  IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the Ohms.
        /// </summary>
        public Decimal  Value           { get; }

        /// <summary>
        /// The value of the Ohm as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Value;


        /// <summary>
        /// The value as KiloOhms.
        /// </summary>
        public Decimal  KW
            => Value / 1000;

        /// <summary>
        /// The value as MegaOhms.
        /// </summary>
        public Decimal  MW
            => Value / 1000000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Ohm based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of an Ohm.</param>
        private Ohm(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse        (Text)

        /// <summary>
        /// Parse the given string as an Ohm.
        /// </summary>
        /// <param name="Text">A text representation of an Ohm.</param>
        public static Ohm Parse(String Text)
        {

            if (TryParse(Text, out var ohm))
                return ohm;

            throw new ArgumentException($"Invalid text representation of an Ohm: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseOhm     (Text)

        /// <summary>
        /// Parse the given string as an Ohm.
        /// </summary>
        /// <param name="Text">A text representation of an Ohm.</param>
        public static Ohm ParseOhm(String Text)
        {

            if (TryParseOhm(Text, out var ohm))
                return ohm;

            throw new ArgumentException($"Invalid text representation of an Ohm: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKOhm    (Text)

        /// <summary>
        /// Parse the given string as a KOhm.
        /// </summary>
        /// <param name="Text">A text representation of a KOhm.</param>
        public static Ohm ParseKOhm(String Text)
        {

            if (TryParseKOhm(Text, out var ohm))
                return ohm;

            throw new ArgumentException($"Invalid text representation of a KOhm: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseMOhm    (Text)

        /// <summary>
        /// Parse the given string as a MOhm.
        /// </summary>
        /// <param name="Text">A text representation of a MOhm.</param>
        public static Ohm ParseMOhm(String Text)
        {

            if (TryParseMOhm(Text, out var ohm))
                return ohm;

            throw new ArgumentException($"Invalid text representation of a MOhm: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseOhm     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm ParseOhm(Decimal  Number,
                                   Int32?   Exponent = null)
        {

            if (TryParseOhm(Number, out var ohm, Exponent))
                return ohm;

            throw new ArgumentException($"Invalid numeric representation of an Ohm: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm ParseOhm(Byte    Number,
                                   Int32?  Exponent = null)
        {

            if (TryParseOhm(Number, out var ohm, Exponent))
                return ohm;

            throw new ArgumentException($"Invalid numeric representation of an Ohm: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKOhm    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a KOhm.
        /// </summary>
        /// <param name="Number">A numeric representation of a KOhm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm ParseKOhm(Decimal  Number,
                                    Int32?   Exponent = null)
        {

            if (TryParseKOhm(Number, out var ohm, Exponent))
                return ohm;

            throw new ArgumentException($"Invalid numeric representation of a KOhm: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a KOhm.
        /// </summary>
        /// <param name="Number">A numeric representation of a KOhm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm ParseKOhm(Byte    Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseKOhm(Number, out var ohm, Exponent))
                return ohm;

            throw new ArgumentException($"Invalid numeric representation of a KOhm: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseMOhm    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a MOhm.
        /// </summary>
        /// <param name="Number">A numeric representation of a MOhm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm ParseMOhm(Decimal  Number,
                                    Int32?   Exponent = null)
        {

            if (TryParseMOhm(Number, out var ohm, Exponent))
                return ohm;

            throw new ArgumentException($"Invalid numeric representation of a MOhm: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a MOhm.
        /// </summary>
        /// <param name="Number">A numeric representation of a MOhm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm ParseMOhm(Byte    Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseMOhm(Number, out var ohm, Exponent))
                return ohm;

            throw new ArgumentException($"Invalid numeric representation of a MOhm: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse     (Text)

        /// <summary>
        /// Try to parse the given text as an Ohm.
        /// </summary>
        /// <param name="Text">A text representation of an Ohm.</param>
        public static Ohm? TryParse(String Text)
        {

            if (TryParse(Text, out var ohm))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryParseOhm  (Text)

        /// <summary>
        /// Try to parse the given text as an Ohm.
        /// </summary>
        /// <param name="Text">A text representation of an Ohm.</param>
        public static Ohm? TryParseOhm(String Text)
        {

            if (TryParseOhm(Text, out var ohm))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryParseKOhm (Text)

        /// <summary>
        /// Try to parse the given text as a KOhm.
        /// </summary>
        /// <param name="Text">A text representation of a KOhm.</param>
        public static Ohm? TryParseKOhm(String Text)
        {

            if (TryParseKOhm(Text, out var ohm))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryParseMOhm (Text)

        /// <summary>
        /// Try to parse the given text as a MOhm.
        /// </summary>
        /// <param name="Text">A text representation of a MOhm.</param>
        public static Ohm? TryParseMOhm(String Text)
        {

            if (TryParseMOhm(Text, out var ohm))
                return ohm;

            return null;

        }

        #endregion


        #region (static) TryParseOhm  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryParseOhm(Decimal  Number,
                                      Int32?   Exponent = null)
        {

            if (TryParseOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryParseOhm(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryParseOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryParseKOhm (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a KOhm.
        /// </summary>
        /// <param name="Number">A numeric representation of a KOhm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryParseKOhm(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryParseKOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a KOhm.
        /// </summary>
        /// <param name="Number">A numeric representation of a KOhm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryParseKOhm(Byte    Number,
                                       Int32?  Exponent = null)
        {

            if (TryParseKOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }

        #endregion

        #region (static) TryParseMOhm (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a MOhm.
        /// </summary>
        /// <param name="Number">A numeric representation of a MOhm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryParseMOhm(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseMOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a MOhm.
        /// </summary>
        /// <param name="Number">A numeric representation of a MOhm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Ohm? TryParseMOhm(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseMOhm(Number, out var ohm, Exponent))
                return ohm;

            return null;

        }

        #endregion


        #region (static) TryParse     (Text,   out Ohm)

        /// <summary>
        /// Parse the given string as an Ohm.
        /// </summary>
        /// <param name="Text">A text representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParse(String Text, out Ohm Ohm)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("KOhm") || Text.EndsWith("KV"))
                    factor = 1000;

                if (Decimal.TryParse(Text, out var value))
                {

                    Ohm = new Ohm(value / factor);

                    return true;

                }

            }
            catch
            { }

            Ohm = default;
            return false;

        }

        #endregion

        #region (static) TryParseOhm  (Text,   out Ohm)

        /// <summary>
        /// Parse the given string as an Ohm.
        /// </summary>
        /// <param name="Text">A text representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParseOhm(String Text, out Ohm Ohm)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Ohm = new Ohm(value);

                    return true;

                }

            }
            catch
            { }

            Ohm = default;
            return false;

        }

        #endregion

        #region (static) TryParseKOhm (Text,   out Ohm)

        /// <summary>
        /// Parse the given string as an Ohm.
        /// </summary>
        /// <param name="Text">A text representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParseKOhm(String Text, out Ohm Ohm)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Ohm = new Ohm(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Ohm = default;
            return false;

        }

        #endregion

        #region (static) TryParseMOhm (Text,   out Ohm)

        /// <summary>
        /// Parse the given string as an Ohm.
        /// </summary>
        /// <param name="Text">A text representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        public static Boolean TryParseMOhm(String Text, out Ohm Ohm)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Ohm = new Ohm(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Ohm = default;
            return false;

        }

        #endregion


        #region (static) TryParseOhm  (Number, out Ohm, Exponent = null)

        /// <summary>
        /// Parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseOhm(Byte     Number,
                                          out Ohm  Ohm,
                                          Int32?   Exponent = null)
        {

            try
            {

                Ohm = new Ohm(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Ohm = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseOhm(Decimal  Number,
                                          out Ohm  Ohm,
                                          Int32?   Exponent = null)
        {

            try
            {

                Ohm = new Ohm(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Ohm = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKOhm (Number, out Ohm, Exponent = null)

        /// <summary>
        /// Parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKOhm(Byte     Number,
                                           out Ohm  Ohm,
                                           Int32?   Exponent = null)
        {

            try
            {

                Ohm = new Ohm(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Ohm = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKOhm(Decimal  Number,
                                           out Ohm  Ohm,
                                           Int32?   Exponent = null)
        {

            try
            {

                Ohm = new Ohm(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Ohm = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseMOhm (Number, out Ohm, Exponent = null)

        /// <summary>
        /// Parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseMOhm(Byte     Number,
                                           out Ohm  Ohm,
                                           Int32?   Exponent = null)
        {

            try
            {

                Ohm = new Ohm(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Ohm = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as an Ohm.
        /// </summary>
        /// <param name="Number">A numeric representation of an Ohm.</param>
        /// <param name="Ohm">The parsed Ohm.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseMOhm(Decimal  Number,
                                           out Ohm  Ohm,
                                           Int32?   Exponent = null)
        {

            try
            {

                Ohm = new Ohm(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Ohm = default;
                return false;
            }

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this Ohm.
        /// </summary>
        public Ohm Clone()

            => new (Value);

        #endregion


        public static Ohm Zero
            => new (0);


        #region Operator overloading

        #region Operator == (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">An Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Ohm Ohm1,
                                           Ohm Ohm2)

            => Ohm1.Equals(Ohm2);

        #endregion

        #region Operator != (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">An Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Ohm Ohm1,
                                           Ohm Ohm2)

            => !Ohm1.Equals(Ohm2);

        #endregion

        #region Operator <  (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">An Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Ohm Ohm1,
                                          Ohm Ohm2)

            => Ohm1.CompareTo(Ohm2) < 0;

        #endregion

        #region Operator <= (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">An Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Ohm Ohm1,
                                           Ohm Ohm2)

            => Ohm1.CompareTo(Ohm2) <= 0;

        #endregion

        #region Operator >  (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">An Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Ohm Ohm1,
                                          Ohm Ohm2)

            => Ohm1.CompareTo(Ohm2) > 0;

        #endregion

        #region Operator >= (Ohm1, Ohm2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Ohm1">An Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Ohm Ohm1,
                                           Ohm Ohm2)

            => Ohm1.CompareTo(Ohm2) >= 0;

        #endregion

        #region Operator +  (Ohm1, Ohm2)

        /// <summary>
        /// Accumulates two Ohms.
        /// </summary>
        /// <param name="Ohm1">An Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        public static Ohm operator + (Ohm Ohm1,
                                       Ohm Ohm2)

            => new (Ohm1.Value + Ohm2.Value);

        #endregion

        #region Operator -  (Ohm1, Ohm2)

        /// <summary>
        /// Substracts two Ohms.
        /// </summary>
        /// <param name="Ohm1">An Ohm.</param>
        /// <param name="Ohm2">Another Ohm.</param>
        public static Ohm operator - (Ohm Ohm1,
                                       Ohm Ohm2)

            => new (Ohm1.Value - Ohm2.Value);

        #endregion

        #endregion

        #region IComparable<Ohm> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Ohms.
        /// </summary>
        /// <param name="Object">An Ohm to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Ohm ohm
                   ? CompareTo(ohm)
                   : throw new ArgumentException("The given object is not an Ohm!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Ohm)

        /// <summary>
        /// Compares two Ohms.
        /// </summary>
        /// <param name="Ohm">An Ohm to compare with.</param>
        public Int32 CompareTo(Ohm Ohm)

            => Value.CompareTo(Ohm.Value);

        #endregion

        #endregion

        #region IEquatable<Ohm> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Ohms for equality.
        /// </summary>
        /// <param name="Object">An Ohm to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Ohm ohm &&
                   Equals(ohm);

        #endregion

        #region Equals(Ohm)

        /// <summary>
        /// Compares two Ohms for equality.
        /// </summary>
        /// <param name="Ohm">An Ohm to compare with.</param>
        public Boolean Equals(Ohm Ohm)

            => Value.Equals(Ohm.Value);

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
