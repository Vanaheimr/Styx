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
    /// How a CBOR byte string is rendered as JSON text, which has no
    /// binary type of its own.
    /// </summary>
    public enum CBORJSONByteStrings
    {

        /// <summary>
        /// Base64 with the URL and filename safe alphabet and without
        /// padding, [RFC 4648] Section 5. The default.
        /// </summary>
        Base64URL,

        /// <summary>
        /// Base64 with the standard alphabet and padding,
        /// [RFC 4648] Section 4.
        /// </summary>
        Base64,

        /// <summary>
        /// Lowercase hexadecimal.
        /// </summary>
        Hex

    }


    /// <summary>
    /// How a metrological value (CBOR tag 44252) crosses over to JSON.
    /// </summary>
    public enum CBORJSONMetrology
    {

        /// <summary>
        /// As one string in the metrological text format, e.g. "1.10 kWh"
        /// or "(230.00 ±0.12) V, k=2". The default: a measurement stays one
        /// value in the JSON document, and every JSON tool can read it.
        /// </summary>
        Text,

        /// <summary>
        /// As a JSON object, e.g. { "value": 1.10, "unit": "Wh", "prefix": "k" }.
        /// </summary>
        Object,

        /// <summary>
        /// Not at all: tag 44252 is left to the handling of unknown tags.
        /// </summary>
        None

    }


    /// <summary>
    /// Which JSON strings are examined for the metrological text format on
    /// the way back into CBOR.
    ///
    /// This is a decision about somebody else's document, so it is the
    /// caller's to make. Turning a string of prose into a measurement is the
    /// kind of mistake nothing downstream can notice: what comes out is a
    /// perfectly well-formed reading of something nobody measured.
    /// </summary>
    public enum CBORJSONReadings
    {

        /// <summary>
        /// None. A string stays a string. The default: a caller who says
        /// nothing about a document gets no guesses about it.
        /// </summary>
        None,

        /// <summary>
        /// Every string is tried against the grammar, which is what recovers
        /// a document that nothing described - and what the round trip of
        /// metrological-text.md Section 3 asks for. The grammar is anchored
        /// and strict, so the hazard is narrow and real: a prose field
        /// holding "1 h" becomes one hour.
        /// </summary>
        Auto

    }


    /// <summary>
    /// What to do with a CBOR data item that this JSON profile does not
    /// cover: an unknown tag, a simple value, undefined.
    /// </summary>
    public enum CBORJSONUnknownItems
    {

        /// <summary>
        /// Refuse the document. The default: JSON that silently drops what
        /// it did not understand is worse than no JSON at all.
        /// </summary>
        Error,

        /// <summary>
        /// Render the item in the diagnostic notation of [RFC 8949]
        /// Section 8 as a JSON string, e.g. "55(h'0102')". This is
        /// readable, but does not convert back.
        /// </summary>
        Diagnostic

    }


    /// <summary>
    /// Whether a JSON string is examined for the metrological text format
    /// on the way back into CBOR.
    /// </summary>
    /// <param name="Path">The JSON Pointer ([RFC 6901]) of the string within the document, e.g. "/readings/0/energy".</param>
    /// <param name="Text">The text of the string.</param>
    public delegate Boolean MetrologicalValueDetector(String  Path,
                                                      String  Text);


    /// <summary>
    /// The options of the document-level conversion between CBOR and JSON.
    /// </summary>
    public sealed class CBORJSONOptions
    {

        #region Properties

        /// <summary>
        /// How a CBOR byte string is rendered as JSON text.
        /// The way back is not covered: JSON has no binary type, and a
        /// string that happens to be Base64 is still just a string.
        /// </summary>
        public CBORJSONByteStrings          ByteStrings                { get; init; } = CBORJSONByteStrings.Base64URL;

        /// <summary>
        /// How a metrological value (CBOR tag 44252) crosses over to JSON.
        /// </summary>
        public CBORJSONMetrology            Metrology                  { get; init; } = CBORJSONMetrology.Text;

        /// <summary>
        /// What to do with a CBOR data item that this JSON profile does
        /// not cover.
        /// </summary>
        public CBORJSONUnknownItems         UnknownItems               { get; init; } = CBORJSONUnknownItems.Error;

        /// <summary>
        /// Which JSON strings are examined for the metrological text format
        /// on the way back into CBOR.
        /// Default: None - nothing is guessed.
        /// Ignored where DetectMetrologicalValues names a predicate, which is
        /// the more precise way of saying the same thing.
        /// </summary>
        public CBORJSONReadings             Readings                   { get; init; } = CBORJSONReadings.None;

        /// <summary>
        /// Which JSON strings are examined for the metrological text format
        /// on the way back into CBOR, decided per path - which is what an
        /// application with a schema should use, and what the hazard of
        /// Readings.Auto calls for. Overrides Readings where it is given.
        /// </summary>
        public MetrologicalValueDetector?   DetectMetrologicalValues   { get; init; }

        /// <summary>
        /// Whether a CBOR map key that is not a text string is stringified
        /// instead of refused. COSE maps have integer keys, so this is
        /// needed more often than it looks; the way back turns it into a
        /// text key, not into the integer it was.
        /// </summary>
        public Boolean                      StringifyMapKeys           { get; init; } = false;

        /// <summary>
        /// Whether the UTF-8 output is indented.
        /// </summary>
        public Boolean                      Indent                     { get; init; } = false;

        /// <summary>
        /// The maximum nesting depth of the document, in both directions.
        /// </summary>
        public Int32                        MaxDepth                   { get; init; } = 64;

        #endregion

        #region Statics

        /// <summary>
        /// The default options: metrological values as text, byte strings
        /// as Base64URL, and an error for everything not covered.
        /// </summary>
        public static CBORJSONOptions  Default      { get; } = new ();

        /// <summary>
        /// Options that never refuse a document: unknown items are rendered
        /// in diagnostic notation and map keys are stringified. Useful for
        /// looking at something, and not for anything that has to convert
        /// back.
        /// </summary>
        public static CBORJSONOptions  Lenient      { get; } = new () {
                                                                   UnknownItems      = CBORJSONUnknownItems.Diagnostic,
                                                                   StringifyMapKeys  = true
                                                               };

        #endregion

    }

}
