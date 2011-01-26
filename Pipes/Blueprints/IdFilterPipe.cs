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

using de.ahzf.blueprints;
using de.ahzf.blueprints.Datastructures;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The IdPipe will return the Id of the given graph element.
    /// </summary>
    public class IdFilterPipe : AbstractComparisonFilterPipe<IElement, ElementId>
    {

        #region Data

        private readonly ElementId _ElementId;

        #endregion

        #region Constructor(s)

        #region IdFilterPipe(myElementId, myFilter)

        public IdFilterPipe(ElementId myElementId, FilterEnum myFilter)
            : base(myFilter)
        {
            _ElementId = myElementId;
        }

        #endregion

        #endregion

        #region ProcessNextStart()

        protected override IElement ProcessNextStart()
        {

            while (true)
            {

                _Starts.MoveNext();
                var _Element = _Starts.Current;

                if (!CompareObjects(_Element.Id, _ElementId))
                    return _Element;

            }

        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "<" + _Starts.Current + ">";
        }

        #endregion

    }

}
