/*
 * Copyright (c) 2010-2018 Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Linq;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias
{

    /// <summary>
    /// Extension methods for the IList interface.
    /// </summary>
    public static class IListExtensions
    {

        #region AddAndReturnList(this List, Element)

        /// <summary>
        /// Another way to add an element to a list.
        /// </summary>
        /// <param name="List">A list of elements.</param>
        /// <param name="Element">The element to be added to the list.</param>
        /// <returns>The changed list.</returns>
        public static IList<T> AddAndReturnList<T>(this IList<T> List, T Element)
        {
            List.Add(Element);
            return List;
        }

        #endregion

        #region AddAndReturnList(this List, Elements)

        /// <summary>
        /// Another way to add an element to a list.
        /// </summary>
        /// <param name="List">A list of elements.</param>
        /// <param name="Elements">Another list to be added to this list.</param>
        /// <returns>The changed list.</returns>
        public static IList<T> AddAndReturnList<T>(this IList<T> List, IEnumerable<T> Elements)
        {
            var NewList = new List<T>(List);
            NewList.AddRange(Elements);
            return NewList;
        }

        #endregion

        #region AddAndReturnElement(this List, Element)

        /// <summary>
        /// Another way to add an value to a list.
        /// </summary>
        /// <param name="List">A list of elements.</param>
        /// <param name="Element">The element to be added to the list.</param>
        /// <returns>The added element.</returns>
        public static T AddAndReturnElement<T>(this IList<T> List, T Element)
        {
            List.Add(Element);
            return Element;
        }

        #endregion


        #region ReverseAndReturn(this List)

        /// <summary>
        /// Reverse and return the given list;
        /// </summary>
        /// <param name="List">A list of elements.</param>
        public static IEnumerable<T> ReverseAndReturn<T>(this IList<T> List)
        {
            return List.Reverse();
        }

        #endregion


        #region RemoveAndReturnFirst(this List)

        /// <summary>
        /// Remove and return first element of the given list;
        /// </summary>
        /// <param name="List">A list of elements.</param>
        public static T RemoveAndReturnFirst<T>(this IList<T> List)
        {

            var Element = List.First();
            List.Remove(Element);

            return Element;

        }

        #endregion

        #region RemoveAndReturnLast(this List)

        /// <summary>
        /// Remove and return last element of the given list;
        /// </summary>
        /// <param name="List">A list of elements.</param>
        public static T RemoveAndReturnLast<T>(this IList<T> List)
        {

            var Element = List.Last();
            List.Remove(Element);

            return Element;

        }

        #endregion



        #region AddRangeAndReturnEnumeration(this List, Elements)

        /// <summary>
        /// Another way to add values to a list.
        /// </summary>
        /// <param name="List">A list of elements.</param>
        /// <param name="Elements">The elements to be added to the list.</param>
        /// <returns>The added element.</returns>
        public static IEnumerable<T> AddRangeAndReturnEnumeration<T>(this IList<T> List, IEnumerable<T> Elements)
        {

            foreach (var element in Elements)
                List.Add(element);

            return Elements;

        }

        #endregion


    }

}
