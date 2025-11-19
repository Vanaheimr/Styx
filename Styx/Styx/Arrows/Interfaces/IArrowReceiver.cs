/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

    #region IArrowReceiver

    /// <summary>
    /// The general interface for everything which
    /// acts as an arrow target.
    /// </summary>
    public interface IArrowReceiver
    {

        /// <summary>
        /// An exception occurred at the arrow sender.
        /// </summary>
        /// <param name="Sender">The sender of this exception.</param>
        /// <param name="ExceptionMessage">The timestamp of the exception.</param>
        /// <param name="Exception">The occurred exception.</param>
        void ProcessExceptionOccurred(Object Sender, DateTimeOffset Timestamp, EventTracking_Id EventTrackingId, Exception ExceptionMessage);

        /// <summary>
        /// The sender of the arrows signalled not to send any more arrows.
        /// </summary>
        /// <param name="Sender">The sender of this arrow.</param>
        /// <param name="Timestamp">The timestamp of the shutdown.</param>
        /// <param name="Message">An optional message.</param>
        void ProcessCompleted(Object Sender, DateTimeOffset Timestamp, EventTracking_Id EventTrackingId, String? Message = null);

    }

    #endregion

    #region IArrowReceiver<in T>

    /// <summary>
    /// The interface for arrow targets accepting a single message.
    /// </summary>
    public interface IArrowReceiver<in T> : IArrowReceiver
    {

        /// <summary>
        /// Accept a single message.
        /// </summary>
        /// <param name="Message">The message.</param>
        void ProcessArrow(EventTracking_Id EventTrackingId, T Message);

    }

    #endregion

    #region IArrowReceiver<in T1, in T2>

    /// <summary>
    /// The interface for targets accepting
    /// arrows having two messages.
    /// </summary>
    public interface IArrowReceiver<in T1, in T2> : IArrowReceiver
    {

        /// <summary>
        /// Accept an arrow having two messages.
        /// </summary>
        /// <param name="Message1">The first message.</param>
        /// <param name="Message2">The second message.</param>
        void ProcessArrow(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2);

    }

    #endregion

    #region IArrowReceiver<in T1, in T2, in T3>

    /// <summary>
    /// The interface for targets accepting
    /// arrows having three messages.
    /// </summary>
    public interface IArrowReceiver<in T1, in T2, in T3> : IArrowReceiver
    {

        /// <summary>
        /// Accept an arrow having three messages.
        /// </summary>
        /// <param name="Message1">The first message.</param>
        /// <param name="Message2">The second message.</param>
        /// <param name="Message3">The third message.</param>
        void ProcessArrow(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3);

    }

    #endregion

    #region IArrowReceiver<in T1, in T2, in T3, in T4>

    /// <summary>
    /// The interface for targets accepting
    /// arrows having four messages.
    /// </summary>
    public interface IArrowReceiver<in T1, in T2, in T3, in T4> : IArrowReceiver
    {

        /// <summary>
        /// Accept an arrow having four messages.
        /// </summary>
        /// <param name="Message1">The first message.</param>
        /// <param name="Message2">The second message.</param>
        /// <param name="Message3">The third message.</param>
        /// <param name="Message4">The fourth message.</param>
        void ProcessArrow(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4);

    }

    #endregion

    #region IArrowReceiver<in T1, in T2, in T3, in T4, in T5>

    /// <summary>
    /// The interface for targets accepting
    /// arrows having five messages.
    /// </summary>
    public interface IArrowReceiver<in T1, in T2, in T3, in T4, in T5> : IArrowReceiver
    {

        /// <summary>
        /// Accept an arrow having five messages.
        /// </summary>
        /// <param name="Message1">The first message.</param>
        /// <param name="Message2">The second message.</param>
        /// <param name="Message3">The third message.</param>
        /// <param name="Message4">The fourth message.</param>
        /// <param name="Message5">The fifth message.</param>
        void ProcessArrow(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5);

    }

    #endregion

}
