/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    public static class ToIEnumerableExtension
    {

        public static IEnumerable<TMessage> ToIEnumerable<TMessage>(this IArrowSender<TMessage> INotification)
        {
            return new ArrowSink<TMessage>(INotification);
        }

    }


    /// <summary>
    /// Filters the consuming objects by calling a Func&lt;S, Boolean&gt;.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming and emitting messages/objects.</typeparam>
    public class ArrowSink<TIn> : IArrowReceiver<TIn>, IEnumerable<TIn>, IEnumerator<TIn>
    {

        #region Data

        /// <summary>
        /// A blocking collection as inter-thread message pipeline.
        /// </summary>
        private readonly BlockingCollection<TIn> BlockingCollection;

        private readonly IEnumerator<TIn> _InternalEnumerator;

        #endregion

        #region Constructor(s)

        #region ArrowSink()

        /// <summary>
        /// Filters the consuming objects by calling a Func&lt;S, Boolean&gt;.
        /// </summary>
        /// <param name="Func">A Func&lt;S, Boolean&gt; filtering the consuming objects. True means filter (ignore).</param>
        public ArrowSink()
        {
            this.BlockingCollection   = new BlockingCollection<TIn>();
            this._InternalEnumerator  = BlockingCollection.GetConsumingEnumerable().GetEnumerator();
        }

        #endregion

        #region (internal) ArrowSink(INotification)

        /// <summary>
        /// Filters the consuming objects by calling a Func&lt;S, Boolean&gt;.
        /// </summary>
        /// <param name="Func">A Func&lt;S, Boolean&gt; filtering the consuming objects. True means filter (ignore).</param>
        internal ArrowSink(IArrowSender<TIn> INotification)
        {

            if (INotification != null)
                INotification.SendTo(this);

            this.BlockingCollection   = new BlockingCollection<TIn>();
            this._InternalEnumerator  = BlockingCollection.GetConsumingEnumerable().GetEnumerator();

        }

        #endregion

        #endregion


        public void ProcessArrow(TIn Message)
        {
            BlockingCollection.Add(Message);
        }

        public void ProcessExceptionOccured(dynamic Sender, DateTime Timestamp, Exception ExceptionMessage)
        {
            BlockingCollection.CompleteAdding();
        }

        public void ProcessCompleted(dynamic Sender, DateTime Timestamp, String Message)
        {
            BlockingCollection.CompleteAdding();
        }



        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator&lt;E&gt; that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TIn> GetEnumerator()
        {
            return _InternalEnumerator;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _InternalEnumerator;
        }

        #endregion

        #region Current

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public TIn Current
        {
            get
            {
                return _InternalEnumerator.Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        Object System.Collections.IEnumerator.Current
        {
            get
            {
                return _InternalEnumerator.Current;
            }
        }

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public Boolean MoveNext()
        {
            return _InternalEnumerator.MoveNext();
        }

        #endregion

        #region Reset()

        /// <summary>
        /// Sets the enumerator to its initial position, which is
        /// before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _InternalEnumerator.Reset();
        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Disposes this pipe.
        /// </summary>
        public virtual void Dispose()
        {
            _InternalEnumerator.Dispose();
        }

        #endregion

    }

}
