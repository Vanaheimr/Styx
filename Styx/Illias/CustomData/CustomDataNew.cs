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

using System.Text;
using System.Text.Json;

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Concrete compact JSON object representation using a sorted property array.
    /// </summary>
    public sealed class CustomDataNew : ACustomDataNew
    {

        #region Data

        private readonly CustomDataProperty[] properties;

        #endregion

        #region Properties

        /// <summary>
        /// The number of properties.
        /// </summary>
        public override UInt32 Count
            => (UInt32) properties.Length;

        #endregion

        #region Constructor(s)

        public CustomDataNew(IEnumerable<CustomDataProperty>? Properties = null)
        {
            properties = Normalize(Properties);
        }

        private CustomDataNew(CustomDataProperty[]  Properties,
                              Boolean               AlreadyNormalized)
        {
            properties = AlreadyNormalized
                             ? Properties
                             : Normalize(Properties);
        }

        #endregion


        #region (static) Empty

        /// <summary>
        /// An empty JSON object.
        /// </summary>
        public static new CustomDataNew Empty { get; }
            = new([], true);

        #endregion

        #region (static) ParseJSON(JSON)

        /// <summary>
        /// Parse an UTF-8 encoded JSON object.
        /// </summary>
        public static CustomDataNew ParseJSON(Byte[] JSON)
            => ParseJSON(JSON.AsSpan());

        /// <summary>
        /// Parse a JSON object from a .NET string.
        /// Prefer the UTF-8 overloads for hot paths, because strings are UTF-16
        /// and need an additional UTF-8 encoding step before parsing.
        /// </summary>
        public static CustomDataNew ParseJSON(String JSON)
        {

            ArgumentNullException.ThrowIfNull(JSON);

            return ParseJSON(Encoding.UTF8.GetBytes(JSON));

        }

        /// <summary>
        /// Parse an UTF-8 encoded JSON object.
        /// </summary>
        public static CustomDataNew ParseJSON(ReadOnlyMemory<Byte> JSON)
            => ParseJSON(JSON.Span);

        /// <summary>
        /// Parse an UTF-8 encoded JSON object.
        /// </summary>
        public static CustomDataNew ParseJSON(ReadOnlySpan<Byte> JSON)
        {

            var reader = new Utf8JsonReader(JSON, true, default);

            if (!reader.Read())
                throw new JsonException("Expected a JSON object.");

            var value = CustomDataValue.ParseJSON(ref reader);

            if (value.Kind != CustomDataValueKind.Object || value.ObjectValue is null)
                throw new JsonException("Expected a JSON object.");

            if (value.ObjectValue is not CustomDataNew customDataObject)
                throw new JsonException("Expected a parsed custom data object.");

            if (reader.Read())
                throw new JsonException("Unexpected content after the JSON object.");

            return customDataObject;

        }

        /// <summary>
        /// Try to parse a JSON object from a .NET string.
        /// Prefer the UTF-8 overloads for hot paths, because strings are UTF-16
        /// and need an additional UTF-8 encoding step before parsing.
        /// </summary>
        public static Boolean TryParseJSON(String              JSON,
                                           out CustomDataNew? CustomData,
                                           out String?         ErrorResponse)
        {

            ArgumentNullException.ThrowIfNull(JSON);

            return TryParseJSON(Encoding.UTF8.GetBytes(JSON),
                                out CustomData,
                                out ErrorResponse);

        }

        /// <summary>
        /// Try to parse an UTF-8 encoded JSON object.
        /// </summary>
        public static Boolean TryParseJSON(ReadOnlySpan<Byte> JSON,
                                           out CustomDataNew? CustomData,
                                           out String?         ErrorResponse)
        {

            try
            {
                CustomData     = ParseJSON(JSON);
                ErrorResponse  = null;
                return true;
            }
            catch (Exception e)
            {
                CustomData     = null;
                ErrorResponse  = e.Message;
                return false;
            }

        }

        #endregion


        #region TryGetValue(PropertyKeyId, out Value)

        /// <summary>
        /// Try to return a property value by property key id.
        /// </summary>
        public override Boolean TryGetValue(Int32               PropertyKeyId,
                                            out CustomDataValue  Value)
        {

            var index = FindPropertyIndex(PropertyKeyId);

            if (index >= 0)
            {
                Value = properties[index].Value;
                return true;
            }

            Value = default;
            return false;

        }

        #endregion

        #region Set(PropertyName,  Value)

        /// <summary>
        /// Return a copy of this object with the given property set.
        /// </summary>
        public override CustomDataNew Set(String           PropertyName,
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
        public override CustomDataNew Set(Int32            PropertyKeyId,
                                          CustomDataValue  Value)
        {

            var index = FindPropertyIndex(PropertyKeyId);

            if (index >= 0)
            {
                var updatedProperties = (CustomDataProperty[]) properties.Clone();
                updatedProperties[index] = new CustomDataProperty(PropertyKeyId, Value);
                return new CustomDataNew(updatedProperties, true);
            }

            var newProperties = new CustomDataProperty[properties.Length + 1];
            var insertAt      = ~index;

            Array.Copy(properties, 0, newProperties, 0, insertAt);
            newProperties[insertAt] = new CustomDataProperty(PropertyKeyId, Value);
            Array.Copy(properties, insertAt, newProperties, insertAt + 1, properties.Length - insertAt);

            return new CustomDataNew(newProperties, true);

        }

        #endregion

        #region Remove(PropertyName)

        /// <summary>
        /// Return a copy of this object without the given property.
        /// </summary>
        public override CustomDataNew Remove(String PropertyName)
            => CustomDataPropertyKeyLookup.TryGetId(PropertyName, out var propertyKeyId)
                   ? Remove(propertyKeyId)
                   : this;

        #endregion

        #region Remove(PropertyKeyId)

        /// <summary>
        /// Return a copy of this object without the given property key id.
        /// </summary>
        public override CustomDataNew Remove(Int32 PropertyKeyId)
        {

            var index = FindPropertyIndex(PropertyKeyId);

            if (index < 0)
                return this;

            if (properties.Length == 1)
                return Empty;

            var newProperties = new CustomDataProperty[properties.Length - 1];

            Array.Copy(properties, 0, newProperties, 0, index);
            Array.Copy(properties, index + 1, newProperties, index, properties.Length - index - 1);

            return new CustomDataNew(newProperties, true);

        }

        #endregion

        #region GetEnumerator()

        public override IEnumerator<CustomDataProperty> GetEnumerator()
            => ((IEnumerable<CustomDataProperty>) properties).GetEnumerator();

        #endregion



        public CustomDataNew Merge(CustomDataNew Other)
        {

            if (Other is null || Other.IsEmpty)
                return this;

            var result = this;

            foreach (var property in Other)
                result = result.Set(property.KeyId, property.Value);

            return result;

        }


        public static implicit operator CustomDataNew(Newtonsoft.Json.Linq.JObject JSONObject)

            => JSONObject is not null
                   ? ParseJSON(JSONObject.ToString(Newtonsoft.Json.Formatting.None))
                   : null;

        public static implicit operator Newtonsoft.Json.Linq.JObject(CustomDataNew JSONObject)

            => JSONObject is not null
                   ? Newtonsoft.Json.Linq.JObject.Parse(JSONObject.ToJSONString())
                   : null;


        #region (private) FindPropertyIndex(PropertyKeyId)

        private Int32 FindPropertyIndex(Int32 PropertyKeyId)
        {

            var lower = 0;
            var upper = properties.Length - 1;

            while (lower <= upper)
            {

                var index = lower + ((upper - lower) / 2);
                var keyId = properties[index].KeyId;

                if (keyId == PropertyKeyId)
                    return index;

                if (keyId < PropertyKeyId)
                    lower = index + 1;
                else
                    upper = index - 1;

            }

            return ~lower;

        }

        #endregion

        #region (internal static) Normalize(Properties)

        internal static CustomDataProperty[] Normalize(IEnumerable<CustomDataProperty>? Properties)
        {

            if (Properties is null)
                return [];

            var properties = new List<CustomDataProperty>();
            var indexes    = new Dictionary<Int32, Int32>();

            foreach (var property in Properties)
            {

                if (indexes.TryGetValue(property.KeyId, out var index))
                    properties[index] = property;
                else
                {
                    indexes.Add(property.KeyId, properties.Count);
                    properties.Add(property);
                }

            }

            if (properties.Count == 0)
                return [];

            properties.Sort(static (a, b) => a.KeyId.CompareTo(b.KeyId));

            return properties.ToArray();

        }

        #endregion

    }

}
