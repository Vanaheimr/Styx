﻿/*
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

        #region IsNeitherNullNorEmpty(this Warning)

        public static Boolean IsNeitherNullNorEmpty(this Warning Warning)

            => Warning is not null &&
               Warning.Text.IsNotNullOrEmpty();

        #endregion

        #region AddAndReturnList(this Warnings, Text)

        public static IList<Warning> AddAndReturnList(this IList<Warning>  Warnings,
                                                      I18NString           Text)

            => Warnings.AddAndReturnList(Warning.Create(Text));

        #endregion

        #region ToJSON(this Warnings, CustomWarningSerializer = null)

        public static JArray ToJSON(this IEnumerable<Warning>                  Warnings,
                                    CustomJObjectSerializerDelegate<Warning>?  CustomWarningSerializer   = null)

            => Warnings.Any()
                   ? new JArray(Warnings.Select(warning => warning.ToJSON(CustomWarningSerializer)))
                   : new JArray();

        #endregion

        #region ToWarning(this Text, Language = Languages.en)

        public static Warning ToWarning(this String  Text,
                                        Languages    Language = Languages.en)

            => Warning.Create(Language,
                              Text);

        #endregion

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


        #region (static) Parse   (JSON, CustomWarningParser = null)

        /// <summary>
        /// Parse the given JSON representation of a warning.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomWarningParser">An optional delegate to parse custom warning JSON objects.</param>
        public static Warning Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<Warning>?  CustomWarningParser   = null)
        {

            if (TryParse(JSON,
                         out var warning,
                         out var errorResponse,
                         CustomWarningParser))
            {
                return warning!;
            }

            throw new ArgumentException("The given JSON representation of a warning is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Warning, out ErrorResponse, CustomCDRParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a warning.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Warning">The parsed warning.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject       JSON,
                                       out Warning?  Warning,
                                       out String?   ErrorResponse)

            => TryParse(JSON,
                        out Warning,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a warning.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Warning">The parsed warning.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomWarningParser">An optional delegate to parse custom warning JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       out Warning?                           Warning,
                                       out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<Warning>?  CustomWarningParser   = null)
        {

            try
            {

                Warning = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Text       [mandatory]

                if (!JSON.ParseMandatoryJSON("text",
                                             "warning text",
                                             I18NString.TryParse,
                                             out I18NString? Text,
                                             out ErrorResponse) ||
                     Text is null)
                {
                    return false;
                }

                #endregion

                #region Parse Context    [optional]

                var Context = JSON.GetString("context");

                #endregion


                Warning = new Warning(Text,
                                      Context);


                if (CustomWarningParser is not null)
                    Warning = CustomWarningParser(JSON,
                                                  Warning);

                return true;

            }
            catch (Exception e)
            {
                Warning        = default;
                ErrorResponse  = "The given JSON representation of a warning is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomWarningSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomWarningSerializer">A delegate to serialize custom warning JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Warning>?  CustomWarningSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("text",      Text.   ToJSON()),

                           Context is not null
                               ? new JProperty("context",   Context.ToString())
                               : null

                       );

            return CustomWarningSerializer is not null
                       ? CustomWarningSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) Create(Text,           Context = null)

        public static Warning Create(String      Text,
                                     Object?     Context  = null)

            => new (I18NString.Create(Text),
                    Context);

        public static Warning Create(I18NString  Text,
                                     Object?     Context  = null)

            => new (Text,
                    Context);

        #endregion

        #region (static) Create(Language, Text, Context = null)

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
