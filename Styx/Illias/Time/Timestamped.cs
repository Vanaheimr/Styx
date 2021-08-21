/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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
using System.Linq;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    #region Timestamped<T>

    /// <summary>
    /// A value with its creation timestamp.
    /// </summary>
    /// <typeparam name="T">The type of the timestamped value.</typeparam>
    public readonly struct Timestamped<T> : IEquatable<Timestamped<T>>,
                                            IEquatable<T>,
                                            IComparable<Timestamped<T>>
    {

        #region Properties

        /// <summary>
        /// The timestamp of the value creation.
        /// </summary>
        public DateTime Timestamp   { get; }

        /// <summary>
        /// The value.
        /// </summary>
        public T        Value       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Value">The value.</param>
        public Timestamped(T Value)
            : this(DateTime.UtcNow, Value)
        { }

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Timestamp">The timestamp.</param>
        /// <param name="Value">The value.</param>
        public Timestamped(DateTime Timestamp, T Value)
        {
            this.Value      = Value;
            this.Timestamp  = Timestamp;
        }

        #endregion


        #region Value -implicit-> Timestamped<Value>

        /// <summary>
        /// Implicit conversatiuon from an non-timestamped value
        /// to a timestamped value.
        /// </summary>
        /// <param name="Value">The value to be timestamped.</param>
        public static implicit operator Timestamped<T>(T Value)
            => new Timestamped<T>(Value);

        #endregion


        #region Operator overloading

        #region Operator == (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped<T> Timestamped1, Timestamped<T> Timestamped2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Timestamped1, Timestamped2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Timestamped1 == null) || ((Object) Timestamped2 == null))
                return false;

            return Timestamped1.Equals(Timestamped2);

        }

        #endregion

        #region Operator == (Timestamped1, Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Timestamped<T> Timestamped1, T Value)
        {

            // If one is null, but not both, return false.
            if (((Object) Timestamped1 == null) || ((Object) Value == null))
                return false;

            return Timestamped1.Value.Equals(Value);

        }

        #endregion

        #region Operator != (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped<T> Timestamped1, Timestamped<T> Timestamped2)
            => !(Timestamped1 == Timestamped2);

        #endregion

        #region Operator != (Timestamped1, Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Timestamped<T> Timestamped1, T Value)
            => !(Timestamped1 == Value);

        #endregion


        #region Operator <  (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Timestamped<T> Timestamped1, Timestamped<T> Timestamped2)
        {

            if ((Object) Timestamped1 == null)
                throw new ArgumentNullException("The given Timestamped1 must not be null!");

            return Timestamped1.CompareTo(Timestamped2) < 0;

        }

        #endregion

        #region Operator <= (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Timestamped<T> Timestamped1, Timestamped<T> Timestamped2)
            => !(Timestamped1 > Timestamped2);

        #endregion

        #region Operator >  (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Timestamped<T> Timestamped1, Timestamped<T> Timestamped2)
        {

            if ((Object) Timestamped1 == null)
                throw new ArgumentNullException("The given Timestamped1 must not be null!");

            return Timestamped1.CompareTo(Timestamped2) > 0;

        }

        #endregion

        #region Operator >= (Timestamped1, Timestamped2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped1">A timestamped value.</param>
        /// <param name="Timestamped2">Another timestamped value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Timestamped<T> Timestamped1, Timestamped<T> Timestamped2)
            => !(Timestamped1 < Timestamped2);

        #endregion

        #endregion

        #region IComparable<Timestamped<T>> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            return CompareTo((Timestamped<T>) Object);

        }

        #endregion

        #region CompareTo(Timestamped)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Timestamped">An object to compare with.</param>
        public Int32 CompareTo(Timestamped<T> Timestamped)
        {

            if ((Object) Timestamped == null)
                throw new ArgumentNullException(nameof(Timestamped), "The given timestamped value must not be null!");

            // Compare the timestamps
            var _Result = Timestamp.CompareTo(Timestamped.Timestamp);

            // If equal: Compare the values
            if (_Result == 0)
                _Result = Value.ToString().CompareTo(Timestamped.Value.ToString());

            return _Result;

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
        {

            if (Object == null)
                return false;

            if (!(Object is Timestamped<T>))
                return false;

            return Equals((Timestamped<T>) Object);

        }

        #endregion

        #region Equals(OtherValue)

        /// <summary>
        /// Compares two timestamped values for equality.
        /// </summary>
        /// <param name="OtherValue"></param>
        /// <returns></returns>
        public Boolean Equals(T OtherValue)
        {

            if ((Object) OtherValue == null)
                return false;

            if (Value == null && OtherValue == null)
                return true;

            if (OtherValue == null)
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
        public Boolean Equals(Timestamped<T> Timestamped)
        {

            if ((Object) Timestamped == null)
                return false;

            if (Value == null && Timestamped.Value == null)
                return true;

            if (Value == null)
                return false;

            if (!Timestamp.Equals(Timestamped.Timestamp))
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
                       Value.    GetHashCode();

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
                             Value.ToString());

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

        #region Timestamp

        private DateTime _Timestamp;

        /// <summary>
        /// The timestamp of the value creation.
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return _Timestamp;
            }
        }

        #endregion

        #region Value

        private readonly T _Value;

        /// <summary>
        /// The value.
        /// </summary>
        public T Value
        {
            get
            {
                return _Value;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region Timestamped_RW(Value)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Value">The value.</param>
        public Timestamped_RW(T Value)
            : this(Value, DateTime.UtcNow)
        { }

        #endregion

        #region Timestamped(Value, Timestamp)

        /// <summary>
        /// Create a new timestamped value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="Timestamp">The timestamp.</param>
        public Timestamped_RW(T Value, DateTime Timestamp)
        {
            _Value      = Value;
            _Timestamp  = Timestamp;
        }

        #endregion

        #endregion


        public void UpdateTimestamp()
        {
            _Timestamp = DateTime.UtcNow;
        }

    }

    #endregion


    public static class TimestampedExtentions
    {

        public static IEnumerable<Timestamped<T>> Deduplicate<T>(this IEnumerable<Timestamped<T>> Enumeration)
        {

            var OrderedEnumeration = Enumeration.
                                         OrderBy(TVP => TVP.Timestamp).
                                         ToArray();

            if (OrderedEnumeration.Length > 0)
            {

                for (var i = 0; i < OrderedEnumeration.Length - 1; i++)
                {
                    if (!OrderedEnumeration[i].Equals(OrderedEnumeration[i + 1]))
                        yield return OrderedEnumeration[i];
                }

                yield return OrderedEnumeration[OrderedEnumeration.Length];

            }

        }


    }

}
