/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/Pipes.NET>
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

namespace de.ahzf.Pipes
{

    /// <summary>
    /// Passes all sensor values while keeping track
    /// of the Min, Max and Average value.
    /// </summary>
    /// <typeparam name="S">The type of the consuming and emitting objects.</typeparam>
    public class MinMaxPipe<S> : AbstractSideEffectPipe<S, S, Tuple<S, S>>
        where S: IComparable, IComparable<S>, IEquatable<S>
    {

        #region Properties

        /// <summary>
        /// The minimum of the passed values.
        /// </summary>
        public S Min { get; protected set; }

        /// <summary>
        /// The maximum of the passed values.
        /// </summary>
        public S Max { get; protected set; }

        #endregion

        #region Constructor(s)

        #region MinMaxFilter<S>(Min, Max, IEnumerable = null, IEnumerator = null)

        /// <summary>
        /// Creates a new MinMaxPipe&lt;S&gt;.
        /// </summary>
        /// <param name="Min">The initial minimum.</param>
        /// <param name="Max">The initial maximum.</param>
        /// <param name="IEnumerable">An optional IEnumerable&lt;Double&gt; as element source.</param>
        /// <param name="IEnumerator">An optional IEnumerator&lt;Double&gt; as element source.</param>
        public MinMaxPipe(S Min, S Max, IEnumerable<S> IEnumerable = null, IEnumerator<S> IEnumerator = null)
            : base(IEnumerable, IEnumerator)
        {
            this.Min = Min;
            this.Max = Max;
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

            if (_InternalEnumerator == null)
                return false;

            if (_InternalEnumerator.MoveNext())
            {

                _CurrentElement = _InternalEnumerator.Current;

                if (Min.CompareTo(_CurrentElement) > 0)
                    Min = _CurrentElement;

                if (Max.CompareTo(_CurrentElement) < 0)
                    Max = _CurrentElement;

                return true;

            }

            return false;

        }

        #endregion

        #region SideEffect

        /// <summary>
        /// The sideeffect produced by this pipe.
        /// </summary>
        public Tuple<S, S> SideEffect
        {
            get
            {
                return new Tuple<S, S>(Min, Max);
            }
        }

        #endregion


        #region ToString()

        /// <summary>
        /// Returns a string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<Min: " + Min + ", Max: " + Max + ">";
        }

        #endregion

    }

}
