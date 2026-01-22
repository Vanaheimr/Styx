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
    /// Extension methods for kilograms.
    /// </summary>
    public static class KilogramExtensions
    {

        /// <summary>
        /// The sum of the given kilogram values.
        /// </summary>
        /// <param name="Kilograms">An enumeration of kilogram values.</param>
        public static Kilogram Sum(this IEnumerable<Kilogram> Kilograms)
        {

            var sum = Kilogram.Zero;

            foreach (var kilogram in Kilograms)
                sum = sum + kilogram;

            return sum;

        }

    }


    /// <summary>
    /// A mass in kilogram (kg).
    /// </summary>
    public readonly struct Kilogram : IEquatable <Kilogram>,
                                      IComparable<Kilogram>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the mass.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The value of the mass as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Math.Round(Value);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new mass in kilogram based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a mass in kilograms.</param>
        private Kilogram(Decimal Value)
        {

            this.Value = Value >= 0
                             ? Value
                             : 0;

        }

        #endregion


        #region (static) Parse         (Text)

        /// <summary>
        /// Parse the given string as a kilogram.
        /// </summary>
        /// <param name="Text">A text representation of a kilogram.</param>
        public static Kilogram Parse(String Text)
        {

            if (TryParse(Text, out var kilogram))
                return kilogram;

            throw new ArgumentException($"Invalid text representation of a kilogram: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKilogram (Text)

        /// <summary>
        /// Parse the given string as a kilogram.
        /// </summary>
        /// <param name="Text">A text representation of a kilogram.</param>
        public static Kilogram ParseKilogram(String Text)
        {

            if (TryParseKilogram(Text, out var kilogram))
                return kilogram;

            throw new ArgumentException($"Invalid text representation of a kilogram: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseGram     (Text)

        /// <summary>
        /// Parse the given string as a gram.
        /// </summary>
        /// <param name="Text">A text representation of a gram.</param>
        public static Kilogram ParseGram(String Text)
        {

            if (TryParseGram(Text, out var kilogram))
                return kilogram;

            throw new ArgumentException($"Invalid text representation of a gram: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseKilogram (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a kilogram.
        /// </summary>
        /// <param name="Number">A numeric representation of a kilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram ParseKilogram(Decimal  Number,
                                             Int32?   Exponent = null)
        {

            if (TryParseKilogram(Number, out var kilogram, Exponent))
                return kilogram;

            throw new ArgumentException($"Invalid numeric representation of a kilogram: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a kilogram.
        /// </summary>
        /// <param name="Number">A numeric representation of a kilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram ParseKilogram(Byte    Number,
                                             Int32?  Exponent = null)
        {

            if (TryParseKilogram(Number, out var kilogram, Exponent))
                return kilogram;

            throw new ArgumentException($"Invalid numeric representation of a kilogram: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseGram     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a gram.
        /// </summary>
        /// <param name="Number">A numeric representation of a gram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram ParseGram(Decimal  Number,
                                         Int32?   Exponent = null)
        {

            if (TryParseGram(Number, out var kilogram, Exponent))
                return kilogram;

            throw new ArgumentException($"Invalid numeric representation of a gram: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a gram.
        /// </summary>
        /// <param name="Number">A numeric representation of a gram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram ParseGram(Byte    Number,
                                         Int32?  Exponent = null)
        {

            if (TryParseGram(Number, out var kilogram, Exponent))
                return kilogram;

            throw new ArgumentException($"Invalid numeric representation of a gram: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse         (Text)

        /// <summary>
        /// Try to parse the given text as a kilogram.
        /// </summary>
        /// <param name="Text">A text representation of a kilogram.</param>
        public static Kilogram? TryParse(String Text)
        {

            if (TryParse(Text, out var kilogram))
                return kilogram;

            return null;

        }

        #endregion

        #region (static) TryParseKilogram (Text)

        /// <summary>
        /// Try to parse the given text as a kilogram.
        /// </summary>
        /// <param name="Text">A text representation of a kilogram.</param>
        public static Kilogram? TryParseKilogram(String Text)
        {

            if (TryParseKilogram(Text, out var kilogram))
                return kilogram;

            return null;

        }

        #endregion

        #region (static) TryParseGram     (Text)

        /// <summary>
        /// Try to parse the given text as a gram.
        /// </summary>
        /// <param name="Text">A text representation of a gram.</param>
        public static Kilogram? TryParseGram(String Text)
        {

            if (TryParseGram(Text, out var kilogram))
                return kilogram;

            return null;

        }

        #endregion


        #region (static) TryParseKilogram (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a kilogram.
        /// </summary>
        /// <param name="Number">A numeric representation of a kilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram? TryParseKilogram(Decimal  Number,
                                                 Int32?   Exponent = null)
        {

            if (TryParseKilogram(Number, out var kilogram, Exponent))
                return kilogram;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a kilogram.
        /// </summary>
        /// <param name="Number">A numeric representation of a kilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram? TryParseKilogram(Byte    Number,
                                                 Int32?  Exponent = null)
        {

            if (TryParseKilogram(Number, out var kilogram, Exponent))
                return kilogram;

            return null;

        }

        #endregion

        #region (static) TryParseGram     (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a gram.
        /// </summary>
        /// <param name="Number">A numeric representation of a gram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram? TryParseGram(Decimal  Number,
                                             Int32?   Exponent = null)
        {

            if (TryParseGram(Number, out var kilogram, Exponent))
                return kilogram;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a gram.
        /// </summary>
        /// <param name="Number">A numeric representation of a gram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Kilogram? TryParseGram(Byte    Number,
                                             Int32?  Exponent = null)
        {

            if (TryParseGram(Number, out var kilogram, Exponent))
                return kilogram;

            return null;

        }

        #endregion


        #region (static) TryParse         (Text,   out Kilogram)

        /// <summary>
        /// Parse the given string as a kilogram.
        /// </summary>
        /// <param name="Text">A text representation of a kilogram.</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        public static Boolean TryParse(String Text, out Kilogram Kilogram)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("g") && !Text.EndsWith("kg"))
                    factor = 1/1000;

                if (Decimal.TryParse(Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Kilogram = new Kilogram(factor * value);

                    return true;

                }

            }
            catch
            { }

            Kilogram = default;
            return false;

        }

        #endregion

        #region (static) TryParseKilogram (Text,   out Kilogram)

        /// <summary>
        /// Parse the given string as a kilogram.
        /// </summary>
        /// <param name="Text">A text representation of a kilogram.</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        public static Boolean TryParseKilogram(String Text, out Kilogram Kilogram)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Kilogram = new Kilogram(value);

                    return true;

                }

            }
            catch
            { }

            Kilogram = default;
            return false;

        }

        #endregion

        #region (static) TryParseGram     (Text,   out Kilogram)

        /// <summary>
        /// Parse the given string as a gram.
        /// </summary>
        /// <param name="Text">A text representation of a gram.</param>
        /// <param name="Kilogram">The parsed gram.</param>
        public static Boolean TryParseGram(String Text, out Kilogram Kilogram)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Kilogram = new Kilogram(value / 1000);

                    return true;

                }

            }
            catch
            { }

            Kilogram = default;
            return false;

        }

        #endregion


        #region (static) TryParseKilogram (Number, out Kilogram, Exponent = null)

        /// <summary>
        /// Parse the given number as a kilogram.
        /// </summary>
        /// <param name="Number">A numeric representation of a kilogram.</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKilogram(Byte          Number,
                                               out Kilogram  Kilogram,
                                               Int32?        Exponent = null)
        {

            try
            {

                Kilogram = new Kilogram(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Kilogram = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a kilogram.
        /// </summary>
        /// <param name="Number">A numeric representation of a kilogram.</param>
        /// <param name="Kilogram">The parsed Kilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKilogram(Decimal       Number,
                                               out Kilogram  Kilogram,
                                               Int32?        Exponent = null)
        {

            try
            {

                Kilogram = new Kilogram(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Kilogram = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseGram     (Number, out Kilogram, Exponent = null)

        /// <summary>
        /// Parse the given number as a kilokilogram.
        /// </summary>
        /// <param name="Number">A numeric representation of a kilokilogram.</param>
        /// <param name="Kilogram">The parsed kilokilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseGram(Byte          Number,
                                           out Kilogram  Kilogram,
                                           Int32?        Exponent = null)
        {

            try
            {

                Kilogram = new Kilogram(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Kilogram = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a kilokilogram.
        /// </summary>
        /// <param name="Number">A numeric representation of a kilokilogram.</param>
        /// <param name="Kilogram">The parsed kilokilogram.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseGram(Decimal       Number,
                                           out Kilogram  Kilogram,
                                           Int32?        Exponent = null)
        {

            try
            {

                Kilogram = new Kilogram(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Kilogram = default;
                return false;
            }

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this Kilogram.
        /// </summary>
        public Kilogram Clone()

            => new (Value);

        #endregion


        public static Kilogram Zero
            => new (0);


        #region Operator overloading

        #region Operator == (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => Kilogram1.Equals(Kilogram2);

        #endregion

        #region Operator != (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => !Kilogram1.Equals(Kilogram2);

        #endregion

        #region Operator <  (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Kilogram Kilogram1,
                                          Kilogram Kilogram2)

            => Kilogram1.CompareTo(Kilogram2) < 0;

        #endregion

        #region Operator <= (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => Kilogram1.CompareTo(Kilogram2) <= 0;

        #endregion

        #region Operator >  (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Kilogram Kilogram1,
                                          Kilogram Kilogram2)

            => Kilogram1.CompareTo(Kilogram2) > 0;

        #endregion

        #region Operator >= (Kilogram1, Kilogram2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Kilogram Kilogram1,
                                           Kilogram Kilogram2)

            => Kilogram1.CompareTo(Kilogram2) >= 0;

        #endregion

        #region Operator +  (Kilogram1, Kilogram2)

        /// <summary>
        /// Accumulates two kilograms.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        public static Kilogram operator + (Kilogram Kilogram1,
                                       Kilogram Kilogram2)

            => new (Kilogram1.Value + Kilogram2.Value);

        #endregion

        #region Operator -  (Kilogram1, Kilogram2)

        /// <summary>
        /// Substracts two kilograms.
        /// </summary>
        /// <param name="Kilogram1">A kilogram.</param>
        /// <param name="Kilogram2">Another kilogram.</param>
        public static Kilogram operator - (Kilogram Kilogram1,
                                       Kilogram Kilogram2)

            => new (Kilogram1.Value - Kilogram2.Value);

        #endregion

        #endregion

        #region IComparable<Kilogram> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two kilograms.
        /// </summary>
        /// <param name="Object">A kilogram to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Kilogram kilogram
                   ? CompareTo(kilogram)
                   : throw new ArgumentException("The given object is not a kilogram!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Kilogram)

        /// <summary>
        /// Compares two kilograms.
        /// </summary>
        /// <param name="Kilogram">A Kilogram to compare with.</param>
        public Int32 CompareTo(Kilogram Kilogram)

            => Value.CompareTo(Kilogram.Value);

        #endregion

        #endregion

        #region IEquatable<Kilogram> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two kilograms for equality.
        /// </summary>
        /// <param name="Object">A kilogram to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Kilogram kilogram &&
                   Equals(kilogram);

        #endregion

        #region Equals(Kilogram)

        /// <summary>
        /// Compares two kilograms for equality.
        /// </summary>
        /// <param name="Kilogram">A kilogram to compare with.</param>
        public Boolean Equals(Kilogram Kilogram)

            => Value.Equals(Kilogram.Value);

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

            => $"{Value} kg";

        #endregion

    }

}
