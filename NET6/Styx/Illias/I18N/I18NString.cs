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

using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for internationalized (I18N) multi-language texts/strings.
    /// </summary>
    public static class I18NStringExtensions
    {

        /// <summary>
        /// The multi-language string is empty.
        /// </summary>
        public static Boolean IsNullOrEmpty(this I18NString Text)
            => Text == null || !Text.Any();

        /// <summary>
        /// The multi-language string is neither null nor empty.
        /// </summary>
        public static Boolean IsNeitherNullNorEmpty(this I18NString Text)
            => Text != null && Text.Any();

        /// <summary>
        /// Return the first string of a multi-language string.
        /// </summary>
        public static String FirstText(this I18NString Text)
            => Text != null && Text.Any()
                   ? Text.First().Text
                   : null;

        /// <summary>
        /// Return the first string of a multi-language string.
        /// </summary>
        public static I18NString ToI18NString(this String  Text,
                                              Languages    Language = Languages.en)
            => Text.IsNotNullOrEmpty()
                   ? I18NString.Create(Language, Text)
                   : null;


        #region SubstringMax(this I18NText, Length)

        /// <summary>
        /// Return a substring of the given maximum length.
        /// </summary>
        /// <param name="I18NText">A text.</param>
        /// <param name="Length">The maximum length of the substring.</param>
        public static I18NString SubstringMax(this I18NString I18NText, Int32 Length)
        {

            if (I18NText == null)
                return null;

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

            if (I18NText == null)
                return null;

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


        #region ToJSON(this I18NString, JPropertyKey)

        /// <summary>
        /// Return a JSON representation of the given internationalized string.
        /// </summary>
        /// <param name="I18NString">An internationalized string.</param>
        /// <param name="JPropertyKey">The name of the JSON property key.</param>
        public static JProperty ToJSON(this I18NString I18NString, String JPropertyKey)
        {

            if (I18NString == null || !I18NString.Any())
                return null;

            return new JProperty(JPropertyKey, I18NString.ToJSON());

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

        #region Add(Language, Text)

        /// <summary>
        /// Add a new language-text-pair to the given
        /// internationalized (I18N) multi-language string.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        /// <param name="Text">The internationalized (I18N) text.</param>
        public I18NString Add(Languages  Language,
                              String     Text)
        {

            if (!i18NStrings.ContainsKey(Language))
                i18NStrings.Add(Language, Text);

            else
                i18NStrings[Language] = Text;

            return this;

        }

        #endregion

        #region Add(I18NPair)

        /// <summary>
        /// Add a new language-text-pair to the given
        /// internationalized (I18N) multi-language string.
        /// </summary>
        /// <param name="I18NPair">The internationalized (I18N) text.</param>
        public I18NString Add(I18NPair I18NPair)
        {

            if (!i18NStrings.ContainsKey(I18NPair.Language))
                i18NStrings.Add(I18NPair.Language, I18NPair.Text);

            else
                i18NStrings[I18NPair.Language] = I18NPair.Text;

            return this;

        }

        #endregion

        #region has(Language)

        /// <summary>
        /// Checks if the given language representation exists.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        public Boolean has(Languages Language)

            => i18NStrings.ContainsKey(Language);

        #endregion

        #region this[Language]

        /// <summary>
        /// Get the text specified by the given language.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        /// <returns>The internationalized (I18N) text or String.Empty</returns>
        public String this[Languages Language]
        {

            get
            {


                if (i18NStrings.TryGetValue(Language, out String Text))
                    return Text;

                return String.Empty;

            }

            set
            {
                i18NStrings[Language] = value;
            }

        }

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


        public static I18NString Parse(String Text)
        {

            if (TryParse(Text, out I18NString I18NText))
                return I18NText;

            return Empty;

        }


        public static I18NString Parse(JObject JSON)
        {

            if (TryParse(JSON, out I18NString I18NText))
                return I18NText;

            return Empty;

        }


        public static T Parse<T>(JObject JSON)
            where T : I18NString, new()
        {

            if (TryParse(JSON, out T I18NText))
                return I18NText;

            return new T();

        }

        public static Boolean TryParse<T>(String Text, out T I18NText)
            where T : I18NString, new()
        {

            I18NText = new T();

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

        public static Boolean TryParse<T>(JObject JSON, out T I18NText)
            where T : I18NString, new()
        {

            I18NText = new T();

            if (JSON == null)
                return true;

            foreach (var JSONProperty in JSON)
            {

                try
                {

                    I18NText.Add(LanguagesExtensions.Parse(JSONProperty.Key),
                                 JSONProperty.Value.Value<String>());

                }
                catch (Exception e)
                {
                    I18NText = null;
                    return false;
                }

            }

            return true;

        }

        #region Count

        /// <summary>
        /// The number of language/value pairs.
        /// </summary>
        public UInt32 Count

            => (UInt32) i18NStrings.Count;

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of the given internationalized string.
        /// </summary>
        public JObject ToJSON()

            => i18NStrings.Any()
                   ? new JObject(i18NStrings.SafeSelect(i18n => new JProperty(i18n.Key.ToString(), i18n.Value)))
                   : new JObject();

        #endregion

        #region Clone

        /// <summary>
        /// Clone this energy source.
        /// </summary>
        public I18NString Clone

            => new I18NString(i18NStrings.SafeSelect(i18n => new I18NPair(i18n.Key, new String(i18n.Value.ToCharArray()))));

        #endregion


        public Boolean Is(Languages  Language,
                          String     Value)
        {

            if (!i18NStrings.ContainsKey(Language))
                return false;

            return i18NStrings[Language].Equals(Value);

        }

        public Boolean IsNot(Languages  Language,
                             String     Value)
        {

            if (!i18NStrings.ContainsKey(Language))
                return true;

            return !i18NStrings[Language].Equals(Value);

        }


        public Boolean Matches(String   Match,
                               Boolean  IgnoreCase  = false)

            => i18NStrings.Any(kvp => IgnoreCase
                                          ? kvp.Value.IndexOf(Match, StringComparison.OrdinalIgnoreCase) >= 0
                                          : kvp.Value.IndexOf(Match) >= 0);


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
        public static Boolean operator == (I18NString I18NString1, I18NString I18NString2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(I18NString1, I18NString2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) I18NString1 == null) || ((Object) I18NString2 == null))
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
        public static Boolean operator != (I18NString I18NString1, I18NString I18NString2)
        {
            return !(I18NString1 == I18NString2);
        }

        #endregion

        #endregion

        #region IEquatable<I18NString> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is I18NString i18NString &&
                  Equals(i18NString);

        #endregion

        #region Equals(I18NString)

        /// <summary>
        /// Compares two I18NString for equality.
        /// </summary>
        /// <param name="i18NString">An I18NString to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(I18NString i18NString)
        {

            if (!(i18NString is I18NString))
                return false;

            if (i18NStrings.Count != i18NString.Count())
                return false;

            foreach (var I18N in i18NStrings)
            {
                if (I18N.Value != i18NString[I18N.Key])
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
