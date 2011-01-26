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
    /// The IdEdgePipe will convert the given EdgeIds into the
    /// corresponding edges of the given graph.
    /// </summary>
    public class IdEdgePipe<S> : AbstractPipe<S, IEdge>
        where S : EdgeId
    {

        #region Data

        private readonly IGraph _IGraph;

        #endregion

        #region Constructor(s)

        #region IdEdgePipe(myIGraph)

        public IdEdgePipe(IGraph myIGraph)
        {
            _IGraph = myIGraph;
        }

        #endregion

        #endregion

        #region ProcessNextStart()

        protected override IEdge ProcessNextStart()
        {

            _Starts.MoveNext();
            return _IGraph.GetEdge(_Starts.Current);

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
