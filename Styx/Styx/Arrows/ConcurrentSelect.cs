/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// Extension methods for the ConcurrentSelectArrow.
    /// </summary>
    public static partial class ConcurrentSelectArrowExtensions
    {

        #region ConcurrentSelect<TIn, TOut>(this ArrowSender, MessageProcessor,  MaxQueueSize = 1000)

        /// <summary>
        /// A concurrent arrow transforming incoming messages into outgoing messages.
        /// </summary>
        /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
        /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="MessageProcessor">A delegate for transforming incoming messages into outgoing messages.</param>
        /// <param name="MaxQueueSize">The maximum number of queued messages for both arrow senders.</param>
        public static ConcurrentSelectArrow<TIn, TOut> ConcurrentSelect<TIn, TOut>(this IArrowSender<TIn>  ArrowSender,
                                                                                   Func<TIn, TOut>         MessageProcessor,
                                                                                   UInt32                  MaxQueueSize = 1000)
        {
            return new ConcurrentSelectArrow<TIn, TOut>(MessageProcessor, MaxQueueSize, ArrowSender);
        }

        #endregion

    }


    /// <summary>
    /// A concurrent arrow transforming incoming messages into outgoing messages.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class ConcurrentSelectArrow<TIn, TOut> : AbstractConcurrentArrow<TIn, TOut>
    {

        #region Data

        private readonly Func<TIn, TOut> MessageProcessor;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// An arrow transforming incoming messages into outgoing messages.
        /// </summary>
        /// <param name="MessageProcessor">A delegate for transforming incoming messages into outgoing messages.</param>
        /// <param name="MaxQueueSize">The maximum number of queued messages for both arrow senders.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public ConcurrentSelectArrow(Func<TIn, TOut>    MessageProcessor,
                                       UInt32             MaxQueueSize  = 1000,
                                       IArrowSender<TIn>  ArrowSender   = null)

            : base(MaxQueueSize, ArrowSender)

        {

            #region Initial checks

            if (MessageProcessor == null)
                throw new ArgumentNullException("MessageProcessor", "The given 'MessageProcessor' delegate must not be null!");

            #endregion

            this.MessageProcessor = MessageProcessor;

        }

        #endregion

        #region (protected) ProcessMessage(MessageIn, out MessageOut)

        /// <summary>
        /// Process the incoming message and return an outgoing message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        /// <param name="MessageOut">The outgoing message.</param>
        protected override Boolean ProcessMessage(TIn MessageIn, out TOut MessageOut)
        {
            // Try-Catch will be done within AbstractConcurrentArrow.ProcessArrow(MessageIn)
            MessageOut = MessageProcessor(MessageIn);
            return true;
        }

        #endregion

    }

}
