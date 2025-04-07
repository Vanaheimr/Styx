/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

    public class Context : AInternalData,
                           IEquatable<Context>,
                           IComparable<Context>,
                           IComparable
    {

        String? DataSource { get; }

        public Context(String?                 DataSource     = null,
                       JObject?                CustomData     = null,
                       UserDefinedDictionary?  InternalData   = null,
                       DateTime?               LastChange     = null)

            : base(CustomData,
                   InternalData,
                   LastChange ?? Timestamp.Now)

        {

            this.DataSource = DataSource;

            unchecked
            {

                hashCode = DataSource?.GetHashCode() ?? 0;

            }

        }


        #region Operator overloading

        #region Operator == (Context1, Context2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Context1">A context.</param>
        /// <param name="Context2">Another context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Context Context1,
                                           Context Context2)

            => Context1.Equals(Context2);

        #endregion

        #region Operator != (Context1, Context2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Context1">A context.</param>
        /// <param name="Context2">Another context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Context Context1,
                                           Context Context2)

            => !Context1.Equals(Context2);

        #endregion

        #region Operator <  (Context1, Context2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Context1">A context.</param>
        /// <param name="Context2">Another context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Context Context1,
                                          Context Context2)

            => Context1.CompareTo(Context2) < 0;

        #endregion

        #region Operator <= (Context1, Context2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Context1">A context.</param>
        /// <param name="Context2">Another context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Context Context1,
                                           Context Context2)

            => Context1.CompareTo(Context2) <= 0;

        #endregion

        #region Operator >  (Context1, Context2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Context1">A context.</param>
        /// <param name="Context2">Another context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Context Context1,
                                          Context Context2)

            => Context1.CompareTo(Context2) > 0;

        #endregion

        #region Operator >= (Context1, Context2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Context1">A context.</param>
        /// <param name="Context2">Another context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Context Context1,
                                           Context Context2)

            => Context1.CompareTo(Context2) >= 0;

        #endregion

        #endregion

        #region IComparable<Context> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two context.
        /// </summary>
        /// <param name="Object">A context to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Context context
                   ? CompareTo(context)
                   : throw new ArgumentException("The given object is not a context!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Context)

        /// <summary>
        /// Compares two context.
        /// </summary>
        /// <param name="Context">A context to compare with.</param>
        public Int32 CompareTo(Context? Context)
        {

            if (DataSource is not null && Context?.DataSource is not null)
                return DataSource.CompareTo(Context.DataSource);

            return 0;

        }

        #endregion

        #endregion

        #region IEquatable<Context> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two context for equality.
        /// </summary>
        /// <param name="Object">A context to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Context context &&
                   Equals(context);

        #endregion

        #region Equals(Context)

        /// <summary>
        /// Compares two context for equality.
        /// </summary>
        /// <param name="Context">A context to compare with.</param>
        public Boolean Equals(Context? Context)

            => (DataSource is null     && Context?.DataSource is null) ||
               (DataSource is not null && Context?.DataSource is not null && DataSource.Equals(Context.DataSource));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => DataSource;

        #endregion


    }


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


        #region Created

        private DateTime created;

        /// <summary>
        /// The timestamp of the creation of this object.
        /// </summary>
        [Mandatory]
        public DateTime Created
        {

            get
            {
                return created;
            }

            set
            {

                if (created != value)
                    SetProperty(ref created,
                                value);

            }

        }

        #endregion

        #region LastChange

        private DateTime lastChange;

        /// <summary>
        /// The timestamp of the last changes within this object.
        /// Can e.g. also be used as a HTTP ETag.
        /// </summary>
        [Mandatory]
        public DateTime LastChangeDate
        {

            get
            {
                return lastChange;
            }

            set
            {

                if (lastChange != value)
                    SetProperty(ref lastChange,
                                value);

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
        /// <param name="InternalData">Optional internal customer specific data, e.g. in combination with custom parsers and serializers, which will not be serialized.</param>
        /// <param name="LastChange">The optional timestamp of the last changes within this object.</param>
        protected AInternalData(JObject?                CustomData,
                                UserDefinedDictionary?  InternalData,
                                DateTime?               Created      = null,
                                DateTime?               LastChange   = null)
        {

            this.created       = Created      ?? LastChange ?? Timestamp.Now;
            this.lastChange    = LastChange   ?? Created    ?? Timestamp.Now;

            this.CustomData    = CustomData   ?? [];
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
                                                      Context?           DataSource        = null,
                                                      EventTracking_Id?  EventTrackingId   = null,
                                   [CallerMemberName] String             PropertyName      = "")
        {

            if (!EqualityComparer<T>.Default.Equals(FieldToChange, NewValue))
            {

                var oldValue       = FieldToChange;
                    FieldToChange  = NewValue;

                PropertyChanged(PropertyName,
                                NewValue,
                                oldValue,
                                DataSource,
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
                                       T                  NewValue,
                                       T                  OldValue,
                                       Context?           DataSource        = null,
                                       EventTracking_Id?  EventTrackingId   = null)
        {

            this.lastChange = Timestamp.Now;

            OnPropertyChanged?.Invoke(LastChangeDate,
                                      EventTrackingId ?? EventTracking_Id.New,
                                      this,
                                      PropertyName,
                                      NewValue,
                                      OldValue,
                                      DataSource);

        }

        #endregion


        protected void CloneFrom(AInternalData InternalData)
        {

            foreach (var handler in InternalData.OnPropertyChanged?.GetInvocationList() ?? [])
            {

                if (handler.Target == InternalData)
                    continue;

                OnPropertyChanged += (OnPropertyChangedDelegate) handler;

            }

        }




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
                                                 Context?           DataSource        = null,
                                                 EventTracking_Id?  EventTrackingId   = null)

            => InternalData.Set(Key,
                                NewValue,
                                OldValue,
                                DataSource,
                                EventTrackingId);



        /// <summary>
        /// An abstract builder for internal customer-specific data.
        /// </summary>
        public abstract class Builder : IInternalData
        {

            #region Properties

            /// <summary>
            /// The timestamp of the creation of this object.
            /// </summary>
            [Mandatory]
            public DateTime               Created           { get; set; }

            /// <summary>
            /// The timestamp of the last changes within this ChargingPool.
            /// Can be used as a HTTP ETag.
            /// </summary>
            [Mandatory]
            public DateTime               LastChangeDate    { get; set; }

            /// <summary>
            /// Optional custom data, e.g. in combination with custom parsers and serializers.
            /// </summary>
            [Optional]
            public JObject                CustomData        { get; }


            /// <summary>
            /// An optional dictionary of customer-specific data.
            /// </summary>
            [Optional]
            public UserDefinedDictionary  InternalData      { get; }

            #endregion

            #region Events

            public event OnPropertyChangedDelegate? OnPropertyChanged;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new data structure for customer specific data.
            /// </summary>
            /// <param name="InternalData">An optional dictionary of internal data.</param>
            protected Builder(JObject?                CustomData,
                              UserDefinedDictionary?  InternalData,
                              DateTime?               Created      = null,
                              DateTime?               LastChange   = null)
            {

                this.Created         = Created      ?? LastChange ?? Timestamp.Now;
                this.LastChangeDate  = LastChange   ?? Created    ?? Timestamp.Now;

                this.CustomData      = CustomData   ?? [];
                this.InternalData    = InternalData ?? new UserDefinedDictionary();

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
                                                     Context?           DataSource        = null,
                                                     EventTracking_Id?  EventTrackingId   = null)

                => InternalData.Set(Key,
                                    NewValue,
                                    OldValue,
                                    DataSource,
                                    EventTrackingId);


            public void DeleteProperty<T>(ref T? FieldToChange,
                                          [CallerMemberName] String PropertyName = "")
            {
                
            }


            public void PropertyChanged<T>(String PropertyName, T OldValue, T NewValue, Context? DataSource = null, EventTracking_Id? EventTrackingId = null)
            {
                
            }


            public void SetProperty<T>(ref T FieldToChange, T NewValue, Context? DataSource = null, EventTracking_Id? EventTrackingId = null, [CallerMemberName] String PropertyName = "")
            {
                
            }



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
            //        catch
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
