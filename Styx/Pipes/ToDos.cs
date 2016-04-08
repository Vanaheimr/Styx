/*
 * Copyright (c) 2010-2016, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    /// <summary>
    /// Extentions methods for the IEndPipe interface.
    /// </summary>
    public static partial class IEndPipeExtentions
    {

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

        // Average, Min, Max

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

        //public static IEnumerable<E> SelectMany<S, E>(this IEndPipe<S>         source,
        //                                              Func<S, IEnumerable<E>>  selector)
        //{
        //    foreach (var element in source)
        //        foreach (var item in selector(element))
        //            yield return item;
        //}

        //public static IEnumerable<E> SelectMany<S, E>(this IEndPipe<S> source,
        //                                              Func<S, UInt64, IEnumerable<E>> selector)
        //{
        //    var counter = 1UL;
        //    foreach (var element in source)
        //    {
        //        foreach (var item in selector(element, counter))
        //            yield return item;
        //        counter++;
        //    }
        //}

        //public static IEnumerable<E> SelectMany<S, TCollection, E>(this IEndPipe<S> source,
        //                                                           Func<S, IEnumerable<TCollection>> collectionSelector,
        //                                                           Func<S, TCollection, E> selector)
        //{
        //    foreach (S element in source)
        //        foreach (TCollection collection in collectionSelector(element))
        //            yield return selector(element, collection);
        //}

        //public static IEnumerable<E> SelectMany<S, TCollection, E>(this IEndPipe<S> source,
        //                                                           Func<S, UInt64, IEnumerable<TCollection>> collectionSelector,
        //                                                           Func<S, TCollection, E> selector)
        //{
        //    var counter = 1UL;
        //    foreach (S element in source)
        //        foreach (TCollection collection in collectionSelector(element, counter++))
        //            yield return selector(element, collection);
        //}

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

        // ToLookup

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

    }

}
