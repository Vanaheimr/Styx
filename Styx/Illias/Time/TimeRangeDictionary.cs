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
    public class TimeRangeDictionary<TKey, TValue>
         where TKey   : notnull
         where TValue : INotBeforeNotAfter
    {

        #region Data

        private readonly ConcurrentDictionary<TKey, List<TValue>> internalList = [];

        #endregion


        public Boolean TryAdd(TKey       Key,
                              TValue     Value,
                              TimeSpan?  Tolerance   = null)
        {

            var valueList = internalList.GetOrAdd(Key, k => new List<TValue>());

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

            var valueList = internalList.GetOrAdd(Key, k => new List<TValue>());

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


        public Boolean ContainsKey(TKey Key)
            => internalList.ContainsKey(Key);


        public Boolean TryGetValue(TKey                             Key,
                                   DateTime                         Timestamp,
                                   [NotNullWhen(true)] out TValue?  Value,
                                   TimeSpan?                        Tolerance   = null)
        {

            Value = default;

            if (internalList.TryGetValue(Key, out var valueList))
            {
                lock (valueList)
                {

                    var results = new List<TValue>();

                    foreach (var item in valueList)
                    {
                        if (item.IsNotBeforeWithinRange(Timestamp, Tolerance) &&
                            item.IsNotAfterWithinRange (Timestamp, Tolerance))
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

        public Boolean TryGetValues(TKey                     Key,
                                    out IEnumerable<TValue>  Values)
        {

            if (internalList.TryGetValue(Key, out var valuelist))
            {
                Values = valuelist;
                return true;
            }

            Values = Array.Empty<TValue>();
            return false;

        }

        public Boolean Remove(TKey Key)
            => internalList.TryRemove(Key, out var _);


        public void Clear()
            => internalList.Clear();


    }

}
