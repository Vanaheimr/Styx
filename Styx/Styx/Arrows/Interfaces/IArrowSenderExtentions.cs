/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

    /// <summary>
    /// Extensions for the IArrowSender interface.
    /// </summary>
    public static partial class IArrowSenderExtensions
    {

        public static void SendTo<T>(this IArrowSender  <T> INotification,
                                          IArrowReceiver<T> Target)
        {
            INotification.OnNotification        += Target.ProcessArrow;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

        public static void SendTo<T1, T2>(this IArrowSender  <T1, T2> INotification,
                                               IArrowReceiver<T1, T2> Target)
        {
            INotification.OnNotification        += Target.ProcessArrow;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

        public static void SendTo<T1, T2, T3>(this IArrowSender  <T1, T2, T3> INotification,
                                                   IArrowReceiver<T1, T2, T3> Target)
        {
            INotification.OnNotification        += Target.ProcessArrow;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

        public static void SendTo<T1, T2, T3, T4>(this IArrowSender  <T1, T2, T3, T4> INotification,
                                                       IArrowReceiver<T1, T2, T3, T4> Target)
        {
            INotification.OnNotification        += Target.ProcessArrow;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

        public static void SendTo<T1, T2, T3, T4, T5>(this IArrowSender  <T1, T2, T3, T4, T5> INotification,
                                                           IArrowReceiver<T1, T2, T3, T4, T5> Target)
        {
            INotification.OnNotification        += Target.ProcessArrow;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

    }

}
