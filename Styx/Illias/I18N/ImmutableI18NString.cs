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
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An immutable internationalized (I18N) multi-language text,
    /// the immutable cousin of the I18NString.
    /// </summary>
    public sealed class ImmutableI18NString : IEquatable<ImmutableI18NString>,
                                              II18NString
    {

        #region Data

        private readonly ImmutableArray<I18NPair> i18NStrings;

        #endregion

        #region Properties

        /// <summary>
        /// The number of language/value pairs.
        /// </summary>
        public UInt32   Count
            => (UInt32) i18NStrings.Length;


        /// <summary>
        /// The empty multi-language text.
        /// </summary>
        public static ImmutableI18NString  Empty    { get; }
            = new (ImmutableArray<I18NPair>.Empty);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an immutable copy of the given multi-language text.
        /// </summary>
        /// <param name="I18NString">A multi-language text.</param>
        public ImmutableI18NString(I18NString I18NString)
            : this((I18NString ?? throw new ArgumentNullException(nameof(I18NString))).Pairs)
        { }

        /// <summary>
        /// Share the given texts of a multi-language text without copying them;
        /// they are valid already, so they are not checked again.
        /// </summary>
        private ImmutableI18NString(ImmutableArray<I18NPair> I18NPairs)
        {

            this.i18NStrings = I18NPairs;

            foreach (var i18NPair in i18NStrings)
                hashCode ^= HashCode.Combine(i18NPair.Language, i18NPair.Text);

        }

        /// <summary>
        /// Create a new immutable multi-language text from the given texts in their order.
        /// </summary>
        /// <param name="I18NStrings">The texts, at most one per language.</param>
        public ImmutableI18NString(IEnumerable<I18NPair> I18NStrings)
        {

            this.i18NStrings = [.. I18NStrings ?? []];

            for (var i = 0; i < i18NStrings.Length; i++)
                for (var j = i + 1; j < i18NStrings.Length; j++)
                    if (i18NStrings[i].Language == i18NStrings[j].Language)
                        throw new ArgumentException($"The language '{i18NStrings[i].Language}' must not be given twice!",
                                                    nameof(I18NStrings));

            // Independent of the order of the languages, as equality is.
            foreach (var i18NPair in i18NStrings)
                hashCode ^= HashCode.Combine(i18NPair.Language, i18NPair.Text);

        }

        /// <summary>
        /// Create a new immutable multi-language text from the given texts in their order.
        /// </summary>
        /// <param name="I18NStrings">The texts, at most one per language.</param>
        public ImmutableI18NString(IEnumerable<KeyValuePair<Languages, String>> I18NStrings)
            : this((I18NStrings ?? []).Select(kvp => new I18NPair(kvp.Key, kvp.Value)))
        { }

        #endregion


        #region (static) Create   (Language, Text)

        /// <summary>
        /// Create a new internationalized (I18N) multi-language string
        /// based on the given language and string.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        /// <param name="Text">The internationalized (I18N) text.</param>
        public static I18NString Create(Languages  Language,
                                        String     Text)

            => new (Language, Text);

        #endregion

        #region (static) Create   (          Text)

        /// <summary>
        /// Create a new internationalized (I18N) multi-language string
        /// based on english and the given string.
        /// </summary>
        /// <param name="Text">The internationalized (I18N) text.</param>
        public static I18NString Create(String Text)

            => new (Languages.en, Text);

        #endregion

        #region (static) Parse    (JSON)

        /// <summary>
        /// Parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static ImmutableI18NString Parse(JObject JSON)
        {

            if (TryParse(JSON, out var i18NText, out _))
                return i18NText;

            return Empty;

        }

        #endregion

        #region (static) TryParse (JSON, out I18NText, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        /// <param name="I18NText">The parsed multi-language string.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out ImmutableI18NString?  I18NText,
                                       [NotNullWhen(false)] out String?               ErrorResponse)
        {

            if (JSON is null)
            {
                I18NText       = null;
                ErrorResponse  = "Empty input!";
                return false;
            }

            var i18NText   = new I18NString();
            ErrorResponse  = null;

            foreach (var JSONProperty in JSON)
            {

                try
                {

                    if (JSONProperty.Key is not null             &&
                        JSONProperty.Key.IsNeitherNullNorEmpty() &&
                        JSONProperty.Value is not null           &&
                        JSONProperty.Value.Type == JTokenType.String)
                    {

                        var value = JSONProperty.Value.Value<String>();

                        if (value is not null &&
                            LanguagesExtensions.TryParse(JSONProperty.Key, out var language))
                        {
                            i18NText.Set(language, value);
                        }

                    }

                }
                catch (Exception e)
                {
                    I18NText      = null;
                    ErrorResponse = e.Message;
                    return false;
                }

            }

            I18NText = new ImmutableI18NString(i18NText);

            return true;

        }

        #endregion


        #region Has  (Language)

        /// <summary>
        /// Whether a text in the given language exists.
        /// </summary>
        /// <param name="Language">A language.</param>
        public Boolean Has(Languages Language)

            => IndexOf(Language) >= 0;

        #endregion

        #region With (Language, Text)

        /// <summary>
        /// Return a copy with the given text in the given language,
        /// replacing an existing text in this language at its place.
        /// </summary>
        /// <param name="Language">A language.</param>
        /// <param name="Text">A text.</param>
        public ImmutableI18NString With(Languages  Language,
                                        String     Text)

        {

            var index = IndexOf(Language);

            return new (index >= 0
                            ? i18NStrings.SetItem(index, new I18NPair(Language, Text))
                            : i18NStrings.Add    (       new I18NPair(Language, Text)));

        }

        #endregion

        #region this[Language]

        /// <summary>
        /// The text in the given language, or an empty text.
        /// </summary>
        /// <param name="Language">A language.</param>
        public String this[Languages Language]
        {
            get
            {

                var index = IndexOf(Language);

                return index >= 0
                           ? i18NStrings[index].Text
                           : String.Empty;

            }
        }

        #endregion


        #region (private) IndexOf(Language)

        /// <summary>
        /// The position of the text in the given language, or -1.
        /// A linear search is faster than hashing for a handful of languages.
        /// </summary>
        private Int32 IndexOf(Languages Language)
        {

            for (var i = 0; i < i18NStrings.Length; i++)
                if (i18NStrings[i].Language == Language)
                    return i;

            return -1;

        }

        #endregion

        #region ToMutable()

        /// <summary>
        /// Return a mutable copy of this multi-language text.
        /// </summary>
        public I18NString ToMutable()
            => new (i18NStrings);

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
        /// Create an immutable copy of the given multi-language text; null stays null.
        /// </summary>
        /// <param name="I18NString">A multi-language text.</param>
        [return: NotNullIfNotNull(nameof(I18NString))]
        public static implicit operator ImmutableI18NString?(I18NString? I18NString)

            => I18NString is not null
                   ? new (I18NString)
                   : null;

        #endregion

        #region Implicit conversion to I18NString

        /// <summary>
        /// Create a mutable copy of the given multi-language text; null stays null.
        /// </summary>
        /// <param name="ImmutableI18NString">An immutable multi-language text.</param>
        [return: NotNullIfNotNull(nameof(ImmutableI18NString))]
        public static implicit operator I18NString?(ImmutableI18NString? ImmutableI18NString)

            => ImmutableI18NString?.ToMutable();

        #endregion

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
               i18NStrings.Length == ImmutableI18NString.i18NStrings.Length &&
               i18NStrings.All(i18NPair => ImmutableI18NString.IndexOf(i18NPair.Language) is var index &&
                                           index >= 0 &&
                                           ImmutableI18NString.i18NStrings[index].Text == i18NPair.Text);

        #endregion

        #endregion

        #region IEnumerable<I18NPair> Members

        /// <summary>
        /// Enumerate the texts in their order.
        /// </summary>
        public IEnumerator<I18NPair> GetEnumerator()
            => ((IEnumerable<I18NPair>) i18NStrings).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

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

            => String.Join("; ", i18NStrings.Select(i18NPair => $"{i18NPair.Language}: {i18NPair.Text}"));

        #endregion

    }

}
