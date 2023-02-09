/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    #region ComparisonFilter delegate

    /// <summary>
    /// A delegate for comparisions.
    /// </summary>
    /// <typeparam name="TValue">The type of the item to compare.</typeparam>
    /// <param name="Expected">The expected value of the item.</param>
    /// <returns>True if matches; False otherwise.</returns>
    public delegate Boolean ComparisonFilter<in TValue>(TValue Expected);

    #endregion

    #region APropertyFilterPipe<...>

    /// <summary>
    /// An abstract class for filtering objects by their properties.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <typeparam name="S">The type of the objects to filter.</typeparam>
    public abstract class APropertyFilterPipe<TKey, TValue, S> : AbstractPipe<S, S>, IFilterPipe<S>
        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        where S    : IReadOnlyProperties<TKey, TValue>
    {

        #region Data

        private readonly TKey                     Key;
        private readonly ComparisonFilter<TValue> ComparisonFilter;
        private          TValue                   ActualValue;

        #endregion

        #region Constructor(s)

        #region APropertyFilterPipe(SourcePipe, Key, ComparisonFilter)

        /// <summary>
        /// Creates a new property filter pipe.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="ComparisonFilter">The comparison filter to use.</param>
        public APropertyFilterPipe(IEndPipe<S>               SourcePipe,
                                   TKey                      Key,
                                   ComparisonFilter<TValue>  ComparisonFilter)

            : base(SourcePipe)

        {

            ComparisonFilter.CheckNull("ComparisonFilter");

            this.Key               = Key;
            this.ComparisonFilter  = ComparisonFilter;

        }

        #endregion

        #region APropertyFilterPipe(SourceEnumerator, Key, ComparisonFilter)

        /// <summary>
        /// Creates a new property filter pipe.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="ComparisonFilter">The comparison filter to use.</param>
        public APropertyFilterPipe(IEnumerator<S>            SourceEnumerator,
                                   TKey                      Key,
                                   ComparisonFilter<TValue>  ComparisonFilter)

            : base(SourceEnumerator)

        {

            ComparisonFilter.CheckNull("ComparisonFilter");

            this.Key               = Key;
            this.ComparisonFilter  = ComparisonFilter;

        }

        #endregion

        #region APropertyFilterPipe(SourceEnumerable, Key, ComparisonFilter)

        /// <summary>
        /// Creates a new property filter pipe.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="ComparisonFilter">The comparison filter to use.</param>
        public APropertyFilterPipe(IEnumerable<S>            SourceEnumerable,
                                   TKey                      Key,
                                   ComparisonFilter<TValue>  ComparisonFilter)

            : base(SourceEnumerable)

        {

            ComparisonFilter.CheckNull("ComparisonFilter");

            this.Key               = Key;
            this.ComparisonFilter  = ComparisonFilter;

        }

        #endregion

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (SourcePipe == null)
                return false;

            while (SourcePipe.MoveNext())
            {

                if (SourcePipe.Current.TryGetProperty(Key, out ActualValue))
                {

                    if (SourcePipe.Current.TryGetProperty(Key, out ActualValue))
                    {
                        if (ComparisonFilter(ActualValue))
                        {
                            _CurrentElement = SourcePipe.Current;
                            return true;
                        }
                    }

                }

            }

            return false;

        }

        #endregion

    }

    #endregion

    #region APropertyFilterPipe<...TCast>

    /// <summary>
    /// An abstract class for filtering objects by their properties.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <typeparam name="S">The type of the objects to filter.</typeparam>
    public abstract class APropertyFilterPipe<TKey, TValue, TCast, S> : AbstractPipe<S, S>, IFilterPipe<S>
        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        where S    : IReadOnlyProperties<TKey, TValue>
    {

        #region Data

        private readonly TKey Key;
        private readonly ComparisonFilter<TCast> ComparisonFilter;
        private TValue ActualValue;

        #endregion

        #region Constructor(s)

        #region APropertyFilterPipe(SourcePipe, Key, ComparisonFilter)

        /// <summary>
        /// Creates a new PropertyFilterPipe.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="ComparisonFilter">The comparison filter to use.</param>
        public APropertyFilterPipe(IEndPipe<S>              SourcePipe,
                                   TKey                     Key,
                                   ComparisonFilter<TCast>  ComparisonFilter)

            : base(SourcePipe)

        {

            #region Initial checks

            if (ComparisonFilter == null)
                throw new ArgumentNullException("ComparisonFilter", "The given ComparisonFilter delegate must not be null!");

            #endregion

            this.Key              = Key;
            this.ComparisonFilter = ComparisonFilter;

        }

        #endregion

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (SourcePipe == null)
                return false;

            while (SourcePipe.MoveNext())
            {

                if (SourcePipe.Current.TryGetProperty(Key, out ActualValue))
                {

                    try
                    {

                        if (ComparisonFilter((TCast) (Object) ActualValue))
                        {
                            _CurrentElement = SourcePipe.Current;
                            return true;
                        }

                    }

                    catch (InvalidCastException)
                    {
                        continue;
                    }

                }

            }

            return false;

        }

        #endregion

    }

    #endregion

}
