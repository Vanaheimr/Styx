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

#region Usings

using System;
using org.GraphDefined.Vanaheimr.Illias.Votes;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{


    public abstract class ANotificator : IArrowSender
    {

        public event StartedEventHandler OnStarted;

        public event ExceptionOccuredEventHandler OnExceptionOccured;

        public event CompletedEventHandler OnCompleted;

        public void SignalStarted(Object Sender, DateTime Timestamp, String Message)
        {

            var OnStartedLocal = OnStarted;
            if (OnStartedLocal != null)
                OnStartedLocal(Sender, Timestamp, Message);

        }

        public void SignalError(Object Sender, DateTime Timestamp, Exception Exception)
        {

            var OnExceptionOccuredLocal = OnExceptionOccured;
            if (OnExceptionOccuredLocal != null)
                OnExceptionOccuredLocal(Sender, Timestamp, Exception);

        }

        public void SignalCompleted(Object Sender, DateTime Timestamp, String Message)
        {

            var OnCompletedLocal = OnCompleted;
            if (OnCompletedLocal != null)
                OnCompletedLocal(Sender, Timestamp, Message);

        }

    }


    public class Notificator<T> : ANotificator, INotificator<T>, IArrowSender<T>
    {

        public event NotificationEventHandler<T> OnNotification;

        public void SendNotification(T Message)
        {

            var OnNotificationLocal = OnNotification;
            if (OnNotificationLocal != null)
                OnNotificationLocal(Message);

        }

    }


    public class VotingNotificator<T, V> : Notificator<T>, IVotingNotificator<T, V>, IVotingSender<T, V>
    {

        #region Data

        private readonly Func<IVote<V>> VoteCreator;
        private readonly V DefaultValue;

        #endregion

        #region Events

        public event VotingEventHandler<T, V> OnVoting;

        #endregion


        public VotingNotificator(Func<IVote<V>> VoteCreator, V DefaultValue)
        {

            if (VoteCreator == null)
                throw new ArgumentNullException("VoteCreator", "The given VoteCreator delegate must not be null!");

            this.VoteCreator = VoteCreator;
            this.DefaultValue = DefaultValue;

        }

        public V SendVoting(T Message)
        {

            if (this.OnVoting == null)
                return DefaultValue;

            var Vote = VoteCreator();

            var OnVotingLocal = OnVoting;
            if (OnVotingLocal != null)
                OnVotingLocal(Message, Vote);

            return Vote.Result;

        }

        public V SendVoting(T Message, IVote<V> Vote)
        {

            if (this.OnVoting == null)
                return Vote.Result;

            var OnVotingLocal = OnVoting;
            if (OnVotingLocal != null)
                OnVotingLocal(Message, Vote);

            return Vote.Result;

        }

    }


    public class Notificator<T1, T2> : ANotificator, INotificator<T1, T2>, IArrowSender<T1, T2>
    {

        public event NotificationEventHandler<T1, T2> OnNotification;

        public void SendNotification(T1 Message1, T2 Message2)
        {

            var OnNotificationLocal = OnNotification;
            if (OnNotificationLocal != null)
                OnNotificationLocal(Message1, Message2);

        }

    }


    public class VotingNotificator<T1, T2, V> : Notificator<T1, T2>, IVotingNotificator<T1, T2, V>, IVotingSender<T1, T2, V>
    {

        #region Data

        private readonly Func<IVote<V>> VoteCreator;
        private readonly V DefaultValue;

        #endregion

        #region Events

        public event VotingEventHandler<T1, T2, V> OnVoting;

        #endregion


        public VotingNotificator(Func<IVote<V>> VoteCreator, V DefaultValue)
        {

            if (VoteCreator == null)
                throw new ArgumentNullException("VoteCreator", "The given VoteCreator delegate must not be null!");

            this.VoteCreator  = VoteCreator;
            this.DefaultValue = DefaultValue;

        }

        public V SendVoting(T1 Message1, T2 Message2)
        {

            if (this.OnVoting == null)
                return DefaultValue;

            var Vote = VoteCreator();

            var OnVotingLocal = OnVoting;
            if (OnVotingLocal != null)
                OnVotingLocal(Message1, Message2, Vote);

            return Vote.Result;

        }

        public V SendVoting(T1 Message1, T2 Message2, IVote<V> Vote)
        {

            if (this.OnVoting == null)
                return Vote.Result;

            var OnVotingLocal = OnVoting;
            if (OnVotingLocal != null)
                OnVotingLocal(Message1, Message2, Vote);

            return Vote.Result;

        }

    }




    public class Notificator<T1, T2, T3> : ANotificator, INotificator<T1, T2, T3>, IArrowSender<T1, T2, T3>
    {

        public event NotificationEventHandler<T1, T2, T3> OnNotification;

        public void SendNotification(T1 Message1, T2 Message2, T3 Message3)
        {

            var OnNotificationLocal = OnNotification;
            if (OnNotificationLocal != null)
                OnNotificationLocal(Message1, Message2, Message3);

        }

    }

    public class VotingNotificator<T1, T2, T3, V> : Notificator<T1, T2, T3>, IVotingNotificator<T1, T2, T3, V>, IVotingSender<T1, T2, T3, V>
    {

        #region Data

        private readonly Func<IVote<V>> VoteCreator;
        private readonly V              DefaultValue;

        #endregion

        #region Events

        public event VotingEventHandler<T1, T2, T3, V> OnVoting;

        #endregion


        public VotingNotificator(Func<IVote<V>> VoteCreator, V DefaultValue)
        {

            if (VoteCreator == null)
                throw new ArgumentNullException(nameof(VoteCreator), "The given VoteCreator delegate must not be null!");

            this.VoteCreator   = VoteCreator;
            this.DefaultValue  = DefaultValue;

        }

        public V SendVoting(T1 Message1, T2 Message2, T3 Message3)
        {

            if (this.OnVoting == null)
                return DefaultValue;

            var Vote = VoteCreator();

            var OnVotingLocal = OnVoting;
            if (OnVotingLocal != null)
                OnVotingLocal(Message1, Message2, Message3, Vote);

            return Vote.Result;

        }

        public V SendVoting(T1 Message1, T2 Message2, T3 Message3, IVote<V> Vote)
        {

            if (this.OnVoting == null)
                return Vote.Result;

            var OnVotingLocal = OnVoting;
            if (OnVotingLocal != null)
                OnVotingLocal(Message1, Message2, Message3, Vote);

            return Vote.Result;

        }

    }



    public delegate void AggregatedNotificationEventHandler<T>(DateTime DateTime, T Message);

    public class AggregatedNotificator<T>
    {

        #region Data

        private Timer   UpdateEVSEStatusTimer;
        private List<T> ListOfT;

        #endregion

        #region Events

        public event AggregatedNotificationEventHandler<IEnumerable<T>>  OnNotification;

        #endregion


        public AggregatedNotificator()
        {

            UpdateEVSEStatusTimer = new Timer(SendNotification2);
            ListOfT               = new List<T>();

        }

        public void SendNotification(DateTime Timestamp, T Message)
        {

            lock (ListOfT)
            {
                ListOfT.Add(Message);
            }

            UpdateEVSEStatusTimer.Change(5000, Timeout.Infinite);
            Console.WriteLine(DateTime.Now + " something added!");

        }

        public void SendNotification2(Object Context)
        {

            UpdateEVSEStatusTimer.Change(Timeout.Infinite, Timeout.Infinite);

            List<T> NewListOfT = null;

            lock (ListOfT)
            {
                NewListOfT = new List<T>(ListOfT);
                ListOfT.Clear();
            }

            var OnNotificationLocal = OnNotification;
            if (OnNotificationLocal != null)
                OnNotificationLocal(DateTime.Now, NewListOfT);

        }

    }

}
