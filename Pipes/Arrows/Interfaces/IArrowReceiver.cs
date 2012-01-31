﻿/*
 * Copyright (c) 2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Arrows.NET <http://www.github.com/ahzf/Arrows.NET>
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

namespace de.ahzf.Arrows
{

    #region IArrowReceiver<TIn> - Attention: TIn and TOutput reversed ;)

    /// <summary>
    /// The common interface for any Arrow implementations accepting messages of type S.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    public interface IArrowReceiver<in TIn>
    {

        /// <summary>
        /// Accepts a message of type TIn from a sender for further processing
        /// and delivery to the subscribers.
        /// </summary>
        /// <param name="Sender">The sender of the message.</param>
        /// <param name="MessageIn">The message.</param>
        /// <returns>True if the message was accepted and could be processed; False otherwise.</returns>
        Boolean ReceiveMessage(Object Sender, TIn MessageIn);

    }

    #endregion

}
