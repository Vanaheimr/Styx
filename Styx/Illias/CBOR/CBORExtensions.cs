/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Illias <https://www.github.com/Vanaheimr/Illias>
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

using System.Text.Json;
using System.Formats.Cbor;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public static class CborToJsonConverter
    {

        public static JObject? CBOR2JSON(this Byte[] cborData)
        {

            //var reader      = new CborReader(cborData);
            //var parsedData  = ParseCborElement(reader);
            //var json        = JsonSerializer.Serialize(
            //                      parsedData,
            //                      new JsonSerializerOptions { WriteIndented = true }
            //                  );

            //return JObject.Parse(json);

            var parsedData = ParseCBORElement(new CborReader(cborData));

            return parsedData is not null
                       ? JObject.FromObject(parsedData)
                       : null;

        }

        public static Object? ParseCBORElement(CborReader reader)
        {
            switch (reader.PeekState())
            {
                case CborReaderState.StartArray:
                    var list = new List<object?>();
                    int? arrayLength = reader.ReadStartArray();
                    while (reader.PeekState() != CborReaderState.EndArray)
                    {
                        list.Add(ParseCBORElement(reader));
                    }
                    reader.ReadEndArray();
                    return list;

                case CborReaderState.StartMap:
                    var dict = new Dictionary<string, object?>();
                    int? mapLength = reader.ReadStartMap();
                    while (reader.PeekState() != CborReaderState.EndMap)
                    {
                        // Schlüssel auslesen – in JSON müssen Schlüssel Strings sein.
                        object? key = ParseCBORElement(reader);
                        string keyStr = key?.ToString() ?? string.Empty;
                        object? value = ParseCBORElement(reader);
                        dict[keyStr] = value;
                    }
                    reader.ReadEndMap();
                    return dict;

                case CborReaderState.UnsignedInteger:
                    return reader.ReadUInt64();

                case CborReaderState.NegativeInteger:
                    return reader.ReadInt64();

                case CborReaderState.TextString:
                    return reader.ReadTextString();

                case CborReaderState.ByteString:
                    // Optional: ByteArrays können z.B. in Base64 kodiert werden, hier erfolgt direktes Lesen.
                    return reader.ReadByteString();

                case CborReaderState.SinglePrecisionFloat:
                    return reader.ReadSingle();

                case CborReaderState.DoublePrecisionFloat:
                    return reader.ReadDouble();

                case CborReaderState.Boolean:
                    return reader.ReadBoolean();

                case CborReaderState.Null:
                    reader.ReadNull();
                    return null;

                default:
                    throw new NotSupportedException($"CBOR-Typ {reader.PeekState()} wird nicht unterstützt.");
            }
        }



        // A simple CBOR map decoder returning a JObject.
        // This assumes that keys are integers (as is typical in a COSE_Key).
        public static JObject DecodeCborMap(byte[] cbor)
        {
            var reader = new CborReader(cbor);
            var mapLen = reader.ReadStartMap();
            var obj = new JObject();
            for (int i = 0; i < mapLen; i++)
            {

                // Read key (assumed to be an integer)
                int key = reader.ReadInt32();

                // For the value, support a few common types.
                switch (reader.PeekState())
                {
                    case CborReaderState.UnsignedInteger:
                    case CborReaderState.NegativeInteger:
                        obj[key.ToString()] = reader.ReadInt64();
                        break;
                    case CborReaderState.TextString:
                        obj[key.ToString()] = reader.ReadTextString();
                        break;
                    case CborReaderState.ByteString:
                        var bytes = reader.ReadByteString();
                        obj[key.ToString()] = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                        break;
                    default:
                        throw new NotSupportedException("Unsupported CBOR type in credentialPublicKey.");
                }

            }
            reader.ReadEndMap();
            return obj;
        }

    }

}
