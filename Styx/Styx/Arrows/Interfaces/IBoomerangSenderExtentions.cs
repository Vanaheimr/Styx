/*
 * Copyright (c) 2010-2019 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// Extentions for the IBoomerangSender interface.
    /// </summary>
    public static partial class IBoomerangSenderExtentions
    {

        public static void ConnectTo<T, TResult>(this IBoomerangSender  <T, TResult> INotification,
                                                      IBoomerangReceiver<T, TResult> Target)
        {
            INotification.OnNotification        += Target.ProcessBoomerang;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

        public static void ConnectTo<T1, T2, TResult>(this IBoomerangSender  <T1, T2, TResult> INotification,
                                                           IBoomerangReceiver<T1, T2, TResult> Target)
        {
            INotification.OnNotification        += Target.ProcessBoomerang;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

        public static void ConnectTo<T1, T2, T3, TResult>(this IBoomerangSender  <T1, T2, T3, TResult> INotification,
                                                               IBoomerangReceiver<T1, T2, T3, TResult> Target)
        {
            INotification.OnNotification        += Target.ProcessBoomerang;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

        public static void ConnectTo<T1, T2, T3, T4, TResult>(this IBoomerangSender  <T1, T2, T3, T4, TResult> INotification,
                                                                   IBoomerangReceiver<T1, T2, T3, T4, TResult> Target)
        {
            INotification.OnNotification        += Target.ProcessBoomerang;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

        public static void ConnectTo<T1, T2, T3, T4, T5, TResult>(this IBoomerangSender  <T1, T2, T3, T4, T5, TResult> INotification,
                                                                       IBoomerangReceiver<T1, T2, T3, T4, T5, TResult> Target)
        {
            INotification.OnNotification        += Target.ProcessBoomerang;
            INotification.OnExceptionOccured    += Target.ProcessExceptionOccured;
            INotification.OnCompleted           += Target.ProcessCompleted;
        }

    }

}
