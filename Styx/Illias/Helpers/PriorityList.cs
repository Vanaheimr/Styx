/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

using System.Collections;
using System.Collections.Concurrent;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A new priority list.
    /// </summary>
    /// <typeparam name="T">The type of the items within the priority list.</typeparam>
    public class PriorityList<T> : IEnumerable<T>
    {

        #region Data

        private readonly ConcurrentDictionary<UInt32, T> priorityList  = [];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new priority list.
        /// </summary>
        public PriorityList()
        { }

        #endregion


        #region Add(          Service)

        /// <summary>
        /// Add a new service to the priority list.
        /// </summary>
        /// <param name="Service">A service.</param>
        public Boolean Add(T Service)

            => priorityList.TryAdd(
                   !priorityList.IsEmpty
                       ? priorityList.Keys.Max() + 1
                       : 1,
                   Service
               );

        #endregion

        #region Add(Priority, Service)

        /// <summary>
        /// Add a new service to the priority list.
        /// </summary>
        /// <param name="Priority">The priority of the service.</param>
        /// <param name="Service">A service.</param>
        public Boolean Add(UInt32  Priority,
                           T       Service)

            => priorityList.TryAdd(
                   Priority,
                   Service
               );

        #endregion


        #region WhenAll(Work)

        /// <summary>
        /// Run every service in priority order and wait until all services are done.
        /// </summary>
        /// <typeparam name="T2">The type of the work results.</typeparam>
        /// <param name="Work">A work to do.</param>
        public Task<T2[]> WhenAll<T2>(Func<T, CancellationToken, Task<T2>>  Work,
                                      CancellationToken                     ExternalCancellation = default)
        {

            if (Work is null)
                return Task.FromResult(Array.Empty<T2>());

            return Task.WhenAll(
                       priorityList.
                           OrderBy(element => element.Key).
                           Select (element => Work(element.Value, ExternalCancellation)
                       )
                   );

        }

        #endregion


        #region (private) ExecuteWithCancellation(Element, Work, ExceptionHandler, CancellationToken)

        private static async Task<T2> ExecuteWithCancellation<T2>(T                                     Element,
                                                                  Func<T, CancellationToken, Task<T2>>  Work,
                                                                  Action<Exception>?                    ExceptionHandler,
                                                                  CancellationToken                     CancellationToken)
        {
            try
            {

                return await Work(
                                 Element,
                                 CancellationToken
                             ).ConfigureAwait(false);

            }
            catch (OperationCanceledException) when (CancellationToken.IsCancellationRequested)
            {
                // Expected, as another service was faster!
                return default!;
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
                return default!;
            }
        }

        #endregion

        #region WhenFirst(Work, VerifyResult, Timeout, OnException, DefaultResult, ExternalCancellation)

        /// <summary>
        /// Run every service in priority order and return the first valid result
        /// or a default result after a timeout.
        /// </summary>
        /// <typeparam name="T2">The type of the work results.</typeparam>
        /// <param name="Work">A work to do.</param>
        /// <param name="VerifyResult">A delegate to verify and filter results.</param>
        /// <param name="Timeout">A timeout.</param>
        /// <param name="DefaultResult">A default result in case of errors or a timeout.</param>
        /// <param name="ExternalCancellation">An external cancellation token.</param>
        public async Task<T2> WhenFirst<T2>(Func<T, CancellationToken, Task<T2>>  Work,
                                            Func<T2, Boolean>                     VerifyResult,
                                            TimeSpan                              Timeout,
                                            Action<Exception>?                    ExceptionHandler,
                                            Func<TimeSpan, T2>                    DefaultResult,
                                            CancellationToken                     ExternalCancellation = default)

        {

            var startTime          = Timestamp.Now;

            using var timeoutCts   = new CancellationTokenSource(Timeout);
            using var linkedCts    = CancellationTokenSource.CreateLinkedTokenSource(
                                         ExternalCancellation,
                                         timeoutCts.Token
                                     );

            var cancellationToken  = linkedCts.Token;


            var allTasks           = priorityList.
                                         OrderBy(element => element.Key).
                                         Select (element => ExecuteWithCancellation(
                                                                element.Value,
                                                                Work,
                                                                ExceptionHandler,
                                                                cancellationToken
                                                            )).
                                         ToList();

            while (allTasks.Count > 0)
            {

                var completedTask = await Task.WhenAny(allTasks).ConfigureAwait(false);
                allTasks.Remove(completedTask);

                if (completedTask.IsCompletedSuccessfully)
                {

                    var resultValue = await completedTask.ConfigureAwait(false);

                    if (!EqualityComparer<T2>.Default.Equals(resultValue, default) &&
                        VerifyResult(resultValue))
                    {
                        // Cancel all other tasks!
                        linkedCts.Cancel();
                        return resultValue;
                    }

                }
                else if (completedTask.IsFaulted && ExceptionHandler is not null)
                {
                    foreach (var ex in completedTask.Exception?.InnerExceptions ?? [])
                        ExceptionHandler(ex);
                }

                // Timeout reached?
                if (cancellationToken.IsCancellationRequested)
                    break;

            }

            return DefaultResult(Timestamp.Now - startTime);

        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator iterating through the priority list.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()

            => priorityList.
                   OrderBy      (element => element.Key).
                   Select       (element => element.Value).
                   ToList       ().
                   GetEnumerator();

        /// <summary>
        /// Returns an enumerator iterating through the priority list.
        /// </summary>
        public IEnumerator<T> GetEnumerator()

            => priorityList.
                   OrderBy      (element => element.Key).
                   Select       (element => element.Value).
                   ToList       ().
                   GetEnumerator();

        #endregion

    }

}
