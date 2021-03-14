/*
 * Copyright (c) 2010-2021, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
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
using System.Text.RegularExpressions;
using System.Globalization;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{


    /// <summary>
    /// Convert to Radians.
    /// </summary>
    /// <param name="val">The value to convert to radians</param>
    /// <returns>The value in radians</returns>
    public static class NumericExtensions
    {

        public static Double ToRadians(this Double Value)
        {
            return Value * (Math.PI / 180);
        }

        public static Double ToRadians(this Latitude Latitude)
        {
            return Latitude.Value * (Math.PI / 180);
        }

        public static Double ToRadians(this Longitude Longitude)
        {
            return Longitude.Value * (Math.PI / 180);
        }


        public static Double ToDegree(this Double Value)
        {
            return Value * (180 / Math.PI);
        }

        public static Double ToDegree(this Latitude Latitude)
        {
            return Latitude.Value * (180 / Math.PI);
        }

        public static Double ToDegree(this Longitude Longitude)
        {
            return Longitude.Value * (180 / Math.PI);
        }

    }

}
