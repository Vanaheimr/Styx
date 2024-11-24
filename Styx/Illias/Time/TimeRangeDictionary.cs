/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A concurrent dictionary whoes values implement INotBeforeNotAfter.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type if the values.</typeparam>
    /// <param name="Tolerance">An optional tolerance of time stamps.</param>
    public class TimeRangeDictionary<TKey, TValue>(TimeSpan? Tolerance = null) : IEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>>,
                                                                                 IEnumerable
         where TKey   : notnull
         where TValue : INotBeforeNotAfter
    {

        #region Data

        private readonly ConcurrentDictionary<TKey, List<TValue>> internalDictionary = [];

        #endregion

        #region Properties

        /// <summary>
        /// The tolerance of time stamps.
        /// </summary>
        public TimeSpan  Tolerance    { get; set; } = Tolerance ?? TimeSpan.FromSeconds(1);

        #endregion


        public Boolean TryAdd(TKey       Key,
                              TValue     Value,
                              TimeSpan?  Tolerance   = null)
        {

            var valueList = internalDictionary.GetOrAdd(Key, k => new List<TValue>());

            lock (valueList)
            {

                foreach (var existingValue in valueList)
                {
                    if (existingValue.IsOverlapping(Value, Tolerance))
                        return false;
                }

                // No overlap found, safe to add the new value
                valueList.Add(Value);
                return true;

            }

        }

        public AddedOrUpdated AddOrUpdate(TKey       Key,
                                          TValue     Value,
                                          TimeSpan?  Tolerance   = null)
        {

            var valueList = internalDictionary.GetOrAdd(Key, k => new List<TValue>());

            lock (valueList)
            {

                var tolerance     = Tolerance ?? TimeSpan.FromSeconds(1);
                var existingItem  = valueList.FirstOrDefault(v => (v.NotBefore ?? DateTime.MinValue).IsEqualToWithinTolerance(Value.NotBefore ?? DateTime.MinValue, tolerance) &&
                                                                  (v.NotAfter  ?? DateTime.MaxValue).IsEqualToWithinTolerance(Value.NotAfter  ?? DateTime.MaxValue, tolerance));

                if (existingItem is not null)
                {

                    //UpdateExistingItem(existingItem, Value);
                    valueList.Remove(existingItem);
                    valueList.Add   (Value);

                    return AddedOrUpdated.Update;

                }

                // Check for overlapping timespans
                foreach (var item in valueList)
                {
                    if (item.IsOverlapping(Value, tolerance))
                        return AddedOrUpdated.Failed;
                }

                valueList.Add(Value);

                return AddedOrUpdated.Add;

            }

        }


        public Boolean TryUpdate(TKey       Key,
                                 TValue     Value,
                                 TValue     NewValue,
                                 TimeSpan?  Tolerance   = null)
        {

            if (!internalDictionary.TryGetValue(Key, out var valueList))
                return false;

            lock (valueList)
            {

                var tolerance   = Tolerance ?? TimeSpan.FromSeconds(1);
                var valueIndex  = valueList.FindIndex(v => (v.NotBefore ?? DateTime.MinValue).IsEqualToWithinTolerance(Value.NotBefore ?? DateTime.MinValue, tolerance) &&
                                                           (v.NotAfter  ?? DateTime.MinValue).IsEqualToWithinTolerance(Value.NotAfter  ?? DateTime.MinValue, tolerance));

                // No matching value found
                if (valueIndex == -1)
                    return false; 

                valueList[valueIndex] = NewValue;

                return true;

            }


        }


        #region ContainsKey (Key,            Timestamp = null, Tolerance = null)

        /// <summary>
        /// Returns true, when the given key exists at the given timestamp.
        /// </summary>
        /// <param name="Key">The key of the value to check.</param>
        /// <param name="Timestamp">An optional point in time to check.</param>
        /// <param name="Tolerance">An optional tolerance of time stamps.</param>
        public Boolean ContainsKey(TKey       Key,
                                   DateTime?  Timestamp   = null,
                                   TimeSpan?  Tolerance   = null)
        {

            if (!Timestamp.HasValue)
                return internalDictionary.ContainsKey(Key);

            var timestamp = Timestamp ?? Illias.Timestamp.Now;

            if (internalDictionary.TryGetValue(Key, out var valueList))
            {
                lock (valueList)
                {
                    foreach (var item in valueList)
                    {
                        if (item.IsNotBeforeWithinRange(timestamp, Tolerance ?? this.Tolerance) &&
                            item.IsNotAfterWithinRange (timestamp, Tolerance ?? this.Tolerance))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

        #endregion

        #region TryGetValue (Key, out Value, Timestamp = null, Tolerance = null)

        /// <summary>
        /// Return the value associated with the specified key and the given time stamp.
        /// </summary>
        /// <param name="Key">The key of the value to get.</param>
        /// <param name="Value">The specified value.</param>
        /// <param name="Timestamp">An optional point in time to check.</param>
        /// <param name="Tolerance">An optional tolerance of time stamps.</param>
        public Boolean TryGetValue(TKey                             Key,
                                   [NotNullWhen(true)] out TValue?  Value,
                                   DateTime?                        Timestamp   = null,
                                   TimeSpan?                        Tolerance   = null)
        {

            Value = default;

            if (internalDictionary.TryGetValue(Key, out var valueList))
            {
                lock (valueList)
                {

                    var results    = new List<TValue>();
                    var timestamp  = Timestamp ?? Illias.Timestamp.Now;

                    foreach (var item in valueList)
                    {
                        if (item.IsNotBeforeWithinRange(timestamp, Tolerance ?? this.Tolerance) &&
                            item.IsNotAfterWithinRange (timestamp, Tolerance ?? this.Tolerance))
                        {
                            results.Add(item);
                        }
                    }

                    if (results.Count == 1)
                    {
                        Value = results.First();
                        return true;
                    }

                    if (results.Count > 1)
                    {
                        Value = results.OrderByDescending(value => value.NotBefore).First();
                        return true;
                    }

                }
            }

            return false;

        }

        #endregion

        #region TryGetValues(Key, out Values)

        /// <summary>
        /// Return all values associated with the specified key.
        /// </summary>
        /// <param name="Key">The key of the values to get.</param>
        /// <param name="Values">The enumeration of values.</param>
        public Boolean TryGetValues(TKey                     Key,
                                    out IEnumerable<TValue>  Values)
        {

            if (internalDictionary.TryGetValue(Key, out var valuelist))
            {
                Values = valuelist;
                return true;
            }

            Values = Array.Empty<TValue>();
            return false;

        }

        #endregion


        public Boolean Remove(TKey Key)
            => internalDictionary.TryRemove(Key, out var _);


        public Boolean TryRemove(TKey Key, out IEnumerable<TValue> Values)
        {

            if (internalDictionary.TryRemove(Key, out var values))
            {
                Values = values ?? [];
                return true;
            }

            Values = [];
            return false;

        }



        public void Clear()
            => internalDictionary.Clear();



        public IEnumerable<TKey> Keys
            => internalDictionary.Keys;

        public IEnumerable<TValue> Values(DateTime?  Timestamp   = null,
                                          TimeSpan?  Tolerance   = null)
        {

            var results    = new List<TValue>();
            var timestamp  = Timestamp ?? Illias.Timestamp.Now;

            foreach (var valueList in internalDictionary.Values.ToList())
            {
                foreach (var item in valueList)
                {
                    if (item.IsNotBeforeWithinRange(timestamp, Tolerance ?? this.Tolerance) &&
                        item.IsNotAfterWithinRange (timestamp, Tolerance ?? this.Tolerance))
                    {
                        results.Add(item);
                    }
                }
            }

            return results;

        }

        public IEnumerator<KeyValuePair<TKey, IEnumerable<TValue>>> GetEnumerator()
        {

            var results = new List<KeyValuePair<TKey, IEnumerable<TValue>>>();

            foreach (var kvp in internalDictionary.ToList())
            {
                results.Add(new KeyValuePair<TKey, IEnumerable<TValue>>(kvp.Key, kvp.Value));
            }

            return results.GetEnumerator();

        }

        IEnumerator IEnumerable.GetEnumerator()
        {

            var results = new List<KeyValuePair<TKey, IEnumerable<TValue>>>();

            foreach (var kvp in internalDictionary.ToList())
            {
                results.Add(new KeyValuePair<TKey, IEnumerable<TValue>>(kvp.Key, kvp.Value));
            }

            return results.GetEnumerator();

        }

    }

}
