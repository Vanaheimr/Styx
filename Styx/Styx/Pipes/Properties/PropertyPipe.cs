﻿/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    #region PropertyPipeExtensions
    
    /// <summary>
    /// Extension methods for the PropertyPipe.
    /// </summary>
    public static class PropertyPipeExtensions
    {

        #region P<TKey, TValue>(this IReadOnlyProperties<...>, Keys)

        /// <summary>
        /// Emits the property values of the given property keys (OR-logic).
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="Properties">An object implementing IReadOnlyProperties&lt;TKey, TValue&gt;.</param>
        /// <param name="Keys">An array of property keys.</param>
        /// <returns>The property values of the given property keys.</returns>
        public static PropertyPipe<TKey, TValue> P<TKey, TValue>(this IReadOnlyProperties<TKey, TValue> Properties,
                                                                 params TKey[] Keys)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

        {
            return new PropertyPipe<TKey, TValue>(Properties, Keys);
        }

        #endregion

        #region P<TKey, TValue>(this IEnumerable<IReadOnlyProperties<...>>, Keys)

        /// <summary>
        /// Emits the property values of the given property keys (OR-logic).
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="IEnumerable">An enumeration of IReadOnlyProperties&lt;TKey, TValue&gt;.</param>
        /// <param name="Keys">An array of property keys.</param>
        /// <returns>The property values of the given property keys.</returns>
        public static PropertyPipe<TKey, TValue> P<TKey, TValue>(this IEndPipe<IReadOnlyProperties<TKey, TValue>>  SourcePipe,
                                                                 params TKey[]                                     Keys)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

        {
            return new PropertyPipe<TKey, TValue>(SourcePipe, Keys);
        }

        #endregion


        #region P<TKey, TValue>(this IReadOnlyProperties<...>, KeyValueFilter)

        /// <summary>
        /// Emits the property values filtered by the given keyvalue filter.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="Properties">An object implementing IReadOnlyProperties&lt;TKey, TValue&gt;.</param>
        /// <param name="KeyValueFilter">An optional delegate for keyvalue filtering.</param>
        /// <returns>The property values of the given property keys.</returns>
        public static PropertyPipe<TKey, TValue> P<TKey, TValue>(this IReadOnlyProperties<TKey, TValue> Properties,
                                                                 KeyValueFilter<TKey, TValue> KeyValueFilter)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {
            return new PropertyPipe<TKey, TValue>(Properties, KeyValueFilter);
        }

        #endregion

        #region P<TKey, TValue>(this IEnumerable<IReadOnlyProperties<...>>, KeyValueFilter)

        /// <summary>
        /// Emits the property values filtered by the given keyvalue filter.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="IEnumerable">An enumeration of IReadOnlyProperties&lt;TKey, TValue&gt;.</param>
        /// <param name="KeyValueFilter">An optional delegate for keyvalue filtering.</param>
        /// <returns>The property values of the given property keys.</returns>
        public static PropertyPipe<TKey, TValue> P<TKey, TValue>(this IEndPipe<IReadOnlyProperties<TKey, TValue>> SourcePipe,
                                                                 KeyValueFilter<TKey, TValue> KeyValueFilter)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {
            return new PropertyPipe<TKey, TValue>(SourcePipe, KeyValueFilter);
        }

        #endregion

    }

    #endregion

    #region PropertyPipe<TKey, TValue>

    /// <summary>
    /// Emits the property values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public class PropertyPipe<TKey, TValue> : AbstractPipe<IReadOnlyProperties<TKey, TValue>, TValue>
        
        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

    {

        #region Data

        private readonly TKey[] Keys;

        private readonly KeyValueFilter<TKey, TValue> KeyValueFilter;

        private IEnumerator<TKey> KeysInterator;

        private IEnumerator<KeyValuePair<TKey, TValue>> KeyValueInterator;

        #endregion

        #region Constructor(s)

        #region PropertyPipe(SourceElement, Keys)

        /// <summary>
        /// Emits the property values of the given property keys (OR-logic).
        /// </summary>
        /// <param name="SourceElement">A single value as element source.</param>
        /// <param name="Keys">The property keys.</param>
        /// <returns>The property values of the given property keys.</returns>
        public PropertyPipe(IReadOnlyProperties<TKey, TValue>  SourceElement,
                            params TKey[]                      Keys)

            : base(SourceElement)

        {
            this.Keys = Keys;
        }

        #endregion

        #region PropertyPipe(SourcePipe, Keys)

        /// <summary>
        /// Emits the property values of the given property keys (OR-logic).
        /// </summary>
        /// <param name="IEnumerable">An optional IEnumerable&lt;IIdentifier&lt;TId&gt;&gt; as element source.</param>
        /// <param name="IEnumerator">An optional IEnumerator&lt;IIdentifier&lt;TId&gt;&gt; as element source.</param>
        /// <param name="Keys">The property keys.</param>
        /// <returns>The property values of the given property keys.</returns>
        public PropertyPipe(IEndPipe<IReadOnlyProperties<TKey, TValue>>  SourcePipe,
                            params TKey[]                                Keys)

            : base(SourcePipe)

        {
            this.Keys = Keys;
        }

        #endregion


        #region PropertyPipe(SourceElement, KeyValueFilter)

        /// <summary>
        /// Emits the property values filtered by the given keyvalue filter.
        /// </summary>
        /// <param name="KeyValueFilter">An optional delegate for keyvalue filtering.</param>
        /// <param name="IEnumerable">An optional IEnumerable&lt;...&gt; as element source.</param>
        /// <param name="IEnumerator">An optional IEnumerator&lt;...&gt; as element source.</param>
        public PropertyPipe(IReadOnlyProperties<TKey, TValue>  SourceElement,
                            KeyValueFilter<TKey, TValue>       KeyValueFilter)

            : base(SourceElement)

        {
            this.KeyValueFilter = (KeyValueFilter != null) ? KeyValueFilter : (k, v) => true;
        }

        #endregion

        #region PropertyPipe(SourcePipe, KeyValueFilter)

        /// <summary>
        /// Emits the property values filtered by the given keyvalue filter.
        /// </summary>
        /// <param name="KeyValueFilter">An optional delegate for keyvalue filtering.</param>
        /// <param name="IEnumerable">An optional IEnumerable&lt;...&gt; as element source.</param>
        /// <param name="IEnumerator">An optional IEnumerator&lt;...&gt; as element source.</param>
        public PropertyPipe(IEndPipe<IReadOnlyProperties<TKey, TValue>>  SourcePipe,
                            KeyValueFilter<TKey, TValue>                 KeyValueFilter)

            : base(SourcePipe)

        {
            this.KeyValueFilter = (KeyValueFilter != null) ? KeyValueFilter : (k, v) => true;
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

            #region Process the key array

            if (Keys != null)
            {

                while (true)
                {

                    if (KeysInterator == null)
                    {

                        if (SourcePipe.MoveNext())
                            KeysInterator = Keys.ToList().GetEnumerator();

                        else
                            return false;

                    }

                    if (KeysInterator.MoveNext())
                    {

                        if (SourcePipe.Current.TryGetProperty(KeysInterator.Current, out _CurrentElement))
                            return true;

                    }

                    KeysInterator = null;

                }

            }

            #endregion

            #region Process the KeyValue filter

            else
            {

                while (true)
                {

                    if (KeyValueInterator == null)
                    {

                        if (SourcePipe.MoveNext())
                            KeyValueInterator = SourcePipe.Current.GetEnumerator();

                        else
                            return false;

                    }

                    while (KeyValueInterator.MoveNext())
                    {

                        if (KeyValueFilter(KeyValueInterator.Current.Key, KeyValueInterator.Current.Value))
                        {
                            _CurrentElement = KeyValueInterator.Current.Value;
                            return true;
                        }

                    }

                    KeyValueInterator = null;

                }

            }

            #endregion

        }

        #endregion


        #region (override) ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {

            if (Keys != null)
                return base.ToString() + "<" + Keys.Aggregate("", (a, b) => a.ToString() + " " + b.ToString()) + ">";

            return base.ToString();

        }

        #endregion

    }

    #endregion

}
