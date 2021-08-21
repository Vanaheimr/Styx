/*
 * Copyright (c) 2014 Achim Friedland <achim.friedland@graphdefined.com>
 * This file is part of eMI3 WWCP <https://www.github.com/eMI3/WWCP-Bindings>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Xml.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class WarningsExtentions
    {

        public static Boolean IsNeitherNullNorEmpty(this Warning Warning)
            => Warning != null && Warning.Text.IsNeitherNullNorEmpty();

        public static IList<Warning> AddAndReturnList(this IList<Warning>  Warnings,
                                                      I18NString           Text)
            => Warnings?.AddAndReturnList(Warning.Create(Text));

    }

    public class Warning
    {

        public I18NString  Text      { get; }
        public Object      Context   { get; }

        private Warning(I18NString  Text,
                        Object      Context  = null)
        {

            this.Text     = Text;
            this.Context  = Context;

        }


        public JObject ToJSON()

            => JSONObject.Create(

                   new JProperty("text",            Text.   ToJSON()),

                   Context != null
                       ? new JProperty("context",   Context.ToString())
                       : null);


        public static Warning Create(I18NString  Text,
                                     Object      Warning  = null)

            => new Warning(Text,
                           Warning);


        public override String ToString()
            => Text.FirstText();

    }

}
