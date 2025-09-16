/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extensions to the String class.
    /// </summary>
    public static class StringExtensions
    {

        #region IsNullOrEmpty       (GivenString)

        /// <summary>
        /// Indicates whether the given (trimmed) string is null or empty.
        /// </summary>
        /// <param name="GivenString">A string.</param>
        public static Boolean IsNullOrEmpty([NotNullWhen(false)] this String? GivenString)

            => String.IsNullOrEmpty(GivenString?.Trim());

        #endregion

        #region IsNotNullOrEmpty    (GivenString)

        /// <summary>
        /// Indicates whether the given (trimmed) string is not null or empty.
        /// </summary>
        /// <param name="GivenString">A string.</param>
        public static Boolean IsNotNullOrEmpty([NotNullWhen(true)] this String? GivenString)

            => !String.IsNullOrEmpty(GivenString?.Trim());

        #endregion

        #region IfNotNullOrEmpty    (GivenString, Mapper, DefaultValue)

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

        #region IsNullOrWhiteSpace  (GivenString)

        /// <summary>
        /// Indicates whether the given string is null, empty,
        /// or consists only of white-space characters.
        /// </summary>
        /// <param name="GivenString">A string.</param>
        public static Boolean IsNullOrWhiteSpace(this String? GivenString)

            => String.IsNullOrWhiteSpace(GivenString);

        #endregion


        #region CloneString         (GivenString)

        /// <summary>
        /// Clones the given string.
        /// </summary>
        /// <param name="GivenString">A string.</param>
        public static String CloneString(this String GivenString)

               // Strings might in some cases still be null!
            => new (GivenString?.ToCharArray() ?? []);

        #endregion

        #region CloneNullableString (GivenString)

        /// <summary>
        /// Clones the given nullable string.
        /// </summary>
        /// <param name="GivenString">A nullable string.</param>
        public static String? CloneNullableString(this String? GivenString)

            => GivenString is not null
                   ? new String(GivenString.ToCharArray())
                   : null;

        #endregion


        #region ToHexString         (this ByteArray, StartIndex  = 0, Length = null, ToLower = true)

        /// <summary>
        /// Converts an array of bytes into its hexadecimal string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        /// <param name="StartIndex">The zero-based starting byte position of a subsequence in this instance.</param>
        /// <param name="Length">The number of bytes in the subsequence.</param>
        /// <param name="ToLower">Whether to convert the resulting string to lower case.</param>
        public static String ToHexString(this Byte[]  ByteArray,
                                         UInt16       StartIndex  = 0,
                                         UInt16?      Length      = null,
                                         Boolean      ToLower     = true)
        {

            var length = Length ?? ByteArray.Length - StartIndex;

            Byte _byte;
            var  _char  = new Char[length * 2];
            var  end    = StartIndex + length;

            for (Int32 y= StartIndex, x= 0; y<end; ++y, ++x)
            {
                _byte      = (byte) (ByteArray[y] >> 4);
                _char[x]   = (char) (_byte>9 ? _byte+0x37 : _byte+0x30);
                _byte      = (byte) (ByteArray[y] & 0xF);
                _char[++x] = (char) (_byte>9 ? _byte+0x37 : _byte+0x30);
            }

            return ToLower
                ? new String(_char).ToLower()
                : new String(_char);

        }

        #endregion

        #region FromHEX             (this HexString)

        /// <summary>
        /// Convert the given hex representation of a byte array
        /// into an array of bytes.
        /// </summary>
        /// <param name="HexString">hex representation of a byte array.</param>
        public static Byte[] FromHEX(this String HexString)
        {

            if (TryParseHEX(HexString, out var byteArray, out var errorResponse))
                return byteArray;

            throw new ArgumentException(
                      errorResponse,
                      nameof(HexString)
                  );

        }

        #endregion

        #region TryParseHEX         (this HexString,    out ByteArray,  out ErrorResponse)

        // Regular expression for detecting Hex (even number of characters, optional 0x prefix)
        public static readonly Regex HEXRegex = new Regex(@"^(0x)?[a-fA-F0-9]+$");

        /// <summary>
        /// Convert the given hex representation of a byte array
        /// into an array of bytes.
        /// </summary>
        /// <param name="HexString">hex representation of a byte array.</param>
        /// <param name="ByteArray">The parsed array of bytes.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseHEX(this String                       HexString,
                                          [NotNullWhen(true)]  out Byte[]?  ByteArray,
                                          [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                var hexString = HexString.Trim().
                                          Replace(" ", "").
                                          Replace(":", "").
                                          Replace("-", "");

                if (hexString.IsNullOrEmpty())
                {
                    ByteArray = [];
                    return true;
                }

                else if (hexString.Length % 2 == 1)
                    ErrorResponse = $"The length of the given hex-representation '{HexString}' of a byte array is invalid!";

                else if (!HEXRegex.IsMatch(hexString))
                    ErrorResponse = $"The given hex-representation '{HexString}' of a byte array is invalid!";

                else
                {

                    ByteArray = Enumerable.Range  (hexString.StartsWith("0x") ? 2 : 0,
                                                   hexString.Length).
                                           Where  (x => x % 2 == 0).
                                           Select (x => Convert.ToByte(hexString.Substring(x, 2), 16)).
                                           ToArray();

                    return true;

                }

            }
            catch (Exception e)
            {
                ErrorResponse = $"The given string '{HexString}' could not be parsed as a hex-representation of a byte array: " + e.Message;
            }

            ErrorResponse ??= $"The given string '{HexString}' is not a valid hex-representation of a byte array!";
            ByteArray       = [];
            return false;

        }

        #endregion


        #region ToBase32            (this ByteArray)

        /// <summary>
        /// Converts an array of bytes into its Base32 string representation.
        /// </summary>
        /// <param name="Text">An array of bytes.</param>
        public static String ToBase32(this String Text)

            => ToBase32(Text.ToUTF8Bytes().AsSpan());


        /// <summary>
        /// Converts an array of bytes into its Base32 string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static String ToBase32(this Byte[] ByteArray)

            => ToBase32(ByteArray.AsSpan());


        /// <summary>
        /// Converts an array of bytes into its Base32 string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static String ToBase32(this ReadOnlySpan<Byte> ByteArray)
        {

            try
            {

                if (ByteArray.Length == 0)
                    return String.Empty;

                var result          = new StringBuilder((ByteArray.Length + 7) * 8 / 5);

                var currentByte     = 0;
                var digit           = 0;
                var index           = 0;
                var bytesRemaining  = ByteArray.Length;

                while (bytesRemaining > 0)
                {
                    currentByte = ByteArray[index++];
                    bytesRemaining--;

                    result.Append(Base32Chars[(currentByte >> 3) & 31]); // First 5 bits

                    digit = (currentByte & 7) << 2; // Last 3 bits

                    if (bytesRemaining > 0)
                    {
                        currentByte = ByteArray[index];
                        digit |= (currentByte >> 6) & 3; // Next 2 bits

                        result.Append(Base32Chars[digit]); // Append the result
                        result.Append(Base32Chars[(currentByte >> 1) & 31]); // Next 5 bits

                        digit = (currentByte & 1) << 4; // Last bit
                    }

                    if (bytesRemaining > 0)
                    {
                        currentByte = ByteArray[index++];
                        bytesRemaining--;

                        digit |= (currentByte >> 4) & 15; // Next 4 bits

                        result.Append(Base32Chars[digit]); // Append the result
                        result.Append(Base32Chars[(currentByte & 15) << 1]); // Remaining bits

                        digit = (currentByte >> 7) & 1; // Last bit
                    }

                    if (bytesRemaining == 0)
                    {
                        result.Append(Base32Chars[digit]); // Final part of the encoding
                        break;
                    }
                }

                // Add padding to make it a multiple of 8 characters
                int padding = (result.Length % 8 == 0) ? 0 : (8 - result.Length % 8);
                return result.ToString().PadRight(result.Length + padding, '=');

            }
            catch (Exception e)
            {
                throw new Exception("Error in ToBase32" + e.Message);
            }

        }

        #endregion

        #region FromBASE32          (this Base32String)

        public static Byte[] FromBASE32(this String Base32String)
        {

            if (TryParseBASE32(Base32String, out var byteArray, out var errorResponse))
                return byteArray;

            throw new ArgumentException(
                      errorResponse,
                      nameof(Base32String)
                  );

        }

        #endregion

        #region TryParseBASE32      (this Base32String, out ByteArray,  out ErrorResponse)

        // Regular expression for detecting Base32 (standard Base32 without line breaks)
        public static readonly Regex Base32Regex = new Regex(@"^[a-zA-Z2-7]+=*$");

        private static readonly string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static Boolean TryParseBASE32(this String                       Base32String,
                                             [NotNullWhen(true)]  out Byte[]?  ByteArray,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                Base32String   = Base32String.Trim();

                if (Base32String.IsNullOrEmpty())
                    ErrorResponse = "The given base32-representation of a byte array must not be null!";

                else if (Base32String.Length % 8 != 0)
                    ErrorResponse = $"The length of the given base32-representation '{Base32String}' of a byte array is invalid ({Base32String.Length} characters)!";

                else if (!Base32Regex.IsMatch(Base32String))
                    ErrorResponse = $"The given base32-representation '{Base32String}' of a byte array is invalid!";

                else
                {


                    Base32String   = Base32String.TrimEnd('=');     // Remove padding characters

                    var byteCount  = Base32String.Length * 5 / 8;   // Calculate the byte count
                    ByteArray      = new Byte[byteCount];

                    var buffer     = 0;
                    var bitsLeft   = 0;
                    var mask       = 0x1F; // 5 bits mask (Base32 has 32 characters, so 5 bits per char)

                    var index      = 0;
                    foreach (char c in Base32String.ToUpperInvariant())
                    {

                        var charValue = Base32Chars.IndexOf(c);

                        //if (charValue < 0)
                        //{
                        //    ErrorResponse = $"Invalid Base32 character '{charValue}' in {Base32String}:{index}!";
                        //    return false;
                        //}

                        buffer <<= 5;
                        buffer |= charValue & mask;
                        bitsLeft += 5;

                        if (bitsLeft >= 8)
                        {
                            ByteArray[index++] = (byte) ((buffer >> (bitsLeft - 8)) & 0xFF);
                            bitsLeft -= 8;
                        }

                    }

                    return true;

                }

            }
            catch (Exception e)
            {
                ErrorResponse = $"The given string '{Base32String}' could not be parsed as a base32-representation of a byte array: " + e.Message;
            }

            ErrorResponse ??= $"The given string '{Base32String}' is not a base32-representation of a byte array!";
            ByteArray       = [];
            return false;

        }

        #endregion


        #region ToBase45            (this ByteArray)

        private const string Base45Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

        /// <summary>
        /// Converts an array of bytes into its Base45 string representation.
        /// </summary>
        /// <param name="Text">An array of bytes.</param>
        public static String ToBase45(this String Text)

            => ToBase45(Text.ToUTF8Bytes().AsSpan());


        /// <summary>
        /// Converts an array of bytes into its Base45 string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static String ToBase45(this Byte[] ByteArray)

            => ToBase45(ByteArray.AsSpan());


        /// <summary>
        /// Converts an array of bytes into its Base45 string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static String ToBase45(this ReadOnlySpan<Byte> ByteArray)
        {

            if (ByteArray.Length == 0)
                return String.Empty;

            var sb = new StringBuilder(ByteArray.Length / 2 * 3 + ByteArray.Length % 2 * 2);

            var i  = 0;
            while (i + 1 < ByteArray.Length)
            {

                var x = (ByteArray[i] << 8) | ByteArray[i + 1];   // 0..65535
                var c = x % 45;                                   // LSB
                var d = (x / 45) % 45;
                var e = x / (45 * 45);                            // MSB (0..32)

                sb.Append(Base45Alphabet[c]);                     // LSB first
                sb.Append(Base45Alphabet[d]);
                sb.Append(Base45Alphabet[e]);

                i += 2;

            }

            if (i < ByteArray.Length)                             // 1 byte left
            {

                var x = ByteArray[i];                             // 0..255
                var c = x % 45;                                   // LSB
                var d = x / 45;

                sb.Append(Base45Alphabet[c]);                     // LSB first
                sb.Append(Base45Alphabet[d]);

            }

            return sb.ToString();

        }

        #endregion

        #region FromBASE45          (this Base45String)

        public static Byte[] FromBASE45(this String Base45String)
        {

            if (TryParseBASE45(Base45String, out var byteArray, out var errorResponse))
                return byteArray;

            throw new ArgumentException(
                      errorResponse,
                      nameof(Base45String)
                  );

        }

        #endregion

        #region TryParseBASE45      (this Base45String, out ByteArray,  out ErrorResponse)

        /// <summary>
        /// Decodes a BASE45 (RFC 9285) string into a byte array.
        /// Accepts only the exact 45-character alphabet (including SPACE).
        /// Returns false with an error message on any invalid length/character/overflow.
        /// </summary>
        public static Boolean TryParseBASE45(this String                       Base45String,
                                             [NotNullWhen(true)]  out Byte[]?  ByteArray,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ByteArray      = null;
            ErrorResponse  = null;

            var len = Base45String.Length;
            if (len == 0)
            {
                ByteArray = [];
                return true;
            }

            // Valid BASE45 lengths are (len % 3) == 0 or 2. A remainder of 1 is impossible.
            var rem = len % 3;
            if (rem == 1)
            {
                ErrorResponse = $"Invalid BASE45 length {len}: length modulo 3 must be 0 or 2.";
                return false;
            }


            // Build a decode map for ASCII 0..127 => value or -1 if invalid.
            // stackalloc to avoid heap allocations; initialize to -1.
            Span<Int32> map = stackalloc Int32[128];
            for (var i = 0; i < map.Length; i++) map[i] = -1;
            for (var i = 0; i < Base45Alphabet.Length; i++)
            {
                // all chars are ASCII
                map[(Int32) Base45Alphabet[i]] = i;
            }

            // Pre-size the output buffer exactly:
            // 3 chars -> 2 bytes; 2 chars -> 1 byte
            var outLen = (len / 3) * 2 + (rem == 2 ? 1 : 0);
            var output = new Byte[outLen];

            var o = 0;

            // Decode all full triplets
            var tripletEnd = (len / 3) * 3;
            for (var i = 0; i < tripletEnd; i += 3)
            {

                var c0 = Base45String[i];
                var c1 = Base45String[i + 1];
                var c2 = Base45String[i + 2];

                if (c0 > 127 || c1 > 127 || c2 > 127 ||
                    map[c0] < 0 || map[c1] < 0 || map[c2] < 0)
                {

                    var badIndex   = (map[c0] < 0 || c0 > 127) ? i :
                                     (map[c1] < 0 || c1 > 127) ? i + 1 : i + 2;

                    var bad        = Base45String[badIndex];

                    ErrorResponse  = $"Invalid BASE45 character '{bad}' (U+{((Int32) bad):X4}) at index {badIndex}.";

                    return false;

                }

                var x = map[c0] + map[c1] * 45 + map[c2] * 45 * 45; // 0..91124
                if (x > 0xFFFF)
                {
                    ErrorResponse = $"Invalid BASE45 triplet at indexes [{i},{i + 1},{i + 2}]: value {x} exceeds 65535.";
                    return false;
                }

                output[o++] = (Byte) (x >>    8);
                output[o++] = (Byte) (x &  0xFF);

            }

            // Decode a final pair if present
            if (rem == 2)
            {

                var i  = tripletEnd;
                var c0 = Base45String[i];
                var c1 = Base45String[i + 1];

                if (c0 > 127 || c1 > 127 || map[c0] < 0 || map[c1] < 0)
                {
                    var badIndex   = (map[c0] < 0 || c0 > 127) ? i : i + 1;
                    var bad        = Base45String[badIndex];
                    ErrorResponse  = $"Invalid BASE45 character '{bad}' (U+{((Int32) bad):X4}) at index {badIndex}.";
                    return false;
                }

                var x = map[c0] + map[c1] * 45; // 0..2024
                if (x > 0xFF)
                {
                    ErrorResponse = $"Invalid BASE45 pair at indexes [{i},{i + 1}]: value {x} exceeds 255.";
                    return false;
                }

                output[o++] = (Byte) x;
            }

            ByteArray = output;
            return true;

        }

        #endregion


        #region ToBase64            (this ByteArray)

        /// <summary>
        /// Converts an array of bytes into its Base64 string representation.
        /// </summary>
        /// <param name="Text">An array of bytes.</param>
        public static String ToBase64(this String Text)

            => ToBase64(Text.ToUTF8Bytes().AsSpan());


        /// <summary>
        /// Converts an array of bytes into its Base64 string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static String ToBase64(this Byte[] ByteArray)

            => ToBase64(ByteArray.AsSpan());


        /// <summary>
        /// Converts an array of bytes into its Base64 string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static String ToBase64(this ReadOnlySpan<Byte> ByteArray)
        {
            try
            {
                return Convert.ToBase64String(ByteArray);
            }
            catch (Exception e)
            {
                throw new Exception("Error in ToBase64" + e.Message);
            }
        }

        #endregion

        #region FromBASE64          (this Base64String)

        public static Byte[] FromBASE64(this String Base64String)
        {

            if (TryParseBASE64(Base64String, out var byteArray, out var errorResponse))
                return byteArray;

            throw new ArgumentException(
                      errorResponse,
                      nameof(Base64String)
                  );

        }

        #endregion

        #region TryParseBASE64      (this Base64String, out ByteArray,  out ErrorResponse)

        // Regular expression for detecting Base64 (standard Base64 without line breaks)
        public static readonly Regex Base64Regex = new Regex(@"^[A-Za-z0-9+/]+={0,2}$");

        public static Boolean TryParseBASE64(this String                       Base64String,
                                             [NotNullWhen(true)]  out Byte[]?  ByteArray,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                Base64String = Base64String.Trim();

                if (Base64String.IsNullOrEmpty())
                    ErrorResponse = "The given base64-representation of a byte array must not be null!";

                else if (Base64String.Length % 4 != 0)
                    ErrorResponse = $"The length of the given base64-representation '{Base64String}' of a byte array is invalid ({Base64String.Length} characters)!";

                else if (!Base64Regex.IsMatch(Base64String))
                    ErrorResponse = $"The given base64-representation '{Base64String}' of a byte array is invalid!";

                else
                {
                    ByteArray = Convert.FromBase64String(Base64String);
                    return true;
                }

            }
            catch (Exception e)
            {
                ErrorResponse = $"The given string '{Base64String}' could not be parsed as a base64-representation of a byte array: " + e.Message;
            }

            ErrorResponse ??= $"The given string '{Base64String}' is not a base64-representation of a byte array!";
            ByteArray       = [];
            return false;

        }

        #endregion


        #region ToBase64URL         (this ByteArray)

        /// <summary>
        /// Converts an array of bytes into its Base64URL string representation.
        /// </summary>
        /// <param name="ByteArray">An array of bytes.</param>
        public static String ToBase64URL(this Byte[] ByteArray)
        {
            try
            {
                return Convert.ToBase64String(ByteArray).Replace('+', '-').Replace('/', '_').TrimEnd('=');
            }
            catch (Exception e)
            {
                throw new Exception("Error in ToBase64URL" + e.Message);
            }
        }

        #endregion

        #region FromBASE64URL       (this Base64String)

        public static Byte[] FromBASE64URL(this String Base64String)
        {

            if (TryParseBASE64URL(Base64String, out var byteArray, out var errorResponse))
                return byteArray;

            throw new ArgumentException(
                      errorResponse,
                      nameof(Base64String)
                  );

        }

        #endregion

        #region TryParseBASE64URL   (this Base64String, out ByteArray,  out ErrorResponse)

        public static Boolean TryParseBASE64URL(this String                       Base64String,
                                                [NotNullWhen(true)]  out Byte[]?  ByteArray,
                                                [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                Base64String = Base64String.Trim().Replace('-', '+').Replace('_', '/');

                switch (Base64String.Length % 4)
                {
                    case 2: Base64String += "=="; break;
                    case 3: Base64String += "=";  break;
                }

                if (Base64String.IsNullOrEmpty())
                    ErrorResponse = "The given base64-representation of a byte array must not be null!";

                else if (Base64String.Length % 4 != 0)
                    ErrorResponse = $"The length of the given base64-representation '{Base64String}' of a byte array is invalid ({Base64String.Length} characters)!";

                else if (!Base64Regex.IsMatch(Base64String))
                    ErrorResponse = $"The given base64-representation '{Base64String}' of a byte array is invalid!";

                else
                {
                    ByteArray = Convert.FromBase64String(Base64String);
                    return true;
                }

            }
            catch (Exception e)
            {
                ErrorResponse = $"The given string '{Base64String}' could not be parsed as a base64-representation of a byte array: " + e.Message;
            }

            ErrorResponse ??= $"The given string '{Base64String}' is not a base64-representation of a byte array!";
            ByteArray       = [];
            return false;

        }

        #endregion


        #region FromBASE64_UTF8     (this Base64String)

        public static String FromBASE64_UTF8(this String Base64String)
        {

            if (TryParseBASE64_UTF8(Base64String, out var utf8String, out var errorResponse))
                return utf8String;

            throw new ArgumentException(
                      errorResponse,
                      nameof(Base64String)
                  );

        }

        #endregion

        #region TryParseBASE64_UTF8 (this Base64String, out UTF8String, out ErrorResponse)

        public static Boolean TryParseBASE64_UTF8(this String                       Base64String,
                                                  [NotNullWhen(true)]  out String?  UTF8String,
                                                  [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ErrorResponse = null;

            if (TryParseBASE64(Base64String, out var byteArray, out ErrorResponse))
            {
                try
                {

                    var utf8Decoder   = new UTF8Encoding().GetDecoder();
                    var decodedChars  = new Char[utf8Decoder.GetCharCount(byteArray, 0, byteArray.Length)];
                    utf8Decoder.GetChars(byteArray, 0, byteArray.Length, decodedChars, 0);

                    UTF8String        = new String(decodedChars);
                    return true;

                }
                catch (Exception e)
                {
                    ErrorResponse = $"The given string '{Base64String}' could not be parsed as a base64-representation of an UTF8-string: " + e.Message;
                }

            }

            UTF8String = null;
            return false;

        }

        #endregion


        #region EscapeForXMLandHTML(Text)

        public static String EscapeForXMLandHTML(this String Text)

            => Text.Replace("<", "&lt;").
                    Replace(">", "&gt;").
                    Replace("&", "&amp;");

        #endregion

        #region ToUTF8String(this ArrayOfBytes, NumberOfBytes = null)

        public static String ToUTF8String(this Byte[]  ArrayOfBytes,
                                          UInt32?      NumberOfBytes = null)

            => ArrayOfBytes is null || ArrayOfBytes.Length == 0
                   ? String.Empty
                   : Encoding.UTF8.GetString(
                         ArrayOfBytes,
                         0,
                         NumberOfBytes.HasValue
                             ? (Int32) NumberOfBytes.Value
                             : ArrayOfBytes.Length
                     );

        public static String ToUTF8String(this IEnumerable<Byte>  EnumerationOfBytes,
                                          UInt32?                 NumberOfBytes = null)

            => EnumerationOfBytes is null || !EnumerationOfBytes.Any()
                   ? String.Empty
                   : Encoding.UTF8.GetString(
                         EnumerationOfBytes.ToArray(),
                         0,
                         NumberOfBytes.HasValue
                             ? (Int32) NumberOfBytes.Value
                             : EnumerationOfBytes.Count()
                     );

        #endregion

        #region ToUTF8String(this MemoryStream, NumberOfBytes = null)

        public static String ToUTF8String(this MemoryStream  MemoryStream,
                                          UInt32?            NumberOfBytes = null)

             => MemoryStream is null || MemoryStream.Length == 0
                    ? String.Empty
                    : Encoding.UTF8.GetString(
                          MemoryStream.ToArray(),
                          0,
                          NumberOfBytes.HasValue
                              ? (Int32) NumberOfBytes.Value
                              : (Int32) MemoryStream.Length
                      );

        #endregion

        #region ToUTF8Bytes(this Text)

        public static Byte[] ToUTF8Bytes(this String? Text)

            => Text is null
                   ? []
                   : Encoding.UTF8.GetBytes(Text);

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
                       : String.Empty;

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
                yield return new String([.. Characters]);

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

            if (String.IsNullOrEmpty(GivenString) && _Delegate is not null)
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

            if (!String.IsNullOrEmpty(GivenString) && _Delegate is not null)
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

            if (Text is null)
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

            if (Text is null)
                yield break;

            if (Delimiters is null || Delimiters.Length == 0)
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
                                               if (tuple[0] is not null)
                                               {
                                                   if (!Dictionary.ContainsKey(tuple[0].Trim()))
                                                       Dictionary.Add(tuple[0].Trim(), "");
                                                   else
                                                       Dictionary[tuple[0].Trim()] = "";
                                               }
                                           }

                                           else if (tuple.Length == 2)
                                           {
                                               if (tuple[0] is not null)
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

            if (Strings is null)
                return false;

            foreach (var text in Strings)
            {
                if (text.Equals(Text, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;

        }

        #endregion


        #region Repeat(Text, Times)

        public static String Repeat(this String Text, Int64 Times)
            => String.Concat(Enumerable.Repeat(Text, (Int32)Times));

        public static String Repeat(this String Text, UInt32 Times)
            => String.Concat(Enumerable.Repeat(Text, (Int32)Times));

        public static String Repeat(this String Text, UInt64 Times)
            => String.Concat(Enumerable.Repeat(Text, (Int32)Times));

        public static String Repeat(this String Text, Int32 Times)
            => String.Concat(Enumerable.Repeat(Text, Math.Max(0, Times)));

        #endregion


        #region TrimToNull(this Text)

        /// <summary>
        /// Trims the whitespace from both ends of the string.
        /// When the resulting string is empty, then it will be converted to null.
        /// </summary>
        /// <param name="Text">A given text.</param>
        public static String? TrimToNull(this String? Text)
        {

            var trimmed = Text?.Trim();

            return Text?.Length > 0
                       ? trimmed
                       : null;

        }

        #endregion

        #region FirstCharToLower(this Text)

        /// <summary>
        /// Converts the first character of the given text to lower case.
        /// </summary>
        /// <param name="Text">A given text.</param>
        public static String FirstCharToLower(this String Text)

            => char.ToLower(Text[0]) + Text[1..];

        #endregion


        #region FixedTimeEquals(Text1, Text2)

        /// <summary>
        /// Determine the equality of two texts in an amount of time which depends on
        /// the length of the sequences, but not the values.
        /// </summary>
        /// <param name="Text1">The first text.</param>
        /// <param name="Text2">The second text.</param>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static Boolean FixedTimeEquals(this String Text1, String? Text2)
        {

            var byteArray1    = Encoding.UTF8.GetBytes(Text1);
            var byteArray2    = Encoding.UTF8.GetBytes(Text2 ?? "");

            // It's important to ensure the byte arrays are of equal length to avoid timing attacks.
            // You could pad the shorter one to match the length of the longer one, but ensure that
            // this process does not introduce timing vulnerabilities itself.
            var length        = Math.Max(byteArray1.Length, byteArray2.Length);
            var paddedArray1  = new Byte[length];
            var paddedArray2  = new Byte[length];

            Buffer.BlockCopy(byteArray1, 0, paddedArray1, 0, byteArray1.Length);
            Buffer.BlockCopy(byteArray2, 0, paddedArray2, 0, byteArray2.Length);

            return CryptographicOperations.FixedTimeEquals(paddedArray1, paddedArray2);

        }

        #endregion


        public static String ToLowerFirstChar(this String Input)
        {
            return String.Concat(char.ToLowerInvariant(Input[0]), Input[1..]);
        }


        #region Matches(this Text, Pattern, IgnoreCase = true)

        public static Boolean Matches(this String?  Text,
                                      String        Pattern,
                                      Boolean       IgnoreCase   = true)

            => Text is not null &&
               (IgnoreCase
                    ? Text.Contains(Pattern, StringComparison.OrdinalIgnoreCase)
                    : Text.Contains(Pattern, StringComparison.Ordinal));

        #endregion


    }

}
