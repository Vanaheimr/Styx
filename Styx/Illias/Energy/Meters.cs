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

#region Usings

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A meter.
    /// </summary>
    public readonly struct Meter : IComparable<Meter>,
                                   IEquatable<Meter>
    {

        #region Data

        private readonly Double InternalValue;

        #endregion

        #region Properties

        /// <summary>
        /// The value of the meter.
        /// </summary>
        public Double   Value

            => IsKiloMeters
                   ? InternalValue * 1000
                   : InternalValue;

        /// <summary>
        /// Value is km, not meters.
        /// </summary>
        public Boolean  IsKiloMeters   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter.
        /// </summary>
        private Meter(Double   Value,
                      Boolean  IsKiloMeters = false)
        {
            this.InternalValue  = Value;
            this.IsKiloMeters   = IsKiloMeters;
        }

        #endregion


        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a meter.
        /// </summary>
        /// <param name="Text">A text representation of a meter.</param>
        public static Meter Parse(String Text)
        {

            if (TryParse(Text, out Meter meter))
                return meter;

            throw new ArgumentException("The given text '" + Text + "' is not a valid meter!");

        }

        #endregion

        #region Parse   (Number)

        /// <summary>
        /// Parse the given number as a meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a meter.</param>
        public static Meter Parse(Single Number)

            => Number >= 0
                   ? new Meter(Number, IsKiloMeters: false)
                   : default;


        /// <summary>
        /// Parse the given number as a meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a meter.</param>
        public static Meter Parse(Double Number)

            => Number >= 0
                   ? new Meter(Number, IsKiloMeters: false)
                   : default;

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Parse the given string as a meter.
        /// </summary>
        /// <param name="Text">A text representation of a meter.</param>
        public static Meter? TryParse(String Text)
        {

            if (TryParse(Text, out Meter meter))
                return meter;

            return null;

        }

        #endregion

        #region TryParse(Number)

        /// <summary>
        /// Parse the given number as a meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a meter.</param>
        public static Meter? TryParse(Single Number)

            => Number >= 0
                   ? new Meter(Number, IsKiloMeters: false)
                   : new Meter?();

        /// <summary>
        /// Parse the given number as a meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a meter.</param>
        public static Meter? TryParse(Double Number)

            => Number >= 0
                   ? new Meter(Number, IsKiloMeters: false)
                   : new Meter?();

        #endregion

        #region TryParse(Text,   out Meter)

        /// <summary>
        /// Parse the given string as a meter.
        /// </summary>
        /// <param name="Text">A text representation of a meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(String Text, out Meter Meter)
        {

            Text = Text?.Trim()?.ToLower();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {

                    if      (Text.EndsWith("km") &&
                             Double.TryParse(Text.Substring(0, Text.Length - 2)?.Trim(), out Double meters) &&
                             meters >= 0)
                    {
                        Meter = new Meter(meters, IsKiloMeters: true);
                        return true;
                    }

                    else if (Text.EndsWith("m") &&
                             Double.TryParse(Text.Substring(0, Text.Length - 1)?.Trim(), out meters) &&
                             meters >= 0)
                    {
                        Meter = new Meter(meters, IsKiloMeters: false);
                        return true;
                    }

                    else if (Double.TryParse(Text, out meters) &&
                             meters >= 0)
                    {
                        Meter = new Meter(meters, IsKiloMeters: false);
                        return true;
                    }

                }
                catch
                { }
            }

            Meter = default;
            return false;

        }

        #endregion

        #region TryParse(Number, out Meter)

        /// <summary>
        /// Parse the given number as a meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(Single Number, out Meter Meter)
        {

            if (Number >= 0)
            {
                try
                {
                    Meter = new Meter(Number, IsKiloMeters: false);
                    return true;
                }
                catch
                { }
            }

            Meter = default;
            return false;

        }

        /// <summary>
        /// Parse the given number as a meter.
        /// </summary>
        /// <param name="Number">A numeric representation of a meter.</param>
        /// <param name="Meter">The parsed Meter.</param>
        public static Boolean TryParse(Double Number, out Meter Meter)
        {

            if (Number >= 0)
            {
                try
                {
                    Meter = new Meter(Number, IsKiloMeters: false);
                    return true;
                }
                catch
                { }
            }

            Meter = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this Meter.
        /// </summary>
        public Meter Clone
            => new Meter(Value, IsKiloMeters);

        #endregion


        #region Operator overloading

        #region Operator == (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A meter.</param>
        /// <param name="Meter2">Another meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Meter Meter1,
                                           Meter Meter2)

            => Meter1.Equals(Meter2);

        #endregion

        #region Operator != (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A meter.</param>
        /// <param name="Meter2">Another meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Meter Meter1,
                                           Meter Meter2)

            => !Meter1.Equals(Meter2);

        #endregion

        #region Operator <  (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A meter.</param>
        /// <param name="Meter2">Another meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Meter Meter1,
                                          Meter Meter2)

            => Meter1.CompareTo(Meter2) < 0;

        #endregion

        #region Operator <= (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A meter.</param>
        /// <param name="Meter2">Another meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Meter Meter1,
                                           Meter Meter2)

            => Meter1.CompareTo(Meter2) <= 0;

        #endregion

        #region Operator >  (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A meter.</param>
        /// <param name="Meter2">Another meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Meter Meter1,
                                          Meter Meter2)

            => Meter1.CompareTo(Meter2) > 0;

        #endregion

        #region Operator >= (Meter1, Meter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter1">A meter.</param>
        /// <param name="Meter2">Another meter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Meter Meter1,
                                           Meter Meter2)

            => Meter1.CompareTo(Meter2) >= 0;

        #endregion

        #region Operator +  (Meter1, Meter2)

        /// <summary>
        /// Accumulates two Meters.
        /// </summary>
        /// <param name="Meter1">A meter.</param>
        /// <param name="Meter2">Another meter.</param>
        public static Meter operator +  (Meter Meter1, Meter Meter2)
            => Meter.Parse(Meter1.Value + Meter2.Value);

        #endregion

        #region Operator -  (Meter1, Meter2)

        /// <summary>
        /// Substracts two Meters.
        /// </summary>
        /// <param name="Meter1">A meter.</param>
        /// <param name="Meter2">Another meter.</param>
        public static Meter operator -  (Meter Meter1, Meter Meter2)
            => Meter.Parse(Meter1.Value - Meter2.Value);

        #endregion

        #endregion

        #region IComparable<Meter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Meter meter
                   ? CompareTo(meter)
                   : throw new ArgumentException("The given object is not a meter!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Meter)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Meter">An object to compare with.</param>
        public Int32 CompareTo(Meter Meter)

            => Value > Meter.Value
                   ? 1
                   : 0;

        #endregion

        #endregion

        #region IEquatable<Meter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Meter meter &&
                   Equals(meter);

        #endregion

        #region Equals(Meter)

        /// <summary>
        /// Compares two Meters for equality.
        /// </summary>
        /// <param name="Meter">A meter to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Meter Meter)

            => Value.Equals(Meter.Value);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalValue.ToString() + (IsKiloMeters ? " km" : " m");

        #endregion

    }

}