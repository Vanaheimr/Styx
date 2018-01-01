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
using System.Text;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for System.Random.
    /// </summary>
    public static class RandomExtensions
    {

        #region GetBytes(this _Random, NumberOfBytes)

        /// <summary>
        /// Get an array of random bytes.
        /// </summary>
        /// <param name="Random">The source of randomness.</param>
        /// <param name="NumberOfBytes">The number of random bytes to genrate.</param>
        public static Byte[] GetBytes(this Random Random, UInt16 NumberOfBytes)
        {

            var ByteArray = new Byte[NumberOfBytes];
            Random.NextBytes(ByteArray);

            return ByteArray;

        }

        #endregion

        #region GetString(this Random, Length)

        /// <summary>
        /// Get random string of the given length.
        /// </summary>
        /// <param name="Random">The source of randomness.</param>
        /// <param name="Length">The the length of the string.</param>
        public static String GetString(this Random Random, UInt16 Length)
        {

            var ByteArray = new Byte[Length];
            Random.NextBytes(ByteArray);

            for (var i = 0; i < ByteArray.Length; i++)
                ByteArray[i] = (Byte) (ByteArray[i] % 26 + 65);

            return Encoding.UTF8.GetString(ByteArray, 0, ByteArray.Length);

        }

        #endregion

        #region RandomString(this Random, Length)

        public static String RandomString(this Random Random, UInt16 Length)
        {

            if (Random == null)
                throw new ArgumentNullException(nameof(Random));

            var _StringBuilder = new StringBuilder();

            for (var i = 0; i < Length; i++)
                _StringBuilder.Append(Random.Next(10));

            return _StringBuilder.ToString();

        }

        #endregion

    }

}
