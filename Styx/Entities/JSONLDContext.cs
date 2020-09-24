/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
    /// A JSON-LD context.
    /// </summary>
    public readonly struct JSONLDContext : IId<JSONLDContext>
    {

        #region Data

        /// <summary>
        /// The internal JSON-LD context.
        /// </summary>
        private readonly String InternalContext;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this JSON-LD context is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalContext.IsNullOrEmpty();

        /// <summary>
        /// The length of the JSON-LD context.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalContext?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new JSON-LD context based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the JSON-LD context.</param>
        private JSONLDContext(String String)
        {
            this.InternalContext  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a JSON-LD context.
        /// </summary>
        /// <param name="Text">A text representation of a JSON-LD context.</param>
        public static JSONLDContext Parse(String Text)
        {

            if (TryParse(Text, out JSONLDContext contextId))
                return contextId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a JSON-LD context must not be null or empty!");

            throw new ArgumentException("The given text representation of a JSON-LD context is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a JSON-LD context.
        /// </summary>
        /// <param name="Text">A text representation of a JSON-LD context.</param>
        public static JSONLDContext? TryParse(String Text)
        {

            if (TryParse(Text, out JSONLDContext contextId))
                return contextId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out JSONLDContext)

        /// <summary>
        /// Try to parse the given text as a JSON-LD context.
        /// </summary>
        /// <param name="Text">A text representation of a JSON-LD context.</param>
        /// <param name="JSONLDContext">The parsed JSON-LD context.</param>
        public static Boolean TryParse(String Text, out JSONLDContext JSONLDContext)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    JSONLDContext = new JSONLDContext(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            JSONLDContext = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this JSON-LD context.
        /// </summary>
        public JSONLDContext Clone

            => new JSONLDContext(
                   new String(InternalContext?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (JSONLDContext1, JSONLDContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="JSONLDContext1">A JSON-LD context.</param>
        /// <param name="JSONLDContext2">Another JSON-LD context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (JSONLDContext JSONLDContext1,
                                           JSONLDContext JSONLDContext2)

            => JSONLDContext1.Equals(JSONLDContext2);

        #endregion

        #region Operator != (JSONLDContext1, JSONLDContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="JSONLDContext1">A JSON-LD context.</param>
        /// <param name="JSONLDContext2">Another JSON-LD context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (JSONLDContext JSONLDContext1,
                                           JSONLDContext JSONLDContext2)

            => !(JSONLDContext1 == JSONLDContext2);

        #endregion

        #region Operator <  (JSONLDContext1, JSONLDContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="JSONLDContext1">A JSON-LD context.</param>
        /// <param name="JSONLDContext2">Another JSON-LD context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (JSONLDContext JSONLDContext1,
                                          JSONLDContext JSONLDContext2)

            => JSONLDContext1.CompareTo(JSONLDContext2) < 0;

        #endregion

        #region Operator <= (JSONLDContext1, JSONLDContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="JSONLDContext1">A JSON-LD context.</param>
        /// <param name="JSONLDContext2">Another JSON-LD context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (JSONLDContext JSONLDContext1,
                                           JSONLDContext JSONLDContext2)

            => !(JSONLDContext1 > JSONLDContext2);

        #endregion

        #region Operator >  (JSONLDContext1, JSONLDContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="JSONLDContext1">A JSON-LD context.</param>
        /// <param name="JSONLDContext2">Another JSON-LD context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (JSONLDContext JSONLDContext1,
                                          JSONLDContext JSONLDContext2)

            => JSONLDContext1.CompareTo(JSONLDContext2) > 0;

        #endregion

        #region Operator >= (JSONLDContext1, JSONLDContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="JSONLDContext1">A JSON-LD context.</param>
        /// <param name="JSONLDContext2">Another JSON-LD context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (JSONLDContext JSONLDContext1,
                                           JSONLDContext JSONLDContext2)

            => !(JSONLDContext1 < JSONLDContext2);

        #endregion


        #region Operator +  (JSONLDPrefix, JSONLDSuffix)

        /// <summary>
        /// Combines a JSON-LD prefix and a suffix.
        /// </summary>
        /// <param name="JSONLDPrefix">A JSON-LD context prefix.</param>
        /// <param name="JSONLDSuffix">Another JSON-LD context suffix.</param>
        /// <returns>true|false</returns>
        public static JSONLDContext operator +  (JSONLDContext JSONLDPrefix,
                                                 String        JSONLDSuffix)

            => JSONLDContext.Parse(JSONLDPrefix.ToString() + "/" + JSONLDSuffix.ToString());

        #endregion

        #endregion

        #region IComparable<JSONLDContext> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is JSONLDContext contextId
                   ? CompareTo(contextId)
                   : throw new ArgumentException("The given object is not a JSON-LD context!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(JSONLDContext)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="JSONLDContext">An object to compare with.</param>
        public Int32 CompareTo(JSONLDContext JSONLDContext)

            => String.Compare(InternalContext,
                              JSONLDContext.InternalContext,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<JSONLDContext> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is JSONLDContext contextId &&
                   Equals(contextId);

        #endregion

        #region Equals(JSONLDContext)

        /// <summary>
        /// Compares two JSON-LD contexts for equality.
        /// </summary>
        /// <param name="JSONLDContext">An JSON-LD context to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(JSONLDContext JSONLDContext)

            => String.Equals(InternalContext,
                             JSONLDContext.InternalContext,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalContext?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalContext ?? "";

        #endregion

    }

}
