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
    /// How invalid UTF-8 byte sequences within CBOR text strings are handled.
    /// </summary>
    public enum CBORUTF8Validation
    {

        /// <summary>
        /// Invalid UTF-8 byte sequences are an error (default).
        /// Within indefinite-length text strings every chunk must be
        /// valid UTF-8 on its own (RFC 8949, Section 3.2.3).
        /// </summary>
        Strict,

        /// <summary>
        /// Invalid UTF-8 byte sequences are replaced by U+FFFD.
        /// </summary>
        Lenient

    }


    /// <summary>
    /// Options for reading CBOR data.
    /// </summary>
    public class CBORReaderOptions
    {

        #region Properties

        /// <summary>
        /// The maximum nesting depth of arrays, maps and tags.
        /// Default: 64.
        /// </summary>
        public Int32                    MaxDepth               { get; init; } = 64;

        /// <summary>
        /// How duplicate CBOR map keys are handled when a map is materialized.
        /// Default: Error.
        /// </summary>
        public CBORDuplicateKeyPolicy   DuplicateKeyPolicy     { get; init; } = CBORDuplicateKeyPolicy.Error;

        /// <summary>
        /// How invalid UTF-8 byte sequences within CBOR text strings are handled.
        /// Default: Strict.
        /// </summary>
        public CBORUTF8Validation       UTF8Validation         { get; init; } = CBORUTF8Validation.Strict;

        /// <summary>
        /// Whether to verify on-the-fly that the CBOR data satisfies the
        /// Core Deterministic Encoding Requirements of RFC 8949, Section 4.2.1:
        /// Shortest-form integer heads, shortest-form floating-point values,
        /// NaN canonicalized to 0xf97e00, no indefinite-length items and
        /// map keys sorted in bytewise lexicographic order of their encodings
        /// without duplicates.
        /// Default: false.
        /// </summary>
        public Boolean                  RequireDeterministic   { get; init; } = false;

        /// <summary>
        /// Whether to additionally verify the preferred serialization of
        /// bignums (RFC 8949, Section 4.2.2): The byte string of a bignum
        /// carries no leading zeroes, and a value a basic integer of major
        /// type 0 or 1 could carry is written as that integer rather than
        /// as a bignum.
        ///
        /// Kept apart from RequireDeterministic because RFC 8949 keeps them
        /// apart: Section 4.2.1 is the core requirement of every
        /// deterministic encoder, Section 4.2.2 is an additional
        /// consideration for protocols that use tags 2 and 3 - which a
        /// protocol carrying exact decimal measurements does.
        /// Default: false.
        /// </summary>
        public Boolean                  RequirePreferredBignums   { get; init; } = false;

        #endregion


        #region Static defaults

        /// <summary>
        /// The default CBOR reader options.
        /// </summary>
        public static CBORReaderOptions  Default      { get; } = new ();

        /// <summary>
        /// CBOR reader options verifying the Core Deterministic Encoding
        /// Requirements of RFC 8949, Section 4.2.1, together with the
        /// preferred serialization of bignums of its Section 4.2.2.
        /// </summary>
        public static CBORReaderOptions  Canonical    { get; } = new () {
                                                                     RequireDeterministic     = true,
                                                                     RequirePreferredBignums  = true
                                                                 };

        #endregion

    }

}
