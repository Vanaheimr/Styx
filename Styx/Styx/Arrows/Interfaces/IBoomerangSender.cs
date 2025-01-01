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

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    #region Delegates

    public delegate TResult BoomerangSenderHandler<T1,                 TResult> (T1 Message1);
    public delegate TResult BoomerangSenderHandler<T1, T2,             TResult> (T1 Message1, T2 Message2);
    public delegate TResult BoomerangSenderHandler<T1, T2, T3,         TResult> (T1 Message1, T2 Message2, T3 Message3);
    public delegate TResult BoomerangSenderHandler<T1, T2, T3, T4,     TResult> (T1 Message1, T2 Message2, T3 Message3, T4 Message4);
    public delegate TResult BoomerangSenderHandler<T1, T2, T3, T4, T5, TResult> (T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5);

    #endregion


    #region IBoomerangSender<T1, TResult>

    public interface IBoomerangSender<T1, TResult> : IArrowSender
    {
        event BoomerangSenderHandler<T1, TResult> OnNotification;
    }

    #endregion

    #region IBoomerangSender<T1, T2, TResult>

    public interface IBoomerangSender<T1, T2, TResult> : IArrowSender
    {
        event BoomerangSenderHandler<T1, T2, TResult> OnNotification;
    }

    #endregion

    #region IBoomerangSender<T1, T2, T3, TResult>

    public interface IBoomerangSender<T1, T2, T3, TResult> : IArrowSender
    {
        event BoomerangSenderHandler<T1, T2, T3, TResult> OnNotification;
    }

    #endregion

    #region IBoomerangSender<T1, T2, T3, T4, TResult>

    public interface IBoomerangSender<T1, T2, T3, T4, TResult> : IArrowSender
    {
        event BoomerangSenderHandler<T1, T2, T3, T4, TResult> OnNotification;
    }

    #endregion

    #region IBoomerangSender<T1, T2, T3, T4, T5, TResult>

    public interface IBoomerangSender<T1, T2, T3, T4, T5, TResult> : IArrowSender
    {
        event BoomerangSenderHandler<T1, T2, T3, T4, T5, TResult> OnNotification;
    }

    #endregion

}
