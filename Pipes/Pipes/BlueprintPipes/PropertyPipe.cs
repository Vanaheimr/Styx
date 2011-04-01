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
using System.Linq;
using System.Collections.Generic;

using de.ahzf.blueprints;
using System.Text;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// The PropertyPipe returns the property value of the
    /// Element identified by the provided key.
    /// </summary>
    /// <typeparam name="TKey">The type of the property keys.</typeparam>
    /// <typeparam name="S">The type of the consuming objects.</typeparam>
    /// <typeparam name="E">The type of the emitting objects.</typeparam>
    public class PropertyPipe<TId, TRevisionId, TKey, TValue, TDatastructure, S>

                    : AbstractPipe<S, TValue>
        
        where TId            : IEquatable<TId>,         IComparable<TId>,         IComparable, TValue
        where TRevisionId    : IEquatable<TRevisionId>, IComparable<TRevisionId>, IComparable, TValue
        where TKey           : IEquatable<TKey>,        IComparable<TKey>,        IComparable
        where TDatastructure : IDictionary<TKey, TValue>
        where S              : IPropertyElement<TId, TRevisionId, TKey, TValue, TDatastructure>

    {

        #region Data

        private readonly TKey[]            _Keys;
        private          IEnumerator<TKey> _PropertyEnumerator;

        #endregion

        #region Constructor(s)

        #region PropertyPipe(myKeys)

        /// <summary>
        /// Creates a new PropertyPipe.
        /// </summary>
        /// <param name="myKeys">The property keys.</param>
        public PropertyPipe(params TKey[] myKeys)
        {
            _Keys = myKeys;
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

                // First set the property enumerator
                if (_PropertyEnumerator == null)
                {

                    if (_InternalEnumerator.MoveNext())
                        _PropertyEnumerator = new List<TKey>(_Keys).GetEnumerator();

                    else
                        return false;

                }

                // Second emit the properties
                if (_PropertyEnumerator.MoveNext())
                {
                    _CurrentElement = (TValue) _InternalEnumerator.Current.Properties.GetProperty(_PropertyEnumerator.Current);
                    return true;
                }

                _PropertyEnumerator = null;

            }

        }

        #endregion


        #region ToString()

        /// <summary>
        /// A string representation of this pipe.
        /// </summary>
        public override String ToString()
        {

            var _StringBuilder = new StringBuilder();

            foreach (var _Key in _Keys)
                _StringBuilder.Append(_Key.ToString() + ", ");

            if (_StringBuilder.Length >= 2)
                _StringBuilder.Length = _StringBuilder.Length - 2;

            return base.ToString() + "<" + _StringBuilder.ToString() + ">";

        }

        #endregion

    }

}
