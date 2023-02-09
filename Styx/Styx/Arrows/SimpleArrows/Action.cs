/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{


    public static class ActionArrowExtensions
    {

        public static ActionArrow<TMessage> Call<TMessage>(this IArrowSender<TMessage>  ArrowSender,
                                                           Action<TMessage>             MessageProcessor)
        {
            return new ActionArrow<TMessage>(MessageProcessor, ArrowSender);
        }

    }

    /// <summary>
    /// The ActionArrow is much like the IdentityArrow, but calls
    /// an Action &lt;TIn&gt; on every accepted message/object before
    /// forwarding it.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming and emitting messages/objects.</typeparam>
    public class ActionArrow<TIn> : AbstractArrow<TIn, TIn>
    {

        #region Data

        private Action<TIn> Action;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The ActionArrow is much like the IdentityArrow, but calls
        /// an Action &lt;TIn&gt; on every accepted message/object before
        /// forwarding it.
        /// </summary>
        /// <param name="Action">An Action &lt;S&gt; to invoke on every accepted message/object before forwarding it.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public ActionArrow(Action<TIn>        Action,
                           IArrowSender<TIn>  ArrowSender = null)

            : base(ArrowSender)

        {

            if (Action == null)
                throw new ArgumentNullException("The given Action<TIn> must not be null!");

            this.Action = Action;

        }

        #endregion

        #region (protected) ProcessMessage(MessageIn, out MessageOut)

        /// <summary>
        /// Process the incoming message and return an outgoing message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        /// <param name="MessageOut">The outgoing message.</param>
        protected override Boolean ProcessMessage(TIn MessageIn, out TIn MessageOut)
        {
            MessageOut = MessageIn;
            Action(MessageIn);
            return true;
        }

        #endregion

    }

}
