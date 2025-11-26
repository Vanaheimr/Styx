/*
 * Copyright (c) 2010-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Aegir <https://www.github.com/Vanaheimr/Aegir>
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

namespace org.GraphDefined.Vanaheimr.Aegir
{

    // lat == y
    // lng == x

    /// <summary>
    /// A vector of geo coordinates.
    /// </summary>
    public class GeoVector
    {

        #region Properties

        #region P

        /// <summary>
        /// The x-component of the vector.
        /// </summary>
        public GeoCoordinate P { get; }

        #endregion

        #region Length

        /// <summary>
        /// The length of the vector.
        /// </summary>
        public Double Length { get; }

        #endregion

        #region NormVector

        /// <summary>
        /// Return a normalized vector.
        /// </summary>
        public GeoVector NormVector

            => new GeoVector(

                   new GeoCoordinate(Latitude. Parse(0),
                                     Longitude.Parse(0)),

                   new GeoCoordinate(Latitude. Parse(P.Latitude. Value / Length),
                                     Longitude.Parse(P.Longitude.Value / Length))

               );

        #endregion

        #endregion

        #region Constructor(s)

        #region GeoVector(GeoCoordinate)

        /// <summary>
        /// Create a 2-dimensional vector of type T.
        /// </summary>
        /// <param name="GeoCoordinate">The x-component of the vector.</param>
        public GeoVector(GeoCoordinate GeoCoordinate)
        {

            this.P       = GeoCoordinate;
            this.Length  = new GeoCoordinate(
                              Latitude.Parse(0),
                              Longitude.Parse(0)
                           ).DistanceTo(GeoCoordinate);

        }

        #endregion

        #region GeoVector(GeoCoordinate1, GeoCoordinate2)

        /// <summary>
        /// Create a 2-dimensional vector of type T.
        /// </summary>
        /// <param name="GeoCoordinate1">A pixel of type T.</param>
        /// <param name="GeoCoordinate2">A pixel of type T.</param>
        public GeoVector(GeoCoordinate GeoCoordinate1,
                         GeoCoordinate GeoCoordinate2)
        {

            this.P       = new GeoCoordinate(
                               Latitude. Parse(GeoCoordinate1.Latitude. Value - GeoCoordinate2.Latitude. Value),
                               Longitude.Parse(GeoCoordinate1.Longitude.Value - GeoCoordinate2.Longitude.Value)
                           );

            this.Length  = GeoCoordinate1.DistanceTo(GeoCoordinate2);

        }

        #endregion

        #region GeoVector(Vector1, Vector2)

        /// <summary>
        /// Create a 2-dimensional vector of type T.
        /// </summary>
        /// <param name="GeoVector1">A vector of type T.</param>
        /// <param name="GeoVector2">A vector of type T.</param>
        public GeoVector(GeoVector GeoVector1,
                         GeoVector GeoVector2)
        {

            this.P       = new GeoCoordinate(
                               Latitude. Parse(GeoVector1.P.Latitude. Value - GeoVector2.P.Latitude. Value),
                               Longitude.Parse(GeoVector1.P.Longitude.Value - GeoVector2.P.Longitude.Value)
                           );

            this.Length  = GeoVector1.P.DistanceTo(GeoVector2.P);

        }

        #endregion

        #endregion


        #region IMaths Members

        //#region Zero

        ///// <summary>
        ///// Return the zero value of this datatype.
        ///// </summary>
        //public IVector2D<T> Zero
        //{
        //    get
        //    {
        //        return new Vector2D<T>(Math.Zero, Math.Zero);
        //    }
        //}

        //#endregion

        //#region NegativeInfinity

        ///// <summary>
        ///// Return the negative infinity of this datatype.
        ///// </summary>
        //public IVector2D<T> NegativeInfinity
        //{
        //    get
        //    {
        //        return new Vector2D<T>(Math.NegativeInfinity, Math.NegativeInfinity);
        //    }
        //}

        //#endregion

        //#region PositiveInfinity

        ///// <summary>
        ///// Return the positive infinity of this datatype.
        ///// </summary>
        //public IVector2D<T> PositiveInfinity
        //{
        //    get
        //    {
        //        return new Vector2D<T>(Math.PositiveInfinity, Math.PositiveInfinity);
        //    }
        //}

        //#endregion


        //#region Min(params Values)

        ///// <summary>
        ///// A method to get the minimum of an array of Doubles.
        ///// </summary>
        ///// <param name="Values">An array of Doubles.</param>
        ///// <returns>The minimum of all values: Min(a, b, ...)</returns>
        //public IVector2D<T> Min(params IVector2D<T>[] Values)
        //{

        //    if (Values is null || Values.Length == 0)
        //        throw new ArgumentException("The given values must not be null or zero!");

        //    if (Values.Length == 1)
        //        return Values[0];

        //    var _X = Values[0].X;
        //    var _Y = Values[0].Y;

        //    for (var i = Values.Length - 1; i >= 1; i--)
        //    {
        //        _X = Math.Min(_X, Values[i].X);
        //        _Y = Math.Min(_Y, Values[i].Y);
        //    }

        //    return new Vector2D<T>(Math.Zero, Math.Zero, _X, _Y);

        //}

        //#endregion

        //#region Max(params Values)

        ///// <summary>
        ///// A method to get the maximum of an array of Doubles.
        ///// </summary>
        ///// <param name="Values">An array of Doubles.</param>
        ///// <returns>The maximum of all values: Min(a, b, ...)</returns>
        //public IVector2D<T> Max(params IVector2D<T>[] Values)
        //{

        //    if (Values is null || Values.Length == 0)
        //        throw new ArgumentException("The given values must not be null or zero!");

        //    if (Values.Length == 1)
        //        return Values[0];

        //    var _X = Values[0].X;
        //    var _Y = Values[0].Y;

        //    for (var i = Values.Length - 1; i >= 1; i--)
        //    {
        //        _X = Math.Max(_X, Values[i].X);
        //        _Y = Math.Max(_Y, Values[i].Y);
        //    }

        //    return new Vector2D<T>(Math.Zero, Math.Zero, _X, _Y);

        //}

        //#endregion


        //#region Add(params Summands)

        ///// <summary>
        ///// A method to add vectors.
        ///// </summary>
        ///// <param name="Summands">An array of vectors.</param>
        ///// <returns>The addition of all summands: v1 + v2 + ...</returns>
        //public IVector2D<T> Add(params IVector2D<T>[] Summands)
        //{

        //    if (Summands is null || Summands.Length == 0)
        //        throw new ArgumentException("The given summands must not be null!");

        //    if (Summands.Length == 1)
        //        return Summands[0];

        //    var _X = Summands[0].X;
        //    var _Y = Summands[0].Y;

        //    for (var i = Summands.Length - 1; i >= 1; i--)
        //    {
        //        _X = Math.Add(_X, Summands[i].X);
        //        _Y = Math.Add(_Y, Summands[i].Y);
        //    }

        //    return new Vector2D<T>(Math.Zero, Math.Zero, _X, _Y);

        //}

        //#endregion

        //#region Sub(v1, v2)

        ///// <summary>
        ///// A method to sub two vectors.
        ///// </summary>
        ///// <param name="v1">A vector.</param>
        ///// <param name="v2">A vector.</param>
        ///// <returns>The subtraction of v2 from v1: v1 - v2</returns>
        //public IVector2D<T> Sub(IVector2D<T> v1, IVector2D<T> v2)
        //{
        //    return new Vector2D<T>(Math.Sub(v1.X,   v2.X),
        //                           Math.Sub(v1.Y, v2.Y));
        //}

        //#endregion

        //#region Mul(params Multiplicators)

        ///// <summary>
        ///// A method to multiply vectors.
        ///// </summary>
        ///// <param name="Multiplicators">An array of vectors.</param>
        ///// <returns>The multiplication of all multiplicators: v1 * v2 * ...</returns>
        //public IVector2D<T> Mul(params IVector2D<T>[] Multiplicators)
        //{

        //    if (Multiplicators is null || Multiplicators.Length == 0)
        //        throw new ArgumentException("The given multiplicators must not be null!");

        //    if (Multiplicators.Length == 1)
        //        return Multiplicators[0];

        //    var _X = Multiplicators[0].X;
        //    var _Y = Multiplicators[0].Y;

        //    for (var i = Multiplicators.Length - 1; i >= 1; i--)
        //    {
        //        _X = Math.Mul(_X, Multiplicators[i].X);
        //        _Y = Math.Mul(_Y, Multiplicators[i].Y);
        //    }

        //    return new Vector2D<T>(Math.Zero, Math.Zero, _X, _Y);

        //}

        //#endregion

        //#region Mul2(a)

        ///// <summary>
        ///// A method to multiply a vector by 2.
        ///// </summary>
        ///// <param name="v">A vector.</param>
        ///// <returns>The multiplication of v by 2: (2*x, 2*y)</returns>
        //public IVector2D<T> Mul2(IVector2D<T> v)
        //{
        //    return new Vector2D<T>(Math.Mul2(v.X),
        //                           Math.Mul2(v.Y));
        //}

        //#endregion

        //#region Div(v1, v2)

        ///// <summary>
        ///// A method to divide two vectors.
        ///// </summary>
        ///// <param name="v1">A vector.</param>
        ///// <param name="v2">A vector.</param>
        ///// <returns>The division of v1 by v2: v1 / v2</returns>
        //public IVector2D<T> Div(IVector2D<T> v1, IVector2D<T> v2)
        //{
        //    return new Vector2D<T>(Math.Div(v1.X,   v2.X),
        //                           Math.Div(v1.Y, v2.Y));
        //}

        //#endregion

        //#region Div2(v)

        ///// <summary>
        ///// A method to divide a vector by 2.
        ///// </summary>
        ///// <param name="v">A vector.</param>
        ///// <returns>The division of v by 2: v / 2</returns>
        //public IVector2D<T> Div2(IVector2D<T> v)
        //{
        //    return new Vector2D<T>(Math.Div2(v.X),
        //                           Math.Div2(v.Y));
        //}

        //#endregion

        //#region Pow(a, b)

        ///// <summary>
        ///// A method to calculate a Double raised to the specified power.
        ///// </summary>
        ///// <param name="v1">A vector.</param>
        ///// <param name="v2">A vector.</param>
        ///// <returns>The values of v1 raised to the specified power of v2: v1^v2</returns>
        //public IVector2D<T> Pow(IVector2D<T> v1, IVector2D<T> v2)
        //{
        //    return new Vector2D<T>(Math.Pow(v1.X,   v2.X),
        //                           Math.Pow(v1.Y, v2.Y));
        //}

        //#endregion


        //#region Inv(v)

        ///// <summary>
        ///// A method to calculate the inverse value of the given vector.
        ///// </summary>
        ///// <param name="v">A vector.</param>
        ///// <returns>The inverse value of v: -v</returns>
        //public IVector2D<T> Inv(IVector2D<T> v)
        //{
        //    return new Vector2D<T>(Math.Inv(v.X),
        //                           Math.Inv(v.Y));
        //}

        //#endregion

        //#region Abs(v)

        ///// <summary>
        ///// A method to calculate the absolute value of the given vector.
        ///// </summary>
        ///// <param name="v">A vector.</param>
        ///// <returns>The absolute value of v: (|a| |b|)</returns>
        //public IVector2D<T> Abs(IVector2D<T> v)
        //{
        //    return new Vector2D<T>(Math.Abs(v.X),
        //                           Math.Abs(v.Y));
        //}

        //#endregion

        //#region Sqrt(v)

        ///// <summary>
        ///// A method to calculate the square root of the vector.
        ///// </summary>
        ///// <param name="v">A vector.</param>
        ///// <returns>The square root of v: Sqrt(v)</returns>
        //public IVector2D<T> Sqrt(IVector2D<T> v)
        //{
        //    return new Vector2D<T>(Math.Sqrt(v.X),
        //                           Math.Sqrt(v.Y));
        //}

        //#endregion


        //#region Distance(a, b)

        ///// <summary>
        ///// A method to calculate the distance between two vectors.
        ///// </summary>
        ///// <param name="v1">A vector.</param>
        ///// <param name="v2">A vector.</param>
        ///// <returns>The distance between v1 and v2.</returns>
        //public IVector2D<T> Distance(IVector2D<T> v1, IVector2D<T> v2)
        //{
        //    return new Vector2D<T>(Math.Abs(Math.Sub(v1.X,   v2.X)),
        //                           Math.Abs(Math.Sub(v1.Y, v2.Y)));
        //}

        //#endregion

        #endregion


        #region IsParallelTo(Vector)

        /// <summary>
        /// Determines if the given vector is parallel or
        /// antiparallel to this vector.
        /// </summary>
        /// <param name="Vector">A vector.</param>
        public Boolean IsParallelTo(GeoVector Vector)
        {

            var ThisNormVector = this.NormVector;
            var ThatNormVector = Vector.NormVector;

            if ((ThisNormVector.P.Longitude.Value.Equals(ThatNormVector.P.Longitude.Value) &&
                 ThisNormVector.P.Latitude.Value. Equals(ThatNormVector.P.Latitude.Value)) ||
                (ThisNormVector.P.Longitude.Value.Equals(-1*ThatNormVector.P.Longitude.Value) &&
                 ThisNormVector.P.Latitude.Value. Equals(-1*ThatNormVector.P.Latitude.Value)))
                return true;

            return false;

        }

        #endregion

        //#region DistanceTo(x, y)

        ///// <summary>
        ///// A method to calculate the distance between this
        ///// vector and the given coordinates of type T.
        ///// </summary>
        ///// <param name="x">A x-coordinate of type T</param>
        ///// <param name="y">A y-coordinate of type T</param>
        ///// <returns>The distance between this vector and the given coordinates.</returns>
        //public Double DistanceTo(GeoCoordinate P2)
        //{

        //    #region Initial Checks

        //    if (P2 is null)
        //        throw new ArgumentNullException("The given geo coordinate must not be null!");

        //    #endregion

        //    var dX = Math.Distance(X, x);
        //    var dY = Math.Distance(Y, y);

        //    return Math.Sqrt(dX * dX + dY * dY);

        //}

        //#endregion

        //#region DistanceTo(Vector)

        ///// <summary>
        ///// A method to calculate the distance between
        ///// this and another vector of type T.
        ///// </summary>
        ///// <param name="Vector">A vector of type T</param>
        ///// <returns>The distance between this pixel and the given pixel.</returns>
        //public Double DistanceTo(GeoVector Vector)
        //{

        //    #region Initial Checks

        //    if (Vector is null)
        //        throw new ArgumentNullException("The given vector must not be null!");

        //    #endregion

        //    var dX = Math.Distance(X, Vector.X);
        //    var dY = Math.Distance(Y, Vector.Y);

        //    return Math.Sqrt(Math.Add(Math.Mul(dX, dX), Math.Mul(dY, dY)));

        //}

        //#endregion


        #region Operator overloadings

        #region Operator == (Vector1, Vector2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Vector1">A Vector&lt;T&gt;.</param>
        /// <param name="Vector2">Another Vector&lt;T&gt;.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GeoVector Vector1, GeoVector Vector2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Vector1, Vector2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Vector1 is null) || ((Object) Vector2 is null))
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
        public static Boolean operator != (GeoVector Vector1, GeoVector Vector2)
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
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            // Check if the given object is an Vector2D<T>.
            var VectorT = (GeoVector) Object;
            if ((Object) VectorT is null)
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
        public Boolean Equals(GeoVector IVector)
        {

            if ((Object) IVector is null)
                return false;

            return this.P.Longitude.Equals(IVector.P.Longitude) &&
                   this.P.Latitude. Equals(IVector.P.Latitude);

        }

        #endregion

        #endregion

        #region IComparable Members

        public int CompareTo(GeoVector other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
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
            return P.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Format("GeoVector: X={0}, Y={1}",
                                 P.Longitude.ToString(),
                                 P.Latitude.ToString());
        }

        #endregion

    }

}
