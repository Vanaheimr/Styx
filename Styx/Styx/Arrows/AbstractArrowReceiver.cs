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
    /// An AbstractArrowReceiver provides most of the functionality that is repeated
    /// in every instance of an ArrowReceiver. Any subclass of AbstractArrowReceiver
    /// should simply implement ProcessArrow(MessageIn).
    /// An arrow accepts/consumes messages/objects of type TIn.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    public abstract class AbstractArrowReceiver<TIn> : IArrowReceiver<TIn>
    {

        #region Properties

        public Boolean  WasStarted     { get; private set; }

        public Boolean  IsCompleted    { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// An event called whenever this arrow started to send messages.
        /// </summary>
        public event StartedEventHandler?             OnStarted;

        /// <summary>
        /// An event called whenever an exception occured at this arrow.
        /// </summary>
        public event ExceptionOccurredEventHandler?   OnExceptionOccurred;

        /// <summary>
        /// An event called whenever this arrow will no longer send any messages.
        /// </summary>
        public event CompletedEventHandler?           OnCompleted;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new abstract arrow receiver.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public AbstractArrowReceiver(IArrowSender<TIn>? ArrowSender = null)
        {

            WasStarted   = false;
            IsCompleted  = false;

            ArrowSender?.SendTo(this);

        }

        #endregion


        #region (abstract) ProcessArrow(MessageIn)

        /// <summary>
        /// Accepts a message of type S from a sender for further processing
        /// and delivery to the subscribers.
        /// </summary>
        /// <param name="MessageIn">The message.</param>
        public abstract void ProcessArrow(EventTracking_Id EventTrackingId, TIn MessageIn);

        #endregion


        #region ProcessStarted(Sender, Timestamp, Message = null)

        /// <summary>
        /// The sender of the arrows signalled to start sending arrows.
        /// </summary>
        /// <param name="Sender">The sender of the started message.</param>
        /// <param name="Timestamp">The timestamp of the start.</param>
        /// <param name="Message">An optional message.</param>
        public void ProcessStarted(Object Sender, DateTime Timestamp2, EventTracking_Id EventTrackingId, String? Message = null)
        {

            if (WasStarted == true)
                return;

            try
            {

                WasStarted = true;

                var onStarted = OnStarted;
                onStarted?.Invoke(this, Timestamp2, EventTrackingId, Message);

            }
            catch (Exception e)
            {

                var onExceptionOccurred = OnExceptionOccurred;
                onExceptionOccurred?.Invoke(this, Timestamp.Now, EventTrackingId, e);

            }

        }

        #endregion

        #region ProcessException(Sender, Timestamp, Exception)

        /// <summary>
        /// Process an occured exception.
        /// </summary>
        /// <param name="Sender">The sender of this exception.</param>
        /// <param name="Timestamp">The timestamp of the exception.</param>
        /// <param name="Exception">The occured exception.</param>
        public void ProcessExceptionOccurred(Object            Sender,
                                             DateTimeOffset    Timestamp,
                                             EventTracking_Id  EventTrackingId,
                                             Exception         Exception)
        {

            try
            {

                var onExceptionOccurred = OnExceptionOccurred;
                onExceptionOccurred?.Invoke(this, Timestamp, EventTrackingId, Exception);

            }
            catch
            { }

        }

        #endregion

        #region ProcessCompleted(Sender, Timestamp, Message = null)

        /// <summary>
        /// The sender of the arrows signalled not to send any more arrows.
        /// </summary>
        /// <param name="Sender">The sender of the completed message.</param>
        /// <param name="Timestamp">The timestamp of the shutdown.</param>
        /// <param name="Message">An optional message.</param>
        public void ProcessCompleted(Object Sender, DateTimeOffset Timestamp2, EventTracking_Id EventTrackingId, String? Message = null)
        {

            if (IsCompleted == true)
                return;

            try
            {

                IsCompleted = true;

                var onCompleted = OnCompleted;
                onCompleted?.Invoke(this, Timestamp2, EventTrackingId, Message);

            }
            catch (Exception e)
            {

                var onExceptionOccurred = OnExceptionOccurred;
                onExceptionOccurred?.Invoke(this, Timestamp.Now, EventTrackingId, e);

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

    }

}
