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
    /// Extention methods for the SelectArrow.
    /// </summary>
    public static partial class SelectArrowExtentions
    {

        public static SelectArrow<TIn, TOut> Select<TIn, TOut>(this IArrowSender<TIn>      Source,
                                                               Func<TIn, TOut>             MessageProcessor,
                                                               Func<Exception, Exception>  OnError = null)
        {
            return new SelectArrow<TIn, TOut>(MessageProcessor, OnError, Source);
        }

    }


    /// <summary>
    /// Transform the message of every incoming arrow by the given delegate.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class SelectArrow<TIn, TOut> : AbstractArrow<TIn, TOut>
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
        public SelectArrow(Func<TIn, TOut>             MessageProcessor,
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

}
