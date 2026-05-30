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

using System.Text.Json;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    [TestFixture]
    public class CanonicalJSONTests
    {

        #region Newtonsoft_JSON_objects_are_sorted_recursively()

        [Test]
        public void Newtonsoft_JSON_objects_are_sorted_recursively()
        {

            var json = new JObject(
                           new JProperty("z", JValue.CreateNull()),
                           new JProperty("b", 1),
                           new JProperty("a", new JObject(
                               new JProperty("d", true),
                               new JProperty("c", new JArray(3, 2, 1))
                           ))
                       );

            Assert.That(
                CanonicalJSON.Serialize(json),
                Is.EqualTo("{\"a\":{\"c\":[3,2,1],\"d\":true},\"b\":1,\"z\":null}")
            );

        }

        #endregion

        #region System_Text_JSON_matches_Newtonsoft_JSON()

        [Test]
        public void System_Text_JSON_matches_Newtonsoft_JSON()
        {

            var newtonsoftJSON = new JObject(
                                     new JProperty("b", 1),
                                     new JProperty("a", new JArray(
                                         new JObject(
                                             new JProperty("y", false),
                                             new JProperty("x", "hello")
                                         )
                                     ))
                                 );

            using var systemTextJSON = JsonDocument.Parse(
                                           "{\"b\":1,\"a\":[{\"y\":false,\"x\":\"hello\"}]}"
                                       );

            Assert.That(
                CanonicalJSON.Serialize(systemTextJSON),
                Is.EqualTo(CanonicalJSON.Serialize(newtonsoftJSON))
            );

        }

        #endregion

        #region Newtonsoft_dates_are_written_as_JSON_strings()

        [Test]
        public void Newtonsoft_dates_are_written_as_JSON_strings()
        {

            var timestamp = DateTimeOffset.Parse("2026-05-30T13:15:35.7422032+00:00");
            var json      = new JObject(
                                new JProperty("timestamp", timestamp)
                            );

            Assert.That(
                CanonicalJSON.Serialize(json),
                Is.EqualTo("{\"timestamp\":\"2026-05-30T13:15:35.7422032+00:00\"}")
            );

        }

        #endregion

    }

}
