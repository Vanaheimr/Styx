/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Illias <https://www.github.com/Vanaheimr/Illias>
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
using System.Text.Json;
using System.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A compact JSON object abstraction optimized for many objects sharing
    /// the same property names.
    /// </summary>
    public abstract class ACustomDataNew : IEnumerable<CustomDataProperty>
    {

        #region Data

        private static readonly JsonWriterOptions DefaultWriterOptions = new() {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        #endregion

        #region Properties

        /// <summary>
        /// The number of properties.
        /// </summary>
        public abstract UInt32 Count { get; }

        /// <summary>
        /// Whether the JSON object is empty.
        /// </summary>
        public Boolean IsEmpty
            => Count == 0;

        public Boolean HasValues
            => Count > 0;

        /// <summary>
        /// Return a property value by property name, if present.
        /// </summary>
        public CustomDataValue? this[String propertyName]

            => TryGetValue(propertyName, out var value)
                   ? value
                   : null;

        /// <summary>
        /// Return a property value by property key id, if present.
        /// </summary>
        public CustomDataValue? this[Int32 propertyKeyId]

            => TryGetValue(propertyKeyId, out var value)
                   ? value
                   : null;

        #endregion


        #region (static) Empty

        /// <summary>
        /// An empty JSON object.
        /// </summary>
        public static CustomDataNew Empty
            => CustomDataNew.Empty;

        #endregion




        #region ToJSONBytes(Indented = false)

        /// <summary>
        /// Serialize this object as UTF-8 encoded JSON.
        /// </summary>
        public Byte[] ToJSONBytes(Boolean Indented = false)
        {

            using  var buffer = new ArrayBufferWriterStream();
            using (var writer = new Utf8JsonWriter(buffer, DefaultWriterOptions with {
                Indented = Indented
            }))
            {
                WriteJSON(writer);
            }

            return buffer.ToArray();

        }

        #endregion

        #region ToJSONString(Indented = false)

        /// <summary>
        /// Serialize this object as a JSON string.
        /// </summary>
        public String ToJSONString(Boolean Indented = false)

            => Encoding.UTF8.GetString(
                   ToJSONBytes(Indented)
               );

        #endregion


        #region ToRAWJSON(Indented = false)

        /// <summary>
        /// Serialize the internal representation as JSON.
        /// This is mainly intended for tests and debugging, not for public APIs.
        /// </summary>
        public String ToRAWJSON(Boolean Indented = false)
        {

            using  var buffer = new ArrayBufferWriterStream();
            using (var writer = new Utf8JsonWriter(buffer, DefaultWriterOptions with {
                Indented = Indented
            }))
            {
                WriteRAWJSON(writer);
            }

            return Encoding.UTF8.GetString(buffer.ToArray());

        }

        #endregion

        #region WriteRAWJSON(Writer)

        /// <summary>
        /// Serialize the internal representation as JSON.
        /// This is mainly intended for tests and debugging, not for public APIs.
        /// </summary>
        public void WriteRAWJSON(Utf8JsonWriter Writer)
        {

            Writer.WriteStartObject();

            Writer.WritePropertyName("keys");
            CustomDataPropertyKeyLookup.WriteRAWJSON(Writer);

            Writer.WritePropertyName("object");
            WriteRAWJSONObject(Writer);

            Writer.WriteEndObject();

        }

        #endregion

        #region WriteRAWJSONObject(Writer)

        /// <summary>
        /// Serialize this object's internal property representation as JSON.
        /// This is mainly intended for tests and debugging, not for public APIs.
        /// </summary>
        public void WriteRAWJSONObject(Utf8JsonWriter Writer)
        {

            Writer.WriteStartArray();

            foreach (var property in this)
            {
                Writer.WriteStartObject();
                Writer.WriteNumber("keyId", property.KeyId);
                Writer.WritePropertyName("value");
                property.Value.WriteRAWJSON(Writer);
                Writer.WriteEndObject();
            }

            Writer.WriteEndArray();

        }

        #endregion

        #region WriteJSON(Writer)

        /// <summary>
        /// Serialize this object via the given UTF-8 JSON writer.
        /// </summary>
        public void WriteJSON(Utf8JsonWriter Writer)
        {

            Writer.WriteStartObject();

            foreach (var property in this)
            {
                Writer.WritePropertyName(CustomDataPropertyKeyLookup.GetText(property.KeyId));
                property.Value.WriteJSON(Writer);
            }

            Writer.WriteEndObject();

        }

        #endregion





        #region ContainsKey(PropertyName)

        /// <summary>
        /// Whether a property exists.
        /// </summary>
        public Boolean ContainsKey(String PropertyName)
            => CustomDataPropertyKeyLookup.TryGetId(PropertyName, out var propertyKeyId) &&
               ContainsKey(propertyKeyId);

        #endregion

        #region ContainsKey(PropertyKeyId)

        /// <summary>
        /// Whether a property key id exists.
        /// </summary>
        public Boolean ContainsKey(Int32 PropertyKeyId)
            => TryGetValue(PropertyKeyId, out _);

        #endregion

        #region TryGetValue(PropertyName,  out Value)

        /// <summary>
        /// Try to return a property value by property name.
        /// </summary>
        public Boolean TryGetValue(String PropertyName,
                                   out CustomDataValue Value)
        {

            if (CustomDataPropertyKeyLookup.TryGetId(PropertyName, out var propertyKeyId))
                return TryGetValue(propertyKeyId, out Value);

            Value = default;
            return false;

        }

        #endregion

        #region TryGetValue(PropertyKeyId, out Value)

        /// <summary>
        /// Try to return a property value by property key id.
        /// </summary>
        public abstract Boolean TryGetValue(Int32               PropertyKeyId,
                                            out CustomDataValue  Value);

        #endregion

        #region Set(PropertyName,  Value)

        /// <summary>
        /// Return a copy of this object with the given property set.
        /// </summary>
        public virtual ACustomDataNew Set(String           PropertyName,
                                          CustomDataValue  Value)

            => Set(
                   CustomDataPropertyKeyLookup.GetOrAdd(PropertyName),
                   Value
               );

        #endregion

        #region Set(PropertyKeyId, Value)

        /// <summary>
        /// Return a copy of this object with the given property set.
        /// </summary>
        public virtual ACustomDataNew Set(Int32            PropertyKeyId,
                                          CustomDataValue  Value)

            => throw new NotSupportedException($"{GetType().Name} does not support immutable property updates.");

        #endregion


        public virtual ACustomDataNew Merge(ACustomDataNew Other)
        {

            if (Other is null || Other.IsEmpty)
                return this;

            var result = this;

            foreach (var property in Other)
                result = result.Set(property.KeyId, property.Value);

            return result;

        }

        //public virtual ACustomDataNew Merge(Newtonsoft.Json.Linq.JObject JSON)
        //{

        //    if (JSON is null || !JSON.HasValues)
        //        return this;

        //    return Merge(ParseJSON(JSON.ToString(Newtonsoft.Json.Formatting.None)));

        //}


        #region Remove(PropertyName)

        /// <summary>
        /// Return a copy of this object without the given property.
        /// </summary>
        public virtual ACustomDataNew Remove(String PropertyName)

            => CustomDataPropertyKeyLookup.TryGetId(PropertyName, out var propertyKeyId)
                   ? Remove(propertyKeyId)
                   : this;

        #endregion

        #region Remove(PropertyKeyId)

        /// <summary>
        /// Return a copy of this object without the given property key id.
        /// </summary>
        public virtual ACustomDataNew Remove(Int32 PropertyKeyId)

            => throw new NotSupportedException($"{GetType().Name} does not support immutable property removals.");

        #endregion


        #region GetEnumerator()

        public abstract IEnumerator<CustomDataProperty> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion

    }

}
