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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A signedPercentage between -100% and +100%
    /// </summary>
    public readonly struct SignedPercentage : IEquatable <SignedPercentage>,
                                              IComparable<SignedPercentage>,
                                              IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the signedPercentage.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The value of the signedPercentage as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Math.Round(Value);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signedPercentage based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a signed percentage.</param>
        private SignedPercentage(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given string as a signed percentage.
        /// </summary>
        /// <param name="Text">A text representation of a signed percentage.</param>
        public static SignedPercentage Parse(String Text)
        {

            if (TryParse(Text, out var signedPercentage))
                return signedPercentage;

            throw new ArgumentException($"Invalid text representation of a signed percentage: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse    (Number)

        /// <summary>
        /// Parse the given number as a signed percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a signed percentage.</param>
        public static SignedPercentage Parse(Decimal Number)
        {

            if (TryParse(Number, out var signedPercentage))
                return signedPercentage;

            throw new ArgumentException($"Invalid numeric representation of a signed percentage: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a signed percentage.
        /// </summary>
        /// <param name="Text">A text representation of a signed percentage.</param>
        public static SignedPercentage? TryParse(String Text)
        {

            if (TryParse(Text, out var signedPercentage))
                return signedPercentage;

            return null;

        }

        #endregion

        #region (static) TryParse (Number)

        /// <summary>
        /// Try to parse the given number as a signed percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a signed percentage.</param>
        public static SignedPercentage? TryParse(Decimal Number)
        {

            if (TryParse(Number, out var signedPercentage))
                return signedPercentage;

            return null;

        }

        #endregion

        #region (static) TryParse (Text,   out SignedPercentage)

        /// <summary>
        /// Parse the given string as a signed percentage.
        /// </summary>
        /// <param name="Text">A text representation of a signed percentage.</param>
        /// <param name="SignedPercentage">The parsed signedPercentage.</param>
        public static Boolean TryParse(String Text, out SignedPercentage SignedPercentage)
        {

            try
            {

                Text = Text.Trim();

                if (Decimal.TryParse(Text, out var value) &&
                    value >= -100 &&
                    value <=  100)
                {

                    SignedPercentage = new SignedPercentage(value);

                    return true;

                }

            }
            catch
            { }

            SignedPercentage = default;
            return false;

        }

        #endregion

        #region (static) TryParse (Number, out SignedPercentage)

        /// <summary>
        /// Parse the given number as a signed percentage.
        /// </summary>
        /// <param name="Number">A numeric representation of a signed percentage.</param>
        /// <param name="SignedPercentage">The parsed SignedPercentage.</param>
        public static Boolean TryParse(Decimal Number, out SignedPercentage SignedPercentage)
        {

            try
            {

                if (Number >= -100 &&
                    Number <=  100)
                {

                    SignedPercentage = new SignedPercentage(Number);

                    return true;

                }

            }
            catch
            { }

            SignedPercentage = default;
            return false;

        }

        #endregion


        #region (static) Parse    (Number, StdDev)

        /// <summary>
        /// Parse the given number as a signed percentage with standard deviation.
        /// </summary>
        /// <param name="Number">A numeric representation of a signed percentage.</param>
        /// <param name="StdDev">The standard deviation of the value.</param>
        public static StdDev<SignedPercentage> Parse(Decimal Number,
                                                     Decimal StdDev)
        {

            if (TryParse(Number, StdDev, out var signedPercentage))
                return signedPercentage;

            throw new ArgumentException($"Invalid numeric representation of a signed percentage with standard deviation: '{Number} {StdDev}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse (Number, StdDev, out SignedPercentage, NumberExponent = null, StdDevExponent = null)

        /// <summary>
        /// Parse the given number as a signed percentage with standard deviation.
        /// </summary>
        /// <param name="Number">A numeric representation of a signed percentage.</param>
        /// <param name="StdDev">The standard deviation of the value.</param>
        /// <param name="SignedPercentage">The parsed signedPercentage with standard deviation.</param>
        /// <param name="NumberExponent">An optional 10^exponent for the number.</param>
        /// <param name="StdDevExponent">An optional 10^exponent for the standard deviation.</param>
        public static Boolean TryParse(Decimal                       Number,
                                       Decimal                       StdDev,
                                       out StdDev<SignedPercentage>  SignedPercentage,
                                       Int32?                        NumberExponent = null,
                                       Int32?                        StdDevExponent = null)
        {

            try
            {
                if (TryParse(Number, out var number) &&
                    TryParse(StdDev, out var stddev))
                {

                    SignedPercentage = new StdDev<SignedPercentage>(number, stddev);

                    return true;

                }
            }
            catch
            { }

            SignedPercentage = default;
            return false;

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this SignedPercentage.
        /// </summary>
        public SignedPercentage Clone()

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (SignedPercentage1, SignedPercentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedPercentage1">A signedPercentage.</param>
        /// <param name="SignedPercentage2">Another signedPercentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignedPercentage SignedPercentage1,
                                           SignedPercentage SignedPercentage2)

            => SignedPercentage1.Equals(SignedPercentage2);

        #endregion

        #region Operator != (SignedPercentage1, SignedPercentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedPercentage1">A signedPercentage.</param>
        /// <param name="SignedPercentage2">Another signedPercentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignedPercentage SignedPercentage1,
                                           SignedPercentage SignedPercentage2)

            => !SignedPercentage1.Equals(SignedPercentage2);

        #endregion

        #region Operator <  (SignedPercentage1, SignedPercentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedPercentage1">A signedPercentage.</param>
        /// <param name="SignedPercentage2">Another signedPercentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SignedPercentage SignedPercentage1,
                                          SignedPercentage SignedPercentage2)

            => SignedPercentage1.CompareTo(SignedPercentage2) < 0;

        #endregion

        #region Operator <= (SignedPercentage1, SignedPercentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedPercentage1">A signedPercentage.</param>
        /// <param name="SignedPercentage2">Another signedPercentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SignedPercentage SignedPercentage1,
                                           SignedPercentage SignedPercentage2)

            => SignedPercentage1.CompareTo(SignedPercentage2) <= 0;

        #endregion

        #region Operator >  (SignedPercentage1, SignedPercentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedPercentage1">A signedPercentage.</param>
        /// <param name="SignedPercentage2">Another signedPercentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SignedPercentage SignedPercentage1,
                                          SignedPercentage SignedPercentage2)

            => SignedPercentage1.CompareTo(SignedPercentage2) > 0;

        #endregion

        #region Operator >= (SignedPercentage1, SignedPercentage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedPercentage1">A signedPercentage.</param>
        /// <param name="SignedPercentage2">Another signedPercentage.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SignedPercentage SignedPercentage1,
                                           SignedPercentage SignedPercentage2)

            => SignedPercentage1.CompareTo(SignedPercentage2) >= 0;

        #endregion

        #region Operator +  (SignedPercentage1, SignedPercentage2)

        /// <summary>
        /// Accumulates two signedPercentages.
        /// </summary>
        /// <param name="SignedPercentage1">A signedPercentage.</param>
        /// <param name="SignedPercentage2">Another signedPercentage.</param>
        public static SignedPercentage operator + (SignedPercentage SignedPercentage1,
                                             SignedPercentage SignedPercentage2)

            => Parse(Math.Min(SignedPercentage1.Value + SignedPercentage2.Value, 100));

        #endregion

        #region Operator -  (SignedPercentage1, SignedPercentage2)

        /// <summary>
        /// Substracts two signedPercentages.
        /// </summary>
        /// <param name="SignedPercentage1">A signedPercentage.</param>
        /// <param name="SignedPercentage2">Another signedPercentage.</param>
        public static SignedPercentage operator - (SignedPercentage SignedPercentage1,
                                             SignedPercentage SignedPercentage2)

            => Parse(Math.Max(SignedPercentage1.Value - SignedPercentage2.Value, 0));

        #endregion

        #endregion

        #region IComparable<SignedPercentage> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two signedPercentages.
        /// </summary>
        /// <param name="Object">A signedPercentage to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SignedPercentage signedPercentage
                   ? CompareTo(signedPercentage)
                   : throw new ArgumentException("The given object is not a signed percentage!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SignedPercentage)

        /// <summary>
        /// Compares two signedPercentages.
        /// </summary>
        /// <param name="SignedPercentage">A signedPercentage to compare with.</param>
        public Int32 CompareTo(SignedPercentage SignedPercentage)

            => Value.CompareTo(SignedPercentage.Value);

        #endregion

        #endregion

        #region IEquatable<SignedPercentage> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signedPercentages for equality.
        /// </summary>
        /// <param name="Object">A signedPercentage to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedPercentage signedPercentage &&
                   Equals(signedPercentage);

        #endregion

        #region Equals(SignedPercentage)

        /// <summary>
        /// Compares two signedPercentages for equality.
        /// </summary>
        /// <param name="SignedPercentage">A signedPercentage to compare with.</param>
        public Boolean Equals(SignedPercentage SignedPercentage)

            => Value.Equals(SignedPercentage.Value);

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
