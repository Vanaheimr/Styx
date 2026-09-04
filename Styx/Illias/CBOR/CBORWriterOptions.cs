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
    /// Options for writing CBOR data.
    /// </summary>
    public class CBORWriterOptions
    {

        #region Properties

        /// <summary>
        /// Whether to enforce the Core Deterministic Encoding Requirements
        /// of RFC 8949, Section 4.2.1: Map keys are sorted in bytewise
        /// lexicographic order of their encodings, duplicate map keys are
        /// rejected, indefinite-length items are forbidden and all NaN
        /// floating-point values are canonicalized to 0xf97e00.
        /// Default: false.
        /// </summary>
        public Boolean  Deterministic             { get; init; } = false;

        /// <summary>
        /// Whether to use the preferred serialization of RFC 8949, Section 4.1
        /// for floating-point numbers: Every value written via WriteDouble or
        /// WriteSingle is shrunk to the shortest floating-point encoding that
        /// preserves its exact value, and all NaN values are written as 0xf97e00
        /// (NaN payloads are not preserved). When false, floating-point values
        /// are written with exactly the requested width.
        /// Integer heads are always encoded in their shortest form.
        /// Default: true.
        /// </summary>
        public Boolean  PreferredFloatEncoding    { get; init; } = true;

        /// <summary>
        /// The maximum nesting depth of arrays, maps and tags.
        /// Default: 64.
        /// </summary>
        public Int32    MaxDepth                  { get; init; } = 64;

        #endregion


        #region Static defaults

        /// <summary>
        /// The default CBOR writer options: Preferred serialization,
        /// but no deterministic encoding requirements.
        /// </summary>
        public static CBORWriterOptions  Default      { get; } = new ();

        /// <summary>
        /// CBOR writer options enforcing the Core Deterministic Encoding
        /// Requirements of RFC 8949, Section 4.2.1.
        /// </summary>
        public static CBORWriterOptions  Canonical    { get; } = new () {
                                                                     Deterministic = true
                                                                 };

        #endregion

    }

}
