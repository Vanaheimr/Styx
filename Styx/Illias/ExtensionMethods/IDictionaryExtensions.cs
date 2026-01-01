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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for the IDictionary interface.
    /// </summary>
    public static class IDictionaryExtensions
    {

        #region AddAndReturnDictionary(this Dictionary, Key, Value)

        /// <summary>
        /// Another way to add an element to a dictionary.
        /// </summary>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="Key">The element key.</param>
        /// <param name="Value">The element value.</param>
        public static IDictionary<K, V> AddAndReturnDictionary<K, V>(this IDictionary<K, V> Dictionary, K Key, V Value)
        {
            Dictionary.Add(Key, Value);
            return Dictionary;
        }

        #endregion

        #region AddAndReturnDictionary(this Dictionary, KeyCreator, Value)

        /// <summary>
        /// Another way to add an element to a dictionary.
        /// </summary>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="KeyCreator">A delegate providing the key.</param>
        /// <param name="Value">The element value.</param>
        public static IDictionary<K, V> AddAndReturnDictionary<K, V>(this IDictionary<K, V> Dictionary, Func<V, K> KeyCreator, V Value)
        {

            Dictionary.Add(KeyCreator(Value), Value);

            return Dictionary;

        }

        #endregion


        #region AddAndReturnKeyValuePair(this Dictionary, Key, Value)

        /// <summary>
        /// Another way to add an value to a dictionary.
        /// </summary>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="Key">The element key.</param>
        /// <param name="Value">The element value.</param>
        public static KeyValuePair<K, V> AddAndReturnKeyValuePair<K, V>(this IDictionary<K, V> Dictionary, K Key, V Value)
        {
            Dictionary.Add(Key, Value);
            return new KeyValuePair<K,V>(Key, Value);
        }

        #endregion

        #region AddAndReturnKeyValuePair(this Dictionary, KeyCreator, Value)

        /// <summary>
        /// Another way to add an value to a dictionary.
        /// </summary>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="KeyCreator">A delegate providing the key.</param>
        /// <param name="Value">The element value.</param>
        public static KeyValuePair<K, V> AddAndReturnKeyValuePair<K, V>(this IDictionary<K, V> Dictionary, Func<V, K> KeyCreator, V Value)
        {

            var keyValuePair = new KeyValuePair<K, V>(KeyCreator(Value), Value);

            Dictionary.Add(keyValuePair);

            return keyValuePair;

        }

        #endregion


        #region AddAndReturnKey(this Dictionary, Key, Value)

        /// <summary>
        /// Another way to add an value to a dictionary.
        /// </summary>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="Key">The element key.</param>
        /// <param name="Value">The element value.</param>
        /// <returns>The element key.</returns>
        public static K AddAndReturnKey<K, V>(this IDictionary<K, V> Dictionary, K Key, V Value)
        {
            Dictionary.Add(Key, Value);
            return Key;
        }

        #endregion

        #region AddAndReturnKey(this Dictionary, KeyCreator, Value)

        /// <summary>
        /// Another way to add an value to a dictionary.
        /// </summary>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="KeyCreator">A delegate providing the key.</param>
        /// <param name="Value">The element value.</param>
        /// <returns>The element key.</returns>
        public static K AddAndReturnKey<K, V>(this IDictionary<K, V> Dictionary, Func<V, K> KeyCreator, V Value)
        {

            var key = KeyCreator(Value);

            Dictionary.Add(key, Value);

            return key;

        }

        #endregion


        #region AddAndReturnValue(this Dictionary, Key,        Value)

        /// <summary>
        /// Another way to add an value to a dictionary.
        /// </summary>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="Key">The element key.</param>
        /// <param name="Value">The element value.</param>
        public static V AddAndReturnValue<K, V>(this IDictionary<K, V> Dictionary, K Key, V Value)
        {
            Dictionary.Add(Key, Value);
            return Value;
        }

        #endregion

        #region AddAndReturnValue(this Dictionary, KeyCreator, Value)

        /// <summary>
        /// Another way to add an value to a dictionary.
        /// </summary>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="KeyCreator">A delegate providing the key.</param>
        /// <param name="Value">The element value.</param>
        public static V AddAndReturnValue<K, V>(this IDictionary<K, V> Dictionary, Func<V, K> KeyCreator, V Value)
        {

            Dictionary.Add(KeyCreator(Value), Value);

            return Value;

        }

        #endregion


        #region RemoveAndReturnValue(this Dictionary, Key)

        /// <summary>
        /// Remove the key and value from the a dictionary and return the value.
        /// </summary>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="Key">The element key.</param>
        public static V? RemoveAndReturnValue<K, V>(this IDictionary<K, V> Dictionary, K Key)
        {

            if (Dictionary.TryGetValue(Key, out var value))
                return value;

            return default;

        }

        #endregion


        #region TryGet(this Dictionary, Key)

        /// <summary>
        /// Try to get return the value for the given key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="Dictionary">A dictionary.</param>
        /// <param name="Key">The property key.</param>
        public static TValue? TryGet<TKey, TValue>(this IDictionary<TKey, TValue> Dictionary, TKey Key)
        {

            if (Dictionary.TryGetValue(Key, out var value))
                return value;

            return default;

        }

        #endregion

    }

}
