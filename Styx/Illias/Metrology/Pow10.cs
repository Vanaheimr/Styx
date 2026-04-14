/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for calculating powers of 10.
    /// </summary>
    public static class Pow10
    {

        /// <summary>
        /// Returns 10 raised to the specified exponent.
        /// </summary>
        /// <param name="exponent">The exponent to which 10 is raised. Must be between -28 and 28.</param>
        public static Decimal Calc(Int32 exponent)
        {

            if (exponent < -28 || exponent > 28)
                throw new ArgumentOutOfRangeException(
                          nameof(exponent),
                          exponent,
                          "The exponent must be between -28 and 28!"
                      );

            if (exponent == 0)
                return 1m;

            Decimal result = 1m;
            var absExponent = Math.Abs(exponent);

            for (var i = 0; i < absExponent; i++)
                result = (exponent > 0)
                             ? result * 10m
                             : result / 10m;

            return result;

        }

    }

}
