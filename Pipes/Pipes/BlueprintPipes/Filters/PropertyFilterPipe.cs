/*
 * Copyright (c) 2010-2011, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/pipes.NET>
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
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The PropertyFilterPipe either allows or disallows all Elements
    /// that have the provided value for a particular key.
    /// </summary>
    /// <typeparam name="TKey">The type of the property keys.</typeparam>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class PropertyFilterPipe<TId, TRevisionId, TKey, TValue, S, E>
                    : AbstractComparisonFilterPipe<S, E>

        where TId            : IEquatable<TId>,         IComparable<TId>,         IComparable, TValue
        where TRevisionId    : IEquatable<TRevisionId>, IComparable<TRevisionId>, IComparable, TValue
        where TKey           : IEquatable<TKey>,        IComparable<TKey>,        IComparable
        where S              : IPropertyElement<TId, TRevisionId, TKey, TValue>
        where E              : TValue, IComparable

    {

        #region Data

        private readonly TKey   _Key;
        private readonly TValue _Value;

        #endregion

        #region Constructor(s)

        #region PropertyFilterPipe(myKey, myValue, myComparisonFilter)

        /// <summary>
        /// Creates a new PropertyFilterPipe.
        /// </summary>
        /// <param name="myKey">The property key.</param>
        /// <param name="myValue">The property value.</param>
        /// <param name="myComparisonFilter">The filter to use.</param>
        public PropertyFilterPipe(TKey myKey, TValue myValue, ComparisonFilter myComparisonFilter)
            : base(myComparisonFilter)
        {
            _Key   = myKey;
            _Value = myValue;
        }

        #endregion

        #endregion


        #region MoveNext()

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next
        /// element; false if the enumerator has passed the end of the
        /// collection.
        /// </returns>
        public override Boolean MoveNext()
        {

            if (_InternalEnumerator == null)
                return false;

            while (true)
            {

                if (_InternalEnumerator.MoveNext())
                {

                    var _IElement = _InternalEnumerator.Current;

                    if (!CompareObjects((E) (Object) _IElement.Properties.GetProperty(_Key), (E) (Object) _Value))
                    {
                        _CurrentElement = _IElement;
                        return true;
                    }

                }

                else
                    return false;

            }
        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {
            return base.ToString() + "<" + _Key + "," + _Filter + "," + _Value + ">";
        }

        #endregion

    }

}
