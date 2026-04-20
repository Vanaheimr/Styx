/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Errors
    /// </summary>
    public static class Errors
    {

        #region (static) Create(Text, Context = null)

        /// <summary>
        /// Create an enumeration of errors based on the given error.
        /// </summary>
        /// <param name="Text">The text of the error.</param>
        /// <param name="Context">An optional context of the error.</param>
        public static IEnumerable<Error> Create(String   Text,
                                                Object?  Context  = null)

            => [
                    Error.Create(
                        I18NString.Create(Text),
                        Context
                    )
               ];

        #endregion

        #region (static) Create(Texts)

        /// <summary>
        /// Create an enumeration of errors based on the given enumeration of errors.
        /// </summary>
        /// <param name="Text">The enumeration of errors.</param>
        public static IEnumerable<Error> From(IEnumerable<String> Texts)

            => Texts.Select(text => Error.Create(text));

        #endregion

    }


    /// <summary>
    /// Extensions methods for errors.
    /// </summary>
    public static class ErrorsExtensions
    {

        #region IsNeitherNullNorEmpty (this Error)

        public static Boolean IsNeitherNullNorEmpty(this Error Error)

            => Error is not null &&
               Error.Text.IsNotNullOrEmpty();

        #endregion

        #region AddAndReturnList      (this Errors, Text)

        public static IList<Error> AddAndReturnList(this IList<Error>  Errors,
                                                      I18NString           Text)

            => Errors.AddAndReturnList(Error.Create(Text));

        #endregion

        #region ToJSON                (this Errors, CustomErrorSerializer = null)

        public static JArray ToJSON(this IEnumerable<Error>                  Errors,
                                    CustomJObjectSerializerDelegate<Error>?  CustomErrorSerializer   = null)

            => Errors.Any()
                   ? new JArray(Errors.Select(error => error.ToJSON(CustomErrorSerializer)))
                   : [];

        #endregion

        #region ToError               (this Text, Language = Languages.en)

        public static Error ToError(this String  Text,
                                        Languages    Language = Languages.en)

            => Error.Create(
                   Language,
                   Text
               );

        #endregion

    }


    /// <summary>
    /// An error.
    /// </summary>
    public class Error
    {

        #region Properties

        public I18NString  Text      { get; }

        public Object?     Context   { get; }

        #endregion

        #region Constructor(s)

        private Error(I18NString  Text,
                        Object?     Context  = null)
        {

            this.Text     = Text;
            this.Context  = Context;

        }

        #endregion


        #region (static) Parse   (JSON, CustomErrorParser = null)

        /// <summary>
        /// Parse the given JSON representation of a error.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomErrorParser">An optional delegate to parse custom error JSON objects.</param>
        public static Error Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<Error>?  CustomErrorParser   = null)
        {

            if (TryParse(JSON,
                         out var error,
                         out var errorResponse,
                         CustomErrorParser))
            {
                return error!;
            }

            throw new ArgumentException("The given JSON representation of a error is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Error, out ErrorResponse, CustomCDRParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a error.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Error">The parsed error.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       [NotNullWhen(true)]  out Error?  Error,
                                       [NotNullWhen(false)] out String?   ErrorResponse)

            => TryParse(JSON,
                        out Error,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a error.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Error">The parsed error.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomErrorParser">An optional delegate to parse custom error JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Error?      Error,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CustomJObjectParserDelegate<Error>?  CustomErrorParser   = null)
        {

            try
            {

                Error = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Text       [mandatory]

                if (!JSON.ParseMandatoryJSON("text",
                                             "error text",
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


                Error = new Error(
                              Text,
                              Context
                          );


                if (CustomErrorParser is not null)
                    Error = CustomErrorParser(JSON,
                                                  Error);

                return true;

            }
            catch (Exception e)
            {
                Error        = default;
                ErrorResponse  = "The given JSON representation of a error is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomErrorSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomErrorSerializer">A delegate to serialize custom error JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Error>?  CustomErrorSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("text",      Text.   ToJSON()),

                           Context is not null
                               ? new JProperty("context",   Context.ToString())
                               : null

                       );

            return CustomErrorSerializer is not null
                       ? CustomErrorSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) Create(          Text, Context = null)

        /// <summary>
        /// Create a new error.
        /// </summary>
        /// <param name="Text">The text of the error.</param>
        /// <param name="Context">An optional context of the error.</param>
        public static Error Create(String   Text,
                                   Object?  Context  = null)

            => new (I18NString.Create(Text),
                    Context);


        /// <summary>
        /// Create a new error.
        /// </summary>
        /// <param name="Text">The text of the error.</param>
        /// <param name="Context">An optional context of the error.</param>
        public static Error Create(I18NString  Text,
                                   Object?     Context  = null)

            => new (Text,
                    Context);

        #endregion

        #region (static) Create(Language, Text, Context = null)

        /// <summary>
        /// Create a new error.
        /// </summary>
        /// <param name="Language">The language of the error.</param>
        /// <param name="Text">The text of the error.</param>
        /// <param name="Context">An optional context of the error.</param>
        public static Error Create(Languages   Language,
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
