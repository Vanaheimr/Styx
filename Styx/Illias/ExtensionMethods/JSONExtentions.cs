/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Hermod <https://www.github.com/Vanaheimr/Hermod>
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

using System.Globalization;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public delegate T        CustomTextParserDelegate<T>            (String  Text,       T       DataObject);

    public delegate T        CustomJObjectParserDelegate<T>         (JObject JSON,       T       DataObject);
    public delegate T?       CustomJObjectParserDelegateNullable<T> (JObject JSON,       T?      DataObject);

    public delegate T        CustomJArrayParserDelegate<T>          (JArray  JSON,       T       DataObject);

    public delegate String   CustomTextSerializerDelegate<T>        (T       DataObject, String  Text);

    public delegate JObject  CustomJObjectSerializerDelegate<T>     (T       DataObject, JObject JSON);

    public delegate JArray   CustomJArraySerializerDelegate<T>      (T       DataObject, JArray  JSON);


    public delegate TResult  Parser            <TResult>(String  Input);

    public delegate Boolean  TryParser         <TResult>(String  Input, out TResult?  arg);
    public delegate Boolean  TryParser2        <TResult>(String  Input, out TResult?  arg, out String? ErrorResponse);
    public delegate Boolean  TryParser3        <TResult>(String  Input, out TResult?  arg, out String? ErrorResponse, CustomJObjectParserDelegate<TResult>? CustomParser = null);
    public delegate Boolean  TryParser4        <TResult>(String  Input, out TResult?  arg, OnExceptionDelegate OnException);


    public delegate Boolean  TryNumericParser  <TResult>(UInt64  Input, out TResult?  arg);

    //public delegate Boolean  TryJObjectParser  <TResult>     (JObject Input, out TResult? arg);
    public delegate Boolean  TryJObjectParser1 <TResult>     (JObject Input, out TResult  arg, out String? ErrorResponse);
    public delegate Boolean  TryJObjectParser2a<TResult>     (JObject Input, [NotNullWhen(true)]  out TResult? arg, [NotNullWhen(false)] out String? ErrorResponse);
    public delegate Boolean  TryJObjectParser2b<TResult>     (JObject Input,                      out TResult? arg, [NotNullWhen(false)] out String? ErrorResponse); // TResult's properties are all optional and thus TResult can be null!
    public delegate Boolean  TryJObjectParser3a<TResult>     (JObject Input, out TResult? arg, out String? ErrorResponse,                 CustomJObjectParserDelegate<TResult>? CustomParser = null);
    public delegate Boolean  TryJObjectParser3b<TResult, TId>(JObject Input, out TResult? arg, out String? ErrorResponse, TId? Id = null, CustomJObjectParserDelegate<TResult>? CustomParser = null) where TId: struct;

    public delegate Boolean  TryJArrayParser   <TResult>     (JArray  Input, out TResult? arg);
    public delegate Boolean  TryJArrayParser2  <TResult>     (JArray  Input, out TResult? arg, out String? ErrorResponse);
    public delegate Boolean  TryJArrayParser3  <T>           (JArray  Input, out StdDev<T>? arg, out String? ErrorResponse) where T: struct;

    public delegate TResult?    ParserNullable <TResult>(String  Input)                                                    where TResult: struct;
    public delegate Boolean  TryParserNullable1<TResult>(String  Input, out TResult? arg)                                  where TResult: struct;
    public delegate Boolean  TryParserNullable2<TResult>(String  Input, out TResult? arg, out String ErrorResponse)        where TResult: struct;
    public delegate Boolean  TryParserNullable3<TResult>(String  Input, out TResult? arg, OnExceptionDelegate OnException) where TResult: struct;


    /// <summary>
    /// Extension methods to parse JSON.
    /// </summary>
    public static class JSONExtensions
    {

        #region Contains             (this JSON, PropertyName)

        public static Boolean Contains(this JObject  JSON,
                                       String        PropertyName)
        {

            if (JSON is null || PropertyName.IsNullOrEmpty())
                return false;

            return JSON[PropertyName] != null;

        }

        #endregion

        #region GetString            (this JSON, PropertyName, DefaultValue = default)

        public static String? GetString(this JObject  JSON,
                                        String        PropertyName,
                                        String?       DefaultValue = default)
        {

            PropertyName = PropertyName.Trim();

            if (JSON is null || PropertyName.IsNullOrEmpty())
                return DefaultValue;

            var value = JSON[PropertyName]?.Value<String>()?.Trim();

            return value.IsNotNullOrEmpty()
                       ? value
                       : DefaultValue;

        }

        #endregion


        #region ParseMandatoryText   (this JSON, PropertyName, PropertyDescription,                               out Text,                   out ErrorResponse)

        public static Boolean ParseMandatoryText(this JObject                      JSON,
                                                 String                            PropertyName,
                                                 String                            PropertyDescription,
                                                 [NotNullWhen(true)]  out String?  Text,
                                                 [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Text = null;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var jsonToken))
            {
                ErrorResponse = $"Missing property '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            if (jsonToken.Type != JTokenType.String)
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            try
            {
                Text = jsonToken?.Value<String>()?.Trim();
            }
            catch
            { }

            if (Text is null)
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region MapMandatory         (this JSON, PropertyName, PropertyDescription,                      Mapper,  out Value,                  out ErrorResponse)

        public static Boolean MapMandatory<T>(this JObject     JSON,
                                              String           PropertyName,
                                              String           PropertyDescription,
                                              Func<String, T>  Mapper,
                                              out T            Value,
                                              out String       ErrorResponse)
        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (Mapper is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                var JSONValue = JSONToken?.Value<String>()?.Trim();

                if (JSONValue.IsNeitherNullNorEmpty())
                {
                    Value          = Mapper(JSONValue);
                    ErrorResponse  = null;
                    return true;
                }

            }
#pragma warning disable RCS1075  // Avoid empty catch clause that catches System.Exception.
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning restore RCS1075  // Avoid empty catch clause that catches System.Exception.
            { }

            Value          = default;
            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
            return false;

        }

//        public static Boolean ParseMandatory<T>(this JObject     JSON,
//                                                String           PropertyName,
//                                                String           PropertyDescription,
//                                                Func<String, T>  Mapper,
//                                                out T?           Value,
//                                                out String       ErrorResponse)

//            where T : struct

//        {

//            Value = null;

//            if (JSON is null)
//            {
//                ErrorResponse = "Invalid JSON provided!";
//                return false;
//            }

//            if (PropertyName.IsNullOrEmpty())
//            {
//                ErrorResponse = "Invalid JSON property name provided!";
//                return false;
//            }

//            if (Mapper is null)
//            {
//                ErrorResponse = "Invalid mapper provided!";
//                return false;
//            }

//            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
//            {
//                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
//                return false;
//            }

//            try
//            {

//                var JSONValue = JSONToken?.Value<String>()?.Trim();

//                if (JSONValue.IsNeitherNullNorEmpty())
//                {
//                    Value          = Mapper(JSONValue);
//                    ErrorResponse  = null;
//                    return true;
//                }

//            }
//#pragma warning disable RCS1075  // Avoid empty catch clause that catches System.Exception.
//#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
//            catch
//#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
//#pragma warning restore RCS1075  // Avoid empty catch clause that catches System.Exception.
//            { }

//            Value          = null;
//            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
//            return false;

//        }

        #endregion

        #region ParseMandatory<T>    (this JSON, PropertyName, PropertyDescription,                   TryParser,  out Value,                  out ErrorResponse)

        public static Boolean ParseMandatory<T>(this JObject                       JSON,
                                                String                             PropertyName,
                                                String                             PropertyDescription,
                                                TryParser<T>                       TryParser,
                                                [NotNullWhen(true)]  out T         Value,
                                                [NotNullWhen(false)] out String?   ErrorResponse)
        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty() &&
                //!TryParser(JSONValue, out Value))
                TryParser(JSONValue, out Value))
            {
                ErrorResponse = null;
                return true;
            }

            Value          = default;
            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
            return false;

        }

        //public static Boolean ParseMandatoryS<T>(this JObject  JSON,
        //                                        String        PropertyName,
        //                                        String        PropertyDescription,
        //                                        TryParser<T>  TryParser,
        //                                        out T?        Value,
        //                                        out String?   ErrorResponse)

        //    where T : struct

        //{

        //    Value = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
        //        return false;
        //    }

        //    var JSONValue = JSONToken?.Value<String>()?.Trim();

        //    if (JSONValue.IsNeitherNullNorEmpty() &&
        //        //!TryParser(JSONValue, out T _Value))
        //        TryParser(JSONValue, out T _Value))
        //    {
        //        Value          = _Value;
        //        ErrorResponse  = null;
        //        return true;
        //    }

        //    Value          = null;
        //    ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //    return false;

        //}

        public static Boolean ParseMandatory<T>(this JObject                       JSON,
                                                String                             PropertyName,
                                                String                             PropertyDescription,
                                                TryParser2<T>                      TryParser,
                                                [NotNullWhen(true)]  out T         Value,
                                                [NotNullWhen(false)] out String?   ErrorResponse)
        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty() &&
                TryParser(JSONValue, out Value, out var errorResponse))
            {
                ErrorResponse = errorResponse;
                return true;
            }

            Value          = default;
            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
            return false;

        }

        //public static Boolean ParseMandatory<T>(this JObject  JSON,
        //                                        String        PropertyName,
        //                                        String        PropertyDescription,
        //                                        TryParser2<T> TryParser,
        //                                        out T?        Value,
        //                                        out String?   ErrorResponse)

        //    where T : struct

        //{

        //    Value = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
        //        return false;
        //    }

        //    var JSONValue = JSONToken?.Value<String>()?.Trim();

        //    if (JSONValue.IsNeitherNullNorEmpty() &&
        //        TryParser(JSONValue, out T value, out String errorResponse))
        //    {
        //        Value          = value;
        //        ErrorResponse  = null;
        //        return true;
        //    }

        //    Value          = null;
        //    ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //    return false;

        //}


        public static Boolean ParseMandatory<T>(this JObject                     JSON,
                                                String                           PropertyName,
                                                String                           PropertyDescription,
                                                TryParser3<T>                    TryParser,
                                                [NotNullWhen(true)]  out T       Value,
                                                [NotNullWhen(false)] out String  ErrorResponse,
                                                CustomJObjectParserDelegate<T>?  CustomParser = null)
        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty() &&
                TryParser(JSONValue, out Value, out var errorResponse, CustomParser))
            {
                ErrorResponse = errorResponse;
                return true;
            }

            Value          = default;
            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
            return false;

        }


        public static Boolean ParseMandatory3<T>(this JObject                     JSON,
                                                 String                           PropertyName,
                                                 String                           PropertyDescription,
                                                 TryParser4<T>                    TryParser,
                                                 [NotNullWhen(true)]  out T       Value,
                                                 [NotNullWhen(false)] out String  ErrorResponse,
                                                 OnExceptionDelegate              OnException)
        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty() &&
                TryParser(JSONValue, out Value, OnException))
            {
                ErrorResponse = null;
                return true;
            }

            Value          = default;
            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
            return false;

        }

        //public static Boolean ParseMandatory<T>(this JObject         JSON,
        //                                        String               PropertyName,
        //                                        String               PropertyDescription,
        //                                        TryParser3<T>        TryParser,
        //                                        out T?               Value,
        //                                        out String           ErrorResponse,
        //                                        OnExceptionDelegate  OnException)

        //    where T : struct

        //{

        //    Value = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
        //        return false;
        //    }

        //    var JSONValue = JSONToken?.Value<String>()?.Trim();

        //    if (JSONValue.IsNeitherNullNorEmpty() &&
        //        TryParser(JSONValue, out T value, OnException))
        //    {
        //        Value          = value;
        //        ErrorResponse  = null;
        //        return true;
        //    }

        //    Value          = null;
        //    ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //    return false;

        //}

        #endregion

        #region ParseMandatory_<T>   (this JSON, PropertyName, PropertyDescription,                   TryParser,  out Value,                  out ErrorResponse)

        public static Boolean ParseMandatory_<T>(this JObject                      JSON,
                                                 String                            PropertyName,
                                                 String                            PropertyDescription,
                                                 ParserNullable<T>                 TryParser,
                                                 [NotNullWhen(true)]  out T        Value,
                                                 [NotNullWhen(false)] out String?  ErrorResponse)

            where T : struct

        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty())
            {

                var ValueNullable = TryParser(JSONValue);

                if (ValueNullable.HasValue)
                {
                    Value          = ValueNullable.Value;
                    ErrorResponse  = null;
                    return true;
                }

            }

            Value          = default;
            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
            return false;

        }

        public static Boolean ParseMandatory_<T>(this JObject           JSON,
                                                 String                 PropertyName,
                                                 String                 PropertyDescription,
                                                 TryParserNullable1<T>  TryParser,
                                                 out T                  Value,
                                                 out String?            ErrorResponse)

            where T : struct

        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty() &&
                TryParser(JSONValue, out T? ValueNullable) &&
                ValueNullable.HasValue)
            {
                Value          = ValueNullable.Value;
                ErrorResponse  = null;
                return true;
            }

            Value          = default;
            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
            return false;

        }

        //public static Boolean ParseMandatoryS<T>(this JObject  JSON,
        //                                        String        PropertyName,
        //                                        String        PropertyDescription,
        //                                        TryParser<T>  TryParser,
        //                                        out T?        Value,
        //                                        out String?   ErrorResponse)

        //    where T : struct

        //{

        //    Value = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
        //        return false;
        //    }

        //    var JSONValue = JSONToken?.Value<String>()?.Trim();

        //    if (JSONValue.IsNeitherNullNorEmpty() &&
        //        //!TryParser(JSONValue, out T _Value))
        //        TryParser(JSONValue, out T _Value))
        //    {
        //        Value          = _Value;
        //        ErrorResponse  = null;
        //        return true;
        //    }

        //    Value          = null;
        //    ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //    return false;

        //}

        public static Boolean ParseMandatory_<T>(this JObject           JSON,
                                                 String                 PropertyName,
                                                 String                 PropertyDescription,
                                                 TryParserNullable2<T>  TryParser,
                                                 out T                  Value,
                                                 out String?            ErrorResponse)

            where T : struct

        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty() &&
                TryParser(JSONValue, out T? ValueNullable, out var errorResponse) &&
                ValueNullable.HasValue)
            {
                Value          = ValueNullable.Value;
                ErrorResponse  = errorResponse;
                return true;
            }

            Value          = default;
            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
            return false;

        }

        //public static Boolean ParseMandatory<T>(this JObject  JSON,
        //                                        String        PropertyName,
        //                                        String        PropertyDescription,
        //                                        TryParser2<T> TryParser,
        //                                        out T?        Value,
        //                                        out String?   ErrorResponse)

        //    where T : struct

        //{

        //    Value = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
        //        return false;
        //    }

        //    var JSONValue = JSONToken?.Value<String>()?.Trim();

        //    if (JSONValue.IsNeitherNullNorEmpty() &&
        //        TryParser(JSONValue, out T value, out String errorResponse))
        //    {
        //        Value          = value;
        //        ErrorResponse  = null;
        //        return true;
        //    }

        //    Value          = null;
        //    ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //    return false;

        //}


        public static Boolean ParseMandatory_<T>(this JObject           JSON,
                                                 String                 PropertyName,
                                                 String                 PropertyDescription,
                                                 TryParserNullable3<T>  TryParser,
                                                 out T                  Value,
                                                 out String?            ErrorResponse,
                                                 OnExceptionDelegate    OnException)

            where T : struct

        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            var JSONValue = JSONToken?.Value<String>()?.Trim();

            if (JSONValue.IsNeitherNullNorEmpty() &&
                TryParser(JSONValue, out T? ValueNullable, OnException) &&
                ValueNullable.HasValue)
            {
                Value          = ValueNullable.Value;
                ErrorResponse  = null;
                return true;
            }

            Value          = default;
            ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
            return false;

        }

        //public static Boolean ParseMandatory<T>(this JObject         JSON,
        //                                        String               PropertyName,
        //                                        String               PropertyDescription,
        //                                        TryParser3<T>        TryParser,
        //                                        out T?               Value,
        //                                        out String           ErrorResponse,
        //                                        OnExceptionDelegate  OnException)

        //    where T : struct

        //{

        //    Value = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
        //        return false;
        //    }

        //    var JSONValue = JSONToken?.Value<String>()?.Trim();

        //    if (JSONValue.IsNeitherNullNorEmpty() &&
        //        TryParser(JSONValue, out T value, OnException))
        //    {
        //        Value          = value;
        //        ErrorResponse  = null;
        //        return true;
        //    }

        //    Value          = null;
        //    ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //    return false;

        //}

        #endregion



        #region ParseMandatory...    (this JSON, PropertyName, PropertyDescription,                               out Numbers...,             out ErrorResponse)

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Boolean,                out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             out Boolean                       BooleanValue,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            BooleanValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null || JSONToken.Type == JTokenType.Null)
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            try
            {
                BooleanValue = JSONToken.Value<Boolean>();
            }
            catch (Exception e)
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Single/Double,          out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             out Single                        SingleValue,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            SingleValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Single.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out SingleValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             [NotNullWhen(true)]  out Double   DoubleValue,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            DoubleValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Double.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out DoubleValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Decimal,                out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             out Decimal                       DecimalValue,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            DecimalValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out DecimalValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out (S)Byte,                out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             out Byte                          ByteValue,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ByteValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Byte.TryParse(JSONToken.Value<String>(), out ByteValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out SByte     SByteValue,
                                             out String?   ErrorResponse)
        {

            SByteValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !SByte.TryParse(JSONToken.Value<String>(), out SByteValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out (U)Int16/32/64,         out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             out Int16                         Int16Value,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Int16Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Int16.TryParse(JSONToken.Value<String>(), out Int16Value))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             out UInt16                        UInt16Value,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            UInt16Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !UInt16.TryParse(JSONToken.Value<String>(), out UInt16Value))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             out Int32                         Int32Value,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Int32Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Int32.TryParse(JSONToken.Value<String>(), out Int32Value))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatory(this JObject                       JSON,
                                             String                             PropertyName,
                                             String                             PropertyDescription,
                                             [NotNullWhen(true)]  out UInt32    UInt32Value,
                                             [NotNullWhen(false)] out String?   ErrorResponse)
        {

            UInt32Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !UInt32.TryParse(JSONToken.Value<String>(), out UInt32Value))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Int64     Int64Value,
                                             out String?   ErrorResponse)
        {

            Int64Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Int64.TryParse(JSONToken.Value<String>(), out Int64Value))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             [NotNullWhen(true)]  out UInt64   UInt64Value,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            UInt64Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !UInt64.TryParse(JSONToken.Value<String>(), out UInt64Value))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion


        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Ampere,                 out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Ampere    AmpereValue,
                                             out String?   ErrorResponse)
        {

            AmpereValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            AmpereValue    = Ampere.ParseA(decimalValue);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Hertz,                  out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Hertz     HertzValue,
                                             out String?   ErrorResponse)
        {

            HertzValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            HertzValue     = Hertz.ParseHz(decimalValue);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Meter,                  out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Meter     MeterValue,
                                             out String?   ErrorResponse)
        {

            MeterValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            MeterValue     = Meter.ParseM(decimalValue);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Percentage,             out ErrorResponse)

        public static Boolean ParseMandatory(this JObject    JSON,
                                             String          PropertyName,
                                             String          PropertyDescription,
                                             out Percentage  PercentageValue,
                                             out String?     ErrorResponse)
        {

            PercentageValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            PercentageValue  = Percentage.Parse(decimalValue);
            ErrorResponse    = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out PercentageInt,          out ErrorResponse)

        public static Boolean ParseMandatory(this JObject        JSON,
                                             String              PropertyName,
                                             String              PropertyDescription,
                                             out PercentageByte  PercentageValue,
                                             out String?         ErrorResponse)
        {

            PercentageValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Byte.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var byteValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            PercentageValue  = PercentageByte.Parse(byteValue);
            ErrorResponse    = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Volt,                   out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Volt      VoltValue,
                                             out String?   ErrorResponse)
        {

            VoltValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            VoltValue      = Volt.ParseV(decimalValue);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Watt,                   out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out Watt      WattValue,
                                             out String?   ErrorResponse)
        {

            WattValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            WattValue      = Watt.ParseW(decimalValue);
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out WattHour,               out ErrorResponse)

        public static Boolean ParseMandatory(this JObject  JSON,
                                             String        PropertyName,
                                             String        PropertyDescription,
                                             out WattHour  WattHourValue,
                                             out String?   ErrorResponse)
        {

            WattHourValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            WattHourValue  = WattHour.ParseWh(decimalValue);
            ErrorResponse    = null;
            return true;

        }

        #endregion

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out Timestamp,              out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             out DateTime                      Timestamp,
                                             [NotNullWhen(false)] out String?  ErrorResponse)

        {

            Timestamp = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken? JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            DateTime timestamp;
            try
            {

                timestamp = JSONToken.Value<DateTime>();

                if (timestamp.Kind != DateTimeKind.Utc)
                    timestamp = timestamp.ToUniversalTime();

            }
            catch
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            Timestamp      = timestamp;
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out TimeSpan,               out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                       JSON,
                                             String                             PropertyName,
                                             String                             PropertyDescription,
                                             [NotNullWhen(true)]  out TimeSpan  TimeSpan,
                                             [NotNullWhen(false)] out String?   ErrorResponse)

        {

            TimeSpan = TimeSpan.MinValue;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                TimeSpan = TimeSpan.FromSeconds(JSONToken.Value<UInt32>());

            }
            catch
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out I18NText,               out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                         JSON,
                                             String                               PropertyName,
                                             String                               PropertyDescription,
                                             [NotNullWhen(true)]  out I18NString  I18NText,
                                             [NotNullWhen(false)] out String?     ErrorResponse)

        {

            I18NText       = I18NString.Empty;
            ErrorResponse  = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken? JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is not JObject i18NObject)
            {
                ErrorResponse = "Invalid i18n JSON string provided!";
                return false;
            }

            foreach (var i18nProperty in i18NObject)
            {
                if (i18nProperty.Key   is not null &&
                    i18nProperty.Value is not null &&
                    Enum.TryParse(i18nProperty.Key, true, out Languages language) &&
                    i18nProperty.Value.Value<String>() is String text)
                {
                    I18NText.Set(language, text);
                }
                else
                {
                    ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                    return false;
                }
            }

            return true;

        }

        #endregion

        #region ParseMandatoryEnum   (this JSON, PropertyName, PropertyDescription,                               out EnumValue,              out ErrorResponse)

        public static Boolean ParseMandatoryEnum<TEnum>(this JObject                      JSON,
                                                        String                            PropertyName,
                                                        String                            PropertyDescription,
                                                        [NotNullWhen(true)]  out TEnum    EnumValue,
                                                        [NotNullWhen(false)] out String?  ErrorResponse)

             where TEnum : struct

        {

            EnumValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing JSON property '{PropertyName}'!";
                return false;
            }

            if (JSONToken is null ||
                !Enum.TryParse(JSONToken.Value<String>(), true, out EnumValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion


        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out JObject,                out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             [NotNullWhen(true)]  out JObject  JObject,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            JObject = null;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                JObject = JSONToken as JObject;

            }
            catch
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out JArray,                 out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                      JSON,
                                             String                            PropertyName,
                                             String                            PropertyDescription,
                                             [NotNullWhen(true)]  out JArray   JArray,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            JArray = null;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                JArray = JSONToken as JArray;

            }
            catch
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatory       (this JSON, PropertyName, PropertyDescription,                               out EnumerationOfStrings,   out ErrorResponse)

        public static Boolean ParseMandatory(this JObject             JSON,
                                             String                   PropertyName,
                                             String                   PropertyDescription,
                                             out IEnumerable<String>  EnumerationOfStrings,
                                             out String               ErrorResponse)
        {

            EnumerationOfStrings = null;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            try
            {

                if (!(JSONToken is JArray JArray))
                {
                    ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                    return false;
                }

                EnumerationOfStrings = JArray.SafeSelect(item => item.Value<String>()).ToArray();

            }
            catch
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion


        #region ParseMandatoryJSON      (this JSON, PropertyName, PropertyDescription,                   TryJObjectParser,  out Value,           out ErrorResponse)

        //public static Boolean ParseMandatoryJSON<T>(this JObject         JSON,
        //                                            String               PropertyName,
        //                                            String               PropertyDescription,
        //                                            TryJObjectParser<T>  TryJObjectParser,
        //                                            out T?               Value,
        //                                            out String?          ErrorResponse)
        //{

        //    Value = default;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryJObjectParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out JToken? JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (JSONToken is not JObject JSONValue)
        //    {
        //        ErrorResponse = "Invalid JSON object '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!TryJObjectParser(JSONValue, out T? value))
        //    {
        //        Value         = default;
        //        ErrorResponse = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";
        //        return false;
        //    }

        //    Value         = value;
        //    ErrorResponse = null;
        //    return true;

        //}

        //public static Boolean ParseMandatoryJSON2<T>(this JObject         JSON,
        //                                            String                PropertyName,
        //                                            String                PropertyDescription,
        //                                            TryJObjectParser2<T>  TryJObjectParser,
        //                                            out T?                Value,
        //                                            out String?           ErrorResponse)
        //{

        //    Value = default;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryJObjectParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out JToken? JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (JSONToken is not JObject JSONValue)
        //    {
        //        ErrorResponse = "Invalid JSON object '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!TryJObjectParser(JSONValue, out T? value, out _))
        //    {
        //        Value         = default;
        //        ErrorResponse = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";
        //        return false;
        //    }

        //    Value         = value;
        //    ErrorResponse = null;
        //    return true;

        //}

        //public static Boolean ParseMandatoryJSON2<T>(this JObject         JSON,
        //                                            String                PropertyName,
        //                                            String                PropertyDescription,
        //                                            TryJObjectParser4<T>  TryJObjectParser,
        //                                            out T?                Value,
        //                                            out String?           ErrorResponse,
        //                                            OnExceptionDelegate?  OnException  = null)
        //{

        //    Value = default;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryJObjectParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out JToken? JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (JSONToken is not JObject JSONValue)
        //    {
        //        ErrorResponse = "Invalid JSON object '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!TryJObjectParser(JSONValue, out T? value, OnException))
        //    {
        //        Value         = default;
        //        ErrorResponse = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";
        //        return false;
        //    }

        //    Value         = value;
        //    ErrorResponse = null;
        //    return true;

        //}


        public static Boolean ParseMandatoryJSON<T>(this JObject                      JSON,
                                                    String                            PropertyName,
                                                    String                            PropertyDescription,
                                                    TryJObjectParser2a<T>              TryJObjectParser,
                                                    [NotNullWhen(true)]  out T?       Value,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryJObjectParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing JSON property '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (JSONToken is not JObject JSONValue)
            {
                ErrorResponse = $"Invalid JSON object '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (!TryJObjectParser(JSONValue,
                                  out var value,
                                  out var errorResponse))
            {
                Value          = default;
                ErrorResponse  = $"JSON property '{PropertyName}' ({PropertyDescription}) could not be parsed: {errorResponse}";
                return false;
            }

            Value         = value;
            ErrorResponse = null;
            return true;
 
        }

        public static Boolean ParseMandatoryJSONS<T>(this JObject                      JSON,
                                                     String                            PropertyName,
                                                     String                            PropertyDescription,
                                                     TryJObjectParser2a<T>              TryJObjectParser,
                                                     [NotNullWhen(true)]  out T?       Value,
                                                     [NotNullWhen(false)] out String?  ErrorResponse)

            where T : struct

        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryJObjectParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing JSON property '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (JSONToken is not JObject JSONValue)
            {
                ErrorResponse = $"Invalid JSON object '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (!TryJObjectParser(JSONValue, out T value, out var errorResponse))
            {
                Value         = default;
                ErrorResponse = $"JSON property '{PropertyName}' ({PropertyDescription}) could not be parsed: {errorResponse}";
                return false;
            }

            Value         = value;
            ErrorResponse = null;
            return true;
 
        }

        public static Boolean ParseMandatoryJSONStruct<T>(this JObject                      JSON,
                                                          String                            PropertyName,
                                                          String                            PropertyDescription,
                                                          TryJObjectParser2a<T>              TryJObjectParser,
                                                          [NotNullWhen(true)]  out T        Value,
                                                          [NotNullWhen(false)] out String?  ErrorResponse)

            where T : struct

        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (TryJObjectParser is null)
            {
                ErrorResponse = "Invalid mapper provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing JSON property '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (!(JSONToken is JObject JSONValue))
            {
                ErrorResponse = $"Invalid JSON object '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (!TryJObjectParser(JSONValue, out T value, out var errorResponse))
            {
                Value         = default;
                ErrorResponse = $"JSON property '{PropertyName}' ({PropertyDescription}) could not be parsed: {errorResponse}";
                return false;
            }

            Value         = value;
            ErrorResponse = null;
            return true;
 
        }

        //public static Boolean ParseMandatoryJSON<T>(this JObject          JSON,
        //                                            String                PropertyName,
        //                                            String                PropertyDescription,
        //                                            TryJObjectParser4<T>  TryJObjectParser,
        //                                            out T?                Value,
        //                                            out String?           ErrorResponse)

        ////    where T : struct

        //{

        //    Value = default;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryJObjectParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!(JSONToken is JObject JSONValue))
        //    {
        //        ErrorResponse = "Invalid JSON object '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!TryJObjectParser(JSONValue, out T value, OnException))
        //    {
        //        Value         = default;
        //        ErrorResponse = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";
        //        return false;
        //    }

        //    Value         = value;
        //    ErrorResponse = null;
        //    return true;
 
        //}

        //public static Boolean ParseMandatoryJSON<T>(this JObject          JSON,
        //                                            String                PropertyName,
        //                                            String                PropertyDescription,
        //                                            TryJObjectParser<T>  TryJObjectParser,
        //                                            out T?                Value,
        //                                            out String            ErrorResponse)

        //    where T : struct

        //{

        //    Value = default;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryJObjectParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!(JSONToken is JObject JSONValue))
        //    {
        //        ErrorResponse = "Invalid JSON object '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!TryJObjectParser(JSONValue, out T value))
        //    {
        //        Value         = default;
        //        ErrorResponse = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";
        //        return false;
        //    }

        //    Value         = value;
        //    ErrorResponse = null;
        //    return true;
 
        //}

        //public static Boolean ParseMandatoryJSON<T>(this JObject          JSON,
        //                                            String                PropertyName,
        //                                            String                PropertyDescription,
        //                                            TryJObjectParser2<T>  TryJObjectParser,
        //                                            out T?                Value,
        //                                            out String            ErrorResponse)

        //    where T : struct

        //{

        //    Value = default;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryJObjectParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!(JSONToken is JObject JSONValue))
        //    {
        //        ErrorResponse = "Invalid JSON object '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!TryJObjectParser(JSONValue, out T value, out String errorResponse))
        //    {
        //        Value         = default;
        //        ErrorResponse = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed: " + errorResponse;
        //        return false;
        //    }

        //    Value         = value;
        //    ErrorResponse = null;
        //    return true;
 
        //}

        //public static Boolean ParseMandatoryJSON<T>(this JObject          JSON,
        //                                            String                PropertyName,
        //                                            String                PropertyDescription,
        //                                            TryJObjectParser4<T>  TryJObjectParser,
        //                                            out T?                Value,
        //                                            out String            ErrorResponse,
        //                                            OnExceptionDelegate   OnException  = null)

        //    where T : struct

        //{

        //    Value = default;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (TryJObjectParser is null)
        //    {
        //        ErrorResponse = "Invalid mapper provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing JSON property '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!(JSONToken is JObject JSONValue))
        //    {
        //        ErrorResponse = "Invalid JSON object '" + PropertyName + "' (" + PropertyDescription + ")!";
        //        return false;
        //    }

        //    if (!TryJObjectParser(JSONValue, out T value, OnException))
        //    {
        //        Value         = default;
        //        ErrorResponse = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";
        //        return false;
        //    }

        //    Value         = value;
        //    ErrorResponse = null;
        //    return true;
 
        //}

        #endregion

        #region ParseMandatory          (this JSON, PropertyName, PropertyDescription,                               out EnumerationOfT,         out ErrorResponse)

        public static Boolean ParseMandatory<T>(this JObject                             JSON,
                                                String                                   PropertyName,
                                                String                                   PropertyDescription,
                                                Parser<T>                                Parser,
                                                [NotNullWhen(true)]  out IEnumerable<T>  EnumerationOfT,
                                                [NotNullWhen(false)] out String?         ErrorResponse)
        {

            EnumerationOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                var ListOfT = new List<T>();

                foreach (var item in JArray)
                {
                    if (item.Type == JTokenType.String)
                        ListOfT.Add(Parser(item?.Value<String>() ?? ""));
                }

                EnumerationOfT = ListOfT;

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatory<T>(this JObject                             JSON,
                                                String                                   PropertyName,
                                                String                                   PropertyDescription,
                                                TryParser<T>                             TryParser,
                                                [NotNullWhen(true)]  out IEnumerable<T>  EnumerationOfT,
                                                [NotNullWhen(false)] out String?         ErrorResponse)
        {

            EnumerationOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                var ListOfT = new List<T>();

                foreach (var item in JArray)
                {
                    if (item is not null &&
                        item.Type == JTokenType.String)
                    {

                        if (TryParser(item.Value<String>() ?? "", out var ItemT) && ItemT is not null)
                            ListOfT.Add(ItemT);

                        else
                        {
                            ErrorResponse = $"Invalid value '{item.Value<String>() ?? ""}' for '{PropertyDescription ?? PropertyName}'!";
                            return false;
                        }

                    }
                }

                EnumerationOfT = ListOfT;

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatoryList      (this JSON, PropertyName, PropertyDescription,                               out ListOfT,                out ErrorResponse)

        public static Boolean ParseMandatoryList<T>(this JObject                      JSON,
                                                    String                            PropertyName,
                                                    String                            PropertyDescription,
                                                    Parser<T>                         Parser,
                                                    [NotNullWhen(true)]  out List<T>  ListOfT,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ListOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item.Type == JTokenType.String)
                        ListOfT.Add(Parser(item?.Value<String>() ?? ""));
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatoryNumericList<T>(this JObject                      JSON,
                                                           String                            PropertyName,
                                                           String                            PropertyDescription,
                                                           TryNumericParser<T>               TryParser,
                                                           [NotNullWhen(true)]  out List<T>  ListOfT,
                                                           [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ListOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item is not null &&
                        item.Type == JTokenType.Integer)
                    {

                        if (TryParser(item.Value<UInt64>(), out var ItemT) && ItemT is not null)
                            ListOfT.Add(ItemT);

                        else
                        {
                            ErrorResponse = $"Invalid value '{item?.Value<String>() ?? ""}' for '{PropertyDescription ?? PropertyName}'!";
                            return false;
                        }

                    }
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatoryList<T>(this JObject                      JSON,
                                                    String                            PropertyName,
                                                    String                            PropertyDescription,
                                                    TryParser<T>                      TryParser,
                                                    [NotNullWhen(true)]  out List<T>  ListOfT,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ListOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item is not null &&
                        item.Type == JTokenType.String)
                    {

                        if (TryParser(item.Value<String>() ?? "", out var ItemT) && ItemT is not null)
                            ListOfT.Add(ItemT);

                        else
                        {
                            ErrorResponse = $"Invalid value '{item?.Value<String>() ?? ""}' for '{PropertyDescription ?? PropertyName}'!";
                            return false;
                        }

                    }
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatoryList<T>(this JObject                      JSON,
                                                    String                            PropertyName,
                                                    String                            PropertyDescription,
                                                    TryParser2<T>                     TryParser,
                                                    [NotNullWhen(true)]  out List<T>  ListOfT,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ListOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item is not null &&
                        item.Type == JTokenType.String)
                    {

                        if (TryParser(item.Value<String>() ?? "",
                                      out var ItemT,
                                      out var errorResponse) && ItemT is not null)
                        {
                            ListOfT.Add(ItemT);
                        }

                        else
                        {
                            ErrorResponse = $"Invalid value '{item?.Value<String>() ?? ""}' for '{PropertyDescription ?? PropertyName}': {errorResponse}";
                            return false;
                        }

                    }
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatoryList<T>(this JObject                      JSON,
                                                    String                            PropertyName,
                                                    String                            PropertyDescription,
                                                    TryJObjectParser2a<T>              TryParser,
                                                    [NotNullWhen(true)]  out List<T>  ListOfT,
                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ListOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item.Type == JTokenType.Object &&
                        item is JObject JSONObject)
                    {

                        if (TryParser(JSONObject,
                                      out var ItemT,
                                      out var errorResponse) && ItemT is not null)
                        {
                            ListOfT.Add(ItemT);
                        }

                        else
                        {
                            ErrorResponse = $"Invalid value '{item.Value<String>() ?? ""}' for '{PropertyDescription ?? PropertyName}': {errorResponse}";
                            return false;
                        }

                    }
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatoryHashSet   (this JSON, PropertyName, PropertyDescription,                               out HashSetOfT,             out ErrorResponse)

        public static Boolean ParseMandatoryHashSet<T>(this JObject                         JSON,
                                                       String                               PropertyName,
                                                       String                               PropertyDescription,
                                                       Parser<T>                            Parser,
                                                       [NotNullWhen(true)]  out HashSet<T>  HashSetOfT,
                                                       [NotNullWhen(false)] out String?     ErrorResponse)
        {

            HashSetOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item.Type == JTokenType.String)
                        HashSetOfT.Add(Parser(item?.Value<String>() ?? ""));
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatoryNumericHashSet<T>(this JObject                         JSON,
                                                              String                               PropertyName,
                                                              String                               PropertyDescription,
                                                              TryNumericParser<T>                  TryParser,
                                                              [NotNullWhen(true)]  out HashSet<T>  HashSetOfT,
                                                              [NotNullWhen(false)] out String?     ErrorResponse)
        {

            HashSetOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item is not null &&
                        item.Type == JTokenType.Integer)
                    {

                        if (TryParser(item.Value<UInt64>(), out var ItemT) && ItemT is not null)
                            HashSetOfT.Add(ItemT);

                        else
                        {
                            ErrorResponse = $"Invalid value '{item?.Value<String>() ?? ""}' for '{PropertyDescription ?? PropertyName}'!";
                            return false;
                        }

                    }
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatoryHashSet<T>(this JObject                         JSON,
                                                       String                               PropertyName,
                                                       String                               PropertyDescription,
                                                       TryParser<T>                         TryParser,
                                                       [NotNullWhen(true)]  out HashSet<T>  HashSetOfT,
                                                       [NotNullWhen(false)] out String?     ErrorResponse)
        {

            HashSetOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item is not null &&
                        item.Type == JTokenType.String)
                    {

                        if (TryParser(item.Value<String>() ?? "", out var ItemT) && ItemT is not null)
                            HashSetOfT.Add(ItemT);

                        else
                        {
                            ErrorResponse = $"Invalid value '{item?.Value<String>() ?? ""}' for '{PropertyDescription ?? PropertyName}'!";
                            return false;
                        }

                    }
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatoryHashSet<T>(this JObject                         JSON,
                                                       String                               PropertyName,
                                                       String                               PropertyDescription,
                                                       TryParser2<T>                        TryParser,
                                                       [NotNullWhen(true)]  out HashSet<T>  HashSetOfT,
                                                       [NotNullWhen(false)] out String?     ErrorResponse)
        {

            HashSetOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item is not null &&
                        item.Type == JTokenType.String)
                    {

                        if (TryParser(item.Value<String>() ?? "",
                                      out var ItemT,
                                      out var errorResponse) && ItemT is not null)
                        {
                            HashSetOfT.Add(ItemT);
                        }

                        else
                        {
                            ErrorResponse = $"Invalid value '{item?.Value<String>() ?? ""}' for '{PropertyDescription ?? PropertyName}': {errorResponse}";
                            return false;
                        }

                    }
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }


        public static Boolean ParseMandatoryHashSet<T>(this JObject                         JSON,
                                                       String                               PropertyName,
                                                       String                               PropertyDescription,
                                                       TryJObjectParser2a<T>                 TryParser,
                                                       [NotNullWhen(true)]  out HashSet<T>  HashSetOfT,
                                                       [NotNullWhen(false)] out String?     ErrorResponse)
        {

            HashSetOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                foreach (var item in JArray)
                {
                    if (item.Type == JTokenType.Object &&
                        item is JObject JSONObject)
                    {

                        if (TryParser(JSONObject,
                                      out var ItemT,
                                      out var errorResponse) && ItemT is not null)
                        {
                            HashSetOfT.Add(ItemT);
                        }

                        else
                        {
                            ErrorResponse = $"Invalid value '{item.Value<String>() ?? ""}' for '{PropertyDescription ?? PropertyName}': {errorResponse}";
                            return false;
                        }

                    }
                }

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ParseMandatoryJSON      (this JSON, PropertyName, PropertyDescription,                               out EnumerationOfT,         out ErrorResponse)

        //public static Boolean ParseMandatoryJSON<T>(this JObject         JSON,
        //                                            String               PropertyName,
        //                                            String               PropertyDescription,
        //                                            TryJObjectParser<T>  TryJObjectParser,
        //                                            out IEnumerable<T>   EnumerationOfT,
        //                                            out String           ErrorResponse)
        //{

        //    EnumerationOfT = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing property '" + PropertyName + "'!";
        //        return false;
        //    }

        //    try
        //    {

        //        if (!(JSONToken is JArray JArray))
        //        {
        //            ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //            return false;
        //        }

        //        var ListOfT = new List<T>();

        //        foreach (var item in JArray)
        //        {
        //            if (item is JObject && TryJObjectParser(item as JObject, out T ItemT))
        //                ListOfT.Add(ItemT);
        //        }

        //        EnumerationOfT = ListOfT;

        //    }
        //    catch
        //    {
        //        ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //        return false;
        //    }

        //    ErrorResponse = null;
        //    return true;

        //}


        public static Boolean ParseMandatoryJSON<T>(this JObject                             JSON,
                                                    String                                   PropertyName,
                                                    String                                   PropertyDescription,
                                                    TryJObjectParser2a<T>                     TryJObjectParser,
                                                    [NotNullWhen(true)]  out IEnumerable<T>  EnumerationOfT,
                                                    [NotNullWhen(false)] out String?         ErrorResponse)
        {

            EnumerationOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                    return false;
                }

                var listOfT        = new List<T>();
                var errorResponse  = "";

                foreach (var item in JArray)
                {

                    if (item is JObject itemJObject &&
                        TryJObjectParser(itemJObject, out var itemT, out errorResponse) &&
                        itemT is not null)
                    {
                        listOfT.Add(itemT);
                    }

                    if (errorResponse is not null)
                    {
                        ErrorResponse = $"Invalid JSON property '{PropertyDescription ?? PropertyName}': {errorResponse}";
                        return false;
                    }

                }

                EnumerationOfT = listOfT;

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        public static Boolean ParseMandatoryJSON<T>(this JObject                             JSON,
                                                    String                                   PropertyName,
                                                    String                                   PropertyDescription,
                                                    TryJObjectParser3a<T>                    TryJObjectParser,
                                                    [NotNullWhen(true)]  out IEnumerable<T>  EnumerationOfT,
                                                    [NotNullWhen(false)] out String?         ErrorResponse,
                                                    CustomJObjectParserDelegate<T>?          CustomParser = null)
        {

            EnumerationOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                var ListOfT        = new List<T>();
                var errorResponse  = "";

                foreach (var item in JArray)
                {

                    if (item is JObject itemJObject &&
                        TryJObjectParser(itemJObject, out var itemT, out errorResponse, CustomParser) &&
                        itemT is not null)
                    {
                        ListOfT.Add(itemT);
                    }

                    if (errorResponse is not null)
                    {
                        ErrorResponse = $"Invalid JSON property '{PropertyDescription ?? PropertyName}': {errorResponse}";
                        return false;
                    }

                }

                EnumerationOfT = ListOfT;

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        public static Boolean ParseMandatoryJSON<T, TId>(this JObject                             JSON,
                                                         String                                   PropertyName,
                                                         String                                   PropertyDescription,
                                                         TryJObjectParser3b<T, TId>               TryJObjectParser,
                                                         [NotNullWhen(true)]  out IEnumerable<T>  EnumerationOfT,
                                                         [NotNullWhen(false)] out String?         ErrorResponse,
                                                         CustomJObjectParserDelegate<T>?          CustomParser = null)

            where TId : struct

        {

            EnumerationOfT = [];

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = $"Missing property '{PropertyName}'!";
                return false;
            }

            try
            {

                if (JSONToken is not JArray JArray)
                {
                    ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                    return false;
                }

                var ListOfT        = new List<T>();
                var errorResponse  = "";

                foreach (var item in JArray)
                {

                    if (item is JObject itemJObject &&
                        TryJObjectParser(itemJObject, out var itemT, out errorResponse, null, CustomParser) &&
                        itemT is not null)
                    {
                        ListOfT.Add(itemT);
                    }

                    if (errorResponse is not null)
                    {
                        ErrorResponse = $"Invalid JSON property '{PropertyDescription ?? PropertyName}': {errorResponse}";
                        return false;
                    }

                }

                EnumerationOfT = ListOfT;

            }
            catch
            {
                ErrorResponse = $"Invalid '{PropertyDescription ?? PropertyName}'!";
                return false;
            }

            ErrorResponse = null;
            return true;

        }

        //public static Boolean ParseMandatoryJSON<T>(this JObject          JSON,
        //                                            String                PropertyName,
        //                                            String                PropertyDescription,
        //                                            TryJObjectParser4<T>  TryJObjectParser,
        //                                            out IEnumerable<T>    EnumerationOfT,
        //                                            out String            ErrorResponse)
        //{

        //    EnumerationOfT = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "Invalid JSON provided!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (!JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {
        //        ErrorResponse = "Missing property '" + PropertyName + "'!";
        //        return false;
        //    }

        //    try
        //    {

        //        if (!(JSONToken is JArray JArray))
        //        {
        //            ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //            return false;
        //        }

        //        var ListOfT = new List<T>();

        //        foreach (var item in JArray)
        //        {
        //            if (item is JObject && TryJObjectParser(item as JObject, out T ItemT, null))
        //                ListOfT.Add(ItemT);
        //        }

        //        EnumerationOfT = ListOfT;

        //    }
        //    catch
        //    {
        //        ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
        //        return false;
        //    }

        //    ErrorResponse = null;
        //    return true;

        //}

        #endregion


        #region ParseMandatoryJSONArray (this JSON, PropertyName, PropertyDescription,            TryJArrayParser,   out Value,                  out ErrorResponse)

        public static Boolean ParseMandatoryJSONArray<T>(this JObject                      JSON,
                                                         String                            PropertyName,
                                                         String                            PropertyDescription,
                                                         TryJArrayParser<T>                TryJArrayParser,
                                                         [NotNullWhen(true)]  out T?       Value,
                                                         [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse  = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse  = "Invalid JSON property name provided!";
                return false;
            }

            if (TryJArrayParser is null)
            {
                ErrorResponse  = "Invalid parser provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse  = $"Missing JSON property '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (JSONToken is not JArray JSONValue)
            {
                ErrorResponse  = $"Invalid JSON array '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (TryJArrayParser(JSONValue, out var value))
            {
                Value          = value;
                ErrorResponse  = null;
                return true;
            }

            ErrorResponse = $"JSON property '{PropertyName}' ({PropertyDescription}) could not be parsed!";
            return false;

        }

        public static Boolean ParseMandatoryJSONArray<T>(this JObject                      JSON,
                                                         String                            PropertyName,
                                                         String                            PropertyDescription,
                                                         TryJArrayParser2<T>               TryJArrayParser,
                                                         [NotNullWhen(true)]  out T?       Value,
                                                         [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Value = default;

            if (JSON is null)
            {
                ErrorResponse  = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse  = "Invalid JSON property name provided!";
                return false;
            }

            if (TryJArrayParser is null)
            {
                ErrorResponse  = "Invalid parser provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse  = $"Missing JSON property '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (JSONToken is not JArray JSONValue)
            {
                ErrorResponse  = $"Invalid JSON object '{PropertyName}' ({PropertyDescription})!";
                return false;
            }

            if (TryJArrayParser(JSONValue,
                                out var value,
                                out var errorResponse))
            {
                Value          = value;
                ErrorResponse  = null;
                return true;
            }

            ErrorResponse  = $"JSON property '{PropertyName}' ({PropertyDescription}) could not be parsed: {errorResponse}";
            return false;

        }

        #endregion



        // Mandatory multiple values...

        #region GetMandatory(this JSON, Key, out Values)

        public static Boolean GetMandatory(this JObject             JSON,
                                           String                   Key,
                                           out IEnumerable<String>  Values)
        {

            if (JSON.TryGetValue(Key, out var JSONToken) && 
                JSONToken is JArray _Values)
            {

                Values = _Values.AsEnumerable().
                                 Select(jtoken => jtoken.Value<String>()).
                                 Where (value  => value != null);

                return true;

            }

            Values = null;
            return false;

        }

        #endregion



        public static Boolean ParseMandatory<T>(this JObject      JSON,
                                                String            PropertyName,
                                                Func<String, T>   Mapper,
                                                T                 InvalidResult,
                                                out T             TOut)
        {

            if (JSON is null ||
                PropertyName.IsNullOrEmpty() ||
                Mapper is null)
            {
                TOut = default;
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out JToken? _JToken) && _JToken?.Value<String>() != null)
            {

                try
                {

                    TOut = Mapper(_JToken?.Value<String>());

                    return !TOut.Equals(InvalidResult);

                }
                catch
                { }

            }

            TOut = default;
            return false;

        }




        // -------------------------------------------------------------------------------------------------------------------------------------
        // Parse Optional
        // -------------------------------------------------------------------------------------------------------------------------------------

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out StringValue,            out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out String?   StringValue,
                                            out String?   ErrorResponse)
        {

            StringValue    = null;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                try
                {
                    StringValue = JSONToken?.Value<String>();
                }
                catch
                {
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";
                }

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional...    (this JSON, PropertyName, PropertyDescription,                               out Numbers...,             out ErrorResponse)

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out Boolean,                out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Boolean?  BooleanValue,
                                            out String?   ErrorResponse)
        {

            BooleanValue   = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                try
                {
                    BooleanValue = JSONToken?.Value<Boolean>();
                }
                catch
                {
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";
                }

                if (!BooleanValue.HasValue)
                    return false;

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out Single/Double,          out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Single?   SingleValue,
                                            out String?   ErrorResponse)
        {

            SingleValue    = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Single.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out Single value))
                    SingleValue = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }


        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Double?   DoubleValue,
                                            out String?   ErrorResponse)
        {

            DoubleValue    = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Double.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out Double value))
                    DoubleValue = value;

                else
                    ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out Decimal,                out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Decimal?  DecimalValue,
                                            out String?   ErrorResponse)
        {

            DecimalValue   = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal value))
                    DecimalValue = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out (S)Byte,                out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Byte?     ByteValue,
                                            out String?   ErrorResponse)
        {

            ByteValue      = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Byte.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out Byte value))
                    ByteValue = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }


        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out SByte?    SByteValue,
                                            out String?   ErrorResponse)
        {

            SByteValue     = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (SByte.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out SByte value))
                    SByteValue = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out (U)Int16/32/64,         out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Int16?    Int16Value,
                                            out String?   ErrorResponse)
        {

            Int16Value     = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Int16.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out Int16 value))
                    Int16Value = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }


        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out UInt16?   UInt16Value,
                                            out String?   ErrorResponse)
        {

            UInt16Value    = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (UInt16.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out UInt16 value))
                    UInt16Value = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }


        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Int32?    Int32Value,
                                            out String?   ErrorResponse)
        {

            Int32Value     = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Int32.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out Int32 value))
                    Int32Value = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }


        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out UInt32?   UInt32Value,
                                            out String?   ErrorResponse)
        {

            UInt32Value    = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (UInt32.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out UInt32 value))
                    UInt32Value = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }


        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Int64?    Int64Value,
                                            out String?   ErrorResponse)
        {

            Int64Value     = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Int64.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out Int64 value))
                    Int64Value = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }


        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out UInt64?   UInt64Value,
                                            out String?   ErrorResponse)
        {

            UInt64Value    = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (UInt64.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out UInt64 value))
                    UInt64Value = value;

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion


        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out Ampere,                 out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Ampere?   AmpereValue,
                                            out String?   ErrorResponse,
                                            Int32?        Multiplicator = null)
        {

            AmpereValue    = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    AmpereValue   = Ampere.ParseA(decimalValue, Multiplicator);

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out Hertz,                  out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Hertz?    HertzValue,
                                            out String?   ErrorResponse,
                                            Int32?        Multiplicator = null)
        {

            HertzValue     = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    HertzValue    = Hertz.ParseHz(decimalValue, Multiplicator);

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out Meter,                  out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Meter?    MeterValue,
                                            out String?   ErrorResponse,
                                            Int32?        Multiplicator = null)
        {

            MeterValue     = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    MeterValue    = Meter.ParseM(decimalValue, Multiplicator);

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out Percentage,             out ErrorResponse)

        public static Boolean ParseOptional(this JObject     JSON,
                                            String           PropertyName,
                                            String           PropertyDescription,
                                            out Percentage?  PercentageValue,
                                            out String?      ErrorResponse)
        {

            PercentageValue  = default;
            ErrorResponse    = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    PercentageValue = Percentage.Parse(decimalValue);

                else
                    ErrorResponse   = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out PercentageByte,         out ErrorResponse)

        public static Boolean ParseOptional(this JObject         JSON,
                                            String               PropertyName,
                                            String               PropertyDescription,
                                            out PercentageByte?  PercentageValue,
                                            out String?          ErrorResponse)
        {

            PercentageValue  = default;
            ErrorResponse    = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Byte.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var byteValue))
                    PercentageValue = PercentageByte.Parse(byteValue);

                else
                    ErrorResponse   = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out PercentageDouble,       out ErrorResponse)

        public static Boolean ParseOptional(this JObject           JSON,
                                            String                 PropertyName,
                                            String                 PropertyDescription,
                                            out PercentageDouble?  PercentageValue,
                                            out String?            ErrorResponse)
        {

            PercentageValue  = default;
            ErrorResponse    = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Double.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleValue))
                    PercentageValue = PercentageDouble.Parse(doubleValue);

                else
                    ErrorResponse   = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out Volt,                   out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Volt?     VoltValue,
                                            out String?   ErrorResponse,
                                            Int32?        Multiplicator = null)
        {

            VoltValue      = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    VoltValue     = Volt.ParseV(decimalValue, Multiplicator);

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out Watt,                   out ErrorResponse)

        public static Boolean ParseOptional(this JObject  JSON,
                                            String        PropertyName,
                                            String        PropertyDescription,
                                            out Watt?     WattValue,
                                            out String?   ErrorResponse,
                                            Int32?        Multiplicator = null)
        {

            WattValue      = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    WattValue     = Watt.ParseW(decimalValue, Multiplicator);

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                               out WattHour,               out ErrorResponse)

        public static Boolean ParseOptional(this JObject   JSON,
                                            String         PropertyName,
                                            String         PropertyDescription,
                                            out WattHour?  WattHourValue,
                                            out String?    ErrorResponse,
                                            Int32?         Multiplicator = null)
        {

            WattHourValue  = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                    WattHourValue = WattHour.TryParseWh(decimalValue, Multiplicator);

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #endregion

        #region ParseOptionalStruct (this JSON, PropertyName, PropertyDescription,                    Parser(s), out Value,                out ErrorResponse)

        public static Boolean ParseOptionalStruct<TStruct>(this JObject        JSON,
                                                           String              PropertyName,
                                                           String              PropertyDescription,
                                                           TryParser<TStruct>  Parser,
                                                           out TStruct?        Value,
                                                           out String?         ErrorResponse)

           // where TStruct : struct

        {

            Value         = default;//new TStruct?();
            ErrorResponse = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (JSONToken.Type == JTokenType.Object && !JSONToken.HasValues)
                    return false;

                if (!Parser(JSONToken.Type == JTokenType.String
                                ? JSONToken?.Value<String>() ?? ""
                                : JSONToken.ToString(),
                            out var value))
                {
                    ErrorResponse = "The value '" + JSONToken + "' is not valid for JSON property '" + PropertyDescription + "'!";
                }

                Value = value;
                return true;

            }

            return false;

        }

        public static Boolean ParseOptionalStruct<TStruct>(this JObject         JSON,
                                                           String               PropertyName,
                                                           String               PropertyDescription,
                                                           TryParser2<TStruct>  Parser,
                                                           out TStruct?         Value,
                                                           out String           ErrorResponse)

            where TStruct : struct

        {

            Value         = new TStruct?();
            ErrorResponse = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (JSONToken.Type == JTokenType.Object && !JSONToken.HasValues)
                    return false;

                if (!Parser(JSONToken.Type == JTokenType.String
                                ? JSONToken.Value<String>()
                                : JSONToken.ToString(),
                            out TStruct value,
                            out         ErrorResponse))
                {
                    ErrorResponse = "The value '" + JSONToken + "' is not valid for JSON property '" + PropertyDescription + "'!";
                }

                Value = value;
                return true;

            }

            return false;

        }


        public static Boolean ParseOptionalStruct<TStruct>(this JObject         JSON,
                                                           String               PropertyName,
                                                           String               PropertyDescription,
                                                           TryParser4<TStruct>  Parser,
                                                           out TStruct?         Value,
                                                           out String?          ErrorResponse,
                                                           OnExceptionDelegate  OnException)

            where TStruct : struct

        {

            Value         = new TStruct?();
            ErrorResponse = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (JSONToken.Type == JTokenType.Object && !JSONToken.HasValues)
                    return false;

                if (!Parser(JSONToken.Type == JTokenType.String
                                ? JSONToken?.Value<String>() ?? ""
                                : JSONToken.ToString(),
                            out TStruct value,
                            OnException))
                {
                    ErrorResponse = "The value '" + JSONToken + "' is not valid for JSON property '" + PropertyDescription + "'!";
                }

                Value = value;
                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptionalEnum   (this JSON, PropertyName, PropertyDescription,                            out EnumValue,               out ErrorResponse)

        public static Boolean ParseOptionalEnum<TEnum>(this JObject  JSON,
                                                       String        PropertyName,
                                                       String        PropertyDescription,
                                                       out TEnum?    EnumValue,
                                                       out String?   ErrorResponse)

            where TEnum : struct

        {

            EnumValue      = null;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                var JSONValue = JSONToken?.Value<String>();
                if (JSONValue is null)
                {
                    ErrorResponse  = "Unknown '" + (PropertyDescription ?? PropertyName) + "'!";
                    EnumValue      = null;
                    return false;
                }

                if (Enum.TryParse(JSONValue, true, out TEnum enumValue))
                {
                    EnumValue      = enumValue;
                    ErrorResponse  = null;
                }

                else
                {
                    ErrorResponse  = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";
                    EnumValue      = null;
                }

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptionalEnums  (this JSON, PropertyName, PropertyDescription,                            out IEnumerable<Enum>,       out ErrorResponse)

        public static Boolean ParseOptionalEnums<TEnum>(this JObject        JSON,
                                                        String              PropertyName,
                                                        String              PropertyDescription,
                                                        out HashSet<TEnum>  EnumValues,
                                                        out String?         ErrorResponse)

            where TEnum : struct

        {

            EnumValues     = new HashSet<TEnum>();
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (!JSON.TryGetValue(PropertyName, out JToken? JSONToken))
            {
                ErrorResponse = "Missing property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is not JArray JSONArray)
            {
                ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            foreach (var JSONItem in JSONArray)
            {

                if (Enum.TryParse(JSONItem?.Value<String>(), true, out TEnum enumValue))
                    EnumValues.Add(enumValue);

                else
                {
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";
                    return false;
                }

            }

            return true;

        }

        #endregion

        #region ParseOptionalStruct (this JSON, PropertyName, PropertyDescription,                            out IEnumerable<T>,          out ErrorResponse)

        public static Boolean ParseOptionalStruct<T>(this JObject        JSON,
                                                     String              PropertyName,
                                                     String              PropertyDescription,
                                                     TryParser<T>        TryParser,
                                                     out IEnumerable<T>  Values,
                                                     out String          ErrorResponse)

            where T : struct

        {

            Values         = new T[0];
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (JSONToken.Type != JTokenType.Array)
                {
                    ErrorResponse  = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                    return false;
                }

                var JSONList = JSONToken as JArray;
                var List     = new List<T>();

                foreach (var JSONItem in JSONList)
                {

                    if (TryParser(JSONItem?.Value<String>(), out T Value))
                        List.Add(Value);

                    else
                    {
                        ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";
                        return false;
                    }

                }

                Values = List;

            }

            return true;

        }

        #endregion


        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out Timestamp,               out ErrorResponse)

        public static Boolean ParseOptional(this JObject   JSON,
                                            String         PropertyName,
                                            String         PropertyDescription,
                                            out DateTime?  Timestamp,
                                            out String     ErrorResponse)
        {

            Timestamp      = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                try
                {
                    Timestamp = JSONToken.Value<DateTime>();
                }
                catch
                {
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";
                }

                return true;

            }

            return false;

        }


        public static Boolean ParseOptional(this JObject               JSON,
                                            String                     PropertyName,
                                            String                     PropertyDescription,
                                            out IEnumerable<DateTime>  Timestamps,
                                            out String                 ErrorResponse)
        {

            Timestamps     = null;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type == JTokenType.Array)
            {

                try
                {

                    var _timestamps = new List<DateTime>();

                    foreach (var timestamp in (JSONToken as JArray).AsEnumerable())
                        _timestamps.Add(JSONToken.Value<DateTime>());

                    Timestamps = _timestamps;

                    return true;

                }
                catch
                {
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";
                }

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out TimeSpan,                out ErrorResponse)

        public static Boolean ParseOptional(this JObject   JSON,
                                            String         PropertyName,
                                            String         PropertyDescription,
                                            out TimeSpan?  Timespan,
                                            out String?    ErrorResponse)
        {

            Timespan       = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = $"Invalid JSON property '{PropertyDescription ?? PropertyName}' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type is not JTokenType.Null)
            {

                try
                {
                    Timespan = TimeSpan.FromSeconds(JSONToken.Value<UInt64>());
                }
                catch
                {
                    ErrorResponse = $"Invalid value for '{PropertyDescription ?? PropertyName}'!";
                }

                return true;

            }

            ErrorResponse = $"Invalid value for '{PropertyDescription ?? PropertyName}'!";
            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out I18NText,                out ErrorResponse)

        public static Boolean ParseOptional(this JObject     JSON,
                                            String           PropertyName,
                                            String           PropertyDescription,
                                            out I18NString?  I18NText,
                                            out String?      ErrorResponse)

        {

            I18NText       = I18NString.Empty;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (JSONToken is not JObject i18NJSON)
                {
                    ErrorResponse = "JSON property '" + (PropertyDescription ?? PropertyName) + "' is not a I18N string!";
                    return true;
                }

                foreach (var jproperty in i18NJSON)
                {

                    try
                    {

                        if (Enum.TryParse<Languages>(jproperty.Key, out var language))
                        {

                            if (jproperty.Value?.Value<String>() is String text)
                            {
                                I18NText.Set(language, text);
                            }
                            else
                            {
                                ErrorResponse = "Invalid text value '" + jproperty.Value + "'!";
                                return true;
                            }

                        }
                        else
                        {
                            ErrorResponse = "Invalid language '" + jproperty.Key + "'!";
                            return true;
                        }

                    }
                    catch
                    {
                        ErrorResponse = "Invalid I18N value for '" + (PropertyDescription ?? PropertyName) + "'!";
                        return true;
                    }

                }

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptionalI18N   (this JSON, PropertyName, PropertyDescription,                    Parser, out IEnumerableOfI18N,       out ErrorResponse)

        public static Boolean ParseOptionalI18N<T>(this JObject          JSON,
                                                   String                PropertyName,
                                                   String                PropertyDescription,
                                                   TryJObjectParser1<T>  Parser,
                                                   out IEnumerable<T>?   IEnumerableOfI18N,
                                                   out String?           ErrorResponse)

            where T: I18NString

        {

            IEnumerableOfI18N  = null;
            ErrorResponse      = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (!(JSONToken is JArray JSONArray))
                {
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
                    return true;
                }

                var List = new List<T>();

                foreach (var element in JSONArray)
                {

                    if (element is null)
                    {
                        ErrorResponse = "A given value within the array is null!";
                        return true;
                    }

                    var item = element.Value<JObject>();
                    if (item is null)
                    {
                        ErrorResponse = "A given value within the array is null or empty!";
                        return true;
                    }

                    if (Parser(item, out T itemT, out var errorResponse))
                        List.Add(itemT);
                    else
                    {
                        ErrorResponse = "A given value within the array is invalid: " + errorResponse;
                        return true;
                    }

                }

                IEnumerableOfI18N = List;

                return true;

            }

            return false;

        }

        #endregion


        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                    Mapper, out Value,                   out ErrorResponse)

        public static Boolean ParseOptional<T>(this JObject     JSON,
                                               String           PropertyName,
                                               String           PropertyDescription,
                                               Func<String, T>  Mapper,
                                               out T?           Value,
                                               out String?      ErrorResponse)
        {

            Value          = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                try
                {

                    Value = JSONToken.Type == JTokenType.String
                                         ? Mapper(JSONToken?.Value<String>() ?? "")
                                         : Mapper(JSONToken.ToString());

                }
                catch
                {
                    ErrorResponse = "Invalid " + PropertyDescription + "!";
                }

                return true;

            }

            return false;

        }

        //public static Boolean ParseOptional<T>(this JObject     JSON,
        //                                       String           PropertyName,
        //                                       String           PropertyDescription,
        //                                       Func<String, T>  Mapper,
        //                                       out T            Value,
        //                                       out String?      ErrorResponse)

        //    where T : struct

        //{

        //    Value          = default;
        //    ErrorResponse  = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "The given JSON object must not be null!";
        //        return true;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return true;
        //    }

        //    if (JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {

        //        // "propertyKey": null -> will be ignored!
        //        if (JSONToken is null || JSONToken.Type == JTokenType.Null)
        //            return false;

        //        try
        //        {

        //            Value = JSONToken.Type == JTokenType.String
        //                                 ? Mapper(JSONToken?.Value<String>() ?? "")
        //                                 : Mapper(JSONToken.ToString());

        //        }
        //        catch
        //        {
        //            ErrorResponse = "Invalid " + PropertyDescription + "!";
        //        }

        //        return true;

        //    }

        //    return false;

        //}

        #endregion

        #region ParseOptional<T>    (this JSON, PropertyName, PropertyDescription,                    Parser, out Value,                   out HTTPResponse)

        public static Boolean ParseOptional<T>(this JObject      JSON,
                                               String            PropertyName,
                                               String            PropertyDescription,
                                               TryParser<T>      Parser,
                                               out T             Value,
                                               out String?       ErrorResponse)
        {

            Value          = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (!Parser(JSONToken.Type == JTokenType.String
                                ? JSONToken.Value<String>()
                                : JSONToken.ToString(),
                            out Value))
                {

                    Value          = default;
                    ErrorResponse  = "The value '" + JSONToken + "' is not valid for JSON property '" + PropertyDescription + "'!";

                }

                return true;

            }

            return false;

        }


        public static Boolean ParseOptional<T>(this JObject      JSON,
                                               String            PropertyName,
                                               String            PropertyDescription,
                                               TryParser2<T>     Parser,
                                               out T             Value,
                                               out String?       ErrorResponse)
        {

            Value          = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (!Parser(JSONToken.Type == JTokenType.String
                                ? JSONToken.Value<String>()
                                : JSONToken.ToString(),
                            out Value,
                            out var ErrorResponse2))
                {

                    Value          = default;
                    ErrorResponse  = "The value '" + JSONToken + "' is not valid for JSON property '" + PropertyDescription + "': " + ErrorResponse2;

                }

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptionalJSON   (this JSON, PropertyName, PropertyDescription,             JObjectParser, out Value,                   out HTTPResponse)

        //public static Boolean ParseOptionalJSON<T>(this JObject         JSON,
        //                                           String               PropertyName,
        //                                           String               PropertyDescription,
        //                                           TryJObjectParser<T>  JObjectParser,
        //                                           out T                Value,
        //                                           out String           ErrorResponse)
        //{

        //    Value          = default;
        //    ErrorResponse  = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "The given JSON object must not be null!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {

        //        // "propertyKey": null -> will be ignored!
        //        if (JSONToken is null || JSONToken.Type == JTokenType.Null)
        //            return false;

        //        if (!(JSONToken is JObject JSON2))
        //            ErrorResponse  = "Invalid " + PropertyDescription + "!";

        //        else if (!JObjectParser(JSON2, out Value))
        //            ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";

        //        return true;

        //    }

        //    return false;

        //}

        //public static Boolean ParseOptionalJSON<T>(this JObject         JSON,
        //                                           String               PropertyName,
        //                                           String               PropertyDescription,
        //                                           TryJObjectParser<T>  JObjectParser,
        //                                           out T?               Value,
        //                                           out String           ErrorResponse)

        //    where T : struct

        //{

        //    Value          = null;
        //    ErrorResponse  = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "The given JSON object must not be null!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {

        //        // "propertyKey": null -> will be ignored!
        //        if (JSONToken is null || JSONToken.Type == JTokenType.Null)
        //            return false;

        //        if (JSONToken is JObject JSON2 &&
        //            JObjectParser(JSON2, out T value))
        //        {
        //            Value = value;
        //            return true;
        //        }

        //        ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";
        //        return true;

        //    }

        //    return false;

        //}


        public static Boolean ParseOptionalJSON<T>(this JObject                      JSON,
                                                   String                            PropertyName,
                                                   String                            PropertyDescription,
                                                   TryJObjectParser2a<T>              JObjectParser,
                                                   [NotNullWhen(true)]  out T?       Value,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Value          = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                {
                    ErrorResponse = $"JSON property '{PropertyName}' must not be null!";
                    return false;
                }

                if (JSONToken is not JObject JSON2)
                {
                    ErrorResponse  = $"JSON property '{PropertyName}' is not an object!";
                    return false;
                }

                if (JObjectParser(JSON2, out Value, out var errorResponse2))
                    return true;

                ErrorResponse  = $"JSON property '{PropertyName}' ({PropertyDescription}) could not be parsed: {errorResponse2}";
                return false;

            }

            //ErrorResponse = "Invalid JSON property!";
            return false;

        }

        public static Boolean ParseOptionalJSON<T>(this JObject          JSON,
                                                   String                PropertyName,
                                                   String                PropertyDescription,
                                                   TryJObjectParser2a<T>  JObjectParser,
                                                   out T?                Value,
                                                   out String?           ErrorResponse)

            where T : struct

        {

            Value          = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (!(JSONToken is JObject JSON2))
                    ErrorResponse  = "Invalid " + PropertyDescription + "!";

                else if (JObjectParser(JSON2, out T value, out var errorResponse2))
                    Value = value;

                else
                    ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed: " + errorResponse2;

                return true;

            }

            return false;

        }

        //public static Boolean ParseOptionalJSON<T>(this JObject          JSON,
        //                                           String                PropertyName,
        //                                           String                PropertyDescription,
        //                                           TryJObjectParser4<T>  JObjectParser,
        //                                           out T                 Value,
        //                                           out String            ErrorResponse,
        //                                           OnExceptionDelegate   OnException)
        //{

        //    Value          = default;
        //    ErrorResponse  = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "The given JSON object must not be null!";
        //        return false;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return false;
        //    }

        //    if (JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {

        //        // "propertyKey": null -> will be ignored!
        //        if (JSONToken is null || JSONToken.Type == JTokenType.Null)
        //            return false;

        //        if (!(JSONToken is JObject JSON2))
        //            ErrorResponse  = "Invalid " + PropertyDescription + "!";

        //        else if (!JObjectParser(JSON2, out Value, OnException))
        //            ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";

        //        return true;

        //    }

        //    return false;

        //}

        #endregion

        #region ParseOptionalJSONArray(this JSON, PropertyName, PropertyDescription,           JArrayParser,  out Value,                   out HTTPResponse)

        public static Boolean ParseOptionalJSONArray<T>(this JObject        JSON,
                                                        String              PropertyName,
                                                        String              PropertyDescription,
                                                        TryJArrayParser<T>  JArrayParser,
                                                        out T?              Value,
                                                        out String?         ErrorResponse)
        {

            Value          = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSON2)
                    ErrorResponse  = "Invalid " + PropertyDescription + "!";

                else if (!JArrayParser(JSON2, out Value))
                    ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";

                return true;

            }

            return false;

        }

        public static Boolean ParseOptionalJSONArray<T>(this JObject        JSON,
                                                        String              PropertyName,
                                                        String              PropertyDescription,
                                                        TryJArrayParser<T>  JArrayParser,
                                                        out T?              Value,
                                                        out String?         ErrorResponse)

            where T : struct

        {

            Value          = null;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is JArray JSON2 &&
                    JArrayParser(JSON2, out T value))
                {
                    Value = value;
                    return true;
                }

                ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed!";
                return true;

            }

            return false;

        }


        public static Boolean ParseOptionalJSONArray<T>(this JObject         JSON,
                                                        String               PropertyName,
                                                        String               PropertyDescription,
                                                        TryJArrayParser2<T>  JArrayParser,
                                                        out T                Value,
                                                        out String?          ErrorResponse)
        {

            Value          = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (!(JSONToken is JArray JSON2))
                    ErrorResponse  = "Invalid " + PropertyDescription + "!";

                else if (!JArrayParser(JSON2, out Value, out var ErrorResponse2))
                    ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed: " + ErrorResponse2;

                return true;

            }

            return false;

        }

        public static Boolean ParseOptionalJSONArray<T>(this JObject         JSON,
                                                        String               PropertyName,
                                                        String               PropertyDescription,
                                                        TryJArrayParser2<T>  JArrayParser,
                                                        out T?               Value,
                                                        out String?          ErrorResponse)

            where T : struct

        {

            Value          = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (!(JSONToken is JArray JSON2))
                    ErrorResponse  = "Invalid " + PropertyDescription + "!";

                else if (JArrayParser(JSON2, out T value, out var ErrorResponse2))
                    Value = value;

                else
                    ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed: " + ErrorResponse2;

                return true;

            }

            return false;

        }



        public static Boolean ParseOptionalJSONArrayX<T>(this JObject         JSON,
                                                        String               PropertyName,
                                                        String               PropertyDescription,
                                                        TryJArrayParser3<T>  JArrayParser,
                                                        Func<JToken, T>      Parser,
                                                        out StdDev<T>?       Value,
                                                        out String?          ErrorResponse)

            where T : struct

        {

            Value          = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (!(JSONToken is JArray JSON2))
                    ErrorResponse  = "Invalid " + PropertyDescription + "!";

                //else if (JArrayParser(JSON2, out StdDev<T>? value, out String? ErrorResponse2, Parser))
                //    Value = value;

                //else
                //    ErrorResponse  = "JSON property '" + PropertyName + "' (" + PropertyDescription + ") could not be parsed: " + ErrorResponse2;

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out JSONObject,              out ErrorResponse)

        public static Boolean ParseOptional(this JObject    JSON,
                                            String          PropertyName,
                                            String          PropertyDescription,
                                            out JObject     JSONObject,
                                            out String      ErrorResponse)

        {

            JSONObject     = new JObject();
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                JSONObject = JSONToken as JObject;

                if (JSONObject is null)
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON object!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptional       (this JSON, PropertyName, PropertyDescription,                            out JSONArray,               out ErrorResponse)

        public static Boolean ParseOptional(this JObject    JSON,
                                            String          PropertyName,
                                            String          PropertyDescription,
                                            out JArray      JSONArray,
                                            out String      ErrorResponse)

        {

            JSONArray      = new JArray();
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                JSONArray = JSONToken as JArray;

                if (JSONArray is null)
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseOptionalJSON   (this JSON, PropertyName, PropertyDescription,                    Parser, out EnumerableT,             out ErrorResponse)

        public static Boolean ParseOptionalJSON<T>(this JObject        JSON,
                                                   String              PropertyName,
                                                   String              PropertyDescription,
                                                   TryParser<T>        Parser,
                                                   out IEnumerable<T>  EnumerableT,
                                                   out String?         ErrorResponse)

        {

            EnumerableT    = [];
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = $"The given property '{PropertyName}' is not a valid JSON array!";
                    return true;
                }

                var list = new List<T>();

                foreach (var element in JSONArray)
                {

                    if (element is null)
                    {
                        ErrorResponse = "A given value within the array is null!";
                        return true;
                    }

                    var item = element.Value<String>()?.Trim();

                    if (item.IsNullOrEmpty())
                    {
                        ErrorResponse = "A given value within the array is null or empty!";
                        return true;
                    }

                    if (Parser(item, out var itemT) && itemT is not null)
                        list.Add(itemT);

                }

                EnumerableT = list;
                return true;

            }

            return false;

        }


        public static Boolean ParseOptionalJSON<T>(this JObject        JSON,
                                                   String              PropertyName,
                                                   String              PropertyDescription,
                                                   TryParser2<T>       Parser,
                                                   out IEnumerable<T>  EnumerableT,
                                                   out String?         ErrorResponse)

        {

            EnumerableT    = [];
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = $"The given property '{PropertyName}' is not a valid JSON array!";
                    return true;
                }

                var list = new List<T>();

                foreach (var element in JSONArray)
                {

                    if (element is null)
                    {
                        ErrorResponse = "A given value within the array is null!";
                        return true;
                    }

                    var item = element.Value<String>()?.Trim();

                    if (item.IsNullOrEmpty())
                    {
                        ErrorResponse = "A given value within the array is null or empty!";
                        return true;
                    }

                    if (Parser(item, out var itemT, out ErrorResponse) && itemT is not null)
                        list.Add(itemT);

                }

                EnumerableT = list;
                return true;

            }

            return false;

        }


        //public static Boolean ParseOptionalJSON<T>(this JObject         JSON,
        //                                           String               PropertyName,
        //                                           String               PropertyDescription,
        //                                           TryParser4<T>        Parser,
        //                                           out IEnumerable<T>   EnumerableT,
        //                                           out String           ErrorResponse,
        //                                           OnExceptionDelegate  OnException)

        //{

        //    EnumerableT    = null;
        //    ErrorResponse  = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "The given JSON object must not be null!";
        //        return true;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return true;
        //    }

        //    if (JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {

        //        // "propertyKey": null -> will be ignored!
        //        if (JSONToken is null || JSONToken.Type == JTokenType.Null)
        //            return false;

        //        if (!(JSONToken is JArray JSONArray))
        //        {
        //            ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
        //            return true;
        //        }

        //        var list = new List<T>();

        //        foreach (var element in JSONArray)
        //        {

        //            if (element is null)
        //            {
        //                ErrorResponse = "A given value within the array is null!";
        //                return true;
        //            }

        //            var item = element.Value<String>()?.Trim();

        //            if (item.IsNullOrEmpty())
        //            {
        //                ErrorResponse = "A given value within the array is null or empty!";
        //                return true;
        //            }

        //            if (Parser(item, out T itemT, OnException))
        //                list.Add(itemT);

        //        }

        //        EnumerableT = list;
        //        return true;

        //    }

        //    return false;

        //}




        //public static Boolean ParseOptionalJSON<T>(this JObject         JSON,
        //                                           String               PropertyName,
        //                                           String               PropertyDescription,
        //                                           TryJObjectParser<T>  Parser,
        //                                           out IEnumerable<T>   EnumerableT,
        //                                           out String           ErrorResponse)

        //{

        //    EnumerableT    = null;
        //    ErrorResponse  = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "The given JSON object must not be null!";
        //        return true;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return true;
        //    }

        //    if (JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {

        //        // "propertyKey": null -> will be ignored!
        //        if (JSONToken is null || JSONToken.Type == JTokenType.Null)
        //            return false;

        //        if (!(JSONToken is JArray JSONArray))
        //        {
        //            ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
        //            return true;
        //        }

        //        var list = new List<T>();

        //        foreach (var element in JSONArray)
        //        {

        //            if (element is null)
        //            {
        //                ErrorResponse = "A given value within the array is null!";
        //                return true;
        //            }

        //            if (!(element is JObject JSONObject))
        //            {
        //                ErrorResponse = "The given token is not a valid JSON object!";
        //                return true;
        //            }

        //            if (Parser(element as JObject, out T itemT))
        //                list.Add(itemT);

        //        }

        //        EnumerableT = list;
        //        return true;

        //    }

        //    return false;

        //}

        public static Boolean ParseOptionalJSON<T>(this JObject                             JSON,
                                                   String                                   PropertyName,
                                                   String                                   PropertyDescription,
                                                   TryJObjectParser2a<T>                    Parser,
                                                   [NotNullWhen(true)]  out IEnumerable<T>  EnumerableT,
                                                   [NotNullWhen(false)] out String?         ErrorResponse)

        {

            EnumerableT    = [];
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
                    return true;
                }

                var list            = new List<T>();
                var errorResponses  = new List<String>();

                foreach (var element in JSONArray)
                {

                    if (element is not JObject JSONObject)
                    {
                        ErrorResponse = "The given token is not a valid JSON object!";
                        return true;
                    }

                    if (Parser(JSONObject,
                               out var itemT,
                               out var errorResponse) && itemT is not null)
                    {
                        list.Add(itemT);
                    }
                    else
                        errorResponses.Add(errorResponse ?? "Could not parse the given item!");

                }

                EnumerableT = list;

                if (errorResponses.Any())
                    ErrorResponse = errorResponses.AggregateWith(Environment.NewLine);

                return true;

            }

            return false;

        }

        public static Boolean Parallel_ParseOptionalJSON<T>(this JObject           JSON,
                                                            String                 PropertyName,
                                                            String                 PropertyDescription,
                                                            TryJObjectParser2a<T>  Parser,
                                                            out IEnumerable<T>     EnumerableT,
                                                            out String?            ErrorResponse)

        {

            EnumerableT    = [];
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
                    return true;
                }

                var list            = new List<T>();
                var errorResponses  = new List<String>();

                foreach (var element in JSONArray)
                {

                    if (element is not JObject JSONObject)
                    {
                        ErrorResponse = "The given token is not a valid JSON object!";
                        return true;
                    }

                    if (Parser(JSONObject,
                               out var itemT,
                               out var errorResponse) && itemT is not null)
                    {
                        list.Add(itemT);
                    }
                    else
                        errorResponses.Add(errorResponse ?? "Could not parse the given item!");

                }

                EnumerableT = list;

                if (errorResponses.Any())
                    ErrorResponse = errorResponses.AggregateWith(Environment.NewLine);

                return true;

            }

            return false;

        }

        //public static Boolean ParseOptionalJSON<T>(this JObject          JSON,
        //                                           String                PropertyName,
        //                                           String                PropertyDescription,
        //                                           TryJObjectParser4<T>  Parser,
        //                                           out IEnumerable<T>    EnumerableT,
        //                                           out String            ErrorResponse,
        //                                           OnExceptionDelegate   OnException)

        //{

        //    EnumerableT    = null;
        //    ErrorResponse  = null;

        //    if (JSON is null)
        //    {
        //        ErrorResponse = "The given JSON object must not be null!";
        //        return true;
        //    }

        //    if (PropertyName.IsNullOrEmpty())
        //    {
        //        ErrorResponse = "Invalid JSON property name provided!";
        //        return true;
        //    }

        //    if (JSON.TryGetValue(PropertyName, out var JSONToken))
        //    {

        //        // "propertyKey": null -> will be ignored!
        //        if (JSONToken is null || JSONToken.Type == JTokenType.Null)
        //            return false;

        //        if (!(JSONToken is JArray JSONArray))
        //        {
        //            ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
        //            return true;
        //        }

        //        var list = new List<T>();

        //        foreach (var element in JSONArray)
        //        {

        //            if (element is null)
        //            {
        //                ErrorResponse = "A given value within the array is null!";
        //                return true;
        //            }

        //            if (!(element is JObject JSONObject))
        //            {
        //                ErrorResponse = "The given token is not a valid JSON object!";
        //                return true;
        //            }

        //            if (Parser(element as JObject, out T itemT, OnException))
        //                list.Add(itemT);

        //        }

        //        EnumerableT = list;
        //        return true;

        //    }

        //    return false;

        //}

        #endregion

        #region ParseOptionalHashSet(this JSON, PropertyName, PropertyDescription,                    Parser, out HashSet,                 out ErrorResponse)

        public static Boolean ParseOptionalHashSet<T>(this JObject    JSON,
                                                      String          PropertyName,
                                                      String          PropertyDescription,
                                                      Parser<T>       Parser,
                                                      out HashSet<T>  HashSet,
                                                      out String?     ErrorResponse)

        {

            HashSet        = new HashSet<T>();
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
                    return true;
                }


                var errorResponses = new List<String>();

                foreach (var element in JSONArray)
                {

                    if (element is null)
                    {
                        ErrorResponse = "A given value within the array is null!";
                        return true;
                    }

                    var item = element.Value<String>();
                    if (item is not null)
                        item = item.Trim();

                    if (item is null || item.IsNullOrEmpty())
                    {
                        ErrorResponse = "A given value within the array is null or empty!";
                        return true;
                    }

                    var itemT = Parser(item);

                    if (itemT is not null)
                        HashSet.Add(itemT);

                    else
                        errorResponses.Add("Could not parse the given item!");

                }

                if (errorResponses.Any())
                    ErrorResponse = errorResponses.AggregateWith(Environment.NewLine);

                return true;

            }

            return false;

        }

        public static Boolean ParseOptionalHashSet<T>(this JObject    JSON,
                                                      String          PropertyName,
                                                      String          PropertyDescription,
                                                      TryParser<T>    Parser,
                                                      out HashSet<T>  HashSet,
                                                      out String?     ErrorResponse)

        {

            HashSet        = new HashSet<T>();
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
                    return true;
                }


                var errorResponses = new List<String>();

                foreach (var element in JSONArray)
                {

                    if (element is null)
                    {
                        ErrorResponse = "A given value within the array is null!";
                        return true;
                    }

                    var item = element.Value<String>();
                    if (item is not null)
                        item = item.Trim();

                    if (item is null || item.IsNullOrEmpty())
                    {
                        ErrorResponse = "A given value within the array is null or empty!";
                        return true;
                    }

                    if (Parser(item,
                               out var itemT) && itemT is not null)
                    {
                        HashSet.Add(itemT);
                    }
                    else
                        errorResponses.Add("Could not parse the given item!");

                }

                if (errorResponses.Any())
                    ErrorResponse = errorResponses.AggregateWith(Environment.NewLine);

                return true;

            }

            return false;

        }


        public static Boolean ParseOptionalHashSet<T>(this JObject    JSON,
                                                      String          PropertyName,
                                                      String          PropertyDescription,
                                                      TryParser2<T>   Parser,
                                                      out HashSet<T>  HashSet,
                                                      out String?     ErrorResponse)

        {

            HashSet        = new HashSet<T>();
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = "The given property '" + PropertyName + "' is not a valid JSON array!";
                    return true;
                }


                var errorResponses = new List<String>();

                foreach (var element in JSONArray)
                {

                    if (element is null)
                    {
                        ErrorResponse = "A given value within the array is null!";
                        return true;
                    }

                    var item = element.Value<String>();
                    if (item is not null)
                        item = item.Trim();

                    if (item is null || item.IsNullOrEmpty())
                    {
                        ErrorResponse = "A given value within the array is null or empty!";
                        return true;
                    }

                    if (Parser(item,
                               out var itemT,
                               out var errorResponse) && itemT is not null)
                    {
                        HashSet.Add(itemT);
                    }
                    else
                        errorResponses.Add(errorResponse ?? "Could not parse the given item!");

                }

                if (errorResponses.Any())
                    ErrorResponse = errorResponses.AggregateWith(Environment.NewLine);

                return true;

            }

            return false;

        }


        public static Boolean ParseOptionalHashSet<T>(this JObject                         JSON,
                                                      String                               PropertyName,
                                                      String                               PropertyDescription,
                                                      TryJObjectParser2a<T>                 Parser,
                                                      [NotNullWhen(true)]  out HashSet<T>  HashSet,
                                                                           out String?     ErrorResponse)

        {

            HashSet        = [];
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = $"The given property '{PropertyName}' is not a valid JSON array!";
                    return true;
                }


                var errorResponses = new List<String>();

                foreach (var element in JSONArray)
                {

                    if (element is not JObject JSONObject)
                    {
                        ErrorResponse = "The given token is not a valid JSON object!";
                        return true;
                    }

                    if (Parser(JSONObject,
                               out var itemT,
                               out var errorResponse) && itemT is not null)
                    {
                        HashSet.Add(itemT);
                    }
                    else
                        errorResponses.Add(errorResponse ?? "Could not parse the given item!");

                }

                if (errorResponses.Any())
                    ErrorResponse = errorResponses.AggregateWith(Environment.NewLine);

                return true;

            }

            return false;

        }



        public static Boolean ParseOptionalHashSetNull<T>(this JObject                         JSON,
                                                          String                               PropertyName,
                                                          String                               PropertyDescription,
                                                          TryJObjectParser2b<T>                Parser,
                                                          [NotNullWhen(true)]  out HashSet<T>  HashSet,
                                                                               out String?     ErrorResponse)

        {

            HashSet        = [];
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = $"The given property '{PropertyName}' is not a valid JSON array!";
                    return true;
                }


                var errorResponses = new List<String>();

                foreach (var element in JSONArray)
                {

                    if (element is not JObject JSONObject)
                    {
                        ErrorResponse = "The given token is not a valid JSON object!";
                        return true;
                    }

                    if (Parser(JSONObject,
                               out var itemT,
                               out var errorResponse) && itemT is not null)
                    {
                        HashSet.Add(itemT);
                    }
                    else
                        errorResponses.Add(errorResponse ?? "Could not parse the given item!");

                }

                if (errorResponses.Any())
                    ErrorResponse = errorResponses.AggregateWith(Environment.NewLine);

                return true;

            }

            return false;

        }

        #endregion



        #region GetOptional(this JSON, Key)

        public static String? GetOptional(this JObject  JSON,
                                          String        PropertyName)
        {

            if (JSON is null)
                return null;

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
                return JSONToken.Value<String>();

            return null;

        }

        #endregion

        #region GetOptional(this JSON, PropertyName, out Values)

        public static Boolean GetOptional(this JObject             JSON,
                                          String                   PropertyName,
                                          String                   PropertyDescription,
                                          out IEnumerable<String>  Values,
                                          out String?              ErrorResponse)
        {

            Values         = Array.Empty<String>();
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken))
            {

                // "propertyKey": null -> will be ignored!
                if (JSONToken is null || JSONToken.Type == JTokenType.Null)
                    return false;

                if (JSONToken is not JArray JSONArray)
                {
                    ErrorResponse = "The given property '" + (PropertyDescription ?? PropertyName) + "' is not a valid JSON array!";
                    return true;
                }

                try
                {

                    Values = JSONArray.AsEnumerable().
                                 Select(jtoken => jtoken?.Value<String>()).
                                 Where (value  => value is not null).
                                 Cast<String>().
                                 ToArray() ?? Array.Empty<String>();

                    return true;

                }
                catch
                { }

                return true;

            }

            return false;

        }

        #endregion





        // Legacy

        //#region ParseI18NString(this JToken)

        //public static I18NString ParseI18NString(this JToken JToken)
        //{

        //    var jobject = JToken as JObject;

        //    if (jobject is null)
        //        throw new ArgumentException("The given JSON token is not a JSON object!", nameof(JToken));

        //    return jobject.ParseI18NString();

        //}

        //#endregion

        //#region ParseI18NString(this JObject)

        //public static I18NString ParseI18NString(this JObject JObject)
        //{

        //    var i18NString = I18NString.Empty;

        //    foreach (var jproperty in JObject)
        //        i18NString.Add((Languages) Enum.Parse(typeof(Languages), jproperty.Key),
        //                       jproperty.Value.Value<String>());

        //    return i18NString;

        //}

        //#endregion

        //#region ParseI18NString(this JObject, PropertyKey)

        //public static I18NString ParseI18NString(this JObject JObject, String PropertyKey)
        //{

        //    if (PropertyKey.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(PropertyKey), "The given property key must not be null or empty!");

        //    var jobject = JObject[PropertyKey] as JObject;

        //    if (jobject is null)
        //        return I18NString.Empty;

        //    var i18NString = I18NString.Empty;

        //    foreach (var jproperty in jobject)
        //        i18NString.Add((Languages) Enum.Parse(typeof(Languages), jproperty.Key),
        //                       jproperty.Value.Value<String>());

        //    return i18NString;

        //}

        //#endregion

        //#region TryParseI18NString(this JToken,  out I18NString)

        //public static Boolean TryParseI18NString(this JToken JToken, out I18NString I18NString)
        //{

        //    var jobject = JToken as JObject;

        //    if (jobject is null)
        //    {
        //        I18NString = null;
        //        return false;
        //    }

        //    return jobject.TryParseI18NString(out I18NString);

        //}

        //#endregion

        //#region TryParseI18NString(this JObject, out I18NString)

        //public static Boolean TryParseI18NString(this JObject JObject, out I18NString i18NString)
        //{

        //    i18NString = I18NString.Empty;

        //    try
        //    {

        //        foreach (var jproperty in JObject)
        //            i18NString.Add((Languages)Enum.Parse(typeof(Languages), jproperty.Key),
        //                           jproperty.Value.Value<String>());

        //        return true;

        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}

        //#endregion

        //#region TryParseI18NString(this JObject, PropertyKey, out I18NString)

        //public static Boolean TryParseI18NString(this JObject JObject, String PropertyKey, out I18NString i18NString)
        //{

        //    if (PropertyKey.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(PropertyKey), "The given property key must not be null or empty!");

        //    i18NString = I18NString.Empty;

        //    if (!(JObject[PropertyKey] is JObject jobject))
        //        throw new ArgumentException("The value of the given JSON property '" + PropertyKey + "' is not a JSON object!", nameof(JObject));

        //    try
        //    {

        //        foreach (var jproperty in JObject)
        //            i18NString.Add((Languages)Enum.Parse(typeof(Languages), jproperty.Key),
        //                           jproperty.Value.Value<String>());

        //        return true;

        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}

        //#endregion


        #region ValueOrDefault(this ParentJObject, PropertyName, DefaultValue = null)

        /// <summary>
        /// Return the value of the JSON property or the given default value.
        /// </summary>
        /// <param name="ParentJObject">The JSON parent object.</param>
        /// <param name="PropertyName">The property name to match.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static JToken ValueOrDefault(this JObject  ParentJObject,
                                            String        PropertyName,
                                            String?       DefaultValue = null)
        {

            #region Initial checks

            if (ParentJObject is null)
                return DefaultValue;

            #endregion

            if (ParentJObject.TryGetValue(PropertyName, out var jsonValue))
                return jsonValue;

            return DefaultValue;

        }

        #endregion

        #region ValueOrFail   (this ParentJObject, PropertyName, ExceptionMessage = null)

        /// <summary>
        /// Return the value of the JSON property or the given default value.
        /// </summary>
        /// <param name="ParentJObject">The JSON parent object.</param>
        /// <param name="PropertyName">The property name to match.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static JToken ValueOrFail(this JObject  ParentJObject,
                                         String        PropertyName,
                                         String?       ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentJObject is null)
                throw new ArgumentNullException(nameof(ParentJObject),  "The given JSON object must not be null!");

            #endregion

            if (ParentJObject.TryGetValue(PropertyName, out var jsonValue))
                return jsonValue;

            throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given JSON property does not exist!");

        }

        #endregion


        #region MapValueOrDefault(ParentJObject, PropertyName, ValueMapper, DefaultValue = null)

        /// <summary>
        /// Return the mapped value of the JSON property or the given default value.
        /// </summary>
        /// <param name="ParentJObject">The JSON parent object.</param>
        /// <param name="PropertyName">The property name to match.</param>
        /// <param name="ValueMapper">A delegate to map the JSON property value.</param>
        /// <param name="DefaultValue">A default value.</param>
        public static T MapValueOrDefault<T>(this JObject     ParentJObject,
                                             String           PropertyName,
                                             Func<JToken, T>  ValueMapper,
                                             T                DefaultValue = default(T))
        {

            #region Initial checks

            if (ParentJObject is null)
                return DefaultValue;

            #endregion

            JToken JSONValue;

            if (ParentJObject.TryGetValue(PropertyName, out JSONValue))
            {

                try
                {
                    return ValueMapper(JSONValue);
                }
#pragma warning disable RCS1075  // Avoid empty catch clause that catches System.Exception.
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch
#pragma warning restore RECS0022
#pragma warning restore RCS1075
                { }

            }

            return DefaultValue;

        }

        #endregion

        #region MapValueOrFail   (ParentJObject, PropertyName, ValueMapper, ExceptionMessage = null)

        /// <summary>
        /// Return the mapped value of the JSON property or throw an exception
        /// having the given optional message.
        /// </summary>
        /// <param name="ParentJObject">The JSON parent object.</param>
        /// <param name="PropertyName">The property name to match.</param>
        /// <param name="ValueMapper">A delegate to map the JSON property value.</param>
        /// <param name="ExceptionMessage">An optional exception message.</param>
        public static T MapValueOrFail<T>(this JObject     ParentJObject,
                                          String           PropertyName,
                                          Func<JToken, T>  ValueMapper,
                                          String           ExceptionMessage = null)
        {

            #region Initial checks

            if (ParentJObject is null)
                throw new ArgumentNullException(nameof(ParentJObject),  "The given JSON object must not be null!");

            if (ValueMapper is null)
                throw new ArgumentNullException(nameof(ValueMapper),    "The given JSON value mapper delegate must not be null!");

            #endregion

            JToken JSONValue;

            if (ParentJObject.TryGetValue(PropertyName, out JSONValue))
            {

                try
                {
                    return ValueMapper(JSONValue);
                }
                catch (Exception e)
                {
                    throw ExceptionMessage.IsNotNullOrEmpty() ? new Exception(ExceptionMessage) : e;
                }

            }

            throw new Exception(ExceptionMessage.IsNotNullOrEmpty() ? ExceptionMessage : "The given JSON property does not exist!");

        }

        #endregion



    }

}
