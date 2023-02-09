﻿/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Tag a struct, class or property as 'optional'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct|AttributeTargets.Class|AttributeTargets.Property,
                    AllowMultiple  = false,
                    Inherited      = true)]
    public class OptionalChoiceAttribute : Attribute
    {

        #region Tags

        /// <summary>
        /// The "choice" group.
        /// </summary>
        public String  Group    { get; }

        #endregion

        /// <summary>
        /// Create a new 'optional'-tag having the choice group.
        /// </summary>
        /// <param name="Group">The "choice" group.</param>
        public OptionalChoiceAttribute(String Group)
        {
            this.Group = Group;
        }

    }

}
