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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Tests
{

    /// <summary>
    /// Tests for the RFC 8949, Section 8 diagnostic notation.
    /// </summary>
    [TestFixture]
    public class CBORDiagnosticNotationTests
    {

        #region Text_strings_are_escaped_like_JSON()

        [Test]
        public void Text_strings_are_escaped_like_JSON()
        {

            Assert.That(CBORValue.FromText("a\"b\\c\nd").ToDiagnosticString(),
                        Is.EqualTo("\"a\\\"b\\\\c\\nd\""));

            Assert.That(CBORValue.FromText("tab\there").ToDiagnosticString(),
                        Is.EqualTo("\"tab\\there\""));

            Assert.That(CBORValue.FromText("\u0001").ToDiagnosticString(),
                        Is.EqualTo("\"\\u0001\""));

        }

        #endregion

        #region Byte_strings_render_as_h_quoted_lowercase_hex()

        [Test]
        public void Byte_strings_render_as_h_quoted_lowercase_hex()
        {

            Assert.That(CBORValue.FromBytes([ 0xAB, 0xCD, 0xEF ]).ToDiagnosticString(),
                        Is.EqualTo("h'abcdef'"));

            Assert.That(CBORValue.FromBytes([]).ToDiagnosticString(),
                        Is.EqualTo("h''"));

        }

        #endregion

        #region Decimal_fractions_render_as_tagged_arrays()

        [Test]
        public void Decimal_fractions_render_as_tagged_arrays()
        {

            Assert.That(CBORValue.FromDecimal(5.0m).ToDiagnosticString(),
                        Is.EqualTo("4([-1, 50])"));

            Assert.That(CBORValue.FromDecimal(-1.1m).ToDiagnosticString(),
                        Is.EqualTo("4([-1, -11])"));

            Assert.That(CBORValue.FromDecimal(273.15m).ToDiagnosticString(),
                        Is.EqualTo("4([-2, 27315])"));

        }

        #endregion

        #region Special_floats_render_as_Infinity_and_NaN()

        [Test]
        public void Special_floats_render_as_Infinity_and_NaN()
        {

            Assert.That(CBORValue.FromDouble(Double.PositiveInfinity).ToDiagnosticString(),  Is.EqualTo("Infinity"));
            Assert.That(CBORValue.FromDouble(Double.NegativeInfinity).ToDiagnosticString(),  Is.EqualTo("-Infinity"));
            Assert.That(CBORValue.FromDouble(Double.NaN).ToDiagnosticString(),               Is.EqualTo("NaN"));
            Assert.That(CBORValue.FromDouble(-0.0).ToDiagnosticString(),                     Is.EqualTo("-0.0"));

        }

        #endregion

        #region Diagnostic_output_is_culture_invariant()

        [Test]
        public void Diagnostic_output_is_culture_invariant()
        {

            // The CI runs on Windows AND within a debian:13 container,
            // so decimal separators must not depend on the current culture!
            var previousCulture = CultureInfo.CurrentCulture;

            try
            {

                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                Assert.That(CBORValue.FromDouble(1.1).ToDiagnosticString(),
                            Is.EqualTo("1.1"));

                Assert.That(CBORValue.FromDouble(1.0e+300).ToDiagnosticString(),
                            Is.EqualTo("1.0e+300"));

                Assert.That(CBORValue.FromDecimal(-273.15m).ToDiagnosticString(),
                            Is.EqualTo("4([-2, -27315])"));

            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }

        }

        #endregion

    }

}
