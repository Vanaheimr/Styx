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
    /// ImmutableI18NString tests.
    /// </summary>
    [TestFixture]
    public class ImmutableI18NString_Tests
    {

        #region A_copy_reads_like_its_source()

        [Test]
        public void A_copy_reads_like_its_source()
        {

            var source = new I18NString(Languages.de, "Hallo").Set(Languages.en, "Hello");
            var copy   = new ImmutableI18NString(source);

            Assert.That(copy.Count,                                              Is.EqualTo(2));
            Assert.That(copy[Languages.en],                                      Is.EqualTo("Hello"));
            Assert.That(copy[Languages.fr],                                      Is.Empty);
            Assert.That(copy.Has(Languages.de),                                  Is.True);
            Assert.That(copy.FirstText(),                                        Is.EqualTo("Hallo"));
            Assert.That(copy.ToString(),                                         Is.EqualTo(source.ToString()));
            Assert.That(copy.ToJSON().ToString(),                                Is.EqualTo(source.ToJSON().ToString()));
            Assert.That(copy.ToMutable(),                                        Is.EqualTo(source));
            Assert.That(ImmutableI18NString.Parse(source.ToJSON()),              Is.EqualTo(copy));

        }

        #endregion

        #region A_copy_is_detached_from_its_source()

        [Test]
        public void A_copy_is_detached_from_its_source()
        {

            var source = new I18NString(Languages.de, "Hallo");
            var copy   = new ImmutableI18NString(source);

            source.Set(Languages.de, "Servus");
            copy.ToMutable().Set(Languages.de, "Moin");

            Assert.That(copy[Languages.de], Is.EqualTo("Hallo"));

        }

        #endregion

        #region With_replaces_a_text_at_its_place_or_appends_it()

        [Test]
        public void With_replaces_a_text_at_its_place_or_appends_it()
        {

            var copy = new ImmutableI18NString(new I18NString(Languages.de, "Hallo").Set(Languages.en, "Hello"));

            Assert.That(copy.With(Languages.de, "Servus").Select(text => text.Text), Is.EqualTo(new[] { "Servus", "Hello" }));
            Assert.That(copy.With(Languages.fr, "Salut"). Select(text => text.Text), Is.EqualTo(new[] { "Hallo", "Hello", "Salut" }));
            Assert.That(copy[Languages.de],                                          Is.EqualTo("Hallo"));

        }

        #endregion

        #region Equality_ignores_the_order_of_languages()

        [Test]
        public void Equality_ignores_the_order_of_languages()
        {

            var deEn = new ImmutableI18NString(new I18NString(Languages.de, "Hallo").Set(Languages.en, "Hello"));
            var enDe = new ImmutableI18NString(new I18NString(Languages.en, "Hello").Set(Languages.de, "Hallo"));

            Assert.That(deEn,                Is.EqualTo(enDe));
            Assert.That(deEn.GetHashCode(),  Is.EqualTo(enDe.GetHashCode()));
            Assert.That(deEn,                Is.Not.EqualTo(enDe.With(Languages.de, "Servus")));
            Assert.That(ImmutableI18NString.Empty,                Is.EqualTo(new ImmutableI18NString(I18NString.Empty)));
            Assert.That(ImmutableI18NString.Empty.ToJSON().Count, Is.Zero);
            Assert.That(ImmutableI18NString.Empty.ToString(),     Is.Empty);

        }

        #endregion

    }

}
