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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Tag a struct, class or property as 'dynamic'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct|AttributeTargets.Class|AttributeTargets.Property,
                    AllowMultiple  = false,
                    Inherited      = true)]
    public class DynamicAttribute : Attribute
    {

        #region Tags

        private readonly String[] _Tags;

        /// <summary>
        /// Additional tags of the 'mandatory'-tag.
        /// </summary>
        public IEnumerable<String> Tags
        {
            get
            {
                return _Tags;
            }
        }

        #endregion

        /// <summary>
        /// Create a new 'dynamic'-tag having the given tags.
        /// </summary>
        /// <param name="Tags">Some tags.</param>
        public DynamicAttribute(params String[] Tags)
        {
            this._Tags = Tags?.Where(tag => !tag.IsNullOrEmpty()).ToArray();
        }

    }

}