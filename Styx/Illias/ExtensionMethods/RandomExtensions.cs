/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

using System.Text;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for System.Random.
    /// </summary>
    public static class RandomExtensions
    {

#pragma warning disable SCS0005 // Weak random number generator.

        #region GetBytes          (NumberOfBytes)

        /// <summary>
        /// Get an array of random bytes.
        /// </summary>
        /// <param name="NumberOfBytes">The number of random bytes to genrate.</param>
        public static Byte[] GetBytes(UInt16 NumberOfBytes)
        {

            var ByteArray = new Byte[NumberOfBytes];
            Random.Shared.NextBytes(ByteArray);
            return ByteArray;

        }

        #endregion

        #region RandomUInt64      (MaxValue)

        /// <summary>
        /// Get a random UInt64.
        /// </summary>
        /// <param name="MaxValue">The optional exclusive upper bound of the random number to be generated. Its value must be greater than or equal to 0. If null then Int32.MaxValue will be used.</param>
        public static UInt64 RandomUInt64(Int32? MaxValue = null)

            => (UInt64) Random.Shared.Next(MaxValue ?? Int32.MaxValue);

        #endregion

        #region RandomString      (Length)

        /// <summary>
        /// Get random string [a-zA-Z1-9]{Length} (without 'I', 'l', 'O', '0') of the given length.
        /// </summary>
        /// <param name="Length">The expected length of the random string.</param>
        public static String RandomString(UInt16 Length)
        {

            var tryAgain  = false;
            var ByteArray = new Byte[Length];
            Random.Shared.NextBytes(ByteArray);

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

                        ByteArray[i] = (Byte) Random.Shared.Next(256);
                        tryAgain     = true;

                    }

                } while (tryAgain);

            }

            return Encoding.UTF8.GetString(ByteArray, 0, Length);

        }

        #endregion

        #region RandomHexString   (Length)

        /// <summary>
        /// Get random string [A-F0-9]{Length} of the given length.
        /// </summary>
        /// <param name="Length">The expected length of the random string.</param>
        public static String RandomHexString(UInt16 Length)
        {

            var byteArray = new Byte[Length / 2];
            Random.Shared.NextBytes(byteArray);

            return byteArray.ToHexString();

        }

        #endregion

        #region RandomNumberString(Length)

        /// <summary>
        /// Get random number as string [0-9]{Length} of the given length.
        /// </summary>
        /// <param name="Length">The the length of the string.</param>
        public static String RandomNumberString(UInt16 Length)
        {

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < Length; i++)
                stringBuilder.Append(Random.Shared.Next(10));

            return stringBuilder.ToString();

        }

        #endregion

#pragma warning restore SCS0005 // Weak random number generator.

    }

}
