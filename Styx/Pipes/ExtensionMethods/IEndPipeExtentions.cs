/*
 * Copyright (c) 2010-2014, Achim 'ahzf' Friedland <achim@graphdefined.org>
 * This file is part of Styx <http://www.github.com/Vanaheimr/Styx>
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
using System.Collections;
using System.Collections.Generic;

using eu.Vanaheimr.Illias.Commons.Collections;

#endregion

namespace eu.Vanaheimr.Styx
{

    public static class IEEnumerableExtentions
    {

        #region Any<T>(this Pipe)

        public static Boolean Any<T>(this IEndPipe<T>  Pipe)
        {

            //ToDo: Will not happen! But perhaps we have some pipes which know
            //      their number of elements!
            var collection = Pipe as ICollection<T>;
            if (collection != null)
                return collection.Count > 0;

            if (Pipe == null)
                return false;

            using (var enumerator = Pipe)//.GetEnumerator())
                return enumerator.MoveNext();

        }

        #endregion

        #region Any<T>(this Pipe, Include)

        public static Boolean Any<T>(this IEndPipe<T>  Pipe,
                                     Func<T, Boolean>  Include)
        {

            foreach (var Item in Pipe)
                if (Include(Item))
                    return true;

            return false;

        }

        #endregion

        #region Contains<T>(this Pipe, Value, Comparer = null)

        public static Boolean Contains<T>(this IEndPipe<T>      Pipe,
                                          T                     Value,
                                          IEqualityComparer<T>  Comparer = null)
        {

            if (Pipe == null)
                return false;

            //ToDo: Will not happen! But perhaps we have some pipes
            //      which know their elements!
            var collection = Pipe as ICollection<T>;
            if (collection != null)
                return collection.Contains(Value);

            if (Comparer == null)
                Comparer = EqualityComparer<T>.Default;

            foreach (var Item in Pipe)
                if (Comparer.Equals(Item, Value))
                    return true;

            return false;

        }

        #endregion

        #region Count<T>(this Pipe, Include = null)

        public static UInt64 Count<T>(this IEndPipe<T>  Pipe,
                                      Func<T, Boolean>  Include = null)
        {

            if (Pipe == null)
                return 0UL;

            //ToDo: Will not happen! But perhaps we have some pipes which know
            //      their number of elements!
            var collection = Pipe as ICollection<T>;
            if (Include == null && collection != null)
                return (UInt64) collection.Count;

            var Counter = 0UL;

            if (Include == null)
            {
                foreach (var Item in Pipe)
                    if (Include(Item))
                        Counter++;
            }

            else
                foreach (var Item in Pipe)
                    Counter++;

            return Counter;

        }

        #endregion

        #region First<T>(this Pipe, Include = null, DefaultValue = default(T))

        public static T First<T>(this IEndPipe<T>  Pipe,
                                 Func<T, Boolean>  Include       = null,
                                 T                 DefaultValue  = default(T))
        {

            if (Pipe == null)
                return DefaultValue;

            if (Include == null)
                foreach (var Item in Pipe)
                    return Item;

            foreach (var Item in Pipe)
                if (Include(Item))
                    return Item;

            return DefaultValue;

        }

        #endregion

        // GroupBy

        // GroupJoin

        #region ++Intersect(this FirstPipe, SecondPipe, Comparer = null)

        //public static IEnumerable<T> Intersect<T>(this IEndPipe<T>      FirstPipe,
        //                                               IEndPipe<T>      SecondPipe,
        //                                          IEqualityComparer<T>  Comparer = null)
        //{

        //    if (FirstPipe == null || SecondPipe == null)
        //        yield break;

        //    if (Comparer == null)
        //        Comparer = EqualityComparer<T>.Default;

        //    var ValueSet = new HashSet<T>(Comparer);

        //    foreach (var Item in SecondPipe)
        //        ValueSet.Add(Item);

        //    // If an element occures multiple time in FirstPipe
        //    // it will be returned just once (implicit FirstPipe.Distinct())
        //    foreach (var Item in FirstPipe)
        //    {

        //        if (ValueSet.Remove(Item))
        //            yield return Item;

        //        if (ValueSet.Count == 0)
        //            yield break;

        //    }

        //}

        #endregion

        // Join

        #region Last<T>(this SourcePipe, Include = null, DefaultValue = default(T))

        public static T Last<T>(this IEndPipe<T>  SourcePipe,
                                Func<T, Boolean>  Include       = null,
                                T                 DefaultValue  = default(T))
        {

            if (SourcePipe == null)
                return DefaultValue;

            T Value = DefaultValue;

            foreach (var Item in SourcePipe)
                if (Include(Item))
                    Value = Item;

            return Value;

        }

        #endregion

        // Min, Max

        #region ++OfType

        //public static IEnumerable<T2> OfType<T1, T2>(this IEndPipe<T1> Pipe)
        //{

        //    foreach (var Item in Pipe)
        //        if (Item is T2)
        //            yield return (T2) (Object) Item;

        //}

        #endregion

        // OrderBy

        // Range

        #region ++Repeat

        //public static IEnumerable<T> Repeat<T>(T Element, UInt64 Count)
        //{

        //    for (var i = 0UL; i < Count; i++)
        //        yield return Element;

        //}

        #endregion

        #region ++Reverse

        //public static IEnumerable<T> Reverse<T>(this IEndPipe<T> Pipe)
        //{

        //    var array = Pipe.ToArray();

        //    for (var i = array.Length - 1; i >= 0; i--)
        //        yield return array[i];

        //}

        #endregion

        #region ++SelectMany

        public static IEnumerable<E> SelectMany<S, E>(this IEndPipe<S>         source,
                                                      Func<S, IEnumerable<E>>  selector)
        {
            foreach (var element in source)
                foreach (var item in selector(element))
                    yield return item;
        }

        public static IEnumerable<E> SelectMany<S, E>(this IEndPipe<S> source,
                                                      Func<S, UInt64, IEnumerable<E>> selector)
        {
            var counter = 1UL;
            foreach (var element in source)
            {
                foreach (var item in selector(element, counter))
                    yield return item;
                counter++;
            }
        }

        public static IEnumerable<E> SelectMany<S, TCollection, E>(this IEndPipe<S> source,
                                                                   Func<S, IEnumerable<TCollection>> collectionSelector,
                                                                   Func<S, TCollection, E> selector)
        {
            foreach (S element in source)
                foreach (TCollection collection in collectionSelector(element))
                    yield return selector(element, collection);
        }

        public static IEnumerable<E> SelectMany<S, TCollection, E>(this IEndPipe<S> source,
                                                                   Func<S, UInt64, IEnumerable<TCollection>> collectionSelector,
                                                                   Func<S, TCollection, E> selector)
        {
            var counter = 1UL;
            foreach (S element in source)
                foreach (TCollection collection in collectionSelector(element, counter++))
                    yield return selector(element, collection);
        }

        #endregion

        #region ++Skip

        //public static IEnumerable<T> Skip<T>(this IEndPipe<T> SourcePipe, UInt64 count)
        //{

        //    var enumerator = source.GetEnumerator();

        //    try
        //    {

        //        while (count-- > 0)
        //            if (!enumerator.MoveNext())
        //                yield break;

        //        while (enumerator.MoveNext())
        //            yield return enumerator.Current;

        //    }
        //    finally
        //    {
        //        enumerator.Dispose();
        //    }

        //}

        #endregion

        #region ++SkipWhile

        //public static IEnumerable<TSource> SkipWhile<TSource>(this IEndPipe<TSource> source, Func<TSource, bool> predicate)
        //{
        //    bool yield = false;

        //    foreach (TSource element in source)
        //    {
        //        if (yield)
        //            yield return element;
        //        else
        //            if (!predicate(element))
        //            {
        //                yield return element;
        //                yield = true;
        //            }
        //    }
        //}

        //public static IEnumerable<TSource> SkipWhile<TSource>(this IEndPipe<TSource> source, Func<TSource, int, bool> predicate)
        //{
        //    int counter = 0;
        //    bool yield = false;

        //    foreach (TSource element in source)
        //    {
        //        if (yield)
        //            yield return element;
        //        else
        //            if (!predicate(element, counter))
        //            {
        //                yield return element;
        //                yield = true;
        //            }
        //        counter++;
        //    }
        //}

        #endregion

        // Sum

        #region ++Take

        //public static IEnumerable<TSource> Take<TSource>(this IEndPipe<TSource> source, int count)
        //{
        //   if (count <= 0)
        //        yield break;

        //    int counter = 0;
        //    foreach (TSource element in source)
        //    {
        //        yield return element;

        //        if (++counter == count)
        //            yield break;
        //    }
        //}

        #endregion

        #region ++TakeWhile

        //public static IEnumerable<TSource> TakeWhile<TSource>(this IEndPipe<TSource> source, Func<TSource, bool> predicate)
        //{
        //    foreach (var element in source)
        //    {
        //        if (!predicate(element))
        //            yield break;

        //        yield return element;
        //    }
        //}

        //public static IEnumerable<TSource> TakeWhile<TSource>(this IEndPipe<TSource> source, Func<TSource, int, bool> predicate)
        //{
        //    int counter = 0;
        //    foreach (var element in source)
        //    {
        //        if (!predicate(element, counter))
        //            yield break;

        //        yield return element;
        //        counter++;
        //    }
        //}

        #endregion

        // ThenBy

        #region ToArray(this SourcePipe, ResetPipeBefore = true, ResetPipeAfter = true)

        /// <summary>
        /// Copies the elements of the given pipe into a new array.
        /// </summary>
        /// <typeparam name="T">The type of the emitting objects.</typeparam>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="ResetPipe">Reset the pipe after operation.</param>
        public static T[] ToArray<T>(this IEndPipe<T>  SourcePipe,
                                     Boolean           ResetPipeBefore  = true,
                                     Boolean           ResetPipeAfter   = true)
        {

            new List<String>().ToArray();
            var Values = new T[8];
            var Size   = 0;

            if (ResetPipeBefore)
                SourcePipe.Reset();

            foreach (var Item in SourcePipe)
            {

                if (Size == Values.Length)
                    Array.Resize(ref Values, Size * 2);

                Values[Size++] = Item;

            }

            if (Size != Values.Length)
                Array.Resize(ref Values, Size);

            if (ResetPipeAfter)
                SourcePipe.Reset();

            return Values;

        }

        #endregion

        #region ToDictionary(this SourcePipe, KeySelector, ValueSelector, Comparer = null, ResetPipeBefore = true, ResetPipeAfter = true)

        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEndPipe<T>         SourcePipe,
                                                                             Func<T, TKey>            KeySelector,
                                                                             Func<T, TValue>          ValueSelector,
                                                                             IEqualityComparer<TKey>  Comparer         = null,
                                                                             Boolean                  ResetPipeBefore  = true,
                                                                             Boolean                  ResetPipeAfter   = true)
        {

            if (Comparer == null)
                Comparer = EqualityComparer<TKey>.Default;

            var Dictionary = new Dictionary<TKey, TValue>(Comparer);

            if (ResetPipeBefore)
                SourcePipe.Reset();

            foreach (var Item in SourcePipe)
                Dictionary.Add(KeySelector  (Item),
                               ValueSelector(Item));

            if (ResetPipeAfter)
                SourcePipe.Reset();

            return Dictionary;

        }

        #endregion

        #region ToList(this SourcePipe, ResetPipeBefore = true, ResetPipeAfter = true)

        public static List<T> ToList<T>(this IEndPipe<T>  SourcePipe,
                                        Boolean           ResetPipeBefore  = true,
                                        Boolean           ResetPipeAfter   = true)
        {

            var List = new List<T>();

            if (ResetPipeBefore)
                SourcePipe.Reset();

            foreach (var Item in SourcePipe)
                List.Add(Item);

            if (ResetPipeAfter)
                SourcePipe.Reset();

            return List;

        }

        #endregion

        // ToLookup

        #region SequenceEqual

        public static bool SequenceEqual<T>(this IEndPipe<T> first,
                                                 IEndPipe<T> second,
                                            IEqualityComparer<T> comparer = null)
        {

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            using (IEnumerator<T> first_enumerator = first.GetEnumerator(),
                    second_enumerator = second.GetEnumerator())
            {

                while (first_enumerator.MoveNext())
                {
                    if (!second_enumerator.MoveNext())
                        return false;

                    if (!comparer.Equals(first_enumerator.Current, second_enumerator.Current))
                        return false;
                }

                return !second_enumerator.MoveNext();

            }

        }

        #endregion

        #region ++Union

        //public static IEnumerable<TSource> Union<TSource>(this IEndPipe<TSource> first,
        //                                                       IEndPipe<TSource> second,
        //                                                  IEqualityComparer<TSource> comparer = null)
        //{

        //    if (comparer == null)
        //        comparer = EqualityComparer<TSource>.Default;

        //    var items = new HashSet<TSource>(comparer);
        //    foreach (var element in first)
        //    {
        //        if (!items.Contains(element))
        //        {
        //            items.Add(element);
        //            yield return element;
        //        }
        //    }

        //    foreach (var element in second)
        //    {
        //        if (!items.Contains(element))
        //        {
        //            items.Add(element);
        //            yield return element;
        //        }
        //    }
        //}

        #endregion


        // Additionals...

        #region Aggregate(this SourcePipe, AggreationDelegate, DefaultT = default(T))

        /// <summary>
        /// Safely aggregates the given enumeration. If the enumeration is null
        /// or has no elements the default value will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="AggreationDelegate">The delegate to aggregate the given enumeration.</param>
        /// <param name="DefaultT">The default value to return for an empty enumeration.</param>
        public static T Aggregate<T>(this IEndPipe<T> SourcePipe,
                                     Func<T, T, T>    AggreationDelegate,
                                     T                DefaultT = default(T))
        {

            if (SourcePipe == null)
                return DefaultT;

            //if (!IEnumerable.Any())
            //    return DefaultT;
            try
            {
                return SourcePipe.AsEnumerable().Aggregate(AggreationDelegate);
            }
            catch (Exception e)
            {
                return DefaultT;
            }

        }

        #endregion

        #region Aggregate(this SourcePipe, AggreationDelegate, DefaultT = default(T))

        /// <summary>
        /// Safely aggregates the given enumeration. If the enumeration is null
        /// or has no elements the default value will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="IEnumerable">An enumeration.</param>
        /// <param name="AggreationDelegate">The delegate to aggregate the given enumeration.</param>
        /// <param name="DefaultT">The default value to return for an empty enumeration.</param>
        public static T Aggregate<T>(this IEndPipe<T>  SourcePipe,
                                     T                 Prefix,
                                     Func<T, T>        Map,
                                     Func<T, T, T>     Reduce,
                                     T                 Suffix,
                                     T                 DefaultValue = default(T))
        {

            if (SourcePipe == null)
                return DefaultValue;

            var _Array = SourcePipe.Select(i => Map(i)).ToArray();

            //if (!IEnumerable.Any())
            //    return DefaultT;
            try
            {
                return Reduce(Reduce(Prefix, _Array.Aggregate(Reduce)), Suffix);
            }
            catch (Exception e)
            {
                return DefaultValue;
            }

        }

        #endregion

    }

}
