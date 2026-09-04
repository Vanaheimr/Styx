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
    /// The state of a CBOR reader, describing the next data item to be read.
    /// </summary>
    public enum CBORReaderState
    {

        /// <summary>
        /// The next data item is an unsigned integer (major type 0).
        /// </summary>
        UnsignedInteger,

        /// <summary>
        /// The next data item is a negative integer (major type 1).
        /// </summary>
        NegativeInteger,

        /// <summary>
        /// The next data item is a definite-length byte string (major type 2).
        /// </summary>
        ByteString,

        /// <summary>
        /// The next data item is an indefinite-length byte string (major type 2).
        /// </summary>
        StartIndefiniteLengthByteString,

        /// <summary>
        /// The next data item is a definite-length text string (major type 3).
        /// </summary>
        TextString,

        /// <summary>
        /// The next data item is an indefinite-length text string (major type 3).
        /// </summary>
        StartIndefiniteLengthTextString,

        /// <summary>
        /// The next data item is an array (major type 4).
        /// </summary>
        StartArray,

        /// <summary>
        /// The current array is complete.
        /// </summary>
        EndArray,

        /// <summary>
        /// The next data item is a map (major type 5).
        /// </summary>
        StartMap,

        /// <summary>
        /// The current map is complete.
        /// </summary>
        EndMap,

        /// <summary>
        /// The next data item is a CBOR tag (major type 6).
        /// </summary>
        Tag,

        /// <summary>
        /// The next data item is a CBOR simple value (major type 7),
        /// other than false, true, null and undefined.
        /// </summary>
        SimpleValue,

        /// <summary>
        /// The next data item is a boolean value.
        /// </summary>
        Boolean,

        /// <summary>
        /// The next data item is a CBOR null value.
        /// </summary>
        Null,

        /// <summary>
        /// The next data item is a CBOR undefined value.
        /// </summary>
        Undefined,

        /// <summary>
        /// The next data item is a half-precision floating-point number.
        /// </summary>
        HalfPrecisionFloat,

        /// <summary>
        /// The next data item is a single-precision floating-point number.
        /// </summary>
        SinglePrecisionFloat,

        /// <summary>
        /// The next data item is a double-precision floating-point number.
        /// </summary>
        DoublePrecisionFloat,

        /// <summary>
        /// The top-level data item was read completely.
        /// </summary>
        Finished

    }

}
