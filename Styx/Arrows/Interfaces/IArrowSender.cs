/*
 * Copyright (c) 2011-2012, Achim 'ahzf' Friedland <code@ahzf.de>
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

namespace de.ahzf.Styx
{

    #region IArrowSender

    public interface IArrowSender
    {

        /// <summary>
        /// An event for signaling the completion of a message delivery.
        /// </summary>
        event CompletionRecipient OnCompleted;

        /// <summary>
        /// An event for signaling an exception.
        /// </summary>
        event ExceptionRecipient OnError;

    }

    #endregion

    #region IArrowSender<TOut>

    // Attention: TIn and TOut reversed ;)

    /// <summary>
    /// The common interface for any Arrow implementation sending messages of type TOut.
    /// </summary>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public interface IArrowSender<TOut> : IArrowSender
    {

        /// <summary>
        /// An event for message delivery.
        /// </summary>
        event MessageRecipient<TOut> OnMessageAvailable;

        /// <summary>
        /// Sends messages/objects from this Arrow to the given recipients.
        /// </summary>
        /// <param name="Recipients">The recipients of the processed messages.</param>
        void SendTo(params MessageRecipient<TOut>[] Recipients);

        /// <summary>
        /// Sends messages/objects from this Arrow to the given recipients.
        /// </summary>
        /// <param name="Recipients">The recipients of the processed messages.</param>
        void SendTo(params IArrowReceiver<TOut>[] Recipients);

    }

    #endregion

}
