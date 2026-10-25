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
    /// address lost its time zone whenever it was read again. Its custom data
    /// was written as "customData", but read as "CustomData", and lost as well.
    /// </remarks>
    [TestFixture]
    public class Address_Tests
    {

        #region (private static) NewAddress(CustomData = null)

        private static Address NewAddress(CustomDataNew? CustomData = null)

            => new (
                   "Musterstraße",
                   "07749",
                   I18NString.Create(Languages.de, "Jena"),
                   Country.Germany,
                   HouseNumber:  "1",
                   CustomData:   CustomData
               );

        #endregion

        #region (private static) CustomData

        /// <summary>
        /// Custom data with numbers with a fraction and texts that look like points in time, which a JSON text read
        /// with the defaults of Newtonsoft turns into doubles and dates.
        /// </summary>
        private static CustomDataNew CustomData

            => CustomDataNew.ParseJSON("""{"vendor":{"rate":1.50,"since":"2024-01-01T12:00:00+02:00","big":123456789012345678901234567890},"tags":["fast",7,"2024-01-01T12:00:00Z"]}""");

        #endregion


        #region AnAddressKeepsItsCustomDataWhenReadAgain()

        [Test]
        public void AnAddressKeepsItsCustomDataWhenReadAgain()
        {

            var address = NewAddress(CustomData);
            var json    = address.ToJSON();

            Assert.That(json["customData"],                Is.Not.Null);

            Assert.That(Address.TryParse(json, out var parsed, out var errorResponse), Is.True, errorResponse);
            Assert.That(parsed!.CustomData,                Is.Not.Null);
            Assert.That(CanonicalJSON.Serialize(parsed.CustomData!.ToJObject()),
                        Is.EqualTo(CanonicalJSON.Serialize(CustomData.ToJObject())));
            Assert.That(parsed.ToJSON().ToString(),        Is.EqualTo(json.ToString()));

        }

        #endregion

        #region AnAddressKeepsItsCustomDataWhenReadAgainAsText()

        [Test]
        public void AnAddressKeepsItsCustomDataWhenReadAgainAsText()
        {

            var address = NewAddress(CustomData);
            var text    = CanonicalJSON.Serialize(address.ToJSON());

            Assert.That(text,                              Does.Contain("""{"big":123456789012345678901234567890,"rate":1.50,"since":"2024-01-01T12:00:00+02:00"}"""));

            Assert.That(Address.TryParse(text, out var parsed, out var errorResponse, null), Is.True, errorResponse);
            Assert.That(CanonicalJSON.Serialize(parsed!.ToJSON()),
                        Is.EqualTo(text));

        }

        #endregion

        #region AnAddressReadsItsCustomDataInLowerCase()

        [Test]
        public void AnAddressReadsItsCustomDataInLowerCase()
        {

            var json = NewAddress().ToJSON();

            json.Add("customData", new JObject(new JProperty("note", "given")));

            Assert.That(Address.TryParse(json, out var parsed, out var errorResponse), Is.True, errorResponse);
            Assert.That(parsed!.CustomData?["note"]?.StringValue,  Is.EqualTo("given"));

        }

        #endregion


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
