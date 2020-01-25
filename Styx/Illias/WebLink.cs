/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A web link.
    /// </summary>
    public class WebLink
    {

        /// <summary>
        /// The text of the WebLink.
        /// </summary>
        public String Text { get; private set; }

        /// <summary>
        /// The URL of the WebLink.
        /// </summary>
        public String Url  { get; private set; }

        /// <summary>
        /// Create a new web link.
        /// </summary>
        /// <param name="Text">The text of the WebLink.</param>
        /// <param name="Url">The URL of the WebLink.</param>
        public WebLink(String Text, String Url)
        {
            this.Text = Text;
            this.Url  = Url;
        }

    }

}
