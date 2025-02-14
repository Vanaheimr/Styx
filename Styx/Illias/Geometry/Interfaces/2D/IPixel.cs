﻿/*
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

#region Usings

using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Geometry
{

    /// <summary>
    /// The interface of a pixel of type T.
    /// </summary>
    /// <typeparam name="T">The internal type of the pixel.</typeparam>
    public interface IPixel<T> : IEquatable<IPixel<T>>, IComparable<IPixel<T>>, IComparable
        where T : IEquatable<T>, IComparable<T>, IComparable
    {

        #region Properties

        /// <summary>
        /// The X-coordinate.
        /// </summary>
        T X { get; }

        /// <summary>
        /// The Y-coordinate.
        /// </summary>
        T Y { get; }

        #endregion


        #region DistanceTo(x, y)

        /// <summary>
        /// A method to calculate the distance between this
        /// pixel and the given coordinates of type T.
        /// </summary>
        /// <param name="x">A x-coordinate of type T</param>
        /// <param name="y">A y-coordinate of type T</param>
        /// <returns>The distance between this pixel and the given coordinates.</returns>
        T DistanceTo(T x, T y);

        #endregion

        #region DistanceTo(Pixel)

        /// <summary>
        /// A method to calculate the distance between
        /// this and another pixel of type T.
        /// </summary>
        /// <param name="Pixel">A pixel of type T</param>
        /// <returns>The distance between this pixel and the given pixel.</returns>
        T DistanceTo(IPixel<T> Pixel);

        #endregion

    }

}
