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

#region Usings

using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An Open Data signature.
    /// </summary>
    public class Signature23
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext  = "https://opendata.social/contexts/UsersAPI+json/signature";

        #endregion

        #region Properties

        /// <summary>
        /// The input format of the data to sign.
        /// </summary>
        [Mandatory]
        public String          InputFormat           { get; }

        /// <summary>
        /// The crypto algorithm used to calculate the signature.
        /// </summary>
        [Mandatory]
        public String          Algorithm             { get; }

        /// <summary>
        /// The output format of the signature.
        /// </summary>
        [Mandatory]
        public String          OutputFormat          { get; }

        /// <summary>
        /// The value of the signature.
        /// </summary>
        [Mandatory]
        public String          Value                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Open Data signature.
        /// </summary>
        /// <param name="InputFormat">The input format of the data to sign.</param>
        /// <param name="Algorithm">The crypto algorithm used to calculate the signature.</param>
        /// <param name="OutputFormat">The output format of the signature.</param>
        /// <param name="Value">The value of the signature.</param>
        public Signature23(String  InputFormat,
                         String  Algorithm,
                         String  OutputFormat,
                         String  Value)
        {

            this.InputFormat   = InputFormat;
            this.Algorithm     = Algorithm;
            this.OutputFormat  = OutputFormat;
            this.Value         = Value;

        }

        #endregion



        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out Signature23?  Signature,
                                       [NotNullWhen(false)] out String?     ErrorResponse)

            => TryParse(JSON,
                        out Signature,
                        out ErrorResponse,
                        null);

        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out Signature23?      Signature,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       CustomJObjectParserDelegate<Signature23>?  CustomChargingTicketParser   = null)
        {

            Signature = new Signature23("a", "b", "c", "d");
            ErrorResponse = null;

            return true;

        }

        #region ToJSON(Embedded = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        public JObject ToJSON(Boolean Embedded = false)

            => JSONObject.Create(

                   !Embedded
                       ? new JProperty("@context",  JSONLDContext.ToString())
                       : null,

                   new JProperty("inputFormat",   InputFormat),
                   new JProperty("algorithm",     Algorithm),
                   new JProperty("outputFormat",  OutputFormat),
                   new JProperty("value",         Value)

               );

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(InputFormat,  ":",
                             Algorithm,    ":",
                             OutputFormat, ":",
                             Value);

        #endregion

    }

}
