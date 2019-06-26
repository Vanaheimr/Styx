﻿/*
 * Copyright (c) 2010-2019 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the JObject class.
    /// </summary>
    public static class JObjectExtensions
    {

        #region AddAndReturn(this JSON, Property)

        public static JObject AddAndReturn(this JObject JSON, JProperty Property)
        {

            JSON.Add(Property);

            return JSON;

        }

        #endregion

        #region AddFirstAndReturn(this JSON, Property)

        public static JObject AddFirstAndReturn(this JObject JSON, JProperty Property)
        {

            JSON.AddFirst(Property);

            return JSON;

        }

        #endregion

        #region IsJSONNullOrEmpty(this JSONToken)

        /// <summary>
        /// Checks whether the given JSON token is null or "null".
        /// </summary>
        /// <param name="JSONToken">A JSON token</param>
        public static Boolean IsJSONNullOrEmpty(this JToken JSONToken)
            => JSONToken == null || JSONToken.Value<String>().IsNullOrEmpty();

        #endregion

    }

}
