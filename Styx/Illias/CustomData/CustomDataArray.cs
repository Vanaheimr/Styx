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

using System.Buffers;
using System.Text.Json;
using System.Collections;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A compact JSON array representation.
    /// </summary>
    public sealed class CustomDataArray : IEnumerable<CustomDataValue>
    {

        private readonly CustomDataValue[] values;

        public Int32 Count
            => values.Length;

        public CustomDataValue this[Int32 Index]
            => values[Index];

        public CustomDataArray(IEnumerable<CustomDataValue>? Values = null)
        {
            values = Values?.ToArray() ?? [];
        }

        private CustomDataArray(CustomDataValue[] Values)
        {
            values = Values;
        }

        public static CustomDataArray Empty { get; }
            = new([]);

        public CustomDataArray Set(Int32           Index,
                                   CustomDataValue Value)
        {

            var newValues = (CustomDataValue[]) values.Clone();
            newValues[Index] = Value;

            return new CustomDataArray(newValues);

        }

        public CustomDataArray Add(CustomDataValue Value)
        {

            var newValues = new CustomDataValue[values.Length + 1];

            Array.Copy(values, newValues, values.Length);
            newValues[^1] = Value;

            return new CustomDataArray(newValues);

        }

        public void WriteJSON(Utf8JsonWriter Writer)
        {

            Writer.WriteStartArray();

            foreach (var value in values)
                value.WriteJSON(Writer);

            Writer.WriteEndArray();

        }

        public void WriteRAWJSON(Utf8JsonWriter Writer)
        {

            Writer.WriteStartArray();

            foreach (var value in values)
                value.WriteRAWJSON(Writer);

            Writer.WriteEndArray();

        }

        public IEnumerator<CustomDataValue> GetEnumerator()
            => ((IEnumerable<CustomDataValue>) values).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

    }

}
