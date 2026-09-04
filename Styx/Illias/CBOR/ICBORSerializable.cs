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

using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The interface of all data structures that can be serialized to
    /// and parsed from CBOR (RFC 8949).
    /// </summary>
    /// <typeparam name="TSelf">The type of the data structure.</typeparam>
    public interface ICBORSerializable<TSelf>
        where TSelf : ICBORSerializable<TSelf>
    {

        /// <summary>
        /// Return the CBOR representation of this data structure.
        /// </summary>
        /// <param name="CustomSerializer">An optional delegate to customize the CBOR representation.</param>
        CBORValue ToCBOR(CustomCBORSerializerDelegate<TSelf>? CustomSerializer = null);


        /// <summary>
        /// Try to parse the given CBOR value as such a data structure.
        /// </summary>
        /// <param name="CBOR">The CBOR value to be parsed.</param>
        /// <param name="Value">The parsed data structure.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        static abstract Boolean TryParse(CBORValue                         CBOR,
                                         out TSelf                         Value,
                                         [NotNullWhen(false)] out String?  ErrorResponse);

    }

}
