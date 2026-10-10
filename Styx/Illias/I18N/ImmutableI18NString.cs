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

using System.Collections;
using System.Collections.Immutable;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An immutable internationalized (I18N) multi-language text,
    /// the immutable cousin of the I18NString.
    /// </summary>
    public sealed class ImmutableI18NString : IEquatable<ImmutableI18NString>,
                                              IEnumerable<I18NPair>
    {

        #region Data

        private readonly ImmutableArray<I18NPair>  texts;
        private readonly Int32                     hashCode;

        #endregion

        #region Properties

        /// <summary>
        /// The number of languages.
        /// </summary>
        public UInt32 Count
            => (UInt32) texts.Length;

        /// <summary>
        /// The text in the given language, or an empty text.
        /// </summary>
        /// <param name="Language">A language.</param>
        public String this[Languages Language]
        {
            get
            {

                foreach (var text in texts)
                {
                    if (text.Language == Language)
                        return text.Text;
                }

                return String.Empty;

            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an immutable copy of the given multi-language text.
        /// </summary>
        /// <param name="I18NString">A multi-language text.</param>
        public ImmutableI18NString(I18NString I18NString)
            : this([.. I18NString ?? throw new ArgumentNullException(nameof(I18NString))])
        { }

        private ImmutableI18NString(ImmutableArray<I18NPair> Texts)
        {

            this.texts = Texts;

            // Order-independent, as the mutable I18NString
            foreach (var text in texts)
                hashCode ^= text.Language.GetHashCode() ^ text.Text.GetHashCode();

        }

        #endregion


        #region (static) Empty

        /// <summary>
        /// The empty multi-language text.
        /// </summary>
        public static ImmutableI18NString Empty { get; }
            = new (ImmutableArray<I18NPair>.Empty);

        #endregion

        #region (static) Parse(JSON)

        /// <summary>
        /// Parse the given JSON representation of a multi-language text.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        public static ImmutableI18NString Parse(JObject JSON)

            => new (I18NString.Parse(JSON) ?? I18NString.Empty);

        #endregion


        #region Has             (Language)

        /// <summary>
        /// Whether a text in the given language exists.
        /// </summary>
        /// <param name="Language">A language.</param>
        public Boolean Has(Languages Language)

            => texts.Any(text => text.Language == Language);

        #endregion

        #region IsNullOrEmpty   ()

        /// <summary>
        /// Whether no text exists.
        /// </summary>
        public Boolean IsNullOrEmpty()
            => texts.IsEmpty;

        #endregion

        #region IsNotNullOrEmpty()

        /// <summary>
        /// Whether at least one text exists.
        /// </summary>
        public Boolean IsNotNullOrEmpty()
            => !texts.IsEmpty;

        #endregion

        #region FirstText       ()

        /// <summary>
        /// The first text, or an empty text.
        /// </summary>
        public String FirstText()

            => texts.IsEmpty
                   ? String.Empty
                   : texts[0].Text;

        #endregion

        #region With            (Language, Text)

        /// <summary>
        /// Return a copy with the given text in the given language,
        /// replacing an existing text in this language at its place.
        /// </summary>
        /// <param name="Language">A language.</param>
        /// <param name="Text">A text.</param>
        public ImmutableI18NString With(Languages  Language,
                                        String     Text)
        {

            for (var index = 0; index < texts.Length; index++)
            {
                if (texts[index].Language == Language)
                    return new (texts.SetItem(index, new I18NPair(Language, Text)));
            }

            return new (texts.Add(new I18NPair(Language, Text)));

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this multi-language text.
        /// </summary>
        public JObject ToJSON()

            => new (texts.Select(text => new JProperty(text.Language.ToString(), text.Text)));

        #endregion

        #region ToMutable()

        /// <summary>
        /// Return a mutable copy of this multi-language text.
        /// </summary>
        public I18NString ToMutable()
            => new (texts);

        #endregion

        #region Clone()

        /// <summary>
        /// An immutable value is its own clone.
        /// </summary>
        public ImmutableI18NString Clone()
            => this;

        #endregion


        #region Operator overloading

        #region Implicit conversion from I18NString

        /// <summary>
        /// Create an immutable copy of the given multi-language text.
        /// </summary>
        /// <param name="I18NString">A multi-language text.</param>
        public static implicit operator ImmutableI18NString(I18NString I18NString)
            => new (I18NString);

        #endregion

        #region Implicit conversion to I18NString

        /// <summary>
        /// Create a mutable copy of the given multi-language text.
        /// </summary>
        /// <param name="ImmutableI18NString">An immutable multi-language text.</param>
        public static implicit operator I18NString(ImmutableI18NString ImmutableI18NString)
            => ImmutableI18NString.ToMutable();

        #endregion

        #endregion

        #region IEnumerable<I18NPair> Members

        /// <summary>
        /// Enumerate the texts in their order.
        /// </summary>
        public IEnumerator<I18NPair> GetEnumerator()
            => ((IEnumerable<I18NPair>) texts).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion

        #region IEquatable<ImmutableI18NString> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two multi-language texts for equality.
        /// </summary>
        /// <param name="Object">A multi-language text to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ImmutableI18NString immutableI18NString &&
                   Equals(immutableI18NString);

        #endregion

        #region Equals(ImmutableI18NString)

        /// <summary>
        /// Compares two multi-language texts for equality, regardless of the order of their languages.
        /// </summary>
        /// <param name="ImmutableI18NString">A multi-language text to compare with.</param>
        public Boolean Equals(ImmutableI18NString? ImmutableI18NString)

            => ImmutableI18NString is not null &&
               texts.Length == ImmutableI18NString.texts.Length &&
               texts.All(text => ImmutableI18NString.Has(text.Language) &&
                                 ImmutableI18NString[text.Language] == text.Text);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Join("; ", texts.Select(text => $"{text.Language}: {text.Text}"));

        #endregion

    }

}
