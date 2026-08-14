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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An exception thrown whenever CBOR data is malformed, violates the
    /// configured conformance rules, or a CBOR writer is used incorrectly.
    /// </summary>
    public class CBORException : Exception
    {

        #region CBORException(Message)

        /// <summary>
        /// Create a new CBOR exception.
        /// </summary>
        /// <param name="Message">A description of the error.</param>
        public CBORException(String Message)

            : base(Message)

        { }

        #endregion

        #region CBORException(Message, InnerException)

        /// <summary>
        /// Create a new CBOR exception.
        /// </summary>
        /// <param name="Message">A description of the error.</param>
        /// <param name="InnerException">The exception that caused this exception.</param>
        public CBORException(String     Message,
                             Exception  InnerException)

            : base(Message,
                   InnerException)

        { }

        #endregion

    }

}
