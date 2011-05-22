/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/Pipes.NET>
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

namespace de.ahzf.Pipes
{

    /// <summary>
    /// BackFilterPipe will fully process the object through its internal pipe.
    /// If the internal pipe yields results, then the original object is emitted
    /// from the BackFilterPipe.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class BackFilterPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>, IMetaPipe
    {

        private IStartPipe<S> _Pipe;

        public BackFilterPipe(IStartPipe<S> pipe)
        {
            _Pipe = pipe;
        }

        
        public override Boolean MoveNext()
        {

            if (_InternalEnumerator == null)
                return false;

            //while (true)
            //{
            //    S s = this.starts.next();
            //    this.pipe.setStarts(new SingleIterator<S>(s));
            //    if (pipe.hasNext())
            //    {
            //        while (pipe.hasNext())
            //        {
            //            pipe.next();
            //        }
            //        return s;
            //    }
            //}

            return false;

        }


        #region Pipes

        /// <summary>
        /// A MetaPipe is a pipe that "wraps" some collection of pipes.
        /// </summary>
        public IEnumerable<IPipe> Pipes
        {
            get
            {
                return new List<IPipe>() { _Pipe as IPipe };
            }
        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "[" + _Pipe + "]";
        }

        #endregion

    }

}
