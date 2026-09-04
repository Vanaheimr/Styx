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
        MLDSA,

        /// <summary>
        /// HMAC [RFC 2104, RFC 9053 Section 3.1], which is not a signature
        /// machinery at all but a message authentication one - and the
        /// difference is the reason it is listed here rather than folded in
        /// beside the others.
        ///
        /// A MAC is SYMMETRIC: whoever can verify one can produce one. It
        /// therefore answers "did this come from someone holding the key",
        /// which is a question only a key holder can ask, and it never
        /// answers "did this come from THAT party" to anybody else. A
        /// signature does. That is why a metrological record is signed and
        /// never MACed - the customer, the operator and the regulator all
        /// have to be able to check a reading, and none of them may be able
        /// to manufacture one.
        ///
        /// Sign and Verify refuse an algorithm of this family outright, so
        /// that a MAC can never be mistaken for a signature by an API that
        /// accepts both.
        /// </summary>
        HMAC,

        /// <summary>
        /// AES-GCM [RFC 9053, Section 4.1], a content ENCRYPTION algorithm.
        /// An AEAD: it produces ciphertext plus an authentication tag, and
        /// authenticates the Enc_structure alongside without encrypting it.
        ///
        /// Note what it does NOT give: an encrypted message says nothing about
        /// who sent it. AEAD integrity means "whoever holds this key wrote
        /// this", which with several recipients means any of them. RFC 9052
        /// Section 8.3 puts it as "either no or very limited data
        /// origination". A signed payload inside an encrypted envelope is how
        /// one gets both.
        /// </summary>
        AESGCM,

        /// <summary>
        /// AES key wrap [RFC 9053 Section 6.2.1, RFC 3394], a RECIPIENT
        /// algorithm: it carries a content key rather than content.
        ///
        /// Deterministic, and deliberately so - the same key wrapped under the
        /// same key-encryption key is always the same bytes. That is safe only
        /// because what it wraps is a uniformly random key rather than a
        /// message, which RFC 3394 relies on squarely.
        /// </summary>
        KeyWrap,

        /// <summary>
        /// The recipient algorithm that transports nothing [RFC 9053, Section
        /// 6.1.1]: the recipient key IS the content key. Its protected bucket
        /// and its ciphertext must both be empty, which is what makes a
        /// COSE_Mac with one direct recipient a COSE_Mac0 with ceremony.
        /// </summary>
        Direct

    }

}
