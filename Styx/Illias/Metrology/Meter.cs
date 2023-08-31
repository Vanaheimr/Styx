/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
        /// The value of the Amperes as Int32.
        /// </summary>
        public Int32    IntegerValue
            => (Int32) Value;


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
            this.Value = Value;
        }

        #endregion


        #region (static) Parse   (Text)

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

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        public static Meter Parse(Decimal Number)
        {

            if (TryParse(Number, out var meter))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a Meter: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        public static Meter Parse(Double Number)
        {

            if (TryParse(Number, out var meter))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a Meter: '{Number}'!",
                                        nameof(Number));

        }


        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        public static Meter Parse(Byte Number)
        {

            if (TryParse(Number, out var meter))
                return meter;

            throw new ArgumentException($"Invalid numeric representation of a Meter: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse(Text)

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

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        public static Meter? TryParse(Decimal Number)
        {

            if (TryParse(Number, out var meter))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        public static Meter? TryParse(Double Number)
        {

            if (TryParse(Number, out var meter))
                return meter;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        public static Meter? TryParse(Byte Number)
        {

            if (TryParse(Number, out var meter))
                return meter;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out Meter)

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

                if (Decimal.TryParse(Text, out var value))
                {

                    Meter = new Meter(value / factor);

                    return true;

                }

            }
            catch
            { }

            Meter = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out Meter)

        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(Byte Number, out Meter Meter)
        {

            Meter = new Meter(Number);

            return true;

        }


        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(Double Number, out Meter Meter)
        {

            Meter = new Meter(Convert.ToDecimal(Number));

            return true;

        }


        /// <summary>
        /// Parse the given number as a Meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a Meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(Decimal Number, out Meter Meter)
        {

            Meter = new Meter(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this Meter.
        /// </summary>
        public Meter Clone

            => new (Value);

        #endregion


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

            => Meter.Parse(Meter1.Value + Meter2.Value);

        #endregion

        #region Operator -  (Meter1, Meter2)

        /// <summary>
        /// Substracts two Meters.
        /// </summary>
        /// <param name="Meter1">A Meter.</param>
        /// <param name="Meter2">Another Meter.</param>
        public static Meter operator - (Meter Meter1,
                                        Meter Meter2)

            => Meter.Parse(Meter1.Value - Meter2.Value);

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
