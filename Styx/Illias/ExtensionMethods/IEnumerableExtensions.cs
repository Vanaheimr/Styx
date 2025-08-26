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

using System.Collections;
using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the IEnumerable interface.
    /// </summary>
    public static class IEnumerableExtensions
    {

        #region StdDev(this IEnumerable)

        /// <summary>
        /// Calculates the standard deviation of the given enumeration of doubles.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of Doubles.</param>
        /// <returns>The stddev of the given enumeration of doubles.</returns>
        public static Double StdDev(this IEnumerable<Double> IEnumerable)

            => IEnumerable.AverageAndStdDev().Item2;

        #endregion

        #region AverageAndStdDev(this IEnumerable)

        /// <summary>
        /// Calculates the standard deviation of the given enumeration of doubles.
        /// </summary>
        /// <param name="IEnumerable">An enumeration of Doubles.</param>
        /// <returns>The mean and stddev of the given enumeration of doubles.</returns>
        public static Tuple<Double, Double> AverageAndStdDev(this IEnumerable<Double> IEnumerable)
        {

            #region Initial Checks

            if (IEnumerable is null)
                throw new ArgumentNullException(nameof(IEnumerable), "The given enumeration of doubles must not be null!");

            var count = IEnumerable.Count();

            if (count == 0)
                return new Tuple<Double, Double>(0, 0);

            if (count == 1)
                return new Tuple<Double, Double>(IEnumerable.First(), 0);

            #endregion

            var average  = IEnumerable.Average();
            var sum      = 0.0;

            foreach (var value in IEnumerable)
                sum += (value - average) * (value - average);

            return new Tuple<Double, Double>(average, Math.Sqrt(sum / (count - 1)));

        }

        #endregion


        #region CalcHashCode(this IEnumerable)

        /// <summary>
        /// Calculates the hash code of the given enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IEnumerable">An enumeration of T.</param>
        /// <returns>The combined GetHashCode() results of all elements of the given enumeration.</returns>
        public static Int32 CalcHashCode<T>(this IEnumerable<T> IEnumerable)
        {

            var hashCode = 3;

            foreach (var value in IEnumerable)
                hashCode ^= (value?.GetHashCode() ?? 0);

            return hashCode;

        }

        #endregion


        #region ForEach<T>(this IEnumerable, Action)

        /// <summary>
        /// Calls the given delegate for each element of the enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration of type T.</param>
        /// <param name="Action">An action to call for each element of the enumeration.</param>
        public static void ForEach<T>(this IEnumerable<T> IEnumerable, Action<T> Action)
        {

            #region Initial checks

            if (IEnumerable is null)
                return;

            if (Action is null)
                throw new ArgumentNullException(nameof(Action), "The given delegate must not be null!");

            #endregion

            foreach (var Element in IEnumerable)
                Action(Element);

        }

        #endregion

        #region TryForEach<T>(this IEnumerable, Action)

        /// <summary>
        /// Calls the given delegate for each element of the enumeration,
        /// but does not fail if any parameter is null.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration of type T.</param>
        /// <param name="Action">An action to call for each element of the enumeration.</param>
        public static void TryForEach<T>(this IEnumerable<T> IEnumerable, Action<T> Action)
        {
            if (IEnumerable is not null && Action is not null)
                foreach (var Element in IEnumerable)
                    Action(Element);
        }

        #endregion

        #region ForEach<S, T>(this IEnumerable, Seed, Action)

        /// <summary>
        /// Calls the given delegate for each element of the enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration of type T.</param>
        /// <param name="Action">An action to call for each element of the enumeration.</param>
        public static S ForEach<S, T>(this IEnumerable<T> IEnumerable, S Seed, Action<S, T> Action)
        {

            #region Initial checks

            if (IEnumerable is null)
                throw new ArgumentNullException(nameof(IEnumerable), "The given IEnumerable must not be null!");

            if (Action      is null)
                throw new ArgumentNullException(nameof(Action),      "The given Action must not be null!");

            #endregion

            S seed = Seed;

            foreach (var Item in IEnumerable)
                Action(seed, Item);

            return seed;

        }

        #endregion

        #region ForEachCounted<T>(this IEnumerable, Action, Counter = 1UL)

        /// <summary>
        /// Calls the given delegate for each element of the enumeration
        /// and count the number of elements.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration of type T.</param>
        /// <param name="Action">An action to call for each element of the enumeration and a counter.</param>
        /// <param name="Counter">The initial value of the counter.</param>
        public static void ForEachCounted<T>(this IEnumerable<T> IEnumerable, Action<T, UInt64> Action, UInt64 Counter = 1UL)
        {

            if (IEnumerable is null || Action is null)
                return;

            foreach (var Element in IEnumerable)
                Action(Element, Counter++);

        }

        #endregion

        #region ForEach<T>(this IEnumerable, First, Remaining)

        /// <summary>
        /// Calls the given delegate for each element of the enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration of type T.</param>
        /// <param name="First">An action to call for the first element of the enumeration.</param>
        /// <param name="Remaining">An action to call for each element except the first element of the enumeration.</param>
        public static void ForEach<T>(this IEnumerable<T> IEnumerable, Action<T> First, Action<T> Remaining)
        {

            #region Initial checks

            if (IEnumerable is null)
                throw new ArgumentNullException(nameof(IEnumerable), "The given IEnumerable must not be null!");

            if (First is null)
                throw new ArgumentNullException(nameof(First),       "The given Action must not be null!");

            if (Remaining is null)
                throw new ArgumentNullException(nameof(Remaining),   "The given Action must not be null!");

            #endregion

            if (!IEnumerable.Any())
                return;

            First(IEnumerable.First());

            foreach (var Element in IEnumerable.Skip(1))
                Remaining(Element);

        }

        #endregion

        #region SelectCounted<T1, T2>(this IEnumerable, Delegate, Counter = 1UL)

        /// <summary>
        /// Calls the given delegate for each element of the enumeration
        /// and count the number of elements.
        /// </summary>
        /// <typeparam name="T1">The type of the enumeration.</typeparam>
        /// <typeparam name="T2">The type of the result enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration of type T.</param>
        /// <param name="Delegate">A delegate to call for a counter and each element of the enumeration.</param>
        /// <param name="Counter">The initial value of the counter.</param>
        public static IEnumerable<T2> SelectCounted<T1, T2>(this IEnumerable<T1> IEnumerable, Func<T1, UInt64, T2> Delegate, UInt64 Counter = 1UL)
        {

            if (IEnumerable is null || Delegate is null)
                yield break;

            if (IEnumerable.Any())
                foreach (var Element in IEnumerable)
                    yield return Delegate(Element, Counter++);

        }

        #endregion

        #region Sum(this IEnumerable)

        /// <summary>
        /// Computes the sum of a sequence of System.UInt64 values.
        /// </summary>
        public static UInt64 Sum(this IEnumerable<UInt64> IEnumerable)
        {

            if (IEnumerable is null)
                return 0;

            var sum = 0UL;

            foreach (var element in IEnumerable)
                sum += element;

            return sum;

        }

        #endregion


        #region Skip<T>(this Enumerable, Count)

        /// <summary>
        /// Skips the given number of elements in the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="Enumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> Enumerable, UInt32 Count)
        {

            var IEnumerator = Enumerable.GetEnumerator();

            for (var i = 0U; i<Count; i++)
                IEnumerator.MoveNext();

            while (IEnumerator.MoveNext())
                yield return IEnumerator.Current;

        }

        /// <summary>
        /// Skips the given number of elements in the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="Enumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> Enumerable, UInt32? Count)
        {

            if (Count.HasValue)
                return Enumerable.Skip(Count.Value);

            return Enumerable;

        }

        /// <summary>
        /// Skips the given number of elements in the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> IEnumerable, UInt64 Count)
        {

            var IEnumerator = IEnumerable.GetEnumerator();

            for (var i = 0UL; i < Count; i++)
                IEnumerator.MoveNext();

            while (IEnumerator.MoveNext())
                yield return IEnumerator.Current;

        }

        /// <summary>
        /// Skips the given number of elements in the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="Enumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> Enumerable, UInt64? Count)
        {

            if (Count.HasValue)
                return Enumerable.Skip(Count.Value);

            return Enumerable;

        }

        /// <summary>
        /// Skips the given number of elements in the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> IEnumerable, Int64 Count)
        {

            var IEnumerator = IEnumerable.GetEnumerator();

            for (var i = 0L; i < Count; i++)
                IEnumerator.MoveNext();

            while (IEnumerator.MoveNext())
                yield return IEnumerator.Current;

        }

        #endregion

        #region Take<T>(this Enumerable, Count)

        /// <summary>
        /// Takes the given number of elements from the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="Enumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> Enumerable, UInt32 Count)
        {

            var IEnumerator = Enumerable.GetEnumerator();

            for (var i = 0U; i < Count; i++)
            {

                if (IEnumerator.MoveNext())
                    yield return IEnumerator.Current;

                else
                    break;

            }

        }

        /// <summary>
        /// Takes the given number of elements from the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="Enumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> Enumerable, UInt32? Count)
        {

            if (Count.HasValue)
                return Enumerable.Take(Count.Value);

            return Enumerable;

        }

        /// <summary>
        /// Skips the given number of elements in the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> IEnumerable, UInt64 Count)
        {

            var IEnumerator = IEnumerable.GetEnumerator();

            for (var i = 0UL; i < Count; i++)
            {

                if (IEnumerator.MoveNext())
                    yield return IEnumerator.Current;

                else
                    break;

            }

        }

        /// <summary>
        /// Takes the given number of elements from the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="Enumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> Enumerable, UInt64? Count)
        {

            if (Count.HasValue)
                return Enumerable.Take(Count.Value);

            return Enumerable;

        }

        /// <summary>
        /// Skips the given number of elements in the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> IEnumerable, Int64 Count)
        {

            var IEnumerator = IEnumerable.GetEnumerator();

            for (var i = 0L; i < Count; i++)
            {

                if (IEnumerator.MoveNext())
                    yield return IEnumerator.Current;

                else
                    break;

            }

        }

        /// <summary>
        /// Takes the given number of elements from the enumeration.
        /// </summary>
        /// <typeparam name="T">The type fo the enumeration.</typeparam>
        /// <param name="Enumerable">An enumeration.</param>
        /// <param name="Count">The number of elements to skip.</param>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> Enumerable, Int64? Count)
        {

            if (Count.HasValue)
                return Enumerable.Take(Count.Value);

            return Enumerable;

        }

        #endregion

        #region SkipTakeFilter(this Enumeration, Skip = null, Take = null)

        /// <summary>
        /// Return a JSON representation for the given enumeration of roaming networks.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="Enumeration">An enumeration of roaming networks.</param>
        /// <param name="Skip">The optional number of roaming networks to skip.</param>
        /// <param name="Take">The optional number of roaming networks to return.</param>
        public static IEnumerable<T> SkipTakeFilter<T>(this IEnumerable<T>  Enumeration,
                                                       UInt64?              Skip  = null,
                                                       UInt64?              Take  = null)
        {

            #region Initial checks

            if (Enumeration is null)
                return [];

            #endregion

            if (Skip.HasValue  &&  Take.HasValue)
                return Enumeration.Skip(Skip).Take(Take);

            if (!Skip.HasValue &&  Take.HasValue)
                return Enumeration.Take(Take);

            if ( Skip.HasValue && !Take.HasValue)
                return Enumeration.Skip(Skip);

            return Enumeration;

        }

        #endregion

        #region IsNullOrEmpty<T>(this Enumerable)

        /// <summary>
        /// The given enumeration is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the enumeration.</typeparam>
        /// <param name="Enumerable">An enumeration.</param>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> Enumerable)
        {

            if (Enumerable is null || !Enumerable.Any())
                return true;

            return false;

        }

        #endregion

        #region IsNeitherNullNorEmpty<T>(this Enumerable)

        public static Boolean IsNeitherNullNorEmpty<T>(this IEnumerable<T> Enumerable)
        {

            if (Enumerable is null)
                return false;

            return Enumerable.Any();

        }

        #endregion


        #region CountIsAtLeast<T>(this Enumerable, NumberOfElements)

        public static Boolean CountIsAtLeast<T>(this IEnumerable<T> Enumerable, UInt64 NumberOfElements)
        {

            if (Enumerable is null)
                return false;

            var enumerator = Enumerable.GetEnumerator();

            while (NumberOfElements > 0 && enumerator.MoveNext())
                NumberOfElements--;

            return NumberOfElements == 0 && !enumerator.MoveNext();

        }

        #endregion

        #region CountIsGreater<T>(this Enumerable, NumberOfElements)

        public static Boolean CountIsGreater<T>(this IEnumerable<T> Enumerable, UInt64 NumberOfElements)
        {

            if (Enumerable is null)
                return false;

            var enumerator = Enumerable.GetEnumerator();

            while (NumberOfElements > 0 && enumerator.MoveNext())
                NumberOfElements--;

            return NumberOfElements == 0 && enumerator.MoveNext();

        }

        #endregion

        #region CountIsGreaterOrEquals<T>(this Enumerable, NumberOfElements)

        public static Boolean CountIsGreaterOrEquals<T>(this IEnumerable<T> Enumerable, UInt64 NumberOfElements)
        {

            if (Enumerable is null)
                return false;

            var enumerator = Enumerable.GetEnumerator();

            while (NumberOfElements > 0 && enumerator.MoveNext())
                NumberOfElements--;

            return NumberOfElements == 0;

        }

        #endregion


        #region When(this Object)

        ///// <summary>
        ///// Return the given object, when the condition delegate returns true.
        ///// Otherwise return default(T).
        ///// </summary>
        ///// <typeparam name="T">The type of the object.</typeparam>
        ///// <param name="Object">An object.</param>
        ///// <param name="ConditionDelegate">A delegate for checking some condition.</param>
        ///// <returns>The object if the condition is true; default(T) otherwise.</returns>
        //public static T When<T>(this T Object, Func<T, Boolean> ConditionDelegate)
        //{

        //    if (ConditionDelegate is null)
        //        throw new ArgumentNullException("ConditionDelegate", "The ConditionDelegate must not be null!");

        //    if (Object is null)
        //        return default(T);

        //    if (ConditionDelegate(Object))
        //        return Object;

        //    return default(T);

        //}

        #endregion

        #region SafeAll(this IEnumerable, Check)

        /// <summary>
        /// Safely determines whether all elements of a sequence satisfy a condition..
        /// </summary>
        /// <typeparam name="TSource">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="Check">A delegate to verify the given condition for the given enumeration.</param>
        public static Boolean SafeAll<TSource>(this IEnumerable<TSource>  IEnumerable,
                                               Func<TSource, Boolean>     Check)
        {

            if (IEnumerable is null || Check is null)
                return false;

            return IEnumerable.All(Check);

        }

        #endregion

        #region SafeAny(this IEnumerable, Filter = null)

        /// <summary>
        /// Safely determines whether a sequence contains any elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="Filter">An optional delegate to filter the given enumeration.</param>
        public static Boolean SafeAny<TSource>([NotNullWhen(true)] this IEnumerable<TSource>?  IEnumerable,
                                               Func<TSource, Boolean>?                         Filter   = null)
        {

            if (IEnumerable is null)
                return false;

            return Filter is not null
                       ? IEnumerable.Any(Filter)
                       : IEnumerable.Any();

        }

        #endregion

        #region SafeSelect(this IEnumerable, SelectionDelegate, DefaultValues = null)

        /// <summary>
        /// Safely selects the given enumeration.
        /// </summary>
        /// <typeparam name="TSource">The type of the enumeration.</typeparam>
        /// <typeparam name="TResult">The type of the resulting enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="SelectionDelegate">The delegate to select the given enumeration.</param>
        /// <param name="DefaultValues">A default value.</param>
        public static IEnumerable<TResult> SafeSelect<TSource, TResult>(this IEnumerable<TSource>  IEnumerable,
                                                                        Func<TSource, TResult>     SelectionDelegate,
                                                                        IEnumerable<TResult>?      DefaultValues   = null)
        {

            DefaultValues ??= [];

            if (IEnumerable is null || SelectionDelegate is null)
                return DefaultValues;

            var items = IEnumerable.ToArray();

            if (items.Length == 0)
                return DefaultValues;

            return items.Select(SelectionDelegate);

        }

        #endregion

        #region SafeWhere(this IEnumerable, Filter)

        /// <summary>
        /// Safely filters the given enumeration.
        /// </summary>
        /// <typeparam name="TSource">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="Filter">An optional delegate to filter the given enumeration.</param>
        public static IEnumerable<TSource> SafeWhere<TSource>(this IEnumerable<TSource>  IEnumerable,
                                                              Func<TSource, Boolean>     Filter)
        {

            if (IEnumerable is null || Filter is null)
                return [];

            return IEnumerable.Where(Filter);

        }

        #endregion

        #region ToSafeHashSet(this IEnumerable)

        /// <summary>
        /// Safely determines whether a sequence contains any elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the enumeration.</typeparam>
        public static HashSet<TSource> ToSafeHashSet<TSource>(this IEnumerable<TSource> IEnumerable)
        {

            if (IEnumerable is null || !IEnumerable.Any())
                return [];

            return new HashSet<TSource>(IEnumerable);

        }

        #endregion

        #region SelectIgnoreErrors(this IEnumerable, SelectionDelegate, DefaultValues = null)

        /// <summary>
        /// Safely selects the given enumeration.
        /// </summary>
        /// <typeparam name="TSource">The type of the enumeration.</typeparam>
        /// <typeparam name="TResult">The type of the resulting enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="SelectionDelegate">The delegate to select the given enumeration.</param>
        /// <param name="DefaultValues">A default value.</param>
        public static IEnumerable<TResult> SelectIgnoreErrors<TSource, TResult>(this IEnumerable<TSource>   IEnumerable,
                                                                                Func<TSource, TResult>      SelectionDelegate,
                                                                                IEnumerable<TResult>?       DefaultValues   = null)
        {

            DefaultValues ??= [];

            if (IEnumerable is null || SelectionDelegate is null)
                return DefaultValues;

            var items = IEnumerable.ToArray();

            if (items.Length == 0)
                return DefaultValues;

            var outItems = new List<TResult>();

            foreach (var item in items)
            {
                try
                {
                    outItems.Add(SelectionDelegate(item));
                }
                catch
                { }
            }

            return outItems;

        }

        #endregion

        #region SafeSelectMany(this IEnumerable, SelectionDelegate, DefaultValues = null)

        /// <summary>
        /// Safely selects the given enumeration.
        /// </summary>
        /// <typeparam name="TSource">The type of the enumeration.</typeparam>
        /// <typeparam name="TResult">The type of the resulting enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration of an enumeration.</param>
        /// <param name="SelectionDelegate">The delegate to select the given enumeration.</param>
        public static IEnumerable<TResult> SafeSelectMany<TSource, TResult>(this IEnumerable<TSource>            IEnumerable,
                                                                            Func<TSource, IEnumerable<TResult>>  SelectionDelegate,
                                                                            IEnumerable<TResult>?                DefaultValues   = null)
        {

            if (DefaultValues is null)
                DefaultValues = [];

            if (IEnumerable is null || SelectionDelegate is null)
                return DefaultValues;

            var Items = IEnumerable.ToArray();

            if (Items.Length == 0)
                return [];

            return Items.SelectMany(SelectionDelegate);

        }

        #endregion


        #region AggregateOrDefault (this Enumeration, AggreationDelegate, DefaultValue = default)

        /// <summary>
        /// Safely aggregates the given enumeration. If the enumeration is null
        /// or has no elements the default value will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="Enumeration">An enumeration.</param>
        /// <param name="AggregationDelegate">The delegate to aggregate the given enumeration.</param>
        /// <param name="DefaultValue">The default value to return for an empty enumeration.</param>
        public static T? AggregateOrDefault<T>(this IEnumerable<T>  Enumeration,
                                               Func<T, T, T>        AggregationDelegate,
                                               T?                   DefaultValue   = default)
        {

            if (!Enumeration.Any())
                return DefaultValue;

            var array = Enumeration.ToArray();

            if (array.Length == 0)
                return DefaultValue;

            try
            {
                return array.Aggregate(AggregationDelegate);
            }
            catch
            {
                return DefaultValue;
            }

        }

        #endregion

        #region AggregateOrDefault (this Enumeration, Prefix, Map, Reduce, Suffix, DefaultT = default)

        /// <summary>
        /// Safely aggregates the given enumeration. If the enumeration is null
        /// or has no elements the default value will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="Enumeration">An enumeration.</param>
        public static T? AggregateOrDefault<T>(this IEnumerable<T>  Enumeration,
                                               T                    Prefix,
                                               Func<T, T>           Map,
                                               Func<T, T, T>        Reduce,
                                               T                    Suffix,
                                               T?                   DefaultValue = default)
        {

            if (!Enumeration.Any())
                return DefaultValue;

            var array = Enumeration.Select(i => Map(i)).ToArray();

            try
            {
                return Reduce(Reduce(Prefix, array.Aggregate(Reduce)), Suffix);
            }
            catch
            {
                return DefaultValue;
            }

        }

        #endregion

        #region AggregateWith      (this Enumeration, Separator, DefaultValue = "")

        /// <summary>
        /// Safely aggregates the given enumeration. If the enumeration is null
        /// or has no elements an empty string will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="Enumeration">An enumeration.</param>
        /// <param name="Separator">A string as element separator.</param>
        /// <param name="DefaultValue">The default value to return for an empty enumeration.</param>
        public static String AggregateWith<T>(this IEnumerable<T>  Enumeration,
                                              Char                 Separator,
                                              String               DefaultValue   = "")
        {

            if (!Enumeration.Any())
                return DefaultValue;

            return String.Join(
                       Separator,
                       Enumeration.Select(element => element?.ToString() ?? "")
                   );

           // return Enumeration.
           //            //Where (element => !EqualityComparer<T>.Default.Equals(element, default)).
           //            Select(element => element?.ToString() ?? "").
           //            AggregateOrDefault((a, b) => a + Separator + b,
           //                               DefaultValue)
           //
           //            ?? DefaultValue;

        }

        #endregion

        #region AggregateWith      (this Enumeration, Separator, DefaultValue = "")

        /// <summary>
        /// Safely aggregates the given enumeration. If the enumeration is null
        /// or has no elements an empty string will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="Enumeration">An enumeration.</param>
        /// <param name="Separator">A string as element separator.</param>
        /// <param name="DefaultValue">The default value to return for an empty enumeration.</param>
        public static String AggregateWith<T>(this IEnumerable<T>  Enumeration,
                                              String               Separator,
                                              String               DefaultValue   = "")
        {

            if (!Enumeration.Any())
                return DefaultValue;

            return String.Join(
                       Separator,
                       Enumeration.Select(element => element?.ToString() ?? "")
                   );

            //return Enumeration.
            //           Select(element => element?.ToString() ?? "").
            //           AggregateOrDefault((a, b) => a + Separator + b,
            //                              DefaultValue)

            //           ?? DefaultValue;

        }

        #endregion

        #region Aggregate          (this EnumerationOfStrings, DefaultValue = "")

        public static String Aggregate(this IEnumerable<String>  EnumerationOfStrings,
                                       String                    DefaultValue   = "")
        {

            if (!EnumerationOfStrings.Any())
                return DefaultValue;

            return String.Concat(EnumerationOfStrings);

        }

        #endregion

        #region CSVAggregate       (this Enumeration, DefaultValue = "")

        private const String csv = ", ";

        public static String AggregateCSV<T>(this IEnumerable<T>  Enumeration,
                                             String               DefaultValue   = "")

            => Enumeration.AggregateWith(
                   csv,
                   DefaultValue
               );

        #endregion


        #region ToPartitions(this IEnumerable, SizeOfPartition)

        public static IEnumerable<IEnumerable<T>> ToPartitions<T>(this IEnumerable<T> IEnumerable, UInt64 SizeOfPartition)
        {

            UInt64 i;
            T[] Partitions;
            var IEnumerator = IEnumerable.GetEnumerator();

            while (IEnumerator.MoveNext())
            {

                Partitions    = new T[SizeOfPartition];
                Partitions[0] = IEnumerator.Current;
                i             = 1UL;

                while (i < SizeOfPartition && IEnumerator.MoveNext())
                {
                    Partitions[i] = IEnumerator.Current;
                    i = i + 1;
                }

                if (i < SizeOfPartition)
                    Partitions = Partitions.Take(i).ToArray();

                yield return Partitions;

            }

        }

        #endregion

        #region ConsumeAll<T>(this IEnumerator)

        /// <summary>
        /// Consume all elements of the given enumerator.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="Enumerator">An IEnumerator.</param>
        /// <returns>An enumerable of T.</returns>
        public static IEnumerable<T> ConsumeAll<T>(this IEnumerator Enumerator)
        {

            var List = new List<T>();

            while (Enumerator.MoveNext())
                List.Add((T) Enumerator.Current);

            return List;

        }

        #endregion

        #region ConsumeAll<T>(this IEnumerator)

        /// <summary>
        /// Consume all elements of the given enumerator.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="Enumerator">An IEnumerator.</param>
        /// <returns>An enumerable of T.</returns>
        public static IEnumerable<T> ConsumeAll<T>(this IEnumerator<T> Enumerator)
        {

            var List = new List<T>();

            while (Enumerator.MoveNext())
                List.Add((T) Enumerator.Current);

            return List;

        }

        #endregion

        #region Swap<T>(this IEnumerator)

        public static IEnumerable<T> Swap<T>(this IEnumerable<T> IEnumerable)
        {

            var enumerator = IEnumerable.GetEnumerator();

            T a = default;
            T b = default;
            Byte Emit = 0;

            while (enumerator.MoveNext())
            {

                if (Emit == 0)
                {
                    a = enumerator.Current;
                    Emit++;
                }

                else
                {
                    b = enumerator.Current;
                    yield return b;
                    yield return a;
                    Emit = 0;
                }

            }

            if (Emit == 1)
                yield return a;

        }

        #endregion

        #region ToHashSet<T>(this Enumeration)

        //public static HashSet<T> ToHashSet2<T>(this IEnumerable<T> Enumeration)
        //{

        //    if (Enumeration is null)
        //        return new HashSet<T>();

        //    var All = Enumeration.ToArray();

        //    if (All.Length == 0)
        //        return new HashSet<T>();

        //    return new HashSet<T>(All);

        //}

        #endregion

        #region ToHashedSet<T>(this Enumeration)

        public static HashedSet<T> ToHashedSet<T>(this IEnumerable<T> Enumeration)

            => new (Enumeration);

        #endregion


        #region WithFirstDo<T>(this Enumeration, Delegate)

        public static void WithFirstDo<T>(this IEnumerable<T>  Enumeration,
                                          Action<T>            Delegate)
        {

            if (Enumeration is null || Delegate is null)
                return;

            var firstItem = Enumeration.FirstOrDefault();

            if (firstItem is not null)
                Delegate(firstItem);

        }

        #endregion

        #region MapFirst<T>(this Enumeration, Delegate)

        public static T2 MapFirst<T, T2>(this IEnumerable<T>  Enumeration,
                                         Func<T, T2>          Delegate,
                                         T2                   DefaultValue)
        {

            if (Enumeration is null || Delegate is null)
                return DefaultValue;

            var firstItem = Enumeration.FirstOrDefault();

            if (firstItem is not null)
                return Delegate(firstItem);

            return DefaultValue;

        }

        #endregion


        #region ULongCount(this Enumeration)

        public static UInt64 ULongCount<T>(this IEnumerable<T> Enumeration)

            => (UInt64) Enumeration.LongCount();

        #endregion

        #region ULongCount(this Enumeration, Predicate)

        public static UInt64 ULongCount<T>(this IEnumerable<T>  Enumeration,
                                           Func<T, Boolean>     Predicate)

            => (UInt64) Enumeration.LongCount(Predicate);

        #endregion


        #region Sum(Source, Selector)

        /// <summary>
        /// Computes the sum of the sequence of System.UInt64 values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="Source">A sequence of values that are used to calculate a sum.</param>
        /// <param name="Selector">A transform function to apply to each element.</param>
        public static UInt64 Sum<TSource>(this IEnumerable<TSource>  Source,
                                          Func<TSource, UInt64>      Selector)
        {

            if (Source is null || Selector is null)
                return 0;

            var sum = 0UL;

            foreach (var item in Source)
                sum += Selector(item);

            return sum;

        }

        #endregion


        #region ReverseAndReturn(this Enumeration)

        /// <summary>
        /// Reverse and return the given enumeration of elements;
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="Enumeration">An enumeration of elements.</param>
        public static IEnumerable<T> ReverseAndReturn<T>(this IEnumerable<T> Enumeration)
        {

            var list = new List<T>(Enumeration);
            list.Reverse();

            return list;

        }

        #endregion


    }

}
