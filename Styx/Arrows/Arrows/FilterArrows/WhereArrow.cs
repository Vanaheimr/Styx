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
    /// Filters the consuming objects by calling a Func&lt;S, Boolean&gt;.
    /// </summary>
    public static class FuncFilterArrowExtensions
    {

        /// <summary>
        /// Filters the consuming objects by calling a Func&lt;S, Boolean&gt;.
        /// </summary>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="FilterFunc">A Func&lt;S, Boolean&gt; filtering the consuming objects. True means filter (ignore).</param>
        public static WhereArrow<TMessage> Where<TMessage>(this IArrowSender<TMessage>  ArrowSender,
                                                           Func<TMessage, Boolean>      Include)
        {
            return new WhereArrow<TMessage>(Include, ArrowSender);
        }

    }

    /// <summary>
    /// Filters the consuming objects by calling a Func&lt;S, Boolean&gt;.
    /// </summary>
    /// <typeparam name="TMessage">The type of the consuming and emitting messages/objects.</typeparam>
    public class WhereArrow<TMessage> : AbstractArrow<TMessage, TMessage>, IFilterArrow<TMessage>
    {

        #region Data

        private readonly Func<TMessage, Boolean> Include;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Filters the consuming objects by calling a Func&lt;S, Boolean&gt;.
        /// </summary>
        /// <param name="FilterFunc">A Func&lt;S, Boolean&gt; filtering the consuming objects. True means filter (ignore).</param>
        public WhereArrow(Func<TMessage, Boolean>  Include,
                          IArrowSender<TMessage>   Source = null)

            : base(Source)

        {

            if (Include == null)
                throw new ArgumentNullException("The given MessageFilter must not be null!");

            this.Include = Include;

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

            if (!Include(MessageIn))
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
