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
    /// The kind of a CBOR value within the CBOR document model.
    /// The decoded width of floating-point numbers is preserved.
    /// </summary>
    public enum CBORValueKind
    {

        /// <summary>
        /// A CBOR null value (the default).
        /// </summary>
        Null,

        /// <summary>
        /// A CBOR undefined value.
        /// </summary>
        Undefined,

        /// <summary>
        /// A boolean value.
        /// </summary>
        Boolean,

        /// <summary>
        /// An unsigned integer (major type 0).
        /// All non-negative integers are normalized to this kind.
        /// </summary>
        UnsignedInteger,

        /// <summary>
        /// A negative integer (major type 1),
        /// covering the full range down to -2^64.
        /// </summary>
        NegativeInteger,

        /// <summary>
        /// A byte string (major type 2).
        /// </summary>
        ByteString,

        /// <summary>
        /// A text string (major type 3).
        /// </summary>
        TextString,

        /// <summary>
        /// An array of CBOR values (major type 4).
        /// </summary>
        Array,

        /// <summary>
        /// A map of CBOR key/value pairs (major type 5).
        /// Keys may be of any CBOR kind.
        /// </summary>
        Map,

        /// <summary>
        /// A tagged CBOR value (major type 6).
        /// </summary>
        Tagged,

        /// <summary>
        /// A CBOR simple value (major type 7),
        /// other than false, true, null and undefined.
        /// </summary>
        SimpleValue,

        /// <summary>
        /// A half-precision floating-point number.
        /// </summary>
        HalfFloat,

        /// <summary>
        /// A single-precision floating-point number.
        /// </summary>
        SingleFloat,

        /// <summary>
        /// A double-precision floating-point number.
        /// </summary>
        DoubleFloat

    }

}
