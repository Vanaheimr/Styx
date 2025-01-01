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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A value with its standard deviation.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="Value">The value.</param>
    /// <param name="StandardDeviation">The standard deviation of the value.</param>
    public readonly struct StdDev<T>(T Value,
                                     T StandardDeviation)

         where T : struct

    {

        #region Properties

        /// <summary>
        /// The value.
        /// </summary>
        public T Value              { get; } = Value;

        /// <summary>
        /// The standard deviation of the value.
        /// </summary>
        public T StandardDeviation  { get; } = StandardDeviation;

        #endregion


        public JArray ToJSON(String? Unit = null)

            => Unit.IsNotNullOrEmpty()
                   ? new(Value, StandardDeviation, Unit)
                   : new(Value, StandardDeviation);


        public JArray ToJSON(Func<T, Double> Mapper,
                             String? Unit = null)

            => Unit.IsNotNullOrEmpty()
                   ? new(Mapper(Value), Mapper(StandardDeviation), Unit)
                   : new(Mapper(Value), Mapper(StandardDeviation));



        public static Boolean TryParse(JArray JSON, out StdDev<T>? SSSSS, out String? ErrorResponse) //, Func<JToken, T> Parser)
        {
            SSSSS = null;
            ErrorResponse = null;

            if (JSON != null)
            {

                if (JSON.Count == 2)
                {

                    try
                    {

                        SSSSS = null;

                        //SSSSS = new StdDev<T>((T)JSON[0],
                        //                     (T)JSON[1]);

                        return true;

                    }
                    catch
                    { }

                }

            }

            return false;

        }

    }

}
