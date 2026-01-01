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
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias.Votes;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Collections
{

    #region IPropertiesExtensions

    /// <summary>
    /// Extensions to the IProperties interface.
    /// </summary>
    public static partial class IPropertiesExtensions
    {

        #region SetAdd<TKey>(this IProperties, Key, FirstValue, params Values)

        public static IProperties<TKey, Object> SetAdd<TKey>(this IProperties<TKey, Object> IProperties, TKey Key, Object FirstValue, params Object[] Values)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            #region Initial checks

            if (IProperties is null)
                throw new ArgumentNullException();

            if (Key is null)
                throw new ArgumentNullException();

            if (FirstValue is null)
                throw new ArgumentNullException();

            #endregion

            HashedSet<Object> _Set = null;
            Object _Value = null;

            if (IProperties.TryGetProperty(Key, out _Value))
            {

                _Set = _Value as HashedSet<Object>;

                if (_Set is null)
                    throw new Exception("The value is not a set!");

                else
                {

                    _Set.Add(FirstValue);

                    if (Values is not null && Values.Any())
                        Values.ForEach(value => _Set.Add(value));

                    return IProperties;

                }

            }

            _Set = new HashedSet<Object>() { FirstValue };

            if (Values is not null && Values.Any())
                Values.ForEach(value => _Set.Add(value));

            IProperties.Set(Key, _Set);

            return IProperties;

        }

        #endregion


        // SetProperty(...)

        #region SetProperty(this IProperties, KeyValuePair)

        /// <summary>
        /// Assign a KeyValuePair to the given IProperties object.
        /// If a value already exists for this key, then the previous key/value is overwritten.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValuePair">A KeyValuePair of type string and object</param>
        public static IProperties<TKey, TValue> SetProperty<TKey, TValue>(this IProperties<TKey, TValue> IProperties, KeyValuePair<TKey, TValue> KeyValuePair)
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            #region Initial checks

            if (IProperties is null)
                throw new ArgumentNullException("The given IProperties must not be null!");

            #endregion

            return IProperties.Set(KeyValuePair.Key, KeyValuePair.Value);

        }

        #endregion

        #region SetProperties(this IProperties, KeyValuePairs)

        /// <summary>
        /// Assign the given enumeration of KeyValuePairs to the IProperties object.
        /// If a value already exists for a key, then the previous key/value is overwritten.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValuePairs">A enumeration of KeyValuePairs of type string and object</param>
        public static IProperties<TKey, TValue> SetProperties<TKey, TValue>(this IProperties<TKey, TValue> IProperties, IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs)
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            #region Initial checks

            if (IProperties is null)
                throw new ArgumentNullException("The given IProperties must not be null!");

            if (KeyValuePairs is null)
                throw new ArgumentNullException("The given KeyValuePair enumeration must not be null!");

            #endregion

            foreach (var _KeyValuePair in KeyValuePairs)
                IProperties.Set(_KeyValuePair.Key, _KeyValuePair.Value);

            return IProperties;

        }

        #endregion

        #region SetProperties(this IProperties, IDictionary)

        /// <summary>
        /// Assign the given IDictionary to the IProperties object.
        /// If a value already exists for a key, then the previous key/value is overwritten.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="IDictionary">A IDictionary of type TKey and TValue</param>
        public static IProperties<TKey, TValue> SetProperties<TKey, TValue>(this IProperties<TKey, TValue> IProperties, IDictionary<TKey, TValue> IDictionary)
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            #region Initial checks

            if (IProperties is null)
                throw new ArgumentNullException("The given IProperties must not be null!");

            if (IDictionary is null)
                throw new ArgumentNullException("The given dictionary must not be null!");

            #endregion

            foreach (var _KeyValuePair in IDictionary)
                IProperties.Set(_KeyValuePair.Key, _KeyValuePair.Value);

            return IProperties;

        }

        #endregion


        #region ListAdd<TKey>(this IProperties, Key, FirstValue, params Values)

        public static IProperties<TKey, Object> ListAdd<TKey>(this IProperties<TKey, Object> IProperties, TKey Key, Object FirstValue, params Object[] Values)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

        {

            #region Initial checks

            if (IProperties is null)
                throw new ArgumentNullException();

            if (Key is null)
                throw new ArgumentNullException();

            if (FirstValue is null)
                throw new ArgumentNullException();

            #endregion

            List<Object> _List = null;
            Object _Value = null;

            if (IProperties.TryGetProperty(Key, out _Value))
            {
                
                _List = _Value as List<Object>;

                if (_List is null)
                    throw new Exception("The value is not a list!");

                else
                {

                    _List.Add(FirstValue);

                    if (Values is not null && Values.Any())
                        _List.AddRange(Values);

                    return IProperties;

                }

            }

            _List = new List<Object>() { FirstValue };

            if (Values is not null && Values.Any())
                _List.AddRange(Values);

            IProperties.Set(Key, _List);

            return IProperties;

        }

        #endregion


        #region ZSetAdd<TKey>(this IProperties, Key, FirstValue, params Values)

        public static IProperties<TKey, Object> ZSetAdd<TKey>(this IProperties<TKey, Object> IProperties, TKey Key, Object FirstValue, params Object[] Values)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

        {

            #region Initial checks

            if (IProperties is null)
                throw new ArgumentNullException();

            if (Key is null)
                throw new ArgumentNullException();

            if (FirstValue is null)
                throw new ArgumentNullException();

            #endregion

            Dictionary<Object, UInt64> _ZSet          = null;
            Object                     _CountedValue  = null;
            UInt64                     _Counter;

            if (IProperties.TryGetProperty(Key, out _CountedValue))
            {

                _ZSet = _CountedValue as Dictionary<Object, UInt64>;

                if (_ZSet is null)
                    throw new Exception("The value is not a counted set!");

                else
                {

                    if (_ZSet.TryGetValue(FirstValue, out _Counter))
                        _ZSet[FirstValue] = _Counter + 1;
                    else
                        _ZSet.Add(FirstValue, 1);

                    if (Values is not null && Values.Any())
                        Values.ForEach(value => {
                            if (_ZSet.TryGetValue(value, out _Counter))
                                _ZSet[value] = _Counter + 1;
                            else
                                _ZSet.Add(value, 1);
                        });

                    return IProperties;

                }

            }

            _ZSet = new Dictionary<Object, UInt64>();
            _ZSet.Add(FirstValue, 1);

            if (Values is not null && Values.Any())
                Values.ForEach(value =>
                {
                    if (_ZSet.TryGetValue(value, out _Counter))
                        _ZSet[value] = _Counter + 1;
                    else
                        _ZSet.Add(value, 1);
                });

            IProperties.Set(Key, _ZSet);

            return IProperties;

        }

        #endregion

        #region Increment<TKey>(this IProperties, Key)

        public static IProperties<TKey, Object> Increment<TKey>(this IProperties<TKey, Object> IProperties, TKey Key)

            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

        {

            #region Initial checks

            if (IProperties is null)
                throw new ArgumentNullException();

            if (Key is null)
                throw new ArgumentNullException();

            #endregion

            UInt64 _Counter;

            if (IProperties.TryGetProperty(Key, out _Counter))
                IProperties.Set(Key, _Counter + 1UL);

            else
                IProperties.Set(Key, 1UL);

            return IProperties;

        }

        #endregion


        #region Remove(KeyValuePair)

        /// <summary>
        /// Remove the given KeyValuePair.
        /// </summary>
        /// <param name="KeyValuePair">A KeyValuePair.</param>
        /// <returns>The value associated with that key prior to the removal.</returns>
        public static TValue Remove<TKey, TValue>(this IProperties<TKey, TValue> IProperties, KeyValuePair<TKey, TValue> KeyValuePair)
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {
            return IProperties.Remove(KeyValuePair.Key, KeyValuePair.Value);
        }

        #endregion

    }

    #endregion

    // Delegates

    #region PropertyAddingEventHandler<TKey, TValue>

    /// <summary>
    /// An event handler called whenever a property value will be added.
    /// </summary>
    /// <param name="Sender">The sender of this event.</param>
    /// <param name="Key">The key of the property to be added.</param>
    /// <param name="Value">The value of the property to be added.</param>
    /// <param name="Vote">A veto vote is a simple way to ask multiple event subscribers if the edge should be added or not.</param>
    public delegate void PropertyAddingEventHandler<TKey, TValue>(IReadOnlyProperties<TKey, TValue>  Sender,
                                                                  TKey                               Key,
                                                                  TValue                             Value,
                                                                  IVote<Boolean>                     Vote)

        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable;

    #endregion

    #region PropertyAddedEventHandler<TKey, TValue>

    /// <summary>
    /// An event handler called whenever a property value was added.
    /// </summary>
    /// <param name="Sender">The sender of this event.</param>
    /// <param name="Key">The key of the added property.</param>
    /// <param name="Value">The value of the added property.</param>
    public delegate void PropertyAddedEventHandler<TKey, TValue>(IReadOnlyProperties<TKey, TValue> Sender,
                                                                 TKey                              Key,
                                                                 TValue                            Value)

        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable;

    #endregion

    #region PropertyChangingEventHandler<TKey, TValue>

    /// <summary>
    /// An event handler called whenever a property value will be changed.
    /// </summary>
    /// <param name="Sender">The sender of this event.</param>
    /// <param name="Key">The key of the property to be changed.</param>
    /// <param name="OldValue">The old value of the property to be changed.</param>
    /// <param name="NewValue">The new value of the property to be changed.</param>
    /// <param name="Vote">A veto vote is a simple way to ask multiple event subscribers if the edge should be added or not.</param>
    public delegate void PropertyChangingEventHandler<TKey, TValue>(IReadOnlyProperties<TKey, TValue> Sender,
                                                                    TKey                              Key,
                                                                    TValue                            OldValue,
                                                                    TValue                            NewValue,
                                                                    IVote<Boolean>                    Vote)

        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable;

    #endregion

    #region PropertyChangedEventHandler<TKey, TValue>

    /// <summary>
    /// An event handler called whenever a property value was changed.
    /// </summary>
    /// <param name="Sender">The sender of this event.</param>
    /// <param name="Key">The key of the changed property.</param>
    /// <param name="OldValue">The old value of the changed property.</param>
    /// <param name="NewValue">The new value of the changed property.</param>
    public delegate void PropertyChangedEventHandler<TKey, TValue>(IReadOnlyProperties<TKey, TValue> Sender,
                                                                   TKey                              Key,
                                                                   TValue                            OldValue,
                                                                   TValue                            NewValue)

        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable;

    #endregion

    #region PropertyRemovingEventHandler<TKey, TValue>

    /// <summary>
    /// An event handler called whenever a property will be removed.
    /// </summary>
    /// <param name="Sender">The sender of this event.</param>
    /// <param name="Key">The key of the property to be removed.</param>
    /// <param name="Value">The value of the property to be removed.</param>
    /// <param name="Vote">A veto vote is a simple way to ask multiple event subscribers if the edge should be added or not.</param>
    public delegate void PropertyRemovingEventHandler<TKey, TValue>(IReadOnlyProperties<TKey, TValue> Sender,
                                                                    TKey                              Key,
                                                                    TValue                            Value,
                                                                    IVote<Boolean>                    Vote)

        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable;

    #endregion

    #region PropertyRemovedEventHandler<TKey, TValue>

    /// <summary>
    /// An event handler called whenever a property was removed.
    /// </summary>
    /// <param name="Sender">The sender of this event.</param>
    /// <param name="Key">The key of the removed property.</param>
    /// <param name="Value">The value of the removed property.</param>
    public delegate void PropertyRemovedEventHandler<TKey, TValue>(IReadOnlyProperties<TKey, TValue> Sender,
                                                                   TKey                              Key,
                                                                   TValue                            Value)

        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable;

    #endregion


    // Interface

    #region IProperties<TKey, TValue>

    /// <summary>
    /// A generic interface maintaining a collection of key/value properties
    /// within the given datastructure.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public interface IProperties<TKey, TValue> : IReadOnlyProperties<TKey, TValue>

        where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable

    {

        #region Events

        /// <summary>
        /// Called whenever a property value will be added.
        /// </summary>
        event PropertyAddingEventHandler<TKey, TValue> OnPropertyAdding;

        /// <summary>
        /// Called whenever a property value was added.
        /// </summary>
        event PropertyAddedEventHandler<TKey, TValue> OnPropertyAdded;


        /// <summary>
        /// Called whenever a property value will be changed.
        /// </summary>
        event PropertyChangingEventHandler<TKey, TValue> OnPropertyChanging;

        /// <summary>
        /// Called whenever a property value was changed.
        /// </summary>
        event PropertyChangedEventHandler<TKey, TValue> OnPropertyChanged;


        /// <summary>
        /// Called whenever a property value will be removed.
        /// </summary>
        event PropertyRemovingEventHandler<TKey, TValue> OnPropertyRemoving;

        /// <summary>
        /// Called whenever a property value was removed.
        /// </summary>
        event PropertyRemovedEventHandler<TKey, TValue> OnPropertyRemoved;

        #endregion

        #region Set(Key, Value)

        /// <summary>
        /// Add a KeyValuePair to the graph element.
        /// If a value already exists for the given key, then the previous value is overwritten.
        /// </summary>
        /// <param name="Key">A key.</param>
        /// <param name="Value">A value.</param>
        IProperties<TKey, TValue> Set(TKey Key, TValue Value);

        #endregion

        #region Remove...

        /// <summary>
        /// Removes all KeyValuePairs associated with the given key.
        /// </summary>
        /// <param name="Key">A key.</param>
        /// <returns>The value associated with that key prior to the removal.</returns>
        TValue Remove(TKey Key);

        /// <summary>
        /// Remove the given key and value pair.
        /// </summary>
        /// <param name="Key">A key.</param>
        /// <param name="Value">A value.</param>
        /// <returns>The value associated with that key prior to the removal.</returns>
        TValue Remove(TKey Key, TValue Value);

        /// <summary>
        /// Remove all KeyValuePairs specified by the given KeyValueFilter.
        /// </summary>
        /// <param name="KeyValueFilter">A delegate to remove properties based on their keys and values.</param>
        /// <returns>A enumeration of all key/value pairs removed by the given KeyValueFilter before their removal.</returns>
        IEnumerable<KeyValuePair<TKey, TValue>> Remove(KeyValueFilter<TKey, TValue> KeyValueFilter = null);

        #endregion

    }

    #endregion

}
