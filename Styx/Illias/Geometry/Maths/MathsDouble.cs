﻿/*
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Geometry.Maths
{

    /// <summary>
    /// Defining math operations on Double.
    /// </summary>
    public sealed class MathsDouble : IMaths<Double>
    {

        #region Singelton

        #region Static constructor

        // Explicit static constructor to tell the compiler
        // not to mark type as 'beforefieldinit'!
        static MathsDouble()
        { }

        #endregion

        #region Instance

        private static readonly IMaths<Double> _Instance = new MathsDouble();

        /// <summary>
        /// Return a singelton instance of this maths class.
        /// </summary>
        public static IMaths<Double> Instance
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
        public Double Zero
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
        public Double NegativeInfinity
        {
            get
            {
                return Double.NegativeInfinity;
            }
        }

        #endregion

        #region PositiveInfinity

        /// <summary>
        /// Return the positive infinity of this datatype.
        /// </summary>
        public Double PositiveInfinity
        {
            get
            {
                return Double.PositiveInfinity;
            }
        }

        #endregion


        #region Min(params Values)

        /// <summary>
        /// A method to get the minimum of an array of Doubles.
        /// </summary>
        /// <param name="Values">An array of Doubles.</param>
        /// <returns>The minimum of all values: Min(a, b, ...)</returns>
        public Double Min(params Double[] Values)
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
        /// A method to get the maximum of an array of Doubles.
        /// </summary>
        /// <param name="Values">An array of Doubles.</param>
        /// <returns>The maximum of all values: Min(a, b, ...)</returns>
        public Double Max(params Double[] Values)
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
        /// A method to add Doubles.
        /// </summary>
        /// <param name="Summands">An array of Doubles.</param>
        /// <returns>The addition of all summands: a + b + ...</returns>
        public Double Add(params Double[] Summands)
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
        /// A method to sub two Doubles.
        /// </summary>
        /// <param name="a">A Double.</param>
        /// <param name="b">A Double.</param>
        /// <returns>The subtraction of b from a: a - b</returns>
        public Double Sub(Double a, Double b)
        {
            return a - b;
        }

        #endregion

        #region Mul(params Multiplicators)

        /// <summary>
        /// A method to multiply Doubles.
        /// </summary>
        /// <param name="Multiplicators">An array of Doubles.</param>
        /// <returns>The multiplication of all multiplicators: a * b * ...</returns>
        public Double Mul(params Double[] Multiplicators)
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
        /// A method to multiply a Double by 2.
        /// </summary>
        /// <param name="a">A Double.</param>
        /// <returns>The multiplication of a by 2: 2*a</returns>
        public Double Mul2(Double a)
        {
            return 2 * a;
        }

        #endregion

        #region Div(a, b)

        /// <summary>
        /// A method to divide two Doubles.
        /// </summary>
        /// <param name="a">A Double.</param>
        /// <param name="b">A Double.</param>
        /// <returns>The division of a by b: a / b</returns>
        public Double Div(Double a, Double b)
        {
            return a / b;
        }

        #endregion

        #region Div2(a)

        /// <summary>
        /// A method to divide a Double by 2.
        /// </summary>
        /// <param name="a">A Double.</param>
        /// <returns>The division of a by 2: a / 2</returns>
        public Double Div2(Double a)
        {
            return a / 2;
        }

        #endregion

        #region Pow(a, b)

        /// <summary>
        /// A method to calculate a Double raised to the specified power.
        /// </summary>
        /// <param name="a">A Double.</param>
        /// <param name="b">A Double.</param>
        /// <returns>The value a raised to the specified power of b: a^b</returns>
        public Double Pow(Double a, Double b)
        {
            return Math.Pow(a, b);
        }

        #endregion


        #region Inv(a)

        /// <summary>
        /// A method to calculate the inverse value of the given Double.
        /// </summary>
        /// <param name="a">A Double.</param>
        /// <returns>The inverse value of a: -a</returns>
        public Double Inv(Double a)
        {
            return -a;
        }

        #endregion

        #region Abs(a)

        /// <summary>
        /// A method to calculate the absolute value of the given Double.
        /// </summary>
        /// <param name="a">A Double.</param>
        /// <returns>The absolute value of a: |a|</returns>
        public Double Abs(Double a)
        {
            return Math.Abs(a);
        }

        #endregion

        #region Sqrt(a)

        /// <summary>
        /// A method to calculate the square root of the Double.
        /// </summary>
        /// <param name="a">A Double.</param>
        /// <returns>The square root of a: Sqrt(a)</returns>
        public Double Sqrt(Double a)
        {
            return Math.Sqrt(a);
        }

        #endregion


        #region Distance(a, b)

        /// <summary>
        /// A method to calculate the distance between two Doubles.
        /// </summary>
        /// <param name="a">An object of type T</param>
        /// <param name="b">An object of type T</param>
        /// <returns>The distance between a and b.</returns>
        public Double Distance(Double a, Double b)
        {
            return Abs(Sub(a, b));
        }

        #endregion

    }

}
