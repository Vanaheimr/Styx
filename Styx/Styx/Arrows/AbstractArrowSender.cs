﻿/*
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
    /// An AbstractArrowSender provides most of the functionality that is repeated
    /// in every instance of an Arrow. Any subclass of AbstractPipe should simply
    /// implement ProcessMessage(MessageIn, out MessageOut).
    /// An Arrow accepts/consumes messages/objects of type TIn and emits
    /// messages/objects of type TOut via an event.
    /// </summary>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public abstract class AbstractArrowSender<TOut> : IArrowSender<TOut>
    {

        #region Events

        /// <summary>
        /// An event for signaling the start of a message delivery.
        /// </summary>
        public event StartedEventHandler?               OnStarted;

        /// <summary>
        /// An event for message delivery.
        /// </summary>
        public event NotificationEventHandler<TOut>?    OnNotification;

        /// <summary>
        /// An event for signaling an exception.
        /// </summary>
        public event ExceptionOccurredEventHandler?      OnExceptionOccurred;

        /// <summary>
        /// An event for signaling the completion of a message delivery.
        /// </summary>
        public event CompletedEventHandler?             OnCompleted;

        #endregion

        #region Properties

        /// <summary>
        /// Turns the recording of the message delivery path ON or OFF.
        /// </summary>
        public Boolean              RecordMessagePath    { get; set; }

        /// <summary>
        /// Returns the message path.
        /// </summary>
        public IEnumerable<Object>  Path                 { get; protected set;  }

        #endregion

        #region Constructor(s)

        #region AbstractArrowSender()

        /// <summary>
        /// Creates a new AbstractArrowSender.
        /// </summary>
        public AbstractArrowSender()
        { }

        #endregion

        #region AbstractArrowSender(MessageRecipients.Recipient, params MessageRecipients.Recipients)

        ///// <summary>
        ///// Creates a new AbstractArrow and adds the given recipients
        ///// to the list of message recipients.
        ///// </summary>
        ///// <param name="Recipient">A recipient of the processed messages.</param>
        ///// <param name="Recipients">The recipients of the processed messages.</param>
        //public AbstractArrowSender(MessageRecipient<TOut> Recipient, params MessageRecipient<TOut>[] Recipients)
        //{

        //    lock (this)
        //    {

        //        if (Recipient != null)
        //            this.OnMessageAvailable += Recipient;

        //        if (Recipients != null)
        //            foreach (var _Recipient in Recipients)
        //                this.OnMessageAvailable += _Recipient;

        //    }

        //}

        #endregion

        #region AbstractArrowSender(IArrowReceiver.Recipient, params IArrowReceiver.Recipients)

        ///// <summary>
        ///// Creates a new AbstractArrow and adds the given recipients
        ///// to the list of message recipients.
        ///// </summary>
        ///// <param name="Recipient">A recipient of the processed messages.</param>
        ///// <param name="Recipients">The recipients of the processed messages.</param>
        //public AbstractArrowSender(IArrowReceiver<TOut> Recipient, params IArrowReceiver<TOut>[] Recipients)
        //{

        //    lock (this)
        //    {

        //        if (Recipient != null)
        //            this.OnMessageAvailable += Recipient.ReceiveMessage;

        //        if (Recipients != null)
        //            foreach (var _Recipient in Recipients)
        //                this.OnMessageAvailable += _Recipient.ReceiveMessage;

        //    }

        //}

        #endregion

        #endregion


        #region (protected) NotifyRecipients(Sender, Message)

        /// <summary>
        /// Accepts a message of type S from a sender for further processing
        /// and delivery to the subscribers.
        /// </summary>
        /// <param name="Sender">The sender of the message.</param>
        /// <param name="MessageIn">The message.</param>
        /// <returns>True if the message was accepted and could be processed; False otherwise.</returns>
        protected Boolean NotifyRecipients(EventTracking_Id EventTrackingId, Object Sender, TOut Message)
        {

            try
            {

                OnNotification?.Invoke(EventTrackingId, Message);

                return true;

            }
            catch (Exception e)
            {
                OnExceptionOccurred?.Invoke(this, Timestamp.Now, EventTrackingId, e);
            }

            return false;

        }

        #endregion


        #region Complete(Sender)

        /// <summary>
        /// Signale the completion of the message delivery.
        /// </summary>
        /// <param name="Sender">The sender of the completion signal.</param>
        /// <param name="Timestamp">The timestamp of the shutdown.</param>
        /// <param name="Message">An optional shutdown message.</param>
        public void Complete(Object Sender, DateTime Timestamp, EventTracking_Id EventTrackingId, String? Message = null)
        {
            try
            {
                OnCompleted?.Invoke(this, Timestamp, EventTrackingId, Message);
            }
            catch (Exception e)
            {
                OnExceptionOccurred?.Invoke(this, Timestamp, EventTrackingId, e);
            }
        }

        #endregion


        #region Dispose()

        /// <summary>
        /// Disposes this pipe.
        /// </summary>
        public virtual void Dispose()
        { }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// A string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return this.GetType().Name;
        }

        #endregion

    }

}
