/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extensions to the TimeSpan class.
    /// </summary>
    public static partial class TimeSpanExtensions
    {

        #region TryParseMilliseconds (Text)

        /// <summary>
        /// Try to parse the given text representation of a milliseconds time span.
        /// </summary>
        /// <param name="Text">A text representation of a milliseconds time span.</param>
        public static TimeSpan? TryParseMilliseconds(String Text)
        {

            if (!UInt32.TryParse(Text, out var number))
                return null;

            return TimeSpan.FromMilliseconds(number);

        }

        #endregion

        #region TryParseSeconds      (Text)

        /// <summary>
        /// Try to parse the given text representation of a seconds time span.
        /// </summary>
        /// <param name="Text">A text representation of a seconds time span.</param>
        public static TimeSpan? TryParseSeconds(String Text)
        {

            if (!UInt32.TryParse(Text, out var number))
                return null;

            return TimeSpan.FromSeconds(number);

        }

        #endregion

        #region TryParseMinutes      (Text)

        /// <summary>
        /// Try to parse the given text representation of a minutes time span.
        /// </summary>
        /// <param name="Text">A text representation of a minutes time span.</param>
        public static TimeSpan? TryParseMinutes(String Text)
        {

            if (!UInt32.TryParse(Text, out var number))
                return null;

            return TimeSpan.FromMinutes(number);

        }

        #endregion


    }

}
