/*
 * Copyright (c) 2010-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for the Id interface.
    /// </summary>
    public static class IIdExtensions
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

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        Boolean  IsNullOrEmpty       { get; }

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        Boolean  IsNotNullOrEmpty    { get; }

        /// <summary>
        /// The length of the identification.
        /// </summary>
        UInt64   Length              { get; }

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        String   ToString();

    }

    /// <summary>
    /// The common interface of datastructures used as an unique identification.
    /// </summary>
    public interface IId<T> : IId,
                              IComparable<T>,
                              IEquatable<T>
    {

    }

    /// <summary>
    /// The common interface of datastructures having an unique identification.
    /// </summary>
    public interface IHasId : IComparable
    { }

    /// <summary>
    /// The common interface of datastructures having an unique identification.
    /// </summary>
    public interface IHasId<TId> : IHasId
                                   //IComparable<TId>,
                                   //IEquatable<TId>

        where TId: IId

    {

        /// <summary>
        /// The unique identification of the data structure.
        /// </summary>
        TId  Id    { get; }

    }

}
