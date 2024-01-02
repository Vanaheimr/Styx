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
using System.Threading.Tasks;
using System.Collections.Concurrent;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// An AbstractArrowReceiver provides most of the functionality that is repeated
    /// in every instance of a concurrent ArrowReceiver. Any subclass of
    /// AbstractConcurrentArrowReceiver should simply implement ProcessArrow(MessageIn).
    /// An arrow accepts/consumes messages/objects of type TIn.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    public abstract class AbstractConcurrentArrowReceiver<TIn> : AbstractArrowReceiver<TIn>
    {

        #region Data

        private readonly BlockingCollection<TIn>  BlockingCollection;
        private readonly Task                     ArrowSenderTask;

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
        /// Creates a new abstract concurrent arrow receiver.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="MaxQueueSize">The maximum number of queued messages for both arrow senders.</param>
        public AbstractConcurrentArrowReceiver(UInt32            MaxQueueSize  = 1000,
                                               IArrowSender<TIn> ArrowSender   = null)

            : base(ArrowSender)

        {

            this.MaxQueueSize        = MaxQueueSize;
            this.BlockingCollection  = new BlockingCollection<TIn>();

            this.ArrowSenderTask     = Task.Factory.StartNew(() =>
            {

                var Enumerator = BlockingCollection.GetConsumingEnumerable().GetEnumerator();

                while (!BlockingCollection.IsCompleted)
                {

                    // Will block until something becomes available!
                    Enumerator.MoveNext();

                    ProcessArrowConcurrently(Enumerator.Current);

                }

            });

        }

        #endregion


        #region (abstract) ProcessArrowConcurrently(MessageIn)

        /// <summary>
        /// Process the incoming message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        protected abstract void ProcessArrowConcurrently(TIn MessageIn);

        #endregion


        public override void ProcessArrow(TIn Message)
        {
            BlockingCollection.Add(Message);
        }

    }

}
