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
    /// Tag a struct, class or property as 'mandatory'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct|AttributeTargets.Class|AttributeTargets.Property,
                    AllowMultiple  = false,
                    Inherited      = true)]
    public class MandatoryAttribute : Attribute
    {

        #region Tags

        /// <summary>
        /// Additional tags of the 'mandatory'-tag.
        /// </summary>
        public IEnumerable<String>  Tags    { get; }

        #endregion

        /// <summary>
        /// Create a new 'mandatory'-tag having the given tags.
        /// </summary>
        /// <param name="Tags">Additional tags.</param>
        public MandatoryAttribute(params String[] Tags)
        {
            this.Tags = Tags?.Where(tag => tag.IsNotNullOrEmpty()).Distinct().ToArray() ?? Array.Empty<String>();
        }

    }

}
