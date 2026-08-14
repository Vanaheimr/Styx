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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A mutable builder for CBOR maps, preserving the insertion order
    /// and supporting collection initializers:
    /// new CBORMap { { "key", 1 }, { 2, "value" } }.
    /// Keys may be of any CBOR kind.
    /// </summary>
    public class CBORMap : IEnumerable<KeyValuePair<CBORValue, CBORValue>>
    {

        #region Data

        private readonly List<KeyValuePair<CBORValue, CBORValue>> entries = [];

        #endregion

        #region Properties

        /// <summary>
        /// The number of key/value pairs within this CBOR map builder.
        /// </summary>
        public Int32  Count
            => entries.Count;

        #endregion


        #region Add(Key, Value)

        /// <summary>
        /// Add the given CBOR key/value pair to this CBOR map builder.
        /// </summary>
        /// <param name="Key">A map key of any CBOR kind.</param>
        /// <param name="Value">A CBOR value.</param>
        public void Add(CBORValue  Key,
                        CBORValue  Value)
        {
            entries.Add(new KeyValuePair<CBORValue, CBORValue>(Key, Value));
        }

        #endregion

        #region ToValue()

        /// <summary>
        /// Return this CBOR map builder as an immutable CBOR value.
        /// </summary>
        public CBORValue ToValue()

            => CBORValue.FromMap(entries);

        #endregion

        #region (implicit) CBORValue(CBORMap)

        /// <summary>
        /// Convert the given CBOR map builder into an immutable CBOR value.
        /// </summary>
        /// <param name="CBORMap">A CBOR map builder.</param>
        public static implicit operator CBORValue(CBORMap CBORMap)

            => CBORMap.ToValue();

        #endregion


        #region IEnumerable Members

        /// <summary>
        /// Enumerate the key/value pairs of this CBOR map builder.
        /// </summary>
        public IEnumerator<KeyValuePair<CBORValue, CBORValue>> GetEnumerator()
            => entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => entries.GetEnumerator();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ToValue().ToDiagnosticString();

        #endregion

    }

}
