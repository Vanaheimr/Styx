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

using System.Globalization;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// A JSON number read through a parser of text is the number it is, whatever
    /// the culture of the machine.
    /// </summary>
    /// <remarks>
    /// The parsers got the number as JValue.ToString() writes it, which is in the
    /// current culture: on a German machine 2.5 became "2,5", and the invariant
    /// parser of a kilogram or a meter read that as 25 - ten times the weight of a
    /// charging cable, and nothing said so.
    /// </remarks>
    [TestFixture]
    [NonParallelizable]
    public class NumberTextCultureTests
    {

        #region Data

        private CultureInfo culture   = default!;
        private CultureInfo uiCulture = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void SetUp()
        {
            culture                        = CultureInfo.CurrentCulture;
            uiCulture                      = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentCulture     = CultureInfo.GetCultureInfo("de-DE");
            CultureInfo.CurrentUICulture   = CultureInfo.GetCultureInfo("de-DE");
        }

        [TearDown]
        public void TearDown()
        {
            CultureInfo.CurrentCulture     = culture;
            CultureInfo.CurrentUICulture   = uiCulture;
        }

        #endregion


        #region ADecimalNumberIsReadAsItIsInAGermanCulture()

        [Test]
        public void ADecimalNumberIsReadAsItIsInAGermanCulture()
        {

            var json = JObject.Parse("""{ "cable_weight": 2.5, "cable_length": 312.5 }""");

            json.ParseOptional("cable_weight",
                               "cable weight",
                               Kilogram.TryParseKG,
                               out Kilogram? weight,
                               out var weightError);

            json.ParseOptional("cable_length",
                               "cable length",
                               Meter.TryParse_cm,
                               out Meter? length,
                               out var lengthError);

            Assert.Multiple(() => {
                Assert.That(weightError,     Is.Null);
                Assert.That(lengthError,     Is.Null);
                Assert.That(weight?.Value,   Is.EqualTo(2.5M),   "The weight is read in the current culture.");
                Assert.That(length?.cm,      Is.EqualTo(312.5M), "The length is read in the current culture.");
            });

        }

        #endregion

        #region AnIntegerIsReadAsItIs()

        [Test]
        public void AnIntegerIsReadAsItIs()
        {

            JObject.Parse("""{ "cable_length": 500 }""").
                ParseOptional("cable_length",
                              "cable length",
                              Meter.TryParse_cm,
                              out Meter? length,
                              out var errorResponse);

            Assert.Multiple(() => {
                Assert.That(errorResponse, Is.Null);
                Assert.That(length?.cm,    Is.EqualTo(500M));
            });

        }

        #endregion

    }

}
