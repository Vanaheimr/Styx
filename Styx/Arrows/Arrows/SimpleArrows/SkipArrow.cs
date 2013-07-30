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
using System.Threading;

#endregion

namespace eu.Vanaheimr.Styx
{

    /// <summary>
    /// The SkipArrow simply sends the incoming message to the recipients
    /// without any processing, but skips the first n messages.
    /// </summary>
    /// <typeparam name="TMessage">The type of the consuming and emitting messages/objects.</typeparam>
    public class SkipArrow<TMessage> : AbstractArrow<TMessage, TMessage>
    {

        #region Data

        private readonly UInt32  NumberOfMessagesToSkip;
        private          Int64   Counter;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The SkipArrow simply sends the incoming message to the recipients
        /// without any processing, but skips the first n messages.
        /// </summary>
        /// <param name="NumberOfMessagesToSkip">The number of messages to skip.</param>
        public SkipArrow(UInt32                  NumberOfMessagesToSkip,
                         IArrowSender<TMessage>  ArrowSender = null)

            : base(ArrowSender)

        {
            this.NumberOfMessagesToSkip = NumberOfMessagesToSkip;
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

            var _Counter = Interlocked.Increment(ref Counter);

            if (_Counter > NumberOfMessagesToSkip)
            {
                MessageOut = MessageIn;
                return true;
            }

            MessageOut = default(TMessage);
            return false;

        }

        #endregion

    }

}
