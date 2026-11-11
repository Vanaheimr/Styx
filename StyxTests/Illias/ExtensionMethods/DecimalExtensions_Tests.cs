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
    /// Tests of the decimals a double, a single and a JSON number are written as.
    /// </summary>
    /// <remarks>
    /// Since .NET 11 a cast of a double to a decimal is exact: (Decimal) 11.9 is
    /// 11.900000000000000355271367880, and so is Newtonsoft's Value&lt;Decimal&gt;() of a JSON
    /// number it read as a double. On .NET 10 both gave 11.9.
    /// </remarks>
    [TestFixture]
    public class DecimalExtensions_Tests
    {

        #region ADoubleIsTheDecimalItIsWrittenAs(Double, Written)

        [TestCase(11.9,         "11.9")]
        [TestCase(3.096,        "3.096")]
        [TestCase(0.0012,       "0.0012")]
        [TestCase(50.2,         "50.2")]
        [TestCase(0.1,          "0.1")]
        [TestCase(230.0,        "230")]
        [TestCase(-49.8,        "-49.8")]
        [TestCase(1e-7,         "0.0000001")]
        [TestCase(12345678.9,   "12345678.9")]
        [TestCase(0.0,          "0")]
        public void ADoubleIsTheDecimalItIsWrittenAs(Double Value, String Written)
        {

            var asDecimal = Value.AsDecimal();

            Assert.Multiple(() => {
                Assert.That(asDecimal.ToString(CultureInfo.InvariantCulture), Is.EqualTo(Written));
                Assert.That(asDecimal,                                        Is.EqualTo(Decimal.Parse(Written, CultureInfo.InvariantCulture)));
                Assert.That((Double) asDecimal,                               Is.EqualTo(Value), "it reads back as the same double");
            });

        }

        #endregion

        #region ASingleIsTheDecimalItIsWrittenAs()

        [Test]
        public void ASingleIsTheDecimalItIsWrittenAs()
        {

            Assert.Multiple(() => {
                Assert.That(0.1f.    AsDecimal().ToString(CultureInfo.InvariantCulture), Is.EqualTo("0.1"));
                Assert.That(230.25f. AsDecimal().ToString(CultureInfo.InvariantCulture), Is.EqualTo("230.25"));
                Assert.That(11.9f.   AsDecimal().ToString(CultureInfo.InvariantCulture), Is.EqualTo("11.9"));
            });

        }

        #endregion

        #region NoDecimalIsMadeOfWhatNoDecimalHolds()

        [Test]
        public void NoDecimalIsMadeOfWhatNoDecimalHolds()
        {

            Assert.Multiple(() => {
                Assert.That(() => Double.NaN.             AsDecimal(), Throws.TypeOf<OverflowException>());
                Assert.That(() => Double.PositiveInfinity.AsDecimal(), Throws.TypeOf<OverflowException>());
                Assert.That(() => 1e30.                   AsDecimal(), Throws.TypeOf<OverflowException>());
                Assert.That(() => Single.NegativeInfinity.AsDecimal(), Throws.TypeOf<OverflowException>());
            });

        }

        #endregion

        #region AJSONNumberIsTheDecimalItIsWrittenAs()

        /// <summary>
        /// A JSON number Newtonsoft read as a double is the decimal of its text, and every
        /// other token is what Value&lt;Decimal&gt;() makes of it - a decimal keeps its scale.
        /// </summary>
        [Test]
        public void AJSONNumberIsTheDecimalItIsWrittenAs()
        {

            var json = JObject.Parse("""{ "price": 11.9, "count": 42, "text": "3.096", "tiny": 1E-07 }""");

            Assert.Multiple(() => {

                Assert.That(json["price"]!.Type,                                                Is.EqualTo(JTokenType.Float), "read as a double");
                Assert.That(json["price"]!.AsDecimal().ToString(CultureInfo.InvariantCulture),  Is.EqualTo("11.9"));
                Assert.That(json["count"]!.AsDecimal(),                                         Is.EqualTo(42m));
                Assert.That(json["text"]! .AsDecimal(),                                         Is.EqualTo(3.096m));
                Assert.That(json["tiny"]! .AsDecimal(),                                         Is.EqualTo(0.0000001m));

                Assert.That(new JValue(1.50m).AsDecimal().Scale,                                Is.EqualTo(2), "a decimal stays as it is");
                Assert.That(new JValue(0.1f). AsDecimal(),                                      Is.EqualTo(0.1m));

            });

        }

        #endregion

        #region AMissingJSONNumberIsNull()

        [Test]
        public void AMissingJSONNumberIsNull()
        {

            var json = JObject.Parse("""{ "maxPower_kW": 11.9, "none": null }""");

            Assert.Multiple(() => {
                Assert.That(json["maxPower_kW"].AsDecimalOrNull(), Is.EqualTo(11.9m));
                Assert.That(json["none"].       AsDecimalOrNull(), Is.Null);
                Assert.That(json["missing"].    AsDecimalOrNull(), Is.Null);
            });

        }

        #endregion

    }

}
