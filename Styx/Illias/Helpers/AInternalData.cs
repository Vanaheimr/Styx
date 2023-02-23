/*
 * Copyright (c) 2010-2023, Achim Friedland <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Hermod <https://www.github.com/Vanaheimr/Hermod>
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

using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An abstract data structure for internal customer-specific data.
    /// </summary>
    public abstract class AInternalData : IInternalData
    {

        #region Properties

        /// <summary>
        /// Optional custom data, e.g. in combination with custom parsers and serializers.
        /// </summary>
        [Optional]
        public JObject                CustomData      { get; }


        /// <summary>
        /// An optional dictionary of customer-specific data.
        /// </summary>
        [Optional]
        public UserDefinedDictionary  InternalData    { get; }


        #region LastChange

        private DateTime lastChange;

        /// <summary>
        /// The timestamp of the last changes within this object.
        /// Can e.g. also be used as a HTTP ETag.
        /// </summary>
        [Mandatory]
        public DateTime LastChange
        {

            get
            {
                return lastChange;
            }

            set
            {

                if (lastChange != value)
                    SetProperty(ref lastChange,
                                value,
                                EventTracking_Id.New);

            }

        }

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// An event called whenever a property of this entity changed.
        /// </summary>
        public event OnPropertyChangedDelegate? OnPropertyChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new data structure for customer specific data.
        /// </summary>
        /// <param name="CustomData">Optional customer specific data, e.g. in combination with custom parsers and serializers.</param>
        /// <param name="InternalData">Optional internal data.</param>
        protected AInternalData(JObject?                CustomData,
                                UserDefinedDictionary?  InternalData)
        {

            this.CustomData    = CustomData   ?? new JObject();
            this.InternalData  = InternalData ?? new UserDefinedDictionary();

        }

        #endregion


        #region SetProperty<T>(ref FieldToChange, NewValue, EventTrackingId = null, [CallerMemberName] PropertyName = "")

        /// <summary>
        /// Change the given field and call the OnPropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the field to be changed.</typeparam>
        /// <param name="FieldToChange">A reference to the field to be changed.</param>
        /// <param name="NewValue">The new value of the field to be changed.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="PropertyName">The name of the property to be changed (set by the compiler!)</param>
        public void SetProperty<T>(ref                T                  FieldToChange,
                                                      T                  NewValue,
                                                      EventTracking_Id?  EventTrackingId   = null,
                                   [CallerMemberName] String             PropertyName      = "")
        {

            if (!EqualityComparer<T>.Default.Equals(FieldToChange, NewValue))
            {

                var oldValue       = FieldToChange;
                    FieldToChange  = NewValue;

                PropertyChanged(PropertyName,
                                oldValue,
                                NewValue,
                                EventTrackingId ?? EventTracking_Id.New);

            }

        }

        #endregion

        #region DeleteProperty<T>(ref FieldToChange, [CallerMemberName] PropertyName = "")

        /// <summary>
        /// Delete the given field and call the OnPropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the field to be deleted.</typeparam>
        /// <param name="FieldToChange">A reference to the field to be deleted.</param>
        /// <param name="PropertyName">The name of the property to be deleted (set by the compiler!)</param>
        public void DeleteProperty<T>(ref                T?      FieldToChange,
                                      [CallerMemberName] String  PropertyName   = "")
        {

            if (FieldToChange is not null)
            {

                var oldValue   = FieldToChange;

                FieldToChange  = default;

                PropertyChanged(PropertyName,
                                oldValue,
                                default);

            }

        }

        #endregion

        #region PropertyChanged<T>(PropertyName, OldValue, NewValue, EventTrackingId)

        /// <summary>
        /// Notify subscribers that a property has changed.
        /// </summary>
        /// <typeparam name="T">The type of the changed property.</typeparam>
        /// <param name="PropertyName">The name of the changed property.</param>
        /// <param name="OldValue">The old value of the changed property.</param>
        /// <param name="NewValue">The new value of the changed property.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        public void PropertyChanged<T>(String             PropertyName,
                                       T                  OldValue,
                                       T                  NewValue,
                                       EventTracking_Id?  EventTrackingId = null)
        {

            #region Initial checks

            if (PropertyName is null)
                throw new ArgumentNullException(nameof(PropertyName), "The given property name must not be null!");

            #endregion

            this.lastChange = Timestamp.Now;

            OnPropertyChanged?.Invoke(LastChange,
                                      EventTrackingId ?? EventTracking_Id.New,
                                      this,
                                      PropertyName,
                                      OldValue,
                                      NewValue);

        }

        #endregion


        public Boolean IsEmpty
            => InternalData.IsEmpty;

        public Boolean IsNotEmpty
            => InternalData.IsNotEmpty;

        public Boolean IsDefined(String Key)
            => InternalData.ContainsKey(Key);

        public Boolean IsDefined(String Key, Object? Value)
            => InternalData.Contains(Key, Value);

        public void IfDefined(String          Key,
                              Action<Object>  ValueDelegate)
            => InternalData.IfDefined(Key, ValueDelegate);

        public void IfDefinedAs<T>(String     Key,
                                   Action<T>  ValueDelegate)
            => InternalData.IfDefinedAs<T>(Key, ValueDelegate);

        public Object? GetInternalData(String Key)
            => InternalData.Get(Key);

        public T? GetInternalDataAs<T>(String Key)
            => InternalData.GetAs<T>(Key);

        public Boolean TryGetInternalData(String Key, out Object? Value)
            => InternalData.TryGet(Key, out Value);

        public Boolean TryGetInternalDataAs<T>(String Key, out T? Value)
            => InternalData.TryGetAs<T>(Key, out Value);



        public SetPropertyResult SetInternalData(String             Key,
                                                 Object?            NewValue,
                                                 Object?            OldValue          = null,
                                                 EventTracking_Id?  EventTrackingId   = null)

            => InternalData.Set(Key,
                                NewValue,
                                OldValue,
                                EventTrackingId);



        /// <summary>
        /// An abstract builder for internal customer-specific data.
        /// </summary>
        public abstract class Builder
        {

            #region Properties

            /// <summary>
            /// Optional custom data, e.g. in combination with custom parsers and serializers.
            /// </summary>
            [Optional]
            public JObject                CustomData      { get; }


            /// <summary>
            /// An optional dictionary of customer-specific data.
            /// </summary>
            [Optional]
            public UserDefinedDictionary  InternalData    { get; }

            /// <summary>
            /// The timestamp of the last changes within this ChargingPool.
            /// Can be used as a HTTP ETag.
            /// </summary>
            [Mandatory]
            public DateTime?              LastChange      { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new data structure for customer specific data.
            /// </summary>
            /// <param name="InternalData">An optional dictionary of internal data.</param>
            protected Builder(JObject?                CustomData,
                              UserDefinedDictionary?  InternalData)
            {

                this.CustomData    = CustomData   ?? new JObject();
                this.InternalData  = InternalData ?? new UserDefinedDictionary();

            }

            #endregion

            public Boolean IsEmpty
                => InternalData.IsEmpty;

            public Boolean IsNotEmpty
                => InternalData.IsNotEmpty;

            public Boolean IsDefined(String Key)
                => InternalData.ContainsKey(Key);

            public Boolean IsDefined(String Key, Object? Value)
                => InternalData.Contains(Key, Value);

            public void IfDefined(String          Key,
                                  Action<Object>  ValueDelegate)
                => InternalData.IfDefined(Key, ValueDelegate);

            public void IfDefinedAs<T>(String     Key,
                                       Action<T>  ValueDelegate)
                => InternalData.IfDefinedAs<T>(Key, ValueDelegate);

            public Object? GetInternalData(String Key)
                => InternalData.Get(Key);

            public T? GetInternalDataAs<T>(String Key)
                => InternalData.GetAs<T>(Key);

            public Boolean TryGetInternalData(String Key, out Object? Value)
                => InternalData.TryGet(Key, out Value);

            public Boolean TryGetInternalDataAs<T>(String Key, out T? Value)
                => InternalData.TryGetAs<T>(Key, out Value);



            public SetPropertyResult SetInternalData(String             Key,
                                                     Object?            NewValue,
                                                     Object?            OldValue          = null,
                                                     EventTracking_Id?  EventTrackingId   = null)

                => InternalData.Set(Key,
                                    NewValue,
                                    OldValue,
                                    EventTrackingId);




            //    public void AddInternalData(String  Key,
            //                                Object  Value)
            //    {
            //        internalData.Add(Key, Value);
            //    }

            //    public void SetInternalData(String  Key,
            //                                Object  Value)
            //    {
            //        lock (internalData)
            //        {

            //            if (!internalData.ContainsKey(Key))
            //                internalData.Add(Key, Value);

            //            else
            //                internalData[Key] = Value;

            //        }
            //    }

            //    public Boolean HasInternalData
            //        => internalData != null && internalData.Count > 0;

            //    public Boolean IsDefined(String  Key)
            //        => internalData.ContainsKey(Key);

            //    public Object? GetInternalData(String  Key)
            //    {

            //        if (internalData.TryGetValue(Key, out Object? value))
            //            return value;

            //        return null;

            //    }

            //    public T? GetInternalDataAs<T>(String  Key)
            //    {

            //        try
            //        {
            //            if (internalData.TryGetValue(Key, out Object? value))
            //                return (T) value;
            //        }
            //        catch (Exception)
            //        { }

            //        return default;

            //    }


            //    public void IfDefined(String          Key,
            //                          Action<Object>  ValueDelegate)
            //    {

            //        if (ValueDelegate is null)
            //            return;

            //        if (internalData.TryGetValue(Key, out Object? value) &&
            //            value is not null)
            //        {
            //            ValueDelegate(value);
            //        }

            //    }

            //    public void IfDefinedAs<T>(String     Key,
            //                               Action<T>  ValueDelegate)
            //    {

            //        if (ValueDelegate == null)
            //            return;

            //        if (internalData.TryGetValue(Key, out Object? value) &&
            //            value is T valueT)
            //        {
            //            ValueDelegate(valueT);
            //        }

            //    }

        }

    }

}
