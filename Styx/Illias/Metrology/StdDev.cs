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
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for calculating the mean and
    /// standard deviation of a set of numbers.
    /// </summary>
    public static class StdDevExtensions
    {

        public static JArray ToJSON<T>(this StdDev<T> StdDev,
                                       String? Unit = null)

         where T : IFloatingPointIeee754<T>,
                   IEquatable<T>,
                   IComparable<T>,
                   IParsable<T>

            => Unit.IsNotNullOrEmpty()
                   ? new (StdDev.Mean, StdDev.StandardDeviation, Unit)
                   : new (StdDev.Mean, StdDev.StandardDeviation);


        public static JArray ToJSON<T>(this StdDev<T>   StdDev,
                                       Func<T, Double>  Mapper,
                                       String?          Unit = null)

         where T : IFloatingPointIeee754<T>,
                   IEquatable<T>,
                   IComparable<T>,
                   IParsable<T>

            => Unit.IsNotNullOrEmpty()
                   ? new (Mapper(StdDev.Mean), Mapper(StdDev.StandardDeviation), Unit)
                   : new (Mapper(StdDev.Mean), Mapper(StdDev.StandardDeviation));


    }


    /// <summary>
    /// A mean value with its standard deviation.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="Mean">The mean value.</param>
    /// <param name="StandardDeviation">The standard deviation of the value.</param>
    public readonly struct StdDev<T>(T Mean,
                                     T StandardDeviation)

        : IParsable  <StdDev<T>>,
          IEquatable <StdDev<T>>,
          IComparable<StdDev<T>>,
          IComparable

         where T : IParsable<T>,
                   IEquatable<T>,
                   IComparable<T>,
                   IComparable

    {

        #region Properties

        /// <summary>
        /// The mean value.
        /// </summary>
        public T Mean                 { get; } = Mean;

        /// <summary>
        /// The standard deviation of the value.
        /// </summary>
        public T StandardDeviation    { get; } = StandardDeviation;

        #endregion


        public static StdDev<T> From(T Mean,
                                     T StandardDeviation)

            => new (Mean, StandardDeviation);


        #region From (Enumerable, IsSampleData = true)

        /// <summary>
        /// Calculates the mean and standard deviation of the given enumeration of numbers.
        /// </summary>
        /// <param name="Enumerable">An enumeration of numbers.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<T2>

            From<T2>(IEnumerable<T2>  Enumerable,
                     Boolean?         IsSampleData   = true)

            where T2 : IFloatingPointIeee754<T2>

        {

            var   mean   = T2.Zero;
            var   m2     = T2.Zero;
            Int64 count  = 0;

            foreach (var element in Enumerable)
            {

                count++;
                var delta1  = element - mean;

                mean += delta1 / T2.CreateChecked(count);
                var delta2  = element - mean;

                m2   += delta1 * delta2;

            }

            if (count == 0)
                return new StdDev<T2>(
                           T2.Zero,
                           T2.Zero
                       );

            if (count == 1)
                return new StdDev<T2>(
                           mean,
                           T2.Zero
                       );

            return new StdDev<T2>(
                       mean,
                       T2.Sqrt(
                           m2 / T2.CreateChecked(IsSampleData == true
                                                     ? count - 1
                                                     : count)
                       )
                   );

        }


        /// <summary>
        /// Calculates the mean and standard deviation of the given enumeration of numbers.
        /// </summary>
        /// <param name="Enumerable">An enumeration of numbers.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<Decimal>

            From(IEnumerable<Decimal>  Enumerable,
                 Boolean?              IsSampleData   = true)

        {

            var   mean   = Decimal.Zero;
            var   m2     = Decimal.Zero;
            Int64 count  = 0;

            foreach (var element in Enumerable)
            {

                count++;
                var delta1  = element - mean;

                mean += delta1 / Decimal.CreateChecked(count);
                var delta2  = element - mean;

                m2   += delta1 * delta2;

            }

            if (count == 0)
                return new StdDev<Decimal>(
                           Decimal.Zero,
                           Decimal.Zero
                       );

            if (count == 1)
                return new StdDev<Decimal>(
                           mean,
                           Decimal.Zero
                       );

            return new StdDev<Decimal>(
                       mean,
                       (Decimal) Math.Sqrt(
                           (Double) (m2 / Decimal.CreateChecked(IsSampleData == true
                                                                    ? count - 1
                                                                    : count))
                       )
                   );

        }

        #endregion

        #region From (Span,       IsSampleData = true)

        /// <summary>
        /// Calculates the mean and standard deviation of the given span of numbers.
        /// </summary>
        /// <param name="Span">A span of numbers.</param>
        /// <param name="IsSampleData">Whether the given data is a sample (n-1) or the entire population (n).</param>
        public static StdDev<T2>

            From<T2>(ReadOnlySpan<T2>  Span,
                     Boolean?          IsSampleData   = true)

            where T2 : IFloatingPointIeee754<T2>

        {

            if (Span.IsEmpty)
                return new StdDev<T2>(
                           T2.Zero,
                           T2.Zero
                       );

            var mean  = T2.Zero;
            var m2    = T2.Zero;

            for (var i = 0; i < Span.Length; i++)
            {

                var n = (long) i + 1;
                T2 delta  = Span[i] - mean;

                mean += delta / T2.CreateChecked(n);
                T2 delta2 = Span[i] - mean;

                m2 += delta * delta2;

            }

            if (Span.Length == 1)
                return new StdDev<T2>(
                           mean,
                           T2.Zero
                       );

            return new StdDev<T2>(
                       mean,
                       T2.Sqrt(
                           m2 / T2.CreateChecked(IsSampleData == true
                                                     ? Span.Length - 1
                                                     : Span.Length)
                       )
                   );

        }

        #endregion



        public static Boolean TryParse(JArray JSON, out StdDev<T>? SSSSS, out String? ErrorResponse) //, Func<JToken, T> Parser)
        {
            SSSSS = null;
            ErrorResponse = null;

            if (JSON is not null)
            {

                if (JSON.Count == 2)
                {

                    try
                    {

                        SSSSS = null;

                        //SSSSS = new StdDev<T>((T)JSON[0],
                        //                     (T)JSON[1]);

                        return true;

                    }
                    catch
                    { }

                }

            }

            return false;

        }



        #region Parse

        /// <summary>
        /// Parses a string in the exact format produced by <see cref="ToString"/> (e.g. "42.5 ±3.2").
        /// </summary>
        public static StdDev<T> Parse(String s)
            => Parse(s, provider: null);

        #endregion

        #region Parse

        /// <summary>
        /// Parses a string in the format "value ± stddev" using the specified format provider.
        /// </summary>
        public static StdDev<T> Parse(String s, IFormatProvider? provider)
        {

            if (TryParse(s, provider, out var result))
                return result;

            throw new FormatException($"String '{s}' was not recognized as a valid StdDev<{typeof(T).Name}>. " +
                                      $"Expected format: 'value ± stddev' (example: \"{new StdDev<T>(default, default)}\").");

        }

        #endregion

        #region TryParse()

        /// <summary>
        /// Attempts to parse a string in the format "value ± stddev".
        /// </summary>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       out StdDev<T>                StdDev)

            => TryParse(Text,
                        null,
                        out StdDev);

        #endregion

        #region TryParse()

        /// <summary>
        /// Attempts to parse a string in the format "value ± stddev" (culture-aware).
        /// </summary>
        public static Boolean TryParse([NotNullWhen(true)] String?  Text,
                                       IFormatProvider?             provider,
                                       out StdDev<T>                StdDev)
        {

            StdDev = default;

            if (String.IsNullOrWhiteSpace(Text))
                return false;

            // Split on the ± character and automatically trim whitespace around each part
            var parts = Text.Split('±', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length != 2)
                return false;

            if (!T.TryParse(parts[0], provider, out var mean) ||
                !T.TryParse(parts[1], provider, out var stdDev))
            {
                return false;
            }

            StdDev = new StdDev<T>(
                         mean,
                         stdDev
                     );

            return true;

        }

        #endregion




        #region Deconstruction (tuple-like support)

        /// <summary>
        /// Deconstructs the struct into its two components.
        /// </summary>
        public void Deconstruct(out T value, out T standardDeviation)
        {
            value              = Mean;
            standardDeviation  = StandardDeviation;
        }

        #endregion


        #region Operator overloading

        #region Operator == (StdDev1, StdDev2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StdDev1">A standard deviation.</param>
        /// <param name="StdDev2">Another standard deviation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StdDev<T> StdDev1,
                                           StdDev<T> StdDev2)

            => StdDev1.Equals(StdDev2);

        #endregion

        #region Operator != (StdDev1, StdDev2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StdDev1">A standard deviation.</param>
        /// <param name="StdDev2">Another standard deviation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StdDev<T> StdDev1,
                                           StdDev<T> StdDev2)

            => !StdDev1.Equals(StdDev2);

        #endregion

        #region Operator <  (StdDev1, StdDev2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StdDev1">A standard deviation.</param>
        /// <param name="StdDev2">Another standard deviation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StdDev<T> StdDev1,
                                          StdDev<T> StdDev2)

            => StdDev1.CompareTo(StdDev2) < 0;

        #endregion

        #region Operator <= (StdDev1, StdDev2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StdDev1">A standard deviation.</param>
        /// <param name="StdDev2">Another standard deviation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (StdDev<T> StdDev1,
                                           StdDev<T> StdDev2)

            => StdDev1.CompareTo(StdDev2) <= 0;

        #endregion

        #region Operator >  (StdDev1, StdDev2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StdDev1">A standard deviation.</param>
        /// <param name="StdDev2">Another standard deviation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StdDev<T> StdDev1,
                                          StdDev<T> StdDev2)

            => StdDev1.CompareTo(StdDev2) > 0;

        #endregion

        #region Operator >= (StdDev1, StdDev2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StdDev1">A standard deviation.</param>
        /// <param name="StdDev2">Another standard deviation.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (StdDev<T> StdDev1,
                                           StdDev<T> StdDev2)

            => StdDev1.CompareTo(StdDev2) >= 0;

        #endregion

        #endregion

        #region IComparable<Meter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two standard deviations.
        /// </summary>
        /// <param name="Object">A standard deviation to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object switch {
                   null              => 1,
                   StdDev<T> stdDev  => CompareTo(stdDev),
                   _                 => throw new ArgumentException("The given object is not a standard deviation!", nameof(Object))
               };

        #endregion

        #region CompareTo(StdDev)

        /// <summary>
        /// Compares two standard deviations.
        /// </summary>
        /// <param name="Object">A standard deviation to compare with.</param>
        public Int32 CompareTo(StdDev<T> StdDev)
        {

            var c = Mean.CompareTo(StdDev.Mean);

            if (c == 0)
                c = StandardDeviation.CompareTo(StdDev.StandardDeviation);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Meter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two standard deviations for equality.
        /// </summary>
        /// <param name="Object">A standard deviation to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StdDev<T> stdDev &&
                   Equals(stdDev);

        #endregion

        #region Equals(StdDev)

        /// <summary>
        /// Compares two standard deviations for equality.
        /// </summary>
        /// <param name="StdDev">A standard deviation to compare with.</param>
        public Boolean Equals(StdDev<T> StdDev)

            => Mean.            Equals(StdDev.Mean) &&
               StandardDeviation.Equals(StdDev.StandardDeviation);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Mean.            GetHashCode() *3 ^
               StandardDeviation.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => $"{Mean} ±{StandardDeviation}";


        #endregion

    }

}
