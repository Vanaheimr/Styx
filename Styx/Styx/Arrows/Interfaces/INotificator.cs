/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

    public interface INotificator<T> : IArrowSender<T>
    {
        void SendNotification(T Message);
    }

    public interface INotificator<T1, T2> : IArrowSender<T1, T2>
    {
        void SendNotification(T1 Message1, T2 Message2);
    }

    public interface INotificator<T1, T2, T3> : IArrowSender<T1, T2, T3>
    {
        void SendNotification(T1 Message1, T2 Message2, T3 Message3);
    }

    public interface INotificator<T1, T2, T3, T4> : IArrowSender<T1, T2, T3, T4>
    {
        void SendNotification(T1 Message1, T2 Message2, T3 Message3, T4 Message4);
    }

    public interface INotificator<T1, T2, T3, T4, T5> : IArrowSender<T1, T2, T3, T4, T5>
    {
        void SendNotification(T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5);
    }

    public interface INotificator<T1, T2, T3, T4, T5, T6> : IArrowSender<T1, T2, T3, T4, T5, T6>
    {
        void SendNotification(T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5, T6 Message6);
    }

    public interface INotificator<T1, T2, T3, T4, T5, T6, T7> : IArrowSender<T1, T2, T3, T4, T5, T6, T7>
    {
        void SendNotification(T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5, T6 Message6, T7 Message7);
    }

}
