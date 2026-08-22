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

using System.Text;
using System.Buffers;
using System.Numerics;
using System.Text.Json;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// The document-level conversion between CBOR and JSON, following
    /// [RFC 8949] Section 6.1 and extending it by one rule: a metrological
    /// value (tag 44252) becomes a single JSON string in the metrological
    /// text format, e.g. "1.10 kWh" or "(230.00 ±0.12) V, k=2".
    ///
    /// A measurement therefore stays one value of the JSON document instead
    /// of falling apart into an object of four properties, every JSON tool
    /// can read and copy it, and it converts back into the very same bytes.
    ///
    /// Both JSON worlds are served: a Newtonsoft JToken tree, and UTF-8
    /// text written and read directly, without a tree in between.
    ///
    /// What round-trips byte for byte - CBOR to JSON and back - are
    /// metrological values, text strings, numbers, booleans, null, arrays
    /// and maps with text keys, given deterministic CBOR encoding.
    ///
    /// What does not, because JSON has no room for the distinction and
    /// guessing it back would be worse than losing it: byte strings (a
    /// string of Base64 is just a string), non-text map keys, binary
    /// floats (they come back as exact decimals), the points in time of
    /// tags 0 and 1 and the UUIDs of tag 37 (they come back as the strings
    /// they became), and anything rendered in diagnostic notation.
    /// </summary>
    public static class CBORJSON
    {

        #region (class)  JSONSink

        /// <summary>
        /// Where the walk over a CBOR document writes its JSON to: either a
        /// Newtonsoft tree or UTF-8 text. Having one walk and two sinks is
        /// what keeps the two outputs from drifting apart.
        /// </summary>
        private abstract class JSONSink
        {

            public abstract void WriteNull       ();
            public abstract void WriteBoolean    (Boolean     Value);
            public abstract void WriteNumber     (Int64       Value);
            public abstract void WriteNumber     (UInt64      Value);
            public abstract void WriteNumber     (BigInteger  Value);
            public abstract void WriteNumber     (Decimal     Value);
            public abstract void WriteNumber     (Double      Value);
            public abstract void WriteString     (String      Value);
            public abstract void WriteObject     (JObject     Value);
            public abstract void StartArray      ();
            public abstract void EndArray        ();
            public abstract void StartObject     ();
            public abstract void PropertyName    (String      Name);
            public abstract void EndObject       ();

        }

        #endregion

        #region (class)  JTokenSink

        /// <summary>
        /// A sink building a Newtonsoft JToken tree.
        /// </summary>
        private sealed class JTokenSink : JSONSink
        {

            private readonly Stack<JContainer>  containers     = new ();
            private          JToken?            root;
            private          String?            propertyName;

            public JToken Result
                => root ?? JValue.CreateNull();

            private void Add(JToken Token)
            {

                if (containers.Count == 0)
                    root = Token;

                else if (containers.Peek() is JObject jsonObject)
                {
                    jsonObject.Add(propertyName!, Token);
                    propertyName = null;
                }

                else
                    ((JArray) containers.Peek()).Add(Token);

            }

            private void Open(JContainer Container)
            {
                Add(Container);
                containers.Push(Container);
            }

            public override void WriteNull       ()                  => Add(JValue.CreateNull());
            public override void WriteBoolean    (Boolean     Value)  => Add(new JValue(Value));
            public override void WriteNumber     (Int64       Value)  => Add(new JValue(Value));
            public override void WriteNumber     (UInt64      Value)  => Add(new JValue(Value));
            public override void WriteNumber     (BigInteger  Value)  => Add(new JValue(Value));
            public override void WriteNumber     (Decimal     Value)  => Add(new JValue(Value));
            public override void WriteNumber     (Double      Value)  => Add(new JValue(Value));
            public override void WriteString     (String      Value)  => Add(new JValue(Value));
            public override void WriteObject     (JObject     Value)  => Add(Value);
            public override void StartArray      ()                  => Open(new JArray());
            public override void StartObject     ()                  => Open(new JObject());
            public override void PropertyName    (String      Name)   => propertyName = Name;
            public override void EndArray        ()                  => containers.Pop();
            public override void EndObject       ()                  => containers.Pop();

        }

        #endregion

        #region (class)  UTF8Sink

        /// <summary>
        /// A sink writing UTF-8 JSON text directly.
        /// Numbers are written as raw text, so that the decimal scale of a
        /// measurement survives untouched.
        /// </summary>
        private sealed class UTF8Sink(Utf8JsonWriter Writer) : JSONSink
        {

            public override void WriteNull       ()                  => Writer.WriteNullValue();
            public override void WriteBoolean    (Boolean     Value)  => Writer.WriteBooleanValue(Value);
            public override void WriteNumber     (Int64       Value)  => Writer.WriteRawValue(Value.ToString(CultureInfo.InvariantCulture),  true);
            public override void WriteNumber     (UInt64      Value)  => Writer.WriteRawValue(Value.ToString(CultureInfo.InvariantCulture),  true);
            public override void WriteNumber     (BigInteger  Value)  => Writer.WriteRawValue(Value.ToString(CultureInfo.InvariantCulture),  true);
            public override void WriteNumber     (Decimal     Value)  => Writer.WriteRawValue(Value.ToString(CultureInfo.InvariantCulture),  true);
            public override void WriteNumber     (Double      Value)  => Writer.WriteRawValue(DoubleText(Value),                             true);
            public override void WriteString     (String      Value)  => Writer.WriteStringValue(Value);
            public override void WriteObject     (JObject     Value)  => Writer.WriteRawValue(Value.ToString(Newtonsoft.Json.Formatting.None), true);
            public override void StartArray      ()                  => Writer.WriteStartArray();
            public override void EndArray        ()                  => Writer.WriteEndArray();
            public override void StartObject     ()                  => Writer.WriteStartObject();
            public override void PropertyName    (String      Name)   => Writer.WritePropertyName(Name);
            public override void EndObject       ()                  => Writer.WriteEndObject();

        }

        #endregion


        #region ToJSON    (CBOR, Options = null)

        /// <summary>
        /// Convert the given CBOR document into a Newtonsoft JSON token.
        /// </summary>
        /// <param name="CBOR">A CBOR document.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static JToken ToJSON(CBORValue         CBOR,
                                    CBORJSONOptions?  Options   = null)
        {

            var sink = new JTokenSink();

            Walk(CBOR, sink, Options ?? CBORJSONOptions.Default, 0);

            return sink.Result;

        }


        /// <summary>
        /// Convert the given encoded CBOR document into a Newtonsoft JSON token.
        /// </summary>
        /// <param name="CBOR">An encoded CBOR document.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static JToken ToJSON(ReadOnlySpan<Byte>  CBOR,
                                    CBORJSONOptions?    Options   = null)

            => ToJSON(CBORValue.Parse(CBOR), Options);

        #endregion

        #region TryToJSON (CBOR, out JSON, out ErrorResponse, Options = null)

        /// <summary>
        /// Try to convert the given encoded CBOR document into a Newtonsoft
        /// JSON token.
        /// </summary>
        /// <param name="CBOR">An encoded CBOR document.</param>
        /// <param name="JSON">The JSON representation of the document.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static Boolean TryToJSON(ReadOnlySpan<Byte>                CBOR,
                                        [NotNullWhen(true)]  out JToken?  JSON,
                                        [NotNullWhen(false)] out String?  ErrorResponse,
                                        CBORJSONOptions?                  Options   = null)
        {

            try
            {
                JSON           = ToJSON(CBOR, Options);
                ErrorResponse  = null;
                return true;
            }
            catch (Exception e)
            {
                JSON           = null;
                ErrorResponse  = e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSONUTF8(CBOR, Options = null)

        /// <summary>
        /// Convert the given CBOR document into UTF-8 encoded JSON text,
        /// without building a JSON tree in between.
        /// </summary>
        /// <param name="CBOR">A CBOR document.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static Byte[] ToJSONUTF8(CBORValue         CBOR,
                                        CBORJSONOptions?  Options   = null)
        {

            var output = new ArrayBufferWriter<Byte>();

            WriteJSONTo(output, CBOR, Options);

            return output.WrittenSpan.ToArray();

        }


        /// <summary>
        /// Convert the given encoded CBOR document into UTF-8 encoded JSON
        /// text, without building a JSON tree in between.
        /// </summary>
        /// <param name="CBOR">An encoded CBOR document.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static Byte[] ToJSONUTF8(ReadOnlySpan<Byte>  CBOR,
                                        CBORJSONOptions?    Options   = null)

            => ToJSONUTF8(CBORValue.Parse(CBOR), Options);

        #endregion

        #region ToJSONText(CBOR, Options = null)

        /// <summary>
        /// Convert the given CBOR document into JSON text.
        /// </summary>
        /// <param name="CBOR">A CBOR document.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static String ToJSONText(CBORValue         CBOR,
                                        CBORJSONOptions?  Options   = null)

            => Encoding.UTF8.GetString(ToJSONUTF8(CBOR, Options));


        /// <summary>
        /// Convert the given encoded CBOR document into JSON text.
        /// </summary>
        /// <param name="CBOR">An encoded CBOR document.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static String ToJSONText(ReadOnlySpan<Byte>  CBOR,
                                        CBORJSONOptions?    Options   = null)

            => Encoding.UTF8.GetString(ToJSONUTF8(CBORValue.Parse(CBOR), Options));

        #endregion

        #region WriteJSONTo(Output, CBOR, Options = null)

        /// <summary>
        /// Write the given CBOR document as UTF-8 encoded JSON text into
        /// the given buffer writer.
        /// </summary>
        /// <param name="Output">Where to write the JSON text to.</param>
        /// <param name="CBOR">A CBOR document.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static void WriteJSONTo(IBufferWriter<Byte>  Output,
                                       CBORValue            CBOR,
                                       CBORJSONOptions?     Options   = null)
        {

            var options = Options ?? CBORJSONOptions.Default;

            using var jsonWriter = new Utf8JsonWriter(
                                       Output,
                                       new JsonWriterOptions {
                                           Indented  = options.Indent,
                                           Encoder   = CBORJSONTextEncoder.Instance
                                       }
                                   );

            Walk(CBOR, new UTF8Sink(jsonWriter), options, 0);

            jsonWriter.Flush();

        }

        #endregion


        #region ToCBOR    (JSON,     Options = null)

        /// <summary>
        /// Convert the given Newtonsoft JSON token into a CBOR document.
        /// Every string that reads as a metrological value becomes one,
        /// unless a detector says otherwise.
        /// </summary>
        /// <param name="JSON">A JSON token.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static CBORValue ToCBOR(JToken?           JSON,
                                       CBORJSONOptions?  Options   = null)

            => FromJSON(JSON,
                        Options ?? CBORJSONOptions.Default,
                        "",
                        0);

        #endregion

        #region ToCBOR    (UTF8JSON, Options = null)

        /// <summary>
        /// Convert the given UTF-8 encoded JSON text into a CBOR document,
        /// without building a JSON tree in between. This is the exact path:
        /// every number keeps the digits it was written with.
        /// </summary>
        /// <param name="UTF8JSON">UTF-8 encoded JSON text.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static CBORValue ToCBOR(ReadOnlySpan<Byte>  UTF8JSON,
                                       CBORJSONOptions?    Options   = null)
        {

            var options  = Options ?? CBORJSONOptions.Default;
            var reader   = new Utf8JsonReader(
                               UTF8JSON,
                               new JsonReaderOptions {
                                   CommentHandling  = JsonCommentHandling.Disallow,
                                   MaxDepth         = options.MaxDepth
                               }
                           );

            if (!reader.Read())
                throw new CBORException("The given JSON document is empty!");

            var cbor = FromUTF8JSON(ref reader, options, "", 0);

            if (reader.Read())
                throw new CBORException("The given JSON document has trailing data!");

            return cbor;

        }

        #endregion

        #region ToCBOR    (JSONText, Options = null)

        /// <summary>
        /// Convert the given JSON text into a CBOR document.
        /// </summary>
        /// <param name="JSONText">JSON text.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static CBORValue ToCBOR(String            JSONText,
                                       CBORJSONOptions?  Options   = null)

            => ToCBOR(Encoding.UTF8.GetBytes(JSONText).AsSpan(), Options);

        #endregion

        #region TryToCBOR (UTF8JSON, out CBOR, out ErrorResponse, Options = null)

        /// <summary>
        /// Try to convert the given UTF-8 encoded JSON text into a CBOR
        /// document.
        /// </summary>
        /// <param name="UTF8JSON">UTF-8 encoded JSON text.</param>
        /// <param name="CBOR">The CBOR representation of the document.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="Options">Optional conversion options.</param>
        public static Boolean TryToCBOR(ReadOnlySpan<Byte>                UTF8JSON,
                                        out CBORValue                     CBOR,
                                        [NotNullWhen(false)] out String?  ErrorResponse,
                                        CBORJSONOptions?                  Options   = null)
        {

            try
            {
                CBOR           = ToCBOR(UTF8JSON, Options);
                ErrorResponse  = null;
                return true;
            }
            catch (Exception e)
            {
                CBOR           = CBORValue.Null;
                ErrorResponse  = e.Message;
                return false;
            }

        }

        #endregion


        #region (private static) Walk           (CBOR, Sink, Options, Depth)

        /// <summary>
        /// Walk a CBOR document and write it to the given JSON sink.
        /// </summary>
        private static void Walk(CBORValue        CBOR,
                                 JSONSink         Sink,
                                 CBORJSONOptions  Options,
                                 Int32            Depth)
        {

            if (Depth > Options.MaxDepth)
                throw new CBORException($"The CBOR document is nested deeper than the allowed {Options.MaxDepth} levels!");

            switch (CBOR.Kind)
            {

                #region Null, undefined, booleans

                case CBORValueKind.Null:
                    Sink.WriteNull();
                    return;

                case CBORValueKind.Boolean:
                    Sink.WriteBoolean(CBOR.AsBoolean());
                    return;

                #endregion

                #region Integers

                case CBORValueKind.UnsignedInteger:
                    {

                        var value = CBOR.AsUInt64();

                        if (value <= Int64.MaxValue)
                            Sink.WriteNumber((Int64) value);
                        else
                            Sink.WriteNumber(value);

                        return;

                    }

                case CBORValueKind.NegativeInteger:
                    {

                        if (CBOR.TryGetInt64(out var value))
                            Sink.WriteNumber(value);
                        else
                            Sink.WriteNumber(CBOR.AsBigInteger());

                        return;

                    }

                #endregion

                #region Binary floating-point numbers

                case CBORValueKind.HalfFloat:
                case CBORValueKind.SingleFloat:
                case CBORValueKind.DoubleFloat:
                    {

                        var value = CBOR.AsDouble();

                        // JSON has no NaN and no infinities.
                        if (Double.IsFinite(value))
                            Sink.WriteNumber(value);
                        else
                            WriteUncovered(CBOR, Sink, Options, "a non-finite floating-point number");

                        return;

                    }

                #endregion

                #region Strings

                case CBORValueKind.ByteString:
                    Sink.WriteString(RenderBytes(CBOR.AsBytes(), Options));
                    return;

                case CBORValueKind.TextString:
                    Sink.WriteString(CBOR.AsText());
                    return;

                #endregion

                #region Arrays

                case CBORValueKind.Array:
                    {

                        Sink.StartArray();

                        foreach (var item in CBOR.AsArray())
                            Walk(item, Sink, Options, Depth + 1);

                        Sink.EndArray();

                        return;

                    }

                #endregion

                #region Maps

                case CBORValueKind.Map:
                    {

                        Sink.StartObject();

                        var names = new HashSet<String>();

                        foreach (var entry in CBOR.AsMap())
                        {

                            var name = MapKeyText(entry.Key, Options);

                            if (!names.Add(name))
                                throw new CBORException($"The JSON object would carry the property '{name}' twice!");

                            Sink.PropertyName(name);

                            Walk(entry.Value, Sink, Options, Depth + 1);

                        }

                        Sink.EndObject();

                        return;

                    }

                #endregion

                #region Tagged data items

                case CBORValueKind.Tagged:
                    {

                        var tag = CBOR.Tag;

                        #region Tag 44252: a metrological value

                        if (tag == CBORTag.MetrologicalValue &&
                            Options.Metrology != CBORJSONMetrology.None)
                        {

                            if (!MetrologicalValue.TryParse(CBOR, out var metrologicalValue, out var errorResponse))
                                throw new CBORException($"Invalid metrological value: {errorResponse}");

                            if (Options.Metrology == CBORJSONMetrology.Text)
                                Sink.WriteString(metrologicalValue.ToString());
                            else
                                Sink.WriteObject(metrologicalValue.ToJSON());

                            return;

                        }

                        #endregion

                        #region Tag 55799: self-described CBOR is transparent

                        if (tag == CBORTag.SelfDescribedCBOR)
                        {
                            Walk(CBOR.UntaggedValue, Sink, Options, Depth + 1);
                            return;
                        }

                        #endregion

                        #region Tags 0 and 1: points in time

                        if (tag == CBORTag.DateTimeString &&
                            CBOR.UntaggedValue.TryGetText(out var timestamp))
                        {
                            Sink.WriteString(timestamp);
                            return;
                        }

                        if (tag == CBORTag.EpochDateTime &&
                            CBOR.UntaggedValue.TryGetDecimal(out var epoch))
                        {
                            Sink.WriteString(DateTimeOffset.FromUnixTimeMilliseconds((Int64) (epoch * 1000)).UtcDateTime.ToISO8601());
                            return;
                        }

                        #endregion

                        #region Tags 2, 3 and 4: exact numbers

                        if (tag == CBORTag.UnsignedBignum ||
                            tag == CBORTag.NegativeBignum)
                        {
                            Sink.WriteNumber(CBOR.AsBigInteger());
                            return;
                        }

                        if (tag == CBORTag.DecimalFraction &&
                            CBOR.TryGetDecimal(out var decimalFraction))
                        {
                            Sink.WriteNumber(decimalFraction);
                            return;
                        }

                        #endregion

                        #region Tag 37: a UUID

                        if (tag == CBORTag.UUID &&
                            CBOR.UntaggedValue.TryGetBytes(out var uuid) &&
                            uuid.Length == 16)
                        {
                            Sink.WriteString(new Guid(uuid, true).ToString());
                            return;
                        }

                        #endregion

                        #region Tags 32, 33, 34 and 36: text that is already text

                        if ((tag == CBORTag.URI       ||
                             tag == CBORTag.Base64URL ||
                             tag == CBORTag.Base64    ||
                             tag == CBORTag.MIMEMessage) &&
                            CBOR.UntaggedValue.TryGetText(out var text))
                        {
                            Sink.WriteString(text);
                            return;
                        }

                        #endregion

                        WriteUncovered(CBOR, Sink, Options, $"the tag {tag}");
                        return;

                    }

                #endregion

                default:
                    WriteUncovered(CBOR, Sink, Options, $"a data item of kind '{CBOR.Kind}'");
                    return;

            }

        }

        #endregion

        #region (private static) WriteUncovered (CBOR, Sink, Options, What)

        /// <summary>
        /// Write a CBOR data item this JSON profile does not cover.
        /// </summary>
        private static void WriteUncovered(CBORValue        CBOR,
                                           JSONSink         Sink,
                                           CBORJSONOptions  Options,
                                           String           What)
        {

            if (Options.UnknownItems == CBORJSONUnknownItems.Diagnostic)
            {
                Sink.WriteString(CBOR.ToDiagnosticString());
                return;
            }

            throw new CBORException($"This JSON profile does not cover {What}!");

        }

        #endregion

        #region (private static) MapKeyText     (Key, Options)

        /// <summary>
        /// The name of the JSON property a CBOR map key becomes.
        /// </summary>
        private static String MapKeyText(CBORValue        Key,
                                         CBORJSONOptions  Options)
        {

            if (Key.TryGetText(out var text))
                return text;

            if (!Options.StringifyMapKeys)
                throw new CBORException($"A CBOR map key of kind '{Key.Kind}' is not a JSON property name! (Consider CBORJSONOptions.StringifyMapKeys)");

            if (Key.TryGetInt64(out var number))
                return number.ToString(CultureInfo.InvariantCulture);

            return Key.ToDiagnosticString();

        }

        #endregion

        #region (private static) RenderBytes    (Bytes, Options)

        /// <summary>
        /// The text a CBOR byte string becomes.
        /// </summary>
        private static String RenderBytes(Byte[]           Bytes,
                                          CBORJSONOptions  Options)

            => Options.ByteStrings switch {
                   CBORJSONByteStrings.Base64  => Convert.ToBase64String(Bytes),
                   CBORJSONByteStrings.Hex     => Convert.ToHexString(Bytes).ToLowerInvariant(),
                   _                           => Bytes.ToBase64URL()
               };

        #endregion

        #region (private static) DoubleText     (Value)

        /// <summary>
        /// A binary floating-point number as JSON text. It always carries a
        /// decimal point, so that it does not silently read back as an
        /// integer.
        /// </summary>
        private static String DoubleText(Double Value)
        {

            var text = Value.ToString("R", CultureInfo.InvariantCulture);

            return text.AsSpan().IndexOfAny('.', 'e', 'E') < 0
                       ? text + ".0"
                       : text;

        }

        #endregion


        #region (private static) FromJSON       (JSON, Options, Path, Depth)

        /// <summary>
        /// Convert a Newtonsoft JSON token into a CBOR data item.
        /// </summary>
        private static CBORValue FromJSON(JToken?          JSON,
                                          CBORJSONOptions  Options,
                                          String           Path,
                                          Int32            Depth)
        {

            if (Depth > Options.MaxDepth)
                throw new CBORException($"The JSON document is nested deeper than the allowed {Options.MaxDepth} levels!");

            if (JSON is null)
                return CBORValue.Null;

            switch (JSON.Type)
            {

                #region JSON objects

                case JTokenType.Object:
                    {

                        var jsonObject = (JObject) JSON;

                        // The JSON object form of a metrological value converts
                        // back just as its text form does.
                        if (Options.Metrology == CBORJSONMetrology.Object &&
                            jsonObject.ContainsKey("value")               &&
                            jsonObject.ContainsKey("unit")                &&
                            MetrologicalValue.TryParse(jsonObject, out var metrologicalValue, out _))
                        {
                            return metrologicalValue.ToCBOR();
                        }

                        var entries = new List<KeyValuePair<CBORValue, CBORValue>>();

                        foreach (var property in jsonObject.Properties())
                            entries.Add(new KeyValuePair<CBORValue, CBORValue>(
                                            CBORValue.FromText(property.Name),
                                            FromJSON(property.Value, Options, PathOf(Path, property.Name, Options), Depth + 1)
                                        ));

                        return CBORValue.FromMap(entries);

                    }

                #endregion

                #region JSON arrays

                case JTokenType.Array:
                    {

                        var items  = new List<CBORValue>();
                        var index  = 0;

                        foreach (var item in (JArray) JSON)
                            items.Add(FromJSON(item, Options, PathOf(Path, (index++).ToString(CultureInfo.InvariantCulture), Options), Depth + 1));

                        return CBORValue.FromArray(items);

                    }

                #endregion

                #region JSON scalars

                case JTokenType.Null:
                case JTokenType.Undefined:
                    return CBORValue.Null;

                case JTokenType.Boolean:
                    return CBORValue.FromBoolean(JSON.Value<Boolean>());

                case JTokenType.Integer:
                    {

                        var value = ((JValue) JSON).Value;

                        return value switch {
                                   BigInteger bigInteger  => CBORValue.FromBigInteger(bigInteger),
                                   UInt64     unsigned    => CBORValue.FromUInt64(unsigned),
                                   _                      => CBORValue.FromInt64(JSON.Value<Int64>())
                               };

                    }

                case JTokenType.Float:
                    {

                        var value = ((JValue) JSON).Value;

                        // A decimal keeps its scale and becomes an exact decimal
                        // fraction; only an actual binary float stays one.
                        return value switch {
                                   Decimal number  => CBORValue.FromDecimal(number),
                                   Single single   => CBORValue.FromDouble(single),
                                   _               => CBORValue.FromDouble(JSON.Value<Double>())
                               };

                    }

                case JTokenType.String:
                    return FromJSONString(JSON.Value<String>() ?? "", Options, Path);

                case JTokenType.Date:
                    return CBORValue.Tagged(
                               CBORTag.DateTimeString,
                               CBORValue.FromText(
                                   JSON.Value<DateTimeOffset>().UtcDateTime.ToISO8601()
                               )
                           );

                case JTokenType.Bytes:
                    return CBORValue.FromBytes((Byte[]) ((JValue) JSON).Value!);

                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                    return CBORValue.FromText(JSON.Value<String>() ?? "");

                #endregion

                default:
                    throw new CBORException($"A JSON token of type '{JSON.Type}' does not convert into CBOR!");

            }

        }

        #endregion

        #region (private static) FromUTF8JSON   (Reader, Options, Path, Depth)

        /// <summary>
        /// Convert UTF-8 encoded JSON text into a CBOR data item, reading
        /// the numbers exactly as they were written.
        /// </summary>
        private static CBORValue FromUTF8JSON(ref Utf8JsonReader  Reader,
                                              CBORJSONOptions     Options,
                                              String              Path,
                                              Int32               Depth)
        {

            if (Depth > Options.MaxDepth)
                throw new CBORException($"The JSON document is nested deeper than the allowed {Options.MaxDepth} levels!");

            switch (Reader.TokenType)
            {

                case JsonTokenType.Null:
                    return CBORValue.Null;

                case JsonTokenType.True:
                    return CBORValue.True;

                case JsonTokenType.False:
                    return CBORValue.False;

                case JsonTokenType.String:
                    return FromJSONString(Reader.GetString() ?? "", Options, Path);

                case JsonTokenType.Number:
                    return FromJSONNumber(NumberText(ref Reader));

                #region JSON arrays

                case JsonTokenType.StartArray:
                    {

                        var items  = new List<CBORValue>();
                        var index  = 0;

                        while (Reader.Read() && Reader.TokenType != JsonTokenType.EndArray)
                            items.Add(FromUTF8JSON(ref Reader, Options, PathOf(Path, (index++).ToString(CultureInfo.InvariantCulture), Options), Depth + 1));

                        return CBORValue.FromArray(items);

                    }

                #endregion

                #region JSON objects

                case JsonTokenType.StartObject:
                    {

                        var entries = new List<KeyValuePair<CBORValue, CBORValue>>();

                        while (Reader.Read() && Reader.TokenType != JsonTokenType.EndObject)
                        {

                            var name = Reader.GetString() ?? "";

                            if (!Reader.Read())
                                throw new CBORException($"The JSON property '{name}' has no value!");

                            entries.Add(new KeyValuePair<CBORValue, CBORValue>(
                                            CBORValue.FromText(name),
                                            FromUTF8JSON(ref Reader, Options, PathOf(Path, name, Options), Depth + 1)
                                        ));

                        }

                        // A metrological value written as a JSON object converts
                        // back just as its text form does.
                        if (Options.Metrology == CBORJSONMetrology.Object &&
                            HasTextKey(entries, "value")                  &&
                            HasTextKey(entries, "unit"))
                        {

                            var jsonObject = new JObject();

                            foreach (var entry in entries)
                                jsonObject.Add(entry.Key.AsText(), ToJSON(entry.Value, Options));

                            if (MetrologicalValue.TryParse(jsonObject, out var metrologicalValue, out _))
                                return metrologicalValue.ToCBOR();

                        }

                        return CBORValue.FromMap(entries);

                    }

                #endregion

                default:
                    throw new CBORException($"Unexpected JSON token '{Reader.TokenType}'!");

            }

        }

        #endregion

        #region (private static) HasTextKey     (Entries, Name)

        /// <summary>
        /// Whether the given map entries carry a text key of that name.
        /// </summary>
        private static Boolean HasTextKey(List<KeyValuePair<CBORValue, CBORValue>>  Entries,
                                          String                                    Name)

            => Entries.Any(entry => entry.Key.Kind == CBORValueKind.TextString &&
                                    entry.Key.AsText() == Name);

        #endregion

        #region (private static) NumberText     (Reader)

        /// <summary>
        /// The digits of a JSON number, exactly as they are written.
        /// </summary>
        private static String NumberText(ref Utf8JsonReader Reader)

            => Encoding.UTF8.GetString(
                   Reader.HasValueSequence
                       ? Reader.ValueSequence.ToArray()
                       : Reader.ValueSpan
               );

        #endregion

        #region (private static) FromJSONNumber (Text)

        /// <summary>
        /// Convert the digits of a JSON number into a CBOR data item,
        /// without ever going through a binary floating-point number:
        /// an integer becomes a CBOR integer or a bignum, everything else
        /// an exact, scale-preserving decimal fraction (tag 4).
        /// </summary>
        private static CBORValue FromJSONNumber(String Text)
        {

            #region Integers

            if (Text.AsSpan().IndexOfAny('.', 'e', 'E') < 0)
            {

                if (Int64.TryParse(Text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var integer))
                    return CBORValue.FromInt64(integer);

                if (BigInteger.TryParse(Text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var bigInteger))
                    return CBORValue.FromBigInteger(bigInteger);

                throw new CBORException($"'{Text}' is not a valid JSON number!");

            }

            #endregion

            #region Everything else: an exact decimal fraction

            var digits    = Text;
            var exponent  = 0;
            var e         = digits.AsSpan().IndexOfAny('e', 'E');

            if (e >= 0)
            {

                if (!Int32.TryParse(digits[(e + 1)..], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out exponent))
                    throw new CBORException($"'{Text}' is not a valid JSON number!");

                digits = digits[..e];

            }

            var point = digits.IndexOf('.');

            if (point >= 0)
            {
                exponent  -= digits.Length - point - 1;
                digits     = String.Concat(digits.AsSpan(0, point), digits.AsSpan(point + 1));
            }

            if (!BigInteger.TryParse(digits, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var mantissa))
                throw new CBORException($"'{Text}' is not a valid JSON number!");

            // The exponent of a decimal fraction is a CBOR integer, but a
            // document that claims 10^100000 only wants to make us allocate.
            if (exponent < -65536 || exponent > 65536)
                throw new CBORException($"The decimal exponent of '{Text}' is out of range!");

            // A decimal fraction states decimal places and its exponent is
            // negative on the wire; an exponent that leaves none denotes the
            // integer it equals (metrological-text.md, Section 3.2).
            if (exponent >= 0)
                return CBORValue.FromBigInteger(mantissa * BigInteger.Pow(10, exponent));

            return CBORValue.Tagged(
                       CBORTag.DecimalFraction,
                       CBORValue.FromArray(
                           CBORValue.FromInt64(exponent),
                           CBORValue.FromBigInteger(mantissa)
                       )
                   );

            #endregion

        }

        #endregion

        #region (private static) FromJSONString (Text, Options, Path)

        /// <summary>
        /// Convert a JSON string into a CBOR data item: a metrological value
        /// when it reads as one, a text string otherwise.
        /// </summary>
        private static CBORValue FromJSONString(String           Text,
                                                CBORJSONOptions  Options,
                                                String           Path)
        {

            // The predicate decides where there is one; otherwise Readings
            // does, and it says None unless the caller said otherwise. A
            // caller who passes nothing gets no guesses: what a wrong guess
            // produces here is a well-formed reading of something nobody
            // measured, and nothing downstream can tell.
            var examine = Options.DetectMetrologicalValues is not null
                              ? Options.DetectMetrologicalValues(Path, Text)
                              : Options.Readings == CBORJSONReadings.Auto;

            if (Options.Metrology == CBORJSONMetrology.Text &&
                examine &&
                MetrologicalValue.TryParse(Text, out var metrologicalValue, out _))
            {
                return metrologicalValue.ToCBOR();
            }

            return CBORValue.FromText(Text);

        }

        #endregion

        #region (private static) PathOf         (Path, Token, Options)

        /// <summary>
        /// The JSON Pointer ([RFC 6901]) of a token within the document.
        /// It is only assembled when somebody is going to look at it.
        /// </summary>
        private static String PathOf(String           Path,
                                     String           Token,
                                     CBORJSONOptions  Options)

            => Options.DetectMetrologicalValues is null
                   ? Path
                   : $"{Path}/{Token.Replace("~", "~0").Replace("/", "~1")}";

        #endregion

    }

}
