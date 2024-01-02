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

using System;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    /// <summary>
    /// Extension methods for where pipes.
    /// </summary>
    public static class WherePipeExtensions
    {

        #region Where(this SourcePipe, WhereDelegate)

        public static WherePipe<S> Where<S, E>(this IEndPipe<S>  SourcePipe,
                                               Predicate<S>      Include)
        {
            return new WherePipe<S>(SourcePipe, Include);
        }

        #endregion

        #region Where(this SourcePipe, CountedWhereDelegate)

        /// <summary>
        /// Starts with 1!
        /// </summary>
        public static WhereCountedPipe<S> Where<S, E>(this IEndPipe<S>     SourcePipe,
                                                      CountedPredicate<S>  CountedInclude)
        {
            return new WhereCountedPipe<S>(SourcePipe, CountedInclude);
        }

        #endregion

    }


    #region WherePipe<S>

    /// <summary>
    /// Maps/converts the consuming objects to emitting objects.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    public class WherePipe<S> : AbstractPipe<S, S>, IFilterPipe<S>
    {

        #region Data

        private readonly Predicate<S>  Include;

        #endregion

        #region Constructor(s)

        #region WherePipe(SourcePipe, Include)

        public WherePipe(IEndPipe<S>   SourcePipe,
                         Predicate<S>  Include)

            : base(SourcePipe)

        {
            this.Include  = Include;
        }

        #endregion

        #region WherePipe(SourceEnumerator, Include)

        public WherePipe(IEnumerator<S>  SourceEnumerator,
                         Predicate<S>    Include)

            : base(SourceEnumerator)

        {
            this.Include  = Include;
        }

        #endregion

        #region WherePipe(SourceEnumerable, Include)

        public WherePipe(IEnumerable<S>  SourceEnumerable,
                         Predicate<S>    Include)

            : base(SourceEnumerable)

        {
            this.Include  = Include;
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

            while (true)
            {

                if (SourcePipe.MoveNext())
                {

                    _CurrentElement = SourcePipe.Current;

                    if (Include(_CurrentElement))
                        return true;

                }

                else
                    return false;

            }

        }

        #endregion

    }

    #endregion

    #region WhereCountedPipe<S>

    /// <summary>
    /// Maps/converts the consuming objects to emitting objects.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    public class WhereCountedPipe<S> : AbstractSideEffectPipe<S, S, UInt64>, IFilterPipe<S>
    {

        #region Data

        private readonly CountedPredicate<S>  CountedInclude;

        #endregion

        #region Constructor(s)

        #region WhereCountedPipe(SourcePipe, CountedInclude)

        public WhereCountedPipe(IEndPipe<S>          SourcePipe,
                                CountedPredicate<S>  CountedInclude)

            : base(SourcePipe, 0)

        {
            this.CountedInclude  = CountedInclude;
        }

        #endregion

        #region WhereCountedPipe(SourceEnumerator, CountedInclude)

        public WhereCountedPipe(IEnumerator<S>       SourceEnumerator,
                                CountedPredicate<S>  CountedInclude)

            : base(SourceEnumerator, 0)

        {
            this.CountedInclude  = CountedInclude;
        }

        #endregion

        #region WhereCountedPipe(SourceEnumerable, CountedInclude)

        public WhereCountedPipe(IEnumerable<S>       SourceEnumerable,
                                CountedPredicate<S>  CountedInclude)

            : base(SourceEnumerable, 0)

        {
            this.CountedInclude  = CountedInclude;
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

                while (true)
                {

                    if (SourcePipe.MoveNext())
                    {

                        _CurrentElement = SourcePipe.Current;

                        if (CountedInclude(_CurrentElement, SideEffect++))
                            return true;

                    }

                    else
                        return false;

                }

            }

            return false;
        }

        #endregion

    }

    #endregion

}
