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

using System.Collections;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A bucket of COSE header parameters [RFC 9052, Section 3].
    ///
    /// A COSE message carries two of these: The protected bucket, which is
    /// covered by the signature, and the unprotected bucket, which is not.
    /// Only what is protected can be trusted after a successful verification.
    ///
    /// Header parameters are immutable here, because the serialization of the
    /// protected bucket is part of the signature input and must not change
    /// after signing.
    /// </summary>
    public sealed class COSEHeaders : IEnumerable<KeyValuePair<CBORValue, CBORValue>>
    {

        #region Data

        private readonly KeyValuePair<CBORValue, CBORValue>[] parameters;

        #endregion

        #region Properties

        /// <summary>
        /// All header parameters, in the order they were given or read.
        /// </summary>
        public IReadOnlyList<KeyValuePair<CBORValue, CBORValue>>  Parameters

            => parameters;

        /// <summary>
        /// The number of header parameters.
        /// </summary>
        public Int32                                              Count

            => parameters.Length;

        /// <summary>
        /// Whether this bucket has no header parameters at all.
        /// </summary>
        public Boolean                                            IsEmpty

            => parameters.Length == 0;

        /// <summary>
        /// The cryptographic algorithm (label 1), or null when absent
        /// or not an integer.
        /// </summary>
        public COSEAlgorithm?                                     Algorithm

            => TryGet(COSEHeaderLabel.Algorithm, out var value) &&
               COSEAlgorithm.TryParse(value, out var algorithm, out _)
                   ? algorithm
                   : null;

        /// <summary>
        /// The key identifier (label 4), or null when absent
        /// or not a byte string.
        /// </summary>
        public Byte[]?                                            KeyIdentifier

            => TryGet(COSEHeaderLabel.KeyIdentifier, out var value) &&
               value.TryGetBytes(out var keyIdentifier)
                   ? keyIdentifier
                   : null;

        /// <summary>
        /// The content type (label 3), either a CoAP Content-Format integer
        /// or a media type text string, or null when absent.
        /// </summary>
        public CBORValue?                                         ContentType

            => TryGet(COSEHeaderLabel.ContentType, out var value)
                   ? (CBORValue?) value
                   : null;

        /// <summary>
        /// The labels a recipient is required to understand (label 2),
        /// or null when absent.
        /// </summary>
        public IReadOnlyList<CBORValue>?                          Critical

            => TryGet(COSEHeaderLabel.Critical, out var value) &&
               value.Kind == CBORValueKind.Array
                   ? value.AsArray()
                   : null;

        #endregion

        #region Constructor(s)

        #region COSEHeaders(params Parameters)

        /// <summary>
        /// Create a new bucket of COSE header parameters.
        /// </summary>
        /// <param name="Parameters">The header parameters.</param>
        public COSEHeaders(params (CBORValue Label, CBORValue Value)[] Parameters)

            : this(Parameters.Select(static parameter => new KeyValuePair<CBORValue, CBORValue>(parameter.Label,
                                                                                               parameter.Value)))

        { }

        #endregion

        #region COSEHeaders(Parameters)

        /// <summary>
        /// Create a new bucket of COSE header parameters.
        /// </summary>
        /// <param name="Parameters">The header parameters.</param>
        public COSEHeaders(IEnumerable<KeyValuePair<CBORValue, CBORValue>> Parameters)
        {

            this.parameters = Parameters.ToArray();

            var labels = new HashSet<CBORValue>();

            foreach (var parameter in this.parameters)
            {
                if (!labels.Add(parameter.Key))
                    throw new COSEException($"The COSE header parameter '{COSEHeaderLabel.Name(parameter.Key)}' was given more than once!");
            }

        }

        #endregion

        #endregion


        #region Static defaults

        /// <summary>
        /// A bucket without any header parameters. As a protected bucket
        /// it is serialized as a zero-length byte string.
        /// </summary>
        public static COSEHeaders  Empty    { get; } = new ();

        #endregion


        #region (static) Create  (Algorithm = null, KeyIdentifier = null)

        /// <summary>
        /// Create a bucket holding the two header parameters almost every
        /// signed COSE message uses.
        /// </summary>
        /// <param name="Algorithm">An optional cryptographic algorithm (label 1).</param>
        /// <param name="KeyIdentifier">An optional key identifier (label 4).</param>
        public static COSEHeaders Create(COSEAlgorithm?  Algorithm       = null,
                                         Byte[]?         KeyIdentifier   = null)
        {

            var parameters = new List<KeyValuePair<CBORValue, CBORValue>>();

            if (Algorithm.HasValue)
                parameters.Add(new (COSEHeaderLabel.Algorithm,
                                    Algorithm.Value.ToCBOR()));

            if (KeyIdentifier is not null)
                parameters.Add(new (COSEHeaderLabel.KeyIdentifier,
                                    CBORValue.FromBytes(KeyIdentifier)));

            return new COSEHeaders(parameters);

        }

        #endregion

        #region (static) Parse   (CBOR)

        /// <summary>
        /// Parse the given CBOR map as a bucket of COSE header parameters.
        /// </summary>
        /// <param name="CBOR">A CBOR map of header parameters.</param>
        public static COSEHeaders Parse(CBORValue CBOR)
        {

            if (TryParse(CBOR, out var headers, out var errorResponse))
                return headers;

            throw new COSEException(errorResponse);

        }

        #endregion

        #region (static) TryParse(CBOR,          out Headers, out ErrorResponse)

        /// <summary>
        /// Try to parse the given CBOR map as a bucket of COSE header parameters.
        /// </summary>
        /// <param name="CBOR">A CBOR map of header parameters.</param>
        /// <param name="Headers">The parsed header parameters.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CBORValue                         CBOR,
                                       [NotNullWhen(true)]  out COSEHeaders?  Headers,
                                       [NotNullWhen(false)] out String?       ErrorResponse)
        {

            Headers = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = $"A bucket of COSE header parameters must be a CBOR map, but was a CBOR {CBOR.Kind}!";
                return false;
            }

            try
            {

                Headers        = new COSEHeaders(CBOR.AsMap());
                ErrorResponse  = null;

                return true;

            }
            catch (Exception e)
            {

                ErrorResponse = e.Message;

                return false;

            }

        }

        #endregion

        #region (static) TryParseProtected(ProtectedBytes, out Headers, out ErrorResponse)

        /// <summary>
        /// Try to parse the serialized protected bucket of a COSE message.
        /// A zero-length byte string is an empty bucket and NOT an encoded
        /// empty map - it is the one encoding that must never be "repaired"
        /// into a0, because the signature covers these exact bytes
        /// [RFC 9052, Section 3].
        /// This is deliberately not an overload of TryParse: A byte array
        /// converts implicitly into a CBOR byte string as well, and the two
        /// readings of the same argument mean very different things here.
        /// </summary>
        /// <param name="ProtectedBytes">The serialized protected bucket.</param>
        /// <param name="Headers">The parsed header parameters.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseProtected(ReadOnlySpan<Byte>                     ProtectedBytes,
                                                [NotNullWhen(true)]  out COSEHeaders?  Headers,
                                                [NotNullWhen(false)] out String?       ErrorResponse)
        {

            if (ProtectedBytes.Length == 0)
            {
                Headers        = Empty;
                ErrorResponse  = null;
                return true;
            }

            if (!CBORValue.TryParse(ProtectedBytes, out var cbor, out ErrorResponse))
            {
                Headers = null;
                return false;
            }

            return TryParse(cbor, out Headers, out ErrorResponse);

        }

        #endregion


        #region TryGet  (Label, out Value)

        /// <summary>
        /// Try to get the value of the given header parameter.
        /// </summary>
        /// <param name="Label">A header parameter label.</param>
        /// <param name="Value">The value of the header parameter.</param>
        public Boolean TryGet(CBORValue Label, out CBORValue Value)
        {

            foreach (var parameter in parameters)
            {
                if (parameter.Key == Label)
                {
                    Value = parameter.Value;
                    return true;
                }
            }

            Value = CBORValue.Null;
            return false;

        }

        #endregion

        #region Contains(Label)

        /// <summary>
        /// Whether the given header parameter is present within this bucket.
        /// </summary>
        /// <param name="Label">A header parameter label.</param>
        public Boolean Contains(CBORValue Label)

            => TryGet(Label, out _);

        #endregion


        #region ToCBOR()

        /// <summary>
        /// Return a CBOR map representation of these header parameters.
        /// </summary>
        public CBORValue ToCBOR()

            => CBORValue.FromMap(parameters);

        #endregion

        #region ToProtectedByteArray()

        /// <summary>
        /// Return the serialization of these header parameters for use as the
        /// PROTECTED bucket of a COSE message, which is a zero-length byte
        /// string when the bucket is empty, and the encoded map otherwise
        /// [RFC 9052, Section 3].
        /// These are the exact bytes the signature is computed over, so they
        /// must be transmitted and stored verbatim.
        /// </summary>
        public Byte[] ToProtectedByteArray()

            => IsEmpty
                   ? []
                   : ToCBOR().ToByteArray();

        #endregion


        #region IEnumerable Members

        /// <summary>
        /// Return an enumerator of all header parameters.
        /// </summary>
        public IEnumerator<KeyValuePair<CBORValue, CBORValue>> GetEnumerator()

            => ((IEnumerable<KeyValuePair<CBORValue, CBORValue>>) parameters).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()

            => parameters.GetEnumerator();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => IsEmpty
                   ? "no header parameters"
                   : ToCBOR().ToDiagnosticString();

        #endregion

    }

}
