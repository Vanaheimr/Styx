/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    /// <summary>
    /// The MinMaxPipe produces two side effects which keep
    /// track of the Min and Max values of S.
    /// </summary>
    /// <typeparam name="S">The type of the consuming and emitting objects.</typeparam>
    public class MinMaxPipe<S> : AbstractTwoSideEffectsPipe<S, S, S, S>
        where S: IComparable, IComparable<S>, IEquatable<S>
    {

        #region Properties

        #region Min

        /// <summary>
        /// The minimum of the passed values.
        /// </summary>
        public S Min
        {
            get
            {
                return InternalSideEffect1;
            }
        }

        #endregion

        #region Max

        /// <summary>
        /// The maximum of the passed values.
        /// </summary>
        public S Max
        {
            get
            {
                return InternalSideEffect2;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region MinMaxFilter<S>(SourcePipe, Min, Max)

        /// <summary>
        /// The MinMaxPipe produces two side effects which keep
        /// track of the Min and Max values of S.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        /// <param name="Min">The initial minimum.</param>
        /// <param name="Max">The initial maximum.</param>
        public MinMaxPipe(IEndPipe<S> SourcePipe, S Min, S Max)
            : base(SourcePipe, Min, Max)
        {
            this.SideEffect1 = Min;
            this.SideEffect2 = Max;
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

            if (SourcePipe.MoveNext())
            {

                _CurrentElement = SourcePipe.Current;

                if (Min.CompareTo(_CurrentElement) > 0)
                    SideEffect1 = _CurrentElement;

                if (Max.CompareTo(_CurrentElement) < 0)
                    SideEffect2 = _CurrentElement;

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
            return base.ToString() + "<Min: " + Min + ", Max: " + Max + ">";
        }

        #endregion

    }

}
