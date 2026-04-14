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

using System.Numerics;
using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for Volt-Ampere Reactives (VARs).
    /// </summary>
    public static class VoltAmpereReactiveExtensions
    {

        /// <summary>
        /// The sum of the given Volt-Ampere Reactive values.
        /// </summary>
        /// <param name="VoltAmpereReactives">An enumeration of Volt-Ampere Reactive values.</param>
        public static VoltAmpereReactive Sum(this IEnumerable<VoltAmpereReactive> VoltAmpereReactives)
        {

            var sum = VoltAmpereReactive.Zero;

            foreach (var voltAmpereReactive in VoltAmpereReactives)
                sum = sum + voltAmpereReactive;

            return sum;

        }

    }


    /// <summary>
    /// A Volt-Ampere Reactive (VAR) value.
    /// </summary>
    public readonly struct VoltAmpereReactive : IEquatable <VoltAmpereReactive>,
                                                IComparable<VoltAmpereReactive>,
                                                IComparable,
                                                IAdditionOperators   <VoltAmpereReactive, VoltAmpereReactive, VoltAmpereReactive>,
                                                ISubtractionOperators<VoltAmpereReactive, VoltAmpereReactive, VoltAmpereReactive>,
                                                IMultiplyOperators   <VoltAmpereReactive, Decimal,            VoltAmpereReactive>,
                                                IDivisionOperators   <VoltAmpereReactive, Decimal,            VoltAmpereReactive>
    {

        #region Properties

        /// <summary>
        /// The zero value of the VoltAmpereReactive.
        /// </summary>
        public static readonly VoltAmpereReactive Zero = new (0m);

        /// <summary>
        /// The value of the VoltAmpereReactive.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the VoltAmpereReactive.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


        /// <summary>
        /// The value as Kilo-Volt-Ampere Reactive.
        /// </summary>
        public Decimal  KW
            => Value / 1000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Volt-Ampere Reactive (VAR) based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a Volt-Ampere Reactive (VAR).</param>
        private VoltAmpereReactive(Decimal Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse        (Text)

        /// <summary>
        /// Parse the given string as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere Reactive (VAR).</param>
        public static VoltAmpereReactive Parse(String Text)
        {

            if (TryParse(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            throw new ArgumentException($"Invalid text representation of a Volt-Ampere Reactive (VAR): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseVAR     (Text)

        /// <summary>
        /// Parse the given string as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere Reactive (VAR).</param>
        public static VoltAmpereReactive ParseVAR(String Text)
        {

            if (TryParseVAR(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            throw new ArgumentException($"Invalid text representation of a Volt-Ampere Reactive (VAR): '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKVAR    (Text)

        /// <summary>
        /// Parse the given string as a Kilo-Volt-Ampere Reactive (kVAR).
        /// </summary>
        /// <param name="Text">A text representation of a Kilo-Volt-Ampere Reactive (kVAR).</param>
        public static VoltAmpereReactive ParseKVAR(String Text)
        {

            if (TryParseKVAR(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            throw new ArgumentException($"Invalid text representation of a Kilo-Volt-Ampere Reactive (kVAR): '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseVAR     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere Reactive (VAR).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive ParseVAR(Decimal  Number,
                                                  Int32?   Exponent = null)
        {

            if (TryParseVAR(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            throw new ArgumentException($"Invalid numeric representation of a Volt-Ampere Reactive (VAR): '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere Reactive (VAR).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive ParseVAR(Byte    Number,
                                                  Int32?  Exponent = null)
        {

            if (TryParseVAR(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            throw new ArgumentException($"Invalid numeric representation of a Volt-Ampere Reactive (VAR): '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKVAR    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Kilo-Volt-Ampere Reactive (kVAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Volt-Ampere Reactive (kVAR).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive ParseKVAR(Decimal  Number,
                                                   Int32?   Exponent = null)
        {

            if (TryParseKVAR(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            throw new ArgumentException($"Invalid numeric representation of a Kilo-Volt-Ampere Reactive (kVAR): '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Kilo-Volt-Ampere Reactive (kVAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Volt-Ampere Reactive (kVAR).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive ParseKVAR(Byte    Number,
                                                   Int32?  Exponent = null)
        {

            if (TryParseKVAR(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            throw new ArgumentException($"Invalid numeric representation of a Kilo-Volt-Ampere Reactive (kVAR): '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse     (Text)

        /// <summary>
        /// Try to parse the given text as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere Reactive (VAR).</param>
        public static VoltAmpereReactive? TryParse(String Text)
        {

            if (TryParse(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            return null;

        }

        #endregion

        #region (static) TryParseVAR  (Text)

        /// <summary>
        /// Try to parse the given text as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere Reactive (VAR).</param>
        public static VoltAmpereReactive? TryParseVAR(String Text)
        {

            if (TryParseVAR(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            return null;

        }

        #endregion

        #region (static) TryParseKVAR (Text)

        /// <summary>
        /// Try to parse the given text as a Kilo-Volt-Ampere Reactive (kVAR).
        /// </summary>
        /// <param name="Text">A text representation of a Kilo-Volt-Ampere Reactive (kVAR).</param>
        public static VoltAmpereReactive? TryParseKVAR(String Text)
        {

            if (TryParseKVAR(Text, out var voltAmpereReactive))
                return voltAmpereReactive;

            return null;

        }

        #endregion


        #region (static) TryParseVAR  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere Reactive (VAR).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive? TryParseVAR(Decimal  Number,
                                                      Int32?   Exponent = null)
        {

            if (TryParseVAR(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere Reactive (VAR).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive? TryParseVAR(Byte    Number,
                                                      Int32?  Exponent = null)
        {

            if (TryParseVAR(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            return null;

        }

        #endregion

        #region (static) TryParseKVAR (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a kV.
        /// </summary>
        /// <param name="Number">A numeric representation of a kV.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive? TryParseKVAR(Decimal  Number,
                                                       Int32?   Exponent = null)
        {

            if (TryParseKVAR(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Kilo-Volt-Ampere Reactive (kVAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Volt-Ampere Reactive (kVAR).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static VoltAmpereReactive? TryParseKVAR(Byte    Number,
                                                       Int32?  Exponent = null)
        {

            if (TryParseKVAR(Number, out var voltAmpereReactive, Exponent))
                return voltAmpereReactive;

            return null;

        }

        #endregion


        #region (static) TryParse     (Text,   out VoltAmpereReactive)

        /// <summary>
        /// Parse the given string as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        public static Boolean TryParse(String Text, out VoltAmpereReactive VoltAmpereReactive)
        {

            VoltAmpereReactive = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            Text = Text.Trim();

            var factor = 1m;

            if      (Text.EndsWith("kvar", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000m;
                Text    = Text[..^4].TrimEnd();
            }

            else if (Text.EndsWith("var",  StringComparison.OrdinalIgnoreCase))
            {
                Text    = Text[..^3].TrimEnd();
            }

            if (Decimal.TryParse(Text,
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {

                VoltAmpereReactive = new VoltAmpereReactive(value * factor);

                return true;

            }

            return false;

        }

        #endregion

        #region (static) TryParseVAR  (Text,   out VoltAmpereReactive)

        /// <summary>
        /// Parse the given string as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Text">A text representation of a Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        public static Boolean TryParseVAR(String Text, out VoltAmpereReactive VoltAmpereReactive)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    VoltAmpereReactive = new VoltAmpereReactive(value);

                    return true;

                }

            }
            catch
            { }

            VoltAmpereReactive = default;
            return false;

        }

        #endregion

        #region (static) TryParseKVAR (Text,   out VoltAmpereReactive)

        /// <summary>
        /// Parse the given string as a Kilo-Volt-Ampere Reactive (kVAR).
        /// </summary>
        /// <param name="Text">A text representation of a Kilo-Volt-Ampere Reactive (kVAR).</param>
        /// <param name="VoltAmpereReactive">The parsed Kilo-Volt-Ampere Reactive (kVAR).</param>
        public static Boolean TryParseKVAR(String Text, out VoltAmpereReactive VoltAmpereReactive)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    VoltAmpereReactive = new VoltAmpereReactive(1000 * value);

                    return true;

                }

            }
            catch
            { }

            VoltAmpereReactive = default;
            return false;

        }

        #endregion


        #region (static) TryParseVAR  (Number, out VoltAmpereReactive, Exponent = null)

        /// <summary>
        /// Parse the given number as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseVAR(Byte                    Number,
                                          out VoltAmpereReactive  VoltAmpereReactive,
                                          Int32?                  Exponent = null)
        {

            try
            {

                VoltAmpereReactive = new VoltAmpereReactive(Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                VoltAmpereReactive = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseVAR(Decimal                 Number,
                                          out VoltAmpereReactive  VoltAmpereReactive,
                                          Int32?                  Exponent = null)
        {

            try
            {

                VoltAmpereReactive = new VoltAmpereReactive(Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                VoltAmpereReactive = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKVAR (Number, out VoltAmpereReactive, Exponent = null)

        /// <summary>
        /// Parse the given number as a Volt-Ampere Reactive (VAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive">The parsed VoltAmpereReactive.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKVAR(Byte                    Number,
                                           out VoltAmpereReactive  VoltAmpereReactive,
                                           Int32?                  Exponent = null)
        {

            try
            {

                VoltAmpereReactive = new VoltAmpereReactive(Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                VoltAmpereReactive = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Kilo-Volt-Ampere Reactive (kVAR).
        /// </summary>
        /// <param name="Number">A numeric representation of a Kilo-Volt-Ampere Reactive (kVAR).</param>
        /// <param name="VoltAmpereReactive">The parsed Kilo-Volt-Ampere Reactive (kVAR).</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKVAR(Decimal                 Number,
                                           out VoltAmpereReactive  VoltAmpereReactive,
                                           Int32?                  Exponent = null)
        {

            try
            {

                VoltAmpereReactive = new VoltAmpereReactive(Number * Pow10.Calc(Exponent ?? 0));

                return true;

            }
            catch
            {
                VoltAmpereReactive = default;
                return false;
            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive2">Another Volt-Ampere Reactive (VAR).</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VoltAmpereReactive VoltAmpereReactive1,
                                           VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.Equals(VoltAmpereReactive2);

        #endregion

        #region Operator != (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive2">Another Volt-Ampere Reactive (VAR).</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VoltAmpereReactive VoltAmpereReactive1,
                                           VoltAmpereReactive VoltAmpereReactive2)

            => !VoltAmpereReactive1.Equals(VoltAmpereReactive2);

        #endregion

        #region Operator <  (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive2">Another Volt-Ampere Reactive (VAR).</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (VoltAmpereReactive VoltAmpereReactive1,
                                          VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.CompareTo(VoltAmpereReactive2) < 0;

        #endregion

        #region Operator <= (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive2">Another Volt-Ampere Reactive (VAR).</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (VoltAmpereReactive VoltAmpereReactive1,
                                           VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.CompareTo(VoltAmpereReactive2) <= 0;

        #endregion

        #region Operator >  (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive2">Another Volt-Ampere Reactive (VAR).</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (VoltAmpereReactive VoltAmpereReactive1,
                                          VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.CompareTo(VoltAmpereReactive2) > 0;

        #endregion

        #region Operator >= (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive2">Another Volt-Ampere Reactive (VAR).</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (VoltAmpereReactive VoltAmpereReactive1,
                                           VoltAmpereReactive VoltAmpereReactive2)

            => VoltAmpereReactive1.CompareTo(VoltAmpereReactive2) >= 0;

        #endregion

        #region Operator +  (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Accumulates two VoltAmpereReactives.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive2">Another Volt-Ampere Reactive (VAR).</param>
        public static VoltAmpereReactive operator + (VoltAmpereReactive VoltAmpereReactive1,
                                                     VoltAmpereReactive VoltAmpereReactive2)

            => new (VoltAmpereReactive1.Value + VoltAmpereReactive2.Value);

        #endregion

        #region Operator -  (VoltAmpereReactive1, VoltAmpereReactive2)

        /// <summary>
        /// Substracts two VoltAmpereReactives.
        /// </summary>
        /// <param name="VoltAmpereReactive1">A Volt-Ampere Reactive (VAR).</param>
        /// <param name="VoltAmpereReactive2">Another Volt-Ampere Reactive (VAR).</param>
        public static VoltAmpereReactive operator - (VoltAmpereReactive VoltAmpereReactive1,
                                                     VoltAmpereReactive VoltAmpereReactive2)

            => new (VoltAmpereReactive1.Value - VoltAmpereReactive2.Value);

        #endregion


        #region Operator *  (VoltAmpereReactive,  Scalar)

        /// <summary>
        /// Multiplies a VoltAmpereReactive with a scalar.
        /// </summary>
        /// <param name="VoltAmpereReactive">A VoltAmpereReactive value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static VoltAmpereReactive operator * (VoltAmpereReactive  VoltAmpereReactive,
                                                     Decimal             Scalar)

            => new (VoltAmpereReactive.Value * Scalar);

        #endregion

        #region Operator /  (VoltAmpereReactive,  Scalar)

        /// <summary>
        /// Divides a VoltAmpereReactive with a scalar.
        /// </summary>
        /// <param name="VoltAmpereReactive">A VoltAmpereReactive value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static VoltAmpereReactive operator / (VoltAmpereReactive  VoltAmpereReactive,
                                                     Decimal             Scalar)

            => new (VoltAmpereReactive.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<VoltAmpereReactive> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Volt-Ampere Reactives (vars).
        /// </summary>
        /// <param name="Object">A Volt-Ampere Reactive (VAR) to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is VoltAmpereReactive voltAmpereReactive
                   ? CompareTo(voltAmpereReactive)
                   : throw new ArgumentException("The given object is not a Volt-Ampere Reactive (VAR)!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(VoltAmpereReactive)

        /// <summary>
        /// Compares two Volt-Ampere Reactives (vars).
        /// </summary>
        /// <param name="VoltAmpereReactive">A Volt-Ampere Reactive (VAR) to compare with.</param>
        public Int32 CompareTo(VoltAmpereReactive VoltAmpereReactive)

            => Value.CompareTo(VoltAmpereReactive.Value);

        #endregion

        #endregion

        #region IEquatable<VoltAmpereReactive> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Volt-Ampere Reactives (vars) for equality.
        /// </summary>
        /// <param name="Object">A Volt-Ampere Reactive (VAR) to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VoltAmpereReactive voltAmpereReactive &&
                   Equals(voltAmpereReactive);

        #endregion

        #region Equals(VoltAmpereReactive)

        /// <summary>
        /// Compares two Volt-Ampere Reactives (vars) for equality.
        /// </summary>
        /// <param name="VoltAmpereReactive">A Volt-Ampere Reactive (VAR) to compare with.</param>
        public Boolean Equals(VoltAmpereReactive VoltAmpereReactive)

            => Value.Equals(VoltAmpereReactive.Value);

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

            => $"{Value.ToString(CultureInfo.InvariantCulture)} VAR";

        #endregion

    }

}
