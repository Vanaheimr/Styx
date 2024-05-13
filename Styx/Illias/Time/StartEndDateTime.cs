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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A structure to store a start and optional end time.
    /// </summary>
    public class StartEndDateTime
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext  = "https://graphdefined.com/contexts/vanaheimr+json/startEndDateTime";

        #endregion

        #region Properties

        /// <summary>
        /// The start time.
        /// </summary>
        public DateTime   StartTime    { get; }

        /// <summary>
        /// The end time.
        /// </summary>
        public DateTime?  EndTime      { get; set; }

        /// <summary>
        /// The duration.
        /// </summary>
        public TimeSpan? Duration

            => EndTime.HasValue
                   ? EndTime.Value - StartTime
                   : new TimeSpan?();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new start and end time structure.
        /// </summary>
        /// <param name="StartTime">The start time.</param>
        /// <param name="EndTime">The end time.</param>
        public StartEndDateTime(DateTime   StartTime,
                                DateTime?  EndTime = null)
        {

            if (EndTime.HasValue && StartTime > EndTime)
                throw new ArgumentException("The start time must not be after the end time!");

            this.StartTime  = StartTime;
            this.EndTime    = EndTime;

        }

        #endregion


        #region (static) Now

        /// <summary>
        /// Return a StartEndDateTime object which start time
        /// is set to the current date and time.
        /// </summary>
        public static StartEndDateTime Now

            => new (Timestamp.Now);

        #endregion


        #region ToJSON(Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation of the given start and optional end time.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        /// <param name="CustomStartEndDateTimeSerializer">A custom start and optional end time serializer.</param>
        public JObject ToJSON(Boolean                                             Embedded                           = false,
                              CustomJObjectSerializerDelegate<StartEndDateTime>?  CustomStartEndDateTimeSerializer   = null)
        {

            var json = JSONObject.Create(

                           Embedded
                               ? null
                               : new JProperty("@context",   JSONLDContext),

                                 new JProperty("start",      StartTime.    ToIso8601()),

                           EndTime.HasValue
                               ? new JProperty("end",        EndTime.Value.ToIso8601())
                               : null
                       );

            return CustomStartEndDateTimeSerializer is not null
                       ? CustomStartEndDateTimeSerializer(this, json)
                       : json;

        }

        #endregion

        #region TryParse(JSON, out StartEndDateTime, out ErrorResponse)

        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out StartEndDateTime?  StartEndDateTime,
                                       [NotNullWhen(false)] out String?            ErrorResponse)
        {

            StartEndDateTime = null;

            try
            {

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Context    [mandatory]

                var context = JSON["@context"]?.Value<String>();

                #endregion

                #region Parse Start      [mandatory]

                if (!JSON.ParseMandatory("start",
                                         "start timestamp",
                                         out DateTime Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse End        [optional]

                if (JSON.ParseOptional("end",
                                       "end timestamp",
                                       out DateTime? End,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                StartEndDateTime = new StartEndDateTime(
                                       Start,
                                       End
                                   );

                return true;

            }
            catch (Exception e)
            {
                StartEndDateTime  = null;
                ErrorResponse     = "The given JSON representation of a start and optional end time is invalid: " + e.Message;
            }

            return false;

        }

        #endregion


        #region (override) GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return StartTime.GetHashCode() * 3 ^
                      (EndTime?. GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   StartTime.ToIso8601(),

                   EndTime.HasValue
                       ? " -> " + EndTime.Value.ToIso8601()
                       : "..."

               );

        #endregion

    }

}
