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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An abstract result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the entity identification.</typeparam>
    public abstract class AEnititiesResult<TResult, TEntity, TId>

        where TResult : AEnitityResult<TEntity, TId>
        where TEntity : class, IHasId<TId>
        where TId     : IId

    {

        #region Properties

        public CommandResult   Result             { get; }
        public IEnumerable<TResult>  SuccessfulItems    { get; }
        public IEnumerable<TResult>  RejectedItems      { get; }

        public IId?                  AuthId             { get; }
        public Object?               Sender        { get; }
        public EventTracking_Id      EventTrackingId    { get; }
        public I18NString            Description        { get; }
        public IEnumerable<Warning>  Warnings           { get; }
        public TimeSpan              Runtime            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract result.
        /// </summary>
        /// <param name="Entity">The entity of the operation.</param>
        /// <param name="EventTrackingId">The unique event tracking identification for correlating this request with other events.</param>
        /// <param name="IsSuccess">Whether the operation was successful, or not.</param>
        /// <param name="Argument"></param>
        /// <param name="ErrorDescription"></param>
        public AEnititiesResult(CommandResult    Result,
                                IEnumerable<TResult>?  SuccessfulEVSEs   = null,
                                IEnumerable<TResult>?  RejectedEVSEs     = null,
                                IId?                   AuthId            = null,
                                Object?                Sender       = null,
                                EventTracking_Id?      EventTrackingId   = null,
                                I18NString?            Description       = null,
                                IEnumerable<Warning>?  Warnings          = null,
                                TimeSpan?              Runtime           = null)
        {

            this.Result           = Result;
            this.SuccessfulItems  = SuccessfulEVSEs ?? Array.Empty<TResult>();
            this.RejectedItems    = RejectedEVSEs   ?? Array.Empty<TResult>();
            this.AuthId           = AuthId;
            this.Sender      = Sender;
            this.EventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            this.Description      = Description     ?? I18NString.Empty;
            this.Warnings         = Warnings        ?? Array.Empty<Warning>();
            this.Runtime          = Runtime         ?? TimeSpan.Zero;

        }

        #endregion


    }

}
