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
    /// The eight major types of the Concise Binary Object Representation (CBOR),
    /// as defined in RFC 8949, Section 3.1.
    /// </summary>
    public enum CBORMajorType : Byte
    {

        /// <summary>
        /// An unsigned integer in the range 0..2^64-1 (major type 0).
        /// </summary>
        UnsignedInteger  = 0,

        /// <summary>
        /// A negative integer in the range -2^64..-1 (major type 1).
        /// The encoded argument n represents the value -1-n.
        /// </summary>
        NegativeInteger  = 1,

        /// <summary>
        /// A byte string (major type 2).
        /// </summary>
        ByteString       = 2,

        /// <summary>
        /// A text string encoded as UTF-8 (major type 3).
        /// </summary>
        TextString       = 3,

        /// <summary>
        /// An array of data items (major type 4).
        /// </summary>
        Array            = 4,

        /// <summary>
        /// A map of pairs of data items (major type 5).
        /// </summary>
        Map              = 5,

        /// <summary>
        /// A tagged data item (major type 6).
        /// </summary>
        Tag              = 6,

        /// <summary>
        /// Floating-point numbers, simple values and the "break" stop code (major type 7).
        /// </summary>
        Simple           = 7

    }

}
