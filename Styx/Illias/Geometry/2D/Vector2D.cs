/*
 * Copyright (c) 2010-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Styx <https://www.github.com/Vanaheimr/Styx>
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
using System.Numerics;


#endregion

namespace org.GraphDefined.Vanaheimr.Illias.Geometry
{

    /// <summary>
    /// A 2-dimensional vector of type T.
    /// </summary>
    /// <typeparam name="T">The internal type of the vector.</typeparam>
    public class Vector2D<T> : IVector2D<T>
        where T : IFloatingPointIeee754<T>
    {

        #region Properties

        #region X

        /// <summary>
        /// The x-component of the vector.
        /// </summary>
        public T X { get; private set; }

        #endregion

        #region Y

        /// <summary>
        /// The y-component of the vector.
        /// </summary>
        public T Y { get; private set; }

        #endregion

        #region Length

        /// <summary>
        /// The length of the vector.
        /// </summary>
        public T Length { get; private set; }

        #endregion


        #region NormVector

        /// <summary>
        /// Return a normalized vector.
        /// </summary>
        public IVector2D<T> NormVector
        {
            get
            {

                // Not the four-argument constructor: that one builds a vector
                // from one point to another as X1 - X2, so passing the origin
                // first returned the negated unit vector - a normalised vector
                // pointing the opposite way to the one it normalises.
                return new Vector2D<T>(X / Length,
                                       Y / Length);

            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region Vector(X, Y)

        /// <summary>
        /// Create a 2-dimensional vector of type T.
        /// </summary>
        /// <param name="X">The x-component of the vector.</param>
        /// <param name="Y">The y-component of the vector.</param>
        public Vector2D(T X, T Y)
        {

            #region Initial Checks

            if (X is null)
                throw new ArgumentNullException("The given x-component must not be null!");

            if (Y is null)
                throw new ArgumentNullException("The given y-component must not be null!");

            #endregion


            this.X      = X;
            this.Y      = Y;
            this.Length = new Pixel<T>(T.Zero, T.Zero).DistanceTo(X, Y);

        }

        #endregion

        #region Vector(X1, Y1, X2, Y2)

        /// <summary>
        /// Create a 2-dimensional vector of type T.
        /// </summary>
        /// <param name="X1">The first x-coordinate of the vector.</param>
        /// <param name="Y1">The first y-coordinate of the vector.</param>
        /// <param name="X2">The second x-coordinate of the vector.</param>
        /// <param name="Y2">The second y-coordinate of the vector.</param>
        public Vector2D(T X1, T Y1, T X2, T Y2)
        {

            #region Initial Checks

            if (X1   is null)
                throw new ArgumentNullException("The given left-coordinate must not be null!");

            if (Y1    is null)
                throw new ArgumentNullException("The given top-coordinate must not be null!");

            if (X2  is null)
                throw new ArgumentNullException("The given right-coordinate must not be null!");

            if (Y2 is null)
                throw new ArgumentNullException("The given bottom-coordinate must not be null!");

            #endregion

            
            this.X      = X1 - X2;
            this.Y      = Y1 - Y2;
            this.Length = new Pixel<T>(X1, Y1).DistanceTo(X2, Y2);

        }

        #endregion

        #region Vector(Pixel1, Pixel2)

        /// <summary>
        /// Create a 2-dimensional vector of type T.
        /// </summary>
        /// <param name="Pixel1">A pixel of type T.</param>
        /// <param name="Pixel2">A pixel of type T.</param>
        public Vector2D(IPixel<T> Pixel1, IPixel<T> Pixel2)
        {

            #region Initial Checks

            if (Pixel1 is null)
                throw new ArgumentNullException("The first pixel must not be null!");

            if (Pixel2 is null)
                throw new ArgumentNullException("The second pixel must not be null!");

            #endregion


            this.X      = Pixel1.X - Pixel2.X;
            this.Y      = Pixel1.Y - Pixel2.Y;
            this.Length = Pixel1.DistanceTo(Pixel2);

        }

        #endregion

        #region Vector(Vector1, Vector2)

        /// <summary>
        /// Create a 2-dimensional vector of type T.
        /// </summary>
        /// <param name="Vector1">A vector of type T.</param>
        /// <param name="Vector2">A vector of type T.</param>
        public Vector2D(IVector2D<T> Vector1, IVector2D<T> Vector2)
        {

            #region Initial Checks

            if (Vector1 is null)
                throw new ArgumentNullException("The first vector must not be null!");

            if (Vector2 is null)
                throw new ArgumentNullException("The second vector must not be null!");

            #endregion


            this.X      = Vector1.X - Vector2.X;
            this.Y      = Vector1.Y - Vector2.Y;
            this.Length = Vector1.DistanceTo(Vector2);

        }

        #endregion

        #endregion


        #region IsParallelTo(Vector)

        /// <summary>
        /// Determines if the given vector is parallel or
        /// antiparallel to this vector.
        /// </summary>
        /// <param name="Vector">A vector.</param>
        public Boolean IsParallelTo(IVector2D<T> Vector)
        {

            var ThisNormVector = this.NormVector;
            var ThatNormVector = Vector.NormVector;

            if ((ThisNormVector.X.Equals(ThatNormVector.X) &&
                 ThisNormVector.Y.Equals(ThatNormVector.Y)) ||
                (ThisNormVector.X.Equals(-ThatNormVector.X) &&
                 ThisNormVector.Y.Equals(-ThatNormVector.Y)))
                return true;

            return false;

        }

        #endregion

        #region DistanceTo(x, y)

        /// <summary>
        /// A method to calculate the distance between this
        /// vector and the given coordinates of type T.
        /// </summary>
        /// <param name="x">A x-coordinate of type T</param>
        /// <param name="y">A y-coordinate of type T</param>
        /// <returns>The distance between this vector and the given coordinates.</returns>
        public T DistanceTo(T x, T y)
        {

            #region Initial Checks

            if (x is null)
                throw new ArgumentNullException("The given x-coordinate must not be null!");

            if (y is null)
                throw new ArgumentNullException("The given y-coordinate must not be null!");

            #endregion

            var dX = T.Abs(X - x);
            var dY = T.Abs(Y - y);

            return T.Sqrt(dX * dX + dY * dY);

        }

        #endregion

        #region DistanceTo(Vector)

        /// <summary>
        /// A method to calculate the distance between
        /// this and another vector of type T.
        /// </summary>
        /// <param name="Vector">A vector of type T</param>
        /// <returns>The distance between this pixel and the given pixel.</returns>
        public T DistanceTo(IVector2D<T> Vector)
        {

            #region Initial Checks

            if (Vector is null)
                throw new ArgumentNullException("The given vector must not be null!");

            #endregion

            var dX = T.Abs(X - Vector.X);
            var dY = T.Abs(Y - Vector.Y);

            return T.Sqrt(dX * dX + dY * dY);

        }

        #endregion


        #region Operator overloadings

        #region Operator == (Vector1, Vector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Vector1">A Vector&lt;T&gt;.</param>
        /// <param name="Vector2">Another Vector&lt;T&gt;.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Vector2D<T> Vector1, Vector2D<T> Vector2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Vector1, Vector2))
                return true;

            // If one is null, but not both, return false.
            if ((Vector1 is null) || (Vector2 is null))
                return false;

            return Vector1.Equals(Vector2);

        }

        #endregion

        #region Operator != (Vector1, Vector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Vector1">A Vector&lt;T&gt;.</param>
        /// <param name="Vector2">Another Vector&lt;T&gt;.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Vector2D<T> Vector1, Vector2D<T> Vector2)
        {
            return !(Vector1 == Vector2);
        }

        #endregion

        #endregion

        #region IEquatable Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)
        {

            if (Object is null)
                return false;

            // Check if the given object is an Vector2D<T>.
            var VectorT = (Vector2D<T>) Object;
            if (VectorT is null)
                return false;

            return this.Equals(VectorT);

        }

        #endregion

        #region Equals(IVector)

        /// <summary>
        /// Compares two vectors for equality.
        /// </summary>
        /// <param name="IVector">A vector to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IVector2D<T>? IVector)
        {

            if (IVector is null)
                return false;

            return this.X.  Equals(IVector.X) &&
                   this.Y.Equals(IVector.Y);

        }

        #endregion

        #endregion

        #region IComparable Members

        public int CompareTo(IVector2D<T>? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return X.GetHashCode() ^ 1 + Y.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Format("Vector2D: X={0}, Y={1}",
                                 X.ToString(),
                                 Y.ToString());
        }

        #endregion

    }

}
