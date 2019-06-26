/*
 * Copyright (c) 2010-2019, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Geometry.Maths;
using org.GraphDefined.Vanaheimr.Aegir;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// A triangle with geo coordinates.
    /// </summary>
    public class GeoTriangle
    {

        #region Properties

        #region P1

        /// <summary>
        /// The first geo coordinate of the triangle.
        /// </summary>
        public GeoCoordinate P1 { get; private set; }

        #endregion

        #region P2

        /// <summary>
        /// The second geo coordinate of the triangle.
        /// </summary>
        public GeoCoordinate P2 { get; private set; }

        #endregion

        #region P3

        /// <summary>
        /// The third geo coordinate of the triangle.
        /// </summary>
        public GeoCoordinate P3 { get; private set; }

        #endregion


        #region E12

        /// <summary>
        /// The first geo coordinate of the triangle.
        /// </summary>
        public GeoLine E12 { get; private set; }

        #endregion

        #region E23

        /// <summary>
        /// The second geo coordinate of the triangle.
        /// </summary>
        public GeoLine E23 { get; private set; }

        #endregion

        #region E31

        /// <summary>
        /// The third geo coordinate of the triangle.
        /// </summary>
        public GeoLine E31 { get; private set; }

        #endregion


        #region CircumCenter

        /// <summary>
        /// Return the cirumcenter of the triangle.
        /// </summary>
        public GeoCoordinate CircumCenter
        {
            get
            {

                var _Line12 = new GeoLine(P1, P2);
                var _Line23 = new GeoLine(P2, P3);

                var _Normale12 = _Line12.Normale;
                var _Normale23 = _Line23.Normale;

                return new GeoLine(_Line12.Center, _Normale12.P).
                           Intersection(
                       new GeoLine(_Line23.Center, _Normale23.P));

            }
        }

        #endregion

        #region CircumCircle

        /// <summary>
        /// Return the circumcircle of the triangle.
        /// </summary>
        public GeoCircle CircumCircle
        {
            get
            {
                var _CircumCenter = this.CircumCenter;
                return new GeoCircle(_CircumCenter, P1.DistanceTo(_CircumCenter));
            }
        }

        #endregion

        #region Borders

        private GeoLine[] _Borders;

        /// <summary>
        /// Return an enumeration of lines representing the
        /// surrounding borders of the triangle.
        /// </summary>
        public IEnumerable<GeoLine> Borders
        {
            get
            {
                return _Borders;
            }
        }

        #endregion


        public List<String> Tags { get; private set; }

        #endregion

        #region Constructor(s)

        #region Triangle(Pixel1, Pixel2, Pixel3)

        /// <summary>
        /// Create a triangle of type T.
        /// </summary>
        /// <param name="Pixel1">The first pixel of the triangle.</param>
        /// <param name="Pixel2">The second pixel of the triangle.</param>
        /// <param name="Pixel3">The third pixel of the triangle.</param>
        public GeoTriangle(GeoCoordinate Pixel1, GeoCoordinate Pixel2, GeoCoordinate Pixel3)
        {

            #region Initial Checks

            if (Pixel1 == null)
                throw new ArgumentNullException("The given first pixel must not be null!");

            if (Pixel2 == null)
                throw new ArgumentNullException("The given second pixel must not be null!");

            if (Pixel3 == null)
                throw new ArgumentNullException("The given third pixel must not be null!");

            #endregion

            #region Math Checks

            if (Pixel1.Equals(Pixel2) ||
                Pixel1.Equals(Pixel3) ||
                Pixel2.Equals(Pixel3))
                throw new ArgumentException("All distances between the pixels must be larger than zero!");

            //if (Pixel1.Longitude.Equals(Pixel2.Longitude) &&
            //    Pixel2.Longitude.Equals(Pixel3.Longitude))
            //    throw new ArgumentException("All three pixels must not be on a single line!");

            //if (Pixel1.Latitude.Equals(Pixel2.Latitude) &&
            //    Pixel2.Latitude.Equals(Pixel3.Latitude))
            //    throw new ArgumentException("All three pixels must not be on a single line!");

            #endregion

            #region Sort Pixels

            // Sort by x-coordinate.
            while (true)
            {

                if (Pixel1.Longitude > Pixel2.Longitude)
                {
                    GeoCoordinate.Swap(ref Pixel1, ref Pixel2);
                    continue;
                }

                if (Pixel1.Longitude > Pixel3.Longitude)
                {
                    GeoCoordinate.Swap(ref Pixel1, ref Pixel3);
                    continue;
                }

                if (Pixel2.Longitude > Pixel3.Longitude)
                {
                    GeoCoordinate.Swap(ref Pixel2, ref Pixel3);
                    continue;
                }

                break;

            }

            // Sort by y-coordinate if x-coordinates are the same
            if (Pixel1.Longitude.Equals(Pixel2.Longitude))
                if (Pixel1.Latitude > Pixel2.Latitude)
                    GeoCoordinate.Swap(ref Pixel1, ref Pixel2);

            if (Pixel2.Longitude.Equals(Pixel3.Longitude))
                if (Pixel2.Latitude > Pixel3.Latitude)
                    GeoCoordinate.Swap(ref Pixel1, ref Pixel2);

            #endregion

            this.P1         = Pixel1;
            this.P2         = Pixel2;
            this.P3         = Pixel3;

            this.E12        = new GeoLine(P1, P2);
            this.E23        = new GeoLine(P2, P3);
            this.E31        = new GeoLine(P3, P1);

            this._Borders   = new GeoLine[3] { E12, E23, E31 };

            this.Tags       = new List<String>();

        }

        #endregion

        #endregion


        private Double DotProduct(GeoVector v1, GeoVector v2)
        {
            return
            (
               v1.P.Longitude.Value * v2.P.Longitude.Value +
               v1.P.Latitude.Value * v2.P.Latitude.Value
            );
        }

        public Boolean Contains(GeoCoordinate GeoCoordinate)
        {

            var v0 = new GeoVector(P3,            P1);
            var v1 = new GeoVector(P2,            P1);
            var v2 = new GeoVector(GeoCoordinate, P1);

            var dot00 = DotProduct(v0, v0);
            var dot01 = DotProduct(v0, v1);
            var dot02 = DotProduct(v0, v2);
            var dot11 = DotProduct(v1, v1);
            var dot12 = DotProduct(v1, v2);

            // Compute barycentric coordinates
            var invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if point is in triangle
            return (u >= 0) && (v >= 0) && (u + v < 1);

        }


        #region Operator overloadings

        #region Operator == (Triangle1, Triangle2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Triangle1">A Triangle&lt;T&gt;.</param>
        /// <param name="Triangle2">Another Triangle&lt;T&gt;.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GeoTriangle Triangle1, GeoTriangle Triangle2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Triangle1, Triangle2))
                return true;

            // If one is null, but not both, return false.
            if (((Object)Triangle1 == null) || ((Object)Triangle2 == null))
                return false;

            return Triangle1.Equals(Triangle2);

        }

        #endregion

        #region Operator != (Triangle1, Triangle2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Triangle1">A Triangle&lt;T&gt;.</param>
        /// <param name="Triangle2">Another Triangle&lt;T&gt;.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(GeoTriangle Triangle1, GeoTriangle Triangle2)
        {
            return !(Triangle1 == Triangle2);
        }

        #endregion

        #endregion

        #region IComparable Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public virtual Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an IGeoTriangle.
            var ITriangle = Object as GeoTriangle;
            if ((Object)ITriangle == null)
                throw new ArgumentException("The given object is not a valid triangle!");

            return CompareTo(ITriangle);

        }

        #endregion

        #region CompareTo(ITriangle)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ITriangle">An object to compare with.</param>
        public Int32 CompareTo(GeoTriangle ITriangle)
        {

            if ((Object)ITriangle == null)
                throw new ArgumentNullException("The given triangle must not be null!");

            // Compare the x-coordinate of the circumcenter
            var _Result = CircumCenter.Longitude.CompareTo(ITriangle.CircumCenter.Longitude);

            // If equal: Compare the y-coordinate of the circumcenter
            if (_Result == 0)
                _Result = this.CircumCenter.Latitude.CompareTo(ITriangle.CircumCenter.Latitude);

            return _Result;

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

            if (Object == null)
                return false;

            // Check if the given object is an GeoTriangle.
            var TriangleT = (GeoTriangle)Object;
            if ((Object)TriangleT == null)
                return false;

            return this.Equals(TriangleT);

        }

        #endregion

        #region Equals(ITriangle)

        /// <summary>
        /// Compares two triangles for equality.
        /// </summary>
        /// <param name="ITriangle">A triangle to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(GeoTriangle ITriangle)
        {

            if ((Object)ITriangle == null)
                return false;

            return (this.P1.Equals(ITriangle.P1) &&
                    this.P2.Equals(ITriangle.P2) &&
                    this.P3.Equals(ITriangle.P3)) ||

                   (this.P1.Equals(ITriangle.P2) &&
                    this.P2.Equals(ITriangle.P3) &&
                    this.P3.Equals(ITriangle.P1)) ||

                   (this.P1.Equals(ITriangle.P3) &&
                    this.P2.Equals(ITriangle.P1) &&
                    this.P3.Equals(ITriangle.P2));

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            return P1.GetHashCode() ^ 1 + P2.GetHashCode() ^ 2 + P3.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Format("Triangle: Pixel1={0}, Pixel2={1}, Pixel3={2}",
                                 P1.ToString(),
                                 P2.ToString(),
                                 P3.ToString());
        }

        #endregion

    }

}
