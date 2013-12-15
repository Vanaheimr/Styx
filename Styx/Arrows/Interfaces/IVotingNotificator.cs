/*
 * Copyright (c) 2011-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
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
using eu.Vanaheimr.Illias.Commons.Votes;

#endregion

namespace eu.Vanaheimr.Styx.Arrows
{

    public interface IVotingNotificator<T, V> : INotificator<T>, IVotingSender<T, V>
    {
        V SendVoting(T Message);
    }

    public interface IVotingNotificator<T1, T2, V> : INotificator<T1, T2>, IVotingSender<T1, T2, V>
    {
        V SendVoting(T1 Message1, T2 Message2);
        V SendVoting(T1 Message1, T2 Message2, IVote<V> Vote);
    }

    public interface IVotingNotificator<T1, T2, T3, V> : INotificator<T1, T2, T3>, IVotingSender<T1, T2, T3, V>
    {
        V SendVoting(T1 Message1, T2 Message2, T3 Message3);
    }

    public interface IVotingNotificator<T1, T2, T3, T4, V> : INotificator<T1, T2, T3, T4>, IVotingSender<T1, T2, T3, T4, V>
    {
        V SendVoting(T1 Message1, T2 Message2, T3 Message3, T4 Message4);
    }

    public interface IVotingNotificator<T1, T2, T3, T4, T5, V> : INotificator<T1, T2, T3, T4, T5>, IVotingSender<T1, T2, T3, T4, T5, V>
    {
        V SendVoting(T1 Message1, T2 Message2, T3 Message3, T4 Message4, T5 Message5);
    }

}
