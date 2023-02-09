/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for select pipes.
    /// </summary>
    public static class SelectPipeExtensions
    {

        #region Select(this SourcePipe, SelectDelegate)

        public static SelectPipe<S, E> Select<S, E>(this IEndPipe<S>  SourcePipe,
                                                         Func<S, E>   SelectDelegate)
        {
            return new SelectPipe<S, E>(SourcePipe, SelectDelegate);
        }

        #endregion

        #region Select(this SourcePipe, CountedSelectDelegate)

        /// <summary>
        /// Starts with 1!
        /// </summary>
        public static SelectCountedPipe<S, E> Select<S, E>(this IEndPipe<S>    SourcePipe,
                                                           Func<S, UInt64, E>  CountedSelectDelegate)
        {
            return new SelectCountedPipe<S, E>(SourcePipe, CountedSelectDelegate);
        }

        #endregion

    }


    #region SelectPipe<S, E>

    /// <summary>
    /// Maps/converts the consuming objects to emitting objects.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class SelectPipe<S, E> : AbstractPipe<S, E>
    {

        #region Data

        private readonly Func<S, E>  SelectDelegate;

        #endregion

        #region Constructor(s)

        #region SelectPipe(SourceValue, SelectDelegate)

        public SelectPipe(S           SourceValue,
                          Func<S, E>  SelectDelegate)

            : base(SourceValue)

        {
            this.SelectDelegate  = SelectDelegate;
        }

        #endregion

        #region SelectPipe(SourcePipe, SelectDelegate)

        public SelectPipe(IEndPipe<S>  SourcePipe,
                          Func<S, E>   SelectDelegate)

            : base(SourcePipe)

        {
            this.SelectDelegate  = SelectDelegate;
        }

        #endregion

        #region SelectPipe(SourceEnumerator, SelectDelegate)

        public SelectPipe(IEnumerator<S>  SourceEnumerator,
                          Func<S, E>      SelectDelegate)

            : base(SourceEnumerator)

        {
            this.SelectDelegate  = SelectDelegate;
        }

        #endregion

        #region SelectPipe(SourceEnumerable, SelectDelegate)

        public SelectPipe(IEnumerable<S>  SourceEnumerable,
                          Func<S, E>      SelectDelegate)

            : base(SourceEnumerable)

        {
            this.SelectDelegate  = SelectDelegate;
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

                _CurrentElement = SelectDelegate(SourcePipe.Current);

                return true;

            }

            return false;

        }

        #endregion

    }

    #endregion

    #region SelectCountedPipe<S, E>

    /// <summary>
    /// Maps/converts the consuming objects to emitting objects.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class SelectCountedPipe<S, E> : AbstractSideEffectPipe<S, E, UInt64>
    {

        #region Data

        private readonly Func<S, UInt64, E>  CountedSelectDelegate;

        #endregion

        #region Constructor(s)

        #region SelectCountedPipe(SourceValue, CountedSelectDelegate)

        public SelectCountedPipe(S                   SourceValue,
                                 Func<S, UInt64, E>  CountedSelectDelegate)

            : base(SourceValue, 0)

        {
            this.CountedSelectDelegate  = CountedSelectDelegate;
        }

        #endregion

        #region SelectCountedPipe(SourcePipe, CountedSelectDelegate)

        public SelectCountedPipe(IEndPipe<S>         SourcePipe,
                                 Func<S, UInt64, E>  CountedSelectDelegate)

            : base(SourcePipe, 0)

        {
            this.CountedSelectDelegate  = CountedSelectDelegate;
        }

        #endregion

        #region SelectCountedPipe(SourceEnumerator, CountedSelectDelegate)

        public SelectCountedPipe(IEnumerator<S>      SourceEnumerator,
                                 Func<S, UInt64, E>  CountedSelectDelegate)

            : base(SourceEnumerator, 0)

        {
            this.CountedSelectDelegate  = CountedSelectDelegate;
        }

        #endregion

        #region SelectCountedPipe(SourceEnumerable, CountedSelectDelegate)

        public SelectCountedPipe(IEnumerable<S>      SourceEnumerable,
                                 Func<S, UInt64, E>  CountedSelectDelegate)

            : base(SourceEnumerable, 0)

        {
            this.CountedSelectDelegate  = CountedSelectDelegate;
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

                _CurrentElement = CountedSelectDelegate(SourcePipe.Current,
                                                        SideEffect++);

                return true;

            }

            return false;

        }

        #endregion

    }

    #endregion

}
