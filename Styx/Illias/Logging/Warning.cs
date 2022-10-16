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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions methods for warnings.
    /// </summary>
    public static class WarningsExtensions
    {

        public static Boolean IsNeitherNullNorEmpty(this Warning Warning)

            => Warning is not null &&
               Warning.Text.IsNeitherNullNorEmpty();

        public static IList<Warning> AddAndReturnList(this IList<Warning>  Warnings,
                                                      I18NString           Text)

            => Warnings.AddAndReturnList(Warning.Create(Text));

    }

    /// <summary>
    /// A warning.
    /// </summary>
    public class Warning
    {

        #region Properties

        public I18NString  Text      { get; }

        public Object?     Context   { get; }

        #endregion

        #region Constructor(s)

        private Warning(I18NString  Text,
                        Object?     Context  = null)
        {

            this.Text     = Text;
            this.Context  = Context;

        }

        #endregion


        #region ToJSON()

        public JObject ToJSON()

            => JSONObject.Create(

                   new JProperty("text",            Text.   ToJSON()),

                   Context is not null
                       ? new JProperty("context",   Context.ToString())
                       : null);

        #endregion


        #region Create(Text,           Context = null)

        public static Warning Create(I18NString  Text,
                                     Object?     Context  = null)

            => new (Text,
                    Context);

        #endregion

        #region Create(Language, Text, Context = null)

        public static Warning Create(Languages   Language,
                                     String      Text,
                                     Object?     Context  = null)

            => new (I18NString.Create(Language, Text),
                    Context);

        #endregion


        #region ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Text.FirstText();

        #endregion

    }

}
