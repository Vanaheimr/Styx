﻿/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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
    /// A percentage (Internal Byte variant).
    /// </summary>
    public readonly struct PercentageByte : IEquatable <PercentageByte>,
                                            IComparable<PercentageByte>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the percentage.
        /// </summary>
        public Byte  Value    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new percentage based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a percentage.</param>
        private PercentageByte(Byte Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as a percentage.
        /// </summary>
        /// <param name="Text">A text representation of a percentage.</param>
        public static PercentageByte Parse(String Text)
        {

            if (TryParse(Text, out var percentage))
                return percentage;

            throw new ArgumentException($"Invalid text representation of a percentage: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseXXX   (Number)

        /// <summary>
        /// Parse the given number as a percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a percentage.</param>
        public static PercentageByte ParseDouble(Double Number)
        {

            if (TryParseDouble(Number, out var percentage))
                return percentage;

            throw new ArgumentException($"Invalid numeric representation of a percentage: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a percentage.</param>
        public static PercentageByte ParseDecimal(Decimal Number)
        {

            if (TryParseDecimal(Number, out var percentage))
                return percentage;

            throw new ArgumentException($"Invalid numeric representation of a percentage: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a percentage.</param>
        public static PercentageByte Parse(Byte Number)
        {

            if (TryParse(Number, out var percentage))
                return percentage;

            throw new ArgumentException($"Invalid numeric representation of a percentage: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as a percentage.
        /// </summary>
        /// <param name="Text">A text representation of a percentage.</param>
        public static PercentageByte? TryParse(String Text)
        {

            if (TryParse(Text, out var percentage))
                return percentage;

            return null;

        }

        #endregion

        #region (static) TryParseXXX(Number)

        /// <summary>
        /// Try to parse the given number as a percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a percentage.</param>
        public static PercentageByte? TryParseDouble(Double Number)
        {

            if (TryParseDouble(Number, out var percentage))
                return percentage;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a percentage.</param>
        public static PercentageByte? TryParseDecimal(Decimal Number)
        {

            if (TryParseDecimal(Number, out var percentage))
                return percentage;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a percentage.</param>
        public static PercentageByte? TryParse(Byte Number)
        {

            if (TryParse(Number, out var percentage))
                return percentage;

            return null;

        }

        #endregion

        #region (static) TryParse   (Text,   out Percentage)

        /// <summary>
        /// Parse the given string as a percentage.
        /// </summary>
        /// <param name="Text">A text representation of a percentage.</param>
        /// <param name="Percentage">The parsed percentage.</param>
        public static Boolean TryParse(String Text, out PercentageByte Percentage)
        {

            try
            {

                Text = Text.Trim();

                if (Byte.TryParse(Text, out var value) &&
                    value <= 100)
                {

                    Percentage = new PercentageByte(value);

                    return true;

                }

            }
            catch
            { }

            Percentage = default;
            return false;

        }

        #endregion

        #region (static) TryParseXXX(Number, out Percentage)

        /// <summary>
        /// Parse the given number as a percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a percentage.</param>
        /// <param name="Percentage">The parsed Percentage.</param>
        public static Boolean TryParseDouble(Double Number, out PercentageByte Percentage)
        {

            try
            {

                if (Number >=   0 &&
                    Number <= 100)
                {

                    Percentage = new PercentageByte((Byte) Math.Round(Number, 0));

                    return true;

                }

            }
            catch
            { }

            Percentage = default;
            return false;

        }


        /// <summary>
        /// Parse the given number as a percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a percentage.</param>
        /// <param name="Percentage">The parsed Percentage.</param>
        public static Boolean TryParseDecimal(Decimal Number, out PercentageByte Percentage)
        {

            try
            {

                if (Number >=   0 &&
                    Number <= 100)
                {

                    Percentage = new PercentageByte(Convert.ToByte(Number));

                    return true;

                }

            }
            catch
            { }

            Percentage = default;
            return false;

        }



        /// <summary>
        /// Parse the given number as a percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a percentage.</param>
        /// <param name="Percentage">The parsed Percentage.</param>
        public static Boolean TryParse(Byte Number, out PercentageByte Percentage)
        {

            try
            {

                if (Number <= 100)
                {

                    Percentage = new PercentageByte(Number);

                    return true;

                }

            }
            catch
            { }

            Percentage = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this Percentage.
        /// </summary>
        public PercentageByte Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (Percentage1, Percentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Percentage1">A percentage.</param>
        /// <param name="Percentage2">Another percentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PercentageByte Percentage1,
                                           PercentageByte Percentage2)

            => Percentage1.Equals(Percentage2);

        #endregion

        #region Operator != (Percentage1, Percentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Percentage1">A percentage.</param>
        /// <param name="Percentage2">Another percentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PercentageByte Percentage1,
                                           PercentageByte Percentage2)

            => !Percentage1.Equals(Percentage2);

        #endregion

        #region Operator <  (Percentage1, Percentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Percentage1">A percentage.</param>
        /// <param name="Percentage2">Another percentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PercentageByte Percentage1,
                                          PercentageByte Percentage2)

            => Percentage1.CompareTo(Percentage2) < 0;

        #endregion

        #region Operator <= (Percentage1, Percentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Percentage1">A percentage.</param>
        /// <param name="Percentage2">Another percentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PercentageByte Percentage1,
                                           PercentageByte Percentage2)

            => Percentage1.CompareTo(Percentage2) <= 0;

        #endregion

        #region Operator >  (Percentage1, Percentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Percentage1">A percentage.</param>
        /// <param name="Percentage2">Another percentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PercentageByte Percentage1,
                                          PercentageByte Percentage2)

            => Percentage1.CompareTo(Percentage2) > 0;

        #endregion

        #region Operator >= (Percentage1, Percentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Percentage1">A percentage.</param>
        /// <param name="Percentage2">Another percentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PercentageByte Percentage1,
                                           PercentageByte Percentage2)

            => Percentage1.CompareTo(Percentage2) >= 0;

        #endregion

        #region Operator +  (Percentage1, Percentage2)

        /// <summary>
        /// Accumulates two percentages.
        /// </summary>
        /// <param name="Percentage1">A percentage.</param>
        /// <param name="Percentage2">Another percentage.</param>
        public static PercentageByte operator + (PercentageByte Percentage1,
                                                 PercentageByte Percentage2)

            => Parse((Byte) Math.Min(Percentage1.Value + Percentage2.Value, 100));

        #endregion

        #region Operator -  (Percentage1, Percentage2)

        /// <summary>
        /// Substracts two percentages.
        /// </summary>
        /// <param name="Percentage1">A percentage.</param>
        /// <param name="Percentage2">Another percentage.</param>
        public static PercentageByte operator - (PercentageByte Percentage1,
                                                 PercentageByte Percentage2)

            => Parse((Byte) Math.Max(Percentage1.Value - Percentage2.Value, 0));

        #endregion

        #endregion

        #region IComparable<PercentageInt> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two percentages.
        /// </summary>
        /// <param name="Object">A percentage to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PercentageByte percentage
                   ? CompareTo(percentage)
                   : throw new ArgumentException("The given object is not a percentage!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Percentage)

        /// <summary>
        /// Compares two percentages.
        /// </summary>
        /// <param name="Percentage">A percentage to compare with.</param>
        public Int32 CompareTo(PercentageByte Percentage)

            => Value.CompareTo(Percentage.Value);

        #endregion

        #endregion

        #region IEquatable<Percentage> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two percentages for equality.
        /// </summary>
        /// <param name="Object">A percentage to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PercentageByte percentage &&
                   Equals(percentage);

        #endregion

        #region Equals(Percentage)

        /// <summary>
        /// Compares two percentages for equality.
        /// </summary>
        /// <param name="Percentage">A percentage to compare with.</param>
        public Boolean Equals(PercentageByte Percentage)

            => Value.Equals(Percentage.Value);

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

            => $"{Value} %";

        #endregion

    }

}
