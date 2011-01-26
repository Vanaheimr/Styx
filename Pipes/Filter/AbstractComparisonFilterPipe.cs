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
    /// The AbstractComparisonFilterPipe provides the necessary functionality
    /// that is required of most ComparisonFilterPipe implementations.
    /// The compareObjects() implementation is useful for comparing two objects
    /// to determine if the current object in the pipe should be filtered.
    /// Depending on the type of ComparisonFilterPipe.Filter used, different
    /// types of comparisons are evaluated.
    /// </summary>
    public abstract class AbstractComparisonFilterPipe<S,T> : AbstractPipe<S, S>, IComparisonFilterPipe<S,T>
        where S : IEquatable<S>
    {

        #region Data

        protected readonly Filter _Filter;

        #endregion

        #region Constructor(s)

        #region AbstractComparisonFilterPipe(myFilter)

        public AbstractComparisonFilterPipe(Filter myFilter)
        {
            _Filter = myFilter;
        }

        #endregion

        #endregion


        public Boolean compareObjects(T myLeftObject, T myRightObject)
        {

            switch (_Filter)
            {

                case Filter.EQUAL:
                    if (null == myLeftObject)
                        return myRightObject == null;
                    return myLeftObject.Equals(myRightObject);

                case Filter.NOT_EQUAL:
                    if (null == myLeftObject)
                        return myRightObject != null;
                    return !myLeftObject.Equals(myRightObject);

                case Filter.GREATER_THAN:
                    if (null == myLeftObject || myRightObject == null)
                        return true;
                    return ((IComparable) myLeftObject).CompareTo(myRightObject) == 1;

                case Filter.LESS_THAN:
                    if (null == myLeftObject || myRightObject == null)
                        return true;
                    return ((IComparable)myLeftObject).CompareTo(myRightObject) == -1;

                case Filter.GREATER_THAN_EQUAL:
                    if (null == myLeftObject || myRightObject == null)
                        return true;
                    return ((IComparable)myLeftObject).CompareTo(myRightObject) >= 0;

                case Filter.LESS_THAN_EQUAL:
                    if (null == myLeftObject || myRightObject == null)
                        return true;
                    return ((IComparable)myLeftObject).CompareTo(myRightObject) <= 0;

                default:
                    throw new Exception("Invalid state as no valid filter was provided");
            
            }

        }


    }

}
