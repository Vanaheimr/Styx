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
    /// The LabelFilterPipe either allows or disallows all
    /// Edges that have the provided label.
    /// </summary>
    public class LabelFilterPipe : AbstractComparisonFilterPipe<IEdge, String>
    {

        #region Data

        private readonly String _Label;

        #endregion

        #region Constructor(s)

        #region LabelFilterPipe(myLabel, myFilter)

        public LabelFilterPipe(String myLabel, FilterEnum myFilter)
            : base(myFilter)
        {
            _Label = myLabel;
        }

        #endregion

        #endregion

        #region MoveNext()

        public override Boolean MoveNext()
        {
            while (true)
            {

                _Starts.MoveNext();
                var _Edge = _Starts.Current;

                if (!CompareObjects(_Edge.Label, _Label))
                {
                    _CurrentItem = _Edge;
                    return true;
                }

            }
        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "<" + _Filter + "," + _Label + ">";
        }

        #endregion

    }

}
