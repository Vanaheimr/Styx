/*
 * Copyright (c) 2010-2014, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Collections;
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.Styx
{

    #region AbstractFilterPipe<S>

    /// <summary>
    /// An AbstractFilterPipe provides most of the functionality that is repeated
    /// in every instance of a FilterPipe. Any subclass of AbstractPipe should simply
    /// implement MoveNext().
    /// </summary>
    /// <typeparam name="S">The type of the filtered objects.</typeparam>
    public abstract class AbstractFilterPipe<S> : AbstractPipe<S, S>,
                                                  IFilterPipe<S>
    {

        #region (protected) AbstractFilterPipe()

        /// <summary>
        /// Creates a new abstract filter pipe.
        /// </summary>
        protected AbstractFilterPipe()
        { }

        #endregion

        #region AbstractFilterPipe(SourceElement)

        /// <summary>
        /// Creates an new abstract filter pipe using the given single value as element source.
        /// </summary>
        /// <param name="SourceElement">A single value as element source.</param>
        public AbstractFilterPipe(S SourceElement)
            : base(SourceElement)
        { }

        #endregion

        #region AbstractFilterPipe(SourcePipe)

        /// <summary>
        /// Creates an new abstract filter pipe using the given pipe as element source.
        /// </summary>
        /// <param name="SourcePipe">A pipe as element source.</param>
        public AbstractFilterPipe(IEndPipe<S> SourcePipe)
            : base(SourcePipe)
        { }

        #endregion

        #region AbstractFilterPipe(SourceEnumerator)

        /// <summary>
        /// Creates an new abstract filter pipe using the given enumerator as element source.
        /// </summary>
        /// <param name="SourceEnumerator">An enumerator as element source.</param>
        public AbstractFilterPipe(IEnumerator<S> SourceEnumerator)
            : base(SourceEnumerator)
        { }

        #endregion

        #region AbstractFilterPipe(SourceEnumerable)

        /// <summary>
        /// Creates an new abstract filter pipe using the given enumerable as element source.
        /// </summary>
        /// <param name="SourceEnumerable">An enumerable as element source.</param>
        public AbstractFilterPipe(IEnumerable<S> SourceEnumerable)
            : base(SourceEnumerable)
        { }

        #endregion

    }

    #endregion

}
