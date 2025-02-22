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
    /// Extension methods for m².
    /// </summary>
    public static class SquareMeterExtensions
    {

        /// <summary>
        /// The sum of the given m² values.
        /// </summary>
        /// <param name="SquareMeters">An enumeration of m² values.</param>
        public static SquareMeter Sum(this IEnumerable<SquareMeter> SquareMeters)
        {

            var sum = SquareMeter.Zero;

            foreach (var squareMeter in SquareMeters)
                sum = sum + squareMeter;

            return sum;

        }

    }


    /// <summary>
    /// A m².
    /// </summary>
    public readonly struct SquareMeter : IEquatable<SquareMeter>,
                                         IComparable<SquareMeter>,
                                         IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the m².
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The value of the SquareMeter as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Value;


        /// <summary>
        /// The value as KiloSquareMeters.
        /// </summary>
        public Decimal  KM
            => Value / 1000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SquareMeter based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a m².</param>
        private SquareMeter(Decimal Value)
        {

            this.Value = Value >= 0
                             ? Value
                             : 0;

        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as a m².
        /// </summary>
        /// <param name="Text">A text representation of a m².</param>
        public static SquareMeter Parse(String Text)
        {

            if (TryParse(Text, out var squareMeter))
                return squareMeter;

            throw new ArgumentException($"Invalid text representation of a m²: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseSM     (Text)

        /// <summary>
        /// Parse the given string as a m².
        /// </summary>
        /// <param name="Text">A text representation of a m².</param>
        public static SquareMeter ParseSM(String Text)
        {

            if (TryParseSM(Text, out var squareMeter))
                return squareMeter;

            throw new ArgumentException($"Invalid text representation of a m²: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseSKM    (Text)

        /// <summary>
        /// Parse the given string as a km².
        /// </summary>
        /// <param name="Text">A text representation of a km².</param>
        public static SquareMeter ParseSKM(String Text)
        {

            if (TryParseSKM(Text, out var squareMeter))
                return squareMeter;

            throw new ArgumentException($"Invalid text representation of a km²: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseSM     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter ParseSM(Decimal  Number,
                                   Int32?   Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            throw new ArgumentException($"Invalid numeric representation of a m²: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter ParseSM(Double  Number,
                                   Int32?  Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            throw new ArgumentException($"Invalid numeric representation of a m²: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter ParseSM(Byte    Number,
                                   Int32?  Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            throw new ArgumentException($"Invalid numeric representation of a m²: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseSKM    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a km².
        /// </summary>
        /// <param name="Number">A numeric representation of a km².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter ParseSKM(Decimal  Number,
                                    Int32?   Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            throw new ArgumentException($"Invalid numeric representation of a km²: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km².
        /// </summary>
        /// <param name="Number">A numeric representation of a km².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter ParseSKM(Double  Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            throw new ArgumentException($"Invalid numeric representation of a km²: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km².
        /// </summary>
        /// <param name="Number">A numeric representation of a km².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter ParseSKM(Byte    Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            throw new ArgumentException($"Invalid numeric representation of a km²: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as a m².
        /// </summary>
        /// <param name="Text">A text representation of a m².</param>
        public static SquareMeter? TryParse(String Text)
        {

            if (TryParse(Text, out var squareMeter))
                return squareMeter;

            return null;

        }

        #endregion

        #region (static) TryParseSM  (Text)

        /// <summary>
        /// Try to parse the given text as a m².
        /// </summary>
        /// <param name="Text">A text representation of a m².</param>
        public static SquareMeter? TryParseSM(String Text)
        {

            if (TryParseSM(Text, out var squareMeter))
                return squareMeter;

            return null;

        }

        #endregion

        #region (static) TryParseSKM (Text)

        /// <summary>
        /// Try to parse the given text as a km².
        /// </summary>
        /// <param name="Text">A text representation of a km².</param>
        public static SquareMeter? TryParseSKM(String Text)
        {

            if (TryParseSKM(Text, out var squareMeter))
                return squareMeter;

            return null;

        }

        #endregion


        #region (static) TryParseSM  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter? TryParseSM(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter? TryParseSM(Double  Number,
                                       Int32?  Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter? TryParseSM(Byte    Number,
                                       Int32?  Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            return null;

        }

        #endregion

        #region (static) TryParseSKM (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a km².
        /// </summary>
        /// <param name="Number">A numeric representation of a km².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter? TryParseSKM(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a km².
        /// </summary>
        /// <param name="Number">A numeric representation of a km².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter? TryParseSKM(Double  Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseSKM(Number, out var squareMeter, Exponent))
                return squareMeter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static SquareMeter? TryParse(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryParseSM(Number, out var squareMeter, Exponent))
                return squareMeter;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,   out SquareMeter)

        /// <summary>
        /// Parse the given string as a m².
        /// </summary>
        /// <param name="Text">A text representation of a m².</param>
        /// <param name="SquareMeter">The parsed m².</param>
        public static Boolean TryParse(String Text, out SquareMeter SquareMeter)
        {

            try
            {

                Text = Text.Trim();

                var factor = 1;

                if (Text.EndsWith("km"))
                    factor = 1000;

                if (Decimal.TryParse(Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    SquareMeter = new SquareMeter(factor * value);

                    return true;

                }

            }
            catch
            { }

            SquareMeter = default;
            return false;

        }

        #endregion

        #region (static) TryParseSM  (Text,   out SquareMeter)

        /// <summary>
        /// Parse the given string as a m².
        /// </summary>
        /// <param name="Text">A text representation of a m².</param>
        /// <param name="SquareMeter">The parsed m².</param>
        public static Boolean TryParseSM(String Text, out SquareMeter SquareMeter)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    SquareMeter = new SquareMeter(value);

                    return true;

                }

            }
            catch
            { }

            SquareMeter = default;
            return false;

        }

        #endregion

        #region (static) TryParseSKM (Text,   out SquareMeter)

        /// <summary>
        /// Parse the given string as a km².
        /// </summary>
        /// <param name="Text">A text representation of a km².</param>
        /// <param name="SquareMeter">The parsed km².</param>
        public static Boolean TryParseSKM(String Text, out SquareMeter SquareMeter)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    SquareMeter = new SquareMeter(1000000 * value);

                    return true;

                }

            }
            catch
            { }

            SquareMeter = default;
            return false;

        }

        #endregion


        #region (static) TryParseSM  (Number, out SquareMeter, Exponent = null)

        /// <summary>
        /// Parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="SquareMeter">The parsed m².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseSM(Byte       Number,
                                        out SquareMeter  SquareMeter,
                                        Int32?     Exponent = null)
        {

            SquareMeter = new SquareMeter(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

            if (Number < 0)
                return false;

            return true;

        }


        /// <summary>
        /// Parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="SquareMeter">The parsed m².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseSM(Double     Number,
                                        out SquareMeter  SquareMeter,
                                        Int32?     Exponent = null)
        {

            try
            {

                SquareMeter = new SquareMeter((Decimal) (Number * Math.Pow(10, Exponent ?? 0)));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                SquareMeter = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a m².
        /// </summary>
        /// <param name="Number">A numeric representation of a m².</param>
        /// <param name="SquareMeter">The parsed m².</param>
        public static Boolean TryParseSM(Decimal    Number,
                                        out SquareMeter  SquareMeter,
                                        Int32?     Exponent = null)
        {

            try
            {

                SquareMeter = new SquareMeter(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                SquareMeter = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseSKM (Number, out SquareMeter, Exponent = null)

        /// <summary>
        /// Parse the given number as a km².
        /// </summary>
        /// <param name="Number">A numeric representation of a km².</param>
        /// <param name="SquareMeter">The parsed km².</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseSKM(Byte       Number,
                                         out SquareMeter  SquareMeter,
                                         Int32?     Exponent = null)
        {

            SquareMeter = new SquareMeter(1000000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

            if (Number < 0)
                return false;

            return true;

        }


        /// <summary>
        /// Parse the given number as a km².
        /// </summary>
        /// <param name="Number">A numeric representation of a km².</param>
        /// <param name="SquareMeter">The parsed km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseSKM(Double     Number,
                                         out SquareMeter  SquareMeter,
                                         Int32?     Exponent = null)
        {

            try
            {

                SquareMeter = new SquareMeter(1000 * (Decimal) (Number * Math.Pow(10, Exponent ?? 0)));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                SquareMeter = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a km².
        /// </summary>
        /// <param name="Number">A numeric representation of a km².</param>
        /// <param name="SquareMeter">The parsed km.</param>
        public static Boolean TryParseSKM(Decimal    Number,
                                         out SquareMeter  SquareMeter,
                                         Int32?     Exponent = null)
        {

            try
            {

                SquareMeter = new SquareMeter(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                SquareMeter = default;
                return false;
            }

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this m².
        /// </summary>
        public SquareMeter Clone()

            => new (Value);

        #endregion


        public static SquareMeter Zero
            => new (0);


        #region Operator overloading

        #region Operator == (SquareMeter1, SquareMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SquareMeter1">A m².</param>
        /// <param name="SquareMeter2">Another m².</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SquareMeter SquareMeter1,
                                           SquareMeter SquareMeter2)

            => SquareMeter1.Equals(SquareMeter2);

        #endregion

        #region Operator != (SquareMeter1, SquareMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SquareMeter1">A m².</param>
        /// <param name="SquareMeter2">Another m².</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SquareMeter SquareMeter1,
                                           SquareMeter SquareMeter2)

            => !SquareMeter1.Equals(SquareMeter2);

        #endregion

        #region Operator <  (SquareMeter1, SquareMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SquareMeter1">A m².</param>
        /// <param name="SquareMeter2">Another m².</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SquareMeter SquareMeter1,
                                          SquareMeter SquareMeter2)

            => SquareMeter1.CompareTo(SquareMeter2) < 0;

        #endregion

        #region Operator <= (SquareMeter1, SquareMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SquareMeter1">A m².</param>
        /// <param name="SquareMeter2">Another m².</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SquareMeter SquareMeter1,
                                           SquareMeter SquareMeter2)

            => SquareMeter1.CompareTo(SquareMeter2) <= 0;

        #endregion

        #region Operator >  (SquareMeter1, SquareMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SquareMeter1">A m².</param>
        /// <param name="SquareMeter2">Another m².</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SquareMeter SquareMeter1,
                                          SquareMeter SquareMeter2)

            => SquareMeter1.CompareTo(SquareMeter2) > 0;

        #endregion

        #region Operator >= (SquareMeter1, SquareMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SquareMeter1">A m².</param>
        /// <param name="SquareMeter2">Another m².</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SquareMeter SquareMeter1,
                                           SquareMeter SquareMeter2)

            => SquareMeter1.CompareTo(SquareMeter2) >= 0;

        #endregion

        #region Operator +  (SquareMeter1, SquareMeter2)

        /// <summary>
        /// Accumulates two SquareMeters.
        /// </summary>
        /// <param name="SquareMeter1">A m².</param>
        /// <param name="SquareMeter2">Another m².</param>
        public static SquareMeter operator + (SquareMeter SquareMeter1,
                                        SquareMeter SquareMeter2)

            => new (SquareMeter1.Value + SquareMeter2.Value);

        #endregion

        #region Operator -  (SquareMeter1, SquareMeter2)

        /// <summary>
        /// Substracts two SquareMeters.
        /// </summary>
        /// <param name="SquareMeter1">A m².</param>
        /// <param name="SquareMeter2">Another m².</param>
        public static SquareMeter operator - (SquareMeter SquareMeter1,
                                        SquareMeter SquareMeter2)

            => new (SquareMeter1.Value - SquareMeter2.Value);

        #endregion

        #endregion

        #region IComparable<SquareMeter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two m²s.
        /// </summary>
        /// <param name="Object">A m² to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SquareMeter squareMeter
                   ? CompareTo(squareMeter)
                   : throw new ArgumentException("The given object is not a m²!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SquareMeter)

        /// <summary>
        /// Compares two m²s.
        /// </summary>
        /// <param name="SquareMeter">A m² to compare with.</param>
        public Int32 CompareTo(SquareMeter SquareMeter)

            => Value.CompareTo(SquareMeter.Value);

        #endregion

        #endregion

        #region IEquatable<SquareMeter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two m²s for equality.
        /// </summary>
        /// <param name="Object">A m² to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SquareMeter squareMeter &&
                   Equals(squareMeter);

        #endregion

        #region Equals(SquareMeter)

        /// <summary>
        /// Compares two m²s for equality.
        /// </summary>
        /// <param name="SquareMeter">A m² to compare with.</param>
        public Boolean Equals(SquareMeter SquareMeter)

            => Value.Equals(SquareMeter.Value);

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

            => $"{Value} m";

        #endregion

    }

}
