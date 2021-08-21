/*
 * Copyright (c) 2010-2021 Achim Friedland <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias.Collections
{

    #region PropertiesException

    /// <summary>
    /// An exception during property processing occurred!
    /// </summary>
    public class PropertiesException : Exception
    {

        /// <summary>
        /// An exception during property processing occurred!
        /// </summary>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public PropertiesException(String Message = null, Exception InnerException = null)
            : base(Message, InnerException)
        { }

    }

    #endregion


    #region IdentificationChangeException

    /// <summary>
    /// Changing the Id property is not allowed.
    /// </summary>
    public class IdentificationChangeException : PropertiesException
    {

        /// <summary>
        /// Throw a new IdentificationChangeException when
        /// someone tries to change the identification.
        /// </summary>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public IdentificationChangeException(String Message = null, Exception InnerException = null)
            : base("Changing the Id property is not allowed!" + Message, InnerException)
        { }

    }

    #endregion

    #region RevIdentificationChangeException

    /// <summary>
    /// Changing the RevId property is not allowed.
    /// </summary>
    public class RevIdentificationChangeException : PropertiesException
    {

        /// <summary>
        /// Throw a new IdentificationChangeException when
        /// someone tries to change the revision identification.
        /// </summary>
        /// <param name="Message">The message that describes the error.</param>
        /// <param name="InnerException">The exception that is the cause of the current exception.</param>
        public RevIdentificationChangeException(String Message = null, Exception InnerException = null)
            : base("Changing the RevId property is not allowed!" + Message, InnerException)
        { }

    }

    #endregion

}
