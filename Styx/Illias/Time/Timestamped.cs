﻿/*
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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for timestamped values.
    /// </summary>
    public static class TimestampedExtensions
    {

        public static IEnumerable<Timestamped<T>> Deduplicate<T>(this IEnumerable<Timestamped<T>> Enumeration)
        {

            var OrderedEnumeration = Enumeration.
                                         OrderBy(TVP => TVP.Timestamp).
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


        /// <summary>
        /// Return a JSON representation of the given timestamped value.
        /// </summary>
        /// <param name="TimestampedT">A timestamped value.</param>
        public static JObject ToJSON<T>(this Timestamped<T> TimestampedT)

            => new (
                   new JProperty("timestamp", TimestampedT.Timestamp.ToIso8601()),
                   new JProperty("value",     TimestampedT.Value?.   ToString())
               );


    }


    #region Timestamped<T>

    /// <summary>
    /// A value with its creation timestamp.
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
        public DateTime  Timestamp    { get; }

        /// <summary>
        /// The value.
        /// </summary>
        public TValue    Value        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Value">The value.</param>
        public Timestamped(TValue Value)
            : this(Illias.Timestamp.Now, Value)
        { }

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Timestamp">The timestamp.</param>
        /// <param name="Value">The value.</param>
        public Timestamped(DateTime  Timestamp,
                           TValue    Value)
        {
            this.Value      = Value;
            this.Timestamp  = Timestamp;
        }

        #endregion


        #region Deconstruct(out Value, out Percent)

        public void Deconstruct(out DateTime Timestamp, out TValue Value)
        {
            Timestamp  = this.Timestamp;
            Value      = this.Value;
        }

        #endregion

        #region Value -implicit-> Timestamped<Value>

        /// <summary>
        /// Implicit conversatiuon from an non-timestamped value
        /// to a timestamped value.
        /// </summary>
        /// <param name="Value">The value to be timestamped.</param>
        public static implicit operator Timestamped<TValue>(TValue Value)

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
        public static Boolean operator == (Timestamped<TValue> Timestamped1, Timestamped<TValue> Timestamped2)

            => Timestamped1.Equals(Timestamped2);

        #endregion

        #region Operator == (Timestamped,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">A timestamped value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped<TValue> Timestamped, TValue Value)

            => Timestamped.Value?.Equals(Value) == true;

        #endregion

        #region Operator == (Timestamped,  Timestamp)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">A timestamped value.</param>
        /// <param name="Timestamp">Another timestamp.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped<TValue> Timestamped, DateTime Timestamp)

            => Timestamped.Timestamp.Equals(Timestamp);

        #endregion

        #region Operator != (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped<TValue> Timestamped1, Timestamped<TValue> Timestamped2)

            => !Timestamped1.Equals(Timestamped2);

        #endregion

        #region Operator != (Timestamped,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped<TValue> Timestamped1, TValue Value)

            => !(Timestamped1.Value?.Equals(Value) == true);

        #endregion

        #region Operator != (Timestamped,  Timestamp)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">A timestamped value.</param>
        /// <param name="Timestamp">Another timestamp.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped<TValue> Timestamped, DateTime Timestamp)

            => !Timestamped.Timestamp.Equals(Timestamp);

        #endregion


        #region Operator <  (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Timestamped<TValue> Timestamped1, Timestamped<TValue> Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) < 0;

        #endregion

        #region Operator <= (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Timestamped<TValue> Timestamped1, Timestamped<TValue> Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) <= 0;

        #endregion

        #region Operator >  (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Timestamped<TValue> Timestamped1, Timestamped<TValue> Timestamped2)

            => Timestamped1.CompareTo(Timestamped2) > 0;

        #endregion

        #region Operator >= (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Timestamped<TValue> Timestamped1, Timestamped<TValue> Timestamped2)

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
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Timestamped<TValue> timestamped &&
                   Equals(timestamped);

        #endregion

        #region Equals(OtherValue)

        /// <summary>
        /// Compares two timestamped values for equality.
        /// </summary>
        /// <param name="OtherValue"></param>
        /// <returns></returns>
        public Boolean Equals(TValue OtherValue)
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
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Timestamped<TValue> Timestamped)
        {

            if (Timestamp != Timestamped.Timestamp)
                return false;

            if (Value == null && Timestamped.Value == null)
                return true;

            if (Value == null)
                return false;

            return Value.Equals(Timestamped.Value);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Timestamp.GetHashCode() * 3 ^
                      (Value?.   GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Timestamp.ToIso8601(),
                             " -> ",
                             Value?.ToString() ?? "");

        #endregion

    }

    #endregion

    #region Timestamped_RW<T>

    /// <summary>
    /// A value with its creation timestamp.
    /// </summary>
    /// <typeparam name="T">The type of the timestamped value.</typeparam>
    public struct Timestamped_RW<T>
    {

        #region Properties

        /// <summary>
        /// The timestamp of the value creation.
        /// </summary>
        public DateTime  Timestamp    { get; private set; }

        /// <summary>
        /// The value.
        /// </summary>
        public T         Value        { get; }

        #endregion

        #region Constructor(s)

        #region Timestamped_RW(Value)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Value">The value.</param>
        public Timestamped_RW(T Value)
            : this(Value, Illias.Timestamp.Now)
        { }

        #endregion

        #region Timestamped_RW(Value, Timestamp)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="Timestamp">The timestamp.</param>
        public Timestamped_RW(T         Value,
                              DateTime  Timestamp)
        {
            this.Value      = Value;
            this.Timestamp  = Timestamp;
        }

        #endregion

        #endregion


        public void UpdateTimestamp()
        {
            this.Timestamp = DateTime.UtcNow;
        }

    }

    #endregion

}
