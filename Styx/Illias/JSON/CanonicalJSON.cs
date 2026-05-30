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
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Deterministic RFC 8259 JSON serialization for signature input.
    /// </summary>
    /// <remarks>
    /// Objects are emitted with ordinally sorted property names, arrays keep their
    /// original order, and no insignificant whitespace is written. This is meant
    /// as a stable JSON byte representation; it is intentionally strict about
    /// values which are not representable as RFC 8259 JSON.
    /// </remarks>
    public static class CanonicalJSON
    {

        private static readonly JsonWriterOptions writerOptions = new() {
            Indented  = false,
            Encoder   = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };


        #region Serialize(Token)

        public static String Serialize(JToken Token)
        {

            using var stream = new MemoryStream();

            using (var writer = new Utf8JsonWriter(stream, writerOptions))
            {
                WriteNewtonsoftToken(writer, Token);
            }

            return Encoding.UTF8.GetString(stream.ToArray());

        }

        #endregion

        #region Serialize(JSONObject)

        public static String Serialize(JObject JSONObject)

            => Serialize((JToken) JSONObject);

        #endregion

        #region Serialize(JSONArray)

        public static String Serialize(JArray JSONArray)

            => Serialize((JToken) JSONArray);

        #endregion

        #region Serialize(JSONDocument)

        public static String Serialize(JsonDocument JSONDocument)

            => Serialize(JSONDocument.RootElement);

        #endregion

        #region Serialize(JSONElement)

        public static String Serialize(JsonElement JSONElement)
        {

            using var stream = new MemoryStream();

            using (var writer = new Utf8JsonWriter(stream, writerOptions))
            {
                WriteJsonElement(writer, JSONElement);
            }

            return Encoding.UTF8.GetString(stream.ToArray());

        }

        #endregion

        #region Serialize(JSONNode)

        public static String Serialize(JsonNode? JSONNode)
        {

            if (JSONNode is null)
                return "null";

            using var document = JsonDocument.Parse(JSONNode.ToJsonString());
            return Serialize(document);

        }

        #endregion


        #region ToUTF8Bytes(Token)

        public static Byte[] ToUTF8Bytes(JToken Token)

            => Encoding.UTF8.GetBytes(Serialize(Token));

        #endregion

        #region ToUTF8Bytes(JSONDocument)

        public static Byte[] ToUTF8Bytes(JsonDocument JSONDocument)

            => Encoding.UTF8.GetBytes(Serialize(JSONDocument));

        #endregion

        #region ToUTF8Bytes(JSONElement)

        public static Byte[] ToUTF8Bytes(JsonElement JSONElement)

            => Encoding.UTF8.GetBytes(Serialize(JSONElement));

        #endregion

        #region ToUTF8Bytes(JSONNode)

        public static Byte[] ToUTF8Bytes(JsonNode? JSONNode)

            => Encoding.UTF8.GetBytes(Serialize(JSONNode));

        #endregion


        #region WriteNewtonsoftToken(Writer, Token)

        private static void WriteNewtonsoftToken(Utf8JsonWriter Writer, JToken Token)
        {

            switch (Token.Type)
            {

                case JTokenType.Object:
                    Writer.WriteStartObject();
                    foreach (var property in ((JObject) Token).Properties().OrderBy(property => property.Name, StringComparer.Ordinal))
                    {
                        Writer.WritePropertyName(property.Name);
                        WriteNewtonsoftToken(Writer, property.Value);
                    }
                    Writer.WriteEndObject();
                    break;

                case JTokenType.Array:
                    Writer.WriteStartArray();
                    foreach (var item in (JArray) Token)
                        WriteNewtonsoftToken(Writer, item);
                    Writer.WriteEndArray();
                    break;

                case JTokenType.Property:
                    var jProperty = (JProperty) Token;
                    Writer.WritePropertyName(jProperty.Name);
                    WriteNewtonsoftToken(Writer, jProperty.Value);
                    break;

                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Date:
                case JTokenType.Bytes:
                    WriteNewtonsoftValue(Writer, (JValue) Token);
                    break;

                default:
                    throw new NotSupportedException($"JToken type '{Token.Type}' can not be represented as canonical RFC 8259 JSON.");

            }

        }

        #endregion

        #region WriteNewtonsoftValue(Writer, Value)

        private static void WriteNewtonsoftValue(Utf8JsonWriter Writer, JValue Value)
        {

            var value = Value.Value;

            if (value is null || Value.Type == JTokenType.Null)
            {
                Writer.WriteNullValue();
                return;
            }

            switch (Value.Type)
            {

                case JTokenType.Boolean:
                    Writer.WriteBooleanValue(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
                    break;

                case JTokenType.String:
                    Writer.WriteStringValue(Convert.ToString(value, CultureInfo.InvariantCulture));
                    break;

                case JTokenType.Integer:
                    WriteIntegerValue(Writer, value);
                    break;

                case JTokenType.Float:
                    WriteFloatValue(Writer, value);
                    break;

                case JTokenType.Date:
                    Writer.WriteStringValue(value switch {
                        DateTimeOffset dateTimeOffset  => dateTimeOffset.ToString("o", CultureInfo.InvariantCulture),
                        DateTime       dateTime        => dateTime.      ToString("o", CultureInfo.InvariantCulture),
                        _                              => Convert.ToString(value, CultureInfo.InvariantCulture)
                    });
                    break;

                case JTokenType.Bytes:
                    Writer.WriteStringValue(Convert.ToBase64String((Byte[]) value));
                    break;

                default:
                    throw new NotSupportedException($"JValue type '{Value.Type}' can not be represented as canonical RFC 8259 JSON.");

            }

        }

        #endregion

        #region WriteJsonElement(Writer, Element)

        private static void WriteJsonElement(Utf8JsonWriter Writer, JsonElement Element)
        {

            switch (Element.ValueKind)
            {

                case JsonValueKind.Object:
                    Writer.WriteStartObject();
                    foreach (var property in Element.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
                    {
                        Writer.WritePropertyName(property.Name);
                        WriteJsonElement(Writer, property.Value);
                    }
                    Writer.WriteEndObject();
                    break;

                case JsonValueKind.Array:
                    Writer.WriteStartArray();
                    foreach (var item in Element.EnumerateArray())
                        WriteJsonElement(Writer, item);
                    Writer.WriteEndArray();
                    break;

                case JsonValueKind.String:
                    Writer.WriteStringValue(Element.GetString());
                    break;

                case JsonValueKind.Number:
                    Writer.WriteRawValue(Element.GetRawText(), skipInputValidation: false);
                    break;

                case JsonValueKind.True:
                    Writer.WriteBooleanValue(true);
                    break;

                case JsonValueKind.False:
                    Writer.WriteBooleanValue(false);
                    break;

                case JsonValueKind.Null:
                    Writer.WriteNullValue();
                    break;

                default:
                    throw new NotSupportedException($"JsonElement value kind '{Element.ValueKind}' can not be represented as canonical RFC 8259 JSON.");

            }

        }

        #endregion

        #region WriteIntegerValue(Writer, Value)

        private static void WriteIntegerValue(Utf8JsonWriter Writer, Object Value)
        {

            switch (Value)
            {

                case SByte sByteValue:
                    Writer.WriteNumberValue(sByteValue);
                    break;

                case Byte byteValue:
                    Writer.WriteNumberValue(byteValue);
                    break;

                case Int16 int16Value:
                    Writer.WriteNumberValue(int16Value);
                    break;

                case UInt16 uint16Value:
                    Writer.WriteNumberValue(uint16Value);
                    break;

                case Int32 int32Value:
                    Writer.WriteNumberValue(int32Value);
                    break;

                case UInt32 uint32Value:
                    Writer.WriteNumberValue(uint32Value);
                    break;

                case Int64 int64Value:
                    Writer.WriteNumberValue(int64Value);
                    break;

                case UInt64 uint64Value:
                    Writer.WriteRawValue(uint64Value.ToString(CultureInfo.InvariantCulture), skipInputValidation: false);
                    break;

                case BigInteger bigIntegerValue:
                    Writer.WriteRawValue(bigIntegerValue.ToString(CultureInfo.InvariantCulture), skipInputValidation: false);
                    break;

                default:
                    Writer.WriteRawValue(Convert.ToString(Value, CultureInfo.InvariantCulture)!, skipInputValidation: false);
                    break;

            }

        }

        #endregion

        #region WriteFloatValue(Writer, Value)

        private static void WriteFloatValue(Utf8JsonWriter Writer, Object Value)
        {

            switch (Value)
            {

                case Decimal decimalValue:
                    Writer.WriteNumberValue(decimalValue);
                    break;

                case Double doubleValue:
                    if (!Double.IsFinite(doubleValue))
                        throw new NotSupportedException("Non-finite floating point values can not be represented as RFC 8259 JSON.");
                    Writer.WriteNumberValue(doubleValue);
                    break;

                case Single singleValue:
                    if (!Single.IsFinite(singleValue))
                        throw new NotSupportedException("Non-finite floating point values can not be represented as RFC 8259 JSON.");
                    Writer.WriteNumberValue(singleValue);
                    break;

                default:
                    Writer.WriteRawValue(Convert.ToString(Value, CultureInfo.InvariantCulture)!, skipInputValidation: false);
                    break;

            }

        }

        #endregion



        #region Sign_ECDSA_SHA256 (Plaintext, PrivateKey)

        public static Byte[] Sign_ECDSA_SHA256(String                  Plaintext,
                                               ECPrivateKeyParameters  PrivateKey)
        {

            var sha256Hash  = SHA256.HashData(Plaintext.ToUTF8Bytes());

            var signer      = SignerUtilities.GetSigner("NONEwithECDSA");
            signer.Init(true, PrivateKey);
            signer.BlockUpdate(sha256Hash, 0, sha256Hash.Length);

            return signer.GenerateSignature();

        }

        #endregion

        #region Sign_ECDSA_SHA512 (Plaintext, PrivateKey)

        public static Byte[] Sign_ECDSA_SHA512(String                  Plaintext,
                                               ECPrivateKeyParameters  PrivateKey)
        {

            var sha256Hash  = SHA512.HashData(Plaintext.ToUTF8Bytes());

            var signer      = SignerUtilities.GetSigner("NONEwithECDSA");
            signer.Init(true, PrivateKey);
            signer.BlockUpdate(sha256Hash, 0, sha256Hash.Length);

            return signer.GenerateSignature();

        }

        #endregion

    }

}
