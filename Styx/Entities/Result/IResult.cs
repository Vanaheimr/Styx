/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for command results interface.
    /// </summary>
    public static class ResultExtensions
    {

        #region ToJSON(this Results,                                CustomResultSerializer = null)

        /// <summary>
        /// Return a JSON representation of the given enumeration of command results.
        /// </summary>
        /// <param name="Results">An enumeration of command results</param>
        /// <param name="CustomResultSerializer">A delegate to serialize custom command result JSON objects.</param>
        public static JArray ToJSON(this IEnumerable<IResult>                  Results,
                                    CustomJObjectSerializerDelegate<IResult>?  CustomResultSerializer   = null)

            => Results.Any()
                   ? new JArray(Results.Select(result => result.ToJSON(CustomResultSerializer: CustomResultSerializer)))
                   : new JArray();

        #endregion

        #region ToJSON(this Results, AdditionalJSONPropertyCreator, CustomResultSerializer = null)

        /// <summary>
        /// Return a JSON representation of the given enumeration of command results.
        /// </summary>
        /// <param name="Results">An enumeration of command results</param>
        /// <param name="AdditionalJSONPropertyCreator">A delegate to create additional JSON properties.</param>
        /// <param name="CustomResultSerializer">A delegate to serialize custom command result JSON objects.</param>
        public static JArray ToJSON<T>(this IEnumerable<IResult<T>>               Results,
                                       Func<IResult<T>, IEnumerable<JProperty>>   AdditionalJSONPropertyCreator,
                                       CustomJObjectSerializerDelegate<IResult>?  CustomResultSerializer   = null)

            => Results.Any()
                   ? new JArray(Results.Select(result => result.ToJSON(AdditionalJSONPropertyCreator,
                                                                       CustomResultSerializer)))
                   : new JArray();

        #endregion

    }


    /// <summary>
    /// The common interface of all command results.
    /// </summary>
    public interface IResult
    {

        /// <summary>
        /// The result of the command.
        /// </summary>
        CommandResult         Result             { get; }

        /// <summary>
        /// The optional unqiue identification of the sender.
        /// </summary>
        IId?                  SenderId           { get; }

        /// <summary>
        /// The optional sender of this result.
        /// </summary>
        Object?               Sender             { get; }

        /// <summary>
        /// The unique event tracking identification for correlating this request with other events.
        /// </summary>
        EventTracking_Id      EventTrackingId    { get; }

        /// <summary>
        /// An optional description of the result.
        /// </summary>
        I18NString            Description        { get; }

        /// <summary>
        /// Optional warnings.
        /// </summary>
        IEnumerable<Warning>  Warnings           { get; }

        /// <summary>
        /// The runtime of the command till this result.
        /// </summary>
        TimeSpan              Runtime            { get; }


        /// <summary>
        /// Return a JSON representation of this command result.
        /// </summary>
        /// <param name="AdditionalJSONProperties">Optional additional JSON properties.</param>
        /// <param name="CustomResultSerializer">A delegate to serialize custom command result JSON objects.</param>
        JObject               ToJSON(IEnumerable<JProperty>?                    AdditionalJSONProperties   = null,
                                     CustomJObjectSerializerDelegate<IResult>?  CustomResultSerializer     = null);


    }


    /// <summary>
    /// The generic common interface of all command results.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public interface IResult<T> : IResult
    {

        /// <summary>
        /// The object of the operation.
        /// </summary>
        T?                    Entity             { get; }


        /// <summary>
        /// Return a JSON representation of this command result.
        /// </summary>
        /// <param name="AdditionalJSONPropertyCreator">An optional additional JSON property creator.</param>
        /// <param name="CustomResultSerializer">A delegate to serialize custom command result JSON objects.</param>
        JObject               ToJSON(Func<IResult<T>, IEnumerable<JProperty>>   AdditionalJSONPropertyCreator,
                                     CustomJObjectSerializerDelegate<IResult>?  CustomResultSerializer   = null);

    }

}
