/*
 * Copyright (c) 2010-2022, Achim Friedland <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// An abstract data structure for internal customer-specific data.
    /// </summary>
    public abstract class AInternalData
    {

        #region Properties

        /// <summary>
        /// Optional custom data, e.g. in combination with custom parsers and serializers.
        /// </summary>
        [Optional]
        protected JObject?                CustomData      { get; }

        /// <summary>
        /// An optional dictionary of customer-specific data.
        /// </summary>
        [Optional]
        protected UserDefinedDictionary?  InternalData    { get; }

        #endregion

        #region Constructor(s)

        ///// <summary>
        ///// Create a new data structure for customer specific data.
        ///// </summary>
        ///// <param name="InternalData">An optional dictionary of customer-specific data.</param>
        //protected AInternalData(JObject?                     CustomData,
        //                        Dictionary<String, Object?>  InternalData)
        //{

        //    this.CustomData    = CustomData;
        //    this.InternalData  = InternalData is not null
        //                             ? new UserDefinedDictionary(InternalData)
        //                             : null;

        //}

        /// <summary>
        /// Create a new data structure for customer specific data.
        /// </summary>
        /// <param name="InternalData">An optional dictionary of customer-specific data.</param>
        protected AInternalData(JObject?                CustomData,
                                UserDefinedDictionary?  InternalData)
        {

            this.CustomData    = CustomData;
            this.InternalData  = InternalData;

        }





        ///// <summary>
        ///// Create a new data structure for customer specific data.
        ///// </summary>
        ///// <param name="InternalData">An optional dictionary of customer-specific data.</param>
        //protected AInternalData(JObject?                              CustomData,
        //                        IReadOnlyDictionary<String, Object>?  InternalData)
        //{

        //    this.CustomData    = CustomData;
        //    this.InternalData  = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();

        //}

        ///// <summary>
        ///// Create a new data structure for customer specific data.
        ///// </summary>
        ///// <param name="InternalData">An optional dictionary of customer-specific data.</param>
        //protected AInternalData(JObject?                                    CustomData,
        //                        IEnumerable<KeyValuePair<String, Object>>?  InternalData)
        //{

        //    this.CustomData    = CustomData;
        //    this.InternalData  = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();

        //}

        #endregion




        public Boolean HasInternalData

            => InternalData is not null &&
               InternalData.Any();

        public Boolean IsDefined(String Key)
        {

            if (InternalData is not null &&
                Key.IsNotNullOrEmpty())
            {
                return InternalData.ContainsKey(Key);
            }

            return false;

        }

        public Boolean IsDefined(String Key, Object Value)
        {

            if (InternalData is not null &&
                Key.IsNotNullOrEmpty() &&
                InternalData.TryGet(Key, out Object? value))
            {
                return Value.Equals(value);
            }

            return false;

        }

        public Object? GetInternalData(String Key)
        {

            if (InternalData is not null &&
                InternalData.TryGet(Key, out Object? value))
            {
                return value;
            }

            return null;

        }

        public T? GetInternalDataAs<T>(String Key)
        {

            if (InternalData is not null &&
                InternalData.TryGet(Key, out Object? value) &&
                value is T valueT)
            {
                return valueT;
            }

            return default;

        }

        public Boolean TryGetInternalData(String Key, out Object? Value)
        {

            if (InternalData is null)
            {
                Value = null;
                return false;
            }

            return InternalData.TryGet(Key, out Value);

        }

        public Boolean TryGetInternalDataAs<T>(String Key, out T? Value)
        {

            if (InternalData is not null &&
                InternalData.TryGet(Key, out Object? value) &&
                value is T valueT)
            {
                Value = valueT;
                return true;
            }

            Value = default;
            return false;

        }


        public void IfDefined(String          Key,
                              Action<Object>  ValueDelegate)
        {

            if (InternalData  is not null &&
                ValueDelegate is not null &&
                InternalData.TryGet(Key, out Object? value) &&
                value is not null)
            {
                ValueDelegate(value);
            }

        }

        public void IfDefinedAs<T>(String     Key,
                                   Action<T>  ValueDelegate)
        {
            if (InternalData is not null)
            {
                lock (InternalData)
                {

                    if (ValueDelegate is not null &&
                        InternalData.TryGet(Key, out Object? value) &&
                        value is T valueT)
                    {
                        ValueDelegate(valueT);
                    }

                }
            }
        }


        public void SetInternalData(String Key,
                                    Object Value)
        {
            if (InternalData is not null)
            {
                lock (InternalData)
                {
                    InternalData.Set(Key, Value);
                }
            }
        }






        ///// <summary>
        ///// An abstract builder for internal customer-specific data.
        ///// </summary>
        //public abstract class Builder : IInternalDataBuilder
        //{

        //    #region Properties

        //    /// <summary>
        //    /// All internal data.
        //    /// </summary>
        //    protected Dictionary<String, Object>  internalData    { get; }

        //    #endregion

        //    #region Constructor(s)

        //    /// <summary>
        //    /// Create a new data structure for internal customer specific data.
        //    /// </summary>
        //    /// <param name="InternalData">An optional dictionary of internal customer-specific data.</param>
        //    protected Builder(Dictionary<String, Object>? InternalData = null)
        //    {
        //        this.internalData = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
        //    }

        //    /// <summary>
        //    /// Create a new data structure for customer specific data.
        //    /// </summary>
        //    /// <param name="InternalData">An optional dictionary of internal customer-specific data.</param>
        //    protected Builder(IReadOnlyDictionary<String, Object>? InternalData)
        //    {
        //        this.internalData = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
        //    }

        //    /// <summary>
        //    /// Create a new data structure for customer specific data.
        //    /// </summary>
        //    /// <param name="InternalData">An optional dictionary of internal customer-specific data.</param>
        //    protected Builder(IEnumerable<KeyValuePair<String, Object>>? InternalData = null)
        //    {
        //        this.internalData = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
        //    }

        //    #endregion


        //    public IEnumerable<KeyValuePair<String, Object>> InternalData
        //        => internalData;


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

        //}

    }

}
