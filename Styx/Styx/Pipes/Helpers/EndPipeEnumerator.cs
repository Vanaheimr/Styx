/*
 * Copyright (c) 2010-2021 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Styx
{

    public static partial class IEndPipeExtentions
    {

        public static EndPipeEnumerator<E> AsEnumerator<E>(this IEndPipe<E> EndPipe)
        {
            return new EndPipeEnumerator<E>(EndPipe);
        }

    }

    public class EndPipeEnumerator<E> : IEnumerator<E>
    {

        private readonly IEndPipe<E> EndPipe;

        public EndPipeEnumerator(IEndPipe<E> EndPipe)
        {
            this.EndPipe = EndPipe;
        }

        public E Current
        {
            get
            {
                return EndPipe.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return EndPipe.Current;
            }
        }

        public Boolean MoveNext()
        {
            return EndPipe.MoveNext();
        }

        public void Reset()
        {
            EndPipe.Reset();
        }

        public void Dispose()
        {
            EndPipe.Dispose();
        }

    }

}