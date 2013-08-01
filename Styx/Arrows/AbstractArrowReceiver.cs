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

namespace eu.Vanaheimr.Styx.Arrows
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

        #region Events

        /// <summary>
        /// An event called whenever an exception occured at this arrow.
        /// </summary>
        public event ExceptionEventHandler OnException;

        /// <summary>
        /// An event called whenever this arrow will no longer send any messages.
        /// </summary>
        public event CompletedEventHandler OnCompleted;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new abstract arrow receiver.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public AbstractArrowReceiver(IArrowSender<TIn> ArrowSender = null)
        {
            if (ArrowSender != null)
                ArrowSender.SendTo(this);
        }

        #endregion


        #region (abstract) ProcessArrow(MessageIn)

        /// <summary>
        /// Accepts a message of type S from a sender for further processing
        /// and delivery to the subscribers.
        /// </summary>
        /// <param name="MessageIn">The message.</param>
        public abstract void ProcessArrow(TIn MessageIn);

        #endregion

        #region ProcessException(Sender, Exception)

        /// <summary>
        /// Process an occured exception.
        /// </summary>
        /// <param name="Sender">The sender of this exception.</param>
        /// <param name="Exception">The occured exception.</param>
        public void ProcessException(dynamic Sender, Exception Exception)
        {

            try
            {

                var OnErrorLocal = OnException;

                if (OnErrorLocal != null)
                    OnErrorLocal(this, Exception);

            }
            catch (Exception)
            { }

        }

        #endregion

        #region ProcessCompleted(Sender, Message = null)

        /// <summary>
        /// The sender of the arrows signaled not to send any more arrows.
        /// </summary>
        /// <param name="Sender">The sender of the completed message.</param>
        /// <param name="Message">An optional message.</param>
        public void ProcessCompleted(dynamic Sender, String Message = null)
        {

            try
            {

                var OnCompletedLocal = OnCompleted;

                if (OnCompletedLocal != null)
                    OnCompletedLocal(this);

            }
            catch (Exception e)
            {

                var OnExceptionLocal = OnException;

                if (OnExceptionLocal != null)
                    OnExceptionLocal(this, e);

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
