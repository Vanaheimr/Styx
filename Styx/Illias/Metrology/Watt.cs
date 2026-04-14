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
    /// Extension methods for Watts.
    /// </summary>
    public static class WattExtensions
    {

        /// <summary>
        /// The sum of the given Watt values.
        /// </summary>
        /// <param name="Watts">An enumeration of Watt values.</param>
        public static Watt Sum(this IEnumerable<Watt> Watts)
        {

            var sum = Watt.Zero;

            foreach (var watt in Watts)
                sum = sum + watt;

            return sum;

        }

    }


    /// <summary>
    /// A Watt value.
    /// </summary>
    public readonly struct Watt : IEquatable <Watt>,
                                  IComparable<Watt>,
                                  IComparable,
                                  IAdditionOperators   <Watt, Watt,    Watt>,
                                  ISubtractionOperators<Watt, Watt,    Watt>,
                                  IDivisionOperators   <Watt, Volt,    Ampere>,
                                  IMultiplyOperators   <Watt, Decimal, Watt>,
                                  IDivisionOperators   <Watt, Decimal, Watt>
    {

        #region Properties

        /// <summary>
        /// The zero value of the Watt.
        /// </summary>
        public static readonly Watt Zero = new (0m);

        /// <summary>
        /// The value of the Watt.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The rounded integer value of the Watt.
        /// </summary>
        public Int32    RoundedIntegerValue

            => Decimal.ToInt32(
                   Decimal.Round(Value, 0, MidpointRounding.AwayFromZero)
               );


#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// The value as kiloWatts.
        /// </summary>
        public Decimal  kW
            => Value / 1000m;
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The value as MegaWatts.
        /// </summary>
        public Decimal  MW
            => Value / 1000000m;

        /// <summary>
        /// The value as GigaWatts.
        /// </summary>
        public Decimal  GW
            => Value / 1000000000m;

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


        #region (static) Parse      (Text)

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

        #region (static) ParseW     (Text)

        /// <summary>
        /// Parse the given string as a W.
        /// </summary>
        /// <param name="Text">A text representation of a W.</param>
        public static Watt ParseW(String Text)
        {

            if (TryParseW(Text, out var watt))
                return watt;

            throw new ArgumentException($"Invalid text representation of a Watt: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKW    (Text)

        /// <summary>
        /// Parse the given string as a kW.
        /// </summary>
        /// <param name="Text">A text representation of a kW.</param>
        public static Watt ParseKW(String Text)
        {

            if (TryParseKW(Text, out var watt))
                return watt;

            throw new ArgumentException($"Invalid text representation of a kW: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseMW    (Text)

        /// <summary>
        /// Parse the given string as a MW.
        /// </summary>
        /// <param name="Text">A text representation of a MW.</param>
        public static Watt ParseMW(String Text)
        {

            if (TryParseMW(Text, out var watt))
                return watt;

            throw new ArgumentException($"Invalid text representation of a MW: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseGW    (Text)

        /// <summary>
        /// Parse the given string as a GW.
        /// </summary>
        /// <param name="Text">A text representation of a GW.</param>
        public static Watt ParseGW(String Text)
        {

            if (TryParseGW(Text, out var watt))
                return watt;

            throw new ArgumentException($"Invalid text representation of a GW: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) FromW      (Number, Exponent = null)

        /// <summary>
        /// From the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromW(Decimal  Number,
                                 Int32?   Exponent = null)
        {

            if (TryFromW(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a Watt: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// From the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromW(Byte    Number,
                                 Int32?  Exponent = null)
        {

            if (TryFromW(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a Watt: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) FromKW     (Number, Exponent = null)

        /// <summary>
        /// From the given number as a kW.
        /// </summary>
        /// <param name="Number">A numeric representation of a kW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromKW(Decimal  Number,
                                  Int32?   Exponent = null)
        {

            if (TryFromKW(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a kW: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// From the given number as a kW.
        /// </summary>
        /// <param name="Number">A numeric representation of a kW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromKW(Byte    Number,
                                  Int32?  Exponent = null)
        {

            if (TryFromKW(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a kW: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) FromMW     (Number, Exponent = null)

        /// <summary>
        /// From the given number as a MW.
        /// </summary>
        /// <param name="Number">A numeric representation of a MW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromMW(Decimal  Number,
                                  Int32?   Exponent = null)
        {

            if (TryFromMW(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a MW: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// From the given number as a MW.
        /// </summary>
        /// <param name="Number">A numeric representation of a MW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromMW(Byte    Number,
                                  Int32?  Exponent = null)
        {

            if (TryFromMW(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a MW: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) FromGW     (Number, Exponent = null)

        /// <summary>
        /// From the given number as a GW.
        /// </summary>
        /// <param name="Number">A numeric representation of a GW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromGW(Decimal  Number,
                                  Int32?   Exponent = null)
        {

            if (TryFromGW(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a GW: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// From the given number as a GW.
        /// </summary>
        /// <param name="Number">A numeric representation of a GW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt FromGW(Byte    Number,
                                  Int32?  Exponent = null)
        {

            if (TryFromGW(Number, out var watt, Exponent))
                return watt;

            throw new ArgumentException($"Invalid numeric representation of a GW: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse   (Text)

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

        #region (static) TryParseW  (Text)

        /// <summary>
        /// Try to parse the given text as a W.
        /// </summary>
        /// <param name="Text">A text representation of a W.</param>
        public static Watt? TryParseW(String Text)
        {

            if (TryParseW(Text, out var watt))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParseKW (Text)

        /// <summary>
        /// Try to parse the given text as a kW.
        /// </summary>
        /// <param name="Text">A text representation of a kW.</param>
        public static Watt? TryParseKW(String Text)
        {

            if (TryParseKW(Text, out var watt))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParseMW (Text)

        /// <summary>
        /// Try to parse the given text as a MW.
        /// </summary>
        /// <param name="Text">A text representation of a MW.</param>
        public static Watt? TryParseMW(String Text)
        {

            if (TryParseMW(Text, out var watt))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryParseGW (Text)

        /// <summary>
        /// Try to parse the given text as a GW.
        /// </summary>
        /// <param name="Text">A text representation of a GW.</param>
        public static Watt? TryParseGW(String Text)
        {

            if (TryParseGW(Text, out var watt))
                return watt;

            return null;

        }

        #endregion


        #region (static) TryFromW   (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromW(Decimal  Number,
                                     Int32?   Exponent = null)
        {

            if (TryFromW(Number, out var watt, Exponent))
                return watt;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromW(Byte    Number,
                                     Int32?  Exponent = null)
        {

            if (TryFromW(Number, out var watt, Exponent))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryFromKW  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a kW.
        /// </summary>
        /// <param name="Number">A numeric representation of a kW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromKW(Decimal  Number,
                                      Int32?   Exponent = null)
        {

            if (TryFromKW(Number, out var watt, Exponent))
                return watt;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a kW.
        /// </summary>
        /// <param name="Number">A numeric representation of a kW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromKW(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryFromKW(Number, out var watt, Exponent))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryFromMW  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a MW.
        /// </summary>
        /// <param name="Number">A numeric representation of a MW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromMW(Decimal  Number,
                                      Int32?   Exponent = null)
        {

            if (TryFromMW(Number, out var watt, Exponent))
                return watt;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a MW.
        /// </summary>
        /// <param name="Number">A numeric representation of a MW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromMW(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryFromMW(Number, out var watt, Exponent))
                return watt;

            return null;

        }

        #endregion

        #region (static) TryFromGW  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a GW.
        /// </summary>
        /// <param name="Number">A numeric representation of a GW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromGW(Decimal  Number,
                                      Int32?   Exponent = null)
        {

            if (TryFromGW(Number, out var watt, Exponent))
                return watt;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a GW.
        /// </summary>
        /// <param name="Number">A numeric representation of a GW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Watt? TryFromGW(Byte    Number,
                                      Int32?  Exponent = null)
        {

            if (TryFromGW(Number, out var watt, Exponent))
                return watt;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,   out Watt)

        /// <summary>
        /// Parse the given string as a Watt.
        /// </summary>
        /// <param name="Text">A text representation of a Watt.</param>
        /// <param name="Watt">The parsed Watt.</param>
        public static Boolean TryParse(String Text, out Watt Watt)
        {

            Watt = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            Text = Text.Trim();

            var factor = 1m;

            if      (Text.EndsWith("kW", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000m;
                Text    = Text[..^2].TrimEnd();
            }

            if      (Text.EndsWith("MW", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000000m;
                Text    = Text[..^2].TrimEnd();
            }

            if      (Text.EndsWith("GW", StringComparison.OrdinalIgnoreCase))
            {
                factor  = 1000000000m;
                Text    = Text[..^2].TrimEnd();
            }

            else if (Text.EndsWith("W",  StringComparison.OrdinalIgnoreCase))
            {
                Text    = Text[..^1].TrimEnd();
            }

            if (Decimal.TryParse(Text,
                                 NumberStyles.Number,
                                 CultureInfo.InvariantCulture,
                                 out var value))
            {

                Watt = new Watt(value * factor);

                return true;

            }

            return false;

        }

        #endregion

        #region (static) TryParseW  (Text,   out Watt)

        /// <summary>
        /// Parse the given string as a W.
        /// </summary>
        /// <param name="Text">A text representation of a W.</param>
        /// <param name="Watt">The parsed W.</param>
        public static Boolean TryParseW(String Text, out Watt Watt)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Watt = new Watt(value);

                    return true;

                }

            }
            catch
            { }

            Watt = default;
            return false;

        }

        #endregion

        #region (static) TryParseKW (Text,   out Watt)

        /// <summary>
        /// Parse the given string as a kW.
        /// </summary>
        /// <param name="Text">A text representation of a kW.</param>
        /// <param name="Watt">The parsed kW.</param>
        public static Boolean TryParseKW(String Text, out Watt Watt)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Watt = new Watt(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Watt = default;
            return false;

        }

        #endregion

        #region (static) TryParseMW (Text,   out Watt)

        /// <summary>
        /// Parse the given string as a MW.
        /// </summary>
        /// <param name="Text">A text representation of a MW.</param>
        /// <param name="Watt">The parsed MW.</param>
        public static Boolean TryParseMW(String Text, out Watt Watt)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Watt = new Watt(1000000 * value);

                    return true;

                }

            }
            catch
            { }

            Watt = default;
            return false;

        }

        #endregion

        #region (static) TryParseGW (Text,   out Watt)

        /// <summary>
        /// Parse the given string as a GW.
        /// </summary>
        /// <param name="Text">A text representation of a GW.</param>
        /// <param name="Watt">The parsed GW.</param>
        public static Boolean TryParseGW(String Text, out Watt Watt)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {

                    Watt = new Watt(1000000000 * value);

                    return true;

                }

            }
            catch
            { }

            Watt = default;
            return false;

        }

        #endregion


        #region (static) TryFromW   (Number, out Watt, Exponent = null)

        /// <summary>
        /// From the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Watt">The parsed Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromW(Byte      Number,
                                       out Watt  Watt,
                                       Int32?    Exponent = null)
        {

            try
            {
                Watt = new Watt(Number * Pow10.Calc(Exponent ?? 0));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                Watt = default;
                return false;
            }
            catch (OverflowException)
            {
                Watt = default;
                return false;
            }

        }


        /// <summary>
        /// From the given number as a Watt.
        /// </summary>
        /// <param name="Number">A numeric representation of a Watt.</param>
        /// <param name="Watt">The parsed Watt.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromW(Decimal   Number,
                                       out Watt  Watt,
                                       Int32?    Exponent = null)
        {

            try
            {
                Watt = new Watt(Number * Pow10.Calc(Exponent ?? 0));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                Watt = default;
                return false;
            }
            catch (OverflowException)
            {
                Watt = default;
                return false;
            }
        }

        #endregion

        #region (static) TryFromKW  (Number, out Watt, Exponent = null)

        /// <summary>
        /// From the given number as a kW.
        /// </summary>
        /// <param name="Number">A numeric representation of a kW.</param>
        /// <param name="Watt">The parsed kW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKW(Byte      Number,
                                        out Watt  Watt,
                                        Int32?    Exponent = null)
        {

            try
            {
                Watt = new Watt(1000 * Number * Pow10.Calc(Exponent ?? 0));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                Watt = default;
                return false;
            }
            catch (OverflowException)
            {
                Watt = default;
                return false;
            }
        }


        /// <summary>
        /// From the given number as a kW.
        /// </summary>
        /// <param name="Number">A numeric representation of a kW.</param>
        /// <param name="Watt">The parsed kW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromKW(Decimal   Number,
                                        out Watt  Watt,
                                        Int32?    Exponent = null)
        {

            try
            {
                Watt = new Watt(1000 * Number * Pow10.Calc(Exponent ?? 0));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                Watt = default;
                return false;
            }
            catch (OverflowException)
            {
                Watt = default;
                return false;
            }
        }

        #endregion

        #region (static) TryFromMW  (Number, out Watt, Exponent = null)

        /// <summary>
        /// From the given number as a MW.
        /// </summary>
        /// <param name="Number">A numeric representation of a MW.</param>
        /// <param name="Watt">The parsed MW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMW(Byte      Number,
                                        out Watt  Watt,
                                        Int32?    Exponent = null)
        {

            try
            {
                Watt = new Watt(1000000 * Number * Pow10.Calc(Exponent ?? 0));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                Watt = default;
                return false;
            }
            catch (OverflowException)
            {
                Watt = default;
                return false;
            }

        }


        /// <summary>
        /// From the given number as a MW.
        /// </summary>
        /// <param name="Number">A numeric representation of a MW.</param>
        /// <param name="Watt">The parsed MW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromMW(Decimal   Number,
                                        out Watt  Watt,
                                        Int32?    Exponent = null)
        {

            try
            {
                Watt = new Watt(1000000 * Number * Pow10.Calc(Exponent ?? 0));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                Watt = default;
                return false;
            }
            catch (OverflowException)
            {
                Watt = default;
                return false;
            }

        }

        #endregion

        #region (static) TryFromGW  (Number, out Watt, Exponent = null)

        /// <summary>
        /// From the given number as a GW.
        /// </summary>
        /// <param name="Number">A numeric representation of a GW.</param>
        /// <param name="Watt">The parsed GW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromGW(Byte      Number,
                                        out Watt  Watt,
                                        Int32?    Exponent = null)
        {

            try
            {
                Watt = new Watt(1000000000 * Number * Pow10.Calc(Exponent ?? 0));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                Watt = default;
                return false;
            }
            catch (OverflowException)
            {
                Watt = default;
                return false;
            }

        }


        /// <summary>
        /// From the given number as a GW.
        /// </summary>
        /// <param name="Number">A numeric representation of a GW.</param>
        /// <param name="Watt">The parsed GW.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryFromGW(Decimal   Number,
                                        out Watt  Watt,
                                        Int32?    Exponent = null)
        {

            try
            {
                Watt = new Watt(1000000000 * Number * Pow10.Calc(Exponent ?? 0));
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                Watt = default;
                return false;
            }
            catch (OverflowException)
            {
                Watt = default;
                return false;
            }

        }

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

            => new (Watt1.Value + Watt2.Value);

        #endregion

        #region Operator -  (Watt1, Watt2)

        /// <summary>
        /// Substracts two Watts.
        /// </summary>
        /// <param name="Watt1">A Watt.</param>
        /// <param name="Watt2">Another Watt.</param>
        public static Watt operator - (Watt Watt1,
                                       Watt Watt2)

            => new (Watt1.Value - Watt2.Value);

        #endregion


        #region Operator /  (Watt,  Volt)

        /// <summary>
        /// Ampere = Watt / Volt
        /// </summary>
        /// <param name="Watt">A Watt.</param>
        /// <param name="Volt">A Volt.</param>
        public static Ampere operator / (Watt Watt,
                                         Volt Volt)

            => Ampere.FromA(Watt.Value / Volt.Value);

        #endregion


        #region Operator *  (Watt,  Scalar)

        /// <summary>
        /// Multiplies a Watt with a scalar.
        /// </summary>
        /// <param name="Watt">A Watt value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Watt operator * (Watt     Watt,
                                       Decimal  Scalar)

            => new (Watt.Value * Scalar);

        #endregion

        #region Operator *  (Scalar,  Watt)

        /// <summary>
        /// Multiplies a scalar with a Watt.
        /// </summary>
        /// <param name="Scalar">A scalar value.</param>
        /// <param name="Watt">A Watt value.</param>
        public static Watt operator * (Decimal  Scalar,
                                       Watt     Watt)

            => new (Scalar * Watt.Value);

        #endregion

        #region Operator /  (Watt,  Scalar)

        /// <summary>
        /// Divides a Watt with a scalar.
        /// </summary>
        /// <param name="Watt">A Watt value.</param>
        /// <param name="Scalar">A scalar value.</param>
        public static Watt operator / (Watt     Watt,
                                       Decimal  Scalar)

            => new (Watt.Value / Scalar);

        #endregion

        #endregion

        #region IComparable<Watt> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two Watts.
        /// </summary>
        /// <param name="Object">A Watt to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null       => 1,
                   Watt watt  => CompareTo(watt),
                   _          => throw new ArgumentException("The given object is not a Watt!", nameof(Object))
               };

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

            => ToString(
                   null,
                   CultureInfo.InvariantCulture
               );


        /// <summary>
        /// Return a text representation of this object using the given
        /// format and culture-specific format provider.
        /// </summary>
        public String ToString(String?           Format,
                               IFormatProvider?  FormatProvider)
        {

            if (String.IsNullOrEmpty(Format) ||
                Format.Equals("G",  StringComparison.OrdinalIgnoreCase) ||
                Format.Equals("W",  StringComparison.OrdinalIgnoreCase))
            {
                return $"{Value.ToString("G", FormatProvider)} W";
            }

            if (Format.Equals("kW", StringComparison.OrdinalIgnoreCase))
            {
                return $"{kW.ToString("G", FormatProvider)} kW";
            }

            if (Format.Equals("MW", StringComparison.OrdinalIgnoreCase))
            {
                return $"{MW.ToString("G", FormatProvider)} MW";
            }

            if (Format.Equals("GW", StringComparison.OrdinalIgnoreCase))
            {
                return $"{GW.ToString("G", FormatProvider)} GW";
            }

            return $"{Value.ToString(Format, FormatProvider)} W";

        }

        #endregion

    }

}
