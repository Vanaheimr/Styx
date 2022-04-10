/*
 * Copyright (c) 2011-2012, Achim Friedland <achim.friedland@graphdefined.com>
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
using System.Collections.Concurrent;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// Filters the consuming objects by calling a Func&lt;S, Boolean&gt;.
    /// </summary>
    public static class CombineArrowExtensions
    {

        public static CombineArrow<TIn1, TIn2, TOut>
            Combine<TIn1, TIn2, TOut>(this IArrowSender<TIn1>  ArrowSender1,
                                      IArrowSender<TIn2>       ArrowSender2,
                                      Func<TIn1, TIn2, TOut>   MessagesProcessor,
                                      UInt32                   MaxQueueSize = 500)
        {
            return new CombineArrow<TIn1, TIn2, TOut>(ArrowSender1, ArrowSender2, MessagesProcessor);
        }

    }


    /// <summary>
    /// An arrow for transforming two incoming messages into a single outgoing message.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first consuming messages/objects.</typeparam>
    /// <typeparam name="TIn2">The type of the second consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class CombineArrow<TIn1, TIn2, TOut> : AbstractArrowSender<TOut>
    {

        #region Data

        private readonly IArrowSender<TIn1>      ArrowSender1;
        private readonly IArrowSender<TIn2>      ArrowSender2;
        private readonly Func<TIn1, TIn2, TOut>  MessagesProcessor;

        private readonly ConcurrentQueue<TIn1>   Queue1;
        private readonly ConcurrentQueue<TIn2>   Queue2;

        #endregion

        #region MaxQueueSize

        /// <summary>
        /// The maximum number of queued messages for both arrow senders.
        /// </summary>
        public UInt32 MaxQueueSize { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new arrow for transforming two incoming messages into a single outgoing message.
        /// </summary>
        /// <param name="ArrowSender1">The first arrow sender.</param>
        /// <param name="ArrowSender2">The second arrow sender.</param>
        /// <param name="MessagesProcessor">A delegate for transforming two incoming messages into a single outgoing message.</param>
        /// <param name="MaxQueueSize">The maximum number of queued messages for both arrow senders.</param>
        public CombineArrow(IArrowSender<TIn1>      ArrowSender1,
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

            this.ArrowSender1       = ArrowSender1;
            this.ArrowSender2       = ArrowSender2;
            this.MessagesProcessor  = MessagesProcessor;
            this.MaxQueueSize       = MaxQueueSize;

            this.Queue1             = new ConcurrentQueue<TIn1>();
            this.Queue2             = new ConcurrentQueue<TIn2>();

            this.ArrowSender1.OnNotification += ReceiveMessage1;
            this.ArrowSender2.OnNotification += ReceiveMessage2;

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

            TIn2 MessageIn2;

            if (Queue2.TryDequeue(out MessageIn2))
                base.NotifyRecipients(this, MessagesProcessor(MessageIn1, MessageIn2));

            else if (this.Queue1.Count < this.MaxQueueSize)
                Queue1.Enqueue(MessageIn1);

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

            TIn1 MessageIn1;

            if (Queue1.TryDequeue(out MessageIn1))
                base.NotifyRecipients(this, MessagesProcessor(MessageIn1, MessageIn2));

            else if (this.Queue2.Count < this.MaxQueueSize)
                this.Queue2.Enqueue(MessageIn2);

        }

        #endregion


    }

}
