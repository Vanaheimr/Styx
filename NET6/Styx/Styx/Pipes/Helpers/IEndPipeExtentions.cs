/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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
    /// Extensions methods for the IEndPipe interface.
    /// </summary>
    public static partial class IEndPipeExtensions
    {

        // Anything which maps from IEndPipe to something unlike IEndPipe...

        #region Aggregate<T>(this SourcePipe, AggreationDelegate, DefaultValue = default(T))

        /// <summary>
        /// Safely aggregates the items emitted by the given pipe. If the pipe is null
        /// or has no elements the default value will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the items emitted by the pipe.</typeparam>
        /// <param name="SourcePipe">A pipe.</param>
        /// <param name="AggreationDelegate">The delegate to aggregate the items emitted by the pipe.</param>
        /// <param name="DefaultValue">The default value to return for an empty pipe.</param>
        public static T Aggregate<T>(this IEndPipe<T> SourcePipe,
                                     Func<T, T, T>    AggreationDelegate,
                                     T                DefaultValue = default(T))
        {

            #region Initial checks

            if (SourcePipe == null)
                return DefaultValue;

            if (AggreationDelegate == null)
                throw new ArgumentNullException("AggreationDelegate");

            #endregion

            using (var Enumerator = SourcePipe.GetEnumerator())
            {

                if (!Enumerator.MoveNext())
                    return DefaultValue;

                T Aggregated = Enumerator.Current;

                while (Enumerator.MoveNext())
                    Aggregated = AggreationDelegate(Aggregated, Enumerator.Current);

                return Aggregated;

            }

        }

        #endregion

        #region Aggregate<T>(this SourcePipe, Prefix, Map, Reduce, Suffix, DefaultT = default(T))

        /// <summary>
        /// Safely aggregates the items emitted by the given pipe. If the pipe is null
        /// or has no elements the default value will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the items emitted by the pipe.</typeparam>
        /// <param name="SourcePipe">A pipe.</param>
        /// <param name="DefaultValue">The default value to return for an empty pipe.</param>
        public static T Aggregate<T>(this IEndPipe<T>  SourcePipe,
                                     T                 Prefix,
                                     Func<T, T>        Map,
                                     Func<T, T, T>     Reduce,
                                     T                 Suffix,
                                     T                 DefaultValue = default(T))
        {

            if (SourcePipe == null)
                return DefaultValue;

            try
            {
                return Reduce(Reduce(Prefix, SourcePipe.Select(Item => Map(Item)).Aggregate(Reduce)), Suffix);
            }
            catch (Exception e)
            {
                return DefaultValue;
            }

        }

        #endregion

        #region All<T>(this SourcePipe, IncludeFilter)

        /// <summary>
        /// Determines whether all items of a pipe satisfy a condition.
        /// </summary>
        /// <typeparam name="T">The type of the items emitted by the pipe.</typeparam>
        /// <param name="SourcePipe">A pipe.</param>
        /// <param name="IncludeFilter">A delegate to test each item emitted by the pipe for a condition.</param>
        /// <returns>True if every item of the pipe passes the specified filter, or if the pipe is empty; otherwise, false.</returns>
        public static Boolean All<T>(this IEndPipe<T> SourcePipe, Func<T, Boolean> IncludeFilter)
        {

            if (SourcePipe == null)
                return true;

            foreach (var Item in SourcePipe)
                if (!IncludeFilter(Item))
                    return false;

            return true;

        }

        #endregion

        #region Any<T>(this SourcePipe)

        /// <summary>
        /// Determines whether a pipe emits any items.
        /// </summary>
        /// <typeparam name="T">The type of the items emitted by the pipe.</typeparam>
        /// <param name="SourcePipe">A pipe.</param>
        /// <returns>True if the pipe emits any items; otherwise, false.</returns>
        public static Boolean Any<T>(this IEndPipe<T> SourcePipe)
        {

            if (SourcePipe == null)
                return false;

            using (var Enumerator = SourcePipe)
                return Enumerator.MoveNext();

        }

        #endregion

        #region Any<T>(this SourcePipe, IncludeFilter)

        /// <summary>
        /// Determines whether any item emitted by a pipe satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the items emitted by the pipe.</typeparam>
        /// <param name="SourcePipe">A pipe.</param>
        /// <param name="IncludeFilter">A delegate to test each item emitted by the pipe for a condition.</param>
        /// <returns>True if the pipe emits any matching items; otherwise, false.</returns>
        public static Boolean Any<T>(this IEndPipe<T> SourcePipe, Func<T, Boolean> IncludeFilter)
        {

            if (SourcePipe == null)
                return false;

            foreach (var Item in SourcePipe)
                if (IncludeFilter(Item))
                    return true;

            return false;

        }

        #endregion

        #region Contains<T>(this SourcePipe, Value, ValueComparer = null)

        /// <summary>
        /// Determines whether a pipe emits the specified element by
        /// using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of the items emitted by the pipe.</typeparam>
        /// <param name="SourcePipe">A pipe.</param>
        /// <param name="Value">The value to locate in the pipe.</param>
        /// <param name="ValueComparer">An equality comparer to compare values.</param>
        /// <returns>True if the pipe contains an item that has the specified value; otherwise, false.</returns>
        public static Boolean Contains<T>(this IEndPipe<T>      SourcePipe,
                                          T                     Value,
                                          IEqualityComparer<T>  ValueComparer = null)
        {

            if (SourcePipe == null)
                return false;

            if (ValueComparer == null)
                ValueComparer = EqualityComparer<T>.Default;

            foreach (var Item in SourcePipe)
                if (ValueComparer.Equals(Item, Value))
                    return true;

            return false;

        }

        #endregion

        #region Count<T>(this SourcePipe, IncludeFilter = null)

        /// <summary>
        /// Returns the number of items in the given pipe satisfying an optional condition.
        /// </summary>
        /// <typeparam name="T">The type of the items emitted by the pipe.</typeparam>
        /// <param name="SourcePipe">A pipe.</param>
        /// <param name="IncludeFilter">A delegate to test each item emitted by the pipe for a condition.</param>
        public static UInt64 Count<T>(this IEndPipe<T>  SourcePipe,
                                      Func<T, Boolean>  IncludeFilter = null)
        {

            if (SourcePipe == null)
                return 0UL;

            var Counter = 0UL;

            if (IncludeFilter == null)
                foreach (var Item in SourcePipe)
                    Counter++;

            else
                foreach (var Item in SourcePipe)
                    if (IncludeFilter(Item))
                        Counter++;

            return Counter;

        }

        #endregion

        #region FirstOrDefault<T>(this SourcePipe, IncludeFilter = null, DefaultValue = default(T))

        /// <summary>
        /// Returns the first item of the given pipe that satisfies a condition
        /// or the given default value if no such item was found.
        /// </summary>
        /// <typeparam name="T">The type of the items emitted by the pipe.</typeparam>
        /// <param name="SourcePipe">A pipe.</param>
        /// <param name="IncludeFilter">A delegate to test each item emitted by the pipe for a condition.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static T FirstOrDefault<T>(this IEndPipe<T>  SourcePipe,
                                          Func<T, Boolean>  IncludeFilter  = null,
                                          T                 DefaultValue   = default(T))
        {

            if (SourcePipe == null)
                return DefaultValue;

            if (IncludeFilter == null)
                foreach (var Item in SourcePipe)
                    return Item;

            foreach (var Item in SourcePipe)
                if (IncludeFilter(Item))
                    return Item;

            return DefaultValue;

        }

        #endregion

        #region LastOrDefault<T>(this SourcePipe, IncludeFilter = null, DefaultValue = default(T))

        /// <summary>
        /// Returns the last item of the given pipe that satisfies a condition
        /// or the given default value if no such item was found.
        /// </summary>
        /// <typeparam name="T">The type of the items emitted by the pipe.</typeparam>
        /// <param name="SourcePipe">A pipe.</param>
        /// <param name="IncludeFilter">A delegate to test each item emitted by the pipe for a condition.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static T LastOrDefault<T>(this IEndPipe<T>  SourcePipe,
                                         Func<T, Boolean>  IncludeFilter  = null,
                                         T                 DefaultValue   = default(T))
        {

            if (SourcePipe == null)
                return DefaultValue;

            T Value = DefaultValue;

            foreach (var Item in SourcePipe)
                if (IncludeFilter(Item))
                    Value = Item;

            return Value;

        }

        #endregion

        #region SequenceEqual

        public static Boolean SequenceEqual<T>(this IEndPipe<T>      first,
                                               IEndPipe<T>           second,
                                               IEqualityComparer<T>  comparer = null)
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

        #region ToArray(this SourcePipe, ResetPipeBefore = false, ResetPipeAfter = false)

        /// <summary>
        /// Copies the elements of the given pipe into a new array.
        /// </summary>
        /// <typeparam name="T">The type of the emitting objects.</typeparam>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="ResetPipe">Reset the pipe after operation.</param>
        public static T[] ToArray<T>(this IEndPipe<T>  SourcePipe,
                                     Boolean           ResetPipeBefore  = false,
                                     Boolean           ResetPipeAfter   = false)
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

        #region ToDictionary(this SourcePipe, KeySelector, ValueSelector, Comparer = null, ResetPipeBefore = false, ResetPipeAfter = false)

        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEndPipe<T>         SourcePipe,
                                                                             Func<T, TKey>            KeySelector,
                                                                             Func<T, TValue>          ValueSelector,
                                                                             IEqualityComparer<TKey>  Comparer         = null,
                                                                             Boolean                  ResetPipeBefore  = false,
                                                                             Boolean                  ResetPipeAfter   = false)
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

        #region ToList(this SourcePipe, ResetPipeBefore = false, ResetPipeAfter = false)

        public static List<T> ToList<T>(this IEndPipe<T>  SourcePipe,
                                        Boolean           ResetPipeBefore  = false,
                                        Boolean           ResetPipeAfter   = false)
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


    }

}
