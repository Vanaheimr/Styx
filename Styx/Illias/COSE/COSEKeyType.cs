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
    /// The key type of a COSE key, as registered within the IANA
    /// "COSE Key Types" registry [RFC 9052, Section 13].
    /// </summary>
    public enum COSEKeyType
    {

        /// <summary>
        /// This value is reserved and never valid within a COSE key.
        /// </summary>
        Reserved     = 0,

        /// <summary>
        /// Octet Key Pair: An elliptic curve key of the curves
        /// Ed25519, Ed448, X25519 or X448.
        /// </summary>
        OKP          = 1,

        /// <summary>
        /// An elliptic curve key with both an x and a y coordinate,
        /// e.g. on the curves P-256, P-384, P-521 or brainpoolP256r1.
        /// </summary>
        EC2          = 2,

        /// <summary>
        /// An RSA key [RFC 8230].
        /// </summary>
        RSA          = 3,

        /// <summary>
        /// A symmetric key.
        /// </summary>
        Symmetric    = 4,

        /// <summary>
        /// A public key for the HSS/LMS hash-based digital
        /// signature algorithm [RFC 8778].
        /// </summary>
        HSS_LMS      = 5,

        /// <summary>
        /// A WalnutDSA public key.
        /// </summary>
        WalnutDSA    = 6

    }

}
