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

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Geometry.Maths
{

    /// <summary>
    /// Defining math operations on Int64.
    /// </summary>
    public class MathsInt64 : IMaths<Int64>
    {

        #region Singelton

        #region Static constructor

        // Explicit static constructor to tell the compiler
        // not to mark type as 'beforefieldinit'!
        static MathsInt64()
        { }

        #endregion

        #region Instance

        private static readonly IMaths<Int64> _Instance = new MathsInt64();

        /// <summary>
        /// Return a singelton instance of this maths class.
        /// </summary>
        public static IMaths<Int64> Instance
        {
            get
            {
                return _Instance;
            }
        }

        #endregion

        #endregion


        #region Zero

        /// <summary>
        /// Return the zero value of this datatype.
        /// </summary>
        public Int64 Zero
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region NegativeInfinity

        /// <summary>
        /// Return the negative infinity of this datatype.
        /// </summary>
        public Int64 NegativeInfinity
        {
            get
            {
                return Int64.MinValue;
            }
        }

        #endregion

        #region PositiveInfinity

        /// <summary>
        /// Return the positive infinity of this datatype.
        /// </summary>
        public Int64 PositiveInfinity
        {
            get
            {
                return Int64.MaxValue;
            }
        }

        #endregion


        #region Min(params Values)

        /// <summary>
        /// A method to get the minimum of an array of Int64s.
        /// </summary>
        /// <param name="Values">An array of Int64s.</param>
        /// <returns>The minimum of all values: Min(a, b, ...)</returns>
        public Int64 Min(params Int64[] Values)
        {

            if (Values == null)
                throw new ArgumentException("The given values must not be null!");

            if (Values.Length == 0)
                return 0;

            if (Values.Length == 1)
                return Values[0];

            var result = Values[0];

            for (var i = Values.Length - 1; i >= 1; i--)
                result = Math.Min(result, Values[i]);

            return result;

        }

        #endregion

        #region Max(params Values)

        /// <summary>
        /// A method to get the maximum of an array of Int64s.
        /// </summary>
        /// <param name="Values">An array of Int64s.</param>
        /// <returns>The maximum of all values: Min(a, b, ...)</returns>
        public Int64 Max(params Int64[] Values)
        {

            if (Values == null)
                throw new ArgumentException("The given values must not be null!");

            if (Values.Length == 0)
                return 0;

            if (Values.Length == 1)
                return Values[0];

            var result = Values[0];

            for (var i = Values.Length - 1; i >= 1; i--)
                result = Math.Max(result, Values[i]);

            return result;

        }

        #endregion


        #region Add(params Summands)

        /// <summary>
        /// A method to add Int64s.
        /// </summary>
        /// <param name="Summands">An array of Int64s.</param>
        /// <returns>The addition of all summands: a + b + ...</returns>
        public Int64 Add(params Int64[] Summands)
        {

            if (Summands == null)
                throw new ArgumentException("The given summands must not be null!");

            if (Summands.Length == 0)
                return 0;

            if (Summands.Length == 1)
                return Summands[0];

            var result = Summands[0];

            for (var i = Summands.Length - 1; i >= 1; i--)
                result += Summands[i];

            return result;

        }

        #endregion

        #region Sub(a, b)

        /// <summary>
        /// A method to sub two Int64s.
        /// </summary>
        /// <param name="a">A Int64.</param>
        /// <param name="b">A Int64.</param>
        /// <returns>The subtraction of b from a: a - b</returns>
        public Int64 Sub(Int64 a, Int64 b)
        {
            return a - b;
        }

        #endregion

        #region Mul(params Multiplicators)

        /// <summary>
        /// A method to multiply Int64s.
        /// </summary>
        /// <param name="Multiplicators">An array of Int64s.</param>
        /// <returns>The multiplication of all multiplicators: a * b * ...</returns>
        public Int64 Mul(params Int64[] Multiplicators)
        {

            if (Multiplicators == null)
                throw new ArgumentException("The given multiplicators must not be null!");

            if (Multiplicators.Length == 0)
                return 0;

            if (Multiplicators.Length == 1)
                return Multiplicators[0];

            var result = Multiplicators[0];

            for (var i = Multiplicators.Length - 1; i >= 1; i--)
                result *= Multiplicators[i];

            return result;

        }

        #endregion

        #region Mul2(a)

        /// <summary>
        /// A method to multiply an Int64 by 2.
        /// </summary>
        /// <param name="a">An Int64.</param>
        /// <returns>The multiplication of a by 2: 2*a</returns>
        public Int64 Mul2(Int64 a)
        {
            return 2 * a;
        }

        #endregion

        #region Div(a, b)

        /// <summary>
        /// A method to divide two Int64s.
        /// </summary>
        /// <param name="a">A Int64.</param>
        /// <param name="b">A Int64.</param>
        /// <returns>The division of a by b: a / b</returns>
        public Int64 Div(Int64 a, Int64 b)
        {
            return a / b;
        }

        #endregion

        #region Div2(a)

        /// <summary>
        /// A method to divide an Int64 by 2.
        /// </summary>
        /// <param name="a">A Int64.</param>
        /// <returns>The division of a by 2: a / 2</returns>
        public Int64 Div2(Int64 a)
        {
            return a / 2;
        }

        #endregion

        #region Pow(a, b)

        /// <summary>
        /// A method to calculate a Int64 raised to the specified power.
        /// </summary>
        /// <param name="a">A Int64.</param>
        /// <param name="b">A Int64.</param>
        /// <returns>The value a raised to the specified power of b: a^b</returns>
        public Int64 Pow(Int64 a, Int64 b)
        {
            return Convert.ToInt64(Math.Pow(a, b));
        }

        #endregion


        #region Inv(a)

        /// <summary>
        /// A method to calculate the inverse value of the given Int64.
        /// </summary>
        /// <param name="a">An Int64.</param>
        /// <returns>The inverse value of a: -a</returns>
        public Int64 Inv(Int64 a)
        {
            return -a;
        }

        #endregion

        #region Abs(a)

        /// <summary>
        /// A method to calculate the absolute value of the given Int64.
        /// </summary>
        /// <param name="a">An Int64.</param>
        /// <returns>The absolute value of a: |a|</returns>
        public Int64 Abs(Int64 a)
        {
            return Math.Abs(a);
        }

        #endregion

        #region Sqrt(a)

        /// <summary>
        /// A method to calculate the square root of the Int64.
        /// </summary>
        /// <param name="a">A Int64.</param>
        /// <returns>The square root of a: Sqrt(a)</returns>
        public Int64 Sqrt(Int64 a)
        {
            return Convert.ToInt64(Math.Round(Math.Sqrt(a)));
        }

        #endregion


        #region Distance(a, b)

        /// <summary>
        /// A method to calculate the distance between two Int64s.
        /// </summary>
        /// <param name="a">An object of type T</param>
        /// <param name="b">An object of type T</param>
        /// <returns>The distance between a and b.</returns>
        public Int64 Distance(Int64 a, Int64 b)
        {
            return Abs(Sub(a, b));
        }

        #endregion

    }

}
