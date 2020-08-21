/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

        #region GetBytes          (this _Random, NumberOfBytes)

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

        #region RandomString      (this Random, Length)

        /// <summary>
        /// Get random string [a-zA-Z1-9]{Length} (without 'I', 'l', 'O', '0') of the given length.
        /// </summary>
        /// <param name="Random">The source of randomness.</param>
        /// <param name="Length">The expected length of the random string.</param>
        public static String RandomString(this Random Random, UInt16 Length)
        {

            var tryAgain  = false;
            var ByteArray = new Byte[Length];
            Random.NextBytes(ByteArray);


            for (var i = 0; i < Length; i++)
            {

                do
                {

                    tryAgain = false;

                    ByteArray[i] = (Byte) ((ByteArray[i] <= 85) // In 33% of all cases create 1-9!
                                               ? (ByteArray[i] %  9) + 49                                  // create 1-9
                                               : (ByteArray[i] % 26) + (ByteArray[i] % 2 == 0 ? 65 : 97)); // create A-Z or a-z

                    if (ByteArray[i] == 0x49 || // I
                        ByteArray[i] == 0x6C || // l
                        ByteArray[i] == 0x4F) { // O

                        ByteArray[i] = (Byte) Random.Next(256);
                        tryAgain     = true;

                    }

                } while (tryAgain);

            }

            return Encoding.UTF8.GetString(ByteArray, 0, Length);

        }

        #endregion

        #region RandomHexString   (this Random, Length)

        /// <summary>
        /// Get random string [A-F0-9]{Length} of the given length.
        /// </summary>
        /// <param name="Random">The source of randomness.</param>
        /// <param name="Length">The expected length of the random string.</param>
        public static String RandomHexString(this Random Random, UInt16 Length)
        {

            var byteArray = new Byte[Length / 2];
            Random.NextBytes(byteArray);

            return byteArray.ToHexString();

        }

        #endregion

        #region RandomNumberString(this Random, Length)

        /// <summary>
        /// Get random number as string [0-9]{Length} of the given length.
        /// </summary>
        /// <param name="Random">The source of randomness.</param>
        /// <param name="Length">The the length of the string.</param>
        public static String RandomNumberString(this Random Random, UInt16 Length)
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
