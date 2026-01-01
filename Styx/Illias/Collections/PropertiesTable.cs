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

using System;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Collections
{

    /// <summary>
    /// Extension methods for PropertiesTables.
    /// </summary>
    public static class PropertiesTables
    {

        #region PTable<TKey, TValue>(this IReadOnlyProperties<...>, Keys)

        /// <summary>
        /// Emits the property values of the given property keys (OR-logic).
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="Properties">An object implementing IReadOnlyProperties&lt;TKey, TValue&gt;.</param>
        /// <param name="Keys">An array of property keys.</param>
        /// <returns>The property values of the given property keys.</returns>
        public static IDictionary<TKey, IEnumerable<TValue>> PTable<TKey, TValue>(this IReadOnlyProperties<TKey, TValue> Properties,
                                                                                  params TKey[] Keys)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

        {

            var _Lookup = new Dictionary<TKey, IEnumerable<TValue>>();

            foreach (var Key in Keys)
                _Lookup.Add(Key, new List<TValue>() { Properties[Key] });

            return _Lookup;

        }

        #endregion

        #region PTable<TKey, TValue>(this IEnumerable<IReadOnlyProperties<...>>, Keys)

        /// <summary>
        /// Emits the property values of the given property keys (OR-logic).
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="IEnumerable">An enumeration of IReadOnlyProperties&lt;TKey, TValue&gt;.</param>
        /// <param name="Keys">An array of property keys.</param>
        /// <returns>The property values of the given property keys.</returns>
        public static IDictionary<TKey, IEnumerable<TValue>> PTable<TKey, TValue>(this IEnumerable<IReadOnlyProperties<TKey, TValue>> IEnumerable,
                                                                                  params TKey[] Keys)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

        {

            var _Lookup = new Dictionary<TKey, List<TValue>>();

            foreach (var Key in Keys)
                _Lookup.Add(Key, new List<TValue>());

            foreach (var Properties in IEnumerable)
                foreach (var Key in Keys)
                    _Lookup[Key].Add(Properties[Key]);

            
            var _Lookup2 = new Dictionary<TKey, IEnumerable<TValue>>();

            foreach (var KeyValuePair in _Lookup)
                _Lookup2.Add(KeyValuePair.Key, KeyValuePair.Value);

            return _Lookup2;

        }

        #endregion

    }

}
