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

        #region Mutable_texts_keep_their_order_and_one_text_per_language()

        [Test]
        public void Mutable_texts_keep_their_order_and_one_text_per_language()
        {

            var text = new I18NString(Languages.de, "Hallo").Set(Languages.en, "Hello").Set(Languages.fr, "Salut");

            text.Set(Languages.en, "Hi");
            Assert.That(text.Select(pair => pair.Text),     Is.EqualTo(new[] { "Hallo", "Hi", "Salut" }));

            text.Remove(Languages.de).Set(Languages.de, "Servus");
            Assert.That(text.Select(pair => pair.Language), Is.EqualTo(new[] { Languages.en, Languages.fr, Languages.de }));

            text[Languages.it] = "Ciao";
            Assert.That(text[Languages.it],                 Is.EqualTo("Ciao"));
            Assert.That(text.Count,                         Is.EqualTo(4));

            var copy = text.Clone();
            text.Set(Languages.en, "Hello");
            Assert.That(copy[Languages.en],                 Is.EqualTo("Hi"));

            Assert.That(() => new I18NString(new I18NPair(Languages.de, "a"), new I18NPair(Languages.de, "b")),
                        Throws.ArgumentException);

            Assert.That(new I18NString(Languages.de, "Hallo").Set(Languages.en, "Hello"),
                        Is.EqualTo(new I18NString(Languages.en, "Hello").Set(Languages.de, "Hallo")));
            Assert.That(new I18NString(Languages.de, "Hallo").Set(Languages.en, "Hello").GetHashCode(),
                        Is.EqualTo(new I18NString(Languages.en, "Hello").Set(Languages.de, "Hallo").GetHashCode()));
            Assert.That(new I18NString(Languages.de, ""),   Is.Not.EqualTo(new I18NString(Languages.en, "")));

        }

        #endregion

        #region Implicit_conversions_keep_null_and_the_order_of_languages()

        [Test]
        public void Implicit_conversions_keep_null_and_the_order_of_languages()
        {

            I18NString?          nullMutable    = null;
            ImmutableI18NString? nullImmutable  = null;

            ImmutableI18NString? immutable      = nullMutable;
            I18NString?          mutable        = nullImmutable;

            Assert.That(immutable,  Is.Null);
            Assert.That(mutable,    Is.Null);

            ImmutableI18NString  copy           = new I18NString(Languages.en, "Hello").Set(Languages.de, "Hallo");
            I18NString           back           = copy;

            Assert.That(copy.Select(text => text.Language),  Is.EqualTo(new[] { Languages.en, Languages.de }));
            Assert.That(back.Select(text => text.Language),  Is.EqualTo(new[] { Languages.en, Languages.de }));
            Assert.That(back[Languages.de],                  Is.EqualTo("Hallo"));

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
