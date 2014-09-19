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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Styx
{

    public static partial class IEnumeratorExtentions
    {

        public static IEndPipeAdaptor<E> AsPipe<E>(this IEnumerator<E> Enumerator)
        {
            return new IEndPipeAdaptor<E>(Enumerator);
        }

    }

    public static partial class IEnumerableExtentions
    {

        public static IEndPipeAdaptor<E> AsPipe<E>(this IEnumerable<E> IEnumerable)
        {
            return new IEndPipeAdaptor<E>(IEnumerable.GetEnumerator());
        }

    }

    public static partial class ArrayExtentions
    {

        public static IEndPipeAdaptor<E> AsPipe<E>(this E[] Array)
        {
            return new IEndPipeAdaptor<E>(Array);
        }

    }

    public class IEndPipeAdaptor<E> : IEndPipe<E>
    {

        private readonly IEnumerator<E> Enumerator;

        public IEndPipeAdaptor(IEnumerator<E> Enumerator)
        {
            this.Enumerator = Enumerator;
        }

        public IEndPipeAdaptor(IEnumerable<E> IEnumerable)
        {
            this.Enumerator = IEnumerable.GetEnumerator();
        }

        public IEndPipeAdaptor(E[] Array)
        {
            this.Enumerator = Array.ToList().GetEnumerator();
        }

        public IEnumerator<E> GetEnumerator()
        {
            return Enumerator;
        }

        public E Current
        {
            get
            {
                return Enumerator.Current;
            }
        }

        public bool MoveNext()
        {
            return Enumerator.MoveNext();
        }

        public IEndPipe<E> Reset()
        {
            Enumerator.Reset();
            return this;
        }

        public IEnumerable<Object> Path
        {
            get
            {
                return new Object[1] { Enumerator };
            }
        }

        public void Dispose()
        {
            Enumerator.Dispose();
        }

    }

}