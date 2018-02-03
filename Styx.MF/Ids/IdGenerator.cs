/*
 * Copyright (c) 2010-2012 Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Illias Commons <http://www.github.com/ahzf/Illias>
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
using System.Threading;

#endregion

namespace de.ahzf.Illias.Commons
{

    #region IdGenerator_UInt64

    /// <summary>
    /// Generate a new Id.
    /// </summary>
    public class IdGenerator_UInt64 : IIdGenerator<UInt64>
    {

        #region Data

        private Int64 _NewId;

        #endregion

        #region NewId

        /// <summary>
        /// Generate and return a new Id.
        /// </summary>
        public UInt64 NewId
        {
            get
            {
                var _NewLocalId = Interlocked.Increment(ref _NewId);
                return (UInt64) _NewLocalId - 1;
            }
        }

        #endregion

    }

    #endregion

    #region IdGenerator_String

    /// <summary>
    /// Generate a new Id.
    /// </summary>
    public class IdGenerator_String : IIdGenerator<String>
    {

        #region Data

        private Int64 _NewId;

        #endregion

        #region NewId

        /// <summary>
        /// Generate and return a new Id.
        /// </summary>
        public String NewId
        {
            get
            {
                var _NewLocalId = Interlocked.Increment(ref _NewId);
                return ((UInt64) _NewLocalId - 1).ToString();
            }
        }

        #endregion

    }

    #endregion

}
