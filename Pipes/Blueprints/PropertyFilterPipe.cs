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
    /// The PropertyFilterPipe either allows or disallows all Elements
    /// that have the provided value for a particular key.
    /// </summary>
    public class PropertyFilterPipe<S, T> : AbstractComparisonFilterPipe<S, T>
        where S : IElement
    {

        #region Data

        private readonly String _Key;
        private readonly T      _Value;

        #endregion

        #region Constructor(s)

        #region PropertyFilterPipe(myKey, myValue, myFilter)

        public PropertyFilterPipe(String myKey, T myValue, FilterEnum myFilter)
            : base(myFilter)
        {
            _Key   = myKey;
            _Value = myValue;
        }

        #endregion

        #endregion

        #region MoveNext()

        public override Boolean MoveNext()
        {
            while (true)
            {
                
                _Starts.MoveNext();
                var _IElement = _Starts.Current;

                if (!CompareObjects(_IElement.GetProperty<T>(_Key), _Value))
                {
                    _CurrentItem = _IElement;
                    return true;
                }

            }
        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return base.ToString() + "<" + _Key + "," + _Filter + "," + _Value + ">";
        }

        #endregion

    }

}
