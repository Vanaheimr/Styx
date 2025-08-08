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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for timestamped values.
    /// </summary>
    public static class TimestampedExtensions
    {

        #region Deduplicate<T>(this Enumeration)

        public static IEnumerable<Timestamped<T>> Deduplicate<T>(this IEnumerable<Timestamped<T>> Enumeration)
        {

            var OrderedEnumeration = Enumeration.
                                         OrderBy(TVP => TVP.Timestamp.ToISO8601()).
                                         ToArray();

            if (OrderedEnumeration.Length > 0)
            {

                for (var i = 0; i < OrderedEnumeration.Length - 1; i++) {
                    if (!OrderedEnumeration[i].Equals(OrderedEnumeration[i + 1]))
                        yield return OrderedEnumeration[i];
                }

                yield return OrderedEnumeration[OrderedEnumeration.Length];

            }

        }

        #endregion

    }


    #region Timestamped<T>

    /// <summary>
    /// A value with a timestamp.
    /// </summary>
    /// <typeparam name="TValue">The type of the timestamped value.</typeparam>
    public readonly struct Timestamped<TValue> : IEquatable<TValue>,
                                                 IEquatable<Timestamped<TValue>>,
                                                 IComparable<Timestamped<TValue>>
    {

        #region Properties

        /// <summary>
        /// The timestamp of the value creation.
        /// </summary>
        public DateTimeOffset  Timestamp    { get; }

        /// <summary>
        /// The value.
        /// </summary>
        public TValue          Value        { get; }

        #endregion

        #region Constructor(s)

        #region Timestamped(Value)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Value">The value.</param>
        public Timestamped(TValue Value)

            : this(Illias.Timestamp.Now,
                   Value)

        { }

        #endregion

        #region Timestamped(Timestamp, Value)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Timestamp">The timestamp.</param>
        /// <param name="Value">The value.</param>
        public Timestamped(DateTimeOffset  Timestamp,
                           TValue          Value)
        {

            this.Timestamp  = Timestamp;
            this.Value      = Value;

            json = new JObject(

                       new JProperty("timestamp",  this.Timestamp.ToISO8601()),

                       new JProperty("value",      Value is Byte    ||
                                                   Value is Single  ||
                                                   Value is Double  ||
                                                   Value is Decimal ||
                                                   Value is SByte   ||
                                                   Value is Int16   ||
                                                   Value is Int32   ||
                                                   Value is Int64   ||
                                                   Value is Int128  ||
                                                   Value is UInt16  ||
                                                   Value is UInt32  ||
                                                   Value is UInt64  ||
                                                   Value is UInt128
                                                       ? Value
                                                       : Value?.ToString())
                   );

            unchecked
            {

                hashCode = Timestamp.GetHashCode() * 3 ^
                          (Value?.   GetHashCode() ?? 0);

            }

        }

        #endregion

        #endregion


        #region Deconstruct(out Timestamp, out Value)

        /// <summary>
        /// Deconstruct the given timestamped value into its timestamp and value.
        /// </summary>
        /// <param name="Timestamp">The timestamp.</param>
        /// <param name="Value">The value.</param>
        public void Deconstruct(out DateTimeOffset  Timestamp,
                                out TValue          Value)
        {
            Timestamp  = this.Timestamp;
            Value      = this.Value;
        }

        #endregion

        #region Value -implicit-> Timestamped<Value>

        /// <summary>
        /// Implicit conversation from an non-timestamped value
        /// to a timestamped value.
        /// </summary>
        /// <param name="Value">The value to be timestamped.</param>
        public static implicit operator Timestamped<TValue>(TValue Value)

            => new (Value);

        #endregion


        #region ToJSON()

        private readonly JObject json;

        public JObject ToJSON()
            => json;

        #endregion


        #region Operator overloading

        #region Operator == (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped<TValue> Timestamped1,
                                           Timestamped<TValue> Timestamped2)

            => Timestamped1.Equals(Timestamped2);

        #endregion

        #region Operator == (Timestamped,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">A timestamped value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped<TValue>  Timestamped,
                                           TValue               Value)

            => Timestamped.Value?.Equals(Value) == true;

        #endregion

        #region Operator == (Timestamped,  Timestamp)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">A timestamped value.</param>
        /// <param name="Timestamp">Another timestamp.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped<TValue>  Timestamped,
                                           DateTime             Timestamp)

            => Timestamped.Timestamp.Equals(Timestamp);

        #endregion

        #region Operator != (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped<TValue>  Timestamped1,
                                           Timestamped<TValue>  Timestamped2)

            => !Timestamped1.Equals(Timestamped2);

        #endregion

        #region Operator != (Timestamped,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped<TValue>  Timestamped1,
                                           TValue               Value)

            => !(Timestamped1.Value?.Equals(Value) == true);

        #endregion

        #region Operator != (Timestamped,  Timestamp)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">A timestamped value.</param>
        /// <param name="Timestamp">Another timestamp.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped<TValue>  Timestamped,
                                           DateTime             Timestamp)

            => !Timestamped.Timestamp.Equals(Timestamp);

        #endregion


        #region Operator <  (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Timestamped<TValue>  Timestamped1,
                                          Timestamped<TValue>  Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) < 0;

        #endregion

        #region Operator <= (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Timestamped<TValue>  Timestamped1,
                                           Timestamped<TValue>  Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) <= 0;

        #endregion

        #region Operator >  (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Timestamped<TValue>  Timestamped1,
                                          Timestamped<TValue>  Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) > 0;

        #endregion

        #region Operator >= (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Timestamped<TValue>  Timestamped1,
                                           Timestamped<TValue>  Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) >= 0;

        #endregion

        #endregion

        #region IComparable<Timestamped<T>> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Timestamped<TValue> timestamped
                   ? CompareTo(timestamped)
                   : throw new ArgumentException("The given object is not a timestamped value!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Timestamped)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">An object to compare with.</param>
        public Int32 CompareTo(Timestamped<TValue> Timestamped)
        {

            var c = Timestamp.CompareTo(Timestamped.Timestamp);

            if (c == 0)
                c = (Value?.ToString() ?? "").CompareTo(Timestamped.Value?.ToString() ?? "");

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Timestamped<T>> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two timestamped values for equality.
        /// </summary>
        /// <param name="Object">A timestamped value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Timestamped<TValue> timestamped &&
                   Equals(timestamped);

        #endregion

        #region Equals(OtherValue)

        /// <summary>
        /// Compares two timestamped values for equality.
        /// </summary>
        /// <param name="OtherValue">A timestamped value to compare with.</param>
        public Boolean Equals(TValue? OtherValue)
        {

            if (Value == null && OtherValue == null)
                return true;

            if (Value == null)
                return false;

            return Value.Equals(OtherValue);

        }

        #endregion

        #region Equals(Timestamped)

        /// <summary>
        /// Compares two timestamped values for equality.
        /// </summary>
        /// <param name="Timestamped">A timestamped value to compare with.</param>
        public Boolean Equals(Timestamped<TValue> Timestamped)
        {

            if (Timestamp.ToISO8601() != Timestamped.Timestamp.ToISO8601())
                return false;

            if (Value is null && Timestamped.Value is null)
                return true;

            if (Value is null)
                return false;

            return Value.Equals(Timestamped.Value);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{Value?.ToString() ?? ""}' @ {Timestamp.ToISO8601()}";

        #endregion

    }

    #endregion

    #region Timestamped_RW<TValue>

    /// <summary>
    /// A value with its creation timestamp.
    /// </summary>
    /// <typeparam name="TValue">The type of the timestamped value.</typeparam>
    public struct Timestamped_RW<TValue> : IEquatable<TValue>,
                                           IEquatable<Timestamped_RW<TValue>>,
                                           IComparable<Timestamped_RW<TValue>>
    {

        #region Properties

        /// <summary>
        /// The timestamp of the value creation.
        /// </summary>
        public DateTimeOffset  Timestamp    { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public TValue          Value        { get; set; }

        #endregion

        #region Constructor(s)

        #region Timestamped_RW(Value)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Value">The value.</param>
        public Timestamped_RW(TValue Value)
            : this(Value, Illias.Timestamp.Now)
        { }

        #endregion

        #region Timestamped_RW(Value, Timestamp)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="Timestamp">The timestamp.</param>
        public Timestamped_RW(TValue          Value,
                              DateTimeOffset  Timestamp)
        {
            this.Value      = Value;
            this.Timestamp  = Timestamp;
        }

        #endregion

        #endregion


        #region Deconstruct(out Timestamp, out Value)

        /// <summary>
        /// Deconstruct the given timestamped value into its timestamp and value.
        /// </summary>
        /// <param name="Timestamp">The timestamp.</param>
        /// <param name="Value">The value.</param>
        public void Deconstruct(out DateTimeOffset  Timestamp,
                                out TValue          Value)
        {
            Timestamp  = this.Timestamp;
            Value      = this.Value;
        }

        #endregion

        #region Value -implicit-> Timestamped_RW<Value>

        /// <summary>
        /// Implicit conversation from an non-timestamped value
        /// to a timestamped value.
        /// </summary>
        /// <param name="Value">The value to be timestamped.</param>
        public static implicit operator Timestamped_RW<TValue>(TValue Value)

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped_RW<TValue>  Timestamped1,
                                           Timestamped_RW<TValue>  Timestamped2)

            => Timestamped1.Equals(Timestamped2);

        #endregion

        #region Operator == (Timestamped,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">A timestamped value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped_RW<TValue>  Timestamped,
                                           TValue                  Value)

            => Timestamped.Value?.Equals(Value) == true;

        #endregion

        #region Operator == (Timestamped,  Timestamp)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">A timestamped value.</param>
        /// <param name="Timestamp">Another timestamp.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped_RW<TValue>  Timestamped,
                                           DateTime                Timestamp)

            => Timestamped.Timestamp.Equals(Timestamp);

        #endregion

        #region Operator != (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped_RW<TValue>  Timestamped1,
                                           Timestamped_RW<TValue>  Timestamped2)

            => !Timestamped1.Equals(Timestamped2);

        #endregion

        #region Operator != (Timestamped,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped_RW<TValue>  Timestamped1,
                                           TValue                  Value)

            => !(Timestamped1.Value?.Equals(Value) == true);

        #endregion

        #region Operator != (Timestamped,  Timestamp)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">A timestamped value.</param>
        /// <param name="Timestamp">Another timestamp.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped_RW<TValue>  Timestamped,
                                           DateTime                Timestamp)

            => !Timestamped.Timestamp.Equals(Timestamp);

        #endregion


        #region Operator <  (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Timestamped_RW<TValue>  Timestamped1,
                                          Timestamped_RW<TValue>  Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) < 0;

        #endregion

        #region Operator <= (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Timestamped_RW<TValue>  Timestamped1,
                                           Timestamped_RW<TValue>  Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) <= 0;

        #endregion

        #region Operator >  (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Timestamped_RW<TValue>  Timestamped1,
                                          Timestamped_RW<TValue>  Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) > 0;

        #endregion

        #region Operator >= (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Timestamped_RW<TValue>  Timestamped1,
                                           Timestamped_RW<TValue>  Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) >= 0;

        #endregion

        #endregion

        #region IComparable<Timestamped_RW<T>> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Timestamped_RW<TValue> timestamped
                   ? CompareTo(timestamped)
                   : throw new ArgumentException("The given object is not a timestamped value!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Timestamped)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">An object to compare with.</param>
        public Int32 CompareTo(Timestamped_RW<TValue> Timestamped)
        {

            var c = Timestamp.CompareTo(Timestamped.Timestamp);

            if (c == 0)
                c = (Value?.ToString() ?? "").CompareTo(Timestamped.Value?.ToString() ?? "");

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Timestamped_RW<T>> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two timestamped values for equality.
        /// </summary>
        /// <param name="Object">A timestamped value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Timestamped_RW<TValue> timestamped &&
                   Equals(timestamped);

        #endregion

        #region Equals(OtherValue)

        /// <summary>
        /// Compares two timestamped values for equality.
        /// </summary>
        /// <param name="OtherValue">A timestamped value to compare with.</param>
        public Boolean Equals(TValue? OtherValue)
        {

            if (Value == null && OtherValue == null)
                return true;

            if (Value == null)
                return false;

            return Value.Equals(OtherValue);

        }

        #endregion

        #region Equals(Timestamped)

        /// <summary>
        /// Compares two timestamped values for equality.
        /// </summary>
        /// <param name="Timestamped">A timestamped value to compare with.</param>
        public Boolean Equals(Timestamped_RW<TValue> Timestamped)
        {

            if (Timestamp.ToISO8601() != Timestamped.Timestamp.ToISO8601())
                return false;

            if (Value is null && Timestamped.Value is null)
                return true;

            if (Value is null)
                return false;

            return Value.Equals(Timestamped.Value);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Timestamp.GetHashCode() * 3 ^
              (Value?.   GetHashCode() ?? 0);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{Value?.ToString() ?? ""}' @ {Timestamp.ToISO8601()}";

        #endregion

    }

    #endregion

}
