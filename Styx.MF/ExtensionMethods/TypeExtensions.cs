/*
 * Copyright (c) 2010-2012 Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Illias Commons <http://www.github.com/ahzf/Illias>
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

namespace de.ahzf.Illias.Commons
{

    /// <summary>
    /// Extensions to the String class.
    /// </summary>
    public static class TypeExtensions
    {

        #region GetRecursiveInterfaces(this Type, AllInterfaces)

        /// <summary>
        /// Traverses the interface inheritance tree and collects all found interfaces.
        /// </summary>
        /// <param name="Interface">The starting innterface.</param>
        /// <param name="AllInterfaces">A list of all interfaces found.</param>
        public static IEnumerable<Type> GetRecursiveInterfaces(this Type Interface, List<Type> AllInterfaces = null)
        {

            if (AllInterfaces == null)
                AllInterfaces = new List<Type>();

            AllInterfaces.Add(Interface);

            var BaseInterfaces = Interface.GetInterfaces();

            if (BaseInterfaces != null && BaseInterfaces.Count() > 0)
                foreach (var BaseInterface in BaseInterfaces)
                    GetRecursiveInterfaces(BaseInterface, AllInterfaces);

            return AllInterfaces;

        }

        #endregion

    }

}
