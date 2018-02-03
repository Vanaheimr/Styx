/*
 * Copyright (c) 2010-2012 Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Illias Commons <http://www.github.com/ahzf/Illias>
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
using System.Text;

#endregion

namespace de.ahzf.Illias.Commons
{

    /// <summary>
    /// Extensions to the String class.
    /// </summary>
    public static class StringExtensions
    {

        #region IsNullOrEmpty

        public static Boolean IsNullOrEmpty(this String myString)
        {
            return String.IsNullOrEmpty(myString);
        }

        #endregion

        #region ToBase64(myString)

        public static String ToBase64(this String myString)
        {

            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(myString));
            }

            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }

        }

        #endregion

        #region FromBase64(myBase64String)

        public static String FromBase64(this String myBase64String)
        {

            try
            {

                var _UTF8Decoder  = new UTF8Encoding().GetDecoder();
                var _Bytes        = Convert.FromBase64String(myBase64String);
                var _DecodedChars = new Char[_UTF8Decoder.GetCharCount(_Bytes, 0, _Bytes.Length)];
                _UTF8Decoder.GetChars(_Bytes, 0, _Bytes.Length, _DecodedChars, 0);

                return new String(_DecodedChars);

            }

            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }

        }

        #endregion

        #region EscapeForXMLandHTML(myString)

        public static String EscapeForXMLandHTML(this String myString)
        {

            if (myString == null)
                throw new ArgumentNullException("myString must not be null!");

            myString = myString.Replace("<", "&lt;");
            myString = myString.Replace(">", "&gt;");
            myString = myString.Replace("&", "&amp;");

            return myString;

        }

        #endregion

        #region ToUTF8String(this myByteArray, NumberOfBytes = 0)

        public static String ToUTF8String(this Byte[] myByteArray, Int32 NumberOfBytes = 0)
        {

            if (myByteArray == null)
                throw new ArgumentNullException("myString must not be null!");

            if (myByteArray.Length == 0)
                return String.Empty;

#if !SILVERLIGHT
            if (NumberOfBytes == 0)
                return Encoding.UTF8.GetString(myByteArray);
            else
#endif
                return Encoding.UTF8.GetString(myByteArray, 0, NumberOfBytes);

        }

        #endregion

        #region ToUTF8Bytes(this myString)

        public static Byte[] ToUTF8Bytes(this String myString)
        {

            if (myString == null)
                throw new ArgumentNullException("myString must not be null!");

            return Encoding.UTF8.GetBytes(myString);

        }

        #endregion

        #region IsNotNullAndContains(this String, Substring)

        /// <summary>
        /// Returns a value indicating whether the specified Substring
        /// occurs within the given string.
        /// </summary>
        /// <param name="String">A string.</param>
        /// <param name="Substring">A substring to search for.</param>
        /// <returns>True if the value parameter occurs within this string.</returns>
        public static Boolean IsNotNullAndContains(this String String, String Substring)
        {

            if (String != null)
                return String.Contains(Substring);

            return false;

        }

        #endregion

        #region DoubleNewLine

        /// <summary>
        /// NewLine but twice.
        /// </summary>
        public static String DoubleNewLine
        {
            get
            {
                return Environment.NewLine + Environment.NewLine;
            }
        }

        #endregion

        #region RemoveQuotes(this String)

        /// <summary>
        /// Removes leading and/or tailing (double) quotes.
        /// </summary>
        /// <param name="String">The string to check.</param>
        public static String RemoveQuotes(this String String)
        {

            var Length       = String.Length;
            var LeadingQuote = String.StartsWith("\"") || String.StartsWith("\'");
            var TailingQuote = String.EndsWith("\"")   || String.EndsWith("\'");

            if (!LeadingQuote && !TailingQuote)
                return String;

            if (LeadingQuote && TailingQuote && Length > 2)
                return String.Substring(1, Length - 1);

            if (LeadingQuote && Length > 1)
                return String.Substring(1, Length);

            if (TailingQuote && Length > 1)
                return String.Substring(0, Length - 1);

            return String.Empty;

        }

        #endregion

    }

}
