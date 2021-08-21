/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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
using System.Threading;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Votes
{

    /// <summary>
    /// A majority vote is a simple way to ask multiple event subscribers
    /// if an action, e.g. AddVertex(...) should be processed or suspended.
    /// If a majority of >50% are okay with it, the result of the vote will be true.
    /// </summary>
    public abstract class ABooleanVote : IVote<Boolean>
    {

        #region Data

        /// <summary>
        /// The current number of vetos.
        /// </summary>
        protected Int32 _NumberOfYesVotes;

        /// <summary>
        /// The current total number of votes.
        /// </summary>
        protected Int32 _NumberOfNoVotes;

        /// <summary>
        /// A delegate to evaluate the result of the voting.
        /// </summary>
        protected VoteEvaluator<Boolean> VoteEvaluator;

        #endregion

        #region Properties

        /// <summary>
        /// The current total number of votes.
        /// </summary>
        public UInt32 TotalNumberOfVotes
        {
            get
            {
                return (UInt32) (_NumberOfYesVotes + _NumberOfNoVotes);
            }
        }

        /// <summary>
        /// The result of the voting.
        /// </summary>
        public Boolean Result
        {
            get
            {
                return VoteEvaluator(_NumberOfYesVotes, _NumberOfNoVotes);
            }
        }

        #endregion


        #region ABooleanVote(VoteEvaluator)

        /// <summary>
        /// An abstract boolean voting.
        /// </summary>
        /// <param name="VoteEvaluator">A delegate to evaluate the result of the voting.</param>
        public ABooleanVote(VoteEvaluator<Boolean> VoteEvaluator)
        {

            if (VoteEvaluator == null)
                throw new ArgumentNullException("VoteEvaluator", "The given VoteEvaluator must not be null!");

            this.VoteEvaluator = VoteEvaluator;

        }

        #endregion


        #region VoteFor(Boolean VotingValue)

        /// <summary>
        /// Give your vote.
        /// </summary>
        /// <param name="VotingValue">The value of the vote.</param>
        public void VoteFor(Boolean VotingValue)
        {

            if (VotingValue)
                Interlocked.Increment(ref _NumberOfYesVotes);

            else
                Interlocked.Increment(ref _NumberOfNoVotes);

        }

        #endregion

    }

}
