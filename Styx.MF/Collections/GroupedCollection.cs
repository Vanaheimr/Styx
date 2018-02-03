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
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;

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
    public class GroupedCollection<TKey, TValue, TGroup> : IGroupedCollection<TKey, TValue, TGroup>
    {

        #region Data

        private Int64 _NumberOfElements;

        private readonly ConcurrentDictionary<TKey, TValue>               Keys;
        private readonly ConcurrentDictionary<TGroup, LinkedList<TValue>> Groups;

        #endregion

        #region Constructor(s)

        #region GroupedCollection()

        /// <summary>
        /// Create a new grouped collection.
        /// </summary>
        public GroupedCollection()
        {
            this.Keys   = new ConcurrentDictionary<TKey, TValue>();
            this.Groups = new ConcurrentDictionary<TGroup, LinkedList<TValue>>();
        }

        #endregion

        #endregion


        #region TryAddValue(Key, Value, Group)

        /// <summary>
        /// Tries to add a KeyValueGroupTriple to the collection.
        /// </summary>
        /// <param name="Key">The key of the triple.</param>
        /// <param name="Value">The value of the triple.</param>
        /// <param name="Group">The group of the triple.</param>
        /// <returns>True if success; false otherwise.</returns>
        public Boolean TryAddValue(TKey Key, TValue Value, TGroup Group)
        {

            #region Initial checks

            if (Key == null)
                throw new ArgumentNullException("Key", "The parameter Key must not be null!");

            if (Group == null)
                throw new ArgumentNullException("Group", "The parameter Group must not be null!");

            #endregion

            if (Keys.TryAdd(Key, Value))
            {

                LinkedList<TValue> _Group;

                lock (Groups)
                {

                    if (Groups.TryGetValue(Group, out _Group))
                    {

                        _Group.AddLast(Value);
                        Interlocked.Increment(ref _NumberOfElements);

                        return true;

                    }
                    else
                    {

                        _Group = new LinkedList<TValue>();
                        _Group.AddLast(Value);

                        if (Groups.TryAdd(Group, _Group))
                        {
                            Interlocked.Increment(ref _NumberOfElements);
                        }

                        return true;

                    }

                }

            }

            return false;

        }

        #endregion


        #region ContainsKey(Key)

        /// <summary>
        /// Determines whether the collection contains the specified key.
        /// </summary>
        /// <param name="Key">A key.</param>
        public Boolean ContainsKey(TKey Key)
        {
            return this.Keys.ContainsKey(Key);
        }

        #endregion

        #region ContainsGroup(Group)

        /// <summary>
        /// Determines whether the collection contains the specified group.
        /// </summary>
        /// <param name="Group">A group.</param>
        public Boolean ContainsGroup(TGroup Group)
        {
            return this.Groups.ContainsKey(Group);
        }

        #endregion


        #region Count()

        /// <summary>
        /// The total number of values in the grouped collection.
        /// </summary>
        public UInt64 Count()
        {
            return (UInt64) _NumberOfElements;
        }

        #endregion

        #region Count(Group)

        /// <summary>
        /// The number of values in the given group collection.
        /// </summary>
        public UInt64 Count(TGroup Group)
        {

            LinkedList<TValue> _Group = null;

            if (Groups.TryGetValue(Group, out _Group))
                return (UInt64) _Group.Count;

            return 0;

        }

        #endregion


        #region TryGetByKey(Key, out Value)

        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <param name="Value">The value.</param>
        /// <returns>True, if the key was found in the grouped collection; False otherwise.</returns>
        public Boolean TryGetByKey(TKey Key, out TValue Value)
        {
            return Keys.TryGetValue(Key, out Value);
        }

        #endregion

        #region TryGetByGroup(Group, out Values)

        /// <summary>
        /// Attempts to get the values associated with the specified group.
        /// </summary>
        /// <param name="Group">The group.</param>
        /// <param name="Values">An enumeration of values.</param>
        /// <returns>True, if the group was found in the grouped collection; False otherwise.</returns>
        public Boolean TryGetByGroup(TGroup Group, out IEnumerable<TValue> Values)
        {

            LinkedList<TValue> _Group = null;

            if (Groups.TryGetValue(Group, out _Group))
            {
                Values = _Group;
                return true;
            }

            Values = new TValue[0];
            return false;

        }

        #endregion


        #region TryRemoveValue(Key, Value, Group)

        /// <summary>
        /// Attempts to remove the given value with the specified
        /// key and group from the grouped collection.
        /// </summary>
        /// <param name="Key">The key of the value.</param>
        /// <param name="Value">The value to remove.</param>
        /// <param name="Group">The group of the value.</param>
        /// <returns>True if success; false otherwise.</returns>
        public Boolean TryRemoveValue(TKey Key, TValue Value, TGroup Group)
        {

            TValue _Value;

            // Remove value from Key-lookup...
            if (this.Keys.TryRemove(Key, out _Value))
            {

                // Remove value from Group-lookup...
                LinkedList<TValue> _Group = null;

                if (Groups.TryGetValue(Group, out _Group))
                {
                    if (_Group.Remove(Value))
                    {

                        // Remove entire group if no value is left!
                        if (_Group.Count == 0)
                        {
                            LinkedList<TValue> _RemovedGroup;
                            return Groups.TryRemove(Group, out _RemovedGroup);
                        }

                        return true;

                    }
                }
            
            }

            return false;

        }

        #endregion

        #region Clear()

        /// <summary>
        /// Removes all keys, values and groups
        /// from the grouped collection.
        /// </summary>
        public void Clear()
        {
            this.Keys.Clear();
            this.Groups.Clear();
        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Return an enumerator for the grouped collection.
        /// </summary>
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var Group in Groups.Values)
                foreach (var Value in Group)
                    yield return Value;
        }

        /// <summary>
        /// Return an enumerator for the grouped collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var Group in Groups.Values)
                foreach (var Value in Group)
                    yield return Value;
        }

        #endregion

    }

}

