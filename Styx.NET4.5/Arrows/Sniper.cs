/*
 * Copyright (c) 2011-2013, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

#if !SILVERLIGHT 

#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

#endregion

namespace eu.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// A class of specialized IEnumerable extension methods.
    /// </summary>
    public static class SniperExtensions
    {

#if SILVERLIGHT

        // todo!

#else

        #region ToSniper(this IEnumerable, Autostart = false, StartAsTask = false, InitialDelay = null)

        /// <summary>
        /// Creates a new Sniper fireing the content of the given IEnumerable.
        /// </summary>
        /// <typeparam name="TMessage">The type of the emitted messages/objects.</typeparam>
        /// <param name="IEnumerable">An enumeration of messages/objects to send.</param>
        /// <param name="Autostart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        public static Sniper<TMessage> ToSniper<TMessage>(this IEnumerable<TMessage>  IEnumerable,
                                                          Boolean                     Autostart     = false,
                                                          Boolean                     StartAsTask   = false,
                                                          Nullable<TimeSpan>          InitialDelay  = null)
        {
            return new Sniper<TMessage>(IEnumerable, Autostart, StartAsTask, InitialDelay);
        }

        #endregion

        #region ToSniper(this IEnumerator, Autostart = false, StartAsTask = false, InitialDelay = null)

        /// <summary>
        /// Creates a new Sniper fireing the content of the given IEnumerable.
        /// </summary>
        /// <typeparam name="TMessage">The type of the emitted messages/objects.</typeparam>
        /// <param name="IEnumerator">An enumerator of messages/objects to send.</param>
        /// <param name="Autostart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        /// <returns>A new Sniper.</returns>
        public static Sniper<TMessage> ToSniper<TMessage>(this IEnumerator<TMessage>  IEnumerator,
                                                          Boolean                     Autostart     = false,
                                                          Boolean                     StartAsTask   = false,
                                                          Nullable<TimeSpan>          InitialDelay  = null)
        {
            return new Sniper<TMessage>(IEnumerator, Autostart, StartAsTask, InitialDelay);
        }

        #endregion

#endif

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
        private readonly IEnumerator<TOut> IEnumerator;

        private readonly Func<TOut> Func;

        #endregion

        #region Properties

        #region InitialDelay

        /// <summary>
        /// The initial delay before starting to fire asynchronously.
        /// </summary>
        public Nullable<TimeSpan> InitialDelay { get; private set; }

        #endregion

        #region IsTask

        /// <summary>
        /// Whether the sniper is running as its own task or not.
        /// </summary>
        public Boolean IsTask
        {
            get
            {
                return FireTask != null;
            }
        }

        #endregion

        #region FireCancellationTokenSource

        /// <summary>
        /// Signals to a FireCancellationToken that it should be canceled.
        /// </summary>
        public CancellationTokenSource FireCancellationTokenSource { get; private set; }

        #endregion

        #region FireCancellationToken

        /// <summary>
        /// Propogates notification that the asynchronous fireing should be canceled.
        /// </summary>
        public CancellationToken FireCancellationToken { get; private set; }

        #endregion

        #region FireTask

        /// <summary>
        /// The internal task for fireing the messages/objects.
        /// </summary>
        public Task FireTask { get; private set; }

        #endregion

        #region Intervall

        /// <summary>
        /// The intervall will throttle the automatic measurement of passive
        /// sensors and the event notifications of active sensors.
        /// </summary>
        public TimeSpan Intervall { get; set; }

        #endregion

        #region ThrottlingSleepDuration

        /// <summary>
        /// The amount of time in milliseconds a passive sensor
        /// will sleep if it is in throttling mode.
        /// </summary>
        public Int32 ThrottlingSleepDuration { get; set; }

        #endregion

        #region LastFireTime

        /// <summary>
        /// The last time the sniper fired.
        /// </summary>
        public DateTime LastFireTime { get; private set; }

        #endregion

        #endregion

        #region Events

        public event NotificationEventHandler<TOut> OnNotification;

        public event CompletedEventHandler OnCompleted;

        public event ExceptionEventHandler OnException;

        #endregion

        #region Constructor(s)

        #region Sniper(IEnumerable, Autostart = false, StartAsTask = false, InitialDelay = 0)

        /// <summary>
        /// The Sniper fetches messages/objects from the given IEnumerable
        /// and sends them to the recipients.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S&gt; as element source.</param>
        /// <param name="Autostart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        public Sniper(IEnumerable<TOut>      IEnumerable,
                      Boolean                Autostart    = false,
                      Boolean                StartAsTask  = false,
                      Nullable<TimeSpan>     InitialDelay = null)
        {

            #region Initial Checks

            if (IEnumerable == null)
                throw new ArgumentNullException("The given IEnumerable must not be null!");

            #endregion

            this.IEnumerator = IEnumerable.GetEnumerator();

            if (this.IEnumerator == null)
                throw new ArgumentNullException("IEnumerable.GetEnumerator() must not be null!");

            this.InitialDelay            = InitialDelay;
            this.Intervall               = TimeSpan.FromSeconds(10);
            this.ThrottlingSleepDuration = 1000;

            if (Autostart)
                StartToFire(StartAsTask);

        }

        #endregion

        #region Sniper(IEnumerator, Autostart = false, StartAsTask = false, InitialDelay = 0)

        /// <summary>
        /// The Sniper fetches messages/objects from the given IEnumerator
        /// and sends them to the recipients.
        /// </summary>
        /// <param name="IEnumerator">An IEnumerator&lt;S&gt; as element source.</param>
        /// <param name="Autostart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        public Sniper(IEnumerator<TOut>      IEnumerator,
                      Boolean                Autostart    = false,
                      Boolean                StartAsTask  = false,
                      Nullable<TimeSpan>     InitialDelay = null)
        {

            #region Initial Checks

            if (IEnumerator == null)
                throw new ArgumentNullException("The given IEnumerator must not be null!");

            #endregion

            this.IEnumerator             = IEnumerator;
            this.InitialDelay            = InitialDelay;
            this.Intervall               = TimeSpan.FromSeconds(10);
            this.ThrottlingSleepDuration = 1000;

            if (Autostart)
                StartToFire(StartAsTask);

        }

        #endregion

        #region Sniper(Func, Autostart = false, StartAsTask = false, InitialDelay = 0)

        /// <summary>
        /// The Sniper fetches messages/objects from the given IEnumerable
        /// and sends them to the recipients.
        /// </summary>
        /// <param name="IEnumerable">An IEnumerable&lt;S&gt; as element source.</param>
        /// <param name="Autostart">Start the sniper automatically.</param>
        /// <param name="StartAsTask">Start the sniper within its own task.</param>
        /// <param name="InitialDelay">Set the initial delay of the sniper in milliseconds.</param>
        public Sniper(Func<TOut>         Func,
                      Boolean            Autostart    = false,
                      Boolean            StartAsTask  = false,
                      Nullable<TimeSpan> InitialDelay = null)
        {

            #region Initial Checks

            if (Func == null)
                throw new ArgumentNullException("The given Func must not be null!");

            #endregion
            
            this.Func                    = Func;
            this.InitialDelay            = InitialDelay;
            this.Intervall               = TimeSpan.FromSeconds(10);
            this.ThrottlingSleepDuration = 1000;

            if (Autostart)
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

            if (FireTask == null)
            {
                FireCancellationTokenSource = new CancellationTokenSource();
                FireCancellationToken       = FireCancellationTokenSource.Token;
                FireTask                    = new Task(StartFireing, FireCancellationToken, TaskCreationOption);
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

            try
            {

                if (IsTask && InitialDelay != null && InitialDelay.HasValue)
                    Thread.Sleep(InitialDelay.Value);

                if (IEnumerator != null)
                {
                    while (IEnumerator.MoveNext())
                    {

                        if (OnNotification != null)
                            OnNotification(IEnumerator.Current);

                        // Sleep if we are in throttling mode
                        while (LastFireTime + Intervall > DateTime.Now)
                            Thread.Sleep(ThrottlingSleepDuration);

                        LastFireTime = DateTime.Now;

                    }
                }

                else if (Func != null)
                {

                    while (true)
                    {

                        if (OnNotification != null)
                            OnNotification(Func());

                        // Sleep if we are in throttling mode
                        while (LastFireTime + Intervall > DateTime.Now)
                            Thread.Sleep(ThrottlingSleepDuration);

                        LastFireTime = DateTime.Now;

                    }

                }

                if (OnCompleted != null)
                    OnCompleted(this);

            }

            catch (Exception e)
            {
                if (OnException != null)
                    OnException(this, e);
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
                if (FireTask != null)
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

#endif
