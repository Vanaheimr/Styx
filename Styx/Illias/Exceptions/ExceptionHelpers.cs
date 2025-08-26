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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{


    /// <summary>
    /// A delegate called whenever an exception occured.
    /// </summary>
    public delegate void OnExceptionDelegate(DateTimeOffset Timestamp, Object Sender, Exception Exception);


    /// <summary>
    /// Some exception helpers.
    /// </summary>
    public static class ExceptionHelpers
    {

        #region CheckNull(Object, ObjectName)

        /// <summary>
        /// Checks if the given item is null. 
        /// </summary>
        /// <param name="Object">Any object.</param>
        /// <param name="ObjectName">The name of the object.</param>
        public static void CheckNull(this Object Object, String ObjectName)
        {

            if (Object is null)
                throw new ArgumentNullException(ObjectName, "The given '" + ObjectName + "' must not be null!");

        }

        #endregion

    }

}
