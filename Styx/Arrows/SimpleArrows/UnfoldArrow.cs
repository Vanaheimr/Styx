/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Hermod <http://www.github.com/Vanaheimr/Hermod>
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

    public static class UnfoldArrowExtentions
    {

        /// <summary>
        /// Turn a single arrow having multiple notifications into
        /// multiple arrows having a single notification each.
        /// </summary>
        /// <typeparam name="T">The type of the notifications.</typeparam>
        /// <param name="In">The arrow sender.</param>
        public static UnfoldArrow<T> Unfold<T>(this IArrowSender<IEnumerable<T>> In)
        {
            return new UnfoldArrow<T>(In);
        }

    }

    /// <summary>
    /// Turn a single arrow having multiple notifications into
    /// multiple arrows having a single notification each.
    /// </summary>
    /// <typeparam name="T">The type of the notifications.</typeparam>
    public class UnfoldArrow<T> : IArrowReceiver<IEnumerable<T>>, IArrowSender<T>
    {

        #region Events

        public event NotificationEventHandler<T> OnNotification;

        public event ExceptionEventHandler OnException;

        public event CompletedEventHandler OnCompleted;

        #endregion

        #region Constructor(s)

        #region SplitterArrow()

        public UnfoldArrow(IArrowSender<IEnumerable<T>> In = null)
        {
            if (In != null)
                In.SendTo(this);
        }

        #endregion

        #endregion


        public void ProcessArrow(IEnumerable<T> Messages)
        {

            foreach (var Message in Messages)
            {
                if (OnNotification != null)
                    OnNotification(Message);
            }

        }

        public void ProcessException(Object Sender, Exception ExceptionMessage)
        {
            if (OnException != null)
                OnException(this, ExceptionMessage);
        }

        public void ProcessCompleted(Object Sender, String Message)
        {
            if (OnCompleted != null)
                OnCompleted(this, Message);
        }

    }

}
