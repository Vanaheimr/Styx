/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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

using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Collections
{

    /// <summary>
    /// Extensions to the IProperties interface.
    /// </summary>
    public static partial class IReadOnlyPropertiesExtensions
    {

        // GetProperty(Key, ...)

        #region GetProperty<TKey, TValue>(this IReadOnlyProperties, Key)
        // Just an alternative syntax!

        /// <summary>
        /// Return the object value of type TValue associated with the provided property key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        public static TValue? GetProperty<TKey, TValue>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                        TKey                                    Key)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {


            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                return value;

            return default;

        }

        #endregion

        #region UseProperty<TKey, TValue>(this IReadOnlyProperties, Key, OnSuccess [Action<TValue>], OnError = null)

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="OnSuccess">A delegate to call for the associated value of the given property key and its value.</param>
        /// <param name="OnError">A delegate to call for the associated value of the given property key when an error occurs.</param>
        public static void UseProperty<TKey, TValue>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                     TKey                                    Key,
                                                     Action<TValue>                          OnSuccess,
                                                     Action<TKey>?                           OnError = null)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                OnSuccess(value);

            else if (OnError is not null)
                OnError(Key);

        }

        #endregion

        #region UseProperty<TKey, TValue>(this IReadOnlyProperties, Key, OnSuccess [Action<TKey, TValue>], OnError = null)

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="OnSuccess">A delegate to call for the associated value of the given property key and its value.</param>
        /// <param name="OnError">A delegate to call for the associated value of the given property key when an error occurs.</param>
        public static void UseProperty<TKey, TValue>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                     TKey                                    Key,
                                                     Action<TKey, TValue>                    OnSuccess,
                                                     Action<TKey>?                           OnError = null)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                OnSuccess(Key, value);

            else if (OnError is not null)
                OnError(Key);

        }

        #endregion

        #region PropertyFunc<TKey, TValue, TResult>(this IReadOnlyProperties, Key, OnSuccessFunc [Func<TValue, TResult>], OnErrorFunc = null)

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="OnSuccessFunc">A delegate to call for the associated property value of the given property key.</param>
        /// <param name="OnErrorFunc">A delegate to call for the associated property key when the key was not found.</param>
        public static TResult? PropertyFunc<TKey, TValue, TResult>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                                   TKey                                    Key,
                                                                   Func<TValue, TResult>                   OnSuccessFunc,
                                                                   Func<TKey, TResult>?                    OnErrorFunc = null)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                return OnSuccessFunc(value);

            if (OnErrorFunc is not null)
                return OnErrorFunc(Key);

            return default;

        }

        #endregion

        #region PropertyFunc<TKey, TValue, TResult>(this IReadOnlyProperties, Key, OnSuccessFunc [Func<TKey, TValue, TResult>], OnErrorFunc = null)

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="OnSuccessFunc">A delegate to call for the key and associated value of the given property key.</param>
        public static TResult? PropertyFunc<TKey, TValue, TResult>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                                   TKey                                    Key,
                                                                   Func<TKey, TValue, TResult>             OnSuccessFunc,
                                                                   Func<TKey, TResult>?                    OnErrorFunc = null)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                return OnSuccessFunc(Key, value);

            if (OnErrorFunc is not null)
                return OnErrorFunc(Key);

            return default;

        }

        #endregion


        // GetProperty(Key, PropertyType...)

        #region GetProperty<TKey, TValue>(this IReadOnlyProperties, Key, PropertyType)
        // Just an alternative syntax!

        /// <summary>
        /// Return the object value of type TValue associated with the provided property key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="PropertyType">The expected type of the property.</param>
        public static TValue? GetProperty<TKey, TValue>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                        TKey                                    Key,
                                                        Type                                    PropertyType)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                if (value.GetType().Equals(PropertyType))
                    return value;

            return default;

        }

        #endregion

        #region GetString<TKey, TValue>(this IReadOnlyProperties, Key)

        /// <summary>
        /// Return the object value of type TValue associated with the provided property key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        public static String GetString<TKey, TValue>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                     TKey                                    Key)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                return (String) (Object) value!;

            throw new Exception("404!");

        }

        #endregion

        #region GetDouble<TKey, TValue>(this IReadOnlyProperties, Key)

        /// <summary>
        /// Return the object value of type TValue associated with the provided property key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        public static Double GetDouble<TKey, TValue>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                     TKey                                    Key)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
            {
                if (value is Double)
                    return (Double) (Object) value;
                else
                    return Double.Parse(
                               value.ToString()?.Replace(",",".") ?? "",
                               NumberStyles.Float,
                               CultureInfo.InvariantCulture
                           );
            }

            throw new Exception("404!");

        }

        #endregion

        #region UseProperty<TKey, TValue>(this IReadOnlyProperties, Key, PropertyType, OnSuccess [Action<TValue>], OnError = null)

        /// <summary>
        /// Call the given delegate if the given property key is assigned
        /// and the type of the value matches.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="PropertyType">The expected type of the property value.</param>
        /// <param name="OnSuccess">A delegate to call for the associated value of the given property key.</param>
        public static void UseProperty<TKey, TValue>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                     TKey                                    Key,
                                                     Type                                    PropertyType,
                                                     Action<TValue>                          OnSuccess,
                                                     Action<TKey>?                           OnError = null)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
            {
                if (value.GetType().Equals(PropertyType))
                    OnSuccess((TValue) (object) value);
            }
            else if (OnError is not null)
                OnError(Key);

        }

        #endregion

        #region UseProperty<TKey, TValue>(this IReadOnlyProperties, Key, PropertyType, OnSuccess [Action<TKey, TValue>], OnError = null)

        /// <summary>
        /// Call the given delegate if the given property key is assigned
        /// and the type of the value matches.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="PropertyType">The expected type of the property value.</param>
        /// <param name="OnSuccess">A delegate to call for the key and associated value of the given property key.</param>
        public static void UseProperty<TKey, TValue>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                     TKey                                    Key,
                                                     Type                                    PropertyType,
                                                     Action<TKey, TValue>                    OnSuccess,
                                                     Action<TKey>?                           OnError = null)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
            {
                if (value.GetType().Equals(PropertyType))
                    OnSuccess(Key, (TValue) (object) value);
            }
            else if (OnError is not null)
                OnError(Key);

        }

        #endregion

        #region PropertyFunc<TKey, TValue>(this IProperties, Key, PropertyType, OnSuccessFunc [Func<TValue, Object>] )

        /// <summary>
        /// Call the given delegate if the given property key is assigned
        /// and the type of the value matches.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="PropertyType">The expected type of the property value.</param>
        /// <param name="OnSuccessFunc">A delegate to call for the associated value of the given property key.</param>
        public static Object? PropertyFunc<TKey, TValue>(this IProperties<TKey, TValue>  IProperties,
                                                         TKey                            Key,
                                                         Type                            PropertyType,
                                                         Func<TValue, Object>            OnSuccessFunc)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IProperties.TryGetProperty(Key, out var value))
                if (value.GetType().Equals(PropertyType))
                    return OnSuccessFunc(value);

            return default(TValue);

        }

        #endregion

        #region PropertyFunc<TKey, TValue>(this IReadOnlyProperties, Key, PropertyType, OnSuccessFunc [Func<TKey, TValue, Object>] )

        /// <summary>
        /// Call the given delegate if the given property key is assigned
        /// and the type of the value matches.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IReadOnlyProperties">An object implementing IReadOnlyProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="PropertyType">The expected type of the property value.</param>
        /// <param name="OnSuccessFunc">A delegate to call for the key and associated value of the given property key.</param>
        public static Object? PropertyFunc<TKey, TValue>(this IReadOnlyProperties<TKey, TValue>  IReadOnlyProperties,
                                                         TKey                                    Key,
                                                         Type                                    PropertyType,
                                                         Func<TKey, TValue, Object>              OnSuccessFunc)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                if (value.GetType().Equals(PropertyType))
                    return OnSuccessFunc(Key, value);

            return default(TValue);

        }

        #endregion







        // Get(Casted/Dynamic)Property(Key, ...)

        #region GetCastedProperty<TKey, TValue, TCast>(this IProperties, Key)
        // Just an alternative syntax!

        /// <summary>
        /// Return the object value of type TValue associated with the provided property key.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <typeparam name="TCast">The casted type of the properety values.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="Key">The property key.</param>
        public static TCast GetCastedProperty<TKey, TValue, TCast>(this IReadOnlyProperties<TKey, TValue>  IProperties,
                                                                   TKey                                    Key)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            return (TCast) (Object) IProperties[Key]!;

        }

        #endregion




        // InvokeProperty???


        // GetKeyValuePair

        #region GetKeyValuePair<TKey, TValue>(this IProperties, Key, OnSuccess [Action<KeyValuePair<TKey, TValue>>] )
        // Note: Renamed for disambiguity with GetProperty(..., Action<TValue>)

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="OnSuccess">A delegate to call for a matching KeyValuePair.</param>
        public static void GetKeyValuePair<TKey, TValue>(this IProperties<TKey, TValue>      IProperties,
                                                         TKey                                Key,
                                                         Action<KeyValuePair<TKey, TValue>>  OnSuccess)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IProperties.TryGetProperty(Key, out var value))
                OnSuccess(new KeyValuePair<TKey, TValue>(Key, value));

        }

        #endregion

        #region GetKeyValuePair<TKey, TValue>(this IProperties, Key, PropertyType, OnSuccess [Action<KeyValuePair<TKey, TValue>>] )
        // Note: Renamed for disambiguity with GetProperty(..., Action<TValue>)

        /// <summary>
        /// Call the given delegate if the given property key is assigned
        /// and the type of the value matches.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="PropertyType">The expected type of the property value.</param>
        /// <param name="OnSuccess">A delegate to call for a matching KeyValuePair.</param>
        public static void GetKeyValuePair<TKey, TValue>(this IProperties<TKey, TValue>      IProperties,
                                                         TKey                                Key,
                                                         Type                                PropertyType,
                                                         Action<KeyValuePair<TKey, TValue>>  OnSuccess)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IProperties.TryGetProperty(Key, out var value) &&
                value is not null &&
                value.GetType().Equals(PropertyType))
            {
                OnSuccess(new KeyValuePair<TKey, TValue>(Key, value));
            }

        }

        #endregion

        #region KeyValuePairFunc<TKey, TValue, TResult>(this IProperties, Key, OnSuccessFunc [Func<KeyValuePair<TKey, TValue>, TResult>] )
        // Note: Renamed for disambiguity with GetProperty(..., Func<TValue, Object>)

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="OnSuccessFunc">A delegate to call for a matching KeyValuePair.</param>
        public static TResult? KeyValuePairFunc<TKey, TValue, TResult>(this IProperties<TKey, TValue>             IProperties,
                                                                       TKey                                       Key,
                                                                       Func<KeyValuePair<TKey, TValue>, TResult>  OnSuccessFunc)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IProperties.TryGetProperty(Key, out var value))
                return OnSuccessFunc(new KeyValuePair<TKey, TValue>(Key, value));

            return default;

        }

        #endregion

        #region KeyValuePairFunc<TKey, TValue, TResult>(this IProperties, Key, PropertyType, OnSuccessFunc [Func<KeyValuePair<TKey, TValue>, TResult>] )
        // Note: Renamed for disambiguity with GetProperty(..., Func<TValue, Object>)

        /// <summary>
        /// Call the given delegate if the given property key is assigned
        /// and the type of the value matches.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="Key">The property key.</param>
        /// <param name="PropertyType">The expected type of the property value.</param>
        /// <param name="OnSuccessFunc">A delegate to call for a matching KeyValuePair.</param>
        public static TResult? KeyValuePairFunc<TKey, TValue, TResult>(this IProperties<TKey, TValue>             IProperties,
                                                                       TKey                                       Key,
                                                                       Type                                       PropertyType,
                                                                       Func<KeyValuePair<TKey, TValue>, TResult>  OnSuccessFunc)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IProperties.TryGetProperty(Key, out TValue value) &&
                value is not null &&
                value.GetType().Equals(PropertyType))
            {
                return OnSuccessFunc(new KeyValuePair<TKey, TValue>(Key, value));
            }

            return default;

        }

        #endregion



        // UseProperties

        #region UseProperties<TKey, TValue>(IProperties, KeyValueFilter, OnSuccess [Action<TValue>] )

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValueFilter">A delegate to filter properties based on their keys and values.</param>
        /// <param name="OnSuccess">A delegate called for the associated value of each matching KeyValuePair.</param>
        public static void UseProperties<TKey, TValue>(this IProperties<TKey, TValue>  IProperties,
                                                       KeyValueFilter<TKey, TValue>    KeyValueFilter,
                                                       Action<TValue>                  OnSuccess)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            foreach (var Property in IProperties.GetProperties(KeyValueFilter))
                OnSuccess(Property.Value);

        }

        #endregion

        #region UseProperties<TKey, TValue>(IProperties, KeyValueFilter, OnSuccess [Action<TKey, TValue>] )

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValueFilter">A delegate to filter properties based on their keys and values.</param>
        /// <param name="OnSuccess">A delegate to call for each matching KeyValuePair.</param>
        public static void UseProperties<TKey, TValue>(this IProperties<TKey, TValue>  IProperties,
                                                       KeyValueFilter<TKey, TValue>    KeyValueFilter,
                                                       Action<TKey, TValue>            OnSuccess)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            foreach (var Property in IProperties.GetProperties(KeyValueFilter))
                OnSuccess(Property.Key, Property.Value);

        }

        #endregion

        #region UseProperties<TKey, TValue>(IProperties, KeyValueFilter, OnSuccess [Action<KeyValuePair<TKey, TValue>>] )

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValueFilter">A delegate to filter properties based on their keys and values.</param>
        /// <param name="OnSuccess">A delegate to call for each matching KeyValuePair.</param>
        public static void UseProperties<TKey, TValue>(this IProperties<TKey, TValue>      IProperties,
                                                       KeyValueFilter<TKey, TValue>        KeyValueFilter,
                                                       Action<KeyValuePair<TKey, TValue>>  OnSuccess)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            IProperties.GetProperties(KeyValueFilter).ForEach(OnSuccess);

        }

        #endregion

        #region PropertiesFunc<TKey, TValue, TResult>(IProperties, KeyValueFilter, OnSuccessFunc [Func<TValue, TResult>] )

        /// <summary>
        /// Call the given func delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValueFilter">A delegate to filter properties based on their keys and values.</param>
        /// <param name="OnSuccessFunc">A delegate returning an object for the associated value of each matching KeyValuePair.</param>
        public static IEnumerable<TResult> PropertiesFunc<TKey, TValue, TResult>(this IProperties<TKey, TValue>  IProperties,
                                                                                 KeyValueFilter<TKey, TValue>    KeyValueFilter,
                                                                                 Func<TValue, TResult>           OnSuccessFunc)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            foreach (var Property in IProperties.GetProperties(KeyValueFilter))
                yield return OnSuccessFunc(Property.Value);

        }

        #endregion

        #region PropertiesFunc<TKey, TValue, TResult>(IProperties, KeyValueFilter, OnSuccessFunc [Func<TKey, TValue, TResult>] )

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValueFilter">A delegate to filter properties based on their keys and values.</param>
        /// <param name="OnSuccessFunc">A delegate returning an object for each matching KeyValuePair.</param>
        public static IEnumerable<TResult> PropertiesFunc<TKey, TValue, TResult>(this IProperties<TKey, TValue>  IProperties,
                                                                                 KeyValueFilter<TKey, TValue>    KeyValueFilter,
                                                                                 Func<TKey, TValue, TResult>     OnSuccessFunc)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            foreach (var Property in IProperties.GetProperties(KeyValueFilter))
                yield return OnSuccessFunc(Property.Key, Property.Value);

        }

        #endregion

        #region PropertiesFunc<TKey, TValue, TResult>(IProperties, KeyValueFilter, OnSuccessFunc [Func<KeyValuePair<TKey, TValue>, TResult>] )

        /// <summary>
        /// Call the given delegate if the given property key is assigned.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValueFilter">A delegate to filter properties based on their keys and values.</param>
        /// <param name="OnSuccessFunc">A delegate returning an object for each matching KeyValuePair.</param>
        public static IEnumerable<TResult> PropertiesFunc<TKey, TValue, TResult>(this IProperties<TKey, TValue>             IProperties,
                                                                                 KeyValueFilter<TKey, TValue>               KeyValueFilter,
                                                                                 Func<KeyValuePair<TKey, TValue>, TResult>  OnSuccessFunc)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            return IProperties.
                       GetProperties(KeyValueFilter).
                       Select       (OnSuccessFunc);

        }

        #endregion



        // FilteredKeys/Values

        #region FilteredKeys<TKey, TValue>(this IProperties, KeyValueFilter)

        /// <summary>
        /// Get a filtered enumeration of all property keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValueFilter">A delegate to filter KeyValuePairs based on their keys and values.</param>
        /// <returns>An enumeration of all selected property values.</returns>
        public static IEnumerable<TKey> FilteredKeys<TKey, TValue>(this IProperties<TKey, TValue>  IProperties,
                                                                   KeyValueFilter<TKey, TValue>    KeyValueFilter)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            return from   keyValuePair
                   in     IProperties.GetProperties(KeyValueFilter)
                   select keyValuePair.Key;

        }

        #endregion

        #region FilteredValues<TKey, TValue>(this IProperties, KeyValueFilter)

        /// <summary>
        /// Get a filtered enumeration of all property values.
        /// </summary>
        /// <typeparam name="TKey">The type of the property key.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="IProperties">An object implementing IProperties.</param>
        /// <param name="KeyValueFilter">A delegate to filter KeyValuePairs based on their keys and values.</param>
        /// <returns>An enumeration of all selected property values.</returns>
        public static IEnumerable<TValue> FilteredValues<TKey, TValue>(this IProperties<TKey, TValue>  IProperties,
                                                                       KeyValueFilter<TKey, TValue>    KeyValueFilter)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            return from   keyValuePair
                   in     IProperties.GetProperties(KeyValueFilter)
                   select keyValuePair.Value;

        }

        #endregion


        #region CompareProperties(this myIProperties1, myIProperties2)

        /// <summary>
        /// Compares the properties of two different IElement objects (vertices or edges).
        /// </summary>
        /// <param name="Properties1">A vertex or edge</param>
        /// <param name="Properties2">Another vertex or edge</param>
        /// <returns>true if both IElement objects carry the same properties</returns>
        public static Boolean CompareProperties<TKey, TValue>(this IProperties<TKey, TValue>  Properties1,
                                                              IProperties<TKey, TValue>       Properties2)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (Object.ReferenceEquals(Properties1, Properties2))
                return true;

            // Get a ordered list of all properties from both
            var properties1 = (from _KeyValuePair in Properties1 orderby _KeyValuePair.Key select _KeyValuePair).ToList();
            var properties2 = (from _KeyValuePair in Properties2 orderby _KeyValuePair.Key select _KeyValuePair).ToList();

            // Check the total number of entries
            if (properties1.Count != properties2.Count)
                return false;

            // Check the entries in detail
            for (var i=0; i<properties1.Count; i++)
            {

                if (!properties1[i].Key.Equals(properties2[i].Key))
                    return false;

                if (Object.ReferenceEquals(properties1[i].Key, properties2[i].Key))
                    continue;

                // Handle with care as just objects are compared!
                if (!properties1[i].Value.Equals(properties2[i].Value))
                    return false;

            }

            return true;

        }

        #endregion





        // TKey, Object


        #region ListGet<TKey>(this IReadOnlyProperties, Key)

        public static List<Object>? ListGet<TKey>(this IReadOnlyProperties<TKey, Object>  IReadOnlyProperties,
                                                  TKey                                    Key)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                return value as List<Object>;

            return null;

        }

        #endregion

        #region ListTryGet<TKey>(this IReadOnlyProperties, Key, out List)

        public static Boolean ListTryGet<TKey>(this IProperties<TKey, Object>  IReadOnlyProperties,
                                               TKey                            Key,
                                               out List<Object>?               List)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
            {

                List = value as List<Object>;

                if (List is not null)
                    return true;

            }

            List = null;
            return false;

        }

        #endregion


        #region SetGet<TKey>(this IReadOnlyProperties, Key)

        public static HashedSet<Object>? SetGet<TKey>(this IReadOnlyProperties<TKey, Object>  IReadOnlyProperties,
                                                      TKey                                    Key)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
                return value as HashedSet<Object>;

            return null;

        }

        #endregion

        #region SetTryGet<TKey>(this IReadOnlyProperties, Key, out Set)

        public static Boolean SetTryGet<TKey>(this IReadOnlyProperties<TKey, Object>  IReadOnlyProperties,
                                              TKey                                    Key,
                                              out HashedSet<Object>?                  Set)

            where TKey : IEquatable<TKey>,
                         IComparable<TKey>,
                         IComparable

        {

            if (IReadOnlyProperties.TryGetProperty(Key, out var value))
            {

                Set = value as HashedSet<Object>;

                if (Set is not null)
                    return true;

            }

            Set = null;
            return false;

        }

        #endregion

    }

}
