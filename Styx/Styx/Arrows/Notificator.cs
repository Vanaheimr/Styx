/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using org.GraphDefined.Vanaheimr.Illias.Votes;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{


    public abstract class ANotificator : IArrowSender
    {

        public event StartedEventHandler?           OnStarted;

        public event ExceptionOccuredEventHandler?  OnExceptionOccured;

        public event CompletedEventHandler?         OnCompleted;

        public void SignalStarted(Object Sender, DateTime Timestamp, EventTracking_Id EventTrackingId, String Message)
        {
            OnStarted?.Invoke(Sender, Timestamp, EventTrackingId, Message);
        }

        public void SignalError(Object Sender, DateTime Timestamp, EventTracking_Id EventTrackingId, Exception Exception)
        {
            OnExceptionOccured?.Invoke(Sender, Timestamp, EventTrackingId, Exception);
        }

        public void SignalCompleted(Object Sender, DateTime Timestamp, EventTracking_Id EventTrackingId, String Message)
        {
            OnCompleted?.Invoke(Sender, Timestamp, EventTrackingId, Message);
        }

    }


    public class Notificator<T> : ANotificator, INotificator<T>, IArrowSender<T>
    {

        public event NotificationEventHandler<T>? OnNotification;

        public void SendNotification(EventTracking_Id EventTrackingId, T Message)
        {
            OnNotification?.Invoke(EventTrackingId, Message);
        }

    }

    public class VotingNotificator<T, V> : Notificator<T>, IVotingNotificator<T, V>, IVotingSender<T, V>
    {

        #region Data

        private readonly Func<IVote<V>>  VoteCreator;
        private readonly V               DefaultValue;

        #endregion

        #region Events

        public event VotingEventHandler<T, V>? OnVoting;

        #endregion


        public VotingNotificator(Func<IVote<V>> VoteCreator, V DefaultValue)
        {

            this.VoteCreator  = VoteCreator ?? throw new ArgumentNullException(nameof(VoteCreator), "The given VoteCreator delegate must not be null!");
            this.DefaultValue = DefaultValue;

        }

        public V SendVoting(EventTracking_Id EventTrackingId, T Message)
        {

            if (OnVoting is null)
                return DefaultValue;

            var Vote = VoteCreator();

            OnVoting?.Invoke(EventTrackingId, Message, Vote);

            return Vote.Result;

        }

        public V SendVoting(EventTracking_Id EventTrackingId, T Message, IVote<V> Vote)
        {

            if (OnVoting is null)
                return Vote.Result;

            OnVoting?.Invoke(EventTrackingId, Message, Vote);

            return Vote.Result;

        }

    }



    public class Notificator<T1, T2> : ANotificator, INotificator<T1, T2>, IArrowSender<T1, T2>
    {

        public event NotificationEventHandler<T1, T2>? OnNotification;

        public void SendNotification(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2)
        {

            OnNotification?.Invoke(EventTrackingId, Message1, Message2);

        }

    }

    public class VotingNotificator<T1, T2, V> : Notificator<T1, T2>, IVotingNotificator<T1, T2, V>, IVotingSender<T1, T2, V>
    {

        #region Data

        private readonly Func<IVote<V>>  VoteCreator;
        private readonly V               DefaultValue;

        #endregion

        #region Events

        public event VotingEventHandler<T1, T2, V> OnVoting;

        #endregion


        public VotingNotificator(Func<IVote<V>> VoteCreator, V DefaultValue)
        {

            this.VoteCreator  = VoteCreator ?? throw new ArgumentNullException(nameof(VoteCreator), "The given VoteCreator delegate must not be null!");
            this.DefaultValue = DefaultValue;

        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2)
        {

            if (OnVoting is null)
                return DefaultValue;

            var Vote = VoteCreator();

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Vote);

            return Vote.Result;

        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, IVote<V> Vote)
        {

            if (OnVoting is null)
                return Vote.Result;

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Vote);

            return Vote.Result;

        }

    }




    public class Notificator<T1, T2, T3> : ANotificator, INotificator<T1, T2, T3>, IArrowSender<T1, T2, T3>
    {

        public event NotificationEventHandler<T1, T2, T3>? OnNotification;

        public void SendNotification(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3)
        {
            OnNotification?.Invoke(EventTrackingId, Message1, Message2, Message3);
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
            this.VoteCreator   = VoteCreator ?? throw new ArgumentNullException(nameof(VoteCreator), "The given VoteCreator delegate must not be null!");
            this.DefaultValue  = DefaultValue;
        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3)
        {

            if (OnVoting is null)
                return DefaultValue;

            var Vote = VoteCreator();

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Message3, Vote);

            return Vote.Result;

        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, IVote<V> Vote)
        {

            if (OnVoting is null)
                return Vote.Result;

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Message3, Vote);

            return Vote.Result;

        }

    }




    public class Notificator<T1, T2, T3, T4> : ANotificator, INotificator<T1, T2, T3, T4>, IArrowSender<T1, T2, T3, T4>
    {

        public event NotificationEventHandler<T1, T2, T3, T4>? OnNotification;

        public void SendNotification(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4)
        {
            OnNotification?.Invoke(EventTrackingId, Message1, Message2, Message3, Message4);
        }

    }

    public class VotingNotificator<T1, T2, T3, T4, V> : Notificator<T1, T2, T3, T4>, IVotingNotificator<T1, T2, T3, T4, V>, IVotingSender<T1, T2, T3, T4, V>
    {

        #region Data

        private readonly Func<IVote<V>> VoteCreator;
        private readonly V              DefaultValue;

        #endregion

        #region Events

        public event VotingEventHandler<T1, T2, T3, T4, V> OnVoting;

        #endregion


        public VotingNotificator(Func<IVote<V>> VoteCreator, V DefaultValue)
        {
            this.VoteCreator   = VoteCreator ?? throw new ArgumentNullException(nameof(VoteCreator), "The given VoteCreator delegate must not be null!");
            this.DefaultValue  = DefaultValue;
        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4)
        {

            if (OnVoting is null)
                return DefaultValue;

            var Vote = VoteCreator();

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Message3, Message4, Vote);

            return Vote.Result;

        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, IVote<V> Vote)
        {

            if (OnVoting is null)
                return Vote.Result;

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Message3, Message4, Vote);

            return Vote.Result;

        }

    }




    public class Notificator<T1, T2, T3, T4, T5> : ANotificator, INotificator<T1, T2, T3, T4, T5>, IArrowSender<T1, T2, T3, T4, T5>
    {

        public event NotificationEventHandler<T1, T2, T3, T4, T5>? OnNotification;

        public void SendNotification(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5)
        {
            OnNotification?.Invoke(EventTrackingId, Message1, Message2, Message3, Message4, Message5);
        }

    }

    public class VotingNotificator<T1, T2, T3, T4, T5, V> : Notificator<T1, T2, T3, T4, T5>, IVotingNotificator<T1, T2, T3, T4, T5, V>, IVotingSender<T1, T2, T3, T4, T5, V>
    {

        #region Data

        private readonly Func<IVote<V>> VoteCreator;
        private readonly V              DefaultValue;

        #endregion

        #region Events

        public event VotingEventHandler<T1, T2, T3, T4, T5, V> OnVoting;

        #endregion


        public VotingNotificator(Func<IVote<V>> VoteCreator, V DefaultValue)
        {
            this.VoteCreator   = VoteCreator ?? throw new ArgumentNullException(nameof(VoteCreator), "The given VoteCreator delegate must not be null!");
            this.DefaultValue  = DefaultValue;
        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5)
        {

            if (OnVoting is null)
                return DefaultValue;

            var Vote = VoteCreator();

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Message3, Message4, Message5, Vote);

            return Vote.Result;

        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5, IVote<V> Vote)
        {

            if (OnVoting is null)
                return Vote.Result;

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Message3, Message4, Message5, Vote);

            return Vote.Result;

        }

    }




    public class Notificator<T1, T2, T3, T4, T5, T6> : ANotificator, INotificator<T1, T2, T3, T4, T5, T6>, IArrowSender<T1, T2, T3, T4, T5, T6>
    {

        public event NotificationEventHandler<T1, T2, T3, T4, T5, T6>? OnNotification;

        public void SendNotification(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5, T6 Message6)
        {
            OnNotification?.Invoke(EventTrackingId, Message1, Message2, Message3, Message4, Message5, Message6);
        }

    }

    public class VotingNotificator<T1, T2, T3, T4, T5, T6, V> : Notificator<T1, T2, T3, T4, T5, T6>, IVotingNotificator<T1, T2, T3, T4, T5, T6, V>, IVotingSender<T1, T2, T3, T4, T5, T6, V>
    {

        #region Data

        private readonly Func<IVote<V>> VoteCreator;
        private readonly V              DefaultValue;

        #endregion

        #region Events

        public event VotingEventHandler<T1, T2, T3, T4, T5, T6, V> OnVoting;

        #endregion


        public VotingNotificator(Func<IVote<V>> VoteCreator, V DefaultValue)
        {
            this.VoteCreator   = VoteCreator ?? throw new ArgumentNullException(nameof(VoteCreator), "The given VoteCreator delegate must not be null!");
            this.DefaultValue  = DefaultValue;
        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5, T6 Message6)
        {

            if (OnVoting is null)
                return DefaultValue;

            var Vote = VoteCreator();

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Message3, Message4, Message5, Message6, Vote);

            return Vote.Result;

        }

        public V SendVoting(EventTracking_Id EventTrackingId, T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5, T6 Message6, IVote<V> Vote)
        {

            if (OnVoting is null)
                return Vote.Result;

            OnVoting?.Invoke(EventTrackingId, Message1, Message2, Message3, Message4, Message5, Message6, Vote);

            return Vote.Result;

        }

    }





    public delegate void AggregatedNotificationEventHandler<T>(DateTime DateTime, T Message);

    public class AggregatedNotificator<T>
    {

        #region Data

        private readonly Timer   UpdateEVSEStatusTimer;
        private readonly List<T> ListOfT;

        #endregion

        #region Events

        public event AggregatedNotificationEventHandler<IEnumerable<T>>?  OnNotification;

        #endregion


        public AggregatedNotificator()
        {

            UpdateEVSEStatusTimer = new Timer(SendNotification2);
            ListOfT               = [];

        }

        public void SendNotification(DateTime Timestamp, T Message)
        {

            lock (ListOfT)
            {
                ListOfT.Add(Message);
            }

            UpdateEVSEStatusTimer.Change(5000, Timeout.Infinite);
            Console.WriteLine(Illias.Timestamp.Now + " something added!");

        }

        public void SendNotification2(Object? Context)
        {

            UpdateEVSEStatusTimer.Change(Timeout.Infinite, Timeout.Infinite);

            List<T>? NewListOfT = null;

            lock (ListOfT)
            {
                NewListOfT = new List<T>(ListOfT);
                ListOfT.Clear();
            }

            OnNotification?.Invoke(Timestamp.Now, NewListOfT);

        }

    }


}
