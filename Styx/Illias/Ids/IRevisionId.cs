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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Provides a generic revision identifier.
    /// </summary>
    /// <typeparam name="TRevId">The type of the revision identifier.</typeparam>
    public interface IRevisionId<TRevId>
        //ToDo: Error 326: cannot implement both 'System.IComparable<TId>' and 'System.IComparable<TRevId>' because they may unify for some type parameter substitutions
        //: IEquatable<TRevId>, IComparable<TRevId>, IComparable  
        where TRevId : IEquatable<TRevId>, IComparable<TRevId>, IComparable
    {

        /// <summary>
        /// A generic revision identifier.
        /// </summary>
        TRevId RevId { get; }

    }

}
