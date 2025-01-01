/*
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

using System.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A simple concurrent list.
    /// </summary>
    /// <typeparam name="T">The type of the elements within the list.</typeparam>
    public class ConcurrentList<T> : IEnumerable<T>
    {

        #region Data

        private readonly List<T>  internalList;
        private readonly Object   syncLock   = new();

        #endregion

        #region Properties

        public Int32 Capacity
        {
            get
            {
                lock (syncLock)
                {
                    return internalList.Capacity;
                }
            }
        }

        public Int32 Count
        {
            get
            {
                lock (syncLock)
                {
                    return internalList.Count;
                }
            }
        }

        #endregion

        #region Constructor(s)

        public ConcurrentList()
        {
            internalList = [];
        }

        public ConcurrentList(Int32 Capacity)
        {
            internalList = new (Capacity);
        }

        public ConcurrentList(IEnumerable<T> Elements)
        {
            internalList = new (Elements);
        }

        public ConcurrentList(params T[] Elements)
        {
            internalList = new (Elements);
        }

        #endregion


        public Boolean TryAdd(T Element)
        {
            lock (syncLock)
            {

                //if (internalList.Contains(Element))
                //    return false;

                internalList.Add(Element);

                return true;

            }
        }

        public Boolean TryAddRange(params T[] Elements)
        {
            lock (syncLock)
            {

                internalList.AddRange(Elements);

                return true;

            }
        }

        public Boolean TryAddRange(IEnumerable<T> Elements)
        {
            lock (syncLock)
            {

                internalList.AddRange(Elements);

                return true;

            }
        }

        public Boolean TrySet(T Element)
        {
            lock (syncLock)
            {

                internalList.Clear();
                internalList.Add(Element);

                return true;

            }
        }

        public Boolean TrySet(params T[] Elements)
        {
            lock (syncLock)
            {

                internalList.Clear();
                internalList.AddRange(Elements);

                return true;

            }
        }

        public Boolean TrySet(IEnumerable<T> Elements)
        {
            lock (syncLock)
            {

                internalList.Clear();
                internalList.AddRange(Elements);

                return true;

            }
        }

        public Boolean TryRemove(T Element)
        {
            lock (syncLock)
            {
                return internalList.Remove(Element);
            }
        }

        public void TryRemoveMultiple(params T[] Elements)
        {
            lock (syncLock)
            {
                foreach (var element in Elements)
                    internalList.Remove(element);
            }
        }

        public void TryRemoveMultiple(IEnumerable<T> Elements)
        {
            lock (syncLock)
            {
                foreach (var element in Elements)
                    internalList.Remove(element);
            }
        }

        public Boolean Contains(T Element)
        {
            lock (syncLock)
            {
                return internalList.Contains(Element);
            }
        }

        public T this[Int32 Index]
        {
            get
            {
                lock (syncLock)
                {

                    if (Index < 0 || Index >= internalList.Count)
                        throw new ArgumentOutOfRangeException(nameof(Index));

                    return internalList[Index];

                }
            }
        }


        public void Clear()
        {
            lock (syncLock)
            {
                internalList.Clear();
            }
        }

        public T[] ToArray()
        {
            lock (syncLock)
            {
                return internalList.ToArray();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {

            List<T> snapshot;

            lock (syncLock)
            {
                snapshot = new List<T>(internalList);
            }

            return snapshot.GetEnumerator();

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
