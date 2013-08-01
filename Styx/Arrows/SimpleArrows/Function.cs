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

    public static class FunctionArrowExtentions
    {

        public static FunctionArrow<TIn, TOut> Call<TIn, TOut>(this IArrowSender<TIn>      Source,
                                                               Func<TIn, TOut>             MessageProcessor,
                                                               Func<Exception, Exception>  OnError = null)
        {
            return new FunctionArrow<TIn, TOut>(MessageProcessor, OnError, Source);
        }

        public static FunctionArrow<TIn1, TIn2, TOut> Call<TIn1, TIn2, TOut>(this IArrowSender<TIn1, TIn2>  Source,
                                                                             Func<TIn1, TIn2, TOut>         MessageProcessor,
                                                                             Func<Exception, Exception>     OnError = null)
        {
            return new FunctionArrow<TIn1, TIn2, TOut>(MessageProcessor, OnError, Source);
        }

    }


    /// <summary>
    /// Transform the message of every incoming arrow by the given delegate.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class FunctionArrow<TIn, TOut> : AbstractArrow<TIn, TOut>
    {

        #region Data

        private readonly Func<TIn,       TOut>       MessageProcessor;
        private readonly Func<Exception, Exception>  OnError;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new FunctionArrow transforming the message of every
        /// incoming arrow by the given delegate.
        /// </summary>
        public FunctionArrow(Func<TIn, TOut>             MessageProcessor,
                             Func<Exception, Exception>  OnError = null,
                             IArrowSender<TIn>           ArrowSender  = null)

            : base(ArrowSender)

        {

            if (MessageProcessor == null)
                throw new ArgumentNullException("The given delegate must not be null!");

            this.MessageProcessor  = MessageProcessor;
            this.OnError           = OnError;

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
            // Try-Catch will be done within AbstractArrow.ProcessArrow(MessageIn)
            MessageOut = MessageProcessor(MessageIn);
            return true;
        }

        #endregion

    }



    public class FunctionArrow<TIn1, TIn2, TOut> : IArrowReceiver<TIn1, TIn2>, IArrowSender<TOut>
    {

        #region Data

        private readonly Func<TIn1, TIn2, TOut> MessageProcessor;
        private readonly Func<Exception, Exception> OnErrorD;

        #endregion

        #region Events

        public event NotificationEventHandler<TOut> OnNotification;

        public event ExceptionEventHandler OnException;

        public event CompletedEventHandler OnCompleted;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Filters the consuming objects by calling a Func&lt;S, Boolean&gt;.
        /// </summary>
        /// <param name="Func">A Func&lt;S, Boolean&gt; filtering the consuming objects. True means filter (ignore).</param>
        public FunctionArrow(Func<TIn1, TIn2, TOut>      MessageProcessor,
                            Func<Exception, Exception>  OnError = null,
                            IArrowSender<TIn1, TIn2>   Source  = null)

        {

            if (MessageProcessor == null)
                throw new ArgumentNullException("The given delegate must not be null!");

            this.MessageProcessor  = MessageProcessor;
            this.OnErrorD          = OnError;

            if (Source != null)
                Source.SendTo(this);

        }

        #endregion

        #region ProcessArrow(MessageIn1, MessageIn2)

        /// <summary>
        /// Process the incoming message and send an outgoing message.
        /// </summary>
        /// <param name="MessageIn1">The first incoming message.</param>
        /// <param name="MessageIn2">The second incoming message.</param>
        public void ProcessArrow(TIn1 MessageIn1, TIn2 MessageIn2)
        {

            if (OnNotification != null)
                OnNotification(MessageProcessor(MessageIn1, MessageIn2));

        }

        #endregion

        public void ProcessException(dynamic Sender, Exception ExceptionMessage)
        {
            if (OnException != null)
                OnException(this, ExceptionMessage);
        }

        public void ProcessCompleted(dynamic Sender, String Message)
        {
            if (OnCompleted != null)
                OnCompleted(this, Message);
        }


    }


}
