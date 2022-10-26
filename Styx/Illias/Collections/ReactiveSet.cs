/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A reactive set.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class ReactiveSet<T> : IEnumerable<T>,
                                  IEquatable<ReactiveSet<T>>
    {

        #region Data

        private readonly HashSet<T> internalSet;

        #endregion

        #region Properties

        /// <summary>
        /// The number of items stored within this reactive set.
        /// </summary>
        public UInt64 Count

            => (UInt64) internalSet.Count;

        #endregion

        #region Typed Delegates

        /// <summary>
        /// A delegate called whenever an item was added to the reactive set.
        /// </summary>
        /// <param name="Timestamp">The timestamp when this change was detected.</param>
        public delegate void OnItemAddedDelegate  (DateTime        Timestamp,
                                                   ReactiveSet<T>  ReactiveSet,
                                                   T               Item);

        /// <summary>
        /// A delegate called whenever an item was removed from the reactive set.
        /// </summary>
        /// <param name="Timestamp">The timestamp when this change was detected.</param>
        public delegate void OnItemRemovedDelegate(DateTime        Timestamp,
                                                   ReactiveSet<T>  ReactiveSet,
                                                   T               Item);

        /// <summary>
        /// A delegate called whenever the reactive set changed.
        /// </summary>
        /// <param name="Timestamp">The timestamp when this change was detected.</param>
        public delegate void OnSetChangedDelegate (DateTime         Timestamp,
                                                   ReactiveSet<T>   ReactiveSet,
                                                   IEnumerable<T>   NewItems,
                                                   IEnumerable<T>?  OldItems = null);

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever an item was added to the reactive set.
        /// </summary>
        public event OnItemAddedDelegate?    OnItemAdded;

        /// <summary>
        /// An event fired whenever an item was removed from the reactive set.
        /// </summary>
        public event OnItemRemovedDelegate?  OnItemRemoved;

        /// <summary>
        /// An event fired whenever the reactive set changed.
        /// </summary>
        public event OnSetChangedDelegate?   OnSetChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reactive set storing on the given items.
        /// </summary>
        /// <param name="Items">An optional array of items to store.</param>
        public ReactiveSet(params T[] Items)
        {

            this.internalSet = Items is null
                                   ? new HashSet<T>()
                                   : new HashSet<T>(Items);

        }

        /// <summary>
        /// Create a new reactive set storing on the given items.
        /// </summary>
        /// <param name="Items">An optional enumeration of items to store.</param>
        public ReactiveSet(IEnumerable<T> Items)
        {

            this.internalSet = Items is null
                                   ? new HashSet<T>()
                                   : new HashSet<T>(Items);

        }

        #endregion


        #region From(Items = null)

        public static ReactiveSet<T> From(IEnumerable<T>? Items = null)

            => Items is null
                   ? new ()
                   : new (Items);

        #endregion


        #region AddAndReturn(Item)

        /// <summary>
        /// Add the given item to the reactive set and return it.
        /// </summary>
        /// <param name="Item">An item.</param>
        public T AddAndReturn(T Item)
        {

            if (Item is not null)
            {

                var added = false;

                lock (internalSet)
                {
                    if (!internalSet.Contains(Item))
                    {
                        internalSet.Add(Item);
                        added = true;
                    }
                }

                if (added)
                    OnItemAdded?.Invoke(Timestamp.Now,
                                        this,
                                        Item);

            }

            return Item;

        }

        #endregion

        #region Add    (Items...)

        /// <summary>
        /// Add the given array of items to the reactive set.
        /// </summary>
        /// <param name="Items">An array of items.</param>
        public ReactiveSet<T> Add(params T[] Items)

            => Add(Items as IEnumerable<T>);

        #endregion

        #region Add    (Items, OnlyOneEvent = false)

        /// <summary>
        /// Add the given enumeration of items to the reactive set.
        /// </summary>
        /// <param name="Items">An enumeration of items.</param>
        /// <param name="OnlyOneEvent">Send only a single change event.</param>
        public ReactiveSet<T> Add(IEnumerable<T>  Items,
                                  Boolean         OnlyOneEvent = false)
        {

            if (Items is not null && Items.Any())
            {

                var newHashSet = Items.ToHashSet();

                foreach (var item in Items)
                {
                    if (internalSet.Contains(item))
                        newHashSet.Remove(item);
                }

                if (newHashSet.Any())
                {

                    lock (internalSet)
                    {
                        foreach (var item in newHashSet)
                            internalSet.Add(item);
                    }

                    var timestamp = Timestamp.Now;

                    if (!OnlyOneEvent)
                        foreach (var item in Items)
                            OnItemAdded?.Invoke(timestamp, this, item);

                    OnSetChanged?.Invoke(timestamp, this, Items);

                }

            }

            return this;

        }

        #endregion

        #region Set    (Items, OnlyOneEvent = false)

        /// <summary>
        /// Remove the given enumeration of items from the reactive set.
        /// </summary>
        /// <param name="Items">An enumeration of items.</param>
        /// <param name="OnlyOneEvent">Send only a single change event.</param>
        public ReactiveSet<T> Set(IEnumerable<T>  Items,
                                  Boolean         OnlyOneEvent = false)
        {

            if (Items is not null && Items.Any())
            {

                var toAdd     = new List<T>();
                var toRemove  = new List<T>();

                lock (internalSet)
                {

                    toAdd.   AddRange(Items.      Except(internalSet));
                    toRemove.AddRange(internalSet.Except(Items));

                    foreach (var item in toRemove)
                        internalSet.Remove(item);

                    foreach (var item in toAdd)
                        internalSet.Add   (item);

                }

                var timestamp = Timestamp.Now;

                if (!OnlyOneEvent)
                {

                    foreach (var item in toRemove)
                        OnItemRemoved?.Invoke(timestamp, this, item);

                    foreach (var item in toAdd)
                        OnItemAdded?.  Invoke(timestamp, this, item);

                }

                if (toAdd.Any() || toRemove.Any())
                    OnSetChanged?.Invoke(timestamp,
                                         this,
                                         toAdd.Concat(toRemove));

            }

            return this;

        }

        #endregion

        #region Replace(Items, OnlyOneEvent = false)

        /// <summary>
        /// Add the given enumeration of items to the reactive set.
        /// </summary>
        /// <param name="Items">An enumeration of items.</param>
        /// <param name="OnlyOneEvent">Send only a single change event.</param>
        public ReactiveSet<T> Replace(IEnumerable<T>  Items,
                                      Boolean         OnlyOneEvent = false)
        {

            if (Items is not null && Items.Any())
            {

                var newHashSet = Items.ToHashSet();

                foreach (var item in Items)
                {
                    if (internalSet.Contains(item))
                        newHashSet.Remove(item);
                }

                if (newHashSet.Any())
                {

                    lock (internalSet)
                    {

                        internalSet.Clear();

                        foreach (var item in Items)
                            internalSet.Add(item);

                    }

                    var timestamp = Timestamp.Now;

                    if (!OnlyOneEvent)
                        foreach (var item in Items)
                            OnItemAdded?.Invoke(timestamp, this, item);

                    OnSetChanged?.Invoke(timestamp, this, Items);

                }

            }

            return this;

        }

        #endregion


        #region Contains(Item)

        /// <summary>
        /// Determines whether the reactive set contains the given item.
        /// </summary>
        /// <param name="Item">An item.</param>
        /// <returns>true if the reactive set contains the specified item; otherwise, false.</returns>
        public Boolean Contains(T Item)

            => internalSet.Contains(Item);

        #endregion

        #region Remove(Items...)

        /// <summary>
        /// Remove the given array of items from the reactive set.
        /// </summary>
        /// <param name="Items">An array of items.</param>
        public ReactiveSet<T> Remove(params T[] Items)

            => Remove(Items as IEnumerable<T>);

        #endregion

        #region Remove(Items, OnlyOneEvent = false)

        /// <summary>
        /// Remove the given enumeration of items from the reactive set.
        /// </summary>
        /// <param name="Items">An enumeration of items.</param>
        /// <param name="OnlyOneEvent">Send only a single change event.</param>
        public ReactiveSet<T> Remove(IEnumerable<T>  Items,
                                     Boolean         OnlyOneEvent = false)
        {

            if (Items is not null &&
                Items.Any())
            {

                lock(internalSet)
                {
                    foreach (var item in Items)
                        internalSet.Remove(item);
                }

                var timestamp = Timestamp.Now;

                if (!OnlyOneEvent)
                    foreach (var item in Items)
                        OnItemRemoved?.Invoke(timestamp, this, item);

                OnSetChanged?.Invoke(timestamp, this, Items);

            }

            return this;

        }

        #endregion

        #region Clear()

        /// <summary>
        /// Remove all items from the reactive set.
        /// </summary>
        public void Clear()
        {
            lock (internalSet)
            {
                internalSet.Clear();
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (ReactiveSet1, ReactiveSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactiveSet1">A ReactiveSet.</param>
        /// <param name="ReactiveSet2">Another ReactiveSet.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ReactiveSet<T> ReactiveSet1,
                                           ReactiveSet<T> ReactiveSet2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ReactiveSet1, ReactiveSet2))
                return true;

            // If one is null, but not both, return false.
            if (ReactiveSet1 is null || ReactiveSet2 is null)
                return false;

            return ReactiveSet1.Equals(ReactiveSet2);

        }

        #endregion

        #region Operator != (ReactiveSet1, ReactiveSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactiveSet1">A ReactiveSet.</param>
        /// <param name="ReactiveSet2">Another ReactiveSet.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ReactiveSet<T> ReactiveSet1,
                                           ReactiveSet<T> ReactiveSet2)

            => !(ReactiveSet1 == ReactiveSet2);

        #endregion

        #region Operator <  (ReactiveSet1, ReactiveSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactiveSet1">A ReactiveSet.</param>
        /// <param name="ReactiveSet2">Another ReactiveSet.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ReactiveSet<T> ReactiveSet1,
                                          ReactiveSet<T> ReactiveSet2)
        {

            if (ReactiveSet1 is null)
                throw new ArgumentNullException(nameof(ReactiveSet1), "The given ReactiveSet1 must not be null!");

            return ReactiveSet1.Count < ReactiveSet2.Count;

        }

        #endregion

        #region Operator <= (ReactiveSet1, ReactiveSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactiveSet1">A ReactiveSet.</param>
        /// <param name="ReactiveSet2">Another ReactiveSet.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ReactiveSet<T> ReactiveSet1,
                                           ReactiveSet<T> ReactiveSet2)

            => !(ReactiveSet1 > ReactiveSet2);

        #endregion

        #region Operator >  (ReactiveSet1, ReactiveSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactiveSet1">A ReactiveSet.</param>
        /// <param name="ReactiveSet2">Another ReactiveSet.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ReactiveSet<T> ReactiveSet1,
                                          ReactiveSet<T> ReactiveSet2)
        {

            if (ReactiveSet1 is null)
                throw new ArgumentNullException(nameof(ReactiveSet1), "The given ReactiveSet1 must not be null!");

            return ReactiveSet1.Count > ReactiveSet2.Count;

        }

        #endregion

        #region Operator >= (ReactiveSet1, ReactiveSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactiveSet1">A ReactiveSet.</param>
        /// <param name="ReactiveSet2">Another ReactiveSet.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ReactiveSet<T> ReactiveSet1,
                                           ReactiveSet<T> ReactiveSet2)

            => !(ReactiveSet1 < ReactiveSet2);

        #endregion

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Enumerate the reactive list.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
            => internalSet.GetEnumerator();

        /// <summary>
        /// Enumerate the reactive list.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => internalSet.GetEnumerator();

        #endregion

        #region IEquatable<ReactiveSet<T>> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is ReactiveSet<T> reactiveSet &&
                   Equals(reactiveSet);

        #endregion

        #region Equals(ReactiveSet)

        /// <summary>
        /// Compares two reactive sets for equality.
        /// </summary>
        /// <param name="ReactiveSet">A reactive set to compare with.</param>
        public Boolean Equals(ReactiveSet<T>? ReactiveSet)

            => ReactiveSet is not null &&
               internalSet.Count == ReactiveSet.internalSet.Count &&
               internalSet.All(item => ReactiveSet.Contains(item));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => internalSet.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("'", internalSet.AggregateWith(", "), "'");

        #endregion

    }

}
