﻿/*
 * Copyright (c) 2010-2021 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for enums.
    /// </summary>
    public static class EnumExtensions
    {

        public static T Set<T>(this Enum Type, T Value) where T : struct
        {
            return (T)(ValueType)(((int)(ValueType)Type | (int)(ValueType)Value));
        }

        public static T Unset<T>(this Enum Type, T Value) where T : struct
        {
            return (T)(ValueType)(((int)(ValueType)Type & ~(int)(ValueType)Value));
        }

        public static Boolean IsSet<T>(this Enum Type, T Value) where T : struct
        {
            return (((int)(ValueType)Type & (int)(ValueType)Value) == (int)(ValueType)Value);
        }

    }

}
