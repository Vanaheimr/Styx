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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// The SameValueFilterArrow will not allow to send two
    /// consecutive identical messages twice.
    /// </summary>
    /// <typeparam name="TMessage">The type of the consuming and emitting messages/objects.</typeparam>
    public class SameValueFilterArrow<TMessage> : AbstractArrow<TMessage, TMessage>,
                                                  IFilterArrow<TMessage>

        where TMessage : IEquatable<TMessage>

    {

        #region Properties

        /// <summary>
        /// The last message send.
        /// </summary>
        public TMessage LastMessage { get; private set; }

        #endregion

        #region Constructor(s)

        #region SameValueFilterArrow()

        /// <summary>
        /// The SameValueFilterArrow will not allow to send two
        /// consecutive identical messages twice.
        /// </summary>
        public SameValueFilterArrow()
        { }

        #endregion

        #region SameValueFilterArrow(LastMessage)

        /// <summary>
        /// The SameValueFilterArrow will not allow to send two
        /// consecutive identical messages twice.
        /// </summary>
        /// <param name="LastMessage">Sets the initial value of LastMessage.</param>
        public SameValueFilterArrow(TMessage LastMessage)
        {
            this.LastMessage = LastMessage;
        }

        #endregion

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

            if (MessageIn.Equals(LastMessage))
            {
                LastMessage = MessageIn;
                return false;
            }

            LastMessage = MessageIn;

            return true;

        }

        #endregion

    }

}
