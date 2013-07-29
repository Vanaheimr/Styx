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
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.Styx
{

    #region AbstractArrow<TIn, TOut>

    /// <summary>
    /// An AbstractArrow provides most of the functionality that is repeated
    /// in every instance of an Arrow. Any subclass of AbstractPipe should simply
    /// implement ProcessMessage(MessageIn, out MessageOut).
    /// An Arrow accepts/consumes messages/objects of type TIn and emits
    /// messages/objects of type TOut via an event.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public abstract class AbstractArrow<TIn, TOut> : IArrow<TIn, TOut>
    {

        #region Events

        public event NotificationEventHandler<TOut>  OnNotification;

        public event ExceptionEventHandler           OnError;

        public event CompletedEventHandler           OnCompleted;

        #endregion

        #region Constructor(s)

        public AbstractArrow(IArrowSender<TIn> ArrowSender = null)
        {

            if (ArrowSender != null)
                ArrowSender.SendTo(this);

        }

        #endregion


        #region (abstract) ProcessMessage(MessageIn, out MessageOut)

        /// <summary>
        /// Process the incoming message and return an outgoing message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        /// <param name="MessageOut">The outgoing message.</param>
        /// <returns>True if the message should be forwarded; False otherwise.</returns>
        protected abstract Boolean ProcessMessage(TIn MessageIn, out TOut MessageOut);

        #endregion


        public void ProcessArrow(TIn Message)
        {

            TOut MessageOut = default(TOut);

            if (ProcessMessage(Message, out MessageOut))
            {
                var OnNotificationLocal = OnNotification;
                if (OnNotificationLocal != null)
                    OnNotificationLocal(MessageOut);
            }

        }

        public void ProcessError(dynamic Sender, Exception ExceptionMessage)
        {
            var OnErrorLocal = OnError;
            if (OnErrorLocal != null)
                OnErrorLocal(this, ExceptionMessage);
        }

        public void ProcessCompleted(dynamic Sender, String Message)
        {
            var OnCompletedLocal = OnCompleted;
            if (OnCompletedLocal != null)
                OnCompletedLocal(this, Message);
        }



        public void Dispose()
        {
        }

    }

    #endregion

}
