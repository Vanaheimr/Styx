/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET
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

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The ObjectFilterPipe will either allow or disallow all objects that pass
    /// through it depending on the result of the compareObject() method.
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class ObjectFilterPipe<S> : AbstractComparisonFilterPipe<S,S>
        where S : IEquatable<S>
    {

        #region Data

        private readonly S _Object;

        #endregion

        #region Constructor(s)

        #region ObjectFilterPipe(myObject, myFilter)

        public ObjectFilterPipe(S myObject, Filter myFilter)
            : base(myFilter)
        {
            _Object = myObject;
        }

        #endregion

        #endregion


        protected override S processNextStart()
        {
            while (true)
            {

                starts.MoveNext();
                var _S = starts.Current;

                if (!this.compareObjects(_S, _Object))
                    return _S;

            }
        }


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "<" + _Object + ">";
        }

        #endregion

    }

}
