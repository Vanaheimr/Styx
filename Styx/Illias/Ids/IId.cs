/*
 * Copyright (c) 2010-2020 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Illias <http://www.github.com/Vanaheimr/Illias>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extention methods for the Id interface.
    /// </summary>
    public static class IIdExtentions
    {

        /// <summary>
        /// Determines whether the beginning of this string instance matches the specified string.
        /// </summary>
        /// <param name="Id">An identificator.</param>
        /// <param name="Prefix"></param>
        public static Boolean StartsWith(this IId Id, String Prefix)
            => Id.ToString().StartsWith(Prefix);

    }


    /// <summary>
    /// The common interface of a datastructure used as an unique identification.
    /// </summary>
    public interface IId : IComparable
    {

        //global::org.GraphDefined.WWCP.ChargingPool_Id Clone { get; }
        //int CompareTo(global::org.GraphDefined.WWCP.ChargingPool_Id EVP_Id);
        //bool Equals(global::org.GraphDefined.WWCP.ChargingPool_Id EVP_Id);
        //int GetHashCode();

        UInt64  Length           { get; }
        Boolean IsNullOrEmpty    { get; }

        String  ToString();

    }

    /// <summary>
    /// The common interface of a datastructure used as an unique identification.
    /// </summary>
    public interface IId<T> : IId,
                              IComparable<T>
    {

        /// <summary>
        /// The unique identification of the data structure.
        /// </summary>
        T Id { get; }

    }

}
