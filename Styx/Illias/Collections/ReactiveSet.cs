/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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

#endregion

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

        private readonly HashSet<T> _Set;

        #endregion

        #region Properties

        #region Count

        /// <summary>
        /// The number of items stored within this reactive set.
        /// </summary>
        public UInt64 Count
        {
            get
            {
                return (UInt64) _Set.Count;
            }
        }

        #endregion

        #endregion

        #region Events

        #region OnItemAdded

        /// <summary>
        /// A delegate called whenever the aggregated dynamic status of all subordinated EVSEs changed.
        /// </summary>
        /// <param name="Timestamp">The timestamp when this change was detected.</param>
        public delegate void OnItemAddedDelegate(DateTime Timestamp, ReactiveSet<T> ReactiveSet, T Item);

        /// <summary>
        /// An event fired whenever the aggregated dynamic status of all subordinated EVSEs changed.
        /// </summary>
        public event OnItemAddedDelegate OnItemAdded;

        #endregion

        #region OnItemRemoved

        /// <summary>
        /// A delegate called whenever the aggregated dynamic status of all subordinated EVSEs changed.
        /// </summary>
        /// <param name="Timestamp">The timestamp when this change was detected.</param>
        public delegate void OnItemRemovedDelegate(DateTime Timestamp, ReactiveSet<T> ReactiveSet, T Item);

        /// <summary>
        /// An event fired whenever the aggregated dynamic status of all subordinated EVSEs changed.
        /// </summary>
        public event OnItemRemovedDelegate OnItemRemoved;

        #endregion

        #endregion

        #region Constructor(s)

        #region ReactiveSet(params Items)

        /// <summary>
        /// Create a new reactive set storing on the given items.
        /// </summary>
        /// <param name="Items">An optional array of items to store.</param>
        public ReactiveSet(params T[] Items)
        {

            if (Items == null)
                this._Set = new HashSet<T>();

            else
                this._Set = new HashSet<T>(Items);

        }

        #endregion

        #region ReactiveSet(Items)

        /// <summary>
        /// Create a new reactive set storing on the given items.
        /// </summary>
        /// <param name="Items">An optional enumeration of items to store.</param>
        public ReactiveSet(IEnumerable<T> Items)
        {

            this._Set = new HashSet<T>(Items);

        }

        #endregion

        #endregion


        #region Add(Items...)

        /// <summary>
        /// Add the given array of items to the reactive set.
        /// </summary>
        /// <param name="Items">An array of items.</param>
        public ReactiveSet<T> Add(params T[] Items)

            => Add(Items as IEnumerable<T>);

        #endregion

        #region Add(Items)

        /// <summary>
        /// Add the given enumeration of items to the reactive set.
        /// </summary>
        /// <param name="Items">An enumeration of items.</param>
        public ReactiveSet<T> Add(IEnumerable<T> Items)
        {

            if (Items != null && Items.Any())
                lock(_Set)
                {

                    var Timestamp = DateTime.Now;

                    Items.ForEach(Item => {

                        _Set.Add(Item);

                         OnItemAdded?.Invoke(Timestamp, this, Item);

                    });

                }

            return this;

        }

        #endregion

        #region AddAndReturn(Item)

        /// <summary>
        /// Add the given item to the reactive set and return it.
        /// </summary>
        /// <param name="Item">An item.</param>
        public T AddAndReturn(T Item)
        {

            if (Item != null)
                lock(_Set)
                {

                    var Timestamp = DateTime.Now;

                    _Set.Add(Item);

                    OnItemAdded?.Invoke(Timestamp, this, Item);

                }

            return Item;

        }

        #endregion


        #region Contains(Item)

        /// <summary>
        /// Determines whether the reactive set contains the given item.
        /// </summary>
        /// <param name="Item">An item.</param>
        /// <returns>true if the reactive set contains the specified item; otherwise, false.</returns>
        public Boolean Contains(T Item)

            => _Set.Contains(Item);

        #endregion

        #region Remove(Items...)

        /// <summary>
        /// Remove the given array of items from the reactive set.
        /// </summary>
        /// <param name="Items">An array of items.</param>
        public ReactiveSet<T> Remove(params T[] Items)

            => Remove(Items as IEnumerable<T>);

        #endregion

        #region Remove(Items)

        /// <summary>
        /// Remove the given enumeration of items from the reactive set.
        /// </summary>
        /// <param name="Items">An enumeration of items.</param>
        public ReactiveSet<T> Remove(IEnumerable<T> Items)
        {

            if (Items != null && Items.Any())
                lock(_Set)
                {

                    var Timestamp = DateTime.Now;

                    Items.ForEach(Item => {

                        _Set.Remove(Item);

                        OnItemRemoved?.Invoke(Timestamp, this, Item);

                    });

                }

            return this;

        }

        #endregion


        #region Set(Items)

        /// <summary>
        /// Remove the given enumeration of items from the reactive set.
        /// </summary>
        /// <param name="Items">An enumeration of items.</param>
        public ReactiveSet<T> Set(IEnumerable<T> Items)
        {

            if (Items?.Any() == true)
            {
                lock (_Set)
                {

                    var ToAdd      = Items.Except(_Set). ToArray();
                    var ToRemove   = _Set. Except(Items).ToArray();
                    var Timestamp  = DateTime.Now;

                    ToRemove.ForEach(Item => {
                        _Set.Remove(Item);
                        OnItemRemoved?.Invoke(Timestamp, this, Item);
                    });

                    ToAdd.ForEach(Item => {
                        _Set.Add(Item);
                        OnItemAdded?.  Invoke(Timestamp, this, Item);
                    });

                }
            }

            return this;

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
        public static Boolean operator == (ReactiveSet<T> ReactiveSet1, ReactiveSet<T> ReactiveSet2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ReactiveSet1, ReactiveSet2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ReactiveSet1 == null) || ((Object) ReactiveSet2 == null))
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
        public static Boolean operator != (ReactiveSet<T> ReactiveSet1, ReactiveSet<T> ReactiveSet2)
        {
            return !(ReactiveSet1 == ReactiveSet2);
        }

        #endregion

        #region Operator <  (ReactiveSet1, ReactiveSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactiveSet1">A ReactiveSet.</param>
        /// <param name="ReactiveSet2">Another ReactiveSet.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ReactiveSet<T> ReactiveSet1, ReactiveSet<T> ReactiveSet2)
        {

            if ((Object) ReactiveSet1 == null)
                throw new ArgumentNullException("The given ReactiveSet1 must not be null!");

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
        public static Boolean operator <= (ReactiveSet<T> ReactiveSet1, ReactiveSet<T> ReactiveSet2)
        {
            return !(ReactiveSet1 > ReactiveSet2);
        }

        #endregion

        #region Operator >  (ReactiveSet1, ReactiveSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactiveSet1">A ReactiveSet.</param>
        /// <param name="ReactiveSet2">Another ReactiveSet.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ReactiveSet<T> ReactiveSet1, ReactiveSet<T> ReactiveSet2)
        {

            if ((Object) ReactiveSet1 == null)
                throw new ArgumentNullException("The given ReactiveSet1 must not be null!");

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
        public static Boolean operator >= (ReactiveSet<T> ReactiveSet1, ReactiveSet<T> ReactiveSet2)
        {
            return !(ReactiveSet1 < ReactiveSet2);
        }

        #endregion

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Enumerate the reactive list.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _Set.GetEnumerator();
        }

        /// <summary>
        /// Enumerate the reactive list.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Set.GetEnumerator();
        }

        #endregion

        #region IEquatable<ReactiveSet<T>> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is a reactive set.
            var ReactiveSet = Object as ReactiveSet<T>;
            if ((Object) ReactiveSet == null)
                return false;

            return this.Equals(ReactiveSet);

        }

        #endregion

        #region Equals(ReactiveSet)

        /// <summary>
        /// Compares two reactive sets for equality.
        /// </summary>
        /// <param name="ReactiveSet">A reactive set to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ReactiveSet<T> ReactiveSet)
        {

            if ((Object) ReactiveSet == null)
                return false;

            if (this.Count != ReactiveSet.Count)
                return false;

            return _Set.All(item => ReactiveSet.Contains(item));

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return _Set.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return "'" + _Set.AggregateWith(", ") + "'";
        }

        #endregion

    }

}
