/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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

using System;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// Extension methods for the MapArrow.
    /// </summary>
    public static class MapArrowExtensions
    {

        #region Map<TIn, TOut>

        /// <summary>
        /// Transform the message of every incoming arrow by the given delegate.
        /// </summary>
        /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
        /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="MessageProcessor">A delegate to transform an incoming message into an outgoing message.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        public static MapArrow<TIn, TOut> Map<TIn, TOut>(this IArrowSender<TIn>      ArrowSender,
                                                         Func<TIn, TOut>             MessageProcessor,
                                                         Func<Exception, Exception>  OnError = null)
        {
            return new MapArrow<TIn, TOut>(MessageProcessor, OnError, ArrowSender);
        }

        #endregion

        #region Map<TIn1, TIn2, TOut>

        /// <summary>
        /// Transform both messages of every incoming arrow by the given delegate.
        /// </summary>
        /// <typeparam name="TIn1">The first type of the consuming messages/objects.</typeparam>
        /// <typeparam name="TIn2">The second type of the consuming messages/objects.</typeparam>
        /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="MessageProcessor">A delegate to transform two incoming messages into an outgoing message.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        public static MapArrow<TIn1, TIn2, TOut> Map<TIn1, TIn2, TOut>(this IArrowSender<TIn1, TIn2>  ArrowSender,
                                                                       Func<TIn1, TIn2, TOut>         MessageProcessor,
                                                                       Func<Exception, Exception>     OnError = null)
        {
            return new MapArrow<TIn1, TIn2, TOut>(MessageProcessor, OnError, ArrowSender);
        }

        #endregion

        #region Map<TIn1, TIn2, TIn3, TOut>

        /// <summary>
        /// Transform three messages of every incoming arrow by the given delegate.
        /// </summary>
        /// <typeparam name="TIn1">The first type of the consuming messages/objects.</typeparam>
        /// <typeparam name="TIn2">The second type of the consuming messages/objects.</typeparam>
        /// <typeparam name="TIn3">The third type of the consuming messages/objects.</typeparam>
        /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        /// <param name="MessageProcessor">A delegate to transform two incoming messages into an outgoing message.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        public static MapArrow<TIn1, TIn2, TIn3, TOut> Map<TIn1, TIn2, TIn3, TOut>(this IArrowSender<TIn1, TIn2, TIn3>  ArrowSender,
                                                                                   Func<TIn1, TIn2, TIn3, TOut>         MessageProcessor,
                                                                                   Func<Exception, Exception>           OnError = null)
        {
            return new MapArrow<TIn1, TIn2, TIn3, TOut>(MessageProcessor, OnError, ArrowSender);
        }

        #endregion

    }

    #region MapArrow<TIn, TOut>

    /// <summary>
    /// Transform the message of every incoming arrow by the given delegate.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class MapArrow<TIn, TOut> : AbstractArrow<TIn, TOut>
    {

        #region Data

        private readonly Func<TIn,       TOut>       MessageProcessor;
        private readonly Func<Exception, Exception>  OnError;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new MapArrow transforming the message of every
        /// incoming arrow by the given delegate.
        /// </summary>
        /// <param name="MessageProcessor">A delegate to transform an incoming message into an outgoing message.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public MapArrow(Func<TIn, TOut>             MessageProcessor,
                        Func<Exception, Exception>  OnError      = null,
                        IArrowSender<TIn>           ArrowSender  = null)

            : base(ArrowSender)

        {

            if (MessageProcessor == null)
                throw new ArgumentNullException("The given delegate must not be null!");

            this.MessageProcessor  = MessageProcessor;
            this.OnError           = (OnError != null) ? OnError : e => e;

        }

        #endregion

        #region (protected) ProcessMessage(MessageIn, out MessageOut)

        /// <summary>
        /// Process the incoming message and return an outgoing message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        /// <param name="MessageOut">The outgoing message.</param>
        protected override Boolean ProcessMessage(TIn MessageIn, out TOut MessageOut)
        {
            // Try-Catch will be done within AbstractArrow.ProcessArrow(MessageIn)
            MessageOut = MessageProcessor(MessageIn);
            return true;
        }

        #endregion

    }

    #endregion

    #region MapArrow<TIn1, TIn2, TOut>

    /// <summary>
    /// Transform both messages of every incoming arrow by the given delegate.
    /// </summary>
    /// <typeparam name="TIn1">The first type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TIn2">The second type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class MapArrow<TIn1, TIn2, TOut> : IArrowReceiver<TIn1, TIn2>, IArrowSender<TOut>
    {

        #region Data

        private readonly Func<TIn1, TIn2, TOut>      MessageProcessor;
        private readonly Func<Exception, Exception>  OnError;

        #endregion

        #region Events

        public event StartedEventHandler OnStarted;

        public event NotificationEventHandler<TOut> OnNotification;

        public event ExceptionOccuredEventHandler OnExceptionOccured;

        public event CompletedEventHandler OnCompleted;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new MapArrow transforming two messages of
        /// every incoming arrow by the given delegate.
        /// </summary>
        /// <param name="MessageProcessor">A delegate to transform two incoming messages into an outgoing message.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public MapArrow(Func<TIn1, TIn2, TOut>      MessageProcessor,
                        Func<Exception, Exception>  OnError      = null,
                        IArrowSender<TIn1, TIn2>    ArrowSender  = null)

        {

            if (MessageProcessor == null)
                throw new ArgumentNullException("The given delegate must not be null!");

            this.MessageProcessor  = MessageProcessor;
            this.OnError           = (OnError != null) ? OnError : e => e;

            if (ArrowSender != null)
                ArrowSender.SendTo(this);

        }

        #endregion


        public void ProcessStarted(Object Sender, DateTime Timestamp, String Message)
        {

            var OnStartedLocal = OnStarted;

            if (OnStartedLocal != null)
                OnStartedLocal(this, Timestamp, Message);

        }

        #region ProcessArrow(MessageIn1, MessageIn2)

        /// <summary>
        /// Process the incoming messages and send an outgoing message.
        /// </summary>
        /// <param name="MessageIn1">The first incoming message.</param>
        /// <param name="MessageIn2">The second incoming message.</param>
        public void ProcessArrow(TIn1 MessageIn1, TIn2 MessageIn2)
        {

            if (OnNotification != null)
                OnNotification(MessageProcessor(MessageIn1, MessageIn2));

        }

        #endregion

        public void ProcessExceptionOccured(Object Sender, DateTime Timestamp, Exception ExceptionMessage)
        {

            var OnExceptionOccuredLocal = OnExceptionOccured;

            if (OnExceptionOccuredLocal != null)
                OnExceptionOccuredLocal(this, Timestamp, ExceptionMessage);

        }

        public void ProcessCompleted(Object Sender, DateTime Timestamp, String Message)
        {

            var OnCompletedLocal = OnCompleted;

            if (OnCompletedLocal != null)
                OnCompletedLocal(this, Timestamp, Message);

        }

    }

    #endregion

    #region MapArrow<TIn1, TIn2, TIn3, TOut>

    /// <summary>
    /// Transform three messages of every incoming arrow by the given delegate.
    /// </summary>
    /// <typeparam name="TIn1">The first type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TIn2">The second type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TIn3">The third type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class MapArrow<TIn1, TIn2, TIn3, TOut> : IArrowReceiver<TIn1, TIn2, TIn3>, IArrowSender<TOut>
    {

        #region Data

        private readonly Func<TIn1, TIn2, TIn3, TOut>  MessageProcessor;
        private readonly Func<Exception, Exception>    OnError;

        #endregion

        #region Events

        public event StartedEventHandler OnStarted;

        public event NotificationEventHandler<TOut> OnNotification;

        public event ExceptionOccuredEventHandler OnExceptionOccured;

        public event CompletedEventHandler OnCompleted;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new MapArrow transforming three messages of
        /// every incoming arrow by the given delegate.
        /// </summary>
        /// <param name="MessageProcessor">A delegate to transform three incoming messages into an outgoing message.</param>
        /// <param name="OnError">A delegate to transform an incoming error into an outgoing error.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public MapArrow(Func<TIn1, TIn2, TIn3, TOut>    MessageProcessor,
                        Func<Exception, Exception>      OnError      = null,
                        IArrowSender<TIn1, TIn2, TIn3>  ArrowSender  = null)

        {

            if (MessageProcessor == null)
                throw new ArgumentNullException("The given delegate must not be null!");

            this.MessageProcessor  = MessageProcessor;
            this.OnError           = (OnError != null) ? OnError : e => e;

            if (ArrowSender != null)
                ArrowSender.SendTo(this);

        }

        #endregion


        public void ProcessStarted(Object Sender, DateTime Timestamp, String Message)
        {

            var OnStartedLocal = OnStarted;

            if (OnStartedLocal != null)
                OnStartedLocal(this, Timestamp, Message);

        }

        #region ProcessArrow(MessageIn1, MessageIn2, MessageIn3)

        /// <summary>
        /// Process the incoming messages and send an outgoing message.
        /// </summary>
        /// <param name="MessageIn1">The first incoming message.</param>
        /// <param name="MessageIn2">The second incoming message.</param>
        /// <param name="MessageIn3">The third incoming message.</param>
        public void ProcessArrow(TIn1 MessageIn1, TIn2 MessageIn2, TIn3 MessageIn3)
        {

            if (OnNotification != null)
                OnNotification(MessageProcessor(MessageIn1, MessageIn2, MessageIn3));

        }

        #endregion

        public void ProcessExceptionOccured(Object Sender, DateTime Timestamp, Exception ExceptionMessage)
        {

            var OnExceptionOccuredLocal = OnExceptionOccured;

            if (OnExceptionOccuredLocal != null)
                OnExceptionOccuredLocal(this, Timestamp, ExceptionMessage);

        }

        public void ProcessCompleted(Object Sender, DateTime Timestamp, String Message)
        {

            var OnCompletedLocal = OnCompleted;

            if (OnCompletedLocal != null)
                OnCompletedLocal(this, Timestamp, Message);

        }

    }

    #endregion

}
