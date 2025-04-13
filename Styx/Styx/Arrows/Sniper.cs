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


#region Usings

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// A class of specialized IEnumerable extension methods.
    /// </summary>
    public static class SniperExtensions
    {

        #region ToSniper(this IEnumerable, AutoStart = false, StartAsTask = false, InitialDelay = null)

        /// <summary>
        /// Creates a new Sniper fireing the content of the given IEnumerable.
        /// </summary>
        /// <typeparam name="TMessage">The type of the emitted messages/objects.</typeparam>
        /// <param name="IEnumerable">An enumeration of messages/objects to send.</param>
        /// <param name="AutoStart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        public static Sniper<TMessage> ToSniper<TMessage>(this IEnumerable<TMessage>  IEnumerable,
                                                          Boolean                     AutoStart     = false,
                                                          Boolean                     StartAsTask   = false,
                                                          Nullable<TimeSpan>          InitialDelay  = null)
        {
            return new Sniper<TMessage>(IEnumerable, AutoStart, StartAsTask, InitialDelay);
        }

        #endregion

        #region ToSniper(this IEnumerator, AutoStart = false, StartAsTask = false, InitialDelay = null)

        /// <summary>
        /// Creates a new Sniper fireing the content of the given IEnumerable.
        /// </summary>
        /// <typeparam name="TMessage">The type of the emitted messages/objects.</typeparam>
        /// <param name="IEnumerator">An enumerator of messages/objects to send.</param>
        /// <param name="AutoStart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        /// <returns>A new Sniper.</returns>
        public static Sniper<TMessage> ToSniper<TMessage>(this IEnumerator<TMessage>  IEnumerator,
                                                          Boolean                     AutoStart     = false,
                                                          Boolean                     StartAsTask   = false,
                                                          Nullable<TimeSpan>          InitialDelay  = null)
        {
            return new Sniper<TMessage>(IEnumerator, AutoStart, StartAsTask, InitialDelay);
        }

        #endregion

    }


    /// <summary>
    /// The Sniper fetches messages/objects from a pipe, an IEnumerable or
    /// via an IEnumerator and sends them to the recipients.
    /// </summary>
    /// <typeparam name="TOut">The type of the emitted messages/objects.</typeparam>
    public class Sniper<TOut> : ISniper<TOut>
    {

        #region Data

        /// <summary>
        /// The internal source of messages/objects.
        /// </summary>
        private readonly IEnumerator<TOut>  IEnumerator;

        private readonly Func<TOut>         Func;

        #endregion

        #region Properties

        /// <summary>
        /// The initial delay before starting to fire asynchronously.
        /// </summary>
        public Nullable<TimeSpan>       InitialDelay                   { get; private set; }

        /// <summary>
        /// Whether the sniper is running as its own task or not.
        /// </summary>
        public Boolean                  IsTask
            => FireTask is not null;

        /// <summary>
        /// Signals to a FireCancellationToken that it should be canceled.
        /// </summary>
        public CancellationTokenSource  FireCancellationTokenSource    { get; private set; }

        /// <summary>
        /// Propogates notification that the asynchronous fireing should be canceled.
        /// </summary>
        public CancellationToken        FireCancellationToken          { get; private set; }

        /// <summary>
        /// The internal task for fireing the messages/objects.
        /// </summary>
        public Task                     FireTask                       { get; private set; }

        /// <summary>
        /// The interval will throttle the automatic measurement of passive
        /// sensors and the event notifications of active sensors.
        /// </summary>
        public TimeSpan                 Interval                      { get; set; }

        /// <summary>
        /// The amount of time in milliseconds a passive sensor
        /// will sleep if it is in throttling mode.
        /// </summary>
        public Int32                    ThrottlingSleepDuration        { get; set; }

        /// <summary>
        /// The last time the sniper fired.
        /// </summary>
        public DateTime                 LastFireTime                   { get; private set; }

        #endregion

        #region Events

        public event StartedEventHandler?               OnStarted;

        public event NotificationEventHandler<TOut>?    OnNotification;

        public event CompletedEventHandler?             OnCompleted;

        public event ExceptionOccurredEventHandler?      OnExceptionOccurred;

        #endregion

        #region Constructor(s)

        #region Sniper(IEnumerable, AutoStart = false, StartAsTask = false, InitialDelay = 0)

        /// <summary>
        /// The Sniper fetches messages/objects from the given IEnumerable
        /// and sends them to the recipients.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S&gt; as element source.</param>
        /// <param name="AutoStart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        public Sniper(IEnumerable<TOut>   IEnumerable,
                      Boolean             AutoStart      = false,
                      Boolean             StartAsTask    = false,
                      Nullable<TimeSpan>  InitialDelay   = null)
        {

            this.IEnumerator              = IEnumerable.GetEnumerator();
            this.InitialDelay             = InitialDelay;
            this.Interval                = TimeSpan.FromSeconds(10);
            this.ThrottlingSleepDuration  = 1000;

            if (AutoStart)
                StartToFire(StartAsTask);

        }

        #endregion

        #region Sniper(IEnumerator, AutoStart = false, StartAsTask = false, InitialDelay = 0)

        /// <summary>
        /// The Sniper fetches messages/objects from the given IEnumerator
        /// and sends them to the recipients.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S&gt; as element source.</param>
        /// <param name="AutoStart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        public Sniper(IEnumerator<TOut>   IEnumerator,
                      Boolean             AutoStart      = false,
                      Boolean             StartAsTask    = false,
                      Nullable<TimeSpan>  InitialDelay   = null)
        {

            this.IEnumerator              = IEnumerator;
            this.InitialDelay             = InitialDelay;
            this.Interval                = TimeSpan.FromSeconds(10);
            this.ThrottlingSleepDuration  = 1000;

            if (AutoStart)
                StartToFire(StartAsTask);

        }

        #endregion

        #region Sniper(Func, AutoStart = false, StartAsTask = false, InitialDelay = 0)

        /// <summary>
        /// The Sniper fetches messages/objects from the given IEnumerable
        /// and sends them to the recipients.
        /// </summary>
        /// <param name="Func">A function returning the next message/object.</param>
        /// <param name="AutoStart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        public Sniper(Func<TOut>          Func,
                      Boolean             AutoStart      = false,
                      Boolean             StartAsTask    = false,
                      Nullable<TimeSpan>  InitialDelay   = null)
        {

            this.Func                     = Func;
            this.InitialDelay             = InitialDelay;
            this.Interval                = TimeSpan.FromSeconds(10);
            this.ThrottlingSleepDuration  = 1000;

            if (AutoStart)
                StartToFire(StartAsTask);

        }

        #endregion

        #endregion


        #region AsTask(TaskCreationOption = TaskCreationOptions.AttachedToParent)

        /// <summary>
        /// Create as task for the message/object fireing.
        /// </summary>
        /// <param name="TaskCreationOption">Specifies flags that control optional behavior for the creation and execution of tasks.</param>
        /// <returns>The created task.</returns>
        public Task AsTask(TaskCreationOptions TaskCreationOption = TaskCreationOptions.AttachedToParent)
        {

            if (FireTask is null)
            {
                FireCancellationTokenSource  = new CancellationTokenSource();
                FireCancellationToken        = FireCancellationTokenSource.Token;
                FireTask                     = new Task(StartFireing, FireCancellationToken, TaskCreationOption);
            }

            return FireTask;

        }

        #endregion

        #region (private) StartFireing()

        /// <summary>
        /// Starts the fireing.
        /// </summary>
        private void StartFireing()
        {

            // if already runnning => return

            OnStarted?.Invoke(this, Timestamp.Now, EventTracking_Id.New);

            try
            {

                if (IsTask && InitialDelay != null && InitialDelay.HasValue)
                    Thread.Sleep(InitialDelay.Value);

                if (IEnumerator is not null)
                {
                    while (IEnumerator.MoveNext())
                    {

                        OnNotification?.Invoke(EventTracking_Id.New, IEnumerator.Current);

                        // Sleep if we are in throttling mode
                        while (LastFireTime + Interval > Timestamp.Now)
                            Thread.Sleep(ThrottlingSleepDuration);

                        LastFireTime = Timestamp.Now;

                    }
                }

                else if (Func is not null)
                {

                    while (true)
                    {

                        OnNotification?.Invoke(EventTracking_Id.New, Func());

                        // Sleep if we are in throttling mode
                        while (LastFireTime + Interval > Timestamp.Now)
                            Thread.Sleep(ThrottlingSleepDuration);

                        LastFireTime = Timestamp.Now;

                    }

                }

                OnCompleted?.Invoke(this, Timestamp.Now, EventTracking_Id.New);

            }

            catch (Exception e)
            {
                OnExceptionOccurred?.Invoke(this, Timestamp.Now, EventTracking_Id.New, e);
            }

        }

        #endregion

        #region StartToFire(StartAsTask = false)

        /// <summary>
        /// Starts the sniper fire!
        /// </summary>
        /// <param name="StartAsTask">Whether to run within a seperate task or not.</param>
        public void StartToFire(Boolean StartAsTask = false)
        {

            if (StartAsTask)
            {

                if (FireTask is not null)
                    FireTask.Start();

                else
                {
                    FireTask = AsTask();
                    FireTask.Start();
                }

            }

            else
                StartFireing();

        }

        #endregion

    }

}
