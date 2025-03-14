﻿/*
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

using System;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Collections
{

    // Delegates

    #region PropertiesInitializer

    /// <summary>
    /// A delegate for IProperties initializing.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    /// <param name="Properties">The properties object.</param>
    public delegate void IPropertiesInitializer<TKey, TValue>(IProperties<TKey, TValue> Properties)

        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable;

    #endregion


    // Interface

    #region IReadOnlyProperties<TKey, TValue>

    /// <summary>
    /// A generic interface maintaining a collection of key/value properties
    /// within the given datastructure.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public interface IReadOnlyProperties<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>

        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

    {

        #region Well-known properties

        //ToDo: Solve ambiguity!
        //#region Id

        ///// <summary>
        ///// The identification.
        ///// </summary>
        //TValue Id { get; }

        //#endregion

        #region IdKey

        /// <summary>
        /// The property key of the identification.
        /// </summary>
        TKey IdKey { get; }

        #endregion

        //ToDo: Solve ambiguity!
        //#region RevId

        ///// <summary>
        ///// The revision identification.
        ///// </summary>
        //TValue RevId { get; }

        //#endregion

        #region RevIdKey

        /// <summary>
        /// The property key of the revision identification.
        /// </summary>
        TKey RevIdKey { get; }

        #endregion

        #region LabelKey

        /// <summary>
        /// The property key of the label.
        /// </summary>
        TKey LabelKey { get; }

        #endregion

        #endregion

        #region Keys/Values

        /// <summary>
        /// An enumeration of all property keys.
        /// </summary>
        IEnumerable<TKey> Keys { get; }

        /// <summary>
        /// An enumeration of all property values.
        /// </summary>
        IEnumerable<TValue> Values { get; }

        #endregion

        #region Contains...

        /// <summary>
        /// Determines if the given key exists.
        /// </summary>
        /// <param name="Key">A key.</param>
        Boolean ContainsKey(TKey Key);

        /// <summary>
        /// Determines if the given value exists.
        /// </summary>
        /// <param name="Value">A value.</param>
        Boolean ContainsValue(TValue Value);

        /// <summary>
        /// Determines if the given key and value exists.
        /// </summary>
        /// <param name="Key">A key.</param>
        /// <param name="Value">A value.</param>
        Boolean Contains(TKey Key, TValue Value);

        /// <summary>
        /// Determines if the given KeyValuePair exists.
        /// </summary>
        /// <param name="KeyValuePair">A KeyValuePair.</param>
        Boolean Contains(KeyValuePair<TKey, TValue> KeyValuePair);

        #endregion

        #region Get...

        /// <summary>
        /// Return the value associated with the given key.
        /// </summary>
        /// <param name="Key">A key.</param>
        TValue this[TKey Key] { get; }

        /// <summary>
        /// Return the value associated with the given key.
        /// </summary>
        /// <param name="Key">A key.</param>
        /// <param name="Value">The associated value.</param>
        /// <returns>True if the returned value is valid. False otherwise.</returns>
        Boolean TryGetProperty(TKey Key, out TValue Value);

        /// <summary>
        /// Return the value associated with the given key.
        /// </summary>
        /// <param name="Key">A key.</param>
        /// <param name="Value">The associated value.</param>
        /// <typeparam name="T">Check if the value is of the given type.</typeparam>
        /// <returns>True if the returned value is valid. False otherwise.</returns>
        Boolean TryGetProperty<T>(TKey Key, out T Value);

        /// <summary>
        /// Return a filtered enumeration of all KeyValuePairs.
        /// </summary>
        /// <param name="KeyValueFilter">A delegate to filter properties based on their keys and values.</param>
        /// <returns>A enumeration of all key/value pairs matching the given KeyValueFilter.</returns>
        IEnumerable<KeyValuePair<TKey, TValue>> GetProperties(KeyValueFilter<TKey, TValue> KeyValueFilter = null);

        #endregion

    }

    #endregion

}
