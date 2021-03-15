/*
 * Copyright (c) 2010-2021, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

    /// <summary>
    /// An abstract data structure for internal customer-specific data.
    /// </summary>
    public abstract class AInternalData
    {

        #region Properties

        /// <summary>
        /// An optional dictionary of customer-specific data.
        /// </summary>
        protected Dictionary<String, Object>  internalData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new data structure for customer specific data.
        /// </summary>
        /// <param name="InternalData">An optional dictionary of customer-specific data.</param>
        protected AInternalData(Dictionary<String, Object> InternalData)
        {
            this.internalData = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
        }

        /// <summary>
        /// Create a new data structure for customer specific data.
        /// </summary>
        /// <param name="InternalData">An optional dictionary of customer-specific data.</param>
        protected AInternalData(IReadOnlyDictionary<String, Object> InternalData)
        {
            this.internalData = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
        }

        /// <summary>
        /// Create a new data structure for customer specific data.
        /// </summary>
        /// <param name="InternalData">An optional dictionary of customer-specific data.</param>
        protected AInternalData(IEnumerable<KeyValuePair<String, Object>> InternalData = null)
        {
            this.internalData = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
        }

        #endregion


        public IEnumerable<KeyValuePair<String, Object>> InternalData
            => internalData;


        public Boolean HasInternalData
            => internalData != null && internalData.Count > 0;

        public Boolean IsDefined(String Key)
        {

            if (internalData == null)
                return false;

            if (Key.IsNullOrEmpty())
                return false;

            return internalData.ContainsKey(Key);

        }

        public Boolean IsDefined(String Key, Object Value)
        {

            if (internalData == null)
                return false;

            if (Key.IsNullOrEmpty())
                return false;

            if (internalData.TryGetValue(Key, out Object value))
                return Value.Equals(value);

            return false;

        }

        public Object GetInternalData(String Key)
        {

            if (internalData == null)
                return null;

            if (internalData.TryGetValue(Key, out Object value))
                return value;

            return null;

        }

        public T GetInternalDataAs<T>(String Key)
        {

            if (internalData == null)
                return default;

            try
            {

                if (internalData.TryGetValue(Key, out Object value))
                    return (T) value;

            }
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
            catch (Exception)
            { }
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.

            return default;

        }

        public Boolean TryGetInternalData(String Key, out Object Value)
        {

            if (internalData == null)
            {
                Value = null;
                return false;
            }

            return internalData.TryGetValue(Key, out Value);

        }

        public Boolean TryGetInternalDataAs<T>(String Key, out T Value)
        {

            if (internalData != null)
            {

                try
                {

                    if (internalData.TryGetValue(Key, out Object value))
                    {
                        Value = (T) value;
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

            if (internalData  != null &&
                ValueDelegate != null &&
                internalData.TryGetValue(Key, out Object value))
            {
                ValueDelegate(value);
            }

        }

        public void IfDefinedAs<T>(String     Key,
                                   Action<T>  ValueDelegate)
        {

            if (ValueDelegate == null)
                return;

            if (internalData.TryGetValue(Key, out Object value) &&
                value is T valueT)
            {
                ValueDelegate(valueT);
            }

        }


        /// <summary>
        /// An abstract builder for internal customer-specific data.
        /// </summary>
        public abstract class Builder : IInternalDataBuilder
        {

            #region Properties

            /// <summary>
            /// All internal data.
            /// </summary>
            protected Dictionary<String, Object>  internalData    { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new data structure for internal customer specific data.
            /// </summary>
            /// <param name="InternalData">An optional dictionary of internal customer-specific data.</param>
            protected Builder(Dictionary<String, Object> InternalData = null)
            {
                this.internalData = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
            }

            /// <summary>
            /// Create a new data structure for customer specific data.
            /// </summary>
            /// <param name="InternalData">An optional dictionary of internal customer-specific data.</param>
            protected Builder(IReadOnlyDictionary<String, Object> InternalData)
            {
                this.internalData = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
            }

            /// <summary>
            /// Create a new data structure for customer specific data.
            /// </summary>
            /// <param name="InternalData">An optional dictionary of internal customer-specific data.</param>
            protected Builder(IEnumerable<KeyValuePair<String, Object>> InternalData = null)
            {
                this.internalData = (InternalData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) ?? new Dictionary<String, Object>();
            }

            #endregion


            public IEnumerable<KeyValuePair<String, Object>> InternalData
                => internalData;


            public void AddInternalData(String  Key,
                                        Object  Value)
            {
                internalData.Add(Key, Value);
            }

            public void SetInternalData(String  Key,
                                        Object  Value)
            {
                lock (internalData)
                {

                    if (!internalData.ContainsKey(Key))
                        internalData.Add(Key, Value);

                    else
                        internalData[Key] = Value;

                }
            }

            public Boolean HasInternalData
                => internalData != null && internalData.Count > 0;

            public Boolean IsDefined(String  Key)
                => internalData.ContainsKey(Key);

            public Object GetInternalData(String  Key)
            {

                if (internalData.TryGetValue(Key, out Object value))
                    return value;

                return null;

            }

            public T GetInternalDataAs<T>(String  Key)
            {

                if (internalData.TryGetValue(Key, out Object value))
                    return (T) value;

                return default;

            }


            public void IfDefined(String          Key,
                                  Action<Object>  ValueDelegate)
            {

                if (ValueDelegate == null)
                    return;

                if (internalData.TryGetValue(Key, out Object value))
                    ValueDelegate(value);

            }

            public void IfDefinedAs<T>(String     Key,
                                       Action<T>  ValueDelegate)
            {

                if (ValueDelegate == null)
                    return;

                if (internalData.TryGetValue(Key, out Object value) &&
                    value is T valueT)
                {
                    ValueDelegate(valueT);
                }

            }

        }

    }

}
