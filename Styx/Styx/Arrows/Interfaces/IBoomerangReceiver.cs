/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    #region IBoomerangReceiver<in T1, TResult>

    /// <summary>
    /// The interface for targets accepting
    /// arrows having one message.
    /// </summary>
    public interface IBoomerangReceiver<in T1, TResult> : IArrowReceiver
    {

        /// <summary>
        /// Accept an arrow having one message.
        /// </summary>
        /// <param name="Message1">The first message.</param>
        TResult ProcessBoomerang(T1 Message1);

    }

    #endregion

    #region IBoomerangReceiver<in T1, in T2, TResult>

    /// <summary>
    /// The interface for targets accepting
    /// arrows having two messages.
    /// </summary>
    public interface IBoomerangReceiver<in T1, in T2, TResult> : IArrowReceiver
    {

        /// <summary>
        /// Accept an arrow having two messages.
        /// </summary>
        /// <param name="Message1">The first message.</param>
        /// <param name="Message2">The second message.</param>
        TResult ProcessBoomerang(T1 Message1, T2 Message2);

    }

    #endregion

    #region IBoomerangReceiver<in T1, in T2, in T3, TResult>

    /// <summary>
    /// The interface for targets accepting
    /// arrows having three messages.
    /// </summary>
    public interface IBoomerangReceiver<in T1, in T2, in T3, TResult> : IArrowReceiver
    {

        /// <summary>
        /// Accept an arrow having three messages.
        /// </summary>
        /// <param name="Message1">The first message.</param>
        /// <param name="Message2">The second message.</param>
        /// <param name="Message3">The third message.</param>
        TResult ProcessBoomerang(T1 Message1, T2 Message2, T3 Message3);

    }

    #endregion

    #region IBoomerangReceiver<in T1, in T2, in T3, in T4, TResult>

    /// <summary>
    /// The interface for targets accepting
    /// arrows having four messages.
    /// </summary>
    public interface IBoomerangReceiver<in T1, in T2, in T3, in T4, TResult> : IArrowReceiver
    {

        /// <summary>
        /// Accept an arrow having four messages.
        /// </summary>
        /// <param name="Message1">The first message.</param>
        /// <param name="Message2">The second message.</param>
        /// <param name="Message3">The third message.</param>
        /// <param name="Message4">The fourth message.</param>
        TResult ProcessBoomerang(T1 Message1, T2 Message2, T3 Message3, T4 Message4);

    }

    #endregion

    #region IBoomerangReceiver<in T1, in T2, in T3, in T4, in T5, TResult>

    /// <summary>
    /// The interface for targets accepting
    /// arrows having five messages.
    /// </summary>
    public interface IBoomerangReceiver<in T1, in T2, in T3, in T4, in T5, TResult> : IArrowReceiver
    {

        /// <summary>
        /// Accept an arrow having five messages.
        /// </summary>
        /// <param name="Message1">The first message.</param>
        /// <param name="Message2">The second message.</param>
        /// <param name="Message3">The third message.</param>
        /// <param name="Message4">The fourth message.</param>
        /// <param name="Message4">The fifth message.</param>
        TResult ProcessBoomerang(T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5);

    }

    #endregion

}
