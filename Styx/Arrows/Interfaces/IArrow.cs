﻿/*
 * Copyright (c) 2011-2012, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Styx <http://www.github.com/ahzf/Styx>
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

namespace de.ahzf.Styx
{

    #region IArrow

    /// <summary>
    /// The common interface for any Arrow implementation.
    /// </summary>
    public interface IArrow : IDisposable
    {

        /// <summary>
        /// Turns the recording of the message delivery path ON or OFF.
        /// </summary>
        Boolean RecordMessagePath { get; set; }

        /// <summary>
        /// Returns the message path.
        /// </summary>
        IEnumerable<Object> Path { get; }

        /// <summary>
        /// Signal the completion of the message delivery.
        /// </summary>
        /// <param name="Sender">The sender of the completion signal.</param>
        void Complete(Object Sender);

    }

    #endregion

    #region IArrow<TIn, TOut>

    /// <summary>
    /// The generic interface for any Arrow implementation.
    /// An Arrow accepts/consumes messages/objects of type S and emits messages/objects
    /// of type E via an event.
    /// </summary>
    /// <typeparam name="TIn">The type of the consuming messages/objects.</typeparam>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public interface IArrow<in TIn, TOut> : IArrowSender<TOut>, IArrowReceiver<TIn>, IArrow
    { }

    #endregion

}
