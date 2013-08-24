/*
 * Copyright (c) 2011-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Styx <http://www.github.com/Vanaheimr/Styx>
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
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

#endregion

namespace eu.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// A concurrent arrow for transforming two incoming messages into a single outgoing message.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first consuming messages/objects.</typeparam>
    /// <typeparam name="TIn2">The type of the second consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class ConcurrentCombineArrow<TIn1, TIn2, TOut> : AbstractArrowSender<TOut>
    {

        #region Data

        private readonly BlockingCollection<TIn1>  BlockingCollection1;
        private readonly BlockingCollection<TIn2>  BlockingCollection2;
        private readonly Func<TIn1, TIn2, TOut>    MessagesProcessor;
        private readonly Task                      ArrowSenderTask;

        #endregion

        #region Properties

        #region MaxQueueSize

        /// <summary>
        /// The maximum number of queued messages for both arrow senders.
        /// </summary>
        public UInt32 MaxQueueSize { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new concurrent arrow for transforming two incoming messages into a single outgoing message.
        /// </summary>
        /// <param name="ArrowSender1">The first arrow sender.</param>
        /// <param name="ArrowSender2">The second arrow sender.</param>
        /// <param name="MessagesProcessor">A delegate for transforming two incoming messages into a single outgoing message.</param>
        /// <param name="MaxQueueSize">The maximum number of queued messages for both arrow senders.</param>
        public ConcurrentCombineArrow(IArrowSender<TIn1>      ArrowSender1,
                                      IArrowSender<TIn2>      ArrowSender2,
                                      Func<TIn1, TIn2, TOut>  MessagesProcessor,
                                      UInt32                  MaxQueueSize = 1000)

        {

            #region Initial checks

            if (ArrowSender1 == null)
                throw new ArgumentNullException("ArrowSender1", "The parameter 'ArrowSender1' must not be null!");

            if (ArrowSender2 == null)
                throw new ArgumentNullException("ArrowSender2", "The parameter 'ArrowSender2' must not be null!");

            if (MessagesProcessor == null)
                throw new ArgumentNullException("MessagesProcessor", "The parameter 'MessagesProcessor' must not be null!");

            #endregion

            this.MessagesProcessor    = MessagesProcessor;
            this.MaxQueueSize         = 1000;
            this.BlockingCollection1  = new BlockingCollection<TIn1>();
            this.BlockingCollection2  = new BlockingCollection<TIn2>();

            ArrowSender1.OnNotification += ReceiveMessage1;
            ArrowSender2.OnNotification += ReceiveMessage2;

            this.ArrowSenderTask = Task.Factory.StartNew(() => {

                var Enumerator1 = BlockingCollection1.GetConsumingEnumerable().GetEnumerator();
                var Enumerator2 = BlockingCollection2.GetConsumingEnumerable().GetEnumerator();

                while (!BlockingCollection1.IsCompleted ||
                       !BlockingCollection2.IsCompleted)
                {

                    // Both will block until something becomes available!
                    Enumerator1.MoveNext();
                    Enumerator2.MoveNext();

                    base.NotifyRecipients(this, MessagesProcessor(Enumerator1.Current, Enumerator2.Current));

                }

            });

        }

        #endregion


        #region ReceiveMessage1(MessageIn1)

        /// <summary>
        /// Accepts a message of type TIn1 from an arrow sender for
        /// further processing and delivery to the subscribers.
        /// </summary>
        /// <param name="MessageIn1">A message from sender 1.</param>
        public void ReceiveMessage1(TIn1 MessageIn1)
        {
            if (BlockingCollection1.Count < this.MaxQueueSize)
                BlockingCollection1.Add(MessageIn1);
        }

        #endregion

        #region ReceiveMessage2(MessageIn2)

        /// <summary>
        /// Accepts a message of type TIn2 from an arrow sender for
        /// further processing and delivery to the subscribers.
        /// </summary>
        /// <param name="MessageIn2">A message from sender 2.</param>
        public void ReceiveMessage2(TIn2 MessageIn2)
        {
            if (BlockingCollection2.Count < this.MaxQueueSize)
                BlockingCollection2.Add(MessageIn2);
        }

        #endregion

    }

}
