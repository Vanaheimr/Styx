/*
 * Copyright (c) 2010-2016, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

    /// <summary>
    /// Extention methods for the CalcArrow.
    /// </summary>
    public static class CalcArrowExtensions
    {

        #region Calc(this ArrowSender, InitialValue, MessageProcessor)

        /// <summary>
        /// Creat a new sideeffect arrow calculating the new sideeffect
        /// and the output message based on the current sideeffect and
        /// the input message.
        /// </summary>
        /// <typeparam name="TData">The type of the consuming messages, the sideeffect and the emitted messages.</typeparam>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="InitialValue">The inital value of the sideeffect of this arrow.</param>
        /// <param name="MessageProcessor">A delegate calculating the new sideeffect and the output message based on the current sideeffect and the input message.</param>
        public static CalcArrow<TData> Calc<TData>(this IArrowSender<TData>   ArrowSender,
                                                   TData                      InitialValue,
                                                   Func<TData, TData, TData>  MessageProcessor)

            where TData : IComparable, IComparable<TData>, IEquatable<TData>

        {
            return new CalcArrow<TData>(InitialValue, MessageProcessor, ArrowSender);
        }

        #endregion

    }


    /// <summary>
    /// A sideeffect arrow calculating the new sideeffect
    /// and the output message based on the current
    /// sideeffect and the input message.
    /// </summary>
    /// <typeparam name="TData">The type of the consuming messages, the sideeffect and the emitted messages.</typeparam>
    public class CalcArrow<TData> : AbstractSideEffectArrow<TData, TData, TData>
        where TData : IComparable, IComparable<TData>, IEquatable<TData>
    {

        #region Properties

        private readonly Func<TData, TData, TData> MessageProcessor;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creat a new sideeffect arrow calculating the new sideeffect
        /// and the output message based on the current sideeffect and
        /// the input message.
        /// </summary>
        /// <param name="InitialValue">The inital value of the sideeffect of this arrow.</param>
        /// <param name="MessageProcessor">A delegate calculating the new sideeffect and the output message based on the current sideeffect and the input message.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public CalcArrow(TData                      InitialValue,
                         Func<TData, TData, TData>  MessageProcessor,
                         IArrowSender<TData>        ArrowSender = null)
        {

            if (MessageProcessor == null)
                throw new ArgumentNullException("MessageProcessor", "The MessageProcessor must not be null!");

            this.SideEffect        = InitialValue;
            this.MessageProcessor  = MessageProcessor;

        }

        #endregion

        #region ProcessMessage(MessageIn, out MessageOut)

        /// <summary>
        /// Process the incoming message and return an outgoing message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        /// <param name="MessageOut">The outgoing message.</param>
        protected override Boolean ProcessMessage(TData MessageIn, out TData MessageOut)
        {

            MessageOut = SideEffect = this.MessageProcessor(this.SideEffect, MessageIn);

            return true;

        }

        #endregion

    }

}
