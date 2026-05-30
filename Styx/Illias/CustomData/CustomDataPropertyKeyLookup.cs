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

using System.Text.Json;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Shared process-wide lookup for JSON property names.
    /// </summary>
    public static class CustomDataPropertyKeyLookup
    {

        private static readonly Lock                       mutex   = new();
        private static readonly Dictionary<String, Int32>  ids     = new(StringComparer.Ordinal);
        private static readonly List<String>               keys    = [];

        public static Int32 Count
        {
            get
            {
                lock (mutex)
                    return keys.Count;
            }
        }

        public static Int32 GetOrAdd(String PropertyName)
        {

            ArgumentNullException.ThrowIfNull(PropertyName);

            lock (mutex)
            {

                if (ids.TryGetValue(PropertyName, out var id))
                    return id;

                id = keys.Count + 1;
                keys.Add(PropertyName);
                ids. Add(PropertyName, id);

                return id;

            }

        }

        public static Boolean TryGetId(String     PropertyName,
                                       out Int32  Id)
        {

            ArgumentNullException.ThrowIfNull(PropertyName);

            lock (mutex)
                return ids.TryGetValue(PropertyName, out Id);

        }

        public static String GetText(Int32 Id)
        {

            lock (mutex)
            {

                if (Id <= 0 || Id > keys.Count)
                    throw new ArgumentOutOfRangeException(nameof(Id), $"Unknown custom data property key id '{Id}'.");

                return keys[Id - 1];

            }

        }

        public static IReadOnlyList<KeyValuePair<Int32, String>> Snapshot()
        {

            lock (mutex)
            {

                var snapshot = new KeyValuePair<Int32, String>[keys.Count];

                for (var i = 0; i < keys.Count; i++)
                    snapshot[i] = new KeyValuePair<Int32, String>(i + 1, keys[i]);

                return snapshot;

            }

        }

        internal static void WriteRAWJSON(Utf8JsonWriter Writer)
        {

            Writer.WriteStartArray();

            foreach (var key in Snapshot())
            {
                Writer.WriteStartObject();
                Writer.WriteNumber("keyId", key.Key);
                Writer.WriteString("key",   key.Value);
                Writer.WriteEndObject();
            }

            Writer.WriteEndArray();

        }

    }

}
