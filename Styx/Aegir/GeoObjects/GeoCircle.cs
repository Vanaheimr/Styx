/*
 * Copyright (c) 2010-2017, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Geometry.Maths;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// A circle of type T.
    /// </summary>
    /// <typeparam name="T">The internal type of the circle.</typeparam>
    public class GeoCircle
    {

        #region Properties

        #region Center

        /// <summary>
        /// The center of the circle.
        /// </summary>
        public GeoCoordinate Center { get; private set; }

        #endregion

        #region Radius

        /// <summary>
        /// The radius of the circle.
        /// </summary>
        public Double Radius { get; private set; }

        #endregion

        #region Diameter

        /// <summary>
        /// The diameter of the circle.
        /// </summary>
        public Double Diameter
        {
            get
            {
                return 2*Radius;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region GeoCircle(Center, Radius)

        /// <summary>
        /// Create a circle of type T.
        /// </summary>
        /// <param name="Center">The center of the circle.</param>
        /// <param name="Radius">The radius of the circle.</param>
        public GeoCircle(GeoCoordinate Center, Double Radius)
        {

            #region Initial Checks

            if (Center == null)
                throw new ArgumentNullException("The given center pixel must not be null!");

            if (Radius == null)
                throw new ArgumentNullException("The given radius must not be null!");

            #endregion

            #region Math Checks

            if (Radius.Equals(0))
                throw new ArgumentException("The given radius must not be zero!");

            #endregion

            this.Center = Center;
            this.Radius = Radius;

        }

        #endregion

        #region GeoCircle(Pixel1, Pixel2, Pixel3)

        /// <summary>
        /// Creates a circumcircle of type T based on three pixels.
        /// </summary>
        /// <param name="Pixel1">The first pixel of the triangle.</param>
        /// <param name="Pixel2">The second pixel of the triangle.</param>
        /// <param name="Pixel3">The third pixel of the triangle.</param>
        public GeoCircle(GeoCoordinate Pixel1, GeoCoordinate Pixel2, GeoCoordinate Pixel3)
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

            //if (Pixel1.Longitude.Value.Equals(Pixel2.Longitude.Value) &&
            //    Pixel2.Longitude.Value.Equals(Pixel3.Longitude.Value))
            //    throw new ArgumentException("All three pixels must not be on a single line!");

            //if (Pixel1.Latitude.Value.Equals(Pixel2.Latitude.Value) &&
            //    Pixel2.Latitude.Value.Equals(Pixel3.Latitude.Value))
            //    throw new ArgumentException("All three pixels must not be on a single line!");

            #endregion

            var _Line12     = new GeoLine(Pixel1, Pixel2);
            var _Line23     = new GeoLine(Pixel2, Pixel3);

            this.Center     = new GeoLine(_Line12.Center, _Line12.Normale).
                                  Intersection(
                              new GeoLine(_Line23.Center, _Line23.Normale));

            this.Radius     = (Center != null) ? Center.DistanceTo(Pixel1) : 0;

        }

        #endregion

        #endregion


        #region (static) IsInCircle(Pixel, EdgePixel1, EdgePixel2, EdgePixel3)

        /// <summary>
        /// Checks if the given first pixel is within the circle
        /// defined by the remaining three edge pixels.
        /// </summary>
        /// <param name="Pixel">The pixel to be checked.</param>
        /// <param name="EdgePixel1">The first edge pixel defining a circle.</param>
        /// <param name="EdgePixel2">The second edge pixel defining a circle.</param>
        /// <param name="EdgePixel3">The third edge pixel defining a circle.</param>
        public static Boolean IsInCircle(GeoCoordinate Pixel, GeoCoordinate EdgePixel1, GeoCoordinate EdgePixel2, GeoCoordinate EdgePixel3)
        {

            #region Initial Checks

            if (Pixel == null)
                throw new ArgumentNullException("The given first pixel must not be null!");

            if (EdgePixel1 == null)
                throw new ArgumentNullException("The given first edgepixel must not be null!");

            if (EdgePixel2 == null)
                throw new ArgumentNullException("The given second edgepixel must not be null!");

            if (EdgePixel3 == null)
                throw new ArgumentNullException("The given third edgepixel must not be null!");

            #endregion

            #region Math Checks

            if (EdgePixel1.Equals(EdgePixel2) ||
                EdgePixel1.Equals(EdgePixel3) ||
                EdgePixel2.Equals(EdgePixel3))
                throw new ArgumentException("All distances between the pixels must be larger than zero!");

            if (EdgePixel1.Longitude.Value.Equals(EdgePixel2.Longitude.Value) &&
                EdgePixel2.Longitude.Value.Equals(EdgePixel3.Longitude.Value))
                throw new ArgumentException("All three pixels must not be on a single line!");

            if (EdgePixel1.Latitude.Value.Equals(EdgePixel2.Latitude.Value) &&
                EdgePixel2.Latitude.Value.Equals(EdgePixel3.Latitude.Value))
                throw new ArgumentException("All three pixels must not be on a single line!");

            #endregion

            var _Line12     = new GeoLine(EdgePixel1, EdgePixel2);
            var _Line23     = new GeoLine(EdgePixel2, EdgePixel3);

            var Center      = new GeoLine(_Line12.Center, _Line12.Normale).
                                  Intersection(
                              new GeoLine(_Line23.Center, _Line23.Normale));

            return Center.DistanceTo(Pixel) <= Center.DistanceTo(EdgePixel1);

        }

        #endregion


        #region Contains(GeoCoordinate)

        /// <summary>
        /// Checks if the given x- and y-coordinates are
        /// located within this circle.
        /// </summary>
        /// <param name="GeoCoordinate">The geo coordinate.</param>
        /// <returns>True if the coordinates are located within this circle; False otherwise.</returns>
        public Boolean Contains(GeoCoordinate GeoCoordinate)
        {

            #region Initial Checks

            if (GeoCoordinate == null)
                throw new ArgumentNullException("The given x-coordinate must not be null!");

            #endregion

            if (Center.DistanceTo(GeoCoordinate).IsLessThanOrEquals(Radius))
                return true;

            return false;

        }

        #endregion

        #region Contains(Circle)

        /// <summary>
        /// Checks if the given circle is located
        /// within this circle.
        /// </summary>
        /// <param name="Circle">A circle of type T.</param>
        /// <returns>True if the circle is located within this circle; False otherwise.</returns>
        public Boolean Contains(GeoCircle Circle)
        {

            #region Initial Checks

            if (Circle == null)
                throw new ArgumentNullException("The given circle must not be null!");

            #endregion

            if (Center.DistanceTo(Circle.Center) <= Radius - Circle.Radius)
                return true;

            return true;

        }

        #endregion

        #region Overlaps(Circle)

        /// <summary>
        /// Checks if the given circle shares some
        /// area with this circle.
        /// </summary>
        /// <param name="Circle">A circle of type T.</param>
        /// <returns>True if the circle shares some area with this circle; False otherwise.</returns>
        public Boolean Overlaps(GeoCircle Circle)
        {

            #region Initial Checks

            if (Circle == null)
                throw new ArgumentNullException("The given circle must not be null!");

            #endregion

            if (Center.DistanceTo(Circle.Center) <= Radius + Circle.Radius)
                return true;

            return true;

        }

        #endregion


        #region Operator overloadings

        #region Operator == (Circle1, Circle2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Circle1">A Circle&lt;T&gt;.</param>
        /// <param name="Circle2">Another Circle&lt;T&gt;.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GeoCircle Circle1, GeoCircle Circle2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Circle1, Circle2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Circle1 == null) || ((Object) Circle2 == null))
                return false;

            return Circle1.Equals(Circle2);

        }

        #endregion

        #region Operator != (Circle1, Circle2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Circle1">A Circle&lt;T&gt;.</param>
        /// <param name="Circle2">Another Circle&lt;T&gt;.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (GeoCircle Circle1, GeoCircle Circle2)
        {
            return !(Circle1 == Circle2);
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

            // Check if the given object is an GeoCircle.
            var CircleT = (GeoCircle) Object;
            if ((Object) CircleT == null)
                return false;

            return this.Equals(CircleT);

        }

        #endregion

        #region Equals(ICircle)

        /// <summary>
        /// Compares two circles for equality.
        /// </summary>
        /// <param name="ICircle">A circle to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(GeoCircle Circle)
        {

            if ((Object) Circle == null)
                return false;

            return this.Center.Equals(Circle.Center) &&
                   this.Radius.Equals(Circle.Radius);

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
            return this.Center.GetHashCode() ^ 1 + Radius.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Format("GeoCircle: Center={0}, Radius={1}",
                                 Center.ToString(),
                                 Radius.ToString());
        }

        #endregion

    }

}
