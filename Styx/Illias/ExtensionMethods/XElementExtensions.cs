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
using System.Text;
using System.Xml.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public delegate Boolean  TryXMLValueParser1  <TResult>(String    Input, [NotNullWhen(true)] out TResult?  Result);
    public delegate Boolean  TryXMLValueParser2  <TResult>(String    Input, [NotNullWhen(true)] out TResult?  Result, [NotNullWhen(false)] out String? ErrorResponse);

    public delegate Boolean  TryXMLElementParser1<TResult>(XElement  Input, [NotNullWhen(true)] out TResult?  Result);
    public delegate Boolean  TryXMLElementParser2<TResult>(XElement  Input, [NotNullWhen(true)] out TResult?  Result, [NotNullWhen(false)] out String? ErrorResponse);


    /// <summary>
    /// Extensions to the XElement class.
    /// </summary>
    public static class XElementExtensions
    {


        // TryParse Mandatory

        #region TryParseMandatory                   (ParentXElement, XName, Description, Parser,                      out Result,     out ErrorResponse)

        public static Boolean TryParseMandatory<T>(this XElement                     ParentXElement,
                                                   XName                             XName,
                                                   String                            Description,
                                                   Func<String, T>                   Parser,
                                                   [NotNullWhen(true)]  out T?       Result,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML element!";
                return false;
            }

            Result = Parser(xml.Value.Trim());

            if (Result is null)
            {
                ErrorResponse  = $"Invalid '{Description}' value '{xml.Value}'!";
                return false;
            }

            return true;

        }

        #endregion

        #region TryParseMandatory                   (ParentXElement, XName, Description, TryXML[Value|Element]Parser, out Result,     out ErrorResponse)

        public static Boolean TryParseMandatory<T>(this XElement                     ParentXElement,
                                                   XName                             XName,
                                                   String                            Description,
                                                   TryXMLValueParser1<T>             TryParser,
                                                   [NotNullWhen(true)]  out T?       Result,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML element!";
                return false;
            }

            if (!TryParser(xml.Value.Trim(), out Result))
            {
                ErrorResponse  = $"Invalid '{Description}' value '{xml.Value}'!";
                return false;
            }

            return true;

        }


        public static Boolean TryParseMandatory<T>(this XElement                     ParentXElement,
                                                   XName                             XName,
                                                   String                            Description,
                                                   TryXMLValueParser2<T>             TryParser,
                                                   [NotNullWhen(true)]  out T?       Result,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML element!";
                return false;
            }

            if (!TryParser(xml.Value.Trim(), out Result, out var errorResponse))
            {
                ErrorResponse = $"Invalid '{Description}' value '{xml.Value}': {errorResponse}";
                return false;
            }

            return true;

        }


        public static Boolean TryParseMandatory<T>(this XElement                     ParentXElement,
                                                   XName                             XName,
                                                   String                            Description,
                                                   TryXMLElementParser1<T>           TryParser,
                                                   [NotNullWhen(true)]  out T?       Result,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML element!";
                return false;
            }

            if (!TryParser(xml, out Result))
            {
                ErrorResponse  = $"Invalid '{Description}' XML element '{xml.Value}'!";
                return false;
            }

            return true;

        }


        public static Boolean TryParseMandatory<T>(this XElement                     ParentXElement,
                                                   XName                             XName,
                                                   String                            Description,
                                                   TryXMLElementParser2<T>           TryParser,
                                                   [NotNullWhen(true)]  out T?       Result,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML element!";
                return false;
            }

            if (!TryParser(xml, out Result, out var errorResponse))
            {
                ErrorResponse  = $"Invalid '{Description}' XML element '{xml.Value}': {errorResponse}";
                return false;
            }

            return true;

        }

        #endregion

        #region TryParseMandatoryBoolean            (ParentXElement, XName, Description,                              out Boolean,    out ErrorResponse)

        public static Boolean TryParseMandatoryBoolean(this XElement                      ParentXElement,
                                                       XName                              XName,
                                                       String                             Description,
                                                       [MaybeNullWhen(true)] out Boolean  Boolean,
                                                       [NotNullWhen(false)]  out String?  ErrorResponse)
        {

            Boolean        = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null)
            {

                if (xml.Value.Trim().Equals("true",  StringComparison.OrdinalIgnoreCase))
                {
                    Boolean = true;
                    return true;
                }

                if (xml.Value.Trim().Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    Boolean = false;
                    return true;
                }

                ErrorResponse = $"Invalid optional '{Description}' value '{xml.Value}'!";
                return false;

            }

            ErrorResponse = $"Missing required '{Description}' XML element!";
            return false;

        }

        #endregion

        #region TryParseMandatoryText               (ParentXElement, XName, Description,                              out Text,       out ErrorResponse)

        public static Boolean TryParseMandatoryText(this XElement                     ParentXElement,
                                                    XName                             XName,
                                                    String                            Description,
                                                    [NotNullWhen(true)]  out String?  Text,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Text         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML element!";
                return false;
            }

            if (xml.Value.IsNullOrEmpty())
            {
                ErrorResponse  = $"Invalid '{Description}' value '{xml.Value}'!";
                return false;
            }

            Text = xml.Value;

            return true;

        }

        #endregion

        #region TryParseMandatoryTimestamp          (ParentXElement, XName, Description,                              out Timestamp,  out ErrorResponse)

        public static Boolean TryParseMandatoryTimestamp(this XElement                      ParentXElement,
                                                         XName                              XName,
                                                         String                             Description,
                                                         [NotNullWhen(true)]  out DateTime  Timestamp,
                                                         [NotNullWhen(false)] out String?   ErrorResponse)
        {

            Timestamp         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML element!";
                return false;
            }

            if (!DateTime.TryParse(xml.Value, out var timestamp))
            {
                ErrorResponse  = $"Invalid '{Description}' value '{xml.Value}'!";
                return false;
            }

            if (timestamp.Kind != DateTimeKind.Utc)
                timestamp = timestamp.ToUniversalTime();

            Timestamp = timestamp;

            return true;

        }


        public static Boolean TryParseMandatoryTimestamp(this XElement                            ParentXElement,
                                                         XName                                    XName,
                                                         String                                   Description,
                                                         [NotNullWhen(true)]  out DateTimeOffset  Timestamp,
                                                         [NotNullWhen(false)] out String?         ErrorResponse)
        {

            Timestamp         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML element!";
                return false;
            }

            if (!DateTimeOffset.TryParse(xml.Value, out var timestamp))
            {
                ErrorResponse  = $"Invalid '{Description}' value '{xml.Value}'!";
                return false;
            }

            Timestamp = timestamp;

            return true;

        }

        #endregion

        #region TryParseMandatoryTimeSpan           (ParentXElement, XName, Description,                              out TimeSpan,   out ErrorResponse)

        public static Boolean TryParseMandatoryTimeSpan(this XElement                      ParentXElement,
                                                        XName                              XName,
                                                        String                             Description,
                                                        [NotNullWhen(true)]  out TimeSpan  TimeSpan,
                                                        [NotNullWhen(false)] out String?   ErrorResponse)
        {

            TimeSpan         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML element!";
                return false;
            }

            if (!UInt64.TryParse(xml.Value, out var seconds))
            {
                ErrorResponse  = $"Invalid '{Description}' value '{xml.Value}'!";
                return false;
            }

            TimeSpan = TimeSpan.FromSeconds(seconds);

            return true;

        }

        #endregion


        #region TryParseMandatoryAttribute          (ParentXElement, XName, Description, Parser,                      out Result,     out ErrorResponse)

        public static Boolean TryParseMandatoryAttribute<T>(this XElement                     ParentXElement,
                                                            XName                             XName,
                                                            String                            Description,
                                                            Func<String, T>                   Parser,
                                                            [NotNullWhen(true)]  out T?       Result,
                                                            [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var attribute = ParentXElement.Attribute(XName);
            if (attribute is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML attribute!";
                return false;
            }

            Result = Parser(attribute.Value.Trim());

            if (Result is null)
            {
                ErrorResponse  = $"Invalid '{Description}' value '{attribute.Value}'!";
                return false;
            }

            return true;

        }

        #endregion

        #region TryParseMandatoryAttribute          (ParentXElement, XName, Description, TryParser,                   out Result,     out ErrorResponse)

        public static Boolean TryParseMandatoryAttribute<T>(this XElement                     ParentXElement,
                                                            XName                             XName,
                                                            String                            Description,
                                                            TryXMLValueParser1<T>             TryParser,
                                                            [NotNullWhen(true)]  out T?       Result,
                                                            [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var attribute = ParentXElement.Attribute(XName);
            if (attribute is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML attribute!";
                return false;
            }

            if (!TryParser(attribute.Value.Trim(), out Result))
            {
                ErrorResponse  = $"Invalid '{Description}' value '{attribute.Value}'!";
                return false;
            }

            return true;

        }


        public static Boolean TryParseMandatoryAttribute<T>(this XElement                     ParentXElement,
                                                            XName                             XName,
                                                            String                            Description,
                                                            TryXMLValueParser2<T>             TryParser,
                                                            [NotNullWhen(true)]  out T?       Result,
                                                            [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Attribute(XName);
            if (xml is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML attribute!";
                return false;
            }

            if (!TryParser(xml.Value.Trim(), out Result, out var errorResponse))
            {
                ErrorResponse = $"Invalid '{Description}' value '{xml.Value}': {errorResponse}";
                return false;
            }

            return true;

        }

        #endregion

        #region TryParseMandatoryTextAttribute      (ParentXElement, XName, Description,                              out Text,       out ErrorResponse)

        public static Boolean TryParseMandatoryTextAttribute(this XElement                     ParentXElement,
                                                             XName                             XName,
                                                             String                            Description,
                                                             [NotNullWhen(true)]  out String?  Text,
                                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Text         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var attribute = ParentXElement.Attribute(XName);
            if (attribute is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML attribute!";
                return false;
            }

            if (attribute.Value.IsNullOrEmpty())
            {
                ErrorResponse  = $"Invalid '{Description}' value '{attribute.Value}'!";
                return false;
            }

            Text = attribute.Value;

            return true;

        }

        #endregion

        #region TryParseMandatoryTimestampAttribute (ParentXElement, XName, Description,                              out Timestamp,  out ErrorResponse)

        public static Boolean TryParseMandatoryTimestampAttribute(this XElement                      ParentXElement,
                                                                  XName                              XName,
                                                                  String                             Description,
                                                                  [NotNullWhen(true)]  out DateTime  Timestamp,
                                                                  [NotNullWhen(false)] out String?   ErrorResponse)
        {

            Timestamp      = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var attribute = ParentXElement.Attribute(XName);
            if (attribute is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML attribute!";
                return false;
            }

            if (!DateTime.TryParse(attribute.Value, out var timestamp))
            {
                ErrorResponse = $"Invalid '{Description}' value '{attribute.Value}'!";
                return false;
            }

            if (timestamp.Kind != DateTimeKind.Utc)
                timestamp = timestamp.ToUniversalTime();

            Timestamp = timestamp;

            return true;

        }


        public static Boolean TryParseMandatoryTimestampAttribute(this XElement                            ParentXElement,
                                                                  XName                                    XName,
                                                                  String                                   Description,
                                                                  [NotNullWhen(true)]  out DateTimeOffset  Timestamp,
                                                                  [NotNullWhen(false)] out String?         ErrorResponse)
        {

            Timestamp      = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var attribute = ParentXElement.Attribute(XName);
            if (attribute is null)
            {
                ErrorResponse  = $"Missing required '{Description}' XML attribute!";
                return false;
            }

            if (!DateTimeOffset.TryParse(attribute.Value, out var timestamp))
            {
                ErrorResponse = $"Invalid '{Description}' value '{attribute.Value}'!";
                return false;
            }

            Timestamp = timestamp;

            return true;

        }

        #endregion


        #region TryParseMandatoryElements           (ParentXElement, XName, Description, TryXML[Value|Element]Parser, out Results,    out ErrorResponse)

        public static Boolean TryParseMandatoryElements<T>(this XElement                            ParentXElement,
                                                           XName                                    XName,
                                                           String                                   Description,
                                                           TryXMLValueParser1<T>                    TryParser,
                                                           [NotNull]            out IEnumerable<T>  Results,
                                                           [NotNullWhen(false)] out String?         ErrorResponse)
        {

            Results        = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new T[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                if (TryParser(xml.Value, out var result))
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter}. '{Description}' value '{xml.Value}'!";
                    return false;
                }

                counter++;

            }

            Results = results;

            return results.Length > 0;

        }


        public static Boolean TryParseMandatoryElements<T>(this XElement                             ParentXElement,
                                                           XName                                     XName,
                                                           String                                    Description,
                                                           TryXMLValueParser2<T>                     TryParser,
                                                           [NotNull]            out IEnumerable<T>?  Results,
                                                           [NotNullWhen(false)] out String?          ErrorResponse)
        {

            Results        = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new T[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                if (TryParser(xml.Value, out var result, out var errorResponse))
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter}. '{Description}' value '{xml.Value}': {errorResponse}";
                    return false;
                }

                counter++;

            }

            Results = results;

            return results.Length > 0;

        }


        public static Boolean TryParseMandatoryElements<T>(this XElement                             ParentXElement,
                                                           XName                                     XName,
                                                           String                                    Description,
                                                           TryXMLElementParser1<T>                   TryParser,
                                                           [NotNull]            out IEnumerable<T>?  Results,
                                                           [NotNullWhen(false)] out String?          ErrorResponse)
        {

            Results        = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new T[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                if (TryParser(xml, out var result))
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter++}. '{Description}' XML element '{xml.Value}'!";
                    return false;
                }

                counter++;

            }

            Results = results;

            return results.Length > 0;

        }


        public static Boolean TryParseMandatoryElements<T>(this XElement                             ParentXElement,
                                                           XName                                     XName,
                                                           String                                    Description,
                                                           TryXMLElementParser2<T>                   TryParser,
                                                           [NotNull]            out IEnumerable<T>?  Results,
                                                           [NotNullWhen(false)] out String?          ErrorResponse)
        {

            Results        = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new T[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                if (TryParser(xml, out var result, out var errorResponse))
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter++}. '{Description}' XML element '{xml.Value}': {errorResponse}";
                    return false;
                }

                counter++;

            }

            Results = results;

            return results.Length > 0;

        }

        #endregion


        // TryParse Optional

        #region TryParseOptional                    (ParentXElement, XName, Description, Parser,                      out Result,     out ErrorResponse)

        public static Boolean TryParseOptional<T>(this XElement                      ParentXElement,
                                                  XName                              XName,
                                                  String                             Description,
                                                  Func<String, T>                    Parser,
                                                  [MaybeNullWhen(true)] out T?       Result,
                                                  [NotNullWhen(false)]  out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null)
            {
                Result         = Parser(xml.Value.Trim());
                ErrorResponse  = $"Invalid optional '{Description}' value '{xml.Value}'!";
                return false;
            }

            return true;

        }

        #endregion

        #region TryParseOptional                    (ParentXElement, XName, Description, TryXML[Value|Element]Parser, out Result,     out ErrorResponse)

        public static Boolean TryParseOptional<T>(this XElement                      ParentXElement,
                                                  XName                              XName,
                                                  String                             Description,
                                                  TryXMLValueParser1<T>              TryParser,
                                                  [MaybeNullWhen(true)] out T?       Result,
                                                  [NotNullWhen(false)]  out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null &&
                !TryParser(xml.Value.Trim(), out Result))
            {
                ErrorResponse  = $"Invalid optional '{Description}' value '{xml.Value}'!";
                return false;
            }

            return true;

        }


        public static Boolean TryParseOptional<T>(this XElement                      ParentXElement,
                                                  XName                              XName,
                                                  String                             Description,
                                                  TryXMLValueParser2<T>              TryParser,
                                                  [MaybeNullWhen(true)] out T?       Result,
                                                  [NotNullWhen(false)]  out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null &&
                !TryParser(xml.Value.Trim(), out Result, out var errorResponse))
            {
                ErrorResponse = $"Invalid optional '{Description}' value '{xml.Value}': {errorResponse}";
                return false;
            }

            return true;

        }


        public static Boolean TryParseOptional<T>(this XElement                      ParentXElement,
                                                  XName                              XName,
                                                  String                             Description,
                                                  TryXMLElementParser1<T>            TryParser,
                                                  [MaybeNullWhen(true)] out T?       Result,
                                                  [NotNullWhen(false)]  out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null &&
                !TryParser(xml, out Result))
            {
                ErrorResponse = $"Invalid optional '{Description}' XML element '{xml.Value}'!";
                return false;
            }

            return true;

        }


        public static Boolean TryParseOptional<T>(this XElement                      ParentXElement,
                                                  XName                              XName,
                                                  String                             Description,
                                                  TryXMLElementParser2<T>            TryParser,
                                                  [MaybeNullWhen(true)] out T?       Result,
                                                  [NotNullWhen(false)]  out String?  ErrorResponse)
        {

            Result         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null &&
                !TryParser(xml, out Result, out var errorResponse))
            {
                ErrorResponse = $"Invalid optional '{Description}' XML element '{xml.Value}': {errorResponse}";
                return false;
            }

            return true;

        }

        #endregion

        #region TryParseOptional                    (ParentXElement, XName, Description,                              out Boolean,    out ErrorResponse)

        public static Boolean TryParseOptional(this XElement                       ParentXElement,
                                               XName                               XName,
                                               String                              Description,
                                               [MaybeNullWhen(true)] out Boolean?  Boolean,
                                               [NotNullWhen(false)]  out String?   ErrorResponse)
        {

            Boolean        = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null)
            {

                if (xml.Value.Trim().Equals("true",  StringComparison.OrdinalIgnoreCase))
                {
                    Boolean = true;
                    return true;
                }

                if (xml.Value.Trim().Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    Boolean = false;
                    return true;
                }

                ErrorResponse = $"Invalid optional '{Description}' value '{xml.Value}'!";
                return false;

            }

            return true;

        }

        #endregion

        #region TryParseOptional                    (ParentXElement, XName, Description,                              out Double,     out ErrorResponse)

        public static Boolean TryParseOptional(this XElement                      ParentXElement,
                                               XName                              XName,
                                               String                             Description,
                                               [MaybeNullWhen(true)] out Double?  Double,
                                               [NotNullWhen(false)]  out String?  ErrorResponse)
        {

            Double         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null)
            {

                if (System.Double.TryParse(xml.Value, out var value))
                {
                    Double = value;
                    return true;
                }

                ErrorResponse = $"Invalid optional '{Description}' value '{xml.Value}'!";
                return false;

            }

            return true;

        }

        #endregion


        #region TryParseOptionalText                (ParentXElement, XName, Description,                              out Text,       out ErrorResponse)

        public static Boolean TryParseOptionalText(this XElement                      ParentXElement,
                                                   XName                              XName,
                                                   String                             Description,
                                                   [MaybeNullWhen(true)] out String?  Text,
                                                   [NotNullWhen(false)]  out String?  ErrorResponse)
        {

            Text           = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null)
            {

                Text = xml.Value.Trim();

                return Text.IsNotNullOrEmpty();

            }

            return true;

        }

        #endregion

        #region TryParseOptionalTimestamp           (ParentXElement, XName, Description,                              out Timestamp,  out ErrorResponse)

        public static Boolean TryParseOptionalTimestamp(this XElement                        ParentXElement,
                                                        XName                                XName,
                                                        String                               Description,
                                                        [MaybeNullWhen(true)] out DateTime?  Timestamp,
                                                        [NotNullWhen(false)]  out String?    ErrorResponse)
        {

            Timestamp      = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null)
            {

                if (!DateTime.TryParse(xml.Value, out var timestamp))
                {
                    ErrorResponse = $"Invalid '{Description}' xml element '{xml.Value}'!";
                    return false;
                }

                if (timestamp.Kind != DateTimeKind.Utc)
                    timestamp = timestamp.ToUniversalTime();

                Timestamp = timestamp;

            }

            return true;

        }


        public static Boolean TryParseOptionalTimestamp(this XElement                              ParentXElement,
                                                        XName                                      XName,
                                                        String                                     Description,
                                                        [MaybeNullWhen(true)] out DateTimeOffset?  Timestamp,
                                                        [NotNullWhen(false)]  out String?          ErrorResponse)
        {

            Timestamp      = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null)
            {

                if (!DateTimeOffset.TryParse(xml.Value, out var timestamp))
                {
                    ErrorResponse = $"Invalid '{Description}' xml element '{xml.Value}'!";
                    return false;
                }

                Timestamp = timestamp;

            }

            return true;

        }

        #endregion

        #region TryParseOptionalTimeSpan            (ParentXElement, XName, Description,                              out TimeSpan,   out ErrorResponse)

        public static Boolean TryParseOptionalTimeSpan(this XElement                        ParentXElement,
                                                       XName                                XName,
                                                       String                               Description,
                                                       [MaybeNullWhen(true)] out TimeSpan?  TimeSpan,
                                                       [NotNullWhen(false)]  out String?    ErrorResponse)
        {

            TimeSpan       = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xml = ParentXElement.Element(XName);
            if (xml is not null)
            {

                if (!Int64.TryParse(xml.Value, out var seconds))
                {
                    ErrorResponse = $"Invalid '{Description}' xml element '{xml.Value}'!";
                    return false;
                }

                TimeSpan = System.TimeSpan.FromSeconds(seconds);

            }

            return true;

        }

        #endregion

        #region TryParseOptionalTextAttribute       (ParentXElement, XName, Description,                              out Text,       out ErrorResponse)

        public static Boolean TryParseOptionalTextAttribute(this XElement                      ParentXElement,
                                                            XName                              XName,
                                                            String                             Description,
                                                            [MaybeNullWhen(true)] out String?  Text,
                                                            [NotNullWhen(false)]  out String?  ErrorResponse)
        {

            Text         = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var attribute = ParentXElement.Attribute(XName);
            if (attribute is not null)
            {

                Text = attribute.Value.Trim();

                return Text.IsNotNullOrEmpty();

            }

            return true;

        }

        #endregion

        #region TryParseOptionalTimestampAttribute  (ParentXElement, XName, Description,                              out Timestamp,  out ErrorResponse)

        public static Boolean TryParseOptionalTimestampAttribute(this XElement                        ParentXElement,
                                                                 XName                                XName,
                                                                 String                               Description,
                                                                 [MaybeNullWhen(true)] out DateTime?  Timestamp,
                                                                 [NotNullWhen(false)]  out String?    ErrorResponse)
        {

            Timestamp      = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var attribute = ParentXElement.Attribute(XName);
            if (attribute is not null)
            {

                if (!DateTime.TryParse(attribute.Value, out var timestamp))
                {
                    ErrorResponse = $"Invalid '{Description}' xml attribute '{attribute.Value}'!";
                    return false;
                }

                if (timestamp.Kind != DateTimeKind.Utc)
                    timestamp = timestamp.ToUniversalTime();

                Timestamp = timestamp;

            }

            return true;

        }


        public static Boolean TryParseOptionalTimestampAttribute(this XElement                              ParentXElement,
                                                                 XName                                      XName,
                                                                 String                                     Description,
                                                                 [MaybeNullWhen(true)] out DateTimeOffset?  Timestamp,
                                                                 [NotNullWhen(false)]  out String?          ErrorResponse)
        {

            Timestamp      = default;
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var attribute = ParentXElement.Attribute(XName);
            if (attribute is not null)
            {

                if (!DateTimeOffset.TryParse(attribute.Value, out var timestamp))
                {
                    ErrorResponse = $"Invalid '{Description}' xml attribute '{attribute.Value}'!";
                    return false;
                }

                Timestamp = timestamp;

            }

            return true;

        }

        #endregion


        #region TryParseOptionalElements            (ParentXElement, XName, Description, Parser,                      out Results,    out ErrorResponse)

        public static Boolean TryParseOptionalElements<T>(this XElement                            ParentXElement,
                                                          XName                                    XName,
                                                          String                                   Description,
                                                          Func<String, T>                          Parser,
                                                          [NotNull]            out IEnumerable<T>  Results,
                                                          [NotNullWhen(false)] out String?         ErrorResponse)
        {

            Results        = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new T[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                var result = Parser(xml.Value);

                if (result is not null)
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter}. '{Description}' value '{xml.Value}'!";
                    return false;
                }

                counter++;

            }

            Results = results;

            return true;

        }

        #endregion

        #region TryParseOptionalElements            (ParentXElement, XName, Description, TryXML[Value|Element]Parser, out Results,    out ErrorResponse)

        public static Boolean TryParseOptionalElements<T>(this XElement                            ParentXElement,
                                                          XName                                    XName,
                                                          String                                   Description,
                                                          TryXMLValueParser1<T>                    TryParser,
                                                          [NotNull]            out IEnumerable<T>  Results,
                                                          [NotNullWhen(false)] out String?         ErrorResponse)
        {

            Results        = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new T[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                if (TryParser(xml.Value, out var result))
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter}. '{Description}' value '{xml.Value}'!";
                    return false;
                }

                counter++;

            }

            Results = results;

            return true;

        }


        public static Boolean TryParseOptionalElements<T>(this XElement                             ParentXElement,
                                                          XName                                     XName,
                                                          String                                    Description,
                                                          TryXMLValueParser2<T>                     TryParser,
                                                          [NotNull]            out IEnumerable<T>?  Results,
                                                          [NotNullWhen(false)] out String?          ErrorResponse)
        {

            Results        = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new T[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                if (TryParser(xml.Value, out var result, out var errorResponse))
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter}. '{Description}' value '{xml.Value}': {errorResponse}";
                    return false;
                }

                counter++;

            }

            Results = results;

            return true;

        }


        public static Boolean TryParseOptionalElements<T>(this XElement                             ParentXElement,
                                                          XName                                     XName,
                                                          String                                    Description,
                                                          TryXMLElementParser1<T>                   TryParser,
                                                          [NotNull]            out IEnumerable<T>?  Results,
                                                          [NotNullWhen(false)] out String?          ErrorResponse)
        {

            Results        = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new T[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                if (TryParser(xml, out var result))
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter++}. '{Description}' XML element '{xml.Value}'!";
                    return false;
                }

                counter++;

            }

            Results = results;

            return true;

        }


        public static Boolean TryParseOptionalElements<T>(this XElement                             ParentXElement,
                                                          XName                                     XName,
                                                          String                                    Description,
                                                          TryXMLElementParser2<T>                   TryParser,
                                                          [NotNull]            out IEnumerable<T>?  Results,
                                                          [NotNullWhen(false)] out String?          ErrorResponse)
        {

            Results        = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new T[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                if (TryParser(xml, out var result, out var errorResponse))
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter++}. '{Description}' XML element '{xml.Value}': {errorResponse}";
                    return false;
                }

                counter++;

            }

            Results = results;

            return true;

        }

        #endregion

        #region TryParseOptionalTimestamps          (ParentXElement, XName, Description,                              out Timestamps, out ErrorResponse)

        public static Boolean TryParseOptionalTimestamps(this XElement                                         ParentXElement,
                                                         XName                                                 XName,
                                                         String                                                Description,
                                                         [NotNull]            out IEnumerable<DateTimeOffset>  Timestamps,
                                                         [NotNullWhen(false)] out String?                      ErrorResponse)
        {

            Timestamps     = [];
            ErrorResponse  = null;

            if (ParentXElement is null)
            {
                ErrorResponse = "The parent XML element must not be null!";
                return false;
            }

            var xmls     = ParentXElement.Elements(XName);
            var results  = new DateTimeOffset[xmls.Count()];
            var counter  = 1;

            foreach (var xml in xmls)
            {

                if (DateTimeOffset.TryParse(xml.Value, out var result))
                    results[counter-1] = result;
                else
                {
                    ErrorResponse = $"Invalid {counter}. '{Description}' value '{xml.Value}'!";
                    return false;
                }

                counter++;

            }

            Timestamps = results;

            return true;

        }

        #endregion


        // XML Elements

        #region ElementOrFail(ParentXElement, XName, Message = null)

        public static XElement ElementOrFail(this XElement  ParentXElement,
                                             XName          XName,
                                             String         Message = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new Exception(Message.IsNotNullOrEmpty() ? Message : "The parent XML element must not be null!");

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                throw new Exception(Message.IsNotNullOrEmpty() ? Message : "The XML element '" + XName.LocalName + "' was not found!");

            return _XElement;

        }

        #endregion

        #region ElementsOrFail(ParentXElement, XName, Message)

        public static IEnumerable<XElement> ElementsOrFail(this XElement  ParentXElement,
                                                           XName          XName,
                                                           String         Message = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new Exception(Message.IsNotNullOrEmpty() ? Message : "The parent XML element must not be null!");

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                throw new Exception(Message.IsNotNullOrEmpty() ? Message : "No XML element '" + XName.LocalName + "' could be found!");

            return _XElements;

        }

        #endregion

        #region IfElementIsNotNullOrEmpty(ParentXElement, XName, ElementAction)

        public static void IfElementIsNotNullOrEmpty(this XElement     ParentXElement,
                                                     XName             XName,
                                                     Action<XElement>  ElementAction)
        {

            #region Initial checks

            if (ParentXElement is null || ElementAction is null)
                return;

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                return;

            if (_XElement is not null)
                ElementAction(_XElement);

        }

        #endregion


        #region ElementValueOrDefault(this ParentXElement, XName, DefaultValue = null)

        /// <summary>
        /// Return the value of the first (in document order) child element with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent XML element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static String ElementValueOrDefault(this XElement  ParentXElement,
                                                   XName          XName,
                                                   String         DefaultValue = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                return DefaultValue;

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                return DefaultValue;

            return _XElement.Value ?? DefaultValue;

        }

        #endregion

        #region ElementValueOrDefault(this ParentXElement, NestedXName, XName, DefaultValue = null)

        /// <summary>
        /// Return the value of the first (in document order) child element with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent XML element.</param>
        /// <param name="NestedXName">The nested System.Xml.Linq.XName.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static String ElementValueOrDefault(this XElement  ParentXElement,
                                                   XName          NestedXName,
                                                   XName          XName,
                                                   String         DefaultValue = null)
        {

            if (ParentXElement is null)
                return DefaultValue;

            var _XElement1 = ParentXElement.Element(XName);

            if (_XElement1 is null)
                return DefaultValue;

            var _XElement2 = _XElement1.Element(NestedXName);

            if (_XElement2 is null)
                return DefaultValue;

            return _XElement2.Value;

        }

        #endregion

        #region ElementValueOrFail(this ParentXElement, XName, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) child element with the
        /// specified System.Xml.Linq.XName or throw an optional exception.
        /// </summary>
        /// <param name="ParentXElement">The XML parent XML element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static String ElementValueOrFail(this XElement  ParentXElement,
                                                XName          XName,
                                                String         ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new Exception(ExceptionMessage);

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
            {

                if (ExceptionMessage.IsNotNullOrEmpty())
                    throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "Missing XML element '" + XName.LocalName + "'!");

                return null;

            }

            return _XElement.Value;

        }

        #endregion

        #region IfValueIsNotNullOrEmpty(ParentXElement, XName, ValueAction)

        public static void IfValueIsNotNullOrEmpty(this XElement   ParentXElement,
                                                   XName           XName,
                                                   Action<String>  ValueAction)
        {

            #region Initial checks

            if (ParentXElement is null || ValueAction is null)
                return;

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                return;

            if (_XElement.Value.IsNotNullOrEmpty())
                ValueAction(_XElement.Value);

        }

        #endregion


        #region ElementValues(ParentXElement, XName)

        public static IEnumerable<String> ElementValues(this XElement  ParentXElement,
                                                        XName          XName)
        {

            #region Initial checks

            if (ParentXElement is null)
                return new String[0];

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                return new String[0];

            return _XElements.Select(xelement => xelement.Value);

        }

        #endregion



        // Map XML Element

        #region MapElement          (ParentXElement, XName, Mapper, Default = default(T))

        public static T? MapElement<T>(this XElement      ParentXElement,
                                       XName              XName,
                                       Func<XElement, T>  Mapper,
                                       T?                 Default   = default)
        {

            if (ParentXElement is null || Mapper is null)
                return Default;

            var xElement = ParentXElement.Element(XName);

            if (xElement is null)
                return Default;

            return Mapper(xElement);

        }

        public static T? MapElement<T>(this XElement                           ParentXElement,
                                       XName                                   XName,
                                       Func<XElement, OnExceptionDelegate, T>  Mapper,
                                       OnExceptionDelegate?                    OnException   = null,
                                       T?                                      Default       = default)
        {

            if (ParentXElement is null || Mapper is null)
                return Default;

            var xElement = ParentXElement.Element(XName);

            if (xElement is null)
                return Default;

            return Mapper(xElement, OnException);

        }

        #endregion

        #region MapElementOrFail    (ParentXElement, XName, Mapper, OnException = null, ExceptionMessage = null)

        public static T MapElementOrFail<T>(this XElement                           ParentXElement,
                                            XName                                   XName,
                                            Func<XElement, OnExceptionDelegate, T>  Mapper,
                                            OnExceptionDelegate?                    OnException        = null,
                                            String?                                 ExceptionMessage   = null)
        {

            if (ParentXElement is null)
                if (OnException is not null)
                    OnException(Timestamp.Now, ParentXElement, new Exception("The parent XML element must not be null!"));
                else
                    throw new Exception("The parent XML element must not be null!");

            if (Mapper is null)
                if (OnException is not null)
                    OnException(Timestamp.Now, ParentXElement, new Exception("Mapper delegate must not be null!"));
                else
                    throw new Exception("Mapper delegate must not be null!");

            var xElement = ParentXElement.Element(XName);

            if (xElement is null)
                if (OnException is not null)
                    OnException(Timestamp.Now, ParentXElement, new Exception(ExceptionMessage));
                else
                    throw new Exception(ExceptionMessage);

            return Mapper(xElement, OnException);

        }


        public static T MapElementOrFail2<T>(this XElement      ParentXElement,
                                             XName              XName,
                                             Func<XElement, T>  Mapper)
        {

            if (ParentXElement is null)
                throw new Exception("The parent XML element must not be null!");

            if (Mapper is null)
                throw new Exception("Mapper delegate must not be null!");

            var xElement = ParentXElement.Element(XName)
                             ?? throw new Exception("The XElement must not be null!");

            return Mapper(xElement);

        }


        public static Boolean MapElementOrFail<T>(this XElement      ParentXElement,
                                                  XName              XName,
                                                  Func<XElement, T>  Mapper,
                                                  out T?             Element,
                                                  out String?        ErrorResponse)
        {

            if (ParentXElement is null)
            {
                Element        = default;
                ErrorResponse  = "The parent XML element must not be null!";
                return false;
            }

            if (Mapper is null)
            {
                Element        = default;
                ErrorResponse  = "The mapper delegate must not be null!";
                return false;
            }

            var xElement = ParentXElement.Element(XName);

            if (xElement is null)
            {
                Element        = default;
                ErrorResponse  = "The XML element was not found!";
                return false;
            }

            Element        = Mapper(xElement);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region MapElementOrNullable(ParentXElement, XName, Mapper, Default = default(T))

        public static T? MapElementOrNullable<T>(this XElement      ParentXElement,
                                                 XName              XName,
                                                 Func<XElement, T>  Mapper)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null || Mapper is null)
                return new T?();

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                return new T?();

            return Mapper(_XElement);

        }

        public static T? MapElementOrNullable<T>(this XElement                           ParentXElement,
                                                 XName                                   XName,
                                                 Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                 OnExceptionDelegate                     OnException = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null || Mapper is null)
                return new T?();

            #endregion

            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                return new T?();

            return Mapper(_XElement, OnException);

        }

        #endregion


        // Map XML Elements

        #region MapElements      (ParentXElement, XName, Mapper, OnException = null)

        public static IEnumerable<T> MapElements<T>(this XElement        ParentXElement,
                                                    XName                XName,
                                                    Func<XElement, T>    Mapper,
                                                    OnExceptionDelegate  OnException = null)
        {

            #region Initial checks

            if (ParentXElement is null || Mapper is null)
                return new T[0];

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                return new T[0];

            return _XElements.Select(XML   => Mapper(XML)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default(T)));

        }

        public static IEnumerable<T> MapElements<T>(this XElement                           ParentXElement,
                                                    XName                                   XName,
                                                    Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                    OnExceptionDelegate                     OnException = null)
        {

            #region Initial checks

            if (ParentXElement is null || Mapper is null)
                return new T[0];

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                return new T[0];

            return _XElements.Select(XML   => Mapper(XML, OnException)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default(T)));

        }

        #endregion

        #region MapElements      (ParentXElement, XWrapper, XName, Mapper, OnException = null)

        public static IEnumerable<T> MapElements<T>(this XElement        ParentXElement,
                                                    XName                XWrapper,
                                                    XName                XName,
                                                    Func<XElement, T>    Mapper,
                                                    OnExceptionDelegate  OnException = null)
        {

            #region Initial checks

            if (ParentXElement is null || Mapper is null)
                return new T[0];

            #endregion

            var _XElement  = ParentXElement.Element(XWrapper);
            if (_XElement is null)
                return new T[0];

            var _XElements = _XElement.Elements(XName);
            if (_XElements is null || !_XElements.Any())
                return new T[0];

            return _XElements.Select(XML   => Mapper(XML)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default(T)));

        }

        public static IEnumerable<T> MapElements<T>(this XElement                           ParentXElement,
                                                    XName                                   XWrapper,
                                                    XName                                   XName,
                                                    Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                    OnExceptionDelegate                     OnException = null)
        {

            #region Initial checks

            if (ParentXElement is null || Mapper is null)
                return new T[0];

            #endregion

            var _XElement  = ParentXElement.Element(XWrapper);
            if (_XElement is null)
                return new T[0];

            var _XElements = _XElement.Elements(XName);
            if (_XElements is null || !_XElements.Any())
                return new T[0];

            return _XElements.Select(XML   => Mapper(XML, OnException)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default(T)));

        }

        #endregion

        #region MapElementsOrFail(ParentXElement, XName, Mapper, OnException = null, ExceptionMessage = null)

        public static IEnumerable<T> MapElementsOrFail<T>(this XElement                           ParentXElement,
                                                          XName                                   XName,
                                                          Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                          OnExceptionDelegate?                    OnException        = null,
                                                          String?                                 ExceptionMessage   = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                if (OnException is not null)
                    OnException(Timestamp.Now, ParentXElement, new Exception("The parent XML element must not be null!"));
                else
                    throw new Exception("The parent XML element must not be null!");

            if (Mapper is null)
                if (OnException is not null)
                    OnException(Timestamp.Now, ParentXElement, new Exception("Mapper delegate must not be null!"));
                else
                    throw new Exception("Mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
            {

                if (OnException is not null)
                {

                    OnException(Timestamp.Now,
                                ParentXElement,
                                new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                                  ? ExceptionMessage
                                                  : "The given XML element must not be null!"));

                    return [];

                }

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!");

            }

            return _XElements.Select(XML   => Mapper(XML, OnException)).
                              Where (value => !EqualityComparer<T>.Default.Equals(value, default));

        }


        public static IEnumerable<T> MapElementsOrFail2<T>(this XElement      ParentXElement,
                                                           XName              XName,
                                                           Func<XElement, T>  Mapper)
        {

            if (ParentXElement is null)
                throw new Exception("The parent XML element must not be null!");

            if (Mapper is null)
                throw new Exception("Mapper delegate must not be null!");

            var xElements = ParentXElement.Elements(XName);

            if (xElements is null || !xElements.Any())
                throw new Exception("The given XML elements must not be null!");

            return xElements.Select(XML   => Mapper(XML)).
                             Where (value => !EqualityComparer<T>.Default.Equals(value, default));

        }

        #endregion

        #region MapElementsOrFail(ParentXElement, XWrapper, XName, Mapper, OnException = null, ExceptionMessage = null)

        public static IEnumerable<T> MapElementsOrFail<T>(this XElement                           ParentXElement,
                                                          XName                                   XWrapper,
                                                          XName                                   XName,
                                                          Func<XElement, OnExceptionDelegate, T>  Mapper,
                                                          OnExceptionDelegate?                    OnException        = null,
                                                          String?                                 ExceptionMessage   = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                if (OnException is not null)
                    OnException(Timestamp.Now, ParentXElement, new Exception("The parent XML element must not be null!"));
                else
                    throw new Exception("The parent XML element must not be null!");

            if (Mapper is null)
                if (OnException is not null)
                    OnException(Timestamp.Now, ParentXElement, new Exception("Mapper delegate must not be null!"));
                else
                    throw new Exception("Mapper delegate must not be null!");

            #endregion

            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement is null)
                if (OnException is not null)
                    OnException(Timestamp.Now, ParentXElement, new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!"));
                else
                    throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!");

            var _XElements = _XElement.Elements(XName);

            if (_XElements is null)
                //if (OnException is not null)
                //    OnException(Timestamp.Now, ParentXElement, new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!"));
                //else
                //    throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!");
                return new T[0];

            var __XElements = _XElements.ToArray();

            if (__XElements.Length == 0)
                //if (OnException is not null)
                //    OnException(Timestamp.Now, ParentXElement, new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given XML element must not be null!"));
                //else
                //    throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given array of XML elements must not be empty!");
                return new T[0];


            return _XElements.Select(XML => Mapper(XML, OnException)).Where(v => !EqualityComparer<T>.Default.Equals(v, default(T)));

        }

        #endregion



        // Map XML Value

        #region MapValueOrNull(ParentXElement, XName)

        public static String MapValueOrNull(this XElement  ParentXElement,
                                            XName          XName)
        {

            if (ParentXElement is null || XName is null)
                return null;

            return ParentXElement?.Element(XName)?.Value;

        }

        #endregion

        #region MapValueOrNull(ParentXElement, XName, ValueMapper)

        public static T MapValueOrNull<T>(this XElement    ParentXElement,
                                          XName            XName,
                                          Func<String, T>  ValueMapper)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper    is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                return default(T);

            if (_XElement.Value.IsNullOrEmpty())
                return default(T);

            return ValueMapper(_XElement.Value);

        }

        #endregion

        #region MapValueOrNullable(ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        public static T? MapValueOrNullable<T>(this XElement    ParentXElement,
                                               XName            XName,
                                               Func<String, T>  ValueMapper,
                                               String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper    is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null || _XElement.Value.IsNullOrEmpty())
                return new T?();


            try
            {

                return ValueMapper(_XElement.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region MapValueOrNullable(ParentXElement, XWrapper, XName, ValueMapper)

        public static T? MapValueOrNullable<T>(this XElement    ParentXElement,
                                               XName            XWrapper,
                                               XName            XName,
                                               Func<String, T>  ValueMapper)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper    is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XWrapper = ParentXElement.Element(XWrapper);

            if (_XWrapper is null)
                return new T?();


            var _XElement = _XWrapper.Element(XName);

            if (_XElement is null || _XElement.Value.IsNullOrEmpty())
                return new T?();

            return ValueMapper(_XElement.Value);

        }

        #endregion

        #region MapValueOrFail(ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        public static T MapValueOrFail<T>(this XElement    ParentXElement,
                                          XName            XName,
                                          Func<String, T>  ValueMapper,
                                          String?          ExceptionMessage   = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper    is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var xElement = ParentXElement.Element(XName);

            if (xElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (xElement.Value.IsNullOrEmpty())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");


            try
            {

                return ValueMapper(xElement.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }


        public static T MapValueOrFail<T>(this XElement                          ParentXElement,
                                          XName                                  XName,
                                          Func<String, OnExceptionDelegate?, T>  ValueMapper,
                                          OnExceptionDelegate?                   OnException        = null,
                                          String?                                ExceptionMessage   = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper    is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var xElement = ParentXElement.Element(XName);

            if (xElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (xElement.Value is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");


            try
            {

                return ValueMapper(xElement.Value, OnException);

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now,
                                    xElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                    ? ExceptionMessage
                                    : "The XML element '" + XName.LocalName + "' is invalid!",
                                e);

            }

        }

        public static T MapValueOrFail<T>(this XElement                           ParentXElement,
                                          XName                                   XName,
                                          Func<String, OnExceptionDelegate?, T?>  ValueMapper,
                                          OnExceptionDelegate?                    OnException        = null,
                                          String?                                 ExceptionMessage   = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper    is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var xElement = ParentXElement.Element(XName);

            if (xElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (xElement.Value is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");

            T? Value;

            try
            {

                Value = ValueMapper(xElement.Value, OnException);

                if (!Value.HasValue)
                    throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "The XML element '" + XName.LocalName + "' is invalid!");

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now,
                                    xElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                    ? ExceptionMessage
                                    : "The XML element '" + XName.LocalName + "' is invalid!",
                                e);

            }

            return Value.Value;

        }

        #endregion

        #region MapValueOrFail(ParentXElement, XWrapper, XName, ValueMapper, ExceptionMessage = null)

        public static T MapValueOrFail<T>(this XElement                         ParentXElement,
                                          XName                                 XWrapper,
                                          XName                                 XName,
                                          Func<String, OnExceptionDelegate, T>  ValueMapper,
                                          OnExceptionDelegate                   OnException       = null,
                                          String                                ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XWrapper = ParentXElement.Element(XWrapper);

            if (_XWrapper is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML wrapper element '" + XWrapper.LocalName + "'!");


            var _XElement = _XWrapper.Element(XName);

            if (_XElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");

            if (_XElement.Value.IsNullOrEmpty())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "' must not be null!");


            try
            {

                return ValueMapper(_XElement.Value, OnException);

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now,
                                    _XElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        public static T MapValueOrFail<T>(this XElement    ParentXElement,
                                          XName            XWrapper,
                                          XName            XName,
                                          Func<String, T>  ValueMapper,
                                          String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XWrapper = ParentXElement.Element(XWrapper);

            if (_XWrapper is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML wrapper element '" + XWrapper.LocalName + "'!");


            var _XElement = _XWrapper.Element(XName);

            if (_XElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");

            if (_XElement.Value is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "' must not be null!");


            try
            {

                return ValueMapper(_XElement.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XWrapper.LocalName + "' > '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region MapValueOrDefault(ParentXElement, XName, ValueMapper, DefaultValue = default(T))

        public static T MapValueOrDefault<T>(this XElement    ParentXElement,
                                             XName            XName,
                                             Func<String, T>  ValueMapper,
                                             T                DefaultValue = default(T))
        {

            #region Initial checks

            if (ParentXElement is null)
                return DefaultValue;

            if (ValueMapper is null)
                return DefaultValue;

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                return DefaultValue;

            if (_XElement.Value.IsNullOrEmpty())
                return DefaultValue;

            return ValueMapper(_XElement.Value);

        }

        #endregion

        #region MapValueOrDefault(ParentXElement, XWrapper, XName, ValueMapper, DefaultValue = default(T))

        public static T MapValueOrDefault<T>(this XElement    ParentXElement,
                                             XName            XWrapper,
                                             XName            XName,
                                             Func<String, T>  ValueMapper,
                                             T                DefaultValue = default(T))
        {

            #region Initial checks

            if (ParentXElement is null)
                return DefaultValue;

            if (ValueMapper is null)
                return DefaultValue;

            #endregion


            var _XWrapper = ParentXElement.Element(XWrapper);

            if (_XWrapper is null)
                return DefaultValue;


            var _XElement = _XWrapper.Element(XName);

            if (_XElement is null || _XElement.Value.IsNullOrEmpty())
                return DefaultValue;


            return ValueMapper(_XElement.Value);

        }

        #endregion

        #region MapBooleanOrFail(ParentXElement, XName, ExceptionMessage = null)

        public static Boolean MapBooleanOrFail(this XElement  ParentXElement,
                                               XName          XName,
                                               String         ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement), "The given XML element must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (_XElement.Value is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");


            switch (_XElement.Value)
            {

                case "0":
                case "false":
                    return false;

                case "1":
                case "true":
                    return true;

                default:
                    throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "The value '" + _XElement.Value + "' of the given XML element '" + XName.LocalName + "' must not be null!");

            }

        }

        #endregion

        #region MapNullableBoolean(ParentXElement, XName)

        public static Boolean? MapNullableBoolean(this XElement  ParentXElement,
                                                  XName          XName)
        {

            #region Initial checks

            if (ParentXElement is null)
                return new Boolean?();

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                return new Boolean?();

            if (_XElement.Value.IsNullOrEmpty())
                return new Boolean?();

            switch (_XElement.Value)
            {

                case "0":
                case "false":
                    return new Boolean?(false);

                case "1":
                case "true":
                    return new Boolean?(true);

                default:
                    return new Boolean?();

            }

        }

        #endregion


        #region ParseTimestampOrFail(...)

        public static DateTime ParseTimestampOrFail(this XElement        ParentXElement,
                                                    XName                XName,
                                                    OnExceptionDelegate  OnException       = null,
                                                    String               ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (_XElement.Value is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");

            try
            {

                var Timestamp = DateTime.Parse(_XElement.Value);

                if (Timestamp.Kind != DateTimeKind.Utc)
                    Timestamp = Timestamp.ToUniversalTime();

                return Timestamp;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now,
                                    _XElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region ParseTimeSpanOrFail(...)

        public static TimeSpan ParseTimeSpanOrFail(this XElement        ParentXElement,
                                                   XName                XName,
                                                   OnExceptionDelegate  OnException       = null,
                                                   String               ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XName);

            if (_XElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing XML element '" + XName.LocalName + "'!");

            if (_XElement.Value is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The value of the given XML element '" + XName.LocalName + "' must not be null!");

            try
            {

                return TimeSpan.FromSeconds(UInt32.Parse(_XElement.Value));

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now,
                                    _XElement,
                                    e);

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML element '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion



        // Map XML Values

        #region MapValues(ParentXElement, XName, ValueMapper)

        public static IEnumerable<T> MapValues<T>(this XElement    ParentXElement,
                                                  XName            XName,
                                                  Func<String, T>  ValueMapper)
        {

            #region Initial checks

            if (ParentXElement is null)
                return new T[0];

            if (ValueMapper is null)
                return new T[0];

            #endregion

            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                return new T[0];

            return _XElements.SafeSelect(xelement => ValueMapper(xelement.Value));

        }

        #endregion

        #region MapValues(ParentXElement, XWrapperName, XElementsName, ValueMapper)

        public static IEnumerable<T> MapValues<T>(this XElement    ParentXElement,
                                                  XName            XWrapperName,
                                                  XName            XElementsName,
                                                  Func<String, T>  ValueMapper)
        {

            #region Initial checks

            if (ParentXElement is null)
                return new T[0];

            if (ValueMapper is null)
                return new T[0];

            #endregion

            var _XElement = ParentXElement.Element(XWrapperName);

            if (_XElement is null)
                return new T[0];

            var _XElements = _XElement.Elements(XElementsName);

            if (_XElements is null || !_XElements.Any())
                return new T[0];

            return _XElements.SafeSelect(xelement => ValueMapper(xelement.Value));

        }

        #endregion

        #region MapValuesOrDefault(ParentXElement, XWrapperName, XElementsName, ValueMapper, DefaultValue = default(T))

        public static IEnumerable<T> MapValuesOrDefault<T>(this XElement    ParentXElement,
                                                           XName            XWrapperName,
                                                           XName            XElementsName,
                                                           Func<String, T>  ValueMapper,
                                                           T                DefaultValue = default)
        {

            #region Initial checks

            if (ParentXElement is null)
                return new T[1] { DefaultValue };

            if (ValueMapper is null)
                return new T[1] { DefaultValue };

            #endregion

            var _XElement = ParentXElement.Element(XWrapperName);

            if (_XElement is null)
                return new T[1] { DefaultValue };

            var _XElements = _XElement.Elements(XElementsName);

            if (_XElements is null)
                return new T[1] { DefaultValue };

            var __XElements = _XElements.ToArray();

            if (__XElements.Length == 0)
                return new T[1] { DefaultValue };

            return _XElements.SafeSelect(__XElement => ValueMapper(__XElement.Value));

        }

        #endregion

        #region MapValuesOrFail(ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        public static IEnumerable<T> MapValuesOrFail<T>(this XElement    ParentXElement,
                                                        XName            XName,
                                                        Func<String, T>  ValueMapper,
                                                        String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XName.LocalName + "'!");


            return _XElements.SafeSelect(__XElement => ValueMapper(__XElement.Value));

        }

        public static IEnumerable<T> MapValuesOrFail<T>(this XElement    ParentXElement,
                                                        XName            XWrapper,
                                                        XName            XName,
                                                        Func<String, T>  ValueMapper,
                                                        String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "Missing XML element '" + XWrapper.LocalName + "'!");


            var _XElements = _XElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");


            return _XElements.SafeSelect(__XElement => ValueMapper(__XElement.Value));

        }

        #endregion

        #region MapEnumValues(ParentXElement, XWrapper, XName, ValueMapper, ExceptionMessage = null)

        public static T MapEnumValues<T>(this XElement    ParentXElement,
                                         XName            XName,
                                         Func<String, T>  ValueMapper,
                                         String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                return default(T);


            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return (T) (Object) Value;

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }


        public static T MapEnumValues<T>(this XElement    ParentXElement,
                                         XName            XWrapper,
                                         XName            XName,
                                         Func<String, T>  ValueMapper,
                                         String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement is null)
                return default(T);


            var _XElements = _XElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                return default(T);


            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return (T) (Object) Value;

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }

        #endregion

        #region MapEnumValuesOrNull(ParentXElement, XWrapper, XName, ValueMapper, ExceptionMessage = null)

        public static T? MapEnumValuesOrNull<T>(this XElement    ParentXElement,
                                                XName            XName,
                                                Func<String, T>  ValueMapper,
                                                String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                return new T?();


            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return new T?((T) (Object) Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }


        public static T? MapEnumValuesOrNull<T>(this XElement    ParentXElement,
                                                XName            XWrapper,
                                                XName            XName,
                                                Func<String, T>  ValueMapper,
                                                String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement is null)
                return default(T);


            var _XElements = _XElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                return new T?();


            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return new T?((T) (Object) Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }

        #endregion

        #region MapEnumValuesOrFail(ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        public static T MapEnumValuesOrFail<T>(this XElement    ParentXElement,
                                               XName            XName,
                                               Func<String, T>  ValueMapper,
                                               String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElements = ParentXElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XName.LocalName + "'!");

            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return (T) (Object) Value;

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }

        public static T MapEnumValuesOrFail<T>(this XElement    ParentXElement,
                                               XName            XWrapper,
                                               XName            XName,
                                               Func<String, T>  ValueMapper,
                                               String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _XElement = ParentXElement.Element(XWrapper);

            if (_XElement is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "Missing XML element '" + XWrapper.LocalName + "'!");


            var _XElements = _XElement.Elements(XName);

            if (_XElements is null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");

            try
            {

                Int64 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int64) (Object) _T;

                return (T) (Object) Value;

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }

        public static T MapEnumValuesOrFail2<T>(this XElement    ParentXElement,
                                                XName            XWrapper,
                                                XName            XName,
                                                Func<String, T>  ValueMapper,
                                                String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML element mapper delegate must not be null!");

            #endregion


            var _WrapperXElements = ParentXElement.Elements(XWrapper);

            if (_WrapperXElements is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                            ? ExceptionMessage
                                            : "Missing XML element(s) '" + XWrapper.LocalName + "'!");


            var _XElements = _WrapperXElements.Select(xml => xml.Element(XName));

            if (_XElements is null || !_XElements.Any())
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "Missing or empty XML elements '" + XWrapper.LocalName + "' > '" + XName.LocalName + "'!");

            try
            {

                Int32 Value = 0;

                foreach (var _T in _XElements.Select(__XElement => ValueMapper(__XElement.Value)))
                    Value |= (Int32) (Object) _T;

                return (T) Enum.ToObject(typeof(T), Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "A value of the XML element '" + XName.LocalName + "' could not be parsed as type '" + typeof(T) + "'!",
                                    e);

            }

        }


        #endregion


        // Attributes

        #region AttributeValueOrDefault(this ParentXElement, XName, DefaultValue)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static String AttributeValueOrDefault(this XElement  ParentXElement,
                                                     XName          XName,
                                                     String         DefaultValue)
        {

            #region Initial checks

            if (ParentXElement is null)
                return DefaultValue;

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute is null)
                return DefaultValue;

            return _XAttribute.Value;

        }

        #endregion

        #region AttributeValueOrFail(this ParentXElement, XName, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static String AttributeValueOrFail(this XElement  ParentXElement,
                                                  XName          XName,
                                                  String         ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            #endregion


            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The given XML attribute '" + XName.LocalName + "' was not found!");

            return _XAttribute.Value;

        }

        #endregion


        #region MapAttributeValueOrFail(this ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static T MapAttributeValueOrFail<T>(this XElement    ParentXElement,
                                                   XName            XName,
                                                   Func<String, T>  ValueMapper,
                                                   String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML attribute mapper delegate must not be null!");

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute is null)
                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The given XML attribute '" + XName.LocalName + "' was not found!");


            try
            {

                return ValueMapper(_XAttribute.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML attribute '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region MapAttributeValueOrDefault(this ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static T MapAttributeValueOrDefault<T>(this XElement    ParentXElement,
                                                      XName            XName,
                                                      Func<String, T>  ValueMapper,
                                                      String           ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML attribute mapper delegate must not be null!");

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute is null)
                return default(T);


            try
            {

                return ValueMapper(_XAttribute.Value);

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML attribute '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion

        #region MapAttributeValueOrDefault(this ParentXElement, XName, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static String MapAttributeValueOrDefault(this XElement  ParentXElement,
                                                        XName          XName,
                                                        String         ExceptionMessage  = null)
        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute is null)
                return null;

            return _XAttribute.Value;

        }

        #endregion

        #region MapAttributeValueOrNullable(this ParentXElement, XName, ValueMapper, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the first (in document order) attribute with the
        /// specified System.Xml.Linq.XName or the given default value.
        /// </summary>
        /// <param name="ParentXElement">The XML parent element.</param>
        /// <param name="XName">The System.Xml.Linq.XName to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static T? MapAttributeValueOrNullable<T>(this XElement    ParentXElement,
                                                        XName            XName,
                                                        Func<String, T>  ValueMapper,
                                                        String           ExceptionMessage = null)

            where T : struct

        {

            #region Initial checks

            if (ParentXElement is null)
                throw new ArgumentNullException(nameof(ParentXElement),  "The given XML element must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),     "The given XML attribute mapper delegate must not be null!");

            #endregion

            var _XAttribute = ParentXElement.Attribute(XName);

            if (_XAttribute is null)
                return new T?();


            try
            {

                return new T?(ValueMapper(_XAttribute.Value));

            }
            catch (Exception e)
            {

                throw new Exception(ExceptionMessage.IsNotNullOrEmpty()
                                        ? ExceptionMessage
                                        : "The XML attribute '" + XName.LocalName + "' is invalid!",
                                    e);

            }

        }

        #endregion



        // Stuff...

        #region ToUTF8Bytes(this XML)

        public static Byte[] ToUTF8Bytes(this XElement XML)
        {

            if (XML is null)
                return new Byte[0];

            return Encoding.UTF8.GetBytes(XML.ToString());

        }

        #endregion


    }

}
