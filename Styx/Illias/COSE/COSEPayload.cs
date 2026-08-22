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
    /// The payload of a COSE message is an opaque byte string: RFC 9052 says
    /// nothing about what is inside it, and a signature covers those bytes
    /// rather than their meaning.
    ///
    /// This class is about the one thing an application very often does want
    /// said about it: that it is CBOR, and that it is written in the
    /// deterministic encoding of RFC 8949, Section 4.2.1. The reason is not
    /// tidiness but forwarding. A receiver that parses a signed record and
    /// serializes it again gets the bytes ITS encoder produces - map entries
    /// sorted, heads at their shortest, no indefinite lengths - and not the
    /// bytes it was handed. Where the signer wrote a different spelling of
    /// the same data, the forwarded signature no longer verifies, and nothing
    /// in the message says why: a signature can not tell being tampered with
    /// from being retyped.
    ///
    /// Signing the deterministic encoding removes the second spelling. There
    /// is then one way to write the record down, and everybody who parses and
    /// re-encodes it arrives at the very bytes the signature covers.
    /// </summary>
    public static class COSEPayload
    {

        #region TryCanonicalize(Payload, out CanonicalPayload)

        /// <summary>
        /// Try to rewrite the given payload in the deterministic CBOR
        /// encoding of RFC 8949, Section 4.2.1.
        ///
        /// Returns false whenever the payload is not one well-formed CBOR
        /// data item and nothing else, which is not an error: text, JSON, an
        /// image or a detached hash have no canonical CBOR form to be
        /// rewritten into, and a COSE payload is allowed to be any of them.
        /// </summary>
        /// <param name="Payload">The payload of a COSE message.</param>
        /// <param name="CanonicalPayload">The deterministic encoding of the payload.</param>
        public static Boolean TryCanonicalize(Byte[]                          Payload,
                                              [NotNullWhen(true)] out Byte[]? CanonicalPayload)
        {

            // Read leniently on purpose: the non-deterministic spelling is
            // exactly what this method exists to accept and rewrite. Reading
            // it strictly would refuse the only input it can help with.
            if (CBORValue.TryParse(Payload, out var cbor, out _))
            {
                CanonicalPayload = cbor.ToByteArray(CBORWriterOptions.Canonical);
                return true;
            }

            CanonicalPayload = null;
            return false;

        }

        #endregion

        #region Canonicalize   (Payload)

        /// <summary>
        /// Rewrite the given payload in the deterministic CBOR encoding of
        /// RFC 8949, Section 4.2.1, or return it unchanged where it is not
        /// one well-formed CBOR data item and nothing else.
        /// </summary>
        /// <param name="Payload">The payload of a COSE message.</param>
        public static Byte[] Canonicalize(Byte[] Payload)

            => TryCanonicalize(Payload, out var canonicalPayload)
                   ? canonicalPayload
                   : Payload;

        #endregion

        #region IsCanonical    (Payload)

        /// <summary>
        /// Whether the given payload survives being parsed and written again:
        /// whether it is one well-formed CBOR data item whose deterministic
        /// encoding is the payload itself.
        ///
        /// This is deliberately the round-trip and not merely a reader with
        /// RequireDeterministic set. What the forwarding receiver actually
        /// does is decode and re-encode, so the question worth answering is
        /// whether that changes anything - not whether a rule list is
        /// satisfied.
        ///
        /// A payload that is not CBOR is not canonical: there is nothing here
        /// to have an opinion about.
        /// </summary>
        /// <param name="Payload">The payload of a COSE message.</param>
        public static Boolean IsCanonical(Byte[] Payload)

            => TryCanonicalize(Payload, out var canonicalPayload) &&
               canonicalPayload.AsSpan().SequenceEqual(Payload);

        #endregion

    }

}
