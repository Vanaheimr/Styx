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

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Provides a generic label.
    /// </summary>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    public interface ILabel<TLabel>
        //ToDo: Error 326: cannot implement both 'System.IComparable<TId>' and 'System.IComparable<TLabel>' because they may unify for some type parameter substitutions
        //: IEquatable<TLabel>, IComparable<TLabel>, IComparable
        where TLabel : IEquatable<TLabel>, IComparable<TLabel>, IComparable
    {

        /// <summary>
        /// A generic label.
        /// </summary>
        TLabel Label { get; }

    }

}
