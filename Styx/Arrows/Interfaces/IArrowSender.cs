/*
 * Copyright (c) 2011-2013, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using eu.Vanaheimr.Illias.Commons.Votes;

#endregion

namespace eu.Vanaheimr.Styx.Arrows
{

    public delegate void NotificationEventHandler<T>(T Message);
    public delegate void NotificationEventHandler<T1, T2>(T1 Message1, T2 Message2);
    public delegate void NotificationEventHandler<T1, T2, T3>(T1 Message1, T2 Message2, T3 Message3);
    public delegate void NotificationEventHandler<T1, T2, T3, T4>(T1 Message1, T2 Message2, T3 Message3, T4 Message4);
    public delegate void NotificationEventHandler<T1, T2, T3, T4, T5>(T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5);

    public delegate void ExceptionEventHandler(Object Sender, Exception Exception);
    public delegate void CompletedEventHandler(Object Sender, String    Message = null);


    #region IArrowSenderExtentions

    /// <summary>
    /// Extentions for the IArrowSender interface.
    /// </summary>
    public static class IArrowSenderExtentions
    {

        public static void SendTo<T>(this IArrowSender<T> INotification, IArrowReceiver<T> Target)
        {
            INotification.OnNotification += Target.ProcessArrow;
            INotification.OnException        += Target.ProcessException;
            INotification.OnCompleted    += Target.ProcessCompleted;
        }

        public static void SendTo<T1, T2>(this IArrowSender<T1, T2> INotification, IArrowReceiver<T1, T2> Target)
        {
            INotification.OnNotification += Target.ProcessArrow;
            INotification.OnException        += Target.ProcessException;
            INotification.OnCompleted    += Target.ProcessCompleted;
        }

        public static void SendTo<T1, T2, T3>(this IArrowSender<T1, T2, T3> INotification, IArrowReceiver<T1, T2, T3> Target)
        {
            INotification.OnNotification += Target.ProcessArrow;
            INotification.OnException        += Target.ProcessException;
            INotification.OnCompleted    += Target.ProcessCompleted;
        }

        public static void SendTo<T1, T2, T3, T4>(this IArrowSender<T1, T2, T3, T4> INotification, IArrowReceiver<T1, T2, T3, T4> Target)
        {
            INotification.OnNotification += Target.ProcessArrow;
            INotification.OnException        += Target.ProcessException;
            INotification.OnCompleted    += Target.ProcessCompleted;
        }

        public static void SendTo<T1, T2, T3, T4, T5>(this IArrowSender<T1, T2, T3, T4, T5> INotification, IArrowReceiver<T1, T2, T3, T4, T5> Target)
        {
            INotification.OnNotification += Target.ProcessArrow;
            INotification.OnException        += Target.ProcessException;
            INotification.OnCompleted    += Target.ProcessCompleted;
        }

    }

    #endregion

    #region IArrowSender

    /// <summary>
    /// The interface for object providing a notification service.
    /// </summary>
    public interface IArrowSender
    {

        /// <summary>
        /// An event for signaling the completion of a message delivery.
        /// </summary>
        event CompletedEventHandler OnCompleted;

        /// <summary>
        /// An event for signaling an exception.
        /// </summary>
        event ExceptionEventHandler OnException;

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

}
