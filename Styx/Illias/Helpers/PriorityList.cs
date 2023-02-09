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

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

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

        private readonly Dictionary<UInt32, T>  _PriorityList;
        private T[]                             _SortedList;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new priority list.
        /// </summary>
        public PriorityList()
        {

            this._PriorityList  = new Dictionary<UInt32, T>();
            this._SortedList    = new T[0];

        }

        #endregion


        #region Add(Service)

        /// <summary>
        /// Add a new service to the priority list.
        /// </summary>
        /// <param name="Service">A service.</param>
        public void Add(T Service)
        {
            lock (_PriorityList)
            {

                _PriorityList.Add(_PriorityList.Count > 0
                                    ? _PriorityList.Keys.Max() + 1
                                    : 1,
                                  Service);

                _SortedList = _PriorityList.
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

            if (Work == null)
                return Task.FromResult(new T2[0]);

            return Task.WhenAll(_SortedList.Select(service => Work(service)));

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

            if (DefaultResult == null)
                return default;

            if (Work == null || VerifyResult == null)
                return DefaultResult(TimeSpan.Zero);

            Task<T2> WorkDone;
            Task<T2> Result;

            #endregion

            var StartTime    = DateTime.UtcNow;

            var ToDoList     = _SortedList.
                                   Select(service => Work(service)).
                                   ToList();

            var TimeoutTask  = Task.Run(() => {
                                                  System.Threading.Thread.Sleep(Timeout);
                                                  return DefaultResult(Timeout) ?? default;
                                              });

            ToDoList.Add(TimeoutTask);

            do
            {

                try
                {

                    // Remove all faulted tasks from a previous run of the loop
                    foreach (var errorTask in ToDoList.Where(_ => _.Status == TaskStatus.Faulted).ToArray())
                    {

                        ToDoList.Remove(errorTask);

                        foreach (var exception in errorTask.Exception.InnerExceptions)
                        {
                            OnException?.Invoke(exception);
                            DebugX.LogT(exception.Message);
                        }

                    }

                    WorkDone = await Task.WhenAny(ToDoList).
                                          ConfigureAwait(false);

                    ToDoList.Remove(WorkDone);

                    if (WorkDone != TimeoutTask)
                    {

                        Result = WorkDone as Task<T2>;

                        if (Result != null &&
                            !EqualityComparer<T2>.Default.Equals(Result.Result, default) &&
                            VerifyResult(Result.Result))
                        {
                            return Result.Result;
                        }

                    }

                }
                catch (Exception e)
                {
                    DebugX.LogT(e.Message);
                    WorkDone = null;
                }

            }
            while (WorkDone != TimeoutTask && ToDoList.Count > 1);

            return DefaultResult(DateTime.UtcNow - StartTime);

        }

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Returns an enumerator iterating through the priority list.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => _SortedList.GetEnumerator();

        /// <summary>
        /// Returns an enumerator iterating through the priority list.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
            => ((IEnumerable<T>) _SortedList).GetEnumerator();

        #endregion

    }

}
