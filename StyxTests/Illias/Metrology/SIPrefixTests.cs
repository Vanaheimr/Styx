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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Tests for SI prefixes.
    /// </summary>
    [TestFixture]
    public class SIPrefixTests
    {

        #region All_25_prefixes_roundtrip_via_symbol_name_and_exponent()

        [Test]
        public void All_25_prefixes_roundtrip_via_symbol_name_and_exponent()
        {

            Assert.That(SIPrefix.All.Count,  Is.EqualTo(25));

            foreach (var siPrefix in SIPrefix.All)
            {

                Assert.That(SIPrefix.TryParse(siPrefix.Symbol, out var fromSymbol),      Is.True,   $"Symbol '{siPrefix.Symbol}'");
                Assert.That(fromSymbol,                                                  Is.EqualTo(siPrefix));

                if (siPrefix.Name.Length > 0)
                {
                    Assert.That(SIPrefix.TryParse(siPrefix.Name.ToLower(), out var fromName),  Is.True,  $"Name '{siPrefix.Name}'");
                    Assert.That(fromName,                                                Is.EqualTo(siPrefix));
                }

                Assert.That(SIPrefix.TryFrom(siPrefix.Exponent, out var fromExponent),   Is.True);
                Assert.That(fromExponent,                                                Is.EqualTo(siPrefix));

            }

        }

        #endregion

        #region Micro_accepts_both_mu_codepoints_and_the_ASCII_fallback()

        [Test]
        public void Micro_accepts_both_mu_codepoints_and_the_ASCII_fallback()
        {

            // The micro sign 'µ' (U+00B5)...
            Assert.That(SIPrefix.Parse("µ"),  Is.EqualTo(SIPrefix.Micro));

            // The Greek small letter mu 'μ' (U+03BC)...
            Assert.That(SIPrefix.Parse("μ"),  Is.EqualTo(SIPrefix.Micro));

            // ...and the ASCII fallback "u" (as in UCUM)!
            Assert.That(SIPrefix.Parse("u"),       Is.EqualTo(SIPrefix.Micro));

            // The canonical symbol is the micro sign U+00B5...
            Assert.That(SIPrefix.Micro.Symbol,     Is.EqualTo("µ"));

        }

        #endregion

        #region Symbol_parsing_is_case_sensitive()

        [Test]
        public void Symbol_parsing_is_case_sensitive()
        {

            Assert.That(SIPrefix.Parse("m"),                      Is.EqualTo(SIPrefix.Milli));
            Assert.That(SIPrefix.Parse("M"),                      Is.EqualTo(SIPrefix.Mega));
            Assert.That(SIPrefix.Parse("d"),                      Is.EqualTo(SIPrefix.Deci));
            Assert.That(SIPrefix.Parse("da"),                     Is.EqualTo(SIPrefix.Deca));
            Assert.That(SIPrefix.Parse("k"),                      Is.EqualTo(SIPrefix.Kilo));
            Assert.That(SIPrefix.Parse("T"),                      Is.EqualTo(SIPrefix.Tera));
            Assert.That(SIPrefix.Parse("q"),                      Is.EqualTo(SIPrefix.Quecto));
            Assert.That(SIPrefix.Parse("Q"),                      Is.EqualTo(SIPrefix.Quetta));

            // "K" is neither a symbol nor a name...
            Assert.That(SIPrefix.TryParse("K", out _),            Is.False);

            // ...while names are case-insensitive!
            Assert.That(SIPrefix.Parse("kilo"),                   Is.EqualTo(SIPrefix.Kilo));
            Assert.That(SIPrefix.Parse("MILLI"),                  Is.EqualTo(SIPrefix.Milli));

        }

        #endregion

        #region Factor_scales_within_the_decimal_range_and_throws_beyond()

        [Test]
        public void Factor_scales_within_the_decimal_range_and_throws_beyond()
        {

            Assert.That(SIPrefix.Kilo. Factor,        Is.EqualTo(1000m));
            Assert.That(SIPrefix.Milli.Factor,        Is.EqualTo(0.001m));
            Assert.That(SIPrefix.None. Factor,        Is.EqualTo(1m));
            Assert.That(SIPrefix.Yotta.Factor,        Is.EqualTo(1_000_000_000_000_000_000_000_000m));

            // Quecto and quetta (10^±30) exceed the range of System.Decimal!
            Assert.That(() => SIPrefix.Quetta.Factor,  Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => SIPrefix.Quecto.Factor,  Throws.TypeOf<ArgumentOutOfRangeException>());

        }

        #endregion

        #region TryFrom_rejects_noncanonical_exponents()

        [Test]
        public void TryFrom_rejects_noncanonical_exponents()
        {

            Assert.That(SIPrefix.TryFrom(  0, out var none),   Is.True);
            Assert.That(none,                                  Is.EqualTo(SIPrefix.None));

            Assert.That(SIPrefix.TryFrom(  3, out _),          Is.True);
            Assert.That(SIPrefix.TryFrom( -2, out _),          Is.True);
            Assert.That(SIPrefix.TryFrom(-30, out _),          Is.True);

            Assert.That(SIPrefix.TryFrom(  4, out _),          Is.False);
            Assert.That(SIPrefix.TryFrom( -4, out _),          Is.False);
            Assert.That(SIPrefix.TryFrom(  5, out _),          Is.False);
            Assert.That(SIPrefix.TryFrom( 33, out _),          Is.False);
            Assert.That(SIPrefix.TryFrom(-33, out _),          Is.False);

        }

        #endregion

        #region The_default_SIPrefix_is_None()

        [Test]
        public void The_default_SIPrefix_is_None()
        {

            Assert.That(default(SIPrefix),           Is.EqualTo(SIPrefix.None));
            Assert.That(default(SIPrefix).IsNone,    Is.True);
            Assert.That(default(SIPrefix).Symbol,    Is.EqualTo(""));

            // Prefixes are ordered by their decimal power...
            Assert.That(SIPrefix.Milli < SIPrefix.None,   Is.True);
            Assert.That(SIPrefix.Kilo  > SIPrefix.None,   Is.True);
            Assert.That(SIPrefix.Kilo  < SIPrefix.Mega,   Is.True);

        }

        #endregion

    }

}
