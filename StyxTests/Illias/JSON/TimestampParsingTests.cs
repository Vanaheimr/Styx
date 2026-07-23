/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Unit tests for parsing timestamps via ParseMandatory/ParseOptional
    /// (out DateTimeOffset): original UTC offsets must be preserved and
    /// timestamp arrays must parse element-wise.
    /// </summary>
    [TestFixture]
    public class TimestampParsingTests
    {

        #region ParseMandatory_TimestampWithOffset_PreservesOffsetAndInstant()

        // A string JSON token carrying an explicit UTC offset must keep that
        // offset (and, of course, the instant). Manually built JObjects keep
        // string tokens as strings - the parse path under test sees the
        // original "+02:00".
        [Test]
        public void ParseMandatory_TimestampWithOffset_PreservesOffsetAndInstant()
        {

            var json     = new JObject(new JProperty("timestamp", "2024-01-02T03:04:05+02:00"));

            var success  = json.ParseMandatory("timestamp",
                                               "a timestamp",
                                               out DateTimeOffset timestamp,
                                               out String?        errorResponse);

            Assert.That(success,                Is.True);
            Assert.That(errorResponse,          Is.Null);
            Assert.That(timestamp.Offset,       Is.EqualTo(TimeSpan.FromHours(2)));
            Assert.That(timestamp.UtcDateTime,  Is.EqualTo(new DateTime(2024, 1, 2, 1, 4, 5, DateTimeKind.Utc)));

        }

        #endregion

        #region ParseMandatory_TimestampZulu_IsUTCInstant()

        // The common path: JObject.Parse converts ISO "Z" strings into UTC
        // date tokens - the parsed DateTimeOffset must be the same instant.
        [Test]
        public void ParseMandatory_TimestampZulu_IsUTCInstant()
        {

            var json     = JObject.Parse(@"{ ""timestamp"": ""2024-06-07T08:09:10.000Z"" }");

            var success  = json.ParseMandatory("timestamp",
                                               "a timestamp",
                                               out DateTimeOffset timestamp,
                                               out String?        errorResponse);

            Assert.That(success,                Is.True);
            Assert.That(errorResponse,          Is.Null);
            Assert.That(timestamp.UtcDateTime,  Is.EqualTo(new DateTime(2024, 6, 7, 8, 9, 10, DateTimeKind.Utc)));

        }

        #endregion

        #region ParseOptional_TimestampWithOffset_PreservesOffsetAndInstant()

        [Test]
        public void ParseOptional_TimestampWithOffset_PreservesOffsetAndInstant()
        {

            var json     = new JObject(new JProperty("timestamp", "2024-01-02T03:04:05-05:30"));

            var success  = json.ParseOptional("timestamp",
                                              "a timestamp",
                                              out DateTimeOffset? timestamp,
                                              out String?         errorResponse);

            Assert.That(success,                       Is.True);
            Assert.That(errorResponse,                 Is.Null);
            Assert.That(timestamp.HasValue,            Is.True);
            Assert.That(timestamp!.Value.Offset,       Is.EqualTo(TimeSpan.FromHours(-5.5)));
            Assert.That(timestamp!.Value.UtcDateTime,  Is.EqualTo(new DateTime(2024, 1, 2, 8, 34, 5, DateTimeKind.Utc)));

        }

        #endregion

        #region ParseOptional_TimestampArray_ParsesEveryElement()

        // Regression test: the timestamp-array overload used to convert the
        // enclosing JArray token itself instead of the loop element - multi-
        // element arrays therefore never parsed. Each element must be parsed
        // on its own, preserving its own offset.
        [Test]
        public void ParseOptional_TimestampArray_ParsesEveryElement()
        {

            var json     = new JObject(
                               new JProperty("timestamps", new JArray(
                                   "2024-01-02T03:04:05+02:00",
                                   "2024-06-07T08:09:10Z"
                               ))
                           );

            var success  = json.ParseOptional("timestamps",
                                              "an enumeration of timestamps",
                                              out IEnumerable<DateTimeOffset> timestamps,
                                              out String?                     errorResponse);

            Assert.That(success,        Is.True);
            Assert.That(errorResponse,  Is.Null);

            var list = timestamps.ToList();
            Assert.That(list,                  Has.Count.EqualTo(2));
            Assert.That(list[0].Offset,        Is.EqualTo(TimeSpan.FromHours(2)));
            Assert.That(list[0].UtcDateTime,   Is.EqualTo(new DateTime(2024, 1, 2, 1, 4,  5, DateTimeKind.Utc)));
            Assert.That(list[1].Offset,        Is.EqualTo(TimeSpan.Zero));
            Assert.That(list[1].UtcDateTime,   Is.EqualTo(new DateTime(2024, 6, 7, 8, 9, 10, DateTimeKind.Utc)));

        }

        #endregion

        #region ParseMandatory_MissingTimestamp_FailsWithErrorResponse()

        [Test]
        public void ParseMandatory_MissingTimestamp_FailsWithErrorResponse()
        {

            var json     = new JObject();

            var success  = json.ParseMandatory("timestamp",
                                               "a timestamp",
                                               out DateTimeOffset timestamp,
                                               out String?        errorResponse);

            Assert.That(success,        Is.False);
            Assert.That(errorResponse,  Is.Not.Null);

        }

        #endregion

    }

}
