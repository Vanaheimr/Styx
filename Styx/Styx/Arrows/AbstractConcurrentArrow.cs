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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    #region AbstractArrow<TIn, TOut>

    /// <summary>
    /// An AbstractArrow provides most of the functionality that is repeated
    /// in every instance of an arrow. Any subclass of AbstractArrow should simply
    /// implement ProcessMessage(MessageIn, out MessageOut).
    /// An Arrow accepts/consumes messages/objects of type TIn and emits
    /// messages/objects of type TOut via an event.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public abstract class AbstractConcurrentArrow<TIn, TOut> : AbstractConcurrentArrowReceiver<TIn>, IArrow<TIn, TOut>
    {

        #region Events

        /// <summary>
        /// An event called whenever this arrow sends a new message.
        /// </summary>
        public event NotificationEventHandler<TOut>?  OnNotification;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract concurrent arrow.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="MaxQueueSize">The maximum number of queued messages for both arrow senders.</param>
        public AbstractConcurrentArrow(UInt32              MaxQueueSize   = 1000,
                                       IArrowSender<TIn>?  ArrowSender    = null)

            : base(MaxQueueSize, ArrowSender)

        { }

        #endregion


        #region (abstract) ProcessMessage(MessageIn, out MessageOut)

        /// <summary>
        /// Process the incoming message and return an outgoing message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        /// <param name="MessageOut">The outgoing message.</param>
        /// <returns>True if the message should be forwarded; False otherwise.</returns>
        protected abstract Boolean ProcessMessage(EventTracking_Id EventTrackingId, TIn MessageIn, out TOut MessageOut);

        #endregion


        #region ProcessArrowConcurrently(Message)

        /// <summary>
        /// Process the incoming arrow.
        /// </summary>
        /// <param name="Message">The message of the arrow.</param>
        protected override void ProcessArrowConcurrently(EventTracking_Id EventTrackingId, TIn Message)
        {

            TOut MessageOut;

            try
            {

                if (ProcessMessage(EventTrackingId, Message, out MessageOut))
                    OnNotification?.Invoke(EventTrackingId, MessageOut);

            }
            catch (Exception e)
            {
                base.ProcessExceptionOccurred(this, Timestamp.Now, EventTrackingId, e);
            }

        }

        #endregion

    }

    #endregion

}
