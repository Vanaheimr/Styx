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
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.Styx.Arrows
{

    /// <summary>
    /// Filters the consuming objects between two values.
    /// </summary>
    /// <typeparam name="TMessage">The type of the consuming and emitting messages/objects.</typeparam>
    public class BandFilterArrow<TMessage> : AbstractFilterArrow<TMessage>
        where TMessage : IComparable<TMessage>, IComparable
    {

        #region Properties

        #region Lower

        /// <summary>
        /// The lower bound of the passed values.
        /// </summary>
        public TMessage Lower { get; private set; }

        #endregion

        #region Upper

        /// <summary>
        /// The upper bound of the passed values.
        /// </summary>
        public TMessage Upper { get; private set; }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Sends a message when then received values are not within
        /// the bounding box of then given Min/Max-values.
        /// </summary>
        /// <param name="Lower">The lower bound of the passed values.</param>
        /// <param name="Upper">The upper bound of the passed values.</param>
        /// <param name="InvertedFilter">Invert the filter behaviour.</param>
        /// <param name="ArrowSender">The sender of the messages/objects.</param>
        public BandFilterArrow(TMessage                Lower,
                               TMessage                Upper,
                               Boolean                 InvertedFilter  = false,
                               IArrowSender<TMessage>  ArrowSender     = null)

            : base(Message => {

                        // Is Message < Lower?
                        if (Message.CompareTo(Lower) < 0)
                            return true;

                        // Is Message > Upper?
                        if (Message.CompareTo(Upper) > 0)
                            return true;

                        return false;

                   },
                   InvertedFilter,
                   ArrowSender)

        {

            this.Lower   = Lower;
            this.Upper   = Upper;

        }

        #endregion

    }

}
