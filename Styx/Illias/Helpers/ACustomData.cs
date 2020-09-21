/*
 * Copyright (c) 2010-2020, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Hermod <http://www.github.com/Vanaheimr/Hermod>
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

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public abstract class ACustomData
    {

        #region Properties

        /// <summary>
        /// An optional dictionary of customer-specific data.
        /// </summary>
        private Dictionary<String, Object>  InternalData   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new data structure for customer specific data.
        /// </summary>
        /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
        protected ACustomData(Dictionary<String, Object> CustomData)
        {
            this.InternalData = CustomData ?? new Dictionary<String, Object>();
        }

        /// <summary>
        /// Create a new data structure for customer specific data.
        /// </summary>
        /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
        protected ACustomData(IReadOnlyDictionary<String, Object> CustomData)
        {
            this.InternalData = (CustomData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
        }

        /// <summary>
        /// Create a new data structure for customer specific data.
        /// </summary>
        /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
        protected ACustomData(IEnumerable<KeyValuePair<String, Object>> CustomData = null)
        {
            this.InternalData = (CustomData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
        }

        #endregion


        public IEnumerable<KeyValuePair<String, Object>> CustomData
            => InternalData;


        public Boolean HasCustomData
            => InternalData != null && InternalData.Count > 0;

        public Boolean IsDefined(String Key)
        {

            if (InternalData == null)
                return false;

            if (Key.IsNullOrEmpty())
                return false;

            return InternalData.ContainsKey(Key);

        }

        public Boolean IsDefined(String Key, Object Value)
        {

            if (InternalData == null)
                return false;

            if (Key.IsNullOrEmpty())
                return false;

            if (InternalData.TryGetValue(Key, out Object _Value))
                return Value.Equals(_Value);

            return false;

        }

        public Object GetCustomData(String Key)
        {

            if (InternalData == null)
                return null;

            if (InternalData.TryGetValue(Key, out Object _Value))
                return _Value;

            return null;

        }

        public T GetCustomDataAs<T>(String Key)
        {

            if (InternalData == null)
                return default;

            try
            {

                if (InternalData.TryGetValue(Key, out Object _Value))
                    return (T) _Value;

            }
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
            catch (Exception)
            { }
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.

            return default;

        }

        public Boolean TryGetCustomData(String Key, out Object Value)
        {

            if (InternalData == null)
            {
                Value = null;
                return false;
            }

            return InternalData.TryGetValue(Key, out Value);

        }

        public Boolean TryGetCustomDataAs<T>(String Key, out T Value)
        {

            if (InternalData != null)
            {

                try
                {

                    if (InternalData.TryGetValue(Key, out Object _Value))
                    {
                        Value = (T)_Value;
                        return true;
                    }

                }
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
                catch (Exception)
                { }
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.

            }

            Value = default;
            return false;

        }


        public void IfDefined(String          Key,
                              Action<Object>  ValueDelegate)
        {

            if (InternalData    != null &&
                ValueDelegate != null &&
                InternalData.TryGetValue(Key, out Object Value))
            {
                ValueDelegate(Value);
            }

        }

        public void IfDefinedAs<T>(String     Key,
                                   Action<T>  ValueDelegate)
        {

            if (InternalData    != null &&
                ValueDelegate != null &&
                InternalData.TryGetValue(Key, out Object Value))
            {

                try
                {
                    ValueDelegate((T) Value);
                }
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
                catch (Exception)
                { }
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.

            }

        }




        public abstract class Builder : ICustomDataBuilder
        {

            #region Properties

            /// <summary>
            /// All custom data.
            /// </summary>
            public Dictionary<String, Object>  CustomData   { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new data structure for customer specific data.
            /// </summary>
            /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
            protected Builder(Dictionary<String, Object> CustomData = null)
            {
                this.CustomData  = CustomData ?? new Dictionary<String, Object>();
            }

            /// <summary>
            /// Create a new data structure for customer specific data.
            /// </summary>
            /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
            protected Builder(IReadOnlyDictionary<String, Object> CustomData)
            {
                this.CustomData = (CustomData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
            }

            /// <summary>
            /// Create a new data structure for customer specific data.
            /// </summary>
            /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
            protected Builder(IEnumerable<KeyValuePair<String, Object>> CustomData = null)
            {
                this.CustomData = (CustomData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
            }

            #endregion


            public void AddCustomData(String  Key,
                                      Object  Value)
            {
                CustomData.Add(Key, Value);
            }

            public void SetCustomData(String  Key,
                                      Object  Value)
            {
                lock (CustomData)
                {

                    if (!CustomData.ContainsKey(Key))
                        CustomData.Add(Key, Value);

                    else
                        CustomData[Key] = Value;

                }
            }

            public Boolean HasCustomData
                => CustomData != null && CustomData.Count > 0;

            public Boolean IsDefined(String  Key)
                => CustomData.ContainsKey(Key);

            public Object GetCustomData(String  Key)
            {

                if (CustomData.TryGetValue(Key, out Object _Value))
                    return _Value;

                return null;

            }

            public T GetCustomDataAs<T>(String  Key)
            {

                if (CustomData.TryGetValue(Key, out Object _Value))
                    return (T) _Value;

                return default(T);

            }


            public void IfDefined(String          Key,
                                  Action<Object>  ValueDelegate)
            {

                if (ValueDelegate == null)
                    return;

                if (CustomData.TryGetValue(Key, out Object _Value))
                    ValueDelegate(_Value);

            }

            public void IfDefinedAs<T>(String     Key,
                                       Action<T>  ValueDelegate)
            {

                if (ValueDelegate == null)
                    return;

                if (CustomData.TryGetValue(Key, out Object _Value) &&
                    _Value is T)
                {
                    ValueDelegate((T)_Value);
                }

            }

        }

    }

}
