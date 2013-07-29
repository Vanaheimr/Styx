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

namespace eu.Vanaheimr.Styx
{

    public interface IVotingSender<T, V> : IArrowSender<T>
    {
        event VotingEventHandler<T, V> OnVoting;
    }

    public interface IVotingSender<T1, T2, V> : IArrowSender<T1, T2>
    {
        event VotingEventHandler<T1, T2, V> OnVoting;
    }

    public interface IVotingSender<T1, T2, T3, V> : IArrowSender<T1, T2, T3>
    {
        event VotingEventHandler<T1, T2, T3, V> OnVoting;
    }

    public interface IVotingSender<T1, T2, T3, T4, V> : IArrowSender<T1, T2, T3, T4>
    {
        event VotingEventHandler<T1, T2, T3, T4, V> OnVoting;
    }

    public interface IVotingSender<T1, T2, T3, T4, T5, V> : IArrowSender<T1, T2, T3, T4, T5>
    {
        event VotingEventHandler<T1, T2, T3, T4, T5, V> OnVoting;
    }

}
