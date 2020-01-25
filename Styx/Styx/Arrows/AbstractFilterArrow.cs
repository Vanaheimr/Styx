/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    #region AbstractFilterArrow<TIn, TOut>

    /// <summary>
    /// An AbstractArrow provides most of the functionality that is repeated
    /// in every instance of an arrow. Any subclass of AbstractArrow should simply
    /// implement ProcessMessage(MessageIn, out MessageOut).
    /// An Arrow accepts/consumes messages/objects of type TIn and emits
    /// messages/objects of type TOut via an event.
    /// </summary>
    /// <typeparam name="TMessage">The type of the filtered messages/objects.</typeparam>
    public abstract class AbstractFilterArrow<TMessage> : AbstractArrow<TMessage, TMessage>, IFilterArrow<TMessage>
    {

        #region Data

        protected readonly Func<TMessage, Boolean>  Include;
        protected readonly Boolean                  InvertedFilter;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract filter arrow.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public AbstractFilterArrow(Func<TMessage, Boolean>  Include         = null,
                                   Boolean                  InvertedFilter  = false,
                                   IArrowSender<TMessage>   ArrowSender     = null)

            : base(ArrowSender)

        { }

        #endregion


        #region (abstract) ForwardMessage(Message)

        /// <summary>
        /// Process the incoming message and decide
        /// whether to forward or filter out.
        /// </summary>
        /// <param name="Message">The message to filter or not.</param>
        /// <returns>True if the message should be forwarded; False otherwise.</returns>
        protected virtual Boolean ForwardMessage(TMessage Message)
        {
            return Include(Message) ^ InvertedFilter;
        }

        #endregion


        protected override Boolean ProcessMessage(TMessage MessageIn, out TMessage MessageOut)
        {

            MessageOut = MessageIn;

            return ForwardMessage(MessageIn);

        }


    }

    #endregion

}
