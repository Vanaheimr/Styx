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

using System.Collections;
using System.Threading;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Collections
{

    public class LockFreeQueue<T> : IEnumerable<T>
    {

        public delegate Task QueueDelegate(LockFreeQueue<T>  sender,
                                           T                 value,
                                           CancellationToken cancellationToken = default);

        /// <summary>
        /// Called whenever a new element is successfully enqueued.
        /// </summary>
        public event QueueDelegate? OnAdded;

        private class Node(T value)
        {
            public T     Value = value;
            public Node? Next;
        }

        private Node  head;
        private Node  tail;
        private Int64 count;


        /// <summary>
        /// The maximum number of elements in the queue.
        /// </summary>
        public UInt64 MaxNumberOfElements    { get; }

        /// <summary>
        /// The number of elements currently in the queue.
        /// </summary>
        public UInt64 Count
            => (UInt64) Interlocked.Read(ref count);


        public LockFreeQueue(UInt64 MaxNumberOfCachedEvents = 500)
        {
            this.head = this.tail     = new Node(default!);
            this.MaxNumberOfElements  = MaxNumberOfCachedEvents;
        }

        /// <summary>
        /// Enqueue a new item at the tail.
        /// </summary>
        public async Task Enqueue(T item, CancellationToken cancellationToken = default)
        {

            var newNode = new Node(item);

            while (true)
            {

                var tailSnapshot = tail;
                var nextSnapshot = Volatile.Read(ref tailSnapshot.Next);

                if (nextSnapshot is null)
                {
                    // Try to link the new node
                    if (Interlocked.CompareExchange(ref tailSnapshot.Next, newNode, null) is null)
                    {

                        // Successfully linked; increment the count
                        Interlocked.Increment(ref count);

                        // Move 'tail' pointer forward if tail is still tailSnapshot
                        Interlocked.CompareExchange(ref tail, newNode, tailSnapshot);

                        // If we've exceeded MaxNumberOfCachedEvents, remove one from the head
                        if ((UInt64) Interlocked.Read(ref count) > MaxNumberOfElements)
                        {
                            // Best effort: remove one item
                            Dequeue();
                        }

                        // Now raise the OnAdded event if any listeners
                        var onAdded = OnAdded;
                        if (onAdded is not null)
                        {
                            // If multiple handlers, run them concurrently
                            var delegates = onAdded.GetInvocationList()
                            .Cast<QueueDelegate>()
                                                   .Select(d => d(this, item, cancellationToken));
                            await Task.WhenAll(delegates);
                        }

                        return;

                    }
                }
                else
                {
                    // Another thread already added a node, so move tail forward and retry
                    Interlocked.CompareExchange(ref tail, nextSnapshot, tailSnapshot);
                }

            }
        }

        /// <summary>
        /// Dequeue the oldest item from the head. Returns default(T) if empty.
        /// </summary>
        public T Dequeue()
        {
            while (true)
            {

                var headSnapshot = head;
                var tailSnapshot = tail;
                var nextSnapshot = Volatile.Read(ref headSnapshot.Next);

                if (headSnapshot == tailSnapshot)
                {

                    // The queue is (probably) empty
                    if (nextSnapshot is null)
                        return default!;

                    // Tail is lagging behind, so move it forward
                    Interlocked.CompareExchange(ref tail, nextSnapshot, tailSnapshot);

                }
                else
                {

                    // There's at least one real node
                    if (nextSnapshot is null)
                        return default!;  // Might happen transiently

                    T value = nextSnapshot.Value;
                    // Try to move head forward
                    if (Interlocked.CompareExchange(ref head, nextSnapshot, headSnapshot) == headSnapshot)
                    {
                        Interlocked.Decrement(ref count);
                        return value;
                    }

                }

            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            // Start from head.Next to skip the dummy node
            var current = Volatile.Read(ref head).Next;
            while (current is not null)
            {
                yield return current.Value;
                current = Volatile.Read(ref current.Next);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

    }

}
