/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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
using System.Threading.Tasks;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the Task class.
    /// </summary>
    public static class TaskExtensions
    {

        #region CallSideeffect(this Task, Delegate)

        /// <summary>
        /// Call the given side effect delegate and continue to process the given Task afterwards.
        /// </summary>
        /// <typeparam name="T">The type of the Task.</typeparam>
        /// <param name="Task">The type of the result produced by the continuation.</param>
        /// <param name="Action">An action to run when the System.Threading.Tasks.Task completes. When run, the delegate will be passed the completed task as an argument.</param>
        public static Task<T> CallSideeffect<T>(this Task<T> Task, Action<Task<T>> Delegate)
        {

            return Task.ContinueWith(SideEffectTask => {

                Delegate(SideEffectTask);

                return SideEffectTask.Result;

            });

        }

        #endregion

        #region RunOrDefault(this Task, Timeout, Default)

        public static T RunOrDefault<T>(this Task<T> Task, TimeSpan Timeout, T Default)
        {

            Task.Wait(Timeout);

            if (Task.IsCompleted)
                return Task.Result;

            return Default;

        }

        #endregion


    }

}
