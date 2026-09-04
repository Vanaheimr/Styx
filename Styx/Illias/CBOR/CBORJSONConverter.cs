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

using System.Globalization;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A converter from CBOR to JSON, e.g. for WebAuthn attestation
    /// objects and COSE keys.
    /// The class keeps the name and the observable behavior of its
    /// former System.Formats.Cbor-based implementation: Within
    /// CBOR2JSON byte strings surface as JValue(Byte[]) and therefore
    /// serialize as Base64 text, while DecodeCborMap renders byte
    /// strings as lowercase hex.
    /// </summary>
    public static class CborToJsonConverter
    {

        #region CBOR2JSON   (this CBORData)

        /// <summary>
        /// Convert the given CBOR data into a JSON object.
        /// The top-level data item must be a CBOR map or null.
        /// </summary>
        /// <param name="CBORData">The encoded CBOR data.</param>
        public static JObject? CBOR2JSON(this Byte[] CBORData)
        {

            var cbor = CBORValue.Parse(CBORData);

            if (cbor.Kind == CBORValueKind.Null)
                return null;

            if (cbor.Kind != CBORValueKind.Map)
                throw new NotSupportedException($"The top-level CBOR data item must be a map, but is of kind '{cbor.Kind}'!");

            return (JObject) ToJSON(cbor);

        }

        #endregion

        #region DecodeCborMap(CBOR)

        /// <summary>
        /// Decode a CBOR map with integer keys, e.g. a COSE_Key holding
        /// a WebAuthn credential public key, into a JSON object.
        /// Byte strings are rendered as lowercase hex.
        /// </summary>
        /// <param name="CBOR">The encoded CBOR data.</param>
        public static JObject DecodeCborMap(Byte[] CBOR)
        {

            var reader  = new CBORReader(CBOR);
            var pairs   = reader.ReadStartMap();
            var json    = new JObject();

            for (var i = 0; pairs.HasValue ? i < pairs.Value : reader.PeekState() != CBORReaderState.EndMap; i++)
            {

                var key = reader.ReadInt64().ToString(CultureInfo.InvariantCulture);

                switch (reader.PeekState())
                {

                    case CBORReaderState.UnsignedInteger:
                    case CBORReaderState.NegativeInteger:
                        json[key] = reader.ReadInt64();
                        break;

                    case CBORReaderState.TextString:
                        json[key] = reader.ReadTextString();
                        break;

                    case CBORReaderState.ByteString:
                        json[key] = reader.ReadByteString().ToHexString();
                        break;

                    default:
                        throw new NotSupportedException("Unsupported CBOR type in credentialPublicKey.");

                }

            }

            reader.ReadEndMap();

            return json;

        }

        #endregion


        #region (private static) ToJSON(CBOR)

        private static JToken ToJSON(CBORValue CBOR)
        {

            switch (CBOR.Kind)
            {

                case CBORValueKind.Null:
                    return JValue.CreateNull();

                case CBORValueKind.Boolean:
                    return new JValue(CBOR.AsBoolean());

                case CBORValueKind.UnsignedInteger:
                    return new JValue(CBOR.AsUInt64());

                case CBORValueKind.NegativeInteger:
                    return new JValue(CBOR.AsInt64());

                case CBORValueKind.ByteString:
                    return new JValue(CBOR.AsBytes());

                case CBORValueKind.TextString:
                    return new JValue(CBOR.AsText());

                case CBORValueKind.HalfFloat:
                case CBORValueKind.SingleFloat:

                    // Note: Half-precision values are an improvement over the
                    // former implementation, which did not support them at all!
                    return new JValue((Single) CBOR.AsDouble());

                case CBORValueKind.DoubleFloat:
                    return new JValue(CBOR.AsDouble());

                case CBORValueKind.Array:
                {

                    var jsonArray = new JArray();

                    foreach (var item in CBOR.AsArray())
                        jsonArray.Add(ToJSON(item));

                    return jsonArray;

                }

                case CBORValueKind.Map:
                {

                    var jsonObject = new JObject();

                    foreach (var entry in CBOR.AsMap())
                        jsonObject[KeyToText(entry.Key)] = ToJSON(entry.Value);

                    return jsonObject;

                }

                default:
                    throw new NotSupportedException($"The CBOR kind '{CBOR.Kind}' is not supported by CBOR2JSON!");

            }

        }

        #endregion

        #region (private static) KeyToText(Key)

        private static String KeyToText(CBORValue Key)

            => Key.Kind switch {
                   CBORValueKind.TextString       => Key.AsText(),
                   CBORValueKind.UnsignedInteger  => Key.AsUInt64().ToString(CultureInfo.InvariantCulture),
                   CBORValueKind.NegativeInteger  => Key.AsInt128().ToString(null, CultureInfo.InvariantCulture),
                   _                              => Key.ToDiagnosticString()
               };

        #endregion

    }

}
