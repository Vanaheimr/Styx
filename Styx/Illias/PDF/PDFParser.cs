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

using System.Globalization;
using System.Text;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Reads PDF objects out of the bytes of a PDF file.
    ///
    /// A hand-written parser rather than a PDF library, because Chargy needs a
    /// vanishingly small part of PDF: the object syntax, and enough of it to walk
    /// from the document catalogue to an embedded attachment.
    /// </summary>
    /// <param name="Data">The bytes of a PDF file, or of an object stream within one.</param>
    /// <param name="Position">Where to start reading.</param>
    public sealed class PDFParser(Byte[]  Data,
                                  Int32   Position = 0)
    {

        #region Data

        private readonly Byte[]  data      = Data;
        private          Int32   position  = Position;

        /// <summary>
        /// How deeply arrays and dictionaries may nest.
        ///
        /// A PDF that arrives for verification is not necessarily well-formed, and
        /// a self-referential structure must exhaust a counter rather than the stack.
        /// </summary>
        private const Int32      MaxDepth  = 64;

        #endregion

        #region Properties

        /// <summary>
        /// The current reading position.
        /// </summary>
        public Int32 Position
        {
            get => position;
            set => position = value;
        }

        #endregion


        #region (static) IsWhitespace(Byte)

        /// <summary>
        /// Whether the given byte separates PDF tokens.
        /// </summary>
        /// <param name="Byte">A byte.</param>
        public static Boolean IsWhitespace(Byte Byte)

            => Byte is 0x00 or 0x09 or 0x0A or 0x0C or 0x0D or 0x20;

        #endregion

        #region (static) IsDelimiter (Byte)

        /// <summary>
        /// Whether the given byte ends a PDF token without being whitespace.
        /// </summary>
        /// <param name="Byte">A byte.</param>
        public static Boolean IsDelimiter(Byte Byte)

            => Byte is 0x28 or 0x29 or 0x3C or 0x3E or 0x5B or 0x5D or
                       0x7B or 0x7D or 0x2F or 0x25;

        #endregion


        #region SkipWhitespace()

        /// <summary>
        /// Skip whitespace and comments.
        /// </summary>
        public void SkipWhitespace()
        {

            while (position < data.Length)
            {

                var current = data[position];

                if (IsWhitespace(current))
                {
                    position++;
                    continue;
                }

                // A comment runs to the end of the line.
                if (current == (Byte) '%')
                {

                    while (position < data.Length &&
                           data[position] != 0x0A &&
                           data[position] != 0x0D)
                    {
                        position++;
                    }

                    continue;

                }

                return;

            }

        }

        #endregion

        #region ReadToken()

        /// <summary>
        /// Read the next bare token, e.g. "obj", "endobj", "stream" or a number.
        ///
        /// Returns an empty string at the end of the data, and returns delimiters
        /// as single character tokens.
        /// </summary>
        public String ReadToken()
        {

            SkipWhitespace();

            if (position >= data.Length)
                return "";

            var start = position;

            if (IsDelimiter(data[position]))
            {
                position++;
                return Encoding.ASCII.GetString(data, start, 1);
            }

            while (position < data.Length &&
                  !IsWhitespace(data[position]) &&
                  !IsDelimiter (data[position]))
            {
                position++;
            }

            return Encoding.ASCII.GetString(data, start, position - start);

        }

        #endregion

        #region PeekToken()

        /// <summary>
        /// Read the next bare token without consuming it.
        /// </summary>
        public String PeekToken()
        {

            var saved  = position;
            var token  = ReadToken();
            position   = saved;

            return token;

        }

        #endregion


        #region ParseObject(Depth = 0)

        /// <summary>
        /// Parse the next object.
        /// </summary>
        /// <param name="Depth">The current nesting depth.</param>
        public PDFObject ParseObject(Int32 Depth = 0)
        {

            if (Depth > MaxDepth)
                return PDFObject.Null;

            SkipWhitespace();

            if (position >= data.Length)
                return PDFObject.Null;

            var current = data[position];

            #region Name

            if (current == (Byte) '/')
                return ParseName();

            #endregion

            #region Literal string

            if (current == (Byte) '(')
                return ParseLiteralString();

            #endregion

            #region Dictionary or hexadecimal string

            if (current == (Byte) '<')
                return position + 1 < data.Length && data[position + 1] == (Byte) '<'
                           ? ParseDictionaryOrStream(Depth)
                           : ParseHexString();

            #endregion

            #region Array

            if (current == (Byte) '[')
                return ParseArray(Depth);

            #endregion

            #region An unexpected closing delimiter

            if (current is (Byte) ']' or (Byte) '>' or (Byte) ')' or (Byte) '}')
            {
                position++;
                return PDFObject.Null;
            }

            #endregion

            #region Keywords, numbers and indirect references

            var token = ReadToken();

            switch (token)
            {

                case "true":
                    return new PDFBoolean(true);

                case "false":
                    return new PDFBoolean(false);

                case "null":
                case "":
                    return PDFObject.Null;

            }

            if (TryParseNumber(token, out var number))
            {

                // "12 0 R" is a reference; "12 0 obj" starts an object. Both begin
                // with two integers, so the third token decides — and if it is
                // neither, the position has to snap back to just after the number.
                if (IsInteger(token))
                {

                    var saved            = position;
                    var generationToken  = ReadToken();

                    if (IsInteger(generationToken))
                    {

                        var keyword = ReadToken();

                        if (keyword == "R")
                            return new PDFReference(
                                       (Int32) number,
                                       Int32.Parse(generationToken, CultureInfo.InvariantCulture)
                                   );

                    }

                    position = saved;

                }

                return new PDFNumber(number);

            }

            #endregion

            // An unknown keyword: skip it rather than fail the whole document.
            return PDFObject.Null;

        }

        #endregion


        #region (private) ParseName()

        /// <summary>
        /// Parse a name, resolving its "#xx" escapes.
        /// </summary>
        private PDFName ParseName()
        {

            position++;   // "/"

            var name = new StringBuilder();

            while (position < data.Length &&
                  !IsWhitespace(data[position]) &&
                  !IsDelimiter (data[position]))
            {

                var current = data[position];

                if (current == (Byte) '#' &&
                    position + 2 < data.Length &&
                    TryParseHexDigit(data[position + 1], out var high) &&
                    TryParseHexDigit(data[position + 2], out var low))
                {
                    name.Append((Char) ((high << 4) | low));
                    position += 3;
                    continue;
                }

                name.Append((Char) current);
                position++;

            }

            return new PDFName(name.ToString());

        }

        #endregion

        #region (private) ParseLiteralString()

        /// <summary>
        /// Parse a "(…)" string, honouring balanced parentheses and escapes.
        /// </summary>
        private PDFString ParseLiteralString()
        {

            position++;   // "("

            var bytes  = new List<Byte>();
            var depth  = 1;

            while (position < data.Length)
            {

                var current = data[position++];

                if (current == (Byte) '\\')
                {

                    if (position >= data.Length)
                        break;

                    var escaped = data[position++];

                    switch ((Char) escaped)
                    {

                        case 'n':   bytes.Add(0x0A);  break;
                        case 'r':   bytes.Add(0x0D);  break;
                        case 't':   bytes.Add(0x09);  break;
                        case 'b':   bytes.Add(0x08);  break;
                        case 'f':   bytes.Add(0x0C);  break;
                        case '(':   bytes.Add(0x28);  break;
                        case ')':   bytes.Add(0x29);  break;
                        case '\\':  bytes.Add(0x5C);  break;

                        // A backslash before a newline continues the line.
                        case '\r':
                            if (position < data.Length && data[position] == 0x0A)
                                position++;
                            break;

                        case '\n':
                            break;

                        default:
                            if (escaped >= (Byte) '0' && escaped <= (Byte) '7')
                            {

                                var octal = escaped - (Byte) '0';

                                for (var i = 0; i < 2 && position < data.Length &&
                                                data[position] >= (Byte) '0' && data[position] <= (Byte) '7'; i++)
                                {
                                    octal = (octal << 3) | (data[position++] - (Byte) '0');
                                }

                                bytes.Add((Byte) octal);

                            }
                            else
                                bytes.Add(escaped);
                            break;

                    }

                    continue;

                }

                if (current == (Byte) '(')
                {
                    depth++;
                    bytes.Add(current);
                    continue;
                }

                if (current == (Byte) ')')
                {

                    depth--;

                    if (depth == 0)
                        break;

                    bytes.Add(current);
                    continue;

                }

                bytes.Add(current);

            }

            return new PDFString([.. bytes]);

        }

        #endregion

        #region (private) ParseHexString()

        /// <summary>
        /// Parse a "&lt;…&gt;" string. An odd number of digits is padded with a
        /// trailing zero, as the specification requires.
        /// </summary>
        private PDFString ParseHexString()
        {

            position++;   // "<"

            var bytes    = new List<Byte>();
            var current  = 0;
            var haveHigh = false;

            while (position < data.Length && data[position] != (Byte) '>')
            {

                if (TryParseHexDigit(data[position], out var digit))
                {

                    if (haveHigh)
                    {
                        bytes.Add((Byte) ((current << 4) | digit));
                        haveHigh = false;
                    }
                    else
                    {
                        current  = digit;
                        haveHigh = true;
                    }

                }

                position++;

            }

            if (haveHigh)
                bytes.Add((Byte) (current << 4));

            if (position < data.Length)
                position++;   // ">"

            return new PDFString([.. bytes]);

        }

        #endregion

        #region (private) ParseArray(Depth)

        /// <summary>
        /// Parse a "[…]" array.
        /// </summary>
        /// <param name="Depth">The current nesting depth.</param>
        private PDFArray ParseArray(Int32 Depth)
        {

            position++;   // "["

            var items = new List<PDFObject>();

            while (true)
            {

                SkipWhitespace();

                if (position >= data.Length)
                    break;

                if (data[position] == (Byte) ']')
                {
                    position++;
                    break;
                }

                var before = position;
                items.Add(ParseObject(Depth + 1));

                // A token that consumed nothing would spin forever.
                if (position == before)
                {
                    position++;
                }

            }

            return new PDFArray(items);

        }

        #endregion

        #region (private) ParseDictionaryOrStream(Depth)

        /// <summary>
        /// Parse a "&lt;&lt;…&gt;&gt;" dictionary and, when one follows, the stream it describes.
        /// </summary>
        /// <param name="Depth">The current nesting depth.</param>
        private PDFObject ParseDictionaryOrStream(Int32 Depth)
        {

            position += 2;   // "<<"

            var entries = new Dictionary<String, PDFObject>(StringComparer.Ordinal);

            while (true)
            {

                SkipWhitespace();

                if (position >= data.Length)
                    break;

                if (data[position] == (Byte) '>')
                {

                    position++;

                    if (position < data.Length && data[position] == (Byte) '>')
                        position++;

                    break;

                }

                if (data[position] != (Byte) '/')
                {

                    // Not a key: skip whatever this is rather than lose the
                    // entries already read.
                    var before = position;
                    ParseObject(Depth + 1);

                    if (position == before)
                        position++;

                    continue;

                }

                var key    = ParseName().Value;
                var value  = ParseObject(Depth + 1);

                entries[key] = value;

            }

            var dictionary = new PDFDictionary(entries);

            #region Is a stream following?

            var savedPosition = position;

            SkipWhitespace();

            if (position + 6 <= data.Length &&
                data.AsSpan(position, 6).SequenceEqual("stream"u8))
            {

                position += 6;

                // The keyword is followed by CRLF or LF, but never by CR alone.
                if (position < data.Length && data[position] == 0x0D)
                    position++;

                if (position < data.Length && data[position] == 0x0A)
                    position++;

                var streamStart  = position;
                var streamEnd    = FindStreamEnd(dictionary, streamStart);

                var raw          = new Byte[streamEnd - streamStart];
                Array.Copy(data, streamStart, raw, 0, raw.Length);

                position         = streamEnd;

                // Step over "endstream".
                SkipWhitespace();

                if (position + 9 <= data.Length &&
                    data.AsSpan(position, 9).SequenceEqual("endstream"u8))
                    position += 9;

                return new PDFStream(dictionary, raw);

            }

            position = savedPosition;

            #endregion

            return dictionary;

        }

        #endregion

        #region (private) FindStreamEnd(Dictionary, StreamStart)

        /// <summary>
        /// Work out where the bytes of a stream end.
        ///
        /// "/Length" is authoritative when it is a direct number and actually
        /// lands on the "endstream" keyword. It often is not — it may be an
        /// indirect reference, or simply wrong in a file produced by a careless
        /// writer — so the fallback is to search for the keyword itself.
        /// </summary>
        /// <param name="Dictionary">The dictionary describing the stream.</param>
        /// <param name="StreamStart">Where the bytes of the stream begin.</param>
        private Int32 FindStreamEnd(PDFDictionary  Dictionary,
                                    Int32          StreamStart)
        {

            if (Dictionary["Length"] is PDFNumber length)
            {

                var declaredEnd = StreamStart + length.AsInt32;

                if (declaredEnd >= StreamStart &&
                    declaredEnd <= data.Length &&
                    LooksLikeEndstream(declaredEnd))
                {
                    return declaredEnd;
                }

            }

            var searchFrom = data.AsSpan(StreamStart).IndexOf("endstream"u8);

            if (searchFrom < 0)
                return data.Length;

            var end = StreamStart + searchFrom;

            // The end-of-line before "endstream" belongs to the keyword, not to
            // the data.
            if (end > StreamStart && data[end - 1] == 0x0A)
                end--;

            if (end > StreamStart && data[end - 1] == 0x0D)
                end--;

            return end;

        }

        #endregion

        #region (private) LooksLikeEndstream(Offset)

        /// <summary>
        /// Whether the "endstream" keyword follows at the given offset, allowing
        /// for the whitespace a writer may have inserted.
        /// </summary>
        /// <param name="Offset">An offset into the data.</param>
        private Boolean LooksLikeEndstream(Int32 Offset)
        {

            var index = Offset;

            while (index < data.Length && IsWhitespace(data[index]))
                index++;

            return index + 9 <= data.Length &&
                   data.AsSpan(index, 9).SequenceEqual("endstream"u8);

        }

        #endregion


        #region (private, static) TryParseNumber  (Token, out Value)

        /// <summary>
        /// Parse a PDF number, which may be written as "4.", ".5" or "--3".
        /// </summary>
        /// <param name="Token">A token.</param>
        /// <param name="Value">The parsed number.</param>
        private static Boolean TryParseNumber(String Token, out Double Value)
        {

            Value = 0;

            if (Token.Length == 0)
                return false;

            // PDF writers have been known to emit "--3" for -3.
            var normalized = Token.Replace("--", "-");

            return Double.TryParse(
                       normalized,
                       NumberStyles.Float,
                       CultureInfo.InvariantCulture,
                       out Value
                   );

        }

        #endregion

        #region (private, static) IsInteger      (Token)

        /// <summary>
        /// Whether the given token is a plain non-negative integer, as object and
        /// generation numbers always are.
        /// </summary>
        /// <param name="Token">A token.</param>
        private static Boolean IsInteger(String Token)
        {

            if (Token.Length == 0)
                return false;

            foreach (var character in Token)
                if (character is < '0' or > '9')
                    return false;

            return true;

        }

        #endregion

        #region (private, static) TryParseHexDigit(Byte, out Value)

        /// <summary>
        /// Parse a single hexadecimal digit.
        /// </summary>
        /// <param name="Byte">A byte.</param>
        /// <param name="Value">The value of the digit.</param>
        private static Boolean TryParseHexDigit(Byte Byte, out Int32 Value)
        {

            Value = Byte switch {
                        >= 0x30 and <= 0x39  => Byte - 0x30,          // '0'..'9'
                        >= 0x41 and <= 0x46  => Byte - 0x41 + 10,     // 'A'..'F'
                        >= 0x61 and <= 0x66  => Byte - 0x61 + 10,     // 'a'..'f'
                        _                    => -1
                    };

            return Value >= 0;

        }

        #endregion


    }

}
