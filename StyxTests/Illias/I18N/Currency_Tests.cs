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

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Currency tests.
    /// </summary>
    [TestFixture]
    public class Currency_Tests
    {

        #region All_currencies_are_known_by_their_ISO_codes()

        [Test]
        public void All_currencies_are_known_by_their_ISO_codes()
        {

            var currencies = typeof(Currency).GetFields().
                                              Select(fieldInfo => fieldInfo.GetValue(null)).
                                              OfType<Currency>().
                                              ToArray();

            using (Assert.EnterMultipleScope())
            {

                Assert.That(currencies.Length,                                             Is.GreaterThan(150));
                Assert.That(currencies.Select(currency => currency.ISOCode),               Is.Unique);
                Assert.That(currencies.Select(currency => currency.Numeric),               Is.Unique);
                Assert.That(currencies.Select(currency => currency.ISOCode),               Has.All.Match("^[A-Z]{3}$"));

                foreach (var code in new[] { "EUR", "SEK", "NOK", "DKK", "JPY", "CAD", "XOF" })
                {
                    Assert.That(Currency.TryParseISO(code, out var currency),              Is.True, code);
                    Assert.That(currency!.ISOCode,                                         Is.EqualTo(code));
                }

                Assert.That(Currency.TryParseISO("XXX", out _),                            Is.False);
                Assert.That(Currency.NOK.Numeric,                                          Is.EqualTo(578));
                Assert.That(Currency.NOK.Name,                                             Is.EqualTo("Norwegian Krone"));

            }

        }

        #endregion

    }

}
