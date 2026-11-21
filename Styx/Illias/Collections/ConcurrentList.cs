/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A simple concurrent list, copied on write: it is read without a lock and without a copy -
    /// every reader sees the elements as they were at one moment - and every change replaces
    /// all of its elements at once. Best for small lists that are read more often than changed.
    /// </summary>
    /// <typeparam name="T">The type of the elements within the list.</typeparam>
    public class ConcurrentList<T> : IEnumerable<T>
    {

        #region Data

        /// <summary>
        /// The elements: never changed, only replaced - under the lock.
        /// </summary>
        private          T[]   elements;
        private readonly Lock  syncLock   = new();

        #endregion

        #region Properties

        /// <summary>
        /// The number of elements.
        /// </summary>
        public Int32 Count
            => Volatile.Read(ref elements).Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new concurrent list.
        /// </summary>
        public ConcurrentList()
        {
            elements = [];
        }

        /// <summary>
        /// Create a new concurrent list with the given initial elements.
        /// </summary>
        /// <param name="Elements">The initial elements of the concurrent list.</param>
        public ConcurrentList(IEnumerable<T> Elements)
        {
            elements = [.. Elements];
        }

        /// <summary>
        /// Create a new concurrent list with the given optional initial elements.
        /// </summary>
        /// <param name="Elements">The optional initial elements of the concurrent list.</param>
        public ConcurrentList(params T[] Elements)
        {
            elements = [.. Elements];
        }

        #endregion


        #region (private) Replace(Change)

        /// <summary>
        /// Replace the elements by what the given change makes of them, under the lock - so that
        /// no change is lost to another one made at the same time.
        /// </summary>
        /// <param name="Change">What becomes of the elements; null when nothing changes.</param>
        private void Replace(Func<T[], T[]?> Change)
        {
            lock (syncLock)
            {

                var changed = Change(elements);

                if (changed is not null)
                    Volatile.Write(ref elements, changed);

            }
        }

        #endregion


        #region TryAdd            (Element)

        public Boolean TryAdd(T Element)
        {

            Replace(current => [.. current, Element]);

            return true;

        }

        #endregion

        #region TryAddRange       (params Elements)

        public Boolean TryAddRange(params T[] Elements)
        {

            if (Elements.Length > 0)
                Replace(current => [.. current, .. Elements]);

            return true;

        }

        #endregion

        #region TryAddRange       (Elements)

        public Boolean TryAddRange(IEnumerable<T> Elements)
        {

            // Read outside of the lock: what reads them might take its time, or read this list.
            T[] added = [.. Elements];

            return TryAddRange(added);

        }

        #endregion

        #region TrySet            (Element)

        public Boolean TrySet(T Element)
        {

            Replace(current => [ Element ]);

            return true;

        }

        #endregion

        #region TrySet            (params Elements)

        public Boolean TrySet(params T[] Elements)
        {

            T[] set = [.. Elements];

            Replace(current => set);

            return true;

        }

        #endregion

        #region TrySet            (Elements)

        public Boolean TrySet(IEnumerable<T> Elements)
        {

            T[] set = [.. Elements];

            Replace(current => set);

            return true;

        }

        #endregion

        #region TryRemove         (Element)

        public Boolean TryRemove(T Element)
        {

            var removed = false;

            Replace(current => {

                var index = Array.IndexOf(current, Element);

                if (index < 0)
                    return null;

                removed = true;

                return [.. current.AsSpan(0, index), .. current.AsSpan(index + 1)];

            });

            return removed;

        }

        #endregion

        #region TryRemoveMultiple (params Elements)

        /// <summary>
        /// Remove the given elements: for each one, its first occurrence, as List.Remove does.
        /// </summary>
        public void TryRemoveMultiple(params T[] Elements)
        {

            if (Elements.Length == 0)
                return;

            Replace(current => {

                var remaining  = new List<T>(current);
                var removed    = false;

                foreach (var element in Elements)
                    removed |= remaining.Remove(element);

                return removed
                           ? [.. remaining]
                           : null;

            });

        }

        #endregion

        #region TryRemoveMultiple (Elements)

        /// <summary>
        /// Remove the given elements: for each one, its first occurrence, as List.Remove does.
        /// </summary>
        public void TryRemoveMultiple(IEnumerable<T> Elements)
        {

            // Read outside of the lock: what reads them might take its time, or read this list.
            T[] removed = [.. Elements];

            TryRemoveMultiple(removed);

        }

        #endregion

        #region Contains          (Element)

        public Boolean Contains(T Element)

            => Array.IndexOf(Volatile.Read(ref elements), Element) >= 0;

        #endregion

        #region this[Index]

        public T this[Int32 Index]
        {
            get
            {

                var current = Volatile.Read(ref elements);

                if (Index < 0 || Index >= current.Length)
                    throw new ArgumentOutOfRangeException(nameof(Index));

                return current[Index];

            }
        }

        #endregion

        #region Clear()

        public void Clear()
        {
            Replace(current => current.Length > 0 ? [] : null);
        }

        #endregion

        #region ToArray()

        /// <summary>
        /// The elements as an array of their own.
        /// </summary>
        public T[] ToArray()

            => [.. Volatile.Read(ref elements)];

        #endregion


        // Called instead of the LINQ methods of the same name: they find no IList<T> here,
        // and would copy all elements by GetEnumerator() and walk through them.

        #region Any()

        /// <summary>
        /// Whether there is any element.
        /// </summary>
        public Boolean Any()

            => Volatile.Read(ref elements).Length > 0;

        #endregion

        #region First()

        /// <summary>
        /// The first element.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is no element.</exception>
        public T First()
        {

            var current = Volatile.Read(ref elements);

            return current.Length > 0
                       ? current[0]
                       : throw new InvalidOperationException("Sequence contains no elements");

        }

        #endregion

        #region FirstOrDefault()

        /// <summary>
        /// The first element, or the default when there is none.
        /// </summary>
        public T? FirstOrDefault()
        {

            var current = Volatile.Read(ref elements);

            return current.Length > 0
                       ? current[0]
                       : default;

        }

        #endregion

        #region Last()

        /// <summary>
        /// The last element.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is no element.</exception>
        public T Last()
        {

            var current = Volatile.Read(ref elements);

            return current.Length > 0
                       ? current[^1]
                       : throw new InvalidOperationException("Sequence contains no elements");

        }

        #endregion

        #region LastOrDefault()

        /// <summary>
        /// The last element, or the default when there is none.
        /// </summary>
        public T? LastOrDefault()
        {

            var current = Volatile.Read(ref elements);

            return current.Length > 0
                       ? current[^1]
                       : default;

        }

        #endregion

        #region ElementAt(Index)

        /// <summary>
        /// The element at the given index.
        /// </summary>
        /// <param name="Index">An index.</param>
        /// <exception cref="ArgumentOutOfRangeException">There is no element at the index.</exception>
        public T ElementAt(Int32 Index)

            => this[Index];

        #endregion

        #region ElementAtOrDefault(Index)

        /// <summary>
        /// The element at the given index, or the default when there is none.
        /// </summary>
        /// <param name="Index">An index.</param>
        public T? ElementAtOrDefault(Int32 Index)
        {

            var current = Volatile.Read(ref elements);

            return Index >= 0 && Index < current.Length
                       ? current[Index]
                       : default;

        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Walk through the elements as they are now: what changes meanwhile is not seen.
        /// </summary>
        public IEnumerator<T> GetEnumerator()

            => ((IEnumerable<T>) Volatile.Read(ref elements)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()

            => GetEnumerator();

        #endregion

    }

}
