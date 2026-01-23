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
    /// Extension methods for Meters.
    /// </summary>
    public static class MeterExtensions
    {

        /// <summary>
        /// The sum of the given Meter values.
        /// </summary>
        /// <param name="Meters">An enumeration of Meter values.</param>
        public static Meter Sum(this IEnumerable<Meter> Meters)
        {

            var sum = Meter.Zero;

            foreach (var meter in Meters)
                sum = sum + meter;

            return sum;

        }

    }


    /// <summary>
    /// A Meter.
    /// </summary>
    public readonly struct Meter : IEquatable<Meter>,
                                   IComparable<Meter>,
                                   IComparable
    {

        #region Properties

        /// <summary>
        /// The value of the Meter.
        /// </summary>
        public Decimal  Value    { get; }

        /// <summary>
        /// The value of the Meter as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Math.Round(Value);


        /// <summary>
        /// The value as centimeters.
        /// </summary>
        public Decimal  CM
            => Value * 100;

        /// <summary>
        /// The value as decimeters.
        /// </summary>
        public Decimal  DM
            => Value * 10;

        /// <summary>
        /// The value as KiloMeters.
        /// </summary>
        public Decimal  KM
            => Value / 1000;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Meter based on the given number.
        /// </summary>
        /// <param name="Value">A numeric representation of a Meter.</param>
        private Meter(Decimal Value)
        {

            this.Value = Value >= 0
                             ? Value
                             : 0;

        }

        #endregion


        #region (static) Parse      (Text)

        /// <summary>
        /// Parse the given string as a Meter.
        /// </summary>
        /// <param name="Text">A text representation of a Meter.</param>
        public static Meter Parse(String Text)
        {

            if (TryParse(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of a Meter: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseCM    (Text)

        /// <summary>
        /// Parse the given string as centimeters.
        /// </summary>
        /// <param name="Text">A text representation of a centimeter.</param>
        public static Meter ParseCM(String Text)
        {

            if (TryParseCM(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of a centimeter: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseDM    (Text)

        /// <summary>
        /// Parse the given string as decimeters.
        /// </summary>
        /// <param name="Text">A text representation of a decimeter.</param>
        public static Meter ParseDM(String Text)
        {

            if (TryParseDM(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of a decimeter: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseM     (Text)

        /// <summary>
        /// Parse the given string as a Meter.
        /// </summary>
        /// <param name="Text">A text representation of a Meter.</param>
        public static Meter ParseM(String Text)
        {

            if (TryParseM(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of a Meter: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) ParseKM    (Text)

        /// <summary>
        /// Parse the given string as a km.
        /// </summary>
        /// <param name="Text">A text representation of a km.</param>
        public static Meter ParseKM(String Text)
        {

            if (TryParseKM(Text, out var meter))
                return meter;

            throw new ArgumentException($"Invalid text representation of a km: '{Text}'!",
                                        nameof(Text));

        }

        #endregion


        #region (static) ParseCM    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseCM(Decimal  Number,
                                    Int32?   Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a km: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseCM(Double  Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a km: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseCM(Byte    Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a km: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseDM    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseDM(Decimal  Number,
                                    Int32?   Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a km: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseDM(Double  Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a km: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseDM(Byte    Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a km: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseM     (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseM(Decimal  Number,
                                   Int32?   Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a Meter: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseM(Double  Number,
                                   Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a Meter: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseM(Byte    Number,
                                   Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a Meter: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) ParseKM    (Number, Exponent = null)

        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseKM(Decimal  Number,
                                    Int32?   Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a km: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseKM(Double  Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a km: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter ParseKM(Byte    Number,
                                    Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a km: '{Number}'!",
                                        nameof(Number));

        }

        #endregion


        #region (static) TryParse   (Text)

        /// <summary>
        /// Try to parse the given text as a Meter.
        /// </summary>
        /// <param name="Text">A text representation of a Meter.</param>
        public static Meter? TryParse(String Text)
        {

            if (TryParse(Text, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParseCM (Text)

        /// <summary>
        /// Try to parse the given text as a km.
        /// </summary>
        /// <param name="Text">A text representation of a km.</param>
        public static Meter? TryParseCM(String Text)
        {

            if (TryParseCM(Text, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParseDM (Text)

        /// <summary>
        /// Try to parse the given text as a km.
        /// </summary>
        /// <param name="Text">A text representation of a km.</param>
        public static Meter? TryParseDM(String Text)
        {

            if (TryParseDM(Text, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParseM  (Text)

        /// <summary>
        /// Try to parse the given text as a Meter.
        /// </summary>
        /// <param name="Text">A text representation of a Meter.</param>
        public static Meter? TryParseM(String Text)
        {

            if (TryParseM(Text, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParseKM (Text)

        /// <summary>
        /// Try to parse the given text as a km.
        /// </summary>
        /// <param name="Text">A text representation of a km.</param>
        public static Meter? TryParseKM(String Text)
        {

            if (TryParseKM(Text, out var meter))
                return meter;

            return null;

        }

        #endregion


        #region (static) TryParseCM (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseCM(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseCM(Double  Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseCM(Number, out var meter, Exponent))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseCM(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseCM(Number, out var meter, Exponent))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParseDM (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseDM(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseDM(Double  Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseDM(Number, out var meter, Exponent))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseDM(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseDM(Number, out var meter, Exponent))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParseM  (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseM(Decimal  Number,
                                       Int32?   Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseM(Double  Number,
                                       Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseM(Byte    Number,
                                       Int32?  Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParseKM (Number, Exponent = null)

        /// <summary>
        /// Try to parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseKM(Decimal  Number,
                                        Int32?   Exponent = null)
        {

            if (TryParseM(Number, out var meter, Exponent))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseKM(Double  Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseKM(Number, out var meter, Exponent))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Meter? TryParseKM(Byte    Number,
                                        Int32?  Exponent = null)
        {

            if (TryParseKM(Number, out var meter, Exponent))
                return meter;

            return null;

        }

        #endregion


        #region (static) TryParse   (Text,   out Meter)

        /// <summary>
        /// Parse the given string as a Meter.
        /// </summary>
        /// <param name="Text">A text representation of a Meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(String Text, out Meter Meter)
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

                    Meter = new Meter(factor * value);

                    return true;

                }

            }
            catch
            { }

            Meter = default;
            return false;

        }

        #endregion

        #region (static) TryParseCM (Text,   out Meter)

        /// <summary>
        /// Parse the given string as a km.
        /// </summary>
        /// <param name="Text">A text representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        public static Boolean TryParseCM(String Text, out Meter Meter)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Meter = new Meter(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Meter = default;
            return false;

        }

        #endregion

        #region (static) TryParseDM (Text,   out Meter)

        /// <summary>
        /// Parse the given string as a km.
        /// </summary>
        /// <param name="Text">A text representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        public static Boolean TryParseDM(String Text, out Meter Meter)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Meter = new Meter(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Meter = default;
            return false;

        }

        #endregion

        #region (static) TryParseM  (Text,   out Meter)

        /// <summary>
        /// Parse the given string as a Meter.
        /// </summary>
        /// <param name="Text">A text representation of a Meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParseM(String Text, out Meter Meter)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Meter = new Meter(value);

                    return true;

                }

            }
            catch
            { }

            Meter = default;
            return false;

        }

        #endregion

        #region (static) TryParseKM (Text,   out Meter)

        /// <summary>
        /// Parse the given string as a km.
        /// </summary>
        /// <param name="Text">A text representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        public static Boolean TryParseKM(String Text, out Meter Meter)
        {

            try
            {

                if (Decimal.TryParse(Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) &&
                    value >= 0)
                {

                    Meter = new Meter(1000 * value);

                    return true;

                }

            }
            catch
            { }

            Meter = default;
            return false;

        }

        #endregion


        #region (static) TryParseCM (Number, out Meter, Exponent = null)

        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseCM(Byte       Number,
                                         out Meter  Meter,
                                         Int32?     Exponent = null)
        {

            Meter = new Meter(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

            if (Number < 0)
                return false;

            return true;

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseCM(Double     Number,
                                         out Meter  Meter,
                                         Int32?     Exponent = null)
        {

            try
            {

                Meter = new Meter(1000 * (Decimal) (Number * Math.Pow(10, Exponent ?? 0)));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Meter = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        public static Boolean TryParseCM(Decimal    Number,
                                         out Meter  Meter,
                                         Int32?     Exponent = null)
        {

            try
            {

                Meter = new Meter(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Meter = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseDM (Number, out Meter, Exponent = null)

        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseDM(Byte       Number,
                                         out Meter  Meter,
                                         Int32?     Exponent = null)
        {

            Meter = new Meter(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

            if (Number < 0)
                return false;

            return true;

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseDM(Double     Number,
                                         out Meter  Meter,
                                         Int32?     Exponent = null)
        {

            try
            {

                Meter = new Meter(1000 * (Decimal) (Number * Math.Pow(10, Exponent ?? 0)));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Meter = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        public static Boolean TryParseDM(Decimal    Number,
                                         out Meter  Meter,
                                         Int32?     Exponent = null)
        {

            try
            {

                Meter = new Meter(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Meter = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseM  (Number, out Meter, Exponent = null)

        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseM(Byte       Number,
                                        out Meter  Meter,
                                        Int32?     Exponent = null)
        {

            Meter = new Meter(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

            if (Number < 0)
                return false;

            return true;

        }


        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseM(Double     Number,
                                        out Meter  Meter,
                                        Int32?     Exponent = null)
        {

            try
            {

                Meter = new Meter((Decimal) (Number * Math.Pow(10, Exponent ?? 0)));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Meter = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParseM(Decimal    Number,
                                        out Meter  Meter,
                                        Int32?     Exponent = null)
        {

            try
            {

                Meter = new Meter(Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Meter = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParseKM (Number, out Meter, Exponent = null)

        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKM(Byte       Number,
                                         out Meter  Meter,
                                         Int32?     Exponent = null)
        {

            Meter = new Meter(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

            if (Number < 0)
                return false;

            return true;

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        /// <param name="Exponent">An optional 10^exponent.</param>
        public static Boolean TryParseKM(Double     Number,
                                         out Meter  Meter,
                                         Int32?     Exponent = null)
        {

            try
            {

                Meter = new Meter(1000 * (Decimal) (Number * Math.Pow(10, Exponent ?? 0)));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Meter = default;
                return false;
            }

        }


        /// <summary>
        /// Parse the given number as a km.
        /// </summary>
        /// <param name="Number">A numeric representation of a km.</param>
        /// <param name="Meter">The parsed km.</param>
        public static Boolean TryParseKM(Decimal    Number,
                                         out Meter  Meter,
                                         Int32?     Exponent = null)
        {

            try
            {

                Meter = new Meter(1000 * Number * (Decimal) Math.Pow(10, Exponent ?? 0));

                if (Number < 0)
                    return false;

                return true;

            }
            catch
            {
                Meter = default;
                return false;
            }

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this Meter.
        /// </summary>
        public Meter Clone()

            => new (Value);

        #endregion


        public static Meter Zero
            => new (0);


        #region Operator overloading

        #region Operator == (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Meter Meter1,
                                           Meter Meter2)

            => Meter1.Equals(Meter2);

        #endregion

        #region Operator != (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Meter Meter1,
                                           Meter Meter2)

            => !Meter1.Equals(Meter2);

        #endregion

        #region Operator <  (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Meter Meter1,
                                          Meter Meter2)

            => Meter1.CompareTo(Meter2) < 0;

        #endregion

        #region Operator <= (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Meter Meter1,
                                           Meter Meter2)

            => Meter1.CompareTo(Meter2) <= 0;

        #endregion

        #region Operator >  (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Meter Meter1,
                                          Meter Meter2)

            => Meter1.CompareTo(Meter2) > 0;

        #endregion

        #region Operator >= (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Meter Meter1,
                                           Meter Meter2)

            => Meter1.CompareTo(Meter2) >= 0;

        #endregion

        #region Operator +  (Meter1, Meter2)

        /// <summary>
        /// Accumulates two Meters.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        public static Meter operator + (Meter Meter1,
                                        Meter Meter2)

            => new (Meter1.Value + Meter2.Value);

        #endregion

        #region Operator -  (Meter1, Meter2)

        /// <summary>
        /// Substracts two Meters.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        public static Meter operator - (Meter Meter1,
                                        Meter Meter2)

            => new (Meter1.Value - Meter2.Value);

        #endregion

        #endregion

        #region IComparable<Meter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two meters.
        /// </summary>
        /// <param name="Object">A meter to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Meter meter
                   ? CompareTo(meter)
                   : throw new ArgumentException("The given object is not a Meter!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Meter)

        /// <summary>
        /// Compares two meters.
        /// </summary>
        /// <param name="Meter">A meter to compare with.</param>
        public Int32 CompareTo(Meter Meter)

            => Value.CompareTo(Meter.Value);

        #endregion

        #endregion

        #region IEquatable<Meter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two meters for equality.
        /// </summary>
        /// <param name="Object">A meter to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Meter meter &&
                   Equals(meter);

        #endregion

        #region Equals(Meter)

        /// <summary>
        /// Compares two meters for equality.
        /// </summary>
        /// <param name="Meter">A meter to compare with.</param>
        public Boolean Equals(Meter Meter)

            => Value.Equals(Meter.Value);

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
