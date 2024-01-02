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

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Collections
{

    /// <summary>
    /// Extensions to the IProperties interface.
    /// </summary>
    public static partial class IReadOnlyPropertiesExtensions
    {

        #region GetDynamicProperty<TKey, TValue>(this IProperties, Key)
        // Just an alternative syntax!

        /// <summary>
        /// Return the object value associated with the provided property key as dynamic.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="Key">The property key.</param>
        public static dynamic GetDynamicProperty<TKey, TValue>(this IProperties<TKey, TValue> IProperties, TKey Key)
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            #region Initial checks

            if (IProperties == null)
                throw new ArgumentNullException("The given IProperties must not be null!");

            #endregion

            TValue _Value;

            if (IProperties.TryGetProperty(Key, out _Value))
                return (dynamic)_Value;

            return default(TValue);

        }

        #endregion

        #region GetDynamicProperty<TKey, TValue>(this IProperties, Key, PropertyType)
        // Just an alternative syntax!

        /// <summary>
        /// Return the object value of type TValue associated with the provided property key as dynamic.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="PropertyType">The expected type of the property.</param>
        public static dynamic GetDynamicProperty<TKey, TValue>(this IProperties<TKey, TValue> IProperties, TKey Key, Type PropertyType)
            where TKey : IEquatable<TKey>, IComparable<TKey>, IComparable
        {

            #region Initial checks

            if (IProperties == null)
                throw new ArgumentNullException("The given IProperties must not be null!");

            #endregion

            TValue _Value;

            if (IProperties.TryGetProperty(Key, out _Value))
                if (_Value.GetType().Equals(PropertyType))
                    return (dynamic)_Value;

            return default(TValue);

        }

        #endregion

    }

}
