/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions methods for integers.
    /// </summary>
    public static class IntegerExtensions
    {

        #region Loop(this Loops, [Action] Do)

        /// <summary>
        /// Loop for the given number of iterations while
        /// calling the given delegate.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this Int32 Loops, Action Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0; i < Loops; i++)
                Do();

        }

        /// <summary>
        /// Loop for the given number of iterations while
        /// calling the given delegate.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this UInt32 Loops, Action Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0U; i < Loops; i++)
                Do();

        }

        /// <summary>
        /// Loop for the given number of iterations while
        /// calling the given delegate.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this Int64 Loops, Action Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0L; i < Loops; i++)
                Do();

        }

        /// <summary>
        /// Loop for the given number of iterations while
        /// calling the given delegate.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this UInt64 Loops, Action Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0UL; i < Loops; i++)
                Do();

        }

        #endregion

        #region Loop(this Loops, [Action<Integer>] Do)

        /// <summary>
        /// Loop for the given number of iterations while calling
        /// the given delegate with the current iteration as parameter.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this Int32 Loops, Action<Int32> Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0; i < Loops; i++)
                Do(i);

        }

        /// <summary>
        /// Loop for the given number of iterations while calling
        /// the given delegate with the current iteration as parameter.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this UInt32 Loops, Action<UInt32> Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0U; i < Loops; i++)
                Do(i);

        }

        /// <summary>
        /// Loop for the given number of iterations while calling
        /// the given delegate with the current iteration as parameter.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this Int64 Loops, Action<Int64> Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0L; i < Loops; i++)
                Do(i);

        }

        /// <summary>
        /// Loop for the given number of iterations while calling
        /// the given delegate with the current iteration as parameter.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this UInt64 Loops, Action<UInt64> Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0UL; i < Loops; i++)
                Do(i);

        }

        #endregion

        #region Loop(this Loops, [Action<Integer, Integer>] Do)

        /// <summary>
        /// Loop for the given number of iterations while calling
        /// the given delegate with the current iteration and
        /// total number of iterations as parameters.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this Int32 Loops, Action<Int32, Int32> Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0; i < Loops; i++)
                Do(i, Loops);

        }

        /// <summary>
        /// Loop for the given number of iterations while calling
        /// the given delegate with the current iteration and
        /// total number of iterations as parameters.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this UInt32 Loops, Action<UInt32, UInt32> Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0U; i < Loops; i++)
                Do(i, Loops);

        }

        /// <summary>
        /// Loop for the given number of iterations while calling
        /// the given delegate with the current iteration and
        /// total number of iterations as parameters.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this Int64 Loops, Action<Int64, Int64> Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0L; i < Loops; i++)
                Do(i, Loops);

        }

        /// <summary>
        /// Loop for the given number of iterations while calling
        /// the given delegate with the current iteration and
        /// total number of iterations as parameters.
        /// </summary>
        /// <param name="Loops">The number of iterations.</param>
        /// <param name="Do">A delegate to call.</param>
        public static void Loop(this UInt64 Loops, Action<UInt64, UInt64> Do)
        {

            if (Do == null)
                throw new ArgumentNullException("Do", "The parameter 'Do' must not be null!");

            for (var i = 0UL; i < Loops; i++)
                Do(i, Loops);

        }

        #endregion




        public static String ToBitString(this UInt64 value, UInt16 NumberOfBits = 64)
        {

            var sb = new StringBuilder(NumberOfBits);
            UInt64 lastBit = 1UL << (NumberOfBits - 1);

            for (int i = 0; i < NumberOfBits; i++)
            {
                if ((value & lastBit) == 0)
                    sb.Append('0');
                else
                    sb.Append('1');
                value <<= 1;
            }

            return sb.ToString();

        }


        public static String ToBitString(this Byte[] ByteArray, UInt16 NumberOfBits = 64)
        {

            var StringBuilder = new StringBuilder(NumberOfBits);

            foreach (var _byte in ByteArray)
            {

                var bitmask = 128;

                do
                {

                    if ((_byte & bitmask) == 0)
                        StringBuilder.Append('0');
                    else
                        StringBuilder.Append('1');

                    bitmask >>= 1;

                } while (bitmask != 0);

            }

            return StringBuilder.ToString();

        }

    }

}
