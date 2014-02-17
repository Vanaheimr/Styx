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
using System.Collections;
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.Styx
{

    public static partial class IEndPipeExtentions
    {

        public static IEndPipe2IEnumerable<E> AsEnumerable<E>(this IEndPipe<E> EndPipe)
        {
            return new IEndPipe2IEnumerable<E>(EndPipe);
        }

    }

    public class IEndPipe2IEnumerable<E> : IEnumerable<E>
    {

        private readonly IEndPipe<E> EndPipe;

        public IEndPipe2IEnumerable(IEndPipe<E> EndPipe)
        {
            this.EndPipe = EndPipe;
        }

        public IEnumerator<E> GetEnumerator()
        {
            return EndPipe.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EndPipe.GetEnumerator();
        }

    }

}