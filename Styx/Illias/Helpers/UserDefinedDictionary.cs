/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// A delegate called whenever a property of the given object changed.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the event.</param>
    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    /// <param name="Sender">The changed object.</param>
    /// <param name="PropertyName">The name of the changed property.</param>
    /// <param name="NewValue">The new value of the changed property.</param>
    /// <param name="OldValue">The old value of the changed property.</param>
    /// <param name="DataSource">An optional data source or context for the status update.</param>
    public delegate Task OnPropertyChangedDelegate(DateTime          Timestamp,
                                                   EventTracking_Id  EventTrackingId,
                                                   Object            Sender,
                                                   String            PropertyName,
                                                   Object?           NewValue,
                                                   Object?           OldValue     = null,
                                                   Context?          DataSource   = null);


    /// <summary>
    /// A delegate called whenever a property of the given object changed.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the event.</param>
    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    /// <param name="Sender">The changed object.</param>
    /// <param name="PropertyName">The name of the changed property.</param>
    /// <param name="OldValue">The old value of the changed property.</param>
    /// <param name="NewValue">The new value of the changed property.</param>
    /// <param name="DataSource">An optional data source or context for the status update.</param>
    public delegate Task OnPropertyChangedDelegate<TSender>(DateTime          Timestamp,
                                                            EventTracking_Id  EventTrackingId,
                                                            TSender           Sender,
                                                            String            PropertyName,
                                                            Object?           NewValue,
                                                            Object?           OldValue     = null,
                                                            Context?          DataSource   = null)

        where TSender: class, IHasId;


    /// <summary>
    /// Results of the UserDefinedDictionary SET method.
    /// </summary>
    public enum SetPropertyResult
    {

        /// <summary>
        /// A new property was added.
        /// </summary>
        Added    = 0,

        /// <summary>
        /// An existing property value was updated.
        /// </summary>
        Changed  = 1,

        /// <summary>
        /// The property could not be updated.
        /// </summary>
        Conflict = 2,

        /// <summary>
        /// The property was removed.
        /// </summary>
        Removed  = 3

    }

    public class UserDefinedDictionary : IEnumerable<KeyValuePair<String, Object?>>
    {

        #region Data

        internal readonly Dictionary<String, Object?> InternalDictionary;

        #endregion

        #region Events

        /// <summary>
        /// An event called whenever a property of this entity changed.
        /// </summary>
        public event OnPropertyChangedDelegate? OnPropertyChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user-defined dictionary.
        /// </summary>
        /// <param name="InternalData">Optional data for initialization.</param>
        public UserDefinedDictionary(Dictionary<String, Object?>? InternalData = null)
        {
            InternalDictionary = InternalData ?? new Dictionary<String, Object?>();
        }

        /// <summary>
        /// Create a new user-defined dictionary.
        /// </summary>
        /// <param name="InternalData">Optional data for initialization.</param>
        public UserDefinedDictionary(IEnumerable<KeyValuePair<String, Object?>> InternalData)
        {
            InternalDictionary = new Dictionary<String, Object?>(InternalData);
        }

        #endregion


        #region Set(Key, NewValue, OldValue = null, DataSource = null, EventTrackingId = null)

        public SetPropertyResult Set(String             Key,
                                     Object?            NewValue,
                                     Object?            OldValue          = null,
                                     Context?           DataSource        = null,
                                     EventTracking_Id?  EventTrackingId   = null)
        {

            // Locks are shit, but ConcurrentDictionary does not compare values correctly!
            lock (InternalDictionary)
            {

                var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

                if (!InternalDictionary.TryGetValue(Key, out var currentValue))
                {

                    InternalDictionary.Add(Key, NewValue);

                    OnPropertyChanged?.Invoke(Timestamp.Now,
                                              eventTrackingId,
                                              this,
                                              Key,
                                              NewValue,
                                              OldValue,
                                              DataSource);

                    return SetPropertyResult.Added;

                }

                if (currentValue?.ToString() != OldValue?.ToString())
                    return SetPropertyResult.Conflict;

                if (NewValue is not null)
                {

                    InternalDictionary[Key] = NewValue;

                    OnPropertyChanged?.Invoke(Timestamp.Now,
                                              eventTrackingId,
                                              this,
                                              Key,
                                              NewValue,
                                              OldValue,
                                              DataSource);

                    return SetPropertyResult.Changed;

                }


                InternalDictionary.Remove(Key);

                OnPropertyChanged?.Invoke(Timestamp.Now,
                                          eventTrackingId,
                                          this,
                                          Key,
                                          null,
                                          OldValue,
                                          DataSource);

                return SetPropertyResult.Removed;

            }

        }

        #endregion

        #region IsEmpty

        public Boolean IsEmpty
            => InternalDictionary.Count == 0;

        #endregion

        #region IsNotEmpty

        public Boolean IsNotEmpty
            => InternalDictionary.Count > 0;

        #endregion

        #region ContainsKey(Key)

        public Boolean ContainsKey(String Key)

            => InternalDictionary.ContainsKey(Key);

        #endregion

        #region Contains(Key, ExpectedValue)

        public Boolean Contains(String  Key,
                                Object? ExpectedValue)
        {
            lock (InternalDictionary)
            {

                if (!InternalDictionary.TryGetValue(Key, out Object? currentValue))
                    return false;

                return currentValue?.Equals(ExpectedValue) == true;

            }
        }

        #endregion

        #region Contains(KeyValuePair)

        public Boolean Contains(KeyValuePair<String, Object?>  KeyValuePair)

            => Contains(KeyValuePair.Key,
                        KeyValuePair.Value);

        #endregion

        #region IfDefined(Key, out ValueDelegate)

        public void IfDefined(String          Key,
                              Action<Object>  ValueDelegate)
        {

            if (ValueDelegate is not null &&
                InternalDictionary.TryGetValue(Key, out Object? value) &&
                value is not null)
            {
                ValueDelegate(value);
            }

        }

        #endregion

        #region IfDefinedAs<T>(Key, out ValueDelegate)

        public void IfDefinedAs<T>(String     Key,
                                   Action<T>  ValueDelegate)
        {
            lock (InternalDictionary)
            {

                if (ValueDelegate is not null &&
                    InternalDictionary.TryGetValue(Key, out Object? value) &&
                    value is T valueT)
                {
                    ValueDelegate(valueT);
                }

            }
        }

        #endregion


        #region Get(Key)

        public Object? Get(String Key)
        {
            lock (InternalDictionary)
            {

                if (InternalDictionary.TryGetValue(Key, out Object? currentValue))
                    return currentValue;

                return null;

            }
        }

        #endregion

        #region TryGet(Key, out Value)

        public Boolean TryGet(String Key, out Object? Value)

            => InternalDictionary.TryGetValue(Key, out Value);

        #endregion

        #region GetAs<T>(Key)

        public T? GetAs<T>(String Key)
        {

            if (InternalDictionary.TryGetValue(Key, out Object? value) &&
                value is T valueT)
            {
                return valueT;
            }

            return default;

        }

        #endregion

        #region TryGetAs<T>(Key, out Value)

        public Boolean TryGetAs<T>(String Key, out T? Value)
        {

            if (InternalDictionary.TryGetValue(Key, out Object? value) &&
                value is T valueT)
            {
                Value = valueT;
                return true;
            }

            Value = default;
            return false;

        }

        #endregion


        #region Remove(Key)

        public Object? Remove(String Key)
        {
            lock (InternalDictionary)
            {

                if (InternalDictionary.TryGetValue(Key, out Object? currentValue))
                {
                    InternalDictionary.Remove(Key);
                    return currentValue;
                }

                return null;

            }
        }

        #endregion


        #region GetEnumerator()

        public IEnumerator<KeyValuePair<String, Object?>> GetEnumerator()
            => InternalDictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => InternalDictionary.GetEnumerator();

        #endregion


    }

}
