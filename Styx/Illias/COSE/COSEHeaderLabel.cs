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
    /// The labels of the IANA "COSE Header Parameters" registry
    /// [RFC 9052, Section 13].
    /// A label is either an integer or a text string, therefore labels are
    /// plain CBOR values here rather than a numeric type of their own.
    /// </summary>
    public static class COSEHeaderLabel
    {

        #region Well-known labels

        /// <summary>
        /// The cryptographic algorithm to use (label 1). Whenever it appears
        /// within the unprotected header, it is not covered by the signature
        /// and thus must not be trusted.
        /// </summary>
        public static CBORValue  Algorithm             { get; } = CBORValue.FromInt64( 1);

        /// <summary>
        /// The header parameters a recipient is required to understand
        /// (label 2). Whenever a label listed here is not processed by the
        /// recipient, the whole message must be rejected [RFC 9052, Section 3.1].
        /// </summary>
        public static CBORValue  Critical              { get; } = CBORValue.FromInt64( 2);

        /// <summary>
        /// The content type of the payload (label 3), either a CoAP
        /// Content-Format integer or a media type text string.
        /// </summary>
        public static CBORValue  ContentType           { get; } = CBORValue.FromInt64( 3);

        /// <summary>
        /// A hint identifying the key used (label 4). A key identifier is an
        /// opaque byte string and carries no trust by itself.
        /// </summary>
        public static CBORValue  KeyIdentifier         { get; } = CBORValue.FromInt64( 4);

        /// <summary>
        /// The full initialization vector (label 5).
        /// </summary>
        public static CBORValue  IV                    { get; } = CBORValue.FromInt64( 5);

        /// <summary>
        /// The partial initialization vector (label 6).
        /// </summary>
        public static CBORValue  PartialIV             { get; } = CBORValue.FromInt64( 6);

        /// <summary>
        /// A counter signature (label 7), superseded by
        /// CounterSignatureV2 [RFC 9338].
        /// </summary>
        public static CBORValue  CounterSignature      { get; } = CBORValue.FromInt64( 7);

        /// <summary>
        /// A counter signature (label 11) [RFC 9338].
        /// </summary>
        public static CBORValue  CounterSignatureV2    { get; } = CBORValue.FromInt64(11);

        /// <summary>
        /// A counter signature without headers (label 12) [RFC 9338].
        /// </summary>
        public static CBORValue  CounterSignature0V2   { get; } = CBORValue.FromInt64(12);

        /// <summary>
        /// An unordered bag of X.509 certificates (label 32) [RFC 9360].
        /// </summary>
        public static CBORValue  X5Bag                 { get; } = CBORValue.FromInt64(32);

        /// <summary>
        /// An ordered chain of X.509 certificates (label 33) [RFC 9360],
        /// starting with the certificate holding the signing key.
        /// </summary>
        public static CBORValue  X5Chain               { get; } = CBORValue.FromInt64(33);

        /// <summary>
        /// The thumbprint of an X.509 certificate (label 34) [RFC 9360].
        /// </summary>
        public static CBORValue  X5T                   { get; } = CBORValue.FromInt64(34);

        /// <summary>
        /// The URI of an X.509 certificate (label 35) [RFC 9360].
        /// </summary>
        public static CBORValue  X5U                   { get; } = CBORValue.FromInt64(35);

        #endregion


        #region IsUnderstood(Label)

        /// <summary>
        /// Whether this implementation processes the given header parameter
        /// under ALL circumstances, which decides whether a message listing
        /// it within its "crit" header parameter may be accepted.
        /// The X.509 parameters are deliberately absent here: they are
        /// processed only by a verification that actually validates the
        /// chain, so
        /// COSEHeaders.VerifyCriticalHeaderParameters(..., AlsoUnderstood)
        /// takes them per call rather than this list taking them for good.
        /// Whoever verifies with a bare public key has not looked at the
        /// certificates, and must not be told otherwise.
        /// </summary>
        /// <param name="Label">A header parameter label.</param>
        public static Boolean IsUnderstood(CBORValue Label)

            => Label == Algorithm     ||
               Label == Critical      ||
               Label == ContentType   ||
               Label == KeyIdentifier;

        #endregion

        #region Name        (Label)

        /// <summary>
        /// Return a human-readable name of the given header parameter label,
        /// for use within error messages.
        /// </summary>
        /// <param name="Label">A header parameter label.</param>
        public static String Name(CBORValue Label)
        {

            if (Label == Algorithm)             return "alg";
            if (Label == Critical)              return "crit";
            if (Label == ContentType)           return "content type";
            if (Label == KeyIdentifier)         return "kid";
            if (Label == IV)                    return "IV";
            if (Label == PartialIV)             return "Partial IV";
            if (Label == CounterSignature)      return "counter signature";
            if (Label == CounterSignatureV2)    return "counter signature version 2";
            if (Label == CounterSignature0V2)   return "counter signature 0 version 2";
            if (Label == X5Bag)                 return "x5bag";
            if (Label == X5Chain)               return "x5chain";
            if (Label == X5T)                   return "x5t";
            if (Label == X5U)                   return "x5u";

            return Label.ToDiagnosticString();

        }

        #endregion

    }

}
