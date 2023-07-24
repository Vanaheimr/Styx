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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for elapsed values.
    /// </summary>
    public static class ElapsedExtensions
    {

        #region Deduplicate<T>(this Enumeration)

        public static IEnumerable<Elapsed<T>> Deduplicate<T>(this IEnumerable<Elapsed<T>> Enumeration)
        {

            var OrderedEnumeration = Enumeration.
                                         OrderBy(TVP => TVP.Time).
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


    #region Elapsed<T>

    /// <summary>
    /// A value with an elapsed time.
    /// </summary>
    /// <typeparam name="TValue">The type of the elapsed value.</typeparam>
    public readonly struct Elapsed<TValue> : IEquatable<TValue>,
                                             IEquatable<Elapsed<TValue>>,
                                             IComparable<Elapsed<TValue>>
    {

        #region Properties

        /// <summary>
        /// The elapsed time.
        /// </summary>
        public TimeSpan  Time     { get; }

        /// <summary>
        /// The value.
        /// </summary>
        public TValue    Value    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new value with an elapsed time.
        /// </summary>
        /// <param name="Time">The elapsed time.</param>
        /// <param name="Value">The value.</param>
        public Elapsed(TimeSpan  Time,
                       TValue    Value)
        {

            this.Time   = Time;
            this.Value  = Value;

            json = new JObject(

                       new JProperty("time",   this.Time.TotalMilliseconds),

                       new JProperty("value",  Value is Byte    ||
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

                hashCode = Time.  GetHashCode() * 3 ^
                          (Value?.GetHashCode() ?? 0);

            }

        }

        #endregion


        #region Deconstruct(out Time, out Value)

        /// <summary>
        /// Deconstruct the given elapsed value into its elapsed time and value.
        /// </summary>
        /// <param name="Time">The elapsed time.</param>
        /// <param name="Value">The value.</param>
        public void Deconstruct(out TimeSpan  Time,
                                out TValue    Value)
        {
            Time   = this.Time;
            Value  = this.Value;
        }

        #endregion


        #region ToJSON()

        private readonly JObject json;

        public JObject ToJSON()
            => json;

        #endregion


        #region Operator overloading

        #region Operator == (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Elapsed<TValue> Elapsed1,
                                           Elapsed<TValue> Elapsed2)

            => Elapsed1.Equals(Elapsed2);

        #endregion

        #region Operator == (Elapsed,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed">An elapsed value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Elapsed<TValue>  Elapsed,
                                           TValue           Value)

            => Elapsed.Value?.Equals(Value) == true;

        #endregion

        #region Operator == (Elapsed,  ElapsedTime)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed">An elapsed value.</param>
        /// <param name="ElapsedTime">Another elapsed time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Elapsed<TValue>  Elapsed,
                                           TimeSpan         ElapsedTime)

            => Elapsed.Time.Equals(ElapsedTime);

        #endregion

        #region Operator != (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Elapsed<TValue>  Elapsed1,
                                           Elapsed<TValue>  Elapsed2)

            => !Elapsed1.Equals(Elapsed2);

        #endregion

        #region Operator != (Elapsed,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Elapsed<TValue>  Elapsed1,
                                           TValue           Value)

            => !(Elapsed1.Value?.Equals(Value) == true);

        #endregion

        #region Operator != (Elapsed,  ElapsedTime)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed">An elapsed value.</param>
        /// <param name="ElapsedTime">Another elapsed time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Elapsed<TValue>  Elapsed,
                                           TimeSpan         ElapsedTime)

            => !Elapsed.Time.Equals(ElapsedTime);

        #endregion


        #region Operator <  (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Elapsed<TValue>  Elapsed1,
                                          Elapsed<TValue>  Elapsed2)

            => Elapsed1.CompareTo(Elapsed2) < 0;

        #endregion

        #region Operator <= (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Elapsed<TValue>  Elapsed1,
                                           Elapsed<TValue>  Elapsed2)

            => Elapsed1.CompareTo(Elapsed2) <= 0;

        #endregion

        #region Operator >  (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Elapsed<TValue>  Elapsed1,
                                          Elapsed<TValue>  Elapsed2)

            => Elapsed1.CompareTo(Elapsed2) > 0;

        #endregion

        #region Operator >= (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Elapsed<TValue>  Elapsed1,
                                           Elapsed<TValue>  Elapsed2)

            => Elapsed1.CompareTo(Elapsed2) >= 0;

        #endregion

        #endregion

        #region IComparable<Elapsed<T>> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Elapsed<TValue> elapsed
                   ? CompareTo(elapsed)
                   : throw new ArgumentException("The given object is not an elapsed value!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Elapsed)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed">An object to compare with.</param>
        public Int32 CompareTo(Elapsed<TValue> Elapsed)
        {

            var c = Time.CompareTo(Elapsed.Time);

            if (c == 0)
                c = (Value?.ToString() ?? "").CompareTo(Elapsed.Value?.ToString() ?? "");

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Elapsed<T>> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two elapsed values for equality.
        /// </summary>
        /// <param name="Object">An elapsed value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Elapsed<TValue> elapsed &&
                   Equals(elapsed);

        #endregion

        #region Equals(OtherValue)

        /// <summary>
        /// Compares two elapsed values for equality.
        /// </summary>
        /// <param name="OtherValue">An elapsed value to compare with.</param>
        public Boolean Equals(TValue? OtherValue)
        {

            if (Value == null && OtherValue == null)
                return true;

            if (Value == null)
                return false;

            return Value.Equals(OtherValue);

        }

        #endregion

        #region Equals(Elapsed)

        /// <summary>
        /// Compares two elapsed values for equality.
        /// </summary>
        /// <param name="Elapsed">An elapsed value to compare with.</param>
        public Boolean Equals(Elapsed<TValue> Elapsed)
        {

            if (Time != Elapsed.Time)
                return false;

            if (Value is null && Elapsed.Value is null)
                return true;

            if (Value is null)
                return false;

            return Value.Equals(Elapsed.Value);

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

            => $"'{Value?.ToString() ?? ""}' @ {Time.TotalMilliseconds:F2}";

        #endregion

    }

    #endregion

    #region Elapsed_RW<TValue>

    /// <summary>
    /// A value with an elapsed time.
    /// </summary>
    /// <typeparam name="TValue">The type of the elapsed value.</typeparam>
    public struct Elapsed_RW<TValue> : IEquatable<TValue>,
                                       IEquatable<Elapsed_RW<TValue>>,
                                       IComparable<Elapsed_RW<TValue>>
    {

        #region Properties

        /// <summary>
        /// The elapsed time.
        /// </summary>
        public TimeSpan  Time     { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public TValue    Value    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new value with an elapsed time.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="Time">The elapsed time.</param>
        public Elapsed_RW(TValue    Value,
                          TimeSpan  Time)
        {
            this.Value  = Value;
            this.Time   = Time;
        }

        #endregion


        #region Deconstruct(out Time, out Value)

        /// <summary>
        /// Deconstruct the given elapsed value into its elapsed time and value.
        /// </summary>
        /// <param name="Time">The elapsed time.</param>
        /// <param name="Value">The value.</param>
        public void Deconstruct(out TimeSpan  Time,
                                out TValue    Value)
        {
            Time   = this.Time;
            Value  = this.Value;
        }

        #endregion


        #region Operator overloading

        #region Operator == (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Elapsed_RW<TValue>  Elapsed1,
                                           Elapsed_RW<TValue>  Elapsed2)

            => Elapsed1.Equals(Elapsed2);

        #endregion

        #region Operator == (Elapsed,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed">An elapsed value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Elapsed_RW<TValue>  Elapsed,
                                           TValue              Value)

            => Elapsed.Value?.Equals(Value) == true;

        #endregion

        #region Operator == (Elapsed,  ElapsedTime)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed">An elapsed value.</param>
        /// <param name="ElapsedTime">Another elapsed time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Elapsed_RW<TValue>  Elapsed,
                                           TimeSpan            ElapsedTime)

            => Elapsed.Time.Equals(ElapsedTime);

        #endregion

        #region Operator != (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Elapsed_RW<TValue>  Elapsed1,
                                           Elapsed_RW<TValue>  Elapsed2)

            => !Elapsed1.Equals(Elapsed2);

        #endregion

        #region Operator != (Elapsed,  Value)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Value">Another value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Elapsed_RW<TValue>  Elapsed1,
                                           TValue              Value)

            => !(Elapsed1.Value?.Equals(Value) == true);

        #endregion

        #region Operator != (Elapsed,  ElapsedTime)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed">An elapsed value.</param>
        /// <param name="ElapsedTime">Another elapsed time.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Elapsed_RW<TValue>  Elapsed,
                                           TimeSpan            ElapsedTime)

            => !Elapsed.Time.Equals(ElapsedTime);

        #endregion


        #region Operator <  (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Elapsed_RW<TValue>  Elapsed1,
                                          Elapsed_RW<TValue>  Elapsed2)

            => Elapsed1.CompareTo(Elapsed2) < 0;

        #endregion

        #region Operator <= (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Elapsed_RW<TValue>  Elapsed1,
                                           Elapsed_RW<TValue>  Elapsed2)

            => Elapsed1.CompareTo(Elapsed2) <= 0;

        #endregion

        #region Operator >  (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Elapsed_RW<TValue>  Elapsed1,
                                          Elapsed_RW<TValue>  Elapsed2)

            => Elapsed1.CompareTo(Elapsed2) > 0;

        #endregion

        #region Operator >= (Elapsed1, Elapsed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed1">An elapsed value.</param>
        /// <param name="Elapsed2">Another elapsed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Elapsed_RW<TValue>  Elapsed1,
                                           Elapsed_RW<TValue>  Elapsed2)

            => Elapsed1.CompareTo(Elapsed2) >= 0;

        #endregion

        #endregion

        #region IComparable<Elapsed_RW<T>> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Elapsed_RW<TValue> elapsed
                   ? CompareTo(elapsed)
                   : throw new ArgumentException("The given object is not an elapsed value!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Elapsed)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Elapsed">An object to compare with.</param>
        public Int32 CompareTo(Elapsed_RW<TValue> Elapsed)
        {

            var c = Time.CompareTo(Elapsed.Time);

            if (c == 0)
                c = (Value?.ToString() ?? "").CompareTo(Elapsed.Value?.ToString() ?? "");

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Elapsed_RW<T>> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two elapsed values for equality.
        /// </summary>
        /// <param name="Object">An elapsed value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Elapsed_RW<TValue> elapsed &&
                   Equals(elapsed);

        #endregion

        #region Equals(OtherValue)

        /// <summary>
        /// Compares two elapsed values for equality.
        /// </summary>
        /// <param name="OtherValue">An elapsed value to compare with.</param>
        public Boolean Equals(TValue? OtherValue)
        {

            if (Value == null && OtherValue == null)
                return true;

            if (Value == null)
                return false;

            return Value.Equals(OtherValue);

        }

        #endregion

        #region Equals(Elapsed)

        /// <summary>
        /// Compares two elapsed values for equality.
        /// </summary>
        /// <param name="Elapsed">An elapsed value to compare with.</param>
        public Boolean Equals(Elapsed_RW<TValue> Elapsed)
        {

            if (Time != Elapsed.Time)
                return false;

            if (Value is null && Elapsed.Value is null)
                return true;

            if (Value is null)
                return false;

            return Value.Equals(Elapsed.Value);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Time.GetHashCode() * 3 ^
              (Value?.     GetHashCode() ?? 0);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{Value?.ToString() ?? ""}' @ {Time.TotalMilliseconds:F2}";

        #endregion

    }

    #endregion

}
