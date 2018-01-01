/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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

namespace org.GraphDefined.Vanaheimr.Illias
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

        #region NewId(UniquenessCheckDelegate)

        /// <summary>
        /// Generate and return a new Id.
        /// </summary>
        /// <param name="UniquenessCheckDelegate">A delegate to check the uniqueness of the generated identification.</param>
        public UInt64 NewId(Func<UInt64, Boolean> UniquenessCheckDelegate)
        {

            UInt64 _NewLocalId;

            do
            {
                _NewLocalId = (UInt64) (Interlocked.Increment(ref _NewId) - 1);
            }
            while (!UniquenessCheckDelegate(_NewLocalId));

            return _NewLocalId;

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

        #region NewId(UniquenessCheck)

        /// <summary>
        /// Generate and return a new Id.
        /// </summary>
        /// <param name="UniquenessCheckDelegate">A delegate to check the uniqueness of the generated identification.</param>
        public String NewId(Func<String, Boolean> UniquenessCheckDelegate)
        {

            String _NewLocalId;

            do
            {
                _NewLocalId = ((UInt64) Interlocked.Increment(ref _NewId) - 1).ToString();
            }
            while (!UniquenessCheckDelegate(_NewLocalId));

            return  _NewLocalId;

        }

        #endregion

    }

    #endregion

}
