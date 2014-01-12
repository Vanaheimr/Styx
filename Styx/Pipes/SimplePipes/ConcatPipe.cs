﻿/*
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
using System.Linq;
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.Styx
{

    /// <summary>
    /// Extention methods for select pipes.
    /// </summary>
    public static class Select23PipeExtensions
    {

        #region Concat(this FirstPipe, params OtherPipes)

        public static ConcatPipe<S> Select<S, E>(this IEndPipe<S> FirstPipe,
                                                 params IEndPipe<S>[] OtherPipes)
        {
            return new ConcatPipe<S>(FirstPipe, OtherPipes);
        }

        #endregion

    }


    #region ConcatPipe<S>

    /// <summary>
    /// Maps/converts the consuming objects to emitting objects.
    /// </summary>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    public class ConcatPipe<S> : AbstractPipe<S, IEndPipe<S>, S>
    {

        #region Data

        private IEndPipe<S>  CurrentPipe;

        #endregion

        #region Constructor(s)

        #region ConcatPipe(FirstPipe, params OtherPipes)

        public ConcatPipe(IEndPipe<S>           FirstPipe,
                          params IEndPipe<S>[]  OtherPipes)

            : base(FirstPipe, new IEndPipeAdaptor<IEndPipe<S>>(OtherPipes))

        {
            this.CurrentPipe  = FirstPipe;
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

            while (true)
            {

                if (CurrentPipe == null)
                {

                    if (!SourcePipe2.MoveNext())
                        return false;

                    CurrentPipe = SourcePipe2.Current;

                }

                if (CurrentPipe.MoveNext())
                {
                    _CurrentElement = CurrentPipe.Current;
                    return true;
                }

                CurrentPipe = null;

            }

        }

        #endregion

    }

    #endregion

}
