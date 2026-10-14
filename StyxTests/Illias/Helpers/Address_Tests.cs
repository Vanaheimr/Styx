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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Address tests.
    /// </summary>
    /// <remarks>
    /// The time zone was written as "timeZone", but read as "TimeZone": an
    /// address lost its time zone whenever it was read again.
    /// </remarks>
    [TestFixture]
    public class Address_Tests
    {

        #region AnAddressKeepsItsTimeZoneWhenReadAgain()

        [Test]
        public void AnAddressKeepsItsTimeZoneWhenReadAgain()
        {

            var address = new Address(
                              "Musterstraße",
                              "07749",
                              I18NString.Create(Languages.de, "Jena"),
                              Country.Germany,
                              HouseNumber:  "1",
                              TimeZone:     Time_Zone.Parse("Europe/Berlin")
                          );

            var json    = address.ToJSON();

            Assert.That(json["timeZone"]?.Value<String>(), Is.EqualTo("Europe/Berlin"));

            Assert.That(Address.TryParse(json, out var parsed, out var errorResponse), Is.True, errorResponse);
            Assert.That(parsed!.TimeZone,                  Is.EqualTo(address.TimeZone));
            Assert.That(parsed.ToJSON().ToString(),        Is.EqualTo(json.ToString()));

        }

        #endregion

    }

}
