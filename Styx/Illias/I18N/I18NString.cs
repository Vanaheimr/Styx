/*
 * Copyright (c) 2010-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for internationalized (I18N) multi-language texts/strings.
    /// </summary>
    public static class I18NStringExtensions
    {

        #region IsNullOrEmpty(this I18NText)

        /// <summary>
        /// The multi-language string is empty.
        /// </summary>
        public static Boolean IsNullOrEmpty(this I18NString I18NText)

            => I18NText is null || !I18NText.Any();

        #endregion

        #region IsNeitherNullNorEmpty(this Text)

        /// <summary>
        /// The multi-language string is neither null nor empty.
        /// </summary>
        public static Boolean IsNeitherNullNorEmpty(this I18NString I18NText)

            => I18NText is not null &&
               I18NText.Any();

        #endregion

        #region FirstText(this I18NText)

        /// <summary>
        /// Return the first string of a multi-language string.
        /// </summary>
        public static String FirstText(this I18NString I18NText)

            => I18NText is not null && I18NText.IsNeitherNullNorEmpty()
                   ? I18NText.First().Text
                   : String.Empty;

        #endregion

        #region ToI18NString(this I18NText, Language = Languages.en)

        /// <summary>
        /// Return the first string of a multi-language string.
        /// </summary>
        public static I18NString ToI18NString(this String  I18NText,
                                              Languages    Language = Languages.en)

            => I18NText is not null && I18NText.IsNotNullOrEmpty()
                   ? I18NString.Create(Language, I18NText)
                   : I18NString.Empty;

        #endregion

        #region SubstringMax(this I18NText, Length)

        /// <summary>
        /// Return a substring of the given maximum length.
        /// </summary>
        /// <param name="I18NText">A text.</param>
        /// <param name="Length">The maximum length of the substring.</param>
        public static I18NString SubstringMax(this I18NString I18NText, Int32 Length)
        {

            if (I18NText is null)
                return I18NString.Empty;

            return new I18NString(I18NText.Select(text => new I18NPair(
                                                              text.Language,
                                                              text.Text.Substring(0, Math.Min(text.Text.Length, Length))
                                                          )));

        }

        #endregion

        #region TrimAll(this I18NText)

        /// <summary>
        /// Trim all texts.
        /// </summary>
        /// <param name="I18NText">A text.</param>
        public static I18NString TrimAll(this I18NString I18NText)
        {

            if (I18NText is null)
                return I18NString.Empty;

            return new I18NString(I18NText.Select(text => new I18NPair(
                                                              text.Language,
                                                              text.Text.Trim()
                                                          )));

        }

        #endregion

        #region ToHTML(this I18NString)

        /// <summary>
        /// Convert the given internationalized (I18N) text/string to HTML.
        /// </summary>
        /// <param name="I18NString">An internationalized (I18N) text/string.</param>
        public static String ToHTML(this I18NString I18NString)
        {

            return I18NString.
                       Select(v => @"<span class=""I18N_" + v.Language + @""">" + v.Text + "</span>").
                       AggregateWith(Environment.NewLine);

        }

        #endregion

        #region ToHTML(this I18NString, Prefix, Postfix)

        /// <summary>
        /// Convert the given internationalized (I18N) text/string to HTML.
        /// </summary>
        /// <param name="I18NString">An internationalized (I18N) text/string.</param>
        /// <param name="Prefix">A prefix.</param>
        /// <param name="Postfix">A postfix.</param>
        public static String ToHTML(this I18NString I18NString, String Prefix, String Postfix)
        {

            return I18NString.
                       Select(v => @"<span class=""I18N_" + v.Language + @""">" + Prefix + v.Text + Postfix + "</span>").
                       AggregateWith(Environment.NewLine);

        }

        #endregion

        #region ToHTMLLink(this I18NString, String URI)

        /// <summary>
        /// Convert the given internationalized (I18N) text/string to a HTML link.
        /// </summary>
        /// <param name="I18NString">An internationalized (I18N) text/string.</param>
        /// <param name="URI">An URI.</param>
        public static String ToHTMLLink(this I18NString I18NString, String URI)
        {

            return I18NString.
                       Select(v => @"<span class=""I18N_" + v.Language + @"""><a href=""" + URI + @"?language=en"">" + v.Text + "</a></span>").
                       AggregateWith(Environment.NewLine);

        }

        #endregion

    }


    /// <summary>
    /// An internationalized (I18N) multi-language text/string.
    /// </summary>
    public class I18NString : IEquatable<I18NString>,
                              IEnumerable<I18NPair>
    {

        #region Data

        private readonly Dictionary<Languages, String> i18NStrings;

        #endregion

        #region Events

        /// <summary>
        /// An event called whenever the multi-language text changed.
        /// </summary>
        public event OnPropertyChangedDelegate? OnPropertyChanged;

        #endregion

        #region Constructor(s)

        #region I18NString()

        /// <summary>
        /// Create a new internationalized (I18N) multi-language string.
        /// </summary>
        public I18NString()
        {
            this.i18NStrings = new Dictionary<Languages, String>();
        }

        #endregion

        #region I18NString(Language, Text)

        /// <summary>
        /// Create a new internationalized (I18N) multi-language string
        /// based on the given language and string.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        /// <param name="Text">The internationalized (I18N) text.</param>
        public I18NString(Languages Language, String Text)
            : this()
        {
            i18NStrings.Add(Language, Text);
        }

        #endregion

        #region I18NString(Texts)

        /// <summary>
        /// Create a new internationalized (I18N) multi-language string
        /// based on the given language and string pairs.
        /// </summary>
        public I18NString(KeyValuePair<Languages, String>[] Texts)
            : this()
        {

            if (Texts != null)
                foreach (var Text in Texts)
                    i18NStrings.Add(Text.Key, Text.Value);

        }

        #endregion

        #region I18NString(I18NPairs)

        /// <summary>
        /// Create a new internationalized (I18N) multi-language string
        /// based on the given I18N-pairs.
        /// </summary>
        public I18NString(IEnumerable<I18NPair> I18NPairs)
            : this()
        {

            if (I18NPairs != null)
                foreach (var Text in I18NPairs)
                    i18NStrings.Add(Text.Language, Text.Text);

        }

        #endregion

        #region I18NString(params I18NPairs)

        /// <summary>
        /// Create a new internationalized (I18N) multi-language string
        /// based on the given I18N-pairs.
        /// </summary>
        public I18NString(params I18NPair[] I18NPairs)
            : this()
        {

            if (I18NPairs != null)
                foreach (var Text in I18NPairs)
                    i18NStrings.Add(Text.Language, Text.Text);

        }

        #endregion

        #endregion


        #region (static) Create(Language, Text)

        /// <summary>
        /// Create a new internationalized (I18N) multi-language string
        /// based on the given language and string.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        /// <param name="Text">The internationalized (I18N) text.</param>
        public static I18NString Create(Languages  Language,
                                        String     Text)

            => new I18NString(Language, Text);

        #endregion

        #region (static) Empty

        /// <summary>
        /// Create an empty internationalized (I18N) multi-language string.
        /// </summary>
        public static I18NString Empty
            => new I18NString();

        #endregion


        #region this[Language]

        /// <summary>
        /// Set the text specified by the given language.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        public String this[Languages Language]
        {

            get
            {


                if (i18NStrings.TryGetValue(Language, out var text))
                    return text;

                return String.Empty;

            }

            set
            {

                var oldText = i18NStrings[Language];
                if (oldText != value)
                {

                    i18NStrings[Language] = value;

                    OnPropertyChanged?.Invoke(Timestamp.Now,
                                              EventTracking_Id.New,
                                              this,
                                              "",
                                              new Tuple<Languages, String>(Language, oldText),
                                              new Tuple<Languages, String>(Language, value));

                }

            }

        }

        #endregion

        #region Set(Language, Text)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// internationalized (I18N) multi-language string.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        /// <param name="Text">The internationalized (I18N) text.</param>
        public I18NString Set(Languages  Language,
                              String     Text)
        {

            if (!i18NStrings.ContainsKey(Language))
            {

                i18NStrings.Add(Language, Text);

                OnPropertyChanged?.Invoke(Timestamp.Now,
                                          EventTracking_Id.New,
                                          this,
                                          "",
                                          null,
                                          new Tuple<Languages, String>(Language, Text));

            }

            else
            {

                var oldText = i18NStrings[Language];
                if (oldText != Text)
                {

                    i18NStrings[Language] = Text;

                    OnPropertyChanged?.Invoke(Timestamp.Now,
                                              EventTracking_Id.New,
                                              this,
                                              "",
                                              new Tuple<Languages, String>(Language, oldText),
                                              new Tuple<Languages, String>(Language, Text));

                }

            }

            return this;

        }

        #endregion

        #region Set(I18NPair)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// internationalized (I18N) multi-language string.
        /// </summary>
        /// <param name="I18NPair">The internationalized (I18N) text.</param>
        public I18NString Set(I18NPair I18NPair)

            => Set(I18NPair.Language,
                   I18NPair.Text);

        #endregion

        #region Set(I18NPairs)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// internationalized (I18N) multi-language string.
        /// </summary>
        /// <param name="I18NPairs">An enumeration of internationalized (I18N) texts.</param>
        public I18NString Set(IEnumerable<I18NPair> I18NPairs)
        {

            foreach (var I18NPair in I18NPairs)
                Set(I18NPair.Language,
                    I18NPair.Text);

            return this;

        }

        #endregion


        #region has  (Language)

        /// <summary>
        /// Checks if the given language representation exists.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        public Boolean has(Languages Language)

            => i18NStrings.ContainsKey(Language);

        #endregion

        #region Is   (Language, Value)

        public Boolean Is(Languages  Language,
                          String     Value)
        {

            if (!i18NStrings.ContainsKey(Language))
                return false;

            return i18NStrings[Language].Equals(Value);

        }

        #endregion

        #region IsNot(Language, Value)

        public Boolean IsNot(Languages  Language,
                             String     Value)
        {

            if (!i18NStrings.ContainsKey(Language))
                return true;

            return !i18NStrings[Language].Equals(Value);

        }

        #endregion

        #region Matches(Match, IgnoreCase = false)

        public Boolean Matches(String   Match,
                               Boolean  IgnoreCase  = false)

            => i18NStrings.Any(kvp => IgnoreCase
                                          ? kvp.Value.IndexOf(Match, StringComparison.OrdinalIgnoreCase) >= 0
                                          : kvp.Value.IndexOf(Match) >= 0);

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given text as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="Text">A string of a JSON representation of a multi-language string.</param>
        public static I18NString? Parse(String Text)
        {

            if (TryParse(Text, out I18NString? i18NText))
                return i18NText;

            return Empty;

        }

        #endregion

        #region Parse(JSON)

        /// <summary>
        /// Parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static I18NString? Parse(JObject JSON)
        {

            if (TryParse(JSON, out I18NString? i18NText))
                return i18NText;

            return Empty;

        }

        #endregion

        #region Parse<TI18NString>(JSON)

        /// <summary>
        /// Parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static TI18NString? Parse<TI18NString>(JObject JSON)
            where TI18NString : I18NString, new()
        {

            if (TryParse(JSON, out TI18NString? i18NText))
                return i18NText;

            return new TI18NString();

        }

        #endregion

        #region TryParse(Text, out I18NText)

        /// <summary>
        /// Try to parse the given text as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="Text">A string of a JSON representation of a multi-language string.</param>
        /// <param name="I18NText"></param>
        public static Boolean TryParse<TI18NString>(String Text, out TI18NString? I18NText)
            where TI18NString : I18NString, new()
        {

            I18NText = new TI18NString();

            if (Text.IsNullOrEmpty())
                return false;

            try
            {
                return TryParse(JObject.Parse(Text), out I18NText);
            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion

        #region TryParse(JSON, out I18NText)

        /// <summary>
        /// Try to parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static Boolean TryParse<TI18NString>(JObject JSON, out TI18NString? I18NText)
            where TI18NString : I18NString, new()
        {

            I18NText = new TI18NString();

            if (JSON is null)
                return true;

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
                            I18NText.Set(language, value);
                        }

                    }

                }
                catch (Exception)
                {
                    I18NText = null;
                    return false;
                }

            }

            return true;

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of the given internationalized string.
        /// </summary>
        public JObject ToJSON()

            => i18NStrings.Any()
                   ? new JObject(i18NStrings.Select(i18n => new JProperty(i18n.Key.ToString(), i18n.Value)))
                   : new JObject();

        #endregion


        #region Remove(Language)

        /// <summary>
        /// Remove the given language from the internationalized (I18N) multi-language text.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        public I18NString Remove(Languages Language)
        {

            if (i18NStrings.ContainsKey(Language))
                i18NStrings.Remove(Language);

            return this;

        }

        #endregion


        #region Count

        /// <summary>
        /// The number of language/value pairs.
        /// </summary>
        public UInt32 Count

            => (UInt32) i18NStrings.Count;

        #endregion

        #region Clone

        /// <summary>
        /// Clone this energy source.
        /// </summary>
        public I18NString Clone

            => new I18NString(i18NStrings.SafeSelect(i18n => new I18NPair(i18n.Key, new String(i18n.Value.ToCharArray()))));

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Enumerate all internationalized (I18N) texts.
        /// </summary>
        public IEnumerator<I18NPair> GetEnumerator()
            => i18NStrings.Select(kvp => new I18NPair(kvp.Key, kvp.Value)).GetEnumerator();

        /// <summary>
        /// Enumerate all internationalized (I18N) texts.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => i18NStrings.Select(kvp => new I18NPair(kvp.Key, kvp.Value)).GetEnumerator();

        #endregion

        #region Operator overloading

        #region Operator == (I18NString1, I18NString2)

        /// <summary>
        /// Compares two I18N-strings for equality.
        /// </summary>
        /// <param name="I18NString1">A I18N-string.</param>
        /// <param name="I18NString2">Another I18N-string.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (I18NString? I18NString1,
                                           I18NString? I18NString2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(I18NString1, I18NString2))
                return true;

            // If one is null, but not both, return false.
            if (I18NString1 is null || I18NString2 is null)
                return false;

            return I18NString1.Equals(I18NString2);

        }

        #endregion

        #region Operator != (I18NString1, I18NString2)

        /// <summary>
        /// Compares two I18N-strings for inequality.
        /// </summary>
        /// <param name="I18NString1">A I18N-string.</param>
        /// <param name="I18NString2">Another I18N-string.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (I18NString? I18NString1,
                                           I18NString? I18NString2)

            => !(I18NString1 == I18NString2);

        #endregion

        #endregion

        #region IEquatable<I18NString> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is I18NString i18NString &&
                  Equals(i18NString);

        #endregion

        #region Equals(I18NString)

        /// <summary>
        /// Compares two I18NString for equality.
        /// </summary>
        /// <param name="OtherI18NString">An I18NString to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(I18NString? OtherI18NString)
        {

            if (OtherI18NString is null)
                return false;

            if (i18NStrings.Count != OtherI18NString.Count)
                return false;

            foreach (var kvp in i18NStrings)
            {
                if (kvp.Value != OtherI18NString[kvp.Key])
                    return false;
            }

            return true;

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {

            Int32 returnValue = 0;

            foreach (var Value in i18NStrings.
                                      Select(I18N => I18N.Key.GetHashCode() ^ I18N.Value.GetHashCode()))
            {
                returnValue ^= Value;
            }

            return returnValue;

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {

            if (i18NStrings.Count == 0)
                return String.Empty;

            return i18NStrings.
                       Select(I18N => I18N.Key.ToString() + ": " + I18N.Value).
                       AggregateWith("; ");

        }

        #endregion

    }

}
