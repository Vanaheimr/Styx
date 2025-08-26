/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    /// <summary>
    /// The CountPipe produces a side effect that is the total
    /// number of objects that have passed through it.
    /// </summary>
    public class CountPipe<S> : AbstractSideEffectPipe<S, S, Int64>
    {

        #region Constructor(s)

        #region CountPipe(SourcePipe, InitialCounterValue = 0)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given pipe as element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="InitialCounterValue">An optional initial counter value.</param>
        public CountPipe(IEndPipe<S>  SourcePipe,
                         Int64        InitialCounterValue = 0)

            : base(SourcePipe, InitialCounterValue)

        { }

        #endregion

        #region CountPipe(SourceEnumerator, InitialCounterValue = 0)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerator as element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        /// <param name="InitialCounterValue">An optional initial counter value.</param>
        public CountPipe(IEnumerator<S>  SourceEnumerator,
                         Int64           InitialCounterValue = 0)

            : base(SourceEnumerator, InitialCounterValue)

        { }

        #endregion

        #region CountPipe(SourceEnumerable, InitialCounterValue = 0)

        /// <summary>
        /// Creates an new abstract side effect pipe using the given enumerable as element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        /// <param name="InitialCounterValue">An optional initial counter value.</param>
        public CountPipe(IEnumerable<S>  SourceEnumerable,
                         Int64           InitialCounterValue = 0)

            : base(SourceEnumerable, InitialCounterValue)
        { }

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

            if (SourcePipe is null)
                return false;

            if (SourcePipe.MoveNext())
            {
                _CurrentElement = SourcePipe.Current;
                Interlocked.Increment(ref InternalSideEffect);
                return true;
            }

            return false;

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + " <" + InternalSideEffect + ">";
        }

        #endregion

    }

}
