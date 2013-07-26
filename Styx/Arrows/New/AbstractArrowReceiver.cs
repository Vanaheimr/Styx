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

    /// <summary>
    /// An AbstractArrow provides most of the functionality that is repeated
    /// in every instance of an Arrow. Any subclass of AbstractPipe should simply
    /// implement ProcessMessage(MessageIn, out MessageOut).
    /// An Arrow accepts/consumes messages/objects of type TIn and emits
    /// messages/objects of type TOut via an event.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    public abstract class AbstractArrowReceiver<TIn> : IArrowReceiver<TIn>
    {

        #region Constructor(s)

        #region AbstractArrowReceiver()

        /// <summary>
        /// Creates a new AbstractArrowReceiver.
        /// </summary>
        public AbstractArrowReceiver()
        { }

        #endregion

        #endregion


        #region (abstract) ProcessArrow(MessageIn)

        /// <summary>
        /// Accepts a message of type S from a sender for further processing
        /// and delivery to the subscribers.
        /// </summary>
        /// <param name="MessageIn">The message.</param>
        public abstract void ProcessArrow(TIn MessageIn);

        #endregion

        #region ProcessCompleted(Sender, Message = null)

        /// <summary>
        /// Signale the completion of the message delivery.
        /// </summary>
        /// <param name="Sender">The sender of the completion signal.</param>
        /// <param name="Message">The message.</param>
        public void ProcessCompleted(dynamic Sender, String Message = null)
        {
            //try
            //{
            //    if (OnCompleted != null)
            //        OnCompleted(this);
            //}
            //catch (Exception e)
            //{
            //    if (OnError != null)
            //        OnError(this, e);
            //}
        }

        #endregion

        public void ProcessError(dynamic Sender, Exception ExceptionMessage)
        {
        }


        #region Dispose()

        /// <summary>
        /// Disposes this pipe.
        /// </summary>
        public virtual void Dispose()
        { }

        #endregion

        #region ToString()

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
