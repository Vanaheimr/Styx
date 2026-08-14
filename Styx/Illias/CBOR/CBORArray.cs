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
    /// A mutable builder for CBOR arrays,
    /// supporting collection initializers:
    /// new CBORArray { 1, "text", true }.
    /// </summary>
    public class CBORArray : IEnumerable<CBORValue>
    {

        #region Data

        private readonly List<CBORValue> items = [];

        #endregion

        #region Properties

        /// <summary>
        /// The number of CBOR values within this CBOR array builder.
        /// </summary>
        public Int32  Count
            => items.Count;

        #endregion


        #region Add(Item)

        /// <summary>
        /// Add the given CBOR value to this CBOR array builder.
        /// </summary>
        /// <param name="Item">A CBOR value.</param>
        public void Add(CBORValue Item)
        {
            items.Add(Item);
        }

        #endregion

        #region ToValue()

        /// <summary>
        /// Return this CBOR array builder as an immutable CBOR value.
        /// </summary>
        public CBORValue ToValue()

            => CBORValue.FromArray(items);

        #endregion

        #region (implicit) CBORValue(CBORArray)

        /// <summary>
        /// Convert the given CBOR array builder into an immutable CBOR value.
        /// </summary>
        /// <param name="CBORArray">A CBOR array builder.</param>
        public static implicit operator CBORValue(CBORArray CBORArray)

            => CBORArray.ToValue();

        #endregion


        #region IEnumerable Members

        /// <summary>
        /// Enumerate the CBOR values of this CBOR array builder.
        /// </summary>
        public IEnumerator<CBORValue> GetEnumerator()
            => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => items.GetEnumerator();

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
