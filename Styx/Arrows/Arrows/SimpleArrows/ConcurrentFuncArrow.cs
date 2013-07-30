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
using System.Collections.Concurrent;

#endregion

namespace eu.Vanaheimr.Styx
{

    /// <summary>
    /// A generic Arrow transforming incoming messages into outgoing messages.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class ConcurrentFuncArrow<TIn, TOut> : AbstractArrowSender<TOut>, IArrow<TIn, TOut>
    {

        #region Data

        /// <summary>
        /// A blocking collection as inter-thread message pipeline.
        /// </summary>
        private readonly BlockingCollection<TIn> BlockingCollection;

        /// <summary>
        /// A delegate for transforming incoming messages into outgoing messages.
        /// </summary>
        private readonly Func<TIn, TOut> MessageProcessor;

        /// <summary>
        /// The internal arrow sender task.
        /// </summary>
        private readonly Task ArrowSenderTask;

        #endregion

        #region Properties

        #region MaxQueueSize

        /// <summary>
        /// The maximum number of queued messages.
        /// </summary>
        public UInt32 MaxQueueSize { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// An arrow transforming incoming messages into outgoing messages.
        /// </summary>
        /// <param name="MessageProcessor">A delegate for transforming incoming messages into outgoing messages.</param>
        public ConcurrentFuncArrow(Func<TIn, TOut> MessageProcessor)
        {

            #region Initial checks

            if (MessageProcessor == null)
                throw new ArgumentNullException("MessageProcessor", "The given 'MessageProcessor' delegate must not be null!");

            #endregion

            this.MessageProcessor   = MessageProcessor;
            this.BlockingCollection = new BlockingCollection<TIn>();
            this.MaxQueueSize       = 1000;

            this.ArrowSenderTask    = Task.Factory.StartNew(() => {

                var Enumerator = BlockingCollection.GetConsumingEnumerable().GetEnumerator();

                while (!BlockingCollection.IsCompleted)
                {

                    // Both will block until something is available!
                    Enumerator.MoveNext();

                    base.NotifyRecipients(this, MessageProcessor(Enumerator.Current));

                }

            });

        }

        #endregion


        #region ProcessArrow(Sender, MessageIn)

        /// <summary>
        /// Accepts a message of type S from a sender for further processing
        /// and delivery to the subscribers.
        /// </summary>
        /// <param name="Sender">The sender of the message.</param>
        /// <param name="MessageIn">The message.</param>
        public void ProcessArrow(TIn MessageIn)
        {
            BlockingCollection.Add(MessageIn);
        }

        #endregion



        public void ProcessError(dynamic Sender, Exception ExceptionMessage)
        {
            throw new NotImplementedException();
        }

        public void ProcessCompleted(dynamic Sender, string Message = null)
        {
            throw new NotImplementedException();
        }

    }

}
