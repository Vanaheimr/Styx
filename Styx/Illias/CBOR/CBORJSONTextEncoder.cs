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

using System.Text.Encodings.Web;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A JSON text encoder that escapes what [RFC 8259], Section 7 requires
    /// and nothing else: the quotation mark, the reverse solidus, and the
    /// control characters below U+0020. Every other character is written as
    /// itself, in UTF-8, which [RFC 8259], Section 8.1 requires of any JSON
    /// exchanged outside a closed ecosystem anyway.
    ///
    /// This exists because none of the encoders that ship with
    /// System.Text.Json can do that. `UnsafeRelaxedJsonEscaping` passes the
    /// Basic Multilingual Plane through unescaped - `ä` and `€` come out as
    /// themselves - but a character above U+FFFF is escaped into a UTF-16
    /// surrogate pair regardless: `JavaScriptEncoder.Create` takes
    /// `UnicodeRange`s, and a `UnicodeRange` cannot reach past U+FFFF, so
    /// `UnicodeRanges.All` means "all of the BMP". Measured on .NET 10:
    /// `Default`, `Create(UnicodeRanges.All)` and `UnsafeRelaxedJsonEscaping`
    /// all write U+1F600 as the escaped surrogate pair rather than as itself.
    ///
    /// Why that mattered enough to write this: Styx converts CBOR to JSON
    /// along two paths - straight to UTF-8 through `Utf8JsonWriter`, and
    /// through a Newtonsoft tree - and they are meant to produce the same
    /// text. Newtonsoft writes the character; `Utf8JsonWriter` wrote the
    /// surrogate pair. The two paths therefore disagreed for exactly the
    /// characters nobody had put in a test, and `BothPathsAgree` said they
    /// agreed because no vector carried one.
    ///
    /// A surrogate pair is also the one place where UTF-16 leaks into a
    /// format that has nothing else to do with it: CBOR text strings are
    /// UTF-8 and JSON text is UTF-8, and the escaped spelling asks a reader
    /// to reassemble a scalar from two code units of an encoding neither of
    /// them uses.
    /// </summary>
    public sealed class CBORJSONTextEncoder : JavaScriptEncoder
    {

        #region Data

        /// <summary>
        /// The shared instance. The encoder holds no state.
        /// </summary>
        public static readonly CBORJSONTextEncoder Instance = new ();

        #endregion

        #region Properties

        /// <summary>
        /// The longest escape this encoder produces is `\uXXXX`.
        /// </summary>
        public override Int32 MaxOutputCharactersPerInputCharacter
            => 6;

        #endregion

        #region WillEncode(UnicodeScalar)

        /// <summary>
        /// Whether the given scalar has to be escaped: the two characters
        /// [RFC 8259], Section 7 names, and the control characters it forbids
        /// unescaped. Nothing else, and in particular nothing for being
        /// large.
        /// </summary>
        /// <param name="UnicodeScalar">A Unicode scalar value.</param>
        public override Boolean WillEncode(Int32 UnicodeScalar)

            => UnicodeScalar == '"'  ||
               UnicodeScalar == '\\' ||
               UnicodeScalar  <  0x20;

        #endregion

        #region FindFirstCharacterToEncode(Text, TextLength)

        /// <summary>
        /// The index of the first character that has to be escaped, or -1.
        ///
        /// Surrogates are deliberately not examined here: a well-formed pair
        /// denotes a scalar this encoder passes through, and an unpaired one
        /// is refused further down by the UTF-8 encoder rather than quietly
        /// escaped into something that reads back as a different string.
        /// </summary>
        public override unsafe Int32 FindFirstCharacterToEncode(Char*  Text,
                                                                Int32  TextLength)
        {

            for (var index = 0; index < TextLength; index++)
                if (WillEncode(Text[index]))
                    return index;

            return -1;

        }

        #endregion

        #region TryEncodeUnicodeScalar(UnicodeScalar, Buffer, BufferLength, out CharsWritten)

        /// <summary>
        /// Write the escape for a scalar that WillEncode named. The
        /// two-character forms of [RFC 8259], Section 7 are preferred where
        /// they exist, because they are what every JSON document in the world
        /// carries; the rest become `\uXXXX`.
        /// </summary>
        public override unsafe Boolean TryEncodeUnicodeScalar(Int32      UnicodeScalar,
                                                              Char*      Buffer,
                                                              Int32      BufferLength,
                                                              out Int32  CharsWritten)
        {

            var escape = UnicodeScalar switch {
                             '"'   => "\\\"",
                             '\\'  => "\\\\",
                             '\b'  => "\\b",
                             '\f'  => "\\f",
                             '\n'  => "\\n",
                             '\r'  => "\\r",
                             '\t'  => "\\t",
                             _     => $"\\u{UnicodeScalar:X4}"
                         };

            if (escape.Length > BufferLength)
            {
                CharsWritten = 0;
                return false;
            }

            for (var index = 0; index < escape.Length; index++)
                Buffer[index] = escape[index];

            CharsWritten = escape.Length;

            return true;

        }

        #endregion

    }

}
