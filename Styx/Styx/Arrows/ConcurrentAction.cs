/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// Extension methods for the ConcurrentActionArrow&lt;TIn&gt;.
    /// </summary>
    public static class ConcurrentActionArrowExtensions
    {

        #region CallConcurrently<TIn>(this ArrowSender, MessageAction, MaxQueueSize = 1000)

        /// <summary>
        /// The ActionArrow is much like the IdentityArrow, but calls
        /// an Action &lt;TIn&gt; on every accepted message/object before
        /// forwarding it.
        /// </summary>
        /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="MessageAction">A delegate called concurrently for every incoming message.</param>
        /// <param name="MaxQueueSize">The maximum number of queued messages for both arrow senders.</param>
        public static ConcurrentActionArrow<TIn> CallConcurrently<TIn>(this IArrowSender<TIn>  ArrowSender,
                                                                       Action<TIn>             MessageAction,
                                                                       UInt32                  MaxQueueSize  = 1000)
        {
            return new ConcurrentActionArrow<TIn>(MessageAction, MaxQueueSize, ArrowSender);
        }

        #endregion

    }


    /// <summary>
    /// The ActionArrow is much like the IdentityArrow, but calls
    /// an Action &lt;TIn&gt; on every accepted message/object before
    /// forwarding it.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    public class ConcurrentActionArrow<TIn> : AbstractConcurrentArrow<TIn, TIn>
    {

        #region Data

        private readonly Action<TIn> MessageAction;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new arrow calling the given delegate concurrently.
        /// </summary>
        /// <param name="MessageAction">A delegate called concurrently for every incoming message.</param>
        /// <param name="MaxQueueSize">The maximum number of queued messages for both arrow senders.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public ConcurrentActionArrow(Action<TIn>         MessageAction,
                                     UInt32              MaxQueueSize  = 1000,
                                     IArrowSender<TIn>?  ArrowSender   = null)

            : base(MaxQueueSize, ArrowSender)

        {

            if (MessageAction is null)
                throw new ArgumentNullException("MessageAction", "The given 'MessageAction' delegate must not be null!");

            this.MessageAction = MessageAction;

        }

        #endregion


        protected override Boolean ProcessMessage(EventTracking_Id EventTrackingId, TIn MessageIn, out TIn MessageOut)
        {
            MessageOut = MessageIn;
            MessageAction(MessageIn);
            return true;
        }

    }

}
