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
using System.Collections.Generic;

#endregion

namespace de.ahzf.Pipes
{

    /// <summary>
    /// A CollectionFilterPipe will take a collection of objects and
    /// a Filter.NOT_EQUAL or Filter.EQUAL argument.
    /// If an incoming object is contained (or not contained) in the
    /// provided collection, then it is emitted (or not emitted).
    /// </summary>
    /// <typeparam name="S">The type of the elements within the filter.</typeparam>
    public class CollectionFilterPipe<S> : AbstractPipe<S, S>, IFilterPipe<S>, IComparisonFilterPipe<S, S>
        where S : IEquatable<S>
    {

        #region Data

        private readonly ICollection<S> _StoredCollection;
        private readonly Filter         _Filter;

        #endregion

        #region Constructor(s)

        #region CollectionFilterPipe(myStoredCollection, myFilter)

        public CollectionFilterPipe(ICollection<S> myStoredCollection, Filter myFilter)
        {

            _StoredCollection = myStoredCollection;

            if (myFilter == Filter.NOT_EQUAL || myFilter == Filter.EQUAL)
                _Filter = myFilter;

            else
                throw new ArgumentOutOfRangeException("The only legal filters are equals and not equals");

        }

        #endregion

        #endregion


        public Boolean compareObjects(S myLeftObject, S myRightObject)
        {

            if (_Filter == Filter.NOT_EQUAL)
                if (_StoredCollection.Contains(myRightObject))
                    return true;

            else
                if (!_StoredCollection.Contains(myRightObject))
                    return true;

            return false;

        }


        private Boolean compareObjects(S myRightObject)
        {

            if (_Filter == Filter.NOT_EQUAL)
                if (_StoredCollection.Contains(myRightObject))
                    return true;

                else
                    if (!_StoredCollection.Contains(myRightObject))
                        return true;

            return false;

        }


        protected override S processNextStart()
        {
            while (true)
            {

                starts.MoveNext();
                var _S = starts.Current;

                if (compareObjects(_S))
                    return _S;

            }
        }

    }

}
