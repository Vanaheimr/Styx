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
    /// Extension methods for Volts.
    /// </summary>
    public static class VoltExtensions
    {

        /// <summary>
        /// The sum of the given Volt values.
        /// </summary>
        /// <param name="Volts">An enumeration of Volt values.</param>
        public static Volt Sum(this IEnumerable<Volt> Volts)
        {

            var sum = Volt.Zero;

            foreach (var volt in Volts)
                sum = sum + volt;

            return sum;

        }

    }


    /// <summary>
    /// A Volt value.
    /// </summary>
    public readonly struct Volt : IEquatable <Volt>,
                                  IComparable<Volt>,
                                  IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the Volts.
        /// </summary>
        public Decimal  Value           { get; }

        /// <summary>
        /// The value of the Volt as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Value;


        /// <summary>
        /// The value as KiloVolts.
        /// </summary>
        public Decimal  KV
            => Value / 1000;

        /// <summary>
        /// The value as MegaVolts.
        /// </summary>
        public Decimal  MV
            => Value / 1000000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Volt based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a Volt.</param>
        private Volt(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as a Volt.
        /// </summary>
        /// <param name="Text">A text representation of a Volt.</param>
        public static Volt Parse(String Text)
        {

            if (TryParse(Text, out var volt))
                return volt;

            throw new ArgumentException($"Invalid text representation of a Volt: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseV     (Text)

        /// <summary>
        /// Parse the given string as a Volt.
        /// </summary>
        /// <param name="Text">A text representation of a Volt.</param>
        public static Volt ParseV(String Text)
        {

            if (TryParseV(Text, out var volt))
                return volt;

            throw new ArgumentException($"Invalid text representation of a Volt: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKV    (Text)

        /// <summary>
        /// Parse the given string as a kV.
        /// </summary>
        /// <param name="Text">A text representation of a kV.</param>
        public static Volt ParseKV(String Text)
        {

            if (TryParseKV(Text, out var volt))
                return volt;

            throw new ArgumentException($"Invalid text representation of a kV: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseV     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Volt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt ParseV(Decimal  Number,
                                  Int32?   Exponent = null)
        {

            if (TryParseV(Number, out var volt, Exponent))
                return volt;

            throw new ArgumentException($"Invalid numeric representation of a Volt: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Volt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt ParseV(Byte    Number,
                                  Int32?  Exponent = null)
        {

            if (TryParseV(Number, out var volt, Exponent))
                return volt;

            throw new ArgumentException($"Invalid numeric representation of a Volt: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKV    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a kV.
        /// </summary>
        /// <param name="Number">A numeric representation of a kV.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt ParseKV(Decimal  Number,
                                   Int32?   Exponent = null)
        {

            if (TryParseKV(Number, out var volt, Exponent))
                return volt;

            throw new ArgumentException($"Invalid numeric representation of a kV: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a kV.
        /// </summary>
        /// <param name="Number">A numeric representation of a kV.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt ParseKV(Byte    Number,
                                   Int32?  Exponent = null)
        {

            if (TryParseKV(Number, out var volt, Exponent))
                return volt;

            throw new ArgumentException($"Invalid numeric representation of a kV: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as a Volt.
        /// </summary>
        /// <param name="Text">A text representation of a Volt.</param>
        public static Volt? TryParse(String Text)
        {

            if (TryParse(Text, out var volt))
                return volt;

            return null;

        }

        #endregion

        #region (static) TryParseV  (Text)

        /// <summary>
        /// Try to parse the given text as a Volt.
        /// </summary>
        /// <param name="Text">A text representation of a Volt.</param>
        public static Volt? TryParseV(String Text)
        {

            if (TryParseV(Text, out var volt))
                return volt;

            return null;

        }

        #endregion

        #region (static) TryParseKV (Text)

        /// <summary>
        /// Try to parse the given text as a kV.
        /// </summary>
        /// <param name="Text">A text representation of a kV.</param>
        public static Volt? TryParseKV(String Text)
        {

            if (TryParseKV(Text, out var volt))
                return volt;

            return null;

        }

        #endregion


        #region (static) TryParseV  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Volt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt? TryParseV(Decimal  Number,
                                      Int32?   Exponent = null)
        {

            if (TryParseV(Number, out var volt, Exponent))
                return volt;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Volt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt? TryParseV(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryParseV(Number, out var volt, Exponent))
                return volt;

            return null;

        }

        #endregion

        #region (static) TryParseKV (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a kV.
        /// </summary>
        /// <param name="Number">A numeric representation of a kV.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt? TryParseKV(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryParseKV(Number, out var volt, Exponent))
                return volt;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a kV.
        /// </summary>
        /// <param name="Number">A numeric representation of a kV.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Volt? TryParseKV(Byte    Number,
                                       Int32?  Exponent = null)
        {

            if (TryParseKV(Number, out var volt, Exponent))
                return volt;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,   out Volt)

        /// <summary>
        /// Parse the given string as a Volt.
        /// </summary>
        /// <param name="Text">A text representation of a Volt.</param>
        /// <param name="Volt">The parsed Volt.</param>
        public static Boolean TryParse(String Text, out Volt Volt)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("kV") || Text.EndsWith("KV"))
                    factor = 1000;

                if (Decimal.TryParse(Text, out var value))
                {

                    Volt = new Volt(value / factor);

                    return true;

                }

            }
            catch
            { }

            Volt = default;
            return false;

        }

        #endregion

        #region (static) TryParseV  (Text,   out Volt)

        /// <summary>
        /// Parse the given string as a Volt.
        /// </summary>
        /// <param name="Text">A text representation of a Volt.</param>
        /// <param name="Volt">The parsed Volt.</param>
        public static Boolean TryParseV(String Text, out Volt Volt)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Volt = new Volt(value);

                    return true;

                }

            }
            catch
            { }

            Volt = default;
            return false;

        }

        #endregion

        #region (static) TryParseKV (Text,   out Volt)

        /// <summary>
        /// Parse the given string as a Volt.
        /// </summary>
        /// <param name="Text">A text representation of a Volt.</param>
        /// <param name="Volt">The parsed Volt.</param>
        public static Boolean TryParseKV(String Text, out Volt Volt)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Volt = new Volt(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Volt = default;
            return false;

        }

        #endregion


        #region (static) TryParseV  (Number, out Volt, Exponent = null)

        /// <summary>
        /// Parse the given number as a Volt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt.</param>
        /// <param name="Volt">The parsed Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseV(Byte      Number,
                                        out Volt  Volt,
                                        Int32?    Exponent = null)
        {

            try
            {

                Volt = new Volt(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Volt = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Volt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt.</param>
        /// <param name="Volt">The parsed Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseV(Decimal   Number,
                                        out Volt  Volt,
                                        Int32?    Exponent = null)
        {

            try
            {

                Volt = new Volt(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Volt = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKV (Number, out Volt, Exponent = null)

        /// <summary>
        /// Parse the given number as a Volt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt.</param>
        /// <param name="Volt">The parsed Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKV(Byte      Number,
                                         out Volt  Volt,
                                         Int32?    Exponent = null)
        {

            try
            {

                Volt = new Volt(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Volt = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Volt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt.</param>
        /// <param name="Volt">The parsed Volt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKV(Decimal   Number,
                                         out Volt  Volt,
                                         Int32?    Exponent = null)
        {

            try
            {

                Volt = new Volt(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                return true;

            }
            catch
            {
                Volt = default;
                return false;
            }

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this Volt.
        /// </summary>
        public Volt Clone()

            => new (Value);

        #endregion


        public static Volt Zero
            => new (0);


        #region Operator overloading

        #region Operator == (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Volt Volt1,
                                           Volt Volt2)

            => Volt1.Equals(Volt2);

        #endregion

        #region Operator != (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Volt Volt1,
                                           Volt Volt2)

            => !Volt1.Equals(Volt2);

        #endregion

        #region Operator <  (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Volt Volt1,
                                          Volt Volt2)

            => Volt1.CompareTo(Volt2) < 0;

        #endregion

        #region Operator <= (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Volt Volt1,
                                           Volt Volt2)

            => Volt1.CompareTo(Volt2) <= 0;

        #endregion

        #region Operator >  (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Volt Volt1,
                                          Volt Volt2)

            => Volt1.CompareTo(Volt2) > 0;

        #endregion

        #region Operator >= (Volt1, Volt2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Volt Volt1,
                                           Volt Volt2)

            => Volt1.CompareTo(Volt2) >= 0;

        #endregion

        #region Operator +  (Volt1, Volt2)

        /// <summary>
        /// Accumulates two Volts.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        public static Volt operator + (Volt Volt1,
                                       Volt Volt2)

            => new (Volt1.Value + Volt2.Value);

        #endregion

        #region Operator -  (Volt1, Volt2)

        /// <summary>
        /// Substracts two Volts.
        /// </summary>
        /// <param name="Volt1">A Volt.</param>
        /// <param name="Volt2">Another Volt.</param>
        public static Volt operator - (Volt Volt1,
                                       Volt Volt2)

            => new (Volt1.Value - Volt2.Value);

        #endregion

        #endregion

        #region IComparable<Volt> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Volts.
        /// </summary>
        /// <param name="Object">A Volt to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Volt volt
                   ? CompareTo(volt)
                   : throw new ArgumentException("The given object is not a Volt!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Volt)

        /// <summary>
        /// Compares two Volts.
        /// </summary>
        /// <param name="Volt">A Volt to compare with.</param>
        public Int32 CompareTo(Volt Volt)

            => Value.CompareTo(Volt.Value);

        #endregion

        #endregion

        #region IEquatable<Volt> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Volts for equality.
        /// </summary>
        /// <param name="Object">A Volt to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Volt volt &&
                   Equals(volt);

        #endregion

        #region Equals(Volt)

        /// <summary>
        /// Compares two Volts for equality.
        /// </summary>
        /// <param name="Volt">A Volt to compare with.</param>
        public Boolean Equals(Volt Volt)

            => Value.Equals(Volt.Value);

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
