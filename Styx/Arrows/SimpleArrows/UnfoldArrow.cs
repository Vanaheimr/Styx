/*
 * Copyright (c) 2010-2014, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
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

        public event StartedEventHandler OnStarted;

        public event NotificationEventHandler<T> OnNotification;

        public event ExceptionOccuredEventHandler OnExceptionOccured;

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



        public void ProcessStarted(Object Sender, DateTime Timestamp, String Message)
        {

            var OnStartedLocal = OnStarted;

            if (OnStartedLocal != null)
                OnStartedLocal(this, Timestamp, Message);

        }

        public void ProcessArrow(IEnumerable<T> Messages)
        {

            var OnNotificationLocal = OnNotification;

            if (OnNotificationLocal != null)
                foreach (var Message in Messages)
                    OnNotificationLocal(Message);

        }

        public void ProcessExceptionOccured(Object Sender, DateTime Timestamp, Exception ExceptionMessage)
        {

            var OnExceptionOccuredLocal = OnExceptionOccured;

            if (OnExceptionOccuredLocal != null)
                OnExceptionOccuredLocal(this, Timestamp, ExceptionMessage);

        }

        public void ProcessCompleted(Object Sender, DateTime Timestamp, String Message)
        {

            var OnCompletedLocal = OnCompleted;

            if (OnCompleted != null)
                OnCompleted(this, Timestamp, Message);

        }

    }

}
