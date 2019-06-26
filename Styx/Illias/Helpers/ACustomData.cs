/*
 * Copyright (c) 2010-2019, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    public abstract class ACustomData
    {

        #region Properties

        /// <summary>
        /// An optional dictionary of customer-specific data.
        /// </summary>
        public IReadOnlyDictionary<String, Object>  CustomData   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new data structure for customer specific data.
        /// </summary>
        /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
        protected ACustomData(IReadOnlyDictionary<String, Object> CustomData)
        {
            this.CustomData = CustomData;
        }

        #endregion


        public Boolean IsDefined(String Key)
        {

            if (CustomData == null)
                return false;

            if (Key.IsNullOrEmpty())
                return false;

            return CustomData.TryGetValue(Key, out Object _Value);

        }

        public Boolean IsDefined(String Key, Object Value)
        {

            if (CustomData == null)
                return false;

            if (Key.IsNullOrEmpty())
                return false;

            if (CustomData.TryGetValue(Key, out Object _Value))
                return Value.Equals(_Value);

            return false;

        }

        public Object GetCustomData(String Key)
        {

            if (CustomData == null)
                return null;

            if (CustomData.TryGetValue(Key, out Object _Value))
                return _Value;

            return null;

        }

        public T GetCustomDataAs<T>(String Key)
        {

            if (CustomData == null)
                return default(T);

            try
            {

                if (CustomData.TryGetValue(Key, out Object _Value))
                    return (T) _Value;

            }
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
            catch (Exception)
            { }
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.

            return default(T);

        }

        public Boolean TryGetCustomData(String Key, out Object Value)
        {

            if (CustomData == null)
            {
                Value = null;
                return false;
            }

            return CustomData.TryGetValue(Key, out Value);

        }

        public Boolean TryGetCustomDataAs<T>(String Key, out T Value)
        {

            if (CustomData != null)
            {

                try
                {

                    if (CustomData.TryGetValue(Key, out Object _Value))
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

            Value = default(T);
            return false;

        }


        public void IfDefined(String          Key,
                              Action<Object>  ValueDelegate)
        {

            if (CustomData    != null &&
                ValueDelegate != null &&
                CustomData.TryGetValue(Key, out Object Value))
            {
                ValueDelegate(Value);
            }

        }

        public void IfDefinedAs<T>(String     Key,
                                   Action<T>  ValueDelegate)
        {

            if (CustomData    != null &&
                ValueDelegate != null &&
                CustomData.TryGetValue(Key, out Object Value))
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

    }


    public abstract class ACustomDataBuilder : ICustomDataBuilder
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
        protected ACustomDataBuilder(Dictionary<String, Object> CustomData = null)
        {
            this.CustomData  = CustomData ?? new Dictionary<String, Object>();
        }

        #endregion


        public void AddCustomData(String  Key,
                                  Object  Value)
        {
            CustomData.Add(Key, Value);
        }

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
