/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

        private readonly Dictionary<UInt32, T>  priorityList;
        private T[]                             sortedList;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new priority list.
        /// </summary>
        public PriorityList()
        {

            this.priorityList  = new Dictionary<UInt32, T>();
            this.sortedList    = Array.Empty<T>();

        }

        #endregion


        #region Add(Service)

        /// <summary>
        /// Add a new service to the priority list.
        /// </summary>
        /// <param name="Service">A service.</param>
        public void Add(T Service)
        {
            lock (priorityList)
            {

                priorityList.Add(priorityList.Count > 0
                                    ? priorityList.Keys.Max() + 1
                                    : 1,
                                  Service);

                sortedList = priorityList.
                                  OrderBy(kvp => kvp.Key).
                                  Select (kvp => kvp.Value).
                                  ToArray();

            }
        }

        #endregion


        #region WhenAll(Work)

        /// <summary>
        /// Run every service in priority order and wait until all services are done.
        /// </summary>
        /// <typeparam name="T2">The type of the work results.</typeparam>
        /// <param name="Work">A work to do.</param>
        public Task<T2[]> WhenAll<T2>(Func<T, Task<T2>> Work)
        {

            if (Work is null)
                return Task.FromResult(Array.Empty<T2>());

            return Task.WhenAll(sortedList.Select(service => Work(service)));

        }

        #endregion

        #region WhenFirst(Work, VerifyResult, Timeout, OnException, DefaultResult)

        /// <summary>
        /// Run every service in priority order and wait until all services are done.
        /// </summary>
        /// <typeparam name="T2">The type of the work results.</typeparam>
        /// <param name="Work">A work to do.</param>
        /// <param name="VerifyResult">A delegate to verify and filter results.</param>
        /// <param name="Timeout">A timeout.</param>
        /// <param name="DefaultResult">A default result in case of errors or a timeout.</param>
        public async Task<T2> WhenFirst<T2>(Func<T, Task<T2>>   Work,
                                            Func<T2, Boolean>   VerifyResult,
                                            TimeSpan            Timeout,
                                            Action<Exception>?  OnException,
                                            Func<TimeSpan, T2>  DefaultResult)

        {

            #region Initial checks

            Task<T2> workDone;
            Task<T2> result;

            #endregion

            var startTime    = Timestamp.Now;

            var toDoList     = sortedList.
                                   Select(service => Work(service)).
                                   ToList();

            var timeoutTask  = Task.Run(() => {
                                                  Thread.Sleep(Timeout);
                                                  return DefaultResult(Timeout);
                                              });

            if (timeoutTask is not null)
                toDoList.Add(timeoutTask);

            do
            {

                try
                {

                    // Remove all faulted tasks from a previous run of the loop
                    foreach (var errorTask in toDoList.Where(_ => _.Status == TaskStatus.Faulted).ToArray())
                    {

                        toDoList.Remove(errorTask);

                        if (errorTask.Exception?.InnerExceptions is not null)
                            foreach (var exception in errorTask.Exception.InnerExceptions)
                                OnException?.Invoke(exception);

                    }

                    workDone = await Task.WhenAny(toDoList).
                                          ConfigureAwait(false);

                    toDoList.Remove(workDone);

                    if (workDone != timeoutTask)
                    {

                        result = workDone;

                        if (result != null &&
                            !EqualityComparer<T2>.Default.Equals(result.Result, default) &&
                            VerifyResult(result.Result))
                        {
                            return result.Result;
                        }

                    }

                }
                catch (Exception e)
                {
                    DebugX.LogT(e.Message);
                    workDone = Task.FromResult(DefaultResult(Timestamp.Now - startTime));
                }

            }
            while (workDone != timeoutTask && toDoList.Count > 1);

            return DefaultResult(Timestamp.Now - startTime);

        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator iterating through the priority list.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => sortedList.GetEnumerator();

        /// <summary>
        /// Returns an enumerator iterating through the priority list.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
            => ((IEnumerable<T>) sortedList).GetEnumerator();

        #endregion

    }

}
