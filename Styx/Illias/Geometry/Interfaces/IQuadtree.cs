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
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Geometry
{

    #region IQuadtree

    /// <summary>
    /// A quadtree is an indexing structure for 2-dimensional spartial data.
    /// It stores the given maximum number of pixels and forkes itself
    /// into four subtrees if this number becomes larger.
    /// Note: This datastructure is not self-balancing!
    /// </summary>
    public interface IQuadtree
    {

        /// <summary>
        /// The maximum number of embedded elements before
        /// four child node will be created.
        /// </summary>
        UInt32 MaxNumberOfEmbeddedPixels { get; }

        /// <summary>
        /// Return the number of embedded pixels
        /// stored within this Quadtree(Node).
        /// </summary>
        UInt64 EmbeddedCount             { get; }

        /// <summary>
        /// Return the number of pixels stored
        /// within the entire quadtree.
        /// </summary>
        UInt64 Count                     { get; }

    }

    #endregion

    #region IQuadtree<T>

    /// <summary>
    /// A quadtree is an indexing structure for 2-dimensional spartial data.
    /// It stores the given maximum number of pixels and forkes itself
    /// into four subtrees if this number becomes larger.
    /// Note: This datastructure is not self-balancing!
    /// </summary>
    /// <typeparam name="T">The internal datatype of the quadtree.</typeparam>
    public interface IQuadtree<T> : IQuadtree, IEnumerable<IPixel<T>>
        where T : IEquatable<T>, IComparable<T>, IComparable
    {

        #region Events

        /// <summary>
        /// An event to notify about an quadtree split happening.
        /// </summary>
        event QuadtreeSplitEventHandler<T> OnTreeSplit;

        #endregion

        #region Methods

        /// <summary>
        /// Add a pixel to the quadtree.
        /// </summary>
        /// <param name="X">The x-coordinate of a pixel of type T.</param>
        /// <param name="Y">The y-coordinate of a pixel of type T.</param>
        void Add(T X, T Y);

        /// <summary>
        /// Add a pixel to the quadtree.
        /// </summary>
        /// <param name="Pixel">A pixel of type T.</param>
        void Add(IPixel<T> Pixel);

        /// <summary>
        /// Return all pixels matching the given pixelselector delegate.
        /// </summary>
        /// <param name="PixelSelector">A delegate selecting which pixels to return.</param>
        IEnumerable<IPixel<T>> Get(PixelSelector<T> PixelSelector);

        /// <summary>
        /// Return all pixels within the given rectangle.
        /// </summary>
        /// <param name="Rectangle">A rectangle selecting which pixels to return.</param>
        IEnumerable<IPixel<T>> Get(IRectangle<T> Rectangle);

        /// <summary>
        /// Remove a pixel from the quadtree.
        /// </summary>
        /// <param name="Pixel">A pixel of type T.</param>
        void Remove(IPixel<T> Pixel);

        /// <summary>
        /// Remove all pixels located within the given rectangle.
        /// </summary>
        /// <param name="Rectangle">A rectangle selecting which pixels to remove.</param>
        void Remove(IRectangle<T> Rectangle);

        #endregion

    }

    #endregion

    #region IQuadtree<T, TValue>

    /// <summary>
    /// A quadtree is an indexing structure for 2-dimensional spartial data.
    /// It stores the given maximum number of pixels and forkes itself
    /// into four subtrees if this number becomes larger.
    /// Note: This datastructure is not self-balancing!
    /// </summary>
    /// <typeparam name="T">The internal datatype of the quadtree.</typeparam>
    /// <typeparam name="TValue">The type of the stored values.</typeparam>
    public interface IQuadtree<T, TValue> : IQuadtree, IEnumerable<IPixelValuePair<T, TValue>>
        where T : IEquatable<T>, IComparable<T>, IComparable
    {

        #region Events

        /// <summary>
        /// An event to notify about an quadtree split happening.
        /// </summary>
        event QuadtreeSplitEventHandler<T, TValue> OnTreeSplit;

        #endregion

        #region Methods

        /// <summary>
        /// Add a pixel together with a value to the quadtree.
        /// </summary>
        /// <param name="X">The x-coordinate of a pixel of type T.</param>
        /// <param name="Y">The y-coordinate of a pixel of type T.</param>
        /// <param name="Value">A value of type TValue.</param>
        void Add(T X, T Y, TValue Value);

        /// <summary>
        /// Add a pixel together with a value to the quadtree.
        /// </summary>
        /// <param name="IPixel">A pixel of type T.</param>
        /// <param name="Value">A value of type TValue.</param>
        void Add(IPixel<T> IPixel, TValue Value);

        /// <summary>
        /// Add a PixelValuePair to the quadtree.
        /// </summary>
        /// <param name="IPixelValuePair">A PixelValuePair.</param>
        void Add(IPixelValuePair<T, TValue> IPixelValuePair);

        /// <summary>
        /// Return all pixels matching the given pixelselector delegate.
        /// </summary>
        /// <param name="PixelSelector">A delegate selecting which pixels to return.</param>
        IEnumerable<IPixelValuePair<T, TValue>> Get(PixelSelector<T> PixelSelector);

        /// <summary>
        /// Return all pixels within the given rectangle.
        /// </summary>
        /// <param name="Rectangle">A rectangle selecting which pixels to return.</param>
        IEnumerable<IPixelValuePair<T, TValue>> Get(IRectangle<T> Rectangle);

        /// <summary>
        /// Remove a pixel from the quadtree.
        /// </summary>
        /// <param name="Pixel">A pixel of type T.</param>
        void Remove(IPixelValuePair<T, TValue> Pixel);

        /// <summary>
        /// Remove all pixels located within the given rectangle.
        /// </summary>
        /// <param name="Rectangle">A rectangle selecting which pixels to remove.</param>
        void Remove(IRectangle<T> Rectangle);

        #endregion

    }

    #endregion

}
