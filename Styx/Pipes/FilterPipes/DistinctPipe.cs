/*
 * Copyright (c) 2010-2015, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

using org.GraphDefined.Vanaheimr.Illias.Collections;
using System;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    public static class DistinctPipeExtensions
    {

        /// <summary>
        /// The DistinctPipe will filter duplicate entries from the enumeration.
        /// </summary>
        /// <typeparam name="S">The type of the elements within the filter.</typeparam>
        public static DistinctPipe<S> Distinct<S>(this IEndPipe<S>      SourcePipe,
                                                  IEqualityComparer<S>  EqualityComparer = null)
        {
            return new DistinctPipe<S>(SourcePipe, EqualityComparer);
        }

    }
    
    /// <summary>
    /// The DuplicateFilterPipe will not allow a duplicate object to pass through it.
    /// This is accomplished by the Pipe maintaining an internal HashSet that is used
    /// to store a history of previously seen objects.
    /// Thus, the more unique objects that pass through this Pipe, the slower it
    /// becomes as a log_2 index is checked for every object.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class DistinctPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>
    {

        #region Data

        private readonly IEqualityComparer<S>  EqualityComparer;
        private readonly HashedSet<S>          ValueHistory;

        #endregion

        #region Constructor(s)

        #region DistinctPipe(SourcePipe, EqualityComparer = null)

        /// <summary>
        /// Creates a new DistinctPipe.
        /// </summary>
        public DistinctPipe(IEndPipe<S> SourcePipe, IEqualityComparer<S> EqualityComparer = null)
            : base(SourcePipe)
        {
            this.EqualityComparer  = (EqualityComparer != null) ? EqualityComparer : EqualityComparer<S>.Default;
            this.ValueHistory      = new HashedSet<S>();
        }

        #endregion

        #endregion

        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (SourcePipe == null)
                return false;

            while (SourcePipe.MoveNext())
            {

                _CurrentElement = SourcePipe.Current;

                if (!ValueHistory.Contains(_CurrentElement))
                {
                    ValueHistory.Add(_CurrentElement);
                    return true;
                }

            }

            return false;

        }

        #endregion

    }

}
