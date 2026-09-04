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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A value within a PDF document.
    ///
    /// Chargy reads PDFs for one reason only: a PDF/A-3 invoice may carry the
    /// charge transparency record as an embedded attachment. So this models just
    /// enough of the PDF object system to walk from the document catalogue to
    /// those attachments — nothing about pages, fonts or rendering.
    /// </summary>
    public abstract record PDFObject
    {

        /// <summary>
        /// The PDF "null" object.
        /// </summary>
        public static readonly PDFObject Null = new PDFNull();

    }


    /// <summary>
    /// The PDF "null" object.
    /// </summary>
    public sealed record PDFNull : PDFObject;


    /// <summary>
    /// A PDF boolean.
    /// </summary>
    /// <param name="Value">The value.</param>
    public sealed record PDFBoolean(Boolean Value) : PDFObject;


    /// <summary>
    /// A PDF number.
    ///
    /// PDF makes no type distinction beyond "integer" and "real", and the
    /// distinction never matters here, so both are kept as a double.
    /// </summary>
    /// <param name="Value">The value.</param>
    public sealed record PDFNumber(Double Value) : PDFObject
    {

        /// <summary>
        /// This number as a 32 bit integer.
        /// </summary>
        public Int32 AsInt32
            => (Int32) Value;

        /// <summary>
        /// This number as a 64 bit integer.
        /// </summary>
        public Int64 AsInt64
            => (Int64) Value;

    }


    /// <summary>
    /// A PDF string.
    ///
    /// Kept as bytes, because a PDF string is a byte string: it may be PDFDocEncoded,
    /// UTF-16BE with a byte order mark, or — since PDF 2.0 — UTF-8.
    /// </summary>
    /// <param name="Bytes">The bytes of the string.</param>
    public sealed record PDFString(Byte[] Bytes) : PDFObject
    {

        /// <summary>
        /// This string as text, honouring an UTF-16 or UTF-8 byte order mark.
        /// </summary>
        public String AsText()
        {

            if (Bytes.Length >= 2 && Bytes[0] == 0xFE && Bytes[1] == 0xFF)
                return System.Text.Encoding.BigEndianUnicode.GetString(Bytes, 2, Bytes.Length - 2);

            if (Bytes.Length >= 3 && Bytes[0] == 0xEF && Bytes[1] == 0xBB && Bytes[2] == 0xBF)
                return System.Text.Encoding.UTF8.GetString(Bytes, 3, Bytes.Length - 3);

            return System.Text.Encoding.UTF8.GetString(Bytes);

        }

    }


    /// <summary>
    /// A PDF name, e.g. "/EmbeddedFiles".
    /// </summary>
    /// <param name="Value">The name, without its leading slash.</param>
    public sealed record PDFName(String Value) : PDFObject;


    /// <summary>
    /// A PDF array.
    /// </summary>
    /// <param name="Items">The elements of the array.</param>
    public sealed record PDFArray(IReadOnlyList<PDFObject> Items) : PDFObject
    {

        /// <summary>
        /// The number of elements.
        /// </summary>
        public Int32 Count
            => Items.Count;

        /// <summary>
        /// The element at the given position, or null when out of range.
        /// </summary>
        /// <param name="Index">The position of an element.</param>
        public PDFObject? this[Int32 Index]
            => Index >= 0 && Index < Items.Count
                   ? Items[Index]
                   : null;

    }


    /// <summary>
    /// A PDF dictionary.
    /// </summary>
    /// <param name="Entries">The entries of the dictionary, keyed by name without its leading slash.</param>
    public sealed record PDFDictionary(IReadOnlyDictionary<String, PDFObject> Entries) : PDFObject
    {

        /// <summary>
        /// The value of the given key, or null when absent.
        /// </summary>
        /// <param name="Key">A key, without its leading slash.</param>
        public PDFObject? this[String Key]
            => Entries.TryGetValue(Key, out var value)
                   ? value
                   : null;

        /// <summary>
        /// Whether this dictionary has the given key.
        /// </summary>
        /// <param name="Key">A key, without its leading slash.</param>
        public Boolean Contains(String Key)
            => Entries.ContainsKey(Key);

    }


    /// <summary>
    /// A PDF stream: a dictionary describing a block of bytes, plus those bytes
    /// still in whatever encoding the dictionary's "/Filter" announces.
    /// </summary>
    /// <param name="Dictionary">The dictionary describing the stream.</param>
    /// <param name="RawData">The still encoded bytes of the stream.</param>
    public sealed record PDFStream(PDFDictionary  Dictionary,
                                   Byte[]         RawData) : PDFObject;


    /// <summary>
    /// A reference to another object, e.g. "12 0 R".
    /// </summary>
    /// <param name="Number">The number of the referenced object.</param>
    /// <param name="Generation">The generation of the referenced object.</param>
    public sealed record PDFReference(Int32  Number,
                                      Int32  Generation) : PDFObject;

}
