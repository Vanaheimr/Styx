/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Text;
using System.Runtime.CompilerServices;
using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the String class.
    /// </summary>
    public static class StringExtensions
    {

        #region IsNullOrEmpty(GivenString)

        /// <summary>
        /// Indicates whether the given (trimmed) string is null or empty.
        /// </summary>
        /// <param name="GivenString">A string.</param>
        public static Boolean IsNullOrEmpty(this String? GivenString)

            => String.IsNullOrEmpty(GivenString?.Trim());

        #endregion

        #region IsNotNullOrEmpty(GivenString)

        /// <summary>
        /// Indicates whether the given (trimmed) string is not null or empty.
        /// </summary>
        /// <param name="GivenString">A string.</param>
        public static Boolean IsNotNullOrEmpty(this String? GivenString)

            => !String.IsNullOrEmpty(GivenString?.Trim());

        #endregion

        #region IfNotNullOrEmpty(GivenString, Mapper, DefaultValue)

        /// <summary>
        /// Mappes the given string if it is not null or empty, or returns the default value.
        /// </summary>
        /// <param name="GivenString">A string.</param>
        /// <param name="Mapper">A string mapper delegate.</param>
        /// <param name="DefaultValue">A default string value.</param>
        public static String IfNotNullOrEmpty(this String?          GivenString,
                                              Func<String, String>  Mapper,
                                              String                DefaultValue)

            => String.IsNullOrEmpty(GivenString?.Trim()) || Mapper is null
                   ? DefaultValue
                   : Mapper(GivenString);

        #endregion


        #region IsNullOrWhiteSpace(GivenString)

        /// <summary>
        /// Indicates whether the given string is null, empty,
        /// or consists only of white-space characters.
        /// </summary>
        /// <param name="GivenString">A string.</param>
        public static Boolean IsNullOrWhiteSpace(this String? GivenString)

            => String.IsNullOrWhiteSpace(GivenString);

        #endregion


        #region ToBase64   (this Text)

        public static String ToBase64(this String Text)
        {

            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(Text));
            }

            catch (Exception e)
            {
                throw new Exception("Error in ToBase64" + e.Message);
            }

        }

        #endregion

        #region ToBase64   (this Bytes)

        public static String ToBase64(this Byte[] Bytes)
        {

            try
            {
                return Convert.ToBase64String(Bytes);
            }

            catch (Exception e)
            {
                throw new Exception("Error in ToBase64" + e.Message);
            }

        }

        #endregion

        #region FromBase64_UTF8(this Text)

        public static String FromBase64_UTF8(this String Text)
        {

            try
            {

                var utf8Decoder  = new UTF8Encoding().GetDecoder();
                var bytes        = Convert.FromBase64String(Text);
                var decodedChars = new Char[utf8Decoder.GetCharCount(bytes, 0, bytes.Length)];
                utf8Decoder.GetChars(bytes, 0, bytes.Length, decodedChars, 0);

                return new String(decodedChars);

            }

            catch (Exception e)
            {
                throw new Exception("Exception in FromBase64(...): " + e.Message);
            }

        }

        #endregion

        #region TryBase64Decode_UTF8(this Text, out Decoded)

        public static Boolean TryBase64Decode_UTF8(this String Text, out String Decoded)
        {

            try
            {

                var utf8Decoder   = new UTF8Encoding().GetDecoder();
                var bytes         = Convert.FromBase64String(Text);
                var decodedChars  = new Char[utf8Decoder.GetCharCount(bytes, 0, bytes.Length)];
                utf8Decoder.GetChars(bytes, 0, bytes.Length, decodedChars, 0);

                Decoded           =  new String(decodedChars);
                return true;

            }
            catch
            { }

            Decoded = default;
            return false;

        }

        #endregion


        #region FromBase64             (this Text)

        public static Byte[] FromBase64(this String Text)

            => Convert.FromBase64String(Text);

        #endregion

        #region TryBase64DecodeToByteArray(this Text, out Decoded)

        public static Boolean TryBase64DecodeToByteArray(this String Text, out Byte[] Decoded)
        {

            try
            {
                Decoded = Convert.FromBase64String(Text);
                return true;
            }
            catch
            { }

            Decoded = default;
            return false;

        }

        #endregion


        #region EscapeForXMLandHTML(Text)

        public static String EscapeForXMLandHTML(this String Text)

            => Text?.Replace("<", "&lt;")?.
                     Replace(">", "&gt;")?.
                     Replace("&", "&amp;");

        #endregion

        #region ToUTF8String(this ArrayOfBytes, NumberOfBytes = -1)

        public static String ToUTF8String(this Byte[]  ArrayOfBytes,
                                          Int32        NumberOfBytes = -1)

            => ArrayOfBytes is null || ArrayOfBytes.Length == 0
                   ? String.Empty
                   : Encoding.UTF8.GetString(ArrayOfBytes,
                                             0,
                                             NumberOfBytes > -1
                                                 ? NumberOfBytes
                                                 : ArrayOfBytes.Length);

        #endregion

        #region ToUTF8String(this MemoryStream, NumberOfBytes = -1)

        public static String ToUTF8String(this MemoryStream MemoryStream, Int32 NumberOfBytes = -1)

             => MemoryStream is null || MemoryStream.Length == 0
                    ? String.Empty
                    : Encoding.UTF8.GetString(MemoryStream.ToArray(),
                                              0,
                                              NumberOfBytes > -1
                                                  ? NumberOfBytes
                                                  : (Int32) MemoryStream.Length);

        #endregion

        #region ToUTF8Bytes(this Text)

        public static Byte[] ToUTF8Bytes(this String? Text)
        {

            if (Text is null)
                return Array.Empty<Byte>();

            return Encoding.UTF8.GetBytes(Text);

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
        public static Boolean IsNotNullAndContains(this String?  String,
                                                   String        Substring)
        {

            if (String is null || Substring is null)
                return false;

            return String.Contains(Substring);

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

        #region Reverse(this String)

        /// <summary>
        /// Reverse the given string.
        /// </summary>
        /// <param name="String">The string to reverse.</param>
        public static String Reverse(this String String)
        {

            if (String.IsNullOrEmpty())
                return String;

            return new String(String.ToCharArray().Reverse().ToArray());

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

        #region RemoveAllBefore(this String, Substring)

        /// <summary>
        /// Removes everything from the string before the given substring.
        /// </summary>
        /// <param name="String">A string.</param>
        /// <param name="Substring">A substring.</param>
        public static String RemoveAllBefore(this String String, String Substring)
        {
            return String.Remove(0, String.IndexOf(Substring) + Substring.Length);
        }

        #endregion

        #region RemoveAllAfter(this String, Substring)

        /// <summary>
        /// Removes everything from the string after the given substring.
        /// </summary>
        /// <param name="String">A string.</param>
        /// <param name="Substring">A substring.</param>
        public static String RemoveAllAfter(this String String, String Substring)
        {
            return String.Remove(String.IndexOf(Substring));
        }

        #endregion

        #region LastIndexOfOrMax(this Text, Pattern)

        public static Int32 LastIndexOfOrMax(this String Text, String Pattern)
        {

            var Index = Text.LastIndexOf(Pattern);

            if (Index < 0)
                return Text.Length;

            else
                return Index;

        }

        #endregion

        #region SubstringMax(this Text, Length)

        /// <summary>
        /// Return a substring of the given maximum length.
        /// </summary>
        /// <param name="Text">A text.</param>
        /// <param name="Length">The maximum length of the substring.</param>
        public static String SubstringMax(this String Text, UInt32 Length)
        {

            if (Text is null)
                return "";

            Text = Text.Trim();

            return Text.IsNotNullOrEmpty()
                       ? Text[..(Int32) Math.Min((UInt32) Text.Length, Length)]
                       : "";

        }

        #endregion

        #region SubTokens(this Text, Length)

        public static IEnumerable<String> SubTokens(this String Text, UInt16 Length)
        {

            var TextCharacterEnumerator = Text.ToCharArray().ToList().GetEnumerator();
            var Characters              = new List<Char>();

            while (TextCharacterEnumerator.MoveNext())
            {

                Characters.Add(TextCharacterEnumerator.Current);

                if (Characters.Count == Length)
                {
                    yield return new String(Characters.ToArray());
                    Characters.Clear();
                }

            }

            if (Characters.Count > 0)
                yield return new String(Characters.ToArray());

        }

        #endregion

        #region ReplacePrefix(this Text, Prefix, Replacement)

        public static String ReplacePrefix(this String  Text,
                                           String       Prefix,
                                           String       Replacement)
        {

            if (Text.StartsWith(Prefix, StringComparison.Ordinal))
                return String.Concat(Replacement, Text.AsSpan(Prefix.Length));

            return Text;

        }

        #endregion

        //#region RemoveLastSlash(this Text)

        //public static String RemoveLastSlash(this String Text)
        //{

        //    if (Text[Text.Length - 1] == '/')
        //        return Text.Substring(0, Text.Length - 1);

        //    return Text;

        //}

        //#endregion

        #region CommonPrefix(this Texts)

        public static String CommonPrefix(this IEnumerable<String> Texts)
        {

            if (Texts is null || !Texts.Any())
                return "";

            if (Texts.Count() == 1)
                return Texts.First();

            var texts          = Texts.Where (text => !(text is null)).ToArray();
            var shortedLength  = Texts.Select(text => text.Length).    Min();
            var pos            = 0;

            while (texts.All(text => text[pos] == texts.First()[pos]))
                pos++;

            return texts.First().Substring(0, pos);

        }

        #endregion



        #region IsNullOrEmpty(GivenString, Delegate, [CallerMemberName] ParameterName = "")

        /// <summary>
        /// Call the given delegate whether the specified string is null or empty.
        /// </summary>
        /// <param name="GivenString">The string.</param>
        /// <param name="Delegate">A delegate to call whenever the given string is null or empty.</param>
        /// <param name="ParameterName">The parameter name of the given string (CallerMemberName).</param>
        public static void IsNullOrEmpty(this String                GivenString,
                                         Action<String>             Delegate,
                                         [CallerMemberName] String  ParameterName = "")
        {

            var _Delegate = Delegate;

            if (String.IsNullOrEmpty(GivenString) && _Delegate != null)
                _Delegate(GivenString);

        }

        #endregion

        #region IsNotNullOrEmpty(GivenString, Delegate, [CallerMemberName] ParameterName = "")

        /// <summary>
        /// Call the given delegate whether the specified string is not null or empty.
        /// </summary>
        /// <param name="GivenString">The string.</param>
        /// <param name="Delegate">A delegate to call whenever the given string is null or empty.</param>
        /// <param name="ParameterName">The parameter name of the given string (CallerMemberName).</param>
        public static void IsNotNullOrEmpty(this String                GivenString,
                                            Action<String>             Delegate,
                                            [CallerMemberName] String  ParameterName = "")
        {

            var _Delegate = Delegate;

            if (!String.IsNullOrEmpty(GivenString) && _Delegate != null)
                _Delegate(GivenString);

        }

        #endregion

        #region FailIfNullOrEmpty(GivenString, ExceptionMessage = null, [CallerMemberName] ParameterName = "")

        /// <summary>
        /// Throws an ArgumentNullException whenever the given string is null or empty.
        /// </summary>
        /// <param name="GivenString">The string.</param>
        /// <param name="ExceptionMessage">An optional message to be added to the exception.</param>
        /// <param name="ParameterName">The parameter name of the given string (CallerMemberName).</param>
        public static void FailIfNullOrEmpty(this String                GivenString,
                                             String                     ExceptionMessage = null,
                                             [CallerMemberName] String  ParameterName    = "")
        {

            if (String.IsNullOrEmpty(GivenString))
            {

                if (String.IsNullOrEmpty(ExceptionMessage))
                    throw new ArgumentNullException(ParameterName);

                else
                    throw new ArgumentNullException(ParameterName, ExceptionMessage);

            }

        }

        #endregion

        #region FailIfNotNullOrEmpty(GivenString, ExceptionMessage = null, [CallerMemberName] ParameterName = "")

        /// <summary>
        /// Throws an ArgumentNullException whenever the given string is not null or empty.
        /// </summary>
        /// <param name="GivenString">The string.</param>
        /// <param name="ExceptionMessage">An optional message to be added to the exception.</param>
        /// <param name="ParameterName">The parameter name of the given string (CallerMemberName).</param>
        public static void FailIfNotNullOrEmpty(this String                GivenString,
                                                String                     ExceptionMessage = null,
                                                [CallerMemberName] String  ParameterName    = "")
        {

            if (!String.IsNullOrEmpty(GivenString))
            {

                if (String.IsNullOrEmpty(ExceptionMessage))
                    throw new ArgumentNullException(ParameterName, "The given parameter must not be null or empty!");

                else
                    throw new ArgumentNullException(ParameterName, ExceptionMessage);

            }

        }

        #endregion

        #region ToKeyValuePairs(Text)

        /// <summary>
        /// Converts the given enumeration of strings into an enumeration of key-value-pairs.
        /// </summary>
        /// <param name="Text">An enumeration of strings.</param>
        public static IEnumerable<String> AggregateIndentedLines(this IEnumerable<String> Text)
        {

            #region Initial checks

            if (Text == null)
                yield break;

            #endregion

            var Enumerator   = Text.GetEnumerator();
            var CurrentLine  = String.Empty;

            while (Enumerator.MoveNext())
            {

                if (Enumerator.Current.StartsWith(" ") || Enumerator.Current.StartsWith("\t"))
                    CurrentLine = CurrentLine + Enumerator.Current.TrimStart();

                else
                {

                    if (CurrentLine != String.Empty)
                        yield return CurrentLine;

                    CurrentLine = Enumerator.Current;

                }

            }

            yield return CurrentLine;

        }

        #endregion

        #region ToKeyValuePairs(Text, Delimiters)

        /// <summary>
        /// Converts the given enumeration of strings into an enumeration of key-value-pairs.
        /// </summary>
        /// <param name="Text">An enumeration of strings.</param>
        /// <param name="Delimiters">The delimiter(s) between keys and values.</param>
        public static IEnumerable<KeyValuePair<String, String>> ToKeyValuePairs(this IEnumerable<String>  Text,
                                                                                params Char[]             Delimiters)
        {

            #region Initial checks

            if (Text == null)
                yield break;

            if (Delimiters == null || Delimiters.Length == 0)
                throw new ArgumentNullException("The given delimiter must not be null or empty!");

            String[] Tokens;

            #endregion

            foreach (var line in Text)
            {

                Tokens = line.Split(Delimiters, 2);

                if (Tokens.Length == 2)
                    yield return new KeyValuePair<String, String>(Tokens[0].Trim(), Tokens[1].Trim());

            }

        }

        #endregion


        #region DoubleSplit(this Text, FirstSeparator, SecondSeparator)

        public static Dictionary<String, String> DoubleSplit(this String  Text,
                                                             Char         FirstSeparator,
                                                             Char         SecondSeparator)
        {

            var Dictionary = new Dictionary<String, String>();

            Text = Text?.Trim();

            if (Text.IsNullOrEmpty())
                return Dictionary;

            Text.Split     (FirstSeparator).
                 SafeSelect(command => command.Split(SecondSeparator)).
                 ForEach   (tuple   => {

                                           if (tuple.Length == 1)
                                           {
                                               if (tuple[0] != null)
                                               {
                                                   if (!Dictionary.ContainsKey(tuple[0].Trim()))
                                                       Dictionary.Add(tuple[0].Trim(), "");
                                                   else
                                                       Dictionary[tuple[0].Trim()] = "";
                                               }
                                           }

                                           else if (tuple.Length == 2)
                                           {
                                               if (tuple[0] != null)
                                               {
                                                   if (!Dictionary.ContainsKey(tuple[0].Trim()))
                                                       Dictionary.Add(tuple[0].Trim(), tuple[1]);
                                                   else
                                                       Dictionary[tuple[0].Trim()] = tuple[1];
                                               }
                                           }

                                       });

            return Dictionary;

        }

        #endregion

        #region DoubleSplitInto(this Text, FirstSeparator, SecondSeparator, Dictionary)

        public static Dictionary<String, String> DoubleSplitInto(this String                 Text,
                                                                 Char                        FirstSeparator,
                                                                 Char                        SecondSeparator,
                                                                 Dictionary<String, String>  Dictionary)
        {

            if (Text.IsNullOrEmpty())
                return Dictionary;

            Text.Split(FirstSeparator).
                 SafeSelect(command => command.Split(SecondSeparator)).
                 ForEach   (tuple   => {

                                           if (tuple.Length == 1)
                                           {
                                               if (!Dictionary.ContainsKey(tuple[0]))
                                                   Dictionary.Add(tuple[0], "");
                                               else
                                                   Dictionary[tuple[0]] = "";
                                           }

                                           if (tuple.Length == 2)
                                           {
                                               if (!Dictionary.ContainsKey(tuple[0]))
                                                   Dictionary.Add(tuple[0], tuple[1]);
                                               else
                                                   Dictionary[tuple[0]] = tuple[1];
                                           }

                                       });

            return Dictionary;

        }

        #endregion



        #region ContainsIgnoreCase(Strings, Text)

        /// <summary>
        /// Determines whether a sequence contains a specified element by using the case-insensitive equality comparer.
        /// </summary>
        /// <param name="Strings">An enumeration of strings.</param>
        /// <param name="Text">The text to search for.</param>
        public static Boolean ContainsIgnoreCase(this IEnumerable<String>  Strings,
                                                 String                    Text)
        {

            if (Strings == null)
                return false;

            foreach (var text in Strings)
            {
                if (text.Equals(Text, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;

        }

        #endregion

    }

}
