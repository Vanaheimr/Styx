/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Threading;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// The CountArrow produces a side effect that is the total
    /// number of messages/objects that have passed through it.
    /// </summary>
    public static class CountArrowExtensions
    {

        #region Count(this ArrowSender, InitialValue)

        /// <summary>
        /// The CountArrow produces a side effect that is the total
        /// number of messages/objects that have passed through it.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="InitialValue">The initial value of the counter.</param>
        public static CountArrow<TMessage> Count<TMessage>(this IArrowSender<TMessage>  ArrowSender,
                                                           Int64                        InitialValue = 0L)
        {
            return new CountArrow<TMessage>(InitialValue, ArrowSender);
        }

        #endregion

    }

    /// <summary>
    /// The CountArrow produces a side effect that is the total
    /// number of messages/objects that have passed through it.
    /// </summary>
    /// <typeparam name="TMessage">The type of the consuming and emitting messages/objects.</typeparam>
    public class CountArrow<TMessage> : AbstractSideEffectArrow<TMessage, TMessage, Int64>
    {

        #region Constructor(s)

        /// <summary>
        /// The CountArrow produces a side effect that is the total
        /// number of messages/objects that have passed through it.
        /// </summary>
        /// <param name="InitialValue">The initial value of the counter.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public CountArrow(Int64                   InitialValue = 0L,
                          IArrowSender<TMessage>  ArrowSender  = null)

            : base(ArrowSender)

        {

            base._SideEffect = InitialValue;

        }

        #endregion

        #region ProcessMessage(MessageIn, out MessageOut)

        /// <summary>
        /// Process the incoming message and return an outgoing message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        /// <param name="MessageOut">The outgoing message.</param>
        protected override Boolean ProcessMessage(TMessage MessageIn, out TMessage MessageOut)
        {
            MessageOut = MessageIn;
            Interlocked.Increment(ref _SideEffect);
            return true;
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of this Arrow.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + _SideEffect + ">";
        }

        #endregion

    }

}
