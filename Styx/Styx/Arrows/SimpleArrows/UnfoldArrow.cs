/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Styx <https://www.github.com/Vanaheimr/Styx>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    public static class UnfoldArrowExtensions
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

        public event StartedEventHandler?           OnStarted;

        public event NotificationEventHandler<T>?   OnNotification;

        public event ExceptionOccurredEventHandler?  OnExceptionOccurred;

        public event CompletedEventHandler?         OnCompleted;

        #endregion

        #region Constructor(s)

        public UnfoldArrow(IArrowSender<IEnumerable<T>>? In = null)
        {
            In?.SendTo(this);
        }

        #endregion


        public void ProcessStarted(Object Sender, DateTimeOffset Timestamp, EventTracking_Id EventTrackingId, String Message)
        {
            OnStarted?.Invoke(this, Timestamp, EventTrackingId, Message);
        }

        public void ProcessArrow(EventTracking_Id EventTrackingId, IEnumerable<T> Messages)
        {
            foreach (var Message in Messages)
                OnNotification?.Invoke(EventTrackingId, Message);
        }

        public void ProcessExceptionOccurred(Object Sender, DateTimeOffset Timestamp, EventTracking_Id EventTrackingId, Exception ExceptionMessage)
        {
            OnExceptionOccurred?.Invoke(this, Timestamp, EventTrackingId, ExceptionMessage);
        }

        public void ProcessCompleted(Object Sender, DateTimeOffset Timestamp, EventTracking_Id EventTrackingId, String? Message = null)
        {
            OnCompleted?.Invoke(this, Timestamp, EventTrackingId, Message);
        }

    }

}
