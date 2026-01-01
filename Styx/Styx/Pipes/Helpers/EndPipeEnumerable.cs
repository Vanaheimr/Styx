/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    public static partial class IEndPipeExtensions
    {

        public static EndPipeEnumerable<E> AsEnumerable<E>(this IEndPipe<E> EndPipe)
        {
            return new EndPipeEnumerable<E>(EndPipe);
        }

    }

    public class EndPipeEnumerable<E> : IEnumerable<E>
    {

        private readonly IEndPipe<E> EndPipe;

        public EndPipeEnumerable(IEndPipe<E> EndPipe)
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
