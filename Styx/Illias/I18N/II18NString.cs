/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class II18NStringExtensions {

        public static JObject ToJSON(this II18NString? I18NString)

            => I18NString is null || I18NString.Count == 0
                   ? []
                   : new JObject(
                         I18NString.Select(text => new JProperty(
                                                       text.Language.ToString(),
                                                       text.Text
                                                   ))
                     );


        #region FirstText        (this I18NString)

        /// <summary>
        /// Return the first string of a multi-language string.
        /// </summary>
        public static String FirstText(this II18NString? I18NString)

            => I18NString.IsNotNullOrEmpty()
                   ? I18NString.First().Text
                   : String.Empty;

        #endregion

        #region IsNullOrEmpty    (this I18NString)

        /// <summary>
        /// The multi-language string is null or empty.
        /// </summary>
        public static Boolean IsNullOrEmpty([NotNullWhen(false)] this II18NString? I18NString)

            => I18NString is null ||
               I18NString.Count == 0;

        #endregion

        #region IsNotNullOrEmpty (this I18NString)

        /// <summary>
        /// The multi-language string is NOT null nor empty.
        /// </summary>
        public static Boolean IsNotNullOrEmpty([NotNullWhen(true)] this II18NString? I18NString)

            => I18NString is not null &&
               I18NString.Count != 0;

        #endregion

    }

    public interface II18NString : IEnumerable<I18NPair>
    {

        UInt32   Count { get; }
        Boolean  Has (Languages Language);
        String   this[Languages Language] { get; }

    }

}
