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
using org.GraphDefined.Vanaheimr.Illias.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// The DuplicateFilterArrow will not allow a duplicate object to pass through it.
    /// This is accomplished by the Arrow maintaining an internal HashSet that is used
    /// to store a history of previously seen objects.
    /// Thus, the more unique objects that pass through this Arrow, the slower it
    /// becomes as a log_2 index is checked for every object.
    /// </summary>
    /// <typeparam name="TMessage">The type of the consuming and emitting messages/objects.</typeparam>
    public class DuplicateFilterArrow<TMessage> : AbstractArrow<TMessage, TMessage>, IFilterArrow<TMessage>
    {

        #region Data

        private readonly HashedSet<TMessage> historySet = [];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The DuplicateFilterArrow will not allow a duplicate object to pass through it.
        /// This is accomplished by the Arrow maintaining an internal HashSet that is used
        /// to store a history of previously seen objects.
        /// Thus, the more unique objects that pass through this Arrow, the slower it
        /// becomes as a log_2 index is checked for every object.
        /// </summary>
        public DuplicateFilterArrow()
        { }

        #endregion

        #region ProcessMessage(MessageIn, out MessageOut)

        /// <summary>
        /// Process the incoming message and return an outgoing message.
        /// </summary>
        /// <param name="MessageIn">The incoming message.</param>
        /// <param name="MessageOut">The outgoing message.</param>
        protected override Boolean ProcessMessage(EventTracking_Id EventTrackingId, TMessage MessageIn, out TMessage MessageOut)
        {

            MessageOut = MessageIn;

            if (!historySet.Contains(MessageIn))
            {
                historySet.Add(MessageIn);
                return true;
            }

            return false;

        }

        #endregion

    }

}
