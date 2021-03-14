/*
 * Copyright (c) 2010-2021 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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

namespace org.GraphDefined.Vanaheimr.Illias.Votes
{

    /// <summary>
    /// A veto vote is a simple way to ask multiple event subscribers
    /// if an action, e.g. AddVertex(...) should be processed or suspended.
    /// If anyone is unhappy with it, the result of the vote will be false.
    /// </summary>
    public class VetoVote : ABooleanVote
    {

        #region VetoVote()

        /// <summary>
        /// A veto vote is a simple way to ask multiple event subscribers
        /// if an action, e.g. AddVertex(...) should be processed or suspended.
        /// If anyone is unhappy with it, the result of the vote will be false.
        /// </summary>
        public VetoVote()
            : base((yes, no) => no == 0)
        { }

        #endregion


        #region Veto()

        /// <summary>
        /// Veto
        /// </summary>
        public void Veto()
        {
            VoteFor(false);
        }

        #endregion

    }

}
