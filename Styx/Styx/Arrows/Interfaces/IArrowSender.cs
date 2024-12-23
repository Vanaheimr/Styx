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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    #region Delegates

    public delegate void    NotificationEventHandler<T>                          (EventTracking_Id EventTrackingId, T  Message);
    public delegate void    NotificationEventHandler<T1, T2>                     (EventTracking_Id EventTrackingId, T1 Message1, T2 Message2);
    public delegate void    NotificationEventHandler<T1, T2, T3>                 (EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3);
    public delegate void    NotificationEventHandler<T1, T2, T3, T4>             (EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4);
    public delegate void    NotificationEventHandler<T1, T2, T3, T4, T5>         (EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5);
    public delegate void    NotificationEventHandler<T1, T2, T3, T4, T5, T6>     (EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5, T6 Message6);
    public delegate void    NotificationEventHandler<T1, T2, T3, T4, T5, T6, T7> (EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5, T6 Message6, T7 Message7);

    public delegate void    StartedEventHandler                                  (Object Sender, DateTime Timestamp, EventTracking_Id EventTrackingId, String?   Message = null);
    public delegate void    ExceptionOccuredEventHandler                         (Object Sender, DateTime Timestamp, EventTracking_Id EventTrackingId, Exception Exception);
    public delegate void    CompletedEventHandler                                (Object Sender, DateTime Timestamp, EventTracking_Id EventTrackingId, String?   Message = null);

    #endregion


    #region IArrowSender

    /// <summary>
    /// The interface for object providing a notification service.
    /// </summary>
    public interface IArrowSender
    {

        /// <summary>
        /// An event for signaling the start of a message delivery.
        /// </summary>
        event StartedEventHandler           OnStarted;

        /// <summary>
        /// An event for signaling the completion of a message delivery.
        /// </summary>
        event CompletedEventHandler         OnCompleted;

        /// <summary>
        /// An event for signaling an exception.
        /// </summary>
        event ExceptionOccuredEventHandler  OnExceptionOccured;

    }

    #endregion

    #region IArrowSender<T>

    /// <summary>
    /// The interface for object providing a single message notification service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IArrowSender<T> : IArrowSender
    {
        event NotificationEventHandler<T> OnNotification;
    }

    #endregion

    #region IArrowSender<T1, T2>

    public interface IArrowSender<T1, T2> : IArrowSender
    {
        event NotificationEventHandler<T1, T2> OnNotification;
    }

    #endregion

    #region IArrowSender<T1, T2, T3>

    public interface IArrowSender<T1, T2, T3> : IArrowSender
    {
        event NotificationEventHandler<T1, T2, T3> OnNotification;
    }

    #endregion

    #region IArrowSender<T1, T2, T3, T4>

    public interface IArrowSender<T1, T2, T3, T4> : IArrowSender
    {
        event NotificationEventHandler<T1, T2, T3, T4> OnNotification;
    }

    #endregion

    #region IArrowSender<T1, T2, T3, T4, T5>

    public interface IArrowSender<T1, T2, T3, T4, T5> : IArrowSender
    {
        event NotificationEventHandler<T1, T2, T3, T4, T5> OnNotification;
    }

    #endregion

    #region IArrowSender<T1, T2, T3, T4, T5, T6>

    public interface IArrowSender<T1, T2, T3, T4, T5, T6> : IArrowSender
    {
        event NotificationEventHandler<T1, T2, T3, T4, T5, T6> OnNotification;
    }

    #endregion

    #region IArrowSender<T1, T2, T3, T4, T5, T6, T7>

    public interface IArrowSender<T1, T2, T3, T4, T5, T6, T7> : IArrowSender
    {
        event NotificationEventHandler<T1, T2, T3, T4, T5, T6, T7> OnNotification;
    }

    #endregion


}
