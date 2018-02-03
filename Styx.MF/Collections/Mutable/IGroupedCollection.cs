/*
 * Copyright (c) 2010-2012 Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Illias Commons <http://www.github.com/ahzf/Illias>
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

namespace de.ahzf.Illias.Commons
{

    /// <summary>
    /// A collection of values with the additional possibility
    /// to group these values.
    /// </summary>
    /// <typeparam name="TKey">The type of the collection keys.</typeparam>
    /// <typeparam name="TValue">The type of the collection values.</typeparam>
    /// <typeparam name="TGroup">The type of the collection groups.</typeparam>
    public interface IGroupedCollection<TKey, TValue, TGroup> : IEnumerable<TValue>
    {

        /// <summary>
        /// Tries to add a KeyValueGroupTriple to the collection.
        /// </summary>
        /// <param name="Key">The key of the triple.</param>
        /// <param name="Value">The value of the triple.</param>
        /// <param name="Group">The group of the triple.</param>
        /// <returns>True if success; false otherwise.</returns>
        Boolean TryAddValue(TKey Key, TValue Value, TGroup Group);


        /// <summary>
        /// Determines whether the collection contains the specified key.
        /// </summary>
        /// <param name="Key">A key.</param>
        Boolean ContainsKey(TKey Key);

        /// <summary>
        /// Determines whether the collection contains the specified group.
        /// </summary>
        /// <param name="Group">A group.</param>
        Boolean ContainsGroup(TGroup Group);


        /// <summary>
        /// The total number of values in the grouped collection.
        /// </summary>
        UInt64  Count();

        /// <summary>
        /// The number of values in the given group collection.
        /// </summary>
        UInt64  Count(TGroup Group);


        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Value">The value.</param>
        /// <returns>True, if the key was found in the grouped collection; False otherwise.</returns>
        Boolean TryGetByKey   (TKey Key, out TValue Value);

        /// <summary>
        /// Attempts to get the values associated with the specified group.
        /// </summary>
        /// <param name="Group">The group.</param>
        /// <param name="Values">An enumeration of values.</param>
        /// <returns>True, if the group was found in the grouped collection; False otherwise.</returns>
        Boolean TryGetByGroup (TGroup Group, out IEnumerable<TValue> Values);


        /// <summary>
        /// Attempts to remove the given value with the specified
        /// key and group from the grouped collection.
        /// </summary>
        /// <param name="Key">The key of the value.</param>
        /// <param name="Value">The value to remove.</param>
        /// <param name="Group">The group of the value.</param>
        /// <returns>True if success; false otherwise.</returns>
        Boolean TryRemoveValue(TKey Key, TValue Value, TGroup Group);

        /// <summary>
        /// Removes all keys, values and groups
        /// from the grouped collection.
        /// </summary>
        void Clear();

    }

}
