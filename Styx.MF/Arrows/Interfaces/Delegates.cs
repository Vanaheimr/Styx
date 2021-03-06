﻿/*
 * Copyright (c) 2011-2012, Achim 'ahzf' Friedland <achim@graph-database.org>
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

namespace de.ahzf.Styx
{

    /// <summary>
    /// A delegate for delivering messages.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message/object.</typeparam>
    /// <param name="Sender">The sender of the message.</param>
    /// <param name="Message">The message.</param>
    /// <returns>True if the message was accepted and could be processed; False otherwise.</returns>
    public delegate Boolean MessageRecipient(Object Sender, Object Message);

    /// <summary>
    /// A delegate for signaling the completion of a message delivery.
    /// </summary>
    /// <param name="Sender">The sender of the completion signal.</param>
    /// <returns>True if the completion message was accepted; False otherwise.</returns>
    public delegate void CompletionRecipient(Object Sender);

    /// <summary>
    /// A delegate for signaling an exception.
    /// </summary>
    /// <param name="Sender">The sender of the message.</param>
    /// <param name="Exception">An exception.</param>
    public delegate void ExceptionRecipient(Object Sender, Exception Exception);

}
