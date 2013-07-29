﻿/*
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

#endregion

namespace eu.Vanaheimr.Styx
{

    /// <summary>
    /// The IdentityArrow is the most basic arrow.
    /// It simply sends the incoming message to the recipients without any processing.
    /// This arrow is useful in various test case situations.
    /// </summary>
    /// <typeparam name="TMessage">The type of the consuming and emitting messages/objects.</typeparam>
    public class IdentityArrow<TMessage> : AbstractArrow<TMessage, TMessage>
    {

        #region Constructor(s)

        #region IdentityArrow()

        /// <summary>
        /// The IdentityArrow is the most basic arrow.
        /// It simply sends the incoming message to the recipients without any processing.
        /// This arrow is useful in various test case situations.
        /// </summary>
        public IdentityArrow()
        { }

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
            return true;
        }

        #endregion

    }

}
