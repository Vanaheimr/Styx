/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A list of timestamped status entries.
    /// </summary>
    /// <typeparam name="T">The type of the status entries.</typeparam>
    public class StatusSchedule<T> : IEnumerable<Timestamped<T>>
    {

        #region Data

        /// <summary>
        /// The maximum size of the status history.
        /// </summary>
        public const UInt16 DefaultMaxStatusListSize = 100;

        private readonly List<Timestamped<T>> statusSchedule;

        #endregion

        #region Properties

        #region CurrentStatus

        private Timestamped<T> currentStatus;

        /// <summary>
        /// The current status.
        /// </summary>
        public Timestamped<T> CurrentStatus

            => currentStatus;

        #endregion

        #region CurrentValue

        /// <summary>
        /// The current status value.
        /// </summary>
        public T CurrentValue

            => CheckCurrentStatus().Value;

        #endregion

        #region NextStatus

        private Timestamped<T>? nextStatus;

        /// <summary>
        /// The next status.
        /// </summary>
        public Timestamped<T>? NextStatus

            => nextStatus;

        #endregion

        #region NextValue

        ///// <summary>
        ///// The next status value.
        ///// </summary>
        //public T? NextValue

        //    => nextStatus?.Value;

        #endregion

        #region MaxStatusHistorySize

        /// <summary>
        /// The maximum number of stored status entries.
        /// </summary>
        public UInt16 MaxStatusHistorySize { get; }

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// A delegate called whenever the current status changed.
        /// </summary>
        /// <param name="Timestamp">The timestamp when this change was detected.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="StatusSchedule">The status schedule.</param>
        /// <param name="NewStatus">The new timestamped status.</param>
        /// <param name="OldStatus">The old timestamped status.</param>
        /// <param name="DataSource">An optional data source or context for the status update.</param>
        public delegate Task OnStatusChangedDelegate(DateTime           Timestamp,
                                                     EventTracking_Id   EventTrackingId,
                                                     StatusSchedule<T>  StatusSchedule,
                                                     Timestamped<T>     NewStatus,
                                                     Timestamped<T>     OldStatus,
                                                     Context?           DataSource);

        /// <summary>
        /// An event fired whenever the current status changed.
        /// </summary>
        public event OnStatusChangedDelegate OnStatusChanged;

        #endregion

        #region Constructor(s)

        #region StatusSchedule(               MaxStatusListSize = DefaultMaxStatusListSize)

        /// <summary>
        /// Create a new status schedule.
        /// </summary>
        /// <param name="MaxStatusListSize">The maximum number of stored status entries.</param>
        public StatusSchedule(UInt16 MaxStatusListSize = DefaultMaxStatusListSize)
        {

            this.MaxStatusHistorySize  = MaxStatusListSize;
            this.statusSchedule        = new List<Timestamped<T>>();

        }

        #endregion

        #region StatusSchedule(InitialValue,  MaxStatusListSize = DefaultMaxStatusListSize)

        /// <summary>
        /// Create a new status schedule.
        /// </summary>
        /// <param name="InitialValue">An initial value.</param>
        /// <param name="MaxStatusListSize">The maximum number of stored status entries.</param>
        public StatusSchedule(T      InitialValue,
                              UInt16 MaxStatusListSize = DefaultMaxStatusListSize)

            : this(MaxStatusListSize)

        {

            statusSchedule.Add(InitialValue);

        }


        /// <summary>
        /// Create a new status schedule.
        /// </summary>
        /// <param name="InitialValue">An initial timestamped value.</param>
        /// <param name="MaxStatusListSize">The maximum number of stored status entries.</param>
        public StatusSchedule(Timestamped<T>  InitialValue,
                              UInt16          MaxStatusListSize = DefaultMaxStatusListSize)

            : this(MaxStatusListSize)

        {

            statusSchedule.Add(InitialValue);

        }

        #endregion

        #region StatusSchedule(InitialValues, MaxStatusListSize = DefaultMaxStatusListSize)

        /// <summary>
        /// Create a new status schedule.
        /// </summary>
        /// <param name="InitialValues">Initial values.</param>
        /// <param name="MaxStatusListSize">The maximum number of stored status entries.</param>
        public StatusSchedule(IEnumerable<T>  InitialValues,
                              UInt16          MaxStatusListSize = DefaultMaxStatusListSize)

            : this(MaxStatusListSize)

        {

            if (InitialValues.IsNeitherNullNorEmpty())
            {
                var Now = Timestamp.Now;
                statusSchedule.AddRange(InitialValues.Select(_ => new Timestamped<T>(Now, _)));
            }

        }


        /// <summary>
        /// Create a new status schedule.
        /// </summary>
        /// <param name="InitialValues">Initial timestamped values.</param>
        /// <param name="MaxStatusListSize">The maximum number of stored status entries.</param>
        public StatusSchedule(IEnumerable<Timestamped<T>>  InitialValues,
                              UInt16                       MaxStatusListSize = DefaultMaxStatusListSize)

            : this(MaxStatusListSize)

        {

            if (InitialValues.IsNeitherNullNorEmpty())
                statusSchedule.AddRange(InitialValues);

        }

        #endregion

        #endregion


        #region Insert(NewStatus, DataSource = null)

        /// <summary>
        /// Insert a new status entry.
        /// </summary>
        /// <param name="NewStatus">A new status.</param>
        public StatusSchedule<T> Insert(T         NewStatus,
                                        Context?  DataSource   = null)

            => Insert(NewStatus,
                      Timestamp.Now,
                      DataSource);

        #endregion

        #region Insert(NewTimestampedStatus, DataSource = null)

        /// <summary>
        /// Insert a new status entry.
        /// </summary>
        /// <param name="NewTimestampedStatus">A new timestamped status.</param>
        public StatusSchedule<T> Insert(Timestamped<T>  NewTimestampedStatus,
                                        Context?        DataSource   = null)

            => Insert(NewTimestampedStatus.Value,
                      NewTimestampedStatus.Timestamp,
                      DataSource);

        #endregion

        #region Insert(Value, Timestamp, DataSource = null)

        /// <summary>
        /// Insert a new status entry.
        /// </summary>
        /// <param name="Value">The value of the new status entry.</param>
        /// <param name="Timestamp">The timestamp of the new status entry.</param>
        /// <param name="DataSource">An optional data source or context for the status update.</param>
        public StatusSchedule<T> Insert(T         Value,
                                        DateTime  Timestamp,
                                        Context?  DataSource   = null)
        {

            lock (statusSchedule)
            {

                // Ignore 'insert' if the values are the same
                if (statusSchedule.Count == 0 ||
                    !EqualityComparer<T>.Default.Equals(Value, statusSchedule.First().Value))
                {

                    var oldStatus          = currentStatus;

                    // Remove any old status having the same timestamp!
                    var newStatusSchedule  = statusSchedule.
                                                 Where (status => status.Timestamp.ToISO8601() != Timestamp.ToISO8601()).
                                                 ToList();

                    newStatusSchedule.Add(new Timestamped<T>(Timestamp, Value));

                    statusSchedule.Clear();
                    statusSchedule.AddRange(newStatusSchedule.
                                                OrderByDescending(v => v.Timestamp).
                                                Take(MaxStatusHistorySize));

                    // Will also call the change-events!
                    CheckCurrentStatus(null, DataSource);

                }

            }

            return this;

        }

        #endregion

        #region Insert (StatusList, DataSource = null)

        /// <summary>
        /// Insert the given enumeration of status entries.
        /// </summary>
        /// <param name="StatusList">An enumeration of status entries.</param>
        /// <param name="DataSource">An optional data source or context for the status update.</param>
        public StatusSchedule<T> Insert(IEnumerable<Timestamped<T>>  StatusList,
                                        Context?                     DataSource   = null)
        {

            lock (statusSchedule)
            {

                var oldStatus          = currentStatus;

                // Remove any old status having the same timestamp!
                var newStatusSchedule  = StatusList.
                                             GroupBy(status => status.Timestamp).
                                             Select (group  => group.
                                                                   OrderByDescending(status => status.Timestamp).
                                                                   First()).
                                             OrderByDescending(v => v.Timestamp).
                                             Take(MaxStatusHistorySize).
                                             ToArray();

                statusSchedule.AddRange(newStatusSchedule);

                CheckCurrentStatus(oldStatus,
                                   DataSource);

            }

            return this;

        }

        #endregion

        #region Set    (StatusList, ChangeMethod = Replace, DataSource = null)

        /// <summary>
        /// Set the given enumeration of status entries.
        /// </summary>
        /// <param name="StatusList">An enumeration of status entries.</param>
        /// <param name="ChangeMethod">A change method.</param>
        /// <param name="DataSource">An optional data source or context for the status update.</param>
        public StatusSchedule<T> Set(IEnumerable<Timestamped<T>>  StatusList,
                                     ChangeMethods                ChangeMethod   = ChangeMethods.Replace,
                                     Context?                     DataSource     = null)

            => ChangeMethod == ChangeMethods.Insert
                   ? Insert (StatusList, DataSource)
                   : Replace(StatusList, DataSource);

        #endregion

        #region Replace(StatusList, DataSource = null)

        /// <summary>
        /// Insert the given enumeration of status entries.
        /// </summary>
        /// <param name="StatusList">An enumeration of status entries.</param>
        /// <param name="DataSource">An optional data source or context for the status update.</param>
        public StatusSchedule<T> Replace(IEnumerable<Timestamped<T>>  StatusList,
                                         Context?                     DataSource   = null)
        {

            lock (statusSchedule)
            {

                var oldStatus          = currentStatus;

                // Remove any status having the same timestamp!
                var newStatusSchedule  = StatusList.
                                             GroupBy(status => status.Timestamp).
                                             Select (group  => group.
                                                                   OrderByDescending(status => status.Timestamp).
                                                                   First()).
                                             OrderByDescending(v => v.Timestamp).
                                             Take(MaxStatusHistorySize).
                                             ToArray();

                statusSchedule.Clear();
                statusSchedule.AddRange(newStatusSchedule);

                CheckCurrentStatus(oldStatus,
                                   DataSource);

            }

            return this;

        }

        #endregion


        #region (private) CheckCurrentStatus(OldStatus = null, DataSource = null)

        private Timestamped<T> CheckCurrentStatus(Timestamped<T>?  OldStatus    = null,
                                                  Context?         DataSource   = null)
        {

            var callChangeEvents  = false;
            var oldStatus         = OldStatus ?? currentStatus;

            lock (statusSchedule)
            {

                var now            = Timestamp.Now;

                var historyList    = statusSchedule.Where(status => status.Timestamp <= now).ToArray();
                if (historyList.Any())
                    currentStatus  = historyList.First();

                var futureList     = statusSchedule.Where(status => status.Timestamp  > now).ToArray();
                    nextStatus     = futureList.Any()
                                         ? futureList.Last()
                                         : null;

                callChangeEvents   = !EqualityComparer<T>.Default.Equals(currentStatus.Value, oldStatus.Value);

            }

            if (callChangeEvents)
                OnStatusChanged?.Invoke(Timestamp.Now,
                                        EventTracking_Id.New,
                                        this,
                                        currentStatus,
                                        oldStatus,
                                        DataSource);

            return currentStatus;

        }

        #endregion


        #region IEnumerable<Timestamped<T>> Members

        /// <summary>
        /// Return a status enumerator.
        /// </summary>
        public IEnumerator<Timestamped<T>> GetEnumerator()

            => statusSchedule.
                   OrderByDescending(status => status.Timestamp).
                   GetEnumerator();

        /// <summary>
        /// Return a status enumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()

            => statusSchedule.
                   OrderByDescending(status => status.Timestamp).
                   GetEnumerator();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => currentStatus.ToString();

        #endregion

    }

}
