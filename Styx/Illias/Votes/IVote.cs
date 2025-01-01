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

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Votes
{

    // Delegates

    #region VoteEvaluator

    /// <summary>
    /// A delegate for evaluating a vote based on the
    /// number of yes and no votes.
    /// </summary>
    /// <typeparam name="TResult">The type of the voting result.</typeparam>
    /// <param name="NumberOfYesVotes">The current number of yes votes.</param>
    /// <param name="NumberOfNoVotes">The current number of no votes.</param>
    public delegate TResult VoteEvaluator<TResult>(Int32 NumberOfYesVotes, Int32 NumberOfNoVotes);

    #endregion


    #region IVoteExtensions

    /// <summary>
    /// Extensions methods for the IVote interface.
    /// </summary>
    public static class IVoteExtensions
    {

        #region Yes(this IVote<Boolean>)

        /// <summary>
        /// Vote 'yes' or 'ok' or 'allow' or 'accept'.
        /// </summary>
        public static void Yes(this IVote<Boolean> Vote)
        {
            Vote.VoteFor(true);
        }

        #endregion

        #region Ok(this IVote<Boolean>)

        /// <summary>
        /// Vote 'yes' or 'ok' or 'allow' or 'accept'.
        /// </summary>
        public static void Ok(this IVote<Boolean> Vote)
        {
            Vote.VoteFor(true);
        }

        #endregion

        #region Allow(this IVote<Boolean>)

        /// <summary>
        /// Vote 'yes' or 'ok' or 'allow' or 'accept'.
        /// </summary>
        public static void Allow(this IVote<Boolean> Vote)
        {
            Vote.VoteFor(true);
        }

        #endregion

        #region Accept(this IVote<Boolean>)

        /// <summary>
        /// Vote 'yes' or 'ok' or 'allow' or 'accept'.
        /// </summary>
        public static void Accept(this IVote<Boolean> Vote)
        {
            Vote.VoteFor(true);
        }

        #endregion


        #region No(this IVote<Boolean>)

        /// <summary>
        /// Vote 'no' or 'deny'.
        /// </summary>
        public static void No(this IVote<Boolean> Vote)
        {
            Vote.VoteFor(false);
        }

        #endregion

        #region Deny(this IVote<Boolean>)

        /// <summary>
        /// Vote 'no' or 'deny'.
        /// </summary>
        public static void Deny(this IVote<Boolean> Vote)
        {
            Vote.VoteFor(false);
        }

        #endregion

    }

    #endregion


    // Interfaces

    #region IVote

    /// <summary>
    /// A vote is a simple way to ask multiple event subscribers
    /// about their opinion and to evaluate the results.
    /// </summary>
    public interface IVote
    {

        /// <summary>
        /// The current number of votes.
        /// </summary>
        UInt32  TotalNumberOfVotes { get; }

    }

    #endregion

    #region IVote<TResult>

    /// <summary>
    /// A vote is a simple way to ask multiple event subscribers
    /// about their opinion and to evaluate the results.
    /// </summary>
    /// <typeparam name="TResult">The type of the voting result.</typeparam>
    public interface IVote<TResult> : IVote
    {

        /// <summary>
        /// Give your vote.
        /// </summary>
        /// <param name="VotingValue">The value of the vote.</param>
        void VoteFor(TResult VotingValue);

        /// <summary>
        /// The result of the voting.
        /// </summary>
        TResult Result { get; }

    }

    #endregion

}
