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
    /// Which signature machinery a COSE algorithm needs.
    ///
    /// The distinction that earns a type of its own is ECDSA against the other
    /// two: ECDSA signs a message DIGEST, chosen by the algorithm, whereas
    /// EdDSA and ML-DSA are pure and take the message itself. Handing a pure
    /// signer a digest produces a signature that is perfectly valid for that
    /// digest and that no other implementation will ever accept - a failure
    /// with no symptom at all until two implementations meet.
    /// </summary>
    public enum COSEAlgorithmFamily
    {

        /// <summary>
        /// Not a signature algorithm: a bare message digest, or one this
        /// library only recognizes in order to refuse it by name.
        /// </summary>
        None,

        /// <summary>
        /// ECDSA over an elliptic curve [RFC 9053, RFC 9864]. Signs the
        /// digest; the signature is the concatenation of r and s.
        /// </summary>
        ECDSA,

        /// <summary>
        /// EdDSA on Ed25519 or Ed448 [RFC 8032, RFC 9053 Section 2.2]. Signs
        /// the message, and derives its nonce from the key and the message,
        /// so it is deterministic without being asked.
        /// </summary>
        EdDSA,

        /// <summary>
        /// ML-DSA, the post-quantum scheme [FIPS 204, RFC 9964]. Signs the
        /// message. Its key is a key pair of an algorithm rather than a point
        /// on a curve, which is why it needs a key type of its own.
        /// </summary>
        MLDSA

    }

}
