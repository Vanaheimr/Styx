/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Pipes.NET <http://www.github.com/ahzf/Pipes.NET>
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

namespace de.ahzf.Pipes
{

    /// <summary>
    /// A class of specialized IEnumerable extension methods.
    /// </summary>
    public static class IEnumerableExtensions
    {

        #region ForEach(this IEnumerable, Action)

        /// <summary>
        /// Iterates over the given enumeration and calls the
        /// given action for each item.
        /// </summary>
        /// <typeparam name="T">The type of the enumerated objects.</typeparam>
        /// <param name="IEnumerable">A enumeration of objects of type T.</param>
        /// <param name="Action">A action method to call for every item of the enumeration.</param>
        public static void ForEach<T>(this IEnumerable<T> IEnumerable, Action<T> Action)
        {

            if (Action == null)
                throw new ArgumentNullException("The parameter 'Action' must not be null!");

            foreach (var _item in IEnumerable)
                Action(_item);

        }

        #endregion

        #region MapEach(this IEnumerable, Func)

        /// <summary>
        /// Iterates over the given enumeration, calls the given func
        /// for each item and returns it immediately.
        /// </summary>
        /// <typeparam name="S">The type of the enumerated objects.</typeparam>
        /// <typeparam name="E">The type of the returning objects.</typeparam>
        /// <param name="IEnumerable">A enumeration of objects of type T.</param>
        /// <param name="Func">A mapping method to call for every item of the enumeration.</param>
        /// <returns>An enumeration of mapped objects of type E</returns>
        public static IEnumerable<E> MapEach<S, E>(this IEnumerable<S> IEnumerable, Func<S, E> Func)
        {

            if (Func == null)
                throw new ArgumentNullException("The parameter 'Func' must not be null!");

            foreach (var _item in IEnumerable)
                yield return Func(_item);

        }

        #endregion

    }

}
