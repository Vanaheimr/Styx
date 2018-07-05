/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extention methods for internationalized (I18N) multilingual texts/strings.
    /// </summary>
    public static class I18NStringExtentions
    {

        /// <summary>
        /// The multilingual string is empty.
        /// </summary>
        public static Boolean IsNullOrEmpty(this I18NString Text)
            => Text == null || !Text.Any();

        /// <summary>
        /// The multilingual string is neither null nor empty.
        /// </summary>
        public static Boolean IsNeitherNullNorEmpty(this I18NString Text)
            => Text != null && Text.Any();

        /// <summary>
        /// Return the first string of a multilingual string.
        /// </summary>
        public static String FirstText(this I18NString Text)
            => Text.Any()
                   ? Text.First().Text
                   : null;

    }

    /// <summary>
    /// An internationalized (I18N) multilingual text/string.
    /// </summary>
    public class I18NString : IEquatable<I18NString>,
                              IEnumerable<I18NPair>
    {

        #region Data

        private readonly Dictionary<Languages, String> I18NStrings;

        #endregion

        #region Constructor(s)

        #region (private) I18NString()

        /// <summary>
        /// Create a new internationalized (I18N) multilingual string.
        /// </summary>
        private I18NString()
        {
            this.I18NStrings = new Dictionary<Languages, String>();
        }

        #endregion

        #region I18NString(Language, Text)

        /// <summary>
        /// Create a new internationalized (I18N) multilingual string
        /// based on the given language and string.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        /// <param name="Text">The internationalized (I18N) text.</param>
        public I18NString(Languages Language, String Text)
            : this()
        {
            I18NStrings.Add(Language, Text);
        }

        #endregion

        #region I18NString(Texts)

        /// <summary>
        /// Create a new internationalized (I18N) multilingual string
        /// based on the given language and string pairs.
        /// </summary>
        public I18NString(KeyValuePair<Languages, String>[] Texts)
            : this()
        {

            if (Texts != null)
                foreach (var Text in Texts)
                    I18NStrings.Add(Text.Key, Text.Value);

        }

        #endregion

        #region I18NString(I18NPairs)

        /// <summary>
        /// Create a new internationalized (I18N) multilingual string
        /// based on the given I18N-pairs.
        /// </summary>
        public I18NString(IEnumerable<I18NPair> I18NPairs)
            : this()
        {

            if (I18NPairs != null)
                foreach (var Text in I18NPairs)
                    I18NStrings.Add(Text.Language, Text.Text);

        }

        #endregion

        #region I18NString(params I18NPairs)

        /// <summary>
        /// Create a new internationalized (I18N) multilingual string
        /// based on the given I18N-pairs.
        /// </summary>
        public I18NString(params I18NPair[] I18NPairs)
            : this()
        {

            if (I18NPairs != null)
                foreach (var Text in I18NPairs)
                    I18NStrings.Add(Text.Language, Text.Text);

        }

        #endregion

        #endregion


        #region (static) Create(Language, Text)

        /// <summary>
        /// Create a new internationalized (I18N) multilingual string
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
        /// Create an empty internationalized (I18N) multilingual string.
        /// </summary>
        public static I18NString Empty
            => new I18NString();

        #endregion

        #region Add(Language, Text)

        /// <summary>
        /// Add a new language-text-pair to the given
        /// internationalized (I18N) multilingual string.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        /// <param name="Text">The internationalized (I18N) text.</param>
        public I18NString Add(Languages  Language,
                              String     Text)
        {

            if (!I18NStrings.ContainsKey(Language))
                I18NStrings.Add(Language, Text);

            else
                I18NStrings[Language] = Text;

            return this;

        }

        #endregion

        #region Add(I18NPair)

        /// <summary>
        /// Add a new language-text-pair to the given
        /// internationalized (I18N) multilingual string.
        /// </summary>
        /// <param name="I18NPair">The internationalized (I18N) text.</param>
        public I18NString Add(I18NPair I18NPair)
        {

            if (!I18NStrings.ContainsKey(I18NPair.Language))
                I18NStrings.Add(I18NPair.Language, I18NPair.Text);

            else
                I18NStrings[I18NPair.Language] = I18NPair.Text;

            return this;

        }

        #endregion

        #region has(Language)

        /// <summary>
        /// Checks if the given language representation exists.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        public Boolean has(Languages Language)

            => I18NStrings.ContainsKey(Language);

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

                String Text;

                if (I18NStrings.TryGetValue(Language, out Text))
                    return Text;

                return String.Empty;

            }

            set
            {
                I18NStrings[Language] = value;
            }

        }

        #endregion

        #region Remove(Language)

        /// <summary>
        /// Remove the given language from the internationalized (I18N) multilingual text.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        public I18NString Remove(Languages Language)
        {

            if (I18NStrings.ContainsKey(Language))
                I18NStrings.Remove(Language);

            return this;

        }

        #endregion


        public Boolean Is(Languages  Language,
                          String     Value)
        {

            if (!I18NStrings.ContainsKey(Language))
                return false;

            return I18NStrings[Language].Equals(Value);

        }

        public Boolean IsNot(Languages  Language,
                             String     Value)
        {

            if (!I18NStrings.ContainsKey(Language))
                return true;

            return !I18NStrings[Language].Equals(Value);

        }



        #region GetEnumerator()

        /// <summary>
        /// Enumerate all internationalized (I18N) texts.
        /// </summary>
        public IEnumerator<I18NPair> GetEnumerator()
            => I18NStrings.Select(kvp => new I18NPair(kvp.Key, kvp.Value)).GetEnumerator();

        /// <summary>
        /// Enumerate all internationalized (I18N) texts.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => I18NStrings.Select(kvp => new I18NPair(kvp.Key, kvp.Value)).GetEnumerator();

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
        {

            if (Object == null)
                return false;

            var I18NString = Object as I18NString;
            if ((Object) I18NString == null)
                return false;

            return Equals(I18NString);

        }

        #endregion

        #region Equals(I18NString)

        /// <summary>
        /// Compares two I18NString for equality.
        /// </summary>
        /// <param name="I18NString">An I18NString to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(I18NString I18NString)
        {

            if ((Object) I18NString == null)
                return false;

            if (I18NStrings.Count != I18NString.Count())
                return false;

            foreach (var I18N in I18NStrings)
            {
                if (I18N.Value != I18NString[I18N.Key])
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

            Int32 ReturnValue = 0;

            foreach (var Value in I18NStrings.
                                      Select(I18N => I18N.Key.GetHashCode() ^ I18N.Value.GetHashCode()))
            {
                ReturnValue ^= Value;
            }

            return ReturnValue;

        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {

            if (I18NStrings.Count == 0)
                return String.Empty;

            return I18NStrings.
                       Select(I18N => I18N.Key.ToString() + ": " + I18N.Value).
                       AggregateWith("; ");

        }

        #endregion

    }

}
