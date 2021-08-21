/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias.Collections;
using System;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    public static class ExceptPipeExtensions
    {

        public static ExceptPipe<S> Except<S>(this IEndPipe<S>      SourcePipe,
                                              S                     Value,
                                              IEqualityComparer<S>  EqualityComparer = null)
        {
            return new ExceptPipe<S>(SourcePipe, Value, EqualityComparer);
        }

        public static ExceptPipe<S> Except<S>(this IEndPipe<S>      FirstPipe,
                                              IEndPipe<S>           SecondPipe,
                                              IEqualityComparer<S>  EqualityComparer = null)
        {
            return new ExceptPipe<S>(FirstPipe, SecondPipe, EqualityComparer);
        }

        public static ExceptPipe<S> Except<S>(this IEndPipe<S>      SourcePipe,
                                              IEnumerable<S>        Enumeration,
                                              IEqualityComparer<S>  EqualityComparer = null)
        {
            return new ExceptPipe<S>(SourcePipe, Enumeration, EqualityComparer);
        }

    }

    public class ExceptPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>
    {

        #region Data

        private readonly IEqualityComparer<S>  EqualityComparer;
        private readonly HashedSet<S>          ValueSet;

        #endregion

        #region Constructor(s)

        #region ExceptPipe(SourcePipe, Value, EqualityComparer = null)

        /// <summary>
        /// Creates a new ExceptPipe.
        /// </summary>
        public ExceptPipe(IEndPipe<S> SourcePipe, S Value, IEqualityComparer<S> EqualityComparer = null)
            : base(SourcePipe)
        {
            this.EqualityComparer  = (EqualityComparer != null) ? EqualityComparer : EqualityComparer<S>.Default;
            this.ValueSet          = new HashedSet<S>() { Value };
        }

        #endregion

        #region ExceptPipe(FirstPipe, SecondPipe, EqualityComparer = null)

        /// <summary>
        /// Creates a new ExceptPipe.
        /// </summary>
        public ExceptPipe(IEndPipe<S> FirstPipe, IEndPipe<S> SecondPipe, IEqualityComparer<S> EqualityComparer = null)
            : base(FirstPipe)
        {
            this.EqualityComparer  = (EqualityComparer != null) ? EqualityComparer : EqualityComparer<S>.Default;
            this.ValueSet          = new HashedSet<S>(SecondPipe.ToList());
        }

        #endregion

        #region ExceptPipe(SourcePipe, Enumeration, EqualityComparer = null)

        /// <summary>
        /// Creates a new ExceptPipe.
        /// </summary>
        public ExceptPipe(IEndPipe<S> SourcePipe, IEnumerable<S> Enumeration, IEqualityComparer<S> EqualityComparer = null)
            : base(SourcePipe)
        {
            this.EqualityComparer  = (EqualityComparer != null) ? EqualityComparer : EqualityComparer<S>.Default;
            this.ValueSet          = new HashedSet<S>(Enumeration);
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

                if (!ValueSet.Contains(_CurrentElement))
                    return true;

            }

            return false;

        }

        #endregion

    }

}
