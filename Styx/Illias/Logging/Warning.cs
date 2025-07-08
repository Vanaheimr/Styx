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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Warnings
    /// </summary>
    public static class Warnings
    {

        #region (static) Create(Text,           Context = null)

        /// <summary>
        /// Create a new enumeration of warnings based on the given warning.
        /// </summary>
        /// <param name="Text">The text of the warning.</param>
        /// <param name="Context">An optional context of the warning.</param>
        public static IEnumerable<Warning> Create(String      Text,
                                                  Object?     Context  = null)

            => [
                    Warning.Create(
                        I18NString.Create(Text),
                        Context
                    )
               ];

        #endregion

    }


    /// <summary>
    /// Extensions methods for warnings.
    /// </summary>
    public static class WarningsExtensions
    {

        #region IsNeitherNullNorEmpty (this Warning)

        public static Boolean IsNeitherNullNorEmpty(this Warning Warning)

            => Warning is not null &&
               Warning.Text.IsNotNullOrEmpty();

        #endregion

        #region AddAndReturnList      (this Warnings, Text)

        public static IList<Warning> AddAndReturnList(this IList<Warning>  Warnings,
                                                      I18NString           Text)

            => Warnings.AddAndReturnList(Warning.Create(Text));

        #endregion

        #region ToJSON                (this Warnings, CustomWarningSerializer = null)

        public static JArray ToJSON(this IEnumerable<Warning>                  Warnings,
                                    CustomJObjectSerializerDelegate<Warning>?  CustomWarningSerializer   = null)

            => Warnings.Any()
                   ? new JArray(Warnings.Select(warning => warning.ToJSON(CustomWarningSerializer)))
                   : [];

        #endregion

        #region ToWarning             (this Text, Language = Languages.en)

        public static Warning ToWarning(this String  Text,
                                        Languages    Language = Languages.en)

            => Warning.Create(
                   Language,
                   Text
               );

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
        public static Boolean TryParse(JObject                            JSON,
                                       [NotNullWhen(true)]  out Warning?  Warning,
                                       [NotNullWhen(false)] out String?   ErrorResponse)

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
                                       [NotNullWhen(true)]  out Warning?      Warning,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
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


                Warning = new Warning(
                              Text,
                              Context
                          );


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


        #region (static) Create(          Text, Context = null)

        /// <summary>
        /// Create a new warning.
        /// </summary>
        /// <param name="Text">The text of the warning.</param>
        /// <param name="Context">An optional context of the warning.</param>
        public static Warning Create(String      Text,
                                     Object?     Context  = null)

            => new (I18NString.Create(Text),
                    Context);


        /// <summary>
        /// Create a new warning.
        /// </summary>
        /// <param name="Text">The text of the warning.</param>
        /// <param name="Context">An optional context of the warning.</param>
        public static Warning Create(I18NString  Text,
                                     Object?     Context  = null)

            => new (Text,
                    Context);

        #endregion

        #region (static) Create(Language, Text, Context = null)

        /// <summary>
        /// Create a new warning.
        /// </summary>
        /// <param name="Language">The language of the warning.</param>
        /// <param name="Text">The text of the warning.</param>
        /// <param name="Context">An optional context of the warning.</param>
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
