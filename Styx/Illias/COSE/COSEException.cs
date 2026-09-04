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
    /// An exception thrown whenever a COSE structure is malformed, uses an
    /// algorithm or a key this implementation does not support, or a COSE
    /// object is built incorrectly.
    /// A failed signature verification is NOT an exception: It is the
    /// ordinary, expected outcome of verifying untrusted data and therefore
    /// reported as a boolean result together with an error response.
    /// </summary>
    public class COSEException : Exception
    {

        #region COSEException(Message)

        /// <summary>
        /// Create a new COSE exception.
        /// </summary>
        /// <param name="Message">A description of the error.</param>
        public COSEException(String Message)

            : base(Message)

        { }

        #endregion

        #region COSEException(Message, InnerException)

        /// <summary>
        /// Create a new COSE exception.
        /// </summary>
        /// <param name="Message">A description of the error.</param>
        /// <param name="InnerException">The exception that caused this exception.</param>
        public COSEException(String     Message,
                             Exception  InnerException)

            : base(Message,
                   InnerException)

        { }

        #endregion

    }

}
