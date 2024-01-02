/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An internationalized (I18N) language text pair.
    /// </summary>
    public readonly struct I18NPair : IEquatable<I18NPair>
    {

        #region Properties

        /// <summary>
        /// The internationalized (I18N) language.
        /// </summary>
        public Languages  Language   { get; }

        /// <summary>
        /// The internationalized (I18N) text.
        /// </summary>
        public String     Text       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new internationalized (I18N) language text pair.
        /// </summary>
        /// <param name="Language">The internationalized (I18N) language.</param>
        /// <param name="Text">The internationalized (I18N) text.</param>
        public I18NPair(Languages Language,
                        String    Text)
        {
            this.Language  = Language;
            this.Text      = Text;
        }

        #endregion


        #region Deconstruct(out Language, out Text)

        public void Deconstruct(out Languages Language, out String Text)
        {
            Language = this.Language;
            Text     = this.Text;
        }

        #endregion


        #region Operator overloading

        #region Operator == (I18NPair1, I18NPair2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="I18NPair1">An internationalized (I18N) language text pair.</param>
        /// <param name="I18NPair2">Another internationalized (I18N) language text pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (I18NPair I18NPair1,
                                           I18NPair I18NPair2)

            => I18NPair1.Equals(I18NPair2);

        #endregion

        #region Operator != (I18NPair1, I18NPair2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="I18NPair1">An internationalized (I18N) language text pair.</param>
        /// <param name="I18NPair2">Another internationalized (I18N) language text pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (I18NPair I18NPair1,
                                           I18NPair I18NPair2)

            => !I18NPair1.Equals(I18NPair2);

        #endregion

        #endregion

        #region IEquatable<I18NPair> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is I18NPair i18NPair &&
                   Equals(i18NPair);

        #endregion

        #region Equals(I8NString)

        /// <summary>
        /// Compares two I18NPair for equality.
        /// </summary>
        /// <param name="I18NPair">An I18NPair to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(I18NPair I18NPair)

            => Language.Equals(I18NPair.Language) &&
               String.Equals(Text,
                             I18NPair.Text,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Language.GetHashCode() ^
               Text.    GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("[", Language, "] ", Text);

        #endregion

    }

}
