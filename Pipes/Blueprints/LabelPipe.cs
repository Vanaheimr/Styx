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

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The LabelPipe will return the label of the given edge.
    /// </summary>
    public class LabelPipe : AbstractPipe<IEdge, String>
    {

        #region ProcessNextStart()

        protected override String ProcessNextStart()
        {
            _Starts.MoveNext();
            return _Starts.Current.Label;
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
