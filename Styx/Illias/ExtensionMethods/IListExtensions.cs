/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for the IList interface.
    /// </summary>
    public static class IListExtensions
    {

        #region AddAndReturnList      (this List, Element)

        /// <summary>
        /// Another way to add an element to a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        /// <param name="Element">The element to be added to the list.</param>
        /// <returns>The changed list.</returns>
        public static IList<T> AddAndReturnList<T>(this IList<T> List, T Element)
        {
            List.Add(Element);
            return List;
        }

        /// <summary>
        /// Another way to add an element to a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        /// <param name="Element">The element to be added to the list.</param>
        /// <returns>The changed list.</returns>
        public static List<T> AddAndReturnList<T>(this List<T> List, T Element)
        {
            List.Add(Element);
            return List;
        }

        #endregion

        #region AddAndReturnList      (this List, Elements)

        /// <summary>
        /// Another way to add an element to a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        /// <param name="Elements">Another list to be added to this list.</param>
        /// <returns>The changed list.</returns>
        public static List<T> AddAndReturnList<T>(this List<T> List, IEnumerable<T> Elements)
        {

            foreach (var element in Elements)
                List.Add(element);

            return List;

        }

        /// <summary>
        /// Another way to add an element to a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        /// <param name="Elements">Another list to be added to this list.</param>
        /// <returns>The changed list.</returns>
        public static IList<T> AddAndReturnList<T>(this IList<T> List, IEnumerable<T> Elements)
        {

            foreach (var element in Elements)
                List.Add(element);

            return List;

        }

        #endregion

        #region AddAndReturnElement   (this List, Element)

        /// <summary>
        /// Another way to add an value to a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        /// <param name="Element">The element to be added to the list.</param>
        /// <returns>The added element.</returns>
        public static T AddAndReturnElement<T>(this IList<T> List, T Element)
        {
            List?.Add(Element);
            return Element;
        }

        #endregion


        #region RemoveAndReturnList   (this List, Element)

        /// <summary>
        /// Remove the given element and return the updated list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        /// <param name="Element">An element of the given list.</param>
        public static List<T> RemoveAndReturnList<T>(this List<T>  List,
                                                     T             Element)
        {

            List.Remove(Element);

            return List;

        }


        /// <summary>
        /// Remove the given element and return the updated list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        /// <param name="Element">An element of the given list.</param>
        public static IList<T> RemoveAndReturnList<T>(this IList<T>  List,
                                                      T              Element)
        {

            List.Remove(Element);

            return List;

        }

        #endregion

        #region RemoveAndReturnElement(this List, Element)

        /// <summary>
        /// Remove the given element and return the element.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        /// <param name="Element">An element of the given list.</param>
        public static T RemoveAndReturnElement<T>(this IList<T>  List,
                                                  T              Element)
        {

            List.Remove(Element);

            return Element;

        }

        #endregion

        #region RemoveAndReturnFirst  (this List)

        /// <summary>
        /// Remove and return first element of the given list;
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        public static T RemoveAndReturnFirst<T>(this IList<T> List)
        {

            var element = List[0];
            List.Remove(element);

            return element;

        }

        #endregion

        #region RemoveAndReturnLast   (this List)

        /// <summary>
        /// Remove and return last element of the given list;
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="List">A list of elements.</param>
        public static T RemoveAndReturnLast<T>(this IList<T> List)
        {

            var element = List.Last();
            List.Remove(element);

            return element;

        }

        #endregion


    }

}
