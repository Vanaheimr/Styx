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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An internationalized (I18N) language text pair.
    /// </summary>
    public struct I18NPair : IEquatable<I18NPair>
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


        #region IEquatable<I18NPair> Members

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

            // Check if the given object is an I18NPair.
            var I18NPair = (I18NPair) Object;
            if ((Object) I18NPair == null)
                return false;

            return this.Equals(I18NPair);

        }

        #endregion

        #region Equals(I8NString)

        /// <summary>
        /// Compares two I18NPair for equality.
        /// </summary>
        /// <param name="I18NPair">An I18NPair to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(I18NPair I18NPair)
        {

            if ((Object) I18NPair == null)
                return false;

            if (I18NPair.Language != Language)
                return false;

            if (I18NPair.Text     != Text)
                return false;

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
            return Language.GetHashCode() ^ Text.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return "[" + Language + "] " + Text;
        }

        #endregion

    }

}
