/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An abstract result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public abstract class AResult<T>
    {

        #region Properties

        public     CommandResult         Result             { get; }

        /// <summary>
        /// The optionalunqiue identification of the sender.
        /// </summary>
        public     IId?                  SenderId           { get; }

        /// <summary>
        /// The optional sender of this result.
        /// </summary>
        public     Object?               Sender             { get; }

        /// <summary>
        /// The object of the operation.
        /// </summary>
        protected  T?                    Object             { get; }

        /// <summary>
        /// The unique event tracking identification for correlating this request with other events.
        /// </summary>
        public     EventTracking_Id      EventTrackingId    { get; }

        //public    String?               Argument           { get; }

        public     I18NString            Description        { get; }

        /// <summary>
        /// Optional warnings or additional information.
        /// </summary>
        public     IEnumerable<Warning>  Warnings           { get; }

        /// <summary>
        /// The runtime of the command till this result.
        /// </summary>
        public     TimeSpan              Runtime            { get;  }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract result.
        /// </summary>
        /// <param name="Entity">An entity.</param>
        /// <param name="Result">A (command) result.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="SenderId">An optional unqiue identification of the sender.</param>
        /// <param name="Sender">An optional sender of this result.</param>
        /// <param name="Description">An optional description of the result.</param>
        /// <param name="Warnings">Optional warnings or additional information.</param>
        /// <param name="Runtime">An optional runtime of the command till this result.</param>
        public AResult(T                      Entity,
                       CommandResult          Result,
                       EventTracking_Id?      EventTrackingId   = null,
                       IId?                   SenderId          = null,
                       Object?                Sender            = null,
                       I18NString?            Description       = null,
                       IEnumerable<Warning>?  Warnings          = null,
                       TimeSpan?              Runtime           = null)
        {

            this.Object            = Entity;
            this.Result            = Result;
            this.EventTrackingId   = EventTrackingId ?? EventTracking_Id.New;
            this.SenderId          = SenderId;
            this.Sender            = Sender;
            this.Description       = Description     ?? I18NString.Empty;
            this.Warnings          = Warnings        ?? Array.Empty<Warning>();
            this.Runtime           = Runtime         ?? TimeSpan.Zero;

        }

        #endregion



        public JObject ToJSON()

            => JSONObject.Create(
                   Description is not null
                       ? Description.Count == 1
                             ? new JProperty("description",  Description.FirstText())
                             : new JProperty("description",  Description.ToJSON())
                       : null
               );


        public override String ToString()

            => Result.ToString();

            //=> IsSuccess
            //        ? "Success"
            //        : "Failed" + (Description is not null && Description.IsNullOrEmpty()
            //                          ? ": " + Description.FirstText()
            //                          : "!");

    }


    /// <summary>
    /// An abstract result for two objects, e.g. linked together.
    /// </summary>
    /// <typeparam name="T1">The type of the result.</typeparam>
    /// <typeparam name="T2">The type of the result.</typeparam>
    public abstract class AResult<T1, T2>

    {

        #region Properties

        /// <summary>
        /// The object of the operation.
        /// </summary>
        protected T1                Object1             { get; }

        /// <summary>
        /// The object of the operation.
        /// </summary>
        protected T2                Object2             { get; }

        /// <summary>
        /// The unique event tracking identification for correlating this request with other events.
        /// </summary>
        public    EventTracking_Id  EventTrackingId     { get; }

        /// <summary>
        /// Whether the operation was successful, or not.
        /// </summary>
        public    Boolean           IsSuccess           { get; }

        public    String?           Argument            { get; }

        public    I18NString?       ErrorDescription    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract result.
        /// </summary>
        /// <param name="Object1">The object of the operation.</param>
        /// <param name="Object2">The object of the operation.</param>
        /// <param name="EventTrackingId">The unique event tracking identification for correlating this request with other events.</param>
        /// <param name="IsSuccess">Whether the operation was successful, or not.</param>
        /// <param name="Argument"></param>
        /// <param name="ErrorDescription"></param>
        public AResult(T1                Object1,
                       T2                Object2,
                       EventTracking_Id  EventTrackingId,
                       Boolean           IsSuccess,
                       String?           Argument           = null,
                       I18NString?       ErrorDescription   = null)
        {

            this.Object1           = Object1;
            this.Object2           = Object2;
            this.EventTrackingId   = EventTrackingId;
            this.IsSuccess         = IsSuccess;
            this.Argument          = Argument;
            this.ErrorDescription  = ErrorDescription;

        }

        #endregion



        public JObject ToJSON()

            => JSONObject.Create(
                   ErrorDescription is not null
                       ? ErrorDescription.Count == 1
                             ? new JProperty("description",  ErrorDescription.FirstText())
                             : new JProperty("description",  ErrorDescription.ToJSON())
                       : null
               );


        public override String ToString()

            => IsSuccess
                    ? "Success"
                    : "Failed" + (ErrorDescription is not null && ErrorDescription.IsNullOrEmpty()
                                      ? ": " + ErrorDescription.FirstText()
                                      : "!");

    }
}
